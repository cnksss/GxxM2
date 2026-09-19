unit BassSound;

interface
uses
  Windows,
  SysUtils,
  Math,
  Classes,
  ExtCtrls,
  Dialogs,
  Bass;

type
  TSoundType = (sntNotSupported, sntMOD, sntMP3, sntOGG, sntWAV);
  TSoundErrorEvent = procedure(Sender:TObject; ErrorText:string) of object;

  TSoundData = class
    m_dwTimeTick:LongWord;
  private
    FHandle:Cardinal;
    FFileName:string;
    FSoundType:TSoundType;
    FPlayed:Boolean;
    FStopBefore:Boolean;

    FLoopCount:Integer;

    //procedure SetVolume(const Value: Single);
  public
    constructor Create(AHandle:Cardinal; AFileName:string; ASoundType:TSoundType);
    destructor Destroy; override;

    function IsPlaying:Boolean;
    function IsPauseing:Boolean;

    function Play():Boolean;
    procedure Stop;
    procedure Pause;
    property Played:Boolean read FPlayed write FPlayed;
    property FileName:string read FFileName write FFileName;
    property Handle:Cardinal read FHandle write FHandle;
    property LoopCount:Integer read FLoopCount write FLoopCount;
  end;

  TBassSound = class
  private
    FOnError:TSoundErrorEvent;
    FActive:Boolean;
    FInitialized:Boolean;
    FInitializeBassDLL:Boolean;
    FTimer:TTimer;
    FProcSoundIdx:Integer;
    FProcMusicIdx:Integer;
    FSoundList:TStringList;
    FMusicList:TStringList;
    FFreeList:TStringList;
    FFileNameList:TStringList;

    FCriticalSection:TRTLCriticalSection;
    procedure OnTimer(Sender:TObject);
    procedure SetActive(Value:Boolean);

    procedure ErrorEvent(const ErrorText:string);

    function GetSoundTypeFromFilename(FileName:string):TSoundType;
    function LoadFromFile(const FileName:string):TSoundData;
    function GetSoundCount:Integer;
    function GetMusicCount:Integer;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Initialize(Handle:THandle);
    procedure Finalize;
    procedure Clear(ClearType:Integer = 0);
    procedure Lock;
    procedure UnLock;
    procedure Play(const FileName:string);
    procedure Stop(const FileName:string);
    procedure PlayMusic(const FileName:string; const LoopCount:Integer = -1; const StopBefore:Boolean = False);
    procedure StopMusic(const FileName:string);
    procedure RepeatMusic(Value:Boolean);
    property Initialized:Boolean read FInitialized;
    property Active:Boolean read FActive write SetActive;
    property SoundCount:Integer read GetSoundCount;
    property MusicCount:Integer read GetMusicCount;
    property OnError:TSoundErrorEvent read FOnError write FOnError;
  end;

const
  MAX_SOUND_COUNT = 50;

var
  g_SoundVolume:Integer = 100;
  g_dwLogonTick:LongWord = 0;

implementation

uses
  MShare;

constructor TBassSound.Create;
begin
  InitializeCriticalSection(FCriticalSection);
  FOnError := nil;
  FSoundList := TStringList.Create;
  FMusicList := TStringList.Create;

  FFileNameList := TStringList.Create;
  FFileNameList.Sorted := True;
  FFileNameList.Duplicates := dupIgnore;

  FFreeList := TStringList.Create;
  FTimer := TTimer.Create(nil);
  FTimer.Interval := 1000;
  FTimer.Enabled := False;
  FTimer.OnTimer := OnTimer;
  FInitialized := False;
  FProcSoundIdx := 0;
  FProcMusicIdx := 0;
  FInitializeBassDLL := False;

  if (HIWORD(BASS_GetVersion) <> BASSVERSION) then
    ErrorEvent('Error BASSVERSION - BASS errocode ' + IntToStr(BASS_ErrorGetCode))
  else if not BASS_Init(-1, 44100, 0, 0, nil) then begin
    ErrorEvent('Error initializing -1 sound - BASS errocode ' + IntToStr(BASS_ErrorGetCode));
  end
  else
    FInitializeBassDLL := True;
