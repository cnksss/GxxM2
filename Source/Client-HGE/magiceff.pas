unit magiceff;

interface

uses
  Windows,
  SysUtils,
  Classes,
  Grobal2,
  HGE,
  SDK,
  ClFunc,
  HUtil32,
  MagicImageOffset,
  GameImages,
  HGECanvas;

const
  MG_READY = 10;
  MG_FLY = 6;
  MG_EXPLOSION = 10;
  READYTIME = 120;
  EXPLOSIONTIME = 100;
  FLYBASE = 10;
  EXPLOSIONBASE = 170;
  // EFFECTFRAME = 260;
  MAXMAGIC = 10;
  FLYOMAAXEBASE = 447;
  THORNBASE = 2967;
  ARCHERBASE = 2607;
  ARCHERBASE2 = 272; // 2609;

  FLYFORSEC = 500;
  FIREGUNFRAME = 6;

  MAXMAGICTYPE = 16;

type
  TMagicEffectBase = array[0..MAXEFFECT - 1] of Integer;
  pTMagicEffectBase = ^TMagicEffectBase;
  TMagicHitEffectBase = array[0..MAXHITEFFECT - 1] of Integer;
  pTMagicHitEffectBase = ^TMagicHitEffectBase;

  // piaoyun 2013-08-19
  TMagicType = (mtReady, {准备} mtFly, {飞} mtExplosion {爆发},
    mtFlyAxe {飞斧}, mtFireWind {火风}, mtFireGun {火炮},
    mtLightingThunder {照明雷}, mtThunder {雷1}, mtExploBujauk,
    mtBujaukGroundEffect, mtKyulKai, mtFlyArrow,
    mt12, mt13 {怪物魔法}, mt14,
    mt15, mt16, mtRedThunder {红色雷电}, mtLava, {岩浆}
    mtFlyArrowEx
    );

  TUseMagicInfo = record
    ServerMagicCode:Integer;
    MagicSerial:Integer;
    target:Int64; // recogcode
    EffectType:TMagicType;
    EffectNumber:Integer;
    targx:Integer;
    targy:Integer;
    Recusion:Boolean;
    anitime:Integer;
    NewLevel:Byte; // 强化等级  几重
    MagicLevel:Byte; // 技能等级 (只有部分技能传过来了) 4级技能强化 -- 4级灵魂火符 4级灭天火  chongchong 2013-12-04
    MagicItemType:Boolean;
  end;
  PTUseMagicInfo = ^TUseMagicInfo;

  TMagicEff = class // Size 0xC8
    m_dwGhostTick:LongWord;
    m_boActive:Boolean; // 0x04
    ServerMagicId:Integer; // 0x08
    MagicId:Integer;
    EffectNumber:Integer;
    MagOwner:TObject; // 0x0C
    TargetActor:TObject; // 0x10
    ImgLib:TGameImages; // 0x14
    EffectBase:Integer; // 0x18           // 魔法预备效果           -- piaoyun 2013-6-17
    MagExplosionBase:Integer; // 0x1C           // 魔法爆发展现的效果偏移 -- piaoyun 2013-6-17
    px, py:Integer; // 0x20 0x24
    rx, ry:Integer; // 0x28 0x2C
    Dir16, OldDir16:byte; // 0x30 0x31
    targetx, targety:Integer; // 0x34 0x38
    TargetRx, TargetRy:Integer; // 0x3C 0x40
    FlyX, FlyY, OldFlyX, OldFlyY:Integer; // 0x44 0x48 0x4C 0x50
    FlyXf, FlyYf:Real; // 0x54 0x5C
    Repetition:Boolean; // 0x64
    FixedEffect:Boolean; // 0x65
    MagicType:TMagicType; // 0x68
    NextEffect:TMagicEff; // 0x6C
    ExplosionFrame:Integer; // 0x70          // 特效图片数量 -- piaoyun 2013-6-17
    NextFrameTime:Integer; // 0x74
    light:Integer; // 0x78
    n7C:Integer;
    bt80:byte;
    bt81:byte;
    start:Integer; // 0x84
    curframe:Integer; // 0x88
    frame:Integer; // 0x8C
    NewLevel:Integer;
    m_nCurrentFrame:Integer;

    m_DrawBlend:Boolean;

    m_LastRunTick:LongWord;
  private
    m_dwRunTime:longword;
    m_dwFrameTime:longword; // 0x90
    m_dwStartTime:longword; // 0x94
    repeattime:longword; // 0x98 馆汗 局聪皋捞记 矫埃 (-1: 拌加)
    steptime:longword; // 0x9C
    fireX, fireY:Integer; // 0xA0 0xA4
    firedisX, firedisY:Integer; // 0xA8 0xAC
    newfiredisX, newfiredisY:Integer; // 0xB0 0xB4
    FireMyselfX, FireMyselfY:Integer; // 0xB8 0xBC
    prevdisx, prevdisy:Integer; // 0xC0 0xC4
  protected
    procedure GetFlyXY(ms:Integer; var fx, fy:Integer);
  public
    constructor Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer);
    destructor Destroy; override;
    function Run:Boolean; virtual; // False:场车澜.
    function Shift:Boolean; virtual;
    procedure DrawEff(); virtual;
    procedure LoadSurface(Sender:TObject); virtual;
  end;

  TCopySelf = class(TMagicEff)
  public
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TFlyingAxe = class(TMagicEff)
    FlyImageBase:Integer;
    ReadyFrame:Integer;
  public
    constructor Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer);
    procedure DrawEff(); override;
    function Run:Boolean; override;
  end;

  TFlyingBug = class(TMagicEff) // Size 0xD0
    FlyImageBase:Integer; // 0xC8
    ReadyFrame:Integer; // 0xCC
  public
    constructor Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TFlyingArrow = class(TFlyingAxe)
  public
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TFlyingArrowEx = class(TFlyingAxe)
  public
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TFlyingFireBall = class(TFlyingAxe) // 0xD0
  public
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
    function Run:Boolean; override;
  end;

  TCharEffect = class(TMagicEff)
  public
    constructor Create(effbase, effframe:Integer; target:TObject);
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TMapEffect = class(TMagicEff)
  public
    RepeatCount:Integer;
    constructor Create(effbase, effframe:Integer; X, Y:Integer);
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TScrollHideEffect = class(TMapEffect)
  public
    constructor Create(effbase, effframe:Integer; X, Y:Integer; target:TObject);
    function Run:Boolean; override;
  end;

  TLightingEffect = class(TMagicEff)
  public
    constructor Create(effbase, effframe:Integer; X, Y:Integer);
    function Run:Boolean; override;
  end;

  TFireNode = record
    X:Integer;
    Y:Integer;
    firenumber:Integer;
  end;

  TFireGunEffect = class(TMagicEff)
  public
    OutofOil:Boolean;
    firetime:longword;
    FireNodes:array[0..FIREGUNFRAME - 1] of TFireNode;
    constructor Create(effbase, sx, sy, tx, ty:Integer);
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TThuderEffect = class(TMagicEff)
  public
    constructor Create(effbase, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TLightingThunder = class(TMagicEff)
  public
    constructor Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TExploBujaukEffect = class(TMagicEff)
    MagicBlend:Boolean;
  public
    constructor Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    function Run:Boolean; override;
    function Shift:Boolean; override;
  end;

  // 4级技能强化 -- 4级灵魂火符 （符飞行） chongchong 2013-12-04
  TExploBujaukEffect4 = class(TMagicEff)
    MagicBlend:Boolean;
  public
    constructor Create(sx, sy, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    function Run:Boolean; override;
  end;

  // 月灵特效类 piaoyun 2013-11-14
  TMoonMonEffect = class(TMagicEff)
    MagicBlend:Boolean;
  public
    constructor Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    //function Run: Boolean; override;
  end;

  // 扩展的一个符类特效 -- piaoyun 2013-6-24
  {TExploBujaukEffectEx = class(TMagicEff)
    MagicNumber: integer;
    MagicBlend: Boolean;
  public
    constructor Create(effbase, sx, sy, tx, ty: integer; Target: TObject);
    procedure DrawEff(); override;
  end; }

  // 红色闪电 piaoyun 2013-08-19
  TRedThunderEffect = class(TMagicEff)
    n0:integer;
  public
    constructor Create(effbase, tx, ty:integer; target:TObject);
    procedure DrawEff(); override;
  end;

  // 岩浆 piaoyun 2013-08-19
  {
  TLavaEffect = class(TMagicEff)
  public
    constructor Create(effbase, tx, ty: integer; target: TObject; nframe: Integer);
    procedure DrawEff(); override;
  end;
  }

  // 火龙特效 piaoyun 2013-08-19
  TFireDragonEffect = class(TMagicEff)
    FlyX1, FlyY1, FlyX2, FlyY2:Integer;
    boflyFixedEffect:Boolean;
  public
    constructor Create(id, effnum, sx, sy, tx, ty:integer; mtype:TMagicType; Recusion:Boolean; anitime:integer);
    procedure DrawEff(); override;
  end;

  TBujaukGroundEffect = class(TMagicEff) // Size  0xD0
  public
    MagicNumber:Integer; // 0xC8
    BoGroundEffect:Boolean; // 0xCC
    constructor Create(effbase, magicnumb, sx, sy, tx, ty:Integer);
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TNormalDrawEffect = class(TMagicEff) // Size 0xCC
    boC8:Boolean;
  public
    constructor Create(xx, yy:Integer; WMImage:TGameImages; effbase, nX:Integer; frmTime:longword; boFlag:Boolean);
    function Run:Boolean; override;
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  THeroShowEffect = class(TMagicEff)
  public
    constructor Create(effbase, effframe:Integer; target:TObject); overload;
    constructor Create(effbase, effframe, nX, nY:Integer); overload;
    function Shift:Boolean; override;
  end;

  TShowPlayEffect = class(TMagicEff)
  public
    constructor Create(effbase, effframe:Integer; target:TObject);
    procedure DrawEff(); override;
  end;

  TPlayEffect = class(TMagicEff)
    m_nMaxCount:Integer;
    m_nPlayCount:Integer;
    m_boBlend:Boolean;
  public
    constructor Create(Images:TGameImages; nCurrX, nCurrY, nImageStart, nImageCount, nPlayCount:Integer; boBlend:Boolean; Target:TObject);
    function Run:Boolean; override;
    function Shift:Boolean; override;
  end;

  TBloodBiteEffect = class(TMagicEff) // 噬血术
  public
    constructor Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer; ANewLevel:Integer);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TContinuousEffect = class(TMagicEff) // 连击
  public
    constructor Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    function Run:Boolean; override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TExploSanYanZhouEffect = class(TMagicEff) // 三焰咒
  public
    OutofOil:Boolean;
    firetime:longword;
    fire2time:Longword;
    FireNodes:array[0..FIREGUNFRAME - 1] of TFireNode;

    constructor Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    function Run:Boolean; override;
  end;

  TExploHuXiaoJueZhouEffect = class(TMagicEff) // 虎啸诀
  public
    constructor Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  TExploBingtianxuediEffect = class(TMagicEff) // 冰天雪地
  public
    constructor Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
    procedure DrawEff(); override;
    //procedure LoadSurface(Sender: TObject); override;
  end;

  // 自定义怪物类飞行特效
  TCustomMonFlyEffect = class(TMagicEff)
  private
    FExplosionImgLib:TGameImages;
    FOnExplosion:TNotifyEvent;
    FTigerOnExplosion:Boolean;

    FIsPlaySound:Boolean;

    FIsFireGunMode:Boolean;
    FOutofOil:Boolean;
    FFiretime:LongWord;
    FireNodes:array of TFireNode;

    FTigerOnFinished:Boolean;
    FOnFinished:TNotifyEvent;
  public
    MagExplosionBase2:Integer;

    MagicBlend:Boolean;
    MagicBlend2:Boolean;
    FlyEffImgLib:TGameImages;
    FlyEffStartIndex:Integer;
    FlyDrawMode:TCustomDrawMode;
    FlyEffDrawMode:TCustomDrawMode;
    NextExplosionFrameTime:Integer;
    ExplosionLockTarget:Boolean;

    FlyLightRange:Byte;
    ExplosionLightRange:Byte;

    LockTarget:Int64;
    LockTargetX, LockTargetY:Integer;

    FTargetList:TList;
    ClientConfig:TMagicClientConfig;
  public
    function Run:Boolean; override;
    function Shift:Boolean; override;
  public
    constructor Create(effbase, sx, sy, tx, ty:Integer; target:TObject;
      FlyFrameCount:Integer; AExplosionImgLib:TGameImages; AExplosionLockTarget:Boolean;
      AIsFireGunMode:Boolean {地狱火模式});
    procedure DrawEff(); override;
    destructor Destroy; override;
    property OnExplosion:TNotifyEvent read FOnExplosion write FOnExplosion;
    property OnFinished:TNotifyEvent read FOnFinished write FOnFinished;
  end;

  // 自定义怪物目标特效
  TCustomMonTargetEffect = class(TMagicEff)
  private
    FTigerOnFinished:Boolean;
    FOnFinished:TNotifyEvent;
  public
    MagExplosionBase2:Integer;
    DrawMode:TCustomDrawMode;
    DrawMode2:TCustomDrawMode;
    FTargetList:TList;
    ClientConfig:TMagicClientConfig;
    LockTarget:Int64;
    LockX, LockY:Integer;
  public
    constructor Create(effbase, effBase2, effframe:Integer; target:TObject); overload;
    constructor Create(effbase, effBase2, effframe, nX, nY:Integer); overload;
    destructor Destroy; override;
    function Shift:Boolean; override;
    procedure DrawEff(); override;
    property OnFinished:TNotifyEvent read FOnFinished write FOnFinished;
  end;

  TJNExploBujaukEffect = class(TMagicEff)
  public
    constructor Create(effbase, sx, sy, tx, ty:integer; target:TObject);
    procedure DrawEff(); override;
  end;

  TExplosion2Effect = class(TMagicEff)
  private
    IsPlayExplosion2:Boolean;
  public

    MagExplosionBase_2:Integer;
    ExplosionFrame_2:Integer;
    constructor Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer);
    function Run:Boolean; override;
  end;

