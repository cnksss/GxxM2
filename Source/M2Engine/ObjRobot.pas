unit ObjRobot;

interface

uses
  Windows, Classes, SysUtils, DateUtils, ObjBase, ObjPlayer, Grobal2, SDK;

const
  sROAUTORUN = '#AUTORUN';
  sRONPCLABLEJMP = 'NPC';
  nRONPCLABLEJMP = 100;
  sRODAY = 'DAY';
  nRODAY = 200;
  sROHOUR = 'HOUR';
  nROHOUR = 201;
  sROMIN = 'MIN';
  nROMIN = 202;
  sROSEC = 'SEC';
  nROSEC = 203;
  sRUNONWEEK = 'RUNONWEEK';                                                                         // 指定星期几运行
  nRUNONWEEK = 300;
  sRUNONDAY = 'RUNONDAY';                                                                           // 指定几日运行
  nRUNONDAY = 301;
  sRUNONHOUR = 'RUNONHOUR';                                                                         // 指定小时运行
  nRUNONHOUR = 302;
  sRUNONMIN = 'RUNONMIN';                                                                           // 指定分钟运行
  nRUNONMIN = 303;
  sRUNONSEC = 'RUNONSEC';
  nRUNONSEC = 304;

type
  TOpType = (o_NPC);

  TAutoRunInfo = record
    dwRunTick: LongWord;                                                                            // 上一次运行时间记录
    dwRunTimeLen: LongWord;                                                                         // 运行间隔时间长
    nRunCmd: Integer;                                                                               // 自动运行类型
    nMoethod: Integer;
    sParam1: string;                                                                                // 运行脚本标签
    sParam2: string;                                                                                // 传送到脚本参数内容
    sParam3: string;
    sParam4: string;
    nParam1: Integer;
    nParam2: Integer;
    nParam3: Integer;
    nParam4: Integer;
    boStatus: Boolean;
  end;

  pTAutoRunInfo = ^TAutoRunInfo;

  TRobotObject = class(TPlayObject)
    m_sScriptFileName: string;
    m_AutoRunList: TList;
  private
    m_boRunOnWeek: Boolean;                                                                         // 是否已执行操作；
    procedure LoadScript();
    procedure ClearScript();
    procedure ProcessAutoRun();
    procedure AutoRun(AutoRunInfo: pTAutoRunInfo);
    procedure AutoRunOfOnWeek(AutoRunInfo: pTAutoRunInfo);
    procedure AutoRunOfOnDay(AutoRunInfo: pTAutoRunInfo);
    procedure AutoRunOfOnHour(AutoRunInfo: pTAutoRunInfo);
    procedure AutoRunOfOnMin(AutoRunInfo: pTAutoRunInfo);
    procedure AutoRunOfOnSec(AutoRunInfo: pTAutoRunInfo);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString); override;
    procedure ReloadScript();
    procedure Run(); override;
  end;

  TRobotManage = class
    RobotHumanList: TGStringList;
  private
    FInitialized: Boolean;
  public
    constructor Create();
    destructor Destroy; override;
    procedure RELOADROBOT();
    procedure LoadRobot();
    procedure UnLoadRobot();
    procedure Run;
  end;

implementation

uses
  M2Share, HUtil32;

{ TRobotObject }

