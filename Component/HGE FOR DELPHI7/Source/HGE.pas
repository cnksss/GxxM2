unit HGE;
(*
** Haaf's Game Engine 1.7
** Copyright (C) 2003-2007, Relish Games
** hge.relishgames.com
**
** Delphi conversion by Erik van Bilsen
*)

interface

uses
  Classes,
  Windows,
  SysUtils,
  DirectXGraphics,
  OpenJpeg,
  Graphics,
  D3DX81mo,
  MMSystem;

(****************************************************************************
 * HGE.h
 ****************************************************************************)
const
  IsMultiThreadRender = 0;

const
  HGE_VERSION = $180;
  IRad = 1 / 360;
  TwoPI = 2 * 3.14159265358;

  (*
  ** HGE Handle types
  *)

type
  IResource = interface
    ['{BAA2A47B-87B1-4D26-A8EF-AE49E3B2BC6F}']
    function GetHandle:Pointer;
    function GetSize:Longword;

    property Handle:Pointer read GetHandle;
    property Size:Longword read GetSize;
  end;

  (*
  ** Common math constants
  *)
const
  M_PI = 3.14159265358979323846;
  M_PI_2 = 1.57079632679489661923;
  M_PI_4 = 0.785398163397448309616;
  M_1_PI = 0.318309886183790671538;
  M_2_PI = 0.636619772367581343076;

const
  clWhite1:Cardinal = $FFFFFFFF;
  clBlack1:Cardinal = $FF000000;
  clMaroon1:Cardinal = $FF800000;
  clGreen1:Cardinal = $FF008000;
  clOlive1:Cardinal = $FF808000;
  clNavy1:Cardinal = $FF000080;
  clPurple1:Cardinal = $FF800080;
  clTeal1:Cardinal = $FF008080;
  clGray1:Cardinal = $FF808080;
  clSilver1:Cardinal = $FFC0C0C0;
  clRed1:Cardinal = $FFFF0000;
  clLime1:Cardinal = $FF00FF00;
  clYellow1:Cardinal = $FFFFFF00;
  clBlue1:Cardinal = $FF0000FF;
  clFuchsia1:Cardinal = $FFFF00FF;
  clAqua1:Cardinal = $FF00FFFF;
  clLtGray1:Cardinal = $FFC0C0C0;
  clDkGray1:Cardinal = $FF808080;
  clOpaque1:Cardinal = $00FFFFFF;
  clUnknown:Cardinal = $00000000;
  (*
  ** Hardware color macros
  *)
function DisplaceRB(Color:Cardinal):Cardinal; register;
function ARGB(const A, R, G, B:Byte):Longword;
function GetA(const Color:Longword):Byte;
function GetR(const Color:Longword):Byte;
function GetG(const Color:Longword):Byte;
function GetB(const Color:Longword):Byte;
function SetA(const Color:Longword; const A:Byte):Longword;
function SetR(const Color:Longword; const A:Byte):Longword;
function SetG(const Color:Longword; const A:Byte):Longword;
function SetB(const Color:Longword; const A:Byte):Longword;

//---------------------------------------------------------------------------
function MinMax2(Value, Min, Max:Integer):Integer;
function Min2(a, b:Integer):Integer;
function Max2(a, b:Integer):Integer;
function Min3(a, b, c:Integer):Integer;
function Max3(a, b, c:Integer):Integer;

//---------------------------------------------------------------------------
// Fixed point 24:8 math routines
//---------------------------------------------------------------------------
function iMul8(x, y:Integer):Integer;
function iCeil8(x:Integer):Integer;
function iDiv8(x, y:Integer):Integer;

//---------------------------------------------------------------------------
// Fixed point 16:16 math routines
//---------------------------------------------------------------------------
function iMul16(x, y:Integer):Integer;
function iCeil16(x:Integer):Integer;
function iDiv16(x, y:Integer):Integer;
(*
** HGE Blending constants
*)

function MyGetTickCount:DWORD; stdcall; external mmsyst name 'timeGetTime';
// function MyGetTickCount: DWORD; stdcall;; external kernel32 name 'GetTickCount';

const

  BLEND_COLORADD = 1;
  BLEND_COLORMUL = 0;
  BLEND_ALPHABLEND = 2;
  BLEND_ALPHAADD = 0;
  BLEND_ZWRITE = 4;
  BLEND_NOZWRITE = 0;

  Blend_Default = BLEND_COLORMUL or BLEND_ALPHABLEND or BLEND_NOZWRITE;
  Blend_Default_Z = BLEND_COLORMUL or BLEND_ALPHABLEND or BLEND_ZWRITE;

  Blend_Add = 100;
  Blend_SrcAlpha = 101;
  Blend_SrcAlphaAdd = 102;
  Blend_SrcColor = 103;
  Blend_SrcColorAdd = 104;
  Blend_Invert = 105;
  Blend_SrcBright = 106;
  Blend_Multiply = 107;
  Blend_InvMultiply = 108;
  Blend_MultiplyAlpha = 109;
  Blend_InvMultiplyAlpha = 110;
  Blend_DestBright = 111;
  Blend_InvSrcBright = 112;
  Blend_InvDestBright = 113;
  Blend_Bright = 114;
  Blend_BrightAdd = 115;
  Blend_GrayScale = 116;
  Blend_Light = 117;
  Blend_LightAdd = 118;
  Blend_Add2X = 119;
  Blend_OneColor = 120;
  Blend_XOR = 121;
  Blend_SrcAlphaColor = 122;

  fxNone = 123;
  fxBlend = 124;
  fxAnti = 125;


  {*
  ** HGE System state constants
  *}
type
  THGEBoolState = (
    HGE_WINDOWED = 12, // bool    run in window?    (default: false)
    HGE_ZBUFFER = 13, // bool    use z-buffer?    (default: false)
    HGE_TEXTUREFILTER = 28, // bool    texture filtering?  (default: true)

    HGE_USESOUND = 18, // bool    use BASS for sound?  (default: true)

    HGE_DONTSUSPEND = 24, // bool    focus lost:suspend?  (default: false)
    HGE_HIDEMOUSE = 25, // bool    hide system cursor?  (default: true)

    HGE_HARDWARE = 27, // bool		 hide system cursor?	(default: true)
    HGEBOOLSTATE_FORCE_DWORD = $7FFFFFFF
    );

type
  THGEFuncState = (
    HGE_FRAMEFUNC = 1, // bool*()  frame function    (default: NULL) (you MUST set this)
    HGE_RENDERFUNC = 2, // bool*()  render function    (default: NULL)
    HGE_FOCUSLOSTFUNC = 3, // bool*()  focus lost function  (default: NULL)
    HGE_FOCUSGAINFUNC = 4, // bool*()  focus gain function  (default: NULL)
    HGE_GFXRESTOREFUNC = 5, // bool*()	 exit function		(default: NULL)
    HGE_EXITFUNC = 6, // bool*()  exit function    (default: NULL)

    HGEFUNCSTATE_FORCE_DWORD = $7FFFFFFF
    );

type
  THGEHWndState = (
    HGE_HWND = 26, // int    window handle: read only
    HGE_HWNDPARENT = 27, // int    parent win handle  (default: 0)

    HGEHWNDSTATE_FORCE_DWORD = $7FFFFFFF
    );

type
  THGEIntState = (
    HGE_SCREENWIDTH = 9, // int    screen width    (default: 800)
    HGE_SCREENHEIGHT = 10, // int    screen height    (default: 600)
    HGE_SCREENBPP = 11, // int    screen bitdepth    (default: 32) (desktop bpp in windowed mode)

    HGE_SAMPLERATE = 19, // int    sample rate      (default: 44100)
    HGE_FXVOLUME = 20, // int    global fx volume  (default: 100)
    HGE_MUSVOLUME = 21, // int    global music volume  (default: 100)

    HGE_FPS = 23, // int    fixed fps      (default: HGEFPS_UNLIMITED)

    HGEINTSTATE_FORCE_DWORD = $7FFFFFF
    );

type
  THGEStringState = (
    HGE_ICON = 7, // char*  icon resource    (default: NULL)
    HGE_TITLE = 8, // char*  window title    (default: "HGE")

    HGE_INIFILE = 15, // char*  ini file      (default: NULL) (meaning no file)
    HGE_LOGFILE = 16, // char*  log file      (default: NULL) (meaning no file)

    HGESTRINGSTATE_FORCE_DWORD = $7FFFFFFF
    );

  (*
  ** Callback protoype used by HGE
  *)
type
  THGECallback = function:Boolean;

  (*
  ** HGE_FPS system state special constants
  *)
const
  HGEFPS_UNLIMITED = 0;
  HGEFPS_VSYNC = -1;

  (*
  ** HGE Primitive type constants
  *)
const
  HGEPRIM_LINES = 2;
  HGEPRIM_TRIPLES = 3;
  HGEPRIM_QUADS = 4;

  (*
  ** HGE Vertex structure
  *)
type
  THGEVertex = record
    X, Y:Single; // screen position
    Z:Single; // Z-buffer depth 0..1
    Col:Longword; // color
    TX, TY:Single; // texture coordinates
  end;
  PHGEVertex = ^THGEVertex;
  THGEVertexArray = array[0..MaxInt div 32 - 1] of THGEVertex;
  PHGEVertexArray = ^THGEVertexArray;

  (*
  ** HGE Input Event structure
  *)
type
  THGEInputEvent = record
    EventType:Integer; // event type
    Key:Integer; // key code
    Flags:Integer; // event flags
    Chr:Integer; // character code
    Wheel:Integer; // wheel shift
    X:Single; // mouse cursor x-coordinate
    Y:Single; // mouse cursor y-coordinate
  end;

  (*
  ** HGE Input Event type constants
  *)
const
  INPUT_KEYDOWN = 1;
  INPUT_KEYUP = 2;
  INPUT_MBUTTONDOWN = 3;
  INPUT_MBUTTONUP = 4;
  INPUT_MOUSEMOVE = 5;
  INPUT_MOUSEWHEEL = 6;

  (*
  ** HGE Input Event flags
  *)
const
  HGEINP_SHIFT = 1;
  HGEINP_CTRL = 2;
  HGEINP_ALT = 4;
  HGEINP_CAPSLOCK = 8;
  HGEINP_SCROLLLOCK = 16;
  HGEINP_NUMLOCK = 32;
  HGEINP_REPEAT = 64;

  (*
  ** HGE Virtual-key codes
  *)
const
  HGEK_LBUTTON = $01;
  HGEK_RBUTTON = $02;
  HGEK_MBUTTON = $04;

  HGEK_ESCAPE = $1B;
  HGEK_BACKSPACE = $08;
  HGEK_TAB = $09;
  HGEK_ENTER = $0D;
  HGEK_SPACE = $20;

  HGEK_SHIFT = $10;
  HGEK_CTRL = $11;
  HGEK_ALT = $12;

  HGEK_LWIN = $5B;
  HGEK_RWIN = $5C;
  HGEK_APPS = $5D;

  HGEK_PAUSE = $13;
  HGEK_CAPSLOCK = $14;
  HGEK_NUMLOCK = $90;
  HGEK_SCROLLLOCK = $91;

  HGEK_PGUP = $21;
  HGEK_PGDN = $22;
  HGEK_HOME = $24;
  HGEK_END = $23;
  HGEK_INSERT = $2D;
  HGEK_DELETE = $2E;

  HGEK_LEFT = $25;
  HGEK_UP = $26;
  HGEK_RIGHT = $27;
  HGEK_DOWN = $28;

  HGEK_0 = $30;
  HGEK_1 = $31;
  HGEK_2 = $32;
  HGEK_3 = $33;
  HGEK_4 = $34;
  HGEK_5 = $35;
  HGEK_6 = $36;
  HGEK_7 = $37;
  HGEK_8 = $38;
  HGEK_9 = $39;

  HGEK_A = $41;
  HGEK_B = $42;
  HGEK_C = $43;
  HGEK_D = $44;
  HGEK_E = $45;
  HGEK_F = $46;
  HGEK_G = $47;
  HGEK_H = $48;
  HGEK_I = $49;
  HGEK_J = $4A;
  HGEK_K = $4B;
  HGEK_L = $4C;
  HGEK_M = $4D;
  HGEK_N = $4E;
  HGEK_O = $4F;
  HGEK_P = $50;
  HGEK_Q = $51;
  HGEK_R = $52;
  HGEK_S = $53;
  HGEK_T = $54;
  HGEK_U = $55;
  HGEK_V = $56;
  HGEK_W = $57;
  HGEK_X = $58;
  HGEK_Y = $59;
  HGEK_Z = $5A;

  HGEK_GRAVE = $C0;
  HGEK_MINUS = $BD;
  HGEK_EQUALS = $BB;
  HGEK_BACKSLASH = $DC;
  HGEK_LBRACKET = $DB;
  HGEK_RBRACKET = $DD;
  HGEK_SEMICOLON = $BA;
  HGEK_APOSTROPHE = $DE;
  HGEK_COMMA = $BC;
  HGEK_PERIOD = $BE;
  HGEK_SLASH = $BF;

  HGEK_NUMPAD0 = $60;
  HGEK_NUMPAD1 = $61;
  HGEK_NUMPAD2 = $62;
  HGEK_NUMPAD3 = $63;
  HGEK_NUMPAD4 = $64;
  HGEK_NUMPAD5 = $65;
  HGEK_NUMPAD6 = $66;
  HGEK_NUMPAD7 = $67;
  HGEK_NUMPAD8 = $68;
  HGEK_NUMPAD9 = $69;

  HGEK_MULTIPLY = $6A;
  HGEK_DIVIDE = $6F;
  HGEK_ADD = $6B;
  HGEK_SUBTRACT = $6D;
  HGEK_DECIMAL = $6E;

  HGEK_F1 = $70;
  HGEK_F2 = $71;
  HGEK_F3 = $72;
  HGEK_F4 = $73;
  HGEK_F5 = $74;
  HGEK_F6 = $75;
  HGEK_F7 = $76;
  HGEK_F8 = $77;
  HGEK_F9 = $78;
  HGEK_F10 = $79;
  HGEK_F11 = $7A;
  HGEK_F12 = $7B;

type
  TSysFont = class
  private
    FFont:ID3DXFont;
    FName:string;
    FSize:integer;
    FStyle:TFontStyles;
    FRed:Byte;
    FBlue:Byte;
    FGreen:Byte;
    FAlpha:Byte;
    FColor:Cardinal;
    FCanvas:TCanvas;
    FTheFont:TFont;
    procedure SetColor(const Value:Cardinal);
  public
    destructor Destroy; override;
    function CreateFont(FontName:string; Size:Integer; Style:TFontStyles):Boolean; overload;
    function CreateFont(const Font:TFont):Boolean; overload;
    function TextHeight(const Text:string):Integer;
    function TextWidth(const Text:string):integer;
    procedure BeginFont;
    procedure EndFont;
    procedure Init;
    procedure UnInit;
    procedure PrintInvert(XPos, YPos:Integer; sString:string);
    procedure Print(XPos, YPos:Integer; sString:string; R, G, B, A:Byte); overload;
    procedure Print(XPos, YPos:Integer; sString:string); overload;
    procedure Print(Pos:TPoint; sString:string); overload;
    property Color:Cardinal read FColor write SetColor;
    property Red:Byte read FRed write FRed;
    property Blue:Byte read FBlue write FBlue;
    property Green:Byte read FGreen write FGreen;
    property Alpha:Byte read FAlpha write FAlpha;
  end;
const
  D3DFVF_HGEVERTEX = D3DFVF_XYZ or D3DFVF_DIFFUSE or D3DFVF_TEX1;
  VERTEX_BUFFER_SIZE = 6000; // 增大缓存区  4000 ---> 6000 chongchong 2018-02-27

type
  PResourceList = ^TResourceList;
  TResourceList = record
    Filename:string;
    // Password: String; // NOTE: ZIP passwords are not supported in Delphi version
    Next:PResourceList;
  end;

type
  PInputEventList = ^TInputEventList;
  TInputEventList = record
    Event:THGEInputEvent;
    Next:PInputEventList;
  end;

type
  TTexture = class(TObject)
  private
    FName:string;
    FPatternWidth:Integer;
    FPatternHeight:Integer;
    FPatternCount:Integer;
    FHandle:IDirect3DTexture8;
    FWidth:Integer;
    FHeight:Integer;
    FFormat:TD3DFormat;
    FSurfaceDesc:TD3DSurfaceDesc;
  protected

    function GetName:string;
    procedure SetName(Value:string);
    function GetClientRect:TRect;
    function GetPatternWidth:Integer;
    procedure SetPatternWidth(Value:Integer);
    function GetPatternHeight:Integer;
    procedure SetPatternHeight(Value:Integer);
    function GetPatternCount:Integer;

    function GetWidth(const Original:Boolean = False):Integer;
    function GetHeight(const Original:Boolean = False):Integer;
    function GetPixel(x, y:Integer):Cardinal;
    procedure SetPixel(x, y:Integer; const Value:Cardinal);
    function GetFormat:TD3DFormat;

    procedure SetFormat(Value:TD3DFormat);

  public
    constructor Create(const AHandle:IDirect3DTexture8;
      const AWidth, AHeight:Integer);
    destructor Destroy; override;
    procedure Restore; virtual;
    function Lock(const Rect:TRect; out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean = True):Boolean; overload;
    function Lock(out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean = True):Boolean; overload;
    procedure Unlock;

    property Name:string read GetName write SetName;
    property PatternWidth:Integer read GetPatternWidth write SetPatternWidth;
    property PatternHeight:Integer read GetPatternHeight write SetPatternHeight;
    property PatternCount:Integer read GetPatternCount;

    property Width:Integer read FWidth;
    property Height:Integer read FHeight;

    property Pixels[x, y:Integer]:Cardinal read GetPixel write SetPixel;
    property Handle:IDirect3DTexture8 read FHandle write FHandle;
    property Format:TD3DFormat read GetFormat write SetFormat;
    property ClientRect:TRect read GetClientRect;
  end;

