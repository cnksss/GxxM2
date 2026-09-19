unit FireDragon;

interface
uses
  Windows, Classes, Grobal2, ObjBase, ObjMon2, SysUtils, M2Share, HashList, GameEvent, HUtil32;

type
  TFireDragon = class(TCentipedeKingMonster)                                                        // 火龙教主
    m_dwLightTick: LongWord;                                                                        // 守护兽发光间隔
  private
    function CheckAttackTarget(nViewRange: Integer): Boolean;                                       // 检测指定范围内是否存在攻击目标
    function MagBigExplosion(nPower, nX, nY: Integer; nRage: Integer): Boolean;                     // 大火圈攻击
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;                                                           // 刷新属性
    function AttackTarget(): Boolean; override;
    procedure Run; override;
  end;

  TFireDragonGuard = class(TAnimalObject)                                                           // 火龙守护兽
    m_boLight: Boolean;                                                                             // 是否发光
    m_dwLightTick: LongWord;                                                                        // 发光间隔
    m_dwLightTime: LongWord;                                                                        // 发光时长
    m_boAttick: Boolean;                                                                            // 是否可以攻击，即最后一个熄灭的怪，负责攻击消息
    s_AttickXY: string;                                                                             // 攻击坐标
  private
    function MagBigExplosion(nPower, nX, nY: Integer; nRage: Integer): Boolean;                     // 小火圈攻击
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure RecalcAbilitys(); override;                                                           // 刷新属性
    function AttackTarget(): Boolean;
    procedure Run; override;
  end;

implementation

{ TFireDragon }

constructor TFireDragon.Create;
begin
  inherited;
  m_boAnimal := False;                                                                              // 不是动物,即不能挖
  m_boStickMode := True;                                                                            // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
  m_btAntiPoison := 200;                                                                            // 中毒躲避
  m_boUnParalysis := True;                                                                          // 防麻痹
  m_nViewRange := 13;
  m_dwAttickTick := MyGetTickCount();
  m_boFixedHideMode := False;                                                                       // 不隐身
  m_dwLightTick := MyGetTickCount();                                                                  // 守护兽发光间
end;

destructor TFireDragon.Destroy;
begin
  inherited;
end;

function TFireDragon.CheckAttackTarget(nViewRange: Integer): Boolean;
var
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  HashItem: HashList.PHashItem;
begin
  Result := False;
  HashItem := m_VisibleActors.First;
  while HashItem <> nil do
  begin
    VisibleBaseObject := HashItem.Item;
    HashItem := m_VisibleActors.Next(HashItem);
    if VisibleBaseObject <> nil then
    begin
      BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
      if BaseObject = nil then Continue;
      if BaseObject.m_boDeath then Continue;
      if IsProperTarget(BaseObject) then
      begin
        if (abs(m_nCurrX - BaseObject.m_nCurrX) <= nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= nViewRange) then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
  end;
end;

// 大火圈攻击

function TFireDragon.MagBigExplosion(nPower, nX, nY, nRage: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
begin
  Result := False;
  m_btDirection := GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY); // 调整火龙的方向
  BaseObjectList := TList.Create;
  try
    GetMapBaseObjects(m_PEnvir, nX, nY, nRage, BaseObjectList);
    if BaseObjectList.Count > 0 then
    begin
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if TargeTBaseObject <> nil then
        begin
          if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) then Continue;
          if IsProperTarget(TargeTBaseObject) then
          begin
            SetTargetCreat(TargeTBaseObject);
            TargeTBaseObject.SendMsg(self, RM_MAGSTRUCK, 0, nPower, 0, 0, '');
            Result := True;
          end;
        end;
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

procedure TFireDragon.RecalcAbilitys;
begin
  inherited;
  m_boStickMode := True;                                                                            // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
  m_btAntiPoison := 200;                                                                            // 中毒躲避
  m_boUnParalysis := True;                                                                          // 防麻痹
  m_boFixedHideMode := False;                                                                       // 不隐身
end;

function TFireDragon.AttackTarget: Boolean;
var
  WAbil: pTAbility;
  nPower, I, K, J: Integer;
  BaseObject: TBaseObject;
  VisibleBaseObject: pTVisibleBaseObject;
  HashItem: HashList.PHashItem;
