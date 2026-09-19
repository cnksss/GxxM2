unit DxMagicBall;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Forms,
  HGE,
  DxCanvas,
  DxControls,
  Dialogs,
  DxComponents,
  GameImages,
  Graphics,
  HGECanvas,
  Math;

type
  TDxMagicBall = class;

  // 红蓝一体设置
  TMagicBallOverallSetting = class(TPersistent)
  private
    FOwner:TDxMagicBall;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FEmptyHPMP:Integer;
    FFullHPMP:Integer;

    FEmptyHP:Integer;
    FFullHP:Integer;

    FSplite:Integer;
    FMiddleZoneWidth:Integer;
    FOnlyViewHP:Boolean;

    //魔法球特效
    FEffectCurrFrame:Integer;
    FEffectLastTick:LongWord;

    FEffectDrawBlend:Boolean;
    FEffectImageType:TImageType; // 使用WIL类型
    FEffectImage:TGameImages;
    FEffectHPMPStart:Integer;

    FEffectHPStart:Integer;

    FEffectImageCount:Integer;
    FEffectPlayInterval:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetEffectImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxMagicBall);
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property EffectImage:TGameImages read FEffectImage;

    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property EmptyHPMP:Integer read FEmptyHPMP write FEmptyHPMP;
    property FullHPMP:Integer read FFullHPMP write FFullHPMP;

    property EmptyHP:Integer read FEmptyHP write FEmptyHP;
    property FullHP:Integer read FFullHP write FFullHP;

    property Splite:Integer read FSplite write FSplite;
    property MiddleZoneWidth:Integer read FMiddleZoneWidth write FMiddleZoneWidth;
    property OnlyViewHP:Boolean read FOnlyViewHP write FOnlyViewHP;

    property EffectDrawBlend:Boolean read FEffectDrawBlend write FEffectDrawBlend;
    property EffectImageType:TImageType read FEffectImageType write SetEffectImageType;
    property EffectHPMPStart:Integer read FEffectHPMPStart write FEffectHPMPStart;
    property EffectHPStart:Integer read FEffectHPStart write FEffectHPStart;

    property EffectImageCount:Integer read FEffectImageCount write FEffectImageCount;
    property EffectPlayInterval:Integer read FEffectPlayInterval write FEffectPlayInterval;
  end;

  // 独立设置
  TMagicBallAloneSetting = class(TPersistent)
  private
    FOwner:TDxMagicBall;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FEmpty:Integer;
    FFull:Integer;

    FEffectCurrFrame:Integer;
    FEffectLastTick:LongWord;

    FEffectDrawBlend:Boolean;
    FEffectImageType:TImageType; // 使用WIL类型
    FEffectImage:TGameImages;
    FEffectStart:Integer;
    FEffectImageCount:Integer;
    FEffectPlayInterval:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetEffectImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxMagicBall);
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property EffectImage:TGameImages read FEffectImage;

    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Empty:Integer read FEmpty write FEmpty;
    property Full:Integer read FFull write FFull;

    property EffectDrawBlend:Boolean read FEffectDrawBlend write FEffectDrawBlend;
    property EffectImageType:TImageType read FEffectImageType write SetEffectImageType;
    property EffectStart:Integer read FEffectStart write FEffectStart;
    property EffectImageCount:Integer read FEffectImageCount write FEffectImageCount;
    property EffectPlayInterval:Integer read FEffectPlayInterval write FEffectPlayInterval;

  end;

  TDxMagicBall = class(TDxControl)
  private
    FBallType:TMagicBallType;
    FValueAlignment:TMagicBallValueAlignment;

    FOverallSetting:TMagicBallOverallSetting;
    FAloneSetting:TMagicBallAloneSetting;

    FOnGetHumAbility:TGetHumAbilityEvent;
    FOnAfterDrawMagicBallArea:TAfterDrawMagicBallAreaEvent;
    FOnAfterDrawMagicBallEffectArea:TAfterDrawMagicBallAreaEvent;

    FOnStopPaint:TNotifyEvent;

  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
    procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;

    property OnGetHumAbility:TGetHumAbilityEvent read FOnGetHumAbility write FOnGetHumAbility;
    property OnAfterDrawMagicBallArea:TAfterDrawMagicBallAreaEvent read FOnAfterDrawMagicBallArea write FOnAfterDrawMagicBallArea;
    property OnAfterDrawMagicBallEffectArea:TAfterDrawMagicBallAreaEvent read FOnAfterDrawMagicBallEffectArea write FOnAfterDrawMagicBallEffectArea;
    property OnStopPaint:TNotifyEvent read FOnStopPaint write FOnStopPaint;
  published
    property BallType:TMagicBallType read FBallType write FBallType;
    property ValueAlignment:TMagicBallValueAlignment read FValueAlignment write FValueAlignment;

    property OverallSetting:TMagicBallOverallSetting read FOverallSetting write FOverallSetting;
    property AloneSetting:TMagicBallAloneSetting read FAloneSetting write FAloneSetting;
  end;

