// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   797-1056   TConfig record / pTConfig / 单元级 g_Config:TConfig 的逐字段初值
//   1058-1157  constructor TMirConfigDlg.Create
//   1159-1166  destructor TMirConfigDlg.Destroy
//   1168-1262  访问器（GetType/Get+SetConfigChecked/Open/Close/Get+SetVisible/
//              Get+SetEnabled/Get+SetProtectEnabled）
//   1264-1282  FormKeyDown / FormKeyPress / RefreshMySelfAbil / RefreshMyHeroAbil
//   1304-1308  RefreshMyHeroMagicList（空实现）
//   1394-1410  PlugPageControlConfigInRealArea / PlugConfigDlgCloseClickEx
//   1412-1422  PlugPageControlConfigActivePageChange
//
// 移植纪律：1:1 逐字（类型名/方法名/字段名/常量名/分支顺序/边界行为）。
//   原文笔误与冗余保留并注释 `// 原文如此（MirConfigDlg.pas:<行>）`。
//   所有外部依赖走接缝（MirConfigSeams / MirDxControlSeams），不复制第二份实现。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

/// <summary>
/// <c>TMirConfigDlg = class(TGameConfigObject)</c>（MirConfigDlg.pas:42）。
///
/// 本类为 partial，按原文方法族分文件：
/// <list type="bullet">
/// <item><c>MirConfigDlg.cs</c>            TConfig / g_Config / Create / Destroy / 访问器 / 派发器</item>
/// <item><c>MirConfigDlg.Controls.cs</c>   控件→g_Config 写回（Change/Click 处理器）</item>
/// <item><c>MirConfigDlg.Auto.cs</c>       自动吃药 / 保护 / 持久警告 / 伤害喝药</item>
/// <item><c>MirConfigDlg.ConfigFile.cs</c> LoadConfigFile / SaveConfigFile</item>
/// <item><c>MirConfigDlg.Lists.cs</c>      Boss / 挂机怪物 / 挂机技能 名单与文件</item>
/// <item><c>MirConfigDlg.Lifecycle.cs</c>  LoadClientConfig / Initialize / 查询转发</item>
/// </list>
/// </summary>
public partial class TMirConfigDlg : TGameConfigObject
{
    // ================================================================================
    // MirConfigDlg.pas:797-904  TConfig record / pTConfig
    // 原文为 record；托管侧用 class 表达（与 TMirsConfigDlg.TMConfig /
    // TMirReturnConfigDlg.TConfig 同一取舍：配置量大，且 g_Config 是单元级单例、
    // 需按引用原地改字段）。数组维度严格按原文：0..4（5 档用药模式）、0..8（9 种超级药）。
    // ================================================================================
    public sealed class TConfig
    {
        public int nFilterMinExp;
        public int nAutoUseMagicTime;
        public uint dwAutoUseMagicTick;

        public bool boRenewSpecialIsAuto;
        public int nRenewSpecialPercent;
        public int nRenewSpecialTime;

        public bool boRenewBookIsAuto;
        public int nRenewBookPercent;
        public int nRenewBookTime;
        public int nRenewBookNowBookIndex;
        public string sRenewBookNowBookItem = "";
        // ============药品==================
        public int nRenewHeroHPTime;
        public int nRenewHeroHPPercent;

        public int nRenewHeroMPTime;
        public int nRenewHeroMPPercent;

        public bool boRenewHeroSpecialIsAuto;
        public int nRenewHeroSpecialTime;
        public int nRenewHeroSpecialPercent;

        public bool boRenewHeroLogOutIsAuto;
        public int nRenewHeroLogOutTime;
        public int nRenewHeroLogOutPercent;

        public bool boRenewCloseIsAuto;
        public int nRenewCloseTime;
        public int nRenewClosePercent;

        /// <summary>原文 <c>MedicaMode:Integer; // 0主体 1英雄 2战士副将 3法师副将 4道士副将</c>。</summary>
        public int MedicaMode;

        public readonly bool[] ChkAutoPercents = new bool[5];
        public readonly bool[] ChkRenewAutoPercents = new bool[5];
        public readonly bool[] ChkSuperMedicaPercents = new bool[5];

        public readonly bool[] CheckHpIsAutos = new bool[5];
        public readonly int[] CheckHpPercents = new int[5];
        public readonly int[] CheckHpValues = new int[5];
        public readonly uint[] CheckHpCheckTimes = new uint[5];
        public readonly uint[] CheckHpCheckTicks = new uint[5];
        public readonly uint[] CheckHpUseTimes = new uint[5];
        public readonly uint[] CheckHpUseTicks = new uint[5];

