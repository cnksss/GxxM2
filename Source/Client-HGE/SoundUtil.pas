unit SoundUtil;

interface
uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  Controls,
  Grobal2,
  ExtCtrls,
  HUtil32;
type
  TPlaySound = class(TThread)
    m_UserCriticalSection:TRTLCriticalSection;
    m_SoundList:TStringList;
    m_SoundTempList:TStringList;
  private
    { Private declarations }
    procedure Run;
  protected
    procedure Execute; override;
  public
    constructor Create;
    destructor Destroy; override;
    procedure PlaySound(nIdx:Integer);
    procedure Clear(ClearType:Integer = 0);
  end;

var
  CurVolume:Integer;
  g_SoundLock:TRTLCriticalSection;
procedure SaveToSoundFile(Stream:TMemoryStream; Index:Integer; const FileName:string);
procedure LoadSoundList(flname:string);
procedure LoadBGMusicList(flname:string);
procedure PlaySound(idx:Integer); overload;
procedure PlaySound(const sFileName:string); overload;
procedure PlaySound(const sFileName:string; LoopCount:Integer; StopBefore:Boolean = False; AlwaysPlay:Boolean = False; PlayClose:Boolean = False {关闭声音还是放}); overload;
procedure PlaySoundEx(const sFileName:string);
procedure StopSound(const sFileName:string);
procedure PlayBGM(wavname:string; ForcePlay:Boolean = False);
procedure AddBGMusic(const WavName:string);
procedure DelBGMusic(const WavName:string);
procedure PlayMp3(wavname:string; boFlag:Boolean);
procedure SilenceSound;
procedure ItemClickSound(std:TStdItem);
procedure ItemUseSound(std:TStdItem);
procedure PlayMapMusic(boFlag:Boolean);

type
  SoundInfo = record
    idx:Integer;
    Name:string;
  end;

