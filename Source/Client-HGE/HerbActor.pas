unit HerbActor;

interface

uses
  Windows,
  SysUtils,
  Classes,
  Grobal2,
  HGE,
  Graphics,
  SDK,
  magiceff,
  Actor,
  GameImages,
  HGECanvas,
  MMSystem;

const
  BEEQUEENBASE = 600;
  DOORDEATHEFFECTBASE = 120;
  WALLLEFTBROKENEFFECTBASE = 224;
  WALLRIGHTBROKENEFFECTBASE = 240;

type
  TDoorState = (dsOpen, dsClose, dsBroken);

  TKillingHerb = class(TActor) // Size 0x250
  private
  public
    constructor Create; override;
    destructor Destroy; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
  end;

  TMineMon = class(TKillingHerb)
  private
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
  end;

  TBeeQueen = class(TActor)
  private
  public
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
  end;

  TCentipedeKingMon = class(TKillingHerb) // Size 0x260
  private
    AttackEffectSurface:TTexture; // 0x250
    BoUseDieEffect:Boolean; // 0x254
    ax:Integer; // 0x258
    ay:Integer; // 0x25C
    procedure LoadEffect();
  public
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Run; override;
    procedure Finalize; override;
  end;
  TBigHeartMon = class(TKillingHerb) // Size 0x260
  private
  public
    procedure CalcActorFrame; override;
  end;
  TSpiderHouseMon = class(TKillingHerb)
  private
  public
    procedure CalcActorFrame; override;
  end;
  TCastleDoor = class(TActor)
  private
    EffectSurface:TTexture;
    ax, ay:Integer;
    oldunitx, oldunity:Integer;
    procedure ApplyDoorState(dstate:TDoorState);
  public
    BoDoorOpen:Boolean;
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure ActionEnded; override;
    procedure Run; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Finalize; override;
  end;

  TWallStructure = class(TActor) // 0x62
  private
    EffectSurface:TTexture;
    BrokenSurface:TTexture;
    ax, ay, bx, by:Integer;
    deathframe:Integer;
    bomarkpos:Boolean; // 못가게 막고 있는지
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TNewWallStructure = class(TActor) // 0x62
  private
    EffectSurface:TTexture;
    BrokenSurface:TTexture;
    ax, ay, bx, by:Integer;
    deathframe:Integer;
    bomarkpos:Boolean; // 못가게 막고 있는지
  public
    constructor Create; override;
    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;
    procedure Finalize; override;
  end;

  TSoccerBall = class(TActor) // 0x9
  private
  public
  end;
  TDragonBody = class(TKillingHerb) // 0x5a
  private
  public
    procedure DrawEff(dx, dy:Integer); override;
    procedure CalcActorFrame(); override;
    procedure LoadSurface(Sender:TObject); override;
  end;
implementation

uses
  ClMain,
  MShare;

{============================== TKillingHerb =============================}

constructor TKillingHerb.Create;
begin
  inherited Create;
end;

destructor TKillingHerb.Destroy;
begin
  inherited Destroy;
end;

