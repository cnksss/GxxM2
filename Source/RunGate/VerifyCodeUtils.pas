unit VerifyCodeUtils;

interface

uses
  Windows, Classes, SysUtils, Graphics, GDIPOBJ, GDIPAPI, Math, ActiveX;

  procedure MakeVerifyCode(Code: string; Bitmap: TBitmap; Font: TFont; WaveValue: Integer; DrawNoise: Boolean = True; InflateRectValue: Integer = -3);

implementation

const
  CaptchaBackgroundColor = clSilver;
  CaptchaBackgroundColorTo = clNone;
  ShapeColor = clBlack;

function ColorToARGB(Color: TColor): ARGB;
var
  c: TColor;
begin
  c := ColorToRGB(Color);
  Result := ARGB( $FF000000 or ((DWORD(c) and $FF) shl 16) or ((DWORD(c) and $FF00) or ((DWORD(c) and $ff0000) shr 16)));
end;

procedure DrawNoise(G: TGPGraphics; Width, Height: Integer);
var
  s, i, j, x, y: Integer;
  HB: TGPHatchBrush;
begin
  s := Max(6, Max(Width, Height) div 50);
  j := (Width * Height div 60);
  HB := TGPHatchBrush.Create(HatchStyleTrellis, ColorToARGB(CaptchaBackgroundColorTo), ColorToARGB(CaptchaBackgroundColor));
  for i := 0 to j do
  begin
    x := Random(Width);
    y := Random(Height);
    G.FillEllipse(HB, x, y, 1 + Random(s), 1 + Random(s));
  end;
  HB.Free;
end;

procedure DrawLineNoise(g: TGPGraphics; Width, Height: Integer; Color: TColor);
var
  s, i, j, x, y, x2, y2: Integer;
  pen: TGPPen;
begin
  s := Max(6, Max(Width, Height) div 50);
  j := (Width * Height div 50);
  pen := TGPPen.Create(ColorToARGB(Color), 1.3);
  for i := 0 to j do
  begin
    x := Random(Width);
    y := Random(Height);
    x2 := 2 + Random(s);
    y2 := 2 + Random(s);
    g.DrawLine(pen, x, y, x + x2, y + y2);
  end;
  pen.Free;
end;

procedure DrawBackGround(G: TGPGraphics; R: TRect);
var
  HB: TGPHatchBrush;
begin
  HB := TGPHatchBrush.Create(HatchStyleSmallConfetti, ColorToARGB(CaptchaBackgroundColorTo), ColorToARGB(CaptchaBackgroundColor));
  G.FillRectangle(HB, R.left, R.Top, R.Right - R.Left, R.Bottom - R.Top);
  HB.Free;

  //--- Some noise
  DrawNoise(G, R.Right - R.Left, R.Bottom - R.Top);
  DrawLineNoise(G, R.Right - R.Left, R.Bottom - R.Top, CaptchaBackgroundColorTo);
end;

procedure ProduceWave(bmp: TBitmap; Distortion: double);
var
  x, y, nx, ny: Integer;
  bmp2: TBitmap;
begin
  if not Assigned(bmp) then
    Exit;

  bmp2 := TBitmap.Create;
  bmp2.Canvas.Lock;
  try
    bmp2.Assign(bmp);
    for y := 0 to bmp.Height do
    begin
      for x := 0 to bmp.Width - 1 do
      begin
        nx := Round(x + (Distortion * Sin(PI * y / 90.0)));
        ny := Round(y + (Distortion * Cos(PI * x / 48.0)));
        if (nx < 0) or (nx >= bmp.Width) then nx := 0;
        if (ny < 0) or (ny >= bmp.Height) then ny := 0;
        bmp.Canvas.Pixels[x, y] := bmp2.Canvas.Pixels[nx, ny];
      end;
    end;
  finally
    bmp2.Canvas.Unlock;
  end;

  bmp2.Free;
end;

