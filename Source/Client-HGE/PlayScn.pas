unit PlayScn;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  Controls,
  Forms,
  DIB,
  Dialogs,
  MapUnit,
  DxComponents,
  HGE,
  HGEFontEx,
  Math,
  IntroScn,
  Grobal2,
  SDK,
  HUtil32,
  Actor,
  GameImages,
  StdCtrls,
  ClFunc,
  magiceff,
  ExtCtrls,
  DxCanvas,
  HGECanvas,
  PlugEngine,
  CustomActor,
  GlobalString,
  MMSystem,
  DropItemsMgr,
  NearActorHintEffect;

const
  LONGHEIGHT_IMAGE = 32;
  FLASHBASE = 410;
  AAX = 16;
  SOFFX = 0;
  SOFFY = 0;

var
  ShakeX:Integer = 0;
  ShakeY:Integer = 0;

  WalkShift:array[DR_UP..DR_UPLEFT] of array[0..5] of TPoint = (
    ((X:0; Y:5), (X:0; Y:6), (X:0; Y:5), (X:0; Y:5), (X:0; Y:6), (X:0; Y:5)), // DR_UP
    ((X:8; Y:5), (X:8; Y:6), (X:8; Y:5), (X:8; Y:5), (X:8; Y:6), (X:8; Y:5)), // DR_UPRIGHT
    ((X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0)), // DR_RIGHT
    ((X:8; Y:5), (X:8; Y:6), (X:8; Y:5), (X:8; Y:5), (X:8; Y:6), (X:8; Y:5)), // DR_DOWNRIGHT
    ((X:0; Y:5), (X:0; Y:6), (X:0; Y:5), (X:0; Y:5), (X:0; Y:6), (X:0; Y:5)), // DR_DOWN
    ((X:8; Y:5), (X:8; Y:6), (X:8; Y:5), (X:8; Y:5), (X:8; Y:6), (X:8; Y:5)), // DR_DOWNLEFT
    ((X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0), (X:8; Y:0)), // DR_LEFT
    ((X:8; Y:5), (X:8; Y:6), (X:8; Y:5), (X:8; Y:5), (X:8; Y:6), (X:8; Y:5)) // DR_UPLEFT
    );

  RunShift:array[DR_UP..DR_UPLEFT] of array[0..5] of TPoint = (
    ((X:0; Y:11), (X:0; Y:10), (X:0; Y:11), (X:0; Y:11), (X:0; Y:10), (X:0; Y:11)), // DR_UP
    ((X:16; Y:11), (X:16; Y:10), (X:16; Y:11), (X:16; Y:11), (X:16; Y:10), (X:16; Y:11)), // DR_UPRIGHT
    ((X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0)), // DR_RIGHT
    ((X:16; Y:11), (X:16; Y:10), (X:16; Y:11), (X:16; Y:11), (X:16; Y:10), (X:16; Y:11)), // DR_DOWNRIGHT
    ((X:0; Y:11), (X:0; Y:10), (X:0; Y:11), (X:0; Y:11), (X:0; Y:10), (X:0; Y:11)), // DR_DOWN
    ((X:16; Y:11), (X:16; Y:10), (X:16; Y:11), (X:16; Y:11), (X:16; Y:10), (X:16; Y:11)), // DR_DOWNLEFT
    ((X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0), (X:16; Y:0)), // DR_LEFT
    ((X:16; Y:11), (X:16; Y:10), (X:16; Y:11), (X:16; Y:11), (X:16; Y:10), (X:16; Y:11)) // DR_UPLEFT
    );

type
  TPlayScene = class(TScene)
    m_nShiftX, m_nShiftY:Integer;

    m_nCurrX:Integer;
    m_nCurrY:Integer;

    m_MapRect:TRect;
    m_btDir:Byte;
    m_boCanDrawTileMap:Boolean;
    m_nCurrentAction:Integer;
    m_nActionCount:Integer;

    m_DrawGroundEffectList:TList;
    m_DrawEffectList:TList;
    m_DrawFlyList:TList;
    m_DrawEventList:TList;
    m_boCanDraw:Boolean;

    {m_MArr: array[0..52 * 3 * 2, 0..52 * 3 * 2] of TNewMapInfo;
    m_ClientRect: TRect;
    m_OldClientRect: TRect;
    m_nBlockLeft: Integer;
    m_nBlockTop: Integer;

    m_nOldBlockLeft: Integer;
    m_nOldBlockTop: Integer;

    m_nNewBlockLeft: Integer;
    m_nNewBlockTop: Integer;

    m_nOldLeft: Integer;
    m_nOldTop: Integer;
    m_boNewMap: Boolean; }

    m_nCurrMonBigHPRecogId:Int64;
    m_nCurrMonLastAttack:LongWord;
  private
    //m_dwMoveTime: Int64;
    m_dwMoveTime:LongWord;
    m_nMoveStepCount:Integer;

    m_dwMonMoveTime:LongWord;
    m_nMonMoveStepCount:Integer;

    m_dwAniTime:longword;
    m_nAniCount:Integer;
    m_nDefXX:Integer;
    m_nDefYY:Integer;
    m_MainSoundTimer:TTimer;
    m_MsgList:TList;
    m_nProcHumIDx:Integer;

    m_nProcDrawSceneIdx:Integer;
    m_nProcDrawEffectIdx:Integer;
    m_nProcDrawItemsIdx:Integer;
    m_nProcDrawActorLabelIdx:Integer;

    m_nProcDropItemsIdx:Integer;

    m_SceneShakeList:TList;
    m_dwSceneShakeTick:LongWord;

    m_boDelaySceneShake:Boolean;
    m_dwDelaySceneShakeTick:LongWord;
    m_dwDelaySceneShakeTime:LongWord;
    m_dwDelaySceneShakeCount:LongWord;

    m_dwLockTargetEffectLastTick:LongWord;
    m_nLockTargetEffectFrame:Integer;

    m_ClearDropItemTick:LongWord;

    m_NearActorHintMgr:TNearActorHintEffectMgr;

    //m_ActorLabelTexture: TTextureTile;
    //m_ActorLabelImages: TTextureImages;

    procedure DrawTileMap(ClientRect:TRect);
    procedure DrawTileEIMap(ClientRect:TRect);

    function MakeActorLabel(ColorStart, ColorStop:TColor):TTexture;

    procedure SoundOnTimer(Sender:TObject);
    function CrashManEx(mx, my:Integer):Boolean;

    procedure ClearDropItem();
    procedure ShowItemName(DropItem:pTDropItem; X, Y:Integer);

    function UnLockCrashManEx(mx, my:Integer):Boolean;

    procedure AddSceneShakeOffset(nX, nY:Smallint);

    procedure DrawDropItemEffect(nX, nY:Integer; DropItem:pTDropItem; IsBelowItem:Boolean);
    procedure DrawDropItemValueEffect(nX, nY:Integer; DropItem:pTDropItem);

    procedure DrawPreviewMonItemEffect(nX, nY:Integer; Item:PClientPreviewMonItem; IsBelowItem:Boolean);
    procedure DrawPreviewMonItemValueEffect(nX, nY:Integer; Item:PClientPreviewMonItem);

  protected //HZQ 20230525 以下函数来自于 private段
    function CrashManEx_2(mx, my:Integer):Boolean;
    procedure LoadTileMapSurface(Sender:TObject);
    procedure LoadSceneSurface(Sender:TObject);
  private
    m_DrawActorList:TList;
    m_SortYDrawActorList:TList;
    function DoSearchActor(ID:Int64; var Index:Integer):Boolean; virtual;
    function DoSearchSortYDrawActtor(nRY:Integer; var Index:Integer):Boolean;
    procedure DoAddActor(Actor:TActor);
    function DoDelActor(Actor:TACtor):Boolean;
  public
    m_ActorList:TGList;
    m_GroundEffectList:TGList;
    m_EffectList:TGList;
    m_FlyList:TGList;
    m_dwBlinkTime:longword;
    m_boViewBlink:Boolean;

    constructor Create;
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Finalize; override;
    procedure OpenScene; override;
    procedure CloseScene; override;
    procedure OpeningScene; override;

    procedure ProcessActors();

    function ButchAnimal(X, Y:Integer):TActor;

    function FindActorList(id:Int64):TActor;
    function FindActor(id:Int64):TActor; overload;
    function FindActor(sname:string):TActor; overload;
    function FindActorXY(X, Y:Integer):TActor; overload;
    function FindActorXY(X, Y:Integer; Actor:TActor):TActor; overload;
    function IsValidActor(Actor:TActor):Boolean;
    function IsValidActorEx(Actor:TActor):Boolean;
    function NewActor(chrid:Int64; cx, cy, cdir:Word; Feature:TFeature; cState:Integer; boLockList:Boolean = True):TActor;
    procedure ActorDied(Actor:TActor); // 磷篮 actor绰 盖 困肺
    procedure SetActorDrawLevel(Actor:TActor; Level:Integer);
    procedure ClearActors;
    procedure DeleteActor(id:Int64);
    procedure DelActor(Actor:TActor);
    procedure AddEffectList(MagicEff:TMagicEff);
    function SendMsg(ident:Integer; chrid:Int64; X, Y, cdir:Integer; Feature:pTFeature; State:Int64; Str:string):TActor;

    procedure NewMagic(aowner:TActor;
      magid, magnumb, cx, cy, tx, ty, targetcode:Int64;
      mtype:TMagicType;
      Recusion:Boolean;
      anitime:Integer;
      var bofly:Boolean; NewLevel:Integer = 0; boMagItemType:Boolean = True; boLockList:Boolean = False; MagicLevel:Integer = 0);

    function NewFlyObject(aowner:TActor; cx, cy, tx, ty, targetcode:Int64; mtype:TMagicType):TMagicEff;
    // function  NewStaticMagic (aowner: TActor; tx, ty, targetcode, effnum: integer);

    procedure ScreenXYfromMCXY(cx, cy:Integer; var sx, sy:Integer);
    procedure CXYfromMouseXY(mx, my:Integer; var ccx, ccy:Integer);
    function GetCharacter(X, Y, wantsel:Integer; var nowsel:Integer; liveonly:Boolean):TActor;
    function GetAttackFocusCharacter(X, Y, wantsel:Integer; var nowsel:Integer; liveonly:Boolean):TActor;
    function IsSelectMyself(X, Y:Integer):Boolean;
    function GetDropItems(X, Y:Integer; var inames:string):pTDropItem; overload;
    function GetDropItems(X, Y:Integer; HintList:TList):pTDropItem; overload;

    function GetXYDropItems(nX, nY:Integer):pTDropItem;

    procedure GetXYDropItemsList(nX, nY:Integer; var ItemList:TList);
    function CanRun(sx, sy, ex, ey:Integer):Boolean;
    function CanWalk(mx, my:Integer):Boolean;
    function CanWalkEx(mx, my:Integer):Boolean;
    function CanHorseRun(sx, sY, ex, ey:Integer):Boolean;
    function NewCanRun(sx, sy, ex, ey:Integer):Boolean;
    function NewCanWalkEx(mx, my:Integer):Boolean;
    function NewCanWalkEx_2(mx, my:Integer):Boolean;
    function CrashMan(mx, my:Integer):Boolean;

    function CanFly(mx, my:Integer):Boolean;
    function MapCanMove(mx, my:Integer):Boolean;
    function MapCanFly(mx, my:Integer):Boolean;
    procedure RefreshScene;
    procedure CleanObjects;

    procedure RenderScene(Sender:TObject); override;
    procedure DrawCurMonBigHPProgress;

    procedure RenderLight(Sender:TObject);

    procedure RenderGuide(Sender:TObject);
    procedure RenderTileMap(Sender:TObject);
    function CanDrawTileMap:Boolean;
    procedure LoadSurface(NotifyEvent:Classes.TNotifyEvent);

    procedure DrawScene(Sender:TObject);
    procedure DrawEffect(Sender:TObject);
    procedure DrawItems(Sender:TObject);
    procedure DrawActorLabel(Sender:TObject);
    procedure DrawNearActorHintEffect(Sender:TObject);

    procedure DrawPreviewItem;

    function UnLockCanWalkEx(mx, my:Integer):Boolean;
    function UnLockCanWalk(mx, my:Integer):Boolean;
    function UnLockCrashMan(mx, my:Integer):Boolean;
    function UnLockCanRun(sx, sY, ex, ey:Integer):Boolean;
    // 屏幕震动特效 piaoyun 2013-09-14
    procedure SceneShake(Count:Integer = 1; DelayTime:Integer = 0);
    procedure CheckSceneShake;
  end;

implementation

uses
  ClMain,
  MShare,
  FState,
  DrawScrn,
  HerbActor,
  AxeMon,
  SoundUtil,
  clEvent,
  GameConfigDlg,
  uWeatherEffectDef,
  SerialWindowsDlg,
  FilterItems;

constructor TPlayScene.Create;
begin
  m_MsgList := TGList.Create;
  m_ActorList := TGList.Create;
  m_ActorList.Capacity := 1000;

  m_DrawActorList := TList.Create;
  m_DrawActorList.Capacity := 1000;

  m_SortYDrawActorList := TList.Create;
  m_SortYDrawActorList.Capacity := 1000;

  m_GroundEffectList := TGList.Create;
  m_EffectList := TGList.Create;
  m_FlyList := TGList.Create;

  m_DrawGroundEffectList := TList.Create;
  m_DrawEffectList := TList.Create;
  m_DrawFlyList := TList.Create;
  m_DrawEventList := TList.Create;

  //m_ActorLabelImages := nil;
  //m_ActorLabelTexture := nil;
  m_dwBlinkTime := MyGetTickCount;
  m_boViewBlink := False;
  m_nProcHumIDx := 0;

  m_nProcDrawSceneIdx := 0;
  m_nProcDrawEffectIdx := 0;
  m_nProcDrawItemsIdx := 0;
  m_nProcDrawActorLabelIdx := 0;

  m_dwMonMoveTime := MyGetTickCount;
  m_nMonMoveStepCount := 0;

  m_dwMoveTime := TimeGetTime();
  m_nMoveStepCount := 0;

  m_dwAniTime := MyGetTickCount;
  m_nAniCount := 0;

  m_MainSoundTimer := TTimer.Create(frmMain.Owner);
  with m_MainSoundTimer do begin
    OnTimer := SoundOnTimer;
    Interval := 1;
    Enabled := False;
  end;

  //nX := SCREENWIDTH div 2 - 210 {192} {192};
  //nY := SCREENHEIGHT div 2 - 150 {146} {150};
  m_boCanDraw := False;

  m_nCurrMonBigHPRecogId := 0;
  m_nCurrMonLastAttack := MyGetTickCount;

  m_SceneShakeList := TList.Create;
  m_dwSceneShakeTick := MyGetTickCount;
  // m_LoadSurface := TLoadSurface.Create(True);

  m_boDelaySceneShake := False;
  m_dwDelaySceneShakeTick := MyGetTickCount;
  m_dwDelaySceneShakeTime := 0;
  m_dwDelaySceneShakeCount := 1;

  m_dwLockTargetEffectLastTick := MyGetTickCount;
  m_nLockTargetEffectFrame := 0;

  m_ClearDropItemTick := MyGetTickCount;


  m_NearActorHintMgr := TNearActorHintEffectMgr.Create;
  m_NearActorHintMgr.FrameCount := 6;  
end;

destructor TPlayScene.Destroy;
var
  I:Integer;
begin
   m_NearActorHintMgr.Free;

  //if m_ActorLabelImages <> nil then
  //  FreeAndNil(m_ActorLabelImages);
  m_MsgList.Free;

  { TODO -ochongchong -c内存泄露【2013-7-13】 : 去内存泄露 }
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      TActor(m_ActorList.Items[I]).Free;
    end;
    m_ActorList.Clear;

    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}

  // -------------------------------------------------------------end

  m_ActorList.Free;
  m_DrawActorList.Free;
  m_SortYDrawActorList.Free;

  m_GroundEffectList.Free;
  m_EffectList.Free;
  m_FlyList.Free;

  m_DrawGroundEffectList.Free;
  m_DrawEffectList.Free;
  m_DrawFlyList.Free;
  m_DrawEventList.Free;

  m_SceneShakeList.Free;
  inherited Destroy;
end;

procedure TPlayScene.SoundOnTimer(Sender:TObject);
begin
  PlaySound(s_main_theme);
  m_MainSoundTimer.Interval := 46 * 1000;
end;

procedure TPlayScene.LoadSurface(NotifyEvent:Classes.TNotifyEvent);
{var
  Actor: TActor;
  Sender: TObject;}
begin
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  NotifyEvent(TObject(TMethod(NotifyEvent).Data));
end;

function TPlayScene.MakeActorLabel(ColorStart, ColorStop:TColor):TTexture;
var
  DIB:TDIB;
  // FileData: Pointer; FileSize: Integer;
begin
  DIB := TDIB.Create;
  DIB.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
  DIB.SetSize(32, 3, 32);

  DIB.Canvas.Brush.Color := ColorStart;
  DIB.Canvas.FillRect(Bounds(0, 0, DIB.Width, 2));

  DIB.Canvas.Brush.Color := ColorStop;
  DIB.Canvas.FillRect(Bounds(0, 2, DIB.Width, 1));

  Result := NewTexture(DIB);
  DIB.Free;
end;

procedure TPlayScene.Initialize;
const
  NpcColorStart = $B4B400;
  NpcColorStop = $767600;
  HumColorStart = $6262FF;
  HumColorStop = $0000CB;

  HumHighLightColorStart = $00FF00;
  HumHighLightColorStop = $009100;

  HumNGColorStart = clYellow;
  HumNGolorStop = $0000CECE;
var
  HumLabel:TTexture;
  HumHightLightLabel:TTexture;

  HumNGLabel:TTexture;
  MonLabel:TTexture;
  NpcLabel:TTexture;

begin
  m_nShiftX := 0;
  m_nShiftY := 0;
  m_nCurrX := -1;
  m_nCurrY := -1;
  m_nCurrentAction := 0;
  m_nActionCount := 0;
  m_nProcDrawSceneIdx := 0;
  m_nProcDrawEffectIdx := 0;
  m_nProcDrawItemsIdx := 0;
  m_nProcDrawActorLabelIdx := 0;
  m_nProcDropItemsIdx := 0;
  // HumLabel := MakeActorLabel(HumColorStart, HumColorStop);
  HumLabel := MakeActorLabel(clRed, clRed);
  HumHightLightLabel := MakeActorLabel(HumHighLightColorStart, HumHighLightColorStop);
  MonLabel := MakeActorLabel(HumColorStart, HumColorStop);
  NpcLabel := MakeActorLabel(NpcColorStart, NpcColorStop);
  HumNGLabel := MakeActorLabel(HumNGColorStart, HumNGolorStop);

  {
  m_ActorLabelImages := TTextureImages.Create;
  m_ActorLabelImages.SetSize(512, 3);
  m_ActorLabelImages.Initialize;

  m_ActorLabelTexture := TTextureTile.Create(m_ActorLabelImages);
  m_ActorLabelTexture.Width := 512;
  m_ActorLabelTexture.Height := 3;
  m_ActorLabelTexture.PatternWidth := 32;
  m_ActorLabelTexture.PatternHeight := 3;
  m_ActorLabelImages.Add(m_ActorLabelTexture);

  d := g_WNewopUIImages.Images[7];

  if d <> nil then
    m_ActorLabelTexture.Add(0, 0, d);

  if HumLabel <> nil then
    m_ActorLabelTexture.Add(0, 0, HumLabel);

  if MonLabel <> nil then
    m_ActorLabelTexture.Add(0, 0, MonLabel);

  if NpcLabel <> nil then
    m_ActorLabelTexture.Add(0, 0, NpcLabel);

  if HumHightLightLabel <> nil then
    m_ActorLabelTexture.Add(0, 0, HumHightLightLabel);

  if HumNGLabel <> nil then
    m_ActorLabelTexture.Add(0, 0, HumNGLabel);
  }

  if HumLabel <> nil then FreeAndNil(HumLabel);
  if MonLabel <> nil then FreeAndNil(MonLabel);
  if NpcLabel <> nil then FreeAndNil(NpcLabel);
  if HumNGLabel <> nil then FreeAndNil(HumNGLabel);
  if HumHightLightLabel <> nil then FreeAndNil(HumHightLightLabel);
  m_boCanDraw := True;
end;

procedure TPlayScene.Finalize;
var
  I, II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;
begin
  m_boCanDraw := False;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      TActor(m_ActorList.Items[I]).Finalize;
    end;

    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
  EventMan.Finalize;

  FillChar(Map.m_OldClientRect, SizeOf(TRect), 0);

  g_DropItemsMgr.Lock;
  try
    for I := 0 to g_DropItemsMgr.Count - 1 do begin
      List := g_DropItemsMgr.Items[I];
      for II := 0 to List.Count - 1 do begin
        DropItem := List.Items[II];
        DropItem.ItemTexture := nil;
        DropItem.NameImageInfo.Width := 0;
        DropItem.NameImageInfo.Height := 0;
        SetLength(DropItem.NameImageInfo.ImageIndexs, 0);
      end;
    end;
  finally
    g_DropItemsMgr.UnLock;
  end;

  //m_ActorLabelTexture := nil;
  //FreeAndNil(m_ActorLabelImages);
end;

procedure TPlayScene.OpenScene;
begin
  FrmDlg.ViewBottomBox(True);
  // SetImeMode(frmMain.Handle, LocalLanguage);

end;

procedure TPlayScene.CloseScene;
begin
  // MainSoundTimer.Enabled := False;
  SilenceSound;

  FrmDlg.HideChatEdit;
  FrmDlg.ViewBottomBox(False);
end;

procedure TPlayScene.OpeningScene;
begin

end;

procedure TPlayScene.RefreshScene;
begin

end;

procedure TPlayScene.CleanObjects;
var
  I:Integer;
  Actor:TActor;
  Effect:TMagicEff;
begin
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    m_DrawActorList.Clear;

    for I := m_ActorList.Count - 1 downto 0 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (Actor <> g_MySelf) then begin
        if (Actor <> g_MyHero) then begin
          Actor.m_boGhost := True;
          AddFreeActorList(Actor);
        end;

        m_ActorList.Delete(I);
      end;
    end;

    if g_MySelf <> nil then begin
      m_DrawActorList.Add(g_MySelf);
    end;

    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}

  m_MsgList.Clear;

  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  try
    {$IFEND}
    if g_MySelf <> nil then begin
      g_MySelf.ProcLastMsg;
      g_MySelf.CleanMsgs;
      // g_MySelf.Finalize;
    end;

    if g_MyHero <> nil then begin
      g_MyHero.CleanMsgs;
      // g_MyHero.Finalize;
    end;

    g_TargetCret := nil;
    g_FocusCret := nil;
    g_FocusCretTick := MyGetTickCount;
    g_MagicTarget := nil;
    g_BrightActor := nil;
    {$IF IsMultiThreadRender = 1}
  finally
    LeaveCriticalSection(g_ActorLock);
  end;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.Lock;
  try
    {$IFEND}
    for I := 0 to m_GroundEffectList.Count - 1 do begin
      Effect := TMagicEff(m_GroundEffectList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_GroundEffectList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_GroundEffectList.UnLock;
  end;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_EffectList.Lock;
  try
    {$IFEND}
    for I := 0 to m_EffectList.Count - 1 do begin
      Effect := TMagicEff(m_EffectList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_EffectList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_EffectList.UnLock;
  end;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_FlyList.Lock;
  try
    {$IFEND}
    for I := 0 to m_FlyList.Count - 1 do begin
      Effect := TMagicEff(m_FlyList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_FlyList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_FlyList.UnLock;
  end;
  {$IFEND}

  EventMan.ClearEvents;
  frmMain.ClearDropItems;

  // g_PlaySound.Clear;

  m_nProcDropItemsIdx := 0;

  {
   m_DrawDropItemList.Clear;
   m_DrawActorList.Clear;
   m_DrawGroundEffectList.Clear;
   m_DrawEffectList.Clear;
   m_DrawFlyList.Clear;
   m_DrawEventList.Clear; }
end;

{---------------------- Draw Map -----------------------}

procedure ClearDropItemName(DropItem:pTDropItem);
begin
  DropItem.NameImageInfo.ImageIndexs := nil;
  DropItem.NameImageInfo.Width := 0;
  DropItem.NameImageInfo.Height := 0;
end;

procedure TPlayScene.ClearDropItem;
var
  I, II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;
  boShowItemName:Boolean;

  dwDropItemFlashTime:DWORD;
  sShowName:string;
begin
  if g_MySelf = nil then Exit;

  g_DropItemsMgr.Lock;
  try
    for I := 0 to g_DropItemsMgr.Count - 1 do begin
      List := g_DropItemsMgr.Items[I];

      if List.Count = 0 then Continue;

      List.RefreshDrawList;

      for II := 0 to List.DrawCount - 1 do begin
        DropItem := List.DrawItems[II];

        // 特殊物品快闪 piaoyun 2013-09-10
        if (DropItem.ShowItem <> nil) and
          g_ConfigDlg.ConfigCheckeds[ckSpecialQuickFlashing] { PlugCheckBoxSpecialQuickFlashing} and
        pTShowItem(DropItem.ShowItem).boShowSpecial then
          dwDropItemFlashTime := 300
        else
          dwDropItemFlashTime := g_dwDropItemFlashTime;

        if (MyGetTickCount - DropItem.FlashTime > dwDropItemFlashTime {g_dwDropItemFlashTime} {5 * 1000}) then begin
          DropItem.FlashTime := MyGetTickCount;
          DropItem.ShowFlash := True;
          DropItem.FlashStepTime := MyGetTickCount;
          DropItem.FlashStep := 0;
        end;

        if DropItem.ShowFlash then begin
          if (MyGetTickCount - DropItem.FlashStepTime >= 20) then begin
            DropItem.FlashStepTime := MyGetTickCount;
            Inc(DropItem.FlashStep);
          end;
          if {(DropItem.FlashStep >= 0) and }(DropItem.FlashStep < 10) then begin //HZQ 20230524

          end else begin
            DropItem.ShowFlash := False;
          end;
        end;

        if GameCanvas.Active and GameCanvas.Initialized then begin
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            DropItem.ItemTexture := g_WDnItemImages.Grays[DropItem.looks]
          else if DropItem = g_FocusItem then
            DropItem.ItemTexture := g_WDnItemImages.Brights[DropItem.looks]
          else
            DropItem.ItemTexture := g_WDnItemImages.Images[DropItem.looks];
        end;

        if DropItem = g_FocusItem then begin
          if g_OldFocusItem <> g_FocusItem then begin
            g_OldFocusItem := g_FocusItem;
          end;
        end;

        if (g_FocusItem = nil) and (g_OldFocusItem <> nil) then begin
          if g_OldFocusItem = DropItem then begin
            g_OldFocusItem := nil;
          end;
        end;

        // 修复内挂 物品显示 无效BUG --- piaoyun 2013-09-07
        if PlugInEnabled then
          boShowItemName := (DropItem.ShowItem <> nil) and pTShowItem(DropItem.ShowItem).boShowName
        else
          boShowItemName := False;

        DropItem.ShowName := boShowItemName or g_boShowAllItem {这个是快捷键 ctrl+shift+z的全局变量--保留原来逻辑没改};

        if GameCanvas.Active and GameCanvas.Initialized and DropItem.ShowName and (CurrentFont <> nil) then begin
          if (Length(DropItem.NameImageInfo.ImageIndexs) <= 0) then begin
            sShowName := DropItem.Name; // ProcessItemName(DropItem.Name);

            if DropItem.OverlapCount > 1 then begin
              if g_ConfigClient.boOverLapItemNumOldShow then begin
                DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName + ' (' + IntToStr(DropItem.OverlapCount) + ')');
              end
              else begin
                DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName + 'x' + IntToStr(DropItem.OverlapCount));
              end;
            end
            else
              DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName);
          end;
        end
        else
          ClearDropItemName(DropItem);
      end;
    end;
  except
    on E:Exception do begin
      DebugOutStr('[Exception] ClearDropItem2');
      DebugOutStr(E.Message);
    end;
  end;
  g_DropItemsMgr.UnLock;
end;

// 显示地上物品 piaoyun 2013-09-10

procedure TPlayScene.ShowItemName(DropItem:pTDropItem; X, Y:Integer);
var
  nX, nY:Integer;
  ShowItem:pTShowItem;

  sShowName:string;
begin
  if DropItem.ShowName and (CurrentFont <> nil) then begin
    if (Length(DropItem.NameImageInfo.ImageIndexs) <= 0) then begin
      sShowName := DropItem.Name; // ProcessItemName(DropItem.Name);

      if DropItem.OverlapCount > 1 then begin
        if g_ConfigClient.boOverLapItemNumOldShow then begin
          DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName + ' (' + IntToStr(DropItem.OverlapCount) + ')');
        end
        else begin
          DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName + 'x' + IntToStr(DropItem.OverlapCount))
        end;
      end
      else
        DropItem.NameImageInfo := CurrentFont.GetImageInfo(sShowName)
    end;

    if (Length(DropItem.NameImageInfo.ImageIndexs) > 0) and (DropItem.NameImageInfo.Width > 0) and
      (DropItem.NameImageInfo.Height > 0) then begin
      nX := X + HALFX - DropItem.NameImageInfo.Width div 2;
      nY := Y + HALFY - DropItem.NameImageInfo.Height * 2;

      CurrentFont.TextOut(nX - 1, nY, DropItem.NameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX + 1, nY, DropItem.NameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY - 1, DropItem.NameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY + 1, DropItem.NameImageInfo.ImageIndexs, clBlack);

      // 特殊物品变色 piaoyun 2013-09-10
      ShowItem := g_FileItemDB.Find(DropItem.Name);
      if (ShowItem <> nil) and ShowItem.boShowSpecial then
        CurrentFont.TextOut(nX, nY, DropItem.NameImageInfo.ImageIndexs, GetRGB(frmMain.nSpecialColor))
      else
        CurrentFont.TextOut(nX, nY, DropItem.NameImageInfo.ImageIndexs, DropItem.ItemColor);
    end;
  end;
end;

{-----------------------------------------------------------------------}

function TPlayScene.CanDrawTileMap:Boolean;
begin
  Result := False; //HZQ 20230524 添加一个默认返回值
end;

procedure TPlayScene.LoadSceneSurface(Sender:TObject);
begin

end;

procedure TPlayScene.LoadTileMapSurface(Sender:TObject);
begin

end;

procedure TPlayScene.ProcessActors();
var
  k:Integer;
  nX, nY:Integer;
  movetick:Boolean;
  evn:TClEvent;
  WaitActor, Actor:TActor;
  meff:TMagicEff;

  nIdx, Index:Integer;
  dwStepMoveTime:Integer;
  dwRunIntervalTime:Integer;
