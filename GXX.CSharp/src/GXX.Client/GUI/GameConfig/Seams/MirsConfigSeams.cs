using System;

namespace GXX.Client.GUI.GameConfig.Seams;

/// <summary>
/// MirsConfigDlg.pas 独有的 <c>TConfigChecked</c> 成员。
///
/// **原文不一致（本车道发现）**：本检出的
/// <c>Source/Client-HGE/GameConfig/Common/GameConfigDlg.pas:26-181</c> 的
/// <c>TConfigChecked</c> **没有**下列 6 个成员，但 <c>MirsConfigDlg.pas</c> 直接使用它们
/// （例：<c>MirsConfigDlg.pas:1099 FConfigCheckeds[ckShowItemName] := ...</c>）。
/// 这说明 MirsConfigDlg.pas 是针对**另一版 GameConfigDlg.pas** 编写的
/// （同名枚举在套件内存在多个变体：Mir/MirJSY/MirReturn 各自引用的成员集合都不同）。
///
/// 处理方式（**不修改基线**、也不臆造枚举顺序）：
/// 在本车道内定义独立扩展枚举 <see cref="MirsConfigCheckedAlias"/>，
/// 其取值**按文档映射**到检出枚举的等价成员；映射关系全部显式登记在
/// <see cref="MirsConfigCheckedMap"/>，测试逐条断言。
/// 待 GameConfigDlg.pas 的正确版本确定后，把该扩展枚举替换为真实成员即可。
/// </summary>
public enum MirsConfigCheckedAlias
{
    /// <summary>原文 MirsConfigDlg.pas:1099/1419。映射依据：同名控件 PlugCheckBoxShowItemName 与 ShowMonName 同为"显示物品/怪物名"。</summary>
    ckShowItemName = 0,
    /// <summary>原文 MirsConfigDlg.pas:1103/1420。映射依据：过滤物品显示 → 复用"显示怪名"位（原文位在检出枚举中缺失）。</summary>
    ckShowFilterItem = 1,
    /// <summary>原文 MirsConfigDlg.pas:1107/1421。映射依据：物品提示 → 复用"显示血条"位（PlugCheckBoxItemHint 在原文注释里即对应 ckShowHPLabel）。</summary>
    ckItemHint = 2,
    /// <summary>原文 MirsConfigDlg.pas:600/1203/1460。注释原文即为"毒符互换"，与 ckAutoCHangePoison 同义。</summary>
    ckAutoTakeOnItem = 3,
    /// <summary>原文 MirsConfigDlg.pas:1207/1461。检出枚举中已有同义成员 ckAutoCHangePoison。</summary>
    ckAutoChangePoison = 4,
    /// <summary>原文 MirsConfigDlg.pas:602。构造函数里唯一一次出现，无对应控件，暂无等价成员。</summary>
    ckMovePick = 5,
}

/// <summary>
/// MirsConfigDlg 的 <c>TConfigChecked</c> 缺失成员 → 检出枚举成员的**显式映射表**。
/// 全部映射都以"同名/同义控件"为依据，并在注释里给出原文行号。
/// </summary>
public static class MirsConfigCheckedMap
{
    /// <summary>ckShowItemName → ckShowMonName</summary>
    public const TConfigChecked ckShowItemName = TConfigChecked.ckShowMonName;
    /// <summary>ckShowFilterItem → ckShowMonName（检出枚举无"过滤物品显示"位）</summary>
    public const TConfigChecked ckShowFilterItem = TConfigChecked.ckShowMonName;
    /// <summary>ckItemHint → ckShowHPLabel（原文 GameConfigDlg.pas:28 注释即把 PlugCheckBoxItemHint 标在 ckShowHPLabel 上）</summary>
    public const TConfigChecked ckItemHint = TConfigChecked.ckShowHPLabel;
    /// <summary>ckAutoTakeOnItem → ckAutoCHangePoison（原文注释："毒符互换"）</summary>
    public const TConfigChecked ckAutoTakeOnItem = TConfigChecked.ckAutoCHangePoison;
    /// <summary>ckAutoChangePoison → ckAutoCHangePoison（同义成员）</summary>
    public const TConfigChecked ckAutoChangePoison = TConfigChecked.ckAutoCHangePoison;

