unit PageNumCtrl;

interface

{$IF CompilerVersion >= 22.0} //delphi XE
  {$DEFINE USE_GDIPLUG}
{$IFEND}

uses
  Windows, Classes, SysUtils, Controls, Math, Messages, Graphics
  {$IFDEF USE_GDIPLUG},Winapi.GDIPAPI, Winapi.GDIPOBJ {$ENDIF};

type
  TPageButtonType = (pbtPageFirst, pbtPagePrior, pbtPageEllipsis, pbtPageNum, pbtPageNext, pbtPageLast);

  PPageButtonInfo = ^TPageButtonInfo;
  TPageButtonInfo = record
    Caption: string;
    ButtonType: TPageButtonType;
    ButtonValue: Integer;
    ButtonRect: TRect;
    Enabled: Boolean;
  end;

type
  TPageNumClickEvent = procedure(Sender: TObject; PageNum: Integer) of Object;

  TPageNumCtrl = class;

  TPageButtonColors = class(TPersistent)
  private
    FOwner: TPageNumCtrl;
    FColors: array[0..14] of TColor;

    function GetColor(const Index: Integer): TColor;
    procedure SetColor(const Index: Integer; const Value: TColor);
  public
    constructor Create(AOwner: TPageNumCtrl);

    procedure Assign(Source: TPersistent); override;
  published
    property Color: TColor index 0 read GetColor write SetColor default clWhite;
    property BorderColor: TColor index 1 read GetColor write SetColor default clGray;
    property TextColor: TColor index 2 read GetColor write SetColor default clBlack;

    property HotColor: TColor index 3 read GetColor write SetColor default clCream;
    property HotBorderColor: TColor index 4 read GetColor write SetColor default clGray;
    property HotTextColor: TColor index 5 read GetColor write SetColor default clBlack;

    property DownColor: TColor index 6 read GetColor write SetColor default $00EEEEEE;
    property DownBorderColor: TColor index 7 read GetColor write SetColor default clGray;
    property DownTextColor: TColor index 8 read GetColor write SetColor default clBlack;

    property SelectColor: TColor index 9 read GetColor write SetColor default $00FF9C39;
    property SelectBorderColor: TColor index 10 read GetColor write SetColor default $00FF9C39;
    property SelectTextColor: TColor index 11 read GetColor write SetColor default clBlack;

    property DisableColor: TColor index 12 read GetColor write SetColor default clWhite;
    property DisableBorderColor: TColor index 13 read GetColor write SetColor default clGray;
    property DisableTextColor: TColor index 14 read GetColor write SetColor default clGray;
  end;

  TPageButtonOption = class(TPersistent)
  private
    FOwner: TPageNumCtrl;
    FMinWidth: Integer;                 // 最小宽度
    FHorzSpaceExpand: Integer;          // 按钮内部水平扩展
    FHeight: Integer;                   // 按钮高度
{$IFDEF USE_GDIPLUG}
    FRoundedSize: Integer;              // 按钮圆角大小
{$ENDIF}
    FBorderWidth: Integer;              // 按钮边框大小

    FPageArrowPenWidth: Integer;        // 换页箭头笔画宽
    FPageArrowHeight: Integer;          // 换页箭头高度
    FPageArrowSpace: Integer;           // 首页/尾页2个箭头之间间隔

    FButtonMargin: Integer;             // 按钮间隔

    FPageShowCount: Integer;            // 显示页数数量

    FPageNumTextOffsetX: Integer;       // 页数文字偏移X
    FPageNumTextOffsetY: Integer;       // 页数文字偏移Y

    FPageFirstShow: Boolean;            // 首页
    FPageLastShow: Boolean;             // 尾页
    FPagePriorShow: Boolean;            // 上一页
    FPageNextShow: Boolean;             // 最后一页

    FPageFirstNumShow: Boolean;         // 首页数字显示
    FPageLastNumShow: Boolean;          // 尾页数字显示

    FPageFirstText: string;             // 首页文字
    FPageLastText: string;              // 尾页文字
    FPagePriorText: string;             // 上一页文字
    FPageNextText: string;              // 下一页文字

    FPageEllipsisText: string;          // 省略号文字

    procedure SetButtonMargin(const Value: Integer);
    procedure SetHeight(const Value: Integer);
    procedure SetHorzSpaceExpand(const Value: Integer);
    procedure SetMinWidth(const Value: Integer);
{$IFDEF USE_GDIPLUG}
    procedure SetRoundedSize(const Value: Integer);
{$ENDIF}
    procedure SetBorderWidth(const Value: Integer);

    procedure SetPageArrowPenWidth(const Value: Integer);
    procedure SetPageArrowHeight(const Value: Integer);
    procedure SetPageArrowSpace(const Value: Integer);

    procedure SetPageShowCount(const Value: Integer);

    procedure SetPageNumTextOffsetX(const Value: Integer);
    procedure SetPageNumTextOffsetY(const Value: Integer);


    procedure SetPageFirstNumShow(const Value: Boolean);
    procedure SetPageFirstShow(const Value: Boolean);
    procedure SetPageFirstText(const Value: string);
    procedure SetPageLastNumShow(const Value: Boolean);
    procedure SetPageLastShow(const Value: Boolean);
    procedure SetPageLastText(const Value: string);
    procedure SetPageNextShow(const Value: Boolean);
    procedure SetPageNextText(const Value: string);
    procedure SetPagePriorShow(const Value: Boolean);
    procedure SetPagePriorText(const Value: string);
    procedure SetPageEllipsisText(const Value: string);
  protected
    procedure Change;
  public
    constructor Create(AOwner: TPageNumCtrl);

    procedure Assign(Source: TPersistent); override;
  published
    property MinWidth: Integer read FMinWidth write SetMinWidth default 40;
    property HorzSpaceExpand: Integer read FHorzSpaceExpand write SetHorzSpaceExpand default 10;
    property Height: Integer read FHeight write SetHeight default 40;
{$IFDEF USE_GDIPLUG}
    property RoundedSize: Integer read FRoundedSize write SetRoundedSize default 10;
{$ENDIF}
    property BorderWidth: Integer read FBorderWidth write SetBorderWidth default 1;


    property PageArrowPenWidth: Integer read FPageArrowPenWidth write SetPageArrowPenWidth default 2;
    property PageArrowHeight: Integer read FPageArrowHeight write SetPageArrowHeight default 9;
    property PageArrowSpace: Integer read FPageArrowSpace write SetPageArrowSpace default 6;

    property ButtonMargin: Integer read FButtonMargin write SetButtonMargin default 6;
    property PageShowCount: Integer read FPageShowCount write SetPageShowCount default 7;

    property PageNumTextOffsetX: Integer read FPageNumTextOffsetX write SetPageNumTextOffsetX default 0;
    property PageNumTextOffsetY: Integer read FPageNumTextOffsetY write SetPageNumTextOffsetY default 0;

    property PageFirstShow: Boolean read FPageFirstShow write SetPageFirstShow default True;
    property PageLastShow: Boolean read FPageLastShow write SetPageLastShow default True;
    property PagePriorShow: Boolean read FPagePriorShow write SetPagePriorShow default True;
    property PageNextShow: Boolean read FPageNextShow write SetPageNextShow default True;

    property PageFirstNumShow: Boolean read FPageFirstNumShow write SetPageFirstNumShow default True;
    property PageLastNumShow: Boolean read FPageLastNumShow write SetPageLastNumShow default True;

    property PageFirstText: string read FPageFirstText write SetPageFirstText;
    property PageLastText: string read FPageLastText write SetPageLastText;

    property PagePriorText: string read FPagePriorText write SetPagePriorText;
    property PageNextText: string read FPageNextText write SetPageNextText;

    property PageEllipsisText: string read FPageEllipsisText write SetPageEllipsisText;
  end;

  TPageNumCtrl = class(TCustomControl)          // TGraphicControl
  private
    FDrawBitmap: TBitmap;

    FButtonColors: TPageButtonColors;
    FButtonOption: TPageButtonOption;

    FCurrentPage: Integer;
    FPageCount: Integer;

    FButtonLeftMargin: Integer;
    FButtonTopMargin: Integer;

    FShowTextX: Integer;
    FShowTextY: Integer;
    FShowText: string;
    FShowTextFont: TFont;

    FAllButtonWidth: Integer;
    FIsRecallPageButtons: Boolean;

    FOnPageNumClick: TPageNumClickEvent;
  private
    FActivePageButton: PPageButtonInfo;
    FMouseDownPageButton: PPageButtonInfo;
    FPageButtons: TList;
    procedure ClearPageButtons;
    procedure DrawButton(IsSelect: Boolean; ButtonInfo: PPageButtonInfo
      {$IFDEF USE_GDIPLUG}; G: TGPGraphics; Font: TGPFont; Pen: TGPPen; Brush, FontBrush: TGPSolidBrush{$ENDIF});

    procedure WMEraseBkgnd(var Message: TWmEraseBkgnd); message WM_ERASEBKGND;
    procedure SetCurrentPage(const Value: Integer);
    procedure SetPageCount(const Value: Integer);
    procedure SetButtonLeftMargin(const Value: Integer);
    procedure SetButtonTopMargin(const Value: Integer);
    procedure SetShowTextFont(const Value: TFont);
  protected
    procedure RecallPageButtons; virtual;
    procedure DoPageNumClick(PageNum: Integer); virtual;
    //procedure DoMouseLeave; override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;

    procedure Loaded; override;
    procedure Paint; override;
    procedure Resize; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

  published
    property Action;
    property Align;
    property Anchors;
    property Enabled;
    property Font;
    property TabStop;
    property Visible;
    property DoubleBuffered;

