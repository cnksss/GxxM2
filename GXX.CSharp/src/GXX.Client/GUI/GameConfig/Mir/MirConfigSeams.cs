// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
//
// 本文件承载该单元的**外部依赖接缝**（均在独占区 GXX.Client/GUI/GameConfig/Mir/** 内）：
//   MirItemSeam / MirItemArraySeam :: 原文 MShare.pas 的 g_UseItems / g_JewelryBoxItems /
//                                    g_GodBlessItems（及 Hero 三兄弟）的**只读取值面**
//   MirClientConfigSeam            :: 原文 MShare.pas 的 g_ClientConfig:TClientConfig
//   MirConfigClientSeam            :: 原文 MShare.pas 的 g_ConfigClient:TConfigClient
//   TConfigClientMir               :: 后者里本单元真正读到的 3 个成员
//   MirConfigGlobalSeam            :: 原文 MShare/ClMain/BassSound/HUtil32/ConfigShare 侧的
//                                    单元级全局量与工具函数
//
// ★ 为什么不并入既有的 Seams/ConfigSeams*.cs：
//   那两个文件**不在本车道的独占分区内**（硬性禁止写分区外文件），且既有
//   `TStdItemSeam` 缺 `AniCount`、`TClientItemSeam` 缺 `Dura/DuraMax`、
//   `TClientConfig`（Seams）缺本单元读的 80 个 `bo*` 与 `dwPluginMinEatItemTime`。
//   按 §14.2「不造第三份实现」+ 本车道不可改分区外文件，这里把**本单元需要的字段**
//   集中在 `MirClientConfigSeam` / `TConfigClientMir` 两个表达里，
//   报告 §偏离登记 D-P10-02/D-P10-03 给出"由集成方并入基线"的最小改法。
//
// ★ 不复制业务逻辑：本文件只有全局量与**已在原文字节级确定**的纯函数
//   （Format / IntToStr / StrToIntDef / SameText / Max / Min），
//   以及未移植单元（IniFiles / ClMain / BassSound / HUtil32）的**可注入委托**。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

// ================================================================================
// 物品数组取值面（原文 TUseItems / TJewelryBoxItems / TGodBlessItems 的元素）
// ================================================================================

/// <summary>
/// 接缝：<c>TClientItem</c>（Grobal2.pas:3706）在本单元用到的字段子集。
/// 逐条来自 usage 扫描：`.s.StdMode`(16) / `.s.AniCount`(6) / `.s.Shape`(4) /
/// `.s.Name`(62，原文写作 <c>.S.Name</c>，Delphi 大小写不敏感) /
/// `.Dura`(28) / `.DuraMax`(18)。
/// 类型严格按原文：StdMode:Byte、AniCount:Word、Shape:Word、Dura/DuraMax:Word。
/// </summary>
public interface IMirConfigItem
{
    /// <summary>原文 <c>.s.Name</c>（TStdItem.Name:string[ITEM_NAME_LEN]）。</summary>
    string Name { get; }
    /// <summary>原文 <c>.s.StdMode</c>（Byte）。</summary>
    byte StdMode { get; }
    /// <summary>原文 <c>.s.Shape</c>（Word）。</summary>
    ushort Shape { get; }
    /// <summary>原文 <c>.s.AniCount</c>（Word）。</summary>
    ushort AniCount { get; }
    /// <summary>原文 <c>.Dura</c>（Word，当前持久）。</summary>
    ushort Dura { get; }
    /// <summary>原文 <c>.DuraMax</c>（Word，最大持久）。</summary>
    ushort DuraMax { get; }
}

/// <summary>接缝元素的默认实现（纯内存，供测试与宿主注入）。</summary>
public sealed class MirConfigItem : IMirConfigItem
{
    public string Name { get; set; } = "";
    public byte StdMode { get; set; }
    public ushort Shape { get; set; }
    public ushort AniCount { get; set; }
    public ushort Dura { get; set; }
    public ushort DuraMax { get; set; } = 1000;
}