begin
  Result := False;
  try
    if not CheckAttackTarget(m_nViewRange) then Exit;                                               // 守护兽13格开始发亮，没有目标则退出
    if (MyGetTickCount - m_dwLightTick > 10000) then
    begin                                                                                           // 通知守护兽发光
      m_dwLightTick := MyGetTickCount();
      if UserEngine.m_MonObjectList.Count > 0 then
      begin                                                                                         // 循环列表，找出同个地图的守护兽，
        Randomize;
        K := Random(6);                                                                             // 随机一个守护兽攻击
        J := 0;
        for I := 0 to UserEngine.m_MonObjectList.Count - 1 do
        begin
          BaseObject := TBaseObject(UserEngine.m_MonObjectList.Items[I]);
          if BaseObject <> nil then
          begin
            if BaseObject.m_PEnvir = m_PEnvir then
            begin                                                                                   // 同个地图内
              if TFireDragonGuard(BaseObject).m_boLight or TFireDragonGuard(BaseObject).m_boAttick then Break; //上次没有处理完就退出循环
              if J = K then
              begin                                                                                 // 最后熄灭的怪，即攻击怪
                TFireDragonGuard(BaseObject).m_boAttick := True;
                TFireDragonGuard(BaseObject).m_dwLightTime := 3000;                                 // 发光时长比其它怪多
                //发送最后熄灭的特殊消息(发亮) --- 这个消息不知道对不对
                BaseObject.SendRefMsg({RM_FAIRYATTACKRATE} RM_FLYAXE, 1, BaseObject.m_nCurrX, BaseObject.m_nCurrY, NativeInt(BaseObject), '');
              end else
              begin                                                                                 // 同时熄灭的怪
                //发送同时熄灭的消息(发亮)
                BaseObject.SendRefMsg(RM_LIGHTING, 1, BaseObject.m_nCurrX, BaseObject.m_nCurrY, NativeInt(BaseObject), '');
              end;
              TFireDragonGuard(BaseObject).m_boLight := True;
              TFireDragonGuard(BaseObject).m_dwSearchEnemyTick := MyGetTickCount();
              Inc(J);
              if J >= 6 then Break;                                                                 // 6个怪就退出循环
            end;
          end;
        end;
      end;
    end;
    if not CheckAttackTarget(m_nViewRange - 2) then Exit;                                           // 火龙魔兽11格,没有目标则退出
    if Integer(MyGetTickCount - m_dwHitTick) > m_nNextHitTime then
    begin
      m_dwHitTick := MyGetTickCount();
      if Random(3) = 0 then
      begin
        //群雷攻击
        SendAttackMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY);
        WAbil := @m_WAbil;
        nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
        {if m_VisibleActors.Count > 0 then
        begin
          for I := 0 to m_VisibleActors.Count - 1 do
          begin
            BaseObject := TBaseObject(pTVisibleBaseObject(m_VisibleActors.Items[I]).BaseObject);
            if BaseObject = nil then Continue;
            if BaseObject.m_boDeath then Continue;
            if IsProperTarget(BaseObject) then
            begin
              if (abs(m_nCurrX - BaseObject.m_nCurrX) < m_nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) < m_nViewRange) then
              begin
                m_dwTargetFocusTick := MyGetTickCount();
                SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 2, NativeInt(BaseObject), '', 600);
                if Random(4) = 0 then m_TargetCret := BaseObject;
              end;
            end;
          end;                                                                                      // for
        end;}
        HashItem := m_VisibleActors.First;
        while HashItem <> nil do
        begin
          VisibleBaseObject := HashItem.Item;
          HashItem := m_VisibleActors.Next(HashItem);
          if VisibleBaseObject <> nil then
          begin
            BaseObject := TBaseObject(VisibleBaseObject.BaseObject);
            if BaseObject = nil then Continue;
            if BaseObject.m_boDeath then Continue;
            if IsProperTarget(BaseObject) then
            begin
              if (abs(m_nCurrX - BaseObject.m_nCurrX) <= m_nViewRange) and (abs(m_nCurrY - BaseObject.m_nCurrY) <= m_nViewRange) then
              begin
                m_dwTargetFocusTick := MyGetTickCount();
                SendDelayMsg(Self, RM_DELAYMAGIC, nPower, MakeLong(BaseObject.m_nCurrX, BaseObject.m_nCurrY), 2, NativeInt(BaseObject), '', 600);
                if Random(4) = 0 then m_TargetCret := BaseObject;
              end;
            end;
          end;
        end;
      end else
      begin                                                                                         // 大火圈攻击
        if m_TargetCret <> nil then
        begin
          WAbil := @m_WAbil;
          nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
          MagBigExplosion(nPower, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, 3);
          SendRefMsg(RM_LIGHTING, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '');
          SendRefMsg(RM_FLYAXE, 1, m_nCurrX, m_nCurrY, NativeInt(m_TargetCret), '', 500);
          //SendDelayMsg(Self, RM_FLYAXE, nPower, MakeLong(m_TargetCret.m_nCurrX, BaseObject.m_nCurrY), 2, NativeInt(BaseObject), '', 600);
        end;
      end;
    end;
    Result := True;
  except
    MainOutMessage('{异常} TFireDragon:AttackTarget');
  end;
