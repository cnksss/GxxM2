using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// 本文件 = uFrm* 窗体族的**共享接缝层**（非编译器要求，纯工程约定）。
//
// 为什么需要它：`Source\RunGate\GateShare.pas` 尚未移植，而本车道的 12 个配置窗体全部依赖它
// 的全局量（g_Config / g_sIniFileName / g_ProcessBlackList / g_WordFilterList ...）与若干类型
// （TAntiPlugActionMode / TAntiPlugConfig / TColorIndexEdit / TSpinEditEx / TProcessBlacklist）。
// 父 agent 规定本车道只能新建 `uFrm*.cs`，故把接缝集中在本文件，各窗体文件只写自己的窗体。
//
// 保真原则：
//   * 结构体/记录 → 托管 class（可空引用语义与原 record 的可写字段一致）；
//   * 枚举成员名、顺序、整数值 **逐条照抄**（INI 里存的是整数，顺序错了配置就串了）；
//   * 字符串表与默认配置 **逐字照抄**（含全角括号、尾随空格、'%s'/'%d' 占位符）。
//
// 已登记偏差：
//   D1. `TVirtualStringTree`（VirtualTrees.pas，第三方控件）未移植 → uFrmGameSpeed/uFrmMagicCD
//       用 WinForms `TreeView`（OwnerDraw + 子 `ListView` 列替代）承载，节点数据放在
//       `TreeNode.Tag` 而不是 GetNodeData。列宽/列头从 .dfm 逐条对齐。
//   D2. `TColorIndexEdit`（ColorIndexEdit.pas）未移植 → 本文件给出最小同构实现
//       （索引 ⇄ TColor，走 ColorIndexToTColor）。
//   D3. `TSpinEditEx`（SpinEditEx.pas）→ 与 GXX.DBServer.SpinControls.cs 同构（命名空间不同，无冲突）；
//       GXX.RunGate 不能引用 GXX.DBServer，故此处独立复刻。
// =====================================================================================

#region GateShare.pas 枚举与记录（照抄）

/// <summary>
/// GateShare.pas:241-255 `TAntiPlugActionMode = (amHit, amSpell, ...)`。
/// **成员顺序即整数值**，INI 里按 `AntiPlugActionModeSections[Mode]` 存节名，
/// 而 `g_Config.ActionList` 是按模式索引的数组，顺序错则配置串位。
/// 末尾 `amAllConcurrent` 在原文被 `(* ... *)` 注释掉，此处同样不定义。
/// </summary>
public enum TAntiPlugActionMode
{
    amHit = 0,              // 攻击
    amSpell = 1,            // 魔法
    amWalk = 2,             // 走路
    amRun = 3,              // 跑步
    amTurn = 4,             // 转向
    amCutMeat = 5,          // 挖肉
    amWalkToHit = 6,        // 走路到攻击
    amHitToWalk = 7,        // 攻击到走路
    amRunToHit = 8,         // 跑步到攻击
    amHitToRun = 9,         // 攻击到跑步
    amWalkToSpell = 10,     // 走路到魔法
    amSpellToWalk = 11,     // 魔法到走路
    amRunToSpell = 12,      // 跑步到魔法
    amSpellToRun = 13,      // 魔法到跑步
    amTurnToHit = 14,       // 转向到攻击
    amHitToTurn = 15,       // 攻击到转向
    amTurnToSpell = 16,     // 转向到魔法
    amSpellToTurn = 17,     // 魔法到转向
    amCutMeatToHit = 18,    // 挖肉到攻击
    amCutMeatToSpell = 19,  // 挖肉到魔法
    amMoveToTurn = 20,      // 移动到转向
    amTurnToMove = 21,      // 转向到移动
    amMoveToCutMeat = 22,   // 移动到挖肉
    amCutMeatToMove = 23,   // 挖肉到移动
    amHitConcurrent = 24,   // 攻击并发
    amSpellConcurrent = 25, // 魔法并发
    amMoveConcurrent = 26   // 移动并发
}

/// <summary>GateShare.pas:220 `TActionProcessMode`（顺序 = 整数值）。</summary>
public enum TActionProcessMode
{
    apmDelay = 0,           // 延时处理 停顿
    apmRebound = 1,         // 反弹卡刀
    apmLost = 2,            // 丢弃封包 卡位
    apmOffline = 3,         // 掉线处理
    apmFakeAttackPass = 4,  // 假刀放行
    apmNoProcess = 5        // 不处理
}

/// <summary>GateShare.pas:221 `TSumActionProcessMode = (sapmNone {不处理}, sampOffline {掉线}, sampLockUser {锁定})`。
/// 注意原文第二个成员拼写为 `sampOffline`（samp 而非 sapm）—— 照抄，不做"统一"。</summary>
public enum TSumActionProcessMode
{
    sapmNone = 0,
    sampOffline = 1,
    sampLockUser = 2
}

/// <summary>GateShare.pas:217 `TBlockIPMethod = (bmDisconnect, bmTempBlock, bmBlockList)`。</summary>
public enum TBlockIPMethod
{
    bmDisconnect = 0,
    bmTempBlock = 1,
    bmBlockList = 2
}

/// <summary>GateShare.pas:218 `TFilterSayMsgMode`。</summary>
public enum TFilterSayMsgMode
{
    fsmmAllBlock = 0,
    fsmmSelfBolck = 1,
    fsmmClose = 2,
    fsmmDisMsg = 3,
    fsmmDisMsgorSys = 4
}

/// <summary>GateShare.pas:224-237 `TAntiPlugAction`（record → class）。</summary>
public class TAntiPlugAction
{
    public bool boEnabled;
    public uint nInterval;          // DWORD
    public TActionProcessMode ProcessMode;
    public bool boProcessScript;
    public TSumActionProcessMode SumProcessMode;
    public bool boShowHint;
    public string sHintText = "";
    public int nCompensationValue;
    public bool boDebug;

    public TAntiPlugAction Clone() => (TAntiPlugAction)MemberwiseClone();
}

/// <summary>GateShare.pas:260-306 `TAntiPlugConfig`（record → class）。</summary>
public class TAntiPlugConfig
{
    /// <summary>`ActionList: array[TAntiPlugActionMode] of TAntiPlugAction`（按枚举整数索引）。</summary>
    public TAntiPlugAction[] ActionList;

    public byte btMsgType;
    public byte btMsgFColor;
    public byte btMsgBColor;

    public int nLockTime;
    public bool boSaveLockStatus;
    public bool boShowLockLog;
    public string sShowLockMsg = "";

    public bool boSpeedClearData;

    public uint dwUserShop_Search_Interval;
    public bool boUserShop_Search_ShowHint;

    public uint dwUserShop_Buy_Interval;
    public bool boUserShop_Buy_ShowHint;

    public uint dwTakeOn_Item_Interval;
    public bool boTakeOn_Item_ShowHint;

    public uint dwDealTry_Attack_Interval;
    public bool boDealTry_Attack_ShowHint;

    public uint dwBrutal_Attack_Interval;
    public bool boBrutal_Attack_ShowHint;

    public bool boShowAttackLog;

    public uint dwContinueSpeedPassIncTime;

    public bool boContinueSpeedCloseSocket;
    public int nContinueSpeedCount;

    public int nSumSpeedCheckTime;
    public int nSumSpeedMaxCount;

    public int dwCollectCount;
    public int dwSpeedValue;

    public bool boZeroCompensationValueClearPool;

    public ushort dwClientUploadPickItemsTime;   // Word

    public TAntiPlugConfig()
    {
        ActionList = new TAntiPlugAction[RunGateConst.ActionModeCount];
        for (int i = 0; i < ActionList.Length; i++) ActionList[i] = new TAntiPlugAction();
    }

    public TAntiPlugConfig Clone()
    {
        var c = (TAntiPlugConfig)MemberwiseClone();
        c.ActionList = new TAntiPlugAction[ActionList.Length];
        for (int i = 0; i < ActionList.Length; i++) c.ActionList[i] = ActionList[i].Clone();
        return c;
    }
}

/// <summary>GateShare.pas:360-363 `TEatItemCDConfig`（Hum/Hero 各 3 项 TItemCDTime）。</summary>
public class TEatItemCDConfig
{
    public const int SlotCount = 3;
    public TItemCDTime[] Hum = new TItemCDTime[SlotCount];
    public TItemCDTime[] Hero = new TItemCDTime[SlotCount];

    public TEatItemCDConfig()
    {
        for (int i = 0; i < SlotCount; i++) { Hum[i] = new TItemCDTime(); Hero[i] = new TItemCDTime(); }
    }
}

/// <summary>GateShare.pas `TItemCDTime`（7 个毫秒 CD）。</summary>
public class TItemCDTime
{
    public int NormalHP;
    public int NormalMP;
    public int NormalHPMP;
    public int SpecialHP;
    public int SpecialMP;
    public int SpecialHPMP;
    public int Other;
}

#endregion

/// <summary>GateShare.pas 中本窗体族用到的常量与字符串表。</summary>
public static class RunGateConst
{
    /// <summary>GateShare.pas:25-26 `HALF_SPEED_INTERVALS_COUNT = 200; SPEED_INTERVALS_COUNT = HALF*2+1`。</summary>
    public const int HalfSpeedIntervalsCount = 200;
    public const int SpeedIntervalsCount = HalfSpeedIntervalsCount * 2 + 1;   // 401

