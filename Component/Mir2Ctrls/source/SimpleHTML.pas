{------------------------------------------------------------------------------}
{                                                                              }
{  SimpleHTML -- Single line HTML renderer                                     }
{  by Kambiz R. Khojasteh                                                      }
{                                                                              }
{  kambiz@delphiarea.com                                                       }
{  http://www.delphiarea.com                                                   }
{                                                                              }
{------------------------------------------------------------------------------}

{
  添加链接默认文字颜色: LinkFontColor chongchong 2018-05-07
}

unit SimpleHTML;

interface

uses
  Windows, SysUtils, Classes, Graphics;

const
  IMAGE_CACHE_BUCKET_SIZE = 32;

type

  TSimpleHTML = class;

  { TImageCache }

  PPImageCacheItem = ^PImageCacheItem;
  PImageCacheItem = ^TImageCacheItem;
  TImageCacheItem = record
    Next: PImageCacheItem;
    Image: TPicture;
    URI: WideString;
  end;

  TImageCacheCallback = procedure(Sender: TObject;
    const URI: WideString; Image: TPicture; UserData: Integer;
    var Continue: Boolean) of object;

  TImageCache = class
  private
    Buckets: array[0..IMAGE_CACHE_BUCKET_SIZE-1] of PImageCacheItem;
    fOwnsImages: Boolean;
  protected
    function HashOf(const URI: WideString): DWORD; virtual;
    function Find(const URI: WideString): PPImageCacheItem;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure Add(const URI: WideString; Image: TPicture);
    function Modify(const URI: WideString; Image: TPicture): Boolean;
    function Delete(const URI: WideString): Boolean;
    function ImageOf(const URI: WideString): TPicture;
    function ForEach(Callback: TImageCacheCallback; UserData: Integer = 0): Boolean;
    property OwnsImages: Boolean read fOwnsImages write fOwnsImages;
  end;

  { TCanvasRecall }

  TCanvasRecall = class(TObject)
  private
    fPen: TPen;
    fFont: TFont;
    fBrush: TBrush;
    fCopyMode: TCopyMode;
    fTextFlags: Integer;
    fReference: TCanvas;
    procedure SetReference(Value: TCanvas);
  public
    constructor Create(AReference: TCanvas);
    destructor Destroy; override;
    procedure Store;
    procedure Retrieve;
    property Reference: TCanvas read fReference write SetReference;
  end;

  { TSimpleLinks }

  TSimpleLinks = class(TObject)
  private
    fItems: TStringList;
    function GetCount: Integer;
    function GetBounds(Index: Integer): TRect;
    function GetRegions(Index: Integer): HRGN;
    function GetLinks(Index: Integer): WideString;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure Add(const Link: WideString; const Rect: TRect); overload;
    procedure Add(const Link: WideString; Rgn: HRGN); overload;
    function IndexOfLinkAt(X, Y: Integer): Integer;
    property Count: Integer read GetCount;
    property Bounds[Index: Integer]: TRect read GetBounds;
    property Regions[Index: Integer]: HRGN read GetRegions;
    property Links[Index: Integer]: WideString read GetLinks; default;
  end;

  { TSimpleHTMLNode }

  TSimpleHTMLNode = class(TObject)
  private
    fParent: TSimpleHTMLNode;
    fTagName: WideString;
    fText: WideString;
    fChildren: TList;
    fAttributes: TStrings;
    function GetAttributes(const Name: WideString): WideString;
    function GetChildCount: Integer;
    function GetChildren(Index: Integer): TSimpleHTMLNode;
    function GetIndex: Integer;
    function GetInnerText: WideString;
    function GetInnerHTML: WideString;
    function GetOuterHTML: WideString;
  protected
    constructor Create(AParent: TSimpleHTMLNode; ATagName: WideString);
    function Parse(const HTML: WideString): Boolean;
    procedure Prepare(Canvas: TCanvas; LinkFontColor: TColor);
    function GetLinks(Canvas: TCanvas; var Rect: TRect; Links: TSimpleLinks): Integer;
    function GetDocument: TSimpleHTML; virtual;
  public
    destructor Destroy; override;
    function Extend(Canvas: TCanvas): TSize;
    procedure Draw(Canvas: TCanvas; LinkFontColor: TColor; Rect: TRect);
    function HasAttribute(const Name: WideString): Boolean;
    function HasChildren: Boolean;
    function AddChild(const ATagName: WideString): TSimpleHTMLNode;
    procedure RemoveChild(Index: Integer);
    procedure RemoveAllChildren;
    property Index: Integer read GetIndex;
    property Parent: TSimpleHTMLNode read fParent;
    property TagName: WideString read fTagName;
    property Text: WideString read fText;
    property InnerText: WideString read GetInnerText;
    property InnerHTML: WideString read GetInnerHTML;
    property OuterHTML: WideString read GetOuterHTML;
    property Document: TSimpleHTML read GetDocument;
    property ChildCount: Integer read GetChildCount;
    property Children[Index: Integer]: TSimpleHTMLNode read GetChildren;
    property Attributes[const Name: WideString]: WideString read GetAttributes;
  end;

  { TSimpleHTML }

  TGetImageEvent = procedure(Sender: TObject;
    const URI: WideString; Image: TPicture) of object;

  TAdvanceGetImageEvent = function(Sender: TObject;
    const URI: WideString): TPicture of object;

  TSimpleHTML = class(TSimpleHTMLNode)
  private
    fParsed: Boolean;
    fImages: TImageCache;
    fOnGetImage: TGetImageEvent;
    fOnAdvanceGetImage: TAdvanceGetImageEvent;
  protected
    function GetImage(const URI: WideString): TPicture;
    function GetDocument: TSimpleHTML; override;
  public
    constructor Create(const HTML: WideString);
    destructor Destroy; override;
    function CollectLinks(Canvas: TCanvas; const Rect: TRect; Links: TSimpleLinks): Integer;
    property IsValid: Boolean read fParsed;
    property OnGetImage: TGetImageEvent read fOnGetImage write fOnGetImage;
    property OnAdvanceGetImage: TAdvanceGetImageEvent read fOnAdvanceGetImage write fOnAdvanceGetImage;
  end;

{ Helper Functions }

function EncodeWebChar(C: WideChar; out Code: WideString): Boolean;
function DecodeWebChar(const Code: WideString; out C: WideChar): Boolean;

function DecodeHtmlEntities(const Str: WideString): WideString;
function EncodeHtmlEntities(const Str: WideString): WideString;

function DecodeWebColor(const WebColor: String; DefColor: TColor): TColor;
function EncodeWebColor(Color: TColor): String;

procedure DrawHTML(Canvas: TCanvas; const Rect: TRect; const Text: WideString);

implementation

const
  CodedChars: array[1..60] of WideString = (
    'nbsp', 'copy', 'reg', 'trade', 'sup1', 'sup2', 'sup3', 'quot', 'amp',
    'lt', 'gt', 'ndash', 'mdash', 'lsquo', 'rsquo', 'ldquo', 'rdquo',
    'bull', 'dagger', 'Dagger', 'prime', 'Prime', 'lsaquo', 'rsaquo',
    'tilde', 'circ', 'spades', 'clubs', 'hearts', 'diams', 'loz', 'larr',
    'rarr', 'uarr', 'darr', 'harr', 'not', 'frac14', 'frac12', 'frac34',
    'plusmn', 'laquo', 'raquo', 'deg', 'ordf', 'ordm', 'iexcl', 'iquest',
    'euro', 'cent', 'pound', 'yen', 'curren', 'sect', 'para', 'macr',
    'middot', 'micro', 'times', 'divide');

  SpecialChars: array[1..60] of WideChar = (
    #0160, #0169, #0174, #8482, #0185, #0178, #0179, #0034, #0038, #0060,
    #0062, #8211, #8212, #8216, #8217, #8220, #8221, #8226, #8224, #8225,
    #8242, #8243, #8249, #8250, #0732, #0710, #9824, #9827, #9829, #9830,
    #9674, #8592, #8594, #8593, #8595, #8596, #0172, #0188, #0189, #0190,
    #0177, #0171, #0187, #0176, #0171, #0186, #0161, #0191, #8364, #0162,
    #0163, #0165, #0164, #0167, #0182, #0175, #0183, #0181, #0215, #0247);

{ Helper Functions }

function EncodeWebChar(C: WideChar; out Code: WideString): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := Low(SpecialChars) to High(SpecialChars) do
    if SpecialChars[I] = C then
    begin
      Code := CodedChars[I];
      Result := True;
      Exit;
    end;
end;

function DecodeWebChar(const Code: WideString; out C: WideChar): Boolean;
var
  I: Integer;
begin
  Result := False;
  if Code <> '' then
  begin
    if Code[1] = '#' then
    begin
      C := #0;
      for I := 2 to Length(Code) do
        if (Code[I] >= '0') and (Code[I] <= '9') then
          C := WideChar(Ord(C) * 10 + Ord(Code[I]) - Ord('0'))
        else
          Exit;
      Result := (C <> #0);
    end
    else
      for I := Low(CodedChars) to High(CodedChars) do
        if CodedChars[I] = Code then
        begin
          C := SpecialChars[I];
          Result := True;
          Exit;
        end;
  end;
end;

function DecodeHtmlEntities(const Str: WideString): WideString;
var
  P, S, E: PWideChar;
  Code: WideString;
begin
  SetString(Result, nil, Length(Str));
  S := PWideChar(Str);
  P := PWideChar(Result);
  while S^ <> #0 do
  begin
    P^ := S^;
    if S^ = '&' then
    begin
      E := S + 1;
      while (E^ <> #0) and (E^ <> ';') do
        Inc(E);
      if E^ = ';' then
      begin
        SetString(Code, S + 1, E - S - 1);
        if DecodeWebChar(Code, P^) then
          S := E;
      end;
    end;
    Inc(P);
    Inc(S);
  end;
  SetLength(Result, P - PWideChar(Result));
end;

function EncodeHtmlEntities(const Str: WideString): WideString;
var
  Code: WideString;
  P, S: PWideChar;
  I: Integer;
begin
  SetString(Result, nil, 8 * Length(Str));
  S := PWideChar(Str);
  P := PWideChar(Result);
  while S^ <> #0 do
  begin
    P^ := S^;
    if EncodeWebChar(S^, Code) then
    begin
      P^ := '&';
      Inc(P);
      for I := 1 to Length(Code) do
      begin
        P^ := Code[I];
        Inc(P);
      end;
      P^ := ';';
    end;
    Inc(P);
    Inc(S);
  end;
  SetLength(Result, P - PWideChar(Result));
end;

function DecodeWebColor(const WebColor: String; DefColor: TColor): TColor;
begin
  if Pos('#', WebColor) = 1 then
  begin
    if Length(WebColor) = 7 then
      try
        Result := RGB(
          StrToInt('$' + Copy(WebColor, 2, 2)),
          StrToInt('$' + Copy(WebColor, 4, 2)),
          StrToInt('$' + Copy(WebColor, 6, 2)));
      except
        Result := DefColor;
      end
    else if Length(WebColor) = 4 then
      try
        Result := RGB(
          (StrToInt('$' + Copy(WebColor, 2, 1)) shl 4) or StrToInt('$' + Copy(WebColor, 2, 1)),
          (StrToInt('$' + Copy(WebColor, 3, 1)) shl 4) or StrToInt('$' + Copy(WebColor, 3, 1)),
          (StrToInt('$' + Copy(WebColor, 4, 1)) shl 4) or StrToInt('$' + Copy(WebColor, 4, 1)));
      except
        Result := DefColor;
      end
    else
      Result := DefColor;
  end
  else if Pos('$', WebColor) = 1 then
    Result := StrToIntDef(WebColor, DefColor)
  else if not IdentToColor('cl' + WebColor, Integer(Result)) then
    Result := DefColor;
end;

function EncodeWebColor(Color: TColor): String;
var
  RGB: Integer;
begin
  if ColorToIdent(Color, Result) then
    Delete(Result, 1, 2)
  else
  begin
    RGB := ColorToRGB(Color);
    Result := '#'
            + IntToHex(GetRValue(RGB), 2)
            + IntToHex(GetGValue(RGB), 2)
            + IntToHex(GetBValue(RGB), 2);
  end;
end;

procedure DrawHTML(Canvas: TCanvas; const Rect: TRect; const Text: WideString);
var
  SaveRgn: HRGN;
begin
  SaveRgn := CreateRectRgn(0, 0, 0, 0);
  GetClipRgn(Canvas.Handle, SaveRgn);
  try
    IntersectClipRect(Canvas.Handle, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom);
    with TSimpleHTML.Create(Text) do
      try
        Draw(Canvas, clRed, Rect);
      finally
        Free;
      end;
  finally
    SelectClipRgn(Canvas.Handle, SaveRgn);
    DeleteObject(SaveRgn);
  end;
end;

{ TImageCache }

constructor TImageCache.Create;
begin
  fOwnsImages := True;
end;

destructor TImageCache.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TImageCache.HashOf(const URI: WideString): DWORD;
var
  I: Integer;
begin
  Result := 0;
  for I := 1 to Length(URI) do
    Result := ((Result shl 2) or (Result shr (SizeOf(Result) * 8 - 2))) xor Ord(URI[I]);
end;

function TImageCache.Find(const URI: WideString): PPImageCacheItem;
var
  B: Integer;
begin
  B := HashOf(URI) mod IMAGE_CACHE_BUCKET_SIZE;
  Result := @Buckets[B];
  while Result^ <> nil do
  begin
    if Result^.URI = URI then
      Exit
    else
      Result := @Result^.Next;
  end;
end;

procedure TImageCache.Clear;
var
  I: Integer;
  C, N: PImageCacheItem;
begin
  for I := Low(Buckets) to High(Buckets) do
  begin
    C := Buckets[I];
    while C <> nil do
    begin
      N := C^.Next;
      if OwnsImages and (C^.Image <> nil) then
        C^.Image.Free;
      Dispose(C);
      C := N;
    end;
    Buckets[I] := nil;
  end;
end;

procedure TImageCache.Add(const URI: WideString; Image: TPicture);
var
  B: Integer;
  Bucket: PImageCacheItem;
begin
  B := HashOf(URI) mod IMAGE_CACHE_BUCKET_SIZE;
  New(Bucket);
  Bucket^.URI := URI;
  Bucket^.Image := Image;
  Bucket^.Next := Buckets[B];
  Buckets[B] := Bucket;
end;

function TImageCache.Modify(const URI: WideString; Image: TPicture): Boolean;
var
  P: PImageCacheItem;
begin
  P := Find(URI)^;
  if P <> nil then
  begin
    if OwnsImages and (P^.Image <> Image) then
      P^.Image.Free;
    P^.Image := Image;
    Result := True;
  end
  else
    Result := False;
end;

function TImageCache.Delete(const URI: WideString): Boolean;
var
  P: PImageCacheItem;
  Prev: PPImageCacheItem;
begin
  Prev := Find(URI);
  P := Prev^;
  if P <> nil then
  begin
    Prev^ := P^.Next;
    if OwnsImages and (P^.Image <> nil) then
      P^.Image.Free;
    Dispose(P);
    Result := True;
  end
  else
    Result := False;
end;

function TImageCache.ImageOf(const URI: WideString): TPicture;
var
  P: PImageCacheItem;
begin
  P := Find(URI)^;
  if P <> nil then
    Result := P^.Image
  else
    Result := nil;
end;

function TImageCache.ForEach(Callback: TImageCacheCallback;
  UserData: Integer): Boolean;
var
  I: Integer;
  P: PImageCacheItem;
begin
  Result := True;
  if Assigned(Callback) then
  begin
    for I := 0 to Length(Buckets) - 1 do
    begin
      P := Buckets[I];
      while P <> nil do
      begin
        Callback(Self, P^.URI, P^.Image, UserData, Result);
        if not Result then Exit;
        P := P^.Next;
      end;
    end;
  end;
end;

{ TCanvasRecall }

constructor TCanvasRecall.Create(AReference: TCanvas);
begin
  fReference := AReference;
  fFont := TFont.Create;
  fPen := TPen.Create;
  fBrush := TBrush.Create;
  Store;
end;

destructor TCanvasRecall.Destroy;
begin
  Retrieve;
  fBrush.Free;
  fPen.Free;
  fFont.Free;
  inherited Destroy;
end;

procedure TCanvasRecall.Store;
begin
  if Assigned(fReference) then
  begin
    fFont.Assign(fReference.Font);
    fPen.Assign(fReference.Pen);
    fBrush.Assign(fReference.Brush);
    fCopyMode := fReference.CopyMode;
    fTextFlags := fReference.TextFlags;
  end;
end;

procedure TCanvasRecall.Retrieve;
begin
  if Assigned(fReference) then
  begin
    fReference.Font.Assign(fFont);
    fReference.Pen.Assign(fPen);
    fReference.Brush.Assign(fBrush);
    fReference.CopyMode := fCopyMode;
    fReference.TextFlags := fTextFlags;
  end;
end;

procedure TCanvasRecall.SetReference(Value: TCanvas);
begin
  if fReference <> Value then
  begin
    Retrieve;
    fReference := Value;
    Store;
  end;
end;

{ TSimpleLinks }

constructor TSimpleLinks.Create;
begin
  fItems := TStringList.Create;
end;

destructor TSimpleLinks.Destroy;
begin
  Clear;
  fItems.Free;
  inherited Destroy;
end;

function TSimpleLinks.GetCount: Integer;
begin
  Result := fItems.Count;
end;

function TSimpleLinks.GetBounds(Index: Integer): TRect;
begin
  GetRgnBox(Regions[Index], Result);
end;

function TSimpleLinks.GetRegions(Index: Integer): HRGN;
begin
  Result := HRGN(fItems.Objects[Index]);
end;

function TSimpleLinks.GetLinks(Index: Integer): WideString;
begin
  Result := fItems.Strings[Index];
end;

procedure TSimpleLinks.Clear;
var
  I: Integer;
  Rgn: HRGN;
begin
  for I := 0 to fItems.Count - 1 do
  begin
    Rgn := HRGN(fItems.Objects[I]);
    DeleteObject(Rgn);
  end;
  fItems.Clear;
end;

procedure TSimpleLinks.Add(const Link: WideString; const Rect: TRect);
begin
  fItems.AddObject(Link, TObject(CreateRectRgnIndirect(Rect)));
end;

procedure TSimpleLinks.Add(const Link: WideString; Rgn: HRGN);
begin
  fItems.AddObject(Link, TObject(Rgn));
end;

function TSimpleLinks.IndexOfLinkAt(X, Y: Integer): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to Count - 1 do
    if PtInRegion(Regions[I], X, Y) then
    begin
      Result := I;
      Exit;
    end;
end;

{ TSimpleHTMLNode }

const
  LAYOUT_SUPERSCRIPT = Integer($80000000);
  LAYOUT_SUBSCRIPT   = Integer($40000000);

constructor TSimpleHTMLNode.Create(AParent: TSimpleHTMLNode; ATagName: WideString);
begin
  fParent := AParent;
  fTagName := ATagName;
end;

destructor TSimpleHTMLNode.Destroy;
var
  I: Integer;
begin
  if Assigned(fChildren) then
  begin
    for I := 0 to fChildren.Count - 1 do
      TSimpleHTMLNode(fChildren[I]).Free;
    fChildren.Free;
  end;
  if Assigned(fAttributes) then
    fAttributes.Free;
  inherited Destroy;
end;

function TSimpleHTMLNode.GetDocument: TSimpleHTML;
begin
  Result := Parent.GetDocument;
end;

function TSimpleHTMLNode.GetAttributes(const Name: WideString): WideString;
begin
  if Assigned(fAttributes) then
    Result := fAttributes.Values[Name]
  else
    Result := '';
end;

function TSimpleHTMLNode.GetChildCount: Integer;
begin
  if Assigned(fChildren) then
    Result := fChildren.Count
  else
    Result := 0;
end;

function TSimpleHTMLNode.GetChildren(Index: Integer): TSimpleHTMLNode;
begin
  Result := TSimpleHTMLNode(fChildren[Index]);
end;

function TSimpleHTMLNode.GetIndex: Integer;
begin
  if Assigned(Parent) then
    Result := Parent.fChildren.IndexOf(Self)
  else
    Result := 0;
end;

function TSimpleHTMLNode.GetInnerText: WideString;
var
  I: Integer;
begin
  if HasChildren then
  begin
    Result := '';
    for I := 0 to ChildCount - 1 do
    begin
      if Result <> '' then
        Result := Result + ' ';
      Result := Result + Children[I].InnerText;
    end;
  end
  else
    Result := Text;
end;

function TSimpleHTMLNode.GetInnerHTML: WideString;
var
  I: Integer;
begin
  if HasChildren then
  begin
    Result := '';
    for I := 0 to ChildCount - 1 do
      Result := Result + Children[I].OuterHTML;
  end
  else
    Result := EncodeHtmlEntities(Text);
end;

function TSimpleHTMLNode.GetOuterHTML: WideString;
var
  I: Integer;
  AttrName, AttrValue: WideString;
begin
  Result := InnerHTML;
  if TagName <> '' then
  begin
    if Result <> '' then
      Result := '>' + Result + '</' + TagName + '>'
    else
      Result := '/>';
    if Assigned(fAttributes) then
      for I := fAttributes.Count - 1 downto 0 do
      begin
        AttrName := fAttributes.Names[I];
        AttrValue := fAttributes.Values[AttrName];
        Result := ' ' + AttrName + '="' + EncodeHtmlEntities(AttrValue) + '"' + Result;
      end;
    Result := '<' + TagName + Result;
  end;
end;

function TSimpleHTMLNode.AddChild(const ATagName: WideString): TSimpleHTMLNode;
begin
  Result := TSimpleHTMLNode.Create(Self, ATagName);
  if not Assigned(fChildren) then
    fChildren := TList.Create;
  fChildren.Add(Result);
end;

procedure TSimpleHTMLNode.RemoveChild(Index: Integer);
var
  Child: TSimpleHTMLNode;
begin
  Child := TSimpleHTMLNode(fChildren[Index]);
  fChildren.Delete(Index);
  Child.Free;
end;

procedure TSimpleHTMLNode.RemoveAllChildren;
var
  I: Integer;
begin
  for I := 0 to fChildren.Count - 1 do
    TSimpleHTMLNode(fChildren[I]).Free;
  fChildren.Clear;
end;

function TSimpleHTMLNode.HasChildren: Boolean;
begin
  Result := Assigned(fChildren) and (fChildren.Count <> 0);
end;

function TSimpleHTMLNode.HasAttribute(const Name: WideString): Boolean;
begin
  Result := Assigned(fAttributes) and (fAttributes.IndexOfName(Name) >= 0);
end;

procedure TSimpleHTMLNode.Prepare(Canvas: TCanvas; LinkFontColor: TColor);
var
  Value: String;
begin
  if HasAttribute('DIR') then
  begin
    Value := Attributes['DIR'];
    if CompareText(Value, 'LTR') = 0 then
      Canvas.TextFlags := Canvas.TextFlags or ETO_RTLREADING
    else if CompareText(Value, 'RTL') = 0 then
      Canvas.TextFlags := Canvas.TextFlags and not ETO_RTLREADING
  end;
  if TagName = 'A' then
    Canvas.Font.Color := LinkFontColor
  else if TagName = 'FONT' then
  begin
    if HasAttribute('FACE') then
      Canvas.Font.Name := Attributes['FACE'];
    if HasAttribute('SIZE') then
    begin
      Value := Attributes['SIZE'];
      if (Pos('+', Value) = 1) or (Pos('-', Value) = 1) then
        Canvas.Font.Size := Canvas.Font.Size + StrToIntDef(Value, 0)
      else
        Canvas.Font.Size := StrToIntDef(Value, Canvas.Font.Size);
    end;
    if HasAttribute('COLOR') then
      Canvas.Font.Color := DecodeWebColor(Attributes['COLOR'], Canvas.Font.Color);
    if HasAttribute('BGCOLOR') then
      Canvas.Brush.Color := DecodeWebColor(Attributes['BGCOLOR'], Canvas.Brush.Color);
  end
  else if (TagName = 'B') or (TagName = 'STRONG') then
    Canvas.Font.Style := Canvas.Font.Style + [fsBold]
  else if TagName = 'I' then
    Canvas.Font.Style := Canvas.Font.Style + [fsItalic]
  else if TagName = 'U' then
    Canvas.Font.Style := Canvas.Font.Style + [fsUnderline]
  else if (TagName = 'S') or (TagName = 'STRIKE') then
    Canvas.Font.Style := Canvas.Font.Style + [fsStrikeOut]
  else if TagName = 'SUP' then
  begin
    Canvas.Font.Size := MulDiv(Canvas.Font.Size, 70, 100);
    Canvas.TextFlags := Canvas.TextFlags or LAYOUT_SUPERSCRIPT;
  end
  else if TagName = 'SUB' then
  begin
    Canvas.Font.Size := MulDiv(Canvas.Font.Size, 70, 100);
    Canvas.TextFlags := Canvas.TextFlags or LAYOUT_SUBSCRIPT;
  end
  else if TagName = 'SMALL' then
    Canvas.Font.Size := MulDiv(Canvas.Font.Size, 75, 100)
  else if TagName = 'BIG' then
    Canvas.Font.Size := MulDiv(Canvas.Font.Size, 125, 100);
end;

function TSimpleHTMLNode.GetLinks(Canvas: TCanvas; var Rect: TRect;
  Links: TSimpleLinks): Integer;
var
  Recall: TCanvasRecall;
  Bounds: TRect;
  I: Integer;
begin
  Result := 0;
  Recall := TCanvasRecall.Create(Canvas);
  try
    Prepare(Canvas, Canvas.Font.Color);
    if (TagName = 'A') and HasAttribute('HREF') then
    begin
      Bounds := Rect;
      with Extend(Canvas) do
      begin
        Bounds.Right := Bounds.Left + cx;
        if LongBool(Canvas.TextFlags and LAYOUT_SUPERSCRIPT) then
          Bounds.Bottom := Bounds.Top + cy
        else if LongBool(Canvas.TextFlags and LAYOUT_SUBSCRIPT) then
          Bounds.Top := Bounds.Bottom - cy
        else
        begin
          Bounds.Top := ((Bounds.Bottom - Bounds.Top) - cy) div 2;
          Bounds.Bottom := Bounds.Top + cy;
        end;
        Inc(Rect.Left, cx);
      end;
      Links.Add(Attributes['HREF'], Bounds);
      Inc(Result);
    end
    else if HasChildren then
      for I := 0 to ChildCount - 1 do
        Inc(Result, Children[I].GetLinks(Canvas, Rect, Links))
    else if LongBool(Canvas.TextFlags and ETO_RTLREADING) then
      Dec(Rect.Right, Extend(Canvas).cx)
    else
      Inc(Rect.Left, Extend(Canvas).cx);
  finally
    Recall.Free;
  end;
end;

function TSimpleHTMLNode.Extend(Canvas: TCanvas): TSize;
var
  Recall: TCanvasRecall;
  Image: TPicture;
  I: Integer;
begin
  Recall := TCanvasRecall.Create(Canvas);
  try
    Prepare(Canvas, Canvas.Font.Color);
    if HasChildren then
    begin
      Result.cx := 0;
      Result.cy := 0;
      for I := 0 to ChildCount - 1 do
        with Children[I].Extend(Canvas) do
        begin
          Inc(Result.cx, cx);
          if Result.cy < cy then
            Result.cy := cy;
        end;
    end
    else if TagName = 'IMG' then
    begin
      Image := Document.GetImage(Attributes['SRC']);
      if HasAttribute('WIDTH') then
        Result.cx := StrToIntDef(Attributes['WIDTH'], 0)
      else
        Result.cx := Image.Width;
      if HasAttribute('HEIGHT') then
        Result.cy := StrToIntDef(Attributes['HEIGHT'], 0)
      else
        Result.cy := Image.Height;
      if HasAttribute('HSPACE') then
        Inc(Result.cx, 2 * StrToIntDef(Attributes['HSPACE'], 0));
      if HasAttribute('VSPACE') then
        Inc(Result.cy, 2 * StrToIntDef(Attributes['VSPACE'], 0));
    end
    else if Text <> '' then
      GetTextExtentPoint32W(Canvas.Handle, PWideChar(Text), Length(Text), Result)
    else
    begin
      Result.cx := 0;
      Result.cy := 0;
    end;
  finally
    Recall.Free;
  end;
end;

procedure TSimpleHTMLNode.Draw(Canvas: TCanvas; LinkFontColor: TColor; Rect: TRect);
var
  Recall: TCanvasRecall;
  Image: TPicture;
  ImageRect: TRect;
  I, W, H, SX, SY, Flags: Integer;
begin
  Recall := TCanvasRecall.Create(Canvas);
  try
    Prepare(Canvas, LinkFontColor);
    if HasChildren then
    begin
      for I := 0 to ChildCount - 1 do
      begin
        Children[I].Draw(Canvas, LinkFontColor, Rect);
        if LongBool(Canvas.TextFlags and ETO_RTLREADING) then
          Dec(Rect.Right, Children[I].Extend(Canvas).cx)
        else
          Inc(Rect.Left, Children[I].Extend(Canvas).cx);
      end;
    end
    else if TagName = 'IMG' then
    begin
      Image := Document.GetImage(Attributes['SRC']);
      if HasAttribute('WIDTH') then
        W := StrToIntDef(Attributes['WIDTH'], 0)
      else
        W := Image.Width;
      if HasAttribute('HEIGHT') then
        H := StrToIntDef(Attributes['HEIGHT'], 0)
      else
        H := Image.Height;
      if HasAttribute('HSPACE') then
        SX := StrToIntDef(Attributes['HSPACE'], 0)
      else
        SX := 0;
      if HasAttribute('VSPACE') then
        SY := StrToIntDef(Attributes['VSPACE'], 0)
      else
        SY := 0;
      if LongBool(Canvas.TextFlags and ETO_RTLREADING) then
      begin
        ImageRect.Left := Rect.Right - W - SX;
        Dec(Rect.Right, W + 2 * SX);
      end
      else
      begin
        ImageRect.Left := Rect.Left + SX;
        Inc(Rect.Left, W + 2 * SX);
      end;
      if LongBool(Canvas.TextFlags and LAYOUT_SUPERSCRIPT) then
        ImageRect.Top := Rect.Top + SY
      else if LongBool(Canvas.TextFlags and LAYOUT_SUBSCRIPT) then
        ImageRect.Top := Rect.Bottom - H - SY
      else
        ImageRect.Top := (Rect.Top + Rect.Bottom - H) div 2;
      ImageRect.Right := ImageRect.Left + W;
      ImageRect.Bottom := ImageRect.Top + H;
      if Assigned(Image.Graphic) then
        Canvas.StretchDraw(ImageRect, Image.Graphic);
    end
    else if Text <> '' then
    begin
      Flags := DT_NOPREFIX or DT_SINGLELINE or DT_NOCLIP;
      if LongBool(Canvas.TextFlags and LAYOUT_SUPERSCRIPT) then
        Flags := Flags or DT_TOP
      else if LongBool(Canvas.TextFlags and LAYOUT_SUBSCRIPT) then
        Flags := Flags or DT_BOTTOM
      else
        Flags := Flags or DT_VCENTER;
      if LongBool(Canvas.TextFlags and ETO_RTLREADING) then
        Flags := Flags or DT_RTLREADING or DT_RIGHT
      else
        Flags := Flags or DT_LEFT;
      DrawTextW(Canvas.Handle, PWideChar(Text), Length(Text), Rect, Flags);
    end
  finally
    Recall.Free;
  end;
end;

function TSimpleHTML.CollectLinks(Canvas: TCanvas; const Rect: TRect;
  Links: TSimpleLinks): Integer;
var
  R: TRect;
begin
  R := Rect;
  Result := GetLinks(Canvas, R, Links);
end;

function TSimpleHTMLNode.Parse(const HTML: WideString): Boolean;
var
  Tag, AttrName, AttrValue: WideString;
  Code, TextSoFar, TextChunk: WideString;
  Current: TSimpleHTMLNode;
  IsEndTag: Boolean;
  P, S, E: PWideChar;
  C: WideChar;
begin
  Result := False;
  Current := Self;
  S := PWideChar(HTML);
  P := S;
  TextSoFar := '';
  while P^ <> #0 do
  begin
    if P^ = '<' then
    begin
      Inc(P);
      // any text left?
      if P <> S + 1 then
      begin
        if TextSoFar = '' then
          SetString(TextSoFar, S, P - S - 1)
        else
        begin
          SetString(TextChunk, S, P - S - 1);
          TextSoFar := TextSoFar + TextChunk;
        end;
      end;
      // is there a text element?
      if TextSoFar <> '' then
      begin
        if Current.HasChildren or (P^ <> '/') then
          Current.AddChild('').fText := TextSoFar
        else
          Current.fText := TextSoFar;
        TextSoFar := '';
      end;
      // is it the end tag of a container element?
      IsEndTag := (P^ = '/');
      if IsEndTag then
        Inc(P);
      // is it a comment?
      if (p^ = '!') and ((P + 1)^ = '-') and ((P + 2)^ = '-') then
      begin
        Inc(P, 3);
        S := P;
        while P^ <> #0 do
        begin
          if (P^ = '>') and (P - S >= 2) and ((P - 1)^ = '-') and ((P - 2)^ = '-') then
            Break;
          Inc(P);
        end;
        if P^ = #0 then
          Exit;
      end
      else
      begin
        // get the tag name
        S := P;
        while IsCharAlphaNumericW(P^) do
          Inc(P);
        if S = P then
          Exit;
        SetString(Tag, S, P - S);
        Tag := UpperCase(Tag);
        // skip blanks
        while (P^ = ' ') or (P^ = #13) or (P^ = #10) or (P^ = #9) do
          Inc(P);
        // was this an end tag of a contanier element?
        if IsEndTag then
        begin
          if (P^ <> '>') or (Current.TagName <> Tag) or (Current.Parent = nil) then
            Exit;
          // go up one level
          Current := Current.Parent;
        end
        else
        begin
          // this is an open tag, get the attributes
          while (P^ <> #0) and (P^ <> '>') and (P^ <> '/') do
          begin
            // get the attribute's name
            S := P;
            while IsCharAlphaNumericW(P^) do
              Inc(P);
            if S = P then
              Exit;
            SetString(AttrName, S, P - S);
            // skip blanks
            while (P^ = ' ') or (P^ = #13) or (P^ = #10) or (P^ = #9) do
              Inc(P);
            // suppose the attribute doesn't have a value
            AttrValue := '';
            // has it a value?
            if P^ = '=' then
            begin
              // skip blanks
              Inc(P);
              while P^ = ' ' do
                Inc(P);
              // get the attribute's value
              if (P^ = #0) or (P^ = '>') then
                Exit;
              S := P;
              if (S^ <> '''') and (S^ <> '"') then
              begin
                // value is not quoted
                repeat
                  Inc(P);
                until (P^ = #0) or (P^ = ' ') or (P^ = '>');
                if P^ = #0 then
                  Exit;
                SetString(AttrValue, S, P - S);
              end
              else
              begin
                // value is quoted
                repeat
                  Inc(P);
                until (P^ = #0) or (P^ = S^);
                if P^ = #0 then
                  Exit;
                Inc(S);
                SetString(AttrValue, S, P - S);
                Inc(P);
              end;
              // skip blanks
              while (P^ = ' ') or (P^ = #13) or (P^ = #10) or (P^ = #9) do
                Inc(P);
            end;
            // add the attribute to the list
            if not Assigned(fAttributes) then
              fAttributes := TStringList.Create;
            fAttributes.Values[AttrName] := DecodeHtmlEntities(AttrValue);
          end;
          // is this a non-container element?
          if P^ = '/' then
          begin
            Inc(P);
            if P^ <> '>' then
               Exit;
            // add the element
            Current.AddChild(Tag).fAttributes := fAttributes;
          end
          else if P^ = '>' then
          begin
            // add the element, and go one level down
            Current := Current.AddChild(Tag);
            Current.fAttributes := fAttributes;
          end
          else
            Exit;
          fAttributes := nil;
        end;
      end;
      S := P + 1;
    end
    else if P^ = '&' then
    begin
      // find the character code
      E := P + 1;
      while (E^ <> #0) and (E^ <> ';') do
        Inc(E);
      // if the code is valid
      if E^ <> #0 then
      begin
        // extract the code
        SetString(Code, P + 1, E - P - 1);
        // decode character
        if DecodeWebChar(Code, C) then
        begin
          // merge the text parts so far
          if P <> S then
          begin
            SetString(TextChunk, S, P - S);
            TextSoFar := TextSoFar + TextChunk + C;
          end
          else
            TextSoFar := TextSoFar + C;
          P := E;
          S := P + 1;
        end;
      end;
    end;
    Inc(P);
  end;
  if Current <> Self then
    Exit;
  // any text left?
  if P <> S then
  begin
    if TextSoFar = '' then
      SetString(TextSoFar, S, P - S)
    else
    begin
      SetString(TextChunk, S, P - S);
      TextSoFar := TextSoFar + TextChunk;
    end;
  end;
  // is there a text element?
  if TextSoFar <> '' then
  begin
    if HasChildren then
      AddChild('').fText := TextSoFar
    else
      fText := TextSoFar;
  end;
  Result := True;
end;

{ TSimpleHTML }

constructor TSimpleHTML.Create(const HTML: WideString);
begin
  inherited Create(nil, 'HTML');
  fParsed := Parse(HTML);
end;

destructor TSimpleHTML.Destroy;
begin
  if Assigned(fImages) then
    fImages.Free;
  inherited Destroy;
end;

function TSimpleHTML.GetDocument: TSimpleHTML;
begin
  Result := Self;
end;

function TSimpleHTML.GetImage(const URI: WideString): TPicture;
begin
  if Assigned(OnAdvanceGetImage) then
    Result := OnAdvanceGetImage(Self, URI)
  else
  begin
    if not Assigned(fImages) then
      fImages := TImageCache.Create;
    Result := fImages.ImageOf(WideLowerCase(URI));
    if not Assigned(Result) then
    begin
      Result := TPicture.Create;
      fImages.Add(WideLowerCase(URI), Result);
      if Assigned(OnGetImage) then
        OnGetImage(Self, URI, Result)
      else if FileExists(URI) then
        try
          Result.LoadFromFile(URI);
        except
          // ignore exceptions
        end;
    end;
  end;
end;

end.
