program M2Server;

uses
  Forms,
  Windows,
  SysUtils,
  Graphics,
  SyncObjs,
  UsrEngn in 'UsrEngn.pas',
  ObjNpc in 'ObjNpc.pas',
  ObjMon2 in 'ObjMon2.pas',
  ObjMon in 'ObjMon.pas',
  ObjGuard in 'ObjGuard.pas',
  ObjBase in 'ObjBase.pas',
  ObjAxeMon in 'ObjAxeMon.pas',
  NoticeM in 'NoticeM.pas',
  Magic in 'Magic.pas',
  M2Share in 'M2Share.pas',
  ItmUnit in 'ItmUnit.pas',
  GameEvent in 'GameEvent.pas',
  Envir in 'Envir.pas',
  Castle in 'Castle.pas',
  RunSock in 'RunSock.pas',
  ObjRobot in 'ObjRobot.pas',
  Common in '..\Common\Common.pas',
  SDK in '..\Common\SDK.pas',
  Grobal2 in '..\Common\Grobal2.pas',
  HUtil32 in '..\Common\HUtil32.pas',
  EncryptUnit in '..\Common\EncryptUnit.pas',
  ObjGame in 'ObjGame.pas',
  DataEngn in 'DataEngn.pas',
  EDcode in '..\Common\EDcode.pas',
  SndaShop in 'SndaShop.pas',
  Boxs in 'Boxs.pas',
  ObjHero in 'ObjHero.pas',
  PathFind in 'PathFind.pas',
  FilterTexts in 'FilterTexts.pas',
  ItemRules in 'ItemRules.pas',
  UserCmds in 'UserCmds.pas',
  GroupItems in 'GroupItems.pas',
  GameGoldDealDB in 'GameGoldDealDB.pas',
  MudUtil in '..\Common\MudUtil.pas',
  ItemEvent in 'ItemEvent.pas',
  ItemEffects in 'ItemEffects.pas',
  CheckUnit in '..\..\Common\CheckUnit.pas',
  Guild in 'Guild.pas',
  UnitPath in '..\Common\UnitPath.pas',
  MD5Util in '..\Common\MD5Util.pas',
  MemoryStreamEx in '..\..\Common\MemoryStreamEx.pas',
  HandleNpcCmds in 'HandleNpcCmds.pas',
  HandleCommands in 'HandleCommands.pas',
  NpcConditionCmd in 'NpcConditionCmd.pas',
  NpcActionCmd in 'NpcActionCmd.pas',
  HashList in '..\Common\HashList.pas',
  HardInfo in '..\Common\HardInfo.pas',
  Nations in 'Nations.pas',
  EncryptUnit_LF in '..\Common\EncryptUnit_LF.pas',
  ObjSmartMon in 'ObjSmartMon.pas',
  ObjFireDragon in 'ObjFireDragon.pas',
  uCustomMonsterUtils in 'uCustomMonsterUtils.pas',
  ObjCustomMon in 'ObjCustomMon.pas',
  uCustomMagicUtils in 'uCustomMagicUtils.pas',
  ObjDummy in 'ObjDummy.pas',
  ObjPlayer in 'ObjPlayer.pas',
  uCustomNpcUtils in 'uCustomNpcUtils.pas',
  uMagicACUtils in 'uMagicACUtils.pas',
  M2Threads in 'M2Threads.pas',
  M2Locker in 'M2Locker.pas',
  VerifyCodeUtils in 'VerifyCodeUtils.pas',
  SafeAreaManager in 'SafeAreaManager.pas',
  uAliyunSendSMSThread in 'uAliyunSendSMSThread.pas',
  ItemDropLimit in 'ItemDropLimit.pas',
  NpcCommon in 'NpcCommon.pas',
  uCustomHeroMagic in 'uCustomHeroMagic.pas',
  PluginImplement in 'PluginImplement.pas',
  PluginInterface in 'PluginInterface.pas',
  PluginManager in 'PluginManager.pas',
  M2DataCommon in 'M2DataCommon.pas',
  MySqlAuctionDB in 'MySqlAuctionDB.pas',
  MySqlStorageDB in 'MySqlStorageDB.pas',
  MySqlUserShopDB in 'MySqlUserShopDB.pas',
  MySqlM2DataDB in 'MySqlM2DataDB.pas',
  SqliteAuctionDB in 'SqliteAuctionDB.pas',
  SqliteM2DataDB in 'SqliteM2DataDB.pas',
  SqliteStorageDB in 'SqliteStorageDB.pas',
  SqliteUserShopDB in 'SqliteUserShopDB.pas',
  M2Definition in 'M2Definition.pas',
  uCombatPowerUtils in 'uCombatPowerUtils.pas',
  SellPlayer in 'SellPlayer.pas',
  ClientPickItemsCfg in 'ClientPickItemsCfg.pas',
  StruckDamageAbsorbUtils in 'StruckDamageAbsorbUtils.pas',
  StringListHelper in 'StringListHelper.pas',
  EncodingHelper in '..\..\Common\EncodingHelper.pas',
  HTTPService in 'HTTPService.pas',
  MemoryModuleEx in 'MemoryModuleEx.pas',
  DesUtils in 'DesUtils.pas',
  LocalDB in 'LocalDB.pas',
  PowerBase64 in 'PowerBase64.pas',
  IdSrvClient in 'Forms\IdSrvClient.pas' {FrmIDSoc},
  svMain in 'Forms\svMain.pas' {frmMain},
  ActionSpeedConfig in 'Forms\ActionSpeedConfig.pas' {frmActionSpeed},
  AttackSabukWallConfig in 'Forms\AttackSabukWallConfig.pas' {FrmAttackSabukWall},
  CastleManage in 'Forms\CastleManage.pas' {frmCastleManage},
  ClientModules in 'Forms\ClientModules.pas' {ftmClientModules},
  ConfigClient in 'Forms\ConfigClient.pas' {FrmConfigClient},
  ConfigMerchant in 'Forms\ConfigMerchant.pas' {frmConfigMerchant},
  ConfigMissionNpcPage in 'Forms\ConfigMissionNpcPage.pas' {FrmMissionNpcPageEditDlg},
  ConfigMonGen in 'Forms\ConfigMonGen.pas' {frmConfigMonGen},
  FSrvValue in 'Forms\FSrvValue.pas' {FrmServerValue},
  FunctionConfig in 'Forms\FunctionConfig.pas' {frmFunctionConfig},
  GameCommand in 'Forms\GameCommand.pas' {frmGameCmd},
  GameConfig in 'Forms\GameConfig.pas' {frmGameConfig},
  GeneralConfig in 'Forms\GeneralConfig.pas' {frmGeneralConfig},
  GroupItemSkillPowerConfig in 'Forms\GroupItemSkillPowerConfig.pas' {FrmGroupItemSkillPower},
  HumanInfo in 'Forms\HumanInfo.pas' {frmHumanInfo},
  ItemSet in 'Forms\ItemSet.pas' {frmItemSet},
  MonsterConfig in 'Forms\MonsterConfig.pas' {frmMonsterConfig},
  OnlineMsg in 'Forms\OnlineMsg.pas' {frmOnlineMsg},
  uFrmClientPlugManager in 'Forms\uFrmClientPlugManager.pas' {FrmClientPlugManager},
  uFrmCombatPowerAddVar in 'Forms\uFrmCombatPowerAddVar.pas' {FrmCombatPowerAddVar},
  uFrmCombatPowerSetting in 'Forms\uFrmCombatPowerSetting.pas' {FrmCombatPowerSetting},
  uFrmCustomItemProperty in 'Forms\uFrmCustomItemProperty.pas' {FrmCustomItemProperty},
  uFrmCustomMagic in 'Forms\uFrmCustomMagic.pas' {FrmCustomMagic},
  uFrmCustomMagicCopySetting in 'Forms\uFrmCustomMagicCopySetting.pas' {FrmCustomMagicCopySetting},
  uFrmCustomMoney in 'Forms\uFrmCustomMoney.pas' {FrmCustomMoney},
  uFrmCustomNpc in 'Forms\uFrmCustomNpc.pas' {FrmCustomNpc},
  uFrmDummySetting in 'Forms\uFrmDummySetting.pas' {FrmDummySetting},
  uFrmGlobalVarEdit in 'Forms\uFrmGlobalVarEdit.pas' {FrmGlobalVarEdit},
  uFrmHeroMagicCondition in 'Forms\uFrmHeroMagicCondition.pas' {FrmHeroMagicCondition},
  uFrmHeroMagicSetting in 'Forms\uFrmHeroMagicSetting.pas' {FrmHeroMagicSetting},
  uFrmItemDropLog in 'Forms\uFrmItemDropLog.pas' {FrmItemDropLog},
  uFrmMainGamePets in 'Forms\uFrmMainGamePets.pas' {FrmGamePets},
  uFrmPlugManager in 'Forms\uFrmPlugManager.pas' {FrmPlugManager},
  uFrmStorageItemsView in 'Forms\uFrmStorageItemsView.pas' {FrmStorageItemsView},
  uFrmUserShopGetMoneyTotal in 'Forms\uFrmUserShopGetMoneyTotal.pas' {FrmUserShopGetMoneyTotal},
  uFrmUserShopView in 'Forms\uFrmUserShopView.pas' {FrmUserShopView},
  ViewKernelInfo in 'Forms\ViewKernelInfo.pas' {frmViewKernelInfo},
  ViewLevel in 'Forms\ViewLevel.pas' {frmViewLevel},
  ViewList in 'Forms\ViewList.pas' {frmViewList},
  ViewList2 in 'Forms\ViewList2.pas' {FrmViewList2},
  ViewOnlineHuman in 'Forms\ViewOnlineHuman.pas' {frmViewOnlineHuman},
  ViewSession in 'Forms\ViewSession.pas' {frmViewSession},
  uSynHighlighterSample in 'uSynHighlighterSample.pas',
  fTxtEditor in 'Forms\fTxtEditor.pas' {frmTXTEditor},
  dlgConfirmReplace in 'Forms\dlgConfirmReplace.pas' {ConfirmReplaceDialog},
  dlgSearchText in 'Forms\dlgSearchText.pas' {TextSearchDialog},
  plgSearchHighlighter in 'plgSearchHighlighter.pas',
  dlgReplaceText in 'Forms\dlgReplaceText.pas' {TextReplaceDialog};

