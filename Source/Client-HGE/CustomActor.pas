unit CustomActor;

interface

uses
  Windows,
  SysUtils,
  Classes,
  Actor,
  Grobal2,
  clFunc,
  SDK,
  HGE,
  MMSystem,
  SoundUtil;

type
  TCustomActor = class(TActor)
  private
    m_nEffectPx, m_nEffectPy:Integer;
    m_BodyEffectSurface:TTexture;

    m_nEffect2Px, m_nEffect2Py:Integer;
    m_BodyEffect2Surface:TTexture;

    m_nCurSelfEffFrame:Integer;
    m_dwCurSelfEffFrameTick:Integer;

    FConfig:TClientCustomMonsterConfig;

    FClientAction:PMonsterClientAction;
  public
    m_nOldChrLight:Integer;
  public
    constructor Create(AMonsterConfig:TClientCustomMonsterConfig); reintroduce;

    procedure CalcActorFrame; override;
    procedure LoadSurface(Sender:TObject); override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Run; override;

    procedure RunSound; override;
    procedure RunActSound(frame:Integer); override;
    property Config:TClientCustomMonsterConfig read FConfig write FConfig;
    (*procedure Finalize; override;
    *)
  end;

implementation

uses
  MShare,
  GameImages,
  HGECanvas,
  GameConfigDlg,
  magiceff;

{ TCustomActor }

constructor TCustomActor.Create(AMonsterConfig:TClientCustomMonsterConfig);
begin
  inherited Create;
  FConfig := AMonsterConfig;
  m_BodySurface := nil;
  m_BodyEffectSurface := nil;
  m_BodyEffect2Surface := nil;
  m_nChrLight := 1;
  m_boCreateEffect := False;
end;