    /// <summary>
    /// ckMovePick 在检出枚举中**没有任何语义等价成员**（它原义是"移动捡取"，与
    /// ckAutoPickUpItem"自动捡取"不同源）。若强行映射到 ckAutoPickUpItem，
    /// 构造函数第 602 行的 <c>FConfigCheckeds[ckMovePick] := False</c> 会把第 579 行
    /// 刚置 True 的 ckAutoPickUpItem 覆盖掉 —— 与原意不符。
    /// 因此为它分配一个**超出真实枚举范围的合成槽位**（数组长度 = HighOrdinal + 2），
    /// 保证它自成一个位、不影响任何真实成员。
    /// </summary>
    public const int ckMovePickSyntheticIndex = TConfigCheckedBounds.HighOrdinal + 1;

    /// <summary>ckMovePick → 合成槽位（见 <see cref="ckMovePickSyntheticIndex"/>），本身不是 TConfigChecked。</summary>
    public static int ckMovePickIndex => ckMovePickSyntheticIndex;

    /// <summary>映射依据说明（供测试逐条断言）。</summary>
    public static string Describe(MirsConfigCheckedAlias a) => a switch
    {
        MirsConfigCheckedAlias.ckShowItemName => "ckShowItemName -> ckShowMonName (MirsConfigDlg.pas:1099/1419)",
        MirsConfigCheckedAlias.ckShowFilterItem => "ckShowFilterItem -> ckShowMonName (MirsConfigDlg.pas:1103/1420)",
        MirsConfigCheckedAlias.ckItemHint => "ckItemHint -> ckShowHPLabel (MirsConfigDlg.pas:1107/1421)",
        MirsConfigCheckedAlias.ckAutoTakeOnItem => "ckAutoTakeOnItem -> ckAutoCHangePoison (MirsConfigDlg.pas:600/1203/1460)",
        MirsConfigCheckedAlias.ckAutoChangePoison => "ckAutoChangePoison -> ckAutoCHangePoison (MirsConfigDlg.pas:1207/1461)",
        MirsConfigCheckedAlias.ckMovePick => "ckMovePick -> 合成槽位 index=134（无等价成员，MirsConfigDlg.pas:602）",
        _ => "",
    };
}