begin
  m_boCanDrawTileMap := False;
  if (g_MySelf = nil) then Exit;

  //WaitActor := nil; //HZQ
  //Actor := nil; //HZQ

  m_DrawGroundEffectList.Clear;
  m_DrawFlyList.Clear;
  m_DrawEventList.Clear;

  g_boDoFastFadeOut := False;
  movetick := False;
  g_nRenderCode := 0;

  {
  // 修改移动帧间隔 chongchong 2018-07-04 23:14:16
  dwStepMoveTime := _Max(100 - Round(100 * g_ClientConfig.nMoveSpeed / 1000) - (g_ClientConfig.dwIncMoveSpeedDecInterval * g_MySelf.m_nMoveSpeed) div 10, 1);

  // 修改移动帧间隔 chongchong 2018-07-04 23:14:16
  dwRunIntervalTime := g_ClientConfig.dwMoveFrameTime;

  if g_MySelf.m_nMoveSpeed <> 0 then
  begin
    dwRunIntervalTime := Max(dwRunIntervalTime - (g_ClientConfig.dwIncMoveSpeedDecInterval * g_MySelf.m_nMoveSpeed), 0);

    if dwStepMoveTime * 6 >= dwRunIntervalTime then
    begin
      dwStepMoveTime := (dwRunIntervalTime - 30) div 6;
    end;
  end;
  }

  // 修改移动帧间隔 chongchong 2018-07-04 23:14:16
  dwRunIntervalTime := Max(Integer(g_ClientConfig.dwMoveFrameTime) - (Integer(g_ClientConfig.dwIncMoveSpeedDecInterval) * g_MySelf.m_nMoveSpeed), 0);

  dwStepMoveTime := dwRunIntervalTime div 6;
  dwStepMoveTime := _Max(dwStepMoveTime - Round(dwStepMoveTime * g_ClientConfig.nMoveSpeed / 1000), 1);

  g_nRenderCode := 1;

  if TimeGetTime - m_dwMoveTime >= Cardinal(dwStepMoveTime) then begin
    m_dwMoveTime := TimeGetTime;
    movetick := True;
    Inc(m_nMoveStepCount);
    if m_nMoveStepCount > 1 then m_nMoveStepCount := 0;
  end;

  g_nRenderCode := 2;
  if MyGetTickCount - m_dwAniTime >= 50 then begin
    m_dwAniTime := MyGetTickCount;
    Inc(m_nAniCount);
    if m_nAniCount > 100000 then m_nAniCount := 0;
  end;

  {if MyGetTickCount - g_dwAniTilesTick > 100 then begin
    g_dwAniTilesTick := MyGetTickCount;
    Inc(g_nAniTiles);
  end;
  if g_nAniTiles >= 20 then g_nAniTiles := 0;  }

  g_nRenderCode := 3;

  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  {$IFEND}
  try
    nIdx := 0;
    while True do begin
      if g_boAppExit then break;
      if nIdx >= m_ActorList.Count then Break;
      Actor := TActor(m_ActorList.Items[nIdx]);
      Actor.m_boCanDraw := False;

      if not Actor.m_boDelActor then begin
        // 处理角色消息移到这里，移动速度更均匀 chongchong 2018-07-07 18:27:07
        //Actor.ProcMsg;
        if movetick then Actor.m_boLockEndFrame := False;

        if not Actor.m_boLockEndFrame then begin
          Actor.ProcMsg; // 处理角色消息

          if movetick then begin
            if Actor.DoMove(m_nMoveStepCount) then begin
              Inc(nIdx);
              Continue;
            end;
          end;

          Actor.Run;

          if Actor <> g_MySelf then Actor.ProcHurryMsg; // 处理魔法消息
        end;

        if Actor = g_MySelf then Actor.ProcHurryMsg; // 处理魔法消息

        if Actor.m_nWaitForRecogId <> 0 then begin
          if Actor.IsIdle then begin
            // DScreen.AddChatBoardString('m_nWaitForRecogId ' + Actor.m_sUserName + ' m_nWaitForRecogId:' + IntToStr(Actor.m_nWaitForRecogId), clWhite, clRed);
            DelChangeFace(Actor.m_nWaitForRecogId);
            WaitActor := NewActor(Actor.m_nWaitForRecogId, Actor.m_nCurrX, Actor.m_nCurrY, Actor.m_btDir, Actor.m_WaitForFeature, Actor.m_nWaitForStatus, False);

            // 修正神兽爬下后，名字不显示，非要移上去才显示  chongchong 2016-04-30
            WaitActor.m_sUserName := Actor.m_sUserName;
            WaitActor.m_btBodyColor := Actor.m_btBodyColor;
            WaitActor.m_nNameColor := Actor.m_nNameColor;
            //WaitActor.m_nCurrNameColor := Actor.m_nCurrNameColor;

            Actor.m_nWaitForRecogId := 0;
            Actor.m_dwDeleteTime := MyGetTickCount;
            Actor.m_boDelActor := True;
            Actor.m_boFreeActor := True;
            if g_MagicTarget = Actor then g_MagicTarget := WaitActor;
          end;
        end;
      end;

      if Actor.m_boDelActor then begin
        g_nRenderCode := 4;

        // 不能直接删除 nIdx 元素，会有问题 chongchong 2018-08-29 10:58:41
        // m_ActorList.Delete(nIdx);
        // m_DrawActorList.Remove(Actor);

        if not DoDelActor(Actor) then begin
          Inc(nIdx);
        end;

        g_nRenderCode := 5;
        // nCheckCode := 19;
        if Actor.m_boFreeActor then begin
          Actor.m_boGhost := True;
          AddFreeActorList(Actor);
        end;
        // nCheckCode := 20;       FreeActor
        g_nRenderCode := 6;

        g_nRenderCode := 7;
        // if g_SerieTarget = Actor then g_SerieTarget := nil;
      end
      else begin
        Inc(nIdx);
      end;
    end;

  except
    on E:Exception do begin
      DebugOutStr('101 Code:' + IntToStr(g_nRenderCode));
      DebugOutStr(E.Message);
    end;
  end;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.UnLock;
  {$IFEND}

  g_nRenderCode := 8;
  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.Lock;
  {$IFEND}
  try
    nIdx := 0;
    while True do begin
      if g_boAppExit then break;
      if nIdx >= m_GroundEffectList.Count then Break;
      meff := m_GroundEffectList[nIdx];
      if meff.m_boActive then begin
        if not meff.Run then begin
          g_nRenderCode := 9;

          m_GroundEffectList.Delete(nIdx);

          g_nRenderCode := 10;
          meff.m_dwGhostTick := MyGetTickCount;
          AddFreeEffectList(meff);
          Continue;
          g_nRenderCode := 11;
        end;
      end;
      Inc(nIdx);
    end;

  except
    on E:Exception do begin
      DebugOutStr('102');
      DebugOutStr(E.Message);
    end;
  end;
  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.UnLock;
  {$IFEND}

  g_nRenderCode := 12;

  {$IF IsMultiThreadRender = 1}
  m_EffectList.Lock;
  {$IFEND}
  try
    nIdx := 0;
    while True do begin
      if g_boAppExit then break;
      if nIdx >= m_EffectList.Count then Break;
      meff := m_EffectList.Items[nIdx];
      if meff.m_boActive then begin
        // ★★★★★★★★★增加 m_LastRunTick 延时，不然灵魂火符类的飞行效果不显示 chongchong 2016-12-17 ★★★★★★★★★
        if MyGetTickCount - meff.m_LastRunTick >= 10 then begin
          meff.m_LastRunTick := MyGetTickCount;
          if not meff.Run then begin
            g_nRenderCode := 13;
            m_EffectList.Delete(nIdx);
            g_nRenderCode := 14;
            meff.m_dwGhostTick := MyGetTickCount;
            AddFreeEffectList(meff);
            Continue;
          end;
        end;
      end;
      Inc(nIdx);
    end;

  except
    on E:Exception do begin
      DebugOutStr('103');
      DebugOutStr(E.Message);
    end;
  end;
  {$IF IsMultiThreadRender = 1}
  m_EffectList.UnLock;
  {$IFEND}

  g_nRenderCode := 15;
  {$IF IsMultiThreadRender = 1}
  m_FlyList.Lock;
  {$IFEND}

  try
    nIdx := 0;
    while True do begin
      if g_boAppExit then break;
      if nIdx >= m_FlyList.Count then Break;
      meff := m_FlyList.Items[nIdx];
      if meff.m_boActive then begin
        if not meff.Run then begin
          g_nRenderCode := 16;
          m_FlyList.Delete(nIdx);
          g_nRenderCode := 17;
          meff.m_dwGhostTick := MyGetTickCount;
          AddFreeEffectList(meff);
          Continue;
        end;
      end;
      Inc(nIdx);
    end;
  except
    on E:Exception do begin
      DebugOutStr('104');
      DebugOutStr(E.Message);
    end;
  end;
  {$IF IsMultiThreadRender = 1}
  m_FlyList.UnLock;
  {$IFEND}

  g_nRenderCode := 18;
  try
    g_nRenderCode := 19;
    EventMan.Execute;
    g_nRenderCode := 20;
  except
    on E:Exception do begin
      DebugOutStr('105-' + IntToStr(g_nRenderCode));
      DebugOutStr(E.Message);
    end;
  end;

  // 清除超过显示范围的物品数据
  g_nRenderCode := 21;

  if MyGetTickCount - m_ClearDropItemTick >= 100 then begin
    m_ClearDropItemTick := MyGetTickCount;
    ClearDropItem();
  end;

  g_nRenderCode := 22;
  // 清除超过显示范围的魔法数据

  {$IF IsMultiThreadRender = 1}
  EventMan.EventList.Lock;
  {$IFEND}
  try
    for k := EventMan.EventList.Count - 1 downto 0 do begin
      if g_boAppExit then break;
      evn := TClEvent(EventMan.EventList[k]);
      if ((abs(evn.m_nX - g_MySelf.m_nCurrX) > 30) and (abs(evn.m_nY - g_MySelf.m_nCurrY) > 30)) or (not evn.m_boVisible) then begin
        g_nRenderCode := 23;
        EventMan.EventList.Delete(k);
        evn.m_dwGhostTick := MyGetTickCount;
        AddFreeEventList(evn);
        g_nRenderCode := 24;
      end;
    end;
  except
    on E:Exception do begin
      DebugOutStr('106');
      DebugOutStr(E.Message);
    end;
  end;
  {$IF IsMultiThreadRender = 1}
  EventMan.EventList.UnLock;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.Lock;
  {$IFEND}
  for nIdx := 0 to m_GroundEffectList.Count - 1 do begin
    m_DrawGroundEffectList.Add(m_GroundEffectList.Items[nIdx]);
  end;
  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.UnLock;
  {$IFEND}
  // ------------------------------------------------------------------------------
  {$IF IsMultiThreadRender = 1}
  m_EffectList.Lock;
  {$IFEND}
  m_DrawEffectList.Clear;
  for nIdx := 0 to m_EffectList.Count - 1 do begin
    m_DrawEffectList.Add(m_EffectList.Items[nIdx]);
  end;
  {$IF IsMultiThreadRender = 1}
  m_EffectList.UnLock;
  {$IFEND}
  // ------------------------------------------------------------------------------
  {$IF IsMultiThreadRender = 1}
  m_FlyList.Lock;
  {$IFEND}
  for nIdx := 0 to m_FlyList.Count - 1 do begin
    m_DrawFlyList.Add(m_FlyList.Items[nIdx]);
  end;
  {$IF IsMultiThreadRender = 1}
  m_FlyList.UnLock;
  {$IFEND}
  // ------------------------------------------------------------------------------
  {$IF IsMultiThreadRender = 1}
  EventMan.EventList.Lock;
  {$IFEND}
  for nIdx := 0 to EventMan.EventList.Count - 1 do begin
    m_DrawEventList.Add(EventMan.EventList.Items[nIdx]);
  end;
  {$IF IsMultiThreadRender = 1}
  EventMan.EventList.UnLock;
  {$IFEND}
  // ------------------------------------------------------------------------------
  m_SortYDrawActorList.Count := 0;
  for nIdx := 0 to m_DrawActorList.Count - 1 do begin
    Actor := m_DrawActorList.Items[nIdx];

    if Actor.m_boDelActor then Continue;

    // 骑马 双人骑被邀请人不显示 chongchong 2013-10-14
    if (Actor.m_btHorse in [1, 2]) and (Actor.m_btDoubleHumHorse = 0) then Continue;

    DoSearchSortYDrawActtor(Actor.m_nRy - Actor.m_nDownDrawLevel, Index);
    m_SortYDrawActorList.Insert(Index, Actor);
  end;

  with Map.m_ClientRect do begin
    Left := g_MySelf.m_nRx - WIDTHGRIDCOUNT;
    Right := g_MySelf.m_nRx + WIDTHGRIDCOUNT;
    Top := g_MySelf.m_nRy - HEIGHTGRIDCOUNT;
    Bottom := g_MySelf.m_nRy + (HEIGHTGRIDCOUNT + 2);
    // DScreen.AddChatBoardString(Format('Left:%d Right:%d Top:%d Bottom:%d Width:%d Height:%d m_nShiftX:%d m_nShiftY:%d',
    // [Left, Right, Top, Bottom, (Right - Left) div 2, (Bottom - Top) div 2,g_MySelf.m_nShiftX,g_MySelf.m_nShiftY]), clWhite, clRed);
  end;

  // 骑马一步三格作个记号 chongchong 2014-08-06
  g_nRenderCode := 25;
  Map.UpdateMapPos(g_MySelf.m_nRx, g_MySelf.m_nRy);
  g_nRenderCode := 26;
  m_nDefXX := -UNITX * 2 - g_MySelf.m_nShiftX + AAX + 14; //修正追心刺推5格时，右下角方向施展左边以上面黑边 2021-01-25 m_nDefXX := -UNITX * 2 - g_MySelf.m_nShiftX + AAX + 14;
  m_nDefYY := -UNITY * 2 - g_MySelf.m_nShiftY;

  // 标记一下：追心刺推5格时，右下角方向施展左边以上面黑边 2021-01-25
  nX := UNITX * 3 + g_MySelf.m_nShiftX;
  nY := UNITY * 2 + g_MySelf.m_nShiftY;

  m_MapRect := Bounds(nX, nY, MAPSURFACEWIDTH, MAPSURFACEHEIGHT);

  if (m_nShiftX <> g_MySelf.m_nShiftX) or (m_nShiftY <> g_MySelf.m_nShiftY) or g_boDoorStatus or (not Map.m_boLoadOk) then begin
    m_boCanDrawTileMap := True;
    // DScreen.AddChatBoardString(Format('m_nShiftX:%d m_nShiftY:%d',[g_MySelf.m_nShiftX,g_MySelf.m_nShiftY]), clGreen, clWhite);
   // with Map.m_ClientRect do
    // DScreen.AddChatBoardString(Format('Left:%d Right:%d Top:%d Bottom:%d Width:%d Height:%d m_nShiftX:%d m_nShiftY:%d',
    // [Left, Right, Top, Bottom, (Right - Left) div 2, (Bottom - Top) div 2,g_MySelf.m_nShiftX,g_MySelf.m_nShiftY]), clWhite, clRed);
  end;
  if Map.m_boLoadOk then begin
    if (Map.m_ClientRect.Left <> Map.m_OldClientRect.Left) or (Map.m_ClientRect.Top <> Map.m_OldClientRect.Top) or (Map.m_sOldMap <> Map.m_sCurrentMap) then begin
      m_boCanDrawTileMap := True;
      Map.m_nOldBlockTop := Map.m_nBlockTop;
      Map.m_nOldBlockLeft := Map.m_nBlockLeft;
      if (Map.m_sOldMap <> Map.m_sCurrentMap) or (abs(Map.m_ClientRect.Left - Map.m_OldClientRect.Left) > 2) or
        (abs(Map.m_ClientRect.Top - Map.m_OldClientRect.Top) > 2) then begin
        m_nShiftX := g_MySelf.m_nShiftX;
        m_nShiftY := g_MySelf.m_nShiftY;
      end;

      Map.m_OldClientRect := Map.m_ClientRect;
      Map.m_sOldMap := Map.m_sCurrentMap;
    end;

    m_btDir := g_MySelf.m_btDir;
    m_nShiftX := g_MySelf.m_nShiftX;
    m_nShiftY := g_MySelf.m_nShiftY;
    m_nCurrentAction := g_MySelf.m_nCurrentAction;
  end;
  g_nRenderCode := 26;
  if (g_AutoSysMsg) and ((MyGetTickCount - g_AutoMsgTick) > g_AutoMsgTime) then begin
    g_AutoMsgTick := MyGetTickCount;
    FrmMain.SendSay(g_AutoMsg);
  end;
end;

procedure TPlayScene.RenderLight(Sender:TObject);
var
  I, j, k, ix, iy:Integer;
  d:TTexture;
  meff:TMagicEff;
  evn:TClEvent;
  Actor:TActor;

  nLight:Integer;
begin
  if g_MySelf = nil then Exit;
  GameCanvas.FillRect(Bounds(0, 0, SCREENWIDTH, SCREENHEIGHT), ARGB(255, g_nDarkValue, g_nDarkValue, g_nDarkValue));

  // drawingbottomline := SCREENHEIGHT - 150;

  try
    if Map.m_boLoadOk then begin
      for j := (Map.m_ClientRect.Top - Map.m_nBlockTop - 4) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do begin
        if j < 0 then begin
          continue;
        end;
        for i := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 5) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 5) do begin
          if (i >= 0) and (i < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
            nLight := Map.m_MArr[i, j].btLight;
            if nLight > 0 then begin
              nLight := _MIN(nLight, 5);
              d := g_WNewopUIImages.Images[210 + nLight];
              if d <> nil then begin
                ScreenXYfromMCXY(i + Map.m_nBlockLeft, j + Map.m_nBlockTop, ix, iy);
                GameCanvas.Draw(ix - d.Width div 2, iy - d.Height div 2, d.ClientRect, d, Blend_SrcColorAdd);
              end;
            end;
          end;
        end;
      end;
    end;

    if m_DrawActorList.Count > 0 then begin
      for k := 0 to m_DrawActorList.Count - 1 do begin
        actor := m_DrawActorList[k];
        if (actor = g_MySelf) or (actor.Light > 0) then begin
          nLight := actor.Light;
          nLight := _MAX(nLight, 0);
          nLight := _MIN(nLight, 5);
          d := g_WNewopUIImages.Images[210 + nLight];
          if d <> nil then begin
            ScreenXYfromMCXY(actor.m_nRx, actor.m_nRy, ix, iy);
            ix := ix + actor.m_nShiftX;
            iy := iy + actor.m_nShiftY;
            GameCanvas.Draw(ix - d.Width div 2, iy - d.Height div 2, d.ClientRect, d, Blend_SrcColorAdd);
          end;
        end;
      end;
    end
    else begin
      if g_MySelf <> nil then begin
        nLight := g_MySelf.Light;
        nLight := _MAX(nLight, 0);
        nLight := _MIN(nLight, 5);
        d := g_WNewopUIImages.Images[210 + nLight];
        if d <> nil then begin
          ScreenXYfromMCXY(g_MySelf.m_nRx, g_MySelf.m_nRy, ix, iy);
          ix := ix + g_MySelf.m_nShiftX;
          iy := iy + g_MySelf.m_nShiftY;
          GameCanvas.Draw(ix - d.Width div 2, iy - d.Height div 2, d.ClientRect, d, Blend_SrcColorAdd);
        end;
      end;
    end;

    for k := 0 to m_DrawEffectList.Count - 1 do begin
      //TNormalDrawEffect(meff)
      meff := TMagicEff(m_DrawEffectList[k]);
      if meff.Light > 0 then begin
        nLight := meff.Light;
        nLight := _MAX(nLight, 1);
        nLight := _MIN(nLight, 5);
        d := g_WNewopUIImages.Images[210 + nLight];
        if d <> nil then begin
          // 修正黑夜冰咆哮目标跑动时一卡卡
          // 原代码为: ScreenXYfromMCXY(meff.Rx, meff.Ry, ix, iy);

          if (meff.Rx > 0) or (meff.Ry > 0) then begin
            ScreenXYfromMCXY(meff.Rx, meff.Ry, ix, iy);
          end
            // +++++ 修正黑夜时，十步一杀不能点亮范围 chongchong 2015-03-04
          else
            ScreenXYfromMCXY(meff.targetx, meff.targety, ix, iy);

          // 修正黑夜丢帧bug chongchong 2015-03-04
          if (meff.TargetActor <> nil) and (meff.TargetActor is TActor) then begin
            ix := iX + TActor(meff.TargetActor).m_nShiftX;
            iY := iY + TActor(meff.TargetActor).m_nShiftY;
          end;

          GameCanvas.Draw(ix - d.Width div 2, iy - d.Height div 2, d.ClientRect, d, Blend_SrcColorAdd);
        end;
      end;
    end;

    for k := 0 to m_DrawEventList.Count - 1 do begin
      evn := TClEvent(m_DrawEventList[k]);
      if evn.m_nLight > 0 then begin
        nLight := evn.m_nLight;
        nLight := _MAX(nLight, 1);
        nLight := _MIN(nLight, 5);
        d := g_WNewopUIImages.Images[210 + nLight];
        if d <> nil then begin
          ScreenXYfromMCXY(evn.m_nX, evn.m_nY, ix, iy);
          GameCanvas.Draw(ix - d.Width div 2, iy - d.Height div 2, d.ClientRect, d, Blend_SrcColorAdd);
        end;
      end;
    end;

  except
    DebugOutStr('107');
  end;
end;

procedure TPlayScene.RenderGuide(Sender:TObject);
//var
  //I, nW: Integer;
  //d: TTexture;
begin
  if g_MySelf = nil then Exit;
  //  GameCanvas.FillRect(Bounds(0, 0, SCREENWIDTH, SCREENHEIGHT), ARGB(255, 160, 160, 160));
  GameCanvas.FillRect(Bounds(g_GuideX, g_GuideY, g_GuideR - g_GuideX, g_GuideB - g_GuideY), clWhite, 0, Blend_SrcColorAdd);
end;

procedure TPlayScene.RenderTileMap(Sender:TObject);
var
  I, J, nY, nX, nImgNumber:integer;
  d:TTexture;

  n, m, mmm, wunit, fridx, ani, anitick:Integer;
  boNeedUpdate:Boolean;
  GameImages:TGameImages;

  ErrorNum:Integer;
  MapYExt:Integer;
begin
  if g_boDepthStencil and g_boRenderTargetTileMap then
    GameCanvas.FillRect(g_RenderTarget[0].ClientRect, clBlack);
  // DScreen.AddChatBoardString('RenderTileMap', clGreen, clWhite);
  if not Map.m_boLoadOk then Exit;
  // 地图背景

  MapYExt := 10;

  nY := -UNITY * 2;
  for J := (Map.m_ClientRect.Top - Map.m_nBlockTop - 1) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + MapYExt) do begin
    nX := AAX + 14 - UNITX;

    for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 1) do begin
      if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (J >= 0) and (J < LOGICALMAPUNIT * 3) then begin
        // 加入韩地图支持 chongchong 2015-09-18
        // nImgNumber := (Map.m_MArr[I, j].wBkImg and $7FFF)
        if not Map.m_boENMap then
          nImgNumber := (Map.m_MArr[I, j].wBkImg and $7FFF)
        else
          nImgNumber := Map.m_MArr[I, j].wBkImg;

        if nImgNumber > 0 then begin
          if (I mod 2 = 0) and (j mod 2 = 0) then begin
            nImgNumber := nImgNumber - 1;
            d := nil;

            boNeedUpdate := False;

            GameImages := g_WTilesImages.GetGameImages(Map.m_MArr[I, j].btUnitBkImg, Map.m_boENMap);
            if GameImages <> nil then begin
              if g_boAutoUpdate then
                boNeedUpdate := GameImages.GetNeedUpdate(nImgNumber); // 检测是否需要更新图库
              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := GameImages.Grays[nImgNumber]
              else
                d := GameImages.Images[nImgNumber];
            end;

            if (d <> nil) then begin
              if (d.Width * d.Height > 4) then begin
                if (nX + d.Width > 0) and (nX <= SCREENWIDTH) and (nY + d.Height > 0) and (nY < MAPSURFACEHEIGHT) then
                  GameCanvas.Draw(nX, nY, d.ClientRect, d);
              end
              else begin
                if g_boAutoUpdate and boNeedUpdate then begin // 检测是否需要更新图库
                  g_boCanDrawTileMap := True;
                  // DebugOutStr('g_boCanDrawTileMap 1');
                end;
              end;
            end;
          end;
        end;
      end;
      Inc(nX, UNITX);
    end;
    Inc(nY, UNITY);
  end;

  // 地图中间层
  if Map.m_boNewMap then begin
    nY := -UNITY * 2;
  end
  else begin
    nY := -UNITY;
  end;

  for J := (Map.m_ClientRect.Top - Map.m_nBlockTop - 1) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + MapYExt) do begin
    nX := AAX + 14 - UNITX;

    for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 1) do begin
      if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (J >= 0) and (J < LOGICALMAPUNIT * 3) then begin
        nImgNumber := Map.m_MArr[I, J].wMidImg;

        if nImgNumber > 0 then begin
          nImgNumber := nImgNumber - 1;
          d := nil;
          boNeedUpdate := False;

          GameImages := g_WSmTilesImages.GetGameImages(Map.m_MArr[I, j].btUnitMidImg, Map.m_boENMap);
          if GameImages <> nil then begin
            if g_boAutoUpdate then
              boNeedUpdate := GameImages.GetNeedUpdate(nImgNumber); // 检测是否需要更新图库
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := GameImages.Grays[nImgNumber]
            else
              d := GameImages.Images[nImgNumber];
          end;

          if (d <> nil) then begin
            if (d.Width * d.Height > 4) then begin
              GameCanvas.Draw(nX, nY, d.ClientRect, d);
            end
            else begin
              if g_boAutoUpdate and boNeedUpdate then begin // 检测是否需要更新图库
                g_boCanDrawTileMap := True;
                // DebugOutStr('g_boCanDrawTileMap 2');
              end;
            end;
          end;
        end;
      end;
      Inc(nX, UNITX);
    end;
    Inc(nY, UNITY);
  end;

  ErrorNum := 0;
  try
    m := m_nDefYY - UNITY;
    ErrorNum := 1;
    for J := (Map.m_ClientRect.Top - Map.m_nBlockTop) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do begin
      if J < 0 then begin
        Inc(m, UNITY);
        Continue;
      end;
      n := m_nDefXX - UNITX * 2;

      ErrorNum := 2;
      for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 2) do begin
        if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (J >= 0) and (J < LOGICALMAPUNIT * 3) then begin
          ErrorNum := 3;

          // 加入韩地图支持 chongchong 2015-09-18
          fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF;
          {
          if not Map.m_boENMap then
            fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF
          else
            fridx := Map.m_MArr[I, j].wFrImg;
          }

          ErrorNum := 4;
          if fridx > 0 then begin
            ErrorNum := 5;
            ani := Map.m_MArr[I, J].btAniFrame;
            wunit := Map.m_MArr[I, J].btArea;
            ErrorNum := 6;
            if (ani and $80) > 0 then begin
              ani := ani and $7F;
            end;
            if ani > 0 then begin
              ErrorNum := 7;
              anitick := Map.m_MArr[I, J].btAniTick;
              fridx := fridx + (m_nAniCount mod (ani + (ani * anitick))) div (1 + anitick);
            end;
            ErrorNum := 8;
            if (Map.m_MArr[I, J].btDoorOffset and $80) > 0 then begin
              if (Map.m_MArr[I, J].btDoorIndex and $7F) > 0 then
                fridx := fridx + (Map.m_MArr[I, J].btDoorOffset and $7F);
            end;
            ErrorNum := 9;
            fridx := fridx - 1;

            ErrorNum := 10;
            d := GetObjs(wunit, fridx, Map.m_boENMap);
            ErrorNum := 11;
            if g_boAutoUpdate then // 检测是否需要更新图库
              boNeedUpdate := GetObjNeedUpdate(wunit, fridx)
            else
              boNeedUpdate := False;
            ErrorNum := 12;
            if (d <> nil) then begin
              if (d.Width * d.Height > 4) then begin
                if (d.Width = 48) and (d.Height = 32) then begin
                  ErrorNum := 13;
                  mmm := m + UNITY - d.Height;
                  ErrorNum := 14;
                  if (n + m_MapRect.Left + d.Width > 0) and (n + m_MapRect.Left <= SCREENWIDTH) and (mmm + m_MapRect.Top + d.Height > 0) and (mmm + m_MapRect.Top < MAPSURFACEHEIGHT) then begin
                    ErrorNum := 15;
                    GameCanvas.Draw(n + m_MapRect.Left, mmm + m_MapRect.Top, d.ClientRect, d);
                  end
                  else begin
                    ErrorNum := 17;
                    if mmm < MAPSURFACEHEIGHT then begin
                      ErrorNum := 18;
                      GameCanvas.Draw(n + m_MapRect.Left, mmm + m_MapRect.Top, d.ClientRect, d);
                    end;
                  end;
                  ErrorNum := 119;
                end;
              end
              else begin
                ErrorNum := 20;
                if g_boAutoUpdate and boNeedUpdate then begin // 检测是否需要更新图库
                  g_boCanDrawTileMap := True;
                end;
              end;
            end;
          end;
        end;
        Inc(n, UNITX);
      end;
      Inc(m, UNITY);
    end;
  except
    on E:Exception do begin
      DebugOutStr('RenderTileMap error-' + IntToStr(ErrorNum));
      DebugOutStr(E.Message);
    end;
  end;
  g_boDoorStatus := False;
end;

procedure TPlayScene.DrawTileMap(ClientRect:TRect);
var
  I, j, nY, nX, nPaintX, nPaintY, nOffsetX, nOffsetY, nImgNumber:Integer;
  d:TTexture;
  SrcRect:TRect;
  PaintRect:TRect;

  n, m, mmm, wunit, fridx, ani, anitick:Integer;
  boNeedUpdate:Boolean;

  dwTimeTick:LongWord;
  GameImages:TGameImages;
  boCheckTimeLimit:Boolean;

  ObjSize:TSize;
  ObjPt:TPoint;

  ErrorNum:Integer;
  MapYExt:Integer;
begin
  if (g_MySelf = nil) or (not Map.m_boLoadOk) then Exit;

  // 地图背景

  nY := -UNITY * 2;
  // 修复正定义UI下面一块透明时，最下边会黑边 (Map.m_ClientRect.Bottom - Map.m_nBlockTop + 1)修改为+10 chongchong 2014-11-02
  MapYExt := 10;
  for j := (Map.m_ClientRect.Top - Map.m_nBlockTop - 1) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + MapYExt) do begin
    nX := AAX + 14 - UNITX;

    // 再次优化chongchong 2014-09-29 21:52:05
    nOffsetY := nY - ClientRect.Top;
    nPaintY := _MAX(nOffsetY, 0) + ShakeY;
    if nPaintY > MAPSURFACEHEIGHT then begin
      Break;
    end;
    //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

    // 骑马一步三格黑边(Map.m_ClientRect.Right - Map.m_nBlockLeft + 1) 修改为 + 10 chongchong 2014-04-28
    for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 10) do begin
      // 再次优化chongchong 2014-09-29 21:52:05
      nOffsetX := nX - ClientRect.Left;
      nPaintX := _MAX(nOffsetX, 0) + ShakeX;
      if nPaintX > SCREENWIDTH then begin
        Break;
      end;
      //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

      if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
        // 加入韩地图支持 chongchong 2015-09-18
        // nImgNumber := (Map.m_MArr[I, j].wBkImg and $7FFF)
        if not Map.m_boENMap then
          nImgNumber := (Map.m_MArr[I, j].wBkImg and $7FFF)
        else
          nImgNumber := Map.m_MArr[I, j].wBkImg;

        if nImgNumber > 0 then begin
          if (I mod 2 = 0) and (j mod 2 = 0) then begin
            nImgNumber := nImgNumber - 1;
            d := nil;
            boNeedUpdate := False;

            // 优化地图绘制 chongchong 2014-03-25
            GameImages := g_WTilesImages.GetGameImages(Map.m_MArr[I, j].btUnitBkImg, Map.m_boENMap);
            if GameImages <> nil then begin
              if g_boAutoUpdate then
                boNeedUpdate := GameImages.GetNeedUpdate(nImgNumber); // 检测是否需要更新图库

              nOffsetX := nX - ClientRect.Left;
              nOffsetY := nY - ClientRect.Top;
              nPaintX := _MAX(nOffsetX, 0) + ShakeX;
              nPaintY := _MAX(nOffsetY, 0) + ShakeY;

              {
              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := GameImages.Grays[nImgNumber]
              else
                d := GameImages.Images[nImgNumber];
              }
              if GameImages.GetCachedImageSize(nImgNumber, ObjSize, ObjPt) then begin
                if ObjSize.cx * ObjSize.cy > 4 then begin
                  PaintRect := Rect(0, 0, ObjSize.cx, ObjSize.cy);
                  if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
                  if nOffsetY < 0 then PaintRect.Top := -nOffsetY;

                  if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                    (nPaintX + ObjSize.cx >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + ObjSize.cy >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                      d := GameImages.Grays[nImgNumber]
                    else
                      d := GameImages.Images[nImgNumber];
                  end;
                end
                else begin
                  if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                    g_boCanDrawTileMap := True;
                end;
              end
              else begin
                if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                  d := GameImages.Grays[nImgNumber]
                else
                  d := GameImages.Images[nImgNumber];
              end;
            end;

            if (d <> nil) then begin
              if d.Width * d.Height > 4 then begin
                PaintRect := d.ClientRect;
                if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
                if nOffsetY < 0 then PaintRect.Top := -nOffsetY;

                if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                  (nPaintX + d.Width >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + d.Height >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                  GameCanvas.Draw(nPaintX, nPaintY, PaintRect, d);

                  {$IF DRAW_MAP_TILE_INFO <> 0}
                  BoldTextOut(nPaintX, nPaintY, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                  BoldTextOut(nPaintX, nPaintY + 16, IntToStr(nImgNumber), clWhite, clBlack);
                  {$IFEND}
                end;
              end
              else begin
                if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                  g_boCanDrawTileMap := True;
              end;
            end;

          end;
        end;
      end;
      Inc(nX, UNITX);
    end;
    Inc(nY, UNITY);
  end;

  // 地图中间层
  if Map.m_boNewMap then begin
    nY := -UNITY * 2;
  end
  else begin
    nY := -UNITY;
  end;

  for j := (Map.m_ClientRect.Top - Map.m_nBlockTop - 1) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + MapYExt) do begin
    nX := AAX + 14 - UNITX;

    // 再次优化chongchong 2014-09-29 21:52:05
    nOffsetY := nY - ClientRect.Top;
    nPaintY := _MAX(nOffsetY, 0) + ShakeY;
    if nPaintY > MAPSURFACEHEIGHT then begin
      Break;
    end;
    //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

    for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 1) do begin
      // 再次优化chongchong 2014-09-29 21:52:05
      nOffsetX := nX - ClientRect.Left;
      nPaintX := _MAX(nOffsetX, 0) + ShakeX;
      if nPaintX > SCREENWIDTH then begin
        Break;
      end;
      //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

      if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
        nImgNumber := Map.m_MArr[I, j].wMidImg;
        if nImgNumber > 0 then begin
          nImgNumber := nImgNumber - 1;
          d := nil;
          boNeedUpdate := False;

          // 优化地图绘制 chongchong 2014-03-25
          GameImages := g_WSmTilesImages.GetGameImages(Map.m_MArr[I, j].btUnitMidImg, Map.m_boENMap);
          if GameImages <> nil then begin
            if g_boAutoUpdate then
              boNeedUpdate := GameImages.GetNeedUpdate(nImgNumber); // 检测是否需要更新图库

            nOffsetX := nX - ClientRect.Left;
            nOffsetY := nY - ClientRect.Top;
            nPaintX := _MAX(nOffsetX, 0) + ShakeX;
            nPaintY := _MAX(nOffsetY, 0) + ShakeY;

            {
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := GameImages.Grays[nImgNumber]
            else
              d := GameImages.Images[nImgNumber];
            }

            if GameImages.GetCachedImageSize(nImgNumber, ObjSize, ObjPt) then begin
              if ObjSize.cx * ObjSize.cy > 4 then begin
                PaintRect := Rect(0, 0, ObjSize.cx, ObjSize.cy);
                if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
                if nOffsetY < 0 then PaintRect.Top := -nOffsetY;

                if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                  (nPaintX + ObjSize.cx >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + ObjSize.cy >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                    d := GameImages.Grays[nImgNumber]
                  else
                    d := GameImages.Images[nImgNumber];
                end;
              end
              else begin
                if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                  g_boCanDrawTileMap := True;
              end;
            end
            else begin
              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := GameImages.Grays[nImgNumber]
              else
                d := GameImages.Images[nImgNumber];
            end;
          end;

          if d <> nil then begin
            if d.Width * d.Height > 4 then begin
              // nOffsetX := nX - ClientRect.Left;
              // nOffsetY := nY - ClientRect.Top;
              // nPaintX := _MAX(nOffsetX, 0) + ShakeX;
              // nPaintY := _MAX(nOffsetY, 0) + ShakeY;

              PaintRect := d.ClientRect;
              if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
              if nOffsetY < 0 then PaintRect.Top := -nOffsetY;
              if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                (nPaintX + d.Width >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + d.Height >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                GameCanvas.Draw(nPaintX, nPaintY, PaintRect, d);

                {$IF DRAW_MAP_TILE_INFO <> 0}
                BoldTextOut(nPaintX, nPaintY, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                BoldTextOut(nPaintX, nPaintY + 16, IntToStr(nImgNumber), clWhite, clBlack);
                {$IFEND}
              end;
            end
            else begin
              if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                g_boCanDrawTileMap := True;
            end;
          end;
        end;
      end;
      Inc(nX, UNITX);
    end;
    Inc(nY, UNITY);
  end;

  ErrorNum := 0;
  try
    ErrorNum := 1;
    m := m_nDefYY - UNITY;
    ErrorNum := 2;
    for j := (Map.m_ClientRect.Top - Map.m_nBlockTop) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do begin
      ErrorNum := 3;
      if j < 0 then begin
        Inc(m, UNITY);
        Continue;
      end;

      ErrorNum := 4;
      n := m_nDefXX - UNITX * 2;

      ErrorNum := 5;

      // +++++ 优化读取地图中Objects素材，占用cpu 2020-03-16 15:44:46
      mmm := m + UNITY - 32 + ShakeY;
      if (mmm + 32 < 0) then begin
        Inc(m, UNITY);
        Continue;
      end;

      if (mmm > MAPSURFACEHEIGHT) then begin
        Break;
      end;
      //---------------------- 优化结束

      for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 2) do begin
        // ++++ 优化读取地图中Objects素材，占用cpu 2020-03-16 15:44:46
        if (n + ShakeX + 48 < 0) then begin
          Inc(n, UNITX);
          Continue;
        end;

        if (n + ShakeX > SCREENWIDTH) then begin
          Break;
        end;
        //---------------------- 优化结束

        ErrorNum := 6;
        if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
          ErrorNum := 7;

          // 加入韩地图支持 chongchong 2015-09-18
          fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF;
          {
          if not Map.m_boENMap then
            fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF
          else
            fridx := Map.m_MArr[I, j].wFrImg;
          }

          ErrorNum := 8;
          if (fridx > 0) then begin
            ErrorNum := 9;
            ani := Map.m_MArr[I, j].btAniFrame;
            wunit := Map.m_MArr[I, j].btArea;

            ErrorNum := 10;
            if (ani and $80) > 0 then begin
              ani := ani and $7F;
            end;

            ErrorNum := 11;
            if ani > 0 then begin
              anitick := Map.m_MArr[I, j].btAniTick;
              fridx := fridx + (m_nAniCount mod (ani + (ani * anitick))) div (1 + anitick);
            end;

            ErrorNum := 12;
            if (Map.m_MArr[I, j].btDoorOffset and $80) > 0 then begin
              if (Map.m_MArr[I, j].btDoorIndex and $7F) > 0 then
                fridx := fridx + (Map.m_MArr[I, j].btDoorOffset and $7F);
            end;

            ErrorNum := 13;
            fridx := fridx - 1;

            // -----优化读取地图中Objects素材，占用cpu 2020-03-16 15:44:46
            // -----上面已经判断了是否在合适的坐标范围，这里不再读Size 2020-03-16 15:43:57
            {
            ErrorNum := 14;
            d := nil;

            ErrorNum := 15;
            if GetObjInfo(wunit, fridx, Map.m_boENMap, ObjSize, ObjPt) then
            begin
              ErrorNum := 16;
              if (ObjSize.cx = 48) and (ObjSize.cy = 32) then
              begin
                ErrorNum := 17;
                mmm := m + UNITY - ObjSize.cy + ShakeY;
                if (n + ShakeX + ObjSize.cx >= 0) and (n + ShakeX <= SCREENWIDTH) and
                  (mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT) then
                begin
                  ErrorNum := 18;
                  d := GetObjs(wunit, fridx, Map.m_boENMap);
                  ErrorNum := 19;
                end;
                ErrorNum := 20;
              end;
            end
            else
            }
            // ------------------------ 优化结束
            begin
              ErrorNum := 21;
              d := GetObjs(wunit, fridx, Map.m_boENMap);
            end;

            ErrorNum := 23;
            if (d <> nil) and (d.Width = 48) and (d.Height = 32) then begin
              ErrorNum := 24;
              mmm := m + UNITY - d.Height + ShakeY;

              ErrorNum := 25;

              // -----优化读取地图中Objects素材，占用cpu 2020-03-16 15:44:46

              //if (n + ShakeX + d.Width >= 0) and (n + ShakeX <= SCREENWIDTH) and
              //  (mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT) then
              begin
                ErrorNum := 26;
                GameCanvas.Draw(n + ShakeX, mmm, d);

                {$IF DRAW_MAP_TILE_INFO <> 0}
                if wunit = 0 then
                  BoldTextOut(n + ShakeX, mmm, 'Objects', clWhite, clBlack)
                else
                  BoldTextOut(n + ShakeX, mmm, 'Objects' + IntToStr(wunit + 1), clWhite, clBlack);
                BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                {$IFEND}
                ErrorNum := 27;
              end;
            end;

          end;
        end;
        Inc(n, UNITX);
      end;
      Inc(m, UNITY);
      //OutputDebugString(PChar(IntToStr(Count)));
    end;
  except
    on E:Exception do begin
      DebugOutStr('DrawTileMap error-' + IntToStr(ErrorNum));
      DebugOutStr(E.Message);
    end;
  end;
end;

procedure TPlayScene.DrawTileEIMap(ClientRect:TRect);
var
  I, j, nY, nX, nPaintX, nPaintY, nOffsetX, nOffsetY, nImgNumber, nFileIdx:Integer;
  d:TTexture;
  PaintRect:TRect;

  boNeedUpdate:Boolean;

  GameImages:TGameImages;

  ObjSize:TSize;
  ObjPt:TPoint;
  m, n, ani, mmm:Integer;
  blend:Boolean;
  MapYExt:Integer;
begin
  if (g_MySelf = nil) or (not Map.m_boLoadOk) then Exit;

  // 地图背景
  nY := -UNITY * 2;
  // 修复正定义UI下面一块透明时，最下边会黑边 (Map.m_ClientRect.Bottom - Map.m_nBlockTop + 1)修改为+10 chongchong 2014-11-02

  {
  if g_ConfigClient.boFullScreenDrawMap then
    MapYExt := 10
  else
    MapYExt := 1;
  }

  MapYExt := 10;

  for j := (Map.m_ClientRect.Top - Map.m_nBlockTop - 1) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + MapYExt) do begin
    nX := AAX + 14 - UNITX;

    // 再次优化chongchong 2014-09-29 21:52:05
    nOffsetY := nY - ClientRect.Top;
    nPaintY := _MAX(nOffsetY, 0) + ShakeY;
    if nPaintY > MAPSURFACEHEIGHT then begin
      Break;
    end;
    //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

    // 骑马一步三格黑边(Map.m_ClientRect.Right - Map.m_nBlockLeft + 1) 修改为 + 10 chongchong 2014-04-28
    for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + MapYExt) do begin

      // 再次优化chongchong 2014-09-29 21:52:05
      nOffsetX := nX - ClientRect.Left;
      nPaintX := _MAX(nOffsetX, 0) + ShakeX;
      if nPaintX > SCREENWIDTH then begin
        Break;
      end;
      //-------------------- 优化结束 chongchong 2014-09-29 21:52:05

      if (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
        nFileIdx := Map.m_MArrEIMapTiteInfo[I div 2, J div 2].btFileIdx;
        nImgNumber := Map.m_MArrEIMapTiteInfo[I div 2, J div 2].wTileIdx;

        if nImgNumber >= 0 then begin
          if (I mod 2 = 0) and (j mod 2 = 0) and
            (nImgNumber >= 0) and (nImgNumber <> 65535) and (nFileIdx < 73) and (nFileIdx >= 0) then begin
            d := nil;
            boNeedUpdate := False;

            // 优化地图绘制 chongchong 2014-03-25
            GameImages := g_EIMapTitleArr[nFileIdx];
            if GameImages <> nil then begin
              if g_boAutoUpdate then
                boNeedUpdate := GameImages.GetNeedUpdate(nImgNumber); // 检测是否需要更新图库

              nOffsetX := nX - ClientRect.Left;
              nOffsetY := nY - ClientRect.Top;
              nPaintX := _MAX(nOffsetX, 0) + ShakeX;
              nPaintY := _MAX(nOffsetY, 0) + ShakeY;

              if GameImages.GetCachedImageSize(nImgNumber, ObjSize, ObjPt) then begin
                if ObjSize.cx * ObjSize.cy > 4 then begin
                  PaintRect := Rect(0, 0, ObjSize.cx, ObjSize.cy);
                  if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
                  if nOffsetY < 0 then PaintRect.Top := -nOffsetY;

                  if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                    (nPaintX + ObjSize.cx >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + ObjSize.cy >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                    if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                      d := GameImages.Grays[nImgNumber]
                    else
                      d := GameImages.Images[nImgNumber];
                  end;
                end
                else begin
                  if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                    g_boCanDrawTileMap := True;
                end;
              end
              else begin
                if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                  d := GameImages.Grays[nImgNumber]
                else
                  d := GameImages.Images[nImgNumber];
              end;
            end;

            if (d <> nil) then begin
              if d.Width * d.Height > 4 then begin
                PaintRect := d.ClientRect;
                if nOffsetX < 0 then PaintRect.Left := -nOffsetX;
                if nOffsetY < 0 then PaintRect.Top := -nOffsetY;

                if (PaintRect.Left < PaintRect.Right) and (PaintRect.Top < PaintRect.Bottom) and
                  (nPaintX + d.Width >= 0) and (nPaintX <= SCREENWIDTH) and (nPaintY + d.Height >= 0) and (nPaintY <= MAPSURFACEHEIGHT) then begin
                  GameCanvas.Draw(nPaintX, nPaintY, PaintRect, d);
                end;
              end
              else begin
                if g_boAutoUpdate and boNeedUpdate then // 检测是否需要更新图库
                  g_boCanDrawTileMap := True;
              end;
            end;

          end;
        end;
      end;
      Inc(nX, UNITX);
    end;
    Inc(nY, UNITY);
  end;

  // 背景层 chongchong 2015-10-01
  try
    m := m_nDefYY - UNITY;
    for j := (Map.m_ClientRect.Top - Map.m_nBlockTop) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do begin
      if j < 0 then begin
        Inc(m, UNITY);
        Continue;
      end;
      n := m_nDefXX - UNITX * 2;
      for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 2) do begin
        if I >= Length(Map.m_MArrEIMapInfo) then Continue;
        if J >= Length(Map.m_MArrEIMapInfo[0]) then Continue;

        nFileIdx := Map.m_MArrEIMapInfo[I, J].btFileIdx2;
        nImgNumber := Map.m_MArrEIMapInfo[I, J].wObj1;

        if (nImgNumber <> 65535) and (nFileIdx < 75) and (nFileIdx > 0) and ((nFileIdx <> 0) or (nImgNumber <> 0)) then begin
          blend := False;
          if ((nFileIdx = 11) or (nFileIdx = 26) or (nFileIdx = 41) or (nFileIdx = 56) or (nFileIdx = 71)) then begin
            ani := map.m_MArrEIMapInfo[i, j].btObj1Ani;
            if (ani <> 255) and (ani > 0) then begin
              if (ani and $80) > 0 then begin
                blend := True;
              end;

              if nFileIdx = 11 then begin
                if ani > 10 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 5)
                else if ani > 0 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod ani)
              end
              else if nFileIdx = 26 then begin
                if ani = 190 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 14)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
              end
              else if nFileIdx = 41 then begin
                if ani = 184 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 16)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
              end
              else if nFileIdx = 56 then begin
                if ani >= 136 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
                else if ani >= 69 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 5)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 6)
              end
              else if nFileIdx = 71 then begin
                nImgNumber := nImgNumber + (m_nAniCount div 2 mod 6)
              end;
            end;
          end;

          // 优化地图绘制 chongchong 2014-03-25
          d := nil;

          GameImages := g_EIMapTitleArr[nFileIdx];
          if GameImages <> nil then begin
            if GameImages.GetCachedImageSize(nImgNumber, ObjSize, ObjPt) then begin
              if (ObjSize.cx * ObjSize.cy >= 4) then begin
                mmm := m + UNITY - ObjSize.cy + ShakeY;
                if (n + ShakeX + ObjSize.cx >= 0) and (n + ShakeX <= SCREENWIDTH) and
                  (mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT) then begin
                  d := GameImages.Images[nImgNumber];
                end;
              end;
            end
            else begin
              d := GameImages.Images[nImgNumber];
            end;
          end;

          if (d <> nil) and (d.Width = 48) and (d.Height = 32) and (not blend) then begin
            mmm := m + UNITY - d.Height + ShakeY;

            // 优化注释的代码为下面两行 chongchong 2014-03-25
            if (n + ShakeX + d.Width >= 0) and (n + ShakeX <= SCREENWIDTH) and
              (mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT) then begin
              GameCanvas.Draw(n + ShakeX, mmm, d);
            end;
          end;
        end;

        //--------------------------------------------------------------------
        nFileIdx := Map.m_MArrEIMapInfo[I, J].btFileIdx1;
        nImgNumber := Map.m_MArrEIMapInfo[I, J].wObj2;

        if (nImgNumber <> 65535) and (nFileIdx < 75) and (nFileIdx > 0) and ((nFileIdx <> 0) or (nImgNumber <> 0)) then begin
          blend := False;

          if ((nFileIdx = 11) or (nFileIdx = 26) or (nFileIdx = 41) or (nFileIdx = 56) or (nFileIdx = 71)) then begin
            ani := map.m_MArrEIMapInfo[i, j].btObj2Ani;
            if (ani <> 255) and (ani > 0) then begin
              if (ani and $80) > 0 then begin
                blend := True;
              end;

              if nFileIdx = 11 then begin
                if ani > 10 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 5)
                else if ani > 0 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod ani)
              end
              else if nFileIdx = 26 then begin
                if ani = 190 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 14)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
              end
              else if nFileIdx = 41 then begin
                if ani = 184 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 16)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
              end
              else if nFileIdx = 56 then begin
                if ani >= 136 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 10)
                else if ani >= 69 then
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 5)
                else
                  nImgNumber := nImgNumber + (m_nAniCount div 2 mod 6)
              end
              else if nFileIdx = 71 then begin
                nImgNumber := nImgNumber + (m_nAniCount div 2 mod 6)
              end;
            end;
          end;

          // 优化地图绘制 chongchong 2014-03-25
          d := nil;

          GameImages := g_EIMapTitleArr[nFileIdx];
          if GameImages <> nil then begin
            if GameImages.GetCachedImageSize(nImgNumber, ObjSize, ObjPt) then begin
              if (ObjSize.cx * ObjSize.cy >= 4) then begin
                mmm := m + UNITY - ObjSize.cy + ShakeY;
                if (n + ShakeX + ObjSize.cx >= 0) and (n + ShakeX <= SCREENWIDTH) and
                  (mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT) then begin
                  d := GameImages.Images[nImgNumber];
                end;
              end;
            end
            else begin
              d := GameImages.Images[nImgNumber];
            end;
          end;

          if (d <> nil) and (d.Width = 48) and (d.Height = 32) and (not blend) then begin
            mmm := m + UNITY - d.Height + ShakeY;
            // 优化注释的代码为下面两行 chongchong 2014-03-25
            if (n + ShakeX + d.Width >= 0) and (n + ShakeX <= SCREENWIDTH) and
              (mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT) then begin
              GameCanvas.Draw(n + ShakeX, mmm, d);
            end;
          end;
        end;

        Inc(n, UNITX);
      end;
      Inc(m, UNITY);
      //OutputDebugString(PChar(IntToStr(Count)));
    end;
  except
    on E:Exception do begin
      //DebugOutStr('DrawTileEIMap error-' + IntToStr(ErrorNum));
      DebugOutStr(E.Message);
    end;
  end;
