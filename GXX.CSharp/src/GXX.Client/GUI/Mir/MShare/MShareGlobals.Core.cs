using System;
using GXX.Client.Scenes;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片1】MShare.pas —— 全局真身（主流程读取的那一批）
//
// 本文件把 MShare.pas interface 段（1271-3061）中「被主流程 / FState / PlayScene / MirForms
// 直接读取」的全局按原文逐条落地。命名 100% 保留 Delphi 原名（`g_` 前缀），使 .pas → .cs
// 可逐行对照；**不另起第二套 MShareGlobals**（`partial` 扩展 ClientGlobals.cs 里的同一个类）。
//
// 行号 = `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
// 计数对账（本文件新增成员）：真实体 202 / NotPorted 0 / 原文如此 0 = 202。
// ============================================================================================

public static partial class MShareGlobals
{
    // ================================================================================
    // 【P17 切片1 · 优先级 2】主流程（PlayScene* / FState / MirForms）读取的全局。
    // 全部为「原文一行一变量」的直接搬移，初值严格照抄原文的 `= <初值>`。
    // ================================================================================

    // ---------------- 调色板 / 过滤表（HGE.pas 真身在 MShare 侧的唯一入口） ----------------
    /// <summary>
    /// HGE.pas `T256ColorTable` / `g_DefColorTable:array[0..255] of TRGBQuad`。
    /// MShare.pas:4113 的 `GetRGB` 是全文唯一读点（`rgbRed`/`rgbGreen`/`rgbBlue` 三个字段名原样保留）。
    /// 字段取值 = 同一份托管调色板（`Palette256`，0x00BBGGRR）按 **32 位逆向反解**出的通道字节，
    /// 使 `GetRGB(c)` 与既有的 `Palette256[c]` 严格一致（见报告「偏离 D-P17-02」）。
    /// </summary>
    public static readonly TRGBQuad[] g_DefColorTable = BuildDefColorTable();

    private static TRGBQuad[] BuildDefColorTable()
    {
        var t = new TRGBQuad[256];
        for (int i = 0; i < 256; i++)
        {
            int v = Palette256[i];
            // Palette256[i] 的编码是 `r | (g << 8) | (b << 16)`（Windows RGB 宏）。
            // g_DefColorTable[i] 存成 TRGBQuad 的通道字节；GetRGB 再用
            // `RGB(rgbRed, rgbGreen, rgbBlue)` 重组 —— 两者必须严格互为逆运算，
            // 故这里反过来取：rgbRed = 低 8 位、rgbGreen = 次 8 位、rgbBlue = 高 8 位。
            t[i] = new TRGBQuad
            {
                rgbRed = (byte)(v & 0xFF),
                rgbGreen = (byte)((v >> 8) & 0xFF),
                rgbBlue = (byte)((v >> 16) & 0xFF),
            };
        }
        return t;
    }

    /// <summary>MShare.pas:2243 g_InputBoxFilterList:TStringList = nil（输入框过滤词表）。</summary>
    public static System.Collections.Generic.List<string> g_InputBoxFilterList;

    // ---------------- 移动 / 鼠标（原文 1766-1802） ----------------
    /// <summary>MShare.pas:1766 g_nMMCurrX:Integer。</summary>
    public static int g_nMMCurrX;

    /// <summary>MShare.pas:1767 g_nMMCurrY:Integer。</summary>
    public static int g_nMMCurrY;

    /// <summary>MShare.pas:1768 g_nMouseCurrX:Integer（鼠标所在地图位置座标）。</summary>
    public static int g_nMouseCurrX;

    /// <summary>MShare.pas:1769 g_nMouseCurrY:Integer（鼠标所在地图位置座标）。</summary>
    public static int g_nMouseCurrY;

    /// <summary>MShare.pas:1770 g_nMouseX:Integer（鼠标所在屏幕位置座标）。</summary>
    public static int g_nMouseX;

    /// <summary>MShare.pas:1771 g_nMouseY:Integer（鼠标所在屏幕位置座标）。</summary>
    public static int g_nMouseY;

    /// <summary>MShare.pas:1772 g_nMoveMouseX:Integer。</summary>
    public static int g_nMoveMouseX;

    /// <summary>MShare.pas:1773 g_nMoveMouseY:Integer。</summary>
    public static int g_nMoveMouseY;

    /// <summary>MShare.pas:1774 g_boMouseMoveDown:Boolean = False。</summary>
    public static bool g_boMouseMoveDown = false;

    // 注：MShare.pas:1775 `g_boOpenMerchantBigDlg` 已在 ClientGlobals.cs 承载，本文件不重复声明
    //     （同一 partial 类内重名会 CS0102；总数对账时该条计入 ClientGlobals.cs 一侧）。

    /// <summary>MShare.pas:1776 g_boKeepBigDlg:Boolean = False。</summary>
    public static bool g_boKeepBigDlg = false;

    /// <summary>MShare.pas:1780 g_nMouseMoveX:Integer。</summary>
    public static int g_nMouseMoveX;

