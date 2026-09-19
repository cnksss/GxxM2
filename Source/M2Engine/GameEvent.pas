unit GameEvent;

interface

uses
  Windows, Classes, SysUtils, SyncObjs, SDK, ObjGame, ObjBase, Envir, Grobal2,
  uCustomMonsterUtils, Math, M2Definition;

type
  TGameEvent = class(TGameObject)                                                                   // 0x40
    nVisibleFlag: Byte;                                                                             // 0x04
    m_Envir: TEnvirnoment;
    m_nX: Integer;                                                                                  // 0x0C
    m_nY: Integer;                                                                                  // 0x10
    m_nEventType: Integer;                                                                          // 0x14
    m_nEventParam: Integer;                                                                         // 0x18
    m_dwOpenStartTick: LongWord;                                                                    // 0x1C
    m_dwContinueTime: LongWord;                                                                     // 0x20  显示时间长度
    m_dwCloseTick: LongWord;                                                                        // 0x24
    m_boClosed: Boolean;                                                                            // 0x28
    m_nDamage: Integer;                                                                             // /0x2C
    m_OwnBaseObject: TBaseObject;                                                                   // 0x30
    m_dwRunStart: LongWord;                                                                         // 0x34
    m_dwRunTick: LongWord;                                                                          // 0x38
    m_boVisible: Boolean;                                                                           // 0x3C
    m_boActive: Boolean;                                                                            // 0x3D
    m_boClose: Boolean;
    m_boAllowClose: Boolean;
  public
    constructor Create(tEnvir: TEnvirnoment; nTX, nTY, nType, dwETime: Integer; boVisible: Boolean); reintroduce;
    destructor Destroy; override;
    procedure Run(); virtual;
    procedure Close();
  end;
  TStoneMineEvent = class(TGameEvent)                                                               // 0x4C
    m_nMineCount: Integer;                                                                          // 0x40
    m_nAddStoneCount: Integer;                                                                      // 0x44
    m_dwAddStoneMineTick: LongWord;                                                                 // 0x48
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType: Integer);
    procedure AddStoneMine();
  end;
  TPileStones = class(TGameEvent)                                                                   // 0x40
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nTime: Integer);
    procedure AddEventParam();
  end;

  THolyCurtainEvent = class(TGameEvent)                                                             // 0x40
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nTime: Integer);
  end;

  TImprisonCurtainEvent = class(TGameEvent)                                                             // 0x40
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nTime: Integer);
  end;

  TFireBurnEvent = class(TGameEvent)                                                                // 0x44
    m_dwRunTick: LongWord;
    m_boCobwebAttack: Boolean;
  public
    constructor Create(Creat: TBaseObject; nX, nY: Integer; nType: Integer; nTime, nDamage: Integer; boCobwebAttack: Boolean = False);
    procedure Run(); override;
  end;

  TSafeEvent = class(TGameEvent)                                                                    // 安全区光环
  private
    FDir: Integer;
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nDir: Integer);
    property Dir: Integer read FDir;
  end;

  TSafeEventEx = class(TSafeEvent)                                                                    // 安全区光环
  private
    FShowTime: Integer;
    FCreateTick: LongWord;
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nDir: Integer; ShowTime: Integer);
    procedure Run(); override;
  end;

  TIcePeakEvent = class(TGameEvent)                                                                 // 雪域卫士 冰峰效果
    m_dwRunTick: LongWord;
  public
    constructor Create(Creat: TBaseObject);
    procedure Run(); override;
  end;

  // 烟花类 nTime控制时间 -- piaoyun 2013-06-27
  TFlowerEvent = class(TGameEvent)
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nTime: Integer);
  end;

  // 地图特效类 -- piaoyun 2013-07-10
  TMapEffectExEvent = class(TGameEvent)
  public
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType: Integer);
  end;

  TMapEffectEvent = class(TGameEvent)                                                               // 播放效果
    m_ViewPlayer: TBaseObject;
    m_nID: Integer;
    m_nLoopCount: Integer;
    m_nFileIndex: Integer;
    m_nImageIndex: Integer;
    m_nImageCount: Integer;
    m_dwSpeedTick: LongWord;
    m_dwSpeedTime: LongWord;
    m_btBlend: Byte;
    m_btLight: Byte;
  public
    constructor Create(ViewPlayer: TBaseObject; Envir: TEnvirnoment; nX, nY: Integer; nFileIndex, nImageIndex, nImageCount, nLoopCount, nSpeedTime: Integer; boBlend: Boolean; btLight: Byte; nID: Integer);
    procedure Run(); override;
  end;

  TEventManager = class                                                                             // 0x0C
    m_EventList: TGList;
    m_ClosedEventList: TGList;
    m_StoneMineEventList: TGList;
    m_nProcEventIDx: Integer;
    m_nProcStoneMineEventIDx: Integer;
  public
    constructor Create();
    destructor Destroy; override;
    function GetGameEvent(Envir: TEnvirnoment; nX, nY: Integer; nType: Integer): TGameEvent;
    procedure AddEvent(Event: TGameEvent; NotMine: Boolean = True);
    procedure Run();
  end;

  TCustomEffectEvent = class(TGameEvent)                                                                // 自定义特效
  private
    FAttackIgnoreDefence: Boolean;
    FAdditionalHP0: Integer;
    FOwnerAppr: Word;
    FAttackIndex: Byte;

    FAttackRange: Integer;
    FAttackInterval: Integer;
    FAdditionalDamages: TAdditionalDamageArray;
  public
    constructor Create(Creat: TBaseObject; nX, nY: Integer; nType: Integer; nTime, nDamage: Integer;
      AAttackIgnoreDefence: Boolean;
      AAdditionalHP0: Integer;
      AOwnerAppr: Word; AAttackIndex: Byte;
      AttackRange: Integer; AttackInterval: Integer; AdditionalDamages: TAdditionalDamageArray);

    procedure Run(); override;
    property OwnerAppr: Word read FOwnerAppr;
    property AttackIndex: Byte read FAttackIndex;
  end;

  TCustomMagicEffectEvent = class(TGameEvent)                                                                // 自定义特效
  private
    FAttackIgnoreDefence: Boolean;
    FAdditionalHP0: Integer;
    FMagicID: Word;
    FNewLevel: Word;

    FAttackRange: Integer;
    FAttackInterval: Integer;
    FAdditionalDamages: TAdditionalDamageArray;
  public
    constructor Create(Creat: TBaseObject; nX, nY: Integer; nType: Integer; nTime, nDamage: Integer;
      AAttackIgnoreDefence: Boolean;
      AAdditionalHP0: Integer;
      AMagicID, ANewLevel: Word;
      AttackRange: Integer; AttackInterval: Integer; AdditionalDamages: TAdditionalDamageArray);

    procedure Run(); override;
    property MagicID: Word read FMagicID;
    property NewLevel: Word read FNewLevel;
  end;


  { 附加功能 }
  TAdditionalFeatures = (afNone, afPalsy {麻痹}, afGreenPoison {绿毒},
    afRedPoison {红毒}, afFreeze {冰冻}, afCobweb {蜘蛛网});

  TMapMagicGameEvent = class(TGameEvent)
  private
    FVisibleTick: LongWord;
    FKeepVisible: Boolean;
    FAttackTime: Integer;
  public
    FAdditional: TAdditionalFeatures;
    constructor Create(Envir: TEnvirnoment; nX, nY: Integer; nType: Integer);
    procedure Run(); override;
    property KeepVisible: Boolean read FKeepVisible write FKeepVisible;
    property VisibleTick: LongWord read FVisibleTick write FVisibleTick;
    property AttackTime: Integer read FAttackTime write FAttackTime;
  end;


