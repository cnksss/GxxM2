// 源单元: Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas (GBK, 5954 lines, CRLF)
// ** 本文件由脚本生成，禁止手工编辑 ** tools/mirreturn/Generate-MirReturnControls.ps1
//
// 覆盖 (原文):
//   10-328     318 control field declarations (script-extracted)
//   1986-2136  77 RefConfig assignments (script-extracted)
//   remaining members used by the other partial files (explicit list in the generator)
//
// 由手写基接口（见︱ MirReturnConfigSeams.cs): PlugScrollBoxBoss/Mons, PlugConfigDlgClose,
//   and the PlugCheckBox*Visible focus triples are declared there and deliberately absent here.
//
// Name mapping (drop the property suffix, so every line maps 1:1 to the original):
//   PlugXxx.Checked   -> PlugXxx   (bool)
//   PlugXxx.Value     -> PlugXxx   (int; string for .Text-backed edits)
//   PlugXxx.ItemIndex -> PlugXxx   (int)
//   PlugXxx.Enabled   -> PlugXxx   (bool)
//   PlugXxx.Max/Min/Position -> PlugXxxMax / PlugXxxMin / PlugXxxPosition

namespace GXX.Client.GUI.GameConfig.Seams;

public partial interface IMirReturnConfigDlgControlsExt : IMirReturnConfigDlgControls
{
    /// <summary>接缝: 原文 控件字段 <c>PlugPageControlConfig</c>.</summary>
    int PlugPageControlConfig { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig1</c>.</summary>
    int PlugTabSheetConfig1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig1</c>.</summary>
    int PlugMemoConfig1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoOrderItem</c>.</summary>
    bool PlugCheckBoxAutoOrderItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoPickUpItem</c>.</summary>
    bool PlugCheckBoxAutoPickUpItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxBGMusic</c>.</summary>
    bool PlugCheckBoxBGMusic { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxContinueButchItem</c>.</summary>
    bool PlugCheckBoxContinueButchItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxDisableSelfStruck</c>.</summary>
    bool PlugCheckBoxDisableSelfStruck { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxDuraWarning</c>.</summary>
    bool PlugCheckBoxDuraWarning { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxExpFilter</c>.</summary>
    bool PlugCheckBoxExpFilter { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHideDescUserName</c>.</summary>
    bool PlugCheckBoxHideDescUserName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHideGhost</c>.</summary>
    bool PlugCheckBoxHideGhost { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHideHumEffect</c>.</summary>
    bool PlugCheckBoxHideHumEffect { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHideTitle</c>.</summary>
    bool PlugCheckBoxHideTitle { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHideWeaponEffect</c>.</summary>
    bool PlugCheckBoxHideWeaponEffect { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxItemHint</c>.</summary>
    bool PlugCheckBoxItemHint { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxJobAndLevel</c>.</summary>
    bool PlugCheckBoxJobAndLevel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxMagicLock</c>.</summary>
    bool PlugCheckBoxMagicLock { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxMovePick</c>.</summary>
    bool PlugCheckBoxMovePick { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNoShift</c>.</summary>
    bool PlugCheckBoxNoShift { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNotParaly</c>.</summary>
    bool PlugCheckBoxNotParaly { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNumberLable</c>.</summary>
    bool PlugCheckBoxNumberLable { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRepeatBGMusic</c>.</summary>
    bool PlugCheckBoxRepeatBGMusic { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSceneShake</c>.</summary>
    bool PlugCheckBoxSceneShake { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShiftSwitch</c>.</summary>
    bool PlugCheckBoxShiftSwitch { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowActorName</c>.</summary>
    bool PlugCheckBoxShowActorName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowFilterItem</c>.</summary>
    bool PlugCheckBoxShowFilterItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowGreenHint</c>.</summary>
    bool PlugCheckBoxShowGreenHint { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowHPLabel</c>.</summary>
    bool PlugCheckBoxShowHPLabel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowHealthNumber</c>.</summary>
    bool PlugCheckBoxShowHealthNumber { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowHighlightHPLabel</c>.</summary>
    bool PlugCheckBoxShowHighlightHPLabel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowItemName</c>.</summary>
    bool PlugCheckBoxShowItemName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowMimiMapDesc</c>.</summary>
    bool PlugCheckBoxShowMimiMapDesc { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowMonName</c>.</summary>
    bool PlugCheckBoxShowMonName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowNGLabel</c>.</summary>
    bool PlugCheckBoxShowNGLabel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowNpcHPLabel</c>.</summary>
    bool PlugCheckBoxShowNpcHPLabel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxShowNpcName</c>.</summary>
    bool PlugCheckBoxShowNpcName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSpeedSlow</c>.</summary>
    bool PlugCheckBoxSpeedSlow { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxVolume</c>.</summary>
    bool PlugCheckBoxVolume { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckDisableChartMemoSize</c>.</summary>
    bool PlugCheckDisableChartMemoSize { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditExpFilter</c>.</summary>
    int PlugEditExpFilter { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>TrackBarVolume</c>.</summary>
    int TrackBarVolume { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig2</c>.</summary>
    int PlugTabSheetConfig2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnDiyAdd</c>.</summary>
    int PlugBtnDiyAdd { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnDiyDel</c>.</summary>
    int PlugBtnDiyDel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnDiyLoad</c>.</summary>
    int PlugBtnDiyLoad { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnDiySave</c>.</summary>
    int PlugBtnDiySave { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxItemCmp</c>.</summary>
    bool PlugCheckBoxItemCmp { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxPickUpAll</c>.</summary>
    bool PlugCheckBoxPickUpAll { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSpecialQuickFlashing</c>.</summary>
    bool PlugCheckBoxSpecialQuickFlashing { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxItemStdMode</c>.</summary>
    int PlugComboBoxItemStdMode { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSearchItem</c>.</summary>
    int PlugEditSearchItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSpecialColor</c>.</summary>
    int PlugEditSpecialColor { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSpecialName</c>.</summary>
    string PlugEditSpecialName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelDefaultItem</c>.</summary>
    int PlugLabelDefaultItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelSpecialColor</c>.</summary>
    int PlugLabelSpecialColor { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2</c>.</summary>
    int PlugMemoConfig2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label24</c>.</summary>
    int PlugMemoConfig2Label24 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label25</c>.</summary>
    int PlugMemoConfig2Label25 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label26</c>.</summary>
    int PlugMemoConfig2Label26 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label27</c>.</summary>
    int PlugMemoConfig2Label27 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label28</c>.</summary>
    int PlugMemoConfig2Label28 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Label29</c>.</summary>
    int PlugMemoConfig2Label29 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Line1</c>.</summary>
    int PlugMemoConfig2Line1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig2Line2</c>.</summary>
    int PlugMemoConfig2Line2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig3</c>.</summary>
    int PlugTabSheetConfig3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3</c>.</summary>
    int PlugMemoConfig3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHeroContinuousNoHitMon</c>.</summary>
    bool PlugCheckBoxHeroContinuousNoHitMon { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHeroRenewAlcoholIsAuto</c>.</summary>
    bool PlugCheckBoxHeroRenewAlcoholIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHeroRenewMedicineAlcoholIsAuto</c>.</summary>
    bool PlugCheckBoxHeroRenewMedicineAlcoholIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHeroShowNumberState</c>.</summary>
    bool PlugCheckBoxHeroShowNumberState { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewAlcoholIsAuto</c>.</summary>
    bool PlugCheckBoxRenewAlcoholIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewDeliriaIsAuto</c>.</summary>
    bool PlugCheckBoxRenewDeliriaIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewMedicineAlcoholIsAuto</c>.</summary>
    bool PlugCheckBoxRenewMedicineAlcoholIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditHeroDodgeHPPercent</c>.</summary>
    int PlugEditHeroDodgeHPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditHeroRenewAlcoholPercent</c>.</summary>
    int PlugEditHeroRenewAlcoholPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditHeroRenewMedicineAlcoholPercent</c>.</summary>
    int PlugEditHeroRenewMedicineAlcoholPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewAlcoholPercent</c>.</summary>
    int PlugEditRenewAlcoholPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewMedicineAlcoholPercent</c>.</summary>
    int PlugEditRenewMedicineAlcoholPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label1</c>.</summary>
    int PlugMemoConfig3Label1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label2</c>.</summary>
    int PlugMemoConfig3Label2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label3</c>.</summary>
    int PlugMemoConfig3Label3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label4</c>.</summary>
    int PlugMemoConfig3Label4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label5</c>.</summary>
    int PlugMemoConfig3Label5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label6</c>.</summary>
    int PlugMemoConfig3Label6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label7</c>.</summary>
    int PlugMemoConfig3Label7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig3Label8</c>.</summary>
    int PlugMemoConfig3Label8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig4</c>.</summary>
    int PlugTabSheetConfig4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4</c>.</summary>
    int PlugMemoConfig4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoPercent</c>.</summary>
    bool PlugCheckBoxAutoPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxCheckDuraIsAuto</c>.</summary>
    bool PlugCheckBoxCheckDuraIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxCheckHPIsAuto</c>.</summary>
    bool PlugCheckBoxCheckHPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxCheckMPIsAuto</c>.</summary>
    bool PlugCheckBoxCheckMPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewAutoPercent</c>.</summary>
    bool PlugCheckBoxRenewAutoPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewHPIsAuto</c>.</summary>
    bool PlugCheckBoxRenewHPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewMPIsAuto</c>.</summary>
    bool PlugCheckBoxRenewMPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewSpecialHPIsAuto</c>.</summary>
    bool PlugCheckBoxRenewSpecialHPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxRenewSpecialMPIsAuto</c>.</summary>
    bool PlugCheckBoxRenewSpecialMPIsAuto { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSuperMedicaPercent</c>.</summary>
    bool PlugCheckBoxSuperMedicaPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedica</c>.</summary>
    bool PlugCheckBoxUseSuperMedica { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName0</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName0 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName1</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName2</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName3</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName4</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName5</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName6</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName7</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseSuperMedicaItemName8</c>.</summary>
    bool PlugCheckBoxUseSuperMedicaItemName8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxCheckHPValue</c>.</summary>
    int PlugComboBoxCheckHPValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxCheckMPValue</c>.</summary>
    int PlugComboBoxCheckMPValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditCheckDura</c>.</summary>
    int PlugEditCheckDura { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditCheckDuraTime</c>.</summary>
    int PlugEditCheckDuraTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditCheckDuraValue</c>.</summary>
    string PlugEditCheckDuraValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditCheckHPPercent</c>.</summary>
    int PlugEditCheckHPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditCheckMPPercent</c>.</summary>
    int PlugEditCheckMPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewHPPercent</c>.</summary>
    int PlugEditRenewHPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewHPTime</c>.</summary>
    int PlugEditRenewHPTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewMPPercent</c>.</summary>
    int PlugEditRenewMPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewMPTime</c>.</summary>
    int PlugEditRenewMPTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewSpecialHPPercent</c>.</summary>
    int PlugEditRenewSpecialHPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewSpecialHPTime</c>.</summary>
    int PlugEditRenewSpecialHPTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewSpecialMPPercent</c>.</summary>
    int PlugEditRenewSpecialMPPercent { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditRenewSpecialMPTime</c>.</summary>
    int PlugEditRenewSpecialMPTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP0</c>.</summary>
    int PlugEditSuperMedicaHP0 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP1</c>.</summary>
    int PlugEditSuperMedicaHP1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP2</c>.</summary>
    int PlugEditSuperMedicaHP2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP3</c>.</summary>
    int PlugEditSuperMedicaHP3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP4</c>.</summary>
    int PlugEditSuperMedicaHP4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP5</c>.</summary>
    int PlugEditSuperMedicaHP5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP6</c>.</summary>
    int PlugEditSuperMedicaHP6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP7</c>.</summary>
    int PlugEditSuperMedicaHP7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHP8</c>.</summary>
    int PlugEditSuperMedicaHP8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime0</c>.</summary>
    int PlugEditSuperMedicaHPTime0 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime1</c>.</summary>
    int PlugEditSuperMedicaHPTime1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime2</c>.</summary>
    int PlugEditSuperMedicaHPTime2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime3</c>.</summary>
    int PlugEditSuperMedicaHPTime3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime4</c>.</summary>
    int PlugEditSuperMedicaHPTime4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime5</c>.</summary>
    int PlugEditSuperMedicaHPTime5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime6</c>.</summary>
    int PlugEditSuperMedicaHPTime6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime7</c>.</summary>
    int PlugEditSuperMedicaHPTime7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaHPTime8</c>.</summary>
    int PlugEditSuperMedicaHPTime8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP0</c>.</summary>
    int PlugEditSuperMedicaMP0 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP1</c>.</summary>
    int PlugEditSuperMedicaMP1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP2</c>.</summary>
    int PlugEditSuperMedicaMP2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP3</c>.</summary>
    int PlugEditSuperMedicaMP3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP4</c>.</summary>
    int PlugEditSuperMedicaMP4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP5</c>.</summary>
    int PlugEditSuperMedicaMP5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP6</c>.</summary>
    int PlugEditSuperMedicaMP6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP7</c>.</summary>
    int PlugEditSuperMedicaMP7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMP8</c>.</summary>
    int PlugEditSuperMedicaMP8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime0</c>.</summary>
    int PlugEditSuperMedicaMPTime0 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime1</c>.</summary>
    int PlugEditSuperMedicaMPTime1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime2</c>.</summary>
    int PlugEditSuperMedicaMPTime2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime3</c>.</summary>
    int PlugEditSuperMedicaMPTime3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime4</c>.</summary>
    int PlugEditSuperMedicaMPTime4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime5</c>.</summary>
    int PlugEditSuperMedicaMPTime5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime6</c>.</summary>
    int PlugEditSuperMedicaMPTime6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime7</c>.</summary>
    int PlugEditSuperMedicaMPTime7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditSuperMedicaMPTime8</c>.</summary>
    int PlugEditSuperMedicaMPTime8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label1</c>.</summary>
    int PlugMemoConfig4Label1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label2</c>.</summary>
    int PlugMemoConfig4Label2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label3</c>.</summary>
    int PlugMemoConfig4Label3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label4</c>.</summary>
    int PlugMemoConfig4Label4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label5</c>.</summary>
    int PlugMemoConfig4Label5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label6</c>.</summary>
    int PlugMemoConfig4Label6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label7</c>.</summary>
    int PlugMemoConfig4Label7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Label8</c>.</summary>
    int PlugMemoConfig4Label8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4LabelHint</c>.</summary>
    int PlugMemoConfig4LabelHint { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4LabelHint2</c>.</summary>
    int PlugMemoConfig4LabelHint2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Line2</c>.</summary>
    int PlugMemoConfig4Line2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Line3</c>.</summary>
    int PlugMemoConfig4Line3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Line4</c>.</summary>
    int PlugMemoConfig4Line4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Line5</c>.</summary>
    int PlugMemoConfig4Line5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Button1</c>.</summary>
    bool PlugMemoConfig4Button1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Button2</c>.</summary>
    bool PlugMemoConfig4Button2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Button3</c>.</summary>
    bool PlugMemoConfig4Button3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Button4</c>.</summary>
    bool PlugMemoConfig4Button4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Button5</c>.</summary>
    bool PlugMemoConfig4Button5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig4Line1</c>.</summary>
    int PlugMemoConfig4Line1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig5</c>.</summary>
    int PlugTabSheetConfig5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5</c>.</summary>
    int PlugMemoConfig5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAssistantHeroAutoShield</c>.</summary>
    bool PlugCheckBoxAssistantHeroAutoShield { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoCHangePoison</c>.</summary>
    bool PlugCheckBoxAutoCHangePoison { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoHideMode</c>.</summary>
    bool PlugCheckBoxAutoHideMode { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoMagic</c>.</summary>
    bool PlugCheckBoxAutoMagic { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoTakeOnItem</c>.</summary>
    bool PlugCheckBoxAutoTakeOnItem { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHeroAutoShield</c>.</summary>
    bool PlugCheckBoxHeroAutoShield { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumAutoShield</c>.</summary>
    bool PlugCheckBoxHumAutoShield { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumManuallyFireBoom</c>.</summary>
    bool PlugCheckBoxHumManuallyFireBoom { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumManuallyMeteorShower</c>.</summary>
    bool PlugCheckBoxHumManuallyMeteorShower { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumManuallySnowWind</c>.</summary>
    bool PlugCheckBoxHumManuallySnowWind { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumShootLightenLockTarget</c>.</summary>
    bool PlugCheckBoxHumShootLightenLockTarget { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxHumStruckShield</c>.</summary>
    bool PlugCheckBoxHumStruckShield { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartCRSHit</c>.</summary>
    bool PlugCheckBoxSmartCRSHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartFireHit</c>.</summary>
    bool PlugCheckBoxSmartFireHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartKTZHit</c>.</summary>
    bool PlugCheckBoxSmartKTZHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartLongHit</c>.</summary>
    bool PlugCheckBoxSmartLongHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartPosLongHit</c>.</summary>
    bool PlugCheckBoxSmartPosLongHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartSwordHit</c>.</summary>
    bool PlugCheckBoxSmartSwordHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartTWNHit</c>.</summary>
    bool PlugCheckBoxSmartTWNHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartWalkLongHit</c>.</summary>
    bool PlugCheckBoxSmartWalkLongHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxSmartWideHit</c>.</summary>
    bool PlugCheckBoxSmartWideHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxAutoMagic</c>.</summary>
    int PlugComboBoxAutoMagic { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditAutoMagicTime</c>.</summary>
    int PlugEditAutoMagicTime { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5Label19</c>.</summary>
    int PlugMemoConfig5Label19 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5Label20</c>.</summary>
    int PlugMemoConfig5Label20 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5Label21</c>.</summary>
    int PlugMemoConfig5Label21 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5Label22</c>.</summary>
    int PlugMemoConfig5Label22 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig5Label23</c>.</summary>
    int PlugMemoConfig5Label23 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig6</c>.</summary>
    int PlugTabSheetConfig6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6</c>.</summary>
    int PlugMemoConfig6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxUseKeyBoard</c>.</summary>
    bool PlugCheckBoxUseKeyBoard { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6Label1</c>.</summary>
    int PlugMemoConfig6Label1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6Label2</c>.</summary>
    int PlugMemoConfig6Label2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6Label3</c>.</summary>
    int PlugMemoConfig6Label3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard1</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard10</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard10 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard11</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard11 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard12</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard12 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard2</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard3</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard4</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard5</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard6</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard7</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard8</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard9</c>.</summary>
    int PlugMemoConfig6LabelKeyBoard9 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc1</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc10</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc10 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc11</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc11 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc12</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc12 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc2</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc3</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc4</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc5</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc6</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc7</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc8</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardDesc9</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardDesc9 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal1</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal10</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal10 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal11</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal11 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal12</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal12 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal2</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal3</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal4</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal4 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal5</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal5 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal6</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal6 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal7</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal8</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoardNormal9</c>.</summary>
    int PlugMemoConfig6LabelKeyBoardNormal9 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6Line1</c>.</summary>
    int PlugMemoConfig6Line1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig7</c>.</summary>
    int PlugTabSheetConfig7 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfigBoss</c>.</summary>
    int PlugMemoConfigBoss { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnBossAdd</c>.</summary>
    int PlugBtnBossAdd { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnBossDel</c>.</summary>
    bool PlugBtnBossDel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugBtnBossModify</c>.</summary>
    bool PlugBtnBossModify { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoDownHorse</c>.</summary>
    bool PlugCheckBoxAutoDownHorse { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoLock</c>.</summary>
    bool PlugCheckBoxAutoLock { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxBlacklistHit</c>.</summary>
    bool PlugCheckBoxBlacklistHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxColorShow</c>.</summary>
    bool PlugCheckBoxColorShow { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxFriendHit</c>.</summary>
    bool PlugCheckBoxFriendHit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNearHint</c>.</summary>
    bool PlugCheckBoxNearHint { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxColorShow</c>.</summary>
    int PlugComboBoxColorShow { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditBoss</c>.</summary>
    string PlugEditBoss { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLblBoss</c>.</summary>
    int PlugLblBoss { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig8</c>.</summary>
    int PlugTabSheetConfig8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8</c>.</summary>
    int PlugMemoConfig8 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugButtonGJRun</c>.</summary>
    int PlugButtonGJRun { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxAutoPickup</c>.</summary>
    bool PlugCheckBoxAutoPickup { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxBagFull</c>.</summary>
    bool PlugCheckBoxBagFull { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxDFAvoid</c>.</summary>
    bool PlugCheckBoxDFAvoid { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxLimitScreen</c>.</summary>
    bool PlugCheckBoxLimitScreen { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNoBluePoison</c>.</summary>
    bool PlugCheckBoxNoBluePoison { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNoDuFu</c>.</summary>
    bool PlugCheckBoxNoDuFu { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNoRedPoison</c>.</summary>
    bool PlugCheckBoxNoRedPoison { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxNotRushMon</c>.</summary>
    bool PlugCheckBoxNotRushMon { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxPlayAttack</c>.</summary>
    bool PlugCheckBoxPlayAttack { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxBagFullValue</c>.</summary>
    int PlugComboBoxBagFullValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxNoBluePoisonValue</c>.</summary>
    int PlugComboBoxNoBluePoisonValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxNoDuFuValue</c>.</summary>
    int PlugComboBoxNoDuFuValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxNoRedPoisonValue</c>.</summary>
    int PlugComboBoxNoRedPoisonValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugComboBoxPlayAttackValue</c>.</summary>
    int PlugComboBoxPlayAttackValue { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditNotRushMonRange</c>.</summary>
    int PlugEditNotRushMonRange { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelNotRushMon</c>.</summary>
    int PlugLabelNotRushMon { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Button1</c>.</summary>
    int PlugMemoConfig8Button1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Button2</c>.</summary>
    int PlugMemoConfig8Button2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Button3</c>.</summary>
    int PlugMemoConfig8Button3 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Line1</c>.</summary>
    int PlugMemoConfig8Line1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Line2</c>.</summary>
    int PlugMemoConfig8Line2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig8Page</c>.</summary>
    int PlugMemoConfig8Page { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig81</c>.</summary>
    int PlugTabSheetConfig81 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugButtonMonNameAdd</c>.</summary>
    int PlugButtonMonNameAdd { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugButtonMonNameDel</c>.</summary>
    bool PlugButtonMonNameDel { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugButtonMonNameEdit</c>.</summary>
    bool PlugButtonMonNameEdit { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditMonName</c>.</summary>
    string PlugEditMonName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelMonName</c>.</summary>
    int PlugLabelMonName { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelShortKey</c>.</summary>
    int PlugLabelShortKey { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig82</c>.</summary>
    int PlugTabSheetConfig82 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig82</c>.</summary>
    int PlugMemoConfig82 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelConfig82C1</c>.</summary>
    int PlugLabelConfig82C1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelConfig82C2</c>.</summary>
    int PlugLabelConfig82C2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLineConfig82C</c>.</summary>
    int PlugLineConfig82C { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig83</c>.</summary>
    int PlugTabSheetConfig83 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugCheckBoxGroupAttack</c>.</summary>
    bool PlugCheckBoxGroupAttack { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugEditNotGroupAttackCount</c>.</summary>
    int PlugEditNotGroupAttackCount { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelGroupAttack</c>.</summary>
    int PlugLabelGroupAttack { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig83</c>.</summary>
    int PlugMemoConfig83 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelConfig83C1</c>.</summary>
    int PlugLabelConfig83C1 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLabelConfig83C2</c>.</summary>
    int PlugLabelConfig83C2 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugLineConfig83C</c>.</summary>
    int PlugLineConfig83C { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugTabSheetConfig9</c>.</summary>
    int PlugTabSheetConfig9 { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfigHelp</c>.</summary>
    object PlugMemoConfigHelp { get; set; }
    /// <summary>接缝: 原文 控件字段 <c>PlugMemoConfig6LabelKeyBoard</c>.</summary>
    string PlugMemoConfig6LabelKeyBoard { get; set; }
    /// <summary>接缝: 原文 2037-2039 TrackBarVolume.Max.</summary>
    int TrackBarVolumeMax { get; set; }
    /// <summary>接缝: 原文 2037-2039 TrackBarVolume.Min.</summary>
    int TrackBarVolumeMin { get; set; }
    /// <summary>接缝: 原文 2037-2039 TrackBarVolume.Position.</summary>
    int TrackBarVolumePosition { get; set; }
}
