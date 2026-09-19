unit Pak;

interface

uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  DIB,
  HGE,
  DxCanvas,
  DxControls,
  GameImages,
  MapFiles,
  HUtil32,
  DesUtils,
  GlobalString,
  AESUtils;

type
  TPakPassword = packed record
    KeyData:array[0..32-1] of LongWord;
    Chain:array[0..20 - 1] of Byte;
    KeyDataLz:array[0..32-1] of LongWord;
    ChainLz:array[0..20 - 1] of Byte;
  end;
  pTPakPassword = ^TPakPassword;

  TPakIndexHeader = packed record
    OffSet:Integer;
    Length:Integer;
  end;
  pTPakIndexHeader = ^TPakIndexHeader;

  TPakFileIndexArray = array of TPakIndexHeader;

  TPakImageInfo = packed record
    btEncr0:Byte; // 0X00
    btEncr1:Byte; // 0X01
    bt2:Byte; // 0X02
    bt3:Byte; // 0X03
    wW:Smallint; // 0X04
    wH:Smallint; // 0X06
    wPx:Smallint; // 0X08
    wPy:Smallint; // 0X0A
  end;
  pTPakImageInfo = ^TPakImageInfo;

  TNewPakImageInfo = packed record
    PixelFormat:TPixelFormat;
    bt2:Byte;
    bt3:Byte;
    boAlpha:Boolean; // 是否有通道数据
    nWidth:SmallInt;
    nHeight:SmallInt;
    px:SmallInt;
    py:SmallInt;
    Length:Integer;
  end;
  pTNewPakImageInfo = ^TNewPakImageInfo;

  TFileHeaderInfo = packed record
    FileType:string[9];
  end;

  TLzFileHeaderInfo = packed record
     FileType : array[0..5-1] of AnsiChar;
  end;

  TPakFileHeader = packed record // 新定义的Pak文件头
    bt1:Byte;
    Title:string[40];
    Size:DWORD;
    ImageCount:DWORD;
    // IndexSize: LongWord;
    bfType:Byte;
    bfReserved1:Byte;
    bfReserved2:Byte;
    bfReserved3:Byte;
    IndexOffSet:LongWord;
    BitCount:Word;
    CreateDate:TDateTime;
    CheckCode:string[12];
    bfReserveds:array[0..2] of Byte;
    Reserve:array[0..5] of Integer;
    KeyData:array[0..31] of DWord;
    Chain:array[0..20 - 1] of Byte;
  end;
  pTPakFileHeader = ^TPakFileHeader;

  // 微端定义
  pTPakKey = ^TPakKey;
  TPakKey = packed record
    ImageCount:Integer;
    PakType:Word; //使用 PakTypeWord的高位记录LzPakV0的BitCount, 其他类型保持不变
    KeyData:array[0..31] of LongWord;
    Chain:array[0..20 - 1] of Byte;
    Reserve:array[0..5] of Integer;
  end;

  TLzPakIndexVer0 = packed record
     nImageOffSet: Integer;
     nDataSize: Integer;
  end;
  PLzPakIndexVer0 = ^TLzPakIndexVer0;

  TDxTextureStyle = (dtsNormal, dtsGray, dtsBright);

  //HZQ 20230711 Gee(string[9]) 和Gom(string [10]) 记录了类型， 龙族的PAK需要解密后才能知道类型
  TPakFileType = (pftPak1, pftPak2, pftPak3, pftLzPakV0, pftLzPakV1, pftLzPakV0orV1);

  TPakImages = class(TGameImages)
  private
    FPakFileType:TPakFileType;
    FileHeader:TPakFileHeader;

    // FPassWord: string;
    FPasswordOK:Boolean;

    FKeyData:array[0..32-1] of DWord;
    FChain:array[0..20 - 1] of Byte;
    
    FKeyDataLz:array[0..32-1] of DWord;
    FChainLz:array[0..20 - 1] of Byte;

    FPak3Password:array[0..63] of LongWord;

    m_ImageSizeList:TList;

    procedure ReadImageHeader(var Buffer; Count:Longint);
    procedure ReadImageHeader_Pak2(var Buffer; Count:Longint);
    procedure ReadImageHeader_Pak3(Index:Integer; var Buffer; Count:Longint);

    procedure DecryptIndexList(InData:Pointer; InSize:Integer; out OutData:Pointer);
    procedure EncryptIndexList(InData:Pointer; InSize:Integer; out OutData:Pointer);

    procedure DecryptIndexList_Pak2(InData:Pointer; InSize:Integer; out OutData:Pointer);
    procedure EncryptIndexList_Pak2(InData:Pointer; InSize:Integer; out OutData:Pointer);

    function EncryptHeader(Header:TPakFileHeader):string;

    function EncryptHeader_GameOfMir(Header:TPakFileHeader):string;
    function DecryptHeader_GameOfMir(const Str:string):TPakFileHeader;

    function DecryptHeader_Pak2(const Str:string):TPakFileHeader;
    function EncryptHeader_Pak2(Header:TPakFileHeader):string;

    function EncryptHeader_Pak3(Header:TPakFileHeader):string;
    function DecryptHeader_Pak3(const Str:string):TPakFileHeader;

    function DecryptHeader_LzPak(const str:string):TPakFileHeader;
    function EncryptHeader_LzPak(Header:TPakFileHeader):string;
    function IsValidLzCheckCode(const sCheckCode:string):Boolean;

    function EncryptS(S:string):string;
    function DecryptS(S:string):string;

    procedure LoadIndex;

    {
    procedure LoadDxImage(IndexHeader: pTPakIndexHeader; DXImage: pTDXImage); overload;
    procedure LoadDxBitmap(IndexHeader: pTPakIndexHeader; DXImage: pTDXImage); overload;
    procedure LoadDxGrayImage(IndexHeader: pTPakIndexHeader; DXImage: pTDXImage); overload;
    procedure LoadDxBrightImage(IndexHeader: pTPakIndexHeader; DXImage: pTDXImage); overload;
    }

    //procedure ReadLzImageHeader(var Buffer; Count:Longint);

    //function MakeDibByPixelFormat(pf:TPixelFormat; nW, nH:Integer):TDIB;
    //function MakeDibByBitCount(nBitCount:Integer; nW, nH:Integer):TDIB; 

    function GetBitCountByPixelFormat(pf:TPixelFormat):Integer;
    function GetNewPakImageDataSize(const ImageHead:TNewPakImageInfo):Integer;
    function GetAlphaDibFromNewFormat(nImgWidth, nImgHeight:Integer; pSrcData:Pointer; nSrcSize:Integer):TDIB;
    function GetCachedLzImageSize(Index:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
    function IsNewSDFormat(const ImageHead:TNewPakImageInfo; nDecompressedSize:Integer):Boolean;
    function IsValidLzImageInfoV0(const ImageInfoV0:TPakImageInfo; nIndex:Integer; nDataSize:Integer; nBitCount:Integer):Boolean;
    function IsValidLzImageInfoV1(const ImageInfoV1:TNewPakImageInfo; nIndex:Integer):Boolean;
    function LoadLzImageDataV0(const ImageInfoV0:TPakImageInfo; msData:TMemoryStream; var Source, AlphaSource:TDIB):Boolean;
    function LoadLzImageDataV1(const ImageInfoV1:TNewPakImageInfo; msData:TMemoryStream; var Source, AlphaSource:TDiB):Boolean;

    procedure LoadDxImageLzPak(nIndex, nImgOffset, nImgDataSize:Integer; dtsStyle:TDxTextureStyle; DXImage:pTDxImage);
    procedure LoadDxImage(Position:Integer; DXImage:pTDXImage; Index:Integer); overload;
    procedure LoadDxBitmap(Position:Integer; DXImage:pTDXImage; Index:Integer); overload;
    procedure LoadDxGrayImage(Position:Integer; DXImage:pTDXImage; Index:Integer); overload;
    procedure LoadDxBrightImage(Position:Integer; DXImage:pTDXImage; Index:Integer); overload;
  protected
    function DecryptHeader(const Str:string):TPakFileHeader; //HZQ 20230525 From Private
    function GetCachedSurface(Index:Integer):TTexture; override;
    function GetCachedGray(Index:Integer):TTexture; override;
    function GetCachedBright(Index:Integer):TTexture; override;
  public
    m_FileStream:TFileStream; // TMapStream; //   

    FCSFileStream:TRTLCriticalSection;

    constructor Create(APassWord:TPakPassword);
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Initialize_UpdateNewFile;
    procedure Finalize; override;
    procedure WriteHeader();
    procedure WriteIndexList(FileStream:TFileStream; Index:Integer); overload;
    procedure UpdateImageDataSize(nIndex, nDataSize: Integer); override;

    procedure InitPak3Password;
    procedure UpdateIndex(PakKey:pTPakKey; IsCompareHeader:Boolean);

    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture; override;
    function GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap; override;

    function GetCachedImageSize(Index:Integer; var ASize:TSize; var APoint:TPoint):Boolean; override;

    procedure LockFileStream;
    procedure UnLockFileStream;
  end;

implementation

uses
  //CompressUnit,
  EncryptUnit_LF,
  UnitDes,
  Math,
  SDK,
  RLEUnit,
  ZLibEx{$IF CLIENTEXE = 1},
  Grobal2,
  UpdateEngine,
  MShare{$IFEND};

const
  PAK3_ENCODE = 1; // PAK3加密
  LZ_PAK_FILE_HEADER_SIZE = 262;

  {$IF CLIENTEXE <> 1}
  g_UpdateRetryTime = 3;
  g_boAutoUpdate = False;
  {$IFEND}

var
  //MainPalette:TRGBQuads; // array[0..255] of TRGBQuad;
  BytesPerPixels:array[TPixelFormat] of Byte = (1, 1, 1, 1, 2, 2, 3, 4, 4);

constructor TPakImages.Create(APassWord:TPakPassword);
begin
  inherited Create;
  Move(APassWord.Chain, FChain, SizeOf(FChain));
  Move(APassWord.KeyData, FKeyData, SizeOf(FKeyData));
  Move(APassWord.ChainLz, FChainLz, SizeOf(FChainLz));
  Move(APassWord.KeyDataLz, FKeyDataLz, SizeOf(FKeyDataLz));
  m_FileStream := nil;
  m_IndexList := TList.Create;
  FPasswordOK := False;
  FPakFileType := pftPak1;
  m_dwMemChecktTick := MyGetTickCount;
  InitializeCriticalSection(FCSFileStream);
end;

destructor TPakImages.Destroy;
begin
  inherited;
  m_IndexList.Free;
  if Assigned(m_ImageSizeList) then m_ImageSizeList.Free;

  DeleteCriticalSection(FCSFileStream);
end;

procedure TPakImages.InitPak3Password;
var
  I:Integer;
  len:Integer;
  a, b, c:DWORD;
  k:Integer;
begin
  {.$I VMProtectBegin.inc}
    {
    FPak3Password[00] := PakFileHeader.KeyData[31];
    FPak3Password[02] := PakFileHeader.KeyData[30];
    ..................
    }
  for I := Low(FKeyData) to High(FKeyData) do begin
    FPak3Password[I * 2] := FKeyData[High(FKeyData) - I];
  end;

  Move(FChain[00], FPak3Password[1], SizeOf(FPak3Password[1]));
  Move(FChain[04], FPak3Password[7], SizeOf(FPak3Password[1]));
  Move(FChain[08], FPak3Password[11], SizeOf(FPak3Password[1]));
  Move(FChain[12], FPak3Password[13], SizeOf(FPak3Password[1]));
  Move(FChain[16], FPak3Password[15], SizeOf(FPak3Password[1]));

  //strGKey := GetGHash3(FKeyData);
  len := Length(FKeyData);

  // k为string类型数据(url) 的下标, 起始值从1开始
  k := 0;

  a := $BCA24215;
  b := $BD194331;
  c := $B99EAC12;

  while len >= 3 do begin
    a := a + FKeyData[k];
    b := b + FKeyData[k + 1];
    c := c + FKeyData[k + 2];

    a := a - b;
    a := a - c;
    a := a xor (c shr 8);
    b := b - c;
    b := b - a;
    b := b xor (a shl 9);
    c := c - a;
    c := c - b;
    c := c xor (b shr 13);
    a := a - b;
    a := a - c;
    a := a xor (c shr 9);
    b := b - c;
    b := b - a;
    b := b xor (a shl 6);
    c := c - a;
    c := c - b;
    c := c xor (b shr 4);
    a := a - b;
    a := a - c;
    a := a xor (c shr 8);
    b := b - c;
    b := b - a;
    b := b xor (a shl 3);
    c := c - a;
    c := c - b;
    c := c xor (b shr 15);

    Inc(k, 3);
    Dec(len, 3);
  end;

  c := c + DWORD(Length(FKeyData));

  if len >= 1 then a := a + FKeyData[k + 0];
  if len >= 2 then a := a + FKeyData[k + 1];
  a := a - b;
  a := a - c;
  a := a xor (c shr 4);
  b := b - c;
  b := b - a;
  b := b xor (a shl 9);
  c := c - a;
  c := c - b;
  c := c xor (b shr 19);
  a := a - b;
  a := a - c;
  a := a xor (c shr 11);
  b := b - c;
  b := b - a;
  b := b xor (a shl 14);
  c := c - a;
  c := c - b;
  c := c xor (b shr 5);
  a := a - b;
  a := a - c;
  a := a xor (c shr 9);
  b := b - c;
  b := b - a;
  b := b xor (a shl 12);
  c := c - a;
  c := c - b;
  c := c xor (b shr 3);

  FPak3Password[3] := a;
  FPak3Password[5] := b;
  FPak3Password[9] := c;

  // unsigned int JSHash(char* str, unsigned int len)
  c := 1315423911;
  for I := 0 to 16 do begin
    c := c xor ((c shl 5) + FPak3Password[I] + (c shr 2));
  end;
  FPak3Password[17] := c;

  // unsigned int DJBHash(char* str, unsigned int len)
  c := 5381;
  for I := 0 to 17 do begin
    c := ((c shl 5) + c) + FPak3Password[I];
  end;
  FPak3Password[19] := c;

  for I := 10 to 31 do begin
    len := I * 2;

    // k为string类型数据(url) 的下标, 起始值从1开始
    k := 0;

    a := $16B997C8;
    b := $48744D94;
    c := $BA06742F;

    while len >= 3 do begin
      a := a + FPak3Password[k];
      b := b + FPak3Password[k + 1];
      c := c + FPak3Password[k + 2];

      a := a - b;
      a := a - c;
      a := a xor (c shr $9);
      b := b - c;
      b := b - a;
      b := b xor (a shl $3);
      c := c - a;
      c := c - b;
      c := c xor (b shr $C);
      a := a - b;
      a := a - c;
      a := a xor (c shr $B);
      b := b - c;
      b := b - a;
      b := b and (a shl $7);
      c := c - a;
      c := c - b;
      c := c xor (b shr $A);
      a := a - b;
      a := a - c;
      a := a xor (c shr $4);
      b := b - c;
      b := b - a;
      b := b xor (a shl $1);
      c := c - a;
      c := c - b;
      c := c xor (b shr $8);

      Inc(k, 3);
      Dec(len, 3);
    end;

    c := c + DWORD(I * 2);

    if len >= 1 then a := a + FPak3Password[k + 0];
    if len >= 2 then a := a + FPak3Password[k + 1];
    a := a - b;
    a := a - c;
    a := a xor (c shr $B);
    b := b - c;
    b := b - a;
    b := b xor (a shl $1);
    c := c - a;
    c := c - b;
    c := c xor (b shr $F);
    a := a - b;
    a := a - c;
    a := a xor (c shr $2);
    b := b - c;
    b := b - a;
    b := b xor (a shl $7);
    c := c - a;
    c := c - b;
    c := c xor (b shr $9);
    a := a - b;
    a := a - c;
    a := a xor (c shr $1);
    b := b - c;
    b := b - a;
    b := b xor (a shl $3);
    c := c - a;
    c := c - b;
    c := c or (b shr $5);

    FPak3Password[I * 2 + 1] := c;
  end;
  {.$I VMProtectEnd.inc}
end;

function TPakImages.IsValidLzCheckCode(const sCheckCode: string): Boolean;
begin
    Result := (sCheckCode = 'D3DM2') or (sCheckCode = 'HXM2') or (sCheckCode = 'HeroM2') or (sCheckCode = 'HeroM2.')
              or (sCheckCode = 'HXM2.')  or (sCheckCode = 'FreeMF');
end;

procedure TPakImages.UpdateImageDataSize(nIndex, nDataSize: Integer);
begin
    if FPakFileType = pftLzPakV0 then begin
       m_ImageSizeList[nIndex] := Pointer(nDataSize);
    end;
end;

procedure TPakImages.UpdateIndex(PakKey:pTPakKey; IsCompareHeader:Boolean);
var
  I:Integer;
  S, FilePath:string;
  nLzV0BitCount:WORD;
  wPakType:WORD;
  FileStream:TFileStream;
  Header:TPakFileHeader;
  HeaderInfo:TFileHeaderInfo;
  IsDoExit:Boolean;
  LzHeaderInfo:TLzFileHeaderInfo;
  byRev:Byte;
begin
  if PakKey.ImageCount <= 0 then begin
    m_boNeedUpdate := False;
    Exit;
  end;

  FileStream := nil;

  nLzV0BitCount := (pakKey.PakType shr 8) and ($FF);
  wPakType := (pakKey.PakType and $00FF);

  IsDoExit := False;

  if Initialized then begin
    if IsCompareHeader and (Integer(FileHeader.ImageCount) = PakKey.ImageCount) and (Integer(FPakFileType) = wPakType)
      and CompareMem(@FileHeader.KeyData[0], @PakKey.KeyData[0], SizeOf(FileHeader.KeyData))
      and CompareMem(@FileHeader.Chain[0], @PakKey.Chain[0], SizeOf(FileHeader.Chain))
      and CompareMem(@FileHeader.Reserve[0], @PakKey.Reserve[0], SizeOf(FileHeader.Reserve)) then begin
      Exit;
    end else begin
      Finalize;
    end;
  end;

  LockFileStream; // 不知道为毛，这样搞减cpu占用
  try
    if FileExists(FileName) then begin
      try
        FileStream := TFileStream.Create(FileName, fmOpenWrite or fmShareDenyNone);
      except
        m_boNeedUpdate := False;
        //if FileStream <> nil then begin //HZQ 20230712 创建文件失败，会自动析构，再次调用可能会导致程序崩溃
        //    FreeAndNil(FileStream);
        //end;
        IsDoExit := True;
      end;
    end else begin
      FilePath := ExtractFilePath(IndexFileName);
      if not DirectoryExists(FilePath) then begin
        ForceDirectories(FilePath);
      end;

      try
        FileStream := TFileStream.Create(FileName, fmOpenWrite or fmShareDenyNone or fmCreate);
      except
        m_boNeedUpdate := False;
        //if FileStream <> nil then FreeAndNil(FileStream); //HZQ 20230712 创建文件失败，会自动析构，再次调用可能会导致程序崩溃
        IsDoExit := True;
      end;
    end;

    if not IsDoExit then begin
      //if PakKey.PakType in [0, 1, 2, 3, 4] then  //pftPak1, pftPak2, pftPak3, pftLzPakV0, pftLzPakV1
      if wPakType in [Ord(pftPak1), Ord(pftPak2), Ord(pftPak3), Ord(pftLzPakV0), Ord(pftLzPakV1) ] then begin
        FPakFileType := TPakFileType(wPakType);
      end else begin
        FPakFileType := pftPak1;
      end;

      FillChar(FileHeader, SizeOf(TPakFileHeader), 0);

      FileHeader.CreateDate := Now;
      FileHeader.ImageCount := PakKey.ImageCount;


      Move(PakKey.KeyData, FKeyData, SizeOf(FKeyData));
      Move(PakKey.Chain, FChain, SizeOf(FChain));
      Move(PakKey.KeyData, FileHeader.KeyData, SizeOf(FKeyData));
      Move(PakKey.Chain, FileHeader.Chain, SizeOf(FChain));
      Move(Pakkey.Reserve, FileHeader.Reserve, SizeOf(PakKey.Reserve));

      FileHeader.Title := 'www.gameofmir2.com';
      if FPakFileType = pftLzPakV0 then begin
          FileHeader.bfType := 0;
          FileHeader.CheckCode := EncryptS('D3DM2');
      end else if FPakFileType = pftLzPakV1 then begin
          FileHeader.bfType := 1;
          FileHeader.CheckCode := EncryptS('D3DM2');
      end else begin
          FileHeader.bfType := 2;
          FileHeader.CheckCode := EncryptS('GEEM2');
      end;

      if FPakFileType in [pftLzPakV0, pftLzPakV1] then begin
          FileHeader.Size := SizeOf(TLzFileHeaderInfo) + SizeOf(TPakFileHeader) + 1;
          FileHeader.IndexOffSet := SizeOf(TLzFileHeaderInfo) + SizeOf(TPakFileHeader) + 1;
      end else begin
          FileHeader.Size := SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader);
          FileHeader.IndexOffSet := SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader);
      end;

      if FPakFileType = pftLzPakV0 then begin
          FileHeader.BitCount := nLzV0BitCount; //LZPAKV0使用
      end else begin
          FileHeader.BitCount := 0;
      end;

      Header := FileHeader;
      FillChar(Header.KeyData, SizeOf(Header.KeyData), 0);
      FillChar(Header.Chain, SizeOf(Header.Chain), 0);

      if FPakFileType in [pftLzPakV0, pftLzPakV1] then begin
          LzHeaderInfo.FileType[0] := 'H';
          LzHeaderInfo.FileType[1] := 'X';
          LzHeaderInfo.FileType[2] := 'M';
          LzHeaderInfo.FileType[3] := '2';
          LzHeaderInfo.FileType[4] := '.';

          S := EncryptHeader_LzPak(Header);

          byRev := 0; //填充字节

          FileStream.Seek(0, soBeginning);
          FileStream.Write(LzHeaderInfo, SizeOf(LzHeaderInfo));
          FileStream.Write(S[1], SizeOf(TPakFileHeader));
          FileStream.Write(byRev, 1);
          FileStream.Size := SizeOf(LzHeaderInfo) + SizeOf(TPakFileHeader) + 1; //262
      end else begin
          if FPakFileType = pftPak1 then begin
            HeaderInfo.FileType := 'GEEM2';
            S := EncryptHeader_GameOfMir(Header);
          end else if FPakFileType = pftPak2 then begin
            HeaderInfo.FileType := 'GEEPAK2';
            S := EncryptHeader_Pak2(Header);
          end else if FPakFileType = pftPak3 then begin
            InitPak3Password;
            HeaderInfo.FileType := 'GEEPAK3';
            S := EncryptHeader_Pak3(Header);
          end;

          FileStream.Seek(0, soBeginning);
          FileStream.Write(HeaderInfo, SizeOf(TFileHeaderInfo));
          FileStream.Write(S[1], SizeOf(TPakFileHeader));
          FileStream.Size := SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader);
      end;

      m_IndexList.Count := PakKey.ImageCount;
      for I := 0 to PakKey.ImageCount - 1 do begin
        m_IndexList.Items[I] := nil;
      end;

      if FPakFileType = pftLzPakV0 then begin
          if m_ImageSizeList = nil then begin
              m_ImageSizeList := TList.Create;
          end;
          m_ImageSizeList.Count := PakKey.ImageCount;
          for I := 0 to PakKey.ImageCount - 1 do begin
             m_ImageSizeList.Items[I] := nil;
          end;
      end;

      WriteIndexList(FileStream, -1);

      FileStream.Free;

      Initialize_UpdateNewFile;
    end;
  finally
    UnLockFileStream;
  end;