implementation

uses M2Share;
{ TFlowerEvent 烟花}

constructor TFlowerEvent.Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nTime: Integer);
begin
  inherited Create(Envir, nX, nY, nType, nTime, True);
end;

{ TStoneMineEvent }

constructor TStoneMineEvent.Create(Envir: TEnvirnoment; nX, nY,
  nType: Integer);
begin
  inherited Create(Envir, nX, nY, nType, 0, False);
  // m_Envir.AddToMapMineEvent(nX, nY, Self);
  m_boVisible := False;
  m_nMineCount := Random(200);
  m_dwAddStoneMineTick := MyGetTickCount();
  m_boActive := False;
  m_nAddStoneCount := Random(80);
  m_boAllowClose := False;
end;

{ TEventManager }

procedure TEventManager.Run;
var
  I, nIdx: Integer;
  Event: TGameEvent;
  dwCheckTime: LongWord;
  boCheckTimeLimit: Boolean;
resourcestring
  sExceptionMsg1 = '[Exception] TEventManager.Run 1';
  sExceptionMsg2 = '[Exception] TEventManager.Run 2';
begin
  boCheckTimeLimit := False;
  dwCheckTime := MyGetTickCount();
  nIdx := m_nProcEventIDx;
  try
    // m_EventList.Lock;
    // try
    while True do
    begin
      if m_EventList.Count <= nIdx then Break;
      Event := TGameEvent(m_EventList.Items[nIdx]);
      if Event.m_boActive and (tick_diff(Event.m_dwRunStart, MyGetTickCount) > 250) then
      begin
        Event.m_dwRunStart := MyGetTickCount();
        Event.Run();
      end;
      if Event.m_boClosed then
      begin
        m_ClosedEventList.Add(Event);
        m_EventList.Delete(nIdx);
        Continue;
      end;
      Inc(nIdx);
      if tick_diff(dwCheckTime, MyGetTickCount) > 10 then
      begin
        boCheckTimeLimit := True;
        m_nProcEventIDx := nIdx;
        Break;
      end;
    end;                                                                                            // while True do begin
    {finally
      m_EventList.UnLock;
    end;}
    if not boCheckTimeLimit then m_nProcEventIDx := 0;
  except
    MainOutMessage(sExceptionMsg1);
  end;

  boCheckTimeLimit := False;
  dwCheckTime := MyGetTickCount();
  nIdx := m_nProcStoneMineEventIDx;
  try
    // m_EventList.Lock;
    // try
    while True do
    begin                                                                                           // 挖矿
      if m_StoneMineEventList.Count <= nIdx then Break;
      Event := TGameEvent(m_StoneMineEventList.Items[nIdx]);
      if tick_diff(dwCheckTime, MyGetTickCount) > 10 then
      begin
        boCheckTimeLimit := True;
        m_nProcStoneMineEventIDx := nIdx;
        Break;
      end;
      if tick_diff(Event.m_dwAddTime, MyGetTickCount) >= 60 * 1000 * 60 then
      begin                                                                                         // 一个小时后删除
        if (Event.m_Envir <> nil) then
          Event.m_Envir.DeleteFromMap(Event.m_nX, Event.m_nY, Event);
        Event.Close;
        m_ClosedEventList.Add(Event);
        m_StoneMineEventList.Delete(nIdx);
        // MainOutMessage('TStoneMineEvent:Free:');
        Continue;
      end;
      Inc(nIdx);
      if tick_diff(dwCheckTime, MyGetTickCount) > 10 then
      begin
        boCheckTimeLimit := True;
        m_nProcStoneMineEventIDx := nIdx;
        Break;
      end;
    end;                                                                                            // while True do begin
    {finally
      m_EventList.UnLock;
    end;}
    if not boCheckTimeLimit then m_nProcStoneMineEventIDx := 0;
  except
    MainOutMessage(sExceptionMsg2);
  end;

  {
  m_EventList.Lock;
  try
    for I := m_EventList.Count - 1 downto 0 do begin
      ItemObject := TItemObject(m_EventList.Items[I]);
      if (not ItemObject.m_boGhost) and ((MyGetTickCount - ItemObject.m_dwRunTick) > 250) then begin
        ItemObject.m_dwRunTick := MyGetTickCount();
        ItemObject.Run();
      end;
      if ItemObject.m_boGhost then begin
        m_ClosedEventList.Add(ItemObject);
        m_EventList.Delete(I);
      end;
    end;
  finally
    m_EventList.UnLock;
  end; }

 { m_ClosedEventList.Lock;
  try }
  for I := m_ClosedEventList.Count - 1 downto 0 do
  begin
    Event := TGameEvent(m_ClosedEventList.Items[I]);
    if (MyGetTickCount - Event.m_dwCloseTick) > 5 * 60 * 1000 then
    begin
      m_ClosedEventList.Delete(I);
      Event.Free;
      // break;
    end;
  end;
  {finally
    m_ClosedEventList.UnLock;
  end;  }