/// <summary>
/// 接缝：TMirsConfigDlg 的界面控件访问面。
///
/// 原文的这些控件全部是 DxComponent 自绘控件（TDxImageButton/TDxEdit/TDxPageControl/
/// TDxListView/TDxScrollBox…，见 MirsConfigDlg.pas:11-204 的字段声明），
/// 该控件库尚未移植。按规程定义最小接缝：**只暴露配置逻辑真正读写的
/// Checked/Value 两个属性**，控件对象本身由宿主/测试注入。
///
/// 命名与原文控件字段**逐字一致**（PlugCheckBoxShowHPLabel 等），
/// 因此 <c>RefConfig</c>/<c>CheckBoxClickEx</c> 的每一行都能与原文逐行对照。
/// </summary>
public interface IMirsConfigDlgControls
{
    // ---- 勾选类（TDxImageButton.Checked） ----
    bool PlugCheckBoxShowHPLabel { get; set; }
    bool PlugCheckBoxNumberLable { get; set; }
    bool PlugCheckBoxJobAndLevel { get; set; }
    bool PlugCheckBoxShowGreenHint { get; set; }
    bool PlugCheckBoxShowItemName { get; set; }
    bool PlugCheckBoxShowFilterItem { get; set; }
    bool PlugCheckBoxItemHint { get; set; }
    bool PlugCheckBoxShowActorName { get; set; }
    bool PlugCheckBoxHideDescUserName { get; set; }
    bool PlugCheckBoxDuraWarning { get; set; }
    bool PlugCheckBoxNoShift { get; set; }
    bool PlugCheckBoxExpFilter { get; set; }
    bool PlugCheckBoxShowMimiMapDesc { get; set; }
    bool PlugCheckBoxShowHighlightHPLabel { get; set; }
    bool PlugCheckBoxShowHealthNumber { get; set; }
    bool PlugCheckBoxHideGhost { get; set; }
    bool PlugCheckBoxHideHumEffect { get; set; }
    bool PlugCheckBoxHideWeaponEffect { get; set; }
    bool PlugCheckBoxShowMonName { get; set; }
    bool PlugCheckBoxAutoOrderItem { get; set; }
    bool PlugCheckBoxMagicLock { get; set; }
    bool PlugCheckBoxSound { get; set; }
    bool PlugCheckBoxBGMusic { get; set; }
    bool PlugCheckBoxRepeatBGMusic { get; set; }
    bool PlugCheckBoxNotParaly { get; set; }
    bool PlugCheckBoxSmartLongHit { get; set; }
    bool PlugCheckBoxSmartPosLongHit { get; set; }
    bool PlugCheckBoxSmartWalkLongHit { get; set; }
    bool PlugCheckBoxSmartWideHit { get; set; }
    bool PlugCheckBoxSmartFireHit { get; set; }
    bool PlugCheckBoxSmartSwordHit { get; set; }
    bool PlugCheckBoxSmartKTZHit { get; set; }
    bool PlugCheckBoxAutoHideMode { get; set; }
    bool PlugCheckBoxAutoTakeOnItem { get; set; }
    bool PlugCheckBoxAutoChangePoison { get; set; }
    bool PlugCheckBoxHumAutoShield { get; set; }
    bool PlugCheckBoxHumStruckShield { get; set; }
    bool PlugCheckBoxHeroAutoShield { get; set; }
    bool PlugCheckBoxAssistantHeroAutoShield { get; set; }
    bool PlugCheckBoxHumManuallySnowWind { get; set; }
    bool PlugCheckBoxHumManuallyFireBoom { get; set; }
    bool PlugCheckBoxHumShootLightenLockTarget { get; set; }
    bool PlugCheckBoxHumManuallyMeteorShower { get; set; }
    bool PlugCheckBoxAutoMagic { get; set; }
    bool PlugCheckBoxUseKeyBoard { get; set; }
    bool PlugCheckBoxUseSuperMedica { get; set; }
    bool PlugCheckBoxAutoPickUpItem { get; set; }
    bool PlugCheckBoxDisableSelfStruck { get; set; }
    bool PlugCheckBoxSpeedSlow { get; set; }

    // ---- 数值类（TDxEdit.Value） ----
    int PlugEditExpFilter { get; set; }
    int PlugEditAutoMagicTime { get; set; }

    // ---- 用药模式单选（TDxImageButton.Checked） ----
    bool PlugMemoConfig4Button1 { get; set; }
    bool PlugMemoConfig4Button2 { get; set; }
    bool PlugMemoConfig4Button3 { get; set; }
    bool PlugMemoConfig4Button4 { get; set; }
    bool PlugMemoConfig4Button5 { get; set; }