end;

procedure TPakImages.WriteIndexList(FileStream:TFileStream; Index:Integer);
var
  I:Integer;
  InData:Pointer;
  InSize:Integer;
  OutData:Pointer;
  Offset:Integer;
  pLzIndexV0:PLzPakIndexVer0;
begin
  case FPakFileType of
    pftPak1:begin
        InSize := m_IndexList.Count * SizeOf(Integer);
        GetMem(InData, InSize);
        GetMem(OutData, InSize);

        for I := 0 to m_IndexList.Count - 1 do begin
          Offset := Integer(m_IndexList.Items[I]);
          if Offset > 0 then begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := Offset;
          end else begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := 0;
          end;
        end;

        EncryptIndexList(Indata, InSize, OutData);

        FileStream.Seek(SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader), soBeginning);
        FileStream.Write(OutData^, InSize);

        FreeMem(InData);
        FreeMem(OutData);
      end;
    pftPak2:begin
        InSize := m_IndexList.Count * SizeOf(Integer);
        GetMem(InData, InSize);
        GetMem(OutData, InSize);
        {.$I VMProtectBegin.inc}
        for I := 0 to m_IndexList.Count - 1 do begin
          Offset := Integer(m_IndexList.Items[I]);
          if Offset > 0 then begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := Offset xor FileHeader.Chain[0];
          end else begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := 0 xor FileHeader.Chain[0];
          end;
        end;
        EncryptIndexList_Pak2(Indata, InSize, OutData);
        {.$I VMProtectEnd.inc}

        FileStream.Seek(SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader), soBeginning);
        FileStream.Write(OutData^, InSize);

        FreeMem(InData);
        FreeMem(OutData);
      end;

    pftPak3:begin
        InSize := m_IndexList.Count * SizeOf(Integer);
        // 只更新4Byte
        if Index < 0 then begin
          GetMem(InData, InSize);
          {.$I VMProtectBegin.inc}// CPU占用不要这玩意 2020-04-18 00:54:42
          for I := 0 to m_IndexList.Count - 1 do begin
            Offset := Integer(m_IndexList.Items[I]);
            if Offset > 0 then begin
              {.$MESSAGE HINT '更改了PAK3的关于符号的操作，观测一段时间后无问题，删除此消息'}
              PInteger(Uint_Ptr(InData) + Cardinal(I) * SizeOf(Integer))^ := Cardinal(Offset) xor FPak3Password[I mod 64] xor (not Cardinal(i));
            end else begin
              PInteger(Integer(InData) + I * SizeOf(Integer))^ := 0 xor FPak3Password[I mod 64] xor (not Cardinal(I));
            end;
          end;
          {.$I VMProtectEnd.inc}

          FileStream.Seek(SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader), soBeginning);
          FileStream.Write(InData^, InSize);

          FreeMem(InData);
        end else if Index <= m_IndexList.Count - 1 then begin
          {.$I VMProtectBegin.inc}// CPU占用不要这玩意 2020-04-18 00:54:42
          Offset := Integer(m_IndexList.Items[Index]);
          if Offset > 0 then
            {.$MESSAGE HINT '更改了PAK3的关于符号的操作，观测一段时间后无问题，删除此消息'}
            Offset := Cardinal(Offset) xor FPak3Password[Index mod 64] xor (not Cardinal(Index))
          else
            Offset := 0 xor FPak3Password[Index mod 64] xor (not Cardinal(Index));
          {.$I VMProtectEnd.inc}

          FileStream.Seek(SizeOf(TFileHeaderInfo) + SizeOf(TPakFileHeader) + Index * SizeOf(Integer), soBeginning);
          FileStream.Write(Offset, SizeOf(Offset));
        end;
      end;

    pftLzPakV0:begin
        InSize := m_IndexList.Count * SizeOf(TLzPakIndexVer0);
        GetMem(InData, InSize);
        GetMem(OutData, InSize);
        pLzIndexV0 := InData;
        for I := 0 to m_IndexList.Count - 1 do begin
          pLzIndexV0.nImageOffSet := Integer(m_IndexList.Items[I]);
          pLzIndexV0.nDataSize := Integer(m_ImageSizeList.Items[i]);
          Inc(pLzIndexV0);
        end;

        EncryptIndexList(Indata, InSize, OutData);

        FileStream.Seek(LZ_PAK_FILE_HEADER_SIZE, soBeginning);
        FileStream.Write(OutData^, InSize);

        FreeMem(InData);
        FreeMem(OutData);
    end;

    pftLzPakV1:begin
        InSize := m_IndexList.Count * SizeOf(Integer);
        GetMem(InData, InSize);
        GetMem(OutData, InSize);

        for I := 0 to m_IndexList.Count - 1 do begin
          Offset := Integer(m_IndexList.Items[I]);
          if Offset > 0 then begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := Offset;
          end else begin
            PInteger(Integer(InData) + I * SizeOf(Integer))^ := 0;
          end;
        end;

        EncryptIndexList(Indata, InSize, OutData);

        FileStream.Seek(LZ_PAK_FILE_HEADER_SIZE, soBeginning);
        FileStream.Write(OutData^, InSize);

        FreeMem(InData);
        FreeMem(OutData);
    end;

  end;