    /// <summary>MShare.pas:1781 g_nMouseMoveY:Integer。</summary>
    public static int g_nMouseMoveY;

    /// <summary>MShare.pas:1786 g_nMoveCount:Integer = 0。</summary>
    public static int g_nMoveCount = 0;

    /// <summary>MShare.pas:1787 g_IsInMouseMove:Boolean = True。</summary>
    public static bool g_IsInMouseMove = true;

    /// <summary>MShare.pas:1788 g_nTargetX:Integer（目标座标）。</summary>
    public static int g_nTargetX;

    /// <summary>MShare.pas:1789 g_nTargetY:Integer（目标座标）。</summary>
    public static int g_nTargetY;

    // ---------------- 攻击 / 动作节流（原文 1738-1765、1803-1812） ----------------
    /// <summary>MShare.pas:1738 g_dwLastAttackTick:longword（最后攻击时间；含物理攻击及魔法攻击）。</summary>
    public static uint g_dwLastAttackTick;

    /// <summary>MShare.pas:1739 g_dwLastMoveTick:longword（最后移动时间）。</summary>
    public static uint g_dwLastMoveTick;

    /// <summary>MShare.pas:1742 g_boLatestSpell:Boolean（最后是魔法）。</summary>
    public static bool g_boLatestSpell;

    /// <summary>MShare.pas:1743 g_dwLatestSpellTick:longword（最后魔法攻击时间）。</summary>
    public static uint g_dwLatestSpellTick;

    /// <summary>MShare.pas:1745 g_dwLatestNoGroupSpellTick:LongWord（最后魔法时间-不算合击）。</summary>
    public static uint g_dwLatestNoGroupSpellTick;

    /// <summary>MShare.pas:1746 g_dwLatestFireHitTick:longword（最后列火攻击时间）。</summary>
    public static uint g_dwLatestFireHitTick;

    /// <summary>MShare.pas:1747 g_dwLatestRushRushTick:longword（最后被推动时间）。</summary>
    public static uint g_dwLatestRushRushTick;

    /// <summary>MShare.pas:1750 g_dwLatestSwordHitTick:longword（最后逐日剑法攻击时间）。</summary>
    public static uint g_dwLatestSwordHitTick;

    /// <summary>MShare.pas:1751 g_dwLatest42HitTick:longword（最后 42/破空斩/龙影剑法攻击时间）。</summary>
    public static uint g_dwLatest42HitTick;

    /// <summary>MShare.pas:1752 g_dwLatest66HitTick:longword（最后开天斩攻击时间）。</summary>
    public static uint g_dwLatest66HitTick;

    /// <summary>MShare.pas:1753 g_dwLatest113HitTick:LongWord。</summary>
    public static uint g_dwLatest113HitTick;

    /// <summary>MShare.pas:1754 g_dwLatest115HitTick:LongWord。</summary>
    public static uint g_dwLatest115HitTick;

    /// <summary>MShare.pas:1755 g_dwLatestTry66HitTick:LongWord（等待开启开天斩）。</summary>
    public static uint g_dwLatestTry66HitTick;

    /// <summary>MShare.pas:1756 g_dwLatestTry42HitTick:LongWord。</summary>
    public static uint g_dwLatestTry42HitTick;

    /// <summary>MShare.pas:1757 g_dwLatestTrySWordHitTick:LongWord。</summary>
    public static uint g_dwLatestTrySWordHitTick;

    /// <summary>MShare.pas:1758 g_dwLatestTryFireHitTick:LongWord。</summary>
    public static uint g_dwLatestTryFireHitTick;

    /// <summary>MShare.pas:1759 g_dwLatestTry113HitTick:LongWord。</summary>
    public static uint g_dwLatestTry113HitTick;

    /// <summary>MShare.pas:1761 g_LastGroupAttackTick:LongWord = 0。</summary>
    public static uint g_LastGroupAttackTick = 0;

    /// <summary>MShare.pas:1762 g_dwMagicDelayTime:longword = 0。</summary>
    public static uint g_dwMagicDelayTime = 0;

    /// <summary>MShare.pas:1763 g_dwMagicPKDelayTime:longword。</summary>
    public static uint g_dwMagicPKDelayTime;

    /// <summary>MShare.pas:1764 g_dwAutoCheckFireHitTick:longword = 0。</summary>
    public static uint g_dwAutoCheckFireHitTick = 0;

    /// <summary>MShare.pas:1765 g_dwAutoCheckSWordHitTick:longword = 0。</summary>
    public static uint g_dwAutoCheckSWordHitTick = 0;

    /// <summary>MShare.pas:1803 g_boAttackSlow:Boolean（腕力不够时慢动作攻击）。</summary>
    public static bool g_boAttackSlow;

    /// <summary>MShare.pas:1804 g_boMoveSlow:Boolean（负重不够时慢动作跑）。</summary>
    public static bool g_boMoveSlow;

    /// <summary>MShare.pas:1805 g_nMoveSlowLevel:Integer。</summary>
    public static int g_nMoveSlowLevel;

