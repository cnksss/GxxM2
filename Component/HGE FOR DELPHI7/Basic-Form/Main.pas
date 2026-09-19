unit Main;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, AsphyreTimer, HGE, HGECanvas, HGEImages, StdCtrls;

type
  TFrmMain = class(TForm)
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormResize(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    procedure TimerEvent(Sender: TObject);
    procedure ProcessEvent(Sender: TObject);
    procedure RenderEvent(Sender: TObject);
    procedure CreateTiles;
  public
    { Public declarations }
  end;

var
  FrmMain: TFrmMain;
  GameCanvas: THGECanvas;


  Target: TTarget;
  Images: THGEimages;
  Font: TSysFont;
  TileIndexs: array[0..80, 0..80] of Integer;
  SpriteImage: THGEImages;
implementation

{$R *.dfm}

procedure DebugOutStr(Msg: string);
var
  flname: string;
  fhandle: TextFile;
begin
//DScreen.AddChatBoardString(msg,clWhite, clBlack);
  //exit;
  flname := '.\!debug.txt';
  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  Writeln(fhandle, Msg);
  CloseFile(fhandle);
end;

procedure TFrmMain.TimerEvent(Sender: TObject);
begin
  //GameCanvas.Render(RenderEvent, 0, True, Target);
  GameCanvas.Render(RenderEvent);
  //GameCanvas.Render(ProcessEvent);
  Timer.Process();
end;

procedure TFrmMain.ProcessEvent(Sender: TObject);
begin
  //GameCanvas.Draw(100, 100, Target);
end;

procedure TFrmMain.RenderEvent(Sender: TObject);
var
  I, J: Integer;
  Texture: TTexture;
begin
  for I := 0 to 80 do
  begin
    for J := 0 to 80 do
    begin
      Texture := Images.Items[TileIndexs[I, J]];
      if Texture <> nil then
        GameCanvas.Draw(Texture, 0, I * 64, J * 64, 1.0, BLEND_DEFAULT_Z);
    end;
  end;
  //GameCanvas.Draw(SpriteImage.Items[0], 0, 100, 100, 0.004, BLEND_DEFAULT_Z);
  {GameCanvas.DrawBlend(200, 150, SpriteImage.Items[1], 0.002);
  GameCanvas.DrawBlend(150, 210, SpriteImage.Items[2], 0.001);
  GameCanvas.DrawColor(150, 210, SpriteImage.Items[2], clRed, False, 0.001);
  GameCanvas.StretchDraw(Bounds(120, 210, 100, 100), SpriteImage.Items[3], True, 0.002);

  GameCanvas.FillRectAlpha(Bounds(120, 210, 200, 200), clRed, 120, 0.1); }
  GameCanvas.HGE.Gfx_SetClipping(119, 209, 100, 100);
  //GameCanvas.ClipRect:=Bounds(119, 209, 100, 100);
  GameCanvas.StretchDraw(Bounds(120, 210, 100, 100), SpriteImage.Items[3], True, 0.002);
  //GameCanvas.ClipRect:=Rect(0,0,0,0);
end;

procedure TFrmMain.CreateTiles;
var
  I, J: Integer;
begin
  for I := 0 to 80 do
  begin
    for J := 0 to 80 do
    begin
      TileIndexs[I, J] := Random(20);
    end;
  end;
end;

procedure TFrmMain.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  Images := THGEImages.Create;
  SpriteImage := THGEImages.Create;

  GameCanvas := THGECanvas.Create;

  GameCanvas.Windowed := True;
  GameCanvas.DepthStencil := True;
  GameCanvas.BitCount := 32;
  GameCanvas.Handle := Handle;
  GameCanvas.LogFileName := '.\!debug.txt';
  if GameCanvas.Initialize then begin
    //DebugOutStr('GameCanvas.UseDelphiWindow:'+BoolToStr(GameCanvas.UseDelphiWindow,True));
    //DebugOutStr('GameCanvas.Windowed:'+BoolToStr(GameCanvas.Windowed,True));
    //DebugOutStr('GameCanvas.ZBuffer:'+BoolToStr(GameCanvas.ZBuffer,True));
    //DebugOutStr('GameCanvas.Handle:'+IntToStr(GameCanvas.Handle)+' Handle:'+IntToStr(Handle));
    //showmessage('1');
    CreateTiles;
    for I := 0 to 20 do
      Images.LoadFromFile('Gfx\' + 't' + IntTostr(I) + '.png');
    Images.LoadFromFile('Gfx\img1.png');
    Images.LoadFromFile('Gfx\img1-2.png');
    Images.LoadFromFile('Gfx\img2.png');

    SpriteImage.LoadFromFile('Gfx\img1.png');
    SpriteImage.LoadFromFile('Gfx\img2.png');
    SpriteImage.LoadFromFile('Gfx\img1-2.png');
    SpriteImage.LoadFromFile('Gfx\Tree2.png');

    Target := GameCanvas.HGE.Target_Create(600, 400, True);
    //showmessage('2');
    Timer.OnTimer := TimerEvent;
    Timer.OnProcess := ProcessEvent;
    Timer.Speed := 60.0;
    Timer.MaxFPS := 20;
    Timer.Enabled := True;
    //Timer.IdleDone := False;
  end;
  //Showmessage(IntToStr(Blend_Default));
   //Showmessage(IntToStr(Blend_Default_Z));
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
begin
  Timer.Enabled := False;
  GameCanvas.Free;
end;

procedure TFrmMain.FormResize(Sender: TObject);
begin
  //GameCanvas.Resize(ClientWidth, ClientHeight);
end;

procedure TFrmMain.Button1Click(Sender: TObject);
begin
  CreateTiles;
end;

end.

