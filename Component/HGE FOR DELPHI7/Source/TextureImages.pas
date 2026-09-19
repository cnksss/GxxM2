unit TextureImages;

interface
uses
  Windows, Classes, SysUtils, Graphics, HGE, HashList, HGEFontEx;
type
  TTextureImages = class;

  TTextureTile = class
  private
    FOwner: TTextureImages;
    FClientRect: TRect;

    FImageRect: array of TImageRect;
    FreeMemCheckTick: LongWord;
    FPatternWidth: Integer;
    FPatternHeight: Integer;
    FRowCount: Integer;
    FColCount: Integer;

    FBuffer: Pointer;
    FInitialized: Boolean;
    FCriticalSection: TRTLCriticalSection;
    function GetTexture: TTexture;
    function GetWidth: Integer;
    function GetHeight: Integer;
    procedure SetWidth(Value: Integer);
    procedure SetHeight(Value: Integer);

    function GetLeft: Integer;
    function GetTop: Integer;
    procedure SetLeft(Value: Integer);
    procedure SetTop(Value: Integer);

    procedure SetPatternWidth(Value: Integer);
    procedure SetPatternHeight(Value: Integer);
    function GetImages(Index: Integer): pTImageRect;
    function GetCount: Integer;
  public
    constructor Create(AOwner: TTextureImages);
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;

    procedure Initialize; virtual;
    procedure Finalize; virtual;
    function GetCachedImage(Index: Integer; var px, py: Integer; var ARect: TRect): TTexture;
    function GetImageInfo(const Text: string): TImageInfo; virtual;
    procedure Draw(const X, Y: Integer; ImageIndexs: TImageIndexs); virtual;

    function Add(const X, Y: Integer; const ATexture: TTexture): pTImageRect; overload; virtual;
    function Add(const Text: string): pTImageRect; overload; virtual;

    procedure TextOut(const X, Y: Integer; const Text: string; Color: TColor); overload; virtual;
    procedure TextOut(const X, Y: Integer; ImageIndexs: TImageIndexs; Color: TColor); overload; virtual;

    function GetActive: pTImageRect;

    property Initialized: Boolean read FInitialized write FInitialized;
    property Owner: TTextureImages read FOwner;
    property Left: Integer read GetLeft write SetLeft;
    property Top: Integer read GetTop write SetTop;
    property Width: Integer read GetWidth write SetWidth;
    property Height: Integer read GetHeight write SetHeight;
    property PatternWidth: Integer read FPatternWidth write SetPatternWidth;
    property PatternHeight: Integer read FPatternHeight write SetPatternHeight;
    property RowCount: Integer read FRowCount;
    property ColCount: Integer read FColCount;
    property Count: Integer read GetCount;
    property Images[Index: Integer]: pTImageRect read GetImages;
    property Texture: TTexture read GetTexture;
  end;

  TTextureImages = class
  private
    FLeft, FTop, FWidth, FHeight: Integer;
    FTexture: TTexture;
    FTextureTiles: array of TTextureTile;
    FCriticalSection: TRTLCriticalSection;
    FCsList: TRTLCriticalSection;
    function GetCount: Integer;
    function GetTile(Index: Integer): TTextureTile;
    procedure SetWidth(Value: Integer);
    procedure SetHeight(Value: Integer);
  public
    FreeMemCheckTick: LongWord;
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    procedure ListLock;
    procedure ListUnLock;
    procedure SetSize(const AWidth, AHeight: Integer);
    procedure Initialize; virtual;
    procedure Finalize; virtual;
    procedure FreeOldMemorys; virtual;
    function Add(): TTextureTile; overload;
    function Add(TextureTile: TTextureTile): TTextureTile; overload;


    property Count: Integer read GetCount;
    property Tiles[Index: Integer]: TTextureTile read GetTile;
    property Texture: TTexture read FTexture write FTexture;

    property Left: Integer read FLeft write FLeft;
    property Top: Integer read FTop write FTop;
    property Width: Integer read FWidth write SetWidth;
    property Height: Integer read FHeight write SetHeight;
  end;


implementation
uses HGECanvas, Math;

constructor TTextureTile.Create(AOwner: TTextureImages);
begin
  InitializeCriticalSection(FCriticalSection);
  FBuffer := nil;
  FOwner := AOwner;
  FPatternWidth := 4;
  FPatternHeight := 4;
  FColCount := 1;
  FRowCount := 1;
  FInitialized := False;
  FClientRect := Rect(FOwner.Left, FOwner.Top, FOwner.Width - FOwner.Left, FOwner.Height - FOwner.Top);