end;

procedure TPlayScene.DrawDropItemEffect(nX, nY:Integer; DropItem:pTDropItem; IsBelowItem:Boolean);
var
  d:TTexture;
  GameImages:TGameImages;
  oX, oY:Integer;
begin
  if g_ConfigDlg.ConfigCheckeds[ckHideItemEffect] and (g_ClientConfig.boHideItemEffect) then Exit;

  if (DropItem.ItemEffect.FileIndex < 0) or (DropItem.ItemEffect.FileIndex >= g_EffectImageList.Count) then Exit;
  if DropItem.ItemEffect.ImageCount = 0 then Exit;
  if DropItem.ItemEffect.BelowItem <> IsBelowItem then Exit;
  if DropItem.ItemEffect.Time <= 0 then Exit;

  GameImages := TGameImages(g_EffectImageList.Objects[DropItem.ItemEffect.FileIndex]);
  if GameImages = nil then Exit;

  if MyGetTickCount - DropItem.ItemEffectTick >= DropItem.ItemEffect.Time then begin
    Inc(DropItem.ItemEffectFrame);
    DropItem.ItemEffectTick := MyGetTickCount;
  end;

  if DropItem.ItemEffectFrame < DropItem.ItemEffect.StartIndex then
    DropItem.ItemEffectFrame := DropItem.ItemEffect.StartIndex
  else if DropItem.ItemEffectFrame >= DropItem.ItemEffect.StartIndex + DropItem.ItemEffect.ImageCount then
    DropItem.ItemEffectFrame := DropItem.ItemEffect.StartIndex;

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := GameImages.GetCachedGrayImage(DropItem.ItemEffectFrame, oX, oY)
  else
    d := GameImages.GetCachedImage(DropItem.ItemEffectFrame, oX, oY);

  if d = nil then Exit;

  if DropItem.ItemEffect.DrawCenter then begin
    nX := nX + DropItem.ItemEffect.OffsetX + (DropItem.TextureWidth - d.Width) div 2;
    nY := nY + DropItem.ItemEffect.OffsetY + (DropItem.TextureHeight - d.Height) div 2;
  end
  else begin
    nX := nX + DropItem.ItemEffect.OffsetX + oX;
    nY := nY + DropItem.ItemEffect.OffsetY + oY;
  end;

  if DropItem.ItemEffect.NoBlend then
    GameCanvas.Draw(nX, nY, d.ClientRect, d)
  else
    GameCanvas.DrawBlend(nX, nY, d.ClientRect, d);
end;

procedure TPlayScene.DrawDropItemValueEffect(nX, nY:Integer; DropItem:pTDropItem);
var
  d:TTexture;
  GameImages:TGameImages;
  oX, oY:Integer;
begin
  if not g_ClientConfig.boShowDropValueItemEff then Exit;
  if not g_ConfigDlg.ConfigCheckeds[ckShowValueItemEffect] then Exit;

  if not g_ConfigClient.boShowValueItemEffect then Exit;
  if g_ConfigClient.dwValueItemEffectCount = 0 then Exit;
  if g_ConfigClient.dwValueItemEffectPlayTime <= 0 then Exit;

  if not DropItem.boValueItem then Exit;

  GameImages := g_WNewopUIImages;

  if MyGetTickCount - DropItem.ValueItemEffectTick >= g_ConfigClient.dwValueItemEffectPlayTime then begin
    Inc(DropItem.ValueItemEffectFrame);
    DropItem.ValueItemEffectTick := MyGetTickCount;
  end;

  if DropItem.ValueItemEffectFrame < 0 then begin
    DropItem.ValueItemEffectFrame := 0
  end else if Cardinal(DropItem.ValueItemEffectFrame) >= g_ConfigClient.dwValueItemEffectCount then begin
    DropItem.ValueItemEffectFrame := 0;
  end;

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := GameImages.GetCachedGrayImage(DropItem.ValueItemEffectFrame + Integer(g_ConfigClient.dwValueItemEffectIndex), oX, oY)
  else
    d := GameImages.GetCachedImage(DropItem.ValueItemEffectFrame + Integer(g_ConfigClient.dwValueItemEffectIndex), oX, oY);

  if d = nil then Exit;

  nX := nX + g_ConfigClient.nValueItemEffectOffsetX + oX;
  nY := nY + g_ConfigClient.nValueItemEffectOffsetY + oY;

  GameCanvas.Draw(nX, nY, d.ClientRect, d);
end;

procedure TPlayScene.DrawScene(Sender:TObject);
var
  I, II, j, k, m, mmm, n, wunit, ani, anitick, fridx, ax, ay, idx, ix, iy, nIdx, nCode, nX, nY:Integer;
  blend:Boolean;

  Actor:TActor;
  d:TTexture;
  evn:TClEvent;
  meff:TMagicEff;
  DropItem:pTDropItem;
  FocusCret:TActor;
  MagicTarget:TActor;

  ObjSize:TSize;
  ObjPt:TPoint;
  Count:Integer;

  nFileIdx:Integer;
  GameImages:TGameImages;
  bo:Boolean;
  _m:Integer;
  _m_nBlockTop:Integer;
  PointDropItemList:TPointDropItemList;

  pHintInfo:PNearActorHintInfo;