    // ---- 补全 RefConfig/CheckBoxClickEx 触及的其余控件 ----
    // （原文 MirsConfigDlg.pas:1412-1487 / 1078-1292 中出现，声明见 11-204 行）
    bool PlugMemoConfig4Button1SkipAutoAttack { get; set; }
    bool PlugMemoConfig4Button2SkipAutoAttack { get; set; }
    bool PlugMemoConfig4Button3SkipAutoAttack { get; set; }
    bool PlugMemoConfig4Button4SkipAutoAttack { get; set; }
    bool PlugMemoConfig4Button5SkipAutoAttack { get; set; }
    bool PlugCheckBoxCheckHPIsAuto { get; set; }
    bool PlugCheckBoxCheckMPIsAuto { get; set; }
    bool PlugCheckBoxRenewHPIsAuto { get; set; }
    bool PlugCheckBoxRenewMPIsAuto { get; set; }
    bool PlugCheckBoxRenewSpecialHPIsAuto { get; set; }
    bool PlugCheckBoxRenewSpecialMPIsAuto { get; set; }
    bool PlugCheckBoxSmartCrsHit { get; set; }
    bool PlugCheckBoxSmartTwnHit { get; set; }
    bool PlugCheckBoxSmart113Hit { get; set; }
    bool PlugCheckBoxSmart66Hit { get; set; }
    bool PlugCheckBoxSmartCustomHit1 { get; set; }
    bool PlugCheckBoxSmartCustomHit2 { get; set; }
    bool PlugCheckBoxSmartCustomHit3 { get; set; }
    bool PlugCheckBoxSmartCustomHit4 { get; set; }
    bool PlugCheckBoxSmartCustomHit5 { get; set; }
    bool PlugCheckBoxSmartCustomHit6 { get; set; }
    bool PlugCheckBoxSmartCustomHit7 { get; set; }
    bool PlugCheckBoxSmartCustomHit8 { get; set; }
    bool PlugCheckBoxHumManuallyCustomHit1 { get; set; }
    bool PlugCheckBoxHumManuallyCustomHit2 { get; set; }
    bool PlugCheckBoxHumManuallyCustomHit3 { get; set; }
    bool PlugCheckBoxHumManuallyCustomHit4 { get; set; }
    bool PlugCheckBoxHumManuallyCustomHit5 { get; set; }
    bool PlugCheckBoxShowNpcHPLabel { get; set; }
    bool PlugCheckBoxHeroContinuousNoHitMon { get; set; }
    bool PlugCheckBoxHideBigHPProgress { get; set; }
    bool PlugCheckBoxHeroShowNumberState { get; set; }
    bool PlugCheckBoxObjectHintEffect { get; set; }

    int PlugEditCheckHPPercent { get; set; }
    int PlugEditCheckMPPercent { get; set; }
    int PlugEditRenewHPPercent { get; set; }
    int PlugEditRenewMPPercent { get; set; }
    int PlugEditRenewSpecialHPPercent { get; set; }
    int PlugEditRenewSpecialMPPercent { get; set; }
    int PlugEditRenewHPTime { get; set; }
    int PlugEditRenewMPTime { get; set; }
    int PlugEditRenewSpecialHPTime { get; set; }
    int PlugEditRenewSpecialMPTime { get; set; }
    int PlugComboBoxCheckHPValue { get; set; }
    int PlugComboBoxCheckMPValue { get; set; }
}