const
  bmg_intro = 'wav\log-in-long2.wav';
  bmg_select = 'wav\sellect-loop2.wav';
  bmg_select_old = 'wav\main_theme.wav';
  bmg_field = 'wav\Field2.wav';
  bmg_gameover = 'wav\Game-over2.wav';
  bmg_gameover176 = 'wav\Game over2.wav';
  bmg_LAVA = 'wav\S6-1.wav';
  bmg_splitshadow = 'wav\splitshadow.wav'; // 分身 piaoyun 2013-07-31

  s_walk_ground_l = 1;
  s_walk_ground_r = 2;
  s_run_ground_l = 3;
  s_run_ground_r = 4;
  s_walk_stone_l = 5;
  s_walk_stone_r = 6;
  s_run_stone_l = 7;
  s_run_stone_r = 8;
  s_walk_lawn_l = 9;
  s_walk_lawn_r = 10;
  s_run_lawn_l = 11;
  s_run_lawn_r = 12;
  s_walk_rough_l = 13;
  s_walk_rough_r = 14;
  s_run_rough_l = 15;
  s_run_rough_r = 16;
  s_walk_wood_l = 17;
  s_walk_wood_r = 18;
  s_run_wood_l = 19;
  s_run_wood_r = 20;
  s_walk_cave_l = 21;
  s_walk_cave_r = 22;
  s_run_cave_l = 23;
  s_run_cave_r = 24;
  s_walk_room_l = 25;
  s_walk_room_r = 26;
  s_run_room_l = 27;
  s_run_room_r = 28;
  s_walk_water_l = 29;
  s_walk_water_r = 30;
  s_run_water_l = 31;
  s_run_water_r = 32;

  // 加入骑马声音 chongchong 2014-10-09
  s_horse_walk_ground_l = 33;
  s_horse_walk_ground_r = 34;
  s_horse_run_ground_l = 35;

  s_hit_short = 50;
  s_hit_wooden = 51;
  s_hit_sword = 52;
  s_hit_do = 53;
  s_hit_axe = 54;
  s_hit_club = 55;
  s_hit_long = 56;
  s_hit_fist = 57;

  s_struck_short = 60;
  s_struck_wooden = 61;
  s_struck_sword = 62;
  s_struck_do = 63;
  s_struck_axe = 64;
  s_struck_club = 65;

  s_struck_body_sword = 70;
  s_struck_body_axe = 71;
  s_struck_body_longstick = 72;
  s_struck_body_fist = 73;

  s_struck_armor_sword = 80;
  s_struck_armor_axe = 81;
  s_struck_armor_longstick = 82;
  s_struck_armor_fist = 83;

  // s_powerup_man         = 80;
  // s_powerup_woman       = 81;
  // s_die_man             = 82;
  // s_die_woman           = 83;
  // s_struck_man          = 84;
  // s_struck_woman        = 85;
  // s_firehit             = 86;

  // s_struck_magic        = 90;
  s_strike_stone = 91;
  s_drop_stonepiece = 92;

  s_rock_door_open = 100;
  s_intro_theme = 102;
  s_meltstone = 101;
  s_main_theme = 102;
  s_norm_button_click = 103;
  s_rock_button_click = 104;
  s_glass_button_click = 105;
  s_money = 106;
  s_eat_drug = 107;
  s_click_drug = 108;
  s_spacemove_out = 109;
  s_spacemove_in = 110;

  s_click_weapon = 111;
  s_click_armor = 112;
  s_click_ring = 113;
  s_click_armring = 114;
  s_click_necklace = 115;
  s_click_helmet = 116;
  s_click_grobes = 117;
  s_itmclick = 118;

  s_phz = 122;

  s_yedo_man = 130;
  s_yedo_woman = 131;
  s_longhit = 132;
  s_widehit = 133;
  s_rush_l = 134;
  s_rush_r = 135;
  s_firehit_ready = 136;
  s_firehit = 137;

  s_man_struck = 138;
  s_wom_struck = 139;
  s_man_die = 144;
  s_wom_die = 145;

  sf_Openbox = 'wav\Openbox.wav';
  sf_Flashbox = 'wav\Flashbox.wav';
  sf_SelectBoxFlash = 'wav\SelectBoxFlash.wav';
  sf_box2exchange = 'wav\box2exchange.wav';
  sf_box2onceagain = 'wav\box2onceagain.wav';

  sf_HeroLogOn = 'wav\HeroLogin.wav';
  sf_HeroLogOut = 'wav\HeroLogout.wav';
  sf_powerup = 'wav\powerup.wav';

  sf_newysound1 = 'wav\newysound1.wav';
  sf_newysound2 = 'wav\newysound3.wav';
  sf_newysound3 = 'wav\newysound3.wav';
  sf_newysound_mix = 'wav\newysound-mix.wav';

  sf_hit_ZRJF_M = 'wav\M56-0.wav';
  sf_hit_ZRJF_W = 'wav\M56-3.wav';

  sf_hit_Lxhy_0 = 'wav\M58-0.wav';
  sf_hit_Lxhy_3 = 'wav\M58-3.wav';

  sf_hero_shield = 'wav\hero-shield.wav';

  sf_cboZs1_start_m = 'wav\cboZs1_start_m.wav'; // +15
  sf_cboZs1_start_w = 'wav\cboZs1_start_w.wav'; // +16
  sf_cboZs2_start = 'wav\cboZs2_start.wav'; // +17
  sf_cboZs3_start_m = 'wav\cboZs3_start_m.wav'; // +18
  sf_cboZs3_start_w = 'wav\cboZs3_start_w.wav'; // +19
  sf_cboZs4_start = 'wav\cboZs4_start.wav'; // +20

  sf_cboFs1_start = 'wav\cboFs1_start.wav'; // +21
  sf_cboFs1_target = 'wav\cboFs1_target.wav'; // +22
  sf_cboFs2_start = 'wav\cboFs2_start.wav'; // +23
  sf_cboFs2_target = 'wav\cboFs2_target.wav'; // +24
  sf_cboFs3_start = 'wav\cboFs3_start.wav'; // +25
  sf_cboFs3_target = 'wav\cboFs3_target.wav'; // +26
  sf_cboFs4_start = 'wav\cboFs4_start.wav'; // +27
  sf_cboFs4_target = 'wav\cboFs4_target.wav'; // +28

  sf_cboDs1_start = 'wav\cboDs1_start.wav'; // +29
  sf_cboDs1_target = 'wav\cboDs1_target.wav'; // +30
  sf_cboDs2_start = 'wav\cboDs2_start.wav'; // +31
  sf_cboDs2_target = 'wav\cboDs2_target.wav'; // +32
  sf_cboDs3_start = 'wav\cboDs3_start.wav'; // +33
  sf_cboDs3_target = 'wav\cboDs3_target.wav'; // +34
  sf_cboDs4_start = 'wav\cboDs4_start.wav'; // +35
  sf_cboDs4_target = 'wav\cboDs4_target.wav'; // +36

  sf_xsls_death = 'wav\xsls_death.wav';
  sf_xsws_pbec = 'wav\xsws_pbec.wav';
  sf_xuanfeng = 'wav\旋风.wav';
  sf_LAVA = 'wav\S6-1.wav';
  sf_longswordhit_ground = 'wav\longsword-hit.wav'; // 开天斩 piaoyun 2013-07-31

  sf_spring = 'wav\spring.wav';

  sf_twnhit = 'wav\M42-2.wav'; // 龙影

  sf_005 = 'wav\005.wav';

  sf_dare_win = 'Wav\dare-win.wav';