procedure TRobotObject.AutoRun(AutoRunInfo: pTAutoRunInfo);
begin
  if (g_RobotNPC = nil) or (AutoRunInfo = nil) or (m_PEnvir = nil) then
  begin
    Exit;
  end;
  if MyGetTickCount - AutoRunInfo.dwRunTick > AutoRunInfo.dwRunTimeLen then
  begin
    case AutoRunInfo.nRunCmd of                                                                     //
      nRONPCLABLEJMP:
        begin
          case AutoRunInfo.nMoethod of                                                              //
            nRODAY:
              begin
                if MyGetTickCount - AutoRunInfo.dwRunTick > 24 * 60 * 60 * 1000 * LongWord(AutoRunInfo.nParam1) then
                begin
                  AutoRunInfo.dwRunTick := MyGetTickCount();
                  m_nScriptGotoCount := 0;
                  g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
                end;
              end;
            nROHOUR:
              begin
                if MyGetTickCount - AutoRunInfo.dwRunTick > 60 * 60 * 1000 * LongWord(AutoRunInfo.nParam1) then
                begin
                  AutoRunInfo.dwRunTick := MyGetTickCount();
                  m_nScriptGotoCount := 0;
                  g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
                end;
              end;
            nROMIN:
              begin
                if MyGetTickCount - AutoRunInfo.dwRunTick > 60 * 1000 * LongWord(AutoRunInfo.nParam1) then
                begin
                  AutoRunInfo.dwRunTick := MyGetTickCount();
                  m_nScriptGotoCount := 0;
                  g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
                end;
              end;
            nROSEC:
              begin
                if MyGetTickCount - AutoRunInfo.dwRunTick > 1000 * LongWord(AutoRunInfo.nParam1) then
                begin
                  AutoRunInfo.dwRunTick := MyGetTickCount();
                  m_nScriptGotoCount := 0;
                  g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
                end;
              end;
            nRUNONWEEK:
              AutoRunOfOnWeek(AutoRunInfo);
            nRUNONDAY:
              AutoRunOfOnDay(AutoRunInfo);
            nRUNONHOUR:
              AutoRunOfOnHour(AutoRunInfo);
            nRUNONMIN:
              AutoRunOfOnMin(AutoRunInfo);
            nRUNONSEC:
              AutoRunOfOnSec(AutoRunInfo);

          end;                                                                                      // case
        end;
      1:
        ;
      2:
        ;
      3:
        ;
    end;                                                                                            // case
  end;
end;

procedure TRobotObject.AutoRunOfOnDay(AutoRunInfo: pTAutoRunInfo);
var
  nMIN, nHOUR: Integer;
  wHour, wMin, wSec, wMSec: Word;
  sMIN, sHOUR: string;
  sLineText, sLabel: string;
begin
  sLineText := AutoRunInfo.sParam1;
  sLineText := GetValidStr3_Ex(sLineText, sHOUR, ':');
  sLineText := GetValidStr3_Ex(sLineText, sMIN, ':');
  nHOUR := StrToIntDef(sHOUR, -1);
  nMIN := StrToIntDef(sMIN, -1);
  sLabel := AutoRunInfo.sParam2;
  DecodeTime(Time, wHour, wMin, wSec, wMSec);
  if (nHOUR in [0..24]) and (nMIN in [0..60]) then
  begin
    if (wHour = nHOUR) then
    begin
      if (wMin = nMIN) then
      begin
        if not AutoRunInfo.boStatus then
        begin
          m_nScriptGotoCount := 0;
          g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
          // MainOutMessage('RUNONWEEK Test ' + AutoRunInfo.sParam1);
          AutoRunInfo.boStatus := True;
        end;
      end
      else
      begin
        AutoRunInfo.boStatus := False;
      end;
    end;
  end;
end;

procedure TRobotObject.AutoRunOfOnHour(AutoRunInfo: pTAutoRunInfo);
begin

end;

procedure TRobotObject.AutoRunOfOnMin(AutoRunInfo: pTAutoRunInfo);
begin

end;

procedure TRobotObject.AutoRunOfOnSec(AutoRunInfo: pTAutoRunInfo);
begin

end;

procedure TRobotObject.AutoRunOfOnWeek(AutoRunInfo: pTAutoRunInfo);
var
  nMIN, nHOUR, nWeek: Integer;
  wWeek, wHour, wMin, wSec, wMSec: Word;
  sMIN, sHOUR, sWeek: string;
  sLineText, sLabel: string;