procedure GetEffectBase(mag, mtype:Integer; var wimg:TGameImages; var idx:Integer; NewLevel:Integer = 0);

implementation

uses
  ClMain,
  Actor,
  SoundUtil,
  MShare;

// 取得魔法效果所在图库  -- piaoyun 2013-6-16
// 获取技能相关资源 chongchong 2015-05-21

procedure GetEffectBase(mag, mtype:Integer; var wimg:TGameImages; var idx:Integer; NewLevel:Integer);
begin
  wimg := nil;
  idx := 0;
  if NewLevel < 0 then NewLevel := 0;
  if NewLevel > 9 then NewLevel := 9;

  case mtype of
    0:begin
        case mag of
          80..82:begin
              wimg := g_WDragonImg;
              if mag = 80 then begin
                if g_MySelf.m_nCurrX >= 84 then begin
                  idx := 130;
                end
                else begin
                  idx := 140;
                end;
              end;
              if mag = 81 then begin
                if (g_MySelf.m_nCurrX >= 78) and (g_MySelf.m_nCurrY >= 48) then begin
                  idx := 150;
                end
                else begin
                  idx := 160;
                end;
              end;
              if mag = 82 then begin
                idx := 180;
              end;
            end;
          89:begin
              wimg := g_WDragonImg;
              idx := 350;
            end;
          98: {// 富贵兽攻击效果} begin
              wimg := g_WMonImages.Images[240];
              idx := 1010;
            end;
          else begin
              wimg := g_WMagicImages;
              if mag in [0..MAXEFFECT - 1] then begin
                idx := EffectBase[NewLevel, mag].ImageOffset;
                wimg := EffectBase[NewLevel, mag].GameImages;
              end;
            end;
        end;
      end;
    1:begin
        wimg := g_WMagicImages;
        if mag in [0..MAXHITEFFECT - 1] then begin
          idx := HitEffectBase[NewLevel, mag].ImageOffset;
          wimg := HitEffectBase[NewLevel, mag].GameImages;
        end;
      end;
  end;

  {
  case mtype of
    0: begin
        case mag of
          8, 27, 33..35, 37..39, 41..42, 43, 44, 45 ..48: begin
              wimg := g_WMagic2Images;
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;
          31: begin
              wimg := g_WMonImages.Indexs[21];
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;
          36: begin
              wimg := g_WMonImages.Indexs[22];
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;
          80..82: begin
              wimg := g_WDragonImg;
              if mag = 80 then begin
                if g_MySelf.m_nCurrX >= 84 then begin
                  idx := 130;
                end else begin
                  idx := 140;
                end;
              end;
              if mag = 81 then begin
                if (g_MySelf.m_nCurrX >= 78) and (g_MySelf.m_nCurrY >= 48) then begin
                  idx := 150;
                end else begin
                  idx := 160;
                end;
              end;
              if mag = 82 then begin
                idx := 180;
              end;
            end;
          89: begin
              wimg := g_WDragonImg;
              idx := 350;
            end;
          59..64: begin
              wimg := g_WMagic4Images;
              if mag in [0..MAXEFFECT - 1] then
                idx := EffectBase[mag];
            end;
          50: begin
              wimg := g_WMagic6Images; // 流星火雨
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;

          198, 199: begin
              wimg := g_WMagic5Images; // 月灵魔法
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;
          73: begin // 分身术
              wimg := g_WMagic5Images;
              if mag in [0..MAXEFFECT - 1] then
                idx := MagicEffectBase[mag];
            end;
        else begin
            wimg := g_WMagicImages;
            if mag in [0..MAXEFFECT - 1] then
              idx := MagicEffectBase[mag];
          end;
        end;
      end;
    1: begin
        case mag of
          0..4: begin
              wimg := g_WMagicImages;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
          5..7: begin
              wimg := g_WMagic2Images;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
          8..10: begin // 合击魔法
              wimg := g_WMagic4Images;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
          11: begin
              wimg := g_WMagic5Images;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
          12, 13: begin // 4级烈火  逐日剑法
              wimg := g_WMagic6Images;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
          19..23: begin // 三绝杀 追心刺 断岳斩 横扫千军
              wimg := g_cboEffectImg;
              if mag in [0..MAXHITEFFECT - 1] then begin
                idx := MagicHitEffectBase[mag];
              end;
            end;
        end;
      end;
  end;
   }
end;

constructor TPlayEffect.Create(Images:TGameImages; nCurrX, nCurrY, nImageStart, nImageCount, nPlayCount:Integer; boBlend:Boolean; Target:TObject);
begin
  inherited Create(111, nImageStart,
    nCurrX, nCurrY,
    nCurrX, nCurrY,
    mtExplosion,
    False,
    0);
  m_nMaxCount := nPlayCount;
  m_nPlayCount := 0;
  m_boBlend := boBlend;
  MagExplosionBase := nImageStart;
  TargetActor := Target;

  ExplosionFrame := nImageCount;

  ImgLib := Images;

  NextFrameTime := 100;
end;

function TPlayEffect.Run:Boolean;
begin
  if (TargetActor <> nil) and TActor(TargetActor).m_boGhost then begin
    Result := False;
    Exit;
  end;

  m_nCurrentFrame := curframe;
  Result := Shift;

  if not Result then begin
    if m_nPlayCount < m_nMaxCount then begin
      Inc(m_nPlayCount);
      curframe := start;
      Result := Shift;
    end
    else if m_nMaxCount <= 0 then begin
      curframe := start;
      Result := Shift;
    end;
  end;

  if Result and (m_nCurrentFrame <> curframe) then begin
    PlayScene.LoadSurface(LoadSurface);
  end;

end;

function TPlayEffect.Shift:Boolean;

  function OverThrough(olddir, newdir:Integer):Boolean;
  begin
    Result := False;
    if abs(olddir - newdir) >= 2 then begin
      Result := True;
      if ((olddir = 0) and (newdir = 15)) or ((olddir = 15) and (newdir = 0)) then
        Result := False;
    end;
  end;
begin
  Result := True;
  if Repetition then begin
    if MyGetTickCount - steptime > longword(NextFrameTime) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
        curframe := start;
    end;
  end
  else begin
    if (frame > 0) and (MyGetTickCount - steptime > longword(NextFrameTime)) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then begin
        curframe := start + frame - 1;
        Result := False;
      end;
    end;
  end;

  if FixedEffect then begin
    if frame = -1 then frame := ExplosionFrame;
    if TargetActor = nil then begin
      // FlyX := targetx - ((g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX);
      // FlyY := targety - ((g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY);
      // PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      // PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);
      // rx := targetx;
      // ry := targety;
      // PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := targetx;
      FlyY := targety;
      // PlayScene.CXYfromMouseXY(targetx, targety, rx, ry);
      // PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
    end
    else begin
      rx := TActor(TargetActor).m_nRx;
      ry := TActor(TargetActor).m_nRy;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := FlyX + TActor(TargetActor).m_nShiftX;
      FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    end;
  end;
end;
{--------------------------------------------------------}

constructor THeroShowEffect.Create(effbase, effframe, nX, nY:Integer);
begin
  inherited Create(111, effbase,
    nX, nY,
    nX, nY,
    mtExplosion,
    False,
    0);
  ImgLib := g_WEffectImg;
  TargetActor := nil;
  MagExplosionBase := effbase;
  ExplosionFrame := effframe;
  NextFrameTime := 100;
end;

constructor THeroShowEffect.Create(effbase, effframe:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    mtExplosion,
    False,
    0);
  ImgLib := g_WEffectImg;
  TargetActor := target;
  MagExplosionBase := effbase;
  ExplosionFrame := effframe;
  NextFrameTime := 100;
end;

function THeroShowEffect.Shift:Boolean;

  function OverThrough(olddir, newdir:Integer):Boolean;
  begin
    Result := False;
    if abs(olddir - newdir) >= 2 then begin
      Result := True;
      if ((olddir = 0) and (newdir = 15)) or ((olddir = 15) and (newdir = 0)) then
        Result := False;
    end;
  end;
begin
  Result := True;
  if Repetition then begin
    if MyGetTickCount - steptime > longword(NextFrameTime) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
        curframe := start;
    end;
  end
  else begin
    if (frame > 0) and (MyGetTickCount - steptime > longword(NextFrameTime)) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then begin
        curframe := start + frame - 1;
        Result := False;
      end;
    end;
  end;

  if FixedEffect then begin
    if frame = -1 then frame := ExplosionFrame;
    if TargetActor = nil then begin
      // FlyX := targetx - ((g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX);
      // FlyY := targety - ((g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY);
      // PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      // PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);
      rx := targetx;
      ry := targety;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
    end
    else begin
      rx := TActor(TargetActor).m_nRx;
      ry := TActor(TargetActor).m_nRy;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := FlyX + TActor(TargetActor).m_nShiftX;
      FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    end;
  end;
end;

constructor TShowPlayEffect.Create(effbase, effframe:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    mtExplosion,
    False,
    0);
  ImgLib := g_WMain2Images;
  TargetActor := target;
  MagExplosionBase := effbase;
  ExplosionFrame := effframe;
  NextFrameTime := 60;
end;

procedure TShowPlayEffect.DrawEff;
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  nx, ny:Integer;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase + FLYBASE + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);

        // 修正护体神盾特效有偏移 chongchong 2016-12-17
        PlayScene.ScreenXYfromMCXY(TActor(TargetActor).m_nRx, TActor(TargetActor).m_nRy, nx, ny);
        nx := nX + TActor(TargetActor).m_nShiftX;
        ny := nY + TActor(TargetActor).m_nShiftY;

        if d <> nil then
          GameCanvas.DrawBlend(
            nx {FlyX} + px - UNITX div 2 - shx,
            ny {FlyY} + py - UNITY div 2 - shy,
            d);
      end
      else begin
        img := MagExplosionBase + curframe; // EXPLOSIONBASE;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);

        // 修正护体神盾特效有偏移 chongchong 2016-12-17
        PlayScene.ScreenXYfromMCXY(TActor(TargetActor).m_nRx, TActor(TargetActor).m_nRy, nx, ny);
        nx := nX + TActor(TargetActor).m_nShiftX;
        ny := nY + TActor(TargetActor).m_nShiftY;

        // 根据苹果引擎修改的，特殊处理某些技能特效，如冰霜雪雨等，特效排除边界
        if (MagicId = 66) and (curframe < 20) then begin
          Dec(py, 225);
          Inc(px, 25);
        end;
        if d <> nil then begin
          if m_DrawBlend then
            GameCanvas.DrawBlend(
              nx {FlyX} + px - UNITX div 2,
              ny {FlyY} + py - UNITY div 2,
              d)
          else
            GameCanvas.Draw(
              FlyX {FlyX} + px - UNITX div 2,
              FlyY {FlyY} + py - UNITY div 2,
              d)
        end;
      end;
    end;
  end;
end;

constructor TMagicEff.Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; {飞行类的魔法要TRUE} anitime:Integer);
var
  tax, tay:Integer;
