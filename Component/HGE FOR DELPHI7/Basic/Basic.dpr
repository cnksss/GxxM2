program Basic;

{$R *.res}

uses
  Windows, SysUtils, HGEImages, HGECanvas, HGEDef, HGE, HGESprite, HGESpriteEngine;

type
  TMonoSprite = class(TSprite)
  private
    FCounter: Double;
    FLife: Integer;
    FHit: Boolean;
  public
    procedure DoMove(const MoveCount: Single); override;
  end;

  TPlayerSprite = class(TSprite)
  public
    procedure DoCollision(const Sprite: TSprite); override;
    procedure DoMove(const MoveCount: Single); override;
  end;

var
  HGE2: IHGE = nil;
  Images: THGEimages;
  Canvas: THGECanvas;
  Font: TSysFont;
  SpriteEngine: TSpriteEngine;
  Tile: array[0..80, 0..80] of IHGESprite;
  PlayerSprite: TPlayerSprite;
  Sprites: array[0..2] of IHGESprite;
  SpriteImage: THGEImages;
  ZBuf: Single = 0.0;

procedure TMonoSprite.DoMove(const MoveCount: Single);
begin
  inherited;
  CollidePos := Point2(X + 80, Y + 80);
  FCounter := FCounter + 1;
  X := X + Sin256(Trunc(FCounter)) * 3;
  Y := Y + Cos256(Trunc(FCounter)) * 3;
  if FHit then
  begin
    FLife := FLife - 1;
    if FLife < 0 then Dead;
  end;
end;

procedure TPlayerSprite.DoCollision(const Sprite: TSprite);
begin
  if Sprite is TMonoSprite then
  begin
    with TMonoSprite(Sprite) do
    begin
      ImageName := 'img1-2';
      Collisioned := False;
      FHit := True;
    end;
  end;
end;

procedure TPlayerSprite.DoMove(const MoveCount: Single);
begin
  inherited;
  CollidePos := Point2(X + 50, Y + 50);



 { if HGE2.Input_GetKeyState(HGEK_UP) then
    Y := Y - 3;
  if HGE2.Input_GetKeyState(HGEK_Down) then
    Y := Y + 3;
  if HGE2.Input_GetKeyState(HGEK_Left) then
    X := X - 3;
  if HGE2.Input_GetKeyState(HGEK_Right) then
    X := X + 3;
  if HGE2.Input_GetKeyState(HGEK_F1) then
    X := X + 3;  }


  Collision;
  Engine.WorldX := X - 340;
  Engine.WorldY := Y - 250;
end;

procedure CreateTiles;
var
  I, J: Integer;
  Texture: ITexture;
begin
  for I := 0 to 80 do
  begin
    for J := 0 to 80 do
    begin
      Texture := Images.Items[Random(20)];
      Tile[I, J] := THGESprite.Create(Texture, Texture.PatternWidth, Texture.PatternHeight, Texture.PatternWidth, Texture.PatternHeight);
      Tile[I, J].SetZ(1.0);
    end;
  end;
end;

procedure CreateSprites;
var
  I: Integer;
begin
  for I := 0 to 200 do
  begin
    with TMonoSprite.Create(SpriteEngine) do
    begin
      ImageName := 'img1';
      Width := PatternWidth;
      Height := PatternHeight;
      Collisioned := True;
      CollideRadius := 80;
      X := Random(5000);
      Y := Random(5000);
      Z := 2;
      FCounter := Random(1000);
      FLife := 15;
      FHit := False;
    end;
  end;

  PlayerSprite := TPlayerSprite.Create(SpriteEngine);
  with TPlayerSprite(PlayerSprite) do
  begin
    ImageName := 'img2';
    Z := 2;
    X := 2560;
    Y := 2560;
    Collisioned := True;
    CollideRadius := 50;
  end;
end;

function FrameFunc: Boolean;
begin

 { if HGE2.Input_GetKeyState(HGEK_UP) then begin
    ZBuf := ZBuf + 0.1;
    if ZBuf > 1.0 then ZBuf := 0.0;
    Sprites[0].SetZ(ZBuf);
    Result := True;
  end;

  if HGE2.Input_GetKeyState(HGEK_Down) then begin
    ZBuf := ZBuf - 0.1;
    if ZBuf < 0.0 then ZBuf := 1.0;
    Sprites[0].SetZ(ZBuf);
    Result := True;
  end; }

  case HGE2.Input_GetKey of
    HGEK_ESCAPE:
      begin
        FreeAndNil(Canvas);
        FreeAndNil(Images);
        FreeAndNil(SpriteEngine);
        FreeAndNil(Font);
        Result := True;
        Exit;
      end;
    HGEK_UP: begin
        ZBuf := ZBuf + 0.1;
        if ZBuf > 1.0 then ZBuf := 0.0;
        Sprites[0].SetZ(ZBuf);
        Result := True;
        Exit;
      end;
    HGEK_Down: begin
        ZBuf := ZBuf - 0.1;
        if ZBuf < 0.0 then ZBuf := 1.0;
        Sprites[0].SetZ(ZBuf);
        Result := True;
        Exit;
      end;
  end;
  Result := False;