procedure TKillingHerb.CalcActorFrame;
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

  case m_nCurrentAction of
    SM_TURN: {//} begin
        m_nStartFrame := pm.ActStand.start; // + Dir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_DIGUP: {// , SM_DIGUP, .} begin
        m_nStartFrame := pm.ActWalk.start; // + Dir * (pm.ActWalk.frame + pm.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + pm.ActWalk.frame - 1;
        m_dwFrameTime := pm.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := pm.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := FALSE;
        m_nMoveStep := 1;
        Shift(m_btDir, 0, 0, 1); // m_nMoveStep, 0, m_nEndFrame-startframe+1);
      end;
    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
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
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
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
    SM_DIGDOWN:begin
        m_nStartFrame := pm.ActDeath.start;
        m_nEndFrame := m_nStartFrame + pm.ActDeath.frame - 1;
        m_dwFrameTime := pm.ActDeath.ftime;
        m_dwStartTime := TimeGetTime;
        m_boDelActionAfterFinished := True; // 이동작이 끝나면 액터 지음
      end;
  end;
end;

function TKillingHerb.GetDefaultFrame(wmode:Boolean):Integer;
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
    Result := pm.ActStand.start + cf;
  end;
end;

{----------------------------------------------------------------------}

procedure TBeeQueen.CalcActorFrame;
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

  case m_nCurrentAction of
    SM_TURN: {// 방향이 없음...} begin
        m_nStartFrame := pm.ActStand.start; // + Dir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_LIGHTINGEX:begin
        m_nStartFrame := pm.ActAttack.start; // + Dir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
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
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
      end;
    SM_HIT:begin
        m_nStartFrame := pm.ActAttack.start; // + Dir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start; // + Dir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start; // + Dir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_nStartFrame := m_nEndFrame; //
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start; // + Dir * (pm.ActDie.frame + pm.ActDie.skip);
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
      end;
  end;
end;

function TBeeQueen.GetDefaultFrame(wmode:Boolean):Integer;
var
  pm:pTMonsterAction;
  cf:Integer;
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
    Result := pm.ActStand.start + cf; // 방향이 없음..
  end;
end;

{----------------------------------------------------------------------}
// 지네왕

procedure TCentipedeKingMon.CalcActorFrame;
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

  case m_nCurrentAction of
    SM_TURN: {//} begin
        m_btDir := 0;
        inherited CalcActorFrame;
      end;
    SM_LIGHTINGEX:begin
        m_btDir := 0;
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        BoUseDieEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 9;
        m_dwEffectFrameTime := 62;
        // BoUseEffect:=True;
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
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
      end;
    SM_HIT:begin
        m_btDir := 0;
        m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        BoUseDieEffect := True;
        m_nEffectFrame := 0;
        m_nEffectStart := 0;
        m_nEffectEnd := m_nEffectStart + 9;
        m_dwEffectFrameTime := 62;
        // BoUseEffect:=True;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_DIGDOWN:begin
        inherited CalcActorFrame;
      end;
    else begin
        m_btDir := 0;
        inherited CalcActorFrame;
      end;
  end;
end;

{----------------------------------------------------------------------}
// 성벽, 성문

constructor TCastleDoor.Create;
begin
  inherited Create;
  m_btDir := 0;
  EffectSurface := nil;
  m_nDownDrawLevel := 1; // 1셀 먼저 그림. (사람 머리가 성문 밑으로 들어가는 것을 막음)
end;

procedure TCastleDoor.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
end;

procedure TCastleDoor.ApplyDoorState(dstate:TDoorState);
var
  bowalk:Boolean;
begin
  Map.MarkCanWalk(m_nCurrX, m_nCurrY - 2, True);
  Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 1, True);
  Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 2, True);
  if dstate = dsClose then
    bowalk := FALSE
  else
    bowalk := True;

  Map.MarkCanWalk(m_nCurrX, m_nCurrY, bowalk);
  Map.MarkCanWalk(m_nCurrX, m_nCurrY - 1, bowalk);
  Map.MarkCanWalk(m_nCurrX, m_nCurrY - 2, bowalk);
  Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 1, bowalk);
  Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 2, bowalk);
  Map.MarkCanWalk(m_nCurrX - 1, m_nCurrY - 1, bowalk);
  Map.MarkCanWalk(m_nCurrX - 1, m_nCurrY, bowalk);
  Map.MarkCanWalk(m_nCurrX - 1, m_nCurrY + 1, bowalk);
  Map.MarkCanWalk(m_nCurrX - 2, m_nCurrY, bowalk);

  if dstate = dsOpen then begin
    Map.MarkCanWalk(m_nCurrX, m_nCurrY - 2, FALSE);
    Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 1, FALSE);
    Map.MarkCanWalk(m_nCurrX + 1, m_nCurrY - 2, FALSE);
  end;
