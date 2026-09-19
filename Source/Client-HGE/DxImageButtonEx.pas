unit DxImageButtonEx;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  HGE,
  HGEFontEx,
  HGECanvas,
  GameImages,
  DxComponents,
  DxControls,
  DxImageButton,
  MShare,
  ClFunc,
  Math,
  HUtil32;

type
  TTokenLine = class;
  TTokenBase = class(TObject)
  private
    FWidth:Integer;
    FHeight:Integer;
    FOffsetX:Integer;
    FOffsetY:Integer;
    FOwner:TTokenLine;
  public
    constructor Create(AOwner:TTokenLine); virtual;
    procedure Initialize; virtual; abstract;
    procedure Paint(PointX, PointY:Integer); virtual; abstract;

    property Owner:TTokenLine read FOwner;
    property Width:Integer read FWidth write FWidth;
    property Height:Integer read FHeight write FHeight;
    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
  end;

  TTokenText = class(TTokenBase)
  private
    FCaption:string;
    FFontName:string;
    FFontColor:TColor;
    FFontSize:Integer;
    FFontStyle:TFontStyles;
    FFontStroke:Boolean;
  public
    constructor Create(AOwner:TTokenLine); override;
    procedure Initialize; override;
    procedure Paint(PointX, PointY:Integer); override;

    property Caption:string read FCaption;
    property FontName:string read FFontName;
    property FontColor:TColor read FFontColor;
    property FontSize:Integer read FFontSize;
    property FontStyle:TFontStyles read FFontStyle;
    property FontStroke:Boolean read FFontStroke;
  end;

  TTokenImage = class(TTokenBase)
  private
    FGameImages:TGameImages;
    FImageIndex:Integer;
    FDrawBlend:Boolean;
  public
    constructor Create(AOwner:TTokenLine); override;
    procedure Initialize; override;
    procedure Paint(PointX, PointY:Integer); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property ImageIndex:Integer read FImageIndex write FImageIndex;
    property DrawBlend:Boolean read FDrawBlend write FDrawBlend;
  end;

  TTokenPlayImage = class(TTokenBase)
  private
    FGameImages:TGameImages;
    FDrawBlend:Boolean;
    FStartIndex:Integer;
    FDrawCount:Integer;
    FDrawTime:Integer;
    FDrawTick:Cardinal;
    FDrawIndex:Integer;
  public
    constructor Create(AOwner:TTokenLine); override;
    procedure Initialize; override;
    procedure Paint(PointX, PointY:Integer); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property StartIndex:Integer read FStartIndex;
    property DrawCount:Integer read FDrawCount;
    property DrawTime:Integer read FDrawTime;
    property DrawBlend:Boolean read FDrawBlend;
  end;

  TTokenLine = class(TObject)
  private
    FTokens:TList;
    FWidth:Integer;
    FHeight:Integer;

    function GetCount:Integer;
    function GetTokens(Index:Integer):TTokenBase;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure RecalSize;
    procedure AddToken(Token:TTokenBase);
    property Count:Integer read GetCount;
    property Tokens[Index:Integer]:TTokenBase read GetTokens;

    property Width:Integer read FWidth;
    property Height:Integer read FHeight;
  end;

  TLineList = class(TObject)
  private
    FLines:TList;
    FWidth:Integer;
    FHeight:Integer;

    function GetCount:Integer;
    function GetLines(Index:Integer):TTokenLine;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure RecalSize;
    function AddLine:TTokenLine;
    property Count:Integer read GetCount;
    property Lines[Index:Integer]:TTokenLine read GetLines;

    property Width:Integer read FWidth;
    property Height:Integer read FHeight;
  end;

  TDxImageButtonEx = class(TDxImageButton)
  private
    FLineList:TLineList;
  protected
    procedure SetCaptionA(Value:TCaption); override;
    procedure SetCaptionV(Value:TCaption); override;
    procedure DoDrawCaption; override;
  public
    {$IF CLIENTEXE = 1}
    constructor Create(AOwner:TDxControl); override;
    {$ELSE}
    constructor Create(AOwner:TComponent); override;
    {$IFEND}
    destructor Destroy(); override;
  end;

implementation

const
  ExpandLineHeight = 2;

  { TTokenBase }

constructor TTokenBase.Create(AOwner:TTokenLine);
begin
  inherited Create;
  FOffsetX := 0;
  FOffsetY := 0;
  FOwner := AOwner;
