unit clEvent;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  Controls,
  Forms,
  Dialogs,
  HGE,
  Grobal2,
  ExtCtrls,
  HUtil32,
  EDcode,
  SDK,
  SoundUtil,
  DxCanvas,
  HGECanvas;

const
  ZOMBIDIGUPDUSTBASE = 420;
  STONEFRAGMENTBASE = 64;
  HOLYCURTAINBASE = 1390;
  FIREBURNBASE = 1630;
  SCULPTUREFRAGMENT = 1349;
  FIREFLOWERBASE = 60;
type
  TClEvent = class
    m_nX:Integer;
    m_nY:Integer;
    m_nDir:Integer;
    m_nPx:Integer;
    m_nPy:Integer;
    m_nEventType:Integer;
    m_nEventParam:Integer;
    m_nServerId:Int64;
    m_Dsurface:TTexture;
    m_boBlend:Boolean;
    m_dwFrameTime:longword;
    m_dwFrameTickTime:longword;
    m_dwCurfRame:Integer;
    m_nLight:Integer;
    m_boVisible:Boolean;
    m_dwLoadSurfaceTime:longword;
    m_dwGhostTick:longword;
    m_boKeepShow:Boolean;
    m_boLoadSound:Boolean;
  private
  public
    constructor Create(svid:Int64; ax, ay, evtype:Integer);
    destructor Destroy; override;
    procedure DrawEvent(ax, ay:Integer); virtual;
    procedure Run; virtual;
    procedure Initialize;
    procedure Finalize;
    procedure LoadSurface(Sender:TObject); virtual;

  end;

  TMapEffectEvent = class(TClEvent)
    m_nFileIndex, m_nImageIndex, m_nImageCount, m_nLoopCount:Integer;
  private
    FPlayCount:Integer;
  public
    constructor Create(nServerId:Int64; nX, nY, nFileIndex, nImageIndex, nImageCount, nSpeedTime, nLoopCount:Integer; boBlend:Boolean; btLight:Integer);
    procedure Run; override;
    procedure LoadSurface(Sender:TObject); override;
  end;

  TCustomEffectEvent = class(TCLEvent)
  private
    FFileIndex, FImageIndex, FImageIndex2, FImageCount:Integer;
    FAppr:Word;
    FBlend2:Boolean;

    m_nPx2:Integer;
    m_nPy2:Integer;
    m_Dsurface2:TTexture;

  public
    constructor Create(svid:Int64; ax, ay, evtype:Integer; wAppr:Word; AttackIndex:Integer);
    procedure Run; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawEvent(ax, ay:Integer); override;
  end;

  TCustomMagicEffectEvent = class(TCLEvent)
  private
    FFileIndex, FImageIndex, FImageIndex2, FImageCount:Integer;
    FBlend2:Boolean;
    m_nPx2:Integer;
    m_nPy2:Integer;
    m_Dsurface2:TTexture;
  public
    constructor Create(svid:Int64; ax, ay, evtype:Integer; wMagicID:Word; wNewLevel:Word);
    procedure Run; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawEvent(ax, ay:Integer); override;
  end;

  TClEventManager = class
  private
  public
    EventList:TGList;
    constructor Create;
    destructor Destroy; override;
    procedure ClearEvents;
    function AddEvent(evn:TClEvent):TClEvent;
    procedure DelEvent(evn:TClEvent);
    procedure DelEventById(svid:Int64);
    function GetEvent(ax, ay, etype:Integer):TClEvent;
    procedure Execute;
    procedure Initialize;
    procedure Finalize;
  end;

implementation

uses
  ClMain,
  GameImages,
  MShare,
  Actor,
  GameConfigDlg;

constructor TMapEffectEvent.Create(nServerId:Int64; nX, nY, nFileIndex, nImageIndex, nImageCount, nSpeedTime, nLoopCount:Integer; boBlend:Boolean; btLight:Integer);
begin
  inherited Create(nServerId, nX, nY, ET_MAPEFFECT);
  m_boBlend := boBlend;
  m_nFileIndex := nFileIndex;
  m_nImageIndex := nImageIndex;
  m_nImageCount := nImageCount;
  m_dwFrameTickTime := nSpeedTime;
  m_nLoopCount := nLoopCount;
  m_dwCurframe := -1;
  m_nLight := btLight;

  FPlayCount := 0;
end;

procedure TMapEffectEvent.LoadSurface(Sender:TObject);
var
  GameImages:TGameImages;