end;

destructor TBassSound.Destroy;
var
  Idx:Integer;
  Sound:TSoundData;
begin
  FTimer.Enabled := False;
  for Idx := 0 to FSoundList.Count - 1 do begin
    Sound := TSoundData(FSoundList.Objects[Idx]);
    FSoundList.Objects[Idx] := nil;
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('UnLoad Error Free sound');
      end;
    end;
  end;

  for Idx := 0 to FMusicList.Count - 1 do begin
    Sound := TSoundData(FMusicList.Objects[Idx]);
    FMusicList.Objects[Idx] := nil;
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('UnLoad Error Free sound');
      end;
    end;
  end;

  for Idx := 0 to FFreeList.Count - 1 do begin
    Sound := TSoundData(FFreeList.Objects[Idx]);
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('UnLoad Error Free sound');
      end;
    end;
  end;

  FFileNameList.Free;
  FMusicList.Free;
  FSoundList.Free;
  FFreeList.Free;
  DeleteCriticalSection(FCriticalSection);

  { TODO -ochongchong -c内存泄露 : 去内存泄露 【2013-7-12】 }
  FTimer.Free;
  //----------------------------------------------------end

  inherited Destroy;
end;

procedure TBassSound.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TBassSound.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TBassSound.Initialize(Handle:THandle);
begin
  if not FInitialized then begin
    if InitializeBassDLLOK and FInitializeBassDLL then begin
      BASS_Start;
      FInitialized := True;
    end;
    FTimer.Enabled := FInitialized;
  end;
end;

procedure TBassSound.Finalize;
var
  Idx:Integer;
  Sound:TSoundData;
begin
  FTimer.Enabled := False;
  FInitialized := False;
  for Idx := 0 to FSoundList.Count - 1 do begin
    Sound := TSoundData(FSoundList.Objects[Idx]);
    FSoundList.Objects[Idx] := nil;
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('TBassSound.Finalize Error Free sound 1');
      end;
    end;
  end;

  for Idx := 0 to FMusicList.Count - 1 do begin
    Sound := TSoundData(FMusicList.Objects[Idx]);
    FMusicList.Objects[Idx] := nil;
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('TBassSound.Finalize Error Free sound 2');
      end;
    end;
  end;

  for Idx := 0 to FFreeList.Count - 1 do begin
    Sound := TSoundData(FFreeList.Objects[Idx]);
    FMusicList.Objects[Idx] := nil;
    if (Sound <> nil) then begin
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('TBassSound.Finalize Error Free sound 3');
      end;
    end;
  end;

  if InitializeBassDLLOK and FInitializeBassDLL then
    BASS_Stop;

  FProcSoundIdx := 0;
  FProcMusicIdx := 0;

  FFileNameList.Clear;
  FMusicList.Clear;
  FSoundList.Clear;
  FFreeList.Clear;
end;

procedure TBassSound.ErrorEvent(const ErrorText:string);
begin
  if Assigned(OnError) then
    OnError(Self, ErrorText);
end;

function TBassSound.GetMusicCount:Integer;
begin
  Result := FMusicList.Count;
end;

function TBassSound.GetSoundCount:Integer;
begin
  Result := FSoundList.Count;
end;

function TBassSound.GetSoundTypeFromFilename(FileName:string):TSoundType;
var
  sExt:string;
begin
  if FileExists(FileName) then begin
    sExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
    if (sExt = '.MOD') or (sExt = '.XM') or (sExt = '.S3M') or (sExt = '.MTM') or (sExt = '.UMX') or (sExt = '.MO3') or (sExt = '.IT') then
      Result := sntMOD
    else if (sExt = '.MP3') then
      Result := sntMP3
    else if (sExt = '.OGG') then
      Result := sntOGG
    else if (sExt = '.WAV') then
      Result := sntWAV
    else
      Result := sntNotSupported;
  end
  else
    Result := sntNotSupported;
end;

function TBassSound.LoadFromFile(const FileName:string):TSoundData;
var
  Handle:Cardinal;
  SoundType:TSoundType;
  //Name: string;