{$IF CompilerVersion >= 22.0} //delphi XE
    property Padding;
{$IFEND}

    property ButtonColors: TPageButtonColors read FButtonColors write FButtonColors;
    property ButtonOption: TPageButtonOption read FButtonOption write FButtonOption;

    property CurrentPage: Integer read FCurrentPage write SetCurrentPage;
    property PageCount: Integer read FPageCount write SetPageCount;

    property ButtonLeftMargin: Integer read FButtonLeftMargin write SetButtonLeftMargin;
    property ButtonTopMargin: Integer read FButtonTopMargin write SetButtonTopMargin;

    property ShowTextX: Integer read FShowTextX write FShowTextX;
    property ShowTextY: Integer read FShowTextY write FShowTextY;
    property ShowText: string read FShowText write FShowText;
    property ShowTextFont: TFont read FShowTextFont write SetShowTextFont;

    property OnPageNumClick: TPageNumClickEvent read FOnPageNumClick write FOnPageNumClick;
  end;

implementation

{ TPageButtonColors }

constructor TPageButtonColors.Create(AOwner: TPageNumCtrl);
begin
  FOwner := AOwner;

  // Color
  FColors[0] := clWhite;
  FColors[1] := clGray;
  FColors[2] := clBlack;

  // Hot
  FColors[3] := clCream;
  FColors[4] := clGray;
  FColors[5] := clBlack;

  // Down
  FColors[6] := $00EEEEEE;
  FColors[7] := clGray;
  FColors[8] := clBlack;

  // Select
  FColors[9] := $00FF9C39;
  FColors[10] := $00FF9C39;
  FColors[11] := clWhite;

  // Disable
  FColors[12] := clWhite;
  FColors[13] := clGray;
  FColors[14] := clGray;