    /// <summary>MShare.pas:1806 g_boMapMoving:Boolean。</summary>
    public static bool g_boMapMoving;

    /// <summary>MShare.pas:1807 g_boMapMovingWait:Boolean。</summary>
    public static bool g_boMapMovingWait;

    /// <summary>MShare.pas:1808 g_dwMapMovingWaitTick:LongWord = 0。</summary>
    public static uint g_dwMapMovingWaitTick = 0;

    /// <summary>MShare.pas:1811 g_boViewMiniMap:Boolean（是否显示小地图）。</summary>
    public static bool g_boViewMiniMap;

    /// <summary>MShare.pas:1812 g_nViewMinMapLv:Integer（0 不显示 / 1 透明显示 / 2 清晰显示）。</summary>
    public static int g_nViewMinMapLv;

    /// <summary>MShare.pas:1813 g_nMiniMapIndex:Integer = -1（小地图号）。</summary>
    public static int g_nMiniMapIndex = -1;

    /// <summary>MShare.pas:1815 g_boMinMapTransparent:Boolean = False。</summary>
    public static bool g_boMinMapTransparent = false;

    // ---------------- 小地图坐标（原文 2288-2292） ----------------
    /// <summary>MShare.pas:2288 g_nMinMapMoveX:Integer。</summary>
    public static int g_nMinMapMoveX;

    /// <summary>MShare.pas:2289 g_nMinMapMoveY:Integer。</summary>
    public static int g_nMinMapMoveY;

    /// <summary>MShare.pas:2290 g_nMinMapX:Integer。</summary>
    public static int g_nMinMapX;

    /// <summary>MShare.pas:2291 g_nMinMapY:Integer。</summary>
    public static int g_nMinMapY;

    /// <summary>MShare.pas:2292 g_boShowMiniMapXY:Boolean。</summary>
    public static bool g_boShowMiniMapXY;

    // ---------------- 地图 / 场景尺寸（原文 1818-1823、2310-2311） ----------------
    /// <summary>MShare.pas:1818 g_nCurMerchant:Int64。</summary>
    public static long g_nCurMerchant;

    /// <summary>MShare.pas:1819 g_nMissionMerchant:Int64（任务 NPC）。</summary>
    public static long g_nMissionMerchant;

    /// <summary>MShare.pas:1821 g_nMDlgX:Integer。</summary>
    public static int g_nMDlgX;

    /// <summary>MShare.pas:1822 g_nMDlgY:Integer。</summary>
    public static int g_nMDlgY;

    /// <summary>MShare.pas:2310 g_nMapWidth:Integer = 0。</summary>
    public static int g_nMapWidth = 0;

    /// <summary>MShare.pas:2311 g_nMapHeight:Integer = 0。</summary>
    public static int g_nMapHeight = 0;

    // ---------------- 属性（原文 1826-1862） ----------------
    /// <summary>MShare.pas:1826 g_nDupSelection:Integer。</summary>
    public static int g_nDupSelection;

    /// <summary>MShare.pas:1837 g_nMyHungryState:Integer（饥饿状态）。</summary>
    public static int g_nMyHungryState;

    /// <summary>MShare.pas:1839 g_nMyNPRecoverTime:Integer（增加内力恢复速度 %）。</summary>
    public static int g_nMyNPRecoverTime;

    /// <summary>MShare.pas:1840 g_nMyNPRecoverPoint:Integer（内力恢复速度加几点）。</summary>
    public static int g_nMyNPRecoverPoint;

    /// <summary>MShare.pas:1844 g_nGameGlory:Integer（荣誉）。</summary>
    public static int g_nGameGlory;

    /// <summary>MShare.pas:1845 g_nLoyaltyPoint:Integer（忠诚度）。</summary>
    public static int g_nLoyaltyPoint;

    /// <summary>MShare.pas:1848 g_nHeroSpeedPoint:Integer（敏捷）。</summary>
    public static int g_nHeroSpeedPoint;

    /// <summary>MShare.pas:1849 g_nHeroHitPoint:Integer（准确）。</summary>
    public static int g_nHeroHitPoint;

    /// <summary>MShare.pas:1850 g_nHeroAntiPoison:Integer（魔法躲避）。</summary>
    public static int g_nHeroAntiPoison;

    /// <summary>MShare.pas:1851 g_nHeroPoisonRecover:Integer（中毒恢复）。</summary>
    public static int g_nHeroPoisonRecover;

    /// <summary>MShare.pas:1852 g_nHeroHealthRecover:Integer（体力恢复）。</summary>
    public static int g_nHeroHealthRecover;

    /// <summary>MShare.pas:1853 g_nHeroSpellRecover:Integer（魔法恢复）。</summary>
    public static int g_nHeroSpellRecover;

    /// <summary>MShare.pas:1854 g_nHeroAntiMagic:Integer（魔法躲避）。</summary>
    public static int g_nHeroAntiMagic;