end;

{ TTokenText }

constructor TTokenText.Create(AOwner:TTokenLine);
begin
  inherited Create(AOwner);
  FCaption := '';
  FFontName := '';
  FFontColor := clWhite;
  FFontSize := 0;
  FFontStyle := [];
  FFontStroke := False;
end;

procedure TTokenText.Initialize;
var
  HGEFont:THGEFont;
begin
  HGEFont := nil;

  if FFontName <> '' then
    HGEFont := TextureFonts.FindFont(FFontName, FFontSize, FFontStyle);

  if HGEFont = nil then
    HGEFont := TextureFonts.FindFont(g_sCurFontName, FFontSize, FFontStyle);

  if HGEFont <> nil then begin
    FWidth := HGEFont.TextWidth(FCaption);
    FHeight := HGEFont.TextHeight('Pp');

    if FFontStroke then begin
      FWidth := FWidth + 2;
      FHeight := FHeight + 2;
    end;
  end;
end;

procedure TTokenText.Paint(PointX, PointY:Integer);
var
  //ARect: TRect;
  HGEFont:THGEFont;
begin
  if FCaption = '' then Exit;

  HGEFont := nil;

  if FFontName <> '' then
    HGEFont := TextureFonts.FindFont(FFontName, FFontSize, FFontStyle);

  if HGEFont = nil then
    HGEFont := TextureFonts.FindFont(g_sCurFontName, FFontSize, FFontStyle);

  if HGEFont <> nil then begin
    if FFontStroke then
      BoldTextOut(HGEFont, PointX + FOffsetX, PointY + FOffsetY, FCaption, FFontColor, clBlack)
    else
      HGEFont.TextOut(PointX + FOffsetX, PointY + FOffsetY, FCaption, FFontColor);
  end;
end;

{ TTokenImage }

constructor TTokenImage.Create(AOwner:TTokenLine);
begin
  inherited Create(AOwner);
  FGameImages := nil;
  FImageIndex := 0;
  FDrawBlend := False;
end;

procedure TTokenImage.Initialize;
var
  Texture:TTexture;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];

    if (Texture <> nil) then begin
      FWidth := Texture.Width;
      FHeight := Texture.Height;
    end;
  end;
end;

procedure TTokenImage.Paint(PointX, PointY:Integer);
var
  Texture:TTexture;
  X, Y:Integer;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];
    if Texture <> nil then begin
      X := PointX + FOffsetX;
      Y := PointY + FOffsetY;
      GameCanvas.Draw(X, Y, Texture);
    end;
  end;
end;

{ TTokenPlayImage }

constructor TTokenPlayImage.Create(AOwner:TTokenLine);
begin
  inherited Create(AOwner);
  FGameImages := nil;
  FDrawBlend := False;
  FStartIndex := 0;
  FDrawCount := 0;
  FDrawTime := 100;
  FDrawTick := MyGetTickCount;
  FDrawIndex := 0;
end;

procedure TTokenPlayImage.Initialize;
begin
  inherited;
  FWidth := 0;
  FHeight := 0;
end;

procedure TTokenPlayImage.Paint(PointX, PointY:Integer);
var
  Texture:TTexture;
  X, Y:Integer;
  Pt:TPoint;
  CurTick:Cardinal;
begin
  if FGameImages = nil then Exit;

  Texture := FGameImages.GetCachedImage(FDrawIndex, Pt.X, Pt.Y);
  if Texture <> nil then begin
    // 修正物品和对应的特效对不上 chongchong 2015-04-11
    X := PointX + FOffsetX + Pt.X;
    Y := PointY + FOffsetY + Pt.Y;

    if FDrawBlend then
      GameCanvas.DrawBlend(X, Y, Texture)
    else
      GameCanvas.Draw(X, Y, Texture)
  end;

  CurTick := MyGetTickCount;
  if CurTick - FDrawTick > Cardinal(FDrawTime) then begin
    FDrawTick := CurTick;
    Inc(FDrawIndex);
    if FDrawIndex - FStartIndex + 1 > DrawCount then
      FDrawIndex := FStartIndex;
  end;
end;

{ TTokenLine }

constructor TTokenLine.Create;
begin
  FTokens := TList.Create;
  FWidth := 0;
  FHeight := 0;
end;

