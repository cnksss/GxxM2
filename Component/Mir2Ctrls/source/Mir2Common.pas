unit Mir2Common;

interface

uses
  Windows, Classes, SysUtils, Controls, Graphics;

type
{$IF CompilerVersion < 18.5}
  TVerticalAlignment = (
    taAlignTop,
    taAlignBottom,
    taVerticalCenter
  );

  TMarginSize = 0..MaxInt;
  TMargins = class(TPersistent)
  private
    FControl: TControl;
    FLeft, FTop, FRight, FBottom: TMarginSize;
    FOnChange: TNotifyEvent;
    procedure SetMargin(Index: Integer; Value: TMarginSize);
  protected
    procedure Change; virtual;
    procedure AssignTo(Dest: TPersistent); override;
    property Control: TControl read FControl;
  public
    constructor Create(Control: TControl); virtual;
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
  published
    property Left: TMarginSize index 0 read FLeft write SetMargin default 0;
    property Top: TMarginSize index 1 read FTop write SetMargin default 0;
    property Right: TMarginSize index 2 read FRight write SetMargin default 0;
    property Bottom: TMarginSize index 3 read FBottom write SetMargin default 0;
  end;
{$IFEND}

  THorzStretchType = (hstNone, hstAll, hstLeft, hstMiddle, hstRight, hstCustom);
  TVertStretchType = (vstNone, vstAll, vstTop, vstCenter, vstBottom, vstCustom);
  TDrawStyle = (dsStretch, dsTile);

  procedure StretchPicture(const Source: Tbitmap; Dest: HDC; DestSize: TPoint;
    DestOffset: TPoint;
    HorzStretchType: THorzStretchType; VertStretchType: TVertStretchType;
    StretchPt: TPoint);

implementation

{$IF CompilerVersion < 18.5}

{ TMargins }

procedure TMargins.AssignTo(Dest: TPersistent);
begin
  if Dest is TMargins then
    with TMargins(Dest) do
    begin
      FLeft := Self.FLeft;
      FTop := Self.FTop;
      FRight := Self.FRight;
      FBottom := Self.FBottom;
      Change;
    end
  else
    inherited;
end;

procedure TMargins.Change;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TMargins.Create(Control: TControl);
begin
  FControl := Control;
  FLeft := 0;
  FTop := 0;
  FRight := 0;
  FBottom := 0;
end;

procedure TMargins.SetMargin(Index: Integer; Value: TMarginSize);
begin
  case Index of
    0:
      if Value <> FLeft then
      begin
        FLeft := Value;
        Change;
      end;
    1:
      if Value <> FTop then
      begin
        FTop := Value;
        Change;
      end;
    2:
      if Value <> FRight then
      begin
        FRight := Value;
        Change;
      end;
    3:
      if Value <> FBottom then
      begin
        FBottom := Value;
        Change;
      end;
  end;
end;
{$IFEND}

{
  功能:对图片进行拉伸绘制
  参数:
    HorzStretchType: 水平拉伸方式
      hstNone:    不拉伸
      hstAll:     整体拉伸
      hstLeft:    拉伸左边的
      hstMiddle:  拉伸中间的
      hstRight:   拉伸右边的
      hstCustom   自定义拉伸

    VertStretchType:垂直拉伸方式
    StretchPt: 自定义拉伸时的拉伸点

    + 增加StretchPt 属性 2010-11-26
}
procedure StretchPicture(const Source: Tbitmap; Dest: HDC; DestSize: TPoint;
  DestOffset: TPoint;
  HorzStretchType: THorzStretchType; VertStretchType: TVertStretchType;
  StretchPt: TPoint);
var
  TempBit, TempBit2: TBitmap;
  SrcR, DestR: TRect;
  CenterSize, BorderSize: Integer;
  Pt1, Pt2: TPoint;         //第一截终止坐标, 第三截起始坐标
  SrcH, Offset: Integer;
  OneImageWidth: Integer;

  function MakeRect(x, y, width, height: Longint): TRect;
  begin
    Result.Left   := x;
    Result.Top    := y;
    Result.Right  := x + width;
    Result.Bottom := y + height;
  end;