begin
  NewLevel := 0;
  ImgLib := g_WMagicImages;
  MagicType := mtype;
  m_nCurrentFrame := -1;
  MagicId := 0;
  m_DrawBlend := True;
  case mtype of
    mtFly, mtBujaukGroundEffect, mtExploBujauk:begin
        start := 0;
        frame := 6;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 10;
        if id = 38 then frame := 10;
        if id = 39 then begin // 寒冰掌
          frame := 4;
          ExplosionFrame := 8;
        end; {
        else if id = 63 then begin // 噬魂沼泽
          frame := 6;
          ExplosionFrame := 50;
          ImgLib := g_WMagic4Images;
        end;  }
        if (id - 81 - 3) < 0 then begin
          bt80 := 1;
          Repetition := True;
          if id = 81 then begin
            if g_MySelf.m_nCurrX >= 84 then begin
              EffectBase := 130;
            end
            else begin
              EffectBase := 140;
            end;
            bt81 := 1;
          end;
          if id = 82 then begin
            if (g_MySelf.m_nCurrX >= 78) and (g_MySelf.m_nCurrY >= 48) then begin
              EffectBase := 150;
            end
            else begin
              EffectBase := 160;
            end;
            bt81 := 2;
          end;
          if id = 83 then begin
            EffectBase := 180;
            bt81 := 3;
          end;
          start := 0;
          frame := 10;
          MagExplosionBase := 190;
          ExplosionFrame := 10;
        end;
      end;
    mt12:begin
        start := 0;
        frame := 6;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 1;
      end;
    mt13:begin
        start := 0;
        frame := 20;
        curframe := start;
        FixedEffect := True;
        Repetition := False;
        ExplosionFrame := 20;
        ImgLib := g_WMonImages.Indexs[21];
      end;
    mtExplosion, mtThunder, mtLightingThunder:begin
        start := 0;
        frame := -1;
        ExplosionFrame := 10;
        curframe := start;
        FixedEffect := True;
        Repetition := False;
        if id = 80 then begin
          bt80 := 2;
          case Random(6) of
            0:begin
                EffectBase := 230;
              end;
            1:begin
                EffectBase := 240;
              end;
            2:begin
                EffectBase := 250;
              end;
            3:begin
                EffectBase := 230;
              end;
            4:begin
                EffectBase := 240;
              end;
            5:begin
                EffectBase := 250;
              end;
          end;
          light := 4;
          ExplosionFrame := 5;
        end;
        if id = 70 then begin
          bt80 := 3;
          case Random(3) of
            0:begin
                EffectBase := 400;
              end;
            1:begin
                EffectBase := 410;
              end;
            2:begin
                EffectBase := 420;
              end;
          end;
          light := 4;
          ExplosionFrame := 5;
        end;
        if id = 71 then begin
          bt80 := 3;
          ExplosionFrame := 20;
        end;
        if id = 72 then begin
          bt80 := 3;
          light := 3;
          ExplosionFrame := 10;
        end;
        if id = 73 then begin
          bt80 := 3;
          light := 5;
          ExplosionFrame := 20;
        end;
        if id = 74 then begin
          bt80 := 3;
          light := 4;
          ExplosionFrame := 35;
        end;
        if id = 90 then begin
          EffectBase := 350;
          MagExplosionBase := 350;
          ExplosionFrame := 30;
        end;
      end;
    mt14:begin
        start := 0;
        frame := -1;
        curframe := start;
        FixedEffect := True;
        Repetition := False;
        ImgLib := g_WMagic2Images;
      end;
    mtFlyAxe:begin
        start := 0;
        frame := 3;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 3;
      end;
    mtFlyArrow:begin
        start := 0;
        frame := 1;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 1;
      end;
    mt15:begin
        start := 0;
        frame := 6;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 2;
      end;
    mt16:begin
        start := 0;
        frame := 1;
        curframe := start;
        FixedEffect := False;
        Repetition := Recusion;
        ExplosionFrame := 1;
      end;
  end;
  n7C := 0;
  {
  case mtype of
     mtReady:
        begin
        end;
     mtFly,             ;
     mtBujaukGroundEffect,
     mtExploBujauk:
        begin
           start := 0;
           frame := 6;
           curframe := start;
           FixedEffect := False;
           Repetition := Recusion;
           ExplosionFrame := 10;
        end;
     mtExplosion,
     mtThunder,
     mtLightingThunder:
        begin
           start := 0;
           frame := -1;
           ExplosionFrame := 10;
           curframe := start;
           FixedEffect := TRUE;
           Repetition := False;
        end;
     mtFlyAxe:
        begin
           start := 0;
           frame := 3;
           curframe := start;
           FixedEffect := False;
           Repetition := Recusion;
           ExplosionFrame := 3;
        end;
     mtFlyArrow:
        begin
           start := 0;
           frame := 1;
           curframe := start;
           FixedEffect := False;
           Repetition := Recusion;
           ExplosionFrame := 1;
        end;
  end;
  }
  ServerMagicId := id; // 辑滚狼 ID
  EffectBase := effnum; // MagicDB - Effect
  targetx := tx; // "   target x
  targety := ty; // "   target y

  if bt80 = 1 then begin
    if id = 81 then begin
      Dec(sx, 14);
      Inc(sy, 20);
    end;
    if id = 81 then begin
      Dec(sx, 70);
      Dec(sy, 10);
    end;
    if id = 83 then begin
      Dec(sx, 60);
      Dec(sy, 70);
    end;
    g_PlaySound.PlaySound(8208);
  end;
  fireX := sx; //
  fireY := sy; //
  FlyX := sx; //
  FlyY := sy;
  OldFlyX := sx;
  OldFlyY := sy;
  FlyXf := sx;
  FlyYf := sy;
  FireMyselfX := g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX;
  FireMyselfY := g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY;
  if bt80 = 0 then begin
    MagExplosionBase := EffectBase + EXPLOSIONBASE;
  end;

  light := 1;

  if fireX <> targetx then
    tax := abs(targetx - fireX)
  else
    tax := 1;
  if fireY <> targety then
    tay := abs(targety - fireY)
  else
    tay := 1;
  if abs(fireX - targetx) > abs(fireY - targety) then begin
    firedisX := Round((targetx - fireX) * (500 / tax));
    firedisY := Round((targety - fireY) * (500 / tax));
  end
  else begin
    firedisX := Round((targetx - fireX) * (500 / tay));
    firedisY := Round((targety - fireY) * (500 / tay));
  end;

  NextFrameTime := 50;
  m_dwFrameTime := MyGetTickCount;
  m_dwStartTime := MyGetTickCount;
  m_dwRunTime := MyGetTickCount;
  steptime := MyGetTickCount;
  repeattime := anitime;
  Dir16 := GetFlyDirection16(sx, sy, tx, ty);
  OldDir16 := Dir16;
  NextEffect := nil;
  m_boActive := True;
  prevdisx := 99999;
  prevdisy := 99999;

  m_LastRunTick := MyGetTickCount;
end;

destructor TMagicEff.Destroy;
begin
  // DebugOutStr('TMagicEff.Destroy');
  inherited Destroy;
end;

function TMagicEff.Shift:Boolean;

  function OverThrough(olddir, newdir:Integer):Boolean;
  begin
    Result := False;
    if abs(olddir - newdir) >= 2 then begin
      Result := True;
      if ((olddir = 0) and (newdir = 15)) or ((olddir = 15) and (newdir = 0)) then
        Result := False;
    end;
  end;
var
  I, rrx, rry, ms, stepx, stepy, newstepx, newstepy, nn:Integer;
  tax, tay, shx, shy, passdir16:Integer;
  crash:Boolean;
  stepxf, stepyf:Real;

  boValue:Boolean;
begin
  Result := True;
  if Repetition then begin
    if MyGetTickCount - steptime > longword(NextFrameTime) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
        curframe := start;
    end;
  end
  else begin
    if (frame > 0) and (MyGetTickCount - steptime > longword(NextFrameTime)) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then begin
        curframe := start + frame - 1;
        Result := False;
      end;
    end;
  end;

  if (not FixedEffect) then begin
    crash := False;
    if TargetActor <> nil then begin
      ms := MyGetTickCount - m_dwFrameTime; // 捞傈 瓤苞甫 弊赴饶 倔付唱 矫埃捞 汝范绰瘤?
      m_dwFrameTime := MyGetTickCount;
      // TargetX, TargetY 犁汲沥
      PlayScene.ScreenXYfromMCXY(TActor(TargetActor).m_nRx,
        TActor(TargetActor).m_nRy,
        targetx,
        targety);
      shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
      shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
      targetx := targetx + shx;
      targety := targety + shy;

      // 货肺款 鸥百阑 谅钎甫 货肺 汲沥茄促.
      if FlyX <> targetx then
        tax := abs(targetx - FlyX)
      else
        tax := 1;
      if FlyY <> targety then
        tay := abs(targety - FlyY)
      else
        tay := 1;
      if abs(FlyX - targetx) > abs(FlyY - targety) then begin
        newfiredisX := Round((targetx - FlyX) * (500 / tax));
        newfiredisY := Round((targety - FlyY) * (500 / tax));
      end
      else begin
        newfiredisX := Round((targetx - FlyX) * (500 / tay));
        newfiredisY := Round((targety - FlyY) * (500 / tay));
      end;

      if firedisX < newfiredisX then firedisX := firedisX + _MAX(1, (newfiredisX - firedisX) div 10);
      if firedisX > newfiredisX then firedisX := firedisX - _MAX(1, (firedisX - newfiredisX) div 10);
      if firedisY < newfiredisY then firedisY := firedisY + _MAX(1, (newfiredisY - firedisY) div 10);
      if firedisY > newfiredisY then firedisY := firedisY - _MAX(1, (firedisY - newfiredisY) div 10);

      stepxf := (firedisX / 700) * ms;
      stepyf := (firedisY / 700) * ms;
      FlyXf := FlyXf + stepxf;
      FlyYf := FlyYf + stepyf;
      FlyX := Round(FlyXf);
      FlyY := Round(FlyYf);

      // Dir16 := GetFlyDirection16 (OldFlyX, OldFlyY, FlyX, FlyY);
      OldFlyX := FlyX;
      OldFlyY := FlyY;

      passdir16 := GetFlyDirection16(FlyX, FlyY, targetx, targety);
      {
     DebugOutStr(IntToStr(prevdisx) + ' ' + IntToStr(prevdisy) + ' / ' + IntToStr(abs(targetx - FlyX)) + ' ' + IntToStr(abs(targety - FlyY)) + '   ' +
       IntToStr(firedisX) + '.' + IntToStr(firedisY) + ' ' +
       IntToStr(FlyX) + '.' + IntToStr(FlyY) + ' ' +
       IntToStr(targetx) + '.' + IntToStr(targety));
       }

      if ((abs(targetx - FlyX) <= 15) and (abs(targety - FlyY) <= 15)) or
        ((abs(targetx - FlyX) >= prevdisx) and (abs(targety - FlyY) >= prevdisy)) or
        OverThrough(OldDir16, passdir16) then begin
        crash := True;
      end
      else begin
        prevdisx := abs(targetx - FlyX);
        prevdisy := abs(targety - FlyY);
        // if (prevdisx <= 5) and (prevdisy <= 5) then crash := TRUE;
      end;
      OldDir16 := passdir16;

    end
    else begin
      ms := MyGetTickCount - m_dwFrameTime; // 瓤苞狼 矫累饶 倔付唱 矫埃捞 汝范绰瘤?

      rrx := targetx - fireX;
      rry := targety - fireY;

      stepx := Round((firedisX / 900) * ms);
      stepy := Round((firedisY / 900) * ms);
      FlyX := fireX + stepx;
      FlyY := fireY + stepy;
    end;

    PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);

    if crash and (TargetActor <> nil) then begin
      FixedEffect := True; // 气惯
      start := 0;
      frame := ExplosionFrame;
      curframe := start;
      Repetition := False;

      // 修复用裂神符打弓箭手时（两个弓箭手站一起，使技能分裂），弓箭手还击声音错误 chongchong 2015-06-16
      if (MagOwner <> nil) and (Self is TExploBujaukEffect4) then
        g_PlaySound.PlaySound(TActor(MagOwner).m_nMagicExplosionSound);
    end;
    // if not Map.CanFly (Rx, Ry) then
    // Result := False;
  end;
  if FixedEffect then begin
    boValue := False;
    if frame = -1 then begin
      frame := ExplosionFrame;
      boValue := True;
    end;
    if TargetActor = nil then begin
      FlyX := targetx - ((g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX);
      FlyY := targety - ((g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY);

      // 修正黑夜 - 冰咆哮对目标，目标跑动时卡 ??????????????????? chongchong 2015-03-04
      if boValue then begin
        PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);
      end;
    end
    else begin
      rx := TActor(TargetActor).m_nRx;
      ry := TActor(TargetActor).m_nRy;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := FlyX + TActor(TargetActor).m_nShiftX;
      FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    end;
  end;
end;

procedure TMagicEff.GetFlyXY(ms:Integer; var fx, fy:Integer);
var
  stepx, stepy:Integer;
begin
  stepx := Round((firedisX / 900) * ms);
  stepy := Round((firedisY / 900) * ms);
  fx := fireX + stepx;
  fy := fireY + stepy;
end;

function TMagicEff.Run:Boolean;
var
  boFixedEffect:Boolean;
  nOFlyX, nOFlyY:Integer;
begin
  boFixedEffect := FixedEffect;
  nOFlyX := FlyX;
  nOFlyY := FlyY;
  Result := Shift;
  if Result then begin
    if MyGetTickCount - m_dwStartTime > 10000 then // 2000 then
      Result := False
    else begin
      if (boFixedEffect <> FixedEffect) or (m_nCurrentFrame <> curframe) or (nOFlyX <> FlyX) or (nOFlyY <> FlyY) then
        PlayScene.LoadSurface(LoadSurface);

      if m_boActive and ((boFixedEffect <> FixedEffect) or (m_nCurrentFrame <> curframe) or (nOFlyX <> FlyX) or (nOFlyY <> FlyY)) then begin
        if (MagicType in [mtFly, mtFlyAxe, mtFlyArrow, mtExploBujauk, mtBujaukGroundEffect]) or (Self is TScrollHideEffect) or
          (Self is TShowPlayEffect) or (Self is THeroShowEffect) then

      end;

      Result := True;
    end;
  end;
  // 移动这里不知道对不对 chongchong 2015-04-11
  m_nCurrentFrame := curframe;
end;

procedure TMagicEff.LoadSurface(Sender:TObject);
begin
  {var
    img: Integer;
    d: TTexture;
    shx, shy: Integer;
  begin
    if m_boActive and ((abs(FlyX - fireX) > 15) or (abs(FlyY - fireY) > 15) or FixedEffect) then
    begin

      shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
      shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

      if ImgLib <> nil then
      begin
        if not FixedEffect then
        begin
          img := EffectBase + FLYBASE + Dir16 * 10;

          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
          else
            d := ImgLib.GetCachedImage(img + curframe, px, py);

        end
        else
        begin
          img := MagExplosionBase + curframe;                                                         // EXPLOSIONBASE;
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := ImgLib.GetCachedGrayImage(img, px, py)
          else
            d := ImgLib.GetCachedImage(img, px, py);
        end;
      end;
    end;
  end;
  }

end;

procedure TMagicEff.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase + FLYBASE + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);

        if d <> nil then
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d);
      end
      else begin
        img := MagExplosionBase + curframe; // EXPLOSIONBASE;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);

        // 根据苹果引擎修改的，特殊处理某些技能特效，如冰霜雪雨等，特效排除边界
        if (MagicId = 66) and (curframe < 20) then begin
          Dec(py, 225);
          Inc(px, 25);
        end;
        if d <> nil then begin
          if m_DrawBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2,
              FlyY + py - UNITY div 2,
              d)
          else
            GameCanvas.Draw(
              FlyX + px - UNITX div 2,
              FlyY + py - UNITY div 2,
              d)
        end;
      end;
    end;
  end;