var
  g_boSoundInitialized:Boolean = False;
  s_Openbox:Integer;
  s_Flashbox:Integer;
  s_SelectBoxFlash:Integer;
  s_box2exchange:Integer;
  s_box2onceagain:Integer;
  s_HeroLogOn:Integer;
  s_HeroLogOut:Integer;
  s_powerup:Integer;

  s_newysound1:Integer;
  s_newysound2:Integer;
  s_newysound3:Integer;
  s_newysound_mix:Integer;

  s_hit_ZRJF_M:Integer;
  s_hit_ZRJF_W:Integer;

  s_hit_Lxhy_0:Integer;
  s_hit_Lxhy_3:Integer;
  s_hero_shield:Integer;

  s_cboZs1_start_m:Integer = -1;
  s_cboZs1_start_w:Integer = -1;
  s_cboZs2_start:Integer = -1;
  s_cboZs3_start_m:Integer = -1;
  s_cboZs3_start_w:Integer = -1;
  s_cboZs4_start:Integer = -1;

  s_cboFs1_start:Integer = -1;
  s_cboFs1_target:Integer = -1;

  s_cboFs2_start:Integer = -1;
  s_cboFs2_target:Integer = -1;

  s_cboFs3_start:Integer = -1;
  s_cboFs3_target:Integer = -1;

  s_cboFs4_start:Integer = -1;
  s_cboFs4_target:Integer = -1;

  s_cboDs1_start:Integer = -1;
  s_cboDs1_target:Integer = -1;

  s_cboDs2_start:Integer = -1;
  s_cboDs2_target:Integer = -1;

  s_cboDs3_start:Integer = -1;
  s_cboDs3_target:Integer = -1;

  s_cboDs4_start:Integer = -1;
  s_cboDs4_target:Integer = -1;

  s_xsls_death:Integer = -1;
  s_xsws_pbec:Integer = -1;
  s_xf:Integer = -1;
  s_dare_win:Integer = -1;

implementation

uses
  BassSound,
  ClMain,
  MShare,
  UpdateEngine;

constructor TPlaySound.Create;
begin
  inherited Create(True);
  InitializeCriticalSection(m_UserCriticalSection);
  m_SoundList := TStringList.Create;
  m_SoundTempList := TStringList.Create;
  // FreeOnTerminate:=True;
  Resume;
end;

destructor TPlaySound.Destroy;
begin
  m_SoundList.Free;
  m_SoundTempList.Free;
  DeleteCriticalSection(m_UserCriticalSection);
  inherited Destroy;
end;

procedure TPlaySound.Clear(ClearType:Integer = 0);
begin
  EnterCriticalSection(m_UserCriticalSection);
  try
    m_SoundList.Clear;
    g_BassSound.Clear(ClearType);
  finally
    LeaveCriticalSection(m_UserCriticalSection);
  end;
end;

procedure TPlaySound.PlaySound(nIdx:Integer);
begin
  if (g_BassSound <> nil) and g_boSound then begin
    if (nIdx >= 0) and (nIdx < g_SoundList.Count) and (g_SoundList[nIdx] <> '') then begin
      EnterCriticalSection(m_UserCriticalSection);
      try
        m_SoundList.AddObject(IntToStr(nIdx), TObject(MyGetTickCount));
      finally
        LeaveCriticalSection(m_UserCriticalSection);
      end;
    end;
  end;
end;