procedure TCustomActor.CalcActorFrame;
var
  ClientAction:PMonsterClientAction;
  ClientConfig:PClientAttackConfig;
  TempDir:Integer;
  I, nX, nY, nDirCount:Integer;
  meff:TMagicEff;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
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

  m_nBodyOffset := 0;

  m_nChrLight := m_nOldChrLight;

  ClientAction := nil;
  case m_nCurrentAction of
    0, SM_TURN:begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin
          ClientAction := @FConfig.Actions[matStoneRevive];

          if ClientAction.CalcDir then begin
            TempDir := m_btDir
          end else begin
            TempDir := 0;
          end;

          m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
          m_nEndFrame := m_nStartFrame;
          m_dwFrameTime := ClientAction.PlayTime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := ClientAction.PlayCount;
        end else begin
          ClientAction := @FConfig.Actions[matStand];

          if ClientAction.CalcDir then begin
            TempDir := m_btDir
          end else begin
            TempDir := 0;
          end;

          m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
          m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
          m_dwFrameTime := ClientAction.PlayTime;
          m_dwStartTime := TimeGetTime;
          m_nDefFrameCount := ClientAction.PlayCount;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
        ClientAction := @FConfig.Actions[matWalk];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := 0;
        m_nCurTick := 0;
        m_nMoveStep := 1;
        if m_nCurrentAction = SM_BACKSTEP then begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        end
        else
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    SM_DIGUP:begin
        ClientAction := @FConfig.Actions[matStoneRevive];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;
        m_nMaxTick := 0;
        m_nCurTick := 0;
        m_nDefFrameCount := ClientAction.PlayCount;

        m_nState := 0;

        Shift(m_btDir, 0, 0, 1);
      end;
    SM_LIGHTINGEX:begin
        {
        m_nStartFrame := pm.ActAttack.start;                                                        // + Dir * (pm.ActAttack.frame + pm.ActAttack.skip);
        m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
        m_dwFrameTime := pm.ActAttack.ftime;
        m_dwStartTime := TimeGetTime;
            // WarMode := TRUE;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        if m_nMagicNum > 0 then
        begin
          SetMagicSound(m_nMagicNum);
          PlayScene.NewMagic(Self,
            111,
            m_nEffectNum,                                                                           // Effect
            m_nCurrX,
            m_nCurrY,
            m_nTargetX,
            m_nTargetY,
            m_nTargetRecog,
            m_MagicType,                                                                            // EffectType
            True,
            0,
            bofly);
          if bofly then
            g_PlaySound.PlaySound(m_nMagicFireSound)
          else
            g_PlaySound.PlaySound(m_nMagicExplosionSound);
        end;
        }
      end;
    SM_HIT:begin
        ClientAction := @FConfig.Actions[matDefAttack];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;
        m_dwWarModeTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        {
        // 下面的代码从 TBanyaGuardMon.CalcActorFrame 搞来的 chongchong 2016-07-14
        m_boUseEffect := True;
        m_nEffectFrame := m_nStartFrame;
        m_nEffectStart := m_nStartFrame;
        m_nEffectEnd := m_nEndFrame;
        m_dwEffectStartTime := TimeGetTime;
        m_dwEffectFrameTime := m_dwFrameTime;
        }
      end;
    SM_STRUCK:begin
        ClientAction := @FConfig.Actions[matStruck];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := m_dwStruckFrameTime;
        m_dwStartTime := TimeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_CustomMagicStatusEffect.m_nStruck := 0;

        {
        if m_nCurrentAction = SM_STRUCK then
        begin
          DScreen.AddChatBoardString('受到攻击', $0000FF, 0);
        end;
        }
      end;
    SM_DEATH:begin
        ClientAction := @FConfig.Actions[matDie];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + ClientAction.PlayCount - 1;
        m_nEndFrame := m_nStartFrame;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_NOWDEATH:begin
        ClientAction := @FConfig.Actions[matDie];

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;
        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;
      end;
    SM_SKELETON:begin

      end;
    SM_ATTACK01, SM_ATTACK02, SM_ATTACK03, SM_ATTACK04, SM_ATTACK05, SM_ATTACK06:begin
        ClientConfig := nil; //HZQ 20230525, 消除警告
        case m_nCurrentAction of
          SM_ATTACK01:begin
              ClientAction := @FConfig.Actions[matAttack1];
              ClientConfig := @FConfig.AttackConfigs[0];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
          SM_ATTACK02:begin
              ClientAction := @FConfig.Actions[matAttack2];
              ClientConfig := @FConfig.AttackConfigs[1];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
          SM_ATTACK03:begin
              ClientAction := @FConfig.Actions[matAttack3];
              ClientConfig := @FConfig.AttackConfigs[2];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
          SM_ATTACK04:begin
              ClientAction := @FConfig.Actions[matAttack4];
              ClientConfig := @FConfig.AttackConfigs[3];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
          SM_ATTACK05:begin
              ClientAction := @FConfig.Actions[matAttack5];
              ClientConfig := @FConfig.AttackConfigs[4];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
          SM_ATTACK06:begin
              ClientAction := @FConfig.Actions[matAttack6];
              ClientConfig := @FConfig.AttackConfigs[5];

              if ClientConfig.Self_LightRange > 0 then begin
                m_nChrLight := ClientConfig.Self_LightRange;
              end;
            end;
        end;

        if ClientAction.CalcDir then
          TempDir := m_btDir
        else
          TempDir := 0;

        m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
        m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
        m_dwFrameTime := ClientAction.PlayTime;
        m_dwStartTime := TimeGetTime;

        m_boUseMagic := True;
        m_nSpellFrame := ClientAction.PlayCount;

        //HZQ 20230525 增加 ClientConfig <> nil
        if (ClientConfig <> nil) and (ClientConfig.Self_StartIndex >= 0) and (ClientConfig.Self_PlayCount > 0) then begin
          // 去掉延时 chongchong 2016-04-27
          //m_nSpellFrame := ClientConfig.Self_PlayCount;

          if (ClientConfig.Self_DirCalcType = mdctCenter) then begin
            PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);

            if ClientConfig.Self_DirCount = mdcDir8 then begin
              nDirCount := 8
            end else begin
              nDirCount := 16;
            end;

            for I := 0 to nDirCount - 1 do begin
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := ClientConfig.Self_StartIndex + I * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount);
              meff.TargetActor := nil;
              meff.NextFrameTime := ClientConfig.Self_PlayTime;
              meff.ExplosionFrame := ClientConfig.Self_PlayCount;

              if I = 1 then
                meff.Light := ClientConfig.Self_LightRange;

              if (ClientConfig.Self_File >= 0) and (ClientConfig.Self_File < g_EffectImageList.Count) then begin
                meff.ImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File])
              end else begin
                meff.ImgLib := g_WMonImages.Images[m_wAppearance];
              end;

              PlayScene.AddEffectList(meff);
            end;
          end;

          // 去掉延时 chongchong 2016-04-27
          {
          else if ClientConfig.Self_PlayDelayAction then
          begin
            m_boUseEffect := True;
            m_boUseMagic := True;
            m_nCurEffFrame := 0;
            m_dwWaitMagicRequest := TimeGetTime;
            m_boWarMode := True;
            m_dwWarModeTime := TimeGetTime;
          end;
          }
        end;

        m_nCurEffFrame := 0;
        m_nCurSelfEffFrame := 0;
        Shift(m_btDir, 0, 0, 1);
      end;
  end;

  FClientAction := ClientAction;
end;