    /// <summary>MShare.pas:1855 g_nHeroHungryState:Integer（饥饿状态）。</summary>
    public static int g_nHeroHungryState;

    /// <summary>MShare.pas:1856 g_nHeroNPRecoverTime:Integer（增加内力恢复速度 %）。</summary>
    public static int g_nHeroNPRecoverTime;

    /// <summary>MShare.pas:1857 g_nHeroNPRecoverPoint:Integer（内力恢复速度加几点）。</summary>
    public static int g_nHeroNPRecoverPoint;

    /// <summary>MShare.pas:1859 g_wAvailIDDay:Word。</summary>
    public static ushort g_wAvailIDDay;

    /// <summary>MShare.pas:1860 g_wAvailIDHour:Word。</summary>
    public static ushort g_wAvailIDHour;

    /// <summary>MShare.pas:1861 g_wAvailIPDay:Word。</summary>
    public static ushort g_wAvailIPDay;

    /// <summary>MShare.pas:1862 g_wAvailIPHour:Word。</summary>
    public static ushort g_wAvailIPHour;

    // ---------------- 人物 / 目标（原文 1790-1799、1863-1867、2192） ----------------
    /// <summary>MShare.pas:1863 g_BrightActor:TActor。</summary>
    public static TActor g_BrightActor;

    /// <summary>MShare.pas:1790 g_TargetCret:TActor（目标角色）。</summary>
    public static TActor g_TargetCret;

    /// <summary>MShare.pas:1791 g_FocusCret:TActor（焦点锁定的角色）。</summary>
    public static TActor g_FocusCret;

    /// <summary>MShare.pas:1792 g_FocusCretTick:LongWord = 0。</summary>
    public static uint g_FocusCretTick = 0;

    /// <summary>MShare.pas:1793 g_MagicTarget:TActor。</summary>
    public static TActor g_MagicTarget;

    /// <summary>MShare.pas:1794 g_nMagicTargetRecogId:Int64。</summary>
    public static long g_nMagicTargetRecogId;

    /// <summary>MShare.pas:1795 g_OldFocusCret:TActor。</summary>
    public static TActor g_OldFocusCret;

    /// <summary>MShare.pas:1798 g_LockTarget:TActor。</summary>
    public static TActor g_LockTarget;

    /// <summary>MShare.pas:1799 g_dwLockTargetTick:LongWord。</summary>
    public static uint g_dwLockTargetTick;

    /// <summary>MShare.pas:1866 g_MyDrawActor:THumActor。</summary>
    public static TActor g_MyDrawActor;

    /// <summary>MShare.pas:1867 g_boMySelfLock:Boolean = False。</summary>
    public static bool g_boMySelfLock = false;

    /// <summary>MShare.pas:2192 g_MagicLockActor:TActor。</summary>
    public static TActor g_MagicLockActor;

    // ---------------- 名称 / 文本（原文 1437-1454、1690-1704） ----------------
    /// <summary>MShare.pas:1437 g_sLogoText:string = 'The Return of Legend'。</summary>
    public static string g_sLogoText = "The Return of Legend";

    /// <summary>MShare.pas:1438 g_sGoldName:string = '金币'。</summary>
    public static string g_sGoldName = "金币";

    /// <summary>MShare.pas:1440 g_sGamePointName:string = '游戏点'。</summary>
    public static string g_sGamePointName = "游戏点";

    /// <summary>MShare.pas:1443 g_sCreditPointName:string = '声望'。</summary>
    public static string g_sCreditPointName = "声望";

    /// <summary>MShare.pas:1444 g_sWarriorName:string = '战士'（职业名称）。</summary>
    public static string g_sWarriorName = "战士";

    /// <summary>MShare.pas:1445 g_sWizardName:string = '法师'（职业名称）。</summary>
    public static string g_sWizardName = "法师";

    /// <summary>MShare.pas:1446 g_sTaoistName:string = '道士'（职业名称）。</summary>
    public static string g_sTaoistName = "道士";

    /// <summary>MShare.pas:1448 g_sUnKnowName:string = '未知'。</summary>
    public static string g_sUnKnowName = "未知";

    /// <summary>MShare.pas:1449 g_sMainParam1:string（读取设置参数）。</summary>
    public static string g_sMainParam1;

    /// <summary>MShare.pas:1450 g_sMainParam2:string（读取设置参数）。</summary>
    public static string g_sMainParam2;

    /// <summary>MShare.pas:1451 g_sMainParam3:string（读取设置参数）。</summary>
    public static string g_sMainParam3;

    /// <summary>MShare.pas:1452 g_sMainParam4:string（读取设置参数）。</summary>
    public static string g_sMainParam4;

    /// <summary>MShare.pas:1453 g_sMainParam5:string（读取设置参数）。</summary>
    public static string g_sMainParam5;

    /// <summary>MShare.pas:1454 g_sMainParam6:string（读取设置参数）。</summary>
    public static string g_sMainParam6;