procedure TPlaySound.Run;
var
  I:Integer;
  nIdx:Integer;
  TempList:TStringList;
  boUpdate:Boolean;
  sFileName:string;
begin
  if (g_BassSound <> nil) and g_boSound and g_boSoundInitialized then begin
    EnterCriticalSection(m_UserCriticalSection);
    try
      TempList := m_SoundTempList;
      m_SoundTempList := m_SoundList;
      m_SoundList := TempList;
    finally
      LeaveCriticalSection(m_UserCriticalSection);
    end;

    while m_SoundTempList.Count > MAX_SOUND_COUNT do begin
      m_SoundTempList.Delete(0);
    end;

    for I := 0 to m_SoundTempList.Count - 1 do begin
      if (not g_boSound) or g_boAppExit then break;
      nIdx := StrToIntDef(m_SoundTempList[I], -1);
      if (nIdx < 0) or (nIdx >= g_SoundList.Count) or (g_SoundList[nIdx] = '') then begin
        m_SoundTempList.Delete(I);
        Continue;
      end;

      if (MyGetTickCount - LongWord(m_SoundTempList.Objects[I])) > 80 then begin
        //m_SoundTempList.Delete(I);
        Continue;
      end;

      // m_SoundTempList.Delete(I);

      if Integer(g_SoundList.Objects[nIdx]) = 1 then begin
        try
          g_BassSound.Play(g_sSelfResourcePath + g_SoundList[nIdx]);
        except
        end;
      end
      else begin
        sFileName := GetAbsolutePathEx(g_sSelfFilePath, g_SoundList[nIdx]);
        boUpdate := False;
        EnterCriticalSection(g_SoundLock);
        try
          if FileExists(sFileName) then begin
            try
              g_BassSound.Play(sFileName);
            except
            end;
          end
          else begin
            if g_boAutoUpdate and (nIdx >= 0) and (nIdx < Length(g_SoundUpDateList)) and (g_SoundUpDateList[nIdx]) then
              boUpdate := True;
          end;
        finally
          LeaveCriticalSection(g_SoundLock);
        end;

        if boUpdate and (g_UpdateEngine <> nil) then
          g_UpdateEngine.Add(sFileName, udtFileWav, nIdx, nil, nil);
      end;
    end;

    m_SoundTempList.Clear;

    if (not g_boSound) then
      g_BassSound.Clear;
  end;
end;

procedure TPlaySound.Execute;
begin
  while (not Terminated) and (not g_boAppExit) do begin
    Run();
    Sleep(1);
  end;
end;

procedure SaveToSoundFile(Stream:TMemoryStream; Index:Integer; const FileName:string);
var
  FilePath:string;
begin
  EnterCriticalSection(g_SoundLock);
  try
    if (Stream <> nil) and (not FileExists(FileName)) then begin
      FilePath := ExtractFilePath(FileName);

      if not DirectoryExists(FilePath) then begin
        ForceDirectories(FilePath);
      end;

      try
        Stream.SaveToFile(FileName);
      except

      end;
    end;
  finally
    LeaveCriticalSection(g_SoundLock);
  end;

  if UpperCase(ExtractFileExt(ExtractFileName(FileName))) = '.LST' then begin
    LoadSoundList('.\wav\sound.lst');
  end;

  if (Index >= 0) and (Index < Length(g_SoundUpDateList)) then
    g_SoundUpDateList[Index] := False;
end;

procedure LoadSoundList(flname:string);
var
  I, k, idx, n:Integer;
  strlist:TStringList;
  Str, Data:string;
  boUpdate:Boolean;