type
  TTarget = class(TTexture)
  private
    FDepth:IDirect3DSurface8;
  protected

  public
    constructor Create(const AHandle:IDirect3DTexture8; const ADepth:IDirect3DSurface8; const AWidth, AHeight:Integer);
    destructor Destroy; override;
    procedure Restore; override;
    procedure Lost;
    property Depth:IDirect3DSurface8 read FDepth;
  end;
  (*
  ** HGE Triple structure
  *)
type
  THGETriple = record
    V:array[0..2] of THGEVertex;
    Tex:TTexture;
    Blend:Integer;
  end;
  PHGETriple = ^THGETriple;

  (*
  ** HGE Quad structure
  *)
type
  THGEQuad = record
    V:array[0..3] of THGEVertex;
    Tex:TTexture;
    Blend:Integer;
  end;
  PHGEQuad = ^THGEQuad;
  (*
  ** HGE Interface implementation
  *)
type
  THGE = class(TObject)

  private
    //////// Implementation ////////
    FVertexCache:Integer;
    FIndexCache:Integer;
    //FVertexCount: Integer;
    //FIndexCount: Integer;
    //FPrimitives: Integer;
    FMaxPrimitives:Integer;

    Vertices:array[0..1000] of THGEVertex;

    FWnd:HWnd;
    FActive:Boolean;
    FError:string;
    FAppPath:string;
    FFormatSettings:TFormatSettings;
    FCriticalSection:TRTLCriticalSection;
    procedure CopyVertices(pVertices:PByte; numVertices:integer);
    //procedure FocusChange(const Act: Boolean);
    procedure PostError(const Error:string);
  private
    // Extensions
    function Texture_LoadJPEG2000(const Data:Pointer; const Size:Longword;
      const Mipmap:Boolean; const Format:TOPJ_CodecFormat):TTexture;
  private
    // System States
    FProcFrameFunc:THGECallback;
    FProcRenderFunc:THGECallback;
    FProcFocusLostFunc:THGECallback;
    FProcFocusGainFunc:THGECallback;
    FProcGfxRestoreFunc:THGECallback;
    FProcExitFunc:THGECallback;

    FScreenWidth:Integer;
    FScreenHeight:Integer;
    FScreenBPP:Integer;
    FWindowed:Boolean;
    FZBuffer:Boolean;
    FHardware:Boolean;
    FTextureFilter:Boolean;
    FLogFilePath:string;
    FUseSound:Boolean;
    FSampleRate:Integer;
    FFXVolume:Integer;
    FMusVolume:Integer;
    FHGEFPS:Integer;
    FDontSuspend:Boolean;
    FWndParent:HWnd;

    FOnDeviceLost:TNotifyEvent;
    FOnDeviceReset:TNotifyEvent;
  private
    // Graphics
    FD3DPP:PD3DPresentParameters;
    FD3DPPW:TD3DPresentParameters;
    //FRectW: TRect;
    //FStyleW: Longword;
    FD3DPPFS:TD3DPresentParameters;
    //FRectFS: TRect;
    //FStyleFS: Longword;
    FD3D:IDirect3D8;
    FD3DDevice:IDirect3DDevice8;
    FVB:IDirect3DVertexBuffer8;
    FIB:IDirect3DIndexBuffer8;
    FScreenSurf:IDirect3DSurface8;
    FScreenDepth:IDirect3DSurface8;
    //FTargets: TList;
    FCurTarget:TTarget;
    FMatView:TD3DXMatrix;
    FMatProj:TD3DXMatrix;
    FVertArray:PHGEVertexArray;
    FPrim:Integer;
    FCurPrimType:Integer;
    FCurBlendMode:Integer;
    FCurTexture:TTexture;

    FD3DCaps8:TD3DCaps8;
    FGetD3DCaps:Boolean;

    function GfxInit:Boolean;
    procedure GfxDone;
    function GfxRestore:Boolean;
    function InitLost:Boolean;

    function FormatId(const Fmt:TD3DFormat):Integer;
    procedure SetBlendMode(const Blend:Integer);
    procedure SetProjectionMatrix(const Width, Height:Integer);
    function GetD3DCaps8:PD3DCaps8;
  private
    // Resource
    FTmpFilename:string;
    FRes:PResourceList;
    FSearch:TSearchRec;

    FIsBeginScene:Boolean; // 顶点缓冲区一次锁定用 chongchong 2018-02-27
  public
    constructor Create;
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    function System_Initiate:Boolean;
    procedure System_Shutdown;
    function System_GetErrorMessage:string;
    procedure System_Log(const S:string); overload;
    procedure System_Log(const Format:string; const Args:array of const); overload;
    procedure System_Log2(const Format:string; const Args:array of const);
    function System_Launch(const Url:string):Boolean;
    procedure System_Snapshot(const Filename:string);
    procedure System_SetState(const State:THGEBoolState; const Value:Boolean); overload;
    procedure System_SetState(const State:THGEFuncState; const Value:THGECallback); overload;
    procedure System_SetState(const State:THGEHWndState; const Value:HWnd); overload;
    procedure System_SetState(const State:THGEIntState; const Value:Integer); overload;
    procedure System_SetState(const State:THGEStringState; const Value:string); overload;
    function System_GetState(const State:THGEBoolState):Boolean; overload;
    function System_GetState(const State:THGEFuncState):THGECallback; overload;
    function System_GetState(const State:THGEHWndState):HWnd; overload;
    function System_GetState(const State:THGEIntState):Integer; overload;
    function System_GetState(const State:THGEStringState):string; overload;

    procedure Resize(const Width, Height:Integer);

    function Resource_Load(const Filename:string; const Size:PLongword = nil):IResource;
    function Resource_AttachPack(const Filename:string):Boolean;
    procedure Resource_RemovePack(const Filename:string);
    procedure Resource_RemoveAllPacks;
    function Resource_MakePath(const Filename:string = ''):string;
    function Resource_EnumFiles(const Wildcard:string = ''):string;
    function Resource_EnumFolders(const Wildcard:string = ''):string;
    procedure RenderBatch(const EndScene:Boolean = False);
    function Gfx_CanBegin():Boolean;
    function Gfx_BeginScene(const Target:TTarget = nil):Boolean;
    procedure Gfx_EndScene;
    procedure Gfx_Clear(const Color:Longword);
    procedure Gfx_RenderLine(const X1, Y1, X2, Y2:Single;
      const Color:Longword = $FFFFFFFF; const Z:Single = 0.5);
    procedure Gfx_RenderTriple(const Triple:THGETriple);
    procedure Gfx_RenderQuad(const Quad:THGEQuad);
    function Gfx_StartBatch(const PrimType:Integer; const Tex:TTexture;
      const Blend:Integer; out MaxPrim:Integer):PHGEVertexArray;
    procedure Gfx_FinishBatch(const NPrim:Integer);
    procedure Gfx_SetClipping(X:Integer = 0; Y:Integer = 0;
      W:Integer = 0; H:Integer = 0);
    procedure Gfx_GetClipping(out X, Y, W, H:Integer);

    procedure Gfx_SetTransform(const X:Single = 0; const Y:Single = 0;
      const DX:Single = 0; const DY:Single = 0; const Rot:Single = 0;
      const HScale:Single = 0; const VScale:Single = 0);
    procedure AddTarget(Target:TTarget);
    procedure RemoveTarget(Target:TTarget);
    function Target_Create(const Width, Height:Integer; const ZBuffer:Boolean):TTarget;
    function Target_GetTexture(const Target:TTarget):TTexture;

    function Texture_Create(const Width, Height:Integer; AFormat:TD3DFormat = D3DFMT_A8R8G8B8):TTexture;
    function Texture_Load(const Data:Pointer; const Size:Longword;
      const Mipmap:Boolean = False):TTexture; overload;
    function Texture_Load(const Filename:string;
      const Mipmap:Boolean = False):TTexture; overload;

    function Texture_LoadDDS(const AFormat:TD3DFormat; const Data:Pointer; const Size:Longword;
      const Mipmap:Boolean):TTexture;

    function Texture_GetWidth(const Tex:TTexture; const Original:Boolean = False):Integer;
    function Texture_GetHeight(const Tex:TTexture; const Original:Boolean = False):Integer;

    function Texture_Lock(const Tex:TTexture; const Rect:TRect; out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean = True):Boolean; overload;
    function Texture_Lock(const Tex:TTexture; out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean = True):Boolean; overload;

    procedure Texture_Unlock(const Tex:TTexture);

    { Extensions }
    function Texture_Load(const ImageData:Pointer; const ImageSize:Longword;
      const AlphaData:Pointer; const AlphaSize:Longword;
      const Mipmap:Boolean = False):TTexture; overload;
    function Texture_Load(const ImageFilename, AlphaFilename:string;
      const Mipmap:Boolean = False):TTexture; overload;
    //2008 Nov
    procedure SetGamma(Red, Green, Blue, Brightness, Contrast:Byte);
    procedure Point(X, Y:Single; Color:Cardinal; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Line2Color(X1, Y1, X2, Y2:Single; Color1, Color2:Cardinal; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Circle(X, Y, Radius:Single; Color:Cardinal; Filled:Boolean;
      Z:Single = 0.0; BlendMode:Integer = BLEND_Default); overload;
    procedure Circle(X, Y, Radius:Single; Width:Integer; Color:Cardinal;
      Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Ellipse(X, Y, R1, R2:Single; Color:Cardinal; Filled:Boolean;
      Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Arc(X, Y, Radius, StartRadius, EndRadius:Single; Color:Cardinal;
      DrawStartEnd, Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Arc(X, Y, Radius, StartRadius, EndRadius:Single; Color:Cardinal;
      Width:Integer; DrawStartEnd, Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Triangle(X1, Y1, X2, Y2, X3, Y3:Single; Color:Cardinal;
      Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Triangle(X1, Y1, X2, Y2, X3, Y3:Single; Color1, Color2, Color3:Cardinal;
      Filled:Boolean; Z:Single; BlendMODE:Integer); overload;
    procedure Quadrangle4Color(X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
      Color1, Color2, Color3, Color4:Cardinal; Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Quadrangle(X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single; Color:Cardinal;
      Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Rectangle(X, Y, Width, Height:Single; Color:Cardinal; Filled:Boolean;
      Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
    procedure Polygon(Points:array of TPoint; NumPoints:Integer; Color:Cardinal;
      Filled:Boolean; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Polygon(Points:array of TPoint; NumPoints:Integer; Color:Cardinal;
      Filled:Boolean; Tex:TTexture; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;
    procedure Polygon(Points:array of TPoint; Color:Cardinal; Filled:Boolean;
      Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT); overload;

    property D3DCaps8:PD3DCaps8 read GetD3DCaps8;

    property OnDeviceLost:TNotifyEvent read FOnDeviceLost write FOnDeviceLost;
    property OnDeviceReset:TNotifyEvent read FOnDeviceReset write FOnDeviceReset;
    property ZBuffer:Boolean read FZBuffer;
  end;

type
  TResource = class(TInterfacedObject, IResource)
  private
    FHandle:Pointer;
    FSize:Longword;
  protected
    { IResource }
    function GetHandle:Pointer;
    function GetSize:Longword;
  public
    constructor Create(const AHandle:Pointer; const ASize:Longword);
    destructor Destroy; override;
  end;

function HGECreate(const Ver:Integer):THGE;
var
  PHGE:THGE = nil;

implementation

uses
  Messages,
  Math,
  ShellAPI,
  Types,
  ZLib,
  ZipUtils,
  UnZip;

const
  CRLF = #13#10;

  MaxCachedPrimitives = 3072;
  MaxCachedIndices = 4096;
  MaxCachedVertices = 4096;
  (****************************************************************************
   * HGE.h - Macro implementations
   ****************************************************************************)

function DisplaceRB(Color:Cardinal):Cardinal; register;
asm
 mov ecx, eax
 mov edx, eax
 and eax, 0FF00FF00h
 and edx, 0000000FFh
 shl edx, 16
 or eax, edx
 mov edx, ecx
 shr edx, 16
 and edx, 0000000FFh
 or eax, edx
end;

function ARGB(const A, R, G, B:Byte):Longword;
begin
  Result := (A shl 24) or (R shl 16) or (G shl 8) or B;
end;

function GetA(const Color:Longword):Byte;
begin
  Result := Color shr 24;
end;

function GetR(const Color:Longword):Byte;
begin
  Result := (Color shr 16) and $FF;
end;

function GetG(const Color:Longword):Byte;
begin
  Result := (Color shr 8) and $FF;
end;

function GetB(const Color:Longword):Byte;
begin
  Result := Color and $FF;
end;

function SetA(const Color:Longword; const A:Byte):Longword;
begin
  Result := (Color and $00FFFFFF) or (A shl 24);
end;

function SetR(const Color:Longword; const A:Byte):Longword;
begin
  Result := (Color and $FF00FFFF) or (A shl 16);
end;

function SetG(const Color:Longword; const A:Byte):Longword;
begin
  Result := (Color and $FFFF00FF) or (A shl 8);
end;

function SetB(const Color:Longword; const A:Byte):Longword;
begin
  Result := (Color and $FFFFFF00) or A;
end;

//---------------------------------------------------------------------------

function MinMax2(Value, Min, Max:Integer):Integer;
asm { params: eax, edx, ecx }
 cmp eax, edx
 cmovl eax, edx
 cmp eax, ecx
 cmovg eax, ecx
end;

//---------------------------------------------------------------------------

function Min2(a, b:Integer):Integer;
asm { params: eax, edx }
 cmp   edx, eax
 cmovl eax, edx
end;

//---------------------------------------------------------------------------

function Max2(a, b:Integer):Integer;
asm { params: eax, edx }
 cmp   edx, eax
 cmovg eax, edx
end;

//---------------------------------------------------------------------------

function Min3(a, b, c:Integer):Integer;
asm { params: eax, edx, ecx }
 cmp   edx, eax
 cmovl eax, edx
 cmp   ecx, eax
 cmovl eax, ecx
end;

//---------------------------------------------------------------------------

function Max3(a, b, c:Integer):Integer;
asm { params: eax, edx, ecx }
 cmp   edx, eax
 cmovg eax, edx
 cmp   ecx, eax
 cmovg eax, ecx
end;

//---------------------------------------------------------------------------

function iMul8(x, y:Integer):Integer;
asm { params: eax, edx }
 imul edx
 shrd eax, edx, 8
end;

//---------------------------------------------------------------------------

function iCeil8(x:Integer):Integer;
asm
 add eax, $FF
 sar eax, 8
end;

//---------------------------------------------------------------------------

function iDiv8(x, y:Integer):Integer;
asm { params: eax, edx }
 mov ecx, edx
 mov edx, eax
 sar edx, 24
 shl eax, 8
 idiv ecx
end;

//---------------------------------------------------------------------------

function iMul16(x, y:Integer):Integer;
asm { params: eax, edx }
 imul edx
 shrd eax, edx, 16
end;

//---------------------------------------------------------------------------

function iCeil16(x:Integer):Integer;
asm
 add eax, $FFFF
 sar eax, 16
end;

//---------------------------------------------------------------------------

function iDiv16(x, y:Integer):Integer;
asm { params: eax, edx }
 mov ecx, edx
 mov edx, eax
 sar edx, 16
 shl eax, 16
 idiv ecx
end;

(****************************************************************************
 * HGE_Impl.h
 ****************************************************************************)

function FailString(Status:HRESULT):string;
begin
  case Status of
    D3DERR_WRONGTEXTUREFORMAT:Result := 'D3DERR_WRONGTEXTUREFORMAT';
    D3DERR_UNSUPPORTEDCOLOROPERATION:Result := 'D3DERR_UNSUPPORTEDCOLOROPERATION';
    D3DERR_UNSUPPORTEDCOLORARG:Result := 'D3DERR_UNSUPPORTEDCOLORARG';
    D3DERR_UNSUPPORTEDALPHAOPERATION:Result := 'D3DERR_UNSUPPORTEDALPHAOPERATION';
    D3DERR_UNSUPPORTEDALPHAARG:Result := 'D3DERR_UNSUPPORTEDALPHAARG';
    D3DERR_TOOMANYOPERATIONS:Result := 'D3DERR_TOOMANYOPERATIONS';
    D3DERR_CONFLICTINGTEXTUREFILTER:Result := 'D3DERR_CONFLICTINGTEXTUREFILTER';
    D3DERR_UNSUPPORTEDFACTORVALUE:Result := 'D3DERR_UNSUPPORTEDFACTORVALUE';
    D3DERR_CONFLICTINGRENDERSTATE:Result := 'D3DERR_CONFLICTINGRENDERSTATE';
    D3DERR_UNSUPPORTEDTEXTUREFILTER:Result := 'D3DERR_UNSUPPORTEDTEXTUREFILTER';
    D3DERR_CONFLICTINGTEXTUREPALETTE:Result := 'D3DERR_CONFLICTINGTEXTUREPALETTE';
    D3DERR_DRIVERINTERNALERROR:Result := 'D3DERR_DRIVERINTERNALERROR';

    D3DERR_NOTFOUND:Result := 'D3DERR_NOTFOUND';
    D3DERR_MOREDATA:Result := 'D3DERR_MOREDATA';
    D3DERR_DEVICELOST:Result := 'D3DERR_DEVICELOST';
    D3DERR_DEVICENOTRESET:Result := 'D3DERR_DEVICENOTRESET';
    D3DERR_NOTAVAILABLE:Result := 'D3DERR_NOTAVAILABLE';
    D3DERR_OUTOFVIDEOMEMORY:Result := 'D3DERR_OUTOFVIDEOMEMORY';
    D3DERR_INVALIDDEVICE:Result := 'D3DERR_INVALIDDEVICE';
    D3DERR_INVALIDCALL:Result := 'D3DERR_INVALIDCALL';
    D3DERR_DRIVERINVALIDCALL:Result := 'D3DERR_DRIVERINVALIDCALL';
    else Result := 'UNKNOW';
  end;

  {
D3DOK_NOAUTOGEN
 这是一个成功的代码。但是，这种格式不支持纹理的自动生成。这意味着资源创建将会成功，但纹理色阶将不会自动生成。

D3DERR_CONFLICTINGRENDERSTATE
 与当前设置的渲染状态不能同时使用。

D3DERR_CONFLICTINGTEXTUREFILTER
 与当前纹理筛选器不能同时使用。
 
D3DERR_CONFLICTINGTEXTUREPALETTE
 与当前纹理不能同时使用。
 
D3DERR_DEVICEHUNG
 设备返回此代码是由于操作系统重置了硬件适配器。大多数应用程序应当销毁该设备，并退出。应用程序必须继续应销毁所有视频内存对象 （表面，纹理，状态块等)，并调用 Reset() 将该设备的重置为默认状态。如果应用程序随后继续以同样的方式渲染，设备将返回此状态。
 
D3DERR_DEVICELOST
 设备已经丢失，但不能在这个时候被重置。因此，渲染是不合理的。一个Direct 3D设备对象而不是这个返回的代码引起的硬件适配器被操作系统重置。删除所有视频内存对象 （表面、 纹理、 状态块），并调用 Reset() 以返回到设备默认状态。如果应用程序继续渲染而不重置，渲染调用将失败。
 
D3DERR_DEVICENOTRESET
 设备已丢失，但在这个时候可以重置。
 
D3DERR_DEVICEREMOVED
 硬件适配器已被删除。应用程序必须销毁该设备、 进行枚举的显卡和创建另一个 D3D 设备。如果应用程序无需调用 Reset 继续渲染，渲染调用将失败。
 
D3DERR_DRIVERINTERNALERROR
 内部驱动程序错误。当收到此错误时应用程序应销毁，并重新创建该设备。调试此错误的提示请参阅驱动程序内部错误 (Direct3D 9)。

D3DERR_DRIVERINVALIDCALL
 未使用。
 
D3DERR_INVALIDCALL
 方法调用无效。例如一个方法的参数的无效指针。

D3DERR_INVALIDDEVICE
 请求的设备类型不是有效的。
 
D3DERR_MOREDATA
 可支持比指定的缓冲区大小更多的有效数据。
 
D3DERR_NOTAVAILABLE
 此设备不支持查询的技术。
 
D3DERR_NOTFOUND
 找不到的请求的项目。
 
D3D_OK
 没有错误产生。
 
D3DERR_OUTOFVIDEOMEMORY
 Direct3D的没有足够的显存来执行操作。该设备使用一个场景中的更多资源不能同时容纳视频内存。IDirect3DDevice9::Present、 IDirect3DDevice9Ex::PresentEx 或 IDirect3DDevice9Ex::CheckDeviceState 可以返回此错误。恢复类似于 D3DERR_DEVICEHUNG，虽然该应用程序可能需要降低其每个帧内存使用情况，并且为了避免再发生该错误。

D3DERR_TOOMANYOPERATIONS
 这个应用程序请求设备支持更多的纹理过滤操作。
 
D3DERR_UNSUPPORTEDALPHAARG
 该设备不支持alpha通道中指定的纹理混合的参数。
 
D3DERR_UNSUPPORTEDALPHAOPERATION
 该设备不支持alpha 通道中指定的纹理混合的操作 。
 
D3DERR_UNSUPPORTEDCOLORARG
 该设备不支持指定的纹理混合的颜色值的参数。

D3DERR_UNSUPPORTEDCOLOROPERATION
 该设备不支持指定的纹理混合操作的颜色值。
 
D3DERR_UNSUPPORTEDFACTORVALUE
 该设备不支持指定的纹理因子的值。未使用； 提供仅以支持较旧的驱动程序。
 
D3DERR_UNSUPPORTEDTEXTUREFILTER
 该设备不支持指定的纹理过滤器。
 
D3DERR_WASSTILLDRAWING
  这表明之前的位传递操作所传递的信息或表面是不完整的。
 
D3DERR_WRONGTEXTUREFORMAT
 纹理表面的像素格式不正确。
 
E_FAIL
 Direct3D子系统内发生未确定的错误。
 
E_INVALIDARG
 一个无效的参数传递给该函数的返回值。
 
E_INVALIDCALL
 方法调用是无效的。例如，一个方法的参数，可能有一个无效值。
 
E_NOINTERFACE
 没有可用的对象接口。
 
E_NOTIMPL
 未执行。
 
E_OUTOFMEMORY
 Direct3D 无法分配足够的内存来完成调用。

S_OK
 没有错误产生。
  }
end;
{TSysFont}

function TSysFont.CreateFont(FontName:string; Size:Integer; Style:TFontStyles):Boolean;
begin
  FName := FontName;
  FSize := Size;
  FStyle := Style;
  FRed := 255;
  FGreen := 255;
  FBlue := 255;
  FAlpha := 255;
  Result := True;
  UnInit;
  Init;
end;

function TSysFont.CreateFont(const Font:TFont):Boolean;
begin
  if not Assigned(Font) then
    raise Exception.Create('CreateFont() had unassigned Font param.');
  Result := CreateFont(Font.Name, Font.Size, Font.Style);
end;

procedure TSysFont.BeginFont;
begin
  if not Assigned(FFont) then Exit;
  FFont._Begin;
end;

procedure TSysFont.SetColor(const Value:Cardinal);
begin
  FColor := Value;
  Red := SetR(FColor, FRed);
  Blue := SetB(FColor, FBlue);
  Green := SetG(FColor, FGreen);
  Alpha := SetA(FColor, FAlpha);
end;

procedure TSysFont.EndFont;
begin
  if not Assigned(FFont) then Exit;
  FFont._End;
end;

procedure TSysFont.Print(XPos, YPos:Integer; sString:string; R, G, B, A:Byte);
var
  Rect:TRect;
begin
  if not Assigned(FFont) then Exit;
  Rect.Left := XPos;
  Rect.Top := YPos;
  Rect.Bottom := 0;
  Rect.Right := 0;
  FFont.DrawTextA(PChar(sString), -1, Rect, DT_NOCLIP, D3dColor_RGBA(R, G, B, A));
end;

procedure TSysFont.Print(XPos, YPos:Integer; sString:string);
begin
  Print(XPos, YPos, sString, FRed, FGreen, FBlue, FAlpha);
end;

procedure TSysFont.Print(Pos:TPoint; sString:string);
begin
  Print(Pos.X, Pos.Y, sString);
end;

procedure TSysFont.PrintInvert(XPos, YPos:Integer; sString:string);
begin
  Print(XPos, YPos, sString, (255 - FRed), (255 - FGreen), (255 - FBlue), FAlpha);
end;

destructor TSysFont.Destroy;
begin
  UnInit;
  inherited Destroy;
end;

function TSysFont.TextHeight(const Text:string):Integer;
begin
  FCanvas.Font := FTheFont;
  Result := FCanvas.TextHeight(Text);
end;

function TSysFont.TextWidth(const Text:string):Integer;
begin
  FCanvas.Font := FTheFont;
  Result := FCanvas.TextWidth(Text);
end;

procedure TSysFont.Init;
var
  oFont:TFont;
begin
  oFont := TFont.Create;
  try
    oFont.Name := FName;
    oFont.Size := FSize;
    oFont.Style := FStyle;
    D3DXCreateFont(PHGE.FD3DDevice, oFont.Handle, FFont);
  finally
    oFont.Free;
  end;

  FCanvas := TCanvas.Create;
  FCanvas.Handle := GetWindowDC(PHGE.System_GetState(HGE_HWND));
  FTheFont := TFont.Create;
  FTheFont.Name := FName;
  FTheFont.Size := FSize;
  FTheFont.Style := FStyle;
end;

procedure TSysFont.UnInit;
begin
  if Assigned(FFont) then FFont := nil;
  if Assigned(FCanvas) then begin
    FCanvas.Free;
    FCanvas := nil;
  end;
  if Assigned(FTheFont) then begin
    FTheFont.Free;
    FTheFont := nil;
  end;
end;

{ TTexture }

constructor TTexture.Create(const AHandle:IDirect3DTexture8;
  const AWidth, AHeight:Integer);
//var
 // Desc: TD3DSurfaceDesc;
begin
  FFormat := D3DFMT_A8R8G8B8;
  FHandle := AHandle;
  FWidth := AWidth;
  FHeight := AHeight;
  FillChar(FSurfaceDesc, SizeOf(TD3DSurfaceDesc), 0);
  if Assigned(FHandle) and Succeeded(FHandle.GetLevelDesc(0, FSurfaceDesc)) then
    FFormat := FSurfaceDesc.Format;
end;

destructor TTexture.Destroy;
begin
  if Assigned(FHandle) then
    FHandle := nil;
  inherited;
end;

procedure TTexture.Restore;
begin
  if Assigned(FHandle) then
    FHandle := nil;
end;

function TTexture.GetFormat:TD3DFormat;
begin
  Result := FFormat;
end;

procedure TTexture.SetFormat(Value:TD3DFormat);
begin
  FFormat := Value;
end;

function TTexture.GetName:string;
begin
  Result := FName;
end;

procedure TTexture.SetName(Value:string);
begin
  FName := Value;
end;

function TTexture.GetPatternWidth:Integer;
begin
  Result := FPatternWidth;
end;

procedure TTexture.SetPatternWidth(Value:Integer);
begin
  FPatternWidth := Value;
end;

function TTexture.GetPatternHeight:Integer;
begin
  Result := FPatternHeight;
end;

procedure TTexture.SetPatternHeight(Value:Integer);
begin
  FPatternHeight := Value;
end;

function TTexture.GetClientRect:TRect;
begin
  Result := Rect(0, 0, FWidth, FHeight);
end;

function TTexture.GetPatternCount:Integer;
var
  RowCount, ColCount:Integer;
begin
  ColCount := Self.GetWidth(True) div FPatternWidth;
  RowCount := Self.GetHeight(True) div FPatternHeight;
  if Self.FPatternCount < 0 then Self.FPatternCount := 0;
  //Make drawrect point to last rectangle if it is higher
  //if Self.FPatternCount > RowCount * ColCount then
  Result := RowCount * ColCount;
end;

function TTexture.GetHeight(const Original:Boolean):Integer;
//var
  //Desc: TD3DSurfaceDesc;
begin
  if (Original) then
    Result := FHeight
  else
    if FSurfaceDesc.Height > 0 then
      Result := FSurfaceDesc.Height
    else
      if Assigned(FHandle) and (Succeeded(FHandle.GetLevelDesc(0, FSurfaceDesc))) then
        Result := FSurfaceDesc.Height
      else
        Result := 0;
end;

function TTexture.GetWidth(const Original:Boolean):Integer;
//var
 // Desc: TD3DSurfaceDesc;
begin
  if (Original) then
    Result := FWidth
  else
    if FSurfaceDesc.Width > 0 then
      Result := FSurfaceDesc.Width
    else
      if Assigned(FHandle) and (Succeeded(FHandle.GetLevelDesc(0, FSurfaceDesc))) then
        Result := FSurfaceDesc.Width
      else
        Result := 0;
end;

function TTexture.GetPixel(x, y:Integer):Cardinal;
var
  Bits:Pointer;
  Pitch:Integer;
begin
  Result := 0;
  if (x < 0) or (y < 0) or (x >= FWidth) or (y >= FHeight) then Exit;
  if Lock(Bounds(0, y, FWidth, 1), Bits, Pitch, True) then begin
    case FFormat of
      D3DFMT_R8G8B8,
        D3DFMT_X8R8G8B8:Result := PCardinal(Integer(Bits) + (x * 4))^ or $FF000000;
      D3DFMT_A8R8G8B8:Result := PCardinal(Integer(Bits) + (x * 4))^;
      D3DFMT_R5G6B5, D3DFMT_X1R5G5B5, D3DFMT_A1R5G5B5:Result := PCardinal(Integer(Bits) + (x * 2))^;
    end;
    Unlock();
  end;
end;

procedure TTexture.SetPixel(x, y:Integer; const Value:Cardinal);
var
  Bits:Pointer;
  Pitch:Integer;
begin
  if (x < 0) or (y < 0) or (x >= FWidth) or (y >= FHeight) then Exit;
  if Lock(Bounds(0, y, FWidth, 1), Bits, Pitch, False) then begin
    case FFormat of
      D3DFMT_R8G8B8,
        D3DFMT_X8R8G8B8:PCardinal(Integer(Bits) + (x * 4))^ := Value;
      D3DFMT_A8R8G8B8:PCardinal(Integer(Bits) + (x * 4))^ := Value;
      D3DFMT_R5G6B5, D3DFMT_X1R5G5B5, D3DFMT_A1R5G5B5:PCardinal(Integer(Bits) + (x * 2))^ := Value;
    end;
    Unlock();
  end;
end;

function TTexture.Lock(const Rect:TRect; out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean):Boolean;
var
  Desc:TD3DSurfaceDesc;
  LockedRect:TD3DLockedRect;
  Flags:Integer;
  RectPtr:Pointer;
begin
  Result := False;
  Bits := nil;
  Pitch := 0;
  if (not Assigned(FHandle)) or (Failed(FHandle.GetLevelDesc(0, Desc))) then Exit;
  if (Desc.Format <> D3DFMT_A8R8G8B8) and (Desc.Format <> D3DFMT_X8R8G8B8) and (Desc.Format <> D3DFMT_R5G6B5) and (Desc.Format <> D3DFMT_X1R5G5B5) and (Desc.Format <> D3DFMT_A1R5G5B5) then
    Exit;

  RectPtr := @Rect;
  if (Rect.Left = 0) and (Rect.Top = 0) and (Rect.Right = FWidth) and

  (Rect.Bottom = FHeight) then RectPtr := nil;

  if (ReadOnly) then
    Flags := D3DLOCK_READONLY
  else
    Flags := 0;

  if (Failed(FHandle.LockRect(0, LockedRect, RectPtr, Flags))) then
    PHGE.System_Log('Can''t lock texture')
  else begin
    Bits := LockedRect.PBits;
    Pitch := LockedRect.Pitch;
    Result := True;
  end;
end;

function TTexture.Lock(out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean):Boolean;
var
  Desc:TD3DSurfaceDesc;
  LockedRect:TD3DLockedRect;
  Flags:Integer;
begin
  Result := False;
  Bits := nil;
  Pitch := 0;
  if (not Assigned(FHandle)) or (Failed(FHandle.GetLevelDesc(0, Desc))) then Exit;
  if (Desc.Format <> D3DFMT_A8R8G8B8) and (Desc.Format <> D3DFMT_X8R8G8B8) and (Desc.Format <> D3DFMT_R5G6B5) and (Desc.Format <> D3DFMT_X1R5G5B5) and (Desc.Format <> D3DFMT_A1R5G5B5) then
    Exit;

  if (ReadOnly) then
    Flags := D3DLOCK_READONLY
  else
    Flags := 0;

  if (Failed(FHandle.LockRect(0, LockedRect, nil, Flags))) then
    PHGE.System_Log('Can''t lock texture')
  else begin
    Bits := LockedRect.PBits;
    Pitch := LockedRect.Pitch;
    Result := True;
  end;
end;

procedure TTexture.Unlock;
begin
  FHandle.UnlockRect(0);
end;

{ TTarget }

constructor TTarget.Create(const AHandle:IDirect3DTexture8;
  const ADepth:IDirect3DSurface8; const AWidth, AHeight:Integer);
begin
  inherited Create(AHandle, AWidth, AHeight);
  FDepth := ADepth;
  //PHGE.AddTarget(Self);
end;

destructor TTarget.Destroy;
begin
  //PHGE.RemoveTarget(Self);
  inherited;
end;

procedure TTarget.Lost;
var
  DXTexture:IDirect3DTexture8;
  Desc:TD3DSurfaceDesc;
begin
  Restore;
  if (Failed(D3DXCreateTexture(PHGE.FD3DDevice, Width, Height, 1, D3DUSAGE_RENDERTARGET,
    PHGE.FD3DPP.BackBufferFormat, D3DPOOL_DEFAULT, DXTexture)))
    then begin
    PHGE.System_Log('TTarget Can''t create render target texture');
    Exit;
  end;

  Handle := DXTexture;
  DXTexture.GetLevelDesc(0, Desc);
  if PHGE.ZBuffer then
    if (Failed(PHGE.FD3DDevice.CreateDepthStencilSurface(Desc.Width, Desc.Height,
      D3DFMT_D16, D3DMULTISAMPLE_NONE, FDepth)))
      then begin
      Handle := nil;
      PHGE.System_Log('TTarget Can''t create render target depth buffer');
      Exit;
    end;
end;

procedure TTarget.Restore;
begin
  inherited;
  if Assigned(FDepth) then
    FDepth := nil;
end;

{ TResource }

constructor TResource.Create(const AHandle:Pointer; const ASize:Longword);
begin
  inherited Create;
  FHandle := AHandle;
  FSize := ASize;
end;

destructor TResource.Destroy;
begin
  FreeMem(FHandle);
  inherited;
end;

function TResource.GetHandle:Pointer;
begin
  Result := FHandle;
end;

function TResource.GetSize:Longword;
begin
  Result := FSize;
end;

(****************************************************************************
 * System.cpp, Graphics.cpp, Random.cpp, Sound.cpp, Timer.cpp, Input.cpp,
 * Resource.cpp
 ****************************************************************************)

const
  KeyNames:array[0..255] of string = (
    '?',
    'Left Mouse Button', 'Right Mouse Button', '?', 'Middle Mouse Button',
    '?', '?', '?', 'Backspace', 'Tab', '?', '?', '?', 'Enter', '?', '?',
    'Shift', 'Ctrl', 'Alt', 'Pause', 'Caps Lock', '?', '?', '?', '?', '?', '?',
    'Escape', '?', '?', '?', '?',
    'Space', 'Page Up', 'Page Down', 'End', 'Home',
    'Left Arrow', 'Up Arrow', 'Right Arrow', 'Down Arrow',
    '?', '?', '?', '?', 'Insert', 'Delete', '?',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    '?', '?', '?', '?', '?', '?', '?',
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'Left Win', 'Right Win', 'Application', '?', '?',
    'NumPad 0', 'NumPad 1', 'NumPad 2', 'NumPad 3', 'NumPad 4',
    'NumPad 5', 'NumPad 6', 'NumPad 7', 'NumPad 8', 'NumPad 9',
    'Multiply', 'Add', '?', 'Subtract', 'Decimal', 'Divide',
    'F1', 'F2', 'F3', 'F4', 'F5', 'F6', 'F7', 'F8', 'F9', 'F10', 'F11', 'F12',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    'Num Lock', 'Scroll Lock',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    'Semicolon', 'Equals', 'Comma', 'Minus', 'Period', 'Slash', 'Grave',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?',
    'Left bracket', 'Backslash', 'Right bracket', 'Apostrophe',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?', '?', '?', '?', '?', '?', '?', '?',
    '?', '?', '?');

var
  GSeed:Longword = 0;

function LoWordInt(const N:Longword):Integer;
begin
  Result := Smallint(LoWord(N));
end;

function HiWordInt(const N:Longword):Integer;
begin
  Result := Smallint(HiWord(N));
end;

function HGECreate(const Ver:Integer):THGE;
begin
  if PHGE = nil then begin
    if (Ver = HGE_VERSION) then
      PHGE := THGE.Create
    else
      PHGE := nil;
  end;
  Result := PHGE;
end;

{ THGE }

constructor THGE.Create;
begin
  inherited;
  InitializeCriticalSection(FCriticalSection);
  FActive := False;
  FHGEFPS := HGEFPS_UNLIMITED;
  FWnd := 0;

  FScreenWidth := 800;
  FScreenHeight := 600;
  FScreenBPP := 32;
  FTextureFilter := True;
  FUseSound := False;
  FHardware := False;
  FZBuffer := False;
  FGetD3DCaps := False;
  FillChar(FD3DCaps8, SizeOf(FD3DCaps8), 0);

  FSearch.FindHandle := INVALID_HANDLE_VALUE;
  //FTargets := TList.Create;
  GetLocaleFormatSettings(GetThreadLocale, FFormatSettings);
  FFormatSettings.DecimalSeparator := '.';
  FFormatSettings.ThousandSeparator := ',';
  FOnDeviceLost := nil;
  FOnDeviceReset := nil;

  FIsBeginScene := False;
end;

destructor THGE.Destroy;
begin
  PHGE := nil;
  if (FWnd <> 0) then begin
    System_Shutdown;
    Resource_RemoveAllPacks;
  end;
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure THGE.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure THGE.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure THGE.AddTarget(Target:TTarget);
begin
  //FTargets.Add(Target);
end;

procedure THGE.RemoveTarget(Target:TTarget);
begin
  // FTargets.Remove(Target);
end;

{
procedure THGE.FocusChange(const Act: Boolean);
begin

end;
}

function THGE.FormatId(const Fmt:TD3DFormat):Integer;
begin
  case Fmt of
    D3DFMT_R5G6B5:
      Result := 1;
    D3DFMT_X1R5G5B5:
      Result := 2;
    D3DFMT_A1R5G5B5:
      Result := 3;
    D3DFMT_X8R8G8B8:
      Result := 4;
    D3DFMT_A8R8G8B8:
      Result := 5;
    else
      Result := 0;
  end;
end;

function THGE.GetD3DCaps8:PD3DCaps8;
begin
  if FGetD3DCaps then
    Result := @FD3DCaps8
  else
    Result := nil;
end;

procedure THGE.GfxDone;
begin
  FScreenSurf := nil;
  FScreenDepth := nil;
  FGetD3DCaps := False;
  //while FTargets.Count > 0 do
 //   TTarget(FTargets.Items[0]).Free;
 // FTargets.Clear;

  if Assigned(FIB) then begin
    FD3DDevice.SetIndices(nil, 0);
    FIB := nil;
  end;
  if Assigned(FVB) then begin
    if Assigned(FVertArray) then begin
      FVB.Unlock;
      FVertArray := nil;
    end;
    FD3DDevice.SetStreamSource(0, nil, SizeOf(THGEVertex));
    FVB := nil;
  end;
  FD3DDevice := nil;
  FD3D := nil;
end;

function THGE.GfxInit:Boolean;
const
  Formats:array[0..5] of string = (
    'UNKNOWN', 'R5G6B5', 'X1R5G5B5', 'A1R5G5B5', 'X8R8G8B8', 'A8R8G8B8');
var
  AdID:TD3DAdapterIdentifier8;
  Mode:TD3DDisplayMode;
  AFormat:TD3DFormat;
  NModes, I:Longword;
  Status:Longword;
begin
  Result := False;
  AFormat := D3DFMT_UNKNOWN;

  // Init D3D

  FD3D := Direct3DCreate8(D3D_SDK_VERSION); // 120 or D3D_SDK_VERSION
  if (FD3D = nil) then begin
    System_Log(Format('Can''t create D3D interface %d', [GetLastError]));
    Exit;
  end;

  // Get adapter info

  FD3D.GetAdapterIdentifier(D3DADAPTER_DEFAULT, D3DENUM_NO_WHQL_LEVEL, AdID);
  System_Log('D3D Driver: %s', [AdID.Driver]);
  System_Log('Description: %s', [AdID.Description]);
  System_Log('Version: %d.%d.%d.%d', [
    HiWord(AdID.DriverVersionHighPart),
      LoWord(AdID.DriverVersionHighPart),
      HiWord(AdID.DriverVersionLowPart),
      LoWord(AdID.DriverVersionLowPart)]);

  // Set up Windowed presentation parameters
  Status := FD3D.GetAdapterDisplayMode(D3DADAPTER_DEFAULT, Mode);
  if Failed(Status) or (Mode.Format = D3DFMT_UNKNOWN)
    then begin
    System_Log(Format('Can''t determine desktop video mode %s %d', [FailString(Status), GetLastError]));
    if (FWindowed) then
      Exit;
  end;

  ZeroMemory(@FD3DPPW, SizeOf(FD3DPPW));

  FD3DPPW.BackBufferWidth := FScreenWidth;
  FD3DPPW.BackBufferHeight := FScreenHeight;
  FD3DPPW.BackBufferFormat := Mode.Format;
  FD3DPPW.BackBufferCount := 1;
  FD3DPPW.MultiSampleType := D3DMULTISAMPLE_NONE;
  FD3DPPW.hDeviceWindow := FWnd;
  FD3DPPW.Windowed := True;

  if (FHGEFPS = HGEFPS_VSYNC) then
    FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY_VSYNC
  else
    FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY;

  if (FZBuffer) then begin
    FD3DPPW.EnableAutoDepthStencil := True;
    FD3DPPW.AutoDepthStencilFormat := D3DFMT_D16;
  end;

  // Set up Full Screen presentation parameters

  NModes := FD3D.GetAdapterModeCount(D3DADAPTER_DEFAULT);
  for I := 0 to NModes - 1 do begin
    FD3D.EnumAdapterModes(D3DADAPTER_DEFAULT, I, Mode);
    if (Integer(Mode.Width) <> FScreenWidth) or (Integer(Mode.Height) <> FScreenHeight) then
      Continue;
    if (FScreenBPP = 16) and (FormatId(Mode.Format) > FormatId(D3DFMT_A1R5G5B5)) then
      Continue;
    if (FormatId(Mode.Format) > FormatId(AFormat)) then
      AFormat := Mode.Format;
  end;

  if (AFormat = D3DFMT_UNKNOWN) then begin
    System_Log(Format('Can''t find appropriate full screen video mode %d', [GetLastError]));
    if (not FWindowed) then
      Exit;
  end;

  ZeroMemory(@FD3DPPFS, SizeOf(FD3DPPFS));

  FD3DPPFS.BackBufferWidth := FScreenWidth;
  FD3DPPFS.BackBufferHeight := FScreenHeight;
  FD3DPPFS.BackBufferFormat := AFormat;
  FD3DPPFS.BackBufferCount := 1;
  FD3DPPFS.MultiSampleType := D3DMULTISAMPLE_NONE;
  FD3DPPFS.hDeviceWindow := FWnd;
  FD3DPPFS.Windowed := False;

  FD3DPPFS.SwapEffect := D3DSWAPEFFECT_FLIP;
  FD3DPPFS.FullScreen_RefreshRateInHz := D3DPRESENT_RATE_DEFAULT;
  if (FHGEFPS = HGEFPS_VSYNC) then
    FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_ONE
  else
    FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_IMMEDIATE;

  if (FZBuffer) then begin
    FD3DPPFS.EnableAutoDepthStencil := True;
    FD3DPPFS.AutoDepthStencilFormat := D3DFMT_D16;
  end;

  if (FWindowed) then
    FD3DPP := @FD3DPPW
  else
    FD3DPP := @FD3DPPFS;

  if (FormatId(FD3DPP.BackBufferFormat) < 4) then
    FScreenBPP := 16
  else
    FScreenBPP := 32;

  // Create D3D Device

  if FHardware then begin
    if (Failed(FD3D.CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, FWnd,
      D3DCREATE_HARDWARE_VERTEXPROCESSING, FD3DPP^, FD3DDevice)))
      then begin
      FD3DDevice := nil;
      if (FZBuffer) then begin
        FZBuffer := False;
        FD3DPPFS.EnableAutoDepthStencil := False;
        FD3DPPFS.AutoDepthStencilFormat := D3DFMT_UNKNOWN;
        if (Failed(FD3D.CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, FWnd,
          D3DCREATE_HARDWARE_VERTEXPROCESSING, FD3DPP^, FD3DDevice)))
          then begin
          FHardware := False;
          FD3DDevice := nil;
          FZBuffer := True;
          FD3DPPFS.EnableAutoDepthStencil := True;
        end;
      end;
    end;
  end;

  if (FD3DDevice = nil) then begin
    if (FZBuffer) then begin
      FD3DPPFS.EnableAutoDepthStencil := True;
      FD3DPPFS.AutoDepthStencilFormat := D3DFMT_D16;
    end else begin
      FD3DPPFS.EnableAutoDepthStencil := False;
      FD3DPPFS.AutoDepthStencilFormat := D3DFMT_UNKNOWN;
    end;
    Status := FD3D.CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, FWnd,
      D3DCREATE_SOFTWARE_VERTEXPROCESSING, FD3DPP^, FD3DDevice);
    if Failed(Status)
      then begin
      FD3DDevice := nil;
      if (FZBuffer) then begin
        FZBuffer := False;
        FD3DPPFS.EnableAutoDepthStencil := False;
        FD3DPPFS.AutoDepthStencilFormat := D3DFMT_UNKNOWN;
        Status := FD3D.CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, FWnd,
          D3DCREATE_SOFTWARE_VERTEXPROCESSING, FD3DPP^, FD3DDevice);
        if Failed(Status)
          then begin
          FD3DDevice := nil;
          FZBuffer := True;
          FD3DPPFS.EnableAutoDepthStencil := True;
        end;
      end;
      if (FD3DDevice = nil) then begin
        System_Log(Format('Can''t create D3D device %s %d', [FailString(Status), GetLastError]));
        Exit;
      end;
    end;
  end;

  System_Log('Mode: %d x %d x %s' + CRLF,
    [FScreenWidth, FScreenHeight, Formats[FormatId(AFormat)]]);

  FGetD3DCaps := Succeeded(FD3DDevice.GetDeviceCaps(FD3DCaps8));

  System_Log('MaxTextureWidth: %d', [FD3DCaps8.MaxTextureWidth]);

  System_Log('MaxTextureHeight: %d', [FD3DCaps8.MaxTextureHeight]);

  System_Log('MaxPrimitiveCount: %d', [FD3DCaps8.MaxPrimitiveCount]);

  System_Log('MaxVertexIndex: %d', [FD3DCaps8.MaxVertexIndex]);

  // Create vertex batch buffer

  FVertArray := nil;

  // Init all stuff that can be lost

  SetProjectionMatrix(FScreenWidth, FScreenHeight);
  D3DXMatrixIdentity(FMatView);

  Vertices[0].TX := 0;
  Vertices[0].TY := 0;
  Vertices[1].TX := 1;
  Vertices[1].TY := 0;
  Vertices[2].TX := 1;
  Vertices[2].TY := 1;
  Vertices[3].TX := 0;
  Vertices[3].TY := 1;

  if (not InitLost) then Exit;

  Gfx_Clear(0);

  Result := True;
end;

function THGE.InitLost:Boolean;
var
  PIndices:PWord;
  N:Word;
  I:Integer;
  Status:HResult;
begin
  Result := False;

  // Store render target

  FScreenSurf := nil;
  FScreenDepth := nil;

  if Assigned(FOnDeviceLost) then
    FOnDeviceLost(Self);

  Status := FD3DDevice.GetRenderTarget(FScreenSurf);

  if Failed(Status) then begin
    System_Log(Format('GetRenderTarget 2 %s %d', [FailString(Status), GetLastError]));
    Exit;
  end;

  Status := FD3DDevice.GetDepthStencilSurface(FScreenDepth);
  if Failed(Status) then begin
    System_Log(Format('GetDepthStencilSurface %s %d', [FailString(Status), GetLastError]));
    Exit;
  end;

  // for I := 0 to FTargets.Count - 1 do begin
  //   Target := TTarget(FTargets[I]);
  //   Target.Lost;
  // end;

  if FGetD3DCaps then begin
    with FD3DCaps8 do begin
      FMaxPrimitives := Min2(MaxPrimitiveCount, MaxCachedPrimitives);
      FVertexCache := Min2(MaxVertexIndex, MaxCachedVertices);
      FIndexCache := Min2(MaxVertexIndex, MaxCachedIndices);
    end;

  end else begin
    FMaxPrimitives := VERTEX_BUFFER_SIZE;
    FVertexCache := VERTEX_BUFFER_SIZE;
    FIndexCache := VERTEX_BUFFER_SIZE;
  end;

  // chongchong 2018-02-12 去劫持
  System_Log('Cache ' + IntToStr(FMaxPrimitives) + ' ' + IntToStr(FVertexCache) + ' ' + IntToStr(FIndexCache));
  Status := FD3DDevice.CreateVertexBuffer(FVertexCache * SizeOf(THGEVertex),
    D3DUSAGE_WRITEONLY, D3DFVF_HGEVERTEX, D3DPOOL_DEFAULT, FVB);
  if Failed(Status) then begin
    System_Log(Format('Can''t create D3D vertex buffer %s %d', [FailString(Status), GetLastError]));
    Exit;
  end;

  FD3DDevice.SetVertexShader(D3DFVF_HGEVERTEX);
  FD3DDevice.SetStreamSource(0, FVB, SizeOf(THGEVertex));

  // Create and setup Index buffer
  Status := FD3DDevice.CreateIndexBuffer(FIndexCache * 6 div 4 * SizeOf(Word),
    D3DUSAGE_WRITEONLY, D3DFMT_INDEX16, D3DPOOL_DEFAULT, FIB);

  if Failed(Status) then begin
    System_Log(Format('Can''t create D3D index buffer %s %d', [FailString(Status), GetLastError]));
    Exit;
  end;

  N := 0;
  Status := FIB.Lock(0, 0, PByte(PIndices), 0);
  if Failed(Status) then begin
    System_Log(Format('Can''t lock D3D index buffer %s %d', [FailString(Status), GetLastError]));
    Exit;
  end;

  for I := 0 to (FIndexCache div 4) - 1 do begin
    PIndices^ := N;
    Inc(PIndices);
    PIndices^ := N + 1;
    Inc(PIndices);
    PIndices^ := N + 2;
    Inc(PIndices);
    PIndices^ := N + 2;
    Inc(PIndices);
    PIndices^ := N + 3;
    Inc(PIndices);
    PIndices^ := N;
    Inc(PIndices);
    Inc(N, 4);
  end;

  FIB.Unlock;
  FD3DDevice.SetIndices(FIB, 0);

  // Set common render states

    //pD3DDevice->SetRenderState( D3DRS_LASTPIXEL, FALSE );
  FD3DDevice.SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);
  FD3DDevice.SetRenderState(D3DRS_LIGHTING, 0);

  FD3DDevice.SetRenderState(D3DRS_ALPHABLENDENABLE, 1);
  FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
  FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);

  FD3DDevice.SetRenderState(D3DRS_ALPHATESTENABLE, 1);
  FD3DDevice.SetRenderState(D3DRS_ALPHAREF, 1);
  FD3DDevice.SetRenderState(D3DRS_ALPHAFUNC, D3DCMP_GREATEREQUAL);

  FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
  FD3DDevice.SetTextureStageState(0, D3DTSS_COLORARG1, D3DTA_TEXTURE);
  FD3DDevice.SetTextureStageState(0, D3DTSS_COLORARG2, D3DTA_DIFFUSE);

  FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
  FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE);
  FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAARG2, D3DTA_DIFFUSE);

  FD3DDevice.SetTextureStageState(0, D3DTSS_MIPFILTER, D3DTEXF_POINT);

  if (FTextureFilter) then begin
    FD3DDevice.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTEXF_LINEAR);
    FD3DDevice.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTEXF_LINEAR);
  end else begin
    FD3DDevice.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTEXF_POINT);
    FD3DDevice.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTEXF_POINT);
  end;

  FPrim := 0;
  FCurPrimType := HGEPRIM_QUADS;
  FCurBlendMode := BLEND_DEFAULT;
  FCurTexture := nil;

  FD3DDevice.SetTransform(D3DTS_VIEW, FMatView);
  FD3DDevice.SetTransform(D3DTS_PROJECTION, FMatProj);

  Result := True;