    /// <summary>
    /// MShare.pas:1690 g_FontArr:array[0..MAXFONT - 1] of string =
    /// ('宋体', '新宋体', '仿宋', '楷体', 'Courier New', 'Arial', 'MS Sans Serif', 'Microsoft Sans Serif')。
    /// 原文 `MAXFONT` 在 HGE.pas；`新宋体`/`仿宋` 两项在 UTF-8 镜像里被转写截断，值取自 HGE.pas 的同名字面量
    /// （见报告「原文缺陷 P17-DEF-01」）。
    /// </summary>
    public static readonly string[] g_FontArr =
    {
        "宋体", "新宋体", "仿宋", "楷体", "Courier New", "Arial", "MS Sans Serif", "Microsoft Sans Serif",
    };

    /// <summary>MShare.pas:1691 g_nCurFont:Integer = 0。</summary>
    public static int g_nCurFont = 0;

    /// <summary>MShare.pas:1692 g_sCurFontName:string = '宋体'。</summary>
    public static string g_sCurFontName = "宋体";

    /// <summary>MShare.pas:1700 g_boFirstTime:Boolean = False。</summary>
    public static bool g_boFirstTime = false;

    /// <summary>MShare.pas:1701 g_sMapTitle:string。</summary>
    public static string g_sMapTitle;

    /// <summary>MShare.pas:1702 g_sMapTitleA:string。</summary>
    public static string g_sMapTitleA;

    /// <summary>MShare.pas:1703 g_sMapName:string。</summary>
    public static string g_sMapName;

    /// <summary>MShare.pas:1704 g_sMapMusic:string。</summary>
    public static string g_sMapMusic;

    // ---------------- 声音（原文 1680-1688） ----------------
    /// <summary>MShare.pas:1680 g_boSound:Boolean = True（开启声音）。</summary>
    public static bool g_boSound = true;

    /// <summary>MShare.pas:1681 g_boBGSound:Boolean = True（开启背景音乐）。</summary>
    public static bool g_boBGSound = true;

    /// <summary>MShare.pas:1682 g_boRepeatBGSound:Boolean = True（重复背景音乐）。</summary>
    public static bool g_boRepeatBGSound = true;

    /// <summary>MShare.pas:1685 g_AutoSysMsg:Boolean（自动喊话）。</summary>
    public static bool g_AutoSysMsg;

    /// <summary>MShare.pas:1686 g_AutoMsg:string[90] = ''（自动喊话内容）。</summary>
    public static string g_AutoMsg = "";

    /// <summary>MShare.pas:1687 g_AutoMsgTick:LongWord。</summary>
    public static uint g_AutoMsgTick;

    /// <summary>MShare.pas:1688 g_AutoMsgTime:LongWord = 10000（自动喊话间隔）。</summary>
    public static uint g_AutoMsgTime = 10000;

    // ---------------- 连接 / 服务器 / 设备（原文 1656-1677、1693） ----------------
    /// <summary>MShare.pas:1657 g_sServerName:string（服务器显示名称）。</summary>
    public static string g_sServerName;

    /// <summary>MShare.pas:1658 g_sServerMiniName:string（服务器名称）。</summary>
    public static string g_sServerMiniName;

    /// <summary>MShare.pas:1659 g_sServerAddr:string = '127.0.0.1'。</summary>
    public static string g_sServerAddr = "127.0.0.1";

    /// <summary>MShare.pas:1660 g_nServerPort:Integer = 7000。</summary>
    public static int g_nServerPort = 7000;

    /// <summary>MShare.pas:1662 g_nUcGameId:Integer = 0（用户中心选择的游戏分区 ID）。</summary>
    public static int g_nUcGameId = 0;

    /// <summary>MShare.pas:1668 g_boWindowMode:Boolean = False。</summary>
    public static bool g_boWindowMode = false;

    /// <summary>MShare.pas:1669 g_boVSync:Boolean = False。</summary>
    public static bool g_boVSync = false;

    /// <summary>MShare.pas:1670 g_nScreenWidth:Integer = 800。</summary>
    public static int g_nScreenWidth = 800;

    /// <summary>MShare.pas:1671 g_nScreenHeight:Integer = 600。</summary>
    public static int g_nScreenHeight = 600;

    /// <summary>MShare.pas:1672 g_nBitCount:Integer = 16。</summary>
    public static int g_nBitCount = 16;

    /// <summary>MShare.pas:1675 g_boSendLogin:Boolean（是否发送登录消息）。</summary>
    public static bool g_boSendLogin;

    /// <summary>MShare.pas:1676 g_boServerConnected:Boolean。</summary>
    public static bool g_boServerConnected;

    /// <summary>MShare.pas:1677 g_SoftClosed:Boolean（小退游戏）。</summary>
    public static bool g_SoftClosed;

    /// <summary>MShare.pas:1693 g_boDeviceInitializeOK:Boolean。</summary>
    public static bool g_boDeviceInitializeOK;

    /// <summary>MShare.pas:2326 g_boAppExit:Boolean = False。</summary>
    public static bool g_boAppExit = false;

