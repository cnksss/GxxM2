unit AxeMon;

interface

uses
  Windows,
  SysUtils,
  Classes,
  Grobal2,
  HGE,
  Graphics,
  SDK,
  ClFunc,
  magiceff,
  Actor,
  clEvent,
  HGECanvas,
  MMSystem;

const
  DEATHEFFECTBASE = 340;
  DEATHFIREEFFECTBASE = 2860;
  AXEMONATTACKFRAME = 6;
  KUDEGIGASBASE = 1445;
  COWMONFIREBASE = 1800;
  COWMONLIGHTBASE = 1900;
  ZOMBILIGHTINGBASE = 350;
  ZOMBIDIEBASE = 340;
  ZOMBILIGHTINGEXPBASE = 520;
  SCULPTUREFIREBASE = 1680;
  MOTHPOISONGASBASE = 3590;
  DUNGPOISONGASBASE = 3590;
  WARRIORELFFIREBASE = 820;
  // Jacky
  SUPERIORGUARDBASE = 760;

type
  TSkeletonOma = class(TActor) // Size:25C
  private
  protected
    EffectSurface:TTexture; // 0x240
    ax:Integer; // 0x244
    ay:Integer; // 0x248

  public
    constructor Create; override;
    // destructor Destroy; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Finalize; override;
  end;

  TMoonMon = class(TActor) // Size 0x274
    EffectSurface:TTexture; // 0x240
    nPx, nPy:Integer;
  protected
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Finalize; override;
  end;

  // Mon-kulou.wzlÀïÃæµÄ÷¼÷Ã
  TMonKuLou_1 = class(TSkeletonOma)
  private
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
  end;

  // Mon36.wzlÀïÃæµÄ¹ÖÎï
  TMon36_X = class(TSkeletonOma)
    EffectSurface:TTexture; // 0x240
    nPx, nPy:Integer;
  private

  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;
  end;

  TDualAxeOma = class(TSkeletonOma) // µµ³¢´øÁö´Â ¸÷
  private
  public
    procedure Run; override;
  end;

  TCatMon = class(TSkeletonOma)
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  private
  public
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure CalcActorFrame; override;
  end;

  TMon38_X = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay:Integer;
    IsDigup:Boolean;
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Finalize; override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
  end;

  TArcherMon = class(TCatMon) // Size: 0x25C Address: 0x00461A90
  public
    procedure Run; override;
  end;

  TScorpionMon = class(TCatMon)
  public
  end;

  THuSuABi = class(TSkeletonOma)
  public
    procedure LoadSurface(Sender:TObject); override;
  end;

  TZombiDigOut = class(TSkeletonOma)
  public
    procedure RunFrameAction(frame:Integer); override;
  end;

  TZombiZilkin = class(TSkeletonOma)
  public
  end;

  TWhiteSkeleton = class(TSkeletonOma)
  public
  end;

  TGasKuDeGi = class(TActor) // Size 0x274
  protected
    AttackEffectSurface:TTexture; // 0x250
    DieEffectSurface:TTexture; // 0x254
    BoUseDieEffect:Boolean; // 0x258
    firedir:Integer; // 0x25C
    fire16dir:Integer; // 0c260
    ax:Integer; // 0x264
    ay:Integer; // 0x268
    bx:Integer;
    by:Integer;
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;

  TFireCowFaceMon = class(TGasKuDeGi)
  public
    function light:Integer; override;
  end;

  TCowFaceKing = class(TGasKuDeGi)
  public
    function light:Integer; override;
  end;

  TZombiLighting = class(TGasKuDeGi)
  protected
  public
  end;
  TSuperiorGuard = class(TGasKuDeGi)
  protected
  public
  end;

  TExplosionSpider = class(TGasKuDeGi)
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
  end;

  TFlyingSpider = class(TSkeletonOma) // Size: 0x25C Address: 0x00461F38
  protected
  public
    procedure CalcActorFrame; override;
  end;

  TSculptureMon = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TMon23_1 = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TMon24_4 = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TMon26_6 = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TMon27_3 = class(TSkeletonOma) // Ñ©ÓòÎÀÊ¿ ±ù·åÐ§¹û
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
    m_boStoneMode:Boolean;
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  // Mon27_6--±ùÖùÀà
  TMon27_6 = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  // Mon29_0--ÎÚ¹êÀà
  TMon29_0 = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TMonEffect = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TDragonBallMonster = class(TSkeletonOma)
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    m_step:Byte; // 1-5½×¶Î
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  // ÑªÁé½ÌÖ÷ chongchong 2014-09-10
  TXueLingLeaderMonster = class(TSkeletonOma)
  private
    m_SeatsSurface:TTexture;
    m_SeatsOffset:TPoint;

    m_WeaponSurface:TTexture;
    m_WeaponOffset:TPoint;

    m_nOldCustomEffectFrame:Integer;
    m_nCustomEffectTick:DWORD;

    m_LineSurface:TTexture;
    m_LineOffset:TPoint;

    m_BallSurfaces:array[0..3] of TTexture;
    m_BallOffsets:array[0..3] of TPoint;
    m_BallMoveOffset:TPoint;
  public
    m_HideDrawBall:Boolean;
    m_nCustomEffectFrame:Integer;

    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;
    procedure Finalize; override;

    function CheckLoadSurface:Boolean; override;
  end;

  TFireSpiritMonster = class(TActor) // »ðÁé
  private
    AttackEffectSurface:TTexture;
    ax, ay:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TEggMonster = class(TActor) // µ°
  private
    AttackEffectSurface:TTexture;
    ax, ay:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;

  TLionMonster = class(TActor) // Ê¨×Ó Mon35-0
  private
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
    procedure Run; override;
  end;

  TMon35FireDragon = class(TActor) // Áú
    AttackEffectSurface:TTexture;
    ax, ay, firedir:Integer;
  private
    procedure AttackEff;
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;

  TSculptureKingMon = class(TSculptureMon)
  public
  end;

  TSmallElfMonster = class(TSkeletonOma)
  public
  end;

  TWarriorElfMonster = class(TSkeletonOma)
  private
    oldframe:Integer;
  public
    procedure RunFrameAction(frame:Integer); override; // ÇÁ·¡ÀÓ¸¶´Ù µ¶Æ¯ÇÏ°Ô ÇØ¾ßÇÒÀÏ
  end;
  // ºçÄ§Ð«ÎÀ
  TElectronicScolpionMon = class(TGasKuDeGi) // Size 0x274 0x3c
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
  end;
  TBossPigMon = class(TGasKuDeGi) // 0x3d
  protected
  public
    procedure LoadSurface(Sender:TObject); override;
  end;
  TKingOfSculpureKingMon = class(TGasKuDeGi) // 0x3e
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
  end;
  TSkeletonKingMon = class(TGasKuDeGi) // 0x3f
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
  end;
  TSamuraiMon = class(TGasKuDeGi) // 0x41
  protected
  public
  end;
  TSkeletonSoldierMon = class(TGasKuDeGi) // 0x42 0x43 0x44
  protected
  public
  end;
  TSkeletonArcherMon = class(TArcherMon) // Size: 0x26C Address: 0x004623B4 //0x45
    AttackEffectSurface:TTexture; // 0x25C
    bo260:Boolean;
    n264:Integer;
    n268:Integer;
  protected
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;
  TBanyaGuardMon = class(TSkeletonArcherMon) // Size: 0x270 Address: 0x00462430 0x46 0x47 0x48 0x4e
    n26C:TTexture;
  protected
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;
  TStoneMonster = class(TSkeletonArcherMon) // Size: 0x270 0x4d 0x4b
    n26C:TTexture;
  protected
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;
  TPBOMA1Mon = class(TCatMon) // 0x49
  protected
  public
    procedure Run; override;
  end;
  TPBOMA6Mon = class(TCatMon) // 0x4f
  protected
  public
    procedure Run; override;
  end;
  TAngel = class(TBanyaGuardMon) // Size: 0x27C 0x51
    n270:Integer;
    n274:Integer;
    n278:TTexture;
  protected
  public
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Finalize; override;
  end;

  // »ðÁú½ÌÖ÷ piaoyun 2013-08-19
  TFireDragon = class(TSkeletonArcherMon)
    n270:TTexture;
  private
    procedure AttackEff;
  protected
    firedir:Byte;
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    //procedure Finalize; override;
  end;

  // »ðÁúÊØ»¤ÊÞ piaoyun 2013-08-19
  TFireDragonGuard = class(TActor)
  private
    EffectSurface:TTexture;
    ax, ay:integer;
    deathframe:integer;
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):integer; override;
    procedure DrawChr(dx, dy:integer; blend:Boolean; boFlag:Boolean); override;
  end;

  TDragonStatue = class(TSkeletonArcherMon) // Size: 0x270 0x54
    n26C:TTexture;
  protected
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure Run; override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Finalize; override;
  end;
implementation

uses
  ClMain,
  SoundUtil,
  GameImages,
  MShare;

constructor TMoonMon.Create;
begin
  inherited Create;
  EffectSurface := nil;
end;

procedure TMoonMon.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
end;

procedure TMoonMon.CalcActorFrame;
var
  pm:pTMonsterAction;
  Actor:TActor;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;
  m_boReverseFrame := False;
  m_boUseEffect := False;
  m_boUseMagic := False;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then begin
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        end
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_DIGUP:begin
        m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_DIGDOWN:begin

      end;
    // SM_SPELL,
    SM_HIT,
      SM_FLYAXE,
      SM_LIGHTING:begin
        Actor := PlayScene.FindActor(m_nTargetRecog);
        if (Actor <> nil) and (Actor.m_boDeath) then {// ÐÞ¸´ÔÂÁé´òÊ¬Ìå chongchong 2018-05-24} begin
          Shift(m_btDir, 0, 0, 1);
          Exit;
        end;

        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime; // ÐÞÕýÔÂÁé¹¥´òÊ¬Ìå/¹ÖÎï´òµÄÎ»ÖÃºÍÔÂÁéÎ»ÖÃ²»Í¬²½ pm.ActAttack.ftime; chongchong 2018-05-24
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;

        m_boUseEffect := True;
        m_boUseMagic := True;
        m_nCurEffFrame := 0;
        m_nMagLight := 2;

        // ÔÂÁé£¬ÊÍ·Å¶¯×÷½áÊøºó£¬ÖÐ¼ä»áÍ£¶Ù0.5Ãë×óÓÒ£¬²Å»áÊÍ·ÅÀ¶É«µÄ·ÉÐÐÄ§·¨¡£ chongchong 2018-09-12 19:47:06
        m_nSpellFrame := 4; // DEFSPELLFRAME

        m_dwWaitMagicRequest := TimeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        m_CurMagic.ServerMagicCode := 111;
        m_CurMagic.MagicSerial := m_nMagicNum;
        m_CurMagic.EffectNumber := m_nMagicNum;
        m_CurMagic.targx := m_nTargetX;
        m_CurMagic.targy := m_nTargetY;
        m_CurMagic.target := m_nTargetRecog;
        m_CurMagic.EffectType := mtFly;
        m_CurMagic.NewLevel := 0;

        Shift(m_btDir, 0, 0, 1);

        { TODO -opiaoyun -cÔö¼Ó : Ôö¼ÓÔÂÁé¹¥»÷ÉùÒô¡¾2013-6-16¡¿ }
        if (m_btRace = 56) then begin
          m_nMagicStartSound := 11000;
          m_nMagicExplosionSound := 11002;
          PlaySound(m_nMagicStartSound);
        end;
      end;

    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;

        { TODO -opiaoyun -cÔö¼Ó : Ôö¼ÓÔÂÁéËÀÍöÉùÒô¡¾2013-6-16¡¿ }
        if m_btRace = 56 then begin
          m_nDieSound := 1925;
          PlaySound(m_nDieSound);
        end;
      end;
    SM_SKELETON:begin
        m_nStartFrame := pm.ActDeath.start; // + m_btDir;
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    {SM_ALIVE:
      begin
        m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
      end; }
  end;
end;

procedure TMoonMon.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;
  bofly:Boolean;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or
    (m_nCurrentAction = SM_BACKSTEP) or
    (m_nCurrentAction = SM_RUN) or
    (m_nCurrentAction = SM_HORSERUN) or
    (m_nCurrentAction = SM_RUSH) or
    (m_nCurrentAction = SM_RUSHKUNG) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);
  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boUseMagic then begin
      m_dwFrameTimetime := Round(m_dwFrameTime / 1.8);
    end
    else begin
      if m_boMsgMuch then
        m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
      else
        m_dwFrameTimetime := m_dwFrameTime;
    end;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        if m_boUseMagic then begin
          if (m_nCurEffFrame = m_nSpellFrame - 2) then begin
            if (m_CurMagic.ServerMagicCode >= 0) then begin
              Inc(m_nCurrentFrame);
              Inc(m_nCurEffFrame);
              m_dwStartTime := TimeGetTime;
            end;
          end
          else begin
            if m_nCurrentFrame < m_nEndFrame - 1 then Inc(m_nCurrentFrame);
            Inc(m_nCurEffFrame);
            m_dwStartTime := TimeGetTime;
          end;
        end
        else begin
          Inc(m_nCurrentFrame);
          m_dwStartTime := TimeGetTime;
        end;

      end
      else begin
        if m_boDelActionAfterFinished then begin
          m_dwDeleteTime := MyGetTickCount;
          m_boDelActor := True;
          m_boFreeActor := True;
        end;
        ActionEnded;
        { TODO -opiaoyun -cÐÞ¸´ : ÔÂÁéËÀÍöÌØÐ§ÎÊÌâ¢Ù¡¾2013-6-7¡¿ }
        (*if {(m_nCurrentAction = SM_DEATH) or }(m_nCurrentAction = SM_NOWDEATH) then begin
          m_nCurrentAction := SM_SKELETON;
          m_dwCurrentActionTick := MyGetTickCount;

          CalcActorFrame;
        end else begin
          m_nCurrentAction := 0;
        end;*)

        m_nCurrentAction := 0;
        m_boUseMagic := FALSE;
      end;

      if m_boUseMagic then begin
        if m_nCurEffFrame = m_nSpellFrame - 1 then begin
          if (m_CurMagic.ServerMagicCode > 0) and (m_CurMagic.EffectNumber > 0) then begin
            with m_CurMagic do
              PlayScene.NewMagic(Self,
                ServerMagicCode,
                EffectNumber, // Effect
                m_nCurrX,
                m_nCurrY,
                targx,
                targy,
                target,
                EffectType, // EffectType
                Recusion,
                anitime,
                bofly);
            if bofly then
              g_PlaySound.PlaySound(m_nMagicFireSound)
            else
              g_PlaySound.PlaySound(m_nMagicExplosionSound);
          end;
          m_CurMagic.ServerMagicCode := 0;
        end;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;

  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  { TODO -opiaoyun -cÐÞ¸´ : ÔÂÁéËÀÍöÌØÐ§ÎÊÌâ¢Ú¡¾2013-6-7¡¿ }
  if (prv <> m_nCurrentFrame) { and (nCurEffFrame <> m_nCurEffFrame)} then begin
    boLoadSurface := True;
    // ActionChanged; //×¢ÊÍµô -- paoyun
  end;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

function TMoonMon.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0;
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    if m_wAppearance in [30..34, 151] then
      m_nDownDrawLevel := 1;
    Result := pm.ActDeath.start + (pm.ActDeath.frame - 1);

  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TMoonMon.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  EffectSurface := nil;
  m_BodySurface := nil;
  mimg := g_WMonImages.Images[m_wAppearance];
  if mimg <> nil then begin
    if (not m_boReverseFrame) then begin
      case m_ColorEffect of
        ceGrayScale:begin
            m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance + 1) + m_nCurrentFrame, m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + m_nCurrentFrame, nPx, nPy);
          end;
        ceBright:begin
            m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance + 1) + m_nCurrentFrame, m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + m_nCurrentFrame, nPx, nPy);
          end;
        else begin
            m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance + 1) + m_nCurrentFrame, m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + m_nCurrentFrame, nPx, nPy);
          end;
      end;
    end
    else begin
      case m_ColorEffect of
        ceGrayScale:begin
            m_BodySurface := mimg.GetCachedGrayImage(
              GetOffset(m_wAppearance + 1) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedGrayImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              nPx, nPy);
          end;
        ceBright:begin
            m_BodySurface := mimg.GetCachedBrightImage(
              GetOffset(m_wAppearance + 1) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedBrightImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              nPx, nPy);
          end;
        else begin
            m_BodySurface := mimg.GetCachedImage(
              GetOffset(m_wAppearance + 1) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            // if not m_boDeath then
            EffectSurface := mimg.GetCachedImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              nPx, nPy);
          end;
      end;

    end;

    {if (m_BodySurface <> nil) and (m_BodySurface.Format in [apf_DXT1..apf_DXT5]) then begin

      if (not m_boReverseFrame) then begin
        m_BodyAlphaSurface := mimg.Alphas[GetOffset(m_wAppearance + 1) + m_nCurrentFrame];
      end else begin
        m_BodyAlphaSurface := mimg.Alphas[
          GetOffset(m_wAppearance + 1) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame)];
      end;

    end else m_BodyAlphaSurface := nil;}

  end;

  LoadActorIcons;

  ActionChanged;
end;

procedure TMoonMon.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
var
  idx, ax, ay:Integer;
  d:TTexture;
  wimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;

  if EffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      nPx + dx + m_nShiftX,
      nPy + dy + m_nShiftY,
      EffectSurface);
  end;

  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;

  if m_boUseMagic and (m_CurMagic.EffectNumber > 0) then begin
    if m_nCurEffFrame in [0..m_nSpellFrame - 1] then begin
      d := nil;
      GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel);
      idx := idx + m_nCurEffFrame;
      if wimg <> nil then
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := wimg.GetCachedGrayImage(idx, ax, ay)
        else
          d := wimg.GetCachedImage(idx, ax, ay);
      if d <> nil then
        GameCanvas.DrawBlend(
          dx + ax + m_nShiftX,
          dy + ay + m_nShiftY,
          d);
    end;
  end;
end;

{============================== TSkeletonOma =============================}

{--------------------------}

constructor TSkeletonOma.Create;
begin
  inherited Create;
  EffectSurface := nil;
  m_boUseEffect := FALSE;
end;

procedure TSkeletonOma.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
end;