procedure TCustomActor.LoadSurface(Sender:TObject);
var
  giBody, giBodyEffect, giBodyEffect2:TGameImages;
  nBodyOffset, nBodyEffectOffset, nBodyEffect2Offset:Integer;
  ClientAction:PMonsterClientAction;
  TempDir, TempOffset:Integer;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
    inherited;
    Exit;
  end;

  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_BodySurface := nil;
  m_BodyEffectSurface := nil;
  m_BodyEffect2Surface := nil;

  if PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]
    and m_boDeath and (not ((m_wAppearance >= 900) and (m_wAppearance <= 906))) then begin
    Finalize;
  end else begin
    {
    case m_nCurrentAction of
      0, SM_TURN:
        begin
          if m_boDeath then
            ClientAction := @FConfig.Actions[matDie]
          else if (m_nState and STATE_STONE_MODE) <> 0 then
            ClientAction := @FConfig.Actions[matStoneRevive]
          else
            ClientAction := @FConfig.Actions[matStand];
        end;
      SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:
        begin
          ClientAction := @FConfig.Actions[matWalk];
        end;
      SM_DIGUP:
        begin
          ClientAction := @FConfig.Actions[matStoneRevive];
        end;
      SM_LIGHTINGEX:
        begin
        end;
      SM_HIT:
        begin
          ClientAction := @FConfig.Actions[matDefAttack];
        end;
      SM_STRUCK:
        begin
          ClientAction := @FConfig.Actions[matStruck];
         end;
      SM_DEATH, SM_NOWDEATH:
        begin
          ClientAction := @FConfig.Actions[matDie];
        end;
      SM_ATTACK01:
        begin
          ClientAction := @FConfig.Actions[matAttack1];
        end;
      SM_ATTACK02:
        begin
          ClientAction := @FConfig.Actions[matAttack2];
        end;
      SM_ATTACK03:
        begin
          ClientAction := @FConfig.Actions[matAttack3];
        end;
      SM_ATTACK04:
        begin
          ClientAction := @FConfig.Actions[matAttack4];
        end;
      SM_ATTACK05:
        begin
          ClientAction := @FConfig.Actions[matAttack5];
        end;
      SM_ATTACK06:
        begin
          ClientAction := @FConfig.Actions[matAttack6];
        end;
    end;
    }

    // 修正mon42-12的怪物，走的时候特效计算不对。走-8570, 走特效-10090 chongchong 2014-11-17
    ClientAction := FClientAction;

    giBody := nil;
    giBodyEffect := nil;
    giBodyEffect2 := nil;
    nBodyOffset := 0; //nBodyOffset 已经在 giBody赋值时确定了，此处用于消除警告
    nBodyEffectOffset := 0; //nBodyEffectOffset 同上
    nBodyEffect2Offset := 0; //nBodyEffect2Offset 同上

    if (ClientAction <> nil) and (ClientAction.StartIndex >= 0) and (ClientAction.PlayCount > 0) then begin
      if (ClientAction.ActionFile >= 0) and (ClientAction.ActionFile < g_EffectImageList.Count) then
        giBody := TGameImages(g_EffectImageList.Objects[ClientAction.ActionFile])
      else
        giBody := g_WMonImages.Images[m_wAppearance];

      nBodyOffset := m_nCurrentFrame;

      if (ClientAction.EffectIndex >= 0) then begin
        if (ClientAction.EffectFile >= 0) and (ClientAction.EffectFile < g_EffectImageList.Count) then
          giBodyEffect := TGameImages(g_EffectImageList.Objects[ClientAction.EffectFile])
        else
          giBodyEffect := g_WMonImages.Images[m_wAppearance];
        //nBodyEffectOffset := ClientAction.EffectIndex + m_nCurrentFrame - ClientAction.StartIndex;

        // 修正计算动作特效不对 chongchong 2014-09-11
        if ClientAction.CalcDir then begin
          TempDir := m_btDir;
          TempOffset := (m_nCurrentFrame - ClientAction.StartIndex) mod (ClientAction.PlayCount + ClientAction.EmptyCount);

          if (ClientAction.ActionType = matDie) and (FConfig.BaseConfig.DieNoCalcDir) then
            nBodyEffectOffset := ClientAction.EffectIndex + TempOffset
          else
            nBodyEffectOffset := ClientAction.EffectIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + TempOffset;
        end else begin
          TempOffset := m_nCurrentFrame - ClientAction.StartIndex;
          nBodyEffectOffset := ClientAction.EffectIndex + TempOffset;
        end;
      end;

      if (ClientAction.EffectIndex2 >= 0) then begin
        if (ClientAction.EffectFile2 >= 0) and (ClientAction.EffectFile2 < g_EffectImageList.Count) then
          giBodyEffect2 := TGameImages(g_EffectImageList.Objects[ClientAction.EffectFile2])
        else
          giBodyEffect2 := g_WMonImages.Images[m_wAppearance];
        //nBodyEffect2Offset := ClientAction.EffectIndex2 + m_nCurrentFrame - ClientAction.StartIndex;

        // 修正计算动作特效不对 chongchong 2014-09-11
        if ClientAction.CalcDir then begin
          TempDir := m_btDir;
          TempOffset := (m_nCurrentFrame - ClientAction.StartIndex) mod (ClientAction.PlayCount + ClientAction.EmptyCount);
          if (ClientAction.ActionType = matDie) and (FConfig.BaseConfig.DieNoCalcDir) then
            nBodyEffect2Offset := ClientAction.EffectIndex2 + TempOffset
          else
            nBodyEffect2Offset := ClientAction.EffectIndex2 + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + TempOffset;
        end else begin
          TempOffset := m_nCurrentFrame - ClientAction.StartIndex;
          nBodyEffect2Offset := ClientAction.EffectIndex2 + TempOffset;
        end;
      end;
    end;

    if giBody <> nil then begin
      if (not m_boReverseFrame) then begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := giBody.GetCachedGrayImage(nBodyOffset, m_nPx, m_nPy);
          ceBright:m_BodySurface := giBody.GetCachedBrightImage(nBodyOffset, m_nPx, m_nPy);
          else
            m_BodySurface := giBody.GetCachedImage(nBodyOffset, m_nPx, m_nPy);
        end;
      end;
    end;

    if giBodyEffect <> nil then begin
      if (not m_boReverseFrame) then begin
        case m_ColorEffect of
          ceGrayScale:m_BodyEffectSurface := giBodyEffect.GetCachedGrayImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
          ceBright:m_BodyEffectSurface := giBodyEffect.GetCachedBrightImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
          else
            m_BodyEffectSurface := giBodyEffect.GetCachedImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
        end;
      end;
    end;

    if giBodyEffect2 <> nil then begin
      if (not m_boReverseFrame) then begin
        case m_ColorEffect of
          ceGrayScale:m_BodyEffect2Surface := giBodyEffect2.GetCachedGrayImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
          ceBright:m_BodyEffect2Surface := giBodyEffect2.GetCachedBrightImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
          else
            m_BodyEffect2Surface := giBodyEffect2.GetCachedImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
        end;
      end;
    end;
  end;

  ActionChanged;
