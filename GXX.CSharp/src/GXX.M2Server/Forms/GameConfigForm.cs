using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// GameConfig.pas TfrmGameConfig 巨片拆分（批次J22 第一片，5277 行）：
/// 游戏速度页（GameSpeed）全部处理器 1:1 —— RefGameSpeedConf/默认/保存、六间隔与六消息数、
/// 超速踢下线、弯腰控制、检测动作次数组、ModValue/uModValue（16 保存按钮）与页切换确认。
/// 其余页（General/Castle/Option/Msg/Time/Price/MsgColor/HumanDie/CharStatus/DieDrop）随后续巨片接入。
/// </summary>
public sealed class GameConfigForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;
    private bool boSendServerConfig;

    // ---- GameSpeed 页控件 ----
    public System.Windows.Forms.CheckBox chkSpeedControl = null!;
    public System.Windows.Forms.NumericUpDown EditHitIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditMagicHitIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditRunIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditWalkIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditTurnIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditDigUpIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxHitMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxSpellMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxRunMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxWalkMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxTurnMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxDigUpMsgCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxboKickOverSpeed = null!;
    public System.Windows.Forms.NumericUpDown EditOverSpeedKickCount = null!;
    public System.Windows.Forms.NumericUpDown EditDropOverSpeed = null!;
    public System.Windows.Forms.CheckBox CheckBoxSpellSendUpdateMsg = null!;
    public System.Windows.Forms.CheckBox CheckBoxActionSendActionMsg = null!;
    public System.Windows.Forms.RadioButton RadioButtonDelyMode = null!;
    public System.Windows.Forms.RadioButton RadioButtonFilterMode = null!;
    public System.Windows.Forms.CheckBox CheckBoxDisableStruck = null!;
    public System.Windows.Forms.CheckBox CheckBoxDisableSelfStruck = null!;
    public System.Windows.Forms.CheckBox chkMagicshieldStruck = null!;
    public System.Windows.Forms.NumericUpDown EditStruckTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxCheckActionCount = null!;
    public System.Windows.Forms.NumericUpDown EditHitCountIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditMagicHitCountIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditMoveCountIntervalTime = null!;
    public System.Windows.Forms.NumericUpDown EditCanHitCount = null!;
    public System.Windows.Forms.NumericUpDown EditCanMagicHitCount = null!;
    public System.Windows.Forms.NumericUpDown EditCanMoveCount = null!;
    public System.Windows.Forms.NumericUpDown EditCheckHitCount = null!;
    public System.Windows.Forms.NumericUpDown EditCheckMagicHitCount = null!;
    public System.Windows.Forms.NumericUpDown EditCheckMoveCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxSendUpdateMsg = null!;
    public System.Windows.Forms.NumericUpDown EditMaxHitDeliveryTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxMagicHitDeliveryTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxRunDeliveryTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxWalkDeliveryTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxTurnDeliveryTime = null!;
    public System.Windows.Forms.NumericUpDown EditMaxDigUpDeliveryTime = null!;
    public System.Windows.Forms.CheckBox chkHorseRun3Grid = null!;
    public System.Windows.Forms.Button ButtonActionSpeedConfig = null!;
    public System.Windows.Forms.Button ButtonGameSpeedDefault = null!;
    public System.Windows.Forms.Button ButtonGameSpeedSave = null!;

    // ---- 城堡页控件（批次J23） ----
    public System.Windows.Forms.NumericUpDown EditRepairDoorPrice = null!;
    public System.Windows.Forms.NumericUpDown EditRepairWallPrice = null!;
    public System.Windows.Forms.NumericUpDown EditHireArcherPrice = null!;
    public System.Windows.Forms.NumericUpDown EditHireGuardPrice = null!;
    public System.Windows.Forms.NumericUpDown EditCastleGoldMax = null!;
    public System.Windows.Forms.NumericUpDown EditCastleOneDayGold = null!;
    public System.Windows.Forms.TextBox EditCastleHomeMap = null!;
    public System.Windows.Forms.NumericUpDown EditCastleHomeX = null!;
    public System.Windows.Forms.NumericUpDown EditCastleHomeY = null!;
    public System.Windows.Forms.TextBox EditCastleName = null!;
    public System.Windows.Forms.NumericUpDown EditWarRangeX = null!;
    public System.Windows.Forms.NumericUpDown EditWarRangeY = null!;
    public System.Windows.Forms.CheckBox CheckBoxGetAllNpcTax = null!;
    public System.Windows.Forms.NumericUpDown EditTaxRate = null!;
    public System.Windows.Forms.NumericUpDown EditCastleMemberPriceRate = null!;
    public System.Windows.Forms.Button ButtonCastleSave = null!;

    // ---- Option 页控件（跑动/交易/掉落/安全区/红名回城/国家/挑战，批次J23） ----
    public System.Windows.Forms.CheckBox chkDisHumRun = null!;
    public System.Windows.Forms.CheckBox chkRunHum = null!;
    public System.Windows.Forms.CheckBox chkRunMon = null!;
    public System.Windows.Forms.CheckBox chkRunNpc = null!;
    public System.Windows.Forms.CheckBox chkRunGuard = null!;
    public System.Windows.Forms.CheckBox chkWarDisHumRun = null!;
    public System.Windows.Forms.CheckBox chkWarHreoRun = null!;
    public System.Windows.Forms.CheckBox chkGMRunAll = null!;
    public System.Windows.Forms.CheckBox chkSafeArea = null!;
    public System.Windows.Forms.CheckBox chkSafeAreaDisNpcRun = null!;
    public System.Windows.Forms.CheckBox chkSafeAreaDisShopStallHumRun = null!;
    public System.Windows.Forms.CheckBox chkSafeAreaDisOffLineHumRun = null!;
    public System.Windows.Forms.CheckBox chkWarDisTeleport = null!;
    public System.Windows.Forms.NumericUpDown seTryDealTime = null!;
    public System.Windows.Forms.NumericUpDown seDealOKTime = null!;
    public System.Windows.Forms.CheckBox chkCanNotGetBackDeal = null!;
    public System.Windows.Forms.CheckBox chkDisableDeal = null!;
    public System.Windows.Forms.CheckBox chkControlDropItem = null!;
    public System.Windows.Forms.CheckBox chkIsSafeDisableDrop = null!;
    public System.Windows.Forms.NumericUpDown seCanDropPrice = null!;
    public System.Windows.Forms.NumericUpDown seCanDropGold = null!;
    public System.Windows.Forms.NumericUpDown EditSafeZoneSize = null!;
    public System.Windows.Forms.CheckBox chkHintSafeZone = null!;
    public System.Windows.Forms.NumericUpDown seHintSafeZoneY = null!;
    public System.Windows.Forms.NumericUpDown seHintSafeZoneFColor = null!;
    public System.Windows.Forms.NumericUpDown seHintSafeZoneBColor = null!;
    public System.Windows.Forms.NumericUpDown seHintSafeZoneFSize = null!;
    public System.Windows.Forms.NumericUpDown EditStartPointSize = null!;
    public System.Windows.Forms.NumericUpDown seGroupMembersMax = null!;
    public System.Windows.Forms.TextBox EditRedHomeMap = null!;
    public System.Windows.Forms.NumericUpDown EditRedHomeX = null!;
    public System.Windows.Forms.NumericUpDown EditRedHomeY = null!;
    public System.Windows.Forms.TextBox EditRedDieHomeMap = null!;
    public System.Windows.Forms.NumericUpDown EditRedDieHomeX = null!;
    public System.Windows.Forms.NumericUpDown EditRedDieHomeY = null!;
    public System.Windows.Forms.TextBox EditHomeMap = null!;
    public System.Windows.Forms.NumericUpDown EditHomeX = null!;
    public System.Windows.Forms.NumericUpDown EditHomeY = null!;
    public System.Windows.Forms.CheckBox chkNationGroupCheck = null!;
    public System.Windows.Forms.CheckBox chkNationGuildCheck = null!;
    public System.Windows.Forms.NumericUpDown seNationSayLevel = null!;
    public System.Windows.Forms.NumericUpDown seTryChallengeTime = null!;
    public System.Windows.Forms.NumericUpDown seChallengeOKTime = null!;
    public System.Windows.Forms.CheckBox chkCanNotGetBackChallenge = null!;
    public System.Windows.Forms.CheckBox chkDisableChallenge = null!;
    public System.Windows.Forms.NumericUpDown seChallengeTime = null!;
    public System.Windows.Forms.NumericUpDown rgChallengeGold = null!;
    public System.Windows.Forms.Button ButtonOptionSave = null!;
    public System.Windows.Forms.Button ButtonOptionSave3 = null!;

    // ---- PK 页控件（批次J23） ----
    public System.Windows.Forms.NumericUpDown EditDecPkPointTime = null!;
    public System.Windows.Forms.NumericUpDown EditDecPkPointCount = null!;
    public System.Windows.Forms.NumericUpDown EditPKFlagTime = null!;
    public System.Windows.Forms.NumericUpDown seHumanAddPKPoint = null!;
    public System.Windows.Forms.NumericUpDown seKillHumanWeaponUnlockRate = null!;
    public System.Windows.Forms.CheckBox chkHeroKillHumanNotWeaponUnlock = null!;
    public System.Windows.Forms.NumericUpDown seDummyAddPKPoint = null!;
    public System.Windows.Forms.NumericUpDown seKillHeroAddPKPoint = null!;
    public System.Windows.Forms.CheckBox CheckBoxKillHumanWinLevel = null!;
    public System.Windows.Forms.CheckBox CheckBoxKilledLostLevel = null!;
    public System.Windows.Forms.CheckBox CheckBoxKillHumanWinExp = null!;
    public System.Windows.Forms.CheckBox CheckBoxKilledLostExp = null!;
    public System.Windows.Forms.NumericUpDown EditKillHumanWinLevel = null!;
    public System.Windows.Forms.NumericUpDown EditKilledLostLevel = null!;
    public System.Windows.Forms.NumericUpDown EditKillHumanWinExp = null!;
    public System.Windows.Forms.NumericUpDown EditKillHumanLostExp = null!;
    public System.Windows.Forms.NumericUpDown EditHumanLevelDiffer = null!;
    public System.Windows.Forms.CheckBox CheckBoxPKLevelProtect = null!;
    public System.Windows.Forms.NumericUpDown EditPKProtectLevel = null!;
    public System.Windows.Forms.NumericUpDown EditRedPKProtectLevel = null!;
    public System.Windows.Forms.Button ButtonOptionSave2 = null!;

    // ---- 测试服/试玩页控件（批次J23） ----
    public System.Windows.Forms.CheckBox CheckBoxTestServer = null!;
    public System.Windows.Forms.CheckBox CheckBoxServiceMode = null!;
    public System.Windows.Forms.CheckBox CheckBoxVentureMode = null!;
    public System.Windows.Forms.CheckBox CheckBoxNonPKMode = null!;
    public System.Windows.Forms.NumericUpDown seStartPermission = null!;
    public System.Windows.Forms.NumericUpDown seTestLevel = null!;
    public System.Windows.Forms.NumericUpDown seTestGold = null!;
    public System.Windows.Forms.NumericUpDown seTestUserLimit = null!;
    public System.Windows.Forms.NumericUpDown seUserFull = null!;
    public System.Windows.Forms.NumericUpDown seHumanMaxGold = null!;
    public System.Windows.Forms.NumericUpDown seHumanTryModeMaxGold = null!;
    public System.Windows.Forms.NumericUpDown seTryModeLevel = null!;
    public System.Windows.Forms.CheckBox CheckBoxTryModeUseStorage = null!;
    public System.Windows.Forms.NumericUpDown seGuildMemberMaxLimit = null!;
    public System.Windows.Forms.NumericUpDown seGuildNameLen = null!;
    public System.Windows.Forms.NumericUpDown seGuildRankNameLen = null!;
    public System.Windows.Forms.NumericUpDown seHumChgMapOrLoginProtectTime = null!;
    public System.Windows.Forms.CheckBox chkOffLineShop = null!;
    public System.Windows.Forms.CheckBox chkOffLineHero = null!;
    public System.Windows.Forms.CheckBox chkOffLineSlave = null!;
    public System.Windows.Forms.CheckBox chkSellItemToNpcShopNoCalcAddProperty = null!;
    public System.Windows.Forms.CheckBox chkShowNewValueFromBuyNpcItem = null!;
    public System.Windows.Forms.Button ButtonOptionSave0 = null!;

    /// <summary>seTestLevelChange 的 Tag 状态（Delphi Tag 1:1：空文本分支置 1 后下次变更 div 10）。</summary>
    private int _seTestLevelTag;

    // ---- General/经验页控件（批次J24） ----
    public System.Windows.Forms.TextBox EditSoftVersionDate = null!;
    public System.Windows.Forms.NumericUpDown EditConsoleShowUserCountTime = null!;
    public System.Windows.Forms.NumericUpDown EditShowLineNoticeTime = null!;
    public System.Windows.Forms.ComboBox ComboBoxLineNoticeColor = null!;
    public System.Windows.Forms.TextBox EditLineNoticePreFix = null!;
    public System.Windows.Forms.CheckBox CheckBoxShowMakeItemMsg = null!;
    public System.Windows.Forms.CheckBox CbViewHack = null!;
    public System.Windows.Forms.CheckBox CkViewAdmfail = null!;
    public System.Windows.Forms.CheckBox CheckBoxShowExceptionMsg = null!;
    public System.Windows.Forms.CheckBox CheckBoxCanOldClientLogon = null!;
    public System.Windows.Forms.CheckBox chkOldClient = null!;
    public System.Windows.Forms.CheckBox CheckBoxSendOnlineCount = null!;
    public System.Windows.Forms.NumericUpDown EditSendOnlineCountRate = null!;
    public System.Windows.Forms.NumericUpDown EditSendOnlineTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterPowerRate = null!;
    public System.Windows.Forms.NumericUpDown EditEditItemsPowerRate = null!;
    public System.Windows.Forms.NumericUpDown EditItemsACPowerRate = null!;
    public System.Windows.Forms.CheckBox chkRecordPublicMsg = null!;
    public System.Windows.Forms.CheckBox chkRecordPrivateMsg = null!;
    public System.Windows.Forms.CheckBox chkRecordGuildMsg = null!;
    public System.Windows.Forms.CheckBox chkRecordCryCryMsg = null!;
    public System.Windows.Forms.CheckBox chkRecordGroupMsg = null!;
    public System.Windows.Forms.CheckBox chkRecordNationMsg = null!;
    public System.Windows.Forms.CheckBox chkPermissionChangeLog = null!;
    public System.Windows.Forms.NumericUpDown EditKillMonExpMultiple = null!;
    public System.Windows.Forms.CheckBox CheckBoxHighLevelKillMonFixExp = null!;
    public System.Windows.Forms.CheckBox CheckBoxHighLevelGroupFixExp = null!;
    public System.Windows.Forms.NumericUpDown EditMaxUpLevelCount = null!;
    public System.Windows.Forms.DataGridView GridLevelExp = null!;
    public System.Windows.Forms.DataGridView GridLevelExpRate = null!;
    public System.Windows.Forms.ComboBox ComboBoxLevelExp = null!;
    public System.Windows.Forms.CheckBox CheckBoxFixExp = null!;
    public System.Windows.Forms.NumericUpDown SpinEditBaseExp = null!;
    public System.Windows.Forms.NumericUpDown SpinEditAddExp = null!;
    public System.Windows.Forms.NumericUpDown EditHighLevel = null!;
    public System.Windows.Forms.NumericUpDown EditHighLevelGetExp = null!;
    public System.Windows.Forms.CheckBox CheckBoxLimitChangeExp = null!;
    public System.Windows.Forms.NumericUpDown RadioGroupMaxLevel = null!;
    public System.Windows.Forms.NumericUpDown rgMaxAC = null!;
    public System.Windows.Forms.NumericUpDown rgMaxHitPoint = null!;
    public System.Windows.Forms.Button ButtonGeneralSave = null!;
    public System.Windows.Forms.Button ButtonExpSave = null!;

    // ---- Msg 页控件（批次J24） ----
    public System.Windows.Forms.NumericUpDown EditSayMsgMaxLen = null!;
    public System.Windows.Forms.NumericUpDown EditSayRedMsgMaxLen = null!;
    public System.Windows.Forms.NumericUpDown EditCanShoutMsgLevel = null!;
    public System.Windows.Forms.NumericUpDown EditSayMsgTime = null!;
    public System.Windows.Forms.NumericUpDown EditSayMsgCount = null!;
    public System.Windows.Forms.NumericUpDown EditDisableSayMsgTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxShutRedMsgShowGMName = null!;
    public System.Windows.Forms.CheckBox CheckBoxShowPreFixMsg = null!;
    public System.Windows.Forms.TextBox EditGMRedMsgCmd = null!;
    public System.Windows.Forms.CheckBox CheckBoxShowWhisperLevelMsg = null!;
    public System.Windows.Forms.TextBox EditShowWhisperLevelMsg = null!;
    public System.Windows.Forms.NumericUpDown EditUserItemSayMsgTime = null!;
    public System.Windows.Forms.NumericUpDown seMaxInputStringLen = null!;
    public System.Windows.Forms.Button ButtonMsgSave = null!;

    // ---- Time 页控件（批次J24） ----
    public System.Windows.Forms.NumericUpDown EditStartCastleWarDays = null!;
    public System.Windows.Forms.NumericUpDown EditStartCastlewarTime = null!;
    public System.Windows.Forms.NumericUpDown EditShowCastleWarEndMsgTime = null!;
    public System.Windows.Forms.NumericUpDown EditCastleWarTime = null!;
    public System.Windows.Forms.NumericUpDown EditGetCastleTime = null!;
    public System.Windows.Forms.NumericUpDown EditGuildWarTime = null!;
    public System.Windows.Forms.NumericUpDown seMakeGhostTime = null!;
    public System.Windows.Forms.NumericUpDown seMakeMonGhostTime = null!;
    public System.Windows.Forms.NumericUpDown seMakeDummyGhostTime = null!;
    public System.Windows.Forms.NumericUpDown seClearDropOnFloorItemTime = null!;
    public System.Windows.Forms.NumericUpDown seSaveHumanRcdTime = null!;
    public System.Windows.Forms.NumericUpDown seHumanFreeDelayTime = null!;
    public System.Windows.Forms.NumericUpDown seGetDBSockMsgTime = null!;
    public System.Windows.Forms.NumericUpDown seFloorItemCanPickUpTime = null!;
    public System.Windows.Forms.NumericUpDown seHorseTakeTime = null!;
    public System.Windows.Forms.NumericUpDown seTakeOnHorseUseTime = null!;
    public System.Windows.Forms.CheckBox chkReadyOnHorseDisableAction = null!;
    public System.Windows.Forms.NumericUpDown seNpcButtonClickTime = null!;
    public System.Windows.Forms.NumericUpDown seNpcActorClickTime = null!;
    public System.Windows.Forms.NumericUpDown sePlayerVarJClearTime = null!;
    public System.Windows.Forms.NumericUpDown seDearRecallTime = null!;
    public System.Windows.Forms.NumericUpDown seMasterRecallTime = null!;
    public System.Windows.Forms.NumericUpDown seGroupRecallTime = null!;
    public System.Windows.Forms.Button ButtonTimeSave = null!;

    // ---- Price 页控件（批次J24） ----
    public System.Windows.Forms.NumericUpDown EditBuildGuildPrice = null!;
    public System.Windows.Forms.NumericUpDown EditGuildWarPrice = null!;
    public System.Windows.Forms.NumericUpDown EditMakeDurgPrice = null!;
    public System.Windows.Forms.NumericUpDown EditSuperRepairPriceRate = null!;
    public System.Windows.Forms.NumericUpDown EditRepairItemDecDura = null!;
    public System.Windows.Forms.Button ButtonPriceSave = null!;

    // ---- 其余页保存按钮（ModValue/uModValue 联动；处理器随巨片接入） ----
    // （ButtonCastleSave/ButtonOptionSave0/2/3 已随批次J23、ButtonGeneralSave/ExpSave/TimeSave/
    //   PriceSave/MsgSave 已随批次J24 接入真实控件）
    public System.Windows.Forms.Button ButtonMsgColorSave = null!;
    public System.Windows.Forms.Button ButtonHumanDieSave = null!;
    public System.Windows.Forms.Button ButtonCharStatusSave = null!;
    public System.Windows.Forms.Button ButtonCheckActionSave = null!;
    public System.Windows.Forms.Button ButtonDieDropUseItemSave = null!;

    // ---- MsgColor 页控件（批次J25） ----
    public System.Windows.Forms.NumericUpDown EditHearMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EdittHearMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditWhisperMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditWhisperMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditGMWhisperMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditGMWhisperMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown seSendWhisperMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown seSendWhisperMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown seRefreshGameGoldFColor = null!;
    public System.Windows.Forms.NumericUpDown seRefreshGameGoldBColor = null!;
    public System.Windows.Forms.NumericUpDown seShowWhisperFColor = null!;
    public System.Windows.Forms.NumericUpDown seShowWhisperBColor = null!;
    public System.Windows.Forms.NumericUpDown seCloseWhisperFColor = null!;
    public System.Windows.Forms.NumericUpDown seCloseWhisperBColor = null!;
    public System.Windows.Forms.NumericUpDown EditRedMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditRedMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditGreenMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditGreenMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditBlueMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditBlueMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditCryMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditCryMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditGuildMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditGuildMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditGroupMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditGroupMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditCustMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditCustMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditNationMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditNationMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditUserSayMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditUserSayMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditTopUserSayMsgFColor = null!;
    public System.Windows.Forms.NumericUpDown EditTopUserSayMsgBColor = null!;
    public System.Windows.Forms.NumericUpDown EditDropItemFColor = null!;
    public System.Windows.Forms.NumericUpDown EditDropItemBColor = null!;
    public System.Windows.Forms.NumericUpDown seNPCLabelNormalColor = null!;
    public System.Windows.Forms.CheckBox chkNPCLabelFontStroke = null!;
    public System.Windows.Forms.NumericUpDown seNPCLabelMouseMoveColor = null!;
    public System.Windows.Forms.NumericUpDown seNPCLabelMouseDownColor = null!;

    // ---- HumanDie/DieDrop 页控件（批次J25） ----
    public System.Windows.Forms.NumericUpDown ScrollBarDieDropUseItemRate = null!;
    public System.Windows.Forms.TextBox EditDieDropUseItemRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarDieRedDropUseItemRate = null!;
    public System.Windows.Forms.TextBox EditDieRedDropUseItemRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarDieScatterBagRate = null!;
    public System.Windows.Forms.TextBox EditDieScatterBagRate = null!;
    public System.Windows.Forms.CheckBox CheckBoxKillByMonstDropUseItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxKillByHumanDropUseItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxDieScatterBag = null!;
    public System.Windows.Forms.CheckBox CheckBoxDieDropGold = null!;
    public System.Windows.Forms.CheckBox CheckBoxDieRedScatterBagAll = null!;
    public System.Windows.Forms.NumericUpDown scrlbrJewelryBoxItem = null!;
    public System.Windows.Forms.TextBox edtJewelryBoxItem = null!;
    public System.Windows.Forms.NumericUpDown scrlbrGodBlessItem = null!;
    public System.Windows.Forms.TextBox edtGodBlessItem = null!;
    public System.Windows.Forms.CheckBox chkKillByMonstDropJewelryBoxItem = null!;
    public System.Windows.Forms.CheckBox chkKillByHumanDropJewelryBoxItem = null!;
    public System.Windows.Forms.CheckBox chkKillByMonstDropGodBlessItem = null!;
    public System.Windows.Forms.CheckBox chkKillByHumanDropGodBlessItem = null!;
    public System.Windows.Forms.NumericUpDown EditScatterBagItemsMinLevel = null!;
    public System.Windows.Forms.NumericUpDown EditDropUseItemsMaxCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxDropUseItem = null!;
    public System.Windows.Forms.NumericUpDown EditDieRedDropUseItemOneRate = null!;
    /// <summary>死亡按槽掉装备几率 19 组滚动条（Delphi Tag 0..18）。</summary>
    public System.Windows.Forms.NumericUpDown[] DieDropUseItemRateScrolls = null!;
    public System.Windows.Forms.TextBox[] edtDieDropUseItemRateCells = null!;

    // ---- CharStatus 页控件（批次J25） ----
    public System.Windows.Forms.CheckBox CheckBoxParalyCanRun = null!;
    public System.Windows.Forms.CheckBox CheckBoxParalyCanWalk = null!;
    public System.Windows.Forms.CheckBox CheckBoxParalyCanHit = null!;
    public System.Windows.Forms.CheckBox CheckBoxParalyCanSpell = null!;
    /// <summary>攻击模式开关组（Delphi TCheckGroupAttatckMode 8 项，HAM_ALL..HAM_NATION）。</summary>
    public System.Windows.Forms.CheckBox[] CheckGroupAttatckModeItems = null!;
    public System.Windows.Forms.GroupBox GroupBoxParaly = null!;

    /// <summary>ButtonActionSpeedConfigClick 接缝（Delphi: TfrmActionSpeed.Create/Open/Free）。</summary>
    public Action? ActionSpeedConfigHandler;

    public GameConfigForm()
    {
        InitializeComponent();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, int left, System.Windows.Forms.Control parent)
    {
        return MakeSpin(caption, top, left, parent, 2000000000);
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, int left, System.Windows.Forms.Control parent, int max)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = caption, Left = left, Top = top + 4, AutoSize = true });
        var edit = new System.Windows.Forms.NumericUpDown { Left = left + 130, Top = top, Width = 90, Maximum = max };
        parent.Controls.Add(edit);
        return edit;
    }

    private System.Windows.Forms.CheckBox MkChk(string caption, int left, int top, System.Windows.Forms.Control parent, Action<object?> handler)
    {
        var chk = new System.Windows.Forms.CheckBox { Text = caption, Left = left, Top = top, AutoSize = true };
        chk.Click += (s, e) => handler(s);
        parent.Controls.Add(chk);
        return chk;
    }

    private System.Windows.Forms.NumericUpDown BindColor(System.Windows.Forms.Control parent, string caption, int top,
        Func<System.Windows.Forms.NumericUpDown> getter, Action<System.Windows.Forms.NumericUpDown> setter, Action<object?> handler)
        => BindColor(parent, caption, top, 10, getter, setter, handler);

    private System.Windows.Forms.NumericUpDown BindColor(System.Windows.Forms.Control parent, string caption, int top, int left,
        Func<System.Windows.Forms.NumericUpDown> getter, Action<System.Windows.Forms.NumericUpDown> setter, Action<object?> handler)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = caption, Left = left, Top = top + 4, AutoSize = true });
        var edit = new System.Windows.Forms.NumericUpDown { Left = left + 130, Top = top, Width = 80, Maximum = 255 };
        edit.ValueChanged += (s, e) => handler(s);
        parent.Controls.Add(edit);
        setter(edit);
        return edit;
    }

    private void InitializeComponent()
    {
        Text = "游戏参数设置";
        Width = 760;
        Height = 560;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var tabs = new System.Windows.Forms.TabControl { Left = 8, Top = 8, Width = 730, Height = 470 };
        Controls.Add(tabs);
        var speedTab = new System.Windows.Forms.TabPage { Text = "游戏速度" };
        tabs.TabPages.Add(speedTab);
        var castleTab = new System.Windows.Forms.TabPage { Text = "城堡" };
        tabs.TabPages.Add(castleTab);
        var optionTab = new System.Windows.Forms.TabPage { Text = "选项" };
        tabs.TabPages.Add(optionTab);
        var pkTab = new System.Windows.Forms.TabPage { Text = "PK" };
        tabs.TabPages.Add(pkTab);
        var testTab = new System.Windows.Forms.TabPage { Text = "测试服" };
        tabs.TabPages.Add(testTab);
        var generalTab = new System.Windows.Forms.TabPage { Text = "常规" };
        tabs.TabPages.Add(generalTab);
        var expTab = new System.Windows.Forms.TabPage { Text = "经验" };
        tabs.TabPages.Add(expTab);
        var msgTab = new System.Windows.Forms.TabPage { Text = "消息" };
        tabs.TabPages.Add(msgTab);
        var timeTab = new System.Windows.Forms.TabPage { Text = "时间" };
        tabs.TabPages.Add(timeTab);
        var priceTab = new System.Windows.Forms.TabPage { Text = "价格" };
        tabs.TabPages.Add(priceTab);
        var colorTab = new System.Windows.Forms.TabPage { Text = "消息颜色" };
        tabs.TabPages.Add(colorTab);
        var dieTab = new System.Windows.Forms.TabPage { Text = "死亡掉落" };
        tabs.TabPages.Add(dieTab);
        var statusTab = new System.Windows.Forms.TabPage { Text = "人物状态" };
        tabs.TabPages.Add(statusTab);

        var gb1 = new System.Windows.Forms.GroupBox { Text = "速度控制", Left = 8, Top = 4, Width = 350, Height = 250 };
        speedTab.Controls.Add(gb1);
        chkSpeedControl = new System.Windows.Forms.CheckBox { Text = "启用速度控制", Left = 12, Top = 16, AutoSize = true };
        chkSpeedControl.Click += (s, e) => chkSpeedControlClick(s);
        gb1.Controls.Add(chkSpeedControl);
        EditHitIntervalTime = MakeSpin("攻击间隔:", 40, 10, gb1);
        EditHitIntervalTime.ValueChanged += (s, e) => EditHitIntervalTimeChange(s);
        EditMagicHitIntervalTime = MakeSpin("魔法间隔:", 66, 10, gb1);
        EditMagicHitIntervalTime.ValueChanged += (s, e) => EditMagicHitIntervalTimeChange(s);
        EditRunIntervalTime = MakeSpin("跑步间隔:", 92, 10, gb1);
        EditRunIntervalTime.ValueChanged += (s, e) => EditRunIntervalTimeChange(s);
        EditWalkIntervalTime = MakeSpin("走路间隔:", 118, 10, gb1);
        EditWalkIntervalTime.ValueChanged += (s, e) => EditWalkIntervalTimeChange(s);
        EditTurnIntervalTime = MakeSpin("换方向间隔:", 144, 10, gb1);
        EditTurnIntervalTime.ValueChanged += (s, e) => EditTurnIntervalTimeChange(s);
        EditDigUpIntervalTime = MakeSpin("挖肉间隔:", 170, 10, gb1);
        EditDigUpIntervalTime.ValueChanged += (s, e) => EditDigUpIntervalTimeChange(s);

        var gb2 = new System.Windows.Forms.GroupBox { Text = "单位时间处理量", Left = 8, Top = 258, Width = 350, Height = 200 };
        speedTab.Controls.Add(gb2);
        EditMaxHitMsgCount = MakeSpin("攻击处理量:", 16, 10, gb2);
        EditMaxHitMsgCount.ValueChanged += (s, e) => EditMaxHitMsgCountChange(s);
        EditMaxSpellMsgCount = MakeSpin("魔法处理量:", 42, 10, gb2);
        EditMaxSpellMsgCount.ValueChanged += (s, e) => EditMaxSpellMsgCountChange(s);
        EditMaxRunMsgCount = MakeSpin("跑步处理量:", 68, 10, gb2);
        EditMaxRunMsgCount.ValueChanged += (s, e) => EditMaxRunMsgCountChange(s);
        EditMaxWalkMsgCount = MakeSpin("走路处理量:", 94, 10, gb2);
        EditMaxWalkMsgCount.ValueChanged += (s, e) => EditMaxWalkMsgCountChange(s);
        EditMaxTurnMsgCount = MakeSpin("变向处理量:", 120, 10, gb2);
        EditMaxTurnMsgCount.ValueChanged += (s, e) => EditMaxTurnMsgCountChange(s);
        EditMaxDigUpMsgCount = MakeSpin("挖肉处理量:", 16, 190, gb2);
        EditMaxDigUpMsgCount.ValueChanged += (s, e) => EditMaxDigUpMsgCountChange(s);

        var gb3 = new System.Windows.Forms.GroupBox { Text = "超速控制", Left = 366, Top = 4, Width = 350, Height = 250 };
        speedTab.Controls.Add(gb3);
        CheckBoxboKickOverSpeed = new System.Windows.Forms.CheckBox { Text = "踢超速人物下线", Left = 12, Top = 16, AutoSize = true };
        CheckBoxboKickOverSpeed.Click += (s, e) => CheckBoxboKickOverSpeedClick(s);
        gb3.Controls.Add(CheckBoxboKickOverSpeed);
        EditOverSpeedKickCount = MakeSpin("超速次数:", 40, 10, gb3);
        EditOverSpeedKickCount.ValueChanged += (s, e) => EditOverSpeedKickCountChange(s);
        EditDropOverSpeed = MakeSpin("超速掉线延时:", 66, 10, gb3);
        EditDropOverSpeed.ValueChanged += (s, e) => EditDropOverSpeedChange(s);
        RadioButtonDelyMode = new System.Windows.Forms.RadioButton { Text = "延时模式", Left = 12, Top = 94, AutoSize = true };
        RadioButtonDelyMode.Click += (s, e) => RadioButtonDelyModeClick(s);
        gb3.Controls.Add(RadioButtonDelyMode);
        RadioButtonFilterMode = new System.Windows.Forms.RadioButton { Text = "过滤模式", Left = 120, Top = 94, AutoSize = true };
        RadioButtonFilterMode.Click += (s, e) => RadioButtonFilterModeClick(s);
        gb3.Controls.Add(RadioButtonFilterMode);
        CheckBoxDisableStruck = new System.Windows.Forms.CheckBox { Text = "不显示被攻弯腰", Left = 12, Top = 120, AutoSize = true };
        CheckBoxDisableStruck.Click += (s, e) => CheckBoxDisableStruckClick(s);
        gb3.Controls.Add(CheckBoxDisableStruck);
        CheckBoxDisableSelfStruck = new System.Windows.Forms.CheckBox { Text = "自己不显示弯腰", Left = 170, Top = 120, AutoSize = true };
        CheckBoxDisableSelfStruck.Click += (s, e) => CheckBoxDisableSelfStruckClick(s);
        gb3.Controls.Add(CheckBoxDisableSelfStruck);
        chkMagicshieldStruck = new System.Windows.Forms.CheckBox { Text = "魔法盾受击弯腰", Left = 12, Top = 146, AutoSize = true };
        chkMagicshieldStruck.Click += (s, e) => chkMagicshieldStruckClick(s);
        gb3.Controls.Add(chkMagicshieldStruck);
        EditStruckTime = MakeSpin("弯腰时间:", 172, 10, gb3);
        EditStruckTime.ValueChanged += (s, e) => EditStruckTimeChange(s);
        CheckBoxSpellSendUpdateMsg = new System.Windows.Forms.CheckBox { Text = "魔法下发更新", Left = 200, Top = 40, AutoSize = true };
        CheckBoxSpellSendUpdateMsg.Click += (s, e) => CheckBoxSpellSendUpdateMsgClick(s);
        gb3.Controls.Add(CheckBoxSpellSendUpdateMsg);
        CheckBoxActionSendActionMsg = new System.Windows.Forms.CheckBox { Text = "动作下发更新", Left = 200, Top = 66, AutoSize = true };
        CheckBoxActionSendActionMsg.Click += (s, e) => CheckBoxActionSendActionMsgClick(s);
        gb3.Controls.Add(CheckBoxActionSendActionMsg);
        CheckBoxSendUpdateMsg = new System.Windows.Forms.CheckBox { Text = "下发动作更新", Left = 200, Top = 92, AutoSize = true };
        CheckBoxSendUpdateMsg.Click += (s, e) => CheckBoxSendUpdateMsgClick(s);
        gb3.Controls.Add(CheckBoxSendUpdateMsg);
        chkHorseRun3Grid = new System.Windows.Forms.CheckBox { Text = "骑马一步三格", Left = 200, Top = 118, AutoSize = true };
        chkHorseRun3Grid.Click += (s, e) => chkHorseRun3GridClick(s);
        gb3.Controls.Add(chkHorseRun3Grid);

        var gb4 = new System.Windows.Forms.GroupBox { Text = "检测动作次数", Left = 366, Top = 258, Width = 350, Height = 340 };
        speedTab.Controls.Add(gb4);
        CheckBoxCheckActionCount = new System.Windows.Forms.CheckBox { Text = "检测动作次数", Left = 12, Top = 16, AutoSize = true };
        CheckBoxCheckActionCount.Click += (s, e) => CheckBoxCheckActionCountClick(s);
        gb4.Controls.Add(CheckBoxCheckActionCount);
        EditHitCountIntervalTime = MakeSpin("攻击检测时间:", 40, 10, gb4);
        EditHitCountIntervalTime.ValueChanged += (s, e) => EditHitCountIntervalTimeChange(s);
        EditMagicHitCountIntervalTime = MakeSpin("魔法检测时间:", 66, 10, gb4);
        EditMagicHitCountIntervalTime.ValueChanged += (s, e) => EditMagicHitCountIntervalTimeChange(s);
        EditMoveCountIntervalTime = MakeSpin("移动检测时间:", 92, 10, gb4);
        EditMoveCountIntervalTime.ValueChanged += (s, e) => EditMoveCountIntervalTimeChange(s);
        EditCanHitCount = MakeSpin("攻击次数:", 118, 10, gb4);
        EditCanHitCount.ValueChanged += (s, e) => EditCanHitCountChange(s);
        EditCanMagicHitCount = MakeSpin("魔法次数:", 144, 10, gb4);
        EditCanMagicHitCount.ValueChanged += (s, e) => EditCanMagicHitCountChange(s);
        EditCanMoveCount = MakeSpin("移动次数:", 170, 10, gb4);
        EditCanMoveCount.ValueChanged += (s, e) => EditCanMoveCountChange(s);
        EditMaxHitDeliveryTime = MakeSpin("攻击下发:", 196, 10, gb4);
        EditMaxHitDeliveryTime.ValueChanged += (s, e) => EditMaxHitDeliveryTimeChange(s);
        EditMaxMagicHitDeliveryTime = MakeSpin("魔法下发:", 222, 10, gb4);
        EditMaxMagicHitDeliveryTime.ValueChanged += (s, e) => EditMaxMagicHitDeliveryTimeChange(s);
        EditMaxRunDeliveryTime = MakeSpin("跑步下发:", 248, 10, gb4);
        EditMaxRunDeliveryTime.ValueChanged += (s, e) => EditMaxRunDeliveryTimeChange(s);
        EditMaxWalkDeliveryTime = MakeSpin("走路下发:", 274, 10, gb4);
        EditMaxWalkDeliveryTime.ValueChanged += (s, e) => EditMaxWalkDeliveryTimeChange(s);
        EditMaxTurnDeliveryTime = MakeSpin("变向下发:", 300, 10, gb4);
        EditMaxTurnDeliveryTime.ValueChanged += (s, e) => EditMaxTurnDeliveryTimeChange(s);
        EditMaxDigUpDeliveryTime = MakeSpin("挖肉下发:", 326, 10, gb4);
        EditMaxDigUpDeliveryTime.ValueChanged += (s, e) => EditMaxDigUpDeliveryTimeChange(s);
        EditCheckHitCount = MakeSpin("检测攻击数:", 196, 190, gb4);
        EditCheckHitCount.ValueChanged += (s, e) => EditCheckHitCountChange(s);
        EditCheckMagicHitCount = MakeSpin("检测魔法数:", 222, 190, gb4);
        EditCheckMagicHitCount.ValueChanged += (s, e) => EditCheckMagicHitCountChange(s);
        EditCheckMoveCount = MakeSpin("检测移动数:", 248, 190, gb4);
        EditCheckMoveCount.ValueChanged += (s, e) => EditCheckMoveCountChange(s);

        ButtonActionSpeedConfig = new System.Windows.Forms.Button { Text = "动作速度设置(&A)", Left = 8, Top = 484, Width = 120, Height = 26 };
        ButtonActionSpeedConfig.Click += (s, e) => ButtonActionSpeedConfigClick(s);
        Controls.Add(ButtonActionSpeedConfig);
        ButtonGameSpeedDefault = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 480, Top = 484, Width = 100, Height = 26 };
        ButtonGameSpeedDefault.Click += (s, e) => ButtonGameSpeedDefaultClick(s);
        Controls.Add(ButtonGameSpeedDefault);
        ButtonGameSpeedSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 590, Top = 484, Width = 100, Height = 26, Enabled = false };
        ButtonGameSpeedSave.Click += (s, e) => ButtonGameSpeedSaveClick(s);
        Controls.Add(ButtonGameSpeedSave);
        ButtonCheckActionSave = new System.Windows.Forms.Button { Text = "保存检测(&C)", Left = 366, Top = 606, Width = 100, Height = 26, Enabled = false };
        ButtonCheckActionSave.Click += (s, e) => ButtonCheckActionSaveClick(s);
        Controls.Add(ButtonCheckActionSave);

        // ================= 城堡页 =================
        var castleGb1 = new System.Windows.Forms.GroupBox { Text = "城堡费用", Left = 8, Top = 4, Width = 350, Height = 200 };
        castleTab.Controls.Add(castleGb1);
        EditRepairDoorPrice = MakeSpin("维修城门:", 16, 10, castleGb1, 2000000000);
        EditRepairDoorPrice.ValueChanged += (s, e) => EditRepairDoorPriceChange(s);
        EditRepairWallPrice = MakeSpin("维修城墙:", 42, 10, castleGb1, 2000000000);
        EditRepairWallPrice.ValueChanged += (s, e) => EditRepairWallPriceChange(s);
        EditHireArcherPrice = MakeSpin("雇用弓箭手:", 68, 10, castleGb1, 2000000000);
        EditHireArcherPrice.ValueChanged += (s, e) => EditHireArcherPriceChange(s);
        EditHireGuardPrice = MakeSpin("雇用守卫:", 94, 10, castleGb1, 2000000000);
        EditHireGuardPrice.ValueChanged += (s, e) => EditHireGuardPriceChange(s);
        EditCastleGoldMax = MakeSpin("城堡存金上限:", 120, 10, castleGb1, 2000000000);
        EditCastleGoldMax.ValueChanged += (s, e) => EditCastleGoldMaxChange(s);
        EditCastleOneDayGold = MakeSpin("一天收入上限:", 146, 10, castleGb1, 2000000000);
        EditCastleOneDayGold.ValueChanged += (s, e) => EditCastleOneDayGoldChange(s);

        var castleGb2 = new System.Windows.Forms.GroupBox { Text = "城堡参数", Left = 366, Top = 4, Width = 350, Height = 200 };
        castleTab.Controls.Add(castleGb2);
        castleGb2.Controls.Add(new System.Windows.Forms.Label { Text = "城堡名称:", Left = 10, Top = 20, AutoSize = true });
        EditCastleName = new System.Windows.Forms.TextBox { Left = 90, Top = 16, Width = 120 };
        EditCastleName.TextChanged += (s, e) => EditCastleNameChange(s);
        castleGb2.Controls.Add(EditCastleName);
        castleGb2.Controls.Add(new System.Windows.Forms.Label { Text = "回城地图:", Left = 10, Top = 46, AutoSize = true });
        EditCastleHomeMap = new System.Windows.Forms.TextBox { Left = 90, Top = 42, Width = 60 };
        EditCastleHomeMap.TextChanged += (s, e) => EditCastleHomeMapChange(s);
        castleGb2.Controls.Add(EditCastleHomeMap);
        EditCastleHomeX = MakeSpin("回城X:", 68, 10, castleGb2, 5000);
        EditCastleHomeX.ValueChanged += (s, e) => EditCastleHomeXChange(s);
        EditCastleHomeY = MakeSpin("回城Y:", 94, 10, castleGb2, 5000);
        EditCastleHomeY.ValueChanged += (s, e) => EditCastleHomeYChange(s);
        EditWarRangeX = MakeSpin("攻城范围X:", 120, 10, castleGb2, 5000);
        EditWarRangeX.ValueChanged += (s, e) => EditWarRangeXChange(s);
        EditWarRangeY = MakeSpin("攻城范围Y:", 146, 10, castleGb2, 5000);
        EditWarRangeY.ValueChanged += (s, e) => EditWarRangeYChange(s);

        var castleGb3 = new System.Windows.Forms.GroupBox { Text = "交易税", Left = 8, Top = 208, Width = 350, Height = 120 };
        castleTab.Controls.Add(castleGb3);
        CheckBoxGetAllNpcTax = new System.Windows.Forms.CheckBox { Text = "收取所有NPC交易税", Left = 12, Top = 18, AutoSize = true };
        CheckBoxGetAllNpcTax.Click += (s, e) => CheckBoxGetAllNpcTaxClick(s);
        castleGb3.Controls.Add(CheckBoxGetAllNpcTax);
        EditTaxRate = MakeSpin("交易税率:", 44, 10, castleGb3, 1000);
        EditTaxRate.ValueChanged += (s, e) => EditTaxRateChange(s);
        EditCastleMemberPriceRate = MakeSpin("成员折扣率:", 70, 10, castleGb3, 1000);
        EditCastleMemberPriceRate.ValueChanged += (s, e) => EditCastleMemberPriceRateChange(s);

        ButtonCastleSave = new System.Windows.Forms.Button { Text = "保存城堡(&C)", Left = 366, Top = 380, Width = 110, Height = 26, Enabled = false };
        ButtonCastleSave.Click += (s, e) => ButtonCastleSaveClick(s);
        castleTab.Controls.Add(ButtonCastleSave);

        // ================= Option 页 =================
        var optGb1 = new System.Windows.Forms.GroupBox { Text = "跑动控制", Left = 8, Top = 4, Width = 350, Height = 260 };
        optionTab.Controls.Add(optGb1);
        chkDisHumRun = new System.Windows.Forms.CheckBox { Text = "禁止人物跑动", Left = 12, Top = 16, AutoSize = true };
        chkDisHumRun.Click += (s, e) => chkDisHumRunClick(s);
        optGb1.Controls.Add(chkDisHumRun);
        chkRunHum = MkChk("允许跑人", 24, 40, optGb1, chkRunHumClick);
        chkRunMon = MkChk("允许跑怪", 130, 40, optGb1, chkRunMonClick);
        chkRunNpc = MkChk("允许跑NPC", 24, 66, optGb1, chkRunNpcClick);
        chkRunGuard = MkChk("允许跑守卫", 130, 66, optGb1, chkRunGuardClick);
        chkWarDisHumRun = MkChk("攻城禁止跑动", 24, 92, optGb1, chkWarDisHumRunClick);
        chkWarHreoRun = MkChk("攻城允许英雄", 130, 92, optGb1, chkWarHreoRunClick);
        chkGMRunAll = MkChk("GM全图跑动", 24, 118, optGb1, chkGMRunAllClick);
        chkSafeArea = MkChk("安全区限制跑动", 24, 144, optGb1, chkSafeAreaClick);
        chkSafeAreaDisNpcRun = MkChk("安全区禁跑NPC", 24, 170, optGb1, chkSafeAreaDisNpcRunClick);
        chkSafeAreaDisShopStallHumRun = MkChk("安全区禁跑摆摊", 24, 196, optGb1, chkSafeAreaDisShopStallHumRunClick);
        chkSafeAreaDisOffLineHumRun = MkChk("安全区禁跑离线", 24, 222, optGb1, chkSafeAreaDisOffLineHumRunClick);
        chkWarDisTeleport = MkChk("攻城禁传送戒指", 130, 170, optGb1, chkWarDisTeleportClick);

        var optGb2 = new System.Windows.Forms.GroupBox { Text = "交易/掉落", Left = 366, Top = 4, Width = 350, Height = 160 };
        optionTab.Controls.Add(optGb2);
        seTryDealTime = MakeSpin("交易确认时间(秒):", 16, 10, optGb2, 60000);
        seTryDealTime.ValueChanged += (s, e) => seTryDealTimeChange(s);
        seDealOKTime = MakeSpin("完成交易时间(秒):", 42, 10, optGb2, 60000);
        seDealOKTime.ValueChanged += (s, e) => seDealOKTimeChange(s);
        chkCanNotGetBackDeal = MkChk("交易后不可取回", 200, 20, optGb2, chkCanNotGetBackDealClick);
        chkDisableDeal = MkChk("禁止交易", 200, 46, optGb2, chkDisableDealClick);
        chkControlDropItem = MkChk("控制物品丢弃", 12, 96, optGb2, chkControlDropItemClick);
        chkIsSafeDisableDrop = MkChk("安全区禁止丢弃", 160, 96, optGb2, chkIsSafeDisableDropClick);
        seCanDropPrice = MakeSpin("可丢价格:", 122, 10, optGb2, 2000000000);
        seCanDropPrice.ValueChanged += (s, e) => seCanDropPriceChange(s);
        seCanDropGold = MakeSpin("可丢金币:", 122, 190, optGb2, 2000000000);
        seCanDropGold.ValueChanged += (s, e) => seCanDropGoldChange(s);

        var optGb3 = new System.Windows.Forms.GroupBox { Text = "安全区/起点/组队", Left = 8, Top = 268, Width = 350, Height = 160 };
        optionTab.Controls.Add(optGb3);
        EditSafeZoneSize = MakeSpin("安全区大小:", 16, 10, optGb3, 1000);
        EditSafeZoneSize.ValueChanged += (s, e) => EditSafeZoneSizeChange(s);
        chkHintSafeZone = MkChk("显示安全区提示", 190, 20, optGb3, chkHintSafeZoneClick);
        seHintSafeZoneY = MakeSpin("提示Y:", 44, 10, optGb3, 5000);
        seHintSafeZoneY.ValueChanged += (s, e) => seHintSafeZoneYChange(s);
        seHintSafeZoneFColor = MakeSpin("前景色:", 70, 10, optGb3, 255);
        seHintSafeZoneFColor.ValueChanged += (s, e) => seHintSafeZoneFColorChange(s);
        seHintSafeZoneBColor = MakeSpin("背景色:", 96, 10, optGb3, 255);
        seHintSafeZoneBColor.ValueChanged += (s, e) => seHintSafeZoneBColorChange(s);
        seHintSafeZoneFSize = MakeSpin("字号:", 122, 10, optGb3, 100);
        seHintSafeZoneFSize.ValueChanged += (s, e) => seHintSafeZoneFSizeChange(s);
        EditStartPointSize = MakeSpin("起点点数:", 132, 190, optGb3, 1000);
        EditStartPointSize.ValueChanged += (s, e) => EditStartPointSizeChange(s);
        seGroupMembersMax = MakeSpin("组队人数上限:", 132, 10, optGb3, 1000);
        seGroupMembersMax.ValueChanged += (s, e) => seGroupMembersMaxChange(s);

        var optGb4 = new System.Windows.Forms.GroupBox { Text = "红名村/应急回城", Left = 366, Top = 168, Width = 350, Height = 190 };
        optionTab.Controls.Add(optGb4);
        optGb4.Controls.Add(new System.Windows.Forms.Label { Text = "红名村地图:", Left = 10, Top = 20, AutoSize = true });
        EditRedHomeMap = new System.Windows.Forms.TextBox { Left = 100, Top = 16, Width = 60 };
        optGb4.Controls.Add(EditRedHomeMap);
        EditRedHomeX = MakeSpin("红名村X:", 42, 10, optGb4, 5000);
        EditRedHomeX.ValueChanged += (s, e) => EditRedHomeXChange(s);
        EditRedHomeY = MakeSpin("红名村Y:", 68, 10, optGb4, 5000);
        EditRedHomeY.ValueChanged += (s, e) => EditRedHomeYChange(s);
        optGb4.Controls.Add(new System.Windows.Forms.Label { Text = "红死地图:", Left = 190, Top = 46, AutoSize = true });
        EditRedDieHomeMap = new System.Windows.Forms.TextBox { Left = 265, Top = 42, Width = 60 };
        optGb4.Controls.Add(EditRedDieHomeMap);
        EditRedDieHomeX = MakeSpin("红死X:", 94, 190, optGb4, 5000);
        EditRedDieHomeX.ValueChanged += (s, e) => EditRedDieHomeXChange(s);
        EditRedDieHomeY = MakeSpin("红死Y:", 120, 190, optGb4, 5000);
        EditRedDieHomeY.ValueChanged += (s, e) => EditRedDieHomeYChange(s);
        optGb4.Controls.Add(new System.Windows.Forms.Label { Text = "回城地图:", Left = 10, Top = 150, AutoSize = true });
        EditHomeMap = new System.Windows.Forms.TextBox { Left = 100, Top = 146, Width = 60 };
        optGb4.Controls.Add(EditHomeMap);
        EditHomeX = MakeSpin("回城X:", 172, 10, optGb4, 5000);
        EditHomeX.ValueChanged += (s, e) => EditHomeXChange(s);
        EditHomeY = MakeSpin("回城Y:", 172, 130, optGb4, 5000);
        EditHomeY.ValueChanged += (s, e) => EditHomeYChange(s);

        var optGb5 = new System.Windows.Forms.GroupBox { Text = "国家/挑战", Left = 8, Top = 432, Width = 350, Height = 130 };
        optionTab.Controls.Add(optGb5);
        chkNationGroupCheck = MkChk("国家组队检查", 12, 16, optGb5, chkNationGroupCheckClick);
        chkNationGuildCheck = MkChk("国家行会检查", 150, 16, optGb5, chkNationGuildCheckClick);
        seNationSayLevel = MakeSpin("国家发言等级:", 42, 10, optGb5, 65535);
        seNationSayLevel.ValueChanged += (s, e) => seNationSayLevelChange(s);
        seTryChallengeTime = MakeSpin("挑战确认(秒):", 68, 10, optGb5, 60000);
        seTryChallengeTime.ValueChanged += (s, e) => seTryChallengeTimeChange(s);
        seChallengeOKTime = MakeSpin("挑战完成(秒):", 94, 10, optGb5, 60000);
        seChallengeOKTime.ValueChanged += (s, e) => seChallengeOKTimeChange(s);
        chkCanNotGetBackChallenge = MkChk("挑战后不可取回", 200, 72, optGb5, chkCanNotGetBackChallengeClick);
        chkDisableChallenge = MkChk("禁止挑战", 200, 98, optGb5, chkDisableChallengeClick);
        seChallengeTime = MakeSpin("挑战时间(分):", 120, 10, optGb5, 60000);
        seChallengeTime.ValueChanged += (s, e) => seChallengeTimeChange(s);
        rgChallengeGold = MakeSpin("挑战附加币:", 120, 190, optGb5, 100);
        rgChallengeGold.ValueChanged += (s, e) => rgChallengeGoldClick(s);

        ButtonOptionSave = new System.Windows.Forms.Button { Text = "保存安全区(&O)", Left = 366, Top = 362, Width = 120, Height = 26, Enabled = false };
        ButtonOptionSave.Click += (s, e) => ButtonOptionSaveClick(s);
        optionTab.Controls.Add(ButtonOptionSave);
        ButtonOptionSave3 = new System.Windows.Forms.Button { Text = "保存跑动交易(&P)", Left = 366, Top = 536, Width = 130, Height = 26, Enabled = false };
        ButtonOptionSave3.Click += (s, e) => ButtonOptionSave3Click(s);
        optionTab.Controls.Add(ButtonOptionSave3);

        // ================= PK 页 =================
        var pkGb1 = new System.Windows.Forms.GroupBox { Text = "PK 点数", Left = 8, Top = 4, Width = 350, Height = 200 };
        pkTab.Controls.Add(pkGb1);
        EditDecPkPointTime = MakeSpin("减PK间隔(秒):", 16, 10, pkGb1, 2000000000);
        EditDecPkPointTime.ValueChanged += (s, e) => EditDecPkPointTimeChange(s);
        EditDecPkPointCount = MakeSpin("每次减点数:", 42, 10, pkGb1, 2000000000);
        EditDecPkPointCount.ValueChanged += (s, e) => EditDecPkPointCountChange(s);
        EditPKFlagTime = MakeSpin("PK标志时间(秒):", 68, 10, pkGb1, 2000000000);
        EditPKFlagTime.ValueChanged += (s, e) => EditPKFlagTimeChange(s);
        seHumanAddPKPoint = MakeSpin("杀人加PK点:", 94, 10, pkGb1, 2000000000);
        seHumanAddPKPoint.ValueChanged += (s, e) => seHumanAddPKPointChange(s);
        seDummyAddPKPoint = MakeSpin("假人杀人加PK:", 120, 10, pkGb1, 2000000000);
        seDummyAddPKPoint.ValueChanged += (s, e) => seDummyAddPKPointChange(s);
        seKillHeroAddPKPoint = MakeSpin("杀英雄加PK:", 146, 10, pkGb1, 2000000000);
        seKillHeroAddPKPoint.ValueChanged += (s, e) => seKillHeroAddPKPointChange(s);
        seKillHumanWeaponUnlockRate = MakeSpin("武器诅咒率:", 172, 10, pkGb1, 2000000000);
        seKillHumanWeaponUnlockRate.ValueChanged += (s, e) => seKillHumanWeaponUnlockRateChange(s);

        var pkGb2 = new System.Windows.Forms.GroupBox { Text = "杀人奖励", Left = 366, Top = 4, Width = 350, Height = 250 };
        pkTab.Controls.Add(pkGb2);
        CheckBoxKillHumanWinLevel = new System.Windows.Forms.CheckBox { Text = "杀人升级", Left = 12, Top = 16, AutoSize = true };
        CheckBoxKillHumanWinLevel.Click += (s, e) => CheckBoxKillHumanWinLevelClick(s);
        pkGb2.Controls.Add(CheckBoxKillHumanWinLevel);
        CheckBoxKilledLostLevel = new System.Windows.Forms.CheckBox { Text = "被杀降级", Left = 24, Top = 42, AutoSize = true };
        CheckBoxKilledLostLevel.Click += (s, e) => CheckBoxKilledLostLevelClick(s);
        pkGb2.Controls.Add(CheckBoxKilledLostLevel);
        EditKillHumanWinLevel = MakeSpin("升级点数:", 68, 10, pkGb2, 2000000000);
        EditKillHumanWinLevel.ValueChanged += (s, e) => EditKillHumanWinLevelChange(s);
        EditKilledLostLevel = MakeSpin("降级点数:", 94, 10, pkGb2, 2000000000);
        EditKilledLostLevel.ValueChanged += (s, e) => EditKilledLostLevelChange(s);
        CheckBoxKillHumanWinExp = new System.Windows.Forms.CheckBox { Text = "杀人得经验", Left = 12, Top = 122, AutoSize = true };
        CheckBoxKillHumanWinExp.Click += (s, e) => CheckBoxKillHumanWinExpClick(s);
        pkGb2.Controls.Add(CheckBoxKillHumanWinExp);
        CheckBoxKilledLostExp = new System.Windows.Forms.CheckBox { Text = "被杀失经验", Left = 24, Top = 148, AutoSize = true };
        CheckBoxKilledLostExp.Click += (s, e) => CheckBoxKilledLostExpClick(s);
        pkGb2.Controls.Add(CheckBoxKilledLostExp);
        EditKillHumanWinExp = MakeSpin("得经验:", 174, 10, pkGb2, 2000000000);
        EditKillHumanWinExp.ValueChanged += (s, e) => EditKillHumanWinExpChange(s);
        EditKillHumanLostExp = MakeSpin("失经验:", 200, 10, pkGb2, 2000000000);
        EditKillHumanLostExp.ValueChanged += (s, e) => EditKillHumanLostExpChange(s);
        EditHumanLevelDiffer = MakeSpin("等级差:", 200, 190, pkGb2, 65535);
        EditHumanLevelDiffer.ValueChanged += (s, e) => EditHumanLevelDifferChange(s);

        var pkGb3 = new System.Windows.Forms.GroupBox { Text = "PK 保护", Left = 8, Top = 208, Width = 350, Height = 110 };
        pkTab.Controls.Add(pkGb3);
        CheckBoxPKLevelProtect = new System.Windows.Forms.CheckBox { Text = "等级保护", Left = 12, Top = 16, AutoSize = true };
        CheckBoxPKLevelProtect.Click += (s, e) => CheckBoxPKLevelProtectClick(s);
        pkGb3.Controls.Add(CheckBoxPKLevelProtect);
        EditPKProtectLevel = MakeSpin("保护等级:", 42, 10, pkGb3, 65535);
        EditPKProtectLevel.ValueChanged += (s, e) => EditPKProtectLevelChange(s);
        EditRedPKProtectLevel = MakeSpin("红名保护:", 68, 10, pkGb3, 65535);
        EditRedPKProtectLevel.ValueChanged += (s, e) => EditRedPKProtectLevelChange(s);
        chkHeroKillHumanNotWeaponUnlock = MkChk("英雄杀人不诅咒", 150, 42, pkGb3, chkHeroKillHumanNotWeaponUnlockClick);

        ButtonOptionSave2 = new System.Windows.Forms.Button { Text = "保存PK(&K)", Left = 366, Top = 260, Width = 110, Height = 26, Enabled = false };
        ButtonOptionSave2.Click += (s, e) => ButtonOptionSave2Click(s);
        pkTab.Controls.Add(ButtonOptionSave2);

        // ================= 测试服页 =================
        var tsGb1 = new System.Windows.Forms.GroupBox { Text = "服务器模式", Left = 8, Top = 4, Width = 350, Height = 130 };
        testTab.Controls.Add(tsGb1);
        CheckBoxTestServer = new System.Windows.Forms.CheckBox { Text = "测试服务器", Left = 12, Top = 16, AutoSize = true };
        CheckBoxTestServer.Click += (s, e) => CheckBoxTestServerClick(s);
        tsGb1.Controls.Add(CheckBoxTestServer);
        CheckBoxServiceMode = MkChk("服务模式", 150, 16, tsGb1, CheckBoxServiceModeClick);
        CheckBoxVentureMode = MkChk("冒险模式", 12, 42, tsGb1, CheckBoxVentureModeClick);
        CheckBoxNonPKMode = MkChk("非PK模式", 150, 42, tsGb1, CheckBoxNonPKModeClick);
        seStartPermission = MakeSpin("初始权限:", 72, 10, tsGb1, 65535);
        seStartPermission.ValueChanged += (s, e) => seStartPermissionChange(s);
        seUserFull = MakeSpin("人数上限:", 98, 10, tsGb1, 2000000000);
        seUserFull.ValueChanged += (s, e) => seUserFullChange(s);

        var tsGb2 = new System.Windows.Forms.GroupBox { Text = "测试参数", Left = 366, Top = 4, Width = 350, Height = 130 };
        testTab.Controls.Add(tsGb2);
        seTestLevel = MakeSpin("测试等级:", 16, 10, tsGb2, 65535);
        seTestLevel.ValueChanged += (s, e) => seTestLevelChange(s);
        seTestGold = MakeSpin("测试金币:", 42, 10, tsGb2, 2000000000);
        seTestGold.ValueChanged += (s, e) => seTestGoldChange(s);
        seTestUserLimit = MakeSpin("测试人数:", 68, 10, tsGb2, 2000000000);
        seTestUserLimit.ValueChanged += (s, e) => seTestUserLimitChange(s);
        seHumanMaxGold = MakeSpin("人物金币上限:", 94, 10, tsGb2, 2000000000);
        seHumanMaxGold.ValueChanged += (s, e) => seHumanMaxGoldChange(s);

        var tsGb3 = new System.Windows.Forms.GroupBox { Text = "试玩/行会/离线", Left = 8, Top = 138, Width = 350, Height = 200 };
        testTab.Controls.Add(tsGb3);
        seHumanTryModeMaxGold = MakeSpin("试玩金币上限:", 16, 10, tsGb3, 2000000000);
        seHumanTryModeMaxGold.ValueChanged += (s, e) => seHumanTryModeMaxGoldChange(s);
        seTryModeLevel = MakeSpin("试玩等级:", 42, 10, tsGb3, 65535);
        seTryModeLevel.ValueChanged += (s, e) => seTryModeLevelChange(s);
        CheckBoxTryModeUseStorage = MkChk("试玩可用仓库", 200, 46, tsGb3, CheckBoxTryModeUseStorageClick);
        seGuildMemberMaxLimit = MakeSpin("行会人数上限:", 68, 10, tsGb3, 2000000000);
        seGuildMemberMaxLimit.ValueChanged += (s, e) => seGuildMemberMaxLimitChange(s);
        seGuildNameLen = MakeSpin("行会名长度:", 94, 10, tsGb3, 1000);
        seGuildNameLen.ValueChanged += (s, e) => seGuildNameLenChange(s);
        seGuildRankNameLen = MakeSpin("封号长度:", 120, 10, tsGb3, 1000);
        seGuildRankNameLen.ValueChanged += (s, e) => seGuildRankNameLenChange(s);
        chkOffLineShop = MkChk("离线禁摆摊", 12, 150, tsGb3, chkOffLineShopClick);
        chkOffLineHero = MkChk("离线禁英雄", 130, 150, tsGb3, chkOffLineHeroClick);
        chkOffLineSlave = MkChk("离线禁宝宝", 240, 150, tsGb3, chkOffLineSlaveClick);
        seHumChgMapOrLoginProtectTime = MakeSpin("换图保护(ms):", 172, 10, tsGb3, 2000000000);
        seHumChgMapOrLoginProtectTime.ValueChanged += (s, e) => seHumChgMapOrLoginProtectTimeChange(s);

        var tsGb4 = new System.Windows.Forms.GroupBox { Text = "NPC 商店", Left = 366, Top = 138, Width = 350, Height = 90 };
        testTab.Controls.Add(tsGb4);
        chkSellItemToNpcShopNoCalcAddProperty = MkChk("卖出不算极品属性", 12, 16, tsGb4, chkSellItemToNpcShopNoCalcAddPropertyClick);
        chkShowNewValueFromBuyNpcItem = MkChk("买入显示极品属性", 12, 46, tsGb4, chkShowNewValueFromBuyNpcItemClick);

        ButtonOptionSave0 = new System.Windows.Forms.Button { Text = "保存测试服(&T)", Left = 366, Top = 240, Width = 120, Height = 26, Enabled = false };
        ButtonOptionSave0.Click += (s, e) => ButtonOptionSave0Click(s);
        testTab.Controls.Add(ButtonOptionSave0);

        // ================= General 常规页 =================
        var genGb1 = new System.Windows.Forms.GroupBox { Text = "公告/版本", Left = 8, Top = 4, Width = 350, Height = 200 };
        generalTab.Controls.Add(genGb1);
        genGb1.Controls.Add(new System.Windows.Forms.Label { Text = "客户端版本日期:", Left = 10, Top = 20, AutoSize = true });
        EditSoftVersionDate = new System.Windows.Forms.TextBox { Left = 130, Top = 16, Width = 110 };
        EditSoftVersionDate.TextChanged += (s, e) => EditSoftVersionDateChange(s);
        genGb1.Controls.Add(EditSoftVersionDate);
        EditConsoleShowUserCountTime = MakeSpin("在线人数显示(秒):", 42, 10, genGb1, 2000000000);
        EditConsoleShowUserCountTime.ValueChanged += (s, e) => EditConsoleShowUserCountTimeChange(s);
        EditShowLineNoticeTime = MakeSpin("公告间隔(秒):", 68, 10, genGb1, 2000000000);
        EditShowLineNoticeTime.ValueChanged += (s, e) => EditShowLineNoticeTimeChange(s);
        genGb1.Controls.Add(new System.Windows.Forms.Label { Text = "公告颜色:", Left = 10, Top = 98, AutoSize = true });
        ComboBoxLineNoticeColor = new System.Windows.Forms.ComboBox { Left = 140, Top = 94, Width = 100, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        ComboBoxLineNoticeColor.Items.Add("红色");
        ComboBoxLineNoticeColor.Items.Add("绿色");
        ComboBoxLineNoticeColor.Items.Add("蓝色");
        ComboBoxLineNoticeColor.SelectedIndexChanged += (s, e) => ComboBoxLineNoticeColorChange(s);
        genGb1.Controls.Add(ComboBoxLineNoticeColor);
        genGb1.Controls.Add(new System.Windows.Forms.Label { Text = "公告前缀:", Left = 10, Top = 126, AutoSize = true });
        EditLineNoticePreFix = new System.Windows.Forms.TextBox { Left = 140, Top = 122, Width = 110 };
        EditLineNoticePreFix.TextChanged += (s, e) => EditLineNoticePreFixChange(s);
        genGb1.Controls.Add(EditLineNoticePreFix);
        CheckBoxShowMakeItemMsg = MkChk("显示造物消息", 12, 152, genGb1, CheckBoxShowMakeItemMsgClick);
        CbViewHack = MkChk("显示外挂消息", 150, 152, genGb1, CbViewHackClick);
        CkViewAdmfail = MkChk("显示登录失败", 12, 178, genGb1, CkViewAdmfailClick);
        CheckBoxShowExceptionMsg = MkChk("显示异常消息", 150, 178, genGb1, CheckBoxShowExceptionMsgClick);

        var genGb2 = new System.Windows.Forms.GroupBox { Text = "在线人数/掉率", Left = 366, Top = 4, Width = 350, Height = 200 };
        generalTab.Controls.Add(genGb2);
        CheckBoxCanOldClientLogon = MkChk("允许旧客户端登录", 12, 16, genGb2, CheckBoxCanOldClientLogonClick);
        chkOldClient = MkChk("旧客户端模式", 170, 16, genGb2, chkOldClientClick);
        CheckBoxSendOnlineCount = MkChk("发送在线人数", 12, 42, genGb2, CheckBoxSendOnlineCountClick);
        EditSendOnlineCountRate = MakeSpin("发送比率:", 68, 10, genGb2, 2000000000);
        EditSendOnlineCountRate.ValueChanged += (s, e) => EditSendOnlineCountRateChange(s);
        EditSendOnlineTime = MakeSpin("发送间隔(秒):", 94, 10, genGb2, 2000000000);
        EditSendOnlineTime.ValueChanged += (s, e) => EditSendOnlineTimeChange(s);
        EditMonsterPowerRate = MakeSpin("怪物攻防倍率:", 120, 10, genGb2, 2000000000);
        EditMonsterPowerRate.ValueChanged += (s, e) => EditMonsterPowerRateChange(s);
        EditEditItemsPowerRate = MakeSpin("物品攻防倍率:", 146, 10, genGb2, 2000000000);
        EditEditItemsPowerRate.ValueChanged += (s, e) => EditEditItemsPowerRateChange(s);
        EditItemsACPowerRate = MakeSpin("物品防具倍率:", 172, 10, genGb2, 2000000000);
        EditItemsACPowerRate.ValueChanged += (s, e) => EditItemsACPowerRateChange(s);

        var genGb3 = new System.Windows.Forms.GroupBox { Text = "聊天记录", Left = 8, Top = 208, Width = 350, Height = 160 };
        generalTab.Controls.Add(genGb3);
        chkRecordPublicMsg = MkChk("记录公聊", 12, 16, genGb3, chkRecordPublicMsgClick);
        chkRecordPrivateMsg = MkChk("记录私聊", 130, 16, genGb3, chkRecordPrivateMsgClick);
        chkRecordGuildMsg = MkChk("记录行会", 248, 16, genGb3, chkRecordGuildMsgClick);
        chkRecordCryCryMsg = MkChk("记录喊话", 12, 42, genGb3, chkRecordCryCryMsgClick);
        chkRecordGroupMsg = MkChk("记录组队", 130, 42, genGb3, chkRecordGroupMsgClick);
        chkRecordNationMsg = MkChk("记录国家", 248, 42, genGb3, chkRecordNationMsgClick);
        chkPermissionChangeLog = MkChk("权限变更日志", 12, 68, genGb3, chkPermissionChangeLogClick);

        ButtonGeneralSave = new System.Windows.Forms.Button { Text = "保存常规(&G)", Left = 8, Top = 374, Width = 110, Height = 26, Enabled = false };
        ButtonGeneralSave.Click += (s, e) => ButtonGeneralSaveClick(s);
        generalTab.Controls.Add(ButtonGeneralSave);

        // ================= 经验页 =================
        var expGb1 = new System.Windows.Forms.GroupBox { Text = "杀怪经验", Left = 8, Top = 4, Width = 350, Height = 130 };
        expTab.Controls.Add(expGb1);
        EditKillMonExpMultiple = MakeSpin("经验倍数:", 16, 10, expGb1, 2000000000);
        EditKillMonExpMultiple.ValueChanged += (s, e) => EditKillMonExpMultipleChange(s);
        CheckBoxHighLevelKillMonFixExp = MkChk("高等级经验不变", 12, 46, expGb1, CheckBoxHighLevelKillMonFixExpClick);
        CheckBoxHighLevelGroupFixExp = MkChk("高等级组队不变", 12, 72, expGb1, CheckBoxHighLevelGroupFixExpClick);
        EditMaxUpLevelCount = MakeSpin("最高升级次数:", 98, 10, expGb1, 2000000000);
        EditMaxUpLevelCount.ValueChanged += (s, e) => EditMaxUpLevelCountChange(s);
        CheckBoxFixExp = MkChk("固定经验", 200, 46, expGb1, CheckBoxFixExpClick);
        CheckBoxLimitChangeExp = MkChk("限制经验变更", 200, 72, expGb1, CheckBoxLimitChangeExpClick);
        EditHighLevel = MakeSpin("高等级:", 124, 10, expGb1, 2000000000);
        EditHighLevel.ValueChanged += (s, e) => EditHighLevelChange(s);
        EditHighLevelGetExp = MakeSpin("高等级经验:", 150, 10, expGb1, 2000000000);
        EditHighLevelGetExp.ValueChanged += (s, e) => EditHighLevelGetExpChange(s);

        var expGb2 = new System.Windows.Forms.GroupBox { Text = "固定/上限", Left = 366, Top = 4, Width = 350, Height = 130 };
        expTab.Controls.Add(expGb2);
        SpinEditBaseExp = MakeSpin("基数经验:", 16, 10, expGb2, 2000000000);
        SpinEditBaseExp.ValueChanged += (s, e) => SpinEditBaseExpChange(s);
        SpinEditAddExp = MakeSpin("增量经验:", 42, 10, expGb2, 2000000000);
        SpinEditAddExp.ValueChanged += (s, e) => SpinEditAddExpChange(s);
        expGb2.Controls.Add(new System.Windows.Forms.Label { Text = "最高有效等级:", Left = 10, Top = 74, AutoSize = true });
        RadioGroupMaxLevel = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 70, Width = 80, Maximum = 2 };
        RadioGroupMaxLevel.ValueChanged += (s, e) => RadioGroupMaxLevelClick(s);
        expGb2.Controls.Add(RadioGroupMaxLevel);
        expGb2.Controls.Add(new System.Windows.Forms.Label { Text = "防具上限:", Left = 10, Top = 100, AutoSize = true });
        rgMaxAC = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 96, Width = 80, Maximum = 2 };
        rgMaxAC.ValueChanged += (s, e) => rgMaxACClick(s);
        expGb2.Controls.Add(rgMaxAC);
        rgMaxHitPoint = MakeSpin("血上限类型:", 122, 10, expGb2, 2);
        rgMaxHitPoint.ValueChanged += (s, e) => rgMaxHitPointClick(s);

        expTab.Controls.Add(new System.Windows.Forms.Label { Text = "升级经验表（等级/经验值）", Left = 8, Top = 138, AutoSize = true });
        expTab.Controls.Add(new System.Windows.Forms.Label { Text = "经验倍率表（等级/百分比）", Left = 370, Top = 138, AutoSize = true });
        GridLevelExp = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 156,
            Width = 350,
            Height = 300,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
        };
        GridLevelExp.Columns.Add("Level", "等级");
        GridLevelExp.Columns.Add("Exp", "经验值");
        GridLevelExp.Columns[0].Width = 60;
        GridLevelExp.Columns[0].ReadOnly = true;
        GridLevelExp.Columns[1].Width = 240;
        GridLevelExp.Rows.Add(1000);
        for (int i = 1; i <= 1000; i++)
            GridLevelExp.Rows[i - 1].Cells[0].Value = i;
        GridLevelExp.CellEndEdit += (s, e) => GridLevelExpSetEditText(s, e.RowIndex);
        expTab.Controls.Add(GridLevelExp);

        GridLevelExpRate = new System.Windows.Forms.DataGridView
        {
            Left = 370,
            Top = 156,
            Width = 346,
            Height = 300,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
        };
        GridLevelExpRate.Columns.Add("Level", "等级");
        GridLevelExpRate.Columns.Add("Rate", "百分比");
        GridLevelExpRate.Columns[0].Width = 60;
        GridLevelExpRate.Columns[1].Width = 240;
        GridLevelExpRate.Rows.Add(1000);
        for (int i = 1; i <= 1000; i++)
            GridLevelExpRate.Rows[i - 1].Cells[0].Value = i;
        expTab.Controls.Add(GridLevelExpRate);

        expTab.Controls.Add(new System.Windows.Forms.Label { Text = "经验计划:", Left = 8, Top = 460, AutoSize = true });
        ComboBoxLevelExp = new System.Windows.Forms.ComboBox { Left = 80, Top = 456, Width = 200, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        foreach (var name in new[]
        {
            "原始经验值", "标准经验值", "当前1/2倍经验", "当前1/5倍经验", "当前1/8倍经验", "当前1/10倍经验",
            "当前1/20倍经验", "当前1/30倍经验", "当前1/40倍经验", "当前1/50倍经验", "当前1/60倍经验",
            "当前1/70倍经验", "当前1/80倍经验", "当前1/90倍经验", "当前1/100倍经验", "当前1/150倍经验",
            "当前1/200倍经验", "当前1/250倍经验", "当前1/300倍经验",
        })
        {
            ComboBoxLevelExp.Items.Add(name);
        }
        ComboBoxLevelExp.SelectedIndexChanged += (s, e) => ComboBoxLevelExpClick(s);
        expTab.Controls.Add(ComboBoxLevelExp);

        ButtonExpSave = new System.Windows.Forms.Button { Text = "保存经验(&E)", Left = 8, Top = 600, Width = 110, Height = 26, Enabled = false };
        ButtonExpSave.Click += (s, e) => ButtonExpSaveClick(s);
        expTab.Controls.Add(ButtonExpSave);

        // ================= Msg 页 =================
        var msgGb1 = new System.Windows.Forms.GroupBox { Text = "聊天限制", Left = 8, Top = 4, Width = 350, Height = 230 };
        msgTab.Controls.Add(msgGb1);
        EditSayMsgMaxLen = MakeSpin("单条长度:", 16, 10, msgGb1, 2000000000);
        EditSayMsgMaxLen.ValueChanged += (s, e) => EditSayMsgMaxLenChange(s);
        EditSayRedMsgMaxLen = MakeSpin("红字长度:", 42, 10, msgGb1, 2000000000);
        EditSayRedMsgMaxLen.ValueChanged += (s, e) => EditSayRedMsgMaxLenChange(s);
        EditCanShoutMsgLevel = MakeSpin("喊话等级:", 68, 10, msgGb1, 65535);
        EditCanShoutMsgLevel.ValueChanged += (s, e) => EditCanShoutMsgLevelChange(s);
        EditSayMsgTime = MakeSpin("发言间隔(秒):", 94, 10, msgGb1, 2000000000);
        EditSayMsgTime.ValueChanged += (s, e) => EditSayMsgTimeChange(s);
        EditSayMsgCount = MakeSpin("间隔条数:", 120, 10, msgGb1, 2000000000);
        EditSayMsgCount.ValueChanged += (s, e) => EditSayMsgCountChange(s);
        EditDisableSayMsgTime = MakeSpin("禁言时间(秒):", 146, 10, msgGb1, 2000000000);
        EditDisableSayMsgTime.ValueChanged += (s, e) => EditDisableSayMsgTimeChange(s);
        EditUserItemSayMsgTime = MakeSpin("物品喊话间隔:", 172, 10, msgGb1, 2000000000);
        EditUserItemSayMsgTime.ValueChanged += (s, e) => EditUserItemSayMsgTimeChange(s);
        seMaxInputStringLen = MakeSpin("输入长度上限:", 198, 10, msgGb1, 2000000000);
        seMaxInputStringLen.ValueChanged += (s, e) => seMaxInputStringLenChange(s);

        var msgGb2 = new System.Windows.Forms.GroupBox { Text = "消息开关", Left = 366, Top = 4, Width = 350, Height = 160 };
        msgTab.Controls.Add(msgGb2);
        CheckBoxShutRedMsgShowGMName = MkChk("禁言显示GM名", 12, 16, msgGb2, CheckBoxShutRedMsgShowGMNameClick);
        CheckBoxShowPreFixMsg = MkChk("显示消息前缀", 12, 42, msgGb2, CheckBoxShowPreFixMsgClick);
        CheckBoxShowWhisperLevelMsg = MkChk("私聊显示等级", 12, 68, msgGb2, CheckBoxShowWhisperLevelMsgClick);
        msgGb2.Controls.Add(new System.Windows.Forms.Label { Text = "GM红字命令:", Left = 10, Top = 100, AutoSize = true });
        EditGMRedMsgCmd = new System.Windows.Forms.TextBox { Left = 110, Top = 96, Width = 60 };
        EditGMRedMsgCmd.TextChanged += (s, e) => EditGMRedMsgCmdChange(s);
        msgGb2.Controls.Add(EditGMRedMsgCmd);
        msgGb2.Controls.Add(new System.Windows.Forms.Label { Text = "私聊后缀:", Left = 10, Top = 128, AutoSize = true });
        EditShowWhisperLevelMsg = new System.Windows.Forms.TextBox { Left = 110, Top = 124, Width = 140 };
        msgGb2.Controls.Add(EditShowWhisperLevelMsg);

        ButtonMsgSave = new System.Windows.Forms.Button { Text = "保存消息(&M)", Left = 366, Top = 170, Width = 110, Height = 26, Enabled = false };
        ButtonMsgSave.Click += (s, e) => ButtonMsgSaveClick(s);
        msgTab.Controls.Add(ButtonMsgSave);

        // ================= Time 页 =================
        var timeGb1 = new System.Windows.Forms.GroupBox { Text = "攻城时间", Left = 8, Top = 4, Width = 350, Height = 160 };
        timeTab.Controls.Add(timeGb1);
        EditStartCastleWarDays = MakeSpin("开战天数:", 16, 10, timeGb1, 2000000000);
        EditStartCastleWarDays.ValueChanged += (s, e) => EditStartCastleWarDaysChange(s);
        EditStartCastlewarTime = MakeSpin("开战时刻:", 42, 10, timeGb1, 2000000000);
        EditStartCastlewarTime.ValueChanged += (s, e) => EditStartCastlewarTimeChange(s);
        EditShowCastleWarEndMsgTime = MakeSpin("结束提示(分):", 68, 10, timeGb1, 2000000000);
        EditShowCastleWarEndMsgTime.ValueChanged += (s, e) => EditShowCastleWarEndMsgTimeChange(s);
        EditCastleWarTime = MakeSpin("攻城时长(分):", 94, 10, timeGb1, 2000000000);
        EditCastleWarTime.ValueChanged += (s, e) => EditCastleWarTimeChange(s);
        EditGetCastleTime = MakeSpin("占领时长(分):", 120, 10, timeGb1, 2000000000);
        EditGetCastleTime.ValueChanged += (s, e) => EditGetCastleTimeChange(s);
        EditGuildWarTime = MakeSpin("行会战(分):", 146, 10, timeGb1, 2000000000);
        EditGuildWarTime.ValueChanged += (s, e) => EditGuildWarTimeChange(s);

        var timeGb2 = new System.Windows.Forms.GroupBox { Text = "清理/保存时间", Left = 366, Top = 4, Width = 350, Height = 260 };
        timeTab.Controls.Add(timeGb2);
        seMakeGhostTime = MakeSpin("尸体清理(秒):", 16, 10, timeGb2, 2000000000);
        seMakeGhostTime.ValueChanged += (s, e) => seMakeGhostTimeChange(s);
        seMakeMonGhostTime = MakeSpin("怪尸清理(秒):", 42, 10, timeGb2, 2000000000);
        seMakeMonGhostTime.ValueChanged += (s, e) => seMakeMonGhostTimeChange(s);
        seMakeDummyGhostTime = MakeSpin("假人清理(秒):", 68, 10, timeGb2, 2000000000);
        seMakeDummyGhostTime.ValueChanged += (s, e) => seMakeDummyGhostTimeChange(s);
        seClearDropOnFloorItemTime = MakeSpin("物品清理(秒):", 94, 10, timeGb2, 2000000000);
        seClearDropOnFloorItemTime.ValueChanged += (s, e) => seClearDropOnFloorItemTimeChange(s);
        seFloorItemCanPickUpTime = MakeSpin("捡物延时(秒):", 120, 10, timeGb2, 2000000000);
        seFloorItemCanPickUpTime.ValueChanged += (s, e) => seFloorItemCanPickUpTimeChange(s);
        seSaveHumanRcdTime = MakeSpin("存档间隔(分):", 146, 10, timeGb2, 2000000000);
        seSaveHumanRcdTime.ValueChanged += (s, e) => seSaveHumanRcdTimeChange(s);
        seHumanFreeDelayTime = MakeSpin("释放延时(分):", 172, 10, timeGb2, 2000000000);
        seHumanFreeDelayTime.ValueChanged += (s, e) => seHumanFreeDelayTimeChange(s);
        seGetDBSockMsgTime = MakeSpin("存取超时(秒):", 198, 10, timeGb2, 2000000000);
        seGetDBSockMsgTime.ValueChanged += (s, e) => seGetDBSockMsgTimeChange(s);
        seDearRecallTime = MakeSpin("夫妻传送:", 224, 10, timeGb2, 2000000000);
        seDearRecallTime.ValueChanged += (s, e) => seDearRecallTimeChange(s);
        seMasterRecallTime = MakeSpin("师徒传送:", 224, 190, timeGb2, 2000000000);
        seMasterRecallTime.ValueChanged += (s, e) => seMasterRecallTimeChange(s);

        var timeGb3 = new System.Windows.Forms.GroupBox { Text = "骑马/NPC", Left = 8, Top = 168, Width = 350, Height = 160 };
        timeTab.Controls.Add(timeGb3);
        seHorseTakeTime = MakeSpin("上下马间隔:", 16, 10, timeGb3, 2000000000);
        seHorseTakeTime.ValueChanged += (s, e) => seHorseTakeTimeChange(s);
        seTakeOnHorseUseTime = MakeSpin("上马准备:", 42, 10, timeGb3, 2000000000);
        seTakeOnHorseUseTime.ValueChanged += (s, e) => seTakeOnHorseUseTimeChange(s);
        chkReadyOnHorseDisableAction = MkChk("准备期禁动作", 200, 46, timeGb3, chkReadyOnHorseDisableActionClick);
        seNpcButtonClickTime = MakeSpin("NPC按钮间隔:", 68, 10, timeGb3, 2000000000);
        seNpcButtonClickTime.ValueChanged += (s, e) => seNpcButtonClickTimeChange(s);
        seNpcActorClickTime = MakeSpin("NPC对象间隔:", 94, 10, timeGb3, 2000000000);
        seNpcActorClickTime.ValueChanged += (s, e) => seNpcActorClickTimeChange(s);
        sePlayerVarJClearTime = MakeSpin("J变量清理:", 120, 10, timeGb3, 255);
        sePlayerVarJClearTime.ValueChanged += (s, e) => sePlayerVarJClearTimeChange(s);
        seGroupRecallTime = MakeSpin("记忆传送:", 146, 10, timeGb3, 2000000000);
        seGroupRecallTime.ValueChanged += (s, e) => seGroupRecallTimeChange(s);

        ButtonTimeSave = new System.Windows.Forms.Button { Text = "保存时间(&I)", Left = 8, Top = 334, Width = 110, Height = 26, Enabled = false };
        ButtonTimeSave.Click += (s, e) => ButtonTimeSaveClick(s);
        timeTab.Controls.Add(ButtonTimeSave);

        // ================= Price 页 =================
        var priceGb = new System.Windows.Forms.GroupBox { Text = "费用", Left = 8, Top = 4, Width = 350, Height = 160 };
        priceTab.Controls.Add(priceGb);
        EditBuildGuildPrice = MakeSpin("建行会费用:", 16, 10, priceGb, 2000000000);
        EditBuildGuildPrice.ValueChanged += (s, e) => EditBuildGuildPriceChange(s);
        EditGuildWarPrice = MakeSpin("行会战费用:", 42, 10, priceGb, 2000000000);
        EditGuildWarPrice.ValueChanged += (s, e) => EditGuildWarPriceChange(s);
        EditMakeDurgPrice = MakeSpin("配药费用:", 68, 10, priceGb, 2000000000);
        EditMakeDurgPrice.ValueChanged += (s, e) => EditMakeDurgPriceChange(s);
        EditSuperRepairPriceRate = MakeSpin("特修倍数:", 94, 10, priceGb, 2000000000);
        EditSuperRepairPriceRate.ValueChanged += (s, e) => EditSuperRepairPriceRateChange(s);
        EditRepairItemDecDura = MakeSpin("修理掉持久:", 120, 10, priceGb, 2000000000);
        EditRepairItemDecDura.ValueChanged += (s, e) => EditRepairItemDecDuraChange(s);

        ButtonPriceSave = new System.Windows.Forms.Button { Text = "保存价格(&P)", Left = 8, Top = 170, Width = 110, Height = 26, Enabled = false };
        ButtonPriceSave.Click += (s, e) => ButtonPriceSaveClick(s);
        priceTab.Controls.Add(ButtonPriceSave);

        // ================= MsgColor 页（批次J25，21 色对 + NPC 标签） =================
        BindColor(colorTab, "听说话前景:", 16, () => EditHearMsgFColor, v => EditHearMsgFColor = v, EditHearMsgFColorChange);
        BindColor(colorTab, "听说话背景:", 42, () => EdittHearMsgBColor, v => EdittHearMsgBColor = v, EdittHearMsgBColorChange);
        BindColor(colorTab, "私聊前景:", 68, () => EditWhisperMsgFColor, v => EditWhisperMsgFColor = v, EditWhisperMsgFColorChange);
        BindColor(colorTab, "私聊背景:", 94, () => EditWhisperMsgBColor, v => EditWhisperMsgBColor = v, EditWhisperMsgBColorChange);
        BindColor(colorTab, "GM私聊前景:", 120, () => EditGMWhisperMsgFColor, v => EditGMWhisperMsgFColor = v, EditGMWhisperMsgFColorChange);
        BindColor(colorTab, "GM私聊背景:", 146, () => EditGMWhisperMsgBColor, v => EditGMWhisperMsgBColor = v, EditGMWhisperMsgBColorChange);
        BindColor(colorTab, "发私聊前景:", 172, () => seSendWhisperMsgFColor, v => seSendWhisperMsgFColor = v, seSendWhisperMsgFColorChange);
        BindColor(colorTab, "发私聊背景:", 198, () => seSendWhisperMsgBColor, v => seSendWhisperMsgBColor = v, seSendWhisperMsgBColorChange);
        BindColor(colorTab, "元宝刷新前景:", 224, () => seRefreshGameGoldFColor, v => seRefreshGameGoldFColor = v, seRefreshGameGoldFColorChange);
        BindColor(colorTab, "元宝刷新背景:", 250, () => seRefreshGameGoldBColor, v => seRefreshGameGoldBColor = v, seRefreshGameGoldBColorChange);
        BindColor(colorTab, "显示私聊前景:", 276, () => seShowWhisperFColor, v => seShowWhisperFColor = v, seShowWhisperFColorChange);
        BindColor(colorTab, "显示私聊背景:", 302, () => seShowWhisperBColor, v => seShowWhisperBColor = v, seShowWhisperBColorChange);
        BindColor(colorTab, "关闭私聊前景:", 328, () => seCloseWhisperFColor, v => seCloseWhisperFColor = v, seCloseWhisperFColorChange);
        BindColor(colorTab, "关闭私聊背景:", 354, () => seCloseWhisperBColor, v => seCloseWhisperBColor = v, seCloseWhisperBColorChange);
        BindColor(colorTab, "红字前景:", 16, 200, () => EditRedMsgFColor, v => EditRedMsgFColor = v, EditRedMsgFColorChange);
        BindColor(colorTab, "红字背景:", 42, 200, () => EditRedMsgBColor, v => EditRedMsgBColor = v, EditRedMsgBColorChange);
        BindColor(colorTab, "绿字前景:", 68, 200, () => EditGreenMsgFColor, v => EditGreenMsgFColor = v, EditGreenMsgFColorChange);
        BindColor(colorTab, "绿字背景:", 94, 200, () => EditGreenMsgBColor, v => EditGreenMsgBColor = v, EditGreenMsgBColorChange);
        BindColor(colorTab, "蓝字前景:", 120, 200, () => EditBlueMsgFColor, v => EditBlueMsgFColor = v, EditBlueMsgFColorChange);
        BindColor(colorTab, "蓝字背景:", 146, 200, () => EditBlueMsgBColor, v => EditBlueMsgBColor = v, EditBlueMsgBColorChange);
        BindColor(colorTab, "喊话前景:", 172, 200, () => EditCryMsgFColor, v => EditCryMsgFColor = v, EditCryMsgFColorChange);
        BindColor(colorTab, "喊话背景:", 198, 200, () => EditCryMsgBColor, v => EditCryMsgBColor = v, EditCryMsgBColorChange);
        BindColor(colorTab, "行会前景:", 224, 200, () => EditGuildMsgFColor, v => EditGuildMsgFColor = v, EditGuildMsgFColorChange);
        BindColor(colorTab, "行会背景:", 250, 200, () => EditGuildMsgBColor, v => EditGuildMsgBColor = v, EditGuildMsgBColorChange);
        BindColor(colorTab, "组队前景:", 276, 200, () => EditGroupMsgFColor, v => EditGroupMsgFColor = v, EditGroupMsgFColorChange);
        BindColor(colorTab, "组队背景:", 302, 200, () => EditGroupMsgBColor, v => EditGroupMsgBColor = v, EditGroupMsgBColorChange);
        BindColor(colorTab, "祝福前景:", 328, 200, () => EditCustMsgFColor, v => EditCustMsgFColor = v, EditCustMsgFColorChange);
        BindColor(colorTab, "祝福背景:", 354, 200, () => EditCustMsgBColor, v => EditCustMsgBColor = v, EditCustMsgBColorChange);
        BindColor(colorTab, "国家前景:", 380, 200, () => EditNationMsgFColor, v => EditNationMsgFColor = v, EditNationMsgFColorChange);
        BindColor(colorTab, "国家背景:", 380, 200, () => EditNationMsgBColor, v => EditNationMsgBColor = v, EditNationMsgBColorChange);
        BindColor(colorTab, "千里传音前景:", 406, 200, () => EditUserSayMsgFColor, v => EditUserSayMsgFColor = v, EditUserSayMsgFColorChange);
        BindColor(colorTab, "千里传音背景:", 432, 200, () => EditUserSayMsgBColor, v => EditUserSayMsgBColor = v, EditUserSayMsgBColorChange);
        BindColor(colorTab, "传音筒前景:", 458, 200, () => EditTopUserSayMsgFColor, v => EditTopUserSayMsgFColor = v, EditTopUserSayMsgFColorChange);
        BindColor(colorTab, "传音筒背景:", 484, 200, () => EditTopUserSayMsgBColor, v => EditTopUserSayMsgBColor = v, EditTopUserSayMsgBColorChange);
        BindColor(colorTab, "掉落提示前景:", 510, 200, () => EditDropItemFColor, v => EditDropItemFColor = v, EditDropItemFColorChange);
        BindColor(colorTab, "掉落提示背景:", 536, 200, () => EditDropItemBColor, v => EditDropItemBColor = v, EditDropItemBColorChange);
        chkNPCLabelFontStroke = new System.Windows.Forms.CheckBox { Text = "NPC标签描边", Left = 380, Top = 562, AutoSize = true };
        chkNPCLabelFontStroke.Click += (s, e) => chkNPCLabelFontStrokeClick(s);
        colorTab.Controls.Add(chkNPCLabelFontStroke);
        BindColor(colorTab, "NPC标签常态:", 588, 380, () => seNPCLabelNormalColor, v => seNPCLabelNormalColor = v, seNPCLabelNormalColorChange);
        BindColor(colorTab, "NPC标签悬停:", 614, 380, () => seNPCLabelMouseMoveColor, v => seNPCLabelMouseMoveColor = v, seNPCLabelMouseMoveColorChange);
        BindColor(colorTab, "NPC标签按下:", 640, 380, () => seNPCLabelMouseDownColor, v => seNPCLabelMouseDownColor = v, seNPCLabelMouseDownColorChange);

        ButtonMsgColorSave = new System.Windows.Forms.Button { Text = "保存颜色(&C)", Left = 8, Top = 640, Width = 110, Height = 26, Enabled = false };
        ButtonMsgColorSave.Click += (s, e) => ButtonMsgColorSaveClick(s);
        colorTab.Controls.Add(ButtonMsgColorSave);

        // ================= 死亡掉落页（批次J25） =================
        var dieGb1 = new System.Windows.Forms.GroupBox { Text = "死亡掉落", Left = 8, Top = 4, Width = 350, Height = 210 };
        dieTab.Controls.Add(dieGb1);
        dieGb1.Controls.Add(new System.Windows.Forms.Label { Text = "掉装备几率:", Left = 10, Top = 20, AutoSize = true });
        ScrollBarDieDropUseItemRate = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 16, Width = 80, Minimum = 1, Maximum = 200 };
        ScrollBarDieDropUseItemRate.ValueChanged += (s, e) => ScrollBarDieDropUseItemRateChange(s);
        dieGb1.Controls.Add(ScrollBarDieDropUseItemRate);
        EditDieDropUseItemRate = new System.Windows.Forms.TextBox { Left = 230, Top = 16, Width = 60, ReadOnly = true };
        dieGb1.Controls.Add(EditDieDropUseItemRate);
        dieGb1.Controls.Add(new System.Windows.Forms.Label { Text = "红名掉装备:", Left = 10, Top = 46, AutoSize = true });
        ScrollBarDieRedDropUseItemRate = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 42, Width = 80, Minimum = 1, Maximum = 200 };
        ScrollBarDieRedDropUseItemRate.ValueChanged += (s, e) => ScrollBarDieRedDropUseItemRateChange(s);
        dieGb1.Controls.Add(ScrollBarDieRedDropUseItemRate);
        EditDieRedDropUseItemRate = new System.Windows.Forms.TextBox { Left = 230, Top = 42, Width = 60, ReadOnly = true };
        dieGb1.Controls.Add(EditDieRedDropUseItemRate);
        dieGb1.Controls.Add(new System.Windows.Forms.Label { Text = "散包几率:", Left = 10, Top = 72, AutoSize = true });
        ScrollBarDieScatterBagRate = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 68, Width = 80, Minimum = 1, Maximum = 200 };
        ScrollBarDieScatterBagRate.ValueChanged += (s, e) => ScrollBarDieScatterBagRateChange(s);
        dieGb1.Controls.Add(ScrollBarDieScatterBagRate);
        EditDieScatterBagRate = new System.Windows.Forms.TextBox { Left = 230, Top = 68, Width = 60, ReadOnly = true };
        dieGb1.Controls.Add(EditDieScatterBagRate);
        CheckBoxKillByMonstDropUseItem = MkChk("被怪杀掉装备", 12, 100, dieGb1, CheckBoxKillByMonstDropUseItemClick);
        CheckBoxKillByHumanDropUseItem = MkChk("被人杀掉装备", 160, 100, dieGb1, CheckBoxKillByHumanDropUseItemClick);
        CheckBoxDieScatterBag = MkChk("死亡散包", 12, 126, dieGb1, CheckBoxDieScatterBagClick);
        CheckBoxDieDropGold = MkChk("死亡掉金币", 160, 126, dieGb1, CheckBoxDieDropGoldClick);
        CheckBoxDieRedScatterBagAll = MkChk("红名全散包", 12, 152, dieGb1, CheckBoxDieRedScatterBagAllClick);
        dieGb1.Controls.Add(new System.Windows.Forms.Label { Text = "散包最低等级:", Left = 10, Top = 182, AutoSize = true });
        EditScatterBagItemsMinLevel = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 178, Width = 80, Maximum = 65535 };
        EditScatterBagItemsMinLevel.ValueChanged += (s, e) => EditScatterBagItemsMinLevelChange(s);
        dieGb1.Controls.Add(EditScatterBagItemsMinLevel);

        var dieGb2 = new System.Windows.Forms.GroupBox { Text = "首饰盒/神佑盒", Left = 366, Top = 4, Width = 350, Height = 160 };
        dieTab.Controls.Add(dieGb2);
        chkKillByMonstDropJewelryBoxItem = MkChk("被怪杀掉首饰盒", 12, 16, dieGb2, chkKillByMonstDropJewelryBoxItemClick);
        chkKillByHumanDropJewelryBoxItem = MkChk("被人杀掉首饰盒", 170, 16, dieGb2, chkKillByHumanDropJewelryBoxItemClick);
        chkKillByMonstDropGodBlessItem = MkChk("被怪杀掉神佑盒", 12, 42, dieGb2, chkKillByMonstDropGodBlessItemClick);
        chkKillByHumanDropGodBlessItem = MkChk("被人杀掉神佑盒", 170, 42, dieGb2, chkKillByHumanDropGodBlessItemClick);
        dieGb2.Controls.Add(new System.Windows.Forms.Label { Text = "首饰盒几率:", Left = 10, Top = 74, AutoSize = true });
        scrlbrJewelryBoxItem = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 70, Width = 80, Minimum = 1, Maximum = 200 };
        scrlbrJewelryBoxItem.ValueChanged += (s, e) => scrlbrJewelryBoxItemChange(s);
        dieGb2.Controls.Add(scrlbrJewelryBoxItem);
        edtJewelryBoxItem = new System.Windows.Forms.TextBox { Left = 230, Top = 70, Width = 60, ReadOnly = true };
        dieGb2.Controls.Add(edtJewelryBoxItem);
        dieGb2.Controls.Add(new System.Windows.Forms.Label { Text = "神佑盒几率:", Left = 10, Top = 104, AutoSize = true });
        scrlbrGodBlessItem = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 100, Width = 80, Minimum = 1, Maximum = 200 };
        scrlbrGodBlessItem.ValueChanged += (s, e) => scrlbrGodBlessItemChange(s);
        dieGb2.Controls.Add(scrlbrGodBlessItem);
        edtGodBlessItem = new System.Windows.Forms.TextBox { Left = 230, Top = 100, Width = 60, ReadOnly = true };
        dieGb2.Controls.Add(edtGodBlessItem);
        dieGb2.Controls.Add(new System.Windows.Forms.Label { Text = "掉装备上限:", Left = 10, Top = 134, AutoSize = true });
        EditDropUseItemsMaxCount = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 130, Width = 80, Maximum = 65535 };
        EditDropUseItemsMaxCount.ValueChanged += (s, e) => EditDropUseItemsMaxCountChange(s);
        dieGb2.Controls.Add(EditDropUseItemsMaxCount);

        var dieGb3 = new System.Windows.Forms.GroupBox { Text = "按槽位掉装备几率（1..1000）", Left = 8, Top = 218, Width = 708, Height = 230 };
        dieTab.Controls.Add(dieGb3);
        DieDropUseItemRateScrolls = new System.Windows.Forms.NumericUpDown[19];
        edtDieDropUseItemRateCells = new System.Windows.Forms.TextBox[19];
        for (int i = 0; i < 19; i++)
        {
            int tag = i;
            int col = 10 + (i % 5) * 138, row = 20 + (i / 5) * 40;
            dieGb3.Controls.Add(new System.Windows.Forms.Label { Text = "槽" + tag, Left = col, Top = row + 4, AutoSize = true });
            var scroll = new System.Windows.Forms.NumericUpDown { Left = col + 40, Top = row, Width = 60, Minimum = 1, Maximum = 1000 };
            scroll.ValueChanged += (s, e) => DieDropUseItemRateScrollChanged(tag);
            dieGb3.Controls.Add(scroll);
            DieDropUseItemRateScrolls[tag] = scroll;
            var edit = new System.Windows.Forms.TextBox { Left = col + 104, Top = row, Width = 30, ReadOnly = true };
            dieGb3.Controls.Add(edit);
            edtDieDropUseItemRateCells[tag] = edit;
        }

        var dieGb4 = new System.Windows.Forms.GroupBox { Text = "总开关", Left = 8, Top = 452, Width = 350, Height = 90 };
        dieTab.Controls.Add(dieGb4);
        CheckBoxDropUseItem = MkChk("启用死亡掉装备", 12, 16, dieGb4, CheckBoxDropUseItemClick);
        dieGb4.Controls.Add(new System.Windows.Forms.Label { Text = "红名单件几率:", Left = 10, Top = 48, AutoSize = true });
        EditDieRedDropUseItemOneRate = new System.Windows.Forms.NumericUpDown { Left = 140, Top = 44, Width = 80, Maximum = 2000000000 };
        EditDieRedDropUseItemOneRate.ValueChanged += (s, e) => EditDieRedDropUseItemOneRateChange(s);
        dieGb4.Controls.Add(EditDieRedDropUseItemOneRate);

        ButtonHumanDieSave = new System.Windows.Forms.Button { Text = "保存死亡(&D)", Left = 366, Top = 456, Width = 110, Height = 26, Enabled = false };
        ButtonHumanDieSave.Click += (s, e) => ButtonHumanDieSaveClick(s);
        dieTab.Controls.Add(ButtonHumanDieSave);
        ButtonDieDropUseItemSave = new System.Windows.Forms.Button { Text = "保存槽位几率(&R)", Left = 366, Top = 488, Width = 130, Height = 26, Enabled = false };
        ButtonDieDropUseItemSave.Click += (s, e) => ButtonDieDropUseItemSaveClick(s);
        dieTab.Controls.Add(ButtonDieDropUseItemSave);

        // ================= 人物状态页（批次J25） =================
        GroupBoxParaly = new System.Windows.Forms.GroupBox { Text = "麻痹状态", Left = 8, Top = 4, Width = 350, Height = 160 };
        statusTab.Controls.Add(GroupBoxParaly);
        CheckBoxParalyCanRun = MkChk("麻痹可跑", 12, 20, GroupBoxParaly, CheckBoxParalyCanRunClick);
        CheckBoxParalyCanWalk = MkChk("麻痹可走", 150, 20, GroupBoxParaly, CheckBoxParalyCanWalkClick);
        CheckBoxParalyCanHit = MkChk("麻痹可攻击", 12, 50, GroupBoxParaly, CheckBoxParalyCanHitClick);
        CheckBoxParalyCanSpell = MkChk("麻痹可魔法", 150, 50, GroupBoxParaly, CheckBoxParalyCanSpellClick);

        statusTab.Controls.Add(new System.Windows.Forms.Label { Text = "攻击模式（至少保留一种）", Left = 8, Top = 172, AutoSize = true });
        CheckGroupAttatckModeItems = new System.Windows.Forms.CheckBox[8];
        string[] modeNames = { "全体", "和平", "夫妻", "师徒", "编组", "行会", "PK", "国家" };
        for (int i = 0; i < 8; i++)
        {
            int idx = i;
            var chk = new System.Windows.Forms.CheckBox { Text = modeNames[i], Left = 12 + (i % 4) * 90, Top = 196 + (i / 4) * 30, AutoSize = true };
            chk.Click += (s, e) => CheckGroupAttatckModeChange(idx, chk.Checked);
            statusTab.Controls.Add(chk);
            CheckGroupAttatckModeItems[i] = chk;
        }

        ButtonCharStatusSave = new System.Windows.Forms.Button { Text = "保存状态(&S)", Left = 8, Top = 262, Width = 110, Height = 26, Enabled = false };
        ButtonCharStatusSave.Click += (s, e) => ButtonCharStatusSaveClick(s);
        statusTab.Controls.Add(ButtonCharStatusSave);

        // 其余页保存按钮（禁用态占位，处理器随巨片拆分接入）
        // （GeneralSave/ExpSave/TimeSave/PriceSave/MsgSave 已随批次J24 接入真实控件）
        ButtonMsgColorSave = new System.Windows.Forms.Button { Left = 0, Top = 0, Width = 0, Height = 0, Enabled = false };
        ButtonHumanDieSave = new System.Windows.Forms.Button { Left = 0, Top = 0, Width = 0, Height = 0, Enabled = false };
        ButtonCharStatusSave = new System.Windows.Forms.Button { Left = 0, Top = 0, Width = 0, Height = 0, Enabled = false };
        ButtonDieDropUseItemSave = new System.Windows.Forms.Button { Left = 0, Top = 0, Width = 0, Height = 0, Enabled = false };
        Controls.Add(ButtonMsgColorSave);
        Controls.Add(ButtonHumanDieSave);
        Controls.Add(ButtonCharStatusSave);
        Controls.Add(ButtonDieDropUseItemSave);
    }

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        ButtonGameSpeedSave.Enabled = true;
        ButtonGeneralSave.Enabled = true;
        ButtonExpSave.Enabled = true;
        ButtonCastleSave.Enabled = true;
        ButtonOptionSave0.Enabled = true;
        ButtonOptionSave.Enabled = true;
        ButtonOptionSave2.Enabled = true;
        ButtonOptionSave3.Enabled = true;
        ButtonTimeSave.Enabled = true;
        ButtonPriceSave.Enabled = true;
        ButtonMsgSave.Enabled = true;
        ButtonMsgColorSave.Enabled = true;
        ButtonHumanDieSave.Enabled = true;
        ButtonCharStatusSave.Enabled = true;
        ButtonCheckActionSave.Enabled = true;
        ButtonDieDropUseItemSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        boSendServerConfig = false;
        ButtonGameSpeedSave.Enabled = false;
        ButtonGeneralSave.Enabled = false;
        ButtonExpSave.Enabled = false;
        ButtonCastleSave.Enabled = false;
        ButtonOptionSave0.Enabled = false;
        ButtonOptionSave.Enabled = false;
        ButtonOptionSave2.Enabled = false;
        ButtonOptionSave3.Enabled = false;
        ButtonTimeSave.Enabled = false;
        ButtonPriceSave.Enabled = false;
        ButtonMsgSave.Enabled = false;
        ButtonMsgColorSave.Enabled = false;
        ButtonHumanDieSave.Enabled = false;
        ButtonCharStatusSave.Enabled = false;
        ButtonCheckActionSave.Enabled = false;
        ButtonDieDropUseItemSave.Enabled = false;
    }

    /// <summary>GameConfigControlChanging 1:1（Delphi var AllowChange → 返回值；测试以 NextAnswer 注入）。</summary>
    public bool GameConfigControlChanging()
    {
        if (boModValued)
        {
            if (M2Forms.MessageBox("参数设置已经被修改，是否确认不保存修改的设置？", "确认信息",
                    M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
            {
                uModValue();
            }
            else
                return false;
        }
        return true;
    }

    /// <summary>Delphi Open()（General 经验/Msg/Time/Price/MsgColor/HumanDie/CharStatus 页加载随巨片拆分接入）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();
        RefGameSpeedConf();
        RefCastleAndOptionAndPkAndTestServer();
        RefGeneralExpMsgTimePrice();
        RefColorDieStatus();
        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    /// <summary>Open 的 MsgColor/HumanDie/CharStatus 页控件回显（Delphi Open 对应片段 1:1）。</summary>
    private void RefColorDieStatus()
    {
        // 消息颜色（Open 尾部原文顺序）
        EditHearMsgFColor.Value = M2Config.btHearMsgFColor;
        EdittHearMsgBColor.Value = M2Config.btHearMsgBColor;
        EditWhisperMsgFColor.Value = M2Config.btWhisperMsgFColor;
        EditWhisperMsgBColor.Value = M2Config.btWhisperMsgBColor;
        EditGMWhisperMsgFColor.Value = M2Config.btGMWhisperMsgFColor;
        EditGMWhisperMsgBColor.Value = M2Config.btGMWhisperMsgBColor;
        seSendWhisperMsgFColor.Value = M2Config.btSendWhisperMsgFColor;
        seSendWhisperMsgBColor.Value = M2Config.btSendWhisperMsgBColor;
        seRefreshGameGoldFColor.Value = M2Config.btRefreshGameGoldFColor;
        seRefreshGameGoldBColor.Value = M2Config.btRefreshGameGoldBColor;
        seShowWhisperFColor.Value = M2Config.btShowWhisperFColor;
        seShowWhisperBColor.Value = M2Config.btShowWhisperBColor;
        seCloseWhisperFColor.Value = M2Config.btCloseWhisperFColor;
        seCloseWhisperBColor.Value = M2Config.btCloseWhisperBColor;
        EditRedMsgFColor.Value = M2Config.btRedMsgFColor;
        EditRedMsgBColor.Value = M2Config.btRedMsgBColor;
        EditGreenMsgFColor.Value = M2Config.btGreenMsgFColor;
        EditGreenMsgBColor.Value = M2Config.btGreenMsgBColor;
        EditBlueMsgFColor.Value = M2Config.btBlueMsgFColor;
        EditBlueMsgBColor.Value = M2Config.btBlueMsgBColor;
        EditCryMsgFColor.Value = M2Config.btCryMsgFColor;
        EditCryMsgBColor.Value = M2Config.btCryMsgBColor;
        EditGuildMsgFColor.Value = M2Config.btGuildMsgFColor;
        EditGuildMsgBColor.Value = M2Config.btGuildMsgBColor;
        EditGroupMsgFColor.Value = M2Config.btGroupMsgFColor;
        EditGroupMsgBColor.Value = M2Config.btGroupMsgBColor;
        EditCustMsgFColor.Value = M2Config.btCustMsgFColor;
        EditCustMsgBColor.Value = M2Config.btCustMsgBColor;
        EditUserSayMsgFColor.Value = M2Config.btUserSayMsgFColor;
        EditUserSayMsgBColor.Value = M2Config.btUserSayMsgBColor;
        EditTopUserSayMsgFColor.Value = M2Config.btTopUserSayMsgFColor;
        EditTopUserSayMsgBColor.Value = M2Config.btTopUserSayMsgBColor;
        EditDropItemFColor.Value = M2Config.btDropItemFColor;
        EditDropItemBColor.Value = M2Config.btDropItemBColor;
        seNPCLabelNormalColor.Value = M2Config.btNPCLabelNormalColor;
        chkNPCLabelFontStroke.Checked = M2Config.boNPCLabelFontStroke;
        seNPCLabelMouseMoveColor.Value = M2Config.btNPCLabelMouseMoveColor;
        seNPCLabelMouseDownColor.Value = M2Config.btNPCLabelMouseDownColor;

        // 死亡掉落（滚动条 Min1/Max200 + Position；槽位组 Min1/Max1000）
        ScrollBarDieDropUseItemRate.Value = Math.Clamp(M2Config.nDieDropUseItemRate, 1, 200);
        ScrollBarDieRedDropUseItemRate.Value = Math.Clamp(M2Config.nDieRedDropUseItemRate, 1, 200);
        ScrollBarDieScatterBagRate.Value = Math.Clamp(M2Config.nDieScatterBagRate, 1, 200);
        CheckBoxKillByMonstDropUseItem.Checked = M2Config.boKillByMonstDropUseItem;
        CheckBoxKillByHumanDropUseItem.Checked = M2Config.boKillByHumanDropUseItem;
        CheckBoxDieScatterBag.Checked = M2Config.boDieScatterBag;
        CheckBoxDieDropGold.Checked = M2Config.boDieDropGold;
        CheckBoxDieRedScatterBagAll.Checked = M2Config.boDieRedScatterBagAll;
        EditScatterBagItemsMinLevel.Value = M2Config.nScatterBagItemsMinLevel;
        EditDropUseItemsMaxCount.Value = M2Config.nDropUseItemsMaxCount;
        CheckBoxDropUseItem.Checked = M2Config.boDropUseItem;
        EditDieRedDropUseItemOneRate.Value = M2Config.nDieRedDropUseItemOneRate;
        chkKillByMonstDropJewelryBoxItem.Checked = M2Config.boKillByMonstDropJewelryBoxItem;
        chkKillByHumanDropJewelryBoxItem.Checked = M2Config.boKillByHumanDropJewelryBoxItem;
        chkKillByMonstDropGodBlessItem.Checked = M2Config.boKillByMonstDropGodBlessItem;
        chkKillByHumanDropGodBlessItem.Checked = M2Config.boKillByHumanDropGodBlessItem;
        scrlbrJewelryBoxItem.Value = Math.Clamp(M2Config.nDropJewelryBoxItemRate, 1, 200);
        scrlbrGodBlessItem.Value = Math.Clamp(M2Config.nDropGodBlessItemRate, 1, 200);
        for (int i = 0; i < 19; i++)
            DieDropUseItemRateScrolls[i].Value = Math.Clamp(M2Config.DieDropUseItemRates[i], 1, 1000);

        // 人物状态（RefCharStatusConf 1:1 + GroupBoxParaly 使能）
        CheckBoxParalyCanRun.Checked = M2Config.boParalyCanRun;
        CheckBoxParalyCanWalk.Checked = M2Config.boParalyCanWalk;
        CheckBoxParalyCanHit.Checked = M2Config.boParalyCanHit;
        CheckBoxParalyCanSpell.Checked = M2Config.boParalyCanSpell;
        for (int i = 0; i < M2Config.AttatckModes.Length; i++)
            CheckGroupAttatckModeItems[i].Checked = M2Config.AttatckModes[i];
        GroupBoxParaly.Enabled = M2Config.boStartGameAuxiliary && M2Config.ClientConfig38ShowMonName;
    }

    /// <summary>Open 的 General/经验/Msg/Time/Price 页控件回显（Delphi Open 对应片段 1:1）。</summary>
    private void RefGeneralExpMsgTimePrice()
    {
        // 经验/杀怪
        EditKillMonExpMultiple.Value = M2Config.dwKillMonExpMultiple;
        CheckBoxHighLevelKillMonFixExp.Checked = M2Config.boHighLevelKillMonFixExp;
        CheckBoxHighLevelGroupFixExp.Checked = M2Config.boHighLevelGroupFixExp;
        EditMaxUpLevelCount.Value = M2Config.nMaxUpLevelCount;
        for (int i = 1; i <= 1000; i++)
            GridLevelExp.Rows[i - 1].Cells[1].Value = M2Config.dwNeedExps[i].ToString();
        for (int i = 1; i <= 1000; i++)
        {
            GridLevelExpRate.Rows[i - 1].Cells[0].Value = i;
            GridLevelExpRate.Rows[i - 1].Cells[1].Value = M2Config.LevelExpRates[i].ToString();
        }

        // General（RefGameVarConf 1:1）
        EditSoftVersionDate.Text = M2Config.nSoftVersionDate.ToString();
        EditConsoleShowUserCountTime.Value = M2Config.dwConsoleShowUserCountTime / 1000;
        EditShowLineNoticeTime.Value = M2Config.dwShowLineNoticeTime / 1000;
        ComboBoxLineNoticeColor.SelectedIndex = Math.Max(0, Math.Min(2, M2Config.nLineNoticeColor));
        EditLineNoticePreFix.Text = M2Config.sLineNoticePreFix;
        CheckBoxShowMakeItemMsg.Checked = M2Config.boShowMakeItemMsg;
        CbViewHack.Checked = M2Config.boViewHackMessage;
        CkViewAdmfail.Checked = M2Config.boViewAdmissionFailure;
        CheckBoxShowExceptionMsg.Checked = M2Config.boShowExceptionMsg;
        CheckBoxSendOnlineCount.Checked = M2Config.boSendOnlineCount;
        EditSendOnlineCountRate.Value = M2Config.nSendOnlineCountRate;
        EditSendOnlineTime.Value = M2Config.dwSendOnlineTime / 1000;
        CheckBoxSendOnlineCountClick(CheckBoxSendOnlineCount);
        EditMonsterPowerRate.Value = M2Config.nMonsterPowerRate;
        EditEditItemsPowerRate.Value = M2Config.nItemsPowerRate;
        EditItemsACPowerRate.Value = M2Config.nItemsACPowerRate;
        CheckBoxCanOldClientLogon.Checked = M2Config.boCanOldClientLogon;
        chkOldClient.Checked = M2Config.boIsOldClient;
        chkRecordPublicMsg.Checked = M2Config.boRecordPublicMsg;
        chkRecordPrivateMsg.Checked = M2Config.boRecordPrivateMsg;
        chkRecordGuildMsg.Checked = M2Config.boRecordGuildMsg;
        chkRecordCryCryMsg.Checked = M2Config.boRecordCryCryMsg;
        chkRecordGroupMsg.Checked = M2Config.boRecordGroupMsg;
        chkRecordNationMsg.Checked = M2Config.boRecordNationMsg;
        chkPermissionChangeLog.Checked = M2Config.boPermissionChangeLog;

        // 固定经验/上限
        CheckBoxFixExp.Checked = M2Config.boUseFixExp;
        SpinEditBaseExp.Value = M2Config.nBaseExp;
        SpinEditAddExp.Value = M2Config.nAddExp;
        SpinEditBaseExp.Enabled = !CheckBoxFixExp.Checked;
        SpinEditAddExp.Enabled = !CheckBoxFixExp.Checked;
        EditHighLevel.Value = M2Config.nHighLevel;
        EditHighLevelGetExp.Value = M2Config.nHighLevelGetExp;
        CheckBoxLimitChangeExp.Checked = M2Config.boLimitChangeExp;
        RadioGroupMaxLevel.Value = M2Config.btMaxLevel;
        rgMaxAC.Value = M2Config.btMaxAC;
        rgMaxHitPoint.Value = M2Config.btMaxHitPoint;

        // Msg
        EditSayMsgMaxLen.Value = M2Config.nSayMsgMaxLen;
        EditSayRedMsgMaxLen.Value = M2Config.nSayRedMsgMaxLen;
        EditCanShoutMsgLevel.Value = M2Config.nCanShoutMsgLevel;
        CheckBoxShutRedMsgShowGMName.Checked = M2Config.boShutRedMsgShowGMName;
        CheckBoxShowPreFixMsg.Checked = M2Config.boShowPreFixMsg;
        EditGMRedMsgCmd.Text = M2ShareState.g_GMRedMsgCmd.ToString();
        CheckBoxShowWhisperLevelMsg.Checked = M2Config.boShowWhisperLevelMsg;
        EditShowWhisperLevelMsg.Text = M2ShareState.g_sShowWhisperLevelMsg;
        EditSayMsgTime.Value = M2Config.dwSayMsgTime / 1000;
        EditSayMsgCount.Value = M2Config.nSayMsgCount;
        EditDisableSayMsgTime.Value = M2Config.dwDisableSayMsgTime / 1000;
        EditUserItemSayMsgTime.Value = M2Config.nUserItemSayMsgTime;
        seMaxInputStringLen.Value = M2Config.nMaxInputStringLen;

        // Time
        EditStartCastleWarDays.Value = M2Config.nStartCastleWarDays;
        EditStartCastlewarTime.Value = M2Config.nStartCastlewarTime;
        EditShowCastleWarEndMsgTime.Value = M2Config.dwShowCastleWarEndMsgTime / (60 * 1000);
        EditCastleWarTime.Value = M2Config.dwCastleWarTime / (60 * 1000);
        EditGetCastleTime.Value = M2Config.dwGetCastleTime / (60 * 1000);
        EditGuildWarTime.Value = M2Config.dwGuildWarTime / (60 * 1000);
        seMakeGhostTime.Value = M2Config.dwMakeGhostTime / 1000;
        seMakeMonGhostTime.Value = M2Config.dwMakeMonGhostTime / 1000;
        seMakeDummyGhostTime.Value = M2Config.dwMakeDummyGhostTime / 1000;
        seClearDropOnFloorItemTime.Value = M2Config.dwClearDropOnFloorItemTime / 1000;
        seHorseTakeTime.Value = M2Config.dwHorseTakeTime;
        seTakeOnHorseUseTime.Value = M2Config.dwTakeOnHorseUseTime;
        chkReadyOnHorseDisableAction.Checked = M2Config.boReadyOnHorseDisableAction;
        seNpcButtonClickTime.Value = M2Config.dwNpcButtonClickTime;
        seNpcActorClickTime.Value = M2Config.dwNpcActorClickTime;
        sePlayerVarJClearTime.Value = M2Config.btPlayerVarJClearTime;
        seSaveHumanRcdTime.Value = M2Config.dwSaveHumanRcdTime / (60 * 1000);
        seHumanFreeDelayTime.Value = M2Config.dwHumanFreeDelayTime / (60 * 1000);
        seGetDBSockMsgTime.Value = M2Config.dwGetDBSockMsgTime / 1000;
        seFloorItemCanPickUpTime.Value = M2Config.dwFloorItemCanPickUpTime / 1000;
        seDearRecallTime.Value = M2Config.dwDearRecallTime;
        seMasterRecallTime.Value = M2Config.dwMasterRecallTime;
        seGroupRecallTime.Value = M2Config.dwGroupRecallTime;

        // Price
        EditBuildGuildPrice.Value = M2Config.nBuildGuildPrice;
        EditGuildWarPrice.Value = M2Config.nGuildWarPrice;
        EditMakeDurgPrice.Value = M2Config.nMakeDurgPrice;
        EditSuperRepairPriceRate.Value = M2Config.nSuperRepairPriceRate;
        EditRepairItemDecDura.Value = M2Config.nRepairItemDecDura;
    }

    /// <summary>Open 的城堡/Option/PK/测试服页控件回显（Delphi Open 对应片段 1:1）。</summary>
    private void RefCastleAndOptionAndPkAndTestServer()
    {
        // 城堡
        EditRepairDoorPrice.Value = M2Config.nRepairDoorPrice;
        EditRepairWallPrice.Value = M2Config.nRepairWallPrice;
        EditHireArcherPrice.Value = M2Config.nHireArcherPrice;
        EditHireGuardPrice.Value = M2Config.nHireGuardPrice;
        EditCastleGoldMax.Value = M2Config.nCastleGoldMax;
        EditCastleOneDayGold.Value = M2Config.nCastleOneDayGold;
        EditCastleHomeMap.Text = M2Config.sCastleHomeMap;
        EditCastleHomeX.Value = M2Config.nCastleHomeX;
        EditCastleHomeY.Value = M2Config.nCastleHomeY;
        EditCastleName.Text = M2Config.sCASTLENAME;
        EditWarRangeX.Value = M2Config.nCastleWarRangeX;
        EditWarRangeY.Value = M2Config.nCastleWarRangeY;
        CheckBoxGetAllNpcTax.Checked = M2Config.boGetAllNpcTax;
        EditTaxRate.Value = M2Config.nCastleTaxRate;

        // 跑动组
        chkDisHumRun.Checked = !M2Config.boDiableHumanRun;
        chkRunHum.Checked = M2Config.boRUNHUMAN;
        chkRunMon.Checked = M2Config.boRUNMON;
        chkRunNpc.Checked = M2Config.boRunNpc;
        chkRunGuard.Checked = M2Config.boRunGuard;
        chkWarDisHumRun.Checked = M2Config.boWarDisHumRun;
        chkWarHreoRun.Checked = M2Config.boWarHreoRun;
        chkWarHreoRun.Enabled = M2Config.boWarDisHumRun;
        chkGMRunAll.Checked = M2Config.boGMRunAll;
        chkSafeArea.Checked = M2Config.boSafeAreaLimited;
        chkDisHumRunClick(chkDisHumRun);
        chkSafeAreaDisNpcRun.Checked = M2Config.boSafeAreaDisNpcRun;
        chkSafeAreaDisShopStallHumRun.Checked = M2Config.boSafeAreaDisShopStallHumRun;
        chkSafeAreaDisOffLineHumRun.Checked = M2Config.boSafeAreaDisOffLineHumRun;
        chkWarDisTeleport.Checked = M2Config.boWarDisTeleport;

        // 安全区
        EditSafeZoneSize.Value = M2Config.nSafeZoneSize;
        chkHintSafeZone.Checked = M2Config.boHintSafeZone;
        seHintSafeZoneY.Value = M2Config.dwHintSafeZoneY;
        seHintSafeZoneFColor.Value = M2Config.btHintSafeZoneFColor;
        seHintSafeZoneBColor.Value = M2Config.btHintSafeZoneBColor;
        seHintSafeZoneFSize.Value = M2Config.btHintSafeZoneFSize;
        chkHintSafeZoneClick(chkHintSafeZone);

        EditStartPointSize.Value = M2Config.nStartPointSize;
        seGroupMembersMax.Value = M2Config.nGroupMembersMax;

        // 红名/回城
        EditRedHomeMap.Text = M2Config.sRedHomeMap;
        EditRedHomeX.Value = M2Config.nRedHomeX;
        EditRedHomeY.Value = M2Config.nRedHomeY;
        EditRedDieHomeMap.Text = M2Config.sRedDieHomeMap;
        EditRedDieHomeX.Value = M2Config.nRedDieHomeX;
        EditRedDieHomeY.Value = M2Config.nRedDieHomeY;
        EditHomeMap.Text = M2Config.sHomeMap;
        EditHomeX.Value = M2Config.nHomeX;
        EditHomeY.Value = M2Config.nHomeY;

        // PK
        EditDecPkPointTime.Value = M2Config.dwDecPkPointTime / 1000;
        EditDecPkPointCount.Value = M2Config.nDecPkPointCount;
        EditPKFlagTime.Value = M2Config.dwPKFlagTime / 1000;
        seHumanAddPKPoint.Value = M2Config.nKillHumanAddPKPoint;
        seKillHumanWeaponUnlockRate.Value = M2Config.dwKillHumanWeaponUnlockRate;
        chkHeroKillHumanNotWeaponUnlock.Checked = M2Config.boHeroKillHumanNotWeaponUnlock;
        seDummyAddPKPoint.Value = M2Config.nDummyAddPKPoint;
        seKillHeroAddPKPoint.Value = M2Config.dwKillHeroAddPKPoint;
        CheckBoxKillHumanWinLevel.Checked = M2Config.boKillHumanWinLevel;
        CheckBoxKilledLostLevel.Checked = M2Config.boKilledLostLevel;
        CheckBoxKillHumanWinExp.Checked = M2Config.boKillHumanWinExp;
        CheckBoxKilledLostExp.Checked = M2Config.boKilledLostExp;
        EditKillHumanWinLevel.Value = M2Config.nKillHumanWinLevel;
        EditKilledLostLevel.Value = M2Config.nKilledLostLevel;
        EditKillHumanWinExp.Value = M2Config.nKillHumanWinExp;
        EditKillHumanLostExp.Value = M2Config.nKillHumanLostExp;
        EditHumanLevelDiffer.Value = M2Config.nHumanLevelDiffer;
        CheckBoxKillHumanWinLevelClick(CheckBoxKillHumanWinLevel);
        CheckBoxKilledLostLevelClick(CheckBoxKilledLostLevel);
        CheckBoxKillHumanWinExpClick(CheckBoxKillHumanWinExp);
        CheckBoxKilledLostExpClick(CheckBoxKilledLostExp);

        // 交易/掉落/挑战
        seTryDealTime.Value = M2Config.dwTryDealTime / 1000;
        seDealOKTime.Value = M2Config.dwDealOKTime / 1000;
        seTryChallengeTime.Value = M2Config.dwTryChallengeTime / 1000;
        seChallengeOKTime.Value = M2Config.dwChallengeOKTime / 1000;
        seChallengeTime.Value = M2Config.dwChallengeTime / 1000 / 60;
        rgChallengeGold.Value = M2Config.btChallengeGoldIndex;
        chkCanNotGetBackChallenge.Checked = M2Config.boCanNotGetBackChallenge;
        chkDisableChallenge.Checked = M2Config.boDisableChallenge;
        EditCastleMemberPriceRate.Value = M2Config.nCastleMemberPriceRate;
        chkCanNotGetBackDeal.Checked = M2Config.boCanNotGetBackDeal;
        chkDisableDeal.Checked = M2Config.boDisableDeal;
        chkControlDropItem.Checked = M2Config.boControlDropItem;
        chkIsSafeDisableDrop.Checked = M2Config.boInSafeDisableDrop;
        seCanDropPrice.Value = M2Config.nCanDropPrice;
        seCanDropGold.Value = M2Config.nCanDropGold;

        // 测试服/试玩
        CheckBoxTestServer.Checked = M2Config.boTestServer;
        CheckBoxServiceMode.Checked = M2Config.boServiceMode;
        CheckBoxVentureMode.Checked = M2Config.boVentureServer;
        CheckBoxNonPKMode.Checked = M2Config.boNonPKServer;
        chkOffLineShop.Checked = M2Config.boOffLineShop;
        chkOffLineHero.Checked = M2Config.boOffLineHero;
        chkOffLineSlave.Checked = M2Config.boOffLineSlave;
        seStartPermission.Value = M2Config.nStartPermission;
        seTestLevel.Value = M2Config.nTestLevel;
        seTestGold.Value = M2Config.nTestGold;
        seTestUserLimit.Value = M2Config.nTestUserLimit;
        seUserFull.Value = M2Config.nUserFull;
        seGuildMemberMaxLimit.Value = M2Config.nGuildMemberMaxLimit;
        seGuildNameLen.Value = M2Config.nGuildNameLen;
        seGuildRankNameLen.Value = M2Config.nGuildRankNameLen;
        seHumChgMapOrLoginProtectTime.Value = M2Config.dwHumChgMapOrLoginProtectTime;
        chkSellItemToNpcShopNoCalcAddProperty.Checked = M2Config.boSellItemToNpcShopNoCalcAddProperty;
        chkShowNewValueFromBuyNpcItem.Checked = M2Config.boShowNewValueFromBuyNpcItem;
        CheckBoxTestServerClick(CheckBoxTestServer);
        seHumanMaxGold.Value = M2Config.nHumanMaxGold;
        seHumanTryModeMaxGold.Value = M2Config.nHumanTryModeMaxGold;
        seTryModeLevel.Value = M2Config.nTryModeLevel;
        CheckBoxTryModeUseStorage.Checked = M2Config.boTryModeUseStorage;

        // 国家
        chkNationGroupCheck.Checked = M2Config.boNationGroupCheck;
        chkNationGuildCheck.Checked = M2Config.boNationGuildCheck;
        seNationSayLevel.Value = M2Config.nNationSayLevel;
    }

    public void RefGameSpeedConf()
    {
        chkSpeedControl.Checked = M2Config.boSpeedControl;

        RefGlobalSpeedCtrl();

        EditHitIntervalTime.Value = M2Config.dwHitIntervalTime;
        EditMagicHitIntervalTime.Value = M2Config.dwMagicHitIntervalTime;
        EditRunIntervalTime.Value = M2Config.dwRunIntervalTime;
        EditWalkIntervalTime.Value = M2Config.dwWalkIntervalTime;
        EditTurnIntervalTime.Value = M2Config.dwTurnIntervalTime;
        EditDigUpIntervalTime.Value = M2Config.dwDigUpIntervalTime;

        EditMaxHitMsgCount.Value = M2Config.nMaxHitMsgCount;
        EditMaxSpellMsgCount.Value = M2Config.nMaxSpellMsgCount;
        EditMaxRunMsgCount.Value = M2Config.nMaxRunMsgCount;
        EditMaxWalkMsgCount.Value = M2Config.nMaxWalkMsgCount;
        EditMaxTurnMsgCount.Value = M2Config.nMaxTurnMsgCount;
        EditMaxDigUpMsgCount.Value = M2Config.nMaxDigUpMsgCount;
        CheckBoxboKickOverSpeed.Checked = M2Config.boKickOverSpeed;
        EditOverSpeedKickCount.Value = M2Config.nOverSpeedKickCount;
        EditDropOverSpeed.Value = M2Config.dwDropOverSpeed;
        CheckBoxboKickOverSpeedClick(CheckBoxboKickOverSpeed);

        CheckBoxSpellSendUpdateMsg.Checked = M2Config.boSpellSendUpdateMsg;
        CheckBoxActionSendActionMsg.Checked = M2Config.boActionSendActionMsg;

        if (M2Config.btSpeedControlMode == 0)
        {
            RadioButtonDelyMode.Checked = true;
            RadioButtonFilterMode.Checked = false;
        }
        else
        {
            RadioButtonDelyMode.Checked = false;
            RadioButtonFilterMode.Checked = true;
        }

        CheckBoxDisableStruck.Checked = M2Config.boDisableStruck;
        CheckBoxDisableStruckClick(CheckBoxDisableStruck); // Delphi OnClick 联动等效（弯腰时间使能）
        CheckBoxDisableSelfStruck.Checked = M2Config.boDisableSelfStruck;
        chkMagicshieldStruck.Checked = M2Config.boMagicshieldStruck;

        EditStruckTime.Value = M2Config.dwStruckTime;

        CheckBoxCheckActionCount.Checked = M2Config.boCheckActionCount;
        EditHitCountIntervalTime.Value = M2Config.dwHitCountIntervalTime;
        EditMagicHitCountIntervalTime.Value = M2Config.dwMagicHitCountIntervalTime;
        EditMoveCountIntervalTime.Value = M2Config.dwMoveCountIntervalTime;
        EditCanHitCount.Value = M2Config.nCanHitCount;
        EditCanMagicHitCount.Value = M2Config.nCanMagicHitCount;
        EditCanMoveCount.Value = M2Config.nCanMoveCount;

        EditCheckHitCount.Value = M2Config.nCheckHitCount;
        EditCheckMagicHitCount.Value = M2Config.nCheckMagicHitCount;
        EditCheckMoveCount.Value = M2Config.nCheckMoveCount;

        CheckBoxSendUpdateMsg.Checked = M2Config.boSendUpdateMsg;

        EditMaxHitDeliveryTime.Value = M2Config.nMaxHitDeliveryTime;
        EditMaxMagicHitDeliveryTime.Value = M2Config.nMaxMagicHitDeliveryTime;
        EditMaxRunDeliveryTime.Value = M2Config.nMaxRunDeliveryTime;
        EditMaxWalkDeliveryTime.Value = M2Config.nMaxWalkDeliveryTime;
        EditMaxTurnDeliveryTime.Value = M2Config.nMaxTurnDeliveryTime;
        EditMaxDigUpDeliveryTime.Value = M2Config.nMaxDigUpDeliveryTime;

        chkHorseRun3Grid.Checked = M2Config.boHorseRun3Grid;
    }

    /// <summary>RefGlobalSpeedCtrl 1:1（原文过程体首行 Exit，控件使能逻辑为死代码保留语义）。</summary>
    public void RefGlobalSpeedCtrl()
    {
        return;
    }

    public void ButtonActionSpeedConfigClick(object? sender)
    {
        // Delphi: TfrmActionSpeed.Create(Owner) + Open + Free；经接缝复用已有窗体
        ActionSpeedConfigHandler?.Invoke();
    }

    public void ButtonGameSpeedDefaultClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2Config.dwHitIntervalTime = 700;
        M2Config.dwMagicHitIntervalTime = 450;
        M2Config.dwRunIntervalTime = 400;
        M2Config.dwWalkIntervalTime = 400;
        M2Config.dwTurnIntervalTime = 100;
        M2Config.dwDigUpIntervalTime = 100;
        M2Config.nMaxHitMsgCount = 1;
        M2Config.nMaxSpellMsgCount = 1;
        M2Config.nMaxRunMsgCount = 1;
        M2Config.nMaxWalkMsgCount = 1;
        M2Config.nMaxTurnMsgCount = 1;
        M2Config.nMaxDigUpMsgCount = 1;
        M2Config.nOverSpeedKickCount = 2;
        M2Config.dwDropOverSpeed = 200;
        M2Config.boKickOverSpeed = true;
        M2Config.boDisableStruck = false;
        M2Config.boDisableSelfStruck = true;
        M2Config.boMagicshieldStruck = false;
        M2Config.dwStruckTime = 300;
        M2Config.boSpellSendUpdateMsg = true;
        M2Config.boActionSendActionMsg = true;
        M2Config.btSpeedControlMode = 0;

        M2Config.nMaxHitDeliveryTime = 100;
        M2Config.nMaxMagicHitDeliveryTime = 100;
        M2Config.nMaxRunDeliveryTime = 100;
        M2Config.nMaxWalkDeliveryTime = 100;
        M2Config.nMaxTurnDeliveryTime = 100;
        M2Config.nMaxDigUpDeliveryTime = 100;

        M2Config.boSendUpdateMsg = false;
        M2Config.boHorseRun3Grid = false;

        RefGameSpeedConf();
        ModValue();
    }

    public void ButtonGameSpeedSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "SpeedControl", M2Config.boSpeedControl);

        Config.WriteInteger("Setup", "HitIntervalTime", (int)M2Config.dwHitIntervalTime);
        Config.WriteInteger("Setup", "MagicHitIntervalTime", (int)M2Config.dwMagicHitIntervalTime);
        Config.WriteInteger("Setup", "RunIntervalTime", (int)M2Config.dwRunIntervalTime);
        Config.WriteInteger("Setup", "WalkIntervalTime", (int)M2Config.dwWalkIntervalTime);
        Config.WriteInteger("Setup", "TurnIntervalTime", (int)M2Config.dwTurnIntervalTime);
        Config.WriteInteger("Setup", "DigUpIntervalTime", (int)M2Config.dwDigUpIntervalTime);

        Config.WriteInteger("Setup", "MaxHitMsgCount", M2Config.nMaxHitMsgCount);
        Config.WriteInteger("Setup", "MaxSpellMsgCount", M2Config.nMaxSpellMsgCount);
        Config.WriteInteger("Setup", "MaxRunMsgCount", M2Config.nMaxRunMsgCount);
        Config.WriteInteger("Setup", "MaxWalkMsgCount", M2Config.nMaxWalkMsgCount);
        Config.WriteInteger("Setup", "MaxTurnMsgCount", M2Config.nMaxTurnMsgCount);
        Config.WriteInteger("Setup", "MaxSitDonwMsgCount", M2Config.nMaxSitDonwMsgCount);
        Config.WriteInteger("Setup", "MaxDigUpMsgCount", M2Config.nMaxDigUpMsgCount);
        Config.WriteInteger("Setup", "OverSpeedKickCount", M2Config.nOverSpeedKickCount);
        Config.WriteBool("Setup", "KickOverSpeed", M2Config.boKickOverSpeed);
        Config.WriteBool("Setup", "SpellSendUpdateMsg", M2Config.boSpellSendUpdateMsg);
        Config.WriteBool("Setup", "ActionSendActionMsg", M2Config.boActionSendActionMsg);
        Config.WriteInteger("Setup", "DropOverSpeed", (int)M2Config.dwDropOverSpeed);
        Config.WriteBool("Setup", "DisableStruck", M2Config.boDisableStruck);
        Config.WriteBool("Setup", "DisableSelfStruck", M2Config.boDisableSelfStruck);
        Config.WriteBool("Setup", "MagicshieldStruck", M2Config.boMagicshieldStruck);
        Config.WriteInteger("Setup", "StruckTime", (int)M2Config.dwStruckTime);
        Config.WriteInteger("Setup", "SpeedControlMode", M2Config.btSpeedControlMode);

        Config.WriteBool("Setup", "SendUpdateMsg", M2Config.boSendUpdateMsg);
        Config.WriteInteger("Setup", "MaxHitDeliveryTime", M2Config.nMaxHitDeliveryTime);
        Config.WriteInteger("Setup", "MaxMagicHitDeliveryTime", M2Config.nMaxMagicHitDeliveryTime);
        Config.WriteInteger("Setup", "MaxRunDeliveryTime", M2Config.nMaxRunDeliveryTime);
        Config.WriteInteger("Setup", "MaxWalkDeliveryTime", M2Config.nMaxWalkDeliveryTime);
        Config.WriteInteger("Setup", "MaxWalkDeliveryTime", M2Config.nMaxWalkDeliveryTime); // Delphi 原文重复写一行，保留
        Config.WriteInteger("Setup", "MaxTurnDeliveryTime", M2Config.nMaxTurnDeliveryTime);
        Config.WriteInteger("Setup", "MaxDigUpDeliveryTime", M2Config.nMaxDigUpDeliveryTime);
        Config.WriteBool("Setup", "HorseRun3Grid", M2Config.boHorseRun3Grid);

        if (boSendServerConfig)
            GameConfigState.SendServerConfig();

        uModValue();
    }

    public void EditHitIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHitIntervalTime = (uint)EditHitIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditMagicHitIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMagicHitIntervalTime = (uint)EditMagicHitIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditRunIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwRunIntervalTime = (uint)EditRunIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditWalkIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwWalkIntervalTime = (uint)EditWalkIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditTurnIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwTurnIntervalTime = (uint)EditTurnIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditMaxHitMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxHitMsgCount = (int)EditMaxHitMsgCount.Value;
        ModValue();
    }

    public void EditMaxSpellMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxSpellMsgCount = (int)EditMaxSpellMsgCount.Value;
        ModValue();
    }

    public void EditMaxRunMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxRunMsgCount = (int)EditMaxRunMsgCount.Value;
        ModValue();
    }

    public void EditMaxWalkMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxWalkMsgCount = (int)EditMaxWalkMsgCount.Value;
        ModValue();
    }

    public void EditMaxTurnMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxTurnMsgCount = (int)EditMaxTurnMsgCount.Value;
        ModValue();
    }

    public void EditMaxDigUpMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxDigUpMsgCount = (int)EditMaxDigUpMsgCount.Value;
        ModValue();
    }

    public void EditOverSpeedKickCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nOverSpeedKickCount = (int)EditOverSpeedKickCount.Value;
        ModValue();
    }

    public void EditDropOverSpeedChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDropOverSpeed = (uint)EditDropOverSpeed.Value;
        ModValue();
    }

    public void CheckBoxSpellSendUpdateMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSpellSendUpdateMsg = CheckBoxSpellSendUpdateMsg.Checked;
        ModValue();
    }

    public void CheckBoxActionSendActionMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boActionSendActionMsg = CheckBoxActionSendActionMsg.Checked;
        ModValue();
    }

    public void CheckBoxboKickOverSpeedClick(object? sender)
    {
        EditOverSpeedKickCount.Enabled = CheckBoxboKickOverSpeed.Checked && M2Config.boSpeedControl;
        if (!boOpened)
            return;
        M2Config.boKickOverSpeed = CheckBoxboKickOverSpeed.Checked;
        ModValue();
    }

    public void RadioButtonDelyModeClick(object? sender)
    {
        if (!boOpened)
            return;
        bool boFalg = RadioButtonDelyMode.Checked;
        if (boFalg)
        {
            M2Config.btSpeedControlMode = 0;
        }
        else
        {
            M2Config.btSpeedControlMode = 1;
        }
        ModValue();
    }

    public void RadioButtonFilterModeClick(object? sender)
    {
        if (!boOpened)
            return;
        bool boFalg = RadioButtonFilterMode.Checked;
        if (boFalg)
        {
            M2Config.btSpeedControlMode = 1;
        }
        else
        {
            M2Config.btSpeedControlMode = 0;
        }
        ModValue();
    }

    public void CheckBoxDisableStruckClick(object? sender)
    {
        EditStruckTime.Enabled = !CheckBoxDisableStruck.Checked;
        if (!boOpened)
            return;
        M2Config.boDisableStruck = CheckBoxDisableStruck.Checked;
        boSendServerConfig = true;
        ModValue();
    }

    public void CheckBoxDisableSelfStruckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableSelfStruck = CheckBoxDisableSelfStruck.Checked;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditStruckTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwStruckTime = (uint)EditStruckTime.Value;
        ModValue();
    }

    public void chkSpeedControlClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSpeedControl = chkSpeedControl.Checked;
        boSendServerConfig = true;
        RefGlobalSpeedCtrl();
        ModValue();
    }

    public void CheckBoxCheckActionCountClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCheckActionCount = CheckBoxCheckActionCount.Checked;
        ModValue();
    }

    public void EditHitCountIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHitCountIntervalTime = (uint)EditHitCountIntervalTime.Value;
        ModValue();
    }

    public void EditMagicHitCountIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMagicHitCountIntervalTime = (uint)EditMagicHitCountIntervalTime.Value;
        ModValue();
    }

    public void EditMoveCountIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMoveCountIntervalTime = (uint)EditMoveCountIntervalTime.Value;
        ModValue();
    }

    public void EditCanHitCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanHitCount = (uint)EditCanHitCount.Value;
        ModValue();
    }

    public void EditCanMagicHitCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanMagicHitCount = (uint)EditCanMagicHitCount.Value;
        ModValue();
    }

    public void EditCanMoveCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanMoveCount = (uint)EditCanMoveCount.Value;
        ModValue();
    }

    public void ButtonCheckActionDefaultClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2Config.dwHitCountIntervalTime = 800;
        M2Config.dwMagicHitCountIntervalTime = 1500;
        M2Config.dwMoveCountIntervalTime = 800;

        M2Config.nCanHitCount = 3;
        M2Config.nCanMagicHitCount = 3;
        M2Config.nCanMoveCount = 3;

        M2Config.nCheckHitCount = 4; // 攻击次数
        M2Config.nCheckMagicHitCount = 3; // 魔法次数
        M2Config.nCheckMoveCount = 4; // 移动次数

        M2Config.boCheckActionCount = true;

        RefGameSpeedConf();
        ModValue();
    }

    public void ButtonCheckActionSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "CheckActionCount", M2Config.boCheckActionCount);
        Config.WriteInteger("Setup", "HitCountIntervalTime", (int)M2Config.dwHitCountIntervalTime);
        Config.WriteInteger("Setup", "MagicHitCountIntervalTime", (int)M2Config.dwMagicHitCountIntervalTime);
        Config.WriteInteger("Setup", "MoveCountIntervalTime", (int)M2Config.dwMoveCountIntervalTime);
        Config.WriteInteger("Setup", "CanHitCount", (int)M2Config.nCanHitCount);
        Config.WriteInteger("Setup", "CanMagicHitCount", (int)M2Config.nCanMagicHitCount);
        Config.WriteInteger("Setup", "CanMoveCount", (int)M2Config.nCanMoveCount);

        Config.WriteInteger("Setup", "CheckHitCount", (int)M2Config.nCheckHitCount);
        Config.WriteInteger("Setup", "CheckMagicHitCount", (int)M2Config.nCheckMagicHitCount);
        Config.WriteInteger("Setup", "CheckMoveCount", (int)M2Config.nCheckMoveCount);

        uModValue();
    }

    public void EditCheckHitCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCheckHitCount = (uint)EditCheckHitCount.Value;
        ModValue();
    }

    public void EditCheckMagicHitCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCheckMagicHitCount = (uint)EditCheckMagicHitCount.Value;
        ModValue();
    }

    public void EditCheckMoveCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCheckMoveCount = (uint)EditCheckMoveCount.Value;
        ModValue();
    }

    public void CheckBoxSendUpdateMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSendUpdateMsg = CheckBoxSendUpdateMsg.Checked;
        ModValue();
    }

    public void EditMaxHitDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxHitDeliveryTime = (int)EditMaxHitDeliveryTime.Value;
        ModValue();
    }

    public void EditMaxMagicHitDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxMagicHitDeliveryTime = (int)EditMaxMagicHitDeliveryTime.Value;
        ModValue();
    }

    public void EditMaxRunDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxRunDeliveryTime = (int)EditMaxRunDeliveryTime.Value;
        ModValue();
    }

    public void EditMaxWalkDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxWalkDeliveryTime = (int)EditMaxWalkDeliveryTime.Value;
        ModValue();
    }

    public void EditMaxTurnDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxTurnDeliveryTime = (int)EditMaxTurnDeliveryTime.Value;
        ModValue();
    }

    public void EditMaxDigUpDeliveryTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxDigUpDeliveryTime = (int)EditMaxDigUpDeliveryTime.Value;
        ModValue();
    }

    public void EditDigUpIntervalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDigUpIntervalTime = (uint)EditDigUpIntervalTime.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void chkHorseRun3GridClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHorseRun3Grid = chkHorseRun3Grid.Checked;
        boSendServerConfig = true;
        ModValue();
    }

    public void chkMagicshieldStruckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boMagicshieldStruck = chkMagicshieldStruck.Checked;
        ModValue();
    }

    // ================= 城堡页（批次J23） =================

    public void EditRepairDoorPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRepairDoorPrice = (int)EditRepairDoorPrice.Value;
        ModValue();
    }

    public void EditRepairWallPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRepairWallPrice = (int)EditRepairWallPrice.Value;
        ModValue();
    }

    public void EditHireArcherPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHireArcherPrice = (int)EditHireArcherPrice.Value;
        ModValue();
    }

    public void EditHireGuardPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHireGuardPrice = (int)EditHireGuardPrice.Value;
        ModValue();
    }

    public void EditCastleGoldMaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleGoldMax = (int)EditCastleGoldMax.Value;
        ModValue();
    }

    public void EditCastleOneDayGoldChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleOneDayGold = (int)EditCastleOneDayGold.Value;
        ModValue();
    }

    public void EditCastleHomeMapChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sCastleHomeMap = EditCastleHomeMap.Text.Trim();
        ModValue();
    }

    public void EditCastleHomeXChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleHomeX = (int)EditCastleHomeX.Value;
        ModValue();
    }

    public void EditCastleHomeYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleHomeY = (int)EditCastleHomeY.Value;
        ModValue();
    }

    public void EditCastleNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sCASTLENAME = EditCastleName.Text.Trim();
        ModValue();
    }

    public void EditWarRangeXChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleWarRangeX = (int)EditWarRangeX.Value;
        ModValue();
    }

    public void EditWarRangeYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleWarRangeY = (int)EditWarRangeY.Value;
        ModValue();
    }

    public void CheckBoxGetAllNpcTaxClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boGetAllNpcTax = CheckBoxGetAllNpcTax.Checked;
        ModValue();
    }

    public void EditTaxRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleTaxRate = (int)EditTaxRate.Value;
        ModValue();
    }

    public void EditCastleMemberPriceRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCastleMemberPriceRate = (int)EditCastleMemberPriceRate.Value;
        ModValue();
    }

    public void ButtonCastleSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "RepairDoor", M2Config.nRepairDoorPrice);
        Config.WriteInteger("Setup", "RepairWall", M2Config.nRepairWallPrice);
        Config.WriteInteger("Setup", "HireArcher", M2Config.nHireArcherPrice);
        Config.WriteInteger("Setup", "HireGuard", M2Config.nHireGuardPrice);
        Config.WriteInteger("Setup", "CastleGoldMax", M2Config.nCastleGoldMax);
        Config.WriteInteger("Setup", "CastleOneDayGold", M2Config.nCastleOneDayGold);
        Config.WriteString("Setup", "CastleName", M2Config.sCASTLENAME);
        Config.WriteString("Setup", "CastleHomeMap", M2Config.sCastleHomeMap);
        Config.WriteInteger("Setup", "CastleHomeX", M2Config.nCastleHomeX);
        Config.WriteInteger("Setup", "CastleHomeY", M2Config.nCastleHomeY);
        Config.WriteInteger("Setup", "CastleWarRangeX", M2Config.nCastleWarRangeX);
        Config.WriteInteger("Setup", "CastleWarRangeY", M2Config.nCastleWarRangeY);
        Config.WriteInteger("Setup", "CastleTaxRate", M2Config.nCastleTaxRate);
        Config.WriteBool("Setup", "CastleGetAllNpcTax", M2Config.boGetAllNpcTax);
        Config.WriteInteger("Setup", "CastleMemberPriceRate", M2Config.nCastleMemberPriceRate);

        uModValue();
    }

    // ================= Option 页（批次J23） =================

    public void chkDisHumRunClick(object? sender)
    {
        bool boChecked = !chkDisHumRun.Checked;
        if (boChecked)
        {
            chkRunHum.Checked = false;
            chkRunHum.Enabled = false;
            chkRunMon.Checked = false;
            chkRunMon.Enabled = false;
            chkWarDisHumRun.Checked = false;
            chkWarDisHumRun.Enabled = false;
            chkRunNpc.Checked = false;
            chkRunGuard.Checked = false;
            chkRunNpc.Enabled = false;
            chkRunGuard.Enabled = false;
            chkGMRunAll.Checked = false;
            chkGMRunAll.Enabled = false;
            chkSafeArea.Checked = false;
            chkSafeArea.Enabled = false;
            chkSafeAreaDisNpcRun.Checked = false;
            chkSafeAreaDisShopStallHumRun.Enabled = false;
            chkSafeAreaDisOffLineHumRun.Enabled = false;
            chkSafeAreaDisNpcRun.Enabled = false;
            chkSafeAreaDisOffLineHumRun.Checked = false;
            chkSafeAreaDisNpcRun.Checked = false;
            chkSafeAreaDisShopStallHumRun.Checked = false;
            chkWarDisTeleport.Enabled = false;
            chkWarDisTeleport.Checked = false;
        }
        else
        {
            chkRunHum.Enabled = true;
            chkRunMon.Enabled = true;
            chkWarDisHumRun.Enabled = true;
            chkRunNpc.Enabled = true;
            chkRunGuard.Enabled = true;
            chkGMRunAll.Enabled = true;
            chkSafeArea.Enabled = true;
            chkSafeAreaDisShopStallHumRun.Enabled = true;
            chkSafeAreaDisOffLineHumRun.Enabled = true;
            chkSafeAreaDisNpcRun.Enabled = true;
            chkWarDisTeleport.Enabled = true;
        }

        if (!boOpened)
            return;
        M2Config.boDiableHumanRun = boChecked;
        ModValue();
    }

    public void chkRunHumClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRUNHUMAN = chkRunHum.Checked;
        ModValue();
    }

    public void chkRunMonClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRUNMON = chkRunMon.Checked;
        ModValue();
    }

    public void chkRunNpcClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRunNpc = chkRunNpc.Checked;
        ModValue();
    }

    public void chkRunGuardClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRunGuard = chkRunGuard.Checked;
        ModValue();
    }

    public void chkWarDisHumRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boWarDisHumRun = chkWarDisHumRun.Checked;
        chkWarHreoRun.Enabled = chkWarDisHumRun.Checked;
        M2Config.boWarHreoRun = chkWarHreoRun.Enabled && chkWarHreoRun.Checked;
        ModValue();
    }

    public void chkWarHreoRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boWarHreoRun = chkWarHreoRun.Checked;
        ModValue();
    }

    public void chkGMRunAllClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boGMRunAll = chkGMRunAll.Checked;
        ModValue();
    }

    public void chkSafeAreaClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSafeAreaLimited = chkSafeArea.Checked;
        ModValue();
    }

    public void chkSafeAreaDisNpcRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSafeAreaDisNpcRun = chkSafeAreaDisNpcRun.Checked;
        ModValue();
    }

    public void chkSafeAreaDisShopStallHumRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSafeAreaDisShopStallHumRun = chkSafeAreaDisShopStallHumRun.Checked;
        ModValue();
    }

    public void chkSafeAreaDisOffLineHumRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSafeAreaDisOffLineHumRun = chkSafeAreaDisOffLineHumRun.Checked;
        ModValue();
    }

    public void chkWarDisTeleportClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boWarDisTeleport = chkWarDisTeleport.Checked;
        ModValue();
    }

    public void seTryDealTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwTryDealTime = (uint)seTryDealTime.Value * 1000;
        ModValue();
    }

    public void seDealOKTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDealOKTime = (uint)seDealOKTime.Value * 1000;
        ModValue();
    }

    public void chkCanNotGetBackDealClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCanNotGetBackDeal = chkCanNotGetBackDeal.Checked;
        ModValue();
    }

    public void chkDisableDealClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableDeal = chkDisableDeal.Checked;
        ModValue();
    }

    public void chkControlDropItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boControlDropItem = chkControlDropItem.Checked;
        ModValue();
    }

    public void chkIsSafeDisableDropClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boInSafeDisableDrop = chkIsSafeDisableDrop.Checked;
        ModValue();
    }

    public void seCanDropPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanDropPrice = (int)seCanDropPrice.Value;
        ModValue();
    }

    public void seCanDropGoldChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanDropGold = (int)seCanDropGold.Value;
        ModValue();
    }

    public void EditSafeZoneSizeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSafeZoneSize = (int)EditSafeZoneSize.Value;
        ModValue();
    }

    public void chkHintSafeZoneClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHintSafeZone = chkHintSafeZone.Checked;
        seHintSafeZoneY.Enabled = M2Config.boHintSafeZone;
        seHintSafeZoneFColor.Enabled = M2Config.boHintSafeZone;
        seHintSafeZoneBColor.Enabled = M2Config.boHintSafeZone;
        seHintSafeZoneFSize.Enabled = M2Config.boHintSafeZone;
        ModValue();
    }

    public void seHintSafeZoneYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHintSafeZoneY = (uint)seHintSafeZoneY.Value;
        ModValue();
    }

    public void seHintSafeZoneFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btHintSafeZoneFColor = (byte)seHintSafeZoneFColor.Value;
        ModValue();
    }

    public void seHintSafeZoneBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btHintSafeZoneBColor = (byte)seHintSafeZoneBColor.Value;
        ModValue();
    }

    public void seHintSafeZoneFSizeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btHintSafeZoneFSize = (byte)seHintSafeZoneFSize.Value;
        ModValue();
    }

    public void EditStartPointSizeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStartPointSize = (int)EditStartPointSize.Value;
        ModValue();
    }

    public void seGroupMembersMaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGroupMembersMax = (int)seGroupMembersMax.Value;
        ModValue();
    }

    public void EditRedHomeXChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRedHomeX = (int)EditRedHomeX.Value;
        ModValue();
    }

    public void EditRedHomeYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRedHomeY = (int)EditRedHomeY.Value;
        ModValue();
    }

    public void EditRedHomeMapChange(object? sender)
    {
        if (!boOpened)
            return;
        ModValue();
    }

    public void EditRedDieHomeMapChange(object? sender)
    {
        if (!boOpened)
            return;
        ModValue();
    }

    public void EditRedDieHomeXChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRedDieHomeX = (int)EditRedDieHomeX.Value;
        ModValue();
    }

    public void EditRedDieHomeYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRedDieHomeY = (int)EditRedDieHomeY.Value;
        ModValue();
    }

    public void EditHomeMapChange(object? sender)
    {
        if (!boOpened)
            return;
        ModValue();
    }

    public void EditHomeXChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHomeX = (int)EditHomeX.Value;
        ModValue();
    }

    public void EditHomeYChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHomeY = (int)EditHomeY.Value;
        ModValue();
    }

    public void chkNationGroupCheckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNationGroupCheck = chkNationGroupCheck.Checked;
        ModValue();
    }

    public void chkNationGuildCheckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNationGuildCheck = chkNationGuildCheck.Checked;
        ModValue();
    }

    public void seNationSayLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nNationSayLevel = (int)seNationSayLevel.Value;
        ModValue();
    }

    public void seTryChallengeTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwTryChallengeTime = (uint)seTryChallengeTime.Value * 1000;
        ModValue();
    }

    public void seChallengeOKTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwChallengeOKTime = (uint)seChallengeOKTime.Value * 1000;
        ModValue();
    }

    public void chkCanNotGetBackChallengeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCanNotGetBackChallenge = chkCanNotGetBackChallenge.Checked;
        ModValue();
    }

    public void chkDisableChallengeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableChallenge = chkDisableChallenge.Checked;
        ModValue();
    }

    public void seChallengeTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwChallengeTime = (uint)seChallengeTime.Value * 1000 * 60;
        ModValue();
    }

    public void rgChallengeGoldClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btChallengeGoldIndex = (byte)rgChallengeGold.Value;
        ModValue();
    }

    public void ButtonOptionSaveClick(object? sender)
    {
        if (EditRedHomeMap.Text == "")
        {
            M2Forms.MessageBox("红名村地图设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditRedHomeMap.CanFocus)
                EditRedHomeMap.Focus();
            return;
        }
        M2Config.sRedHomeMap = EditRedHomeMap.Text.Trim();

        if (EditRedDieHomeMap.Text == "")
        {
            M2Forms.MessageBox("红名村地图设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditRedDieHomeMap.CanFocus)
                EditRedDieHomeMap.Focus();
            return;
        }
        M2Config.sRedDieHomeMap = EditRedDieHomeMap.Text.Trim();

        if (EditHomeMap.Text == "")
        {
            M2Forms.MessageBox("应急回城地图设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditHomeMap.CanFocus)
                EditHomeMap.Focus();
            return;
        }
        M2Config.sHomeMap = EditHomeMap.Text.Trim();

        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "SafeZoneSize", M2Config.nSafeZoneSize);
        Config.WriteBool("Setup", "HintSafeZone", M2Config.boHintSafeZone);
        Config.WriteInteger("Setup", "HintSafeZoneY", (int)M2Config.dwHintSafeZoneY);
        Config.WriteInteger("Setup", "HintSafeZoneFColor", M2Config.btHintSafeZoneFColor);
        Config.WriteInteger("Setup", "HintSafeZoneBColor", M2Config.btHintSafeZoneBColor);
        Config.WriteInteger("Setup", "HintSafeZoneFSize", M2Config.btHintSafeZoneFSize);

        Config.WriteInteger("Setup", "StartPointSize", M2Config.nStartPointSize);

        Config.WriteString("Setup", "RedHomeMap", M2Config.sRedHomeMap);
        Config.WriteInteger("Setup", "RedHomeX", M2Config.nRedHomeX);
        Config.WriteInteger("Setup", "RedHomeY", M2Config.nRedHomeY);

        Config.WriteString("Setup", "RedDieHomeMap", M2Config.sRedDieHomeMap);
        Config.WriteInteger("Setup", "RedDieHomeX", M2Config.nRedDieHomeX);
        Config.WriteInteger("Setup", "RedDieHomeY", M2Config.nRedDieHomeY);

        Config.WriteString("Setup", "HomeMap", M2Config.sHomeMap);
        Config.WriteInteger("Setup", "HomeX", M2Config.nHomeX);
        Config.WriteInteger("Setup", "HomeY", M2Config.nHomeY);

        Config.WriteBool("Setup", "NationGroupCheck", M2Config.boNationGroupCheck);
        Config.WriteBool("Setup", "NationGuildCheck", M2Config.boNationGuildCheck);
        Config.WriteInteger("Setup", "NationSayLevel", M2Config.nNationSayLevel);

        uModValue();
    }

    public void ButtonOptionSave3Click(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "DiableHumanRun", M2Config.boDiableHumanRun);
        Config.WriteBool("Setup", "RunHuman", M2Config.boRUNHUMAN);
        Config.WriteBool("Setup", "RunMon", M2Config.boRUNMON);
        Config.WriteBool("Setup", "RunNpc", M2Config.boRunNpc);
        Config.WriteBool("Setup", "RunGuard", M2Config.boRunGuard);
        Config.WriteBool("Setup", "WarDisableHumanRun", M2Config.boWarDisHumRun);
        Config.WriteBool("Setup", "GMRunAll", M2Config.boGMRunAll);
        Config.WriteBool("Setup", "SafeAreaLimitedRun", M2Config.boSafeAreaLimited);

        Config.WriteInteger("Setup", "TryDealTime", (int)M2Config.dwTryDealTime);
        Config.WriteInteger("Setup", "DealOKTime", (int)M2Config.dwDealOKTime);
        Config.WriteBool("Setup", "CanNotGetBackDeal", M2Config.boCanNotGetBackDeal);
        Config.WriteBool("Setup", "DisableDeal", M2Config.boDisableDeal);
        Config.WriteBool("Setup", "ControlDropItem", M2Config.boControlDropItem);
        Config.WriteBool("Setup", "InSafeDisableDrop", M2Config.boInSafeDisableDrop);
        Config.WriteInteger("Setup", "CanDropGold", M2Config.nCanDropGold);
        Config.WriteInteger("Setup", "CanDropPrice", M2Config.nCanDropPrice);

        Config.WriteInteger("Setup", "DecLightItemDrugTime", (int)M2Config.dwDecLightItemDrugTime);

        Config.WriteInteger("Setup", "TryChallengeTime", (int)M2Config.dwTryChallengeTime);
        Config.WriteInteger("Setup", "ChallengeOKTime", (int)M2Config.dwChallengeOKTime);
        Config.WriteBool("Setup", "CanNotGetBackChallenge", M2Config.boCanNotGetBackChallenge);
        Config.WriteBool("Setup", "DisableChallenge", M2Config.boDisableChallenge);
        Config.WriteBool("Setup", "SafeAreaDisNpcRun", M2Config.boSafeAreaDisNpcRun);

        // 安全区禁止穿摆摊人物
        Config.WriteBool("Setup", "SafeAreaDisShopStallHumRun", M2Config.boSafeAreaDisShopStallHumRun);
        // 安全区禁止穿离线人物
        Config.WriteBool("Setup", "SafeAreaDisOffLineHumRun", M2Config.boSafeAreaDisOffLineHumRun);
        // 攻城区域禁止传送戒指
        Config.WriteBool("Setup", "WarDisTeleport", M2Config.boWarDisTeleport);
        // 攻城区域禁止穿英雄
        Config.WriteBool("Setup", "WarHreoRun", M2Config.boWarHreoRun);
        // 挑战时间
        Config.WriteInteger("Setup", "ChallengeTime", (int)M2Config.dwChallengeTime);
        // 挑战附加币
        Config.WriteInteger("Setup", "ChallengeGoldIndex", M2Config.btChallengeGoldIndex);

        GameConfigState.SendMapCanRun();
        uModValue();
    }

    // ================= PK 页（批次J23） =================

    public void EditDecPkPointTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDecPkPointTime = (uint)EditDecPkPointTime.Value * 1000;
        ModValue();
    }

    public void EditDecPkPointCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nDecPkPointCount = (int)EditDecPkPointCount.Value;
        ModValue();
    }

    public void EditPKFlagTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwPKFlagTime = (uint)EditPKFlagTime.Value * 1000;
        ModValue();
    }

    public void seHumanAddPKPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nKillHumanAddPKPoint = (int)seHumanAddPKPoint.Value;
        ModValue();
    }

    public void seDummyAddPKPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nDummyAddPKPoint = (int)seDummyAddPKPoint.Value;
        ModValue();
    }

    public void seKillHeroAddPKPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwKillHeroAddPKPoint = (uint)seKillHeroAddPKPoint.Value;
        ModValue();
    }

    public void seKillHumanWeaponUnlockRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwKillHumanWeaponUnlockRate = (uint)seKillHumanWeaponUnlockRate.Value;
        ModValue();
    }

    public void chkHeroKillHumanNotWeaponUnlockClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHeroKillHumanNotWeaponUnlock = chkHeroKillHumanNotWeaponUnlock.Checked;
        ModValue();
    }

    public void CheckBoxKillHumanWinLevelClick(object? sender)
    {
        bool boStatus = CheckBoxKillHumanWinLevel.Checked;
        CheckBoxKilledLostLevel.Enabled = boStatus;
        EditKillHumanWinLevel.Enabled = boStatus;
        EditKilledLostLevel.Enabled = boStatus;
        if (!boStatus)
        {
            CheckBoxKilledLostLevel.Checked = false;
            if (!CheckBoxKillHumanWinExp.Checked)
                EditHumanLevelDiffer.Enabled = false;
        }
        else
        {
            EditHumanLevelDiffer.Enabled = true;
        }
        if (!boOpened)
            return;
        M2Config.boKillHumanWinLevel = boStatus;
        ModValue();
    }

    public void CheckBoxKilledLostLevelClick(object? sender)
    {
        bool boStatus = CheckBoxKilledLostLevel.Checked;
        EditKilledLostLevel.Enabled = boStatus;
        if (!boOpened)
            return;
        M2Config.boKilledLostLevel = boStatus;
        ModValue();
    }

    public void CheckBoxKillHumanWinExpClick(object? sender)
    {
        bool boStatus = CheckBoxKillHumanWinExp.Checked;
        CheckBoxKilledLostExp.Enabled = boStatus;
        EditKillHumanWinExp.Enabled = boStatus;
        EditKillHumanLostExp.Enabled = boStatus;
        if (!boStatus)
        {
            CheckBoxKilledLostExp.Checked = false;
            if (!CheckBoxKillHumanWinLevel.Checked)
                EditHumanLevelDiffer.Enabled = false;
        }
        else
        {
            EditHumanLevelDiffer.Enabled = true;
        }
        if (!boOpened)
            return;
        M2Config.boKillHumanWinExp = boStatus;
        ModValue();
    }

    public void CheckBoxKilledLostExpClick(object? sender)
    {
        bool boStatus = CheckBoxKilledLostExp.Checked;
        EditKillHumanLostExp.Enabled = boStatus;
        if (!boOpened)
            return;
        M2Config.boKilledLostExp = boStatus;
        ModValue();
    }

    public void EditKillHumanWinLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nKillHumanWinLevel = (int)EditKillHumanWinLevel.Value;
        ModValue();
    }

    public void EditKilledLostLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nKilledLostLevel = (int)EditKilledLostLevel.Value;
        ModValue();
    }

    public void EditKillHumanWinExpChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nKillHumanWinExp = (int)EditKillHumanWinExp.Value;
        ModValue();
    }

    public void EditKillHumanLostExpChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nKillHumanLostExp = (int)EditKillHumanLostExp.Value;
        ModValue();
    }

    public void EditHumanLevelDifferChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHumanLevelDiffer = (int)EditHumanLevelDiffer.Value;
        ModValue();
    }

    public void CheckBoxPKLevelProtectClick(object? sender)
    {
        bool boStatus = CheckBoxPKLevelProtect.Checked;
        EditPKProtectLevel.Enabled = boStatus;
        if (!boOpened)
            return;
        M2Config.boPKLevelProtect = boStatus;
        ModValue();
    }

    public void EditPKProtectLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nPKProtectLevel = (int)EditPKProtectLevel.Value;
        ModValue();
    }

    public void EditRedPKProtectLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRedPKProtectLevel = (int)EditRedPKProtectLevel.Value;
        ModValue();
    }

    public void ButtonOptionSave2Click(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "DecPkPointTime", (int)M2Config.dwDecPkPointTime);
        Config.WriteInteger("Setup", "DecPkPointCount", M2Config.nDecPkPointCount);
        Config.WriteInteger("Setup", "PKFlagTime", (int)M2Config.dwPKFlagTime);
        Config.WriteInteger("Setup", "KillHumanAddPKPoint", M2Config.nKillHumanAddPKPoint);
        Config.WriteInteger("Setup", "KillHumanDecLuckPoint", M2Config.nKillHumanDecLuckPoint);

        Config.WriteBool("Setup", "KillHumanWinLevel", M2Config.boKillHumanWinLevel);
        Config.WriteBool("Setup", "KilledLostLevel", M2Config.boKilledLostLevel);
        Config.WriteInteger("Setup", "KillHumanWinLevelPoint", M2Config.nKillHumanWinLevel);
        Config.WriteInteger("Setup", "KilledLostLevelPoint", M2Config.nKilledLostLevel);
        Config.WriteBool("Setup", "KillHumanWinExp", M2Config.boKillHumanWinExp);
        Config.WriteBool("Setup", "KilledLostExp", M2Config.boKilledLostExp);
        Config.WriteInteger("Setup", "KillHumanWinExpPoint", M2Config.nKillHumanWinExp);
        Config.WriteInteger("Setup", "KillHumanLostExpPoint", M2Config.nKillHumanLostExp);
        Config.WriteInteger("Setup", "HumanLevelDiffer", M2Config.nHumanLevelDiffer);

        Config.WriteInteger("Setup", "DummyAddPKPoint", M2Config.nDummyAddPKPoint);

        // 杀英雄增加PK值 (点数)
        Config.WriteInteger("Setup", "KillHeroAddPKPoint", (int)M2Config.dwKillHeroAddPKPoint);

        Config.WriteBool("Setup", "PKProtect", M2Config.boPKLevelProtect);
        Config.WriteInteger("Setup", "PKProtectLevel", M2Config.nPKProtectLevel);
        Config.WriteInteger("Setup", "RedPKProtectLevel", M2Config.nRedPKProtectLevel);

        Config.WriteInteger("Setup", "KillHumanWeaponUnlockRate", (int)M2Config.dwKillHumanWeaponUnlockRate);
        Config.WriteBool("Setup", "HeroKillHumanNotWeaponUnlock", M2Config.boHeroKillHumanNotWeaponUnlock);

        uModValue();
    }

    // ================= 测试服/试玩页（批次J23） =================

    public void CheckBoxTestServerClick(object? sender)
    {
        bool boStatue = CheckBoxTestServer.Checked;
        seTestLevel.Enabled = boStatue;
        seTestGold.Enabled = boStatue;
        seTestUserLimit.Enabled = boStatue;
        if (!boOpened)
            return;
        M2Config.boTestServer = boStatue;
        ModValue();
    }

    public void CheckBoxServiceModeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boServiceMode = CheckBoxServiceMode.Checked;
        ModValue();
    }

    public void CheckBoxVentureModeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boVentureServer = CheckBoxVentureMode.Checked;
        ModValue();
    }

    public void CheckBoxNonPKModeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNonPKServer = CheckBoxNonPKMode.Checked;
        ModValue();
    }

    public void seTestLevelChange(object? sender)
    {
        // Delphi 空文本分支在 NumericUpDown 无空态，语义保留于 _seTestLevelTag 状态机
        if (_seTestLevelTag == 1 && seTestLevel.Value != 0)
        {
            _seTestLevelTag = 0;
            seTestLevel.Value = seTestLevel.Value / 10;
        }
        if (!boOpened)
            return;
        M2Config.nTestLevel = (int)seTestLevel.Value;
        ModValue();
    }

    public void seTestGoldChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nTestGold = (int)seTestGold.Value;
        ModValue();
    }

    public void seTestUserLimitChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nTestUserLimit = (int)seTestUserLimit.Value;
        ModValue();
    }

    public void seStartPermissionChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStartPermission = (int)seStartPermission.Value;
        ModValue();
    }

    public void seUserFullChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUserFull = (int)seUserFull.Value;
        ModValue();
    }

    public void seHumanMaxGoldChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHumanMaxGold = (int)seHumanMaxGold.Value;
        ModValue();
    }

    public void seHumanTryModeMaxGoldChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHumanTryModeMaxGold = (int)seHumanTryModeMaxGold.Value;
        ModValue();
    }

    public void seTryModeLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nTryModeLevel = (int)seTryModeLevel.Value;
        ModValue();
    }

    public void CheckBoxTryModeUseStorageClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boTryModeUseStorage = CheckBoxTryModeUseStorage.Checked;
        ModValue();
    }

    public void seGuildMemberMaxLimitChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGuildMemberMaxLimit = (int)seGuildMemberMaxLimit.Value;
        ModValue();
    }

    public void seGuildNameLenChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGuildNameLen = (int)seGuildNameLen.Value;
        ModValue();
    }

    public void seGuildRankNameLenChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGuildRankNameLen = (int)seGuildRankNameLen.Value;
        ModValue();
    }

    public void seHumChgMapOrLoginProtectTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHumChgMapOrLoginProtectTime = (uint)seHumChgMapOrLoginProtectTime.Value;
        ModValue();
    }

    public void chkOffLineShopClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boOffLineShop = chkOffLineShop.Checked;
        ModValue();
    }

    public void chkOffLineHeroClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boOffLineHero = chkOffLineHero.Checked;
        ModValue();
    }

    public void chkOffLineSlaveClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boOffLineSlave = chkOffLineSlave.Checked;
        ModValue();
    }

    public void chkSellItemToNpcShopNoCalcAddPropertyClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSellItemToNpcShopNoCalcAddProperty = chkSellItemToNpcShopNoCalcAddProperty.Checked;
        ModValue();
    }

    public void chkShowNewValueFromBuyNpcItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShowNewValueFromBuyNpcItem = chkShowNewValueFromBuyNpcItem.Checked;
        ModValue();
    }

    public void ButtonOptionSave0Click(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteString("Server", "TestServer", M2Config.boTestServer ? "-1" : "0");
        Config.WriteInteger("Server", "TestLevel", M2Config.nTestLevel);
        Config.WriteInteger("Server", "TestGold", M2Config.nTestGold);
        Config.WriteInteger("Server", "TestServerUserLimit", M2Config.nTestUserLimit);
        Config.WriteString("Server", "ServiceMode", M2Config.boServiceMode ? "-1" : "0");
        Config.WriteString("Server", "NonPKServer", M2Config.boNonPKServer ? "-1" : "0");
        Config.WriteString("Server", "VentureServer", M2Config.boVentureServer ? "-1" : "0");
        Config.WriteInteger("Setup", "StartPermission", M2Config.nStartPermission);
        Config.WriteInteger("Server", "UserFull", M2Config.nUserFull);
        Config.WriteInteger("Setup", "HumChgMapOrLoginProtectTime", (int)M2Config.dwHumChgMapOrLoginProtectTime);

        Config.WriteInteger("Setup", "HumanMaxGold", M2Config.nHumanMaxGold);
        Config.WriteInteger("Setup", "HumanTryModeMaxGold", M2Config.nHumanTryModeMaxGold);
        Config.WriteInteger("Setup", "TryModeLevel", M2Config.nTryModeLevel);
        Config.WriteBool("Setup", "TryModeUseStorage", M2Config.boTryModeUseStorage);
        Config.WriteInteger("Setup", "GroupMembersMax", M2Config.nGroupMembersMax);
        Config.WriteInteger("Setup", "MaxLevel", M2Config.btMaxLevel);
        Config.WriteInteger("Setup", "MaxAC", M2Config.btMaxAC);
        Config.WriteInteger("Setup", "MaxHitPoint", M2Config.btMaxHitPoint);

        // 行会人数限制
        Config.WriteInteger("Setup", "GuildMemberMaxLimit", M2Config.nGuildMemberMaxLimit);
        // 离线挂机禁止项
        Config.WriteBool("Setup", "OffLineShop", M2Config.boOffLineShop);
        Config.WriteBool("Setup", "OffLineHero", M2Config.boOffLineHero);
        Config.WriteBool("Setup", "OffLineSlave", M2Config.boOffLineSlave);

        Config.WriteInteger("Setup", "GuildNameLen", M2Config.nGuildNameLen);
        Config.WriteInteger("Setup", "GuildRankNameLen", M2Config.nGuildRankNameLen);

        if (boSendServerConfig)
            GameConfigState.SendServerConfig();

        uModValue();
    }

    // ================= General/经验页（批次J24） =================

    public void EditSoftVersionDateChange(object? sender)
    {
        if (!boOpened)
            return;
        ModValue();
    }

    public void EditConsoleShowUserCountTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwConsoleShowUserCountTime = (uint)EditConsoleShowUserCountTime.Value * 1000;
        ModValue();
    }

    public void EditShowLineNoticeTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwShowLineNoticeTime = (uint)EditShowLineNoticeTime.Value * 1000;
        ModValue();
    }

    public void ComboBoxLineNoticeColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nLineNoticeColor = ComboBoxLineNoticeColor.SelectedIndex;
        ModValue();
    }

    public void EditLineNoticePreFixChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sLineNoticePreFix = EditLineNoticePreFix.Text.Trim();
        ModValue();
    }

    public void CheckBoxShowMakeItemMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShowMakeItemMsg = CheckBoxShowMakeItemMsg.Checked;
        ModValue();
    }

    public void CbViewHackClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boViewHackMessage = CbViewHack.Checked;
        ModValue();
    }

    public void CkViewAdmfailClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boViewAdmissionFailure = CkViewAdmfail.Checked;
        ModValue();
    }

    public void CheckBoxShowExceptionMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShowExceptionMsg = CheckBoxShowExceptionMsg.Checked;
        ModValue();
    }

    public void CheckBoxCanOldClientLogonClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCanOldClientLogon = CheckBoxCanOldClientLogon.Checked;
        ModValue();
    }

    public void chkOldClientClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boIsOldClient = chkOldClient.Checked;
        ModValue();
    }

    public void CheckBoxSendOnlineCountClick(object? sender)
    {
        bool boStatus = CheckBoxSendOnlineCount.Checked;
        EditSendOnlineCountRate.Enabled = boStatus;
        EditSendOnlineTime.Enabled = boStatus;
        if (!boOpened)
            return;
        M2Config.boSendOnlineCount = boStatus;
        ModValue();
    }

    public void EditSendOnlineCountRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSendOnlineCountRate = (int)EditSendOnlineCountRate.Value;
        ModValue();
    }

    public void EditSendOnlineTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwSendOnlineTime = (uint)EditSendOnlineTime.Value * 1000;
        ModValue();
    }

    public void EditMonsterPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMonsterPowerRate = (int)EditMonsterPowerRate.Value;
        ModValue();
    }

    public void EditEditItemsPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemsPowerRate = (int)EditEditItemsPowerRate.Value;
        ModValue();
    }

    public void EditItemsACPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemsACPowerRate = (int)EditItemsACPowerRate.Value;
        ModValue();
    }

    public void chkRecordPublicMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordPublicMsg = chkRecordPublicMsg.Checked;
        ModValue();
    }

    public void chkRecordPrivateMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordPrivateMsg = chkRecordPrivateMsg.Checked;
        ModValue();
    }

    public void chkRecordGuildMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordGuildMsg = chkRecordGuildMsg.Checked;
        ModValue();
    }

    public void chkRecordCryCryMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordCryCryMsg = chkRecordCryCryMsg.Checked;
        ModValue();
    }

    public void chkRecordGroupMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordGroupMsg = chkRecordGroupMsg.Checked;
        ModValue();
    }

    public void chkRecordNationMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRecordNationMsg = chkRecordNationMsg.Checked;
        ModValue();
    }

    public void chkPermissionChangeLogClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boPermissionChangeLog = chkPermissionChangeLog.Checked;
        ModValue();
    }

    public void EditKillMonExpMultipleChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwKillMonExpMultiple = (uint)EditKillMonExpMultiple.Value;
        ModValue();
    }

    public void CheckBoxHighLevelKillMonFixExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHighLevelKillMonFixExp = CheckBoxHighLevelKillMonFixExp.Checked;
        ModValue();
    }

    public void CheckBoxHighLevelGroupFixExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHighLevelGroupFixExp = CheckBoxHighLevelGroupFixExp.Checked;
        ModValue();
    }

    public void EditMaxUpLevelCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxUpLevelCount = (int)EditMaxUpLevelCount.Value;
        ModValue();
    }

    public void GridLevelExpSetEditText(object? sender, int rowIndex)
    {
        if (!boOpened)
            return;
        ModValue();
    }

    /// <summary>ComboBoxLevelExpClick 1:1（19 经验计划；IDNO 早退；标准计划 4000000000 div 1000 基数、
    /// 26+I 起覆盖；倍数计划对 1..1000 级整除取 1 下限；结尾刷新表格 + ModValue）。</summary>
    public void ComboBoxLevelExpClick(object? sender)
    {
        const uint HIGH_VALUE = 4200000000;
        if (!boOpened)
            return;
        if (M2Forms.MessageBox("升级经验计划设置的经验将立即生效，是否确认使用此经验计划？", "确认信息",
                M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDNO)
        {
            return;
        }
        var levelExpScheme = (TLevelExpScheme)ComboBoxLevelExp.SelectedIndex;
        switch (levelExpScheme)
        {
            case TLevelExpScheme.s_OldLevelExp:
                Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001);
                break;
            case TLevelExpScheme.s_StdLevelExp:
            {
                bool isHighLongWord = false;
                Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001);
                uint dwOneLevelExp = 4000000000u / 1000u; // High(dwNeedExps) = 1000
                for (int i = 1; i <= AbilRecalc.MAXCHANGELEVEL; i++)
                {
                    if (26 + i > AbilRecalc.MAXCHANGELEVEL)
                        break;
                    uint dwExp;
                    if (isHighLongWord)
                    {
                        dwExp = HIGH_VALUE;
                    }
                    else
                    {
                        long int64Value = (long)dwOneLevelExp * i;
                        if (int64Value >= HIGH_VALUE)
                        {
                            isHighLongWord = true;
                            int64Value = HIGH_VALUE;
                        }
                        dwExp = (uint)int64Value;
                    }
                    if (dwExp == 0)
                        dwExp = 1;
                    M2Config.dwNeedExps[26 + i] = dwExp;
                }
                break;
            }
            default:
            {
                uint div = levelExpScheme switch
                {
                    TLevelExpScheme.s_2Mult => 2,
                    TLevelExpScheme.s_5Mult => 5,
                    TLevelExpScheme.s_8Mult => 8,
                    TLevelExpScheme.s_10Mult => 10,
                    TLevelExpScheme.s_20Mult => 20,
                    TLevelExpScheme.s_30Mult => 30,
                    TLevelExpScheme.s_40Mult => 40,
                    TLevelExpScheme.s_50Mult => 50,
                    TLevelExpScheme.s_60Mult => 60,
                    TLevelExpScheme.s_70Mult => 70,
                    TLevelExpScheme.s_80Mult => 80,
                    TLevelExpScheme.s_90Mult => 90,
                    TLevelExpScheme.s_100Mult => 100,
                    TLevelExpScheme.s_150Mult => 150,
                    TLevelExpScheme.s_200Mult => 200,
                    TLevelExpScheme.s_250Mult => 250,
                    TLevelExpScheme.s_300Mult => 300,
                    _ => 1,
                };
                for (int i = 1; i <= AbilRecalc.MAXCHANGELEVEL; i++)
                {
                    uint dwExp = M2Config.dwNeedExps[i] / div;
                    if (dwExp == 0)
                        dwExp = 1;
                    M2Config.dwNeedExps[i] = dwExp;
                }
                break;
            }
        }
        for (int i = 1; i <= 1000; i++)
            GridLevelExp.Rows[i - 1].Cells[1].Value = M2Config.dwNeedExps[i].ToString();
        ModValue();
    }

    public void CheckBoxFixExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boUseFixExp = CheckBoxFixExp.Checked;
        SpinEditBaseExp.Enabled = !CheckBoxFixExp.Checked;
        SpinEditAddExp.Enabled = !CheckBoxFixExp.Checked;
        ModValue();
    }

    public void SpinEditBaseExpChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nBaseExp = (int)SpinEditBaseExp.Value;
        ModValue();
    }

    public void SpinEditAddExpChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nAddExp = (int)SpinEditAddExp.Value;
        ModValue();
    }

    public void EditHighLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHighLevel = (int)EditHighLevel.Value;
        ModValue();
    }

    public void EditHighLevelGetExpChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHighLevelGetExp = (int)EditHighLevelGetExp.Value;
        ModValue();
    }

    public void CheckBoxLimitChangeExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLimitChangeExp = CheckBoxLimitChangeExp.Checked;
        ModValue();
    }

    public void RadioGroupMaxLevelClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btMaxLevel = (byte)RadioGroupMaxLevel.Value;
        ModValue();
    }

    public void rgMaxACClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btMaxAC = (byte)rgMaxAC.Value;
        ModValue();
    }

    public void rgMaxHitPointClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btMaxHitPoint = (byte)rgMaxHitPoint.Value;
        ModValue();
    }

    public void ButtonGeneralSaveClick(object? sender)
    {
        int softVersionDate = GXX.Core.Rtl.DelphiRTL.StrToIntDef(EditSoftVersionDate.Text.Trim(), -1);
        if (softVersionDate < 0)
        {
            M2Forms.MessageBox("客户端版号设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditSoftVersionDate.CanFocus)
                EditSoftVersionDate.Focus();
            return;
        }
        M2Config.nSoftVersionDate = softVersionDate;

        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "SoftVersionDate", M2Config.nSoftVersionDate);
        Config.WriteInteger("Setup", "ConsoleShowUserCountTime", (int)M2Config.dwConsoleShowUserCountTime);
        Config.WriteInteger("Setup", "ShowLineNoticeTime", (int)M2Config.dwShowLineNoticeTime);
        Config.WriteInteger("Setup", "LineNoticeColor", M2Config.nLineNoticeColor);
        M2ShareState.StringConfIni.WriteString("String", "LineNoticePreFix", M2Config.sLineNoticePreFix);
        Config.WriteBool("Setup", "ShowMakeItemMsg", M2Config.boShowMakeItemMsg);
        Config.WriteString("Server", "ViewHackMessage", M2Config.boViewHackMessage ? "-1" : "0");
        Config.WriteString("Server", "ViewAdmissionFailure", M2Config.boViewAdmissionFailure ? "-1" : "0");
        Config.WriteBool("Setup", "ShowExceptionMsg", M2Config.boShowExceptionMsg);

        Config.WriteBool("Setup", "SendOnlineCount", M2Config.boSendOnlineCount);
        Config.WriteInteger("Setup", "SendOnlineCountRate", M2Config.nSendOnlineCountRate);
        Config.WriteInteger("Setup", "SendOnlineTime", (int)M2Config.dwSendOnlineTime);

        Config.WriteInteger("Setup", "MonsterPowerRate", M2Config.nMonsterPowerRate);
        Config.WriteInteger("Setup", "ItemsPowerRate", M2Config.nItemsPowerRate);
        Config.WriteInteger("Setup", "ItemsACPowerRate", M2Config.nItemsACPowerRate);
        Config.WriteBool("Setup", "CanOldClientLogon", M2Config.boCanOldClientLogon);
        Config.WriteBool("Setup", "IsOldClient", M2Config.boIsOldClient);

        Config.WriteBool("Setup", "RecordPublicMsg", M2Config.boRecordPublicMsg);
        Config.WriteBool("Setup", "RecordPrivateMsg", M2Config.boRecordPrivateMsg);
        Config.WriteBool("Setup", "RecordGuildMsg", M2Config.boRecordGuildMsg);
        Config.WriteBool("Setup", "RecordCryCryMsg", M2Config.boRecordCryCryMsg);
        Config.WriteBool("Setup", "RecordGroupMsg", M2Config.boRecordGroupMsg);
        Config.WriteBool("Setup", "RecordNationMsg", M2Config.boRecordNationMsg);
        Config.WriteBool("Setup", "PermissionChangeLog", M2Config.boPermissionChangeLog);

        uModValue();
    }

    /// <summary>ButtonExpSaveClick 1:1（网格逐行非 0 校验、写 Exps.ini 全键：Level1..1000/
    /// LevelExpRate1..1000/固定经验/倍率/组队共享；原文写 'Setup' 节 ShareExp* 到 ExpConfig）。</summary>
    public void ButtonExpSaveClick(object? sender)
    {
        var needExps = new uint[1001];
        for (int i = 1; i <= 1000; i++)
        {
            var cell = GridLevelExp.Rows[i - 1].Cells[1].Value?.ToString() ?? "";
            long v = GXX.Core.Rtl.DelphiRTL.StrToInt64Def(cell, 0);
            uint dwExp = (uint)Math.Max(0L, Math.Min(v, uint.MaxValue));
            if (dwExp == 0)
            {
                M2Forms.MessageBox($"等级 {i} 升级经验设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
                if (GridLevelExp.IsHandleCreated)
                    GridLevelExp.CurrentCell = GridLevelExp.Rows[i - 1].Cells[1];
                return;
            }
            needExps[i] = dwExp;
        }
        Array.Copy(needExps, M2Config.dwNeedExps, 1001);

        for (int i = 1; i <= 1000; i++)
        {
            var cell = GridLevelExpRate.Rows[i - 1].Cells[1].Value?.ToString() ?? "";
            M2Config.LevelExpRates[i] = (uint)GXX.Core.Rtl.DelphiRTL.StrToIntDef(cell, 0);
        }

        var ExpConfig = M2ShareState.ExpConfigIni;
        ExpConfig.WriteInteger("Exp", "KillMonExpMultiple", (int)M2Config.dwKillMonExpMultiple);
        ExpConfig.WriteBool("Exp", "HighLevelKillMonFixExp", M2Config.boHighLevelKillMonFixExp);
        for (int i = 1; i <= 1000; i++)
            ExpConfig.WriteString("Exp", "Level" + i, M2Config.dwNeedExps[i].ToString());
        for (int i = 1; i <= 1000; i++)
            ExpConfig.WriteInteger("Exp", "LevelExpRate" + i, (int)M2Config.LevelExpRates[i]);

        ExpConfig.WriteBool("Exp", "UseFixExp", M2Config.boUseFixExp);
        ExpConfig.WriteInteger("Exp", "BaseExp", M2Config.nBaseExp);
        ExpConfig.WriteInteger("Exp", "AddExp", M2Config.nAddExp);
        ExpConfig.WriteInteger("Exp", "HighLevel", M2Config.nHighLevel);
        ExpConfig.WriteInteger("Exp", "HighLevelGetExp", M2Config.nHighLevelGetExp);
        ExpConfig.WriteBool("Exp", "LimitChangeExp", M2Config.boLimitChangeExp);
        ExpConfig.WriteInteger("Exp", "MaxUpLevelCount", M2Config.nMaxUpLevelCount);

        ExpConfig.WriteBool("Exp", "HighLevelGroupFixExp", M2Config.boHighLevelGroupFixExp);

        ExpConfig.WriteBool("Setup", "ShareExpGroupSameScreen", M2Config.boShareExpGroupSameScreen);
        ExpConfig.WriteBool("Setup", "ShareExpGroupSameMap", M2Config.boShareExpGroupSameMap);
        ExpConfig.WriteBool("Setup", "ShareExpHeroSameMap", M2Config.boShareExpHeroSameMap);

        uModValue();
    }

    // ================= Msg 页（批次J24） =================

    public void EditSayMsgMaxLenChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSayMsgMaxLen = (int)EditSayMsgMaxLen.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void EditSayRedMsgMaxLenChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSayRedMsgMaxLen = (int)EditSayRedMsgMaxLen.Value;
        ModValue();
    }

    public void EditCanShoutMsgLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nCanShoutMsgLevel = (int)EditCanShoutMsgLevel.Value;
        ModValue();
    }

    public void EditSayMsgTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwSayMsgTime = (uint)EditSayMsgTime.Value * 1000;
        ModValue();
    }

    public void EditSayMsgCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSayMsgCount = (int)EditSayMsgCount.Value;
        ModValue();
    }

    public void EditDisableSayMsgTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDisableSayMsgTime = (uint)EditDisableSayMsgTime.Value * 1000;
        ModValue();
    }

    public void CheckBoxShutRedMsgShowGMNameClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShutRedMsgShowGMName = CheckBoxShutRedMsgShowGMName.Checked;
        ModValue();
    }

    public void CheckBoxShowPreFixMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShowPreFixMsg = CheckBoxShowPreFixMsg.Checked;
        ModValue();
    }

    public void EditGMRedMsgCmdChange(object? sender)
    {
        if (!boOpened)
            return;
        string sCmd = EditGMRedMsgCmd.Text;
        M2ShareState.g_GMRedMsgCmd = sCmd != "" ? sCmd[0] : '\0';
        ModValue();
    }

    public void CheckBoxShowWhisperLevelMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boShowWhisperLevelMsg = CheckBoxShowWhisperLevelMsg.Checked;
        ModValue();
    }

    public void EditUserItemSayMsgTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUserItemSayMsgTime = (int)EditUserItemSayMsgTime.Value;
        ModValue();
    }

    public void seMaxInputStringLenChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxInputStringLen = (int)seMaxInputStringLen.Value;
        ModValue();
    }

    public void ButtonMsgSaveClick(object? sender)
    {
        string sShowWhisperLevelMsg = EditShowWhisperLevelMsg.Text;
        if (!sShowWhisperLevelMsg.Contains("%u"))
        {
            M2Forms.MessageBox("私聊后缀信息设置错误！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditShowWhisperLevelMsg.CanFocus)
                EditShowWhisperLevelMsg.Focus();
            return;
        }
        M2ShareState.g_sShowWhisperLevelMsg = sShowWhisperLevelMsg;

        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "SayMsgMaxLen", M2Config.nSayMsgMaxLen);
        Config.WriteInteger("Setup", "SayMsgTime", (int)M2Config.dwSayMsgTime);
        Config.WriteInteger("Setup", "SayMsgCount", M2Config.nSayMsgCount);
        Config.WriteInteger("Setup", "SayRedMsgMaxLen", M2Config.nSayRedMsgMaxLen);
        Config.WriteBool("Setup", "ShutRedMsgShowGMName", M2Config.boShutRedMsgShowGMName);
        Config.WriteInteger("Setup", "CanShoutMsgLevel", M2Config.nCanShoutMsgLevel);
        M2ShareState.CommandConfIni.WriteString("Command", "GMRedMsgCmd", M2ShareState.g_GMRedMsgCmd.ToString());
        Config.WriteBool("Setup", "ShowPreFixMsg", M2Config.boShowPreFixMsg);
        Config.WriteBool("Setup", "ShowWhisperLevelMsg", M2Config.boShowWhisperLevelMsg);
        M2ShareState.StringConfIni.WriteString("String", "ShowWhisperLevelMsg", M2ShareState.g_sShowWhisperLevelMsg);
        Config.WriteInteger("Setup", "UserItemSayMsgTime", M2Config.nUserItemSayMsgTime);
        Config.WriteInteger("Setup", "DisableSayMsgTime", (int)M2Config.dwDisableSayMsgTime);
        Config.WriteInteger("Setup", "MaxInputStringLen", M2Config.nMaxInputStringLen);

        if (boSendServerConfig)
            GameConfigState.SendServerConfig();

        uModValue();
    }

    // ================= Time 页（批次J24） =================

    public void EditStartCastleWarDaysChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStartCastleWarDays = (int)EditStartCastleWarDays.Value;
        ModValue();
    }

    public void EditStartCastlewarTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStartCastlewarTime = (int)EditStartCastlewarTime.Value;
        ModValue();
    }

    public void EditShowCastleWarEndMsgTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwShowCastleWarEndMsgTime = (uint)EditShowCastleWarEndMsgTime.Value * (60 * 1000);
        ModValue();
    }

    public void EditCastleWarTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwCastleWarTime = (uint)EditCastleWarTime.Value * (60 * 1000);
        ModValue();
    }

    public void EditGetCastleTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwGetCastleTime = (uint)EditGetCastleTime.Value * (60 * 1000);
        ModValue();
    }

    public void EditGuildWarTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwGuildWarTime = (uint)EditGuildWarTime.Value * (60 * 1000);
        ModValue();
    }

    public void seMakeGhostTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMakeGhostTime = (uint)seMakeGhostTime.Value * 1000;
        ModValue();
    }

    public void seMakeMonGhostTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMakeMonGhostTime = (uint)seMakeMonGhostTime.Value * 1000;
        ModValue();
    }

    public void seMakeDummyGhostTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMakeDummyGhostTime = (uint)seMakeDummyGhostTime.Value * 1000;
        ModValue();
    }

    public void seClearDropOnFloorItemTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwClearDropOnFloorItemTime = (uint)seClearDropOnFloorItemTime.Value * 1000;
        ModValue();
    }

    public void seSaveHumanRcdTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwSaveHumanRcdTime = (uint)seSaveHumanRcdTime.Value * (60 * 1000);
        ModValue();
    }

    public void seHumanFreeDelayTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHumanFreeDelayTime = (uint)seHumanFreeDelayTime.Value * (60 * 1000);
        ModValue();
    }

    public void seGetDBSockMsgTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwGetDBSockMsgTime = (uint)seGetDBSockMsgTime.Value * 1000;
        ModValue();
    }

    public void seFloorItemCanPickUpTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwFloorItemCanPickUpTime = (uint)seFloorItemCanPickUpTime.Value * 1000;
        ModValue();
    }

    public void seHorseTakeTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHorseTakeTime = (uint)seHorseTakeTime.Value;
        ModValue();
    }

    public void seTakeOnHorseUseTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwTakeOnHorseUseTime = (uint)seTakeOnHorseUseTime.Value;
        ModValue();
    }

    public void chkReadyOnHorseDisableActionClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boReadyOnHorseDisableAction = chkReadyOnHorseDisableAction.Checked;
        ModValue();
    }

    public void seNpcButtonClickTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwNpcButtonClickTime = (uint)seNpcButtonClickTime.Value;
        ModValue();
    }

    public void seNpcActorClickTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwNpcActorClickTime = (uint)seNpcActorClickTime.Value;
        ModValue();
    }

    public void sePlayerVarJClearTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btPlayerVarJClearTime = (byte)sePlayerVarJClearTime.Value;
        ModValue();
    }

    public void seDearRecallTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwDearRecallTime = (uint)seDearRecallTime.Value;
        ModValue();
    }

    public void seMasterRecallTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMasterRecallTime = (uint)seMasterRecallTime.Value;
        ModValue();
    }

    public void seGroupRecallTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwGroupRecallTime = (uint)seGroupRecallTime.Value;
        ModValue();
    }

    public void ButtonTimeSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "StartCastleWarDays", M2Config.nStartCastleWarDays);
        Config.WriteInteger("Setup", "StartCastlewarTime", M2Config.nStartCastlewarTime);
        Config.WriteInteger("Setup", "ShowCastleWarEndMsgTime", (int)M2Config.dwShowCastleWarEndMsgTime);
        Config.WriteInteger("Setup", "CastleWarTime", (int)M2Config.dwCastleWarTime);
        Config.WriteInteger("Setup", "GetCastleTime", (int)M2Config.dwGetCastleTime);
        Config.WriteInteger("Setup", "GuildWarTime", (int)M2Config.dwGuildWarTime);
        Config.WriteInteger("Setup", "SaveHumanRcdTime", (int)M2Config.dwSaveHumanRcdTime);
        Config.WriteInteger("Setup", "HumanFreeDelayTime", (int)M2Config.dwHumanFreeDelayTime);
        Config.WriteInteger("Setup", "GetDBSockMsgTime", (int)M2Config.dwGetDBSockMsgTime);
        Config.WriteInteger("Setup", "MakeGhostTime", (int)M2Config.dwMakeGhostTime);
        Config.WriteInteger("Setup", "MakeMonGhostTime", (int)M2Config.dwMakeMonGhostTime);
        Config.WriteInteger("Setup", "MakeDummyGhostTime", (int)M2Config.dwMakeDummyGhostTime);
        Config.WriteInteger("Setup", "ClearDropOnFloorItemTime", (int)M2Config.dwClearDropOnFloorItemTime);
        Config.WriteInteger("Setup", "FloorItemCanPickUpTime", (int)M2Config.dwFloorItemCanPickUpTime);
        Config.WriteInteger("Setup", "DearRecallTime", (int)M2Config.dwDearRecallTime);
        Config.WriteInteger("Setup", "DearRecallTime", (int)M2Config.dwMasterRecallTime); // Delphi 原文重复键名取 Master 值，保留
        Config.WriteInteger("Setup", "GroupRecallTime", (int)M2Config.dwGroupRecallTime);
        Config.WriteInteger("Setup", "HorseTakeTime", (int)M2Config.dwHorseTakeTime);
        Config.WriteInteger("Setup", "TakeOnHorseUseTime", (int)M2Config.dwTakeOnHorseUseTime);
        Config.WriteBool("Setup", "ReadyOnHorseDisableAction", M2Config.boReadyOnHorseDisableAction);
        Config.WriteInteger("Setup", "NpcButtonClickTime", (int)M2Config.dwNpcButtonClickTime);
        Config.WriteInteger("Setup", "NpcActorClickTime", (int)M2Config.dwNpcActorClickTime);
        Config.WriteInteger("Setup", "PlayerVarJClearTime", M2Config.btPlayerVarJClearTime);
        uModValue();
    }

    // ================= Price 页（批次J24） =================

    public void EditBuildGuildPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nBuildGuildPrice = (int)EditBuildGuildPrice.Value;
        ModValue();
    }

    public void EditGuildWarPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGuildWarPrice = (int)EditGuildWarPrice.Value;
        ModValue();
    }

    public void EditMakeDurgPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMakeDurgPrice = (int)EditMakeDurgPrice.Value;
        ModValue();
    }

    public void EditSuperRepairPriceRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSuperRepairPriceRate = (int)EditSuperRepairPriceRate.Value;
        ModValue();
    }

    public void EditRepairItemDecDuraChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRepairItemDecDura = (int)EditRepairItemDecDura.Value;
        ModValue();
    }

    public void ButtonPriceSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "BuildGuild", M2Config.nBuildGuildPrice);
        Config.WriteInteger("Setup", "MakeDurg", M2Config.nMakeDurgPrice);
        Config.WriteInteger("Setup", "GuildWarFee", M2Config.nGuildWarPrice);
        Config.WriteInteger("Setup", "SuperRepairPriceRate", M2Config.nSuperRepairPriceRate);
        Config.WriteInteger("Setup", "RepairItemDecDura", M2Config.nRepairItemDecDura);
        Config.WriteBool("Setup", "SellItemToNpcShopNoCalcAddProperty", M2Config.boSellItemToNpcShopNoCalcAddProperty);
        Config.WriteBool("Setup", "ShowNewValueFromBuyNpcItem", M2Config.boShowNewValueFromBuyNpcItem);
        uModValue();
    }

    // ================= MsgColor 页（批次J25，色对处理器逐控件 1:1） =================

    public void EditHearMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btHearMsgFColor = (byte)EditHearMsgFColor.Value;
        ModValue();
    }

    public void EdittHearMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btHearMsgBColor = (byte)EdittHearMsgBColor.Value;
        ModValue();
    }

    public void EditWhisperMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btWhisperMsgFColor = (byte)EditWhisperMsgFColor.Value;
        ModValue();
    }

    public void EditWhisperMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btWhisperMsgBColor = (byte)EditWhisperMsgBColor.Value;
        ModValue();
    }

    public void EditGMWhisperMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGMWhisperMsgFColor = (byte)EditGMWhisperMsgFColor.Value;
        ModValue();
    }

    public void EditGMWhisperMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGMWhisperMsgBColor = (byte)EditGMWhisperMsgBColor.Value;
        ModValue();
    }

    public void seSendWhisperMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btSendWhisperMsgFColor = (byte)seSendWhisperMsgFColor.Value;
        ModValue();
    }

    public void seSendWhisperMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btSendWhisperMsgBColor = (byte)seSendWhisperMsgBColor.Value;
        ModValue();
    }

    public void seRefreshGameGoldFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btRefreshGameGoldFColor = (byte)seRefreshGameGoldFColor.Value;
        ModValue();
    }

    public void seRefreshGameGoldBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btRefreshGameGoldBColor = (byte)seRefreshGameGoldBColor.Value;
        ModValue();
    }

    public void seShowWhisperFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btShowWhisperFColor = (byte)seShowWhisperFColor.Value;
        ModValue();
    }

    public void seShowWhisperBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btShowWhisperBColor = (byte)seShowWhisperBColor.Value;
        ModValue();
    }

    public void seCloseWhisperFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCloseWhisperFColor = (byte)seCloseWhisperFColor.Value;
        ModValue();
    }

    public void seCloseWhisperBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCloseWhisperBColor = (byte)seCloseWhisperBColor.Value;
        ModValue();
    }

    public void EditRedMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btRedMsgFColor = (byte)EditRedMsgFColor.Value;
        ModValue();
    }

    public void EditRedMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btRedMsgBColor = (byte)EditRedMsgBColor.Value;
        ModValue();
    }

    public void EditGreenMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGreenMsgFColor = (byte)EditGreenMsgFColor.Value;
        ModValue();
    }

    public void EditGreenMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGreenMsgBColor = (byte)EditGreenMsgBColor.Value;
        ModValue();
    }

    public void EditBlueMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btBlueMsgFColor = (byte)EditBlueMsgFColor.Value;
        ModValue();
    }

    public void EditBlueMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btBlueMsgBColor = (byte)EditBlueMsgBColor.Value;
        ModValue();
    }

    public void EditCryMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCryMsgFColor = (byte)EditCryMsgFColor.Value;
        ModValue();
    }

    public void EditCryMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCryMsgBColor = (byte)EditCryMsgBColor.Value;
        ModValue();
    }

    public void EditGuildMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGuildMsgFColor = (byte)EditGuildMsgFColor.Value;
        ModValue();
    }

    public void EditGuildMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGuildMsgBColor = (byte)EditGuildMsgBColor.Value;
        ModValue();
    }

    public void EditGroupMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGroupMsgFColor = (byte)EditGroupMsgFColor.Value;
        ModValue();
    }

    public void EditGroupMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btGroupMsgBColor = (byte)EditGroupMsgBColor.Value;
        ModValue();
    }

    public void EditCustMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCustMsgFColor = (byte)EditCustMsgFColor.Value;
        ModValue();
    }

    public void EditCustMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btCustMsgBColor = (byte)EditCustMsgBColor.Value;
        ModValue();
    }

    public void EditNationMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btNationMsgFColor = (byte)EditNationMsgFColor.Value;
        ModValue();
    }

    public void EditNationMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btNationMsgBColor = (byte)EditNationMsgBColor.Value;
        ModValue();
    }

    public void EditUserSayMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btUserSayMsgFColor = (byte)EditUserSayMsgFColor.Value;
        ModValue();
    }

    public void EditUserSayMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btUserSayMsgBColor = (byte)EditUserSayMsgBColor.Value;
        ModValue();
    }

    public void EditTopUserSayMsgFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btTopUserSayMsgFColor = (byte)EditTopUserSayMsgFColor.Value;
        ModValue();
    }

    public void EditTopUserSayMsgBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btTopUserSayMsgBColor = (byte)EditTopUserSayMsgBColor.Value;
        ModValue();
    }

    public void EditDropItemFColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btDropItemFColor = (byte)EditDropItemFColor.Value;
        ModValue();
    }

    public void EditDropItemBColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btDropItemBColor = (byte)EditDropItemBColor.Value;
        ModValue();
    }

    public void seNPCLabelNormalColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btNPCLabelNormalColor = (byte)seNPCLabelNormalColor.Value;
        ModValue();
    }

    public void chkNPCLabelFontStrokeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNPCLabelFontStroke = chkNPCLabelFontStroke.Checked;
        ModValue();
    }

    public void seNPCLabelMouseMoveColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btNPCLabelMouseMoveColor = (byte)seNPCLabelMouseMoveColor.Value;
        ModValue();
    }

    public void seNPCLabelMouseDownColorChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btNPCLabelMouseDownColor = (byte)seNPCLabelMouseDownColor.Value;
        ModValue();
    }

    public void ButtonMsgColorSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "HearMsgFColor", M2Config.btHearMsgFColor);
        Config.WriteInteger("Setup", "HearMsgBColor", M2Config.btHearMsgBColor);
        Config.WriteInteger("Setup", "WhisperMsgFColor", M2Config.btWhisperMsgFColor);
        Config.WriteInteger("Setup", "WhisperMsgBColor", M2Config.btWhisperMsgBColor);
        Config.WriteInteger("Setup", "GMWhisperMsgFColor", M2Config.btGMWhisperMsgFColor);
        Config.WriteInteger("Setup", "GMWhisperMsgBColor", M2Config.btGMWhisperMsgBColor);
        Config.WriteInteger("Setup", "SendWhisperMsgFColor", M2Config.btSendWhisperMsgFColor);
        Config.WriteInteger("Setup", "SendWhisperMsgBColor", M2Config.btSendWhisperMsgBColor);
        Config.WriteInteger("Setup", "RefreshGameGoldFColor", M2Config.btRefreshGameGoldFColor);
        Config.WriteInteger("Setup", "RefreshGameGoldBColor", M2Config.btRefreshGameGoldBColor);
        Config.WriteInteger("Setup", "ShowWhisperFColor", M2Config.btShowWhisperFColor);
        Config.WriteInteger("Setup", "ShowWhisperBColor", M2Config.btShowWhisperBColor);
        Config.WriteInteger("Setup", "CloseWhisperFColor", M2Config.btCloseWhisperFColor);
        Config.WriteInteger("Setup", "CloseWhisperBColor", M2Config.btCloseWhisperBColor);
        Config.WriteInteger("Setup", "CryMsgFColor", M2Config.btCryMsgFColor);
        Config.WriteInteger("Setup", "CryMsgBColor", M2Config.btCryMsgBColor);
        Config.WriteInteger("Setup", "GreenMsgFColor", M2Config.btGreenMsgFColor);
        Config.WriteInteger("Setup", "GreenMsgBColor", M2Config.btGreenMsgBColor);
        Config.WriteInteger("Setup", "BlueMsgFColor", M2Config.btBlueMsgFColor);
        Config.WriteInteger("Setup", "BlueMsgBColor", M2Config.btBlueMsgBColor);
        Config.WriteInteger("Setup", "RedMsgFColor", M2Config.btRedMsgFColor);
        Config.WriteInteger("Setup", "RedMsgBColor", M2Config.btRedMsgBColor);
        Config.WriteInteger("Setup", "GuildMsgFColor", M2Config.btGuildMsgFColor);
        Config.WriteInteger("Setup", "GuildMsgBColor", M2Config.btGuildMsgBColor);
        Config.WriteInteger("Setup", "GroupMsgFColor", M2Config.btGroupMsgFColor);
        Config.WriteInteger("Setup", "GroupMsgBColor", M2Config.btGroupMsgBColor);
        Config.WriteInteger("Setup", "CustMsgFColor", M2Config.btCustMsgFColor);
        Config.WriteInteger("Setup", "CustMsgBColor", M2Config.btCustMsgBColor);
        Config.WriteInteger("Setup", "NationMsgBColor", M2Config.btNationMsgBColor); // Delphi 原文 B 色先写，保留
        Config.WriteInteger("Setup", "NationMsgFColor", M2Config.btNationMsgFColor);
        Config.WriteInteger("Setup", "UserSayMsgFColor", M2Config.btUserSayMsgFColor);
        Config.WriteInteger("Setup", "UserSayMsgBColor", M2Config.btUserSayMsgBColor);
        Config.WriteInteger("Setup", "TopUserSayMsgFColor", M2Config.btTopUserSayMsgFColor);
        Config.WriteInteger("Setup", "TopUserSayMsgBColor", M2Config.btTopUserSayMsgBColor);
        Config.WriteInteger("Setup", "DropItemFColor", M2Config.btDropItemFColor);
        Config.WriteInteger("Setup", "DropItemBColor", M2Config.btDropItemBColor);
        Config.WriteInteger("Setup", "NPCLabelNormalColor", M2Config.btNPCLabelNormalColor);
        Config.WriteBool("Setup", "NPCLabelFontStroke", M2Config.boNPCLabelFontStroke);
        Config.WriteInteger("Setup", "NPCLabelMouseMoveColor", M2Config.btNPCLabelMouseMoveColor);
        Config.WriteInteger("Setup", "NPCLabelMouseDownColor", M2Config.btNPCLabelMouseDownColor);
        uModValue();
        GameConfigState.SendServerConfig(); // Delphi 尾部无条件 SendServerConfig
    }

    // ================= HumanDie/DieDrop 页（批次J25） =================

    public void ScrollBarDieDropUseItemRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarDieDropUseItemRate.Value;
        EditDieDropUseItemRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nDieDropUseItemRate = nPostion;
        ModValue();
    }

    public void ScrollBarDieRedDropUseItemRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarDieRedDropUseItemRate.Value;
        EditDieRedDropUseItemRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nDieRedDropUseItemRate = nPostion;
        ModValue();
    }

    public void ScrollBarDieScatterBagRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarDieScatterBagRate.Value;
        EditDieScatterBagRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nDieScatterBagRate = nPostion;
        ModValue();
    }

    public void CheckBoxKillByMonstDropUseItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByMonstDropUseItem = CheckBoxKillByMonstDropUseItem.Checked;
        ModValue();
    }

    public void CheckBoxKillByHumanDropUseItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByHumanDropUseItem = CheckBoxKillByHumanDropUseItem.Checked;
        ModValue();
    }

    public void CheckBoxDieScatterBagClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDieScatterBag = CheckBoxDieScatterBag.Checked;
        ModValue();
    }

    public void CheckBoxDieDropGoldClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDieDropGold = CheckBoxDieDropGold.Checked;
        ModValue();
    }

    public void CheckBoxDieRedScatterBagAllClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDieRedScatterBagAll = CheckBoxDieRedScatterBagAll.Checked;
        ModValue();
    }

    public void chkKillByMonstDropJewelryBoxItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByMonstDropJewelryBoxItem = chkKillByMonstDropJewelryBoxItem.Checked;
        ModValue();
    }

    public void chkKillByHumanDropJewelryBoxItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByHumanDropJewelryBoxItem = chkKillByHumanDropJewelryBoxItem.Checked;
        ModValue();
    }

    public void chkKillByMonstDropGodBlessItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByMonstDropGodBlessItem = chkKillByMonstDropGodBlessItem.Checked;
        ModValue();
    }

    public void chkKillByHumanDropGodBlessItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boKillByHumanDropGodBlessItem = chkKillByHumanDropGodBlessItem.Checked;
        ModValue();
    }

    public void scrlbrJewelryBoxItemChange(object? sender)
    {
        int nPostion = (int)scrlbrJewelryBoxItem.Value;
        edtJewelryBoxItem.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nDropJewelryBoxItemRate = nPostion;
        ModValue();
    }

    public void scrlbrGodBlessItemChange(object? sender)
    {
        int nPostion = (int)scrlbrGodBlessItem.Value;
        edtGodBlessItem.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nDropGodBlessItemRate = nPostion;
        ModValue();
    }

    public void EditScatterBagItemsMinLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nScatterBagItemsMinLevel = (int)EditScatterBagItemsMinLevel.Value;
        ModValue();
    }

    public void EditDropUseItemsMaxCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nDropUseItemsMaxCount = (int)EditDropUseItemsMaxCount.Value;
        ModValue();
    }

    public void CheckBoxDropUseItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDropUseItem = CheckBoxDropUseItem.Checked;
        ModValue();
    }

    public void EditDieRedDropUseItemOneRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nDieRedDropUseItemOneRate = (int)EditDieRedDropUseItemOneRate.Value;
        ModValue();
    }

    /// <summary>ScrollBarDieDropUseItemRate0Change 泛化（Delphi FindComponent 按 Tag 0..18 定位编辑框）。</summary>
    public void DieDropUseItemRateScrollChanged(int tag)
    {
        int nPostion = (int)DieDropUseItemRateScrolls[tag].Value;
        edtDieDropUseItemRateCells[tag].Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.DieDropUseItemRates[tag] = nPostion;
        ModValue();
    }

    public void ButtonHumanDieSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "DieScatterBag", M2Config.boDieScatterBag);
        Config.WriteInteger("Setup", "DieScatterBagRate", M2Config.nDieScatterBagRate);
        Config.WriteBool("Setup", "DieRedScatterBagAll", M2Config.boDieRedScatterBagAll);
        Config.WriteInteger("Setup", "DieDropUseItemRate", M2Config.nDieDropUseItemRate);
        Config.WriteInteger("Setup", "DieRedDropUseItemRate", M2Config.nDieRedDropUseItemRate);
        Config.WriteBool("Setup", "DieDropGold", M2Config.boDieDropGold);
        Config.WriteBool("Setup", "KillByHumanDropUseItem", M2Config.boKillByHumanDropUseItem);
        Config.WriteBool("Setup", "KillByMonstDropUseItem", M2Config.boKillByMonstDropUseItem);
        Config.WriteBool("Setup", "KillByMonstDropJewelryBoxItem", M2Config.boKillByMonstDropJewelryBoxItem);
        Config.WriteBool("Setup", "KillByHumanDropJewelryBoxItem", M2Config.boKillByHumanDropJewelryBoxItem);
        Config.WriteBool("Setup", "KillByMonstDropGodBlessItem", M2Config.boKillByMonstDropGodBlessItem);
        Config.WriteBool("Setup", "KillByHumanDropGodBlessItem", M2Config.boKillByHumanDropGodBlessItem);
        Config.WriteInteger("Setup", "DieScatterBagRate", M2Config.nDieScatterBagRate); // Delphi 原文连写两行同键，保留
        Config.WriteInteger("Setup", "DieScatterBagRate", M2Config.nDieScatterBagRate);
        Config.WriteInteger("Setup", "DropJewelryBoxItemRate", M2Config.nDropJewelryBoxItemRate);
        Config.WriteInteger("Setup", "DropGodBlessItemRate", M2Config.nDropGodBlessItemRate);
        Config.WriteInteger("Setup", "DropUseItemsMaxCount", M2Config.nDropUseItemsMaxCount);
        Config.WriteInteger("Setup", "ScatterBagItemsMinLevel", M2Config.nScatterBagItemsMinLevel);
        uModValue();
    }

    public void ButtonDieDropUseItemSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "DropUseItem", M2Config.boDropUseItem);
        Config.WriteInteger("Setup", "DieRedDropUseItemOneRate", M2Config.nDieRedDropUseItemOneRate);
        for (int i = 0; i < M2Config.DieDropUseItemRates.Length; i++)
            Config.WriteInteger("Setup", "DieDropUseItemRates" + i, M2Config.DieDropUseItemRates[i]);
        uModValue();
    }

    // ================= CharStatus 页（批次J25） =================

    public void CheckBoxParalyCanRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boParalyCanRun = CheckBoxParalyCanRun.Checked;
        ModValue();
        GameConfigState.SendServerConfig(); // Delphi 原文每个弯腰开关后无条件 SendServerConfig
    }

    public void CheckBoxParalyCanWalkClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boParalyCanWalk = CheckBoxParalyCanWalk.Checked;
        ModValue();
        GameConfigState.SendServerConfig();
    }

    public void CheckBoxParalyCanHitClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boParalyCanHit = CheckBoxParalyCanHit.Checked;
        ModValue();
        GameConfigState.SendServerConfig();
    }

    public void CheckBoxParalyCanSpellClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boParalyCanSpell = CheckBoxParalyCanSpell.Checked;
        ModValue();
        GameConfigState.SendServerConfig();
    }

    /// <summary>CheckGroupAttatckModeChange 1:1（关闭最后一种攻击模式时弹窗回勾并 Exit）。</summary>
    public void CheckGroupAttatckModeChange(int index, bool newState)
    {
        if (!boOpened)
            return;
        if (!newState)
        {
            M2Config.AttatckModes[index] = false;
            bool boFindOK = false;
            for (int i = 0; i < M2Config.AttatckModes.Length; i++)
            {
                if (M2Config.AttatckModes[i])
                {
                    boFindOK = true;
                    break;
                }
            }
            if (!boFindOK)
            {
                M2Forms.MessageBox("最少要选择一种攻击模式", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
                CheckGroupAttatckModeItems[index].Checked = true;
                M2Config.AttatckModes[index] = true;
                return;
            }
        }
        else
        {
            M2Config.AttatckModes[index] = true;
        }
        ModValue();
    }

    public void ButtonCharStatusSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "ParalyCanRun", M2Config.boParalyCanRun);
        Config.WriteBool("Setup", "ParalyCanWalk", M2Config.boParalyCanWalk);
        Config.WriteBool("Setup", "ParalyCanHit", M2Config.boParalyCanHit);
        Config.WriteBool("Setup", "ParalyCanSpell", M2Config.boParalyCanSpell);
        for (int i = 0; i < M2Config.AttatckModes.Length; i++)
            Config.WriteBool("Setup", "AttatckModes" + i, M2Config.AttatckModes[i]);
        uModValue();
    }
}