end;
{var
  I: Integer;
  Event: TGameEvent;
begin
  m_EventList.Lock;
  try
    for I := m_EventList.Count - 1 downto 0 do begin
      Event := TGameEvent(m_EventList.Items[I]);
      if Event.m_boActive and ((MyGetTickCount - Event.m_dwRunStart) > 250) then begin
        Event.m_dwRunStart := MyGetTickCount();
        Event.Run();
      end;
      if Event.m_boClosed then begin
        m_EventList.Delete(I);
        m_ClosedEventList.Lock;
        try
          m_ClosedEventList.Add(Event);
        finally
          m_ClosedEventList.UnLock;
        end;
      end;
    end;
  finally
    m_EventList.UnLock;
  end;

  m_ClosedEventList.Lock;
  try
    for I := m_ClosedEventList.Count - 1 downto 0 do begin
      Event := TGameEvent(m_ClosedEventList.Items[I]);
      if (MyGetTickCount - Event.m_dwCloseTick) > 5 * 60 * 1000 then begin
        m_ClosedEventList.Delete(I);
        Event.Free;
      end;
    end;
  finally
    m_ClosedEventList.UnLock;
  end;

end;  }

function TEventManager.GetGameEvent(Envir: TEnvirnoment; nX, nY,
  nType: Integer): TGameEvent;
var
  I: Integer;
  Event: TGameEvent;
begin
  Result := nil;
  {m_EventList.Lock;
  try }
  for I := 0 to m_EventList.Count - 1 do
  begin
    Event := TGameEvent(m_EventList.Items[I]);
    if Event <> nil then
    begin
      if (Event.m_Envir = Envir) and
        (Event.m_nX = nX) and
        (Event.m_nY = nY) and
        (Event.m_nEventType = nType) then
      begin
        Result := Event;
        Exit;
      end;
    end;
  end;

  {if nType = ET_STONEMINE then begin
    for I := 0 to m_StoneMineEventList.Count - 1 do begin
      Event := TGameEvent(m_StoneMineEventList.Items[I]);
      if Event <> nil then begin
        if (Event.m_Envir = Envir) and
          (Event.m_nX = nX) and
          (Event.m_nY = nY) and
          (Event.m_nEventType = nType) then begin
          Result := Event;
          break;
        end;
      end;
    end;
  end;}
  {finally
    m_EventList.UnLock;
  end; }
end;

procedure TEventManager.AddEvent(Event: TGameEvent; NotMine: Boolean);
begin
 { m_EventList.Lock;
  try }
  if NotMine then
    m_EventList.Add(Event)
  else
    m_StoneMineEventList.Add(Event);
 { finally
    m_EventList.UnLock;
  end;}
end;

constructor TEventManager.Create();                                                                 // 004A8014
begin
  m_EventList := TGList.Create;
  m_ClosedEventList := TGList.Create;
  m_StoneMineEventList := TGList.Create;
  m_nProcEventIDx := 0;
  m_nProcStoneMineEventIDx := 0;
end;

destructor TEventManager.Destroy;
var
  I: Integer;
begin
  for I := 0 to m_EventList.Count - 1 do
  begin
    TGameEvent(m_EventList.Items[I]).Free;
  end;
  m_EventList.Free;

  for I := 0 to m_StoneMineEventList.Count - 1 do
  begin
    TGameEvent(m_StoneMineEventList.Items[I]).Free;
  end;
  m_StoneMineEventList.Free;

  for I := 0 to m_ClosedEventList.Count - 1 do
  begin
    TGameEvent(m_ClosedEventList.Items[I]).Free;
  end;
  m_ClosedEventList.Free;
  inherited;
end;


{ THolyCurtainEvent }

constructor THolyCurtainEvent.Create(Envir: TEnvirnoment; nX, nY, nType, nTime: Integer);           // 004A7E60
begin
  inherited Create(Envir, nX, nY, nType, nTime, True);
end;

constructor TImprisonCurtainEvent.Create(Envir: TEnvirnoment; nX, nY, nType, nTime: Integer);           // 004A7E60
begin
  inherited Create(Envir, nX, nY, nType, nTime, True);
end;

