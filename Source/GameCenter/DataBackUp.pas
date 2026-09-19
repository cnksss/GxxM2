unit DataBackUp;

interface
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  ExtCtrls, ComCtrls, VCLZip, ShellApi;
type
  TBackUpTask = class
  private
    VCLZip: TVCLZip;
    FSourceDirectory: string;
    FDestDirectory: string;
    FTodayDate: TDate;
    FMode: Byte;
    FStart: Boolean;
    FBackUpCount: Integer;
    FFailCount: Integer;
    FBackUp: TNotifyEvent;

    FTodayBackUpTaskOK: Boolean;
    FBackUpTick: LongWord;
    FHour, FMin: Word;
    // 是否压缩 piaoyun 2013-08-30
    FIsCompress: Boolean;
    procedure SearchFiles(Recursive: Boolean; FileList: TStrings);
    // 拷贝文件夹 piaoyun 2013-08-30
    function DoCopyDir(SourceDirectory: string; DestDirectory: string): Boolean;
    // ZIP压缩文件 piaoyun 2013-08-30
    function ZipFile(sSaveFileName: string): Boolean;

    procedure VCLZipFilePercentDone(Sender: TObject; Percent: Integer);
    procedure VCLZipTotalPercentDone(Sender: TObject; Percent: Integer);
    procedure SetSourceDirectory(Value: string);
    procedure SetDestDirectory(Value: string);
    procedure SetStart(Value: Boolean);
    procedure BackUp1(Sender: TObject);
    procedure BackUp2(Sender: TObject);
    procedure SetMode(Value: Byte);
  public
    constructor Create();
    destructor Destroy; override;
    procedure Initialize;
    procedure Run();
    property SourceDirectory: string read FSourceDirectory write SetSourceDirectory;
    property DestDirectory: string read FDestDirectory write SetDestDirectory;
    property Start: Boolean read FStart write SetStart;
    property BackUpCount: Integer read FBackUpCount;
    property FailCount: Integer read FFailCount;
    property Mode: Byte read FMode write SetMode;
    property Hour: Word read FHour write FHour;
    property Min: Word read FMin write FMin;
    property IsCompress: Boolean read FIsCompress write FIsCompress;
    //property IsCompress: Boolean read FIsCompress write SetIsCompress;
  end;

  TBackUpManager = class
    m_CriticalSection: TRTLCriticalSection;
    m_BackUpList: TList;
    m_TimerStart: TTimer;
  private
    procedure TimerStartTimer(Sender: TObject);
    //procedure ClearStartTick();
    function GetStart: Boolean;
    procedure SetStart(Value: Boolean);
  public
    constructor Create();
    destructor Destroy; override;
    procedure Add(Obj: TObject);
    function Find(const Source: string): TObject;
    function Delete(const Source: string): Boolean;
    procedure Clear();
    procedure Run();
    property Start: Boolean read GetStart write SetStart;
  end;

implementation

function SaveFilePath: string;
var
  wYear, wMonth, wDay: Word;
  wHour, wMin, wSec, wMSec: Word;
begin
  DecodeDate(Now, wYear, wMonth, wDay);
  DecodeTime(Time, wHour, wMin, wSec, wMSec);
  Result := Format('%d-%d-%d.%2d-%2d', [wYear, wMonth, wDay, wHour, wMin]);
end;

function GetLastDirName(const sSourceDirectory: string): string;
var
  TempList: TStringList;