implementation

constructor TMagicBallOverallSetting.Create(AOwner:TDxMagicBall);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;
  FEmptyHPMP := -1;
  FFullHPMP := -1;
  FEmptyHP := -1;
  FFullHP := -1;
  FSplite := -1;
  FImage := nil;
  FMiddleZoneWidth := 0;
  FOnlyViewHP := False;

  FEffectCurrFrame := 0;
  FEffectLastTick := MyGetTickCount;
  FEffectDrawBlend := False;
  FEffectImageType := Prguse_wil;
  FEffectHPMPStart := -1;
  FEffectHPStart := -1;
  FEffectImageCount := 0;
  FEffectPlayInterval := 200;

  FOnChange := nil;
  FOnGetImage := nil;
end;

procedure TMagicBallOverallSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TMagicBallOverallSetting.SetEffectImageType(Value:TImageType);
begin
  if FEffectImageType <> Value then begin
    FEffectImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FEffectImageType, TObject(FEffectImage));
    Changed;
  end;
end;

procedure TMagicBallOverallSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then begin
    FOnGetImage(Self, FImageType, TObject(FImage));
    FOnGetImage(Self, FEffectImageType, TObject(FEffectImage));
  end;
  Changed;
end;

procedure TMagicBallOverallSetting.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TMagicBallOverallSetting.Assign(Source:TPersistent);
begin
  if Source is TMagicBallOverallSetting then begin
    OnGetImage := TMagicBallOverallSetting(Source).OnGetImage;
    FImageType := TMagicBallOverallSetting(Source).FImageType;

    FEmptyHPMP := TMagicBallOverallSetting(Source).FEmptyHPMP;
    FFullHPMP := TMagicBallOverallSetting(Source).FFullHPMP;
    FEmptyHP := TMagicBallOverallSetting(Source).FEmptyHP;
    FFullHP := TMagicBallOverallSetting(Source).FFullHP;
    FSplite := TMagicBallOverallSetting(Source).FSplite;
    FMiddleZoneWidth := TMagicBallOverallSetting(Source).FMiddleZoneWidth;
    FOnlyViewHP := TMagicBallOverallSetting(Source).FOnlyViewHP;

    Changed;
  end;
end;

constructor TMagicBallAloneSetting.Create(AOwner:TDxMagicBall);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;

  FOwner := AOwner;
  FEmpty := -1;
  FFull := -1;

  FEffectCurrFrame := 0;
  FEffectLastTick := MyGetTickCount;
  FEffectDrawBlend := False;
  FEffectImageType := Prguse_wil;
  FEffectStart := -1;
  FEffectImageCount := 0;
  FEffectPlayInterval := 200;

  FImage := nil;
  FOnChange := nil;
  FOnGetImage := nil;
end;

procedure TMagicBallAloneSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TMagicBallAloneSetting.SetEffectImageType(Value:TImageType);
begin
  if FEffectImageType <> Value then begin
    FEffectImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FEffectImageType, TObject(FEffectImage));
    Changed;
  end;
end;

procedure TMagicBallAloneSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then begin
    FOnGetImage(Self, FImageType, TObject(FImage));
    FOnGetImage(Self, FEffectImageType, TObject(FEffectImage));
  end;
  Changed;
end;