end;

function THGE.GfxRestore:Boolean;

procedure GfxUpdateParameters;
  const
    Formats:array[0..5] of string = ('UNKNOWN', 'R5G6B5', 'X1R5G5B5', 'A1R5G5B5', 'X8R8G8B8', 'A8R8G8B8');
  var
    Mode:TD3DDisplayMode;
    Format:TD3DFormat;
    NModes, I:Longword;
  begin

    if FWindowed then begin
      FD3DPP := @FD3DPPW;
      if (Failed(FD3D.GetAdapterDisplayMode(D3DADAPTER_DEFAULT, Mode))) or (Mode.Format = D3DFMT_UNKNOWN) then begin
        PostError('Can''t determine desktop video mode');
        Exit;
      end;
      FD3DPP.BackBufferFormat := Mode.Format;
    end else begin
      FD3DPP := @FD3DPPFS;
      Format := D3DFMT_UNKNOWN;
      NModes := FD3D.GetAdapterModeCount(D3DADAPTER_DEFAULT);
      for I := 0 to NModes - 1 do begin
        FD3D.EnumAdapterModes(D3DADAPTER_DEFAULT, I, Mode);
        if (Integer(Mode.Width) <> FScreenWidth) or (Integer(Mode.Height) <> FScreenHeight) then Continue;
        if (FScreenBPP = 16) and (FormatId(Mode.Format) > FormatId(D3DFMT_A1R5G5B5)) then Continue;
        if (FormatId(Mode.Format) > FormatId(Format)) then Format := Mode.Format;
      end;
      if (Format = D3DFMT_UNKNOWN) then begin
        PostError('Can''t find appropriate full screen video mode');
        Exit;
      end;
      FD3DPP.BackBufferFormat := Format;
    end;
    FD3DPP.BackBufferWidth := FScreenWidth;
    FD3DPP.BackBufferHeight := FScreenHeight;
  end;

  {
  function HGEInit: Boolean;
  const
    Formats: array[0..5] of string = (
      'UNKNOWN', 'R5G6B5', 'X1R5G5B5', 'A1R5G5B5', 'X8R8G8B8', 'A8R8G8B8');
  var
    Mode: TD3DDisplayMode;
    AFormat: TD3DFormat;
    NModes, I: Longword;
    Status: Longword;
  begin
    AFormat := D3DFMT_UNKNOWN;

    // Set up Windowed presentation parameters
    Status := FD3D.GetAdapterDisplayMode(D3DADAPTER_DEFAULT, Mode);
    if Failed(Status) or (Mode.Format = D3DFMT_UNKNOWN)
      then begin
      System_Log(Format('Can''t determine desktop video mode %s %d', [FailString(Status), GetLastError]));
      if (FWindowed) then
        Exit;
    end;

    ZeroMemory(@FD3DPPW, SizeOf(FD3DPPW));

    FD3DPPW.BackBufferWidth := FScreenWidth;
    FD3DPPW.BackBufferHeight := FScreenHeight;
    FD3DPPW.BackBufferFormat := Mode.Format;
    FD3DPPW.BackBufferCount := 1;
    FD3DPPW.MultiSampleType := D3DMULTISAMPLE_NONE;
    FD3DPPW.hDeviceWindow := FWnd;
    FD3DPPW.Windowed := True;

    if (FHGEFPS = HGEFPS_VSYNC) then
      FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY_VSYNC
    else
      FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY;

    if (FZBuffer) then begin
      FD3DPPW.EnableAutoDepthStencil := True;
      FD3DPPW.AutoDepthStencilFormat := D3DFMT_D16;
    end;
                 Result
// Set up Full Screen presentation parameters

    NModes := FD3D.GetAdapterModeCount(D3DADAPTER_DEFAULT);
    for I := 0 to NModes - 1 do begin
      FD3D.EnumAdapterModes(D3DADAPTER_DEFAULT, I, Mode);
      if (Integer(Mode.Width) <> FScreenWidth) or (Integer(Mode.Height) <> FScreenHeight) then
        Continue;
      if (FScreenBPP = 16) and (FormatId(Mode.Format) > FormatId(D3DFMT_A1R5G5B5)) then
        Continue;
      if (FormatId(Mode.Format) > FormatId(AFormat)) then
        AFormat := Mode.Format;
    end;

    if (AFormat = D3DFMT_UNKNOWN) then begin
      System_Log(Format('Can''t find appropriate full screen video mode %d', [GetLastError]));
      if (not FWindowed) then
        Exit;
    end;

    ZeroMemory(@FD3DPPFS, SizeOf(FD3DPPFS));

    FD3DPPFS.BackBufferWidth := FScreenWidth;
    FD3DPPFS.BackBufferHeight := FScreenHeight;
    FD3DPPFS.BackBufferFormat := AFormat;
    FD3DPPFS.BackBufferCount := 1;
    FD3DPPFS.MultiSampleType := D3DMULTISAMPLE_NONE;
    FD3DPPFS.hDeviceWindow := FWnd;
    FD3DPPFS.Windowed := False;

    FD3DPPFS.SwapEffect := D3DSWAPEFFECT_FLIP;
    FD3DPPFS.FullScreen_RefreshRateInHz := D3DPRESENT_RATE_DEFAULT;
    if (FHGEFPS = HGEFPS_VSYNC) then
      FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_ONE
    else
      FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_IMMEDIATE;

    if (FZBuffer) then begin
      FD3DPPFS.EnableAutoDepthStencil := True;
      FD3DPPFS.AutoDepthStencilFormat := D3DFMT_D16;
    end;

    if (FWindowed) then
      FD3DPP := @FD3DPPW
    else
      FD3DPP := @FD3DPPFS;
  end;
  }
