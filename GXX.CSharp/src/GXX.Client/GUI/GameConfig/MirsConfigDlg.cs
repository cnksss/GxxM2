using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// Mirs/MirsConfigDlg.pas（3656 行）的 **配置数据层 + 可测行为层** 1:1 移植。
///
/// 本文件覆盖（原文行号）：
/// <list type="bullet">
/// <item>327-418  <c>TConfig</c> 记录（★ 全部字段与数组维度）</item>
/// <item>421-543  全局 <c>g_Config:TConfig</c> 的**逐字段初值**</item>
/// <item>545-603  <c>TMirsConfigDlg.Create</c> 的字段初值与 22 条勾选位默认值</item>
/// <item>613-679  访问器：GetType/Get+SetConfigChecked/Open/Close/Get+SetVisible/Get+SetEnabled/Get+SetProtectEnabled</item>
/// <item>681-753  空实现：FormKeyDown/FormKeyPress/RefreshMySelfAbil/RefreshMyHeroAbil/
///                 RefreshMyHeroMagicList/RefreshUnBindItemList/PlugPageControlConfigActivePageChange + Logout</item>
/// <item>731-740  PlugPageControlConfigInRealArea / PlugConfigDlgCloseClickEx</item>
/// <item>1078-1291 <c>CheckBoxClickEx</c>（50 条控件→配置位映射 + 3 处副作用）</item>
/// <item>1412-1487 <c>RefConfig</c>（配置位→控件回写，含 g_boSound/g_boBGSound/g_boRepeatBGSound 副作用）</item>
/// <item>2431-2472 <c>GetShowItem/FindShowItem/FindHintItem/FindPickItem/HintItem</c>（转发 FileItemDB）</item>
/// <item>2474-2536 <c>Struck/HealthChange</c>（伤害量计算与分派）</item>
/// </list>
///
/// 未覆盖部分（**已在交付报告逐条登记**）：全部 UI 构建/几何（2148-2381 Initialize）、
/// <c>LoadClientConfig</c>(1885-2147)、<c>LoadConfigFile</c>(3198-3426)、<c>SaveConfigFile</c>(3427-3656)、
/// <c>AutoProtect/AutoUseItem/AutoEatHPItem/…/DamageHPUseItem/DamageMPUseItem/AutoUseMagic/DuraWarning</c>
/// （2538-3197）、<c>LoadHelpFile</c>(755-799)、<c>RefShowItem/ListViewItemClick/DLabelDefaultItemClick/
/// DEditSearchItemChange/DComboBoxItemStdModeSelect</c>（801-1077）、<c>RefUseItemConfig*</c>（1293-1411）、
/// <c>RefConfig</c> 中未列出的 12 个 <c>PlugMemoConfig4Button*SkipAutoAttack</c> 相关行。
/// 原因：这些方法与 DxComponent 自绘控件树（TDxPageControl/TDxListView/TDxScrollBox/TDxLine…）
/// 深度耦合，而该控件库尚未移植。
/// </summary>
public class TMirsConfigDlg : TGameConfigObject
{
    // ================================================================================
    // MirsConfigDlg.pas:327-418  TConfig 记录
    // 原文为 record，托管侧用 class 表达（配置量大且需按 g_Config 全局单例改字段）。
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
    }

    /// <summary>
    /// MirsConfigDlg.pas:421-543 的全局 <c>g_Config:TConfig</c> 初值（**逐字段照抄**）。
    /// 注意原文几个不对称的初值：RenewMPTimes 是 (0,0,0,0,0) 而同族的 RenewHPTimes 是
    /// (1000,1000,1000,1000,1000)；RenewSpecialHPTicks/RenewSpecialMPTicks 用 0 而
    /// RenewHPTicks/RenewMPTicks 也用 0 —— 均照抄。
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
            MedicaMode = 0,                                                                          // 0主体 1英雄 2战士副将 3法师副将 4道士副将
        };
        Fill(c.CheckHpIsAutos, false, false, false, false, false);
        Fill(c.CheckHpPercents, 0, 0, 0, 0, 0);
        Fill(c.CheckHpValues, 0, 0, 0, 0, 0);
        Fill(c.CheckHpCheckTimes, 1000u, 1000u, 1000u, 1000u, 1000u);
        Fill(c.CheckHpCheckTicks, 0u, 0u, 0u, 0u, 0u);
        Fill(c.CheckHpUseTimes, 10000u, 10000u, 10000u, 10000u, 10000u);
        Fill(c.CheckHpUseTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.CheckMpIsAutos, false, false, false, false, false);
        Fill(c.CheckMpPercents, 0, 0, 0, 0, 0);
        Fill(c.CheckMpValues, 0, 0, 0, 0, 0);
        Fill(c.CheckMpCheckTimes, 1000u, 1000u, 1000u, 1000u, 1000u);
        Fill(c.CheckMpCheckTicks, 0u, 0u, 0u, 0u, 0u);
        Fill(c.CheckMpUseTimes, 10000u, 10000u, 10000u, 10000u, 10000u);
        Fill(c.CheckMpUseTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.RenewHPIsAutos, false, false, false, false, false);
        Fill(c.RenewHPPercents, 10, 10, 10, 10, 10);
        Fill(c.RenewHPTimes, 1000, 1000, 1000, 1000, 1000);
        Fill(c.RenewHPTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.RenewMPIsAutos, false, false, false, false, false);
        Fill(c.RenewMPPercents, 10, 10, 10, 10, 10);
        Fill(c.RenewMPTimes, 0, 0, 0, 0, 0);                  // 原文如此：其余同族为 1000，此处为 0
        Fill(c.RenewMPTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.RenewSpecialHPIsAutos, false, false, false, false, false);
        Fill(c.RenewSpecialHPPercents, 10, 10, 10, 10, 10);
        Fill(c.RenewSpecialHPTimes, 1000, 1000, 1000, 1000, 1000);
        Fill(c.RenewSpecialHPTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.RenewSpecialMPIsAutos, false, false, false, false, false);
        Fill(c.RenewSpecialMPPercents, 10, 10, 10, 10, 10);
        Fill(c.RenewSpecialMPTimes, 1000, 1000, 1000, 1000, 1000);
        Fill(c.RenewSpecialMPTicks, 0u, 0u, 0u, 0u, 0u);

        Fill(c.UseSuperMedicas, false, false, false, false, false);

        string[] names =
        {
            "太阳水", "强效太阳水", "万年雪霜", "疗伤药", "疗伤药(任务)",
            "强效万年雪霜", "强效疗伤药", "超级万年雪霜", "超级疗伤药"
        };
        for (int i = 0; i < 9; i++) c.SuperMedicaItemNames[i] = names[i];

        for (int m = 0; m < 5; m++)
            for (int i = 0; i < 9; i++)
            {
                c.SuperMedicaUses[m, i] = false;
                c.SuperMedicaHPs[m, i] = 0;
                c.SuperMedicaHPTimes[m, i] = 500;
                c.SuperMedicaHPTicks[m, i] = 0;
                c.SuperMedicaMPs[m, i] = 0;
                c.SuperMedicaMPTimes[m, i] = 500;
                c.SuperMedicaMPTicks[m, i] = 0;
            }
        return c;
    }

    private static void Fill(bool[] a, params bool[] v) { for (int i = 0; i < v.Length; i++) a[i] = v[i]; }
    private static void Fill(int[] a, params int[] v) { for (int i = 0; i < v.Length; i++) a[i] = v[i]; }
    private static void Fill(uint[] a, params uint[] v) { for (int i = 0; i < v.Length; i++) a[i] = v[i]; }

    // ================================================================================
    // MirsConfigDlg.pas:206-223（private 字段）+ 420 行的全局引用
    // ================================================================================
    protected bool FLoadControl;
    protected bool FLoadConfig;
    protected IntPtr FHandle;
    protected byte FScreenMode;
    protected TClientVersion FClientVersion;
    protected bool FWindowMode;

    protected bool FEnabled;
    // 长度 = HighOrdinal + 2：末尾多一格给 ckMovePick 的合成槽位（见 MirsConfigCheckedMap.ckMovePickSyntheticIndex）
    protected readonly bool[] FConfigCheckeds = new bool[TConfigCheckedBounds.HighOrdinal + 2];
    protected bool FInitializeed;

    protected uint FHintItemDuraTick;
    protected TClientConfig FClientConfig = new TClientConfig();
    protected bool FProtectEnabled;
    protected uint FProtectEnabledTick;

    /// <summary>接缝：原文的 DxComponent 控件树（MirsConfigDlg.pas:11-204）。</summary>
    protected IMirsConfigDlgControls Plug;

    /// <summary>原文 <c>FileItemDB</c>（MirsConfigDlg.pas:642/875/2433…，来自 FilterItems 单元）。</summary>
    protected TFileItemDB FileItemDB => FilterItemsGlobal.g_FileItemDB;

    /// <summary>原文 <c>g_MySelf</c> / <c>g_MyHero</c> 的伤害相关读取接缝。</summary>
    public Func<object, int> GetActorHP = _ => 0;
    /// <summary>接缝：<c>g_MyHero.m_Abil.MP</c>。</summary>
    public Func<object, int> GetActorMP = _ => 0;

    // ================================================================================
    // MirsConfigDlg.pas:545-603  constructor TMirsConfigDlg.Create
    // ================================================================================
    public TMirsConfigDlg(IMirsConfigDlgControls controls = null)
    {
        Plug = controls ?? new MirsConfigDlgControlsStub();

        FEnabled = false;
        FProtectEnabled = true;
        FProtectEnabledTick = ConfigSeams.MyGetTickCount();
        FHandle = IntPtr.Zero;

        FScreenMode = 0;
        FClientVersion = TClientVersion.cvMirs;
        FWindowMode = true;
        FLoadControl = false;
        FLoadConfig = false;
        FInitializeed = false;
        // 原文 FillChar(FConfigCheckeds, SizeOf(FConfigCheckeds), 0) / FillChar(FClientConfig, …, 0)
        for (int i = 0; i < FConfigCheckeds.Length; i++) FConfigCheckeds[i] = false;
        FHintItemDuraTick = ConfigSeams.MyGetTickCount();

        /*
        FProtectList := TStringList.Create;
        FProtectList.Add('随机传送卷');
        FProtectList.Add('地牢逃脱卷');
        FProtectList.Add('回城卷');
        FProtectList.Add('行会回城卷');
        FProtectList.Add('盟重传送石');
        FProtectList.Add('比奇传送石');
        FProtectList.Add('随机传送石');
        FProtectList.Add('小退');
        */

        FConfigCheckeds[(int)TConfigChecked.ckShowHPLabel] = true;
        FConfigCheckeds[(int)TConfigChecked.ckShowUserName] = true;
        FConfigCheckeds[(int)TConfigChecked.ckMagicLock] = true;
        FConfigCheckeds[(int)TConfigChecked.ckAutoOrderItem] = true;
        FConfigCheckeds[(int)TConfigChecked.ckNotNeedShift] = true;
        FConfigCheckeds[(int)TConfigChecked.ckAutoPickUpItem] = true;
        FConfigCheckeds[(int)TConfigChecked.ckBGMusic] = true;
        FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic] = true;
        FConfigCheckeds[(int)TConfigChecked.ckNotParaly] = false;

        FConfigCheckeds[(int)TConfigChecked.ckSmartLongHit] = false;                              // 刀刀刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartPosLongHit] = false;                           // 隔位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWalkLongHit] = false;                          // 走位刺杀
        FConfigCheckeds[(int)TConfigChecked.ckSmartWideHit] = false;                              // 智能半月
        FConfigCheckeds[(int)TConfigChecked.ckSmartFireHit] = false;                              // 自动烈火
        FConfigCheckeds[(int)TConfigChecked.ckSmartSwordHit] = false;                             // 逐日剑法
        FConfigCheckeds[(int)TConfigChecked.ckSmartCrsHit] = false;                               // 抱月刀 双龙斩
        FConfigCheckeds[(int)TConfigChecked.ckSmartTwnHit] = false;                               // 龙影剑法

        FConfigCheckeds[(int)TConfigChecked.ckHumAutoShield] = false;                             // 自动开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumStruckShield] = false;                           // 被攻击开盾
        FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget] = true;                  // 疾光电影锁定目标
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom] = false;                       // 手动控制爆裂火焰
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind] = false;                       // 手动控制冰咆哮
        FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower] = false;                   // 手动控制流星火雨

        FConfigCheckeds[(int)MirsConfigCheckedMap.ckAutoTakeOnItem] = false;                      // 毒符互换（原文 ckAutoTakeOnItem）

        FConfigCheckeds[MirsConfigCheckedMap.ckMovePickIndex] = false;                            // （原文 ckMovePick，无等价成员 → 合成槽位）
    }

    /// <summary>
    /// 原文 <c>destructor TMirsConfigDlg.Destroy;</c>（MirsConfigDlg.pas:605-610）：
    /// <c>if FEnabled then SaveConfigFile;</c>。
    /// 托管侧不能声明终结器（原文另有公开方法 Finalize），故实现为显式 <see cref="Destroy"/>。
    /// </summary>
    public void Destroy()
    {
        if (FEnabled)
            SaveConfigFile();
        //FProtectList.Free;
    }

    // ================================================================================
    // MirsConfigDlg.pas:613-679  访问器
    // ================================================================================

    /// <summary>原文 613-616。</summary>
    public override TConfigDlgType GetType() => TConfigDlgType.ptDefault;

    /// <summary>
    /// **原文缺陷（本车道仅登记，不改语义）**：MirsConfigDlg.pas:618-621 的
    /// <c>GetConfigChecked</c> 直接返回 <c>FConfigCheckeds[Index]</c>，
    /// 而 <c>GetEnabled</c>/<c>GetVisible</c> 是唯一会做 nil/初始化保护的；
    /// 同时 <c>ckAutoTakeOnItem</c>/<c>ckMovePick</c> 在检出枚举里不存在（见接缝映射）。
    /// </summary>
    public override bool GetConfigChecked(TConfigChecked Index) => FConfigCheckeds[(int)Index];

    /// <summary>原文 623-630：值变化时才写并 <c>RefConfig</c>。</summary>
    public override void SetConfigChecked(TConfigChecked Index, bool Value)
    {
        if (FConfigCheckeds[(int)Index] != Value)
        {
            FConfigCheckeds[(int)Index] = Value;
            RefConfig();
        }
    }

    /// <summary>原文 632-635：空实现。</summary>
    public override void Open()
    {
    }

    /// <summary>原文 637-643：<c>FEnabled=False</c>、隐藏窗体、<c>FileItemDB.BackUp</c>。</summary>
    public override void Close()
    {
        FEnabled = false;
        SetPlugConfigDlgVisible(false);   // 原文 if PlugConfigDlg <> nil then PlugConfigDlg.Visible := False;
        FileItemDB.BackUp();
    }

    /// <summary>原文 645-649：<c>if PlugConfigDlg &lt;&gt; nil then Result := PlugConfigDlg.Visible;</c>
    /// —— 注意 **PlugConfigDlg = nil 时 Result 保持未初始化（False）**，且**不设默认值**。</summary>
    public override bool GetVisible()
    {
        bool Result = false;
        if (PlugConfigDlg != null)
            Result = PlugConfigDlgVisible;
        return Result;
    }

    /// <summary>原文 651-655。</summary>
    public override void SetVisible(bool Value)
    {
        if (PlugConfigDlg != null)
            SetPlugConfigDlgVisible(Value);
    }

    /// <summary>原文 657-660。</summary>
    public override bool GetEnabled() => FEnabled;

    /// <summary>原文 662-667：置 False 时**同时清 FLoadConfig**。</summary>
    public override void SetEnabled(bool Value)
    {
        FEnabled = Value;
        if (!FEnabled)
            FLoadConfig = false;
    }

    /// <summary>原文 669-672。</summary>
    public override bool GetProtectEnabled() => FProtectEnabled;

    /// <summary>原文 674-679：置 True 时刷新 <c>FProtectEnabledTick</c>（置 False 不刷新）。</summary>
    public override void SetProtectEnabled(bool Value)
    {
        FProtectEnabled = Value;
        if (FProtectEnabled)
            FProtectEnabledTick = ConfigSeams.MyGetTickCount();
    }

    // ================================================================================
    // MirsConfigDlg.pas:681-729  空实现与其它
    // ================================================================================

    /// <summary>原文 681-684：空实现（返回 False）。</summary>
    public override bool FormKeyDown(ref ushort Key, DelphiShiftState Shift) => false;

    /// <summary>原文 686-689：空实现（返回 False）。</summary>
    public override bool FormKeyPress(ref char Key) => false;

    /// <summary>原文 691-694：空实现。</summary>
    public override void RefreshMySelfAbil() { }

    /// <summary>原文 696-699：空实现。</summary>
    public override void RefreshMyHeroAbil() { }

    /// <summary>原文 721-724：空实现（与 RefreshMySelfMagicList 不同，英雄侧为空）。</summary>
    public override void RefreshMyHeroMagicList() { }

    /// <summary>原文 726-729：空实现。</summary>
    public override void RefreshUnBindItemList() { }

    /// <summary>原文 742-746：注释块 + 空实现。</summary>
    public override void RefActorList() { }

    /// <summary>
    /// 原文 MirsConfigDlg.pas **没有** 这个方法；基类 <see cref="TGameConfigObject.RefKeyboardConfig"/>
    /// 来自 GameConfigDlg.pas:209 的抽象声明（该声明在 Mirs 单元未实现，说明 Mirs 版对应的
    /// GameConfigDlg.pas 也缺这一项 —— 与"6 个 TConfigChecked 成员缺失"同源）。
    /// 托管侧必须实现以满足抽象契约，按 Mirs 单元"未涉及即空实现"的风格处理。
    /// </summary>
    public override void RefKeyboardConfig() { }

    /// <summary>原文 742-746 的 PlugPageControlConfigActivePageChange。</summary>
    public void PlugPageControlConfigActivePageChange() { }

    /// <summary>原文 748-753：<c>FLoadConfig := False; FEnabled := False; SaveConfigFile;</c>。</summary>
    public override void Logout()
    {
        FLoadConfig = false;
        FEnabled = false;
        SaveConfigFile();
    }

    /// <summary>
    /// 原文 731-734：<c>IsRealArea := not ((X &gt;= PlugPageControlConfig.Width - 12) and (Y &lt;= PlugPageControlConfig.Height + 20));</c>
    /// 控件尺寸经接缝读取。
    /// </summary>
    public void PlugPageControlConfigInRealArea(int X, int Y, out bool IsRealArea)
    {
        IsRealArea = !((X >= PlugPageControlConfigWidth - 12) && (Y <= PlugPageControlConfigHeight + 20));
    }

    /// <summary>原文 736-740：<c>if PlugConfigDlg &lt;&gt; nil then PlugConfigDlg.Visible := False;</c></summary>
    public void PlugConfigDlgCloseClickEx()
    {
        if (PlugConfigDlg != null)
            SetPlugConfigDlgVisible(false);
    }

    // ================================================================================
    // MirsConfigDlg.pas:1078-1291  CheckBoxClickEx
    // ================================================================================

    /// <summary>
    /// 控件名 → 配置位 的映射表（原文 50 条 else-if 链，逐条对照 MirsConfigDlg.pas 行号）。
    /// 用表驱动而非 50 个 if：**判定顺序、命中即停的语义与原文一致**
    /// （原文是 <c>if / else if</c> 链，第一个 <c>Sender</c> 相等即执行并跳过其余）。
    /// </summary>
    private static readonly (string Sender, int Index, string Line)[] CheckBoxTable =
    {
        ("PlugCheckBoxShowHPLabel",                (int)TConfigChecked.ckShowHPLabel,                "1081-1084"),
        ("PlugCheckBoxNumberLable",                (int)TConfigChecked.ckShowNumberLable,            "1085-1088"),
        ("PlugCheckBoxJobAndLevel",                (int)TConfigChecked.ckShowJobAndLevel,            "1089-1092"),
        ("PlugCheckBoxShowGreenHint",              (int)TConfigChecked.ckShowGreenHint,              "1093-1096"),
        ("PlugCheckBoxShowItemName",               (int)MirsConfigCheckedMap.ckShowItemName,         "1097-1100"),
        ("PlugCheckBoxShowFilterItem",             (int)MirsConfigCheckedMap.ckShowFilterItem,       "1101-1104"),
        ("PlugCheckBoxItemHint",                   (int)MirsConfigCheckedMap.ckItemHint,             "1105-1108"),
        ("PlugCheckBoxShowActorName",              (int)TConfigChecked.ckShowUserName,               "1109-1112"),
        ("PlugCheckBoxHideDescUserName",           (int)TConfigChecked.ckOnlyShowCharName,           "1113-1116"),
        ("PlugCheckBoxDuraWarning",                (int)TConfigChecked.ckDuraWarning,                "1117-1120"),
        ("PlugCheckBoxNoShift",                    (int)TConfigChecked.ckNotNeedShift,               "1121-1124"),
        ("PlugCheckBoxExpFilter",                  (int)TConfigChecked.ckFilterExp,                  "1125-1128"),
        ("PlugCheckBoxShowMimiMapDesc",            (int)TConfigChecked.ckShowMapDesc,                "1129-1132"),
        ("PlugCheckBoxNotParaly",                  (int)TConfigChecked.ckNotParaly,                  "1133-1136"),
        ("PlugCheckBoxShowHighlightHPLabel",       (int)TConfigChecked.ckShowHighlightHPLabel,       "1137-1140"),
        ("PlugCheckBoxShowHealthNumber",           (int)TConfigChecked.ckShowMoveLable,              "1141-1144"),
        ("PlugCheckBoxHideGhost",                  (int)TConfigChecked.ckHideGhost,                  "1145-1148"),
        ("PlugCheckBoxHideHumEffect",              (int)TConfigChecked.ckHideHumEffect,              "1149-1152"),
        ("PlugCheckBoxHideWeaponEffect",           (int)TConfigChecked.ckHideWeaponEffect,           "1153-1156"),
        ("PlugCheckBoxShowMonName",                (int)TConfigChecked.ckShowMonName,                "1157-1160"),
        ("PlugCheckBoxAutoOrderItem",              (int)TConfigChecked.ckAutoOrderItem,              "1161-1164"),
        ("PlugCheckBoxMagicLock",                  (int)TConfigChecked.ckMagicLock,                  "1165-1168"),
        ("PlugCheckBoxSmartLongHit",               (int)TConfigChecked.ckSmartLongHit,               "1169-1172"),
        ("PlugCheckBoxSmartPosLongHit",            (int)TConfigChecked.ckSmartPosLongHit,            "1173-1176"),
        ("PlugCheckBoxSmartWalkLongHit",           (int)TConfigChecked.ckSmartWalkLongHit,           "1177-1180"),
        ("PlugCheckBoxSmartWideHit",               (int)TConfigChecked.ckSmartWideHit,               "1181-1184"),
        ("PlugCheckBoxSmartFireHit",               (int)TConfigChecked.ckSmartFireHit,               "1185-1188"),
        ("PlugCheckBoxSmartSwordHit",              (int)TConfigChecked.ckSmartSwordHit,              "1189-1192"),
        ("PlugCheckBoxSmartKTZHit",                (int)TConfigChecked.ckSmart66Hit,                 "1193-1196"),
        ("PlugCheckBoxAutoHideMode",               (int)TConfigChecked.ckAutoHideMode,               "1197-1200"),
        ("PlugCheckBoxAutoTakeOnItem",             (int)MirsConfigCheckedMap.ckAutoTakeOnItem,       "1201-1204"),
        ("PlugCheckBoxAutoCHangePoison",           (int)MirsConfigCheckedMap.ckAutoChangePoison,     "1205-1208"),
        ("PlugCheckBoxHumAutoShield",              (int)TConfigChecked.ckHumAutoShield,              "1209-1212"),
        ("PlugCheckBoxHumStruckShield",            (int)TConfigChecked.ckHumStruckShield,            "1213-1216"),
        ("PlugCheckBoxHeroAutoShield",             (int)TConfigChecked.ckHeroAutoShield,             "1217-1221"),
        ("PlugCheckBoxAssistantHeroAutoShield",    (int)TConfigChecked.ckAssistantHeroAutoShield,    "1222-1226"),
        ("PlugCheckBoxHumManuallySnowWind",        (int)TConfigChecked.ckHumManuallySnowWind,        "1227-1230"),
        ("PlugCheckBoxHumManuallyFireBoom",        (int)TConfigChecked.ckHumManuallyFireBoom,        "1231-1234"),
        ("PlugCheckBoxHumShootLightenLockTarget",  (int)TConfigChecked.ckHumShootLightenLockTarget,  "1235-1238"),
        ("PlugCheckBoxHumManuallyMeteorShower",    (int)TConfigChecked.ckHumManuallyMeteorShower,    "1239-1242"),
        ("PlugCheckBoxAutoMagic",                  (int)TConfigChecked.ckAutoUseMagic,               "1243-1246"),
        ("PlugCheckBoxUseKeyBoard",                (int)TConfigChecked.ckUseKeyBoard,                "1247-1250"),
        ("PlugCheckBoxUseSuperMedica",             (int)TConfigChecked.ckUseSuperMedica,             "1251-1255"),
        ("PlugCheckBoxDisableSelfStruck",          (int)TConfigChecked.ckDisableSelfStruck,          "1256-1259"),
        ("PlugCheckBoxSpeedSlow",                  (int)TConfigChecked.ckSpeedSlow,                  "1260-1263"),
        ("PlugCheckBoxAutoPickUpItem",             (int)TConfigChecked.ckAutoPickUpItem,             "1264-1267"),
        // 注意：PlugCheckBoxSound / PlugCheckBoxBGMusic / PlugCheckBoxRepeatBGMusic **不在此表**
        // —— 它们是原文 1268-1290 的三个"带副作用"分支，必须走到专门的代码，
        //    否则会被表里的 `FConfigCheckeds[...] = Checked; return;` 提前截断而丢掉副作用。
    };

    /// <summary>
    /// 原文 MirsConfigDlg.pas:1078-1291。
    ///
    /// 语义要点（逐字保留）：
    /// 1) 开头 <c>if FClientConfig.boNotCanUseClientConfig then Exit;</c> —— **服务端禁止时整个点击被忽略**；
    /// 2) 50 条 <c>else if 控件 = Sender</c>，命中即执行并结束；
    /// 3) 三处**副作用**：<c>PlugCheckBoxSound</c> 只改 <c>g_boSound</c> 并打聊天栏（"开"/"关"，注意
    ///    两个分支的颜色都是 clWhite/clBlack）；<c>PlugCheckBoxBGMusic</c> 额外同步 <c>g_boBGSound</c>；
    ///    <c>PlugCheckBoxRepeatBGMusic</c> 额外同步 <c>g_boRepeatBGSound</c> 并调 <c>SetRepeatBGSound</c>。
    /// </summary>
    public void CheckBoxClickEx(string senderControlName)
    {
        if (FClientConfig.boNotCanUseClientConfig) return;

        foreach (var row in CheckBoxTable)
        {
            if (row.Sender == senderControlName)
            {
                FConfigCheckeds[row.Index] = GetControlChecked(row.Sender);
                return;
            }
        }

        // 原文 1268-1279：PlugCheckBoxSound（唯一一个**不写 FConfigCheckeds** 的分支）
        if (senderControlName == "PlugCheckBoxSound")
        {
            MirsConfigGlobalSeam.g_boSound = GetControlChecked("PlugCheckBoxSound");
            if (MirsConfigGlobalSeam.g_boSound)
            {
                ChatBoardSeam.AddChatBoardString("[音效 开]", 0x00FFFFFF /*clWhite*/, 0x00000000 /*clBlack*/);
            }
            else
            {
                // 原文如此（MirsConfigDlg.pas:1277）：这里**没有分号**，是 Delphi 的合法表达式语句
                ChatBoardSeam.AddChatBoardString("[音效 关]", 0x00FFFFFF, 0x00000000);
            }
            return;
        }

        // 原文 1280-1290 的两个带同步副作用的分支
        if (senderControlName == "PlugCheckBoxBGMusic")
        {
            FConfigCheckeds[(int)TConfigChecked.ckBGMusic] = GetControlChecked("PlugCheckBoxBGMusic");
            MirsConfigGlobalSeam.g_boBGSound = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];
            return;
        }
        if (senderControlName == "PlugCheckBoxRepeatBGMusic")
        {
            FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic] = GetControlChecked("PlugCheckBoxRepeatBGMusic");
            MirsConfigGlobalSeam.g_boRepeatBGSound = FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic];
            MirsConfigGlobalSeam.SetRepeatBGSound(MirsConfigGlobalSeam.g_boRepeatBGSound);
            return;
        }
    }

    // ================================================================================
    // MirsConfigDlg.pas:1412-1487  RefConfig
    // ================================================================================

    /// <summary>
    /// 原文 MirsConfigDlg.pas:1412-1487。
    ///
    /// 语义要点（逐字保留）：
    /// 1) 开头 <c>if not FInitializeed then Exit;</c>；
    /// 2) 把 FConfigCheckeds 的位回写到各控件；
    /// 3) <c>PlugCheckBoxSound.Checked := g_boSound;</c>（读全局，不是配置位）；
    /// 4) <c>PlugCheckBoxBGMusic.Checked</c> 被赋值**两次**（1444 与 1424，均为同一个表达式，原文如此）；
    /// 5) 副作用：<c>g_boBGSound := FConfigCheckeds[ckBGMusic]</c>、
    ///    <c>g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic]</c>、<c>SetRepeatBGSound(...)</c>；
    /// 6) 末尾 <c>PlugMemoConfig4Button1..5.Checked := g_Config.MedicaMode = 0..4</c>
    ///    （5 个独立布尔比较，非单选逻辑）；
    /// 7) 最后调用 <c>RefUseItemConfig(g_Config.MedicaMode)</c>。
    /// </summary>
    public void RefConfig()
    {
        if (!FInitializeed) return;
        Plug.PlugCheckBoxShowHPLabel = FConfigCheckeds[(int)TConfigChecked.ckShowHPLabel];
        Plug.PlugCheckBoxNumberLable = FConfigCheckeds[(int)TConfigChecked.ckShowNumberLable];
        Plug.PlugCheckBoxJobAndLevel = FConfigCheckeds[(int)TConfigChecked.ckShowJobAndLevel];
        Plug.PlugCheckBoxShowGreenHint = FConfigCheckeds[(int)TConfigChecked.ckShowGreenHint];
        Plug.PlugCheckBoxShowItemName = FConfigCheckeds[(int)MirsConfigCheckedMap.ckShowItemName];
        Plug.PlugCheckBoxShowFilterItem = FConfigCheckeds[(int)MirsConfigCheckedMap.ckShowFilterItem];
        Plug.PlugCheckBoxItemHint = FConfigCheckeds[(int)MirsConfigCheckedMap.ckItemHint];
        Plug.PlugCheckBoxDisableSelfStruck = FConfigCheckeds[(int)TConfigChecked.ckDisableSelfStruck];
        Plug.PlugCheckBoxSpeedSlow = FConfigCheckeds[(int)TConfigChecked.ckSpeedSlow];
        Plug.PlugCheckBoxBGMusic = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];
        Plug.PlugCheckBoxAutoPickUpItem = FConfigCheckeds[(int)TConfigChecked.ckAutoPickUpItem];

        Plug.PlugCheckBoxShowActorName = FConfigCheckeds[(int)TConfigChecked.ckShowUserName];
        Plug.PlugCheckBoxHideDescUserName = FConfigCheckeds[(int)TConfigChecked.ckOnlyShowCharName];
        Plug.PlugCheckBoxDuraWarning = FConfigCheckeds[(int)TConfigChecked.ckDuraWarning];
        Plug.PlugCheckBoxNoShift = FConfigCheckeds[(int)TConfigChecked.ckNotNeedShift];
        Plug.PlugCheckBoxExpFilter = FConfigCheckeds[(int)TConfigChecked.ckFilterExp];
        Plug.PlugCheckBoxShowMimiMapDesc = FConfigCheckeds[(int)TConfigChecked.ckShowMapDesc];
        Plug.PlugCheckBoxShowHighlightHPLabel = FConfigCheckeds[(int)TConfigChecked.ckShowHighlightHPLabel];
        Plug.PlugCheckBoxShowHealthNumber = FConfigCheckeds[(int)TConfigChecked.ckShowMoveLable];

        Plug.PlugCheckBoxHideGhost = FConfigCheckeds[(int)TConfigChecked.ckHideGhost];
        Plug.PlugCheckBoxHideHumEffect = FConfigCheckeds[(int)TConfigChecked.ckHideHumEffect];
        Plug.PlugCheckBoxHideWeaponEffect = FConfigCheckeds[(int)TConfigChecked.ckHideWeaponEffect];
        Plug.PlugCheckBoxShowMonName = FConfigCheckeds[(int)TConfigChecked.ckShowMonName];
        Plug.PlugCheckBoxAutoOrderItem = FConfigCheckeds[(int)TConfigChecked.ckAutoOrderItem];
        Plug.PlugCheckBoxMagicLock = FConfigCheckeds[(int)TConfigChecked.ckMagicLock];

        Plug.PlugCheckBoxSound = MirsConfigGlobalSeam.g_boSound;
        Plug.PlugCheckBoxBGMusic = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];              // 原文如此：第 2 次赋同一个表达式
        Plug.PlugCheckBoxRepeatBGMusic = FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic];
        Plug.PlugCheckBoxNotParaly = FConfigCheckeds[(int)TConfigChecked.ckNotParaly];

        MirsConfigGlobalSeam.g_boBGSound = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];
        MirsConfigGlobalSeam.g_boRepeatBGSound = FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic];
        MirsConfigGlobalSeam.SetRepeatBGSound(MirsConfigGlobalSeam.g_boRepeatBGSound);

        Plug.PlugCheckBoxSmartLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartLongHit];
        Plug.PlugCheckBoxSmartPosLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartPosLongHit];
        Plug.PlugCheckBoxSmartWalkLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartWalkLongHit];
        Plug.PlugCheckBoxSmartWideHit = FConfigCheckeds[(int)TConfigChecked.ckSmartWideHit];
        Plug.PlugCheckBoxSmartFireHit = FConfigCheckeds[(int)TConfigChecked.ckSmartFireHit];
        Plug.PlugCheckBoxSmartSwordHit = FConfigCheckeds[(int)TConfigChecked.ckSmartSwordHit];
        Plug.PlugCheckBoxSmartKTZHit = FConfigCheckeds[(int)TConfigChecked.ckSmart66Hit];
        Plug.PlugCheckBoxAutoHideMode = FConfigCheckeds[(int)TConfigChecked.ckAutoHideMode];
        Plug.PlugCheckBoxAutoTakeOnItem = FConfigCheckeds[(int)MirsConfigCheckedMap.ckAutoTakeOnItem];
        Plug.PlugCheckBoxAutoChangePoison = FConfigCheckeds[(int)MirsConfigCheckedMap.ckAutoChangePoison];
        Plug.PlugCheckBoxHumAutoShield = FConfigCheckeds[(int)TConfigChecked.ckHumAutoShield];

        Plug.PlugCheckBoxHumStruckShield = FConfigCheckeds[(int)TConfigChecked.ckHumStruckShield];
        Plug.PlugCheckBoxHeroAutoShield = FConfigCheckeds[(int)TConfigChecked.ckHeroAutoShield];
        Plug.PlugCheckBoxAssistantHeroAutoShield = FConfigCheckeds[(int)TConfigChecked.ckAssistantHeroAutoShield];
        Plug.PlugCheckBoxHumManuallySnowWind = FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind];
        Plug.PlugCheckBoxHumManuallyFireBoom = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom];
        Plug.PlugCheckBoxHumShootLightenLockTarget = FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget];
        Plug.PlugCheckBoxHumManuallyMeteorShower = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower];
        Plug.PlugCheckBoxAutoMagic = FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic];
        Plug.PlugCheckBoxUseKeyBoard = FConfigCheckeds[(int)TConfigChecked.ckUseKeyBoard];

        Plug.PlugCheckBoxUseSuperMedica = FConfigCheckeds[(int)TConfigChecked.ckUseSuperMedica];

        Plug.PlugCheckBoxAutoMagic = FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic];       // 原文如此：第 2 次赋同一个表达式

        Plug.PlugEditExpFilter = g_Config.nFilterMinExp;
        Plug.PlugEditAutoMagicTime = g_Config.nAutoUseMagicTime;

        Plug.PlugMemoConfig4Button1 = g_Config.MedicaMode == 0;
        Plug.PlugMemoConfig4Button2 = g_Config.MedicaMode == 1;
        Plug.PlugMemoConfig4Button3 = g_Config.MedicaMode == 2;
        Plug.PlugMemoConfig4Button4 = g_Config.MedicaMode == 3;
        Plug.PlugMemoConfig4Button5 = g_Config.MedicaMode == 4;
        RefUseItemConfig(g_Config.MedicaMode);
    }

    /// <summary>原文 <c>FConfigCheckeds:array[TConfigChecked] of Boolean</c>（MirsConfigDlg.pas:216）。</summary>
    public override bool[] ConfigCheckeds => FConfigCheckeds;

    // ================================================================================
    // MirsConfigDlg.pas:2431-2472  ShowItem 查询族（转发 FileItemDB）
    // ================================================================================

    /// <summary>原文 2431-2434。</summary>
    public override TShowItem GetShowItem(string ItemName) => FileItemDB.Find(ItemName);

    /// <summary>原文 2436-2445：命中则返回 <c>boShowName</c>（byte 与 0 比较），未命中 False。</summary>
    public override bool FindShowItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);
        if (ShowItem != null)
            return ShowItem.boShowName != 0;
        return false;
    }

    /// <summary>原文 2447-2456。</summary>
    public override bool FindHintItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);
        if (ShowItem != null)
            return ShowItem.boHintMsg != 0;
        return false;
    }

    /// <summary>原文 2458-2467。</summary>
    public override bool FindPickItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);
        if (ShowItem != null)
            return ShowItem.boPickup != 0;
        return false;
    }

    /// <summary>原文 2469-2472。</summary>
    public override void HintItem(string ItemName, int X, int Y)
    {
        FileItemDB.Hint(ItemName, X, Y);
    }

    // ================================================================================
    // MirsConfigDlg.pas:2474-2536  Struck / HealthChange
    // ================================================================================

    /// <summary>
    /// 原文 2474-2500。
    /// 语义要点：<c>FEnabled</c> 门禁 → <c>g_MySelf &lt;&gt; nil</c> → 分别对
    /// <c>Actor = g_MySelf</c>（nObj=0）与 <c>Actor = g_MyHero</c>（nObj=1）算伤害量
    /// <c>减血量 = 旧HP - HP</c>，只在 &gt; 0 时调 <c>DamageHPUseItem</c>。
    /// **英雄分支嵌在 g_MySelf &lt;&gt; nil 的 if 内**（原文如此：g_MySelf 为 nil 时英雄也不处理）。
    /// </summary>
    public override void Struck(object Actor, int HP, int MaxHP)
    {
        if (FEnabled)
        {
            if (ConfigShareSeam.g_MySelf != null)
            {
                if (ReferenceEquals(Actor, ConfigShareSeam.g_MySelf))
                {
                    int nDamage = GetActorHP(Actor) - HP;
                    if (nDamage > 0)
                        DamageHPUseItem(0, nDamage);
                }

                if (ConfigShareSeam.g_MyHero != null)
                {
                    if (ReferenceEquals(Actor, ConfigShareSeam.g_MyHero))
                    {
                        int nDamage = GetActorHP(Actor) - HP;
                        if (nDamage > 0)
                            DamageHPUseItem(1, nDamage);
                    }
                }
            }
        }
    }

    /// <summary>原文 2502-2536：在 Struck 基础上再加 MP 分支（HP 与 MP 各自独立判 &gt; 0）。</summary>
    public override void HealthChange(object Actor, int HP, int MP, int MaxHP)
    {
        if (FEnabled)
        {
            if (ConfigShareSeam.g_MySelf != null)
            {
                if (ReferenceEquals(Actor, ConfigShareSeam.g_MySelf))
                {
                    int nDamage = GetActorHP(Actor) - HP;
                    if (nDamage > 0)
                        DamageHPUseItem(0, nDamage);

                    nDamage = GetActorMP(Actor) - MP;
                    if (nDamage > 0)
                        DamageMPUseItem(0, nDamage);
                }

                if (ConfigShareSeam.g_MyHero != null)
                {
                    if (ReferenceEquals(Actor, ConfigShareSeam.g_MyHero))
                    {
                        int nDamage = GetActorHP(Actor) - HP;
                        if (nDamage > 0)
                            DamageHPUseItem(1, nDamage);

                        nDamage = GetActorMP(Actor) - MP;
                        if (nDamage > 0)
                            DamageMPUseItem(1, nDamage);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 接缝：MirsConfigDlg.pas:2983-3078 <c>DamageHPUseItem(nObj, nDamage)</c>（未移植，
    /// 依赖 g_ItemArr/FindHumHPItemIndex 等已在 ConfigShare 移植但完整自动用药流程含 UI 与计时）。
    /// </summary>
    public Action<int, int> DamageHPUseItem = (nObj, nDamage) => { };

    /// <summary>接缝：MirsConfigDlg.pas:3079-3166 <c>DamageMPUseItem(nObj, nDamage)</c>（未移植，同上）。</summary>
    public Action<int, int> DamageMPUseItem = (nObj, nDamage) => { };

    // ================================================================================
    // 接缝/未覆盖方法（保留原文签名，行为登记为未移植）
    // ================================================================================

    /// <summary>接缝：MirsConfigDlg.pas:1293-1411 <c>RefUseItemConfig</c>（用药配置控件回写，依赖 TDxScrollBox 控件树）。</summary>
    public void RefUseItemConfig(int nObj) => RefUseItemConfigSeam(nObj);

    /// <summary>接缝：<c>RefUseItemConfig</c> 的注入点。</summary>
    public Action<int> RefUseItemConfigSeam = nObj => { };

    /// <summary>接缝：MirsConfigDlg.pas:755-799 <c>LoadHelpFile</c>（TDxChatMemo/TDxLines 着色，未移植）。</summary>
    public void LoadHelpFile() => LoadHelpFileSeam();

    /// <summary>接缝：<c>LoadHelpFile</c> 的注入点。</summary>
    public Action LoadHelpFileSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:801-860 <c>ClearShowItem</c> / <c>RefShowItem</c>（TDxListView）。</summary>
    public override void ClearShowItem() => ClearShowItemSeam();

    /// <summary>接缝：<c>ClearShowItem</c> 的注入点。</summary>
    public Action ClearShowItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:807-860 <c>RefShowItem</c>。</summary>
    public override void RefShowItem() => RefShowItemSeam();

    /// <summary>接缝：<c>RefShowItem</c> 的注入点。</summary>
    public Action RefShowItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:701-719 <c>RefreshMySelfMagicList</c>（TDxComboBox，依赖 g_MagicList）。</summary>
    public override void RefreshMySelfMagicList() => RefreshMySelfMagicListSeam();

    /// <summary>接缝：<c>RefreshMySelfMagicList</c> 的注入点。</summary>
    public Action RefreshMySelfMagicListSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:1885-2147 <c>LoadClientConfig</c>（大量控件回写）。</summary>
    public override void LoadClientConfig(TClientConfig ClientConfig)
    {
        FClientConfig = ClientConfig;
        LoadClientConfigSeam(ClientConfig);
    }

    /// <summary>接缝：<c>LoadClientConfig</c> 的注入点。</summary>
    public Action<TClientConfig> LoadClientConfigSeam = _ => { };

    /// <summary>接缝：MirsConfigDlg.pas:2148-2381 <c>Initialize</c>（UI 装载与全部控件定位）。</summary>
    public override void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode)
    {
        FHandle = Handle;
        FScreenMode = ScreenMode;
        FClientVersion = ClientVersion;
        FWindowMode = WindowMode;
        InitializeSeam(Handle, ScreenMode, ClientVersion, WindowMode);
    }

    /// <summary>接缝：<c>Initialize</c> 的注入点。</summary>
    public Action<IntPtr, byte, TClientVersion, bool> InitializeSeam = (h, s, v, w) => { };

    /// <summary>接缝：MirsConfigDlg.pas:2382-2385 <c>Finalize</c>（释放控件）。</summary>
    public override void Finalize() => FinalizeSeam();

    /// <summary>接缝：<c>Finalize</c> 的注入点。</summary>
    public Action FinalizeSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2387-2390 <c>Logon</c>（设 <c>g_sPlugServerName</c>）。</summary>
    public override void Logon(string ServerName) => LogonSeam(ServerName);

    /// <summary>接缝：<c>Logon</c> 的注入点。</summary>
    public Action<string> LogonSeam = s => { };

    /// <summary>接缝：MirsConfigDlg.pas:2392-2425 <c>LoadConfig</c>（INI 读取 + 控件回写）。</summary>
    public override void LoadConfig(string CharName) => LoadConfigSeam(CharName);

    /// <summary>接缝：<c>LoadConfig</c> 的注入点。</summary>
    public Action<string> LoadConfigSeam = s => { };

    /// <summary>接缝：MirsConfigDlg.pas:2426-2429 <c>Run</c>（计时驱动自动用药，依赖 g_ItemArr 全流程）。</summary>
    public override void Run() => RunSeam();

    /// <summary>接缝：<c>Run</c> 的注入点。</summary>
    public Action RunSeam = () => { };

    /// <summary>
    /// 原文 MirsConfigDlg.pas:3191-3196。
    /// **逐字语义**：<c>Result := False; if FConfigCheckeds[ckFilterExp] then Result := Exp &lt; g_Config.nFilterMinExp;</c>
    /// —— 即"经验过滤"未勾选时**永远返回 False**（不过滤）；勾选后返回的是
    /// <c>Exp &lt; 下限</c>（**小于**才算"可过滤"），比较按 Delphi 的
    /// <c>LongWord vs Integer</c> 提升规则在 **Int64** 域完成（避免 uint 与负数的回绕）。
    /// </summary>
    public override bool CanFilterExp(uint Exp)
    {
        bool Result = false;
        if (FConfigCheckeds[(int)TConfigChecked.ckFilterExp])
            Result = (long)Exp < (long)g_Config.nFilterMinExp;
        return Result;
    }

    /// <summary>接缝：MirsConfigDlg.pas:3198-3426 <c>LoadConfigFile</c>（大段 INI 读取，~230 行）。</summary>
    public void LoadConfigFile() => LoadConfigFileSeam();

    /// <summary>接缝：<c>LoadConfigFile</c> 的注入点。</summary>
    public Action LoadConfigFileSeam = () => { };

    /// <summary>
    /// 接缝：MirsConfigDlg.pas:3427-3656 <c>SaveConfigFile</c>（大段 INI 落盘，~230 行）。
    /// **注意调用点非常多**（Destroy/Logout/Close 等），未移植时必须保持"不落盘"不会误伤调用方。
    /// </summary>
    public void SaveConfigFile() => SaveConfigFileSeam();

    /// <summary>接缝：<c>SaveConfigFile</c> 的注入点。</summary>
    public Action SaveConfigFileSeam = () => { };

    // ---- BOSS 列表（基类 GameConfigDlg.pas:223-225 声明，Mirs 单元未涉及 → 空实现） ----
    /// <summary>接缝：MirsConfigDlg.pas 未实现（基类声明见 GameConfigDlg.pas:223），按空实现处理。</summary>
    public override void AddToBossList(string sName) { }
    /// <summary>接缝：MirsConfigDlg.pas 未实现（基类声明见 GameConfigDlg.pas:224），按空实现处理。</summary>
    public override void RemoveFromBossList(string sName) { }
    /// <summary>接缝：MirsConfigDlg.pas 未实现（基类声明见 GameConfigDlg.pas:225，原文参数名拼写为 sNamt），按空实现处理。</summary>
    public override void AddOrRemoveBossList(string sNamt) { }

    /// <summary>接缝：MirsConfigDlg.pas:2779-2934 <c>AutoProtect</c>（~155 行自动保护）。</summary>
    public void AutoProtect() => AutoProtectSeam();

    /// <summary>接缝：<c>AutoProtect</c> 的注入点。</summary>
    public Action AutoProtectSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:3167-3190 <c>AutoUseMagic</c>。</summary>
    public void AutoUseMagic() => AutoUseMagicSeam();

    /// <summary>接缝：<c>AutoUseMagic</c> 的注入点。</summary>
    public Action AutoUseMagicSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2935-2982 <c>DuraWarning</c>。</summary>
    public void DuraWarning() => DuraWarningSeam();

    /// <summary>接缝：<c>DuraWarning</c> 的注入点。</summary>
    public Action DuraWarningSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2538-2554 <c>AutoUseItem</c>。</summary>
    public void AutoUseItem() => AutoUseItemSeam();

    /// <summary>接缝：<c>AutoUseItem</c> 的注入点。</summary>
    public Action AutoUseItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2555-2611 <c>AutoEatHPItem</c>。</summary>
    public void AutoEatHPItem() => AutoEatHPItemSeam();

    /// <summary>接缝：<c>AutoEatHPItem</c> 的注入点。</summary>
    public Action AutoEatHPItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2612-2668 <c>AutoEatMPItem</c>。</summary>
    public void AutoEatMPItem() => AutoEatMPItemSeam();

    /// <summary>接缝：<c>AutoEatMPItem</c> 的注入点。</summary>
    public Action AutoEatMPItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2669-2723 <c>AutoEatSpecialHPItem</c>。</summary>
    public void AutoEatSpecialHPItem() => AutoEatSpecialHPItemSeam();

    /// <summary>接缝：<c>AutoEatSpecialHPItem</c> 的注入点。</summary>
    public Action AutoEatSpecialHPItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:2724-2778 <c>AutoEatSpecialMPItem</c>。</summary>
    public void AutoEatSpecialMPItem() => AutoEatSpecialMPItemSeam();

    /// <summary>接缝：<c>AutoEatSpecialMPItem</c> 的注入点。</summary>
    public Action AutoEatSpecialMPItemSeam = () => { };

    /// <summary>接缝：MirsConfigDlg.pas:862-877 <c>ListViewItemClick</c>（TDxListView 列勾选写回 + SaveToFile）。</summary>
    public void ListViewItemClick(int ARow, int ACol, TShowItem showItem, bool viewItemChecked)
    {
        if (showItem != null)
        {
            switch (ACol)
            {
                case 0: break;
                case 1: showItem.boHintMsg = (byte)(viewItemChecked ? 1 : 0); break;
                case 2: showItem.boPickup = (byte)(viewItemChecked ? 1 : 0); break;
                case 3: showItem.boShowName = (byte)(viewItemChecked ? 1 : 0); break;
            }
            FileItemDB.SaveToFile();
        }
    }

    // ---- RefConfig 触及但本文件未导出为接缝的控件尺寸（PlugPageControlConfigInRealArea 用） ----
    /// <summary>接缝：<c>PlugPageControlConfig.Width</c>。</summary>
    public int PlugPageControlConfigWidth { get; set; }
    /// <summary>接缝：<c>PlugPageControlConfig.Height</c>。</summary>
    public int PlugPageControlConfigHeight { get; set; }
    /// <summary>接缝：<c>PlugConfigDlg</c> 是否存在（原文以 <c>&lt;&gt; nil</c> 判定）。</summary>
    public object PlugConfigDlg { get; set; } = new object();
    /// <summary>接缝：<c>PlugConfigDlg.Visible</c>。</summary>
    public bool PlugConfigDlgVisible { get; private set; }

    private void SetPlugConfigDlgVisible(bool v) => PlugConfigDlgVisible = v;

    /// <summary>按控件名读取 Checked（仅覆盖 CheckBoxTable 与 Sound/BGMusic/RepeatBGMusic）。</summary>
    private bool GetControlChecked(string name) => name switch
    {
        "PlugCheckBoxShowHPLabel" => Plug.PlugCheckBoxShowHPLabel,
        "PlugCheckBoxNumberLable" => Plug.PlugCheckBoxNumberLable,
        "PlugCheckBoxJobAndLevel" => Plug.PlugCheckBoxJobAndLevel,
        "PlugCheckBoxShowGreenHint" => Plug.PlugCheckBoxShowGreenHint,
        "PlugCheckBoxShowItemName" => Plug.PlugCheckBoxShowItemName,
        "PlugCheckBoxShowFilterItem" => Plug.PlugCheckBoxShowFilterItem,
        "PlugCheckBoxItemHint" => Plug.PlugCheckBoxItemHint,
        "PlugCheckBoxShowActorName" => Plug.PlugCheckBoxShowActorName,
        "PlugCheckBoxHideDescUserName" => Plug.PlugCheckBoxHideDescUserName,
        "PlugCheckBoxDuraWarning" => Plug.PlugCheckBoxDuraWarning,
        "PlugCheckBoxNoShift" => Plug.PlugCheckBoxNoShift,
        "PlugCheckBoxExpFilter" => Plug.PlugCheckBoxExpFilter,
        "PlugCheckBoxShowMimiMapDesc" => Plug.PlugCheckBoxShowMimiMapDesc,
        "PlugCheckBoxNotParaly" => Plug.PlugCheckBoxNotParaly,
        "PlugCheckBoxShowHighlightHPLabel" => Plug.PlugCheckBoxShowHighlightHPLabel,
        "PlugCheckBoxShowHealthNumber" => Plug.PlugCheckBoxShowHealthNumber,
        "PlugCheckBoxHideGhost" => Plug.PlugCheckBoxHideGhost,
        "PlugCheckBoxHideHumEffect" => Plug.PlugCheckBoxHideHumEffect,
        "PlugCheckBoxHideWeaponEffect" => Plug.PlugCheckBoxHideWeaponEffect,
        "PlugCheckBoxShowMonName" => Plug.PlugCheckBoxShowMonName,
        "PlugCheckBoxAutoOrderItem" => Plug.PlugCheckBoxAutoOrderItem,
        "PlugCheckBoxMagicLock" => Plug.PlugCheckBoxMagicLock,
        "PlugCheckBoxSmartLongHit" => Plug.PlugCheckBoxSmartLongHit,
        "PlugCheckBoxSmartPosLongHit" => Plug.PlugCheckBoxSmartPosLongHit,
        "PlugCheckBoxSmartWalkLongHit" => Plug.PlugCheckBoxSmartWalkLongHit,
        "PlugCheckBoxSmartWideHit" => Plug.PlugCheckBoxSmartWideHit,
        "PlugCheckBoxSmartFireHit" => Plug.PlugCheckBoxSmartFireHit,
        "PlugCheckBoxSmartSwordHit" => Plug.PlugCheckBoxSmartSwordHit,
        "PlugCheckBoxSmartKTZHit" => Plug.PlugCheckBoxSmartKTZHit,
        "PlugCheckBoxAutoHideMode" => Plug.PlugCheckBoxAutoHideMode,
        "PlugCheckBoxAutoTakeOnItem" => Plug.PlugCheckBoxAutoTakeOnItem,
        "PlugCheckBoxAutoCHangePoison" => Plug.PlugCheckBoxAutoChangePoison,
        "PlugCheckBoxHumAutoShield" => Plug.PlugCheckBoxHumAutoShield,
        "PlugCheckBoxHumStruckShield" => Plug.PlugCheckBoxHumStruckShield,
        "PlugCheckBoxHeroAutoShield" => Plug.PlugCheckBoxHeroAutoShield,
        "PlugCheckBoxAssistantHeroAutoShield" => Plug.PlugCheckBoxAssistantHeroAutoShield,
        "PlugCheckBoxHumManuallySnowWind" => Plug.PlugCheckBoxHumManuallySnowWind,
        "PlugCheckBoxHumManuallyFireBoom" => Plug.PlugCheckBoxHumManuallyFireBoom,
        "PlugCheckBoxHumShootLightenLockTarget" => Plug.PlugCheckBoxHumShootLightenLockTarget,
        "PlugCheckBoxHumManuallyMeteorShower" => Plug.PlugCheckBoxHumManuallyMeteorShower,
        "PlugCheckBoxAutoMagic" => Plug.PlugCheckBoxAutoMagic,
        "PlugCheckBoxUseKeyBoard" => Plug.PlugCheckBoxUseKeyBoard,
        "PlugCheckBoxUseSuperMedica" => Plug.PlugCheckBoxUseSuperMedica,
        "PlugCheckBoxDisableSelfStruck" => Plug.PlugCheckBoxDisableSelfStruck,
        "PlugCheckBoxSpeedSlow" => Plug.PlugCheckBoxSpeedSlow,
        "PlugCheckBoxAutoPickUpItem" => Plug.PlugCheckBoxAutoPickUpItem,
        "PlugCheckBoxSound" => Plug.PlugCheckBoxSound,
        "PlugCheckBoxBGMusic" => Plug.PlugCheckBoxBGMusic,
        "PlugCheckBoxRepeatBGMusic" => Plug.PlugCheckBoxRepeatBGMusic,
        _ => false,
    };
}