procedure TSkeletonOma.CalcActorFrame;
var
  pm:pTMonsterAction;
  bofly:Boolean;
  meff:THeroShowEffect;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;
  m_boReverseFrame := FALSE;
  m_boUseEffect := FALSE;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then begin
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        end
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_DIGUP: {// °È±â ¾øÀ½, SM_DIGUP, ¹æÇâ ¾øÀ½.} begin
        if (m_btRace in [23]) then begin // or (m_btRace = 54) or (m_btRace = 55) then begin
          m_nStartFrame := pm.ActDeath.start;
          m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;
        end
        else begin
          m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
          m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;

          if (m_btRace = 254) and (m_wAppearance = 401) then begin
            meff := THeroShowEffect.Create(0 + 240, 10, self);

            if meff <> nil then begin
              meff.ImgLib := g_WMagic8Images16;
              meff.NextFrameTime := m_dwFrameTime;
              PlayScene.AddEffectList(meff);
              Exit;
            end
          end
            // ÕÙ»½Ç¿»¯Ê¥ÊÞ³öÉúÐ§¹û chongchong 2013-12-10
          else if (m_btRace = 54) and ((m_wAppearance = 270) or (m_wAppearance = 272) or (m_wAppearance = 274)) then begin
            meff := nil;
            if m_wAppearance = 270 then
              meff := THeroShowEffect.Create(0 + 220, 10, self)
            else if m_wAppearance = 272 then
              meff := THeroShowEffect.Create(0 + 230, 10, self)
            else if m_wAppearance = 274 then
              meff := THeroShowEffect.Create(0 + 240, 10, self);

            if meff <> nil then begin
              meff.ImgLib := g_WMagic8Images16;
              meff.NextFrameTime := m_dwFrameTime;
              PlayScene.AddEffectList(meff);
              Exit;
            end
            else begin
              m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
              m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
              m_dwFrameTime := pm.ActDeath.ftime;
              m_dwStartTime := TimeGetTime;
            end;
          end;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_DIGDOWN:begin
        if m_btRace = 55 then begin
          // ½Å¼ö1 ÀÎ °æ¿ì ¿ªº¯½Å
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
          m_boReverseFrame := True;
          // WarMode := FALSE;
          Shift(m_btDir, 0, 0, 1);
        end
        else if m_btRace = 255 then begin
          m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
          m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;

          m_boReverseFrame := True;
          // WarMode := FALSE;
          Shift(m_btDir, 0, 0, 1);
        end;
      end;
    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
        if (m_btRace = 16) or (m_btRace = 54) then
          m_boUseEffect := True;
        Shift(m_btDir, 0, 0, 1);

        if m_nMagicNum > 0 then begin
          SetMagicSound(m_nMagicNum);
          PlayScene.NewMagic(Self,
            111,
            m_nEffectNum, // Effect
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            m_MagicType, // EffectType
            True,
            0,
            bofly);
          // g_PlaySound.PlaySound(m_nMagicStartSound);

          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
      end;
    SM_HIT,
      SM_FLYAXE,
      SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
        if (m_btRace = 16) or (m_btRace = 54) then
          m_boUseEffect := True;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        // ÉñÊÞÅÀÏÂµÄÊ±ºòµ½ÕâÀïÀ´¾ÍÓÐÎÊÌâ  chongchong 2016-03-18
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;

        if ((m_btRace = 54) and ((m_wAppearance = 270) or (m_wAppearance = 272) or (m_wAppearance = 274))) or
          ((m_btRace = 55) and ((m_wAppearance = 271) or (m_wAppearance = 273) or (m_wAppearance = 275))) or
          ((m_btRace = 254) and (m_wAppearance = 401)) or
          ((m_btRace = 255) and (m_wAppearance = 402)) then begin
          meff := nil;
          if (m_wAppearance = 270) or (m_wAppearance = 271) then
            meff := THeroShowEffect.Create(0 + 1970, 10, self)
          else if (m_wAppearance = 272) or (m_wAppearance = 273) then
            meff := THeroShowEffect.Create(0 + 1980, 10, self)
          else if (m_wAppearance = 274) or (m_wAppearance = 275) or (m_wAppearance = 401) or (m_wAppearance = 402) then
            meff := THeroShowEffect.Create(0 + 1990, 10, self);

          if meff <> nil then begin
            meff.ImgLib := g_WMagic8Images16;
            meff.NextFrameTime := 80;
            PlayScene.AddEffectList(meff);
            Exit;
          end
        end
        else if m_btRace <> 22 then begin
          m_boUseEffect := True
        end
      end;
    SM_SKELETON:begin
        m_nStartFrame := pm.ActDeath.start;
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_ALIVE:begin
        m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

function TSkeletonOma.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    // ¿ì¸é±ÍÀÏ °æ¿ì
    if m_wAppearance in [30..34, 151] then // ¿ì¸é±ÍÀÎ °æ¿ì ½ÃÃ¼°¡ »ç¶÷À» µ¤´Â °ÍÀ» ¸·±â À§ÇØ
      m_nDownDrawLevel := 1;

    if m_boSkeleton then
      Result := pm.ActDeath.start
    else
      Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TSkeletonOma.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  EffectSurface := nil;

  case m_btRace of
    14, 15, 17, 22, 53:begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:EffectSurface := g_WMonImages.Indexs[3].GetCachedGrayImage(DEATHEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
            ceBright:EffectSurface := g_WMonImages.Indexs[3].GetCachedBrightImage(DEATHEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
            else
              EffectSurface := g_WMonImages.Indexs[3].GetCachedImage(DEATHEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
          end;

      end;
    23:begin
        if m_nCurrentAction = SM_DIGUP then begin
          m_BodySurface := nil;

          case m_ColorEffect of
            ceGrayScale:EffectSurface := g_WMonImages.Indexs[4].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, ax, ay);
            ceBright:EffectSurface := g_WMonImages.Indexs[4].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, ax, ay);
            else
              EffectSurface := g_WMonImages.Indexs[4].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, ax, ay);
          end;
          m_boUseEffect := True;
        end
        else
          m_boUseEffect := False;
      end;
  end;
end;

procedure TSkeletonOma.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;

  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        ActionChanged;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if prv <> m_nCurrentFrame then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;

  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

procedure TSkeletonOma.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;

  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;

  if m_boUseEffect then
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
end;

{============================== TSkeletonOma =============================}

// ÇØ°ñ ¿À¸¶(ÇØ°ñ, Å«µµ³¢ÇØ°ñ, ÇØ°ñÀü»ç)

{--------------------------}

procedure TDualAxeOma.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;
  meff:TFlyingAxe;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);

  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        ActionChanged;
      end;
      if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = AXEMONATTACKFRAME - 4) then begin
        // ¸¶¹ý ¹ß»ç
        meff := TFlyingAxe(PlayScene.NewFlyObject(Self,
          m_nCurrX,
          m_nCurrY,
          m_nTargetX,
          m_nTargetY,
          m_nTargetRecog,
          mtFlyAxe));
        if meff <> nil then begin
          meff.ImgLib := g_WMonImages.Indexs[3];
          case m_btRace of
            15:meff.FlyImageBase := FLYOMAAXEBASE;
            22:meff.FlyImageBase := THORNBASE;
          end;
        end;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if prv <> m_nCurrentFrame then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{============================== TGasKuDeGi =============================}

procedure TWarriorElfMonster.RunFrameAction(frame:Integer); // ÇÁ·¡ÀÓ¸¶´Ù µ¶Æ¯ÇÏ°Ô ÇØ¾ßÇÒÀÏ
var
  meff:TMapEffect;