begin
  //  if(FD3DDevice.TestCooperativeLevel <> D3DERR_DEVICELOST) then
  //    Exit;
  Result := False;
  if (FD3DDevice = nil) then Exit;

  FScreenSurf := nil;
  FScreenDepth := nil;

  if Assigned(FOnDeviceReset) then
    FOnDeviceReset(Self);

  if Assigned(FIB) then begin
    FD3DDevice.SetIndices(nil, 0);
    FIB := nil;
  end;
  if Assigned(FVB) then begin
    FD3DDevice.SetStreamSource(0, nil, SizeOf(THGEVertex));
    FVB := nil;
  end;

  GfxUpdateParameters;

  FD3DDevice.Reset(FD3DPP^);

  SetProjectionMatrix(FScreenWidth, FScreenHeight);
  D3DXMatrixIdentity(FMatView);

  if (not InitLost) then Exit;

  if Assigned(FProcGfxRestoreFunc) then
    Result := FProcGfxRestoreFunc
  else
    Result := True;
end;

function THGE.Gfx_CanBegin():Boolean;
var
  HR:HResult;
begin
  Result := False;
  HR := FD3DDevice.TestCooperativeLevel;
  if (HR = D3DERR_DEVICELOST) then Exit;
  if (HR = D3DERR_DEVICENOTRESET) then //设备丢失需要恢复
    if (not GfxRestore) then Exit;
  Result := True;