end;

procedure TPageButtonColors.Assign(Source: TPersistent);
begin
  if Source is TPageButtonColors then
  begin
    FColors := TPageButtonColors(Source).FColors;
    FOwner.Invalidate;
  end
  else
    inherited;
end;


function TPageButtonColors.GetColor(const Index: Integer): TColor;
begin
  Result := FColors[Index];
end;

procedure TPageButtonColors.SetColor(const Index: Integer; const Value: TColor);
begin
  FColors[Index] := Value;
end;

{ TPageButtonOption }

constructor TPageButtonOption.Create(AOwner: TPageNumCtrl);
begin
  FOwner := AOwner;

  FMinWidth := 40;
  FHorzSpaceExpand := 10;
  FHeight := 40;
  FRoundedSize := 10;
  FBorderWidth := 1;

  FPageArrowPenWidth := 2;
  FPageArrowHeight := 9;
  FPageArrowSpace := 6;

  FButtonMargin := 6;
  FPageShowCount := 7;

  FPageNumTextOffsetX := 0;
  FPageNumTextOffsetY := 0;

  FPageFirstShow := True;
  FPageLastShow := True;
  FPagePriorShow := True;
  FPageNextShow := True;

  FPageFirstNumShow := True;
  FPageLastNumShow := True;

  FPageFirstText := '';
  FPageLastText := '';

  FPagePriorText := '';
  FPageNextText := '';
  FPageEllipsisText := '······';
end;

procedure TPageButtonOption.Assign(Source: TPersistent);
var
  Option: TPageButtonOption;
begin
  if Source is TPageButtonOption then
  begin
    Option := Source as TPageButtonOption;

    FMinWidth := Option.FMinWidth;
    FHorzSpaceExpand := Option.FHorzSpaceExpand;
    FHeight := Option.FHeight;
{$IFDEF USE_GDIPLUG}
    FRoundedSize := Option.FRoundedSize;
{$ENDIF}
    FBorderWidth := Option.FBorderWidth;

    FPageArrowPenWidth := Option.FPageArrowPenWidth;
    FPageArrowHeight := Option.FPageArrowHeight;
    FPageArrowSpace := Option.FPageArrowSpace;

    FButtonMargin := Option.FButtonMargin;
    FPageShowCount := Option.FPageShowCount;

    FPageNumTextOffsetX := Option.FPageNumTextOffsetX;
    FPageNumTextOffsetY := Option.FPageNumTextOffsetY;

    FPageFirstShow := Option.FPageFirstShow;
    FPageLastShow := Option.FPageLastShow;
    FPagePriorShow := Option.FPagePriorShow;
    FPageNextShow := Option.FPageNextShow;

    FPageFirstNumShow := Option.FPageFirstNumShow;
    FPageLastNumShow := Option.FPageLastNumShow;

    FPageFirstText := Option.FPageFirstText;
    FPageLastText := Option.FPageLastText;
    FPagePriorText := Option.FPagePriorText;
    FPageNextText := Option.FPageNextText;

    Change;
  end
  else
    inherited;
end;

procedure TPageButtonOption.Change;
begin
  FOwner.RecallPageButtons;
  FOwner.Invalidate;
end;

procedure TPageButtonOption.SetButtonMargin(const Value: Integer);
begin
  if FButtonMargin <> Value then
  begin
    FButtonMargin := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetHeight(const Value: Integer);
begin
  if FHeight <> Value then
  begin
    FHeight := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetHorzSpaceExpand(const Value: Integer);
begin
  if FHorzSpaceExpand <> Value then
  begin
    FHorzSpaceExpand := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetMinWidth(const Value: Integer);
begin
  if FMinWidth <> Value then
  begin
    FMinWidth := Value;
    Change;
  end;
end;

{$IFDEF USE_GDIPLUG}
procedure TPageButtonOption.SetRoundedSize(const Value: Integer);
begin
  if FRoundedSize <> Value then
  begin
    FRoundedSize := Value;
    Change;
  end;
end;
{$ENDIF}

procedure TPageButtonOption.SetBorderWidth(const Value: Integer);
begin
  if FBorderWidth <> Value then
  begin
    FBorderWidth := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageArrowPenWidth(const Value: Integer);
begin
  if FPageArrowPenWidth <> Value then
  begin
    FPageArrowPenWidth := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageArrowHeight(const Value: Integer);
begin
  if FPageArrowHeight <> Value then
  begin
    FPageArrowHeight := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageArrowSpace(const Value: Integer);
begin
  if FPageArrowSpace <> Value then
  begin
    FPageArrowSpace := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageShowCount(const Value: Integer);
begin
  if FPageShowCount <> Value then
  begin
    FPageShowCount := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageNumTextOffsetX(const Value: Integer);
begin
  if FPageNumTextOffsetX <> Value then
  begin
    FPageNumTextOffsetX := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageNumTextOffsetY(const Value: Integer);