begin
  Result := nil;
  if (not FInitialized) then Exit;
  Handle := 0;
  SoundType := GetSoundTypeFromFilename(FileName);
  if SoundType <> sntNotSupported then begin
    case SoundType of
      //sntMOD: Handle := BASS_MusicLoad(False, PChar(FileName), 0, 0, BASS_MUSIC_RAMP, 0);
      sntMP3,
        sntOGG, sntWAV:Handle := BASS_StreamCreateFile(False, PChar(FileName), 0, 0, 0);
      // sntWAV: Handle := BASS_SampleLoad(False, PChar(FileName), 0, 0, 3, BASS_SAMPLE_OVER_POS);   ExtractFileName(
      sntNotSupported:; //Error('Unable to load sound ' + FileName + ' - format is not supported');
    end;
    if (Handle = 0) then begin
      if (SoundType <> sntNotSupported) then
        ErrorEvent('Unable to load sound ' + FileName + ' - BASS errocode ' + IntToStr(BASS_ErrorGetCode))
    end
    else begin
      //Name := ExtractFileName(FileName);
      Result := TSoundData.Create(Handle, FileName, SoundType);
    end;
  end;
end;

procedure TBassSound.OnTimer(Sender:TObject);
var
  Index, Idx, nCount:Integer;
  Sound:TSoundData;
  dwTimeTick:longword;
  boCheckTimeLimit:Boolean;
begin
  Lock;
  try
    // 声音太多停一些。只保留一部分的声音就可以 2019-05-10 23:48:54
    while FSoundList.Count > MAX_SOUND_COUNT do begin
      Sound := TSoundData(FSoundList.Objects[0]);
      if (Sound <> nil) then begin
        if Sound.IsPlaying then
          Sound.Stop;
        try
          FreeAndNil(Sound);
        except

        end;
      end;

      Index := FFileNameList.IndexOf(FSoundList.Strings[0]);
      if Index >= 0 then begin
        nCount := Integer(FFileNameList.Objects[Index]);
        if nCount > 0 then begin
          Dec(nCount);
          FFileNameList.Objects[Index] := TObject(nCount);
        end;
      end;

      FSoundList.Delete(0);
    end;

    Idx := FProcSoundIdx;
    boCheckTimeLimit := False;
    dwTimeTick := MyGetTickCount;

    while True do begin
      if FSoundList.Count <= Idx then Break;
      Sound := TSoundData(FSoundList.Objects[Idx]);

      if Sound.IsPlaying then begin
        if FActive then
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
        else
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0);

        Sound.m_dwTimeTick := MyGetTickCount;
        Inc(Idx);
        Continue;
        {
           end
           else // 我也不知道这里对不对 2020-04-23
           begin
             FFreeList.AddObject(FSoundList.Strings[Idx], Sound);
             FSoundList.Delete(Idx);
        }
      end;

      if (MyGetTickCount - Sound.m_dwTimeTick > 1000 * 10) then begin
        FFreeList.AddObject(FSoundList.Strings[Idx], Sound);
        FSoundList.Delete(Idx);
        Continue;
      end;

      Inc(Idx);
      if (MyGetTickCount - dwTimeTick > 10) or (not FActive) then begin
        boCheckTimeLimit := True;
        FProcSoundIdx := Idx;
        Break;
      end;
    end;
    if not boCheckTimeLimit then FProcSoundIdx := 0;

    Idx := FProcMusicIdx;
    boCheckTimeLimit := False;
    dwTimeTick := MyGetTickCount;
    while True do begin
      if FMusicList.Count <= Idx then Break;
      Sound := TSoundData(FMusicList.Objects[Idx]);
      if Sound = nil then begin
        FMusicList.Delete(Idx);
        Continue;
      end;

      if Sound.IsPlaying then begin
        if FActive then
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
        else
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0);

        Sound.m_dwTimeTick := MyGetTickCount;
        Inc(Idx);
        Continue;
      end;

      if (MyGetTickCount - Sound.m_dwTimeTick > 1000 * 10) then begin
        FFreeList.AddObject(FMusicList.Strings[Idx], Sound);
        FMusicList.Delete(Idx);
        Continue;
      end;

      Inc(Idx);
      if (MyGetTickCount - dwTimeTick > 10) or (not FActive) then begin
        boCheckTimeLimit := True;
        FProcMusicIdx := Idx;
        Break;
      end;
    end;
    if not boCheckTimeLimit then FProcMusicIdx := 0;

    for Idx := 0 to FFreeList.Count - 1 do begin
      Sound := TSoundData(FFreeList.Objects[Idx]);
      Index := FFileNameList.IndexOf(FFreeList.Strings[Idx]);
      if Index >= 0 then begin
        nCount := Integer(FFileNameList.Objects[Index]);
        if nCount > 0 then begin
          Dec(nCount);
          FFileNameList.Objects[Index] := TObject(nCount);
        end;
      end;
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('TBassSound.Run Error Free sound');
      end;
    end;
    FFreeList.Clear;
  except
    ErrorEvent('TBassSound.Run Error');
  end;
  UnLock;
