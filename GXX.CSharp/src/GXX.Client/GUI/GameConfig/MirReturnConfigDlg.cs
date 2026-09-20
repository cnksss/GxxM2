// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 覆盖范围（原文行号）：
//   1-7      interface uses
//   9-526    TMirReturnConfigDlg 类声明（320 个 DxComponent 控件字段 + 15 个 private 字段 + 方法声明）
//   533-637  implementation uses + TConfig record / pTConfig
//   640-787  全局 g_Config:TConfig 的逐字段初值
//   789-869  Create / Destroy
//   871-961  访问器 GetType / Get+SetConfigChecked / Open / Close / Get+SetVisible /
//            Get+SetEnabled / Get+SetProtectEnabled / FormKeyDown / FormKeyPress
//   963-1034 Refresh* / PlugPageControlConfigInRealArea / PlugConfigDlgCloseClickEx /
//            PlugPageControlConfigActivePageChange / Logout
//   1036-1080 LoadHelpFile
//   1082-1158 ClearShowItem / RefShowItem
//   4191-4207 NumberSort_1
//   4479-4484 CanFilterExp
//   5134-5147 CheckBossNameExists
//   5421-5434 CheckGJMonNameExists
//   4486-5070 LoadConfigFile / SaveConfigFile
//   2139-2260、2262-2545、2546-2556、5700-5705、5706-5767  控件取值→g_Config 写回
//   5842-5941 DCheckBox*PercentClick
//   4454-4477 AutoUseMagic
//   3441-3758 AutoUseItem / Eat*Item
//   3759-4190 AutoProtect / DuraWarning
//   4209-4452 DamageHPUseItem / DamageMPUseItem
//   3374-3440 Struck / HealthChange
//   2564-3274 LoadClientConfig / Initialize / Finalize / Logon / LoadConfig / Run / RefActorList
//   3275-3373 GetShowItem / FindShowItem / FindHintItem / FindPickItem / HintItem
//   5116-5251 Boss 列表（DMemoBossListClick/Check*/Add/Del/Modify/SaveOrLoadBossList）
//   5253-5398 特殊物品自定义（DEditSpecialColorChange/DBtnDiy*）
//   5399-5537 挂机怪物名单（DBtnGJPageControlClick/DMemoGJMonListClick/Check*/Add/Del/Edit/SaveOrLoadGJMonList）
//   5539-5698 SaveOrLoadGJMagicList1 / SaveOrLoadGJMagicList2
//   5776-5841 RefreshGJMagic
//   5942-5954 OnChangedVolumePosition / OnChanggingVolumePosition
//
// 未覆盖（需 DxComponent 自绘控件树 / ClMain 真实 frmMain，已在交付报告逐条登记）：
//   1005-1007 PlugPageControlConfigInRealArea 的几何判定已接缝化
//   1159-2138 物品过滤页（ListViewItemClick / DLabelDefaultItemClick / DEditSearchItemChange /
//             DComboBoxItemStdModeSelect / DComboBoxColorShow / CheckBoxClickEx / RefUseItemConfig* /
//             RefKeyBoardConfig / RefConfig）
//   2571-2902 LoadClientConfig 的控件构建段
//   2903-3274 Initialize 的控件构建段
//   5071-5133 快捷键标签（DLabelKeyBoardKeyDown / DLabelKeyBoardMouseDown）
//   5735-5758 挂机页鼠标移动提示
//   5842-5941 中依赖控件 Selected 状态的判断
// 见 docs/并行报告-p5-client-mirreturn.md。
//
// 移植纪律：1:1 逐字（类型名/方法名/字段名/常量名/分支顺序/边界行为），原文笔误与冗余保留并注释
// `// 原文如此（MirReturnConfigDlg.pas:<行>）`。所有外部依赖走接缝，绝不复制第二份实现。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// <c>TMirReturnConfigDlg = class(TGameConfigObject)</c>（MirReturnConfigDlg.pas:9）。
///
/// 与同族的 <see cref="TMirsConfigDlg"/>（MirsConfigDlg.pas）保持**同一套接缝风格**：
/// 控件树收敛为 <see cref="IMirReturnConfigDlgControls"/>（属性名与原文控件字段逐字一致），
/// 全局量收敛到 <see cref="MirReturnGlobalSeam"/>。
///
/// 本类为 partial，按原文方法族分文件：
/// <list type="bullet">
/// <item><c>MirReturnConfigDlg.cs</c>                TConfig / g_Config / Create / 访问器 / 空实现</item>
/// <item><c>MirReturnConfigDlg.Controls.cs</c>       控件→g_Config 写回（Change/Click 处理器）</item>
/// <item><c>MirReturnConfigDlg.ConfigFile.cs</c>     LoadConfigFile / SaveConfigFile</item>
/// <item><c>MirReturnConfigDlg.Lists.cs</c>          Boss/挂机怪物/挂机技能 名单与文件</item>
/// <item><c>MirReturnConfigDlg.Auto.cs</c>           自动吃药/保护/持久警告/伤害喝药</item>
/// <item><c>MirReturnConfigDlg.Lifecycle.cs</c>      LoadClientConfig / Initialize / 查询转发</item>
/// </list>
/// </summary>
public partial class TMirReturnConfigDlg : TGameConfigObject
{
    // ================================================================================
    // MirReturnConfigDlg.pas:533-638  TConfig record / pTConfig
    // 原文为 record，托管侧用 class 表达（与 TMirsConfigDlg.TConfig 同一取舍：
    // 配置量大，且 g_Config 是单元级单例、需按引用改字段）。
    // 数组维度严格按原文：0..4（5 档用药模式）、0..8（9 种超级药）。
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