end;

destructor TTextureTile.Destroy;
begin
  if FBuffer <> nil then
    FreeMem(FBuffer);
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure TTextureTile.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TTextureTile.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

function TTextureTile.GetTexture: TTexture;
begin
  Result := FOwner.Texture;
end;

function TTextureTile.GetWidth: Integer;
begin
  Result := (FClientRect.Right - FClientRect.Left);
end;

function TTextureTile.GetHeight: Integer;
begin
  Result := (FClientRect.Bottom - FClientRect.Top);
end;

function TTextureTile.GetLeft: Integer;
begin
  Result := FClientRect.Left;
end;

function TTextureTile.GetTop: Integer;
begin
  Result := FClientRect.Top;
end;

procedure TTextureTile.SetLeft(Value: Integer);
var
  AWidth: Integer;
begin
  AWidth := FClientRect.Right - FClientRect.Left;
  FClientRect.Left := Min(Value, FOwner.Width);
  FClientRect.Right := Min(FClientRect.Left + AWidth, FOwner.Width);
  if FPatternWidth > 0 then
    FColCount := (FClientRect.Right - FClientRect.Left) div FPatternWidth;
  SetLength(FImageRect, FColCount * FRowCount);
end;

procedure TTextureTile.SetTop(Value: Integer);
var
  AHeight: Integer;
begin
  AHeight := FClientRect.Bottom - FClientRect.Top;
  FClientRect.Top := Min(Value, FOwner.Height);
  FClientRect.Bottom := Min(FClientRect.Top + AHeight, FOwner.Height);
  if FPatternHeight > 0 then
    FRowCount := (FClientRect.Bottom - FClientRect.Top) div FPatternHeight;
  SetLength(FImageRect, FColCount * FRowCount);
end;

procedure TTextureTile.SetWidth(Value: Integer);
begin
  FClientRect.Right := Min(FClientRect.Left + Value, FOwner.Width);
  if FPatternWidth > 0 then
    FColCount := (FClientRect.Right - FClientRect.Left) div FPatternWidth;
  SetLength(FImageRect, FColCount * FRowCount);
end;

procedure TTextureTile.SetHeight(Value: Integer);
begin
  FClientRect.Bottom := Min(FClientRect.Top + Value, FOwner.Height);
  if FPatternHeight > 0 then
    FRowCount := (FClientRect.Bottom - FClientRect.Top) div FPatternHeight;
  SetLength(FImageRect, FColCount * FRowCount);
end;

procedure TTextureTile.SetPatternWidth(Value: Integer);
begin
  if FPatternWidth <> Value then begin
    FPatternWidth := Min(Width, Value);
    if FPatternWidth > 0 then
      FColCount := (FClientRect.Right - FClientRect.Left) div FPatternWidth;
    SetLength(FImageRect, FColCount * FRowCount);
  end;
end;

procedure TTextureTile.SetPatternHeight(Value: Integer);
begin
  if FPatternHeight <> Value then begin
    FPatternHeight := Min(Height, Value);
    if FPatternHeight > 0 then
      FRowCount := (FClientRect.Bottom - FClientRect.Top) div FPatternHeight;
    SetLength(FImageRect, FColCount * FRowCount);
  end;
end;

procedure TTextureTile.Initialize;
{var
  Y: Integer;
  Bits: Pointer;
  Pitch: Integer;}
begin
  FInitialized := True;
  SetLength(FImageRect, FColCount * FRowCount);
  {if FBuffer <> nil then begin

    if FOwner.Texture.Lock(FClientRect, Bits, Pitch) then begin
      if (Bits <> nil) and (Pitch > 0) then begin
        for Y := 0 to Height - 1 do begin
          DesP := PByte(Integer(FBuffer) + Y * Width * 4);
          SrcP := PByte(Integer(Bits) + Y * Pitch);
          Move(SrcP^, DesP^, Pitch);
        end;
      end else begin
        ImageRect.Active := False;
      end;
      FOwner.Texture.Unlock;
    end else begin
      ImageRect.Active := False;
        //DebugOutStr(Format('(4)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
    end;

    FreeMem(FBuffer);
    FBuffer := nil;
  end;  }
end;

procedure TTextureTile.Finalize;
begin
  FInitialized := False;
  SetLength(FImageRect, 0);