end;

procedure TFireDragon.Run;
var
  I: Integer;
  VisibleBaseObject: pTVisibleBaseObject;
  HashItem: HashList.PHashItem;
begin
  try
    if not m_boGhost and not m_boDeath and (m_wStatusTimeArr[POISON_STONE {5}] = 0) then
    begin
      if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then
      begin
        m_dwSearchEnemyTick := MyGetTickCount();
        SearchTarget();                                                                             // 搜索可攻击目标
      end;
      if Integer(MyGetTickCount - m_dwWalkTick) > m_nWalkSpeed then
      begin
        m_dwWalkTick := MyGetTickCount();
        if (MyGetTickCount - m_dwAttickTick) > 3000 then
        begin
          if AttackTarget() then
          begin
            inherited;
            Exit;
          end;
          if (MyGetTickCount - m_dwAttickTick) > 10000 then
          begin
            (*
            if m_VisibleActors.Count > 0 then begin
              for I := 0 to m_VisibleActors.Count - 1 do begin
                Dispose({pTVisibleBaseObject}(m_VisibleActors.Items[I]));
              end;
            end;
            *)
            HashItem := m_VisibleActors.First;
            while HashItem <> nil do
            begin
              VisibleBaseObject := HashItem.Item;
              HashItem := m_VisibleActors.Next(HashItem);
              if VisibleBaseObject <> nil then
              begin
                Dispose({pTVisibleBaseObject} VisibleBaseObject);
              end;
            end;
            m_VisibleActors.Clear;
            m_dwAttickTick := MyGetTickCount();
          end;
        end;
      end;
    end;
  except
    MainOutMessage('{异常} TFireDragon:Run');
  end;
  inherited;
end;

{ TFireDragonGuard }

constructor TFireDragonGuard.Create;
begin
  inherited;
  m_boStoneMode := True;                                                                            // 人物不能攻击，石像化
  m_boAnimal := False;                                                                              // 不是动物,即不能挖
  m_boStickMode := True;                                                                            // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
  m_btAntiPoison := 200;                                                                            // 中毒躲避
  m_boUnParalysis := True;                                                                          // 防麻痹
  m_boLight := False;                                                                               // 是否发光
  m_boAttick := False;                                                                              // 是否可以攻击，即最后一个熄灭的怪，负责攻击消息
  s_AttickXY := '';                                                                                 // 攻击坐标
  m_dwLightTime := 2500;                                                                            // 发光时长
end;

destructor TFireDragonGuard.Destroy;
begin
  inherited;
end;

function TFireDragonGuard.MagBigExplosion(nPower, nX, nY,
  nRage: Integer): Boolean;
var
  I: Integer;
  BaseObjectList: TList;
  TargeTBaseObject: TBaseObject;
  FireBurnEvent: TFireBurnEvent;