begin
  if FPageNumTextOffsetY <> Value then
  begin
    FPageNumTextOffsetY := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageFirstNumShow(const Value: Boolean);
begin
  if FPageFirstNumShow <> Value then
  begin
    FPageFirstNumShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageFirstShow(const Value: Boolean);
begin
  if FPageFirstShow <> Value then
  begin
    FPageFirstShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageFirstText(const Value: string);
begin
  if FPageFirstText <> Value then
  begin
    FPageFirstText := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageLastNumShow(const Value: Boolean);
begin
  if FPageLastNumShow <> Value then
  begin
    FPageLastNumShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageLastShow(const Value: Boolean);
begin
  if FPageLastShow <> Value then
  begin
    FPageLastShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageLastText(const Value: string);
begin
  if FPageLastText <> Value then
  begin
    FPageLastText := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageNextShow(const Value: Boolean);
begin
  if FPageNextShow <> Value then
  begin
    FPageNextShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageNextText(const Value: string);
begin
  if FPageNextText <> Value then
  begin
    FPageNextText := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPagePriorShow(const Value: Boolean);
begin
  if FPagePriorShow <> Value then
  begin
    FPagePriorShow := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPagePriorText(const Value: string);
begin
  if FPagePriorText <> Value then
  begin
    FPagePriorText := Value;
    Change;
  end;
end;

procedure TPageButtonOption.SetPageEllipsisText(const Value: string);
begin
  if FPageEllipsisText <> Value then
  begin
    FPageEllipsisText := Value;
    Change;
  end;
end;

{ TPageNumCtrl }

constructor TPageNumCtrl.Create(AOwner: TComponent);
begin
  inherited;

  ControlStyle := [csAcceptsControls, csCaptureMouse, csClickEvents,
    csOpaque, csDoubleClicks, csReplicatable, csPannable, csGestures];

  //FDoubleBuffered := True;
  FDrawBitmap := TBitmap.Create;
  FDrawBitmap.PixelFormat := pf24bit;

  FButtonColors := TPageButtonColors.Create(Self);
  FButtonOption := TPageButtonOption.Create(Self);

  FCurrentPage := 0;
  FPageCount := 0;

  FButtonLeftMargin := 0;
  FButtonTopMargin := 0;

  FShowTextX := 0;
  FShowTextY := 0;
  FShowText := '';
  FShowTextFont := TFont.Create;

  FAllButtonWidth := 0;
  FIsRecallPageButtons := False;

  FActivePageButton := nil;
  FMouseDownPageButton := nil;
  FPageButtons := TList.Create;
end;

destructor TPageNumCtrl.Destroy;
begin
  FButtonColors.Free;
  FButtonOption.Free;
  FShowTextFont.Free;

  ClearPageButtons;
  FPageButtons.Free;
  FDrawBitmap.Free;
  inherited;
end;

procedure TPageNumCtrl.DoPageNumClick(PageNum: Integer);
begin
  if Assigned(FOnPageNumClick) then
  begin
    FOnPageNumClick(Self, PageNum);
  end;
end;

procedure TPageNumCtrl.Loaded;
begin
  inherited;
  RecallPageButtons;
end;

(*
// 用边界算文字大小更准确
  //G.MeasureString(ButtonInfo.Caption, -1, Font, MakePoint(R.Left * 1.0, R.Top), RectF);
SizeF GetTextBounds(const Font& font,const StringFormat& strFormat,const CString& szText)
{
    GraphicsPath graphicsPathObj;
    FontFamily fontfamily;
    font.GetFamily(&fontfamily);
    graphicsPathObj.AddString(szText,-1,&fontfamily,font.GetStyle(),font.GetSize(),\
                              PointF(0,0),&strFormat);
    RectF rcBound;
    /// 获取边界范围
    graphicsPathObj.GetBounds(&rcBound);
    /// 返回文本的宽高
    return SizeF(rcBound.Width,rcBound.Height);
}
*)

procedure TPageNumCtrl.DrawButton(IsSelect: Boolean; ButtonInfo: PPageButtonInfo
  {$IFDEF USE_GDIPLUG}; G: TGPGraphics; Font: TGPFont; Pen: TGPPen; Brush, FontBrush: TGPSolidBrush{$ENDIF});