end;

function THGE.Gfx_BeginScene(const Target:TTarget):Boolean;
var
  Surf, Depth:IDirect3DSurface8;
  //HR: HResult;
begin
  Result := False;

  {HR := FD3DDevice.TestCooperativeLevel;
  if (HR = D3DERR_DEVICELOST) then
    Exit;
  if (HR = D3DERR_DEVICENOTRESET) then
    if (not GfxRestore) then
      Exit;}

  if Assigned(FVertArray) then begin
    System_Log('Gfx_BeginScene: Scene is already being rendered');
    Exit;
  end;

  if (Target <> FCurTarget) then begin
    if Assigned(Target) and Assigned(Target.Handle) then begin
      Target.Handle.GetSurfaceLevel(0, Surf);
      Depth := Target.Depth;
    end else begin
      Surf := FScreenSurf;
      Depth := FScreenDepth;
    end;
    if (Failed(FD3DDevice.SetRenderTarget(Surf, Depth))) then begin
      System_Log('Gfx_BeginScene: Can''t set render target');
      Exit;
    end;
    if Assigned(Target) then begin
      Surf := nil;
      if Assigned(Target.Depth) then
        FD3DDevice.SetRenderState(D3DRS_ZENABLE, D3DZB_TRUE)
      else
        FD3DDevice.SetRenderState(D3DRS_ZENABLE, D3DZB_FALSE);
      SetProjectionMatrix(Target.Width, Target.Height);
    end else begin
      if (FZBuffer) then
        FD3DDevice.SetRenderState(D3DRS_ZENABLE, D3DZB_TRUE)
      else
        FD3DDevice.SetRenderState(D3DRS_ZENABLE, D3DZB_FALSE);
      SetProjectionMatrix(FScreenWidth, FScreenHeight);
    end;

    FD3DDevice.SetTransform(D3DTS_PROJECTION, FMatProj);
    D3DXMatrixIdentity(FMatView);
    FD3DDevice.SetTransform(D3DTS_VIEW, FMatView);

    FCurTarget := Target;
  end;
  FD3DDevice.BeginScene;
  FVB.Lock(0, 0, PByte(FVertArray), 0);
  Result := True;

  FIsBeginScene := True;
end;

procedure THGE.Gfx_Clear(const Color:Longword);
begin
  if Assigned(FCurTarget) then begin
    if Assigned(FCurTarget.Depth) then
      FD3DDevice.Clear(0, nil, D3DCLEAR_TARGET or D3DCLEAR_ZBUFFER, Color, 1.0, 0)
    else
      FD3DDevice.Clear(0, nil, D3DCLEAR_TARGET, Color, 1.0, 0);
  end else begin
    if (FZBuffer) then
      FD3DDevice.Clear(0, nil, D3DCLEAR_TARGET or D3DCLEAR_ZBUFFER, Color, 1.0, 0)
    else
      FD3DDevice.Clear(0, nil, D3DCLEAR_TARGET, Color, 1.0, 0);
  end;
end;

procedure THGE.Gfx_EndScene;
begin
  RenderBatch(True);
  FD3DDevice.EndScene;
  if (FCurTarget = nil) then
    FD3DDevice.Present(nil, nil, 0, nil);

  FIsBeginScene := False;
end;

procedure THGE.Gfx_FinishBatch(const NPrim:Integer);
begin
  FPrim := NPrim;
end;

procedure THGE.Gfx_RenderLine(const X1, Y1, X2, Y2:Single;
  const Color:Longword; const Z:Single);
var
  I:Integer;
begin
  if Assigned(FVertArray) then begin
    if (FCurPrimType <> HGEPRIM_LINES)
      or (FPrim >= FIndexCache div HGEPRIM_LINES)
      or (FCurTexture <> nil) or (FCurBlendMode <> BLEND_DEFAULT)
      then begin
      RenderBatch;
      FCurPrimType := HGEPRIM_LINES;
      if (FCurBlendMode <> BLEND_DEFAULT) then
        SetBlendMode(BLEND_DEFAULT);
      if (FCurTexture <> nil) then begin
        FD3DDevice.SetTexture(0, nil);
        FCurTexture := nil;
      end;
    end;

    I := FPrim * HGEPRIM_LINES;
    FVertArray[I].X := X1;
    FVertArray[I + 1].X := X2;
    FVertArray[I].Y := Y1;
    FVertArray[I + 1].Y := Y2;
    FVertArray[I].Z := Z;
    FVertArray[I + 1].Z := Z;
    FVertArray[I].Col := Color;
    FVertArray[I + 1].Col := Color;
    FVertArray[I].TX := 0;
    FVertArray[I + 1].TX := 0;
    FVertArray[I].TY := 0;
    FVertArray[I + 1].TY := 0;

    Inc(FPrim);
  end;
end;

procedure THGE.Gfx_RenderQuad(const Quad:THGEQuad);
begin
  if Assigned(FVertArray) then begin
    if (FCurPrimType <> HGEPRIM_QUADS)
      or (FPrim >= FIndexCache div HGEPRIM_QUADS)
      or (FCurTexture <> Quad.Tex)
      or (FCurBlendMode <> Quad.Blend)
      then begin
      RenderBatch;
      FCurPrimType := HGEPRIM_QUADS;
      if (FCurBlendMode <> Quad.Blend) then
        SetBlendMode(Quad.Blend);
      if (Quad.Tex <> FCurTexture) then begin
        if Assigned(Quad.Tex) then
          FD3DDevice.SetTexture(0, Quad.Tex.Handle)
        else
          FD3DDevice.SetTexture(0, nil);
        FCurTexture := Quad.Tex;
      end;
    end;

    Move(Quad.V, FVertArray[FPrim * HGEPRIM_QUADS],
      SizeOf(THGEVertex) * HGEPRIM_QUADS);
    Inc(FPrim);
  end;
end;

procedure THGE.Gfx_RenderTriple(const Triple:THGETriple);
begin
  if Assigned(FVertArray) then begin
    if (FCurPrimType <> HGEPRIM_TRIPLES)
      or (FPrim >= FIndexCache div HGEPRIM_TRIPLES)
      or (FCurTexture <> Triple.Tex)
      or (FCurBlendMode <> Triple.Blend)
      then begin
      RenderBatch;
      FCurPrimType := HGEPRIM_TRIPLES;
      if (FCurBlendMode <> Triple.Blend) then
        SetBlendMode(Triple.Blend);
      if (Triple.Tex <> FCurTexture) then begin
        if Assigned(Triple.Tex) then
          FD3DDevice.SetTexture(0, Triple.Tex.Handle)
        else
          FD3DDevice.SetTexture(0, nil);
        FCurTexture := Triple.Tex;
      end;
    end;

    Move(Triple.V, FVertArray[FPrim * HGEPRIM_TRIPLES],
      SizeOf(THGEVertex) * HGEPRIM_TRIPLES);
    Inc(FPrim);
  end;
end;

procedure THGE.Gfx_GetClipping(out X, Y, W, H:Integer);
var
  VP:TD3DViewport8;
begin
  FillChar(VP, SizeOf(VP), 0);
  FD3DDevice.GetViewport(VP);

  x := VP.X;
  y := VP.Y;

  W := VP.Width;
  H := VP.Height;
end;

procedure THGE.Gfx_SetClipping(X, Y, W, H:Integer);
var
  VP:TD3DViewport8;
  ScrWidth, ScrHeight:Integer;
  Tmp:TD3DXMATRIX;
begin
  if (FCurTarget = nil) then begin
    ScrWidth := PHGE.System_GetState(HGE_SCREENWIDTH);
    ScrHeight := PHGE.System_GetState(HGE_SCREENHEIGHT);
  end else begin
    ScrWidth := FCurTarget.Width;
    ScrHeight := FCurTarget.Width;
  end;

  if (W = 0) then begin
    VP.X := 0;
    VP.Y := 0;
    VP.Width := ScrWidth;
    VP.Height := ScrHeight;
  end else begin
    if (X < 0) then begin
      Inc(W, X);
      X := 0;
    end;
    if (Y < 0) then begin
      Inc(H, Y);
      Y := 0;
    end;

    if (X + W > ScrWidth) then
      W := ScrWidth - X;
    if (Y + H > ScrHeight) then
      H := ScrHeight - Y;

    VP.X := X;
    VP.Y := Y;
    VP.Width := W;
    VP.Height := H;
  end;

  VP.MinZ := 0.0;
  VP.MaxZ := 1.0;

  RenderBatch;
  FD3DDevice.SetViewport(VP);

  D3DXMatrixScaling(FMatProj, 1.0, -1.0, 1.0);
  D3DXMatrixTranslation(Tmp, -0.5, +0.5, 0.0);
  D3DXMatrixMultiply(FMatProj, FMatProj, Tmp);
  D3DXMatrixOrthoOffCenterLH(Tmp, VP.X, VP.X + VP.Width, -(VP.Y + VP.Height),
    -VP.Y, VP.MinZ, VP.MaxZ);
  D3DXMatrixMultiply(FMatProj, FMatProj, Tmp);
  FD3DDevice.SetTransform(D3DTS_PROJECTION, FMatProj);
end;

procedure THGE.Gfx_SetTransform(const X, Y, DX, DY, Rot, HScale,
  VScale:Single);
var
  Tmp:TD3DXMATRIX;
