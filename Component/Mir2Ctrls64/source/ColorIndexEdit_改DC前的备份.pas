{*******************************************************}
{                                                       }
{       创    建： chongchong                           }
{       版    本： 1.0                                  }
{       创建日期： 2013-07-20                           }
{                                                       }
{       单元说明： 索引颜色控件                         }
{                                                       }
{       版权所有 (C) 2013 GeeM2                         }
{                                                       }
{*******************************************************}


unit ColorIndexEdit;


interface

uses
  Windows, SysUtils, Classes, Messages, Spin, Graphics, Controls;

type
  TColorIndexEdit = class(TSpinEdit)
  private
    FColorWidth: Integer;
    FColorValue: Integer;
    FShowNoneColor: Boolean;

    FCanvas: TCanvas;

    procedure SetEditRect;
    procedure SetColorWidth(const Value: Integer);
    function CheckValue (NewValue: LongInt): LongInt;

    procedure WMSize(var Message: TWMSize); message WM_SIZE;
    procedure WMPaint(var Message: TWMPaint); message WM_PAINT;
    function GetValue: LongInt;
    procedure SetValue(const Value: LongInt);
    procedure SetShowNoneColor(const Value: Boolean);
  protected
    procedure CreateWnd; override;
    procedure Change; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
  published
    property ColorWidth: Integer read FColorWidth write SetColorWidth default 16;
    property Value: LongInt read GetValue write SetValue;
    property ShowNoneColor: Boolean read FShowNoneColor write SetShowNoneColor;
  end;

implementation

(*
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
  Result := RGB(ColorTable.rgbRed, ColorTable.rgbGreen,ColorTable.rgbBlue);
end;
*)

function ColorIndexToTColor(ColorIndex: Byte): TColor;
const
  ColorArray: array[Byte] of TColor = (
      $000000, $000080, $008000, $008080, $800000, $800080, $808000, $C0C0C0,
      $978055, $C8B99D, $73737B, $29292D, $52525A, $5A5A63, $393942, $18181D,
      $101018, $181829, $080810, $7179F2, $5F67E1, $5A5AFF, $3131FF, $525AD6,
      $001094, $182994, $000839, $001073, $0018B5, $5263BD, $101842, $99AAFF,
      $00105A, $293973, $314AA5, $737B94, $3152BD, $102152, $18317B, $10182D,
      $314A8C, $002994, $0031BD, $5273C6, $18316B, $426BC6, $004ACE, $3963A5,
      $18315A, $00102A, $000815, $00183A, $000008, $000029, $00004A, $00009D,
      $0000DC, $0000DE, $0000FB, $52739C, $4A6B94, $294A73, $183152, $184A8C,
      $114488, $00214A, $101821, $5A94D6, $216BC6, $006BEF, $0077FF, $8494A5,
      $213142, $081018, $081829, $001021, $182939, $39638C, $102942, $18426B,
      $184A7B, $004A94, $7B848C, $5A636B, $39424A, $182129, $293946, $94A5B5,
      $5A6B7B, $94B1CE, $738CA5, $5A738C, $7394B5, $73A5D6, $4AA5EF, $8CC6EF,
      $42637B, $39566B, $5A94BD, $003963, $ADC6D6, $294252, $186394, $ADD6EF,
      $638CA5, $4A5A63, $7BA5BD, $18425A, $318CBD, $293135, $638494, $4A6B7B,
      $5A8CA5, $294A5A, $397B9C, $103142, $21ADEF, $001018, $002129, $006B9C,
      $5A8494, $184252, $295A6B, $21637B, $217B9C, $00A5DE, $39525A, $102931,
      $7BBDCE, $395A63, $4A8494, $29A5C6, $189C10, $4A8C42, $428C31, $299410,
      $101808, $181808, $102908, $294218, $ADB5A5, $73736B, $292918, $4A4218,
      $4A4231, $DEC663, $FFDD44, $EFD68C, $396B73, $39DEF7, $8CEFF7, $00E7F7,
      $5A6B6B, $A58C5A, $EFB539, $CE9C4A, $B58431, $6B5231, $D6DEDE, $B5BDBD,
      $848C8C, $DEF7F7, $180800, $391808, $291008, $001808, $002908, $A55200,
      $DE7B00, $4A2910, $6B3910, $8C5210, $A55A21, $5A3110, $844210, $845231,
      $312118, $7B5A4A, $A56B52, $633929, $DE4A10, $212929, $394A4A, $182929,
      $294A4A, $427B7B, $4A9C9C, $295A5A, $144242, $003939, $005959, $2C35CA,
      $21736B, $003129, $103931, $183931, $004A42, $186352, $29735A, $184A31,
      $002118, $003118, $103918, $4A8463, $4ABD6B, $4AB563, $4ABD63, $4A9C5A,
      $398C4A, $4AC663, $4AD663, $4A8452, $297331, $5AC663, $4ABD52, $00FF10,
      $182918, $4A884A, $4AE74A, $005A00, $008800, $009400, $00DE00, $00EE00,
      $00FB00, $945A4A, $B57363, $D68C7B, $D67B6B, $FF8877, $CEC6C6, $9C9494,
      $C6949C, $393131, $841829, $840018, $52424A, $7B4252, $735A63, $F7B5CE,
      $9C7B8C, $CC2277, $FFAADD, $2AB4F0, $9F00DF, $B317E3, $F0FBFF, $A4A0A0,
      $808080, $0000FF, $00FF00, $00FFFF, $FF0000, $FF00FF, $FFFF00, $FFFFFF
    );
begin
  Result := ColorArray[ColorIndex];
end;

const
  LeftMargin = 1;

{ TColorIndexEdit }