        public readonly bool[] RenewMPIsAutos = new bool[5];
        public readonly int[] RenewMPPercents = new int[5];
        public readonly int[] RenewMPTimes = new int[5];
        public readonly uint[] RenewMPTicks = new uint[5];

        public readonly bool[] RenewSpecialHPIsAutos = new bool[5];
        public readonly int[] RenewSpecialHPPercents = new int[5];
        public readonly int[] RenewSpecialHPTimes = new int[5];
        public readonly uint[] RenewSpecialHPTicks = new uint[5];

        public readonly bool[] RenewSpecialMPIsAutos = new bool[5];
        public readonly int[] RenewSpecialMPPercents = new int[5];
        public readonly int[] RenewSpecialMPTimes = new int[5];
        public readonly uint[] RenewSpecialMPTicks = new uint[5];

        public readonly bool[] UseSuperMedicas = new bool[5];
        public readonly string[] SuperMedicaItemNames = new string[9];
        public readonly bool[,] SuperMedicaUses = new bool[5, 9];
        public readonly int[,] SuperMedicaHPs = new int[5, 9];
        public readonly int[,] SuperMedicaHPTimes = new int[5, 9];
        public readonly int[,] SuperMedicaHPTicks = new int[5, 9];

        public readonly int[,] SuperMedicaMPs = new int[5, 9];
        public readonly int[,] SuperMedicaMPTimes = new int[5, 9];
        public readonly int[,] SuperMedicaMPTicks = new int[5, 9];

        public readonly bool[] CheckDuraIsAutos = new bool[5];
        public readonly int[] CheckDuraMin = new int[5];
        /// <summary>原文 <c>array[0..4] of string[20]</c>（短字符串）。托管侧用 string 承载，长度裁剪在写入点处理。</summary>
        public readonly string[] CheckDuraValue = new string[5];
        public readonly int[] CheckDuraTime = new int[5];
        public readonly uint[] CheckDuraCheckTicks = new uint[5];