begin
  if not m_boVisible then Exit;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if g_MySelf = nil then Exit;
  m_dwLoadSurfaceTime := MyGetTickCount;
  g_EffectImageList.Lock;
  try
    if (m_nFileIndex >= 0) and (m_nFileIndex < g_EffectImageList.Count) then begin
      GameImages := TGameImages(g_EffectImageList.Objects[m_nFileIndex]);
      if GameImages <> nil then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := GameImages.GetCachedGrayImage(m_nImageIndex + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := GameImages.GetCachedImage(m_nImageIndex + m_dwCurframe, m_nPx, m_nPy);
      end;
    end;
  finally
    g_EffectImageList.UnLock;
  end;
end;

procedure TMapEffectEvent.Run;
var
  dwCurframe:Integer;
begin
  if not m_boVisible then Exit;
  if (m_nFileIndex >= 0) and (m_nFileIndex < g_EffectImageList.Count) then begin
    dwCurframe := m_dwCurframe;
    if MyGetTickCount - m_dwFrameTime > m_dwFrameTickTime then begin
      m_dwFrameTime := MyGetTickCount;
      Inc(m_dwCurframe);
    end;
    if (m_dwCurframe < 0) or (m_dwCurframe >= m_nImageCount) then begin
      // if m_nLoopCount <= 0 then m_boVisible := False;
      // Dec(m_nLoopCount);
      m_dwCurframe := 0;
      Inc(FPlayCount);
    end;

    if (FPlayCount > m_nLoopCount) then begin
      m_boVisible := False;
      Exit;
    end;

    // 修改载入间隔  chongchong 2016-02-23  5 * 1000 （本单元所有5*1000全改成了2 * 1000

      // DScreen.AddChatBoardString('m_dwCurframe:'+IntToStr(m_dwCurframe), clBlack, clWhite);
    if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurframe <> m_dwCurframe) then begin
      PlayScene.LoadSurface(LoadSurface);
    end;
  end;
end;

constructor TClEvent.Create(svid:Int64; ax, ay, evtype:Integer);
begin
  m_nServerId := svid;
  m_nX := ax;
  m_nY := ay;
  m_nEventType := evtype;
  m_nEventParam := 0;
  m_boBlend := False;
  m_dwFrameTime := MyGetTickCount;
  m_dwCurframe := 0;
  m_nLight := 0;
  m_boVisible := True;
  m_dwFrameTickTime := 20;
  m_Dsurface := nil;
  m_dwLoadSurfaceTime := 0; // MyGetTickCount;
  m_boKeepShow := False;

  m_boLoadSound := False;
end;

destructor TClEvent.Destroy;
begin
  inherited Destroy;
end;

procedure TClEvent.Initialize;
begin

end;

procedure TClEvent.Finalize;
begin
  m_Dsurface := nil;
end;

procedure TClEvent.LoadSurface(Sender:TObject);
var
  Offset:Integer;
