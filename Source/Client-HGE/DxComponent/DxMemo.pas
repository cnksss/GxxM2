unit DxMemo;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  StdCtrls,
  Graphics,
  Forms,
  HGE,
  HGEFontEx,
  DxControls,
  DxComponents,
  DxLabel,
  GameImages,
  HUtil32;

type
  pTViewItem = ^TViewItem;
  TViewItem = record
    Caption:string;
    Data:Pointer;
    Style:TButtonStyle;
    Checked:Boolean;
    Color:TDxCaptionColor;
    ImageIndex:TDxImageIndex;
    Transparent:Boolean;
    CaptionTexture:TImageInfo;
    TimeTick:LongWord;
    Alignment:TAlignment;
    ParentViewItem:pTViewItem;
    Items:TList;
    StringLineEx:TObject;
  end;

  TDxListView = class;
  TDxListItem = class;
  TDxTreeView = class;
  TDxTreeNode = class;
  TDxChatMemo = class;
  TDxScrollControl = class(TDxControl)
  private
    FShowScroll:Boolean;
    FScrollBars:TScrollStyle;
    FScrollSize:Integer;

    FItemHeight:Integer;
    FItemIndex:Integer;
    FHotItemIndex:Integer;

    FPosition:Integer;

    FVisibleItemCount:Integer;

    FScrollImageIndex:TDxImageIndex;

    FPrevImageIndex:TDxImageIndex;
    FNextImageIndex:TDxImageIndex;
    FBarImageIndex:TDxImageIndex;

    FPrevImageSize:Integer;
    FNextImageSize:Integer;
    FBarImageSize:Integer;

    FPrevMouseDown:Boolean;
    FNextMouseDown:Boolean;
    FBarMouseDown:Boolean;

    FPrevMouseMove:Boolean;
    FNextMouseMove:Boolean;
    FBarMouseMove:Boolean;

    FScrollMouseDown:Boolean;

    FOnScroll:TNotifyEvent;

    FBarTop:Integer;

    FOffSetX, FOffSetY:Integer;
    FShowItemCount:Integer;
    FMouseMoveTick:LongWord;

    FMaxRect:TRect;
    FExpandSize:Integer;
    FCanMouseWheel:Boolean;

    FAutoShowScroll:Boolean;

    FMouseHorizontal:Boolean; //True Left False Top

    FMouseScroll:Boolean; //所有区域都能拖拽移动
    FMouseX:Integer;
    FMouseY:Integer;
    FMouseScrollDown:Boolean;

    {$MESSAGE HINT 'FMouseSpring 变量没用到，但是不能删，可能会影响插件API的内存结构'}
    FMouseSpring:Boolean; //
    FSpringStep:Word;
    FDX:Integer; //按下坐标
    FDY:Integer;
    FfSpeedX:Double;
    FfSpeedY:Double;
    FDTime:Cardinal;
    FMTime:Cardinal;
    FStartSpring:Boolean;
    function InPrevRange(X, Y:Integer; vRect:TRect):Boolean;
    function InNextRange(X, Y:Integer; vRect:TRect):Boolean;
    function InBarRange(X, Y:Integer; vRect:TRect):Boolean;
    function GetVisibleHeight:Integer;

    procedure ScrollImageIndexChange(Sender:TObject); stdcall;
    procedure PrevImageIndexChange(Sender:TObject); stdcall;
    procedure NextImageIndexChange(Sender:TObject); stdcall;
    procedure BarImageIndexChange(Sender:TObject); stdcall;

    procedure SetPosition(Value:Integer);
    procedure SetVisibleItemCount(Value:Integer);
    procedure SetExpandSize(Value:Integer);
    procedure SetItemIndex(Value:Integer);
    procedure SetItemHeight(Value:Integer);

    procedure SetScrollBars(Value:TScrollStyle);
    procedure SetScrollSize(Value:Integer);
    procedure SetShowItemCount(Value:Integer);

    procedure SetAutoShowScroll(Value:Boolean);
  protected
    function CanMove:Boolean; override;
    procedure SetOnGetImage(Value:TOnGetImage); override;
    procedure ChangeShowItemCount(); virtual;
    procedure DoResize(var NewRect:TRect); override;
    procedure DoClick(X, Y:Integer); override;
    procedure DoMouseUp(); override;
    procedure DoMouseEnter; override;
    procedure DoMouseLeave; override;
    function ScrollMouseDown(Button:TMouseButton; X, Y:Integer):Boolean;
    function ScrollMouseMove(Shift:TShiftState; X, Y:Integer):Boolean;
    function ScrollMouseUp():Boolean;

    procedure AutoSetShowScroll; virtual;

  public
    procedure MouseWheelDown(Shift:TShiftState; MousePos:TPoint); override;
    procedure MouseWheelUp(Shift:TShiftState; MousePos:TPoint); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Next;
    procedure Previous;
    procedure First;
    procedure Last;
    procedure DoScroll(Value:Integer); virtual;
    function MinValue:Integer; virtual;
    function MaxValue:Integer; virtual;
    property MaxRect:TRect read FMaxRect write FMaxRect;
    property OnScroll:TNotifyEvent read FOnScroll write FOnScroll;
    property VisibleHeight:Integer read GetVisibleHeight;
    property HotItemIndex:Integer read FHotItemIndex write FHotItemIndex;

    procedure AutoCalcShowItemCount;
  published
    property Align;
    property BackgroundColor;

    property CanMouseWheel:Boolean read FCanMouseWheel write FCanMouseWheel;
    property ShowScroll:Boolean read FShowScroll write FShowScroll;
    property ItemHeight:Integer read FItemHeight write SetItemHeight;
    property ItemIndex:Integer read FItemIndex write SetItemIndex;
    property ScrollBars:TScrollStyle read FScrollBars write SetScrollBars;
    property ScrollSize:Integer read FScrollSize write SetScrollSize;
    property ScrollImageIndex:TDxImageIndex read FScrollImageIndex write FScrollImageIndex;
    property PrevImageIndex:TDxImageIndex read FPrevImageIndex write FPrevImageIndex;
    property NextImageIndex:TDxImageIndex read FNextImageIndex write FNextImageIndex;
    property BarImageIndex:TDxImageIndex read FBarImageIndex write FBarImageIndex;

    property Position:Integer read FPosition write SetPosition;
    property VisibleItemCount:Integer read FVisibleItemCount write SetVisibleItemCount;
    property ExpandSize:Integer read FExpandSize write SetExpandSize;

    property OffSetX:Integer read FOffSetX write FOffSetX;
    property OffSetY:Integer read FOffSetY write FOffSetY;
    property ShowItemCount:Integer read FShowItemCount write SetShowItemCount; // 显示列表行数   TDxListView有效

    property AutoShowScroll:Boolean read FAutoShowScroll write SetAutoShowScroll;
    property MouseHorizontal:Boolean read FMouseHorizontal write FMouseHorizontal;
    property MouseScroll:Boolean read FMouseScroll write FMouseScroll;
    property MouseScrollDown:Boolean read FMouseScrollDown write FMouseScrollDown;

  end;

  TDxScrollBox = class(TDxScrollControl)
  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    procedure Paint; override;
    procedure Update; override;
  end;

  TLineColor = record
    FC, BC:TColor;
  end;
  pTLineColor = ^TLineColor;

  TDxLines = class(TStringList)
  private
    FItemList:array of Pointer;
    FItemHeightList:array of Integer;
    function GetItem(Index:Integer):pTViewItem;
    function GetItemHeight(Index:Integer):Integer;
    procedure SetItemHeight(Index:Integer; Value:Integer);
  protected

  public
    Owner:TDxChatMemo;
    constructor Create;
    destructor Destroy; override;

    function AddObject(const S:string; AObject:TObject):Integer; override;
    procedure Clear; override;
    procedure Delete(Index:Integer); override;
    procedure InsertObject(Index:Integer; const S:string;
      AObject:TObject); override;

    function AddItem(const S:string):pTViewItem;

    property Items[Index:Integer]:pTViewItem read GetItem;
    property ItemHeights[Index:Integer]:Integer read GetItemHeight write SetItemHeight;
  published

  end;

  TTokenType = (tt_Text, tt_Item);

  {---------------- 一行分为多个字符 chongchong 2013-08-26---------------}
  PStringToken = ^TStringToken;
  TStringToken = record
    FColor:TColor;
    BColor:TColor;
    Flag:Integer;
    Text:WideString;
    TokenType:TTokenType;
  end;

  TStringLineEx = class(TObject)
  private
    FTokens:TList;
    FLineBackColor:TColor;
    function GetCount:Integer;
    function GetTokens(Index:Integer):PStringToken;
  public
    constructor Create;
    destructor Destroy; override;
    property Count:Integer read GetCount;
    property Tokens[Index:Integer]:PStringToken read GetTokens; default;
    function Add(FColor, BColor:TColor; Flag:Integer; Text:string; TokenType:TTokenType):PStringToken;
    procedure Delete(Index:Integer);
    property LineBackColor:TColor read FLineBackColor;
    procedure Clear;
  end;
  {----------------------------------------------------------------------}

  TDxChatMemo = class(TDxScrollControl) // 聊天框专用
  private
    FOnChange:TNotifyEvent;
    FLines:TStrings;
    FTopLines:TStrings;

    FDrawLines:TStrings;
    FDrawTopLines:TStrings;
    FAutoScroll:Boolean;
    FOnGetItem:TOnGetItem;
    FTopIndex:Integer;

    FFontBackTransparent:Boolean;
    FFontName:string;
    FFontSize:Integer;
    FFontStroke:Boolean;

    FDrawSelect:Boolean;
    FDrawBorder:Boolean;

    FDownViewItem:pTViewItem;
    FMoveViewItem:pTViewItem;
    //FTextList: TStringList;
    FCriticalSection:TRTLCriticalSection;

    //FSelectIndex: Integer;

    FDeleteCount:Integer;
    FDeleteTopCount:Integer;

    FBagItemForeColor:TColor;
    FBagItemBackColor:TColor;

    FOnItemClick:TItemClickEvent;
    FOnItemMouseMove:TItemClickEvent;
    FOnItemMouseLeave:TNotifyEvent;

    procedure SetLines(Value:TStrings);
    procedure SetTopLines(Value:TStrings);
    //procedure GetTextList(const Text: string);
    procedure RefDrawList;

    procedure SetFontName(Value:string);
    procedure SetFontSize(Value:Integer);
    procedure SetFontStroke(Value:Boolean);
  protected

    procedure DoResize(var NewRect:TRect); override;

    procedure DoItemClick(ItemName:string; MakeIndex:Integer; R:TRect); virtual;
    procedure DoItemMouseMove(ItemName:string; MakeIndex:Integer; R:TRect); virtual;
    procedure DoItemMouseLeave; virtual;
    procedure AutoSetShowScroll; override;

  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure DoScroll(Value:Integer); override;
    procedure KeyDown(var Key:Word; Shift:TShiftState); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;

  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;

    function MaxValue:Integer; override;
    procedure Initialize; override;
    procedure Finalize; override;
    procedure Paint; override;

    procedure LoadFromFile(const FileName:string);
    procedure Add(const S:string; FC, BC:TColor);
    procedure Insert(Index:Integer; const S:string; FC, BC:TColor);
    procedure Delete(Index:Integer);

    procedure AddTop(const S:string; FC, BC:TColor; TimeOut:Integer);
    procedure InsertTop(Index:Integer; const S:string; FC, BC:TColor; TimeOut:Integer);
    procedure DeleteTop(Index:Integer);

    procedure RemoveDelete;
    procedure RemoveDeleteTop;

    procedure Clear;
    procedure Lock;
    procedure UnLock;
    property TopIndex:Integer read FTopIndex write FTopIndex;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;

    property OnGetItem:TOnGetItem read FOnGetItem write FOnGetItem;
    property BagItemForeColor:TColor read FBagItemForeColor write FBagItemForeColor;
    property BagItemBackColor:TColor read FBagItemBackColor write FBagItemBackColor;
  published
    property Lines:TStrings read FDrawLines write SetLines;
    property TopLines:TStrings read FDrawTopLines write SetTopLines;
    property AutoScroll:Boolean read FAutoScroll write FAutoScroll;
    property DrawSelect:Boolean read FDrawSelect write FDrawSelect;
    property DrawBorder:Boolean read FDrawBorder write FDrawBorder;

    property OnItemClick:TItemClickEvent read FOnItemClick write FOnItemClick;
    property OnItemMouseMove:TItemClickEvent read FOnItemMouseMove write FOnItemMouseMove;
    property OnItemMouseLeave:TNotifyEvent read FOnItemMouseLeave write FOnItemMouseLeave;

    property FontBackTransparent:Boolean read FFontBackTransparent write FFontBackTransparent;
    property FontName:string read FFontName write SetFontName;
    property FontSize:Integer read FFontSize write SetFontSize;
    property FontStroke:Boolean read FFontStroke write SetFontStroke;
  end;

  TDxTreeNode = class
  private
    FOwner:TDxTreeView;
    FParent:TDxTreeNode;

    FList:TList;
    FCaption:string;

    FStyle:TButtonStyle;
    FChecked:Boolean;
    FExpand:Boolean;
    FLevel:Integer;
    FIndex:Integer;

    FClickSound:TClickSound;
    FOnClick:TOnClickEx;
    FOnClickSound:TOnClickSound;

    procedure SetCaption(Value:string);
    procedure SetExpand(Value:Boolean);
    procedure SetChecked(Value:Boolean);
    function GetLevel:Integer;
    function GetItem(Index:Integer):TDxTreeNode;
    function GetCount:Integer;

  protected

  public
    Data:Pointer;
    CaptionColor:TDxCaptionColor;
    MouseDowned:Boolean;
    MouseMoveed:Boolean;
    constructor Create();
    destructor Destroy(); override;

    property List:TList read FList;
    property Owner:TDxTreeView read FOwner write FOwner;
    property Parent:TDxTreeNode read FParent write FParent;
    property Caption:string read FCaption write SetCaption;
    property Style:TButtonStyle read FStyle write FStyle;
    property Expand:Boolean read FExpand write SetExpand;
    property Checked:Boolean read FChecked write SetChecked;
    property ClickCount:TClickSound read FClickSound write FClickSound;
    property OnClickSound:TOnClickSound read FOnClickSound write FOnClickSound;
    property Items[Index:Integer]:TDxTreeNode read GetItem;
    property Count:Integer read GetCount;
    property Level:Integer read GetLevel;
    procedure Add(Item:TDxTreeNode); overload;
    procedure Add(TreeNodeList:TList); overload;
    procedure Delete(Item:TDxTreeNode);
    procedure Clear;
    function IndexOf(Item:TDxTreeNode):Integer;
    function ExpandCount:Integer;
  end;

  TDxTreeViewPaintExpandButtonEvent = procedure(Sender:TObject; PaintRect:TRect; IsExpand:Boolean) of object; stdcall;
  TDxTreeView = class(TDxScrollControl)
  private
    FList:TList;
    TreeNodeList:TList;
    FOnSelect:TTreeNodeEventEvent;
    FShowButton:Boolean;
    FStyle:TButtonStyle;
    FRadioTreeNode:TDxTreeNode;
    FDownTreeNode:TDxTreeNode;

    FMoveTreeNode:TDxTreeNode;
    FCriticalSection:TRTLCriticalSection;

    FIsOverPaintExpandButton:Boolean;
    FOnPaintExpandButton:TDxTreeViewPaintExpandButtonEvent;

    function GetItem(Index:Integer):TDxTreeNode;
    function Get(Index:Integer):TDxTreeNode;
    function GetCount:Integer;
  protected
    procedure DoClick(X, Y:Integer); override;
    procedure AutoSetShowScroll; override;

  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure DoScroll(Value:Integer); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy(); override;
    function MaxValue:Integer; override;
    procedure Paint; override;
    property Items[Index:Integer]:TDxTreeNode read GetItem;
    property Indexs[Index:Integer]:TDxTreeNode read Get;
    property Count:Integer read GetCount;
    procedure Add(Item:TDxTreeNode);
    procedure Delete(Item:TDxTreeNode);
    procedure Clear;
    procedure RefTreeNodeList();
    function IndexOf(Item:TDxTreeNode):Integer;
    procedure Lock;
    procedure UnLock;
    property OnSelect:TTreeNodeEventEvent read FOnSelect write FOnSelect;
  published
    property Style:TButtonStyle read FStyle write FStyle;
    property ShowButton:Boolean read FShowButton write FShowButton;

    property IsOverPaintExpandButton:Boolean read FIsOverPaintExpandButton write FIsOverPaintExpandButton default False;
    property OnPaintExpandButton:TDxTreeViewPaintExpandButtonEvent read FOnPaintExpandButton write FOnPaintExpandButton;
  end;

  TDxListItem = class(TStringList)

  private
    FDown:Boolean;
    FMove:Boolean;
    FStyle:TButtonStyle;
    FChecked:Boolean;
    FSelected:Boolean;
    FItemList:array of TViewItem;

    function GetItem(Index:Integer):pTViewItem;
    function GetMouseDown(Index:Integer):Boolean;
    function GetMouseMove(Index:Integer):Boolean;
    procedure SetMouseDown(Index:Integer; Value:Boolean);
    procedure SetMouseMove(Index:Integer; Value:Boolean);

    function GetChecked(Index:Integer):Boolean;
    procedure SetChecked(Index:Integer; Value:Boolean);
  protected

  public
    Owner:TDxListView;
    constructor Create;
    destructor Destroy; override;

    function AddObject(const S:string; AObject:TObject):Integer; override;
    procedure Clear; override;
    procedure Delete(Index:Integer); override;
    procedure InsertObject(Index:Integer; const S:string;
      AObject:TObject); override;

    function AddItem(const S:string; AObject:TObject):pTViewItem;

    property Items[Index:Integer]:pTViewItem read GetItem;

    property ItemDown[Index:Integer]:Boolean read GetMouseDown write SetMouseDown;
    property ItemMove[Index:Integer]:Boolean read GetMouseMove write SetMouseMove;
    property ItemChecked[Index:Integer]:Boolean read GetChecked write SetChecked;

    property Style:TButtonStyle read FStyle write FStyle;
    property Checked:Boolean read FChecked write FChecked;
    property Selected:Boolean read FSelected write FSelected;

    property Down:Boolean read FDown write FDown;
    property Move:Boolean read FMove write FMove;
  published

  end;

  TViewField = class(TPersistent)
  private
    FCaption:WideString;
    FColor:TDxCaptionColor;
    FAlignment:TAlignment;
    FOnChange:TNotifyEvent;
    function GetCaption:string;
    procedure SetCaption(Value:string);
    procedure Changed;
  protected
  public
    constructor Create();
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
  published
    property Color:TDxCaptionColor read FColor write FColor;
    property Caption:string read GetCaption write SetCaption;
    property Alignment:TAlignment read FAlignment write FAlignment;
  end;

  TDxListView = class(TDxScrollControl)
  private
    FLines:TList;
    FColRects:array of TRect;
    FFields:array of TViewField;
    FCurrViewField:TViewField;
    FTopIndex:Integer;
    FOnListItemClick:TOnListItem;
    FOnListItemMouseDown:TOnListItemMouseDown;
    FOnListItemMouseMove:TOnListItemMouseMove;

    FDownListItem:TDxListItem;
    FMoveListItem:TDxListItem;

    FDownViewItem:pTViewItem;
    FMoveViewItem:pTViewItem;

    FCol, FRow:Integer;

    FMouseX, FMouseY:Integer;

    FShowGridLine:Boolean;
    FGridLineColor:TColor;

    FOnViewItemPaint:TOnViewItemPaint;
    FCheckItemControlSize:Boolean;

    FMouseDownFieldLine:Boolean;
    FMouseMoveFieldLine:Boolean;
    FChangeFieldRects:PRect;
    FSelectCol:Integer;
    FMouseDownCol:Integer;
    FSelectControl:Boolean;
    FCriticalSection:TRTLCriticalSection;

    FCanSelect:Boolean;

    function GetCount:Integer;
    function GetViewItem(Index:Integer):TDxListItem;
    function GetColRect(Index:Integer):TRect;
    function GetColCount:Integer;
    procedure SetColRect(Index:Integer; Value:TRect);
    procedure SetColCount(Value:Integer);
    function GetViewRect:TRect;
    procedure SetHeaderHeight(Value:Integer);
    function GetField(Index:Integer):TViewField;
    procedure SetField(Index:Integer; Value:TViewField);
    // function GetViewField: TViewField;
    // procedure SetViewField(Value: TViewField);

    procedure ViewFieldChanged(Sender:TObject); stdcall;
  protected
    function CanMove:Boolean; override;
    procedure ChangeShowItemCount(); override;
    procedure DoResize(var NewRect:TRect); override;
    procedure DoClick(X, Y:Integer); override;
    procedure DoHide; override;
    function SelectControl(X, Y:Integer; ColRect:TRect; ViewItem:pTViewItem):Boolean;
  public
    procedure MouseLeave; override;
    function InRange(X, Y:Integer):Boolean; override;
    procedure DoScroll(Value:Integer); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;

  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    function MaxValue:Integer; override;
    procedure Paint; override;
    function Add:TDxListItem;
    procedure Delete(Index:Integer);
    procedure Clear;
    procedure Lock;
    procedure UnLock;
    property Count:Integer read GetCount;
    property Items[Index:Integer]:TDxListItem read GetViewItem;

    property HeaderHeight:Integer read FOffSetY write SetHeaderHeight;
    property ColRects[Index:Integer]:TRect read GetColRect write SetColRect;
    property Fields[Index:Integer]:TViewField read GetField write SetField;
    property ViewRect:TRect read GetViewRect;

    property OnViewItemPaint:TOnViewItemPaint read FOnViewItemPaint write FOnViewItemPaint;
    property OnListItemClick:TOnListItem read FOnListItemClick write FOnListItemClick;

    property OnListItemMouseDown:TOnListItemMouseDown read FOnListItemMouseDown write FOnListItemMouseDown;
    property OnListItemMouseMove:TOnListItemMouseMove read FOnListItemMouseMove write FOnListItemMouseMove;

    property DownViewItem:pTViewItem read FDownViewItem;
    property MoveViewItem:pTViewItem read FMoveViewItem write FMoveViewItem;
  published
    // property Field: TViewField read FCurrViewField write SetViewField;
    property Field:TViewField read FCurrViewField write FCurrViewField;
    // property Field: TViewField read GetViewField write SetViewField;
    property ColCount:Integer read GetColCount write SetColCount;
    property ShowGridLine:Boolean read FShowGridLine write FShowGridLine;
    property GridLineColor:TColor read FGridLineColor write FGridLineColor;
    property CheckItemControlSize:Boolean read FCheckItemControlSize write FCheckItemControlSize;
    property CanSelect:Boolean read FCanSelect write FCanSelect;
  end;