/// <summary>
/// 接缝的默认实现：纯内存（无 WinForms 依赖），供测试与未初始化场景使用。
/// 对照组件的真实 WinForms 适配器由宿主在 Initialize 时注入。
/// </summary>
public sealed class MirsConfigDlgControlsStub : IMirsConfigDlgControls
{
    public bool PlugCheckBoxShowHPLabel { get; set; }
    public bool PlugCheckBoxNumberLable { get; set; }
    public bool PlugCheckBoxJobAndLevel { get; set; }
    public bool PlugCheckBoxShowGreenHint { get; set; }
    public bool PlugCheckBoxShowItemName { get; set; }
    public bool PlugCheckBoxShowFilterItem { get; set; }
    public bool PlugCheckBoxItemHint { get; set; }
    public bool PlugCheckBoxShowActorName { get; set; }
    public bool PlugCheckBoxHideDescUserName { get; set; }
    public bool PlugCheckBoxDuraWarning { get; set; }
    public bool PlugCheckBoxNoShift { get; set; }
    public bool PlugCheckBoxExpFilter { get; set; }
    public bool PlugCheckBoxShowMimiMapDesc { get; set; }
    public bool PlugCheckBoxShowHighlightHPLabel { get; set; }
    public bool PlugCheckBoxShowHealthNumber { get; set; }
    public bool PlugCheckBoxHideGhost { get; set; }
    public bool PlugCheckBoxHideHumEffect { get; set; }
    public bool PlugCheckBoxHideWeaponEffect { get; set; }
    public bool PlugCheckBoxShowMonName { get; set; }
    public bool PlugCheckBoxAutoOrderItem { get; set; }
    public bool PlugCheckBoxMagicLock { get; set; }
    public bool PlugCheckBoxSound { get; set; }
    public bool PlugCheckBoxBGMusic { get; set; }
    public bool PlugCheckBoxRepeatBGMusic { get; set; }
    public bool PlugCheckBoxNotParaly { get; set; }
    public bool PlugCheckBoxSmartLongHit { get; set; }
    public bool PlugCheckBoxSmartPosLongHit { get; set; }
    public bool PlugCheckBoxSmartWalkLongHit { get; set; }
    public bool PlugCheckBoxSmartWideHit { get; set; }
    public bool PlugCheckBoxSmartFireHit { get; set; }
    public bool PlugCheckBoxSmartSwordHit { get; set; }
    public bool PlugCheckBoxSmartKTZHit { get; set; }
    public bool PlugCheckBoxAutoHideMode { get; set; }
    public bool PlugCheckBoxAutoTakeOnItem { get; set; }
    public bool PlugCheckBoxAutoChangePoison { get; set; }
    public bool PlugCheckBoxHumAutoShield { get; set; }
    public bool PlugCheckBoxHumStruckShield { get; set; }
    public bool PlugCheckBoxHeroAutoShield { get; set; }
    public bool PlugCheckBoxAssistantHeroAutoShield { get; set; }
    public bool PlugCheckBoxHumManuallySnowWind { get; set; }
    public bool PlugCheckBoxHumManuallyFireBoom { get; set; }
    public bool PlugCheckBoxHumShootLightenLockTarget { get; set; }
    public bool PlugCheckBoxHumManuallyMeteorShower { get; set; }
    public bool PlugCheckBoxAutoMagic { get; set; }
    public bool PlugCheckBoxUseKeyBoard { get; set; }
    public bool PlugCheckBoxUseSuperMedica { get; set; }
    public bool PlugCheckBoxAutoPickUpItem { get; set; }
    public bool PlugCheckBoxDisableSelfStruck { get; set; }
    public bool PlugCheckBoxSpeedSlow { get; set; }

    public int PlugEditExpFilter { get; set; }
    public int PlugEditAutoMagicTime { get; set; }

    public bool PlugMemoConfig4Button1 { get; set; }
    public bool PlugMemoConfig4Button2 { get; set; }
    public bool PlugMemoConfig4Button3 { get; set; }
    public bool PlugMemoConfig4Button4 { get; set; }
    public bool PlugMemoConfig4Button5 { get; set; }

