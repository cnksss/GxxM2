unit HUtil32;

// ============================================
// Latest Update date : 1998 1
// Add/Update Function and procedure :
// 		CaptureString
// Str_PCopy          	(4/29)
// 			Str_PCopyEx			 	(5/2)
// 			memset					(6/3)
// SpliteBitmap         (9/3)
// ArrestString         (10/27)  {name changed}
// IsStringNumber       (98'1/1)
// 			GetDirList				(98'12/9)
// GetFileDate          (98'12/9)
// CatchString          (99'2/4)
// DivString            (99'2/4)
// DivTailString        (99'2/4)
// SPos                 (99'2/9)
// ============================================


interface

{$WARNINGS OFF 1002}

uses
  Classes, SysUtils, StrUtils, WinTypes, WinProcs, Graphics, Messages, Dialogs
  {$IF CompilerVersion >= 22},System.UITypes{$IFEND};

type
  Str4096 = array[0..4096] of Char;
  Str256 = array[0..256] of Char;
  TyNameTable = record
    Name: string;
    varl: LongInt;
  end;

  TLRect = record
    Left, Top, Right, Bottom: LongInt;
  end;

const
  MAXDEFCOLOR = 16;
  ColorNames: array[1..MAXDEFCOLOR] of TyNameTable = (
    (Name: 'BLACK'; varl: clBlack),
    (Name: 'BROWN'; varl: clMaroon),
    (Name: 'MARGENTA'; varl: clFuchsia),
    (Name: 'GREEN'; varl: clGreen),
    (Name: 'LTGREEN'; varl: clOlive),
    (Name: 'BLUE'; varl: clNavy),
    (Name: 'LTBLUE'; varl: clBlue),
    (Name: 'PURPLE'; varl: clPurple),
    (Name: 'CYAN'; varl: clTeal),
    (Name: 'LTCYAN'; varl: clAqua),
    (Name: 'GRAY'; varl: clGray),
    (Name: 'LTGRAY'; varl: clSilver),
    (Name: 'YELLOW'; varl: clyellow),
    (Name: 'LIME'; varl: clLime),
    (Name: 'WHITE'; varl: clWhite),
    (Name: 'RED'; varl: clRed)
    );

  MAXLISTMARKER = 3;
  LiMarkerNames: array[1..MAXLISTMARKER] of TyNameTable = (
    (Name: 'DISC'; varl: 0),
    (Name: 'CIRCLE'; varl: 1),
    (Name: 'SQUARE'; varl: 2)
    );

  MAXPREDEFINE = 3;
  PreDefineNames: array[1..MAXPREDEFINE] of TyNameTable = (
    (Name: 'LEFT'; varl: 0),
    (Name: 'RIGHT'; varl: 1),
    (Name: 'CENTER'; varl: 2)
    );

var
  PosArray: array of Integer;


function CountGarbage(paper: TCanvas; Src: PChar; TargWidth: LongInt): Integer; {garbage}
{[ArrestString]
      Result = Remain string,
      RsltStr = captured string
}
function ArrestString(Source, SearchAfter, ArrestBefore: string;
  const DropTags: array of string; var RsltStr: string): string;
{*}
function ArrestVariable(Source: string; Left, Center, Right: Char; StartPos: Integer; var ArrestStr: string): Integer;
function ArrestStringEx2(Source: string; Left, Center, Right: Char; var ArrestStr: string): Integer;
function ArrestStringEx(const Source: WideString; const SearchStart, SearchEnd: WideChar; var ArrestStr: string): WideString;

function ArrestStringEx_Ansi(const Source: string; const SearchStart, SearchEnd: Char; var ArrestStr: string): string;

function CaptureString(Source: string; var rdstr: string): string;
procedure ClearWindow(aCanvas: TCanvas; aLeft, aTop, aRight, aBottom: LongInt; aColor: TColor);
function CombineDirFile(SrcDir, TargName: string): string;
{*}
function CompareLStr(Src, targ: string; compn: Integer): Boolean;
function CompareBackLStr(Src, targ: string; compn: Integer): Boolean;
function CompareBuffer(p1, P2: PByte; len: Integer): Boolean;
function CreateMask(Src: PChar; TargPos: Integer): string;
procedure DrawTileImage(Canv: TCanvas; Rect: TRect; TileImage: TBitmap);
procedure DrawingGhost(rc: TRect);

function FloatToString(F: real): string;
function FloatToStrFixFmt(fVal: Double; prec, digit: Integer): string;
function FileSize(const fname: string): LongInt;
{*}
function FileCopy(Source, Dest: string): Boolean;
function FileCopyEx(Source, Dest: string): Boolean;
function GetSpaceCount(Str: string): LongInt;
function RemoveSpace(Str: string): string;
function GetFirstWord(Str: string; var sWord: string; var FrontSpace: LongInt): string;
function GetDefColorByName(Str: string): TColor;
function GetULMarkerType(Str: string): LongInt;
{*}
function GetValidStr3(Str: string; var Dest: string; const Divider: array of Char): string;
function GetValidStr3_Ex(Str: string; var Dest: string; const Divider: Char): string;

function GetValidStrCap(Str: string; var Dest: string; const Divider: array of Char): string;
function GetStrToCoords(Str: string): TRect;
function GetDefines(Str: string): LongInt;
function GetValueFromMask(Src: PChar; Mask: string): string;
procedure GetDirList(Path: string; fllist: TStringList);
function GetFileDate(FileName: string): Integer; // DOS format file date..
function HexToIntEx(shap_str: string): LongInt;
function HexToInt(Str: string): LongInt;
function IntToStr2(n: Integer): string;
function IntToStrFill(num, len: Integer; Fill: Char): string;
function IsInB(Src: string; Pos: Integer; targ: string): Boolean;
function IsInRect(x, y: Integer; Rect: TRect): Boolean;
function IsEnglish(Ch: Char): Boolean;
function IsEngNumeric(Ch: Char): Boolean;
function IsFloatNumeric(Str: string): Boolean;
function IsUniformStr(Src: string; Ch: Char): Boolean;
function IsStringNumber(Str: string): Boolean;
function IsNumber(Str: string): Boolean;
function KillFirstSpace(var Str: string): LongInt;
procedure KillGabageSpace(var Str: string);
function LRect(l, t, r, b: LongInt): TLRect;
procedure MemPCopy(Dest: PChar; Src: string);
procedure MemCpy(Dest, Src: PChar; Count: LongInt); {PChar type}
procedure memcpy2(TargAddr, SrcAddr: LongInt; Count: Integer); {Longint type}
procedure memset(Buffer: PChar; FillChar: Char; Count: Integer);
procedure PCharSet(P: PChar; n: Integer; Ch: Char);
function ReplaceChar(Src: string; srcchr, repchr: Char): string;
function Str_ToDate(Str: string): TDateTime;
function Str_ToTime(Str: string): TDateTime;
function Str_ToInt(Str: string; Def: LongInt): LongInt;
function Str_ToFloat(Str: string): real;
function SkipStr(Src: string; const Skips: array of Char): string;
procedure ShlStr(Source: PChar; Count: Integer);
procedure ShrStr(Source: PChar; Count: Integer);
procedure Str256PCopy(Dest: PChar; const Src: string);
function _StrPas(Dest: PChar): string;
function Str_PCopy(Dest: PChar; Src: string): Integer;
function Str_PCopyEx(Dest: PChar; const Src: string; buflen: LongInt): Integer;
procedure SpliteBitmap(DC: hdc; x, y: Integer; Bitmap: TBitmap; transcolor: TColor);
procedure TiledImage(Canv: TCanvas; Rect: TLRect; TileImage: TBitmap);
function Trim_R(const Str: string): string;
function IsEqualFont(SrcFont, TarFont: TFont): Boolean;
function CutHalfCode(Str: string): string;
function ConvertToShortName(Canvas: TCanvas; Source: string; WantWidth: Integer): string;
{*}
function CatchString(Source: string; cap: Char; var catched: string): string;
function DivString(Source: string; cap: Char; var sel: string): string;
function DivTailString(Source: string; cap: Char; var sel: string): string;
function SPos(substr, Str: string): Integer;
function NumCopy(Str: string): Integer;
function GetMonDay: string;
function BoolToStr(boo: Boolean): string;
function BoolToStr2(boo: Boolean): string;
function StrToBool(Str: string): Boolean;

function TagCount(Source: WideString; Tag: WideChar): Integer;

function _MIN(N1, N2: Integer): Integer;
function _MAX(N1, N2: Integer): Integer;
function _MinLong(N1, N2: LongWord): LongWord;
function _MaxLong(N1, N2: LongWord): LongWord;

function IsIPaddr(Ip: string): Boolean;
function IntToSex(btSex: Byte): string;
function IntToJob(btJob: Byte): string;
function GetCodeMsgSize(x: Double): Integer;
function GetDayCount(MaxDate, MinDate: TDateTime): Integer;
function BoolToIntStr(boBoolean: Boolean): string;
function BoolToCStr(boBoolean: Boolean): string;
function BooleanToStr(boo: Boolean): string;
function BoolToInt(boBoolean: Boolean): Integer;

function MakeHumanFeature(btRaceImg, btDress, btWeapon, btHair: Byte): Integer;
function MakeMonsterFeature(btRaceImg, btWeapon: Byte; wAppr: Word): Integer;

function IsVarNumber(Str: string): Boolean;

procedure DisPoseAndNil(var Obj);
function InString(sData: string): PChar;
function OutString(Data: PChar): string;

procedure SafeFillChar(out x; Count: Integer; V: Char); OVERLOAD;
procedure SafeFillChar(out x; Count: Integer; V: Byte); OVERLOAD;