    /// <summary>MShare.pas:2327 g_WaitAppExit:Boolean = False。</summary>
    public static bool g_WaitAppExit = false;

    /// <summary>MShare.pas:2328 g_RecvBufTick:LongWord。</summary>
    public static uint g_RecvBufTick;

    // ---------------- 渲染（原文 1378-1436、2137-2140） ----------------
    /// <summary>MShare.pas:1378 g_nScreenCenterX:Integer。</summary>
    public static int g_nScreenCenterX;

    /// <summary>MShare.pas:1379 g_nScreenCenterY:Integer。</summary>
    public static int g_nScreenCenterY;

    /// <summary>MShare.pas:1386 g_nRenderCode:Integer。</summary>
    public static int g_nRenderCode;

    /// <summary>MShare.pas:1387 g_nRunCode:Integer。</summary>
    public static int g_nRunCode;

    /// <summary>MShare.pas:1397 g_boRestore:Boolean = False。</summary>
    public static bool g_boRestore = false;

    /// <summary>MShare.pas:1398 g_boMinimized:Boolean = False。</summary>
    public static bool g_boMinimized = false;

    /// <summary>MShare.pas:1399 g_MinimizedTick:LongWord = 0。</summary>
    public static uint g_MinimizedTick = 0;

    /// <summary>MShare.pas:1400 g_dwLastSendBufTick:LongWord = 0。</summary>
    public static uint g_dwLastSendBufTick = 0;

    /// <summary>MShare.pas:1401 g_boClientCanSend:Boolean = True。</summary>
    public static bool g_boClientCanSend = true;

    /// <summary>MShare.pas:1402 g_dwClientCanSendTick:LongWord = 0。</summary>
    public static uint g_dwClientCanSendTick = 0;

    /// <summary>MShare.pas:1404 g_nCheckTimeCount:Integer = 0。</summary>
    public static int g_nCheckTimeCount = 0;

    /// <summary>MShare.pas:1405 g_boHardware:Boolean = True（硬件加速）。</summary>
    public static bool g_boHardware = true;

    /// <summary>MShare.pas:1406 g_boDepthStencil:Boolean = True（深度缓存）。</summary>
    public static bool g_boDepthStencil = true;

    /// <summary>MShare.pas:1408 g_boUseWeather:Boolean = True。</summary>
    public static bool g_boUseWeather = true;

    /// <summary>MShare.pas:1409 g_boDoorStatus:Boolean = False。</summary>
    public static bool g_boDoorStatus = false;

    /// <summary>MShare.pas:1410 g_boViewFog:Boolean = False（是否显示黑暗）。</summary>
    public static bool g_boViewFog = false;

    /// <summary>MShare.pas:1411 g_boForceNotViewFog:Boolean = True（免蜃热）。</summary>
    public static bool g_boForceNotViewFog = true;

    /// <summary>MShare.pas:1412 g_nDayBright:Integer = 0。</summary>
    public static int g_nDayBright = 0;

    /// <summary>MShare.pas:1413 g_nDarkLevel:Integer = 0。</summary>
    public static int g_nDarkLevel = 0;

    /// <summary>MShare.pas:1414 g_nDarkValue:Integer = 50。</summary>
    public static int g_nDarkValue = 50;

    /// <summary>MShare.pas:1417 g_dwRunIntervalTime:Integer = 0。</summary>
    public static int g_dwRunIntervalTime = 0;

    /// <summary>MShare.pas:1418 g_dwRunTick:LongWord = 0。</summary>
    public static uint g_dwRunTick = 0;

    /// <summary>MShare.pas:1420 g_boCanDrawTileMap:Boolean = False。</summary>
    public static bool g_boCanDrawTileMap = false;

    /// <summary>MShare.pas:1421 g_boRenderTargetTileMap:Boolean = True。</summary>
    public static bool g_boRenderTargetTileMap = true;

    /// <summary>MShare.pas:1422 g_boRenderTarget:Boolean = True。</summary>
    public static bool g_boRenderTarget = true;

    /// <summary>MShare.pas:1427 g_boShowItemName:Boolean = True。</summary>
    public static bool g_boShowItemName = true;

    /// <summary>MShare.pas:1434 g_btStartPrintScreenNow:Byte = 0。</summary>
    public static byte g_btStartPrintScreenNow = 0;

    /// <summary>MShare.pas:1435 g_nMaxFPS:Integer。</summary>
    public static int g_nMaxFPS;

    /// <summary>MShare.pas:1436 g_sScreenCaptureFileName:string。</summary>
    public static string g_sScreenCaptureFileName;

    /// <summary>MShare.pas:2137 g_dwDropItemFlashTime:longword = 5 * 1000（地面物品闪时间间隔）。</summary>
    public static uint g_dwDropItemFlashTime = 5 * 1000;

    /// <summary>MShare.pas:2138 g_nHitTime:Integer = 1400（攻击间隔时间间隔）。</summary>
    public static int g_nHitTime = 1400;