end;

procedure TTextureTile.Draw(const X, Y: Integer; ImageIndexs: TImageIndexs);
var
  I, nX, nIndex: Integer;
  ImageRect: pTImageRect;
begin
  if FOwner.Texture <> nil then begin
    nX := X;
    for I := 0 to Length(ImageIndexs) - 1 do begin
      nIndex := ImageIndexs[I];
      if (nIndex >= 0) and (nIndex < Length(FImageRect)) then begin
        ImageRect := @FImageRect[nIndex];
        if (ImageRect.Rect.Right > ImageRect.Rect.Left) and (ImageRect.Rect.Bottom > ImageRect.Rect.Top) then
          GameCanvas.Draw(nX, Y, ImageRect.Rect, FOwner.Texture);
        Inc(nX, ImageRect.Rect.Right - ImageRect.Rect.Left);
      end;
    end;
  end;
end;

function TTextureTile.GetImageInfo(const Text: string): TImageInfo;
begin

end;

function TTextureTile.Add(const Text: string): pTImageRect;
begin
  Result := nil;
end;

procedure TTextureTile.TextOut(const X, Y: Integer; const Text: string; Color: TColor);
begin

end;

procedure TTextureTile.TextOut(const X, Y: Integer; ImageIndexs: TImageIndexs; Color: TColor);
begin

end;

function TTextureTile.Add(const X, Y: Integer; const ATexture: TTexture): pTImageRect;
var
  I, Index, nX, nY, AWidth, AHeight: Integer;
  SBits: Pointer;
  SPitch: Integer;

  DBits: Pointer;
  DPitch: Integer;
  DestRect, SrcRect: TRect;
  Left, Top: Integer;
  SrcP: PByte;
  DesP: PByte;
begin
  Result := nil;
  for I := 0 to Length(FImageRect) - 1 do begin
    if not FImageRect[I].Active then begin
      Result := @FImageRect[I];
      FImageRect[I].Active := True;
      FImageRect[I].Index := I;
      FImageRect[I].X := X;
      FImageRect[I].Y := Y;
      FImageRect[I].Time := GetTickCount;

      nX := I mod FColCount * FPatternWidth + FClientRect.Left;
      nY := I div FColCount * FPatternHeight + FClientRect.Top;

      AWidth := Min(FPatternWidth, ATexture.Width);
      AHeight := Min(FPatternHeight, ATexture.Height);
      SrcRect := Bounds(0, 0, AWidth, AHeight);
      DestRect := Bounds(nX, nY, AWidth, AHeight);
      FImageRect[I].Rect := DestRect;

      //DebugOutStr(Format('(1) I:%d Left:%d, Top:%d, Right:%d, Bottom:%d,FRowCount:%d,FColCount:%d', [I, DestRect.Left, DestRect.Top, DestRect.Right, DestRect.Bottom, FRowCount, FColCount]));
      if FOwner.Texture.Lock(DestRect, DBits, DPitch, False) and (DBits <> nil) and (DPitch > 0) then begin
        if ATexture.Lock(SrcRect, SBits, SPitch) and (SBits <> nil) and (SPitch > 0) then begin
          //DebugOutStr(Format('(2) I:%d Left:%d, Top:%d, AWidth:%d, AHeight:%d,FRowCount:%d,FColCount:%d', [I, Left, Top, AWidth, AHeight, FRowCount, FColCount]));
          for nY := 0 to AHeight - 1 do begin
            SrcP := PByte(Integer(SBits) + nY * SPitch);
            DesP := PByte(Integer(DBits) + nY * DPitch);
            Move(SrcP^, DesP^, Min(SPitch, DPitch));
          end;
        end;
        ATexture.Unlock;
      end;
      FOwner.Texture.Unlock;
      break;
    end;
  end;
end;

function TTextureTile.GetCount: Integer;
begin
  Result := Length(FImageRect);
end;

function TTextureTile.GetImages(Index: Integer): pTImageRect;
begin
  if (Index >= 0) and (Index < Length(FImageRect)) and FImageRect[Index].Active then begin
    Result := @FImageRect[Index];
  end else Result := nil;
end;

function TTextureTile.GetCachedImage(Index: Integer; var px, py: Integer; var ARect: TRect): TTexture;
begin
  if (FOwner.Texture <> nil) and (Index >= 0) and (Index < Length(FImageRect)) and FImageRect[Index].Active then begin
    Result := FOwner.Texture;
    ARect := FImageRect[Index].Rect;
    px := FImageRect[Index].X;
    py := FImageRect[Index].Y;
  end else Result := nil;