constructor TColorIndexEdit.Create(AOwner: TComponent);
begin
  inherited;
  MinValue := 0;
  MaxValue := 255;
  MaxLength := 3;

  Value := 0;
  FColorValue := ColorIndexToTColor(Value);

  FColorWidth := 16;

  FShowNoneColor := False;

  FCanvas := TControlCanvas.Create;
  TControlCanvas(FCanvas).Control := Self;
  FCanvas.Font.Assign(Font);
end;

destructor TColorIndexEdit.Destroy;
begin
  FCanvas.Free;
  inherited;
end;

procedure TColorIndexEdit.CreateWnd;
begin
  inherited;
  SetEditRect;
end;

procedure TColorIndexEdit.SetColorWidth(const Value: Integer);
begin
  if FColorWidth <> Value then
  begin
    FColorWidth := Value;
    SetEditRect;
  end;
end;

procedure TColorIndexEdit.SetEditRect;
var
  R: TRect;
begin
  //if HandleAllocated then
  begin
    R := ClientRect;
    Inc(R.Left);
    Dec(R.Right);

    R.Left := R.Left + FColorWidth + LeftMargin - 1;
    InflateRect(R, 0, 0);

    SendMessage(Handle, EM_SETRECT, 0, Longint(@R));
    Invalidate;
  end;

end;

(*
procedure TColorIndexEdit.WMPaint(var Message: TWMPaint);
var
  DC: HDC;
  R: TRect;
  OldColor: TColor;
  TextSize: TSize;
begin
  inherited;
  DC := GetDC(Handle);
  R.Left := 2;
  R.Right := FColorWidth - 2 + LeftMargin;
  R.Top := ClientRect.Top + 2;
  R.Bottom := ClientRect.Bottom - 2;

  if Value >= 0 then
  begin
    { 画外面边框色 }
    OldColor := Brush.Color;
    Brush.Color := clBlack;
    Windows.FillRect(DC, R, Brush.Handle);

    { 画中间填充色 }
    Windows.InflateRect(R, -1, -1);
    Brush.Color := FColorValue;
    Windows.FillRect(DC, R, Brush.Handle);
    Brush.Color := OldColor;
  end
  else
  begin
    OldColor := Brush.Color;

    Brush.Color := Color;
    Windows.FillRect(DC, ClientRect, Brush.Handle);


    Windows.GetTextExtentPointA(DC, '无颜色', 6, TextSize);

    Brush.Color := Font.Color;
    Windows.TextOut(DC, 0, (ClientRect.Bottom - ClientRect.Top - TextSize.cy) div 2, '无颜色', 6);
    Brush.Color := OldColor;
  end;

  ReleaseDC(Handle, DC);
end;
*)

procedure TColorIndexEdit.WMPaint(var Message: TWMPaint);
var
  R: TRect;

  OldPenStyle: TPenStyle;
  OldPenColor: TColor;
  OldBrushStyle: TBrushStyle;
  OldBrushColor: TColor;
begin
  inherited;
  R.Left := 2;
  R.Right := FColorWidth - 2 + LeftMargin;
  R.Top := ClientRect.Top + 2;
  R.Bottom := ClientRect.Bottom - 2;

  OldPenStyle := FCanvas.Pen.Style;
  OldPenColor := FCanvas.Pen.Color;
  OldBrushStyle := FCanvas.Brush.Style;
  OldBrushColor := FCanvas.Brush.Color;

  if Value >= 0 then
  begin
    { 画外面边框色 }
    FCanvas.Brush.Color := clBlack;
    FCanvas.FillRect(R);

    { 画中间填充色 }
    Windows.InflateRect(R, -1, -1);
    FCanvas.Brush.Color := FColorValue;
    FCanvas.FillRect(R);
  end
  else
  begin
    { 画外面边框色 }
    FCanvas.Pen.Style := psSolid;
    FCanvas.Pen.Color := clBtnFace;
    FCanvas.Brush.Style := bsClear;
    FCanvas.Rectangle(R);

    InflateRect(R, -2, -2);

    FCanvas.Pen.Color := clBtnFace;
    FCanvas.Brush.Color := clBtnFace;
    FCanvas.Brush.Style := bsBDiagonal;
    FCanvas.Rectangle(R);
  end;

  FCanvas.Pen.Style := OldPenStyle;
  FCanvas.Pen.Color := OldPenColor;
  FCanvas.Brush.Style := OldBrushStyle;
  FCanvas.Brush.Color := OldBrushColor;
end;

procedure TColorIndexEdit.WMSize(var Message: TWMSize);
begin
  inherited;
  SetEditRect;
end;

procedure TColorIndexEdit.Change;
var
  V: Integer;
begin
  inherited;
  if Length(Text) = 0 then
    V := 0
  else
    V := Value;

  if Value >= 0 then
    FColorValue := ColorIndexToTColor(V);
  Invalidate;
end;

function TColorIndexEdit.GetValue: LongInt;
begin
  Result := StrToIntDef(Text, 0);
end;

procedure TColorIndexEdit.SetValue(const Value: LongInt);
begin
  Text := IntToStr(CheckValue(Value));
end;

function TColorIndexEdit.CheckValue(NewValue: Integer): LongInt;
begin
  Result := NewValue;
  if (MaxValue <> MinValue) then
  begin
    if NewValue < MinValue then
      Result := MinValue
    else if NewValue > MaxValue then
      Result := MaxValue;
  end;
end;

procedure TColorIndexEdit.SetShowNoneColor(const Value: Boolean);
begin
  FShowNoneColor := Value;
  if FShowNoneColor then
    MinValue := -1
  else
    MinValue := 0;
end;

end.
