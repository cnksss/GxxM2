unit Nations;

interface

uses
  Windows, Classes, SysUtils, Controls, IniFiles, Grobal2, ObjBase, ObjPlayer, M2Definition;

type
  TNationManage = class
  private
    FCount: Integer;
    NationList: array[1..MAXNATIONCOUNT] of TList;
    NationConfigList: array[1..MAXNATIONCOUNT] of TNationInfo;

    function Get(Index: Integer): pTNationInfo;
  public
    constructor Create();
    destructor Destroy; override;
    procedure SaveConfig(); overload;
    procedure SaveConfig(btNation: Word); overload;
    procedure LoadConfig();
    function IsMember(PlayObject: TPlayObject): Boolean;

    procedure AddMember(PlayObject: TPlayObject);
    procedure DeleteMember(PlayObject: TPlayObject);
    procedure RenameNationName(btNation: Word; NewName: string);
    function GetNationName(PlayObject: TPlayObject): string;
    function GetNationInfo(const NationName: string): pTNationInfo;
    function GetNationIndex(const NationName: string): Integer;
    procedure SendNationMsg(btNation: Word; sMsg: string);
    property Items[Index: Integer]: pTNationInfo read Get;
    property Count: Integer read FCount;
  end;

implementation

uses
  M2Share;

constructor TNationManage.Create();
var
  I: Integer;
begin
  FCount := 0;
  for I := 1 to MAXNATIONCOUNT do
  begin
    NationList[I] := TList.Create();
    NationConfigList[I].sName := '';
    NationConfigList[I].nPeoples := 0;
    NationConfigList[I].sRedHomeMap := g_Config.sRedHomeMap;
    NationConfigList[I].nRedHomeX := g_Config.nRedHomeX;
    NationConfigList[I].nRedHomeY := g_Config.nRedHomeY;
    NationConfigList[I].sHomeMap := g_Config.sHomeMap;
    NationConfigList[I].nHomeX := g_Config.nHomeX;
    NationConfigList[I].nHomeY := g_Config.nHomeY;
    NationConfigList[I].sKingName := '';
    NationConfigList[I].nGold := 0; // 金币
    NationConfigList[I].wBuilding := 0; // 建筑能力
    NationConfigList[I].wArm := 0; // 军事能力
    NationConfigList[I].wEconomy := 0; // 经济能力
    NationConfigList[I].wPolitics := 0; // 政治能力
    NationConfigList[I].wContribution := 0; // 国家贡献
    NationConfigList[I].btMaps := 0; // 地图数
  end;
end;

destructor TNationManage.Destroy;
var
  I: Integer;
begin
  for I := 1 to MAXNATIONCOUNT do
  begin
    NationList[I].Free;
  end;
  inherited;
end;

function TNationManage.GetNationInfo(const NationName: string): pTNationInfo;
var
  I: Integer;
begin
  Result := nil;
  if NationName <> '' then
  begin
    for I := 1 to MAXNATIONCOUNT do
    begin
      if CompareText(NationConfigList[I].sName, NationName) = 0 then
      begin
        Result := @NationConfigList[I];
        break;
      end;
    end;
  end;
end;

function TNationManage.GetNationIndex(const NationName: string): Integer;
var
  I: Integer;
begin
  Result := 0;
  if NationName <> '' then
  begin
    for I := 1 to MAXNATIONCOUNT do
    begin
      if CompareText(NationConfigList[I].sName, NationName) = 0 then
      begin
        Result := I;
        break;
      end;
    end;
  end;
end;

function TNationManage.GetNationName(PlayObject: TPlayObject): string;
begin
  Result := '';
  if (PlayObject.m_btNation >= 1) and (PlayObject.m_btNation <= MAXNATIONCOUNT) and (NationConfigList[PlayObject.m_btNation].sName
    <> '') then
  begin
    AddMember(PlayObject);
    Result := NationConfigList[PlayObject.m_btNation].sName;
  end;
end;

procedure TNationManage.AddMember(PlayObject: TPlayObject);
var
  I: Integer;
begin
  if (PlayObject.m_btNation >= 1) and (PlayObject.m_btNation <= MAXNATIONCOUNT) then
  begin
    for I := 0 to NationList[PlayObject.m_btNation].Count - 1 do
    begin
      if NationList[PlayObject.m_btNation].Items[I] = PlayObject then
      begin
        Exit;
      end;
    end;
    NationList[PlayObject.m_btNation].Add(PlayObject);
  end;
end;

procedure TNationManage.DeleteMember(PlayObject: TPlayObject);
var
  I: Integer;
begin
  if (PlayObject.m_btNation >= 1) and (PlayObject.m_btNation <= MAXNATIONCOUNT) then
  begin
    for I := 0 to NationList[PlayObject.m_btNation].Count - 1 do
    begin
      if NationList[PlayObject.m_btNation].Items[I] = PlayObject then
      begin
        NationList[PlayObject.m_btNation].Delete(I);
        break;
      end;
    end;
  end;
end;

function TNationManage.IsMember(PlayObject: TPlayObject): Boolean;
var
  I: Integer;