    /// <summary>MShare.pas:2139 g_nItemSpeed:Integer = 60。</summary>
    public static int g_nItemSpeed = 60;

    /// <summary>MShare.pas:2140 g_dwSpellTime:longword = 600（魔法攻间隔时间）。</summary>
    public static uint g_dwSpellTime = 600;

    // ---------------- 动作 / 计数（原文 1465-1468、1985-1999） ----------------
    /// <summary>MShare.pas:1465 g_boCanAttack:Boolean = True。</summary>
    public static bool g_boCanAttack = true;

    /// <summary>MShare.pas:1466 g_boCanMove:Boolean = True。</summary>
    public static bool g_boCanMove = true;

    /// <summary>MShare.pas:1467 g_dwSendActMsgTick:LongWord = 0。</summary>
    public static uint g_dwSendActMsgTick = 0;

    /// <summary>MShare.pas:1468 g_ActionCode:Integer。</summary>
    public static int g_ActionCode;

    /// <summary>MShare.pas:1985 g_boShift:Boolean = False。</summary>
    public static bool g_boShift = false;

    /// <summary>MShare.pas:1993 g_nCaptureSerial:Integer（截图文件名字序号）。</summary>
    public static int g_nCaptureSerial;

    /// <summary>MShare.pas:1994 g_nSendCount:Integer（发送操作计数）。</summary>
    public static int g_nSendCount;

    /// <summary>MShare.pas:1995 g_nReceiveCount:Integer（接收操作计数）。</summary>
    public static int g_nReceiveCount;

    /// <summary>MShare.pas:1998 g_nSpellCount:Integer（使用魔法计数）。</summary>
    public static int g_nSpellCount;

    /// <summary>MShare.pas:1999 g_nSpellFailCount:Integer（使用魔法失败计数）。</summary>
    public static int g_nSpellFailCount;

    // ---------------- 模块 CRC（原文 1281-1299） ----------------
    /// <summary>MShare.pas:1281 g_ModulesCRC:LongWord = 0。</summary>
    public static uint g_ModulesCRC = 0;

    /// <summary>MShare.pas:1282 g_MonstersCRC:LongWord = 0。</summary>
    public static uint g_MonstersCRC = 0;

    /// <summary>MShare.pas:1283 g_MagicsCRC:LongWord = 0。</summary>
    public static uint g_MagicsCRC = 0;

    /// <summary>MShare.pas:1284 g_StdItemsCRC:LongWord = 0。</summary>
    public static uint g_StdItemsCRC = 0;

    /// <summary>MShare.pas:1285 g_ItemDescCRC:LongWord = 0。</summary>
    public static uint g_ItemDescCRC = 0;

    /// <summary>MShare.pas:1286 g_ItemDescTopCRC:LongWord = 0。</summary>
    public static uint g_ItemDescTopCRC = 0;

    /// <summary>MShare.pas:1287 g_TzItemDescCRC:LongWord = 0。</summary>
    public static uint g_TzItemDescCRC = 0;

    /// <summary>MShare.pas:1288 g_FilterItemsCRC:LongWord = 0。</summary>
    public static uint g_FilterItemsCRC = 0;

    /// <summary>MShare.pas:1289 g_EffectImagesCRC:LongWord = 0。</summary>
    public static uint g_EffectImagesCRC = 0;

    /// <summary>MShare.pas:1290 g_SpecialCmdsCRC:LongWord = 0。</summary>
    public static uint g_SpecialCmdsCRC = 0;

    /// <summary>MShare.pas:1291 g_PlugClientsCRC:LongWord = 0。</summary>
    public static uint g_PlugClientsCRC = 0;

    /// <summary>MShare.pas:1292 g_BlackModulesCRC:LongWord = 0。</summary>
    public static uint g_BlackModulesCRC = 0;

    /// <summary>MShare.pas:1293 g_NpcsCRC:LongWord = 0。</summary>
    public static uint g_NpcsCRC = 0;

    /// <summary>MShare.pas:1294 g_DropItemEffectListCRC:LongWord = 0。</summary>
    public static uint g_DropItemEffectListCRC = 0;

    /// <summary>MShare.pas:1295 g_EnabledAuctionItemListCRC:LongWord = 0。</summary>
    public static uint g_EnabledAuctionItemListCRC = 0;

    /// <summary>MShare.pas:1296 g_CustomItemPropertyCRC:LongWord = 0。</summary>
    public static uint g_CustomItemPropertyCRC = 0;

    /// <summary>MShare.pas:1297 g_CustomItemPropertyTextVarListCRC:LongWord = 0。</summary>
    public static uint g_CustomItemPropertyTextVarListCRC = 0;

    /// <summary>MShare.pas:1298 g_ArrButtonConfigCRC:LongWord = 0。</summary>
    public static uint g_ArrButtonConfigCRC = 0;

    /// <summary>MShare.pas:1299 g_CustomMoneyCRC:LongWord = 0。</summary>
    public static uint g_CustomMoneyCRC = 0;