var
  R: PRect;
{$IFDEF USE_GDIPLUG}
  Path: TGPGraphicsPath;
  RectF: TGPRectF;
  StrFormat: TGPStringFormat;
  Pt1, Pt2: TPoint;
  ArrowWidth: Integer;
  HalfArrowSpace: Integer;
{$ENDIF}
begin
  R := @ButtonInfo.ButtonRect;
{$IFDEF USE_GDIPLUG}

  // 绘制外框
  if ButtonInfo.ButtonType <> pbtPageEllipsis then
  begin
    Path := TGPGraphicsPath.Create();
    Path.AddArc(R.Right - FButtonOption.FRoundedSize, R.Top, FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, 270, 90);
    Path.AddArc(R.Right - FButtonOption.FRoundedSize, R.Bottom - FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, 0, 90);
    Path.AddArc(R.Left, R.Bottom - FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, 90, 90);
    Path.AddArc(R.Left, R.Top, FButtonOption.FRoundedSize, FButtonOption.FRoundedSize, 180, 90);
    Path.AddLine(R.Left + FButtonOption.FRoundedSize, R.Top, R.Right - FButtonOption.FRoundedSize / 2, R.Top);

    Pen.SetWidth(FButtonOption.FBorderWidth);

    if IsSelect then
    begin
      Brush.SetColor(ColorRefToARGB(FButtonColors.SelectColor));
      Pen.SetColor(ColorRefToARGB(FButtonColors.SelectBorderColor));
    end
    else if (ButtonInfo = FMouseDownPageButton) and (ButtonInfo.Enabled) then
    begin
      Brush.SetColor(ColorRefToARGB(FButtonColors.DownColor));
      Pen.SetColor(ColorRefToARGB(FButtonColors.DownBorderColor));
    end
    else if (ButtonInfo = FActivePageButton) and (ButtonInfo.Enabled) then
    begin
      Brush.SetColor(ColorRefToARGB(FButtonColors.HotColor));
      Pen.SetColor(ColorRefToARGB(FButtonColors.HotBorderColor));
    end
    else if ButtonInfo.Enabled then
    begin
      Brush.SetColor(ColorRefToARGB(FButtonColors.Color));
      Pen.SetColor(ColorRefToARGB(FButtonColors.BorderColor));
    end
    else
    begin
      Brush.SetColor(ColorRefToARGB(FButtonColors.DisableColor));
      Pen.SetColor(ColorRefToARGB(FButtonColors.DisableBorderColor));
    end;

    G.FillPath(Brush, Path);
    G.DrawPath(Pen, Path);

    Path.Free;
  end;

  Pen.SetWidth(FButtonOption.PageArrowPenWidth);
  if IsSelect then
  begin
    FontBrush.SetColor(ColorRefToARGB(FButtonColors.SelectTextColor));
    Pen.SetColor(ColorRefToARGB(FButtonColors.SelectTextColor));
  end
  else if (ButtonInfo = FMouseDownPageButton) and (ButtonInfo.Enabled) then
  begin
    FontBrush.SetColor(ColorRefToARGB(FButtonColors.DownTextColor));
    Pen.SetColor(ColorRefToARGB(FButtonColors.DownTextColor));
  end
  else if (ButtonInfo = FActivePageButton) and (ButtonInfo.Enabled) then
  begin
    FontBrush.SetColor(ColorRefToARGB(FButtonColors.HotTextColor));
    Pen.SetColor(ColorRefToARGB(FButtonColors.HotTextColor));
  end
  else if ButtonInfo.Enabled then
  begin
    FontBrush.SetColor(ColorRefToARGB(FButtonColors.TextColor));
    Pen.SetColor(ColorRefToARGB(FButtonColors.TextColor));
  end
  else
  begin
    FontBrush.SetColor(ColorRefToARGB(FButtonColors.DisableTextColor));
    Pen.SetColor(ColorRefToARGB(FButtonColors.DisableTextColor));
  end;

  ArrowWidth := FButtonOption.FPageArrowHeight div 2;
  HalfArrowSpace := FButtonOption.FPageArrowSpace div 2;

  if ButtonInfo.ButtonType in [pbtPageEllipsis, pbtPageNum] then
  begin
    // 对齐方式
    StrFormat := TGPStringFormat.Create();
    StrFormat.SetAlignment(StringAlignmentCenter);
    StrFormat.SetLineAlignment(StringAlignmentCenter);

    // 不偏移对不齐
    RectF := MakeRect(R.Left + 0.0,  R.Top + 0.0, R.Width, R.Height);

    if FButtonOption.FPageNumTextOffsetX <> 0 then
      RectF.Width := RectF.Width + FButtonOption.FPageNumTextOffsetX * 2;

    if FButtonOption.FPageNumTextOffsetY <> 0 then
      RectF.Height := RectF.Height + FButtonOption.FPageNumTextOffsetY * 2;

    G.DrawString(ButtonInfo.Caption, -1, Font, RectF, StrFormat, FontBrush);

    StrFormat.Free;
  end
  else if ButtonInfo.ButtonType = pbtPageFirst then
  begin
    if Length(FButtonOption.FPageFirstText) > 0 then
    begin
      // 对齐方式
      StrFormat := TGPStringFormat.Create();
      StrFormat.SetAlignment(StringAlignmentCenter);
      StrFormat.SetLineAlignment(StringAlignmentCenter);

      // 不偏移对不齐
      RectF := MakeRect(R.Left + 0.0,  R.Top + 0.0, R.Width, R.Height);

      if FButtonOption.FPageNumTextOffsetX <> 0 then
        RectF.Width := RectF.Width + FButtonOption.FPageNumTextOffsetX * 2;

      if FButtonOption.FPageNumTextOffsetY <> 0 then
        RectF.Height := RectF.Height + FButtonOption.FPageNumTextOffsetY * 2;

      G.DrawString(FButtonOption.FPageFirstText, -1, Font, RectF, StrFormat, FontBrush);

      StrFormat.Free;
    end
    else
    begin
      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2 + ArrowWidth - HalfArrowSpace;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X - ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);
      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;

      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2 + ArrowWidth + HalfArrowSpace;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X - ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);
      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;
    end;
  end
  else if ButtonInfo.ButtonType = pbtPagePrior then
  begin
    if Length(FButtonOption.FPagePriorText) > 0 then
    begin
      // 对齐方式
      StrFormat := TGPStringFormat.Create();
      StrFormat.SetAlignment(StringAlignmentCenter);
      StrFormat.SetLineAlignment(StringAlignmentCenter);

      // 不偏移对不齐
      RectF := MakeRect(R.Left + 0.0,  R.Top + 0.0, R.Width, R.Height);

      if FButtonOption.FPageNumTextOffsetX <> 0 then
        RectF.Width := RectF.Width + FButtonOption.FPageNumTextOffsetX * 2;

      if FButtonOption.FPageNumTextOffsetY <> 0 then
        RectF.Height := RectF.Height + FButtonOption.FPageNumTextOffsetY * 2;

      G.DrawString(FButtonOption.FPagePriorText, -1, Font, RectF, StrFormat, FontBrush);

      StrFormat.Free;
    end
    else
    begin
      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2 + ArrowWidth;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X - ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);

      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;
    end;
  end
  else if ButtonInfo.ButtonType = pbtPageNext then
  begin
    if Length(FButtonOption.FPageNextText) > 0 then
    begin
      // 对齐方式
      StrFormat := TGPStringFormat.Create();
      StrFormat.SetAlignment(StringAlignmentCenter);
      StrFormat.SetLineAlignment(StringAlignmentCenter);

      // 不偏移对不齐
      RectF := MakeRect(R.Left + 0.0,  R.Top + 0.0, R.Width, R.Height);

      if FButtonOption.FPageNumTextOffsetX <> 0 then
        RectF.Width := RectF.Width + FButtonOption.FPageNumTextOffsetX * 2;

      if FButtonOption.FPageNumTextOffsetY <> 0 then
        RectF.Height := RectF.Height + FButtonOption.FPageNumTextOffsetY * 2;

      G.DrawString(FButtonOption.FPageNextText, -1, Font, RectF, StrFormat, FontBrush);

      StrFormat.Free;
    end
    else
    begin
      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X + ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);

      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;
    end;
  end

  else if ButtonInfo.ButtonType = pbtPageLast then
  begin
    if Length(FButtonOption.FPageLastText) > 0 then
    begin
      // 对齐方式
      StrFormat := TGPStringFormat.Create();
      StrFormat.SetAlignment(StringAlignmentCenter);
      StrFormat.SetLineAlignment(StringAlignmentCenter);

      // 不偏移对不齐
      RectF := MakeRect(R.Left + 0.0,  R.Top + 0.0, R.Width, R.Height);

      if FButtonOption.FPageNumTextOffsetX <> 0 then
        RectF.Width := RectF.Width + FButtonOption.FPageNumTextOffsetX * 2;

      if FButtonOption.FPageNumTextOffsetY <> 0 then
        RectF.Height := RectF.Height + FButtonOption.FPageNumTextOffsetY * 2;

      G.DrawString(FButtonOption.FPageLastText, -1, Font, RectF, StrFormat, FontBrush);

      StrFormat.Free;
    end
    else
    begin
      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2 + - HalfArrowSpace;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X + ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);

      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;

      Pt1.X := R.Left + (R.Right - R.Left - ArrowWidth) div 2 + HalfArrowSpace;
      Pt1.Y := R.Top + (R.Bottom - R.Top - FButtonOption.FPageArrowHeight) div 2;
      Pt2.X := Pt1.X + ArrowWidth;
      Pt2.Y := Pt1.Y + ArrowWidth;

      Path := TGPGraphicsPath.Create();
      Path.AddLine(Pt1.X, Pt1.Y, Pt2.X, Pt2.Y);

      Path.AddLine(Pt2.X, Pt2.Y, Pt1.X, Pt1.Y + FButtonOption.FPageArrowHeight);

      G.DrawPath(Pen, Path);
      Path.Free;
    end;
  end