    /// <summary>GateShare.pas:448 `GateClass: string = 'GameGate';`。</summary>
    public const string GateClass = "GameGate";

    /// <summary>`High(TAntiPlugActionMode) + 1`。</summary>
    public const int ActionModeCount = 27;

    /// <summary>uFrmGameSpeed.pas:12 `WM_STARTEDITING = WM_USER + 778`。</summary>
    public const int WmStartEditingGameSpeed = 0x0400 + 778;

    /// <summary>uFrmMagicCD.pas:11 `WM_STARTEDITING = WM_USER + 779`。</summary>
    public const int WmStartEditingMagicCD = 0x0400 + 779;

    /// <summary>GateShare.pas:1338-1344 的封包类型位掩码（uFrmLogClientPacketSetting 使用）。
    /// ★ 注意 `CPT_OTHER` 在原文中**不存在**：DFM 的 chkLogOther 恒勾选恒禁用，且代码从不读它。</summary>
    public const int CptMove = 1;      // GateShare.pas:1338 CPT_MOVE  = 1
    public const int CptHit = 2;       // GateShare.pas:1339 CPT_HIT   = 2
    public const int CptSpell = 4;     // GateShare.pas:1340 CPT_SPELL = 4
    public const int CptQuery = 8;     // GateShare.pas:1341 CPT_QUERY = 8
    public const int CptTeam = 16;     // GateShare.pas:1342 CPT_TEAM  = 16
    public const int CptGuild = 32;    // GateShare.pas:1343 CPT_GUILD = 32
    public const int CptShop = 64;     // GateShare.pas:1344 CPT_SHOP  = 64

    /// <summary>GateShare.pas:464-465 `ActionProcessModeNames`（**逐字照抄，含中文**）。</summary>
    public static readonly string[] ActionProcessModeNames =
    {
        "停顿操作", "反弹卡刀", "卡位操作", "掉线处理", "假刀放行", "不做处理"
    };

    /// <summary>GateShare.pas:467-468 `ActionProcessModeNames2`（仅 apmLost 一项与上表不同：「卡位操作」→「丢弃封包」）。</summary>
    public static readonly string[] ActionProcessModeNames2 =
    {
        "停顿操作", "反弹卡刀", "卡位操作", "掉线处理", "丢弃封包", "不做处理"
    };

    /// <summary>GateShare.pas:470-471 `SumActionProcessModeNames`。</summary>
    public static readonly string[] SumActionProcessModeNames = { "不处理", "掉线操作", "锁定用户" };

    /// <summary>GateShare.pas:473-486 `AntiPlugActionModeNames`（注意前 6 条带两个尾随空格）。</summary>
    public static readonly string[] AntiPlugActionModeNames =
    {
        "攻击间隔  ", "魔法间隔  ", "走路间隔  ",
        "跑步间隔  ", "转向间隔  ", "挖肉间隔  ",
        "走路→攻击", "攻击→走路",
        "跑步→攻击", "攻击→跑步",
        "走路→魔法", "魔法→走路",
        "跑步→魔法", "魔法→跑步",
        "转向→攻击", "攻击→转向",
        "转向→魔法", "魔法→转向",
        "挖肉→攻击", "挖肉→魔法",
        "移动→转向", "转向→移动",
        "移动→挖肉", "挖肉→移动",
        "攻击并发数", "魔法并发数",
        "移动并发数"
    };

    /// <summary>GateShare.pas:488-501 `AntiPlugActionModeNames_2`（无尾随空格版）。</summary>
    public static readonly string[] AntiPlugActionModeNames2 =
    {
        "攻击", "魔法", "走路",
        "跑步", "转向", "挖肉",
        "走路→攻击", "攻击→走路",
        "跑步→攻击", "攻击→跑步",
        "走路→魔法", "魔法→走路",
        "跑步→魔法", "魔法→跑步",
        "转向→攻击", "攻击→转向",
        "转向→魔法", "魔法→转向",
        "挖肉→攻击", "挖肉→魔法",
        "移动→转向", "转向→移动",
        "移动→挖肉", "挖肉→移动",
        "攻击并发", "魔法并发",
        "移动并发"
    };

    /// <summary>GateShare.pas:503-516 `AntiPlugActionModeNames_3`。</summary>
    public static readonly string[] AntiPlugActionModeNames3 =
    {
        "攻击间隔", "魔法间隔", "走路间隔",
        "跑步间隔", "转向间隔", "挖肉间隔",
        "走路→攻击", "攻击→走路",
        "跑步→攻击", "攻击→跑步",
        "走路→魔法", "魔法→走路",
        "跑步→魔法", "魔法→跑步",
        "转向→攻击", "攻击→转向",
        "转向→魔法", "魔法→转向",
        "挖肉→攻击", "挖肉→魔法",
        "移动→转向", "转向→移动",
        "移动→挖肉", "挖肉→移动",
        "攻击并发数", "魔法并发数",
        "移动并发数"
    };

    /// <summary>GateShare.pas:518-532 `AntiPlugActionModeSections`（INI 节名）。</summary>
    public static readonly string[] AntiPlugActionModeSections =
    {
        "Hit", "Spell", "Walk",
        "Run", "Turn", "CutMeat",
        "WalkToHit", "HitToWalk",
        "RunToHit", "HitToRun",
        "WalkToSpell", "SpellToWalk",
        "RunToSpell", "SpellToRun",
        "TurnToHit", "HitToTurn",
        "TurnToSpell", "SpellToTurn",
        "CutMeatToHit", "CutMeatToSpell",
        "MoveToTurn", "TurnToMove",
        "MoveToCutMeat", "CutMeatToMove",
        "HitConcurrent", "SpellConcurrent",
        "MoveConcurrent"
    };
}

/// <summary>GateShare.pas 中本窗体族用到的全局量（**可写**，测试直接改）。</summary>
public static class FormGlobals
{
    /// <summary>GateShare.pas:1073 `g_sIniFileName: string;`（由 RunGate 启动时赋值）。</summary>
    public static string g_sIniFileName = "";

    /// <summary>GateShare.pas:1105 `g_wActionSpeedIntervals: array[TAntiPlugActionMode] of TSpeedIntervals`。</summary>
    public static readonly ushort[][] g_wActionSpeedIntervals = NewSpeedIntervals();

    /// <summary>GateShare.pas:1107 `g_boSendSpeedIntervalsToClient`。</summary>
    public static readonly bool[] g_boSendSpeedIntervalsToClient = new bool[RunGateConst.ActionModeCount];

    /// <summary>GateShare.pas:1075 `g_sActionIntervalsFileNames`（默认空串 → 不落盘）。</summary>
    public static readonly string[] g_sActionIntervalsFileNames = new string[RunGateConst.ActionModeCount];

    /// <summary>GateShare.pas:1321 `g_MagicCDListFileName`。</summary>
    public static string g_MagicCDListFileName = "";

    /// <summary>GateShare.pas:1322 `g_MagicCDList: TMagicIntervalList`。</summary>
    public static readonly TMagicIntervalList g_MagicCDList = new TMagicIntervalList();

    /// <summary>GateShare.pas:1327-1332 MagicCD 全局量（内联初值逐字照抄）。</summary>
    public static byte g_btMagicCDMsgType = 0;
    public static string g_sMagicCDMsgText = "技能尚未冷确，请等待%time秒";
    public static byte g_btMagicCDFColor = 0xFF;
    public static byte g_btMagicCDBColor = 0x38;
    public static int g_nMagicCDShowX = 30;
    public static int g_nMagicCDShowY = 40;

    /// <summary>GateShare.pas:1334 `g_EatItemCDConfig: TEatItemCDConfig;`（FillChar 0 初始化 → 全 0）。</summary>
    public static readonly TEatItemCDConfig g_EatItemCDConfig = new TEatItemCDConfig();

    /// <summary>GateShare.pas:1121 `g_WordFilterList: TSafeHashStringList;`（TStrings 语义：Text/Add/Clear/SaveToFile）。</summary>
    public static readonly TSafeHashStringList g_WordFilterList = new TSafeHashStringList();

    /// <summary>GateShare.pas `g_sWordFilterFileName`。</summary>
    public static string g_sWordFilterFileName = "";

    /// <summary>GateShare.pas:1241-1242 消息过滤全局量。</summary>
    public static bool g_boFilterSayMsg = false;
    public static TFilterSayMsgMode g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;
    public static string g_WarnSayMsg = "您发送的信息里包含了非法字符。";
    public static bool g_boFilterSayTriggerScript = false;

    /// <summary>GateShare.pas:1136 `g_ProcessBlackList: TProcessBlacklist;`。</summary>
    public static readonly TProcessBlackList g_ProcessBlackList = new TProcessBlackList();

    /// <summary>GateShare.pas:1137-1138。</summary>
    public static string g_ProcessBlacklistStr = "";
    public static byte[] g_ProcessBlacklistMD5 = new byte[16];