end;

function TCustomActor.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  TempDir:Integer;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
    Result := inherited GetDefaultFrame(wmode);
    Exit;
  end;

  //Result := 0; //HZQ 下面没条路径均有赋值
  m_nChrLight := m_nOldChrLight;

  if m_boDeath then begin
    if m_boSkeleton then begin
      Result := FConfig.Actions[matDie].StartIndex
    end else begin
      if FConfig.Actions[matDie].CalcDir then
        TempDir := m_btDir
      else
        TempDir := 0;
      Result := FConfig.Actions[matDie].StartIndex + TempDir * (FConfig.Actions[matDie].PlayCount + FConfig.Actions[matDie].EmptyCount) + (FConfig.Actions[matDie].PlayCount - 1);
    end;

    FClientAction := @FConfig.Actions[matDie];
  end else begin
    if (m_nState and STATE_STONE_MODE) <> 0 then begin
      if FConfig.Actions[matStoneRevive].CalcDir then
        TempDir := m_btDir
      else
        TempDir := 0;

      Result := FConfig.Actions[matStoneRevive].StartIndex + TempDir * (FConfig.Actions[matStoneRevive].PlayCount + FConfig.Actions[matStoneRevive].EmptyCount);

      FClientAction := @FConfig.Actions[matStoneRevive];
    end else begin
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= FConfig.Actions[matStand].PlayCount then
        cf := 0
      else
        cf := m_nCurrentDefFrame;

      if FConfig.Actions[matStand].CalcDir then
        TempDir := m_btDir
      else
        TempDir := 0;

      Result := FConfig.Actions[matStand].StartIndex + TempDir * (FConfig.Actions[matStand].PlayCount + FConfig.Actions[matStand].EmptyCount) + cf;

      FClientAction := @FConfig.Actions[matStand];
    end;
  end;
end;