{$ELSE}
  Canvas.Rectangle(R^);

  DrawText(Canvas.Handle, ButtonInfo.Caption, -1, ButtonInfo.ButtonRect, DT_NOPREFIX or DT_SINGLELINE or DT_CENTER or DT_VCENTER);
{$ENDIF}
end;

procedure TPageNumCtrl.Paint;
var
  I: Integer;
  ButtonInfo: PPageButtonInfo;
  SelectButton: PPageButtonInfo;
{$IFDEF USE_GDIPLUG}
  Graphics: TGPGraphics;
  GPFont: TGPFont;
  Pen: TGPPen;
  Brush: TGPSolidBrush;
  FontBrush: TGPSolidBrush;
  R: TRect;
  Flags: LongInt;
{$ENDIF}
begin
  inherited;
  if not FIsRecallPageButtons then
    RecallPageButtons;

  SelectButton := nil;

  Canvas.Font.Assign(Font);

{$IFDEF USE_GDIPLUG}
  Graphics := TGPGraphics.Create(FDrawBitmap.Canvas.Handle);
  Graphics.SetSmoothingMode(SmoothingModeHighQuality);
  Graphics.SetTextRenderingHint(TextRenderingHintSystemDefault);

  GPFont := TGPFont.Create(Font.Name, Font.Size, FontStyleRegular, UnitPoint);
  Pen := TGPPen.Create(ColorRefToARGB(FButtonColors.BorderColor));
  Brush := TGPSolidBrush.Create(ColorRefToARGB(FButtonColors.Color));
  FontBrush := TGPSolidBrush.Create(ColorRefToARGB(FButtonColors.TextColor));
{$ENDIF}

  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];

    if (ButtonInfo.ButtonType = pbtPageNum) and (ButtonInfo.ButtonValue = FCurrentPage) then
    begin
      SelectButton := ButtonInfo;
    end
    else
    begin
      DrawButton(False, ButtonInfo {$IFDEF USE_GDIPLUG}, Graphics, GPFont, Pen, Brush, FontBrush {$ENDIF});
    end;
  end;

  if SelectButton <> nil then
    DrawButton(True, SelectButton {$IFDEF USE_GDIPLUG}, Graphics, GPFont, Pen, Brush, FontBrush {$ENDIF});