function TrimEx(const S: string): string; OVERLOAD;
function TrimEx(const S: WideString): WideString; OVERLOAD;
function TrimLeftEx(const S: string): string; OVERLOAD;
function TrimLeftEx(const S: WideString): WideString; OVERLOAD;
function TrimRightEx(const S: string): string; OVERLOAD;
function TrimRightEx(const S: WideString): WideString; OVERLOAD;

function ColorIndexToTColor(ColorIndex: Byte): TColor;

function sub_49ADB8(nPos: Integer; sMsg, sStr, sText: string): string;
implementation
const
  TextChars = [#32..#255];

function ColorIndexToTColor(ColorIndex: Byte): TColor;
const
  ColorArray: array[0..1023] of Byte = (
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
var
  ColorTable: PRGBQuad;
begin
  ColorTable := @ColorArray[0];
  Inc(ColorTable, ColorIndex);
  Result := RGB(ColorTable.rgbRed, ColorTable.rgbGreen, ColorTable.rgbBlue);
end;

function sub_49ADB8(nPos: Integer; sMsg, sStr, sText: string): string;                     // 0049ADB8
var
  n10: Integer;
  s14, s18: string;
begin
  if nPos > 0 then
  begin
    s14 := Copy(sMsg, 1, nPos - 1);
    s18 := Copy(sMsg, Length(sStr) + nPos, Length(sMsg));
    Result := s14 + sText + s18;
  end
  else
  begin
    n10 := Pos(sStr, sMsg);
    if n10 > 0 then
    begin
      s14 := Copy(sMsg, 1, n10 - 1);
      s18 := Copy(sMsg, Length(sStr) + n10, Length(sMsg));
      Result := s14 + sText + s18;
    end
    else
      Result := sMsg;
  end;
end;

function TrimEx(const S: string): string;
var
  I, L: Integer;
begin
  L := Length(S);
  I := 1;

{$IF CompilerVersion >= 22}
  while (I <= L) and ((S[I] <= ' ') or (not CharInSet(S[I], TextChars))) do
    Inc(I);
  if I > L then
    Result := ''
  else
  begin
    while ((S[L] <= ' ') or (not CharInSet(S[L], TextChars))) do
      Dec(L);
    Result := Copy(S, I, L - I + 1);
  end;
{$ELSE}
  while (I <= L) and ((S[I] <= ' ') or (not (S[I] in TextChars))) do
    Inc(I);
  if I > L then
    Result := ''
  else
  begin
    while ((S[L] <= ' ') or (not (S[L] in TextChars))) do
      Dec(L);
    Result := Copy(S, I, L - I + 1);
  end;
{$IFEND}
end;

function TrimEx(const S: WideString): WideString;
var
  I, L: Integer;
begin
  L := Length(S);
  I := 1;
{$IF CompilerVersion >= 22}
  while (I <= L) and ((S[I] <= ' ') or (not CharInSet(string(S[I])[1], TextChars))) do
    Inc(I);
  if I > L then
    Result := ''
  else
  begin
    while ((S[L] <= ' ') or (not CharInSet(string(S[L])[1], TextChars))) do
      Dec(L);
    Result := Copy(S, I, L - I + 1);
  end;
{$ELSE}
  while (I <= L) and ((S[I] <= ' ') or (not (string(S[I])[1] in TextChars))) do
    Inc(I);
  if I > L then
    Result := ''
  else
  begin
    while ((S[L] <= ' ') or (not (string(S[L])[1] in TextChars))) do
      Dec(L);
    Result := Copy(S, I, L - I + 1);
  end;
{$IFEND}
end;

function TrimLeftEx(const S: string): string;
var
  I, L: Integer;
begin
  L := Length(S);
  I := 1;
{$IF CompilerVersion >= 22}
  while (I <= L) and ((S[I] <= ' ') or (not CharInSet(S[I], TextChars))) do
    Inc(I);
  Result := Copy(S, I, Maxint);
{$ELSE}
  while (I <= L) and ((S[I] <= ' ') or (not (S[I] in TextChars))) do
    Inc(I);
  Result := Copy(S, I, Maxint);
{$IFEND}
end;

function TrimLeftEx(const S: WideString): WideString;
var
  I, L: Integer;
begin
  L := Length(S);
  I := 1;
{$IF CompilerVersion >= 22}
  while (I <= L) and ((S[I] <= ' ') or (not CharInSet(string(S[I])[1], TextChars))) do
    Inc(I);
  Result := Copy(S, I, Maxint);
{$ELSE}
  while (I <= L) and ((S[I] <= ' ') or (not (string(S[I])[1] in TextChars))) do
    Inc(I);
  Result := Copy(S, I, Maxint);
{$IFEND}
end;

function TrimRightEx(const S: string): string;
var
  I: Integer;
begin
  I := Length(S);
{$IF CompilerVersion >= 22}
  while (I > 0) and ((S[I] <= ' ') or (not CharInSet(S[I], TextChars))) do
    Dec(I);
  Result := Copy(S, 1, I);
{$ELSE}
  while (I > 0) and ((S[I] <= ' ') or (not (S[I] in TextChars))) do
    Dec(I);
  Result := Copy(S, 1, I);
{$IFEND}
end;

function TrimRightEx(const S: WideString): WideString;
var
  I: Integer;
begin
  I := Length(S);
{$IF CompilerVersion >= 22}
  while (I > 0) and ((S[I] <= ' ') or (not CharInSet(string(S[I])[1], TextChars))) do
    Dec(I);
  Result := Copy(S, 1, I);
{$ELSE}
  while (I > 0) and ((S[I] <= ' ') or (not (string(S[I])[1] in TextChars))) do
    Dec(I);
  Result := Copy(S, 1, I);
{$IFEND}
end;

function StrToBool(Str: string): Boolean;
begin
  Result := Boolean(Str_ToInt(Str, 0));
end;

procedure SafeFillChar(out x; Count: Integer; V: Char);
begin
  FillChar(x, Count, V);
end;

procedure SafeFillChar(out x; Count: Integer; V: Byte);
begin
  FillChar(x, Count, V);
end;

function InString(sData: string): PChar;
var
  nLength: Integer;
begin
  nLength := Length(sData);
  GetMem(Result, nLength + SizeOf(Integer) + 1);
  Move(nLength, Result^, SizeOf(Integer));
  Move(sData[1], Result[SizeOf(Integer)], nLength + 1);
end;

function OutString(Data: PChar): string;
var
  nLength: Integer;
begin
  Move(Data^, nLength, SizeOf(Integer));
  SetLength(Result, nLength - 1);
  Move(Data[SizeOf(Integer)], Result[1], nLength - 1);
  FreeMem(Data);
end;

function MakeHumanFeature(btRaceImg, btDress, btWeapon, btHair: Byte): Integer;
begin
  Result := MakeLong(MakeWord(btRaceImg, btWeapon), MakeWord(btHair, btDress));
end;

function MakeMonsterFeature(btRaceImg, btWeapon: Byte; wAppr: Word): Integer;
begin
  Result := MakeLong(MakeWord(btRaceImg, btWeapon), wAppr);
end;


function BoolToInt(boBoolean: Boolean): Integer;
begin
  if boBoolean then
    Result := 1
  else
    Result := 0;
end;

function BoolToIntStr(boBoolean: Boolean): string;
begin
  Result := IntToStr(Integer(boBoolean));
end;

function BoolToCStr(boBoolean: Boolean): string;
begin
  begin
    if boBoolean then
      Result := '是'
    else
      Result := '否';
  end;
end;

function GetDayCount(MaxDate, MinDate: TDateTime): Integer;
begin
  Result := _MAX(Trunc(MaxDate) - Trunc(MinDate), 0);
end;

function GetCodeMsgSize(x: Double): Integer;
begin
  if Int(x) < x then
    Result := Trunc(x) + 1
  else
    Result := Trunc(x)
end;

function IntToSex(btSex: Byte): string;
begin
  case btSex of
    0: Result := '男';
    1: Result := '女';
  else
    Result := '未知';
  end;
end;

function IntToJob(btJob: Byte): string;
begin
  case btJob of
    0: Result := '战士';
    1: Result := '法师';
    2: Result := '道士';
  else
    Result := '未知';
  end;
end;

function IsIPaddr(Ip: string): Boolean;
var
  Node: array[0..3] of Integer;
  tIP: string;
  tNode: string;
  tPos: Integer;
  tLen: Integer;
begin
  Result := False;
  if Ip = '' then
    Exit;
  tIP := Ip;
  tLen := Length(tIP);
  tPos := Pos('.', tIP);
  tNode := MidStr(tIP, 1, tPos - 1);
  tIP := MidStr(tIP, tPos + 1, tLen - tPos);
  if not TryStrToInt(tNode, Node[0]) then
    Exit;

  tLen := Length(tIP);
  tPos := Pos('.', tIP);
  tNode := MidStr(tIP, 1, tPos - 1);
  tIP := MidStr(tIP, tPos + 1, tLen - tPos);
  if not TryStrToInt(tNode, Node[1]) then
    Exit;

  tLen := Length(tIP);
  tPos := Pos('.', tIP);
  tNode := MidStr(tIP, 1, tPos - 1);
  tIP := MidStr(tIP, tPos + 1, tLen - tPos);
  if not TryStrToInt(tNode, Node[2]) then
    Exit;

  if not TryStrToInt(tIP, Node[3]) then
    Exit;
  for tLen := Low(Node) to High(Node) do
  begin
    if (Node[tLen] < 0) or (Node[tLen] > 255) then
      Exit;
  end;
  Result := True;
end;

function CaptureString(Source: string; var rdstr: string): string;
var
  st, et, c, len, I: Integer;
begin
  if Source = '' then
  begin
    rdstr := ''; Result := '';
    Exit;
  end;
  c := 1;
  // et := 0;
  len := Length(Source);
  while Source[c] = ' ' do
    if c < len then
      Inc(c)
    else
      Break;

  if (Source[c] = '"') and (c < len) then
  begin

    st := c + 1;
    et := len;
    for I := c + 1 to len do
      if Source[I] = '"' then
      begin
        et := I - 1;
        Break;
      end;

  end
  else
  begin
    st := c;
    et := len;
    for I := c to len do
      if Source[I] = ' ' then
      begin
        et := I - 1;
        Break;
      end;

  end;

  rdstr := Copy(Source, st, (et - st + 1));
  if len >= (et + 2) then
    Result := Copy(Source, et + 2, len - (et + 1))
  else
    Result := '';

end;


function CountUglyWhiteChar(sptr: PChar): LongInt;
var
  Cnt, Killw: LongInt;
begin
  Killw := 0;
  for Cnt := (StrLen(sptr) - 1) downto 0 do
  begin
    if sptr[Cnt] = ' ' then
    begin
      Inc(Killw);
      {sPtr[Cnt] := #0;}
    end
    else
      Break;
  end;
  Result := Killw;
end;


function CountGarbage(paper: TCanvas; Src: PChar; TargWidth: LongInt): Integer; {garbage}
var
  gab, destWidth: Integer;
begin

  gab := CountUglyWhiteChar(Src);
  destWidth := paper.TextWidth(StrPas(Src)) - gab;
  Result := TargWidth - destWidth + (gab * paper.TextWidth(' '));

end;


function GetSpaceCount(Str: string): LongInt;
var
  Cnt, len, SpaceCount: LongInt;
begin
  SpaceCount := 0;
  len := Length(Str);
  for Cnt := 1 to len do
    if Str[Cnt] = ' ' then
      SpaceCount := SpaceCount + 1;
  Result := SpaceCount;
end;

function RemoveSpace(Str: string): string;
var
  I: Integer;
begin
  Result := '';
  for I := 1 to Length(Str) do
    if Str[I] <> ' ' then
      Result := Result + Str[I];
end;

function KillFirstSpace(var Str: string): LongInt;
var
  Cnt, len: LongInt;
begin
  Result := 0;
  len := Length(Str);
  for Cnt := 1 to len do
    if Str[Cnt] <> ' ' then
    begin
      Str := Copy(Str, Cnt, len - Cnt + 1);
      Result := Cnt - 1;
      Break;
    end;
end;

procedure KillGabageSpace(var Str: string);
var
  Cnt, len: LongInt;
begin
  len := Length(Str);
  for Cnt := len downto 1 do
    if Str[Cnt] <> ' ' then
    begin
      Str := Copy(Str, 1, Cnt);
      KillFirstSpace(Str);
      Break;
    end;
end;

function GetFirstWord(Str: string; var sWord: string; var FrontSpace: LongInt): string;
var
  Cnt, len, n: LongInt;
  DestBuf: Str4096;
begin
  len := Length(Str);
  if len <= 0 then
    Result := ''
  else
  begin
    FrontSpace := 0;
    for Cnt := 1 to len do
    begin
      if Str[Cnt] = ' ' then
        Inc(FrontSpace)
      else
        Break;
    end;
    n := 0;
    for Cnt := Cnt to len do
    begin
      if Str[Cnt] <> ' ' then
        DestBuf[n] := Str[Cnt]
      else
      begin
        DestBuf[n] := #0;
        sWord := StrPas(DestBuf);
        Result := Copy(Str, Cnt, len - Cnt + 1);
        Exit;
      end;
      Inc(n);
    end;
    DestBuf[n] := #0;
    sWord := StrPas(DestBuf);
    Result := '';
  end;
end;

function HexToIntEx(shap_str: string): LongInt;
begin
  Result := HexToInt(Copy(shap_str, 2, Length(shap_str) - 1));
end;

function HexToInt(Str: string): LongInt;
var
  digit: Char;
  Count, I: Integer;
  cur, Val: LongInt;
begin
  Val := 0;
  Count := Length(Str);
  for I := 1 to Count do
  begin
    digit := Str[I];
    if (digit >= '0') and (digit <= '9') then
      cur := Ord(digit) - Ord('0')
    else if (digit >= 'A') and (digit <= 'F') then
      cur := Ord(digit) - Ord('A') + 10
    else if (digit >= 'a') and (digit <= 'f') then
      cur := Ord(digit) - Ord('a') + 10
    else
      cur := 0;
    Val := Val + (cur shl (4 * (Count - I)));
  end;
  Result := Val;
  // Result := (Val and $0000FF00) or ((Val shl 16) and $00FF0000) or ((Val shr 16) and $000000FF);
end;

function Str_ToInt(Str: string; Def: LongInt): LongInt;
begin
  Result := Def;
  if Str <> '' then
  begin
    if ((Word(Str[1]) >= Word('0')) and (Word(Str[1]) <= Word('9'))) or
      (Str[1] = '+') or (Str[1] = '-') then
    try
      Result := StrToInt64(Str);
    except
    end;
  end;
end;

function Str_ToDate(Str: string): TDateTime;
begin
  if Trim(Str) = '' then
    Result := Date
  else
    Result := StrToDate(Str);
end;

function Str_ToTime(Str: string): TDateTime;
begin
  if Trim(Str) = '' then
    Result := Time
  else
    Result := StrToTime(Str);
end;

function Str_ToFloat(Str: string): real;
begin
  if Str <> '' then
  try
    Result := StrToFloat(Str);
    Exit;
  except
  end;
  Result := 0;
end;

procedure DrawingGhost(rc: TRect);
var
  DC: hdc;
begin
  DC := GetDC(0);
  DrawFocusRect(DC, rc);
  ReleaseDC(0, DC);
end;

function FloatToString(F: real): string;
begin
  Result := FloatToStrFixFmt(F, 5, 2);
end;

function FloatToStrFixFmt(fVal: Double; prec, digit: Integer): string;
var
  Cnt, Dest, len, I, J: Integer;
  fstr: string;
  buf: array[0..255] of Char;
label
  end_conv;
begin
  Cnt := 0; Dest := 0;
  fstr := FloatToStrF(fVal, ffGeneral, 15, 3);
  len := Length(fstr);
  for I := 1 to len do
  begin
    if fstr[I] = '.' then
    begin
      buf[Dest] := '.'; Inc(Dest);
      Cnt := 0;
      for J := I + 1 to len do
      begin
        if Cnt < digit then
        begin
          buf[Dest] := fstr[J]; Inc(Dest);
        end
        else
        begin
          goto end_conv;
        end;
        Inc(Cnt);
      end;
      goto end_conv;
    end;
    if Cnt < prec then
    begin
      buf[Dest] := fstr[I]; Inc(Dest);
    end;
    Inc(Cnt);
  end;
  end_conv:
  buf[Dest] := Char(0);
  Result := StrPas(buf);
end;


function FileSize(const fname: string): LongInt;
var
  SearchRec: TSearchRec;
begin
  if FindFirst(ExpandFileName(fname), faAnyFile, SearchRec) = 0 then
    Result := SearchRec.Size
  else
    Result := -1;
end;


function FileCopy(Source, Dest: string): Boolean;
var
  fSrc, fDst, len: Integer;
  Size: LongInt;
  Buffer: packed array[0..2047] of Byte;
begin
  Result := False; { Assume that it WONT work }
  if Source <> Dest then
  begin
    fSrc := FileOpen(Source, fmOpenRead);
    if fSrc >= 0 then
    begin
      Size := FileSeek(fSrc, 0, 2);
      FileSeek(fSrc, 0, 0);
      fDst := FileCreate(Dest);
      if fDst >= 0 then
      begin
        while Size > 0 do
        begin
          len := FileRead(fSrc, Buffer, SizeOf(Buffer));
          FileWrite(fDst, Buffer, len);
          Size := Size - len;
        end;
        FileSetDate(fDst, FileGetDate(fSrc));
        FileClose(fDst);
        FileSetAttr(Dest, FileGetAttr(Source));
        Result := True;
      end;
      FileClose(fSrc);
    end;
  end;
end;

function FileCopyEx(Source, Dest: string): Boolean;
var
  fSrc, fDst, len: Integer;
  Size: LongInt;
  Buffer: array[0..512000] of Byte;
begin
  Result := False; { Assume that it WONT work }
  if Source <> Dest then
  begin
    fSrc := FileOpen(Source, fmOpenRead or fmShareDenyNone);
    if fSrc >= 0 then
    begin
      Size := FileSeek(fSrc, 0, 2);
      FileSeek(fSrc, 0, 0);
      fDst := FileCreate(Dest);
      if fDst >= 0 then
      begin
        while Size > 0 do
        begin
          len := FileRead(fSrc, Buffer, SizeOf(Buffer));
          FileWrite(fDst, Buffer, len);
          Size := Size - len;
        end;
        FileSetDate(fDst, FileGetDate(fSrc));
        FileClose(fDst);
        FileSetAttr(Dest, FileGetAttr(Source));
        Result := True;
      end;
      FileClose(fSrc);
    end;
  end;
end;


function GetDefColorByName(Str: string): TColor;
var
  Cnt: Integer;
  COmpStr: string;
begin
  COmpStr := UpperCase(Str);
  for Cnt := 1 to MAXDEFCOLOR do
  begin
    if COmpStr = ColorNames[Cnt].Name then
    begin
      Result := TColor(ColorNames[Cnt].varl);
      Exit;
    end;
  end;
  Result := $0;
end;

function GetULMarkerType(Str: string): LongInt;
var
  Cnt: Integer;
  COmpStr: string;
begin
  COmpStr := UpperCase(Str);
  for Cnt := 1 to MAXLISTMARKER do
  begin
    if COmpStr = LiMarkerNames[Cnt].Name then
    begin
      Result := LiMarkerNames[Cnt].varl;
      Exit;
    end;
  end;
  Result := 1;
end;

function GetDefines(Str: string): LongInt;
var
  Cnt: Integer;
  COmpStr: string;
begin
  COmpStr := UpperCase(Str);
  for Cnt := 1 to MAXPREDEFINE do
  begin
    if COmpStr = PreDefineNames[Cnt].Name then
    begin
      Result := PreDefineNames[Cnt].varl;
      Exit;
    end;
  end;
  Result := -1;
end;

procedure ClearWindow(aCanvas: TCanvas; aLeft, aTop, aRight, aBottom: LongInt; aColor: TColor);
begin
  with aCanvas do
  begin
    Brush.Color := aColor;
    Pen.Color := aColor;
    Rectangle(0, 0, aRight - aLeft, aBottom - aTop);
  end;
end;


procedure DrawTileImage(Canv: TCanvas; Rect: TRect; TileImage: TBitmap);
var
  I, J, ICnt, JCnt, BmWidth, BmHeight: Integer;
begin

  BmWidth := TileImage.Width;
  BmHeight := TileImage.Height;
  ICnt := ((Rect.Right - Rect.Left) + BmWidth - 1) div BmWidth;
  JCnt := ((Rect.Bottom - Rect.Top) + BmHeight - 1) div BmHeight;

  UnrealizeObject(Canv.Handle);
  SelectPalette(Canv.Handle, TileImage.Palette, False);
  RealizePalette(Canv.Handle);

  for J := 0 to JCnt do
  begin
    for I := 0 to ICnt do
    begin

      { if (I * BmWidth) < (Rect.Right-Rect.Left) then
         BmWidth := TileImage.Width else
          BmWidth := (Rect.Right - Rect.Left) - ((I-1) * BmWidth);

       if (
       BmWidth := TileImage.Width;
       BmHeight := TileImage.Height;  }

      BitBlt(Canv.Handle,
        Rect.Left + I * BmWidth,
        Rect.Top + (J * BmHeight),
        BmWidth,
        BmHeight,
        TileImage.Canvas.Handle,
        0,
        0,
        SRCCOPY);

    end;
  end;

end;


procedure TiledImage(Canv: TCanvas; Rect: TLRect; TileImage: TBitmap);
var
  I, J, ICnt, JCnt, BmWidth, BmHeight: Integer;
  Rleft, RTop, RWidth, RHeight, BLeft, btop: LongInt;
begin

  if Assigned(TileImage) then
    if TileImage.Handle <> 0 then
    begin

      BmWidth := TileImage.Width;
      BmHeight := TileImage.Height;
      ICnt := (Rect.Right + BmWidth - 1) div BmWidth - (Rect.Left div BmWidth);
      JCnt := (Rect.Bottom + BmHeight - 1) div BmHeight - (Rect.Top div BmHeight);

      UnrealizeObject(Canv.Handle);
      SelectPalette(Canv.Handle, TileImage.Palette, False);
      RealizePalette(Canv.Handle);

      for J := 0 to JCnt do
      begin
        for I := 0 to ICnt do
        begin

          if I = 0 then
          begin
            BLeft := Rect.Left - ((Rect.Left div BmWidth) * BmWidth);
            Rleft := Rect.Left;
            RWidth := BmWidth;
          end
          else
          begin
            if I = ICnt then
              RWidth := Rect.Right - ((Rect.Right div BmWidth) * BmWidth)
            else
              RWidth := BmWidth;
            BLeft := 0;
            Rleft := (Rect.Left div BmWidth) + (I * BmWidth);
          end;


          if J = 0 then
          begin
            btop := Rect.Top - ((Rect.Top div BmHeight) * BmHeight);
            RTop := Rect.Top;
            RHeight := BmHeight;
          end
          else
          begin
            if J = JCnt then
              RHeight := Rect.Bottom - ((Rect.Bottom div BmHeight) * BmHeight)
            else
              RHeight := BmHeight;
            btop := 0;
            RTop := (Rect.Top div BmHeight) + (J * BmHeight);
          end;

          BitBlt(Canv.Handle,
            Rleft,
            RTop,
            RWidth,
            RHeight,
            TileImage.Canvas.Handle,
            BLeft,
            btop,
            SRCCOPY);

        end;
      end;

    end;
end;

// 这个垃圾代码重写于2020-12-25 chongchong ************************************
// 修正 GetValidStr3('116/金戒指東|116/古铜戒指|', str, ['|') 得到 str 不正确
{
function GetValidStr3(Str: string; var Dest: string; const Divider: array of Char): string;
const
  BUF_SIZE = 20480; // $7FFF;
var
  buf: array[0..BUF_SIZE] of WideChar;
  BufCount, Count, srclen, I, ArrCount: LongInt;
  WideStr: WideString;
  WideCh: WideChar;
label
  CATCH_DIV;
begin
  WideStr := WideString(Str);
  WideCh := #0; // Jacky
  try
    srclen := Length(WideStr);
    BufCount := 0;
    Count := 1;

    if srclen >= BUF_SIZE - 1 then
    begin
      raise Exception.Create('GetValidStr3 Error, Length(Str) > 20480');
      Result := '';
      Dest := '';
      Exit;
    end;

    if WideStr = '' then
    begin
      Dest := '';
      Result := WideStr;
      Exit;
    end;
    ArrCount := SizeOf(Divider) div SizeOf(Char);

    while True do
    begin
      if Count <= srclen then
      begin
        WideCh := WideStr[Count];
        for I := 0 to ArrCount - 1 do
          if WideCh = WideChar(Divider[I]) then
            goto CATCH_DIV;
      end;
      if (Count > srclen) then
      begin
        CATCH_DIV:
        if (BufCount > 0) then
        begin
          if BufCount < BUF_SIZE - 1 then
          begin
            buf[BufCount] := #0;
            Dest := string(WideString(buf));

            if Count >= srclen then
              Result := ''
            else
              Result := Copy(WideStr, Count + 1, srclen - Count);
          end;
          Break;
        end
        else
        begin
          if (Count > srclen) then
          begin
            Dest := '';
            Result := Copy(WideStr, Count + 2, srclen - 1);
            Break;
          end;
        end;
      end
      else
      begin
        if BufCount < BUF_SIZE - 1 then
        begin
          buf[BufCount] := WideCh;
          Inc(BufCount);
        end; // else
        // ShowMessage ('BUF_SIZE overflow !');
      end;
      Inc(Count);
    end;
  except
    Dest := '';
    Result := '';
  end;
end;
}

function GetValidStr3(Str: string; var Dest: string; const Divider: array of Char): string;
var
  I, II, Len, DividerCount, StartIndex: Integer;
  IsFound, IsStart: Boolean;
{$IFDEF UNICODE}
  C: Char;
{$ELSE}
  StrTemp: WideString;
  C: WideChar;
{$ENDIF}
begin
  Dest := Str;
  Result := '';
  Len := Length(Str);
  DividerCount := SizeOf(Divider) div SizeOf(Char);
  if (Len = 0) or (DividerCount = 0) then Exit;

  IsStart := False;
  StartIndex := 1;

{$IFDEF UNICODE}
  for I := 1 to Len do
  begin
    C := Str[I];

    IsFound := False;
    for II := 0 to DividerCount - 1 do
    begin
      if C = WideChar(Divider[II]) then
      begin
        IsFound := True;
        Break;
      end;
    end;

    // 丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉
    if IsFound then
    begin
      if IsStart then
      begin
        Dest := Copy(Str, StartIndex, I - StartIndex);
        Result := Copy(Str, I + 1, Len - I);
        Exit;
      end;
    end
    else if not IsStart then
    begin
      IsStart := True;
      StartIndex := I;
    end;
  end;

  // 如果只有最前面有分隔符，后面都没有，把最前面的分隔符全丢掉
  if StartIndex > 1 then
  begin
    Dest := Copy(Str, StartIndex, Len - StartIndex + 1);
  end;
{$ELSE}
  StrTemp := Str;
  Len := Length(StrTemp);

  for I := 1 to Len do
  begin
    C := StrTemp[I];

    IsFound := False;
    for II := 0 to DividerCount - 1 do
    begin
      if C = WideChar(Divider[II]) then
      begin
        IsFound := True;
        Break;
      end;
    end;

    // 丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉
    if IsFound then
    begin
      if IsStart then
      begin
        Dest := Copy(StrTemp, StartIndex, I - StartIndex);
        Result := Copy(StrTemp, I + 1, Len - I);
        Exit;
      end;
    end
    else if not IsStart then
    begin
      IsStart := True;
      StartIndex := I;
    end;
  end;

  // 如果只有最前面有分隔符，后面都没有，把最前面的分隔符全丢掉
  if StartIndex > 1 then
  begin
    Dest := Copy(StrTemp, StartIndex, Len - StartIndex + 1);
  end;
{$ENDIF}
end;


function GetValidStrCap(Str: string; var Dest: string; const Divider: array of Char): string;
begin
  Str := TrimLeft(Str);
  if Str <> '' then
  begin
    if Str[1] = '"' then
      Result := CaptureString(Str, Dest)
    else
    begin
      Result := GetValidStr3(Str, Dest, Divider);
    end;
  end
  else
  begin
    Result := '';
    Dest := '';
  end;
end;

// 这个垃圾代码重写于2020-12-25 chongchong ************************************

// 修正 GetValidStr3('116/金戒指東|116/古铜戒指|', str, ['|') 得到 str 不正确
{
function GetValidStr3_Ex(Str: string; var Dest: string; const Divider: Char): string;
const
  BUF_SIZE = 204800; // $7FFF;
var
  buf: array[0..BUF_SIZE] of WideChar;
  BufCount, Count, srclen: LongInt;
  WideStr: WideString;
  WideCh: WideChar;
  WideDivider: WideChar;
label
  CATCH_DIV;
begin
  WideStr := WideString(Str);
  WideCh := #0; // Jacky
  try
    srclen := Length(WideStr);
    BufCount := 0;
    Count := 1;

    if srclen >= BUF_SIZE - 1 then
    begin
      raise Exception.Create('GetValidStr3 Error, Length(Str) > 204800');
      Result := '';
      Dest := '';
      Exit;
    end;

    if WideStr = '' then
    begin
      Dest := '';
      Result := WideStr;
      Exit;
    end;

    WideDivider := WideChar(Divider);

    while True do
    begin
      if Count <= srclen then
      begin
        WideCh := WideStr[Count];
        if WideCh = WideDivider then
          goto CATCH_DIV;
      end;
      if (Count > srclen) then
      begin
        CATCH_DIV:
        if (BufCount > 0) then
        begin
          if BufCount < BUF_SIZE - 1 then
          begin
            buf[BufCount] := #0;
            Dest := string(WideString(buf));

            if Count >= srclen then
              Result := ''
            else
              Result := Copy(WideStr, Count + 1, srclen - Count);
          end;
          Break;
        end
        else
        begin
          if (Count > srclen) then
          begin
            Dest := '';
            Result := Copy(WideStr, Count + 2, srclen - 1);
            Break;
          end;
        end;
      end
      else
      begin
        if BufCount < BUF_SIZE - 1 then
        begin
          buf[BufCount] := WideCh;
          Inc(BufCount);
        end; // else
        // ShowMessage ('BUF_SIZE overflow !');
      end;
      Inc(Count);
    end;
  except
    Dest := '';
    Result := '';
  end;
end;
}

function GetValidStr3_Ex(Str: string; var Dest: string; const Divider: Char): string;
var
  I, Len, DividerCount, StartIndex: Integer;
  IsStart: Boolean;
{$IFDEF UNICODE}
  C: Char;
{$ELSE}
  StrTemp: WideString;
  C, WideDivider: WideChar;
{$ENDIF}
begin
  Dest := Str;
  Result := '';
  Len := Length(Str);
  DividerCount := SizeOf(Divider) div SizeOf(Char);
  if (Len = 0) or (DividerCount = 0) then Exit;

  IsStart := False;
  StartIndex := 1;

{$IFDEF UNICODE}
  for I := 1 to Len do
  begin
    C := Str[I];

    // 丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉
    if C = Divider then
    begin
      if IsStart then
      begin
        Dest := Copy(Str, StartIndex, I - StartIndex);
        Result := Copy(Str, I + 1, Len - I);
        Exit;
      end
      else
        StartIndex := I + 1;     // 没有别的全是分隔符，都丢了 2021-01-21
    end
    else if not IsStart then
    begin
      IsStart := True;
      StartIndex := I;
    end;
  end;

  // 如果只有最前面有分隔符，后面都没有，把最前面的分隔符全丢掉
  if StartIndex > 1 then
  begin
    Dest := Copy(Str, StartIndex, Len - StartIndex + 1);
  end;
{$ELSE}
  WideDivider := WideChar(Divider);

  StrTemp := Str;
  Len := Length(StrTemp);
  for I := 1 to Len do
  begin
    C := StrTemp[I];

    // 丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉
    if C = WideDivider then
    begin
      if IsStart then
      begin
        Dest := Copy(StrTemp, StartIndex, I - StartIndex);
        Result := Copy(StrTemp, I + 1, Len - I);
        Exit;
      end
      else    // 没有别的全是分隔符，都丢了 2021-01-21
        StartIndex := I + 1;
    end
    else if not IsStart then
    begin
      IsStart := True;
      StartIndex := I;
    end;
  end;

  // 如果只有最前面有分隔符，后面都没有，把最前面的分隔符全丢掉
  if StartIndex > 1 then
  begin
    Dest := Copy(StrTemp, StartIndex, Len - StartIndex + 1);
  end;
{$ENDIF}
end;

function IntToStr2(n: Integer): string;
begin
  if n < 10 then
    Result := '0' + IntToStr(n)
  else
    Result := IntToStr(n);
end;

function IntToStrFill(num, len: Integer; Fill: Char): string;
var
  I: Integer;
  Str: string;
begin
  Result := '';
  Str := IntToStr(num);
  for I := 1 to len - Length(Str) do
    Result := Result + Fill;
  Result := Result + Str;
end;

function IsInB(Src: string; Pos: Integer; targ: string): Boolean;
var
  tLen, I: Integer;
begin
  Result := False;
  tLen := Length(targ);
  if Length(Src) < Pos + tLen then
    Exit;
  for I := 0 to tLen - 1 do
    if UpCase(Src[Pos + I]) <> UpCase(targ[I + 1]) then
      Exit;

  Result := True;
end;

function IsInRect(x, y: Integer; Rect: TRect): Boolean;
begin
  if (x >= Rect.Left) and (x <= Rect.Right) and (y >= Rect.Top) and (y <= Rect.Bottom) then
    Result := True
  else
    Result := False;
end;

function IsStringNumber(Str: string): Boolean;
var
  I: Integer;
begin
  Result := True;
  for I := 1 to Length(Str) do
    if (Byte(Str[I]) < Byte('0')) or (Byte(Str[I]) > Byte('9')) then
    begin
      Result := False;
      Break;
    end;
end;

function IsNumber(Str: string): Boolean;
var
  I, n: Integer;
begin
  Result := False;
  if Str = '' then
    Exit;
  Result := True;
  n := 1;
  if (Str[1] = '-') then
    n := 2;
  for I := n to Length(Str) do
    if (Byte(Str[I]) < Byte('0')) or (Byte(Str[I]) > Byte('9')) then
    begin
      Result := False;
      Break;
    end;
end;

{function IsVarNumber (str: string): boolean;
var i: integer;
begin
   Result := FALSE;
   if length(str) <= 3 then begin
     if (UpCase(str[1]) = 'P') or (UpCase(str[1]) = 'G') or (UpCase(str[1]) = 'M') or (UpCase(str[1]) = 'I') or (UpCase(str[1]) = 'D') or (UpCase(str[1]) = 'N') or (UpCase(str[1]) = 'A') then begin
       if (length(str) = 3) and IsStringNumber(str[2]) and IsStringNumber(str[3]) then Result := TRUE
       else if (length(str) = 2) and IsStringNumber(str[2]) then Result := TRUE;
     end;
   end;
end; }

function IsVarNumber(Str: string): Boolean;
begin
  Result := (CompareLStr(Str, 'HUMAN', Length('HUMAN'))) or (CompareLStr(Str, 'GUILD', Length('GUILD'))) or (CompareLStr(Str, 'GLOBAL', Length('GLOBAL')));
end;

{Return : remain string}

function ArrestString(Source, SearchAfter, ArrestBefore: string;
  const DropTags: array of string; var RsltStr: string): string;
const
  BUF_SIZE = $7FFF;
var
  buf: array[0..BUF_SIZE] of Char;
  BufCount, SrcCount, srclen, {AfterLen, BeforeLen,} DropCount, I: Integer;
  ArrestNow: Boolean;
begin
  try
    // EnterCriticalSection (CSUtilLock);
    RsltStr := ''; {result string}
    srclen := Length(Source);

    if srclen > BUF_SIZE then
    begin
      Result := '';
      Exit;
    end;

    BufCount := 0;
    SrcCount := 1;
    ArrestNow := False;
    DropCount := SizeOf(DropTags) div SizeOf(string);

    if (SearchAfter = '') then
      ArrestNow := True;

    // GetMem (Buf, BUF_SIZE);

    while True do
    begin
      if SrcCount > srclen then
        Break;

      if not ArrestNow then
      begin
        if IsInB(Source, SrcCount, SearchAfter) then
          ArrestNow := True;
      end
      else
      begin
        buf[BufCount] := Source[SrcCount];
        if IsInB(Source, SrcCount, ArrestBefore) or (BufCount >= BUF_SIZE - 2) then
        begin
          BufCount := BufCount - Length(ArrestBefore);
          buf[BufCount + 1] := #0;
          RsltStr := string(buf);
          BufCount := 0;
          Break;
        end;

        for I := 0 to DropCount - 1 do
        begin
          if IsInB(Source, SrcCount, DropTags[I]) then
          begin
            BufCount := BufCount - Length(DropTags[I]);
            Break;
          end;
        end;

        Inc(BufCount);
      end;
      Inc(SrcCount);
    end;

    if (ArrestNow) and (BufCount <> 0) then
    begin
      buf[BufCount] := #0;
      RsltStr := string(buf);
    end;

    Result := Copy(Source, SrcCount + 1, srclen - SrcCount); {result is remain string}
  finally
    // LeaveCriticalSection (CSUtilLock);
  end;
end;

function ArrestStringEx(const Source: WideString; const SearchStart, SearchEnd: WideChar; var ArrestStr: string): WideString;
var
  SrcLen, I, FoundIndex: Integer;
  P, P1, P2: PWideChar;
  WS: WideString;
begin
  Result := Source;
  ArrestStr := '';
  {result string}
  if Length(Source) = 0 then
  begin
    Result := '';
    Exit;
  end;

  SrcLen := Length(Source);
  P := PWideChar(Source);
  P1 := P;
  P2 := nil;
  FoundIndex := 0;
  for I := 0 to SrcLen - 1 do
  begin
    if P1^ = SearchStart then
    begin
      FoundIndex := I + 1;
      Inc(P1);
      P2 := P1;
      Break;
    end;
    Inc(P1);
  end;

  if P2 <> nil then
  begin
    for I := FoundIndex to SrcLen - 1 do
    begin
      if P1^ = SearchEnd then
      begin
        WS := Copy(Source, P2 - P + 1, P1 - P2);
        ArrestStr := WS;
        Result := Copy(Source, P1 - P + 1 + 1, SrcLen - (P1 + 1 - P));
        Break;
      end;
      Inc(P1);
    end;
  end;
end;

function ArrestStringEx_Ansi(const Source: string; const SearchStart, SearchEnd: Char; var ArrestStr: string): string;
var
  SrcLen, I, FoundIndex: Integer;
  P, P1, P2: PChar;
begin
  Result := Source;
  ArrestStr := '';
  {result string}
  if Length(Source) = 0 then
  begin
    Result := '';
    Exit;
  end;

  SrcLen := Length(Source);
  P := PChar(Source);
  P1 := P;
  P2 := nil;
  FoundIndex := 0;
  for I := 0 to SrcLen - 1 do
  begin
    if P1^ = SearchStart then
    begin
      FoundIndex := I + 1;
      Inc(P1);
      P2 := P1;
      Break;
    end;
    Inc(P1);
  end;

  if P2 <> nil then
  begin
    for I := FoundIndex to SrcLen - 1 do
    begin
      if P1^ = SearchEnd then
      begin
        ArrestStr := Copy(Source, P2 - P + 1, P1 - P2);
        Result := Copy(Source, P1 + 1 - P + 1, SrcLen - (P1 + 1 - P));
        Break;
      end;
      Inc(P1);
    end;
  end;
end;

function ArrestVariable(Source: string; Left, Center, Right: Char; StartPos: Integer; var ArrestStr: string): Integer;
var
  Index: Integer;
  bo1D: Boolean;
  bo2D: Boolean;
  nPos: Integer;
  nLen: Integer;
begin
  ArrestStr := '';
  Result := 0;
  nPos := 0;
  if Source = '' then
  begin
    Exit;
  end;

  Index := StartPos;
  bo1D := False;
  bo2D := False;
  if Index <= 0 then
    Index := 1;
  while True do
  begin
    if Index > Length(Source) then
    begin
      break;
    end;

    if (Source[Index] = Left) then
    begin
      bo1D := True;
      nPos := Index;
      Inc(Index);
      if Index > Length(Source) then
        break;

      if (Source[Index] = Center) then
      begin
        bo2D := True;
        Inc(Index);
        if Index > Length(Source) then
          break;
      end
      else
      begin
        bo1D := False;
        bo2D := False;
      end;
    end;

    if bo1D and bo2D then
    begin
      if (Source[Index] = Right) then
      begin
        nLen := Index - nPos - 1;
        ArrestStr := Copy(Source, nPos + 1, nLen);
        Result := nPos;
        break;
      end;
    end;

    Inc(Index);
  end;
end;

function ArrestStringEx2(Source: string; Left, Center, Right: Char; var ArrestStr: string): Integer;
var
  Index: Integer;
  bo1D: Boolean;
  bo2D: Boolean;
  nPos: Integer;
  nLen: Integer;
begin
  ArrestStr := '';
  Result := 0;
  nPos := 0;
  if Source = '' then
  begin
    Exit;
  end;

  bo1D := False;
  bo2D := False;
  for Index := 1 to Length(Source) do
  begin
    if Source[Index] = Left then
    begin
      bo1D := True;
      nPos := Index;
      Continue;
    end;
    if (Source[Index] = Center) then
    begin
      if bo1D then
      begin
        bo2D := True;
        Continue;
      end
      else
      begin
        bo1D := False;
        bo2D := False;
      end;
    end;
    if (Source[Index] = Right) then
    begin
      if bo1D and bo2D then
      begin
        nLen := Index - nPos - 1;
        ArrestStr := Copy(Source, nPos + 1, nLen);
        Result := nPos;
        break;
      end
      else
      begin
        bo1D := False;
        bo2D := False;
      end;
    end;
  end;
end;


function SkipStr(Src: string; const Skips: array of Char): string;
var
  I, len, c: Integer;
  NowSkip: Boolean;
begin
  len := Length(Src);
  // Count := sizeof(Skips) div sizeof (Char);

  for I := 1 to len do
  begin
    NowSkip := False;
    for c := Low(Skips) to High(Skips) do
      if Src[I] = Skips[c] then
      begin
        NowSkip := True;
        Break;
      end;
    if not NowSkip then
      Break;
  end;

  Result := Copy(Src, I, len - I + 1);

end;


function GetStrToCoords(Str: string): TRect;
var
  Temp: string;
begin

  Str := GetValidStr3(Str, Temp, [',', ' ']); Result.Left := Str_ToInt(Temp, 0);
  Str := GetValidStr3(Str, Temp, [',', ' ']); Result.Top := Str_ToInt(Temp, 0);
  Str := GetValidStr3(Str, Temp, [',', ' ']); Result.Right := Str_ToInt(Temp, 0);
  GetValidStr3(Str, Temp, [',', ' ']); Result.Bottom := Str_ToInt(Temp, 0);

end;

function CombineDirFile(SrcDir, TargName: string): string;
begin
  if (SrcDir = '') or (TargName = '') then
  begin
    Result := SrcDir + TargName;
    Exit;
  end;
  if SrcDir[Length(SrcDir)] = '\' then
    Result := SrcDir + TargName
  else
    Result := SrcDir + '\' + TargName;
end;

function CompareLStr(Src, targ: string; compn: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  if (compn <= 0) or (Length(Src) < compn) or (Length(targ) < compn) then
    Exit;
  Result := True;
  for I := 1 to compn do
    if UpCase(Src[I]) <> UpCase(targ[I]) then
    begin
      Result := False;
      Break;
    end;
end;

function CompareBuffer(p1, P2: PByte; len: Integer): Boolean;
var
  I: Integer;
begin
  Result := True;
  for I := 0 to len - 1 do
    if PByte(NativeInt(p1) + I)^ <> PByte(NativeInt(P2) + I)^ then
    begin
      Result := False;
      Break;
    end;
end;

function CompareBackLStr(Src, targ: string; compn: Integer): Boolean;
var
  I, slen, tLen: Integer;
begin
  Result := False;
  if compn <= 0 then
    Exit;
  if Length(Src) < compn then
    Exit;
  if Length(targ) < compn then
    Exit;
  slen := Length(Src);
  tLen := Length(targ);
  Result := True;
  for I := 0 to compn - 1 do
    if UpCase(Src[slen - I]) <> UpCase(targ[tLen - I]) then
    begin
      Result := False;
      Break;
    end;
end;


function IsEnglish(Ch: Char): Boolean;
begin
  Result := False;
  if ((Ch >= 'A') and (Ch <= 'Z')) or ((Ch >= 'a') and (Ch <= 'z')) then
    Result := True;
end;

function IsEngNumeric(Ch: Char): Boolean;
begin
  Result := False;
  if IsEnglish(Ch) or ((Ch >= '0') and (Ch <= '9')) then
    Result := True;
end;

function IsFloatNumeric(Str: string): Boolean;
begin
  if Trim(Str) = '' then
  begin
    Result := False;
    Exit;
  end;
  try
    StrToFloat(Str);
    Result := True;
  except
    Result := False;
  end;
end;

procedure PCharSet(P: PChar; n: Integer; Ch: Char);
var
  I: Integer;
begin
  for I := 0 to n - 1 do
    (P + I)^ := Ch;
end;

function ReplaceChar(Src: string; srcchr, repchr: Char): string;
var
  I, len: Integer;
begin
  if Src <> '' then
  begin
    len := Length(Src);
    for I := 0 to len - 1 do
      if Src[I] = srcchr then
        Src[I] := repchr;
  end;
  Result := Src;
end;


function IsUniformStr(Src: string; Ch: Char): Boolean;
var
  I, len: Integer;
begin
  Result := True;
  if Src <> '' then
  begin
    len := Length(Src);
    for I := 0 to len - 1 do
      if Src[I] = Ch then
      begin
        Result := False;
        Break;
      end;
  end;
end;


function CreateMask(Src: PChar; TargPos: Integer): string;

  function IsNumber(Chr: Char): Boolean;
  begin
    if (Chr >= '0') and (Chr <= '9') then
      Result := True
    else
      Result := False;
  end;
var
  intFlag, Loop: Boolean;
  Cnt, IntCnt, SrcLen: Integer;
  Ch, Ch2: Char;
begin
  intFlag := False;
  Loop := True;
  Cnt := 0;
  IntCnt := 0;
  SrcLen := StrLen(Src);

  while Loop do
  begin
    Ch := PChar(NativeInt(Src) + Cnt)^;
    case Ch of
      #0:
        begin
          Result := '';
          Break;
        end;
      ' ':
        begin
        end;
    else
      begin

        if not intFlag then
        begin { Now Reading char }
          if IsNumber(Ch) then
          begin
            intFlag := True;
            Inc(IntCnt);
          end;
        end
        else
        begin { If, now reading integer }
          if not IsNumber(Ch) then
          begin { XXE+3 }
            case UpCase(Ch) of
              'E':
                begin
                  if (Cnt >= 1) and (Cnt + 2 < SrcLen) then
                  begin
                    Ch := PChar(NativeInt(Src) + Cnt - 1)^;
                    if IsNumber(Ch) then
                    begin
                      Ch := PChar(NativeInt(Src) + Cnt + 1)^;
                      Ch2 := PChar(NativeInt(Src) + Cnt + 2)^;
                      if not ((Ch = '+') and (IsNumber(Ch2))) then
                      begin
                        intFlag := False;
                      end;
                    end;
                  end;
                end;
              '+':
                begin
                  if (Cnt >= 1) and (Cnt + 1 < SrcLen) then
                  begin
                    Ch := PChar(NativeInt(Src) + Cnt - 1)^;
                    Ch2 := PChar(NativeInt(Src) + Cnt + 1)^;
                    if not ((UpCase(Ch) = 'E') and (IsNumber(Ch2))) then
                    begin
                      intFlag := False;
                    end;
                  end;
                end;
              '.':
                begin
                  if (Cnt >= 1) and (Cnt + 1 < SrcLen) then
                  begin
                    Ch := PChar(NativeInt(Src) + Cnt - 1)^;
                    Ch2 := PChar(NativeInt(Src) + Cnt + 1)^;
                    if not ((IsNumber(Ch)) and (IsNumber(Ch2))) then
                    begin
                      intFlag := False;
                    end;
                  end;
                end;

            else
              intFlag := False;
            end;
          end;
        end; {end of case else}
      end; {end of Case}
    end;
    if (intFlag) and (Cnt >= TargPos) then
    begin
      Result := '%' + Format('%d', [IntCnt]);
      Exit;
    end;
    Inc(Cnt);
  end;
end;

function GetValueFromMask(Src: PChar; Mask: string): string;

  function Positon(Str: string): Integer;
  var
    str2: string;
  begin
    str2 := Copy(Str, 2, Length(Str) - 1);
    Result := StrToIntDef(str2, 0);
    if Result <= 0 then
      Result := 1;
  end;

  function IsNumber(Ch: Char): Boolean;
  begin
    case Ch of
      '0'..'9': Result := True;
    else
      Result := False;
    end;
  end;
var
  intFlag, Loop, Sign: Boolean;
  buf: Str256;
  BufCount, Pos, LocCount, TargLoc, SrcLen: Integer;
  Ch, Ch2: Char;
begin
  SrcLen := StrLen(Src);
  LocCount := 0;
  BufCount := 0;
  Pos := 0;
  intFlag := False;
  Loop := True;
  Sign := False;

  if Mask = '' then
    Mask := '%1';
  TargLoc := Positon(Mask);

  while Loop do
  begin
    if Pos >= SrcLen then
      Break;
    Ch := PChar(Src + Pos)^;
    if not intFlag then
    begin {now reading chars}
      if LocCount < TargLoc then
      begin
        if IsNumber(Ch) then
        begin
          intFlag := True;
          BufCount := 0;
          Inc(LocCount);
        end
        else
        begin
          if not Sign then
          begin {default '+'}
            if Ch = '-' then
              Sign := True;
          end
          else
          begin
            if Ch <> ' ' then
              Sign := False;
          end;
        end;
      end
      else
      begin
        Break;
      end;
    end;
    if intFlag then
    begin {now reading numbers}
      buf[BufCount] := Ch;
      Inc(BufCount);
      if not IsNumber(Ch) then
      begin
        case Ch of
          'E', 'e':
            begin
              if (Pos >= 1) and (Pos + 2 < SrcLen) then
              begin
                Ch := PChar(Src + Pos - 1)^;
                if IsNumber(Ch) then
                begin
                  Ch := PChar(Src + Pos + 1)^;
                  Ch2 := PChar(Src + Pos + 2)^;
                  if not ((Ch = '+') or (Ch = '-') and (IsNumber(Ch2))) then
                  begin
                    Dec(BufCount);
                    intFlag := False;
                  end;
                end;
              end;
            end;
          '+', '-':
            begin
              if (Pos >= 1) and (Pos + 1 < SrcLen) then
              begin
                Ch := PChar(Src + Pos - 1)^;
                Ch2 := PChar(Src + Pos + 1)^;
                if not ((UpCase(Ch) = 'E') and (IsNumber(Ch2))) then
                begin
                  Dec(BufCount);
                  intFlag := False;
                end;
              end;
            end;
          '.':
            begin
              if (Pos >= 1) and (Pos + 1 < SrcLen) then
              begin
                Ch := PChar(Src + Pos - 1)^;
                Ch2 := PChar(Src + Pos + 1)^;
                if not ((IsNumber(Ch)) and (IsNumber(Ch2))) then
                begin
                  Dec(BufCount);
                  intFlag := False;
                end;
              end;
            end;
        else
          begin
            intFlag := False;
            Dec(BufCount);
          end;
        end;
      end;
    end;
    Inc(Pos);
  end;
  if LocCount = TargLoc then
  begin
    buf[BufCount] := #0;
    if Sign then
      Result := '-' + StrPas(buf)
    else
      Result := StrPas(buf);
  end
  else
    Result := '';
end;

procedure GetDirList(Path: string; fllist: TStringList);
var
  SearchRec: TSearchRec;
begin
  if FindFirst(Path, faAnyFile, SearchRec) = 0 then
  begin
    fllist.AddObject(SearchRec.Name, TObject(SearchRec.Time));
    while True do
    begin
      if FindNext(SearchRec) = 0 then
      begin
        fllist.AddObject(SearchRec.Name, TObject(SearchRec.Time));
      end
      else
      begin
        SysUtils.FindClose(SearchRec);
        Break;
      end;
    end;
  end;
end;

function GetFileDate(FileName: string): Integer; // DOS format file date..
var
  SearchRec: TSearchRec;
begin
  Result := 0; // jacky
  if FindFirst(FileName, faAnyFile, SearchRec) = 0 then
  begin
    Result := SearchRec.Time;
    SysUtils.FindClose(SearchRec);
  end;
end;




procedure ShlStr(Source: PChar; Count: Integer);
var
  I, len: Integer;
begin
  len := StrLen(Source);
  while (Count > 0) do
  begin
    for I := 0 to len - 2 do
      Source[I] := Source[I + 1];
    Source[len - 1] := #0;

    Dec(Count);
  end;
end;

procedure ShrStr(Source: PChar; Count: Integer);
var
  I, len: Integer;
begin
  len := StrLen(Source);
  while (Count > 0) do
  begin
    for I := len - 1 downto 0 do
      Source[I + 1] := Source[I];
    Source[len + 1] := #0;

    Dec(Count);
  end;
end;

function LRect(l, t, r, b: LongInt): TLRect;
begin
  Result.Left := l;
  Result.Top := t;
  Result.Right := r;
  Result.Bottom := b;
end;

procedure MemPCopy(Dest: PChar; Src: string);
var
  I: Integer;
begin
  for I := 0 to Length(Src) - 1 do
    Dest[I] := Src[I + 1];
end;

procedure MemCpy(Dest, Src: PChar; Count: LongInt);
var
  I: LongInt;
begin
  for I := 0 to Count - 1 do
  begin
    PChar(NativeInt(Dest) + I)^ := PChar(NativeInt(Src) + I)^;
  end;
end;

procedure memcpy2(TargAddr, SrcAddr: LongInt; Count: Integer);
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    PChar(TargAddr + I)^ := PChar(SrcAddr + I)^;
end;

procedure memset(Buffer: PChar; FillChar: Char; Count: Integer);
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    Buffer[I] := FillChar;
end;

procedure Str256PCopy(Dest: PChar; const Src: string);
begin
  StrPLCopy(Dest, Src, 255);
end;

function _StrPas(Dest: PChar): string;
var
  I: Integer;
begin
  Result := '';
  for I := 0 to Length(Dest) - 1 do
    if Dest[I] <> Chr(0) then
      Result := Result + Dest[I]
    else
      Break;
end;

function Str_PCopy(Dest: PChar; Src: string): Integer;
var
  len, I: Integer;
begin
  len := Length(Src);
  for I := 1 to len do
    Dest[I - 1] := Src[I];
  Dest[len] := #0;
  Result := len;
end;

function Str_PCopyEx(Dest: PChar; const Src: string; buflen: LongInt): Integer;
var
  len, I: Integer;
begin
  len := _MIN(Length(Src), buflen);
  for I := 1 to len do
    Dest[I - 1] := Src[I];
  Dest[len] := #0;
  Result := len;
end;

function Str_Catch(Src, Dest: string; len: Integer): string; // Result is rests..
begin

end;

function Trim_R(const Str: string): string;
var
  I, len, tr: Integer;
begin
  tr := 0;
  len := Length(Str);
  for I := len downto 1 do
    if Str[I] = ' ' then
      Inc(tr)
    else
      Break;
  Result := Copy(Str, 1, len - tr);
end;

function IsEqualFont(SrcFont, TarFont: TFont): Boolean;
begin
  Result := True;
  if SrcFont.Name <> TarFont.Name then
    Result := False;
  if SrcFont.Color <> TarFont.Color then
    Result := False;
  if SrcFont.Style <> TarFont.Style then
    Result := False;
  if SrcFont.Size <> TarFont.Size then
    Result := False;
end;


function CutHalfCode(Str: string): string;
var
  Pos, len: Integer;
begin

  Result := '';
  Pos := 1;
  len := Length(Str);

  while True do
  begin

    if Pos > len then
      Break;

    if (Str[Pos] > #127) then
    begin

      if ((Pos + 1) <= len) and (Str[Pos + 1] > #127) then
      begin
        Result := Result + Str[Pos] + Str[Pos + 1];
        Inc(Pos);
      end;

    end
    else
      Result := Result + Str[Pos];

    Inc(Pos);

  end;
end;


function ConvertToShortName(Canvas: TCanvas; Source: string; WantWidth: Integer): string;
var
  I, len: Integer;
  Str: string;
begin
  if Length(Source) > 3 then
    if Canvas.TextWidth(Source) > WantWidth then
    begin

      len := Length(Source);
      for I := 1 to len do
      begin

        Str := Copy(Source, 1, (len - I));
        Str := Str + '..';

        if Canvas.TextWidth(Str) < (WantWidth - 4) then
        begin
          Result := CutHalfCode(Str);
          Exit;
        end;

      end;

      Result := CutHalfCode(Copy(Source, 1, 2)) + '..';
      Exit;

    end;

  Result := Source;

end;


function DuplicateBitmap(Bitmap: TBitmap): HBitmap;
var
  hbmpOldSrc, hbmpOldDest, hbmpNew: HBitmap;
  hdcSrc, hdcDest: hdc;

begin
  hdcSrc := CreateCompatibleDC(0);
  hdcDest := CreateCompatibleDC(hdcSrc);

  hbmpOldSrc := SelectObject(hdcSrc, Bitmap.Handle);

  hbmpNew := CreateCompatibleBitmap(hdcSrc, Bitmap.Width, Bitmap.Height);

  hbmpOldDest := SelectObject(hdcDest, hbmpNew);

  BitBlt(hdcDest, 0, 0, Bitmap.Width, Bitmap.Height, hdcSrc, 0, 0,
    SRCCOPY);

  SelectObject(hdcDest, hbmpOldDest);
  SelectObject(hdcSrc, hbmpOldSrc);

  DeleteDC(hdcDest);
  DeleteDC(hdcSrc);

  Result := hbmpNew;
end;


procedure SpliteBitmap(DC: hdc; x, y: Integer; Bitmap: TBitmap; transcolor: TColor);
var
  hdcMixBuffer, hdcBackMask, hdcForeMask, hdcCopy: hdc;
  hOld, hbmCopy, hbmMixBuffer, hbmBackMask, hbmForeMask: HBitmap;
  OldColor: TColor;
begin

  {UnrealizeObject (DC);}
(*   SelectPalette (DC, bitmap.Palette, FALSE);
  RealizePalette (DC);
 *)

  hbmCopy := DuplicateBitmap(Bitmap);
  hdcCopy := CreateCompatibleDC(DC);
  hOld := SelectObject(hdcCopy, hbmCopy);

  hdcBackMask := CreateCompatibleDC(DC);
  hdcForeMask := CreateCompatibleDC(DC);
  hdcMixBuffer := CreateCompatibleDC(DC);

  hbmBackMask := CreateBitmap(Bitmap.Width, Bitmap.Height, 1, 1, nil);
  hbmForeMask := CreateBitmap(Bitmap.Width, Bitmap.Height, 1, 1, nil);
  hbmMixBuffer := CreateCompatibleBitmap(DC, Bitmap.Width, Bitmap.Height);

  SelectObject(hdcBackMask, hbmBackMask);
  SelectObject(hdcForeMask, hbmForeMask);
  SelectObject(hdcMixBuffer, hbmMixBuffer);

  OldColor := SetBkColor(hdcCopy, transcolor); // clWhite);

  BitBlt(hdcForeMask, 0, 0, Bitmap.Width, Bitmap.Height, hdcCopy, 0, 0, SRCCOPY);

  SetBkColor(hdcCopy, OldColor);

  BitBlt(hdcBackMask, 0, 0, Bitmap.Width, Bitmap.Height, hdcForeMask, 0, 0, NOTSRCCOPY);

  BitBlt(hdcMixBuffer, 0, 0, Bitmap.Width, Bitmap.Height, DC, x, y, SRCCOPY);

  BitBlt(hdcMixBuffer, 0, 0, Bitmap.Width, Bitmap.Height, hdcForeMask, 0, 0, SRCAND);

  BitBlt(hdcCopy, 0, 0, Bitmap.Width, Bitmap.Height, hdcBackMask, 0, 0, SRCAND);

  BitBlt(hdcMixBuffer, 0, 0, Bitmap.Width, Bitmap.Height, hdcCopy, 0, 0, SRCPAINT);

  BitBlt(DC, x, y, Bitmap.Width, Bitmap.Height, hdcMixBuffer, 0, 0, SRCCOPY);

  {DeleteObject (hbmCopy);}
  DeleteObject(SelectObject(hdcCopy, hOld));
  DeleteObject(SelectObject(hdcForeMask, hOld));
  DeleteObject(SelectObject(hdcBackMask, hOld));
  DeleteObject(SelectObject(hdcMixBuffer, hOld));

  DeleteDC(hdcCopy);
  DeleteDC(hdcForeMask);
  DeleteDC(hdcBackMask);
  DeleteDC(hdcMixBuffer);

end;

function TagCount(Source: WideString; Tag: WideChar): Integer;
var
  I, tCount: Integer;
begin
  tCount := 0;
  for I := 1 to Length(Source) do
    if Source[I] = Tag then
      Inc(tCount);
  Result := tCount;
end;

{ "xxxxxx" => xxxxxx }

function TakeOffTag(Src: string; Tag: Char; var rstr: string): string;
var
  N2: Integer;
begin
  N2 := Pos(Tag, Copy(Src, 2, Length(Src)));
  rstr := Copy(Src, 2, N2 - 1);
  Result := Copy(Src, N2 + 2, Length(Src) - N2);
end;

function CatchString(Source: string; cap: Char; var catched: string): string;
var
  n: Integer;
begin
  Result := '';
  catched := '';
  if Source = '' then
    Exit;
  if Length(Source) < 2 then
  begin
    Result := Source;
    Exit;
  end;
  if Source[1] = cap then
  begin
    if Source[2] = cap then // ##abc#
      Source := Copy(Source, 2, Length(Source));
    if TagCount(Source, WideChar(cap)) >= 2 then
    begin
      Result := TakeOffTag(Source, cap, catched);
    end
    else
      Result := Source;
  end
  else
  begin
    if TagCount(Source, WideChar(cap)) >= 2 then
    begin
      n := Pos(cap, Source);
      Source := Copy(Source, n, Length(Source));
      Result := TakeOffTag(Source, cap, catched);
    end
    else
      Result := Source;
  end;
end;

{ GetValidStr3客 崔府 侥喊磊啊 楷加栏肺 唱棵版快 贸府 救凳 }
{ 侥喊磊啊 绝阑 版快, nil 府畔.. }

function DivString(Source: string; cap: Char; var sel: string): string;
var
  n: Integer;
begin
  if Source = '' then
  begin
    sel := '';
    Result := '';
    Exit;
  end;
  n := Pos(cap, Source);
  if n > 0 then
  begin
    sel := Copy(Source, 1, n - 1);
    Result := Copy(Source, n + 1, Length(Source));
  end
  else
  begin
    sel := Source;
    Result := '';
  end;
end;

function DivTailString(Source: string; cap: Char; var sel: string): string;
var
  I, n: Integer;
begin
  if Source = '' then
  begin
    sel := '';
    Result := '';
    Exit;
  end;
  n := 0;
  for I := Length(Source) downto 1 do
    if Source[I] = cap then
    begin
      n := I;
      Break;
    end;
  if n > 0 then
  begin
    sel := Copy(Source, n + 1, Length(Source));
    Result := Copy(Source, 1, n - 1);
  end
  else
  begin
    sel := '';
    Result := Source;
  end;
end;


function SPos(substr, Str: string): Integer;
var
  I, J, len, slen: Integer;
  flag: Boolean;
begin
  Result := -1;
  len := Length(Str);
  slen := Length(substr);
  for I := 0 to len - slen do
  begin
    flag := True;
    for J := 1 to slen do
    begin
      if Byte(Str[I + J]) >= $B0 then
      begin
        if (J < slen) and (I + J < len) then
        begin
          if substr[J] <> Str[I + J] then
          begin
            flag := False;
            Break;
          end;
          if substr[J + 1] <> Str[I + J + 1] then
          begin
            flag := False;
            Break;
          end;
        end
        else
          flag := False;
      end
      else if substr[J] <> Str[I + J] then
      begin
        flag := False;
        Break;
      end;
    end;
    if flag then
    begin
      Result := I + 1;
      Break;
    end;
  end;
end;

function NumCopy(Str: string): Integer;
var
  I: Integer;
  Data: string;
begin
  Data := '';
  for I := 1 to Length(Str) do
  begin
    if (Word('0') <= Word(Str[I])) and (Word('9') >= Word(Str[I])) then
    begin
      Data := Data + Str[I];
    end
    else
      Break;
  end;
  Result := Str_ToInt(Data, 0);
end;

function GetMonDay: string;
var
  Year, mon, Day: Word;
  Str: string;
begin
  DecodeDate(Date, Year, mon, Day);
  Str := IntToStr(Year);
  if mon < 10 then
    Str := Str + '0' + IntToStr(mon)
  else
    Str := IntToStr(mon);
  if Day < 10 then
    Str := Str + '0' + IntToStr(Day)
  else
    Str := IntToStr(Day);
  Result := Str;
end;

function BoolToStr(boo: Boolean): string;
begin
  if boo then
    Result := 'TRUE'
  else
    Result := 'FALSE';
end;

function BoolToStr2(boo: Boolean): string;
begin
  if boo then
    Result := '1'
  else
    Result := '0';
end;

function BooleanToStr(boo: Boolean): string;
begin
  if boo then
    Result := '是'
  else
    Result := '否';
end;

function _MIN(N1, N2: Integer): Integer;
begin
  if N1 < N2 then
    Result := N1
  else
    Result := N2;
end;

function _MAX(N1, N2: Integer): Integer;
begin
  if N1 > N2 then
    Result := N1
  else
    Result := N2;
end;

function _MinLong(N1, N2: LongWord): LongWord;
begin
  if N1 < N2 then
    Result := N1
  else
    Result := N2;
end;

function _MaxLong(N1, N2: LongWord): LongWord;
begin
  if N1 > N2 then
    Result := N1
  else
    Result := N2;
end;

procedure DisPoseAndNil(var Obj);
var
  Temp: Pointer;
begin
  Temp := Pointer(Obj);
  Pointer(Obj) := nil;
  Dispose(Temp);
end;

end.
