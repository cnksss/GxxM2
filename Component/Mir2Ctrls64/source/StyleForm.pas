unit StyleForm;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Mir2Common;

type
  TStyleForm = class(TPersistent)
  private
    FOwner: TForm;
    FStyleImage: TBitmap;

    FStretchPoint: TPoint;
    FHorzStretchType: THorzStretchType;
    FVertStretchType: TVertStretchType;

    FOnChange: TNotifyEvent;
    procedure SetStyleImage(const Value: TBitmap);
    procedure SetHorzStretchPoint(const Value: Integer);
    procedure SetHorzStretchType(const Value: THorzStretchType);
    procedure SetVertStretchPoint(const Value: Integer);
    procedure SetVertStretchType(const Value: TVertStretchType);
  private
    procedure StyleImageChange(Sender: TObject);

    property OnChange: TNotifyEvent read FOnChange write FOnChange;
  protected
    procedure AssignTo(Dest: TPersistent); override;
  public
    constructor Create(AOwner: TForm);
    destructor Destroy; override;
  published
    property StyleImage: TBitmap read FStyleImage write SetStyleImage;

    property HorzStretchPoint: Integer read FStretchPoint.X write SetHorzStretchPoint default 0;
    property VertStretchPoint: Integer read FStretchPoint.Y write SetVertStretchPoint default 0;

    property HorzStretchType: THorzStretchType read FHorzStretchType write SetHorzStretchType default hstNone;
    property VertStretchType: TVertStretchType read FVertStretchType write SetVertStretchType default vstNone;
  end;

  TCustomStyleForm = class(TForm)
  private
{$IF CompilerVersion < 18.5}
    FPadding: TMargins;
{$IFEND}
    FStyle: TStyleForm;
    FBorderColor: TColor;
    FEnabledEraseBkgnd: Boolean;

    FMemBit: TBitmap;

{$IF CompilerVersion < 18.5}
    procedure SetPadding(const Value: TMargins);
    procedure OnPaddingChanged(Sender: TObject);
{$IFEND}

    procedure SetStyle(const Value: TStyleForm);
    procedure OnStyleChanged(Sender: TObject);

    procedure WMNCCalcSize(var Message: TWMNCCalcSize); message WM_NCCALCSIZE;
    procedure WMNCPaint(var Message: TWMNCPaint); message WM_NCPAINT;
    procedure WMPaint(var Message: TWMPaint); message WM_PAINT;
    procedure WMEraseBkgnd(var Message: TWmEraseBkgnd); message WM_ERASEBKGND;
    procedure SetBorderColor(const Value: TColor);
    procedure SetEnabledEraseBkgnd(const Value: Boolean);
  protected
    procedure AdjustClientRect(var Rect: TRect); override;
    function GetClientRect: TRect; override;
    procedure AlignControls(AControl: TControl; var Rect: TRect);  override;

    procedure CreateParams(var Params: TCreateParams); override;

    procedure PaintBorder(Rgn: HRGN = 1);
    procedure PaintStyleForm(DC: HDC);

    procedure Paint; override;

    procedure PaintWindow(DC: HDC); override;
  public
    { Public declarations }
    constructor CreateNew(AOwner: TComponent; Dummy: Integer = 0); override;
    destructor Destroy; override;
  published
{$IF CompilerVersion < 18.5}
    property Padding: TMargins read FPadding write SetPadding;
{$IFEND}
    property Style: TStyleForm read FStyle write SetStyle;
    property BorderColor: TColor read FBorderColor write SetBorderColor default clBlack;
    property EnabledEraseBkgnd: Boolean read FEnabledEraseBkgnd write SetEnabledEraseBkgnd default True;
  end;

implementation

{ TStyleForm }

procedure TStyleForm.AssignTo(Dest: TPersistent);
begin
  if Dest is TStyleForm then
    with TStyleForm(Dest) do
    begin
      FStyleImage.Assign(Self.FStyleImage);
    end
  else inherited AssignTo(Dest);