destructor TTokenLine.Destroy;
begin
  Clear;
  FTokens.Free;
  inherited;
end;

procedure TTokenLine.Clear;
var
  I:Integer;
begin
  for I := 0 to FTokens.Count - 1 do begin
    TTokenBase(FTokens.Items[I]).Free;
  end;
  FTokens.Clear;
end;

function TTokenLine.GetCount:Integer;
begin
  Result := FTokens.Count;
end;

function TTokenLine.GetTokens(Index:Integer):TTokenBase;
begin
  Result := FTokens.Items[Index];
end;

procedure TTokenLine.AddToken(Token:TTokenBase);
begin
  FTokens.Add(Token);
end;

procedure TTokenLine.RecalSize;
var
  I:Integer;
  Token:TTokenBase;
begin
  FWidth := 0;
  FHeight := 0;

  for I := 0 to FTokens.Count - 1 do begin
    Token := TTokenBase(FTokens.Items[I]);
    Token.Initialize;

    if Token is TTokenText then begin
      FHeight := Max(FHeight, Token.Height);
      FWidth := FWidth + Token.Width;
    end;
  end;

  if CurrentFont <> nil then begin
    if FWidth <= 0 then
      FWidth := CurrentFont.TextWidth('0');

    if FHeight <= 0 then
      FHeight := g_CurrentFontHeight;
  end;
end;

{ TLineList }

constructor TLineList.Create;
begin
  FLines := TList.Create;
  FWidth := 0;
  FHeight := 0;
end;

destructor TLineList.Destroy;
begin
  Clear;
  FLines.Free;
  inherited;
end;

function TLineList.AddLine:TTokenLine;
begin
  Result := TTokenLine.Create;
  FLines.Add(Result);
end;

procedure TLineList.Clear;
var
  I:Integer;
begin
  for I := 0 to FLines.Count - 1 do begin
    TTokenLine(FLines.Items[I]).Free;
  end;
  FLines.Clear;
end;

function TLineList.GetCount:Integer;
begin
  Result := FLines.Count;
end;

function TLineList.GetLines(Index:Integer):TTokenLine;
begin
  Result := FLines.Items[Index];
end;

procedure TLineList.RecalSize;
var
  I:Integer;
  Line:TTokenLine;
  AddHeight:Integer;
begin
  FWidth := 0;
  FHeight := 0;
  AddHeight := 0;
  for I := 0 to FLines.Count - 1 do begin
    Line := FLines.Items[I];
    Line.RecalSize;

    if FWidth < Line.FWidth then
      FWidth := Line.FWidth;

    FHeight := FHeight + Line.FHeight + AddHeight;
    AddHeight := ExpandLineHeight;
  end;
end;

// 提示窗口支持自定义图片显示 chongchong 2015-01-10

