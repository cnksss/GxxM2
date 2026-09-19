unit GameImages;

interface
uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  DIB,
  HGE,
  HUtil32;
const
  LOADIMAGEMODE = 0;
  USEMAPSTREAM = 0;

const
  MAX_IMAGE_SIZE = 8 shl 10 shl 10; // 单张图片大小不能超过4M

  MAX_IMAGE_WIDTH = 3200;
  MAX_IMAGE_HEIGHT = 3200;

type
  TDebugTextOut = procedure(Msg:string; boWriteDate:Boolean);
  TLibType = (ltUseCache, ltLoadBmp, ltLoadBmpFile, ltLoadMemory);
  TDXImage = record
    // 添加数据，记录图片附加信息 chongchong 2014-04-02
    nWidth:Word;
    nHeight:Word;

    nPx:SmallInt;
    nPy:SmallInt;

    Bitmap:TBitmap;
    Surface:TTexture;
    Gray:TTexture;
    Bright:TTexture;

    dwLatestTime:LongWord;
    dwLatestGrayTime:LongWord;
    dwLatestBrightTime:LongWord;

    boUpdateStart:Boolean; // 更新状态
    dwUpdateStartTick:LongWord;
    boUpdateStop:Boolean; // 是否不需要更新
  end;
  pTDxImage = ^TDXImage;

  TDxImageArr = array[0..MaxListSize div 4] of TDXImage;
  PTDxImageArr = ^TDxImageArr;

  TGameImages = class
    m_dwFreeMemCheckTick:LongWord;
    m_dwUseCheckTick:LongWord;
    m_dwMemCheckTick:LongWord;

    m_dwMemCheckGrayTick:LongWord;
    m_dwMemCheckBrightTick:LongWord;

    m_nProcIdx:Integer;
    m_nProcGrayIdx:Integer;
    m_nProcBrightIdx:Integer;

    m_boUpdateIndex:Boolean; // 是否需要更新 索引文件
    m_boUpdateIndexing:Boolean; // 正在更新 索引文件
    m_dwUpdateIndexingTick:LongWord;

    m_boNeedUpdate:Boolean; // 是否需要更新
    m_IndexList:TList;

    m_dwMemChecktTick:DWORD;
  private
    FFileName:string; // 0x24
    FIndexFileName:string;
    FImageCount:Integer; // 0x28

    FAppr:Word;
    FBitCount:Byte;
    FInitialized:Boolean;
    FLibType:TLibType;
    FTransparentColor:Cardinal;
    FD3DFormat:Boolean;

    FResetWZLAlpha:Boolean;

    procedure SetFileName(Value:string);
    // FCriticalSection: TRTLCriticalSection;
  protected
    // 传奇加亮和灰度效果 使用自定义渲染特效 PixelShader等方式可以实现，但一些低端机器不支持，
    // 所以这里采用从源头上直接读取加亮和灰度的资源，可保证所有机器都能支持

    function MakeDibByPixelFormat(pf:TPixelFormat; nW, nH:Integer):TDIB;
    function MakeDibByBitCount(nBitCount:Integer; nW, nH:Integer):TDIB; 

    function GetCachedSurface(Index:Integer):TTexture; virtual;
    function GetCachedGray(Index:Integer):TTexture; virtual;
    function GetCachedBright(Index:Integer):TTexture; virtual;
    function GetCachedBitmap(Index:Integer):TBitmap; virtual;

    procedure FreeOldMemorys_Ex;
  public
    IndexList:TList;
    GrayIndexList:TList;
    BrightIndexList:TList;
    m_ImgArr:PTDxImageArr;
    constructor Create();
    destructor Destroy; override;
    procedure OutMessage(const Msg:string; boWriteDate:Boolean = True);
    function GetImageArray(Index:Integer):TTexture;
    function GetGrayArray(Index:Integer):TTexture;
    function GetBrightArray(Index:Integer):TTexture;

    function TryLock:Boolean;
    procedure Lock;
    procedure UnLock;

    procedure Initialize; virtual;
    procedure Finalize; virtual;
    procedure ClearCache; virtual;
    procedure ClearCacheFilter(Filters:TList); virtual;

    function GetCachedImage(Index:Integer; var px, py:Integer):TTexture; virtual;
    function GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture; virtual;
    function GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture; virtual;
    function GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap; virtual;

    function GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean; virtual; abstract;

    function GetNeedUpdate(Index:Integer):Boolean;

    procedure SaveToFile(Index:Integer; const FileName:string);
    procedure UpdateImageDataSize(nIndex:Integer; nDataSize:Integer);virtual;

    property Images[Index:Integer]:TTexture read GetCachedSurface;
    property Grays[Index:Integer]:TTexture read GetCachedGray;
    property Brights[Index:Integer]:TTexture read GetCachedBright;

    property Bitmaps[Index:Integer]:TBitmap read GetCachedBitmap;
    property FileName:string read FFileName write SetFileName;
    property IndexFileName:string read FIndexFileName write FIndexFileName;
    property ImageCount:Integer read FImageCount write FImageCount;
    property BitCount:Byte read FBitCount write FBitCount;
    property Initialized:Boolean read FInitialized write FInitialized;
    property LibType:TLibType read FLibType write FLibType;
    property TransparentColor:Cardinal read FTransparentColor write FTransparentColor;
    property D3DFormat:Boolean read FD3DFormat write FD3DFormat;

    procedure FreeOldGrayMemorys(Index:Integer);
    procedure FreeOldBrightMemorys(Index:Integer);
    procedure FreeOldMemorys(Index:Integer); overload;

    // 加上6秒清理一次参数Clear1Min，用于地图对象 chongchong 2014-03-25
    procedure FreeOldMemorys(Clear1Min:Boolean = False); overload;

    procedure DrawZoom(paper:TCanvas; X, Y, Index:Integer; Zoom:Real);
    procedure DrawZoomEx(paper:TCanvas; X, Y, Index:Integer; Zoom:Real; leftzero:Boolean);

    property ResetWZLAlpha:Boolean read FResetWZLAlpha write FResetWZLAlpha;
  end;

function WidthBytes(BitCount:Byte; Width:Integer):Integer;
function ANS(N:Integer):Integer;
procedure NewBitmapFile(const AWidth, AHeight, ABitCount:Integer; var FileData:Pointer; var FileSize:Integer; RGB565:Boolean = True);

//procedure Convert32(Source: TDIB);
//procedure ConvertBright32(Source: TDIB);
//procedure ConvertGray32(Source: TDIB);

procedure FileData32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
procedure FileDataBright32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
procedure FileDataGray32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
procedure DebugTextOut(Msg:string; boWriteDate:Boolean = True);

function ExtractFileNameOnly(const fname:string):string;
function ExtractFilePath(const FileName:string):string;
var
  g_boD3DFormat:Boolean = False;

  g_NullImage:TTexture = nil;

  g_DebugTextOut:TDebugTextOut = nil;
  g_LoadCriticalSection:TRTLCriticalSection;
implementation


uses Wil,
  Wis,
  Wzl,
  DxCanvas;