{TSafeEvent}

{ TSafeEvent 安全区光环}

constructor TSafeEvent.Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nDir: Integer);
begin
  inherited Create(Envir, nX, nY, nType, MyGetTickCount, True);
  FDir := nDir;
  m_boAllowClose := False;
end;

constructor TSafeEventEx.Create(Envir: TEnvirnoment; nX, nY: Integer; nType, nDir: Integer; ShowTime: Integer);
begin
  inherited Create(Envir, nX, nY, nType, nDir);
  m_boAllowClose := True;

  FShowTime := ShowTime;
  FCreateTick := MyGetTickCount;
end;

procedure TSafeEventEx.Run();
begin
  if (not m_boClosed) and m_boAllowClose then
  begin
    if MyGetTickCount - FCreateTick >= FShowTime * 1000 then
    begin
      m_boClosed := True;
      Close();
    end;
  end;
end;

// 雪域卫士 冰峰效果

constructor TIcePeakEvent.Create(Creat: TBaseObject);
begin
  inherited Create(Creat.m_PEnvir, Creat.m_nCurrX, Creat.m_nCurrY, ET_ICEPEAK, 0, True);
  m_nEventParam := Creat.m_btDirection;
  m_OwnBaseObject := Creat;
end;

procedure TIcePeakEvent.Run();
begin
  if (m_OwnBaseObject <> nil) then
  begin
    if m_OwnBaseObject.m_boGhost then
    begin
      m_dwOpenStartTick := MyGetTickCount;
      m_dwContinueTime := 1000 * 60;
      m_OwnBaseObject := nil;
    end;
  end;

  if m_OwnBaseObject = nil then
    inherited;
end;
{ TFireBurnEvent }

constructor TFireBurnEvent.Create(Creat: TBaseObject; nX, nY: Integer; nType: Integer; nTime, nDamage: Integer; boCobwebAttack: Boolean); // 004A7EBC
begin
  inherited Create(Creat.m_PEnvir, nX, nY, nType, nTime, True);
  m_nDamage := nDamage;
  m_boCobwebAttack := boCobwebAttack;
  m_OwnBaseObject := Creat;
end;

procedure TFireBurnEvent.Run;
var
  I, nPower: Integer;
  BaseObjectList: TList;          
  TargeTBaseObject: TBaseObject;
begin
  // 修改站在火墙内为3秒一次伤害 chongchong 2014-10-08
  if ((MyGetTickCount - m_dwRunTick) > 3000) then
  begin
    m_dwRunTick := MyGetTickCount();
    BaseObjectList := TList.Create;
    if m_Envir <> nil then
    begin
      m_Envir.GeTBaseObjects(m_nX, m_nY, True, BaseObjectList);


      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (TargeTBaseObject <> nil) and (m_OwnBaseObject <> nil) and (m_OwnBaseObject.IsProperTarget(TargeTBaseObject)) then
        begin
          if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]) then
            nPower := GetSkillLastPowerNG(m_nDamage,                                                // 内功技能
              GetSkillAttackPowerNG(m_OwnBaseObject, TSmartObject(m_OwnBaseObject).m_UserMagics[22], m_nDamage),
              GetSkillDefensePowerNG(TargeTBaseObject, TSmartObject(m_OwnBaseObject).m_UserMagics[22], m_nDamage))
          else
            nPower := m_nDamage;

          // 修改防火墙无效 -- piaoyun 2013-07-17
          if (not TargeTBaseObject.UnFireCross) then
            TargeTBaseObject.SendMsg(m_OwnBaseObject, RM_MAGSTRUCK_MINE, 0, nPower, 0, 22, '');

          if m_boCobwebAttack then
          begin
            TargeTBaseObject.OpenCobwebWinding(5);
          end;
        end;
      end;
    end;
    BaseObjectList.Free;
  end;

  inherited;

  if g_Config.boDisableChangeMapFireCross then
  begin
    if not m_boClosed then
    begin
      if ((m_OwnBaseObject = nil) or (m_OwnBaseObject.m_PEnvir <> m_Envir)) then
      begin
        //if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boSuperMan) then Exit;                   // 增加 机器人除外
        m_boClosed := True;
        Close();
      end;
    end;
  end;
end;

{ TGameEvent }

constructor TGameEvent.Create(tEnvir: TEnvirnoment; nTX, nTY, nType, dwETime: Integer; boVisible: Boolean);
begin
  inherited Create;
  m_ObjGame := Obj_Event;
  m_dwOpenStartTick := MyGetTickCount();
  m_nEventType := nType;
  m_nEventParam := 0;
  m_dwContinueTime := dwETime;
  m_boVisible := boVisible;
  m_boClosed := False;
  m_Envir := tEnvir;
  m_nX := nTX;
  m_nY := nTY;
  m_boActive := True;
  m_nDamage := 0;
  m_OwnBaseObject := nil;
  m_dwRunStart := MyGetTickCount();
  m_dwRunTick := 500;
  m_boAllowClose := True;
  if (m_Envir <> nil) and (m_boVisible) then
  begin
    m_Envir.AddToMap(m_nX, m_nY, Self);
  end
  else
    m_boVisible := False;
end;

destructor TGameEvent.Destroy;
// var
// I: Integer;
begin
  if (m_Envir <> nil) then
    m_Envir.DeleteFromMap(m_nX, m_nY, Self);
  {
  for I := 0 to EventCheck.Count - 1 do begin
    if EventCheck.Items[I] = Self then begin
      EventCheck.Delete(I);
      break;
    end;
  end;
  }
  // m_boClose := True;
  inherited Destroy;
end;