end;
{
procedure TCopySelf.LoadSurface(Sender: TObject);
var
  btDir, img: Integer;
  d: TTexture;
begin

  if m_boActive and ((abs(FlyX - fireX) > 15) or (abs(FlyY - fireY) > 15) or FixedEffect) then
  begin
    if ImgLib <> nil then
    begin
      if not FixedEffect then
      begin
        case Dir16 of
          0, 1: btDir := 0;
          2, 3: btDir := 1;
          4, 5: btDir := 2;
          6, 7: btDir := 3;
          8, 9: btDir := 4;
          10, 11: btDir := 5;
          12, 13: btDir := 6;
          14, 15: btDir := 7;
        end;
        img := EffectBase + FLYBASE + btDir * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
      end
      else
      begin
        img := MagExplosionBase + curframe;                                                         // EXPLOSIONBASE;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
      end;
    end;
  end;
end;
}

procedure TCopySelf.DrawEff();
var
  btDir, img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then begin
      if not FixedEffect then begin
        case Dir16 of
          0, 1:btDir := 0;
          2, 3:btDir := 1;
          4, 5:btDir := 2;
          6, 7:btDir := 3;
          8, 9:btDir := 4;
          10, 11:btDir := 5;
          12, 13:btDir := 6;
          14, 15:btDir := 7;
          else btDir := 0; //HZQ 20230525 消除警告，永远不会到这个流程
        end;
        img := EffectBase + FLYBASE + btDir * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);

        if d <> nil then
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d);
      end else begin
        img := MagExplosionBase + curframe; // EXPLOSIONBASE;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);

        if d <> nil then
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2,
            FlyY + py - UNITY div 2,
            d);
      end;
    end;
  end;
end;

{------------------------------------------------------------}

// TFlyingAxe : 朝酒啊绰 档尝

{------------------------------------------------------------}

constructor TFlyingAxe.Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer);
begin
  inherited Create(id, effnum, sx, sy, tx, ty, mtype, Recusion, anitime);
  FlyImageBase := FLYOMAAXEBASE;
  ReadyFrame := 65;
end;

{
procedure TFlyingAxe.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin

  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then
  begin
    if not FixedEffect then
    begin
      img := FlyImageBase + Dir16 * 10;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);
    end
    else
    begin

    end;
  end;
end;
}

function TFlyingAxe.Run:Boolean;
begin
  // 修正飞行魔法不显示 chong 2018-12-22 20:19:57
  // 掷斧骷髅（抛斧）  RACE=87   RACEIMG=15
  // 半兽统领（抛斧）  RACE=104  RACEIMG=15
  // 暗黑战士（飞刺）  RACE=93   RACEIMG=22
  // 暴牙蜘蛛（飞刺）  RACE=93   RACEIMG=22

  Result := True;
  if MyGetTickCount - m_dwRunTime >= 50 then begin
    m_dwRunTime := MyGetTickCount;
    Result := inherited Run;
  end;
end;

procedure TFlyingAxe.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      //
      img := FlyImageBase + Dir16 * 10;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);
      if d <> nil then begin
        // 舅颇喉珐爹窍瘤 臼澜
        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d.ClientRect, d);
      end;
    end
    else begin

    end;
  end;
end;

function TFlyingArrow.Run:Boolean;
begin
  Result := True;
  if MyGetTickCount - m_dwRunTime > 100 then begin
    // 不能要这个，不然弓骷髅弓箭手射的箭看不到 chongchong 2018-12-22 20:19:47
    //m_dwRunTime := MyGetTickCount;
    Result := inherited Run;
  end;
end;

procedure TFlyingArrow.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > 40) or (abs(FlyY - fireY) > 40) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      img := FlyImageBase + Dir16; // * 10;   + curframe    + curframe + curframe
      // img := 272 + Dir16;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);

      if d <> nil then begin

        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy - 46,
          d.ClientRect, d);

        // DScreen.AddChatBoardString(Format('X:%d Y:%d ', [FlyX + px - UNITX div 2 - shx, FlyY + py - UNITY div 2 - shy - 46]), 0, $FFFFFF);
      end;
    end;
  end;
end;

{ TFlyingArrowEx }

function TFlyingArrowEx.Run:Boolean;
begin
  Result := True;
  if MyGetTickCount - m_dwRunTime > 100 then begin
    m_dwRunTime := MyGetTickCount;
    Result := inherited Run;
  end;
end;

procedure TFlyingArrowEx.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > 40) or (abs(FlyY - fireY) > 40) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      img := FlyImageBase;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);

      if d <> nil then begin

        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy - 8,
          d.ClientRect, d);

        // DScreen.AddChatBoardString(Format('X:%d Y:%d ', [FlyX + px - UNITX div 2 - shx, FlyY + py - UNITY div 2 - shy - 46]), 0, $FFFFFF);
      end;
    end;
  end;
end;

{--------------------------------------------------------}

constructor TCharEffect.Create(effbase, effframe:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    mtExplosion,
    False,
    0);
  TargetActor := target;
  frame := effframe;
  NextFrameTime := 30;

end;

function TCharEffect.Run:Boolean;
begin
  Result := True;
  m_nCurrentFrame := curframe;
  if MyGetTickCount - steptime > LongWord(NextFrameTime) then begin
    steptime := MyGetTickCount;
    Inc(curframe);
    if curframe > start + frame - 1 then begin
      curframe := start + frame - 1;
      Result := False;
    end;
  end;
  if Result then begin
    if (m_nCurrentFrame <> curframe) then begin
      PlayScene.LoadSurface(LoadSurface);

    end;
  end;
end;

{
procedure TCharEffect.LoadSurface(Sender: TObject);
var
  d: TTexture;
begin

  if TargetActor <> nil then
  begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(EffectBase + curframe, px, py);
  end;
end;
}

procedure TCharEffect.DrawEff();
var
  d:TTexture;
begin
  if TargetActor <> nil then begin
    rx := TActor(TargetActor).m_nRx;
    ry := TActor(TargetActor).m_nRy;
    PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
    FlyX := FlyX + TActor(TargetActor).m_nShiftX;
    FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(EffectBase + curframe, px, py);
    if d <> nil then begin
      GameCanvas.DrawBlend(
        FlyX + px - UNITX div 2,
        FlyY + py - UNITY div 2,
        d);
    end;
  end;
end;

{--------------------------------------------------------}

constructor TMapEffect.Create(effbase, effframe:Integer; X, Y:Integer);
begin
  inherited Create(111, effbase,
    X, Y,
    X, Y,
    mtExplosion,
    False,
    0);
  TargetActor := nil;
  frame := effframe;
  NextFrameTime := 30;
  RepeatCount := 0;
end;

function TMapEffect.Run:Boolean;
begin
  Result := True;
  m_nCurrentFrame := curframe;
  if MyGetTickCount - steptime > longword(NextFrameTime) then begin
    steptime := MyGetTickCount;
    Inc(curframe);
    if curframe > start + frame - 1 then begin
      curframe := start + frame - 1;
      if RepeatCount > 0 then begin
        Dec(RepeatCount);
        curframe := start;
      end
      else
        Result := False;
    end;
  end;
  if Result and (m_nCurrentFrame <> curframe) then
    PlayScene.LoadSurface(LoadSurface);
end;
{
procedure TMapEffect.LoadSurface(Sender: TObject);
var
  d: TTexture;
begin

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(EffectBase + curframe, px, py);
end;
}

procedure TMapEffect.DrawEff();
var
  d:TTexture;
begin
  rx := targetx;
  ry := targety;
  PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(EffectBase + curframe, px, py);
  if d <> nil then begin
    GameCanvas.DrawBlend(
      FlyX + px - UNITX div 2,
      FlyY + py - UNITY div 2,
      d);
  end;
end;

{--------------------------------------------------------}

constructor TScrollHideEffect.Create(effbase, effframe:Integer; X, Y:Integer; target:TObject);
begin
  inherited Create(effbase, effframe, X, Y);
  // TargetCret := TActor(target);//在出现有人用随机之类时，将设置目标
end;

function TScrollHideEffect.Run:Boolean;
var
  FocusCret:TActor;
begin
  Result := inherited Run;
  if frame = 7 then begin
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    FocusCret := g_FocusCret;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
    if (FocusCret <> nil) and PlayScene.IsValidActor(FocusCret) then begin
      FocusCret.m_dwDeleteTime := MyGetTickCount;
      FocusCret.m_boDelActor := True;
      FocusCret.m_boFreeActor := False;
      FocusCret.CleanMsgs;
    end;
  end;
end;

{--------------------------------------------------------}

constructor TLightingEffect.Create(effbase, effframe:Integer; X, Y:Integer);
begin

end;

function TLightingEffect.Run:Boolean;
begin
  Result := False; // Jacky
end;

{--------------------------------------------------------}

constructor TFireGunEffect.Create(effbase, sx, sy, tx, ty:Integer);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty, // TActor(target).XX, TActor(target).m_nCurrY,
    mtFireGun,
    True,
    0);
  NextFrameTime := 50;
  FillChar(FireNodes, SizeOf(TFireNode) * FIREGUNFRAME, #0);
  OutofOil := False;
  firetime := MyGetTickCount;
end;

function TFireGunEffect.Run:Boolean;
var
  I:Integer;
  allgone:Boolean;
  boLoadSurface:Boolean;
begin
  Result := True;
  boLoadSurface := False;

  if MyGetTickCount - steptime > longword(NextFrameTime) then begin
    Shift;
    boLoadSurface := True;
    steptime := MyGetTickCount;
    // if not FixedEffect then begin
    if not OutofOil then begin
      if (MagOwner <> nil) and ((abs(rx - TActor(MagOwner).m_nRx) >= 5) or (abs(ry - TActor(MagOwner).m_nRy) >= 5) or (MyGetTickCount - firetime > 800)) then
        OutofOil := True;
      for I := FIREGUNFRAME - 2 downto 0 do begin
        FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
        FireNodes[I + 1] := FireNodes[I];
      end;
      FireNodes[0].firenumber := 1;
      FireNodes[0].X := FlyX;
      FireNodes[0].Y := FlyY;
    end
    else begin
      allgone := True;
      for I := FIREGUNFRAME - 2 downto 0 do begin
        if FireNodes[I].firenumber <= FIREGUNFRAME then begin
          FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
          FireNodes[I + 1] := FireNodes[I];
          allgone := False;
          boLoadSurface := False;
        end;
      end;
      if allgone then Result := False;
    end;
  end;

  if boLoadSurface then
    PlayScene.LoadSurface(LoadSurface);
end;
{
procedure TFireGunEffect.LoadSurface(Sender: TObject);
var
  I, img: Integer;
  d: TTexture;
begin
  for I := 0 to FIREGUNFRAME - 1 do
  begin
    if (FireNodes[I].firenumber <= FIREGUNFRAME) and (FireNodes[I].firenumber > 0) then
    begin
      img := EffectBase + (FireNodes[I].firenumber - 1);
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);
    end;
  end;
end;
}

procedure TFireGunEffect.DrawEff();
var
  I, shx, shy, fireX, fireY, prx, pry, img:Integer;
  d:TTexture;
begin
  prx := -1;
  pry := -1;
  for I := 0 to FIREGUNFRAME - 1 do begin
    if (FireNodes[I].firenumber <= FIREGUNFRAME) and (FireNodes[I].firenumber > 0) then begin
      // 修正地狱火模式 cboEffect, 开始: 2770; 数量25飞行效果在左上角播放，原因是没有初始化的坐标也播放了  2019-04-30 10:52:28
      if (FireNodes[I].firenumber < I + 1) then Continue;

      shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
      shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

      img := EffectBase + (FireNodes[I].firenumber - 1);
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);

      if d <> nil then begin
        fireX := FireNodes[I].X + px - UNITX div 2 - shx;
        fireY := FireNodes[I].Y + py - UNITY div 2 - shy;
        if (fireX <> prx) or (fireY <> pry) then begin
          prx := fireX;
          pry := fireY;
          GameCanvas.DrawBlend(fireX, fireY, d);
        end;
      end;
    end;
  end;
end;

{--------------------------------------------------------}

constructor TThuderEffect.Create(effbase, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    tx, ty,
    tx, ty, // TActor(target).XX, TActor(target).m_nCurrY,
    mtThunder,
    False,
    0);
  TargetActor := target;
end;
{
procedure TThuderEffect.LoadSurface(Sender: TObject);
var
  img, px, py: Integer;
  d: TTexture;
begin
  img := EffectBase;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(img + curframe, px, py);
end;
}

procedure TThuderEffect.DrawEff();
var
  img, px, py:Integer;
  d:TTexture;
begin
  img := EffectBase;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(img + curframe, px, py);
  if d <> nil then begin
    GameCanvas.DrawBlend(
      FlyX + px - UNITX div 2,
      FlyY + py - UNITY div 2,
      d);
  end;
end;

{--------------------------------------------------------}

constructor TLightingThunder.Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty, // TActor(target).XX, TActor(target).m_nCurrY,
    mtLightingThunder,
    False,
    0);
  TargetActor := target;