end;

constructor TStyleForm.Create(AOwner: TForm);
begin
  FOwner := AOwner;
  FStyleImage := TBitmap.Create;
  FStyleImage.OnChange := StyleImageChange;
end;

destructor TStyleForm.Destroy;
begin
  FStyleImage.Free;
  inherited;
end;

procedure TStyleForm.SetHorzStretchPoint(const Value: Integer);
begin
  if FStretchPoint.X <> Value then
  begin
    FStretchPoint.X := Value;
    FOwner.Invalidate;
  end;
end;

procedure TStyleForm.SetHorzStretchType(const Value: THorzStretchType);
begin
  if FHorzStretchType <> Value then
  begin
    FHorzStretchType := Value;
    FOwner.Invalidate;
  end;
end;

procedure TStyleForm.SetVertStretchPoint(const Value: Integer);
begin
  if FStretchPoint.Y <> Value then
  begin
    FStretchPoint.Y := Value;
    FOwner.Invalidate;
  end;
end;

procedure TStyleForm.SetVertStretchType(const Value: TVertStretchType);
begin
  if FVertStretchType <> Value then
  begin
    FVertStretchType := Value;
    FOwner.Invalidate;
  end;
end;

procedure TStyleForm.SetStyleImage(const Value: TBitmap);
begin
  FStyleImage.Assign(Value);
end;

procedure TStyleForm.StyleImageChange(Sender: TObject);
begin
  if Sender = FStyleImage then
  begin
    if Assigned(FOnChange) then
      FOnChange(Self);
  end;
end;

{ TFrmCustomTitle }

constructor TCustomStyleForm.CreateNew(AOwner: TComponent; Dummy: Integer = 0);
begin
  inherited;
{$IF CompilerVersion < 18.5}
  FPadding := TMargins.Create(Self);
  FPadding.OnChange := OnPaddingChanged;
{$IFEND}

  FMemBit := TBitmap.Create;
  FStyle := TStyleForm.Create(Self);
  FStyle.OnChange := OnStyleChanged;

  BorderWidth := 0;
  FBorderColor := clBlack;
  //BorderStyle := bsNone;
end;

destructor TCustomStyleForm.Destroy;
begin
  inherited;
{$IF CompilerVersion < 18.5}
  FPadding.Free;
{$IFEND}
  FMemBit.Free;
  FStyle.Free;
end;

{$IF CompilerVersion < 18.5}
procedure TCustomStyleForm.SetPadding(const Value: TMargins);
begin
  FPadding.Assign(Value);
  PaintStyleForm(Canvas.Handle);
end;

procedure TCustomStyleForm.OnPaddingChanged(Sender: TObject);
begin
  Realign;
  Invalidate;
end;
{$IFEND}

procedure TCustomStyleForm.SetStyle(const Value: TStyleForm);
begin
  FStyle.Assign(Value);
end;

procedure TCustomStyleForm.AdjustClientRect(var Rect: TRect);
begin
  inherited AdjustClientRect(Rect);
{$IF CompilerVersion < 18.5}
  Inc(Rect.Left, Padding.Left);
  Inc(Rect.Top, Padding.Top);
  Dec(Rect.Right, Padding.Right);
  Dec(Rect.Bottom, Padding.Bottom);
{$IFEND}
end;

function TCustomStyleForm.GetClientRect: TRect;
begin
  Result := inherited GetClientRect;
{
  // 调用本方法后，Padding区域为非客户区，在设计窗体的时候，很不方便
  // 因此采用 AlignControls 这种方式来修改
  Inc(Result.Left, Padding.Left);
  Inc(Result.Top, Padding.Top);
  Dec(Result.Right, Padding.Right);
  Dec(Result.Bottom, Padding.Bottom);
}
end;

procedure TCustomStyleForm.AlignControls(AControl: TControl;
  var Rect: TRect);