procedure TGameEvent.Run;
begin
  if m_boAllowClose then
  begin
    if MyGetTickCount - m_dwOpenStartTick > m_dwContinueTime then
    begin
      m_boClosed := True;
      Close();
      // MainOutMessage('TGameEvent Close:'+IntToStr(m_nEventType));
    end;
  end;
  if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boGhost or (m_OwnBaseObject.m_boDeath)) then
    m_OwnBaseObject := nil;
end;

procedure TGameEvent.Close;
begin
  m_dwCloseTick := MyGetTickCount();
  if m_boVisible then
  begin
    m_boVisible := False;
    if m_Envir <> nil then
    begin
      m_Envir.DeleteFromMap(m_nX, m_nY, Self);
    end;
    m_Envir := nil;
  end;
end;

// 地图特效

constructor TMapEffectEvent.Create(ViewPlayer: TBaseObject; Envir: TEnvirnoment; nX, nY: Integer; nFileIndex, nImageIndex, nImageCount, nLoopCount, nSpeedTime: Integer; boBlend: Boolean; btLight: Byte; nID: Integer);
begin
  inherited Create(Envir, nX, nY, ET_MAPEFFECT, 0, True);
  m_ViewPlayer := ViewPlayer;
  m_nID := nID;
  m_btLight := btLight;
  m_boAllowClose := nLoopCount >= 0;
  m_ObjGame := Obj_MapEffect;
  m_nFileIndex := nFileIndex;
  m_nImageIndex := nImageIndex;
  m_nImageCount := nImageCount;
  m_nLoopCount := nLoopCount;
  m_dwSpeedTime := nSpeedTime;
  m_dwSpeedTick := MyGetTickCount() + m_dwSpeedTime * m_nImageCount * m_nLoopCount;
  if boBlend then
    m_btBlend := 1
  else
    m_btBlend := 0;
  m_nEventParam := m_btBlend;
end;

procedure TMapEffectEvent.Run();
begin
  if (not m_boClosed) and m_boAllowClose then
  begin
    if MyGetTickCount > m_dwSpeedTick then
    begin
      m_boClosed := True;
      Close();
    end;
  end;
end;

{ TPileStones }

constructor TPileStones.Create(Envir: TEnvirnoment; nX, nY, nType,
  nTime: Integer);                                                                          
begin
  inherited Create(Envir, nX, nY, nType, nTime, True);
  m_nEventParam := 1;
end;

procedure TPileStones.AddEventParam;                                                        
begin
  if m_nEventParam < 5 then Inc(m_nEventParam);
end;

procedure TStoneMineEvent.AddStoneMine;                                                     
begin
  m_nMineCount := m_nAddStoneCount;
  m_dwAddStoneMineTick := MyGetTickCount();
end;

{ TMapEffectExEvent }

constructor TMapEffectExEvent.Create(Envir: TEnvirnoment; nX, nY, nType: Integer);
begin
  // ??????????????? piaoyun 2013-11-14
  inherited Create(Envir, nX, nY, nType, 10 * 1000, True);
end;

{ TCustomEffectEvent }

constructor TCustomEffectEvent.Create(Creat: TBaseObject; nX, nY, nType,
  nTime, nDamage: Integer;
  AAttackIgnoreDefence: Boolean;
  AAdditionalHP0: Integer;
  AOwnerAppr: Word; AAttackIndex: Byte;
  AttackRange, AttackInterval: Integer;
  AdditionalDamages: TAdditionalDamageArray);
begin
  inherited Create(Creat.m_PEnvir, nX, nY, nType, nTime, True);

  m_nDamage := nDamage;
  m_OwnBaseObject := Creat;

  FAttackIgnoreDefence := AAttackIgnoreDefence;
  FAdditionalHP0 := AAdditionalHP0;

  FOwnerAppr := AOwnerAppr;
  FAttackIndex := AAttackIndex;

  FAttackRange := AttackRange;
  FAttackInterval := AttackInterval;
  if FAttackInterval <= 0 then
    FAttackInterval := 1;

  FAdditionalDamages := AdditionalDamages;
end;

procedure TCustomEffectEvent.Run;
const
  BASE_DELAY: LongWord = 300;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;

  nDamage: Integer;
  btGetBackHP, btGetBackMP: Integer;
  Int64Value: Int64;