end;
{
procedure TLightingThunder.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin
  img := EffectBase + Dir16 * 10;
  if curframe < 6 then
  begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(img + curframe, px, py);
  end;
end;
}

procedure TLightingThunder.DrawEff();
var
  img, sx, sy, px, py:Integer;
  d:TTexture;
begin
  img := EffectBase + Dir16 * 10;
  if curframe < 6 then begin

    //shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    //shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(img + curframe, px, py);
    if d <> nil then begin
      if MagOwner <> nil then
        PlayScene.ScreenXYfromMCXY(TActor(MagOwner).m_nRx,
          TActor(MagOwner).m_nRy,
          sx,
          sy);
      // 修正在跑动或走动的时候释放疾光电影特效会错位chongchong 2016-12-27
      // + TActor(MagOwner).m_nShiftX, +TActor(MagOwner).m_nShiftY
      GameCanvas.DrawBlend(
        sx + TActor(MagOwner).m_nShiftX + px - UNITX div 2,
        sy + TActor(MagOwner).m_nShiftY + py - UNITY div 2,
        d);
    end;
  end;
end;

{--------------------------------------------------------}

constructor TExploBujaukEffect.Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 3;
  TargetActor := target;
  NextFrameTime := 50;
  MagicBlend := False;
end;

function TExploBujaukEffect.Run:Boolean;
begin
  Result := inherited Run;
end;

function TExploBujaukEffect.Shift:Boolean;

  function OverThrough(olddir, newdir:Integer):Boolean;
  begin
    Result := False;
    if abs(olddir - newdir) >= 2 then begin
      Result := True;
      if ((olddir = 0) and (newdir = 15)) or ((olddir = 15) and (newdir = 0)) then
        Result := False;
    end;
  end;
var
  {I,} rrx, rry, ms, stepx, stepy{, newstepx, newstepy, nn}:Integer;
  tax, tay, shx, shy, passdir16:Integer;
  crash:Boolean;
  stepxf, stepyf:Real;

  boValue:Boolean;
begin
  Result := True;
  if Repetition then begin
    if MyGetTickCount - steptime > longword(NextFrameTime) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
        curframe := start;
    end;
  end
  else begin
    if (frame > 0) and (MyGetTickCount - steptime > longword(NextFrameTime)) then begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then begin
        curframe := start + frame - 1;
        Result := False;
      end;
    end;
  end;

  if (not FixedEffect) then begin
    crash := False;
    if TargetActor <> nil then begin
      ms := MyGetTickCount - m_dwFrameTime; // 捞傈 瓤苞甫 弊赴饶 倔付唱 矫埃捞 汝范绰瘤?
      m_dwFrameTime := MyGetTickCount;
      // TargetX, TargetY 犁汲沥
      PlayScene.ScreenXYfromMCXY(TActor(TargetActor).m_nRx,
        TActor(TargetActor).m_nRy,
        targetx,
        targety);
      shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
      shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
      targetx := targetx + shx;
      targety := targety + shy;

      // 货肺款 鸥百阑 谅钎甫 货肺 汲沥茄促.
      if FlyX <> targetx then
        tax := abs(targetx - FlyX)
      else
        tax := 1;
      if FlyY <> targety then
        tay := abs(targety - FlyY)
      else
        tay := 1;
      if abs(FlyX - targetx) > abs(FlyY - targety) then begin
        newfiredisX := Round((targetx - FlyX) * (500 / tax));
        newfiredisY := Round((targety - FlyY) * (500 / tax));
      end else begin
        newfiredisX := Round((targetx - FlyX) * (500 / tay));
        newfiredisY := Round((targety - FlyY) * (500 / tay));
      end;

      if firedisX < newfiredisX then firedisX := firedisX + _MAX(1, (newfiredisX - firedisX) div 10);
      if firedisX > newfiredisX then firedisX := firedisX - _MAX(1, (firedisX - newfiredisX) div 10);
      if firedisY < newfiredisY then firedisY := firedisY + _MAX(1, (newfiredisY - firedisY) div 10);
      if firedisY > newfiredisY then firedisY := firedisY - _MAX(1, (firedisY - newfiredisY) div 10);

      stepxf := (firedisX / 700) * ms;
      stepyf := (firedisY / 700) * ms;
      FlyXf := FlyXf + stepxf;
      FlyYf := FlyYf + stepyf;
      FlyX := Round(FlyXf);
      FlyY := Round(FlyYf);

      // Dir16 := GetFlyDirection16 (OldFlyX, OldFlyY, FlyX, FlyY);
      OldFlyX := FlyX;
      OldFlyY := FlyY;

      passdir16 := GetFlyDirection16(FlyX, FlyY, targetx, targety);
      {
     DebugOutStr(IntToStr(prevdisx) + ' ' + IntToStr(prevdisy) + ' / ' + IntToStr(abs(targetx - FlyX)) + ' ' + IntToStr(abs(targety - FlyY)) + '   ' +
       IntToStr(firedisX) + '.' + IntToStr(firedisY) + ' ' +
       IntToStr(FlyX) + '.' + IntToStr(FlyY) + ' ' +
       IntToStr(targetx) + '.' + IntToStr(targety));
       }

      if ((abs(targetx - FlyX) <= 60) and (abs(targety - FlyY) <= 60)) or
        ((abs(targetx - FlyX) >= prevdisx) and (abs(targety - FlyY) >= prevdisy)) or
        OverThrough(OldDir16, passdir16) then begin
        crash := True;
      end else begin
        prevdisx := abs(targetx - FlyX);
        prevdisy := abs(targety - FlyY);
        // if (prevdisx <= 5) and (prevdisy <= 5) then crash := TRUE;
      end;
      OldDir16 := passdir16;

    end else begin
      ms := MyGetTickCount - m_dwFrameTime; // 瓤苞狼 矫累饶 倔付唱 矫埃捞 汝范绰瘤?

      rrx := targetx - fireX;
      rry := targety - fireY;

      stepx := Round((firedisX / 900) * ms);
      stepy := Round((firedisY / 900) * ms);
      FlyX := fireX + stepx;
      FlyY := fireY + stepy;
    end;

    PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);

    if crash and (TargetActor <> nil) then begin
      FixedEffect := True; // 气惯
      start := 0;
      frame := ExplosionFrame;
      curframe := start;
      Repetition := False;

      if MagOwner <> nil then
        g_PlaySound.PlaySound(TActor(MagOwner).m_nMagicExplosionSound);

    end;
    // if not Map.CanFly (Rx, Ry) then
    // Result := False;
  end;
  if FixedEffect then begin
    boValue := False;
    if frame = -1 then begin
      frame := ExplosionFrame;
      boValue := True;
    end;
    if TargetActor = nil then begin
      FlyX := targetx - ((g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX);
      FlyY := targety - ((g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY);

      // 修正黑夜 - 冰咆哮对目标，目标跑动时卡 ??????????????????? chongchong 2015-03-04
      if boValue then begin
        PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);
      end;
    end
    else begin
      rx := TActor(TargetActor).m_nRx;
      ry := TActor(TargetActor).m_nRy;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := FlyX + TActor(TargetActor).m_nShiftX;
      FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    end;
  end;
end;

{
procedure TExploBujaukEffect.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin
  if m_boActive and ((abs(FlyX - fireX) > 30) or (abs(FlyY - fireY) > 30) or FixedEffect) then
  begin
    if not FixedEffect then
    begin
      img := EffectBase + Dir16 * 10;

      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);

      if NewLevel > 0 then
      begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + 170 + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + 170 + curframe, px, py);
      end;
    end
    else
    begin
      img := MagExplosionBase + curframe;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);
    end;
  end;
end;
}

procedure TExploBujaukEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  // meff: TMapEffect;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          // 混合效果
          if MagicBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d)
          else
            GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d.ClientRect, d);
        end;
        if NewLevel > 0 then begin
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := ImgLib.GetCachedGrayImage(img + 170 + curframe, px, py)
          else
            d := ImgLib.GetCachedImage(img + 170 + curframe, px, py);
          if d <> nil then
            GameCanvas.DrawBlend(FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy, d);
        end;
      end
      else begin
        img := MagExplosionBase + curframe;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2,
            FlyY + py - UNITY div 2,
            d);
        end;
      end;
    end;
  end;
end;

{--------------------------------------------------------}
// 4级技能强化 -- 4级灵魂火符 （符飞行） chongchong 2013-12-04

constructor TExploBujaukEffect4.Create(sx, sy, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, 140 {符在资源中的起始位置},
    sx, sy,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 3;
  TargetActor := target;
  NextFrameTime := 50;
  MagicBlend := False;

  ImgLib := g_WMagic6Images;
  MagExplosionBase := 300;
end;

function TExploBujaukEffect4.Run:Boolean;
begin
  Result := inherited Run;
end;

procedure TExploBujaukEffect4.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  // meff: TMapEffect;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          // 混合效果
          if MagicBlend then
            GameCanvas.DrawBlend(FlyX + px - UNITX div 2 - shx, FlyY + py - UNITY div 2 - shy, d)
          else
            GameCanvas.Draw(FlyX + px - UNITX div 2 - shx, FlyY + py - UNITY div 2 - shy, d.ClientRect, d);
        end;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + 170 + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + 170 + curframe, px, py);

        if d <> nil then
          GameCanvas.DrawBlend(FlyX + px - UNITX div 2 - shx, FlyY + py - UNITY div 2 - shy, d);
      end
      else begin
        img := MagExplosionBase + curframe;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(FlyX + px - UNITX div 2, FlyY + py - UNITY div 2, d);
        end;
      end;
    end;
  end;
end;
{--------------------------------------------------------}

constructor TMoonMonEffect.Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 6;
  TargetActor := target;
  NextFrameTime := 50;
  MagicBlend := True;
end;

procedure TMoonMonEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          // 混合效果
          if MagicBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d)
          else
            GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d.ClientRect, d);
        end;
      end
      else begin
        img := MagExplosionBase + curframe;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2,
            FlyY + py - UNITY div 2,
            d);
        end;
      end;
    end;
  end;
end;

{--------------------------------------------------------}

constructor TBujaukGroundEffect.Create(effbase, magicnumb, sx, sy, tx, ty:Integer);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty,
    mtBujaukGroundEffect,
    True,
    0);
  frame := 3;
  MagicNumber := magicnumb;
  BoGroundEffect := False;
  NextFrameTime := 50;
end;

function TBujaukGroundEffect.Run:Boolean;
begin
  Result := True;
  if (MyGetTickCount - m_dwRunTime > 30) or FixedEffect then begin
    m_dwRunTime := MyGetTickCount;
    Result := inherited Run;
    if not FixedEffect then begin
      if ((abs(targetx - FlyX) <= 15) and (abs(targety - FlyY) <= 15)) or
        ((abs(targetx - FlyX) >= prevdisx) and (abs(targety - FlyY) >= prevdisy)) then begin
        FixedEffect := True;
        start := 0;
        frame := ExplosionFrame;
        curframe := start;
        Repetition := False;
        if MagOwner <> nil then
          g_PlaySound.PlaySound(TActor(MagOwner).m_nMagicExplosionSound);
        PlayScene.LoadSurface(LoadSurface);
        Result := True;
      end
      else begin
        prevdisx := abs(targetx - FlyX);
        prevdisy := abs(targety - FlyY);
      end;
    end;
  end;
end;

procedure TBujaukGroundEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  // meff: TMapEffect;
begin
  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      img := EffectBase + Dir16 * 10;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);
      if d <> nil then begin

        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d.ClientRect, d);
      end;
    end
    else begin
      if (NewLevel = 0) or (MagicNumber = 46 {诅咒术}) or (MagicNumber = 67 {新诅咒术}) then begin
        if MagicNumber = 11 then
          img := EffectBase + 16 * 10 + curframe
        else
          img := EffectBase + 18 * 10 + curframe;

        if MagicNumber = 46 then begin
          GetEffectBase(MagicNumber - 1, 0, ImgLib, img);
          // 诅咒术目标物效修改 chongchong 2015-05-23
          //img := img + 10 + curframe;
          img := img + 170 + curframe;
        end
          // 新诅咒术 chongchong 2015-07-25
        else if MagicNumber = 67 then begin
          GetEffectBase(MagicNumber - 1, 0, ImgLib, img);
          img := img + 10 + curframe;
        end;
      end
      else begin
        if NewLevel in [1..3] then begin
          case MagicNumber of
            11:begin
                img := 2470 + curframe; // 幽灵盾
              end;
            12:begin
                img := 2410 + curframe; // 神圣战甲术
              end;
          end;

        end
        else if NewLevel in [4..6] then begin
          case MagicNumber of
            11:begin
                img := 2490 + curframe; // 幽灵盾
              end;
            12:begin
                img := 2430 + curframe; // 神圣战甲术
              end;
          end;
        end
        else begin
          case MagicNumber of
            11:begin
                img := 2520 + curframe; // 幽灵盾
              end;
            12:begin
                img := 2450 + curframe; // 神圣战甲术
              end;
          end;
        end;
      end;

      if (NewLevel = 0) or (MagicNumber = 46) then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
      end
      else begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagic7Images16.GetCachedGrayImage(img, px, py)
        else
          d := g_WMagic7Images16.GetCachedImage(img, px, py);
      end;

      if d <> nil then begin
        GameCanvas.DrawBlend(
          FlyX + px - UNITX div 2, // - shx,
          FlyY + py - UNITY div 2, // - shy,
          d);
      end;
    end;
  end;
end;

{ TNormalDrawEffect }

