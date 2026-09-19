unit Guild;

interface

uses
  Windows, SysUtils, Classes, IniFiles, ObjBase, Math, ObjPlayer, ObjHero, M2Definition, StringListHelper;

type
  TGuildRank = record
    nRankNo: Integer;
    sRankName: string;
    MemberList: TStringList;
  end;

  pTGuildRank = ^TGuildRank;

  TWarGuild = record
    Guild: TObject;
    dwWarTick: LongWord;
    dwWarTime: LongWord;
  end;

  pTWarGuild = ^TWarGuild;

  PWantAddMember = ^TWantAddMember;

  TWantAddMember = record
    UserName: string;
    Sex: Byte;
    Job: Byte;
    Level: LongWord;
    AddTime: TDateTime;
  end;

  pTGuild = ^TGuild;

  TGuild = class
    sGuildName: string; // 0x04
    NoticeList: TStringList; // 0x08
    GuildWarList: TStringList; // 0x0C
    GuildAllyList: TStringList; // 0x10
    GuildAttentionList: TStringList; // 关注行会
    GuildRequestAllList: TStringList;

    m_RankList: TList; // 0x14 职位列表
    nContestPoint: Integer; // 0x18
    boTeamFight: Boolean; // 0x1C;
    // MatchPoint   :Integer;
    TeamFightDeadList: TStringList; // 0x20
    m_boEnableAuthAlly: Boolean; // 0x24
    dwSaveTick: LongWord; // 0x28
    boChanged: Boolean; // 0x2C;
    m_DynamicVarList: TList;
    m_nMemberMaxLimit: Integer; // 行会成员最高数量
  private
    m_Config: TIniFile;
    m_nBuildPoint: Integer; // 建筑度
    m_nAurae: Integer; // 人气度
    m_nStability: Integer; // 安定度
    m_nFlourishing: Integer; // 繁荣度
    m_nChiefItemCount: Integer; // 行会领取装备数量
    m_ExemptList: TStrings; // 移除豁免列表

    // 招贤设置 chongchong 2015-08-25
    m_JoinJob: Integer;
    m_JoinLevel: LongWord;
    m_JoinMsg: string;
    m_EnabledAlly: Boolean;

    FWantAddMemberList: TList;

    function SetGuildInfo(sChief: string): Boolean;
    procedure ClearRank();
    procedure SaveGuildFile(sFileName: string);
    procedure SaveGuildConfig(sFileName: string);
    procedure SaveGuildWantAddMembers(sFileName: string);
    function GetMemberCount(): Integer;
    function GetOnlineMemberCount(): Integer;
    function GetMemberIsFull(): Boolean;
    procedure SetAuraePoint(nPoint: Integer);
    procedure SetBuildPoint(nPoint: Integer);
    procedure SetStabilityPoint(nPoint: Integer);
    procedure SetFlourishPoint(nPoint: Integer);
    procedure SetChiefItemCount(nPoint: Integer);

    function GetWantAddMemberCount: Integer;
    function GetWantAddMemebers(Index: Integer): PWantAddMember;
    procedure SetEnabledAlly(Value: Boolean);
  public
    constructor Create(sName: string);
    destructor Destroy; override;

    procedure SaveGuildInfoFile();
    function LoadGuild(): Boolean;
    function LoadGuildFile(sGuildFileName: string): Boolean;
    function LoadGuildConfig(sGuildFileName: string): Boolean;
    function LoadGuildWantAddMembers(sGuildFileName: string): Boolean;
    procedure UpdateGuildFile;
    procedure CheckSaveGuildFile;
    function IsMember(sName: string): Boolean;
    function IsAllyGuild(AOtherGuild: TGuild): Boolean;
    function IsWarGuild(AOtherGuild: TGuild): Boolean;
    function IsAttentionGuild(AOtherGuild: TGuild): Boolean;
    function IsRequestAllListGuild(Guild: TGuild): Boolean;
    function DelAllyGuild(Guild: TGuild): Boolean;
    function DelAttentionGuild(Guild: TGuild): Boolean;
    function DelRequestAllListGuild(Guild: TGuild): Boolean;
    procedure TeamFightWhoDead(sName: string);
    procedure TeamFightWhoWinPoint(sName: string; nPoint: Integer);
    procedure SendGuildMsg(sMsg: string);
    procedure SendGuildMsgEx(sMsg: string);
    procedure RefMemberName();
    function GetRankName2(PlayObject: TPlayObject; var nRankNo: Integer): string; overload;
    function GetRankName(UserName: string; var nRankNo: Integer): string; overload;
    function DelMember(sHumName: string): Boolean;
    function UpdateRank(sRankData: string): Integer;
    function CancelGuld(sHumName: string): Boolean;
    function AllyGuild(Guild: TGuild): Boolean;
    function AddWarGuild(Guild: TGuild): pTWarGuild;
    function AddAttentionGuild(Guild: TGuild): Boolean;
    function AddRequestAllListGuild(Guild: TGuild): Boolean;
    function AddMember(PlayObject: TPlayObject): Boolean;
    function AddMember2(UserName: string): Boolean;
    function DelHumanObj(PlayObject: TPlayObject): Boolean;
    function GetChiefName(): string;
    procedure BackupGuildFile();
    procedure StopWarGuild(Guild: TGuild);
    procedure StartTeamFight();
    procedure EndTeamFight();
    procedure AddTeamFightMember(sHumanName: string);
    function GetGuildMasterCount: Integer;
    procedure GetGuildMaster(var Master1, Master2: TPlayObject);
    procedure GetGuildMasterName(var Master1, Master2: string);
    procedure UpdateJoinCondition(AJob: Integer; ALevel: Integer; AMsg: string);
    procedure AddWantAddMember(Player: TPlayObject);
    procedure ClearWantAddMembers;
    function DeleteWantAddMember(UserName: string): Boolean;
    function FindWantAddMember(UserName: string): PWantAddMember;
    function FindWantAddMemberIndex(UserName: string): Integer;
    function IsExemptedPersonnel(const UserName: string): Boolean;
    procedure AddExemptedPersonnel(const UserName: string);

    property Count: Integer read GetMemberCount;
    property OnlineCount: Integer read GetOnlineMemberCount;
    property IsFull: Boolean read GetMemberIsFull;
    property nBuildPoint: Integer read m_nBuildPoint write SetBuildPoint;
    property nAurae: Integer read m_nAurae write SetAuraePoint;
    property nStability: Integer read m_nStability write SetStabilityPoint;
    property nFlourishing: Integer read m_nFlourishing write SetFlourishPoint;
    property nChiefItemCount: Integer read m_nChiefItemCount write SetChiefItemCount;
    property JoinJob: Integer read m_JoinJob;
    property JoinLevel: LongWord read m_JoinLevel;
    property JoinMsg: string read m_JoinMsg;
    property WantAddMemberCount: Integer read GetWantAddMemberCount;
    property WantAddMemebers[Index: Integer]: PWantAddMember read GetWantAddMemebers;
    property EnabledAlly: Boolean read m_EnabledAlly write SetEnabledAlly;
  end;

  TGuildManager = class
    GuildList: TList; // 0x4
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadGuildInfo();
    procedure SaveGuildList();
    function MemberOfGuild(sName: string): TGuild;
    function AddGuild(sGuildName, sChief: string): Boolean;
    function FindGuild(sGuildName: string): TGuild;
    function DelGuild(sGuildName: string; var IsFound: Boolean): Boolean;
    procedure ClearGuildInf();
    procedure Run();
    procedure SaveGuildVariable();
  end;

implementation

uses
  M2Share, HUtil32, Grobal2;

{ TGuildManager }
function TGuildManager.AddGuild(sGuildName, sChief: string): Boolean; // 0049A4A4
var
  Guild: TGuild;
begin
  Result := False;
  if CheckGuildName(sGuildName) and (FindGuild(sGuildName) = nil) then
  begin
    Guild := TGuild.Create(sGuildName);
    Guild.SetGuildInfo(sChief);
    GuildList.Add(Guild);
    SaveGuildList();
    Result := True;

    g_GrobalPlayer.m_MyGuild := Guild;
    if g_ManageNPC <> nil then
    begin
      TPlayObject(g_GrobalPlayer).m_nScriptGotoCount := 0;
      g_ManageNPC.GotoLable(TPlayObject(g_GrobalPlayer), '@LoadGuild', False);
    end;
    g_GrobalPlayer.m_MyGuild := nil;
  end;
end;

function TGuildManager.DelGuild(sGuildName: string; var IsFound: Boolean): Boolean; // 0049A550
var
  i: Integer;
  Guild: TGuild;