begin
  if ((MyGetTickCount - m_dwRunTick) > FAttackInterval * 1000) then
  begin
    m_dwRunTick := MyGetTickCount();

    BaseObjectList := TList.Create;
    if m_Envir <> nil then
    begin
      if FAttackRange = 0 then
      begin
        TargeTBaseObject := m_Envir.GetMovingObject(m_nX, m_nY, True);
        if TargeTBaseObject <> nil then
          BaseObjectList.Add(TargeTBaseObject);
      end
      else
        m_Envir.GetRangeBaseObject(m_nX, m_nY, FAttackRange, True, BaseObjectList);

      // 特效发起者死亡 chongchong 2018-08-28 13:48:15
      if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boDeath) or (m_OwnBaseObject.m_boGhost) then
      begin
        m_OwnBaseObject := nil;
      end;

      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (TargeTBaseObject <> nil) and (m_OwnBaseObject <> nil) and (m_OwnBaseObject.IsProperTarget(TargeTBaseObject)) then
        begin
          if FAttackIgnoreDefence then                               // 忽视目标防御
            nDamage := TargeTBaseObject.GetMagStruckDamage(m_OwnBaseObject, m_nDamage, nil)
          else
            nDamage := m_nDamage;

          nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);

          {
          if nDamage > 0 then
            nDamage := NewAbilPower(1, nDamage);                                    // 元素增加攻击伤害
          }

          //nDamage := TargeTBaseObject.StruckDamage(nDamage, m_OwnBaseObject);

          // 2020-07-09
          if (nDamage > 0) and (m_OwnBaseObject <> nil) then
          begin
            // 吸血
            if (FAdditionalDamages[5].Checked) and
              (Random(FAdditionalDamages[5].Rate) = 0) and
              (FAdditionalDamages[5].Time > 0) then
            begin
              btGetBackHP := Round(nDamage / 100 * FAdditionalDamages[5].Time);
              if btGetBackHP > 0 then
              begin
                Int64Value := Int64(m_OwnBaseObject.m_WAbil.HP) + btGetBackHP;
                if Int64Value <= m_OwnBaseObject.m_WAbil.MaxHP then
                  m_OwnBaseObject.m_WAbil.HP := Int64Value
                else
                  m_OwnBaseObject.m_WAbil.HP := m_OwnBaseObject.m_WAbil.MaxHP;
              end;
            end;

            // 吸蓝
            if (FAdditionalDamages[6].Checked) and
              (Random(FAdditionalDamages[6].Rate) = 0) and
              (FAdditionalDamages[6].Time > 0) then
            begin
              btGetBackMP := Round(nDamage / 100 * FAdditionalDamages[6].Time);
              if TargeTBaseObject.m_WAbil.MP < btGetBackMP then
                btGetBackMP := TargeTBaseObject.m_WAbil.MP;
              if btGetBackMP > 0 then
              begin
                Int64Value := m_OwnBaseObject.m_WAbil.MP + btGetBackMP;
                if Int64Value <= m_OwnBaseObject.m_WAbil.MaxMP then
                  m_OwnBaseObject.m_WAbil.MP := Int64Value
                else
                  m_OwnBaseObject.m_WAbil.MP := m_OwnBaseObject.m_WAbil.MaxMP;

                TargeTBaseObject.m_WAbil.MP := Max(TargeTBaseObject.m_WAbil.MP - btGetBackMP, 0);
                TargeTBaseObject.HealthSpellChanged(50);
              end;
            end;
          end;


          TargeTBaseObject.SendMsg(m_OwnBaseObject, RM_MAGSTRUCK_MINE, 0, nDamage, 0, 22, '');

          // 目标不防毒
          if (not TargeTBaseObject.UnPosion) then
          begin
            // 中绿毒
            if (FAdditionalDamages[0].Checked) and
              (Random(FAdditionalDamages[0].Rate) = 0) and
              (FAdditionalDamages[0].Time > 0) then
            begin
              TargeTBaseObject.MakePosion(POISON_DECHEALTH, FAdditionalDamages[0].Time, FAdditionalHP0);
            end;

            // 中红毒
            if (FAdditionalDamages[1].Checked) and
              (Random(FAdditionalDamages[1].Rate) = 0) and
              (FAdditionalDamages[1].Time > 0) then
            begin
              TargeTBaseObject.MakePosion(POISON_DAMAGEARMOR, FAdditionalDamages[1].Time, 0);
            end;
          end;

          // 麻痹
          if (not TargeTBaseObject.UnParalysis) and
            (FAdditionalDamages[2].Checked) and
            (Random(FAdditionalDamages[2].Rate) = 0) and
            (FAdditionalDamages[2].Time > 0) then
          begin
            TargeTBaseObject.MakePosion(POISON_STONE, FAdditionalDamages[2].Time, 0);
          end;

          // 冰冻
          if (not TargeTBaseObject.UnFrozen) and
            (FAdditionalDamages[3].Checked) and
            (Random(FAdditionalDamages[3].Rate) = 0) and
            (FAdditionalDamages[3].Time > 0) then
          begin
            TargeTBaseObject.MakeFrozen(FAdditionalDamages[3].Time);
          end;

          // 冰封
          if ((not TargeTBaseObject.m_boUnForeverFrozen) or (Random(100) >= TargeTBaseObject.m_nUnForeverFrozenRate)) and
            (FAdditionalDamages[10].Checked) and
            (Random(FAdditionalDamages[10].Rate) = 0) and
            (FAdditionalDamages[10].Time > 0) then
          begin
            TargeTBaseObject.OpenForeverFrozen(FAdditionalDamages[10].Time);
          end;

          // 蛛网
          if (not TargeTBaseObject.UnCobwebWinding) and
            (FAdditionalDamages[7].Checked) and
            (Random(FAdditionalDamages[7].Rate) = 0) and
            (FAdditionalDamages[7].Time > 0) then
          begin
            TargeTBaseObject.OpenCobwebWinding(FAdditionalDamages[7].Time);
          end;

          // 0防御
          if (FAdditionalDamages[8].Checked) and
            (Random(FAdditionalDamages[8].Rate) = 0) and
            (FAdditionalDamages[8].Time > 0) then
          begin
            //TargeTBaseObject.ZeroArmor(FAdditionalDamages[8].Time);
            TargeTBaseObject.OpenZeroAC(FAdditionalDamages[8].Time);
          end;

          // 0魔御
          if (FAdditionalDamages[9].Checked) and
            (Random(FAdditionalDamages[9].Rate) = 0) and
            (FAdditionalDamages[9].Time > 0) then
          begin
            TargeTBaseObject.OpenZeroMAC(FAdditionalDamages[9].Time);
          end;

        end;
      end;
    end;
    BaseObjectList.Free;
  end;

  inherited;

  if g_Config.boDisableChangeMapFireCross then
  begin
    if not m_boClosed then
    begin
      if (m_OwnBaseObject = nil) or (m_OwnBaseObject.m_PEnvir <> m_Envir) then
      begin
        //if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boSuperMan) then Exit;                   // 增加 机器人除外
        m_boClosed := True;
        Close();
      end
      else
      begin
        // 特效发起者死亡 chongchong 2018-08-28 13:48:15
        if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boDeath) or (m_OwnBaseObject.m_boGhost) then
        begin
          m_boClosed := True;
          Close();
        end;
      end;
    end;
  end;