var
  ColorArray:array[0..1023] of byte = (
    $00, $00, $00, $00, $00, $00, $80, $00, $00, $80, $00, $00, $00, $80, $80, $00,
    $80, $00, $00, $00, $80, $00, $80, $00, $80, $80, $00, $00, $C0, $C0, $C0, $00,
    $97, $80, $55, $00, $C8, $B9, $9D, $00, $73, $73, $7B, $00, $29, $29, $2D, $00,
    $52, $52, $5A, $00, $5A, $5A, $63, $00, $39, $39, $42, $00, $18, $18, $1D, $00,
    $10, $10, $18, $00, $18, $18, $29, $00, $08, $08, $10, $00, $71, $79, $F2, $00,
    $5F, $67, $E1, $00, $5A, $5A, $FF, $00, $31, $31, $FF, $00, $52, $5A, $D6, $00,
    $00, $10, $94, $00, $18, $29, $94, $00, $00, $08, $39, $00, $00, $10, $73, $00,
    $00, $18, $B5, $00, $52, $63, $BD, $00, $10, $18, $42, $00, $99, $AA, $FF, $00,
    $00, $10, $5A, $00, $29, $39, $73, $00, $31, $4A, $A5, $00, $73, $7B, $94, $00,
    $31, $52, $BD, $00, $10, $21, $52, $00, $18, $31, $7B, $00, $10, $18, $2D, $00,
    $31, $4A, $8C, $00, $00, $29, $94, $00, $00, $31, $BD, $00, $52, $73, $C6, $00,
    $18, $31, $6B, $00, $42, $6B, $C6, $00, $00, $4A, $CE, $00, $39, $63, $A5, $00,
    $18, $31, $5A, $00, $00, $10, $2A, $00, $00, $08, $15, $00, $00, $18, $3A, $00,
    $00, $00, $08, $00, $00, $00, $29, $00, $00, $00, $4A, $00, $00, $00, $9D, $00,
    $00, $00, $DC, $00, $00, $00, $DE, $00, $00, $00, $FB, $00, $52, $73, $9C, $00,
    $4A, $6B, $94, $00, $29, $4A, $73, $00, $18, $31, $52, $00, $18, $4A, $8C, $00,
    $11, $44, $88, $00, $00, $21, $4A, $00, $10, $18, $21, $00, $5A, $94, $D6, $00,
    $21, $6B, $C6, $00, $00, $6B, $EF, $00, $00, $77, $FF, $00, $84, $94, $A5, $00,
    $21, $31, $42, $00, $08, $10, $18, $00, $08, $18, $29, $00, $00, $10, $21, $00,
    $18, $29, $39, $00, $39, $63, $8C, $00, $10, $29, $42, $00, $18, $42, $6B, $00,
    $18, $4A, $7B, $00, $00, $4A, $94, $00, $7B, $84, $8C, $00, $5A, $63, $6B, $00,
    $39, $42, $4A, $00, $18, $21, $29, $00, $29, $39, $46, $00, $94, $A5, $B5, $00,
    $5A, $6B, $7B, $00, $94, $B1, $CE, $00, $73, $8C, $A5, $00, $5A, $73, $8C, $00,
    $73, $94, $B5, $00, $73, $A5, $D6, $00, $4A, $A5, $EF, $00, $8C, $C6, $EF, $00,
    $42, $63, $7B, $00, $39, $56, $6B, $00, $5A, $94, $BD, $00, $00, $39, $63, $00,
    $AD, $C6, $D6, $00, $29, $42, $52, $00, $18, $63, $94, $00, $AD, $D6, $EF, $00,
    $63, $8C, $A5, $00, $4A, $5A, $63, $00, $7B, $A5, $BD, $00, $18, $42, $5A, $00,
    $31, $8C, $BD, $00, $29, $31, $35, $00, $63, $84, $94, $00, $4A, $6B, $7B, $00,
    $5A, $8C, $A5, $00, $29, $4A, $5A, $00, $39, $7B, $9C, $00, $10, $31, $42, $00,
    $21, $AD, $EF, $00, $00, $10, $18, $00, $00, $21, $29, $00, $00, $6B, $9C, $00,
    $5A, $84, $94, $00, $18, $42, $52, $00, $29, $5A, $6B, $00, $21, $63, $7B, $00,
    $21, $7B, $9C, $00, $00, $A5, $DE, $00, $39, $52, $5A, $00, $10, $29, $31, $00,
    $7B, $BD, $CE, $00, $39, $5A, $63, $00, $4A, $84, $94, $00, $29, $A5, $C6, $00,
    $18, $9C, $10, $00, $4A, $8C, $42, $00, $42, $8C, $31, $00, $29, $94, $10, $00,
    $10, $18, $08, $00, $18, $18, $08, $00, $10, $29, $08, $00, $29, $42, $18, $00,
    $AD, $B5, $A5, $00, $73, $73, $6B, $00, $29, $29, $18, $00, $4A, $42, $18, $00,
    $4A, $42, $31, $00, $DE, $C6, $63, $00, $FF, $DD, $44, $00, $EF, $D6, $8C, $00,
    $39, $6B, $73, $00, $39, $DE, $F7, $00, $8C, $EF, $F7, $00, $00, $E7, $F7, $00,
    $5A, $6B, $6B, $00, $A5, $8C, $5A, $00, $EF, $B5, $39, $00, $CE, $9C, $4A, $00,
    $B5, $84, $31, $00, $6B, $52, $31, $00, $D6, $DE, $DE, $00, $B5, $BD, $BD, $00,
    $84, $8C, $8C, $00, $DE, $F7, $F7, $00, $18, $08, $00, $00, $39, $18, $08, $00,
    $29, $10, $08, $00, $00, $18, $08, $00, $00, $29, $08, $00, $A5, $52, $00, $00,
    $DE, $7B, $00, $00, $4A, $29, $10, $00, $6B, $39, $10, $00, $8C, $52, $10, $00,
    $A5, $5A, $21, $00, $5A, $31, $10, $00, $84, $42, $10, $00, $84, $52, $31, $00,
    $31, $21, $18, $00, $7B, $5A, $4A, $00, $A5, $6B, $52, $00, $63, $39, $29, $00,
    $DE, $4A, $10, $00, $21, $29, $29, $00, $39, $4A, $4A, $00, $18, $29, $29, $00,
    $29, $4A, $4A, $00, $42, $7B, $7B, $00, $4A, $9C, $9C, $00, $29, $5A, $5A, $00,
    $14, $42, $42, $00, $00, $39, $39, $00, $00, $59, $59, $00, $2C, $35, $CA, $00,
    $21, $73, $6B, $00, $00, $31, $29, $00, $10, $39, $31, $00, $18, $39, $31, $00,
    $00, $4A, $42, $00, $18, $63, $52, $00, $29, $73, $5A, $00, $18, $4A, $31, $00,
    $00, $21, $18, $00, $00, $31, $18, $00, $10, $39, $18, $00, $4A, $84, $63, $00,
    $4A, $BD, $6B, $00, $4A, $B5, $63, $00, $4A, $BD, $63, $00, $4A, $9C, $5A, $00,
    $39, $8C, $4A, $00, $4A, $C6, $63, $00, $4A, $D6, $63, $00, $4A, $84, $52, $00,
    $29, $73, $31, $00, $5A, $C6, $63, $00, $4A, $BD, $52, $00, $00, $FF, $10, $00,
    $18, $29, $18, $00, $4A, $88, $4A, $00, $4A, $E7, $4A, $00, $00, $5A, $00, $00,
    $00, $88, $00, $00, $00, $94, $00, $00, $00, $DE, $00, $00, $00, $EE, $00, $00,
    $00, $FB, $00, $00, $94, $5A, $4A, $00, $B5, $73, $63, $00, $D6, $8C, $7B, $00,
    $D6, $7B, $6B, $00, $FF, $88, $77, $00, $CE, $C6, $C6, $00, $9C, $94, $94, $00,
    $C6, $94, $9C, $00, $39, $31, $31, $00, $84, $18, $29, $00, $84, $00, $18, $00,
    $52, $42, $4A, $00, $7B, $42, $52, $00, $73, $5A, $63, $00, $F7, $B5, $CE, $00,
    $9C, $7B, $8C, $00, $CC, $22, $77, $00, $FF, $AA, $DD, $00, $2A, $B4, $F0, $00,
    $9F, $00, $DF, $00, $B3, $17, $E3, $00, $F0, $FB, $FF, $00, $A4, $A0, $A0, $00,
    $80, $80, $80, $00, $00, $00, $FF, $00, $00, $FF, $00, $00, $00, $FF, $FF, $00,
    $FF, $00, $00, $00, $FF, $00, $FF, $00, $FF, $FF, $00, $00, $FF, $FF, $FF, $00
    );