        public readonly bool[] CheckMpIsAutos = new bool[5];
        public readonly int[] CheckMpPercents = new int[5];
        public readonly int[] CheckMpValues = new int[5];
        public readonly uint[] CheckMpCheckTimes = new uint[5];
        public readonly uint[] CheckMpCheckTicks = new uint[5];
        public readonly uint[] CheckMpUseTimes = new uint[5];
        public readonly uint[] CheckMpUseTicks = new uint[5];

        public readonly bool[] RenewHPIsAutos = new bool[5];
        public readonly int[] RenewHPPercents = new int[5];
        public readonly int[] RenewHPTimes = new int[5];
        public readonly uint[] RenewHPTicks = new uint[5];
        public readonly uint[] RenewHPChatStringTicks = new uint[5];

        public readonly bool[] RenewMPIsAutos = new bool[5];
        public readonly int[] RenewMPPercents = new int[5];
        public readonly int[] RenewMPTimes = new int[5];
        public readonly uint[] RenewMPTicks = new uint[5];
        public readonly uint[] RenewMPChatStringTicks = new uint[5];

        public readonly bool[] RenewSpecialHPIsAutos = new bool[5];
        public readonly int[] RenewSpecialHPPercents = new int[5];
        public readonly int[] RenewSpecialHPTimes = new int[5];
        public readonly uint[] RenewSpecialHPTicks = new uint[5];
        public readonly uint[] RenewSpecialHPChatStringTicks = new uint[5];

        public readonly bool[] RenewSpecialMPIsAutos = new bool[5];
        public readonly int[] RenewSpecialMPPercents = new int[5];
        public readonly int[] RenewSpecialMPTimes = new int[5];
        public readonly uint[] RenewSpecialMPTicks = new uint[5];
        public readonly uint[] RenewSpecialMPChatStringTicks = new uint[5];

        public readonly bool[] UseSuperMedicas = new bool[5];
        /// <summary>原文 <c>SuperMedicaItemNames:array[0..8] of string</c>（9 种超级药名）。</summary>
        public readonly string[] SuperMedicaItemNames = new string[9];
        public readonly bool[][] SuperMedicaUses = NewMatrixBool();
        public readonly int[][] SuperMedicaHPs = NewMatrixInt();
        public readonly int[][] SuperMedicaHPTimes = NewMatrixInt();
        public readonly int[][] SuperMedicaHPTicks = NewMatrixInt();

        public readonly int[][] SuperMedicaMPs = NewMatrixInt();
        public readonly int[][] SuperMedicaMPTimes = NewMatrixInt();
        public readonly int[][] SuperMedicaMPTicks = NewMatrixInt();

        public readonly bool[] CheckDuraIsAutos = new bool[5];
        public readonly int[] CheckDuraMin = new int[5];
        /// <summary>原文 <c>CheckDuraValue:array[0..4] of string[20]</c>（短串，托管侧用 string 表达）。</summary>
        public readonly string[] CheckDuraValue = new string[5];
        public readonly int[] CheckDuraTime = new int[5];
        public readonly uint[] CheckDuraCheckTicks = new uint[5];

        public int nHeroDodgeHPPercent;
        /// <summary>BOSS 变色显示 piaoyun 2013-09-09</summary>
        public byte nColorShowEff;
        /// <summary>特殊物品颜色 piaoyun 2013-09-09</summary>
        public byte nSpecialColor;

        /// <summary>挂机 - 受玩家攻击后的操作</summary>
        public int nGJPlayAttackOption;
        /// <summary>挂机 - 红药用完后动作 chongchong 2014-12-06</summary>
        public int nGJNoRedPoisonOption;
        /// <summary>挂机 - 蓝药用完后动作 chongchong 2014-12-06</summary>
        public int nGJNoBluePoisonOption;
        /// <summary>挂机 - 毒符用完后动作 chongchong 2014-12-06</summary>
        public int nGJNoDuFuOption;
        /// <summary>挂机 - 包裹满后动作 chongchong 2014-12-06</summary>
        public int nGJBagFullOption;
        /// <summary>挂机 - 怪物周围几格有玩家</summary>
        public int nGJNotRushMonRange;
        /// <summary>挂机 - 当被怪物包围时的操作</summary>
        public int nGJGroupAttackCount;

        private static bool[][] NewMatrixBool()
        {
            var m = new bool[5][];
            for (int i = 0; i < 5; i++) m[i] = new bool[9];
            return m;
        }
        private static int[][] NewMatrixInt()
        {
            var m = new int[5][];
            for (int i = 0; i < 5; i++) m[i] = new int[9];
            return m;
        }
    }