end;

procedure TCastleDoor.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Self);
  EffectSurface := nil;
  mimg := g_WMonImages.Images[m_wAppearance];
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:EffectSurface := mimg.GetCachedGrayImage(DOORDEATHEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
      ceBright:EffectSurface := mimg.GetCachedBrightImage(DOORDEATHEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
      else
        EffectSurface := mimg.GetCachedImage(DOORDEATHEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
    end;

end;

procedure TCastleDoor.CalcActorFrame;
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
  m_boUseEffect := FALSE;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_sUserName := ' ';

  case m_nCurrentAction of
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
        ApplyDoorState(dsBroken); // 움직일 수 있게
      end;
    SM_STRUCK:begin
        m_nStartFrame := pm.ActStruck.start + m_btDir * (pm.ActStruck.frame + pm.ActStruck.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStruck.frame - 1;
        m_dwFrameTime := m_dwStruckFrameTime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;
    SM_DIGUP: {// 문 열림} begin
        m_nStartFrame := pm.ActAttack.start;
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
        ApplyDoorState(dsOpen); // 움직일 수 있게
      end;
    SM_DIGDOWN: {// 문 닫힘} begin
        m_nStartFrame := pm.ActCritical.start;
        m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
        m_dwFrameTime := pm.ActCritical.ftime;
        m_dwStartTime := TimeGetTime;
        BoDoorOpen := FALSE;
        m_boHoldPlace := True;
        ApplyDoorState(dsClose); // 못움직임
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + pm.ActDie.frame - 1;
        m_nEndFrame := m_nStartFrame;
        m_nDefFrameCount := 0;
        ApplyDoorState(dsBroken); // 움직일 수 있게
      end;
    else {// 방향이 없음...} begin
        if m_btDir < 3 then begin
          m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
          m_nEndFrame := m_nStartFrame; // + pm.ActStand.frame - 1;
          m_dwFrameTime := pm.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := 0; // pm.ActStand.frame;
          Shift(m_btDir, 0, 0, 1);
          BoDoorOpen := FALSE;
          m_boHoldPlace := True;
          ApplyDoorState(dsClose); // 못움직이게
        end
        else begin
          m_nStartFrame := pm.ActCritical.start; // 열려있는 상태
          m_nEndFrame := m_nStartFrame;
          m_nDefFrameCount := 0;

          BoDoorOpen := True;
          m_boHoldPlace := FALSE;
          ApplyDoorState(dsOpen); // 걸을 수 있음
        end;
      end;
  end;
end;

function TCastleDoor.GetDefaultFrame(wmode:Boolean):Integer;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  if m_boDeath then begin
    Result := pm.ActDie.start + pm.ActDie.frame - 1;
    m_nDownDrawLevel := 2;
  end
  else begin
    if BoDoorOpen then begin
      m_nDownDrawLevel := 2;
      Result := pm.ActCritical.start; // + Dir * (pm.ActStand.frame + pm.ActStand.skip);
    end
    else begin
      m_nDownDrawLevel := 1;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
    end;
  end;
end;

procedure TCastleDoor.ActionEnded;
begin
  if m_nCurrentAction = SM_DIGUP then begin // 문열림
    BoDoorOpen := True;
    m_boHoldPlace := FALSE;
  end;
  // if CurrentAction = SM_DIGDOWN then
  // DefaultMotion;
end;

procedure TCastleDoor.Run;
begin
  if (Map.m_nCurUnitX <> oldunitx) or (Map.m_nCurUnitY <> oldunity) then begin
    if m_boDeath then
      ApplyDoorState(dsBroken)
    else if BoDoorOpen then
      ApplyDoorState(dsOpen)
    else
      ApplyDoorState(dsClose);
  end;
  oldunitx := Map.m_nCurUnitX;
  oldunity := Map.m_nCurUnitY;
  inherited Run;
end;

procedure TCastleDoor.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited DrawChr(dx, dy, blend, FALSE);
  if m_boUseEffect and not blend then begin
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
  end;
end;

{----------------------------------------------------------------------}
// 성벽

constructor TWallStructure.Create;
begin
  inherited Create;
  m_btDir := 0;
  EffectSurface := nil;
  BrokenSurface := nil;
  bomarkpos := FALSE;
  // DownDrawLevel := 1;
end;

procedure TWallStructure.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
  BrokenSurface := nil;
end;

procedure TWallStructure.CalcActorFrame;
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
  m_boUseEffect := FALSE;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_sUserName := ' ';
  deathframe := 0;

  case m_nCurrentAction of
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        deathframe := pm.ActStand.start + m_btDir;
        Shift(m_btDir, 0, 0, 1);
        m_boUseEffect := True;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + pm.ActDie.frame - 1;
        m_nEndFrame := m_nStartFrame;
        m_nDefFrameCount := 0;
        deathframe := pm.ActStand.start + m_btDir;
      end;
    SM_DIGUP: {// 모습이 변경될때 마다} begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        deathframe := pm.ActStand.start + m_btDir;
        m_boUseEffect := True;
      end;
    else {// 방향이 없음...} begin
        m_nStartFrame := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame; // + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := 0; // pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
        m_boHoldPlace := True;
        if (m_nCurrentAction = SM_TURN) then begin
          deathframe := pm.ActStand.start + m_btDir;
          // m_boHoldPlace := False;
          // DScreen.AddChatBoardString('CalcActorFrame SM_TURN:' + IntToStr(m_nCurrentAction) + ' m_btDir:' + IntToStr(m_btDir) + ' deathframe:' + IntToStr(deathframe), clRed, clWhite);

        end;
      end;
  end;
  // DScreen.AddChatBoardString('CalcActorFrame:' + IntToStr(m_nCurrentAction) + ' m_btDir:' + IntToStr(m_btDir) + ' deathframe:' + IntToStr(deathframe), clRed, clWhite);

end;

procedure TWallStructure.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  // LoadNameSurface;
  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  BrokenSurface := nil;
  EffectSurface := nil;

  mimg := g_WMonImages.Images[m_wAppearance];
  // DebugOutStr('LoadSurface2:' + IntToStr(m_wAppearance));

  if deathframe > 0 then begin // (CurrentAction = SM_NOWDEATH) or (CurrentAction = SM_DEATH) then begin
    if mimg <> nil then begin
      // DScreen.AddChatBoardString('1 m_wAppearance:' + IntToStr(m_wAppearance) + ' GetOffset:' + IntToStr(GetOffset(m_wAppearance) + deathframe) + ' FileName:' + mimg.FileName, clRed, clWhite);

      case m_ColorEffect of
        ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
        ceBright:m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
        else
          m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
      end;
    end;

  end
  else begin
    inherited LoadSurface(Self);
  end;

  if m_wAppearance >= 904 then begin
    {mimg := g_WMonImages.Images[m_wAppearance - 3];
    if mimg <> nil then begin
      // DScreen.AddChatBoardString('2 m_wAppearance:' + IntToStr(m_wAppearance - 3) +' GetOffset:'+ IntToStr(GetOffset(m_wAppearance - 3) + 8) + ' FileName:' + mimg.FileName, clRed, clWhite);
      case m_ColorEffect of
        ceGrayScale: BrokenSurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance - 3) + 8 + m_btDir, bx, by);
        ceBright: BrokenSurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance - 3) + 8 + m_btDir, bx, by);
      else BrokenSurface := mimg.GetCachedImage(GetOffset(m_wAppearance - 3) + 8 + m_btDir, bx, by);
      end;
    end;}
  end
  else begin
    if mimg <> nil then begin
      case m_ColorEffect of
        ceGrayScale:BrokenSurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + 8 + m_btDir, bx, by);
        ceBright:BrokenSurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + 8 + m_btDir, bx, by);
        else
          BrokenSurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + 8 + m_btDir, bx, by);
      end;
    end;
  end;

  if mimg <> nil then begin
    // DScreen.AddChatBoardString('4 m_wAppearance:' + IntToStr(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame)) + ' FileName:' + mimg.FileName, clRed, clWhite);
    if m_boUseEffect then begin
      if m_wAppearance >= 904 then begin
        { case m_ColorEffect of
           ceGrayScale: EffectSurface := mimg.GetCachedGrayImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
           ceBright: EffectSurface := mimg.GetCachedBrightImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
         else EffectSurface := mimg.GetCachedImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
         end; }

      end
      else begin
        case m_ColorEffect of
          ceGrayScale:EffectSurface := mimg.GetCachedGrayImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
          ceBright:EffectSurface := mimg.GetCachedBrightImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
          else
            EffectSurface := mimg.GetCachedImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
        end;
      end;
    end;
  end;
  ActionChanged;
end;

function TWallStructure.GetDefaultFrame(wmode:Boolean):Integer;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  Result := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
end;

procedure TWallStructure.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited DrawChr(dx, dy, blend, boFlag);
  if (BrokenSurface <> nil) and (not blend) then begin
    GameCanvas.Draw(dx + bx + m_nShiftX,
      dy + by + m_nShiftY,
      BrokenSurface.ClientRect,
      BrokenSurface);
  end;
  if m_boUseEffect and (not blend) then begin
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
  end;
end;

procedure TWallStructure.Run;
begin
  if m_boDeath then begin
    if bomarkpos then begin
      Map.MarkCanWalk(m_nCurrX, m_nCurrY, True);
      bomarkpos := FALSE;
    end;
  end
  else begin
    if not bomarkpos then begin
      Map.MarkCanWalk(m_nCurrX, m_nCurrY, FALSE);
      bomarkpos := True;
    end;
  end;
  PlayScene.SetActorDrawLevel(Self, 0);
  inherited Run;
end;

{----------------------------------------------------------------------}
// 성벽

constructor TNewWallStructure.Create;
begin
  inherited Create;
  m_btDir := 0;
  EffectSurface := nil;
  BrokenSurface := nil;
  bomarkpos := FALSE;
  // DownDrawLevel := 1;
end;

procedure TNewWallStructure.Finalize;
begin
  inherited Finalize;
  EffectSurface := nil;
  BrokenSurface := nil;
end;

procedure TNewWallStructure.CalcActorFrame;
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
  m_boUseEffect := FALSE;
  m_nCurrentFrame := -1;

  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  m_sUserName := ' ';
  deathframe := 0;

  case m_nCurrentAction of
    SM_NOWDEATH:begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        deathframe := pm.ActStand.start + m_btDir;
        Shift(m_btDir, 0, 0, 1);
        // m_boUseEffect := True;
      end;
    SM_DEATH:begin
        m_nStartFrame := pm.ActDie.start + pm.ActDie.frame - 1;
        m_nEndFrame := m_nStartFrame;
        m_nDefFrameCount := 0;
      end;
    SM_DIGUP: {// 모습이 변경될때 마다} begin
        m_nStartFrame := pm.ActDie.start;
        m_nEndFrame := m_nStartFrame + pm.ActDie.frame - 1;
        m_dwFrameTime := pm.ActDie.ftime;
        m_dwStartTime := TimeGetTime;
        deathframe := pm.ActStand.start + m_btDir;
        // m_boUseEffect := True;
      end;
    else {// 방향이 없음...} begin
        m_nStartFrame := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame; // + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_nDefFrameCount := 0; // pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
        m_boHoldPlace := True;
      end;
  end;
end;

procedure TNewWallStructure.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  // LoadNameSurface;

  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  BrokenSurface := nil;
  EffectSurface := nil;

  mimg := g_WMonImages.Images[m_wAppearance];
  if deathframe > 0 then begin // (CurrentAction = SM_NOWDEATH) or (CurrentAction = SM_DEATH) then begin
    if mimg <> nil then begin
      // DScreen.AddChatBoardString('1 m_wAppearance:' + IntToStr(m_wAppearance) + ' FileName:' + mimg.FileName, clRed, clBlue);

      case m_ColorEffect of
        ceGrayScale:m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
        ceBright:m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
        else
          m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + deathframe, m_nPx, m_nPy);
      end;
    end;

  end
  else begin
    inherited LoadSurface(Self);
  end;

  mimg := g_WMonImages.Images[m_wAppearance - 904];
  if mimg <> nil then begin
    // DScreen.AddChatBoardString('2 m_wAppearance:' + IntToStr(m_wAppearance - 904) + ' FileName:' + mimg.FileName, clRed, clBlue);
    case m_ColorEffect of
      ceGrayScale:BrokenSurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance - 904) + 8 + m_btDir, bx, by);
      ceBright:BrokenSurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance - 904) + 8 + m_btDir, bx, by);
      else
        BrokenSurface := mimg.GetCachedImage(GetOffset(m_wAppearance - 904) + 8 + m_btDir, bx, by);
    end;

    if m_boUseEffect then begin

      case m_ColorEffect of
        ceGrayScale:EffectSurface := mimg.GetCachedGrayImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
        ceBright:EffectSurface := mimg.GetCachedBrightImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
        else
          EffectSurface := mimg.GetCachedImage(WALLLEFTBROKENEFFECTBASE + (m_nCurrentFrame - m_nStartFrame), ax, ay);
      end;
    end;
  end;
  ActionChanged;