constructor TNormalDrawEffect.Create(xx, yy:Integer; WMImage:TGameImages; effbase, nX:Integer; frmTime:longword; boFlag:Boolean);
begin
  inherited Create(111, effbase, xx, yy, xx, yy, mtReady, True, 0);
  ImgLib := WMImage;
  EffectBase := effbase;
  start := 0;
  curframe := 0;
  frame := nX;
  NextFrameTime := frmTime;
  boC8 := boFlag;
end;
{
procedure TNormalDrawEffect.LoadSurface(Sender: TObject);
var
  d: TTexture;
  nPx, nPy: Integer;
begin
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(EffectBase + curframe, nPx, nPy);
end;
}

procedure TNormalDrawEffect.DrawEff();
var
  d:TTexture;
  nRx, nRy, nPx, nPy:Integer;
begin
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(EffectBase + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(EffectBase + curframe, nPx, nPy);
  if d <> nil then begin
    PlayScene.ScreenXYfromMCXY(FlyX, FlyY, nRx, nRy);
    if boC8 then begin
      GameCanvas.DrawBlend(nRx + nPx - UNITX div 2, nRy + nPy - UNITY div 2, d);
    end
    else begin
      GameCanvas.Draw(nRx + nPx - UNITX div 2, nRy + nPy - UNITY div 2, d.ClientRect, d);
    end;
  end;
end;

function TNormalDrawEffect.Run:Boolean;
begin
  Result := True;
  m_nCurrentFrame := curframe;
  if m_boActive and (MyGetTickCount - steptime > longword(NextFrameTime)) then begin
    steptime := MyGetTickCount;
    Inc(curframe);
    if curframe > start + frame - 1 then begin
      curframe := start;
      Result := False;
    end;
  end;

  if Result and (m_nCurrentFrame <> curframe) then
    PlayScene.LoadSurface(LoadSurface);
end;

{ TFlyingBug }

constructor TFlyingBug.Create(id, effnum, sx, sy, tx, ty:Integer;
  mtype:TMagicType; Recusion:Boolean; anitime:Integer);
begin
  inherited Create(id, effnum, sx, sy, tx, ty, mtype, Recusion, anitime);
  FlyImageBase := FLYOMAAXEBASE;
  ReadyFrame := 65;
end;
{
procedure TFlyingBug.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin

  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then
  begin
    if not FixedEffect then
    begin
      img := FlyImageBase + (Dir16 div 2) * 10;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);
    end
    else
    begin
      img := curframe + MagExplosionBase;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);
    end;
  end;
end;
}

procedure TFlyingBug.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      img := FlyImageBase + (Dir16 div 2) * 10;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
      else
        d := ImgLib.GetCachedImage(img + curframe, px, py);
      if d <> nil then begin
        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d.ClientRect, d);
      end;
    end
    else begin
      img := curframe + MagExplosionBase;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);
      if d <> nil then begin
        GameCanvas.Draw(FlyX + px - UNITX div 2,
          FlyY + py - UNITY div 2,
          d.ClientRect, d);
      end;
    end;
  end;
end;

{ TFlyingFireBall }

function TFlyingFireBall.Run:Boolean;
begin
  Result := inherited Run;
end;
{
procedure TFlyingFireBall.LoadSurface(Sender: TObject);
var
  d: TTexture;
begin

  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then
  begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(FlyImageBase + (GetFlyDirection(FlyX, FlyY, targetx, targety) * 10) + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(FlyImageBase + (GetFlyDirection(FlyX, FlyY, targetx, targety) * 10) + curframe, px, py);
  end;
end;
}

procedure TFlyingFireBall.DrawEff();
var
  d:TTexture;
begin
  if m_boActive and ((abs(FlyX - fireX) > ReadyFrame) or (abs(FlyY - fireY) > ReadyFrame)) then begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(FlyImageBase + (GetFlyDirection(FlyX, FlyY, targetx, targety) * 10) + curframe, px, py)
    else
      d := ImgLib.GetCachedImage(FlyImageBase + (GetFlyDirection(FlyX, FlyY, targetx, targety) * 10) + curframe, px, py);
    if d <> nil then
      GameCanvas.DrawBlend(
        FlyX + px - UNITX div 2,
        FlyY + py - UNITY div 2,
        d);
  end;
end;
// //噬血术

constructor TBloodBiteEffect.Create(id, effnum, sx, sy, tx, ty:Integer; mtype:TMagicType; Recusion:Boolean; anitime:Integer; ANewLevel:Integer);
begin
  // 噬血术强化技能打中目标后目标身上物资 chongchong 2014-05-19
  inherited Create(id, effnum, sx, sy, tx, ty, mtype, Recusion, anitime);
  NewLevel := ANewLevel;
  TargetActor := nil; // target;
  NextFrameTime := 40;
  ExplosionFrame := 20;
  ImgLib := g_WMagic9Images;
  light := 3;
  if NewLevel in [1..3] then begin
    MagExplosionBase := 690;
  end
  else if NewLevel in [4..6] then begin
    MagExplosionBase := 840;
  end
  else begin
    MagExplosionBase := 990;
  end;
end;

procedure TBloodBiteEffect.DrawEff();
var
  nIdx:Integer;
  d:TTexture;
begin
  // if m_boActive then begin
  if curframe >= 20 then begin
    if NewLevel in [1..3] then begin
      nIdx := MagExplosionBase + (NewLevel - 1) * 20 + curframe;
    end
    else if NewLevel in [4..6] then begin
      nIdx := MagExplosionBase + (NewLevel - 4) * 20 + curframe;
    end
    else begin
      nIdx := MagExplosionBase + (NewLevel - 7) * 20 + curframe;
    end;
  end
  else begin
    nIdx := MagExplosionBase + curframe;
  end;

  if ImgLib <> nil then begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(nIdx, px, py)
    else
      d := ImgLib.GetCachedImage(nIdx, px, py);
    if d <> nil then begin
      GameCanvas.DrawBlend(
        FlyX + px - UNITX div 2,
        FlyY + py - UNITY div 2,
        d);
    end;
  end;
end;
{
procedure TBloodBiteEffect.LoadSurface(Sender: TObject);
var
  d: TTexture;
  nIdx: Integer;
begin
  if curframe >= 20 then
  begin
    if NewLevel in [1..3] then
    begin
      nIdx := MagExplosionBase + (NewLevel - 1) * 20 + curframe;
    end
    else if NewLevel in [4..6] then
    begin
      nIdx := MagExplosionBase + (NewLevel - 4) * 20 + curframe;
    end
    else
    begin
      nIdx := MagExplosionBase + (NewLevel - 7) * 20 + curframe;
    end;
  end
  else
  begin
    nIdx := MagExplosionBase + curframe;
  end;
  if ImgLib <> nil then
  begin
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(nIdx, px, py)
    else
      d := ImgLib.GetCachedImage(nIdx, px, py);
  end;
end;
}
{--------------------------------------------------------}

constructor TContinuousEffect.Create(effbase, sx, sy, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 3;
  TargetActor := target;
  NextFrameTime := 50;
end;

function TContinuousEffect.Run:Boolean;
begin
  Result := inherited Run;
end;
{
procedure TContinuousEffect.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin
  if m_boActive and ((abs(FlyX - fireX) > 30) or (abs(FlyY - fireY) > 30) or FixedEffect) then
  begin
    if ImgLib <> nil then
    begin
      if not FixedEffect then
      begin

        if EffectNumber = 105 then                                                                  // 惊雷爆
          img := EffectBase + FLYBASE + curframe
        else
          img := EffectBase + FLYBASE + Dir16 * 10 + curframe;

      // img := EffectBase + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
      end
      else
      begin
        if EffectNumber = 109 then
        begin                                                                                       // 八卦掌
          img := MagExplosionBase + curframe + Dir16 * 10;
        end
        else if EffectNumber = 111 then
        begin                                                                                       // 万剑归宗
          img := MagExplosionBase + curframe + Dir16 * 10;
        end
        else
          img := MagExplosionBase + curframe;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
      end;
    end;
  end;
end;
}

procedure TContinuousEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  // meff: TMapEffect;
begin
  if m_boActive and ((abs(FlyX - fireX) > 15) or (abs(FlyY - fireY) > 15) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if ImgLib <> nil then begin
      if not FixedEffect then begin
        if EffectNumber = 105 then // 惊雷爆
          img := EffectBase + FLYBASE + curframe
        else
          img := EffectBase + Dir16 * 10 + curframe;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d.ClientRect, d);
        end;
      end
      else begin

        if EffectNumber = 109 then begin // 八卦掌
          img := MagExplosionBase + curframe + Dir16 * 10;
        end
        else if EffectNumber = 111 then begin // 万剑归宗
          img := MagExplosionBase + curframe + Dir16 * 10;
        end
        else
          img := MagExplosionBase + curframe;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);

        if d <> nil then begin
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2,
            FlyY + py - UNITY div 2,
            d);
        end;
      end;
    end;
  end;
end;

{---------------------------冰天雪地-------------------------}

constructor TExploBingtianxuediEffect.Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, 0,
    sx, sY,
    tx, ty,
    mtExplosion,
    False,
    0);
  MagExplosionBase := effbase;
  TargetActor := nil;
  NextFrameTime := 50;
  ExplosionFrame := 8;
end;

procedure TExploBingtianxuediEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  //shx, shy: Integer;
begin
  if m_boActive then begin

    //shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    //shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then begin
      img := MagExplosionBase + (Dir16 div 2) * 10 + curframe; // EXPLOSIONBASE;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := ImgLib.GetCachedGrayImage(img, px, py)
      else
        d := ImgLib.GetCachedImage(img, px, py);

      if d <> nil then
        GameCanvas.DrawBlend(
          FlyX + px - UNITX div 2,
          FlyY + py - UNITY div 2,
          d);
    end;
  end;
end;
{
procedure TExploBingtianxuediEffect.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
  shx, shy: Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > 15) or (abs(FlyY - fireY) > 15) or FixedEffect) then
  begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then
    begin
      if not FixedEffect then
      begin
        img := EffectBase + FLYBASE + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
      end
      else
      begin
        img := MagExplosionBase + (Dir16 div 2) * 10 + curframe;                                    // EXPLOSIONBASE;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img, px, py)
        else
          d := ImgLib.GetCachedImage(img, px, py);
      end;
    end;
  end;
end;
}
{---------------------------三焰咒-------------------------}

const
  FIREGUNFRAME_SanYanZhou = 3;