/// <summary>
/// 接缝：原文 <c>TUseItems = array[Low(THumanUseItems)..High(THumanUseItems)] of TClientItem</c>
/// 一类的**定长数组**（Delphi 的 <c>Low()/High()</c> 语义保留为 <see cref="Low"/>/<see cref="High"/>）。
/// DuraWarning 用 <c>J := Low(...); while J &lt;= High(...)</c> 遍历（5855-5860 等 6 处）。
/// </summary>
public sealed class MirItemArraySeam
{
    private readonly List<IMirConfigItem> _items = new List<IMirConfigItem>();
    public int Low => 0;
    public int High => _items.Count - 1;
    public IMirConfigItem this[int index]
    {
        get => _items[index];
        set
        {
            while (_items.Count <= index) _items.Add(new MirConfigItem());
            _items[index] = value;
        }
    }
    public void SetAll(IEnumerable<IMirConfigItem> items) { _items.Clear(); _items.AddRange(items); }
    public void Clear() => _items.Clear();
    public int Count => _items.Count;
}

// ================================================================================
// 全局配置（原文 MShare.pas 的 g_ClientConfig / g_ConfigClient）
// ================================================================================

/// <summary>
/// 接缝：原文 <c>g_ClientConfig:TClientConfig</c>（MShare.pas）在本单元读到的成员。
/// 逐条来自 usage 扫描（见报告 §对账表）：80 个 <c>bo*</c> + <c>dwPluginMinEatItemTime</c>。
/// 类型照原文：Delphi <c>Boolean</c> 一律用 <c>bool</c>（本单元只做 <c>if g_ClientConfig.boXxx</c> 判定）。
/// </summary>
public sealed class MirClientConfigSeam
{
    public uint dwPluginMinEatItemTime;

    public bool boShowHPLabel;
    public bool boShowNumberLable;
    public bool boShowMoveLable;
    public bool boShowJobAndLevel;
    public bool boFilterExp;
    public bool boShowGreenHint;
    public bool boShowUserName;
    public bool boOnlyShowCharName;
    public bool boAutoPickUpItem;
    public bool boNoCaton;
    public bool boDisableSelfStruck;
    public bool boSpeedSlow;
    public bool boMagicLock;
    public bool boPickupAll;
    public bool boAutoOrderItem;
    public bool boDuraWarning;
    public bool boNotNeedShift;
    public bool boShiftSwitch;
    public bool boHideGhost;
    public bool boHideHumEffect;
    public bool boHideWeaponEffect;
    public bool boShowMapDesc;
    public bool boShowHighlightHPLabel;
    public bool boAutoHideMode;
    public bool boHumAutoShield;
    public bool boHumStruckShield;
    public bool boSmartLongHit;
    public bool boSmartPosLongHit;
    public bool boSmartWalkLongHit;
    public bool boSmartWideHit;
    public bool boSmartFireHit;
    public bool boSmartSwordHit;
    public bool boSmartCrsHit;
    public bool boSmartTwnHit;
    public bool boBGMusic;
    public bool boRepeatBGMusic;
    public bool boShowMonName;
    public bool boShowNGLabel;
    public bool boNotParaly;
    public bool boHumManuallySnowWind;
    public bool boHumManuallyFireBoom;
    public bool boHumShootLightenLockTarget;
    public bool boHumManuallyMeteorShower;
    public bool boAutoCHangePoison;
    public bool boSmart66Hit;
    public bool boHeroAutoShield;
    public bool boAssistantHeroAutoShield;
    public bool boSceneShake;
    public bool boShowNpcName;
    public bool boShowNpcHPLabel;
    public bool boHideTitle;
    public bool boDisableChartMemoSize;
    public bool boItemCompare;
    public bool boVolume;
    public bool boContinueButchItem;
    public bool boDisableDeal;
    public bool boShowUpdateStatus;
    public bool boSimpleShowActor;
    public bool boSimpleShowHumanDress;
    public bool boSimpleShowHumanWeapon;
    public bool boHideItemEffect;
    public bool boBagFastItemCompare;
    public bool boHumManuallyFire;
    public bool boShowHPUnit;
    public bool boSimpleShowBB;
    public bool boShowDropValueItemEff;
    public bool boAutoGroupAttack;
    public bool boAutoGroupNoAttackMon;
    public bool boHumManuallyMove10Attack;
    public bool boAutoDetourPath;
    public bool boHideMonsterIcons;
    public bool boDimFireEffect;