        public int nHeroDodgeHPPercent;
        /// <summary>BOSS变色显示 piaoyun 2013-09-09（原文 <c>Byte</c>）</summary>
        public byte nColorShowEff;
        /// <summary>特殊物品颜色 piaoyun 2013-09-09（原文 <c>Byte</c>）</summary>
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
    }

    /// <summary>
    /// MirReturnConfigDlg.pas:640-787 的全局 <c>g_Config:TConfig</c> 初值（**逐字段照抄**）。
    /// 脚本从原文抽取后回读比对（见 docs/并行报告-p5-client-mirreturn.md 的抽取脚本记录）。
    ///
    /// 原文几处**不对称**初值（照抄，不"修正"）：
    /// <list type="bullet">
    /// <item><c>RenewMPTimes = (0,0,0,0,0)</c> 而同族的 <c>RenewHPTimes = (1000,...)</c>（687 vs 682）</item>
    /// <item><c>CheckHpCheckTimes = (1000,...)</c>、<c>CheckHpUseTimes = (10000,...)</c>（666/668）</item>
    /// <item><c>CheckDuraMin = (2,...)</c>、<c>CheckDuraTime = (10,...)</c>（770/772）</item>
    /// <item><c>nColorShowEff = 3</c>、<c>nSpecialColor = 249</c>（776/777）</item>
    /// <item><c>nGJNotRushMonRange = 7</c>、<c>nGJGroupAttackCount = 3</c>（785/786）</item>
    /// <item>774/775 的 <c>nHeroDodgeHPPercent: 0</c> —— 原文在 <c>CheckDuraCheckTicks</c> 之后、
    ///       <c>nColorShowEff</c> 之前，与 record 声明顺序一致</item>
    /// </list>
    /// 原文 641-787 未出现的字段（nRenewHeroHPTime/…/boRenewClosePercent 等）在 Delphi 里
    /// **不在有初值的 record 常量里**，故保持零值 —— 与 TMirsConfigDlg 同一处理。
    /// </summary>
    public static readonly TConfig g_Config = CreateDefaultConfig();

    private static TConfig CreateDefaultConfig()
    {
        var c = new TConfig
        {
            nFilterMinExp = 0,
            nAutoUseMagicTime = 0,
            dwAutoUseMagicTick = 0,
            // ============保护=================
            boRenewSpecialIsAuto = false,
            nRenewSpecialPercent = 0,
            nRenewSpecialTime = 0,

            boRenewBookIsAuto = false,
            nRenewBookPercent = 0,
            nRenewBookTime = 0,
            nRenewBookNowBookIndex = 0,
            sRenewBookNowBookItem = "",
            // ============药品==================

            MedicaMode = 0,                                // 0主体 1英雄 2战士副将 3法师副将 4道士副将

            nHeroDodgeHPPercent = 0,
            nColorShowEff = 3,                             // BOSS变色显示 piaoyun 2013-09-09
            nSpecialColor = 249,

            nGJPlayAttackOption = 0,
            nGJNoRedPoisonOption = 0,
            nGJNoBluePoisonOption = 0,
            nGJNoDuFuOption = 0,
            nGJBagFullOption = 0,

            nGJNotRushMonRange = 7,
            nGJGroupAttackCount = 3,
        };

        // ChkAutoPercents / ChkRenewAutoPercents / ChkSuperMedicaPercents —（659-661）全 False
        // ChkAutoPercents: (False,False,False,False,False)
        // ChkRenewAutoPercents: (False,False,False,False,False)
        // ChkSuperMedicaPercents: (False,False,False,False,False)

        // CheckHpIsAutos: (False,...)（663）
        // CheckHpPercents: (0,...)（664）
        // CheckHpValues: (0,...)（665）
        Fill(c.CheckHpCheckTimes, 1000, 1000, 1000, 1000, 1000);        // 666
        // CheckHpCheckTicks: (0,...)（667）
        Fill(c.CheckHpUseTimes, 10000, 10000, 10000, 10000, 10000);     // 668
        // CheckHpUseTicks: (0,...)（669）

        // CheckMpIsAutos: (False,...)（672）
        // CheckMpPercents: (0,...)（673）
        // CheckMpValues: (0,...)（674）
        Fill(c.CheckMpCheckTimes, 1000, 1000, 1000, 1000, 1000);        // 675
        // CheckMpCheckTicks: (0,...)（676）
        Fill(c.CheckMpUseTimes, 10000, 10000, 10000, 10000, 10000);     // 677
        // CheckMpUseTicks: (0,...)（678）

        // RenewHPIsAutos: (False,...)（680）
        Fill(c.RenewHPPercents, 10, 10, 10, 10, 10);                    // 681
        Fill(c.RenewHPTimes, 1000, 1000, 1000, 1000, 1000);             // 682
        // RenewHPTicks: (0,...)（683）

        // RenewMPIsAutos: (False,...)（685）
        Fill(c.RenewMPPercents, 10, 10, 10, 10, 10);                    // 686
        // RenewMPTimes: (0, 0, 0, 0, 0)（687）★ 原文如此：与 RenewHPTimes 不对称
        // RenewMPTicks: (0,...)（688）

        // RenewSpecialHPIsAutos: (False,...)（690）
        Fill(c.RenewSpecialHPPercents, 10, 10, 10, 10, 10);             // 691
        Fill(c.RenewSpecialHPTimes, 1000, 1000, 1000, 1000, 1000);      // 692
        // RenewSpecialHPTicks: (0,...)（693）

        // RenewSpecialMPIsAutos: (False,...)（695）
        Fill(c.RenewSpecialMPPercents, 10, 10, 10, 10, 10);             // 696
        Fill(c.RenewSpecialMPTimes, 1000, 1000, 1000, 1000, 1000);      // 697
        // RenewSpecialMPTicks: (0,...)（698）

        // UseSuperMedicas: (False,False,False,False,False)（700）

        // SuperMedicaItemNames（702-711）—— 9 个中文药名，逐字照抄
        string[] names = { "太阳水", "强效太阳水", "万年雪霜", "疗伤药", "疗伤药(任务)",
                           "强效万年雪霜", "强效疗伤药", "超级万年雪霜", "超级疗伤药" };
        for (int i = 0; i < names.Length; i++) c.SuperMedicaItemNames[i] = names[i];

        // SuperMedicaUses（713-719）/ SuperMedicaHPs（721-727）/ SuperMedicaHPTicks（737-743）/
        // SuperMedicaMPs（745-751）/ SuperMedicaMPTicks（761-766）—— 全 0/False
        Fill2(c.SuperMedicaHPTimes, 500);                               // 729-735
        Fill2(c.SuperMedicaMPTimes, 500);                               // 753-759

        // CheckDuraIsAutos: (False,...)（769）
        Fill(c.CheckDuraMin, 2, 2, 2, 2, 2);                            // 770
        // CheckDuraValue: ('', '', '', '', '')（771）
        Fill(c.CheckDuraTime, 10, 10, 10, 10, 10);                      // 772
        // CheckDuraCheckTicks: (0,...)（773）

        return c;
    }

    private static void Fill(uint[] a, params uint[] v) { for (int i = 0; i < v.Length; i++) a[i] = v[i]; }
    private static void Fill(int[] a, params int[] v) { for (int i = 0; i < v.Length; i++) a[i] = v[i]; }
    private static void Fill2(int[,] a, int value)
    {
        for (int i = 0; i <= 4; i++)
            for (int j = 0; j <= 8; j++)
                a[i, j] = value;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:329-345  private 字段（15 个）
    // ================================================================================
    protected bool FLoadControl;
    protected bool FLoadConfig;
    protected IntPtr FHandle;
    protected byte FScreenMode;
    protected TClientVersion FClientVersion;
    protected bool FWindowMode;

    protected bool FEnabled;
    // 长度 = HighOrdinal + 2：末尾多一格给 ckMovePick 的合成槽位
    // （见 MirReturnCheckedMap.ckMovePickSyntheticIndex，与 MirsConfigDlg 同一裁定）
    protected readonly bool[] FConfigCheckeds = new bool[TConfigCheckedBounds.HighOrdinal + 2];
    protected bool FInitializeed;

    //FProtectList: TStringList;（原文 341 整行被注释掉 —— 原文如此）
    protected uint FHintItemDuraTick;
    protected TClientConfig FClientConfig = new TClientConfig();
    protected bool FProtectEnabled;
    protected uint FProtectEnabledTick;

    /// <summary>
    /// 原文 <c>g_ClientConfig.dwPluginMinEatItemTime</c> 的快照
    /// （原文在 2218-2240 / 2318-2487 / 4670-4743 共 12 处夹紧里**直接读全局**）。
    /// 本字段在构造函数里从 <see cref="MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime"/> 取值。
    /// 【精确签名要求】请集成方在只读接缝 <c>Seams/ConfigSeams.cs:98</c> 的 <c>TClientConfig</c> 上补
    /// <c>public uint dwPluginMinEatItemTime = 500;</c>（原文 Grobal2.pas:5089/6436），
    /// 随后本字段即可改为直读 <c>ClientGlobalSeam.g_ClientConfig.dwPluginMinEatItemTime</c>。
    /// </summary>
    protected uint MinEatItemTimeFloor;

    /// <summary>接缝：原文的 DxComponent 控件树（MirReturnConfigDlg.pas:10-328）。</summary>
    protected IMirReturnConfigDlgControlsExt Plug;

    /// <summary>
    /// 原文 <c>FileItemDB</c>（MirReturnConfigDlg.pas:901/2699/3333/3340/3351/3362/3371/5273/5281/5284，
    /// 来自 implementation uses 的 <c>FilterItems</c> 单元）。
    /// 与 <see cref="TMirsConfigDlg"/> 同一接缝来源（<c>FilterItemsGlobal.g_FileItemDB</c>）。
    /// </summary>
    protected TFileItemDB FileItemDB => FilterItemsGlobal.g_FileItemDB;

    // ================================================================================
    // MirReturnConfigDlg.pas:789-860  constructor TMirReturnConfigDlg.Create
    // ================================================================================
    public TMirReturnConfigDlg(IMirReturnConfigDlgControlsExt controls = null)
    {
        Plug = controls ?? new MirReturnConfigDlgControlsStub();

        FEnabled = false;                                       // 791
        FProtectEnabled = true;                                 // 792
        FProtectEnabledTick = ConfigSeams.MyGetTickCount();     // 793
        FHandle = IntPtr.Zero;                                   // 794
        FScreenMode = 0;                                        // 795
        FClientVersion = TClientVersion.cvSerial;               // 796 ★ 原文是 cvSerial（Mirs 侧是 cvMirs）
        FWindowMode = true;                                     // 797
        FInitializeed = false;                                  // 798
        FLoadControl = false;                                   // 799
        FLoadConfig = false;                                    // 800
        // FillChar(FConfigCheckeds, SizeOf(FConfigCheckeds), 0)（801）
        for (int i = 0; i < FConfigCheckeds.Length; i++) FConfigCheckeds[i] = false;
        // FillChar(FClientConfig, SizeOf(FClientConfig), 0)（802）
        FClientConfig = new TClientConfig();
        MinEatItemTimeFloor = MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime;
        // 原文 802 的 FillChar 之后，g_ClientConfig 是本对象之外的全局；此处快照一次供
        // 6 处 Max() 夹紧使用（见 FClientConfig 字段注释与报告的接缝清单）。

        FHintItemDuraTick = ConfigSeams.MyGetTickCount();        // 804

        /*
        FProtectList := TStringList.Create;（806-816 整段被 {} 注释掉 —— 原文如此）
        FProtectList.Add('随机传送卷');
        FProtectList.Add('地牢逃脱卷');
        FProtectList.Add('回城卷');
        FProtectList.Add('行会回城卷');
        FProtectList.Add('盟重传送石');
        FProtectList.Add('比奇传送石');
        FProtectList.Add('随机传送石');
        FProtectList.Add('小退');
        */

        FConfigCheckeds[(int)TConfigChecked.ckShowHPLabel] = true;                       // 818
        FConfigCheckeds[(int)TConfigChecked.ckShowUserName] = false;                    // 819
        FConfigCheckeds[(int)TConfigChecked.ckMagicLock] = true;                         // 820
        FConfigCheckeds[(int)TConfigChecked.ckAutoOrderItem] = true;                    // 821
        FConfigCheckeds[(int)TConfigChecked.ckNotNeedShift] = true;                     // 822
        FConfigCheckeds[(int)TConfigChecked.ckAutoPickUpItem] = true;                   // 823
        FConfigCheckeds[(int)TConfigChecked.ckBGMusic] = true;                          // 824
        FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic] = true;                    // 825
        FConfigCheckeds[(int)TConfigChecked.ckNotParaly] = false;                       // 826

        FConfigCheckeds[(int)TConfigChecked.ckSmartLongHit] = false;                    // 828 刀刀刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartPosLongHit] = false;                 // 829 隔位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWalkLongHit] = false;                // 830 走位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWideHit] = false;                    // 831 智能半月
        FConfigCheckeds[(int)TConfigChecked.ckSmartFireHit] = false;                    // 832 自动烈火
        FConfigCheckeds[(int)TConfigChecked.ckSmartSwordHit] = false;                   // 833 逐日剑法
        FConfigCheckeds[(int)TConfigChecked.ckSmartCrsHit] = false;                     // 834 抱月刀 双龙斩
        FConfigCheckeds[(int)TConfigChecked.ckSmartTwnHit] = false;                     // 835 龙影剑法

        FConfigCheckeds[(int)TConfigChecked.ckHumAutoShield] = false;                   // 837 自动开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumStruckShield] = false;                 // 838 被攻击开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget] = true;        // 839 疾光电影锁定目标
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom] = false;             // 840 手动控制爆裂火焰
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind] = false;             // 841 手动控制冰咆哮
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower] = false;         // 842 手动控制流星火雨

        FConfigCheckeds[(int)MirReturnCheckedMap.ckAutoTakeOnItem] = false;             // 844 毒符互换

        FConfigCheckeds[(int)TConfigChecked.ckShowNpcName] = false;                    // 846
        FConfigCheckeds[(int)TConfigChecked.ckShowNpcHPLabel] = false;                 // 847
        FConfigCheckeds[(int)TConfigChecked.ckShowNGLabel] = false;                    // 848

        FConfigCheckeds[(int)TConfigChecked.ckNearHint] = true;                         // 850
        FConfigCheckeds[(int)TConfigChecked.ckAutoLock] = false;                        // 851
        FConfigCheckeds[(int)TConfigChecked.ckColorShow] = false;                       // 852
        FConfigCheckeds[(int)TConfigChecked.ckSpecialQuickFlashing] = false;            // 853
        FConfigCheckeds[(int)TConfigChecked.ckBlacklistHit] = false;                    // 854
        FConfigCheckeds[(int)TConfigChecked.ckFriendHit] = false;                       // 855
        FConfigCheckeds[(int)TConfigChecked.ckSceneShake] = false;                      // 856
        FConfigCheckeds[(int)TConfigChecked.ckAutoDownHorse] = false;                   // 857

        FConfigCheckeds[MirReturnCheckedMap.ckMovePickIndex] = false;                   // 859
    }

    /// <summary>
    /// 原文 <c>destructor TMirReturnConfigDlg.Destroy;</c>（MirReturnConfigDlg.pas:862-869）：
    /// <c>if FEnabled then SaveConfigFile; … inherited;</c>。
    /// 托管侧不能声明终结器（原文另有公开方法 <c>Finalize</c>），故实现为显式 <see cref="Destroy"/>。
    /// </summary>
    public void Destroy()
    {
        if (FEnabled)
            SaveConfigFile();

        //FProtectList.Free;（原文 867 —— 原文如此）
        // inherited;（868）→ 基类 TGameConfigObject 无析构逻辑
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:871-951  访问器
    // ================================================================================

    /// <summary>原文 871-874：<c>Result := ptDefault;</c></summary>
    public override TConfigDlgType GetType() => TConfigDlgType.ptDefault;

    /// <summary>
    /// 原文 876-879：直接返回 <c>FConfigCheckeds[Index]</c>，**无任何保护**
    /// （与 <c>GetVisible</c> 的 <c>PlugConfigDlg &lt;&gt; nil</c> 检查形成对照 —— 原文如此）。
    /// </summary>
    public override bool GetConfigChecked(TConfigChecked Index) => FConfigCheckeds[(int)Index];

    /// <summary>原文 881-888：值变化时才写并 <c>RefConfig</c>。</summary>
    public override void SetConfigChecked(TConfigChecked Index, bool Value)
    {
        if (FConfigCheckeds[(int)Index] != Value)
        {
            FConfigCheckeds[(int)Index] = Value;
            RefConfig();
        }
    }

    /// <summary>原文 890-893：**空实现**（原文如此）。</summary>
    public override void Open() { }

    /// <summary>
    /// 原文 895-902：
    /// <c>FLoadConfig := False; FEnabled := False; if PlugConfigDlg &lt;&gt; nil then PlugConfigDlg.Visible := False;
    /// FileItemDB.BackUp;</c>
    /// </summary>
    public override void Close()
    {
        FLoadConfig = false;                                       // 897
        FEnabled = false;                                          // 898
        if (Plug.PlugConfigDlg != null)                            // 899
            Plug.PlugConfigDlgVisible = false;                     // 900
        FileItemDB.BackUp();                                       // 901
    }

    /// <summary>
    /// 原文 904-908：
    /// <c>if PlugConfigDlg &lt;&gt; nil then Result := PlugConfigDlg.Visible;</c>
    /// **原文缺陷（本车道登记，不改语义）**：<c>Result</c> 未预置初值 ——
    /// <c>PlugConfigDlg = nil</c> 时返回的是**未定义值**（Delphi 里是栈垃圾）。
    /// 托管侧 bool 默认 false，等价于"取到 false 分支"，已注释。
    /// </summary>
    public override bool GetVisible()
    {
        bool Result = false;                                       // 原文如此（MirReturnConfigDlg.pas:904-908 未预置 Result）
        if (Plug.PlugConfigDlg != null)
            Result = Plug.PlugConfigDlgVisible;
        return Result;
    }

    /// <summary>
    /// 原文 910-927：
    /// <c>PlugConfigDlg.Visible := Value;</c> 然后**焦点三段 else-if**
    /// （ShowHPLabel → NumberLable → JobAndLevel 的 <c>Visible</c> + <c>SetFocus</c>），
    /// 最后 <c>if Value then RefreshGJMagic;</c>。
    /// </summary>
    public override void SetVisible(bool Value)
    {
        if (Plug.PlugConfigDlg != null)                            // 912
        {
            Plug.PlugConfigDlgVisible = Value;                     // 914
            if (Plug.PlugCheckBoxShowHPLabelVisible)               // 915
                Plug.PlugCheckBoxShowHPLabelSetFocus();            // 916
            else if (Plug.PlugCheckBoxNumberLableVisible)          // 917
                Plug.PlugCheckBoxNumberLableSetFocus();            // 918
            else if (Plug.PlugCheckBoxJobAndLevelVisible)          // 919
                Plug.PlugCheckBoxJobAndLevelSetFocus();            // 920

            if (Value)                                             // 922
            {
                RefreshGJMagic();                                  // 924
            }
        }
    }

    /// <summary>原文 929-932。</summary>
    public override bool GetEnabled() => FEnabled;

    /// <summary>原文 934-939：<c>FEnabled := Value; if not FEnabled then FLoadConfig := False;</c></summary>
    public override void SetEnabled(bool Value)
    {
        FEnabled = Value;
        if (!FEnabled)
            FLoadConfig = false;
    }

    /// <summary>原文 941-944。</summary>
    public override bool GetProtectEnabled() => FProtectEnabled;

    /// <summary>原文 946-951：置 True 时刷新 <c>FProtectEnabledTick</c>。</summary>
    public override void SetProtectEnabled(bool Value)
    {
        FProtectEnabled = Value;
        if (FProtectEnabled)
            FProtectEnabledTick = ConfigSeams.MyGetTickCount();
    }

    /// <summary>原文 953-956：**空实现**（原文如此，无 <c>Result</c> 赋值）。</summary>
    public override bool FormKeyDown(ref ushort Key, DelphiShiftState Shift) => false;

    /// <summary>原文 958-961：**空实现**。</summary>
    public override bool FormKeyPress(ref char Key) => false;

    /// <summary>原文 963-966：**空实现**。</summary>
    public override void RefreshMySelfAbil() { }

    /// <summary>原文 968-971：**空实现**。</summary>
    public override void RefreshMyHeroAbil() { }

    /// <summary>原文 995-998：**空实现**。</summary>
    public override void RefreshMyHeroMagicList() { }

    /// <summary>原文 1000-1003：**空实现**。</summary>
    public override void RefreshUnBindItemList() { }

    /// <summary>原文 1005-1008（原文行 1005-1007 为方法体）：
    /// <c>IsRealArea := not ((X &gt;= PlugPageControlConfig.Width - 12) and (Y &lt;= PlugPageControlConfig.Height + 20));</c>
    /// **原文缺陷（登记不改）**：Y 判据用的是 <c>Height + 20</c>（下界外），
    /// 与 X 判据的 <c>Width - 12</c>（右上角关闭按钮区）不一致 —— 照抄。
    /// </summary>
    public void PlugPageControlConfigInRealArea(int X, int Y, out bool IsRealArea)
    {
        IsRealArea = !((X >= Plug.PlugPageControlConfigWidth - 12) &&
                       (Y <= Plug.PlugPageControlConfigHeight + 20));
    }

    /// <summary>原文 1010-1014：<c>if PlugConfigDlg &lt;&gt; nil then PlugConfigDlg.Visible := False;</c></summary>
    public void PlugConfigDlgCloseClickEx()
    {
        if (Plug.PlugConfigDlg != null)
            Plug.PlugConfigDlgVisible = false;
    }

    /// <summary>
    /// 原文 1016-1027：仅当 <c>ActivePageIndex = 0</c> 时做焦点三段 else-if
    /// （与 <see cref="SetVisible"/> 的五段体同形，但只在第 0 页执行）。
    /// </summary>
    public void PlugPageControlConfigActivePageChange()
    {
        if (Plug.PlugPageControlConfigActivePageIndex == 0)        // 1018
        {
            if (Plug.PlugCheckBoxShowHPLabelVisible)               // 1020
                Plug.PlugCheckBoxShowHPLabelSetFocus();            // 1021
            else if (Plug.PlugCheckBoxNumberLableVisible)          // 1022
                Plug.PlugCheckBoxNumberLableSetFocus();            // 1023
            else if (Plug.PlugCheckBoxJobAndLevelVisible)          // 1024
                Plug.PlugCheckBoxJobAndLevelSetFocus();            // 1025
        }
    }

    /// <summary>
    /// 原文 1029-1034：<c>FLoadConfig := False; FEnabled := False; SaveConfigFile;</c>
    /// 注意与 <see cref="Close"/> 的区别：Logout **无条件** SaveConfigFile，且**不**碰 PlugConfigDlg。
    /// </summary>
    public override void Logout()
    {
        FLoadConfig = false;                                       // 1031
        FEnabled = false;                                          // 1032
        SaveConfigFile();                                          // 1033
    }

    /// <summary>
    /// 原文 1082-1086：<c>PlugMemoConfig2.Clear; PlugMemoConfig2.ColCount := 6;</c>
    /// </summary>
    public override void ClearShowItem()
    {
        Plug.PlugMemoConfig2Clear();
        Plug.PlugMemoConfig2ColCount = 6;
    }

    /// <summary>
    /// 原文 4479-4484：
    /// <c>Result := False; if FConfigCheckeds[ckFilterExp] then Result := Exp &lt; g_Config.nFilterMinExp;</c>
    /// **边界行为照抄**：Delphi 侧 <c>Exp:LongWord</c> 与 <c>nFilterMinExp:Integer</c> 比较是
    /// **无符号 vs 有符号**，编译器按 Integer 提升 —— 若 <c>nFilterMinExp</c> 为负，
    /// 则 <c>LongWord &lt; 负Integer</c> 会走有符号比较。托管侧显式转 <c>long</c> 复刻同一语义
    /// （与 TMirsConfigDlg.CanFilterExp 同一处理）。
    /// </summary>
    public override bool CanFilterExp(uint Exp)
    {
        bool Result = false;                                       // 4481
        if (FConfigCheckeds[(int)TConfigChecked.ckFilterExp])      // 4482
            Result = (long)Exp < (long)g_Config.nFilterMinExp;     // 4483
        return Result;
    }

    /// <summary>
    /// 原文 <c>RefreshGJMagic</c> 的接缝（MirReturnConfigDlg.pas:5776-5841，见 Lifecycle 分片）。
    /// </summary>
    public void RefreshGJMagic() => RefreshGJMagicCore();
}