end;

procedure TPakImages.Initialize;
var
  I:Integer;
  S:string;
  HeaderInfo:TFileHeaderInfo;
  LzHeaderInfo:TLzFileHeaderInfo;    
begin
  try
    FillChar(FileHeader, SizeOf(TPakFileHeader), #0);
    if not Initialized then begin
      if FileExists(FileName) then begin
        m_boUpdateIndex := True;
        m_boUpdateIndexing := False;
        if m_FileStream = nil then
          m_FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone);

        FillChar(FPak3Password, SizeOf(FPak3Password), 0);

        LockFileStream;
        try
          m_FileStream.Seek(0, soBeginning);
          SetLength(S, SizeOf(TFileHeaderInfo));
          m_FileStream.Read(S[1], SizeOf(TFileHeaderInfo));
          Move(S[1], HeaderInfo, SizeOf(TFileHeaderInfo));

          if CompareLStr(HeaderInfo.FileType, 'GEEM2', Length('GEEM2')) then begin
            FPakFileType := pftPak1;
            m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
          end else if CompareLStr(HeaderInfo.FileType, 'GEEPAK2', Length('GEEPAK2')) then begin
            FPakFileType := pftPak2;
            m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
          end else if CompareLStr(HeaderInfo.FileType, 'GEEPAK3', Length('GEEPAK3')) then begin
            FPakFileType := pftPak3;
            m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
          end else begin
             Move(S[1], LzHeaderInfo, SizeOf(LzHeaderInfo));
             if CompareLStr(string(LzHeaderInfo.FileType), 'HXM2', Length('HXM2')) then begin
                 FPakFileType :=  pftLzPakV0orV1; //此时还不知道LZPAK的类型
                 m_FileStream.Seek(SizeOf(LzHeaderInfo), soBeginning);
             end;
          end;  

          SetLength(S, SizeOf(TPakFileHeader));
          m_FileStream.Read(S[1], SizeOf(TPakFileHeader));
        finally
          UnLockFileStream;
        end;

        if FPakFileType = pftPak1 then begin
          FileHeader := DecryptHeader_GameOfMir(S)
        end else if FPakFileType = pftPak2 then begin
          FileHeader := DecryptHeader_Pak2(S)
        end else if FPakFileType = pftPak3 then begin
          InitPak3Password;
          FileHeader := DecryptHeader_Pak3(S);
        end else if FPakFileType = pftLzPakV0orV1 then begin
          FileHeader := DecryptHeader_LzPak(S);
          if FileHeader.bfType = 1 then begin
              FPakFileType := pftLzPakV1;
          end else if FileHeader.bfType = 0 then begin
              FPakFileType := pftLzPakV0;
              m_ImageSizeList := TList.Create;
          end;
          m_FileStream.Read(S[1], 1);
        end;

        if FPakFileType in [pftLzPakV0, pftLzPakV1] then begin
            Move(FKeyDataLz, FileHeader.KeyData, SizeOf(FKeyDataLz));
            Move(FChainLz, FileHeader.Chain, SizeOf(FChainLz));
        end else begin
            Move(FKeyData, FileHeader.KeyData, SizeOf(FKeyData));
            Move(FChain, FileHeader.Chain, SizeOf(FChain));
        end;

        S := DecryptS(FileHeader.CheckCode);

        if (FPakFileType in [pftPak1, pftPak2, pftPak3]) then begin
            FPasswordOK := (S = 'GEEM2');
        end else begin
            FPasswordOK := IsValidLzCheckCode(S);
        end;

        if FPasswordOK then begin
          // OutMessage('TPakImages.Initialize 3 ' + FileName + ' S:' + S);
          ImageCount := FileHeader.ImageCount;
          BitCount := FileHeader.BitCount;

          m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);
          for I := 0 to ImageCount - 1 do begin
            m_ImgArr[I].nWidth := 0;
            m_ImgArr[I].nHeight := 0;
            m_ImgArr[I].nPx := 0;
            m_ImgArr[I].nPy := 0;

            m_ImgArr[I].boUpdateStop := False;
            m_ImgArr[I].boUpdateStart := False;
            m_ImgArr[I].dwUpdateStartTick := MyGetTickCount;
          end;

          LoadIndex;
          Initialized := True;
        end else begin
          OutMessage(Format(DecodeResStr(SPakPasswordErr2), [FileName]));
        end;
      end else begin
        {$IF CLIENTEXE = 1}
        if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boAutoUpdate then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, -1, Self, nil) then begin
            m_boUpdateIndexing := True;
            m_dwUpdateIndexingTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
      end;
    end;
  except
    on E:Exception do begin
      //DebugOutStr('[Exception]TPakImages.Initialize; ' + E.Message + ' ' + FileName);
      raise Exception.Create(E.Message);
    end;
  end;
end;

procedure TPakImages.Initialize_UpdateNewFile;
var
  I:Integer;
  S:string;
  HeaderInfo:TFileHeaderInfo;
  LzHeaderInfo:TLzFileHeaderInfo;
begin
  if Initialized then Exit;

  if not FileExists(FileName) then begin
    {$IF CLIENTEXE = 1}
    if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boAutoUpdate then begin
      if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, -1, Self, nil) then begin
        m_boUpdateIndexing := True;
        m_dwUpdateIndexingTick := MyGetTickCount;
      end;
    end;
    {$IFEND}
  end;

  FillChar(FileHeader, SizeOf(TPakFileHeader), #0);
  m_boUpdateIndex := True;
  m_boUpdateIndexing := False;

  if m_FileStream = nil then
    m_FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone);

  LockFileStream;
  try
    m_FileStream.Seek(0, soBeginning);
    SetLength(S, SizeOf(TFileHeaderInfo));
    m_FileStream.Read(S[1], SizeOf(TFileHeaderInfo));
    Move(S[1], HeaderInfo, SizeOf(TFileHeaderInfo));

    if CompareLStr(HeaderInfo.FileType, 'GEEM2', Length('GEEM2')) then begin
      FillChar(FPak3Password, SizeOf(FPak3Password), 0);
      FPakFileType := pftPak1;
      m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
    end else if CompareLStr(HeaderInfo.FileType, 'GEEPAK2', Length('GEEPAK2')) then begin
      FillChar(FPak3Password, SizeOf(FPak3Password), 0);
      FPakFileType := pftPak2;
      m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
    end else if CompareLStr(HeaderInfo.FileType, 'GEEPAK3', Length('GEEPAK3')) then begin
      FPakFileType := pftPak3;
      m_FileStream.Seek(SizeOf(TFileHeaderInfo), soBeginning);
    end else begin
      Move(S[1], LzHeaderInfo, SizeOf(LzHeaderInfo));
      if CompareLStr(string(LzHeaderInfo.FileType), 'HXM2', Length('HXM2')) then begin
         FPakFileType :=  pftLzPakV0orV1; //此时还不知道LZPAK的类型
         m_FileStream.Seek(SizeOf(LzHeaderInfo), soBeginning);
      end;
    end;

    SetLength(S, SizeOf(TPakFileHeader));
    m_FileStream.Read(S[1], SizeOf(TPakFileHeader));
  finally
    UnLockFileStream;
  end;

  if FPakFileType = pftPak1 then
    FileHeader := DecryptHeader_GameOfMir(S)
  else if FPakFileType = pftPak2 then
    FileHeader := DecryptHeader_Pak2(S)
  else if FPakFileType = pftPak3 then begin
    FileHeader := DecryptHeader_Pak3(S);
  end else if FPakFileType = pftLzPakV0orV1 then begin
    FileHeader := DecryptHeader_LzPak(S);
    if FileHeader.bfType = 1 then begin
        FPakFileType := pftLzPakV1;
    end else if FileHeader.bfType = 0 then begin
        FPakFileType := pftLzPakV0;
        if m_ImageSizeList = nil  then begin
            m_ImageSizeList := TList.Create;
            m_ImageSizeList.Count := FileHeader.ImageCount;
        end; 
    end;
    m_FileStream.Read(S[1], 1);
    //UnitDes.GetKeyData(FSOriginPsssword, @FChain, @FKeyData);
  end;

  if FPakFileType in [pftLzPakV0, pftLzPakV1] then begin
      Move(FKeyDataLz, FileHeader.KeyData, SizeOf(FKeyDataLz));
      Move(FChainLz, FileHeader.Chain, SizeOf(FChainLz));
  end else begin
      Move(FKeyData, FileHeader.KeyData, SizeOf(FKeyData));
      Move(FChain, FileHeader.Chain, SizeOf(FChain));
  end;

  S := DecryptS(FileHeader.CheckCode);
  if (FPakFileType in [pftPak1, pftPak2, pftPak3]) then begin
      FPasswordOK := (S = 'GEEM2');
  end else begin
      FPasswordOK := IsValidLzCheckCode(S);
  end;
  //FPasswordOK := True;

  if FPasswordOK then begin
    // OutMessage('TPakImages.Initialize 3 ' + FileName + ' S:' + S);
    ImageCount := FileHeader.ImageCount;
    BitCount := FileHeader.BitCount;

    m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);
    for I := 0 to ImageCount - 1 do begin
      m_ImgArr[I].nWidth := 0;
      m_ImgArr[I].nHeight := 0;
      m_ImgArr[I].nPx := 0;
      m_ImgArr[I].nPy := 0;

      m_ImgArr[I].boUpdateStop := False;
      m_ImgArr[I].boUpdateStart := False;
      m_ImgArr[I].dwUpdateStartTick := MyGetTickCount;
    end;

    {
    m_IndexList.Capacity := FileHeader.ImageCount;
    m_IndexList.Count := FileHeader.ImageCount;;
    for I := 0 to m_IndexList.Count - 1 do
    begin
      m_IndexList.Items[I] := nil;
    end;
    }

    Initialized := True;
  end
  else begin
    OutMessage(Format(DecodeResStr(SPakPasswordErr2), [FileName]));
  end;