{$IFDEF USE_GDIPLUG}
  Pen.Free;
  Brush.Free;
  FontBrush.Free;
  Graphics.Free;

  if Length(FShowText) > 0 then
  begin
    I := FDrawBitmap.Canvas.TextHeight('Pp');
    FDrawBitmap.Canvas.Font.Assign(FShowTextFont);

    FDrawBitmap.Canvas.Font.Height := MulDiv(FDrawBitmap.Canvas.Font.Height, FCurrentPPI, 96);

    FDrawBitmap.Canvas.TextOut(MulDiv(FShowTextX, FCurrentPPI, 96), FButtonTopMargin + (FButtonOption.FHeight - I) div 2 + FShowTextY, FShowText);
  end;

  Canvas.Draw(0, 0, FDrawBitmap);
{$ENDIF}
end;

procedure TPageNumCtrl.ClearPageButtons;
var
  I: Integer;
  ButtonInfo: PPageButtonInfo;
begin
  FActivePageButton := nil;
  FMouseDownPageButton := nil;
  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];
    Dispose(ButtonInfo);
  end;
  FPageButtons.Clear;
end;

procedure TPageNumCtrl.RecallPageButtons;
var
  ButtonInfo: PPageButtonInfo;
  HalfPageCount, StartPage, EndPage: Integer;
  I, L, T, W: Integer;
begin
  ClearPageButtons;
  if FPageCount = 0 then Exit;
  if FButtonOption.FPageFirstShow then
  begin
    New(ButtonInfo);

    if Length(FButtonOption.FPageFirstText)  = 0 then
      ButtonInfo.Caption := '«'
    else
      ButtonInfo.Caption := FButtonOption.FPageFirstText;

    ButtonInfo.ButtonType := pbtPageFirst;
    ButtonInfo.ButtonValue := 1;
    ButtonInfo.Enabled := FCurrentPage > 1;
    FPageButtons.Add(ButtonInfo);
  end;

  if FButtonOption.FPagePriorShow then
  begin
    New(ButtonInfo);

    if Length(FButtonOption.FPagePriorText)  = 0 then
      ButtonInfo.Caption := '‹'
    else
      ButtonInfo.Caption := FButtonOption.FPagePriorText;

    ButtonInfo.ButtonType := pbtPagePrior;
    ButtonInfo.ButtonValue := FCurrentPage - 1;
    ButtonInfo.Enabled := FCurrentPage > 1;
    FPageButtons.Add(ButtonInfo);
  end;

  HalfPageCount := (FButtonOption.FPageShowCount - 1) div 2;
  StartPage := FCurrentPage - HalfPageCount;
  EndPage := FCurrentPage + HalfPageCount;

  if StartPage <= 0 then
  begin
    EndPage := Min(FPageCount, EndPage + 1 - StartPage);
    StartPage := 1;
  end;

  if EndPage > FPageCount then
  begin
    StartPage := Max(1, StartPage + FPageCount - EndPage);
    EndPage := FPageCount;
  end;

  if FButtonOption.FPageFirstNumShow then
  begin
    if StartPage >= 2 then
    begin
      New(ButtonInfo);
      ButtonInfo.Caption := '1';
      ButtonInfo.ButtonType := pbtPageNum;
      ButtonInfo.Enabled := True;
      ButtonInfo.ButtonValue := 1;
      FPageButtons.Add(ButtonInfo);

      New(ButtonInfo);
      ButtonInfo.Caption := FButtonOption.FPageEllipsisText;
      ButtonInfo.ButtonType := pbtPageEllipsis;
      ButtonInfo.Enabled := False;
      FPageButtons.Add(ButtonInfo);
    end;
  end;

  for I := StartPage to EndPage do
  begin
    New(ButtonInfo);
    ButtonInfo.Caption := IntToStr(I);
    ButtonInfo.ButtonType := pbtPageNum;
    ButtonInfo.Enabled := FCurrentPage <> I;
    ButtonInfo.ButtonValue := I;
    FPageButtons.Add(ButtonInfo);
  end;

  if FButtonOption.FPageLastNumShow then
  begin
    if FPageCount > EndPage then
    begin
      New(ButtonInfo);
      ButtonInfo.Caption := FButtonOption.FPageEllipsisText;
      ButtonInfo.ButtonType := pbtPageEllipsis;
      ButtonInfo.Enabled := False;
      FPageButtons.Add(ButtonInfo);

      New(ButtonInfo);
      ButtonInfo.Caption := IntToStr(FPageCount);
      ButtonInfo.ButtonType := pbtPageNum;
      ButtonInfo.Enabled := True;
      ButtonInfo.ButtonValue := FPageCount;
      FPageButtons.Add(ButtonInfo);
    end;
  end;

  if FButtonOption.FPageNextShow then
  begin
    New(ButtonInfo);

    if Length(FButtonOption.FPageNextText)  = 0 then
      ButtonInfo.Caption := '›'
    else
      ButtonInfo.Caption := FButtonOption.FPageNextText;

    ButtonInfo.ButtonType := pbtPageNext;
    ButtonInfo.Enabled := FCurrentPage < FPageCount;
    FPageButtons.Add(ButtonInfo);
  end;

  if FButtonOption.FPageLastShow then
  begin
    New(ButtonInfo);

    if Length(FButtonOption.FPageLastText)  = 0 then
      ButtonInfo.Caption := '»'
    else
      ButtonInfo.Caption := FButtonOption.FPageLastText;

    ButtonInfo.ButtonType := pbtPageLast;
    ButtonInfo.Enabled := FCurrentPage < FPageCount;
    FPageButtons.Add(ButtonInfo);
  end;

  L := MulDiv(FButtonLeftMargin, FCurrentPPI, 96);
  T := MulDiv(FButtonTopMargin, FCurrentPPI, 96);
  FAllButtonWidth := 0;
  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];
    W := Canvas.TextWidth(ButtonInfo.Caption);
    W := Max(W + FButtonOption.FHorzSpaceExpand * 2, FButtonOption.FMinWidth);
    ButtonInfo.ButtonRect := Rect(L, T, L + W, T + FButtonOption.FHeight);
    L := L + W + FButtonOption.FButtonMargin;
    Inc(FAllButtonWidth, W + FButtonOption.FButtonMargin);
  end;
  Dec(FAllButtonWidth, FButtonOption.FButtonMargin);

  FIsRecallPageButtons := True;