function ExtractFilePath(const FileName:string):string;
var
  I:integer;
begin
  I := LastDelimiter(PathDelim + DriveDelim, FileName);
  Result := Copy(FileName, 1, I);
end;

function ExtractFileNameOnly(const fname:string):string;
var
  extpos:integer;
  ext, fn:string;
begin
  ext := ExtractFileExt(fname);
  fn := ExtractFileName(fname);
  if ext <> '' then begin
    extpos := Pos(ext, fn);
    Result := Copy(fn, 1, extpos - 1);
  end
  else
    Result := fn;
end;

procedure DebugTextOut(Msg:string; boWriteDate:Boolean);
begin
  if Assigned(g_DebugTextOut) then
    g_DebugTextOut(Msg, boWriteDate);
end;

function Is2n(N:Integer):Boolean; // 检测是否是2次幂
begin
  if (N = 0) then
    Result := False
  else
    Result := ((N and (N - 1)) = 0) and (N <> 0) and (N <> 1);
end;

function ANS(N:Integer):Integer; // 2次幂
var
  nR, nT:Integer;
begin
  if Is2n(N) then
    Result := N
  else begin
    nR := 1;
    nT := N;
    while (nT <> 0) do begin
      nT := nT shr 1;
      nR := nR shl 1;
    end;
    if (N = 0) or ((N shl 1) = nR) then
      nR := nR shr 1;

    Result := nR;
    if Result in [0, 1] then Result := 2;
  end;
end;

function WidthBytes(BitCount:Byte; Width:Integer):Integer;
begin
  Result := (((Width * BitCount) + 31) div 32) * 4;
end;

const
  MaxPixelCount = 32768;
type
  pRGBArray32 = ^TRGBArray32;
  TRGBArray32 = array[0..MaxPixelCount - 1] of TRGBQuad;

  pRGBArray24 = ^TRGBArray24;
  TRGBArray24 = array[0..MaxPixelCount - 1] of TRGBTriple;

  (*
  procedure Convert32(Source: TDIB);
  var
    X, Y: Integer;

    SrcP: PByte;
    DesP: PByte;

    lsDIB: TDIB;

    OrigRow24: pRGBArray24;
    DestRow32: pRGBArray32;
  begin
    lsDIB := TDIB.Create;
    lsDIB.Assign(Source);
    Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
    Source.SetSize(Source.Width, Source.Height, 32);
    case lsDIB.BitCount of
      8:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTable_8_32Bit[SrcP^];
              Inc(SrcP);
              Inc(PCardinal(DesP));
            end;
          end;
        end;
      16:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];
              Inc(SrcP, 2);
              Inc(DesP, 4);
            end;
          end;
        end;
      24:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            OrigRow24 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];

            for X := 0 to Source.Width - 1 do
            begin
              // 这个判断无意义 chongchong 2014-08-05
              {
              if (OrigRow24[X].rgbtRed = 0) and (OrigRow24[X].rgbtGreen = 0) and (OrigRow24[X].rgbtBlue = 0) then
              begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end
              else
              }
              begin
                DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
                DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
                DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;
        end;
      32:
        begin
          {for Y := 0 to Source.Height - 1 do begin
            OrigRow32 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do begin
              if (OrigRow32[X].rgbRed = 0) and (OrigRow32[X].rgbGreen = 0) and (OrigRow32[X].rgbBlue = 0) then begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end else begin
                DestRow32[X].rgbRed := OrigRow32[X].rgbRed;
                DestRow32[X].rgbGreen := OrigRow32[X].rgbGreen;
                DestRow32[X].rgbBlue := OrigRow32[X].rgbBlue;
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;}
        end;
    end;
    lsDIB.Free;
  end;

  procedure ConvertBright32(Source: TDIB);
  var
    X, Y: Integer;
    SrcP: PByte;
    DesP: PByte;

    lsDIB: TDIB;

    OrigRow24: pRGBArray24;
    OrigRow32, DestRow32: pRGBArray32;
  begin
    lsDIB := TDIB.Create;
    lsDIB.Assign(Source);
    Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
    Source.SetSize(Source.Width, Source.Height, 32);
    case lsDIB.BitCount of
      8:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTableBright_8_32Bit[SrcP^];
              Inc(SrcP);
              Inc(PCardinal(DesP));
            end;
          end;
        end;
      16:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTableBright_16_32Bit[PWord(SrcP)^];
              Inc(SrcP, 2);
              Inc(DesP, 4);
            end;
          end;
        end;
      24:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            OrigRow24 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              if (OrigRow24[X].rgbtRed = 0) and (OrigRow24[X].rgbtGreen = 0) and (OrigRow24[X].rgbtBlue = 0) then
              begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end
              else
              begin
                DestRow32[X].rgbRed := _MIN(255, Round(OrigRow24[X].rgbtRed * 1.3));
                DestRow32[X].rgbGreen := _MIN(255, Round(OrigRow24[X].rgbtGreen * 1.3));
                DestRow32[X].rgbBlue := _MIN(255, Round(OrigRow24[X].rgbtBlue * 1.3));
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;
        end;
      32:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            OrigRow32 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              if (OrigRow32[X].rgbRed = 0) and (OrigRow32[X].rgbGreen = 0) and (OrigRow32[X].rgbBlue = 0) then
              begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end
              else
              begin
                DestRow32[X].rgbRed := _MIN(255, Round(OrigRow32[X].rgbRed * 1.3));
                DestRow32[X].rgbGreen := _MIN(255, Round(OrigRow32[X].rgbGreen * 1.3));
                DestRow32[X].rgbBlue := _MIN(255, Round(OrigRow32[X].rgbBlue * 1.3));
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;
        end;
    end;
    lsDIB.Free;
  end;

  procedure ConvertGray32(Source: TDIB);
  var
    X, Y: Integer;
    SrcP: PByte;
    DesP: PByte;

    lsDIB: TDIB;

    OrigRow24: pRGBArray24;
    OrigRow32, DestRow32: pRGBArray32;
  begin
    lsDIB := TDIB.Create;
    lsDIB.Assign(Source);
    Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
    Source.SetSize(Source.Width, Source.Height, 32);
    case lsDIB.BitCount of
      8:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTableGray_8_32Bit[SrcP^];
              Inc(SrcP);
              Inc(PCardinal(DesP));
            end;
          end;
        end;
      16:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            SrcP := lsDIB.ScanLine[Y];
            DesP := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              PCardinal(DesP)^ := ColorTableGray_16_32Bit[PWord(SrcP)^];
              Inc(SrcP, 2);
              Inc(DesP, 4);
            end;
          end;
        end;
      24:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            OrigRow24 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              if (OrigRow24[X].rgbtRed = 0) and (OrigRow24[X].rgbtGreen = 0) and (OrigRow24[X].rgbtBlue = 0) then
              begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end
              else
              begin
                DestRow32[X].rgbRed := g_Grays[OrigRow24[X].rgbtRed + OrigRow24[X].rgbtGreen + OrigRow24[X].rgbtBlue];
                DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
                DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;
        end;
      32:
        begin
          for Y := 0 to Source.Height - 1 do
          begin
            OrigRow32 := lsDIB.ScanLine[Y];
            DestRow32 := Source.ScanLine[Y];
            for X := 0 to Source.Width - 1 do
            begin
              if (OrigRow32[X].rgbRed = 0) and (OrigRow32[X].rgbGreen = 0) and (OrigRow32[X].rgbBlue = 0) then
              begin
                DestRow32[X].rgbRed := 0;
                DestRow32[X].rgbGreen := 0;
                DestRow32[X].rgbBlue := 0;
                DestRow32[X].rgbReserved := 0;
              end
              else
              begin
                DestRow32[X].rgbRed := g_Grays[OrigRow32[X].rgbRed + OrigRow32[X].rgbGreen + OrigRow32[X].rgbBlue];
                DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
                DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
                DestRow32[X].rgbReserved := 0;
              end;
            end;
          end;
        end;
    end;
    lsDIB.Free;
  end;
  *)