begin
  sLineText := AutoRunInfo.sParam1;
  sLineText := GetValidStr3(sLineText, sWeek, [':']);
  sLineText := GetValidStr3(sLineText, sHOUR, [':']);
  sLineText := GetValidStr3(sLineText, sMIN, [':']);
  nWeek := StrToIntDef(sWeek, -1);
  nHOUR := StrToIntDef(sHOUR, -1);
  nMIN := StrToIntDef(sMIN, -1);
  sLabel := AutoRunInfo.sParam2;
  DecodeTime(Time, wHour, wMin, wSec, wMSec);
  wWeek := DayOfTheWeek(Now);
  if (nWeek in [1..7]) and (nHOUR in [0..24]) and (nMIN in [0..60]) then
  begin
    if (wWeek = nWeek) and (wHour = nHOUR) then
    begin
      if (wMin = nMIN) then
      begin
        if not AutoRunInfo.boStatus then
        begin
          m_nScriptGotoCount := 0;
          g_RobotNPC.GotoLable(Self, AutoRunInfo.sParam2, False);
          // MainOutMessage('RUNONWEEK Test ' + AutoRunInfo.sParam1);
          AutoRunInfo.boStatus := True;
        end;
      end
      else
      begin
        AutoRunInfo.boStatus := False;
      end;
    end;
  end;
end;

procedure TRobotObject.ClearScript;
var
  I: Integer;
begin
  for I := 0 to m_AutoRunList.Count - 1 do
  begin
    Dispose(pTAutoRunInfo(m_AutoRunList.Items[I]));
  end;
  m_AutoRunList.Clear;
end;

constructor TRobotObject.Create;
begin
  inherited;
  m_AutoRunList := TList.Create;
  m_boSuperMan := True;
  m_boRunOnWeek := False;
end;

destructor TRobotObject.Destroy;
begin
  ClearScript();
  m_AutoRunList.Free;
  inherited;
end;

procedure TRobotObject.LoadScript;
var
  I: Integer;
  LoadList: TStringList;
  sFileName: string;
  sLineText: string;
  sActionType: string;
  sRunCmd: string;
  sMoethod: string;
  sParam1: string;
  sParam2: string;
  sParam3: string;
  sParam4: string;
  AutoRunInfo: pTAutoRunInfo;