begin
  if m_nCurrentAction = SM_HIT then begin
    if (frame = 5) and (oldframe <> frame) then begin
      {if m_wAppearance = 171 then begin
        meff := TMapEffect.Create(WARRIORELFFIREBASE + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMonImages.Indexs[18];
        meff.NextFrameTime := 100;
      end
      else  if m_wAppearance = 273 then begin
        meff := TMapEffect.Create(WARRIORELFFIREBASE + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMonImages.Indexs[18];
        meff.NextFrameTime := 100;
      end
      else  if m_wAppearance = 275 then begin
        meff := TMapEffect.Create(WARRIORELFFIREBASE + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMonImages.Indexs[18];
        meff.NextFrameTime := 100;
      end;    }
      {meff := TMapEffect.Create(WARRIORELFFIREBASE + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
      meff.ImgLib := g_WMonImages.Indexs[18];
      meff.NextFrameTime := 100; }
      // Ê¥ÊÞ»ðÈ¦ÐÞ¸´ piaoyun 2013-09-16
      if (m_wAppearance = 270) or (m_wAppearance = 271) then begin
        meff := TMapEffect.Create(250 + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMagic8Images16;
        meff.NextFrameTime := 100;
      end
      else if (m_wAppearance = 272) or (m_wAppearance = 273) then begin
        meff := TMapEffect.Create(330 + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMagic8Images16;
        meff.NextFrameTime := 100;
      end
      else if (m_wAppearance = 274) or (m_wAppearance = 275) then begin
        meff := TMapEffect.Create(410 + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMagic8Images16;
        meff.NextFrameTime := 100;
      end

        // ÐÂÉñÊÞ Åç»ð ÐÞ¸Ä Mon41-2, Mon41-3 chongchong 2014-11-21
      else if (m_wAppearance = 401) or (m_wAppearance = 402) then begin
        {
        if m_btDir = 6 then
          meff := TMapEffect.Create(1270 + 14 * m_btDir + 1, 6, m_nCurrX, m_nCurrY - 1)
        else
          meff := TMapEffect.Create(1270 + 14 * m_btDir, 6, m_nCurrX, m_nCurrY - 1);
        meff.ImgLib := g_WMonImages.Images[m_wAppearance];
        meff.NextFrameTime := 120;
        }
        meff := TMapEffect.Create(410 + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMagic8Images16;
        meff.NextFrameTime := 100;
      end

      else begin
        // ÐÞ¸´ÉñÊÞ±ÀÀ£ÎÊÌâ piaoyun 2013-10-17
        meff := TMapEffect.Create(WARRIORELFFIREBASE + 10 * m_btDir + 1, 5, m_nCurrX, m_nCurrY);
        meff.ImgLib := g_WMonImages.Indexs[18];
        meff.NextFrameTime := 100;
      end;

      PlayScene.AddEffectList(meff);
    end;
    oldframe := frame;
  end;
end;

{============================== TGasKuDeGi =============================}

// TCatMon : ±ªÀÌ,  ÇÁ·¡ÀÓÀº ÇØ°ñÀÌ¶û °°°í, ÅÍÁö´Â ¾Ö´Ï°¡ ¾øÀ½.

{--------------------------}

procedure TCatMon.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;
  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;
end;

procedure TCatMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if (m_wAppearance = 342) and (not m_boDeath) then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedGrayImage(
          m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedBrightImage(
          m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedImage(
          m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
    end;
  end;
end;

procedure TCatMon.DrawEff(dx, dy:Integer);
begin
  inherited;

  if (m_wAppearance = 342) and (AttackEffectSurface <> nil) then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TCatMon.CalcActorFrame;
var
  meff:TMagicEff;
  Actor:TActor;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited;
  if m_wAppearance = 342 then begin
    m_nCurrentFrame := -1;
    case m_nCurrentAction of
      SM_LIGHTING:begin
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(561, 36, Actor.m_nCurrX, Actor.m_nCurrY);
            meff.ImgLib := g_WMagic4Images;
            meff.NextFrameTime := 50;
            PlayScene.AddEffectList(meff);
          end;
        end;
    end;
  end;
end;

{============================= TArcherMon =============================}

procedure TArcherMon.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;
  meff:TMagicEff;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);

  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        ActionChanged;
      end;
      if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = 4) then begin
        if (m_wAppearance = 323) then begin
          meff := TFlyingArrow(PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrow));
          if meff <> nil then begin
            meff.ImgLib := g_WEffectImg; // WMon5Img;
            TFlyingArrow(meff).NextFrameTime := 30;
            TFlyingArrow(meff).ReadyFrame := 40;
            TFlyingArrow(meff).FlyImageBase := ARCHERBASE2;
          end;
        end
        else if (m_wAppearance = 328) or (m_wAppearance = 329) then begin
          meff := PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrowEx); // mt16  ÐÞÕýmon36µÄ¹ÖÎï¼ý¿´²»µ½ chongchong 2017-12-20
          if meff <> nil then begin
            meff.ImgLib := g_WMonImages.Indexs[33];
            TFlyingAxe(meff).NextFrameTime := 30;
            TFlyingArrow(meff).ReadyFrame := 40;
            meff.frame := 4;
            TFlyingAxe(meff).FlyImageBase := m_nBodyOffset + 86 + m_btDir * 20;
          end;
        end
        else if (m_wAppearance = 618) then begin
          meff := TFlyingArrowEx(PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrowEx));
          if meff <> nil then begin
            meff.ImgLib := g_WMonEffectImg;
            TFlyingArrowEx(meff).NextFrameTime := 30;
            TFlyingArrowEx(meff).ReadyFrame := 40;
            TFlyingArrowEx(meff).FlyImageBase := 180 + m_btDir * 10;
          end;
        end
          //MON38_2 Í¶ÖÀÏà¹ØÐ§¹û piaoyun 2014-01-04
        else if (m_wAppearance = 632) then begin
          meff := TFlyingArrow(PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrowEx));
          if meff <> nil then begin
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.frame := 10; // Õâ¸öËÆºõºÍÉä³ÌÓÐ¹Ø¡£¡£
            TFlyingArrow(meff).NextFrameTime := 30;
            TFlyingArrow(meff).ReadyFrame := 100;
            TFlyingArrow(meff).FlyImageBase := 1390 + m_btDir * 2;
          end;
        end
          // MON38_4 Í¶ÖÀÏà¹ØÐ§¹û piaoyun 2014-01-04
        else if (m_wAppearance = 634) then begin
          meff := TFlyingArrow(PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrowEx));
          if meff <> nil then begin
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.frame := 10; // Õâ¸öËÆºõºÍÉä³ÌÓÐ¹Ø¡£¡£
            TFlyingArrow(meff).NextFrameTime := 30;
            TFlyingArrow(meff).ReadyFrame := 100;
            TFlyingArrow(meff).FlyImageBase := 2200 + m_btDir * 2;
          end;
        end
        else begin
          meff := TFlyingArrow(PlayScene.NewFlyObject(Self,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFlyArrow));
          if meff <> nil then begin
            meff.ImgLib := g_WEffectImg; // WMon5Img;
            TFlyingArrow(meff).NextFrameTime := 30;
            TFlyingArrow(meff).ReadyFrame := 40;
            TFlyingArrow(meff).FlyImageBase := ARCHERBASE2;
          end;
        end;

      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if prv <> m_nCurrentFrame then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{============================= TZombiDigOut =============================}

procedure TZombiDigOut.RunFrameAction(frame:Integer);
var
  clEvent:TClEvent;
begin
  if m_nCurrentAction = SM_DIGUP then begin
    if frame = 6 then begin
      clEvent := TClEvent.Create(m_nCurrentEvent, m_nCurrX, m_nCurrY, ET_DIGOUTZOMBI);
      clEvent.m_nDir := m_btDir;
      EventMan.AddEvent(clEvent);
    end;
  end;
end;

{============================== THuSuABi =============================}

// Çã¼ö¾Æºñ

{--------------------------}

procedure THuSuABi.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  EffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:EffectSurface := g_WMonImages.Indexs[3].GetCachedGrayImage(DEATHFIREEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
      ceBright:EffectSurface := g_WMonImages.Indexs[3].GetCachedBrightImage(DEATHFIREEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
      else
        EffectSurface := g_WMonImages.Indexs[3].GetCachedImage(DEATHFIREEFFECTBASE + m_nCurrentFrame - m_nStartFrame, ax, ay);
    end;

end;

{============================== TGasKuDeGi =============================}

// ´ëÇü±¸µ¥±â (°¡½º½î´Â ±¸µ¥±â)

{--------------------------}

constructor TGasKuDeGi.Create;
begin
  inherited Create;
  AttackEffectSurface := nil;
  DieEffectSurface := nil;
  m_boUseEffect := FALSE;
  BoUseDieEffect := FALSE;
end;

procedure TGasKuDeGi.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
  DieEffectSurface := nil;
end;

procedure TGasKuDeGi.CalcActorFrame;
var
  pm:pTMonsterAction;
  Actor:TActor;
  scx, scy, stx, sty:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  // if (m_nOldCurrentAction = SM_HIT) or (m_nOldCurrentAction = SM_LIGHTING) then
  // m_boUseEffect := False;
  m_boUseEffect := False;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_HIT,
      SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        if m_btRace = 20 then
          m_nEffectEnd := m_nEndFrame + 1
        else
          m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        // 16¹æÇâÀÎ ¸¶¹ý ¼³Á¤
        Actor := PlayScene.FindActor(m_nTargetRecog);
        if Actor <> nil then begin
          PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, scx, scy);
          PlayScene.ScreenXYfromMCXY(Actor.m_nCurrX, Actor.m_nCurrY, stx, sty);
          fire16dir := GetFlyDirection16(scx, scy, stx, sty);
          // meff := TCharEffect.Create (ZOMBILIGHTINGEXPBASE, 12, actor);  //¸Â´Â »ç¶÷ È¿°ú
          // meff.ImgLib := g_WMonImages.Indexs[5];
          // meff.NextFrameTime := 50;
          // PlayScene.EffectList.Add (meff);
        end
        else
          fire16dir := firedir * 2;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        {
        if m_btRace = 40 then
           BoUseDieEffect := TRUE;
        }
        if (m_btRace = 40) or (m_btRace = 65) or (m_btRace = 66) or (m_btRace = 67) or (m_btRace = 68) or (m_btRace = 69) then
          BoUseDieEffect := True;
      end;
    SM_SKELETON:begin
        m_nStartFrame := pm.ActDeath.start;
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

function TGasKuDeGi.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    if m_boSkeleton then
      Result := pm.ActDeath.start
    else
      Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TGasKuDeGi.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  DieEffectSurface := nil;
  case m_btRace of
    // ¹¥»÷Ð§¹û
    16: {// ¶´Çù} begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedGrayImage(
                KUDEGIGASBASE - 1 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, // °¡½º´Â Ã³À½ ÇÑÇÁ·¹À½ ´Ê°Ô ½ÃÀÛÇÔ.
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedBrightImage(
                KUDEGIGASBASE - 1 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, // °¡½º´Â Ã³À½ ÇÑÇÁ·¹À½ ´Ê°Ô ½ÃÀÛÇÔ.
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedImage(
                KUDEGIGASBASE - 1 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, // °¡½º´Â Ã³À½ ÇÑÇÁ·¹À½ ´Ê°Ô ½ÃÀÛÇÔ.
                ax, ay);
          end;

      end;
    20: {// »ðÑæÎÖÂê} begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedGrayImage(
                COWMONFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedBrightImage(
                COWMONFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedImage(
                COWMONFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

      end;
    21: {// ÎÖÂê½ÌÖ÷} begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedGrayImage(
                COWMONLIGHTBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedBrightImage(
                COWMONLIGHTBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedImage(
                COWMONLIGHTBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

      end;
    24:begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[1].GetCachedGrayImage(
                SUPERIORGUARDBASE + (m_btDir * 8) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[1].GetCachedBrightImage(
                SUPERIORGUARDBASE + (m_btDir * 8) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[1].GetCachedImage(
                SUPERIORGUARDBASE + (m_btDir * 8) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

      end;

    40: {// ½©Ê¬1} begin
        if m_boUseEffect then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[5].GetCachedGrayImage(
                ZOMBILIGHTINGBASE + (fire16dir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[5].GetCachedBrightImage(
                ZOMBILIGHTINGBASE + (fire16dir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[5].GetCachedImage(
                ZOMBILIGHTINGBASE + (fire16dir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;
        end;

        if BoUseDieEffect then begin
          case m_ColorEffect of
            ceGrayScale:DieEffectSurface := g_WMonImages.Indexs[5].GetCachedGrayImage(
                ZOMBIDIEBASE + m_nCurrentFrame - m_nStartFrame, //
                bx, by);
            ceBright:DieEffectSurface := g_WMonImages.Indexs[5].GetCachedBrightImage(
                ZOMBIDIEBASE + m_nCurrentFrame - m_nStartFrame, //
                bx, by);
            else
              DieEffectSurface := g_WMonImages.Indexs[5].GetCachedImage(
                ZOMBIDIEBASE + m_nCurrentFrame - m_nStartFrame, //
                bx, by);
          end;
        end;
      end;
    52: {// Ð¨¶ê} begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedGrayImage(
                MOTHPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedBrightImage(
                MOTHPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[4].GetCachedImage(
                MOTHPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

      end;
    53: {// ·à³æ} begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedGrayImage(
                DUNGPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedBrightImage(
                DUNGPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[3].GetCachedImage(
                DUNGPOISONGASBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

      end;
    64:begin
        if m_boUseEffect then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                720 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                720 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                720 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;

        end;
      end;
    65:begin
        if BoUseDieEffect then begin
          case m_ColorEffect of
            ceGrayScale:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                350 + m_nCurrentFrame - m_nStartFrame, bx, by);
            ceBright:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                350 + m_nCurrentFrame - m_nStartFrame, bx, by);
            else
              DieEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                350 + m_nCurrentFrame - m_nStartFrame, bx, by);
          end;

        end;
      end;
    66:begin
        if BoUseDieEffect then begin
          case m_ColorEffect of
            ceGrayScale:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
            ceBright:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
            else
              DieEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
          end;

        end;
      end;
    67:begin
        if BoUseDieEffect then begin
          case m_ColorEffect of
            ceGrayScale:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                1160 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, bx, by);
            ceBright:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                1160 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, bx, by);
            else
              DieEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                1160 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, bx, by);
          end;

        end;
      end;
    68:begin
        if BoUseDieEffect then begin
          case m_ColorEffect of
            ceGrayScale:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
            ceBright:DieEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
            else
              DieEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                1600 + m_nCurrentFrame - m_nStartFrame, bx, by);
          end;
        end;
      end;

  end;
end;

procedure TGasKuDeGi.Run;
var
  prv, nEffectFrame:Integer;
  m_dwEffectFrameTimetime, m_dwFrameTimetime:longword;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    if m_boMsgMuch then
      m_dwEffectFrameTimetime := Round(m_dwEffectFrameTime * 2 / 3)
    else
      m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        BoUseDieEffect := False;
        ActionChanged;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nEffectFrame <> m_nEffectFrame) then
    boLoadSurface := True;

  if (nEffectFrame <> m_nEffectFrame) then
    ActionChanged;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

procedure TGasKuDeGi.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;
  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;
end;

procedure TGasKuDeGi.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;

  if BoUseDieEffect then
    if DieEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + bx + m_nShiftX,
        dy + by + m_nShiftY,
        DieEffectSurface);
    end;
end;

{-----------------------------------------------------------}

function TFireCowFaceMon.light:Integer;
var
  L:Integer;
begin
  L := m_nChrLight;
  if L < 2 then begin
    if m_boUseEffect then
      L := 2;
  end;
  Result := L;
end;

function TCowFaceKing.light:Integer;
var
  L:Integer;
begin
  L := m_nChrLight;
  if L < 2 then begin
    if m_boUseEffect then
      L := 2;
  end;
  Result := L;
end;
// ------------------------------------TMonEffect----------------------------------

procedure TMonEffect.CalcActorFrame;
var
  pm:pTMonsterAction;
  haircount:Integer;
  Actor:TActor;
  meff:TMagicEff;
  nX, nY:Integer;
  bofly:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;

    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;

        Shift(m_btDir, 0, 0, 1);

        if m_wAppearance = 262 then begin // Mon23-0
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;

          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else begin
          if m_nMagicNum > 0 then begin
            SetMagicSound(m_nMagicNum);
            PlayScene.NewMagic(Self,
              111,
              m_nEffectNum, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              m_MagicType, // EffectType
              True,
              0,
              bofly);
            if bofly then
              g_PlaySound.PlaySound(m_nMagicFireSound)
            else
              g_PlaySound.PlaySound(m_nMagicExplosionSound);
          end;
        end;
      end;

    SM_LIGHTING:begin
        if m_btRace = 101 then begin
          m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
          m_dwFrameTime := pm.ActAttack.ftime;
        end
        else begin
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
        end;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        if m_wAppearance = 220 then begin // Mon23-0
          if m_nMagicNum = 1 then begin
            m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
            m_dwFrameTime := pm.ActAttack.ftime;
          end;
        end
        else if m_wAppearance = 237 then begin // Ö©ÖëÍø
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(3730, 10, Actor);
            meff.ImgLib := g_WMonImages.Indexs[24];
            meff.TargetActor := nil;
            meff.NextFrameTime := 200;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 240) then begin // mon25-0
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 241) then begin // mon25-1
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 7;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 250) then begin // mon26-0
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 251) then begin // mon26-1
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 255) then begin // MON26-5
          if m_nMagicNum > 1 then begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
          end
          else begin
            m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
            m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
            m_dwFrameTime := pm.ActCritical.ftime;
          end;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 7;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
          //
        end
        else if (m_wAppearance = 321) then begin // Mon33-1
          PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
          meff := TThuderEffect.Create(780, nX, nY, nil); // target);
          meff.NewLevel := 0;
          meff.ExplosionFrame := 9;
          meff.ImgLib := g_WMonImages.Indexs[33];
          PlayScene.AddEffectList(meff);

          meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
          meff.MagExplosionBase := 790;
          meff.TargetActor := nil;
          meff.NextFrameTime := 80;
          meff.ExplosionFrame := 10;
          meff.ImgLib := g_WMonImages.Indexs[33];
          PlayScene.AddEffectList(meff);

          if m_nMagicNum > 1 then begin // ÓÄÁé¶ÜÐ§¹û
            // SetMagicSound(m_nMagicNum);
            PlayScene.NewMagic(Self,
              111,
              11, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              mtBujaukGroundEffect, // EffectType
              True,
              0,
              bofly);
            {if bofly then
              g_PlaySound.PlaySound(m_nMagicFireSound)
            else
              g_PlaySound.PlaySound(m_nMagicExplosionSound);  }
          end;
        end
        else if (m_wAppearance = 322) then begin // Mon33-1
          PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
          meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
          meff.MagExplosionBase := 1320;
          meff.TargetActor := nil;
          meff.NextFrameTime := 80;
          meff.ExplosionFrame := 30;
          meff.ImgLib := g_WMonImages.Indexs[33];
          PlayScene.AddEffectList(meff);

          if m_nMagicNum > 1 then begin // 12Õ½¼×ÊõÐ§¹û  ×çÖäÊõ
            // SetMagicSound(m_nMagicNum);
            PlayScene.NewMagic(Self,
              111,
              38, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              mtBujaukGroundEffect, // EffectType
              True,
              0,
              bofly);
            {if bofly then
              g_PlaySound.PlaySound(m_nMagicFireSound)
            else
              g_PlaySound.PlaySound(m_nMagicExplosionSound);  }

          end;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        if m_wAppearance = 223 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

          // mon23-3Ôö¼Ó¹¥»÷ÉùÒô chongchong 2014-11-15
          PlaySound(10100);
        end
        else if m_wAppearance = 232 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if m_wAppearance = 233 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if m_wAppearance = 233 then begin
          PlaySound(63);
        end
        else if m_wAppearance = 237 then begin
          // Mon24-7Ôö¼Ó¹¥»÷ÉùÒô chongchong 2014-11-21
          PlaySound(453);
        end
        else if (m_wAppearance = 255) then begin // MON26-5
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 7;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 320) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        // ÐÞ¸´ À×Ñ×ÖëÍõ Mon24-7  ÌØÐ§´íÎó
        //if (m_wAppearance = 222) or (m_wAppearance = 233) or (m_wAppearance = 237) then
        case m_wAppearance of
          222, 233, 237 {, 251}:begin
              m_boUseEffect := True;
              m_nEffectFrame := 0;
              m_nEffectStart := 0;
              m_nEffectEnd := m_nEffectStart + 10;
              m_dwEffectStartTime := TimeGetTime;
              m_dwEffectFrameTime := m_dwFrameTime;
            end;
        end;

      end;
  end;
end;

procedure TMonEffect.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMonEffect.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then begin
    if m_wAppearance = 222 then begin // Mom23-2 ËÀÍöÐ§¹û
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedGrayImage(
            1790 + m_nEffectFrame, //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedBrightImage(
            1790 + m_nEffectFrame, //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedImage(
            1790 + m_nEffectFrame, //
            ax, ay);
      end;
    end
    else if m_wAppearance = 223 then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedGrayImage(
            m_nBodyOffset + 420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedBrightImage(
            m_nBodyOffset + 420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedImage(
            m_nBodyOffset + 420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 232 then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedGrayImage(
            1100 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedBrightImage(
            1100 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedImage(
            1100 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 233 then begin
      if m_boDeath then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedGrayImage(
              1760 + m_nEffectFrame, //
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedBrightImage(
              1760 + m_nEffectFrame, //
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedImage(
              1760 + m_nEffectFrame, //
              ax, ay);
        end;
      end
      else begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedGrayImage(
              1680 + firedir * 10 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedBrightImage(
              1680 + firedir * 10 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedImage(
              1680 + firedir * 10 + m_nEffectFrame,
              ax, ay);
        end;
      end;
    end
      // À×Ñ×ÖëÍõ Mon24-7 ËÀÍöÌØÐ§ piaoyun 2013-11-12
    else if (m_wAppearance = 237) and (m_boDeath) then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedGrayImage(
            3710 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedBrightImage(
            3710 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedImage(
            3710 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 240 then begin // mon25-0
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedGrayImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedBrightImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 241 then begin // mon25-1
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedGrayImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedBrightImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[25].GetCachedImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 250 then begin // mon26-0
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
            420 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if (m_wAppearance = 251) then begin // mon26-1
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
            930 + firedir * 10 + m_nEffectFrame,
            ax, ay);
      end;
    end
    else if m_wAppearance = 255 then begin // MON26-5
      if m_nCurrentAction = SM_LIGHTING then begin
        if m_nMagicNum > 1 then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
                2650 + firedir * 10 + m_nEffectFrame,
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
                2650 + firedir * 10 + m_nEffectFrame,
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
                2650 + firedir * 10 + m_nEffectFrame,
                ax, ay);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
                2570 + firedir * 10 + m_nEffectFrame,
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
                2570 + firedir * 10 + m_nEffectFrame,
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
                2570 + firedir * 10 + m_nEffectFrame,
                ax, ay);
          end;
        end;
      end;
    end
      ////////////////////////////////////////////////////////////////////////////
    else if (m_wAppearance = 262) then {// Mon27-2 piaoyun 2013-11-15} begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
            1182 + m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
            1182 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
            1182 + m_nEffectFrame, ax, ay);
      end;
    end
      ////////////////////////////////////////////////////////////////////////////
    else if (m_wAppearance = 320) then begin // Mon33-0
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(350 + firedir * 10 + m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(350 + firedir * 10 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(350 + firedir * 10 + m_nEffectFrame, ax, ay);
      end;
    end
    else if (m_wAppearance = 322) then begin // Mon33-1

    end;
  end;
end;

function TMonEffect.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TMonEffect.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TMonEffect.Run;
var
  m_dwEffectFrameTimetime:LongWord;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;
  inherited Run;
end;
{----------------------------TMon26_6-------------------------------------------}

procedure TMon26_6.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:THeroShowEffect;
  Actor:TActor;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        m_nEffectStart := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEffectEnd := m_nStartFrame + pm.ActStand.frame - 1;
        m_nEffectFrame := m_nEffectStart;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := pm.ActStand.ftime;
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;

        m_boUseEffect := True;
        m_nEffectStart := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEffectEnd := m_nStartFrame + pm.ActWalk.frame - 1;
        m_nEffectFrame := m_nEffectStart;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := pm.ActWalk.ftime;
      end;
    SM_LIGHTING:begin
        m_boUseEffect := True;
        firedir := m_btDir;
        if m_nMagicNum = 0 then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;

          m_nEffectFrame := 0; // startframe;
          m_nEffectStart := 0; // startframe;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            if m_wAppearance = 256 then begin
              meff := THeroShowEffect.Create(3750, 12, Actor);
              meff.ImgLib := g_WMonImages.Indexs[26];
              meff.NextFrameTime := 200;
              PlayScene.AddEffectList(meff);
            end
            else if m_wAppearance = 266 then begin
              meff := THeroShowEffect.Create(3550, 12, Actor);
              meff.ImgLib := g_WMonImages.Indexs[27];
              meff.NextFrameTime := 200;
              PlayScene.AddEffectList(meff);
            end
            else if m_wAppearance = 267 then begin
              meff := THeroShowEffect.Create(4580, 12, Actor);
              meff.ImgLib := g_WMonImages.Indexs[27];
              meff.NextFrameTime := 200;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end
        else begin
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;

          m_nEffectFrame := 0; // startframe;
          m_nEffectStart := 0; // startframe;
          m_nEffectEnd := m_nEffectStart + 8;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        m_nEffectStart := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEffectEnd := m_nStartFrame + pm.ActAttack.frame - 1;
        m_nEffectFrame := m_nEffectStart;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := pm.ActAttack.ftime;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TMon26_6.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMon26_6.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then begin
    if m_nCurrentAction = SM_LIGHTING then begin
      if m_wAppearance = 256 then begin
        if m_nMagicNum = 0 then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
                3580 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
                3580 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
                3580 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
                3670 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
                3670 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
                3670 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end;
      end
      else if m_wAppearance = 266 then begin
        if m_nMagicNum = 0 then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
                3380 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
                3380 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
                3380 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
                3470 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
                3470 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
                3470 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end;
      end
      else if m_wAppearance = 267 then begin
        if m_nMagicNum = 0 then begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
                4410 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
                4410 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
                4410 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
                4500 + firedir * 10 + m_nEffectFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
                4500 + firedir * 10 + m_nEffectFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
                4500 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        end;
      end;
    end
    else begin
      if m_wAppearance = 256 then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedGrayImage(
              3240 + m_nEffectFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedBrightImage(
              3240 + m_nEffectFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[26].GetCachedImage(
              3240 + m_nEffectFrame, ax, ay);
        end;
      end
      else if m_wAppearance = 266 then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
              3040 + m_nEffectFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
              3040 + m_nEffectFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
              3040 + m_nEffectFrame, ax, ay);
        end;
      end
      else if m_wAppearance = 267 then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
              4070 + m_nEffectFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
              4070 + m_nEffectFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
              4070 + m_nEffectFrame, ax, ay);
        end;
      end;
    end;
  end;
end;

function TMon26_6.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TMon26_6.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TMon26_6.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;

// ------------------------------------TMon23_1----------------------------------

procedure TMon23_1.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin
          m_nStartFrame := (850 - m_nBodyOffset) + m_btDir * 10;
          m_nEndFrame := m_nStartFrame;
          m_dwFrameTime := 100;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := 6;
          m_boStruckShowNumber := False;

          m_boShowBigHPProgress := False;
          m_boSendQueryBigHPProgress := False;
        end
        else begin
          m_boUseEffect := True;
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
          m_dwFrameTime := pm.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActStand.frame;
          Shift(m_btDir, 0, 0, 1);
        end;
      end;
    SM_DIGUP: {// °È±â ¾øÀ½, SM_DIGUP, ¹æÇâ ¾øÀ½.} begin
        m_nStartFrame := (850 - m_nBodyOffset) + m_btDir * 10;
        m_nEndFrame := m_nStartFrame + 6 - 1;
        m_dwFrameTime := 100;
        m_dwStartTime := TimeGetTime;
        m_nState := 0;
        // WarMode := FALSE;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_HIT:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TMon23_1.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMon23_1.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedGrayImage(
          m_nBodyOffset + 510 + m_nCurrentFrame,
          ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedBrightImage(
          m_nBodyOffset + 510 + m_nCurrentFrame,
          ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[23].GetCachedImage(
          m_nBodyOffset + 510 + m_nCurrentFrame,
          ax, ay);
    end;
end;

function TMon23_1.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    if (m_nState and STATE_STONE_MODE) <> 0 then begin
      Result := (850 - m_nBodyOffset) + m_btDir * 10
    end
    else begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;
end;

procedure TMon23_1.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TMon23_1.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;

procedure TMon24_4.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := FALSE;

  case m_nCurrentAction of
    SM_TURN:begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin
          m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
          m_nEndFrame := m_nStartFrame;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActDeath.frame;
          m_boStruckShowNumber := False;

          m_boShowBigHPProgress := False;
          m_boSendQueryBigHPProgress := False;
        end
        else begin
          m_boUseEffect := True;
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
          m_dwFrameTime := pm.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActStand.frame;

          firedir := m_btDir;
          m_nEffectStart := 2270 + pm.ActStand.start + m_btDir * 10;
          m_nEffectFrame := m_nEffectStart;
          m_nEffectEnd := m_nEffectStart + pm.ActStand.frame;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;

        firedir := m_btDir;
        m_nEffectStart := 2270 + pm.ActWalk.start + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + pm.ActWalk.frame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_DIGUP: {// °È±â ¾øÀ½, SM_DIGUP, ¹æÇâ ¾øÀ½.} begin

        m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := FALSE;

        // ÐÞ¸´ Mon24_4 ÌØÐ§´íÎó piaoyun 2013-11-12
        {
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectStart := 2270 + pm.ActDeath.start + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + pm.ActDeath.frame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        Shift(m_btDir, 0, 0, 1);
        }
      end;
    SM_HIT:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;

        firedir := m_btDir;
        m_nEffectStart := 2270 + pm.ActAttack.start + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + pm.ActAttack.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TMon24_4.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMon24_4.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedGrayImage(
          m_nEffectFrame, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedBrightImage(
          m_nEffectFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[24].GetCachedImage(
          m_nEffectFrame, ax, ay);
    end;
end;

function TMon24_4.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    if (m_nState and STATE_STONE_MODE) <> 0 then begin
      Result := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
    end
    else begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;
end;

procedure TMon24_4.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, AttackEffectSurface);
    end;
end;

procedure TMon24_4.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;
{-----------------------------------------------------------}

// procedure TZombiLighting.Run;

{-----------------------------------------------------------}

procedure TSculptureMon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := FALSE;

  case m_nCurrentAction of
    SM_TURN:begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin
          if (m_btRace = 48) or (m_btRace = 49) then
            m_nStartFrame := pm.ActDeath.start // + Dir * (pm.ActDeath.frame + pm.ActDeath.skip)
          else
            m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
          m_nEndFrame := m_nStartFrame;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActDeath.frame;
          m_boStruckShowNumber := False;

          m_boShowBigHPProgress := False;
          m_boSendQueryBigHPProgress := False;
        end
        else begin
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
          m_dwFrameTime := pm.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActStand.frame;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
    // ×æÂê½ÌÖ÷½â³ýÊ¯»¯×´Ì¬ piaoyun 2013-11-20
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_DIGUP: {// °È±â ¾øÀ½, SM_DIGUP, ¹æÇâ ¾øÀ½.} begin
        if (m_btRace = 48) or (m_btRace = 49) then begin
          m_nStartFrame := pm.ActDeath.start;
        end
        else begin
          m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        end;
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
        m_nState := 0;
        // WarMode := FALSE;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        if m_btRace = 49 then begin
          m_boUseEffect := True;
          firedir := m_btDir;
          m_nEffectFrame := 0; // startframe;
          m_nEffectStart := 0; // startframe;
          m_nEffectEnd := m_nEffectStart + 8;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TSculptureMon.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TSculptureMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  case m_btRace of
    48, 49:begin
        if m_boUseEffect then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[7].GetCachedGrayImage(
                SCULPTUREFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[7].GetCachedBrightImage(
                SCULPTUREFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[7].GetCachedImage(
                SCULPTUREFIREBASE + (firedir * 10) + m_nEffectFrame - m_nEffectStart, //
                ax, ay);
          end;
      end;
  end;
end;

function TSculptureMon.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    if (m_nState and STATE_STONE_MODE) <> 0 then begin
      case m_btRace of
        47:Result := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        48, 49:Result := pm.ActDeath.start;
      end;
    end
    else begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;
end;

procedure TSculptureMon.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TSculptureMon.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;

{ TBanyaGuardMon }

function TBanyaGuardMon.GetDefaultFrame(wmode:Boolean):Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := inherited GetDefaultFrame(wmode);
end;

procedure TBanyaGuardMon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_boUseEffect := False;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  case m_nCurrentAction of
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_LIGHTING:begin
        // ÐÞ¸ÄÄ§Áú½ÌÖ÷Ö§³Ö»ØÑª¹¦ÄÜ chongchong 2014-11-14
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);

        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        m_nCurEffFrame := 0;
        // m_boUseMagic := True;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        if (m_btRace = 71) then begin
          m_boUseEffect := True;
          m_nEffectFrame := m_nStartFrame;
          m_nEffectStart := m_nStartFrame;
          m_nEffectEnd := m_nEndFrame;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
      end;
    SM_DIGUP:begin
        // Ä§Áú½ÌÖ÷²»ÒªÊ¯»¯ËÕÐÑ×´Ì¬ chongchong 2014-11-14
        {if m_wAppearance = 218 then
        begin
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame;
        end
        else
        }
        inherited;
      end;
    else
      {
      if m_nCurrentAction = SM_STRUCK then
      begin
        DScreen.AddChatBoardString('ÊÜµ½¹¥»÷', $0000FF, 0);
      end;
      }
      inherited;
  end;

end;

constructor TBanyaGuardMon.Create;
begin
  inherited;
  n26C := nil;
end;

procedure TBanyaGuardMon.DrawEff(dx, dy:Integer);
begin
  inherited;
  if m_boUseEffect and (n26C <> nil) then begin
    GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, n26C);
  end;
end;

procedure TBanyaGuardMon.Finalize;
begin
  inherited Finalize;
  n26C := nil;
end;

procedure TBanyaGuardMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  n26C := nil;
  if bo260 then begin
    case m_btRace of
      70:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedGrayImage(
                2320 + m_nCurrentFrame - m_nStartFrame, n264, n268);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedBrightImage(
                2320 + m_nCurrentFrame - m_nStartFrame, n264, n268);
            else
              AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedImage(
                2320 + m_nCurrentFrame - m_nStartFrame, n264, n268);
          end;

        end;
      71:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedGrayImage(
                2870 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, n264, n268);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedBrightImage(
                2870 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, n264, n268);
            else
              AttackEffectSurface := g_WMonImages.Indexs[21].GetCachedImage(
                2870 + (m_btDir * 10) + m_nCurrentFrame - m_nStartFrame, n264, n268);
          end;
        end;
      78:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedGrayImage(
                3120 + (m_btDir * 20) + m_nCurrentFrame - m_nStartFrame, n264, n268);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedBrightImage(
                3120 + (m_btDir * 20) + m_nCurrentFrame - m_nStartFrame, n264, n268);
            else
              AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedImage(
                3120 + (m_btDir * 20) + m_nCurrentFrame - m_nStartFrame, n264, n268);
          end;
        end;
    end;
  end
  else begin
    if m_boUseEffect then begin
      case m_btRace of
        70:begin
            if m_nCurrentAction = SM_HIT then begin
              case m_ColorEffect of
                ceGrayScale:n26C := g_WMonImages.Indexs[21].GetCachedGrayImage(
                    2230 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
                ceBright:n26C := g_WMonImages.Indexs[21].GetCachedBrightImage(
                    2230 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
                else
                  n26C := g_WMonImages.Indexs[21].GetCachedImage(
                    2230 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
              end;
            end;
          end;
        71:begin
            case m_nCurrentAction of
              SM_HIT:begin

                  case m_ColorEffect of
                    ceGrayScale:
                      n26C := g_WMonImages.Indexs[21].GetCachedGrayImage(
                        2780 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                    ceBright:
                      n26C := g_WMonImages.Indexs[21].GetCachedBrightImage(
                        2780 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[21].GetCachedImage(
                        2780 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                  end;

                end;
              SM_FLYAXE..SM_LIGHTING:begin
                  case m_ColorEffect of
                    ceGrayScale:
                      n26C := g_WMonImages.Indexs[21].GetCachedGrayImage(
                        2960 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                    ceBright:
                      n26C := g_WMonImages.Indexs[21].GetCachedBrightImage(
                        2960 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[21].GetCachedImage(
                        2960 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                        ax, ay);
                  end;

                end;
            end;
          end;
        72:begin
            if m_nCurrentAction = SM_HIT then begin
              case m_ColorEffect of
                ceGrayScale:n26C := g_WMonImages.Indexs[21].GetCachedGrayImage(
                    3490 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
                ceBright:n26C := g_WMonImages.Indexs[21].GetCachedBrightImage(
                    3490 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
                else
                  n26C := g_WMonImages.Indexs[21].GetCachedImage(
                    3490 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
              end;

            end;
          end;
        78:begin
            if m_nCurrentAction = SM_HIT then begin
              case m_ColorEffect of
                ceGrayScale:n26C := g_WMonImages.Indexs[22].GetCachedGrayImage(
                    3440 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
                ceBright:n26C := g_WMonImages.Indexs[22].GetCachedBrightImage(
                    3440 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
                else
                  n26C := g_WMonImages.Indexs[22].GetCachedImage(
                    3440 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart,
                    ax, ay);
              end;

            end;
          end;
      end;
    end;
  end;
end;

procedure TBanyaGuardMon.Run;
var
  prv, nEffectFrame:Integer;
  m_dwEffectFrameTimetime, m_dwFrameTimetime:longword;
  bo11:Boolean;
  boLoadSurface:Boolean;

  target:TActor;
  scx, scy, sctx, scty:Integer;
  meff:TMagicEff;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    if m_boMsgMuch then
      m_dwEffectFrameTimetime := Round(m_dwEffectFrameTime * 2 / 3)
    else
      m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        bo260 := FALSE;
      end;
      if m_nCurrentAction = SM_LIGHTING then begin
        if (m_nCurrentFrame - m_nStartFrame) = 4 then begin
          if (m_btRace = 70) or (m_btRace = 81) then begin
            PlayScene.NewMagic(Self, m_nMagicNum, 8, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, m_nTargetRecog, mtThunder, FALSE, 30, bo11);
            g_PlaySound.PlaySound(10112);
          end;
          if (m_btRace = 71) then begin

            // ÐÞÕýÅ£Ä§¼ÀË¾»ðÇò¹¥»÷£¬È±ÉÙÄ¿±êÐ§¹û chongchong 2014-11-14
            //PlayScene.NewMagic(Self, 1, 1, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, m_nTargetRecog, mtFly, True, 30, bo11);

            target := PlayScene.FindActor(m_nTargetRecog);
            PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, scx, scy);
            PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, sctx, scty);
            meff := TMagicEff.Create(1, 1, scx, scy, sctx, scty, mtFly, True, 30);
            meff.ImgLib := g_WMagicImages;
            meff.start := 0;
            meff.frame := 10;
            meff.MagExplosionBase := 170;
            meff.ExplosionFrame := 10;
            meff.TargetActor := target;
            PlayScene.AddEffectList(meff);

            g_PlaySound.PlaySound(10012);
          end;
          if (m_btRace = 72) then begin
            PlayScene.NewMagic(Self, 11, 32, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, m_nTargetRecog, mt13, FALSE, 30, bo11);
            g_PlaySound.PlaySound(2276);
          end;
          if (m_btRace = 78) then begin
            PlayScene.NewMagic(Self, 11, 37, m_nCurrX, m_nCurrY, m_nCurrX, m_nCurrY, m_nRecogId, mt13, FALSE, 30, bo11);
            g_PlaySound.PlaySound(2396);
          end;
        end;
      end;
      m_nCurrentDefFrame := 0;
      m_dwDefFrameTime := TimeGetTime;
    end;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nEffectFrame <> m_nEffectFrame) then
    boLoadSurface := True;
  if (nEffectFrame <> m_nEffectFrame) then
    ActionChanged;
  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TElectronicScolpionMon }

procedure TElectronicScolpionMon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  case m_nCurrentAction of
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    else begin
        inherited;
      end;
  end;
end;

procedure TElectronicScolpionMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if (m_btRace = 60) and m_boUseEffect and (m_nCurrentAction = SM_LIGHTING) then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedGrayImage(
          430 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedBrightImage(
          430 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedImage(
          430 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
    end;
  end;
end;

{ TBossPigMon }

procedure TBossPigMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if (m_btRace = 61) and m_boUseEffect then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedGrayImage(
          860 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedBrightImage(
          860 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedImage(
          860 + (firedir * 10) + m_nEffectFrame - m_nEffectStart,
          ax, ay);
    end;

  end;
end;

{ TKingOfSculpureKingMon }

procedure TKingOfSculpureKingMon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  case m_nCurrentAction of
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        m_nEffectFrame := pm.ActDie.start;
        m_nEffectStart := pm.ActDie.start;
        m_nEffectEnd := pm.ActDie.start + pm.ActDie.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        m_boUseEffect := True;
      end;
    else
      inherited;
  end;
end;

procedure TKingOfSculpureKingMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if (m_btRace = 62) and m_boUseEffect then begin
    case m_nCurrentAction of
      SM_HIT:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedGrayImage(
                1490 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedBrightImage(
                1490 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedImage(
                1490 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_LIGHTING:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedGrayImage(
                1380 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedBrightImage(
                1380 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedImage(
                1380 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_NOWDEATH:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedGrayImage(
                1470 + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedBrightImage(
                1470 + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[19].GetCachedImage(
                1470 + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;
        end;
    end;
  end;
end;

{ TSkeletonArcherMon }

procedure TSkeletonArcherMon.CalcActorFrame;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited;
  if (m_nCurrentAction = SM_NOWDEATH) and (m_btRace <> 72) then begin
    bo260 := True;
  end;
end;

procedure TSkeletonArcherMon.DrawEff(dx, dy:Integer);
begin
  inherited;
  if bo260 and (AttackEffectSurface <> nil) then begin
    GameCanvas.DrawBlend(dx + n264 + m_nShiftX, dy + n268 + m_nShiftY, AttackEffectSurface);
  end;
end;

procedure TSkeletonArcherMon.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TSkeletonArcherMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if bo260 then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
          1600 + m_nEffectFrame - m_nEffectStart, n264, n268);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
          1600 + m_nEffectFrame - m_nEffectStart, n264, n268);
      else
        AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
          1600 + m_nEffectFrame - m_nEffectStart, n264, n268);
    end;

  end;
end;

procedure TSkeletonArcherMon.Run;
var
  m_dwFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boMsgMuch then
    m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
  else
    m_dwFrameTimetime := m_dwFrameTime;
  if m_nCurrentAction <> 0 then begin
    if (TimeGetTime - m_dwStartTime) > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
      end
      else begin
        m_nCurrentAction := 0;
        bo260 := FALSE;
        ActionChanged;
      end;
    end;
  end;

  inherited;
end;

{ TFlyingSpider }

procedure TFlyingSpider.CalcActorFrame;
var
  Eff8:TNormalDrawEffect;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited;
  if m_nCurrentAction = SM_NOWDEATH then begin
    Eff8 := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMonImages.Indexs[12], 1420, 20, m_dwFrameTime, True);
    if Eff8 <> nil then begin
      Eff8.MagOwner := g_MySelf;
      PlayScene.AddEffectList(Eff8);
    end;
  end;
end;

{ TExplosionSpider }

procedure TExplosionSpider.CalcActorFrame;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited;
  case m_nCurrentAction of
    SM_HIT:begin
        m_boUseEffect := FALSE;
      end;
    SM_NOWDEATH:begin
        m_nEffectStart := m_nStartFrame;
        m_nEffectFrame := m_nStartFrame;
        m_dwEffectStartTime := TimeGetTime();
        m_dwEffectFrameTime := m_dwFrameTime;
        m_nEffectEnd := m_nEndFrame;
        m_boUseEffect := True;
      end;
  end;
end;

procedure TExplosionSpider.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[14].GetCachedGrayImage(
          730 + m_nEffectFrame - m_nEffectStart, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[14].GetCachedBrightImage(
          730 + m_nEffectFrame - m_nEffectStart, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[14].GetCachedImage(
          730 + m_nEffectFrame - m_nEffectStart, ax, ay);
    end;

end;

{ TSkeletonKingMon }

procedure TSkeletonKingMon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  case m_nCurrentAction of
    SM_BACKSTEP, SM_WALK:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nEffectFrame := pm.ActWalk.start;
        m_nEffectStart := pm.ActWalk.start;
        m_nEffectEnd := pm.ActWalk.start + pm.ActWalk.frame - 1;
        m_dwEffectStartTime := TimeGetTime();
        m_dwEffectFrameTime := m_dwFrameTime;
        m_boUseEffect := True;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;

      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime();
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_FLYAXE:begin
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime();
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_LIGHTING:begin
        m_nStartFrame := 80 + pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime();
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; //pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        m_nEffectFrame := pm.ActStruck.start;
        m_nEffectStart := pm.ActStruck.start;
        m_nEffectEnd := pm.ActStruck.start + pm.ActStruck.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        m_boUseEffect := True;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        m_nEffectFrame := pm.ActDie.start;
        m_nEffectStart := pm.ActDie.start;
        m_nEffectEnd := pm.ActDie.start + pm.ActDie.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        m_boUseEffect := True;
      end;
    else begin
        inherited;
      end;
  end;
end;

procedure TSkeletonKingMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  if (m_btRace = 63) and m_boUseEffect then begin
    case m_nCurrentAction of
      SM_WALK:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3060 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3060 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3060 + (m_btDir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_HIT:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3140 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3140 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3140 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_FLYAXE:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3300 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3300 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3300 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_LIGHTING:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3220 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3220 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3220 + (firedir * 10) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
      SM_STRUCK:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3380 + (m_btDir * 2) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3380 + (m_btDir * 2) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3380 + (m_btDir * 2) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;
        end;
      SM_NOWDEATH:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedGrayImage(
                3400 + (m_btDir * 4) + m_nEffectFrame - m_nEffectStart, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedBrightImage(
                3400 + (m_btDir * 4) + m_nEffectFrame - m_nEffectStart, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[20].GetCachedImage(
                3400 + (m_btDir * 4) + m_nEffectFrame - m_nEffectStart, ax, ay);
          end;

        end;
    end;
  end;
end;

procedure TSkeletonKingMon.Run;
var
  prv, nEffectFrame:Integer;
  m_dwEffectFrameTimetime, m_dwFrameTimetime:longword;
  meff:TFlyingFireBall;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    if m_boMsgMuch then
      m_dwEffectFrameTimetime := Round(m_dwEffectFrameTime * 2 / 3)
    else
      m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        BoUseDieEffect := FALSE;
        ActionChanged;
      end;

      if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = 4) then begin
        meff := TFlyingFireBall(PlayScene.NewFlyObject(Self,
          m_nCurrX,
          m_nCurrY,
          m_nTargetX,
          m_nTargetY,
          m_nTargetRecog,
          mt12));
        if meff <> nil then begin
          meff.ImgLib := g_WMonImages.Indexs[20];
          meff.NextFrameTime := 40;
          meff.FlyImageBase := 3573;
        end;
      end;
      m_nCurrentDefFrame := 0;
      m_dwDefFrameTime := TimeGetTime;
    end;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nEffectFrame <> m_nEffectFrame) then
    boLoadSurface := True;
  if (nEffectFrame <> m_nEffectFrame) then
    ActionChanged;
  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TStoneMonster }

procedure TStoneMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
  bofly:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_boUseMagic := FALSE;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_btDir := 0;
  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        if not m_boUseEffect then begin
          m_boUseEffect := True;
          m_nEffectFrame := m_nStartFrame;
          m_nEffectStart := m_nStartFrame;
          m_nEffectEnd := m_nEndFrame;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 300;
        end;
      end;

    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        if not m_boUseEffect then begin
          m_boUseEffect := True;
          m_nEffectFrame := m_nStartFrame;
          m_nEffectStart := m_nStartFrame;
          m_nEffectEnd := m_nStartFrame + 25;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 150;
        end;
        if m_nMagicNum > 0 then begin
          SetMagicSound(m_nMagicNum);
          PlayScene.NewMagic(Self,
            111,
            m_nEffectNum, // Effect
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            m_MagicType, // EffectType
            True,
            0,
            bofly);
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        if not m_boUseEffect then begin
          m_boUseEffect := True;
          m_nEffectFrame := m_nStartFrame;
          m_nEffectStart := m_nStartFrame;
          m_nEffectEnd := m_nStartFrame + 25;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 150;
        end;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start;
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; //  pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        bo260 := True;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nStartFrame + 19;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := 80;
      end;
  end;
end;

constructor TStoneMonster.Create;
begin
  inherited;
  n26C := nil;
  m_boUseEffect := FALSE;
  bo260 := FALSE;
end;

procedure TStoneMonster.Finalize;
begin
  inherited Finalize;
  n26C := nil;
end;

procedure TStoneMonster.DrawEff(dx, dy:Integer);
begin
  inherited;
  if m_boUseEffect and (n26C <> nil) then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      n26C);
  end;
end;

procedure TStoneMonster.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;
  AttackEffectSurface := nil;
  n26C := nil;
  if bo260 then begin
    case m_btRace of
      75:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedGrayImage(
                2530 + m_nEffectFrame - m_nEffectStart, n264, n268);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedBrightImage(
                2530 + m_nEffectFrame - m_nEffectStart, n264, n268);
            else
              AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedImage(
                2530 + m_nEffectFrame - m_nEffectStart, n264, n268);
          end;

        end;
      77:begin
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedGrayImage(
                2660 + m_nEffectFrame - m_nEffectStart, n264, n268);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedBrightImage(
                2660 + m_nEffectFrame - m_nEffectStart, n264, n268);
            else
              AttackEffectSurface := g_WMonImages.Indexs[22].GetCachedImage(
                2660 + m_nEffectFrame - m_nEffectStart, n264, n268);
          end;

        end;
    end;
  end
  else begin
    if m_boUseEffect then
      case m_btRace of
        75:begin
            case m_nCurrentAction of
              SM_HIT:begin
                  case m_ColorEffect of
                    ceGrayScale:n26C := g_WMonImages.Indexs[22].GetCachedGrayImage(
                        2500 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    ceBright:n26C := g_WMonImages.Indexs[22].GetCachedBrightImage(
                        2500 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[22].GetCachedImage(
                        2500 + m_nEffectFrame - m_nEffectStart, ax, ay);
                  end;

                end;
              SM_TURN:begin
                  case m_ColorEffect of
                    ceGrayScale:n26C := g_WMonImages.Indexs[22].GetCachedGrayImage(
                        2490 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    ceBright:n26C := g_WMonImages.Indexs[22].GetCachedBrightImage(
                        2490 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[22].GetCachedImage(
                        2490 + m_nEffectFrame - m_nEffectStart, ax, ay);
                  end;

                end;
            end;
          end;
        77:begin
            case m_nCurrentAction of
              SM_HIT:begin
                  case m_ColorEffect of
                    ceGrayScale:n26C := g_WMonImages.Indexs[22].GetCachedGrayImage(
                        2630 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    ceBright:n26C := g_WMonImages.Indexs[22].GetCachedBrightImage(
                        2630 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[22].GetCachedImage(
                        2630 + m_nEffectFrame - m_nEffectStart, ax, ay);
                  end;
                end;
              SM_TURN:begin
                  case m_ColorEffect of
                    ceGrayScale:n26C := g_WMonImages.Indexs[22].GetCachedGrayImage(
                        2620 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    ceBright:n26C := g_WMonImages.Indexs[22].GetCachedBrightImage(
                        2620 + m_nEffectFrame - m_nEffectStart, ax, ay);
                    else
                      n26C := g_WMonImages.Indexs[22].GetCachedImage(
                        2620 + m_nEffectFrame - m_nEffectStart, ax, ay);
                  end;

                end;
            end;
          end;
      end;
  end;
end;

procedure TStoneMonster.Run;
var
  prv, nEffectFrame:Integer;
  m_dwEffectFrameTimetime:longword;
  m_dwFrameTimetime:longword;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or
    (m_nCurrentAction = SM_BACKSTEP) or
    (m_nCurrentAction = SM_RUN) or
    (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect or bo260 then begin
    if m_boMsgMuch then
      m_dwEffectFrameTimetime := Round(m_dwEffectFrameTime * 2 / 3)
    else
      m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
        bo260 := FALSE;
      end;
    end;
  end;

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        ActionChanged;
      end;
      m_nCurrentDefFrame := 0;
      m_dwDefFrameTime := TimeGetTime;
    end;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nEffectFrame <> m_nEffectFrame) then
    boLoadSurface := True;

  if (nEffectFrame <> m_nEffectFrame) then
    ActionChanged;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TAngel }

procedure TAngel.DrawChr(dx, dy:Integer;
  blend, boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
end;

procedure TAngel.Finalize;
begin
  inherited Finalize;
  n278 := nil;
end;

procedure TAngel.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  // LoadNameSurface;

  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  n278 := nil;

  mimg := g_WMonImages.Images[m_wAppearance];
  if mimg <> nil then begin
    if (not m_boReverseFrame) then begin
      case m_ColorEffect of
        ceGrayScale:begin
            m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
            n278 := mimg.GetCachedGrayImage(1280 + m_nCurrentFrame, n270, n274);
          end;
        ceBright:begin
            m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
            n278 := mimg.GetCachedBrightImage(1280 + m_nCurrentFrame, n270, n274);
          end;
        else begin
            m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
            n278 := mimg.GetCachedImage(1280 + m_nCurrentFrame, n270, n274);
          end;
      end;

    end
    else begin
      case m_ColorEffect of
        ceGrayScale:begin
            m_BodySurface := mimg.GetCachedGrayImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            n278 := mimg.GetCachedGrayImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              n270, n274);
          end;
        ceBright:begin
            m_BodySurface := mimg.GetCachedBrightImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            n278 := mimg.GetCachedBrightImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              n270, n274);
          end;
        else begin
            m_BodySurface := mimg.GetCachedImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              m_nPx, m_nPy);
            n278 := mimg.GetCachedImage(
              GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
              n270, n274);
          end;
      end;

    end;

    {if (m_BodySurface <> nil) and (m_BodySurface.Format in [apf_DXT1..apf_DXT5]) then begin

      if (not m_boReverseFrame) then begin
        m_BodyAlphaSurface := mimg.Alphas[GetOffset(m_wAppearance) + m_nCurrentFrame];
      end else begin
        m_BodyAlphaSurface := mimg.Alphas[
          GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame)];
      end;

    end else m_BodyAlphaSurface := nil;}

  end;

  LoadActorIcons;

end;

{ TPBOMA6Mon }

procedure TPBOMA6Mon.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;
  meff:TFlyingAxe;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;

  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN)
    or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  prv := m_nCurrentFrame; //HZQ 20230520 ²Â²â¿ÉÄÜÊÇÒª±¸·ÝÉÏÒ»Ö¡

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        ActionChanged;
      end;

      if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = 4) then begin
        meff := TFlyingAxe(PlayScene.NewFlyObject(Self,
          m_nCurrX,
          m_nCurrY,
          g_nTargetX,
          g_nTargetY,
          m_nTargetRecog,
          mt16));
        if meff <> nil then begin
          meff.ImgLib := g_WMonImages.Indexs[22];
          meff.NextFrameTime := 50;
          meff.FlyImageBase := 1989;
        end;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if prv <> m_nCurrentFrame then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TDragonStatue }

procedure TDragonStatue.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_btDir := 0;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  case m_nCurrentAction of
    SM_DIGUP:begin
        Shift(0, 0, 0, 1);
        m_nStartFrame := 0;
        m_nEndFrame := 9;
        m_dwFrameTime := 100;
        m_dwStartTime := TimeGetTime;
      end;
    SM_LIGHTING:begin
        m_nStartFrame := 0;
        m_nEndFrame := 9;
        m_dwFrameTime := 100;
        m_dwStartTime := TimeGetTime;
        m_boUseEffect := True;
        m_nEffectStart := 0;
        m_nEffectFrame := 0;
        m_nEffectEnd := 9;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := 100;
      end;
  end;
end;

constructor TDragonStatue.Create;
begin
  inherited;
  n26C := nil;
end;

procedure TDragonStatue.Finalize;
begin
  inherited Finalize;
  n26C := nil;
end;

procedure TDragonStatue.DrawEff(dx, dy:Integer);
begin
  inherited;
  if m_boUseEffect and (EffectSurface <> nil) then begin
    GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, EffectSurface);
  end;
end;

procedure TDragonStatue.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  // LoadNameSurface;

  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  EffectSurface := nil;

  mimg := g_WDragonImg;
  if mimg <> nil then begin
    case m_ColorEffect of
      ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
      ceBright:m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
      else
        m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
    end;

    {if (m_BodySurface <> nil) and (m_BodySurface.Format in [apf_DXT1..apf_DXT5]) then begin

      m_BodyAlphaSurface := mimg.Alphas[GetOffset(m_wAppearance) + m_nCurrentFrame];

    end else m_BodyAlphaSurface := nil;}
  end;
  if m_boUseEffect then begin
    case m_btRace of
      84..86:begin
          case m_ColorEffect of
            ceGrayScale:EffectSurface := mimg.GetCachedGrayImage(310 + m_nEffectFrame, ax, ay);
            ceBright:EffectSurface := mimg.GetCachedBrightImage(310 + m_nEffectFrame, ax, ay);
            else
              EffectSurface := mimg.GetCachedImage(310 + m_nEffectFrame, ax, ay);
          end;

        end;
      87..89:begin
          case m_ColorEffect of
            ceGrayScale:EffectSurface := mimg.GetCachedGrayImage(330 + m_nEffectFrame, ax, ay);
            ceBright:EffectSurface := mimg.GetCachedBrightImage(330 + m_nEffectFrame, ax, ay);
            else
              EffectSurface := mimg.GetCachedImage(330 + m_nEffectFrame, ax, ay);
          end;
        end;
    end;
  end;
end;

procedure TDragonStatue.Run;
var
  prv, nEffectFrame:Integer;
  dwEffectFrameTime, m_dwFrameTimetime:longword;
  bo11:Boolean;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_btDir := 0;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;
  boLoadSurface := CheckLoadSurface;
  m_boMsgMuch := m_MsgList.Count >= 2;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    if m_boMsgMuch then
      dwEffectFrameTime := Round(m_dwEffectFrameTime * 2 / 3)
    else
      dwEffectFrameTime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > dwEffectFrameTime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;

  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        bo260 := FALSE;
        ActionChanged;
      end;
      if (m_nCurrentAction = SM_LIGHTING) and (m_nCurrentFrame = 4) then begin
        PlayScene.NewMagic(Self, 74, 74, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, 0, mtThunder, FALSE, 30, bo11);
        g_PlaySound.PlaySound(8222);
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nEffectFrame <> m_nEffectFrame) then
    boLoadSurface := True;

  if (nEffectFrame <> m_nEffectFrame) then
    ActionChanged;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TPBOMA1Mon }

procedure TPBOMA1Mon.Run;
var
  prv:Integer;
  m_dwFrameTimetime:longword;
  meff:TFlyingBug;
  boLoadSurface:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then begin
    Exit;
  end;

  prv := m_nCurrentFrame; //HZQ 20230520 ¿ÉÄÜµÄ³õÖµ

  boLoadSurface := CheckLoadSurface;

  m_boMsgMuch := m_MsgList.Count >= 2;

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if m_boMsgMuch then
      m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
    else
      m_dwFrameTimetime := m_dwFrameTime;

    if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
      if m_nCurrentFrame < m_nEndFrame then begin
        Inc(m_nCurrentFrame);
        m_dwStartTime := TimeGetTime;
      end
      else begin
        m_nCurrentAction := 0;
        m_boUseEffect := FALSE;
        ActionChanged;
      end;
      if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = 4) then begin
        meff := TFlyingBug(PlayScene.NewFlyObject(Self,
          m_nCurrX,
          m_nCurrY,
          m_nTargetX,
          m_nTargetY,
          m_nTargetRecog,
          mt15));
        if meff <> nil then begin
          meff.ImgLib := g_WMonImages.Indexs[22];
          meff.NextFrameTime := 50;
          meff.FlyImageBase := 350;
          meff.MagExplosionBase := 430;
        end;
      end;
    end;
    m_nCurrentDefFrame := 0;
    m_dwDefFrameTime := TimeGetTime;
  end
  else begin
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if prv <> m_nCurrentFrame then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;
  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

{ TFireDragon }

procedure TFireDragon.CalcActorFrame;
var
  pm:PTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  try
    m_boUseMagic := False;
    case m_btDir of
      3, 4:firedir := 0;
      5:firedir := 1;
      6, 7:firedir := 2;
    end;
    m_btDir := 0;
    m_nCurrentFrame := -1;
    m_nBodyOffset := GetOffset(m_wAppearance);
    pm := GetRaceByPM(m_btRace, m_wAppearance);
    if pm = nil then Exit;
    case m_nCurrentAction of
      SM_DIGUP:begin
          Shift(0, 0, 0, 1);
          m_nStartFrame := 0;
          m_nEndFrame := 9;
          m_dwFrameTime := 300;
          m_dwStartTime := TimeGetTime;
        end;
      SM_HIT:begin
          m_btDir := 0;

          m_nStartFrame := 0;
          m_nEndFrame := 19;
          m_dwFrameTime := 150;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectStart := 0;
          m_nEffectFrame := 0;
          m_nEffectEnd := 19;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 100;
          m_nCurEffFrame := 0;
          m_boUseMagic := True;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
        end;
      SM_LIGHTING:begin //´ó»ðÈ¦¹¥»÷
          m_nStartFrame := 0;
          m_nEndFrame := 5;
          m_dwFrameTime := 150;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectStart := 0;
          m_nEffectFrame := 0;
          m_nEffectEnd := 4;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 100;
          m_nCurEffFrame := 0;
          m_boUseMagic := True;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
        end;
      {SM_FLYAXE:
        begin
          m_nStartFrame := 0;
          m_nEndFrame := 20;
          m_dwFrameTime := 150;
          m_dwStartTime := TimeGetTime;
          m_boUseEffect := True;
          m_nEffectStart := 0;
          m_nEffectFrame := 0;
          m_nEffectEnd := 4;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := 150;
          m_nCurEffFrame := 0;
          m_boUseMagic := True;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
        end;  }
      SM_STRUCK:begin
          m_nStartFrame := 0;
          m_nEndFrame := 9;
          m_dwFrameTime := 300;
          m_dwStartTime := TimeGetTime;

          m_CustomMagicStatusEffect.m_nStruck := 0;
        end;
      {81..83: begin
        m_nStartFrame:=0;
        m_nEndFrame:=5;
        m_dwFrameTime:=150;
        m_dwStartTime:=MyGetTickCount;
        m_boUseEffect:=True;
        m_nEffectStart:=0;
        m_nEffectFrame:=0;
        m_nEffectEnd:=10;
        m_dwEffectStartTime:=MyGetTickCount;
        m_dwEffectFrameTime:=150;
        m_nCurEffFrame:=0;
        m_boUseMagic:=True;
        m_dwWarModeTime:=MyGetTickCount;
        Shift (m_btDir, 0, 0, 1);
      end;}
    end;
  except
    DebugOutStr('TFireDragon.CalcActorFrame');
  end;
end;

constructor TFireDragon.Create;
begin
  inherited;
  n270 := nil;
  firedir := 0;
end;

procedure TFireDragon.AttackEff;
var
  n8, nC, n10, n14, n18:integer;
  bo11:Boolean;
  i, iCount:integer;
begin
  try
    if m_boDeath then exit;
    n8 := m_nCurrX;
    nC := m_nCurrY;
    iCount := Random(4);
    for i := 0 to iCount do begin
      n10 := Random(4);
      n14 := Random(8);
      n18 := Random(8);
      case n10 of
        0:begin
            PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14 - 2, nC + n18 + 1, 0, mtThunder {mtRedThunder}, False, 30, bo11);
          end;
        1:begin
            PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14, nC + n18, 0, mtThunder, False, 30, bo11);
          end;
        2:begin
            PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14, nC + n18 + 1, 0, mtThunder, False, 30, bo11);
          end;
        3:begin
            PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14 - 2, nC + n18, 0, mtThunder, False, 30, bo11);
          end;
      end;
      PlaySound(8206);
    end;
  except
    DebugOutStr('TFireDragon.AttackEff');
  end;
end;

procedure TFireDragon.DrawEff(dx, dy:Integer);
begin
  inherited;
  if m_boUseEffect and (n270 <> nil) then begin
    GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, n270);
  end;
end;

procedure TFireDragon.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  try
    mimg := {FrmMain.WDragonImg} g_WDragonImg;
    if mimg = nil then exit;
    //if (not m_boReverseFrame) then begin
    case m_nCurrentAction of
      SM_HIT:begin
          m_BodySurface := mimg.GetCachedImage(40 + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      SM_LIGHTING:begin
          m_BodySurface := mimg.GetCachedImage(10 + firedir * 10 + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      {81: begin
        m_BodySurface := mimg.GetCachedImage (10 + m_nCurrentFrame, m_nPx, m_nPy);
      end;
      82: begin
        m_BodySurface := mimg.GetCachedImage (20 + m_nCurrentFrame, m_nPx, m_nPy);
      end;
      83: begin
        m_BodySurface := mimg.GetCachedImage (30 + m_nCurrentFrame, m_nPx, m_nPy);
      end;}
      else begin
          m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
        end;
    end;
    (*end else begin
      case m_nCurrentAction of
        SM_HIT: begin
          m_BodySurface := mimg.GetCachedImage (40 + m_nEndFrame - m_nCurrentFrame, ax, ay);
        end;
        SM_LIGHTING: begin
          m_BodySurface := mimg.GetCachedImage (10 + m_btDir * 10 + m_nCurrentFrame, m_nPx, m_nPy);
        end;
        {81: begin
          m_BodySurface := mimg.GetCachedImage (10 + m_nEndFrame - m_nCurrentFrame, ax, ay);
        end;
        82: begin
          m_BodySurface := mimg.GetCachedImage (20 + m_nEndFrame - m_nCurrentFrame, ax, ay);
        end;
        83: begin
          m_BodySurface := mimg.GetCachedImage (30 + m_nEndFrame - m_nCurrentFrame, ax, ay);
        end; }
        else begin
          m_BodySurface := mimg.GetCachedImage (GetOffset (m_wAppearance) + m_nEndFrame - m_nCurrentFrame, m_nPx, m_nPy);
        end;
      end;
    end;  *)

    if m_boUseEffect then begin
      case m_nCurrentAction of
        SM_HIT:begin
            n270 := {FrmMain.WDragonImg} g_WDragonImg.GetCachedImage(60 + m_nEffectFrame, ax, ay);
          end;
        SM_LIGHTING:begin
            n270 := {FrmMain.WDragonImg} g_WDragonImg.GetCachedImage(90 + firedir * 10 + m_nEffectFrame, ax, ay);
          end;
        (*81: begin
          n270 := {FrmMain.WDragonImg}g_WDragonImages.GetCachedImage (90 + m_nEffectFrame, ax, ay);
        end;
        82: begin
          n270 := {FrmMain.WDragonImg}g_WDragonImages.GetCachedImage (100 + m_nEffectFrame, ax, ay);
        end;
        83: begin
          n270 := {FrmMain.WDragonImg}g_WDragonImages.GetCachedImage (110 + m_nEffectFrame, ax, ay);
        end; *)

      end;
    end;
  except
    DebugOutStr('TFireDragon.LoadSurface');
  end;
end;

procedure TFireDragon.Run;
var
  prv:integer;
  m_dwEffectFrameTimetime, m_dwFrameTimetime:longword;
  bo11:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  try
    if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN) then exit;

    m_boMsgMuch := FALSE;
    if m_MsgList.Count >= 2 then m_boMsgMuch := TRUE;
    if m_boRunSound then begin
      PlaySound(8201);
      m_boRunSound := False;
    end;

    if m_boUseEffect then begin
      if m_boMsgMuch then
        m_dwEffectFrameTimetime := Round(m_dwEffectFrameTime * 2 / 3)
      else
        m_dwEffectFrameTimetime := m_dwEffectFrameTime;
      if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
        m_dwEffectStartTime := TimeGetTime;
        if m_nEffectFrame < m_nEffectEnd then begin
          Inc(m_nEffectFrame);
        end
        else begin
          m_boUseEffect := FALSE;
        end;
      end;
    end;

    prv := m_nCurrentFrame;
    if m_nCurrentAction <> 0 then begin
      if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
        m_nCurrentFrame := m_nStartFrame;

      if m_boMsgMuch then
        m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
      else
        m_dwFrameTimetime := m_dwFrameTime;

      if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
        if m_nCurrentFrame < m_nEndFrame then begin
          Inc(m_nCurrentFrame);
          m_dwStartTime := TimeGetTime;
        end
        else begin
          m_nCurrentAction := 0;
          m_boUseEffect := FALSE;
          bo260 := False;
        end;

        if (m_nCurrentAction = SM_LIGHTING) and (m_nCurrentFrame - m_nStartFrame = 1) then begin
          PlayScene.NewMagic(Self, 102, 102, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, m_nTargetRecog, mtFly, True, 30, bo11);
        end;

        if (m_nCurrentAction = SM_FLYAXE) and (m_nCurrentFrame - m_nStartFrame = 1) then begin
          PlayScene.NewMagic(Self, 103, 103, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY, m_nTargetRecog, mtExplosion, True, 30, bo11);
        end;

        if (m_nCurrentAction = SM_HIT) { and (m_nCurrentFrame = 4) } then begin //and (m_nCurrentFrame = 4) then begin
          AttackEff;
          PlaySound(8202);
        end;

        {if (m_nCurrentAction = 81) or (m_nCurrentAction = 82) or (m_nCurrentAction = 83) then begin
          if (m_nCurrentFrame - m_nStartFrame) = 4 then begin
            PlayScene.NewMagic (Self,m_nCurrentAction,m_nCurrentAction,m_nCurrX,m_nCurrY,m_nTargetX,m_nTargetY,m_nTargetRecog,mtFly,True,30,bo11);
            PlaySound(8203);
          end;
        end;  }

      end;
      m_nCurrentDefFrame := 0;
      m_dwDefFrameTime := TimeGetTime;
    end
    else begin
      if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
        if TimeGetTime - m_dwDefFrameTime > 300 then begin
          m_dwDefFrameTime := TimeGetTime;
          Inc(m_nCurrentDefFrame);
          if m_nCurrentDefFrame >= m_nDefFrameCount then
            m_nCurrentDefFrame := 0;
        end;
        DefaultMotion;
      end;
    end;

    if prv <> m_nCurrentFrame then begin
      m_dwLoadSurfaceTime := MyGetTickCount;
      LoadSurface(Self);
      //PlayScene.LoadSurface(LoadSurface);
    end;
    if CheckLoadUserName then LoadNameSurface;
    if CheckLoadNumberLable then LoadNumberLableSurface;
    if CheckLoadSay then LoadSaySurface;
    //if CheckLoadActorIcon then LoadActorIcons;
    //if CheckLoadHealthNumber then LoadHealthNumber;
    if CheckLoadFengHaoSurface then LoadFengHaoSurface; // ÐÞ¸´Ò»Ö±ÅÜ³ÆºÅÏÔÊ¾´íÎó chongchong 2014-10-20
    //if CheckLoadPlayEffect then LoadPlayEffectSurface;
  except
    DebugOutStr('TFireDragon.Run');
  end;
end;

{-------------------------------------------------------------------------------}
// Ñ©ÓòÎÀÊ¿ ±ù·åÐ§¹û

constructor TMon27_3.Create;
begin
  inherited Create;
  m_boStoneMode := False;
end;

procedure TMon27_3.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  // 26-7¹ÖÎï
  if (m_btRace = 201) then begin
    if (m_nState and STATE_STONE_MODE) = 0 then
      m_wAppearance := 258;
  end;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin // ±ù·åÃ»ÓÐËéµÄÇé¿ö
          m_boStoneMode := True;
          if m_btRace = 201 then begin
            m_nStartFrame := 0; // + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
            m_nEndFrame := m_nStartFrame;
            m_dwFrameTime := pm.ActDeath.ftime;
            m_dwStartTime := TimeGetTime;
            m_nDefFrameCount := pm.ActDeath.frame;
            m_boStruckShowNumber := False;
            m_boShowBigHPProgress := False;
            m_boSendQueryBigHPProgress := False;
          end
          else begin
            m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
            m_nEndFrame := m_nStartFrame;
            m_dwFrameTime := pm.ActDeath.ftime;
            m_dwStartTime := TimeGetTime;
            m_nDefFrameCount := pm.ActDeath.frame;
            m_boStruckShowNumber := False;

            m_boShowBigHPProgress := False;
            m_boSendQueryBigHPProgress := False;
          end;
        end
        else begin
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
          m_dwFrameTime := pm.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := pm.ActStand.frame;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_DIGUP: {// ±ù·å¿ªÊ¼ÆÆËé ¹ÖÎï³öÀ´µÄÐ§¹û} begin
        if m_btRace = 201 then begin
          m_nStartFrame := 20;
          m_nEndFrame := 7;
          m_dwFrameTime := 200;
          m_dwStartTime := TimeGetTime;
        end
        else begin
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
        end;
        // WarMode := FALSE;
        Shift(m_btDir, 0, 0, 1);
        // Mon27-3½â³ýÊ¯»¯×´Ì¬ piaoyun 2013-11-20
        m_nState := 0;
        firedir := m_btDir;
      end;
    SM_HIT:begin
        if m_btRace = 201 then begin
          m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
          m_dwFrameTime := pm.ActAttack.ftime;
        end
        else begin
          if m_boStoneMode then begin
            m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
            m_dwFrameTime := pm.ActAttack.ftime;
          end
          else begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
          end;

          m_dwStartTime := TimeGetTime;

          m_boUseEffect := True;
          firedir := m_btDir;
          m_nEffectFrame := 0; // startframe;
          m_nEffectStart := 0; // startframe;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;

        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;

        if m_btRace <> 201 then begin
          m_boUseEffect := True;
          firedir := m_btDir;
          m_nEffectFrame := 0; // startframe;
          m_nEffectStart := 0; // startframe;
          m_nEffectEnd := m_nEffectStart + 9;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
      end;
  end;
end;

procedure TMon27_3.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMon27_3.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_btRace = 201 then Exit;
  if m_boUseEffect then begin
    if m_boDeath then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
            1770 + (firedir * 10) + m_nEffectFrame, //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
            1770 + (firedir * 10) + m_nEffectFrame, //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
            1770 + (firedir * 10) + m_nEffectFrame, //
            ax, ay);
      end;
    end
    else begin
      if m_boStoneMode then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
              1610 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
              1610 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
              1610 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
        end;
      end
      else begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(
              1690 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(
              1690 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(
              1690 + (firedir * 10) + m_nEffectFrame, //
              ax, ay);
        end;
      end;
    end;
  end;
end;

function TMon27_3.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0;
  if (m_btRace = 201) then begin
    if (m_nState and STATE_STONE_MODE) = 0 then
      m_wAppearance := 258;
  end;

  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    if (m_nState and STATE_STONE_MODE) <> 0 then begin
      if m_btRace = 201 then
        Result := 0
      else
        Result := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
    end
    else begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;
end;

procedure TMon27_3.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TMon27_3.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
var
  nX, nY:Integer;
  d:TTexture;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;

  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;

  if m_nCurrentAction = SM_DIGUP then begin
    case m_ColorEffect of
      ceGrayScale:d := g_WMonImages.Indexs[27].GetCachedGrayImage(1850 + (firedir * 10) + (m_nCurrentFrame - m_nStartFrame), nX, nY);
      ceBright:d := g_WMonImages.Indexs[27].GetCachedBrightImage(1850 + (firedir * 10) + (m_nCurrentFrame - m_nStartFrame), nX, nY);
      else
        d := g_WMonImages.Indexs[27].GetCachedImage(1850 + (firedir * 10) + (m_nCurrentFrame - m_nStartFrame), nX, nY);
    end;
    if d <> nil then
      DrawEffSurface(d, dx + nX + m_nShiftX, dy + nY + m_nShiftY, blend, m_ColorEffect);
  end;

  if m_boUseEffect and (EffectSurface <> nil) then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      EffectSurface);
  end;
end;

procedure TMon27_3.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;

// ------------------------------------TDragonBallMonster----------------------------------

constructor TDragonBallMonster.Create;
begin
  inherited;
  // ½×¶Î1
  m_step := 0;
end;

procedure TDragonBallMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:TMagicEff;
  nX, nY:Integer;
  bofly:Boolean;
  I, J:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);

  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  // DScreen.AddChatBoardString('m_wAppearance£º' + IntToStr(m_wAppearance) + ' m_nBodyOffset£º' + IntToStr(m_nBodyOffset), clGreen, clWhite);
  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
        if (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) or (m_wAppearance = 350) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + pm.ActStand.ftime - 1;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start;
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
        if (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) or (m_wAppearance = 350) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + pm.ActStand.ftime - 1;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
      end;

    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;

        Shift(m_btDir, 0, 0, 1);

        if m_nMagicNum > 0 then begin
          SetMagicSound(m_nMagicNum);
          if (m_wAppearance = 324) then begin
            PlayScene.NewMagic(Self,
              111,
              m_nEffectNum, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              m_MagicType, // EffectType
              True,
              -1,
              bofly);
          end
          else begin
            PlayScene.NewMagic(Self,
              111,
              m_nEffectNum, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              m_MagicType, // EffectType
              True,
              0,
              bofly);
          end;
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;

        if (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) or (m_wAppearance = 350) then begin
          m_boUseEffect := True;
        end;
      end;

    SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;

        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;

        if m_wAppearance = 350 then begin
          if m_nMagicNum = 1 then begin
            PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
            meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
            meff.MagExplosionBase := 3350;
            meff.TargetActor := nil;
            meff.NextFrameTime := 80;
            meff.ExplosionFrame := 20;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);
          end
          else if m_nMagicNum = 2 then begin
            //m_nMagicStartSound := s_hit_Lxhy_0;
            //m_nMagicFireSound := s_hit_Lxhy_0;
            PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
            meff := TThuderEffect.Create(780, nX, nY, nil); // target);
            meff.NewLevel := 0;
            meff.ExplosionFrame := 9;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);

            g_PlaySound.PlaySound(s_hit_Lxhy_3);

            meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
            meff.MagExplosionBase := 790;
            meff.TargetActor := nil;
            meff.NextFrameTime := 80;
            meff.ExplosionFrame := 10;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);
          end
          else if m_nMagicNum = 3 then begin
            for I := -4 to 4 do begin
              for J := -2 to 2 do begin
                PlayScene.ScreenXYfromMCXY(m_nTargetX + I * 2, m_nTargetY + J * 2, nX, nY);
                meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
                meff.MagExplosionBase := 3390 + Random(5) * 20;
                meff.TargetActor := nil;
                meff.NextFrameTime := 50 + Random(50);
                meff.ExplosionFrame := 20;
                meff.ImgLib := g_WMonImages.Indexs[33];
                PlayScene.AddEffectList(meff);
              end;
            end;
          end;
        end
          // ÕæºüÔÂÌìÖé ÌØÐ§´¦Àí piaoyun 2013-12-03
        else if (m_wAppearance = 327) then begin
          if (m_nMagicNum and 1) = 1 then begin
            // °Ë·½Ïò¼²¹âµçÓ°
            PlayScene.ScreenXYfromMCXY(Self.m_nCurrX, Self.m_nCurrY, nX, nY);
            for i := 0 to 7 do begin
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 1440 + i * 20;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 10;
              meff.ImgLib := g_WMagic7Images16;
              PlayScene.AddEffectList(meff);
            end;
            g_PlaySound.PlaySound(10100);
          end;

          if (m_nMagicNum and 2) = 2 then begin
            //m_nMagicStartSound := s_hit_Lxhy_0;
            //m_nMagicFireSound := s_hit_Lxhy_0;
            PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
            meff := TThuderEffect.Create(780, nX, nY, nil); // target);
            meff.NewLevel := 0;
            meff.ExplosionFrame := 9;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);

            g_PlaySound.PlaySound(s_hit_Lxhy_3);

            meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
            meff.MagExplosionBase := 790;
            meff.TargetActor := nil;
            meff.NextFrameTime := 80;
            meff.ExplosionFrame := 10;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);

          end;

          if (m_nMagicNum and 4) = 4 then begin
            PlayScene.ScreenXYfromMCXY(m_nTargetX, m_nTargetY, nX, nY);
            meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
            meff.MagExplosionBase := 3350;
            meff.TargetActor := nil;
            meff.NextFrameTime := 80;
            meff.ExplosionFrame := 20;
            meff.ImgLib := g_WMonImages.Indexs[33];
            PlayScene.AddEffectList(meff);
          end;
        end;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + pm.ActAttack.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start;
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        if (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + pm.ActStruck.frame - 1;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        {if (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + pm.ActDie.ftime - 1;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;}
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        // // ÐÞÕý¾ÅÎ²ÁéÊ¯ËÀÍöÌØÐ§ Mon33-7 (add (m_wAppearance = 324) or) chongchong 2014-05-20
        if (m_wAppearance = 324) or (m_wAppearance = 325) or (m_wAppearance = 326) or (m_wAppearance = 327) or (m_wAppearance = 350) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + pm.ActDie.frame - 1;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end;
      end;
  end;
end;

procedure TDragonBallMonster.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TDragonBallMonster.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;

  // ºüÀêÌìÖé chongchong 2015-04-08
  if (m_wAppearance = 350) then begin
    if (not m_boDeath) then begin
      // 2¸ö½×¶Î -- ÖØÐÂÐÞÕýÆðÊ¼Æ«ÒÆ
      m_nBodyOffset := 2900;
      if m_step = 0 then
        m_nBodyOffset := 2900
      else
        m_nBodyOffset := 3220;

      ////////////////////////////¸²¸Çm_nBodyOffset///////////////////////////////
      mimg := g_WMonImages.Indexs[33];
      if mimg <> nil then begin
        if (not m_boReverseFrame) then begin
          case m_ColorEffect of
            ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
          end;
        end;
      end;
      ////////////////////////////////////////////////////////////////////////////

      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
      end;
    end
    else if m_boDeath and m_boUseEffect then begin
      // ÐÞÕýºüÔÂÌìÖéËÀÍöÌØÐ§ Mon33-10 chongchong 2014-05-20
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
      end;
    end;

    Exit;
  end;

  if (m_wAppearance = 327) then begin
    if (not m_boDeath) then begin
      // 5¸ö½×¶Î -- ÖØÐÂÐÞÕýÆðÊ¼Æ«ÒÆ
      m_nBodyOffset := GetOffset(m_wAppearance);
      m_nBodyOffset := m_nBodyOffset + m_step * 80;

      ////////////////////////////¸²¸Çm_nBodyOffset///////////////////////////////
      mimg := g_WMonImages.Images[m_wAppearance];
      if mimg <> nil then begin
        if (not m_boReverseFrame) then begin
          case m_ColorEffect of
            ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(
                m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
                m_nPx, m_nPy);
          end;
        end;
      end;
      ////////////////////////////////////////////////////////////////////////////

      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
            m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
      end;
    end
    else if m_boDeath and m_boUseEffect then begin
      // ÐÞÕýºüÔÂÌìÖéËÀÍöÌØÐ§ Mon33-10 chongchong 2014-05-20
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
            {m_nBodyOffset + 100} 3320 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
      end;
    end;

    Exit;
  end;
  //////////////////////////////////////////////////////////////////////////////

  if m_boUseEffect then begin
    if (m_wAppearance = 324) and m_boDeath then begin
      // ÐÞÕý¾ÅÎ²ÁéÊ¯ËÀÍöÌØÐ§ Mon33-7 chongchong 2014-05-20
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
            {m_nBodyOffset + 100} 2640 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
            {m_nBodyOffset + 100} 2640 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
            {m_nBodyOffset + 100} 2640 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
      end;
    end
    else if (m_wAppearance = 325) or (m_wAppearance = 326) then begin // Mon33-8 Mon33-9 Mon33-8Mon33-10
      if m_nCurrentAction = SM_TURN then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
              m_nBodyOffset + 50 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
              m_nBodyOffset + 50 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
              m_nBodyOffset + 50 + m_nEffectFrame,
              ax, ay);
        end;
      end
      else if m_nCurrentAction = SM_WALK then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
              m_nBodyOffset + 60 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
              m_nBodyOffset + 60 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
              m_nBodyOffset + 60 + m_nEffectFrame,
              ax, ay);
        end;
      end
      else if m_nCurrentAction = SM_HIT then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
              m_nBodyOffset + 70 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
              m_nBodyOffset + 70 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
              m_nBodyOffset + 70 + m_nEffectFrame,
              ax, ay);
        end;
      end
      else if m_nCurrentAction = SM_STRUCK then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
              m_nBodyOffset + 80 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
              m_nBodyOffset + 80 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
              m_nBodyOffset + 80 + m_nEffectFrame,
              ax, ay);
        end;
      end
      else if m_nCurrentAction = SM_NOWDEATH then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
              m_nBodyOffset + 90 + m_nEffectFrame,
              ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
              m_nBodyOffset + 90 + m_nEffectFrame,
              ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
              m_nBodyOffset + 90 + m_nEffectFrame,
              ax, ay);
        end;
      end;
    end;
  end;
end;

function TDragonBallMonster.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky

  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + cf;
  end;
end;

procedure TDragonBallMonster.DrawEff(dx, dy:Integer);
begin
  if (m_boUseEffect) or (m_wAppearance = 327) or (m_wAppearance = 350) then begin
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
  end;
end;

procedure TDragonBallMonster.Run;
var
  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;
  inherited Run;
end;
{------------------------------------------------------------------------------}
{----------------------------TFireSpiritMonster-------------------------------------------}

procedure TFireSpiritMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;

      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        m_boUseMagic := True;
        m_nCurEffFrame := 0;
        m_nMagLight := 2;
        m_nSpellFrame := DEFSPELLFRAME;
        m_dwWaitMagicRequest := TimeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        m_CurMagic.ServerMagicCode := 111;
        m_CurMagic.MagicSerial := m_nMagicNum;
        m_CurMagic.EffectNumber := m_nMagicNum;
        m_CurMagic.targx := m_nTargetX;
        m_CurMagic.targy := m_nTargetY;
        m_CurMagic.target := m_nTargetRecog;
        m_CurMagic.EffectType := mtFly;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TFireSpiritMonster.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TFireSpiritMonster.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  case m_ColorEffect of
    ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedGrayImage(
        m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
    ceBright:AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedBrightImage(
        m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
    else
      AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedImage(
        m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
  end;
end;

function TFireSpiritMonster.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TFireSpiritMonster.DrawEff(dx, dy:Integer);
begin
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TFireSpiritMonster.Run;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited Run;
end;
{------------------------------------------------------------------------------}
{----------------------------TEggMonster-------------------------------------------}

procedure TEggMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start;
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start;
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TEggMonster.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TEggMonster.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  case m_ColorEffect of
    ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedGrayImage(
        m_nBodyOffset + 30 + m_nCurrentFrame, ax, ay);
    ceBright:AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedBrightImage(
        m_nBodyOffset + 30 + m_nCurrentFrame, ax, ay);
    else
      AttackEffectSurface := g_WMonImages.Indexs[34].GetCachedImage(
        m_nBodyOffset + 30 + m_nCurrentFrame, ax, ay);
  end;
end;

function TEggMonster.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + cf;
  end;
end;

procedure TEggMonster.DrawEff(dx, dy:Integer);
begin
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;
{----------------------------TMon35FireDragon-------------------------------------------}

procedure TMon35FireDragon.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;

      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        AttackEff;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TMon35FireDragon.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TMon35FireDragon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_wAppearance = 342 then begin
    if not m_boDeath then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedGrayImage(
            m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedBrightImage(
            m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedImage(
            m_nBodyOffset + 340 + m_nCurrentFrame, ax, ay);
      end;
    end
    else if m_boUseEffect and m_boDeath then begin
      // ÐÞÕý½ðÁúËÀÍöÌØÐ§ Mon33-7 chongchong 2014-05-20
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedGrayImage(
            {m_nBodyOffset + 100} 1610 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedBrightImage(
            {m_nBodyOffset + 100} 1610 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedImage(
            {m_nBodyOffset + 100} 1610 + (m_nCurrentFrame - m_nStartFrame), //
            ax, ay);
      end;
    end;
  end;
end;

function TMon35FireDragon.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TMon35FireDragon.DrawEff(dx, dy:Integer);
begin
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TMon35FireDragon.AttackEff;
var
  n8, nC, n10, n14, n18:Integer;
  bo11:Boolean;
  I, iCount:Integer;
begin
  n8 := m_nCurrX;
  nC := m_nCurrY;
  // PlayScene.NewMagic (Self,80,80,XX,YY,n8 - 3,nC + 3,0,mtThunder,False,30,bo11);
  // PlayScene.NewMagic (Self,80,80,XX,YY,n8 - 3,nC + 3,0,mtThunder,False,30,bo11);
  iCount := Random(4);
  for I := 0 to iCount do begin
    n10 := Random(4);
    n14 := Random(8);
    n18 := Random(8);
    case n10 of
      0:begin
          PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14 - 2, nC + n18 + 1, 0, mtThunder, FALSE, 30, bo11);
        end;
      1:begin
          PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14, nC + n18, 0, mtThunder, FALSE, 30, bo11);
        end;
      2:begin
          PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14, nC + n18 + 1, 0, mtThunder, FALSE, 30, bo11);
        end;
      3:begin
          PlayScene.NewMagic(Self, 80, 80, m_nCurrX, m_nCurrY, n8 - n14 - 2, nC + n18, 0, mtThunder, FALSE, 30, bo11);
        end;
    end;
    g_PlaySound.PlaySound(8206);
  end;
  g_PlaySound.PlaySound(8202);
end;

{----------------------------TLionMonster-------------------------------------------}

procedure TLionMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_RUN:begin
        m_nStartFrame := pm.ActRun.start + m_btDir * (pm.ActRun.frame + pm.ActRun.skip);
        m_nEndFrame := m_nStartFrame + pm.ActRun.frame - 1;
        m_dwFrameTime := pm.ActRun.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActRun.usetick;
        m_nCurTick := 0;
        // WarMode := False;
        if m_nCurrentAction = SM_RUN then
          m_nMoveStep := 2
        else
          m_nMoveStep := 1;

        // DScreen.AddChatBoardString('SM_RUN m_dwFrameTime:' + IntToStr(m_dwFrameTime), clGreen, clWhite);
        Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    SM_LIGHTING:begin // Ê¨×Óºð
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        firedir := m_btDir;
        m_nEffectStart := 580 + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + 10;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        // Ôö¼ÓÊ¨×Ó¹¥»÷ÉùÒô chongchong 2014-11-15
        PlaySound(2422);
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        firedir := m_btDir;

        m_nEffectStart := 500 + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + 10;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        // Ôö¼ÓÊ¨×Ó¹¥»÷ÉùÒô chongchong 2014-11-15
        PlaySound(2422);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_boUseEffect := True;
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

procedure TLionMonster.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TLionMonster.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedGrayImage(
          m_nEffectFrame, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedBrightImage(
          m_nEffectFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[35].GetCachedImage(
          m_nEffectFrame, ax, ay);
    end;
end;

function TLionMonster.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TLionMonster.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        AttackEffectSurface);
    end;
end;

procedure TLionMonster.Run;
var
  m_dwEffectFrameTimetime:longword;
  nEffectFrame:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  nEffectFrame := m_nEffectFrame;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := FALSE;
      end;
    end;
  end;
  if nEffectFrame <> m_nEffectFrame then begin
    LoadSurface(self);
    ActionChanged;
  end;
  inherited Run;

end;

{ TMonKuLou_1 }

procedure TMonKuLou_1.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:THeroShowEffect;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited CalcActorFrame;
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  // ¸²¸Ç¸¸Àà SM_DIGUPÏûÏ¢ piaoyun 2013-07-27
  case m_nCurrentAction of
    SM_DIGUP: {// ³öÉúÐ§¹û} begin
        meff := nil;
        // ÕÙ»½Ç¿»¯÷¼÷Ã³öÉúÐ§¹û chongchong 2013-12-09
        if m_wAppearance = 950 then
          meff := THeroShowEffect.Create(0 + 1040, 15, self)
        else if m_wAppearance = 951 then
          meff := THeroShowEffect.Create(0 + 1060, 15, self)
        else if m_wAppearance = 952 then
          meff := THeroShowEffect.Create(0 + 1080, 15, self);

        if meff <> nil then begin
          meff.ImgLib := g_WMagic7Images16;
          meff.NextFrameTime := 50;
          PlayScene.AddEffectList(meff);
        end
        else begin
          if (m_btRace = 200) then begin
            m_nStartFrame := pm.ActDeath.start;
          end
          else begin
            m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
          end;
          m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
          m_dwFrameTime := pm.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
        end;
      end;
  end;
end;

procedure TMonKuLou_1.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
end;

procedure TMonKuLou_1.Run;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited;

end;

{ TMon36_X }

constructor TMon36_X.Create;
begin
  inherited Create;
  EffectSurface := nil;
end;

procedure TMon36_X.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:TMagicEff;
  Actor:TActor;
  bofly:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited CalcActorFrame;
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;
  case m_nCurrentAction of
    //SM_HIT:
    SM_FLYAXE,
      SM_LIGHTING:begin
        Actor := PlayScene.FindActor(m_nTargetRecog);
        if m_wAppearance = 600 then begin
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(0, 4, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 200;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 604) or (m_wAppearance = 605) then begin
          if m_wAppearance = 605 then begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;
            m_dwWarModeTime := TimeGetTime;
          end;
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(60, 8, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 200;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 607) then begin
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(10, 6, Actor);
            meff.ImgLib := g_WMagic2Images;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
            g_PlaySound.PlaySound(10112);
          end;
        end
        else if (m_wAppearance = 609) then begin
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(10, 10, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 610) then begin
          if Actor <> nil then begin
            if m_nMagicNum > 0 then begin
              // ÖØ»÷
              m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
              m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
              m_dwFrameTime := pm.ActAttack2.ftime;
              m_dwStartTime := TimeGetTime;
              m_dwWarModeTime := TimeGetTime;

              meff := THeroShowEffect.Create(60, 7, Actor);
              meff.ImgLib := g_WMonEffectImg;
              meff.NextFrameTime := 200;
              PlayScene.AddEffectList(meff);
            end; { else
            begin

            end; }
          end;
        end
          // Ô¶³ÌÄ§·¨¹¥»÷ Mon36-15
        else if (m_wAppearance = 614) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(90, 10, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 616) then begin
          if Actor <> nil then begin
            if m_nMagicNum > 0 then begin
              m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
              m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
              m_dwFrameTime := pm.ActAttack2.ftime;
              m_dwStartTime := TimeGetTime;
              m_dwWarModeTime := TimeGetTime;

              // Æø¹¦²¨ÌØÐ§
              meff := THeroShowEffect.Create(190, 6, Actor);
              meff.ImgLib := g_WMagic2Images;
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end
            else begin
              meff := THeroShowEffect.Create(420 + m_btDir * 10, 10, Actor);
              meff.ImgLib := g_WMonEffectImg;
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end
        else if (m_wAppearance = 619) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;

          if Actor <> nil then begin
            meff := THeroShowEffect.Create(100 + m_btDir * 10, 10, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
          // Mon36-21
        else if (m_wAppearance = 620) then begin
          if m_nMagicNum > 0 then begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;
            m_dwWarModeTime := TimeGetTime;

            // ÈºÌå¹¥»÷
            if Actor <> nil then begin
              meff := THeroShowEffect.Create(20, 10, Actor);
              meff.ImgLib := g_WMonEffectImg;
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end
        else if (m_wAppearance = 621) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;
          PlayScene.NewMagic(Self,
            111,
            0, // Effect
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFly, // EffectType
            True,
            0,
            bofly);
        end
        else if (m_wAppearance = 622) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;
          PlayScene.NewMagic(Self,
            111,
            39,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtFly,
            True,
            0,
            bofly);
        end
        else if (m_wAppearance = 623) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;

          PlayScene.NewMagic(Self,
            111,
            21,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtExplosion,
            True,
            0,
            bofly);
        end
        else if (m_wAppearance = 626) then begin
          if m_nMagicNum > 0 then begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * 10; //(pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + 9 - 1; //pm.ActAttack2.frame + 3 - 1;
            m_dwFrameTime := 100; //pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;
            m_dwWarModeTime := TimeGetTime;

            // ËÄ¼¶ÁÒ»ðÌØÐ§
            if Actor <> nil then begin
              meff := THeroShowEffect.Create(m_btDir * 10, 6, Actor);
              meff.ImgLib := g_WMagic6Images;
              meff.NextFrameTime := 140;
              PlayScene.AddEffectList(meff);
            end;
          end
          else begin
            m_nStartFrame := pm.ActAttack2.start + 80 + m_btDir * 10; //(pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + 8 - 1; //pm.ActAttack2.frame + 3 - 1;
            m_dwFrameTime := 100; //pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;
            m_dwWarModeTime := TimeGetTime;

            // ÂÌ¶¾ÌØÐ§
            if Actor <> nil then begin
              meff := THeroShowEffect.Create(20, 10, Actor);
              meff.ImgLib := g_WMonEffectImg;
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end
        else if (m_wAppearance = 628) then begin
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(30, 6, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
          // ºìÉ«À×µçÊõ
        else if (m_wAppearance = 629) then begin
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(230, 6, Actor);
            meff.ImgLib := g_WDragonImg;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 364) then begin
          Shift(m_btDir, 0, 0, 1);
          m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
          m_dwFrameTime := pm.ActAttack.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(1960 + m_btDir * 10, 6, Actor);
            meff.ImgLib := g_WMagic7Images16;
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 365) then begin
          PlayScene.NewMagic(Self,
            111,
            51,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtExplosion,
            True,
            -1,
            bofly);
        end
        else if (m_wAppearance = 366) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;

          if Actor <> nil then begin
            if m_nMagicNum > 0 then begin
              PlayScene.NewMagic(Self,
                111,
                63,
                m_nCurrX,
                m_nCurrY,
                m_nTargetX,
                m_nTargetY,
                m_nTargetRecog,
                mtFly,
                True,
                -1,
                bofly);
            end
            else begin
              // µØÓüÀ×¹âÌØÐ§
              meff := THeroShowEffect.Create(2800, 6, Actor);
              meff.ImgLib := g_WMonImages.Indexs[37];
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end;
      end;
    SM_LIGHTINGEX:begin
        if (m_wAppearance = 607) then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;
          m_dwWarModeTime := TimeGetTime;

          PlayScene.NewMagic(Self,
            111,
            51,
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            mtExplosion,
            True,
            -1,
            bofly);
          g_PlaySound.PlaySound(s_hit_Lxhy_3);
          {Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then
          begin
            meff := THeroShowEffect.Create(60, 8, Actor);
            meff.ImgLib := g_WMonEffectImg;
            meff.NextFrameTime := 200;
            PlayScene.AddEffectList(meff);
          end; }
        end;
      end;
    SM_DIGUP: {// ³öÉúÐ§¹û} begin
        if (m_wAppearance = 601) then begin
          m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
        end;
        if (m_wAppearance = 628) then begin
          m_nStartFrame := pm.ActCritical.start + 80 + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
          m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
          m_dwFrameTime := pm.ActCritical.ftime;
          m_dwStartTime := TimeGetTime;
        end;
        //Shift(m_btDir, 0, 0, 1);
      end;
  end;
end;

procedure TMon36_X.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  EffectSurface := nil; // ×ÔÉíÌØÐ§
  if m_boUseEffect then
    if m_wAppearance = 604 then begin
      case m_ColorEffect of
        ceGrayScale:EffectSurface := g_WMonEffectImg.GetCachedGrayImage(60 + m_nEffectFrame, ax, ay);
        ceBright:EffectSurface := g_WMonEffectImg.GetCachedBrightImage(60 + m_nEffectFrame, ax, ay);
        else
          EffectSurface := g_WMonEffectImg.GetCachedImage(60 + m_nEffectFrame, ax, ay);
      end;
    end;
  {if m_wAppearance = 622 then
  begin
    case m_ColorEffect of
      ceGrayScale: EffectSurface := g_WMonEffectImg.GetCachedGrayImage(260 + m_btDir * 20 + m_nEffectFrame, ax, ay);
      ceBright: EffectSurface := g_WMonEffectImg.GetCachedBrightImage(260 + m_btDir * 20 + m_nEffectFrame, ax, ay);
    else
      EffectSurface := g_WMonEffectImg.GetCachedImage(260 + m_btDir * 20 + m_nEffectFrame, ax, ay);
    end;
  end; }
end;

procedure TMon36_X.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited;
  if not (m_btDir in [0..7]) then Exit;

  if EffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      nPx + dx + m_nShiftX,
      nPy + dy + m_nShiftY,
      EffectSurface);
  end;

  {if m_BodySurface <> nil then
  begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
  end; }
end;

procedure TMon36_X.Run;
var
  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;
  inherited Run;
end;

{ TFireDragonGuard }

constructor TFireDragonGuard.Create;
begin
  inherited Create;
  m_btDir := 0;
  EffectSurface := nil;
  //DownDrawLevel := 1;
end;

procedure TFireDragonGuard.DrawChr(dx, dy:integer; blend,
  boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited DrawChr(dx, dy, blend, boFlag);
  if m_boUseEffect and (not blend) then begin
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
  end;
end;

procedure TFireDragonGuard.CalcActorFrame;
var
  pm:PTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_boUseEffect := FALSE;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then exit;

  deathframe := 0;
  m_boUseEffect := FALSE;

  case m_nCurrentAction of
    SM_LIGHTING, SM_FLYAXE: {//ÂýµÄËÙ¶È·¢ÁÁ} begin
        //m_nStartFrame := pm.ActAttack.start;
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime + 80;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
      end;

    {SM_DEATH:
       begin
          m_nStartFrame := pm.ActDie.start + pm.ActDie.frame - 1;
          m_nEndFrame := m_nStartFrame;
          m_nDefFrameCount := 0;
       end;
    SM_DIGUP:
       begin
          m_nStartFrame := pm.ActDie.start;
          m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
          m_dwFrameTime := pm.ActDie.ftime;
          m_dwStartTime := TimeGetTime;
          deathframe := pm.ActStand.start + m_btDir;
          m_boUseEffect := TRUE;
       end;  }
    else begin
        m_nStartFrame := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame; // + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := 0; //pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
        m_boHoldPlace := TRUE;
      end;
  end;
end;

function TFireDragonGuard.GetDefaultFrame(wmode:Boolean):integer;
var
  pm:PTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  Result := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
end;

procedure TFireDragonGuard.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  //inherited LoadSurface;
  mimg := g_WDragonImg;
  if m_btDir = 1 then {// ÓÒ} begin
    m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + 22, m_nPx, m_nPy);
  end
  else if m_btDir = 0 then begin
    m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + 2, m_nPx, m_nPy);
  end;
  //BrokenSurface := mimg.GetCachedImage (GetOffset (m_wAppearance)+2, bx, by);

  if m_boUseEffect then begin
    if m_btDir = 1 then
      EffectSurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + 22 + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame), ax, ay)
    else
      EffectSurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + 2 + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame), ax, ay);
  end;
end;

{ TMon27_6 ±ùÖù¹ÖÎïÀàÊµÏÖ}

procedure TMon27_6.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;

        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 6;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + pm.ActAttack.frame;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start;
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;
        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 8;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
  end;
end;

procedure TMon27_6.DrawEff(dx, dy:Integer);
begin
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TMon27_6.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

function TMon27_6.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + pm.ActDie.frame - 1;
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + cf;
  end;
end;

procedure TMon27_6.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(2516 + m_nEffectFrame, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(2516 + m_nEffectFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(2516 + m_nEffectFrame, ax, ay);
    end;
    if m_boDeath then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(2490 + m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(2490 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(2490 + m_nEffectFrame, ax, ay);
      end;
    end;
  end;
end;

procedure TMon27_6.Run;
var
  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;
  inherited Run;
end;

{ TMon29_0 }

procedure TMon29_0.CalcActorFrame;
var
  pm:pTMonsterAction;
  bofly:Boolean;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_boUseEffect := False;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_BACKSTEP:begin
        m_nStartFrame := pm.ActWalk.start + m_btDir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_WALK then
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        else begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_LIGHTING:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;

        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 3;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        Shift(m_btDir, 0, 0, 1);

        PlayScene.NewMagic(Self,
          111,
          32, // Effect
          m_nCurrX,
          m_nCurrY,
          m_nTargetX,
          m_nTargetY,
          m_nTargetRecog,
          mtExplosion, // EffectType
          True,
          0,
          bofly);

      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        {m_boUseEffect := True;
        firedir := m_btDir;

        m_nEffectStart := 500 + m_btDir * 10;
        m_nEffectFrame := m_nEffectStart;
        m_nEffectEnd := m_nEffectStart + 10;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;}
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
        {firedir := m_btDir;
        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 8;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime; }
      end;
  end;
end;

procedure TMon29_0.DrawEff(dx, dy:Integer);
begin
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TMon29_0.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

function TMon29_0.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0;
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TMon29_0.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;
  if m_boUseEffect then begin
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[29].GetCachedGrayImage(420 + firedir * 20 + m_nEffectFrame, ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[29].GetCachedBrightImage(420 + firedir * 20 + m_nEffectFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[29].GetCachedImage(420 + firedir * 20 + m_nEffectFrame, ax, ay);
    end;
    {if m_boDeath then
    begin
      case m_ColorEffect of
        ceGrayScale: AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedGrayImage(2490 + m_nEffectFrame, ax, ay);
        ceBright: AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedBrightImage(2490 + m_nEffectFrame, ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[27].GetCachedImage(2490 + m_nEffectFrame, ax, ay);
      end;
    end; }
  end
end;

procedure TMon29_0.Run;
var
  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;
  inherited Run;
end;

{ TMon32_Base }

constructor TMon38_X.Create;
begin
  inherited;
  IsDigup := False;
end;

procedure TMon38_X.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:TMagicEff;
  //Actor: TActor;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  inherited CalcActorFrame;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  m_nCurrentFrame := -1;
  m_boUseEffect := False;
  meff := nil;

  case m_nCurrentAction of
    SM_TURN:begin
        if m_wAppearance = 630 then begin
          //if (m_nState and STATE_STONE_MODE) <> 0 then
          if not IsDigup then begin
            {if (m_btRace = 48) or (m_btRace = 49) then
              m_nStartFrame := pm.ActDeath.start                                                      // + Dir * (pm.ActDeath.frame + pm.ActDeath.skip)
            else
              m_nStartFrame := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);}
            m_nStartFrame := pm.ActCritical.start + m_btDir * 10;
            m_nEndFrame := m_nStartFrame;
          end;
        end;
      end;
    SM_DIGUP:begin
        begin
          if m_wAppearance = 630 then begin
            meff := THeroShowEffect.Create(80 + m_btDir * 10, 9, self);
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.NextFrameTime := pm.ActCritical.ftime;
            PlayScene.AddEffectList(meff);

            m_nStartFrame := pm.ActCritical.start;
            m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
            m_dwFrameTime := pm.ActCritical.ftime;
            m_dwStartTime := TimeGetTime;
            //Shift(m_btDir, 0, 0, 1);
            IsDigup := True;
          end;
        end;
      end;
    SM_HIT:begin
        if (m_wAppearance = 633) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 5;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 635) or (m_wAppearance = 636) or (m_wAppearance = 637) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if m_wAppearance = 643 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

          PlaySound(2412);
        end;
      end;
    SM_LIGHTING:begin
        //Actor := PlayScene.FindActor(m_nTargetRecog);
        if m_wAppearance = 630 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if m_wAppearance = 640 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 5;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if m_wAppearance = 641 then begin
          if m_nMagicNum = 0 then
            // ÆÕÍ¨¹¥»÷
            meff := THeroShowEffect.Create(5690 + m_btDir * 10, 6, self)
          else
            meff := THeroShowEffect.Create(5770 + m_btDir * 10, 6, self);

          if meff <> nil then begin
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
            //Exit;
          end;
        end
        else if m_wAppearance = 642 then begin
          if m_nMagicNum = 0 then begin
            meff := THeroShowEffect.Create(6530, 5, self);
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;
          end
          else if m_nMagicNum = 1 then begin
            meff := THeroShowEffect.Create(6550 + m_btDir * 10, 8, self);
            m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
            m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
            m_dwFrameTime := pm.ActCritical.ftime;
            m_dwStartTime := TimeGetTime;
            {m_boUseEffect := True;
            m_nEffectFrame := 0;
            m_nEffectStart := 0;
            m_nEffectEnd := m_nEffectStart + 10;
            m_dwEffectStartTime := TimeGetTime;
            m_dwEffectFrameTime := m_dwFrameTime; }
          end
          else if m_nMagicNum = 2 then begin
            meff := THeroShowEffect.Create(6632, 5, self);
            m_nStartFrame := pm.ActCritical.start + 80 + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
            m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
            m_dwFrameTime := pm.ActCritical.ftime;
            m_dwStartTime := TimeGetTime;

            m_boUseEffect := True;
            m_nEffectFrame := 0;
            m_nEffectStart := 0;
            m_nEffectEnd := m_nEffectStart + 5;
            m_dwEffectStartTime := TimeGetTime;
            m_dwEffectFrameTime := m_dwFrameTime;
          end;

          if meff <> nil then begin
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
            //Exit;
          end;
        end
        else if m_wAppearance = 638 then begin
          m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
          m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1; //
          m_dwFrameTime := pm.ActAttack2.ftime;
          m_dwStartTime := TimeGetTime;

          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 6;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

          // Ä¿±êÉíÉÏÌØÐ§
          {if Actor <> nil then
          begin
            meff := THeroShowEffect.Create(4220, 3, Actor);
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.NextFrameTime := 200;
            PlayScene.AddEffectList(meff);
          end;}

        end
        else if m_wAppearance = 643 then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;

          if m_nMagicNum = 1 then begin
            m_nStartFrame := pm.ActAttack2.start + m_btDir * (pm.ActAttack2.frame + pm.ActAttack2.skip);
            m_nEndFrame := m_nStartFrame + pm.ActAttack2.frame - 1;
            m_dwFrameTime := pm.ActAttack2.ftime;
            m_dwStartTime := TimeGetTime;

            PlaySound(2396);
            // Ä¿±êÉíÉÏÌØÐ§ -- ×ªÒÆµ½ run º¯ÊýÄÚ²¿
          end else begin
            m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
            m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
            m_dwFrameTime := pm.ActCritical.ftime;
            m_dwStartTime := TimeGetTime;

            PlaySound(122);
          end;
        end;
      end;
    SM_DEATH:begin

      end;
    SM_NOWDEATH:begin
        {m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        firedir := m_btDir;   }

        if (m_wAppearance = 630) then begin
          meff := THeroShowEffect.Create(510 + m_btDir * 10, 10, Self);
          meff.ImgLib := g_WMonImages.Indexs[38];
          meff.NextFrameTime := pm.ActDeath.ftime;
          PlayScene.AddEffectList(meff);
        end
        else if (m_wAppearance = 636) then begin
          m_boUseEffect := True;
          m_nEffectFrame := 0;
          m_nEffectStart := 0;
          m_nEffectEnd := m_nEffectStart + 10;
          m_dwEffectStartTime := TimeGetTime;
          m_dwEffectFrameTime := m_dwFrameTime;
        end
        else if (m_wAppearance = 643) then begin
          PlaySound(1125);
        end;
      end;
  end;
end;

function TMon38_X.GetDefaultFrame(wmode:Boolean):Integer;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0;
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  Result := inherited GetDefaultFrame(wmode);

  if m_wAppearance = 630 then begin
    //if (m_nState and STATE_STONE_MODE) <> 0 then
    if not IsDigup then begin
      Result := pm.ActCritical.start + m_btDir * 10;
    end
  end;
  {
  if m_boDeath then
  begin
    Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
  end
  else
  begin
    if (m_nState and STATE_STONE_MODE) <> 0 then
    begin
      Result := pm.ActDeath.start + m_btDir * (pm.ActDeath.frame + pm.ActDeath.skip);
    end
    else
    begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;}
end;

procedure TMon38_X.LoadSurface(Sender:TObject);
var
  nOffset:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  AttackEffectSurface := nil;

  // Ê¼ÖÕ¸½¼Ó¹âÐ§
  if m_wAppearance = 643 then begin
    if m_boUseEffect then begin
      if m_nCurrentAction = SM_HIT then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
              m_nBodyOffset + 660 + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
              m_nBodyOffset + 660 + m_btDir * 10 + m_nEffectFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
              m_nBodyOffset + 660 + m_btDir * 10 + m_nEffectFrame, ax, ay);
        end;
      end else if m_nCurrentAction = SM_LIGHTING then begin
        if m_nMagicNum = 1 then
          nOffset := 750
        else
          nOffset := 850;

        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
              m_nBodyOffset + nOffset + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
              m_nBodyOffset + nOffset + m_btDir * 10 + m_nEffectFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
              m_nBodyOffset + nOffset + m_btDir * 10 + m_nEffectFrame, ax, ay);
        end;
      end else begin
        if (m_nCurrentAction <> SM_WALK) and (m_nCurrentAction <> SM_RUN) then
          case m_ColorEffect of
            ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
                m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
            ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
                m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
            else
              AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
                m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
          end;
      end;
    end else begin
      pm := GetRaceByPM(m_btRace, m_wAppearance);
      // Õ¾Á¢Ê±ºòÊ¼ÖÕ·¢¹â
      // Ã»ÓÐºÃ°ì·¨ÅÐ¶Ï¹ÖÎïÊÇ·ñÔÚÔ­µØ×´Ì¬¡£¡£ -- Ëã·¨¹«Ê½£ºÉíÌåÆ«ÒÆ + µ±Ç°Ö¡ < ÉíÌåÆ«ÒÆ + Õ¾Á¢×ÜÖ¡Êý  Ôò¶Ï¶¨ÎªÕ¾Á¢×´Ì¬ piaoyun 2014-01-06
      if m_nBodyOffset + m_nCurrentFrame < m_nBodyOffset + pm.ActStand.start + 8 * (pm.ActStand.frame + pm.ActStand.skip) then begin
        case m_ColorEffect of
          ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
              m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
          ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
              m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
          else
            AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
              m_nBodyOffset + 580 + m_nCurrentFrame, ax, ay);
        end;
      end;
    end;
  end
  else if m_boUseEffect then begin
    if (m_wAppearance = 630) then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
            m_nBodyOffset + 430 + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
            m_nBodyOffset + 430 + m_btDir * 10 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
            m_nBodyOffset + 430 + m_btDir * 10 + m_nEffectFrame, ax, ay);
      end;
    end
    else if (m_wAppearance = 633) then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
            m_nBodyOffset + 340 + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
            m_nBodyOffset + 340 + m_btDir * 10 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
            m_nBodyOffset + 340 + m_btDir * 10 + m_nEffectFrame, ax, ay);
      end;
    end
    else if (m_wAppearance = 635) or (m_wAppearance = 637) or (m_wAppearance = 640) then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
            m_nBodyOffset + 340 + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
            m_nBodyOffset + 340 + m_btDir * 10 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
            m_nBodyOffset + 340 + m_btDir * 10 + m_nEffectFrame, ax, ay);
      end;
    end
    else if (m_wAppearance = 636) then begin
      if m_nCurrentAction = SM_NOWDEATH then
        nOffset := 421
      else
        nOffset := 340;

      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
            m_nBodyOffset + nOffset + m_btDir * 10 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
            m_nBodyOffset + nOffset + m_btDir * 10 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
            m_nBodyOffset + nOffset + m_btDir * 10 + m_nEffectFrame, ax, ay);
      end;
    end
    else if (m_wAppearance = 638) then begin
      case m_ColorEffect of
        ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedGrayImage(
            m_nBodyOffset + 420 + {m_nCurrentFrame} m_nEffectFrame, ax, ay);
        ceBright:AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedBrightImage(
            m_nBodyOffset + 420 + m_nEffectFrame, ax, ay);
        else
          AttackEffectSurface := g_WMonImages.Indexs[38].GetCachedImage(
            m_nBodyOffset + 420 + m_nEffectFrame, ax, ay);
      end;
    end
  end;
end;

procedure TMon38_X.Run;
var
  m_dwEffectFrameTimetime:longword;
  Actor:TActor;
  meff:TMagicEff;
  fire16dir:Integer;
  scx, scy, stx, sty:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;

      // Ä¿±êÌØÐ§£¬µ°ÌÛ´¦Àí¡£¡£¡£ piaoyun 2014-01-03
      if (m_nCurrentAction = SM_LIGHTING) and (m_nEffectEnd - m_nEffectFrame = 2) then begin
        if (m_wAppearance = 643) then begin
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            if m_nMagicNum = 1 then begin
              meff := THeroShowEffect.Create(7510, 7, Actor);
              meff.ImgLib := g_WMonImages.Indexs[38];
              meff.NextFrameTime := 100;
              PlayScene.AddEffectList(meff);
            end;
          end;
        end
        else if (m_wAppearance = 640) then begin
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            meff := THeroShowEffect.Create(5171 + m_btDir * 10, 5, Actor);
            meff.ImgLib := g_WMonImages.Indexs[38];
            meff.NextFrameTime := 100;
            PlayScene.AddEffectList(meff);
          end;
        end
        else if (m_wAppearance = 642) and (m_nMagicNum = 2) then begin
          Actor := PlayScene.FindActor(m_nTargetRecog);
          meff := THeroShowEffect.Create(6640, 6, Actor);
          meff.ImgLib := g_WMonImages.Indexs[38];
          meff.NextFrameTime := 100;
          PlayScene.AddEffectList(meff);
        end else if (m_wAppearance = 638) then begin
          // ÐÞÕýMon38-8ÊÍ·ÅÄ§·¨Ê±£¬Ä§·¨·ÉÐÐ½Ç¶ÈÓÐÆ«²î chongchong 2014-05-20
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, scx, scy);
            PlayScene.ScreenXYfromMCXY(Actor.m_nCurrX, Actor.m_nCurrY, stx, sty);
            fire16dir := GetFlyDirection16(scx, scy, stx, sty);
            meff := THeroShowEffect.Create(4140 + fire16dir * 5, 5, Actor);
          end
          else begin
            meff := THeroShowEffect.Create(4140 + m_btDir * 10, 5, nil);
          end;

          meff.ImgLib := g_WMonImages.Indexs[38];
          meff.NextFrameTime := 100;
          PlayScene.AddEffectList(meff);

          // Ä¿±êÉíÉÏÌØÐ§
          meff := THeroShowEffect.Create(4220, 3, Actor);
          meff.ImgLib := g_WMonImages.Indexs[38];
          meff.NextFrameTime := 200;
          PlayScene.AddEffectList(meff);
        end;
      end;
    end;
  end;
  inherited Run;
end;

procedure TMon38_X.DrawChr(dx, dy:Integer; blend, boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited;
  {if not (m_btDir in [0..7]) then Exit;
  if m_BodySurface <> nil then
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
  }
end;

procedure TMon38_X.DrawEff(dx, dy:Integer);
begin
  inherited;
  if AttackEffectSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + ax + m_nShiftX,
      dy + ay + m_nShiftY,
      AttackEffectSurface);
  end;
end;

procedure TMon38_X.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

// ------------------------------------TXueLingLeaderMonster----------------------------------

constructor TXueLingLeaderMonster.Create;
begin
  inherited;
  m_nEffectFrame := 0;
  m_nCustomEffectFrame := 0;
  m_nOldCustomEffectFrame := 0;
  m_BallMoveOffset.X := 0;
  m_BallMoveOffset.Y := 0;

  m_HideDrawBall := False;
end;

procedure TXueLingLeaderMonster.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
  m_nEffectFrame := 0;
  m_nCustomEffectFrame := 0;
  m_nOldCustomEffectFrame := 0;
end;

procedure TXueLingLeaderMonster.CalcActorFrame;
var
  pm:pTMonsterAction;
  meff:TMagicEff;
  nX, nY:Integer;
  Actor:TActor;
begin
  if (m_nChangeAppr >= 0) then begin
    if (m_nCurrentAction = SM_ATTACK01) or
      (m_nCurrentAction = SM_ATTACK02) or
      (m_nCurrentAction = SM_ATTACK03) or
      (m_nCurrentAction = SM_ATTACK04) or
      (m_nCurrentAction = SM_ATTACK05) or
      (m_nCurrentAction = SM_ATTACK06) then
      m_nCurrentAction := SM_HIT;

    inherited;
    Exit;
  end;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);

  if pm = nil then Exit;
  m_boUseEffect := False;

  // DScreen.AddChatBoardString('m_wAppearance£º' + IntToStr(m_wAppearance) + ' m_nBodyOffset£º' + IntToStr(m_nBodyOffset), clGreen, clWhite);
  case m_nCurrentAction of
    SM_TURN, SM_WALK, SM_BACKSTEP, SM_STRUCK:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;

        if m_nCurrentAction = SM_STRUCK then
          m_dwFrameTime := m_dwStruckFrameTime
        else
          m_dwFrameTime := pm.ActStand.ftime;

        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;

    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActStand.start;
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);

        if (m_nMagicNum >= 1) and (m_nMagicNum <= 5) then begin
          m_HideDrawBall := False;

          // ¹¥»÷ÌØÐ§ÒÔÄ¿±êÎªÖÐÐÄ£¬²»¹Ì¶¨ chongchong 2014-09-20
          // nX := m_nCurrX - 1;
          // nY := m_nCurrY - 2;

          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor <> nil then begin
            nX := Actor.m_nCurrX;
            nY := Actor.m_nCurrY;
            if m_nMagicNum in [3, 5] then begin
              nX := m_nCurrX - 1;
              nY := m_nCurrY - 2;
            end;

            //meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
            meff := TCustomMonTargetEffect.Create(0, -1, 0, nX, nY);

            // ÐÞÕýºÚÒ¹ - ÑªÁú½ÌÖ÷¼¼ÄÜ²»µãÁÁ chongchong 2015-03-04
            meff.light := 3;
            case m_nMagicNum of
              1:begin
                  meff.MagExplosionBase := 3104;
                  meff.ExplosionFrame := 10;
                  TCustomMonTargetEffect(meff).DrawMode := mdmNormal;

                  PlaySound(9210);
                end;
              2:begin
                  meff.MagExplosionBase := 3128;
                  meff.ExplosionFrame := 9;

                  PlaySound(10330);
                end;
              3:begin
                  meff.MagExplosionBase := 3184;
                  meff.ExplosionFrame := 8;
                  TCustomMonTargetEffect(meff).DrawMode := mdmNormal;

                  PlaySound(10090);
                end;
              4:begin
                  meff.MagExplosionBase := 3160;
                  meff.ExplosionFrame := 10;

                  PlaySound(10152);
                end;
              5:begin
                  meff.MagExplosionBase := 3144;
                  meff.ExplosionFrame := 12;

                  PlaySound(10280);
                end;
            end;
            meff.TargetActor := nil;
            meff.NextFrameTime := 80;

            meff.ImgLib := g_WMonImages.Indexs[39];
            PlayScene.AddEffectList(meff);
          end;
        end;
      end;
    SM_LIGHTING:begin
        if m_nMagicNum <> 5 then begin
          m_HideDrawBall := True;
          m_nCustomEffectFrame := 0;
        end;
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + pm.ActAttack.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;

        m_boUseEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + pm.ActDie.frame - 1;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;

        m_nDieSound := 11059;
        PlaySound(m_nDieSound);
      end;
  end;
end;

procedure TXueLingLeaderMonster.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  EffectSurface := nil;
  m_SeatsSurface := nil;
  m_WeaponSurface := nil;
  m_LineSurface := nil;
  m_BallSurfaces[0] := nil;
  m_BallSurfaces[1] := nil;
  m_BallSurfaces[2] := nil;
  m_BallSurfaces[3] := nil;

  m_nBodyOffset := GetOffset(m_wAppearance);

  mimg := g_WMonImages.Indexs[39];
  if mimg <> nil then begin
    if (not m_boReverseFrame) then begin
      case m_ColorEffect of
        ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := mimg.GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := mimg.GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end
    else begin
      case m_ColorEffect of
        ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(
            m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
            m_nPx, m_nPy);
        ceBright:m_BodySurface := mimg.GetCachedBrightImage(
            m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
            m_nPx, m_nPy);
        else
          m_BodySurface := mimg.GetCachedImage(
            m_nBodyOffset + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame),
            m_nPx, m_nPy);
      end;
    end;

    case m_ColorEffect of
      ceGrayScale:begin
          m_SeatsSurface := mimg.GetCachedGrayImage(3216, m_SeatsOffset.X, m_SeatsOffset.Y);

          if not m_boDeath then begin
            m_WeaponSurface := mimg.GetCachedImage(3200, m_WeaponOffset.X, m_WeaponOffset.Y);

            m_LineSurface := mimg.GetCachedImage(3032 + m_nCustomEffectFrame, m_LineOffset.X, m_LineOffset.Y);
            m_BallSurfaces[0] := mimg.GetCachedImage(3040 + m_nCustomEffectFrame, m_BallOffsets[0].X, m_BallOffsets[0].Y);
            m_BallSurfaces[1] := mimg.GetCachedImage(3048 + m_nCustomEffectFrame, m_BallOffsets[1].X, m_BallOffsets[1].Y);
            if m_HideDrawBall then begin
              case m_nMagicNum of
                1:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3072 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                2:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3080 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                3:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3088 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                4:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3096 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
              end;
            end
            else
              m_BallSurfaces[2] := mimg.GetCachedImage(3056 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
            m_BallSurfaces[3] := mimg.GetCachedImage(3064 + m_nCustomEffectFrame, m_BallOffsets[3].X, m_BallOffsets[3].Y);
          end
          else
            m_WeaponSurface := mimg.GetCachedGrayImage(3208 + (m_nCurrentFrame - m_nStartFrame), m_WeaponOffset.X, m_WeaponOffset.Y);
        end;
      ceBright:begin
          m_SeatsSurface := mimg.GetCachedBrightImage(3216, m_SeatsOffset.X, m_SeatsOffset.Y);

          if not m_boDeath then begin
            m_WeaponSurface := mimg.GetCachedBrightImage(3200, m_WeaponOffset.X, m_WeaponOffset.Y);

            m_LineSurface := mimg.GetCachedBrightImage(3032 + m_nCustomEffectFrame, m_LineOffset.X, m_LineOffset.Y);
            m_BallSurfaces[0] := mimg.GetCachedBrightImage(3040 + m_nCustomEffectFrame, m_BallOffsets[0].X, m_BallOffsets[0].Y);
            m_BallSurfaces[1] := mimg.GetCachedBrightImage(3048 + m_nCustomEffectFrame, m_BallOffsets[1].X, m_BallOffsets[1].Y);
            if m_HideDrawBall then begin
              case m_nMagicNum of
                1:
                  m_BallSurfaces[2] := mimg.GetCachedBrightImage(3072 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                2:
                  m_BallSurfaces[2] := mimg.GetCachedBrightImage(3080 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                3:
                  m_BallSurfaces[2] := mimg.GetCachedBrightImage(3088 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                4:
                  m_BallSurfaces[2] := mimg.GetCachedBrightImage(3096 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
              end;
            end
            else
              m_BallSurfaces[2] := mimg.GetCachedBrightImage(3056 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
            m_BallSurfaces[3] := mimg.GetCachedBrightImage(3064 + m_nCustomEffectFrame, m_BallOffsets[3].X, m_BallOffsets[3].Y);
          end
          else
            m_WeaponSurface := mimg.GetCachedBrightImage(3208 + (m_nCurrentFrame - m_nStartFrame), m_WeaponOffset.X, m_WeaponOffset.Y);
        end;
      else begin
          m_SeatsSurface := mimg.GetCachedImage(3216, m_SeatsOffset.X, m_SeatsOffset.Y);

          if not m_boDeath then begin
            m_WeaponSurface := mimg.GetCachedImage(3200, m_WeaponOffset.X, m_WeaponOffset.Y);

            m_LineSurface := mimg.GetCachedImage(3032 + m_nCustomEffectFrame, m_LineOffset.X, m_LineOffset.Y);
            m_BallSurfaces[0] := mimg.GetCachedImage(3040 + m_nCustomEffectFrame, m_BallOffsets[0].X, m_BallOffsets[0].Y);
            m_BallSurfaces[1] := mimg.GetCachedImage(3048 + m_nCustomEffectFrame, m_BallOffsets[1].X, m_BallOffsets[1].Y);
            if m_HideDrawBall then begin
              case m_nMagicNum of
                1:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3072 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                2:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3080 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                3:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3088 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
                4:
                  m_BallSurfaces[2] := mimg.GetCachedImage(3096 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
              end;
            end
            else
              m_BallSurfaces[2] := mimg.GetCachedImage(3056 + m_nCustomEffectFrame, m_BallOffsets[2].X, m_BallOffsets[2].Y);
            m_BallSurfaces[3] := mimg.GetCachedImage(3064 + m_nCustomEffectFrame, m_BallOffsets[3].X, m_BallOffsets[3].Y);
          end
          else
            m_WeaponSurface := mimg.GetCachedImage(3208 + (m_nCurrentFrame - m_nStartFrame), m_WeaponOffset.X, m_WeaponOffset.Y);
        end;
    end;
  end;
  ////////////////////////////////////////////////////////////////////////////

  {
  case m_ColorEffect of
    ceGrayScale: AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedGrayImage(
        m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
    ceBright: AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedBrightImage(
        m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
  else
    AttackEffectSurface := g_WMonImages.Indexs[33].GetCachedImage(
      m_nBodyOffset + 40 + m_nCurrentFrame, ax, ay);
  end;
  }
end;

function TXueLingLeaderMonster.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky

  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;

  if m_boDeath then begin
    Result := pm.ActDie.start + (pm.ActDie.frame - 1);
  end
  else begin
    m_nDefFrameCount := pm.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;
    Result := pm.ActStand.start + cf;
  end;
end;

procedure TXueLingLeaderMonster.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
var
  nX, nY:Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  if not (m_btDir in [0..7]) then Exit;

  if m_SeatsSurface <> nil then begin
    DrawEffSurface(m_SeatsSurface, dx + m_SeatsOffset.X + m_nShiftX + 150, dy + m_SeatsOffset.Y + m_nShiftY - 100, blend, ceNone);
  end;

  if m_BodySurface <> nil then begin
    DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;

  if m_WeaponSurface <> nil then begin
    DrawEffSurface(m_WeaponSurface, dx + m_WeaponOffset.X + m_nShiftX, dy + m_WeaponOffset.Y + m_nShiftY, blend, ceNone);
  end;

  if m_boUseEffect then begin
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
  end;

  if m_LineSurface <> nil then begin
    GameCanvas.DrawBlend(
      dx + m_LineOffset.X + m_nShiftX,
      dy + m_LineOffset.Y + m_nShiftY,
      m_LineSurface);
  end;

  if m_BallSurfaces[0] <> nil then begin
    GameCanvas.DrawBlend(
      dx + m_BallOffsets[0].X + m_nShiftX - 120,
      dy + m_BallOffsets[0].Y + m_nShiftY - 267,
      m_BallSurfaces[0]);
  end;

  if m_BallSurfaces[1] <> nil then begin
    GameCanvas.DrawBlend(
      dx + m_BallOffsets[1].X + m_nShiftX,
      dy + m_BallOffsets[1].Y + m_nShiftY - 360,
      m_BallSurfaces[1]);
  end;

  if m_BallSurfaces[2] <> nil then begin
    if m_HideDrawBall then begin
      nX := dx + m_BallOffsets[2].X + m_nShiftX + 150 - m_nCustomEffectFrame * 55;
      nY := dy + m_BallOffsets[2].Y + m_nShiftY - 310 + m_nCustomEffectFrame * 55;
      GameCanvas.DrawBlend(nX, nY, m_BallSurfaces[2]);

      if m_nCustomEffectFrame = 4 then begin
        m_HideDrawBall := False;
      end;
    end
    else begin
      GameCanvas.DrawBlend(
        dx + m_BallOffsets[2].X + m_nShiftX + 150,
        dy + m_BallOffsets[2].Y + m_nShiftY - 310,
        m_BallSurfaces[2]);
    end;
  end;

  if m_BallSurfaces[3] <> nil then begin
    GameCanvas.DrawBlend(
      dx + m_BallOffsets[3].X + m_nShiftX + 220,
      dy + m_BallOffsets[3].Y + m_nShiftY - 160,
      m_BallSurfaces[3]);
  end;
end;

procedure TXueLingLeaderMonster.Run;
var
  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  if m_boUseEffect then begin
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        m_boUseEffect := False;
      end;
    end;
  end;

  if MyGetTickCount - m_nCustomEffectTick > 200 then begin
    m_nCustomEffectTick := MyGetTickCount;
    if m_nCustomEffectFrame < 4 then
      Inc(m_nCustomEffectFrame)
    else
      m_nCustomEffectFrame := 0;
  end;

  inherited Run;
end;

function TXueLingLeaderMonster.CheckLoadSurface:Boolean;
begin
  Result := inherited CheckLoadSurface;
  if not Result then begin
    Result := m_nOldCustomEffectFrame <> m_nCustomEffectFrame;
    m_nOldCustomEffectFrame := m_nCustomEffectFrame;
  end;
end;
{------------------------------------------------------------------------------}

end.