    /// <summary>GateShare.pas:1059 `nSumSpeedMaxCount`（INI 里 MaxClientMsgCount 写的是这个"看似同名实则不同"的常量）。</summary>
    public const int nMaxClientMsgCount = 5;

    /// <summary>GateShare.pas:1256 `g_dwSayMaxCount: LongWord = 2;`。</summary>
    public static uint g_dwSayMaxCount = 2;

    // ---------------- 安全过滤（uFrmSafeFilter）用到的全局量 ----------------
    /// <summary>GateShare.pas:1317 `g_OnlyWhiteListLink: Boolean = False;`。</summary>
    public static bool g_OnlyWhiteListLink = false;

    // 以下 26 个来自 GateShare.pas 的同族全局量（uFrmSafeFilter 的 Open/btnOKClick 读写）。
    // 原文初值（GateShare.pas `var` 段）：由 LoadConfig 从 Config.ini 读入，未命中键时保持 0/False。
    public static int g_nMaxConnOfIPaddr = 0;
    public static TBlockIPMethod g_BlockMethod = TBlockIPMethod.bmDisconnect;
    public static uint g_dwAttackTick = 0;
    public static int g_nAttackCount = 0;
    public static int g_nMaxClientPacketSize = 0;
    public static int g_nMaxClientPacketCount = 0;
    public static bool g_boKickOverPacketSize = false;
    public static uint g_dwKeepConnectTimeOut = 0;
    public static bool g_boCheckClientPacketLegal = false;
    public static int g_nCheckClientPacketCount = 0;
    public static bool g_boSayMsgControl = false;
    public static uint g_dwSayMaxLen = 0;
    public static uint g_dwSayTime = 0;
    public static uint g_dwSayDisableTime = 0;
    public static uint g_dwIPCountLimitTime1 = 0;
    public static uint g_dwIPCountLimit1 = 0;
    public static uint g_dwIPCountLimitTime2 = 0;
    public static uint g_dwIPCountLimit2 = 0;
    public static uint g_dwDefenseLevel = 0;
    public static bool g_boDefenseToLevel1 = false;
    public static uint g_dwDefenseToLevel1 = 0;
    public static bool g_boResotreDefense = false;
    public static uint g_dwResotreDefense = 0;
    public static bool g_boAutoClearTemp = false;
    public static uint g_dwAutoClearTemp = 0;
    public static bool g_boAddAllToTemp = false;
    public static uint g_dwAddAllToTemp = 0;
    public static bool g_boOpenCheckClient = false;
    public static TBlockIPMethod g_CheckClientFailBlockMethod = TBlockIPMethod.bmTempBlock;   // DFM 的 cbb ItemIndex=1

    // ---------------- uFrmReadFileIP 的 12 个全局量（GateShare.pas 同族） ----------------
    public static string g_sFYReadDenyIPFile = "";
    public static uint g_dwFYReadDenyIPTime = 0;
    public static string g_sFYDownDenyIPUrl = "";
    public static uint g_dwFYDownDenyIPTime = 0;
    public static string g_sFYReadPassIPFile = "";
    public static uint g_dwFYReadPassIPTime = 0;
    public static string g_sFYDownPassIPUrl = "";
    public static uint g_dwFYDownPassIPTime = 0;
    public static string g_sFYReadDenyMACFile = "";
    public static uint g_dwFYReadDenyMACTime = 0;
    public static string g_sFYDownDenyMACUrl = "";
    public static uint g_dwFYDownDenyMACTime = 0;

    // ---------------- uFrmAntiPlugUpdateSetting 的 3 个全局量 ----------------
    public static bool g_boAntiPlugAutoUpdateCheck = false;
    public static int g_wAntiPlugUpdateCheckInterval = 1;
    public static string g_sAntiPlugUpdateConfigUrl = "";

    // ---------------- uFrmHitInterval 的全局量 ----------------
    public static string g_sHitIntervalsFileName = "";

    /// <summary>GateShare.pas `g_dwHitIntervals: array of LongWord`（长度决定表格行数）。</summary>
    public static uint[] g_dwHitIntervals = new uint[0];

    // ---------------- uFrmLogClientPacketSetting 全局量 ----------------
    public static int g_nLogClientPacketType = 0;
    public static bool g_boLogClientPacket = false;
    public static string g_sLogClientPakcetUserFile = "";
    public static readonly TSafeHashStringList g_LogClientPacketUser = new TSafeHashStringList();

    // ---------------- uFrmInterval 依赖的"按模式是否支持加速间隔" ----------------
    /// <summary>GateShare.pas:1351-1365 `function ActionModeUseSpeedIntervals(ActionMode): Boolean`。
    /// 注意 amTurn / amCutMeat / amHitToTurn / amSpellToTurn / amMoveToTurn / amMoveToCutMeat 被注释掉 → False。</summary>
    public static bool ActionModeUseSpeedIntervals(TAntiPlugActionMode actionMode)
    {
        switch (actionMode)
        {
            case TAntiPlugActionMode.amHit:
            case TAntiPlugActionMode.amSpell:
            case TAntiPlugActionMode.amWalk:
            case TAntiPlugActionMode.amRun:
            case TAntiPlugActionMode.amWalkToHit:
            case TAntiPlugActionMode.amHitToWalk:
            case TAntiPlugActionMode.amRunToHit:
            case TAntiPlugActionMode.amHitToRun:
            case TAntiPlugActionMode.amWalkToSpell:
            case TAntiPlugActionMode.amSpellToWalk:
            case TAntiPlugActionMode.amRunToSpell:
            case TAntiPlugActionMode.amSpellToRun:
            case TAntiPlugActionMode.amTurnToHit:
            case TAntiPlugActionMode.amTurnToSpell:
            case TAntiPlugActionMode.amCutMeatToHit:
            case TAntiPlugActionMode.amCutMeatToSpell:
            case TAntiPlugActionMode.amTurnToMove:
            case TAntiPlugActionMode.amCutMeatToMove:
                return true;
            default:
                return false;
        }
    }

    /// <summary>GateShare.pas:367 `procedure RebuildSendToClientSpeedIntervalsText;`
    /// 的纯逻辑部分：把 `g_boSendSpeedIntervalsToClient` + 各模式 `g_wActionSpeedIntervals`
    /// 拼成发送给客户端的字节缓冲（原文 81920 字节栈缓冲 + Move 逐段拷）。
    /// 原文尾部还会把它塞进全局消息，此处只重算缓冲，返回字节数组以便单测。</summary>
    public static byte[] RebuildSendToClientSpeedIntervalsText()
    {
        // 原文：Move(g_boSendSpeedIntervalsToClient, PB^, SizeOf(...)) → BufSize := SizeOf(...)
        // Boolean 数组在 Delphi 中每项 1 字节。
        var buffer = new List<byte>(RunGateConst.ActionModeCount * (1 + RunGateConst.SpeedIntervalsCount * 2));
        for (int i = 0; i < RunGateConst.ActionModeCount; i++)
            buffer.Add(g_boSendSpeedIntervalsToClient[i] ? (byte)1 : (byte)0);

        for (int m = 0; m < RunGateConst.ActionModeCount; m++)
        {
            if (!g_boSendSpeedIntervalsToClient[m]) continue;   // 原 :1380
            for (int i = 0; i < RunGateConst.SpeedIntervalsCount; i++)
            {
                ushort v = g_wActionSpeedIntervals[m][i];       // Word → 小端 2 字节
                buffer.Add((byte)(v & 0xFF));
                buffer.Add((byte)((v >> 8) & 0xFF));
            }
        }
        return buffer.ToArray();
    }

    /// <summary>GateShare.pas:632-1066 `g_Config: TAntiPlugConfig = (...)` 的**逐字段初值**（1:1 照抄）。
    /// 本窗体族只用到写侧，读取侧（LoadConfig）属 RunGate 主窗体车道，此处仅提供初值。</summary>
    public static readonly TAntiPlugConfig g_Config = CreateDefaultConfig();

    /// <summary>GateShare.pas:626 `g_DefaultConfig: TAntiPlugConfig;`（FillChar 0 初始化 → 全 0/False/
    /// 空串，仅 ActionList 的 ProcessMode 为 apmDelay=0、SumProcessMode 为 sapmNone=0）。</summary>
    public static readonly TAntiPlugConfig g_DefaultConfig = new TAntiPlugConfig();