end;

procedure TPakImages.Finalize;
var
  I:Integer;
begin
  Initialized := False;
  Lock;
  try
    IndexList.Clear;
    GrayIndexList.Clear;
    BrightIndexList.Clear;
    if m_ImgArr <> nil then begin
      for I := 0 to ImageCount - 1 do begin
        if m_ImgArr[I].Surface <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Surface);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Gray <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Gray);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Bright <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Bright);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;
        if m_ImgArr[I].Bitmap <> nil then begin
          FreeAndNil(m_ImgArr[I].Bitmap);
        end;
      end;
      FreeMem(m_ImgArr);
    end;

    m_ImgArr := nil;
    ImageCount := 0;
    m_IndexList.Clear;
    if m_FileStream <> nil then
      FreeAndNil(m_FileStream);
  finally
    UnLock;
  end;
end;

procedure TPakImages.EncryptIndexList(InData:Pointer; InSize:Integer; out OutData:Pointer);
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  EncryptCBC(Indata^, Outdata^, InSize, @Chain, @KeyData);
end;

procedure TPakImages.DecryptIndexList(InData:Pointer; InSize:Integer; out OutData:Pointer);
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  DecryptCBC(Indata^, Outdata^, InSize, @Chain, @KeyData);
end;

procedure TPakImages.EncryptIndexList_Pak2(InData:Pointer; InSize:Integer; out OutData:Pointer);
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  {.$I VMProtectBegin.inc}
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  EncryptCBC(Indata^, Outdata^, InSize, @Chain, @KeyData);

  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  EncryptCBC(Outdata^, Outdata^, InSize, @Chain, @KeyData);
  {.$I VMProtectEnd.inc}
end;

procedure TPakImages.DecryptIndexList_Pak2(InData:Pointer; InSize:Integer; out OutData:Pointer);
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  {.$I VMProtectBegin.inc}
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  DecryptCBC(Indata^, Outdata^, InSize, @Chain, @KeyData);

  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  DecryptCBC(Outdata^, Outdata^, InSize, @Chain, @KeyData);
  {.$I VMProtectEnd.inc}
end;

function TPakImages.EncryptHeader(Header:TPakFileHeader):string;
var
  S:string;
begin
  SetLength(S, SizeOf(TPakFileHeader));
  Move(Header, S[1], SizeOf(TPakFileHeader));
  Result := EncryptStrDes(S, IntToStr(442517066));
end;

function TPakImages.DecryptHeader(const Str:string):TPakFileHeader;
var
  S:string;
begin
  SetLength(S, SizeOf(TPakFileHeader));
  DecryptDes(Str[1], S[1], SizeOf(TPakFileHeader), IntToStr(442517066));
  Move(S[1], Result, SizeOf(TPakFileHeader));
end;

function TPakImages.EncryptHeader_GameOfMir(Header:TPakFileHeader):string;
var
  S:string;
begin
  SetLength(S, SizeOf(TPakFileHeader));
  Move(Header, S[1], SizeOf(TPakFileHeader));
  Result := EncryptStrDes(S, IntToStr(PakEncryKey^));
end;

function TPakImages.DecryptHeader_GameOfMir(const Str:string):TPakFileHeader;
var
  S:string;
begin
  SetLength(S, SizeOf(TPakFileHeader));
  DecryptDes(Str[1], S[1], SizeOf(TPakFileHeader), IntToStr(PakEncryKey^));
  Move(S[1], Result, SizeOf(TPakFileHeader));
end;

function TPakImages.EncryptHeader_Pak2(Header:TPakFileHeader):string;
begin
  SetLength(Result, SizeOf(TPakFileHeader));
  EncryptDes_New(Header, Result[1], Length(Result), IntToStr(PakEncryKey^));
end;

function TPakImages.DecryptHeader_Pak2(const Str:string):TPakFileHeader;
begin
  DecryptDes_New(Str[1], Result, SizeOf(TPakFileHeader), IntToStr(PakEncryKey^));
end;

function TPakImages.EncryptHeader_Pak3(Header:TPakFileHeader):string;
begin
  SetLength(Result, SizeOf(TPakFileHeader));
  AESEncrypt(@FPak3Password[0], 16, @Header, @Result[1], SizeOf(TPakFileHeader));
end;

function TPakImages.DecryptHeader_Pak3(const Str:string):TPakFileHeader;
begin
  AESDecrypt(@FPak3Password[0], 16, @Str[1], @Result, SizeOf(TPakFileHeader));
end;

function TPakImages.DecryptHeader_LzPak(const str:string):TPakFileHeader;
const
    sHeadPassword:string = '442517066';
begin
    DecryptDes(str[1], Result, SizeOf(Result) , sHeadPassword);
end;

function TPakImages.EncryptHeader_LzPak(Header:TPakFileHeader):string;
const
    sHeadPassword:string = '442517066';
begin
    SetLength(Result, Sizeof(Header));
    EncryptDes(Header, Result[1], SizeOf(Header), sHeadPassword);
end;

function TPakImages.EncryptS(S:string):string;
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  if S <> '' then begin
    SetLength(Result, Length(S));
    Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
    Move(FileHeader.Chain, Chain, SizeOf(Chain));
    EncryptCBC(S[1], Result[1], Length(S), @Chain, @KeyData);
  end
  else
    Result := '';
end;

function TPakImages.DecryptS(S:string):string;
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  if S <> '' then begin
    SetLength(Result, Length(S));
    Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
    Move(FileHeader.Chain, Chain, SizeOf(Chain));
    DecryptCBC(S[1], Result[1], Length(S), @Chain, @KeyData);
  end
  else
    Result := '';
end;

procedure TPakImages.ReadImageHeader(var Buffer; Count:Longint);
var
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  m_FileStream.Read(Buffer, Count);
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  DecryptCBC(Buffer, Buffer, Count, @Chain, @KeyData);
end;

procedure TPakImages.ReadImageHeader_Pak2(var Buffer; Count:Longint);
var
  I:Integer;
  KeyData:array[0..31] of DWord;
  Chain:array[0..20 - 1] of Byte;
begin
  m_FileStream.Read(Buffer, Count);

  {.$I VMProtectBegin.inc}
  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  for I := 0 to Length(KeyData) - 1 do begin
    if I mod 2 = 0 then
      KeyData[I] := KeyData[I] xor KeyData[I + 1];
  end;
  DecryptCBC(Buffer, Buffer, Count, @Chain, @KeyData);

  Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
  Move(FileHeader.Chain, Chain, SizeOf(Chain));
  DecryptCBC(Buffer, Buffer, Count, @Chain, @KeyData);

  {.$I VMProtectEnd.inc}
end;

procedure TPakImages.ReadImageHeader_Pak3(Index:Integer; var Buffer; Count:Longint);
var
  S:string;
  K1, K2:Integer;
begin
  m_FileStream.Read(Buffer, Count);

  {$IF PAK3_ENCODE = 1}
  {$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectBegin.inc}
  {$IFEND}
  SetLength(S, 16);

  K1 := (Index + 00) mod 64;
  K2 := (Index + 14) mod 64;
  K1 := FPak3Password[K1] xor FPak3Password[K2];
  Move(K1, S[1 + 00], 4);

  K1 := (Index + 12) mod 64;
  K2 := (Index + 19) mod 64;
  K1 := FPak3Password[K1] and FPak3Password[K2];
  Move(K1, S[1 + 04], 4);

  K1 := (Index + 10) mod 64;
  K2 := (Index + 28) mod 64;
  K1 := FPak3Password[K2] xor (not FPak3Password[K1]);
  Move(K1, S[1 + 08], 4);

  K1 := (Index + 01) mod 64;
  K1 := FPak3Password[K1];
  Move(K1, S[1 + 12], 4);

  {$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectEnd.inc}
  {$IFEND}

  AESDecrypt(@S[1], Length(S), @Buffer, @Buffer, Count);

  {$IFEND}
end;

(*
function TPakImages.MakeDibByBitCount(nBitCount:Integer; nW, nH:Integer):TDIB;
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

function TPakImages.MakeDibByPixelFormat(pf:TPixelFormat; nW, nH:Integer):TDIB;
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
*)

procedure TPakImages.LoadIndex;
var
  I, Index,  nSingleIndexSize:Integer;
  OutData:Pointer;
  InData:Pointer;
  InSize:Integer;
  //OffetArray: array of Integer;
  pLzPakIndexV0:PLzPakIndexVer0;
begin
  m_IndexList.Capacity := FileHeader.ImageCount;
  m_IndexList.Count := FileHeader.ImageCount;

  if FPakFileType = pftLzPakV0 then begin
      nSingleIndexSize := SizeOf(TLzPakIndexVer0);
      m_ImageSizeList.Capacity := FileHeader.ImageCount;
      m_ImageSizeList.Count := FileHeader.ImageCount;
  end else begin
      nSingleIndexSize := SizeOf(Integer);
  end;

  InSize := FileHeader.ImageCount * Cardinal(nSingleIndexSize);

  if InSize = 0 then Exit;

  GetMem(InData, InSize);

  LockFileStream;
  try
    m_FileStream.Seek(FileHeader.IndexOffSet, soBeginning);
    m_FileStream.Read(InData^, InSize);
  finally
    UnLockFileStream;
  end;

  if FPakFileType = pftPak1 then begin
    GetMem(OutData, InSize);

    DecryptIndexList(InData, InSize, OutData);

    for I := 0 to FileHeader.ImageCount - 1 do begin
      Index := PInteger(Integer(OutData) + I * SizeOf(Integer))^;
      m_IndexList.Items[I] := Pointer(Index);
    end;

    FreeMem(OutData);
  end  else if FPakFileType = pftPak2 then begin
    GetMem(OutData, InSize);

    DecryptIndexList_Pak2(InData, InSize, OutData);

    for I := 0 to FileHeader.ImageCount - 1 do begin
      Index := PInteger(Integer(OutData) + I * SizeOf(Integer))^;
      Index := Index xor FileHeader.Chain[0];
      m_IndexList.Items[I] := Pointer(Index);
    end;

    FreeMem(OutData);
  end else if FPakFileType = pftPak3 then begin
    {$IF PRIVATE_CLIENT = 0}
    {.$I VMProtectBegin.inc}// CPU占用不要这玩意 2020-04-18 00:54:42
    {$IFEND}
    for I := 0 to FileHeader.ImageCount - 1 do begin
      Index := PInteger(Integer(InData) + I * SizeOf(Integer))^;
      {.$MESSAGE HINT '更改了PAK3的关于符号的操作，观测一段时间后无问题，删除此消息'}
      Index := Cardinal(Index) xor FPak3Password[I mod 64] xor (not Cardinal(I));
      m_IndexList.Items[I] := Pointer(Index);
    end;
    {$IF PRIVATE_CLIENT = 0}
    {.$I VMProtectEnd.inc}
    {$IFEND}
  end else if (FPakFileType = pftLzPakV0) or (FPakFileType = pftLzPakV1) then begin
     GetMem(OutData, InSize);
     DecryptIndexList(InData, InSize, OutData);
     if FPakFileType = pftLzPakV1 then begin
         for I := 0 to FileHeader.ImageCount - 1 do begin
            Index := PInteger(Integer(OutData) + I * SizeOf(Integer))^;
            m_IndexList.Items[I] := Pointer(Index);
         end;
     end else if FPakFileType = pftLzPakV0 then begin
         pLzPakIndexV0 := OutData;
         for i := 0 to FileHeader.ImageCount - 1 do begin
             m_IndexList.Items[i] := Pointer(pLzPakIndexV0.nImageOffSet);
             m_ImageSizeList.Items[i] := Pointer(pLzPakIndexV0.nDataSize);
             Inc(pLzPakIndexV0);
         end;
     end;
     FreeMem(OutData);
  end else begin
    for I := 0 to FileHeader.ImageCount - 1 do begin
      m_IndexList.Items[I] := nil;
    end;
  end;

  FreeMem(InData);