end;

procedure TPageNumCtrl.Resize;
begin
  inherited;
  FDrawBitmap.Width := Width;
  FDrawBitmap.Height := Height;
end;

procedure TPageNumCtrl.WMEraseBkgnd(var Message: TWmEraseBkgnd);
begin
  //message.Result := 1;
  inherited;
end;

{
procedure TPageNumCtrl.DoMouseLeave;
begin
  inherited;
  FActivePageButton := nil;
  FMouseDownPageButton := nil;
end;
}

procedure TPageNumCtrl.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  I: Integer;
  ButtonInfo: PPageButtonInfo;
  TempButton: PPageButtonInfo;
begin
  inherited;

  TempButton := nil;
  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];

    if PtInRect(ButtonInfo.ButtonRect, Point(X, Y)) then
    begin
      TempButton := ButtonInfo;
      Break;
    end;
  end;

  if (FMouseDownPageButton <> TempButton) and (TempButton <> nil) and (TempButton.Enabled) then
  begin
    FMouseDownPageButton := TempButton;
    Repaint;
  end;
end;

procedure TPageNumCtrl.MouseMove(Shift: TShiftState; X, Y: Integer);
var
  I: Integer;
  ButtonInfo: PPageButtonInfo;
  TempButton: PPageButtonInfo;
begin
  inherited;

  TempButton := nil;
  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];

    if PtInRect(ButtonInfo.ButtonRect, Point(X, Y)) then
    begin
      TempButton := ButtonInfo;
      Break;
    end;
  end;

  if (FActivePageButton <> TempButton) then
  begin
    FActivePageButton := TempButton;
    Invalidate;
  end;

  if FMouseDownPageButton <> TempButton then
  begin
    FMouseDownPageButton := nil;
    Invalidate;
  end;
end;

procedure TPageNumCtrl.MouseUp(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
var
  I: Integer;
  ButtonInfo: PPageButtonInfo;
  TempButton: PPageButtonInfo;
begin
  inherited;

  TempButton := nil;
  for I := 0 to FPageButtons.Count - 1 do
  begin
    ButtonInfo := FPageButtons.Items[I];

    if PtInRect(ButtonInfo.ButtonRect, Point(X, Y)) then
    begin
      TempButton := ButtonInfo;
      Break;
    end;
  end;

  if (TempButton <> nil) and (TempButton = FMouseDownPageButton) then
  begin

    if FMouseDownPageButton.ButtonType = pbtPageFirst then
      DoPageNumClick(1)
    else if (FMouseDownPageButton.ButtonType = pbtPagePrior) and (FCurrentPage > 1) then
      DoPageNumClick(FCurrentPage - 1)
    else if (FMouseDownPageButton.ButtonType = pbtPageNum) then
      DoPageNumClick(FMouseDownPageButton.ButtonValue)
    else if (FMouseDownPageButton.ButtonType = pbtPageNext) and (FCurrentPage < FPageCount) then
      DoPageNumClick(FCurrentPage + 1)
    else if FMouseDownPageButton.ButtonType = pbtPageLast then
      DoPageNumClick(FPageCount);

    Invalidate;
  end;
end;

procedure TPageNumCtrl.SetButtonLeftMargin(const Value: Integer);
begin
  if FButtonLeftMargin <> Value then
  begin
    FButtonLeftMargin := Value;
    RecallPageButtons;
    Invalidate;
  end;
end;

procedure TPageNumCtrl.SetButtonTopMargin(const Value: Integer);
begin
  if FButtonTopMargin <> Value then
  begin
    FButtonTopMargin := Value;
    RecallPageButtons;
    Invalidate;
  end;
end;

procedure TPageNumCtrl.SetCurrentPage(const Value: Integer);
begin
  if FCurrentPage <> Value then
  begin
    FCurrentPage := Value;
    RecallPageButtons;
    Invalidate;
  end;
end;

procedure TPageNumCtrl.SetPageCount(const Value: Integer);
begin
  if FPageCount <> Value then
  begin
    FPageCount := Value;
    RecallPageButtons;
    Invalidate;
  end;
end;

procedure TPageNumCtrl.SetShowTextFont(const Value: TFont);
begin
  FShowTextFont.Assign(Value);
end;

end.