procedure FileData32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
var
  X, Y:Integer;
  PBits:PByte;
  PDest:PByte;
  SrcP:PByte;
  DesP:PByte;

  Bits:Pointer;
  Pitch:Integer;
begin
  FileData := nil;
  FileSize := 0;
  case Source.BitCount of
    8:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          PBits := Source.ScanLine[Y];
          PDest := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(PDest)^ := ColorTable_8_32Bit[PBits^];
            Inc(PBits);
            Inc(PCardinal(PDest));
          end;
        end;
      end;
    16:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);
        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          SrcP := Source.ScanLine[Y];
          DesP := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      end;
    24:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          PBits := Source.ScanLine[Y];
          PDest := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(PDest)^ := PCardinal(PBits)^ or $FF000000;
            Inc(PBits, 3);
            Inc(PCardinal(PDest));
          end;
        end;
      end;
    32:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          PBits := Source.ScanLine[Y];
          PDest := PByte(Integer(Bits) + Y * Pitch);
          Move(PBits^, PDest^, Pitch);
        end;
      end;
  end;
end;

procedure FileDataBright32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
var
  X, Y:Integer;
  PBits:PByte;
  PDest:PByte;
  SrcP:PByte;
  DesP:PByte;

  Bits:Pointer;
  Pitch:Integer;

  OrigRow24, DestRow24:pRGBArray24;
  OrigRow32, DestRow32:pRGBArray32;
begin
  FileData := nil;
  FileSize := 0;
  case Source.BitCount of
    8:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          PBits := Source.ScanLine[Y];
          PDest := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(PDest)^ := ColorTableBright_8_32Bit[PBits^];
            Inc(PBits);
            Inc(PCardinal(PDest));
          end;
        end;
      end;
    16:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);
        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          SrcP := Source.ScanLine[Y];
          DesP := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(DesP)^ := ColorTableBright_16_32Bit[PWord(SrcP)^];
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      end;
    24:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          OrigRow24 := Source.ScanLine[Y];
          DestRow24 := pRGBArray24(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            if (OrigRow24[X].rgbtRed = 0) and (OrigRow24[X].rgbtGreen = 0) and (OrigRow24[X].rgbtBlue = 0) then begin
              OrigRow24[X].rgbtRed := 0;
              OrigRow24[X].rgbtGreen := 0;
              OrigRow24[X].rgbtBlue := 0;
            end
            else begin
              DestRow24[X].rgbtRed := _MIN(255, Round(OrigRow24[X].rgbtRed * 1.3));
              DestRow24[X].rgbtGreen := _MIN(255, Round(OrigRow24[X].rgbtGreen * 1.3));
              DestRow24[X].rgbtBlue := _MIN(255, Round(OrigRow24[X].rgbtBlue * 1.3));
            end;
          end;
        end;
      end;
    32:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          OrigRow32 := Source.ScanLine[Y];
          DestRow32 := pRGBArray32(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            if (OrigRow32[X].rgbRed = 0) and (OrigRow32[X].rgbGreen = 0) and (OrigRow32[X].rgbBlue = 0) then begin
              DestRow32[X].rgbRed := 0;
              DestRow32[X].rgbGreen := 0;
              DestRow32[X].rgbBlue := 0;
              DestRow32[X].rgbReserved := 0;
            end
            else begin
              DestRow32[X].rgbRed := _MIN(255, Round(OrigRow32[X].rgbRed * 1.3));
              DestRow32[X].rgbGreen := _MIN(255, Round(OrigRow32[X].rgbGreen * 1.3));
              DestRow32[X].rgbBlue := _MIN(255, Round(OrigRow32[X].rgbBlue * 1.3));
              DestRow32[X].rgbReserved := 0;
            end;
          end;
        end;
      end;
  end;
end;

procedure FileDataGray32(Source:TDIB; var FileData:Pointer; var FileSize:Integer);
var
  X, Y:Integer;
  PBits:PByte;
  PDest:PByte;
  SrcP:PByte;
  DesP:PByte;

  Bits:Pointer;
  Pitch:Integer;

  OrigRow24, DestRow24:pRGBArray24;
  OrigRow32, DestRow32:pRGBArray32;
begin
  FileData := nil;
  FileSize := 0;
  case Source.BitCount of
    8:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          PBits := Source.ScanLine[Y];
          PDest := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(PDest)^ := ColorTableGray_8_32Bit[PBits^];
            Inc(PBits);
            Inc(PCardinal(PDest));
          end;
        end;
      end;
    16:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);
        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;
        for Y := 0 to Source.Height - 1 do begin
          SrcP := Source.ScanLine[Y];
          DesP := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            PCardinal(DesP)^ := ColorTableGray_16_32Bit[PWord(SrcP)^];
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      end;
    24:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          OrigRow24 := Source.ScanLine[Y];
          DestRow24 := pRGBArray24(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            if (OrigRow24[X].rgbtRed = 0) and (OrigRow24[X].rgbtGreen = 0) and (OrigRow24[X].rgbtBlue = 0) then begin
              OrigRow24[X].rgbtRed := 0;
              OrigRow24[X].rgbtGreen := 0;
              OrigRow24[X].rgbtBlue := 0;
            end
            else begin
              DestRow24[X].rgbtRed := g_Grays[OrigRow24[X].rgbtRed + OrigRow24[X].rgbtGreen + OrigRow24[X].rgbtBlue];
              DestRow24[X].rgbtGreen := DestRow24[X].rgbtRed;
              DestRow24[X].rgbtBlue := DestRow24[X].rgbtRed;
            end;
          end;
        end;
      end;
    32:begin
        NewBitmapFile(Source.Width, Source.Height, 32, FileData, FileSize);

        Bits := Pointer(Integer(FileData) + SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader));
        Pitch := Source.Width * 4;

        for Y := 0 to Source.Height - 1 do begin
          OrigRow32 := Source.ScanLine[Y];
          DestRow32 := pRGBArray32(Integer(Bits) + Y * Pitch);
          for X := 0 to Source.Width - 1 do begin
            if (OrigRow32[X].rgbRed = 0) and (OrigRow32[X].rgbGreen = 0) and (OrigRow32[X].rgbBlue = 0) then begin
              DestRow32[X].rgbRed := 0;
              DestRow32[X].rgbGreen := 0;
              DestRow32[X].rgbBlue := 0;
              DestRow32[X].rgbReserved := 0;
            end
            else begin
              DestRow32[X].rgbRed := g_Grays[OrigRow32[X].rgbRed + OrigRow32[X].rgbGreen + OrigRow32[X].rgbBlue];
              DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
              DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
              DestRow32[X].rgbReserved := 0;
            end;
          end;
        end;
      end;
  end;
end;

procedure NewBitmapFile(const AWidth, AHeight, ABitCount:Integer; var FileData:Pointer; var FileSize:Integer; RGB565:Boolean);
var
  FileHeader:PBitmapFileHeader;
  InfoHeader:PBitmapInfoHeader;

  Buffer:Pointer;
begin
  FileSize := SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader) + AWidth * AHeight * (ABitCount div 8);
  if ABitCount = 8 then
    FileSize := FileSize + SizeOf(ColorArray);

  FileData := AllocMem(FileSize);

  // 位图文件头
  Buffer := FileData;
  FileHeader := PBitmapFileHeader(Buffer);
  FileHeader.bfType := 19778; // MakeWord(Ord('B'), Ord('M'));
  FileHeader.bfSize := FileSize; // SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader) + AWidth * AHeight * (ABitCount div 8);
  FileHeader.bfOffBits := FileHeader.bfSize - Cardinal(AWidth * AHeight * (ABitCount div 8)); //hzq 20230525 Cardinal()
  FileHeader.bfReserved1 := 0;
  FileHeader.bfReserved2 := 0;

  Buffer := Pointer(Integer(Buffer) + SizeOf(TBitmapFileHeader));

  // 位图信息头
  InfoHeader := PBitmapInfoHeader(Buffer);
  InfoHeader.biSize := SizeOf(TBitmapInfoHeader);
  InfoHeader.biWidth := AWidth;
  InfoHeader.biHeight := -AHeight;
  InfoHeader.biPlanes := 1;
  InfoHeader.biBitCount := ABitCount;
  // if ABitCount = 16 then
   // InfoHeader.biCompression := BI_BITFIELDS
  // else
  InfoHeader.biCompression := BI_RGB; // BI_RGB;
  InfoHeader.biSizeImage := AWidth * AHeight * (ABitCount div 8);
  InfoHeader.biXPelsPerMeter := 0;
  InfoHeader.biYPelsPerMeter := 0;
  InfoHeader.biClrUsed := 0;
  InfoHeader.biClrImportant := 0;

  if ABitCount = 8 then begin
    Buffer := Pointer(Integer(Buffer) + SizeOf(TBitmapInfoHeader));
    Move(ColorArray, Buffer^, SizeOf(ColorArray));
    // end else
      {if ABitCount = 16 then begin
      Buffer := Pointer(Integer(Buffer) + SizeOf(TBitmapInfoHeader));
      if RGB565 then begin
        PInteger(Buffer)^ := $F800;
        Inc(PInteger(Buffer));
        PInteger(Buffer)^ := $07E0;
        Inc(PInteger(Buffer));
        PInteger(Buffer)^ := $001F;
        Inc(PInteger(Buffer));
      end else begin
        PInteger(Buffer)^ := $7C00;
        Inc(PInteger(Buffer));
        PInteger(Buffer)^ := $03E0;
        Inc(PInteger(Buffer));
        PInteger(Buffer)^ := $001F;
        Inc(PInteger(Buffer));
      end;}
  end;