    /// <summary>GateShare.pas:632-1066 的默认配置构造（ActionList 27 项 + 标量字段）。</summary>
    public static TAntiPlugConfig CreateDefaultConfig()
    {
        var c = new TAntiPlugConfig();

        // ---- ActionList（GateShare.pas:634-1021，顺序 = TAntiPlugActionMode）----
        // 每项 9 个字段：boEnabled / nInterval / ProcessMode / boProcessScript / SumProcessMode /
        //               boShowHint / sHintText / nCompensationValue / boDebug
        // 大部分条目字段完全一致（boEnabled=False, boProcessScript=False, SumProcessMode=sapmNone,
        // boShowHint=False, nCompensationValue=0, boDebug=False），只有 nInterval / ProcessMode / sHintText 不同。
        // 用 FillAction 显式给出差异项，聚合字段统一按原文取值（原 :635-646 等）。
        void Set(int mode, uint interval, TActionProcessMode processMode, string hintText)
        {
            var a = c.ActionList[mode];
            a.boEnabled = false;                    // 原 :635 / :649 / ...
            a.nInterval = interval;
            a.ProcessMode = processMode;
            a.boProcessScript = false;              // 原 :638 / :652 / ...
            a.SumProcessMode = TSumActionProcessMode.sapmNone;   // 原 :639 / :653 / ...
            a.boShowHint = false;                   // 原 :643 / :657 / ...
            a.sHintText = hintText;
            a.nCompensationValue = 0;               // 原 :645 / :659 / ...
            a.boDebug = false;                      // 原 :646 / :660 / ...
        }

        Set(0, 0, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【攻击】速度出现异常");           // 原 :634-647
        Set(1, 1200, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【魔法】速度出现异常");        // 原 :648-661
        Set(2, 540, TActionProcessMode.apmRebound, "[提示]: 您的【走路】速度出现异常");                // 原 :662-675
        Set(3, 540, TActionProcessMode.apmRebound, "[提示]: 您的【跑步】速度出现异常");                // 原 :676-689
        Set(4, 100, TActionProcessMode.apmRebound, "[提示]: 您的【转向】速度出现异常");                // 原 :690-703
        Set(5, 620, TActionProcessMode.apmRebound, "[提示]: 您的【挖肉】速度出现异常");                // 原 :704-717
        Set(6, 540, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【走动攻击】速度出现异常");      // 原 :718-731
        Set(7, 600, TActionProcessMode.apmRebound, "[提示]: 您的【攻击走动】速度出现异常");            // 原 :732-745
        Set(8, 540, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【跑动攻击】速度出现异常");      // 原 :746-759
        Set(9, 600, TActionProcessMode.apmRebound, "[提示]: 您的【攻击跑动】速度出现异常");            // 原 :760-773
        Set(10, 540, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【走动魔法】速度出现异常");     // 原 :774-787
        Set(11, 1200, TActionProcessMode.apmRebound, "[提示]: 您的【魔法走动】速度出现异常");          // 原 :788-801
        Set(12, 540, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【跑动魔法】速度出现异常");     // 原 :802-815
        Set(13, 1200, TActionProcessMode.apmRebound, "[提示]: 您的【魔法跑动】速度出现异常");          // 原 :816-829
        Set(14, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :830-843 转向到攻击
        Set(15, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :844-857 攻击到转向
        Set(16, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :858-871 转向到魔法
        Set(17, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :872-885 魔法到转向
        Set(18, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :886-899 挖肉到攻击
        Set(19, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :900-913 挖肉到魔法
        Set(20, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :914-927 移动到转向
        Set(21, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :928-941 转向到移动
        Set(22, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :942-955 移动到挖肉
        Set(23, 250, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常");                  // 原 :956-969 挖肉到移动
        // 原 :970-983 / :984-997 / :998-1010：三个并发项 nInterval=1、ProcessMode=apmFakeAttackPass、
        // sHintText 不同（没有「您的…速度出现异常」前缀）
        Set(24, 1, TActionProcessMode.apmFakeAttackPass, "[提示]: 请爱护游戏环境，关闭加速外挂重新登陆");
        Set(25, 1, TActionProcessMode.apmFakeAttackPass, "[提示]: 请爱护游戏环境，关闭加速外挂重新登陆");
        Set(26, 1, TActionProcessMode.apmFakeAttackPass, "[提示]: 请爱护游戏环境，关闭加速外挂重新登陆");

        // ---- 标量字段（GateShare.pas:1024-1065）----
        c.btMsgType = 0;                                        // 原 :1024
        c.btMsgFColor = 0xFF;                                   // 原 :1025 btMsgFColor: $FF
        c.btMsgBColor = 0x38;                                   // 原 :1026 btMsgBColor: $38
        c.nLockTime = 5;                                        // 原 :1028
        c.boSaveLockStatus = true;                              // 原 :1029
        c.boShowLockLog = false;                                // 原 :1030
        c.sShowLockMsg = "超速已经被锁定(原因：%s)，%d秒后自动解锁！";   // 原 :1031
        c.boSpeedClearData = false;                             // 原 :1033
        c.dwUserShop_Search_Interval = 200;                     // 原 :1035
        c.boUserShop_Search_ShowHint = true;                    // 原 :1036
        c.dwUserShop_Buy_Interval = 200;                        // 原 :1038
        c.boUserShop_Buy_ShowHint = true;                       // 原 :1039
        c.dwTakeOn_Item_Interval = 200;                         // 原 :1041
        c.boTakeOn_Item_ShowHint = true;                        // 原 :1042
        c.dwDealTry_Attack_Interval = 1500;                     // 原 :1044
        c.boDealTry_Attack_ShowHint = true;                     // 原 :1045
        c.dwBrutal_Attack_Interval = 700;                       // 原 :1047
        c.boBrutal_Attack_ShowHint = true;                      // 原 :1048
        c.boShowAttackLog = false;                              // 原 :1050
        c.dwContinueSpeedPassIncTime = 30;                      // 原 :1053
        c.boContinueSpeedCloseSocket = false;                   // 原 :1055
        c.nContinueSpeedCount = 4;                              // 原 :1056
        c.nSumSpeedCheckTime = 20;                              // 原 :1058
        c.nSumSpeedMaxCount = 5;                                // 原 :1059
        c.dwCollectCount = 15;                                  // 原 :1061
        c.dwSpeedValue = 9;                                     // 原 :1062
        c.boZeroCompensationValueClearPool = false;             // 原 :1064
        c.dwClientUploadPickItemsTime = 30;                     // 原 :1065
        return c;
    }

    /// <summary>GateShare.pas:3296 `TProcessBlacklist.FMaxCount := 80` 的初值（构造函数里设置）。
    /// 测试复位时必须一并恢复 —— 否则某个用例把 MaxCount 调小后，后续用例的"加满 80 项"会静默只加到 MaxCount 项。</summary>
    public const int ProcessBlackListDefaultMaxCount = 80;

    /// <summary>测试辅助：把全部全局量复位到 GateShare.pas 的初值。</summary>
    public static void ResetForTest()
    {
        // ---- GateShare.pas 单元级全局量（本窗体族读写的全部） ----
        ResetFormGlobalsForTest();
        // ---- 容器/数组型全局量 ----
        g_WordFilterList.Clear();
        g_LogClientPacketUser.Clear();
        g_ProcessBlackList.Clear();
        g_ProcessBlackList.MaxCount = ProcessBlackListDefaultMaxCount;   // ★ 复位 FMaxCount（原 :3271）
        g_MagicCDList.Clear();
        Array.Clear(g_boSendSpeedIntervalsToClient, 0, g_boSendSpeedIntervalsToClient.Length);
        for (int i = 0; i < g_sActionIntervalsFileNames.Length; i++) g_sActionIntervalsFileNames[i] = "";
        for (int m = 0; m < g_wActionSpeedIntervals.Length; m++)          // ★ 401×27 的速度间隔表
            Array.Clear(g_wActionSpeedIntervals[m], 0, g_wActionSpeedIntervals[m].Length);
        ResetConfigForTest();
    }

    /// <summary>`ResetForTest` 的标量部分（拆分仅为可读性）。</summary>
    private static void ResetFormGlobalsForTest()
    {
        // uFrmMessageFilter
        g_boFilterSayMsg = false;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;
        g_WarnSayMsg = "您发送的信息里包含了非法字符。";
        g_boFilterSayTriggerScript = false;

        // uFrmSafeFilter（GateShare.pas `g_*` 段；初值 0/False，GateClass 的
        // CheckClientFailBlockMethod 默认 = bmTempBlock，因为 .dfm 的 cbb ItemIndex=1）
        g_OnlyWhiteListLink = false;
        g_nMaxConnOfIPaddr = 0;
        g_BlockMethod = TBlockIPMethod.bmDisconnect;
        g_dwAttackTick = 0;
        g_nAttackCount = 0;
        g_nMaxClientPacketSize = 0;
        g_nMaxClientPacketCount = 0;
        g_boKickOverPacketSize = false;
        g_dwKeepConnectTimeOut = 0;
        g_boCheckClientPacketLegal = false;
        g_nCheckClientPacketCount = 0;
        g_boSayMsgControl = false;
        g_dwSayMaxLen = 0;
        g_dwSayTime = 0;
        g_dwSayMaxCount = 2;                       // GateShare.pas:1256 内联初值
        g_dwSayDisableTime = 0;
        g_dwIPCountLimitTime1 = 0;
        g_dwIPCountLimit1 = 0;
        g_dwIPCountLimitTime2 = 0;
        g_dwIPCountLimit2 = 0;
        g_dwDefenseLevel = 0;
        g_boDefenseToLevel1 = false;
        g_dwDefenseToLevel1 = 0;
        g_boResotreDefense = false;
        g_dwResotreDefense = 0;
        g_boAutoClearTemp = false;
        g_dwAutoClearTemp = 0;
        g_boAddAllToTemp = false;
        g_dwAddAllToTemp = 0;
        g_boOpenCheckClient = false;
        g_CheckClientFailBlockMethod = TBlockIPMethod.bmTempBlock;

        // uFrmMagicCD
        g_btMagicCDMsgType = 0;
        g_sMagicCDMsgText = "技能尚未冷确，请等待%time秒";
        g_btMagicCDFColor = 0xFF;
        g_btMagicCDBColor = 0x38;
        g_nMagicCDShowX = 30;
        g_nMagicCDShowY = 40;

        // uFrmLogClientPacketSetting / uFrmProcessBlacklist
        g_nLogClientPacketType = 0;
        g_boLogClientPacket = false;
        g_ProcessBlacklistStr = "";
        g_ProcessBlacklistMD5 = new byte[16];

        // 路径型（原文由 RunGate 启动时赋值；测试里显式清空以免串场）
        g_sIniFileName = "";
        g_sWordFilterFileName = "";
        g_sLogClientPakcetUserFile = "";
        g_MagicCDListFileName = "";
        g_sHitIntervalsFileName = "";

        // uFrmReadFileIP
        g_sFYReadDenyIPFile = ""; g_dwFYReadDenyIPTime = 0;
        g_sFYDownDenyIPUrl = ""; g_dwFYDownDenyIPTime = 0;
        g_sFYReadPassIPFile = ""; g_dwFYReadPassIPTime = 0;
        g_sFYDownPassIPUrl = ""; g_dwFYDownPassIPTime = 0;
        g_sFYReadDenyMACFile = ""; g_dwFYReadDenyMACTime = 0;
        g_sFYDownDenyMACUrl = ""; g_dwFYDownDenyMACTime = 0;

        // uFrmAntiPlugUpdateSetting
        g_boAntiPlugAutoUpdateCheck = false;
        g_wAntiPlugUpdateCheckInterval = 1;
        g_sAntiPlugUpdateConfigUrl = "";
    }

    /// <summary>测试辅助：`g_Config` 就地复位为 GateShare.pas:632-1066 的初值（保持同一实例）。</summary>
    public static void ResetConfigForTest()
    {
        var fresh = CreateDefaultConfig();
        var target = g_Config;
        for (int i = 0; i < target.ActionList.Length; i++)
        {
            var src = fresh.ActionList[i];
            var dst = target.ActionList[i];
            dst.boEnabled = src.boEnabled;
            dst.nInterval = src.nInterval;
            dst.ProcessMode = src.ProcessMode;
            dst.boProcessScript = src.boProcessScript;
            dst.SumProcessMode = src.SumProcessMode;
            dst.boShowHint = src.boShowHint;
            dst.sHintText = src.sHintText;
            dst.nCompensationValue = src.nCompensationValue;
            dst.boDebug = src.boDebug;
        }
        target.btMsgType = fresh.btMsgType;
        target.btMsgFColor = fresh.btMsgFColor;
        target.btMsgBColor = fresh.btMsgBColor;
        target.nLockTime = fresh.nLockTime;
        target.boSaveLockStatus = fresh.boSaveLockStatus;
        target.boShowLockLog = fresh.boShowLockLog;
        target.sShowLockMsg = fresh.sShowLockMsg;
        target.boSpeedClearData = fresh.boSpeedClearData;
        target.dwUserShop_Search_Interval = fresh.dwUserShop_Search_Interval;
        target.boUserShop_Search_ShowHint = fresh.boUserShop_Search_ShowHint;
        target.dwUserShop_Buy_Interval = fresh.dwUserShop_Buy_Interval;
        target.boUserShop_Buy_ShowHint = fresh.boUserShop_Buy_ShowHint;
        target.dwTakeOn_Item_Interval = fresh.dwTakeOn_Item_Interval;
        target.boTakeOn_Item_ShowHint = fresh.boTakeOn_Item_ShowHint;
        target.dwDealTry_Attack_Interval = fresh.dwDealTry_Attack_Interval;
        target.boDealTry_Attack_ShowHint = fresh.boDealTry_Attack_ShowHint;
        target.dwBrutal_Attack_Interval = fresh.dwBrutal_Attack_Interval;
        target.boBrutal_Attack_ShowHint = fresh.boBrutal_Attack_ShowHint;
        target.boShowAttackLog = fresh.boShowAttackLog;
        target.dwContinueSpeedPassIncTime = fresh.dwContinueSpeedPassIncTime;
        target.boContinueSpeedCloseSocket = fresh.boContinueSpeedCloseSocket;
        target.nContinueSpeedCount = fresh.nContinueSpeedCount;
        target.nSumSpeedCheckTime = fresh.nSumSpeedCheckTime;
        target.nSumSpeedMaxCount = fresh.nSumSpeedMaxCount;
        target.dwCollectCount = fresh.dwCollectCount;
        target.dwSpeedValue = fresh.dwSpeedValue;
        target.boZeroCompensationValueClearPool = fresh.boZeroCompensationValueClearPool;
        target.dwClientUploadPickItemsTime = fresh.dwClientUploadPickItemsTime;
    }

    private static ushort[][] NewSpeedIntervals()
    {
        var arr = new ushort[RunGateConst.ActionModeCount][];
        for (int i = 0; i < arr.Length; i++) arr[i] = new ushort[RunGateConst.SpeedIntervalsCount];
        return arr;
    }
}

#region 可注入接缝（不依赖 WinForms 的纯逻辑侧只通过本区交互）

/// <summary>消息框接缝：窗体**不在**纯逻辑里直接弹窗；纯逻辑只调用本接缝。</summary>
public static class MessageBoxSeam
{
    /// <summary>
    /// 是否允许弹出**真实**模态对话框。默认 true（生产行为）。
    /// 单元测试必须置 false —— 无头环境下 `ShowDialog` 会启动消息循环并挂住 testhost
    /// （本车道曾因此出现 10 分钟超时）。测试通过 xUnit 集合夹具统一关闭。
    /// </summary>
    public static bool UiEnabled = true;

    /// <summary>默认实现走 WinForms `MessageBox.Show`（与原 `Application.MessageBox` 语义对应）。</summary>
    public static Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult> Show =
        (text, caption, buttons, icon) =>
        {
            if (!UiEnabled) return DialogResult.OK;      // 非交互（测试）环境：不阻塞
            return MessageBox.Show(text, caption, buttons, icon);
        };

    /// <summary>`Application.MessageBox(..., MB_OK + MB_ICONINFORMATION)` 的等价封装。</summary>
    public static DialogResult ShowInformation(string text, string caption)
        => Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);

    /// <summary>`Application.MessageBox(..., MB_OK or MB_ICONERROR)`（`ErrMessage`）。</summary>
    public static DialogResult ShowError(string text, string caption)
        => Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

    /// <summary>`Application.MessageBox(..., MB_OKCANCEL + MB_ICONQUESTION) = IDOK`（`QuestionMessage`）。</summary>
    public static bool Ask(string text, string caption)
        => Show(text, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;

    /// <summary>Delphi `Dialogs.InputQuery(Caption, Prompt, var Value): Boolean` 的托管签名
    /// （`Func&lt;&gt;` 无法表达 `ref` 出参，故单独定义委托）。返回 false = 用户取消。</summary>
    public delegate bool InputQueryHandler(string caption, string prompt, ref string value);

    /// <summary>可注入的 InputQuery 实现（默认弹一个最小输入窗体，测试注入假实现断言分支）。</summary>
    public static InputQueryHandler InputQueryWithValue = DefaultInputQuery;

    private static bool DefaultInputQuery(string caption, string prompt, ref string value)
    {
        // 非交互（测试）环境：绝不 ShowDialog —— 否则会启动消息循环挂住 testhost。
        if (!UiEnabled) return false;

        using var form = new Form
        {
            Text = caption,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(320, 110),
            MaximizeBox = false,
            MinimizeBox = false
        };
        var label = new Label { Left = 10, Top = 12, Width = 300, Height = 20, Text = prompt };
        var edit = new TextBox { Left = 10, Top = 36, Width = 300, Text = value ?? "" };
        var ok = new Button { Left = 155, Top = 68, Width = 75, Height = 25, Text = "确定", DialogResult = DialogResult.OK };
        var cancel = new Button { Left = 235, Top = 68, Width = 75, Height = 25, Text = "取消", DialogResult = DialogResult.Cancel };
        form.Controls.AddRange(new Control[] { label, edit, ok, cancel });
        form.AcceptButton = ok;
        form.CancelButton = cancel;
        if (form.ShowDialog() != DialogResult.OK) return false;
        value = edit.Text;
        return true;
    }
}

/// <summary>接缝：待 GateShare.pas `TSafeHashStringList` 移植后接入。
/// 本窗体族用到的子集：Lock/UnLock/Clear/Count/Add/Delete/IndexOf/Text/SaveToFile/LoadFromFile。</summary>
public class TSafeHashStringList
{
    private readonly List<string> _items = new List<string>();

    // 原文用自旋锁保护；单线程窗体场景下保持可重入计数即可（不影响语义）。
    private int _lockDepth;

    public void Lock() => _lockDepth++;
    public void UnLock() => _lockDepth = Math.Max(0, _lockDepth - 1);
    public int Count => _items.Count;
    public string this[int index] { get => _items[index]; set => _items[index] = value; }

    public void Clear() => _items.Clear();
    public int Add(string s) { _items.Add(s ?? ""); return _items.Count - 1; }
    public void Delete(int index) { if (index >= 0 && index < _items.Count) _items.RemoveAt(index); }

    /// <summary>Delphi `TList.Remove(Item)` 语义：按**对象/值**删除首个匹配项（原文 `g_TempIPList.Delete(sIPaddr)`）。
    /// 返回是否删除成功。</summary>
    public bool DeleteItem(string s)
    {
        int i = _items.IndexOf(s ?? "");
        if (i < 0) return false;
        _items.RemoveAt(i);
        return true;
    }

    public int IndexOf(string s) => _items.IndexOf(s ?? "");

    /// <summary>`TStrings.Text`：读 = CRLF 连接；写 = 清空后按行拆分（Delphi SetText 语义，保留末尾空行规则）。</summary>
    public string Text
    {
        get => string.Join("\r\n", _items);
        set
        {
            _items.Clear();
            if (string.IsNullOrEmpty(value)) return;
            var parts = value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            // Delphi TStrings.SetText：末尾换行不产生额外空行
            int count = parts.Length;
            if (count > 0 && parts[count - 1] == "") count--;
            for (int i = 0; i < count; i++) _items.Add(parts[i]);
        }
    }

    public string[] Lines => _items.ToArray();

    /// <summary>原文 `TStrings.SaveToFile` → 复用 GXX.Core 的 GBK 落盘（每行 + CRLF，含末尾换行）。</summary>
    public void SaveToFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var list = new GXX.Core.Util.TStringList();
        foreach (string s in _items) list.Add(s);
        list.SaveToFile(fileName);
    }

    /// <summary>原文 `TStrings.LoadFromFile` → GBK 读入（Delphi SetTextStr 行语义）。</summary>
    public void LoadFromFile(string fileName)
    {
        _items.Clear();
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return;
        var list = new GXX.Core.Util.TStringList();
        list.LoadFromFile(fileName);
        foreach (string line in list.AsEnumerable()) _items.Add(line);
    }
}

/// <summary>接缝：待 GateShare.pas `TProcessInfo` / `TProcessBlacklist` 移植后接入。
/// 原文要点（GateShare.pas:3271-3300）：`FMaxCount := 80`；Count &gt;= MaxCount 时 Add 直接返回 nil；
/// MD5 必须 32 字符且 IsHexString；重复（同名+同 MD5）返回 nil。</summary>
public class TProcessInfo
{
    public string ProcessName = "";
    public string ProcessMD5 = "";
}

/// <summary>GateShare.pas `TProcessBlacklist` 的最小保真移植（本窗体族只用 Add/Delete/Count/MaxCount/Items）。</summary>
public class TProcessBlackList
{
    private readonly List<TProcessInfo> FList = new List<TProcessInfo>();

    public int MaxCount { get; set; } = 80;      // 原 GateShare.pas:3271 FMaxCount := 80

    public int Count => FList.Count;
    public TProcessInfo[] Items => FList.ToArray();

    public void Lock() { }
    public void UnLock() { }

    public void Clear() => FList.Clear();

    /// <summary>GateShare.pas:3296-3300 `function Add(ProcessName, ProcessMD5): PTProcessInfo`。</summary>
    public TProcessInfo Add(string processName, string processMD5)
    {
        if (FList.Count >= MaxCount) return null;     // 原 :3296
        foreach (var item in FList)
        {
            if (string.Equals(item.ProcessMD5, processMD5, StringComparison.OrdinalIgnoreCase))
                return null;                          // 原 :3298（MD5 已存在）
        }
        var info = new TProcessInfo { ProcessName = processName, ProcessMD5 = processMD5 };
        FList.Add(info);
        return info;
    }

    public void Delete(TProcessInfo info) => FList.Remove(info);
    public void Delete(int index) { if (index >= 0 && index < FList.Count) FList.RemoveAt(index); }
}

/// <summary>接缝：待 MagicIntervalUtils.pas 移植后接入。
/// `TMagicInterval = record MagicId: Word; Interval: LongWord; end;`。</summary>
public class TMagicInterval
{
    public int MagicId;
    public uint Interval;
}

/// <summary>GateShare.pas `TMagicIntervalList` 的最小保真移植（Find/Add/Clear/Count/SaveToFile/Lock）。</summary>
public class TMagicIntervalList
{
    private readonly List<TMagicInterval> FList = new List<TMagicInterval>();

    public int Count => FList.Count;
    public TMagicInterval this[int index] => FList[index];

    public void Lock() { }
    public void UnLock() { }
    public void Clear() => FList.Clear();

    /// <summary>按 MagicId 查找（顺序无关）。</summary>
    public TMagicInterval Find(int magicId)
    {
        foreach (var m in FList) if (m.MagicId == magicId) return m;
        return null;
    }

    /// <summary>按 MagicId 添加；已存在返回 null（与原 `Add` 一致）。</summary>
    public TMagicInterval Add(int magicId)
    {
        if (Find(magicId) != null) return null;
        var m = new TMagicInterval { MagicId = magicId, Interval = 0 };
        FList.Add(m);
        return m;
    }

    /// <summary>原文 `SaveToFile`（INI 文本：[Interval] MagicId=Interval）。</summary>
    public void SaveToFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var ini = new TIniFileEx(fileName);
        foreach (var m in FList)
            ini.WriteInteger("Interval", DelphiRTL.IntToStr(m.MagicId), (int)m.Interval);
        ini.Dispose();
    }
}

/// <summary>
/// 接缝：待 ColorIndexEdit.pas 移植后接入。原文用颜色索引（0..255）而不是 TColor。
///
/// ★ 本类原先误写为「16 常用色 + 6×6×6 计算调色板」，那会让下标 ≥ 200 左右算出 &gt;255 的分量并抛
///   `ArgumentException`（实测：`btMsgFColor = $FF` 即下标 255 直接崩）。
/// 现已按原文 **Source\Common\HUtil32.pas:212-293** 逐字节改正：
///   原文不是计算式调色板，而是一张 **`ColorArray: array[0..1023] of Byte`（256 项 × 4 字节 B,G,R,0）** 的常量表，
///   `ColorIndexToTColor` 以 `Inc(PRGBQuad, ColorIndex)` 步进取第 index 项，再 `RGB(rgbRed, rgbGreen, rgbBlue)`。
///   故 **0..255 全部有定义，且原文不存在越界分支**（形参是 Byte）。
/// </summary>
public static class ColorIndex
{
    /// <summary>
    /// HUtil32.pas:219-293 `function ColorIndexToTColor(ColorIndex: Byte): TColor` 的 1:1 移植。
    /// 表项布局 = `TRGBQuad`（B, G, R, Reserved），与原文字节序完全一致。
    /// </summary>
    public static Color ColorIndexToTColor(byte index)
    {
        // 原 :290-291 `ColorTable := @ColorArray[0]; Inc(ColorTable, ColorIndex);`
        int offset = index * 4;
        byte b = ColorArray1024[offset + 0];
        byte g = ColorArray1024[offset + 1];
        byte r = ColorArray1024[offset + 2];
        // 第 4 字节是 TRGBQuad.Reserved（原表恒为 $00），不参与 RGB
        return Color.FromArgb(255, r, g, b);   // 原 :292 `Result := RGB(rgbRed, rgbGreen, rgbBlue)`
    }

    /// <summary>调色板项数（= `High(Byte) + 1`）。原文 `ColorIndex` 是 Byte，故合法下标恒为 0..255。</summary>
    public const int ColorCount = 256;

    /// <summary>测试可见：第 <paramref name="index"/> 项的原始 (B, G, R) 三元组（不改动行为）。</summary>
    public static (byte B, byte G, byte R) RawEntry(byte index)
    {
        int offset = index * 4;
        return (ColorArray1024[offset + 0], ColorArray1024[offset + 1], ColorArray1024[offset + 2]);
    }

    private static readonly byte[] ColorArray1024 =
    {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x80, 0x00,
        0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x80, 0x00, 0x00, 0xC0, 0xC0, 0xC0, 0x00,
        0x97, 0x80, 0x55, 0x00, 0xC8, 0xB9, 0x9D, 0x00, 0x73, 0x73, 0x7B, 0x00, 0x29, 0x29, 0x2D, 0x00,
        0x52, 0x52, 0x5A, 0x00, 0x5A, 0x5A, 0x63, 0x00, 0x39, 0x39, 0x42, 0x00, 0x18, 0x18, 0x1D, 0x00,
        0x10, 0x10, 0x18, 0x00, 0x18, 0x18, 0x29, 0x00, 0x08, 0x08, 0x10, 0x00, 0x71, 0x79, 0xF2, 0x00,
        0x5F, 0x67, 0xE1, 0x00, 0x5A, 0x5A, 0xFF, 0x00, 0x31, 0x31, 0xFF, 0x00, 0x52, 0x5A, 0xD6, 0x00,
        0x00, 0x10, 0x94, 0x00, 0x18, 0x29, 0x94, 0x00, 0x00, 0x08, 0x39, 0x00, 0x00, 0x10, 0x73, 0x00,
        0x00, 0x18, 0xB5, 0x00, 0x52, 0x63, 0xBD, 0x00, 0x10, 0x18, 0x42, 0x00, 0x99, 0xAA, 0xFF, 0x00,
        0x00, 0x10, 0x5A, 0x00, 0x29, 0x39, 0x73, 0x00, 0x31, 0x4A, 0xA5, 0x00, 0x73, 0x7B, 0x94, 0x00,
        0x31, 0x52, 0xBD, 0x00, 0x10, 0x21, 0x52, 0x00, 0x18, 0x31, 0x7B, 0x00, 0x10, 0x18, 0x2D, 0x00,
        0x31, 0x4A, 0x8C, 0x00, 0x00, 0x29, 0x94, 0x00, 0x00, 0x31, 0xBD, 0x00, 0x52, 0x73, 0xC6, 0x00,
        0x18, 0x31, 0x6B, 0x00, 0x42, 0x6B, 0xC6, 0x00, 0x00, 0x4A, 0xCE, 0x00, 0x39, 0x63, 0xA5, 0x00,
        0x18, 0x31, 0x5A, 0x00, 0x00, 0x10, 0x2A, 0x00, 0x00, 0x08, 0x15, 0x00, 0x00, 0x18, 0x3A, 0x00,
        0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x29, 0x00, 0x00, 0x00, 0x4A, 0x00, 0x00, 0x00, 0x9D, 0x00,
        0x00, 0x00, 0xDC, 0x00, 0x00, 0x00, 0xDE, 0x00, 0x00, 0x00, 0xFB, 0x00, 0x52, 0x73, 0x9C, 0x00,
        0x4A, 0x6B, 0x94, 0x00, 0x29, 0x4A, 0x73, 0x00, 0x18, 0x31, 0x52, 0x00, 0x18, 0x4A, 0x8C, 0x00,
        0x11, 0x44, 0x88, 0x00, 0x00, 0x21, 0x4A, 0x00, 0x10, 0x18, 0x21, 0x00, 0x5A, 0x94, 0xD6, 0x00,
        0x21, 0x6B, 0xC6, 0x00, 0x00, 0x6B, 0xEF, 0x00, 0x00, 0x77, 0xFF, 0x00, 0x84, 0x94, 0xA5, 0x00,
        0x21, 0x31, 0x42, 0x00, 0x08, 0x10, 0x18, 0x00, 0x08, 0x18, 0x29, 0x00, 0x00, 0x10, 0x21, 0x00,
        0x18, 0x29, 0x39, 0x00, 0x39, 0x63, 0x8C, 0x00, 0x10, 0x29, 0x42, 0x00, 0x18, 0x42, 0x6B, 0x00,
        0x18, 0x4A, 0x7B, 0x00, 0x00, 0x4A, 0x94, 0x00, 0x7B, 0x84, 0x8C, 0x00, 0x5A, 0x63, 0x6B, 0x00,
        0x39, 0x42, 0x4A, 0x00, 0x18, 0x21, 0x29, 0x00, 0x29, 0x39, 0x46, 0x00, 0x94, 0xA5, 0xB5, 0x00,
        0x5A, 0x6B, 0x7B, 0x00, 0x94, 0xB1, 0xCE, 0x00, 0x73, 0x8C, 0xA5, 0x00, 0x5A, 0x73, 0x8C, 0x00,
        0x73, 0x94, 0xB5, 0x00, 0x73, 0xA5, 0xD6, 0x00, 0x4A, 0xA5, 0xEF, 0x00, 0x8C, 0xC6, 0xEF, 0x00,
        0x42, 0x63, 0x7B, 0x00, 0x39, 0x56, 0x6B, 0x00, 0x5A, 0x94, 0xBD, 0x00, 0x00, 0x39, 0x63, 0x00,
        0xAD, 0xC6, 0xD6, 0x00, 0x29, 0x42, 0x52, 0x00, 0x18, 0x63, 0x94, 0x00, 0xAD, 0xD6, 0xEF, 0x00,
        0x63, 0x8C, 0xA5, 0x00, 0x4A, 0x5A, 0x63, 0x00, 0x7B, 0xA5, 0xBD, 0x00, 0x18, 0x42, 0x5A, 0x00,
        0x31, 0x8C, 0xBD, 0x00, 0x29, 0x31, 0x35, 0x00, 0x63, 0x84, 0x94, 0x00, 0x4A, 0x6B, 0x7B, 0x00,
        0x5A, 0x8C, 0xA5, 0x00, 0x29, 0x4A, 0x5A, 0x00, 0x39, 0x7B, 0x9C, 0x00, 0x10, 0x31, 0x42, 0x00,
        0x21, 0xAD, 0xEF, 0x00, 0x00, 0x10, 0x18, 0x00, 0x00, 0x21, 0x29, 0x00, 0x00, 0x6B, 0x9C, 0x00,
        0x5A, 0x84, 0x94, 0x00, 0x18, 0x42, 0x52, 0x00, 0x29, 0x5A, 0x6B, 0x00, 0x21, 0x63, 0x7B, 0x00,
        0x21, 0x7B, 0x9C, 0x00, 0x00, 0xA5, 0xDE, 0x00, 0x39, 0x52, 0x5A, 0x00, 0x10, 0x29, 0x31, 0x00,
        0x7B, 0xBD, 0xCE, 0x00, 0x39, 0x5A, 0x63, 0x00, 0x4A, 0x84, 0x94, 0x00, 0x29, 0xA5, 0xC6, 0x00,
        0x18, 0x9C, 0x10, 0x00, 0x4A, 0x8C, 0x42, 0x00, 0x42, 0x8C, 0x31, 0x00, 0x29, 0x94, 0x10, 0x00,
        0x10, 0x18, 0x08, 0x00, 0x18, 0x18, 0x08, 0x00, 0x10, 0x29, 0x08, 0x00, 0x29, 0x42, 0x18, 0x00,
        0xAD, 0xB5, 0xA5, 0x00, 0x73, 0x73, 0x6B, 0x00, 0x29, 0x29, 0x18, 0x00, 0x4A, 0x42, 0x18, 0x00,
        0x4A, 0x42, 0x31, 0x00, 0xDE, 0xC6, 0x63, 0x00, 0xFF, 0xDD, 0x44, 0x00, 0xEF, 0xD6, 0x8C, 0x00,
        0x39, 0x6B, 0x73, 0x00, 0x39, 0xDE, 0xF7, 0x00, 0x8C, 0xEF, 0xF7, 0x00, 0x00, 0xE7, 0xF7, 0x00,
        0x5A, 0x6B, 0x6B, 0x00, 0xA5, 0x8C, 0x5A, 0x00, 0xEF, 0xB5, 0x39, 0x00, 0xCE, 0x9C, 0x4A, 0x00,
        0xB5, 0x84, 0x31, 0x00, 0x6B, 0x52, 0x31, 0x00, 0xD6, 0xDE, 0xDE, 0x00, 0xB5, 0xBD, 0xBD, 0x00,
        0x84, 0x8C, 0x8C, 0x00, 0xDE, 0xF7, 0xF7, 0x00, 0x18, 0x08, 0x00, 0x00, 0x39, 0x18, 0x08, 0x00,
        0x29, 0x10, 0x08, 0x00, 0x00, 0x18, 0x08, 0x00, 0x00, 0x29, 0x08, 0x00, 0xA5, 0x52, 0x00, 0x00,
        0xDE, 0x7B, 0x00, 0x00, 0x4A, 0x29, 0x10, 0x00, 0x6B, 0x39, 0x10, 0x00, 0x8C, 0x52, 0x10, 0x00,
        0xA5, 0x5A, 0x21, 0x00, 0x5A, 0x31, 0x10, 0x00, 0x84, 0x42, 0x10, 0x00, 0x84, 0x52, 0x31, 0x00,
        0x31, 0x21, 0x18, 0x00, 0x7B, 0x5A, 0x4A, 0x00, 0xA5, 0x6B, 0x52, 0x00, 0x63, 0x39, 0x29, 0x00,
        0xDE, 0x4A, 0x10, 0x00, 0x21, 0x29, 0x29, 0x00, 0x39, 0x4A, 0x4A, 0x00, 0x18, 0x29, 0x29, 0x00,
        0x29, 0x4A, 0x4A, 0x00, 0x42, 0x7B, 0x7B, 0x00, 0x4A, 0x9C, 0x9C, 0x00, 0x29, 0x5A, 0x5A, 0x00,
        0x14, 0x42, 0x42, 0x00, 0x00, 0x39, 0x39, 0x00, 0x00, 0x59, 0x59, 0x00, 0x2C, 0x35, 0xCA, 0x00,
        0x21, 0x73, 0x6B, 0x00, 0x00, 0x31, 0x29, 0x00, 0x10, 0x39, 0x31, 0x00, 0x18, 0x39, 0x31, 0x00,
        0x00, 0x4A, 0x42, 0x00, 0x18, 0x63, 0x52, 0x00, 0x29, 0x73, 0x5A, 0x00, 0x18, 0x4A, 0x31, 0x00,
        0x00, 0x21, 0x18, 0x00, 0x00, 0x31, 0x18, 0x00, 0x10, 0x39, 0x18, 0x00, 0x4A, 0x84, 0x63, 0x00,
        0x4A, 0xBD, 0x6B, 0x00, 0x4A, 0xB5, 0x63, 0x00, 0x4A, 0xBD, 0x63, 0x00, 0x4A, 0x9C, 0x5A, 0x00,
        0x39, 0x8C, 0x4A, 0x00, 0x4A, 0xC6, 0x63, 0x00, 0x4A, 0xD6, 0x63, 0x00, 0x4A, 0x84, 0x52, 0x00,
        0x29, 0x73, 0x31, 0x00, 0x5A, 0xC6, 0x63, 0x00, 0x4A, 0xBD, 0x52, 0x00, 0x00, 0xFF, 0x10, 0x00,
        0x18, 0x29, 0x18, 0x00, 0x4A, 0x88, 0x4A, 0x00, 0x4A, 0xE7, 0x4A, 0x00, 0x00, 0x5A, 0x00, 0x00,
        0x00, 0x88, 0x00, 0x00, 0x00, 0x94, 0x00, 0x00, 0x00, 0xDE, 0x00, 0x00, 0x00, 0xEE, 0x00, 0x00,
        0x00, 0xFB, 0x00, 0x00, 0x94, 0x5A, 0x4A, 0x00, 0xB5, 0x73, 0x63, 0x00, 0xD6, 0x8C, 0x7B, 0x00,
        0xD6, 0x7B, 0x6B, 0x00, 0xFF, 0x88, 0x77, 0x00, 0xCE, 0xC6, 0xC6, 0x00, 0x9C, 0x94, 0x94, 0x00,
        0xC6, 0x94, 0x9C, 0x00, 0x39, 0x31, 0x31, 0x00, 0x84, 0x18, 0x29, 0x00, 0x84, 0x00, 0x18, 0x00,
        0x52, 0x42, 0x4A, 0x00, 0x7B, 0x42, 0x52, 0x00, 0x73, 0x5A, 0x63, 0x00, 0xF7, 0xB5, 0xCE, 0x00,
        0x9C, 0x7B, 0x8C, 0x00, 0xCC, 0x22, 0x77, 0x00, 0xFF, 0xAA, 0xDD, 0x00, 0x2A, 0xB4, 0xF0, 0x00,
        0x9F, 0x00, 0xDF, 0x00, 0xB3, 0x17, 0xE3, 0x00, 0xF0, 0xFB, 0xFF, 0x00, 0xA4, 0xA0, 0xA0, 0x00,
        0x80, 0x80, 0x80, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00,
        0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00
    };
}

#endregion

#region 最小 WinForms 控件复刻（Delphi 侧第三方/自研控件）

/// <summary>WinForms 控件缺失的原生消息发送辅助（`SendMessage` 在 .NET 8 未公开）。</summary>
internal static class WinFormsControlHelper
{
    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>等价于 Delphi 的 `SendMessage(Handle, Msg, WParam, LParam)`。</summary>
    public static IntPtr SendMessage(this Control control, int msg, IntPtr wParam, IntPtr lParam)
        => SendMessage(control.Handle, msg, wParam, lParam);
}

/// <summary>
/// 接缝：`TVirtualStringTree`（VirtualTrees.pas，第三方控件）未移植 →
/// 用 WinForms `TreeView` 的 OwnerDraw 多列视图承载（见文件头偏差 D1）。
/// 节点数据 = `TreeNode.Tag as TAntiPlugActionMode`（原文为 `GetNodeData → PNodeData.ActionMode`）。
/// 控件本身**只负责绘制**，所有取文本/可否编辑/提示/勾选规则都走
/// <see cref="GameSpeedLogic"/> 的静态纯函数，便于单测。
/// </summary>
public class TVirtualStringTreeStub : TreeView
{
    /// <summary>列宽（外部按 .dfm 的 Columns 依次赋值；`+` 号对应原文 3 个可变宽列）。</summary>
    public readonly System.Collections.Generic.List<int> ColumnWidths = new System.Collections.Generic.List<int>();

    /// <summary>`DefaultNodeHeight = 22`（.dfm）。</summary>
    public int DefaultNodeHeight = 22;

    /// <summary>`Header.DefaultHeight = 24`（.dfm）。</summary>
    public int HeaderDefaultHeight = 24;

    /// <summary>原文 `IsEditing`。</summary>
    public bool IsEditing { get; set; }

    /// <summary>测试可见：当前"聚焦"节点（原文 FocusedNode）。</summary>
    public TreeNode FocusedNode2 => SelectedNode;

    public TVirtualStringTreeStub()
    {
        DrawMode = TreeViewDrawMode.OwnerDrawText;   // 逐节点自绘（原文 TreeOptions.PaintOptions 的等价近似）
        ItemHeight = DefaultNodeHeight;
        ShowLines = false;
        ShowPlusMinus = false;
        ShowRootLines = false;
        HideSelection = false;
        FullRowSelect = true;
        BackColor = System.Drawing.Color.White;
    }

    /// <summary>原文 `GetNodeData(Node)` → 托管侧从 Tag 取 ActionMode。</summary>
    public TAntiPlugActionMode? GetActionMode(TreeNode node)
        => node?.Tag is TAntiPlugActionMode m ? m : (TAntiPlugActionMode?)null;

    /// <summary>原文 `AddChild(nil)`。</summary>
    public TreeNode AddChildMode(TAntiPlugActionMode mode)
    {
        var node = new TreeNode();
        node.Tag = mode;
        Nodes.Add(node);
        return node;
    }

    /// <summary>原文 `GetFirst()`。</summary>
    public TreeNode GetFirst() => Nodes.Count > 0 ? Nodes[0] : null;

    /// <summary>原文 `GetNext(Node)`。</summary>
    public TreeNode GetNext(TreeNode node)
    {
        if (node == null) return null;
        int i = node.Index;
        return i + 1 < Nodes.Count ? Nodes[i + 1] : null;
    }

    /// <summary>原文 `CheckState[Node]`（csCheckedNormal / csUncheckedNormal）。</summary>
    public bool GetCheckState(TreeNode node) => node?.Checked ?? false;
    public void SetCheckState(TreeNode node, bool value) { if (node != null) node.Checked = value; }

    /// <summary>原文 `InvalidateNode(Node)`。</summary>
    public void InvalidateNode(TreeNode node) => Invalidate();
}

/// <summary>Delphi `Spin.pas` 的 `TSpinEdit`（与 GXX.DBServer.SpinControls.cs 同构；命名空间不同）。
/// 依据（原 DFM + 代码交叉验证）：uFrmItemEatCD.dfm 的 TSpinEditEx 全是 `MaxValue=0 MinValue=0` 且 `Value=0`，
/// 而 uFrmGameSpeed 代码把 `MaxValue := High(Integer)` 后直接 `Value := g_Config...nInterval`（可达 1200）
/// → **编程赋值不做 Min/Max 裁剪**，裁剪只发生在上下按钮。WinForms `NumericUpDown.Value` 会裁剪，
/// 故把底层范围放宽到 int 全域，DFM 的 Min/Max 单独保存。</summary>
public class TSpinEdit : NumericUpDown
{
    /// <summary>DFM: MinValue。</summary>
    public int DfmMinValue;

    /// <summary>DFM: MaxValue（0 表示原文未设上限）。</summary>
    public int DfmMaxValue;

    public TSpinEdit()
    {
        Minimum = int.MinValue;
        Maximum = int.MaxValue;
    }

    /// <summary>DFM/代码: Value（int；编程赋值不裁剪，原文如此）。</summary>
    public new int Value
    {
        get => (int)base.Value;
        set => base.Value = value;
    }

    public void SetDfmRange(int minValue, int maxValue)
    {
        DfmMinValue = minValue;
        DfmMaxValue = maxValue;
    }

    public override void UpButton()
    {
        if (DfmMaxValue != 0 && Value >= DfmMaxValue) return;
        base.UpButton();
    }

    public override void DownButton()
    {
        if (Value <= DfmMinValue) return;
        base.DownButton();
    }
}

/// <summary>SpinEditEx.pas 的 `TSpinEditEx`</summary>
public class TSpinEditEx : TSpinEdit
{
}

/// <summary>ColorIndexEdit.pas 的 `TColorIndexEdit`：值域 0..255 的颜色索引选择器。</summary>
public class TColorIndexEdit : NumericUpDown
{
    public TColorIndexEdit()
    {
        Minimum = 0;
        Maximum = 255;
    }

    /// <summary>原文 `Value: Byte`。</summary>
    public byte ColorIndex
    {
        get => (byte)base.Value;
        set => base.Value = value;
    }

    protected override void OnValueChanged(EventArgs e)
    {
        base.OnValueChanged(e);
        // 全限定：属性名 ColorIndex 会遮蔽同名的静态类 ColorIndex（C# 成员优先于类型名）。
        BackColor = GXX.RunGate.ColorIndex.ColorIndexToTColor(ColorIndex);
    }
}

#endregion