function GetStrinLineExText(Line:TStringLineEx; IncludeItem:Boolean = True):string;
procedure GetTextListEx(HGEFont:THGEFont; const Text:WideString; const FColor, BColor:TColor; TextList:TList; LineBackColor:
  TColor = clNone; MaxWidth:Integer = 0; ItemFColor:TColor = clBlue; ItemBColor:TColor = clWhite); overload;
procedure GetTextListEx(const Text:WideString; const FColor, BColor:TColor; TokenLine:TStringLineEx; ItemFColor:TColor = clBlue; ItemBColor:TColor = clWhite); overload;

implementation

uses
  Math,
  HGECanvas;

constructor TDxScrollControl.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  AutoSize := False;
  FShowScroll := True;
  FScrollSize := 16;
  FScrollBars := ssHorizontal;

  FAutoShowScroll := False;
  FMouseHorizontal := False;
  Transparent := True;

  Width := 200;
  Height := 100;
  FScrollImageIndex := TDxImageIndex.Create;
  FPrevImageIndex := TDxImageIndex.Create;
  FNextImageIndex := TDxImageIndex.Create;
  FBarImageIndex := TDxImageIndex.Create;

  FScrollImageIndex.OnChange := ScrollImageIndexChange;
  FPrevImageIndex.OnChange := PrevImageIndexChange;
  FNextImageIndex.OnChange := NextImageIndexChange;
  FBarImageIndex.OnChange := BarImageIndexChange;

  FScrollImageIndex.OnGetImage := ImageIndex.OnGetImage;
  FPrevImageIndex.OnGetImage := ImageIndex.OnGetImage;
  FNextImageIndex.OnGetImage := ImageIndex.OnGetImage;
  FBarImageIndex.OnGetImage := ImageIndex.OnGetImage;

  FPrevMouseDown := False;
  FNextMouseDown := False;
  FBarMouseDown := False;
  FScrollMouseDown := False;

  FPrevMouseMove := False;
  FNextMouseMove := False;
  FBarMouseMove := False;

  FItemIndex := -1;
  FPosition := 0;
  FItemHeight := 12;
  FOnScroll := nil;

  FVisibleItemCount := 0;
  FExpandSize := 0;
  FBarTop := 0;
  FOffSetX := 0;
  FOffSetY := 0;
  FShowItemCount := 0;
  FMouseMoveTick := MyGetTickCount;
  OwnerMove := True;
  FCanMouseWheel := True;

  FSpringStep := 50;
end;

destructor TDxScrollControl.Destroy;
begin
  FScrollImageIndex.Free;
  FPrevImageIndex.Free;
  FNextImageIndex.Free;
  FBarImageIndex.Free;
  inherited Destroy;
end;
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------

procedure TDxScrollControl.SetOnGetImage(Value:TOnGetImage);
begin
  inherited SetOnGetImage(Value);
  FScrollImageIndex.OnGetImage := Value;
  FPrevImageIndex.OnGetImage := Value;
  FNextImageIndex.OnGetImage := Value;
  FBarImageIndex.OnGetImage := Value;
end;

procedure TDxScrollControl.ScrollImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if (FScrollImageIndex.Image <> nil) then begin
    if FScrollImageIndex.Up >= 0 then
      nIndex := FScrollImageIndex.Up
    else if FScrollImageIndex.Hot >= 0 then
      nIndex := FScrollImageIndex.Hot
    else if FScrollImageIndex.Down >= 0 then
      nIndex := FScrollImageIndex.Down;

    if (nIndex >= 0) then begin
      Texture := FScrollImageIndex.Image.Images[nIndex];
      if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then
        FScrollSize := Texture.Width;
    end;
  end;
end;

procedure TDxScrollControl.PrevImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if (PrevImageIndex.Image <> nil) then begin
    if FPrevImageIndex.Up >= 0 then
      nIndex := FPrevImageIndex.Up
    else if FPrevImageIndex.Hot >= 0 then
      nIndex := FPrevImageIndex.Hot
    else if FPrevImageIndex.Down >= 0 then
      nIndex := FPrevImageIndex.Down;

    if (nIndex >= 0) then begin
      Texture := PrevImageIndex.Image.Images[nIndex];
      if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then
        FPrevImageSize := Texture.Height;
    end;
  end;
end;

procedure TDxScrollControl.NextImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if FNextImageIndex.Up >= 0 then
    nIndex := FNextImageIndex.Up
  else if FNextImageIndex.Hot >= 0 then
    nIndex := FNextImageIndex.Hot
  else if FNextImageIndex.Down >= 0 then
    nIndex := FNextImageIndex.Down;

  if (NextImageIndex.Image <> nil) and (nIndex >= 0) then begin
    Texture := NextImageIndex.Image.Images[nIndex];
    if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then
      FNextImageSize := Texture.Height;
  end;
end;

procedure TDxScrollControl.BarImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if FBarImageIndex.Up >= 0 then
    nIndex := FBarImageIndex.Up
  else if FBarImageIndex.Hot >= 0 then
    nIndex := FBarImageIndex.Hot
  else if FBarImageIndex.Down >= 0 then
    nIndex := FBarImageIndex.Down;

  if (BarImageIndex.Image <> nil) and (nIndex >= 0) then begin
    Texture := BarImageIndex.Image.Images[nIndex];
    if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then
      FBarImageSize := Texture.Height;
  end;
end;

function TDxScrollControl.GetVisibleHeight:Integer;
begin
  if FMouseHorizontal then
    Result := Width - OffSetX
  else
    Result := Height - OffSetY;
end;

function TDxScrollControl.InPrevRange(X, Y:Integer; vRect:TRect):Boolean; // 检测鼠标点在 向上的按钮
begin
  Result := (X >= vRect.Right - FScrollSize) and (Y <= vRect.Top + FPrevImageSize);
end;

function TDxScrollControl.InNextRange(X, Y:Integer; vRect:TRect):Boolean; // 检测鼠标点在 向下的按钮
begin
  Result := (X >= vRect.Right - FScrollSize) and (Y >= vRect.Top + (Height - FNextImageSize));
end;

function TDxScrollControl.InBarRange(X, Y:Integer; vRect:TRect):Boolean; // 检测鼠标点在滚动条的按钮
var
  nHeight:Integer;
  nMaxValue:Integer;
  nBarTop:Integer;
begin
  Result := False;
  if FShowScroll then begin
    if (X >= vRect.Right - FScrollSize) and (Y < vRect.Bottom - FNextImageSize) and
      (Y > vRect.Top + FPrevImageSize) then begin
      if FPosition > 0 then begin
        nMaxValue := MaxValue;
        nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;
        if (nMaxValue > 0) and (nHeight > 0) and (nMaxValue > VisibleHeight) then
          nBarTop := Round(FPosition * nHeight / (nMaxValue - VisibleHeight))
        else
          nBarTop := 0;
        Result := (Y >= vRect.Top + FPrevImageSize + nBarTop) and (Y <= vRect.Top + FPrevImageSize + nBarTop + FBarImageSize);
      end
      else begin
        Result := (Y <= vRect.Top + FPrevImageSize + FBarImageSize);
      end;
    end;
  end;
end;

procedure TDxScrollControl.DoScroll(Value:Integer); // 滚动
var
  I:Integer;
  D:TDxControl;
begin
  if Value <> 0 then begin
    for I := 0 to ControlCount - 1 do begin
      D := Control[I];
      if FMouseHorizontal then
        D.Left := D.Left + Value
      else
        D.Top := D.Top + Value;
    end;
  end;
  if (Assigned(FOnScroll)) then FOnScroll(Self);
end;

procedure TDxScrollControl.AutoCalcShowItemCount;
begin
  ShowItemCount := VisibleHeight div ItemHeight;
end;

procedure TDxScrollControl.SetExpandSize(Value:Integer);
var
  nMaxValue, nMaxScrollValue:Integer;
begin
  if FExpandSize <> Value then begin
    FExpandSize := Value;
    if FShowScroll then begin
      nMaxValue := MaxValue;
      nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

      if (nMaxValue > 0) and (nMaxScrollValue > 0) then
        FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
      else
        FBarTop := 0;

      if nMaxValue > (Height) then begin
        if FPosition + VisibleHeight >= nMaxValue then
          FBarTop := nMaxScrollValue
        else if FPosition = 0 then
          FBarTop := 0;
      end
      else
        FBarTop := 0;
    end;
  end;
end;

function TDxScrollControl.MinValue:Integer;
var
  I:Integer;
  D:TDxControl;
  nMinValue:Integer;
begin
  nMinValue := 0;
  for I := 0 to ControlCount - 1 do begin
    D := Control[I];
    nMinValue := Min(nMinValue, D.Top);
  end;

  Result := nMinValue;
end;

function TDxScrollControl.MaxValue:Integer;
var
  I:Integer;
  D:TDxControl;
  nMaxValue:Integer;
begin
  FMaxRect := Rect(High(Integer), High(Integer), 0, 0);
  for I := 0 to ControlCount - 1 do begin
    D := Control[I];
    FMaxRect := LongRect(FMaxRect, D.ClientRect);
  end;

  if FMouseHorizontal then
    nMaxValue := Max(FMaxRect.Right - Min(FMaxRect.Left, 0), 0)
  else
    nMaxValue := Max(FMaxRect.Bottom - Min(FMaxRect.Top, 0), 0);

  if VisibleItemCount > 0 then begin
    if nMaxValue > VisibleHeight then begin
      if (nMaxValue mod VisibleHeight) > VisibleItemCount * ItemHeight then begin
        nMaxValue := nMaxValue + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end
    else begin
      if nMaxValue > VisibleItemCount * ItemHeight then begin
        nMaxValue := nMaxValue + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end;
  end;

  Result := nMaxValue + FExpandSize; // ItemHeight;
end;

procedure TDxScrollControl.SetVisibleItemCount(Value:Integer);
var
  nMaxValue, nMaxScrollValue:Integer;
begin
  if VisibleHeight div ItemHeight < Value then
    FVisibleItemCount := 0
  else
    FVisibleItemCount := Value;

  if FShowScroll then begin
    nMaxValue := MaxValue;
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > (Height) then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := nMaxScrollValue
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.SetPosition(Value:Integer); // 设置滚动指针
var
  P, nMaxValue, nMaxScrollValue:Integer;
begin
  nMaxValue := MaxValue;
  if FPosition <> Value then begin
    P := Value;
    if P < 0 then P := 0;

    if VisibleHeight > nMaxValue then begin
      P := 0;
    end
    else begin
      if P + VisibleHeight > nMaxValue then
        P := nMaxValue - VisibleHeight;
    end;

    DoScroll(FPosition - P);
    FPosition := P;
  end;

  if FShowScroll then begin
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > VisibleHeight then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := Height - FPrevImageSize - FNextImageSize - FBarImageSize
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.Next; // 向下滚动
var
  P:Integer;
  nMaxScrollValue:Integer;
  nMaxValue:Integer;
begin
  nMaxValue := MaxValue;
  if (nMaxValue > 0) and (nMaxValue > VisibleHeight) then begin
    if (FPosition + VisibleHeight < nMaxValue) then begin
      if FPosition + VisibleHeight + FItemHeight <= nMaxValue then begin
        P := FPosition + FItemHeight;
        DoScroll(FPosition - P);
        FPosition := P;
      end
      else begin
        P := nMaxValue - VisibleHeight;
        DoScroll(FPosition - P);
        FPosition := P;
      end;
    end;
  end
  else begin
    if FPosition > 0 then begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;

  if FShowScroll then begin
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > (Height) then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := nMaxScrollValue
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.Previous; // 向上滚动
var
  P:Integer;
  nMaxScrollValue:Integer;
  nMaxValue:Integer;
begin
  nMaxValue := MaxValue;
  if FPosition > 0 then begin
    if (nMaxValue > 0) and (nMaxValue > VisibleHeight) then begin
      if FPosition - FItemHeight >= 0 then begin
        P := FPosition - FItemHeight;
        DoScroll(FPosition - P);
        FPosition := P;
      end
      else begin
        P := 0;
        DoScroll(FPosition - P);
        FPosition := P;
      end;
    end
    else begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;

  if FShowScroll then begin
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > VisibleHeight then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := nMaxScrollValue
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.First; // 滚动条回到顶部
var
  P:Integer;
  nMaxScrollValue:Integer;
  nMaxValue, nMinValue:Integer;
begin
  nMinValue := MinValue;
  if nMinValue < 0 then begin
    DoScroll(Max(abs(nMinValue), FPosition));
    FPosition := 0;
  end;
  if FPosition > 0 then begin
    P := 0;
    DoScroll(FPosition - P);
    FPosition := P;
  end;

  if FShowScroll then begin
    nMaxValue := MaxValue;
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > VisibleHeight then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := nMaxScrollValue
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.Last; // 滚动条回到底部
var
  P:Integer;
  nMaxScrollValue:Integer;
  nMaxValue:Integer;
begin
  nMaxValue := MaxValue;
  if (nMaxValue > 0) and (nMaxValue > VisibleHeight) then begin
    if FPosition + VisibleHeight < nMaxValue then begin
      P := nMaxValue - VisibleHeight;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end
  else begin
    if FPosition > 0 then begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;

  if FShowScroll then begin
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > VisibleHeight then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := nMaxScrollValue
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else
      FBarTop := 0;
  end;
end;

procedure TDxScrollControl.SetShowItemCount(Value:Integer);
begin
  if FShowItemCount <> Value then begin
    FShowItemCount := Value;
    ChangeShowItemCount;
  end;
end;

procedure TDxScrollControl.SetAutoShowScroll(Value:Boolean);
begin
  if FAutoShowScroll <> Value then begin
    FAutoShowScroll := Value;
    if Value then begin
      AutoSetShowScroll;
    end;
  end;
end;

procedure TDxScrollControl.ChangeShowItemCount;
begin

end;

procedure TDxScrollControl.DoClick(X, Y:Integer);
begin
  inherited;
end;

procedure TDxScrollControl.MouseWheelDown(Shift:TShiftState; MousePos:TPoint);
begin
  if FCanMouseWheel then begin
    inherited MouseWheelDown(Shift, MousePos);
    Next;
  end;
end;

procedure TDxScrollControl.MouseWheelUp(Shift:TShiftState; MousePos:TPoint);
begin
  if FCanMouseWheel then begin
    inherited MouseWheelUp(Shift, MousePos);
    Previous;
  end;
end;

procedure TDxScrollControl.SetItemIndex(Value:Integer);
var
  nItemCount:Integer;
begin
  if FItemIndex <> Value then begin
    FItemIndex := Value;
    nItemCount := (MaxValue - VisibleItemCount * ItemHeight) div FItemHeight;
    if FItemIndex >= nItemCount then FItemIndex := -1;
    if FItemIndex >= 0 then begin
      Position := FItemIndex * FItemHeight;
    end;
  end;
end;

procedure TDxScrollControl.AutoSetShowScroll;
begin

end;

procedure TDxScrollControl.SetItemHeight(Value:Integer);
begin
  FItemHeight := Max(Value, 1);
end;

procedure TDxScrollControl.SetScrollSize(Value:Integer);
begin
  if FScrollSize <> Value then begin
    FScrollSize := Value;
  end;
end;

procedure TDxScrollControl.SetScrollBars(Value:TScrollStyle);
begin
  if FScrollBars <> Value then begin
    FScrollBars := Value;
  end;
end;

procedure TDxScrollControl.DoMouseUp();
begin
  FBarMouseDown := False;
  FPrevMouseDown := False;
  FNextMouseDown := False;
  FScrollMouseDown := False;
  FMouseScrollDown := False;
end;

procedure TDxScrollControl.DoMouseEnter;
begin
  inherited;
end;

procedure TDxScrollControl.DoMouseLeave;
begin
  FPrevMouseMove := False;
  FNextMouseMove := False;
  FBarMouseMove := False;
  inherited;
end;

procedure TDxScrollControl.DoResize(var NewRect:TRect);
var
  nMaxScrollValue:Integer;
  nMaxValue:Integer;
begin
  inherited;
  if FShowScroll then begin
    nMaxValue := MaxValue;
    nMaxScrollValue := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nMaxScrollValue > 0) then
      FBarTop := Max(Round(FPosition * nMaxScrollValue / (nMaxValue - VisibleHeight)), 0)
    else
      FBarTop := 0;

    if nMaxValue > VisibleHeight then begin
      if FPosition + VisibleHeight >= nMaxValue then
        FBarTop := Height - FPrevImageSize - FNextImageSize - FBarImageSize
      else if FPosition = 0 then
        FBarTop := 0;
    end
    else begin
      FBarTop := 0;
      Position := 0;
    end;
  end;
end;

function TDxScrollControl.CanMove:Boolean;
begin
  Result := not (FScrollMouseDown or FPrevMouseDown or FNextMouseDown or FBarMouseDown); // and MouseDowned;
end;

function TDxScrollControl.ScrollMouseDown(Button:TMouseButton; X, Y:Integer):Boolean;
var
  nHeight:Integer;
  nMaxValue:Integer;
  nBarTop:Integer;
  vRect:TRect;
  vtRect:TRect;
begin
  Result := False;
  vtRect := VirtualRect;
  vRect := vtRect;
  FMouseMoveTick := MyGetTickCount + 600;
  if FShowScroll then begin
    if (Button = mbLeft) then begin
      FScrollMouseDown := (X >= vRect.Right - FScrollSize);
      Result := FScrollMouseDown;
      if not (FPrevMouseDown or FNextMouseDown or FBarMouseDown) then begin
        FPrevMouseDown := InPrevRange(X, Y, vRect);
        FNextMouseDown := InNextRange(X, Y, vRect);
        FBarMouseDown := InBarRange(X, Y, vRect);
      end;

      if not (FPrevMouseDown or FNextMouseDown or FBarMouseDown) then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
        if PointInRect(Point(X, Y), vRect) then begin
          nMaxValue := MaxValue;
          nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;
          if ((nMaxValue > 0) and (nMaxValue > VisibleHeight) and (nHeight > 0)) or (Position > 0) then begin
            nBarTop := Max(Y - vRect.Top - FPrevImageSize - FBarImageSize div 2, 0);
            Position := Round(nBarTop * (nMaxValue - VisibleHeight) / nHeight);
          end else begin
            Position := 0;
          end;
          // Exit;
        end;
      end else begin
        if FPrevMouseDown and InPrevRange(X, Y, vRect) then begin
          Previous;
        end else if FNextMouseDown and InNextRange(X, Y, vRect) then begin
          Next;
        end;
      end;
    end;
    FItemIndex := (Y - vtRect.Top - OffSetY) div FItemHeight + FPosition div FItemHeight;
    {
    if FItemIndex >= (MaxValue - VisibleItemCount * ItemHeight) div FItemHeight then
      FItemIndex := -1;
    }
  end else begin
    //HZQ  20230525 解决在不显示滚动条时，计算ItemIndex, 否则ItemIndex永远为-1
    FItemIndex := (Y - vtRect.Top - OffSetY) div FItemHeight + FPosition div FItemHeight;
  end;
  if FMouseScroll then begin
    FMouseScrollDown := True;
    FMouseX := X;
    FMouseY := Y;
  end;
end;

function TDxScrollControl.ScrollMouseMove(Shift:TShiftState; X, Y:Integer):Boolean;
var
  nHeight:Integer;
  nMaxValue:Integer;
  nBarTop:Integer;
  vtRect:TRect;
  vRect:TRect;
begin
  Result := False;
  if FShowScroll then begin
    vtRect := VirtualRect;
    vRect := vtRect;

    if not (FPrevMouseDown or FNextMouseDown or FBarMouseDown) then begin // or MouseDowned or FScrollMouseDown
      FPrevMouseMove := InPrevRange(X, Y, vRect);
      FNextMouseMove := InNextRange(X, Y, vRect);
      FBarMouseMove := InBarRange(X, Y, vRect);
    end;

    // Result := (FPrevMouseDown or FNextMouseDown or FBarMouseDown or FScrollMouseDown);
  end;
  if FBarMouseDown and FShowScroll then begin
    FPrevMouseMove := False;
    FNextMouseMove := False;

    FNextMouseDown := False;
    FPrevMouseDown := False;

    nMaxValue := MaxValue;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;
    if ((nMaxValue > 0) and (nMaxValue > VisibleHeight) and (nHeight > 0)) or (Position > 0) then begin
      nBarTop := Max(Y - vRect.Top - FPrevImageSize - FBarImageSize div 2, 0);
      Position := Round(nBarTop * (nMaxValue - VisibleHeight) / nHeight);
    end
    else begin
      Position := 0;
    end;
  end
  else begin
    if FPrevMouseDown then begin
      if Longint(MyGetTickCount - FMouseMoveTick) > 100 then begin
        FMouseMoveTick := MyGetTickCount;
        Previous;
      end;
    end
    else if FNextMouseDown then begin
      if Longint(MyGetTickCount - FMouseMoveTick) > 100 then begin
        FMouseMoveTick := MyGetTickCount;
        Next;
      end;
    end;
    vRect := VirtualRect;
    FHotItemIndex := (Y - vRect.Top - OffSetY) div FItemHeight + FPosition div FItemHeight;
  end;

  if FMouseScrollDown then begin
    if (FMouseX <> X) and FMouseHorizontal then begin
      //      if FMouseX > X then
      //        Position := Position + 1
      //      else
      //        Position := Position - 1;
      Position := Position + (FMouseX - X);

      FMouseX := X;
    end;
    if (FMouseY <> Y) and not FMouseHorizontal then begin
      //      if FMouseY > Y then
      //        Position := Position + (FMouseY - Y)
      //      else
      Position := Position + (FMouseY - Y);

      FMouseY := Y;
    end;
  end;