begin
  Result := False;
  IsFound := False;
  for i := 0 to GuildList.Count - 1 do
  begin
    Guild := TGuild(GuildList.Items[i]);
    if CompareText(Guild.sGuildName, sGuildName) = 0 then
    begin
      IsFound := True;
      if Guild.m_RankList.Count > 1 then
        Break;
      Guild.BackupGuildFile();
      GuildList.Delete(i);
      SaveGuildList();
      Result := True;
      Break;
    end;
  end;
end;

procedure TGuildManager.ClearGuildInf; // 0049A02C
var
  i: Integer;
begin
  for i := 0 to GuildList.Count - 1 do
  begin
    TGuild(GuildList.Items[i]).Free;
  end;
  GuildList.Clear;
end;

constructor TGuildManager.Create;
begin
  GuildList := TList.Create;
end;

destructor TGuildManager.Destroy;
var
  i: Integer;
begin
  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-20】 }
  for i := 0 to GuildList.Count - 1 do
    TGuild(GuildList.Items[i]).Free;

  GuildList.Clear;
  GuildList.Free;

  inherited;
end;

function TGuildManager.FindGuild(sGuildName: string): TGuild; // 0049A36C
var
  i: Integer;
begin
  Result := nil;
  for i := 0 to GuildList.Count - 1 do
  begin
    if TGuild(GuildList.Items[i]).sGuildName = sGuildName then
    begin
      Result := TGuild(GuildList.Items[i]);
      Break;
    end;
  end;
end;

// modify chongchong 2013-07-20
procedure TGuildManager.LoadGuildInfo; // 0049A078
var
  i: Integer;
  Guild: TGuild;
  sGuildName: string;
  LoadList: TStringList;
begin
  if not FileExists(g_Config.sGuildFile) then
  begin
    MainOutMessage('行会信息文件未找到！');
    Exit;
  end;

  LoadList := TStringList.Create;
  try
    LoadList.LoadFromFile(g_Config.sGuildFile);
    for i := 0 to LoadList.Count - 1 do
    begin
      sGuildName := Trim(LoadList.Strings[i]);
      if sGuildName <> '' then
      begin
        Guild := TGuild.Create(sGuildName);
        GuildList.Add(Guild);
      end;
    end;
  finally
    LoadList.Free;
  end;

  for i := GuildList.Count - 1 downto 0 do
  begin
    Guild := GuildList.Items[i];
    if not Guild.LoadGuild() then
    begin
      MainOutMessage(Guild.sGuildName + ' 读取出错！');
      Guild.Free;
      GuildList.Delete(i);
      SaveGuildList();
    end;
  end;
  MainOutMessage('已读取 ' + IntToStr(GuildList.Count) + '个行会信息.');
end;

function TGuildManager.MemberOfGuild(sName: string): TGuild;
var
  i: Integer;
begin
  Result := nil;
  for i := 0 to GuildList.Count - 1 do
  begin
    if TGuild(GuildList.Items[i]).IsMember(sName) then
    begin
      Result := TGuild(GuildList.Items[i]);
      Break;
    end;
  end;
end;

procedure TGuildManager.SaveGuildList; // 0049A260
var
  i: Integer;
  SaveList: TStringList;
begin
  if nServerIndex <> 0 then
    Exit;
  SaveList := TStringList.Create;
  for i := 0 to GuildList.Count - 1 do
  begin
    SaveList.Add(TGuild(GuildList.Items[i]).sGuildName);
  end; // for
  try
    SaveList.SaveToFile(g_Config.sGuildFile);
  except
    MainOutMessage('行会信息保存失败！');
  end;
  SaveList.Free;
end;

procedure TGuildManager.SaveGuildVariable;
var
  i: Integer;
  Guild: TGuild;
begin
  for i := GuildList.Count - 1 downto 0 do
  begin
    Guild := GuildList.Items[i];
    g_GrobalPlayer.m_MyGuild := Guild;
    if g_ManageNPC <> nil then
    begin
      TPlayObject(g_GrobalPlayer).m_nScriptGotoCount := 0;
      g_ManageNPC.GotoLable(TPlayObject(g_GrobalPlayer), '@LoadGuild', False);
    end;
  end;
  g_GrobalPlayer.m_MyGuild := nil;
end;

procedure TGuildManager.Run; // 0049A61C
var
  i: Integer;
  II: Integer;
  Guild: TGuild;
  boChanged: Boolean;
  WarGuild: pTWarGuild;
begin
  for i := 0 to GuildList.Count - 1 do
  begin
    Guild := TGuild(GuildList.Items[i]);
    boChanged := False;
    for II := Guild.GuildWarList.Count - 1 downto 0 do
    begin
      WarGuild := pTWarGuild(Guild.GuildWarList.Objects[II]);
      if (MyGetTickCount - WarGuild.dwWarTick) > WarGuild.dwWarTime then
      begin
        Guild.StopWarGuild(TGuild(WarGuild.Guild));
        Guild.GuildWarList.Delete(II);
        Dispose(WarGuild);
        boChanged := True;
      end;
    end;

    if boChanged then
    begin
      Guild.UpdateGuildFile();

      Guild.RefMemberName();
    end;
    Guild.CheckSaveGuildFile;
  end;
end;

{ TGuild }

procedure TGuild.ClearRank; // 00497C78
var
  i: Integer;
  GuildRank: pTGuildRank;
begin
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    GuildRank.MemberList.Free;
    Dispose(GuildRank);
  end; // for
  m_RankList.Clear;
end;

constructor TGuild.Create(sName: string); // 00497B04
var
  sFileName: string;
begin
  sGuildName := sName;
  NoticeList := TStringList.Create;
  GuildWarList := TStringList.Create;
  GuildAllyList := TStringList.Create;
  GuildAttentionList := TStringList.Create;
  GuildRequestAllList := TStringList.Create;
  m_RankList := TList.Create;
  TeamFightDeadList := TStringList.Create;
  m_ExemptList := TStringList.Create;
  dwSaveTick := 0;
  boChanged := False;
  nContestPoint := 0;
  boTeamFight := False;
  m_boEnableAuthAlly := False;

  sFileName := g_Config.sGuildDir + sName + '.ini';
  m_Config := TIniFile.Create(sFileName);
  if not FileExists(sFileName) then
  begin
    m_Config.WriteString('Guild', 'GuildName', sName);
  end;

  m_nBuildPoint := 0;
  m_nAurae := 0;
  m_nStability := 0;
  m_nFlourishing := 0;
  m_nChiefItemCount := 0;

  // 行会人数限制 piaoyun 2013-07-17
  m_nMemberMaxLimit := g_Config.nGuildMemberMaxLimit;
  m_DynamicVarList := TList.Create;

  m_JoinJob := 7;
  m_JoinLevel := 0;
  m_JoinMsg := '欢迎加入本行会';

  m_EnabledAlly := False;

  FWantAddMemberList := TList.Create;
end;

function TGuild.DelAllyGuild(Guild: TGuild): Boolean; // 00499CEC
var
  i: Integer;
  AllyGuild: TGuild;
begin
  Result := False;
  for i := 0 to GuildAllyList.Count - 1 do
  begin
    AllyGuild := TGuild(GuildAllyList.Objects[i]);
    if AllyGuild = Guild then
    begin
      GuildAllyList.Delete(i);
      Result := True;
      Break;
    end;
  end; // for
  SaveGuildInfoFile();
end;

function TGuild.DelAttentionGuild(Guild: TGuild): Boolean;
var
  i: Integer;
  AttentionGuild: TGuild;
begin
  Result := False;
  for i := 0 to GuildAttentionList.Count - 1 do
  begin
    AttentionGuild := TGuild(GuildAttentionList.Objects[i]);
    if AttentionGuild = Guild then
    begin
      GuildAttentionList.Delete(i);
      Result := True;
      Break;
    end;
  end; // for
  SaveGuildInfoFile();
end;

function TGuild.DelRequestAllListGuild(Guild: TGuild): Boolean;
var
  i: Integer;
  RequestAllListGuild: TGuild;
begin
  Result := False;
  for i := 0 to GuildRequestAllList.Count - 1 do
  begin
    RequestAllListGuild := TGuild(GuildRequestAllList.Objects[i]);
    if RequestAllListGuild = Guild then
    begin
      GuildRequestAllList.Delete(i);
      Result := True;
      Break;
    end;
  end;
  SaveGuildInfoFile();
end;

destructor TGuild.Destroy;
var
  i: Integer;