    /// <summary>原文 <c>g_ClientConfig.boCloseBookProtect</c>（1315/1330 的书保护关闭位）。</summary>
    public bool boCloseBookProtect;
    /// <summary>原文 <c>g_ClientConfig.boCloseLogoutProtect</c>（1315/1343 的小退保护关闭位）。</summary>
    public bool boCloseLogoutProtect;
}

/// <summary>
/// 接缝：原文 <c>TClientConfig</c>（Grobal2.pas）在 MirConfigDlg 侧的**完整读取面**。
///
/// 字段 = <c>LoadClientConfig</c>（3851-4534）与 <c>RefConfig</c>（2764-2961）里出现的
/// 全部 <c>FClientConfig.*</c>（87 个 bo* + <c>UseSuperMedicaItemNames</c> +
/// <c>ClientConfigTabSheetVisibles</c>）—— **不是**推测性移植：
/// 每一个都在报告 §对账表里对应到原文行号。
///
/// ★ 类型对齐说明（集成方须知）：
///   基线 <c>TGameConfigObject.LoadClientConfig(TClientConfig)</c> 的形参是
///   <c>Seams.TClientConfig</c>（只保留 3 个字段的**最小面**），与本类型**不是同一个类**。
///   故 <see cref="TMirConfigDlg.LoadClientConfig"/> 的形参用基线类型，
///   再由 <see cref="From"/> 搬值 —— 见报告 §偏离登记 D-P10-03 的最小改法。
/// </summary>
public sealed class MirClientConfigFull
{
    /// <summary>原文 <c>boNotCanUseClientConfig</c>（LoadClientConfig 的总门禁）。</summary>
    public bool boNotCanUseClientConfig;
    /// <summary>原文 <c>UseSuperMedicaItemNames:array of string</c>（9 项超级药名，来自服务端配置）。</summary>
    public string[] UseSuperMedicaItemNames = Array.Empty<string>();
    /// <summary>原文 <c>ClientConfigTabSheetVisibles:array of array of Boolean</c>（页签可见性矩阵）。</summary>
    public bool[][] ClientConfigTabSheetVisibles = Array.Empty<bool[]>();