constructor TExploSanYanZhouEffect.Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sY,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 5;
  TargetActor := target;
  NextFrameTime := 50;

  FillChar(FireNodes, SizeOf(TFireNode) * FIREGUNFRAME_SanYanZhou, #0);
  OutofOil := False;
  firetime := MyGetTickCount;
  fire2time := MyGetTickCount;
end;

function TExploSanYanZhouEffect.Run:Boolean;
var
  I:Integer;
  allgone:Boolean;
  boLoadSurface:Boolean;
begin
  Result := True;
  boLoadSurface := False;

  if MyGetTickCount - steptime >= longword(NextFrameTime) then begin
    Shift;
    boLoadSurface := True;
    steptime := MyGetTickCount;

    if MyGetTickCount - fire2time >= longword(NextFrameTime * 2) then begin
      fire2time := MyGetTickCount;
      // if not FixedEffect then begin
      if not OutofOil then begin
        if (MagOwner <> nil) and ((abs(rx - TActor(MagOwner).m_nRx) >= 10) or (abs(ry - TActor(MagOwner).m_nRy) >= 10) or (MyGetTickCount - firetime > 800)) then
          OutofOil := True;
        for I := FIREGUNFRAME_SanYanZhou - 2 downto 0 do begin
          FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
          FireNodes[I + 1] := FireNodes[I];
        end;
        FireNodes[0].firenumber := 1;
        FireNodes[0].X := FlyX;
        FireNodes[0].Y := FlyY;
      end
      else begin
        allgone := True;
        for I := FIREGUNFRAME_SanYanZhou - 2 downto 0 do begin
          if FireNodes[I].firenumber <= FIREGUNFRAME_SanYanZhou then begin
            FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
            FireNodes[I + 1] := FireNodes[I];
            allgone := False;
            boLoadSurface := False;
          end;
        end;
        if allgone then Result := False;
      end;
    end;
  end;

  if boLoadSurface then
    PlayScene.LoadSurface(LoadSurface);
end;

procedure TExploSanYanZhouEffect.DrawEff();
var
  img:Integer;
  d1, d2:TTexture;
  shx, shy:Integer;

  //I, fireX, fireY, prx, pry: Integer;
  I, _fireX, _fireY, prx, pry:Integer; //HZQ 20230525 解决和类成员函数同名的问题
  Pt1, Pt2:TPoint;
begin
  {$MESSAGE HINT '关于三焰咒技能的更改 fireX, fireY改为带下划线的，初始判断时使用类成员未初始化，参考地狱火技能'}
  if m_boActive and ((abs(FlyX - fireX) > 30) or (abs(FlyY - fireY) > 30) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if not FixedEffect then begin
      img := EffectBase + Dir16 * 10;

      d1 := ImgLib.GetCachedImage(img + curframe + 160, Pt1.X, Pt1.Y);
      if d1 <> nil then begin
        GameCanvas.Draw(FlyX + Pt1.X - UNITX div 2 - shx, FlyY + Pt1.Y - UNITY div 2 - shy, d1);
      end;

      d2 := ImgLib.GetCachedImage(img + curframe, Pt2.X, Pt2.Y);
      if d2 <> nil then begin
        GameCanvas.DrawBlend(FlyX + Pt2.X - UNITX div 2 - shx, FlyY + Pt2.Y - UNITY div 2 - shy, d2);
      end;

      prx := -1;
      pry := -1;
      for I := 0 to FIREGUNFRAME_SanYanZhou - 1 do begin
        if (FireNodes[I].firenumber <= FIREGUNFRAME_SanYanZhou) and (FireNodes[I].firenumber > 0) then begin
          // 修正地狱火模式 cboEffect, 开始: 2770; 数量25飞行效果在左上角播放，原因是没有初始化的坐标也播放了  2019-04-30 10:52:28
          if (FireNodes[I].firenumber < I + 1) then Continue;

          shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
          shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

          if (d1 <> nil) or (d2 <> nil) then begin
            _fireX := FireNodes[I].X + Pt1.X - UNITX div 2 - shx;
            _fireY := FireNodes[I].Y + Pt1.Y - UNITY div 2 - shy;
            if (_fireX <> prx) or (_fireY <> pry) then begin
              prx := _fireX;
              pry := _fireY;

              if d1 <> nil then
                GameCanvas.Draw(_fireX, _fireY, d1);

              if d2 <> nil then
                GameCanvas.DrawBlend(_fireX - Pt1.X + Pt2.X, _fireY - Pt1.Y + Pt2.Y, d2);
            end;
          end;
        end;
      end;
    end else begin
      img := MagExplosionBase + curframe;
      d1 := ImgLib.GetCachedImage(img, px, py);
      if d1 <> nil then begin
        GameCanvas.DrawBlend(FlyX + px - UNITX div 2, FlyY + py - UNITY div 2, d1);
      end;
    end;
  end;
end;

{------------------------虎啸诀---------------------------}

constructor TExploHuXiaoJueZhouEffect.Create(effbase, sx, sY, tx, ty:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    sx, sY,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := 5;
  TargetActor := target;
  NextFrameTime := 80;
end;

{procedure TExploHuXiaoJueZhouEffect.LoadSurface(Sender: TObject);
var
  img: Integer;
  d: TTexture;
begin
  if m_boActive and ((abs(FlyX - fireX) > 30) or (abs(FlyY - fireY) > 30) or FixedEffect) then
  begin
    if not FixedEffect then
    begin
      img := EffectBase + Dir16 * 5 + curframe;
      d := ImgLib.GetCachedImage(img, px, py);

      d := ImgLib.GetCachedImage(img + 80, px, py);

    end
    else
    begin
      img := 3740 + Dir16 * 5 + curframe;
      d := ImgLib.GetCachedImage(img, px, py);

      img := MagExplosionBase + Dir16 * 5 + curframe;
      d := ImgLib.GetCachedImage(img, px, py);
    end;
  end;
end;
}

procedure TExploHuXiaoJueZhouEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > 30) or (abs(FlyY - fireY) > 30) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if not FixedEffect then begin
      // 修正虎啸诀发出后的动作(非打击) chongchong 2013-11-09
      //img := EffectBase + Dir16 * 5 + curframe;
      if curframe >= 5 then curframe := 0;
      img := 3580 + Dir16 * 5 + curframe;

      d := ImgLib.GetCachedImage(img, px, py);
      if d <> nil then begin
        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d);
      end;

      d := ImgLib.GetCachedImage(img + 80, px, py);
      if d <> nil then begin
        GameCanvas.DrawBlend(
          FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d);
      end;
    end
    else begin
      img := 3740 + Dir16 * 5 + curframe;
      d := ImgLib.GetCachedImage(img, px, py);
      if d <> nil then begin
        GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
          FlyY + py - UNITY div 2 - shy,
          d);
      end;

      if d <> nil then begin
        GameCanvas.DrawBlend(
          FlyX + px - UNITX div 2,
          FlyY + py - UNITY div 2,
          d);
      end;

      img := MagExplosionBase + Dir16 * 5 + curframe;
      d := ImgLib.GetCachedImage(img, px, py);
      if d <> nil then begin
        GameCanvas.DrawBlend(
          FlyX + px - UNITX div 2,
          FlyY + py - UNITY div 2,
          d);
      end;

    end;
  end;
end;

{ TRedThunderEffect }

constructor TRedThunderEffect.Create(effbase, tx, ty:integer;
  target:TObject);
begin
  inherited Create(111, effbase,
    tx, ty,
    tx, ty, //TActor(target).XX, TActor(target).m_nCurrY,
    mtRedThunder,
    FALSE,
    0);
  TargetActor := target;
  n0 := random(7);
end;

procedure TRedThunderEffect.DrawEff;
var
  img, px, py:integer;
  d:TTexture;
begin
  ImgLib := {FrmMain.WDragonImg} g_WDragonImg;
  img := EffectBase;

  (*
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(img + {(7 * n0)} + curframe, px, py)
  else
    d := ImgLib.GetCachedImage(img + {(7 * n0)} + curframe, px, py);
  *)
  d := Imglib.GetCachedImage(img + (7 * n0) + curframe, px, py);
  //end;
  if d <> nil then begin
    GameCanvas.DrawBlend(
      FlyX + px - UNITX div 2,
      FlyY + py - UNITY div 2,
      d);
  end;
end;

{ TFireDragonEffect }

constructor TFireDragonEffect.Create(id, effnum, sx, sy, tx, ty:integer;
  mtype:TMagicType; Recusion:Boolean; anitime:integer);
begin
  inherited Create(id, effnum, sx, sy, tx, ty, mtype, Recusion, anitime);
  FlyX1 := 0;
  FlyY1 := 0;
  FlyX2 := 0;
  FlyY2 := 0;
  boflyFixedEffect := false;
end;

procedure TFireDragonEffect.DrawEff;
var
  img:integer;
  d:TTexture;
  shx, shy:integer;
begin
  if m_boActive and ((Abs(FlyX - fireX) > 15) or (Abs(FlyY - fireY) > 15) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if not FixedEffect then begin
      //img := EffectBase + FLYBASE + Dir16 * 10;
      // 与方向有关的魔法效果
      case Dir16 of
        7:img := EffectBase + FLYBASE * 0;
        8:img := EffectBase + FLYBASE * 1;
        9:img := EffectBase + FLYBASE * 2;
        10:img := EffectBase + FLYBASE * 3;
        11:img := EffectBase + FLYBASE * 4;
        12:img := EffectBase + FLYBASE * 5;
        else
          img := -1;
      end;

      if img >= 0 then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);

        if d <> nil then
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d);
      end;
    end;
  end
  else begin
    // 与方向无关的魔法效果（例如爆炸）
  {img := MagExplosionBase + curframe;                                                             //EXPLOSIONBASE;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := ImgLib.GetCachedGrayImage(img, px, py)
  else
    d := ImgLib.GetCachedImage(img, px, py);

  if not boflyFixedEffect then
  begin
    flyx2 := TActor(TargetActor).m_nCurrX;
    flyy2 := TActor(TargetActor).m_nCurrY;
    boflyFixedEffect := true;
  end;
  PlayScene.ScreenXYfromMCXY(flyx2, flyy2, FlyX1, FlyY1);
  if d <> nil then
  begin
    GameCanvas.DrawBlend(
      FlyX1 + px - UNITX div 2,
      FlyY1 + py - UNITY div 2,
      d);
  end; }

    img := MagExplosionBase + curframe; // EXPLOSIONBASE;
    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
      d := ImgLib.GetCachedGrayImage(img, px, py)
    else
      d := ImgLib.GetCachedImage(img, px, py);

    if d <> nil then
      GameCanvas.DrawBlend(
        FlyX + px - UNITX div 2,
        FlyY + py - UNITY div 2,
        d);
  end;
end;

constructor TCustomMonFlyEffect.Create(effbase, sx, sy, tx, ty:Integer;
  target:TObject; FlyFrameCount:Integer; AExplosionImgLib:TGameImages;
  AExplosionLockTarget:Boolean; AIsFireGunMode:Boolean);
begin
  inherited Create(111, effbase,
    sx, sy,
    tx, ty,
    mtExploBujauk,
    True,
    0);
  frame := FlyFrameCount;
  TargetActor := target;
  NextFrameTime := 50;
  MagicBlend := True;
  FExplosionImgLib := AExplosionImgLib;
  ExplosionLockTarget := AExplosionLockTarget;

  FlyEffImgLib := nil;
  FlyEffStartIndex := -1;
  FlyDrawMode := mdmBlend;
  FlyEffDrawMode := mdmBlend;

  FlyLightRange := 0;
  ExplosionLightRange := 0;

  LockTarget := 0;
  LockTargetX := 0;
  LockTargetY := 0;

  FTargetList := TList.Create;
  FTigerOnExplosion := False;
  FTigerOnFinished := False;

  if FlyFrameCount > 2 then begin
    FIsFireGunMode := AIsFireGunMode;
    SetLength(FireNodes, FlyFrameCount);
    FillChar(FireNodes[0], SizeOf(TFireNode) * FlyFrameCount, #0);
    FOutofOil := False;
    FFiretime := MyGetTickCount;

    // 不把这个去掉，一大块全堆在目标身上 chongchong 2016-08-19
    if FisFireGunMode then
      TargetActor := nil;
  end;

  FIsPlaySound := False;
end;

destructor TCustomMonFlyEffect.Destroy;
var
  I:Integer;
begin
  if Assigned(FTargetList) then begin
    for I := 0 to FTargetList.Count - 1 do begin
      Dispose(PInt64(FTargetList.Items[I]));
    end;

    FTargetList.Free;
  end;
  inherited;
end;

function TCustomMonFlyEffect.Shift:Boolean;
begin
  Result := inherited Shift;
end;

// 支持空目标播放目标效果 chongchong 2015-07-31 17:04:20

function TCustomMonFlyEffect.Run:Boolean;
var
  I:Integer;
  allgone:Boolean;
  boLoadSurface:Boolean;
begin
  if FIsFireGunMode then begin
    Result := True;
    boLoadSurface := False;

    if MyGetTickCount - steptime > longword(NextFrameTime) then begin
      Shift;
      boLoadSurface := True;
      steptime := MyGetTickCount;
      if not FOutofOil then begin
        if (MagOwner <> nil) and ((abs(rx - TActor(MagOwner).m_nRx) >= 5) or (abs(ry - TActor(MagOwner).m_nRy) >= 5) or (MyGetTickCount - FFiretime > 800)) then
          FOutofOil := True;
        for I := Length(FireNodes) - 2 downto 0 do begin
          FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
          FireNodes[I + 1] := FireNodes[I];
        end;
        FireNodes[0].firenumber := 1;
        FireNodes[0].X := FlyX;
        FireNodes[0].Y := FlyY;
      end
      else begin
        allgone := True;
        for I := Length(FireNodes) - 2 downto 0 do begin
          if FireNodes[I].firenumber <= Length(FireNodes) then begin
            FireNodes[I].firenumber := FireNodes[I].firenumber + 1;
            FireNodes[I + 1] := FireNodes[I];
            allgone := False;
            boLoadSurface := False;
          end;
        end;
        if allgone then Result := False;
      end;
    end;

    if boLoadSurface then
      PlayScene.LoadSurface(LoadSurface);

    Exit;
  end;

  Result := inherited Run;
  if FixedEffect and (not FIsPlaySound) then begin
    FIsPlaySound := True;
    if MagOwner <> nil then
      g_PlaySound.PlaySound(TActor(MagOwner).m_nMagicExplosionSound);
  end;
end;

procedure TCustomMonFlyEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
  I, _fireX, _fireY, prx, pry:Integer;
  _fireX2, _fireY2, prx2, pry2:Integer;
begin
  if FIsFireGunMode then begin
    // 地狱火模式:先不要目标效果，用了目标效果不是还没到目标身上就没了，就是堆一起 chongchong 2016-08-19
    //if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then
    if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1)) then begin
      if (not FixedEffect) or ((FExplosionImgLib = nil) or (ExplosionFrame = 0)) then begin
        prx := -1;
        pry := -1;

        prx2 := -1;
        pry2 := -1;
        for I := 0 to Length(FireNodes) - 1 do begin
          if (FireNodes[I].firenumber <= Length(FireNodes)) and (FireNodes[I].firenumber > 0) then begin
            // 修正地狱火模式 cboEffect, 开始: 2770; 数量25飞行效果在左上角播放，原因是没有初始化的坐标也播放了 2019-04-30 10:52:28
            if (FireNodes[I].firenumber < I + 1) then Continue;

            shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
            shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

            if (FlyEffImgLib <> nil) and (FlyEffStartIndex >= 0) then begin
              img := FlyEffStartIndex + (FireNodes[I].firenumber - 1);

              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := FlyEffImgLib.GetCachedGrayImage(img, px, py)
              else
                d := FlyEffImgLib.GetCachedImage(img, px, py);

              if d <> nil then begin
                _fireX := FireNodes[I].X + px - UNITX div 2 - shx;
                _fireY := FireNodes[I].Y + py - UNITY div 2 - shy;
                if (_fireX <> prx) or (_fireY <> pry) then begin
                  prx := _fireX;
                  pry := _fireY;

                  if FlyEffDrawMode = mdmBlend then
                    GameCanvas.DrawBlend(_fireX, _fireY, d)
                  else
                    GameCanvas.Draw(_fireX, _fireY, d);
                end;
              end;
            end;

            img := EffectBase + (FireNodes[I].firenumber - 1);
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := ImgLib.GetCachedGrayImage(img, px, py)
            else
              d := ImgLib.GetCachedImage(img, px, py);
            if d <> nil then begin
              _fireX2 := FireNodes[I].X + px - UNITX div 2 - shx;
              _fireY2 := FireNodes[I].Y + py - UNITY div 2 - shy;
              if (_fireX2 <> prx2) or (_fireY2 <> pry2) then begin
                prx2 := _fireX2;
                pry2 := _fireY2;

                if FlyDrawMode = mdmBlend then
                  GameCanvas.DrawBlend(_fireX2, _fireY2, d)
                else
                  GameCanvas.Draw(_fireX2, _fireY2, d);
              end;
            end;
          end;
        end;
      end
        {
        else
        begin
          if NextFrameTime <> NextExplosionFrameTime then
            NextFrameTime := NextExplosionFrameTime;

          if not FTigerOnExplosion then
          begin
            if Assigned(FOnExplosion) then
              FOnExplosion(Self);
            FTigerOnExplosion := True;
          end;

          if ((FExplosionImgLib = nil) or (ExplosionFrame = 0) or (curframe >= ExplosionFrame - 2)) and (not FTigerOnFinished) then
          begin
            if Assigned(FOnFinished) then
              FOnFinished(Self);
            FTigerOnFinished := True;
          end;

          if FExplosionImgLib <> nil then
          begin
            img := MagExplosionBase + curframe;
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := FExplosionImgLib.GetCachedGrayImage(img, px, py)
            else
              d := FExplosionImgLib.GetCachedImage(img, px, py);

            if d <> nil then
            begin
              light := ExplosionLightRange;

              // 爆炸效果不锁定目标 chongchong 2014-10-18
              if not ExplosionLockTarget then
                TargetActor := nil;

              if MagicBlend then
              begin
                GameCanvas.DrawBlend(
                  FlyX + px - UNITX div 2,
                  FlyY + py - UNITY div 2,
                  d);
              end
              else
              begin
                GameCanvas.Draw(
                  FlyX + px - UNITX div 2,
                  FlyY + py - UNITY div 2,
                  d);
              end;
            end;
          end;
        end;
        }
    end;

    Exit;
  end;

  // 修正距离太近时看不到飞行效果 chongchong 2015-04-11
  if m_boActive and ((abs(FlyX - fireX) > 1) or (abs(FlyY - fireY) > 1) or FixedEffect) then begin
    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;
    if not FixedEffect then begin
      // 飞行特效
      if (FlyEffImgLib <> nil) and (FlyEffStartIndex >= 0) then begin
        img := FlyEffStartIndex;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := FlyEffImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := FlyEffImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          if FlyEffDrawMode = mdmBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d)
          else
            GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d.ClientRect, d);
        end;
      end;

      // 飞行效果 chongchong 2015-03-09
      if ImgLib <> nil then begin
        img := EffectBase;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          light := FlyLightRange;
          // 混合效果
          if FlyDrawMode = mdmBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d)
          else
            GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d.ClientRect, d);
        end;
      end
    end
    else begin
      if NextFrameTime <> NextExplosionFrameTime then
        NextFrameTime := NextExplosionFrameTime;

      if not FTigerOnExplosion then begin
        if Assigned(FOnExplosion) then
          FOnExplosion(Self);
        FTigerOnExplosion := True;
      end;

      if ((FExplosionImgLib = nil) or (ExplosionFrame = 0) or (curframe >= ExplosionFrame - 2)) and (not FTigerOnFinished) then begin
        if Assigned(FOnFinished) then
          FOnFinished(Self);
        FTigerOnFinished := True;
      end;

      if FExplosionImgLib <> nil then begin
        if MagExplosionBase >= 0 then begin
          img := MagExplosionBase + curframe;
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := FExplosionImgLib.GetCachedGrayImage(img, px, py)
          else
            d := FExplosionImgLib.GetCachedImage(img, px, py);

          if d <> nil then begin
            light := ExplosionLightRange;

            // 爆炸效果不锁定目标 chongchong 2014-10-18
            if not ExplosionLockTarget then
              TargetActor := nil;

            if MagicBlend then begin
              GameCanvas.DrawBlend(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d);
            end
            else begin
              GameCanvas.Draw(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d);
            end;
          end;
        end;

        if MagExplosionBase2 >= 0 then begin
          img := MagExplosionBase2 + curframe;
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := FExplosionImgLib.GetCachedGrayImage(img, px, py)
          else
            d := FExplosionImgLib.GetCachedImage(img, px, py);

          if d <> nil then begin
            light := ExplosionLightRange;

            // 爆炸效果不锁定目标 chongchong 2014-10-18
            if not ExplosionLockTarget then
              TargetActor := nil;

            if MagicBlend2 then begin
              GameCanvas.DrawBlend(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d);
            end
            else begin
              GameCanvas.Draw(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d);
            end;
          end;
        end;
      end;
    end;
  end;