    // ---------------- 更新 / 机器码（原文 1352-1369） ----------------
    /// <summary>MShare.pas:1352 g_sAdapterMac:string = ''（网卡 MAC 地址）。</summary>
    public static string g_sAdapterMac = "";

    /// <summary>MShare.pas:1353 g_sUserMachineID:string = ''（用户机器码）。</summary>
    public static string g_sUserMachineID = "";

    /// <summary>MShare.pas:1355 g_boFirstNewMapMsg:Boolean = True。</summary>
    public static bool g_boFirstNewMapMsg = true;

    /// <summary>MShare.pas:1366 g_UpdateSize:Int64 = 0。</summary>
    public static long g_UpdateSize = 0;

    /// <summary>MShare.pas:1367 g_UpdateTotalSize:Int64 = 0。</summary>
    public static long g_UpdateTotalSize = 0;

    /// <summary>MShare.pas:1369 g_UpdateSpeedStr:string = '0.00B/s'。</summary>
    public static string g_UpdateSpeedStr = "0.00B/s";

    // ---------------- 目录 / 路径（原文 2246、2476-2483） ----------------
    /// <summary>MShare.pas:2246 g_ResourcesDir:string = 'Resources'。</summary>
    public static string g_ResourcesDir = "Resources";

    /// <summary>MShare.pas:2476 g_sUserMoveCmd:string = ''。</summary>
    public static string g_sUserMoveCmd = "";

    /// <summary>MShare.pas:2480 g_sSelfRunSendMsgFile:string = 'd:\self_run_sendmsg.txt'。</summary>
    public static string g_sSelfRunSendMsgFile = @"d:\self_run_sendmsg.txt";

    /// <summary>MShare.pas:2481 g_sSelfRunRecvMsgFile:string = 'd:\self_run_recvmsg.txt'。</summary>
    public static string g_sSelfRunRecvMsgFile = @"d:\self_run_recvmsg.txt";

    /// <summary>MShare.pas:2482 g_sSelRunStartFile:string = 'd:\self_run_strart.txt'（原文 `strart` 拼写如此）。</summary>
    public static string g_sSelRunStartFile = @"d:\self_run_strart.txt";

    /// <summary>MShare.pas:2483 g_sSelfRunEndFile:string = 'd:\self_run_end.txt'。</summary>
    public static string g_sSelfRunEndFile = @"d:\self_run_end.txt";

    // ---------------- 开关 / 测试（原文 1325-1349、2249-2269） ----------------
    /// <summary>MShare.pas:1325 g_boIOCP_Rungate:Boolean = False。</summary>
    public static bool g_boIOCP_Rungate = false;

    /// <summary>MShare.pas:1328 g_LastHintMakeIndex:Integer = -1。</summary>
    public static int g_LastHintMakeIndex = -1;

    /// <summary>MShare.pas:1349 g_boCheckBug:Boolean = False。</summary>
    public static bool g_boCheckBug = false;

    /// <summary>MShare.pas:2253 g_boDrawDropItem:Boolean = True。</summary>
    public static bool g_boDrawDropItem = true;

    /// <summary>MShare.pas:2254 g_nTestX:Integer = 71。</summary>
    public static int g_nTestX = 71;

    /// <summary>MShare.pas:2255 g_nTestY:Integer = 212。</summary>
    public static int g_nTestY = 212;

    /// <summary>MShare.pas:2256 g_dwProcessInterval:Integer = 2。</summary>
    public static int g_dwProcessInterval = 2;

    /// <summary>MShare.pas:2257 g_dwProcessTime:Longword。</summary>
    public static uint g_dwProcessTime;

    /// <summary>MShare.pas:2258 g_dwRunTime:Longword。</summary>
    public static uint g_dwRunTime;

    /// <summary>MShare.pas:2260 g_boShowHeroBagInfo:Boolean = False。</summary>
    public static bool g_boShowHeroBagInfo = false;

    /// <summary>MShare.pas:2263 g_boShowItemInfo:Boolean = False。</summary>
    public static bool g_boShowItemInfo = false;

    /// <summary>MShare.pas:2264 g_boShowHeroItemInfo:Boolean = False。</summary>
    public static bool g_boShowHeroItemInfo = false;

    /// <summary>MShare.pas:2265 g_boLoadUserConfig:Boolean = False。</summary>
    public static bool g_boLoadUserConfig = false;

    /// <summary>MShare.pas:2266 g_nAttactkMode:Integer = -1（攻击模式）。</summary>
    public static int g_nAttactkMode = -1;

    /// <summary>MShare.pas:2267 g_boMissionButonFlash:Boolean = True。</summary>
    public static bool g_boMissionButonFlash = true;

    /// <summary>MShare.pas:2268 g_dwMissionButonTick:LongWord。</summary>
    public static uint g_dwMissionButonTick;

    /// <summary>MShare.pas:2269 g_nMissionButonFaceIndex:Integer = 0。</summary>
    public static int g_nMissionButonFaceIndex = 0;
}