    public bool boShowHPLabel;
    public bool boShowNumberLable;
    public bool boShowMoveLable;
    public bool boShowJobAndLevel;
    public bool boFilterExp;
    public bool boShowGreenHint;
    public bool boShowUserName;
    public bool boOnlyShowCharName;
    public bool boAutoPickUpItem;
    public bool boNoCaton;
    public bool boDisableSelfStruck;
    public bool boSpeedSlow;
    public bool boMagicLock;
    public bool boPickupAll;
    public bool boAutoOrderItem;
    public bool boDuraWarning;
    public bool boNotNeedShift;
    public bool boShiftSwitch;
    public bool boHideGhost;
    public bool boHideHumEffect;
    public bool boHideWeaponEffect;
    public bool boShowMapDesc;
    public bool boShowHighlightHPLabel;
    public bool boAutoHideMode;
    public bool boSmart113Hit;
    public bool boSmartLongHit;
    public bool boSmartPosLongHit;
    public bool boSmartWalkLongHit;
    public bool boSmartWideHit;
    public bool boSmartFireHit;
    public bool boSmartSwordHit;
    public bool boSmartCrsHit;
    public bool boSmartTwnHit;
    public bool boSmart66Hit;
    public bool boSmartCustomHit1;
    public bool boSmartCustomHit2;
    public bool boSmartCustomHit3;
    public bool boSmartCustomHit4;
    public bool boSmartCustomHit5;
    public bool boSmartCustomHit6;
    public bool boSmartCustomHit7;
    public bool boSmartCustomHit8;
    public bool boHumAutoShield;
    public bool boHumStruckShield;
    public bool boHeroAutoShield;
    public bool boAssistantHeroAutoShield;
    public bool boHumShootLightenLockTarget;
    public bool boHumManuallyFireBoom;
    public bool boHumManuallySnowWind;
    public bool boHumManuallyMeteorShower;
    public bool boHumManuallyMove10Attack;
    public bool boHumManuallyFire;
    public bool boAutoCHangePoison;
    public bool boBGMusic;
    public bool boRepeatBGMusic;
    public bool boVolume;
    public bool boShowMonName;
    public bool boShowNpcName;
    public bool boShowNpcHPLabel;
    public bool boShowNGLabel;
    public bool boShowHPUnit;
    public bool boNotParaly;
    public bool boHideTitle;
    public bool boAutoOpenSpell;
    public bool boDisableChartMemoSize;
    public bool boItemCompare;
    public bool boBagFastItemCompare;
    public bool boContinueButchItem;
    public bool boDisableDeal;
    public bool boShowUpdateStatus;
    public bool boSimpleShowActor;
    public bool boSimpleShowHumanDress;
    public bool boSimpleShowHumanWeapon;
    public bool boHideItemEffect;
    public bool boShowDropValueItemEff;
    public bool boAutoGroupAttack;
    public bool boAutoGroupNoAttackMon;
    public bool boAutoContinueAttack;
    public bool boHideActorIcons;
    public bool boHideMonsterIcons;
    public bool boDimFireEffect;
    public bool boAutoDetourPath;
    public bool boSceneShake;

    /// <summary>
    /// 把基线最小面搬进来（原文 <c>FClientConfig := ClientConfig^;</c> —— 整记录赋值）。
    /// 基线只暴露 3 个字段，其余保持本类型默认值（报告 D-P10-03 登记了这条损失）。
    /// </summary>
    public static MirClientConfigFull From(TClientConfig src)
    {
        var c = new MirClientConfigFull();
        if (src != null)
            c.boNotCanUseClientConfig = src.boNotCanUseClientConfig;
        return c;
    }

    /// <summary>
    /// 从 <see cref="ClientGlobalSeam"/>（GameConfigDlgs.cs 持有的 <c>g_ConfigClient</c>）
    /// 把 <c>ClientConfigs[]</c> 桥接进 <see cref="TMirConfigDlg"/> 读的那份 <c>g_ConfigClient</c>。
    ///
    /// 原文只有一个 <c>g_ConfigClient</c>（MShare.pas:2180），托管侧因为
    /// <see cref="ClientGlobalSeam"/> 与 <c>MirConfigGlobalSeam</c> 分处两个文件而不能共用同一实例
    /// （前者在禁改区 GameConfigDlgs.cs，后者在只读的 Seams/ 下）。
    /// 集成方把两处合成一个后即可删除本方法（报告 D-P10-03）。
    /// </summary>
    public static void BridgeClientConfigsFromGlobalSeam()
    {
        // __BridgeFromGlobalSeam 由 GameConfigDlgs.cs 所在程序集侧提供（见 MirConfigGlobalSeam）
        MirConfigGlobalSeam.g_ConfigClient.ClientConfigs_Ex =
            global::GXX.Client.GUI.GameConfig.ClientGlobalSeam.ConfigClientConfigs ?? Array.Empty<bool>();
    }
}