begin
  Result := False;
  if (PlayObject.m_btNation >= 1) and (PlayObject.m_btNation <= MAXNATIONCOUNT) then
  begin
    for I := 0 to NationList[PlayObject.m_btNation].Count - 1 do
    begin
      if NationList[PlayObject.m_btNation].Items[I] = PlayObject then
      begin
        Result := True;
        break;
      end;
    end;
  end;
end;

procedure TNationManage.SendNationMsg(btNation: Word; sMsg: string);
var
  I: Integer;
  PlayObject: TPlayObject;
  nCheckCode: Integer;
begin
  nCheckCode := 0;
  try
    if (btNation >= 1) and (btNation <= MAXNATIONCOUNT) then
    begin
      if g_Config.boShowPreFixMsg then
        sMsg := g_Config.sNationMsgPreFix + sMsg;

      for I := 0 to NationList[btNation].Count - 1 do
      begin
        nCheckCode := 3;
        PlayObject := TPlayObject(NationList[btNation].Items[I]);
        if PlayObject = nil then
          Continue;

        nCheckCode := 4;
        if PlayObject.m_boBanNationChat then
        begin
          nCheckCode := 5;
          PlayObject.SendMsg(PlayObject, RM_NATIONMESSAGE, 0, g_Config.btNationMsgFColor, g_Config.btNationMsgBColor, 0, sMsg);
          nCheckCode := 6;
        end;
      end;
    end;
  except
    on e: Exception do
    begin
      MainOutMessage('[Exceptiion] TNationManage.SendNationMsg CheckCode: ' + IntToStr(nCheckCode) + ' Msg = ' + sMsg);
      MainOutMessage(e.Message);
    end;
  end;
end;

function TNationManage.Get(Index: Integer): pTNationInfo;
begin
  if (Index >= 1) and (Index <= MAXNATIONCOUNT) and (NationConfigList[Index].sName <> '') then
    Result := @NationConfigList[Index]
  else
    Result := nil;
end;

procedure TNationManage.SaveConfig(btNation: Word);
var
  Config: TIniFile;
  NationInfo: pTNationInfo;
  sFileName: string;
begin
  if (btNation >= 1) and (btNation <= MAXNATIONCOUNT) then
  begin
    NationInfo := @NationConfigList[btNation];
    if NationInfo.sName <> '' then
    begin
      sFileName := g_Config.sEnvirDir + '\Nations\';
      if not DirectoryExists(sFileName) then
        ForceDirectories(sFileName);

      sFileName := g_Config.sEnvirDir + Format('\Nations\%s.ini', [NationInfo.sName]);

      Config := TIniFile.Create(sFileName);

      Config.WriteInteger('Info', 'Peoples', NationInfo.nPeoples);
      Config.WriteString('Info', 'RedHomeMap', NationInfo.sRedHomeMap);
      Config.WriteInteger('Info', 'nRedHomeX', NationInfo.nRedHomeX);
      Config.WriteInteger('Info', 'RedHomeY', NationInfo.nRedHomeY);
      Config.WriteString('Info', 'HomeMap', NationInfo.sHomeMap);
      Config.WriteInteger('Info', 'HomeX', NationInfo.nHomeX);
      Config.WriteInteger('Info', 'HomeY', NationInfo.nHomeY);
      Config.WriteString('Info', 'King', NationInfo.sKingName);

      Config.WriteInteger('Info', 'Gold', NationInfo.nGold); // 金币
      Config.WriteInteger('Info', 'Building', NationInfo.wBuilding); // 建筑能力
      Config.WriteInteger('Info', 'Arm', NationInfo.wArm); // 军事能力
      Config.WriteInteger('Info', 'Economy', NationInfo.wEconomy); // 经济能力
      Config.WriteInteger('Info', 'Politics', NationInfo.wPolitics); // 政治能力
      Config.WriteInteger('Info', 'Contribution', NationInfo.wContribution); // 国家贡献
      Config.WriteInteger('Info', 'Maps', NationInfo.btMaps); // 地图数

      Config.Free;
    end;
  end;
end;

procedure TNationManage.SaveConfig();
var
  I: Integer;
  Config: TIniFile;
  NationInfo: pTNationInfo;
  sFileName: string;