end;

function TNewWallStructure.GetDefaultFrame(wmode:Boolean):Integer;
var
  pm:pTMonsterAction;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;
  Result := 0; // jacky
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  Result := pm.ActStand.start + m_btDir; // * (pm.ActStand.frame + pm.ActStand.skip);
end;

procedure TNewWallStructure.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;
  inherited DrawChr(dx, dy, blend, boFlag);
  if (BrokenSurface <> nil) and (not blend) then begin
    GameCanvas.Draw(dx + bx + m_nShiftX,
      dy + by + m_nShiftY,
      BrokenSurface.ClientRect,
      BrokenSurface);
  end;
  if m_boUseEffect and (not blend) then begin
    if EffectSurface <> nil then begin
      GameCanvas.DrawBlend(
        dx + ax + m_nShiftX,
        dy + ay + m_nShiftY,
        EffectSurface);
    end;
  end;
end;

procedure TNewWallStructure.Run;
begin
  if m_boDeath then begin
    if bomarkpos then begin
      Map.MarkCanWalk(m_nCurrX, m_nCurrY, True);
      bomarkpos := FALSE;
    end;
  end
  else begin
    if not bomarkpos then begin
      Map.MarkCanWalk(m_nCurrX, m_nCurrY, FALSE);
      bomarkpos := True;
    end;
  end;
  PlayScene.SetActorDrawLevel(Self, 0);
  inherited Run;