begin
  Offset := 0;
  OneImageWidth := Source.Width;
  //Offset := ImageIndex * OneImageWidth;
  SrcH := Source.Height;

  if (HorzStretchType = hstNone) and (VertStretchType = vstNone) then
  begin
    BitBlt(Dest, DestOffset.X, DestOffset.Y,
      OneImageWidth, SrcH,
      Source.Canvas.Handle, 0, 0, SRCCOPY);
    Exit;
  end;

  CenterSize := 0;
  if HorzStretchType = hstCustom then
  begin
    if StretchPt.X <= 1 then
      HorzStretchType := hstLeft
    else if StretchPt.X >= OneImageWidth then
      HorzStretchType := hstRight
    else
    begin
      CenterSize := 1;
      Pt1.X := StretchPt.X;
      Pt2.X := StretchPt.X + CenterSize;

      HorzStretchType := hstMiddle;
    end;
  end
  else if HorzStretchType = hstMiddle then
  begin
    if OneImageWidth mod 2 = 0 then
      CenterSize := 2
    else
      CenterSize := 1;
    Pt1.X := (OneImageWidth - CenterSize) div 2;
    Pt2.X := Pt1.X + CenterSize;
  end;

  TempBit := TBitmap.Create;
  TempBit2 := TBitmap.Create;
  try
    TempBit.Width := DestSize.X;
    TempBit.Height := SrcH;
    TempBit.PixelFormat := pf32bit;

    TempBit2.Width := DestSize.X;
    TempBit2.Height := DestSize.Y;
    TempBit2.PixelFormat := pf32bit;

    case HorzStretchType of
      hstNone:
        begin
          SrcR := MakeRect(0, 0, OneImageWidth, SrcH);
          DestR := SrcR;

          SrcR.Left := SrcR.Left + Offset;
          BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
        end;
      hstAll:
        begin
          SrcR := MakeRect(0, 0, OneImageWidth, SrcH);
          DestR := MakeRect(0, 0, DestSize.X, SrcH);

          SrcR.Left := SrcR.Left + Offset;
          StretchBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);
        end;
      hstLeft:
        begin
          { 画左边拉伸部分 }
          if DestSize.X > OneImageWidth then
          begin
            SrcR := MakeRect(0, 0, 1, SrcH);
            DestR := MakeRect(0, 0, DestSize.X - (OneImageWidth - 1), SrcH);
          end
          else
          begin
            SrcR := MakeRect(0, 0, 1, SrcH);
            DestR := SrcR;
          end;
          SrcR.Left := SrcR.Left + Offset;
          StretchBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

          { 画右边不拉伸部分 }
          if DestSize.X > OneImageWidth then
          begin
            SrcR := MakeRect(1, 0, OneImageWidth - 1, SrcH);
            DestR := MakeRect(DestSize.X - (OneImageWidth - 1), 0, OneImageWidth - 1, SrcH);
          end
          else
          begin
            SrcR := MakeRect(OneImageWidth - (DestSize.X - 1), 0, DestSize.X - 1, SrcH);
            DestR := MakeRect(1, 0, DestSize.X - 1, SrcH);
          end;
          SrcR.Left := SrcR.Left + Offset;
          BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
        end;
      hstMiddle:
        begin
          // 当图片比控件大时, 只取左右两部分
          if OneImageWidth >= DestSize.X then
          begin
            { 画左边部分 }
            SrcR := MakeRect(0, 0, DestSize.X div 2, SrcH);
            DestR := SrcR;
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            { 画右边部分 }
            DestR := MakeRect(SrcR.Right - SrcR.Left, 0,
              DestSize.X - (SrcR.Right - SrcR.Left), SrcH);
            SrcR := MakeRect(OneImageWidth - (DestSize.X - SrcR.Right + SrcR.Left),
              0, DestSize.X - SrcR.Right + SrcR.Left, SrcH);
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
          end
          else
          begin
            SrcR := MakeRect(0, 0, Pt1.X, SrcH);
            DestR := SrcR;
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            SrcR := MakeRect(Pt1.X, 0, CenterSize, SrcH);
            DestR := MakeRect(Pt1.X, 0, DestSize.X - (OneImageWidth - CenterSize), SrcH);
            SrcR.Left := SrcR.Left + Offset;
            StretchBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top,
              SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

            SrcR := MakeRect(Pt2.X, 0, OneImageWidth - Pt2.X, SrcH);
            DestR := MakeRect(DestSize.X - (OneImageWidth - Pt2.X), 0,
              OneImageWidth - Pt2.X, SrcH);
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

          {
            // 老版本,不支持指定点.只能从中间算点
            if OneImageWidth mod 2 = 0 then
              CenterSize := 2
            else
              CenterSize := 1;
            BorderSize := (OneImageWidth - CenterSize) div 2;

            SrcR := MakeRect(0, 0, BorderSize, SrcH);
            DestR := SrcR;
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            SrcR := MakeRect(BorderSize, 0, CenterSize, SrcH);
            DestR := MakeRect(BorderSize, 0, DestSize.X - BorderSize * 2, SrcH);
            SrcR.Left := SrcR.Left + Offset;
            StretchBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top,
              SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

            SrcR := MakeRect(BorderSize + CenterSize, 0, BorderSize, SrcH);
            DestR := MakeRect(DestSize.X - BorderSize, 0, BorderSize, SrcH);
            SrcR.Left := SrcR.Left + Offset;
            BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
            }
          end;
        end;
      hstRight:
        begin
          { 画左边不拉伸部分 }
          if DestSize.X > OneImageWidth then
          begin
            SrcR := MakeRect(0, 0, OneImageWidth - 1, SrcH);
            DestR := SrcR;
          end
          else
          begin
            SrcR := MakeRect(0, 0, DestSize.X - 1, SrcH);
            DestR := SrcR;
          end;
          SrcR.Left := SrcR.Left + Offset;
          BitBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

          { 画右边拉伸部分 }
          if DestSize.X > OneImageWidth then
          begin
            SrcR := MakeRect(OneImageWidth - 1, 0, 1, SrcH);
            DestR := MakeRect(OneImageWidth - 1, 0, DestSize.X - (OneImageWidth - 1), SrcH);
          end
          else
          begin
            SrcR := MakeRect(OneImageWidth - 1, 0, 1, SrcH);
            DestR := MakeRect(DestSize.X - 1, 0, 1, SrcH);
          end;
          SrcR.Left := SrcR.Left + Offset;
          StretchBlt(TempBit.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            Source.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);
        end;
    end;

    if VertStretchType = vstCustom then
    begin
      if StretchPt.Y <= 1 then
        VertStretchType := vstTop
      else if StretchPt.Y >= SrcH then
        VertStretchType := vstBottom
      else
      begin
        CenterSize := 1;
        Pt1.Y := StretchPt.Y;
        Pt2.Y := StretchPt.Y + CenterSize;
        VertStretchType := vstCenter;
      end;
    end
    else if VertStretchType = vstCenter then
    begin
      if SrcH mod 2 = 0 then
        CenterSize := 2
      else
        CenterSize := 1;
      Pt1.Y := (SrcH - CenterSize) div 2;
      Pt2.Y := Pt1.Y + CenterSize;
    end;

    case VertStretchType of
      vstNone:
        begin
          SrcR := MakeRect(0, 0, DestSize.X, SrcH);
          DestR := SrcR;

          BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
        end;
      vstAll:
        begin
          SrcR := MakeRect(0, 0, DestSize.X, SrcH);
          DestR := MakeRect(0, 0, DestSize.X, DestSize.Y);

          StretchBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);
        end;
      vstTop:
        begin
          { 画上边拉伸部分 }
          if DestSize.Y > SrcH then
          begin
            SrcR := MakeRect(0, 0, DestSize.X, 1);
            DestR := MakeRect(0, 0, DestSize.X, DestSize.Y - (SrcH - 1));
          end
          else
          begin
            SrcR := MakeRect(0, 0, DestSize.X, 1);
            DestR := SrcR;
          end;
          StretchBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

          { 画下边不拉伸部分 }
          if DestSize.Y > SrcH then
          begin
            SrcR := MakeRect(0, 1, DestSize.X, SrcH - 1);
            DestR := MakeRect(0, DestSize.Y - (SrcH - 1), DestSize.X, SrcH - 1);
          end
          else
          begin
            SrcR := MakeRect(OneImageWidth - (DestSize.X - 1), 0, DestSize.X - 1, SrcH);
            DestR := MakeRect(1, 0, DestSize.X - 1, SrcH);
          end;
          BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
        end;
      vstCenter:
        begin
          // 当图片比控件大时, 只取上下两部分
          if SrcH >= DestSize.Y then
          begin
            { 画上边部分 }
            SrcR := MakeRect(0, 0, DestSize.X, DestSize.Y div 2);
            DestR := SrcR;
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            { 画下边部分 }
            DestR := MakeRect(0, SrcR.Bottom - SrcR.Top, DestSize.X,
              DestSize.Y  - SrcR.Bottom + SrcR.Top);
            SrcR := MakeRect(0, SrcH - (DestSize.Y - SrcR.Bottom + SrcR.Top),
              DestSize.X, DestSize.Y - SrcR.Bottom + SrcR.Top);
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
          end
          else
          begin
            SrcR := MakeRect(0, 0, DestSize.X, Pt1.Y);
            DestR := SrcR;
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            SrcR := MakeRect(0, Pt1.Y, DestSize.X, CenterSize);
            DestR := MakeRect(0, Pt1.Y, DestSize.X, DestSize.Y - (SrcH - CenterSize));
            StretchBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top,
              SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

            SrcR := MakeRect(0, Pt2.Y, DestSize.X, SrcH - Pt2.Y);
            DestR := MakeRect(0, DestSize.Y - (SrcH - Pt2.Y),
              DestSize.X, SrcH - Pt2.Y);
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            (*
            // 老版本,不支持指定点.只能从中间算点
            if SrcH mod 2 = 0 then
              CenterSize := 2
            else
              CenterSize := 1;
            BorderSize := (SrcH - CenterSize) div 2;

            { 画上边 }
            SrcR := MakeRect(0, 0, DestSize.X, BorderSize);
            DestR := SrcR;
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

            { 画中间 }
            SrcR := MakeRect(0, BorderSize, DestSize.X, CenterSize);
            DestR := MakeRect(0, BorderSize, DestSize.X, DestSize.Y - BorderSize * 2);
            StretchBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top,
              SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);

            { 画下边 }
            SrcR := MakeRect(0, BorderSize + CenterSize, DestSize.X, BorderSize);
            DestR := MakeRect(0, DestSize.Y - BorderSize, DestSize.X, BorderSize);
            BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
              DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
              TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);
            *)
          end;
        end;
      vstBottom:
        begin
          { 画上边不拉伸部分 }
          if DestSize.Y > SrcH then
          begin
            SrcR := MakeRect(0, 0, DestSize.X, SrcH - 1);
            DestR := SrcR;
          end
          else
          begin
            SrcR := MakeRect(0, 0, DestSize.X, DestSize.Y - 1);
            DestR := SrcR;
          end;
          BitBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top, SRCCOPY);

          { 画下边拉伸部分 }
          if DestSize.Y > SrcH then
          begin
            SrcR := MakeRect(0, SrcH - 1, DestSize.X, 1);
            DestR := MakeRect(0, SrcH - 1, DestSize.X, DestSize.Y - (SrcH - 1));
          end
          else
          begin
            SrcR := MakeRect(0, SrcH - 1, DestSize.X, 1);
            DestR := MakeRect(0, DestSize.Y - 1, DestSize.X, 1);
          end;
          StretchBlt(TempBit2.Canvas.Handle, DestR.Left, DestR.Top,
            DestR.Right - DestR.Left, DestR.Bottom -  DestR.Top,
            TempBit.Canvas.Handle, SrcR.Left, SrcR.Top,
            SrcR.Right - SrcR.Left, SrcR.Bottom - SrcR.Top, SRCCOPY);
        end;
    end;
    //

    BitBlt(Dest, DestOffset.X, DestOffset.Y,
      TempBit2.Width, TempBit2.Height,
      TempBit2.Canvas.Handle, 0, 0, SRCCOPY);
  finally
    TempBit.Free;
    TempBit2.Free;
  end;
end;


end.