begin
  m_NearActorHintMgr.CleaerHint();
   
  if TimeGetTime - m_dwLockTargetEffectLastTick >= 100 then begin
    m_dwLockTargetEffectLastTick := TimeGetTime;
    Inc(m_nLockTargetEffectFrame);
  end;

  if (m_nLockTargetEffectFrame < 1190) or (m_nLockTargetEffectFrame > 1196) then begin
    m_nLockTargetEffectFrame := 1190;
  end;

  nCode := 0;
  Actor := nil;
  try
    g_nRenderCode := 29;
    m := m_nDefYY - UNITY;
    Count := 0;
    for j := (Map.m_ClientRect.Top - Map.m_nBlockTop) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do begin
      if (not m_boCanDraw) or g_boAppExit then break;
      if j < 0 then begin
        Inc(m, UNITY);
        Continue;
      end;

      g_nRenderCode := 30;
      n := m_nDefXX - UNITX * 2;
      nCode := 1;

      // 再次优化chongchong 2020-04-13 19:18:54
      //if n > SCREENWIDTH then  Break;

      g_nRenderCode := 31;
      for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 2) do begin
        // 再次优化chongchong 2020-04-13 19:18:54
        //mmm := m + UNITY + ShakeY;
        //if mmm > MAPSURFACEHEIGHT then Break;

        if (not m_boCanDraw) or g_boAppExit then break;
        if Map.m_boLoadOk and (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
          if Map.m_boEIMap then begin
            nCode := 7;

            nFileIdx := Map.m_MArrEIMapInfo[I, J].btFileIdx2;
            fridx := Map.m_MArrEIMapInfo[I, J].wObj1;

            nCode := 8;
            if (fridx <> 65535) and (nFileIdx < 75) and (nFileIdx > 0) and ((nFileIdx <> 0) or (fridx <> 0)) then begin
              blend := False;
              if ((nFileIdx = 11) or (nFileIdx = 26) or (nFileIdx = 41) or (nFileIdx = 56) or (nFileIdx = 71)) then begin
                ani := map.m_MArrEIMapInfo[i, j].btObj1Ani;
                if (ani <> 255) and (ani > 0) then begin
                  if (ani and $80) > 0 then begin
                    blend := True;
                  end;

                  if nFileIdx = 11 then begin
                    if ani > 10 then
                      fridx := fridx + (m_nAniCount div 2 mod 5)
                    else if ani > 0 then
                      fridx := fridx + (m_nAniCount div 2 mod ani)
                  end
                  else if nFileIdx = 26 then begin
                    if ani = 190 then
                      fridx := fridx + (m_nAniCount div 2 mod 14)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                  end
                  else if nFileIdx = 41 then begin
                    if ani = 184 then
                      fridx := fridx + (m_nAniCount div 2 mod 16)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                  end
                  else if nFileIdx = 56 then begin
                    if ani >= 136 then
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                    else if ani >= 69 then
                      fridx := fridx + (m_nAniCount div 2 mod 5)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 6)
                  end
                  else if nFileIdx = 71 then begin
                    fridx := fridx + (m_nAniCount div 2 mod 6)
                  end;
                end;
              end;

              // 优化地图绘制 chongchong 2014-03-25
              GameImages := g_EIMapTitleArr[nFileIdx];
              if GameImages <> nil then begin
                if GameImages.GetCachedImageSize(fridx, ObjSize, ObjPt) then begin
                  if not blend then begin
                    if ((ObjSize.cx <> 48) or (ObjSize.cy <> 32)) and (ObjSize.cx > 0) and (ObjSize.cx * ObjSize.cy > 4) then begin
                      mmm := m + UNITY - ObjSize.cy + ShakeY;
                      if (n + ShakeX + ObjSize.cx > 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + ObjSize.cy > 0) and (mmm < MAPSURFACEHEIGHT) then begin
                        d := GameImages.Images[fridx];
                        if (d <> nil) then begin
                          GameCanvas.Draw(n + ShakeX, mmm, d);

                          {$IF DRAW_MAP_TILE_INFO <> 0}
                          BoldTextOut(n + ShakeX, mmm, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                          BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                          {$IFEND}
                        end;
                      end;
                    end;
                  end
                  else begin
                    if (ObjSize.cx > 0) and (ObjSize.cx * ObjSize.cy > 4) then begin
                      //mmm := m + ObjPt.Y - 68 + ShakeY;
                      mmm := m + UNITY - ObjSize.cy + ShakeY;
                      if ((n + ObjPt.X + ShakeX + ObjSize.cx >= 0) and (n + ObjPt.Y + ShakeX <= SCREENWIDTH)) and
                        ((mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT)) then begin
                        d := GameImages.Images[fridx];

                        if d <> nil then
                          GameCanvas.DrawBlend(n + ShakeX, mmm, d);
                      end;
                    end;
                  end;
                end
                else begin
                  d := GameImages.GetCachedImage(fridx, ax, ay);
                  if d <> nil then begin
                    if not blend then begin
                      if (d.Width <> 48) or (d.Height <> 32) then begin
                        mmm := m + UNITY - d.Height + ShakeY;
                        if (n + ShakeX + d.Width > 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + d.Height > 0) and (mmm < MAPSURFACEHEIGHT) then begin
                          GameCanvas.Draw(n + ShakeX, mmm, d);

                          {$IF DRAW_MAP_TILE_INFO <> 0}
                          BoldTextOut(n + ShakeX, mmm, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                          BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                          {$IFEND}
                        end;
                      end;
                    end
                    else begin
                      if (d.Width * d.Height > 4) then begin
                        mmm := m + ay - 68 + ShakeY;
                        if ((n + ax - 2 + ShakeX + d.Width >= 0) and (n + ax - 2 + ShakeX <= SCREENWIDTH)) and
                          ((mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT)) then
                          GameCanvas.DrawBlend(n + ax - 2 + ShakeX, mmm, d);
                      end;
                    end;
                  end;
                end;
              end;
            end;

            //--------------------------------------------------------------------
            nFileIdx := Map.m_MArrEIMapInfo[I, J].btFileIdx1;
            fridx := Map.m_MArrEIMapInfo[I, J].wObj2;

            nCode := 8;
            if (fridx <> 65535) and (nFileIdx < 75) and (nFileIdx > 0) and ((nFileIdx <> 0) or (fridx <> 0)) then begin
              blend := False;

              if ((nFileIdx = 11) or (nFileIdx = 26) or (nFileIdx = 41) or (nFileIdx = 56) or (nFileIdx = 71)) then begin
                ani := map.m_MArrEIMapInfo[i, j].btObj2Ani;
                if (ani <> 255) and (ani > 0) then begin
                  if (ani and $80) > 0 then begin
                    blend := True;
                  end;

                  if nFileIdx = 11 then begin
                    if ani > 10 then
                      fridx := fridx + (m_nAniCount div 2 mod 5)
                    else if ani > 0 then
                      fridx := fridx + (m_nAniCount div 2 mod ani)
                  end
                  else if nFileIdx = 26 then begin
                    if ani = 190 then
                      fridx := fridx + (m_nAniCount div 2 mod 14)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                  end
                  else if nFileIdx = 41 then begin
                    if ani = 184 then
                      fridx := fridx + (m_nAniCount div 2 mod 16)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                  end
                  else if nFileIdx = 56 then begin
                    if ani >= 136 then
                      fridx := fridx + (m_nAniCount div 2 mod 10)
                    else if ani >= 69 then
                      fridx := fridx + (m_nAniCount div 2 mod 5)
                    else
                      fridx := fridx + (m_nAniCount div 2 mod 6)
                  end
                  else if nFileIdx = 71 then begin
                    fridx := fridx + (m_nAniCount div 2 mod 6)
                  end;
                end;
              end;

              // 优化地图绘制 chongchong 2014-03-25
              GameImages := g_EIMapTitleArr[nFileIdx];
              if GameImages <> nil then begin
                if GameImages.GetCachedImageSize(fridx, ObjSize, ObjPt) then begin
                  if (ObjSize.cx * ObjSize.cy > 4) then begin
                    if not blend then begin
                      if ((ObjSize.cx <> 48) or (ObjSize.cy <> 32)) and (ObjSize.cx > 0) then begin
                        mmm := m + UNITY - ObjSize.cy + ShakeY;
                        if (n + ShakeX + ObjSize.cx >= 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + ObjSize.cy >= 0) and (mmm < MAPSURFACEHEIGHT) then begin
                          d := GameImages.Images[fridx];
                          if (d <> nil) then begin
                            GameCanvas.Draw(n + ShakeX, mmm, d);

                            {$IF DRAW_MAP_TILE_INFO <> 0}
                            BoldTextOut(n + ShakeX, mmm, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                            BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                            {$IFEND}
                          end;
                        end;
                      end;
                    end
                    else begin
                      if (ObjSize.cx > 0) then begin
                        mmm := m + ObjPt.Y - 68 + ShakeY;

                        if ((n + ObjPt.X + ShakeX + ObjSize.cx >= 0) and (n + ObjPt.Y + ShakeX <= SCREENWIDTH)) and
                          ((mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT)) then begin
                          d := GameImages.Images[fridx];

                          if d <> nil then
                            GameCanvas.DrawBlend(n + ShakeX, mmm, d);

                          //GameCanvas.FillRect(Rect(n + ShakeX, mmm, n + ShakeX + d.Width, mmm + d.Height), clred);
                        end;
                      end;
                    end;
                  end;
                end
                else begin
                  d := GameImages.GetCachedImage(fridx, ax, ay);
                  if (d <> nil) and (d.Width * d.Height > 4) then begin
                    if not blend then begin
                      if (d.Width <> 48) or (d.Height <> 32) then begin
                        mmm := m + UNITY - d.Height + ShakeY;
                        if (n + ShakeX + d.Width > 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + d.Height > 0) and (mmm < MAPSURFACEHEIGHT) then begin
                          GameCanvas.Draw(n + ShakeX, mmm, d);

                          {$IF DRAW_MAP_TILE_INFO <> 0}
                          BoldTextOut(n + ShakeX, mmm, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                          BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                          {$IFEND}
                        end;
                      end;
                    end
                    else begin
                      mmm := m + ay - 68 + ShakeY;
                      if ((n + ax - 2 + ShakeX + d.Width >= 0) and (n + ax - 2 + ShakeX <= SCREENWIDTH)) and
                        ((mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT)) then
                        GameCanvas.DrawBlend(n + ax - 2 + ShakeX, mmm, d);
                    end;
                  end;
                end;
              end;
            end;
          end
          else begin
            if Map.m_MArr[I, j].wAniTiles > 0 then begin
              fridx := -1;
              ani := Map.m_MArr[I, j].btAniTilesFrame;
              anitick := Map.m_MArr[I, j].btAniTilesTick;

              if Map.m_MArr[I, j].btAniType = 32 then begin
                if (Map.m_MArr[I, j].wAniTiles <> 991) and
                  (Map.m_MArr[I, j].wAniTiles <> 999) and
                  (Map.m_MArr[I, j].wAniTiles <> 1016) and
                  (Map.m_MArr[I, j].wAniTiles <> 1026) and
                  (Map.m_MArr[I, j].wAniTiles <> 1007) and
                  (Map.m_MArr[I, j].wAniTiles <> 1045) then
                  fridx := ((m_nAniCount * 25) div anitick mod ani) * anitick;
              end
              else if Map.m_MArr[I, j].btAniType = 33 then begin
                if (Map.m_MArr[I, j].wAniTiles <> 2575) and
                  (Map.m_MArr[I, j].wAniTiles <> 2586) and
                  (Map.m_MArr[I, j].wAniTiles <> 2564) and
                  (Map.m_MArr[I, j].wAniTiles <> 2285) and
                  (Map.m_MArr[I, j].wAniTiles <> 2291) and
                  (Map.m_MArr[I, j].wAniTiles <> 2303) and
                  (Map.m_MArr[I, j].wAniTiles <> 2315) and
                  (Map.m_MArr[I, j].wAniTiles <> 2329) and
                  (Map.m_MArr[I, j].wAniTiles <> 2341) and
                  (Map.m_MArr[I, j].wAniTiles <> 2353) and
                  (Map.m_MArr[I, j].wAniTiles <> 2366) and
                  (Map.m_MArr[I, j].wAniTiles <> 2379) and
                  (Map.m_MArr[I, j].wAniTiles <> 2403) and
                  (Map.m_MArr[I, j].wAniTiles <> 2432) and
                  (Map.m_MArr[I, j].wAniTiles <> 2495) and
                  (Map.m_MArr[I, j].wAniTiles <> 2463) and
                  (Map.m_MArr[I, j].wAniTiles <> 2529)
                  then
                  fridx := ((m_nAniCount * 25) div anitick mod ani) * 306;
              end
              else begin
                fridx := fridx + (m_nAniCount mod (ani + (ani * anitick))) div (1 + anitick);
              end;

              if fridx >= 0 then begin
                // 优化地图绘制 chongchong 2014-03-25
                //d := g_WAniTilesImages1.Images[Map.m_MArr[I, j].wAniTiles + fridx];
                d := nil;
                if g_WAniTilesImages1.GetCachedImageSize(Map.m_MArr[I, j].wAniTiles + fridx, ObjSize, ObjPt) then begin
                  if ObjSize.cx * ObjSize.cy > 4 then begin
                    mmm := m + UNITY + ShakeY;
                    if (n + ShakeX + ObjSize.cx > 0) and (mmm + ObjSize.cy > 0) and (n + ShakeX < SCREENWIDTH) and (mmm < MAPSURFACEHEIGHT) then begin
                      d := g_WAniTilesImages1.Images[Map.m_MArr[I, j].wAniTiles + fridx];
                    end;
                  end;
                end
                else
                  d := g_WAniTilesImages1.Images[Map.m_MArr[I, j].wAniTiles + fridx];

                if (d <> nil) and (d.Width * d.Height > 4) then begin
                  mmm := m + UNITY + ShakeY; // - d.Height;
                  if (n + ShakeX + d.Width > 0) and (mmm + d.Height > 0) and (n + ShakeX < SCREENWIDTH) and (mmm < MAPSURFACEHEIGHT) then begin
                    GameCanvas.Draw(n + ShakeX, mmm, d);
                    {$IF DRAW_MAP_TILE_INFO <> 0}
                    BoldTextOut(n + ShakeX, mmm, ExtractFileNameOnly(GameImages.FileName), clWhite, clBlack);
                    BoldTextOut(n + ShakeX, mmm + 16, IntToStr(Map.m_MArr[I, j].wAniTiles + fridx), clWhite, clBlack);
                    {$IFEND}
                  end;
                end;
              end;
            end;

            // 加入韩地图支持 chongchong 2015-09-18
            fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF;
            {
            if not Map.m_boENMap then
              fridx := (Map.m_MArr[I, j].wFrImg) and $7FFF
            else
              fridx := Map.m_MArr[I, j].wFrImg;
            }
            if fridx > 0 then begin
              blend := False;

              wunit := Map.m_MArr[I, j].btArea;
              ani := Map.m_MArr[I, j].btAniFrame;

              if (ani and $80) > 0 then begin
                blend := True;
                ani := ani and $7F;
              end;

              if ani > 0 then begin
                anitick := Map.m_MArr[I, j].btAniTick;
                fridx := fridx + (m_nAniCount mod (ani + (ani * anitick))) div (1 + anitick);
              end;
              if (Map.m_MArr[I, j].btDoorOffset and $80) > 0 then begin
                if (Map.m_MArr[I, j].btDoorIndex and $7F) > 0 then
                  fridx := fridx + (Map.m_MArr[I, j].btDoorOffset and $7F);
              end;

              fridx := fridx - 1;
              nCode := 2;

              // 优化地图绘制 chongchong 2014-03-26
              nCode := 201;
              if GetObjInfo(wunit, fridx, Map.m_boENMap, ObjSize, ObjPt) then begin
                if (ObjSize.cx * ObjSize.cy > 4) and (ObjSize.cx > 0) then begin
                  nCode := 202;
                  if not blend then begin
                    nCode := 203;
                    // 增加图片大于0的判断，有些文件长宽增为负值 and (ObjSize.cx *  ObjSize.cy > 4) and (ObjSize.cx > 0) chongchong 2018-07-26 10:23:46
                    if ((ObjSize.cx <> 48) or (ObjSize.cy <> 32)) then begin
                      nCode := 204;
                      mmm := m + UNITY - ObjSize.cy + ShakeY;
                      nCode := 205;
                      if (n + ShakeX + ObjSize.cx >= 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + ObjSize.cy >= 0) and (mmm < MAPSURFACEHEIGHT) then begin
                        nCode := 206;
                        d := GetObjsEx(wunit, fridx, Map.m_boENMap, ax, ay);
                        nCode := 207;
                        if (d <> nil) then begin
                          nCode := 208;
                          //if not ((wunit = 24) and  (fridx= 6107)) then
                          GameCanvas.Draw(n + ShakeX, mmm, d);

                          {$IF DRAW_MAP_TILE_INFO <> 0}
                          if wunit = 0 then
                            BoldTextOut(n + ShakeX, mmm, 'Objects', clWhite, clBlack)
                          else
                            BoldTextOut(n + ShakeX, mmm, 'Objects' + IntToStr(wunit + 1), clWhite, clBlack);
                          BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                          {$IFEND}

                          nCode := 209;
                        end;
                      end;
                    end;
                  end
                  else begin
                    nCode := 210;

                    // 修正地图 nd5072-nd5074  nd2013 灯光位置错误  chongchong 2015-10-11
                    if (wUnit = 19) and (fridx >= 443) and (fridx <= 450) and (not SameText(g_sMapName, 'nd2013')) and (not SameText(g_sMapName, 'n0')) and
                      (not SameText(g_sMapName, 'nd024')) then begin
                      ObjPt.Y := -140;
                    end
                      // 地图n6(124 159) Object20 452-459 资源设置的偏移不对，此处单独修复 chongchong 2015-01-25
                    else if (wunit = 19) and (fridx >= 452) and (fridx <= 459) then begin
                      ObjPt.Y := -140;
                    end;

                    nCode := 211;
                    mmm := m + ObjPt.Y - 68 + ShakeY;

                    if ((n + ObjPt.X - 2 + ShakeX + ObjSize.cx >= 0) and (n + ObjPt.Y - 2 + ShakeX <= SCREENWIDTH)) and
                      ((mmm + ObjSize.cy >= 0) and (mmm <= MAPSURFACEHEIGHT)) then begin
                      nCode := 212;
                      d := GetObjsEx(wunit, fridx, Map.m_boENMap, ax, ay);
                      nCode := 213;
                      if d <> nil then begin
                        nCode := 214;
                        if (wUnit = 19) and (fridx >= 443) and (fridx <= 450) then begin
                          GameCanvas.DrawBlend(n + ax - 4 + ShakeX, mmm, d);
                        end
                        else if (wunit = 19) and (fridx >= 452) and (fridx <= 459) then begin
                          GameCanvas.DrawBlend(n + ax - 70 + ShakeX, mmm, d);
                        end
                        else
                          GameCanvas.DrawBlend(n + ax - 2 + ShakeX, mmm, d);

                        nCode := 215;
                      end;
                    end;
                  end;
                end;
              end
              else begin
                nCode := 216;
                d := GetObjsEx(wunit, fridx, Map.m_boENMap, ax, ay);
                nCode := 217;
                if (d <> nil) and (d.Width * d.Height > 4) then begin
                  nCode := 218;
                  if not blend then begin
                    nCode := 219;
                    if (d.Width <> 48) or (d.Height <> 32) then begin
                      nCode := 220;
                      mmm := m + UNITY - d.Height + ShakeY;
                      nCode := 221;
                      if (n + ShakeX + d.Width > 0) and (n + ShakeX <= SCREENWIDTH) and (mmm + d.Height > 0) and (mmm < MAPSURFACEHEIGHT) then begin
                        GameCanvas.Draw(n + ShakeX, mmm, d);

                        {$IF DRAW_MAP_TILE_INFO <> 0}
                        if wunit = 0 then
                          BoldTextOut(n + ShakeX, mmm, 'Objects', clWhite, clBlack)
                        else
                          BoldTextOut(n + ShakeX, mmm, 'Objects' + IntToStr(wunit + 1), clWhite, clBlack);
                        BoldTextOut(n + ShakeX, mmm + 16, IntToStr(fridx), clWhite, clBlack);
                        {$IFEND}

                        nCode := 224;
                      end;
                    end;
                  end
                  else begin
                    nCode := 226;
                    mmm := m + ay - 68 + ShakeY;
                    nCode := 227;
                    if ((n + ax - 2 + ShakeX + d.Width >= 0) and (n + ax - 2 + ShakeX <= SCREENWIDTH)) and
                      ((mmm + d.Height >= 0) and (mmm <= MAPSURFACEHEIGHT)) then
                      GameCanvas.DrawBlend(n + ax - 2 + ShakeX, mmm, d);
                    nCode := 228;
                  end;
                end;
              end;
            end;
          end;
        end; // if Map.m_boLoadOk and (I >= 0) and (I < LOGICALMAPUNIT * 3) and (j >= 0) and (j < LOGICALMAPUNIT * 3) then begin
        // GameCanvas.FrameRect(Bounds(n, m, UNITX, UNITY), clRed);
        Inc(n, UNITX);
      end; // for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2)

      g_nRenderCode := 32;
      nCode := 3;
      // 修正UI（隐藏掉主UI）无遮挡地图及怪物时，从上往下跑怪物要跑多点才出 (+LONGHEIGHT_IMAGE) 2019-09-03 23:58:52
      if (j <= (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE)) and (not g_boServerChanging) then begin
        ///////////////////////////////////////////////代码块AAAA下移///////////////////////////
        for k := 0 to m_DrawEventList.Count - 1 do begin
          if (not m_boCanDraw) or g_boAppExit then break;
          evn := TClEvent(m_DrawEventList[k]);
          if not evn.m_boVisible then Continue;

          // 修正烟花播放时在所有人物最前面 chongchong 2015-01-11
          if evn.m_nEventType in [ET_FIREFLOWER_1..ET_FIREFLOWER_8] then Continue; // 不画烟花

          // // 修复地图魔法特效 MAPEFFECT map3_8_001 38 48 42 910 23 10 100 1 5 (WIL编号42 是Magic2.wzl; 坐标 38 48) 处显示错误 chongchong 2015-11-03
          if j = (evn.m_nY - Map.m_nBlockTop) then begin
            nCode := 4;
            evn.DrawEvent((evn.m_nX - Map.m_ClientRect.Left) * UNITX + m_nDefXX, m);
            nCode := 5;
          end;
        end;
        ///////////////////////////////////////////////代码块AAAA下移///////////////////////////

        g_nRenderCode := 33;
        g_DropItemsMgr.Lock;
        try
          if g_boDrawDropItem and (g_DropItemsMgr.Count > 0) then begin
            // 显示地面物品外形
            nIdx := g_DropItemsMgr.GetItemListIndexByY(J + Map.m_nBlockTop);
            if nIdx >= g_DropItemsMgr.Count then nIdx := g_DropItemsMgr.Count - 1;
            for k := nIdx downto 0 do begin
              if (not m_boCanDraw) or g_boAppExit then break;

              PointDropItemList := g_DropItemsMgr.Items[k];

              if PointDropItemList.Count = 0 then Continue;

              if PointDropItemList.Y < J + Map.m_nBlockTop then Break;
              if PointDropItemList.Y <> J + Map.m_nBlockTop then Continue;

              for II := 0 to PointDropItemList.DrawCount - 1 do begin
                DropItem := PointDropItemList.DrawItems[II];

                if not DropItem.Visible then Continue;

                if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                  d := g_WDnItemImages.Grays[DropItem.looks]
                else if DropItem = g_FocusItem then
                  d := g_WDnItemImages.Brights[DropItem.looks]
                else
                  d := g_WDnItemImages.Images[DropItem.looks];

                DropItem.ItemTexture := d;

                if (d <> nil) and (d.Width * d.Height > 4) then begin
                  ix := (DropItem.X - Map.m_ClientRect.Left) * UNITX + m_nDefXX;
                  iy := m;
                  nX := ix + HALFX - (d.Width div 2) + ShakeX;
                  nY := iy + HALFY - (d.Height div 2) + ShakeY;
                  DropItem.TextureWidth := d.Width;
                  DropItem.TextureHeight := d.Height;
                  nCode := 6;

                  DrawDropItemEffect(nX, nY, DropItem, True);

                  GameCanvas.Draw(nX,
                    nY,
                    d.ClientRect,
                    d);
                  nCode := 7;

                  DrawDropItemEffect(nX, nY, DropItem, False);

                  // 物品闪光那里搞来的，不知道对不对 2020-07-12 23:02:56
                  nX := (DropItem.X - Map.m_ClientRect.Left) * UNITX + m_nDefXX + SOFFX;
                  nY := (DropItem.Y - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + SOFFY;
                  DrawDropItemValueEffect(nX, nY, DropItem);
                end;
              end;
            end;
          end;
        finally
          g_DropItemsMgr.UnLock;
        end;

        nIdx := 0;
        g_nRenderCode := 34;

        if DoSearchSortYDrawActtor(J + Map.m_nBlockTop, mmm) then begin
          for k := mmm to m_SortYDrawActorList.Count - 1 do begin
            Actor := m_SortYDrawActorList[k];

            if Actor = nil then Continue;

            if (J <> Actor.m_nRy - Map.m_nBlockTop - Actor.m_nDownDrawLevel) then Break;

            Actor.m_boCanDraw := True;
            Actor.m_nSayX := (Actor.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + Actor.m_nShiftX + 24;

            // 骑马 - 血条/血量 坐标位置 chongchong 2013-10-12
            if Actor.m_btHorse = 0 then begin
              if Actor.m_boDeath then
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 60 + (Actor.m_nDownDrawLevel * UNITY)
              else
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 95 + (Actor.m_nDownDrawLevel * UNITY);

              // 血灵教主血条偏移 chongchong 2014-09-11
              if Actor is TXueLingLeaderMonster then begin
                Actor.m_nSayY := Actor.m_nSayY - 100;
                Actor.m_nSayX := Actor.m_nSayX + 35;
              end
                // 雕像血条上移 chongchong 2015-01-27
              else if (Actor is TStatuaryNpcActor) then begin
                if TStatuaryNpcActor(Actor).boScaleShow then
                  Actor.m_nSayY := Actor.m_nSayY - 35
                else
                  Actor.m_nSayY := Actor.m_nSayY - 28
              end

                // 修正33,34的npc名字不居中
              else if (Actor is TNpcActor) and (Actor.m_wAppearance in [33, 34]) then begin
                Actor.m_nSayX := Actor.m_nSayX - 18;
              end;
            end else if Actor.m_btHorse = 1 then begin
              if Actor.m_boDeath then
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 100 + (Actor.m_nDownDrawLevel * UNITY)
              else
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 135 + (Actor.m_nDownDrawLevel * UNITY);

              // 修正官方双人马血条偏移 2019-08-26 21:40:05
              if Actor.m_btDoubleHumHorse = 0 then begin
                case Actor.m_btDir of
                  DR_UP:begin
                      Actor.m_nSayY := Actor.m_nSayY - 5;
                    end;
                  DR_UPRIGHT:begin
                      Actor.m_nSayX := Actor.m_nSayX - 5;
                      Actor.m_nSayY := Actor.m_nSayY + 5;
                    end;
                  DR_RIGHT:begin
                      Actor.m_nSayX := Actor.m_nSayX - 10;
                    end;
                  DR_DOWNRIGHT:begin
                      Actor.m_nSayX := Actor.m_nSayX - 5;
                    end;
                  DR_DOWN:begin
                      Actor.m_nSayY := Actor.m_nSayY - 5;
                    end;
                  DR_DOWNLEFT:begin
                      Actor.m_nSayX := Actor.m_nSayX + 5;
                    end;
                  DR_LEFT:begin
                      Actor.m_nSayX := Actor.m_nSayX + 10;
                    end;
                  DR_UPLEFT:begin
                      Actor.m_nSayX := Actor.m_nSayX + 10;
                      Actor.m_nSayY := Actor.m_nSayY + 5;
                    end;
                end;
              end;
            end else if Actor.m_btHorse = 2 then begin
              if Actor.m_boDeath then
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 85 + (Actor.m_nDownDrawLevel * UNITY)
              else
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 120 + (Actor.m_nDownDrawLevel * UNITY);
            end else if Actor.m_btHorse > 0 then begin
              if Actor.m_boDeath then
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 85 + (Actor.m_nDownDrawLevel * UNITY)
              else
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 120 + (Actor.m_nDownDrawLevel * UNITY);
            end;

            { TODO -opiaoyun -c修改 : 内怪隐藏怪物排除城墙 Actor.m_btRace in [98, 99]【2013-5-27】 }
            if (PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and
              Actor.m_boDeath and (not (Actor.m_btRace in [0, 1, 98, 99])) then Continue;

            // 修复女战王不能隐藏尸体 chongchong 2016-01-22
            if (PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and
              Actor.m_boDeath and (Actor.m_btRace = RC_PLAYOBJECT) and Actor.m_boPlayMoster then Continue;

            nX := (Actor.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + ShakeX;
            nY := m + (Actor.m_nDownDrawLevel * UNITY) + ShakeY;

            // 顶戴花翎指定绘制顺序 chongchong 2014-09-02
            nCode := 8;

            if (Actor = g_LockTarget) and (not Actor.m_boDeath) and (not Actor.m_boGhost) and (tick_diff(g_dwLockTargetTick, MyGetTickCount) <= 8000) then begin
              Actor.DrawLockTargetEffect(m_nLockTargetEffectFrame, nX, nY);
            end;

            // 放在这里而非Run中，用来处理站立时速度快，跑时速度慢 chongchong 2017-11-27
            if Actor.CheckLoadActorIcon then Actor.LoadActorIcons;
            Actor.ShowIcons(True);

            // 绘制 PLAYEFFECT 特效 chongchong 2016-10-29s
            Actor.DrawPlayEffect(nX, nY, True);

            nCode := 81;

            Actor.DrawSelfEffect(nX, nY, True);

            // 自定义技能绘制，自己绘制两次chongchong 2015-03-20
            Actor.DrawChr(nX, nY, False, True);

            Actor.DrawSelfEffect(nX, nY, False);

            if g_ConfigClient.boShowExploreItemIcon and (g_ConfigClient.btExploreItemIconShowType = 0) then begin
              Actor.DrawExploreItemEffect(nX, nY);
            end;

            // 放在这里而非Run中，用来处理站立时速度快，跑时速度慢 绘制 PLAYEFFECT 特效 chongchong 2016-10-29s
            if Actor.CheckLoadPlayEffect then Actor.LoadPlayEffectSurface;
            Actor.DrawPlayEffect(nX, nY, False);

            if g_ConfigDlg.ConfigCheckeds[ckObjectHintEffect] then begin
                //Actor.DrawNearObjectHintEffect(nX, nY);
                m_NearActorHintMgr.AddActorHint(nX, nY, 0);
                pHintInfo := m_NearActorHintMgr.GetHintInfo(-1);
                if not Actor.GetNearObjectHintInfo(pHintInfo.X, pHintInfo.Y, pHintInfo.nFriendFlag) then begin
                    m_NearActorHintMgr.DeleteLatestHint;
                end;
            end;

            nCode := 9;
            // GameCanvas.FrameRect(Bounds(g_nScreenCenterX - 6, g_nScreenCenterY - 6, 6, 6), clRed);
             // GameCanvas.FrameRect(Bounds((Actor.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX - 3, m + (Actor.m_nDownDrawLevel * UNITY) - 3, 3, 3), clLime);
          end;
        end;

        {
        for k := 0 to m_DrawActorList.Count - 1 do
        begin
          if (not m_boCanDraw) or g_boAppExit then break;
          Actor := m_DrawActorList[k];
          if Actor.m_boDelActor then Continue;

          // 骑马 双人骑被邀请人不显示 chongchong 2013-10-14
          if (Actor.m_btHorse in [1, 2]) and (Actor.m_btDoubleHumHorse = 0) then Continue;

          if (j = Actor.m_nRy - Map.m_nBlockTop - Actor.m_nDownDrawLevel) then
          begin
          end;
        end;
        }

        g_nRenderCode := 35;
        for k := 0 to m_DrawFlyList.Count - 1 do begin
          if (not m_boCanDraw) or g_boAppExit then break;
          meff := TMagicEff(m_DrawFlyList[k]);
          nCode := 10;
          if j = (meff.ry - Map.m_nBlockTop) then
            meff.DrawEff();
          nCode := 11;
        end;
        g_nRenderCode := 36;
      end; // for I := (Map.m_ClientRect.Left - Map.m_nBlockLeft - 2) to (Map.m_ClientRect.Right - Map.m_nBlockLeft + 2) do begin

      Inc(m, UNITY);
    end;

    (*
    // 修复地图魔法特效 MAPEFFECT map3_8_001 38 48 42 910 23 10 100 1 5 (WIL编号42 是Magic2.wzl; 坐标 38 48) 处显示错误
    ///////////////////////////////////////////////代码块AAAA移动这里///////////////////////////
    m := m_nDefYY - UNITY;
    Count := 0;
    for j := (Map.m_ClientRect.Top - Map.m_nBlockTop) to (Map.m_ClientRect.Bottom - Map.m_nBlockTop + LONGHEIGHT_IMAGE) do
    begin
      if (not m_boCanDraw) or g_boAppExit then break;
      if j < 0 then
      begin
        Inc(m, UNITY);
        Continue;
      end;

      g_nRenderCode := 30;
      n := m_nDefXX - UNITX * 2;
      nCode := 1;

      g_nRenderCode := 32;
      nCode := 3;
      if (j <= (Map.m_ClientRect.Bottom - Map.m_nBlockTop)) and (not g_boServerChanging) then
      begin
        for k := 0 to m_DrawEventList.Count - 1 do
        begin
          if (not m_boCanDraw) or g_boAppExit then break;
          evn := TClEvent(m_DrawEventList[k]);
          if not evn.m_boVisible then Continue;

          // 修正烟花播放时在所有人物最前面 chongchong 2015-01-11
          if evn.m_nEventType in [ET_FIREFLOWER_1..ET_FIREFLOWER_8] then Continue;   // 不画烟花
          if j = (evn.m_nY - Map.m_nBlockTop) then
          begin
            nCode := 4;
            evn.DrawEvent((evn.m_nX - Map.m_ClientRect.Left) * UNITX + m_nDefXX, m);
            nCode := 5;
          end;
        end;
      end;

      Inc(m, UNITY);
    end;
    ///////////////////////////////////////////////代码块AAAA移动这里//////////////////////////
    *)

  except
    on E:Exception do begin
      if nCode = 8 then
        DebugOutStr('107 Code:' + IntToStr(nCode) + ' ' + Actor.m_sUserName)
      else
        DebugOutStr('107 Code:' + IntToStr(nCode));
      DebugOutStr(e.Message);
    end;
  end;

  g_nRenderCode := 37;
  if (not g_boServerChanging) and m_boCanDraw and (not g_boAppExit) then begin

    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    FocusCret := g_FocusCret;
    MagicTarget := g_MagicTarget;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}

    try
      // if not g_boCheckBadMapMode then
      if (g_MySelf.m_nState and $00800000 = 0) then
        g_MySelf.DrawChr((g_MySelf.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + ShakeX, (g_MySelf.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + ShakeY, True, False);

      { TODO -ochongchong -c修改 : 焦点人物翅膀移上去后，比人物的亮(原因在于Actor.DrawChr绘制时已经判断过g_FocusCret) 【2013-08-16】 }
      // 再次修改，当NPC被遮挡时，鼠标移上去不能完整显示NPC chongchong 2014-04-23
      if (FocusCret <> nil) then begin
        if IsValidActorEx(FocusCret) and (FocusCret <> g_MySelf) then
          if (not (PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost])) or
            ((PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and (not FocusCret.m_boDeath)) then
            if (FocusCret.m_nState and $00800000 = 0) then
              FocusCret.DrawChr(
                (FocusCret.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX,
                (FocusCret.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY, True, False);

        if g_ConfigClient.boShowExploreItemIcon and (g_ConfigClient.btExploreItemIconShowType = 1) then begin
          FocusCret.DrawExploreItemEffect((FocusCret.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX, (FocusCret.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY);
        end;
      end;

      if (MagicTarget <> nil) then begin
        if IsValidActorEx(MagicTarget) and (MagicTarget <> g_MySelf) then
          if (not (PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost])) or
            ((PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and (not MagicTarget.m_boDeath)) then
            if MagicTarget.m_nState and $00800000 = 0 then
              MagicTarget.DrawChr(
                (MagicTarget.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + ShakeX,
                (MagicTarget.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + ShakeY, True, False);
      end;

    except
      DebugOutStr('108');
    end;

    // chongchong 2013-09-10
    if (GameCanvas <> nil) and (CurrentFont <> nil) then begin
      if g_FBTime > 0 then begin
        CurrentFont.TextOut(41, 106, g_sFBTime, clBlack); //81
        CurrentFont.TextOut(40, 105, g_sFBTime, clLime); //80
      end
      else if g_MirrorMapTime > 0 then begin
        CurrentFont.TextOut(41, 106, g_sMirrorMapTime, clBlack); //81
        CurrentFont.TextOut(40, 105, g_sMirrorMapTime, clLime); //80
      end else if g_TimeMapTime > 0 then begin //HZQ
        CurrentFont.TextOut(41, 106, g_sTimeMapTime, clBlack); //81 HZQ //这个分支是从新文件补全的
        CurrentFont.TextOut(40, 105, g_sTimeMapTime, clLime); //80 HZQ
      end;

      if g_FBExitTime > 0 then begin
        nX := (g_nScreenWidth - CurrentFont.TextWidth(g_sFBExitTime)) div 2;
        CurrentFont.TextOut(nX + 1, 81, g_sFBExitTime, clBlack);
        CurrentFont.TextOut(nX, 80, g_sFBExitTime, clYellow);
      end
      else if g_FBFailTime > 0 then begin
        nX := (g_nScreenWidth - CurrentFont.TextWidth(g_sFBFailTime)) div 2;
        CurrentFont.TextOut(nX + 1, 81, g_sFBFailTime, clBlack);
        CurrentFont.TextOut(nX, 80, g_sFBFailTime, clYellow);
      end;
    end;
  end;
  g_nRenderCode := 38;
end;

procedure TPlayScene.DrawEffect(Sender:TObject);
var
  k:Integer;
  Actor:TActor;
  meff:TMagicEff;
begin
  if (not m_boCanDraw) or g_boAppExit then Exit;
  g_nRenderCode := 39;
  try
    for k := 0 to m_DrawActorList.Count - 1 do begin
      if (not m_boCanDraw) or g_boAppExit then break;
      Actor := m_DrawActorList[k];
      if (not Actor.m_boDelActor) then
        Actor.DrawEff(
          (Actor.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX,
          (Actor.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY);
    end;
  except
    on E:Exception do begin
      DebugOutStr('109');
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 40;
  try
    for k := 0 to m_DrawEffectList.Count - 1 do begin
      if (not m_boCanDraw) or g_boAppExit then break;
      meff := TMagicEff(m_DrawEffectList[k]);
      meff.DrawEff();
    end;
  except
    on E:Exception do begin
      DebugOutStr('110');
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 41;
end;

procedure TPlayScene.DrawPreviewMonItemEffect(nX, nY:Integer; Item:PClientPreviewMonItem; IsBelowItem:Boolean);
var
  d:TTexture;
  GameImages:TGameImages;
  oX, oY:Integer;
begin
  if g_ConfigDlg.ConfigCheckeds[ckHideItemEffect] and (g_ClientConfig.boHideItemEffect) then Exit;

  if (Item.ItemEffect.FileIndex < 0) or (Item.ItemEffect.FileIndex >= g_EffectImageList.Count) then Exit;
  if Item.ItemEffect.ImageCount = 0 then Exit;
  if Item.ItemEffect.BelowItem <> IsBelowItem then Exit;
  if Item.ItemEffect.Time <= 0 then Exit;

  GameImages := TGameImages(g_EffectImageList.Objects[Item.ItemEffect.FileIndex]);
  if GameImages = nil then Exit;

  if MyGetTickCount - Item.ItemEffectTick >= Item.ItemEffect.Time then begin
    Inc(Item.ItemEffectFrame);
    Item.ItemEffectTick := MyGetTickCount;
  end;

  if Item.ItemEffectFrame < Item.ItemEffect.StartIndex then
    Item.ItemEffectFrame := Item.ItemEffect.StartIndex
  else if Item.ItemEffectFrame >= Item.ItemEffect.StartIndex + Item.ItemEffect.ImageCount then
    Item.ItemEffectFrame := Item.ItemEffect.StartIndex;

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := GameImages.GetCachedGrayImage(Item.ItemEffectFrame, oX, oY)
  else
    d := GameImages.GetCachedImage(Item.ItemEffectFrame, oX, oY);

  if d = nil then Exit;

  if Item.ItemEffect.DrawCenter then begin
    nX := nX + Item.ItemEffect.OffsetX + (Item.TextureWidth - d.Width) div 2;
    nY := nY + Item.ItemEffect.OffsetY + (Item.TextureHeight - d.Height) div 2;
  end
  else begin
    nX := nX + Item.ItemEffect.OffsetX + oX;
    nY := nY + Item.ItemEffect.OffsetY + oY;
  end;

  if Item.ItemEffect.NoBlend then
    GameCanvas.Draw(nX, nY, d.ClientRect, d)
  else
    GameCanvas.DrawBlend(nX, nY, d.ClientRect, d);
end;

procedure TPlayScene.DrawPreviewMonItemValueEffect(nX, nY:Integer; Item:PClientPreviewMonItem);
var
  d:TTexture;
  GameImages:TGameImages;
  oX, oY:Integer;
begin
  if not g_ClientConfig.boShowDropValueItemEff then Exit;
  if not g_ConfigDlg.ConfigCheckeds[ckShowValueItemEffect] then Exit;

  if not g_ConfigClient.boShowValueItemEffect then Exit;
  if g_ConfigClient.dwValueItemEffectCount = 0 then Exit;
  if g_ConfigClient.dwValueItemEffectPlayTime <= 0 then Exit;

  if not Item.PreiewMonItem.boValueItem then Exit;

  GameImages := g_WNewopUIImages;

  if MyGetTickCount - Item.ValueItemEffectTick >= g_ConfigClient.dwValueItemEffectPlayTime then begin
    Inc(Item.ValueItemEffectFrame);
    Item.ValueItemEffectTick := MyGetTickCount;
  end;

  if Item.ValueItemEffectFrame < 0 then
    Item.ValueItemEffectFrame := 0
  else if Item.ValueItemEffectFrame >= Integer(g_ConfigClient.dwValueItemEffectCount) then
    Item.ValueItemEffectFrame := 0;

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := GameImages.GetCachedGrayImage(Item.ValueItemEffectFrame + Integer(g_ConfigClient.dwValueItemEffectIndex), oX, oY)
  else
    d := GameImages.GetCachedImage(Item.ValueItemEffectFrame + Integer(g_ConfigClient.dwValueItemEffectIndex), oX, oY);

  if d = nil then Exit;

  nX := nX + g_ConfigClient.nValueItemEffectOffsetX + oX;
  nY := nY + g_ConfigClient.nValueItemEffectOffsetY + oY;

  GameCanvas.Draw(nX, nY, d.ClientRect, d);
end;

procedure TPlayScene.DrawPreviewItem();
const
  COL_WIDTH = 100;
  ROW_HEIGHT = 40;
var
  I, II:Integer;
  Item:PClientPreviewMonItem;
  nCount, nIndex, nRow, nCol, nX, nY, nTempX, nTempY:Integer;
  d:TTexture;
  sName:string;
  Actor:TActor;
begin
  if (g_PreviewItem = nil) or (Length(g_PreviewItem) = 0) then Exit;
  if g_nPreviewItemActorRecog = 0 then Exit;
  if g_dwPreviewItemShowTime = 0 then Exit;

  if tick_diff(g_dwPreviewItemShowTick, MyGetTickCount) > g_dwPreviewItemShowTime then Exit;

  Actor := PlayScene.FindActor(g_nPreviewItemActorRecog);
  if Actor = nil then Exit;
  if Actor.m_boGhost or Actor.m_boDeath then Exit;

  nCount := Length(g_PreviewItem);
  if nCount < 3 then begin
    nCol := nCount;
    nRow := 1;
  end
  else begin
    nCol := Trunc(Sqrt(nCount));
    nRow := (nCount + nCol - 1) div nCol;
  end;

  nIndex := 0;
  nY := (Actor.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY - Round(((nRow - 1) / 2) * ROW_HEIGHT);
  for I := 0 to nRow - 1 do begin
    nX := (Actor.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX - Round(((nCol - 1) / 2) * COL_WIDTH);

    for II := 0 to nCol - 1 do begin
      Item := @g_PreviewItem[nIndex];

      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := g_WDnItemImages.Grays[Item.PreiewMonItem.wLooks]
      else
        d := g_WDnItemImages.Images[Item.PreiewMonItem.wLooks];

      if d <> nil then begin
        Item.TextureWidth := d.Width;
        Item.TextureHeight := d.Height;

        nTempX := nX + (20 - d.Width) div 2;
        nTempY := nY + (20 - d.Height) div 2;

        DrawPreviewMonItemEffect(nTempX, nTempY, Item, True);

        GameCanvas.Draw(nTempX,
          nTempY,
          d.ClientRect,
          d);

        DrawPreviewMonItemEffect(nTempX, nTempY, Item, False);
      end;

      sName := Item.PreiewMonItem.sName;
      if Item.PreiewMonItem.nCount > 1 then begin
        sName := sName + ' ×' + IntToStr(Item.PreiewMonItem.nCount);
      end;

      nTempX := nX + (20 - CurrentFont.TextWidth(sName)) div 2;
      nTempY := nY + 20;

      BoldTextOut(nTempX, nTempY, sName, GetRGB(Item.PreiewMonItem.btColor));

      if d <> nil then begin
        nTempX := nX + (20 - d.Width) div 2;
        nTempY := nY + (20 - d.Height) div 2;

        DrawPreviewMonItemValueEffect(nTempX, nTempY, Item);
      end;

      nX := nX + COL_WIDTH;
      Inc(nIndex);
      if nIndex >= nCount then Exit;
    end;

    nY := nY + ROW_HEIGHT;
  end;
end;

procedure TPlayScene.DrawActorLabel(Sender:TObject);
var
  k, j, nX, nY:Integer;
  //nMinX, nMaxX: Integer;
  //nMinY, nMaxY: Integer;
  Actor:TActor;
  rc, rc2:TRect;
  Abil:TAbility;
  HP, MP, HpImgWidth, HpImgHeigh, HPOffsetX, HPOffsetY:Integer;
  d:TTexture;
  FocusCret:TActor;
  Color1:Cardinal;
  Color2:Cardinal;
  Color3:Cardinal;
  Color4:Cardinal;
  boShowHPLabel:Boolean;

  BaseConfig:PClientBaseConfig;
  GameImage:TGameImages;
  DBackGround, DHP:TTexture;
  NpcConfig:PClientCustomNpcConfig;

  HpBarOffsetX, HpBarOffsetY:Integer;
begin
  {$MESSAGE HINT '血条应该在此绘制，稍后改BUG会用到'}
  if (not m_boCanDraw) or g_boAppExit then Exit;
  g_nRenderCode := 47;

  //nMinX := g_MySelf.m_nCurrX - (HEIGHTGRIDCOUNT - 1);
  //nMaxX := g_MySelf.m_nCurrX + (HEIGHTGRIDCOUNT - 1);

  //nMinY := g_MySelf.m_nCurrY - (HEIGHTGRIDCOUNT - 1);
  //nMaxY := g_MySelf.m_nCurrY + (BOTTOMGRIDCOUNT - 1);

  g_nRenderCode := 48;
  try
    g_nRenderCode := 49;
    // if (m_ActorLabelImages <> nil) and (m_ActorLabelImages.Texture <> nil) then begin
    g_nRenderCode := 50;
    for k := 0 to m_DrawActorList.Count - 1 do begin // 显示NPC血条
      if (not m_boCanDraw) or g_boAppExit then break;
      Actor := m_DrawActorList[k];
      if (Actor.m_boDelActor) then Continue;

      // 摆摊不绘制些信息 add not Actor.m_boShopStall 2019-07-23 10:14:54
      if (not Actor.m_boDeath) and Actor.m_boCanDraw and (not Actor.m_boShopStall) then begin
        if Actor.m_noInstanceOpenHealth then
          if MyGetTickCount - Actor.m_dwOpenHealthStart > Actor.m_dwOpenHealthTime then
            Actor.m_noInstanceOpenHealth := False;

        if (Actor.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
          HpBarOffsetX := g_ClientConfig.nHumHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nHumHPBarOffsetY;
        end else if (Actor.m_btRace = RC_MERCHANT) then begin
          HpBarOffsetX := g_ClientConfig.nNpcHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nNpcHPBarOffsetY;
        end else begin
          HpBarOffsetX := g_ClientConfig.nMonHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nMonHPBarOffsetY;
        end;

        if (PlugInEnabled and g_ClientConfig.boShowHPLabel and g_ConfigDlg.ConfigCheckeds[ckShowHPLabel])
          or Actor.m_noInstanceOpenHealth or Actor.m_boOpenHealth then begin
          g_nRenderCode := 51;
          Abil := Actor.m_Abil;

          if Abil.MaxHP < Abil.HP then
            Abil.MaxHP := Abil.HP;

          if Abil.MaxMP < Abil.MP then
            Abil.MaxMP := Abil.MP;

          d := g_NewopUI170TextureArray[0]; //各种颜色血条图像的数组
          if d <> nil then
            rc := Rect(0, 0, d.Width, d.Height)
          else
            rc := Rect(0, 0, 32, 3);

          HpImgWidth := rc.Right;
          HpImgHeigh := rc.Bottom;

          HPOffsetX := HpImgWidth div 2;
          HPOffsetY := HpImgHeigh * 2 + 4;

          g_nRenderCode := 54;

          if ((Actor.m_btRace = RC_MERCHANT) or (Actor.m_btRace = RC_PEACENPC)) then begin
            if Abil.MaxHP <= 0 then begin
              Abil.HP := 1;
              Abil.MaxHP := 1;
            end;
          end;

          HP := Abil.HP;
          MP := Abil.MP;
          Abil.HP := Abil.MaxHP;
          Abil.MP := Abil.MaxMP;

          boShowHPLabel := True;

          if (Actor.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
            if (g_ClientConfig.boHumStruckShowNumber and Actor.m_boStruckShowNumber) or
              (not g_ClientConfig.boHumStruckShowNumber) or (Actor = g_MySelf) or
              (Actor = g_MyHero) {自己英雄显血 piaoyun 2013-09-07} or Actor.m_boOpenHealth then begin
              if {g_ClientConfig.boShowNumberLable and g_ConfigDlg.ConfigCheckeds[ckShowNumberLable] and}(Abil.MaxHP > 0) then begin
                Abil.HP := HP;
                Abil.MP := MP;
              end;
            end;
          end else if (Actor.m_btRace = RC_MERCHANT) then begin
            // 是否显示NPC血条 piaoyun 2013-07-31
            if g_ConfigDlg.ConfigCheckeds[ckShowNpcHPLabel]
              and (not (Actor.m_wAppearance in [54..58, 60..68, 90..92, 94..98])) then begin
              boShowHPLabel := True;
            end else begin
              boShowHPLabel := False;
            end;
          end else if (Abil.MaxHP > 0) then begin
            // 宠物不显示血条 2019-11-18 11:51:48
            if g_ClientConfig.boPetNoShowHPProgress and (Actor.m_HumsBBType = bbGamePet) then begin
              boShowHPLabel := False
            end else if (g_ClientConfig.boMonStruckShowNumber and Actor.m_boStruckShowNumber)
              or (not g_ClientConfig.boMonStruckShowNumber) or Actor.m_boOpenHealth then begin
              Abil.HP := HP;
              Abil.MP := MP;
            end;
          end else begin
            // 宠物不显示血条 2019-11-18 11:51:48
            if g_ClientConfig.boPetNoShowHPProgress and (Actor.m_HumsBBType = bbGamePet) then
              boShowHPLabel := False;
          end;

          if boShowHPLabel then begin
            if Abil.MaxHP > 0 then begin
              if Abil.HP < Abil.MaxHP then
                rc.Right := rc.Left + Round((rc.Right - rc.Left) / Abil.MaxHP * Abil.HP);
            end;

            // 当穿戴护身属性装备，血条调用新资源 chongchong 2014-04-07
            if g_ClientConfig.boShowMagicShieldHP and Actor.m_boMagicShield then begin
              // 其他人但等级血条还没取到的时候默认显示蓝条 chongchong 2014-10-23
              if (Abil.Level = 0) and (Actor <> g_MySelf) then begin
                rc2 := rc;
                rc := Rect(0, 0, 0, HpImgHeigh);
              end
              else if (Actor.m_btJob = 0) and (Abil.Level < 28) then
                rc2 := Rect(0, 0, 0, HpImgHeigh)
              else begin
                rc2 := Rect(0, 0, HpImgWidth, HpImgHeigh);
                if Abil.MaxMP > 0 then begin
                  if Abil.MP < Abil.MaxMP then
                    rc2.Right := rc2.Left + Round((rc2.Right - rc2.Left) / Abil.MaxMP * Abil.MP);
                end;
              end;

              GameCanvas.Draw(Actor.m_nSayX - HPOffsetX + HpBarOffsetX, Actor.m_nSayY - HPOffsetY + HpBarOffsetY, g_NewopUI170TextureArray[0]);

              if rc.Right - rc.Left > 0 then
                GameCanvas.Draw(Actor.m_nSayX - HPOffsetX + HpBarOffsetX, Actor.m_nSayY - HPOffsetY + HpBarOffsetY, rc, g_NewopUI170TextureArray[1]);

              if rc2.Right - rc2.Left > 0 then
                GameCanvas.Draw(Actor.m_nSayX - HPOffsetX + HpBarOffsetX, Actor.m_nSayY - HPOffsetY + HpBarOffsetY, rc2, g_NewopUI170TextureArray[7]);
            end else begin
              // NPC怪物 chongchong 2014-04-13
              nX := Actor.m_nSayX - HPOffsetX;
              nY := Actor.m_nSayY - HPOffsetY;
              if Actor.m_btRace = RC_MERCHANT then begin
                NpcConfig := nil;
                if Actor.m_wAppearance >= 10000 then begin
                  NpcConfig := GetCustomNpcConfig(Actor.m_wAppearance);
                end;

                if (NpcConfig = nil) or (NpcConfig.BaseConfig.HPFile < 0) or
                  (NpcConfig.BaseConfig.HPFile >= g_EffectImageList.Count) or
                  (NpcConfig.BaseConfig.HPStartIndex < 0) then begin
                  GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, g_NewopUI170TextureArray[0]);
                  GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, rc, g_NewopUI170TextureArray[4]);
                end else begin
                  GameImage := TGameImages(g_EffectImageList.Objects[NpcConfig.BaseConfig.HPFile]);

                  // 默认怪物绘制
                  if GameImage = nil then begin
                    GameCanvas.Draw(nX + NpcConfig.BaseConfig.HPOffsetX, nY + NpcConfig.BaseConfig.HPOffsetY, g_NewopUI170TextureArray[0]);
                    GameCanvas.Draw(nX + NpcConfig.BaseConfig.HPOffsetX, nY + NpcConfig.BaseConfig.HPOffsetY, rc, g_NewopUI170TextureArray[4]);
                  end else begin
                    DBackGround := GameImage.Images[NpcConfig.BaseConfig.HPStartIndex];
                    DHP := GameImage.Images[NpcConfig.BaseConfig.HPStartIndex + 1];
                    if DBackGround <> nil then begin
                      nX := Actor.m_nSayX - DbackGround.Width div 2;
                      GameCanvas.Draw(nX + NpcConfig.BaseConfig.HPBgOffsetX, nY + NpcConfig.BaseConfig.HPBgOffsetY, DBackGround);

                      if DHP <> nil then begin
                        rc := Rect(0, 0, DHP.Width, DHP.Height);
                        if Actor.m_Abil.MaxHP > 0 then
                          rc.Right := rc.Left + Round((RC.Right - rc.Left) / Actor.m_Abil.MaxHP * Actor.m_Abil.HP);
                        GameCanvas.Draw(nX + NpcConfig.BaseConfig.HPOffsetX, nY + NpcConfig.BaseConfig.HPOffsetY, rc, DHP);
                      end;
                    end;
                  end;
                end;
              end else if (Actor.m_btRealRace = RC_GUARD) or (Actor.m_btRealRace = RC_ARCHERGUARD) then begin //大刀，弓箭手 chongchong 2014-04-13
                GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, g_NewopUI170TextureArray[0]);
                GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, rc, g_NewopUI170TextureArray[6]);
              end else if (Actor is TCustomActor) then begin // 自定义怪物支持自定义血条 chongchong 2014-08-12
                BaseConfig := @(Actor as TCustomActor).Config.BaseConfig;
                if BaseConfig.HPStartIndex < 0 then begin
                  // 默认血条加入全局的偏移HpBarOffsetX/HpBarOffsetY 2020-10-29 10:57:59
                  GameCanvas.Draw(nX + BaseConfig.HPOffsetX + HpBarOffsetX, nY + BaseConfig.HPOffsetY + HpBarOffsetY, g_NewopUI170TextureArray[0]);
                  // 默认怪物绘制
                  GameCanvas.Draw(nX + BaseConfig.HPOffsetX + HpBarOffsetX, nY + BaseConfig.HPOffsetY + HpBarOffsetY, rc, g_NewopUI170TextureArray[5]);
                end else begin
                  if (BaseConfig.HPFile >= 0) and (BaseConfig.HPFile < g_EffectImageList.Count) then
                    GameImage := TGameImages(g_EffectImageList.Objects[BaseConfig.HPFile])
                  else
                    GameImage := g_WMonImages.Images[Actor.m_wAppearance];

                  // 默认怪物绘制
                  if GameImage = nil then begin
                    // 默认血条加入全局的偏移HpBarOffsetX/HpBarOffsetY 2020-10-29 10:57:59

                    GameCanvas.Draw(nX + BaseConfig.HPOffsetX + HpBarOffsetX, nY + BaseConfig.HPOffsetY + HpBarOffsetY, g_NewopUI170TextureArray[0]);
                    GameCanvas.Draw(nX + BaseConfig.HPOffsetX + HpBarOffsetX, nY + BaseConfig.HPOffsetY + HpBarOffsetY, rc, g_NewopUI170TextureArray[5]);
                  end else begin
                    DBackGround := GameImage.Images[BaseConfig.HPStartIndex];
                    DHP := GameImage.Images[BaseConfig.HPStartIndex + 1];
                    if DBackGround <> nil then begin
                      nX := Actor.m_nSayX - DbackGround.Width div 2;
                      GameCanvas.Draw(nX + BaseConfig.HPBgOffsetX, nY + BaseConfig.HPBgOffsetY, DBackGround);

                      if DHP <> nil then begin
                        rc := Rect(0, 0, DHP.Width, DHP.Height);
                        if Actor.m_Abil.MaxHP > 0 then
                          rc.Right := rc.Left + Round((RC.Right - rc.Left) / Actor.m_Abil.MaxHP * Actor.m_Abil.HP);
                        GameCanvas.Draw(nX + BaseConfig.HPOffsetX, nY + BaseConfig.HPOffsetY, rc, DHP);
                      end;
                    end;
                  end;
                end;
              end else begin
                GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, g_NewopUI170TextureArray[0]);

                // 高亮显血对自己和英雄有效 chongchong 2013-12-10
                if (Actor.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) and g_ClientConfig.boShowHighlightHPLabel
                  and g_ConfigDlg.ConfigCheckeds[ckShowHighlightHPLabel] and ((Actor = g_MySelf) or (Actor = g_MyHero)) then begin
                  GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, rc, g_NewopUI170TextureArray[2])
                end else begin
                  if Actor.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT] then
                    GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, rc, g_NewopUI170TextureArray[1])
                  else
                    GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, rc, g_NewopUI170TextureArray[5]);
                end;
              end;
            end;
          end;
        end;

        // 摆摊不绘制些信息 add not Actor.m_boShopStall 2019-07-23 10:14:54

        if (Actor.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) and
          (THumActor(Actor).m_boTrainingNG) and (THumActor(Actor).m_AbilNG.MaxNH > 0) and
          (g_ClientConfig.boShowNGLabel) and g_ConfigDlg.ConfigCheckeds[ckShowNGLabel] and
          (not Actor.m_boShopStall) then begin // 人物 英雄 绘制内功黄条 piaoyun 2013-09-09

          //HZQ 20230525 按照上面的思路补全HpImgWidth和HpOffsetX, Y的值
          {$MESSAGE HINT '需要测试补全的代码'}
          d := g_NewopUI170TextureArray[0]; //各种颜色血条图像的数组3是黄条，但是这个地方画了血条的
          if d <> nil then
            rc := Rect(0, 0, d.Width, d.Height)
          else
            rc := Rect(0, 0, 32, 3);

          HpImgWidth := rc.Right;
          HpImgHeigh := rc.Bottom;

          HPOffsetX := HpImgWidth div 2;
          HPOffsetY := HpImgHeigh * 2 + 4;

          rc := Rect(0, 0, HpImgWidth, HpImgHeigh);
          //d := m_ActorLabelTexture.GetCachedImage(5, nX, nY, rc);
          //if d <> nil then
          begin
            rc2 := Rect(0, 0, HpImgWidth, HpImgHeigh);
            rc2.Right := rc2.Left + Round((rc2.Right - rc2.Left) * THumActor(Actor).m_AbilNG.NH / THumActor(Actor).m_AbilNG.MaxNH);
            GameCanvas.Draw(Actor.m_nSayX - HPOffsetX + HpBarOffsetX, Actor.m_nSayY - HPOffsetY + HpImgHeigh + HpBarOffsetY, g_NewopUI170TextureArray[0]);
            GameCanvas.Draw(Actor.m_nSayX - HPOffsetX + HpBarOffsetX, Actor.m_nSayY - HPOffsetY + HpImgHeigh + HpBarOffsetY, rc2, g_NewopUI170TextureArray[3]);
          end;
        end;
      end;

      // 顶戴花翎指定绘制顺序 chongchong 2014-09-02
      if Actor.m_boCanDraw then begin
        // 摆摊不绘制些信息 add not Actor.m_boShopStall 2019-07-23 10:14:54
        if not Actor.m_boShopStall then begin
          Actor.ShowIcons(False);
        end;
      end;

      if PlugInEnabled then begin
        g_nRenderCode := 59;
        Actor.ShowNumberLable;

        // 放在这里而非Run中，用来处理站立时速度快，跑时速度慢 chongchong 2017-11-27
        if Actor.CheckLoadHealthNumber then Actor.LoadHealthNumber;

        g_nRenderCode := 60;
        Actor.ShowHealthNumber;
      end;

      g_nRenderCode := 61;
      Actor.ShowShopName;

      if not ((g_FocusCret = Actor) or (g_boSelectMyself and (g_MySelf = Actor))) then begin
        g_nRenderCode := 62;
        Actor.ShowName;
      end;

      g_nRenderCode := 63;
      Actor.ShowSay;
    end;
  except
    on E:Exception do begin
      DebugOutStr(IntToStr(g_nRenderCode));
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 66;
  try
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    FocusCret := g_FocusCret;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
    if (FocusCret <> nil) and IsValidActorEx(FocusCret) then begin
      //if (FocusCret.m_PreviewItem = nil) or (Length(FocusCret.m_PreviewItem) = 0) or (tick_diff(g_FocusCretTick, MyGetTickCount) > 3000) then
      FocusCret.ShowName;

      //if tick_diff(g_FocusCretTick, MyGetTickCount) <= 3000 then
      //  FocusCret.DrawPreviewItem((FocusCret.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX, (FocusCret.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY);
    end;

    g_nRenderCode := 671;
    if g_boSelectMyself then begin
      g_MySelf.ShowName;
    end;

    DrawPreviewItem;
  except
    on E:Exception do begin
      DebugOutStr('112');
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 68;
  // if g_StringsImages.Texture <> nil then
   // GameCanvas.Draw(0, 0, g_StringsImages.Texture);
end;

procedure TPlayScene.DrawItems(Sender:TObject);
var
  I, II, ix, iy, ax, ay:Integer;
  d:TTexture;
  DropItem:pTDropItem;
  List:TPointDropItemList;
begin
  if (not m_boCanDraw) or g_boAppExit then Exit;
  g_nRenderCode := 42;
  // 地面物品 闪亮
  try
    for I := 0 to g_DropItemsMgr.Count - 1 do begin
      if (not m_boCanDraw) or g_boAppExit then break;

      List := g_DropItemsMgr.Items[I];
      if List.Count = 0 then Continue;

      for II := 0 to List.DrawCount - 1 do begin
        DropItem := List.DrawItems[II];

        if (not DropItem.Visible) then Continue;
        //ix := (DropItem.X - Map.m_ClientRect.Left) * UNITX + m_nDefXX + SOFFX;
        //iy := (DropItem.Y - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + SOFFY;
        ix := (Cutecode_T(DropItem.X) - Map.m_ClientRect.Left) * UNITX + m_nDefXX + SOFFX;
        iy := (Cutecode_T(DropItem.Y) - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + SOFFY;
        if DropItem.ShowFlash then begin
          d := g_WMainImages.GetCachedImage(410 + DropItem.FlashStep, ax, ay);
          if d <> nil then
            GameCanvas.DrawBlend(ix + ax, iy + ay, d);
        end;
      end;
    end;
  except
    on E:Exception do begin
      DebugOutStr('113');
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 43;
  try
    for I := 0 to g_DropItemsMgr.Count - 1 do begin
      if (not m_boCanDraw) or g_boAppExit then break;

      List := g_DropItemsMgr.Items[I];
      if List.Count = 0 then Continue;

      for II := 0 to List.DrawCount - 1 do begin
        DropItem := List.DrawItems[II];
        if (not DropItem.Visible) then Continue;
        ix := (DropItem.X - Map.m_ClientRect.Left) * UNITX + m_nDefXX + SOFFX;
        iy := (DropItem.Y - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + SOFFY;
        ShowItemName(DropItem, ix, iy);
      end;
    end;
  except
    on E:Exception do begin
      DebugOutStr('114');
      DebugOutStr(E.Message);
    end;
  end;
  g_nRenderCode := 44;
end;

procedure TPlayScene.DrawNearActorHintEffect(Sender: TObject);
var
    i, nStartIndex, pX, pY:Integer;
    pHintInfo:PNearActorHintInfo;
    d:TTexture;
const
    NORMAL_START_INDEX = 144;
    FRIEND_START_INDEX = 154;
begin
    for i := 0 to m_NearActorHintMgr.InfoCount - 1 do begin
        pHintInfo := m_NearActorHintMgr.GetHintInfo(i);
        if pHintInfo <> nil then begin
            if pHintInfo.nFriendFlag <> 0 then begin
                nStartIndex := FRIEND_START_INDEX;
            end else begin
                nStartIndex := NORMAL_START_INDEX;
            end;
            d := g_WNewopUIImages.GetCachedImage(nStartIndex + m_NearActorHintMgr.FrameIndex, px, py);
            if d <> nil then begin
                GameCanvas.Draw(pHintInfo.X + px, pHintInfo.Y + py, d, Blend_SrcColorAdd); //Blend_SrcColorAdd
            end;
        end;
    end;
    m_NearActorHintMgr.FrameDrive();
end;

procedure TPlayScene.RenderScene(Sender:TObject);
var
  d:TTexture;
  msgstr:string;
  i:Integer;
  e:TClEvent;
  x, y:Integer;
  intX, intY:Smallint;
begin
  {$IF TESTMODE = 1}
  if not g_boStartRender then begin
    g_boStartRender := True;
    //OutputDebugString(PChar('确定到第一次渲染开始：' + IntToStr(MyGetTickCount - g_NoticeClickTick)));
  end;
  {$IFEND}

  // if g_boDepthStencil then
  // GameCanvas.FillRect(Bounds(0, 0, SCREENWIDTH, SCREENHEIGHT), clBlack);
  try
    if (g_MySelf = nil) then begin
      msgstr := '正在退出游戏，请稍候...';
      BoldTextOut((SCREENWIDTH - CurrentFont.TextWidth(msgstr)) div 2, (SCREENHEIGHT - g_CurrentFontHeight) div 2, msgstr, clWhite);
      Exit;
    end;

    if (m_SceneShakeList.Count > 0) and (MyGetTickCount - m_dwSceneShakeTick >= 50) then begin
      m_dwSceneShakeTick := MyGetTickCount;
      i := Integer(m_SceneShakeList.Items[0]);
      m_SceneShakeList.Delete(0);
      intX := Smallint(LoWord(I));
      intY := SmallInt(HiWord(I));
      ShakeX := intX;
      ShakeY := intY;
    end;

    g_nRenderCode := 27;
    if g_boRenderTargetTileMap then begin
      g_nRenderCode := 271;
      GameCanvas.Draw(0, 0, m_MapRect, g_RenderTarget[0]); // Bounds(m_nTileMapX, m_nTileMapY, MAPSURFACEWIDTH, MAPSURFACEHEIGHT)
    end else if Map.m_boEIMap then begin
      g_nRenderCode := 272;
      DrawTileEIMap(m_MapRect);
    end
    else begin
      g_nRenderCode := 273;
      DrawTileMap(m_MapRect);
    end;

    // DrawAniTiles(Sender);

    g_nRenderCode := 28;
    DrawScene(Sender);

    g_nRenderCode := 281;
    DrawEffect(Sender);

    g_nRenderCode := 282;
    DrawItems(Sender);

    g_nRenderCode := 45;

    { TODO -opiaoyun -c特效 : 天气效果扩展【2013-07-15】 }
    for i := 0 to High(g_WeateherEffect) do begin
      if g_WeateherEffect[i].boIsUsed then begin
        if MyGetTickCount - g_WeateherEffect[i].dwTick > 100 then begin
          g_WeateherEffect[i].dwTick := MyGetTickCount;
          Inc(g_WeateherEffect[i].dwIndex);
        end;
        if (g_WeateherEffect[i].dwIndex < g_WeateherEffect[i].dwStartOffset) or
          (g_WeateherEffect[i].dwIndex > g_WeateherEffect[i].dwEndOffset) then
          g_WeateherEffect[i].dwIndex := g_WeateherEffect[i].dwStartOffset;

        case i of
          0:d := g_WEffectImg_SE.Images[g_WeateherEffect[i].dwIndex]; // 黄沙效果
          1, 2:d := g_WEffectImg_EX.Images[g_WeateherEffect[i].dwIndex]; // 花瓣效果 下雪效果
          else
            d := g_WEffectWeatherImg.Images[g_WeateherEffect[i].dwIndex];
        end;

        if d <> nil then begin
          { TODO -c修正 -opiaoyun : 修复下雪 黑暗问题 【2013-7-14】}
          if not g_WeateherEffect[i].boIsDark then
            // GameCanvas.DrawBlend(0, 0, d)
            GameCanvas.StretchDraw(Rect(0, 0, GameCanvas.Width, GameCanvas.Height), d.ClientRect, d, 255, Blend_SrcAlphaColor)
          else
            GameCanvas.StretchDraw(Rect(0, 0, GameCanvas.Width, GameCanvas.Height), d.ClientRect, d, Blend_OneColor);
        end;
      end
    end;

    g_nRenderCode := 450;
    for i := 0 to m_DrawEventList.Count - 1 do begin
      if (not m_boCanDraw) or g_boAppExit then break;
      e := TClEvent(m_DrawEventList[i]);
      if not e.m_boVisible then Continue;

      // 修正烟花播放时在所有人物最前面 chongchong 2015-01-11
      if (e.m_nEventType in [ET_FIREFLOWER_1..ET_FIREFLOWER_8]) then {// 只画烟花} begin
        PlayScene.ScreenXYfromMCXY(e.m_nX, e.m_nY, x, y);
        e.DrawEvent(x, y);
      end;
    end;

    g_nRenderCode := 451;
    // 黑夜 - 只显示照亮范围的怪物血条，代码移到这里 chongchong 2015-03-09
    if ((g_boViewFog and g_ClientConfig.boViewFog) or g_boMapNight) and g_boRenderTarget then begin
      DrawActorLabel(Sender);
    end;

    g_nRenderCode := 452;
    if ((g_boViewFog and g_ClientConfig.boViewFog) or g_boMapNight) and g_boRenderTarget then begin
      GameCanvas.Draw(0, 0, g_RenderTarget[2].ClientRect, g_RenderTarget[2], Blend_Multiply);
    end;
    g_nRenderCode := 46;
  except
    on E:Exception do begin
      DebugOutStr('[Exception] TPlayScene.RenderScene Error; Code = ' + IntToStr(g_nRenderCode));
      DebugOutStr(E.Message);
    end;
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookDrawScene1) then begin
    try
      HookDrawScene1();
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookDrawScene1');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}

  try
    g_nRenderCode := 470;
    // 黑夜 - 只显示照亮范围的怪物血条，代码上移 chongchong 2015-03-09
    if not (((g_boViewFog and g_ClientConfig.boViewFog) or g_boMapNight) and g_boRenderTarget) then begin
      DrawActorLabel(Sender);
    end;

    if g_ConfigDlg.ConfigCheckeds[ckObjectHintEffect] then begin
        DrawNearActorHintEffect(Sender);
    end;

    g_nRenderCode := 471;
    DrawCurMonBigHPProgress;

    g_nRenderCode := 472;
    // 不显示小地图时，右上角显示地图信息 chongchong 2015-08-12
    if (g_ClientVersion = cvMirNewUI205) and (g_MySelf <> nil) and
      (not FrmDlg.DMinMapDlg.Visible) and
      (not FrmDlg.CheckDMinMapBigDlgVisible) and
      (not FrmDlg.CheckDMinMapDlgExVisible) and
      (Length(g_sMapTitle) > 0) then begin
      msgstr := g_sMapTitle + ' ' + IntToStr(g_MySelf.m_nCurrX) + ':' + IntToStr(g_MySelf.m_nCurrY);
      BoldTextOut(g_nScreenWidth - CurrentFont.TextWidth(msgstr) - 10, 16, msgstr, clWhite);
    end;
  except
    on E:Exception do begin
      DebugOutStr('[Exception] TPlayScene.RenderScene Error; Code = ' + IntToStr(g_nRenderCode));
      DebugOutStr(E.Message);
    end;
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookDrawScene2) then begin
    try
      HookDrawScene2();
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookDrawScene2');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}
end;

procedure TPlayScene.DrawCurMonBigHPProgress;
var
  nX, nY, nX2, nY2:Integer;
  D:TTexture;
  R:TRect;
  Color:TColor;
  Actor:TActor;
  Abil:pTAbility;
  HPBlockSize, nPreSize, nCurSize, nCurMaxSize:LongWord;
  nBGWidth:Integer;
  HPBlockIndex:LongWord;
  S:string;
  HPBlockNum:array of TTexture;
  I, Index:Integer;
begin
  if (not g_ConfigDlg.ConfigCheckeds[ckHideBigHPProgress]) and (MyGetTickCount - m_nCurrMonLastAttack <= 5000) then begin
    Actor := FindActor(m_nCurrMonBigHPRecogId);
    if Actor = nil then Exit;

    if Actor.m_boDeath or Actor.m_boGhost then Exit;

    if not Actor.m_boShowBigHPProgress then Exit;
    if Actor.m_Abil.MaxHP = 0 then Exit;

    Abil := @Actor.m_Abil;

    nBGWidth := 0;

    // 背景
    if (Actor.m_BigHPProgressInfo.nHPBGIndex >= 0) and (Actor.m_BigHPProgressInfo.nHPBGIndex < 10) then begin
      d := g_WNewopUIImages.Images[760 + Actor.m_BigHPProgressInfo.nHPBGIndex];

      if d <> nil then begin
        nBGWidth := d.Width;
      end;
    end;

    if Actor.m_BigHPProgressInfo.btHorizAlign = 1 then
      nX := (g_nScreenWidth - nBGWidth) div 2 + Actor.m_BigHPProgressInfo.nHPBGX
    else if Actor.m_BigHPProgressInfo.btHorizAlign = 2 then
      nX := g_nScreenWidth - nBGWidth + Actor.m_BigHPProgressInfo.nHPBGX
    else
      nX := Actor.m_BigHPProgressInfo.nHPBGX;

    nY := Actor.m_BigHPProgressInfo.nHPBGY;

    // 背景
    if (Actor.m_BigHPProgressInfo.nHPBGIndex >= 0) and (Actor.m_BigHPProgressInfo.nHPBGIndex < 10) then begin
      d := g_WNewopUIImages.Images[760 + Actor.m_BigHPProgressInfo.nHPBGIndex];

      if d <> nil then begin
        GameCanvas.Draw(nX, nY, D);
      end;
    end;

    // 图像
    if (Actor.m_BigHPProgressInfo.nImageIndex >= 0) and (Actor.m_BigHPProgressInfo.nImageIndex < 100) then begin
      d := g_WNewopUIImages.Images[780 + Actor.m_BigHPProgressInfo.nImageIndex];

      if d <> nil then begin
        GameCanvas.Draw(nX + Actor.m_BigHPProgressInfo.nImageX, nY + Actor.m_BigHPProgressInfo.nImageY, D);
      end;
    end;

    if Actor.m_BigHPProgressInfo.wHPBlockCount <= 1 then begin
      // 血条
      if (Actor.m_BigHPProgressInfo.nHPIndex >= 0) and (Actor.m_BigHPProgressInfo.nHPIndex < 10) then begin
        d := g_WNewopUIImages.Images[770 + Actor.m_BigHPProgressInfo.nHPIndex];

        if d <> nil then begin
          R := Rect(0, 0, d.Width, d.Height);
          if Abil.MaxHP > 0 then
            R.Right := R.Left + Round((R.Right - R.Left) / Abil.MaxHP * Abil.HP);

          GameCanvas.Draw(nX + Actor.m_BigHPProgressInfo.nHPX, nY + Actor.m_BigHPProgressInfo.nHPY, R, D);
        end;
      end;

      if (Actor.m_BigHPProgressInfo.boShowHPValue >= 0) and (Actor.m_BigHPProgressInfo.boShowHPValue <= 255) then begin
        Color := GetRGB(Actor.m_BigHPProgressInfo.boShowHPValue);
        CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nHPValueX, nY + Actor.m_BigHPProgressInfo.nHPValueY, Format('%u/%u', [Abil.HP, Abil.MaxHP]), Color);
      end;
    end
    else begin
      HPBlockSize := (Abil.MaxHP + Actor.m_BigHPProgressInfo.wHPBlockCount - 1) div Actor.m_BigHPProgressInfo.wHPBlockCount;
      if HPBlockSize < 1 then HPBlockSize := 1;

      HPBlockIndex := Abil.HP div HPBlockSize;
      if Abil.HP mod HPBlockSize <> 0 then
        HPBlockIndex := HPBlockIndex + 1
      else if HPBlockIndex < 1 then
        HPBlockIndex := 1;

      nPreSize := (HPBlockIndex - 1) * HPBlockSize;
      nCurSize := Abil.HP - nPreSize;

      if HPBlockIndex = HPBlockSize then
        nCurMaxSize := Abil.MaxHP - nPreSize
      else
        nCurMaxSize := HPBlockSize;

      if HPBlockIndex > 1 then begin
        d := g_WNewopUIImages.Images[770 + (HPBlockIndex - 2 + 10) mod 10]; // - 1
        if d <> nil then begin
          R := Rect(0, 0, d.Width, d.Height);
          GameCanvas.Draw(nX + Actor.m_BigHPProgressInfo.nHPX, nY + Actor.m_BigHPProgressInfo.nHPY, R, D);
        end;
      end;

      d := g_WNewopUIImages.Images[770 + (HPBlockIndex - 1) mod 10]; // - 0
      if d <> nil then begin
        R := Rect(0, 0, d.Width, d.Height);
        if nCurMaxSize > 0 then
          R.Right := R.Left + Round((R.Right - R.Left) / nCurMaxSize * nCurSize);
        GameCanvas.Draw(nX + Actor.m_BigHPProgressInfo.nHPX, nY + Actor.m_BigHPProgressInfo.nHPY, R, D);
      end;

      S := IntToStr(HPBlockIndex);
      SetLength(HPBlockNum, Length(S) + 1);

      HPBlockNum[0] := g_WNewopUIImages.Images[749];
      for I := 1 to Length(S) do begin
        Index := Ord(S[I]) - Ord('0');
        HPBlockNum[I] := g_WNewopUIImages.Images[750 + Index];
      end;

      nX2 := Actor.m_BigHPProgressInfo.nHPBlockOffsetX;
      nY2 := Actor.m_BigHPProgressInfo.nHPBlockOffsetY;

      for I := Low(HPBlockNum) to High(HPBlockNum) do begin
        if HPBlockNum[I] <> nil then begin
          GameCanvas.Draw(nX + nX2, nY + nY2, HPBlockNum[I]);
          Inc(nX2, HPBlockNum[I].Width);
        end;
      end;

      if (Actor.m_BigHPProgressInfo.boShowHPValue >= 0) and (Actor.m_BigHPProgressInfo.boShowHPValue <= 255) then begin
        Color := GetRGB(Actor.m_BigHPProgressInfo.boShowHPValue);
        CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nHPValueX, nY + Actor.m_BigHPProgressInfo.nHPValueY, Format('%u/%u', [nCurSize, nCurMaxSize]), Color);
      end;
    end;

    if (Actor.m_BigHPProgressInfo.boShowLevel >= 0) and (Actor.m_BigHPProgressInfo.boShowLevel <= 255) then begin
      Color := GetRGB(Actor.m_BigHPProgressInfo.boShowLevel);
      CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nLevelX, nY + Actor.m_BigHPProgressInfo.nLevelY, IntToStr(Abil.Level), Color);
    end;

    if (Actor.m_BigHPProgressInfo.boShowMonName >= 0) and (Actor.m_BigHPProgressInfo.boShowMonName <= 255) then begin
      Color := GetRGB(Actor.m_BigHPProgressInfo.boShowMonName);
      CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nMonNameX, nY + Actor.m_BigHPProgressInfo.nMonNameY, Actor.m_sUserName, Color);
    end;

    if (Actor.m_BigHPProgressInfo.boShowHPPercent >= 0) and (Actor.m_BigHPProgressInfo.boShowHPPercent <= 255) then begin
      Color := GetRGB(Actor.m_BigHPProgressInfo.boShowHPPercent);
      CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nHPPercentX, nY + Actor.m_BigHPProgressInfo.nHPPercentY, Format('%d%%', [Round(Abil.HP / Abil.MaxHP * 100)]), Color);
    end;

    if (Actor.m_BigHPProgressInfo.boShowExpHinter >= 0) and (Actor.m_BigHPProgressInfo.boShowExpHinter <= 255) then begin
      Color := GetRGB(Actor.m_BigHPProgressInfo.boShowExpHinter);
      CurrentFont.TextOut(nX + Actor.m_BigHPProgressInfo.nExpHinterX, nY + Actor.m_BigHPProgressInfo.nExpHinterY, Format('归属(%s)', [Actor.m_BigHPProgressInfo.m_ExpHinterName]), Color);
    end;
  end;
end;

{-------------------------------------------------------}

procedure TPlayScene.AddEffectList(MagicEff:TMagicEff);
begin
  {$IF IsMultiThreadRender = 1}
  m_EffectList.Lock;
  {$IFEND}
  m_EffectList.Add(MagicEff);
  {$IF IsMultiThreadRender = 1}
  m_EffectList.UnLock;
  {$IFEND}
end;

{ TODO -opiaoyun -c注释 : 新技能调用处-只管预备动作完成后的特效展示 【2013-6-17】 }

// magnumb = 数据库Effect字段

procedure TPlayScene.NewMagic(aowner:TActor;
  magid, magnumb {Effect}, cx, cy, tx, ty, targetcode:Int64;
  mtype:TMagicType; // EffectType
  Recusion:Boolean;
  anitime:Integer;
  var bofly:Boolean; NewLevel:Integer; boMagItemType:
  Boolean; boLockList:Boolean; MagicLevel:Integer);
var
  I, scx, scy, sctx, scty, effnum, nC:Integer;
  meff:TMagicEff;
  target:TActor;
  wimg:TGameImages;
  boFind:Boolean;
  boMon33_7Effect:Boolean;
begin
  bofly := False;
  boFind := False;
  boMon33_7Effect := False;
  {  if aowner.m_CurMagic.ServerMagicCode = 999 then
      Exit; }
  if magid <> 111 then
  begin
    if boLockList then begin
      {$IF IsMultiThreadRender = 1}
      m_EffectList.Lock;
      try
        {$IFEND}
        for I := 0 to m_EffectList.Count - 1 do
          if TMagicEff(m_EffectList[I]).ServerMagicId = magid then begin
            boFind := True;
            break;
          end;
        {$IF IsMultiThreadRender = 1}
      finally
        m_EffectList.UnLock;
      end;
      {$IFEND}
    end
    else begin
      for I := 0 to m_EffectList.Count - 1 do
        if TMagicEff(m_EffectList[I]).ServerMagicId = magid then begin
          boFind := True;
          break;
        end;
    end;
  end;

  if not boFind then begin
    wimg := nil;
    meff := nil;
    ScreenXYfromMCXY(cx, cy, scx, scy);
    ScreenXYfromMCXY(tx, ty, sctx, scty);

    if magnumb > 0 then
      GetEffectBase(magnumb - 1, 0, wimg, effnum, NewLevel) // magnumb{Effect}
    else
      effnum := -magnumb;
    target := FindActor(targetcode);
    case mtype of // EffectType
      mtReady, mtFly, mtFlyAxe:begin
          case magnumb of
            39:begin
                meff := TMagicEff.Create(magid {替为magnumb，击中后的效果改变了}, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.TargetActor := target;
                meff.frame := 4;
                if wimg <> nil then
                  meff.ImgLib := wimg;
              end;
            63: {// 噬魂沼泽} begin
                {
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.TargetActor := target;
                meff.ImgLib := g_WMagic4Images;
                meff.MagExplosionBase := 780;
                meff.ExplosionFrame := 50;
                meff.frame := 6;
                }

                // 噬魂沼泽 不要飞行效果 chongchong 2018-06-25 11:38:32
                meff := TShowPlayEffect.Create(780, 36, target);
                meff.ImgLib := g_WMagic4Images;
                meff.NextFrameTime := 50;
              end;
            64:begin // 末日审判
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtExplosion, Recusion, anitime);
                meff.MagExplosionBase := 230;
                meff.NextFrameTime := 60;
                meff.ExplosionFrame := 40;
                meff.light := 3;
                meff.ImgLib := g_WMagic4Images;
              end;
            74:begin // 分身术
                meff := TCopySelf.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.frame := 10;
                meff.MagExplosionBase := 90;
                meff.NextFrameTime := 60;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.ImgLib := g_WMagic5Images;
                meff.TargetActor := target;
                PlaySoundEx(bmg_splitshadow); // 分身术音效 2013-07-31
              end;

            199:begin
                (*
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.MagExplosionBase := 270 + 5{指向空白帧-进行欺骗-后续利用SM_716加入爆炸效果，蛋疼 piaoyun 2013-11-12};
                meff.NextFrameTime := 120;
                meff.ExplosionFrame := 3;
                meff.light := 3;
                meff.ImgLib := g_WMagic5Images;
                meff.TargetActor := FindActor(aowner.m_nTargetRecog);
                *)
                { // 月灵攻击
                meff := TMoonMonEffect.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                //meff := TMagicEff.Create(magid, 110, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.frame := 3;
                meff.ImgLib := g_WMagic5Images;
                meff.MagExplosionBase := 270;
                meff.NextFrameTime := 120;
                meff.ExplosionFrame := 6;
                meff.light := 3;
                meff.TargetActor := FindActor(aowner.m_nTargetRecog); }

                // 月灵轻击特效处理 piaoyun 2013-11-14

                // 月灵轻击飞行效果 110 + aowner.m_btDir * 20 chongchong 2014-05-20
                // aowner.m_nCurrX, aowner.m_nCurrY, target.m_nCurrX, target.m_nCurrY
                meff := TMoonMonEffect.Create(110 + GetFlyDirection16(scx, scy, sctx, scty) * 10, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 270; // 爆炸效果 chongchong 2014-05-20
                {
                meff := TMoonMonEffect.Create(100, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 270;
                }
                meff.ExplosionFrame := 6;
                meff.ImgLib := g_WMagic5Images;
                meff.frame := 3;
              end;

            200:begin // 月灵重击
                {meff := TMoonMonEffect.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.frame := 6;
                meff.ImgLib := g_WMagic5Images;
                meff.MagExplosionBase := 450;
                meff.NextFrameTime := 60;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.TargetActor := FindActor(aowner.m_nTargetRecog);
                }
                // 月灵重击特效处理 piaoyun 2013-11-14
                meff := TMoonMonEffect.Create({280} 290 + GetFlyDirection16(scx, scy, sctx, scty) * 10, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 450;
                meff.ExplosionFrame := 10;
                meff.ImgLib := g_WMagic5Images;
                //meff.frame := 6;
              end;

            // 火龙教主特效 piaoyun 2013-08-19
            102:begin
                Effnum := 130;
                meff := TFireDragonEffect.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                //meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.ImgLib := g_WDragonImg;
                meff.TargetActor := target; //nil;//是目标
                meff.NextFrameTime := 120;
                meff.MagExplosionBase := 200;
                meff.ExplosionFrame := 20;
                //if wimg <> nil then meff.ImgLib := wimg;
              end;

            {end
            else  if magnumb = 104 then begin // 双龙破
              meff := TMagicEff.Create(magid, 2610 - 10, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.EffectNumber := magnumb;
              meff.ImgLib := g_cboEffectImg;
              meff.TargetActor := target;
              meff.MagExplosionBase := 2770;
              meff.NextFrameTime := 50;
              meff.ExplosionFrame := 25;
              meff.frame := 5;
            end
            else  if magnumb = 105 then begin // 凤舞祭
              meff := TMagicEff.Create(magid, 2420 - 10, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.EffectNumber := magnumb;
              meff.ImgLib := g_cboEffectImg;
              meff.TargetActor := target;
              meff.MagExplosionBase := 2580;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 8;
              meff.frame := 3;
            end
            else  if magnumb = 106 then begin // 惊雷爆
              meff := TMagicEff.Create(magid, 4230 - 10, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.EffectNumber := magnumb;
              meff.ImgLib := g_cboEffectImg;
              meff.TargetActor := target;
              meff.MagExplosionBase := 4240;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 7;
              meff.frame := 4;
            end
            else  if magnumb = 109 then begin // 八卦掌
              meff := TMagicEff.Create(magid, 2090 - 10, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.EffectNumber := magnumb;
              meff.ImgLib := g_cboEffectImg;
              meff.TargetActor := target;
              meff.MagExplosionBase := 2251;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 4;
              meff.frame := 3;
            end
            else  if magnumb = 111 then begin // 万剑归宗
              meff := TMagicEff.Create(magid, 2820 - 10, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.EffectNumber := magnumb;
              meff.ImgLib := g_cboEffectImg;
              meff.TargetActor := target;
              meff.MagExplosionBase := 2980;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 10;
              meff.frame := 5;   }
            else begin
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.TargetActor := target;
                if wimg <> nil then
                  meff.ImgLib := wimg;
              end;
              bofly := True;
          end;
        end;
      mtExplosion {2}:
        case magnumb of
          // 嗜血术吸血效果 piaoyun 2013-09-15
          57:begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              if meff <> nil then begin
                meff.TargetActor := target;
                meff.NextFrameTime := 50;
                // 噬血术打中目标后吸血动作 1090->1040 chongchong 2014-05-19
                if NewLevel = 0 then begin
                  meff.ImgLib := g_WMagic2Images;
                  meff.MagExplosionBase := 1090;
                  meff.ExplosionFrame := 10;
                end
                else begin
                  meff.ImgLib := g_WMagic9Images;
                  meff.ExplosionFrame := 20;
                  case NewLevel of
                    1:meff.MagExplosionBase := 710;
                    2:meff.MagExplosionBase := 730;
                    3:meff.MagExplosionBase := 750;
                    4:meff.MagExplosionBase := 860;
                    5:meff.MagExplosionBase := 880;
                    6:meff.MagExplosionBase := 900;
                    7:meff.MagExplosionBase := 1010;
                    8:meff.MagExplosionBase := 1030;
                    9:meff.MagExplosionBase := 1050;
                    else
                      meff.MagExplosionBase := 1050;
                  end;
                end;
              end;
            end;
          // magnumb = Effect
// 火龙教主特效-大火圈效果 piaoyun 2013-08-19
          103:begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.ImgLib := g_WDragonImg;
              meff.TargetActor := target; //nil;//是目标
              meff.NextFrameTime := 120;
              meff.MagExplosionBase := 200;
              meff.ExplosionFrame := 20;
            end;

          (*
          // 被万剑归宗攻击人物头顶冒毒烟效果 chongchong 2013-11-10
          111:
            begin
              meff := TPlayEffect.Create(g_cboEffect, scx, scy, 4010, 10, ReplayCount, True, target);
            end;
          *)

          18:begin // 诱惑之光
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1570;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
            end;
          21:begin // 爆裂火焰
              nC := 0;
              if boLockList then begin

                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7)))
                      and (abs(meff.targetx - sctx) <= 2) and (abs(meff.targety - scty) <= 2) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7)))
                    and (abs(meff.targetx - sctx) <= 2) and (abs(meff.targety - scty) <= 2) then begin
                    Inc(nC);
                  end;
                end;
              end;

              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              if NewLevel = 0 then begin
                meff.MagExplosionBase := 1660;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 20;
                meff.light := 3;
              end
              else if NewLevel in [1..3] then begin
                meff.MagExplosionBase := 350;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 15;
                meff.light := 3;
                meff.ImgLib := g_WMagic7Images16;
              end
              else if NewLevel in [4..6] then begin
                meff.MagExplosionBase := 380;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 15;
                meff.light := 3;
                meff.ImgLib := g_WMagic7Images16;
              end
              else begin
                meff.MagExplosionBase := 410;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 15;
                meff.light := 3;
                meff.ImgLib := g_WMagic7Images16;
              end;
            end;
          26:begin // 心灵启示
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 10;
              meff.light := 2;
            end;
          27:begin // 群体治疗术
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1800;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 10;
              meff.light := 3;
            end;
          30:begin // 圣言术
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 3930;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 16;
              meff.light := 3;
            end;

          32:begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 620;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 10;
              meff.light := 2;
              meff.ImgLib := g_WMagic2Images;
            end;

          // 新加技能测试--- piaoyun 2013-6-17
          201:begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 110;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 40;
              meff.light := 3;
              meff.ImgLib := g_WMagic10Images;
            end;

          203:begin // 死亡之眼 piaoyun 2013-6-24
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 30;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 22;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;

          205:begin // 冰霜雪雨 piaoyun 2013-6-24
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 370;
              meff.MagicId := 66;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 70;
              meff.ExplosionFrame := 40;

              // 修正黑夜 - 冰霜雪雨点亮范围小 chongchong 2015-03-04
              meff.light := 2;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;

          206:begin // 冰霜群雨 piaoyun 2013-6-24
              // 连续修正4次坐标，造成错位的技能叠加特效
              ScreenXYfromMCXY(tx - 2, ty - 2, sctx, scty);
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 80;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 17;
              if wimg <> nil then
                meff.ImgLib := wimg;

              meff.TargetRx := tx - 2;
              meff.TargetRy := ty - 2;
              meff.MagOwner := aowner;
              m_EffectList.Add(meff);

              ScreenXYfromMCXY(tx + 2, ty - 2, sctx, scty);
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 80;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 17;
              if wimg <> nil then
                meff.ImgLib := wimg;

              meff.TargetRx := tx + 2;
              meff.TargetRy := ty - 2;
              meff.MagOwner := aowner;
              m_EffectList.Add(meff);

              ScreenXYfromMCXY(tx + 2, ty + 2, sctx, scty);
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 80;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 17;
              if wimg <> nil then
                meff.ImgLib := wimg;

              meff.TargetRx := tx + 2;
              meff.TargetRy := ty + 2;
              meff.MagOwner := aowner;
              m_EffectList.Add(meff);

              ScreenXYfromMCXY(tx - 2, ty + 2, sctx, scty);
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 80;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 17;
              if wimg <> nil then
                meff.ImgLib := wimg;

              meff.TargetRx := tx - 2;
              meff.TargetRy := ty + 2;
              meff.MagOwner := aowner;
              m_EffectList.Add(meff);

              ScreenXYfromMCXY(tx, ty, sctx, scty);
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 80;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 17;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;

          208:begin // 旋风斩 piaoyun 2013-09-14
              {meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1620;
              meff.TargetActor := g_MySelf;                                                              // target;
              meff.NextFrameTime := 120;
              meff.ExplosionFrame := 6;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;}
              meff := nil;
            end;
          209:begin // 五雷轰 piaoyun 2013-09-14
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1640;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 120;
              meff.ExplosionFrame := 12;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;
          210:begin // 幽冥火符 piaoyun 2013-09-14
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1670;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 120;
              meff.ExplosionFrame := 8;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;

          31:begin // 冰咆哮
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                  (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;

              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              if NewLevel = 0 then begin
                meff.MagExplosionBase := 3850;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 20;
                meff.light := 3;
              end
              else if NewLevel in [1..3] then begin
                meff.MagExplosionBase := 90;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 20;
                meff.light := 3;
                meff.ImgLib := g_WMagic8Images16;
              end
              else if NewLevel in [4..6] then begin
                meff.MagExplosionBase := 110;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 20;
                meff.light := 3;
                meff.ImgLib := g_WMagic8Images16;
              end
              else begin
                meff.MagExplosionBase := 130;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 20;
                meff.light := 3;
                meff.ImgLib := g_WMagic8Images16;
              end;
            end;
          34:begin // 灭天火
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                  (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              if NewLevel = 0 then begin
                // 4级技能强化 -- 4级灭天火 （攻击效果） chongchong 2013-12-04
                if MagicLevel = 4 then begin
                  meff.MagExplosionBase := 100;
                  meff.TargetActor := target; // target;
                  meff.NextFrameTime := 80;
                  meff.ExplosionFrame := 20;
                  meff.light := 3;
                  meff.ImgLib := g_WMagic6Images;
                end
                else begin
                  meff.MagExplosionBase := 140;
                  meff.TargetActor := target; // target;
                  meff.NextFrameTime := 80;
                  meff.ExplosionFrame := 10;
                  meff.light := 3;
                  if wimg <> nil then
                    meff.ImgLib := wimg;
                end;
              end
              else if NewLevel in [1..3] then begin

                meff.MagExplosionBase := 380;
                meff.TargetActor := target; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end
              else if NewLevel in [4..6] then begin
                meff.MagExplosionBase := 390;
                meff.TargetActor := target; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end
              else begin
                meff.MagExplosionBase := 400;
                meff.TargetActor := target; // target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end;
            end;
          40:begin // 净化术
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 620;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 20;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;
          45:begin // 火龙气焰
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                      (abs(meff.targetx - sctx) <= 2) and (abs(meff.targety - scty) <= 2) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                    (abs(meff.targetx - sctx) <= 2) and (abs(meff.targety - scty) <= 2) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 920;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 20;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;
          52:begin // 飓风破
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1010;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 20;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;
          48:begin // 噬血术
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and

                  (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              if NewLevel = 0 then begin
                // 噬血术击中目标后特效 chongchong 2014-05-19
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.NewLevel := NewLevel;
                meff.MagExplosionBase := 1060;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 20;
                meff.light := 3;
                if wimg <> nil then
                  meff.ImgLib := wimg;

                {
                meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
                meff.NewLevel := NewLevel;
                meff.MagExplosionBase := 1130;
                meff.TargetActor := nil;                                                            // target;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 40;
                meff.light := 3;
                if wimg <> nil then
                  meff.ImgLib := wimg;
                }
              end
              else begin
                meff := TBloodBiteEffect.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime, NewLevel); // NewLevel
              end;
            end;
          49:begin // 骷髅咒
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1110;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 10;
              meff.light := 3;
              if wimg <> nil then
                meff.ImgLib := wimg;
            end;
          51:begin // 流星火雨
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                      (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin // 限制同一坐标同一魔法的数量
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              // 传入-1则为 Mon33_7 光圈特效 piaoyun 2013-12-02
              if (anitime = -1) then begin
                boMon33_7Effect := True;
                anitime := 0;
              end;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              if NewLevel = 0 then begin
                if boMon33_7Effect then
                  meff.MagExplosionBase := 650
                else
                  meff.MagExplosionBase := 640;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 55;
                meff.ExplosionFrame := 40;
                meff.light := 3;
                if wimg <> nil then
                  meff.ImgLib := wimg;
              end
              else if NewLevel in [1..3] then begin
                meff.MagExplosionBase := 530;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 30;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end
              else if NewLevel in [4..6] then begin
                meff.MagExplosionBase := 560;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 30;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end
              else begin
                meff.MagExplosionBase := 590;
                meff.TargetActor := nil; // target;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 30;
                meff.light := 3;
                meff.ImgLib := g_WMagic9Images;
              end;
            end;

          4:begin // 施毒术
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                      (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
              if NewLevel = 0 then begin

              end
              else begin
                meff.TargetActor := target;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 10;
                meff.light := 3;
                meff.ImgLib := g_WMagic7Images16;
                if NewLevel in [1..3] then begin
                  if boMagItemType then
                    meff.MagExplosionBase := 620
                  else
                    meff.MagExplosionBase := 830;
                end
                else if NewLevel in [4..6] then begin
                  if boMagItemType then
                    meff.MagExplosionBase := 630
                  else
                    meff.MagExplosionBase := 830;
                end
                else begin
                  if boMagItemType then
                    meff.MagExplosionBase := 640
                  else
                    meff.MagExplosionBase := 830;
                end;
              end;
            end;

          33:begin // 群体施毒术 chongchong 2015-05-23
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                      (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                    (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              meff.TargetActor := target;
              meff.NextFrameTime := 50;
              meff.ExplosionFrame := 24;
              meff.light := 3;
              meff.ImgLib := g_WMagic2Images;
              meff.MagExplosionBase := 950
            end;
          60:begin //
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 170;
              meff.TargetActor := target;
              meff.NextFrameTime := 100;
              meff.ExplosionFrame := 6;
              meff.light := 3;
              meff.ImgLib := g_WMagic4Images;
            end;

          61:begin //
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 495;
              meff.TargetActor := target;
              meff.NextFrameTime := 60;
              meff.ExplosionFrame := 25;
              meff.light := 3;
              meff.ImgLib := g_WMagic4Images;
            end;

          62:begin //
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 390;
              meff.TargetActor := target;
              meff.NextFrameTime := 50;
              meff.ExplosionFrame := 50;
              meff.light := 3;
              meff.ImgLib := g_WMagic4Images;
            end;

          65:begin //
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 560;
              meff.TargetActor := target;
              meff.NextFrameTime := 50;
              meff.ExplosionFrame := 50;
              meff.light := 3;
              meff.ImgLib := g_WMagic4Images;
            end;

          106:begin // 冰天雪地
              meff := TExploBingtianxuediEffect.Create(3150, scx, scy, sctx, scty, target);
              meff.ImgLib := g_cboEffect;
            end;

          55:begin // 倚天辟地
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1391;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 60;
              meff.ExplosionFrame := 14;
              meff.light := 3;
              meff.ImgLib := g_WMagic2Images;
            end;

          70: {// 捕捉宠物} begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1000;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 60;
              meff.ExplosionFrame := 10;
              meff.light := 3;
              meff.ImgLib := g_WNewopUIImages;
            end;
          71: {// 招魂术} begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 1110;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 60;
              meff.ExplosionFrame := 10;
              meff.light := 3;
              meff.ImgLib := g_WMagic2Images;
            end;
          116:begin
              // 血魄一击，这个鸟目标特效特殊处理，因为目标特效不是连续的 chongchong 2018-01-31
              meff := TExplosion2Effect.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 60;

              meff.MagExplosionBase := 2060 + aowner.m_btDir * 10;
              meff.ExplosionFrame := 6;

              TExplosion2Effect(meff).MagExplosionBase_2 := 2150;
              TExplosion2Effect(meff).ExplosionFrame_2 := 10;

              meff.light := 3;
              meff.ImgLib := g_WMagic8Images;
            end;
          117:begin
              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.MagExplosionBase := 2200 + aowner.m_btDir * 20;
              meff.TargetActor := nil; // target;
              meff.NextFrameTime := 60;
              meff.ExplosionFrame := 13;
              meff.light := 3;
              meff.ImgLib := g_WMagic8Images;
            end;
          else begin // 默认
              nC := 0;
              if boLockList then begin
                {$IF IsMultiThreadRender = 1}
                m_EffectList.Lock;
                try
                  {$IFEND}
                  for I := 0 to m_EffectList.Count - 1 do begin
                    meff := TMagicEff(m_EffectList[I]);
                    if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and (meff.EffectBase = effnum) and
                      ((meff.NewLevel = NewLevel) or
                      ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                      ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                      ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                      (abs(meff.targetx - sctx) <= 0) and (abs(meff.targety - scty) <= 0) then begin
                      Inc(nC);
                    end;
                  end;
                  {$IF IsMultiThreadRender = 1}
                finally
                  m_EffectList.UnLock;
                end;
                {$IFEND}
              end
              else begin
                for I := 0 to m_EffectList.Count - 1 do begin
                  meff := TMagicEff(m_EffectList[I]);
                  if (meff.ServerMagicId = magid) and (meff.EffectBase = effnum) and (meff.EffectBase = effnum) and
                    ((meff.NewLevel = NewLevel) or
                    ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                    ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                    ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                    (abs(meff.targetx - sctx) <= 0) and (abs(meff.targety - scty) <= 0) then begin
                    Inc(nC);
                  end;
                end;
              end;
              if nC > 2 then Exit;

              meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
              meff.NewLevel := NewLevel;
              meff.TargetActor := target;
              meff.NextFrameTime := 80;
            end;
        end;
      mtFireWind:
        meff := nil;
      mtFireGun:
        meff := TFireGunEffect.Create(930, scx, scy, sctx, scty);
      mtThunder:begin // 雷电术
          nC := 0;
          if boLockList then begin
            {$IF IsMultiThreadRender = 1}
            m_EffectList.Lock;
            try
              {$IFEND}
              for I := 0 to m_EffectList.Count - 1 do begin
                meff := TMagicEff(m_EffectList[I]);
                if (meff.MagicType = mtThunder) and
                  ((meff.NewLevel = NewLevel) or
                  ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                  ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                  ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                  (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                  Inc(nC);
                end;
              end;
              {$IF IsMultiThreadRender = 1}
            finally
              m_EffectList.UnLock;
            end;
            {$IFEND}
          end
          else begin
            for I := 0 to m_EffectList.Count - 1 do begin
              meff := TMagicEff(m_EffectList[I]);
              if (meff.MagicType = mtThunder) and
                ((meff.NewLevel = NewLevel) or
                ((meff.NewLevel in [1..3]) and (NewLevel in [1..3])) or
                ((meff.NewLevel in [4..6]) and (NewLevel in [4..6])) or
                ((meff.NewLevel >= 7) and (NewLevel >= 7))) and
                (abs(meff.targetx - sctx) <= 1) and (abs(meff.targety - scty) <= 1) then begin
                Inc(nC);
              end;
            end;
          end;
          if nC > 2 then Exit;

          if NewLevel = 0 then begin
            if magnumb = 80 then begin
              meff := TThuderEffect.Create(230 + Random(6) * 10, sctx, scty, nil); // target);
              meff.NewLevel := 0;
              meff.ExplosionFrame := 5;
              meff.ImgLib := g_WDragonImg;
            end
            else begin
              meff := TThuderEffect.Create(10, sctx, scty, nil); // target);
              meff.NewLevel := NewLevel;
              meff.ExplosionFrame := 6;
              meff.ImgLib := g_WMagic2Images;
            end;
          end
          else begin
            if NewLevel in [1..3] then begin
              meff := TThuderEffect.Create(210, sctx, scty, nil); // target);
              meff.NewLevel := NewLevel;
              meff.ExplosionFrame := 6;
              meff.ImgLib := g_WMagic7Images16;
            end
            else if NewLevel in [4..6] then begin
              meff := TThuderEffect.Create(230, sctx, scty, nil); // target);
              meff.NewLevel := NewLevel;
              meff.ExplosionFrame := 6;
              meff.ImgLib := g_WMagic7Images16;
            end
            else begin
              meff := TThuderEffect.Create(250, sctx, scty, nil); // target);
              meff.NewLevel := NewLevel;
              meff.ExplosionFrame := 8;
              meff.ImgLib := g_WMagic7Images16;
            end;
          end;
        end;
      mtLightingThunder: {// 疾光电影 } begin
          if NewLevel = 0 then begin
            meff := TLightingThunder.Create(970, scx, scy, sctx, scty, target);
          end
          else if NewLevel in [1..3] then begin
            meff := TLightingThunder.Create(1100, scx, scy, sctx, scty, target);
            if meff <> nil then
              meff.ImgLib := g_WMagic7Images16;
          end
          else if NewLevel in [4..6] then begin
            meff := TLightingThunder.Create(1270, scx, scy, sctx, scty, target);
            if meff <> nil then
              meff.ImgLib := g_WMagic7Images16;
          end
          else begin
            meff := TLightingThunder.Create(1440, scx, scy, sctx, scty, target);
            if meff <> nil then
              meff.ImgLib := g_WMagic7Images16;
          end;

          // 修正在跑动时释放疾光电影会错位 chongchong 2016-12-27
          if meff <> nil then
            meff.TargetActor := aowner;
        end;
      mtExploBujauk {8 符类}:begin
          case magnumb of // 灵魂火符
            10:begin
                if NewLevel = 0 then begin
                  // 4级技能强化 -- 4级灵魂火符 （符飞行） chongchong 2013-12-04
                  if MagicLevel >= 4 then begin
                    meff := TExploBujaukEffect4.Create(scx, scy, sctx, scty, target);
                  end
                  else begin
                    meff := TExploBujaukEffect.Create(1160, scx, scy, sctx, scty, target);
                    meff.MagExplosionBase := 1360;
                    meff.NewLevel := 0;
                  end;
                end
                else if NewLevel in [1..3] then begin
                  meff := TExploBujaukEffect.Create(600, scx, scy, sctx, scty, target);
                  meff.MagExplosionBase := 1620;
                  meff.ExplosionFrame := 6;
                  meff.ImgLib := g_WMagic8Images16;
                  meff.frame := 8;
                  meff.NewLevel := NewLevel;
                end
                else if NewLevel in [4..6] then begin
                  meff := TExploBujaukEffect.Create(940, scx, scy, sctx, scty, target);
                  meff.MagExplosionBase := 1630;
                  meff.ExplosionFrame := 6;
                  meff.ImgLib := g_WMagic8Images16;
                  meff.NewLevel := NewLevel;
                  meff.frame := 8;
                end
                else begin
                  meff := TExploBujaukEffect.Create(1280, scx, scy, sctx, scty, target);
                  meff.MagExplosionBase := 1640;
                  meff.ExplosionFrame := 6;
                  meff.ImgLib := g_WMagic8Images16;
                  meff.NewLevel := NewLevel;
                  meff.frame := 8;
                end;
              end;
            99:begin
                meff := TJNExploBujaukEffect.Create(1510, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 1590;
                meff.TargetActor := target; //nil;//是目标
                meff.NextFrameTime := 80; //时间
                meff.ExplosionFrame := 8; //往后播放的帧数
                if wimg <> nil then
                  meff.ImgLib := wimg;
              end;
            202: {// 裂神符 piaoyun 2013-6-24} begin
                meff := TExploBujaukEffect.Create(1110, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 1640;
                meff.ExplosionFrame := 6;
                TExploBujaukEffect(meff).MagicBlend := True;
                meff.ImgLib := g_WMagic8Images16;
                // meff.NewLevel := NewLevel;
                meff.frame := 6;
              end;
            17:begin // 集体隐身术
                meff := TExploBujaukEffect.Create(1160, scx, scy, sctx, scty, target);
                meff.MagExplosionBase := 1540;
              end;
            104:begin // 凤舞祭
                meff := TContinuousEffect.Create(2420, scx, scy, sctx, scty, target);
                meff.EffectNumber := 104;
                meff.ImgLib := g_cboEffect;
                meff.TargetActor := target;
                meff.MagExplosionBase := 2580;
                meff.NextFrameTime := 100;
                meff.ExplosionFrame := 8;
                meff.frame := 3;
                meff.NewLevel := 0;
              end;
            105:begin // 惊雷爆
                meff := TContinuousEffect.Create(4230, scx, scy, sctx, scty, target);
                meff.EffectNumber := 105;
                meff.ImgLib := g_cboEffect;
                meff.TargetActor := target;
                meff.MagExplosionBase := 4240;
                meff.NextFrameTime := 100;
                meff.ExplosionFrame := 7;
                meff.frame := 4;
                meff.NewLevel := 0;
              end;
            107:begin // 双龙破
                meff := TContinuousEffect.Create(2610, scx, scy, sctx, scty, target);
                meff.EffectNumber := 107;
                meff.ImgLib := g_cboEffect;
                meff.MagExplosionBase := 2770;
                meff.NextFrameTime := 50;
                meff.ExplosionFrame := 25;
                meff.frame := 5;
                meff.NewLevel := 0;
              end;
            108:begin // 虎啸诀
                meff := TExploHuXiaoJueZhouEffect.Create(3740, scx, scy, sctx, scty, target);
                meff.EffectNumber := 108;
                meff.ImgLib := g_cboEffect;
                meff.MagExplosionBase := 3901;
                meff.ExplosionFrame := 5;
                meff.NewLevel := 0;
              end;
            109:begin // 八卦掌
                meff := TContinuousEffect.Create(2090, scx, scy, sctx, scty, target);
                meff.EffectNumber := 109;
                meff.ImgLib := g_cboEffect;
                meff.TargetActor := target;
                meff.MagExplosionBase := 2250;
                meff.NextFrameTime := 80;
                meff.ExplosionFrame := 10;
                meff.frame := 3;
                meff.NewLevel := 0;
              end;
            110:begin // 三焰咒
                meff := TExploSanYanZhouEffect.Create(3240, scx, scy, sctx, scty, target);
                meff.EffectNumber := 110;
                meff.ImgLib := g_cboEffect;
                meff.MagExplosionBase := 3560;
                meff.ExplosionFrame := 6;
                meff.frame := 5;
                meff.NewLevel := 0;
              end;
            111:begin // 万剑归宗
                meff := TContinuousEffect.Create(2820, scx, scy, sctx, scty, target);
                meff.ImgLib := g_cboEffect;
                meff.EffectNumber := 111;
                meff.TargetActor := target;
                meff.MagExplosionBase := 2980;
                meff.NextFrameTime := 100;
                meff.ExplosionFrame := 10;
                meff.frame := 5;
                meff.NewLevel := 0;
              end;

          end;
          bofly := True;
        end;
      mtBujaukGroundEffect:begin
          meff := TBujaukGroundEffect.Create(1160, magnumb, scx, scy, sctx, scty);
          meff.NewLevel := NewLevel;
          if (NewLevel = 0) or (magnumb = 46) then begin
            case magnumb of
              11:meff.ExplosionFrame := 16; // 幽灵盾
              12:meff.ExplosionFrame := 16; // 神圣战甲术
              46:meff.ExplosionFrame := 10; // 24 诅咒术目标物效修改 chongchong 2015-05-23
              67:meff.ExplosionFrame := 24; // 新诅咒术目标物效修改 chongchong 2015-07-25
            end;
          end
          else begin
            meff.ExplosionFrame := 20;
          end;
          bofly := True;
        end;
      mtKyulKai:begin
          meff := nil; // TKyulKai.Create (1380, scx, scy, sctx, scty);
        end;
      mt12:begin

        end;
      mt13:begin
          meff := TMagicEff.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
          if meff <> nil then begin
            case magnumb of
              32:begin
                  meff.ImgLib := g_WMonImages.Indexs[21];
                  meff.MagExplosionBase := 3580;
                  meff.TargetActor := target;
                  meff.light := 3;
                  meff.NextFrameTime := 20;
                end;
              37:begin
                  meff.ImgLib := g_WMonImages.Indexs[22];
                  meff.MagExplosionBase := 3520;
                  meff.TargetActor := target;
                  meff.light := 5;
                  meff.NextFrameTime := 60;
                end;
            end;
          end;
        end;
      mt14:begin
          meff := TThuderEffect.Create(140, sctx, scty, nil); // target);
          meff.ExplosionFrame := 10;
          meff.ImgLib := g_WMagic2Images;
        end;
      mt15:begin
          meff := TFlyingBug.Create(magid, effnum, scx, scy, sctx, scty, mtype, Recusion, anitime);
          meff.TargetActor := target;
          bofly := True;
        end;
      mt16:begin

        end;
      mtRedThunder:begin
          meff := TRedThunderEffect.Create(230, sctx, scty, nil);
          meff.ExplosionFrame := 6;
        end;
      mtLava:begin
          {case magnumb of
            91: meff := TLavaEffect.Create(470, sctx, scty, nil, 10); //岩浆
            92: meff := TLavaEffect.Create(350, sctx, scty, nil, 34); //火龙守护攻击效果
          end;}
        end;
    end;

    if (meff <> nil) then begin
      meff.TargetRx := tx;
      meff.TargetRy := ty;
      if meff.TargetActor <> nil then begin
        meff.TargetRx := TActor(meff.TargetActor).m_nCurrX;
        meff.TargetRy := TActor(meff.TargetActor).m_nCurrY;
      end;
      meff.MagOwner := aowner;
      if boLockList then begin
        {$IF IsMultiThreadRender = 1}
        m_EffectList.Lock;
        {$IFEND}
        m_EffectList.Add(meff);
        {$IF IsMultiThreadRender = 1}
        m_EffectList.UnLock;
        {$IFEND}
      end
      else begin
        m_EffectList.Add(meff);
      end;
    end;
  end;
end;

function TPlayScene.NewFlyObject(aowner:TActor; cx, cy, tx, ty, targetcode:Int64; mtype:TMagicType):TMagicEff;
var
  scx, scy, sctx, scty:Integer;
  meff:TMagicEff;
begin
  ScreenXYfromMCXY(cx, cy, scx, scy);
  ScreenXYfromMCXY(tx, ty, sctx, scty);
  case mtype of
    mtFlyArrow:meff := TFlyingArrow.Create(1, 1, scx, scy, sctx, scty, mtype, True, 0);
    mtFlyArrowEx:meff := TFlyingArrowEx.Create(1, 1, scx, scy, sctx, scty, mtype, True, 0);
    mt12:meff := TFlyingFireBall.Create(1, 1, scx, scy, sctx, scty, mtype, True, 0);
    mt15:meff := TFlyingBug.Create(1, 1, scx, scy, sctx, scty, mtype, True, 0);
    else
      meff := TFlyingAxe.Create(1, 1, scx, scy, sctx, scty, mtype, True, 0);
  end;
  meff.TargetRx := tx;
  meff.TargetRy := ty;
  meff.TargetActor := FindActor(targetcode);
  meff.MagOwner := aowner;
  {$IF IsMultiThreadRender = 1}
  m_FlyList.Lock;
  {$IFEND}
  m_FlyList.Add(meff);
  {$IF IsMultiThreadRender = 1}
  m_FlyList.UnLock;
  {$IFEND}
  Result := meff;
end;

// 地图座标cx, cy 转换成sx, sY屏幕座标

procedure TPlayScene.ScreenXYfromMCXY(cx, cy:Integer; var sx, sY:Integer);
var
  nWidth, nHeight:Integer;
begin
  if g_MySelf = nil then Exit;

  // sx := (cx - g_MySelf.m_nRx) * UNITX + g_nScreenCenterX + UNITX div 2 - g_MySelf.m_nShiftX;
  // sY := (cy - g_MySelf.m_nRy) * UNITY + g_nScreenCenterY + UNITY div 2 - g_MySelf.m_nShiftY;
  nWidth := abs(g_MySelf.m_nRX - cx);
  nHeight := abs(g_MySelf.m_nRY - cy);
  if g_MySelf.m_nRX > cx then begin
    sx := g_nScreenCenterX - nWidth * UNITX;
  end
  else begin
    sx := g_nScreenCenterX + nWidth * UNITX;
  end;
  if g_MySelf.m_nRY > cy then begin
    sY := g_nScreenCenterY - nHeight * UNITY;
  end
  else begin
    sY := g_nScreenCenterY + nHeight * UNITY;
  end;

  // 骑马一步三格丢地上物品提示 2014-08-14 23:28:21
  sx := sx - g_MySelf.m_nShiftX; // 骑马一步三格2次修正 (-14 改为 -0) chongchong 2014-10-13
  sY := sY - g_MySelf.m_nShiftY; // 人物显示位置下移 (+0改为+14) chongchong 2014-10-13
end;

// 屏幕座标 mx, my转换成ccx, ccy地图座标

procedure TPlayScene.CXYfromMouseXY(mx, my:Integer; var ccx, ccy:Integer);
var
  nWidth, nHeight:Integer;
begin
  if g_MySelf = nil then Exit;

  // ccx := Round((mx - g_nScreenCenterX + g_MySelf.m_nShiftX - UNITX) / UNITX) + g_MySelf.m_nRx;
  // ccy := Round((my - g_nScreenCenterY + g_MySelf.m_nShiftY - UNITY) / UNITY) + g_MySelf.m_nRy;

  nWidth := abs(mx - g_nScreenCenterX + g_MySelf.m_nShiftX) div UNITX;
  nHeight := abs(my - g_nScreenCenterY + g_MySelf.m_nShiftY) div UNITY;
  if (abs(g_nScreenCenterX - mx) mod UNITX > UNITX div 2) then
    Inc(nWidth);

  if (abs(g_nScreenCenterY - my) mod UNITY > UNITY div 2) then
    Inc(nHeight);

  if g_nScreenCenterX > mx then begin
    ccx := g_MySelf.m_nRX - nWidth;
  end
  else begin
    ccx := g_MySelf.m_nRX + nWidth;
  end;

  if g_nScreenCenterY > my then begin
    ccy := g_MySelf.m_nRY - nHeight;
  end
  else begin
    ccy := g_MySelf.m_nRY + nHeight;
  end;

  {nLeft := (WIDTHGRIDCOUNT - 1) * UNITX - UNITX div 2;
  nTop := (HEIGHTGRIDCOUNT - 2) * UNITY - UNITY;

  ccx := Round((mx - nLeft + g_MySelf.m_nShiftX - UNITX) / UNITX) + g_MySelf.m_nRx;
  ccy := Round((my - nTop + g_MySelf.m_nShiftY - UNITY) / UNITY) + g_MySelf.m_nRy;  }

 { if SCREENWIDTH = 800 then begin
    ccx := Round((mx - 364 + g_MySelf.m_nShiftX - UNITX) / UNITX) + g_MySelf.m_nRx;
    ccy := Round((my - 192 + g_MySelf.m_nShiftY - UNITY) / UNITY) + g_MySelf.m_nRy;
  end
  else  if SCREENWIDTH = 900 then begin
    ccx := Round((mx - 390 + g_MySelf.m_nShiftX - UNITX) / UNITX) + g_MySelf.m_nRx;
    ccy := Round((my - 192 + g_MySelf.m_nShiftY - UNITY) / UNITY) + g_MySelf.m_nRy;
  end else begin
    ccx := Round((mx - 485 + g_MySelf.m_nShiftX - UNITX) / UNITX) + g_MySelf.m_nRx;
    ccy := Round((my - 270 + g_MySelf.m_nShiftY - UNITY) / UNITY) + g_MySelf.m_nRy;
  end; }
end;

function TPlayScene.GetCharacter(X, Y, wantsel:Integer; var nowsel:Integer; liveonly:Boolean):TActor;
var
  k, I, ccx, ccy, dx, dy:Integer;
  a:TActor;
  boFind:Boolean;
begin
  Result := nil;

  nowsel := -1;
  CXYfromMouseXY(X, Y, ccx, ccy);
  for k := ccy + 8 downto ccy - 1 do begin
    boFind := False;
    {$IF IsMultiThreadRender = 1}
    m_ActorList.Lock;
    try
      {$IFEND}
      for I := m_ActorList.Count - 1 downto 0 do begin
        a := TActor(m_ActorList.Items[I]);
        if a <> g_MySelf then begin
          //  宠物无实体模式 +((a.m_HumsBBType <> bbGamePet) or (not g_ClientConfig.boPetNoEntity)) 2019-11-15 22:51:27
          if (not liveonly or not a.m_boDeath) and (a.m_boHoldPlace) and (a.m_boVisible) and ((a.m_HumsBBType <> bbGamePet) or (not g_ClientConfig.boPetNoEntity)) then begin
            if (a.m_nCurrY = k) then begin
              dx := (a.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + a.m_nPx + a.m_nShiftX;
              dy := (a.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + a.m_nPy + a.m_nShiftY;

              if a.CheckSelect(X - dx, Y - dy) then begin
                Result := a;
                Inc(nowsel);
                if nowsel >= wantsel then begin
                  boFind := True;
                  break;
                end;
              end;
            end;
          end;
        end;
      end;
      {$IF IsMultiThreadRender = 1}
    finally
      m_ActorList.UnLock;
    end;
    {$IFEND}
    if boFind then break;
  end;
end;

// 取得鼠标所指坐标的角色

function TPlayScene.GetAttackFocusCharacter(X, Y, wantsel:Integer; var nowsel:Integer; liveonly:Boolean):TActor;
var
  k, I, ccx, ccy, dx, dy, centx, centy:Integer;
  a:TActor;
  boFind:Boolean;
  ChrW, ChrH:Integer;
  nPx, nPY:Integer;
begin
  boFind := False;

  Result := GetCharacter(X, Y, wantsel, nowsel, liveonly);
  if Result = nil then begin

    nowsel := -1;
    CXYfromMouseXY(X, Y, ccx, ccy);
    for k := ccy + 8 downto ccy - 1 do begin
      {$IF IsMultiThreadRender = 1}
      m_ActorList.Lock;
      try
        {$IFEND}
        for I := m_ActorList.Count - 1 downto 0 do begin
          a := TActor(m_ActorList.Items[I]);
          if (a <> g_MySelf) then begin
            //  宠物无实体模式 +((a.m_HumsBBType <> bbGamePet) or (not g_ClientConfig.boPetNoEntity)) 2019-11-15 22:51:27
            if (not liveonly or not a.m_boDeath) and (a.m_boHoldPlace) and (a.m_boVisible) and ((a.m_HumsBBType <> bbGamePet) or (not g_ClientConfig.boPetNoEntity)) then begin
              if a.m_nCurrY = k then begin
                nPx := a.m_nPx;
                nPY := a.m_nPy;

                //修正大怪物微端更新资源不完整时点不上 chongchong 2014-11-02
                //如更新时造成 a.m_nPx = -190 a.m_nPy = -90，身体又没更新过来时，这时候很难点上 chongchong 2015-02-02

                if a.m_btHorse > 0 then begin
                  if a.m_HorseSurface = nil then begin
                    nPx := 0;
                    nPy := 0;
                  end;
                end
                else if a.m_BodySurface = nil then begin
                  nPx := 0;
                  nPy := 0;
                end;

                dx := (a.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + nPx + a.m_nShiftX;
                dy := (a.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + nPy + a.m_nShiftY;

                ChrW := a.CharWidth;
                ChrH := a.CharHeight;

                if ChrW > 40 then
                  centx := (ChrW - 40) div 2
                else begin
                  centx := 0;

                  //修正目标太小点不上，微端更新资源不完整时 chongchong 2014-11-02
                  if (ChrW < 10) and (not ((a.m_btRace = 56 {月灵死亡}) and (a.m_boDeath or a.m_boGhost))) then begin
                    ChrW := 60;
                    centx := (ChrW - 40) div 2;
                  end;
                end;

                if ChrH > 70 then
                  centy := (ChrH - 70) div 2
                else begin
                  centy := 0;

                  //修正目标太小点不上，微端更新资源不完整时 chongchong 2014-11-02
                  if (ChrH < 10) and (not ((a.m_btRace = 56 {月灵死亡}) and (a.m_boDeath or a.m_boGhost))) then begin
                    ChrH := 80;
                    centy := (ChrH - 70) div 2;
                  end;
                end;

                if (X - dx >= centx) and (X - dx <= ChrW - centx) and (Y - dy >= centy) and (Y - dy <= ChrH - centy) then begin
                  Result := a;
                  Inc(nowsel);
                  if nowsel >= wantsel then begin
                    boFind := True;
                    break;
                  end;
                end;
              end;
            end;
          end;
        end;
        {$IF IsMultiThreadRender = 1}
      finally
        m_ActorList.UnLock;
      end;
      {$IFEND}
      if boFind then break;
    end;
  end;
end;

function TPlayScene.IsSelectMyself(X, Y:Integer):Boolean;
var
  k, ccx, ccy, dx, dy:Integer;
begin
  Result := False;
  CXYfromMouseXY(X, Y, ccx, ccy);
  for k := ccy + 2 downto ccy - 1 do begin
    if g_MySelf.m_nCurrY = k then begin

      dx := (g_MySelf.m_nRx - Map.m_ClientRect.Left) * UNITX + m_nDefXX + g_MySelf.m_nPx + g_MySelf.m_nShiftX;
      dy := (g_MySelf.m_nRy - Map.m_ClientRect.Top - 1) * UNITY + m_nDefYY + g_MySelf.m_nPy + g_MySelf.m_nShiftY;
      if g_MySelf.CheckSelect(X - dx, Y - dy) then begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

// 取得指定座标地面物品
// x,y 为屏幕座标

function TPlayScene.GetDropItems(X, Y:Integer; var inames:string):pTDropItem;
var
  II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;

  ccx, ccy, ssx, ssy, dx1, dy1, dx2, dy2, nW, nH:Integer;
  S:TTexture;
begin
  Result := nil;

  CXYfromMouseXY(X, Y, ccx, ccy);
  ScreenXYfromMCXY(ccx, ccy, ssx, ssy);
  inames := '';

  g_DropItemsMgr.Lock;
  try
    List := g_DropItemsMgr.GetItemListByPoint(ccx, ccy);
    if List <> nil then begin
      for II := 0 to List.Count - 1 do begin
        DropItem := List.Items[II];

        S := g_WDnItemImages.Images[DropItem.looks];
        if S = nil then Continue;

        nW := S.Width;
        nH := S.Height;

        dx1 := (X - ssx) + (nW div 2) - 3;
        dy1 := (Y - ssy) + (nH div 2);

        // 修正提示有时候不太准 2020-01-06 01:11:13
        if nW > 16 then nW := 16;
        if nH > 16 then nH := 16;
        dx2 := (X - ssx) + (nW div 2) - 3;
        dy2 := (Y - ssy) + (nH div 2);

        // 修正提示有时候不太准 2020-01-06 01:11:13
        if ((dx2 >= -10) and (dx2 <= 10) and (dy2 >= -10) and (dy2 <= 10)) or CheckTextureAlpha(S, dx1, dy1) then begin
          if Result = nil then Result := DropItem;
          begin
            inames := inames + DropItem.Name + '\';
          end;
        end;
      end;
    end;
  finally
    g_DropItemsMgr.UnLock;
  end;
end;

function TPlayScene.GetDropItems(X, Y:Integer; HintList:TList):pTDropItem;
var
  I, II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;
  ItemNameList:THintLines;
  ccx, ccy, ssx, ssy, dx, dy, ix, iy:Integer;
  S:TTexture;
begin
  Result := nil;
  CXYfromMouseXY(X, Y, ccx, ccy);
  ScreenXYfromMCXY(ccx, ccy, ssx, ssy);

  g_DropItemsMgr.Lock;
  try
    for I := 0 to g_DropItemsMgr.Count - 1 do begin
      List := g_DropItemsMgr.Items[I];
      for II := 0 to List.Count - 1 do begin
        DropItem := List.Items[II];
        if DropItem.Visible then begin
          ScreenXYfromMCXY(DropItem.X, DropItem.Y, ix, iy);

          if (X >= ix - DropItem.TextureWidth div 2) and (Y >= iy - DropItem.TextureHeight div 2) and
            (X <= ix + DropItem.TextureWidth div 2) and (Y <= iy + DropItem.TextureHeight div 2) then begin
            S := g_WDnItemImages.Images[DropItem.looks];
            if S = nil then Continue;
            dx := X - (iX - DropItem.TextureWidth div 2);
            dy := Y - (iY - DropItem.TextureHeight div 2);

            if CheckTextureAlpha(S, dx, dy) or
              CheckTextureAlpha(S, dx + 1, dy) or
              CheckTextureAlpha(S, dx - 1, dy) or
              CheckTextureAlpha(S, dx, dy + 1) or
              CheckTextureAlpha(S, dx, dy - 1) then begin
              if Result = nil then Result := DropItem;
              ItemNameList := THintLines.Create;
              ItemNameList.Add(DropItem.Name, clWhite, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
              HintList.Add(ItemNameList);
            end;
          end;
        end;
      end;

    end;
  finally
    g_DropItemsMgr.UnLock;
  end;
end;

procedure TPlayScene.GetXYDropItemsList(nX, nY:Integer; var ItemList:TList);
var
  II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;
begin
  g_DropItemsMgr.Lock;
  try
    List := g_DropItemsMgr.GetItemListByPoint(nX, nY);
    if List <> nil then begin
      for II := 0 to List.Count - 1 do begin
        DropItem := List.Items[II];
        if DropItem.Visible and (DropItem.X = nX) and (DropItem.Y = nY) then begin
          ItemList.Add(DropItem);
          Exit;
        end;
      end;
    end;
  finally
    g_DropItemsMgr.UnLock;
  end;
end;

function TPlayScene.GetXYDropItems(nX, nY:Integer):pTDropItem;
var
  II:Integer;
  List:TPointDropItemList;
  DropItem:pTDropItem;
begin
  g_DropItemsMgr.Lock;
  try
    Result := nil;
    List := g_DropItemsMgr.GetItemListByPoint(nX, nY);
    if List <> nil then begin
      for II := 0 to List.Count - 1 do begin
        DropItem := List.Items[II];
        if DropItem.Visible and (DropItem.X = nX) and (DropItem.Y = nY) then begin
          Result := DropItem;
          Exit;
        end;
      end;
    end;
  finally
    g_DropItemsMgr.UnLock;
  end;
end;

function TPlayScene.CanHorseRun(sx, sY, ex, ey:Integer):Boolean;
var
  ndir, rx, ry:Integer;
begin
  ndir := GetNextDirection(sx, sY, ex, ey);
  rx := sx;
  ry := sY;
  GetNextPosXY(ndir, rx, ry);

  Result := Map.CanMove(rx, ry);

  if not Result then Exit;

  Result := CanWalkEx(rx, ry);

  if not Result then Exit;

  GetNextPosXY(ndir, rx, ry);

  Result := Map.CanMove(rx, ry);

  if not Result then Exit;

  Result := CanWalkEx(rx, ry);

  if not Result then Exit;

  Result := Map.CanMove(ex, ey);

  if not Result then Exit;

  Result := CanWalkEx(ex, ey);
end;

(*
function TPlayScene.UnLockCanRun(sx, sY, ex, ey: Integer): Boolean;
var
  ndir, rx, ry: Integer;
begin
  ndir := GetNextDirection(sx, sY, ex, ey);
  rx := sx;
  ry := sY;
  GetNextPosXY(ndir, rx, ry);

  if Map.CanMove(rx, ry) and Map.CanMove(ex, ey) then
    Result := True
  else
    Result := False;

  if UnLockCanWalkEx(rx, ry) and UnLockCanWalkEx(ex, ey) then
    Result := True
  else
    Result := False;

  // 骑马一步三格 chongchong 2013-10-16 `234  `qew2`
  if (g_MySelf.m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then begin
    GetNextPosXY(ndir, rx, ry);

    if Map.CanMove(rx, ry) and Map.CanMove(ex, ey) then
      Result := True
    else
      Result := False;

    if UnLockCanWalkEx(rx, ry) and UnLockCanWalkEx(ex, ey) then
      Result := True
    else
      Result := FALSE;
  end;
end;
*)

{$MESSAGE HINT '重写了UnLockCanRun, 需要观察'} 
function TPlayScene.UnLockCanRun(sx, sY, ex, ey:Integer):Boolean;
var
  ndir, rx, ry:Integer;
begin
  Result := False;

  ndir := GetNextDirection(sx, sY, ex, ey);
  rx := sx;
  ry := sY;
  GetNextPosXY(ndir, rx, ry); //GetNextPosXY会改变 rx, ry的值

  if Map.CanMove(rx, ry) and Map.CanMove(ex, ey) then begin
    if UnLockCanWalkEx(rx, ry) and UnLockCanWalkEx(ex, ey) then begin
      if (g_MySelf.m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then begin
        GetNextPosXY(ndir, rx, ry);
        if Map.CanMove(rx, ry) and Map.CanMove(ex, ey) then begin
          Result := UnLockCanWalkEx(rx, ry) and UnLockCanWalkEx(ex, ey);
        end;
      end else begin
        Result := True;
      end;
    end;
  end;
end;

// chongchong 2018-09-18

function TPlayScene.CanRun(sx, sY, ex, ey:Integer):Boolean;
var
  ndir, rx, ry:Integer;
begin
  ndir := GetNextDirection(sx, sY, ex, ey);
  rx := sx;
  ry := sY;
  GetNextPosXY(ndir, rx, ry);

  Result := CanWalkEx(rx, ry) and CanWalkEx(ex, ey);

  // 骑马一步三格 chongchong 2013-10-16
  if Result and (g_MySelf.m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then begin
    GetNextPosXY(ndir, rx, ry);
    Result := CanWalkEx(rx, ry);
  end;
end;

function TPlayScene.NewCanRun(sx, sy, ex, ey:Integer):Boolean;
var
  ndir, rx, ry:Integer;
begin
  ndir := GetNextDirection(sx, sy, ex, ey);
  rx := sx;
  ry := sy;
  GetNextPosXY(ndir, rx, ry);

  if NewCanWalkEx(rx, ry) and NewCanWalkEx(ex, ey) then
    Result := True
  else
    Result := False;
end;

function TPlayScene.UnLockCanWalkEx(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.CanMove(mx, my) then
    Result := not UnLockCrashManEx(mx, my);
end;

function TPlayScene.CanWalkEx(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.CanMove(mx, my) then
    Result := not CrashManEx(mx, my);
end;

function TPlayScene.NewCanWalkEx(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.NewCanMove(mx, my) then // Result := TRUE;
    Result := not CrashManEx(mx, my);
end;

function TPlayScene.NewCanWalkEx_2(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.NewCanMove(mx, my) then // Result := TRUE;
    Result := not CrashManEx(mx, my);
end;

// 穿人、穿怪、穿NPC、穿守卫 -- piaoyun 2013-07-19

function TPlayScene.UnLockCrashManEx(mx, my:Integer):Boolean;
var
  I:Integer;
  Actor:TActor;
begin
  Result := False;
  if g_MySelf = nil then Exit;
  if (abs(g_MySelf.m_nCurrX - mx) > 12) or (abs(g_MySelf.m_nCurrY - my) > 12) then Exit;

  for I := 0 to m_ActorList.Count - 1 do begin
    Actor := TActor(m_ActorList.Items[I]);
    if (Actor.m_boVisible) and (Actor.m_boHoldPlace) and (not Actor.m_boDeath) and (Actor.m_nCurrX = mx) and (Actor.m_nCurrY = my) then begin
      //  宠物无实体模式 2019-11-15 22:51:27
      if ((Actor.m_HumsBBType = bbGamePet) and g_ClientConfig.boPetNoEntity) then Continue;

      {
      // 不让穿人物的英雄
      if (Actor = g_MyHero) then
      begin
        Result := True;
        Break;
      end;
      }
      if (Actor.m_btRace in [0, 1]) and (not Actor.m_boPlayMoster) and g_HumanRunConfig.boCanRunHuman then Continue;
      if (Actor.m_btRace = RC_MERCHANT) and (g_HumanRunConfig.boCanRunNpc or (Actor.m_wAppearance in [54..58, 94..98])) then Continue;

      // m_btRace 大于 0 并不等于 50 则为怪物
      if (
        (
        (Actor.m_btRace > RC_HEROOBJECT) and
        (Actor.m_btRace <> RC_MERCHANT) and
        (Actor.m_btRealRace <> 55) and
        (not (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]))
        ) or (
        (Actor.m_btRace = 0) and Actor.m_boPlayMoster
        )
        ) and g_HumanRunConfig.boCanRunMon then Continue;

      // 守卫
      if g_HumanRunConfig.boCanRunGuard and (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]) then Continue;

      // 被邀请骑马的人可被穿 chongchong 2013-10-17
      if (Actor.m_btHorse in [1, 2]) { and (Actor.m_btDoubleHumHorse = 0)} then Continue;

      {
      // 人物可以穿自己的英雄 chongchong 2016-03-11
      if Actor = g_MyHero then Continue;
      }

      Result := True;
      Break;
    end;
  end;
end;

// 穿人、穿怪、穿NPC、穿守卫 -- piaoyun 2013-07-19

function TPlayScene.CrashManEx(mx, my:Integer):Boolean;
var
  I:Integer;
  Actor:TActor;
begin
  Result := False;
  if g_MySelf = nil then Exit;
  if (abs(g_MySelf.m_nCurrX - mx) > 12) or (abs(g_MySelf.m_nCurrY - my) > 12) then Exit;

  // 2018-09-20 01:11:08
  // 这里考虑要不要换成 if DoSearchSortYDrawActtor(J + Map.m_nBlockTop, mmm) then

  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := m_ActorList.Count - 1 downto 0 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (Actor.m_boVisible) and (Actor.m_boHoldPlace) and (not Actor.m_boDeath) and (Actor.m_nCurrX = mx) and (Actor.m_nCurrY = my) then begin
        //  宠物无实体模式 2019-11-15 22:51:27
        if ((Actor.m_HumsBBType = bbGamePet) and g_ClientConfig.boPetNoEntity) then Continue;

        {
        // 不让穿人物的英雄
        if (Actor = g_MyHero) then
        begin
          Result := True;
          Break;
        end;
        }

        if (Actor.m_btRace in [0, 1]) and (not Actor.m_boPlayMoster) and g_HumanRunConfig.boCanRunHuman then Continue;
        if (Actor.m_btRace = RC_MERCHANT) and (g_HumanRunConfig.boCanRunNpc or (Actor.m_wAppearance in [54..58, 94..98])) then Continue;

        // m_btRace 大于 0 并不等于 50 则为怪物
        if (
          (
          (Actor.m_btRace > RC_HEROOBJECT) and
          (Actor.m_btRace <> RC_MERCHANT) and
          (Actor.m_btRealRace <> 55) and
          (not (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]))
          ) or (
          (Actor.m_btRace = 0) and Actor.m_boPlayMoster
          )
          ) and g_HumanRunConfig.boCanRunMon then Continue;

        // 守卫
        if g_HumanRunConfig.boCanRunGuard and (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]) then Continue;

        // 被邀请骑马的人可被穿 chongchong 2013-10-17
        if (Actor.m_btHorse in [1, 2]) { and (Actor.m_btDoubleHumHorse = 0)} then Continue;

        {
        // 人物可以穿自己的英雄 chongchong 2016-03-11
        if Actor = g_MyHero then Continue;
        }

        Result := True;
        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.CrashManEx_2(mx, my:Integer):Boolean;
var
  I:Integer;
  Actor:TActor;
begin
  Result := False;
  if g_MySelf = nil then Exit;
  if (abs(g_MySelf.m_nCurrX - mx) > 12) or (abs(g_MySelf.m_nCurrY - my) > 12) then Exit;

  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := m_ActorList.Count - 1 downto 0 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (Actor.m_boVisible) and (Actor.m_boHoldPlace) and (not Actor.m_boDeath) and (Actor.m_nCurrX = mx) and (Actor.m_nCurrY = my) then begin
        //  宠物无实体模式 2019-11-15 22:51:27
        if ((Actor.m_HumsBBType = bbGamePet) and g_ClientConfig.boPetNoEntity) then Continue;

        {
        // 不让穿人物的英雄
        if (Actor = g_MyHero) then
        begin
          Result := True;
          Break;
        end;
        }
        if (Actor.m_btRace in [0, 1]) and (not Actor.m_boPlayMoster) and g_HumanRunConfig.boCanRunHuman then Continue;
        if (Actor.m_btRace = RC_MERCHANT) and (g_HumanRunConfig.boCanRunNpc or (Actor.m_wAppearance in [54..58, 94..98])) then Continue;

        // m_btRace 大于 0 并不等于 50 则为怪物
        if (
          (
          (Actor.m_btRace > RC_HEROOBJECT) and
          (Actor.m_btRace <> RC_MERCHANT) and
          (Actor.m_btRealRace <> 55) and
          (not (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]))
          ) or (
          (Actor.m_btRace = 0) and Actor.m_boPlayMoster
          )
          ) and g_HumanRunConfig.boCanRunMon then Continue;

        // 守卫
        if g_HumanRunConfig.boCanRunGuard and (Actor.m_btRealRace in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD]) then Continue;

        // 被邀请骑马的人可被穿 chongchong 2013-10-17
        if (Actor.m_btHorse in [1, 2]) { and (Actor.m_btDoubleHumHorse = 0)} then Continue;

        {
        // 人物可以穿自己的英雄 chongchong 2016-03-11
        if Actor = g_MyHero then Continue;
        }

        Result := True;
        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.CanWalk(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.CanMove(mx, my) then
    Result := not CrashMan(mx, my);
end;

function TPlayScene.UnLockCanWalk(mx, my:Integer):Boolean;
begin
  Result := False;
  if Map.CanMove(mx, my) then
    Result := not UnLockCrashMan(mx, my);
end;

function TPlayScene.UnLockCrashMan(mx, my:Integer):Boolean;
var
  I:Integer;
  Actor:TActor;
begin
  Result := False;

  for I := 0 to m_ActorList.Count - 1 do begin
    Actor := TActor(m_ActorList.Items[I]);
    if (Actor.m_boVisible) and (Actor.m_boHoldPlace) and (not Actor.m_boDeath) and (Actor.m_nCurrX = mx) and (Actor.m_nCurrY = my) then begin
      //  宠物无实体模式 2019-11-15 22:51:27
      if ((Actor.m_HumsBBType = bbGamePet) and g_ClientConfig.boPetNoEntity) then Continue;

      // 传送门可走步穿 piaoyun 2013-07-26
      if (Actor.m_btRace = RC_MERCHANT) and (g_HumanRunConfig.boCanRunNpc or (Actor.m_wAppearance in [54..58, 94..98])) then Continue;

      // 被邀请到马上的人可穿 chongchong 2013-10-16
      if (Actor.m_btHorse in [1, 2]) { and (Actor.m_btDoubleHumHorse = 0)} then Continue;

      Result := True;
      Break;
    end;
  end;
end;

function TPlayScene.CrashMan(mx, my:Integer):Boolean;
var
  I:Integer;
  Actor:TActor;
begin
  Result := False;

  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (Actor.m_boVisible) and (Actor.m_boHoldPlace) and (not Actor.m_boDeath) and (Actor.m_nCurrX = mx) and (Actor.m_nCurrY = my) then begin
        //  宠物无实体模式 2019-11-15 22:51:27
        if ((Actor.m_HumsBBType = bbGamePet) and g_ClientConfig.boPetNoEntity) then Continue;

        // 传送门可走步穿 piaoyun 2013-07-26
        // 修正安全区走npc时反弹  chongchong 2016-07-14  // {g_HumanRunConfig.boCanRunNpc or}
        if (Actor.m_btRace = RC_MERCHANT) and ({g_HumanRunConfig.boCanRunNpc or}(Actor.m_wAppearance in [54..58, 94..98])) then Continue;

        // 被邀请到马上的人可穿 chongchong 2013-10-16
        if (Actor.m_btHorse in [1, 2]) { and (Actor.m_btDoubleHumHorse = 0)} then Continue;
        Result := True;
        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.CanFly(mx, my:Integer):Boolean;
begin
  Result := MapCanFly(mx, my);
end;

function TPlayScene.MapCanMove(mx, my:Integer):Boolean;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_CriticalSection);
  try
    {$IFEND}
    Result := Map.CanMove(mx, my);
    {$IF IsMultiThreadRender = 1}
  finally
    LeaveCriticalSection(g_CriticalSection);
  end;
  {$IFEND}
end;

function TPlayScene.MapCanFly(mx, my:Integer):Boolean;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_CriticalSection);
  try
    {$IFEND}
    Result := Map.CanFly(mx, my);
    {$IF IsMultiThreadRender = 1}
  finally
    LeaveCriticalSection(g_CriticalSection);
  end;
  {$IFEND}
end;
{------------------------ Actor ------------------------}

function TPlayScene.FindActorList(id:Int64):TActor;
var
  Index:Integer;
  Actor:TActor;
begin
  Result := nil;
  if DoSearchActor(ID, Index) then begin
    Actor := m_ActorList.Items[Index];
    Result := Actor;
  end;

  {
  for I := 0 to m_ActorList.Count - 1 do
  begin
    Actor := TActor(m_ActorList.Items[I]);
    if (not Actor.m_boDelActor) and (Actor.m_nRecogId = id) then
    begin
      Result := Actor;
      Break;
    end;
  end;
  }
end;

function TPlayScene.FindActor(id:Int64):TActor;
var
  Index:Integer;
  Actor:TActor;
begin
  Result := nil;
  if DoSearchActor(ID, Index) then begin
    Actor := m_ActorList.Items[Index];
    if not Actor.m_boDelActor then
      Result := Actor;
  end;

  (*
{$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
{$IFEND}
    for I := 0 to m_ActorList.Count - 1 do
    begin
      Actor := TActor(m_ActorList.Items[I]);
      if (not Actor.m_boDelActor) and (Actor.m_nRecogId = id) then
      begin
        Result := Actor;
        Break;
      end;
    end;
{$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
{$IFEND}
  *)
end;

function TPlayScene.FindActor(sname:string):TActor;
var
  I:Integer;
  Actor:TActor;
begin
  Result := nil;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (not Actor.m_boDelActor) and (CompareText(Actor.m_sUserName, sname) = 0) then begin
        Result := Actor;
        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.FindActorXY(X, Y:Integer):TActor;
var
  I:Integer;
  Actor:TActor;
begin
  Result := nil;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      Actor := TActor(m_ActorList.Items[I]);
      if (not Actor.m_boDelActor) and (Actor.m_nCurrX = X) and (Actor.m_nCurrY = Y) then begin
        Result := Actor;
        if (not Result.m_boDeath) and Result.m_boVisible and Result.m_boHoldPlace then
          Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.FindActorXY(X, Y:Integer; Actor:TActor):TActor;
var
  I:Integer;
  a:TActor;
begin
  Result := nil;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      a := TActor(m_ActorList.Items[I]);
      if (not a.m_boDelActor) and
        (a.m_nCurrX = X) and
        (a.m_nCurrY = Y) and
        (Actor = a) then begin
        Result := a;
        if not ((not Result.m_boDeath) and Result.m_boVisible and Result.m_boHoldPlace) then
          Result := nil;

        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.IsValidActor(Actor:TActor):Boolean;
var
  I:Integer;
  a:TActor;
begin
  Result := False;

  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      a := TActor(m_ActorList.Items[I]);
      if (not a.m_boDelActor) and (a = Actor) then begin
        Result := True;
        Break;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.IsValidActorEx(Actor:TActor):Boolean;
var
  I:Integer;
  a:TActor;
begin
  Result := False;
  for I := 0 to m_DrawActorList.Count - 1 do begin
    a := TActor(m_DrawActorList.Items[I]);
    if (not a.m_boDelActor) and (a = Actor) then begin
      Result := True;
      Break;
    end;
  end;
end;

function TPlayScene.NewActor(chrid:Int64; cx, cy, cdir:Word; Feature:TFeature; cState:Integer; boLockList:Boolean):TActor;
var
  I:Integer;
  Actor:TActor;
  boCreate:Boolean;

  RaceImg, Race:Integer;

  MonAppr:Word;
  IsFoundCustomMonsterConfig:Boolean;
  MonsterConfig:PClientCustomMonsterConfig;
begin
  boCreate := True;
  Result := nil; // jacky
  if g_boMapMoving then begin
    // DScreen.AddChatBoardString('正在更换地图禁止创建角色', clGreen, clWhite);
    Exit; // 正在更换地图禁止创建角色
  end;

  Actor := FindActorList(chrid);
  if Actor <> nil then begin
    Actor.m_boDelActor := False;
    Result := Actor;
    boCreate := False;
  end;

  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Actor := g_MyHero;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}

  // DScreen.AddChatBoardString(Format('NewActor 1 chrid:%d',
  // [chrid]), clGreen, clWhite);

  if boCreate and (Actor <> nil) and (Actor.m_nRecogId = chrid) then begin
    with Actor do begin
      m_boDelActor := False;
      m_nRecogId := chrid;
      m_nCurrX := cx;
      m_nCurrY := cy;
      m_nRx := m_nCurrX;
      m_nRy := m_nCurrY;
      m_btDir := cdir;
      m_Feature := Feature;
      m_btRace := 1;

      m_nChangeAppr := pTHumFeature(@m_Feature.Buffer).nChangeAppr;
      if m_nChangeAppr >= 0 then
        m_wAppearance := m_nChangeAppr
      else
        m_wAppearance := 0;
      m_btHorse := pTHumFeature(@m_Feature.Buffer).btHorseType;
      m_btDoubleHumHorse := pTHumFeature(@m_Feature.Buffer).btDoubleHumHorseType;
      m_boShowHorseWingsEffect := pTHumFeature(@m_Feature.Buffer).boShowHorseWingsEffect;
      m_btHorseHum := pTHumFeature(@m_Feature.Buffer).btHorseHum;
      m_btHorseHumExpand := pTHumFeature(@m_Feature.Buffer).btHorseHumExpand;
      m_btHorseHair := pTHumFeature(@m_Feature.Buffer).btHorseHair;
      m_btHorseEffectType := pTHumFeature(@m_Feature.Buffer).btHorseEffectType;

      m_boShowFashion := pTHumFeature(@m_Feature.Buffer).boShowFashion;
      m_boMagicShield := pTHumFeature(@m_Feature.Buffer).boMagicShield;
      m_btReLevel := pTHumFeature(@m_Feature.Buffer).btReLevel;

      m_btSex := pTHumFeature(@m_Feature.Buffer).btGender;
      m_btJob := pTHumFeature(@m_Feature.Buffer).btJob;
      m_btHair := pTHumFeature(@m_Feature.Buffer).btHair;
      m_wDress := pTHumFeature(@m_Feature.Buffer).wDress;
      m_wWeapon := pTHumFeature(@m_Feature.Buffer).wWeapon;
      m_wWeaponSound := pTHumFeature(@m_Feature.Buffer).wWeaponSound;
      m_wEffect := pTHumFeature(@m_Feature.Buffer).wDressEffType;
      m_wEffect_30 := pTHumFeature(@m_Feature.Buffer).wDressEffType_30;

      m_boEffectNormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEffNormalDraw;
      m_boEffect_30NormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEff_30NormalDraw;
      m_boDressEffNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffNoSex;
      m_boDressEff_30NoSex := pTHumFeature(@m_Feature.Buffer).boDressEff_30NoSex;

      m_wShield := pTHumFeature(@m_Feature.Buffer).wShield; // 盾牌 chongchong 2013-09-16

      m_btOldHair := pTHumFeature(@m_Feature.Buffer).btOldHair;
      m_boPlayMoster := pTHumFeature(@m_Feature.Buffer).boPlayMoster;

      m_btCaseltGuild := pTHumFeature(@m_Feature.Buffer).btCaseltGuild; // 1=沙行会成员 //2=沙行会掌门
      m_nWeaponEffectIndex := pTHumFeature(@m_Feature.Buffer).nWeaponEffectIndex; // 武器发光效果外观wil 编号
      m_wDBWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDBWeaponEffectOffSet; // 武器发光效果外观DB 2020-11-11 00:50:43
      m_wWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wWeaponEffectOffSet; // 武器发光效外观偏移
      m_nDressEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressEffectIndex; // 衣服发光效外观wil 编号
      m_wDressEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressEffectOffSet; // 衣服发光效外观偏移
      m_nShieldEffectIndex := pTHumFeature(@m_Feature.Buffer).nShieldEffectIndex; // 武器发光效果外观wil 编号
      m_wShieldEffectOffSet := pTHumFeature(@m_Feature.Buffer).wShieldEffectOffSet; // 武器发光效外观偏移

      m_boShopStall := pTHumFeature(@m_Feature.Buffer).boShopStall;

      m_boDressEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressEffectNoBlend;
      m_boDressEffectNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffectNoSex;

      m_nMedalEffectIndex := pTHumFeature(@m_Feature.Buffer).nMedalEffectIndex; // 衣服发光效外观wil 编号
      m_wMedalEffectOffSet := pTHumFeature(@m_Feature.Buffer).wMedalEffectOffSet; // 衣服发光效外观偏移
      m_boMedalEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoBlend;
      m_boMedalEffectNoSex := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoSex;

      m_boWeaponEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoBlend;
      m_boWeaponEffectNoSex := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoSex;
      m_boShieldEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoBlend;
      m_boShieldEffectNoSex := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoSex;

      m_nDressAddEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressAddEffectIndex;
      m_bDressAddEffectOrder := pTHumFeature(@m_Feature.Buffer).bDressAddEffectOrder;
      m_wDressAddEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressAddEffectOffSet;
      m_wDressAddEffectCount := pTHumFeature(@m_Feature.Buffer).wDressAddEffectCount;
      m_wDressAddEffectTime := pTHumFeature(@m_Feature.Buffer).wDressAddEffectTime;
      m_boDressAddEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressAddEffectNoBlend;
      m_boDressAddEffectDrawCenter := pTHumFeature(@m_Feature.Buffer).boDressAddEffectDrawCenter;

      m_btCboDressUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboDressUseDiyImage;
      m_btCboWeaponUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboWeaponUseDiyImage;

      m_sActiveFengHaoName := pTHumFeature(@m_Feature.Buffer).sActiveFengHaoName; // 激活的封号名
      m_nActiveFengHaoID := pTHumFeature(@m_Feature.Buffer).nActiveFengHaoID; // 激活的封号ID
      m_dwActiveFengHaoLooks := pTHumFeature(@m_Feature.Buffer).dwActiveFengHaoLooks; // 激活封号的Looks
      m_btActiveFengHaoReserved := pTHumFeature(@m_Feature.Buffer).btActiveFengHaoReserved;
      m_nActiveFengHaoColor := GetRGB(pTHumFeature(@m_Feature.Buffer).btActiveFengHaoColor); // 激活封号的颜色

      m_btBodyColor := pTHumFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色

      m_boShowHair := pTHumFeature(@m_Feature.Buffer).boShowHair;

      m_Action := nil;
      m_nState := cstate;
      m_SayingArr[0].Text := '';
      LoadSurface(Actor);
    end;

    if boLockList then begin
      {$IF IsMultiThreadRender = 1}
      m_ActorList.Lock;
      {$IFEND}
      //m_ActorList.Add(Actor);
      DoAddActor(Actor);
      {$IF IsMultiThreadRender = 1}
      m_ActorList.UnLock;
      {$IFEND}
    end
    else begin
      //m_ActorList.Add(Actor);
      DoAddActor(Actor);
    end;
    Result := Actor;

    Exit;
  end;

  if boCreate and IsChangingFace(chrid) then boCreate := False;
  if boCreate then begin
    RaceImg := pTMonFeature(@Feature.Buffer).wRaceImg;
    Race := pTMonFeature(@Feature.Buffer).btRace;

    if
      (
      not (
      (Race in [RC_GUARD {卫士}, 12 {巡逻卫士}, RC_ARCHERGUARD {大刀}, 55 {练功师},
      110 {沙巴克城门}, 111 {沙巴克左中右墙}, 112 {弓箭手}, 142 {巡逻箭手}])
      or
      (RaceImg in [0, 1, 50])
      )
      ) then begin
      if pTMonFeature(@Feature.Buffer).HumBBType = bbNo then begin
        if not pTMonFeature(@Feature.Buffer).IsDisableSimpleActor then begin
          if g_ClientConfig.boSimpleShowActor and g_ConfigDlg.ConfigCheckeds[ckSimpleShowActor] then begin
            if not g_ConfigClient.boCustomActorSimpleShow then begin
              // 换装成稻草人
              pTMonFeature(@Feature.Buffer).wRaceImg := 18;
              pTMonFeature(@Feature.Buffer).wAppr := 27;
              pTMonFeature(@Feature.Buffer).btRace := 83;
            end
            else begin
              pTMonFeature(@Feature.Buffer).wRaceImg := g_ConfigClient.nSimpleActorRaceImg;
              pTMonFeature(@Feature.Buffer).wAppr := g_ConfigClient.nSimpleActorAppr;
              pTMonFeature(@Feature.Buffer).btRace := g_ConfigClient.nSimpleActorRace;
            end;
          end;
        end;
      end
      else begin
        if g_ClientConfig.boSimpleShowBB and g_ConfigDlg.ConfigCheckeds[ckSimpleShowBB] then begin
          if not g_ConfigClient.boCustomBBSimpleShow then begin
            // 换装成稻草人
            pTMonFeature(@Feature.Buffer).wRaceImg := 18;
            pTMonFeature(@Feature.Buffer).wAppr := 27;
            pTMonFeature(@Feature.Buffer).btRace := 83;
          end
          else begin
            pTMonFeature(@Feature.Buffer).wRaceImg := g_ConfigClient.nSimpleBBRaceImg;
            pTMonFeature(@Feature.Buffer).wAppr := g_ConfigClient.nSimpleBBAppr;
            pTMonFeature(@Feature.Buffer).btRace := g_ConfigClient.nSimpleBBRace;
          end;
        end;
      end;
    end;

    case pTMonFeature(@Feature.Buffer).wRaceImg of
      0:Actor := THumActor.Create; // 人物
      1:Actor := THeroActor.Create; // 英雄
      9:Actor := TSoccerBall.Create; // 足球
      13:Actor := TKillingHerb.Create; // 食人花
      14:Actor := TSkeletonOma.Create; // 骷髅
      15:Actor := TDualAxeOma.Create; // 掷斧骷髅

      16:Actor := TGasKuDeGi.Create; // 洞蛆

      17:Actor := TCatMon.Create; // 钩爪猫
      18:Actor := THuSuABi.Create; // 稻草人
      19:Actor := TCatMon.Create; // 沃玛战士

      20:Actor := TFireCowFaceMon.Create; // 火焰沃玛
      21:Actor := TCowFaceKing.Create; // 沃玛教主
      22:Actor := TDualAxeOma.Create; // 黑暗战士
      23:Actor := TWhiteSkeleton.Create; // 变异骷髅
      24:Actor := TSuperiorGuard.Create; // 带刀卫士
      30:Actor := TCatMon.Create; // 朝俺窿
      31:Actor := TCatMon.Create; // 角蝇
      32:Actor := TScorpionMon.Create; // 蝎子

      33:Actor := TCentipedeKingMon.Create; // 触龙神
      34:Actor := TBigHeartMon.Create; // 赤月恶魔
      35:Actor := TSpiderHouseMon.Create; // 幻影蜘蛛
      36:Actor := TExplosionSpider.Create; // 月魔蜘蛛
      37:Actor := TFlyingSpider.Create; //

      40:Actor := TZombiLighting.Create; // 僵尸1
      41:Actor := TZombiDigOut.Create; // 僵尸2
      42:Actor := TZombiZilkin.Create; // 僵尸3

      43:Actor := TBeeQueen.Create; // 角蝇巢

      45, 210, 224:Actor := TArcherMon.Create; // 弓箭手   祖玛弓箭手
      47:Actor := TSculptureMon.Create; // 祖玛雕像
      48:Actor := TSculptureMon.Create; //
      49:Actor := TSculptureKingMon.Create; // 祖玛教主

      50:begin
          if pTMonFeature(@Feature.Buffer).wAppr = 273 then
            Actor := TStatuaryNpcActor.Create
          else
            Actor := TNpcActor.Create;
        end;

      52:Actor := TGasKuDeGi.Create; // 楔蛾
      53:Actor := TGasKuDeGi.Create; // 粪虫

      54:Actor := TSmallElfMonster.Create; // 神兽
      55:Actor := TWarriorElfMonster.Create; // 圣兽

      56:Actor := TMoonMon.Create; // 月灵

      60:Actor := TElectronicScolpionMon.Create; // 虹魔蝎卫
      61:Actor := TBossPigMon.Create;
      62:Actor := TKingOfSculpureKingMon.Create;
      63:Actor := TSkeletonKingMon.Create;
      64:Actor := TGasKuDeGi.Create;
      65:Actor := TSamuraiMon.Create;
      66:Actor := TSkeletonSoldierMon.Create;
      67:Actor := TSkeletonSoldierMon.Create;
      68:Actor := TSkeletonSoldierMon.Create;
      69:Actor := TSkeletonArcherMon.Create;
      70:Actor := TBanyaGuardMon.Create; // 牛魔系列
      71:Actor := TBanyaGuardMon.Create;
      72:Actor := TBanyaGuardMon.Create;
      73:Actor := TPBOMA1Mon.Create;
      74:Actor := TCatMon.Create;
      75:Actor := TStoneMonster.Create;
      76:Actor := TSuperiorGuard.Create;
      77:Actor := TStoneMonster.Create;
      78:Actor := TBanyaGuardMon.Create;
      79:Actor := TPBOMA6Mon.Create;
      80:Actor := TMineMon.Create;
      81:Actor := TAngel.Create;

      83:Actor := TFireDragon.Create; // 火龙教主
      84:Actor := TDragonStatue.Create;
      90:Actor := TDragonBody.Create; // 龙
      95:actor := TFireDragonGuard.Create; // 火龙守护兽 piaoyun 2013-08-19

      98:Actor := TWallStructure.Create; // LeftWall
      99:Actor := TCastleDoor.Create; // MainDoor

      100:Actor := TMon23_1.Create;
      101, 103, 104, 105, 115, 116:Actor := TMonEffect.Create;
      102:Actor := TMon24_4.Create;
      106:Actor := TMon26_6.Create;
      107:Actor := TMon27_3.Create;
      108, 109, 110, 350:Actor := TDragonBallMonster.Create;
      111:Actor := TFireSpiritMonster.Create; // 火灵
      112:Actor := TEggMonster.Create;
      113:Actor := TLionMonster.Create; // 狮子 Mon35-0
      114:Actor := TMon35FireDragon.Create; // 蓝龙、金龙
      117:Actor := TMon27_6.Create; // Mon27_6--冰柱
      118:Actor := TMon29_0.Create; // Mon29_0--乌龟

      200:Actor := TMonKuLou_1.Create; // 新骷髅测试 piaoyun  2013-07-27
      201:Actor := TMon27_3.Create;
      202, 203, 204, 205, 206, 207, 208, 209:Actor := TMon36_X.Create; // Mon36_X怪物
      220, 221, 222, 223, 225:Actor := TMon38_X.Create; // Mon38 怪物 piaoyun 2014-01-03

      253:Actor := TXueLingLeaderMonster.Create;

      254:Actor := TSkeletonOma.Create; // 神兽
      255:Actor := TWarriorElfMonster.Create; // 圣兽

      // 自定义怪物 chongchong 2014-07-20
      156:begin
          if (pTMonFeature(@Feature.Buffer).btRace in [154, 155, 156, 157]) then begin
            MonAppr := pTMonFeature(@Feature.Buffer).wAppr;

            IsFoundCustomMonsterConfig := False;
            for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
              MonsterConfig := g_CustomMonsterConfig.Items[I];
              if MonsterConfig.wMonsterAppr = MonAppr then begin
                IsFoundCustomMonsterConfig := True;
                Actor := TCustomActor.Create(MonsterConfig^);
                Break;
              end;
            end;

            if not IsFoundCustomMonsterConfig then begin
              DScreen.AddChatBoardString(Format(DecodeResStr(SCustomMonNoConfig), [MonAppr]), clWhite, clRed);
              Actor := TActor.Create;
            end;
          end
          else
            Actor := TActor.Create;
        end;
      else
        Actor := TActor.Create;
    end;

    with Actor do begin
      m_nRecogId := chrid;
      m_nCurrX := cx;
      m_nCurrY := cy;
      m_nRx := m_nCurrX;
      m_nRy := m_nCurrY;
      m_btDir := cdir;
      m_Feature := Feature;
      m_btRace := pTMonFeature(@Feature.Buffer).wRaceImg;
      m_btRealRace := pTMonFeature(@Feature.Buffer).btRace;
      m_HumsBBType := pTMonFeature(@Feature.Buffer).HumBBType;
      m_IsExploreItem := pTMonFeature(@Feature.Buffer).IsExploreItem;

      if m_btRace in [0, 1] then begin
        m_nChangeAppr := pTHumFeature(@m_Feature.Buffer).nChangeAppr;
        if m_nChangeAppr >= 0 then
          m_wAppearance := m_nChangeAppr
        else
          m_wAppearance := 0;
        m_btHorse := pTHumFeature(@m_Feature.Buffer).btHorseType;
        m_btDoubleHumHorse := pTHumFeature(@m_Feature.Buffer).btDoubleHumHorseType;
        m_boShowHorseWingsEffect := pTHumFeature(@m_Feature.Buffer).boShowHorseWingsEffect;
        m_btHorseHum := pTHumFeature(@m_Feature.Buffer).btHorseHum;
        m_btHorseHumExpand := pTHumFeature(@m_Feature.Buffer).btHorseHumExpand;
        m_btHorseHair := pTHumFeature(@m_Feature.Buffer).btHorseHair;
        m_btHorseEffectType := pTHumFeature(@m_Feature.Buffer).btHorseEffectType;

        m_boShowFashion := pTHumFeature(@m_Feature.Buffer).boShowFashion;
        m_boMagicShield := pTHumFeature(@m_Feature.Buffer).boMagicShield;
        m_btReLevel := pTHumFeature(@m_Feature.Buffer).btReLevel;

        if g_ClientVersion in [cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205] then begin
          if Actor = g_MySelf then begin
            TSerialWindows(FrmDlg).DDownHorse.Visible := (m_btHorse in [1, 2]) and (m_btDoubleHumHorse = 0);
            TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.Checked := m_boShowFashion;

            if m_boShowFashion <> g_ConfigDlg.ConfigCheckeds[ckShowFashion] then begin
              TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.Checked := not m_boShowFashion;
              TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.OnClick(TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion, 0, 0);
            end;
          end
          else if Actor = g_MyHero then
            TSerialWindows(FrmDlg).NewStateWindows.DHeroSWShowFashion.Checked := m_boShowFashion;
        end;

        m_btSex := pTHumFeature(@m_Feature.Buffer).btGender;
        m_btJob := pTHumFeature(@m_Feature.Buffer).btJob;
        m_btHair := pTHumFeature(@m_Feature.Buffer).btHair;
        m_wDress := pTHumFeature(@m_Feature.Buffer).wDress;
        m_wWeapon := pTHumFeature(@m_Feature.Buffer).wWeapon;
        m_wWeaponSound := pTHumFeature(@m_Feature.Buffer).wWeaponSound;
        m_wEffect := pTHumFeature(@m_Feature.Buffer).wDressEffType;
        m_wEffect_30 := pTHumFeature(@m_Feature.Buffer).wDressEffType_30;
        m_wShield := pTHumFeature(@m_Feature.Buffer).wShield; // 盾牌 chongchong 2013-09-16

        m_boEffectNormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEffNormalDraw;
        m_boEffect_30NormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEff_30NormalDraw;
        m_boDressEffNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffNoSex;
        m_boDressEff_30NoSex := pTHumFeature(@m_Feature.Buffer).boDressEff_30NoSex;

        m_btOldHair := pTHumFeature(@m_Feature.Buffer).btOldHair;
        m_boPlayMoster := pTHumFeature(@m_Feature.Buffer).boPlayMoster;

        m_btCaseltGuild := pTHumFeature(@m_Feature.Buffer).btCaseltGuild; // 1=沙行会成员 //2=沙行会掌门
        m_nWeaponEffectIndex := pTHumFeature(@m_Feature.Buffer).nWeaponEffectIndex; // 武器发光效果外观wil 编号
        m_wDBWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDBWeaponEffectOffSet; // 武器发光效果外观DB 2020-11-11 00:51:03
        m_wWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wWeaponEffectOffSet; // 武器发光效外观偏移
        m_nDressEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressEffectIndex; // 衣服发光效外观wil 编号
        m_wDressEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressEffectOffSet; // 衣服发光效外观偏移
        m_nShieldEffectIndex := pTHumFeature(@m_Feature.Buffer).nShieldEffectIndex; // 武器发光效果外观wil 编号
        m_wShieldEffectOffSet := pTHumFeature(@m_Feature.Buffer).wShieldEffectOffSet; // 武器发光效外观偏移

        m_boShopStall := pTHumFeature(@m_Feature.Buffer).boShopStall;
        m_btBodyColor := pTHumFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色

        m_boDressEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressEffectNoBlend;
        m_boDressEffectNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffectNoSex;

        m_nMedalEffectIndex := pTHumFeature(@m_Feature.Buffer).nMedalEffectIndex; // 衣服发光效外观wil 编号
        m_wMedalEffectOffSet := pTHumFeature(@m_Feature.Buffer).wMedalEffectOffSet; // 衣服发光效外观偏移
        m_boMedalEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoBlend;
        m_boMedalEffectNoSex := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoSex;

        m_btCboDressUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboDressUseDiyImage;
        m_btCboWeaponUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboWeaponUseDiyImage;

        m_boWeaponEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoBlend;
        m_boWeaponEffectNoSex := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoSex;
        m_boShieldEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoBlend;
        m_boShieldEffectNoSex := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoSex;

        m_boShowHair := pTHumFeature(@m_Feature.Buffer).boShowHair;

        m_nDressAddEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressAddEffectIndex;
        m_bDressAddEffectOrder := pTHumFeature(@m_Feature.Buffer).bDressAddEffectOrder;
        m_wDressAddEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressAddEffectOffSet;
        m_wDressAddEffectCount := pTHumFeature(@m_Feature.Buffer).wDressAddEffectCount;
        m_wDressAddEffectTime := pTHumFeature(@m_Feature.Buffer).wDressAddEffectTime;
        m_boDressAddEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressAddEffectNoBlend;
        m_boDressAddEffectDrawCenter := pTHumFeature(@m_Feature.Buffer).boDressAddEffectDrawCenter;

        //m_sShowUserName := pTHumFeature(@m_Feature.Buffer).sShowName;

        m_HumsBBType := bbNo;

        m_sActiveFengHaoName := pTHumFeature(@m_Feature.Buffer).sActiveFengHaoName; // 激活的封号名
        m_nActiveFengHaoID := pTHumFeature(@m_Feature.Buffer).nActiveFengHaoID; // 激活的封号ID
        m_dwActiveFengHaoLooks := pTHumFeature(@m_Feature.Buffer).dwActiveFengHaoLooks; // 激活封号的Looks
        m_btActiveFengHaoReserved := pTHumFeature(@m_Feature.Buffer).btActiveFengHaoReserved;
        m_nActiveFengHaoColor := GetRGB(pTHumFeature(@m_Feature.Buffer).btActiveFengHaoColor); // 激活封号的颜色
      end
      else begin
        m_btRace := pTMonFeature(@m_Feature.Buffer).wRaceImg;
        m_wWeapon := 0; // pTMonFeature(@m_Feature.Buffer).wWeapon;
        m_wWeaponSound := 0;
        m_wAppearance := pTMonFeature(@m_Feature.Buffer).wAppr; // Feature.wDress;
        m_wEffect := pTMonFeature(@m_Feature.Buffer).wWeapon;
        m_btBodyColor := pTMonFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色
        m_boShowHair := True;

        m_nChangeAppr := pTMonFeature(@m_Feature.Buffer).nChangeAppr;

        // 2019-08-17 10:17:20
        if pTMonFeature(@Feature.Buffer).MonLevel > 0 then
          m_Abil.Level := pTMonFeature(@Feature.Buffer).MonLevel;
      end;

      { if m_btHair < 6 then begin
         if (m_btHair < 4) or (m_btHorse > 0) then begin
           case m_btSex of
             0: m_nHairOffset := m_btHair * 2 * HUMANFRAME;
             1: m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
           end;
         end else begin
           case m_btHair of
             4: m_nHairOffset := 3600;
             5: m_nHairOffset := 4800;
           else m_nHairOffset := -1;
           end;
         end;
       end else begin
         m_nHairOffset := 3600 + (m_btHair - 6) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
       end;}

      m_Action := nil;
      if m_btRace in [0, 1] then begin
        // m_btSex := m_btDress mod 2;
      end
      else begin
        m_btSex := 0;
      end;
      m_nState := cstate;
      m_SayingArr[0].Text := '';
    end;
    if boLockList then begin
      {$IF IsMultiThreadRender = 1}
      m_ActorList.Lock;
      {$IFEND}
      //m_ActorList.Add(Actor);
      DoAddActor(Actor);
      // modify chongchong 2012-07-13 -----------------------------------------
      // old code: m_ActorList.UnLock;
      {$IF IsMultiThreadRender = 1}
      m_ActorList.UnLock;
      {$IFEND}
      // modify end  ---------------------------------------------------------- ;
    end
    else begin
      //m_ActorList.Add(Actor);
      DoAddActor(Actor);
    end;
    if chrid = g_nMagicTargetRecogId then begin // 修正神兽变身后，锁定失效的问题
      g_nMagicTargetRecogId := 0;
      if g_MagicLockActor = nil then begin
        g_MagicLockActor := Actor;
      end;
    end;
    Result := Actor;
  end;
end;

procedure TPlayScene.ActorDied(Actor:TActor);
//var
  //I: Integer;
  //flag: Boolean;
  //A: TActor;
begin
  // 优化这段代码，不知道搞什么飞机 chongchong 2018-02-13

  // 不明白为什么对象列表有一个未死，又把删掉的加回去 chongchong 2018-02-15

  //DoDelActor(Actor); 加这个会有内存泄露

  {
  for I := 0 to m_ActorList.Count - 1 do
  begin
    if m_ActorList[I] = Actor then
    begin
      m_ActorList.Delete(I);
      Break;
    end;
  end;

  flag := False;
  for I := 0 to m_ActorList.Count - 1 do
  begin
    A := TActor(m_ActorList[I]);
    if not A.m_boDeath then
    begin
      m_ActorList.Insert(I, Actor);
      flag := True;
      Break;
    end;
  end;

  if not flag then m_ActorList.Add(Actor);
  }
end;

procedure TPlayScene.SetActorDrawLevel(Actor:TActor; Level:Integer);
begin
  if Level = 0 then begin
    m_DrawActorList.Remove(Actor);
    m_DrawActorList.Insert(0, Actor);
  end;
end;

procedure TPlayScene.ClearActors;
var
  I:Integer;
  Actor:TActor;
  Effect:TMagicEff;
begin
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := m_ActorList.Count - 1 downto 0 do begin
      Actor := TActor(m_ActorList.Items[I]);

      Actor.m_boGhost := True;
      AddFreeActorList(Actor);

      m_ActorList.Delete(I);
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}

  if g_MyHero <> nil then begin
    g_MyHero.m_boGhost := True;
    AddFreeActorList(g_MyHero);
  end;

  m_DrawActorList.Clear;

  m_MsgList.Clear;

  m_SortYDrawActorList.Clear;

  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  if g_MySelf <> nil then begin
    g_MySelf.ProcLastMsg;
    g_MySelf.CleanMsgs;
    // g_MySelf.Finalize;
  end;

  if g_MyHero <> nil then begin
    g_MyHero.CleanMsgs;
    // g_MyHero.Finalize;
  end;

  g_BrightActor := nil;
  g_MySelf := nil;
  g_MyHero := nil;
  g_TargetCret := nil;
  g_FocusCret := nil;
  g_FocusCretTick := MyGetTickCount;
  g_MagicTarget := nil;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}

  g_LockTarget := nil;
  g_dwLockTargetTick := MyGetTickCount;

  {$IF IsMultiThreadRender = 1}
  m_GroundEffectList.Lock;
  try
    {$IFEND}
    for I := 0 to m_GroundEffectList.Count - 1 do begin
      Effect := TMagicEff(m_GroundEffectList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_GroundEffectList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_GroundEffectList.UnLock;
  end;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_EffectList.Lock;
  try
    {$IFEND}
    for I := 0 to m_EffectList.Count - 1 do begin
      Effect := TMagicEff(m_EffectList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_EffectList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_EffectList.UnLock;
  end;
  {$IFEND}

  {$IF IsMultiThreadRender = 1}
  m_FlyList.Lock;
  try
    {$IFEND}
    for I := 0 to m_FlyList.Count - 1 do begin
      Effect := TMagicEff(m_FlyList[I]);
      Effect.m_dwGhostTick := MyGetTickCount;
      AddFreeEffectList(Effect);
    end;
    m_FlyList.Clear;
    {$IF IsMultiThreadRender = 1}
  finally
    m_FlyList.UnLock;
  end;
  {$IFEND}
  EventMan.ClearEvents;

  frmMain.ClearDropItems;

  // g_PlaySound.Clear;

  m_nProcDropItemsIdx := 0;

  {m_DrawDropItemList.Clear;
  m_DrawActorList.Clear;
  m_DrawGroundEffectList.Clear;
  m_DrawEffectList.Clear;
  m_DrawFlyList.Clear;
  m_DrawEventList.Clear; }
end;

procedure TPlayScene.DeleteActor(id:Int64);
var
  Index:Integer;
  Actor:TActor;
begin
  Actor := nil;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    if DoSearchActor(Id, Index) then begin
      Actor := m_ActorList.Items[Index];
    end;

    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}

  if Actor <> nil then begin
    m_DrawActorList.Remove(Actor);

    if (Actor = g_MyHero) then begin
      Actor.m_dwDeleteTime := MyGetTickCount;
      Actor.m_boDelActor := True;
      Actor.m_boFreeActor := False;
      Actor.CleanMsgs;
    end
    else begin
      Actor.m_dwDeleteTime := MyGetTickCount;
      Actor.m_boDelActor := True;
      Actor.m_boFreeActor := True;
    end;
  end;
end;

procedure TPlayScene.DelActor(Actor:TActor);
var
  Index:Integer;
begin
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    if DoSearchActor(Actor.m_nRecogId, Index) then begin
      Actor.m_dwDeleteTime := MyGetTickCount;
      Actor.m_boDelActor := True;
      Actor.m_boFreeActor := True;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

function TPlayScene.ButchAnimal(X, Y:Integer):TActor;
var
  I:Integer;
  a:TActor;
begin
  Result := nil;
  {$IF IsMultiThreadRender = 1}
  m_ActorList.Lock;
  try
    {$IFEND}
    for I := 0 to m_ActorList.Count - 1 do begin
      a := TActor(m_ActorList[I]);
      if a.m_boDeath and (not (a.m_btRace in [1])) then begin
        if (abs(a.m_nCurrX - X) = 0) and (abs(a.m_nCurrY - Y) = 0) then begin
          Result := a;
          Break;
        end;
      end;
    end;

    if Result = nil then begin
      for I := 0 to m_ActorList.Count - 1 do begin
        a := TActor(m_ActorList[I]);
        if a.m_boDeath and (not (a.m_btRace in [1])) then begin
          if (abs(a.m_nCurrX - X) <= 1) and (abs(a.m_nCurrY - Y) <= 1) then begin
            Result := a;
            Break;
          end;
        end;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    m_ActorList.UnLock;
  end;
  {$IFEND}
end;

{------------------------- Msg -------------------------}
//HZQ 20230821 名字改为 RecvMsg更好一些, 实际时处理接收的消息
function TPlayScene.SendMsg(ident:Integer; chrid:Int64; X, Y, cdir:Integer; Feature:pTFeature; State:Int64; Str:string):TActor;
resourcestring
  sFTest = '你的好友[%s]出现在坐标(%d:%d)，方向 %s';
  sBTest = '你的敌人[%s]出现在坐标(%d:%d)，方向 %s';
var
  I, Index:Integer;
  Actor:TActor;
  Newchrid:Int64;
  Magic:pTClientMagic;
  MDFMagic:PTClientMagic;
begin
  Actor := nil; //HZQ 20230524
  Result := nil;

  case ident of
    SM_TEST:begin
        Actor := NewActor(111, 254 {x}, 214 {y}, 0, Feature^, 0); // NewActor(111, 254 {x}, 214 {y}, 0, 0, 0);
        g_MySelf := THumActor(Actor);
        Map.LoadMap('0', g_MySelf.m_nCurrX, g_MySelf.m_nCurrY);
      end;

    SM_CHANGEMAP,
      SM_NEWMAP:begin
        { TODO -c修改 -opiaoyun : ★★★★★★暂时注释微端更新，增加处理速度 【2013-5-19】}

        {$IF TESTMODE = 1}
        DScreen.AddChatBoardString('使用物品到换地图间隔1:' + IntToStr(MyGetTickCount - m_EatItemTick), clWhite, clBlack);
        {$IFEND}

        //g_boFirstNewMapMsg := False;

        g_sMapName := Str;

        Map.LoadMap(Str, X, Y);
        Map.m_sOldMap := '';
        g_nDarkLevel := cdir;
        if g_nDarkLevel = 0 then
          g_boViewFog := False
        else
          g_boViewFog := True;

        if g_boViewFog then begin
          case g_nDarkLevel of
            1:g_nDarkValue := 10;
            2:g_nDarkValue := 160;
          end;
        end;

        {g_boWeatherEffect1 := False;            // 天气效果
        g_boWeatherEffect2 := False;            // 天气效果
        g_boWeatherEffect3 := False;            // 天气效果}

        if g_boViewMiniMap or (TSerialWindows(FrmDlg).DMinMapDlgEx.Visible) then begin
          // BoViewMiniMap := False;
          g_nMiniMapIndex := -1;
          frmMain.SendWantMiniMap;
        end else begin
          g_nMiniMapIndex := -1;
          if (g_ClientConfig.boUseFindPath and FrmDlg.CheckDMinMapBigDlgVisible) then
            frmMain.SendWantMiniMap
          else if FrmDlg.IsDGJPointsShow then
            FrmMain.SendWantMiniMap(True);
        end;

        if (ident = SM_NEWMAP) and (g_MySelf <> nil) then begin
          g_MySelf.m_nCurrX := X;
          g_MySelf.m_nCurrY := Y;
          g_MySelf.m_nRx := X;
          g_MySelf.m_nRy := Y;
          DelActor(g_MySelf);
          g_MySelf := nil;
        end else begin
          // if g_MySelf <> nil then
            // g_MySelf.Finalize;

        end;
        g_boCanDrawTileMap := True;

        // g_dwRunTick := MyGetTickCount - 20;
        // frmMain.TimerRenderTimer(Self);

        {$IF TESTMODE = 1}
        DScreen.AddChatBoardString('使用物品到换地图间隔2:' + IntToStr(MyGetTickCount - m_EatItemTick), clWhite, clBlack);
        {$IFEND}
      end;
    SM_LOGON:begin
        g_dwRenewSelfLogOutTick := MyGetTickCount;
        Actor := FindActor(chrid);
        if Actor = nil then begin
          Actor := NewActor(chrid, X, Y, LoByte(cdir), Feature^, State);
          if Actor <> nil then begin
            Actor.m_nChrLight := Hibyte(cdir);
            cdir := Lobyte(cdir);

            if Actor is TCustomActor then
              TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;

            if Feature <> nil then
              Actor.SendMsg(SM_TURN, X, Y, cdir, Feature^, State, '', 0)
            else
              Actor.SendMsg(SM_TURN, X, Y, cdir, State, '', 0);
          end;
          // Actor.SendMsg(SM_TURN, X, Y, cdir, Feature^, State, '', 0);
        end;

        {$IF IsMultiThreadRender = 1}
        EnterCriticalSection(g_CriticalSection);
        try
          {$IFEND}
          if Actor <> nil then begin
            if g_MySelf <> nil then begin
              DelActor(g_MySelf);
              g_MySelf := nil;
            end;
            g_dwCheckModuleTick := MyGetTickCount;
            g_MySelf := THumActor(Actor);
            g_boCanDrawTileMap := True;

            if g_ClientVersion in [cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205] then begin
              if Actor = g_MySelf then begin
                TSerialWindows(FrmDlg).DDownHorse.Visible := (Actor.m_btHorse in [1, 2]) and (Actor.m_btDoubleHumHorse = 0);
                TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.Checked := Actor.m_boShowFashion;
              end else if Actor = g_MyHero then begin
                TSerialWindows(FrmDlg).NewStateWindows.DHeroSWShowFashion.Checked := Actor.m_boShowFashion;
              end;
            end;

            {if g_ClientVersion <= cvSerial then
              for I := 0 to 49 do
                g_WNewopUIImages.Images[I];}

            // 登陆即切换界面 -- piaoyun 2013-08-16
            if FrmDlg is TSerialWindows then begin
              case g_ClientConfig.boStateWindowsType of
                0:
                  TSerialWindows(FrmDlg).Set176StateWindows;
                1:
                  TSerialWindows(FrmDlg).Set185StateWindows;
                2:
                  TSerialWindows(FrmDlg).SetSerialStateWindows;
              end;
            end;
          end;
          {$IF IsMultiThreadRender = 1}
        finally
          LeaveCriticalSection(g_CriticalSection);
        end;
        {$IFEND}
        g_ConfigDlg.Logon(g_sServerName);
        g_boCanDrawTileMap := True;

        SaveOrLoadMyBagItemList(False);
      end;

    SM_HEROLOGON, SM_MYHEROLOGON:begin
        Actor := FindActor(chrid);
        if Actor = nil then
          Actor := NewActor(chrid, X, Y, Lobyte(cdir), Feature^, State);
        if Actor <> nil then begin
          { 召出英雄后立马切换地图，英雄图标不显示  chongchong 2013-09-15 }
          if ident = SM_MYHEROLOGON then
            g_MyHero := THeroActor(Actor);

          Actor.m_btSex := HiByte(cdir);
          cdir := LoByte(cdir);

          if Feature <> nil then
            Actor.SendMsg(SM_TURN, X, Y, cdir, Feature^, State, '', 0)
          else
            Actor.SendMsg(SM_TURN, X, Y, cdir, State, '', 0);

          AddEffectList(THeroShowEffect.Create(800, 10, Actor));
          PlaySound(s_HeroLogOn);

          if g_ClientVersion in [cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205] then begin
            if Actor = g_MySelf then begin
              TSerialWindows(FrmDlg).DDownHorse.Visible := (Actor.m_btHorse in [1, 2]) and (Actor.m_btDoubleHumHorse = 0);
              TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.Checked := Actor.m_boShowFashion;
            end else if Actor = g_MyHero then begin
              TSerialWindows(FrmDlg).NewStateWindows.DHeroSWShowFashion.Checked := Actor.m_boShowFashion;
            end;
          end;
        end;
      end;

    SM_HIDE, SM_DISAPPEARMYHERO:begin
        Actor := FindActor(chrid);
        // 修复英雄尸体清理后偶尔图标不会消失 chongchong 2013-11-23
        //if (Actor = g_MyHero) and (Actor.m_boDeath) {修复英雄离主人距离远后窗体消失 piaoyun 2013-09-11} then
        if ident = SM_DISAPPEARMYHERO then begin
          FrmDlg.CloseDHeroStateDlg;
          FrmDlg.CloseDHeroItemBagDlg;
          FrmDlg.CloseDHeroStateWinDlg;
          g_WaitingHeroUseItem.Item.s.Name := '';
          g_HeroEatingItem.s.Name := '';

          g_MyHero := nil;
        end;

        if Actor = nil then Exit;

        if Actor <> nil then begin
          Int64Rec(Newchrid).Lo := X; // 神兽变身才用了这个东东
          Int64Rec(Newchrid).Hi := Y;
          if (Newchrid <> 0) and (g_MagicLockActor = Actor) then begin
            g_MagicLockActor := FindActor(Newchrid);
            if g_MagicLockActor = nil then begin
              g_nMagicTargetRecogId := Newchrid;
            end;
          end;

          if Actor.m_boDelActionAfterFinished then begin
            Exit;
          end;

          if Actor.m_nWaitForRecogId <> 0 then begin
            Exit;
          end;

          // 修复英雄尸体清理后偶尔图标不会消失 chongchong 2013-11-23
          if (g_MyHero = Actor) then begin
            Actor.m_dwDeleteTime := MyGetTickCount;
            Actor.m_boDelActor := True;
            Actor.m_boFreeActor := False;
            Actor.CleanMsgs;
          end else begin
            Actor.m_dwDeleteTime := MyGetTickCount;
            Actor.m_boDelActor := True;
            Actor.m_boFreeActor := True;
          end;
          // DScreen.AddChatBoardString(Format('SM_HIDE nIdx:%d', [chrid]), clGreen, clWhite);
        end;
      end;
    else begin
        Actor := FindActor(chrid);

        if (Actor = nil) and ((ident = SM_TURN) or (ident = SM_DEATH) or (ident = SM_NOWDEATH) or (ident = SM_SKELETON) or (ident = SM_DIGUP) or (ident = SM_ALIVE)) and (Feature <> nil) then begin
          Actor := NewActor(chrid, X, Y, LoByte(cdir), Feature^, State);

          // 一个对象刚好死亡一瞬间发过来，后面可能没有动作，导致这个对象永不死亡 chongchong 2018-08-25 23:55:32
          if (Actor <> nil) and ((ident = SM_DEATH) or (ident = SM_NOWDEATH)) then begin
            Actor.m_boStruckShowNumber := False;
            Actor.m_boShowBigHPProgress := False;
            Actor.m_boDeath := True;
            Actor.m_dwDeathTick := TimeGetTime;
          end;
        end;

        // if (ident = SM_TURN) and (Actor<>nil) then
       // DScreen.AddChatBoardString('(ident = SM_TURN):' + ' m_btDir:' + IntToStr(Actor.m_btDir)+' m_wAppearance:' + IntToStr(Actor.m_wAppearance), clRed, clWhite);

        if ((ident = SM_TURN) or (ident = SM_RUN) or (ident = SM_HORSERUN) or (ident = SM_WALK) or
          (ident = SM_BACKSTEP) or (ident = SM_MAGICMOVE) {十步一杀移动} or
          (ident = SM_DEATH) or (ident = SM_NOWDEATH) or (ident = SM_SKELETON) or
          (ident = SM_DIGUP) or (ident = SM_ALIVE)) or ((ident >= SM_CUSTOM_MAGICMOVE001) and (iDent < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT)) then begin
          // if Actor = nil then
          // Actor := NewActor(chrid, X, Y, LoByte(cdir), Feature^, State);

          if Actor <> nil then begin
            if ident <> SM_BACKSTEP then begin
              Actor.m_nChrLight := Hibyte(cdir);
              if Actor is TCustomActor then
                TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;
              cdir := Lobyte(cdir);
            end else begin
              if Actor is TCustomActor then
                TCustomActor(Actor).m_nOldChrLight := Actor.m_nChrLight;
            end;

            if ident = SM_SKELETON then begin
              // DScreen.AddChatBoardString('Actor.m_boDeath 1', clRed, clWhite);

              Actor.m_boDeath := True;
              Actor.m_dwDeathTick := MyGetTickCount;
              Actor.m_boSkeleton := True;
              Actor.m_boStruckShowNumber := False;
              Actor.m_boShowBigHPProgress := False;
              Actor.m_boSendQueryBigHPProgress := False;
              if Actor = g_MySelf then begin
                g_boCanDrawTileMap := True;
              end;
            end;
            //////////////////////////////////////////////////////////////////////
            // 近身开盾 piaoyun 2013-09-10
            if ((ident = SM_RUN) or (ident = SM_WALK) or (ident = SM_TURN)) then begin
              if (g_MySelf <> nil) and
                (abs(g_MySelf.m_nCurrX - X) <= 2) and
                (abs(g_MySelf.m_nCurrY - Y) <= 2) then begin
                if (not g_MySelf.m_boDeath) and (g_ClientConfig.boHumStruckShield and g_ConfigDlg.ConfigCheckeds[ckHumStruckShield])
                  {(g_MySelf.m_btJob = 1) and }{and CanUseMagic and CanNextAction(caSpell) and ServerAcceptNextAction}then begin
                  if MyGetTickCount - g_dwHumStruckShieldTick > 600 then begin
                    g_dwHumStruckShieldTick := MyGetTickCount;

                    g_MagicList.Lock;
                    try
                      // 战士
                      MDFMagic := nil;

                      if g_MySelf.m_btJob = 0 then begin
                        for I := 0 to g_MagicList.Count - 1 do begin
                          Magic := g_MagicList.Items[I];
                          if Magic.Def.wMagicId = 87 then {// 武力盾} begin
                            MDFMagic := Magic;
                            Break;
                          end;
                        end;

                        if MDFMagic <> nil then begin
                          if g_MySelf.m_nState and $00100000 = 0 then begin
                            if (MDFMagic.Def.wSpell + MDFMagic.Def.wDefSpell <= g_MySelf.m_Abil.MP) then begin
                              FrmMain.UseMagic(g_nMouseX, g_nMouseY, MDFMagic);
                            end;
                          end;
                        end else begin
                          for I := 0 to g_MagicList.Count - 1 do begin
                            Magic := g_MagicList.Items[I];
                            if Magic.Def.wMagicId = 88 then {// 新武力盾} begin
                              MDFMagic := Magic;
                              Break;
                            end;
                          end;

                          if MDFMagic <> nil then begin
                            if g_MySelf.m_nState and $00040000 = 0 then begin
                              if (MDFMagic.Def.wSpell + MDFMagic.Def.wDefSpell <= g_MySelf.m_Abil.MP) then begin
                                FrmMain.UseMagic(g_nMouseX, g_nMouseY, MDFMagic);
                              end;
                            end;
                          end;
                        end;
                      end else if g_MySelf.m_btJob = 1 then begin  // 法师
                        for I := 0 to g_MagicList.Count - 1 do begin
                          Magic := g_MagicList.Items[I];
                          if Magic.Def.wMagicId = 31 then {// 魔法盾} begin
                            MDFMagic := Magic;
                            Break;
                          end;
                        end;

                        if MDFMagic <> nil then begin
                          if g_MySelf.m_nState and $00100000 = 0 then begin
                            if (MDFMagic.Def.wSpell + MDFMagic.Def.wDefSpell <= g_MySelf.m_Abil.MP) then begin
                              FrmMain.UseMagic(g_nMouseX, g_nMouseY, MDFMagic);
                            end;
                          end;
                        end;
                      end else if g_MySelf.m_btJob = 2 then begin // 道士
                        for I := 0 to g_MagicList.Count - 1 do begin
                          Magic := g_MagicList.Items[I];
                          if Magic.Def.wMagicId = 73 then {// 道力盾} begin
                            MDFMagic := Magic;
                            Break;
                          end;
                        end;

                        if MDFMagic <> nil then begin
                          if g_MySelf.m_nState and $00100000 = 0 then begin
                            if (MDFMagic.Def.wSpell + MDFMagic.Def.wDefSpell <= g_MySelf.m_Abil.MP) then begin
                              FrmMain.UseMagic(g_nMouseX, g_nMouseY, MDFMagic);
                            end;
                          end;
                        end else begin
                          for I := 0 to g_MagicList.Count - 1 do begin
                            Magic := g_MagicList.Items[I];
                            if Magic.Def.wMagicId = 89 then {// 新道力盾} begin
                              MDFMagic := Magic;
                              Break;
                            end;
                          end;

                          if MDFMagic <> nil then begin
                            if g_MySelf.m_nState and $00020000 = 0 then begin
                              if (MDFMagic.Def.wSpell + MDFMagic.Def.wDefSpell <= g_MySelf.m_Abil.MP) then begin
                                FrmMain.UseMagic(g_nMouseX, g_nMouseY, MDFMagic);
                              end;
                            end;
                          end;
                        end;
                      end;
                    finally
                      g_MagicList.UnLock;
                    end;
                  end;
                end;
              end;

              if (g_MySelf <> nil) then begin
                if (abs(g_MySelf.m_nCurrX - X) <= 4) and (abs(g_MySelf.m_nCurrY - Y) <= 4) then begin
                  if Actor.m_btRace = 0 then begin
                    // 好友近身提示 piaoyun 2013-09-11
                    if g_ConfigDlg.ConfigCheckeds[ckFriendHit] then begin
                      if TSerialWindows(FrmDlg).DMemoFriend.Lines.IndexOf(Actor.m_sUserName) <> -1 then begin
                        Index := g_MySelf.m_FriendHitList.IndexOf(Actor.m_sUserName);
                        if Index = -1 then begin
                          with Actor do
                            DScreen.AddChatBoardString(Format(sFTest, [m_sUserName, m_nCurrX,
                              m_nCurrY, DirsStr[GetNextDirection(g_MySelf.m_nCurrX, g_MySelf.m_nCurrY,
                                m_nCurrX, m_nCurrY)]]), clBlue, clWhite);
                        end;
                      end;
                    end;

                    // 黑名单近身提示 piaoyun 2013-09-11
                    if g_ConfigDlg.ConfigCheckeds[ckBlacklistHit] then begin
                      if TSerialWindows(FrmDlg).DMemoHeiMingDan.Lines.IndexOf(Actor.m_sUserName) <> -1 then begin
                        Index := g_MySelf.m_FriendHitList.IndexOf(Actor.m_sUserName);
                        if Index = -1 then begin
                          with Actor do
                            DScreen.AddChatBoardString(Format(sBTest, [m_sUserName, m_nCurrX,
                              m_nCurrY, DirsStr[GetNextDirection(g_MySelf.m_nCurrX, g_MySelf.m_nCurrY,
                                m_nCurrX, m_nCurrY)]]), clRed, clWhite);
                        end;
                      end;
                    end;

                    Index := g_MySelf.m_FriendHitList.IndexOf(Actor.m_sUserName);
                    if Index = -1 then
                      g_MySelf.m_FriendHitList.Add(Actor.m_sUserName);
                  end;
                end else begin
                  if Actor.m_btRace = 0 then begin
                    Index := g_MySelf.m_FriendHitList.IndexOf(Actor.m_sUserName);
                    if Index <> -1 then
                      g_MySelf.m_FriendHitList.Delete(Index);
                  end;
                end;
              end;
            end;
            //////////////////////////////////////////////////////////////////////

          end;
        end;

        if Actor = nil then Exit;

        case ident of
          SM_FEATURECHANGED:begin
              Actor.m_Feature := Feature^;

              // 开启内挂的怪物简装，M.SetBodyColor 250 7改变怪物身体颜色，会让怪物显示出原型
              if
                (
                not (
                (Actor.m_btRace in [RC_GUARD {卫士}, 12 {巡逻卫士}, RC_ARCHERGUARD {大刀}, 55 {练功师},
                110 {沙巴克城门}, 111 {沙巴克左中右墙}, 112 {弓箭手}, 142 {巡逻箭手}])
                or
                (pTMonFeature(@Actor.m_Feature.Buffer).wRaceImg in [0, 1, 50])
                )
                ) then begin
                if pTMonFeature(@Actor.m_Feature.Buffer).HumBBType = bbNo then begin
                  if not pTMonFeature(@Feature.Buffer).IsDisableSimpleActor then begin
                    if g_ClientConfig.boSimpleShowActor and g_ConfigDlg.ConfigCheckeds[ckSimpleShowActor] then begin
                      if not g_ConfigClient.boCustomActorSimpleShow then begin
                        // 换装成稻草人
                        pTMonFeature(@Actor.m_Feature.Buffer).wRaceImg := 18;
                        pTMonFeature(@Actor.m_Feature.Buffer).wAppr := 27;
                        pTMonFeature(@Actor.m_Feature.Buffer).btRace := 83;
                      end
                      else begin
                        pTMonFeature(@Actor.m_Feature.Buffer).wRaceImg := g_ConfigClient.nSimpleActorRaceImg;
                        pTMonFeature(@Actor.m_Feature.Buffer).wAppr := g_ConfigClient.nSimpleActorAppr;
                        pTMonFeature(@Actor.m_Feature.Buffer).btRace := g_ConfigClient.nSimpleActorRace;
                      end;
                    end;
                  end;
                end
                else begin
                  if g_ClientConfig.boSimpleShowBB and g_ConfigDlg.ConfigCheckeds[ckSimpleShowBB] then begin
                    if not g_ConfigClient.boCustomBBSimpleShow then begin
                      // 换装成稻草人
                      pTMonFeature(@Actor.m_Feature.Buffer).wRaceImg := 18;
                      pTMonFeature(@Actor.m_Feature.Buffer).wAppr := 27;
                      pTMonFeature(@Actor.m_Feature.Buffer).btRace := 83;
                    end
                    else begin
                      pTMonFeature(@Actor.m_Feature.Buffer).wRaceImg := g_ConfigClient.nSimpleBBRaceImg;
                      pTMonFeature(@Actor.m_Feature.Buffer).wAppr := g_ConfigClient.nSimpleBBAppr;
                      pTMonFeature(@Actor.m_Feature.Buffer).btRace := g_ConfigClient.nSimpleBBRace;
                    end;
                  end;
                end;
              end;

              Actor.FeatureChanged;
            end;
          SM_CHARSTATUSCHANGED:begin
              // Actor.m_Feature := Feature^;
              Actor.m_nState := State;

              if Actor = g_MySelf then begin
                if (g_MySelf.m_nState and $00080000 <> 0) or (g_MySelf.m_nState and $04000000 <> 0) then begin
                  g_MySelf.CancelAction;
                  g_MySelf.m_boWarMode := False;
                  g_MySelf.CalcActorFrame;
                  g_MySelf.ActionChanged;
                end;
              end;
            end;
          else begin
              if ident = SM_TURN then begin
                if Str <> '' then
                  Actor.m_sUserName := Str;
              end;

              if Feature <> nil then
                Actor.SendMsg(ident, X, Y, cdir, Feature^, State, '', 0)
              else
                Actor.SendMsg(ident, X, Y, cdir, State, '', 0);
            end;
        end;
      end;
  end;
  Result := Actor;
end;

procedure TPlayScene.CheckSceneShake;
begin
  if m_boDelaySceneShake then begin
    if tick_diff(m_dwDelaySceneShakeTick, MyGetTickCount) >= m_dwDelaySceneShakeTime then begin
      m_boDelaySceneShake := False;
      SceneShake(m_dwDelaySceneShakeCount, 0);
    end;
  end;
end;

procedure TPlayScene.SceneShake(Count:Integer; DelayTime:Integer);
var
  I:Integer;
begin
  if DelayTime = 0 then begin
    // 是否开启屏幕震动效果 piaoyun 2013-09-14
    for I := 0 to Count - 1 do begin
      AddSceneShakeOffset(0, -10);
      AddSceneShakeOffset(0, 0);
      AddSceneShakeOffset(0, -8);
      AddSceneShakeOffset(0, 0);
      AddSceneShakeOffset(0, -6);
      AddSceneShakeOffset(0, 0);
      AddSceneShakeOffset(0, -4);
      AddSceneShakeOffset(0, 0);
    end;
  end
  else begin
    m_boDelaySceneShake := True;
    m_dwDelaySceneShakeTick := MyGetTickCount;
    m_dwDelaySceneShakeTime := DelayTime;
    m_dwDelaySceneShakeCount := Count;
  end;
end;

procedure TPlayScene.AddSceneShakeOffset(nX, nY:Smallint);
var
  N:Integer;
begin
  N := MakeLong(nX, nY);
  m_SceneShakeList.Add(Pointer(N));
end;

procedure TPlayScene.DoAddActor(Actor:TActor);
var
  Index:Integer;
begin
  if not DoSearchActor(Actor.m_nRecogId, Index) then begin
    m_ActorList.Insert(Index, Actor);
    m_DrawActorList.Add(Actor);
  end;
end;

function TPlayScene.DoDelActor(Actor:TACtor):Boolean;
var
  Index:Integer;
begin
  Result := False;
  if DoSearchActor(Actor.m_nRecogId, Index) then begin
    m_ActorList.Delete(Index);
    Result := True;
  end;

  m_DrawActorList.Remove(Actor);
end;

function TPlayScene.DoSearchActor(ID:Int64;
  var Index:Integer):Boolean;
var
  L, H, I:Integer;
  C:Int64;
begin
  Result := False;
  L := 0;
  H := m_ActorList.Count - 1;
  while L <= H do begin
    I := (L + H) shr 1;
    C := TActor(m_ActorList.Items[I]).m_nRecogId - ID;
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := True;
        L := I; // 不重复 if not Duplicates then
      end;
    end;
  end;
  Index := L;
end;

function TPlayScene.DoSearchSortYDrawActtor(nRY:Integer; var Index:Integer):Boolean;
var
  L, H, I, C:Integer;
  A:TActor;
begin
  Result := False;
  L := 0;
  H := m_SortYDrawActorList.Count - 1;
  while L <= H do begin
    I := (L + H) shr 1;
    A := TActor(m_SortYDrawActorList.Items[I]);
    C := (A.m_nRy - A.m_nDownDrawLevel) - nRY;
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := True;
      end;
    end;
  end;
  Index := L;
end;

end.