end;

{ TMineMon }

procedure TMineMon.CalcActorFrame;
begin
  inherited;

end;

constructor TMineMon.Create;
begin
  inherited;
end;

procedure TCentipedeKingMon.DrawEff(dx, dy:Integer);
begin
  if m_boUseEffect then
    if AttackEffectSurface <> nil then
      GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, AttackEffectSurface);
end;

procedure TCentipedeKingMon.LoadEffect;
begin
  AttackEffectSurface := nil;
  if m_boUseEffect then
    case m_ColorEffect of
      ceGrayScale:AttackEffectSurface := g_WMonImages.Indexs[15].GetCachedGrayImage(
          100 + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      ceBright:AttackEffectSurface := g_WMonImages.Indexs[15].GetCachedBrightImage(
          100 + m_nEffectFrame - m_nEffectStart,
          ax, ay);
      else
        AttackEffectSurface := g_WMonImages.Indexs[15].GetCachedImage(
          100 + m_nEffectFrame - m_nEffectStart,
          ax, ay);
    end;
end;

procedure TCentipedeKingMon.Finalize;
begin
  inherited Finalize;
  AttackEffectSurface := nil;
end;

procedure TCentipedeKingMon.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  inherited LoadSurface(Sender);
  LoadEffect();
end;

function TMineMon.GetDefaultFrame(wmode:Boolean):Integer;
begin
  if (m_nChangeAppr >= 0) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end else begin
    Result := 0; //HZQ 2030524
  end;
end;

{ TBigHeartMon }

procedure TBigHeartMon.CalcActorFrame;
begin
  m_btDir := 0;
  inherited CalcActorFrame;
end;

{ TSpiderHouseMon }

procedure TSpiderHouseMon.CalcActorFrame;
begin
  m_btDir := 0;
  inherited CalcActorFrame;
end;

procedure TCentipedeKingMon.Run;
var
  nEffectFrame:Integer;
begin
  if (m_nCurrentAction = SM_WALK) or
    (m_nCurrentAction = SM_BACKSTEP) or
    (m_nCurrentAction = SM_HORSERUN) or
    (m_nCurrentAction = SM_RUN)
    then begin
    Exit;
  end;

  CheckLoadSurface;

  nEffectFrame := m_nEffectFrame;
  if BoUseDieEffect then begin
    if (m_nCurrentFrame - m_nStartFrame) >= 5 then begin
      BoUseDieEffect := FALSE;
      m_boUseEffect := True;
      m_dwEffectStartTime := TimeGetTime();
      m_nEffectFrame := 0;
      LoadEffect();
    end;
  end;
  if m_boUseEffect then begin
    if (TimeGetTime - m_dwEffectStartTime) > m_dwEffectFrameTime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
        LoadEffect();
      end
      else
        m_boUseEffect := FALSE;
    end;
  end;

  if nEffectFrame <> m_nEffectFrame then
    LoadSurface(self);

  inherited;
end;

{ TDragonBody }

procedure TDragonBody.CalcActorFrame;
var
  pm:pTMonsterAction;
begin
  m_btDir := 0;
  m_boUseMagic := FALSE;
  m_nCurrentFrame := -1;
  m_nBodyOffset := GetOffset(m_wAppearance);
  pm := GetRaceByPM(m_btRace, m_wAppearance);
  if pm = nil then Exit;
  if m_nCurrentAction = SM_DIGUP then begin
    m_nMaxTick := pm.ActWalk.ftime;
    m_nCurTick := 0;
    m_nMoveStep := 1;
    Shift(m_btDir, 0, 0, 1);
  end;
  m_nStartFrame := 0;
  m_nEndFrame := 1;
  m_dwFrameTime := 400;
  m_dwStartTime := TimeGetTime();
end;

procedure TDragonBody.DrawEff(dx, dy:Integer);
begin
  if not (m_btDir in [0..7]) then Exit;
  if m_BodySurface <> nil then
    GameCanvas.DrawBlend(dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, m_BodySurface);
end;

procedure TDragonBody.LoadSurface(Sender:TObject);
begin
  if (m_nChangeAppr >= 0) then begin
    inherited;
    Exit;
  end;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;

  // LoadNameSurface;

  case m_ColorEffect of
    ceGrayScale:m_BodySurface := g_WDragonImg.GetCachedGrayImage(GetOffset(m_wAppearance), m_nPx, m_nHpy);
    ceBright:m_BodySurface := g_WDragonImg.GetCachedBrightImage(GetOffset(m_wAppearance), m_nPx, m_nHpy);
    else
      m_BodySurface := g_WDragonImg.GetCachedImage(GetOffset(m_wAppearance), m_nPx, m_nHpy);
  end;

  LoadActorIcons;
  ActionChanged;
end;

end.