/// <summary>
/// 接缝：原文 <c>g_ConfigClient:TConfigClient</c>（MShare.pas:2180）在本单元读到的成员
/// —— 只有 3 个：<c>boCustomConfigDlg</c>(x4)、<c>boCustomUI</c>(x4)、<c>ClientConfigs_Ex</c>(x1)。
/// 独立于 GUI/Mir/MirForms.cs 的 <c>TConfigClient</c>（那个不在本分区、且无这 3 个成员）。
/// </summary>
public sealed class TConfigClientMir
{
    /// <summary>原文 <c>boCustomConfigDlg:Boolean</c>（4530/4537 一带决定是否走自定义 UI）。</summary>
    public bool boCustomConfigDlg;
    /// <summary>原文 <c>boCustomUI:Boolean</c>。</summary>
    public bool boCustomUI;
    /// <summary>原文 <c>ClientConfigs_Ex:array of Boolean</c>（1154 取 <c>[0]</c> 作为耐久自动吃药默认值）。</summary>
    public bool[] ClientConfigs_Ex = Array.Empty<bool>();
}

// ================================================================================
// 单元级全局与外部函数接缝
// ================================================================================

/// <summary>
/// 接缝：MirConfigDlg.pas 依赖的单元级全局量与未移植单元的函数。
/// 命名与原文字节级一致（<c>g_</c> 前缀保留），便于与本 .pas 对照。
/// </summary>
public static class MirConfigGlobalSeam
{
    // ---- MShare.pas ----
    /// <summary>接缝：MShare.pas <c>g_ClientConfig:TClientConfig</c>。</summary>
    public static MirClientConfigSeam g_ClientConfig = new MirClientConfigSeam();
    /// <summary>接缝：MShare.pas:2180 <c>g_ConfigClient:TConfigClient</c>。</summary>
    public static TConfigClientMir g_ConfigClient = new TConfigClientMir();

    /// <summary>接缝：MShare.pas <c>g_UseItems:TUseItems</c>（人物装备位）。</summary>
    public static readonly MirItemArraySeam g_UseItems = new MirItemArraySeam();
    /// <summary>接缝：MShare.pas <c>g_HeroUseItems:TUseItems</c>（英雄装备位）。</summary>
    public static readonly MirItemArraySeam g_HeroUseItems = new MirItemArraySeam();
    /// <summary>接缝：MShare.pas <c>g_JewelryBoxItems:TJewelryBoxItems</c>（首饰盒）。</summary>
    public static readonly MirItemArraySeam g_JewelryBoxItems = new MirItemArraySeam();
    /// <summary>接缝：MShare.pas <c>g_HeroJewelryBoxItems:TJewelryBoxItems</c>。</summary>
    public static readonly MirItemArraySeam g_HeroJewelryBoxItems = new MirItemArraySeam();
    /// <summary>接缝：MShare.pas <c>g_GodBlessItems:TGodBlessItems</c>（神佑）。</summary>
    public static readonly MirItemArraySeam g_GodBlessItems = new MirItemArraySeam();
    /// <summary>接缝：MShare.pas <c>g_HeroGodBlessItems:TGodBlessItems</c>。</summary>
    public static readonly MirItemArraySeam g_HeroGodBlessItems = new MirItemArraySeam();

    /// <summary>接缝：MShare.pas <c>g_NGProtectItems:TStringList</c>（内挂保护物品名，1337-1371）。</summary>
    public static TStrings g_NGProtectItems = new TStrings();
    /// <summary>接缝：MShare.pas:2224 <c>g_CustomUnbindItemList:TList</c>（自定义绑定物品）。</summary>
    public static List<object> g_CustomUnbindItemList = new List<object>();