end;

{ TCustomMagicEffectEvent }

constructor TCustomMagicEffectEvent.Create(Creat: TBaseObject; nX, nY,
  nType, nTime, nDamage: Integer; AAttackIgnoreDefence: Boolean;
  AAdditionalHP0: Integer; AMagicID, ANewLevel: Word; AttackRange,
  AttackInterval: Integer; AdditionalDamages: TAdditionalDamageArray);
begin
  inherited Create(Creat.m_PEnvir, nX, nY, nType, nTime, True);

  m_nDamage := nDamage;
  m_OwnBaseObject := Creat;

  FAttackIgnoreDefence := AAttackIgnoreDefence;
  FAdditionalHP0 := AAdditionalHP0;

  FMagicID := AMagicID;
  FNewLevel := ANewLevel;

  FAttackRange := AttackRange;
  FAttackInterval := AttackInterval;
  if FAttackInterval <= 0 then
    FAttackInterval := 1;

  FAdditionalDamages := AdditionalDamages;

  m_dwRunTick := MyGetTickCount;
end;

procedure TCustomMagicEffectEvent.Run;
const
  BASE_DELAY: LongWord = 300;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;

  nDamage: Integer;
begin
  if ((MyGetTickCount - m_dwRunTick) > FAttackInterval * 1000) then
  begin
    m_dwRunTick := MyGetTickCount();

    BaseObjectList := TList.Create;
    if m_Envir <> nil then
    begin
      if FAttackRange = 0 then
      begin
        TargeTBaseObject := m_Envir.GetMovingObject(m_nX, m_nY, True);
        if TargeTBaseObject <> nil then
          BaseObjectList.Add(TargeTBaseObject);
      end
      else
        m_Envir.GetRangeBaseObject(m_nX, m_nY, FAttackRange, True, BaseObjectList);

      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if (TargeTBaseObject <> nil) and (m_OwnBaseObject <> nil) and (m_OwnBaseObject.IsProperTarget(TargeTBaseObject)) then
        begin
          nDamage := m_nDamage;
          (*
          if FAttackIgnoreDefence then                               // 忽视目标防御
            nDamage := TargeTBaseObject.GetMagStruckDamage(nil, m_nDamage)
          else
            nDamage := m_nDamage;

          nDamage := TargeTBaseObject.NewAbilPower(3, nDamage);

          // SetSuckDamage 伤害吸收
          if TargeTBaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
          begin
            if (TSmartObject(TargeTBaseObject).m_nSuckDamagePoint > 0) and (nDamage > 0) and (TSmartObject(TargeTBaseObject).m_nSuckDamageRate > 0) then
            begin // 吸收伤害
              if Random(100) < TSmartObject(TargeTBaseObject).m_nSuckDamageProbability then
              begin
                nSuckDamagePoint := Round(TSmartObject(TargeTBaseObject).m_nSuckDamageRate / 1000 * nDamage);
                if nSuckDamagePoint > TSmartObject(TargeTBaseObject).m_nSuckDamagePoint then
                  nSuckDamagePoint := TSmartObject(TargeTBaseObject).m_nSuckDamagePoint;
                Dec(TSmartObject(TargeTBaseObject).m_nSuckDamagePoint, nSuckDamagePoint);
                nDamage := Max(nDamage - nSuckDamagePoint, 0);
              end;
            end;
          end;

          // 伤害封顶
          nDamage := TargeTBaseObject.GetAttackPowerMax(nDamage);

          {
          if nDamage > 0 then
            nDamage := NewAbilPower(1, nDamage);                                    // 元素增加攻击伤害
          }

          nDamage := TargeTBaseObject.StruckDamage(nDamage, m_OwnBaseObject);
          *)

          // procedure TBaseObject.ClientMagStruck(ProcessMsg: pTProcessMessage; var boResult: Boolean);中会重新计算伤害，在此前不要计算 chongchong 2016-12-16
          TargeTBaseObject.SendMsg(m_OwnBaseObject, RM_MAGSTRUCK_MINE, 0, nDamage, 0, MagicID, '');

          // 目标不防毒
          if (not TargeTBaseObject.UnPosion) then
          begin
            // 中绿毒
            if (FAdditionalDamages[0].Checked) and
              (Random(100) < FAdditionalDamages[0].Rate) and
              (FAdditionalDamages[0].Time > 0) then
            begin
              TargeTBaseObject.MakePosion(POISON_DECHEALTH, FAdditionalDamages[0].Time, FAdditionalHP0);
            end;

            // 中红毒
            if (FAdditionalDamages[1].Checked) and
              (Random(100) < FAdditionalDamages[1].Rate) and
              (FAdditionalDamages[1].Time > 0) then
            begin
              TargeTBaseObject.MakePosion(POISON_DAMAGEARMOR, FAdditionalDamages[1].Time, 0);
            end;
          end;

          // 麻痹
          if (not TargeTBaseObject.UnParalysis) and
            (FAdditionalDamages[2].Checked) and
            (Random(100) < FAdditionalDamages[2].Rate) and
            (FAdditionalDamages[2].Time > 0) then
          begin
            TargeTBaseObject.MakePosion(POISON_STONE, FAdditionalDamages[2].Time, 0);
          end;

          // 冰冻
          if (not TargeTBaseObject.UnFrozen) and
            (FAdditionalDamages[3].Checked) and
            (Random(100) < FAdditionalDamages[3].Rate) and
            (FAdditionalDamages[3].Time > 0) then
          begin
            TargeTBaseObject.MakeFrozen(FAdditionalDamages[3].Time);
          end;

          // 冰封
          if ((not TargeTBaseObject.m_boUnForeverFrozen) or (Random(100) >= TargeTBaseObject.m_nUnForeverFrozenRate)) and
            (FAdditionalDamages[10].Checked) and
            (Random(100) < FAdditionalDamages[10].Rate) and
            (FAdditionalDamages[10].Time > 0) then
          begin
            TargeTBaseObject.OpenForeverFrozen(FAdditionalDamages[10].Time);
          end;

          // 蛛网
          if (not TargeTBaseObject.UnCobwebWinding) and
            (FAdditionalDamages[7].Checked) and
            (Random(100) < FAdditionalDamages[7].Rate) and
            (FAdditionalDamages[7].Time > 0) then
          begin
            TargeTBaseObject.OpenCobwebWinding(FAdditionalDamages[7].Time);
          end;

          // 0防御
          if (FAdditionalDamages[8].Checked) and
            (Random(100) < FAdditionalDamages[8].Rate) and
            (FAdditionalDamages[8].Time > 0) then
          begin
            //TargeTBaseObject.ZeroArmor(FAdditionalDamages[8].Time);
            TargeTBaseObject.OpenZeroAC(FAdditionalDamages[8].Time);
          end;

          // 0魔御
          if (FAdditionalDamages[9].Checked) and
            (Random(100) < FAdditionalDamages[9].Rate) and
            (FAdditionalDamages[9].Time > 0) then
          begin
            TargeTBaseObject.OpenZeroMAC(FAdditionalDamages[9].Time);
          end;

        end;
      end;
    end;
    BaseObjectList.Free;
  end;

  inherited;

  if g_Config.boDisableChangeMapFireCross then
  begin
    if not m_boClosed then
    begin
      if ((m_OwnBaseObject = nil) or (m_OwnBaseObject.m_PEnvir <> m_Envir)) then
      begin
        //if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boSuperMan) then Exit;                   // 增加 机器人除外
        m_boClosed := True;
        Close();
      end;
    end;
  end;