end;

function TDxScrollControl.ScrollMouseUp():Boolean;
begin
  Result := True;
  FBarMouseDown := False;
  FPrevMouseDown := False;
  FNextMouseDown := False;
  FScrollMouseDown := False;
  FMouseScrollDown := False;
end;

{---------------------------------TDxScrollBox------------------------------------}

function TDxScrollBox.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vRect := VirtualRect;
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

procedure TDxScrollBox.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  FStartSpring := False;
  FfSpeedX := 0;
  FfSpeedY := 0;
  FDX := X;
  FDY := Y;
  FDTime := MyGetTickCount;
  ScrollMouseDown(Button, X, Y);
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxScrollBox.MouseMove(Shift:TShiftState; X, Y:Integer);
begin
  FMTime := MyGetTickCount;
  ScrollMouseMove(Shift, X, Y);
  inherited MouseMove(Shift, X, Y);
end;

procedure TDxScrollBox.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  InScroll:Boolean;
begin
  //  FUX := X;
  //  FUY := Y;
  //  FUTime := MyGetTickCount;
  if (FDX <> -1) and (FDY <> -1) and not FStartSpring and (MyGetTickCount - FMTime < 100) then begin
    FfSpeedX := (FDX - X) / (MyGetTickCount - FDTime) * FSpringStep;
    FfSpeedY := (FDY - Y) / (MyGetTickCount - FDTime) * FSpringStep;
    FStartSpring := True;
  end;
  InScroll := (FBarMouseDown or FPrevMouseDown or FNextMouseDown);
  ScrollMouseUp();
  if not InScroll then
    inherited MouseUp(Button, Shift, X, Y);
end;

procedure TDxScrollBox.Update;
begin
  inherited;
  if not FStartSpring then Exit;
  if FMouseHorizontal then begin
    Position := Position + Trunc(FfSpeedX);
    FfSpeedX := FfSpeedX * 0.9;
    if (Abs(FfSpeedX) < 1.0) or (Position + VisibleHeight >= MaxValue) or (Position = 0) then begin
      FStartSpring := False;
      FDX := -1;
      FDY := -1;
      FfSpeedX := 0.0;
    end;
  end
  else begin
    Position := Position + Trunc(FfSpeedY);
    FfSpeedY := FfSpeedY * 0.9;
    if (Abs(FfSpeedY) < 1.0) or (Position + VisibleHeight >= MaxValue) or (Position = 0) then begin
      FStartSpring := False;
      FDX := -1;
      FDY := -1;
      FfSpeedY := 0.0;
    end;
  end;
end;

procedure TDxScrollBox.Paint;
var
  I, nIndex:Integer;
  Texture:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if Designing then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  if not Transparent then
    FillRect(vtRect, vtRect, vbRect, BackgroundColor);

  DoPaint();

  // ClipRect := GameCanvas.ClipRect;
  // GameCanvas.ClipRect := vbRect;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else begin
    if ImageIndex.Image <> nil then begin
      if ImageIndex.Up >= 0 then begin
        Texture := ImageIndex.Image.Images[ImageIndex.Up];
        if Texture <> nil then begin
          DrawRect(vtRect, vtRect, vbRect, Texture);
        end;
      end;
    end;
  end;

  for I := ControlCount - 1 downto 0 do
    if Control[I].Visible then
      Control[I].Paint;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
  //  if not (TDxControl(Owner) is TDxScrollBox) then
  //  CurrentFont.TextOut(vbRect.Left, vbRect.Top, Format('FfSpeedX:%f  FfSpeedY:%f',[FfSpeedX, FfSpeedY]), clWhite);
  if FShowScroll then begin
    if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
      Texture := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
      if Texture <> nil then begin
        PaintRect := vtRect;
        PaintRect.Left := vtRect.Left + (Width - Texture.Width);
        PaintRect.Right := PaintRect.Left + Texture.Width;
        GameCanvas.StretchDraw(PaintRect, Texture);
      end;
    end;

    if FPrevImageIndex.Image <> nil then begin
      nIndex := -1;
      if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then
        nIndex := FPrevImageIndex.Down
      else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then
        nIndex := FPrevImageIndex.Hot
      else if (FPrevImageIndex.Up >= 0) then
        nIndex := FPrevImageIndex.Up;
      if (nIndex >= 0) then begin
        Texture := FPrevImageIndex.Image.Images[nIndex];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Top + 1;
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;

    if FNextImageIndex.Image <> nil then begin
      nIndex := -1;
      if FNextMouseDown and (FNextImageIndex.Down >= 0) then
        nIndex := FNextImageIndex.Down
      else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then
        nIndex := FNextImageIndex.Hot
      else if (FNextImageIndex.Up >= 0) then
        nIndex := FNextImageIndex.Up;

      if nIndex >= 0 then begin
        Texture := FNextImageIndex.Image.Images[nIndex];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Top + (Height - Texture.Height - 1);
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;

    if FBarImageIndex.Image <> nil then begin
      nIndex := -1;
      if FBarMouseDown and (FBarImageIndex.Down >= 0) then
        nIndex := FBarImageIndex.Down
      else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then
        nIndex := FBarImageIndex.Hot
      else if (FBarImageIndex.Up >= 0) then
        nIndex := FBarImageIndex.Up;

      if nIndex >= 0 then begin
        Texture := FBarImageIndex.Image.Images[nIndex];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - FScrollSize) + (FScrollSize - Texture.Width) div 2 + 1;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Top + FPrevImageSize + FBarTop;
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;
  end;
  // GameCanvas.ClipRect := ClipRect;
end;

{-------------------------------------------------------------------------------}

{------------------------------------------------------------------------------}

constructor TDxLines.Create;
begin
  inherited;
  FItemList := nil;
  FItemHeightList := nil;
end;

destructor TDxLines.Destroy;
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    ViewItem := FItemList[I];
    ViewItem.Color.Free;
    ViewItem.ImageIndex.Free;
    ViewItem.Items.Free;
    if ViewItem.StringLineEx <> nil then
      ViewItem.StringLineEx.Free;
    Dispose(ViewItem);
  end;
  SetLength(FItemList, 0);
  SetLength(FItemHeightList, 0);
  FItemList := nil;
  FItemHeightList := nil;
  inherited;
end;

function TDxLines.AddItem(const S:string):pTViewItem;
begin
  AddObject(S, nil);
  Result := FItemList[Length(FItemList) - 1];
end;

function TDxLines.AddObject(const S:string; AObject:TObject):Integer;
var
  ViewItem:pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  SetLength(FItemHeightList, Length(FItemHeightList) + 1);
  FItemHeightList[Length(FItemHeightList) - 1] := 0;
  New(ViewItem);
  FItemList[Length(FItemList) - 1] := ViewItem;
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  ViewItem.ParentViewItem := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.Items := TList.Create;
  ViewItem.TimeTick := MyGetTickCount;
  ViewItem.ImageIndex.OnGetImage := Owner.OnGetImage;
  ViewItem.StringLineEx := nil;
  Result := inherited AddObject(S, AObject);
end;

procedure TDxLines.Clear;
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    ViewItem := FItemList[I];
    ViewItem.Color.Free;
    ViewItem.ImageIndex.Free;
    ViewItem.Items.Free;
    if ViewItem.StringLineEx <> nil then
      ViewItem.StringLineEx.Free;
    Dispose(ViewItem);
  end;
  SetLength(FItemList, 0);
  SetLength(FItemHeightList, 0);
  FItemList := nil;
  FItemHeightList := nil;
  inherited Clear;
end;

procedure TDxLines.Delete(Index:Integer);
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  ViewItem := FItemList[Index];
  ViewItem.Color.Free;
  ViewItem.ImageIndex.Free;
  ViewItem.Items.Free;
  if ViewItem.StringLineEx <> nil then
    ViewItem.StringLineEx.Free;
  Dispose(ViewItem);

  for I := Index to Length(FItemList) - 2 do begin
    FItemList[I] := FItemList[I + 1];
    FItemHeightList[I] := FItemHeightList[I + 1];
  end;
  SetLength(FItemList, Length(FItemList) - 1);
  SetLength(FItemHeightList, Length(FItemHeightList) - 1);
  inherited Delete(Index);
end;

procedure TDxLines.InsertObject(Index:Integer; const S:string;
  AObject:TObject);
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  SetLength(FItemHeightList, Length(FItemHeightList) + 1);
  for I := Length(FItemList) - 1 downto Index do begin
    FItemList[I] := FItemList[I - 1];
    FItemHeightList[I] := FItemHeightList[I - 1];
  end;
  FItemHeightList[Index] := 0;
  New(ViewItem);
  FItemList[Index] := ViewItem;
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  ViewItem.ParentViewItem := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;

  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.Items := TList.Create;
  ViewItem.TimeTick := MyGetTickCount;
  ViewItem.ImageIndex.OnGetImage := Owner.OnGetImage;
  inherited InsertObject(Index, S, AObject);
end;

function TDxLines.GetItem(Index:Integer):pTViewItem;
begin
  Result := nil;

  if FItemList = nil then
    Exit;
  if (Index < 0) or (Index >= Length(FItemList)) then
    Exit;

  Result := FItemList[Index];
end;

function TDxLines.GetItemHeight(Index:Integer):Integer;
begin
  Result := FItemHeightList[Index];
end;

procedure TDxLines.SetItemHeight(Index:Integer; Value:Integer);
begin
  FItemHeightList[Index] := Value;
end;
{-------------------------------------------------------------------------------}

constructor TDxChatMemo.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  InitializeCriticalSection(FCriticalSection);
  FDrawLines := TDxLines.Create;
  FDrawTopLines := TDxLines.Create;
  TDxLines(FDrawLines).Owner := Self;
  TDxLines(FDrawTopLines).Owner := Self;

  FLines := TDxLines.Create;
  FTopLines := TDxLines.Create;
  TDxLines(FLines).Owner := Self;
  TDxLines(FTopLines).Owner := Self;
  //FTextList := TStringList.Create;
  inherited Create(AOwner);
  FDrawSelect := False;
  FDrawBorder := False;
  FShowScroll := False;
  FShowItemCount := 9;

  FDeleteCount := 0;
  FDeleteTopCount := 0;

  FDownViewItem := nil;
  FMoveViewItem := nil;
  FOnChange := nil;
  FOnGetItem := nil;
  FAutoScroll := False;
  FOffSetX := 0;
  FTopIndex := 0;

  Height := 12 * FShowItemCount;
  Width := 388;
  VisibleItemCount := 1;

  FFontBackTransparent := False;
  FFontName := '';
  FFontSize := 9;
  FFontStroke := False;

  FBagItemForeColor := clBlue;
  FBagItemBackColor := clWhite;
end;

destructor TDxChatMemo.Destroy;
begin
  FDrawLines.Free;

  FDrawTopLines.Free;

  FLines.Free;
  FTopLines.Free;
  //FTextList.Free;
  DeleteCriticalSection(FCriticalSection);

  inherited Destroy;
end;

// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------

function TDxChatMemo.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vRect := VirtualRect;
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

procedure TDxChatMemo.DoScroll(Value:Integer);
begin
  FTopIndex := (Position - Value) div ItemHeight;
end;

procedure TDxChatMemo.DoResize(var NewRect:TRect);
var
  nNewWidth:Integer;
begin
  nNewWidth := NewRect.Right - NewRect.Top;
  if nNewWidth <> Width then
    RefDrawList;
  inherited;
end;

procedure TDxChatMemo.DoItemClick(ItemName:string; MakeIndex:Integer; R:TRect);
begin
  if Assigned(FOnItemClick) then
    FOnItemClick(Self, ItemName, MakeIndex, R);
end;

procedure TDxChatMemo.DoItemMouseMove(ItemName:string; MakeIndex:Integer; R:TRect);
begin
  if Assigned(FOnItemMouseMove) then
    FOnItemMouseMove(Self, ItemName, MakeIndex, R);
end;

procedure TDxChatMemo.DoItemMouseLeave;
begin
  if Assigned(FOnItemMouseLeave) then
    FOnItemMouseLeave(Self);
end;

procedure TdxChatMemo.AutoSetShowScroll;
begin
  if FAutoShowScroll then
    ShowScroll := (FDrawLines.Count + FDrawTopLines.Count) > Min(VisibleHeight div ItemHeight, ShowItemCount);
end;

procedure TDxChatMemo.SetLines(Value:TStrings);
begin
  Lock;
  try
    FDownViewItem := nil;
    FMoveViewItem := nil;
    FLines.Clear;
    FLines.AddStrings(Value);
  finally
    UnLock;
  end;
  RefDrawList;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.SetTopLines(Value:TStrings);
begin
  Lock;
  try
    FDownViewItem := nil;
    FMoveViewItem := nil;
    FTopLines.Clear;
    FTopLines.AddStrings(Value);
  finally
    UnLock;
  end;
  RefDrawList;
  AutoSetShowScroll;
end;

procedure TDxChatMemo.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  nRow:Integer;
  vRect:TRect;
  ViewItem:pTViewItem;

  I:Integer;
  StringLineEx:TStringLineEx;
  StringToken:PStringToken;
  nX1, nX2:Integer;
  HGEFont:THGEFont;

  R1, R2:TRect;
begin
  if not ScrollMouseDown(Button, X, Y) then begin
    vRect := VirtualRect;

    ViewItem := nil;

    nRow := (Y - vRect.Top - OffSetY) div ItemHeight;
    if nRow >= (MaxValue - VisibleItemCount * ItemHeight) div ItemHeight then
      nRow := -1;
    if (nRow >= 0) and (nRow < FDrawTopLines.Count) then begin
      ViewItem := pTViewItem(TDxLines(FDrawTopLines).Items[nRow])
    end else begin
      nRow := (Y - vRect.Top - OffSetY) div ItemHeight + Position div ItemHeight - FDrawTopLines.Count;
      if nRow >= (MaxValue - VisibleItemCount * ItemHeight) div ItemHeight then
        nRow := -1;
      if (nRow >= 0) and (nRow < FDrawLines.Count) then begin
        ViewItem := pTViewItem(TDxLines(FDrawLines).Items[nRow]);
      end
    end;

    if nRow >= 0 then begin
      if FDownViewItem <> ViewItem then begin
        FDownViewItem := ViewItem;
      end;

      if FDownViewItem <> nil then begin
        HGEFont := TextureFonts.FindFont(FDownViewItem.Color.Down.Name, FDownViewItem.Color.Down.Size, FDownViewItem.Color.Down.Style);

        if HGEFont <> nil then begin
          StringLineEx := TStringLineEx(FDownViewItem.StringLineEx);
          nX1 := OffSetX;
          if StringLineEx <> nil then begin
            for I := 0 to StringLineEx.Count - 1 do begin
              StringToken := StringLineEx[I];
              nX2 := nX1 + HGEFont.TextWidth(StringToken.Text);
              if (X - vRect.Left >= nX1) and (X - vRect.Left <= nX2) then begin
                if StringToken.TokenType = tt_Item then begin
                  R1 := VisibleRect;
                  OffsetRect(R1, 0, OffsetY);
                  nRow := Max((Y - vRect.Top - OffSetY) div ItemHeight, 0);
                  R2 := Rect(R1.Left + nX1, R1.Top + nRow * ItemHeight, R1.Left + nX2, R1.Top + (nRow + 1) * ItemHeight);
                  DoItemClick(StringToken.Text, StringToken.Flag, R2);
                  Exit;
                end;
              end;
              nX1 := nX2;
            end;
          end;
        end;
      end;
    end;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxChatMemo.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  nRow:Integer;
  vRect:TRect;
  ViewItem:pTViewItem;

  I:Integer;
  StringLineEx:TStringLineEx;
  StringToken:PStringToken;
  nX1, nX2:Integer;
  HGEFont:THGEFont;

  R1, R2:TRect;
  IsInSayItem:Boolean;
begin
  IsInSayItem := False;

  FMoveViewItem := nil;
  if not ScrollMouseMove(Shift, X, Y) then begin
    vRect := VirtualRect;

    ViewItem := nil;

    nRow := (Y - vRect.Top - OffSetY) div ItemHeight;
    if nRow >= (MaxValue - VisibleItemCount * ItemHeight) div ItemHeight then
      nRow := -1;
    if (nRow >= 0) and (nRow < FDrawTopLines.Count) then begin
      ViewItem := pTViewItem(TDxLines(FDrawTopLines).Items[nRow])
    end
    else begin
      nRow := (Y - vRect.Top - OffSetY) div ItemHeight + Position div ItemHeight - FDrawTopLines.Count;
      if nRow >= (MaxValue - VisibleItemCount * ItemHeight) div ItemHeight then
        nRow := -1;
      if (nRow >= 0) and (nRow < FDrawLines.Count) then begin
        ViewItem := pTViewItem(TDxLines(FDrawLines).Items[nRow]);
      end
    end;

    if nRow >= 0 then begin
      if FMoveViewItem <> ViewItem then
        FMoveViewItem := ViewItem;

      if FMoveViewItem <> nil then begin
        HGEFont := TextureFonts.FindFont(FMoveViewItem.Color.Down.Name, FMoveViewItem.Color.Down.Size, FMoveViewItem.Color.Down.Style);

        if HGEFont <> nil then begin
          StringLineEx := TStringLineEx(FMoveViewItem.StringLineEx);
          nX1 := OffSetX;
          if StringLineEx <> nil then begin
            for I := 0 to StringLineEx.Count - 1 do begin
              StringToken := StringLineEx[I];
              nX2 := nX1 + HGEFont.TextWidth(StringToken.Text);
              if (X - vRect.Left >= nX1) and (X - vRect.Left <= nX2) then begin
                if StringToken.TokenType = tt_Item then begin
                  IsInSayItem := True;
                  R1 := VisibleRect;
                  OffsetRect(R1, 0, OffsetY);
                  nRow := Max((Y - vRect.Top - OffSetY) div ItemHeight, 0);
                  R2 := Rect(R1.Left + nX1, R1.Top + nRow * ItemHeight, R1.Left + nX2, R1.Top + (nRow + 1) * ItemHeight);
                  DoItemMouseMove(StringToken.Text, StringToken.Flag, R2);
                  Break;
                end;
              end;
              nX1 := nX2;
            end;
          end;
        end;
      end;
    end;
  end;
  inherited MouseMove(Shift, X, Y);

  if not IsInSayItem then begin
    DoItemMouseLeave();
  end;
end;

procedure TDxChatMemo.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  InScroll:Boolean;
begin
  InScroll := (FBarMouseDown or FPrevMouseDown or FNextMouseDown);
  FDownViewItem := nil;
  FMoveViewItem := nil;
  if not InScroll then
    inherited MouseUp(Button, Shift, X, Y);
end;

procedure TDxChatMemo.KeyDown(var Key:Word; Shift:TShiftState);
begin
  case Key of
    VK_UP:Previous;
    VK_DOWN:Next;
    VK_PRIOR:if Position >= Height then
        Position := Position - Height
      else
        Position := 0;
    VK_NEXT:if Position + Height < MaxValue then
        Position := Position + Height
      else
        Position := MaxValue;
  end;
  inherited KeyDown(Key, Shift);
end;

function TDxChatMemo.MaxValue:Integer;
begin
  Result := (FDrawTopLines.Count + FDrawLines.Count + 1) * ItemHeight;
  // 修正滚动条拖到最后时下边空一大块 chongchong 2015-07-01
  if VisibleItemCount > 0 then begin
    //Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
  end;
  Result := Result + ExpandSize;
end;

procedure TDxChatMemo.LoadFromFile(const FileName:string);
begin
  Lock;
  try
    FDownViewItem := nil;
    FMoveViewItem := nil;
    FLines.Clear;
    FTopLines.Clear;
    if FileExists(FileName) then
      FLines.LoadFromFile(FileName);
  finally
    UnLock;
  end;
  RefDrawList();
end;

function GetStrinLineExText(Line:TStringLineEx; IncludeItem:Boolean):string;
var
  I:Integer;
begin
  Result := '';
  if IncludeItem then begin
    for I := 0 to Line.Count - 1 do begin
      Result := Result + Line[I].Text;
    end;
  end
  else begin
    for I := 0 to Line.Count - 1 do begin
      if Line[I].TokenType <> tt_Item then
        Result := Result + Line[I].Text;
    end;
  end;
end;

{ TODO -ochongchong  -c新增 : 让聊天框的文字支持自定义颜色----支持多行 【2013-08-27】 }
// {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}