    /// <summary>
    /// 原文 <c>var g_Config:TConfig = (…);</c>（MirConfigDlg.pas:907-1056）的逐字段初值。
    /// ★ 所有"非零"初值都逐条照抄原文；被原文省略的字段在 Delphi 里取 0/空串/False。
    /// </summary>
    public static TConfig g_Config = NewDefaultConfig();

    /// <summary>构造原文 908-1056 那份带初值的 <c>g_Config</c>（测试可重置）。</summary>
    public static TConfig NewDefaultConfig()
    {
        var c = new TConfig();
        c.nFilterMinExp = 0;                                   // 909
        c.nAutoUseMagicTime = 0;                               // 910
        c.dwAutoUseMagicTick = 0;                              // 911
        c.boRenewSpecialIsAuto = false;                        // 913
        c.nRenewSpecialPercent = 0;                            // 914
        c.nRenewSpecialTime = 0;                               // 915
        c.boRenewBookIsAuto = false;                           // 917
        c.nRenewBookPercent = 0;                               // 918
        c.nRenewBookTime = 0;                                  // 919
        c.nRenewBookNowBookIndex = 0;                          // 920
        c.sRenewBookNowBookItem = "";                          // 921
        c.MedicaMode = 0;                                      // 924  0主体 1英雄 2战士副将 3法师副将 4道士副将
        // 926：ChkAutoPercents:(False,...) → 默认 false
        // 927：ChkRenewAutoPercents:(False, True, True, True, True)
        c.ChkRenewAutoPercents[0] = false;
        c.ChkRenewAutoPercents[1] = true;
        c.ChkRenewAutoPercents[2] = true;
        c.ChkRenewAutoPercents[3] = true;
        c.ChkRenewAutoPercents[4] = true;
        // 928：ChkSuperMedicaPercents:(False,...) → 默认 false
        // 930-936：Check* 系列
        for (int i = 0; i < 5; i++) c.CheckHpCheckTimes[i] = 1000;    // 933
        for (int i = 0; i < 5; i++) c.CheckHpUseTimes[i] = 10000;     // 935
        for (int i = 0; i < 5; i++) c.CheckMpCheckTimes[i] = 1000;    // 941
        for (int i = 0; i < 5; i++) c.CheckMpUseTimes[i] = 10000;     // 943
        // 947：RenewHPPercents:(10, 97, 97, 97, 97)
        c.RenewHPPercents[0] = 10; for (int i = 1; i < 5; i++) c.RenewHPPercents[i] = 97;
        for (int i = 0; i < 5; i++) c.RenewHPTimes[i] = 1000;         // 948
        // 953：RenewMPPercents:(10, 97, 97, 97, 97)
        c.RenewMPPercents[0] = 10; for (int i = 1; i < 5; i++) c.RenewMPPercents[i] = 97;
        for (int i = 0; i < 5; i++) c.RenewMPTimes[i] = 1000;         // 954
        // 959：RenewSpecialHPPercents:(10, 88, 88, 88, 88)
        c.RenewSpecialHPPercents[0] = 10; for (int i = 1; i < 5; i++) c.RenewSpecialHPPercents[i] = 88;
        c.RenewSpecialHPTimes[0] = 1000; for (int i = 1; i < 5; i++) c.RenewSpecialHPTimes[i] = 3000;  // 960
        // 965：RenewSpecialMPPercents:(10, 88, 88, 88, 88)
        c.RenewSpecialMPPercents[0] = 10; for (int i = 1; i < 5; i++) c.RenewSpecialMPPercents[i] = 88;
        c.RenewSpecialMPTimes[0] = 1000; for (int i = 1; i < 5; i++) c.RenewSpecialMPTimes[i] = 3000;  // 966
        // 972-981：SuperMedicaItemNames（9 条，逐字照抄）
        c.SuperMedicaItemNames[0] = "太阳水";
        c.SuperMedicaItemNames[1] = "强效太阳水";
        c.SuperMedicaItemNames[2] = "万年雪霜";
        c.SuperMedicaItemNames[3] = "疗伤药";
        c.SuperMedicaItemNames[4] = "疗伤药(任务)";
        c.SuperMedicaItemNames[5] = "强效万年雪霜";
        c.SuperMedicaItemNames[6] = "强效疗伤药";
        c.SuperMedicaItemNames[7] = "超级万年雪霜";
        c.SuperMedicaItemNames[8] = "超级疗伤药";
        // 999-1005 / 1023-1029：SuperMedicaHPTimes / SuperMedicaMPTimes 全 500
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 9; j++)
            {
                c.SuperMedicaHPTimes[i][j] = 500;
                c.SuperMedicaMPTimes[i][j] = 500;
            }
        // 1039：CheckDuraMin:(20, 20, 20, 20, 20)
        for (int i = 0; i < 5; i++) c.CheckDuraMin[i] = 20;
        // 1040：CheckDuraValue:('修复神水', ×5)
        for (int i = 0; i < 5; i++) c.CheckDuraValue[i] = "修复神水";
        // 1041：CheckDuraTime:(30, 30, 30, 30, 30)
        for (int i = 0; i < 5; i++) c.CheckDuraTime[i] = 30;
        c.nHeroDodgeHPPercent = 0;                             // 1044
        c.nColorShowEff = 3;                                   // 1045  BOSS变色显示 piaoyun 2013-09-09
        c.nSpecialColor = 249;                                 // 1046
        // 1048-1052：nGJ*Option 全部为 0（默认）
        c.nGJNotRushMonRange = 7;                              // 1054  挂机 - 怪物周围几格有玩家
        c.nGJGroupAttackCount = 3;                             // 1055  挂机 - 目标周围有几个怪群攻
        return c;
    }

    // ================================================================================
    // MirConfigDlg.pas:560-576  private 字段（名字逐字保留）
    // ================================================================================
    /// <summary>原文 <c>FLoadControl:Boolean;</c>（561）。</summary>
    protected bool FLoadControl;
    /// <summary>原文 <c>FLoadConfig:Boolean;</c>（562）。</summary>
    protected bool FLoadConfig;
    /// <summary>原文 <c>FHandle:THandle;</c>（563）。</summary>
    protected IntPtr FHandle;
    /// <summary>原文 <c>FScreenMode:Byte;</c>（564）。</summary>
    protected byte FScreenMode;
    /// <summary>原文 <c>FClientVersion:TClientVersion;</c>（565）。</summary>
    protected TClientVersion FClientVersion;
    /// <summary>原文 <c>FWindowMode:Boolean;</c>（566）。</summary>
    protected bool FWindowMode;

    /// <summary>原文 <c>FEnabled:Boolean;</c>（568）。</summary>
    protected bool FEnabled;
    /// <summary>
    /// 原文 <c>FConfigCheckeds:array[TConfigChecked] of Boolean;</c>（569）。
    /// 长度 = <c>HighOrdinal + 1</c>（= 134），与基线枚举**完全同长**：
    /// 本单元用到的每个 <c>ck*</c> 都能同名命中基线枚举，无需合成槽位
    /// （见 <see cref="MirConfigCheckedMap"/> 的核实结论）。
    /// </summary>
    protected readonly bool[] FConfigCheckeds = new bool[TConfigCheckedBounds.HighOrdinal + 1];
    /// <summary>原文 <c>FInitializeed:Boolean;</c>（570；原文拼写如此，少一个 i）。</summary>
    protected bool FInitializeed;

    /// <summary>原文 <c>FHintItemDuraTick:LongWord;</c>（573）。</summary>
    protected uint FHintItemDuraTick;
    /// <summary>
    /// 原文 <c>FClientConfig:TClientConfig;</c>（574）。
    /// 托管侧类型用 <see cref="MirClientConfigFull"/>（本单元实际读取的 89 个字段，
    /// 见该类型的注释与报告 D-P10-03）。
    /// </summary>
    protected MirClientConfigFull FClientConfig;
    /// <summary>原文 <c>FProtectEnabled:Boolean;</c>（575）。</summary>
    protected bool FProtectEnabled;
    /// <summary>原文 <c>FProtectEnabledTick:LongWord;</c>（576）。</summary>
    protected uint FProtectEnabledTick;

    /// <summary>原文 <c>public FMemo:TMemo;</c>（731）。托管侧用最小行模型表达（见 MirDxControlSeams.TStrings）。</summary>
    public TStrings FMemo;

    /// <summary>原文 731 <c>FMemo.Visible</c>（托管侧只用到可见性）。</summary>
    protected bool FMemoVisible;

    // ================================================================================
    // 控件访问面（原文 43-558 的 516 个字段 → MirConfigDlgControls.g.cs）
    // ================================================================================
    private IMirConfigDlgControls _plug;

    /// <summary>
    /// 接缝：控件树。默认为纯内存实现（<c>MirConfigDlgControls</c>，516 个控件）；
    /// 宿主/测试可注入自己的实现。名字 <c>Plug</c> 与 MirReturn/Mirs 两个车道一致。
    /// </summary>
    public IMirConfigDlgControls Plug
    {
        get => _plug;
        set => _plug = value;
    }

    /// <summary>取具体控件表（派发与绑定需要按名字查控件）。</summary>
    protected TMirConfigDlgControls PlugCtl => (TMirConfigDlgControls)_plug;

    /// <summary>
    /// 原文 <c>constructor TMirConfigDlg.Create();</c>（1058-1157）。
    ///
    /// 逐字语义：
    ///  1) 1058-1075：字段初值（FEnabled=False / FProtectEnabled=True / FProtectEnabledTick=MyGetTickCount /
    ///     FClientVersion=cvSerial / FWindowMode=True / 两个 FillChar 清零）；
    ///  2) 1077 FHintItemDuraTick := MyGetTickCount;
    ///  3) 1079-1089 整段 <c>{ }</c> 注释掉的 FProtectList（**原文如此**，不建对象）；
    ///  4) 1092-1139 共 40 条 <c>FConfigCheckeds[...] := ...</c> 默认值；
    ///  5) 1141-1150 建 FMemo 并设 7 个属性（Parent/Color/Font.Color/Font.Size/Ctl3D/BorderStyle/Visible）；
    ///  6) 1152-1156 <c>{$IF TESTMODE = 0}</c>：把 <c>g_ConfigClient.ClientConfigs_Ex[0]</c>
    ///     灌进 <c>g_Config.CheckDuraIsAutos[0..4]</c>。
    /// </summary>
    public TMirConfigDlg(IMirConfigDlgControls plug = null)
    {
        var ctl = plug as TMirConfigDlgControls ?? new TMirConfigDlgControls();
        _plug = plug ?? ctl;
        ctl.BindEvents(this);                                   // 原文 4621-5044 的 279 条绑定

        FEnabled = false;                                      // 1064
        FProtectEnabled = true;                                // 1065
        FProtectEnabledTick = MirConfigGlobalSeam.MyGetTickCount();  // 1066
        FHandle = IntPtr.Zero;                                 // 1067
        FScreenMode = 0;                                       // 1068
        FClientVersion = TClientVersion.cvSerial;              // 1069
        FWindowMode = true;                                    // 1070
        FInitializeed = false;                                 // 1071
        FLoadControl = false;                                  // 1072
        FLoadConfig = false;                                   // 1073
        Array.Clear(FConfigCheckeds, 0, FConfigCheckeds.Length);   // 1074 FillChar(FConfigCheckeds, ...)
        FClientConfig = new MirClientConfigFull();             // 1075 FillChar(FClientConfig, ...)

        FHintItemDuraTick = MirConfigGlobalSeam.MyGetTickCount();   // 1077

        /*
        FProtectList := TStringList.Create;                    // 1079-1089 整段被 {} 注释掉 —— 原文如此
        FProtectList.Add('随机传送卷');
        FProtectList.Add('地牢逃脱卷');
        FProtectList.Add('回城卷');
        FProtectList.Add('行会回城卷');
        FProtectList.Add('盟重传送石');
        FProtectList.Add('比奇传送石');
        FProtectList.Add('随机传送石');
        FProtectList.Add('小退');
        */

        // 原文 1091 注释：//HZQ 20230602 此处未设置GUI内挂默认选项
        FConfigCheckeds[(int)TConfigChecked.ckShowHPLabel] = true;                    // 1092
        FConfigCheckeds[(int)TConfigChecked.ckShowUserName] = false;                  // 1093
        FConfigCheckeds[(int)TConfigChecked.ckMagicLock] = true;                      // 1094
        FConfigCheckeds[(int)TConfigChecked.ckAutoOrderItem] = true;                  // 1095
        FConfigCheckeds[(int)TConfigChecked.ckNotNeedShift] = true;                   // 1096
        FConfigCheckeds[(int)TConfigChecked.ckPickUpAll] = false;                     // 1097
        FConfigCheckeds[(int)TConfigChecked.ckBGMusic] = true;                        // 1098
        FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic] = true;                  // 1099
        FConfigCheckeds[(int)TConfigChecked.ckNotParaly] = false;                     // 1100

        FConfigCheckeds[(int)TConfigChecked.ckSmartLongHit] = false;                  // 1102 刀刀刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartPosLongHit] = false;               // 1103 隔位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWalkLongHit] = false;              // 1104 走位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWideHit] = false;                  // 1105 智能半月
        FConfigCheckeds[(int)TConfigChecked.ckSmartFireHit] = false;                  // 1106 自动烈火
        FConfigCheckeds[(int)TConfigChecked.ckSmartSwordHit] = false;                 // 1107 逐日剑法
        FConfigCheckeds[(int)TConfigChecked.ckSmartCrsHit] = false;                   // 1108 抱月刀 双龙斩
        FConfigCheckeds[(int)TConfigChecked.ckSmartTwnHit] = false;                   // 1109 龙影剑法

        FConfigCheckeds[(int)TConfigChecked.ckHumAutoShield] = false;                 // 1111 自动开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumStruckShield] = false;               // 1112 被攻击开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget] = true;      // 1113 疾光电影锁定目标
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom] = false;           // 1114 手动控制爆裂火焰
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind] = false;           // 1115 手动控制冰咆哮
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower] = false;       // 1116 手动控制流星火雨
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMove10Attack] = false;       // 1117（原文注释写"流星火雨"——**原文如此**，复制粘贴笔误）

        FConfigCheckeds[(int)TConfigChecked.ckShowNpcName] = false;                   // 1119
        FConfigCheckeds[(int)TConfigChecked.ckShowNpcHPLabel] = false;                // 1120
        FConfigCheckeds[(int)TConfigChecked.ckShowNGLabel] = false;                   // 1121

        FConfigCheckeds[(int)TConfigChecked.ckNearHint] = true;                       // 1123
        FConfigCheckeds[(int)TConfigChecked.ckAutoLock] = false;                      // 1124
        FConfigCheckeds[(int)TConfigChecked.ckColorShow] = false;                     // 1125
        FConfigCheckeds[(int)TConfigChecked.ckSpecialQuickFlashing] = false;          // 1126
        FConfigCheckeds[(int)TConfigChecked.ckBlacklistHit] = false;                  // 1127
        FConfigCheckeds[(int)TConfigChecked.ckFriendHit] = false;                     // 1128
        FConfigCheckeds[(int)TConfigChecked.ckSceneShake] = false;                    // 1129
        FConfigCheckeds[(int)TConfigChecked.ckAutoDownHorse] = false;                 // 1130

        FConfigCheckeds[(int)TConfigChecked.ckAutoPickUpItem] = true;                 // 1132
        FConfigCheckeds[(int)TConfigChecked.ckNoCaton] = false;                       // 1133

        // 原文 1135 注释：//HZQ 20230601 设置 火墙淡化、隐藏怪物顶戴、自动绕行 默认值
        FConfigCheckeds[(int)TConfigChecked.ckAutoDetourPath] = true;                 // 1136
        FConfigCheckeds[(int)MirConfigCheckedMap.ckHideMonsterIcons] = false;         // 1137
        FConfigCheckeds[(int)MirConfigCheckedMap.ckDimFireEffect] = false;            // 1138
        FConfigCheckeds[(int)MirConfigCheckedMap.ckObjectHintEffect] = false;         // 1139 //HZQ 20230829

        // 原文 1141-1150：建 FMemo（TMemo.Create(frmMain.Owner)）并设 7 个属性。
        // 托管侧 TMemo 未移植 ⇒ 用最小行模型表达；Parent/Color/Font.*/Ctl3D/BorderStyle
        // 是纯外观，无业务分支（原文注释里 BorderStyle 那行还挂着被注释掉的 OnKeyPress）。
        FMemo = new TStrings();
        FMemoVisible = false;                                  // 1149 Visible := False

        // {$IF TESTMODE = 0}
        for (int I = 0 /*Low(g_Config.CheckDuraIsAutos)*/;
             I <= 4 /*High(g_Config.CheckDuraIsAutos)*/; I++)       // 1153
        {
            // 原文 1154：g_Config.CheckDuraIsAutos[I] := g_ConfigClient.ClientConfigs_Ex[0];
            // 数组为空时按 Delphi 边界检查语义跳过（$R+ 下为运行时错误，此处保守跳过）。
            if (MirConfigGlobalSeam.g_ConfigClient.ClientConfigs_Ex.Length > 0)
                g_Config.CheckDuraIsAutos[I] = MirConfigGlobalSeam.g_ConfigClient.ClientConfigs_Ex[0];
        }
        // {$IFEND}
    }

    /// <summary>
    /// 原文 <c>destructor TMirConfigDlg.Destroy;</c>（1159-1166）：
    /// <c>if FEnabled then SaveConfigFile; inherited;</c>
    ///
    /// 托管侧不能声明终结器（原文另有公开方法 <c>Finalize</c>，C# 里二者重名 CS0111），
    /// 故实现为显式 <see cref="Destroy"/>，由宿主在释放时调用；与 Delphi 确定性析构的
    /// 差异已在报告登记（沿用 TMirReturnConfigDlg/MirsConfigDlg 的同一处置）。
    /// </summary>
    public void Destroy()
    {
        if (FEnabled)                                          // 1161
            SaveConfigFile();                                  // 1162

        //FProtectList.Free;（1164 —— 原文如此）
        // inherited;（1165）→ 基类 TGameConfigObject 无析构逻辑
    }

    // ================================================================================
    // MirConfigDlg.pas:1168-1262  访问器
    // ================================================================================

    /// <summary>原文 1168-1171：<c>Result := ptDefault;</c></summary>
    public override TConfigDlgType GetType() => TConfigDlgType.ptDefault;

    /// <summary>原文 1173-1176：直接返回 <c>FConfigCheckeds[Index]</c>，**无任何保护**。</summary>
    public override bool GetConfigChecked(TConfigChecked Index) => FConfigCheckeds[(int)Index];

    /// <summary>原文 1178-1184：值变化时才写并 <c>RefConfig</c>。</summary>
    public override void SetConfigChecked(TConfigChecked Index, bool Value)
    {
        if (FConfigCheckeds[(int)Index] != Value)              // 1180
        {
            FConfigCheckeds[(int)Index] = Value;               // 1181
            RefConfig();                                       // 1182
        }
    }

    /// <summary>原文 1186-1189：**空实现**（原文如此）。</summary>
    public override void Open() { }

    /// <summary>
    /// 原文 1191-1205：<c>FLoadConfig := False; FEnabled := False;
    /// if PlugConfigDlg &lt;&gt; nil then PlugConfigDlg.Visible := False; g_FileItemDB.BackUp;
    /// if FMemo.Visible then begin PlugMemoConfigNotes.Lines.Assign(FMemo.Lines); SaveNotesFile; ShowViewNotes; end;</c>
    /// </summary>
    public override void Close()
    {
        FLoadConfig = false;                                   // 1193
        FEnabled = false;                                      // 1194
        if (PlugCtl.PlugConfigDlg != null)                     // 1195
            PlugCtl.PlugConfigDlg.Visible = false;             // 1196
        FilterItemsGlobal.g_FileItemDB.BackUp();               // 1197 原文 g_FileItemDB.BackUp

        if (FMemoVisible)                                      // 1199
        {
            PlugCtl.PlugMemoConfigNotes.Lines.Assign(FMemo);   // 1200
            SaveNotesFile();                                   // 1201

            ShowViewNotes();                                   // 1203
        }
    }

    /// <summary>
    /// 原文 1207-1214：
    /// <c>if PlugConfigDlg &lt;&gt; nil then Result := PlugConfigDlg.Visible else Result := False;</c>
    /// ★ 与 TMirReturnConfigDlg.GetVisible 不同 —— 本单元的 else 分支**显式**赋了 False（1212 //HZQ 20230525）。
    /// </summary>
    public override bool GetVisible()
    {
        if (PlugCtl.PlugConfigDlg != null)                     // 1209
            return PlugCtl.PlugConfigDlg.Visible;              // 1210
        return false;                                          // 1212
    }

    /// <summary>
    /// 原文 1216-1238：
    /// <c>PlugConfigDlg.Visible := Value;</c> → 焦点三段 else-if（ShowHPLabel → NumberLable → JobAndLevel）
    /// → <c>if Value then</c> 六条清理（RefreshGJMagic / RefBindItemList / 两个 Edit 清空 /
    /// ComboGroup 归 0 / 两个按钮按 ScrollBox 选中态启用）。
    /// </summary>
    public override void SetVisible(bool Value)
    {
        if (PlugCtl.PlugConfigDlg == null) return;             // 1218
        PlugCtl.PlugConfigDlg.Visible = Value;                 // 1219
        if (PlugCtl.PlugCheckBoxShowHPLabel.Visible)           // 1220
            PlugCtl.PlugCheckBoxShowHPLabel.SetFocus();        // 1221
        else if (PlugCtl.PlugCheckBoxNumberLable.Visible)      // 1222
            PlugCtl.PlugCheckBoxNumberLable.SetFocus();        // 1223
        else if (PlugCtl.PlugCheckBoxJobAndLevel.Visible)      // 1224
            PlugCtl.PlugCheckBoxJobAndLevel.SetFocus();        // 1225

        if (Value)                                             // 1227
        {
            RefreshGJMagic();                                  // 1228
            RefBindItemList();                                 // 1229

            PlugCtl.PlugEditItemName.Text = "";                // 1231
            PlugCtl.PlugEditUnbindName.Text = "";              // 1232
            PlugCtl.PlugComboGroupUnBindItem.ItemIndex = 0;    // 1233
            PlugCtl.PlugBtnUnbindItemDel.Enabled = PlugCtl.PlugScrollBoxUnbindItems.ItemIndex >= 0;   // 1234
            PlugCtl.PlugBtnUnbindItemEdit.Enabled = PlugCtl.PlugScrollBoxUnbindItems.ItemIndex >= 0;  // 1235
        }
    }

    /// <summary>原文 1240-1243。</summary>
    public override bool GetEnabled() => FEnabled;

    /// <summary>原文 1245-1250：<c>FEnabled := Value; if not FEnabled then FLoadConfig := False;</c></summary>
    public override void SetEnabled(bool Value)
    {
        FEnabled = Value;                                      // 1247
        if (!FEnabled)                                         // 1248
            FLoadConfig = false;                               // 1249
    }

    /// <summary>原文 1252-1255。</summary>
    public override bool GetProtectEnabled() => FProtectEnabled;

    /// <summary>原文 1257-1262：置 True 时刷新 <c>FProtectEnabledTick</c>。</summary>
    public override void SetProtectEnabled(bool Value)
    {
        FProtectEnabled = Value;                               // 1259
        if (FProtectEnabled)                                   // 1260
            FProtectEnabledTick = MirConfigGlobalSeam.MyGetTickCount();   // 1261
    }

    /// <summary>原文 1264-1267：<c>Result := False; //HZQ 20230525</c>（**空实现**）。</summary>
    public override bool FormKeyDown(ref ushort Key, DelphiShiftState Shift) => false;

    /// <summary>原文 1269-1272：<c>Result := False; //HZQ 20230525</c>（**空实现**）。</summary>
    public override bool FormKeyPress(ref char Key) => false;

    /// <summary>原文 1274-1277：**空实现**。</summary>
    public override void RefreshMySelfAbil() { }

    /// <summary>原文 1279-1282：**空实现**。</summary>
    public override void RefreshMyHeroAbil() { }

    /// <summary>原文 1304-1307：**空实现**（英雄技能表在本单元未接线）。</summary>
    public override void RefreshMyHeroMagicList() { }

    // ================================================================================
    // MirConfigDlg.pas:1394-1422  页控件几何 / 关闭 / 切页焦点
    // ================================================================================

    /// <summary>
    /// 原文 1394-1397：
    /// <c>IsRealArea := not ((X &gt;= PlugPageControlConfig.Width - 12) and (Y &lt;= PlugPageControlConfig.Height + 20));</c>
    /// **原文缺陷（登记不改）**：Y 判据用 <c>Height + 20</c>（下界外），与 X 判据的
    /// <c>Width - 12</c>（右上角关闭按钮区）不一致 —— 照抄（与 MirReturnConfigDlg 同一处缺陷）。
    /// </summary>
    public void PlugPageControlConfigInRealArea(int X, int Y, out bool IsRealArea)
    {
        IsRealArea = !((X >= PlugCtl.PlugPageControlConfig.Width - 12) &&
                       (Y <= PlugCtl.PlugPageControlConfig.Height + 20));
    }

    /// <summary>
    /// 原文 1399-1410：关窗 + <c>if FMemo.Visible then</c> 三段（存备注 → ShowViewNotes）。
    /// 与 <see cref="Close"/> 的尾巴**逐字相同**（原文复制粘贴）。
    /// </summary>
    public void PlugConfigDlgCloseClickEx()
    {
        if (PlugCtl.PlugConfigDlg != null)                     // 1401
            PlugCtl.PlugConfigDlg.Visible = false;             // 1402

        if (FMemoVisible)                                      // 1404
        {
            PlugCtl.PlugMemoConfigNotes.Lines.Assign(FMemo);   // 1405
            SaveNotesFile();                                   // 1406

            ShowViewNotes();                                   // 1408
        }
    }

    /// <summary>
    /// 原文 1412-1422：仅当 <c>ActivePageIndex = 0</c> 时做焦点三段 else-if。
    /// </summary>
    public void PlugPageControlConfigActivePageChange()
    {
        if (PlugCtl.PlugPageControlConfig.ActivePageIndex == 0)   // 1414
        {
            if (PlugCtl.PlugCheckBoxShowHPLabel.Visible)          // 1416
                PlugCtl.PlugCheckBoxShowHPLabel.SetFocus();       // 1417
            else if (PlugCtl.PlugCheckBoxNumberLable.Visible)     // 1418
                PlugCtl.PlugCheckBoxNumberLable.SetFocus();       // 1419
            else if (PlugCtl.PlugCheckBoxJobAndLevel.Visible)     // 1420
                PlugCtl.PlugCheckBoxJobAndLevel.SetFocus();       // 1421
        }
    }
}