end;

{ TMapMagicGameEvent }

constructor TMapMagicGameEvent.Create(Envir: TEnvirnoment; nX, nY, nType: Integer);
begin
  inherited Create(Envir, nX, nY, nType, MyGetTickCount, True);
  m_boAllowClose := False;
end;

procedure TMapMagicGameEvent.Run;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  if (m_Envir <> nil) and ((MyGetTickCount - m_dwRunTick) >= FAttackTime * 1000) then
  begin
    m_dwRunTick := MyGetTickCount();
    BaseObjectList := TList.Create;

    m_Envir.GeTBaseObjects(m_nX, m_nY, True, BaseObjectList);
    for I := 0 to BaseObjectList.Count - 1 do
    begin
      TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
      if (TargeTBaseObject <> nil) and (TargeTBaseObject.m_btRaceServer = RC_PLAYOBJECT) then
      begin
        FVisibleTick := MyGetTickCount;

        if not FKeepVisible then
          TargeTBaseObject.SendRefMsg(RM_SHOWEVENT, MakeWord(m_nEventType, m_nEventParam), NativeInt(Self), m_nX, m_nY, '');

        TargeTBaseObject.DamageHealth(m_nDamage, nil);
        //TargeTBaseObject.HealthSpellChanged();
        TargeTBaseObject.SendRefMsg(RM_STRUCK, m_nDamage, TargeTBaseObject.m_WAbil.HP, TargeTBaseObject.m_WAbil.MaxHP, NativeInt(nil), '', 200);

        // 站在泉水上，又要加上闪光的效果，再弄一个效果发过去
        if m_nEventType in [ET_SPRINGS1, ET_SPRINGS2, ET_SPRINGS3] then
        begin
          TargeTBaseObject.SendRefMsg(RM_SHOWEVENT, ET_SPRINGS_LIGHT, NativeInt(Self), m_nX, m_nY, '');
        end;

        case FAdditional of
          afPalsy {麻痹}:
            if (TargeTBaseObject.m_wStatusTimeArr[POISON_STONE] = 0) and (Random(10) = 0) then TargeTBaseObject.MakePosion(POISON_STONE, 3, 0);
          afGreenPoison {绿毒}:
            if (TargeTBaseObject.m_wStatusTimeArr[POISON_DECHEALTH] = 0) then TargeTBaseObject.MakePosion(POISON_DECHEALTH, m_nDamage, 0);
          afRedPoison {红毒}:
            if (TargeTBaseObject.m_wStatusTimeArr[POISON_DAMAGEARMOR] = 0) then TargeTBaseObject.MakePosion(POISON_DAMAGEARMOR, m_nDamage, 0);
          afFreeze {冰冻}:
            if (TargeTBaseObject.m_wStatusTimeArr[STATE_FROZEN] = 0) and (Random(10) = 0) then TargeTBaseObject.MakeFrozen(3);
          afCobweb {蜘蛛网}:
            if not TargeTBaseObject.m_boCobwebWindingStatus and (Random(10) = 0) then TargeTBaseObject.OpenCobwebWinding(3);
        end;
      end;
    end;
    BaseObjectList.Free;
  end;

  inherited;
end;

end.
