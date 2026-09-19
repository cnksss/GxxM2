{------------------------------------------------------------------------------}
{                                                                              }
{  TRealTimeMarquee v3.5                                                       }
{  by Kambiz R. Khojasteh                                                      }
{                                                                              }
{  kambiz@delphiarea.com                                                       }
{  http://www.delphiarea.com                                                   }
{                                                                              }
{------------------------------------------------------------------------------}

{.$I DELPHIAREA.INC} 

unit RealTimeMarquee;

interface

uses
  Windows, Messages, Classes, Graphics, Controls, mmSystem, SimpleHTML;

type

  TRealTimeMarquee = class;

  TMarqueeStyle = (msText, msHTML, msOwnerDraw);
  TMarqueeTextStyle = msText..msHTML;

  TMarqueeAlignment = (maCenter, maLeftOrTop, maRightOrBottom);

  TMarqueeItem = class(TCollectionItem)
  private
    fText: WideString;
    fAlignment: TMarqueeAlignment;
    fStyle: TMarqueeStyle;
    fPrepared: Boolean;
    fBitmap: TBitmap;
    fLinks: TSimpleLinks;
    fSize: TSize;
    fTag: Integer;
    procedure SetText(const Value: WideString);
    procedure SetStyle(Value: TMarqueeStyle);
    procedure SetAlignment(Value: TMarqueeAlignment);
    function GetSize: Integer;
    function GetWidth: Integer;
    function GetHeight: Integer;
    function GetShowing: Boolean;
    function GetMarquee: TRealTimeMarquee;
    function HtmlImage(Sender: TObject; const URI: WideString): TPicture;
  protected
    function GetDisplayName: String; override;
    procedure Draw(DC: HDC; X, Y: Integer); virtual;
    function LinkAt(X, Y: Integer; out Link: WideString): Boolean; virtual;
    procedure Reset; virtual;
    procedure Prepare; virtual;
    procedure PrepareText; virtual;
    procedure PrepareHTML; virtual;
    procedure PrepareOwnerDraw; virtual;
    property Bitmap: TBitmap read fBitmap;
    property Size: Integer read GetSize;
    property Width: Integer read GetWidth;
    property Height: Integer read GetHeight;
    property Prepared: Boolean read fPrepared;
  public
    constructor Create(Collection: TCollection); override;
    destructor Destroy; override;
    procedure Assign(Source: TPersistent); override;
    property Marquee: TRealTimeMarquee read GetMarquee;
    property Showing: Boolean read GetShowing;
  published
    property Alignment: TMarqueeAlignment read fAlignment write SetAlignment default maCenter;
    property Style: TMarqueeStyle read fStyle write SetStyle default msText;
    property Text: WideString read fText write SetText;
    property Tag: Integer read fTag write fTag default 0;
  end;

  TMarqueeCollection = class(TOwnedCollection)
  private
    function GetMarquee: TRealTimeMarquee;
    function GetItems(Index: Integer): TMarqueeItem;
  protected
    procedure Update(Item: TCollectionItem); override;
    procedure Reset; virtual;
  public
    constructor Create(AOwner: TRealTimeMarquee);
    procedure LoadFromStrings(Strings: TStrings; TextStyle: TMarqueeTextStyle);
    procedure SaveToStrings(Strings: TStrings);
    procedure BeginUpdate; override;
    procedure EndUpdate; override;
    function Add: TMarqueeItem;
    function Insert(Index: Integer): TMarqueeItem;
    procedure Delete(Index: Integer);
    property Marquee: TRealTimeMarquee read GetMarquee;
    property Items[Index: Integer]: TMarqueeItem read GetItems; default;
  end;

  TPercent = 0..100;

  TMarqueeOrientation = (moHorizontal, moVertical);

  TMarqueeItemEvent = procedure(Sender: TObject;
    Item: TMarqueeItem) of object;

  TMarqueeDrawItemEvent = procedure(Sender: TObject;
    Item: TMarqueeItem; Bitmap: TBitmap) of object;

  TMarqueeLinkEvent = procedure(Sender: TObject;
    const Link: WideString) of object;

  TMarqueeImageEvent = procedure(Sender: TObject;
    const URI: WideString; Image: TPicture) of object;

  TRealTimeMarquee = class(TCustomControl)
  private
    fItems: TMarqueeCollection;
    fImages: TImageCache;
    fActive: Boolean;
    fInterval: Cardinal;
    fStep: Byte;
    fSpacing: TPercent;
    fLoopSpacing: TPercent;
    fNumLoops: Cardinal;
    fOrientation: TMarqueeOrientation;
    fVisibleCount: Cardinal;
    fOnWrap: TNotifyEvent;
    fOnLink: TMarqueeLinkEvent;
    fOnImage: TMarqueeImageEvent;
    fOnDrawItem: TMarqueeDrawItemEvent;
    fOnMouseEnter: TNotifyEvent;
    fOnMouseLeave: TNotifyEvent;
    Resolution: Cardinal;
    TimerID: Cardinal;
    OldestIndex: Integer;
    NewestIndex: Integer;
    NewestOffset: Integer;
    OldestOffset: Integer;
    LoopsStarted: Cardinal;
    LoopsEnded: Cardinal;
    ActiveLink: WideString;
    MouseIsInside: Boolean;
    GapBetweenItems: Integer;
    GapBetweenLoops: Integer;
    CS: TRTLCriticalSection;
    TimeCaps: TTimeCaps;
    procedure SetActive(Value: Boolean);
    procedure SetStep(Value: Byte);
    procedure SetInterval(Value: Cardinal);
    procedure SetSpacing(Value: TPercent);
    procedure SetLoopSpacing(Value: TPercent);
    procedure SetOrientation(Value: TMarqueeOrientation);
    procedure SetItems(Value: TMarqueeCollection);
  private
    procedure StartTimer;
    procedure StopTimer;
    procedure CheckTimer;
    procedure StepIt;
    procedure RecalcPositions;
    procedure UpdateRect(DC: HDC; const Rect: TRect);
    function LinkAt(const Pt: TPoint; out Link: WideString): Boolean;
    procedure ItemChanged(Item: TMarqueeItem);
  protected
    procedure Paint; override;
    procedure Loaded; override;
    procedure Click; override;
    procedure CreateWnd; override;
    procedure DestroyWnd; override;
    procedure WndProc(var Message: TMessage); override;
    procedure DoMouseEnter; virtual;
    procedure DoMouseLeave; virtual;
    procedure DoWrap; virtual;
    procedure DoLink(const Link: WideString); virtual;
    function DoGetImage(const URI: WideString): TPicture; virtual;
    procedure DoDrawItem(Item: TMarqueeItem; Bitmap: TBitmap); virtual;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function TryLock: Boolean;
    procedure Lock;
    procedure Unlock;
    procedure Reset;
    procedure Invalidate; override;
    property LoopCount: Cardinal read LoopsEnded;
    property VisibleCount: Cardinal read fVisibleCount;
  published
    property Active: Boolean read fActive write SetActive default False;
    property Align;
    property Anchors;
    property BiDiMode;
    property Color;
    property Constraints;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property Font;
    property Interval: Cardinal read fInterval write SetInterval default 10;
    property Items: TMarqueeCollection read fItems write SetItems;
    property NumLoops: Cardinal read fNumLoops write fNumLoops default 0;
    property LoopSpacing: TPercent read fLoopSpacing write SetLoopSpacing default 100;
    property Orientation: TMarqueeOrientation read fOrientation write SetOrientation default moHorizontal;
    property ParentBiDiMode;
    property ParentColor;
    property ParentCtl3D;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ShowHint;
    property Spacing: TPercent read fSpacing write SetSpacing default 100;
    property Step: Byte read fStep write SetStep default 1;
    property TabOrder;
    property TabStop;
    property Visible;
    property OnCanResize;
    property OnClick;
    property OnConstrainedResize;
    {$IFDEF COMPILER6_UP}
    property OnContextPopup;
    {$ENDIF}
    property OnDockDrop;
    property OnDockOver;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnDrawItem: TMarqueeDrawItemEvent read fOnDrawItem write fOnDrawItem;
    property OnEndDock;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnImage: TMarqueeImageEvent read fOnImage write fOnImage;
    property OnMouseDown;
    property OnMouseEnter: TNotifyEvent read fOnMouseEnter write fOnMouseEnter;
    property OnMouseLeave: TNotifyEvent read fOnMouseLeave write fOnMouseLeave;
    property OnMouseMove;
    property OnMouseUp;
    property OnLink: TMarqueeLinkEvent read fOnLink write fOnLink;
    property OnResize;
    property OnStartDock;
    property OnStartDrag;
    property OnUnDock;
    property OnWrap: TNotifyEvent read fOnWrap write fOnWrap;
  end;

  { TCompatibleCanvas }

  TCompatibleCanvas = class(TCanvas)
  public
    constructor Create;
    destructor Destroy; override;
  end;

procedure Register;

implementation

uses
  SysUtils, Forms, ShellAPI;

const
  CM_MARQUEE_WRAPPED = WM_USER;

procedure Register;
begin
  RegisterComponents('Delphi Area', [TRealTimeMarquee]);
end;

{ Fix for incorrect translation of Windows API }

function _ScrollDC(DC: HDC; DX, DY: Integer; Scroll, Clip: PRect;
  Rgn: HRGN; Update: PRect): BOOL; stdcall;
  external user32 name 'ScrollDC';

function _GetUpdateRect(hWnd: HWND; lpRect: PRect; bErase: BOOL): BOOL; stdcall;
  external user32 name 'GetUpdateRect';

{ Timer Callback Function }

procedure TimerProc(uTimerID, uMessage, dwUser, dw1, dw2: DWORD); stdcall;
begin
  TRealTimeMarquee(dwUser).StepIt;
end;

{ TMarqueeItem }

constructor TMarqueeItem.Create(Collection: TCollection);
begin
  inherited Create(Collection);
  fBitmap := TBitmap.Create;
end;

destructor TMarqueeItem.Destroy;
begin
  fBitmap.Free;
  if Assigned(fLinks) then
    fLinks.Free;
  inherited Destroy;
end;

procedure TMarqueeItem.Assign(Source: TPersistent);
begin
  if Source is TMarqueeItem then
  begin
    Marquee.Lock;
    try
      Text := TMarqueeItem(Source).Text;
      Style := TMarqueeItem(Source).Style;
    finally
      Marquee.Unlock;
    end;
  end
  else
    inherited Assign(Source);
end;

procedure TMarqueeItem.Draw(DC: HDC; X, Y: Integer);
begin
  if not Prepared then
    Prepare;
  if RectVisible(DC, Rect(X, Y, X + fSize.cx, Y + fSize.cy)) then
  begin
    Bitmap.Canvas.Lock;
    try
      BitBlt(DC, X, Y, fSize.cx, fSize.cy, Bitmap.Canvas.Handle, 0, 0, SRCCOPY);
    finally
      fBitmap.Canvas.Unlock;
    end;
  end;
end;

function TMarqueeItem.LinkAt(X, Y: Integer; out Link: WideString): Boolean;
var
  I: Integer;
begin
  Result := False;
  if Prepared and (Style = msHTML) and (fLinks.Count <> 0) then
  begin
    I := fLinks.IndexOfLinkAt(X, Y);
    if I >= 0 then
    begin
      Link := fLinks[I];
      Result := True;
    end;
  end;
end;

function TMarqueeItem.HtmlImage(Sender: TObject; const URI: WideString): TPicture;
begin
  Result := Marquee.DoGetImage(URI);
end;

function TMarqueeItem.GetDisplayName: String;
begin
  Result := Text;
  if Result = '' then
    Result := inherited GetDisplayName;
end;

function TMarqueeItem.GetMarquee: TRealTimeMarquee;
begin
  Result := TMarqueeCollection(Collection).Marquee;
end;

function TMarqueeItem.GetSize: Integer;
begin
  if not Prepared then
    Prepare;
  if Marquee.Orientation = moHorizontal then
    Result := fSize.cx
  else
    Result := fSize.cy;
end;

function TMarqueeItem.GetWidth: Integer;
begin
  if not Prepared then
    Prepare;
  Result := fSize.cx;
end;

function TMarqueeItem.GetHeight: Integer;
begin
  if not Prepared then
    Prepare;
  Result := fSize.cy;
end;

function TMarqueeItem.GetShowing: Boolean;
begin
  with Marquee do
  begin
    if VisibleCount = 0 then
      Result := False
    else if VisibleCount > Cardinal(Items.Count) then
      Result := True
    else if OldestIndex <= NewestIndex then
      Result := (Index >= OldestIndex) and (Index <= NewestIndex)
    else
      Result := (Index >= OldestIndex) or (Index <= NewestIndex);
  end;
end;

procedure TMarqueeItem.SetText(const Value: WideString);
begin
  if Text <> Value then
  begin
    Marquee.Lock;
    try
      fText := Value;
      if Prepared then
        Reset;
      Changed(False);
    finally
      Marquee.Unlock;
    end;
  end;
end;

procedure TMarqueeItem.SetStyle(Value: TMarqueeStyle);
begin
  if Style <> Value then
  begin
    Marquee.Lock;
    try
      fStyle := Value;
      if Prepared then
        Reset;
      Changed(False);
    finally
      Marquee.Unlock;
    end;
  end;
end;

procedure TMarqueeItem.SetAlignment(Value: TMarqueeAlignment);
begin
  if Alignment <> Value then
  begin
    Marquee.Lock;
    try
      fAlignment := Value;
      if Marquee.Orientation = moVertical then
        Reset;
      Changed(False);
    finally
      Marquee.Unlock;
    end;
  end;
end;

procedure TMarqueeItem.Prepare;
begin
  if Assigned(fLinks) then
    fLinks.Clear;
  Bitmap.Canvas.Lock;
  try
    case Style of
      msText: PrepareText;
      msHTML: PrepareHTML;
      msOwnerDraw: PrepareOwnerDraw;
    end;
  finally
    Bitmap.Canvas.Unlock;
  end;
  fPrepared := True;
end;

procedure TMarqueeItem.PrepareText;
var
  Rect: TRect;
  tm: TTextMetricW;
  OldFont: HFONT;
  DC: HDC;
  DrawFlags: Cardinal;
begin
  DrawFlags := DT_CENTER;
  if Marquee.Orientation = moVertical then
  begin
    case Alignment of
      maLeftOrTop: DrawFlags := DT_LEFT;
      maRightOrBottom: DrawFlags := DT_RIGHT;
    end;
    DrawFlags := DrawFlags or DT_WORDBREAK;
  end
  else
    DrawFlags := DrawFlags or DT_SINGLELINE;
  DrawFlags := DrawFlags or DT_NOPREFIX or DT_NOCLIP or DT_EXPANDTABS;
  DC := CreateCompatibleDC(0);
  try
    OldFont := SelectObject(DC, Marquee.Font.Handle);
    try
      GetTextMetricsW(DC, tm);
      if Marquee.Orientation = moHorizontal then
        SetRect(Rect, 0, 0, MaxInt, 0)
      else
        SetRect(Rect, 0, 0, Marquee.ClientWidth - tm.tmAveCharWidth, 0);
      DrawTextW(DC, PWideChar(Text), Length(Text), Rect,
        Marquee.DrawTextBiDiModeFlags(DrawFlags or DT_CALCRECT));
    finally
      SelectObject(DC, OldFont);
    end;
  finally
    DeleteDC(DC);
  end;
  Inc(Rect.Right, tm.tmAveCharWidth);
  fSize.cx := Rect.Right - Rect.Left;
  fSize.cy := Rect.Bottom - Rect.Top;
  Bitmap.Width := fSize.cx;
  Bitmap.Height := fSize.cy;
  Bitmap.Canvas.Brush.Color := Marquee.Color;
  Bitmap.Canvas.Font := Marquee.Font;
  Bitmap.Canvas.FillRect(Rect);
  Bitmap.Canvas.Brush.Style := bsClear;
  InflateRect(Rect, -tm.tmAveCharWidth div 2, 0);
  DrawTextW(Bitmap.Canvas.Handle, PWideChar(Text), Length(Text), Rect,
    Marquee.DrawTextBiDiModeFlags(DrawFlags));
end;

procedure TMarqueeItem.PrepareHTML;
var
  HTML: TSimpleHTML;
  tm: TTextMetricW;
  Rect: TRect;
  Canvas: TCanvas;
begin
  HTML := TSimpleHTML.Create(Text);
  try
    HTML.OnAdvanceGetImage := HtmlImage;
    Canvas := TCompatibleCanvas.Create;
    try
      Canvas.Font := Marquee.Font;
      fSize := HTML.Extend(Canvas);
      GetTextMetricsW(Canvas.Handle, tm);
    finally
      Canvas.Free;
    end;
    Inc(fSize.cx, tm.tmAveCharWidth);
    Rect.Left := 0;
    Rect.Top := 0;
    Rect.Right := fSize.cx;
    Rect.Bottom := fSize.cy;
    Bitmap.Width := fSize.cx;
    Bitmap.Height := fSize.cy;
    Bitmap.Canvas.Brush.Color := Marquee.Color;
    Bitmap.Canvas.Font := Marquee.Font;
    Bitmap.Canvas.FillRect(Rect);
    if Marquee.UseRightToLeftReading then
      Bitmap.Canvas.TextFlags := Bitmap.Canvas.TextFlags or ETO_RTLREADING
    else
      Bitmap.Canvas.TextFlags := Bitmap.Canvas.TextFlags and not ETO_RTLREADING;
    InflateRect(Rect, -tm.tmAveCharWidth div 2, 0);
    HTML.Draw(Bitmap.Canvas, clBlue, Rect);
    if not Assigned(fLinks) then
      fLinks := TSimpleLinks.Create;
    HTML.CollectLinks(Bitmap.Canvas, Rect, fLinks);
  finally
    HTML.Free;
  end;
end;

procedure TMarqueeItem.PrepareOwnerDraw;
begin
  Bitmap.Width := 0;
  Bitmap.Height := 0;
  Bitmap.Canvas.Brush.Color := Marquee.Color;
  Bitmap.Canvas.Font := Marquee.Font;
  Marquee.DoDrawItem(Self, fBitmap);
  fSize.cx := fBitmap.Width;
  fSize.cy := fBitmap.Height;
end;

procedure TMarqueeItem.Reset;
begin
  Marquee.Lock;
  try
    if Prepared then
    begin
      fPrepared := False;
      if Assigned(fLinks) then
        fLinks.Clear;
      fBitmap.Handle := 0;
      FillChar(fSize, SizeOf(fSize), 0);
    end;
  finally
    Marquee.Unlock;
  end;
end;

{ TMarqueeCollection }

constructor TMarqueeCollection.Create(AOwner: TRealTimeMarquee);
begin
  inherited Create(AOwner, TMarqueeItem);
end;

function TMarqueeCollection.GetMarquee: TRealTimeMarquee;
begin
  Result := TRealTimeMarquee(GetOwner);
end;

function TMarqueeCollection.GetItems(Index: Integer): TMarqueeItem;
begin
  Result := TMarqueeItem(inherited Items[Index]);
end;

procedure TMarqueeCollection.Update(Item: TCollectionItem);
begin
  Marquee.ItemChanged(TMarqueeItem(Item));
end;

procedure TMarqueeCollection.BeginUpdate;
begin
  Marquee.Lock;
  inherited BeginUpdate;
end;

procedure TMarqueeCollection.EndUpdate;
begin
  inherited EndUpdate;
  Marquee.Unlock;
end;

function TMarqueeCollection.Add: TMarqueeItem;
begin
  Marquee.Lock;
  try
    Result := TMarqueeItem(inherited Add);
  finally
    Marquee.Unlock;
  end;
end;

function TMarqueeCollection.Insert(Index: Integer): TMarqueeItem;
begin
  Marquee.Lock;
  try
    Result := TMarqueeItem(inherited Insert(Index));
  finally
    Marquee.Unlock;
  end;
end;

procedure TMarqueeCollection.Delete(Index: Integer);
begin
  Marquee.Lock;
  try
    {$IFDEF COLPILER6_UP}
    inherited Delete(Index);
    {$ELSE}
    Items[Index].Free;
    {$ENDIF}
  finally
    Marquee.Unlock;
  end;
end;

procedure TMarqueeCollection.Reset;
var
  I: Integer;
begin
  Marquee.Lock;
  try
    for I := 0 to Count - 1 do
      Items[I].Reset;
  finally
    Marquee.Unlock;
  end;
end;

procedure TMarqueeCollection.LoadFromStrings(Strings: TStrings;
  TextStyle: TMarqueeTextStyle);
var
  I: Integer;
begin
  BeginUpdate;
  try
    Clear;
    for I := 0 to Strings.Count - 1 do
      with Add do
      begin
        Text := Strings[I];
        Style := TextStyle;
      end;
  finally
    EndUpdate;
  end;
end;

procedure TMarqueeCollection.SaveToStrings(Strings: TStrings);
var
  I: Integer;
begin
  Strings.BeginUpdate;
  try
    Strings.Clear;
    for I := 0 to Count - 1 do
      Strings.Add(Items[I].Text);
  finally
    Strings.EndUpdate;
  end;
end;

{ TRealTimeMarquee }

constructor TRealTimeMarquee.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  InitializeCriticalSection(CS);
  timeGetDevCaps(@TimeCaps, SizeOf(TTimeCaps));
  fItems := TMarqueeCollection.Create(Self);
  fImages := TImageCache.Create;
  fOrientation := moHorizontal;
  fLoopSpacing := 100;
  fSpacing := 100;
  fStep := 1;
  Interval := 10;
  Width := 200;
  Height := 20;
end;

destructor TRealTimeMarquee.Destroy;
begin
  if HandleAllocated then
    DestroyWnd
  else
    StopTimer;
  fImages.Free;
  fItems.Free;
  DeleteCriticalSection(CS);
  inherited Destroy;
end;

procedure TRealTimeMarquee.SetActive(Value: Boolean);
begin
  if Active <> Value then
  begin
    Lock;
    try
      fActive := Value;
      CheckTimer;
    finally
      Unlock;
    end;
  end;
end;

procedure TRealTimeMarquee.SetOrientation(Value: TMarqueeOrientation);
begin
  if Orientation <> Value then
  begin
    Lock;
    try
      fOrientation := Value;
      if HandleAllocated then
      begin
        if Orientation = moHorizontal then
        begin
          GapBetweenItems := MulDiv(ClientWidth, Spacing, 100);
          GapBetweenLoops := MulDiv(ClientWidth, LoopSpacing, 100);
        end
        else
        begin
          GapBetweenItems := MulDiv(ClientHeight, Spacing, 100);
          GapBetweenLoops := MulDiv(ClientHeight, LoopSpacing, 100);
        end;
      end;
      Reset;
    finally
      Unlock;
    end;
  end;
end;

procedure TRealTimeMarquee.SetLoopSpacing(Value: TPercent);
begin
  if LoopSpacing <> Value then
  begin
    fLoopSpacing := Value;
    if HandleAllocated then
    begin
      Lock;
      try
        if Orientation = moHorizontal then
          GapBetweenLoops := MulDiv(ClientWidth, LoopSpacing, 100)
        else
          GapBetweenLoops := MulDiv(ClientHeight, LoopSpacing, 100);
        if VisibleCount <> 0 then
        begin
          RecalcPositions;
          Invalidate;
        end;
      finally
        Unlock;
      end;
    end;
  end;
end;

procedure TRealTimeMarquee.SetSpacing(Value: TPercent);
begin
  if Spacing <> Value then
  begin
    fSpacing := Value;
    if HandleAllocated then
    begin
      Lock;
      try
        if Orientation = moHorizontal then
          GapBetweenItems := MulDiv(ClientWidth, Spacing, 100)
        else
          GapBetweenItems := MulDiv(ClientHeight, Spacing, 100);
        if VisibleCount <> 0 then
        begin
          RecalcPositions;
          Invalidate;
        end;
      finally
        Unlock;
      end;
    end;
  end;
end;

procedure TRealTimeMarquee.SetStep(Value: Byte);
begin
  if Value = 0 then
    Value := 1;
  if Step <> Value then
  begin
    Lock;
    try
      fStep := Value;
    finally
      Unlock;
    end;
  end
end;

procedure TRealTimeMarquee.SetInterval(Value: Cardinal);
begin
  if Value < TimeCaps.wPeriodMin then
    Value := TimeCaps.wPeriodMin
  else if Value > TimeCaps.wPeriodMax then
    Value := TimeCaps.wPeriodMax;
  if Interval <> Value then
  begin
    StopTimer;
    fInterval := Value;
    Resolution := Interval div 10;
    if Resolution < TimeCaps.wPeriodMin then
      Resolution := TimeCaps.wPeriodMin;
    CheckTimer;
  end;
end;

procedure TRealTimeMarquee.SetItems(Value: TMarqueeCollection);
begin
  Items.Assign(Value);
end;

procedure TRealTimeMarquee.ItemChanged(Item: TMarqueeItem);
begin
  Lock;
  try
    if not Assigned(Item) then
    begin
      if VisibleCount <> 0 then
      begin
        RecalcPositions;
        Invalidate;
      end;
      CheckTimer;
    end
    else if Item.Showing then
      Invalidate;
   finally
    Unlock;
  end;
end;

procedure TRealTimeMarquee.StepIt;
var
  DC: HDC;
  Rect: TRect;
  Wrapped: Boolean;
  ClientSize: Integer;
  Gap: Integer;
begin
  if not TryLock then
    Exit;
  try
    if (TimerID = 0) or (Items.Count = 0) then
      Exit;
    Wrapped := False;
    Rect := ClientRect;
    if Orientation = moHorizontal then
      ClientSize := Rect.Right - Rect.Left
    else
      ClientSize := Rect.Bottom - Rect.Top;
    DC := GetDC(WindowHandle);
    try
      // no visible item? make the first item visible.
      if VisibleCount = 0 then
      begin
        OldestIndex := 0;
        NewestIndex := 0;
        OldestOffset := 0;
        NewestOffset := 0;
        Inc(LoopsStarted);
        Inc(fVisibleCount);
      end;
      // scroll content
      Inc(OldestOffset, Step);
      Inc(NewestOffset, Step);
      if not _GetUpdateRect(WindowHandle, nil, False) and
         not (csDesigning in ComponentState) then
      begin
        if Orientation = moVertical then
        begin
          _ScrollDC(DC, 0, -Step, @Rect, nil, 0, nil);
          Rect.Top := Rect.Bottom - Step;
        end
        else if UseRightToLeftAlignment then
        begin
          _ScrollDC(DC, +Step, 0, @Rect, nil, 0, nil);
          Rect.Right := Rect.Left + Step;
        end
        else
        begin
          _ScrollDC(DC, -Step, 0, @Rect, nil, 0, nil);
          Rect.Left := Rect.Right - Step;
        end;
      end;
      // did the oldest item get off the screen?
      if OldestOffset >= Items[OldestIndex].Size + ClientSize then
      begin
        Dec(OldestOffset, Items[OldestIndex].Size);
        Dec(fVisibleCount);
        Inc(OldestIndex);
        if OldestIndex >= Items.Count then
        begin
          OldestIndex := 0;
          Wrapped := True;
          Inc(LoopsEnded);
          if (NumLoops <> 0) and (LoopsEnded >= NumLoops) then
          begin
            fActive := False;
            LoopsStarted := 0;
            LoopsEnded := 0;
          end;
          Dec(OldestOffset, GapBetweenLoops);
        end
        else
          Dec(OldestOffset, GapBetweenItems);
      end;
      // does the next item get on the screen
      if Active then
      begin
        if NewestIndex = Items.Count - 1 then
          Gap := GapBetweenLoops
        else
          Gap := GapBetweenItems;
        if (NewestOffset >= Items[NewestIndex].Size + Gap) and
           ((NumLoops = 0) or (LoopsStarted < NumLoops) or (NewestIndex < Items.Count - 1)) then
        begin
          Dec(NewestOffset, Items[NewestIndex].Size);
          Dec(NewestOffset, Gap);
          Inc(NewestIndex);
          if NewestIndex = Items.Count then
          begin
            NewestIndex := 0;
            Inc(LoopsStarted);
          end;
          Inc(fVisibleCount);
        end;
      end;
      // update screen
      UpdateRect(DC, Rect);
    finally
      ReleaseDC(WindowHandle, DC)
    end;
  finally
    Unlock;
  end;
  if MouseIsInside and Enabled and not Dragging then
    PostMessage(WindowHandle, WM_SETCURSOR, WindowHandle, HTCLIENT);
  if Wrapped then
    PostMessage(WindowHandle, CM_MARQUEE_WRAPPED, 0, 0);
end;

procedure TRealTimeMarquee.UpdateRect(DC: HDC; const Rect: TRect);
var
  X, Y, W, H, I, N: Integer;
  Item: TMarqueeItem;
begin
  FillRect(DC, Rect, Brush.Handle);
  if VisibleCount <> 0 then
  begin
    with ClientRect do
    begin
      W := Right - Left;
      H := Bottom - Top;
    end;
    I := NewestIndex;
    Item := Items[I];
    if Orientation = moVertical then
    begin
      Y := H - NewestOffset;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: X := (W - Item.Width) div 2;
          maLeftOrTop: X := 0;
        else
          X := W - Item.Width;
        end;
        Item.Draw(DC, X, Y);
        if I = 0 then
        begin
          I := Items.Count;
          Dec(Y, GapBetweenLoops);
        end
        else
          Dec(Y, GapBetweenItems);
        if Y < Rect.Top then
          Exit;
        Dec(I);
        Item := Items[I];
        Dec(Y, Item.Height);
      end;
    end
    else if UseRightToLeftAlignment then
    begin
      X := NewestOffset - Item.Width;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: Y := (H - Item.Height) div 2;
          maLeftOrTop: Y := 0;
        else
          Y := H - Item.Height;
        end;
        Item.Draw(DC, X, Y);
        Inc(X, Item.Width);
        if I = 0 then
        begin
          I := Items.Count;
          Inc(X, GapBetweenLoops);
        end
        else
          Inc(X, GapBetweenItems);
        if X > Rect.Right then
          Exit;
        Dec(I);
        Item := Items[I];
      end;
    end
    else
    begin
      X := W - NewestOffset;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: Y := (H - Item.Height) div 2;
          maLeftOrTop: Y := 0;
        else
          Y := H - Item.Height;
        end;
        Item.Draw(DC, X, Y);
        if I = 0 then
        begin
          I := Items.Count;
          Dec(X, GapBetweenLoops);
        end
        else
          Dec(X, GapBetweenItems);
        if X < Rect.Left then
          Exit;
        Dec(I);
        Item := Items[I];
        Dec(X, Item.Width);
      end;
    end;
  end;
end;

function TRealTimeMarquee.LinkAt(const Pt: TPoint; out Link: WideString): Boolean;
var
  X, Y, W, H, I, N: Integer;
  Item: TMarqueeItem;
begin
  Result := False;
  if VisibleCount <> 0 then
  begin
    with ClientRect do
    begin
      W := Right - Left;
      H := Bottom - Top;
    end;
    I := NewestIndex;
    Item := Items[I];
    if Orientation = moVertical then
    begin
      Y := H - NewestOffset;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: X := (W - Item.Width) div 2;
          maLeftOrTop: X := 0;
        else
          X := W - Item.Width;
        end;
        if Item.LinkAt(Pt.X - X, Pt.Y - Y, Link) then
        begin
          Result := True;
          Exit;
        end;
        if I = 0 then
        begin
          I := Items.Count;
          Dec(Y, GapBetweenLoops);
        end
        else
          Dec(Y, GapBetweenItems);
        Dec(I);
        Item := Items[I];
        Dec(Y, Item.Height);
      end;
    end
    else if UseRightToLeftAlignment then
    begin
      X := NewestOffset - Item.Width;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: Y := (H - Item.Height) div 2;
          maLeftOrTop: Y := 0;
        else
          Y := H - Item.Height;
        end;
        if Item.LinkAt(Pt.X - X, Pt.Y - Y, Link) then
        begin
          Result := True;
          Exit;
        end;
        Inc(X, Item.Width);
        if I = 0 then
        begin
          I := Items.Count;
          Inc(X, GapBetweenLoops);
        end
        else
          Inc(X, GapBetweenItems);
        Dec(I);
        Item := Items[I];
      end;
    end
    else
    begin
      X := W - NewestOffset;
      for N := 1 to VisibleCount do
      begin
        case Item.Alignment of
          maCenter: Y := (H - Item.Height) div 2;
          maLeftOrTop: Y := 0;
        else
          Y := H - Item.Height;
        end;
        if Item.LinkAt(Pt.X - X, Pt.Y - Y, Link) then
        begin
          Result := True;
          Exit;
        end;
        if I = 0 then
        begin
          I := Items.Count;
          Dec(X, GapBetweenLoops);
        end
        else
          Dec(X, GapBetweenItems);
        Dec(I);
        Item := Items[I];
        Dec(X, Item.Width);
      end;
    end;
  end;
end;

procedure TRealTimeMarquee.RecalcPositions;
var
  ClientSize: Integer;
  Offset, Gap, Loop: Integer;
begin
  if VisibleCount <> 0 then
  begin
    fVisibleCount := 0;
    if Items.Count = 0 then
      Exit;
    if Orientation = moHorizontal then
      ClientSize := ClientWidth
    else
      ClientSize := ClientHeight;
    if NewestIndex >= Items.Count then
      NewestIndex := Items.Count - 1;
    while (NewestIndex > 0) and not Items[NewestIndex].Prepared do
      Dec(NewestIndex);
    OldestIndex := NewestIndex;
    OldestOffset := NewestOffset;
    if OldestIndex = 0 then
      Gap := GapBetweenLoops
    else
      Gap := GapBetweenItems;
    Loop := LoopsStarted;
    Offset := OldestOffset - Items[OldestIndex].Size;
    while Items[OldestIndex].Prepared and (Offset + Gap < ClientSize) do
    begin
      Inc(fVisibleCount);
      Inc(Offset, Items[OldestIndex].Size);
      Inc(Offset, Gap);
      OldestOffset := Offset;
      if OldestIndex = 0 then
      begin
        OldestIndex := Items.Count - 1;
        Gap := GapBetweenLoops;
        Dec(Loop);
        if Loop = 0 then
          Exit;
      end
      else
      begin
        Dec(OldestIndex);
        Gap := GapBetweenItems;
      end;
    end;
  end;
end;

procedure TRealTimeMarquee.StartTimer;
begin
  Lock;
  try
    if (TimerID = 0) and HandleAllocated and not (csLoading in ComponentState) then
    begin
      timeBeginPeriod(Resolution);
      TimerID := timeSetEvent(Interval, Resolution, @TimerProc, DWORD(Self), TIME_PERIODIC);
    end;
  finally
    Unlock;
  end;
end;

procedure TRealTimeMarquee.StopTimer;
begin
  Lock;
  try
    if TimerID <> 0 then
    begin
      timeKillEvent(TimerID);
      timeEndPeriod(Resolution);
      TimerID := 0;
    end;
  finally
    Unlock;
  end;
end;

procedure TRealTimeMarquee.CheckTimer;
begin
  if Active and not (csLoading in ComponentState) and
     HandleAllocated and (Items.Count <> 0)
  then
    StartTimer
  else
    StopTimer;
end;

procedure TRealTimeMarquee.Paint;
begin
  Lock;
  try
    UpdateRect(Canvas.Handle, ClientRect);
  finally
    Unlock;
  end;
end;

procedure TRealTimeMarquee.Loaded;
begin
  inherited Loaded;
  CheckTimer;
end;

procedure TRealTimeMarquee.CreateWnd;
begin
  inherited CreateWnd;
  CheckTimer;
end;

procedure TRealTimeMarquee.DestroyWnd;
begin
  StopTimer;
  inherited DestroyWnd;
end;

procedure TRealTimeMarquee.WndProc(var Message: TMessage);
var
  Pt: TPoint;
begin
  case Message.Msg of
    CM_COLORCHANGED,
    CM_FONTCHANGED,
    CM_BIDIMODECHANGED:
    begin
      Lock;
      try
        Items.Reset;
        inherited WndProc(Message);
        RecalcPositions;
      finally
        Unlock;
      end;
    end;
    WM_SIZE:
    begin
      Lock;
      try
        inherited WndProc(Message);
        if Orientation = moHorizontal then
        begin
          GapBetweenItems := MulDiv(ClientWidth, Spacing, 100);
          GapBetweenLoops := MulDiv(ClientWidth, LoopSpacing, 100);
        end
        else
        begin
          Items.Reset;
          GapBetweenItems := MulDiv(ClientHeight, Spacing, 100);
          GapBetweenLoops := MulDiv(ClientHeight, LoopSpacing, 100);
        end;
        RecalcPositions;
      finally
        Unlock;
      end;
    end;
    WM_PAINT:
    begin
      Lock;
      try
        inherited WndProc(Message);
      finally
        Unlock;
      end;
    end;
    WM_ERASEBKGND:
      Message.Result := 1;
    WM_SETCURSOR:
    begin
      if not Dragging and Enabled then
      begin
        GetCursorPos(Pt);
        MapWindowPoints(0, WindowHandle, Pt, 1);
        if LinkAt(Pt, ActiveLink) then
        begin
          SetCursor(Screen.Cursors[crHandPoint]);
          Message.Result := 1;
        end;
      end;
      if Message.Result = 0 then
      begin
        ActiveLink := '';
        inherited WndProc(Message);
      end;
    end;
    WM_DESTROY:
    begin
      StopTimer;
      inherited WndProc(Message);
    end;
    CM_MOUSEENTER:
    begin
      MouseIsInside := True;
      DoMouseEnter;
    end;
    CM_MOUSELEAVE:
    begin
      MouseIsInside := False;
      DoMouseLeave;
    end;
    CM_MARQUEE_WRAPPED:
    begin
      DoWrap;
    end;
  else
    inherited WndProc(Message);
  end;
end;

procedure TRealTimeMarquee.Click;
begin
  inherited Click;
  if ActiveLink <> '' then
    DoLink(ActiveLink);
end;

procedure TRealTimeMarquee.Invalidate;
begin
  if HandleAllocated then
  begin
    inherited Invalidate;
    if MouseIsInside and Enabled and not Dragging then
      PostMessage(WindowHandle, WM_SETCURSOR, WindowHandle, HTCLIENT);
  end;
end;

procedure TRealTimeMarquee.Reset;
begin
  Lock;
  try
    fItems.Reset;
    fVisibleCount := 0;
    LoopsStarted := 0;
    LoopsEnded := 0;
    Invalidate;
  finally
    Unlock;
  end;
end;

function TRealTimeMarquee.TryLock: Boolean;
begin
  Result := TryEnterCriticalSection(CS);
end;

procedure TRealTimeMarquee.Lock;
begin
  EnterCriticalSection(CS);
end;

procedure TRealTimeMarquee.Unlock;
begin
  LeaveCriticalSection(CS);
end;

procedure TRealTimeMarquee.DoMouseEnter;
begin
  if Assigned(OnMouseEnter) then
    OnMouseEnter(Self);
end;

procedure TRealTimeMarquee.DoMouseLeave;
begin
  if Assigned(OnMouseLeave) then
    OnMouseLeave(Self);
end;

procedure TRealTimeMarquee.DoWrap;
begin
  if Assigned(OnWrap) then
    OnWrap(Self);
end;

procedure TRealTimeMarquee.DoLink(const Link: WideString);
begin
  if Assigned(OnLink) then
    OnLink(Self, Link)
  else if Link <> '' then
    ShellExecuteW(Handle, 'open', PWideChar(Link), nil, nil, SW_NORMAL);
end;

function TRealTimeMarquee.DoGetImage(const URI: WideString): TPicture;
begin
  Result := fImages.ImageOf(WideLowerCase(URI));
  if not Assigned(Result) then
  begin
    Result := TPicture.Create;
    fImages.Add(WideLowerCase(URI), Result);
    if Assigned(OnImage) then
      OnImage(Self, URI, Result)
    else if FileExists(URI) then
      try
        Result.LoadFromFile(URI);
      except
        // ignore exceptions
      end;
  end;
end;

procedure TRealTimeMarquee.DoDrawItem(Item: TMarqueeItem; Bitmap: TBitmap);
begin
  if Assigned(OnDrawItem) then
    OnDrawItem(Self, Item, Bitmap);
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