procedure DrawGDIPImage(gr: TGPGraphics; Canvas: TCanvas; P: TPoint; bmp: TGraphic; Transparent: Boolean = False); overload;
var
  Img: TGPImage;
  pstm: IStream;
  hGlobal: THandle;
  pcbWrite: Longint;
  ms: TMemoryStream;
  graphics: TGPGraphics;
  ImageAttributes: TGPImageAttributes;
  r, g, b: byte;
  GPBmp: TGPBitmap;
  Aclr: TGPColor;
begin
  if (not Assigned(gr) and not Assigned(Canvas)) then
    Exit;

  graphics := gr;
  if not Assigned(graphics) then
  begin
    graphics := TGPGraphics.Create(Canvas.Handle);
    graphics.SetSmoothingMode(SmoothingModeAntiAlias);
  end;

  ms := TMemoryStream.Create;
  bmp.SaveToStream(ms);
  hGlobal := GlobalAlloc(GMEM_MOVEABLE, ms.Size);
  if (hGlobal = 0) then
  begin
    ms.Free;
    raise Exception.Create('Could not allocate memory for image');
  end;

  try
    pstm := nil;

    // Create IStream* from global memory
    CreateStreamOnHGlobal(hGlobal, TRUE, pstm);
    pstm.Write(ms.Memory, ms.Size,@pcbWrite);

    Img := TGPImage.Create(pstm);
    if Transparent and (Img.GetType <> ImageTypeMetafile) then
    begin
      GPBmp := TGPBitmap.Create(pstm);
      GPBmp.GetPixel(0, 0, AClr);
      GPBmp.Free;

      r := GetRed(AClr);
      g := GetGreen(AClr);
      b := GetBlue(AClr);

      ImageAttributes := TGPImageAttributes.Create;
      ImageAttributes.SetColorKey(MakeColor(r, g, b), MakeColor(r, g, b), ColorAdjustTypeDefault);
      graphics.DrawImage(Img, MakeRect(P.X, P.Y, Img.GetWidth, Img.Getheight),  // destination rectangle
       0, 0,        // upper-left corner of source rectangle
       Img.GetWidth,       // width of source rectangle
       Img.GetHeight,      // height of source rectangle
       UnitPixel,
       ImageAttributes);
      //graphics.DrawImage(Img, P.X, P.y);
      ImageAttributes.Free;
    end
    else
      graphics.DrawImage(Img, P.X, P.y);

    Img.Free;
    ms.Free;
  finally
    GlobalFree(hGlobal);
  end;

  if not Assigned(gr) then
    graphics.Free;
end;


procedure DrawCaptchaCode(G: TGPGraphics; Canvas: TCanvas; Code: string; Font: TFont; WaveValue: Integer; InflateRectValue: Integer = -3);
var
  bmpG: TGPGraphics;
  fontFamily: TGPFontFamily;
  gpfont: TGPFont;
  stringFormat: TGPStringFormat;
  Path: TGPGraphicsPath;
  w, h, fs: integer;
  R: TRect;
  RF: TGPRectF;
  //f: double;
  GPBrsh: TGPBrush;
  bmp: TBitmap;