procedure TCustomActor.DrawChr(dx, dy:Integer; blend, boFlag:Boolean);
var
  ClientConfig:PClientAttackConfig;
  giSelfEffect:TGameImages;
  SelfEffectSurface:TTexture;
  px, py:Integer;
  nDir:Integer;

  procedure DrawSelfMagicEffect(BeforeDraw:Boolean);
  begin
    // 攻击后自身魔法效果chongchong 2014-07-22
    if m_boUseMagic and (m_CurMagic.EffectNumber <> 0) then begin
      ClientConfig := nil;

      case m_CurMagic.EffectNumber of
        -1 * SM_ATTACK01:ClientConfig := @FConfig.AttackConfigs[0];
        -1 * SM_ATTACK02:ClientConfig := @FConfig.AttackConfigs[1];
        -1 * SM_ATTACK03:ClientConfig := @FConfig.AttackConfigs[2];
        -1 * SM_ATTACK04:ClientConfig := @FConfig.AttackConfigs[3];
        -1 * SM_ATTACK05:ClientConfig := @FConfig.AttackConfigs[4];
        -1 * SM_ATTACK06:ClientConfig := @FConfig.AttackConfigs[5];
      end;

      // 自身动画支持自定义播放时间 chongchong 2014-09-04
      if (m_nCurEffFrame = 0) then begin
        m_nCurSelfEffFrame := 0;
        m_dwCurSelfEffFrameTick := TimeGetTime;
      end else if MyGetTickCount - Cardinal(m_dwCurSelfEffFrameTick) >= ClientConfig.Self_PlayTime then begin
        if m_nCurSelfEffFrame <= ClientConfig.Self_PlayCount then begin
          Inc(m_nCurSelfEffFrame);
        end;
        m_dwCurSelfEffFrameTick := TimeGetTime;
      end;

      // 非8方向攻击特效 chongchong 2014-09-12
      if (ClientConfig <> nil) and (((ClientConfig.Self_DrawOrder = mdoPriorMagic) and BeforeDraw)
        or ((ClientConfig.Self_DrawOrder = mdoPriorSelf) and (not BeforeDraw)))
        and (ClientConfig.Self_DirCalcType <> mdctCenter) then begin
        giSelfEffect := nil;
        if (ClientConfig.Self_StartIndex >= 0) and
          (ClientConfig.Self_PlayCount > 0) and
          ({m_nCurEffFrame} m_nCurSelfEffFrame in [0..ClientConfig.Self_PlayCount - 1]) then begin
          if (ClientConfig.Self_File >= 0) and (ClientConfig.Self_File < g_EffectImageList.Count) then
            giSelfEffect := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File])
          else
            giSelfEffect := g_WMonImages.Images[m_wAppearance];

          if giSelfEffect <> nil then begin

            if ClientConfig.Self_DirCalcType = mdctNone then begin
              SelfEffectSurface := giSelfEffect.GetCachedImage(ClientConfig.Self_StartIndex + m_nCurSelfEffFrame {m_nCurEffFrame}, px, py);
            end else if ClientConfig.Self_DirCalcType = mdctNormal then begin
              if ((m_CurMagic.targx = -1) and (m_CurMagic.targy = -1)) then
                nDir := m_btDir
              else if ClientConfig.Self_DirCount = mdcDir8 then
                nDir := GetNextDirection(m_nCurrX, m_nCurrY, m_CurMagic.targx, m_CurMagic.targy)
              else
                nDir := GetFlyDirection16(m_nCurrX, m_nCurrY, m_CurMagic.targx, m_CurMagic.targy);

              SelfEffectSurface := giSelfEffect.GetCachedImage(ClientConfig.Self_StartIndex +
                nDir * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount) +
                m_nCurSelfEffFrame {m_nCurEffFrame}, px, py);
            end;

            //SelfEffectSurface := giSelfEffect.GetCachedImage(ClientConfig.Self_StartIndex + m_nCurSelfEffFrame {m_nCurEffFrame}, px, py);

            if SelfEffectSurface <> nil then begin
              if ClientConfig.Self_DrawMode = mdmBlend then begin
                GameCanvas.DrawBlend(dx + px + m_nShiftX, dy + py + m_nShiftY, SelfEffectSurface);
              end else begin
                GameCanvas.Draw(dx + px + m_nShiftX, dy + py + m_nShiftY, SelfEffectSurface);
              end;
            end;
          end;
        end;
      end;
    end;
  end;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
    inherited DrawChr(dx, dy, blend, boFlag);
    Exit;
  end;

  DrawSelfMagicEffect(True);
  if FConfig.BaseConfig.DrawOrder = mdoSelf_Eff1_Eff2 then begin
    inherited DrawChr(dx, dy, blend, boFlag);

    if m_BodyEffectSurface <> nil then begin
      if FConfig.BaseConfig.DrawMode = mdmBlend then begin
        GameCanvas.DrawBlend(
          dx + m_nEffectPx + m_nShiftX,
          dy + m_nEffectPy + m_nShiftY,
          m_BodyEffectSurface);
      end else begin
        GameCanvas.Draw(
          dx + m_nEffectPx + m_nShiftX,
          dy + m_nEffectPy + m_nShiftY,
          m_BodyEffectSurface);
      end;
    end;

    if m_BodyEffect2Surface <> nil then begin
      if FConfig.BaseConfig.DrawMode2 = mdmBlend then begin
        GameCanvas.DrawBlend(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end else begin
        GameCanvas.Draw(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end;
    end;
  end else if FConfig.BaseConfig.DrawOrder = mdoEff1_Self_Eff2 then begin
    if m_BodyEffectSurface <> nil then begin
      if FConfig.BaseConfig.DrawMode = mdmBlend then begin
        GameCanvas.DrawBlend(dx + m_nEffectPx + m_nShiftX, dy + m_nEffectPy + m_nShiftY, m_BodyEffectSurface);
      end else begin
        GameCanvas.Draw(dx + m_nEffectPx + m_nShiftX, dy + m_nEffectPy + m_nShiftY, m_BodyEffectSurface);
      end;
    end;

    inherited DrawChr(dx, dy, blend, boFlag);

    if m_BodyEffect2Surface <> nil then begin
      if FConfig.BaseConfig.DrawMode2 = mdmBlend then begin
        GameCanvas.DrawBlend(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end else begin
        GameCanvas.Draw(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end;
    end;
  end else begin
    if m_BodyEffectSurface <> nil then begin
      if FConfig.BaseConfig.DrawMode = mdmBlend then begin
        GameCanvas.DrawBlend(dx + m_nEffectPx + m_nShiftX, dy + m_nEffectPy + m_nShiftY, m_BodyEffectSurface);
      end else begin
        GameCanvas.Draw(dx + m_nEffectPx + m_nShiftX, dy + m_nEffectPy + m_nShiftY, m_BodyEffectSurface);
      end;
    end;

    if m_BodyEffect2Surface <> nil then begin
      if FConfig.BaseConfig.DrawMode2 = mdmBlend then begin
        GameCanvas.DrawBlend(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end else begin
        GameCanvas.Draw(dx + m_nEffect2Px + m_nShiftX, dy + m_nEffect2Py + m_nShiftY, m_BodyEffect2Surface);
      end;
    end;
    inherited DrawChr(dx, dy, blend, boFlag);
  end;
  DrawSelfMagicEffect(False);
end;

procedure TCustomActor.Run;
var
  meff:TMagicEff;
  Actor:TActor;
  ClientConfig:PClientAttackConfig;
  IntCurrentX, IntCurrentY, IntTargetX, IntTargetY:Integer;
  I, FlyDir:Integer;
  TempTargetRecog:Int64;
  FlyImgLib, FlyEffImgLib, ExplosionImgLib:TGameImages;
  TargetEffImgLib:TGameImages;
  Targets:TStringList;

  boFind:Boolean;

  m_dwEffectFrameTimetime:longword;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
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

  inherited;

  if m_boUseMagic then begin
    //OutputDebugString(PChar(IntToStr(m_nCurEffFrame)));

    if (m_nCurEffFrame = m_nSpellFrame - 1) then {// (m_nCurEffFrame in [0..m_nSpellFrame - 1])   (m_nCurrentFrame = m_nEndFrame - 1)} begin
      if not m_boCreateEffect then begin
        m_boCreateEffect := True;
        ClientConfig := nil;

        case m_CurMagic.EffectNumber of
          -1 * SM_ATTACK01:ClientConfig := @FConfig.AttackConfigs[0];
          -1 * SM_ATTACK02:ClientConfig := @FConfig.AttackConfigs[1];
          -1 * SM_ATTACK03:ClientConfig := @FConfig.AttackConfigs[2];
          -1 * SM_ATTACK04:ClientConfig := @FConfig.AttackConfigs[3];
          -1 * SM_ATTACK05:ClientConfig := @FConfig.AttackConfigs[4];
          -1 * SM_ATTACK06:ClientConfig := @FConfig.AttackConfigs[5];
        end;

        // 没有飞行，只有目标播放效果
        if (ClientConfig = nil) then Exit;

        if (ClientConfig.Fly_StartIndex < 0) or (ClientConfig.Fly_PlayCount <= 0) then begin
          if ((ClientConfig.Target_StartIndex >= 0) or (ClientConfig.Target_StartIndex2 >= 0)) and (ClientConfig.Target_PlayCount > 0) and
            // 不是持续播放 chongchogn 2015-03-10
          ((not ClientConfig.Target_KeepPlay) or (ClientConfig.Target_KeepTime = 0)) then begin
            if (ClientConfig.Target_File >= 0) and (ClientConfig.Target_File < g_EffectImageList.Count) then
              TargetEffImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Target_File])
            else
              TargetEffImgLib := g_WMonImages.Images[m_wAppearance];

            Actor := PlayScene.FindActor(m_nTargetRecog);
            if Actor <> nil then begin
              boFind := False;

              if ClientConfig.Target_LockDraw then begin
                for I := 0 to PlayScene.m_EffectList.Count - 1 do begin
                  if TMagicEff(PlayScene.m_EffectList[I]) is TCustomMonTargetEffect then begin
                    meff := PlayScene.m_EffectList[I];
                    if (TCustomMonTargetEffect(meff).EffectBase = ClientConfig.Target_StartIndex) and
                      (TCustomMonTargetEffect(meff).MagExplosionBase2 = ClientConfig.Target_StartIndex2) and
                      (TCustomMonTargetEffect(meff).ExplosionFrame = ClientConfig.Target_PlayCount) and
                      (TCustomMonTargetEffect(meff).DrawMode = ClientConfig.Target_DrawMode) and
                      (TCustomMonTargetEffect(meff).DrawMode2 = ClientConfig.Target_DrawMode2) and
                      (TCustomMonTargetEffect(meff).ImgLib = TargetEffImgLib) and
                      (TCustomMonTargetEffect(meff).Light = ClientConfig.Target_LightRange) and
                      (TCustomMonTargetEffect(meff).NextFrameTime = ClientConfig.Target_PlayTime) and
                      (TCustomMonTargetEffect(meff).MagOwner = Self) and
                      (TCustomMonTargetEffect(meff).TargetActor = Actor) then
                      boFind := True;
                  end;
                end;
              end
              else begin
                PlayScene.ScreenXYfromMCXY(Actor.m_nCurrX, Actor.m_nCurrY, IntTargetX, IntTargetY);

                for I := 0 to PlayScene.m_EffectList.Count - 1 do begin
                  if TMagicEff(PlayScene.m_EffectList[I]) is TCustomMonTargetEffect then begin
                    meff := PlayScene.m_EffectList[I];
                    if (TCustomMonTargetEffect(meff).EffectBase = ClientConfig.Target_StartIndex) and
                      (TCustomMonTargetEffect(meff).MagExplosionBase2 = ClientConfig.Target_StartIndex2) and
                      (TCustomMonTargetEffect(meff).ExplosionFrame = ClientConfig.Target_PlayCount) and
                      (TCustomMonTargetEffect(meff).DrawMode = ClientConfig.Target_DrawMode) and
                      (TCustomMonTargetEffect(meff).DrawMode2 = ClientConfig.Target_DrawMode2) and
                      (TCustomMonTargetEffect(meff).ImgLib = TargetEffImgLib) and
                      (TCustomMonTargetEffect(meff).Light = ClientConfig.Target_LightRange) and
                      (TCustomMonTargetEffect(meff).NextFrameTime = ClientConfig.Target_PlayTime) and
                      (meff.MagOwner = Self) and
                      (meff.targetx = IntTargetX) and
                      (meff.targety = IntTargetY) then
                      boFind := True;
                  end;
                end;
              end;

              if not boFind then begin
                if ClientConfig.Target_LockDraw then
                  meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor)
                else
                  meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);
                TCustomMonTargetEffect(meff).DrawMode := ClientConfig.Target_DrawMode;
                TCustomMonTargetEffect(meff).DrawMode2 := ClientConfig.Target_DrawMode2;
                meff.ImgLib := TargetEffImgLib;
                meff.Light := ClientConfig.Target_LightRange;
                meff.NextFrameTime := ClientConfig.Target_PlayTime;
                meff.MagOwner := Self;
                PlayScene.AddEffectList(meff);
              end;
            end;

            if Length(m_Saying) > 0 then begin
              Targets := TStringList.Create;
              try
                Targets.Delimiter := ',';
                Targets.DelimitedText := m_Saying;
                for I := 0 to Targets.Count - 1 do begin
                  TempTargetRecog := StrToInt64Def(Trim(Targets[I]), 0);
                  if TempTargetRecog <> 0 then begin
                    Actor := PlayScene.FindActor(TempTargetRecog);
                    if Actor <> nil then begin
                      if ClientConfig.Target_LockDraw then
                        meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor)
                      else
                        meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);
                      TCustomMonTargetEffect(meff).DrawMode := ClientConfig.Target_DrawMode;
                      TCustomMonTargetEffect(meff).DrawMode2 := ClientConfig.Target_DrawMode2;
                      meff.ImgLib := TargetEffImgLib;
                      meff.Light := ClientConfig.Target_LightRange;
                      meff.NextFrameTime := ClientConfig.Target_PlayTime;
                      PlayScene.AddEffectList(meff);
                    end;
                  end;
                end;
              finally
                Targets.Free;
              end;
            end;
          end;
        end
        else if (ClientConfig.Fly_StartIndex >= 0) or (ClientConfig.Fly_PlayCount > 0) then begin
          Actor := PlayScene.FindActor(m_nTargetRecog);
          if Actor = nil then Exit;

          PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, IntCurrentX, IntCurrentY);
          PlayScene.ScreenXYfromMCXY(m_CurMagic.targx, m_CurMagic.targy, IntTargetX, IntTargetY);

          FlyDir := 0;
          if ClientConfig.Fly_CalcDir then begin
            if ClientConfig.Fly_DirCount = mdcDir8 then
              FlyDir := GetFlyDirection(IntCurrentX, IntCurrentY, IntTargetX, IntTargetY)
            else
              FlyDir := GetFlyDirection16(IntCurrentX, IntCurrentY, IntTargetX, IntTargetY);
          end;

          ExplosionImgLib := nil;
          if ((ClientConfig.Explosion_StartIndex >= 0) or (ClientConfig.Explosion_StartIndex2 >= 0)) and (ClientConfig.Explosion_PlayCount > 0) then begin
            if (ClientConfig.Explosion_File >= 0) and (ClientConfig.Explosion_File < g_EffectImageList.Count) then
              ExplosionImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Explosion_File])
            else
              ExplosionImgLib := g_WMonImages.Images[m_wAppearance];
          end;

          FlyImgLib := nil;
          if ClientConfig.Fly_StartIndex >= 0 then begin
            if (ClientConfig.Fly_File >= 0) and (ClientConfig.Fly_File < g_EffectImageList.Count) then
              FlyImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Fly_File])
            else
              FlyImgLib := g_WMonImages.Images[m_wAppearance];
          end;

          FlyEffImgLib := nil;
          if ClientConfig.FlyEff_StartIndex >= 0 then begin
            if (ClientConfig.FlyEff_File >= 0) and (ClientConfig.Fly_File < g_EffectImageList.Count) then
              FlyEffImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.FlyEff_File])
            else
              FlyEffImgLib := g_WMonImages.Images[m_wAppearance];
          end;

          meff := TCustomMonFlyEffect.Create(ClientConfig.Fly_StartIndex + FlyDir * (ClientConfig.Fly_PlayCount + ClientConfig.Fly_EmptyCount),
            IntCurrentX, IntCurrentY, IntTargetX, IntTargetY, Actor, ClientConfig.Fly_PlayCount, ExplosionImgLib, ClientConfig.Explosion_LockDraw, False);

          TCustomMonFlyEffect(meff).FlyDrawMode := ClientConfig.Fly_DrawMode;
          TCustomMonFlyEffect(meff).FlyLightRange := ClientConfig.Fly_LightRange;
          TCustomMonFlyEffect(meff).ExplosionLightRange := ClientConfig.Explosion_LightRange;

          meff.ImgLib := FlyImgLib;
          meff.Light := 1;

          if ClientConfig.Explosion_KeepPlay and (ClientConfig.Explosion_KeepTime > 0) then begin
            meff.MagExplosionBase := -1;
            TCustomMonFlyEffect(meff).MagExplosionBase2 := -1;
            meff.ExplosionFrame := 0;
          end
          else begin
            meff.MagExplosionBase := ClientConfig.Explosion_StartIndex;
            TCustomMonFlyEffect(meff).MagExplosionBase2 := ClientConfig.Explosion_StartIndex2;
            meff.ExplosionFrame := ClientConfig.Explosion_PlayCount;
          end;

          TCustomMonFlyEffect(meff).MagicBlend := ClientConfig.Explosion_DrawMode = mdmBlend;
          TCustomMonFlyEffect(meff).MagicBlend2 := ClientConfig.Explosion_DrawMode2 = mdmBlend;

          meff.NextFrameTime := ClientConfig.Fly_PlayTime;

          TCustomMonFlyEffect(meff).NextExplosionFrameTime := ClientConfig.Explosion_PlayTime;
          TCustomMonFlyEffect(meff).FlyEffImgLib := FlyEffImgLib;
          TCustomMonFlyEffect(meff).FlyEffStartIndex := ClientConfig.FlyEff_StartIndex + FlyDir * (ClientConfig.Fly_PlayCount + ClientConfig.Fly_EmptyCount);
          TCustomMonFlyEffect(meff).FlyEffDrawMode := ClientConfig.FlyEff_DrawMode;

          if (meff <> nil) then begin
            meff.TargetRx := m_CurMagic.targx;
            meff.TargetRy := m_CurMagic.targy;
            if meff.TargetActor <> nil then begin
              meff.TargetRx := TActor(meff.TargetActor).m_nCurrX;
              meff.TargetRy := TActor(meff.TargetActor).m_nCurrY;
            end;
            meff.MagOwner := Self;

            {$IF IsMultiThreadRender = 1}
            PlayScene.m_EffectList.Lock;
            {$IFEND}
            PlayScene.m_EffectList.Add(meff);
            {$IF IsMultiThreadRender = 1}
            PlayScene.m_EffectList.UnLock;
            {$IFEND}

          end;
        end;
      end;
    end
      // 由于在基类的Run里面有 if (m_nCurEffFrame = m_nSpellFrame - 2) or (MagicTimeOut) then
    else begin
      m_boCreateEffect := False;
    end;
  end;