end;

// ------------------------------------------------------------------------------

constructor TGameImages.Create();
begin
  // InitializeCriticalSection(FCriticalSection);
  FLibType := ltUseCache;
  FInitialized := False;
  FBitCount := 8;
  FImageCount := 0;
  FAppr := 0;
  m_ImgArr := nil;

  IndexList := TList.Create;
  GrayIndexList := TList.Create;
  BrightIndexList := TList.Create;

  m_dwMemCheckTick := MyGetTickCount;
  m_dwMemCheckGrayTick := MyGetTickCount;
  m_dwMemCheckBrightTick := MyGetTickCount;

  m_dwFreeMemCheckTick := MyGetTickCount;
  FTransparentColor := $FF000000;
  FD3DFormat := False; // g_boD3DFormat;

  m_nProcIdx := 0;
  m_nProcGrayIdx := 0;
  m_nProcBrightIdx := 0;

  m_boUpdateIndex := True;
  m_boUpdateIndexing := False;
  m_dwUpdateIndexingTick := MyGetTickCount;
  m_boNeedUpdate := True;

  FResetWZLAlpha := False;
end;

destructor TGameImages.Destroy;
begin
  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-13】 }
  Finalize();
  IndexList.Free;
  GrayIndexList.Free;
  BrightIndexList.Free;
  inherited;
end;

procedure TGameImages.OutMessage(const Msg:string; boWriteDate:Boolean);
begin
  DebugTextOut(Msg);
end;

function TGameImages.TryLock:Boolean;
begin
  Result := TryEnterCriticalSection(g_LoadCriticalSection);
end;

procedure TGameImages.Lock;
begin
  EnterCriticalSection(g_LoadCriticalSection);
end; 

function TGameImages.MakeDibByBitCount(nBitCount, nW, nH: Integer): TDIB;
begin
   case nbitCount of
        8: begin
            Result := TDIB.Create;
            Result.ColorTable := g_DefColorTable; //MainPalette;
            Result.UpdatePalette;
            Result.SetSize(nW, nH, 8);
        end;
        16: begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(5, 6, 5);
            Result.SetSize(nW, nH, 16);
        end;
        24: begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
            Result.SetSize(nW, nH, 24);
        end;
        32: begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
            Result.SetSize(nW, nH, 32);
        end;

        else begin
            Result := nil;
        end;
    end;
end;

function TGameImages.MakeDibByPixelFormat(pf: TPixelFormat; nW, nH: Integer): TDIB;
begin
    case pf of
        pf8bit:begin
            Result := TDIB.Create;
            Result.ColorTable := g_DefColorTable; //MainPalette;
            Result.UpdatePalette;
            Result.SetSize(nW, nH, 8);
        end;

        pf15bit, pf16bit:begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(5, 6, 5);
            Result.SetSize(nW, nH, 16);
        end;

        pf24bit:begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
            Result.SetSize(nW, nH, 24);
        end;

        pf32bit:begin
            Result := TDIB.Create;
            Result.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
            Result.SetSize(nW, nH, 32);
        end;

        else begin
            Result := nil;
        end;  
      end;
end;

procedure TGameImages.UnLock;
begin
  LeaveCriticalSection(g_LoadCriticalSection);
end;

procedure TGameImages.UpdateImageDataSize(nIndex, nDataSize: Integer);
begin

end;

procedure TGameImages.Initialize;
begin

end;

procedure TGameImages.Finalize;
begin

end;

procedure TGameImages.SetFileName(Value:string);
begin
  FFileName := Value;
  if Self is TWMImages then begin
    FIndexFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wix';
  end
  else if Self is TWzlImages then begin
    FIndexFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzx';
  end
  else begin
    FIndexFileName := FFileName;
  end;
end;