begin
  for I := 1 to MAXNATIONCOUNT do
  begin
    NationInfo := @NationConfigList[I];
    if NationInfo.sName <> '' then
    begin
      sFileName := g_Config.sEnvirDir + '\Nations\';
      if not DirectoryExists(sFileName) then
        ForceDirectories(sFileName);

      sFileName := g_Config.sEnvirDir + Format('\Nations\%s.ini', [NationInfo.sName]);

      Config := TIniFile.Create(sFileName);
      Config.WriteInteger('Info', 'Peoples', NationInfo.nPeoples);
      Config.WriteString('Info', 'RedHomeMap', NationInfo.sRedHomeMap);
      Config.WriteInteger('Info', 'nRedHomeX', NationInfo.nRedHomeX);
      Config.WriteInteger('Info', 'RedHomeY', NationInfo.nRedHomeY);
      Config.WriteString('Info', 'HomeMap', NationInfo.sHomeMap);
      Config.WriteInteger('Info', 'HomeX', NationInfo.nHomeX);
      Config.WriteInteger('Info', 'HomeY', NationInfo.nHomeY);
      Config.WriteString('Info', 'King', NationInfo.sKingName);

      Config.WriteInteger('Info', 'Gold', NationInfo.nGold); // 金币
      Config.WriteInteger('Info', 'Building', NationInfo.wBuilding); // 建筑能力
      Config.WriteInteger('Info', 'Arm', NationInfo.wArm); // 军事能力
      Config.WriteInteger('Info', 'Economy', NationInfo.wEconomy); // 经济能力
      Config.WriteInteger('Info', 'Politics', NationInfo.wPolitics); // 政治能力
      Config.WriteInteger('Info', 'Contribution', NationInfo.wContribution); // 国家贡献
      Config.WriteInteger('Info', 'Maps', NationInfo.btMaps); // 地图数

      Config.Free;
    end;
  end;
end;

procedure TNationManage.LoadConfig();
var
  sFileName: string;
  I: Integer;
  Config: TIniFile;
  NationInfo: pTNationInfo;
begin
  sFileName := g_Config.sEnvirDir + '\Nations\Nations.ini';
  if FileExists(sFileName) then
  begin
    Config := TIniFile.Create(sFileName);
    for I := 1 to MAXNATIONCOUNT do
      NationConfigList[I].sName := Trim(Config.ReadString('Names', 'NationalNames' + IntToStr(I), ''));

    Config.Free;

    for I := 1 to MAXNATIONCOUNT do
    begin
      if NationConfigList[I].sName <> '' then
      begin
        sFileName := g_Config.sEnvirDir + Format('\Nations\%s.ini', [NationConfigList[I].sName]);
        if FileExists(sFileName) then
        begin
          Inc(FCount);
          NationInfo := @NationConfigList[I];
          Config := TIniFile.Create(sFileName);
          NationInfo.nPeoples := Config.ReadInteger('Info', 'Peoples', 0);
          NationInfo.sRedHomeMap := Config.ReadString('Info', 'RedHomeMap', g_Config.sRedHomeMap);
          NationInfo.nRedHomeX := Config.ReadInteger('Info', 'nRedHomeX', g_Config.nRedHomeX);
          NationInfo.nRedHomeY := Config.ReadInteger('Info', 'RedHomeY', g_Config.nRedHomeY);
          NationInfo.sHomeMap := Config.ReadString('Info', 'HomeMap', g_Config.sHomeMap);
          NationInfo.nHomeX := Config.ReadInteger('Info', 'HomeX', g_Config.nHomeX);
          NationInfo.nHomeY := Config.ReadInteger('Info', 'HomeY', g_Config.nHomeY);
          NationInfo.sKingName := Config.ReadString('Info', 'King', NationInfo.sKingName);

          NationInfo.nGold := Config.ReadInteger('Info', 'Gold', 0); // 金币
          NationInfo.wBuilding := Config.ReadInteger('Info', 'Building', 0); // 建筑能力
          NationInfo.wArm := Config.ReadInteger('Info', 'Arm', 0); // 军事能力
          NationInfo.wEconomy := Config.ReadInteger('Info', 'Economy', 0); // 经济能力
          NationInfo.wPolitics := Config.ReadInteger('Info', 'Politics', 0); // 政治能力
          NationInfo.wContribution := Config.ReadInteger('Info', 'Contribution', 0); // 国家贡献
          NationInfo.btMaps := Config.ReadInteger('Info', 'Maps', NationInfo.btMaps); // 地图数

          Config.Free;
        end;
      end;
    end;
  end;
end;

procedure TNationManage.RenameNationName(btNation: Word; NewName: string);
var
  I: Integer;
  Player: TPlayObject;
  NationInfo: pTNationInfo;
  OldNationName, sOldFile, sNewFile: string;
  sFileName: string;
  Config: TIniFile;
begin
  // 国家名字已经存在
  if GetNationIndex(NewName) > 0 then
    Exit;

  if (btNation >= 1) and (btNation <= MAXNATIONCOUNT) then
  begin
    NationInfo := @NationConfigList[btNation];
    OldNationName := NationInfo.sName;
    NationInfo.sName := NewName;

    for I := 0 to NationList[btNation].Count - 1 do
    begin
      Player := NationList[btNation].Items[I];
      Player.m_sNationaName := NewName;
    end;

    sOldFile := g_Config.sEnvirDir + Format('\Nations\%s.ini', [OldNationName]);
    sNewFile := g_Config.sEnvirDir + Format('\Nations\%s.ini', [NewName]);
    RenameFile(sOldFile, sNewFile);

    g_NationManage.SaveConfig;

    sFileName := g_Config.sEnvirDir + '\Nations\Nations.ini';
    if FileExists(sFileName) then
    begin
      Config := TIniFile.Create(sFileName);
      for I := 1 to MAXNATIONCOUNT do
      begin
        Config.WriteString('Names', 'NationalNames' + IntToStr(I), NationConfigList[I].sName);
      end;
      Config.Free;
    end;
  end;
end;

end.