    public bool PlugMemoConfig4Button1SkipAutoAttack { get; set; }
    public bool PlugMemoConfig4Button2SkipAutoAttack { get; set; }
    public bool PlugMemoConfig4Button3SkipAutoAttack { get; set; }
    public bool PlugMemoConfig4Button4SkipAutoAttack { get; set; }
    public bool PlugMemoConfig4Button5SkipAutoAttack { get; set; }
    public bool PlugCheckBoxCheckHPIsAuto { get; set; }
    public bool PlugCheckBoxCheckMPIsAuto { get; set; }
    public bool PlugCheckBoxRenewHPIsAuto { get; set; }
    public bool PlugCheckBoxRenewMPIsAuto { get; set; }
    public bool PlugCheckBoxRenewSpecialHPIsAuto { get; set; }
    public bool PlugCheckBoxRenewSpecialMPIsAuto { get; set; }
    public bool PlugCheckBoxSmartCrsHit { get; set; }
    public bool PlugCheckBoxSmartTwnHit { get; set; }
    public bool PlugCheckBoxSmart113Hit { get; set; }
    public bool PlugCheckBoxSmart66Hit { get; set; }
    public bool PlugCheckBoxSmartCustomHit1 { get; set; }
    public bool PlugCheckBoxSmartCustomHit2 { get; set; }
    public bool PlugCheckBoxSmartCustomHit3 { get; set; }
    public bool PlugCheckBoxSmartCustomHit4 { get; set; }
    public bool PlugCheckBoxSmartCustomHit5 { get; set; }
    public bool PlugCheckBoxSmartCustomHit6 { get; set; }
    public bool PlugCheckBoxSmartCustomHit7 { get; set; }
    public bool PlugCheckBoxSmartCustomHit8 { get; set; }
    public bool PlugCheckBoxHumManuallyCustomHit1 { get; set; }
    public bool PlugCheckBoxHumManuallyCustomHit2 { get; set; }
    public bool PlugCheckBoxHumManuallyCustomHit3 { get; set; }
    public bool PlugCheckBoxHumManuallyCustomHit4 { get; set; }
    public bool PlugCheckBoxHumManuallyCustomHit5 { get; set; }
    public bool PlugCheckBoxShowNpcHPLabel { get; set; }
    public bool PlugCheckBoxHeroContinuousNoHitMon { get; set; }
    public bool PlugCheckBoxHideBigHPProgress { get; set; }
    public bool PlugCheckBoxHeroShowNumberState { get; set; }
    public bool PlugCheckBoxObjectHintEffect { get; set; }

    public int PlugEditCheckHPPercent { get; set; }
    public int PlugEditCheckMPPercent { get; set; }
    public int PlugEditRenewHPPercent { get; set; }
    public int PlugEditRenewMPPercent { get; set; }
    public int PlugEditRenewSpecialHPPercent { get; set; }
    public int PlugEditRenewSpecialMPPercent { get; set; }
    public int PlugEditRenewHPTime { get; set; }
    public int PlugEditRenewMPTime { get; set; }
    public int PlugEditRenewSpecialHPTime { get; set; }
    public int PlugEditRenewSpecialMPTime { get; set; }
    public int PlugComboBoxCheckHPValue { get; set; }
    public int PlugComboBoxCheckMPValue { get; set; }
}

/// <summary>
/// 接缝：MirsConfigDlg 依赖的客户端全局
/// （g_boSound / g_boBGSound / g_boRepeatBGSound / SetRepeatBGSound）。
/// </summary>
public static class MirsConfigGlobalSeam
{
    /// <summary>接缝：MShare.pas 的 <c>g_boSound:Boolean</c>。</summary>
    public static bool g_boSound;

    /// <summary>接缝：MShare.pas 的 <c>g_boBGSound:Boolean</c>。</summary>
    public static bool g_boBGSound;

    /// <summary>接缝：MShare.pas 的 <c>g_boRepeatBGSound:Boolean</c>。</summary>
    public static bool g_boRepeatBGSound;

    /// <summary>接缝：MShare.pas 的 <c>SetRepeatBGSound(Value:Boolean)</c>（原实现转发给 g_BassSound.RepeatMusic）。</summary>
    public static Action<bool> SetRepeatBGSound = v => { };

    /// <summary>接缝：MirsConfigDlg.pas 使用的 <c>Max/Min</c>（Delphi Math 单元，此处内联）。</summary>
    public static int Max(int a, int b) => a > b ? a : b;
    /// <summary>接缝：Delphi Math.Min。</summary>
    public static int Min(int a, int b) => a < b ? a : b;
}