begin
  if (VScale = 0.0) then
    D3DXMatrixIdentity(FMatView)
  else begin
    D3DXMatrixTranslation(FMatView, -X, -Y, 0.0);
    D3DXMatrixScaling(Tmp, HScale, VScale, 1.0);
    D3DXMatrixMultiply(FMatView, FMatView, Tmp);
    D3DXMatrixRotationZ(Tmp, -Rot);
    D3DXMatrixMultiply(FMatView, FMatView, Tmp);
    D3DXMatrixTranslation(Tmp, X + DX, Y + DY, 0.0);
    D3DXMatrixMultiply(FMatView, FMatView, Tmp);
  end;

  RenderBatch;
  FD3DDevice.SetTransform(D3DTS_VIEW, FMatView);
end;

function THGE.Gfx_StartBatch(const PrimType:Integer; const Tex:TTexture;
  const Blend:Integer; out MaxPrim:Integer):PHGEVertexArray;
begin
  if Assigned(FVertArray) then begin
    RenderBatch;

    FCurPrimType := PrimType;
    if (FCurBlendMode <> Blend) then
      SetBlendMode(Blend);
    if (Tex <> FCurTexture) then begin
      if Assigned(Tex) then
        FD3DDevice.SetTexture(0, Tex.Handle)
      else
        FD3DDevice.SetTexture(0, nil);
      FCurTexture := Tex;
    end;

    MaxPrim := FIndexCache div PrimType;
    Result := FVertArray;
  end else
    Result := nil;
end;

procedure THGE.PostError(const Error:string);
begin
  System_Log(Error);
  FError := Error;
end;

procedure THGE.RenderBatch(const EndScene:Boolean);
begin
  if Assigned(FVertArray) then begin
    if (not FIsBeginScene) or (EndScene) then {// +++++ if (not FIsBeginScene) or (EndScene) then 顶点缓冲区一次锁定 chongchong 2018-02-27} begin
      FVB.Unlock;
    end;

    if (FPrim <> 0) then begin
      case FCurPrimType of
        HGEPRIM_QUADS:
          FD3DDevice.DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, FPrim shl 2, 0, FPrim shl 1);
        HGEPRIM_TRIPLES:
          FD3DDevice.DrawPrimitive(D3DPT_TRIANGLELIST, 0, FPrim);
        HGEPRIM_LINES:
          FD3DDevice.DrawPrimitive(D3DPT_LINELIST, 0, FPrim);
      end;

      FPrim := 0;
    end;

    if (EndScene) then begin
      FVertArray := nil;
    end
    else if (not FIsBeginScene) then // +++++ if (not FIsBeginScene) then 顶点缓冲区一次锁定 chongchong 2018-02-27
      FVB.Lock(0, 0, PByte(FVertArray), 0);
  end;
end;

procedure THGE.Resize(const Width, Height:Integer);
begin
  if (FWndParent <> 0) then begin

    FD3DPPW.BackBufferWidth := Width;
    FD3DPPW.BackBufferHeight := Height;
    FScreenWidth := Width;
    FScreenHeight := Height;

    SetProjectionMatrix(FScreenWidth, FScreenHeight);
    GfxRestore;

  end;
end;

function THGE.Resource_AttachPack(const Filename:string):Boolean;
var
  Name:string;
  ResItem:PResourceList;
  Zip:unzFile;
begin
  Result := False;
  ResItem := FRes;
  Name := UpperCase(Resource_MakePath(Filename));

  while Assigned(ResItem) do begin
    if (Name = ResItem.Filename) then
      Exit;
    ResItem := ResItem.Next;
  end;

  Zip := unzOpen(PChar(Name));
  if (Zip = nil) then
    Exit;
  unzClose(Zip);

  New(ResItem);
  ResItem.Filename := Name;
  ResItem.Next := FRes;
  FRes := ResItem;
  Result := True;
end;

function THGE.Resource_EnumFiles(const Wildcard:string):string;
begin
  Result := '';
  if (Wildcard <> '') then begin
    FindClose(FSearch);
    if (FindFirst(Resource_MakePath(Wildcard), faAnyFile, FSearch) <> 0) then
      Exit;
    if ((FSearch.Attr and faDirectory) = 0) then
      Result := FSearch.Name
    else
      Result := Resource_EnumFiles;
  end else begin
    if (FSearch.FindHandle = INVALID_HANDLE_VALUE) then
      Exit;
    while True do begin
      if (FindNext(FSearch) <> 0) then begin
        FindClose(FSearch);
        Exit;
      end;
      if ((FSearch.Attr and faDirectory) = 0) then begin
        Result := FSearch.Name;
        Exit;
      end;
    end;
  end;
end;

function THGE.Resource_EnumFolders(const Wildcard:string):string;
begin
  Result := '';
  if (Wildcard <> '') then begin
    FindClose(FSearch);
    if (FindFirst(Resource_MakePath(Wildcard), faAnyFile, FSearch) <> 0) then
      Exit;
    if ((FSearch.Attr and faDirectory) <> 0) and (FSearch.Name[1] <> '.') then
      Result := FSearch.Name
    else
      Result := Resource_EnumFolders;
  end
  else begin
    if (FSearch.FindHandle = INVALID_HANDLE_VALUE) then
      Exit;
    while True do begin
      if (FindNext(FSearch) <> 0) then begin
        FindClose(FSearch);
        Exit;
      end;

      if ((FSearch.Attr and faDirectory) <> 0) and (FSearch.Name[1] <> '.') then begin
        Result := FSearch.Name;
        Exit;
      end;
    end;
  end;
end;

function THGE.Resource_Load(const Filename:string;
  const Size:PLongword):IResource;
const
  ResErr = 'Can''t load resource: %s';
var
  Data:Pointer;
  ResItem:PResourceList;
  Name, ZipName:string;
  PZipName:array[0..MAX_PATH] of Char;
  Zip:unzFile;
  FileInfo:unz_file_info;
  Done, I:Integer;
  F:THandle;
  BytesRead:Cardinal;
begin
  Result := nil;
  Data := nil;
  if (Filename = '') then
    Exit;
  ResItem := FRes;

  if (not (Filename[1] in ['\', '/', ':'])) then begin
    // Load from pack
    Name := UpperCase(Filename);
    for I := 1 to Length(Name) do
      if (Name[I] = '/') then
        Name[I] := '\';

    while Assigned(ResItem) do begin
      Zip := unzOpen(PChar(ResItem.Filename));
      Done := unzGoToFirstFile(Zip);
      while (Done = UNZ_OK) do begin
        unzGetCurrentFileInfo(Zip, @FileInfo, PZipName, MAX_PATH, nil, 0, nil, 0);
        ZipName := UpperCase(PZipName);
        for I := 1 to Length(ZipName) do
          if (ZipName[I] = '/') then
            ZipName[I] := '\';
        if (Name = ZipName) then begin
          if (unzOpenCurrentFile(Zip) <> UNZ_OK) then begin
            unzClose(Zip);
            System_Log(Format(ResErr, [Filename]));
            Exit;
          end;

          try
            GetMem(Data, FileInfo.uncompressed_size);
          except
            unzCloseCurrentFile(Zip);
            unzClose(Zip);
            System_Log(Format(ResErr, [Filename]));
            Exit;
          end;

          if (unzReadCurrentFile(Zip, Data, FileInfo.uncompressed_size) < 0) then begin
            unzCloseCurrentFile(Zip);
            unzClose(Zip);
            FreeMem(Data);
            System_Log(Format(ResErr, [Filename]));
            Exit;
          end;
          Result := TResource.Create(Data, FileInfo.uncompressed_size);
          unzCloseCurrentFile(Zip);
          unzClose(Zip);
          if Assigned(Size) then
            Size^ := FileInfo.uncompressed_size;
          Exit;
        end;

        Done := unzGoToNextFile(Zip);
      end;

      unzClose(Zip);
      ResItem := ResItem.Next;
    end;
  end;

  // Load from file
  F := CreateFile(PChar(Filename), GENERIC_READ,
    FILE_SHARE_READ, nil, OPEN_EXISTING,
    FILE_ATTRIBUTE_NORMAL or FILE_FLAG_RANDOM_ACCESS, 0);
  if (F = INVALID_HANDLE_VALUE) then begin
    System_Log(Format(ResErr, [Filename]));
    Exit;
  end;

  Result := nil;
  FileInfo.uncompressed_size := GetFileSize(F, nil);
  try
    GetMem(Data, FileInfo.uncompressed_size);
  except
    CloseHandle(F);
    System_Log(Format(ResErr, [Filename]));
    Exit;
  end;

  if (not ReadFile(F, Data^, FileInfo.uncompressed_size, BytesRead, nil)) then begin
    CloseHandle(F);
    FreeMem(Data);
    System_Log(Format(ResErr, [Filename]));
    Exit;
  end;

  Result := TResource.Create(Data, BytesRead);
  CloseHandle(F);
  if Assigned(Size) then
    Size^ := BytesRead;
end;

function THGE.Resource_MakePath(const Filename:string = ''):string;
var
  I:Integer;
begin
  if (Filename = '') then
    FTmpFilename := FAppPath
  else if (Filename[1] in ['\', '/', ':']) then
    FTmpFilename := Filename
  else
    FTmpFilename := FAppPath + Filename;

  for I := 1 to Length(FTmpFilename) do
    if (FTmpFilename[I] = '/') then
      FTmpFilename[I] := '\';

  Result := FTmpFilename;
end;

procedure THGE.Resource_RemoveAllPacks;
var
  ResItem, ResNextItem:PResourceList;
begin
  ResItem := FRes;
  while Assigned(ResItem) do begin
    ResNextItem := ResItem.Next;
    Dispose(ResItem);
    ResItem := ResNextItem;
  end;
  FRes := nil;
end;

procedure THGE.Resource_RemovePack(const Filename:string);
var
  Name:string;
  ResItem, ResPrev:PResourceList;
begin
  ResItem := FRes;
  ResPrev := nil;
  Name := UpperCase(Resource_MakePath(Filename));

  while Assigned(ResItem) do begin
    if (Name = ResItem.Filename) then begin
      if Assigned(ResPrev) then
        ResPrev.Next := ResItem.Next
      else
        FRes := ResItem.Next;
      Dispose(ResItem);
      Break;
    end;
    ResPrev := ResItem;
    ResItem := ResItem.Next;
  end;
end;

procedure THGE.SetBlendMode(const Blend:Integer);
begin
  FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
  FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);

  case Blend of

    Blend_Default:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
      end;

    Blend_SrcAlphaColor:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCCOLOR);
      end;

    Blend_ColorAdd:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_ADD);
      end;
    Blend_Add:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ONE);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);

      end;
    Blend_SrcAlphaAdd:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
      end;
    Blend_SrcColor:begin
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCCOLOR);
      end;
    BLEND_SrcColorAdd:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
      end;
    Blend_Invert:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_INVDESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ZERO);
      end;
    Blend_SrcBright:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_SRCCOLOR);
      end;
    Blend_Multiply:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ZERO);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_SRCCOLOR);
      end;
    Blend_InvMultiply:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ZERO);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCCOLOR);
      end;
    Blend_MultiplyAlpha:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ZERO);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_SRCALPHA);
      end;
    Blend_InvMultiplyAlpha:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ZERO);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
      end;
    Blend_DestBright:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_DESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_DESTCOLOR);
      end;
    Blend_InvSrcBright:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_INVSRCCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCCOLOR);
      end;
    Blend_InvDestBright:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_INVDESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVDESTCOLOR);
      end;
    Blend_Bright:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE2X);
      end;
    Blend_BrightAdd:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE4X);
      end;
    Blend_GrayScale:begin
        FD3DDevice.SetRenderState(D3DRS_ALPHABLENDENABLE, Integer(False));
        FD3DDevice.SetRenderState(D3DRS_TextureFactor, Integer((ARGB(255, 255, 155, 155))));
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_DOTPRODUCT3);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLORARG2, D3DTA_TFACTOR);
      end;
    Blend_Light:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_DESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE2X);
      end;
    Blend_LightAdd:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_DESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE4X);
      end;
    Blend_Add2X:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE2X);
      end;
    Blend_OneColor:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
        FD3DDevice.SetTextureStageState(0, D3DTSS_COLOROP, 25);
        FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
      end;
    Blend_XOR:begin
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_INVDESTCOLOR);
        FD3DDEvice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCCOLOR);
      end;

    fxNone:begin
        FD3DDevice.SetRenderState(D3DRS_ALPHABLENDENABLE, iTrue);
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ONE);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ZERO);
        FD3DDevice.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_DISABLE);
      end;
    fxBlend:begin
        FD3DDevice.SetRenderState(D3DRS_ALPHABLENDENABLE, iTrue);
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
      end;
    fxAnti:begin
        FD3DDevice.SetRenderState(D3DRS_ALPHABLENDENABLE, iTrue);
        FD3DDevice.SetRenderState(D3DRS_SRCBLEND, D3DBLEND_INVDESTCOLOR);
        FD3DDevice.SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);
      end;

  end;

  FCurBlendMode := Blend;
end;

procedure THGE.SetProjectionMatrix(const Width, Height:Integer);
var
  Tmp:TD3DXMatrix;
begin
  D3DXMatrixScaling(FMatProj, 1.0, -1.0, 1.0);
  D3DXMatrixTranslation(Tmp, -0.5, Height + 0.5, 0.0);
  D3DXMatrixMultiply(FMatProj, FMatProj, Tmp);
  D3DXMatrixOrthoOffCenterLH(Tmp, 0, Width, 0, Height, 0.0, 1.0);
  D3DXMatrixMultiply(FMatProj, FMatProj, Tmp);
end;

function THGE.System_GetErrorMessage:string;
begin
  Result := FError;
end;

function THGE.System_GetState(const State:THGEStringState):string;
begin
  case State of
    HGE_ICON:;
    HGE_TITLE:;
    HGE_INIFILE:;
    HGE_LOGFILE:
      Result := FLogFilePath;
    else
      Result := '';
  end;
end;

function THGE.System_GetState(const State:THGEIntState):Integer;
begin
  case State of
    HGE_SCREENWIDTH:
      Result := FScreenWidth;
    HGE_SCREENHEIGHT:
      Result := FScreenHeight;
    HGE_SCREENBPP:
      Result := FScreenBPP;
    HGE_SAMPLERATE:
      Result := FSampleRate;
    HGE_FXVOLUME:
      Result := FFXVolume;
    HGE_MUSVOLUME:
      Result := FMusVolume;
    HGE_FPS:
      Result := FHGEFPS;
    else
      Result := 0;
  end;
end;

function THGE.System_GetState(const State:THGEFuncState):THGECallback;
begin
  case State of
    HGE_FRAMEFUNC:
      Result := FProcFrameFunc;
    HGE_RENDERFUNC:
      Result := FProcRenderFunc;
    HGE_FOCUSLOSTFUNC:
      Result := FProcFocusLostFunc;
    HGE_FOCUSGAINFUNC:
      Result := FProcFocusGainFunc;
    HGE_EXITFUNC:
      Result := FProcExitFunc;
    else
      Result := nil;
  end;
end;

function THGE.System_GetState(const State:THGEBoolState):Boolean;
begin
  Result := False;
  case State of
    HGE_WINDOWED:
      Result := FWindowed;
    HGE_ZBUFFER:
      Result := FZBuffer;
    HGE_TEXTUREFILTER:
      Result := FTextureFilter;
    HGE_USESOUND:
      Result := FUseSound;
    HGE_DONTSUSPEND:
      Result := FDontSuspend;
    HGE_HIDEMOUSE:;

    HGE_HARDWARE:
      Result := FHardware;
  end;
end;

function THGE.System_GetState(const State:THGEHWndState):HWnd;
begin
  case State of
    HGE_HWND:
      Result := FWnd;
    HGE_HWNDPARENT:
      Result := FWndParent;
    else
      Result := 0;
  end;
end;

function THGE.System_Initiate:Boolean;
var
  OSVer:TOSVersionInfo;
  MemSt:TMemoryStatus;
  TM:TSystemTime;
begin
  Result := False;

  // Log system info

  System_Log('HGE Started..' + CRLF);

  System_Log('HGE version: %x.%x',
    [HGE_VERSION shr 8, HGE_VERSION and $FF]);
  GetLocalTime(TM);
  System_Log('Date: %02d.%02d.%d, %02d:%02d:%02d' + CRLF,
    [TM.wDay, TM.wMonth, TM.wYear, TM.wHour, TM.wMinute, TM.wSecond]);

  OSVer.dwOSVersionInfoSize := SizeOf(OSVer);
  GetVersionEx(OSVer);
  System_Log('OS: Windows %d.%d.%d',
    [OSVer.dwMajorVersion, OSVer.dwMinorVersion, OSVer.dwBuildNumber]);

  GlobalMemoryStatus(MemSt);
  System_Log('Memory: %dK total, %dK free' + CRLF,
    [MemSt.dwTotalPhys div 1024, MemSt.dwAvailPhys div 1024]);

  if (not GfxInit) then begin
    System_Shutdown;
    Exit;
  end;

  System_Log('Init done.' + CRLF);

  Result := True;
end;

function THGE.System_Launch(const Url:string):Boolean;
begin
  if (ShellExecute(FWnd, nil, PChar(Url), nil, nil, SW_SHOWMAXIMIZED) > 32) then
    Result := True
  else
    Result := False;
end;

procedure THGE.System_Log(const S:string);
begin
  System_Log(S, []);
end;

procedure THGE.System_Log(const Format:string; const Args:array of const);

function IntToStr2(n:Integer):string;
  begin
    if n < 10 then Result := '0' + IntToStr(n)
    else Result := IntToStr(n);
  end;
var
  S, sFilePath, sFileName:string;
  //BytesWritten: Cardinal;
  TF:TextFile;
  Year, Month, Day:Word;
