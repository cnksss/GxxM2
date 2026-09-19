unit HGECanvas;
(*
** hgeCanvas helper class
** Extension to the HGE engine
** Extension added by DraculaLin
** This extension is NOT part of the original HGE engine.
*)

interface

uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  Math,
  DirectXGraphics,
  HGEFontEx,
  HGEImages,
  HGE;

type
  THGECanvas = class
  private
    FHGE:THGE;
    FWidth, FHeight:Single;
    FTexWidth, FTexHeight:Integer;
    FActive:Boolean;
    FOnInitialize:TNotifyEvent;
    FOnFinalize:TNotifyEvent;

    FZBuffer:Single;
    FInitialized:Boolean;

    FUseSound:Boolean;
    FWindowed:Boolean;
    FDepthStencil:Boolean;
    FHardware:Boolean;
    FScreenWidth:Integer;
    FScreenHeight:Integer;
    FBitCount:Integer;
    FHandle:Hwnd;
    FLogFilePath:string;
    FVSync:Boolean;

    FMaxFontSize:Integer;
    procedure SetColor(Color:Cardinal); overload;
    procedure SetColor(Color1, Color2, Color3, Color4:Cardinal); overload;

    procedure SetMirror(MirrorX, MirrorY:Boolean);

    //procedure SetUseSound(Value: Boolean);
    //procedure SetWindowed(Value: Boolean);
    //procedure SetDepthStencil(Value: Boolean);
    procedure SetZBuffer(Value:Single);

    //procedure SetBitCount(Value: Integer);
    //procedure SetHandle(Value: Hwnd);
    //procedure SetLogFileName(Value: string);

    //procedure SetHardware(Value: Boolean);
    //procedure SetVSync(Value: Boolean);
    //function GetHardware: Boolean;
    //function GetUseSound: Boolean;
    //function GetWindowed: Boolean;
    //function GetDepthStencil: Boolean;
    //function GetVSync: Boolean;
    //function GetZBuffer: Single;
    //function GetWidth: Integer;
    //function GetHeight: Integer;
    //function GetBitCount: Integer;
    //function GetHandle: Hwnd;
    //function GetLogFileName: string;

    function GetClipRect:TRect;
    procedure SetClipRect(Value:TRect);

    function GetDeviceLost:TNotifyEvent;
    procedure SetDeviceLost(Value:TNotifyEvent);
    function GetDeviceReset:TNotifyEvent;
    procedure SetDeviceReset(Value:TNotifyEvent);

  protected
    procedure SetPattern(Texture:TTexture; PatternIndex:Integer); overload;
    procedure SetPattern(Texture:TTexture; ClientRect:TRect); overload;
    procedure SetPattern(Texture:TTexture); overload;
    procedure SetWidth(Value:Integer);
    procedure SetHeight(Value:Integer);
  protected
    FQuad:THGEQuad;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Resize(const AWidth, AHeight:Integer);
    function Initialize:Boolean;
    procedure Finalize;
    procedure Render(Handler:TNotifyEvent; Background:Cardinal = 0; FillBk:Boolean = True; Target:TTarget = nil);

    procedure Draw(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;
    procedure Draw(X, Y:Integer; SrcRect:TRect; Texture:TTexture; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;
    procedure Draw(X, Y:Integer; Texture:TTexture; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawColor(X, Y:Integer; Texture:TTexture; Color:TColor; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;
    procedure DrawColor(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawColorEx(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture;
      BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure StretchDraw(const DestRect:TRect; Texture:TTexture;
      BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture; Alpha:Byte;
      BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture; Alpha:Byte; Color:TColor;
      BlendMode:Integer; Z:Single); overload;

    procedure DrawAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture;
      Alpha:Byte; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawAlpha(X, Y:Integer; Texture:TTexture;
      Alpha:Byte; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawColorAlpha(const X, Y:Integer; Texture:TTexture; Color:TColor;
      Alpha:Byte; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawColorAlpha(const X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor;
      Alpha:Byte; BlendMode:Integer = Blend_Default; Z:Single = 0.0); overload;

    procedure DrawBlend(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Z:Single = 0.0); overload;
    procedure DrawBlendColorAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; Z:Single = 0.0);
    procedure DrawBlendColor(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Z:Single = 0.0);
    procedure DrawBlendAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Alpha:Byte; Z:Single = 0.0);
    procedure DrawBlend(X, Y:Integer; Texture:TTexture; Z:Single = 0.0); overload;

    procedure FrameRect(const Rect:TRect; const Color:TColor; Z:Single = 0.0; BlendMode:Integer = Blend_Default);
    procedure FillRect(const Rect:TRect; Color:TColor; Z:Single = 0.0; BlendMode:Integer = Blend_Default); overload;
    procedure FillRect(const Rect:TRect; Color1, Color2, Color3, Color4:Cardinal; Filled:Boolean = True; Z:Single = 0.0; BlendMode:Integer = Blend_Default); overload;
    procedure FillRectAlpha(const DestRect:TRect; Color:TColor; Alpha:Integer; Z:Single = 0.0; BlendMode:Integer = Blend_Default);
    procedure Line(Pt1, Pt2:TPoint; Color:TColor; Z:Single = 0.0; BlendMode:Integer = Blend_Default);
    procedure FillTri(const p1, p2, p3:TPoint; c1, c2, c3:TColor;
      Z:Single = 0.0; BlendMode:Integer = Blend_Default); //画三角形
    procedure Circle(X, Y, Radius:Single; Color:TColor; Filled:Boolean = False; Z:Single = 0.0; BlendMode:Integer = Blend_Default);
    //------------------------------------------------------------------------------------------------------------------------
    procedure Draw(Image:TTexture; PatternIndex:Integer; X, Y:Single; BlendMode:Integer); overload;
    procedure Draw(Image:TTexture; PatternIndex:Integer; X, Y, Z:Single; BlendMode:Integer); overload;
    procedure DrawEx(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY:Single;
      MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer); overload;
    procedure DrawEx(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY:Single;
      MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawEx(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
      DoCenter:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
      DoCenter, MirrorX, MirrorY:Boolean; Red, Green, Blue, Alpha:Byte; BlendMode:Integer); overload;
    procedure DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
      DoCenter:Boolean; Red, Green, Blue, Alpha:Byte; BlendMode:Integer); overload;
    procedure DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y:Single;
      Red, Green, Blue, Alpha:Byte; BlendMode:Integer); overload;
    procedure DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
      DoCenter, MirrorX, MirrorY:Boolean; Alpha:Byte; BlendMode:Integer); overload;
    procedure DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
      DoCenter:Boolean; Alpha:Byte; BlendMode:Integer); overload;
    procedure DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y:Single;
      Alpha:Byte; Blendmode:Integer); overload;
    procedure DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
      DoCenter, MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer); overload;
    procedure DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
      DoCenter:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer); overload;
    procedure DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y:Single;
      Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer); overload;
    procedure DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
      DoCenter, MirrorX, MirrorY:Boolean; Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer); overload;
    procedure DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
      DoCenter:Boolean; Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer); overload;
    procedure DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y:Single;
      Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer); overload;
    procedure Draw4V(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
      MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure Draw4V(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
      MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer); overload;
    procedure DrawStretch(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2:Single;
      MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
    procedure DrawPart(Texture:TTexture; X, Y, SrcX, SrcY, Width, Height,
      ScaleX, ScaleY, CenterX, CenterY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawPart(Texture:TTexture; X, Y, SrcX, SrcY, Width, Height:Single;
      Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawRotate(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
      Angle, ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawRotate(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
      Angle:Real; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawRotateColor4(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
      Angle, ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
    procedure DrawRotateC(Image:TTexture; PatternIndex:Integer; X, Y, Angle,
      ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawRotateC(Image:TTexture; PatternIndex:Integer; X, Y, Angle:Single;
      Color:Cardinal; BlendMode:Integer); overload;
    procedure DrawWaveX(Image:TTexture; X, Y, Width, Height:Integer; Amp, Len,
      Phase:Integer; Color:Cardinal; BlendMode:Integer);
    procedure DrawWaveY(Image:TTexture; X, Y, Width, Height:Integer; Amp, Len,
      Phase:Integer; Color:Cardinal; BlendMode:Integer);

    property HGE:THGE read FHGE;

    property Active:Boolean read FActive;
    property Initialized:Boolean read FInitialized;

    property UseSound:Boolean read FUseSound write FUseSound;
    property Windowed:Boolean read FWindowed write FWindowed;
    property Hardware:Boolean read FHardware write FHardware;
    property DepthStencil:Boolean read FDepthStencil write FDepthStencil;
    property VSync:Boolean read FVSync write FVSync;
    property ZBuffer:Single read FZBuffer write SetZBuffer;

    property MaxFontSize:Integer read FMaxFontSize write FMaxFontSize;
    property Width:Integer read FScreenWidth write FScreenWidth;
    property Height:Integer read FScreenHeight write FScreenHeight;
    property BitCount:Integer read FBitCount write FBitCount;
    property Handle:HWnd read FHandle write FHandle;
    property LogFilePath:string read FLogFilePath write FLogFilePath;

    {
    property Active: Boolean read FActive;
    property UseSound: Boolean read GetUseSound write SetUseSound;
    property Windowed: Boolean read GetWindowed write SetWindowed;
    property Hardware: Boolean read GetHardware write SetHardware;
    property DepthStencil: Boolean read GetDepthStencil write SetDepthStencil;
    property VSync: Boolean read GetVSync write SetVSync;
    property ZBuffer: Single read FZBuffer write SetZBuffer;
    property Width: Integer read GetWidth write SetWidth;
    property Height: Integer read GetHeight write SetHeight;

    property BitCount: Integer read GetBitCount write SetBitCount;
    property Handle: HWnd read GetHandle write SetHandle;
    property LogFileName: string read GetLogFileName write SetLogFileName;
    }

    property ClipRect:TRect read GetClipRect write SetClipRect;
    property OnInitialize:TNotifyEvent read FOnInitialize write FOnInitialize;
    property OnFinalize:TNotifyEvent read FOnFinalize write FOnFinalize;

    property OnDeviceLost:TNotifyEvent read GetDeviceLost write SetDeviceLost;
    property OnDeviceReset:TNotifyEvent read GetDeviceReset write SetDeviceReset;
  end;

var
  GameCanvas:THGECanvas = nil;
  TextureFonts:THGEFonts;

  // 游戏中需要用到的三种字体 -- piaoyun
  CurrentFont:THGEFont = nil;
  CurrentUnderlineFont:THGEFont = nil;
  CurrentBoldFont:THGEFont = nil;

  g_CurrentFontHeight:Integer = 12;

implementation
uses HGEDef;

//------------------------------------------------------------------------------

//------------------------------------------------------------------------------

constructor THGECanvas.Create;
begin
  FActive := False;
  FOnInitialize := nil;
  FOnFinalize := nil;
  FZBuffer := 0.0;
  FInitialized := False;
  FUseSound := False;
  FWindowed := True;
  FDepthStencil := False;
  FHardware := False;
  FVSync := False;
  FScreenWidth := 800;
  FScreenHeight := 600;
  FBitCount := 16;
  FHandle := 0;
  FMaxFontSize := 20;
  FLogFilePath := '';
  FHGE := HGECreate(HGE_VERSION);
  TextureFonts := THGEFonts.Create;
end;

destructor THGECanvas.Destroy;
begin
  TextureFonts.Free;
  FHGE.Free;
  inherited;
end;

function THGECanvas.GetDeviceLost:TNotifyEvent;
begin
  Result := FHGE.OnDeviceLost;
end;

procedure THGECanvas.SetDeviceLost(Value:TNotifyEvent);
begin
  FHGE.OnDeviceLost := Value;
end;

function THGECanvas.GetDeviceReset:TNotifyEvent;
begin
  Result := FHGE.OnDeviceReset;
end;

procedure THGECanvas.SetDeviceReset(Value:TNotifyEvent);
begin
  FHGE.OnDeviceReset := Value;
end;

{ TODO -opiaoyun -cHGE注释 : HGECanvas类初始化-字体，窗口大小等【2013-6-20】 }

function THGECanvas.Initialize:Boolean;
//var
  //D3DCaps8: PD3DCaps8;
  //MaxTextureWidth: LongWord;
  //MaxTextureHeight: LongWord;
begin
  Result := False;

  FHGE.System_SetState(HGE_HARDWARE, FHardware);
  FHGE.System_SetState(HGE_USESOUND, FUseSound);
  FHGE.System_SetState(HGE_WINDOWED, FWindowed);
  FHGE.System_SetState(HGE_ZBUFFER, FDepthStencil);
  if FVSync then
    FHGE.System_SetState(HGE_FPS, HGEFPS_VSYNC)
  else
    FHGE.System_SetState(HGE_FPS, HGEFPS_UNLIMITED);
  FHGE.System_SetState(HGE_SCREENWIDTH, FScreenWidth);
  FHGE.System_SetState(HGE_SCREENHEIGHT, FScreenHeight);
  FHGE.System_SetState(HGE_SCREENBPP, FBitCount);
  FHGE.System_SetState(HGE_HWND, FHandle);
  FHGE.System_SetState(HGE_LOGFILE, FLogFilePath);

  if FHGE.System_Initiate then begin
    //D3DCaps8 := FHGE.D3DCaps8;

    {
    if D3DCaps8 <> nil then
    begin
      MaxTextureWidth := Min(D3DCaps8.MaxTextureWidth, 2048);
      MaxTextureHeight := Min(D3DCaps8.MaxTextureHeight, 2048);
    end
    else
    begin
      MaxTextureWidth := 2048;
      MaxTextureHeight := 2048;
    end;
    }

    CurrentFont := THGEFont.Create(1000);
    CurrentFont.Font.Name := '宋体';
    CurrentFont.Font.Size := 9;
    CurrentFont.Font.Style := [];
    CurrentFont.m_dwFreeMemCheckTime := 1000 * 60 * 5;

    CurrentUnderlineFont := THGEFont.Create(500);
    CurrentUnderlineFont.Font.Name := '宋体';
    CurrentUnderlineFont.Font.Size := 9;
    CurrentUnderlineFont.Font.Style := [fsUnderline];
    CurrentUnderlineFont.m_dwFreeMemCheckTime := 1000 * 60 * 3;

    CurrentBoldFont := THGEFont.Create(500);
    CurrentBoldFont.Font.Name := '宋体';
    CurrentBoldFont.Font.Size := 10;
    CurrentBoldFont.Font.Style := [fsBold];
    CurrentBoldFont.m_dwFreeMemCheckTime := 1000 * 60 * 3;

    TextureFonts.Add(CurrentFont);
    TextureFonts.Add(CurrentUnderlineFont);
    TextureFonts.Add(CurrentBoldFont);
    FActive := True;
    TextureFonts.Initialize;
    //FActive := True;

    Result := FActive;
    FInitialized := False;

    if Assigned(FOnInitialize) then
      FOnInitialize(Self);
    FInitialized := True;
  end;
end;

procedure THGECanvas.Finalize;
begin
  if FActive then begin
    FActive := False;
    FInitialized := False;
    if Assigned(FOnFinalize) then
      FOnFinalize(Self);

    TextureFonts.Finalize;
    FHGE.System_Shutdown;
  end;
end;

procedure THGECanvas.Render(Handler:TNotifyEvent; Background:Cardinal; FillBk:Boolean; Target:TTarget);
begin
  if FActive and FInitialized then begin
    if FHGE.Gfx_CanBegin then begin
      FHGE.Gfx_BeginScene(Target);

      if FillBk then
        FHGE.Gfx_Clear(Background);

      FHGE.RenderBatch;

      try
        Handler(Self);
      except
        on E:Exception do begin
          FHGE.System_Log('[Exception] Handler');
          FHGE.System_Log(E.Message);
        end;
      end;
      //FHGE.RenderBatch;
      FHGE.Gfx_EndScene;
    end;
  end;
end;

procedure THGECanvas.Resize(const AWidth, AHeight:Integer);
begin
  FHGE.Resize(AWidth, AHeight);
end;

{
procedure THGECanvas.SetHardware(Value: Boolean);
begin
  FHGE.System_SetState(HGE_HARDWARE, Value);
end;

procedure THGECanvas.SetUseSound(Value: Boolean);
begin
  FHGE.System_SetState(HGE_USESOUND, Value);
end;

procedure THGECanvas.SetWindowed(Value: Boolean);
begin
  FHGE.System_SetState(HGE_WINDOWED, Value);
end;

procedure THGECanvas.SetDepthStencil(Value: Boolean);
begin
  FHGE.System_SetState(HGE_ZBUFFER, Value);
end;

procedure THGECanvas.SetVSync(Value: Boolean);
begin
  if Value then
    FHGE.System_SetState(HGE_FPS, HGEFPS_VSYNC)
  else
    FHGE.System_SetState(HGE_FPS, HGEFPS_UNLIMITED);
end;
}

procedure THGECanvas.SetZBuffer(Value:Single);
begin
  FZBuffer := Value;
end;

procedure THGECanvas.SetWidth(Value:Integer);
begin
  FHGE.System_SetState(HGE_SCREENWIDTH, Value);
end;

procedure THGECanvas.SetHeight(Value:Integer);
begin
  FHGE.System_SetState(HGE_SCREENHEIGHT, Value);
end;

{
procedure THGECanvas.SetBitCount(Value: Integer);
begin
  FHGE.System_SetState(HGE_SCREENBPP, Value);
end;

procedure THGECanvas.SetHandle(Value: Hwnd);
begin
  FHGE.System_SetState(HGE_HWND, Value);
end;

procedure THGECanvas.SetLogFileName(Value: string);
begin
  FHGE.System_SetState(HGE_LOGFILE, Value);
end;

function THGECanvas.GetHardware: Boolean;
begin
  Result := FHGE.System_GetState(HGE_HARDWARE);
end;

function THGECanvas.GetUseSound: Boolean;
begin
  Result := FHGE.System_GetState(HGE_USESOUND);
end;

function THGECanvas.GetWindowed: Boolean;
begin
  Result := FHGE.System_GetState(HGE_WINDOWED);
end;

function THGECanvas.GetDepthStencil: Boolean;
begin
  Result := FHGE.System_GetState(HGE_ZBUFFER);
end;

function THGECanvas.GetVSync: Boolean;
begin
  if FHGE.System_GetState(HGE_FPS) = HGEFPS_VSYNC then
    Result := True
  else
    Result := False;
end;

function THGECanvas.GetZBuffer: Single;
begin
  Result := ZBuffer;
end;

function THGECanvas.GetWidth: Integer;
begin
  Result := FHGE.System_GetState(HGE_SCREENWIDTH);
end;

function THGECanvas.GetHeight: Integer;
begin
  Result := FHGE.System_GetState(HGE_SCREENHEIGHT);
end;

function THGECanvas.GetBitCount: Integer;
begin
  Result := FHGE.System_GetState(HGE_SCREENBPP);
end;

function THGECanvas.GetHandle: Hwnd;
begin
  Result := FHGE.System_GetState(HGE_HWND);
end;

function THGECanvas.GetLogFileName: string;
begin
  Result := FHGE.System_GetState(HGE_LOGFILE);
end;
}

function THGECanvas.GetClipRect:TRect;
var
  X, Y, W, H:Integer;
begin
  FHGE.Gfx_GetClipping(X, Y, W, H);
  Result := Rect(X, Y, W, H);
end;

procedure THGECanvas.SetClipRect(Value:TRect);
begin
  FHGE.Gfx_SetClipping(Value.Left, Value.Top, Value.Right - Value.Left, Value.Bottom - Value.Top);
end;

procedure THGECanvas.SetColor(Color:Cardinal);
begin
  FQuad.V[0].Col := Color;
  FQuad.V[1].Col := Color;
  FQuad.V[2].Col := Color;
  FQuad.V[3].Col := Color;
end;

procedure THGECanvas.SetColor(Color1:Cardinal; Color2:Cardinal; Color3:Cardinal; Color4:Cardinal);
begin
  FQuad.V[0].Col := Color1;
  FQuad.V[1].Col := Color2;
  FQuad.V[2].Col := Color3;
  FQuad.V[3].Col := Color4;
end;

procedure THGECanvas.SetPattern(Texture:TTexture);
var
  TexX1, TexY1, TexX2, TexY2:Single;
  Left, Right, Top, Bottom:Integer;
  PHeight, PWidth:Integer;
begin
  if Assigned(Texture) then begin
    FTexWidth := FHGE.Texture_GetWidth(Texture);
    FTexHeight := FHGE.Texture_GetHeight(Texture);
  end else begin
    FTexWidth := 1;
    FTexHeight := 1;
  end;

  FQuad.V[0].TX := 0;
  FQuad.V[0].TY := 0;
  FQuad.V[1].TX := 0;
  FQuad.V[1].TY := 0;
  FQuad.V[2].TX := 0;
  FQuad.V[2].TY := 0;
  FQuad.V[3].TX := 0;
  FQuad.V[3].TY := 0;
  FQuad.Tex := nil;
  if (FTexWidth <= 0) or (FTexHeight <= 0) then Exit;
  FQuad.Tex := Texture;

  PHeight := FTexWidth;
  PWidth := FTexHeight;

  Left := 0;
  Right := Left + PWidth;
  Top := 0;
  Bottom := Top + PHeight;

  FWidth := Right - Left;
  FHeight := Bottom - Top;

  TexX1 := Left / FTexWidth;
  TexY1 := Top / FTexHeight;
  TexX2 := (Left + FWidth) / FTexWidth;
  TexY2 := (Top + FHeight) / FTexHeight;

  FQuad.V[0].TX := TexX1;
  FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2;
  FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2;
  FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1;
  FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetPattern(Texture:TTexture; ClientRect:TRect);
var
  TexX1, TexY1, TexX2, TexY2:Single;
  Left, Right, Top, Bottom:Integer;
  PHeight, PWidth:Integer;
  SrcRect:TRect;
begin
  if Assigned(Texture) then begin
    FTexWidth := FHGE.Texture_GetWidth(Texture);
    FTexHeight := FHGE.Texture_GetHeight(Texture);
    SrcRect := ShortRect(ClientRect, Texture.ClientRect);
  end else begin
    FTexWidth := 1;
    FTexHeight := 1;
    SrcRect := ShortRect(ClientRect, Rect(0, 0, 1, 1));
  end;
  FQuad.V[0].TX := 0;
  FQuad.V[0].TY := 0;
  FQuad.V[1].TX := 0;
  FQuad.V[1].TY := 0;
  FQuad.V[2].TX := 0;
  FQuad.V[2].TY := 0;
  FQuad.V[3].TX := 0;
  FQuad.V[3].TY := 0;
  FQuad.Tex := nil;
  if (FTexWidth <= 0) or (FTexHeight <= 0) then Exit;
  PHeight := SrcRect.Bottom - SrcRect.Top;
  PWidth := SrcRect.Right - SrcRect.Left;
  if (PWidth <= 0) or (PHeight <= 0) then Exit;

  FQuad.Tex := Texture;

  Left := SrcRect.Left;
  Right := Left + PWidth;
  Top := SrcRect.Top;
  Bottom := Top + PHeight;

  FWidth := Right - Left;
  FHeight := Bottom - Top;

  TexX1 := Left / FTexWidth;
  TexY1 := Top / FTexHeight;
  TexX2 := (Left + FWidth) / FTexWidth;
  TexY2 := (Top + FHeight) / FTexHeight;

  FQuad.V[0].TX := TexX1;
  FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2;
  FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2;
  FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1;
  FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetPattern(Texture:TTexture; PatternIndex:Integer);
var
  TexX1, TexY1, TexX2, TexY2:Single;
  Left, Right, Top, Bottom:Integer;
  PHeight, PWidth, RowCount, ColCount:Integer;
begin
  if Assigned(Texture) then begin
    FTexWidth := FHGE.Texture_GetWidth(Texture);
    FTexHeight := FHGE.Texture_GetHeight(Texture);
  end else begin
    FTexWidth := 1;
    FTexHeight := 1;
  end;
  if (FTexWidth <= 0) or (FTexHeight <= 0) then Exit;

  FQuad.Tex := Texture;

  PHeight := Max(Texture.PatternHeight, 1);
  PWidth := Max(Texture.PatternWidth, 1);
  ColCount := Texture.Width div PWidth;
  RowCount := Texture.Height div PHeight;

  if PatternIndex < 0 then PatternIndex := 0;
  if PatternIndex >= RowCount * ColCount then
    PatternIndex := RowCount * ColCount - 1;
  Left := (PatternIndex mod ColCount) * PWidth;
  Right := Left + PWidth;
  Top := (PatternIndex div ColCount) * PHeight;
  Bottom := Top + PHeight;
  //FTX := TexX;
  //FTY := TexY;
  FWidth := Right - Left;
  FHeight := Bottom - Top;

  TexX1 := Left / FTexWidth;
  TexY1 := Top / FTexHeight;
  TexX2 := (Left + FWidth) / FTexWidth;
  TexY2 := (Top + FHeight) / FTexHeight;

  FQuad.V[0].TX := TexX1;
  FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2;
  FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2;
  FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1;
  FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetMirror(MirrorX, MirrorY:Boolean);
var
  TX, TY:Single;
begin
  if (MirrorX) then begin
    TX := FQuad.V[0].TX;
    FQuad.V[0].TX := FQuad.V[1].TX;
    FQuad.V[1].TX := TX;
    TY := FQuad.V[0].TY;
    FQuad.V[0].TY := FQuad.V[1].TY;
    FQuad.V[1].TY := TY;
    TX := FQuad.V[3].TX;
    FQuad.V[3].TX := FQuad.V[2].TX;
    FQuad.V[2].TX := TX;
    TY := FQuad.V[3].TY;
    FQuad.V[3].TY := FQuad.V[2].TY;
    FQuad.V[2].TY := TY;
  end;

  if (MirrorY) then begin
    TX := FQuad.V[0].TX;
    FQuad.V[0].TX := FQuad.V[3].TX;
    FQuad.V[3].TX := TX;
    TY := FQuad.V[0].TY;
    FQuad.V[0].TY := FQuad.V[3].TY;
    FQuad.V[3].TY := TY;
    TX := FQuad.V[1].TX;
    FQuad.V[1].TX := FQuad.V[2].TX;
    FQuad.V[2].TX := TX;
    TY := FQuad.V[1].TY;
    FQuad.V[1].TY := FQuad.V[2].TY;
    FQuad.V[2].TY := TY;
  end;
end;

procedure THGECanvas.Draw(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer; Z:Single);
var
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  if Texture = nil then Exit;

  SetPattern(Texture, SrcRect);
  if FQuad.Tex <> Texture then Exit;
  SetColor(Color1, Color2, Color3, Color4);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;

  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw(X, Y:Integer; SrcRect:TRect; Texture:TTexture; BlendMode:Integer; Z:Single);
begin
  Draw(X, Y, SrcRect, Texture, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF, BlendMode, Z);
end;

procedure THGECanvas.Draw(X, Y:Integer; Texture:TTexture; BlendMode:Integer; Z:Single);
begin
  if Texture <> nil then
    Draw(X, Y, Texture.ClientRect, Texture, BlendMode, Z);
end;

procedure THGECanvas.DrawColor(X, Y:Integer; Texture:TTexture; Color:TColor; BlendMode:Integer; Z:Single);
begin
  if Texture <> nil then
    DrawColor(X, Y, Texture.ClientRect, Texture, Color, BlendMode, Z);
end;

procedure THGECanvas.DrawColor(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; BlendMode:Integer; Z:Single);
var
  Color4:Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

{ TODO -ochongchong -c新增 : 文字添加半透明支持 【2013-08-02】}

procedure THGECanvas.DrawColorEx(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; BlendMode:Integer; Z:Single);
var
  Color4:Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  Color4 := Color4 and ((Alpha shl 24) or $00FFFFFF);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture; Alpha:Byte;
  BlendMode:Integer; Z:Single);
var
  TempX1, TempY1, TempX2, TempY2:Single;
  ScaleX, ScaleY:Single;
  PaintRect:TRect;
  SWidth, SHeight:Integer;
  DWidth, DHeight:Integer;

  Color4:Cardinal;
begin
  Color4 := ARGB(Alpha, 255, 255, 255);
  SetPattern(Texture, SrcRect);
  if FQuad.Tex <> Texture then Exit;
  SetColor(Color4, Color4, Color4, Color4);
  SWidth := DestRect.Right - DestRect.Left;
  SHeight := DestRect.Bottom - DestRect.Top;
  if (SWidth <= 0) or (SHeight <= 0) then Exit;

  PaintRect := ShortRect(SrcRect, Texture.ClientRect);
  DWidth := PaintRect.Right - PaintRect.Left;
  DHeight := PaintRect.Bottom - PaintRect.Top;

  if (DWidth <= 0) or (DHeight <= 0) then Exit;
  ScaleX := SWidth / DWidth;
  ScaleY := SHeight / DHeight;
  TempX1 := DestRect.Left;
  TempY1 := DestRect.Top;
  TempX2 := (DestRect.Left + FWidth * ScaleX);
  TempY2 := (DestRect.Top + FHeight * ScaleY);

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;

  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture; Alpha:Byte; Color:TColor;
  BlendMode:Integer; Z:Single);
var
  TempX1, TempY1, TempX2, TempY2:Single;
  ScaleX, ScaleY:Single;
  PaintRect:TRect;
  SWidth, SHeight:Integer;
  DWidth, DHeight:Integer;

  Color4:Cardinal;
  Red, Green, Blue:Byte;
begin
  Red := Byte(Color);
  Green := Byte(Color shr 8);
  Blue := Byte(Color shr 16);
  Color4 := ARGB(Alpha, Red, Green, Blue);
  SetPattern(Texture, SrcRect);
  if FQuad.Tex <> Texture then Exit;
  SetColor(Color4, Color4, Color4, Color4);
  SWidth := DestRect.Right - DestRect.Left;
  SHeight := DestRect.Bottom - DestRect.Top;
  if (SWidth <= 0) or (SHeight <= 0) then Exit;

  PaintRect := ShortRect(SrcRect, Texture.ClientRect);
  DWidth := PaintRect.Right - PaintRect.Left;
  DHeight := PaintRect.Bottom - PaintRect.Top;

  if (DWidth <= 0) or (DHeight <= 0) then Exit;
  ScaleX := SWidth / DWidth;
  ScaleY := SHeight / DHeight;
  TempX1 := DestRect.Left;
  TempY1 := DestRect.Top;
  TempX2 := (DestRect.Left + FWidth * ScaleX);
  TempY2 := (DestRect.Top + FHeight * ScaleY);

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;

  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.StretchDraw(const DestRect, SrcRect:TRect; Texture:TTexture;
  BlendMode:Integer; Z:Single);
var
  TempX1, TempY1, TempX2, TempY2:Single;
  ScaleX, ScaleY:Single;
  PaintRect:TRect;
  SWidth, SHeight:Integer;
  DWidth, DHeight:Integer;
begin
  SetPattern(Texture, SrcRect);
  if FQuad.Tex <> Texture then Exit;
  SetColor($FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF);
  SWidth := DestRect.Right - DestRect.Left;
  SHeight := DestRect.Bottom - DestRect.Top;
  if (SWidth <= 0) or (SHeight <= 0) then Exit;

  PaintRect := ShortRect(SrcRect, Texture.ClientRect);
  DWidth := PaintRect.Right - PaintRect.Left;
  DHeight := PaintRect.Bottom - PaintRect.Top;

  if (DWidth <= 0) or (DHeight <= 0) then Exit;
  ScaleX := SWidth / DWidth;
  ScaleY := SHeight / DHeight;
  TempX1 := DestRect.Left;
  TempY1 := DestRect.Top;
  TempX2 := (DestRect.Left + FWidth * ScaleX);
  TempY2 := (DestRect.Top + FHeight * ScaleY);

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;

  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.StretchDraw(const DestRect:TRect; Texture:TTexture;
  BlendMode:Integer; Z:Single);
begin
  StretchDraw(DestRect, Texture.ClientRect, Texture, BlendMode, Z);
end;

procedure THGECanvas.DrawAlpha(X, Y:Integer; Texture:TTexture; Alpha:Byte;
  BlendMode:Integer; Z:Single);
begin
  DrawAlpha(X, Y, Texture.ClientRect, Texture, Alpha, BlendMode, Z);
end;

procedure THGECanvas.DrawAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Alpha:Byte;
  BlendMode:Integer; Z:Single);
var
  Color4:Cardinal;
begin
  Color4 := ARGB(Alpha, 255, 255, 255);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawColorAlpha(const X, Y:Integer; Texture:TTexture; Color:TColor;
  Alpha:Byte; BlendMode:Integer; Z:Single);
var
  Color4:Cardinal;
  Red, Green, Blue:Byte;
begin
  Red := Byte(Color);
  Green := Byte(Color shr 8);
  Blue := Byte(Color shr 16);
  Color4 := ARGB(Alpha, Red, Green, Blue);
  Draw(X, Y, Texture.ClientRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawColorAlpha(const X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor;
  Alpha:Byte; BlendMode:Integer; Z:Single);
var
  Color4:Cardinal;
  Red, Green, Blue:Byte;
begin
  Red := Byte(Color);
  Green := Byte(Color shr 8);
  Blue := Byte(Color shr 16);
  Color4 := ARGB(Alpha, Red, Green, Blue);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawBlendColorAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; Z:Single);
var
  Color4:Cardinal;
  Red, Green, Blue:Byte;
begin
  if Alpha >= 255 then begin
    Color4 := DisplaceRB(Color) or $FF000000;
  end else begin
    Red := Byte(Color);
    Green := Byte(Color shr 8);
    Blue := Byte(Color shr 16);
    Color4 := ARGB(Alpha, Red, Green, Blue);
  end;
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.DrawBlendColor(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Z:Single);
var
  Color4:Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.DrawBlendAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Alpha:Byte; Z:Single);
var
  Color4:Cardinal;
begin
  Color4 := ARGB(Alpha, 255, 255, 255);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.DrawBlend(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Z:Single);
begin
  Draw(X, Y, SrcRect, Texture, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.DrawBlend(X, Y:Integer; Texture:TTexture; Z:Single);
begin
  Draw(X, Y, Texture.ClientRect, Texture, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.FrameRect(const Rect:TRect; const Color:TColor; Z:Single; BlendMode:Integer);
var
  Color4:Cardinal;
begin
  if (Rect.Right > Rect.Left) and (Rect.Bottom > Rect.Top) then begin
    Color4 := DisplaceRB(Color) or $FF000000;
    //HGE.Rectangle(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, Color4, False, Z, BlendMode);
    HGE.Quadrangle(
      Rect.Left - 1, Rect.Top,
      Rect.Right, Rect.Top,
      Rect.Right, Rect.Bottom,
      Rect.Left, Rect.Bottom,
      Color4,
      False, Z, BlendMode);

    {  HGE.Line2Color(Rect.Left, Rect.Top, Rect.Right, Rect.Top, Color4, Color4, Z, BlendMode);
      HGE.Line2Color(Rect.Left, Rect.Bottom, Rect.Right, Rect.Bottom, Color4, Color4, Z, BlendMode);

      HGE.Line2Color(Rect.Left, Rect.Top-1, Rect.Left, Rect.Bottom, Color4, Color4, Z, BlendMode);
      HGE.Line2Color(Rect.Right, Rect.Top, Rect.Right, Rect.Bottom, Color4, Color4, Z, BlendMode);   }

  end;
end;

procedure THGECanvas.FillRect(const Rect:TRect; Color:TColor; Z:Single; BlendMode:Integer);
var
  Color4:Cardinal;
begin
  if (Rect.Right > Rect.Left) and (Rect.Bottom > Rect.Top) then begin
    Color4 := DisplaceRB(Color) or $FF000000;

    //HGE.Rectangle(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, Color4, True, Z, BlendMode);
    HGE.Quadrangle(
      Rect.Left, Rect.Top,
      Rect.Right, Rect.Top,
      Rect.Right, Rect.Bottom,
      Rect.Left, Rect.Bottom,
      Color4,
      True, Z, BlendMode);
  end;
end;

procedure THGECanvas.FillRect(const Rect:TRect; Color1, Color2, Color3, Color4:Cardinal; Filled:Boolean; Z:Single; BlendMode:Integer);
begin
  if (Rect.Right > Rect.Left) and (Rect.Bottom > Rect.Top) then begin

    //  HGE.Quadrangle4Color(Rect.Left, Rect.Top, Rect.Right, Rect.Top, Rect.Right, Rect.Bottom,
    //    Rect.Left, Rect.Bottom, Color1, Color2, Color3, Color4, Filled, Z, BlendMode);
    HGE.Quadrangle4Color(
      Rect.Left, Rect.Top,
      Rect.Right, Rect.Top,
      Rect.Right, Rect.Bottom,
      Rect.Left, Rect.Bottom,
      Color1, Color2, Color3, Color4,
      True, Z, BlendMode);
  end;
end;

procedure THGECanvas.FillRectAlpha(const DestRect:TRect; Color:TColor; Alpha:Integer; Z:Single; BlendMode:Integer);
var
  Color4:Cardinal;
  Red, Green, Blue:Byte;
begin
  if (DestRect.Right > DestRect.Left) and (DestRect.Bottom > DestRect.Top) then begin
    Red := Byte(Color);
    Green := Byte(Color shr 8);
    Blue := Byte(Color shr 16);
    Color4 := ARGB(Alpha, Red, Green, Blue);

    HGE.Rectangle(DestRect.Left, DestRect.Top, DestRect.Right - DestRect.Left, DestRect.Bottom - DestRect.Top, Color4, True, Z, BlendMode);
  end;
end;

procedure THGECanvas.Line(Pt1, Pt2:TPoint; Color:TColor; Z:Single; BlendMode:Integer);
var
  Color4:Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  HGE.Line2Color(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y, Color4, Color4, Z, BlendMode);
end;

procedure THGECanvas.FillTri(const p1, p2, p3:TPoint; c1, c2, c3:TColor;
  Z:Single; BlendMode:Integer); //画三角形
var
  Color1, Color2, Color3:Cardinal;
begin
  Color1 := DisplaceRB(c1) or $FF000000;
  Color2 := DisplaceRB(c2) or $FF000000;
  Color3 := DisplaceRB(c3) or $FF000000;
  HGE.Triangle(P1.X, P1.Y, P2.X, P2.Y, P3.X, P3.Y, Color1, Color2, Color3, True, Z, BlendMODE);
end;

procedure THGECanvas.Circle(X, Y, Radius:Single; Color:TColor; Filled:Boolean; Z:Single; BlendMode:Integer);
var
  Color4:Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  HGE.Circle(X, Y, Radius, Color4, Filled, Z, BlendMode);
end;
//--------------------------------------------------------------------------------------------------------------

procedure THGECanvas.Draw(Image:TTexture; PatternIndex:Integer; X, Y, Z:Single; BlendMode:Integer);
var
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor($FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw(Image:TTexture; PatternIndex:Integer; X, Y:Single; BlendMode:Integer);
var
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor($FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY:Single;
  MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
var
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color1, Color2, Color3, Color4);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
var
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;

  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image:TTexture; PatternIndex:Integer; X:Single; Y:Single; Scale:Single; DoCenter:Boolean; Color:Cardinal; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
  DoCenter, MirrorX, MirrorY:Boolean; Red, Green, Blue, Alpha:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
  DoCenter:Boolean; Red, Green, Blue, Alpha:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawColor1(Image:TTexture; PatternIndex:Integer; X, Y:Single;
  Red, Green, Blue, Alpha:Byte; BlendMode:Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
  DoCenter, MirrorX, MirrorY:Boolean; Alpha:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
  DoCenter:Boolean; Alpha:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;

  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image:TTexture; PatternIndex:Integer; X, Y:Single;
  Alpha:Byte; Blendmode:Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
  DoCenter, MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
  DoCenter:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, centerPosY, Scale, Scale,
    False, False, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawColor4(Image:TTexture; PatternIndex:Integer; X, Y:Single;
  Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y, ScaleX, ScaleY:Single;
  DoCenter, MirrorX, MirrorY:Boolean; Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y, Scale:Single;
  DoCenter:Boolean; Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer);
var
  CenterPosX, CenterPosY:Single;
begin
  if DoCenter then begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image:TTexture; PatternIndex:Integer; X, Y:Single;
  Alpha1, Alpha2, Alpha3, Alpha4:Byte; BlendMode:Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.Draw4V(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
  MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  FQuad.V[0].X := X1;
  FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2;
  FQuad.V[1].Y := Y2;
  FQuad.V[2].X := X3;
  FQuad.V[2].Y := Y3;
  FQuad.V[3].X := X4;
  FQuad.V[3].Y := Y4;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw4V(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
  MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color1, Color2, Color3, Color4);
  FQuad.V[0].X := X1;
  FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2;
  FQuad.V[1].Y := Y2;
  FQuad.V[2].X := X3;
  FQuad.V[2].Y := Y3;
  FQuad.V[3].X := X4;
  FQuad.V[3].Y := Y4;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawStretch(Image:TTexture; PatternIndex:Integer; X1, Y1, X2, Y2:Single;
  MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  FQuad.V[0].X := X1;
  FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2;
  FQuad.V[1].Y := Y1;
  FQuad.V[2].X := X2;
  FQuad.V[2].Y := Y2;
  FQuad.V[3].X := X1;
  FQuad.V[3].Y := Y2;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawPart(Texture:TTexture; X, Y, SrcX, SrcY, Width, Height,
  ScaleX, ScaleY, CenterX, CenterY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
var
  TexX1, TexY1, TexX2, TexY2:Single;
  TempX1, TempY1, TempX2, TempY2:Single;
begin
  // FTX := SrcX;
  // FTY := SrcY;
  FWidth := Width;
  FHeight := Height;

  if Assigned(Texture) then begin
    FTexWidth := FHGE.Texture_GetWidth(Texture);
    FTexHeight := FHGE.Texture_GetHeight(Texture);
  end else begin
    FTexWidth := 1;
    FTexHeight := 1;
  end;

  FQuad.Tex := Texture;

  TexX1 := SrcX / FTexWidth;
  TexY1 := SrcY / FTexHeight;
  TexX2 := (SrcX + Width) / FTexWidth;
  TexY2 := (SrcY + Height) / FTexHeight;

  FQuad.V[0].TX := TexX1;
  FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2;
  FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2;
  FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1;
  FQuad.V[3].TY := TexY2;

  FQuad.V[0].Z := 0.5;
  FQuad.V[1].Z := 0.5;
  FQuad.V[2].Z := 0.5;
  FQuad.V[3].Z := 0.5;
  SetColor(Color);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1;
  FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2;
  FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2;
  FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1;
  FQuad.V[3].Y := TempY2;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawPart(Texture:TTexture; X, Y, SrcX, SrcY, Width, Height:Single;
  Color:Cardinal; BlendMode:Integer);
begin
  DrawPart(Texture, X, Y, SrcX, SrcY, Width, Height, 1, 1, 0, 0, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawRotate(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
  Angle, ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
var
  TX1, TY1, TX2, TY2, SinT, CosT:Single;
begin
  // if (VScale=0) then
    // VScale := HScale;
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  TX1 := -CenterX * ScaleX;
  TY1 := -CenterY * ScaleY;
  TX2 := (FWidth - CenterX) * ScaleX;
  TY2 := (FHeight - CenterY) * ScaleY;

  if (Angle <> 0.0) then begin
    CosT := Cos(Angle);
    SinT := Sin(Angle);

    FQuad.V[0].X := TX1 * CosT - TY1 * SinT + X;
    FQuad.V[0].Y := TX1 * SinT + TY1 * CosT + Y;

    FQuad.V[1].X := TX2 * CosT - TY1 * SinT + X;
    FQuad.V[1].Y := TX2 * SinT + TY1 * CosT + Y;

    FQuad.V[2].X := TX2 * CosT - TY2 * SinT + X;
    FQuad.V[2].Y := TX2 * SinT + TY2 * CosT + Y;

    FQuad.V[3].X := TX1 * CosT - TY2 * SinT + X;
    FQuad.V[3].Y := TX1 * SinT + TY2 * CosT + Y;
  end else begin
    FQuad.V[0].X := TX1 + X;
    FQuad.V[0].Y := TY1 + Y;
    FQuad.V[1].X := TX2 + X;
    FQuad.V[1].Y := TY1 + Y;
    FQuad.V[2].X := TX2 + X;
    FQuad.V[2].Y := TY2 + Y;
    FQuad.V[3].X := TX1 + X;
    FQuad.V[3].Y := TY2 + Y;
  end;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawRotate(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
  Angle:Real; Color:Cardinal; BlendMode:Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, CenterX, CenterY, Angle, 1, 1, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawRotateColor4(Image:TTexture; PatternIndex:Integer; X, Y, CenterX, CenterY,
  Angle, ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color1, Color2, Color3, Color4:Cardinal; BlendMode:Integer);
var
  TX1, TY1, TX2, TY2, SinT, CosT:Single;
begin
  // if (VScale=0) then
    // VScale := HScale;
  SetPattern(Image, PatternIndex);
  SetColor(Color1, Color2, Color3, Color4);
  TX1 := -CenterX * ScaleX;
  TY1 := -CenterY * ScaleY;
  TX2 := (FWidth - CenterX) * ScaleX;
  TY2 := (FHeight - CenterY) * ScaleY;

  if (Angle <> 0.0) then begin
    CosT := Cos(Angle);
    SinT := Sin(Angle);

    FQuad.V[0].X := TX1 * CosT - TY1 * SinT + X;
    FQuad.V[0].Y := TX1 * SinT + TY1 * CosT + Y;

    FQuad.V[1].X := TX2 * CosT - TY1 * SinT + X;
    FQuad.V[1].Y := TX2 * SinT + TY1 * CosT + Y;

    FQuad.V[2].X := TX2 * CosT - TY2 * SinT + X;
    FQuad.V[2].Y := TX2 * SinT + TY2 * CosT + Y;

    FQuad.V[3].X := TX1 * CosT - TY2 * SinT + X;
    FQuad.V[3].Y := TX1 * SinT + TY2 * CosT + Y;
  end else begin
    FQuad.V[0].X := TX1 + X;
    FQuad.V[0].Y := TY1 + Y;
    FQuad.V[1].X := TX2 + X;
    FQuad.V[1].Y := TY1 + Y;
    FQuad.V[2].X := TX2 + X;
    FQuad.V[2].Y := TY2 + Y;
    FQuad.V[3].X := TX1 + X;
    FQuad.V[3].Y := TY2 + Y;
  end;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);

end;

procedure THGECanvas.DrawRotateC(Image:TTexture; PatternIndex:Integer; X, Y, Angle,
  ScaleX, ScaleY:Single; MirrorX, MirrorY:Boolean; Color:Cardinal; BlendMode:Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, Image.PatternWidth div 2, Image.PatternHeight div 2,
    Angle, ScaleX, ScaleY, MirrorX, MirrorY, Color, BlendMode);
end;

procedure THGECanvas.DrawRotateC(Image:TTexture; PatternIndex:Integer; X, Y, Angle:Single;
  Color:Cardinal; BlendMode:Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, Image.PatternWidth div 2, Image.PatternHeight div 2,
    Angle, 1, 1, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawWaveX(Image:TTexture; X, Y, Width, Height:Integer; Amp, Len,
  Phase:Integer; Color:Cardinal; BlendMode:Integer);
var
  I, J:Integer;
begin
  for J := 0 to Width do begin
    I := Trunc(J * Image.PatternWidth / Width);
    DrawPart(Image, X + J, Y + Amp * Sin((Phase + J) * PI * Width / Len / 256),
      I, 0, 1, Height, Color, BlendMode);
  end;
end;

procedure THGECanvas.DrawWaveY(Image:TTexture; X, Y, Width, Height:Integer; Amp, Len,
  Phase:Integer; Color:Cardinal; BlendMode:Integer);
var
  I, J:Integer;
begin
  for J := 0 to Height do begin
    I := Trunc(J * Image.PatternHeight / Height);
    DrawPart(Image, X + Amp * Sin((Phase + J) * PI * Height / Len / 256), Y + J,
      0, I, Width, 1, Color, BlendMode);
  end;
end;

initialization

end.