end;

function RenderFunc: Boolean;
var
  I, J: Integer;
begin
  HGE2.Gfx_BeginScene;
  HGE2.Gfx_Clear(0);
  //SpriteEngine.Draw;
  //SpriteEngine.Move(1);
  //SpriteEngine.Dead;

  for I := 0 to 80 do
  begin
    for J := 0 to 80 do
    begin
      Tile[I, J].Render(I * 64, J * 64);
    end;
  end;

  Canvas.Draw(SpriteImage.Items[0], 0, 100, 100, 0.001, BLEND_ALPHAADD);
  Canvas.Draw(SpriteImage.Items[1], 0, 200, 150, 0.002, BLEND_ALPHAADD);
  Canvas.Draw(SpriteImage.Items[2], 0, 150, 210, 0.003, Blend_SrcColor);

 // HGE2.Gfx_Clear(0);
 // Sprites[1].Render(200, 150);
//  Sprites[0].Render(100, 100);
 // Sprites[2].Render(150, 210);

  Font.Print(10, 10, Format('Z:%f', [ZBuf]));
  //Font.Print(100,100,IntToStr(HGE2.Timer_GetFPS));
  HGE2.Gfx_EndScene;
  Result := False;
end;

procedure Main;
var
  I: Integer;
begin
  HGE2 := HGECreate(HGE_VERSION);
  HGE2.System_SetState(HGE_FRAMEFUNC, FrameFunc);
  HGE2.System_SetState(HGE_RENDERFUNC, RenderFunc);
  HGE2.System_SetState(HGE_USESOUND, False);
  HGE2.System_SetState(HGE_HIDEMOUSE, False);
  HGE2.System_SetState(HGE_WINDOWED, True);

  HGE2.System_SetState(HGE_SCREENWIDTH, 800);
  HGE2.System_SetState(HGE_SCREENHEIGHT, 600);
  HGE2.System_SetState(HGE_SCREENBPP, 16);
  HGE2.System_SetState(HGE_FPS, 20);

  HGE2.System_SetState(HGE_ZBUFFER, True);



  //HGE2.System_SetState(HGE_TEXTUREFILTER,True);
  //HGE2.System_SetState(HGE_FPS, HGEFPS_VSYNC);
  //HGE2.System_SetState(HGE_SHOWSPLASH, False);
  Canvas := THGeCanvas.Create;
  Images := THGEImages.Create;
  SpriteImage := THGEImages.Create;
  SpriteEngine := TSpriteEngine.Create(nil);
  Spriteengine.Images := Images;
  SpriteEngine.Canvas := Canvas;

  Canvas.UseDelphiWindow:=False;

  if (HGE2.System_Initiate) then
  begin
    Font := TSysFont.Create;
    Font.CreateFont('arial', 15, []);
    for I := 0 to 20 do
      Images.LoadFromFile('Gfx\' + 't' + IntTostr(I) + '.png');
    Images.LoadFromFile('Gfx\img1.png');
    Images.LoadFromFile('Gfx\img1-2.png');
    Images.LoadFromFile('Gfx\img2.png');
    //CreateSprites;
    //SpriteImage.LoadFromFile('Gfx\img1.png');
    SpriteImage.LoadFromFile('Gfx\img1.png');
    SpriteImage.LoadFromFile('Gfx\img2.png');
    SpriteImage.LoadFromFile('Gfx\img1-2.png');
    Sprites[0] := THGESprite.Create(SpriteImage.Items[0], 256, 160, 160, 160);
    Sprites[1] := THGESprite.Create(SpriteImage.Items[1], 128, 100, 100, 100);
    Sprites[2] := THGESprite.Create(SpriteImage.Items[2], 256, 160, 160, 160);
    //Sprites[0].SetZ(0.2);
    //Sprites[1].SetZ(0.4);
   // Sprites[2].SetZ(0.3);

    //Sprites[0].SetZ(0.8,3);
    //Sprites[0].SetZ(0.5,3);
    CreateTiles;
    HGE2.System_Start;
  end
  else
    MessageBox(0, PChar(HGE2.System_GetErrorMessage), 'Error', MB_OK or MB_ICONERROR or MB_SYSTEMMODAL);

  HGE2.System_Shutdown;
  HGE2 := nil;
end;

begin
  //ReportMemoryLeaksOnShutdown := True;
  Main;
end.