function TGameImages.GetCachedSurface(Index:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetCachedGray(Index:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetCachedBright(Index:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetCachedBitmap(Index:Integer):TBitmap;
begin
  Result := nil;
end;

function TGameImages.GetCachedImage(Index:Integer; var px, py:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture;
begin
  Result := nil;
end;

function TGameImages.GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap;
begin
  Result := nil;
end;

function TGameImages.GetImageArray(Index:Integer):TTexture;
begin
  if (Index >= 0) and (Index < ImageCount) then
    Result := m_ImgArr[Index].Surface
  else
    Result := nil;
end;

function TGameImages.GetGrayArray(Index:Integer):TTexture;
begin
  if (Index >= 0) and (Index < ImageCount) then
    Result := m_ImgArr[Index].Gray
  else
    Result := nil;
end;

function TGameImages.GetBrightArray(Index:Integer):TTexture;
begin
  if (Index >= 0) and (Index < ImageCount) then
    Result := m_ImgArr[Index].Bright
  else
    Result := nil;
end;

function TGameImages.GetNeedUpdate(Index:Integer):Boolean;
begin
  if m_boNeedUpdate and (m_ImgArr <> nil) and (Index >= 0) and (Index < ImageCount) then
    Result := not m_ImgArr[Index].boUpdateStop
  else
    Result := False;
end;

procedure TGameImages.ClearCacheFilter(Filters:TList);
begin

end;

procedure TGameImages.ClearCache;
var
  I:Integer;
begin
  Lock;
  try
    m_nProcIdx := 0;
    m_nProcGrayIdx := 0;
    m_nProcBrightIdx := 0;
    IndexList.Clear;
    GrayIndexList.Clear;
    BrightIndexList.Clear;
    if m_ImgArr <> nil then begin
      for I := 0 to ImageCount - 1 do begin
        if m_ImgArr[I].Surface <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Surface);
          except
            OutMessage('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Gray <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Gray);
          except
            OutMessage('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Bright <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Bright);
          except
            OutMessage('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Bitmap <> nil then begin
          FreeAndNil(m_ImgArr[I].Bitmap);
        end;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TGameImages.FreeOldMemorys(Index:Integer);
var
  nIdx, nIndex:Integer;
  dwTimeTick:longword;
  boCheckTimeLimit:Boolean;
begin
  if Initialized and TryLock then begin
    dwTimeTick := MyGetTickCount;
    if m_ImgArr <> nil then begin
      nIdx := m_nProcIdx;
      boCheckTimeLimit := False;
      while True do begin
        if IndexList.Count <= nIdx then Break;
        nIndex := Integer(IndexList.Items[nIdx]);

        if (Index <> nIndex) and (nIndex >= 0) and (nIndex < FImageCount) then begin
          if (m_ImgArr[nIndex].Surface = nil) then begin
            IndexList.Delete(nIdx);
            Continue;
          end;

          // 20秒前的缓存数据清理掉 chongchong 2014-06-26   原来是2分钟
          if (m_ImgArr[nIndex].Surface <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestTime > 5 * 1000) then begin
            IndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Surface);
            except
              OutMessage('[Exception] Texture.Free');
            end;
            Continue;
          end;
        end;

        Inc(nIdx);
        if (MyGetTickCount - dwTimeTick) > 10 then begin
          boCheckTimeLimit := True;
          m_nProcIdx := nIdx;
          Break;
        end;
      end;
      if not boCheckTimeLimit then m_nProcIdx := 0;
    end;

    UnLock;
  end;
end;

procedure TGameImages.FreeOldBrightMemorys(Index:Integer);
var
  nIndex:Integer;
  dwTimeTick:longword;
  nIdx:Integer;
  boCheckTimeLimit:Boolean;
begin
  if Initialized and TryLock then begin

    dwTimeTick := MyGetTickCount;
    if m_ImgArr <> nil then begin
      nIdx := m_nProcBrightIdx;
      boCheckTimeLimit := False;
      while True do begin
        if BrightIndexList.Count <= nIdx then Break;
        nIndex := Integer(BrightIndexList.Items[nIdx]);
        if (Index <> nIndex) and (nIndex >= 0) and (nIndex < FImageCount) then begin
          if (m_ImgArr[nIndex].Bright = nil) then begin
            BrightIndexList.Delete(nIdx);
            Continue;
          end;

          if (m_ImgArr[nIndex].Bright <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestBrightTime > 5 * 1000) then begin
            BrightIndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Bright);
            except
              OutMessage('[Exception] Texture.Free');
            end;
          end;
        end;

        Inc(nIdx);
        if (MyGetTickCount - dwTimeTick) > 6 then begin
          boCheckTimeLimit := True;
          m_nProcBrightIdx := nIdx;
          Break;
        end;

      end;
      if not boCheckTimeLimit then m_nProcBrightIdx := 0;
    end;
    UnLock;
  end;
end;

procedure TGameImages.FreeOldGrayMemorys(Index:Integer);
var
  nIndex:Integer;
  dwTimeTick:longword;
  nIdx:Integer;
  boCheckTimeLimit:Boolean;
begin
  if Initialized and TryLock then begin

    dwTimeTick := MyGetTickCount;
    if m_ImgArr <> nil then begin
      nIdx := m_nProcGrayIdx;
      boCheckTimeLimit := False;
      while True do begin
        if GrayIndexList.Count <= nIdx then Break;
        nIndex := Integer(GrayIndexList.Items[nIdx]);
        if (Index <> nIndex) and (nIndex >= 0) and (nIndex < FImageCount) then begin
          if (m_ImgArr[nIndex].Gray = nil) then begin
            GrayIndexList.Delete(nIdx);
            Continue;
          end;
          if (m_ImgArr[nIndex].Gray <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestGrayTime > 5 * 1000) then begin
            GrayIndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Gray);
            except
              OutMessage('[Exception] Texture.Free');
            end;

            Continue;
          end;
        end;
        Inc(nIdx);
        if (MyGetTickCount - dwTimeTick) > 6 then begin
          boCheckTimeLimit := True;
          m_nProcGrayIdx := nIdx;
          Break;
        end;
      end;
      if not boCheckTimeLimit then m_nProcGrayIdx := 0;
    end;
    UnLock;
  end;
end;

procedure TGameImages.FreeOldMemorys(Clear1Min:Boolean = False);
var
  nIndex:Integer;
  dwTimeTick:longword;
  nIdx:Integer;
  boCheckTimeLimit:Boolean;
  ClearInterval:LongWord;
begin
  if Initialized and TryLock then begin
    if Clear1Min then
      ClearInterval := 10 * 1000 // 修改释放间隔  chongchong 2016-02-23  20 * 1000
    else
      ClearInterval := 15 * 1000; // 修改释放间隔  chongchong 2016-02-23  30 * 1000

    if m_ImgArr <> nil then begin
      // if (MyGetTickCount - m_dwFreeMemCheckTick > 1 * 1000) then begin
       // m_dwFreeMemCheckTick := MyGetTickCount;

      dwTimeTick := MyGetTickCount;
      nIdx := m_nProcIdx;
      boCheckTimeLimit := False;
      while True do begin
        if IndexList.Count <= nIdx then Break;
        nIndex := Integer(IndexList.Items[nIdx]);
        if (nIndex >= 0) and (nIndex < ImageCount) then begin
          if (m_ImgArr[nIndex].Surface = nil) then begin
            IndexList.Delete(nIdx);
            Continue;
          end;

          if (m_ImgArr[nIndex].Surface <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestTime > ClearInterval) then begin
            IndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Surface);
            except
              OutMessage('[Exception] Texture.Free');
            end;
            Continue;
          end;
        end;
        Inc(nIdx);
        if (MyGetTickCount - dwTimeTick) > 10 then begin
          boCheckTimeLimit := True;
          m_nProcIdx := nIdx;
          Break;
        end;
      end;
      if not boCheckTimeLimit then m_nProcIdx := 0;

      dwTimeTick := MyGetTickCount;
      nIdx := m_nProcGrayIdx;
      boCheckTimeLimit := False;
      while True do begin
        if GrayIndexList.Count <= nIdx then Break;
        nIndex := Integer(GrayIndexList.Items[nIdx]);
        if (nIndex >= 0) and (nIndex < ImageCount) then begin
          if (m_ImgArr[nIndex].Gray = nil) then begin
            GrayIndexList.Delete(nIdx);
            Continue;
          end;
          if (m_ImgArr[nIndex].Gray <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestGrayTime > 3 * 1000) then begin
            GrayIndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Gray);
            except
              OutMessage('[Exception] Texture.Free');
            end;
            Continue;
          end;
        end;
        Inc(nIdx);
        if (MyGetTickCount - dwTimeTick) > 10 then begin
          boCheckTimeLimit := True;
          m_nProcGrayIdx := nIdx;
          Break;
        end;
      end;
      if not boCheckTimeLimit then m_nProcGrayIdx := 0;

      dwTimeTick := MyGetTickCount;
      nIdx := m_nProcBrightIdx;
      boCheckTimeLimit := False;
      while True do begin
        if BrightIndexList.Count <= nIdx then Break;
        nIndex := Integer(BrightIndexList.Items[nIdx]);
        if (nIndex >= 0) and (nIndex < ImageCount) then begin
          if (m_ImgArr[nIndex].Bright = nil) then begin
            BrightIndexList.Delete(nIdx);
            Continue;
          end;

          if (m_ImgArr[nIndex].Bright <> nil) and (MyGetTickCount - m_ImgArr[nIndex].dwLatestBrightTime > 3 * 1000) then begin
            BrightIndexList.Delete(nIdx);
            try
              FreeAndNil(m_ImgArr[nIndex].Bright);
            except
              OutMessage('[Exception] Texture.Free');
            end;

            Continue;
          end;
        end;

        if (MyGetTickCount - dwTimeTick) > 10 then begin
          boCheckTimeLimit := True;
          m_nProcBrightIdx := nIdx;
          Break;
        end;
        Inc(nIdx);
      end;
      if not boCheckTimeLimit then m_nProcBrightIdx := 0;

    end;
    UnLock;
  end;
end;

procedure TGameImages.SaveToFile(Index:Integer; const FileName:string);
var
  D:TTexture;
begin
  D := Images[Index];
  if D <> nil then try
    // D.SaveToFile(FileName);
  except

  end;
end;

procedure TGameImages.DrawZoom(paper:TCanvas; X, Y, Index:Integer; Zoom:Real);
var
  rc:TRect;
  bmp:TBitmap;
begin
  bmp := Bitmaps[Index];
  if bmp <> nil then begin
    rc.Left := X;
    rc.Top := Y;
    rc.Right := X + Round(bmp.Width * Zoom);
    rc.Bottom := Y + Round(bmp.Height * Zoom);
    if (rc.Right > rc.Left) and (rc.Bottom > rc.Top) then begin
      paper.StretchDraw(rc, bmp);
    end;
  end;
end;

procedure TGameImages.DrawZoomEx(paper:TCanvas; X, Y, Index:Integer; Zoom:Real; leftzero:Boolean);
var
  rc:TRect;
  bmp, bmp2:TBitmap;
begin
  bmp := Bitmaps[Index];
  if bmp <> nil then begin
    bmp2 := TBitmap.Create;
    bmp2.Width := Round(bmp.Width * Zoom);
    bmp2.Height := Round(bmp.Height * Zoom);
    bmp2.PixelFormat := pf32bit;
    rc.Left := X;
    rc.Top := Y;
    rc.Right := X + Round(bmp.Width * Zoom);
    rc.Bottom := Y + Round(bmp.Height * Zoom);

    if (rc.Right > rc.Left) and (rc.Bottom > rc.Top) then begin
      bmp2.Canvas.StretchDraw(Rect(0, 0, bmp2.Width, bmp2.Height), bmp);
      if leftzero then begin
        SpliteBitmap(paper.handle, X, Y, bmp2, $0)
      end
      else begin
        SpliteBitmap(paper.handle, X, Y - bmp2.Height, bmp2, $0);
      end;
    end;
    bmp2.Free;
  end;
end;

procedure TGameImages.FreeOldMemorys_Ex;
var
  I, nIndex:Integer;
  Img:pTDxImage;
  CurrentTick:DWORD;
begin
  if MyGetTickCount - m_dwMemChecktTick < 10000 then Exit;
  if m_ImgArr = nil then Exit;

  {
  Lock;
  try
  }
  m_dwMemChecktTick := MyGetTickCount;
  CurrentTick := MyGetTickCount;
  for I := IndexList.Count - 1 downto 0 do begin
    nIndex := Integer(IndexList.Items[I]);
    if (nIndex >= 0) and (nIndex < ImageCount) then begin
      Img := @m_ImgArr[nIndex];

      if (Img.Surface = nil) then begin
        IndexList.Delete(I);
      end
      else if (CurrentTick - Img.dwLatestTime > 3 * 1000) then {// 修改释放间隔  chongchong 2016-02-23  30 * 1000} begin
        IndexList.Delete(I);
        try
          FreeAndNil(Img.Surface);
        except
          OutMessage('[Exception] Texture.Free');
        end;
      end;
    end;
  end;

  for I := BrightIndexList.Count - 1 downto 0 do begin
    nIndex := Integer(BrightIndexList.Items[I]);
    if (nIndex >= 0) and (nIndex < ImageCount) then begin
      Img := @m_ImgArr[nIndex];

      if (Img.Bright = nil) then begin
        BrightIndexList.Delete(I);
      end
      else if (CurrentTick - Img.dwLatestBrightTime > 3 * 1000) then {// 修改释放间隔  chongchong 2016-02-23  30 * 1000} begin
        BrightIndexList.Delete(I);
        try
          FreeAndNil(Img.Bright);
        except
          OutMessage('[Exception] Texture.Free');
        end;
      end;
    end;
  end;

  for I := GrayIndexList.Count - 1 downto 0 do begin
    nIndex := Integer(GrayIndexList.Items[I]);
    if (nIndex >= 0) and (nIndex < ImageCount) then begin
      Img := @m_ImgArr[nIndex];

      if (Img.Gray = nil) then begin
        GrayIndexList.Delete(I);
      end
      else if (CurrentTick - Img.dwLatestGrayTime > 3 * 1000) then {// 修改释放间隔  chongchong 2016-02-23  30 * 1000} begin
        GrayIndexList.Delete(I);
        try
          FreeAndNil(Img.Gray);
        except
          OutMessage('[Exception] Texture.Free');
        end;
      end;
    end;
  end;
  {
  finally
    UnLock;
  end;
  }
end;

// ------------------------------------------------------------------------------
type
  PFormatBitInfo = ^TFormatBitInfo;
  TFormatBitInfo = record
    rAt, rNo:Integer;
    gAt, gNo:Integer;
    bAt, bNo:Integer;
    aAt, aNo:Integer;
  end;
const
  FormatInfo:array[0..49] of TFormatBitInfo = (
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_Unknown
    (rAt:16; rNo:8; gAt:8; gNo:8; bAt:0; bNo:8; aAt: - 1; aNo:0), // apf_R8G8B8
    (rAt:16; rNo:8; gAt:8; gNo:8; bAt:0; bNo:8; aAt:24; aNo:8), // apf_A8R8G8B8
    (rAt:16; rNo:8; gAt:8; gNo:8; bAt:0; bNo:8; aAt: - 1; aNo:0), // apf_X8R8G8B8
    (rAt:11; rNo:5; gAt:5; gNo:6; bAt:0; bNo:5; aAt: - 1; aNo:0), // apf_R5G6B5
    (rAt:10; rNo:5; gAt:5; gNo:5; bAt:0; bNo:5; aAt: - 1; aNo:0), // apf_X1R5G5B5
    (rAt:10; rNo:5; gAt:5; gNo:5; bAt:0; bNo:5; aAt:15; aNo:1), // apf_A1R5G5B5
    (rAt:8; rNo:4; gAt:4; gNo:4; bAt:0; bNo:4; aAt:12; aNo:4), // apf_A4R4G4B4
    (rAt:5; rNo:3; gAt:2; gNo:3; bAt:0; bNo:2; aAt: - 1; aNo:0), // apf_R3G3B2
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:0; aNo:8), // apf_A8
    (rAt:5; rNo:3; gAt:2; gNo:3; bAt:0; bNo:2; aAt:8; aNo:8), // apf_A8R3G3B2
    (rAt:8; rNo:4; gAt:4; gNo:4; bAt:0; bNo:4; aAt: - 1; aNo:0), // apf_X4R4G4B4
    (rAt:0; rNo:10; gAt:10; gNo:10; bAt:20; bNo:10; aAt:30; aNo:2), // apf_A2B10G10R10
    (rAt:0; rNo:16; gAt:16; gNo:16; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_G16R16
    (rAt:20; rNo:10; gAt:10; gNo:10; bAt:0; bNo:10; aAt:30; aNo:2), // apf_A2R10G10B10
    (rAt:0; rNo:16; gAt:16; gNo:16; bAt:32; bNo:16; aAt:48; aNo:16), // apf_A16B16G16R16
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_L8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:8; aNo:8), // apf_A8L8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:4; aNo:4), // apf_A4L4
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_V8U8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_L6V5U5
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_X8L8V8U8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_Q8W8V8U8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_V16U16
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:30; aNo:2), // apf_A2W10V10U10
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_UYVY
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_R8G8_B8G8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_YUY2
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_G8R8_G8B8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_DXT1
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_DXT2
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_DXT3
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_DXT4
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_DXT5
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_L16
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_Q16W16V16U16
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_R16F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_G16R16F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_A16B16G16R16F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_R32F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_G32R32F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_A32B32G32R32F
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_CxV8U8
    (rAt:0; rNo:8; gAt:8; gNo:8; bAt:16; bNo:8; aAt:24; aNo:8), // apf_A8B8G8R8
    (rAt:0; rNo:8; gAt:8; gNo:8; bAt:16; bNo:8; aAt: - 1; aNo:0), // apf_X8B8G8R8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:24; aNo:8), // apf_A8X8V8U8
    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt: - 1; aNo:0), // apf_L8X8V8U8

    (rAt: - 1; rNo:0; gAt: - 1; gNo:0; bAt: - 1; bNo:0; aAt:2; aNo:6), // apf_A6L2
    (rAt:4; rNo:2; gAt:2; gNo:2; bAt:0; bNo:2; aAt:6; aNo:2), // apf_A2R2G2B2
    (rAt:18; rNo:9; gAt:9; gNo:9; bAt:0; bNo:9; aAt:27; aNo:5) // apf_A5R9G9B9
    );

procedure InitGrays();
var
  I, X:Integer;
begin
  X := 0;
  for I := 0 to 255 do begin
    g_Grays[X] := I;
    Inc(X);
    g_Grays[X] := I;
    Inc(X);
    g_Grays[X] := I;
    Inc(X);
  end;
end;

procedure BuildColorLevels(ctable:TRGBQuads);

  function GetColor(Source:Cardinal):Word;
  var
    Value:Cardinal;
    Info:PFormatBitInfo;
    Mask:Cardinal;
  begin
    Info := @FormatInfo[6];

    // -> Blue Component
    if (Info.bNo > 0) then begin
      Mask := (1 shl Info.bNo) - 1;
      Value := (((Source and $FF) * Mask) div 255) shl Info.bAt;
    end;

    // -> Green Component
    if (Info.gNo > 0) then begin
      Mask := (1 shl Info.gNo) - 1;
      Value := Value or
        ((((Source shr 8) and $FF) * Mask) div 255) shl Info.gAt;
    end;

    // -> Red Component
    if (Info.rNo > 0) then begin
      Mask := (1 shl Info.rNo) - 1;
      Value := Value or
        ((((Source shr 16) and $FF) * Mask) div 255) shl Info.rAt;
    end;

    // -> Alpha Component
    if (Info.aNo > 0) then begin
      Mask := (1 shl Info.aNo) - 1;
      Value := Value or
        ((((Source shr 24) and $FF) * Mask) div 255) shl Info.aAt;
    end;

    Move(Value, Result, 2);
  end;
var
  n, i:integer;
  pal1, pal2:TRGBQuad;
begin
  ColorTable_16_32Bit[0] := 0;
  ColorTableGray_16_32Bit[0] := 0;
  ColorTableBright_16_32Bit[0] := 0;
  ColorTableGray_16[0] := 0;
  ColorTableBright_16[0] := 0;

  for I := 1 to High(Word) do begin
    pal1.rgbRed := I and $F800 shr 8;
    pal1.rgbGreen := I and $07E0 shr 3;
    pal1.rgbBlue := I and $001F shl 3;
    pal1.rgbReserved := 255;

    ColorTable_16_32Bit[I] := Cardinal(pal1) or $FF000000;

    n := Round((pal1.rgbRed + pal1.rgbGreen + pal1.rgbBlue) / 3);
    pal2.rgbRed := n;
    pal2.rgbGreen := n;
    pal2.rgbBlue := n;

    ColorTableGray_16_32Bit[I] := Cardinal(pal2) or $FF000000;
    ColorTableGray_16[I] := GetColor(Cardinal(pal2) or $FF000000); // (pal2.rgbRed shl 8 and $F800) or (pal2.rgbGreen shl 3 and $07E0) or (pal2.rgbBlue shr 3 and $001F);

    pal2.rgbRed := _MIN(Round(pal1.rgbRed * 1.3), 255);
    pal2.rgbGreen := _MIN(Round(pal1.rgbGreen * 1.3), 255);
    pal2.rgbBlue := _MIN(Round(pal1.rgbBlue * 1.3), 255);

    ColorTableBright_16_32Bit[I] := Cardinal(pal2) or $FF000000;

    ColorTableBright_16[I] := (pal2.rgbRed shl 8 and $F800) or (pal2.rgbGreen shl 3 and $07E0) or (pal2.rgbBlue shr 3 and $001F);
  end;

  for I := Low(ColorTable_8_32Bit) to High(ColorTable_8_32Bit) do begin
    pal1 := g_DefColorTable[I];
    if Integer(pal1) <> 0 then begin
      ColorTable_8_32Bit[I] := Cardinal(pal1) or $FF000000;

      ColorTable_8_16Bit[I] := GetColor(Cardinal(pal1) or $FF000000); // (pal1.rgbRed shl 8 and $F800) or (pal1.rgbGreen shl 3 and $07E0) or (pal1.rgbBlue shr 3 and $001F);

      n := Round((pal1.rgbRed + pal1.rgbGreen + pal1.rgbBlue) / 3);
      pal2.rgbRed := n;
      pal2.rgbGreen := n;
      pal2.rgbBlue := n;
      ColorTableGray_8_32Bit[I] := Cardinal(pal2) or $FF000000;

      ColorTableGray_8_16Bit[I] := (pal2.rgbRed shl 8 and $F800) or (pal2.rgbGreen shl 3 and $07E0) or (pal2.rgbBlue shr 3 and $001F);

      pal2.rgbRed := _MIN(Round(pal1.rgbRed * 1.3), 255);
      pal2.rgbGreen := _MIN(Round(pal1.rgbGreen * 1.3), 255);
      pal2.rgbBlue := _MIN(Round(pal1.rgbBlue * 1.3), 255);
      ColorTableBright_8_32Bit[I] := Cardinal(pal2) or $FF000000;

      ColorTableBright_8_16Bit[I] := (pal2.rgbRed shl 8 and $F800) or (pal2.rgbGreen shl 3 and $07E0) or (pal2.rgbBlue shr 3 and $001F);
    end
    else begin
      ColorTable_8_16Bit[I] := 0;
      ColorTableGray_8_16Bit[I] := 0;
      ColorTableBright_8_16Bit[I] := 0;

      ColorTable_8_32Bit[I] := 0;
      ColorTableGray_8_32Bit[I] := 0;
      ColorTableBright_8_32Bit[I] := 0;
    end;
  end;
end;

initialization
  InitializeCriticalSection(g_LoadCriticalSection);
  Move(ColorArray, g_DefColorTable, SizeOf(g_DefColorTable));
  InitGrays();
  BuildColorLevels(TRGBQuads(g_DefColorTable));
finalization
  DeleteCriticalSection(g_LoadCriticalSection);
end.