function ProcessButtonText(S:string; TokenList:TTokenLine; Font:TDxFont):string;
// aaa{自自定颜色|100}bbbb

  function NewTokenText(Text:string):string;
  var
    S1, S2, S3, S4:string;
    Index1, Index2, Index3, nColor:Integer;
    boCustomColorText:Boolean;
    TokenText:TTokenText;
  begin
    if Length(Text) = 0 then begin
      TokenText := TTokenText.Create(TokenList);
      TokenText.FFontSize := Font.Size;
      TokenText.FFontStyle := Font.Style;
      TokenText.FFontColor := Font.Color;
      TokenText.FFontStroke := Font.Bold;
      TokenText.FCaption := Text;
      TokenText.FFontName := Font.Name;
      TokenList.AddToken(TokenText);
      Result := Text;
      Exit;
    end;

    Result := '';

    Index1 := Pos('{', Text);

    if Index1 > 0 then
      Index2 := Pos('}', Text)
    else
      Index2 := 0;

    while (Index1 > 0) and (Index2 > 0) and (Text <> '') do begin
      S1 := Copy(Text, 1, Index1 - 1);
      S2 := Copy(Text, Index1 + 1, Index2 - Index1 - 1);

      if Length(S1) > 0 then begin
        TokenText := TTokenText.Create(TokenList);
        TokenText.FFontSize := Font.Size;
        TokenText.FFontStyle := Font.Style;
        TokenText.FFontColor := Font.Color;
        TokenText.FFontStroke := Font.Bold;
        TokenText.FCaption := S1;
        TokenText.FFontName := Font.Name;
        TokenList.AddToken(TokenText);
        Result := Result + TokenText.FCaption;
      end;

      boCustomColorText := False;
      if Length(S2) > 0 then begin
        Index3 := Pos('|', S2);
        if Index3 > 0 then begin
          S3 := Copy(S2, 1, Index3 - 1);
          S4 := Copy(S2, Index3 + 1, MaxInt);

          nColor := StrToIntDef(S4, -1);
          if (nColor >= 0) and (nColor <= 255) then begin
            boCustomColorText := True;

            if Length(S3) > 0 then begin
              TokenText := TTokenText.Create(TokenList);
              TokenText.FFontSize := Font.Size;
              TokenText.FFontStyle := Font.Style;
              TokenText.FFontColor := GetRGB(nColor);
              TokenText.FFontStroke := Font.Bold;
              TokenText.FCaption := S3;
              TokenText.FFontName := Font.Name;
              TokenList.AddToken(TokenText);
              Result := Result + TokenText.FCaption;
            end;
          end;
        end;
      end;

      if not boCustomColorText then begin
        TokenText := TTokenText.Create(TokenList);
        TokenText.FFontSize := Font.Size;
        TokenText.FFontStyle := Font.Style;
        TokenText.FFontColor := Font.Color;
        TokenText.FFontStroke := Font.Bold;
        TokenText.FCaption := '{' + S2 + '}';
        TokenText.FFontName := Font.Name;
        TokenList.AddToken(TokenText);
        Result := Result + TokenText.FCaption;
      end;

      Text := Copy(Text, Index2 + 1, MaxInt);
      Index1 := Pos('{', Text);
      if Index1 > 0 then
        Index2 := Pos('}', Text)
      else
        Index2 := 0;
    end;

    if Length(Text) > 0 then begin
      TokenText := TTokenText.Create(TokenList);
      TokenText.FFontSize := Font.Size;
      TokenText.FFontStyle := Font.Style;
      TokenText.FFontColor := Font.Color;
      TokenText.FFontStroke := Font.Bold;
      TokenText.FCaption := Text;
      TokenText.FFontName := Font.Name;
      TokenList.AddToken(TokenText);
      Result := Result + TokenText.FCaption;
    end;
  end;

  procedure NewTokenImage(Text:string; AGameImages:TGameImages; AImageIndex:Integer; OffsetX, OffsetY:Integer);
  var
    TokenImage:TTokenImage;
  begin
    if AGameImages <> nil then begin
      TokenImage := TTokenImage.Create(TokenList);
      TokenImage.FGameImages := AGameImages;
      TokenImage.FImageIndex := AImageIndex;
      TokenImage.FOffsetX := OffsetX;
      TokenImage.FOffsetY := OffsetY;

      TokenList.AddToken(TokenImage);
    end;
  end;

  procedure NewTokenPlayImage(Text:string; AGameImages:TGameImages; AImageIndex, APlayCount, APlayTime:Integer; OffsetX, OffsetY:Integer; IsBlendDraw:Boolean);
  var
    TokenImage:TTokenPlayImage;
  begin
    if AGameImages <> nil then begin
      TokenImage := TTokenPlayImage.Create(TokenList);
      TokenImage.FGameImages := AGameImages;
      TokenImage.FStartIndex := AImageIndex;
      TokenImage.FDrawIndex := AImageIndex;
      TokenImage.FDrawCount := APlayCount;
      TokenImage.FDrawTime := APlayTime;
      TokenImage.FDrawTick := MyGetTickCount;
      TokenImage.FOffsetX := OffsetX;
      TokenImage.FOffsetY := OffsetY;
      TokenImage.FDrawBlend := IsBlendDraw;
      TokenList.AddToken(TokenImage);
    end;
  end;

  function CheckTokenImage(Text:string; var AGameImages:TGameImages;
    var ImageIndex, OffsetX, OffsetY:Integer;
    var IsPlayImg:Boolean; var PlayCount, PlayTime:Integer; var IsBlendDraw:Boolean):Boolean;
  var
    Temp:string;
    SName, S1, S2, S3, S4, S5, S6, S7 {, sID}:string;
    I1, I2:Integer;
  begin
    Result := False; //'<Img:N:F:X:Y>'
    AGameImages := nil;

    if Length(Text) > 2 then begin
      Temp := Copy(Text, 2, Length(Text) - 2);
      Temp := GetValidStr3_Ex(Temp, SName, ':');
      Temp := GetValidStr3_Ex(Temp, S1, ':');
      Temp := GetValidStr3_Ex(Temp, S2, ':');
      Temp := GetValidStr3_Ex(Temp, S3, ':');
      Temp := GetValidStr3_Ex(Temp, S4, ':');
      Temp := GetValidStr3_Ex(Temp, S5, ':');
      Temp := GetValidStr3_Ex(Temp, S6, ':');
      Temp := GetValidStr3_Ex(Temp, S7, ':');

      I1 := StrToIntDef(S1, -1);
      I2 := StrToIntDef(S2, -1);
      IsPlayImg := False;
      IsBlendDraw := True;

      // <img: xxxx
      if SameText(SName, 'Img') and (I2 >= 0) then begin
        if (I1 >= 0) then begin
          Result := True;

          if I2 < g_EffectImageList.Count then
            AGameImages := TGameImages(g_EffectImageList.Objects[I2]);

          ImageIndex := I1;
          OffsetX := StrToIntDef(S3, 0);
          OffsetY := StrToIntDef(S4, 0);
        end;
      end else if SameText(SName, 'Looks') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WBagItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
      end else if SameText(SName, 'DnItems') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WDnItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
      end else if SameText(SName, 'StateItem') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WStateItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
      end else if SameText(SName, 'NewopUI') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WNewopUIImages;
        ImageIndex := I1;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
      end else if SameText(SName, 'PlayImg') and (I1 >= 0) and (I2 >= 0) then begin
        //格式: <PlayImg:F:N:C:T:X:Y:M>
        //F表示WIL文件序号,N表示播放开始图片,C表示播放张数,T表示播放速度(毫秒),X是横向坐标,Y是纵向坐标;M绘制模式
        Result := True;
        IsPlayImg := True;

        PlayCount := StrToIntDef(S3, 0);
        PlayTime := StrToIntDef(S4, 0);

        if PlayTime <= 0 then
          PlayTime := 100;

        if (I1 < g_EffectImageList.Count) and (PlayCount > 0) then
          AGameImages := TGameImages(g_EffectImageList.Objects[I1]);

        ImageIndex := I2;

        OffsetX := StrToIntDef(S5, 0);
        OffsetY := StrToIntDef(S6, 0);
        IsBlendDraw := StrToIntDef(S7, 0) <> 0;
      end;
    end;
  end;