begin
  EnterCriticalSection(g_SoundLock);
  try
    boUpdate := False;
    g_boSoundInitialized := False;
    g_SoundList.Clear;
    SetLength(g_SoundUpDateList, 0);
    if FileExists(flname) then begin
      strlist := TStringList.Create;
      strlist.LoadFromFile(flname);
      idx := 0;
      for I := 0 to strlist.Count - 1 do begin
        Str := strlist[I];
        if Str <> '' then begin
          if Str[1] = ';' then Continue;
          Str := Trim(GetValidStr3(Str, Data, [':', ' ', #9]));
          n := StrToIntDef(Data, 0);
          if n > idx then begin
            for k := 0 to n - g_SoundList.Count - 1 do
              g_SoundList.Add('');
            g_SoundList.Add(Str);

            if FileExists(g_sSelfResourcePath + g_SoundList[g_SoundList.Count - 1]) then
              g_SoundList.Objects[g_SoundList.Count - 1] := TObject(1);

            idx := n;
          end;
        end;
      end;
      strlist.Free;
      g_boSoundInitialized := True;
    end
    else begin
      if g_boAutoUpdate then
        boUpdate := True;
    end;

    s_Openbox := g_SoundList.Count;
    s_Flashbox := g_SoundList.Count + 1;
    s_SelectBoxFlash := g_SoundList.Count + 2;
    s_box2exchange := g_SoundList.Count + 3;
    s_box2onceagain := g_SoundList.Count + 4;
    s_HeroLogOn := g_SoundList.Count + 5;
    s_HeroLogOut := g_SoundList.Count + 6;
    s_powerup := g_SoundList.Count + 7;
    s_newysound1 := g_SoundList.Count + 8;
    s_newysound2 := g_SoundList.Count + 9;
    s_newysound3 := g_SoundList.Count + 10;
    s_newysound_mix := g_SoundList.Count + 11;
    s_hit_ZRJF_M := g_SoundList.Count + 12;
    s_hit_ZRJF_W := g_SoundList.Count + 13;
    s_hit_Lxhy_0 := g_SoundList.Count + 14;
    s_hit_Lxhy_3 := g_SoundList.Count + 15;
    s_hero_shield := g_SoundList.Count + 16;

    s_cboZs1_start_m := g_SoundList.Count + 17;
    s_cboZs1_start_w := g_SoundList.Count + 18;
    s_cboZs2_start := g_SoundList.Count + 19;
    s_cboZs3_start_m := g_SoundList.Count + 20;
    s_cboZs3_start_w := g_SoundList.Count + 21;
    s_cboZs4_start := g_SoundList.Count + 22;
    s_cboFs1_start := g_SoundList.Count + 23;
    s_cboFs1_target := g_SoundList.Count + 24;
    s_cboFs2_start := g_SoundList.Count + 25;
    s_cboFs2_target := g_SoundList.Count + 26;
    s_cboFs3_start := g_SoundList.Count + 27;
    s_cboFs3_target := g_SoundList.Count + 28;
    s_cboFs4_start := g_SoundList.Count + 29;
    s_cboFs4_target := g_SoundList.Count + 30;
    s_cboDs1_start := g_SoundList.Count + 31;
    s_cboDs1_target := g_SoundList.Count + 32;
    s_cboDs2_start := g_SoundList.Count + 33;
    s_cboDs2_target := g_SoundList.Count + 34;
    s_cboDs3_start := g_SoundList.Count + 35;
    s_cboDs3_target := g_SoundList.Count + 36;
    s_cboDs4_start := g_SoundList.Count + 37;
    s_cboDs4_target := g_SoundList.Count + 38;
    s_xsls_death := g_SoundList.Count + 39;
    s_xsws_pbec := g_SoundList.Count + 40;
    s_xf := g_SoundList.Count + 41;
    s_dare_win := g_SoundList.Count + 47;

    g_SoundList.Add(sf_Openbox);
    g_SoundList.Add(sf_Flashbox);
    g_SoundList.Add(sf_SelectBoxFlash);
    g_SoundList.Add(sf_box2exchange);
    g_SoundList.Add(sf_box2onceagain);

    g_SoundList.Add(sf_HeroLogOn);
    g_SoundList.Add(sf_HeroLogOut);
    g_SoundList.Add(sf_powerup);

    g_SoundList.Add(sf_newysound1);
    g_SoundList.Add(sf_newysound2);
    g_SoundList.Add(sf_newysound3);
    g_SoundList.Add(sf_newysound_mix);
    g_SoundList.Add(sf_hit_ZRJF_M);
    g_SoundList.Add(sf_hit_ZRJF_W);
    g_SoundList.Add(sf_hit_Lxhy_0);
    g_SoundList.Add(sf_hit_Lxhy_3);
    g_SoundList.Add(sf_hero_shield);

    g_SoundList.Add(sf_cboZs1_start_m);
    g_SoundList.Add(sf_cboZs1_start_w);
    g_SoundList.Add(sf_cboZs2_start);
    g_SoundList.Add(sf_cboZs3_start_m);
    g_SoundList.Add(sf_cboZs3_start_w);
    g_SoundList.Add(sf_cboZs4_start);

    g_SoundList.Add(sf_cboFs1_start);
    g_SoundList.Add(sf_cboFs1_target);
    g_SoundList.Add(sf_cboFs2_start);
    g_SoundList.Add(sf_cboFs2_target);
    g_SoundList.Add(sf_cboFs3_start);
    g_SoundList.Add(sf_cboFs3_target);
    g_SoundList.Add(sf_cboFs4_start);
    g_SoundList.Add(sf_cboFs4_target);

    g_SoundList.Add(sf_cboDs1_start);
    g_SoundList.Add(sf_cboDs1_target);
    g_SoundList.Add(sf_cboDs2_start);
    g_SoundList.Add(sf_cboDs2_target);
    g_SoundList.Add(sf_cboDs3_start);
    g_SoundList.Add(sf_cboDs3_target);
    g_SoundList.Add(sf_cboDs4_start);
    g_SoundList.Add(sf_cboDs4_target);
    g_SoundList.Add(sf_xsls_death);
    g_SoundList.Add(sf_xsws_pbec);
    g_SoundList.Add(sf_xuanfeng);
    g_SoundList.Add(sf_LAVA);
    g_SoundList.Add(sf_longswordhit_ground);
    g_SoundList.Add(sf_spring);
    g_SoundList.Add(sf_twnhit);
    g_SoundList.Add(sf_005);
    g_SoundList.Add(sf_dare_win);

    for I := s_Openbox to g_SoundList.Count - 1 do begin
      if FileExists(g_sSelfResourcePath + g_SoundList[I]) then
        g_SoundList.Objects[I] := TObject(1);
    end;

    SetLength(g_SoundUpDateList, g_SoundList.Count);
    for I := 0 to Length(g_SoundUpDateList) - 1 do
      g_SoundUpDateList[I] := True;

    {for I := 0 to g_SoundList.Count - 1 do
    begin
      g_SoundUpDateList.Add(Pointer(0));
      if FileExists(g_sSelfFilePath + 'Resources\' + g_SoundList[I]) then
        g_SoundUpDateList.Items[I] := Pointer(0)
      else if FileExists(g_sSelfFilePath + g_SoundList[I]) then
        g_SoundUpDateList.Items[I] := Pointer(0)
      else
        g_SoundUpDateList.Items[I] := Pointer(1);
    end;

    g_SoundList.SaveToFile('g_SoundList.txt');
    g_BassSound.SoundList := g_SoundList;
    }
  finally
    LeaveCriticalSection(g_SoundLock);
  end;

  if boUpdate and (g_UpdateEngine <> nil) then
    g_UpdateEngine.Add(GetWavFiles(g_sSelfFilePath + 'wav\sound.lst'), udtFileWav, -1, nil, nil);

  // 保存文件。。。。。。。。。。。
  //g_SoundList.SaveToFile('C:\MUSIC.TXT');
end;

procedure PlaySound(const sFileName:string);
var
  boUpdate:Boolean;
  sTemp:string;
begin
  if g_boSound then begin
    sTemp := GetAbsolutePathEx(g_sSelfFilePath + 'Wav\', sFileName);
    boUpdate := False;
    EnterCriticalSection(g_SoundLock);
    try
      if FileExists(sTemp) then begin
        try
          g_BassSound.Play(sTemp);
        except
        end;
      end
      else begin
        if g_boAutoUpdate then
          boUpdate := True;
      end;
    finally
      LeaveCriticalSection(g_SoundLock);
    end;
    if boUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(sTemp), udtFileWav, -1, nil, nil);
  end;
end;

procedure PlaySoundEx(const sFileName:string);
var
  boUpdate:Boolean;
  sTemp:string;
begin
  if g_boSound then begin
    sTemp := GetAbsolutePathEx(g_sSelfFilePath, sFileName);

    boUpdate := False;
    EnterCriticalSection(g_SoundLock);
    try
      if FileExists(sTemp) then begin
        try
          g_BassSound.Play(sTemp);
        except
        end;
      end
      else begin
        if g_boAutoUpdate then
          boUpdate := True;
      end;
    finally
      LeaveCriticalSection(g_SoundLock);
    end;
    if boUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(sTemp), udtFileWav, -1, nil, nil);
  end;
end;

procedure StopSound(const sFileName:string);
var
  sTemp:string;
begin
  if not g_boSound then Exit;
  sTemp := GetAbsolutePathEx(g_sSelfFilePath, sFileName);
  EnterCriticalSection(g_SoundLock);
  try
    if FileExists(sTemp) then begin
      try
        g_BassSound.Stop(sTemp);
        g_BassSound.StopMusic(sTemp);
      except
      end;
    end;
  finally
    LeaveCriticalSection(g_SoundLock);
  end;
end;

procedure PlaySound(idx:Integer);
var
  boUpdate:Boolean;
begin
  if g_boSound then begin
    if (idx >= 0) and (idx < g_SoundList.Count) then begin
      if g_SoundList[idx] <> '' then begin
        boUpdate := False;
        EnterCriticalSection(g_SoundLock);
        try
          if Integer(g_SoundList.Objects[idx]) = 1 then begin
            try
              g_BassSound.Play(g_sSelfResourcePath + g_SoundList[idx]);
            except
            end;
          end
          else begin
            if FileExists(g_sSelfFilePath + g_SoundList[idx]) then begin
              try
                g_BassSound.Play(g_sSelfFilePath + g_SoundList[idx]);
              except
              end;
            end
            else begin
              if g_boAutoUpdate and (idx >= 0) and (idx < Length(g_SoundUpDateList)) and g_SoundUpDateList[idx] then
                boUpdate := True;
            end;
          end;
        finally
          LeaveCriticalSection(g_SoundLock);
        end;

        if boUpdate and (g_UpdateEngine <> nil) then
          g_UpdateEngine.Add(GetWavFiles(g_sSelfFilePath + g_SoundList[idx]), udtFileWav, idx, nil, nil);
      end;
    end;
  end;
end;

procedure PlaySound(const sFileName:string; LoopCount:Integer; StopBefore:Boolean; AlwaysPlay:Boolean; PlayClose:Boolean);
var
  boUpdate:Boolean;
  sTemp:string;
begin
  if g_boSound or AlwaysPlay or PlayClose then begin
    sTemp := GetAbsolutePathEx(g_sSelfFilePath, sFileName);

    boUpdate := False;
    EnterCriticalSection(g_SoundLock);
    try
      if FileExists(sTemp) then begin
        // 无限次数播放 2019-08-01 00:52:05
        if AlwaysPlay then LoopCount := -1;

        try
          if g_boRepeatBGSound then
            g_BassSound.PlayMusic(sTemp, LoopCount, StopBefore)
          else
            g_BassSound.PlayMusic(sTemp, 0, StopBefore);
        except
        end;
      end
      else begin
        if g_boAutoUpdate then
          boUpdate := True;
      end;
    finally
      LeaveCriticalSection(g_SoundLock);
    end;
    if boUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(sTemp), udtFileWav, -1, nil, nil);
  end;
end;

procedure PlayMapMusic(boFlag:Boolean);
var
  sFileName:string;
begin
  if (g_sMapMusic = '') or (not boFlag) or (not FileExists(g_sMapMusic)) then begin
    PlayMp3('', False);
    Exit;
  end;
  sFileName := g_sMapMusic; // '.\Music\' + IntToStr(g_nMapMusic) + '.mp3';
  PlayMp3(sFileName, boFlag);
end;

procedure LoadBGMusicList(flname:string);
var
  strlist:TStringList;
  Str, sMapName, sFileName:string;
  pFileName:^string;
  I:Integer;
begin
  if FileExists(flname) then begin
    strlist := TStringList.Create;
    strlist.LoadFromFile(flname);
    for I := 0 to strlist.Count - 1 do begin
      Str := strlist[I];
      if (Str = '') or (Str[1] = ';') then Continue;
      Str := GetValidStr3(Str, sMapName, [':', ' ', #9]);
      Str := GetValidStr3(Str, sFileName, [':', ' ', #9]);
      sMapName := Trim(sMapName);
      sFileName := Trim(sFileName);

      if (sMapName <> '') and (sFileName <> '') then begin
        New(pFileName);
        pFileName^ := sFileName;
        BGMusicList.AddObject(sMapName, TObject(pFileName));
      end;
    end;
    strlist.Free;
  end;
end;

procedure PlayBGM(wavname:string; ForcePlay:Boolean = False);
var
  boUpdate:Boolean;
  sTemp:string;
begin
  if (not g_boBGSound) and (not ForcePlay) then Exit;
  if wavname <> '' then begin
    sTemp := GetAbsolutePathEx(g_sSelfFilePath, wavname);

    boUpdate := False;
    EnterCriticalSection(g_SoundLock);
    try
      if FileExists(sTemp) then begin
        try
          g_BassSound.Clear;
          g_BassSound.PlayMusic(sTemp);
        except
        end;
      end
      else begin
        if g_boAutoUpdate then
          boUpdate := True;
      end;
    finally
      LeaveCriticalSection(g_SoundLock);
    end;

    if boUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(sTemp), udtFileWav, -1, nil, nil);
  end;
end;

procedure AddBGMusic(const WavName:string);
var
  FileName:string;
begin
  if not g_boBGSound then Exit;
  if (Length(WavName) = 0) then Exit;

  FileName := GetAbsolutePathEx(g_sSelfFilePath, WavName);

  if (not FileExists(FileName)) then begin
    if g_boAutoUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(FileName), udtFileWav, -1, nil, nil);
    Exit;
  end;

  EnterCriticalSection(g_SoundLock);
  try
    try
      g_BassSound.PlayMusic(FileName);
    except
    end;
  finally
    LeaveCriticalSection(g_SoundLock);
  end;
end;

procedure DelBGMusic(const WavName:string);
var
  FileName:string;
begin
  if not g_boBGSound then Exit;

  FileName := g_sSelfFilePath + WavName;
  if (Length(WavName) = 0) or (not FileExists(FileName)) then Exit;

  EnterCriticalSection(g_SoundLock);
  try
    try
      g_BassSound.StopMusic(FileName);
    except
    end;
  finally
    LeaveCriticalSection(g_SoundLock);
  end;
end;

procedure PlayMp3(wavname:string; boFlag:Boolean);
var
  boUpdate:Boolean;
begin
  if not boFlag then begin
    g_BassSound.Clear;
    Exit;
  end;

  if not g_boBGSound then Exit;

  if wavname <> '' then begin
    boUpdate := False;
    EnterCriticalSection(g_SoundLock);
    try
      if FileExists(wavname) then begin
        try
          // DScreen.AddChatBoardString('PlayMp31:', clRed, clWhite);

          g_BassSound.Clear;

          if g_boRepeatBGSound then
            g_BassSound.PlayMusic(wavname)
          else
            g_BassSound.PlayMusic(wavname, 0);
        except
        end;
      end
      else begin
        if g_boAutoUpdate then
          boUpdate := True;
      end;
    finally
      LeaveCriticalSection(g_SoundLock);
    end;

    if boUpdate and (g_UpdateEngine <> nil) then
      g_UpdateEngine.Add(GetWavFiles(wavname), udtFileWav, -1, nil, nil);
  end;
end;

procedure SilenceSound;
begin
  g_PlaySound.Clear;
end;

procedure ItemClickSound(std:TStdItem);
begin
  case std.StdMode of
    0:PlaySound(s_click_drug);
    31: // if std.AniCount > 0 then
      PlaySound(s_itmclick);
    // else
    // PlaySound(s_click_drug);
    5, 6, 68, 69:PlaySound(s_click_weapon);
    10, 11, 66, 67:PlaySound(s_click_armor);
    22, 23, 81, 82:PlaySound(s_click_ring);
    24, 26, 79, 80:begin
        if (Pos('手镯', std.Name) > 0) or (Pos('手套', std.Name) > 0) then
          PlaySound(s_click_grobes)
        else
          PlaySound(s_click_armring);
      end;
    19, 20, 21, 75, 76, 77:PlaySound(s_click_necklace);
    15, 78:PlaySound(s_click_helmet);
    else
      PlaySound(s_itmclick);
  end;
end;

procedure ItemUseSound(std:TStdItem);
begin
  case std.StdMode of
    0:PlaySound(s_click_drug);
    1:PlaySound(s_eat_drug);
    2:begin
        case std.Shape of
          1..3:PlaySound(s_itmclick);
          else
            PlaySound(s_eat_drug);
        end;
      end;
    else
      ;
  end;
end;
initialization
  InitializeCriticalSection(g_SoundLock);
finalization
  DeleteCriticalSection(g_SoundLock);
end.