end;

//HZQ 20230712 WriteHeader函数有明显的BUG，没有考率到TFileHeaderInfo的存在, 程序中也未使用
procedure TPakImages.WriteHeader();
var
  S:string;
  Header:TPakFileHeader;
begin
  if Initialized then begin
    ImageCount := m_IndexList.Count;
    FileHeader.bfType := 2;
    FileHeader.ImageCount := m_IndexList.Count;
    FileHeader.IndexOffSet := SizeOf(TPakFileHeader);

    Header := FileHeader;
    FillChar(Header.KeyData, SizeOf(Header.KeyData), 0);
    FillChar(Header.Chain, SizeOf(Header.Chain), 0);

    S := EncryptHeader(Header);
    LockFileStream;
    try
      m_FileStream.Seek(0, soBeginning);
      m_FileStream.Write(S[1], SizeOf(TPakFileHeader));
    finally
      UnLockFileStream;
    end;
  end;
end;

function TPakImages.GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap;
begin
  Result := nil;
  m_dwUseCheckTick := MyGetTickCount;
  if (Index >= 0) then begin
    if Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) then begin
      Lock;
      try
        if m_ImgArr[Index].Surface = nil then begin

          LoadDxBitmap(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
          PX := m_ImgArr[Index].nPx;
          PY := m_ImgArr[Index].nPy;

          if m_ImgArr[Index].Bitmap <> nil then
            IndexList.Add(Pointer(Index));
          Result := m_ImgArr[Index].Bitmap;
        end else begin
          m_ImgArr[Index].dwLatestTime := MyGetTickCount;
          PX := m_ImgArr[Index].nPx;
          PY := m_ImgArr[Index].nPy;
          Result := m_ImgArr[Index].Bitmap;
        end;
      finally
        UnLock;
      end;
    end;
  end;
end;

function TPakImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index >= 0) then begin
    Lock;
    try
      if Initialized and (Index < ImageCount) and FPasswordOK then begin
        FreeOldMemorys_Ex;

        if m_ImgArr[Index].Surface = nil then begin
          try
            if (Index >= 0) and (Index < m_IndexList.Count) then begin
              if FPakFileType = pftLzPakV0 then begin
                 //OutputDebugString(PChar(Format('IndexCount = %d, SizeListCount = %d', [m_IndexList.Count, m_ImageSizeList.Count])));
                 LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), Integer(m_ImageSizeList.Items[Index]), dtsNormal, @m_ImgArr[Index]);
              end else if FPakFileType = pftLzPakV1 then begin
                 LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), 0, dtsNormal, @m_ImgArr[Index]);
              end else begin
                 LoadDxImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
              end;
            end;
          except
            OutMessage(Format(DecodeResStr(SPakGetCacheImageErr), [FileName, Index]));
          end;

          PX := m_ImgArr[Index].nPx;
          PY := m_ImgArr[Index].nPy;

          m_ImgArr[Index].dwLatestTime := MyGetTickCount;

          if m_ImgArr[Index].Surface <> nil then
            IndexList.Add(Pointer(Index));

          Result := m_ImgArr[Index].Surface;

          {$IF CLIENTEXE = 1}
          if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
            ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
            g_boDeviceInitializeOK then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
              m_ImgArr[Index].boUpdateStart := True;
              m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
            end;
          end;
          {$IFEND}
          if Result = nil then
            Result := g_NullImage;
        end else begin
          m_ImgArr[Index].dwLatestTime := MyGetTickCount;
          PX := m_ImgArr[Index].nPx;
          PY := m_ImgArr[Index].nPy;
          Result := m_ImgArr[Index].Surface;
        end;
      end
      else begin
        {$IF CLIENTEXE = 1}
        if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
          if Index >= ImageCount then
            ImageIndex := 0
          else
            ImageIndex := -1;

          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
            m_boUpdateIndexing := True;
            m_dwUpdateIndexingTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
      end;
    finally
      UnLock;
    end;
  end;
end;

function TPakImages.GetCachedLzImageSize(Index:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
    nPosition:Integer;
    ImageInfoV0:TPakImageInfo;
    NewImageInfo:TNewPakImageInfo;
    nLzV0ImageDataSize:Integer;
    bCanRead:Boolean;
begin
    Result := False;
    nLzV0ImageDataSize := 0;
    Lock;
    try
      if(Initialized and (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and FPasswordOK) then begin
        if m_ImgArr[Index].nWidth * m_ImgArr[Index].nHeight = 0 then begin
          nPosition := Integer(m_IndexList.Items[Index]);

          if FPakFileType = pftLzPakV0 then begin
              nLzV0ImageDataSize := Integer(m_ImageSizeList.Items[Index]);
              bCanRead := (nLzV0ImageDataSize > 0) and (nPosition >= LZ_PAK_FILE_HEADER_SIZE)
                      and (nPosition + SizeOf(TPakImageInfo) <= m_FileStream.Size);
          end else if FPakFileType = pftLzPakV1 then begin
              bCanRead :=  (nPosition >= LZ_PAK_FILE_HEADER_SIZE) and (nPosition + SizeOf(TNewPakImageInfo) <= m_FileStream.Size);
          end else begin
              bCanRead := False;
          end;   

          if bCanRead then begin
            if FPakFileType= pftLzPakV0 then begin
                //nLzV0ImageDataSize := Integer(m_ImageSizeList.Items[Index]);
                LockFileStream;
                try
                  m_FileStream.Position := nPosition;
                  ReadImageHeader(ImageInfoV0, SizeOf(ImageInfoV0));
                finally
                  UnLockFileStream;
                end;
            end else if FPakFileType = pftLzPakV1 then begin
                LockFileStream;
                try
                  m_FileStream.Position := nPosition;
                  ReadImageHeader(NewImageInfo, SizeOf(NewImageInfo));
                finally
                  UnLockFileStream;
                end;
            end;

            if FPakFileType = pftLzPakV0 then begin
              if not IsValidLzImageInfoV0(ImageInfoV0, Index, nLzV0ImageDataSize, FileHeader.BitCount) then begin
                if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                  and g_boAutoUpdate then begin
                  {$IF CLIENTEXE = 1}
                  if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, -1, Self, nil) then begin
                    m_boUpdateIndexing := True;
                    m_dwUpdateIndexingTick := MyGetTickCount;
                  end;
                  {$IFEND}
                end;
                Exit;
              end;

              m_ImgArr[Index].nWidth := ImageInfoV0.wW;
              m_ImgArr[Index].nHeight := ImageInfoV0.wH;
              m_ImgArr[Index].nPx := ImageInfoV0.wpx;
              m_ImgArr[Index].nPy := ImageInfoV0.wpy;

              ASize.cx := ImageInfoV0.wW;
              ASize.cy := ImageInfoV0.wH;
              APoint.X := ImageInfoV0.wpx;
              APoint.Y := ImageInfoV0.wpy;
            end else begin
              // 文件发生错误后，全部重更新 2020-08-03 21:10:01
              if not IsValidLzImageInfoV1(NewImageInfo, Index) then begin
                if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
                  and g_boAutoUpdate then begin
                  {$IF CLIENTEXE = 1}
                  if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, -1, Self, nil) then begin
                    m_boUpdateIndexing := True;
                    m_dwUpdateIndexingTick := MyGetTickCount;
                  end;
                  {$IFEND}
                end;
                Exit;
              end;

              m_ImgArr[Index].nWidth := NewImageInfo.nWidth;
              m_ImgArr[Index].nHeight := NewImageInfo.nHeight;
              m_ImgArr[Index].nPx := NewImageInfo.px;
              m_ImgArr[Index].nPy := NewImageInfo.py;

              ASize.cx := NewImageInfo.nWidth;
              ASize.cy := NewImageInfo.nHeight;
              APoint.X := NewImageInfo.px;
              APoint.Y := NewImageInfo.Py;
            end;
            Result := True;
          end;
        end else begin
          ASize.cx := m_ImgArr[Index].nWidth;
          ASize.cy := m_ImgArr[Index].nHeight;
          APoint.X := m_ImgArr[Index].nPx;
          APoint.Y := m_ImgArr[Index].nPy;
          Result := True;
        end;
      end;
    finally
        UnLock;
    end;
end;

function TPakImages.GetCachedImageSize(Index:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
  nPosition:Integer;
  NewImageInfo:TNewPakImageInfo;
begin
  if FPakFileType in [pftLzPakV0, pftLzPakV1] then begin
      Result := GetCachedLzImageSize(Index, ASize, APoint);
      Exit;
  end;

  Result := False; 
  Lock;
  try
    if (Initialized and (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and FPasswordOK) then begin
      if m_ImgArr[Index].nWidth * m_ImgArr[Index].nHeight = 0 then begin
        nPosition := Integer(m_IndexList.Items[Index]);

        if (nPosition >= SizeOf(TPakFileHeader)) and (nPosition + SizeOf(TNewPakImageInfo) <= m_FileStream.Size) then begin
          if FPakFileType = pftPak1 then begin
            LockFileStream;
            try
              m_FileStream.Position := nPosition;
              ReadImageHeader(NewImageInfo, SizeOf(TNewPakImageInfo));
            finally
              UnLockFileStream;
            end;
          end else if FPakFileType = pftPak2 then begin
            LockFileStream;
            try
              m_FileStream.Position := nPosition;
              ReadImageHeader_Pak2(NewImageInfo, SizeOf(TNewPakImageInfo));
            finally
              UnLockFileStream;
            end;
          end else if FPakFileType = pftPak3 then begin
            LockFileStream;
            try
              m_FileStream.Position := nPosition;
              ReadImageHeader_Pak3(Index, NewImageInfo, SizeOf(TNewPakImageInfo));
            finally
              UnLockFileStream;
            end;
          end;

          // 文件发生错误后，全部重更新 2020-08-03 21:10:01
          if (not (NewImageInfo.PixelFormat in [pf8bit, pf15bit, pf16bit, pf24bit, pf32bit])) or
            ((Abs(NewImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(NewImageInfo.py) > MAX_IMAGE_HEIGHT)) or
            (NewImageInfo.Length >= MAX_IMAGE_SIZE) or
            ((NewImageInfo.nWidth <= 0) or (NewImageInfo.nHeight <= 0)) or
            ((NewImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (NewImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
            if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime))
              and g_boAutoUpdate then begin
              {$IF CLIENTEXE = 1}
              if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, -1, Self, nil) then begin
                m_boUpdateIndexing := True;
                m_dwUpdateIndexingTick := MyGetTickCount;
              end;
              {$IFEND}
            end;

            Exit;
          end;  

          m_ImgArr[Index].nWidth := NewImageInfo.nWidth;
          m_ImgArr[Index].nHeight := NewImageInfo.nHeight;
          m_ImgArr[Index].nPx := NewImageInfo.px;
          m_ImgArr[Index].nPy := NewImageInfo.py;

          ASize.cx := NewImageInfo.nWidth;
          ASize.cy := NewImageInfo.nHeight;
          APoint.X := NewImageInfo.px;
          APoint.Y := NewImageInfo.Py;
          Result := True;
        end;
      end else begin
        ASize.cx := m_ImgArr[Index].nWidth;
        ASize.cy := m_ImgArr[Index].nHeight;
        APoint.X := m_ImgArr[Index].nPx;
        APoint.Y := m_ImgArr[Index].nPy;
        Result := True;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TPakImages.GetCachedGrayImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if Initialized and (Index < ImageCount) and FPasswordOK then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        if (Index >= 0) and (Index < m_IndexList.Count) then begin
            if FPakFileType = pftLzPakV0 then begin
                LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), Integer(m_ImageSizeList.Items[Index]), dtsGray, @m_ImgArr[Index]);
            end else if FPakFileType = pftLzPakV1 then begin
                LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), 0, dtsGray, @m_ImgArr[Index]);
            end else begin
                LoadDxGrayImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
            end;
        end;

        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Gray;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if Index >= ImageCount then
          ImageIndex := 0
        else
          ImageIndex := -1;

        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TPakImages.GetCachedBrightImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if Initialized and (Index < ImageCount) and FPasswordOK then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        if (Index >= 0) and (Index < m_IndexList.Count) then begin
            if FPakFileType = pftLzPakV0 then begin
                LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), Integer(m_ImageSizeList.Items[Index]), dtsBright, @m_ImgArr[Index]);
            end else if FPakFileType = pftLzPakV1 then begin
                LoadDxImageLzPak(Index, Integer(m_IndexList.Items[Index]), 0, dtsBright, @m_ImgArr[Index]);
            end else begin
                LoadDxBrightImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
            end;
        end;

        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Bright;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if Index >= ImageCount then
          ImageIndex := 0
        else
          ImageIndex := -1;

        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TPakImages.GetCachedSurface(Index:Integer):TTexture;
