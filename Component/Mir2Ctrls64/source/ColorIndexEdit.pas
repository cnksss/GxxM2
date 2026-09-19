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

    //FCanvas: TCanvas; //DC句柄数是有限资源，动态申请DC更好 //HZQ

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

  //FCanvas := TControlCanvas.Create;
  //TControlCanvas(FCanvas).Control := Self;
  //FCanvas.Font.Assign(Font);
end;

destructor TColorIndexEdit.Destroy;
begin
  //FCanvas.Free;
  inherited;
end;

procedure TColorIndexEdit.CreateWnd;
begin
  inherited;
  SetEditRect;
end;

procedure TColorIndexEdit.SetColorWidth(const Value: Integer);
begin
  if FColorWidth <> Value then begin
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

    //SendMessage(Handle, EM_SETRECT, 0, Longint(@R));  //这个地方在64位会引起崩溃，应该使用原生整型存储指针 HZQ 2023-03-16
    SendMessage(Handle, EM_SETRECT, 0, NativeInt(@R));
    Invalidate;
  end;

end;


procedure TColorIndexEdit.WMPaint(var Message: TWMPaint);
var
  DC: HDC;
  R: TRect;
  OldColor: TColor;
  TextSize: TSize;
begin
  inherited;
  DC := GetDC(Handle);
  try
      R.Left := 2;
      R.Right := FColorWidth - 2 + LeftMargin;
      R.Top := ClientRect.Top + 2;
      R.Bottom := ClientRect.Bottom - 2;

      if Value >= 0 then begin
        { 画外面边框色 }
        OldColor := Brush.Color;
        Brush.Color := clBlack;
        Windows.FillRect(DC, R, Brush.Handle);

        { 画中间填充色 }
        Windows.InflateRect(R, -1, -1);
        Brush.Color := FColorValue;
        Windows.FillRect(DC, R, Brush.Handle);
        Brush.Color := OldColor;
      end else begin
        OldColor := Brush.Color;

        Brush.Color := Color;
        Windows.FillRect(DC, ClientRect, Brush.Handle);


        Windows.GetTextExtentPointA(DC, '无颜色', 6, TextSize);

        Brush.Color := Font.Color;
        Windows.TextOut(DC, 0, (ClientRect.Bottom - ClientRect.Top - TextSize.cy) div 2, '无颜色', 6);
        Brush.Color := OldColor;
      end;
  finally
      ReleaseDC(Handle, DC);
  end;
end;

(*
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
*)

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