begin
  NoticeList.Free;
  GuildWarList.Free;
  GuildAllyList.Free;
  ClearRank();
  m_RankList.Free;
  TeamFightDeadList.Free;
  m_Config.Free;
  m_ExemptList.Free;

  GuildAttentionList.Free;
  GuildRequestAllList.Free;

  for i := 0 to m_DynamicVarList.Count - 1 do
  begin
    Dispose(pTDynamicVar(m_DynamicVarList.Items[i]));
  end;
  m_DynamicVarList.Free;

  ClearWantAddMembers;
  FWantAddMemberList.Free;
  inherited;
end;

function TGuild.IsAllyGuild(AOtherGuild: TGuild): Boolean;
begin
  Result := (AOtherGuild <> nil) and (GuildAllyList.IndexOf(AOtherGuild.sGuildName) >= 0);
end;

function TGuild.IsWarGuild(AOtherGuild: TGuild): Boolean; // Cursor 2023-05-27 16:57:40 (重写此函数)
var
  i, j: Integer;
  tmpSelfAlly, tmpOtherAlly: TGuild;
begin
  Result := False;

  // 自己的行会与目标行会敌对
  if GuildWarList.IndexOf(AOtherGuild.sGuildName) >= 0 then
  begin
    Result := True;
    Exit;
  end;

  // 自己的行会与目标的联盟行会敌对
  for i := 0 to AOtherGuild.GuildAllyList.Count - 1 do
  begin
    tmpOtherAlly := TGuild(AOtherGuild.GuildAllyList.Objects[i]);
    if tmpOtherAlly.GuildWarList.IndexOf(sGuildName) >= 0 then
    begin
      Result := True;
      Exit;
    end;
  end;

  // 自己的联盟行会与目标行会敌对
  for i := 0 to GuildAllyList.Count - 1 do
  begin
    tmpSelfAlly := TGuild(GuildAllyList.Objects[i]);
    if tmpSelfAlly.GuildWarList.IndexOf(AOtherGuild.sGuildName) >= 0 then
    begin
      Result := True;
      Exit;
    end;
  end;

  // 自己的联盟行会与对方的联盟行会敌对
  for i := 0 to GuildAllyList.Count - 1 do
  begin
    tmpSelfAlly := TGuild(GuildAllyList.Objects[i]);
    for j := 0 to AOtherGuild.GuildAllyList.Count - 1 do
    begin
      tmpOtherAlly := TGuild(AOtherGuild.GuildAllyList.Objects[j]);
      if tmpSelfAlly.GuildWarList.IndexOf(tmpOtherAlly.sGuildName) >= 0 then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function TGuild.IsAttentionGuild(AOtherGuild: TGuild): Boolean;
var
  i: Integer;
  AttentionGuild: TGuild;
begin
  Result := False;
  for i := 0 to GuildAttentionList.Count - 1 do
  begin
    AttentionGuild := TGuild(GuildAttentionList.Objects[i]);
    if AttentionGuild = AOtherGuild then
    begin
      Result := True;
      Break;
    end;
  end;
end;

procedure TGuild.AddExemptedPersonnel(const UserName: string);
begin
  if m_ExemptList.IndexOf(UserName) < 0 then
    m_ExemptList.Add(UserName);
end;

function TGuild.IsExemptedPersonnel(const UserName: string): Boolean;
var
  i: Integer;
begin
  i := m_ExemptList.IndexOf(UserName);
  if i >= 0 then
    m_ExemptList.Delete(i);

  Result := i >= 0;
end;

function TGuild.IsRequestAllListGuild(Guild: TGuild): Boolean;
begin
  Result := GuildRequestAllList.IndexOf(Guild.sGuildName) >= 0;
end;

function TGuild.IsMember(sName: string): Boolean; // 00498714
var
  i, II: Integer;
  GuildRank: pTGuildRank;
begin
  Result := False;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      if GuildRank.MemberList.Strings[II] = sName then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function TGuild.LoadGuild(): Boolean; // 00497CE4
var
  sFileName: string;
begin
  sFileName := sGuildName + '.txt';
  Result := LoadGuildFile(sFileName);
  LoadGuildConfig(sGuildName + '.ini');
  LoadGuildWantAddMembers(g_Config.sGuildDir + sGuildName + '.add');
end;

function TGuild.LoadGuildConfig(sGuildFileName: string): Boolean;
begin
  m_nBuildPoint := m_Config.ReadInteger('Guild', 'BuildPoint', m_nBuildPoint);
  m_nAurae := m_Config.ReadInteger('Guild', 'Aurae', m_nAurae);
  m_nStability := m_Config.ReadInteger('Guild', 'Stability', m_nStability);
  m_nFlourishing := m_Config.ReadInteger('Guild', 'Flourishing', m_nFlourishing);
  m_nChiefItemCount := m_Config.ReadInteger('Guild', 'ChiefItemCount', m_nChiefItemCount);
  m_nMemberMaxLimit := m_Config.ReadInteger('Guild', 'MemberMaxLimit', m_nMemberMaxLimit);
  if m_nMemberMaxLimit <= 0 then
    m_nMemberMaxLimit := g_Config.nGuildMemberMaxLimit;

  m_JoinJob := m_Config.ReadInteger('Guild', 'JoinJob', m_JoinJob);
  m_JoinLevel := m_Config.ReadInteger('Guild', 'JoinLevel', m_JoinLevel);
  m_JoinMsg := m_Config.ReadString('Guild', 'JoinMsg', m_JoinMsg);

  m_EnabledAlly := m_Config.ReadBool('Guild', 'EnabledAlly', m_EnabledAlly);

  Result := True;
end;

function TGuild.LoadGuildWantAddMembers(sGuildFileName: string): Boolean;
var
  sLine, sUserName, sSex, sJob, sLevel, sAddTime: string;
  SL: TStringList;
  i: Integer;
  AddMember: PWantAddMember;
  DateFmt: TFormatSettings;