begin
{$IF CompilerVersion < 18.5}
  Inc(Rect.Left, Padding.Left);
  Inc(Rect.Top, Padding.Top);
  Dec(Rect.Right, Padding.Right);
  Dec(Rect.Bottom, Padding.Bottom);
{$IFEND}
  inherited AlignControls(AControl, Rect);
end;

procedure TCustomStyleForm.WMNCCalcSize(var Message: TWMNCCalcSize);
begin
  inherited;
  {
  不用本方法，本方法用了后，非客户端区域不能放控件
  with Message.CalcSize_Params^ do
  begin
    Inc(rgrc[0].Left, Padding.Left);
    Inc(rgrc[0].Top, Padding.Top);
    Dec(rgrc[0].Right, Padding.Right);
    Dec(rgrc[0].Bottom, Padding.Bottom);
  end;
  }
end;

procedure TCustomStyleForm.OnStyleChanged(Sender: TObject);
begin
  PaintStyleForm(Canvas.Handle);
end;

procedure TCustomStyleForm.PaintBorder(Rgn: HRGN);
var
  Flags: DWORD;
  IsWinNT: Boolean;
  WR, BR, CR: TRect;
  WinDC: HDC;
  OldColor: TColor;
  Pt: TPoint;
  WinStyle: Integer;
begin
  if (BorderWidth = 0) then Exit;

  { ClientRect 应偏移标题栏及窗体边框的边距 }
  Windows.GetClientRect(Handle, CR);
  Windows.GetWindowRect(Handle, WR);
  Pt := ClientToScreen(CR.TopLeft);
  OffsetRect(CR, Pt.X - WR.Left, Pt.Y - WR.Top);

  WinStyle := GetWindowLong(Handle, GWL_STYLE);
  if (WinStyle and WS_VSCROLL) <> 0 then
    Inc(CR.Right, GetSystemMetrics(SM_CXVSCROLL));

  if (WinStyle and WS_HSCROLL) <> 0 then
    Inc(CR.Bottom, GetSystemMetrics(SM_CXHSCROLL));

  BR := CR;
  InflateRect(BR, BorderWidth, BorderWidth);

  Flags := DCX_CACHE or DCX_CLIPSIBLINGS or DCX_WINDOW or DCX_VALIDATE;
  IsWinNT := Win32Platform and VER_PLATFORM_WIN32_NT <> 0;

  if (Rgn = 1) or not IsWinNT then
    WinDC := GetDCEx(Handle, 0, Flags)
  else
    WinDC := GetDCEx(Handle, Rgn, Flags or DCX_INTERSECTRGN);

  if WinDC = 0 then Exit;

  IntersectClipRect(WinDC, BR.Left, BR.Top, BR.Right, BR.Bottom);
  ExcludeClipRect(WinDC, CR.Left, CR.Top, CR.Right, CR.Bottom);

  OldColor := Brush.Color;
  Brush.Color := FBorderColor;
  Windows.FillRect(WinDC, BR, Brush.Handle);
  Brush.Color := OldColor;
end;

procedure TCustomStyleForm.PaintStyleForm(DC: HDC);
var
  BR, CR, DR: TRect;

  Size: TPoint;
  //OldRgn, DrawRgn: HRGN;