begin
  if not m_boVisible then Exit;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if g_MySelf = nil then Exit;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_Dsurface := nil;
  case m_nEventType of
    ET_DIGOUTZOMBI:
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        m_Dsurface := g_WMonImages.Indexs[6].GetCachedGrayImage(ZOMBIDIGUPDUSTBASE + m_nDir, m_nPx, m_nPy)
      else
        m_Dsurface := g_WMonImages.Indexs[6].GetCachedImage(ZOMBIDIGUPDUSTBASE + m_nDir, m_nPx, m_nPy);
    ET_PILESTONES:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WEffectImg.GetCachedGrayImage(STONEFRAGMENTBASE + (m_nEventParam - 1), m_nPx, m_nPy)
        else
          m_Dsurface := g_WEffectImg.GetCachedImage(STONEFRAGMENTBASE + (m_nEventParam - 1), m_nPx, m_nPy);
      end;
    ET_HOLYCURTAIN:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagicImages.GetCachedGrayImage(HOLYCURTAINBASE + (m_dwCurframe mod 10), m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagicImages.GetCachedImage(HOLYCURTAINBASE + (m_dwCurframe mod 10), m_nPx, m_nPy);
      end;
    ET_SAFERECT:begin
        case m_nDir of
          DR_UP:Offset := 2050;
          DR_UPRIGHT:Offset := 2060;
          DR_RIGHT:Offset := 2070;

          DR_DOWNRIGHT:Offset := 2080;
          DR_DOWN:Offset := 2090;
          DR_DOWNLEFT:Offset := 2100;
          DR_LEFT:Offset := 2110;
          DR_UPLEFT:Offset := 2040;
          else
            Offset := -1;
        end;

        if Offset > 0 then begin
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            m_Dsurface := g_WMagic10Images.GetCachedGrayImage(Offset, m_nPx, m_nPy)
          else
            m_Dsurface := g_WMagic10Images.GetCachedImage(Offset, m_nPx, m_nPy);

          if m_nDir in [DR_DOWNLEFT, DR_UPLEFT] then
            m_nPx := 6 // 31
          else
            m_nPx := 0; // 25
          m_nPy := 0; // 13
        end;
      end;
    ET_FIRE:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagicImages.GetCachedGrayImage(FIREBURNBASE + ((m_dwCurframe div 2) mod 6), m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagicImages.GetCachedImage(FIREBURNBASE + ((m_dwCurframe div 2) mod 6), m_nPx, m_nPy);
      end;
    ET_FIRELevel1,
      ET_FIRELevel2,
      ET_FIRELevel3:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagic7Images16.GetCachedGrayImage(90 + (m_nEventType - ET_FIRELevel1) * 10 + ((m_dwCurframe div 2) mod 8), m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagic7Images16.GetCachedImage(90 + (m_nEventType - ET_FIRELevel1) * 10 + ((m_dwCurframe div 2) mod 8), m_nPx, m_nPy);
      end;

    ET_SCULPEICE:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMonImages.Indexs[7].GetCachedGrayImage(SCULPTUREFRAGMENT, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMonImages.Indexs[7].GetCachedImage(SCULPTUREFRAGMENT, m_nPx, m_nPy);
      end;
    ET_FIREFLOWER_1..ET_FIREFLOWER_8:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagic3Images.GetCachedGrayImage(FIREFLOWERBASE + 20 * (m_nEventType - ET_FIREFLOWER_1) + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagic3Images.GetCachedImage(FIREFLOWERBASE + 20 * (m_nEventType - ET_FIREFLOWER_1) + m_dwCurframe, m_nPx, m_nPy);
      end;

    ET_ICEPEAK:begin // 雪域卫士 冰峰效果
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMonImages.Indexs[27].GetCachedGrayImage(2010 + 10 * m_nEventParam + 9, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMonImages.Indexs[27].GetCachedImage(2010 + 10 * m_nEventParam + 9, m_nPx, m_nPy);
      end;

    ET_HOLYCURTAIN2:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagic10Images.GetCachedGrayImage(970 + (m_dwCurframe mod 15), m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagic10Images.GetCachedImage(970 + (m_dwCurframe mod 15), m_nPx, m_nPy);
      end;

    // 地图效果 -- piaoyun 2013-07-10
    ET_THUNDER:begin
        // if {g_WDragonImg}g_MySelf <> nil then
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WDragonImg.GetCachedGrayImage(420 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WDragonImg.GetCachedImage(420 + m_dwCurframe, m_nPx, m_nPy);
      end;
    ET_LAVA:begin
        // if {g_WDragonImg}g_MySelf <> nil then
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WDragonImg.GetCachedGrayImage(470 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WDragonImg.GetCachedImage(470 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 火龙守护兽 特效 piaoyun 2013-08-19
    ET_FIREDRAGON:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WDragonImg.GetCachedGrayImage(350 + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WDragonImg.GetCachedImage(350 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // Mon33-7 光圈特效 piaoyun 2013-12-01
    ET_FIREMON33_7:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMonImages.Indexs[33].GetCachedGrayImage(2670 + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMonImages.Indexs[33].GetCachedImage(2670 + m_dwCurframe, m_nPx, m_nPy);
      end;

    { TODO -ochongchong -c新增 : 加入地图魔法事件触发 【2013-08-21】}

    // 地钉效果 地图特效 chongchong 2013-08-20
    ET_DEDING:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMonImages.Indexs[14].GetCachedGrayImage(410 + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMonImages.Indexs[14].GetCachedImage(410 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 闪光效果  地图特效 chongchong 2013-08-20
    ET_FLASHLIGHT:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagic3Images.GetCachedGrayImage(20 + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WMagic3Images.GetCachedImage(20 + m_dwCurframe, m_nPx, m_nPy)
      end;

    // 岩桨效果 chongchong 2013-08-21
    ET_LAVA2:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WDragonImg.GetCachedGrayImage(440 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WDragonImg.GetCachedImage(440 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 龙头燃烧 chongchong 2013-08-21
    ET_FIREDRAGON2:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WDragonImg.GetCachedGrayImage(350 + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := g_WDragonImg.GetCachedImage(350 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 传送门 chongchong 2013-08-21
    ET_DOOR1..ET_DOOR5:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(4490 + (m_nEventType - ET_DOOR1) * 10 { GetNpcOffset(m_nEventType - ET_DOOR1 + 54)} + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WNpcImgImages.Indexs[0].GetCachedImage(4490 + (m_nEventType - ET_DOOR1) * 10 { GetNpcOffset(m_nEventType - ET_DOOR1 + 54)} + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 白色雷电 chongchong 2013-08-21
    ET_THUNDER2:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMagic2Images.GetCachedGrayImage(10 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WMagic2Images.GetCachedImage(10 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 三眼泉 1 chongchong 2013-09-14
    ET_SPRINGS1..ET_SPRINGS3:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMain2Images.GetCachedGrayImage(550 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WMain2Images.GetCachedImage(550 + m_dwCurframe, m_nPx, m_nPy);
      end;

    // 三眼泉闪光 chongchong 2013-09-14
    ET_SPRINGS_LIGHT:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_WMain2Images.GetCachedGrayImage(670 + m_dwCurframe, m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_WMain2Images.GetCachedImage(670 + m_dwCurframe, m_nPx, m_nPy);
      end;
    ET_CUSTOM_SAFE_POINT1..ET_CUSTOM_SAFE_POINT2:begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := g_SafePointEffect.GetCachedGrayImage((m_nEventType - ET_CUSTOM_SAFE_POINT1) * 10 + (m_dwCurframe mod 10), m_nPx, m_nPy)
            {PlaySoundEx(bmg_skill_11); }
        else
          m_Dsurface := g_SafePointEffect.GetCachedImage((m_nEventType - ET_CUSTOM_SAFE_POINT1) * 10 + (m_dwCurframe mod 10), m_nPx, m_nPy);
      end;
  end;
end;

procedure TClEvent.DrawEvent(ax, ay:Integer);
var
  nPx, nPY:Integer;
  RRR:TTexture;
begin
  if not m_boVisible then Exit;
  if m_Dsurface <> nil then begin
    nPx := m_nPx;
    nPY := m_nPY;

    // 几张图合成一个效果 chongchong 2013-09-14
    if m_nEventType in [ET_SPRINGS1, ET_SPRINGS2, ET_SPRINGS3] then begin
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        RRR := g_WMain2Images.GetCachedGrayImage(530 + m_nEventType - ET_SPRINGS1, m_nPx, m_nPy)
      else
        RRR := g_WMain2Images.GetCachedImage(530 + m_nEventType - ET_SPRINGS1, m_nPx, m_nPy);

      if RRR <> nil then begin
        if m_boBlend then
          GameCanvas.DrawBlend(ax + m_nPx, ay + m_nPy, RRR)
        else
          GameCanvas.Draw(ax + m_nPx, ay + m_nPy, RRR);
      end;

      // 对坐标，让泉水和泉眼对上 chongchong
      nPx := 7;
      nPY := -50;
    end;

    //HZQ 20230601 m_EventType = 5 //火墙, 添加火墙淡化功能    g_ClientConfig.boDimFireEffect
    if (m_nEventType = 5) and (g_ConfigDlg.ConfigCheckeds[ckDimFireEffect]) then begin
      if m_boBlend then begin
        GameCanvas.DrawColorAlpha(ax + nPx, ay + nPY, m_Dsurface.ClientRect, m_Dsurface, $00FFFFFF, 96, Blend_SrcAlphaAdd);
      end else begin
        GameCanvas.DrawAlpha(ax + nPx, ay + nPY, m_Dsurface, 96);
      end;
    end else begin
      if m_boBlend then begin
        GameCanvas.DrawBlend(ax + nPx, ay + nPY, m_Dsurface)
      end else begin
        GameCanvas.Draw(ax + nPx, ay + nPY, m_Dsurface);
      end;
    end;
  end;
end;

procedure TClEvent.Run;
var
  dwCurframe:Integer;
  dwCurHolyCurtainframe:Integer;
  dwCurHolyCurtainframe2:Integer;
  dwCurFireframe:Integer;
begin
  if not m_boVisible then Exit;
  dwCurHolyCurtainframe := m_dwCurframe mod 10;
  dwCurHolyCurtainframe2 := m_dwCurframe mod 15;
  dwCurFireframe := 0;

  if m_nEventType in [ET_FIRELevel1,
    ET_FIRELevel2,
    ET_FIRELevel3] then
    dwCurFireframe := ((m_dwCurframe div 2) mod 8)
  else if m_nEventType = ET_FIRE then
    dwCurFireframe := ((m_dwCurframe div 2) mod 6);

  if MyGetTickCount - m_dwFrameTime > m_dwFrameTickTime then begin
    m_dwFrameTime := MyGetTickCount;
    Inc(m_dwCurframe);
    // Inc(dwCurFireframe);
  end;
  case m_nEventType of
    ET_DIGOUTZOMBI:begin
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) then begin
          PlayScene.LoadSurface(LoadSurface);
        end;
      end;
    ET_PILESTONES:begin
        dwCurframe := m_nEventParam;
        if m_nEventParam <= 0 then m_nEventParam := 1;
        if m_nEventParam > 5 then m_nEventParam := 5;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurframe <> m_nEventParam) then
          PlayScene.LoadSurface(LoadSurface);
      end;

    ET_HOLYCURTAIN:begin
        m_boBlend := True;
        m_nLight := 1;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurHolyCurtainframe <> m_dwCurframe mod 10) then
          PlayScene.LoadSurface(LoadSurface);
      end;
    ET_SAFERECT:begin
        m_boBlend := True;
        m_nLight := 1;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) then
          PlayScene.LoadSurface(LoadSurface);
      end;
    ET_FIRE:begin
        m_boBlend := True;
        m_nLight := 1;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> ((m_dwCurframe div 2) mod 6)) then
          PlayScene.LoadSurface(LoadSurface);
      end;
    ET_FIRELevel1,
      ET_FIRELevel2,
      ET_FIRELevel3:begin
        m_boBlend := True;
        m_nLight := 2;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> ((m_dwCurframe div 2) mod 8)) then
          PlayScene.LoadSurface(LoadSurface);
      end;

    ET_SCULPEICE:begin
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) then begin
          PlayScene.LoadSurface(LoadSurface);
        end;
      end;
    ET_FIREFLOWER_1..ET_FIREFLOWER_8:begin
        if m_dwCurframe >= 20 then begin
          m_dwCurframe := 0;
          m_boVisible := False;
        end
        else begin
          // 修正烟花播放声音不对 + m_boLoadSound，防止多次播放 chongchong 2019-03-04 12:32:25
          if (m_dwCurframe = 0) and (not m_boLoadSound) then begin
            case m_nEventType of
              ET_FIREFLOWER_1:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_2:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_3:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_4:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_5:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_6:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_7:g_PlaySound.PlaySound(s_newysound_mix);
              ET_FIREFLOWER_8:g_PlaySound.PlaySound(s_newysound_mix);
            end;
            m_boLoadSound := True;
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);

        end;
        m_boBlend := True;
        m_nLight := 1;
      end;
    ET_ICEPEAK:begin // 雪域卫士 冰峰效果
        m_boBlend := False;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (m_Dsurface = nil) then begin
          // DScreen.AddChatBoardString('TClEvent.Run:'+IntToStr(m_nServerId), clRed, clBlue);
          PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    ET_HOLYCURTAIN2:begin
        m_boBlend := True;
        m_nLight := 1;
        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurHolyCurtainframe2 <> m_dwCurframe mod 15) then
          PlayScene.LoadSurface(LoadSurface);
      end;

    // 地图效果 -- piaoyun 2013-07-10
    ET_THUNDER: {// 闪电} begin
        if m_dwCurframe >= 4 then {// 只有4帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (m_dwCurframe = 0) and (not m_boLoadSound) then {// 第一帧时候播放声音  修正音爆 and (not m_boLoadSound) 2019-11-14 12:50:13} begin
            g_PlaySound.PlaySound(1923);
            m_boLoadSound := True;
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
        m_boBlend := True;
        m_nLight := 1;
      end;

    ET_LAVA: {// 岩浆} begin
        if m_dwCurframe >= 10 then {// 只有10帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (m_dwCurframe = 0) and (not m_boLoadSound) then {// 第一帧时候播放声音  修正音爆 and (not m_boLoadSound) 2019-11-14 12:50:13} begin
            g_PlaySound.PlaySound(11055);
            m_boLoadSound := True;
          end;
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
        m_boBlend := True;
        m_nLight := 1;
      end;

    // 火龙守护兽 特效 piaoyun 2013-08-19
    ET_FIREDRAGON:begin
        if m_dwCurframe >= 35 then begin
          m_dwCurframe := 0;
          m_boVisible := False;
        end
        else begin
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;

        m_boBlend := TRUE;
        m_nLight := 1;
      end;

    // Mon33-7 光圈特效 piaoyun 2013-12-01
    ET_FIREMON33_7:begin
        if m_dwCurframe >= 7 then begin
          m_dwCurframe := 1;
          //m_boVisible := False;
        end
        else begin
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;

        m_boBlend := TRUE;
        m_nLight := 1;
      end;

    // 地钉效果 地图特效 chongchong 2013-08-20
    ET_DEDING:begin
        if m_dwCurframe >= 6 then {// 只有6帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    // 闪光效果 地图特效 chongchong 2013-08-20
    ET_FLASHLIGHT:begin
        m_boBlend := TRUE;
        m_nLight := 1;
        if m_dwCurframe >= 10 then {// 只有6帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    ET_LAVA2: {// 岩浆2} begin
        m_boBlend := True;
        m_nLight := 1;
        if m_dwCurframe >= 20 then {// 只有20帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (m_dwCurframe = 0) and (not m_boLoadSound) then {// 第一帧时候播放声音  修正音爆 and (not m_boLoadSound) 2019-11-14 12:50:13} begin
            g_PlaySound.PlaySound(10090);
            m_boLoadSound := False;
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    // 龙头燃烧 chongchong 2013-08-21
    ET_FIREDRAGON2:begin
        if m_dwCurframe >= 35 then begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (m_dwCurframe = 0) and (not m_boLoadSound) then {// 第一帧时候播放声音  修正音爆 and (not m_boLoadSound) 2019-11-14 12:50:13} begin
            g_PlaySound.PlaySound(10090);
            m_boLoadSound := False;
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;

        m_boBlend := TRUE;
        m_nLight := 1;
      end;

    ET_DOOR1..ET_DOOR5:begin
        m_boBlend := True;
        m_nLight := 1;
        if m_dwCurframe >= 10 then {// 只有10帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    ET_THUNDER2:begin
        m_boBlend := True;
        m_nLight := 1;
        if m_dwCurframe >= 5 then {// 只有5帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if (m_dwCurframe = 0) and (not m_boLoadSound) then {// 第一帧时候播放声音  修正音爆 and (not m_boLoadSound) 2019-11-14 12:50:13} begin
            g_PlaySound.PlaySound(1923);
            m_boLoadSound := False;
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;

    // 三种泉水，就泉眼不一样
    ET_SPRINGS1, ET_SPRINGS2, ET_SPRINGS3:begin
        m_boBlend := True;
        m_nLight := 1;
        if m_dwCurframe >= 12 then {// 只有12帧} begin
          m_dwCurframe := 0;
          if not m_boKeepShow then
            m_boVisible := False;
        end
        else begin
          if m_dwCurframe = 0 then begin
            //g_PlaySound.PlaySound(11057);                    // 第一帧时候播放声音
          end;
          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;
    ET_SPRINGS_LIGHT:begin
        m_boBlend := True;
        m_nLight := 1;
        if m_dwCurframe >= 18 then {// 只有18帧} begin
          m_dwCurframe := 0;
          m_boVisible := False;
        end
        else begin
          if m_dwCurframe = 0 then begin
            //g_PlaySound.PlaySound(11057);                    // 第一帧时候播放声音
          end;

          if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurFireframe <> m_dwCurframe) then
            PlayScene.LoadSurface(LoadSurface);
        end;
      end;
    // 20 - 75为扩展自定义安全区光圈特效 chongchong 2017-04-18
    ET_CUSTOM_SAFE_POINT1..ET_CUSTOM_SAFE_POINT2:begin
        m_boBlend := m_nEventType <= 55;
        m_nLight := 1;

        if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurHolyCurtainframe <> m_dwCurframe mod 10) then
          PlayScene.LoadSurface(LoadSurface);
      end;
  end;
end;

{-----------------------------------------------------------------------------}

{-----------------------------------------------------------------------------}

constructor TClEventManager.Create;
begin
  EventList := TGList.Create;
end;

destructor TClEventManager.Destroy;
var
  I:Integer;
begin
  for I := 0 to EventList.Count - 1 do
    TClEvent(EventList[I]).Free;
  EventList.Free;
  inherited Destroy;
end;

procedure TClEventManager.Initialize;
var
  I:Integer;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do
      TClEvent(EventList[I]).Initialize;
  finally
    EventList.UnLock;
  end;
end;

procedure TClEventManager.Finalize;
var
  I:Integer;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do
      TClEvent(EventList[I]).Finalize;
  finally
    EventList.UnLock;
  end;
end;

procedure TClEventManager.ClearEvents;
var
  I:Integer;
  evn:TClEvent;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do begin
      evn := TClEvent(EventList[I]);
      evn.m_dwGhostTick := MyGetTickCount;
      AddFreeEventList(evn);
    end;
    EventList.Clear;
  finally
    EventList.UnLock;
  end;
end;

function TClEventManager.AddEvent(evn:TClEvent):TClEvent;
var
  I:Integer;
  Event:TClEvent;
  boFind:Boolean;

  MapEvent1:TMapEffectEvent;
  MapEvent2:TMapEffectEvent;
begin
  Result := nil; //HZQ 20230524 添加初始值
  EventList.Lock;
  try
    // 修复MAPEFFECT在同一个坐标播放多个不同wil中的资源，只能播放一个
    // MAPEFFECT <$map> <$X> <$Y> 3 380 20 10 190 0 5
    // MAPEFFECT <$map> <$X> <$Y> 4 80 6 20 190 0 5
    if evn is TMapEffectEvent then begin
      MapEvent1 := evn as TMapEffectEvent;

      boFind := False;
      for I := 0 to EventList.Count - 1 do begin
        Event := EventList[I];
        if Event is TMapEffectEvent then begin
          MapEvent2 := Event as TMapEffectEvent;

          //if (Event = evn) or (TClEvent(EventList[I]).m_nServerId = evn.m_nServerId) then
          if (MapEvent2.m_nEventType = MapEvent1.m_nEventType) and
            (MapEvent2.m_nEventParam = MapEvent1.m_nEventParam) and
            (MapEvent2.m_nX = MapEvent1.m_nX) and (MapEvent2.m_nY = MapEvent1.m_nY) and
            (MapEvent2.m_nDir = MapEvent1.m_nDir) and (MapEvent2.m_nFileIndex = MapEvent1.m_nFileIndex) and

          // 修正
          // MAPEFFECT <$MAP> <$X> <$Y> 3 100 60 10 100 1
          // MAPEFFECT <$MAP> <$X> <$Y> 3 200 20 15 200 1
          // 不能同时执行 chongchong 2015-09-23

          (MapEvent2.m_nImageIndex = MapEvent1.m_nImageIndex) and
            (MapEvent2.m_nImageCount = MapEvent1.m_nImageCount) and
            (MapEvent2.m_nLoopCount = MapEvent1.m_nLoopCount) then begin
            MapEvent1.m_dwCurfRame := 0;
            MapEvent1.m_boVisible := True;
            boFind := True;
            Result := MapEvent1;
            break;
          end;
        end;
      end;
    end else begin
      boFind := False;
      for I := 0 to EventList.Count - 1 do begin
        Event := EventList[I];
        //if (Event = evn) or (TClEvent(EventList[I]).m_nServerId = evn.m_nServerId) then
        if (Event.m_nEventType = evn.m_nEventType) and
          (Event.m_nEventParam = evn.m_nEventParam) and
          (Event.m_nX = evn.m_nX) and (Event.m_nY = evn.m_nY) and
          (Event.m_nDir = evn.m_nDir) and

        // 修正烟花特效可以多次播放 chongchong 2019-03-06 20:51:34
        (not (Event.m_nEventType in [ET_FIREFLOWER_1..ET_FIREFLOWER_8])) then begin
          evn.m_dwCurfRame := 0;
          evn.m_boVisible := True;
          boFind := True;
          Result := evn;
          break;
        end;
      end;
    end;

    if not boFind then begin
      EventList.Add(evn);
      Result := evn;
    end else begin
      evn.Free;
    end;
  finally
    EventList.UnLock;
  end;
end;

procedure TClEventManager.DelEvent(evn:TClEvent);
var
  I:Integer;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do begin
      if EventList[I] = evn then begin
        evn.m_boVisible := False;
        Break;
      end;
    end;
  finally
    EventList.UnLock;
  end;
end;

procedure TClEventManager.DelEventById(svid:Int64);
var
  I:Integer;
  evn:TClEvent;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do begin
      evn := TClEvent(EventList[I]);
      if evn.m_nServerId = svid then begin
        evn.m_boVisible := False;
        Break;
      end;
    end;
  finally
    EventList.UnLock;
  end;
end;

function TClEventManager.GetEvent(ax, ay, etype:Integer):TClEvent;
var
  I:Integer;
begin
  Result := nil;
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do
      if (TClEvent(EventList[I]).m_nX = ax) and (TClEvent(EventList[I]).m_nY = ay) and
        (TClEvent(EventList[I]).m_nEventType = etype) then begin
        Result := TClEvent(EventList[I]);
        Break;
      end;
  finally
    EventList.UnLock;
  end;
end;

procedure TClEventManager.Execute;
var
  I:Integer;
begin
  EventList.Lock;
  try
    for I := 0 to EventList.Count - 1 do
      TClEvent(EventList[I]).Run;
  finally
    EventList.UnLock;
  end;
end;

{ TCustomEffectEvent }

constructor TCustomEffectEvent.Create(svid:Int64; ax, ay, evtype:Integer;
  wAppr:Word; AttackIndex:Integer);
var
  I:Integer;
  MonsterConfig:PClientCustomMonsterConfig;
begin
  inherited Create(svid, ax, ay, ET_CUSTOM_EFF);
  FFileIndex := -2;
  FImageIndex := -1;
  FImageIndex2 := -1;
  FImageCount := 0;
  FAppr := wAppr;

  m_nPx2 := 0;
  m_nPy2 := 0;
  m_Dsurface2 := nil;

  if (AttackIndex >= Low(TClientAttackConfigs)) and (AttackIndex <= High(TClientAttackConfigs)) then begin
    for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
      MonsterConfig := g_CustomMonsterConfig.Items[I];
      if MonsterConfig.wMonsterAppr = wAppr then begin
        m_boBlend := MonsterConfig.AttackConfigs[AttackIndex].Target_DrawMode = mdmBlend;
        FBlend2 := MonsterConfig.AttackConfigs[AttackIndex].Target_DrawMode2 = mdmBlend;

        m_dwFrameTickTime := MonsterConfig.AttackConfigs[AttackIndex].Target_PlayTime;

        if (MonsterConfig.AttackConfigs[AttackIndex].Fly_StartIndex >= 0) and
          (MonsterConfig.AttackConfigs[AttackIndex].Fly_PlayCount > 0) and
          ((MonsterConfig.AttackConfigs[AttackIndex].Explosion_StartIndex >= 0) or (MonsterConfig.AttackConfigs[AttackIndex].Explosion_StartIndex2 >= 0)) and
          (MonsterConfig.AttackConfigs[AttackIndex].Explosion_PlayCount > 0) and
          (MonsterConfig.AttackConfigs[AttackIndex].Explosion_KeepPlay) and
          (MonsterConfig.AttackConfigs[AttackIndex].Explosion_KeepTime > 0) then begin
          FFileIndex := MonsterConfig.AttackConfigs[AttackIndex].Explosion_File;
          FImageIndex := MonsterConfig.AttackConfigs[AttackIndex].Explosion_StartIndex;
          FImageIndex2 := MonsterConfig.AttackConfigs[AttackIndex].Explosion_StartIndex2;
          FImageCount := MonsterConfig.AttackConfigs[AttackIndex].Explosion_PlayCount;

          m_nLight := MonsterConfig.AttackConfigs[AttackIndex].Explosion_KeepLightRange;
        end
        else begin
          FFileIndex := MonsterConfig.AttackConfigs[AttackIndex].Target_File;
          FImageIndex := MonsterConfig.AttackConfigs[AttackIndex].Target_StartIndex;
          FImageIndex2 := MonsterConfig.AttackConfigs[AttackIndex].Target_StartIndex2;
          FImageCount := MonsterConfig.AttackConfigs[AttackIndex].Target_PlayCount;

          m_nLight := MonsterConfig.AttackConfigs[AttackIndex].Target_KeepLightRange;
        end;
        Break;
      end;
    end;
  end;

end;

procedure TCustomEffectEvent.LoadSurface(Sender:TObject);
var
  GameImages:TGameImages;
begin
  if not m_boVisible then Exit;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if g_MySelf = nil then Exit;

  if FFileIndex = -2 then Exit;
  if (FImageIndex = -1) and (FImageIndex2 = -1) then Exit;
  if FImageCount = 0 then Exit;

  m_dwLoadSurfaceTime := MyGetTickCount;

  g_EffectImageList.Lock;
  try
    if (FFileIndex >= 0) and (FFileIndex < g_EffectImageList.Count) then
      GameImages := TGameImages(g_EffectImageList.Objects[FFileIndex])
    else
      GameImages := g_WMonImages.Images[FAppr];

    if GameImages <> nil then begin
      if FImageIndex >= 0 then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface := GameImages.GetCachedGrayImage(FImageIndex + m_dwCurframe, m_nPx, m_nPy)
        else
          m_Dsurface := GameImages.GetCachedImage(FImageIndex + m_dwCurframe, m_nPx, m_nPy);
      end;

      if FImageIndex2 >= 0 then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface2 := GameImages.GetCachedGrayImage(FImageIndex2 + m_dwCurframe, m_nPx2, m_nPy2)
        else
          m_Dsurface2 := GameImages.GetCachedImage(FImageIndex2 + m_dwCurframe, m_nPx2, m_nPy2);
      end;
    end;
  finally
    g_EffectImageList.UnLock;
  end;
end;

procedure TCustomEffectEvent.Run;
var
  dwCurframe:Integer;
begin
  if not m_boVisible then Exit;
  if (FFileIndex >= -1) and (FFileIndex < g_EffectImageList.Count) then begin
    dwCurframe := m_dwCurframe;
    if MyGetTickCount - m_dwFrameTime > m_dwFrameTickTime then begin
      m_dwFrameTime := MyGetTickCount;
      Inc(m_dwCurframe);
    end;
    if (m_dwCurframe < 0) or (m_dwCurframe >= FImageCount) then begin
      m_dwCurframe := 0;
    end;
    if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurframe <> m_dwCurframe) then begin
      PlayScene.LoadSurface(LoadSurface);
    end;
  end;
end;

procedure TCustomEffectEvent.DrawEvent(ax, ay:Integer);
var
  nPx, nPY:Integer;
begin
  if not m_boVisible then Exit;
  inherited DrawEvent(ax, ay);

  if m_Dsurface2 <> nil then begin
    nPx := m_nPx2;
    nPY := m_nPY2;

    if FBlend2 then
      GameCanvas.DrawBlend(ax + nPx, ay + nPY, m_Dsurface2)
    else
      GameCanvas.Draw(ax + nPx, ay + nPY, m_Dsurface2);
  end;
end;

{ TCustomMagicEffectEvent }

constructor TCustomMagicEffectEvent.Create(svid:Int64; ax, ay, evtype:Integer;
  wMagicID, wNewLevel:Word);
var
  I:Integer;
  MagicConfig:PClientCustomMagicConfig;

  PlusLevel:TMagicPlusLevel;
begin
  // 自定义特效可以多次播放 2021-03-13 12:21:05
  // inherited Create(svid, ax, ay, ET_CUSTOM_EFF);
  inherited Create(svid, ax, ay, ET_CUSTOM_EFF + wMagicID);
  FFileIndex := -2;
  FImageIndex := -1;
  FImageIndex2 := -1;
  FImageCount := 0;

  m_nPx2 := 0;
  m_nPy2 := 0;
  m_Dsurface2 := nil;

  case wNewLevel of
    0:PlusLevel := mplNone;
    1..3:PlusLevel := mpl1_3;
    4..6:PlusLevel := mpl4_6;
    else
      PlusLevel := mpl7_9;
  end;

  for I := 0 to g_CustomMagicConfig.Count - 1 do begin
    MagicConfig := g_CustomMagicConfig.Items[I];
    if MagicConfig.wMagicID = wMagicID then begin
      m_boBlend := MagicConfig.MagicConfigs[PlusLevel].Target_DrawMode = mdmBlend;
      FBlend2 := MagicConfig.MagicConfigs[PlusLevel].Target_DrawMode2 = mdmBlend;
      m_dwFrameTickTime := MagicConfig.MagicConfigs[PlusLevel].Target_PlayTime;

      FFileIndex := MagicConfig.MagicConfigs[PlusLevel].Target_File;
      FImageIndex := MagicConfig.MagicConfigs[PlusLevel].Target_StartIndex;
      FImageIndex2 := MagicConfig.MagicConfigs[PlusLevel].Target_StartIndex2;
      FImageCount := MagicConfig.MagicConfigs[PlusLevel].Target_PlayCount;
      m_nLight := MagicConfig.MagicConfigs[PlusLevel].Target_KeepLightRange;
      Break;
    end;
  end;
end;

procedure TCustomMagicEffectEvent.DrawEvent(ax, ay:Integer);
var
  nPx, nPY:Integer;
begin
  if not m_boVisible then Exit;
  inherited DrawEvent(ax, ay);

  if m_Dsurface2 <> nil then begin
    nPx := m_nPx2;
    nPY := m_nPY2;

    if FBlend2 then
      GameCanvas.DrawBlend(ax + nPx, ay + nPY, m_Dsurface2)
    else
      GameCanvas.Draw(ax + nPx, ay + nPY, m_Dsurface2);
  end;
end;

procedure TCustomMagicEffectEvent.LoadSurface(Sender:TObject);
var
  GameImages:TGameImages;
begin
  if not m_boVisible then Exit;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if g_MySelf = nil then Exit;

  if (FFileIndex < 0) or (FFileIndex >= g_EffectImageList.Count) then Exit;
  if (FImageIndex < 0) and (FImageIndex2 < 0) then Exit;
  if FImageCount = 0 then Exit;

  m_dwLoadSurfaceTime := MyGetTickCount;

  g_EffectImageList.Lock;
  try
    GameImages := TGameImages(g_EffectImageList.Objects[FFileIndex]);

    if GameImages <> nil then begin
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        m_Dsurface := GameImages.GetCachedGrayImage(FImageIndex + m_dwCurframe, m_nPx, m_nPy)
      else
        m_Dsurface := GameImages.GetCachedImage(FImageIndex + m_dwCurframe, m_nPx, m_nPy);

      if FImageIndex2 >= 0 then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          m_Dsurface2 := GameImages.GetCachedGrayImage(FImageIndex2 + m_dwCurframe, m_nPx2, m_nPy2)
        else
          m_Dsurface2 := GameImages.GetCachedImage(FImageIndex2 + m_dwCurframe, m_nPx2, m_nPy2);
      end;
    end;
  finally
    g_EffectImageList.UnLock;
  end;
end;

procedure TCustomMagicEffectEvent.Run;
var
  dwCurframe:Integer;
begin
  if not m_boVisible then Exit;
  if (FFileIndex >= -1) and (FFileIndex < g_EffectImageList.Count) then begin
    dwCurframe := m_dwCurframe;
    if MyGetTickCount - m_dwFrameTime > m_dwFrameTickTime then begin
      m_dwFrameTime := MyGetTickCount;
      Inc(m_dwCurframe);
    end;
    if (m_dwCurframe < 0) or (m_dwCurframe >= FImageCount) then begin
      m_dwCurframe := 0;
    end;
    if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or (dwCurframe <> m_dwCurframe) then begin
      PlayScene.LoadSurface(LoadSurface);
    end;
  end;
end;

end.
