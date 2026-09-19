unit HtmlLabel;

interface

uses
  Windows, SysUtils, Classes, Messages, Controls, Forms, Graphics, SimpleHTML;

type
  TLinkClickEvent = procedure(Sender: TObject; const Link: string) of object;

  THtmlLabel = class(TGraphicControl)
  private
    FBitmap: TBitmap;
    FSize: TSize;
    FLinks: TSimpleLinks;
    FActiveLink: string;
    FOnLinkClick: TLinkClickEvent;

    procedure PrepareHTML; virtual;
  protected
    procedure Paint; override;
    procedure AdjustSize; override;

    procedure AdjustBounds; dynamic;
    procedure DoLink(const Link: string); virtual;
    procedure Click; override;

    procedure WndProc(var Message: TMessage); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
  published
    property Color;
    property ParentColor;
    property Font;
    property Text;
    property Hint;

    property Align;
    property Anchors;
    property AutoSize;
    property Constraints;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property ParentShowHint;
    property PopupMenu;
    property ShowHint;
    
{$IF CompilerVersion > 22.0}
    property Touch;
    property OnGesture;
    property OnMouseActivate;
    property OnMouseDown;
    property OnMouseEnter;
    property OnMouseLeave;
{$IFEND}

    property Visible;
    property OnClick;
    property OnContextPopup;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDock;
    property OnEndDrag;

    property OnMouseMove;
    property OnMouseUp;
    property OnStartDock;
    property OnStartDrag;

    property OnLinkClick: TLinkClickEvent read  FOnLinkClick write FOnLinkClick;
  end;

  TCompatibleCanvas = class(TCanvas)
  public
    constructor Create;
    destructor Destroy; override;
  end;

implementation

{ THtmlLabel }

constructor THtmlLabel.Create(AOwner: TComponent);
begin
  inherited;
  FBitmap := TBitmap.Create;
  FLinks := TSimpleLinks.Create;
  FActiveLink := '';
end;

destructor THtmlLabel.Destroy;
begin
  FBitmap.Free;
  FLinks.Free;
  inherited;
end;

procedure THtmlLabel.DoLink(const Link: string);
begin
  if Assigned(FOnLinkClick) then
  begin
    OnLinkClick(Self, FActiveLink);
  end;
end;

procedure THtmlLabel.Click;
begin
  inherited;
  if Length(FActiveLink) > 0 then
  begin
    DoLink(FActiveLink);
  end;
end;

procedure THtmlLabel.MouseMove(Shift: TShiftState; X, Y: Integer);
var
  Index: Integer;
begin
  inherited;
  FActiveLink := '';
  if (not Dragging) and Enabled and (FLinks.Count <> 0) then
  begin
    Index := FLinks.IndexOfLinkAt(X, Y - (Height - FBitmap.Height) div 2);
    if Index >= 0 then
    begin
      FActiveLink := FLinks[Index];
      SetCursor(Screen.Cursors[crHandPoint]);
    end;
  end;
end;

procedure THtmlLabel.Paint;
begin
  inherited;
  Canvas.Draw(0, (Height - FBitmap.Height) div 2, FBitmap);
end;

procedure THtmlLabel.PrepareHTML;
var
  HTML: TSimpleHTML;
  Metric: TTextMetricW;
  Rect: TRect;
  Canvas: TCanvas;
  TextFlags: LongInt;
begin
  HTML := TSimpleHTML.Create(Text);
  try
    //HTML.OnAdvanceGetImage := HtmlImage;
    Canvas := TCompatibleCanvas.Create;
    try
      Canvas.Font := Font;
      FSize := HTML.Extend(Canvas);
      GetTextMetricsW(Canvas.Handle, Metric);
    finally
      Canvas.Free;
    end;

    // 2边留空不要
    //Inc(FSize.cx, Metric.tmAveCharWidth);

    Rect.Left := 0;
    Rect.Top := 0;
    Rect.Right := FSize.cx;
    Rect.Bottom := FSize.cy;
    FBitmap.Width := FSize.cx;
    FBitmap.Height := FSize.cy;
    FBitmap.Canvas.Brush.Color := Color;
    FBitmap.Canvas.Font := Font;
    FBitmap.Canvas.FillRect(Rect);

    if UseRightToLeftReading then
      TextFlags := TextFlags or ETO_RTLREADING
    else
      TextFlags := TextFlags and not ETO_RTLREADING;

    FBitmap.Canvas.TextFlags := TextFlags;

    // 2边留空不要
    //InflateRect(Rect, -Metric.tmAveCharWidth div 2, 0);

    HTML.Draw(FBitmap.Canvas, clBlue, Rect);
    HTML.CollectLinks(FBitmap.Canvas, Rect, FLinks);
  finally
    HTML.Free;
  end;
end;

procedure THtmlLabel.AdjustBounds;
begin
  if AutoSize then
  begin
    Width := FSize.cx;
    Height := FSize.cy;
  end;
end;

procedure THtmlLabel.AdjustSize;
begin
  inherited;
  AdjustBounds;
end;

procedure THtmlLabel.WndProc(var Message: TMessage);
begin
  case Message.Msg of
    CM_COLORCHANGED,
    CM_FONTCHANGED,
    CM_TEXTCHANGED,
    CM_BIDIMODECHANGED:
    begin
      PrepareHTML;
      AdjustBounds;
      Invalidate;
    end;
  else
    inherited WndProc(Message);
  end;

end;

{ TCompatibleCanvas }

constructor TCompatibleCanvas.Create;
begin
  inherited Create;
  Handle := CreateCompatibleDC(0);
end;

destructor TCompatibleCanvas.Destroy;
var
  DC: HDC;
begin
  DC := Handle;
  Handle := 0;
  if DC <> 0 then
    DeleteObject(DC);
  inherited Destroy;
end;

end.