procedure TMagicBallAloneSetting.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TMagicBallAloneSetting.Assign(Source:TPersistent);
begin
  if Source is TMagicBallAloneSetting then begin
    OnGetImage := TMagicBallAloneSetting(Source).OnGetImage;
    FImageType := TMagicBallAloneSetting(Source).FImageType;

    FEmpty := TMagicBallAloneSetting(Source).FEmpty;
    FFull := TMagicBallAloneSetting(Source).FFull;

    Changed;
  end;
end;

constructor TDXMagicBall.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  FBallType := mbtHPMP;
  FValueAlignment := mbaBottom;

  FOverallSetting := TMagicBallOverallSetting.Create(Self);
  FAloneSetting := TMagicBallAloneSetting.Create(Self);

  FOverallSetting.OnGetImage := OnGetImage;
  FAloneSetting.OnGetImage := OnGetImage;

  Width := 90;
  Height := 90;
end;

destructor TDXMagicBall.Destroy;
begin
  FOverallSetting.Free;
  FAloneSetting.Free;
  inherited Destroy;
end;

procedure TDXMagicBall.Paint;
var
  btJob:Byte;
  dwLevel, dwHP, dwMaxHP, dwMP, dwMaxMP:LongWord;

  d:TTexture;
  PaintRect, vtRect, R:TRect;
  nWidth, nHeight:Integer;

  GameImage:TGameImages;
  EffectGameImage:TGameImages;