var
  Index1, Index2:Integer;
  StrB, StrC:string;
  OX, OY:Integer;
  GameImages:TGameImages;
  GameImageIndex:Integer;
  IsPlayImg, IsBlendDraw:Boolean;
  PlayCount, PlayTime:Integer;

  //ShowText: string;
  //btColor: Byte;
begin
  Result := '';
  Index1 := Pos('<', S);
  if Index1 = 0 then begin
    Result := Result + NewTokenText(S);
  end else begin
    while S <> '' do begin
      StrB := StrB + Copy(S, 1, Index1 - 1);
      S := Copy(S, Index1, MAXINT);

      Index2 := Pos('>', S);
      if Index2 = 0 then begin
        Result := Result + NewTokenText(StrB + S);
        Break;
      end
      else begin
        IsPlayImg := False;
        IsBlendDraw := False;

        StrC := Copy(S, 1, Index2);
        if not CheckTokenImage(StrC, GameImages, GameImageIndex, OX, OY, IsPlayImg, PlayCount, PlayTime, IsBlendDraw) then
          StrB := StrB + StrC
        else begin
          if Length(StrB) > 0 then begin
            Result := Result + NewTokenText(StrB);
          end;

          if IsPlayImg then
            NewTokenPlayImage(StrC, GameImages, GameImageIndex, PlayCount, PlayTime, OX, OY, IsBlendDraw)
          else
            NewTokenImage(StrC, GameImages, GameImageIndex, OX, OY);

          StrB := '';
          StrC := '';
        end;

        S := Copy(S, Index2 + 1, MaxInt);
        Index1 := Pos('<', S);
        if Index1 = 0 then begin
          S := StrB + S;
          if S <> '' then begin
            Result := Result + NewTokenText(S);
          end;
          Break;
        end;
      end;
    end;
  end;
end;

{ TDxImageButtonEx }