    /// <summary>接缝：MShare.pas <c>g_MagicList:TGList</c>（技能列表；元素是 pTClientMagic）。</summary>
    public static List<object> g_MagicList = new List<object>();
    /// <summary>接缝：MShare.pas <c>g_GJUseMagic1</c>（挂机技能表 1）。</summary>
    public static List<object> g_GJUseMagic1 = new List<object>();
    /// <summary>接缝：MShare.pas <c>g_GJUseMagic2</c>（挂机技能表 2）。</summary>
    public static List<object> g_GJUseMagic2 = new List<object>();
    /// <summary>接缝：MShare.pas <c>g_BossList:TStringList</c>。</summary>
    public static TStrings g_BossList = new TStrings();
    /// <summary>接缝：MShare.pas <c>g_GJMonList:TStringList</c>。</summary>
    public static TStrings g_GJMonList = new TStrings();

    /// <summary>接缝：MShare.pas <c>g_ConfigDlgUIStream:TMemoryStream</c>（4536-4565 的 UI 流）。</summary>
    public static object g_ConfigDlgUIStream;
    /// <summary>接缝：原文 TESTMODE=1 用的 <c>g_TestModeUIPath</c>（本托管侧走 TESTMODE=0，仅为 1:1 保留）。</summary>
    public static string g_TestModeUIPath = "";
    /// <summary>接缝：原文 TESTMODE=1 用的 <c>g_TestModeUseCustomUI</c>。</summary>
    public static bool g_TestModeUseCustomUI;

    /// <summary>接缝：FState.pas <c>g_DropItemsMgr.RefreshDrawList</c>（1670/1801）。</summary>
    public static Action RefreshDropItemsDrawList = () => { };

    // ---- BassSound.pas / SoundUtil.pas ----
    /// <summary>接缝：BassSound.pas <c>g_SoundVolume:Integer</c>（7990/7996）。</summary>
    public static int g_SoundVolume;
    /// <summary>接缝：SoundUtil.pas <c>g_boBGSound:Boolean</c>。</summary>
    public static bool g_boBGSound;
    /// <summary>接缝：SoundUtil.pas <c>g_boRepeatBGSound:Boolean</c>。</summary>
    public static bool g_boRepeatBGSound;
    /// <summary>接缝：SoundUtil.pas <c>g_sMapMusic:string</c>。</summary>
    public static string g_sMapMusic = "";

    // ---- ClMain.pas frmMain 的广播接缝 ----
    /// <summary>接缝：ClMain.pas <c>frmMain.SendPlugInConfig(v:Integer)</c>（7 处调用）。</summary>
    public static Action<int> SendPlugInConfig = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.GetRGB(value:Integer):TColor</c>（4 处）。</summary>
    public static Func<int, int> GetRGB = v => v;
    /// <summary>接缝：ClMain.pas <c>frmMain.nSpecialColor</c>（4534 一带）。</summary>
    public static Action<byte> SetFrmMainSpecialColor = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nColorShowEff</c>。</summary>
    public static Action<byte> SetFrmMainColorShowEff = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJPlayAttackOption</c>。</summary>
    public static Action<int> SetFrmMainGJPlayAttackOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoRedPoisonOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoRedPoisonOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoBluePoisonOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoBluePoisonOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoDuFuOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoDuFuOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJBagFullOption</c>。</summary>
    public static Action<int> SetFrmMainGJBagFullOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNotRushMonRange</c>。</summary>
    public static Action<int> SetFrmMainGJNotRushMonRange = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJGroupAttackCount</c>。</summary>
    public static Action<int> SetFrmMainGJGroupAttackCount = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.ChangePoisonCharm(Magic:pTClientMagic)</c>。</summary>
    public static Action<object> ChangePoisonCharm = _ => { };

    // ---- HUtil32.pas / SDK.pas / MShare.pas 的工具函数（已在原文里是纯函数，逐字复刻） ----
    /// <summary>原文 <c>DecodeResStr(s:string):string</c>（51 处；HUtil32.pas 的资源串解码）。</summary>
    public static Func<string, string> DecodeResStr = s => s ?? "";