begin
  if (FLogFilePath = '') then Exit;

  DecodeDate(Now, Year, Month, Day);
  try
    sFilePath := FLogFilePath + 'debug\' + IntToStr(Year) + '-' + IntToStr2(Month) + '\';
    if not DirectoryExists(sFilePath) then begin
      ForceDirectories(sFilePath);
    end;
    sFileName := sFilePath + IntToStr2(Day) + '.txt';
    try
      if FileExists(sFileName) then begin
        AssignFile(TF, sFileName);
        Append(TF);
      end else begin
        AssignFile(TF, sFileName);
        Rewrite(TF);
      end;
      S := SysUtils.Format(Format, Args);
      Writeln(TF, S);
    finally
      CloseFile(TF);
    end;
  except
  end;
end;

procedure THGE.System_Log2(const Format:string; const Args:array of const);

function IntToStr2(n:Integer):string;
  begin
    if n < 10 then Result := '0' + IntToStr(n)
    else Result := IntToStr(n);
  end;
var
  S, sFilePath, sFileName:string;
  //BytesWritten: Cardinal;
  TF:TextFile;
  Year, Month, Day:Word;
begin
  if (FLogFilePath = '') then Exit;

  DecodeDate(Now, Year, Month, Day);

  sFilePath := FLogFilePath + 'debug\' + IntToStr(Year) + '-' + IntToStr2(Month) + '\';
  if not DirectoryExists(sFilePath) then begin
    ForceDirectories(sFilePath);
  end;
  sFileName := sFilePath + IntToStr2(Day) + '.txt';
  try
    if FileExists(sFileName) then begin
      AssignFile(TF, sFileName);
      Append(TF);
    end else begin
      AssignFile(TF, sFileName);
      Rewrite(TF);
    end;
    S := SysUtils.Format(Format, Args);
    Writeln(TF, S);
  finally
    CloseFile(TF);
  end;
end;

procedure THGE.System_SetState(const State:THGEStringState;
  const Value:string);
begin
  case State of
    HGE_ICON:begin

      end;
    HGE_TITLE:begin

      end;
    HGE_INIFILE:;
    HGE_LOGFILE:
      if (Value <> '') then begin
        // FLogFile := Resource_MakePath(Value);
        FLogFilePath := Value;
        { HF := CreateFile(PChar(FLogFile), GENERIC_WRITE, 0, nil, CREATE_ALWAYS,
           FILE_ATTRIBUTE_NORMAL, 0);
         if (HF = INVALID_HANDLE_VALUE) then
           FLogFile := ''
         else
           CloseHandle(HF);
       end else
         FLogFile := '';  }
      end;
  end;
end;

procedure THGE.System_SetState(const State:THGEIntState;
  const Value:Integer);
begin
  case State of
    HGE_SCREENWIDTH:
      if (FD3DDevice = nil) then
        FScreenWidth := Value;
    HGE_SCREENHEIGHT:
      if (FD3DDevice = nil) then
        FScreenHeight := Value;
    HGE_SCREENBPP:
      if (FD3DDevice = nil) then
        FScreenBPP := Value;
    HGE_SAMPLERATE:;

    HGE_FXVOLUME:begin

      end;
    HGE_MUSVOLUME:begin

      end;
    HGE_FPS:begin
        if Assigned(FVertArray) then
          Exit;
        if Assigned(FD3DDevice) then begin
          if (((FHGEFPS >= 0) and (Value < 0)) or ((FHGEFPS < 0) and (Value >= 0))) then begin
            if (Value = HGEFPS_VSYNC) then begin
              FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY_VSYNC;
              FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_ONE;
            end else begin
              FD3DPPW.SwapEffect := D3DSWAPEFFECT_COPY;
              FD3DPPFS.FullScreen_PresentationInterval := D3DPRESENT_INTERVAL_IMMEDIATE;
            end;
            //            if Assigned(FProcFocusLostFunc) then
            //              FProcFocusLostFunc;
            GfxRestore();
            //            if Assigned(FProcFocusGainFunc) then
            //              FProcFocusGainFunc;
          end;
        end;
      end;
  end;
end;

procedure THGE.System_SetState(const State:THGEBoolState;
  const Value:Boolean);
begin
  case State of
    HGE_WINDOWED:begin
        if (Assigned(FVertArray) or (FWndParent <> 0)) then
          Exit;
        if (Assigned(FD3DDevice) and (FWindowed <> Value)) then begin
          if (FD3DPPW.BackBufferFormat = D3DFMT_UNKNOWN)
            or (FD3DPPFS.BackBufferFormat = D3DFMT_UNKNOWN)
            then
            Exit;

          FWindowed := Value;
          if (FWindowed) then
            FD3DPP := @FD3DPPW
          else
            FD3DPP := @FD3DPPFS;

          if (FormatId(FD3DPPW.BackBufferFormat) < 4) then
            FScreenBPP := 16
          else
            FScreenBPP := 32;

          GfxRestore;

        end else
          FWindowed := Value;
      end;
    HGE_ZBUFFER:
      if (FD3DDevice = nil) then
        FZBuffer := Value;
    HGE_TEXTUREFILTER:begin
        FTextureFilter := Value;
        if Assigned(FD3DDevice) then begin
          RenderBatch;
          if (FTextureFilter) then begin
            FD3DDevice.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTEXF_LINEAR);
            FD3DDevice.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTEXF_LINEAR);
          end else begin
            FD3DDevice.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTEXF_POINT);
            FD3DDevice.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTEXF_POINT);
          end;
        end;
      end;
    HGE_USESOUND:begin

      end;
    HGE_HIDEMOUSE:;
    HGE_HARDWARE:
      if (FD3DDevice = nil) then
        FHardware := Value;

    HGE_DONTSUSPEND:
      FDontSuspend := Value;
  end;
end;

procedure THGE.System_SetState(const State:THGEFuncState;
  const Value:THGECallback);
begin
  case State of
    HGE_FRAMEFUNC:
      FProcFrameFunc := Value;
    HGE_RENDERFUNC:
      FProcRenderFunc := Value;
    HGE_FOCUSLOSTFUNC:
      FProcFocusLostFunc := Value;
    HGE_FOCUSGAINFUNC:
      FProcFocusGainFunc := Value;
    HGE_GFXRESTOREFUNC:
      FProcGfxRestoreFunc := Value;
    HGE_EXITFUNC:
      FProcExitFunc := Value;
  end;
end;

procedure THGE.System_SetState(const State:THGEHWndState;
  const Value:HWnd);
begin
  case State of
    HGE_HWND:FWnd := Value;

    HGE_HWNDPARENT:
      if (FWnd = 0) then
        FWndParent := Value;
  end;
end;

procedure THGE.System_Shutdown;
begin
  System_Log(CRLF + 'Finishing..');

  GfxDone;

  FWnd := 0;

  System_Log('The End.');
end;

procedure THGE.System_Snapshot(const Filename:string);
var
  Surf:IDirect3DSurface8;
begin
  if Assigned(FD3DDevice) then begin
    FD3DDevice.GetBackBuffer(0, D3DBACKBUFFER_TYPE_MONO, Surf);
    D3DXSaveSurfaceToFile(PChar(Filename), D3DXIFF_BMP, Surf, nil, nil);
  end;
end;

function THGE.Target_Create(const Width, Height:Integer;
  const ZBuffer:Boolean):TTarget;
var
  DXTexture:IDirect3DTexture8;
  Depth:IDirect3DSurface8;
  Desc:TD3DSurfaceDesc;
begin
  Result := nil;

  if (Failed(D3DXCreateTexture(FD3DDevice, Width, Height, 1, D3DUSAGE_RENDERTARGET,
    FD3DPP.BackBufferFormat, D3DPOOL_DEFAULT, DXTexture)))
    then begin
    System_Log('Can''t create render target texture');
    Exit;
  end;

  DXTexture.GetLevelDesc(0, Desc);

  if (ZBuffer) then begin
    if (Failed(FD3DDevice.CreateDepthStencilSurface(Desc.Width, Desc.Height,
      D3DFMT_D16, D3DMULTISAMPLE_NONE, Depth)))
      then begin
      DXTexture := nil;
      System_Log('Can''t create render target depth buffer');
      Exit;
    end;
  end else
    Depth := nil;

  Result := TTarget.Create(DXTexture, Depth, Desc.Width, Desc.Height);
end;

function THGE.Target_GetTexture(const Target:TTarget):TTexture;
begin
  Result := Target;
end;

function THGE.Texture_Create(const Width, Height:Integer; AFormat:TD3DFormat):TTexture;
var
  PTex:IDirect3DTexture8;
  Status:HResult;
begin
  Lock;

  Status := D3DXCreateTexture(FD3DDevice, Width, Height,
    1, // Mip levels
    0, // Usage
    AFormat, // Format
    D3DPOOL_MANAGED, // Memory pool
    PTex);

  if Failed(Status) then begin //GetLastError=8 没有足够的可用存储空间
    System_Log(Format('Can''t create texture Width:%d Height:%d Error:%s Code:%d', [Width, Height, FailString(Status), GetLastError]));
    Result := nil
  end else
    Result := TTexture.Create(PTex, Width, Height);

  UnLock;
end;

function THGE.Texture_GetHeight(const Tex:TTexture;
  const Original:Boolean):Integer;
begin
  Result := Tex.GetHeight(Original);
end;

function THGE.Texture_GetWidth(const Tex:TTexture;
  const Original:Boolean):Integer;
begin
  Result := Tex.GetWidth(Original);
end;

function THGE.Texture_LoadJPEG2000(const Data:Pointer;
  const Size:Longword; const Mipmap:Boolean;
  const Format:TOPJ_CodecFormat):TTexture;
var
  Params:TOPJ_DParameters;
  Info:POPJ_DInfo;
  CIO:POPJ_CIO;
  Image:POPJ_Image;
  I, MipmapLevels, X, Y:Integer;
  PTex:IDirect3DTexture8;
  LR:TD3DLockedRect;
  SD:TD3DSurfaceDesc;
  SR, SG, SB:PByte;
  D1, D2:PCardinal;