begin
  TempList := TStringList.Create;
  ExtractStrings(['\', '/'], [], PChar(sSourceDirectory), TempList);
  if TempList.Count > 0 then
    Result := TempList.Strings[TempList.Count - 1]
  else
    Result := SaveFilePath;
  TempList.Free;
end;

procedure TBackUpTask.VCLZipFilePercentDone(Sender: TObject; Percent: Integer);
begin

end;

procedure TBackUpTask.VCLZipTotalPercentDone(Sender: TObject; Percent: Integer);
begin

end;

procedure TBackUpTask.SetSourceDirectory(Value: string);
begin
  FSourceDirectory := Value;
  if FSourceDirectory <> '' then
    FSourceDirectory := IncludeTrailingPathDelimiter(FSourceDirectory);
end;

procedure TBackUpTask.SetDestDirectory(Value: string);
begin
  FDestDirectory := Value;
  if FDestDirectory <> '' then
    FDestDirectory := IncludeTrailingPathDelimiter(FDestDirectory);
end;

procedure TBackUpTask.SetStart(Value: Boolean);
begin
  if FStart <> Value then
  begin
    FStart := Value;
  end;
end;

procedure TBackUpTask.SearchFiles(Recursive: Boolean; FileList: TStrings);

  procedure SearchDirectory(const SDirectory: string);
  var
    SearchRec: TSearchRec;
    Res: Integer;
  const
    AllFilesMask = '*.*';
  begin
    // (rom) this may not work for network drives and compressed files
    // (rom) because of faAnyFile
    Res := FindFirst(SourceDirectory + SDirectory + AllFilesMask, faAnyFile, SearchRec);
    try
      while Res = 0 do
      begin
        if (SearchRec.Name <> '.') and (SearchRec.Name <> '..') then
        begin
          if (SearchRec.Attr and faDirectory) = 0 then
            FileList.Add(SourceDirectory + SDirectory + SearchRec.Name)
          else
            if Recursive then
              SearchDirectory(SDirectory + SearchRec.Name + PathDelim);
        end;
        Res := FindNext(SearchRec);
      end;
    finally
      FindClose(SearchRec);
    end;
  end;

begin
  SearchDirectory('');
end;

function TBackUpTask.DoCopyDir(SourceDirectory: string; DestDirectory: string): Boolean;
var
  hFindFile: Cardinal;
  t, tfile: string;
  sCurDir: string[255];
  FindFileData: WIN32_FIND_DATA;
begin
  //记录当前目录
  sCurDir := GetCurrentDir;
  ChDir(SourceDirectory);
  hFindFile := FindFirstFile('*.*', FindFileData);
  if hFindFile <> INVALID_HANDLE_VALUE then
  begin
    if not DirectoryExists(DestDirectory) then
      ForceDirectories(DestDirectory);
    repeat
      tfile := FindFileData.cFileName;
      if (tfile = '.') or (tfile = '..') then Continue;
      if FindFileData.dwFileAttributes = FILE_ATTRIBUTE_DIRECTORY then
      begin
        t := DestDirectory + '\' + tfile;
        if not DirectoryExists(t) then ForceDirectories(t);
        if SourceDirectory[Length(SourceDirectory)] <> '\' then
          DoCopyDir(SourceDirectory + '\' + tfile, t)
        else
          DoCopyDir(SourceDirectory + tfile, DestDirectory + tfile);
      end else
      begin
        t := DestDirectory + '\' + tFile;
        CopyFile(PChar(tfile), PChar(t), True);
      end;
    until FindNextFile(hFindFile, FindFileData) = False;
///       FindClose(hFindFile);
  end
  else
  begin
    ChDir(sCurDir);
    Result := False;
    Exit;
  end;
  //回到当前目录
  ChDir(sCurDir);
  Result := True;
end;

function TBackUpTask.ZipFile(sSaveFileName: string): Boolean;                                       //压缩文件
begin
  //Result := False;
  VCLZip := TVCLZip.Create(nil);
  VCLZip.OnFilePercentDone := VCLZipFilePercentDone;
 // VCLUnZip.OnStartUnZip := VCLUnZipStartUnZip;
 // VCLUnZip.OnStartUnZipInfo := VCLUnZipStartUnZipInfo;
  VCLZip.OnTotalPercentDone := VCLZipTotalPercentDone;
  try
    with VCLZip do
    begin
      ClearZip;
      Recurse := True;
      //RelativePaths := True;
      ZipName := sSaveFileName;
    end;

    SearchFiles(True, VCLZip.FilesList);

    try
      VCLZip.Zip;
      Result := True;
    except
      Result := False;
    end;
  finally
    FreeAndNil(VCLZip);
  end;
end;

procedure TBackUpTask.BackUp1(Sender: TObject);

  function CanBackUp: Boolean;
  var
    wHour, wMin, wSec, wMSec: Word;
  begin
    DecodeTime(Time, wHour, wMin, wSec, wMSec);
    Result := (FHour = wHour) and (FMin = wMin);
  end;
var
  sSaveFileName: string;
  sSaveFilePath: string;
begin
  if (not FTodayBackUpTaskOK) and CanBackUp() then
  begin
    FTodayBackUpTaskOK := True;
    sSaveFilePath := DestDirectory + SaveFilePath + '\';
    if not DirectoryExists(sSaveFilePath) then ForceDirectories(sSaveFilePath);                     //创建目录
    sSaveFileName := sSaveFilePath + GetLastDirName(SourceDirectory);

    // 是否启用压缩 piaoyun 2013-08-30
    if FIsCompress then
    begin
      if ZipFile(sSaveFileName + '.ZIP') then
        FBackUpCount := FBackUpCount + 1
      else
        FFailCount := FFailCount + 1;
    end
    else
    begin
      if DoCopyDir(SourceDirectory,sSaveFileName) then
        FBackUpCount := FBackUpCount + 1
      else
        FFailCount := FFailCount + 1;
    end;
  end;

  if FTodayDate <> Date then
  begin
    FTodayDate := Date;
    FTodayBackUpTaskOK := False;
  end;
end;

procedure TBackUpTask.BackUp2(Sender: TObject);
var
  sSaveFileName: string;
  sSaveFilePath: string;
begin
  if GetTickCount - FBackUpTick > (FHour * 60 * 60 * 1000 + FMin * 60 * 1000) then
  begin
    FBackUpTick := GetTickCount;
    sSaveFilePath := DestDirectory + SaveFilePath + '\';
    if not DirectoryExists(sSaveFilePath) then ForceDirectories(sSaveFilePath);                     //创建目录
    sSaveFileName := sSaveFilePath + GetLastDirName(SourceDirectory);
    // 是否启用压缩 piaoyun 2013-08-30
    if FIsCompress then
    begin
      if ZipFile(sSaveFileName  + '.ZIP') then
        FBackUpCount := FBackUpCount + 1
      else
        FFailCount := FFailCount + 1;
    end
    else
    begin
      if DoCopyDir(SourceDirectory,sSaveFileName) then
        FBackUpCount := FBackUpCount + 1
      else
        FFailCount := FFailCount + 1;
    end;
  end;
end;

procedure TBackUpTask.SetMode(Value: Byte);
begin
  FMode := Value;
  if FMode = 0 then
    FBackUp := BackUp1
  else
    FBackUp := BackUp2;
end;

constructor TBackUpTask.Create();
begin
  inherited;
  FTodayDate := Date;
  FMode := 0;
  FStart := False;
  FBackUpCount := 0;
  FBackUp := nil;
  FTodayBackUpTaskOK := False;
  FBackUpTick := GetTickCount;
  FHour := 12;
  FMin := 0;
  VCLZip := nil;
  FIsCompress := True;
end;

destructor TBackUpTask.Destroy;
begin
  FStart := False;
  while VCLZip <> nil do Application.ProcessMessages;
  inherited;
end;

procedure TBackUpTask.Initialize;
begin

end;

procedure TBackUpTask.Run();
begin
  if FStart and Assigned(FBackUp) then
    FBackUp(Self);
end;
//------------------------------------------------------------------------------

procedure TBackUpManager.TimerStartTimer(Sender: TObject);
begin
  Run;
end;

{procedure TBackUpManager.ClearStartTick();
begin

end; }

constructor TBackUpManager.Create();
begin
  inherited;
  InitializeCriticalSection(m_CriticalSection);
  m_BackUpList := TList.Create;
  m_TimerStart := TTimer.Create(nil);
  m_TimerStart.Enabled := False;
  m_TimerStart.Interval := 1000;
  m_TimerStart.OnTimer := TimerStartTimer;
end;

destructor TBackUpManager.Destroy;
var
  I: Integer;
begin
  m_TimerStart.Enabled := False;
  for I := 0 to m_BackUpList.Count - 1 do
  begin
    TBackUpTask(m_BackUpList.Items[I]).Free;
  end;
  m_TimerStart.Free;
  m_BackUpList.Free;
  DeleteCriticalSection(m_CriticalSection);
  inherited;
end;

procedure TBackUpManager.Add(Obj: TObject);
begin
  EnterCriticalSection(m_CriticalSection);
  try
    m_BackUpList.Add(Obj);
  finally
    LeaveCriticalSection(m_CriticalSection);
  end;
end;

function TBackUpManager.Find(const Source: string): TObject;
var
  I: Integer;
begin
  Result := nil;
  EnterCriticalSection(m_CriticalSection);
  try
    for I := 0 to m_BackUpList.Count - 1 do
    begin
      if CompareText(TBackUpTask(m_BackUpList.Items[I]).SourceDirectory, Source) = 0 then
      begin
        Result := TBackUpTask(m_BackUpList.Items[I]);
        break;
      end;
    end;
  finally
    LeaveCriticalSection(m_CriticalSection);
  end;
end;

function TBackUpManager.Delete(const Source: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  EnterCriticalSection(m_CriticalSection);
  try
    for I := m_BackUpList.Count - 1 downto 0 do
    begin
      if CompareText(TBackUpTask(m_BackUpList.Items[I]).SourceDirectory, Source) = 0 then
      begin
        TBackUpTask(m_BackUpList.Items[I]).Free;
        m_BackUpList.Delete(I);
        Result := True;
        break;
      end;
    end;
  finally
    LeaveCriticalSection(m_CriticalSection);
  end;
end;

procedure TBackUpManager.Clear();
var
  I: Integer;
begin
  EnterCriticalSection(m_CriticalSection);
  try
    for I := 0 to m_BackUpList.Count - 1 do
    begin
      TBackUpTask(m_BackUpList.Items[I]).Free;
    end;
    m_BackUpList.Clear;
  finally
    LeaveCriticalSection(m_CriticalSection);
  end;
end;

procedure TBackUpManager.Run();
var
  I: Integer;
begin
  EnterCriticalSection(m_CriticalSection);
  try
    for I := 0 to m_BackUpList.Count - 1 do
    begin
      TBackUpTask(m_BackUpList.Items[I]).Run;
    end;
  finally
    LeaveCriticalSection(m_CriticalSection);
  end;
end;

function TBackUpManager.GetStart: Boolean;
begin
  Result := m_TimerStart.Enabled;
end;

procedure TBackUpManager.SetStart(Value: Boolean);
begin
  m_TimerStart.Enabled := Value;
end;


end.