{$R *.res}

{$IF CompilerVersion >= 21.0}
{$WEAKLINKRTTI ON}
{$RTTI EXPLICIT METHODS([]) PROPERTIES([]) FIELDS([])}
{$Include FastMM4Options.inc}
{$IFEND}

{$IFDEF CPUX64}
  {$R DllRes\DllRes64.res}
{$ELSE}
  {$R DllRes\DllRes32.res}
{$ENDIF}

{$R DB\M2Data.RES}

{$IFNDEF CPUX64}
const
  IMAGE_FILE_LARGE_ADDRESS_AWARE = $0020;
  // 突破2G内存限制 chongchong 2017-03-31
  {$SetPEFlags IMAGE_FILE_LARGE_ADDRESS_AWARE}
{$ENDIF}

(*
var
  //保存原API函数地址
  Kernel_GetTickCount: function(): Cardinal; stdcall;

  //自定义api函数
function MyGetTickCount(): Cardinal; stdcall;
begin
  Result := 2419200000  + Kernel_GetTickCount;
end;
*)

{$IFNDEF CPUX64}

procedure Start();
begin
  { TODO -ochongchong -c内存泄露 : 去++++内存泄露【2013-07-18】 }
  // TCriticalSection的内存泄露暂时找不出来，先注册掉
{$IFDEF EnableMemoryLeakReporting}
  //RegisterExpectedMemoryLeak(TCriticalSection, 1);
{$ENDIF}

  //TryHookProcedureEx(kernel32, 'GetTickCount', @MyGetTickCount, @Kernel_GetTickCount);
  {$IF CompilerVersion >= 22}
  FormatSettings.{$IFEND}DateSeparator := '-';
  Application.Initialize;
  Application.HintPause := 0;
  Application.HintShortPause := 0;
  Application.HintHidePause := 5000;
  asm
        jz      @@Start
        jnz     @@Start
        db      0EBh

@@Start:
  end;
  Application.CreateForm(TfrmMain, frmMain);
  Application.CreateForm(TFrmIDSoc, FrmIDSoc);
  Application.CreateForm(TConfirmReplaceDialog, ConfirmReplaceDialog);
  Application.Run;
end;
{$ENDIF}

// 上面汇编等效代码 -- piaoyun 2013-07-17

begin
  Application.Initialize;
  Application.HintPause := 100;
  Application.HintShortPause := 100;
  Application.HintHidePause := 5000;

  Application.CreateForm(TFrmMain, FrmMain);
  Application.CreateForm(TFrmIDSoc, FrmIDSoc);
  Application.Run;
end.