begin
  if (Padding.Left = 0) and (Padding.Top = 0) and
    (Padding.Right = 0) and (Padding.Bottom = 0) then Exit;

  BR := ClientRect;
  CR := BR;
  Inc(CR.Left, Padding.Left);
  Dec(CR.Right, Padding.Right);
  Inc(CR.Top, Padding.Top);
  Dec(CR.Bottom, Padding.Bottom);

  //DrawRgn := CreateRectRgn(BR.Left, BR.Top, BR.Right, BR.Bottom);
  //OldRgn := SelectClipRgn(Canvas.Handle, DrawRgn);
  //ExcludeClipRect(Canvas.Handle, CR.Left, CR.Top, CR.Right, CR.Bottom);

  FMemBit.Width := BR.Right - BR.Left;
  FMemBit.Height := BR.Bottom - BR.Top;

  if (Style.StyleImage.Width > 0) and (Style.StyleImage.Height > 0) then
  begin
    Size.X := BR.Right - BR.Left;
    Size.Y := BR.Bottom - BR.Top;
    StretchPicture(FStyle.FStyleImage, FMemBit.Canvas.Handle, Size,
      Point(-BorderWidth, -BorderWidth),
      FStyle.HorzStretchType, FStyle.VertStretchType, FStyle.FStretchPoint)
  end
  else
  begin
    FMemBit.Canvas.Brush.Style := bsSolid;
    FMemBit.Canvas.Brush.Color := FBorderColor;
    FMemBit.Canvas.FillRect(BR);

    FMemBit.Canvas.Brush.Color := Color;
    FMemBit.Canvas.FillRect(CR);
  end;

  if csDesigning in ComponentState then
  begin
    if Padding.Top > 0 then
    begin
      DR := Rect(0, 0, BR.Right, Padding.Top);
      Bitblt(DC, DR.Left, DR.Top, DR.Right - DR.Left, DR.Bottom - DR.Top, FMemBit.Canvas.Handle, DR.Left, DR.Top, SRCCOPY);
    end;

    if Padding.Bottom > 0 then
    begin
      DR := Rect(0, BR.Bottom - Padding.Bottom, BR.Right, BR.Bottom);
      Bitblt(DC, DR.Left, DR.Top, DR.Right - DR.Left, DR.Bottom - DR.Top, FMemBit.Canvas.Handle, DR.Left, DR.Top, SRCCOPY);
    end;

    if Padding.Left > 0 then
    begin
      DR := Rect(0, Padding.Top, Padding.Left, BR.Bottom - Padding.Bottom);
      Bitblt(DC, DR.Left, DR.Top, DR.Right - DR.Left, DR.Bottom - DR.Top, FMemBit.Canvas.Handle, DR.Left, DR.Top, SRCCOPY);
    end;

    if Padding.Right > 0 then
    begin
      DR := Rect(BR.Right - Padding.Right, Padding.Top, BR.Right, BR.Bottom - Padding.Bottom);
      Bitblt(DC, DR.Left, DR.Top, DR.Right - DR.Left, DR.Bottom - DR.Top, FMemBit.Canvas.Handle, DR.Left, DR.Top, SRCCOPY);
    end;
  end
  else
    Bitblt(DC, 0, 0, FMemBit.Width, FMemBit.Height, FMemBit.Canvas.Handle, 0, 0, SRCCOPY);

  //SelectClipRgn(Canvas.Handle, OldRgn);
  //IntersectClipRect(Canvas.Handle, BR.Left, BR.Top, BR.Right, BR.Bottom);
  //DeleteObject(DrawRgn);
end;


procedure TCustomStyleForm.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
  Params.WindowClass.style := Params.WindowClass.style or CS_HREDRAW or CS_VREDRAW;
end;

procedure TCustomStyleForm.WMNCPaint(var Message: TWMNCPaint);
begin
  inherited;
  //DefaultHandler(Message);
  PaintBorder(Message.RGN);
end;

procedure TCustomStyleForm.PaintWindow(DC: HDC);
begin
  inherited;
  PaintStyleForm(DC);
end;

procedure TCustomStyleForm.WMPaint(var Message: TWMPaint);
begin
  inherited;
{
  if csDesigning in ComponentState then
  begin
    inherited;
    PaintStyleForm(Canvas.Handle);
  end
  else
  begin
    PaintStyleForm(Canvas.Handle);
    inherited;
  end;
}
end;

procedure TCustomStyleForm.Paint;
begin
  inherited;
end;

procedure TCustomStyleForm.SetBorderColor(const Value: TColor);
begin
  if FBorderColor <> Value then
  begin
    FBorderColor := Value;
    Invalidate;
  end;
end;

procedure TCustomStyleForm.SetEnabledEraseBkgnd(const Value: Boolean);
begin
  if FEnabledEraseBkgnd <> Value then
  begin
    FEnabledEraseBkgnd := Value;
    Invalidate;
  end;