begin
  Result := False;
  BaseObjectList := TList.Create;
  try
    FireBurnEvent := TFireBurnEvent.Create(self, nX, nY, ET_FIREDRAGON, 4000, 0);                   //客户端显示小火圈效果
    g_EventManager.AddEvent(FireBurnEvent);

    GetMapBaseObjects(m_PEnvir, nX, nY, nRage, BaseObjectList);
    if BaseObjectList.Count > 0 then
    begin
      for I := 0 to BaseObjectList.Count - 1 do
      begin
        TargeTBaseObject := TBaseObject(BaseObjectList.Items[I]);
        if TargeTBaseObject <> nil then
        begin
          if TargeTBaseObject.m_boDeath or (TargeTBaseObject.m_boGhost) then Continue;
          if IsProperTarget(TargeTBaseObject) then
          begin
            SetTargetCreat(TargeTBaseObject);
            TargeTBaseObject.SendMsg(self, RM_MAGSTRUCK, 0, nPower, 0, 0, '');
            Result := True;
          end;
        end;
      end;
    end;
  finally
    BaseObjectList.Free;
  end;
end;

procedure TFireDragonGuard.RecalcAbilitys;
begin
  inherited;
  m_boStoneMode := True;                                                                            // 人物不能攻击，石像化
  m_boAnimal := False;                                                                              // 不是动物,即不能挖
  m_boStickMode := True;                                                                            // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
  m_btAntiPoison := 200;                                                                            // 中毒躲避
  m_boUnParalysis := True;                                                                          // 防麻痹
end;

function TFireDragonGuard.AttackTarget: Boolean;

  function IsChar(str: string): integer;                                                            //判断有几个'|'号
  var
    I: integer;
  begin
    Result := 0;
    if length(str) <= 0 then Exit;
    for I := 1 to length(str) do
      if (str[I] = '|') then Inc(Result);
  end;
var
  I, nX, nY, nPower: Integer;
  str, Str1, s30, s2C: string;
  WAbil: pTAbility;
begin
  Result := False;
  try
    if Integer(MyGetTickCount - m_dwHitTick) > m_nNextHitTime then
    begin
      m_dwHitTick := MyGetTickCount();
      if Pos('|', s_AttickXY) > 0 then
      begin                                                                                         // 根据配置文件的攻击坐标，发消息显示场景
        WAbil := @m_WAbil;
        nPower := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));
        str := s_AttickXY;
        for I := 0 to IsChar(s_AttickXY) do
        begin
          str := GetValidStr3(str, str1, ['|']);
          if Str1 <> '' then
          begin
            s30 := GetValidStr3(Str1, s2C, [',', #9]);                                              // X,Y
            nX := Str_ToInt(s2C, 0);
            nY := Str_ToInt(s30, 0);
            MagBigExplosion(nPower, nX, nY, 1);
          end;
        end;
      end;
      Result := True;
    end;
  except
    MainOutMessage('{异常} TFireDragonGuard:AttackTarget');
  end;
end;

procedure TFireDragonGuard.Run;
begin
  try
    if not m_boGhost and not m_boDeath and (m_wStatusTimeArr[POISON_STONE {5}] = 0) then
    begin
      if Integer(MyGetTickCount - m_dwWalkTick) > m_nWalkSpeed then
      begin
        m_dwWalkTick := MyGetTickCount();
         {SendRefMsg(RM_FAIRYATTACKRATE, 1, m_nCurrX, m_nCurrY, NativeInt(self), '');
          MainOutMessage('发亮'); }
        if m_boLight then
        begin                                                                                       // 发亮
          if (MyGetTickCount - m_dwSearchEnemyTick) > m_dwLightTime then
          begin
            m_dwSearchEnemyTick := MyGetTickCount();
            m_dwLightTime := 2500;                                                                  // 发光时长
            m_boLight := False;
          end;
        end;
        if m_boAttick and (not m_boLight) and (s_AttickXY <> '') then
        begin                                                                                       // 可以攻击
          if AttackTarget then m_boAttick := False;                                                 // 处理攻击代码
        end;
      end;
    end;
    inherited;
  except
    MainOutMessage('{异常} TFireDragonGuard:Run');
  end;
end;

end.