end;

procedure TBassSound.Play(const FileName:string);
var
  Index:Integer;
  nCount, MaxCount:Integer;
  Sound:TSoundData;
begin
  if (not FInitialized) {放开后面的选项 2017-05-13} or (not FActive) then Exit;

  {
  // 这里会做一次二分查找
  Index := FFileNameList.IndexOf(FileName);
  if Index >= 0 then
  begin
    nCount := Integer(FFileNameList.Objects[Index]);
    if nCount >= High(Integer) then
      nCount := 1;
    FFileNameList.Objects[Index] := TObject(nCount + 1);
  end
  else
  begin
    nCount := 1;

    // 这里再一次二分查找
    FFileNameList.AddObject(FileName, TObject(nCount));
  end;
  }

  // 这样只做一次二分查找， 重复的不会加，但会返回位置 2019-05-10 23:48:21
  Index := FFileNameList.AddObject(FileName, TObject(0));
  if Index >= 0 then begin
    nCount := Integer(FFileNameList.Objects[Index]);
    if nCount >= High(Integer) then
      nCount := 0;
    FFileNameList.Objects[Index] := TObject(nCount + 1);
  end else begin
    nCount := 0; //HZQ 20230525
  end;

  // 修正周边有很多怪时，刚进入系统的一会声音比较大 chongchong 2015-11-13
  // 因为怪物的声音全堆在一起了
  if MyGetTickCount - g_dwLogonTick <= 2000 then
    MaxCount := 3
  else
    MaxCount := 0;

  if (MaxCount = 0) or (nCount < MaxCount) then begin //限制同一个声音允许同时播放数量
    Sound := LoadFromFile(FileName);

    if (Sound <> nil) then begin
      if FActive then
        BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
      else
        BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0 / 100);

      Sound.Play();
      Lock;
      try
        FSoundList.AddObject(FileName, Sound);
      finally
        UnLock;
      end;
    end;
  end;
end;

procedure TBassSound.Stop(const FileName:string);
var
  I:Integer;
  Sound:TSoundData;
begin
  Lock;
  try
    for I := 0 to FSoundList.Count - 1 do begin
      if SameText(FSoundList[I], FileName) then begin
        Sound := TSoundData(FSoundList.Objects[I]);
        if (Sound <> nil) then begin
          if Sound.IsPlaying then
            Sound.Stop;
          try
            FreeAndNil(Sound);
          except

          end;
        end;
        FSoundList.Delete(I);
        Exit;
      end;
    end;

    for I := FMusicList.Count - 1 downto 0 do begin
      Sound := TSoundData(FMusicList.Objects[I]);
      if (Sound <> nil) then begin
        if Sound.IsPlaying then
          Sound.Stop;
        try
          FreeAndNil(Sound);
        except

        end;
      end;
      FMusicList.Delete(I);
      Exit;
    end;
  finally
    UnLock;
  end;
end;

procedure TBassSound.RepeatMusic(Value:Boolean);
var
  Sound:TSoundData;
  Idx:Integer;