end;

function TTextureTile.GetActive: pTImageRect;
var
  I: Integer;
  nX, nY: Integer;
begin
  Result := nil;
  for I := 0 to Length(FImageRect) - 1 do begin
    if not FImageRect[I].Active then begin
      FImageRect[I].Active := True;
      FImageRect[I].Index := I;
      FImageRect[I].Time := GetTickCount;
      nX := I mod FColCount * FPatternWidth + FClientRect.Left;
      nY := I div FColCount * FPatternHeight + FClientRect.Top;
      FImageRect[I].Rect := Bounds(nX, nY, FPatternWidth, FPatternHeight);
      Result := @FImageRect[I];
      break;
    end;
  end;
end;


//------------------------------------------------------------------------------

constructor TTextureImages.Create();
begin
  InitializeCriticalSection(FCsList);
  InitializeCriticalSection(FCriticalSection);
  FLeft := 0;
  FTop := 0;
  FWidth := 2048;
  FHeight := 2048;
  FTexture := nil;
  Setlength(FTextureTiles, 0);
  FreeMemCheckTick := GetTickCount;
end;

destructor TTextureImages.Destroy;
var
  I: Integer;
begin
  for I := 0 to Length(FTextureTiles) - 1 do
    FTextureTiles[I].Free;
  Setlength(FTextureTiles, 0);

  if FTexture <> nil then
    FTexture.Free;
  DeleteCriticalSection(FCsList);
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure TTextureImages.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TTextureImages.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TTextureImages.ListLock;
begin
  EnterCriticalSection(FCsList);
end;

procedure TTextureImages.ListUnLock;
begin
  LeaveCriticalSection(FCsList);
end;

procedure TTextureImages.SetSize(const AWidth, AHeight: Integer);
begin
  if FTexture = nil then begin
    FWidth := AWidth;
    FHeight := AHeight;
  end;
end;

procedure TTextureImages.SetWidth(Value: Integer);
var
  I: Integer;
begin
  if FTexture = nil then begin
    if FWidth <> Value then begin
      FWidth := Value;
      for I := 0 to Length(FTextureTiles) - 1 do
        FTextureTiles[I].Width := FWidth;
    end;
  end;
end;

procedure TTextureImages.SetHeight(Value: Integer);
var
  I: Integer;
begin
  if FTexture = nil then begin
    if FHeight <> Value then begin
      FHeight := Value;
      for I := 0 to Length(FTextureTiles) - 1 do
        FTextureTiles[I].Height := FHeight;
    end;
  end;
end;


function TTextureImages.GetCount: Integer;
begin
  Result := Length(FTextureTiles);
end;

function TTextureImages.GetTile(Index: Integer): TTextureTile;
begin
  if (Index >= 0) and (Index < Length(FTextureTiles)) then
    Result := FTextureTiles[Index]
  else
    Result := nil;
end;

procedure TTextureImages.Initialize;
var
  I: Integer;
begin
  ListLock;
  try
    if FTexture = nil then
      FTexture := GameCanvas.HGE.Texture_Create(FWidth, FHeight);

    for I := 0 to Count - 1 do begin
      Tiles[I].Initialize;
    end;
  finally
    ListUnLock;
  end;
end;

procedure TTextureImages.Finalize;
var
  I: Integer;
begin
  ListLock;
  try
    for I := 0 to Count - 1 do begin
      Tiles[I].Finalize;
    end;

    if FTexture <> nil then
      FreeAndNil(FTexture);
  finally
    ListUnLock;
  end;
end;


function TTextureImages.Add(): TTextureTile;
begin
  Setlength(FTextureTiles, Length(FTextureTiles) + 1);
  FTextureTiles[Length(FTextureTiles) - 1] := TTextureTile.Create(Self);
  Result := FTextureTiles[Length(FTextureTiles) - 1];
end;

function TTextureImages.Add(TextureTile: TTextureTile): TTextureTile;
begin
  Setlength(FTextureTiles, Length(FTextureTiles) + 1);
  FTextureTiles[Length(FTextureTiles) - 1] := TextureTile;
  FTop := FTop + TextureTile.Height;
  Result := FTextureTiles[Length(FTextureTiles) - 1];
end;

procedure TTextureImages.FreeOldMemorys;
begin

end;


end.