begin
  btJob := 0;

  if FOverallSetting.OnlyViewHP then
    dwLevel := 20
  else
    dwLevel := 30;

  dwHP := 1500;
  dwMaxHP := 2000;

  dwMP := 1700;
  dwMaxMP := 2000;

  if Assigned(FOnGetHumAbility) then
    FOnGetHumAbility(Self, btJob, dwLevel, dwHP, dwMaxHP, dwMP, dwMaxMP);

  if (dwMaxHP <= 0) or (dwMaxMP <= 0) then Exit;

  vtRect := VirtualRect;

  if FBallType = mbtHPMP then begin
    GameImage := FOverallSetting.Image;
    if GameImage <> nil then begin
      if (btJob = 0) and (dwLevel < 28) then begin
        // 战士<28时只绘血球
        if (FOverallSetting.FEmptyHP >= 0) then begin
          d := GameImage.Images[FOverallSetting.FEmptyHP];
          if d <> nil then
            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2, vtRect.Top + (Height - d.Height) div 2, d);
        end;

        if (FOverallSetting.FFullHP >= 0) then begin
          d := GameImage.Images[FOverallSetting.FFullHP];
          if d <> nil then begin
            PaintRect := d.ClientRect;

            case FValueAlignment of
              mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxHP * dwHP), 0);
              mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
              mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxHP * dwHP), 0);
              else //mbaBottom:
                PaintRect.Top := MAX(Round(d.Height / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            end;

            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

            if Assigned(FOnAfterDrawMagicBallArea) then begin
              R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
              R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
              R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
              R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

              FOnAfterDrawMagicBallArea(Self, True, R);
            end;
          end
        end;
      end else begin
        if (FOverallSetting.FEmptyHPMP >= 0) then begin
          d := GameImage.Images[FOverallSetting.FEmptyHPMP];
          if d <> nil then
            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2, vtRect.Top + (Height - d.Height) div 2, d);
        end;

        if (FOverallSetting.FFullHPMP >= 0) then begin
          d := GameImage.Images[FOverallSetting.FFullHPMP];
          if d <> nil then begin
            PaintRect := d.ClientRect;
            if FValueAlignment in [mbaLeft, mbaRight] then begin
              nWidth := d.Width;
              nHeight := d.Height div 2;
              PaintRect.Bottom := nHeight - FOverallSetting.FMiddleZoneWidth div 2;
            end
            else begin
              nWidth := d.Width div 2;
              nHeight := d.Height;
              PaintRect.Right := nWidth - FOverallSetting.FMiddleZoneWidth div 2;
            end;

            case FValueAlignment of
              mbaLeft:PaintRect.Right := MAX(Round(nWidth / dwMaxHP * dwHP), 0);
              mbaRight:PaintRect.Left := MAX(Round(nWidth / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
              mbaTop:PaintRect.Bottom := MAX(Round(nHeight / dwMaxHP * dwHP), 0);
              else //mbaBottom:
                PaintRect.Top := MAX(Round(nHeight / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            end;

            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

            if Assigned(FOnAfterDrawMagicBallArea) then begin
              R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
              R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
              R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
              R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

              FOnAfterDrawMagicBallArea(Self, True, R);
            end;

            PaintRect := d.ClientRect;
            if FValueAlignment in [mbaLeft, mbaRight] then
              PaintRect.Top := nHeight + FOverallSetting.FMiddleZoneWidth div 2
            else
              PaintRect.Left := nWidth + FOverallSetting.FMiddleZoneWidth div 2;

            case FValueAlignment of
              mbaLeft:PaintRect.Right := PaintRect.Left + MAX(Round(nWidth / dwMaxMP * dwMP), 0);
              mbaRight:PaintRect.Left := PaintRect.Left + MAX(Round(nWidth / dwMaxMP * MAX(dwMaxHP - dwMP, 0)), 0);
              mbaTop:PaintRect.Bottom := MAX(Round(nHeight / dwMaxMP * dwMP), 0);
              else //mbaBottom:
                PaintRect.Top := MAX(Round(nHeight / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
            end;

            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

            if Assigned(FOnAfterDrawMagicBallArea) then begin
              R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
              R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
              R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
              R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

              FOnAfterDrawMagicBallArea(Self, False, R);
            end;
          end;
        end;

        // 绘制分隔条，移动了最后面
        {
        if (FOverallSetting.FSplite >= 0) then
        begin
          d := GameImage.Images[FOverallSetting.FSplite];
          if d <> nil then
          begin
            PaintRect.Left := vtRect.Left + (Width - d.Width) div 2;
            PaintRect.Top := vtRect.Top + (Height - d.Height) div 2;

            GameCanvas.Draw(PaintRect.Left, PaintRect.Top, d);
          end;
        end;
        }
      end;
    end;

    GameImage := FOverallSetting.FEffectImage;
    if (GameImage <> nil) and (FOverallSetting.EffectImageCount > 0) and (FOverallSetting.EffectPlayInterval > 0) then begin
      if MyGetTickCount - FOverallSetting.FEffectLastTick >= FOverallSetting.EffectPlayInterval then begin
        FOverallSetting.FEffectLastTick := MyGetTickCount;
        Inc(FOverallSetting.FEffectCurrFrame);
      end;

      if (FOverallSetting.FEffectCurrFrame < 0) or (FOverallSetting.FEffectCurrFrame >= FOverallSetting.EffectImageCount) then begin
        FOverallSetting.FEffectCurrFrame := 0;
      end;

      if (btJob = 0) and (dwLevel < 28) then begin
        if (FOverallSetting.FEffectHPStart >= 0) then begin
          d := GameImage.Images[FOverallSetting.FEffectHPStart + FOverallSetting.FEffectCurrFrame];

          PaintRect := d.ClientRect;

          case FValueAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxHP * dwHP), 0);
            mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxHP * dwHP), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(d.Height / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
          end;

          if not FOverallSetting.FEffectDrawBlend then
            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d)
          else
            GameCanvas.DrawBlend(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

          if Assigned(FOnAfterDrawMagicBallEffectArea) then begin
            R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
            R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
            R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
            R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

            FOnAfterDrawMagicBallEffectArea(Self, True, R);
          end;
        end;
      end
      else begin
        if (FOverallSetting.FEffectHPMPStart >= 0) then begin
          d := GameImage.Images[FOverallSetting.FEffectHPMPStart + FOverallSetting.FEffectCurrFrame];
          if d <> nil then begin
            PaintRect := d.ClientRect;
            if FValueAlignment in [mbaLeft, mbaRight] then begin
              nWidth := d.Width;
              nHeight := d.Height div 2;
              PaintRect.Bottom := nHeight - FOverallSetting.FMiddleZoneWidth div 2;
            end
            else begin
              nWidth := d.Width div 2;
              nHeight := d.Height;
              PaintRect.Right := nWidth - FOverallSetting.FMiddleZoneWidth div 2;
            end;

            case FValueAlignment of
              mbaLeft:PaintRect.Right := MAX(Round(nWidth / dwMaxHP * dwHP), 0);
              mbaRight:PaintRect.Left := MAX(Round(nWidth / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
              mbaTop:PaintRect.Bottom := MAX(Round(nHeight / dwMaxHP * dwHP), 0);
              else //mbaBottom:
                PaintRect.Top := MAX(Round(nHeight / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            end;

            if not FOverallSetting.FEffectDrawBlend then
              GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Width) div 2 + PaintRect.Top, PaintRect, d)
            else
              GameCanvas.DrawBlend(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Width) div 2 + PaintRect.Top, PaintRect, d);

            if Assigned(FOnAfterDrawMagicBallEffectArea) then begin
              R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
              R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
              R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
              R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

              FOnAfterDrawMagicBallEffectArea(Self, True, R);
            end;

            PaintRect := d.ClientRect;
            if FValueAlignment in [mbaLeft, mbaRight] then
              PaintRect.Top := nHeight + FOverallSetting.FMiddleZoneWidth div 2
            else
              PaintRect.Left := nWidth + FOverallSetting.FMiddleZoneWidth div 2;

            case FValueAlignment of
              mbaLeft:PaintRect.Right := PaintRect.Left + MAX(Round(nWidth / dwMaxMP * dwMP), 0);
              mbaRight:PaintRect.Left := PaintRect.Left + MAX(Round(nWidth / dwMaxMP * MAX(dwMaxHP - dwMP, 0)), 0);
              mbaTop:PaintRect.Bottom := MAX(Round(nHeight / dwMaxMP * dwMP), 0);
              else //mbaBottom:
                PaintRect.Top := MAX(Round(nHeight / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
            end;

            if not FOverallSetting.FEffectDrawBlend then
              GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d)
            else
              GameCanvas.DrawBlend(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

            if Assigned(FOnAfterDrawMagicBallEffectArea) then begin
              R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
              R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
              R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
              R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

              FOnAfterDrawMagicBallEffectArea(Self, False, R);
            end;
          end;
        end;
      end;
    end;

    // 绘制中间的分隔图片
    GameImage := FOverallSetting.Image;
    if GameImage <> nil then begin
      if not ((btJob = 0) and (dwLevel < 28)) then begin
        if (FOverallSetting.FSplite >= 0) then begin
          d := GameImage.Images[FOverallSetting.FSplite];
          if d <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - d.Width) div 2;
            PaintRect.Top := vtRect.Top + (Height - d.Height) div 2;

            GameCanvas.Draw(PaintRect.Left, PaintRect.Top, d);
          end;
        end;
      end;
    end;
  end
  else if FBallType = mbtHP then begin
    GameImage := FAloneSetting.Image;
    if GameImage <> nil then begin
      if (FAloneSetting.FEmpty >= 0) then begin
        d := GameImage.Images[FAloneSetting.FEmpty];
        if d <> nil then
          GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2, vtRect.Top + (Height - d.Height) div 2, d);
      end;

      if (FAloneSetting.FFull >= 0) then begin
        d := GameImage.Images[FAloneSetting.FFull];
        if d <> nil then begin
          PaintRect := d.ClientRect;

          case FValueAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxHP * dwHP), 0);
            mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxHP * dwHP), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(d.Height / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
          end;

          GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

          if Assigned(FOnAfterDrawMagicBallArea) then begin
            R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
            R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
            R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
            R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

            FOnAfterDrawMagicBallArea(Self, True, R);
          end;
        end;
      end;
    end;

    GameImage := FAloneSetting.FEffectImage;
    if (GameImage <> nil) and (FAloneSetting.FEffectImageCount > 0) and (FAloneSetting.FEffectPlayInterval > 0) then begin
      if MyGetTickCount - FAloneSetting.FEffectLastTick >= FAloneSetting.FEffectPlayInterval then begin
        FAloneSetting.FEffectLastTick := MyGetTickCount;
        Inc(FAloneSetting.FEffectCurrFrame);
      end;

      if (FAloneSetting.FEffectCurrFrame < 0) or (FAloneSetting.FEffectCurrFrame >= FAloneSetting.EffectImageCount) then begin
        FAloneSetting.FEffectCurrFrame := 0;
      end;

      if (FAloneSetting.FEffectStart >= 0) then begin
        d := GameImage.Images[FAloneSetting.FEffectStart + FAloneSetting.FEffectCurrFrame];
        if d <> nil then begin
          PaintRect := d.ClientRect;

          case FValueAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxHP * dwHP), 0);
            mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxHP * dwHP), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(d.Height / dwMaxHP * MAX(dwMaxHP - dwHP, 0)), 0);
          end;

          if not FAloneSetting.FEffectDrawBlend then
            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d)
          else
            GameCanvas.DrawBlend(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

          if Assigned(FOnAfterDrawMagicBallEffectArea) then begin
            R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
            R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
            R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
            R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

            FOnAfterDrawMagicBallEffectArea(Self, True, R);
          end;
        end;
      end;
    end;
  end
  else if FBallType = mbtMP then begin
    GameImage := FAloneSetting.Image;
    if GameImage <> nil then begin
      if (FAloneSetting.FEmpty >= 0) then begin
        d := GameImage.Images[FAloneSetting.FEmpty];
        if d <> nil then
          GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2, vtRect.Top + (Height - d.Height) div 2, d);
      end;

      if (FAloneSetting.FFull >= 0) then begin
        d := GameImage.Images[FAloneSetting.FFull];
        if d <> nil then begin
          PaintRect := d.ClientRect;

          case FValueAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxMP * dwMP), 0);
            mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxMP * dwMP), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(d.Height / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
          end;

          GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

          if Assigned(FOnAfterDrawMagicBallArea) then begin
            R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
            R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
            R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
            R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

            FOnAfterDrawMagicBallArea(Self, False, R);
          end;
        end;
      end;
    end;

    GameImage := FAloneSetting.FEffectImage;
    if (GameImage <> nil) and (FAloneSetting.FEffectImageCount > 0) and (FAloneSetting.FEffectPlayInterval > 0) then begin
      if MyGetTickCount - FAloneSetting.FEffectLastTick >= FAloneSetting.FEffectPlayInterval then begin
        FAloneSetting.FEffectLastTick := MyGetTickCount;
        Inc(FAloneSetting.FEffectCurrFrame);
      end;

      if (FAloneSetting.FEffectCurrFrame < 0) or (FAloneSetting.FEffectCurrFrame >= FAloneSetting.EffectImageCount) then begin
        FAloneSetting.FEffectCurrFrame := 0;
      end;

      if (FAloneSetting.FEffectStart >= 0) then begin
        d := GameImage.Images[FAloneSetting.FEffectStart + FAloneSetting.FEffectCurrFrame];
        if d <> nil then begin
          PaintRect := d.ClientRect;

          case FValueAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(d.Width / dwMaxMP * dwMP), 0);
            mbaRight:PaintRect.Left := MAX(Round(d.Width / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(d.Height / dwMaxMP * dwMP), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(d.Height / dwMaxMP * MAX(dwMaxMP - dwMP, 0)), 0);
          end;

          if not FAloneSetting.FEffectDrawBlend then
            GameCanvas.Draw(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d)
          else
            GameCanvas.DrawBlend(vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left, vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top, PaintRect, d);

          if Assigned(FOnAfterDrawMagicBallEffectArea) then begin
            R.Left := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left;
            R.Top := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Top;
            R.Right := vtRect.Left + (Width - d.Width) div 2 + PaintRect.Right;
            R.Bottom := vtRect.Top + (Height - d.Height) div 2 + PaintRect.Bottom;

            FOnAfterDrawMagicBallEffectArea(Self, False, R);
          end;
        end;
      end;
    end;
  end;

  if Assigned(FOnStopPaint) then
    FOnStopPaint(Self);
end;

procedure TDXMagicBall.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FOverallSetting.OnGetImage := Value;
  FAloneSetting.OnGetImage := Value;
end;

procedure TDXMagicBall.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  inherited;
end;

end.