begin
  if (Code <> '') and (Assigned(G) or Assigned(Canvas)) then
  begin
    R := Canvas.ClipRect;
    InflateRect(R, InflateRectValue, InflateRectValue);
    w := R.Right - R.Left;
    h := R.Bottom - R.Top;
    RF := MakeRect(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top * 1.0);

    G.SetSmoothingMode(SmoothingModeAntiAlias);
    G.SetTextRenderingHint(TextRenderingHintAntiAlias);

    fontFamily:= TGPFontFamily.Create(Font.Name);
    if (fontFamily.GetLastStatus in [FontFamilyNotFound, FontStyleNotFound]) then
    begin
      fontFamily.Free;
      fontFamily := TGPFontFamily.Create('Arial');
    end;

    fs := TFontStyle(byte(Font.Style));
    gpfont := TGPFont.Create(fontFamily, Font.Size , fs, UnitPoint);

    stringFormat := TGPStringFormat.Create;
    stringFormat.SetAlignment(StringAlignmentCenter);
    stringFormat.SetLineAlignment(StringAlignmentCenter);
    stringFormat.SetHotkeyPrefix(HotkeyPrefixShow);

    Path := TGPGraphicsPath.Create;
    Path.AddString(Code, Length(Code), fontFamily, fs, Font.Size, RF, stringFormat);

    GPBrsh := TGPSolidBrush.Create(ColorToARGB(Font.Color));

    bmp := TBitmap.Create;
    bmp.Canvas.Lock;
    try
      bmp.Width := w;
      bmp.Height := h;

      bmp.Canvas.Brush.Color := clWhite;
      bmp.Canvas.FillRect(Rect(0, 0, bmp.Width, bmp.Height));

      bmpG := TGPGraphics.Create(bmp.Canvas.Handle);
      //bmpG.SetSmoothingMode(SmoothingModeAntiAlias);
      if Assigned(GPBrsh) then
        bmpG.FillPath(GPBrsh, Path);

      //f := -(5 + Random(15));
      //ProduceWave(bmp, f);
      if WaveValue <> 0 then
      begin
        ProduceWave(bmp, WaveValue);     // Å¤ÇúÖµ  -(8 + Random(6))
      end;


      DrawGDIPImage(G, Canvas, Point(R.Left, R.Top), bmp, True);

      bmpG.Free;
    finally
      bmp.Canvas.Unlock;
    end;

    bmp.Free;

    fontFamily.Free;
    gpfont.Free;
    Path.Free;
  end;
end;

procedure DrawArbitraryShape(G: TGPGraphics; Width, Height: Integer);
var
  pc, w2, h2, t: Integer;
  GPptsF: array[0..8] of TGPPoint;
  Path: TGPGraphicsPath;
  Pen: TGPPen;
  rc, gc, bc: Byte;
begin
  w2 := Width div 2;
  h2 := Height div 2;
  rc := GetRValue(ShapeColor);
  gc := GetGValue(ShapeColor);
  bc := GetBValue(ShapeColor);

  pc := 9;

  GPptsF[0].X := Random(w2);
  GPptsF[0].Y := Random(h2);
  GPptsF[1].X := w2 + Random(w2);
  GPptsF[1].Y := Random(h2);
  GPptsF[2].X := Random(w2);
  GPptsF[2].Y := h2 + Random(Height);
  GPptsF[3].X := w2 + Random(w2);
  GPptsF[3].Y := h2 + Random(Height);
  GPptsF[4].X := Random(Width);
  GPptsF[4].Y := Random(Height);
  GPptsF[5].X := Random(Width);
  GPptsF[5].Y := Random(Height);
  GPptsF[6].X := (w2 div 2) + Random(Width - w2);
  GPptsF[6].Y := (h2 div 2) + Random(Height - h2);
  GPptsF[7].X := (w2 div 2) + Random(Width - w2);
  GPptsF[7].Y := (h2 div 2) + Random(Height - h2);
  GPptsF[8].X := w2 + Random(10);
  GPptsF[8].Y := h2 + Random(20);

  t := Random(30);

  Path := TGPGraphicsPath.Create();
  Path.AddCurve(PGPPoint(@GPPtsF), pc, t);
  Path.CloseFigure;
  //Path.AddPolygon(PGPPoint(@GPPtsF), pc);
  //Path.AddLines(PGPPoint(@GPPtsF), 20);
  Pen := TGPPen.Create(MakeColor(150, rc, gc, bc), 2);
  G.DrawPath(pen, Path);
  Path.Free;
  Pen.Free;
end;

procedure MakeVerifyCode(Code: string; Bitmap: TBitmap; Font: TFont; WaveValue: Integer; DrawNoise: Boolean = True; InflateRectValue: Integer = -3);
var
  G: TGPGraphics;
begin
  G := TGPGraphics.Create(Bitmap.Canvas.Handle);
  try
    G.SetSmoothingMode(SmoothingModeAntiAlias);
    DrawBackGround(G, Bitmap.Canvas.ClipRect);
    
    DrawCaptchaCode(G, Bitmap.Canvas, Code, Font, WaveValue, InflateRectValue);

    if DrawNoise then
      DrawArbitraryShape(G, Bitmap.Width, Bitmap.Height);
  finally
    G.Free;
  end;
end;

end.