var
  nPosition:Integer;
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock; // ★★★★★★★★这个要写到最外层，微端更新的时候才不占用cpu★★★★ 2020-04-17 22:52:50
  try
    if Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) and FPasswordOK then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        nPosition := Integer(m_IndexList.Items[Index]);


        if FPakFileType = pftLzPakV0 then begin
            LoadDxImageLzPak(Index, nPosition, Integer(m_ImageSizeList.Items[Index]), dtsNormal, @m_ImgArr[Index]);
        end else if FPakFileType = pftLzPakV1 then begin
            LoadDxImageLzPak(Index, nPosition, 0, dtsNormal, @m_ImgArr[Index]);
        end else begin
            LoadDxImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
        end;  
        //LoadDxImage(nPosition, @m_ImgArr[Index], Index);

        m_ImgArr[Index].dwLatestTime := MyGetTickCount;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        Result := m_ImgArr[Index].Surface;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if Index >= ImageCount then
          ImageIndex := 0
        else
          ImageIndex := -1;

        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TPakImages.GetCachedGray(Index:Integer):TTexture;
var
  nPosition:Integer;
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock; // ★★★★★★★★这个要写到最外层，微端更新的时候才不占用cpu★★★★ 2020-04-17 22:52:50
  try
    if Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) and FPasswordOK then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        nPosition := Integer(m_IndexList.Items[Index]);

        if FPakFileType = pftLzPakV0 then begin
            LoadDxImageLzPak(Index, nPosition, Integer(m_ImageSizeList.Items[Index]), dtsGray, @m_ImgArr[Index]);
        end else if FPakFileType = pftLzPakV1 then begin
            LoadDxImageLzPak(Index, nPosition, 0, dtsGray, @m_ImgArr[Index]);
        end else begin
            LoadDxGrayImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
        end;
        //LoadDxGrayImage(nPosition, @m_ImgArr[Index], Index); 

        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        Result := m_ImgArr[Index].Gray;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if Index >= ImageCount then
          ImageIndex := 0
        else
          ImageIndex := -1;

        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TPakImages.GetCachedBright(Index:Integer):TTexture;
var
  nPosition:Integer;
  ImageIndex:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock; // ★★★★★★★★这个要写到最外层，微端更新的时候才不占用cpu★★★★ 2020-04-17 22:52:50
  try
    if Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) and FPasswordOK then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        nPosition := Integer(m_IndexList.Items[Index]);

        if FPakFileType = pftLzPakV0 then begin
            LoadDxImageLzPak(Index, nPosition, Integer(m_ImageSizeList.Items[Index]), dtsBright, @m_ImgArr[Index]);
        end else if FPakFileType = pftLzPakV1 then begin
            LoadDxImageLzPak(Index, nPosition, 0, dtsBright, @m_ImgArr[Index]);
        end else begin
            LoadDxBrightImage(Integer(m_IndexList.Items[Index]), @m_ImgArr[Index], Index);
        end;
        //LoadDxBrightImage(nPosition, @m_ImgArr[Index], Index);

        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImagePak, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        Result := m_ImgArr[Index].Bright;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if Index >= ImageCount then
          ImageIndex := 0
        else
          ImageIndex := -1;

        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexPak, ImageIndex, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

procedure TPakImages.LoadDxBitmap(Position:Integer; DXImage:pTDXImage; Index:Integer);
begin

end;

{
procedure TPakImages.ReadLzImageHeader(var Buffer; Count:Longint);
var
    KeyData:array[0..31] of DWord;
    Chain:array[0..20 - 1] of Byte;
begin
    m_FileStream.Read(Buffer, Count);
    Move(FileHeader.KeyData, KeyData, SizeOf(KeyData));
    Move(FileHeader.Chain, Chain, SizeOf(Chain));
    DecryptCBC(Buffer, Buffer, Count, @Chain, @KeyData);
end; }

function TPakImages.IsValidLzImageInfoV0(const ImageInfoV0:TPakImageInfo; nIndex:Integer; nDataSize:Integer; nBitCount:Integer):Boolean;
begin
    Result := True;
    if not ((nBitCount = 8) or (nBitCount = 16) or (nBitCount = 24) or (nBitCount = 32))  then begin
        Result := False;
        Exit;
    end;

    if ((Abs(ImageInfoV0.wPx) > MAX_IMAGE_WIDTH) or (Abs(ImageInfoV0.wPy) > MAX_IMAGE_HEIGHT)) then begin
       OutMessage(Format(DecodeResStr(SPakLoadDxImageErr), [FileName, nIndex, ImageInfoV0.wPx, ImageInfoV0.wPy]));
       Result := False;
       Exit;
    end;

    if (nDataSize >= MAX_IMAGE_SIZE) then begin
        OutMessage(Format(DecodeResStr(SPakLoadDxImageLenErr), [FileName, nIndex, nDataSize]));
        Result := False;
        Exit;
    end;

    if ((ImageInfoV0.wW <= 0) or (ImageInfoV0.wH <= 0)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, nIndex, ImageInfoV0.wW, ImageInfoV0.wH]));
      Result := False;
      Exit;
    end;

    if ((ImageInfoV0.wW >= MAX_IMAGE_WIDTH) or (ImageInfoV0.wH >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, nIndex, ImageInfoV0.wW, ImageInfoV0.wH]));
      Result := False;
    end;
end;

function TPakImages.IsValidLzImageInfoV1(const ImageInfoV1:TNewPakImageInfo; nIndex:Integer):Boolean;
begin
    Result := True;
    if not (ImageInfoV1.PixelFormat in [pf8bit, pf15bit, pf16bit, pf24bit, pf32bit]) then begin
      Result := False;
      Exit;
    end;

    if ((Abs(ImageInfoV1.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfoV1.py) > MAX_IMAGE_HEIGHT)) then begin
       OutMessage(Format(DecodeResStr(SPakLoadDxImageErr), [FileName, nIndex, ImageInfoV1.px, ImageInfoV1.py]));
       Result := False;
       Exit;
    end;

    if (ImageInfoV1.Length >= MAX_IMAGE_SIZE) then begin
        OutMessage(Format(DecodeResStr(SPakLoadDxImageLenErr), [FileName, nIndex, ImageInfoV1.Length]));
        Result := False;
        Exit;
    end;

    if ((ImageInfoV1.nWidth <= 0) or (ImageInfoV1.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, nIndex, ImageInfoV1.nWidth, ImageInfoV1.nHeight]));
      Result := False;
      Exit;
    end;

    if ((ImageInfoV1.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfoV1.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, nIndex, ImageInfoV1.nWidth, ImageInfoV1.nHeight]));
      Result := False;
    end;
end;

function TPakImages.LoadLzImageDataV0(const ImageInfoV0:TPakImageInfo; msData:TMemoryStream; var Source, AlphaSource:TDIB):Boolean;
var
    pCompressedData, pUncompressedData:Pointer;
    nWidthBytes, nBytesPerPixel, nImgSize, nUncompressedSize:Integer;
begin
    Result := False;
    Source := nil;
    AlphaSource := nil;
    pCompressedData := msData.Memory;
    nWidthBytes := WidthBytes(FileHeader.BitCount, ImageInfoV0.wW);
    nImgSize :=  nWidthBytes * ImageInfoV0.wH;
    case ImageInfoV0.btEncr0 of
        1:begin
            pUncompressedData := GetMemory(nImgSize * 2);
            nBytesPerPixel := FileHeader.BitCount div 8;
            try
                RLEUnit.DecodeRLE(pCompressedData, pUncompressedData, ImageInfoV0.wW, ImageInfoV0.wH, nBytesPerPixel);
                Source := MakeDibByBitCount(FileHeader.BitCount, ImageInfoV0.wW, ImageInfoV0.wH);
                if Source <> nil then begin
                    Move(pUncompressedData^, Source.PBits^, Source.Height * Source.WidthBytes);
                    Result := True;
                end;
            except

            end;
            if pUncompressedData <> nil then FreeMemory(pUncompressedData);
        end;

        2:begin
           try
               ZLibEx.DecompressBuf(pCompressedData, msData.Size, nImgsize, pUncompressedData, nUncompressedSize);
               if nUncompressedSize = nImgSize then begin
                   Source := MakeDibByBitCount(FileHeader.BitCount, ImageInfoV0.wW, ImageInfoV0.wH);
                   if Source <> nil then begin
                       Move(pUncompressedData^, Source.PBits^, Source.Height * Source.WidthBytes);
                       Result := True;
                   end;
               end;
           except

           end;

           if pUncompressedData <> nil then FreeMemory(pUncompressedData);
        end;

        else begin
             pUncompressedData := msData.Memory;
             Source := MakeDibByBitCount(FileHeader.BitCount, ImageInfoV0.wW, ImageInfoV0.wH);
             Move(pUncompressedData^, Source.PBits^, Source.Height * Source.WidthBytes);
             Result := True;
        end;
    end;
end;

function TPakImages.GetBitCountByPixelFormat(pf:TPixelFormat):Integer;
begin
    case pf of
      pf1bit: Result := 1;
      pf4bit: Result := 4;
      pf8bit: Result := 8;
      pf15bit: Result := 16;
      pf16bit: Result := 16;
      pf24bit: Result := 24;
      pf32bit: Result := 32;
      else Result := 0;
    end;
end;

function TPakImages.IsNewSDFormat(const ImageHead:TNewPakImageInfo; nDecompressedSize:Integer):Boolean;
var
    nRealSize:Integer;
begin
    Result := False;
    if (ImageHead.PixelFormat = pf16bit) and (not ImageHead.boAlpha) then begin
        nRealSize := ImageHead.nWidth * ImageHead.nHeight * 2; //WideBytes(pf16bit) = 2;
        if nRealSize <> nDecompressedSize then begin
            Result := True;
        end;
    end;
end;

function TPakImages.GetNewPakImageDataSize(const ImageHead:TNewPakImageInfo):Integer;
begin
    Result := 0;
    if(ImageHead.nWidth < 0) or (ImageHead.nHeight < 0) or (ImageHead.Length < 0) then begin
        Exit;  //有问题的数据
    end;

    if(ImageHead.Length > 0) then begin
        Result := ImageHead.Length;
    end else begin
        case ImageHead.PixelFormat of
          pf8bit: Result := WidthBytes(8, ImageHead.nWidth) * ImageHead.nHeight;
          pf15bit, pf16bit: Result := WidthBytes(16, ImageHead.nWidth) * ImageHead.nHeight;
          pf24bit: Result := WidthBytes(24, ImageHead.nWidth) * ImageHead.nHeight;
          pf32bit: Result := WidthBytes(32, ImageHead.nWidth) * ImageHead.nHeight;
          else Result := 0;
        end;

        if Result > 0 then begin
            if ImageHead.boAlpha then begin//透明图则要加上alpha数据块大小
                Result := Result + (WidthBytes(8, ImageHead.nWidth) * ImageHead.nHeight);
            end;
            Result := Result;// + SizeOf(TLzPakImageHeadVer1);
        end;
    end;
