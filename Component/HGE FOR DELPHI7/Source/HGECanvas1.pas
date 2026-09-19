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
  HashTable,
  TextureImages,
  HGEImages,
  HGE;


type
  TTextTile = class(TTextureTile)
    m_dwFreeMemCheckTime: LongWord;
  private
    FList: THashTable;
    FFont: TFont;
    FFontWidth: Integer;
    FFontHeight: Integer;
    FFontWidthBold: Integer;

    FDoubleFontWidth: Integer;
    FDoubleFontHeight: Integer;
    FDoubleFontWidthBold: Integer;
    FProcIdx: Integer;

    procedure SetFont(Value: TFont);
  public
    constructor Create(AOwner: TTextureImages; ATextCount: Integer = 2000);
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Finalize; override;
    procedure InitEnglishString;
    procedure GetFontSize;
    procedure FreeOldMemorys;
    procedure TextRect(const X, Y: Integer; SrcRect: TRect; const ImageIndexs: TImageIndexs; Color: TColor = clWhite); overload;
    procedure TextRect(const X, Y: Integer; SrcRect: TRect; const Text: string; Color: TColor = clWhite); overload;
    procedure TextRect(const X, Y: Integer; SrcRect: TRect; ImageInfos: TImageInfos; Color: TColor = clWhite); overload;

    procedure TextOut(const X, Y: Integer; const Text: string; Color: TColor = clWhite); overload; override;
    procedure TextOut(const X, Y: Integer; ImageIndexs: TImageIndexs; Color: TColor = clWhite); overload; override;


    function TextWidth(const Text: string): Integer;
    function TextHeight(const Text: string): Integer;
    function GetImageInfo(const Text: string): TImageInfo; override;
    function GetImageInfos(const Text: string): TImageInfos;
    function Add(const Text: string): pTImageRect; override;
    property Font: TFont read FFont write SetFont;
    property List: THashTable read FList;

  end;

  TTextureFonts = class(TTextureImages)
  public
    procedure FreeOldMemorys; override;
    function FindFont(FontName: string = '宋体'; FontSize: Integer = 9; FontStyles: TFontStyles = []): TTextTile;
    function AddFont(FontName: string = '宋体'; FontSize: Integer = 9; FontStyles: TFontStyles = []): TTextTile;
  end;

  TTextureFontManage = class
  private
    FList: TList;
    FCriticalSection: TRTLCriticalSection;
  public

    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    procedure Initialize;
    procedure Finalize;
    procedure FreeOldMemorys;
    function FindFont(FontName: string = '宋体'; FontSize: Integer = 9; FontStyles: TFontStyles = []): TTextTile;
  end;

  THGECanvas = class
  private
    FHGE: THGE;
    FWidth, FHeight: Single;
    FTexWidth, FTexHeight: Integer;
    FActive: Boolean;
    FOnInitialize: TNotifyEvent;
    FOnFinalize: TNotifyEvent;
    FZBuffer: Single;



    FUseSound: Boolean;
    FWindowed: Boolean;
    FDepthStencil: Boolean;
    FHardware: Boolean;
    FScreenWidth: Integer;
    FScreenHeight: Integer;
    FBitCount: Integer;
    FHandle: Hwnd;
    FLogFileName: string;
    FVSync: Boolean;

    FMaxFontSize: Integer;
    procedure SetColor(Color: Cardinal); overload;
    procedure SetColor(Color1, Color2, Color3, Color4: Cardinal); overload;
    procedure SetPattern(Texture: TTexture; PatternIndex: Integer); overload;
    procedure SetPattern(Texture: TTexture; ClientRect: TRect); overload;
    procedure SetPattern(Texture: TTexture); overload;
    procedure SetMirror(MirrorX, MirrorY: Boolean);

    procedure SetUseSound(Value: Boolean);
    procedure SetWindowed(Value: Boolean);
    procedure SetDepthStencil(Value: Boolean);
    procedure SetZBuffer(Value: Single);
    procedure SetWidth(Value: Integer);
    procedure SetHeight(Value: Integer);
    procedure SetBitCount(Value: Integer);
    procedure SetHandle(Value: Hwnd);
    procedure SetLogFileName(Value: string);

    procedure SetHardware(Value: Boolean);
    procedure SetVSync(Value: Boolean);
    function GetHardware: Boolean;
    function GetUseSound: Boolean;
    function GetWindowed: Boolean;
    function GetDepthStencil: Boolean;
    function GetVSync: Boolean;
    function GetZBuffer: Single;
    function GetWidth: Integer;
    function GetHeight: Integer;
    function GetBitCount: Integer;
    function GetHandle: Hwnd;
    function GetLogFileName: string;

    function GetClipRect: TRect;
    procedure SetClipRect(Value: TRect);
  protected
    FQuad: THGEQuad;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Resize(const AWidth, AHeight: Integer);
    function Initialize: Boolean;
    procedure Finalize;
    procedure Render(Handler: TNotifyEvent; Background: Cardinal = 0; FillBk: Boolean = True; Target: TTarget = nil);

    procedure Draw(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;
    procedure Draw(X, Y: Integer; SrcRect: TRect; Texture: TTexture; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;
    procedure Draw(X, Y: Integer; Texture: TTexture; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;

    procedure DrawColor(X, Y: Integer; Texture: TTexture; Color: TColor; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;
    procedure DrawColor(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color: TColor; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;


    procedure StretchDraw(const DestRect, SrcRect: TRect; Texture: TTexture;
      BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;

    procedure StretchDraw(const DestRect: TRect; Texture: TTexture;
      BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;


    procedure DrawAlpha(X, Y: Integer; SrcRect: TRect; Texture: TTexture;
      Alpha: Byte; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;

    procedure DrawAlpha(X, Y: Integer; Texture: TTexture;
      Alpha: Byte; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;


    procedure DrawColorAlpha(const X, Y: Integer; Texture: TTexture; Color: TColor;
      Alpha: Byte; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;

    procedure DrawColorAlpha(const X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color: TColor;
      Alpha: Byte; BlendMode: Integer = Blend_Default; Z: Single = 0.0); overload;

    procedure DrawBlend(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Z: Single = 0.0); overload;
    procedure DrawBlend(X, Y: Integer; Texture: TTexture; Z: Single = 0.0); overload;

    procedure FrameRect(const Rect: TRect; const Color: TColor; Z: Single = 0.0; BlendMode: Integer = Blend_Default);
    procedure FillRect(const Rect: TRect; Color: TColor; Z: Single = 0.0; BlendMode: Integer = Blend_Default); overload;
    procedure FillRect(const Rect: TRect; Color1, Color2, Color3, Color4: Cardinal; Filled: Boolean = True; Z: Single = 0.0; BlendMode: Integer = Blend_Default); overload;
    procedure FillRectAlpha(const DestRect: TRect; Color: TColor; Alpha: Integer; Z: Single = 0.0; BlendMode: Integer = Blend_Default);
    procedure Line(Pt1, Pt2: TPoint; Color: TColor; Z: Single = 0.0; BlendMode: Integer = Blend_Default);
    procedure FillTri(const p1, p2, p3: TPoint; c1, c2, c3: TColor;
      Z: Single = 0.0; BlendMode: Integer = Blend_Default); //画三角形
    procedure Circle(X, Y, Radius: Single; Color: TColor; Filled: Boolean = False; Z: Single = 0.0; BlendMode: Integer = Blend_Default);
    //------------------------------------------------------------------------------------------------------------------------
    procedure Draw(Image: TTexture; PatternIndex: Integer; X, Y: Single; BlendMode: Integer); overload;
    procedure Draw(Image: TTexture; PatternIndex: Integer; X, Y, Z: Single; BlendMode: Integer); overload;
    procedure DrawEx(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY: Single;
      MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer); overload;
    procedure DrawEx(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY: Single;
      MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawEx(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
      DoCenter: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
      DoCenter, MirrorX, MirrorY: Boolean; Red, Green, Blue, Alpha: Byte; BlendMode: Integer); overload;
    procedure DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
      DoCenter: Boolean; Red, Green, Blue, Alpha: Byte; BlendMode: Integer); overload;
    procedure DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y: Single;
      Red, Green, Blue, Alpha: Byte; BlendMode: Integer); overload;
    procedure DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
      DoCenter, MirrorX, MirrorY: Boolean; Alpha: Byte; BlendMode: Integer); overload;
    procedure DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
      DoCenter: Boolean; Alpha: Byte; BlendMode: Integer); overload;
    procedure DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y: Single;
      Alpha: Byte; Blendmode: Integer); overload;
    procedure DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
      DoCenter, MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer); overload;
    procedure DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
      DoCenter: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer); overload;
    procedure DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y: Single;
      Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer); overload;
    procedure DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
      DoCenter, MirrorX, MirrorY: Boolean; Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer); overload;
    procedure DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
      DoCenter: Boolean; Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer); overload;
    procedure DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y: Single;
      Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer); overload;
    procedure Draw4V(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4: Single;
      MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure Draw4V(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4: Single;
      MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer); overload;
    procedure DrawStretch(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2: Single;
      MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
    procedure DrawPart(Texture: TTexture; X, Y, SrcX, SrcY, Width, Height,
      ScaleX, ScaleY, CenterX, CenterY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawPart(Texture: TTexture; X, Y, SrcX, SrcY, Width, Height: Single;
      Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawRotate(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
      Angle, ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawRotate(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
      Angle: Real; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawRotateColor4(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
      Angle, ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
    procedure DrawRotateC(Image: TTexture; PatternIndex: Integer; X, Y, Angle,
      ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawRotateC(Image: TTexture; PatternIndex: Integer; X, Y, Angle: Single;
      Color: Cardinal; BlendMode: Integer); overload;
    procedure DrawWaveX(Image: TTexture; X, Y, Width, Height: Integer; Amp, Len,
      Phase: Integer; Color: Cardinal; BlendMode: Integer);
    procedure DrawWaveY(Image: TTexture; X, Y, Width, Height: Integer; Amp, Len,
      Phase: Integer; Color: Cardinal; BlendMode: Integer);

    property HGE: THGE read FHGE;

    property Active: Boolean read FActive;
    property UseSound: Boolean read FUseSound write FUseSound;
    property Windowed: Boolean read FWindowed write FWindowed;
    property Hardware: Boolean read FHardware write FHardware;
    property DepthStencil: Boolean read FDepthStencil write FDepthStencil;
    property VSync: Boolean read FVSync write FVSync;
    property ZBuffer: Single read FZBuffer write SetZBuffer;

    property MaxFontSize: Integer read FMaxFontSize write FMaxFontSize;
    property Width: Integer read FScreenWidth write FScreenWidth;
    property Height: Integer read FScreenHeight write FScreenHeight;
    property BitCount: Integer read FBitCount write FBitCount;
    property Handle: HWnd read FHandle write FHandle;
    property LogFileName: string read FLogFileName write FLogFileName;

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

    property ClipRect: TRect read GetClipRect write SetClipRect;
    property OnInitialize: TNotifyEvent read FOnInitialize write FOnInitialize;
    property OnFinalize: TNotifyEvent read FOnFinalize write FOnFinalize;
  end;

var
  GameCanvas: THGECanvas = nil;
  TextureFonts: TTextureFontManage;

  CurrentFonts: TTextureFonts;
  CurrentUnderlineFonts: TTextureFonts;
  CurrentBoldFonts: TTextureFonts;

  CurrentFont: TTextTile = nil;
  CurrentUnderlineFont: TTextTile = nil;
  CurrentBoldFont: TTextTile = nil;
implementation
uses HGEDef;

//------------------------------------------------------------------------------

constructor TTextTile.Create(AOwner: TTextureImages; ATextCount: Integer);
begin
  inherited Create(AOwner);
  FFont := TFont.Create;
  FFont.Name := '宋体';
  FFont.Size := 9;
  FFont.Charset := GB2312_CHARSET;
  FFont.Style := [];
  FFontWidth := 6;
  FFontHeight := 12;
  FFontWidthBold := 7;

  FDoubleFontWidth := 12;
  FDoubleFontHeight := 12;
  FDoubleFontWidthBold := 13;
  FProcIdx := 94;
  FList := THashTable.Create(ATextCount);
  m_dwFreeMemCheckTime := 1000 * 60;
end;

procedure TTextTile.SetFont(Value: TFont);
begin
  FFont.Assign(Value);
end;

destructor TTextTile.Destroy;
begin
  FList.Free;
  FFont.Free;
  inherited;
end;

procedure TTextTile.Initialize;
begin
  Lock;
  try
    inherited;
    InitEnglishString;
  finally
    UnLock;
  end;
end;

procedure TTextTile.Finalize;
begin
  Lock;
  try
    FProcIdx := 94;
    FList.Clear;
    inherited;
  finally
    UnLock;
  end;
end;

procedure TTextTile.FreeOldMemorys;
var
  nIdx: Integer;
  ImageRect: pTImageRect;
  dwTimeTick: longword;
  boCheckTimeLimit: Boolean;
begin
  nIdx := FProcIdx;
  boCheckTimeLimit := False;
  dwTimeTick := GetTickCount;
  while True do begin
    if (Count <= nIdx) or (Count <= 94) then Break;
    ImageRect := Images[nIdx];
    if (ImageRect <> nil) and ImageRect.Active then begin
      if GetTickCount - ImageRect.Time > m_dwFreeMemCheckTime then begin
        Lock;
        try
          ImageRect.Active := False;
          List.Remove(ImageRect.S);
        finally
          UnLock;
        end;
      end;
    end;
    Inc(nIdx);
    if (GetTickCount - dwTimeTick) > 10 then begin
      boCheckTimeLimit := True;
      FProcIdx := nIdx;
      Break;
    end;
  end;
  if not boCheckTimeLimit then FProcIdx := 94;
end;

procedure TTextTile.GetFontSize;
var
  //TextMetric: TTextMetric;
  BitmapInfo: TBitmapInfo;
  HHBitmap: HBitmap;
  HHDC: HDC;
  TextSize: TSize;
  OldStyle: TFontStyles;
begin
  OldStyle := FFont.Style;
  FFont.Style := [];
  //DebugOutStr('FFont Size:' + IntToStr(FFont.Size));
  HHDC := CreateCompatibleDC(0);

  SelectObject(HHDC, FFont.Handle);

  Windows.GetTextExtentPoint32W(HHDC, '0', 1, TextSize);
  FFontWidth := abs(TextSize.cx);
  FFontHeight := abs(TextSize.cy);

  //DebugOutStr(Format('FFontWidth:%d FFontHeight:%d', [FFontWidth, FFontHeight]));

  Windows.GetTextExtentPoint32W(HHDC, '一', 1, TextSize);
  FDoubleFontWidth := abs(TextSize.cx);
  FDoubleFontHeight := abs(TextSize.cy);

  //DebugOutStr(Format('FDoubleFontWidth:%d FDoubleFontHeight:%d', [FDoubleFontWidth, FDoubleFontHeight]));

  DeleteDC(HHDC);

  FFont.Style := [fsBold];
  HHDC := CreateCompatibleDC(0);

  SelectObject(HHDC, FFont.Handle);

  Windows.GetTextExtentPoint32W(HHDC, '0', 1, TextSize);
  FFontWidthBold := abs(TextSize.cx);

  Windows.GetTextExtentPoint32W(HHDC, '一', 1, TextSize);
  FDoubleFontWidthBold := abs(TextSize.cx);
  FFont.Style := OldStyle;


{  FFontWidth := 6;
  FFontHeight := 12;
  FFontWidthBold := 7;

  FDoubleFontWidth := 12;
  FDoubleFontHeight := 12;
  FDoubleFontWidthBold := 13; }

  //DebugOutStr(Format('(1) FFontWidth:%d, FFontHeight:%d, FFontWidthBold:%d, FDoubleFontWidth:%d, FDoubleFontHeight:%d, FDoubleFontWidthBold:%d',
  //[FFontWidth, FFontHeight, FFontWidthBold, FDoubleFontWidth, FDoubleFontHeight, FDoubleFontWidthBold]));
end;

function TTextTile.TextHeight(const Text: string): Integer;
var
  sText: WideString;
begin
  sText := Text;
  if Length(Text) = Length(sText) then begin
    Result := FFontHeight;
  end else begin
    Result := Max(FFontHeight, FDoubleFontHeight);
  end;
  //DebugOutStr('Result TextHeight:' + IntToStr(Result));
end;


function TTextTile.TextWidth(const Text: string): Integer;
var
  nCount, nsCount, nwCount: Integer;
  sText: WideString;
begin
  sText := Text;
  nsCount := Length(Text);
  nwCount := Length(sText);
  if nsCount = nwCount then begin
    if fsBold in FFont.Style then
      Result := FFontWidthBold * Length(Text)
    else
      Result := FFontWidth * Length(Text);
  end else begin
    nCount := nsCount - nwCount; //双字节字符数
    if fsBold in FFont.Style then
      Result := FDoubleFontWidthBold * Max(nCount, 0) + Max((nwCount - nCount), 0) * FFontWidthBold
    else
      Result := FDoubleFontWidth * Max(nCount, 0) + Max((nwCount - nCount), 0) * FFontWidth;
  end;
  //DebugOutStr('Result TextWidth:' + IntToStr(Result));
end;

function TTextTile.Add(const Text: string): pTImageRect;
var
  nWidth, nHeight, X, Y: Integer;

  PBitmapBits: Pointer; // PIntegerArray;

  BitmapInfo: TBitmapInfo;
  HHBitmap: HBitmap;
  HHDC: HDC;


  Bits: Pointer;
  Pitch: Integer;

  SrcP: PByte;
  DesP: PByte;
  Pix: Cardinal;


  FileHeader: PBitmapFileHeader;
  InfoHeader: PBitmapInfoHeader;

  DestRect, SrcRect: TRect;

  ImageRect: pTImageRect;

begin
  Result := nil;
  if not Initialized then Exit;
  //if Text = '' then Exit;

  ImageRect := GetActive;
  if ImageRect <> nil then begin
    //DebugOutStr(Format('(1) Left:%d, Top:%d, AWidth:%d, AHeight:%d', [ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]));
    ImageRect.S := Text;
    nWidth := TextWidth(Text);
    nHeight := PatternHeight;
    //DebugOutStr(Format('AWidth:%d, AHeight:%d', [nWidth, nHeight]));
    FillChar(BitmapInfo, SizeOf(BitmapInfo), 0);

    with BitmapInfo.bmiHeader do begin
  //位图信息头
      biSize := SizeOf(TBitmapInfoHeader);
      biWidth := nWidth;
      biHeight := -nHeight;
      biPlanes := 1;
      biBitCount := 32;
      biCompression := BI_RGB;
    end;

    HHDC := CreateCompatibleDC(0);
    PBitmapBits := nil;
    HHBitmap := CreateDIBSection(HHDC, BitmapInfo, DIB_RGB_COLORS, PBitmapBits, 0, 0);
    if (HHBitmap <> 0) and (PBitmapBits <> nil) then begin
      //DebugOutStr(Format('(2) Left:%d, Top:%d, AWidth:%d, AHeight:%d', [ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]));
      SelectObject(HHDC, FFont.Handle);
      SelectObject(HHDC, HHBitmap);
      SetTextColor(HHDC, RGB(255, 255, 255)); //设文字颜色为白色
      SetBkColor(HHDC, RGB(0, 0, 0)); //设背景颜色为黑色
      Windows.TextOut(HHDC, 0, 0, PChar(Text), Length(Text));
      SrcRect := ImageRect.Rect;

      nWidth := Min(SrcRect.Right - SrcRect.Left, nWidth);
      nHeight := Min(SrcRect.Bottom - SrcRect.Top, nHeight);

      SrcRect := Bounds(SrcRect.Left, SrcRect.Top, nWidth, nHeight);
      ImageRect.Rect := SrcRect;
      //GameCanvas.HGE.Lock;
      //try
      if Owner.Texture.Lock(SrcRect, Bits, Pitch, False) then begin
        try
          if (Bits <> nil) and (Pitch > 0) then begin
            for Y := 0 to nHeight - 1 do begin
              SrcP := PByte(Integer(PBitmapBits) + Y * nWidth * 4);
              DesP := PByte(Integer(Bits) + Y * Pitch);
            //FillChar(DesP^, Pitch, 0);
              for X := 0 to nWidth - 1 do begin
                if PCardinal(SrcP)^ <> 0 then
                  PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000 //$FF000000;
                else
                  PCardinal(DesP)^ := $00000000;

                Inc(SrcP, 4);
                Inc(DesP, 4);
              end;
            end;
         // DebugOutStr(Format('(3)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));

            FList.Add(Text, Text, ImageRect);
          end else begin
            ImageRect.Active := False;
          end;
        finally
          Owner.Texture.Unlock;
        //GameCanvas.HGE.UnLock;
        end;
      end else begin
        ImageRect.Active := False;
        //DebugOutStr(Format('(4)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
      end;
      //finally
        //Owner.Texture.Unlock;
        //GameCanvas.HGE.UnLock;
      //end;
      DeleteObject(HHBitmap);
    end else ImageRect.Active := False;
    DeleteDC(HHDC);
  end;
  if (ImageRect <> nil) and not ImageRect.Active then
    ImageRect := nil;
  Result := ImageRect;
end;

procedure TTextTile.InitEnglishString;
var
  I: Integer;
begin
  for I := 0 to 9 do //0~9
    Add(IntToStr(I));

  for I := 97 to 122 do //a~z      //10~25
    Add(Chr(I));

  for I := 65 to 90 do //A~Z     //26~51
    Add(Chr(I));

  Add('~'); //52
  Add('!'); //52
  Add('@'); //52
  Add('#'); //52
  Add('$'); //52
  Add('%'); //52
  Add('^');
  Add('&');
  Add('^');
  Add('&');
  Add('*');
  Add('(');
  Add(')');
  Add('_');
  Add('+');
  Add('-');
  Add('=');
  Add('{');
  Add('}');
  Add('[');
  Add(']');
  Add('\');
  Add('|');
  Add('<');
  Add('>');
  Add('/');
  Add('?');
  Add(':');
  Add('.');
  Add(',');
  Add(';');
  Add(' ');
end;

function TTextTile.GetImageInfos(const Text: string): TImageInfos;
var
  I: Integer;
  TextList: TStringList;
begin
  SetLength(Result, 0);
  if not Initialized then Exit;
  TextList := TStringList.Create;
  TextList.Text := Text;
  for I := 0 to TextList.Count - 1 do begin
    SetLength(Result, Length(Result) + 1);
    Result[Length(Result) - 1] := GetImageInfo(TextList.Strings[I]);
  end;
  TextList.Free;
end;

function TTextTile.GetImageInfo(const Text: string): TImageInfo;
var
  I: Integer;
  ImageRect: pTImageRect;
  sText: WideString;
  S: string;
  //ImageInfo:TImageInfo;
begin
  Result.Width := 0;
  Result.Height := 0;
  SetLength(Result.ImageIndexs, 0);
  if not Initialized then Exit;
  Lock;
  try
  //if Text = '' then Exit;
    if Length(Text) > 0 then begin
      sText := Text;
      for I := 1 to Length(sText) do begin
        S := sText[I];
        ImageRect := FList.Datas[S];

        if ImageRect <> nil then begin
          ImageRect.Time := GetTickCount;
          Result.Width := Result.Width + (ImageRect.Rect.Right - ImageRect.Rect.Left);
          Result.Height := Max(Result.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
          SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
          Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := ImageRect.Index;
        end else begin
          ImageRect := Add(S);
          if ImageRect <> nil then begin
            Result.Width := Result.Width + (ImageRect.Rect.Right - ImageRect.Rect.Left);
            Result.Height := Max(Result.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
            SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
            Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := ImageRect.Index;
          end else begin
            Result.Width := Result.Width + TextWidth('0');
       // Result.Height := Max(Result.Height, TextHeight('0'));
            SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
            Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := -1;
          end;
        end;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TTextTile.TextRect(const X, Y: Integer; SrcRect: TRect; const Text: string; Color: TColor);
var
  ImageInfos: TImageInfos;
begin
  if not Initialized then Exit;
  ImageInfos := GetImageInfos(Text);
  TextRect(X, Y, SrcRect, ImageInfos, Color);
end;

  //IntersectRect

procedure TTextTile.TextRect(const X, Y: Integer; SrcRect: TRect; ImageInfos: TImageInfos; Color: TColor);
var
  I, II: Integer;
  nLeft, nTop, nX, nY, nOffsetX, nOffsetY, nWidth, nHeight, nH: Integer;
  ImageRect: pTImageRect;
  PaintRect: TRect;
  DestRect: TRect;
begin
  if not Initialized then Exit;
  if (Owner.Texture <> nil) and (SrcRect.Right > SrcRect.Left) and (SrcRect.Bottom > SrcRect.Top) then begin
    nY := 0;
    nOffsetY := SrcRect.Top;
    nHeight := SrcRect.Bottom - SrcRect.Top;
    for I := 0 to Length(ImageInfos) - 1 do begin
      if nHeight <= 0 then break;
      if ImageInfos[I].Height <= 0 then begin
        if nOffsetY >= TextHeight('0') then begin
          Dec(nOffsetY, TextHeight('0'));
          Continue;
        end;

        if nOffsetY > 0 then begin
          Inc(nY, TextHeight('0') - nOffsetY);
          Dec(nHeight, TextHeight('0') - nOffsetY);
          nOffsetY := 0;
          Continue;
        end;

        Inc(nY, TextHeight('0'));
        Dec(nHeight, TextHeight('0'));
        Continue;
      end;

      if (nOffsetY > 0) and (nOffsetY >= ImageInfos[I].Height) then begin
        Dec(nOffsetY, ImageInfos[I].Height);
        Continue;
      end;

      nX := 0;
      nWidth := SrcRect.Right - SrcRect.Left;
      nOffsetX := SrcRect.Left;
      nH := ImageInfos[I].Height - nOffsetY;

      for II := 0 to Length(ImageInfos[I].ImageIndexs) - 1 do begin
        if nWidth <= 0 then break;
        ImageRect := Images[ImageInfos[I].ImageIndexs[II]];
        if (ImageRect <> nil) and (ImageRect.Rect.Right > ImageRect.Rect.Left) then begin
          if (nOffsetX > 0) then begin
            if (nOffsetX >= ImageRect.Rect.Right - ImageRect.Rect.Left) then begin
              nOffsetX := nOffsetX - (ImageRect.Rect.Right - ImageRect.Rect.Left);
              Continue;
            end;
          end;
          ImageRect.Time := GetTickCount;

          PaintRect := Bounds(ImageRect.Rect.Left + nOffsetX, ImageRect.Rect.Top + nOffsetY, Min(ImageRect.Rect.Right - ImageRect.Rect.Left - nOffsetX, nWidth), Min(ImageRect.Rect.Bottom - ImageRect.Rect.Top - nOffsetY, nHeight));
          if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then
            GameCanvas.DrawColor(X + nX, Y + nY, PaintRect, Owner.Texture, Color);

          Inc(nX, PaintRect.Right - PaintRect.Left);
          Dec(nWidth, PaintRect.Right - PaintRect.Left);
          nH := Min(nH, PaintRect.Bottom - PaintRect.Top);
          nOffsetX := 0;

        end else begin
          if (nOffsetX > 0) then begin
            if nOffsetX >= TextWidth('0') then begin
              nOffsetX := nOffsetX - TextWidth('0');
              Continue;
            end;
            Inc(nX, TextWidth('0') - nOffsetX);
            Dec(nWidth, TextWidth('0') - nOffsetX);
            nOffsetX := 0;
            Continue;
          end;
          Inc(nX, TextWidth('0'));
          Dec(nWidth, TextWidth('0'));
        end;
      end; //for II := 0 to Length(ImageInfos[I].ImageIndexs) - 1 do begin
      Inc(nY, nH);
      Dec(nHeight, nH);
      nOffsetY := 0;
    end;
  end;
end;

procedure TTextTile.TextRect(const X, Y: Integer; SrcRect: TRect; const ImageIndexs: TImageIndexs; Color: TColor);
var
  I, II: Integer;
  nLeft, nTop, nX, nOffsetX, nOffsetY, nWidth, nHeight, nH: Integer;
  ImageRect: pTImageRect;
  PaintRect: TRect;
  DestRect: TRect;
begin
  if not Initialized then Exit;
  if (Owner.Texture <> nil) and (SrcRect.Right > SrcRect.Left) and (SrcRect.Bottom > SrcRect.Top) then begin
    nX := 0;

    nOffsetY := SrcRect.Top;
    nHeight := SrcRect.Bottom - SrcRect.Top;

    nWidth := SrcRect.Right - SrcRect.Left;
    nOffsetX := SrcRect.Left;

    for I := 0 to Length(ImageIndexs) - 1 do begin
      if nWidth <= 0 then break;
      ImageRect := Images[ImageIndexs[I]];
      if (ImageRect <> nil) and (ImageRect.Rect.Right > ImageRect.Rect.Left) then begin
        if (nOffsetX > 0) then begin
          if (nOffsetX >= ImageRect.Rect.Right - ImageRect.Rect.Left) then begin
            nOffsetX := nOffsetX - (ImageRect.Rect.Right - ImageRect.Rect.Left);
            Continue;
          end;
        end;
        ImageRect.Time := GetTickCount;

        PaintRect := Bounds(ImageRect.Rect.Left + nOffsetX, ImageRect.Rect.Top + nOffsetY, Min(ImageRect.Rect.Right - ImageRect.Rect.Left - nOffsetX, nWidth), Min(ImageRect.Rect.Bottom - ImageRect.Rect.Top - nOffsetY, nHeight));

        if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
          GameCanvas.DrawColor(X + nX, Y, PaintRect, Owner.Texture, Color);

          Inc(nX, PaintRect.Right - PaintRect.Left);
          Dec(nWidth, PaintRect.Right - PaintRect.Left);
        end;

        nOffsetX := 0;

      end else begin
        if (nOffsetX > 0) then begin
          if nOffsetX >= TextWidth('0') then begin
            nOffsetX := nOffsetX - TextWidth('0');
            Continue;
          end;
          Inc(nX, TextWidth('0') - nOffsetX);
          Dec(nWidth, TextWidth('0') - nOffsetX);
          nOffsetX := 0;
          Continue;
        end;
        Inc(nX, TextWidth('0'));
        Dec(nWidth, TextWidth('0'));
      end;
    end;
  end;
end;

procedure TTextTile.TextOut(const X, Y: Integer; const Text: string; Color: TColor);
var
  I, nWidth, nHeight: Integer;
  ImageInfos: TImageInfos;
begin
  if not Initialized then Exit;
  nWidth := 0;
  nHeight := 0;
  ImageInfos := GetImageInfos(Text);
  for I := 0 to Length(ImageInfos) - 1 do begin
    if nWidth < ImageInfos[I].Width then
      nWidth := ImageInfos[I].Width;

    if ImageInfos[I].Height <= 0 then
      Inc(nHeight, TextHeight('0'))
    else
      Inc(nHeight, ImageInfos[I].Height);
  end;
  if (nWidth > 0) and (nHeight > 0) then
    TextRect(X, Y, Rect(0, 0, nWidth, nHeight), ImageInfos, Color);
end;

procedure TTextTile.TextOut(const X, Y: Integer; ImageIndexs: TImageIndexs; Color: TColor);
var
  I, nX, nIndex: Integer;
  ImageRect: pTImageRect;
begin
  if not Initialized then Exit;
  if Owner.Texture <> nil then begin
    nX := X;
    for I := 0 to Length(ImageIndexs) - 1 do begin
      nIndex := ImageIndexs[I];
      ImageRect := Images[nIndex];
      if (ImageRect <> nil) then begin
        ImageRect.Time := GetTickCount;
        if (ImageRect.Rect.Right > ImageRect.Rect.Left) and (ImageRect.Rect.Bottom > ImageRect.Rect.Top) then begin
          GameCanvas.DrawColor(nX, Y, ImageRect.Rect, Owner.Texture, Color);
          Inc(nX, ImageRect.Rect.Right - ImageRect.Rect.Left);
        end else begin
          Inc(nX, TextWidth('0'));
        end;
      end else begin
        Inc(nX, TextWidth('0'));
      end;
    end;
  end;
end;
//------------------------------------------------------------------------------

function TTextureFonts.FindFont(FontName: string; FontSize: Integer; FontStyles: TFontStyles): TTextTile;
var
  I: Integer;
  TextTile: TTextTile;
  boFind: Boolean;
begin
  ListLock;
  try
    boFind := False;
    if FontName = '' then FontName := '宋体';
    for I := 0 to Count - 1 do begin
      TextTile := TTextTile(Tiles[I]);
      if (CompareText(TextTile.Font.Name, FontName) = 0) and
        (TextTile.Font.Size = FontSize) and
        (TextTile.Font.Style = FontStyles) then begin
        Result := TextTile;
        boFind := True;
        break;
      end;
    end;
    if not boFind then
      Result := AddFont(FontName, FontSize, FontStyles);
  finally
    ListUnLock;
  end;
end;

function TTextureFonts.AddFont(FontName: string; FontSize: Integer; FontStyles: TFontStyles): TTextTile;
var
  TextTile: TTextTile;
  nStringCount: Integer;
begin
  if FontName = '' then FontName := '宋体';
  if CurrentFonts = Self then //12*12    1024*512
    //nStringCount := 3570
    nStringCount := 1764
  else
    if CurrentUnderlineFonts = Self then //12*12  512*512
    nStringCount := 1764
  else
    if CurrentBoldFonts = Self then //14*13 512*512
    nStringCount := 1404
  else
    nStringCount := 300;

  TextTile := TTextTile.Create(Self, nStringCount);
  TextTile.Font.Name := FontName;
  TextTile.Font.Size := FontSize;
  TextTile.Font.Style := FontStyles;
  TextTile.GetFontSize;

  TextTile.Left := Left;
  TextTile.Top := Top;
  TextTile.PatternWidth := TextTile.TextWidth('一');
  TextTile.PatternHeight := TextTile.TextHeight('一');
  TextTile.Width := Width;
  TextTile.Height := TextTile.PatternWidth * TextTile.PatternHeight * nStringCount div Width + TextTile.PatternHeight;

  TextTile.Initialize;
  Add(TextTile);
  Result := TextTile;
end;

procedure TTextureFonts.FreeOldMemorys;
var
  I: Integer;
  TextTile: TTextTile;
begin
  //if GetTickCount - FreeMemCheckTick > 5000 then begin
    //FreeMemCheckTick := GetTickCount;
  ListLock;
  try
    for I := 0 to Count - 1 do begin
      TextTile := TTextTile(Tiles[I]);
      TextTile.FreeOldMemorys;
    end;
  finally
    ListUnLock;
  end;
  //end;
end;

constructor TTextureFontManage.Create();
begin
  InitializeCriticalSection(FCriticalSection);
  FList := TList.Create;
end;

destructor TTextureFontManage.Destroy;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
    TTextureFonts(FList.Items[I]).Free;
  FList.Free;
  DeleteCriticalSection(FCriticalSection);
end;

procedure TTextureFontManage.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TTextureFontManage.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TTextureFontManage.Initialize;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      TTextureFonts(FList.Items[I]).Initialize;
  finally
    UnLock;
  end;
end;

procedure TTextureFontManage.Finalize;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      TTextureFonts(FList.Items[I]).Finalize;
  finally
    UnLock;
  end;
end;

procedure TTextureFontManage.FreeOldMemorys;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      TTextureFonts(FList.Items[I]).FreeOldMemorys;
  finally
    UnLock;
  end;
  CurrentFonts.FreeOldMemorys;
  CurrentUnderlineFonts.FreeOldMemorys;
  CurrentBoldFonts.FreeOldMemorys;
end;

function TTextureFontManage.FindFont(FontName: string; FontSize: Integer; FontStyles: TFontStyles): TTextTile;
var
  I, II, nWidth, nHeight: Integer;
  nColCount, nRowCount, nRowSize: Integer;
  TextTile: TTextTile;
  NewTextureFonts: TTextureFonts;
  S: string;
  boFind: Boolean;
begin
  Result := nil;
  if FontName = '' then FontName := '宋体';
  if (CompareText(CurrentFont.Font.Name, FontName) = 0) and
    (FontSize = CurrentFont.Font.Size)
    and (FontStyles = CurrentFont.Font.Style) then begin
    Result := CurrentFont;
    Exit;
  end;

  if (CompareText(CurrentUnderlineFont.Font.Name, FontName) = 0) and
    (FontSize = CurrentUnderlineFont.Font.Size)
    and (FontStyles = CurrentUnderlineFont.Font.Style) then begin
    Result := CurrentUnderlineFont;
    Exit;
  end;

  if (CompareText(CurrentBoldFont.Font.Name, FontName) = 0) and
    (FontSize = CurrentBoldFont.Font.Size)
    and (FontStyles = CurrentBoldFont.Font.Style) then begin
    Result := CurrentBoldFont;
    Exit;
  end;

  boFind := False;
  Lock;
  try
    for I := 0 to FList.Count - 1 do begin
      NewTextureFonts := TTextureFonts(TTextureFonts(FList.Items[I]));
      for II := 0 to NewTextureFonts.Count - 1 do begin
        TextTile := TTextTile(NewTextureFonts.Tiles[II]);
        if (CompareText(TextTile.Font.Name, FontName) = 0) and
          (TextTile.Font.Size = FontSize) and
          (TextTile.Font.Style = FontStyles) then begin
          Result := TextTile;
          boFind := True;
          break;
        end;
      end;
    end;

    if not boFind then begin
      NewTextureFonts := TTextureFonts.Create;
      FList.Add(NewTextureFonts);

      TextTile := TTextTile.Create(NewTextureFonts, 1);
      TextTile.Font.Name := FontName;
      TextTile.Font.Size := FontSize;
      TextTile.Font.Style := FontStyles;
      TextTile.GetFontSize;

      nWidth := TextTile.TextWidth('一');
      nHeight := TextTile.TextHeight('一');
      TextTile.Free;

      nColCount := 512 div nWidth;
      nRowCount := 300 div nColCount;
      nRowSize := nRowCount * nHeight;
 { S := '';
  if fsBold in FontStyles   then
    S := S +  'fsBold:';
  if fsItalic in FontStyles   then
    S := S +  'fsItalic:';
  if fsUnderline in FontStyles   then
    S := S +  'fsUnderline:';
  if fsStrikeOut in FontStyles   then
    S := S +  'fsStrikeOut:';

  DebugOutStr(Format('FontName:%s FontSize:%d FontStyles:%s AHeight:%d nWidth:%d nHeight:%d', [FontName, FontSize, S, n11, nWidth, nHeight]));
  }

      NewTextureFonts.SetSize(512, nRowSize);
      NewTextureFonts.Initialize;

      Result := NewTextureFonts.FindFont(FontName, FontSize, FontStyles);
    end;
  finally
    UnLock;
  end;
end;
//------------------------------------------------------------------------------

constructor THGECanvas.Create;
begin
  FActive := False;
  FOnInitialize := nil;
  FOnFinalize := nil;
  FZBuffer := 0.0;

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
  FLogFileName := '';
  FHGE := HGECreate(HGE_VERSION);
  TextureFonts := TTextureFontManage.Create;
  CurrentFonts := TTextureFonts.Create;
  CurrentUnderlineFonts := TTextureFonts.Create;
  CurrentBoldFonts := TTextureFonts.Create;
end;

destructor THGECanvas.Destroy;
begin
  TextureFonts.Free;
  CurrentFonts.Free;
  CurrentUnderlineFonts.Free;
  CurrentBoldFonts.Free;
  FHGE.Free;
  inherited;
end;

function THGECanvas.Initialize: Boolean;
var
  D3DCaps8: PD3DCaps8;
  MaxTextureWidth: LongWord;
  MaxTextureHeight: LongWord;
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
  FHGE.System_SetState(HGE_LOGFILE, FLogFileName);

  if FHGE.System_Initiate then begin
    D3DCaps8 := FHGE.D3DCaps8;
    if D3DCaps8 <> nil then begin
      MaxTextureWidth := Min(D3DCaps8.MaxTextureWidth, 2048);
      MaxTextureHeight := Min(D3DCaps8.MaxTextureHeight, 2048);
    end else begin
      MaxTextureWidth := 2048;
      MaxTextureHeight := 2048;
    end;

    //CurrentFonts.SetSize(1024, 512); //1024 * 512 的纹理 可以存放 (1024 /12) * (512 / 12) =  3570 个汉字  宋体 字体大小=9 样式=无  3570个汉字足够使用 不需要定时释放不使用

    CurrentFonts.SetSize(512, 512); //512 * 512 的纹理 可以存放 (512 /12) * (512 / 12) =  1764 个汉字  宋体 字体大小=9 样式=无
    CurrentUnderlineFonts.SetSize(512, 512); //512 * 512 的纹理 可以存放 (512 /12) * (512 / 12) =  1764 个汉字  宋体 字体大小=9 样式=下划线 用于NPC对话框里下划线文字 会定时释放不用的
    CurrentBoldFonts.SetSize(512, 512); //512 * 512 的纹理 可以存放 (512 /12) * (512 / 12) =  1764 个汉字  宋体 字体大小=9 样式=粗体 会定时释放不用的

    CurrentFonts.Initialize;
    CurrentUnderlineFonts.Initialize;
    CurrentBoldFonts.Initialize;

    CurrentFont := CurrentFonts.FindFont('宋体', 9);
    CurrentUnderlineFont := CurrentUnderlineFonts.FindFont('宋体', 9, [fsUnderline]);
    CurrentBoldFont := CurrentBoldFonts.FindFont('宋体', 10, [fsBold]);

    CurrentFont.m_dwFreeMemCheckTime := 1000 * 60 * 5;
    CurrentUnderlineFont.m_dwFreeMemCheckTime := 1000 * 60 * 5;
    CurrentBoldFont.m_dwFreeMemCheckTime := 1000 * 60 * 5;


    TextureFonts.Initialize;
    if Assigned(FOnInitialize) then
      FOnInitialize(Self);
    FActive := True;
    Result := FActive;
  end;
end;

procedure THGECanvas.Finalize;
begin
  if FActive then begin
    FActive := False;
    if Assigned(FOnFinalize) then
      FOnFinalize(Self);

    CurrentFonts.Finalize;
    CurrentUnderlineFonts.Finalize;
    CurrentBoldFonts.Finalize;
    TextureFonts.Finalize;
    FHGE.System_Shutdown;
  end;
end;

procedure THGECanvas.Render(Handler: TNotifyEvent; Background: Cardinal; FillBk: Boolean; Target: TTarget);
begin
  if FActive then begin
    FHGE.Gfx_BeginScene(Target);
    if FillBk then
      FHGE.Gfx_Clear(Background);
    Handler(Self);
    FHGE.Gfx_EndScene;
  end;
end;

procedure THGECanvas.Resize(const AWidth, AHeight: Integer);
begin
  FHGE.Resize(AWidth, AHeight);
end;

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

procedure THGECanvas.SetZBuffer(Value: Single);
begin
  FZBuffer := Value;
end;

procedure THGECanvas.SetWidth(Value: Integer);
begin
  FHGE.System_SetState(HGE_SCREENWIDTH, Value);
end;

procedure THGECanvas.SetHeight(Value: Integer);
begin
  FHGE.System_SetState(HGE_SCREENHEIGHT, Value);
end;

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

function THGECanvas.GetClipRect: TRect;
var
  X, Y, W, H: Integer;
begin
  FHGE.Gfx_GetClipping(X, Y, W, H);
  Result := Rect(X, Y, W, H);
end;

procedure THGECanvas.SetClipRect(Value: TRect);
begin
  FHGE.Gfx_SetClipping(Value.Left, Value.Top, Value.Right - Value.Left, Value.Bottom - Value.Top);
end;

procedure THGECanvas.SetColor(Color: Cardinal);
begin
  FQuad.V[0].Col := Color;
  FQuad.V[1].Col := Color;
  FQuad.V[2].Col := Color;
  FQuad.V[3].Col := Color;
end;

procedure THGECanvas.SetColor(Color1: Cardinal; Color2: Cardinal; Color3: Cardinal; Color4: Cardinal);
begin
  FQuad.V[0].Col := Color1;
  FQuad.V[1].Col := Color2;
  FQuad.V[2].Col := Color3;
  FQuad.V[3].Col := Color4;
end;

procedure THGECanvas.SetPattern(Texture: TTexture);
var
  TexX1, TexY1, TexX2, TexY2: Single;
  Left, Right, Top, Bottom: Integer;
  PHeight, PWidth: Integer;
begin
  if Assigned(Texture) then begin
    FTexWidth := FHGE.Texture_GetWidth(Texture);
    FTexHeight := FHGE.Texture_GetHeight(Texture);
  end else begin
    FTexWidth := 1;
    FTexHeight := 1;
  end;

  FQuad.V[0].TX := 0; FQuad.V[0].TY := 0;
  FQuad.V[1].TX := 0; FQuad.V[1].TY := 0;
  FQuad.V[2].TX := 0; FQuad.V[2].TY := 0;
  FQuad.V[3].TX := 0; FQuad.V[3].TY := 0;
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

  FQuad.V[0].TX := TexX1; FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2; FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2; FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1; FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetPattern(Texture: TTexture; ClientRect: TRect);
var
  TexX1, TexY1, TexX2, TexY2: Single;
  Left, Right, Top, Bottom: Integer;
  PHeight, PWidth: Integer;
  SrcRect: TRect;
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
  FQuad.V[0].TX := 0; FQuad.V[0].TY := 0;
  FQuad.V[1].TX := 0; FQuad.V[1].TY := 0;
  FQuad.V[2].TX := 0; FQuad.V[2].TY := 0;
  FQuad.V[3].TX := 0; FQuad.V[3].TY := 0;
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

  FQuad.V[0].TX := TexX1; FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2; FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2; FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1; FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetPattern(Texture: TTexture; PatternIndex: Integer);
var
  TexX1, TexY1, TexX2, TexY2: Single;
  Left, Right, Top, Bottom: Integer;
  PHeight, PWidth, RowCount, ColCount: Integer;
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

  FQuad.V[0].TX := TexX1; FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2; FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2; FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1; FQuad.V[3].TY := TexY2;
end;

procedure THGECanvas.SetMirror(MirrorX, MirrorY: Boolean);
var
  TX, TY: Single;
begin
  if (MirrorX) then
  begin
    TX := FQuad.V[0].TX; FQuad.V[0].TX := FQuad.V[1].TX; FQuad.V[1].TX := TX;
    TY := FQuad.V[0].TY; FQuad.V[0].TY := FQuad.V[1].TY; FQuad.V[1].TY := TY;
    TX := FQuad.V[3].TX; FQuad.V[3].TX := FQuad.V[2].TX; FQuad.V[2].TX := TX;
    TY := FQuad.V[3].TY; FQuad.V[3].TY := FQuad.V[2].TY; FQuad.V[2].TY := TY;
  end;

  if (MirrorY) then
  begin
    TX := FQuad.V[0].TX; FQuad.V[0].TX := FQuad.V[3].TX; FQuad.V[3].TX := TX;
    TY := FQuad.V[0].TY; FQuad.V[0].TY := FQuad.V[3].TY; FQuad.V[3].TY := TY;
    TX := FQuad.V[1].TX; FQuad.V[1].TX := FQuad.V[2].TX; FQuad.V[2].TX := TX;
    TY := FQuad.V[1].TY; FQuad.V[1].TY := FQuad.V[2].TY; FQuad.V[2].TY := TY;
  end;
end;

procedure THGECanvas.Draw(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer; Z: Single);
var
  TempX1, TempY1, TempX2, TempY2: Single;
begin
  SetPattern(Texture, SrcRect);
  if FQuad.Tex <> Texture then Exit;
  SetColor(Color1, Color2, Color3, Color4);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw(X, Y: Integer; SrcRect: TRect; Texture: TTexture; BlendMode: Integer; Z: Single);
begin
  Draw(X, Y, SrcRect, Texture, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF, BlendMode, Z);
end;

procedure THGECanvas.Draw(X, Y: Integer; Texture: TTexture; BlendMode: Integer; Z: Single);
begin
  Draw(X, Y, Texture.ClientRect, Texture, BlendMode, Z);
end;

procedure THGECanvas.DrawColor(X, Y: Integer; Texture: TTexture; Color: TColor; BlendMode: Integer; Z: Single);
begin
  DrawColor(X, Y, Texture.ClientRect, Texture, Color, BlendMode, Z);
end;

procedure THGECanvas.DrawColor(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color: TColor; BlendMode: Integer; Z: Single);
var
  Color4: Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.StretchDraw(const DestRect, SrcRect: TRect; Texture: TTexture;
  BlendMode: Integer; Z: Single);
var
  TempX1, TempY1, TempX2, TempY2: Single;
  ScaleX, ScaleY: Single;
  PaintRect: TRect;
  SWidth, SHeight: Integer;
  DWidth, DHeight: Integer;
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

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.StretchDraw(const DestRect: TRect; Texture: TTexture;
  BlendMode: Integer; Z: Single);
begin
  StretchDraw(DestRect, Texture.ClientRect, Texture, BlendMode, Z);
end;

procedure THGECanvas.DrawAlpha(X, Y: Integer; Texture: TTexture; Alpha: Byte;
  BlendMode: Integer; Z: Single);
begin
  DrawAlpha(X, Y, Texture.ClientRect, Texture, Alpha, BlendMode, Z);
end;

procedure THGECanvas.DrawAlpha(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Alpha: Byte;
  BlendMode: Integer; Z: Single);
var
  Color4: Cardinal;
begin
  Color4 := ARGB(Alpha, 255, 255, 255);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawColorAlpha(const X, Y: Integer; Texture: TTexture; Color: TColor;
  Alpha: Byte; BlendMode: Integer; Z: Single);
var
  Color4: Cardinal;
  Red, Green, Blue: Byte;
begin
  Red := Byte(Color);
  Green := Byte(Color shr 8);
  Blue := Byte(Color shr 16);
  Color4 := ARGB(Alpha, Red, Green, Blue);
  Draw(X, Y, Texture.ClientRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawColorAlpha(const X, Y: Integer; SrcRect: TRect; Texture: TTexture; Color: TColor;
  Alpha: Byte; BlendMode: Integer; Z: Single);
var
  Color4: Cardinal;
  Red, Green, Blue: Byte;
begin
  Red := Byte(Color);
  Green := Byte(Color shr 8);
  Blue := Byte(Color shr 16);
  Color4 := ARGB(Alpha, Red, Green, Blue);
  Draw(X, Y, SrcRect, Texture, Color4, Color4, Color4, Color4, BlendMode, Z);
end;

procedure THGECanvas.DrawBlend(X, Y: Integer; SrcRect: TRect; Texture: TTexture; Z: Single);
begin
  Draw(X, Y, SrcRect, Texture, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.DrawBlend(X, Y: Integer; Texture: TTexture; Z: Single);
begin
  Draw(X, Y, Texture.ClientRect, Texture, Blend_SrcAlphaColor, Z);
end;

procedure THGECanvas.FrameRect(const Rect: TRect; const Color: TColor; Z: Single; BlendMode: Integer);
var
  Color4: Cardinal;
begin
  if (Rect.Right - Rect.Left > 0) and (Rect.Bottom - Rect.Top > 0) then begin
    Color4 := DisplaceRB(Color) or $FF000000;
    HGE.Rectangle(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, Color4, False, Z, BlendMode);
  end;
end;

procedure THGECanvas.FillRect(const Rect: TRect; Color: TColor; Z: Single; BlendMode: Integer);
var
  Color4: Cardinal;
begin
  if (Rect.Right - Rect.Left > 0) and (Rect.Bottom - Rect.Top > 0) then begin
    Color4 := DisplaceRB(Color) or $FF000000;
    HGE.Rectangle(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, Color4, True, Z, BlendMode);
  end;
end;

procedure THGECanvas.FillRect(const Rect: TRect; Color1, Color2, Color3, Color4: Cardinal; Filled: Boolean; Z: Single; BlendMode: Integer);
begin
  if (Rect.Right - Rect.Left > 0) and (Rect.Bottom - Rect.Top > 0) then begin
    HGE.Quadrangle4Color(Rect.Left, Rect.Top, Rect.Right, Rect.Top, Rect.Right, Rect.Bottom,
      Rect.Left, Rect.Bottom, Color1, Color2, Color3, Color4, Filled, Z, BlendMode);
  end;
end;



procedure THGECanvas.FillRectAlpha(const DestRect: TRect; Color: TColor; Alpha: Integer; Z: Single; BlendMode: Integer);
var
  Color4: Cardinal;
  Red, Green, Blue: Byte;
begin
  if (DestRect.Right - DestRect.Left > 0) and (DestRect.Bottom - DestRect.Top > 0) then begin
    Red := Byte(Color);
    Green := Byte(Color shr 8);
    Blue := Byte(Color shr 16);
    Color4 := ARGB(Alpha, Red, Green, Blue);
    HGE.Rectangle(DestRect.Left, DestRect.Top, DestRect.Right - DestRect.Left, DestRect.Bottom - DestRect.Top, Color4, True, Z, BlendMode);
  end;
end;

procedure THGECanvas.Line(Pt1, Pt2: TPoint; Color: TColor; Z: Single; BlendMode: Integer);
var
  Color4: Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  HGE.Line2Color(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y, Color4, Color4, Z, BlendMode);
end;

procedure THGECanvas.FillTri(const p1, p2, p3: TPoint; c1, c2, c3: TColor;
  Z: Single; BlendMode: Integer); //画三角形
var
  Color1, Color2, Color3: Cardinal;
begin
  Color1 := DisplaceRB(c1) or $FF000000;
  Color2 := DisplaceRB(c2) or $FF000000;
  Color3 := DisplaceRB(c3) or $FF000000;
  HGE.Triangle(P1.X, P1.Y, P2.X, P2.Y, P3.X, P3.Y, Color1, Color2, Color3, True, Z, BlendMODE);
end;

procedure THGECanvas.Circle(X, Y, Radius: Single; Color: TColor; Filled: Boolean; Z: Single; BlendMode: Integer);
var
  Color4: Cardinal;
begin
  Color4 := DisplaceRB(Color) or $FF000000;
  HGE.Circle(X, Y, Radius, Color4, Filled, Z, BlendMode);
end;
//--------------------------------------------------------------------------------------------------------------

procedure THGECanvas.Draw(Image: TTexture; PatternIndex: Integer; X, Y, Z: Single; BlendMode: Integer);
var
  TempX1, TempY1, TempX2, TempY2: Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor($FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;

  FQuad.V[0].Z := Z;
  FQuad.V[1].Z := Z;
  FQuad.V[2].Z := Z;
  FQuad.V[3].Z := Z;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw(Image: TTexture; PatternIndex: Integer; X, Y: Single; BlendMode: Integer);
var
  TempX1, TempY1, TempX2, TempY2: Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor($FFFFFFFF, $FFFFFFFF, $FFFFFFFF, $FFFFFFFF);
  TempX1 := X;
  TempY1 := Y;
  TempX2 := X + FWidth;
  TempY2 := Y + FHeight;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY: Single;
  MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
var
  TempX1, TempY1, TempX2, TempY2: Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color1, Color2, Color3, Color4);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY, ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
var
  TempX1, TempY1, TempX2, TempY2: Single;
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;

  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawEx(Image: TTexture; PatternIndex: Integer; X: Single; Y: Single; Scale: Single; DoCenter: Boolean; Color: Cardinal; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
  DoCenter, MirrorX, MirrorY: Boolean; Red, Green, Blue, Alpha: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
  DoCenter: Boolean; Red, Green, Blue, Alpha: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawColor1(Image: TTexture; PatternIndex: Integer; X, Y: Single;
  Red, Green, Blue, Alpha: Byte; BlendMode: Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, ARGB(Alpha, Red, Green, Blue), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
  DoCenter, MirrorX, MirrorY: Boolean; Alpha: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
  DoCenter: Boolean; Alpha: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;

  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha1(Image: TTexture; PatternIndex: Integer; X, Y: Single;
  Alpha: Byte; Blendmode: Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, ARGB(Alpha, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
  DoCenter, MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
  DoCenter: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, centerPosY, Scale, Scale,
    False, False, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawColor4(Image: TTexture; PatternIndex: Integer; X, Y: Single;
  Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False, Color1, Color2, Color3, Color4, BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y, ScaleX, ScaleY: Single;
  DoCenter, MirrorX, MirrorY: Boolean; Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, ScaleX, ScaleY,
    MirrorX, MirrorY,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y, Scale: Single;
  DoCenter: Boolean; Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer);
var
  CenterPosX, CenterPosY: Single;
begin
  if DoCenter then
  begin
    CenterPosX := Image.PatternWidth div 2;
    CenterPosY := Image.PatternHeight div 2;
  end
  else
  begin
    CenterPosX := 0;
    CenterPosY := 0;
  end;
  DrawEx(Image, PatternIndex, X, Y, CenterPosX, CenterPosY, Scale, Scale,
    False, False,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.DrawAlpha4(Image: TTexture; PatternIndex: Integer; X, Y: Single;
  Alpha1, Alpha2, Alpha3, Alpha4: Byte; BlendMode: Integer);
begin
  DrawEx(Image, PatternIndex, X, Y, 0, 0, 1, 1,
    False, False,
    ARGB(Alpha1, 255, 255, 255), ARGB(Alpha2, 255, 255, 255), ARGB(Alpha3, 255, 255, 255), ARGB(Alpha4, 255, 255, 255), BlendMode);
end;

procedure THGECanvas.Draw4V(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4: Single;
  MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  FQuad.V[0].X := X1; FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2; FQuad.V[1].Y := Y2;
  FQuad.V[2].X := X3; FQuad.V[2].Y := Y3;
  FQuad.V[3].X := X4; FQuad.V[3].Y := Y4;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.Draw4V(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2, X3, Y3, X4, Y4: Single;
  MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color1, Color2, Color3, Color4);
  FQuad.V[0].X := X1; FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2; FQuad.V[1].Y := Y2;
  FQuad.V[2].X := X3; FQuad.V[2].Y := Y3;
  FQuad.V[3].X := X4; FQuad.V[3].Y := Y4;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawStretch(Image: TTexture; PatternIndex: Integer; X1, Y1, X2, Y2: Single;
  MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
begin
  SetPattern(Image, PatternIndex);
  SetColor(Color);
  FQuad.V[0].X := X1; FQuad.V[0].Y := Y1;
  FQuad.V[1].X := X2; FQuad.V[1].Y := Y1;
  FQuad.V[2].X := X2; FQuad.V[2].Y := Y2;
  FQuad.V[3].X := X1; FQuad.V[3].Y := Y2;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawPart(Texture: TTexture; X, Y, SrcX, SrcY, Width, Height,
  ScaleX, ScaleY, CenterX, CenterY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
var
  TexX1, TexY1, TexX2, TexY2: Single;
  TempX1, TempY1, TempX2, TempY2: Single;
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

  FQuad.V[0].TX := TexX1; FQuad.V[0].TY := TexY1;
  FQuad.V[1].TX := TexX2; FQuad.V[1].TY := TexY1;
  FQuad.V[2].TX := TexX2; FQuad.V[2].TY := TexY2;
  FQuad.V[3].TX := TexX1; FQuad.V[3].TY := TexY2;

  FQuad.V[0].Z := 0.5;
  FQuad.V[1].Z := 0.5;
  FQuad.V[2].Z := 0.5;
  FQuad.V[3].Z := 0.5;
  SetColor(Color);
  TempX1 := X - CenterX * ScaleX;
  TempY1 := Y - CenterY * ScaleY;
  TempX2 := (X + FWidth * ScaleX) - CenterX * ScaleX;
  TempY2 := (Y + FHeight * ScaleY) - CenterY * ScaleY;

  FQuad.V[0].X := TempX1; FQuad.V[0].Y := TempY1;
  FQuad.V[1].X := TempX2; FQuad.V[1].Y := TempY1;
  FQuad.V[2].X := TempX2; FQuad.V[2].Y := TempY2;
  FQuad.V[3].X := TempX1; FQuad.V[3].Y := TempY2;
  SetMirror(MirrorX, MirrorY);

  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawPart(Texture: TTexture; X, Y, SrcX, SrcY, Width, Height: Single;
  Color: Cardinal; BlendMode: Integer);
begin
  DrawPart(Texture, X, Y, SrcX, SrcY, Width, Height, 1, 1, 0, 0, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawRotate(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
  Angle, ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
var
  TX1, TY1, TX2, TY2, SinT, CosT: Single;
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
    FQuad.V[0].X := TX1 + X; FQuad.V[0].Y := TY1 + Y;
    FQuad.V[1].X := TX2 + X; FQuad.V[1].Y := TY1 + Y;
    FQuad.V[2].X := TX2 + X; FQuad.V[2].Y := TY2 + Y;
    FQuad.V[3].X := TX1 + X; FQuad.V[3].Y := TY2 + Y;
  end;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);
end;

procedure THGECanvas.DrawRotate(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
  Angle: Real; Color: Cardinal; BlendMode: Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, CenterX, CenterY, Angle, 1, 1, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawRotateColor4(Image: TTexture; PatternIndex: Integer; X, Y, CenterX, CenterY,
  Angle, ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color1, Color2, Color3, Color4: Cardinal; BlendMode: Integer);
var
  TX1, TY1, TX2, TY2, SinT, CosT: Single;
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
    FQuad.V[0].X := TX1 + X; FQuad.V[0].Y := TY1 + Y;
    FQuad.V[1].X := TX2 + X; FQuad.V[1].Y := TY1 + Y;
    FQuad.V[2].X := TX2 + X; FQuad.V[2].Y := TY2 + Y;
    FQuad.V[3].X := TX1 + X; FQuad.V[3].Y := TY2 + Y;
  end;
  SetMirror(MirrorX, MirrorY);
  FQuad.Blend := BlendMode;
  FHGE.Gfx_RenderQuad(FQuad);

end;

procedure THGECanvas.DrawRotateC(Image: TTexture; PatternIndex: Integer; X, Y, Angle,
  ScaleX, ScaleY: Single; MirrorX, MirrorY: Boolean; Color: Cardinal; BlendMode: Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, Image.PatternWidth div 2, Image.PatternHeight div 2,
    Angle, ScaleX, ScaleY, MirrorX, MirrorY, Color, BlendMode);
end;

procedure THGECanvas.DrawRotateC(Image: TTexture; PatternIndex: Integer; X, Y, Angle: Single;
  Color: Cardinal; BlendMode: Integer);
begin
  DrawRotate(Image, PatternIndex, X, Y, Image.PatternWidth div 2, Image.PatternHeight div 2,
    Angle, 1, 1, False, False, Color, BlendMode);
end;

procedure THGECanvas.DrawWaveX(Image: TTexture; X, Y, Width, Height: Integer; Amp, Len,
  Phase: Integer; Color: Cardinal; BlendMode: Integer);
var
  I, J: Integer;
begin
  for J := 0 to Width do
  begin
    I := Trunc(J * Image.PatternWidth / Width);
    DrawPart(Image, X + J, Y + Amp * Sin((Phase + J) * PI * Width / Len / 256),
      I, 0, 1, Height, Color, BlendMode);
  end;
end;

procedure THGECanvas.DrawWaveY(Image: TTexture; X, Y, Width, Height: Integer; Amp, Len,
  Phase: Integer; Color: Cardinal; BlendMode: Integer);
var
  I, J: Integer;
begin
  for J := 0 to Height do
  begin
    I := Trunc(J * Image.PatternHeight / Height);
    DrawPart(Image, X + Amp * Sin((Phase + J) * PI * Height / Len / 256), Y + J,
      0, I, Width, 1, Color, BlendMode);
  end;
end;

initialization


end.