begin
  if (not FInitialized) then Exit;
  Lock;
  try
    for Idx := 0 to FMusicList.Count - 1 do begin
      Sound := TSoundData(FMusicList.Objects[Idx]);
      if (Sound <> nil) then begin
        // 这里不知道有没有问题 chongchong 2018-02-07
        if FActive then
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
        else
          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0 / 100);

        if Value then
          Sound.LoopCount := -1
        else if Sound.LoopCount = -1 then
          Sound.LoopCount := 0;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TBassSound.PlayMusic(const FileName:string; const LoopCount:Integer; const StopBefore:Boolean);
var
  Sound:TSoundData;
  Idx:Integer;
  boFind:Boolean;
begin
  if (not FInitialized) then Exit;
  boFind := False;
  Lock;
  try
    if StopBefore then begin
      for Idx := FMusicList.Count - 1 downto 0 do begin
        Sound := TSoundData(FMusicList.Objects[Idx]);
        if Sound.FStopBefore = StopBefore then begin
          Sound.Stop;
          Sound.LoopCount := 0;
        end;
      end;
    end;

    for Idx := 0 to FMusicList.Count - 1 do begin
      if (CompareText(FMusicList.Strings[Idx], FileName) = 0) then begin
        Sound := TSoundData(FMusicList.Objects[Idx]);
        if (Sound <> nil) and (Sound.FStopBefore = StopBefore) then begin

          boFind := True;
          Sound.LoopCount := LoopCount;
          Sound.m_dwTimeTick := MyGetTickCount;

          // 修正同一电脑 A 角色点 PlaySound 全局播放到B角色时，点到B角色会继续播放后面部分的 chonchong 2015-12-12
          //BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100);
          //if FActive then Sound.Play();

          if FActive then
            BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
          else
            BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0);

          // 修下重复播放声音 2019-12-17
          if not Sound.IsPlaying then
            Sound.Play();

          break;
        end;
      end;
    end;
  finally
    UnLock;
  end;

  if not boFind then begin
    Sound := LoadFromFile(FileName);
    if (Sound <> nil) then begin
      Sound.FStopBefore := StopBefore;

      Sound.LoopCount := LoopCount;
      Lock;
      try
        FMusicList.AddObject(FileName, Sound);
      finally
        UnLock;
      end;

      // 修正同一电脑 A 角色点 PlaySound 全局播放到B角色时，点到B角色会继续播放后面部分的 chonchong 2015-12-12
      //BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100);
      //if FActive then Sound.Play();

      if FActive then
        BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100)
      else
        BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0);
      Sound.Play();

    end;
  end;
end;

procedure TBassSound.StopMusic(const FileName:string);
var
  I:Integer;
  Sound:TSoundData;
begin
  if (not FInitialized) then Exit;

  Lock;
  try
    for I := 0 to FMusicList.Count - 1 do begin
      if SameText(FMusicList[I], FileName) then begin
        Sound := TSoundData(FMusicList.Objects[I]);
        if (Sound <> nil) then begin
          if Sound.IsPlaying then
            Sound.Stop;
          try
            FreeAndNil(Sound);
          except

          end;
        end;
        FMusicList.Delete(I);
        Exit;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TBassSound.Clear(ClearType:Integer = 0);
var
  Sound:TSoundData;
  Idx:Integer;
begin
  Lock;
  try
    FTimer.Enabled := False;
    FFileNameList.Clear;

    if ClearType in [0, 1] then begin
      for Idx := 0 to FSoundList.Count - 1 do begin
        Sound := TSoundData(FSoundList.Objects[Idx]);
        if (Sound <> nil) then begin
          if Sound.IsPlaying then
            Sound.Stop;
          FFreeList.AddObject(FSoundList.Strings[Idx], Sound);
        end;
      end;
      FSoundList.Clear;
    end;

    if ClearType in [0, 2] then begin
      for Idx := 0 to FMusicList.Count - 1 do begin
        Sound := TSoundData(FMusicList.Objects[Idx]);
        if (Sound <> nil) then begin
          if Sound.IsPlaying then
            Sound.Stop;

          BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100);

          FFreeList.AddObject(FMusicList.Strings[Idx], Sound);
        end;
      end;
      FMusicList.Clear;
    end;

    for Idx := 0 to FFreeList.Count - 1 do begin
      Sound := TSoundData(FFreeList.Objects[Idx]);
      try
        FreeAndNil(Sound);
      except
        ErrorEvent('TBassSound.Clear Error Free sound');
      end;
    end;
    FFreeList.Clear;

    FProcSoundIdx := 0;
    FProcMusicIdx := 0;
    FTimer.Enabled := FInitialized;
  finally
    UnLock;
  end;