end;

function TPakImages.GetAlphaDibFromNewFormat(nImgWidth, nImgHeight:Integer; pSrcData:Pointer; nSrcSize:Integer):TDIB;
var
    nImgSize, nAlphaSize, nAlphaLineSize, nAlphaWidthBytes:Integer;
    x, y :Integer;
    pAlpha, pSrcAlphaData:PByte;
    byAlpha:Byte;
begin
    Result := nil;
    nImgSize := nImgWidth * nImgHeight * 2; //WidthBytes(pf16bit); //只有pf16bit才会到这个流程
    nAlphaLineSize := nImgWidth div 2;
    if (nImgWidth and $01) <> 0 then begin
        Inc(nAlphaLineSize);
    end;
    nAlphaSize := nImgHeight * nAlphaLineSize;

    if nAlphaSize > (nSrcSize - nImgSize) then Exit;

    pSrcAlphaData := Pointer(UINT_PTR(pSrcData) + UINT(nImgSize));
    nAlphaWidthBytes := (((nImgWidth * 8) + 31) div 32) * 4;
    Result := TDib.Create;
    Result.SetSize(nImgWidth, nImgHeight, 8);

    pAlpha := Result.PBits;

    for y := nImgHeight - 1 downto 0 do begin
        for x := 0 to nImgWidth - 1 do begin
            byAlpha :=  PByte(INT_PTR(pSrcAlphaData) + (y * nAlphaLineSize + (x div 2)))^;
            if (x and $01) = 0 then begin
                PByte(INT_PTR(pAlpha) + x)^ := Min(255, ((byAlpha and $F0) shr 4) * 17);
            end else begin
                PByte(INT_PTR(pAlpha) + x)^ := Min(255, (byAlpha and $0F) * 17);
            end;
        end;
        Inc(pAlpha, nAlphaWidthBytes);
    end;
end;

function TPakImages.LoadLzImageDataV1(const ImageInfoV1:TNewPakImageInfo; msData:TMemoryStream; var Source, AlphaSource:TDiB):Boolean;
var
    nImageDataSize :Integer;
    pCompressedData, pUncompressedData, pSrcImageData, pAlphaData:Pointer;
    nBitCount, nWidthBytes, nUnCompressedSize:Integer;
    bHasAlphaData:Boolean;