    /// <summary>接缝：MShare.pas <c>MyGetTickCount:DWORD</c>（原文 external mmsyst 'timeGetTime'）。</summary>
    public static uint MyGetTickCount() => GXX.Core.Rtl.DelphiRTL.GetTickCount();

    /// <summary>原文 <c>SameText(a, b)</c>（SysUtils：大小写不敏感不区分区域）。</summary>
    public static bool SameText(string a, string b)
        => string.Compare(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase) == 0;

    /// <summary>原文 <c>CompareText(a, b)</c>（返回 &lt;0/0/&gt;0）。</summary>
    public static int CompareText(string a, string b)
        => string.Compare(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>原文 <c>Trim(s)</c>。</summary>
    public static string Trim(string s) => (s ?? "").Trim();

    /// <summary>原文 <c>IntToStr</c>。</summary>
    public static string IntToStr(int v) => v.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>原文 <c>StrToIntDef(s, def)</c>：失败返回 <paramref name="def"/>，**不抛异常**。</summary>
    public static int StrToIntDef(string s, int def)
        => int.TryParse((s ?? "").Trim(), System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : def;

    /// <summary>原文 <c>Round(x:Extended):Int64</c>（DuraWarning 的持久百分比换算）。</summary>
    public static int Round(double d) => (int)Math.Round(d, MidpointRounding.ToEven);

    /// <summary>原文 <c>Math.Max(a, b):Integer</c>。</summary>
    public static int Max(int a, int b) => a > b ? a : b;
    /// <summary>原文 <c>Math.Min(a, b):Integer</c>。</summary>
    public static int Min(int a, int b) => a < b ? a : b;
    /// <summary>原文 <c>Math.Max(a, b):Int64</c>（Integer/LongWord 混用时的提升形态）。</summary>
    public static long MaxL(long a, long b) => a > b ? a : b;
    /// <summary>原文 <c>Math.Min(a, b):Int64</c>。</summary>
    public static long MinL(long a, long b) => a < b ? a : b;

    /// <summary>
    /// 原文 <c>Format(fmt, args)</c> 的 <c>'%s'/'%d'</c> 最小实现（本单元 52 处全部是
    /// 单/双占位模板，见报告 §原文缺陷清单）。
    /// </summary>
    public static string Format1(string template, object arg)
        => ReplaceFirst(template, arg);
    /// <summary>原文 <c>Format</c> 两占位形态。</summary>
    public static string Format2(string template, object a, object b)
        => ReplaceFirst(ReplaceFirst(template, a), b);

    private static string ReplaceFirst(string template, object arg)
    {
        if (template == null) return "";
        int i = template.IndexOf("%s", StringComparison.Ordinal);
        if (i < 0) return template;
        return template.Substring(0, i) + Convert.ToString(arg, System.Globalization.CultureInfo.InvariantCulture)
             + template.Substring(i + 2);
    }

    /// <summary>原文 <c>IntToHex(v, n)</c>（本单元 0 处；保留以便配置文件的十六进制字段）。</summary>
    public static string IntToHex(int v, int digits) => v.ToString("X" + digits, System.Globalization.CultureInfo.InvariantCulture);

    // ---- IniFiles.pas / SysUtils 的文件系统接缝（原文 10 处 DirectoryExists/ForceDirectories/FileExists） ----
    /// <summary>原文 <c>ExtractFilePath(ParamStr(0))</c>（含结尾反斜杠）。</summary>
    public static string AppPath = "";
    /// <summary>原文 <c>DirectoryExists</c>。</summary>
    public static Func<string, bool> DirectoryExists = _ => false;
    /// <summary>原文 <c>ForceDirectories</c>。</summary>
    public static Action<string> ForceDirectories = _ => { };
    /// <summary>原文 <c>FileExists</c>。</summary>
    public static Func<string, bool> FileExists = _ => false;
    /// <summary>原文 <c>DeleteFile</c>。</summary>
    public static Action<string> DeleteFile = _ => { };
    /// <summary>原文 <c>TStringList.SaveToFile</c>（用于 Lines/名单落盘）。</summary>
    public static Action<string, IEnumerable<string>> SaveTextFile = (_, __) => { };
    /// <summary>原文 <c>TStringList.LoadFromFile</c>。</summary>
    public static Func<string, List<string>> LoadTextFile = _ => new List<string>();
    /// <summary>原文 <c>ExtractFilePath(path)</c>（含结尾反斜杠）。</summary>
    public static string ExtractFilePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        int i = path.LastIndexOfAny(new[] { '\\', '/' });
        return i < 0 ? "" : path.Substring(0, i + 1);
    }

    // ---- ConfigShare.pas ----
    /// <summary>原文 <c>ConfigShare.pas GetKeyDownStr(Key, Shift, IncludeFN)</c>（16 处）。</summary>
    public static Func<ushort, DelphiShiftState, bool, string> GetKeyDownStr = (k, s, f) => "";
    /// <summary>原文 <c>g_ShortcutKeys:TShortcutKeys</c>（ConfigShare.pas:31）。</summary>
    public static readonly TShortcutKeys g_ShortcutKeys = new TShortcutKeys();
    /// <summary>原文 <c>g_sPlugUserName</c>（ConfigShare.pas）。</summary>
    public static string g_sPlugUserName = "";
    /// <summary>原文 <c>g_sPlugServerName</c>（ConfigShare.pas）。</summary>
    public static string g_sPlugServerName = "";
    /// <summary>原文 <c>g_sSelfFilePath</c>（MShare.pas）。</summary>
    public static string g_sSelfFilePath = "";
    /// <summary>原文 <c>g_IsClientPickItemsChanged</c>（FilterItems.pas:38）。</summary>
    public static byte g_IsClientPickItemsChanged;

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        g_ClientConfig = new MirClientConfigSeam();
        g_ConfigClient = new TConfigClientMir();
        g_UseItems.Clear();
        g_HeroUseItems.Clear();
        g_JewelryBoxItems.Clear();
        g_HeroJewelryBoxItems.Clear();
        g_GodBlessItems.Clear();
        g_HeroGodBlessItems.Clear();
        g_NGProtectItems = new TStrings();
        g_CustomUnbindItemList = new List<object>();
        g_MagicList = new List<object>();
        g_GJUseMagic1 = new List<object>();
        g_GJUseMagic2 = new List<object>();
        g_BossList = new TStrings();
        g_GJMonList = new TStrings();
        g_ConfigDlgUIStream = null;
        g_SoundVolume = 0;
        g_boBGSound = false;
        g_boRepeatBGSound = false;
        g_sMapMusic = "";
        SendPlugInConfig = _ => { };
        GetRGB = v => v;
        SetFrmMainSpecialColor = _ => { };
        SetFrmMainColorShowEff = _ => { };
        SetFrmMainGJPlayAttackOption = _ => { };
        SetFrmMainGJNoRedPoisonOption = _ => { };
        SetFrmMainGJNoBluePoisonOption = _ => { };
        SetFrmMainGJNoDuFuOption = _ => { };
        SetFrmMainGJBagFullOption = _ => { };
        SetFrmMainGJNotRushMonRange = _ => { };
        SetFrmMainGJGroupAttackCount = _ => { };
        ChangePoisonCharm = _ => { };
        DecodeResStr = s => s ?? "";
        AppPath = "";
        DirectoryExists = _ => false;
        ForceDirectories = _ => { };
        FileExists = _ => false;
        DeleteFile = _ => { };
        SaveTextFile = (_, __) => { };
        LoadTextFile = _ => new List<string>();
        GetKeyDownStr = (k, s, f) => "";
        g_sPlugUserName = "";
        g_sPlugServerName = "";
        g_sSelfFilePath = "";
        g_IsClientPickItemsChanged = 0;
        RefreshDropItemsDrawList = () => { };
    }
}