end;

procedure TBassSound.SetActive(Value:Boolean);
var
  Sound:TSoundData;
  Idx:Integer;
begin
  if FActive <> Value then begin
    FActive := Value;
    if (not FInitialized) then Exit;

    Lock;
    try
      if FActive then begin
        for Idx := 0 to FMusicList.Count - 1 do begin
          Sound := TSoundData(FMusicList.Objects[Idx]);
          if (Sound <> nil) and ((Sound.LoopCount > 0) or (Sound.LoopCount = -1)) then begin
            Sound.m_dwTimeTick := MyGetTickCount;
            // 修改恢复声音播放 chongchong 2015-04-01
            //Sound.Play();
            BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, g_SoundVolume / 100);
          end;
        end;
        FTimer.Enabled := FInitialized;
      end
      else begin
        FTimer.Enabled := FInitialized; // 将 False 改成了FInitialized 修改声音相关播放占用系统资源 chongchong 2017-03-26
        for Idx := 0 to FSoundList.Count - 1 do begin
          Sound := TSoundData(FSoundList.Objects[Idx]);
          if (Sound <> nil) then begin
            if Sound.IsPlaying then
              Sound.Stop;
            try
              FreeAndNil(Sound);
            except
              ErrorEvent('TBassSound.SetActive Error Free sound');
            end;
          end;
        end;
        FSoundList.Clear;

        for Idx := 0 to FMusicList.Count - 1 do begin
          Sound := TSoundData(FMusicList.Objects[Idx]);
          if (Sound <> nil) then begin
            if Sound.IsPlaying then begin
              // 停止声音播放 chongchong 2015-04-01
              //Sound.Pause;
              BASS_ChannelSetAttribute(Sound.FHandle, BASS_ATTRIB_VOL, 0);
            end;
          end;
        end;
        FProcSoundIdx := 0;
        FProcMusicIdx := 0;

      end;
    finally
      UnLock;
    end;
  end;
end;

constructor TSoundData.Create(AHandle:Cardinal; AFileName:string; ASoundType:TSoundType);
begin
  FHandle := AHandle;
  FFileName := AFileName;
  FSoundType := ASoundType;
  FPlayed := False;
  FLoopCount := 0;
  m_dwTimeTick := MyGetTickCount;

  FStopBefore := False;
end;

destructor TSoundData.Destroy;
begin
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD, //: BASS_StreamFree(FHandle);
      sntMP3,
        sntOGG, sntWAV:BASS_StreamFree(FHandle);
      //sntWAV: BASS_SampleFree(Handle); BASS_MusicFree
    end;
  end;
end;

procedure LoopSyncProc(handle:HSYNC; channel, data:DWORD; user:Pointer); stdcall;
var
  Sound:TSoundData;
begin
  if user <> nil then begin
    Sound := TSoundData(user);
    Sound.m_dwTimeTick := MyGetTickCount;

    if Sound.LoopCount > 0 then
      Sound.LoopCount := Sound.LoopCount - 1;

    if (Sound.LoopCount > 0) or (Sound.LoopCount = -1) then
      BASS_ChannelSetPosition(channel, 0, 0);
  end;
end;