begin
  Result := False;

  if not FileExists(sGuildFileName) then
    Exit;

  SL := TStringList.Create;
  try
    try
      SL.LoadFromFile(sGuildFileName, TEncoding.ANSI);
      for i := 0 to SL.Count - 1 do
      begin
        sLine := SL.Strings[i];
        if Length(sLine) > 0 then
        begin
          sLine := GetValidStr3_Ex(sLine, sUserName, #9);
          sLine := GetValidStr3_Ex(sLine, sSex, #9);
          sLine := GetValidStr3_Ex(sLine, sJob, #9);
          sLine := GetValidStr3_Ex(sLine, sLevel, #9);
          sLine := GetValidStr3_Ex(sLine, sAddTime, #9);

          if Length(sUserName) > 0 then
          begin
            New(AddMember);
            FWantAddMemberList.Add(AddMember);

            AddMember.UserName := sUserName;
            AddMember.Sex := StrToIntDef(sSex, 0);
            AddMember.Job := StrToIntDef(sJob, 0);
            AddMember.Level := StrToIntDef(sLevel, 0);

{$IF CompilerVersion >= 22}
            DateFmt := TFormatSettings.Create(GetThreadLocale);
{$ELSE}
            GetLocaleFormatSettings(GetThreadLocale, DateFmt);
{$IFEND}
            DateFmt.DateSeparator := '-';
            DateFmt.TimeSeparator := ':';

            AddMember.AddTime := StrToDateTime(sAddTime, DateFmt);
          end;
        end;
      end;
    except
      MainOutMessage('打开文件出错 文件:' + sGuildFileName);
    end;
  finally
    SL.Free;
  end;

  Result := True;
end;

function TGuild.LoadGuildFile(sGuildFileName: string): Boolean;
var
  i, II, III, IIII: Integer;
  LoadList: TStringList;
  s18, s1C, s20, s24, sFileName: string;
  n28, n2C: Integer;
  GuildWar: pTWarGuild;
  GuildRank: pTGuildRank;
  NewGuildRank: pTGuildRank;
  Guild: TGuild;
  boCheckChange: Boolean;
  boSaveChange: Boolean;
begin
  Result := False;
  GuildRank := nil;
  sFileName := g_Config.sGuildDir + sGuildFileName;
  if not FileExists(sFileName) then
    Exit;

  ClearRank();
  NoticeList.Clear;
  for i := 0 to GuildWarList.Count - 1 do
    Dispose(pTWarGuild(GuildWarList.Objects[i]));

  GuildWarList.Clear;
  GuildAllyList.Clear;
  GuildAttentionList.Clear;
  GuildRequestAllList.Clear;
  n28 := 0;
  s24 := '';
  LoadList := TStringList.Create;
  LoadList.LoadFromFile(sFileName);
  for i := 0 to LoadList.Count - 1 do
  begin
    s18 := LoadList.Strings[i];
    if (s18 = '') or (s18[1] = ';') then
      Continue;

    if s18[1] <> '+' then
    begin
      if s18 = g_Config.sGuildNotice then
        n28 := 1;
      if s18 = g_Config.sGuildWar then
        n28 := 2;
      if s18 = g_Config.sGuildAll then
        n28 := 3;
      if s18 = g_Config.sGuildMember then
        n28 := 4;
      if s18 = g_Config.sGuildAttention then
        n28 := 5;
      if s18 = g_Config.sGuildRequestAllList then
        n28 := 6;
      if s18[1] = '#' then
      begin
        s18 := Copy(s18, 2, Length(s18) - 1);
        s18 := GetValidStr3(s18, s1C, [' ', ',']);
        n2C := Str_ToInt(s1C, 0);
        s24 := Trim(s18);

        // ++++++ 只有分组没有成员时，分组不读取 chongchong 2015-08-25
        if (n28 = 4) and (n2C > 0) and (s24 <> '') then
        begin
          if Length(s24) > g_Config.nGuildRankNameLen then // 行会封号长度限制 chongchong 2013-08-24
            s24 := Copy(s24, 1, g_Config.nGuildRankNameLen { 30 } );

          New(GuildRank);
          GuildRank.nRankNo := n2C;
          GuildRank.sRankName := s24;
          GuildRank.MemberList := TStringList.Create;
          m_RankList.Add(GuildRank);
        end;
      end;
      Continue;
    end;

    s18 := Copy(s18, 2, Length(s18) - 1);
    case n28 of
      1:
        NoticeList.Add(s18);
      2:
        begin
          while (s18 <> '') do
          begin
            s18 := GetValidStr3(s18, s1C, [' ', ',']);
            if s1C = '' then
              Break;
            New(GuildWar);
            GuildWar.Guild := g_GuildManager.FindGuild(s1C);
            if GuildWar.Guild <> nil then
            begin
              GuildWar.dwWarTick := MyGetTickCount();
              GuildWar.dwWarTime := Str_ToInt(Trim(s20), 0);
              GuildWarList.AddObject(TGuild(GuildWar.Guild).sGuildName, TObject(GuildWar));
            end
            else
            begin
              Dispose(GuildWar);
            end;
          end;
        end;
      3:
        begin
          while (s18 <> '') do
          begin
            s18 := GetValidStr3(s18, s1C, [' ', ',']);
            s18 := GetValidStr3(s18, s20, [' ', ',']);
            if s1C = '' then
              Break;
            Guild := g_GuildManager.FindGuild(s1C);
            if Guild <> nil then
              GuildAllyList.AddObject(s1C, Guild);
          end;
        end;
      4:
        begin
          // 修改 只有分组没有成员时，分组不读取 chongchong 2015-08-25
          (*
            if (n2C > 0) and (s24 <> '') then
            begin
            if GuildRank = nil then
            begin
            // 行会封号长度限制 chongchong 2013-08-24
            if Length(s24) > g_Config.nGuildRankNameLen then
            s24 := Copy(s24, 1, g_Config.nGuildRankNameLen {30});

            New(GuildRank);
            GuildRank.nRankNo := n2C;
            GuildRank.sRankName := s24;
            GuildRank.MemberList := TStringList.Create;
            m_RankList.Add(GuildRank);
            end;

            while (s18 <> '') do
            begin
            s18 := GetValidStr3(s18, s1C, [' ', ',']);
            if s1C = '' then Break;
            GuildRank.MemberList.Add(s1C);
            end;
            end;
          *)

          if GuildRank <> nil then
          begin
            while (s18 <> '') do
            begin
              s18 := GetValidStr3(s18, s1C, [' ', ',']);
              if s1C = '' then
                Break;
              GuildRank.MemberList.Add(s1C);
            end;
          end;
        end;
      5:
        begin
          while (s18 <> '') do
          begin
            s18 := GetValidStr3(s18, s1C, [' ', ',']);
            s18 := GetValidStr3(s18, s20, [' ', ',']);
            if s1C = '' then
              Break;
            Guild := g_GuildManager.FindGuild(s1C);
            if Guild <> nil then
              GuildAttentionList.AddObject(s1C, Guild);
          end;
        end;
      6:
        begin
          while (s18 <> '') do
          begin
            s18 := GetValidStr3(s18, s1C, [' ', ',']);
            s18 := GetValidStr3(s18, s20, [' ', ',']);
            if s1C = '' then
              Break;
            Guild := g_GuildManager.FindGuild(s1C);
            if Guild <> nil then
              GuildRequestAllList.AddObject(s1C, Guild);
          end;
        end;

    end; // case
  end;
  LoadList.Free;
  boSaveChange := False;

  // 增加 清除重复人物
  i := 0;
  while True do
  begin
    if i >= m_RankList.Count then
      Break;
    GuildRank := m_RankList.Items[i];
    II := 0;
    while True do
    begin
      if II >= GuildRank.MemberList.Count then
        Break;
      boCheckChange := False;
      III := 0;
      n2C := 0;
      while True do
      begin
        if III >= m_RankList.Count then
          Break;
        NewGuildRank := m_RankList.Items[III];
        IIII := 0;
        while True do
        begin
          if IIII >= NewGuildRank.MemberList.Count then
            Break;
          if GuildRank.MemberList.Strings[II] = NewGuildRank.MemberList.Strings[IIII] then
            Inc(n2C);
          if n2C >= 2 then
          begin
            if not boSaveChange then
              boSaveChange := True;
            boCheckChange := True;
            NewGuildRank.MemberList.Delete(IIII);
            Dec(n2C);
            Continue;
          end;
          Inc(IIII);
        end;
        Inc(III);
      end;
      if boCheckChange then
        Continue;
      Inc(II);
    end;
    Inc(i);
  end;

  if boSaveChange then
    SaveGuildInfoFile();
  Result := True;
end;

procedure TGuild.RefMemberName; // 00498F60
var
  i, II: Integer;
  GuildRank: pTGuildRank;
  BaseObject: TBaseObject;
begin
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      BaseObject := TBaseObject(GuildRank.MemberList.Objects[II]);
      if BaseObject <> nil then
      begin
        BaseObject.RefShowName;
        if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
        begin
          if TPlayObject(BaseObject).m_MyHero <> nil then
          begin
            THeroObject(TPlayObject(BaseObject).m_MyHero).RefShowName;
          end;
        end;
      end;
    end;
  end;
end;

procedure TGuild.SaveGuildInfoFile; // 004985EC
begin
  if nServerIndex = 0 then
  begin
    SaveGuildFile(g_Config.sGuildDir + sGuildName + '.txt');
    SaveGuildConfig(g_Config.sGuildDir + sGuildName + '.ini');
    SaveGuildWantAddMembers(g_Config.sGuildDir + sGuildName + '.add');
  end
  else
  begin
    SaveGuildFile(g_Config.sGuildDir + sGuildName + '.' + IntToStr(nServerIndex));
  end;
end;

procedure TGuild.SaveGuildConfig(sFileName: string);
begin
  m_Config.WriteString('Guild', 'GuildName', sGuildName);
  m_Config.WriteInteger('Guild', 'BuildPoint', m_nBuildPoint);
  m_Config.WriteInteger('Guild', 'Aurae', m_nAurae);
  m_Config.WriteInteger('Guild', 'Stability', m_nStability);
  m_Config.WriteInteger('Guild', 'Flourishing', m_nFlourishing);
  m_Config.WriteInteger('Guild', 'ChiefItemCount', m_nChiefItemCount);
  m_Config.WriteInteger('Guild', 'MemberMaxLimit', m_nMemberMaxLimit);
  m_Config.WriteInteger('Guild', 'JoinJob', m_JoinJob);
  m_Config.WriteInteger('Guild', 'JoinLevel', m_JoinLevel);
  m_Config.WriteString('Guild', 'JoinMsg', m_JoinMsg);
  m_Config.WriteBool('Guild', 'EnabledAlly', m_EnabledAlly);
end;

procedure TGuild.SaveGuildWantAddMembers(sFileName: string);
var
  i: Integer;
  AddMember: PWantAddMember;
  SL: TStringList;
begin
  SL := TStringList.Create;
  try
    for i := 0 to FWantAddMemberList.Count - 1 do
    begin
      AddMember := FWantAddMemberList.Items[i];
      SL.Add(AddMember.UserName + #9 + IntToStr(AddMember.Sex) + #9 + IntToStr(AddMember.Job) + #9 + IntToStr(AddMember.Level) +
        #9 + FormatDateTime('yyyy/mm/dd hh:nn:ss', AddMember.AddTime));
    end;

    SL.SaveToFile(sFileName);
  finally
    SL.Free;
  end;
end;

procedure TGuild.SaveGuildFile(sFileName: string);
var
  SaveList: TStringList;
  i, II: Integer;
  WarGuild: pTWarGuild;
  GuildRank: pTGuildRank;
  n14: Integer;
begin
  SaveList := TStringList.Create;
  SaveList.Add(g_Config.sGuildNotice);
  for i := 0 to NoticeList.Count - 1 do
  begin
    SaveList.Add('+' + NoticeList.Strings[i]);
  end;
  SaveList.Add(' ');
  SaveList.Add(g_Config.sGuildWar);
  for i := 0 to GuildWarList.Count - 1 do
  begin
    WarGuild := pTWarGuild(GuildWarList.Objects[i]);
    n14 := WarGuild.dwWarTime - (MyGetTickCount - WarGuild.dwWarTick);
    if n14 <= 0 then
      Continue;
    SaveList.Add('+' + GuildWarList.Strings[i] + ' ' + IntToStr(n14));
  end;
  SaveList.Add(' ');
  SaveList.Add(g_Config.sGuildAll);
  for i := 0 to GuildAllyList.Count - 1 do
  begin
    SaveList.Add('+' + GuildAllyList.Strings[i]);
  end;

  SaveList.Add(' ');
  SaveList.Add(g_Config.sGuildAttention);
  for i := 0 to GuildAttentionList.Count - 1 do
  begin
    SaveList.Add('+' + GuildAttentionList.Strings[i]);
  end;

  SaveList.Add(' ');
  SaveList.Add(g_Config.sGuildRequestAllList);
  for i := 0 to GuildRequestAllList.Count - 1 do
  begin
    SaveList.Add('+' + GuildRequestAllList.Strings[i]);
  end;

  SaveList.Add(' ');
  SaveList.Add(g_Config.sGuildMember);
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    SaveList.Add('#' + IntToStr(GuildRank.nRankNo) + ' ' + GuildRank.sRankName);
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      SaveList.Add('+' + GuildRank.MemberList.Strings[II]);
    end;
  end;
  try
    SaveList.SaveToFile(sFileName);
  except
    MainOutMessage('保存行会信息失败！ ' + sFileName);
  end;
  SaveList.Free;
end;

procedure TGuild.SendGuildMsg(sMsg: string); // 00498FF0
var
  i: Integer;
  II: Integer;
  GuildRank: pTGuildRank;
  PlayObject: TPlayObject;
  nCheckCode: Integer;
  sOldMsg: string;
begin
  nCheckCode := 0;
  try
    sOldMsg := sMsg;
    if g_Config.boShowPreFixMsg then
      sMsg := g_Config.sGuildMsgPreFix + sMsg;

    nCheckCode := 1;
    for i := 0 to m_RankList.Count - 1 do
    begin
      GuildRank := m_RankList.Items[i];
      nCheckCode := 2;

      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        nCheckCode := 3;
        PlayObject := TPlayObject(GuildRank.MemberList.Objects[II]);
        if PlayObject = nil then
          Continue;

        nCheckCode := 4;
        if PlayObject.m_boBanGuildChat then
        begin
          nCheckCode := 5;
          PlayObject.SendMsg(PlayObject, RM_GUILDMESSAGE, 0, g_Config.btGuildMsgFColor, g_Config.btGuildMsgBColor, 0, sMsg);
          nCheckCode := 6;
        end;
      end;
    end;

    if nCheckCode = 6 then
    begin
      // 记录行会聊天信息 chongchong 2013-07-23
      if g_Config.boRecordGuildMsg and (Length(sOldMsg) > 0) then
        MainOutMessage('[行会] ' + sGuildName + ':' + sOldMsg);
    end;
  except
    on e: Exception do
    begin
      MainOutMessage('[Exceptiion] TGuild:SendGuildMsg CheckCode: ' + IntToStr(nCheckCode) + ' GuildName = ' + sGuildName +
        ' Msg = ' + sMsg);
      MainOutMessage(e.Message);
    end;
  end;
end;

procedure TGuild.SendGuildMsgEx(sMsg: string);
var
  i: Integer;
  II: Integer;
  GuildRank: pTGuildRank;
  PlayObject: TPlayObject;
  nCheckCode: Integer;
  sOldMsg: string;
begin
  nCheckCode := 0;
  try
    sOldMsg := sMsg;
    if g_Config.boShowPreFixMsg then
      sMsg := g_Config.sGuildMsgPreFix + sMsg;
    // if RankList = nil then exit;
    nCheckCode := 1;
    for i := 0 to m_RankList.Count - 1 do
    begin
      GuildRank := m_RankList.Items[i];
      nCheckCode := 2;
      // if GuildRank.MemberList = nil then Continue;
      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        nCheckCode := 3;
        PlayObject := TPlayObject(GuildRank.MemberList.Objects[II]);
        if PlayObject = nil then
          Continue;
        nCheckCode := 4;
        if PlayObject.m_boBanGuildChat then
        begin
          nCheckCode := 5;
          PlayObject.m_dwSayAdvertiseTick := MyGetTickCount;
          PlayObject.SendMsg(PlayObject, RM_GUILDMESSAGE, 0, g_Config.btGuildMsgFColor, g_Config.btGuildMsgBColor, 0, sMsg);
          nCheckCode := 6;
        end;
      end;
    end;

    if nCheckCode = 6 then
    begin
      // 记录行会聊天信息 chongchong 2013-07-23
      if g_Config.boRecordGuildMsg and (Length(sOldMsg) > 0) then
      begin
        MainOutMessage('[行会] ' + sGuildName + ':' + sOldMsg);
      end;
    end;
    (*
      TGuild.SendGuildMsg CheckCode: 5 GuildName = 〖統治〗 Msg = 〖行会〗釢fěη﹖: 换的玩撒
      2004-12-2 15:45:48 Access violation at address 0041FD64 in module 'M2Server.exe'. Read of address 00000008
    *);
  except
    on e: Exception do
    begin
      MainOutMessage('[Exceptiion] TGuild:SendGuildMsg CheckCode: ' + IntToStr(nCheckCode) + ' GuildName = ' + sGuildName +
        ' Msg = ' + sMsg);
      MainOutMessage(e.Message);
    end;
  end;
end;

function TGuild.SetGuildInfo(sChief: string): Boolean; // 00498984
var
  GuildRank: pTGuildRank;
begin
  if m_RankList.Count = 0 then
  begin
    New(GuildRank);
    GuildRank.nRankNo := 1;
    GuildRank.sRankName := g_Config.sGuildChief;
    GuildRank.MemberList := TStringList.Create;
    GuildRank.MemberList.Add(sChief);
    m_RankList.Add(GuildRank);
    SaveGuildInfoFile();
  end;
  Result := True;
end;

function TGuild.GetRankName2(PlayObject: TPlayObject; var nRankNo: Integer): string; // 004987F0
var
  i, II: Integer;
  GuildRank: pTGuildRank;
begin
  Result := '';
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      if SameText(GuildRank.MemberList.Strings[II], PlayObject.m_sCharName) then
      begin
        GuildRank.MemberList.Objects[II] := PlayObject;
        nRankNo := GuildRank.nRankNo;
        Result := GuildRank.sRankName;
        // PlayObject.RefShowName();
        PlayObject.SendMsg(PlayObject, RM_CHANGEGUILDNAME, 0, 0, 0, 0, '');
        Exit;
      end;
    end; // for
  end;
end;

function TGuild.GetRankName(UserName: string; var nRankNo: Integer): string; // 004987F0
var
  i, II: Integer;
  GuildRank: pTGuildRank;
begin
  Result := '';
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      if SameText(GuildRank.MemberList.Strings[II], UserName) then
      begin
        nRankNo := GuildRank.nRankNo;
        Result := GuildRank.sRankName;
        Exit;
      end;
    end; // for
  end;
end;

function TGuild.GetChiefName: string; // 00498928
var
  GuildRank: pTGuildRank;
begin
  Result := '';
  if m_RankList.Count <= 0 then
    Exit;
  GuildRank := m_RankList.Items[0];
  if GuildRank.MemberList.Count <= 0 then
    Exit;
  Result := GuildRank.MemberList.Strings[0];
end;

procedure TGuild.CheckSaveGuildFile();
begin
  if boChanged and ((MyGetTickCount - dwSaveTick) > 30 * 1000) then
  begin
    boChanged := False;
    SaveGuildInfoFile();
  end;
end;

function TGuild.GetGuildMasterCount: Integer;
var
  i: Integer;
  GuildRank: pTGuildRank;
begin
  Result := 0;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    if GuildRank.nRankNo = 1 then
    begin
      Result := GuildRank.MemberList.Count;
    end;
  end;

end;

procedure TGuild.GetGuildMaster(var Master1, Master2: TPlayObject);
var
  i: Integer;
  GuildRank: pTGuildRank;
begin
  Master1 := nil;
  Master2 := nil;

  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    if GuildRank.nRankNo = 1 then
    begin
      if GuildRank.MemberList.Count > 0 then
      begin
        Master1 := TPlayObject(GuildRank.MemberList.Objects[0]);
        if GuildRank.MemberList.Count > 1 then
          Master2 := TPlayObject(GuildRank.MemberList.Objects[1]);
        Exit;
      end;
    end;
  end;
end;

procedure TGuild.GetGuildMasterName(var Master1, Master2: string);
var
  i: Integer;
  GuildRank: pTGuildRank;
begin
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    if GuildRank.nRankNo = 1 then
    begin
      if GuildRank.MemberList.Count > 0 then
      begin
        Master1 := GuildRank.MemberList.Strings[0];
        if GuildRank.MemberList.Count > 1 then
          Master2 := GuildRank.MemberList.Strings[1];
        Exit;
      end;
    end;
  end;
end;

function TGuild.DelHumanObj(PlayObject: TPlayObject): Boolean; // 00498ECC
var
  i, II: Integer;
  GuildRank: pTGuildRank;
begin
  Result := False;
  CheckSaveGuildFile();
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      if TPlayObject(GuildRank.MemberList.Objects[II]) = PlayObject then
      begin
        GuildRank.MemberList.Objects[II] := nil;
        Result := True;
        Exit;
      end;
    end;
  end;
end;

procedure TGuild.TeamFightWhoDead(sName: string); // 00499EC8
var
  i, n10: Integer;
begin
  if not boTeamFight then
    Exit;
  if TeamFightDeadList = nil then
    Exit;

  for i := 0 to TeamFightDeadList.Count - 1 do
  begin
    if TeamFightDeadList.Strings[i] = sName then
    begin
      n10 := Integer(TeamFightDeadList.Objects[i]);
      TeamFightDeadList.Objects[i] := TObject(MakeLong(LoWord(n10) + 1, HiWord(n10)));
    end;
  end;
end;

procedure TGuild.TeamFightWhoWinPoint(sName: string; nPoint: Integer); // 00499DE4
var
  i, n14: Integer;
begin
  if not boTeamFight then
    Exit;
  Inc(nContestPoint, nPoint);
  for i := 0 to TeamFightDeadList.Count - 1 do
  begin
    if TeamFightDeadList.Strings[i] = sName then
    begin
      n14 := Integer(TeamFightDeadList.Objects[i]);
      TeamFightDeadList.Objects[i] := TObject(MakeLong(LoWord(n14), HiWord(n14) + nPoint));
    end;
  end;
end;

procedure TGuild.UpdateGuildFile();
begin
  boChanged := True;
  dwSaveTick := MyGetTickCount();
  SaveGuildInfoFile();
end;

procedure TGuild.BackupGuildFile; // 00498AFC
var
  i, II: Integer;
  PlayObject: TPlayObject;
  GuildRank: pTGuildRank;
begin
  if nServerIndex = 0 then
    SaveGuildFile(g_Config.sGuildDir + sGuildName + '.' + IntToStr(MyGetTickCount) + '.bak');
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      PlayObject := TPlayObject(GuildRank.MemberList.Objects[II]);
      if PlayObject <> nil then
      begin
        PlayObject.m_MyGuild := nil;
        PlayObject.RefRankInfo(0, '');
        PlayObject.RefShowName(); // 10/31
      end;
    end;
    GuildRank.MemberList.Free;
    Dispose(GuildRank);
  end;
  m_RankList.Clear;
  NoticeList.Clear;
  for i := 0 to GuildWarList.Count - 1 do
  begin
    Dispose(pTWarGuild(GuildWarList.Objects[i]));
  end;
  GuildWarList.Clear;
  GuildAllyList.Clear;
  GuildAttentionList.Clear;
  GuildRequestAllList.Clear;
  SaveGuildInfoFile();
end;

function TGuild.AddMember(PlayObject: TPlayObject): Boolean; // 00498CA8
var
  i: Integer;
  GuildRank: pTGuildRank;
  GuildRank18: pTGuildRank;
begin
  // if IsFull then Exit;
  GuildRank18 := nil;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    if GuildRank.nRankNo = 99 then
    begin
      GuildRank18 := GuildRank;
      Break;
    end;
  end;
  if GuildRank18 = nil then
  begin
    New(GuildRank18);
    GuildRank18.nRankNo := 99;
    GuildRank18.sRankName := g_Config.sGuildMemberRank;
    GuildRank18.MemberList := TStringList.Create;
    m_RankList.Add(GuildRank18);
  end;
  GuildRank18.MemberList.AddObject(PlayObject.m_sCharName, TObject(PlayObject));
  UpdateGuildFile();
  Result := True;
end;

function TGuild.AddMember2(UserName: string): Boolean;
var
  i: Integer;
  GuildRank: pTGuildRank;
  GuildRank18: pTGuildRank;
begin
  // if IsFull then Exit;
  GuildRank18 := nil;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    if GuildRank.nRankNo = 99 then
    begin
      GuildRank18 := GuildRank;
      Break;
    end;
  end;
  if GuildRank18 = nil then
  begin
    New(GuildRank18);
    GuildRank18.nRankNo := 99;
    GuildRank18.sRankName := g_Config.sGuildMemberRank;
    GuildRank18.MemberList := TStringList.Create;
    m_RankList.Add(GuildRank18);
  end;
  GuildRank18.MemberList.AddObject(UserName, nil);
  UpdateGuildFile();
  Result := True;
end;

function TGuild.DelMember(sHumName: string): Boolean;
var
  i, II: Integer;
  tmpPlayer: TPlayObject;
  GuildRank: pTGuildRank;
begin
  Result := False;

  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];

    for II := 0 to GuildRank.MemberList.Count - 1 do
    begin
      if GuildRank.MemberList.Strings[II] = sHumName then
      begin
        tmpPlayer := TPlayObject(GuildRank.MemberList.Objects[II]);
        if (tmpPlayer <> nil) and (g_FunctionNPC <> nil) then
        begin
          tmpPlayer.m_nScriptGotoCount := 0;
          g_FunctionNPC.GotoLable(tmpPlayer, '@ExitGuildBefore', False);
        end;

        if not IsExemptedPersonnel(sHumName) then
        begin
          GuildRank.MemberList.Delete(II);
          UpdateGuildFile;
          Result := True;
        end;
        Exit;
      end;
    end;
  end;
end;

function TGuild.CancelGuld(sHumName: string): Boolean; // 00498A50
var
  GuildRank: pTGuildRank;
begin
  Result := False;
  if m_RankList.Count <> 1 then
    Exit;
  GuildRank := m_RankList.Items[0];
  if GuildRank.MemberList.Count <> 1 then
    Exit;
  if GuildRank.MemberList.Strings[0] = sHumName then
  begin
    BackupGuildFile();
    Result := True;
  end;
end;

function TGuild.UpdateRank(sRankData: string): Integer;

  procedure ClearRankList(var RankList: TList);
  var
    i: Integer;
    GuildRank: pTGuildRank;
  begin
    for i := 0 to RankList.Count - 1 do
    begin
      GuildRank := RankList.Items[i];
      GuildRank.MemberList.Free;
      Dispose(GuildRank);
    end;
    RankList.Free;
  end;

var
  i, II: Integer;
  III: Integer;
  IIII: Integer;
  GuildRankList: TList;
  GuildRank: pTGuildRank;
  NewGuildRank: pTGuildRank;
  sRankInfo: string;
  sRankNo: string;
  sRankName: string;
  sMemberName: string;
  n28: Integer;
  n2C: Integer;
  n3C: Integer;
  n4C: Integer;
  boCheckChange: Boolean;
  PlayObject: TPlayObject;
begin
  GuildRankList := TList.Create;
  GuildRank := nil;
  n4C := 0;
  while (True) do
  begin
    if sRankData = '' then
      Break;
    sRankData := GetValidStr3(sRankData, sRankInfo, [#$0D]);
    sRankInfo := Trim(sRankInfo);
    if sRankInfo = '' then
      Continue;
    if sRankInfo[1] = '#' then
    begin // 取得职称的名称
      sRankInfo := Copy(sRankInfo, 2, Length(sRankInfo) - 1);
      sRankInfo := GetValidStr3(sRankInfo, sRankNo, [' ', '<']);
      sRankInfo := GetValidStr3(sRankInfo, sRankName, ['<', '>']);
      if Length(sRankName) > g_Config.nGuildRankNameLen then // 行会封号长度限制 chongchong 2013-08-24
        sRankName := Copy(sRankName, 1, g_Config.nGuildRankNameLen);
      if GuildRank <> nil then
      begin
        GuildRankList.Add(GuildRank);
      end;
      New(GuildRank);
      GuildRank.nRankNo := Str_ToInt(sRankNo, 99);
      GuildRank.sRankName := Trim(sRankName);
      GuildRank.MemberList := TStringList.Create;
      Continue;
    end;

    if GuildRank = nil then
      Continue;
    while (True) do
    begin // 将成员名称加入职称表里
      if n4C > m_nMemberMaxLimit then
        Break; // 限制成员数量
      if sRankInfo = '' then
        Break;
      sRankInfo := GetValidStr3(sRankInfo, sMemberName, [' ', ',']);
      if sMemberName <> '' then
        GuildRank.MemberList.Add(sMemberName);
      Inc(n4C);
      // if I > g_Config.nGuildMemberMaxLimit then Break; //限制成员数量
    end;
  end;

  if GuildRank <> nil then
  begin
    GuildRankList.Add(GuildRank);
  end;

  // 校验成员列表是否有改变，如果未修改则退出
  boCheckChange := False;
  if m_RankList.Count = GuildRankList.Count then
  begin
    boCheckChange := True;
    for i := 0 to m_RankList.Count - 1 do
    begin
      GuildRank := m_RankList.Items[i];
      NewGuildRank := GuildRankList.Items[i];
      if (GuildRank.nRankNo = NewGuildRank.nRankNo) and (GuildRank.sRankName = NewGuildRank.sRankName) and
        (GuildRank.MemberList.Count = NewGuildRank.MemberList.Count) then
      begin
        for II := 0 to GuildRank.MemberList.Count - 1 do
        begin
          if GuildRank.MemberList.Strings[II] <> NewGuildRank.MemberList.Strings[II] then
          begin
            boCheckChange := False; // 如果有改变则将其置为FALSE
            Break;
          end;
        end;
      end
      else
      begin
        boCheckChange := False;
        Break;
      end;
    end;
    if boCheckChange then
    begin
      Result := -1;
      ClearRankList(GuildRankList);
      Exit;
    end;
  end;

  // 检查行会掌门职业是否为空
  Result := -2;
  if (GuildRankList.Count > 0) then
  begin
    GuildRank := GuildRankList.Items[0];
    if GuildRank.nRankNo = 1 then
    begin
      if GuildRank.sRankName <> '' then
      begin
        Result := 0;
      end
      else
      begin
        Result := -3;
      end;
    end;
  end;

  { TODO -ochongchong -c新增 : 行会封号名称过滤 【2013-07-24】 }
  if Result = 0 then
  begin
    for i := 0 to GuildRankList.Count - 1 do
    begin
      GuildRank := GuildRankList.Items[i];
      if GetNameInFilterList(GuildRank.sRankName) then
      begin
        Result := -100;
        Break;
      end;
    end;
  end;

  // 检查行会掌门人是否在线(？？？)
  if Result = 0 then
  begin
    GuildRank := GuildRankList.Items[0];
    if GuildRank.MemberList.Count <= 2 then
    begin
      // 新行会系统不用检查掌门是否在线 chongchong 2015-08-25
      if not g_Config.boOpenNewGuild then
      begin
        n28 := GuildRank.MemberList.Count;
        for i := 0 to GuildRank.MemberList.Count - 1 do
        begin
          if UserEngine.GetPlayObject(GuildRank.MemberList.Strings[i]) = nil then
          begin
            Dec(n28);
            Break;
          end;
        end;
        if n28 <= 0 then
          Result := -5;
      end;
    end
    else
    begin
      Result := -4;
    end;
  end;

  if Result = 0 then
  begin
    for i := 0 to m_RankList.Count - 1 do
    begin
      GuildRank := m_RankList.Items[i];
      boCheckChange := True;
      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        boCheckChange := False;
        sMemberName := GuildRank.MemberList.Strings[II];
        for III := 0 to GuildRankList.Count - 1 do
        begin // 搜索新列表
          NewGuildRank := GuildRankList.Items[III];
          for n28 := 0 to NewGuildRank.MemberList.Count - 1 do
          begin
            if NewGuildRank.MemberList.Strings[n28] = sMemberName then
            begin
              boCheckChange := True;
              Break;
            end;
          end;
          if boCheckChange then
            Break;
        end;

        if not boCheckChange then
        begin // 原列表中的人物名称是否在新的列表中
          Result := -6;
          Break;
        end;
      end;
      if not boCheckChange then
        Break;
    end;

    for i := 0 to GuildRankList.Count - 1 do
    begin
      GuildRank := GuildRankList.Items[i];
      boCheckChange := True;
      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        boCheckChange := False;
        sMemberName := GuildRank.MemberList.Strings[II];
        for III := 0 to m_RankList.Count - 1 do
        begin // 搜索新列表
          NewGuildRank := m_RankList.Items[III];
          for n28 := 0 to NewGuildRank.MemberList.Count - 1 do
          begin
            if NewGuildRank.MemberList.Strings[n28] = sMemberName then
            begin
              boCheckChange := True;
              Break;
            end;
          end;
          if boCheckChange then
            Break;
        end;

        if not boCheckChange then
        begin // 原列表中的人物名称是否在新的列表中
          Result := -6;
          Break;
        end;
      end;
      if not boCheckChange then
        Break;
    end;

    { if (Result = 0) and (n2C <> n30) then begin
      Result := -6;
      end; }
  end;

  if Result = 0 then
  begin // 检查职位号是否重复及非法
    for i := 0 to GuildRankList.Count - 1 do
    begin
      n28 := pTGuildRank(GuildRankList.Items[i]).nRankNo;
      for III := i + 1 to GuildRankList.Count - 1 do
      begin
        if (pTGuildRank(GuildRankList.Items[III]).nRankNo = n28) or (n28 <= 0) or (n28 > 99) then
        begin
          Result := -7;
          Break;
        end;
      end;
      if Result <> 0 then
        Break;
    end;
  end;

  if Result = 0 then
  begin // 检查职位号是否重复及非法
    for i := 0 to GuildRankList.Count - 1 do
    begin
      n28 := pTGuildRank(GuildRankList.Items[i]).nRankNo;
      // if n28 <> 1 then begin
      for III := i + 1 to GuildRankList.Count - 1 do
      begin
        if (pTGuildRank(GuildRankList.Items[III]).nRankNo = n28) or (n28 <= 0) or (n28 > 99) then
        begin
          Result := -7;
          Break;
        end;
      end;
      // end;
      if Result <> 0 then
        Break;
    end;
  end;

  if Result = 0 then
  begin // 检查掌门数量
    n3C := 0; // 掌门数
    n2C := 0;
    for i := 0 to GuildRankList.Count - 1 do
    begin
      n28 := pTGuildRank(GuildRankList.Items[i]).nRankNo;
      if n28 = 1 then
      begin
        Inc(n2C);
        Inc(n3C, pTGuildRank(GuildRankList.Items[i]).MemberList.Count);
        if n3C > 2 then
        begin
          Result := -4;
          Break;
        end;
      end;
      if n2C > 1 then
      begin
        Result := -7;
        Break;
      end;
    end;
  end;

  if Result = 0 then
  begin // 检测人物是否重复
    for i := 0 to GuildRankList.Count - 1 do
    begin
      GuildRank := GuildRankList.Items[i];
      for II := 0 to GuildRank.MemberList.Count - 1 do
      begin
        boCheckChange := False;
        n2C := 0;
        for III := 0 to GuildRankList.Count - 1 do
        begin
          NewGuildRank := GuildRankList.Items[III];
          for IIII := 0 to NewGuildRank.MemberList.Count - 1 do
          begin
            if GuildRank.MemberList.Strings[II] = NewGuildRank.MemberList.Strings[IIII] then
              Inc(n2C);
            if n2C >= 2 then
            begin
              boCheckChange := True;
              Result := -7;
              Break;
            end;
          end;
          if boCheckChange then
            Break;
        end;
        if boCheckChange then
          Break;
      end;
      if boCheckChange then
        Break;
    end;
  end;

  if Result = 0 then
  begin
    ClearRankList(m_RankList);
    m_RankList := GuildRankList;
    // 更新在线人物职位表
    for i := 0 to m_RankList.Count - 1 do
    begin
      GuildRank := m_RankList.Items[i];
      for III := 0 to GuildRank.MemberList.Count - 1 do
      begin
        PlayObject := UserEngine.GetPlayObject(GuildRank.MemberList.Strings[III]);
        if (PlayObject <> nil) and (PlayObject.m_MyGuild = Self) then
        begin
          GuildRank.MemberList.Objects[III] := TObject(PlayObject);
          PlayObject.RefRankInfo(GuildRank.nRankNo, GuildRank.sRankName);
          PlayObject.RefShowName(); // 10/31
        end
        else
          GuildRank.MemberList.Objects[III] := nil;
      end;
    end;
    UpdateGuildFile();
  end
  else
  begin
    ClearRankList(GuildRankList);
  end;
end;

procedure TGuild.UpdateJoinCondition(AJob: Integer; ALevel: Integer; AMsg: string);
begin
  if AJob < 0 then
    AJob := 0
  else if AJob > 7 then
    AJob := 7;

  m_JoinJob := AJob;
  m_JoinLevel := Max(ALevel, 0);
  m_JoinMsg := AMsg;
  UpdateGuildFile;
end;

function TGuild.AllyGuild(Guild: TGuild): Boolean; // 00499C2C
var
  i: Integer;
begin
  Result := False;
  for i := 0 to GuildAllyList.Count - 1 do
  begin
    if GuildAllyList.Objects[i] = Guild then
    begin
      Exit;
    end;
  end;
  GuildAllyList.AddObject(Guild.sGuildName, Guild);
  SaveGuildInfoFile();
  Result := True;
end;

function TGuild.AddWarGuild(Guild: TGuild): pTWarGuild;
var
  i: Integer;
  WarGuild: pTWarGuild;
  M: Integer;
  S: string;
begin
  Result := nil;
  if Guild <> nil then
  begin
    if not IsAllyGuild(Guild) then
    begin
      WarGuild := nil;
      for i := 0 to GuildWarList.Count - 1 do
      begin
        if pTWarGuild(GuildWarList.Objects[i]).Guild = Guild then
        begin
          WarGuild := pTWarGuild(GuildWarList.Objects[i]);
          WarGuild.dwWarTick := MyGetTickCount();
          WarGuild.dwWarTime := g_Config.dwGuildWarTime { 10800000 };

          // 分钟
          M := g_Config.dwGuildWarTime div 60000;
          S := StringReplace(g_sGuildWarTime, '%s', Guild.sGuildName, []);
          S := StringReplace(S, '%d', IntToStr(M), []);

          SendGuildMsg(S);
          Break;
        end;
      end;
      if WarGuild = nil then
      begin
        New(WarGuild);
        WarGuild.Guild := Guild;
        WarGuild.dwWarTick := MyGetTickCount();
        WarGuild.dwWarTime := g_Config.dwGuildWarTime { 10800000 };
        GuildWarList.AddObject(Guild.sGuildName, TObject(WarGuild));

        // 分钟
        M := g_Config.dwGuildWarTime div 60000;
        S := StringReplace(g_sGuildWarStart, '%s', Guild.sGuildName, []);
        S := StringReplace(S, '%d', IntToStr(M), []);
        SendGuildMsg(S);
      end;
      Result := WarGuild;
    end;
  end;
  RefMemberName();
  UpdateGuildFile();
end;

function TGuild.AddAttentionGuild(Guild: TGuild): Boolean; // 00499C2C
var
  i: Integer;
begin
  Result := False;
  for i := 0 to GuildAttentionList.Count - 1 do
  begin
    if GuildAttentionList.Objects[i] = Guild then
    begin
      Exit;
    end;
  end;
  GuildAttentionList.AddObject(Guild.sGuildName, Guild);
  SaveGuildInfoFile();
  Result := True;
end;

function TGuild.AddRequestAllListGuild(Guild: TGuild): Boolean; // 00499C2C
var
  i: Integer;
begin
  Result := False;
  for i := 0 to GuildRequestAllList.Count - 1 do
  begin
    if GuildRequestAllList.Objects[i] = Guild then
    begin
      Exit;
    end;
  end;
  GuildRequestAllList.AddObject(Guild.sGuildName, Guild);
  SaveGuildInfoFile();
  Result := True;
end;

procedure TGuild.StopWarGuild(Guild: TGuild); // 00499B4C
begin
  SendGuildMsg('***' + Guild.sGuildName + '行会战争结束');
end;

function TGuild.GetMemberCount: Integer;
var
  i: Integer;
  GuildRank: pTGuildRank;
begin
  Result := 0;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];
    Inc(Result, GuildRank.MemberList.Count);
  end;
end;

function TGuild.GetOnlineMemberCount: Integer;
var
  i, j: Integer;
  GuildRank: pTGuildRank;
  PlayObject: TPlayObject;
begin
  Result := 0;
  for i := 0 to m_RankList.Count - 1 do
  begin
    GuildRank := m_RankList.Items[i];

    for j := 0 to GuildRank.MemberList.Count - 1 do
    begin
      PlayObject := UserEngine.GetPlayObject(GuildRank.MemberList.Strings[j]);
      if (PlayObject <> nil) and (not PlayObject.m_boOffLine) then
        Inc(Result, 1);
    end;
  end;
end;

function TGuild.GetMemberIsFull: Boolean;
begin
  Result := False;
  if GetMemberCount >= m_nMemberMaxLimit then
  begin
    Result := True;
  end;
end;

procedure TGuild.StartTeamFight;
begin
  nContestPoint := 0;
  boTeamFight := True;
  TeamFightDeadList.Clear;
end;

procedure TGuild.EndTeamFight;
begin
  boTeamFight := False;
end;

procedure TGuild.AddTeamFightMember(sHumanName: string);
begin
  TeamFightDeadList.Add(sHumanName);
end;

procedure TGuild.SetAuraePoint(nPoint: Integer);
begin
  m_nAurae := nPoint;
  boChanged := True;
end;

procedure TGuild.SetBuildPoint(nPoint: Integer);
begin
  m_nBuildPoint := nPoint;
  boChanged := True;
end;

procedure TGuild.SetFlourishPoint(nPoint: Integer);
begin
  m_nFlourishing := nPoint;
  boChanged := True;
end;

procedure TGuild.SetStabilityPoint(nPoint: Integer);
begin
  m_nStability := nPoint;
  boChanged := True;
end;

procedure TGuild.SetChiefItemCount(nPoint: Integer);
begin
  m_nChiefItemCount := nPoint;
  boChanged := True;
end;

procedure TGuild.AddWantAddMember(Player: TPlayObject);
var
  AddMember: PWantAddMember;
begin
  AddMember := FindWantAddMember(Player.m_sCharName);
  if AddMember = nil then
  begin
    New(AddMember);
    FWantAddMemberList.Add(AddMember);
  end;

  AddMember.UserName := Player.m_sCharName;
  AddMember.Sex := Player.m_btGender;
  AddMember.Job := Player.m_btJob;
  AddMember.Level := Player.m_Abil.Level;
  AddMember.AddTime := Now();
  UpdateGuildFile;
end;

procedure TGuild.ClearWantAddMembers;
var
  i: Integer;
  AddMember: PWantAddMember;
begin
  for i := 0 to FWantAddMemberList.Count - 1 do
  begin
    AddMember := FWantAddMemberList.Items[i];
    Dispose(AddMember);
  end;
  FWantAddMemberList.Clear;
end;

function TGuild.DeleteWantAddMember(UserName: string): Boolean;
var
  Index: Integer;
  AddMember: PWantAddMember;
begin
  Result := False;
  Index := FindWantAddMemberIndex(UserName);
  if Index <> -1 then
  begin
    AddMember := FWantAddMemberList.Items[Index];
    Dispose(AddMember);
    FWantAddMemberList.Delete(Index);
    UpdateGuildFile;

    Result := True;
  end;
end;

function TGuild.FindWantAddMember(UserName: string): PWantAddMember;
var
  Index: Integer;
begin
  Result := nil;
  Index := FindWantAddMemberIndex(UserName);
  if Index <> -1 then
  begin
    Result := FWantAddMemberList.Items[Index];
  end;
end;

function TGuild.FindWantAddMemberIndex(UserName: string): Integer;
var
  i: Integer;
  AddMember: PWantAddMember;
begin
  Result := -1;
  for i := 0 to FWantAddMemberList.Count - 1 do
  begin
    AddMember := FWantAddMemberList.Items[i];
    if SameText(UserName, AddMember.UserName) then
    begin
      Result := i;
      Break;
    end;
  end;
end;

function TGuild.GetWantAddMemberCount: Integer;
begin
  Result := FWantAddMemberList.Count;
end;

function TGuild.GetWantAddMemebers(Index: Integer): PWantAddMember;
begin
  Result := nil;
  if (Index >= 0) and (Index < FWantAddMemberList.Count) then
    Result := FWantAddMemberList.Items[Index];
end;

procedure TGuild.SetEnabledAlly(Value: Boolean);
begin
  if m_EnabledAlly <> Value then
  begin
    m_EnabledAlly := Value;
    UpdateGuildFile;
  end;
end;

end.