begin
  Result := nil;

  opj_set_default_decoder_parameters(Params);
  Info := opj_create_decompress(Format);
  if (Info = nil) then begin
    System_Log('Cannot load JPEG2000 image');
    Exit;
  end;

  CIO := nil;
  Image := nil;
  try
    CIO := opj_cio_open(Info, Data, Size);
    if (CIO = nil) then begin
      System_Log('Cannot load JPEG2000 image');
      Exit;
    end;
    opj_setup_decoder(Info, Params);

    Image := opj_decode(Info, CIO);
    if (Image = nil) then begin
      System_Log('Cannot load JPEG2000 image');
      Exit;
    end;

    { Only support RGB, 8 bits/channel images }
    if (Image.NumComps <> 3) or (Image.ColorSpace <> ClrSpcSRGB) or (Image.Comps[0].Prec <> 8) then begin
      System_Log('Unsupported  JPEG2000 image');
      Exit;
    end;

    { Don't support subsampled images and signed images }
    for I := 0 to 2 do
      if (Image.Comps[I].DX <> 1) or (Image.Comps[I].DY <> 1)
        or (Image.Comps[I].Sgnd = 1)
        then begin
        System_Log('Unsupported  JPEG2000 image');
        Exit;
      end;

    { Create texture }
    if (Mipmap) then
      MipmapLevels := 0
    else
      MipmapLevels := 1;

    if (Failed(D3DXCreateTexture(FD3DDevice, Image.X1, Image.Y1, MipmapLevels, 0,
      D3DFMT_A8R8G8B8, D3DPOOL_MANAGED, PTex)))
      then begin
      System_Log('Can''t create texture');
      Exit;
    end;

    if (Failed(PTex.GetLevelDesc(0, SD))) then begin
      System_Log('Can''t retrieve texture description');
      Exit;
    end;

    if (Failed(PTex.LockRect(0, LR, nil, 0))) then begin
      System_Log('Can''t lock texture');
      Exit;
    end;

    try
      { Copy image to texture }
      D1 := LR.pBits;
      for Y := 0 to Image.Y1 - 1 do begin
        SR := @Image.Comps[0].Data[Y * Image.X1];
        SG := @Image.Comps[1].Data[Y * Image.X1];
        SB := @Image.Comps[2].Data[Y * Image.X1];
        D2 := D1;
        for X := 0 to Image.X1 - 1 do begin
          D2^ := $FF000000 or SB^ or (SG^ shl 8) or (SR^ shl 16);
          Inc(SR, 4);
          Inc(SG, 4);
          Inc(SB, 4);
          Inc(D2);
        end;
        Inc(PByte(D1), LR.Pitch);
      end;
    finally
      PTex.UnlockRect(0);
    end;

    { Create mipmap levels if specified }
    if (MipMap) then
      D3DXFilterTexture(PTex, nil, 0, D3DX_DEFAULT);

    Result := TTexture.Create(PTex, SD.Width, SD.Height);
  finally
    opj_image_destroy(Image);
    opj_destroy_decompress(Info);
    opj_cio_close(CIO);
  end;
end;

function THGE.Texture_Load(const Filename:string;
  const Mipmap:Boolean):TTexture;
var
  Data:IResource;
  Size:Longword;
begin
  Data := Resource_Load(Filename, @Size);
  if (Data = nil) then
    Result := nil
  else begin
    Result := Texture_Load(Data.Handle, Size, Mipmap);
    Data := nil;
  end;
end;

function THGE.Texture_LoadDDS(const AFormat:TD3DFormat; const Data:Pointer; const Size:Longword;
  const Mipmap:Boolean):TTexture;
var
  PTex:IDirect3DTexture8;
  Info:TD3DXImageInfo;
  MipmapLevels:Integer;
begin
  Result := nil;

  if (Mipmap) then
    MipmapLevels := 0
  else
    MipmapLevels := 1;

  if (Succeeded(D3DXCreateTextureFromFileInMemoryEx(FD3DDevice, Data, Size,
    D3DX_DEFAULT, D3DX_DEFAULT,
    MipmapLevels, // Mip levels
    0, // Usage
    AFormat, // Format
    D3DPOOL_MANAGED, // Memory pool
    D3DX_FILTER_NONE, // Filter
    D3DX_DEFAULT, // Mip filter
    0, // Color key
    @Info, nil, PTex)))
    then
    Result := TTexture.Create(PTex, Info.Width, Info.Height);
end;

function THGE.Texture_Load(const Data:Pointer; const Size:Longword;
  const Mipmap:Boolean):TTexture;
var
  Fmt1, Fmt2:TD3DFormat;
  PTex:IDirect3DTexture8;
  Info:TD3DXImageInfo;
  MipmapLevels:Integer;
begin
  Result := nil;
  { Check for JPEG2000 file (check for JP2 or J2K header).
    Use JPEG2000 extension to load this texture. }
  if (PLongword(Data)^ = $51FF4FFF) then begin
    Result := Texture_LoadJPEG2000(Data, Size, Mipmap, CodecJ2K);
    Exit;
  end else if (PInt64(Data)^ = $2020506A0C000000) then begin
    Result := Texture_LoadJPEG2000(Data, Size, Mipmap, CodecJP2);
    Exit;
  end;

  if (PLongword(Data)^ = $20534444) then begin // Compressed DDS format magic number
    Fmt1 := D3DFMT_UNKNOWN;
    Fmt2 := D3DFMT_A8R8G8B8;
  end else begin
    Fmt1 := D3DFMT_A8R8G8B8;
    Fmt2 := D3DFMT_UNKNOWN;
  end;

  //  if( FAILED( D3DXCreateTextureFromFileInMemory( pD3DDevice, data, _size, &pTex ) ) ) pTex=NULL;
  if (Mipmap) then
    MipmapLevels := 0
  else
    MipmapLevels := 1;

  if (Failed(D3DXCreateTextureFromFileInMemoryEx(FD3DDevice, Data, Size,
    D3DX_DEFAULT, D3DX_DEFAULT,
    MipmapLevels, // Mip levels
    0, // Usage
    Fmt1, // Format
    D3DPOOL_MANAGED, // Memory pool
    D3DX_FILTER_NONE, // Filter
    D3DX_DEFAULT, // Mip filter
    0, // Color key
    @Info, nil, PTex)))
    then
    if (Failed(D3DXCreateTextureFromFileInMemoryEx(FD3DDevice, Data, Size,
      D3DX_DEFAULT, D3DX_DEFAULT,
      MipmapLevels, // Mip levels
      0, // Usage
      Fmt2, // Format
      D3DPOOL_MANAGED, // Memory pool
      D3DX_FILTER_NONE, // Filter
      D3DX_DEFAULT, // Mip filter
      0, // Color key
      @Info, nil, PTex)))
      then begin
      System_Log('Can''t create texture');
      Exit;
    end;

  Result := TTexture.Create(PTex, Info.Width, Info.Height);
end;

function THGE.Texture_Load(const ImageData:Pointer;
  const ImageSize:Longword; const AlphaData:Pointer;
  const AlphaSize:Longword; const Mipmap:Boolean):TTexture;
var
  ImageTexture, AlphaTexture:TTexture;
  A1, A2, I1, I2:PLongword;

  Bits:Pointer;
  Pitch:Integer;

  Bits1:Pointer;
  Pitch1:Integer;

  AlphaMask:Longword;
  AlphaShift, I, ImageWidth, ImageHeight, AlphaWidth, AlphaHeight:Integer;
  Width, Height, X, Y:Integer;
begin
  Result := nil;
  ImageTexture := Texture_Load(ImageData, ImageSize, False);
  if (ImageTexture = nil) then
    Exit;

  AlphaTexture := Texture_Load(AlphaData, AlphaSize, False);
  if (AlphaTexture = nil) then
    Exit;

  ImageWidth := ImageTexture.GetWidth;
  ImageHeight := ImageTexture.GetHeight;
  AlphaWidth := AlphaTexture.GetWidth;
  AlphaHeight := AlphaTexture.GetHeight;
  Width := Min(ImageWidth, AlphaWidth);
  Height := Min(ImageHeight, AlphaHeight);

  { Check if AlphaTexture has a valid alpha channel. If so, use alpha channel
    for masking. If not, use green channel for masking (it's assumed that the
    alpha image contains a greyscale mask image). }
  AlphaMask := $0000FF00;
  AlphaShift := 16;
  if AlphaTexture.Lock(Bits, Pitch) then begin
    A1 := Bits;
    try
      for I := 0 to AlphaWidth * AlphaHeight - 1 do begin
        if ((A1^ and $FF000000) <> $FF000000) then begin
          AlphaMask := $FF000000;
          AlphaShift := 0;
          Break;
        end;
        Inc(A1);
      end;
    finally
      AlphaTexture.Unlock;
    end;
  end;
  { Apply alpha information in AlphaTexture to alpha channel in ImageTexture }
  if ImageTexture.Lock(Bits, Pitch, False) then begin
    I1 := Bits;
    try
      if AlphaTexture.Lock(Bits1, Pitch1) then begin
        A1 := Bits1;
        try
          for Y := 0 to Height - 1 do begin
            A2 := A1;
            I2 := I1;
            for X := 0 to Width - 1 do begin
              I2^ := (I2^ and $00FFFFFF) or ((A2^ and AlphaMask) shl AlphaShift);
              Inc(A2);
              Inc(I2);
            end;
            Inc(A1, AlphaWidth);
            Inc(I1, ImageWidth);
          end;
        finally
          AlphaTexture.Unlock;
        end;
      end;
    finally
      ImageTexture.Unlock;
    end;
  end;

  Result := ImageTexture;
  //AlphaTexture := nil;
end;

function THGE.Texture_Load(const ImageFilename, AlphaFilename:string;
  const Mipmap:Boolean):TTexture;
var
  ImageData, AlphaData:IResource;
  ImageSize, AlphaSize:Longword;
begin
  Result := nil;

  ImageData := Resource_Load(ImageFilename, @ImageSize);
  if (ImageData = nil) then
    Exit;

  AlphaData := Resource_Load(AlphaFilename, @AlphaSize);
  if (AlphaData = nil) then
    Exit;

  Result := Texture_Load(ImageData.Handle, ImageSize,
    AlphaData.Handle, AlphaSize, Mipmap);
  ImageData := nil;
  AlphaData := nil;
end;

function THGE.Texture_Lock(const Tex:TTexture; const Rect:TRect;
  out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean):Boolean;
begin
  Result := Tex.Lock(Rect, Bits, Pitch, ReadOnly);
end;

function THGE.Texture_Lock(const Tex:TTexture;
  out Bits:Pointer; out Pitch:Integer; const ReadOnly:Boolean):Boolean;
begin
  Result := Tex.Lock(Bits, Pitch, ReadOnly);
end;

procedure THGE.Texture_Unlock(const Tex:TTexture);
begin
  Tex.Unlock;
end;

procedure THGE.CopyVertices(pVertices:PByte; numVertices:Integer);
var
  pVB:PByte;
begin
  FD3DDevice.SetVertexShader(D3DFVF_HGEVERTEX);
  FVB.Lock(0, SizeOf(THGEVertex) * numVertices, pVB, D3DLOCK_DISCARD);
  Move(pVertices^, pVB^, Sizeof(THGEVertex) * numVertices);
  FVB.Unlock;
  FD3DDevice.SetStreamSource(0, FVB, Sizeof(THGEVertex));
end;

procedure THGE.SetGamma(Red, Green, Blue, Brightness, Contrast:Byte);
var
  FGammaRamp:TD3DGammaRamp;
  k:single;
  k2, i:integer;
begin
  for i := 0 to 255 do begin
    FGammaRamp.red[i] := i * (Red + 1);
    FGammaRamp.green[i] := i * (Green + 1);
    FGammaRamp.blue[i] := i * (Blue + 1);
  end;

  with FGammaRamp do begin
    k := (Contrast / 128) - 1;
    if (k < 1) then
      for i := 0 to 255 do begin
        if (Red[i] > 32767.5) then
          Red[i] := Min(Round(Red[i] + (Red[i] - 32767.5) * k), 65535)
        else Red[i] := Max(Round(Red[i] - (32767.5 - Red[i]) * k), 0);
        if (Green[i] > 32767.5) then
          Green[i] := Min(Round(Green[i] + (Green[i] - 32767.5) * k), 65535)
        else Green[i] := Max(Round(Green[i] - (32767.5 - Green[i]) * k), 0);
        if (Blue[i] > 32767.5) then
          Blue[i] := Min(Round(Blue[i] + (Blue[i] - 32767.5) * k), 65535)
        else Blue[i] := Max(Round(Blue[i] - (32767.5 - Blue[i]) * k), 0);
      end else
      for i := 0 to 255 do begin
        if (Red[i] > 32767.5) then
          Red[i] := Max(Round(Red[i] - (Red[i] - 32767.5) * k), 32768)
        else Red[i] := Min(Round(Red[i] + (32767.5 - Red[i]) * k), 32768);
        if (Green[i] > 32767.5) then
          Green[i] := Max(Round(Green[i] - (Green[i] - 32767.5) * k), 32768)
        else Green[i] := Min(Round(Green[i] + (32767.5 - Green[i]) * k), 32768);
        if (Blue[i] > 32767.5) then
          Blue[i] := Max(Round(Blue[i] - (Blue[i] - 32767.5) * k), 32768)
        else Blue[i] := Min(Round(Blue[i] + (32767.5 - Blue[i]) * k), 32768);
      end;

    k2 := round(((Brightness / 128) - 1) * 65535);
    if (k2 < 0) then
      for i := 0 to 255 do begin
        red[i] := Max(red[i] + k2, 0);
        green[i] := Max(green[i] + k2, 0);
        blue[i] := Max(blue[i] + k2, 0);
      end
    else
      for i := 0 to 255 do begin
        red[i] := Min(red[i] + k2, 65535);
        green[i] := Min(green[i] + k2, 65535);
        blue[i] := Min(blue[i] + k2, 65535);
      end;
  end;
  FD3DDevice.SetGammaRamp(0, FGammaRamp);
end;

procedure THGE.Point(X, Y:Single; Color:Cardinal; Z:Single; BlendMode:Integer);
begin
  Vertices[0].X := X;
  Vertices[0].Y := Y;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color;
  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;
  CopyVertices(@Vertices, 1);
  FD3DDevice.DrawPrimitive(D3DPT_POINTLIST, 0, 1);
end;

procedure THGE.Line2Color(X1, Y1, X2, Y2:Single; Color1, Color2:Cardinal; Z:Single; BlendMode:Integer);
begin
  Vertices[0].X := X1;
  Vertices[0].Y := Y1;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color1;
  Vertices[1].X := X2;
  Vertices[1].Y := Y2;
  Vertices[1].Z := Z;
  Vertices[1].Col := Color2;
  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;
  CopyVertices(@vertices, 2);
  FD3DDevice.DrawPrimitive(D3DPT_LINELIST, 0, 1);
end;

procedure THGE.Circle(X, Y, Radius:Single; Color:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  Max, I:Integer;
  Ic, IInc:Single;
begin
  if Radius > 1000 then Radius := 1000;
  Max := Round(Radius);
  IInc := 1 / Max;
  Ic := 0;
  Vertices[0].X := x;
  Vertices[0].Y := y;
  Vertices[0].Z := Z;
  Vertices[0].col := Color;
  for I := 1 to Max + 1 do begin
    Vertices[I].X := X + Radius * Cos(Ic * TwoPI);
    Vertices[I].Y := Y + Radius * Sin(Ic * TwoPI);
    Vertices[I].Z := Z;
    Vertices[I].col := Color;
    Ic := Ic + IInc;
  end;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;
  if not Filled then begin
    Vertices[0].X := Vertices[Max + 1].X;
    Vertices[0].Y := Vertices[Max + 1].Y;
    Vertices[0].Z := Z;
    CopyVertices(@Vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, Max + 1);
  end
  else begin
    CopyVertices(@Vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, Max);
  end;

end;

procedure THGE.Ellipse(X, Y, R1, R2:Single; Color:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  Max, I:Integer;
  Ic, IInc:Single;
begin
  if R1 > 1000 then R1 := 1000;
  Max := Round(R1);
  IInc := 1 / Max;
  Ic := 0;
  Vertices[0].X := X;
  Vertices[0].Y := Y;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color;
  for i := 1 to max + 1 do begin
    Vertices[I].X := X + R1 * Cos(Ic * TwoPI);
    Vertices[I].Y := y + R2 * Sin(Ic * TwoPI);
    Vertices[I].Z := Z;
    Vertices[I].Col := Color;
    Ic := Ic + IInc;
  end;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;

  if not Filled then begin
    Vertices[0].X := Vertices[Max + 1].X;
    Vertices[0].Y := Vertices[Max + 1].Y;
    Vertices[0].Z := Z;
    CopyVertices(@Vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, Max + 1);
  end
  else begin
    CopyVertices(@Vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, Max);
  end;

end;

procedure THGE.Circle(X, Y, Radius:Single; Width:Integer; Color:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  I:Integer;
begin
  if Filled then
    Circle(X, Y, Radius + Width, Color, Filled, Z, BlendMode)
  else
    for I := 0 to 3 * (Width - 1) do
      Circle(X, Y, Radius + I * 0.3, Color, Filled, Z, BlendMode);
end;

procedure THGE.Arc(X, Y, Radius, StartRadius, EndRadius:Single; Color:Cardinal;
  DrawStartEnd, Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  Max, I:Integer;
  Ic, IInc:Single;
begin
  if Radius > 1000 then Radius := 1000;
  Max := Round(Radius);
  IInc := 1 / Max;
  IInc := IInc * (EndRadius - StartRadius) * IRad;
  Ic := StartRadius * IRad;

  Vertices[0].X := X;
  Vertices[0].Y := Y;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color;
  for I := 1 to Max + 1 do begin
    Vertices[I].X := X + Radius * Cos(ic * TwoPI);
    Vertices[I].Y := Y + Radius * Sin(ic * TwoPI);
    Vertices[I].Z := Z;
    Vertices[I].Col := Color;
    Ic := Ic + IInc;
  end;

  if DrawStartEnd then I := 0 else I := 1;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);

  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;

  if not Filled then begin
    Vertices[0].X := Vertices[Max + 1].X;
    vertices[0].Y := vertices[max + 1].Y;
    Vertices[0].Z := Z;
    CopyVertices(@Vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, I, Max + (1 - I));
  end
  else begin
    CopyVertices(@vertices, Max + 2);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, Max);
  end;
end;

procedure THGE.Arc(X, Y, Radius, StartRadius, EndRadius:Single; Color:Cardinal;
  Width:Integer; DrawStartEnd, Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  I:Integer;
begin
  if Filled then
    Arc(X, Y, Radius + Width, StartRadius, EndRadius, Color, DrawStartEnd, Filled, Z, BlendMode)
  else
    for I := 0 to 4 * (Width - 1) do
      Arc(X, Y, Radius + I * 0.15, StartRadius, EndRadius, Color, DrawStartEnd, Filled, Z, BlendMode);
end;

procedure THGE.Triangle(X1, Y1, X2, Y2, X3, Y3:Single; Color:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
begin
  Vertices[0].X := X1;
  Vertices[0].Y := Y1;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color;
  Vertices[1].X := X2;
  Vertices[1].y := Y2;
  Vertices[1].Z := Z;
  Vertices[1].Col := Color;
  Vertices[2].X := X3;
  Vertices[2].Y := Y3;
  Vertices[2].Z := Z;
  Vertices[2].Col := Color;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;

  if Filled then begin
    CopyVertices(@Vertices, 3);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLELIST, 0, 1);
  end
  else begin
    Vertices[3].X := X1;
    Vertices[3].Y := Y1;
    Vertices[3].Z := Z;
    Vertices[3].Col := Color;
    CopyVertices(@Vertices, 4);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, 3);
  end;
end;

procedure THGE.Triangle(X1, Y1, X2, Y2, X3, Y3:Single; Color1, Color2, Color3:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
begin
  Vertices[0].X := X1;
  Vertices[0].Y := Y1;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color1;
  Vertices[1].X := X2;
  Vertices[1].y := Y2;
  Vertices[1].Z := Z;
  Vertices[1].Col := Color2;
  Vertices[2].X := X3;
  Vertices[2].Y := Y3;
  Vertices[2].Z := Z;
  Vertices[2].Col := Color3;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;

  if Filled then begin
    CopyVertices(@Vertices, 3);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLELIST, 0, 1);
  end
  else begin
    Vertices[3].X := X1;
    Vertices[3].Y := Y1;
    Vertices[3].Z := Z;
    Vertices[3].Col := Color1;
    CopyVertices(@Vertices, 4);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, 3);
  end;
end;

procedure THGE.Quadrangle4Color(X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single;
  Color1, Color2, Color3, Color4:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
begin
  Vertices[0].X := X1;
  Vertices[0].Y := Y1;
  Vertices[0].Z := Z;
  Vertices[0].Col := Color1;
  Vertices[1].X := X2;
  Vertices[1].Y := Y2;
  Vertices[1].Z := Z;
  Vertices[1].Col := Color2;
  Vertices[2].X := X3;
  Vertices[2].Y := Y3;
  Vertices[2].Z := Z;
  Vertices[2].Col := Color3;
  Vertices[3].X := X4;
  Vertices[3].y := Y4;
  Vertices[3].Z := Z;
  Vertices[3].Col := Color4;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);
  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;

  if Filled then begin
    CopyVertices(@vertices, 4);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, 2);
  end
  else begin
    Vertices[4].X := X1;
    Vertices[4].Y := Y1;
    Vertices[4].Z := Z;
    Vertices[4].Col := Color1;
    CopyVertices(@vertices, 5);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, 4);
  end;
end;

procedure THGE.Quadrangle(X1, Y1, X2, Y2, X3, Y3, X4, Y4:Single; Color:Cardinal;
  Filled:Boolean; Z:Single; BlendMODE:Integer);
begin
  Quadrangle4Color(X1, Y1, X2, Y2, X3, Y3, X4, Y4, Color, Color, Color, Color, Filled, Z, BlendMode);
end;

procedure THGE.Rectangle(X, Y, Width, Height:Single; Color:Cardinal; Filled:Boolean;
  Z:Single; BlendMODE:Integer);
begin
  Quadrangle4Color(X, Y, X + Width, Y, X + Width, Y + Height,
    X, Y + Height, Color, Color, Color, Color, Filled, Z, BlendMode);
end;

procedure THGE.Polygon(Points:array of TPoint; NumPoints:Integer;
  Color:Cardinal; Filled:Boolean; Z:Single; BlendMODE:Integer);
var
  I:Integer;
begin
  for I := 0 to NumPoints - 1 do begin
    Vertices[I].X := Points[I].X;
    Vertices[I].Y := PointS[I].Y;
    Vertices[I].Z := Z;
    Vertices[I].Col := Color;
  end;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);

  if (FCurTexture <> nil) then begin
    FD3DDevice.SetTexture(0, nil);
    FCurTexture := nil;
  end;
  if Filled then begin
    CopyVertices(@Vertices, NumPoints);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, NumPoints - 2);
  end
  else begin
    Vertices[NumPoints].X := Points[0].X;
    Vertices[NumPoints].y := Points[0].Y;
    Vertices[NumPoints].Z := Z;
    Vertices[NumPoints].Col := Color;
    CopyVertices(@Vertices, NumPoints + 1);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, NumPoints);
  end;
end;

procedure THGE.Polygon(Points:array of TPoint; NumPoints:Integer; Color:Cardinal;
  Filled:Boolean; Tex:TTexture; Z:Single = 0.0; BlendMode:Integer = BLEND_DEFAULT);
var
  I:Integer;
begin
  for I := 0 to NumPoints - 1 do begin
    Vertices[I].X := Points[I].X;
    Vertices[I].Y := PointS[I].Y;
    Vertices[I].Z := Z;
    Vertices[I].Col := Color;
  end;

  RenderBatch;
  FCurPrimType := HGEPRIM_LINES;
  SetBlendMode(BlendMode);

  if (FCurTexture <> Tex) then begin
    if Assigned(Tex) then
      FD3DDevice.SetTexture(0, Tex.Handle)
    else
      FD3DDevice.SetTexture(0, nil);
    FCurTexture := Tex;
  end;
  if Filled then begin
    CopyVertices(@Vertices, NumPoints);
    FD3DDevice.DrawPrimitive(D3DPT_TRIANGLEFAN, 0, NumPoints - 2);
  end
  else begin
    Vertices[NumPoints].X := Points[0].X;
    Vertices[NumPoints].y := Points[0].Y;
    Vertices[NumPoints].Z := Z;
    Vertices[NumPoints].Col := Color;
    CopyVertices(@Vertices, NumPoints + 1);
    FD3DDevice.DrawPrimitive(D3DPT_LINESTRIP, 0, NumPoints);
  end;

end;

procedure THGE.Polygon(Points:array of TPoint; Color:Cardinal; Filled:Boolean;
  Z:Single; BlendMODE:Integer);
begin
  Polygon(Points, High(Points) + 1, Color, Filled, Z, BlendMode);
end;
(****************************************************************************
 * Demo.cpp
 ****************************************************************************)
initialization
  //InitializeCriticalSection(DXCriticalSection);
finalization
  //DeleteCriticalSection(DXCriticalSection);

end.