end;

procedure TCustomActor.RunActSound(frame:Integer);
//var
//  FileName: string;
begin
  if not m_boRunSound then Exit;

  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
    inherited RunActSound(frame);
    Exit;
  end;

  case m_nCurrentAction of
    SM_TURN:begin
        if (frame = 1) and (Random(8) = 1) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstNormal]);
          m_boRunSound := False;
        end;
      end;
    SM_HIT:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK01:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack1]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK02:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack2]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK03:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack3]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK04:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack4]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK05:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack5]);
          m_boRunSound := False;
        end;
      end;
    SM_ATTACK06:begin
        if (frame = 3) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstAttack6]);
          m_boRunSound := False;
        end;
      end;
    SM_NOWDEATH:begin
        if (frame = 2) then begin
          PlaySound(FConfig.BaseConfig.Sounds[mstDie]);
          m_boRunSound := False;
        end;
      end;
  end;
end;

procedure TCustomActor.RunSound;
//var
//  FileName: string;
begin
  if (m_nChangeAppr >= 0) and (m_btRace <> 156) then begin
    inherited RunSound;
    Exit;
  end;

  m_boRunSound := True;
  SetSound;
  case m_nCurrentAction of
    SM_STRUCK:begin
        PlaySound(FConfig.BaseConfig.Sounds[mstStruck]);
        if (m_nStruckWeaponSound >= 0) then g_PlaySound.PlaySound(m_nStruckWeaponSound);
      end;
    SM_DIGUP:begin
        PlaySound(FConfig.BaseConfig.Sounds[mstDigUP]);
      end;
  end;
end;

end.