procedure GetTextListEx(HGEFont:THGEFont; const Text:WideString; const FColor, BColor:TColor; TextList:TList; LineBackColor:TColor; MaxWidth:Integer; ItemFColor, ItemBColor:TColor);
var
  Index, Index2, CustomTextLen:Integer;
  STemp, SLine:WideString;
  WChar:WideChar;
  OutToken:TStringToken;
  SRemberDef, SRemberCustom:WideString;

  function NewLine:TStringLineEx;
  begin
    Result := TStringLineEx.Create;
    Result.FLineBackColor := LineBackColor;
    TextList.Add(Result);
  end;

  function ProcessCustomColor(var StringToken:TStringToken; var Len:Integer):Boolean;
  var
    PWChar:PWideChar;
    FoundIndex:Integer;
    FoundText:WideString;
    S1, S2, S3:string;
  begin
    //aa{aabbcc|249:200:0}ee
    // abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz{aabbcc|249:200:0}
    // abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789{aabbcc|249:200:0}
    PWChar := PWideChar(Text);
    Inc(PWChar, Index);
    FoundIndex := Pos(WideString('}'), WideString(PWChar)); //HZQ, 解决Pos的重载问题 20230423
    if FoundIndex = 0 then begin
      Result := False;
      Exit;
    end;

    Len := FoundIndex + 1;

    // 取{}中间一部分 aabbcc|100:200:0
    FoundText := Copy(Text, Index + 1, FoundIndex - 1);
    FoundIndex := Pos(WideString('|'), FoundText); //HZQ
    if FoundIndex = 0 then begin
      FoundIndex := Pos(WideString('/'), FoundText);
      if FoundIndex = 0 then begin
        Result := False;
        Exit;
      end;

      StringToken.TokenType := tt_Item;
      StringToken.Text := Copy(FoundText, 1, FoundIndex - 1);
      FoundText := Copy(FoundText, FoundIndex + 1, MaxInt);
      StringToken.Flag := StrToIntDef(FoundText, 0);
      StringToken.FColor := ItemFColor;
      StringToken.BColor := ItemBColor;

      Result := True;
    end else begin
      StringToken.TokenType := tt_Text;

      StringToken.Text := Copy(FoundText, 1, FoundIndex - 1); // aabbcc
      FoundText := Copy(FoundText, FoundIndex + 1, MaxInt); // 100:200:0
      if Length(FoundText) = 0 then begin // |后面没内容
        Result := False;
        Exit;
      end;

      FoundText := GetValidStr3(FoundText, S1, [' ', #9, ':']);
      FoundText := GetValidStr3(FoundText, S2, [' ', #9, ':']);
      FoundText := GetValidStr3(FoundText, S3, [' ', #9, ':']);

      StringToken.FColor := ColorIndexToTColor(StrToIntDef(S1, 255));
      StringToken.BColor := ColorIndexToTColor(StrToIntDef(S2, 255));
      StringToken.Flag := StrToIntDef(S3, 0);

      Result := True;
    end;
  end;

  function DoProcessText:Boolean;
  begin
    Result := False;
    STemp := SLine + WChar;
    if (MaxWidth > 0) and (HGEFont.TextWidth(STemp) > MaxWidth) then begin
      if Length(SRemberCustom) > 0 then
        TStringLineEx(TextList[TextList.Count - 1]).Add(FColor, BColor, 0, Copy(SLine, Length(SRemberCustom) + 1, MaxInt), tt_Text)
      else
        NewLine.Add(FColor, BColor, 0, SLine, tt_Text);
      SLine := '';
      SRemberCustom := '';
    end else begin
      SLine := STemp;
      Inc(Index);
      if Index > Length(Text) then begin
        if Length(SRemberCustom) > 0 then
          TStringLineEx(TextList[TextList.Count - 1]).Add(FColor, BColor, 0, Copy(SLine, Length(SRemberCustom) + 1, MaxInt), tt_Text)
        else
          NewLine.Add(FColor, BColor, 0, SLine, tt_Text);
        SRemberCustom := '';
        Result := True;
        //Break;
      end;
    end;
  end;
begin
  TextList.Clear;

  Index := 1;
  SLine := '';
  STemp := '';

  //aa{aabbcc|249:200:0}ee

  while True do begin
    if Index > Length(Text) then Break;
    WChar := Text[Index];
    if WChar <> '{' then begin
      if DoProcessText then Break;
    end else begin
      if not ProcessCustomColor(OutToken, CustomTextLen) then begin
        // 如果不符合定义颜色的格式，则照常处理
        if DoProcessText then Break;
      end
      else begin
        // 如果符合定义颜色的格式，则将自定义格式中的内容处理完

        SRemberDef := sLine;
        if (Length(sLine) > 0) then begin
          // {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}
          if Length(SRemberCustom) > 0 then begin
            if Length(sLine) > Length(SRemberCustom) then
              TStringLineEx(TextList[TextList.Count - 1]).Add(FColor, BColor, 0, Copy(SLine, Length(SRemberCustom) + 1, MaxInt), tt_Text)
          end
          else
            NewLine.Add(FColor, BColor, 0, SLine, tt_Text);
        end;

        Index2 := 1;
        while True do begin
          if Index2 > Length(OutToken.Text) then Break;
          WChar := OutToken.Text[Index2];

          STemp := SLine + WChar;
          if (MaxWidth > 0) and (HGEFont.TextWidth(STemp) > MaxWidth) then begin
            if Length(SRemberDef) > 0 then
              TStringLineEx(TextList[TextList.Count - 1]).Add(OutToken.FColor, OutToken.BColor, OutToken.Flag, Copy(SLine, Length(SRemberDef) + 1, MaxInt), OutToken.TokenType)
            else
              NewLine.Add(OutToken.FColor, OutToken.BColor, OutToken.Flag, SLine, OutToken.TokenType);
            SRemberDef := '';
            SLine := '';
          end
          else begin
            SLine := STemp;
            Inc(Index2);
            if Index2 > Length(OutToken.Text) then begin
              if Length(SRemberDef) > 0 then
                TStringLineEx(TextList[TextList.Count - 1]).Add(OutToken.FColor, OutToken.BColor, OutToken.Flag, Copy(SLine, Length(SRemberDef) + 1, MaxInt), OutToken.TokenType)
              else
                NewLine.Add(OutToken.FColor, OutToken.BColor, OutToken.Flag, SLine, OutToken.TokenType);
              SRemberDef := '';
              SRemberCustom := SLine;
              Break;
            end;
          end;
        end;

        Index := Index + CustomTextLen;
      end;
    end;
  end;
end;

{ TODO -ochongchong  -c新增 : 让聊天框的文字支持自定义颜色 --- 仅单行 【2013-08-27】 }
// {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}

procedure GetTextListEx(const Text:WideString; const FColor, BColor:TColor; TokenLine:TStringLineEx; ItemFColor, ItemBColor:TColor);
var
  Index, Index2, CustomTextLen:Integer;
  SLine:WideString;
  WChar:WideChar;
  OutToken:TStringToken;
  SRemberDef, SRemberCustom:WideString;

  function ProcessCustomColor(var StringToken:TStringToken; var Len:Integer):Boolean;
  var
    PWChar:PWideChar;
    FoundIndex:Integer;
    FoundText:WideString;
    S1, S2, S3:string;
  begin
    //aa{aabbcc|249:200:0}ee
    // abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz{aabbcc|249:200:0}
    // abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789{aabbcc|249:200:0}
    PWChar := PWideChar(Text);
    Inc(PWChar, Index);
    FoundIndex := Pos(WideString('}'), WideString(PWChar));
    if FoundIndex = 0 then begin
      Result := False;
      Exit;
    end;

    Len := FoundIndex + 1;

    // 取{}中间一部分 aabbcc|100:200:0
    FoundText := Copy(Text, Index + 1, FoundIndex - 1);
    FoundIndex := Pos(WideString('|'), FoundText);
    if FoundIndex = 0 then begin
      FoundIndex := Pos(WideString('/'), FoundText);
      if FoundIndex = 0 then begin
        Result := False;
        Exit;
      end;
      StringToken.TokenType := tt_Item;
      StringToken.Text := Copy(FoundText, 1, FoundIndex - 1);
      FoundText := Copy(FoundText, FoundIndex + 1, MaxInt);
      StringToken.Flag := StrToIntDef(FoundText, 0);
      StringToken.FColor := ItemFColor;
      StringToken.BColor := ItemBColor;

      Result := True;
    end else begin
      StringToken.TokenType := tt_Text;
      StringToken.Text := Copy(FoundText, 1, FoundIndex - 1); // aabbcc
      FoundText := Copy(FoundText, FoundIndex + 1, MaxInt); // 100:200:0

      if Length(FoundText) = 0 then {// |后面没内容} begin
        Result := False;
        Exit;
      end;

      FoundText := GetValidStr3(FoundText, S1, [' ', #9, ':']);
      FoundText := GetValidStr3(FoundText, S2, [' ', #9, ':']);
      FoundText := GetValidStr3(FoundText, S3, [' ', #9, ':']);

      StringToken.FColor := ColorIndexToTColor(StrToIntDef(S1, 255));
      StringToken.BColor := ColorIndexToTColor(StrToIntDef(S2, 255));
      StringToken.Flag := StrToIntDef(S3, 0);

      Result := True;
    end;
  end;

  function DoProcessText:Boolean;
  begin
    Result := False;
    SLine := SLine + WChar;
    Inc(Index);
    if Index > Length(Text) then begin
      TokenLine.Add(FColor, BColor, 0, Copy(SLine, Length(SRemberCustom) + 1, MaxInt), tt_Text);
      SRemberCustom := '';
      Result := True;
    end;
  end;
begin
  TokenLine.Clear;

  Index := 1;
  SLine := '';

  //aa{aabbcc|249:200:0}ee

  while True do begin
    if Index > Length(Text) then Break;
    WChar := Text[Index];
    if WChar <> '{' then begin
      if DoProcessText then Break;
    end
    else begin
      if not ProcessCustomColor(OutToken, CustomTextLen) then begin
        // 如果不符合定义颜色的格式，则照常处理
        if DoProcessText then Break;
      end
      else begin
        // 如果符合定义颜色的格式，则将自定义格式中的内容处理完

        SRemberDef := sLine;
        if (Length(sLine) > 0) then begin
          // {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}
          if Length(SRemberCustom) > 0 then begin
            if Length(sLine) > Length(SRemberCustom) then
              TokenLine.Add(FColor, BColor, 0, Copy(SLine, Length(SRemberCustom) + 1, MaxInt), tt_Text)
          end
          else
            TokenLine.Add(FColor, BColor, 0, SLine, tt_Text);
        end;

        Index2 := 1;
        while True do begin
          if Index2 > Length(OutToken.Text) then Break;
          WChar := OutToken.Text[Index2];

          SLine := SLine + WChar;
          Inc(Index2);
          if Index2 > Length(OutToken.Text) then begin
            TokenLine.Add(OutToken.FColor, OutToken.BColor, OutToken.Flag, Copy(SLine, Length(SRemberDef) + 1, MaxInt), OutToken.TokenType);
            SRemberDef := '';
            SRemberCustom := SLine;
            Break;
          end;
        end;

        Index := Index + CustomTextLen;
      end;
    end;
  end;
end;

procedure TDxChatMemo.RefDrawList;
var
  I, II:Integer;
  ViewItem:pTViewItem;
  AddViewItem:pTViewItem;
  TextList:TList;
  sText:string;
  MaxTextWidth:Integer;
  HGEFont:THGEFont;
begin
  Lock;
  TextList := TList.Create;
  try

    FDownViewItem := nil;
    FMoveViewItem := nil;

    FDrawLines.Clear;

    FDrawTopLines.Clear;

    Position := 0;
    FDeleteCount := 0;
    FDeleteTopCount := 0;

    if ShowScroll then
      MaxTextWidth := Width - ScrollSize - OffSetX
    else
      MaxTextWidth := Width - OffSetX;

    for I := 0 to FTopLines.Count - 1 do begin
      ViewItem := TDxLines(FTopLines).Items[I];

      // GetTextList(ViewItem.Caption);

      HGEFont := TextureFonts.FindFont(FFontName, FFontSize, []);
      GetTextListEx({CurrentFont} HGEFont, ViewItem.Caption, ViewItem.Color.Up.Color, ViewItem.Color.Up.BColor, TextList, ViewItem.Color.Up.BColor, MaxTextWidth, FBagItemForeColor, FBagItemBackColor);

      for II := 0 to TextList.Count - 1 do begin
        sText := GetStrinLineExText(TextList[II]);

        AddViewItem := TDxLines(FDrawTopLines).AddItem(sText);
        AddViewItem.StringLineEx := TextList[II];

        AddViewItem.Color.Up.Name := FFontName;
        AddViewItem.Color.Hot.Name := FFontName;
        AddViewItem.Color.Down.Name := FFontName;
        AddViewItem.Color.Disabled.Name := FFontName;
        AddViewItem.Color.Checked.Name := FFontName;

        AddViewItem.Color.Up.Size := FFontSize;
        AddViewItem.Color.Hot.Size := FFontSize;
        AddViewItem.Color.Down.Size := FFontSize;
        AddViewItem.Color.Disabled.Size := FFontSize;
        AddViewItem.Color.Checked.Size := FFontSize;

        ViewItem.Items.Add(AddViewItem);
        AddViewItem.ParentViewItem := ViewItem;
        AddViewItem.TimeTick := ViewItem.TimeTick;
        AddViewItem.Transparent := False;
        AddViewItem.Caption := sText;
        AddViewItem.Color.Up.Color := ViewItem.Color.Up.Color;
        AddViewItem.Color.Up.BColor := ViewItem.Color.Up.BColor;
        AddViewItem.Color.Up.Bold := ViewItem.Color.Up.Bold;
        AddViewItem.Color.Hot.Assign(AddViewItem.Color.Up);
        AddViewItem.Color.Down.Assign(AddViewItem.Color.Up);
        AddViewItem.Color.Disabled.Assign(AddViewItem.Color.Up);
        AddViewItem.CaptionTexture.Width := 0;
        AddViewItem.CaptionTexture.Height := 0;
        AddViewItem.CaptionTexture.ImageIndexs := nil;

        // 修复最后一行在默认情况下不显示 注释 {+ 1} chongchong 2013-11-14
        if FAutoScroll and ((FDrawTopLines.Count + (FDrawLines.Count - FTopIndex {+ 1})) * ItemHeight > VisibleHeight) then Next;
        // if (Assigned(FOnChange)) then FOnChange(Self);
      end;
    end;

    if ShowScroll then
      MaxTextWidth := Width - ScrollSize - OffSetX
    else
      MaxTextWidth := Width - OffSetX;

    for I := 0 to FLines.Count - 1 do begin
      ViewItem := TDxLines(FLines).Items[I];
      //GetTextList(ViewItem.Caption);

      HGEFont := TextureFonts.FindFont(FFontName, FFontSize, []);
      GetTextListEx({CurrentFont} HGEFont, ViewItem.Caption, ViewItem.Color.Up.Color, ViewItem.Color.Up.BColor, TextList, clNone, MaxTextWidth, FBagItemForeColor, FBagItemBackColor);

      for II := 0 to TextList.Count - 1 do begin
        sText := GetStrinLineExText(TextList[II]);

        AddViewItem := TDxLines(FDrawLines).AddItem(sText);
        AddViewItem.StringLineEx := TextList[II];

        AddViewItem.Color.Up.Name := FFontName;
        AddViewItem.Color.Hot.Name := FFontName;
        AddViewItem.Color.Down.Name := FFontName;
        AddViewItem.Color.Disabled.Name := FFontName;
        AddViewItem.Color.Checked.Name := FFontName;

        AddViewItem.Color.Up.Size := FFontSize;
        AddViewItem.Color.Hot.Size := FFontSize;
        AddViewItem.Color.Down.Size := FFontSize;
        AddViewItem.Color.Disabled.Size := FFontSize;
        AddViewItem.Color.Checked.Size := FFontSize;

        ViewItem.Items.Add(AddViewItem);
        AddViewItem.ParentViewItem := ViewItem;
        AddViewItem.Transparent := False;
        AddViewItem.Caption := sText;
        AddViewItem.Color.Up.Color := ViewItem.Color.Up.Color;
        AddViewItem.Color.Up.BColor := ViewItem.Color.Up.BColor;
        AddViewItem.Color.Up.Bold := ViewItem.Color.Up.Bold;
        AddViewItem.Color.Hot.Assign(AddViewItem.Color.Up);
        AddViewItem.Color.Down.Assign(AddViewItem.Color.Up);
        AddViewItem.Color.Disabled.Assign(AddViewItem.Color.Up);
        AddViewItem.CaptionTexture.Width := 0;
        AddViewItem.CaptionTexture.Height := 0;
        AddViewItem.CaptionTexture.ImageIndexs := nil;

        TDxLines(FDrawLines).ItemHeights[FDrawLines.Count - 1] := Max(TDxLines(FDrawLines).ItemHeights[FDrawLines.Count - 1], g_CurrentFontHeight);

        // 修复最后一行在默认情况下不显示 注释 {+ 1} chongchong 2013-11-14
        if FAutoScroll and ((FDrawTopLines.Count + (FDrawLines.Count - FTopIndex {+ 1})) * ItemHeight > VisibleHeight) then Next;
        // if (Assigned(FOnChange)) then FOnChange(Self);
      end;
    end;
  finally
    TextList.Free;
    UnLock;
  end;
end;

procedure TDxChatMemo.Insert(Index:Integer; const S:string; FC, BC:TColor);
begin

end;

procedure TDxChatMemo.SetFontName(Value:string);
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  if FFontName <> Value then begin
    FFontName := Value;

    for I := 0 to FDrawLines.Count - 1 do begin
      ViewItem := TDxLines(FDrawLines).Items[I];
      ViewItem.Color.Down.Name := FFontName;
      ViewItem.Color.Up.Name := FFontName;
      ViewItem.Color.Hot.Name := FFontName;
      ViewItem.Color.Checked.Name := FFontName;
    end;

    RefDrawList;
  end;
end;

procedure TDxChatMemo.SetFontSize(Value:Integer);
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  if FFontSize <> Value then begin
    FFontSize := Value;

    for I := 0 to FDrawLines.Count - 1 do begin
      ViewItem := TDxLines(FDrawLines).Items[I];
      ViewItem.Color.Down.Size := FFontSize;
      ViewItem.Color.Up.Size := FFontSize;
      ViewItem.Color.Hot.Size := FFontSize;
      ViewItem.Color.Checked.Size := FFontSize;
    end;

    RefDrawList;
  end;
end;

procedure TDxChatMemo.SetFontStroke(Value:Boolean);
begin
  if FFontStroke <> Value then begin
    FFontStroke := Value;
  end;
end;

(*
procedure TDxChatMemo.GetTextList(const Text: string);
var
  TextWidth: Integer;
  Index, Len: Integer;
  S, sText: string;
  wText: WideString;
begin
  FTextList.Clear;
  if ShowScroll then
    TextWidth := Width - ScrollSize
  else
    TextWidth := Width;

  Len := CurrentFont.TextWidth(Text);

  if Len > TextWidth then
  begin
    sText := '';
    Index := 1;
    wText := Text;

    while True do
    begin
      if Index > Length(wText) then break;
      S := sText + wText[Index];
      if CurrentFont.TextWidth(S) > TextWidth then
      begin
        FTextList.Add(sText);
        sText := ' ';
      end else
      begin
        sText := sText + wText[Index];
        Inc(Index);
        if Index > Length(wText) then
        begin
          FTextList.Add(sText);
          break;
        end;
      end;
    end;
  end else
  begin
    FTextList.Add(Text);
  end;
end;
*)

procedure TDxChatMemo.Add(const S:string; FC, BC:TColor);
var
  I:integer;
  sText:string;

  ViewItem:pTViewItem;
  AddViewItem:pTViewItem;

  TextList:TList;
  MaxTextWidth:Integer;
  HGEFont:THGEFont;
begin
  Lock;
  TextList := TList.Create;
  try
    ViewItem := TDxLines(FLines).AddItem(S);

    ViewItem.Color.Up.Name := FFontName;
    ViewItem.Color.Hot.Name := FFontName;
    ViewItem.Color.Down.Name := FFontName;
    ViewItem.Color.Disabled.Name := FFontName;
    ViewItem.Color.Checked.Name := FFontName;

    ViewItem.Color.Up.Size := FFontSize;
    ViewItem.Color.Hot.Size := FFontSize;
    ViewItem.Color.Down.Size := FFontSize;
    ViewItem.Color.Disabled.Size := FFontSize;
    ViewItem.Color.Checked.Size := FFontSize;

    ViewItem.StringLineEx := nil;
    ViewItem.Transparent := False;
    ViewItem.Caption := S;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);

    ViewItem.CaptionTexture.Width := 0;
    ViewItem.CaptionTexture.Height := 0;
    ViewItem.CaptionTexture.ImageIndexs := nil;

    if ShowScroll then
      MaxTextWidth := Width - ScrollSize - OffSetX
    else
      MaxTextWidth := Width - OffSetX;

    HGEFont := TextureFonts.FindFont(FFontName, FFontSize, []);
    GetTextListEx({CurrentFont} HGEFont, S, FC, BC, TextList, clNone, MaxTextWidth, FBagItemForeColor, FBagItemBackColor);

    for I := 0 to TextList.Count - 1 do begin
      sText := GetStrinLineExText(TextList[I]);
      AddViewItem := TDxLines(FDrawLines).AddItem(sText);

      AddViewItem.Color.Up.Name := FFontName;
      AddViewItem.Color.Hot.Name := FFontName;
      AddViewItem.Color.Down.Name := FFontName;
      AddViewItem.Color.Disabled.Name := FFontName;
      AddViewItem.Color.Checked.Name := FFontName;

      AddViewItem.Color.Up.Size := FFontSize;
      AddViewItem.Color.Hot.Size := FFontSize;
      AddViewItem.Color.Down.Size := FFontSize;
      AddViewItem.Color.Disabled.Size := FFontSize;
      AddViewItem.Color.Checked.Size := FFontSize;

      AddViewItem.StringLineEx := TextList[I];
      ViewItem.Items.Add(AddViewItem);
      AddViewItem.ParentViewItem := ViewItem;
      AddViewItem.Transparent := False;
      AddViewItem.Caption := sText;
      AddViewItem.Color.Up.Color := FC;
      AddViewItem.Color.Up.BColor := BC;
      AddViewItem.Color.Up.Bold := False;
      AddViewItem.Color.Hot.Assign(AddViewItem.Color.Up);
      AddViewItem.Color.Down.Assign(AddViewItem.Color.Up);
      AddViewItem.Color.Disabled.Assign(AddViewItem.Color.Up);

      AddViewItem.CaptionTexture.Width := 0;
      AddViewItem.CaptionTexture.Height := 0;
      AddViewItem.CaptionTexture.ImageIndexs := nil;

      TDxLines(FDrawLines).ItemHeights[FDrawLines.Count - 1] := Max(TDxLines(FDrawLines).ItemHeights[FDrawLines.Count - 1], g_CurrentFontHeight);

      // 修复最后一行在默认情况下不显示 注释 {+ 1} chongchong 2013-11-14
      if FAutoScroll and ((FDrawTopLines.Count + (FDrawLines.Count - FTopIndex { + 1})) * ItemHeight > VisibleHeight) then
        Next;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

  finally
    TextList.Free;
    UnLock;
  end;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.Delete(Index:Integer);
var
  I, MaxShowLine:Integer;
  ParentViewItem, SubViewItem:pTViewItem;
begin
  Lock;
  try
    ParentViewItem := TDxLines(FLines).Items[Index];
    if (ParentViewItem <> nil) then begin
      for I := FDrawLines.Count - 1 downto 0 do begin
        SubViewItem := TDxLines(FDrawLines).Items[I];
        if SubViewItem.ParentViewItem = ParentViewItem then begin
          if SubViewItem = FDownViewItem then FDownViewItem := nil;
          if SubViewItem = FMoveViewItem then FMoveViewItem := nil;
          FDrawLines.Delete(I);
        end;
      end;

      TDxLines(FLines).Delete(Index);
    end;

    if FAutoScroll then begin
      MaxShowLine := Min(VisibleHeight div ItemHeight, ShowItemCount);
      MaxShowLine := MaxShowLine - FDrawTopLines.Count;

      if FTopIndex + MaxShowLine >= FDrawLines.Count then begin
        FTopIndex := Max(FDrawLines.Count - MaxShowLine, 0);

        Position := FTopIndex * ItemHeight;
      end
      else
        Previous;
    end;
  finally
    UnLock;
  end;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.InsertTop(Index:Integer; const S:string; FC, BC:TColor; TimeOut:Integer);
begin

end;

procedure TDxChatMemo.AddTop(const S:string; FC, BC:TColor; TimeOut:Integer);
var
  I:integer;
  sText:string;

  ViewItem:pTViewItem;
  AddViewItem:pTViewItem;
  TextList:TList;
  MaxTextWidth:Integer;
  HGEFont:THGEFont;
begin
  Lock;
  TextList := TList.Create;
  try
    ViewItem := TDxLines(FTopLines).AddItem(S);

    ViewItem.Color.Up.Name := FFontName;
    ViewItem.Color.Hot.Name := FFontName;
    ViewItem.Color.Down.Name := FFontName;
    ViewItem.Color.Disabled.Name := FFontName;
    ViewItem.Color.Checked.Name := FFontName;

    ViewItem.Color.Up.Size := FFontSize;
    ViewItem.Color.Hot.Size := FFontSize;
    ViewItem.Color.Down.Size := FFontSize;
    ViewItem.Color.Disabled.Size := FFontSize;
    ViewItem.Color.Checked.Size := FFontSize;

    ViewItem.StringLineEx := nil;
    ViewItem.TimeTick := MyGetTickCount + TimeOut * 1000;
    ViewItem.Transparent := False;
    ViewItem.Caption := S;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.CaptionTexture.Width := 0;
    ViewItem.CaptionTexture.Height := 0;
    ViewItem.CaptionTexture.ImageIndexs := nil;

    //GetTextList(S);
    if ShowScroll then
      MaxTextWidth := Width - ScrollSize - OffSetX
    else
      MaxTextWidth := Width - OffSetX;

    HGEFont := TextureFonts.FindFont(FFontName, FFontSize, []);
    GetTextListEx({CurrentFont} HGEFont, S, FC, BC, TextList, BC, MaxTextWidth, FBagItemForeColor, FBagItemBackColor);

    for I := 0 to TextList.Count - 1 do begin
      sText := GetStrinLineExText(TextList[I]);

      AddViewItem := TDxLines(FDrawTopLines).AddItem(sText);

      AddViewItem.Color.Up.Name := FFontName;
      AddViewItem.Color.Hot.Name := FFontName;
      AddViewItem.Color.Down.Name := FFontName;
      AddViewItem.Color.Disabled.Name := FFontName;
      AddViewItem.Color.Checked.Name := FFontName;

      AddViewItem.Color.Up.Size := FFontSize;
      AddViewItem.Color.Hot.Size := FFontSize;
      AddViewItem.Color.Down.Size := FFontSize;
      AddViewItem.Color.Disabled.Size := FFontSize;
      AddViewItem.Color.Checked.Size := FFontSize;

      AddViewItem.StringLineEx := TextList[I];
      ViewItem.Items.Add(AddViewItem);
      AddViewItem.ParentViewItem := ViewItem;
      AddViewItem.TimeTick := MyGetTickCount + TimeOut * 1000;
      AddViewItem.Transparent := False;
      AddViewItem.Caption := sText;
      AddViewItem.Color.Up.Color := FC;
      AddViewItem.Color.Up.BColor := BC;
      AddViewItem.Color.Up.Bold := False;
      AddViewItem.Color.Hot.Assign(AddViewItem.Color.Up);
      AddViewItem.Color.Down.Assign(AddViewItem.Color.Up);
      AddViewItem.Color.Disabled.Assign(AddViewItem.Color.Up);
      AddViewItem.CaptionTexture.Width := 0;
      AddViewItem.CaptionTexture.Height := 0;
      AddViewItem.CaptionTexture.ImageIndexs := nil;

      // 修复最后一行在默认情况下不显示 注释 {+ 1} chongchong 2013-11-14
      if FAutoScroll and ((FDrawTopLines.Count + (FDrawLines.Count - FTopIndex {+ 1})) * ItemHeight > VisibleHeight) then Next;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;
  finally
    TextList.Free;
    UnLock;
  end;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.DeleteTop(Index:Integer);
var
  I:Integer;
  //boNeedScroll: Boolean;
  ParentViewItem, SubViewItem:pTViewItem;
begin
  Lock;
  try
    ParentViewItem := TDxLines(FTopLines).Items[Index];
    if (ParentViewItem <> nil) then begin
      for I := FDrawTopLines.Count - 1 downto 0 do begin
        SubViewItem := TDxLines(FDrawTopLines).Items[I];
        if SubViewItem.ParentViewItem = ParentViewItem then begin
          if SubViewItem = FDownViewItem then FDownViewItem := nil;
          if SubViewItem = FMoveViewItem then FMoveViewItem := nil;
          FDrawTopLines.Delete(I);
        end;
      end;

      TDxLines(FTopLines).Delete(Index);
    end;
    if FAutoScroll then Previous;
  finally
    UnLock;
  end;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.RemoveDelete;
begin

end;

procedure TDxChatMemo.RemoveDeleteTop;
begin

end;

procedure TDxChatMemo.Clear;
begin
  Lock;
  try
    FDownViewItem := nil;
    FMoveViewItem := nil;
    FLines.Clear;
    FTopLines.Clear;
    FDrawLines.Clear;
    FDrawTopLines.Clear;
    Position := 0;
    FDeleteCount := 0;
    FDeleteTopCount := 0;
  finally
    UnLock;
  end;

  AutoSetShowScroll;
end;

procedure TDxChatMemo.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDxChatMemo.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TDxChatMemo.Initialize;
begin

end;

procedure TDxChatMemo.Finalize;
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  Lock;
  try
    for I := 0 to FDrawTopLines.Count - 1 do begin
      ViewItem := TDxLines(FDrawTopLines).Items[I];
      ViewItem.CaptionTexture.Width := 0;
      ViewItem.CaptionTexture.Height := 0;
      ViewItem.CaptionTexture.ImageIndexs := nil;
    end;
    for I := 0 to FDrawLines.Count - 1 do begin
      ViewItem := TDxLines(FDrawLines).Items[I];
      ViewItem.CaptionTexture.Width := 0;
      ViewItem.CaptionTexture.Height := 0;
      ViewItem.CaptionTexture.ImageIndexs := nil;
    end;
  finally
    UnLock;
  end;
end;

procedure TDxChatMemo.Paint; // 开始绘制
var
  I, {II, III,} nIndex, {nCount,} nLeft, nTop, n01, nX, nY:Integer;
  FaceIndex:Integer;
  //nHeight:Integer;
  nMaxValue:Integer;
  //nBarTop:Integer;

  Texture:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  DestRect:TRect;
  //ListItem:TDxListItem;
  ViewItem:pTViewItem;

  NewRect:TRect;

  Font:TDxFont;
  HGEFont:THGEFont;

  J:Integer;
  StringLineEx:TStringLineEx;
  StringToken:PStringToken;

  StartLine:Integer;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  if Designing or FDrawBorder then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  if not Transparent then
    FillRect(vtRect, vtRect, vbRect, BackgroundColor);

  DoPaint();

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else begin
    n01 := 0;
    nTop := OffSetY;
    nLeft := OffSetX;

    nMaxValue := Min(VisibleHeight div ItemHeight, ShowItemCount);
    Lock;
    try
      for I := 0 to FDrawTopLines.Count - 1 do begin
        ViewItem := TDxLines(FDrawTopLines).Items[I];

        if ViewItem.ImageIndex.Image <> nil then begin
          if ViewItem.Style = bsButton then begin
            if ViewItem = FDownViewItem then begin
              FaceIndex := ViewItem.ImageIndex.Down;
              if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                FaceIndex := ViewItem.ImageIndex.Up;
            end
            else if ViewItem = FMoveViewItem then begin
              FaceIndex := ViewItem.ImageIndex.Hot;
              if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                FaceIndex := ViewItem.ImageIndex.Up;
            end
            else
              FaceIndex := ViewItem.ImageIndex.Up;
          end
          else begin
            if ViewItem = FMoveViewItem then begin
              if ViewItem.Checked then
                FaceIndex := ViewItem.ImageIndex.Down
              else if ViewItem.ImageIndex.Hot >= 0 then
                FaceIndex := ViewItem.ImageIndex.Hot
              else
                FaceIndex := ViewItem.ImageIndex.Up;
            end
            else begin
              if ViewItem.Checked then
                FaceIndex := ViewItem.ImageIndex.Down
              else
                FaceIndex := ViewItem.ImageIndex.Up;
            end;
          end;

          if FaceIndex >= 0 then begin
            Texture := ViewItem.ImageIndex.Image.Images[FaceIndex];
            if Texture <> nil then begin
              PaintRect.Left := vtRect.Left + nLeft;
              PaintRect.Right := PaintRect.Left + Texture.Width;
              PaintRect.Top := vtRect.Top + nTop + (ItemHeight - Texture.Height) div 2;
              PaintRect.Bottom := PaintRect.Top + Texture.Height;
              DrawRect(PaintRect, vtRect, vbRect, Texture);
            end;
          end;
        end;

        if ViewItem.Caption <> '' then begin
          if ViewItem.Style = bsButton then begin
            if ViewItem = FDownViewItem then begin
              Font := ViewItem.Color.Down;
            end
            else if ViewItem = FMoveViewItem then begin
              Font := ViewItem.Color.Hot;
            end
            else
              Font := ViewItem.Color.Up;
          end
          else begin
            if ViewItem = FMoveViewItem then begin
              if ViewItem.Checked then
                Font := ViewItem.Color.Down
              else if ViewItem.ImageIndex.Hot >= 0 then
                Font := ViewItem.Color.Hot
              else
                Font := ViewItem.Color.Up;
            end
            else begin
              if ViewItem.Checked then
                Font := ViewItem.Color.Down
              else
                Font := ViewItem.Color.Up;
            end;
          end;
          HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);

          if (HGEFont <> nil) then begin
            if ViewItem.StringLineEx = nil then begin
              ViewItem.CaptionTexture := HGEFont.GetImageInfo(ViewItem.Caption);

              if Length(ViewItem.CaptionTexture.ImageIndexs) > 0 then begin
                PaintRect := Rect(0, 0, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                DestRect.Left := vtRect.Left + nLeft;
                DestRect.Right := DestRect.Left + ViewItem.CaptionTexture.Width;
                DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
                DestRect.Bottom := DestRect.Top + ViewItem.CaptionTexture.Height;

                PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
                if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                  if Font.Bold then begin
                    HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                    HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                    HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                    HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                    HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                  end
                  else begin
                    if not FontBackTransparent then
                      FillRect(DestRect, vtRect, vbRect, Font.BColor);
                    HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                  end;
                end;
              end;
            end

              { TODO -ochongchong  -c新增 : 让绘制聊天框的文字支持自定义颜色 }
            else begin
              StringLineEx := TStringLineEx(ViewItem.StringLineEx);

              if StringLineEx <> nil then begin
                nLeft := OffSetX;

                if (StringLineEx.FLineBackColor <> clNone) or ((I = ItemIndex) and (DrawSelect)) then begin
                  PaintRect := Rect(0, 0, 10, HGEFont.TextHeight('|'));
                  DestRect.Left := vtRect.Left + nLeft;
                  DestRect.Right := DestRect.Left + 10;
                  DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
                  DestRect.Bottom := DestRect.Top + HGEFont.TextHeight('|');

                  PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);

                  if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                    NewRect := PaintRect;
                    if FItemHeight > NewRect.Bottom - NewRect.Top then
                      Windows.InflateRect(NewRect, 0, (FItemHeight - (NewRect.Bottom - NewRect.Top)) div 2);

                    NewRect.Right := NewRect.Left + Width - ScrollSize;
                    OffsetRect(NewRect, nX, nY);

                    if StringLineEx.FLineBackColor <> clNone then
                      FillRect(NewRect, StringLineEx.FLineBackColor);

                    if (I = ItemIndex) and (DrawSelect) then
                      FillRect(NewRect, clRed);
                  end;
                end;

                for J := 0 to StringLineEx.Count - 1 do begin
                  StringToken := StringLineEx[J];

                  ViewItem.CaptionTexture := HGEFont.GetImageInfo(StringToken.Text);

                  if Length(ViewItem.CaptionTexture.ImageIndexs) > 0 then begin
                    PaintRect := Rect(0, 0, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                    DestRect.Left := vtRect.Left + nLeft;
                    DestRect.Right := DestRect.Left + ViewItem.CaptionTexture.Width;
                    DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
                    DestRect.Bottom := DestRect.Top + ViewItem.CaptionTexture.Height;

                    PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);

                    if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                      if Font.Bold then begin
                        HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                        HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                        HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                        HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                        HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.FColor);
                      end
                      else begin
                        if not FontBackTransparent then
                          FillRect(DestRect, vtRect, vbRect, StringToken.BColor);

                        // 描边效果不好，暂时去掉
                        if (StringToken.TokenType = tt_Text) and ((StringToken.Flag <> 0) or FFontStroke) then begin
                          HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack); //StringToken.BColor
                          HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                          HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                          HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                          //HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.FColor);
                        end;

                        HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.FColor);
                      end;
                    end;

                    nLeft := nLeft + ViewItem.CaptionTexture.Width;
                  end;
                end;
              end;
            end;
          end;
        end;
        Inc(n01);
        Inc(nTop, ItemHeight);
        if n01 >= nMaxValue then break;
      end;

      nMaxValue := Min(VisibleHeight div ItemHeight, ShowItemCount);
      nMaxValue := nMaxValue - n01;

      n01 := 0;
      if nMaxValue > 0 then begin
        //StartLine := FTopIndex - nMaxValue;
        for I := FTopIndex to FDrawLines.Count - 1 do begin
          ViewItem := TDxLines(FDrawLines).Items[I];
          if (ViewItem <> nil) and (ViewItem.ImageIndex <> nil) and (ViewItem.ImageIndex.Image <> nil) then begin
            if ViewItem.Style = bsButton then begin
              if ViewItem = FDownViewItem then begin
                FaceIndex := ViewItem.ImageIndex.Down;
                if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                  FaceIndex := ViewItem.ImageIndex.Up;
              end
              else if ViewItem = FMoveViewItem then begin
                FaceIndex := ViewItem.ImageIndex.Hot;
                if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                  FaceIndex := ViewItem.ImageIndex.Up;
              end
              else
                FaceIndex := ViewItem.ImageIndex.Up;
            end
            else begin
              if ViewItem = FMoveViewItem then begin
                if ViewItem.Checked then
                  FaceIndex := ViewItem.ImageIndex.Down
                else if ViewItem.ImageIndex.Hot >= 0 then
                  FaceIndex := ViewItem.ImageIndex.Hot
                else
                  FaceIndex := ViewItem.ImageIndex.Up;
              end
              else begin
                if ViewItem.Checked then
                  FaceIndex := ViewItem.ImageIndex.Down
                else
                  FaceIndex := ViewItem.ImageIndex.Up;
              end;
            end;

            if FaceIndex >= 0 then begin
              Texture := ViewItem.ImageIndex.Image.Images[FaceIndex];
              if Texture <> nil then begin
                PaintRect.Left := vtRect.Left + nLeft;
                PaintRect.Right := PaintRect.Left + Texture.Width;
                PaintRect.Top := vtRect.Top + nTop + (ItemHeight - Texture.Height) div 2;
                PaintRect.Bottom := PaintRect.Top + Texture.Height;
                DrawRect(PaintRect, vtRect, vbRect, Texture);
              end;
            end;
          end;

          if (ViewItem <> nil) and (ViewItem.Caption <> '') then begin
            if ViewItem.Style = bsButton then begin
              if ViewItem = FDownViewItem then begin
                Font := ViewItem.Color.Down;
              end
              else if ViewItem = FMoveViewItem then begin
                Font := ViewItem.Color.Hot;
              end
              else
                Font := ViewItem.Color.Up;
            end
            else begin
              if ViewItem = FMoveViewItem then begin
                if ViewItem.Checked then
                  Font := ViewItem.Color.Down
                else if ViewItem.ImageIndex.Hot >= 0 then
                  Font := ViewItem.Color.Hot
                else
                  Font := ViewItem.Color.Up;
              end
              else begin
                if ViewItem.Checked then
                  Font := ViewItem.Color.Down
                else
                  Font := ViewItem.Color.Up;
              end;
            end;

            HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);

            if (HGEFont <> nil) then begin
              if ViewItem.StringLineEx = nil then begin
                ViewItem.CaptionTexture := HGEFont.GetImageInfo(ViewItem.Caption);

                if Length(ViewItem.CaptionTexture.ImageIndexs) > 0 then begin
                  PaintRect := Rect(0, 0, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                  DestRect.Left := vtRect.Left + nLeft;
                  DestRect.Right := DestRect.Left + ViewItem.CaptionTexture.Width;
                  DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
                  DestRect.Bottom := DestRect.Top + ViewItem.CaptionTexture.Height;

                  PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
                  if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                    if (I = ItemIndex) and (DrawSelect) then begin
                      NewRect := PaintRect;
                      NewRect.Right := NewRect.Left + Width - ScrollSize;
                      OffsetRect(NewRect, nX, nY);
                      GameCanvas.FillRect(NewRect, clRed);
                    end;

                    if Font.Bold then begin
                      HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                    end
                    else begin
                      if not FontBackTransparent then
                        FillRect(DestRect, vtRect, vbRect, Font.BColor);
                      HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                    end;
                  end;
                end;
              end

                { TODO -ochongchong  -c新增 : 让绘制聊天框的文字支持自定义颜色 }
              else begin
                StringLineEx := TStringLineEx(ViewItem.StringLineEx);

                if StringLineEx <> nil then begin
                  nLeft := OffSetX;
                  for J := 0 to StringLineEx.Count - 1 do begin
                    StringToken := StringLineEx[J];

                    ViewItem.CaptionTexture := HGEFont.GetImageInfo(StringToken.Text);

                    if Length(ViewItem.CaptionTexture.ImageIndexs) > 0 then begin
                      PaintRect := Rect(0, 0, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                      DestRect.Left := vtRect.Left + nLeft;
                      DestRect.Right := DestRect.Left + ViewItem.CaptionTexture.Width;
                      DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
                      DestRect.Bottom := DestRect.Top + ViewItem.CaptionTexture.Height;

                      PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
                      if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                        if StringLineEx.FLineBackColor <> clNone then begin
                          NewRect := PaintRect;
                          NewRect.Right := NewRect.Left + Width - ScrollSize;
                          OffsetRect(NewRect, nX, nY);
                          FillRect(NewRect, StringLineEx.FLineBackColor);
                        end;

                        if (I = ItemIndex) and (DrawSelect) then begin
                          NewRect := PaintRect;
                          NewRect.Right := NewRect.Left + Width - ScrollSize;
                          OffsetRect(NewRect, nX, nY);
                          GameCanvas.FillRect(NewRect, clRed);
                        end;

                        if Font.Bold then begin
                          HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                          HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                          HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                          HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.BColor);
                          HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.FColor);
                        end
                        else begin
                          if not FontBackTransparent then begin
                            if FItemHeight > DestRect.Bottom - DestRect.Top then begin
                              Windows.InflateRect(DestRect, 0, (FItemHeight - (DestRect.Bottom - DestRect.Top)) div 2);
                            end;

                            //if DestRect.Bottom + 4 >= vtRect.Bottom then
                            //  DestRect.Bottom := DestRect.Bottom - 1;

                            FillRect(DestRect, vtRect, vbRect, StringToken.BColor);
                          end;

                          // 描边效果不好，暂时去掉

                          if (StringToken.TokenType = tt_Text) and ((StringToken.Flag <> 0) or FFontStroke) then begin
                            HGEFont.TextRect(nX - 1, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack); //StringToken.BColor
                            HGEFont.TextRect(nX + 1, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                            HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                            HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, clBlack);
                          end;

                          HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, StringToken.FColor);
                        end;
                      end;

                      nLeft := nLeft + ViewItem.CaptionTexture.Width;
                    end;
                  end;
                end;
              end;
            end;
          end;
          Inc(n01);
          Inc(nTop, ItemHeight);
          if n01 >= nMaxValue then break;
        end;
      end;
    finally
      UnLock;
    end;

    if ImageIndex.Image <> nil then begin
      if ImageIndex.Up >= 0 then begin
        Texture := ImageIndex.Image.Images[ImageIndex.Up];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - Texture.Width) div 2;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Top;
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;

    if FShowScroll then begin
      if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
        Texture := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
        if Texture <> nil then begin
          PaintRect := vtRect;
          // 修正滚动条对齐 chongchong 2015-11-02
          PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1; //vtRect.Left + (Width - Texture.Width);
          PaintRect.Right := PaintRect.Left + Texture.Width;
          GameCanvas.StretchDraw(PaintRect, Texture);
        end;
      end;

      if FPrevImageIndex.Image <> nil then begin
        nIndex := -1;
        if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then
          nIndex := FPrevImageIndex.Down
        else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then
          nIndex := FPrevImageIndex.Hot
        else if (FPrevImageIndex.Up >= 0) then
          nIndex := FPrevImageIndex.Up;
        if (nIndex >= 0) then begin
          Texture := FPrevImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            // 修正滚动条对齐 chongchong 2015-11-02
            //PaintRect.Top := vtRect.Top + 1;
            PaintRect.Top := vtRect.Top;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FNextImageIndex.Image <> nil then begin
        nIndex := -1;
        if FNextMouseDown and (FNextImageIndex.Down >= 0) then
          nIndex := FNextImageIndex.Down
        else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then
          nIndex := FNextImageIndex.Hot
        else if (FNextImageIndex.Up >= 0) then
          nIndex := FNextImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FNextImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;

            // 修正滚动条对齐 chongchong 2015-11-02
            //PaintRect.Top := vtRect.Top + (Height - Texture.Height - 1);
            PaintRect.Top := vtRect.Top + (Height - Texture.Height);

            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FBarImageIndex.Image <> nil then begin
        nIndex := -1;
        if FBarMouseDown and (FBarImageIndex.Down >= 0) then
          nIndex := FBarImageIndex.Down
        else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then
          nIndex := FBarImageIndex.Hot
        else if (FBarImageIndex.Up >= 0) then
          nIndex := FBarImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FBarImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + (FScrollSize - Texture.Width) div 2 + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + FPrevImageSize + FBarTop;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;
    end;
  end;

  if Assigned(OnStartSubPaint) then
    OnStartSubPaint(Self);

  for I := ControlCount - 1 downto 0 do
    if Control[I].Visible then
      Control[I].Paint;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
end;
{------------------------------------------------------------------------------}

{------------------------------------------------------------------------------}

constructor TDxListItem.Create;
begin
  inherited;
  FDown := False;
  FMove := False;
  FStyle := bsButton;
  FChecked := False;
  FItemList := nil;
  Owner := nil;
end;

destructor TDxListItem.Destroy;
var
  I:Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited;
end;

// -------------------------------------------------------------------------------

function TDxListItem.AddItem(const S:string; AObject:TObject):pTViewItem;
begin
  AddObject(S, AObject);
  Result := @FItemList[Length(FItemList) - 1];
end;

function TDxListItem.AddObject(const S:string; AObject:TObject):Integer;
var
  ViewItem:pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  ViewItem := @FItemList[Length(FItemList) - 1];
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  // ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.ImageIndex.OnGetImage := Owner.OnGetImage;

  ViewItem.Alignment := taCenter;
  Result := inherited AddObject(S, AObject);
end;

procedure TDxListItem.Clear;
var
  I:Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited Clear;
end;

procedure TDxListItem.Delete(Index:Integer);
var
  I:Integer;
begin
  FItemList[Index].Color.Free;
  FItemList[Index].ImageIndex.Free;
  for I := 0 to Length(FItemList) - 2 do begin
    FItemList[I] := FItemList[I + 1];
  end;
  SetLength(FItemList, Length(FItemList) - 1);
  inherited Delete(Index);
end;

procedure TDxListItem.InsertObject(Index:Integer; const S:string;
  AObject:TObject);
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  for I := Length(FItemList) - 1 downto Index do begin
    FItemList[I] := FItemList[I - 1];
  end;
  ViewItem := @FItemList[Index];
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  // ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.ImageIndex.OnGetImage := Owner.OnGetImage;
  ViewItem.Alignment := taCenter;
  inherited InsertObject(Index, S, AObject);
end;

function TDxListItem.GetMouseDown(Index:Integer):Boolean;
begin
  // Result := FItemList[Index].Down;
end;

function TDxListItem.GetMouseMove(Index:Integer):Boolean;
begin
  // Result := FItemList[Index].Move;
end;

function TDxListItem.GetChecked(Index:Integer):Boolean;
begin
  Result := FItemList[Index].Checked;
end;

procedure TDxListItem.SetChecked(Index:Integer; Value:Boolean);
var
  I:Integer;
begin
  if Value and (FItemList[Index].Checked <> Value) then begin
    case FStyle of
      bsButton:;
      bsRadio:begin
          for I := 0 to Length(FItemList) - 1 do begin
            FItemList[I].Checked := False;
          end;
          FItemList[Index].Checked := True;
        end;
      bsCheckBox:FItemList[Index].Checked := not FItemList[Index].Checked;
    end;
  end
  else
    FItemList[Index].Checked := Value;
end;

procedure TDxListItem.SetMouseDown(Index:Integer; Value:Boolean);
begin
  {if Value and (FItemList[Index].Down <> Value) then begin
    for I := 0 to Length(FItemList) - 1 do begin
      FItemList[I].Down := False;
    end;
  end;

  FItemList[Index].Down := Value;  }
end;

procedure TDxListItem.SetMouseMove(Index:Integer; Value:Boolean);
begin
  { if Value and (FItemList[Index].Move <> Value) then begin
     for I := 0 to Length(FItemList) - 1 do begin
       FItemList[I].Move := False;
     end;
   end;
   FItemList[Index].Move := Value; }
end;

function TDxListItem.GetItem(Index:Integer):pTViewItem;
begin
  Result := @FItemList[Index];
end;
{------------------------------------------------------------------------------}

constructor TViewField.Create();
begin
  FCaption := '';
  FAlignment := taCenter;
  FColor := TDxCaptionColor.Create;
end;

destructor TViewField.Destroy;
begin
  FColor.Free;
  inherited;
end;

procedure TViewField.Assign(Source:TPersistent);
begin
  if Source is TViewField then begin
    Caption := TViewField(Source).Caption;
    Alignment := TViewField(Source).Alignment;
    Color.Assign(TViewField(Source).Color);
    Changed;
  end;
end;

function TViewField.GetCaption:string;
begin
  Result := FCaption;
end;

procedure TViewField.SetCaption(Value:string);
begin
  FCaption := Value;
  Changed;
end;

procedure TViewField.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

{------------------------------------------------------------------------------}

constructor TDxListView.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  InitializeCriticalSection(FCriticalSection);
  FLines := TList.Create;
  inherited Create(AOwner);
  FTopIndex := 0;
  HeaderHeight := 36;
  ShowItemCount := (Height - HeaderHeight) div ItemHeight;
  ColCount := 3;

  FDownViewItem := nil;
  FMoveViewItem := nil;
  FDownListItem := nil;
  FMoveListItem := nil;

  FOnListItemMouseDown := nil;
  FOnListItemMouseMove := nil;

  FShowGridLine := False;
  FGridLineColor := $005894B8;
  FOnViewItemPaint := nil;
  FCheckItemControlSize := False;
  FMouseMoveFieldLine := False;
  FMouseDownFieldLine := False;
  FMouseX := 0;
  FMouseY := 0;

  FChangeFieldRects := nil;
  FSelectCol := 0;
  FCurrViewField := TViewField.Create;
  FCurrViewField.OnChange := ViewFieldChanged;
  FCurrViewField.Color.OnChange := ViewFieldChanged;

  FSelectControl := False;
end;

destructor TDxListView.Destroy;
var
  I:Integer;
begin
  for I := 0 to FLines.Count - 1 do
    TDxListItem(FLines[I]).Free;
  FLines.Free;

  for I := 0 to Length(FFields) - 1 do begin
    FFields[I].Free;
  end;

  FFields := nil;
  FCurrViewField.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited Destroy;
end;

function TDxListView.MaxValue:Integer;
begin
  Result := FLines.Count * ItemHeight;
  if VisibleItemCount > 0 then begin
    // if VisibleItemCount > 0 then begin
    Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
    {if Result > VisibleHeight then begin
      if (Result mod VisibleHeight) > VisibleItemCount * ItemHeight then begin
        Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end else begin
      if Result > VisibleItemCount * ItemHeight then begin
        Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end;}
  // end;
  end;
  Result := Result + ExpandSize;
end;

procedure TDxListView.Clear;
var
  I:Integer;
begin
  Lock;
  try
    FDownViewItem := nil;
    FMoveViewItem := nil;
    FDownListItem := nil;
    FMoveListItem := nil;
    FTopIndex := 0;
    for I := 0 to FLines.Count - 1 do
      TDxListItem(FLines[I]).Free;
    FLines.Clear;
    First;
    Position := 0;
  finally
    UnLock;
  end;
end;

procedure TDxListView.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDxListView.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

function TDxListView.GetCount:Integer;
begin
  Lock;
  try
    Result := FLines.Count;
  finally
    UnLock;
  end;
end;

procedure TDxListView.DoHide;
begin
  FDownListItem := nil;
  FDownViewItem := nil;
  FMoveListItem := nil;
  FMoveViewItem := nil;
end;

procedure TDxListView.DoClick(X, Y:Integer);
var
  I:Integer;
  nHeight:Integer;
  nLeft:Integer;
  vRect:TRect;
  ViewItem:pTViewItem;
  ListItem:TDxListItem;
begin
  FCol := -1;
  FRow := -1;
  if PointInRect(Point(X, Y), ShortRect(ViewRect, VisibleRect)) then begin
    vRect := VirtualRect;
    FCol := 0;
    nLeft := X - vRect.Left;
    for I := 0 to Length(FColRects) - 1 do begin
      if (nLeft >= FColRects[I].Left) and (nLeft < FColRects[I].Right) then begin
        FCol := I;
        break;
      end;
    end;
    // nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight + Position div ItemHeight;
    nHeight := Y - vRect.Top - HeaderHeight;
    if nHeight >= 0 then begin
      FRow := nHeight div ItemHeight + FTopIndex;
      if (FRow >= 0) and (FRow < Count) then begin
        ListItem := Items[FRow];
        if (FCol >= 0) and (FCol < ListItem.Count) then begin
          if FCheckItemControlSize then begin
            ViewItem := ListItem.Items[FCol];

            if SelectControl(X, Y,
              Bounds(
              FColRects[FCol].Left + vRect.Left,
              vRect.Top + (nHeight div ItemHeight) * ItemHeight + HeaderHeight,
              FColRects[FCol].Right - FColRects[FCol].Left,
              ItemHeight), ViewItem) then begin
              if FCanSelect then begin
                //ListItem.Selected := not ListItem.Selected;
              end;

              ListItem.ItemChecked[FCol] := True;
              case ViewItem.Style of
                bsRadio:begin
                    for I := 0 to ListItem.Count - 1 do begin
                      ListItem.Items[I].Checked := False;
                    end;
                    ViewItem.Checked := True;
                  end;
                bsCheckBox:begin
                    ViewItem.Checked := not ViewItem.Checked;
                  end;
              end;
              if Assigned(FOnListItemClick) then
                FOnListItemClick(Self, FRow, FCol, ListItem, ViewItem);
            end;
          end
          else begin
            if FCanSelect then begin
              //ListItem.Selected := not ListItem.Selected;
            end;

            ListItem.ItemChecked[FCol] := True;
            ViewItem := ListItem.Items[FCol];
            case ViewItem.Style of
              bsRadio:begin
                  for I := 0 to ListItem.Count - 1 do begin
                    ListItem.Items[I].Checked := False;
                  end;
                  ViewItem.Checked := True;
                end;
              bsCheckBox:begin
                  ViewItem.Checked := not ViewItem.Checked;
                end;
            end;
            if Assigned(FOnListItemClick) then
              FOnListItemClick(Self, FRow, FCol, ListItem, ViewItem);
          end;
        end;
      end;
    end;
  end;

  inherited;
end;

function TDxListView.GetViewRect:TRect;
begin
  Result := VirtualRect;
  Result.Top := Result.Top + FOffSetY;
  Result.Bottom := Result.Top + FShowItemCount * ItemHeight;

  if ShowScroll then
    Result.Right := Result.Right - FScrollSize;
  Result := ShortRect(Result, VirtualRect);
end;

procedure TDxListView.SetHeaderHeight(Value:Integer);
begin
  if FOffSetY <> Value then begin
    FOffSetY := Value;
  end;
end;

procedure TDxListView.DoResize(var NewRect:TRect);
begin
  inherited;
end;

procedure TDxListView.ChangeShowItemCount();
begin

end;

function TDxListView.CanMove:Boolean;
begin
  Result := False;
  if inherited CanMove then begin
    Result := (not ((FChangeFieldRects <> nil) and (FSelectCol >= 0)) and (not FSelectControl));
  end;
end;

function TDxListView.InRange(X, Y:Integer):Boolean;
var
  I:Integer;
  boInrange:Boolean;
  vRect:TRect;
  vtRect:TRect;
  vbRect:TRect;
  nLeft:Integer;
begin
  vtRect := VirtualRect;
  vbRect := VisibleRect;
  if PointInRect(Point(X, Y), vbRect) then begin

    boInrange := False;
    if ShowScroll then begin
      vRect := vtRect;
      vRect.Left := vRect.Right - FScrollSize;
      vRect.Right := vRect.Left + FScrollSize;
      vRect := ShortRect(vbRect, vRect);
      if (vRect.Left >= vRect.Right) or (vRect.Top >= vRect.Bottom) then begin
        Result := False;
        Exit;
      end;
      boInrange := PointInRect(Point(X, Y), vRect);
    end;

    if not boInrange then begin
      // boInrange := PointInRect(Point(X, Y), ShortRect(vtRect, vbRect));
      if Designing then begin
        if not boInrange then begin
          nLeft := X - vtRect.Left;
          for I := 0 to Length(FColRects) - 1 do begin
            if abs(nLeft - FColRects[I].Right) <= 1 then begin
              boInrange := True;
              break;
            end;
          end;
        end;
      end;
    end;

    if not boInrange then begin
      vtRect.Bottom := vtRect.Top + Min(ShowItemCount * ItemHeight + Self.OffSetY, vtRect.Bottom - vtRect.Top);
      boInrange := PointInRect(Point(X, Y), ShortRect(vtRect, vbRect));
    end;

    if Assigned(OnInRealArea) then begin
      OnInRealArea(Self, X - vtRect.Left, Y - vtRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

function TDxListView.SelectControl(X, Y:Integer; ColRect:TRect; ViewItem:pTViewItem):Boolean;
var
  PaintRect:TRect;
  Font:TDxFont;
  Texture:TTexture;
  FaceIndex:Integer;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  Result := False;
  if ViewItem <> nil then begin
    if ViewItem.ImageIndex.Image <> nil then begin
      if ViewItem.Style = bsButton then begin
        if ViewItem = FDownViewItem then begin
          FaceIndex := ViewItem.ImageIndex.Down;
          if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
            FaceIndex := ViewItem.ImageIndex.Up;
        end
        else if ViewItem = FMoveViewItem then begin
          FaceIndex := ViewItem.ImageIndex.Hot;
          if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
            FaceIndex := ViewItem.ImageIndex.Up;
        end
        else
          FaceIndex := ViewItem.ImageIndex.Up;
      end
      else begin
        if ViewItem = FMoveViewItem then begin
          if ViewItem.Checked then
            FaceIndex := ViewItem.ImageIndex.Down
          else if ViewItem.ImageIndex.Hot >= 0 then
            FaceIndex := ViewItem.ImageIndex.Hot
          else
            FaceIndex := ViewItem.ImageIndex.Up;
        end
        else begin
          if ViewItem.Checked then
            FaceIndex := ViewItem.ImageIndex.Down
          else
            FaceIndex := ViewItem.ImageIndex.Up;
        end;
      end;

      if FaceIndex >= 0 then begin
        Texture := ViewItem.ImageIndex.Image.Images[FaceIndex];
        if Texture <> nil then begin

          case ViewItem.Alignment of
            taLeftJustify:PaintRect := Bounds(ColRect.Left,
                ColRect.Top + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
            taRightJustify:PaintRect := Bounds(ColRect.Left + (ColRect.Right - ColRect.Left - Texture.Width),
                ColRect.Top + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
            taCenter:PaintRect := Bounds(ColRect.Left + (ColRect.Right - ColRect.Left - Texture.Width) div 2,
                ColRect.Top + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
          end;

          if PointInRect(Point(X, Y), PaintRect) then begin
            Result := True;
            Exit;
          end;
        end;
      end;
    end;

    if (ViewItem.Caption <> '') then begin
      if ViewItem = FDownViewItem then begin
        Font := ViewItem.Color.Down;
      end
      else if ViewItem = FMoveViewItem then begin
        Font := ViewItem.Color.Hot;
      end
      else begin
        if ViewItem.Checked then
          Font := ViewItem.Color.Checked
        else
          Font := ViewItem.Color.Up;
      end;

      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
      if HGEFont <> nil then begin
        ImageInfo := HGEFont.GetImageInfo(ViewItem.Caption);
        case ViewItem.Alignment of
          taLeftJustify:PaintRect := Bounds(ColRect.Left,
              ColRect.Top + (ItemHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
          taRightJustify:PaintRect := Bounds(ColRect.Left + (ColRect.Right - ColRect.Left - ImageInfo.Width),
              ColRect.Top + (ItemHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
          taCenter:PaintRect := Bounds(ColRect.Left + (ColRect.Right - ColRect.Left - ImageInfo.Width) div 2,
              ColRect.Top + (ItemHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
        end;

        if PointInRect(Point(X, Y), PaintRect) then begin
          Result := True;
        end;
      end;
    end;
  end;
end;

procedure TDxListView.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  I:Integer;
  nHeight:Integer;
  nLeft:Integer;
  vRect:TRect;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
begin
  FCol := -1;
  FRow := -1;
  FMouseDownCol := -1;
  if (not ScrollMouseDown(Button, X, Y)) then begin
    vRect := VirtualRect;
    FCol := 0;
    nLeft := X - vRect.Left;
    FMouseX := X;
    FMouseY := Y;

    FSelectCol := -1;
    FChangeFieldRects := nil;
    if Designing then begin
      if FMouseMoveFieldLine then begin
        for I := 0 to ColCount - 1 do begin
          if abs(nLeft - ColRects[I].Right) <= 1 then begin
            FMouseDownFieldLine := True;
            FSelectCol := I;
            FChangeFieldRects := @FColRects[I];
            break;
          end;
        end;
      end;
    end;

    if PointInRect(Point(X, Y), ShortRect(ViewRect, VisibleRect)) then begin
      for I := 0 to ColCount - 1 do begin
        if (nLeft >= ColRects[I].Left) and (nLeft < ColRects[I].Right) then begin
          FCol := I;
          FMouseDownCol := I;
          break;
        end;
      end;

      if Designing then begin
        if (FMouseDownCol >= 0) and (FMouseDownCol < Length(FFields)) then begin
          FCurrViewField.OnChange := nil;
          FCurrViewField.Color.OnChange := nil;
          FCurrViewField.Assign(FFields[FMouseDownCol]);
          FCurrViewField.OnChange := ViewFieldChanged;
          FCurrViewField.Color.OnChange := ViewFieldChanged;
        end;
      end;

      nHeight := Y - vRect.Top - HeaderHeight;
      if nHeight >= 0 then begin
        FRow := nHeight div ItemHeight + FTopIndex;
        if (FRow >= 0) and (FRow < FLines.Count) then begin
          ListItem := Items[FRow];
          if (ListItem <> nil) and (FCol >= 0) and (FCol < ListItem.Count) then begin
            if FCheckItemControlSize then begin
              ViewItem := ListItem.Items[FCol];
              if SelectControl(X, Y,
                Bounds(
                ColRects[FCol].Left + vRect.Left,
                vRect.Top + (nHeight div ItemHeight) * ItemHeight + HeaderHeight,
                ColRects[FCol].Right - ColRects[FCol].Left,
                ItemHeight), ViewItem) then begin

                if FDownListItem <> ListItem then
                  FDownListItem := ListItem;

                if (FDownViewItem <> ListItem.Items[FCol]) then
                  FDownViewItem := ListItem.Items[FCol];

                FSelectControl := True;

                if Assigned(FOnListItemMouseDown) then
                  FOnListItemMouseDown(Self, Button, Shift, X, Y, FRow, FCol, ListItem, ViewItem, True);
              end
              else begin
                if Assigned(FOnListItemMouseDown) then
                  FOnListItemMouseDown(Self, Button, Shift, X, Y, FRow, FCol, ListItem, ViewItem, False);
              end;
            end
            else begin
              if FDownListItem <> ListItem then begin
                FDownListItem := ListItem;
              end;

              if (FDownViewItem <> ListItem.Items[FCol]) then begin
                FDownViewItem := ListItem.Items[FCol];
              end;

              ViewItem := ListItem.Items[FCol];

              if Assigned(FOnListItemMouseDown) then
                FOnListItemMouseDown(Self, Button, Shift, X, Y, FRow, FCol, ListItem, ViewItem, True);
            end;
          end;
        end;
      end;
    end;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxListView.MouseLeave;
begin
  if Designing then begin
    Application.MainForm.Cursor := GetDefaultCursor;
  end;
end;

procedure TDxListView.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  I:Integer;
  nHeight:Integer;
  nLeft:Integer;
  vRect:TRect;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;

  nMoveSize:Integer;
  ColRect:TRect;
begin
  FCol := -1;
  FRow := -1;
  FMoveListItem := nil;
  FMoveViewItem := nil;
  if (not ScrollMouseMove(Shift, X, Y)) then begin
    vRect := VirtualRect;
    FCol := 0;
    nLeft := X - vRect.Left;
    if Designing then begin
      if not FMouseDownFieldLine then begin
        FMouseMoveFieldLine := False;
        Application.MainForm.Cursor := GetDefaultCursor;

        for I := 0 to Length(FColRects) - 1 do begin
          if abs(nLeft - FColRects[I].Right) <= 1 then begin
            FMouseMoveFieldLine := True;
            Application.MainForm.Cursor := crHSplit;
            break;
          end;
        end;
      end
      else begin
        nMoveSize := X - FMouseX;

        if (FChangeFieldRects <> nil) and (FSelectCol >= 0) then begin
          FChangeFieldRects.Right := FChangeFieldRects.Right + nMoveSize;
          for I := FSelectCol + 1 to ColCount - 1 do begin
            ColRect := ColRects[I];
            ColRect.Left := ColRect.Left + nMoveSize;
            ColRect.Right := ColRect.Right + nMoveSize;
            ColRects[I] := ColRect;
          end;
        end;
        FMouseX := X;
      end;
    end;

    if PointInRect(Point(X, Y), ShortRect(ViewRect, VisibleRect)) then begin

      for I := 0 to ColCount - 1 do begin
        ColRect := ColRects[I];
        if (nLeft >= ColRect.Left) and (nLeft < ColRect.Right) then begin
          FCol := I;
          break;
        end;
      end;

      nHeight := Y - vRect.Top - HeaderHeight;
      if nHeight >= 0 then begin
        FRow := nHeight div ItemHeight + FTopIndex;
        if (FRow >= 0) and (FRow < FLines.Count) then begin
          ListItem := Items[FRow];
          if (ListItem <> nil) and (FCol >= 0) and (FCol < ListItem.Count) then begin
            if FCheckItemControlSize then begin
              ViewItem := ListItem.Items[FCol];
              ColRect := ColRects[FCol];
              if SelectControl(X, Y,
                Bounds(
                vRect.Left + ColRect.Left,
                vRect.Top + (nHeight div ItemHeight) * ItemHeight + HeaderHeight,
                ColRect.Right - ColRect.Left,
                ItemHeight), ViewItem) then begin

                if FMoveListItem <> ListItem then
                  FMoveListItem := ListItem;

                if (FMoveViewItem <> ListItem.Items[FCol]) then
                  FMoveViewItem := ListItem.Items[FCol];

                if Assigned(FOnListItemMouseMove) then
                  FOnListItemMouseMove(Self, Shift, X, Y, FRow, FCol, ListItem, ViewItem, True);
              end
              else begin
                if Assigned(FOnListItemMouseMove) then
                  FOnListItemMouseMove(Self, Shift, X, Y, FRow, FCol, ListItem, ViewItem, False);
              end;
            end
            else begin
              ListItem := Items[FRow];
              if (FCol >= 0) and (FCol < ListItem.Count) then begin

                if FMoveListItem <> ListItem then
                  FMoveListItem := ListItem;

                if (FMoveViewItem <> ListItem.Items[FCol]) then
                  FMoveViewItem := ListItem.Items[FCol];

                ViewItem := ListItem.Items[FCol];

                if Assigned(FOnListItemMouseMove) then
                  FOnListItemMouseMove(Self, Shift, X, Y, FRow, FCol, ListItem, ViewItem, True);
              end;
            end;
          end;
        end;
      end;
    end;
  end
  else begin
    if Designing then begin
      Application.MainForm.Cursor := GetDefaultCursor;
    end;
  end;

  inherited MouseMove(Shift, X, Y);
end;

procedure TDxListView.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  InScroll:Boolean;

var
  I, nIndex:Integer;
  nHeight:Integer;
  nLeft:Integer;
  vRect:TRect;
  ListItem, Temp:TDxListItem;
begin
  if Assigned(PopupMenu) and (Button = mbRight) then begin
    PopupMenu.PopupMenu := Self;
    if RootCtrl <> nil then begin
      if X + PopupMenu.Width > RootCtrl.Width then
        PopupMenu.Left := X - PopupMenu.Width
      else
        PopupMenu.Left := X;
      if Y + PopupMenu.Height > RootCtrl.Height then
        PopupMenu.Top := Y - PopupMenu.Height
      else
        PopupMenu.Top := Y;
    end;
    PopupMenu.Show;
  end
  else begin
    FCol := -1;
    FRow := -1;
    if PointInRect(Point(X, Y), ShortRect(ViewRect, VisibleRect)) then begin
      vRect := VirtualRect;
      FCol := 0;
      nLeft := X - vRect.Left;
      for I := 0 to Length(FColRects) - 1 do begin
        if (nLeft >= FColRects[I].Left) and (nLeft < FColRects[I].Right) then begin
          FCol := I;
          break;
        end;
      end;
      // nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight + Position div ItemHeight;
      nHeight := Y - vRect.Top - HeaderHeight;
      if nHeight >= 0 then begin
        FRow := nHeight div ItemHeight + FTopIndex;
        if (FRow >= 0) and (FRow < Count) then begin
          ListItem := Items[FRow];
          if FCanSelect then begin
            if ssCtrl in Shift then begin
              ListItem.Selected := not ListItem.Selected;
            end
            else if ssShift in Shift then begin
              nIndex := -1;
              for I := 0 to Count - 1 do begin
                Temp := Items[I];
                if Temp.Selected then begin
                  if nIndex < 0 then begin
                    nIndex := I;
                  end;
                  Temp.Selected := False;
                end;
              end;

              if nIndex >= 0 then begin
                if nIndex > FRow then begin
                  for I := FRow to nIndex do begin
                    Temp := Items[I];
                    Temp.Selected := True;
                  end;
                end
                else if nIndex < FRow then begin
                  for I := nIndex to FRow do begin
                    Temp := Items[I];
                    Temp.Selected := True;
                  end;
                end;
              end;

              ListItem.Selected := True;
            end
            else begin
              for I := 0 to Count - 1 do begin
                Temp := Items[I];
                Temp.Selected := False;
              end;

              ListItem.Selected := True;
            end;
          end;
        end;
      end;
    end;

    InScroll := (FBarMouseDown or FPrevMouseDown or FNextMouseDown);
    ScrollMouseUp();
    FSelectCol := -1;
    FChangeFieldRects := nil;
    FMouseDownFieldLine := False;
    FMouseMoveFieldLine := False;
    FSelectControl := False;

    FDownListItem := nil;
    FDownViewItem := nil;

    if not InScroll then
      inherited MouseUp(Button, Shift, X, Y);
  end;
end;

procedure TDxListView.Paint;
var
  I, II, III, nIndex, nCount, nLeft, nTop, n01, nX, nY:Integer;
  FaceIndex:Integer;
  nHeight:Integer;
  nMaxValue:Integer;
  nBarTop:Integer;

  Texture:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  DestRect:TRect;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
  Font:TDxFont;

  ClipRect:TRect;
  ViewField:TViewField;
  PaintOverride:Boolean;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  { if Designing then begin
     // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
     FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
   end;}

  DoPaint();

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else begin
    if ImageIndex.Image <> nil then begin
      if ImageIndex.Up >= 0 then begin
        Texture := ImageIndex.Image.Images[ImageIndex.Up];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - Texture.Width) div 2;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Top + (Height - Texture.Height) div 2;
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;

    if HeaderHeight > 0 then begin // 画字段名称
      for I := 0 to Length(FFields) - 1 do begin
        nLeft := OffSetX + ColRects[I].Left;
        ViewField := FFields[I];

        if (ViewField <> nil) and (ViewField.Caption <> '') then begin
          HGEFont := TextureFonts.FindFont(ViewField.Color.Up.Name, ViewField.Color.Up.Size, ViewField.Color.Up.Style);
          if HGEFont <> nil then begin
            ImageInfo := HGEFont.GetImageInfo(ViewField.Caption);
            if Length(ImageInfo.ImageIndexs) > 0 then begin

              case ViewField.Alignment of
                taLeftJustify:DestRect := Bounds(vtRect.Left + nLeft,
                    vtRect.Top + (HeaderHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
                taRightJustify:DestRect := Bounds(vtRect.Left + nLeft + (ColRects[I].Right - ColRects[I].Left - ImageInfo.Width),
                    vtRect.Top + (HeaderHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
                taCenter:DestRect := Bounds(vtRect.Left + nLeft + (ColRects[I].Right - ColRects[I].Left - ImageInfo.Width) div 2,
                    vtRect.Top + (HeaderHeight - ImageInfo.Height) div 2, ImageInfo.Width, ImageInfo.Height);
              end;

              PaintRect := Rect(0, 0, ImageInfo.Width, ImageInfo.Height);

              PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
              if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                if ViewField.Color.Up.Bold then begin
                  HGEFont.TextRect(nX - 1, nY, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.BColor);
                  HGEFont.TextRect(nX + 1, nY, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.BColor);
                  HGEFont.TextRect(nX, nY - 1, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.BColor);
                  HGEFont.TextRect(nX, nY + 1, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.BColor);
                  HGEFont.TextRect(nX, nY, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.Color);
                end
                else begin
                  HGEFont.TextRect(nX, nY, PaintRect, ImageInfo.ImageIndexs, ViewField.Color.Up.Color);
                end;
              end;
            end;
          end;
        end;
        if Designing then begin
          CurrentFont.TextOut(vtRect.Left + nLeft + 1, vtRect.Top + 1, IntToStr(ColRects[I].Right - ColRects[I].Left));
        end;
      end;
    end;

    n01 := 0;
    nTop := HeaderHeight;
    // nIndex := Position div ItemHeight;
    // nMaxValue := Min((Height - HeaderHeight) div ItemHeight, ShowItemCount);
    Lock;
    try
      for I := FTopIndex to FLines.Count - 1 do begin
        ListItem := FLines[I];

        if ListItem.Selected then begin
          PaintRect := Bounds(vtRect.Left, vtRect.Top + nTop + 1, vtRect.Right - vtRect.Left, ItemHeight - 1);
          GameCanvas.FillRectAlpha(PaintRect, clWhite, 50);
        end;

        nCount := Min(ListItem.Count, ColCount);
        for II := 0 to nCount - 1 do begin
          ViewItem := ListItem.Items[II];
          nLeft := OffSetX + ColRects[II].Left;

          PaintOverride := False;

          if Assigned(FOnViewItemPaint) then
            FOnViewItemPaint(Self, II, I,
              Bounds(
              vtRect.Left + nLeft,
              vtRect.Top + nTop,
              ColRects[II].Right - ColRects[II].Left,
              ItemHeight), ViewItem, PaintOverride);

          if not PaintOverride then begin
            if ViewItem.ImageIndex.Image <> nil then begin
              if ViewItem.Style = bsButton then begin
                if ViewItem = FDownViewItem then begin
                  FaceIndex := ViewItem.ImageIndex.Down;
                  if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                    FaceIndex := ViewItem.ImageIndex.Up;
                end
                else if ViewItem = FMoveViewItem then begin
                  FaceIndex := ViewItem.ImageIndex.Hot;
                  if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                    FaceIndex := ViewItem.ImageIndex.Up;
                end
                else
                  FaceIndex := ViewItem.ImageIndex.Up;
              end
              else begin
                if ViewItem = FMoveViewItem then begin
                  if ViewItem.Checked then
                    FaceIndex := ViewItem.ImageIndex.Down
                  else if ViewItem.ImageIndex.Hot >= 0 then
                    FaceIndex := ViewItem.ImageIndex.Hot
                  else
                    FaceIndex := ViewItem.ImageIndex.Up;
                end
                else begin
                  if ViewItem.Checked then
                    FaceIndex := ViewItem.ImageIndex.Down
                  else
                    FaceIndex := ViewItem.ImageIndex.Up;
                end;
              end;

              if FaceIndex >= 0 then begin
                Texture := ViewItem.ImageIndex.Image.Images[FaceIndex];
                if Texture <> nil then begin
                  case ViewItem.Alignment of
                    taLeftJustify:PaintRect := Bounds(vtRect.Left + nLeft,
                        vtRect.Top + nTop + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
                    taRightJustify:PaintRect := Bounds(vtRect.Left + nLeft + (ColRects[II].Right - ColRects[II].Left - Texture.Width),
                        vtRect.Top + nTop + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
                    taCenter:PaintRect := Bounds(vtRect.Left + nLeft + (ColRects[II].Right - ColRects[II].Left - Texture.Width) div 2,
                        vtRect.Top + nTop + (ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
                  end;

                  DrawRect(PaintRect, vtRect, vbRect, Texture);
                end;
              end;
            end;

            // end else FOnViewItemPaint(Self, II, I, ColRects[II], ViewItem);

            if ViewItem.Caption <> '' then begin
              if ViewItem = FDownViewItem then begin
                Font := ViewItem.Color.Down;
              end
              else if ViewItem = FMoveViewItem then begin
                Font := ViewItem.Color.Hot;
              end
              else begin
                Font := ViewItem.Color.Up;
              end;
              HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
              if HGEFont <> nil then begin
                ViewItem.CaptionTexture := HGEFont.GetImageInfo(ViewItem.Caption);
                if Length(ViewItem.CaptionTexture.ImageIndexs) > 0 then begin
                  case ViewItem.Alignment of
                    taLeftJustify:DestRect := Bounds(vtRect.Left + nLeft,
                        vtRect.Top + nTop + (ItemHeight - ViewItem.CaptionTexture.Height) div 2, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                    taRightJustify:DestRect := Bounds(vtRect.Left + nLeft + (ColRects[II].Right - ColRects[II].Left - ViewItem.CaptionTexture.Width),
                        vtRect.Top + nTop + (ItemHeight - ViewItem.CaptionTexture.Height) div 2, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                    taCenter:DestRect := Bounds(vtRect.Left + nLeft + (ColRects[II].Right - ColRects[II].Left - ViewItem.CaptionTexture.Width) div 2,
                        vtRect.Top + nTop + (ItemHeight - ViewItem.CaptionTexture.Height) div 2, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                  end;
                  PaintRect := Rect(0, 0, ViewItem.CaptionTexture.Width, ViewItem.CaptionTexture.Height);
                  PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
                  if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                    if Font.Bold then begin
                      HGEFont.TextRect(nX - 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX + 1, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY - 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY + 1, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.BColor);
                      HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                    end
                    else begin
                      HGEFont.TextRect(nX, nY, PaintRect, ViewItem.CaptionTexture.ImageIndexs, Font.Color);
                    end;
                  end;
                end;
              end;
            end;
          end;
        end;
        Inc(n01);
        Inc(nTop, ItemHeight);
        if n01 >= ShowItemCount then break;
      end;
    finally
      UnLock;
    end;

    if FShowScroll then begin
      if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
        Texture := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
        if Texture <> nil then begin
          PaintRect := vtRect;
          PaintRect.Left := vtRect.Left + (Width - Texture.Width);
          PaintRect.Right := PaintRect.Left + Texture.Width;
          GameCanvas.StretchDraw(PaintRect, Texture);
        end;
      end;

      if FPrevImageIndex.Image <> nil then begin
        nIndex := -1;
        if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then
          nIndex := FPrevImageIndex.Down
        else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then
          nIndex := FPrevImageIndex.Hot
        else if (FPrevImageIndex.Up >= 0) then
          nIndex := FPrevImageIndex.Up;
        if (nIndex >= 0) then begin
          Texture := FPrevImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + 1;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FNextImageIndex.Image <> nil then begin
        nIndex := -1;
        if FNextMouseDown and (FNextImageIndex.Down >= 0) then
          nIndex := FNextImageIndex.Down
        else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then
          nIndex := FNextImageIndex.Hot
        else if (FNextImageIndex.Up >= 0) then
          nIndex := FNextImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FNextImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + (Height - Texture.Height - 1);
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FBarImageIndex.Image <> nil then begin
        nIndex := -1;
        if FBarMouseDown and (FBarImageIndex.Down >= 0) then
          nIndex := FBarImageIndex.Down
        else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then
          nIndex := FBarImageIndex.Hot
        else if (FBarImageIndex.Up >= 0) then
          nIndex := FBarImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FBarImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + (FScrollSize - Texture.Width) div 2 + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + FPrevImageSize + FBarTop;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;
    end;
  end;

  // if Designing then
    // GameCanvas.FrameRect(ViewRect, clWhite);

  if FShowGridLine then begin
    nY := vtRect.Bottom;
    for I := 0 to ColCount - 1 do begin // 竖线
      if I = 0 then begin
        nX := vtRect.Left + ColRects[I].Left;
        GameCanvas.Line(Point(nX, vtRect.Top), Point(nX, ViewRect.Bottom), FGridLineColor);
      end;
      nX := vtRect.Left + ColRects[I].Right;
      GameCanvas.Line(Point(nX, vtRect.Top), Point(nX, ViewRect.Bottom), FGridLineColor);
    end;

    // 横线

    nCount := (ViewRect.Bottom - ViewRect.Top) div ItemHeight;

    nY := vtRect.Top;
    GameCanvas.Line(Point(ViewRect.Left, nY), Point(ViewRect.Right, nY), FGridLineColor);

    for I := 0 to nCount - 1 do begin
      nY := ViewRect.Top + I * ItemHeight;
      GameCanvas.Line(Point(ViewRect.Left, nY), Point(ViewRect.Right, nY), FGridLineColor);
    end;

    nY := ViewRect.Bottom - 1;
    GameCanvas.Line(Point(ViewRect.Left, nY), Point(ViewRect.Right, nY), FGridLineColor);

  end;

  if Assigned(OnStartSubPaint) then
    OnStartSubPaint(Self);

  for I := ControlCount - 1 downto 0 do
    if Control[I].Visible then
      Control[I].Paint;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);

  // GameCanvas.ClipRect := ClipRect;
end;

function TDxListView.GetViewItem(Index:Integer):TDxListItem;
begin
  Lock;
  try
    if (Index >= 0) and (Index < FLines.Count) then
      Result := FLines[Index]
    else
      Result := nil;
  finally
    UnLock;
  end;
end;

function TDxListView.GetColRect(Index:Integer):TRect;
begin
  if (Index >= 0) and (Index < Length(FColRects)) then
    Result := FColRects[Index]
  else
    Result := Rect(0, 0, 0, 0);
end;

procedure TDxListView.SetColRect(Index:Integer; Value:TRect);
begin
  if (Index >= 0) and (Index < Length(FColRects)) then
    FColRects[Index] := Value;
end;

function TDxListView.GetField(Index:Integer):TViewField;
begin
  if (Index >= 0) and (Index < Length(FFields)) then
    Result := FFields[Index]
  else
    Result := nil;
end;

procedure TDxListView.SetField(Index:Integer; Value:TViewField);
begin
  if (Index >= 0) and (Index < Length(FFields)) then
    FFields[Index] := Value;
end;

procedure TDxListView.ViewFieldChanged(Sender:TObject);
begin
  if Designing and (FMouseDownCol >= 0) and (FMouseDownCol < Length(FFields)) then
    FFields[FMouseDownCol].Assign(FCurrViewField);
end;

function TDxListView.GetColCount:Integer;
begin
  Result := Length(FColRects);
end;

procedure TDxListView.SetColCount(Value:Integer);
var
  I, nWidth, nMaxWidth, nColCount:Integer;
begin
  // if FColCount <> Value then begin
  Lock;
  try
    nColCount := Max(Value, 1);
    nColCount := Min(nColCount, Width);

    if ShowScroll then
      nMaxWidth := (Width - FScrollSize)
    else
      nMaxWidth := Width;

    nWidth := Round(nMaxWidth / nColCount);

    if Length(FFields) > nColCount then begin
      for I := nColCount - 1 to Length(FFields) - 1 do begin
        FreeAndNiL(FFields[I]);
      end;
    end;

    SetLength(FFields, nColCount);
    SetLength(FColRects, nColCount);

    for I := 0 to Length(FColRects) - 1 do begin
      if I >= Length(FColRects) - 1 then
        FColRects[I] := Bounds(I * nWidth, 0, nWidth + (nMaxWidth - nWidth * Length(FColRects)) - 1, Height)
      else
        FColRects[I] := Bounds(I * nWidth, 0, nWidth, Height);
    end;

    for I := 0 to Length(FFields) - 1 do begin
      if FFields[I] = nil then
        FFields[I] := TViewField.Create;
    end;
  finally
    UnLock;
  end;
end;

procedure TDxListView.DoScroll(Value:Integer);
begin
  FTopIndex := (Position - Value) div ItemHeight;
end;

function TDxListView.Add:TDxListItem;
begin
  Result := TDxListItem.Create;
  Result.Owner := Self;
  FLines.Add(Result);
end;

procedure TDxListView.Delete(Index:Integer);
begin
  if (Index >= 0) and (Index <= FLines.Count - 1) then begin
    TDxListItem(FLines.Items[Index]).Free;
    FLines.Delete(Index);
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxTreeNode.Create();
begin
  FOwner := nil;
  FParent := nil;

  FCaption := '';

  FStyle := bsRadio;
  FChecked := False;
  FExpand := False;
  FLevel := 0;
  FIndex := 0;

  FClickSound := csNone;
  FOnClick := nil;
  FOnClickSound := nil;

  MouseDowned := False;
  MouseMoveed := False;

  FList := TList.Create;
  CaptionColor := TDxCaptionColor.Create;
end;

destructor TDxTreeNode.Destroy();
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do
    TDxTreeNode(FList.Items[I]).Free;
  FList.Free;
  CaptionColor.Free;
  inherited;
end;

procedure TDxTreeNode.Clear;
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do begin
    TDxTreeNode(FList.Items[I]).Free;
  end;
  FList.Clear;
  Owner.RefTreeNodeList();
end;

procedure TDxTreeNode.Add(Item:TDxTreeNode);
begin
  FList.Add(Item);
  Item.Owner := Owner;
  Item.Parent := Self;
  Item.Expand := True;
  if (Item.Parent = nil) or Item.Parent.Expand then
    Owner.RefTreeNodeList();
end;

procedure TDxTreeNode.Delete(Item:TDxTreeNode);
begin
  FList.Remove(Item);
end;

procedure TDxTreeNode.SetCaption(Value:string);
begin
  if FCaption <> Value then
    FCaption := Value;
end;

procedure TDxTreeNode.SetExpand(Value:Boolean);
var
  I:Integer;
  TreeNode:TDxTreeNode;
begin
  if FExpand <> Value then begin
    if FList.Count > 0 then
      FExpand := Value
    else
      FExpand := True;
    if not FExpand then begin
      for I := 0 to FList.Count - 1 do begin
        TreeNode := TDxTreeNode(FList.Items[I]);
        TreeNode.MouseDowned := False;
        TreeNode.MouseMoveed := False;
        // TreeNode.Expand := Value;
        TreeNode.Checked := Value;
      end;
    end;
    Owner.RefTreeNodeList();
  end;
end;

function TDxTreeNode.GetLevel:Integer;
var
  P:TDxTreeNode;
begin
  Result := 0;
  P := Parent;
  while True do begin
    if P = nil then break;
    P := P.Parent;
    Inc(Result);
  end;
end;

procedure TDxTreeNode.SetChecked(Value:Boolean);
begin
  if FChecked <> Value then begin
    FChecked := Value;
    {if Parent = nil then begin

    end else begin
      for I := 0 to FList.Count - 1 do begin
        TreeNode := TDxTreeNode(FList.Items[I]);
        TreeNode.Checked := Value;
      end;
    end;}
  end;
end;

function TDxTreeNode.GetItem(Index:Integer):TDxTreeNode;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TDxTreeNode.GetCount:Integer;
begin
  Result := FList.Count;
end;

function TDxTreeNode.IndexOf(Item:TDxTreeNode):Integer;
begin
  Result := FList.IndexOf(Item);
end;

function TDxTreeNode.ExpandCount:Integer;
var
  I:Integer;
  TreeNode:TDxTreeNode;
begin
  Result := 1;
  if Expand then begin
    Inc(Result, Count);
    for I := 0 to FList.Count - 1 do begin
      TreeNode := TDxTreeNode(FList.Items[I]);
      Inc(Result, TreeNode.ExpandCount);
    end;
  end;
end;

procedure TDxTreeNode.Add(TreeNodeList:TList);
var
  I:Integer;
  TreeNode:TDxTreeNode;
begin
  if Expand then begin
    TreeNodeList.Add(Self);
    for I := 0 to FList.Count - 1 do begin
      TreeNode := TDxTreeNode(FList.Items[I]);
      if TreeNode.Expand then
        TreeNode.Add(TreeNodeList)
      else
        TreeNodeList.Add(TreeNode);
    end;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxTreeView.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  InitializeCriticalSection(FCriticalSection);
  FList := TList.Create;
  TreeNodeList := TList.Create;

  inherited Create(AOwner);

  FOnSelect := nil;
  FShowButton := True;
  FRadioTreeNode := nil;
  FDownTreeNode := nil;
  FMoveTreeNode := nil;
  FStyle := bsRadio;

  FIsOverPaintExpandButton := False;
end;

destructor TDxTreeView.Destroy();
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do begin
    Items[I].Free;
  end;
  FList.Free;
  TreeNodeList.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited Destroy();
end;

procedure TDxTreeView.Clear;
var
  I:Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do begin
      Items[I].Free;
    end;
    FList.Clear;
    Position := 0;
    FRadioTreeNode := nil;
    FDownTreeNode := nil;
    FMoveTreeNode := nil;
  finally
    UnLock;
  end;
  RefTreeNodeList();
end;

procedure TDxTreeView.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDxTreeView.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

function TDxTreeView.GetItem(Index:Integer):TDxTreeNode;
begin
  Lock;
  try
    if (Index >= 0) and (Index < FList.Count) then
      Result := FList.Items[Index]
    else
      Result := nil;
  finally
    UnLock;
  end;
end;

function TDxTreeView.GetCount:Integer;
begin
  Lock;
  try
    Result := FList.Count;
  finally
    UnLock;
  end;
end;

procedure TDxTreeView.Add(Item:TDxTreeNode);
begin
  Lock;
  try
    FList.Add(Item);
    Item.Owner := Self;
    Item.Parent := nil;
    // Item.Expand := True;
  finally
    UnLock;
  end;
  RefTreeNodeList();
end;

procedure TDxTreeView.Delete(Item:TDxTreeNode);
begin
  Lock;
  try
    FList.Remove(Item);
    if Item = FDownTreeNode then
      FDownTreeNode := nil;
    if Item = FMoveTreeNode then
      FMoveTreeNode := nil;
    if Item = FRadioTreeNode then
      FRadioTreeNode := nil;
  finally
    UnLock;
  end;
  AutoSetShowScroll;
end;

function TDxTreeView.IndexOf(Item:TDxTreeNode):Integer;
var
  D, P:TDxTreeNode;
begin
  Lock;
  try
    if Item.Parent = nil then
      Result := FList.IndexOf(Item)
    else begin
      D := Item;
      P := D.Parent;

      Result := P.List.IndexOf(D) + 1;

      P := P.Parent;

      while True do begin
        if P.Parent = nil then begin
          Inc(Result, FList.IndexOf(P) + 1);
          break;
        end
        else begin
          Inc(Result, P.Count);
        end;
        P := P.Parent;
        if P = nil then break;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TDxTreeView.MaxValue:Integer;
begin
  Result := TreeNodeList.Count * ItemHeight + ItemHeight;
  if VisibleItemCount > 0 then begin
    Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
    {if Result > VisibleHeight then begin
      if (Result mod VisibleHeight) > VisibleItemCount * ItemHeight then begin
        Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end else begin
      if Result > VisibleItemCount * ItemHeight then begin
        Result := Result + (VisibleHeight - VisibleItemCount * ItemHeight);
      end;
    end;}
  end;
  Result := Result + ExpandSize;
end;

procedure TDxTreeView.RefTreeNodeList();
var
  I:Integer;
  TreeNode:TDxTreeNode;
begin
  Lock;
  try
    TreeNodeList.Clear;
    for I := 0 to FList.Count - 1 do begin
      TreeNode := Items[I];
      TreeNode.Add(TreeNodeList);
      if not TreeNode.Expand then
        TreeNodeList.Add(TreeNode);
    end;
  finally
    UnLock;
  end;

  AutoSetShowScroll;
end;

function TDxTreeView.Get(Index:Integer):TDxTreeNode;
begin
  Lock;
  try
    if (Index >= 0) and (Index < TreeNodeList.Count) then
      Result := TreeNodeList[Index]
    else
      Result := nil;
  finally
    UnLock;
  end;
end;

procedure TDxTreeView.DoScroll(Value:Integer);
begin

end;

function TDxTreeView.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vRect := VirtualRect;
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

procedure TDxTreeView.DoClick(X, Y:Integer);
var
  nLeft:Integer;
  nRow:Integer;
  vRect:TRect;
  TreeNode:TDxTreeNode;
begin
  vRect := VirtualRect;
  vRect.Left := vRect.Right - FScrollSize;
  vRect.Right := vRect.Left + FScrollSize;
  if PointInRect(Point(X, Y), vRect) then begin

  end
  else begin

    vRect := VirtualRect;
    nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
    TreeNode := Indexs[nRow];

    if TreeNode <> nil then begin
      nLeft := (TreeNode.Level + 1) * CurrentFont.TextWidth('0') * 2;
      vRect.Left := vRect.Left + nLeft;
      vRect.Right := vRect.Right - FScrollSize;
      if PointInRect(Point(X, Y), vRect) then begin

        TreeNode.Expand := not TreeNode.Expand;

        RefTreeNodeList();

        case FStyle of
          bsRadio:begin
              if FRadioTreeNode <> TreeNode then begin
                if FRadioTreeNode <> nil then
                  FRadioTreeNode.Checked := False;
                FRadioTreeNode := TreeNode;
              end;
              TreeNode.Checked := True;
            end;
          bsCheckBox:begin
              TreeNode.Checked := not TreeNode.Checked;
            end;
        end;

        if Assigned(FOnSelect) then
          FOnSelect(Self, TreeNode);
      end;
    end;
  end;
  inherited;
end;

procedure TDxTreeView.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  nLeft:Integer;
  nRow:Integer;
  vRect:TRect;
  TreeNode:TDxTreeNode;
begin
  if not ScrollMouseDown(Button, X, Y) then begin
    vRect := VirtualRect;
    RefTreeNodeList();
    nRow := (Y - vRect.Top + Position) div ItemHeight;
    TreeNode := Indexs[nRow];

    if TreeNode <> nil then begin
      nLeft := (TreeNode.Level + 1) * CurrentFont.TextWidth('0') * 2;
      vRect.Left := vRect.Left + nLeft;
      vRect.Right := vRect.Right - FScrollSize;
      if PointInRect(Point(X, Y), vRect) then begin
        if FDownTreeNode <> TreeNode then begin
          if FDownTreeNode <> nil then
            FDownTreeNode.MouseDowned := False;
          FDownTreeNode := TreeNode;
        end;
        TreeNode.MouseDowned := True;
      end;
    end;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxTreeView.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  nLeft:Integer;
  nRow:Integer;
  vRect:TRect;
  TreeNode:TDxTreeNode;
begin
  if not (ScrollMouseMove(Shift, X, Y) or FBarMouseDown or FPrevMouseDown or FNextMouseDown) then begin
    vRect := VirtualRect;

    nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
    TreeNode := Indexs[nRow];

    if TreeNode <> nil then begin
      nLeft := (TreeNode.Level + 1) * CurrentFont.TextWidth('0') * 2;
      vRect.Left := vRect.Left + nLeft;
      vRect.Right := vRect.Right - FScrollSize;
      if PointInRect(Point(X, Y), vRect) then begin
        if FMoveTreeNode <> TreeNode then begin
          if FMoveTreeNode <> nil then
            FMoveTreeNode.MouseMoveed := False;
          FMoveTreeNode := TreeNode;
        end;
        TreeNode.MouseMoveed := True;
      end;
    end;
  end;
  inherited MouseMove(Shift, X, Y);
end;

procedure TDxTreeView.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  nLeft:Integer;
  nRow:Integer;
  vRect:TRect;
  TreeNode:TDxTreeNode;
  InScroll:Boolean;
begin
  InScroll := (FBarMouseDown or FPrevMouseDown or FNextMouseDown);
  ScrollMouseUp();

  vRect := VirtualRect;
  // RefTreeNodeList();
  nRow := (Y - vRect.Top + Position) div ItemHeight;
  TreeNode := Indexs[nRow];

  if TreeNode <> nil then begin
    nLeft := (TreeNode.Level + 1) * CurrentFont.TextWidth('0') * 2;
    vRect.Left := vRect.Left + nLeft;
    vRect.Right := vRect.Right - FScrollSize;
    if PointInRect(Point(X, Y), vRect) then begin
      if FDownTreeNode <> nil then
        FDownTreeNode.MouseDowned := False;
    end;
  end;
  if not InScroll then
    inherited MouseUp(Button, Shift, X, Y);
end;

procedure TDxTreeView.AutoSetShowScroll;
begin
  if FAutoShowScroll then
    ShowScroll := TreeNodeList.Count > VisibleHeight div ItemHeight;
end;

procedure TDxTreeView.Paint;
var
  I, II, III, nIndex, nCount, nLeft, nTop:Integer;

  X1, Y1, X2, Y2, nHeight, nWidth, nX, nY:Integer;

  FaceIndex:Integer;

  nVisibleCount:Integer;
  nBarTop:Integer;

  Texture:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  DestRect:TRect;
  TreeNode:TDxTreeNode;
  Font:TDxFont;

  Pt1, Pt2, Pt3:TPoint;

  ClipRect:TRect;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if Designing then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  DoPaint();

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else begin
    if ImageIndex.Image <> nil then begin
      if ImageIndex.Up >= 0 then begin
        Texture := ImageIndex.Image.Images[ImageIndex.Up];
        if Texture <> nil then begin
          PaintRect.Left := vtRect.Left + (Width - Texture.Width) div 2;
          PaintRect.Right := PaintRect.Left + Texture.Width;
          PaintRect.Top := vtRect.Left;
          PaintRect.Bottom := PaintRect.Top + Texture.Height;
          DrawRect(PaintRect, vtRect, vbRect, Texture);
        end;
      end;
    end;

    nTop := FOffSetY;
    nIndex := Position div ItemHeight;
    nVisibleCount := VisibleHeight div ItemHeight;
    Lock;
    try
      nCount := Min(nIndex + nVisibleCount - 1, TreeNodeList.Count - 1);
      for I := nIndex to nCount do begin
        TreeNode := TreeNodeList[I];
        if FShowButton then begin
          if FIsOverPaintExpandButton then begin
            PaintRect := Bounds(vtRect.Left + FOffSetX + TreeNode.Level * 16, vtRect.Top + nTop, 10, 10);
            if Assigned(FOnPaintExpandButton) then
              FOnPaintExpandButton(Self, PaintRect, TreeNode.Expand);
          end
          else begin
            if TreeNode.Expand then begin // →
              PaintRect := Bounds(vtRect.Left + FOffSetX + TreeNode.Level * 16, vtRect.Top + (ItemHeight - 10) div 2 + nTop, 4, 10);
              Pt1 := Point(PaintRect.Left, PaintRect.Top);
              Pt2 := Point(PaintRect.Right, PaintRect.Top + (PaintRect.Bottom - PaintRect.Top) div 2);
              Pt3 := Point(PaintRect.Left, PaintRect.Bottom);
              GameCanvas.FillTri(Pt1, Pt2, Pt3, 4760808, 4760808, 4760808);
            end
            else begin // ↓
              PaintRect := Bounds(vtRect.Left + FOffSetX + TreeNode.Level * 16, vtRect.Top + (ItemHeight - 4) div 2 + nTop, 10, 4);
              Pt1 := Point(PaintRect.Left, PaintRect.Top);
              Pt2 := Point(PaintRect.Right, PaintRect.Top);
              Pt3 := Point(PaintRect.Left + (PaintRect.Right - PaintRect.Left) div 2, PaintRect.Bottom);
              GameCanvas.FillTri(Pt1, Pt2, Pt3, 4760808, 4760808, 4760808);
            end;
          end;
        end;

        if FShowButton then begin
          nLeft := FOffSetX + TreeNode.Level * 16 + 16;
        end
        else begin
          nLeft := FOffSetX + TreeNode.Level * 16;
        end;

        if TreeNode.Caption <> '' then begin
          if TreeNode.Checked then begin
            Font := TreeNode.CaptionColor.Down;
          end
          else begin
            if TreeNode.MouseDowned then begin
              Font := TreeNode.CaptionColor.Down;
            end
            else if TreeNode.MouseMoveed then begin
              Font := TreeNode.CaptionColor.Hot;
            end
            else begin
              Font := TreeNode.CaptionColor.Up;
            end;
          end;
          HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
          if HGEFont <> nil then begin
            ImageInfo := HGEFont.GetImageInfo(TreeNode.Caption);
            if Length(ImageInfo.ImageIndexs) > 0 then begin
              PaintRect := Rect(0, 0, ImageInfo.Width, ImageInfo.Height);
              DestRect.Left := vtRect.Left + nLeft;
              DestRect.Right := DestRect.Left + ImageInfo.Width;
              DestRect.Top := vtRect.Top + nTop + (ItemHeight - HGEFont.TextHeight('0')) div 2;
              DestRect.Bottom := DestRect.Top + ImageInfo.Height;

              PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
              if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
                if Font.Bold then begin
                  HGEFont.TextRect(nX - 1, nY, PaintRect, ImageInfo.ImageIndexs, Font.BColor);
                  HGEFont.TextRect(nX + 1, nY, PaintRect, ImageInfo.ImageIndexs, Font.BColor);
                  HGEFont.TextRect(nX, nY - 1, PaintRect, ImageInfo.ImageIndexs, Font.BColor);
                  HGEFont.TextRect(nX, nY + 1, PaintRect, ImageInfo.ImageIndexs, Font.BColor);
                  HGEFont.TextRect(nX, nY, PaintRect, ImageInfo.ImageIndexs, Font.Color);
                end
                else begin
                  HGEFont.TextRect(nX, nY, PaintRect, ImageInfo.ImageIndexs, Font.Color);
                end;
              end;
            end;
          end;
          Inc(nTop, ItemHeight);
        end;
      end;
    finally
      UnLock;
    end;

    if FShowScroll then begin
      if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
        Texture := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
        if Texture <> nil then begin
          PaintRect := vtRect;
          PaintRect.Left := vtRect.Left + (Width - Texture.Width);
          PaintRect.Right := PaintRect.Left + Texture.Width;
          GameCanvas.StretchDraw(PaintRect, Texture);
        end;
      end;

      if FPrevImageIndex.Image <> nil then begin
        nIndex := -1;
        if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then
          nIndex := FPrevImageIndex.Down
        else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then
          nIndex := FPrevImageIndex.Hot
        else if (FPrevImageIndex.Up >= 0) then
          nIndex := FPrevImageIndex.Up;
        if (nIndex >= 0) then begin
          Texture := FPrevImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + 1;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FNextImageIndex.Image <> nil then begin
        nIndex := -1;
        if FNextMouseDown and (FNextImageIndex.Down >= 0) then
          nIndex := FNextImageIndex.Down
        else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then
          nIndex := FNextImageIndex.Hot
        else if (FNextImageIndex.Up >= 0) then
          nIndex := FNextImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FNextImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + ((FScrollSize - Texture.Width) div 2) + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + (Height - Texture.Height - 1);
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;

      if FBarImageIndex.Image <> nil then begin
        nIndex := -1;
        if FBarMouseDown and (FBarImageIndex.Down >= 0) then
          nIndex := FBarImageIndex.Down
        else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then
          nIndex := FBarImageIndex.Hot
        else if (FBarImageIndex.Up >= 0) then
          nIndex := FBarImageIndex.Up;

        if nIndex >= 0 then begin
          Texture := FBarImageIndex.Image.Images[nIndex];
          if Texture <> nil then begin
            PaintRect.Left := vtRect.Left + (Width - FScrollSize) + (FScrollSize - Texture.Width) div 2 + 1;
            PaintRect.Right := PaintRect.Left + Texture.Width;
            PaintRect.Top := vtRect.Top + FPrevImageSize + FBarTop;
            PaintRect.Bottom := PaintRect.Top + Texture.Height;
            DrawRect(PaintRect, vtRect, vbRect, Texture);
          end;
        end;
      end;
    end;
  end;
  if Assigned(OnStartSubPaint) then
    OnStartSubPaint(Self);

  for I := ControlCount - 1 downto 0 do
    if Control[I].Visible then
      Control[I].Paint;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);

  // GameCanvas.ClipRect := ClipRect;
end;

{----------------------------------------------------------------------}

{ TStringLineEx }

constructor TStringLineEx.Create;
begin
  FTokens := TList.Create;
end;

destructor TStringLineEx.Destroy;
begin
  Clear;
  FTokens.Free;
  inherited;
end;

function TStringLineEx.Add(FColor, BColor:TColor; Flag:Integer;
  Text:string; TokenType:TTokenType):PStringToken;
begin
  New(Result);
  Result.FColor := FColor;
  Result.BColor := BColor;
  Result.Flag := Flag;
  Result.Text := Text;
  Result.TokenType := TokenType;
  FTokens.Add(Result);
end;

procedure TStringLineEx.Delete(Index:Integer);
begin
  if (Index >= 0) and (Index < Count) then begin
    Dispose(PStringToken(FTokens[Index]));
    FTokens.Delete(Index);
  end;
end;

procedure TStringLineEx.Clear;
var
  I:Integer;
begin
  for I := 0 to FTokens.Count - 1 do begin
    Dispose(PStringToken(FTokens[I]));
  end;
  FTokens.Clear;
end;

function TStringLineEx.GetCount:Integer;
begin
  Result := FTokens.Count;
end;

function TStringLineEx.GetTokens(Index:Integer):PStringToken;
begin
  if (Index >= 0) and (Index <= FTokens.Count - 1) then
    Result := FTokens[Index]
  else
    Result := nil;
end;

{----------------------------------------------------------------------}

end.
