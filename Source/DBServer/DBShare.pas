unit DBShare;

interface
uses
  Windows, Messages, Classes, SysUtils, StrUtils, JSocket, IniFiles, Controls,
  Grobal2, Common, MudUtil, SDK, PlugIn, uRunGateList, RoleDB;

resourcestring
  g_sUpDateTime = '更新日期: 2025/04/28';
  g_sProductName = 'GxxM2数据库服务器 1.0';
  g_sProgram = '程序制作: GxxM2';
  g_sWebSite = '程序网站: http://www.gxxm2.com';
  g_sProgramName = 'DBServer';

const
  TextChars = [#32..#255];
  RUNGATEMAXSESSION = 100;

  DBSUSETHREAD = 0;

  MIN_CHAR_NAME_LEN = 4;
  MAX_CHAR_NAME_LEN = 14;


type
  TRouteInfo = record
    nGateCount: Integer;
    sSelGateIP: string[15];
    sGameGateIP: array[0..7] of string[15];
    nGameGatePort: array[0..7] of Integer;
    nGameGateDBPort: array[0..7] of Integer;
    dwGameGateConnectTick: array[0..7] of LongWord;

    // 主网关有几个不能连接时，分配备用网关 chongchong 2015-07-24
    GameGateDisconnectCount: Integer;
    EnabledRunGate2List: Boolean;
    RunGate2List: TRunGateList;
  end;
  pTRouteInfo = ^TRouteInfo;

  TModuleInfo = record
    Module: TObject;
    ModuleName: string;
    Address: string;
    Buffer: string;
  end;
  pTModuleInfo = ^TModuleInfo;

  TMagicDB = record
    wMagicID: Word;
    sName: string;
    MagicAttr: TMagicAttr;
  end;
  pTMagicDB = ^TMagicDB;

  PSessionRunGateInfo = ^TSessionRunGateInfo;
  TSessionRunGateInfo = record
    Socket: TCustomWinSocket;
    dwSendTick: LongWord;
    dwReceiveTick: LongWord;                                                                                                      
    nSckHandle: Integer;
    sRemoteAddr: string;
    nRemotePort: Integer;

    sRecvText: string;
  end;

procedure LoadConfig_DataSaveDB;
procedure LoadConfig();
procedure LoadIPTable();
procedure LoadGateID();
procedure SaveServerInfo();
procedure LoadServerInfo();
function LoadChrNameList(sFileName: string): Boolean;

function CheckServerIP(sIP: string): Boolean;
function GetGateID(sIPaddr: string): Integer;
function GetCodeMsgSize(X: Double): Integer;
function CheckChrName(sChrName: string): Boolean;
function CheckSpecialChar(sChrName: WideString): Boolean;

procedure MainOutMessage(sMsg: string);
procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
function CheckDenyChrName(sChrName: string): Boolean;
function CheckNumberName(sChrName: string): Boolean;
function CheckLetterName(sChrName: string): Boolean;
function CheckFilterRankingChrName(sChrName: string): Boolean;
function CheckFilterNewHumanChrName(sChrName: string): Boolean;

function GetRankingList(nTablePage, nPageType: Integer): TRoleRankList;

function GetMapIndex(sMap: string): Integer;
function GateRouteIP(sGateIP: string; var nPort: Integer): string;

function GateActiveRouteIP(sGateIP: string; var nPort: Integer): string;
function CheckActiveRunGate(sGateIP: string; nPort: Integer): Boolean;

function AddModule(ModuleInfo: pTModuleInfo): pTModuleInfo;
procedure RemoveModule(Module: TObject);
procedure UpdateModule(ModuleInfo: pTModuleInfo);

procedure UnLoadMagicList;
procedure UnLoadStdItemList;
function GetMagicName(wMagicId: Word; MagicAttr: TMagicAttr): string;
function GetStdItemName(nPosition: Integer): string;
function GetMagicTypeString(MagicAttr: TMagicAttr): string;

function Date2MyDate(Dt: TDateTime): Integer;
function MyDate2Date(dt: Integer): TDateTime;

var
  g_sFilePath: string;
  g_PlugInManage: TPlugInManage;

  g_RoleDB: TRoleDB;

  g_nVersion: Integer = 0;
  g_nHumDataSize: Integer = 0;

  g_sDataDBFilePath: string = '.\FDB\';
  g_sBackupPath: string = '.\FDB\';
  g_sLogPath: string = '.\Log\';

  g_nServerPort: Integer = 6000;
  g_sServerAddr: string = '0.0.0.0';
  g_nGatePort: Integer = 5100;
  g_sGateAddr: string = '0.0.0.0';
  g_nIDServerPort: Integer = 5600;
  g_sIDServerAddr: string = '127.0.0.1';

  g_sServerName: string = 'GeeM2';
  g_sConfFileName: string = '.\Dbsrc.ini';
  g_sGateConfFileName: string = '.\!ServerInfo.txt';

  g_sGateListFileName: string = '.\!GateList.ini';

  g_sServerIPConfFileNmae: string = '.\!AddrTable.txt';
  g_sGateIDConfFileName: string = '.\SelectID.txt';

  g_boStartService: Boolean = False;
  g_boRemoteClose: Boolean = False;
  g_boSoftClose: Boolean = False;

  g_sMapFile: string;
  g_DenyChrNameList: TStringList;
  g_ServerIPList: TStringList;
  g_GateIDList: TStringList;
  g_MapList: TStringList;

  g_RouteInfo: array[0..19] of TRouteInfo;

  g_boDynamicIPMode: Boolean = False;

  g_ModuleList: TSortStringList;
  g_dwShowModuleTick: LongWord;

  g_HumanRankList: TRoleRankList;
  g_WarriorRankList: TRoleRankList;
  g_WizardRankList: TRoleRankList;
  g_TaoistRankList: TRoleRankList;
  g_MasterRankList: TRoleRankList;


  g_HeroRankList: TRoleRankList;
  g_HeroWarriorRankList: TRoleRankList;
  g_HeroWizardRankList: TRoleRankList;
  g_HeroTaoistRankList: TRoleRankList;


  g_MainLogMsgList: TGStringList;
  g_boShowLogMsg: Boolean = True;
  g_dwShowMainLogTick: LongWord;

  g_boShowQuryChrLog: Boolean = False;

  g_nRankingMinLevel: Integer = 20;
  g_nRankingMaxLevel: Integer = 500;
  g_nRankingCount: Integer = 100;

  g_boAutoRefRanking: Boolean = True;
  g_nAutoRefRankingType: Integer = 0;
  g_dwAutoRefRankingTick: LongWord;

  g_nRefRankingHour1: Integer = 0;
  g_nRefRankingHour2: Integer = 0;

  g_nRefRankingMinute1: Integer = 5;
  g_nRefRankingMinute2: Integer = 5;
  g_TodayDate: TDate = 0;

  g_RefDate: TDate = 0;

  g_MagicList: TStringList;
  g_StdItemList: TStringList;

  g_dwGameCenterHandle: THandle;
  g_sNowStartServer: string = '正在启动数据库服务器...';
  g_sNowStartServerOK: string = '数据库服务器启动完成...';

  g_nWorkStatus: Integer = 0;
  g_dwWorkStatusTick: LongWord;

  g_nCreateHumCount: Integer = 0;
  g_nDeleteHumCount: Integer = 0;
  g_nLoadHumCount: Integer = 0;
  g_nSaveHumCount: Integer = 0;
  g_nCreateHeroCount: Integer = 0;
  g_nDeleteHeroCount: Integer = 0;
  g_nLoadHeroCount: Integer = 0;
  g_nSaveHeroCount: Integer = 0;

  g_Ranking_CS: TRTLCriticalSection;
  g_boRefRanking: Boolean = False;
  g_boCanRanking: Boolean = True;
  g_RefRankingTick: LongWord = 0;

  g_boCanCreateHuman: Boolean = True;                                                               //允许建立新人物
  g_boCanDeleteHuman: Boolean = True;                                                               //允许删除人物
  g_boCanGetBackDeleteHuman: Boolean = True;                                                        //允许找回删除的人物
  g_nCanDeleteHumanLowLevel: Integer = 45;                                                          //以上级别不允许被删除

  g_boForbidNumberName: Boolean = False;                                                            //禁止建立包含数字的人物名
  g_boForbidLetterName: Boolean = False;                                                            //禁止建立全英文人物名

  g_boDenyChrName: Boolean = False;                                                                 //允许特殊字符创建人物

  g_boUseActiveRunGage: Boolean = False;
  g_boShowBlockIPLog: Boolean = False;

  g_nCreateChrNameCount: Integer = 20;

  g_boSqliteFastSave: Boolean = False;

  g_nDataSaveDBType: Integer = 0;
  g_sDataSaveDBServer: string = '';
  g_wDataSaveDBPort: Word = 3306;
  g_sDataSaveDBUser: string = '';
  g_sDataSaveDBPassword: string = '';
  g_sDataSaveDataBase: string = '';

  g_FilterNewHumanNameTextList: TStringList;
  g_FilterRankingNameTextList: TStringList;

  g_FirstName: TStringList;
  g_LastName: TStringList;

  SessionRunGateArray: array[0..RUNGATEMAXSESSION - 1] of TSessionRunGateInfo;

implementation
uses HUtil32;

procedure UnLoadMagicList;
var
  I: Integer;
begin
  for I := 0 to g_MagicList.Count - 1 do
  begin
    Dispose(pTMagicDB(g_MagicList.Objects[I]));
  end;
  g_MagicList.Clear;
end;

procedure UnLoadStdItemList;
begin
  g_StdItemList.Clear;
end;

function GetStdItemName(nPosition: Integer): string;
begin
  Result := '';
  if (nPosition - 1 >= 0) and (nPosition < g_StdItemList.Count) then
  begin
    Result := g_StdItemList[nPosition - 1];
    {StdItem := g_StdItemList.Items[nPosition - 1];
    if StdItem <> nil then begin
      Result := StdItem.Name;
    end;}
  end;
end;

function GetMagicTypeString(MagicAttr: TMagicAttr): string;
begin
  case MagicAttr of
    mtHum: Result := '人物技能';
    mtHero: Result := '英雄技能';
    mtContinuous: Result := '连击技能';
    mtDefense, mtAttack: Result := '内功技能';
  end;
end;

function GetMagicName(wMagicId: Word; MagicAttr: TMagicAttr): string;
var
  I: Integer;
  MagicDB: pTMagicDB;
begin
  Result := '';
  for I := 0 to g_MagicList.Count - 1 do
  begin
    MagicDB := pTMagicDB(g_MagicList.Objects[I]);
    if (MagicDB.wMagicID = wMagicId) then
    begin
      if MagicAttr <> MagicDB.MagicAttr then Continue;
      Result := MagicDB.sName;
      break;
    end;
  end;
end;

function AddModule(ModuleInfo: pTModuleInfo): pTModuleInfo;
var
  I: Integer;
  Module: pTModuleInfo;
begin
  //Result := nil;
  for I := 0 to g_ModuleList.Count - 1 do begin
    if pTModuleInfo(g_ModuleList.Objects[I]).Module = ModuleInfo.Module then begin
      pTModuleInfo(g_ModuleList.Objects[I])^ := ModuleInfo^;
      Result := pTModuleInfo(g_ModuleList.Objects[I]);
      Exit;
    end;
  end;
  New(Module);
  Module^ := ModuleInfo^;
  g_ModuleList.AddObject(Module.ModuleName, TObject(Module));
  Result := Module;
end;

procedure RemoveModule(Module: TObject);
var
  I: Integer;
begin
  for I := 0 to g_ModuleList.Count - 1 do
  begin
    if pTModuleInfo(g_ModuleList.Objects[I]).Module = Module then
    begin
      Dispose(pTModuleInfo(g_ModuleList.Objects[I]));
      g_ModuleList.Delete(I);
      Break;
    end;
  end;
end;

procedure UpdateModule(ModuleInfo: pTModuleInfo);
var
  I: Integer;
  Module: pTModuleInfo;
begin
  for I := 0 to g_ModuleList.Count - 1 do
  begin
    if pTModuleInfo(g_ModuleList.Objects[I]).Module = ModuleInfo.Module then
    begin
      pTModuleInfo(g_ModuleList.Objects[I])^ := ModuleInfo^;
      Exit;
    end;
  end;
  New(Module);
  Module^ := ModuleInfo^;
  g_ModuleList.AddObject(Module.ModuleName, TObject(Module));
end;


function GetRankingList(nTablePage, nPageType: Integer): TRoleRankList;
begin
  Result := nil;
  case nTablePage of
    0:
      begin
        case nPageType of
          0: Result := g_HumanRankList;
          1: Result := g_WarriorRankList;
          2: Result := g_WizardRankList;
          3: Result := g_TaoistRankList;
        end;
      end;
    1:
      begin
        case nPageType of
          0: Result := g_HeroRankList;
          1: Result := g_HeroWarriorRankList;
          2: Result := g_HeroWizardRankList;
          3: Result := g_HeroTaoistRankList;
        end;
      end;
    2: Result := g_MasterRankList;
  end;
end;

function CheckServerIP(sIP: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to g_ServerIPList.Count - 1 do
  begin
    if CompareText(sIP, g_ServerIPList.Strings[I]) = 0 then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function LoadChrNameList(sFileName: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  if FileExists(sFileName) then
  begin
    g_DenyChrNameList.Clear;
    g_DenyChrNameList.LoadFromFile(sFileName);
    I := 0;
    while (True) do
    begin
      if g_DenyChrNameList.Count <= I then Break;
      if Trim(g_DenyChrNameList.Strings[I]) = '' then
      begin
        g_DenyChrNameList.Delete(I);
        Continue;
      end;
      Inc(I);
    end;
    Result := True;
  end;
end;

procedure SaveServerInfo();
var
  I, J: Integer;
  LoadList: TStringList;
  Conf: TIniFile;

  S: string;
  RunGateList: TRunGateList;
begin
  LoadList := TStringList.Create;
  try
    for I := Low(g_RouteInfo) to High(g_RouteInfo) do
    begin
      if g_RouteInfo[I].nGateCount > 0 then
      begin
        S := g_RouteInfo[I].sSelGateIP + #9;

        for J := 0 to g_RouteInfo[I].nGateCount - 1 do
        begin
          S := S + g_RouteInfo[I].sGameGateIP[J] + #9 + IntToStr(g_RouteInfo[I].nGameGatePort[J]) + #9;
        end;

        LoadList.Add(S);
      end;
      LoadList.SaveToFile(g_sGateConfFileName);
    end;
  finally
    LoadList.Free;
  end;


  Conf := TIniFile.Create(g_sGateListFileName);
  try
    for I := Low(g_RouteInfo) to High(g_RouteInfo) do
    begin
      Conf.EraseSection('GateDBPort' + IntToStr(I));
      
      if g_RouteInfo[I].nGateCount = 0 then Continue;

      for J := 0 to g_RouteInfo[I].nGateCount - 1 do
      begin
        Conf.WriteInteger('GateDBPort' + IntToStr(I), IntToStr(J + 1), g_RouteInfo[I].nGameGateDBPort[J]);
      end;
      
      Conf.WriteBool('setup', 'enable' + IntToStr(I), g_RouteInfo[I].EnabledRunGate2List);
      Conf.WriteInteger('setup', 'count' + IntToStr(I), g_RouteInfo[I].GameGateDisconnectCount);

      Conf.EraseSection('list' + IntToStr(I));
      RunGateList := g_RouteInfo[I].RunGate2List;
      for J := 0 to RunGateList.Count - 1 do
      begin
        Conf.WriteString('list' + IntToStr(I), IntToStr(J),
          IntToStr(Integer(RunGateList.Items[J].Enabled)) + #9 +
          RunGateList.Items[J].IP + #9 +
          IntToStr(RunGateList.items[J].Port) + #9 +
          IntToStr(RunGateList.items[J].Level) + #9 +
          IntToStr(RunGateList.items[J].DBPort)
          );
      end;
    end;
  finally
    Conf.Free;
  end;
end;

procedure LoadServerInfo();
var
  I, J: Integer;
  LoadList: TStringList;
  nRouteIdx, nGateIdx, nServerIndex: Integer;
  sLineText, sSelGateIPaddr, sGameGateIPaddr, sGameGate, sGameGatePort, sMapName, sMapInfo, sServerIndex: string;
  Conf: TIniFile;

  S1, S2, S3, S4, S5: string;
  nPort: Integer;
  RunGateList: TRunGateList;
begin
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    g_RouteInfo[I].nGateCount := 0;
    g_RouteInfo[I].sSelGateIP := '';
    FillChar(g_RouteInfo[I].sGameGateIP, SizeOf(g_RouteInfo[I].sGameGateIP), #0);
    FillChar(g_RouteInfo[I].nGameGatePort, SizeOf(g_RouteInfo[I].nGameGatePort), #0);
    FillChar(g_RouteInfo[I].nGameGateDBPort, SizeOf(g_RouteInfo[I].nGameGateDBPort), #0);
    FillChar(g_RouteInfo[I].dwGameGateConnectTick, SizeOf(g_RouteInfo[I].dwGameGateConnectTick), #0);

    g_RouteInfo[I].GameGateDisconnectCount := 1;
    g_RouteInfo[I].EnabledRunGate2List := False;
    g_RouteInfo[I].RunGate2List.Clear;
  end;

  if not FileExists(g_sGateConfFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.Add('127.0.0.1 127.0.0.1 7200');
    try
      LoadList.SaveToFile(g_sGateConfFileName);
    except
    end;
    LoadList.Free;
  end;

  if FileExists(g_sGateConfFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(g_sGateConfFileName);
    except
    end;
    nRouteIdx := 0;
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sGameGate := GetValidStr3(sLineText, sSelGateIPaddr, [' ', #9]);
        if (sGameGate = '') or (sSelGateIPaddr = '') then Continue;
        g_RouteInfo[nRouteIdx].sSelGateIP := Trim(sSelGateIPaddr);
        g_RouteInfo[nRouteIdx].nGateCount := 0;
        nGateIdx := 0;
        while (sGameGate <> '') do
        begin
          sGameGate := GetValidStr3(sGameGate, sGameGateIPaddr, [' ', #9]);
          sGameGate := GetValidStr3(sGameGate, sGameGatePort, [' ', #9]);
          g_RouteInfo[nRouteIdx].sGameGateIP[nGateIdx] := Trim(sGameGateIPaddr);
          g_RouteInfo[nRouteIdx].nGameGatePort[nGateIdx] := StrToIntDef(sGameGatePort, 0);
          Inc(nGateIdx);
        end;
        g_RouteInfo[nRouteIdx].nGateCount := nGateIdx;
        Inc(nRouteIdx);
      end;
    end;
    LoadList.Free;
  end;

  if FileExists(g_sGateListFileName) then
  begin
    LoadList := TStringList.Create;
    try
      Conf := TIniFile.Create(g_sGateListFileName);
      try
        for I := Low(g_RouteInfo) to High(g_RouteInfo) do
        begin
          for J := 0 to g_RouteInfo[I].nGateCount - 1 do
          begin
            g_RouteInfo[I].nGameGateDBPort[J] := Conf.ReadInteger('GateDBPort' + IntToStr(I), IntToStr(J + 1), 0);
          end;

          RunGateList := g_RouteInfo[I].RunGate2List;
          g_RouteInfo[I].EnabledRunGate2List := Conf.ReadBool('setup', 'enable' + IntToStr(I), False);
          g_RouteInfo[I].GameGateDisconnectCount := Conf.ReadInteger('setup', 'count' + IntToStr(I), 1);
          if (g_RouteInfo[I].GameGateDisconnectCount < 1) or (g_RouteInfo[I].GameGateDisconnectCount > 8) then
            g_RouteInfo[I].GameGateDisconnectCount := 1;
              
          Conf.ReadSectionValues('list' + IntToStr(I), LoadList);

          for J := 0 to LoadList.Count - 1 do
          begin
            sLineText := LoadList.Strings[J];
            if (Length(sLineText) = 0) or (sLineText[1] = ';') then Continue;

            sLineText := LoadList.ValueFromIndex[J];

            sLineText := GetValidStr3(sLineText, S1, [' ', #9]);
            sLineText := GetValidStr3(sLineText, S2, [' ', #9]);
            sLineText := GetValidStr3(sLineText, S3, [' ', #9]);
            sLineText := GetValidStr3(sLineText, S4, [' ', #9]);
            sLineText := GetValidStr3(sLineText, S5, [' ', #9]);

            nPort := StrToIntDef(S3, 0);
            if IsIPaddr(S2) and (nPort > 0) and (nPort <= 65535) then
            begin
              RunGateList.Add(StrToIntDef(S1, 0) > 0, S2, nPort, StrToIntDef(S5, 0), StrToIntDef(S4, 0));
            end;
          end;

          RunGateList.DoSort;
        end;
      finally
        Conf.Free;
      end;
    finally
      LoadList.Free;
    end;
  end;

  Conf := TIniFile.Create(g_sConfFileName);
  g_sMapFile := Conf.ReadString('Setup', 'MapFile', g_sMapFile);
  Conf.Free;

  g_MapList.Clear;
  if FileExists(g_sMapFile) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(g_sMapFile);
    except
    end;
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText <> '') and (sLineText[1] = '[') then
      begin
        sLineText := ArrestStringEx(sLineText, '[', ']', sMapName);
        sMapInfo := GetValidStr3(sMapName, sMapName, [#32, #9]);
        sServerIndex := Trim(GetValidStr3(sMapInfo, sMapInfo, [#32, #9]));
        nServerIndex := StrToIntDef(sServerIndex, 0);
        g_MapList.AddObject(sMapName, TObject(nServerIndex));
      end;
    end;
    LoadList.Free;
  end;
end;

procedure LoadGateID();
var
  I: Integer;
  LoadList: TStringList;
  sLineText: string;
  sID: string;
  sIPaddr: string;
  nID: Integer;
begin
  g_GateIDList.Clear;
  if FileExists(g_sGateIDConfFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(g_sGateIDConfFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText = '') or (sLineText[1] = ';') then Continue;
      sLineText := GetValidStr3(sLineText, sID, [' ', #9]);
      sLineText := GetValidStr3(sLineText, sIPaddr, [' ', #9]);
      nID := StrToIntDef(sID, -1);
      if nID < 0 then Continue;
      g_GateIDList.AddObject(sIPaddr, TObject(nID))
    end;
    LoadList.Free;
  end;
end;

function GetGateID(sIPaddr: string): Integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 0 to g_GateIDList.Count - 1 do
  begin
    if g_GateIDList.Strings[I] = sIPaddr then
    begin
      Result := Integer(g_GateIDList.Objects[I]);
      Break;
    end;
  end;
end;

procedure LoadIPTable();
begin
  if not FileExists(g_sServerIPConfFileNmae) then
  begin
    g_ServerIPList.Add('127.0.0.1');
    try
      g_ServerIPList.SaveToFile(g_sServerIPConfFileNmae);
    except
    end;
  end
  else
  begin
    g_ServerIPList.Clear;
    try
      g_ServerIPList.LoadFromFile(g_sServerIPConfFileNmae);
    except
      MainOutMessage('加载IP列表文件 ' + g_sServerIPConfFileNmae + ' 出错！！！');
    end;
  end;
end;

function GateRouteIP(sGateIP: string; var nPort: Integer): string;
  function GetRoute(RouteInfo: pTRouteInfo; var nGatePort: Integer): string;
  var
    nGateIndex: Integer;
  begin
    nGateIndex := Random(RouteInfo.nGateCount);
    Result := RouteInfo.sGameGateIP[nGateIndex];
    nGatePort := RouteInfo.nGameGatePort[nGateIndex];
  end;
var
  I: Integer;
  RouteInfo: pTRouteInfo;
begin
  nPort := 0;
  Result := '';
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    RouteInfo := @g_RouteInfo[I];
    if RouteInfo.sSelGateIP = sGateIP then
    begin
      Result := GetRoute(RouteInfo, nPort);
      Break;
    end;
  end;
end;

function CheckActiveRunGate(sGateIP: string; nPort: Integer): Boolean;
var
  I: Integer;
  Session: PSessionRunGateInfo;
begin
  Result := False;
  for I := 0 to RUNGATEMAXSESSION - 1 do
  begin
    Session := @SessionRunGateArray[I];
    if Session.Socket <> nil then
    begin
      if SameText(Session.sRemoteAddr, sGateIP) and (Session.nRemotePort = nPort) and (GetTickCount - Session.dwReceiveTick <= 2500) then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function GateActiveRouteIP(sGateIP: string; var nPort: Integer): string;
  function GetRoute(RouteInfo: pTRouteInfo; var nGatePort: Integer): string;
  var
    I: Integer;
    RunGateList: TList;
    nGateIndex: Integer;
    RunGateInfo: PRunGateInfo;
    MaxLevel: Integer;
  begin
    RunGateList := TList.Create;
    try
      for I := 0 to RouteInfo.nGateCount - 1 do
      begin
        if GetTickCount - RouteInfo.dwGameGateConnectTick[I] <= 3000 then
        begin
          RunGateList.Add(Pointer(I));
        end;
      end;

      if (RunGateList.Count = 0) and (not RouteInfo.EnabledRunGate2List) then
      begin
        Result := '';
        Exit;
      end;

      if RunGateList.Count > 0 then
      begin
        nGateIndex := Random(RunGateList.Count);
        nGateIndex := Integer(RunGateList.Items[nGateIndex]);
        if (nGateIndex >= 0) and (nGateIndex < RouteInfo.nGateCount) then
        begin
          Result := RouteInfo.sGameGateIP[nGateIndex];
          nGatePort := RouteInfo.nGameGatePort[nGateIndex];
        end;
      end;

      // 分配备用列表
      if RouteInfo.EnabledRunGate2List and
        (RouteInfo.nGateCount - RunGateList.Count >= RouteInfo.GameGateDisconnectCount) and
        (RouteInfo.RunGate2List.Count > 0) then
      begin
        RunGateList.Clear;
        MaxLevel := RouteInfo.RunGate2List.SortItems[0].Level;
        for I := 0 to RouteInfo.RunGate2List.Count - 1 do
        begin
          RunGateInfo := RouteInfo.RunGate2List.SortItems[I];
          if RunGateInfo.Enabled then
          begin
            if (RunGateInfo.Level = MaxLevel) then
            begin
              if GetTickCount - RunGateInfo.LastResponseTick <= 3000 then
                RunGateList.Add(Pointer(I));
            end
            else
            begin
              if RunGateList.Count > 0 then
                Break
              else
              begin
                MaxLevel := RunGateInfo.Level;
                if GetTickCount - RunGateInfo.LastResponseTick <= 3000 then
                  RunGateList.Add(Pointer(I));
              end;
            end;
          end;
        end;

        if RunGateList.Count > 0 then
        begin
          nGateIndex := Random(RunGateList.Count);
          nGateIndex := Integer(RunGateList.Items[nGateIndex]);
          if (nGateIndex >= 0) and (nGateIndex < RouteInfo.RunGate2List.Count) then
          begin
            Result := RouteInfo.RunGate2List.SortItems[nGateIndex].IP;
            nGatePort := RouteInfo.RunGate2List.SortItems[nGateIndex].Port;
          end;
        end;
      end;
    finally
      RunGateList.Free;
    end;
  end;
var
  I: Integer;
  RouteInfo: pTRouteInfo;
begin
  nPort := 0;
  Result := '';
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    RouteInfo := @g_RouteInfo[I];
    if RouteInfo.sSelGateIP = sGateIP then
    begin
      Result := GetRoute(RouteInfo, nPort);
      Break;
    end;
  end;
end;

function GetMapIndex(sMap: string): Integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 0 to g_MapList.Count - 1 do
  begin
    if g_MapList.Strings[I] = sMap then
    begin
      Result := Integer(g_MapList.Objects[I]);
      Break;
    end;
  end;
end;

procedure LoadConfig_DataSaveDB;
var
  Conf: TIniFile;
begin
  Conf := TIniFile.Create(g_sConfFileName);
  if Conf <> nil then
  begin
    g_nDataSaveDBType := Conf.ReadInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
    g_sDataSaveDBServer := Conf.ReadString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
    g_wDataSaveDBPort := Conf.ReadInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
    g_sDataSaveDBUser := Conf.ReadString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
    g_sDataSaveDBPassword := Conf.ReadString('DataSaveDB', 'DataSaveDBPassword', g_sDataSaveDBPassword);
    g_sDataSaveDataBase := Conf.ReadString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);

    Conf.Free;
  end;
end;

procedure LoadConfig();
var
  Conf: TIniFile;
  LoadInteger: Integer;
  sFileName: string;
begin
  Conf := TIniFile.Create(g_sConfFileName);
  if Conf <> nil then
  begin
    g_sDataDBFilePath := Conf.ReadString('DB', 'Dir', g_sDataDBFilePath);
    g_sBackupPath := Conf.ReadString('DB', 'Backup', g_sBackupPath);
    g_sLogPath := Conf.ReadString('DB', 'LogDir', g_sLogPath);

    g_nServerPort := Conf.ReadInteger('Setup', 'ServerPort', g_nServerPort);
    g_sServerAddr := Conf.ReadString('Setup', 'ServerAddr', g_sServerAddr);

    g_nGatePort := Conf.ReadInteger('Setup', 'GatePort', g_nGatePort);
    g_sGateAddr := Conf.ReadString('Setup', 'GateAddr', g_sGateAddr);

    g_sIDServerAddr := Conf.ReadString('Server', 'IDSAddr', g_sIDServerAddr);
    g_nIDServerPort := Conf.ReadInteger('Server', 'IDSPort', g_nIDServerPort);

    g_sServerName := Conf.ReadString('Setup', 'ServerName', g_sServerName);

    g_boCanCreateHuman := Conf.ReadBool('Setup', 'CanCreateHuman', g_boCanCreateHuman);             //允许建立新人物
    g_boCanDeleteHuman := Conf.ReadBool('Setup', 'CanDeleteHuman', g_boCanDeleteHuman);             //允许删除人物
    g_boCanGetBackDeleteHuman := Conf.ReadBool('Setup', 'CanGetBackDeleteHuman', g_boCanGetBackDeleteHuman); //允许找回删除的人物
    g_nCanDeleteHumanLowLevel := Conf.ReadInteger('Setup', 'CanDeleteHumanLowLevel', g_nCanDeleteHumanLowLevel); //以上级别不允许被删除
    g_boForbidNumberName := Conf.ReadBool('Setup', 'ForbidNumberName', g_boForbidNumberName);       //禁止建立包含数字的人物名
    g_boForbidLetterName := Conf.ReadBool('Setup', 'ForbidLetterName', g_boForbidLetterName);       //禁止建立全英文人物名

    g_boDenyChrName := Conf.ReadBool('Setup', 'DenyChrName', g_boDenyChrName);

    g_nCreateChrNameCount := Conf.ReadInteger('Setup', 'CreateChrNameCount', g_nCreateChrNameCount);

    if g_nCreateChrNameCount < 1 then
    begin
      g_nCreateChrNameCount := 1;
      Conf.WriteInteger('Setup', 'CreateChrNameCount', g_nCreateChrNameCount);
    end;

    LoadInteger := Conf.ReadInteger('Setup', 'DynamicIPMode', -1);
    if LoadInteger < 0 then
    begin
      Conf.WriteBool('Setup', 'DynamicIPMode', g_boDynamicIPMode);
    end
    else
      g_boDynamicIPMode := LoadInteger = 1;
    g_boCanRanking := Conf.ReadBool('Setup', 'CanRanking', g_boCanRanking);
    g_boAutoRefRanking := Conf.ReadBool('Setup', 'AutoRefRanking', g_boAutoRefRanking);
    g_nRankingCount  := Conf.ReadInteger('Setup', 'gRankingCount', g_nRankingCount);
    g_nRankingMinLevel := Conf.ReadInteger('Setup', 'RankingMinLevel', g_nRankingMinLevel);
    g_nRankingMaxLevel := Conf.ReadInteger('Setup', 'RankingMaxLevel', g_nRankingMaxLevel);
    g_nRefRankingHour1 := Conf.ReadInteger('Setup', 'RefRankingHour1', g_nRefRankingHour1);
    g_nRefRankingHour2 := Conf.ReadInteger('Setup', 'RefRankingHour2', g_nRefRankingHour2);

    g_nRefRankingMinute1 := Conf.ReadInteger('Setup', 'RefRankingMinute1', g_nRefRankingMinute1);
    g_nRefRankingMinute2 := Conf.ReadInteger('Setup', 'RefRankingMinute2', g_nRefRankingMinute2);

    g_nAutoRefRankingType := Conf.ReadInteger('Setup', 'AutoRefRankingType', g_nAutoRefRankingType);

    g_boUseActiveRunGage := Conf.ReadBool('Setup', 'UseActiveRunGage', g_boUseActiveRunGage);

    g_boShowBlockIPLog := Conf.ReadBool('Setup', 'ShowBlockIPLog', g_boShowBlockIPLog);

    LoadInteger := Conf.ReadInteger('Setup', 'SqliteFastSave', -1);
    if LoadInteger < 0 then
    begin
      Conf.WriteBool('Setup', 'SqliteFastSave', g_boSqliteFastSave);
    end;
    g_boSqliteFastSave := Conf.ReadBool('Setup', 'SqliteFastSave', g_boSqliteFastSave);

    {
    g_nDataSaveDBType := Conf.ReadInteger('DataSaveDB', 'DataSaveDBType', g_nDataSaveDBType);
    g_sDataSaveDBServer := Conf.ReadString('DataSaveDB', 'DataSaveDBServer', g_sDataSaveDBServer);
    g_wDataSaveDBPort := Conf.ReadInteger('DataSaveDB', 'DataSaveDBPort', g_wDataSaveDBPort);
    g_sDataSaveDBUser := Conf.ReadString('DataSaveDB', 'DataSaveDBUser', g_sDataSaveDBUser);
    g_sDataSaveDBPassword := Conf.ReadString('DataSaveDB', 'DataSaveDBPassword', g_sDataSaveDBPassword);
    g_sDataSaveDataBase := Conf.ReadString('DataSaveDB', 'DataSaveDataBase', g_sDataSaveDataBase);
    }
    Conf.Free;
  end;
  LoadIPTable();
  LoadGateID();
  LoadServerInfo();
  LoadChrNameList('DenyChrName.txt');

  sFileName := g_sFilePath + 'FilterNewHumanNameString.txt';
  if not FileExists(sFileName) then
  begin
    g_FilterNewHumanNameTextList.Add(' ');
    g_FilterNewHumanNameTextList.Add(#9);
    g_FilterNewHumanNameTextList.SaveToFile(sFileName);
  end
  else
  begin
    try
      g_FilterNewHumanNameTextList.LoadFromFile(sFileName);
    except

    end;
  end;

  sFileName := g_sFilePath + 'FilterRankingNameString.txt';
  if not FileExists(sFileName) then
  begin
    g_FilterRankingNameTextList.Add('GM');
    g_FilterRankingNameTextList.Add('管理');
    g_FilterRankingNameTextList.SaveToFile(sFileName);
  end
  else
  begin
    try
      g_FilterRankingNameTextList.LoadFromFile(g_sFilePath + 'FilterRankingNameString.txt');
    except

    end;
  end;


  sFileName := g_sFilePath + 'FirstName.txt';
  if not FileExists(sFileName) then
  begin
    g_FirstName.Add('姓');
    g_FirstName.SaveToFile(sFileName);
  end
  else
  begin
    try
      g_FirstName.LoadFromFile(g_sFilePath + 'FirstName.txt');
    except

    end;
  end;
        
  sFileName := g_sFilePath + 'LastName.txt';
  if not FileExists(sFileName) then
  begin
    g_LastName.Add('名');
    g_LastName.SaveToFile(sFileName);
  end
  else
  begin
    try
      g_LastName.LoadFromFile(g_sFilePath + 'LastName.txt');
    except

    end;
  end;

end;

function GetCodeMsgSize(X: Double): Integer;
begin
  if Int(X) < X then
    Result := Trunc(X) + 1
  else
    Result := Trunc(X)
end;

function CheckDenyChrName(sChrName: string): Boolean;
var
  I: Integer;
begin
  Result := True;
  for I := 0 to g_DenyChrNameList.Count - 1 do
  begin
    if CompareText(sChrName, g_DenyChrNameList.Strings[I]) = 0 then
    begin
      Result := False;
      Break;
    end;
  end;
end;

function CheckFilterNewHumanChrName(sChrName: string): Boolean;
var
  I: Integer;
begin
  // 非法字符过滤不区分大小写 2020-05-16
  sChrName := UpperCase(sChrName);
  
  Result := (Pos(#1, sChrName) > 0) or (Pos(#255, sChrName) > 0);
  if not Result then
  begin
    for I := 0 to g_FilterNewHumanNameTextList.Count - 1 do
    begin
      if (Pos(UpperCase(g_FilterNewHumanNameTextList.Strings[I]), sChrName) > 0) then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function CheckFilterRankingChrName(sChrName: string): Boolean;
var
  I: Integer;
begin
  Result := (Pos(#1, sChrName) > 0) or (Pos(#255, sChrName) > 0);
  if not Result then
    for I := 0 to g_FilterRankingNameTextList.Count - 1 do
    begin
      if (Pos(g_FilterRankingNameTextList.Strings[I], sChrName) > 0) then
      begin
        Result := True;
        Break;
      end;
    end;

  {for I := 0 to g_FilterRankingNameTextList.Count - 1 do begin
    if (CompareText(sChrName, g_FilterRankingNameTextList.Strings[I]) = 0) or
      AnsiContainsText(sChrName, g_FilterRankingNameTextList.Strings[I]) then begin
      Result := True;
      Break;
    end;
  end;}
end;

function CheckNumberName(sChrName: string): Boolean;
var
  I: Integer;
  Chr: Char;
  wChrName: WideString;
  S: string;
begin
  Result := False;
  wChrName := sChrName;
  for I := 1 to Length(wChrName) do
  begin
    S := wChrName[I];
    Chr := S[1];
    if (Chr in ['0'..'9']) then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function CheckLetterName(sChrName: string): Boolean;
var
  I: Integer;
  Chr: Char;
  wChrName: WideString;
  S: string;
begin
  Result := True;
  wChrName := sChrName;
  for I := 1 to Length(wChrName) do
  begin
    S := wChrName[I];
    Chr := UpperCase(S)[1];
    if not (Chr in ['A'..'Z']) then
    begin
      Result := False;
      Exit;
    end;
  end;
end;


{
  下面有一堆搞人的字符，支持大小写转换，但Sqlite又不支持，用如下代码来跑出来的:
  var
    I, J: Integer;
    WC1: WideChar;
    sTemp1, sTemp2: string;

    IsBreak: Boolean;
  begin
    mmo1.Lines.Clear;
    for I := 0 to High(Byte) do
    begin
      for J := 0 to High(Byte) do
      begin
        WC1 := WideChar(I * 256 + J);
        sTemp1 := WC1;

        if Length(WC1) > 0 then
        begin
          sTemp2 := WideUpperCase(sTemp1);

          if sTemp1 <> sTemp2 then
          begin
            if mmo1.Lines.IndexOf(sTemp1) < 0 then
              mmo1.Lines.Add(sTemp1);
          end;
        end;
      end;
    end;
}

function CheckCanCaseChar(sChrName: WideString): Boolean;
const
  FilterChars: WideString =
    'ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩЁАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ' +
    'μńňɑαβγδεζηθικλνξοπρστυφχψωабвгдежзийклмнопрстуфхцчшщъыьэюяёⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ';
var
  I, J: Integer;
  WC1, WC2: WideChar;
begin
  Result := True;
  for I := 1 to Length(sChrName) do
  begin
    WC1 := sChrName[I];

    for J := 1 to Length(FilterChars) do
    begin
      WC2 := FilterChars[J];

      if WC1 = WC2 then
      begin
        Result := False;
        Break;
      end;
    end;
  end;
end;

function CheckChrName(sChrName: string): Boolean;
var
  I: Integer;
  Chr: Char;
  boIsTwoByte: Boolean;
  FirstChr: Char;
begin
  Result := True;
  boIsTwoByte := False;
  FirstChr := #0;
  for I := 1 to Length(sChrName) do
  begin
    Chr := (sChrName[I]);
    if boIsTwoByte then
    begin
      //if Chr < #$A1 then Result:=False; //如果小于就是非法字符
//      if Chr < #$81 then Result:=False; //如果小于就是非法字符

      if not ((FirstChr <= #$F7) and (Chr >= #$40) and (Chr <= #$FE)) then
        if not ((FirstChr > #$F7) and (Chr >= #$40) and (Chr <= #$A0)) then Result := False;
      boIsTwoByte := False;
    end
    else
    begin                                                                                           //0045BEC0
      //if (Chr >= #$B0) and (Chr <= #$C8) then begin
      if (Chr >= #$81) and (Chr <= #$FE) then
      begin
        boIsTwoByte := True;
        FirstChr := Chr;
      end
      else
      begin                                                                                         //0x0045BED2
        if not ((Chr >= '0' {#30}) and (Chr <= '9' {#39})) and
          not ((Chr >= 'a' {#61}) and (Chr <= 'z') {#7A}) and
          not ((Chr >= 'A' {#41}) and (Chr <= 'Z' {#5A})) then
          Result := False;
      end;
    end;
    if not Result then Break;
  end;

  if Result then
  begin
    Result := CheckCanCaseChar(sChrName);
  end;
end;

function CheckSpecialChar(sChrName: WideString): Boolean;
const
  FilterChars: WideString = ' /@?''"\.,:;`~!#$%^&*()-_+|[]{}';
var
  I, J: Integer;
  WC1, WC2: WideChar;
begin
  Result := True;
  for I := 1 to Length(sChrName) do
  begin
    WC1 := sChrName[I];

    for J := 1 to Length(FilterChars) do
    begin
      WC2 := FilterChars[J];

      if WC1 = WC2 then
      begin
        Result := False;
        Break;
      end;
    end;
  end;
end;

procedure MainOutMessage(sMsg: string);
var
  tMsg: string;
begin
  if not g_boShowLogMsg then Exit;
  g_MainLogMsgList.Lock;
  try
    tMsg := '[' + DateTimeToStr(Now) + '] ' + sMsg;
    g_MainLogMsgList.Add(tMsg);
  finally
    g_MainLogMsgList.UnLock;
  end;
end;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  nParam := MakeLong(Word(tDBServer), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_dwGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

procedure InitRouteInfo;
var
  I: Integer;
begin
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    g_RouteInfo[I].RunGate2List := TRunGateList.Create;
  end;
end;

procedure FinalRouteInfo;
var
  I: Integer;
begin
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    g_RouteInfo[I].RunGate2List.Free;
  end;
end;

function Date2MyDate(Dt: TDateTime): Integer;
var
  Y, M, D: Word;
begin
  DecodeDate(Dt, Y, M, D);
  Result := Y * 10000 + M * 100 + D;
end;

function MyDate2Date(dt: Integer): TDateTime;
var
  Y, M, D: Word;
begin
  Result := 0;

  if dt > 10000000 then
  begin
    Y := dt div 10000;
    M := (dt - Y * 10000) div 100;
    D := dt mod 100;
    TryEncodeDate(Y, M, D, Result);
  end;
end;


initialization
  begin
    g_MainLogMsgList := TGStringList.Create;
    g_DenyChrNameList := TStringList.Create;
    g_ServerIPList := TStringList.Create;
    g_GateIDList := TStringList.Create;
    g_MapList := TStringList.Create;
    g_FilterNewHumanNameTextList := TStringList.Create;
    g_FilterRankingNameTextList := TStringList.Create;
    g_FirstName := TStringList.Create;
    g_LastName := TStringList.Create;
    InitializeCriticalSection(g_Ranking_CS);

    InitRouteInfo;
  end;

finalization
  begin
    g_DenyChrNameList.Free;
    g_ServerIPList.Free;
    g_GateIDList.Free;
    g_MapList.Free;
    g_FilterNewHumanNameTextList.Free;
    g_FilterRankingNameTextList.Free;   
    g_FirstName.Free;
    g_LastName.Free;
    g_MainLogMsgList.Free;
    DeleteCriticalSection(g_Ranking_CS);

    FinalRouteInfo;
  end;

end.