end;

procedure TCustomStyleForm.WMEraseBkgnd(var Message: TWmEraseBkgnd);
begin
  if not FEnabledEraseBkgnd then
  begin
    if ((Padding.Left > 0) or (Padding.Top > 0) or (Padding.Right > 0) or (Padding.Bottom > 0))
      and (not (csDesigning in ComponentState)) then
      Message.Result := 1
    else
      Inherited;
  end
  else
    Inherited;
end;

(*
procedure TCustomStyleForm.PaintStyleForm(Rgn: HRGN);
var
  Flags: DWORD;
  IsWinNT: Boolean;
  WR, BR, CR: TRect;
  WinDC: HDC;
  OldColor: TColor;
  Pt: TPoint;
  Bitmap: TBitmap;
  WinStyle: Integer;

  Size: TPoint;
begin
  if (BorderWidth = 0) and (Padding.Left = 0) and (Padding.Top = 0) and
    (Padding.Right = 0) and (Padding.Bottom = 0) then Exit;

  { ClientRect 应偏移标题栏及窗体边框的边距 }
  Windows.GetClientRect(Handle, CR);
  Windows.GetWindowRect(Handle, WR);
  Pt := ClientToScreen(CR.TopLeft);
  OffsetRect(CR, Pt.X - WR.Left, Pt.Y - WR.Top);

  WinStyle := GetWindowLong(Handle, GWL_STYLE);
  if (WinStyle and WS_VSCROLL) <> 0 then
    Inc(CR.Right, GetSystemMetrics(SM_CXVSCROLL));

  if (WinStyle and WS_HSCROLL) <> 0 then
    Inc(CR.Bottom, GetSystemMetrics(SM_CXHSCROLL));

  BR := CR;
  InflateRect(BR, BorderWidth, BorderWidth);
  Inc(CR.Left, Padding.Left);
  Dec(CR.Right, Padding.Right);
  Inc(CR.Top, Padding.Top);
  Dec(CR.Bottom, Padding.Bottom);

  {
  BR := CR;
  
  InflateRect(BR, BorderWidth, BorderWidth);
  Dec(BR.Left, Padding.Left);
  Inc(BR.Right, Padding.Right);
  Dec(BR.Top, Padding.Top);
  Inc(BR.Bottom, Padding.Bottom);
  }

  Flags := DCX_CACHE or DCX_CLIPSIBLINGS or DCX_WINDOW or DCX_VALIDATE;
  IsWinNT := Win32Platform and VER_PLATFORM_WIN32_NT <> 0;

  if (Rgn = 1) or not IsWinNT then
    WinDC := GetDCEx(Handle, 0, Flags)
  else
    WinDC := GetDCEx(Handle, Rgn, Flags or DCX_INTERSECTRGN);

  if WinDC = 0 then Exit;

  IntersectClipRect(WinDC, BR.Left, BR.Top, BR.Right, BR.Bottom);
  ExcludeClipRect(WinDC, CR.Left, CR.Top, CR.Right, CR.Bottom);

  Bitmap := Style.StyleImage;
  if (Bitmap.Width > 0) and (Bitmap.Height > 0) then
  begin
    //StretchBlt(WinDC, 0, 0, Width, Height, Bitmap.Canvas.Handle, 0, 0, Bitmap.Width, Bitmap.Height, SRCCOPY);
    Size.X := BR.Right - BR.Left;
    Size.Y := BR.Bottom - BR.Top;

    StretchPicture(FStyle.FStyleImage, WinDC, Size, BR.TopLeft, FStyle.HorzStretchType, FStyle.VertStretchType, FStyle.FStretchPoint)
  end
  else
  begin
    OldColor := Brush.Color;
    Brush.Color := FBorderColor;
    Windows.FillRect(WinDC, BR, Brush.Handle);
    Brush.Color := OldColor;
  end;

  ReleaseDC(Handle, WinDC);
end;
*)

end.