{$IF  CLIENTEXE = 1}

constructor TDxImageButtonEx.Create(AOwner:TDxControl);
{$ELSE}

  constructor TDxImageButtonEx.Create(AOwner:TComponent);
    {$IFEND}
  begin
    inherited Create(AOwner);
    FLineList := TLineList.Create;
  end;

  destructor TDxImageButtonEx.Destroy();
  begin
    FLineList.Free;
    inherited Destroy;
  end;

  procedure TDxImageButtonEx.SetCaptionA(Value:TCaption);
  var
    SL:TStringList;
    I:Integer;
    TokenLine:TTokenLine;
    sNewLine:string;
  begin
    FLineList.Clear;
    SL := TStringList.Create;
    try
      SL.Delimiter := '\';
      SL.Text := Value;

      Value := '';
      sNewLine := '';
      for I := 0 to SL.Count - 1 do begin
        TokenLine := FLineList.AddLine;
        // 'aaa{自自定颜色|100}bbbb'
        Value := Value + sNewLine + ProcessButtonText(SL.Strings[I], TokenLine, CaptionColor.Up);
        sNewLine := sLineBreak;
      end;

      FLineList.RecalSize;
    finally
      SL.Free;
    end;
    inherited SetCaptionA(Value);
  end;

  procedure TDxImageButtonEx.SetCaptionV(Value:TCaption);
  //var
    //SL:TStringList;
    //I:Integer;
    //TokenLine:TTokenLine;
    //sNewLine:string;
  begin
    //HZQ 20230628 SetCaptionV改变了已设置好的数据图像数据，所以，把下面这段去掉了，只更新文本
    (*
    FLineList.Clear;
    SL := TStringList.Create;
    try
      SL.Delimiter := '\';
      SL.Text := Value;

      Value := '';
      sNewLine := '';
      for I := 0 to SL.Count - 1 do begin
        TokenLine := FLineList.AddLine;
        // 'aaa{自自定颜色|100}bbbb'
        Value := Value + sNewLine + ProcessButtonText(SL.Strings[I], TokenLine, CaptionColor.Up);
        sNewLine := sLineBreak;
      end;

      FLineList.RecalSize;
    finally
      SL.Free;
    end;
    *)
    inherited SetCaptionV(Value);
  end;

  procedure TDxImageButtonEx.DoDrawCaption;
  var
    X, Y:Integer;
    I, II:Integer;
    vtRect:TRect;
    R, RLine:TRect;
    Pt:TPoint;
    Line:TTokenLine;
    Token:TTokenBase;
  begin
    if Caption = '' then Exit;

    vtRect := VirtualRect;
    X := 0;
    Y := 0;
    if Enabled then begin
      if MouseDowned {or Checked} then begin
        X := CaptionOffsetX + CaptionDownOffsetX;
        Y := CaptionOffsetY + CaptionDownOffsetY;
      end
      else begin
        X := CaptionOffsetX;
        Y := CaptionOffsetY;
      end;
    end;

    R.Left := vtRect.Left + (Width - FLineList.Width) div 2;
    R.Top := vtRect.Top + (Height - FLineList.Height) div 2;
    R.Right := R.Left + FLineList.Width;
    R.Bottom := R.Top + FLineList.Height;

    for I := 0 to FLineList.Count - 1 do begin
      Line := FLineList.Lines[I];
      for II := 0 to Line.Count - 1 do begin
        Token := Line.Tokens[II];
        //if Token is TTokenImage then begin
        //    OutputDebugString('Hello');
        //end;

        if not (Token is TTokenText) then begin
            Token.Paint(vtRect.Left, vtRect.Top);
        end;
      end;
    end;

    RLine := R;
    for I := 0 to FLineList.Count - 1 do begin
      Line := FLineList.Lines[I];

      RLine.Left := R.Left + (R.Right - R.Left - Line.FWidth) div 2;
      RLine.Right := RLine.Left + Line.FWidth;

      Pt := RLine.TopLeft;
      for II := 0 to Line.Count - 1 do begin
        Token := Line.Tokens[II];
        if (Token is TTokenText) then begin
          Token.Paint(Pt.X + X, Pt.Y + Y);
          Pt.X := Pt.X + Token.Width;
        end;
      end;

      RLine.Top := RLine.Top + Line.Height + ExpandLineHeight;
    end;
  end;

end.