end;

{ TCustomMonTargetEffect }

constructor TCustomMonTargetEffect.Create(effbase, effBase2, effframe, nX, nY:Integer);
var
  NewX, NewY:Integer;
begin
  PlayScene.ScreenXYfromMCXY(nX, nY, NewX, NewY);
  inherited Create(111, effbase,
    NewX, NewY,
    NewX, NewY,
    mtExplosion,
    False,
    0);
  targetx := NewX;
  targety := NewY;

  MagExplosionBase2 := effBase2;

  ImgLib := g_WEffectImg;
  TargetActor := nil;
  MagExplosionBase := effbase;
  ExplosionFrame := effframe;
  NextFrameTime := 100;

  LockTarget := 0;
  FTigerOnFinished := False;
  FTargetList := TList.Create;
end;

constructor TCustomMonTargetEffect.Create(effbase, effBase2, effframe:Integer; target:TObject);
begin
  inherited Create(111, effbase,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    TActor(target).m_nCurrX, TActor(target).m_nCurrY,
    mtExplosion,
    False,
    0);
  MagExplosionBase2 := effBase2;
  ImgLib := g_WEffectImg;
  TargetActor := target;
  MagExplosionBase := effbase;
  ExplosionFrame := effframe;
  NextFrameTime := 100;

  LockTarget := 0;
  FTigerOnFinished := False;
  FTargetList := TList.Create;

  // 修正自定义技能飞行后爆炸有一定几率第一帧出现错位，原因可能是绘制时没有调用shift chongchong 2018-02-12
  Shift;
end;

function TCustomMonTargetEffect.Shift:Boolean;

{
function OverThrough(olddir, newdir: Integer): Boolean;
begin
  Result := False;
  if abs(olddir - newdir) >= 2 then
  begin
    Result := True;
    if ((olddir = 0) and (newdir = 15)) or ((olddir = 15) and (newdir = 0)) then
      Result := False;
  end;
end;
}
begin
  Result := inherited Shift;
  // 血灵教主在这里有bug，修改 chongchong 2014-09-10
  {
  Result := True;
  if Repetition then
  begin
    if MyGetTickCount - steptime > longword(NextFrameTime) then
    begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
        curframe := start;
    end;
  end
  else
  begin
    if (frame > 0) and (MyGetTickCount - steptime > longword(NextFrameTime)) then
    begin
      steptime := MyGetTickCount;
      Inc(curframe);
      if curframe > start + frame - 1 then
      begin
        curframe := start + frame - 1;
        Result := False;
      end;
    end;
  end;

  if FixedEffect then
  begin
    if frame = -1 then frame := ExplosionFrame;
    if TargetActor = nil then
    begin
      // FlyX := targetx - ((g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX);
      // FlyY := targety - ((g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY);
      // PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      // PlayScene.CXYfromMouseXY(FlyX, FlyY, rx, ry);
      rx := targetx;
      ry := targety;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
    end
    else
    begin
      rx := TActor(TargetActor).m_nRx;
      ry := TActor(TargetActor).m_nRy;
      PlayScene.ScreenXYfromMCXY(rx, ry, FlyX, FlyY);
      FlyX := FlyX + TActor(TargetActor).m_nShiftX;
      FlyY := FlyY + TActor(TargetActor).m_nShiftY;
    end;
  end;
  }
end;

procedure TCustomMonTargetEffect.DrawEff();
var
  img:Integer;
  d:TTexture;
  shx, shy:Integer;
begin
  if m_boActive and ((abs(FlyX - fireX) > 15) or (abs(FlyY - fireY) > 15) or FixedEffect) then begin

    shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
    shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

    if ImgLib <> nil then begin
      if not FixedEffect then begin
        img := EffectBase + FLYBASE + Dir16 * 10;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := ImgLib.GetCachedGrayImage(img + curframe, px, py)
        else
          d := ImgLib.GetCachedImage(img + curframe, px, py);

        if d <> nil then begin
          if DrawMode = mdmBlend then
            GameCanvas.DrawBlend(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d)
          else
            GameCanvas.Draw(
              FlyX + px - UNITX div 2 - shx,
              FlyY + py - UNITY div 2 - shy,
              d);
        end;

      end
      else begin
        if MagExplosionBase >= 0 then begin
          img := MagExplosionBase + curframe; // EXPLOSIONBASE;
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := ImgLib.GetCachedGrayImage(img, px, py)
          else
            d := ImgLib.GetCachedImage(img, px, py);

          if (curframe >= ExplosionFrame - 2) and (not FTigerOnFinished) then begin
            if Assigned(FOnFinished) then
              FOnFinished(Self);
            FTigerOnFinished := True;
          end;

          // 根据苹果引擎修改的，特殊处理某些技能特效，如冰霜雪雨等，特效排除边界
          if (MagicId = 66) and (curframe < 20) then begin
            Dec(py, 225);
            Inc(px, 25);
          end;
          if d <> nil then begin
            if DrawMode = mdmBlend then
              GameCanvas.DrawBlend(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d)
            else
              GameCanvas.Draw(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d)
          end;
        end;

        if MagExplosionBase2 >= 0 then begin
          img := MagExplosionBase2 + curframe; // EXPLOSIONBASE;
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := ImgLib.GetCachedGrayImage(img, px, py)
          else
            d := ImgLib.GetCachedImage(img, px, py);

          if (curframe >= ExplosionFrame - 2) and (not FTigerOnFinished) then begin
            if Assigned(FOnFinished) then
              FOnFinished(Self);
            FTigerOnFinished := True;
          end;

          // 根据苹果引擎修改的，特殊处理某些技能特效，如冰霜雪雨等，特效排除边界
          if (MagicId = 66) and (curframe < 20) then begin
            Dec(py, 225);
            Inc(px, 25);
          end;
          if d <> nil then begin
            if DrawMode2 = mdmBlend then
              GameCanvas.DrawBlend(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d)
            else
              GameCanvas.Draw(
                FlyX + px - UNITX div 2,
                FlyY + py - UNITY div 2,
                d)
          end;
        end;
      end;
    end;
  end;
end;

destructor TCustomMonTargetEffect.Destroy;
var
  I:Integer;
begin
  if Assigned(FTargetList) then begin
    for I := 0 to FTargetList.Count - 1 do begin
      Dispose(PInt64(FTargetList.Items[I]));
    end;
    FTargetList.Free;
  end;
  inherited;
end;

{ TJNExploBujaukEffect }

constructor TJNExploBujaukEffect.Create(effbase, sx, sy, tx, ty:integer;
  target:TObject);
begin
  inherited Create(112, effbase,
    sx, sy,
    tx, ty,
    mtExploBujauk,
    TRUE,
    0);
  frame := 8;
  TargetActor := target;
  NextFrameTime := 50;
end;

procedure TJNExploBujaukEffect.DrawEff;
var
  img:integer;
  d:TTexture;
  shx, shy:integer;
begin
  try
    if m_boActive and ((Abs(FlyX - fireX) > 50) or (Abs(FlyY - fireY) > 50) or FixedEffect) then begin
      shx := (g_MySelf.m_nRx * UNITX + g_MySelf.m_nShiftX) - FireMyselfX;
      shy := (g_MySelf.m_nRy * UNITY + g_MySelf.m_nShiftY) - FireMyselfY;

      if not FixedEffect then begin
        img := EffectBase + (dir16 div 2) * 10;
        d := ImgLib.GetCachedImage(img + curframe, px, py);
        if d <> nil then begin
          GameCanvas.Draw(FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d.ClientRect, d);
        end;

        if EffectBase = 140 then
          d := ImgLib.GetCachedImage(img + curframe + 170, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(
            FlyX + px - UNITX div 2 - shx,
            FlyY + py - UNITY div 2 - shy,
            d);
        end;
      end
      else begin
        //气惯
        img := MagExplosionBase + curframe;
        d := ImgLib.GetCachedImage(img, px, py);
        if d <> nil then begin
          GameCanvas.DrawBlend(
            FLyX + px - UNITX div 2,
            FlyY + py - UNITY div 2,
            d);
        end;
      end;
    end;
  except
    DebugOutStr('TJNExploBujaukEffect.DrawEff');
  end;
end;
{ TExplosion2Effect }

constructor TExplosion2Effect.Create(id, effnum, sx, sy, tx, ty:Integer;
  mtype:TMagicType; Recusion:Boolean; anitime:Integer);
begin
  IsPlayExplosion2 := False;
  inherited Create(id, effnum, sx, sy, tx, ty, mtype, Recusion, anitime);
end;

function TExplosion2Effect.Run:Boolean;
begin
  Result := inherited Run;

  if (not Result) and FixedEffect and (not IsPlayExplosion2) then begin
    IsPlayExplosion2 := True;
    curframe := 0;
    frame := ExplosionFrame_2;
    MagExplosionBase := MagExplosionBase_2;

    Result := True;
  end;

  //FixedEffect
  //m_nCurrentFrame := curframe;
end;

end.