function TSoundData.Play():Boolean;
begin
  Result := False;

  m_dwTimeTick := MyGetTickCount;
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:begin
          if not Played then begin
            if (FLoopCount <> 0) then
              BASS_ChannelSetSync(FHandle, BASS_SYNC_END, 0, LoopSyncProc, Self);
            BASS_ChannelFlags(FHandle, 0, BASS_MUSIC_LOOP);
          end;
          //BASS_ChannelGetInfo(Handle, ChannelInfo);
            //if (FLoopCount <> 0) then
            //  BASS_ChannelFlags(FHandle, BASS_MUSIC_LOOP, BASS_MUSIC_LOOP)
           // else
          Result := BASS_ChannelPlay(FHandle, False);
        end;
      {sntWAV: begin
          Channel := BASS_SampleGetChannel(Handle, False);
          if Looped then
            BASS_ChannelFlags(Channel, BASS_SAMPLE_LOOP, BASS_SAMPLE_LOOP)
          else
            BASS_ChannelFlags(Channel, 0, BASS_SAMPLE_LOOP);
          Result := BASS_ChannelPlay(Channel, False);
        end; }
    end;
  end;
  Played := True;
end;

function TSoundData.IsPlaying:Boolean;
begin
  Result := False;
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:begin
          Result := (BASS_ChannelIsActive(FHandle) = BASS_ACTIVE_PLAYING);
        end;
      {sntWAV: begin
          HC := BASS_SampleGetChannel(Handle, False);
          Result := (BASS_ChannelIsActive(HC) = BASS_ACTIVE_PLAYING);
          //showmessage('IsPlaying:' + booltostr(Result, true));
        end;}
    end;
  end;
end;

function TSoundData.IsPauseing:Boolean;
begin
  Result := False;
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:begin
          Result := (BASS_ChannelIsActive(FHandle) = BASS_ACTIVE_PAUSED);
        end;
      {sntWAV: begin
          HC := BASS_SampleGetChannel(Handle, False);
          Result := (BASS_ChannelIsActive(HC) = BASS_ACTIVE_PAUSED);
        end; }
    end;
  end;
end;

procedure TSoundData.Pause;
begin
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:begin
          BASS_ChannelPause(FHandle);
        end;
      {sntWAV: begin
          HC := BASS_SampleGetChannel(Handle, False);
          BASS_ChannelPause(HC);
          //showmessage(booltostr(BASS_ChannelPause(Handle), true));
          //showmessage(IntToStr(BASS_ChannelPause(HC)));
          //BASS_SampleStop(Handle);
          //showmessage('BASS_ACTIVE_PAUSED=' + booltostr((BASS_ChannelIsActive(HC) = BASS_ACTIVE_PAUSED), true));
        end;}
    end;
  end;
end;

procedure TSoundData.Stop;
begin
  Played := True;
  if FHandle <> 0 then begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:begin
          BASS_ChannelSetSync(FHandle, BASS_SYNC_END, 0, LoopSyncProc, nil);
          BASS_ChannelStop(FHandle);
          BASS_ChannelSetPosition(FHandle, 0, BASS_POS_BYTE);
        end;
      //sntWAV: BASS_SampleStop(Handle);
    end;
  end;
end;

(*
function TSoundData.GetVolume: Single;
var
  Volume: Single;
begin
  if FHandle <> 0 then
  begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:
        begin
          BASS_ChannelGetAttribute(FHandle, BASS_ATTRIB_VOL, Volume);
        end;
      {sntWAV: begin
          HC := BASS_SampleGetChannel(Handle, False);
          BASS_ChannelGetAttribute(HC, BASS_ATTRIB_VOL, Volume);
        end;}
    end;
    Result := Volume;
  end
  else
    Result := 0;
end;

procedure TSoundData.SetVolume(const Value: Single);
begin
  if FHandle <> 0 then
  begin
    case FSoundType of
      sntMOD,
        sntMP3,
        sntOGG, sntWAV:
        begin
          BASS_ChannelSetAttribute(FHandle, BASS_ATTRIB_VOL, Value);
        end;
      {sntWAV: begin
          HC := BASS_SampleGetChannel(Handle, False);
          BASS_ChannelSetAttribute(HC, BASS_ATTRIB_VOL, Value);
        end;}
    end;
  end;
end;
*)

end.