begin
  sFileName := g_Config.sEnvirDir + 'Robot_def\' + m_sScriptFileName + '.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sActionType, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sRunCmd, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sMoethod, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sParam1, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sParam2, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sParam3, [' ', '/', #9]);
        sLineText := GetValidStr3(sLineText, sParam4, [' ', '/', #9]);
        if CompareText(sActionType, sROAUTORUN) = 0 then
        begin
          if CompareText(sRunCmd, sRONPCLABLEJMP) = 0 then
          begin
            New(AutoRunInfo);
            AutoRunInfo.dwRunTick := MyGetTickCount;
            AutoRunInfo.dwRunTimeLen := 0;
            AutoRunInfo.boStatus := False;
            AutoRunInfo.nRunCmd := nRONPCLABLEJMP;
            if CompareText(sMoethod, sRODAY) = 0 then
              AutoRunInfo.nMoethod := nRODAY;
            if CompareText(sMoethod, sROHOUR) = 0 then
              AutoRunInfo.nMoethod := nROHOUR;
            if CompareText(sMoethod, sROMIN) = 0 then
              AutoRunInfo.nMoethod := nROMIN;
            if CompareText(sMoethod, sROSEC) = 0 then
              AutoRunInfo.nMoethod := nROSEC;
            if CompareText(sMoethod, sRUNONWEEK) = 0 then
              AutoRunInfo.nMoethod := nRUNONWEEK;
            if CompareText(sMoethod, sRUNONDAY) = 0 then
              AutoRunInfo.nMoethod := nRUNONDAY;
            if CompareText(sMoethod, sRUNONHOUR) = 0 then
              AutoRunInfo.nMoethod := nRUNONHOUR;
            if CompareText(sMoethod, sRUNONMIN) = 0 then
              AutoRunInfo.nMoethod := nRUNONMIN;
            if CompareText(sMoethod, sRUNONSEC) = 0 then
              AutoRunInfo.nMoethod := nRUNONSEC;

            AutoRunInfo.sParam1 := sParam1;
            AutoRunInfo.sParam2 := sParam2;
            AutoRunInfo.sParam3 := sParam3;
            AutoRunInfo.sParam4 := sParam4;
            AutoRunInfo.nParam1 := StrToIntDef(sParam1, 1);
            m_AutoRunList.Add(AutoRunInfo);
          end;
        end;

      end;
    end;
    LoadList.Free;
  end;
end;

procedure TRobotObject.ProcessAutoRun;
var
  I: Integer;
  AutoRunInfo: pTAutoRunInfo;
begin
  for I := 0 to m_AutoRunList.Count - 1 do
  begin
    AutoRunInfo := pTAutoRunInfo(m_AutoRunList.Items[I]);
    AutoRun(AutoRunInfo);
  end;
end;

procedure TRobotObject.ReloadScript;
begin
  ClearScript();
  LoadScript();
end;

procedure TRobotObject.Run;
begin
  ProcessAutoRun();
  // inherited;
end;

procedure TRobotObject.SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString);
begin

end;

{ TRobotManage }

constructor TRobotManage.Create;
begin
  RobotHumanList := TGStringList.Create;
  FInitialized := False;
  // LoadRobot();
end;

destructor TRobotManage.Destroy;
begin
  UnLoadRobot();
  RobotHumanList.Free;
  inherited;
end;

procedure TRobotManage.LoadRobot;
var
  I: Integer;
  LoadList: TStringList;
  sFileName: string;
  sLineText: string;
  sRobotName: string;
  sScriptFileName: string;
  RobotHuman: TRobotObject;
begin
  RobotHumanList.Lock;
  try
    FInitialized := False;
    sFileName := g_Config.sEnvirDir + 'Robot.txt';
    if FileExists(sFileName) then
    begin
      LoadList := TStringList.Create;
      LoadList.LoadFromFile(sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := LoadList.Strings[I];
        if (sLineText <> '') and (sLineText[1] <> ';') then
        begin
          sLineText := GetValidStr3(sLineText, sRobotName, [' ', '/', #9]);
          sLineText := GetValidStr3(sLineText, sScriptFileName, [' ', '/', #9]);
          if (sRobotName <> '') and (sScriptFileName <> '') then
          begin
            RobotHuman := TRobotObject.Create;
            RobotHuman.m_sCharName := sRobotName;
            if RobotHuman.m_sCharName = '' then
              RobotHuman.m_sCharName := 'TRobotObject';
            RobotHuman.m_sMapName := '0';
            RobotHuman.m_PEnvir := g_MapManager.FindMap(RobotHuman.m_sMapName);
            RobotHuman.m_sScriptFileName := sScriptFileName;
            RobotHuman.LoadScript;
            RobotHumanList.AddObject(RobotHuman.m_sCharName, RobotHuman);
          end;
        end;
      end;
      LoadList.Free;
    end;
  finally
    FInitialized := True;
    RobotHumanList.UnLock;
  end;
end;

procedure TRobotManage.RELOADROBOT;
begin
  UnLoadRobot();
  LoadRobot();
end;

procedure TRobotManage.Run;
var
  I: Integer;
resourcestring
  sExceptionMsg = '[Exception] TRobotManage.Run';
begin
  if not FInitialized then
    Exit;
  RobotHumanList.Lock;
  try
    for I := 0 to RobotHumanList.Count - 1 do
    begin
      if not FInitialized then
        break;
      if RobotHumanList.Objects[I] <> nil then
        TRobotObject(RobotHumanList.Objects[I]).Run;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
  RobotHumanList.UnLock;
end;

procedure TRobotManage.UnLoadRobot;
var
  I: Integer;
begin
  RobotHumanList.Lock;
  try
    FInitialized := False;
    for I := 0 to RobotHumanList.Count - 1 do
    begin
      if RobotHumanList.Objects[I] <> nil then
        TRobotObject(RobotHumanList.Objects[I]).Free;
    end;
    RobotHumanList.Clear;
  finally
    RobotHumanList.UnLock;
  end;
end;

end.