begin
    Result := False;
    Source := nil;
    AlphaSource := nil;
    nBitCount := GetBitCountByPixelFormat(ImageInfoV1.PixelFormat);
    if (nBitCount < 8) then Exit;

    bHasAlphaData := False;
    nWidthBytes := WidthBytes(nBitCount, ImageInfoV1.nWidth);
    nImageDataSize := ImageInfoV1.nHeight * nWidthBytes;
    if ImageInfoV1.boAlpha then begin
        nUncompressedSize := nImageDataSize + (ImageInfoV1.nHeight * ImageInfoV1.nWidth);
    end else begin
        nUncompressedSize := nImageDataSize;
    end;

    if ImageInfoV1.Length > 0 then begin
       pCompressedData := msData.Memory;
       try
           try
              ZLibEx.DecompressBuf(pCompressedData, msData.Size, nUncompressedSize, pUncompressedData, nUncompressedSize);
           except
              nImageDataSize := 0;
           end;
           pSrcImageData := pUncompressedData;
           bHasAlphaData := IsNewSDFormat(ImageInfoV1, nImageDataSize);
       except
           pSrcImageData := nil;
       end;
    end else begin
       pSrcImageData := msData.Memory;
       pUncompressedData := nil;
    end;  

    if pSrcImageData <> nil then begin
        Source := MakeDibByPixelFormat(ImageInfoV1.PixelFormat, ImageInfoV1.nWidth, ImageInfoV1.nHeight);
        if Source <> nil then begin
            Move(pSrcImageData^, Source.PBits^, Source.Height * Source.WidthBytes);
            Result := True;
        end;
    end;

    if Result then begin 
        if ImageInfoV1.boAlpha then begin
           AlphaSource := MakeDibByBitCount(8, ImageInfoV1.nWidth, ImageInfoV1.nHeight);
           pAlphaData := Pointer(UINT_PTR(pSrcImageData) + UINT(nImageDataSize));
           Move(pAlphaData^, AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
        end else begin
           if bHasAlphaData then begin
               AlphaSource := GetAlphaDibFromNewFormat(ImageInfoV1.nWidth, ImageInfoV1.nHeight, pSrcImageData, nUnCompressedSize);
           end;
        end;
    end;
    if pUncompressedData <> nil then FreeMemory(pUncompressedData);  
end;

procedure TPakImages.LoadDxImageLzPak(nIndex, nImgOffset, nImgDataSize:Integer; dtsStyle:TDxTextureStyle; DXImage:pTDxImage);
var
  ImageInfoV0:TPakImageInfo;
  ImageInfoV1:TNewPakImageInfo;
  Source:TDIB;
  AlphaSource:TDIB;
  boError:Boolean;
  msData:TMemoryStream;
  nW, nH, nPx, nPy:Integer;
begin
  boError := True;
  Source := nil;
  AlphaSource := nil;
  nW := 0; nH := 0; nPx := 0; nPy := 0;

  if FPakFileType = pftLzPakV0 then begin
      if (nImgOffset > LZ_PAK_FILE_HEADER_SIZE) and (nImgOffset + SizeOf(ImageInfoV0) <= m_FileStream.Size) then begin
          LockFileStream;
          try
               m_FileStream.Position := nImgOffset;
               ReadImageHeader(ImageInfoV0, SizeOf(ImageInfoV0));
               nImgDataSize := nImgDataSize - SizeOf(ImageInfoV0);
               if IsValidLzImageInfoV0(ImageInfoV0, nIndex, nImgDataSize, FileHeader.BitCount) then begin
                   nW := ImageInfoV0.wW;
                   nH := ImageInfoV0.wH;
                   nPx := ImageInfoV0.wPx;
                   nPy := ImageInfoV0.wPy;
                   if nW * nH > 4 then begin
                       msData := TMemoryStream.Create;
                       msData.SetSize(nImgDataSize);
                       if m_FileStream.Read(msData.Memory^, nImgDataSize) = nImgDataSize then begin
                           boError := not LoadLzImageDataV0(ImageInfoV0, msData, Source, AlphaSource);
                       end;
                       msData.Free;
                   end else begin
                       boError := False;
                   end;
               end;
          finally
              UnLockFileStream;
          end;
      end;
  end else begin
      if (nImgOffset > LZ_PAK_FILE_HEADER_SIZE) and (nImgOffset + SizeOf(ImageInfoV1) <= m_FileStream.Size) then begin
           LockFileStream;
          try
               m_FileStream.Position := nImgOffset;
               ReadImageHeader(ImageInfoV1, SizeOf(ImageInfoV1));
               if IsValidLzImageInfoV1(ImageInfoV1, nIndex) then begin
                   nW := ImageInfoV1.nWidth;
                   nH := ImageInfoV1.nHeight;
                   nPx := ImageInfoV1.px;
                   nPy := ImageInfoV1.py;
                   if nW * nH > 4 then begin
                       nImgDataSize := GetNewPakImageDataSize(ImageInfoV1);
                       if nImgDataSize > 0 then begin
                           msData := TMemoryStream.Create;
                           msData.SetSize(nImgDataSize);
                           if m_FileStream.Read(msData.Memory^, nImgDataSize) = nImgDataSize then begin
                               boError := not LoadLzImageDataV1(ImageInfoV1, msData, Source, AlphaSource);
                           end;
                           msData.Free;
                       end;
                   end else begin
                       boError := False;
                   end;
               end;
          finally
              UnLockFileStream;
          end;
      end;
  end;

    DXImage.dwLatestTime := MyGetTickCount;
    if not boError then begin
        if Source <> nil then begin
            DXImage.nWidth := nW;
            DXImage.nHeight := nH;
            DXImage.nPx := nPx;
            DXImage.nPy := nPy;

            if dtsStyle = dtsNormal then begin
                DXImage.Surface := NewTexture(Source, AlphaSource);
            end else if dtsStyle = dtsGray then begin
                DXImage.Gray := NewTextureGray(Source, AlphaSource);
            end else if dtsBright = dtsBright then begin
                DXImage.Bright := NewTextureBright(Source, AlphaSource);
            end;

        end else begin
            DXImage.nWidth := 1;
            DXImage.nHeight := 1;
            DXImage.nPx := nPx;
            DXImage.nPy := nPy;
            DXImage.Surface := NULLTexture;
        end;
    end else begin
        DXImage.nWidth := 0;
        DXImage.nHeight := 0;
        DXImage.nPx := 0;
        DXImage.nPy := 0;
        DXImage.Surface := nil;
    end;

    if Source <> nil then Source.Free;
    if AlphaSource <> nil then AlphaSource.Free;

end;

//HZQ 20230712 LZ的PAK另开格式了，此处没有使用
procedure TPakImages.LoadDxImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  ImageInfo:TNewPakImageInfo;
  Source:TDIB;
  nSize:Integer;
  InData:Pointer;
  OutData:Pointer;
  OutSize:LongInt;
  AlphaData:PByte;
  AlphaSource:TDIB;

  boError:Boolean;
begin
  boError := False;

  if (Position >= SizeOf(TPakFileHeader)) and (Position + SizeOf(TNewPakImageInfo) <= m_FileStream.Size) then begin
    if FPakFileType = pftPak1 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end else if FPakFileType = pftPak2 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak2(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end else if FPakFileType = pftPak3 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak3(Index, ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end;

    if not (ImageInfo.PixelFormat in [pf8bit, pf15bit, pf16bit, pf24bit, pf32bit]) then begin
      boError := True;
    end;

    {
    if (not boError) and ((ImageInfo.nWidth > 2048) or (ImageInfo.nHeight > 2048)) then
    begin
      OutMessage('[Exception] TPakImages::LoadDxImage Size Error (' + Format('File:%s; Index: %d; Width: %d; Height: %d)', [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;
    }

    if (not boError) and ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      //OutMessage('[Exception] TPakImages::LoadDxImage Position Error (' + Format('File:%s; Index: %d; X: %d; Y: %d)', [FileName, Index, ImageInfo.px, ImageInfo.py]));
      OutMessage(Format(DecodeResStr(SPakLoadDxImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));
      boError := True;
    end;

    if not (boError) and (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageLenErr), [FileName, Index, ImageInfo.Length]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    DXImage.dwLatestTime := MyGetTickCount;
    if (not boError) and (ImageInfo.nWidth * ImageInfo.nHeight <= 4) then begin
      DXImage.nWidth := 1;
      DXImage.nHeight := 1;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;

      Exit;
    end;

    // 当图片出现错误时，让其重新更新 2020-05-29
    if boError then begin
      DXImage.nWidth := 0;
      DXImage.nHeight := 0;
      DXImage.nPx := 0;
      DXImage.nPy := 0;
      DXImage.Surface := nil;
    end else begin
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;

      //Source := nil;
      OutData := nil;
      if ImageInfo.Length > 0 then begin
        if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
          GetMem(InData, ImageInfo.Length);

          LockFileStream;
          try
            m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
            m_FileStream.Read(InData^, ImageInfo.Length);
          finally
            UnLockFileStream;
          end;

          // 优化 2020-04-13 01:55:17
          case ImageInfo.PixelFormat of
            pf8bit:nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf15bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf16bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf24bit:nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf32bit:nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
            else
              nSize := 0;
          end;

          try
            DecompressBuf(InData, ImageInfo.Length, nSize, OutData, OutSize);
          except
            if InData <> nil then FreeMem(InData);
            if OutData <> nil then FreeMem(OutData);
            InData := nil;
            OutData := nil;
            OutMessage(Format(DecodeResStr(SPakImageDecompressErr), [FileName, Index]));
            boError := True;
          end;

          if InData <> nil then FreeMem(InData);

          if (not boError) and (OutData <> nil) then begin

            Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

            if Source <> nil then begin
              // 这里搞下看能不能优化一部分 2020-04-16 02:27:24
              //Source.Canvas.Brush.Color := clblack;
              //Source.Canvas.FillRect(Source.Canvas.ClipRect);

              Move(OutData^, Source.PBits^, Source.Height * Source.WidthBytes);

              if not ImageInfo.boAlpha then begin
                DXImage.Surface := NewTexture(Source);
              end else begin
                AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight);
                AlphaData := PByte(Integer(OutData) + Source.Height * Source.WidthBytes);
                Move(AlphaData^, AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
                DXImage.Surface := NewTexture(Source, AlphaSource);
                AlphaSource.Free;
              end;

              Source.Free;

              if OutData <> nil then FreeMem(OutData);
              OutData := nil;
            end;
          end;
        end;
      end else begin
        Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

        if Source <> nil then begin
          Source.Canvas.Brush.Color := clblack;
          Source.Canvas.FillRect(Source.Canvas.ClipRect);

          if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
            LockFileStream;
            try
              m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
              m_FileStream.Read(Source.PBits^, Source.Height * Source.WidthBytes); // * (Source.BitCount div 8)
            finally
              UnLockFileStream;
            end;

            if not ImageInfo.boAlpha then begin
              DXImage.Surface := NewTexture(Source);
            end else begin
              AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight); 
              LockFileStream;
              try
                m_FileStream.Position := Position + SizeOf(TNewPakImageInfo) + Source.Height * Source.WidthBytes;
                m_FileStream.Read(AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
              finally
                UnLockFileStream;
              end;

              DXImage.Surface := NewTexture(Source, AlphaSource);
              AlphaSource.Free;
            end;

            Source.Free;
          end;
        end;
      end;
    end; // end of not boError
  end;
end;

procedure TPakImages.LoadDxGrayImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  ImageInfo:TNewPakImageInfo;
  Source:TDIB;
  InData:Pointer;
  OutData:Pointer;
  OutSize:LongInt;
  AlphaData:PByte;
  AlphaSource:TDIB;

  boError:Boolean;
  nSize:Integer;
begin
  boError := False;

  if (Position >= SizeOf(TPakFileHeader)) and (Position + SizeOf(TNewPakImageInfo) <= m_FileStream.Size) then begin
    if FPakFileType = pftPak1 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end
    else if FPakFileType = pftPak2 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak2(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end
    else if FPakFileType = pftPak3 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak3(Index, ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end;

    if not (ImageInfo.PixelFormat in [pf8bit, pf15bit, pf16bit, pf24bit, pf32bit]) then begin
      boError := True;
    end;

    {
    if (not boError) and ((ImageInfo.nWidth > 2048) or (ImageInfo.nHeight > 2048)) then
    begin
      OutMessage('[Exception] TPakImages::LoadDxGrayImage Size Error (' + Format('File:%s; Index: %d; Width: %d; Height: %d)', [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;
    }

    if (not boError) and ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      //OutMessage('[Exception] TPakImages::LoadDxGrayImage Position Error (' + Format('File:%s; Index: %d; X: %d; Y: %d)', [FileName, Index, ImageInfo.px, ImageInfo.py]));
      OutMessage(Format(DecodeResStr(SPakLoadDxGrayImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));
      boError := True;
    end;

    if not (boError) and (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxGrayImageLenErr), [FileName, Index, ImageInfo.Length]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxGrayImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SPakLoadDxGrayImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    DXImage.dwLatestGrayTime := MyGetTickCount;
    if (not boError) and (ImageInfo.nWidth * ImageInfo.nHeight <= 4) then begin
      DXImage.nWidth := 1;
      DXImage.nHeight := 1;

      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;
      Exit;
    end;

    // 当图片出现错误时，让其重新更新 2020-05-29
    if boError then begin
      DXImage.nWidth := 0;
      DXImage.nHeight := 0;

      DXImage.nPx := 0;
      DXImage.nPy := 0;
      DXImage.Surface := nil;
    end
    else begin
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;

      //Source := nil;
      OutData := nil;
      if ImageInfo.Length > 0 then begin
        if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
          GetMem(InData, ImageInfo.Length);

          LockFileStream;
          try
            m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
            m_FileStream.Read(InData^, ImageInfo.Length);
          finally
            UnLockFileStream;
          end;

          // 优化 2020-04-13 01:55:17
          case ImageInfo.PixelFormat of
            pf8bit:nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf15bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf16bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf24bit:nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf32bit:nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
            else
              nSize := 0;
          end;

          try
            DecompressBuf(InData, ImageInfo.Length, nSize, OutData, OutSize);
          except
            if InData <> nil then FreeMem(InData);
            if OutData <> nil then FreeMem(OutData);
            InData := nil;
            OutData := nil;
            OutMessage(Format(DecodeResStr(SPakGrayImageDecompErr), [FileName, Index]));
            boError := True;
          end;

          if (not boError) and (OutData <> nil) then begin
            Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

            if Source <> nil then begin
              Source.Canvas.Brush.Color := clblack;
              Source.Canvas.FillRect(Source.Canvas.ClipRect);
              Move(OutData^, Source.PBits^, Source.Height * Source.WidthBytes);

              if not ImageInfo.boAlpha then begin
                DXImage.Gray := NewTextureGray(Source);
              end else begin
                AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight);
                AlphaData := PByte(Integer(OutData) + Source.Height * Source.WidthBytes);
                Move(AlphaData^, AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
                DXImage.Gray := NewTextureGray(Source, AlphaSource);
                AlphaSource.Free;
              end;

              Source.Free;

              if InData <> nil then FreeMem(InData);
              if OutData <> nil then FreeMem(OutData);
              OutData := nil;
            end;
          end;
        end;
      end else begin
        Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);
        if Source <> nil then begin
          Source.Canvas.Brush.Color := clblack;
          Source.Canvas.FillRect(Source.Canvas.ClipRect);

          if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
            LockFileStream;
            try
              m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
              m_FileStream.Read(Source.PBits^, Source.Height * Source.WidthBytes); // * (Source.BitCount div 8)
            finally
              UnLockFileStream;
            end;

            if not ImageInfo.boAlpha then begin
              DXImage.Gray := NewTextureGray(Source);
            end else begin
              AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight);
              LockFileStream;
              try
                m_FileStream.Position := Position + SizeOf(TNewPakImageInfo) + Source.Height * Source.WidthBytes;
                m_FileStream.Read(AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
              finally
                UnLockFileStream;
              end;

              DXImage.Gray := NewTextureGray(Source, AlphaSource);
              AlphaSource.Free;
            end;

            Source.Free;
          end;
        end;
      end;
    end; // end of not boError
  end;
end;

procedure TPakImages.LoadDxBrightImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  ImageInfo:TNewPakImageInfo;
  Source:TDIB;
  InData:Pointer;
  OutData:Pointer;
  OutSize:LongInt;
  AlphaData:PByte;
  AlphaSource:TDIB;

  boError:Boolean;

  nSize:Integer;
begin
  boError := False;

  if (Position >= SizeOf(TPakFileHeader)) and (Position + SizeOf(TNewPakImageInfo) <= m_FileStream.Size) then begin
    if FPakFileType = pftPak1 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end else if FPakFileType = pftPak2 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak2(ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end else if FPakFileType = pftPak3 then begin
      LockFileStream;
      try
        m_FileStream.Position := Position;
        ReadImageHeader_Pak3(Index, ImageInfo, SizeOf(TNewPakImageInfo));
      finally
        UnLockFileStream;
      end;
    end;

    if not (ImageInfo.PixelFormat in [pf8bit, pf15bit, pf16bit, pf24bit, pf32bit]) then begin
      boError := True;
    end;

    {
    if (not boError) and ((ImageInfo.nWidth > 2048) or (ImageInfo.nHeight > 2048)) then
    begin
      OutMessage('[Exception] TPakImages.LoadDxBrightImage Size Error (' + Format('File:%s; Index: %d; Width: %d; Height: %d)', [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;
    }

    if (not boError) and ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      //OutMessage('[Exception] TPakImages::LoadDxBrightImage Position Error (' + Format('File:%s; Index: %d; X: %d; Y: %d)', [FileName, Index, ImageInfo.px, ImageInfo.py]));
      OutMessage(Format(DecodeResStr(SPakBrightImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));
      boError := True;
    end;

    if not (boError) and (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SPakBrightImageLenErr), [FileName, Index, ImageInfo.Length]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SPakBrightImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    if not (boError) and ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SPakBrightImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));
      boError := True;
    end;

    DXImage.dwLatestBrightTime := MyGetTickCount;
    if (not boError) and (ImageInfo.nWidth * ImageInfo.nHeight <= 4) then begin
      DXImage.nWidth := 1;
      DXImage.nHeight := 1;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;
      Exit;
    end;

    // 当图片出现错误时，让其重新更新 2020-05-29
    if boError then begin
      DXImage.nWidth := 0;
      DXImage.nHeight := 0;
      DXImage.nPx := 0;
      DXImage.nPy := 0;
      DXImage.Surface := nil;
    end
    else begin
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;

      //Source := nil;
      OutData := nil;
      if ImageInfo.Length > 0 then begin
        if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
          GetMem(InData, ImageInfo.Length);

          LockFileStream;
          try
            m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
            m_FileStream.Read(InData^, ImageInfo.Length);
          finally
            UnLockFileStream;
          end;

          // 优化 2020-04-13 01:55:17
          case ImageInfo.PixelFormat of
            pf8bit:nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf15bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf16bit:nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf24bit:nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
            pf32bit:nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
            else
              nSize := 0;
          end;

          try
            DecompressBuf(InData, ImageInfo.Length, nSize, OutData, OutSize);
          except
            if InData <> nil then FreeMem(InData);
            if OutData <> nil then FreeMem(OutData);
            OutMessage(Format(DecodeResStr(SPakBrightImageDecompErr), [FileName, Index]));
            boError := True;
          end;

          if (not boError) and (OutData <> nil) then begin
            Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);
            if Source <> nil then begin
              Source.Canvas.Brush.Color := clblack;
              Source.Canvas.FillRect(Source.Canvas.ClipRect);
              Move(OutData^, Source.PBits^, Source.Height * Source.WidthBytes);

              if not ImageInfo.boAlpha then begin
                DXImage.Bright := NewTextureBright(Source);
              end else begin
                AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight);
                AlphaData := PByte(Integer(OutData) + Source.Height * Source.WidthBytes);
                Move(AlphaData^, AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
                DXImage.Bright := NewTextureBright(Source, AlphaSource);
                AlphaSource.Free;
              end;

              Source.Free;

              if InData <> nil then FreeMem(InData);
              if OutData <> nil then FreeMem(OutData);
              OutData := nil;
            end;
          end;
        end;
      end else begin
        Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);
        if Source <> nil then begin
          Source.Canvas.Brush.Color := clblack;
          Source.Canvas.FillRect(Source.Canvas.ClipRect);

          if FPakFileType in [pftPak1, pftPak2, pftPak3] then begin
            LockFileStream;
            try
              m_FileStream.Position := Position + SizeOf(TNewPakImageInfo);
              m_FileStream.Read(Source.PBits^, Source.Height * Source.WidthBytes); // * (Source.BitCount div 8)
            finally
              UnLockFileStream;
            end;

            if not ImageInfo.boAlpha then begin
              DXImage.Bright := NewTextureBright(Source);
            end else begin
              AlphaSource := MakeDibByBitCount(8, ImageInfo.nWidth, ImageInfo.nHeight);
              LockFileStream;
              try
                m_FileStream.Position := Position + SizeOf(TNewPakImageInfo) + Source.Height * Source.WidthBytes;
                m_FileStream.Read(AlphaSource.PBits^, AlphaSource.Height * AlphaSource.WidthBytes);
              finally
                UnLockFileStream;
              end;

              DXImage.Bright := NewTextureBright(Source, AlphaSource);
              AlphaSource.Free;
            end;

            Source.Free;
          end;
        end;
      end;
    end; // end of boError
  end;
end;

procedure TPakImages.LockFileStream;
begin
  EnterCriticalSection(FCSFileStream);
end;

procedure TPakImages.UnLockFileStream;
begin
  LeaveCriticalSection(FCSFileStream);
end;


end.
