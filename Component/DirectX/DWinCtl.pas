unit DWinCtl;

interface

uses
  Windows, Types, Messages, Classes, Graphics, SysUtils, StdCtrls, Controls, Forms, DxSurfaces, Grids,
  Menus, Clipbrd, HUtil32, GameImages, Share, Math;

var
  WINLEFT: Integer = 60;
  WINTOP: Integer = 60;

type
  TClickSound = (csNone, csStone, csGlass, csNorm);

  TDControl = class;

  TDPageControl = class;
  TDPopupMenu = class;
  TImageIndex = type Integer;

  TOnDirectPaint = procedure(Sender: TObject; dsurface: TDxSurface) of object;
  TOnKeyPress = procedure(Sender: TObject; var Key: Char) of object;
  TOnKeyDown = procedure(Sender: TObject; var Key: Word; Shift: TShiftState) of object;
  TOnMouseMove = procedure(Sender: TObject; Shift: TShiftState; X, Y: Integer) of object;
  TOnMouseDown = procedure(Sender: TObject; Button: TMouseButton; Shift: TShiftState; X, Y: Integer) of object;
  TOnMouseUp = procedure(Sender: TObject; Button: TMouseButton; Shift: TShiftState; X, Y: Integer) of object;
  TOnClick = procedure(Sender: TObject) of object;
  TOnClickEx = procedure(Sender: TObject; X, Y: Integer) of object;
  TOnInRealArea = procedure(Sender: TObject; X, Y: Integer; var IsRealArea: Boolean) of object;

  TOnGridSelect = procedure(Sender: TObject; ACol, ARow: Integer; Button: TMouseButton; Shift: TShiftState) of object;
  TOnGridMove = procedure(Sender: TObject; ACol, ARow: Integer; Shift: TShiftState) of object;

  TOnGridPaint = procedure(Sender: TObject; ACol, ARow: Integer; Rect: TRect; State: TGridDrawState; dsurface: TDxSurface) of object;
  TOnClickSound = procedure(Sender: TObject; Clicksound: TClickSound) of object;
  TOnScroll = procedure(Sender: TObject; Increment: Integer) of object;


  TButtonStyle = (bsButton, bsRadio, bsCheckBox);
  TInValue = (vInteger, vString);
  TMenuStyle = (sXP, sVista);

  TDxLines = class;
  TDxListItem = class;
  TDxTreeView = class;
  TDxTreeNode = class;
  TDxCaptionColor = class;
  TDxImageIndex = class;

  TViewItem = record
    Caption: string;
    ShowCaption: Boolean;
    Data: Pointer;
    Style: TButtonStyle;
    Checked: Boolean;
    Color: TDxCaptionColor;
    ImageIndex: TDxImageIndex;
    Down, Move: Boolean;
    Transparent: Boolean;
    Image: TGameImages;
    TimeTick: LongWord;
    Left: Integer;
    Top: Integer;
    Width: Integer;
    Height: Integer;
    Index: Integer;
    Text: string;
    OffsetX, OffsetY: Integer;
    Char: array[0..1024 - 1] of Char;
  end;
  pTViewItem = ^TViewItem;

  TOnListItem = procedure(Sender: TObject; ARow, ACol: Integer; ListItem: TDxListItem; ViewItem: pTViewItem) of object;


  TChatMemoEventEvent = procedure(Sender: TObject; S: string; Lines: TDxLines; ViewItem: pTViewItem) of object;

  TTreeNodeEventEvent = procedure(Sender: TObject; TreeNode: TDxTreeNode) of object;

  TLineColor = record
    FC, BC: TColor;
  end;
  pTLineColor = ^TLineColor;


  TColors = class(TGraphicsObject)
  private
    FDisabled: TColor;
    FBkgrnd: TColor;
    FSelected: TColor;
    FBorder: TColor;
    FFont: TColor;
    FHot: TColor;
    FDown: TColor;
    FLine: TColor;
    FUp: TColor;
  public
    constructor Create();
  published
    property Disabled: TColor read FDisabled write FDisabled;
    property Background: TColor read FBkgrnd write FBkgrnd;
    property Selected: TColor read FSelected write FSelected;
    property Border: TColor read FBorder write FBorder;
    property Font: TColor read FFont write FFont;
    property Up: TColor read FUp write FUp;
    property Hot: TColor read FHot write FHot;
    property Down: TColor read FDown write FDown;
    property Line: TColor read FLine write FLine;
  end;

  TDCustomControl = class(TCustomControl)
  private
    procedure CMDialogChar(var Message: TCMDialogChar); message CM_DIALOGCHAR;
    procedure CMTextChanged(var Message: TMessage); message CM_TEXTCHANGED;
    procedure CMCtl3DChanged(var Message: TMessage); message CM_CTL3DCHANGED;
    procedure WMSize(var Message: TMessage); message WM_SIZE;
  protected
    procedure AdjustClientRect(var Rect: TRect); override;
    procedure CreateParams(var Params: TCreateParams); override;
  public
    constructor Create(AOwner: TComponent); override;
  end;

  TDControl = class(TDCustomControl)
  private
    FIdx: Integer;
    FCaption: string; //0x1F0
    FDParent: TDControl; //0x1F4
    FEnableFocus: Boolean; //0x1F8
    FOnDirectPaint: TOnDirectPaint; //0x1FC
    FOnKeyPress: TOnKeyPress; //0x200
    FOnKeyDown: TOnKeyDown; //0x204
    FOnMouseMove: TOnMouseMove; //0x208
    FOnMouseDown: TOnMouseDown; //0x20C
    FOnMouseUp: TOnMouseUp; //0x210
    FOnDblClick: TNotifyEvent; //0x214
    FOnClick: TOnClickEx; //0x218
    FOnInRealArea: TOnInRealArea; //0x21C
    FOnBackgroundClick: TOnClick; //0x220
    FDowned: Boolean;
    FOnProcess: TNotifyEvent;

    FParentNotify: Boolean;
    FX, FY: Integer;
    FString: string;
    FIndex: Integer;
    FData: Pointer;
    FKeyPreview: Boolean;
    FFade: Boolean;
    FFadeAlpha: Integer;
    FFadeTick: Integer;
    procedure SetCaption(Str: string); //dynamic;
    procedure SetDParent(Value: TDControl);
    function GetMouseMove: Boolean;
    function GetClientRect: TRect;
    procedure CaptionChaged; dynamic;
    procedure SetDowned(Value: Boolean);
    function GetFirstDParent: TDControl;

    function GetVisible: Boolean;
    procedure SetVisible(Value: Boolean);

  protected
    FVisible: Boolean;
    FControlVisible: Boolean;
    procedure ReleaseFocus; dynamic;
    procedure DoShow; dynamic;
    //procedure CMVisibleChanged(var Message: TMessage); message CM_VISIBLECHANGED;
  public
    Background: Boolean; //0x24D
    DControls: TList; //0x250
      //FaceSurface: TDxSurface;
    WLib: TGameImages; //0x254
    FaceIndex: Integer; //0x258
    WantReturn: Boolean; //Background老锭, Click狼 荤侩 咯何..

    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Paint; override;
    procedure Loaded; override;
    procedure UnLoaded;

    function SurfaceX(X: Integer): Integer;
    function SurfaceY(Y: Integer): Integer;
    function LocalX(X: Integer): Integer;
    function LocalY(Y: Integer): Integer;
    procedure AddChild(dcon: TDControl);
    procedure ChangeChildOrder(dcon: TDControl);
    function InRange(X, Y: Integer): Boolean; dynamic;
    function KeyPress(var Key: Char): Boolean; dynamic;
    function KeyDown(var Key: Word; Shift: TShiftState): Boolean; dynamic;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; dynamic;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; dynamic;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; dynamic;
    function DblClick(X, Y: Integer): Boolean; dynamic;
    function Click(X, Y: Integer): Boolean; dynamic;
    function MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean; dynamic;
    function MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean; dynamic;

    function CanFocusMsg: Boolean;

    procedure SetImgIndex(Lib: TGameImages; Index: Integer); dynamic;
    procedure DirectPaint(dsurface: TDxSurface); dynamic;
    procedure Process; dynamic;
    procedure BringToFront;
    procedure SetFocus; override;

    property MouseMoveing: Boolean read GetMouseMove;

    property Idx: Integer read FIdx write FIdx;
    property TData: string read FString write FString;
    property Data: Pointer read FData write FData;
    property FirstDParent: TDControl read GetFirstDParent;
  published
    //property Visible;
    property OnProcess: TNotifyEvent read FOnProcess write FOnProcess;
    property OnDirectPaint: TOnDirectPaint read FOnDirectPaint write FOnDirectPaint;
    property OnKeyPress: TOnKeyPress read FOnKeyPress write FOnKeyPress;
    property OnKeyDown: TOnKeyDown read FOnKeyDown write FOnKeyDown;
    property OnMouseMove: TOnMouseMove read FOnMouseMove write FOnMouseMove;
    property OnMouseDown: TOnMouseDown read FOnMouseDown write FOnMouseDown;
    property OnMouseUp: TOnMouseUp read FOnMouseUp write FOnMouseUp;
    property OnDblClick: TNotifyEvent read FOnDblClick write FOnDblClick;
    property OnClick: TOnClickEx read FOnClick write FOnClick;
    property OnInRealArea: TOnInRealArea read FOnInRealArea write FOnInRealArea;
    property OnBackgroundClick: TOnClick read FOnBackgroundClick write FOnBackgroundClick;
    property Caption: string read FCaption write SetCaption;
    property DParent: TDControl read FDParent write SetDParent;
    property Visible: Boolean read GetVisible write SetVisible;
    property ControlVisible: Boolean read FControlVisible write FControlVisible;

    property Fade: Boolean read FFade write FFade;
    property EnableFocus: Boolean read FEnableFocus write FEnableFocus;
    property Downed: Boolean read FDowned write SetDowned;
    property ParentNotify: Boolean read FParentNotify write FParentNotify;
    property KeyPreview: Boolean read FKeyPreview write FKeyPreview;
    property ClientRect: TRect read GetClientRect;
    property PosX: Integer read FX write FX;
    property PosY: Integer read FY write FY;
    property Color;
    property Font;
    property Hint;
    property ShowHint;
    property Align;
  end;

  TDButton = class(TDControl)
  private
    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;
    FButtonStyle: TButtonStyle;
    FAlignment: TAlignment;
    FShowCaption: Boolean;
    FColors: TColors;
    procedure SetAlignment(Value: TAlignment);
  protected

  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure Process; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
  published
    property Alignment: TAlignment read FAlignment write SetAlignment;
    property ShowCaption: Boolean read FShowCaption write FShowCaption;
    property Colors: TColors read FColors write FColors;
    property ClickCount: TClickSound read FClickSound write FClickSound;
    property OnClick: TOnClickEx read FOnClick write FOnClick;
    property OnClickSound: TOnClickSound read FOnClickSound write FOnClickSound;
    property Style: TButtonStyle read FButtonStyle write FButtonStyle;
  end;

  TDGrid = class(TDControl)
  private
    FColCount, FRowCount: Integer;
    FColWidth, FRowHeight: Integer;
    FViewTopLine: Integer;
    SelectCell: TPoint;
    DownPos: TPoint;
    FOnGridSelect: TOnGridSelect;
    FOnGridMouseMove: TOnGridMove;
    FOnGridPaint: TOnGridPaint;
    function GetColRow(X, Y: Integer; var ACol, ARow: Integer): Boolean;
  public
    cx, cy: Integer;
    Col, row: Integer;
    constructor Create(AOwner: TComponent); override;
    function InRange(X, Y: Integer): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function Click(X, Y: Integer): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure ClearSelect;
  published
    property ColCount: Integer read FColCount write FColCount;
    property RowCount: Integer read FRowCount write FRowCount;
    property ColWidth: Integer read FColWidth write FColWidth;
    property RowHeight: Integer read FRowHeight write FRowHeight;
    property ViewTopLine: Integer read FViewTopLine write FViewTopLine;
    property OnGridSelect: TOnGridSelect read FOnGridSelect write FOnGridSelect;
    property OnGridMouseMove: TOnGridMove read FOnGridMouseMove write FOnGridMouseMove;
    property OnGridPaint: TOnGridPaint read FOnGridPaint write FOnGridPaint;
  end;

  TDWindow = class(TDButton)
  private
    FFloating: Boolean;
    SpotX, SpotY: Integer;

  protected
    procedure DoShow; override;
    //procedure SetVisible(flag: Boolean);
  public
    DialogResult: TModalResult;
    constructor Create(AOwner: TComponent); override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    procedure Show;
    function ShowModal: Integer;
    //procedure SetFocus; override;
  published
    //property Visible: Boolean read FVisible write SetVisible;
    property Floating: Boolean read FFloating write FFloating;

  end;

  TDTabSheet = class(TDWindow)
  private
    FPageControl: TDPageControl;
    procedure SetPageControl(Value: TDPageControl);
    procedure WMNCHitTest(var Message: TWMNCHitTest); message WM_NCHITTEST;
  protected
    procedure ReadState(Reader: TReader); override;
  public
    constructor Create(AOwner: TComponent); override;
    procedure DirectPaint(dsurface: TDxSurface); override;
  published
    property PageControl: TDPageControl read FPageControl write SetPageControl;
  end;



  TDMenuItem = class(TObject)
  private
    FVisible: Boolean;
    FEnabled: Boolean;
    FCaption: string;
    FMenu: TDPopupMenu;
    FChecked: Boolean;
  public
    Index: Integer;
    constructor Create();
    destructor Destroy; override;
    property Visible: Boolean read FVisible write FVisible;
    property Enabled: Boolean read FEnabled write FEnabled;
    property Caption: string read FCaption write FCaption;
    property Checked: Boolean read FChecked write FChecked;
    property Menu: TDPopupMenu read FMenu write FMenu;
  end;
  //TDMenuItems = array of TDMenuItem;

  TDPopupMenu = class(TDControl)
  private
    FItems: TStrings;
    FColors: TColors;
    FMoveItemIndex: Integer;
    FItemSize: Integer;
    FMouseMove: Boolean;
    FMouseDown: Boolean;

    FOwnerMenu: TDPopupMenu;
    FItemIndex: Integer;
    FOwnerItemIndex: TImageIndex;
    FActiveMenu: TDPopupMenu;
    FDControl: TDControl;

    FStyle: TMenuStyle;
    FAlpha: Byte;
    FDrawBorder: Boolean;
    function GetMenu(Index: Integer): TDPopupMenu;
    procedure SetMenu(Index: Integer; Value: TDPopupMenu);
    function GetItem(Index: Integer): TDMenuItem;
    function GetCount: Integer;
    procedure SetOwnerItemIndex(Value: TImageIndex);
    procedure SetOwnerMenu(Value: TDPopupMenu);
    procedure SetItems(Value: TStrings);
    function GetItems: TStrings;
    procedure SetColors(Value: TColors);

    procedure SetItemIndex(Value: Integer);
  protected
    procedure CreateWnd; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Paint; override;
    procedure Process; override;
    function InRange(X, Y: Integer): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    function KeyPress(var Key: Char): Boolean; override;
    function KeyDown(var Key: Word; Shift: TShiftState): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function Click(X, Y: Integer): Boolean; override;
    procedure Show; overload;
    procedure Show(d: TDControl); overload;
    procedure Hide;
    procedure Insert(Index: Integer; ACaption: string; Item: TDPopupMenu; nIndex: Integer = 0);
    procedure Delete(Index: Integer);
    procedure Clear;
    function Find(ACaption: string): TDPopupMenu;
    function IndexOf(Item: TDPopupMenu): Integer;
    procedure Add(ACaption: string; Item: TDPopupMenu; nIndex: Integer = 0);
    procedure Remove(Item: TDPopupMenu);
    property Count: Integer read GetCount;
    property Menus[Index: Integer]: TDPopupMenu read GetMenu write SetMenu;
    property Items[Index: Integer]: TDMenuItem read GetItem;
    property DControl: TDControl read FDControl write FDControl;
  published
    property OwnerMenu: TDPopupMenu read FOwnerMenu write SetOwnerMenu;
    property OwnerItemIndex: TImageIndex read FOwnerItemIndex write SetOwnerItemIndex default -1;
    property MenuItems: TStrings read GetItems write SetItems;
    property Colors: TColors read FColors write SetColors;
    property ItemIndex: Integer read FItemIndex write SetItemIndex default -1;
    property Style: TMenuStyle read FStyle write FStyle;
    property Alpha: Byte read FAlpha write FAlpha default 255;
    property DrawBorder: Boolean read FDrawBorder write FDrawBorder default True;
  end;

  //显示控件
  TDLabel = class(TDControl)
  private

    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;
    FAutoSize: Boolean;

    FCurrColor: TColor;
    FUpColor: TColor;
    FHotColor: TColor;
    FDownColor: TColor;

    FColor: TColor;

    FShadowSize: Integer;
    FShadowColor: Integer;
    FUseTick: longword;
    FBold: Boolean;

    FBackground: Boolean;
    FBackgroundColor: TColor;
    FBoldColor: TColor;
    FBorder: Boolean;
    FBorderColor: TColor;
    FHotBorder: TColor;
    FDownBorder: TColor;

    FButtonStyle: TButtonStyle;
    FClickTime: LongWord;

    FBorderCurrColor: TColor;

    FAlignment: TAlignment;
    procedure SetUpColor(Value: TColor);
    procedure CaptionChaged; override;
    procedure SetAlignment(Value: TAlignment);
    procedure SetAutoSize(Value: Boolean);
  public
    constructor Create(AOwner: TComponent); override;
    function InRange(X, Y: Integer): Boolean; override;

    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure Process; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    //property Style: TButtonStyle read FButtonStyle write FButtonStyle;
    property ClickTime: LongWord read FClickTime write FClickTime;
    property Canvas;
  published
    property Alignment: TAlignment read FAlignment write SetAlignment;


    property AutoSize: Boolean read FAutoSize write SetAutoSize;
    property Color: TColor read FColor write FColor;
    property UpColor: TColor read FUpColor write SetUpColor;
    property HotColor: TColor read FHotColor write FHotColor;
    property DownColor: TColor read FDownColor write FDownColor;
    property BackgroundColor: TColor read FBackgroundColor write FBackgroundColor;
    property BorderColor: TColor read FBorderColor write FBorderColor;

    property HotBorderColor: TColor read FHotBorder write FHotBorder;
    property DownBorderColor: TColor read FDownBorder write FDownBorder;
    property BoldColor: TColor read FBoldColor write FBoldColor;

    property UseTick: longword read FUseTick write FUseTick;
    property BoldFont: Boolean read FBold write FBold;
    property DrawBackground: Boolean read FBackground write FBackground;
    property Border: Boolean read FBorder write FBorder;

    property ClickCount: TClickSound read FClickSound write FClickSound;
    property OnClick: TOnClickEx read FOnClick write FOnClick;
    property OnClickSound: TOnClickSound read FOnClickSound write FOnClickSound;
    property Style: TButtonStyle read FButtonStyle write FButtonStyle;

  end;


  //输入控件
  TDEdit = class(TDControl)
  private
    Ticks: Integer;
    FText: WideString;
    FViewPos: Integer;
    FTextAdjust: Integer;
    FSelIndex: Integer;
    FBlinkTicks: Integer;
    FReadOnly: Boolean;
    FMaxLength: Integer;
    FTabOrder: Integer;
    bDoubleByte: Boolean;
    InputStr: string;
    KeyByteCount: Integer;

    FSelText: WideString;

    FBeginIndex: Integer;
    FEndIndex: Integer;
    FOnChange: TNotifyEvent;
    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;
    FMainMenu: TDPopupMenu;
    FDisabledColor: TColor;
    FBkgrndColor: TColor;
    FSelectedColor: TColor;
    FBorderColor: TColor;
    FSelTextColor: TColor;
    FFontColor: TColor;
    FSelTextFontColor: TColor;

    FHotBorder: TColor;
    FDownBorder: TColor;

    FPasswordText: string;
    FShowPasswordText: Boolean;
    FSelectText: Boolean;
    FPaste: Boolean;
    FDrawBkgrnd: Boolean;
    FDrawBorder: Boolean;
    FPasswordChar: Char;
    FInValue: TInValue;

    FBorderCurrColor: TColor;

    FAlignment: TAlignment;
    function GetText: WideString;
    function GetSelText: WideString;
    procedure SetViewPos(const Value: Integer);
    procedure SetText(const Value: WideString);
    procedure SetSelText(const Value: WideString);
    procedure SetSelIndex(const Value: Integer);
    procedure DrawText(dsurface: TDxSurface; const vRect: TRect);
    procedure DrawSelector(dsurface: TDxSurface; const vRect: TRect);


    function CharRect(Index: Integer): TRect;
    function NeedToScroll(): Boolean;
    procedure ScrollToRight(Index: Integer);
    procedure ScrollToLeft(Index: Integer);
    procedure SelectChar(const MousePos: TPoint);
    procedure StripWrong(var Text: string);
    procedure SetMaxLength(const Value: Integer);

    procedure SetSelLength(const Value: Integer);
    function GetSelLength: Integer;

    procedure EnterKey(var Key: Word);
  protected

  public
    constructor Create(AOwner: TComponent); override;
    function InRange(X, Y: Integer): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure Process; override;

    procedure Paint; override;

    function KeyPress(var Key: Char): Boolean; override;
    function KeyDown(var Key: Word; Shift: TShiftState): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    procedure SelectAll;
    procedure DeleteText;
    procedure CopyText;
    procedure CutText;
    procedure PasteText;
    property ViewPos: Integer read FViewPos write SetViewPos;
    property SelStart: Integer read FSelIndex write SetSelIndex;
    property SelLength: Integer read GetSelLength write SetSelLength;
  published
    property Text: WideString read FText write SetText;
    property SelText: WideString read FSelText write SetSelText;

    property TextAdjust: Integer read FTextAdjust write FTextAdjust;
    property SelIndex: Integer read FSelIndex write SetSelIndex;

    property BlinkTicks: Integer read FBlinkTicks write FBlinkTicks;
    property ReadOnly: Boolean read FReadOnly write FReadOnly;
    property MaxLength: Integer read FMaxLength write SetMaxLength;
    property DisabledColor: TColor read FDisabledColor write FDisabledColor;
    property BkgrndColor: TColor read FBkgrndColor write FBkgrndColor;
    property SelectedColor: TColor read FSelectedColor write FSelectedColor;
    property BorderColor: TColor read FBorderColor write FBorderColor;
    property SelTextColor: TColor read FSelTextColor write FSelTextColor;
    property FontColor: TColor read FFontColor write FFontColor;
    property SelTextFontColor: TColor read FSelTextFontColor write FSelTextFontColor;
    property HotBorderColor: TColor read FHotBorder write FHotBorder;
    property DownBorderColor: TColor read FDownBorder write FDownBorder;

    property PasswordChar: Char read FPasswordChar write FPasswordChar;
    property DrawBackground: Boolean read FDrawBkgrnd write FDrawBkgrnd;
    property DrawBorder: Boolean read FDrawBorder write FDrawBorder;
    property AllowSelectText: Boolean read FSelectText write FSelectText;
    property Paste: Boolean read FPaste write FPaste;
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property MainMenu: TDPopupMenu read FMainMenu write FMainMenu;
    property InValue: TInValue read FInValue write FInValue;
  end;

  TDCombobox = class(TDControl)
  private
    FItems: TStrings;
    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;
    FOnChange: TNotifyEvent;
    FOnPopup: TNotifyEvent;
    FAutoSize: Boolean;

    FCurrColor: TColor;
    FUpColor: TColor;
    FHotColor: TColor;
    FDownColor: TColor;

    FButtonColor: TColor;

    FBackground: Boolean;
    FBackgroundColor: TColor;
    FBorder: Boolean;
    FBorderColor: TColor;
    FHotBorder: TColor;
    FDownBorder: TColor;
    FMainMenu: TDPopupMenu;

    FBorderCurrColor: TColor;

    FAlignment: TAlignment;

    procedure CaptionChaged; override;
    procedure SetUpColor(Value: TColor);
    procedure SetItems(Value: TStrings);
    function GetItems: TStrings;

    procedure SetItemIndex(Value: Integer);
    function GetItemIndex: Integer;
    procedure MainMenuClick(Sender: TObject; X, Y: Integer);
    procedure SetAlignment(Value: TAlignment);
    procedure ItemChanged(Sender: TObject);
  protected
    procedure CreateWnd; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    function InRange(X, Y: Integer): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure Process; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
  published
    property Alignment: TAlignment read FAlignment write SetAlignment;
    property Items: TStrings read GetItems write SetItems;
    property ItemIndex: Integer read GetItemIndex write SetItemIndex;

    property AutoSize: Boolean read FAutoSize write FAutoSize;
    property UpColor: TColor read FUpColor write SetUpColor;
    property HotColor: TColor read FHotColor write FHotColor;
    property DownColor: TColor read FDownColor write FDownColor;
    property BackgroundColor: TColor read FBackgroundColor write FBackgroundColor;
    property BorderColor: TColor read FBorderColor write FBorderColor;

    property HotBorderColor: TColor read FHotBorder write FHotBorder;
    property DownBorderColor: TColor read FDownBorder write FDownBorder;

    property DrawBackground: Boolean read FBackground write FBackground;
    property Border: Boolean read FBorder write FBorder;
    property ButtonColor: TColor read FButtonColor write FButtonColor;

    property ClickCount: TClickSound read FClickSound write FClickSound;
    property OnClick: TOnClickEx read FOnClick write FOnClick;
    property OnClickSound: TOnClickSound read FOnClickSound write FOnClickSound;
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property OnPopup: TNotifyEvent read FOnPopup write FOnPopup;
    property MainMenu: TDPopupMenu read FMainMenu write FMainMenu;
    property Text: string read FCaption write FCaption;
  end;

  TDCheckBox = class(TDButton)
  private
    FUpColor: TColor;
    FHotColor: TColor;
    FDownColor: TColor;
    procedure SetAlignment(Value: TAlignment);
    procedure CaptionChaged; override;
  public
    constructor Create(AOwner: TComponent); override;
    function InRange(X, Y: Integer): Boolean; override;
    procedure SetImgIndex(Lib: TGameImages; Index: Integer); override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure Process; override;
  published
    property Alignment: TAlignment read FAlignment write SetAlignment;
    property Checked: Boolean read FDowned write FDowned;
    property UpColor: TColor read FUpColor write FUpColor;
    property HotColor: TColor read FHotColor write FHotColor;
    property DownColor: TColor read FDownColor write FDownColor;
  end;


  TDxFont = class(TPersistent)

  private
    FOnChange: TNotifyEvent;
    FColor: TColor;
    FBColor: TColor;
    FName: TFontName;
    FStyle: TFontStyles;
    FSize: Integer;
    FBold: Boolean;
    procedure SetColor(Value: TColor);
    procedure SetBColor(Value: TColor);
    procedure SetName(Value: TFontName);
    procedure SetSize(Value: Integer);
    procedure SetStyle(Value: TFontStyles);
    procedure SetBold(Value: Boolean);
  protected
    procedure Changed; //dynamic;
  public
    constructor Create;
    procedure Assign(Source: TPersistent); override;

  published
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property Color: TColor read FColor write SetColor;
    property BColor: TColor read FBColor write SetBColor;
    property Name: TFontName read FName write SetName;
    property Size: Integer read FSize write SetSize;
    property Style: TFontStyles read FStyle write SetStyle;
    property Bold: Boolean read FBold write SetBold;
  end;

  TDxCaptionColor = class(TPersistent)

  private
    FOnChange: TNotifyEvent;
    FUp: TDxFont;
    FHot: TDxFont;
    FDown: TDxFont;
    FDisabled: TDxFont;
    procedure SetUp(Value: TDxFont);
    procedure SetHot(Value: TDxFont);
    procedure SetDown(Value: TDxFont);
    procedure SetDisabled(Value: TDxFont);
    procedure FontChange(Sender: TObject);
  protected

  public
    constructor Create;
    destructor Destroy; override;
    procedure Assign(Source: TPersistent); override;
  published
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property Up: TDxFont read FUp write SetUp;
    property Hot: TDxFont read FHot write SetHot;
    property Down: TDxFont read FDown write SetDown;
    property Disabled: TDxFont read FDisabled write SetDisabled;
  end;


  TDxLines = class(TStringList)

  private
    FWidth, FHeight: Integer;
    FItemList: array of TViewItem;
    FOwner: TObject;

    function GetItem(Index: Integer): pTViewItem;
  protected

  public
    TimeTick: LongWord;
    constructor Create(AOwner: TObject);
    destructor Destroy; override;

    function AddObject(const S: string; AObject: TObject): Integer; override;
    procedure Clear; override;
    procedure Delete(Index: Integer); override;
    procedure InsertObject(Index: Integer; const S: string;
      AObject: TObject); override;

    function AddItem(const S: string; AObject: TObject): pTViewItem;

    property Items[Index: Integer]: pTViewItem read GetItem;
  published
    property Width: Integer read FWidth write FWidth;
    property Height: Integer read FHeight write FHeight;
    property Owner: TObject read FOwner write FOwner;
  end;


  TDxList = class(TStringList)
  public
    constructor Create;
    destructor Destroy; override;

    procedure Clear; override;
    procedure Delete(Index: Integer); override;
  end;




  TDPageControl = class(TDWindow)
  private

    FActivePage: Integer;
    FTabRect: TRect;

    procedure SetActivePage(Value: Integer);

    procedure SetTabLeft(Value: Integer);
    function GetTabLeft: Integer;

    function GetTabTop: Integer;
    procedure SetTabTop(Value: Integer);

    function GetTabWidth: Integer;
    procedure SetTabWidth(Value: Integer);

    function GetTabHeight: Integer;
    procedure SetTabHeight(Value: Integer);
  protected
    procedure ShowControl(AControl: TControl); override;
    procedure ReadState(Reader: TReader); override;
    procedure WMSize(var Message: TWMSize); message WM_SIZE;
  public
    Tabs: TList;
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Add(D: TDControl);
    procedure Delete(D: TDControl);
  published
    property ActivePage: Integer read FActivePage write SetActivePage;
    property TabLeft: Integer read GetTabLeft write SetTabLeft;
    property TabTop: Integer read GetTabTop write SetTabTop;
    property TabWidth: Integer read GetTabWidth write SetTabWidth;
    property TabHeight: Integer read GetTabHeight write SetTabHeight;
  end;

  TDxImageIndex = class(TPersistent)
  private
    FOnChange: TNotifyEvent;
    FImage: TGameImages;
    FUp: Integer;
    FHot: Integer;
    FDown: Integer;
    FDisabled: Integer;
    procedure SetUp(Value: Integer);
    procedure SetHot(Value: Integer);
    procedure SetDown(Value: Integer);
    procedure SetDisabled(Value: Integer);

    procedure SetImage(Value: TGameImages);
  protected
    procedure Changed; //dynamic;
  public
    constructor Create;
    procedure Assign(Source: TPersistent); override;
  published
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property Image: TGameImages read FImage write SetImage;
    property Up: Integer read FUp write SetUp;
    property Hot: Integer read FHot write SetHot;
    property Down: Integer read FDown write SetDown;
    property Disabled: Integer read FDisabled write SetDisabled;
  end;

  TDMemo = class(TDButton)
  private
    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;

    FBackground: Boolean;
    FBorder: Boolean;

    FMainMenu: TDPopupMenu;

    FColors: TColors;

    FShowScroll: Boolean;
    FScrollBars: TScrollStyle;
    FScrollSize: Integer;

    FItemHeight: Integer;
    FItemIndex: Integer;

    FMaxValue, FPosition: Integer;
    FRemoveSize: Integer;


    FImageIndex: TDxImageIndex;
    FScrollImageIndex: TDxImageIndex;
    FPrevImageIndex: TDxImageIndex;
    FNextImageIndex: TDxImageIndex;
    FBarImageIndex: TDxImageIndex;


    FPrevImageSize: Integer;
    FNextImageSize: Integer;
    FBarImageSize: Integer;

    FPrevMouseDown: Boolean;
    FNextMouseDown: Boolean;
    FBarMouseDown: Boolean;

    FPrevMouseMove: Boolean;
    FNextMouseMove: Boolean;
    FBarMouseMove: Boolean;

    FOnScroll: TNotifyEvent;

    FBarTop: Integer;

    FOffSetX, FOffSetY, FBottomHeight: Integer;
    FDrawLineCount: Integer;
    FMouseMoveTick: LongWord;

    function InPrevRange(X, Y: Integer; vRect: TRect): Boolean;
    function InNextRange(X, Y: Integer; vRect: TRect): Boolean;
    function InBarRange(X, Y: Integer; vRect: TRect): Boolean;

    procedure SetMaxValue(Value: Integer);
    procedure SetPosition(Value: Integer);
    procedure SetRemoveSize(Value: Integer);

    procedure SetItemIndex(Value: Integer);
    procedure SetItemHeight(Value: Integer);

    procedure SetScrollBars(Value: TScrollStyle);
    procedure SetScrollSize(Value: Integer);

    procedure SetColors(Value: TColors);

    procedure ScrollImageIndexChange(Sender: TObject);
    procedure PrevImageIndexChange(Sender: TObject);
    procedure NextImageIndexChange(Sender: TObject);
    procedure BarImageIndexChange(Sender: TObject);

    procedure SetImageIndex(Value: TDxImageIndex);
    procedure SetScrollImageIndex(Value: TDxImageIndex);
    procedure SetPrevImageIndex(Value: TDxImageIndex);
    procedure SetNextImageIndex(Value: TDxImageIndex);
    procedure SetBarImageIndex(Value: TDxImageIndex);

    procedure SetBarPosition;
  protected
    procedure DoScroll(Value: Integer); virtual;
    procedure DoPostion(var ALeft, ATop: Integer); virtual;
    procedure DoSize(var AWidth, AHeight: Integer); virtual;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    procedure Next;
    procedure Previous;
    procedure First;
    procedure Last;

    procedure RefPosinton;
    procedure SetBounds(ALeft, ATop, AWidth, AHeight: Integer); override;

    function InRange(X, Y: Integer): Boolean; override;
    procedure Process; override;
    procedure DirectPaint(dsurface: TDxSurface); override;

    function KeyDown(var Key: Word; Shift: TShiftState): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean; override;
    function MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean; override;
  published
    property OnScroll: TNotifyEvent read FOnScroll write FOnScroll;
    property Colors: TColors read FColors write SetColors;

    property ShowScroll: Boolean read FShowScroll write FShowScroll;
    property ItemHeight: Integer read FItemHeight write SetItemHeight;
    property ItemIndex: Integer read FItemIndex write SetItemIndex;
    property ScrollBars: TScrollStyle read FScrollBars write SetScrollBars;
    property ScrollSize: Integer read FScrollSize write SetScrollSize;

    property ImageIndex: TDxImageIndex read FImageIndex write SetImageIndex;
    property ScrollImageIndex: TDxImageIndex read FScrollImageIndex write SetScrollImageIndex;
    property PrevImageIndex: TDxImageIndex read FPrevImageIndex write SetPrevImageIndex;
    property NextImageIndex: TDxImageIndex read FNextImageIndex write SetNextImageIndex;
    property BarImageIndex: TDxImageIndex read FBarImageIndex write SetBarImageIndex;

    property MaxValue: Integer read FMaxValue write SetMaxValue;
    property Position: Integer read FPosition write SetPosition;
    property RemoveSize: Integer read FRemoveSize write SetRemoveSize;
    property OffSetX: Integer read FOffSetX write FOffSetX;
    property OffSetY: Integer read FOffSetY write FOffSetY;
    property BottomHeight: Integer read FBottomHeight write FBottomHeight;

    property DrawLineCount: Integer read FDrawLineCount write FDrawLineCount; //显示列表行数   TDxListView有效

    property DrawBackground: Boolean read FBackground write FBackground;
    property Border: Boolean read FBorder write FBorder;

    property ClickCount: TClickSound read FClickSound write FClickSound;
    property OnClick: TOnClickEx read FOnClick write FOnClick;
    property OnClickSound: TOnClickSound read FOnClickSound write FOnClickSound;
    property MainMenu: TDPopupMenu read FMainMenu write FMainMenu;
    property OnMouseWheel;
    property OnMouseWheelDown;
    property OnMouseWheelUp;
  end;



  TDxChatMemo = class(TDMemo) //聊天框专用
  private
    FOnChange: TNotifyEvent;
    FStrings: TStrings;
    FTopStrings: TStrings;
    FAutoScroll: Boolean;
    FTopIndex: Integer;
    FFontBackTransparent: Boolean;
    FTopCount: Integer;
    FOffsetHeight: Integer;

    FOnItemClick: TChatMemoEventEvent;
    FOnItemMouseDown: TChatMemoEventEvent;
    FOnItemMouseMove: TChatMemoEventEvent;
    FOnItemMouseUp: TChatMemoEventEvent;

    procedure SetStrings(Value: TStrings);
    procedure SetTopStrings(Value: TStrings);
    function GetTopCount: Integer;
    function GetClientWidth: Integer;
    function GetClientHeight: Integer;
    procedure InitButton;
  protected
    procedure DoScroll(Value: Integer); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    function Click(X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;

    function KeyDown(var Key: Word; Shift: TShiftState): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    procedure LoadFromFile(const FileName: string);


    function Insert(Index: Integer; const S: string; FC, BC: TColor): TDxLines;
    procedure Delete(Index: Integer);



    function InsertTop(Index: Integer; const S: string; FC, BC: TColor; TimeOut: Integer): TDxLines;
    procedure DeleteTop(Index: Integer);

    function Add(const S: string; FC, BC: TColor): TDxLines; overload;
    function Add(const S: string; FC, BC: TColor; ALines: TDxLines): TDxLines; overload;

    function AddTop(const S: string; FC, BC: TColor; TimeOut: Integer): TDxLines; overload;
    function AddTop(const S: string; FC, BC: TColor; TimeOut: Integer; ALines: TDxLines): TDxLines; overload;
    procedure Clear;

    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property FontBackTransparent: Boolean read FFontBackTransparent write FFontBackTransparent;
  published
    property TopCount: Integer read GetTopCount write FTopCount;
    property TopIndex: Integer read FTopIndex write FTopIndex;
    property Strings: TStrings read FStrings write SetTopStrings;
    property TopStrings: TStrings read FTopStrings write SetStrings;
    property AutoScroll: Boolean read FAutoScroll write FAutoScroll;
    property ClientWidth: Integer read GetClientWidth;
    property ClientHeight: Integer read GetClientHeight;
    property OnItemClick: TChatMemoEventEvent read FOnItemClick write FOnItemClick;
    property OnItemMouseDown: TChatMemoEventEvent read FOnItemMouseDown write FOnItemMouseDown;
    property OnItemMouseMove: TChatMemoEventEvent read FOnItemMouseMove write FOnItemMouseMove;
    property OnItemMouseUp: TChatMemoEventEvent read FOnItemMouseUp write FOnItemMouseUp;
  end;



  TDxTreeNode = class
  private
    FOwner: TDxTreeView;
    FParent: TDxTreeNode;

    FList: TList;
    FCaption: string;

    FStyle: TButtonStyle;
    FChecked: Boolean;
    FExpand: Boolean;
    FLevel: Integer;
    FIndex: Integer;

    FClickSound: TClickSound;
    FOnClick: TOnClickEx;
    FOnClickSound: TOnClickSound;

    procedure SetCaption(Value: string);
    procedure SetExpand(Value: Boolean);
    procedure SetChecked(Value: Boolean);
    function GetLevel: Integer;
    function GetItem(Index: Integer): TDxTreeNode;
    function GetCount: Integer;
  public
    Data: Pointer;
    CaptionColor: TDxCaptionColor;
    MouseDowned: Boolean;
    MouseMoveed: Boolean;
    constructor Create();
    destructor Destroy(); override;
    property List: TList read FList;
    property Owner: TDxTreeView read FOwner write FOwner;
    property Parent: TDxTreeNode read FParent write FParent;
    property Caption: string read FCaption write SetCaption;
    property Style: TButtonStyle read FStyle write FStyle;
    property Expand: Boolean read FExpand write SetExpand;
    property Checked: Boolean read FChecked write SetChecked;
    property ClickCount: TClickSound read FClickSound write FClickSound;
    property OnClickSound: TOnClickSound read FOnClickSound write FOnClickSound;
    property Items[Index: Integer]: TDxTreeNode read GetItem;
    property Count: Integer read GetCount;
    property Level: Integer read GetLevel;
    procedure Add(Item: TDxTreeNode); overload;
    procedure Add(TreeNodeList: TList); overload;
    procedure Delete(Item: TDxTreeNode);
    procedure Clear;
    function IndexOf(Item: TDxTreeNode): Integer;
    function ExpandCount: Integer;
  end;

  TDxTreeView = class(TDMemo)
  private
    FList: TList;
    TreeNodeList: TList;
    FOnSelect: TTreeNodeEventEvent;
    FShowButton: Boolean;
    function GetItem(Index: Integer): TDxTreeNode;
    function Get(Index: Integer): TDxTreeNode;
    function GetCount: Integer;
  protected
    procedure DoScroll(Value: Integer); override;

  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy(); override;

    procedure DirectPaint(dsurface: TDxSurface); override;
    function Click(X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;

    property Items[Index: Integer]: TDxTreeNode read GetItem;
    property Indexs[Index: Integer]: TDxTreeNode read Get;
    property Count: Integer read GetCount;
    procedure Add(Item: TDxTreeNode);
    procedure Delete(Item: TDxTreeNode);
    procedure Clear;
    procedure RefTreeNodeList();
    function IndexOf(Item: TDxTreeNode): Integer;
  published
    property OnSelect: TTreeNodeEventEvent read FOnSelect write FOnSelect;
    property ShowButton: Boolean read FShowButton write FShowButton;
  end;

  TDxListItem = class(TStringList)

  private
    FItemList: array of TViewItem;
    function GetItem(Index: Integer): pTViewItem;
  protected

  public
    constructor Create;
    destructor Destroy; override;

    function AddObject(const S: string; AObject: TObject): Integer; override;
    procedure Clear; override;
    procedure Delete(Index: Integer); override;
    procedure InsertObject(Index: Integer; const S: string;
      AObject: TObject); override;

    function AddItem(const S: string; AObject: TObject): pTViewItem;

    property Items[Index: Integer]: pTViewItem read GetItem;
  published

  end;

  TDxListView = class(TDMemo)
  private
    FLines: TList;
    ColWidths: array of Integer;
    FColCount: Integer;

    FOnListItemClick: TOnListItem;
    function GetCount: Integer;
    function GetViewItem(Index: Integer): TDxListItem;
    function GetColWidth(Index: Integer): Integer;
    procedure SetColWidth(Index: Integer; Value: Integer);
    procedure SetColCount(Value: Integer);
    procedure FillItemMouse;
  protected
    function InRange(X, Y: Integer): Boolean; override;
    procedure DoScroll(Value: Integer); override;
    //procedure DoClick(X, Y: Integer); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function Click(X, Y: Integer): Boolean; override;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean; override;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean; override;
    procedure DirectPaint(dsurface: TDxSurface); override;
    function Add: TDxListItem;
    procedure Clear;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TDxListItem read GetViewItem;

    property ColWidth[Index: Integer]: Integer read GetColWidth write SetColWidth;
  published
    property ColCount: Integer read FColCount write SetColCount;
    property HeaderHeight: Integer read FOffSetY write FOffSetY;
    property OnListItemClick: TOnListItem read FOnListItemClick write FOnListItemClick;
  end;

  TDxTabSheet = class(TDControl);



  TDWinManager = class
  private
  public
    DWinList: TList; //list of TDControl;
    constructor Create;
    destructor Destroy; override;
    procedure AddDControl(dcon: TDControl; Visible: Boolean);
    procedure DelDControl(dcon: TDControl);
    procedure ClearAll;
    procedure Process;
    function KeyPress(var Key: Char): Boolean;
    function KeyDown(var Key: Word; Shift: TShiftState): Boolean;
    function MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
    function MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
    function MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
    function MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean;
    function MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean;


    function DblClick(X, Y: Integer): Boolean;
    function Click(X, Y: Integer): Boolean;
    procedure DirectPaint(dsurface: TDxSurface);
  end;

procedure Register;
procedure SetDFocus(dcon: TDControl);
procedure ReleaseDFocus;
procedure SetDCapture(dcon: TDControl);
procedure ReleaseDCapture;

procedure ChgNumber(var No1, No2: Integer);
type
  TDebugPro = procedure(sMsg: string);

var
  LabelClickTimeTick: LongWord;
  MouseWheelControl: TDControl = nil;
  MouseCaptureControl: TDControl = nil; //mouse message
  FocusedControl: TDControl = nil; //Key message
  MainWinHandle: Integer;
  ModalDWindow: TDControl = nil;
  MouseMoveControl: TDControl = nil;
  MouseDownControl: TDControl = nil;
  ActiveMenu: TDPopupMenu = nil;

  DWinMan: TDWinManager;
  DebugPro: TDebugPro = nil;
implementation

const
  TextChars = [#32..#255];
  BorderStyles: array[TBorderStyle] of Dword = (0, WS_BORDER);

procedure Register;
begin
  RegisterComponents('MirGame', [TDControl, TDButton, TDGrid,
    TDWindow, TDPopupMenu, TDLabel, TDEdit, TDCheckBox, TDCombobox,
      TDMemo, TDTabSheet, TDPageControl, TDxChatMemo, TDxListView]);
end;

procedure ChgNumber(var No1, No2: Integer);
var
  NO3: Integer;
begin
  if No1 > No2 then begin
    NO3 := No2;
    No2 := No1;
    No1 := NO3;
  end;
end;

procedure SetDFocus(dcon: TDControl);
begin
  FocusedControl := dcon;
end;

procedure ReleaseDFocus;
begin
  FocusedControl := nil;
end;

procedure SetDCapture(dcon: TDControl);
begin
  SetCapture(MainWinHandle);
  MouseCaptureControl := dcon;
  MouseWheelControl := dcon;
end;

procedure ReleaseDCapture;
begin
  ReleaseCapture;
  MouseCaptureControl := nil;
end;


constructor TDCustomControl.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  ControlStyle := [csAcceptsControls, csCaptureMouse, csClickEvents,
    csSetCaption, csDoubleClicks, csReplicatable, csParentBackground];
  Width := 60;
  Height := 30;
end;

procedure TDCustomControl.AdjustClientRect(var Rect: TRect);
begin
  inherited AdjustClientRect(Rect);
  Canvas.Font := Font;
  Inc(Rect.Top, Canvas.TextHeight('0'));
  InflateRect(Rect, -1, -1);
  if Ctl3D then InflateRect(Rect, -1, -1);
end;

procedure TDCustomControl.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
  with Params.WindowClass do
    Style := Style and not (CS_HREDRAW or CS_VREDRAW);
end;

procedure TDCustomControl.CMDialogChar(var Message: TCMDialogChar);
begin
  with Message do
    if IsAccel(CharCode, Caption) and CanFocus then
    begin
      SelectFirst;
      Result := 1;
    end else
      inherited;
end;

procedure TDCustomControl.CMTextChanged(var Message: TMessage);
begin
  Invalidate;
  Realign;
end;

procedure TDCustomControl.CMCtl3DChanged(var Message: TMessage);
begin
  inherited;
  Invalidate;
  Realign;
end;

procedure TDCustomControl.WMSize(var Message: TMessage);
begin
  inherited;
  Invalidate;

end;

{----------------------------- TDControl -------------------------------}

constructor TDControl.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  DParent := nil;
  //if not (csDesigning in ComponentState) then
  inherited Visible := False;

  FEnableFocus := False;
  Background := False;

  FOnDirectPaint := nil;
  FOnKeyPress := nil;
  FOnKeyDown := nil;
  FOnMouseMove := nil;
  FOnMouseDown := nil;
  FOnMouseUp := nil;
  FOnInRealArea := nil;
  DControls := TList.Create;
  FDParent := nil;
  FOnProcess := nil;
  Width := 80;
  Height := 24;
  FCaption := '';
  FVisible := True;
  FControlVisible := True;
   //FaceSurface := nil;
  WLib := nil;
  FaceIndex := 0;

  FParentNotify := False;
  FX := 0;
  FY := 0;
  FIdx := -1;
  FString := '';
  FData := nil;
  FKeyPreview := False;
  FFade := False;
  FFadeAlpha := 255;
  FFadeTick := GetTickCount;
end;

destructor TDControl.Destroy;
var
  I: Integer;
  dcon: TDControl;
begin
  if Self = FocusedControl then ReleaseDFocus;
  if Self = MouseCaptureControl then ReleaseDCapture;
  if Self = ModalDWindow then ModalDWindow := nil;
  if Self = MouseMoveControl then MouseMoveControl := nil;
  if Self = MouseDownControl then MouseDownControl := nil;
  if Self = MouseWheelControl then MouseWheelControl := nil;
  if DParent <> nil then
    for I := DParent.DControls.Count - 1 downto 0 do begin
      dcon := TDControl(DParent.DControls.Items[I]);
      if dcon = Self then begin
        DParent.DControls.Delete(I);
        Break;
      end;
    end;
  DControls.Free;
  inherited Destroy;
end;

function TDControl.GetMouseMove: Boolean;
begin
  Result := MouseMoveControl = Self;
end;

function TDControl.GetClientRect: TRect;
begin
  Result.Left := SurfaceX(Left);
  Result.Top := SurfaceY(Top);
  Result.Right := Result.Left + Width;
  Result.Bottom := Result.Top + Height;
end;

function TDControl.GetFirstDParent: TDControl;
var
  d: TDControl;
begin
  Result := nil;
  d := Self;
  while True do begin
    if (d <> nil) and (d.DParent <> nil) and (not d.DParent.Background) then begin
      d := d.DParent;
    end else break;
  end;
  Result := d;
end;

procedure TDControl.SetCaption(Str: string);
begin
  FCaption := Str;
  if csDesigning in ComponentState then begin
    Refresh;
  end else CaptionChaged;
end;

procedure TDControl.CaptionChaged;
begin

end;

procedure TDControl.Paint;
begin
  if csDesigning in ComponentState then begin
    if Self is TDTabSheet then begin
      if TDTabSheet(Self).PageControl = nil then begin
        with Canvas do begin
          Pen.Color := clBlack;
          MoveTo(0, 0);
          LineTo(Width - 1, 0);
          LineTo(Width - 1, Height - 1);
          LineTo(0, Height - 1);
          LineTo(0, 0);
          LineTo(Width - 1, Height - 1);
          MoveTo(Width - 1, 0);
          LineTo(0, Height - 1);
          TextOut((Width - TextWidth(Caption)) div 2, (Height - TextHeight(Caption)) div 2, Caption);
        end;
      end else begin
        if Visible then begin
          with Canvas do begin
            Pen.Color := clBlack;
            MoveTo(0, 0);
            LineTo(Width - 1, 0);
            LineTo(Width - 1, Height - 1);
            LineTo(0, Height - 1);
            LineTo(0, 0);
            LineTo(Width - 1, Height - 1);
            MoveTo(Width - 1, 0);
            LineTo(0, Height - 1);
            TextOut((Width - TextWidth(Caption)) div 2, (Height - TextHeight(Caption)) div 2, Caption);
          end;
        end;
      end;
    end else begin
      with Canvas do begin
        Pen.Color := clBlack;
        MoveTo(0, 0);
        LineTo(Width - 1, 0);
        LineTo(Width - 1, Height - 1);
        LineTo(0, Height - 1);
        LineTo(0, 0);
        TextOut((Width - TextWidth(Caption)) div 2, (Height - TextHeight(Caption)) div 2, Caption);
      end;
    end;
  end;
end;

procedure TDControl.Loaded;
var
  I: Integer;
  dcon: TDControl;
begin
  if not (csDesigning in ComponentState) then begin
    if Parent <> nil then
      for I := 0 to TControl(Parent).ComponentCount - 1 do begin
        if TControl(Parent).Components[I] is TDControl then begin
          dcon := TDControl(TControl(Parent).Components[I]);
          if dcon.DParent = Self then begin
            AddChild(dcon);
          end;
        end;
      end;
  end;
end;

function TDControl.SurfaceX(X: Integer): Integer;
var
  d: TDControl;
begin
  d := Self;
  while True do begin
    if d.DParent = nil then Break;
    X := X + d.DParent.Left;
    d := d.DParent;
  end;
  Result := X;
end;

function TDControl.SurfaceY(Y: Integer): Integer;
var
  d: TDControl;
begin
  d := Self;
  while True do begin
    if d.DParent = nil then Break;
    Y := Y + d.DParent.Top;
    d := d.DParent;
  end;
  Result := Y;
end;


function TDControl.LocalX(X: Integer): Integer;
var
  d: TDControl;
begin
  d := Self;
  while True do begin
    if d.DParent = nil then Break;
    X := X - d.DParent.Left;
    d := d.DParent;
  end;
  Result := X;
end;

function TDControl.LocalY(Y: Integer): Integer;
var
  d: TDControl;
begin
  d := Self;
  while True do begin
    if d.DParent = nil then Break;
    Y := Y - d.DParent.Top;
    d := d.DParent;
  end;
  Result := Y;
end;

procedure TDControl.UnLoaded;
var
  I: Integer;
  dcon: TDControl;
begin
  if DParent <> nil then
    for I := DParent.DControls.Count - 1 downto 0 do begin
      dcon := TDControl(DParent.DControls.Items[I]);
      if dcon = Self then begin
        DParent.DControls.Delete(I);
      end;
    end;
end;

procedure TDControl.DoShow;
begin

end;

{procedure TDControl.CMVisibleChanged(var Message: TMessage);
begin
  inherited;
  if not (csDesigning in ComponentState) then begin
    if Visible then begin
      SetFocus;
    end else begin
      ReleaseFocus;
    end;
    DoShow;
  end;
end;  }

function TDControl.GetVisible: Boolean;
begin
  Result := FVisible and FControlVisible;
end;

procedure TDControl.SetVisible(Value: Boolean);
begin
  if FVisible <> Value then begin
    FVisible := Value;
    if (csDesigning in ComponentState) then begin
      inherited Visible := FVisible;
    end else begin
      if FVisible then begin
        SetFocus;
      end else begin
        ReleaseFocus;
      end;
      DoShow;
    end;
  end;
end;

procedure TDControl.ReleaseFocus;
var
  I: Integer;
begin
  if Self = FocusedControl then ReleaseDFocus;
  if Self = MouseCaptureControl then ReleaseDCapture;
  if Self = ModalDWindow then ModalDWindow := nil;
  if Self = MouseMoveControl then MouseMoveControl := nil;
  if Self = MouseDownControl then MouseDownControl := nil;
  if DControls <> nil then begin
    for I := 0 to DControls.Count - 1 do begin
      TDControl(DControls[I]).ReleaseFocus;
    end;
  end;
end;

procedure TDControl.SetDParent(Value: TDControl);
var
  I: Integer;
begin
  Parent := Value;
  if Value <> FDParent then begin
    if FDParent <> nil then UnLoaded;
    FDParent := Value;
    if FDParent <> nil then begin
      //SetCanvas(FDParent.Canvas);
      for I := 0 to FDParent.DControls.Count - 1 do
        if Self = FDParent.DControls[I] then Exit;
      FDParent.DControls.Add(Self);

    end;
  end;
end;

procedure TDControl.SetDowned(Value: Boolean);
var
  I: Integer;
  d: TDControl;
begin
  if not (csDesigning in ComponentState) then begin
    if Self is TDButton then begin
      case TDButton(Self).Style of
        bsButton: begin
            FDowned := Value;
          end;
        bsRadio: begin
            if Value and not FDowned then begin
              if (FDParent <> nil) then begin
                for I := 0 to FDParent.DControls.Count - 1 do begin
                  d := TDControl(FDParent.DControls[I]);
                  if (d <> Self) and (d is TDButton) and (TDButton(d).Style = bsRadio) then begin
                    TDButton(d).Downed := False;
                  end;
                end;
              end;
            end;
            FDowned := Value;
          end;
        bsCheckBox: begin
            FDowned := Value;
          end;
      end;
    end else
      if Self is TDLabel then begin
      case TDLabel(Self).Style of
        bsButton: begin
            FDowned := Value;
          end;
        bsRadio: begin
            if Value and not FDowned then begin
              if (FDParent <> nil) then begin
                for I := 0 to FDParent.DControls.Count - 1 do begin
                  d := TDControl(FDParent.DControls[I]);
                  if (d <> Self) and (d is TDLabel) and (TDLabel(d).Style = bsRadio) then begin
                    TDLabel(d).Downed := False;
                  end;
                end;
              end;
            end;
            FDowned := Value;
          end;
        bsCheckBox: begin
            FDowned := Value;
          end;
      end;
    end else begin
      FDowned := Value;
    end;
  end else FDowned := Value;
end;

procedure TDControl.SetFocus;
var
  I: Integer;
begin
  if EnableFocus and Visible then SetDFocus(Self);
  if (FocusedControl = nil) and Visible then begin
    for I := 0 to DControls.Count - 1 do begin
      if (FocusedControl = nil) then
        TDControl(DControls[I]).SetFocus
      else break;
    end;
  end;
end;

procedure TDControl.AddChild(dcon: TDControl);
var
  I: Integer;
begin
  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]) = dcon then Exit;
  DControls.Add(Pointer(dcon));
end;

procedure TDControl.ChangeChildOrder(dcon: TDControl);
var
  I: Integer;
begin
  if not ((Self is TDWindow) or (Self is TDPageControl)) then Exit;
  if TDWindow(dcon).Floating then begin
    for I := 0 to DControls.Count - 1 do begin
      if dcon = DControls[I] then begin
        DControls.Delete(I);
        Break;
      end;
    end;
    DControls.Add(dcon);
  end;
end;

procedure TDControl.BringToFront;
var
  I: Integer;
begin
  if not ((Self is TDWindow) or (Self is TDPageControl)) then Exit;
  if DParent <> nil then begin
    for I := 0 to DParent.DControls.Count - 1 do begin
      if Self = DParent.DControls[I] then begin
        DParent.DControls.Delete(I);
        Break;
      end;
    end;
    DParent.DControls.Add(Self);
  end;
end;

procedure DebugOutStr(Msg: string);
var
  flname: string;
  fhandle: TextFile;
begin
//DScreen.AddChatBoardString(msg,clWhite, clBlack);
  //exit;
  if Assigned(DebugPro) then
    DebugPro(Msg);

  {flname := '.\DebugOutStr.txt';
  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  Writeln(fhandle, TimeToStr(Time) + ' ' + Msg);
  CloseFile(fhandle);}
end;

function TDControl.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;

    if Assigned(FOnInRealArea) then begin
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    end else begin
      if WLib <> nil then begin
        d := WLib.Images[FaceIndex];
        if d <> nil then begin
          if d.Pixels[X - Left, Y - Top] = 0 then
            boInrange := False;
        end;
      end;
    end;
    Result := boInrange;
  end else Result := False;
end;

function TDControl.KeyPress(var Key: Char): Boolean;
var
  I: Integer;
  d: TDControl;
begin
  Result := False;
  if Background then Exit;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).KeyPress(Key) then begin
        Result := True;
        Exit;
      end;
  if (FocusedControl = Self) then begin
    if Assigned(FOnKeyPress) then FOnKeyPress(Self, Key);
    Result := True;
  end;
  d := FirstDParent;
  if (d <> nil) and (d <> Self) and (FocusedControl <> d) and d.KeyPreview then
    if Assigned(d.OnKeyPress) then d.OnKeyPress(d, Key);
end;

function TDControl.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
var
  I: Integer;
  d: TDControl;
begin
  Result := False;
  if Background then Exit;

  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).KeyDown(Key, Shift) then begin
        Result := True;
        Exit;
      end;

  if (FocusedControl = Self) then begin
    if Assigned(FOnKeyDown) then FOnKeyDown(Self, Key, Shift);
    Result := True;
  end;

  d := FirstDParent;
  if (d <> nil) and (d <> Self) and (FocusedControl <> d) and d.KeyPreview then
    if Assigned(d.OnKeyDown) then d.OnKeyDown(d, Key, Shift);
end;

function TDControl.CanFocusMsg: Boolean;
begin
  if (MouseCaptureControl = nil) or ((MouseCaptureControl <> nil) and ((MouseCaptureControl = Self) {or (MouseCaptureControl = DParent){ or (MouseCaptureControl.ParentNotify)})) then
    Result := True
  else
    Result := False;
end;

function TDControl.MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean;
var
  I: Integer;
  Handled: Boolean;
begin
  Result := False;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).MouseWheelDown(Shift, MousePos) then begin
        Result := True;
        Exit;
      end;

  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(OnMouseWheelDown) then
        OnMouseWheelDown(Self, Shift, MousePos, Handled);
      Result := True;
    end;
    Exit;
  end;
end;

function TDControl.MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean;
var
  I: Integer;
  Handled: Boolean;
begin
  Result := False;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).MouseWheelUp(Shift, MousePos) then begin
        Result := True;
        Exit;
      end;

  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(OnMouseWheelUp) then
        OnMouseWheelUp(Self, Shift, MousePos, Handled);
      Result := True;
    end;
    Exit;
  end;
end;


function TDControl.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).MouseMove(Shift, X - Left, Y - Top) then begin
        Result := True;
        Exit;
      end;

  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(FOnMouseMove) then
        FOnMouseMove(Self, Shift, X, Y);
      Result := True;
    end;
    Exit;
  end;

  if Background then Exit;

  if InRange(X, Y) then begin
    MouseMoveControl := Self;
    if Assigned(FOnMouseMove) then
      FOnMouseMove(Self, Shift, X, Y);
    Result := True;
  end;
end;

function TDControl.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).MouseDown(Button, Shift, X - Left, Y - Top) then begin
        Result := True;
        Exit;
      end;
  if Background then begin
    if Assigned(FOnBackgroundClick) then begin
      WantReturn := False;
      FOnBackgroundClick(Self);
      if WantReturn then Result := True;
    end;
    ReleaseDFocus;
    Exit;
  end;
  if CanFocusMsg then begin
    if InRange(X, Y) or (MouseCaptureControl = Self) then begin
      //DebugOutStr(Name+' TDControl.MouseDown '+Caption);
      MouseMoveControl := nil;
      MouseDownControl := Self;
      if Assigned(FOnMouseDown) then
        FOnMouseDown(Self, Button, Shift, X, Y);
      if EnableFocus then SetDFocus(Self);
         //else ReleaseDFocus;
      Result := True;
    end;
  end;
end;

function TDControl.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).MouseUp(Button, Shift, X - Left, Y - Top) then begin
        Result := True;
        Exit;
      end;

  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(FOnMouseUp) then
        FOnMouseUp(Self, Button, Shift, X, Y);
      Result := True;
    end;
    Exit;
  end;

  if Background then Exit;
  if InRange(X, Y) then begin
    if Assigned(FOnMouseUp) then
      FOnMouseUp(Self, Button, Shift, X, Y);
    Result := True;
  end;
end;

function TDControl.DblClick(X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(FOnDblClick) then
        FOnDblClick(Self);
      Result := True;
    end;
    Exit;
  end;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).DblClick(X - Left, Y - Top) then begin
        Result := True;
        Exit;
      end;
  if Background then Exit;
  if InRange(X, Y) then begin
    if Assigned(FOnDblClick) then
      FOnDblClick(Self);
    Result := True;
  end;
end;

function TDControl.Click(X, Y: Integer): Boolean;
var
  I, nY: Integer;
begin
  Result := False;
  nY := 0;

  if (MouseCaptureControl <> nil) then begin
    if (MouseCaptureControl = Self) then begin
      if Assigned(FOnClick) then
        FOnClick(Self, X, Y);
      Result := True;
    end;
    Exit;
  end;
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible and TDControl(DControls[I]).Enabled then
      if TDControl(DControls[I]).Click(X - Left, Y - Top + nY) then begin
        Result := True;
        Exit;
      end;
  if Background then Exit;
  if InRange(X, Y) then begin
    if Assigned(FOnClick) then
      FOnClick(Self, X, Y);
    Result := True;
  end;
end;


procedure TDControl.SetImgIndex(Lib: TGameImages; Index: Integer);
var
  d: TDxSurface;
begin
   //FaceSurface := dsurface;
  if Lib <> nil then begin
    d := Lib.Images[Index];
    WLib := Lib;
    FaceIndex := Index;
    if d <> nil then begin
      Width := d.Width;
      Height := d.Height;
    end;
  end;
end;

procedure TDControl.Process;
var
  I: Integer;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).Process;
end;

procedure TDControl.DirectPaint(dsurface: TDxSurface);
var
  I: Integer;
  d: TDxSurface;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
  end;
  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;


{--------------------- TDButton --------------------------}


constructor TDButton.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FDowned := False;
  FOnClick := nil;
  FEnableFocus := False;
  FClickSound := csNone;
  FButtonStyle := bsButton;
  FAlignment := taCenter;
  FShowCaption := False;
  FColors := TColors.Create;
end;

destructor TDButton.Destroy;
begin
  FColors.Free;
  inherited Destroy;
end;

procedure TDButton.SetAlignment(Value: TAlignment);
begin
  if FAlignment <> Value then begin
    FAlignment := Value;
  end;
end;

function TDButton.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) then begin
    //Result := inherited MouseMove(Shift, X, Y);
    case FButtonStyle of
      bsButton: begin
          if MouseCaptureControl = Self then
            if InRange(X, Y) then Downed := True
            else Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;

  end;
end;

function TDButton.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    if (not Background) and (MouseCaptureControl = nil) then begin
      case FButtonStyle of
        bsButton: begin
            Downed := True;
          end;
        bsRadio: begin

          end;
        bsCheckBox: begin

          end;
      end;
      SetDCapture(Self);
    end;
    Result := True;
  end;
end;

function TDButton.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  d: TDControl;
  boDown: Boolean;
begin
  Result := False;
  ReleaseDCapture;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    if not Background then begin
      if InRange(X, Y) then begin
        case FButtonStyle of
          bsButton: begin
              Downed := False;
              if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
              if Assigned(FOnClick) then FOnClick(Self, X, Y);
            end;
          bsRadio: begin
              boDown := Downed;
              if (FDParent <> nil) then begin
                for I := 0 to FDParent.DControls.Count - 1 do begin
                  d := TDControl(FDParent.DControls[I]);
                  if (d <> Self) and (d is TDButton) and (TDButton(d).Style = bsRadio) then begin
                    TDButton(d).Downed := False;
                  end;
                end;
              end;

              Downed := True;

              if not boDown then begin
                if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
                if Assigned(FOnClick) then FOnClick(Self, X, Y);
              end;

            end;
          bsCheckBox: begin
              Downed := not Downed;
              if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
              if Assigned(FOnClick) then FOnClick(Self, X, Y);
            end;
        end;
      end;
    end;
    case FButtonStyle of
      bsButton: begin
          Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;
    Result := True;
    Exit;
  end else begin
    //ReleaseDCapture;
    case FButtonStyle of
      bsButton: begin
          Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;
  end;
end;

procedure TDButton.DirectPaint(dsurface: TDxSurface);
var
  I, nX: Integer;
  d: TDxSurface;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
  end;

  if FShowCaption then begin
    if FCaption <> '' then begin
      case FAlignment of
        taLeftJustify: nX := SurfaceX(Left);
        taRightJustify: nX := Max(SurfaceX(Left), SurfaceX(Left + (Width - dsurface.TextWidth(FCaption))));
        taCenter: nX := SurfaceX(Left) + (Width - dsurface.TextWidth(FCaption)) div 2;
      end;
      if Downed then
        dsurface.TextOut(nX + 1,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2 + 1, FCaption, Colors.Down)
      else
        dsurface.TextOut(nX,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, FCaption, Colors.Up);
    end;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;

procedure TDButton.Process;
var
  I: Integer;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  if Downed then begin
    Font.Color := FColors.Down;
  end else
    if MouseMoveing then begin
    Font.Color := FColors.Hot;
  end else begin
    Font.Color := FColors.Up;
  end;
  {if CompareStr(FOldCaption, FCaption) <> 0 then begin
    FOldCaption := FCaption;
    CaptionChaged;
  end; }
  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).Process;
end;

{------------------------- TDGrid --------------------------}

constructor TDGrid.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FColCount := 8;
  FRowCount := 5;
  FColWidth := 36;
  FRowHeight := 32;
  FOnGridSelect := nil;
  FOnGridMouseMove := nil;
  FOnGridPaint := nil;
end;

function TDGrid.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

function TDGrid.GetColRow(X, Y: Integer; var ACol, ARow: Integer): Boolean;
begin
  Result := False;
  if InRange(X, Y) then begin
    ACol := (X - Left) div FColWidth;
    ARow := (Y - Top) div FRowHeight;
    Result := True;
  end;
end;

procedure TDGrid.ClearSelect;
begin
  SelectCell.X := -1;
  SelectCell.Y := -1;
end;

function TDGrid.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  ACol, ARow: Integer;
begin
  Result := False;
  //if mbLeft = Button then begin
  if GetColRow(X, Y, ACol, ARow) then begin
    SelectCell.X := ACol;
    SelectCell.Y := ARow;
    DownPos.X := X;
    DownPos.Y := Y;
    SetDCapture(Self);
    Result := True;
  end;
  //end;
end;

function TDGrid.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  ACol, ARow: Integer;
begin
  Result := False;
  if InRange(X, Y) then begin
    if GetColRow(X, Y, ACol, ARow) then begin
      if Assigned(FOnGridMouseMove) then
        FOnGridMouseMove(Self, ACol, ARow, Shift);
    end;
    Result := True;
  end;
end;

function TDGrid.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  ACol, ARow: Integer;
begin
  Result := False;
  //if mbLeft = Button then begin
  if GetColRow(X, Y, ACol, ARow) then begin
    if (SelectCell.X = ACol) and (SelectCell.Y = ARow) then begin
      Col := ACol;
      row := ARow;
      if Assigned(FOnGridSelect) then
        FOnGridSelect(Self, ACol, ARow, Button, Shift);
    end;
    Result := True;
  end;
  ReleaseDCapture;
  //end;
end;

function TDGrid.Click(X, Y: Integer): Boolean;
var
  ACol, ARow: Integer;
begin
  Result := False;
  { if GetColRow (X, Y, acol, arow) then begin
      if Assigned (FOnGridSelect) then
         FOnGridSelect (self, acol, arow, []);
      Result := TRUE;
   end; }
end;

procedure TDGrid.DirectPaint(dsurface: TDxSurface);
var
  I, j: Integer;
  rc: TRect;
begin
  if Assigned(FOnGridPaint) then
    for I := 0 to FRowCount - 1 do
      for j := 0 to FColCount - 1 do begin
        rc := Rect(Left + j * FColWidth, Top + I * FRowHeight, Left + j * (FColWidth + 1) - 1, Top + I * (FRowHeight + 1) - 1);
        if (SelectCell.Y = I) and (SelectCell.X = j) then
          FOnGridPaint(Self, j, I, rc, [gdSelected], dsurface)
        else FOnGridPaint(Self, j, I, rc, [], dsurface);
      end;
end;

constructor TDTabSheet.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FPageControl := nil;
  Visible := False;
  ControlStyle := ControlStyle + [csNoDesignVisible];
end;

procedure TDTabSheet.WMNCHitTest(var Message: TWMNCHitTest);
begin
  if not (csDesigning in ComponentState) then
    Message.Result := HTTRANSPARENT
  else
    inherited;
end;

procedure TDTabSheet.DirectPaint(dsurface: TDxSurface);
var
  I, nX: Integer;
  d: TDxSurface;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
  end;
  //dsurface.FrameRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), clRed);
  {if FShowCaption then begin
    if FCaption <> '' then begin
      case FAlignment of
        taLeftJustify: nX := SurfaceX(Left);
        taRightJustify: nX := Max(SurfaceX(Left), SurfaceX(Left + (Width - dsurface.TextWidth(FCaption))));
        taCenter: nX := SurfaceX(Left) + (Width - dsurface.TextWidth(FCaption)) div 2;
      end;
      if Downed then
        dsurface.TextOut(nX + 1,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2 + 1, FCaption, Colors.Down)
      else
        dsurface.TextOut(nX,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, FCaption, Colors.Up);
    end;
  end; }

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;

procedure TDTabSheet.ReadState(Reader: TReader);
begin
  if Reader.Parent is TDPageControl then
    TDPageControl(Reader.Parent).Add(Self);
  inherited ReadState(Reader);
end;

procedure TDTabSheet.SetPageControl(Value: TDPageControl);
begin
  if FPageControl <> Value then begin
    if FPageControl <> nil then
      FPageControl.Delete(Self);
    FPageControl := Value;
    FPageControl.Add(Self);
    //FPageControl.ActivePage := FPageControl.Tabs.Count - 1;
  end;
end;
{--------------------- TDWindown --------------------------}


constructor TDWindow.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FFloating := False;
  FEnableFocus := True;
  Width := 120;
  Height := 120;
end;

procedure TDWindow.DoShow;
begin
  if Visible and Floating and (DParent <> nil) then
    DParent.ChangeChildOrder(Self);
end;

 {
procedure TDWindow.SetVisible(flag: Boolean);
begin
  FVisible := flag;
  if FVisible then begin
    if Floating then begin
      if DParent <> nil then
        DParent.ChangeChildOrder(Self);
    end;
    SetFocus;
  end else begin
    ReleaseFocus;
  end;
end;   }

{procedure TDWindow.SetFocus;
var
  I: Integer;
begin
  if (FocusedControl = nil) and EnableFocus then SetFocus;
  if (FocusedControl = nil) then begin
    for I := 0 to DControls.Count - 1 do begin
      TDControl(DControls[I]).SetFocus;
    end;
  end;
end; }



function TDWindow.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  al, at: Integer;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if Result and FFloating and ((MouseCaptureControl = Self) or ((MouseCaptureControl <> nil) and MouseCaptureControl.ParentNotify)) then begin
    if (SpotX <> X) or (SpotY <> Y) then begin
      al := Left + (X - SpotX);
      at := Top + (Y - SpotY);
      if al + Width < WINLEFT then al := WINLEFT - Width;
      if al > WINRIGHT then al := WINRIGHT;
      if at + Height < WINTOP then at := WINTOP - Height;
      if at + Height > BOTTOMEDGE then at := BOTTOMEDGE - Height;
      Left := al;
      Top := at;
      SpotX := X;
      SpotY := Y;
    end;
  end;
end;

function TDWindow.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    SpotX := X;
    SpotY := Y;
    BringToFront;
    Result := True;
  end;
end;

{            and ((MouseCaptureControl = nil){ or (MouseCaptureControl.ParentNotify))
begin
  Result := inherited MouseDown(Button, Shift, X, Y);
  if Result then begin
    BringToFront;
    SpotX := X;
    SpotY := Y;
  end;
end;}

function TDWindow.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseUp(Button, Shift, X, Y);
end;

procedure TDWindow.Show;
begin
  Visible := True;
  if Floating then begin
    if DParent <> nil then
      DParent.ChangeChildOrder(Self);
  end;
  //if EnableFocus then SetDFocus(Self);
end;

function TDWindow.ShowModal: Integer;
begin
  Result := 0; //Jacky
  Visible := True;
  ModalDWindow := Self;
  if EnableFocus then SetDFocus(Self);
end;




constructor TDLabel.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FAutoSize := True;
  Downed := False;
  FOnClick := nil;
  FEnableFocus := False;
  FClickSound := csNone;
  Caption := Name;

  FBackgroundColor := clWhite;
  FIdx := -1;
  FShadowSize := 4;
  FShadowColor := $40000000;

  FBorderColor := $00608490;

  FHotBorder := $005894B8;
  FDownBorder := $005894B8;

  FBoldColor := $00040404;
  FCurrColor := FUpColor;
  FBold := False;
  FBackground := False;
  FBorder := False;
  FButtonStyle := bsButton;
  if Assigned(MainForm) then begin
    Canvas.Font.Assign(MainForm.Canvas.Font);
    Canvas.Brush.Assign(MainForm.Canvas.Brush);
    Font.Assign(MainForm.Canvas.Font);
  end;
  FUpColor := Canvas.Font.Color;
  FHotColor := Canvas.Font.Color;
  FDownColor := Canvas.Font.Color;
  FClickTime := 0;
  FAlignment := taLeftJustify;
end;

procedure TDLabel.SetAutoSize(Value: Boolean);
begin
  FAutoSize := Value;
  if not (csDesigning in ComponentState) then begin
    if Assigned(ImageCanvas) then begin

      if FAutoSize then begin
        Width := ImageCanvas.TextWidth(Caption);
        Height := ImageCanvas.TextHeight('0');
      //DebugOutStr(Format('TDLabel.SetCaption Caption:%s  Width:%d  Height:%d', [Caption, Width, Height]));
      end;
    end;
  end;
end;

procedure TDLabel.SetAlignment(Value: TAlignment);
begin
  if FAlignment <> Value then begin
    FAlignment := Value;
  end;
end;

function TDLabel.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDLabel.SetUpColor(Value: TColor);
begin
  if FUpColor <> Value then begin
    FUpColor := Value;
    FCurrColor := Value;
  end;
end;

procedure TDLabel.CaptionChaged;
begin
  if Assigned(ImageCanvas) then begin
    if not (csDesigning in ComponentState) then begin
      if FAutoSize then begin
        Width := ImageCanvas.TextWidth(Caption);
        Height := ImageCanvas.TextHeight('0');
      end;
    end;
  end;
end;

function TDLabel.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) and Enabled then begin
    Result := inherited MouseMove(Shift, X, Y);
    case FButtonStyle of
      bsButton: begin
          if MouseCaptureControl = Self then
            if InRange(X, Y) then Downed := True
            else Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;

  end;
end;

function TDLabel.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    if (not Background) and (MouseCaptureControl = nil) then begin
      case FButtonStyle of
        bsButton: begin
            Downed := True;
          end;
        bsRadio: begin

          end;
        bsCheckBox: begin

          end;
      end;
      SetDCapture(Self);
    end;
    Result := True;
  end;
end;

function TDLabel.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  d: TDControl;
  boDown: Boolean;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    ReleaseDCapture;
    if not Background then begin
      if InRange(X, Y) then begin
        case FButtonStyle of
          bsButton: begin
              Downed := False;
              if GetTickCount - LabelClickTimeTick > FClickTime then begin
                LabelClickTimeTick := GetTickCount;
                if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
                if Assigned(FOnClick) then FOnClick(Self, X, Y);
              end;
            end;
          bsRadio: begin
              if GetTickCount - LabelClickTimeTick > FClickTime then begin
                LabelClickTimeTick := GetTickCount;
                //showmessage(IntToStr(FClickTime));
                boDown := Downed;
                if (FDParent <> nil) then begin
                  for I := 0 to FDParent.DControls.Count - 1 do begin
                    d := TDControl(FDParent.DControls[I]);
                    if (d <> Self) and (d is TDLabel) and (TDLabel(d).Style = bsRadio) then begin
                      TDLabel(d).Downed := False;
                    end;
                  end;
                end;
                Downed := True;
                if (not boDown) then begin
                  if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
                  if Assigned(FOnClick) then FOnClick(Self, X, Y);
                end;
              end;
            end;
          bsCheckBox: begin
              if GetTickCount - LabelClickTimeTick > FClickTime then begin
                LabelClickTimeTick := GetTickCount;
                Downed := not Downed;
                if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
                if Assigned(FOnClick) then FOnClick(Self, X, Y);
              end;
            end;
        end;
      end;
    end;
    case FButtonStyle of
      bsButton: begin
          Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;
    Result := True;
    Exit;
  end else begin
    ReleaseDCapture;
    case FButtonStyle of
      bsButton: begin
          Downed := False;
        end;
      bsRadio: begin

        end;
      bsCheckBox: begin

        end;
    end;
  end;
end;

procedure TDLabel.Process;
var
  OldSize: Integer;
  OldFontStyle: TFontStyles;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);

  if Downed then begin
    FCurrColor := FDownColor;
  end else
    if MouseMoveing then begin
    FCurrColor := FHotColor;
  end else begin
    FCurrColor := FUpColor;
  end;

  FBorderCurrColor := FBorderColor;
  if Enabled then begin
    if MouseDownControl = Self then FBorderCurrColor := FDownBorder;
    if MouseMoveControl = Self then FBorderCurrColor := FHotBorder;
  end;
end;

procedure TDLabel.DirectPaint(dsurface: TDxSurface);
var
  I, nX: Integer;
  nAlpha: Integer;
  d: TDxSurface;
  ARect: TRect;
  FontStyle: TFontStyles;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  if FBackground then
    dsurface.FillRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), FBackgroundColor);
  if FBorder then
    dsurface.FrameRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), FBorderCurrColor);

  SetImageFont(Font.Size, fsBold in Font.Style);
  FontStyle := ImageFont.Styles;
  ImageFont.Styles := Font.Style;

  if Caption <> '' then begin
    if Caption = '-' then begin
      dsurface.FillRect(Bounds(SurfaceX(Left), SurfaceY(Top) + dsurface.TextHeight('0') div 2, Width, 1), FCurrColor);
    end else begin
      case FAlignment of
        taLeftJustify: nX := SurfaceX(Left);
        taRightJustify: nX := Max(SurfaceX(Left), SurfaceX(Left) + (Width - dsurface.TextWidth(Caption)));
        taCenter: nX := SurfaceX(Left) + (Width - dsurface.TextWidth(Caption)) div 2;
      end;
      if FBold then begin
        dsurface.BoldTextOut(nX,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, Caption, FCurrColor, FBoldColor);
      end else begin
        dsurface.TextOut(nX,
          SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, Caption, FCurrColor);
      end;
    end;
  end;
  ImageFont.Styles := FontStyle;
  SetImageFont();

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;

constructor TColors.Create();
begin
  inherited Create;
  FDisabled := clBtnFace;
  FBkgrnd := clWhite;
  FBorder := clGray;
  FFont := clBlack;
  FUp := $00F1EFAB;
  FHot := $00F1EFAB;
  FDown := $00F1EFAB;
  FLine := clBtnFace;
end;

constructor TDMenuItem.Create();
begin
  inherited;
  FVisible := True;
  FEnabled := True;
  FChecked := False;
  FCaption := '';
  FMenu := nil;
end;

destructor TDMenuItem.Destroy;
begin
  //if FMenu <> nil then FMenu.Free;
  inherited;
end;

constructor TDPopupMenu.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FItems := TStringList.Create();
  FColors := TColors.Create;
  FActiveMenu := nil;
  FOwnerMenu := nil;
  FOwnerItemIndex := 0;
  FMoveItemIndex := -1;
  FItemIndex := -1;
  Width := 150;
  Height := 100;

  FAlpha := 255;
  FDrawBorder := True;
  FStyle := sXP;
  Add('Item1', nil);
  Add('Item2', nil);
  Add('Item3', nil);
  Add('Item4', nil);
end;

destructor TDPopupMenu.Destroy;
begin
  while Count > 0 do begin
    Items[0].Free;
    Delete(0);
  end;
  FItems.Free;
  FColors.Free;
  inherited Destroy;
end;

procedure TDPopupMenu.Paint;
var
  I: Integer;
begin
  if csDesigning in ComponentState then begin
    with Canvas do begin
      Brush.Color := clWhite;//clMenu;
      FillRect(ClipRect);
      Pen.Color := clInactiveBorder;

      for I := 0 to Count - 1 do begin
        MoveTo(5, Height div Count * I);
        LineTo(Width - 5, Height div Count * I);
        TextOut((Width - TextWidth(FItems[I])) div 2, Height div Count * I + (Height div Count - TextHeight(FItems[I])) div 2, FItems[I]);
      end;

      MoveTo(0, 0);
      LineTo(Width - 1, 0);
      LineTo(Width - 1, Height - 1);
      LineTo(0, Height - 1);
      LineTo(0, 0);
    end;
  end;
end;

procedure TDPopupMenu.CreateWnd;
begin
  inherited;
  if FItems = nil then FItems := TStringList.Create();
end;

procedure TDPopupMenu.SetOwnerMenu(Value: TDPopupMenu);
var
  Index: Integer;
begin
  if FOwnerMenu <> Value then begin
    if (FOwnerMenu <> nil) then begin
      Index := FOwnerMenu.IndexOf(Self);
      if Index >= 0 then begin
        FOwnerMenu.Menus[Index] := nil;
      end;
    end;
    FOwnerMenu := Value;
  end;
end;

procedure TDPopupMenu.SetOwnerItemIndex(Value: TImageIndex);
var
  Index: Integer;
begin
  if FOwnerMenu <> nil then begin
    if (FOwnerItemIndex >= 0) and (FOwnerItemIndex < FOwnerMenu.Count) then FOwnerMenu.Menus[FOwnerItemIndex] := nil;
    if (Value >= 0) and (Value < FOwnerMenu.Count) then begin
      for Index := Value to FOwnerMenu.Count - 1 do begin
        if FOwnerMenu.Menus[Index] = nil then begin
          FOwnerMenu.Menus[Index] := Self;
          FOwnerItemIndex := Index;
          Break;
        end;
      end;
    end else FOwnerItemIndex := -1;
  end else FOwnerItemIndex := -1;
end;

function TDPopupMenu.GetCount: Integer;
begin
  Result := FItems.Count;
end;

function TDPopupMenu.GetItems: TStrings;
begin
  if csDesigning in ComponentState then Refresh;
  Result := FItems;
end;

procedure TDPopupMenu.SetColors(Value: TColors);
begin
  FColors.Assign(Value);
end;

procedure TDPopupMenu.SetItems(Value: TStrings);
var
  I: Integer;
begin
  Clear;
  FItems.Assign(Value);
  for I := 0 to FItems.Count - 1 do begin
    FItems.Objects[I] := nil;
    FItems.Objects[I] := TDMenuItem.Create;
  end;
end;

procedure TDPopupMenu.SetItemIndex(Value: Integer);
begin
  FItemIndex := Value;
  if FItemIndex >= FItems.Count then FItemIndex := -1;
  {if FItemIndex <> Value then begin

  end;}
end;

function TDPopupMenu.GetItem(Index: Integer): TDMenuItem;
begin
  if (Index >= 0) and (Index < FItems.Count) then begin
    if FItems.Objects[Index] = nil then begin
      FItems.Objects[Index] := TDMenuItem.Create;
    end;
    Result := TDMenuItem(FItems.Objects[Index]);
  end else Result := nil;
end;

function TDPopupMenu.GetMenu(Index: Integer): TDPopupMenu;
begin
  if (Index >= 0) and (Index < FItems.Count) then begin
    if FItems.Objects[Index] = nil then begin
      FItems.Objects[Index] := TDMenuItem.Create;
    end;
    Result := TDPopupMenu(TDMenuItem(FItems.Objects[Index]).Menu);
  end else Result := nil;
end;

procedure TDPopupMenu.SetMenu(Index: Integer; Value: TDPopupMenu);
begin
  if FItems.Objects[Index] = nil then begin
    FItems.Objects[Index] := TDMenuItem.Create;
  end;
  TDMenuItem(FItems.Objects[Index]).Menu := Value;
end;

function TDPopupMenu.IndexOf(Item: TDPopupMenu): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to FItems.Count - 1 do begin
    if FItems.Objects[I] = nil then begin
      FItems.Objects[I] := TDMenuItem.Create();
    end;
    if TDMenuItem(FItems.Objects[I]).Menu = Item then begin
      Result := I;
      Exit;
    end;
  end;
end;

procedure TDPopupMenu.Insert(Index: Integer; ACaption: string; Item: TDPopupMenu; nIndex: Integer);
var
  MenuItem: TDMenuItem;
begin
  MenuItem := TDMenuItem.Create();
  MenuItem.Menu := Item;
  MenuItem.Index := nIndex;
  FItems.InsertObject(Index, ACaption, MenuItem);
  //if csDesigning in ComponentState then Refresh;
end;

procedure TDPopupMenu.Add(ACaption: string; Item: TDPopupMenu; nIndex: Integer);
begin
  Insert(GetCount, ACaption, Item, nIndex);
end;

procedure TDPopupMenu.Remove(Item: TDPopupMenu);
var
  I: Integer;
begin
  I := IndexOf(Item); if I >= 0 then Delete(I);
end;

procedure TDPopupMenu.Delete(Index: Integer);
begin
  FItems.Delete(Index);
end;

procedure TDPopupMenu.Clear;
begin
  FItemIndex := -1;
  while Count > 0 do begin
    Items[0].Free;
    Delete(0);
  end;
end;

function TDPopupMenu.Find(ACaption: string): TDPopupMenu;
var
  I: Integer;
begin
  Result := nil;
  ACaption := StripHotkey(ACaption);
  for I := 0 to Count - 1 do
    if AnsiSameText(ACaption, StripHotkey(Items[I].Caption)) then
    begin
      Result := Menus[I];
      System.Break;
    end;
end;

procedure TDPopupMenu.Show;
begin
  Visible := True;
  {if Floating then begin
    if DParent <> nil then
      DParent.ChangeChildOrder(Self);
  end;
  if EnableFocus then SetDFocus(Self); }
  ActiveMenu := Self;
end;

procedure TDPopupMenu.Show(d: TDControl);
begin
  //if Count = 0 then Exit;
  Visible := True;
  DControl := d;
 { if Floating then begin
    if DParent <> nil then
      DParent.ChangeChildOrder(Self);
  end;  }
  //if EnableFocus then SetDFocus(Self);
  ActiveMenu := Self;
end;

procedure TDPopupMenu.Hide;
var
  I: Integer;
begin
  //inherited;
  Visible := False;

  if ActiveMenu = Self then ActiveMenu := nil;
  if OwnerMenu <> nil then ActiveMenu := OwnerMenu;
  for I := 0 to Count - 1 do begin
    if (Menus[I] <> nil) { and (not Items[I].Visible)} then begin
      Menus[I].Hide;
    end;
  end;
end;

function TDPopupMenu.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDPopupMenu.Process;
var
  d: TDxSurface;
  I, n1C, n2C: Integer;
  rc: TRect;
  nX, nY: Integer;

begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  if not Assigned(ImageCanvas) then Exit;
  if not Assigned(MainForm) then Exit;

  SetImageFont();

  FItemSize := Round(ImageCanvas.TextHeight('0') * 1.5);

  n1C := 0;

  if FStyle = sVista then begin
    for I := 0 to FItems.Count - 1 do begin
      if n1C < ImageCanvas.TextWidth(FItems.Strings[I]) then
        n1C := ImageCanvas.TextWidth(FItems.Strings[I]);
    end;

    n1C := n1C + ImageCanvas.TextHeight('0') * 4;
    if n1C <> Width then Width := n1C;
  end;

  if FStyle = sVista then begin
    n2C := FItemSize * FItems.Count + ImageCanvas.TextHeight('0') * 2;
    if n2C <> Height then Height := n2C;
  end else begin
    n2C := FItemSize * FItems.Count + ImageCanvas.TextHeight('0');
    if n2C <> Height then Height := n2C;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).Process;
end;

procedure TDPopupMenu.DirectPaint(dsurface: TDxSurface);
var
  d: TDxSurface;
  I, n1C, n2C: Integer;
  rc: TRect;
  nX, nY: Integer;

  CColor: TColor;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

{------------------------------------------------------------------------------}
  rc := ClientRect;
  rc.Left := rc.Left + 1;
  rc.Right := rc.Right - 1;
  rc.Top := rc.Top + 1;
  rc.Bottom := rc.Bottom - 1;
  if FAlpha = 255 then
    dsurface.FillRect(rc, FColors.Background)
  else
    dsurface.FillRectAlpha(rc, FColors.Background, FAlpha);

  rc := ClientRect;
  if FDrawBorder then
    dsurface.FrameRect(rc, FColors.Border);
{------------------------------------------------------------------------------}
  rc.Left := rc.Left + dsurface.TextHeight('0') + 3;
  rc.Right := rc.Right - dsurface.TextHeight('0') - 3;

{------------------------------------------------------------------------------}
  if FItems.Count > 0 then begin
    if FStyle = sVista then begin

    end else begin
      if FMouseMove then begin
        if (FMoveItemIndex >= 0) and (FMoveItemIndex < FItems.Count) then begin
          if FItems.Strings[FMoveItemIndex] <> '-' then begin
            rc := ClientRect;
            rc.Left := rc.Left + 2;
            rc.Right := rc.Right - 2;
            rc.Top := rc.Top + dsurface.TextHeight('0') div 2 + FMoveItemIndex * FItemSize;
            rc.Bottom := rc.Top + FItemSize;

            if FAlpha = 255 then
              dsurface.FillRect(rc, FColors.Hot)
            else
              dsurface.FillRectAlpha(rc, FColors.Hot, FAlpha);

          end;
        end;
      end else
        if FMouseDown then begin
        if (FItemIndex >= 0) and (FItemIndex < FItems.Count) then begin
          if FItems.Strings[FItemIndex] <> '-' then begin
            rc := ClientRect;
            rc.Left := rc.Left + 2;
            rc.Right := rc.Right - 2;
            rc.Top := rc.Top + dsurface.TextHeight('0') div 2 + FItemIndex * FItemSize;
            rc.Bottom := rc.Top + FItemSize;
            if FAlpha = 255 then
              dsurface.FillRect(rc, FColors.Hot)
            else
              dsurface.FillRectAlpha(rc, FColors.Hot, FAlpha);
          end;
        end;
      end;
    end;
{------------------------------------------------------------------------------}
    nY := 0;
    rc := ClientRect;
    rc.Left := rc.Left + 4;
    rc.Right := rc.Right - 4;
    rc.Top := rc.Top + 4;
    rc.Bottom := rc.Bottom - 4;
    //rc.Left := rc.Left + dsurface.TextHeight('0') div 2;
    //rc.Right := rc.Right - dsurface.TextHeight('0') div 2;

    for I := 0 to FItems.Count - 1 do begin
      if FItems[I] = '-' then begin
        nY := (dsurface.TextHeight('0') div 2 + I * FItemSize) + FItemSize div 2;
        rc.Top := SurfaceY(Top) + nY;
        rc.Bottom := rc.Top + 1;
        dsurface.FrameRect(rc, FColors.Line);
      end;
    end;
{------------------------------------------------------------------------------}
    rc := ClientRect;
    if FStyle = sVista then begin
      nX := dsurface.TextHeight('0') * 2 + rc.Left;
    end else begin
      nX := rc.Left + 2;
    end;
{------------------------------------------------------------------------------}
    for I := 0 to FItems.Count - 1 do begin
      if FItems[I] <> '-' then begin
        CColor := FColors.Font;
        if Items[I].Enabled then begin
          if FMoveItemIndex = I then CColor := FColors.Selected
          else CColor := FColors.Font;
        end else begin
          CColor := FColors.Disabled;
        end;
        if FStyle = sVista then begin
          nY := (dsurface.TextHeight('0') + I * FItemSize) + (FItemSize - dsurface.TextHeight('0')) div 2;
        end else begin
          nY := (dsurface.TextHeight('0') div 2 + I * FItemSize) + (FItemSize - dsurface.TextHeight('0')) div 2;
        end;
        nY := SurfaceY(Top) + nY;
        dsurface.TextOut(nX, nY, FItems[I], CColor);
      end;
    end;
{------------------------------------------------------------------------------}
    if FStyle = sVista then begin
      if FMouseMove then begin
        if (FMoveItemIndex >= 0) and (FMoveItemIndex < FItems.Count) then begin
          if FItems.Strings[FMoveItemIndex] <> '-' then begin
            rc := ClientRect;
            rc.Left := rc.Left + 3;
            rc.Right := rc.Right - 3;
            rc.Top := rc.Top + dsurface.TextHeight('0') + FMoveItemIndex * FItemSize;
            rc.Bottom := rc.Top + FItemSize;
            dsurface.FillRectAlpha(Rect(rc.Left, rc.Top, rc.Right, rc.Top + FItemSize div 3), FColors.Hot, 60);
            dsurface.FillRectAlpha(Rect(rc.Left, rc.Top + FItemSize div 3, rc.Right, rc.Top + FItemSize), FColors.Hot, 100);
            dsurface.FrameRect(rc, FColors.Hot);
          end;
        end;
      end else
        if FMouseDown then begin
        if (FItemIndex >= 0) and (FItemIndex < FItems.Count) then begin
          if FItems.Strings[FItemIndex] <> '-' then begin
            rc := ClientRect;
            rc.Left := rc.Left + 3;
            rc.Right := rc.Right - 3;
            rc.Top := rc.Top + dsurface.TextHeight('0') + FItemIndex * FItemSize;
            rc.Bottom := rc.Top + FItemSize;

            dsurface.FillRectAlpha(Rect(rc.Left, rc.Top, rc.Right, rc.Top + FItemSize div 3), FColors.Hot, 100);
            dsurface.FillRectAlpha(Rect(rc.Left, rc.Top + FItemSize div 3, rc.Right, rc.Top + FItemSize), FColors.Down, 200);
            dsurface.FrameRect(rc, FColors.Hot);
          end;
        end;
      end;
    end;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;

function TDPopupMenu.KeyPress(var Key: Char): Boolean;
begin

end;

function TDPopupMenu.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin

end;

function TDPopupMenu.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) then begin
    Result := inherited MouseMove(Shift, X, Y);
    if MouseCaptureControl = Self then
      if InRange(X, Y) then Downed := True
      else Downed := False;
  end;
  FMouseMove := Result;
  if (FItemSize <> 0) and FMouseMove and (Count > 0) then begin
    FMoveItemIndex := (Y - ImageCanvas.TextHeight('0') - Top) div FItemSize;
    if (FMoveItemIndex >= 0) and (FMoveItemIndex < FItems.Count) then begin
      if Menus[FMoveItemIndex] <> FActiveMenu then begin
        if FActiveMenu <> nil then FActiveMenu.Hide;
        FActiveMenu := nil;
        if Items[FMoveItemIndex].Enabled then begin
          FActiveMenu := Menus[FMoveItemIndex];
          if (FActiveMenu <> nil) and (not FActiveMenu.Visible) then FActiveMenu.Show(Self);
        end;
      end;
    end else begin
      if FActiveMenu <> nil then FActiveMenu.Hide;
      FActiveMenu := nil;
      FMoveItemIndex := -1;
    end;
  end else FMoveItemIndex := -1;
end;

function TDPopupMenu.Click(X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
 { if (ActiveMenu <> nil) then begin
    if (ActiveMenu = Self) then begin

      if Assigned(FOnClick) then
        FOnClick(Self, X, Y);

      Result := True;
    end;
    Exit;
  end; }
  for I := DControls.Count - 1 downto 0 do
    if TDControl(DControls[I]).Visible then
      if TDControl(DControls[I]).Click(X - Left, Y - Top) then begin
        Result := True;
        Exit;
      end;
  if InRange(X, Y) then begin
    if Assigned(FOnClick) then
      FOnClick(Self, X, Y);
    Result := True;
  end;
end;

function TDPopupMenu.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    //if (not Background) and (MouseCaptureControl = nil) then begin
    Downed := True;
      //SetDCapture(Self);
   // end;
    Result := True;
  end;
  FMouseDown := Result;
  FMouseMove := Result;
  if (FItemSize <> 0) and FMouseDown and (Count > 0) then begin
    FItemIndex := (Y - ImageCanvas.TextHeight('0') - Top) div FItemSize;
  end;
end;

function TDPopupMenu.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    Result := True;
    Downed := False;
    FMouseDown := not Result;
    if InRange(X, Y) then begin
      if (FItemIndex >= 0) and (FItemIndex < Count) and Items[FItemIndex].Enabled then begin
        if (FActiveMenu <> nil) then begin
          if (not FActiveMenu.Visible) then FActiveMenu.Show(Self);
        end else Hide;
      end else if (Count <= 0) then Hide;
    end;
  end else begin
    ReleaseCapture;
    Downed := False;
  end;
end;

  //输入控件
{--------------------- TDEdit --------------------------}

constructor TDEdit.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  Downed := False;
  FOnClick := nil;
  FOnChange := nil;
  FMainMenu := nil;
  FEnableFocus := True;
  FClickSound := csNone;
  FInValue := vString;
  Width := 100;
  Height := 20;

  FText := 'DEdit'; //Some text
  Ticks := 0;

  FBlinkTicks := 1000;
  FReadOnly := False;
  FMaxLength := 0;

  bDoubleByte := False;
  KeyByteCount := 0;

  FPasswordText := '';
  FShowPasswordText := False;

  FBkgrndColor := clBlack;
  FSelectedColor := clWhite;
  FBorderColor := clWhite;
  FSelTextColor := $00DC802C;
  FFontColor := clWhite;
  FSelTextFontColor := clWhite;
  FDisabledColor := clBtnFace;

  Ticks := 0;
  FBlinkTicks := 4;
  //FPosTextWidth := 0;
  FSelectText := False;
  FPaste := False;
  FDrawBkgrnd := False;
  FDrawBorder := True;
  FPasswordChar := #0;
  FTextAdjust := 0;
  FSelIndex := 0;
  FSelText := '';
  FEndIndex := -1;
  FBeginIndex := -1;

  FBorderColor := $00608490;

  FHotBorder := $005894B8;
  FDownBorder := $005894B8;
end;

procedure TDEdit.Paint;
begin
  if csDesigning in ComponentState then begin
    with Canvas do begin
      Brush.Color := clWhite;
      FillRect(ClipRect);
      Pen.Color := cl3DDkShadow;
      MoveTo(0, 0);
      LineTo(Width - 1, 0);
      LineTo(Width - 1, Height - 1);
      LineTo(0, Height - 1);
      LineTo(0, 0);

      {Pen.Color := cl3DDkShadow;
      MoveTo(0, 0);
      LineTo(Width - 1, 0);

      MoveTo(Width - 1, Height - 1);
      Pen.Color := clCream;
      LineTo(Width - 1, 0);
      LineTo(0, Height - 1);

      Pen.Color := cl3DDkShadow;
      LineTo(0, Height - 1);
      LineTo(0, 0);   }
      TextOut(2 {(Width - TextWidth(Text)) div 2}, (Height - TextHeight(Text)) div 2, Text);
    end;
  end;
end;

//------------------------------------------------------------------------------

procedure TDEdit.SetViewPos(const Value: Integer);
var
  mSize: Integer;
begin
  FViewPos := Value;
  //if csDesigning in ComponentState then Exit;
  if Assigned(ImageCanvas) then begin
    mSize := 4 + ImageCanvas.TextWidth(Text) + (Height div 2);
  end else begin
    mSize := 4 + Canvas.TextWidth(Text) + (Height div 2);
  end;
  if (FViewPos > mSize - Width) then FViewPos := mSize - Width;
  if (FViewPos < 0) then FViewPos := 0;
end;


//------------------------------------------------------------------------------

procedure TDEdit.SetMaxLength(const Value: Integer);
begin
  FMaxLength := Value;
  if (FMaxLength > 0) and (Length(FText) > FMaxLength) then
  begin
    FText := Copy(FText, 1, FMaxLength);
    if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
    if FSelIndex < 0 then FSelIndex := 0;
  end;
end;

//---------------------------------------------------------------------------

procedure TDEdit.StripWrong(var Text: string);
var
  I: Integer;
begin
  Text := Trim(Text);
  for I := Length(Text) downto 1 do
    if (not (Text[I] in TextChars)) then
      Delete(Text, I, 1);
end;

//---------------------------------------------------------------------------

function TDEdit.GetSelText: WideString;
var
  I, Len: Integer;
  sText: string;
  P: PChar;
begin
  if FPasswordChar <> #0 then begin
    //for I := 1 to Length(FSelText) do Result := Result + FPasswordChar;
    //Result := '';
    Len := Length(FSelText);
    if Len > 0 then begin
      SetLength(sText, Len);
      P := PChar(sText);
      for I := 1 to Len do begin
        P^ := FPasswordChar;
        Inc(P);
      end;
      Result := sText;
    end else Result := '';
  end else begin
    Result := FSelText;
  end;
end;

//---------------------------------------------------------------------------

function TDEdit.GetText: WideString;
var
  I, Len: Integer;
  sText: string;
  P: PChar;
begin
  //Result := '';
  if FPasswordChar <> #0 then begin
    Len := Length(FText);
    if Len > 0 then begin
      SetLength(sText, Len);
      P := PChar(sText);
      for I := 1 to Len do begin
        P^ := FPasswordChar;
        Inc(P);
      end;
      Result := sText;
    end else Result := '';
  end else begin
    Result := FText;
  end;
end;

//---------------------------------------------------------------------------

function TDEdit.CharRect(Index: Integer): TRect;
var
  sText: WideString;
  TextBefore: WideString;
  TextAfter: WideString;
  sLeft, sRight: Integer;
  aCanvas: TCanvas;
begin
   //-------modi by huasoft-------------------------------------
   // (2) Extract part of text prior to selector
  sText := GetText;
  TextBefore := '';
  TextAfter := '';
  if (Index > 0) then TextBefore := Copy(sText, 1, Index);
  if (Index >= 0) then TextAfter := Copy(sText, 1, Index + 1);

   // (3) Determine selected position
  if Assigned(ImageCanvas) then begin

    sLeft := 0 + ImageCanvas.TextWidth(TextBefore);

    if (TextAfter <> '') and (Index < Length(sText)) then
      sRight := 0 + ImageCanvas.TextWidth(TextAfter)
    else sRight := sLeft + (Height div 2);
  end else begin
    sLeft := 0 + Canvas.TextWidth(TextBefore);

    if (TextAfter <> '') and (Index < Length(sText)) then
      sRight := 0 + Canvas.TextWidth(TextAfter)
    else sRight := sLeft + (Height div 2);
  end;
  //--------------------------------------------------------------------

   // (4) Determine selected rectangle
  Result.Left := sLeft;
  Result.Right := sRight;
  Result.Top := 2;
  Result.Bottom := Height - 2;
end;

//------------------------------------------------------------------------------

procedure TDEdit.ScrollToRight(Index: Integer);
var
  ChRect: TRect;
begin
  ChRect := CharRect(Index);
  if (ChRect.Right <= ChRect.Left) and (ChRect.Right = 0) then Exit;

  ViewPos := ChRect.Right - Width + 2;
end;

//---------------------------------------------------------------------------

procedure TDEdit.ScrollToLeft(Index: Integer);
var
  ChRect: TRect;
begin
  ChRect := CharRect(Index);
  if (ChRect.Right <= ChRect.Left) and (ChRect.Right = 0) then Exit;

  ViewPos := ChRect.Left - 2;
end;

//---------------------------------------------------------------------------

function TDEdit.NeedToScroll(): Boolean;
var
  ChRect, PaintRect, CutRect: TRect;
begin
  ChRect := CharRect(FSelIndex);
  if (ChRect.Right <= ChRect.Left) and (ChRect.Right = 0) then begin
    Result := False;
    Exit;
  end;

  PaintRect := ClientRect;
  CutRect := ShortRect(MoveRect(ChRect, Point(PaintRect.Left - FViewPos, 0)),
    PaintRect);
  Result := (CutRect.Right - CutRect.Left) < (ChRect.Right - ChRect.Left);
end;

//---------------------------------------------------------------------------

procedure TDEdit.SelectChar(const MousePos: TPoint);
var
  I, Search: Integer;
  ChRect, vRect, PaintRect: TRect;
  RelPoint: TPoint;
  sText: WideString;
begin
  RelPoint.X := MousePos.X - Left;
  RelPoint.Y := MousePos.Y - Top;
  sText := GetText;
  if RelPoint.X <= 0 then begin
    Search := 0;
  end else begin
    Search := Length(sText);
  end;
  for I := 0 to Length(sText) do begin
    ChRect := CharRect(I);
    if (ChRect.Right <= ChRect.Left) {and (ChRect.Right = 0) } then Exit;

    vRect := MoveRect(ChRect, Point(-FViewPos, 0));
    if (PointInRect(RelPoint, vRect)) then begin
      Search := I;
      Break;
    end;
  end;

  if sText <> '' then begin
    if (FBeginIndex = FEndIndex) and (FBeginIndex <> FSelIndex) then begin
      FBeginIndex := Search;
      FEndIndex := Search;
    end else begin
      FEndIndex := Search;
      if FBeginIndex < FEndIndex then begin
        FSelText := Copy(FText, FBeginIndex + 1, FEndIndex - FBeginIndex);
      end else begin
        FSelText := Copy(FText, FEndIndex + 1, FBeginIndex - FEndIndex);
      end;
    end;
    FSelIndex := Search;
  end else begin
    FBeginIndex := -1;
    FEndIndex := -1;
    FSelText := '';
    FSelIndex := 0;
  end;

  if (NeedToScroll()) then begin
    if (MousePos.X >= Width div 2) then ScrollToRight(FSelIndex)
    else ScrollToLeft(FSelIndex);
  end;
end;

//==============================================================================

procedure TDEdit.SetSelText(const Value: WideString);
begin

end;

//==============================================================================

procedure TDEdit.SetSelIndex(const Value: Integer);
begin
  if FSelIndex <> Value then begin
    FSelIndex := Value;

    if (FSelIndex > Length(FText)) then FSelIndex := Length(Text);
    if (FSelIndex < 0) then FSelIndex := 0;
    FSelText := '';
    FEndIndex := -1;
    FBeginIndex := -1;
  end;
end;

//==============================================================================

procedure TDEdit.SetSelLength(const Value: Integer);
begin
  if abs(FBeginIndex - FEndIndex) <> Value then begin
    if (FSelIndex >= Length(FText)) then begin
      FSelText := '';
      FEndIndex := -1;
      FBeginIndex := -1;
    end else begin
      FBeginIndex := FSelIndex;
      FEndIndex := Min(Length(FText), Value);
      FSelText := Copy(FText, FBeginIndex + 1, FEndIndex - FBeginIndex);
    end;
  end;
end;

//==============================================================================

function TDEdit.GetSelLength: Integer;
begin
  Result := abs(abs(FBeginIndex) - abs(FEndIndex));
end;

//==============================================================================

function TDEdit.KeyPress(var Key: Char): Boolean;
var
  Ch: Char;
  wCh: WideChar;
  AddTx: string;
  nBeginIndex: Integer;
  nEndIndex: Integer;
begin
  Result := False;
  if inherited KeyPress(Key) then begin
    if (FReadOnly) or (not Enabled) then Exit;
    Result := True;
    Ch := Key;

    if (IsDBCSLeadByte(Ord(Key)) or bDoubleByte) then begin
      if FInValue = vString then begin
        bDoubleByte := True;
        Inc(KeyByteCount);
        InputStr := InputStr + Ch;
      end else begin
        InputStr := '';
        Ch := #0;
        Exit;
      end;
    end;

    if FInValue = vInteger then begin
      if not (Ch in ['0'..'9']) then Exit;
    end;

    if (Ch in TextChars) then begin
      if (FSelText <> '') then begin
        nBeginIndex := FBeginIndex;
        nEndIndex := FEndIndex;
        ChgNumber(nBeginIndex, nEndIndex);
        if (nEndIndex = FSelIndex) and (nEndIndex <= Length(FText)) then Dec(FSelIndex, Length(FSelText));
        Delete(FText, nBeginIndex + 1, Length(FSelText));
        FSelText := '';
        FEndIndex := -1;
        FBeginIndex := -1;
        if (FSelIndex < 0) then FSelIndex := 0;
        if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
        if (NeedToScroll()) then ScrollToLeft(FSelIndex);
      end;
      if (FMaxLength < 1) or (Length(FText) < FMaxLength) then begin
        if not bDoubleByte then begin
          if FInValue = vInteger then begin
            if not IsStringNumber(Ch) then begin
              Ch := #0;
              Exit;
            end;
          end;
          if (FText = '') or (FSelIndex >= Length(FText)) then FText := FText + Ch
          else Insert(Ch, FText, FSelIndex + 1);
          Inc(FSelIndex);
        end else
          if (KeyByteCount >= 2) then begin
          if (FText = '') or (FSelIndex >= Length(FText)) then FText := FText + InputStr
          else Insert(InputStr, FText, FSelIndex + 1);
          bDoubleByte := False;
          KeyByteCount := 0;
          InputStr := '';
          Inc(FSelIndex);
        end;

        if (FSelIndex < 0) then FSelIndex := 0;
        if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
        if (NeedToScroll()) then ScrollToRight(FSelIndex);
        if (Assigned(FOnChange)) then FOnChange(Self);
      end;
    end;
  end;
end;

procedure TDEdit.EnterKey(var Key: Word);
begin
  case Key of
    VK_BACK,
      byte('D'): DeleteText;
    byte('C'): CopyText;
    byte('X'): CutText;
    byte('Z'): ;
    byte('V'): PasteText;
    byte('A'): SelectAll;
  end;
end;

function TDEdit.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin
  Result := False;
  if inherited KeyDown(Key, Shift) and Enabled then begin
    Result := True;
    case Key of
      VK_RIGHT:
        begin
          if (FSelIndex < Length(Text)) then Inc(FSelIndex);
          if (NeedToScroll()) then ScrollToRight(FSelIndex);
        end;
      VK_LEFT:
        begin
          if (FSelIndex > 0) then Dec(FSelIndex);
          if (NeedToScroll()) then ScrollToLeft(FSelIndex);
        end;
      VK_BACK: EnterKey(Key);

      VK_DELETE:
        if (not FReadOnly) then begin
          Delete(FText, FSelIndex, 1);
         { if (FInValue = vInteger) and (FText = '') then begin
            FText := '0';
          end; }
          if (FSelIndex < 0) then FSelIndex := 0;
          if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
          if (Assigned(FOnChange)) then FOnChange(Self);
        end;

      VK_HOME: begin
          FSelIndex := 0;
          if (NeedToScroll()) then ScrollToLeft(FSelIndex);
        end;

      VK_END: begin
          FSelIndex := Length(Text);
          if (NeedToScroll()) then ScrollToRight(FSelIndex);
        end;
      VK_RETURN: ; //if (Owner <> nil) and (not Owner.CallNextKeyEvent) then Exit;
    end;
  end;
  if (ssCtrl in Shift) and Enabled then EnterKey(Key);
end;

procedure TDEdit.DeleteText;
var
  nBeginIndex: Integer;
  nEndIndex: Integer;
begin
  if not FReadOnly then begin
    //Showmessage('TDEdit.DeleteText');
    if (FSelText <> '') then begin
      nBeginIndex := FBeginIndex;
      nEndIndex := FEndIndex;
      ChgNumber(nBeginIndex, nEndIndex);
      if (nEndIndex = FSelIndex) and (nEndIndex <= Length(FText)) then
        Dec(FSelIndex, Length(FSelText));
      Delete(FText, nBeginIndex + 1, Length(FSelText));
      FSelText := '';
      FEndIndex := -1;
      FBeginIndex := -1;
     { if (FInValue = vInteger) and (FText = '') then begin
        FText := '0';
      end; }
      if (FSelIndex < 0) then FSelIndex := 0;
      if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
      if (NeedToScroll()) then ScrollToLeft(FSelIndex);
    end else begin
      //Showmessage('TDEdit.DeleteText: FText:'+IntToStr(Length(FText))+' FSelIndex:'+IntToStr(FSelIndex));
      Delete(FText, FSelIndex, 1);
      {if (FInValue = vInteger) and (FText = '') then begin
        FText := '0';
      end;    }
      if (FSelIndex > 0) then Dec(FSelIndex);
      if (FSelIndex < 0) then FSelIndex := 0;
      if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
      if (NeedToScroll()) then ScrollToRight(FSelIndex);
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
end;

procedure TDEdit.CopyText;
var
  Clipboard: TClipboard;
begin
  if (FSelText <> '') then begin
    Clipboard := TClipboard.Create();
    Clipboard.AsText := string(FSelText);
    Clipboard.Free();
  end;
end;

procedure TDEdit.CutText;
var
  Clipboard: TClipboard;
  nBeginIndex: Integer;
  nEndIndex: Integer;
begin
  if FReadOnly then Exit;
  if (FSelText <> '') then begin
    nBeginIndex := FBeginIndex;
    nEndIndex := FEndIndex;
    ChgNumber(nBeginIndex, nEndIndex);
    if (nEndIndex = FSelIndex) and (nEndIndex < Length(FText)) then
      Dec(FSelIndex, Length(FSelText));
    Delete(FText, nBeginIndex + 1, Length(FSelText));
    if (FSelIndex < 0) then FSelIndex := 0;
    Clipboard := TClipboard.Create();
    Clipboard.AsText := string(FSelText);
    Clipboard.Free();

    FSelText := '';
    FEndIndex := -1;
    FBeginIndex := -1;

    if (NeedToScroll()) then ScrollToLeft(FSelIndex);
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
end;

procedure TDEdit.PasteText;
var
  Clipboard: TClipboard;
  AddTx: string;
  TextBefore: WideString;
  TextAfter: WideString;
  I: Integer;
  nBeginIndex: Integer;
  nEndIndex: Integer;
begin
  if FReadOnly or (not FPaste) then Exit;
  nBeginIndex := FBeginIndex;
  nEndIndex := FEndIndex;
  ChgNumber(nBeginIndex, nEndIndex);

  if (FSelText <> '') then begin
    if (nEndIndex = FSelIndex) and (nEndIndex <= Length(FText)) then
      Dec(FSelIndex, Length(FSelText));
    Delete(FText, nBeginIndex + 1, Length(FSelText));
    if (FSelIndex < 0) then FSelIndex := 0;
    FSelText := '';
    FEndIndex := -1;
    FBeginIndex := -1;
    if (NeedToScroll()) then ScrollToLeft(FSelIndex);
  end;
  Clipboard := TClipboard.Create();

  AddTx := Clipboard.AsText;
  StripWrong(AddTx);
  if FInValue = vInteger then begin
    if not IsStringNumber(AddTx) then AddTx := '0';
  end;
  Insert(AddTx, FText, FSelIndex + 1);
  Inc(FSelIndex, Length(AddTx));

  if (FMaxLength > 0) and (Length(FText) > FMaxLength) then
  begin
    FText := Copy(FText, 1, FMaxLength);
    if (FSelIndex > Length(FText)) then FSelIndex := Length(FText);
  end;

  if (NeedToScroll()) then ScrollToRight(FSelIndex);

  Clipboard.Free();

  if (Assigned(FOnChange)) then FOnChange(Self);
end;

function TDEdit.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) then begin
    Result := inherited MouseMove(Shift, X, Y);
    if MouseCaptureControl = Self then
      if InRange(X, Y) then Downed := True
      else Downed := False;
  end;

  if Result and Downed and (ssLeft in Shift) then begin
    if FSelectText then begin
      SelectChar(Point(X, Y));
    end else begin
      FBeginIndex := -1;
      FEndIndex := -1;
      FSelText := '';
      SelectChar(Point(X, Y));
    end;
  end;
end;

function TDEdit.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  d: TDControl;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    if (MouseCaptureControl = nil) then begin
      Downed := True;
      SetDCapture(Self);
    end;
    if (Button = mbLeft) and Enabled then begin
      FSelText := '';
      FBeginIndex := -1;
      FEndIndex := -1;
      SelectChar(Point(X, Y));
    end;
    Result := True;
  end;
end;

function TDEdit.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  Clipboard: TClipboard;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    ReleaseDCapture;
    if not Background then begin
      if InRange(X, Y) and Enabled then begin
        if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
        if Assigned(FOnClick) then FOnClick(Self, X, Y);
        if Assigned(MainMenu) and (Button = mbRight) then begin
          if SelText = '' then begin
            MainMenu.Items[0].Enabled := False;
            MainMenu.Items[1].Enabled := False;
            MainMenu.Items[3].Enabled := False;
          end else begin
            MainMenu.Items[0].Enabled := True;
            MainMenu.Items[1].Enabled := True;
            MainMenu.Items[3].Enabled := True;
          end;
          MainMenu.Items[5].Enabled := Text <> '';

          Clipboard := TClipboard.Create;
          if Clipboard.AsText <> '' then MainMenu.Items[2].Enabled := True else
            MainMenu.Items[2].Enabled := False;
          Clipboard.Free;

          if SurfaceX(X) + MainMenu.Width > SCREENWIDTH then MainMenu.Left := SurfaceX(X) - MainMenu.Width
          else MainMenu.Left := SurfaceX(X);
          if SurfaceY(Y) + MainMenu.Height > SCREENHEIGHT then MainMenu.Top := SurfaceY(Y) - MainMenu.Height
          else MainMenu.Top := SurfaceY(Y);
          MainMenu.Show(Self);
        end;
      end;
    end;
    Downed := False;
    Result := True;
    Exit;
  end else begin
    ReleaseDCapture;
    Downed := False;
  end;
end;

procedure TDEdit.SetText(const Value: WideString);
var
  X, nSelCount: Integer;
  sText: string;
  Changed: Boolean;
begin
  Changed := (Value <> FText);
  FText := Value;
  if (FMaxLength > 0) and (Length(FText) > FMaxLength) then
    FText := Copy(FText, 1, FMaxLength);
  //sText := Value;
  //StripWrong(sText);

  FText := Value;
  if FInValue = vInteger then begin
    if not IsStringNumber(FText) then FText := '0';
  end;

  FSelIndex := Length(FText);
  FSelText := '';
  FBeginIndex := -1;
  FEndIndex := -1;
  FViewPos := 0;
  if csDesigning in ComponentState then Refresh;
  if (Assigned(FOnChange)) and Changed then FOnChange(Self);
end;

procedure TDEdit.SelectAll;
begin
  if not FSelectText then Exit;
  if FSelText <> FText then begin
    FBeginIndex := 0;
    FEndIndex := Length(FText);
    FSelText := FText;
  end;
end;

procedure TDEdit.DrawText(dsurface: TDxSurface; const vRect: TRect);
var
  PaintRect: TRect;
  RectText: TRect;
  TextTop: Integer;
  sText: Widestring;
  nTextWidth: Integer;
  nTextHeight: Integer;
  ARect, BRect, CRect: TRect;
  AText, BText, CText: string;
begin
  AText := '';
  BText := '';
  CText := '';
  FillChar(ARect, SizeOf(TRect), 0);
  FillChar(BRect, SizeOf(TRect), 0);
  FillChar(CRect, SizeOf(TRect), 0);

  sText := GetText;
  if sText <> '' then begin
    PaintRect := vRect;
    RectText := ShortRect(ShrinkRect(PaintRect, 2, 0), PaintRect);
    if (RectText.Right > RectText.Left) and (RectText.Bottom > RectText.Top) then begin
      PaintRect := vRect;
      TextTop := dsurface.TextHeight(sText);
      if (TextTop > 0) then
        TextTop := ((PaintRect.Bottom - PaintRect.Top) - TextTop) div 2;

      if FBeginIndex <> FEndIndex then begin
        if FBeginIndex < FEndIndex then begin
          BText := Copy(sText, FBeginIndex + 1, FEndIndex - FBeginIndex);
          AText := Copy(sText, 1, FBeginIndex);
          CText := Copy(sText, FEndIndex + 1, Length(sText) - FEndIndex);
        end else begin
          BText := Copy(sText, FEndIndex + 1, FBeginIndex - FEndIndex);
          AText := Copy(sText, 1, FEndIndex);
          CText := Copy(sText, FBeginIndex + 1, Length(sText) - FBeginIndex);
        end;

        nTextHeight := dsurface.TextHeight('pP');
        nTextWidth := dsurface.TextWidth(AText);

        if nTextWidth > FViewPos then begin
          ARect.Left := FViewPos;
          ARect.Top := 0;
          ARect.Right := ARect.Left + Min(nTextWidth - FViewPos, RectText.Right - RectText.Left);
          ARect.Bottom := nTextHeight;
        end;

        if ARect.Right - ARect.Left < RectText.Right - RectText.Left then begin
          if nTextWidth < FViewPos then
            BRect.Left := FViewPos - nTextWidth
          else
            BRect.Left := 0;

          BRect.Top := 0;
          BRect.Right := BRect.Left + Min((RectText.Right - RectText.Left) - (ARect.Right - ARect.Left), dsurface.TextWidth(BText));
          BRect.Bottom := nTextHeight;
        end;

        CRect.Left := 0;
        CRect.Top := 0;
        CRect.Right := CRect.Left + Min((RectText.Right - RectText.Left) - (ARect.Right - ARect.Left) - (BRect.Right - BRect.Left), dsurface.TextWidth(CText));
        CRect.Bottom := nTextHeight;
      end else begin
        AText := sText;
        ARect.Left := FViewPos;
        ARect.Top := 0;
        ARect.Right := ARect.Left + Min(RectText.Right - RectText.Left, dsurface.TextWidth(AText));
        ARect.Bottom := dsurface.TextHeight(sText);
      end;

      ImageFont.DrawEditText(dsurface, PaintRect.Left + 2, PaintRect.Top + TextTop + FTextAdjust,
        ARect, BRect, CRect, AText, BText, CText,
        FontColor, SelTextFontColor, SelTextColor);
    end;
  end;
end;
(*procedure TDEdit.DrawText(dsurface: TDxSurface; const vRect: TRect);
var
  ChRect: TRect;
  PaintRect: TRect;
  RectText: TRect;
  SelRect: TRect;
  TextTop: Integer;
  PrevRect: TRect;
  sText: WideString;
  nIndex, nBeginIndex, nEndIndex: Integer;
begin
  PaintRect := vRect;
  sText := GetText;
  RectText := ShortRect(ShrinkRect(PaintRect, 2, 0), ClientRect);
  if (RectText.Right > RectText.Left) and (RectText.Bottom > RectText.Top) then begin
    TextTop := dsurface.TextHeight(sText);
    if (TextTop > 0) then
      TextTop := ((PaintRect.Bottom - PaintRect.Top) - TextTop) div 2;

    if (FBeginIndex <> FEndIndex) then begin //画选择的TEXT
      nBeginIndex := FBeginIndex;
      nEndIndex := FEndIndex;

      if FBeginIndex > FEndIndex then begin
        nBeginIndex := FEndIndex;
        nEndIndex := FBeginIndex;
      end;

      SelRect := RectText;

      {if nBeginIndex < nEndIndex then begin
        ChRect := CharRect(nBeginIndex);
        SelRect.Left := PaintRect.Left + Max((ChRect.Left - FViewPos), 0) + 1;
        ChRect := Bounds(PaintRect.Left, RectText.Top, SelRect.Left - PaintRect.Left, SelRect.Bottom - SelRect.Top);
        dsurface.TextRect(ChRect, PaintRect.Left + 2 - FViewPos, PaintRect.Top + TextTop + FTextAdjust, sText, FontColor);


        ChRect := CharRect(nEndIndex);
        SelRect.Right := PaintRect.Left + Min((ChRect.Left - FViewPos), Width) + 2;
        ChRect := Bounds(SelRect.Right, PaintRect.Top, RectText.Right - SelRect.Right, SelRect.Bottom - SelRect.Top);
        dsurface.TextRect(ChRect, PaintRect.Left + 2 - FViewPos, PaintRect.Top + TextTop + FTextAdjust, sText, FontColor);
      end;
      //SelRect := ShrinkRect(SelRect, 1, 1);
      dsurface.TextOut(SelRect.Left + 1, SelRect.Top + TextTop + FTextAdjust, GetSelText, SelTextFontColor, SelTextColor);
      //dsurface.FillRect(SelRect, SelTextColor);
    end else
      dsurface.TextRect(RectText, PaintRect.Left + 2 - FViewPos, PaintRect.Top + TextTop + FTextAdjust, sText, FontColor);   }


      if nBeginIndex < nEndIndex then begin
        ChRect := CharRect(nBeginIndex);
        SelRect.Left := PaintRect.Left + Max((ChRect.Left - FViewPos), 0) + 1;

        ChRect := CharRect(nEndIndex);
        SelRect.Right := PaintRect.Left + Min((ChRect.Left - FViewPos), Width) + 2;
      end;
      SelRect := ShrinkRect(SelRect, 1, 1);
      //dsurface.TextOut(SelRect.Left + 1, SelRect.Top + TextTop + FTextAdjust, GetSelText, SelTextFontColor, SelTextColor);
      dsurface.FillRect(SelRect, SelTextColor);
    end;
    dsurface.TextRect(RectText, PaintRect.Left + 2 - FViewPos, PaintRect.Top + TextTop + FTextAdjust, sText, FontColor);
  end;
end;

*)

procedure TDEdit.DrawSelector(dsurface: TDxSurface; const vRect: TRect);
var
  sLeft, sRight: Integer;
  TextBefore: Widestring;
  TextAfter: Widestring;
  SelRect: TRect;
  sText: Widestring;
begin
 // (1) Extract part of text prior to selector
  TextBefore := '';
  TextAfter := '';
  sText := GetText;
//  if (FSelIndex > 0) then TextBefore := Copy(FText, 1, FSelIndex);
//  if (FSelIndex >= 0) then TextAfter := Copy(FText, 1, FSelIndex + 1);
  if (FSelIndex >= 0) then TextBefore := Copy(sText, FSelIndex + 1, Length(sText));
  if (FSelIndex >= 0) then TextAfter := Copy(sText, FSelIndex + 2, Length(sText));

 // (2) Determine selector position
  if Assigned(ImageCanvas) then begin
    sLeft := 2 + ImageCanvas.TextWidth(sText) - ImageCanvas.TextWidth(TextBefore) - FViewPos;
    if (TextAfter <> '') and (FSelIndex < Length(sText)) then
      sRight := 2 + ImageCanvas.TextWidth(sText) - ImageCanvas.TextWidth(TextAfter) - FViewPos
    else sRight := sLeft + (Height div 2);
  end;

  SelRect.Left := sLeft + vRect.Left;

  SelRect.Right := SelRect.Left + 2;

  SelRect.Top := FTextAdjust + vRect.Top + 2;
  SelRect.Bottom := FTextAdjust + vRect.Bottom - 2;

  SelRect := ShortRect(SelRect, vRect);

  if (SelRect.Right > SelRect.Left) and (SelRect.Bottom > SelRect.Top) then
    dsurface.FillRect(SelRect, SelectedColor);
end;

//---------------------------------------------------------------------------

function TDEdit.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDEdit.Process;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  FBorderCurrColor := FBorderColor;
  if Enabled then begin
    if MouseDownControl = Self then FBorderCurrColor := FDownBorder;
    if MouseMoveControl = Self then FBorderCurrColor := FHotBorder;
  end;
end;

procedure TDEdit.DirectPaint(dsurface: TDxSurface);
var
  I: Integer;
  d: TDxSurface;
  PaintRect: TRect;
  TextRect: TRect;
  PrevRect: TRect;
  BorderCurrColor: TColor;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  //PaintRect := ClientRect;

  D := TDxSurface.Create(ImageCanvas.DDraw);
  D.SetSize(Width, Height);
  PaintRect := Bounds(0, 0, Width, Height);
  if FDrawBkgrnd then
    D.FillRect(PaintRect, FBkgrndColor);

  //if FDrawBkgrnd then
    //dsurface.FillRect(PaintRect, FBkgrndColor);

  if FDrawBorder then
    D.FrameRect(PaintRect, FBorderCurrColor);
    //dsurface.FrameRect(PaintRect, FBorderCurrColor);

  DrawText(D, PaintRect);
  //DrawText(dsurface, PaintRect);

  if (FocusedControl = Self) and Enabled then begin
    Inc(Ticks);
    if (FBlinkTicks = 0) or ((Ticks div FBlinkTicks) and $01 = 0) then begin
      DrawSelector(D, PaintRect);
      //DrawSelector(dsurface, PaintRect);
    end;
  end else begin
    FBeginIndex := -1;
    FEndIndex := -1;
    FSelText := '';
  end;

  dsurface.Draw(SurfaceX(Left), SurfaceY(Top), D.ClientRect, D, True);

  D.Free;
end;


constructor TDCombobox.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FItems := TStringList.Create();
  TStringList(FItems).OnChange := ItemChanged;
  FAutoSize := False;
  Downed := False;
  FOnClick := nil;
  FEnableFocus := True;
  FClickSound := csNone;
  Caption := Name;
  FUpColor := Canvas.Font.Color;
  FHotColor := Canvas.Font.Color;
  FDownColor := Canvas.Font.Color;
  FBackgroundColor := clWhite;

  FBorderColor := $00608490;

  FHotBorder := $005894B8;
  FDownBorder := $005894B8;
  FButtonColor := $00488184;
  FCurrColor := FUpColor;

  FBackground := False;
  FBorder := True;
  FMainMenu := nil;
  FOnChange := nil;
  FOnPopup := nil;
  FAlignment := taCenter;
end;

procedure TDCombobox.SetAlignment(Value: TAlignment);
begin
  if FAlignment <> Value then begin
    FAlignment := Value;
  end;
end;

destructor TDCombobox.Destroy;
begin
  FItems.Free;
  inherited Destroy;
end;

function TDCombobox.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDCombobox.SetUpColor(Value: TColor);
begin
  if FUpColor <> Value then begin
    FUpColor := Value;
    FCurrColor := Value;
  end;
end;

function TDCombobox.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) then begin
    Result := inherited MouseMove(Shift, X, Y);
    if MouseCaptureControl = Self then
      if InRange(X, Y) then Downed := True
      else Downed := False;
    if Result and (not Downed) then FCurrColor := FHotColor;
  end;
end;

function TDCombobox.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  d: TDControl;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    if (not Background) and (MouseCaptureControl = nil) then begin
      Downed := True;
      SetDCapture(Self);
      if Assigned(FMainMenu) then begin

        FMainMenu.Left := SurfaceX(Left);
        if SurfaceY(Top + Height) + FMainMenu.Height > SCREENHEIGHT then FMainMenu.Top := SurfaceY(Top - FMainMenu.Height)
        else FMainMenu.Top := SurfaceY(Top + Height);

        if Assigned(FOnPopup) then FOnPopup(Self);

        FMainMenu.OnClick := MainMenuClick;
        FMainMenu.MenuItems := Items;
        FMainMenu.Show(Self);
      end;
     // end;
    end;
    Result := True;
  end;
end;

function TDCombobox.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    ReleaseDCapture;
    if not Background then begin
      if InRange(X, Y) and Enabled then begin
        if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
        if Assigned(FOnClick) then FOnClick(Self, X, Y);
      end;
    end;
    Downed := False;
    Result := True;
    Exit;
  end else begin
    ReleaseDCapture;
    Downed := False;
  end;
end;

procedure TDCombobox.CreateWnd;
begin
  inherited;
  if FItems = nil then FItems := TStringList.Create();
  TStringList(FItems).OnChange := ItemChanged;
end;

function TDCombobox.GetItems: TStrings;
begin
  if csDesigning in ComponentState then Refresh;
  Result := FItems;
end;

procedure TDCombobox.SetItems(Value: TStrings);
begin
  FItems.Clear;
  FItems.Assign(Value);
end;

procedure TDCombobox.SetItemIndex(Value: Integer);
begin
  if (FMainMenu <> nil) {and (Value >= 0) and (Value < FItems.Count)} then begin
    FMainMenu.MenuItems := Items;
    FMainMenu.ItemIndex := Value;
    if (FMainMenu.ItemIndex >= 0) and (FMainMenu.ItemIndex < FMainMenu.Count) and (FMainMenu.MenuItems[FMainMenu.ItemIndex] <> '-') then
      Caption := FMainMenu.MenuItems[FMainMenu.ItemIndex]
    else
      Caption := '';
  end;
end;

function TDCombobox.GetItemIndex: Integer;
begin
  if (FMainMenu <> nil) then
    Result := FMainMenu.ItemIndex
  else Result := -1;
end;

procedure TDCombobox.MainMenuClick(Sender: TObject; X, Y: Integer);
begin
  if (FMainMenu.ItemIndex >= 0) and (FMainMenu.MenuItems[FMainMenu.ItemIndex] <> '-') then
    Caption := FMainMenu.MenuItems[FMainMenu.ItemIndex];
  if Assigned(FOnChange) then FOnChange(Self);
end;

procedure TDCombobox.ItemChanged(Sender: TObject);
begin
  //Caption := '';
  ItemIndex := GetItemIndex;
end;

procedure TDCombobox.CaptionChaged;
begin
  if Assigned(ImageCanvas) and Assigned(MainForm) then begin
    if not (csDesigning in ComponentState) then begin

      SetImageFont(Font.Size, fsBold in Font.Style);

      if FAutoSize then begin
        Width := ImageCanvas.TextWidth(Caption);
        Height := ImageCanvas.TextHeight('0');
      end;

      Width := Max(Width, 6);
      Height := Max(Height, 20);

      SetImageFont();
    end;
  end;
end;

procedure TDCombobox.Process;
var
  I: Integer;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  if Downed then begin
    FCurrColor := FDownColor;
  end else
    if MouseMoveing then begin
    FCurrColor := FHotColor;
  end else begin
    FCurrColor := FUpColor;
  end;

  FBorderCurrColor := FBorderColor;
  if Enabled then begin
    if MouseDownControl = Self then FBorderCurrColor := FDownBorder;
    if MouseMoveControl = Self then FBorderCurrColor := FHotBorder;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).Process;
end;

procedure TDCombobox.DirectPaint(dsurface: TDxSurface);
var
  I, nX, X1, Y1, X2, Y2: Integer;
  nAlpha: Integer;
  d: TDxSurface;

  ARect: TRect;
  BorderCurrColor: TColor;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  SetImageFont();

  if FBackground then
    dsurface.FillRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), FBackgroundColor);
  if FBorder then
    dsurface.FrameRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), FBorderCurrColor);

  if FCaption <> '' then begin
    case FAlignment of
      taLeftJustify: nX := SurfaceX(Left);
      taRightJustify: nX := Max(SurfaceX(Left), SurfaceX(Left + (Width - 16 - dsurface.TextWidth(FCaption))));
      taCenter: nX := SurfaceX(Left) + (Width - 16 - dsurface.TextWidth(FCaption)) div 2;
    end;
    dsurface.TextOut(nX,
      SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, FCaption, FCurrColor);
  end;

//画三角形
{-------------------------------------------------------------------------------}
  X1 := SurfaceX(Left) + (Width - 16) + 5;
  Y1 := SurfaceY(Top) + (Height - 5) div 2;

  if Downed then Y1 := Y1 + 1;
  X2 := X1 + 3;
  Y2 := Y1 + 4;

  for I := X1 to X1 + 6 do begin
    dsurface.Line(I, Y1, X2, Y2, FButtonColor);
  end;
{-------------------------------------------------------------------------------}

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;

constructor TDCheckBox.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FButtonStyle := bsCheckBox;
  EnableFocus := False;
  FUpColor := clSilver;
  FHotColor := clWhite;
  FDownColor := clWhite;
  FAlignment := taRightJustify;
end;

procedure TDCheckBox.SetAlignment(Value: TAlignment);
begin
  if FAlignment <> Value then begin
    FAlignment := Value;
  end;
end;

function TDCheckBox.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
  d: TDxSurface;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin
    boInrange := True;
    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDCheckBox.SetImgIndex(Lib: TGameImages; Index: Integer);
var
  d: TDxSurface;
begin
  if Lib <> nil then begin
    d := Lib.Images[Index];
    WLib := Lib;
    FaceIndex := Index;
    if d <> nil then begin
      if Assigned(ImageCanvas) then
        Width := d.Width + ImageCanvas.TextWidth(Caption)
      else
        Width := d.Width;
      Height := d.Height;
    end;
  end;
end;

procedure TDCheckBox.CaptionChaged;
begin
  if Assigned(ImageCanvas) then begin
    Width := Max(Width, Width + ImageCanvas.TextWidth(Caption));
    Height := Max(Height, Height + ImageCanvas.TextHeight('0'));
  end;
end;

procedure TDCheckBox.Process;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);
  Font.Color := FUpColor;

  if Enabled then begin
    if MouseDownControl = Self then Font.Color := FDownColor;
    if MouseMoveControl = Self then Font.Color := FHotColor;
  end;
end;

procedure TDCheckBox.DirectPaint(dsurface: TDxSurface);
var
  I: Integer;
  d: TDxSurface;
  X: Integer;
begin
  d := nil;
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface);

  X := SurfaceX(Left);
  if (WLib <> nil) and (Caption <> '') then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      Width := d.Width + dsurface.TextWidth(Caption);
      case FAlignment of
        taLeftJustify: ;
        taRightJustify: X := X + d.Width;
      end;
    end;
  end;

  dsurface.TextOut(X, SurfaceY(Top) + (Height - dsurface.TextHeight('0')) div 2, FCaption, Font.Color);

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);
end;
{-------------------------------------------------------------------------------}

constructor TDPageControl.Create(AOwner: TComponent);
begin
  inherited Create(Aowner);
  Tabs := TList.Create;
  FActivePage := 0;
  Width := 120;
  Height := 120;
  FTabRect := Bounds(Left, Top, Width, Height);
end;

destructor TDPageControl.Destroy;
begin
  Tabs.Free;
  inherited Destroy;
end;

procedure TDPageControl.WMSize(var Message: TWMSize);
begin
  inherited;
  {if csLoading in ComponentState then
    Exit;

  if (FActivePage <> -1) and (FActivePage >= 0) and (FActivePage < Tabs.Count) then
  begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
    if (csDesigning in ComponentState) then
      Invalidate;
  end;}
end;

procedure TDPageControl.ReadState(Reader: TReader);
begin
  inherited ReadState(Reader);
  {Tabs.Clear;
  inherited ReadState(Reader);
  if (FActivePage <> -1) and (FActivePage >= 0) and (FActivePage < Tabs.Count)
    then begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
  end else
    FActivePage := -1;}
end;

procedure TDPageControl.ShowControl(AControl: TControl);
//var
  //I: Integer;
begin
  {for I := 0 to Tabs.Count - 1 do begin
    if Tabs[I] = AControl then begin
      ActivePage := I;
      Exit;
    end;
  end; }
  inherited ShowControl(AControl);
end;

procedure TDPageControl.SetActivePage(Value: Integer);
var
  Index: Integer;
  ParentForm: TCustomForm;
begin
  if {(FActivePage <> Value) and }(Value >= 0) and (Value < Tabs.Count) then begin

    if (csDesigning in ComponentState) then begin
      ParentForm := GetParentForm(Self);
      if ParentForm <> nil then
        if ContainsControl(ParentForm.ActiveControl) then
          ParentForm.ActiveControl := Self;
    end;

    for Index := 0 to Tabs.Count - 1 do begin
      TDControl(Tabs[Index]).Visible := False;
    end;

    if (Value >= 0) and (Value < Tabs.Count) then begin
      TDControl(Tabs[Value]).BringToFront;
      TDControl(Tabs[Value]).Visible := True;
      TDControl(Tabs[Value]).Left := FTabRect.Left;
      TDControl(Tabs[Value]).Top := FTabRect.Top;
      TDControl(Tabs[Value]).Width := FTabRect.Right;
      TDControl(Tabs[Value]).Height := FTabRect.Bottom;
    end;
   {
    if (FActivePage >= 0) and (FActivePage < Tabs.Count) then begin
      TDControl(Tabs[FActivePage]).Visible := False;
    end; }

    FActivePage := Value;

    if (csDesigning in ComponentState) then begin
      if ParentForm <> nil then
        if ParentForm.ActiveControl = Self then
          SelectFirst;
    end;
  end;
  if (csDesigning in ComponentState) then
    Invalidate;
end;

procedure TDPageControl.Add(D: TDControl);
begin
  if Tabs.IndexOf(D) < 0 then begin
    Tabs.Add(D);
    D.Visible := False;
    D.Left := FTabRect.Left;
    D.Top := FTabRect.Top;
    D.Width := FTabRect.Right;
    D.Height := FTabRect.Bottom;
    if (FActivePage < 0) or (FActivePage >= Tabs.Count) then
      ActivePage := 0;
  end;
end;

procedure TDPageControl.Delete(D: TDControl);
begin
  Tabs.Remove(D);
end;

procedure TDPageControl.SetTabLeft(Value: Integer);
begin
  FTabRect.Left := Min(Value, Width);
  if (FActivePage >= 0) and (FActivePage < Tabs.Count) then begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
    if (csDesigning in ComponentState) then
      Invalidate;
  end;
end;

function TDPageControl.GetTabLeft: Integer;
begin
  Result := FTabRect.Left;
end;

function TDPageControl.GetTabTop: Integer;
begin
  Result := FTabRect.Top;
end;

procedure TDPageControl.SetTabTop(Value: Integer);
begin
  FTabRect.Top := Min(Value, Height);
  if (FActivePage >= 0) and (FActivePage < Tabs.Count) then begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
    if (csDesigning in ComponentState) then
      Invalidate;
  end;
end;

function TDPageControl.GetTabWidth: Integer;
begin
  Result := FTabRect.Right;
end;

procedure TDPageControl.SetTabWidth(Value: Integer);
begin
  FTabRect.Right := Min(Value, Width);
  if (FActivePage >= 0) and (FActivePage < Tabs.Count) then begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
    if (csDesigning in ComponentState) then
      Invalidate;
  end;
end;

function TDPageControl.GetTabHeight: Integer;
begin
  Result := FTabRect.Bottom;
end;

procedure TDPageControl.SetTabHeight(Value: Integer);
begin
  FTabRect.Bottom := Min(Value, Height);
  if (FActivePage >= 0) and (FActivePage < Tabs.Count) then begin
    TDControl(Tabs[FActivePage]).BringToFront;
    TDControl(Tabs[FActivePage]).Visible := True;
    TDControl(Tabs[FActivePage]).Left := FTabRect.Left;
    TDControl(Tabs[FActivePage]).Top := FTabRect.Top;
    TDControl(Tabs[FActivePage]).Width := FTabRect.Right;
    TDControl(Tabs[FActivePage]).Height := FTabRect.Bottom;
    if (csDesigning in ComponentState) then
      Invalidate;
  end;
end;

{-------------------------------------------------------------------------------}

constructor TDxList.Create;
begin
  inherited;
end;

destructor TDxList.Destroy;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    Objects[I].Free;
  inherited;
end;

procedure TDxList.Clear;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    Objects[I].Free;
  inherited;
end;

procedure TDxList.Delete(Index: Integer);
begin
  Objects[Index].Free;
  inherited Delete(Index);
end;
{------------------------------------------------------------------------------}

constructor TDxLines.Create(AOwner: TObject);
begin
  inherited Create;
  FOwner := AOwner;
  FItemList := nil;
  FWidth := 0;
  FHeight := 0;
end;

destructor TDxLines.Destroy;
var
  I: Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited;
end;

function TDxLines.AddItem(const S: string; AObject: TObject): pTViewItem;
begin
  AddObject(S, AObject);
  Result := @FItemList[Length(FItemList) - 1];
end;

function TDxLines.AddObject(const S: string; AObject: TObject): Integer;
var
  ViewItem: pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  ViewItem := @FItemList[Length(FItemList) - 1];
  ViewItem.Down := False;
  ViewItem.Move := False;
  ViewItem.Caption := S;
  ViewItem.ShowCaption := True;
  ViewItem.Data := nil;
  ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.TimeTick := GetTickCount;
  ViewItem.Index := -1;
  ViewItem.Text := '';
  ViewItem.OffsetX := 0;
  ViewItem.OffsetY := 0;
  FillChar(ViewItem.Char, SizeOf(ViewItem.Char), 0);
  TimeTick := GetTickCount;
  Result := inherited AddObject(S, AObject);
end;

procedure TDxLines.Clear;
var
  I: Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited Clear;
end;

procedure TDxLines.Delete(Index: Integer);
var
  I: Integer;
begin
  FItemList[Index].Color.Free;
  FItemList[Index].ImageIndex.Free;
  for I := Index to Length(FItemList) - 2 do begin
    FItemList[I] := FItemList[I + 1];
  end;
  SetLength(FItemList, Length(FItemList) - 1);
  inherited Delete(Index);
end;

procedure TDxLines.InsertObject(Index: Integer; const S: string;
  AObject: TObject);
var
  I: Integer;
  ViewItem: pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  for I := Length(FItemList) - 1 downto Index do begin
    FItemList[I] := FItemList[I - 1];
  end;
  ViewItem := @FItemList[Index];
  ViewItem.Down := False;
  ViewItem.Move := False;
  ViewItem.Caption := S;
  ViewItem.ShowCaption := True;
  ViewItem.Data := nil;
  ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  ViewItem.TimeTick := GetTickCount;
  ViewItem.Index := -1;
  ViewItem.Text := '';
  ViewItem.OffsetX := 0;
  ViewItem.OffsetY := 0;
  TimeTick := GetTickCount;
  inherited InsertObject(Index, S, AObject);
end;

function TDxLines.GetItem(Index: Integer): pTViewItem;
begin
  Result := @FItemList[Index];
end;

constructor TDxFont.Create;
begin
  FColor := clWhite;
  FBColor := clBlack;
  FName := ''; //宋体
  FStyle := [];
  FSize := 9;
  FBold := False;
end;

procedure TDxFont.Changed;
begin
  if Assigned(FOnChange) then FOnChange(Self);
end;

procedure TDxFont.SetColor(Value: TColor);
begin
  if FColor <> Value then begin
    FColor := Value;
    Changed;
  end;
end;

procedure TDxFont.SetBColor(Value: TColor);
begin
  if FBColor <> Value then begin
    FBColor := Value;
    Changed;
  end;
end;

procedure TDxFont.SetName(Value: TFontName);
begin
  if FName <> Value then begin
    FName := Value;
    Changed;
  end;
end;

procedure TDxFont.SetSize(Value: Integer);
begin
  if FSize <> Value then begin
    FSize := Value;
    Changed;
  end;
end;

procedure TDxFont.SetStyle(Value: TFontStyles);
begin
  if FStyle <> Value then begin
    FStyle := Value;
    Changed;
  end;
end;

procedure TDxFont.SetBold(Value: Boolean);
begin
  if FBold <> Value then begin
    FBold := Value;
    Changed;
  end;
end;

procedure TDxFont.Assign(Source: TPersistent);
begin
  //inherited;
  if Source is TDxFont then begin
    FColor := TDxFont(Source).Color;
    FBColor := TDxFont(Source).BColor;
    FName := TDxFont(Source).Name;
    FStyle := TDxFont(Source).Style;
    FSize := TDxFont(Source).Size;
    FBold := TDxFont(Source).Bold;
    Changed;
  end;
end;

constructor TDxCaptionColor.Create;
begin
  inherited;
  FUp := TDxFont.Create;
  FHot := TDxFont.Create;
  FDown := TDxFont.Create;
  FDisabled := TDxFont.Create;

  FUp.Bold := True;
  FHot.Bold := True;
  FDown.Bold := True;
  FDisabled.Bold := True;

  FUp.Color := clWhite;
  FHot.Color := clWhite;
  FDown.Color := clWhite;
  FDisabled.Color := clBtnFace;

  FDisabled := TDxFont.Create;
  FUp.OnChange := FontChange;
  FHot.OnChange := FontChange;
  FDown.OnChange := FontChange;
  FDisabled.OnChange := FontChange;
end;

destructor TDxCaptionColor.Destroy;
begin
  FUp.Free;
  FHot.Free;
  FDown.Free;
  FDisabled.Free;
  inherited Destroy;
end;

procedure TDxCaptionColor.Assign(Source: TPersistent);
begin
  //inherited;
  if Source is TDxCaptionColor then begin
    FUp.Assign(TDxCaptionColor(Source).Up);
    FHot.Assign(TDxCaptionColor(Source).Hot);
    FDown.Assign(TDxCaptionColor(Source).Down);
    FDisabled.Assign(TDxCaptionColor(Source).Disabled);
  end;
end;

procedure TDxCaptionColor.SetUp(Value: TDxFont);
begin
  FUp.Assign(Value);
end;

procedure TDxCaptionColor.SetHot(Value: TDxFont);
begin
  FHot.Assign(Value);
end;

procedure TDxCaptionColor.SetDown(Value: TDxFont);
begin
  FDown.Assign(Value);
end;

procedure TDxCaptionColor.SetDisabled(Value: TDxFont);
begin
  FDisabled.Assign(Value);
end;

procedure TDxCaptionColor.FontChange(Sender: TObject);
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

{constructor TDxBorderColor.Create;
begin
  inherited Create;
  Up.Color := $00608490;
  Up.Bold := False;
  Hot.Color := $005894B8;
  Hot.Bold := False;
  Down.Color := $005894B8;
  Down.Bold := False;
  Disabled.Color := clBtnFace;
  Disabled.Bold := False;
end;   }
//---------------------------------------------------------------------------

constructor TDxImageIndex.Create;
begin
  inherited;
  FUp := -1;
  FDown := -1;
  FHot := -1;
  FDisabled := -1;
  FOnChange := nil;
  FImage := nil;
end;

procedure TDxImageIndex.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TDxImageIndex.Assign(Source: TPersistent);
begin
  inherited;
  if Source is TDxImageIndex then begin
    FImage := TDxImageIndex(Source).Image;
    FUp := TDxImageIndex(Source).Up;
    FDown := TDxImageIndex(Source).Down;
    FHot := TDxImageIndex(Source).Hot;
    FDisabled := TDxImageIndex(Source).Disabled;
    Changed;
  end;
end;

procedure TDxImageIndex.SetImage(Value: TGameImages);
begin
  if FImage <> Value then begin
    FImage := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetUp(Value: Integer);
begin
  if FUp <> Value then begin
    FUp := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetHot(Value: Integer);
begin
  if FHot <> Value then begin
    FHot := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetDown(Value: Integer);
begin
  if FDown <> Value then begin
    FDown := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetDisabled(Value: Integer);
begin
  if FDisabled <> Value then begin
    FDisabled := Value;
    Changed;
  end;
end;

constructor TDMemo.Create(AOwner: TComponent);
begin
  inherited Create(aowner);
  FShowScroll := True;
  FScrollSize := 16;
  FScrollBars := ssHorizontal;

  Width := 200;
  Height := 100;

  FImageIndex := TDxImageIndex.Create;
  FScrollImageIndex := TDxImageIndex.Create;
  FPrevImageIndex := TDxImageIndex.Create;
  FNextImageIndex := TDxImageIndex.Create;
  FBarImageIndex := TDxImageIndex.Create;

  FScrollImageIndex.OnChange := ScrollImageIndexChange;
  FPrevImageIndex.OnChange := PrevImageIndexChange;
  FNextImageIndex.OnChange := NextImageIndexChange;
  FBarImageIndex.OnChange := BarImageIndexChange;

  FPrevMouseDown := False;
  FNextMouseDown := False;
  FBarMouseDown := False;

  FPrevMouseMove := False;
  FNextMouseMove := False;
  FBarMouseMove := False;

  FItemIndex := -1;
  FMaxValue := 100;
  FPosition := 0;
  FRemoveSize := 50;
  FItemHeight := 12;
  FOnScroll := nil;

  FBarTop := 0;
  FOffSetX := 0;
  FOffSetY := 0;
  FDrawLineCount := 0;
  FMouseMoveTick := GetTickCount;


  Downed := False;
  FOnClick := nil;
  FEnableFocus := False;
  FClickSound := csNone;
  Caption := Name;


  FColors := TColors.Create;
  FColors.Background := clWhite;
  FColors.Border := $00488184;
  FColors.Hot := $0078B3B6;
  //FColors.Down := $0078B3B6;
  FColors.Selected := clNavy;
  FColors.Down := clBtnFace;

  FBackground := False;
  FBorder := False;
  FMainMenu := nil;
  FBottomHeight := 0;
  //Canvas.Font.Assign(Font);
end;

destructor TDMemo.Destroy;
begin
  FColors.Free;
  FImageIndex.Free;
  FScrollImageIndex.Free;
  FPrevImageIndex.Free;
  FNextImageIndex.Free;
  FBarImageIndex.Free;
  inherited Destroy;
end;

procedure TDMemo.ScrollImageIndexChange(Sender: TObject);
var
  D: TDxSurface;
  nIndex: Integer;
begin
  if (FScrollImageIndex.Image <> nil) then begin
    if FScrollImageIndex.Up >= 0 then
      nIndex := FScrollImageIndex.Up
    else
      if FScrollImageIndex.Hot >= 0 then
      nIndex := FScrollImageIndex.Hot
    else
      if FScrollImageIndex.Down >= 0 then
      nIndex := FScrollImageIndex.Down;

    if (nIndex >= 0) then begin
      D := FScrollImageIndex.Image.Images[nIndex];
      if D <> nil then
        FScrollSize := D.Width;
    end;
  end;
end;

procedure TDMemo.PrevImageIndexChange(Sender: TObject);
var
  D: TDxSurface;
  nIndex: Integer;
begin
  if (PrevImageIndex.Image <> nil) then begin
    if FPrevImageIndex.Up >= 0 then
      nIndex := FPrevImageIndex.Up
    else
      if FPrevImageIndex.Hot >= 0 then
      nIndex := FPrevImageIndex.Hot
    else
      if FPrevImageIndex.Down >= 0 then
      nIndex := FPrevImageIndex.Down;

    if (nIndex >= 0) then begin
      D := PrevImageIndex.Image.Images[nIndex];
      if D <> nil then
        FPrevImageSize := D.Height;
    end;
  end;
end;

procedure TDMemo.NextImageIndexChange(Sender: TObject);
var
  D: TDxSurface;
  nIndex: Integer;
begin
  if FNextImageIndex.Up >= 0 then
    nIndex := FNextImageIndex.Up
  else
    if FNextImageIndex.Hot >= 0 then
    nIndex := FNextImageIndex.Hot
  else
    if FNextImageIndex.Down >= 0 then
    nIndex := FNextImageIndex.Down;

  if (NextImageIndex.Image <> nil) and (nIndex >= 0) then begin
    D := NextImageIndex.Image.Images[nIndex];
    if D <> nil then
      FNextImageSize := D.Height;
  end;
end;

procedure TDMemo.BarImageIndexChange(Sender: TObject);
var
  D: TDxSurface;
  nIndex: Integer;
begin
  if FBarImageIndex.Up >= 0 then
    nIndex := FBarImageIndex.Up
  else
    if FBarImageIndex.Hot >= 0 then
    nIndex := FBarImageIndex.Hot
  else
    if FBarImageIndex.Down >= 0 then
    nIndex := FBarImageIndex.Down;

  if (BarImageIndex.Image <> nil) and (nIndex >= 0) then begin
    D := BarImageIndex.Image.Images[nIndex];
    if D <> nil then
      FBarImageSize := D.Height;
  end;
end;

procedure TDMemo.RefPosinton;
begin
  DoScroll(0);
end;


procedure TDMemo.DoPostion(var ALeft, ATop: Integer);
begin

end;

procedure TDMemo.DoSize(var AWidth, AHeight: Integer);
begin

end;


procedure TDMemo.SetBarPosition;
var
  nMaxValue, nHeight: Integer;
begin
  if FShowScroll then begin //尺寸变化 重新计算滚动条位置
    nMaxValue := FMaxValue - FRemoveSize;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;

        //DebugOutStr('FBarTop:' + IntToStr(FBarTop) + ' nHeight:' + IntToStr(nHeight));
  end;
end;

procedure TDMemo.SetBounds(ALeft, ATop, AWidth, AHeight: Integer);
var
  NewLeft, NewTop, NewWidth, NewHeight: Integer;
begin
  NewLeft := ALeft;
  NewTop := ATop;
  NewWidth := AWidth;
  NewHeight := AHeight;

  if not (csDesigning in ComponentState) then begin
    if (ALeft <> Left) or (ATop <> Top) then begin
      DoPostion(NewLeft, NewTop);
    end;
    if (AWidth <> Width) or (AHeight <> Height) then begin
      DoSize(NewWidth, NewHeight);
    end;
  end;

  ALeft := NewLeft;
  ATop := NewTop;
  AWidth := NewWidth;
  AHeight := NewHeight;
  if not (csDesigning in ComponentState) then begin
    if (AWidth <> Width) or (AHeight <> Height) then begin

      if DControls <> nil then
        DoScroll(0); //尺寸变化 刷新控件
      SetBarPosition;
    end;
  end;
  inherited SetBounds(ALeft, ATop, AWidth, AHeight);
end;

procedure TDMemo.DoScroll(Value: Integer); //滚动
var
  I: Integer;
  vRect: TRect;
  ARect: TRect;
  BRect: TRect;
  D: TDControl;
begin
  //if Value <> 0 then begin
  vRect := Rect(0, 0, Width, Height);
  for I := 0 to DControls.Count - 1 do begin
    D := DControls[I];
    D.Top := D.Top + Value;

    ARect := Bounds(D.Left, D.Top, D.Width, D.Height);
    if (D.Top >= vRect.Top) and (ARect.Bottom <= vRect.Bottom) then
      D.Visible := IntersectRect(BRect, vRect, ARect)
    else
      D.Visible := False;
  end;
  //end;
  if (Assigned(FOnScroll)) then FOnScroll(Self);
end;

procedure TDMemo.SetMaxValue(Value: Integer); //设置滚动最大值
var
  P, nMaxValue, nHeight: Integer;
begin
  if FMaxValue <> Value then begin
    FMaxValue := Max(Value, 0);
    if FMaxValue - FRemoveSize >= 0 then begin
      if FPosition > FMaxValue - FRemoveSize then begin
        P := Max(FMaxValue - FRemoveSize, 0);
        //if FMaxValue < Height then P := 0;
        DoScroll(FPosition - P);
        FPosition := P;
      end;
    end else begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;
  //SetBarPosition;

  if FShowScroll then begin
    nMaxValue := FMaxValue - FRemoveSize;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;
  end;
end;

procedure TDMemo.SetPosition(Value: Integer); //设置滚动指针
var
  P, nMaxValue, nHeight: Integer;
begin
  if FPosition <> Value then begin
    P := Value;
    if P < 0 then P := 0;
    if P > FMaxValue - FRemoveSize then
      P := Max(FMaxValue - FRemoveSize, 0);

    DoScroll(FPosition - P);
    FPosition := P;
  end;
  //SetBarPosition;

  {if FShowScroll then begin
    nMaxValue := FMaxValue - FRemoveSize;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;
  end;}

  if FShowScroll then begin
    nMaxValue := FMaxValue - FRemoveSize;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;
  end;
end;

procedure TDMemo.SetRemoveSize(Value: Integer);
var
  P, nMaxValue, nHeight: Integer;
begin
  if FRemoveSize <> Value then begin
    FRemoveSize := Value;
    if FMaxValue - FRemoveSize >= 0 then begin
      if FPosition > FMaxValue - FRemoveSize then begin
        P := Max(FMaxValue - FRemoveSize, 0);
        //if FMaxValue < Height then P := 0;
        DoScroll(FPosition - P);
        FPosition := P;
      end;
    end else begin
      P := 0;
      DOScroll(FPosition - P);
      FPosition := P;
    end;
  end;
  //SetBarPosition;

  if FShowScroll then begin
    nMaxValue := FMaxValue - FRemoveSize;
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;
  end;
end;

function TDMemo.InPrevRange(X, Y: Integer; vRect: TRect): Boolean; //检测鼠标点在 向上的按钮
begin
  Result := (X >= vRect.Right - FScrollSize) and (Y <= vRect.Top + FPrevImageSize);
end;

function TDMemo.InNextRange(X, Y: Integer; vRect: TRect): Boolean; //检测鼠标点在 向下的按钮
begin
  Result := (X >= vRect.Right - FScrollSize) and (Y >= vRect.Top + (Height - FNextImageSize));
end;

function TDMemo.InBarRange(X, Y: Integer; vRect: TRect): Boolean; //检测鼠标点在滚动条的按钮
var
  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;
begin
  Result := False;
  if FShowScroll then begin
    if (X >= vRect.Right - FScrollSize) and (Y < vRect.Right - FNextImageSize) and
      (Y > vRect.Top + FPrevImageSize) then begin
      if FPosition > 0 then begin
        nMaxValue := FMaxValue - FRemoveSize;
        nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;
        if (nMaxValue > 0) and (nHeight > 0) then
          nBarTop := Round(FPosition * nHeight / nMaxValue)
        else
          nBarTop := FPrevImageSize;
        Result := (Y >= vRect.Top + FPrevImageSize + nBarTop) and (Y <= vRect.Top + FPrevImageSize + nBarTop + FBarImageSize);
      end else begin
        Result := (Y <= vRect.Top + FPrevImageSize + FBarImageSize);
      end;
    end;
  end;
end;

procedure TDMemo.Next; //向下滚动
var
  P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nPosition: Integer;
begin
  nMaxValue := FMaxValue - FRemoveSize;
  if FShowScroll then
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize
  else
    nHeight := Height;

  if (nMaxValue > 0) {and (FMaxValue > nHeight)} and (nHeight > 0) then begin
    if (FPosition < nMaxValue) then begin
      if FPosition + FItemHeight <= nMaxValue then begin
        P := FPosition + FItemHeight;
        DoScroll(FPosition - P);
        FPosition := P;
      end else begin
        P := nMaxValue;
        DOScroll(FPosition - P);
        FPosition := P;
      end;
    end;
  end else begin
    if FPosition > 0 then begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;

  if FShowScroll then begin
    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    {if FBarTop >= nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize; }
  end;
end;

procedure TDMemo.Previous; //向上滚动
var
  P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nPosition: Integer;
begin
  if FPosition > 0 then begin
    nMaxValue := FMaxValue - FRemoveSize;
    if FShowScroll then
      nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize
    else
      nHeight := Height;

    if (nMaxValue > 0) and (nHeight > 0) then begin
      if FPosition - FItemHeight >= 0 then begin
        P := FPosition - FItemHeight;
        DoScroll(FPosition - P);
        FPosition := P;
      end else begin
        P := 0;
        DoScroll(FPosition - P);
        FPosition := P;
      end;
    end else begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;

  if FShowScroll then begin
    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    {if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;    }
  end;
end;

procedure TDMemo.First; //滚动条回到顶部
var
  P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
begin
  if FPosition > 0 then begin
    P := 0;
    DoScroll(FPosition - P);
    FPosition := P;
  end;
  if FShowScroll then begin
    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;
  end;
end;

procedure TDMemo.Last; //滚动条回到底部
var
  P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nPosition: Integer;
begin
  nMaxValue := FMaxValue - FRemoveSize;
  if FShowScroll then
    nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize
  else
    nHeight := Height;
  //if (nMaxValue > 0) and (MaxValue > Height) and (nHeight > 0) then begin
  if (nMaxValue > 0) and (nHeight > 0) and (nMaxValue > nHeight) then begin
    if FPosition < nMaxValue then begin
      P := nMaxValue;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end else begin
    if FPosition > 0 then begin
      P := 0;
      DoScroll(FPosition - P);
      FPosition := P;
    end;
  end;
  if FShowScroll then begin
    if (nMaxValue > 0) and (nHeight > 0) then
      FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
    else
      FBarTop := 0;

    if FPosition >= nMaxValue then
      FBarTop := Height - FNextImageSize - FBarImageSize
    else
      if FPosition = 0 then
      FBarTop := FPrevImageSize
    else
      FBarTop := FBarTop + FPrevImageSize;

    if FBarTop > nHeight - FNextImageSize - FBarImageSize then
      FBarTop := nHeight - FNextImageSize - FBarImageSize;

    if FBarTop < FPrevImageSize then
      FBarTop := FPrevImageSize;
  end;
end;

procedure TDMemo.SetImageIndex(Value: TDxImageIndex);
begin
  FImageIndex.Assign(Value);
end;

procedure TDMemo.SetScrollImageIndex(Value: TDxImageIndex);
begin
  FScrollImageIndex.Assign(Value);
end;

procedure TDMemo.SetPrevImageIndex(Value: TDxImageIndex);
begin
  FPrevImageIndex.Assign(Value);
end;

procedure TDMemo.SetNextImageIndex(Value: TDxImageIndex);
begin
  FNextImageIndex.Assign(Value);
end;

procedure TDMemo.SetBarImageIndex(Value: TDxImageIndex);
begin
  FBarImageIndex.Assign(Value);
end;

procedure TDMemo.SetColors(Value: TColors);
begin
  FColors.AssignTo(Value);
end;

function TDMemo.InRange(X, Y: Integer): Boolean;
var
  boInrange: Boolean;
begin
  if (X >= Left) and (X < Left + Width) and (Y >= Top) and (Y < Top + Height) then begin

    boInrange := True;

    if ShowScroll then begin
      if (X <= Left + Width - FScrollSize) and (Y >= Top + Height - FBottomHeight) then
        boInrange := False;
    end else begin
      if (Y >= Top + Height - FBottomHeight) then
        boInrange := False;
    end;

    if Assigned(FOnInRealArea) then
      FOnInRealArea(Self, X - Left, Y - Top, boInrange);
    Result := boInrange;
  end else
    Result := False;
end;

procedure TDMemo.SetItemIndex(Value: Integer);
var
  nItemCount: Integer;
begin
  if FItemIndex <> Value then begin
    FItemIndex := Value;
    nItemCount := FMaxValue div FItemHeight;
    if FItemIndex >= nItemCount then FItemIndex := -1;
    if FItemIndex >= 0 then begin
      if FItemIndex * FItemHeight < FRemoveSize then
        Position := 0
      else
        Position := FItemIndex * FItemHeight - FRemoveSize;
    end;
  end;
end;

procedure TDMemo.SetItemHeight(Value: Integer);
begin
  if csDesigning in ComponentState then begin
    FItemHeight := Value; //Max(Value, Canvas.TextHeight('Pp'));
  end else begin
    FItemHeight := Max(Value, ImageCanvas.TextHeight('Pp'));
  end;
end;

procedure TDMemo.SetScrollSize(Value: Integer);
begin
  if FScrollSize <> Value then begin
    FScrollSize := Value;
  end;
end;

procedure TDMemo.SetScrollBars(Value: TScrollStyle);
begin
  if FScrollBars <> Value then begin
    FScrollBars := Value;
  end;
end;

procedure TDMemo.Process;
var
  I: Integer;
begin
  if Assigned(FOnProcess) then FOnProcess(Self);

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).Process;
end;

procedure TDMemo.DirectPaint(dsurface: TDxSurface);
var
  DControl: TDControl;

  I, nIndex: Integer;

  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;

  Surface: TDxSurface;
  D: TDxSurface;
  vtRect: TRect;
  vbRect: TRect;
  PaintRect: TRect;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  if FBackground then
    dsurface.FillRect(Bounds(SurfaceX(Left), SurfaceY(Top), Width, Height), FColors.Background);

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);

  Surface := TDxSurface.Create(ImageCanvas.DDraw);
  Surface.SetSize(Width, Height);

  if ImageIndex.Image <> nil then begin
    if ImageIndex.Up >= 0 then begin
      D := ImageIndex.Image.Images[ImageIndex.Up];
      if D <> nil then begin
        Surface.Draw((Surface.Width - D.Width) div 2, 0, D.ClientRect, D, False);
      end;
    end;
  end;

  if FShowScroll then begin
    if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
      D := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
      if D <> nil then begin
        Surface.StretchDraw(Bounds(Surface.Width - D.Width, 0, D.Width, Surface.Height), D, False);
        Surface.Draw(Surface.Width - D.Width, 0, Bounds(0, 0, D.Width, 16), D, False);
        Surface.Draw(Surface.Width - D.Width, Surface.Height - 16, Bounds(0, D.Height - 16, D.Width, D.Height), D, False);
      end;
    end;

    if FPrevImageIndex.Image <> nil then begin
      nIndex := -1;
      if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then nIndex := FPrevImageIndex.Down
      else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then nIndex := FPrevImageIndex.Hot
      else if (FPrevImageIndex.Up >= 0) then nIndex := FPrevImageIndex.Up;
      if (nIndex >= 0) then begin
        D := FPrevImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, 1, D.ClientRect, D);
        end;
      end;
    end;

    if FNextImageIndex.Image <> nil then begin
      nIndex := -1;
      if FNextMouseDown and (FNextImageIndex.Down >= 0) then nIndex := FNextImageIndex.Down
      else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then nIndex := FNextImageIndex.Hot
      else if (FNextImageIndex.Up >= 0) then nIndex := FNextImageIndex.Up;

      if nIndex >= 0 then begin
        D := FNextImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, (Surface.Height - D.Height - 1), D.ClientRect, D);
        end;
      end;
    end;

    if FBarImageIndex.Image <> nil then begin
      nIndex := -1;
      if FBarMouseDown and (FBarImageIndex.Down >= 0) then nIndex := FBarImageIndex.Down
      else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then nIndex := FBarImageIndex.Hot
      else if (FBarImageIndex.Up >= 0) then nIndex := FBarImageIndex.Up;

      if nIndex >= 0 then begin
        D := FBarImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, FBarTop, D.ClientRect, D);
        end;
      end;
    end;
  end;

  dsurface.Draw(SurfaceX(Left), SurfaceY(Top), Surface.ClientRect, Surface, True);
  Surface.Free;
end;

function TDMemo.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin
  Result := inherited KeyDown(Key, Shift);
end;

function TDMemo.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I, P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;
  vRect: TRect;
  AOnMouseDown: TOnMouseDown;
begin
  Result := False;
  AOnMouseDown := OnMouseDown;
  OnMouseDown := nil;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    Result := True;
    //DebugOutStr('TDMemo.MouseDown2');
    FPrevMouseMove := False;
    FNextMouseMove := False;
    FBarMouseMove := False;
    FMouseMoveTick := GetTickCount + 600;
    vRect := Bounds(Left, Top, Width, Height);

    if (Button = mbLeft) and FShowScroll then begin
      FPrevMouseDown := InPrevRange(X, Y, vRect);
      FNextMouseDown := InNextRange(X, Y, vRect);
      FBarMouseDown := InBarRange(X, Y, vRect);

      if (not FPrevMouseDown) and (not FNextMouseDown) and (not FBarMouseDown) then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
        if PointInRect(Point(X, Y), vRect) then begin
          nMaxValue := FMaxValue - FRemoveSize;
          nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;

          if ((nMaxValue > 0) and (MaxValue > Height) and (nHeight > 0)) or (Position > 0) then begin
            nBarTop := Max(Y - vRect.Top - FPrevImageSize - FBarImageSize div 2, 0);
            Position := Round(nBarTop * nMaxValue / nHeight);
          end else begin
            Position := 0;
          end;

          if (nMaxValue > 0) and (nHeight > 0) then
            FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
          else
            FBarTop := 0;

          if FPosition = nMaxValue then
            FBarTop := Height - FNextImageSize - FBarImageSize
          else
            if FPosition = 0 then
            FBarTop := FPrevImageSize
          else
            FBarTop := FBarTop + FPrevImageSize;

          Exit;
        end;
      end else begin
        if FPrevMouseDown then begin
          Previous;
        end else
          if FNextMouseDown then begin
          Next;
        end;
      end;
    end;

    FItemIndex := (Y - vRect.Top) div FItemHeight + FPosition div FItemHeight;
    if FItemIndex >= FMaxValue div FItemHeight then FItemIndex := -1;

    OnMouseDown := AOnMouseDown;
    if Assigned(OnMouseDown) then
      OnMouseDown(Self, Button, Shift, X, Y);
    //DebugOutStr('TDMemo.MouseDown FItemIndex:' + IntToStr(FItemIndex));
  end else begin
    FPrevMouseDown := False;
    FNextMouseDown := False;
    FBarMouseDown := False;
    FPrevMouseMove := False;
    FNextMouseMove := False;
    FBarMouseMove := False;
  end;
  OnMouseDown := AOnMouseDown;
end;

function TDMemo.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  P: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;
  vRect: TRect;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if Result and (MouseCaptureControl = Self) then begin
    //if MouseCaptureControl <> nil then DebugOutStr('TDMemo.MouseMove1:' + MouseCaptureControl.Name);
    //DebugOutStr('TDMemo.MouseMove2');
    if FShowScroll then begin
      vRect := Bounds(Left, Top, Width, Height);
      FPrevMouseMove := InPrevRange(X, Y, vRect);
      FNextMouseMove := InNextRange(X, Y, vRect);
      FBarMouseMove := InBarRange(X, Y, vRect);
    end;

    if FBarMouseDown and FShowScroll then begin
      FPrevMouseMove := False;
      FNextMouseMove := False;

      nMaxValue := FMaxValue - FRemoveSize;
      nHeight := Height - FPrevImageSize - FNextImageSize - FBarImageSize;
      if ((nMaxValue > 0) and (MaxValue > Height) and (nHeight > 0)) or (Position > 0) then begin
        nBarTop := Max(Y - vRect.Top - FPrevImageSize - FBarImageSize div 2, 0);
        Position := Round(nBarTop * nMaxValue / nHeight);
      end else begin
        Position := 0;
      end;

      if (nMaxValue > 0) and (nHeight > 0) then
        FBarTop := Max(Round(FPosition * nHeight / nMaxValue), 0)
      else
        FBarTop := 0;

      if FPosition = nMaxValue then
        FBarTop := Height - FNextImageSize - FBarImageSize
      else
        if FPosition = 0 then
        FBarTop := FPrevImageSize
      else
        FBarTop := FBarTop + FPrevImageSize;

    end else begin
      if FPrevMouseMove and FPrevMouseDown then begin
        if Longint(GetTickCount - FMouseMoveTick) > 100 then begin
          FMouseMoveTick := GetTickCount;
          Previous;
        end;
      end else
        if FNextMouseMove and FNextMouseDown then begin
        if Longint(GetTickCount - FMouseMoveTick) > 100 then begin
          FMouseMoveTick := GetTickCount;
          Next;
        end;
      end;
      vRect := Bounds(Left, Top, Width, Height);
      FItemIndex := (Y - vRect.Top) div FItemHeight + FPosition div FItemHeight;
      if FItemIndex >= FMaxValue div FItemHeight then
        FItemIndex := -1;
    end;
  end else begin
      //DebugOutStr('TDMemo.MouseMove3');
    //FPrevMouseDown := False;
    //FNextMouseDown := False;
    //FBarMouseDown := False;
    //DebugOutStr('TDMemo.MouseMove FBarMouseMove3:'+BoolToStr(FBarMouseMove)+' FBarMouseDown:'+BoolToStr(FBarMouseDown));
    FPrevMouseMove := False;
    FNextMouseMove := False;
    FBarMouseMove := False;
    //FItemIndex := -1;
  end;
end;

function TDMemo.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    if (not Background) and InRange(X, Y) then begin
      Result := True;
    end;
  end;
  FBarMouseDown := False;
  FPrevMouseDown := False;
  FNextMouseDown := False;
  ReleaseCapture;
  Downed := False;
end;

function TDMemo.MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean;
begin
  if MouseWheelControl = Self then Next;
end;

function TDMemo.MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean;
begin
  if MouseWheelControl = Self then Previous;
end;
{-------------------------------------------------------------------------------}

constructor TDxChatMemo.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FShowScroll := False;
  FDrawLineCount := 9;
  FRemoveSize := 12 * 8;
  FMaxValue := 0;
  Height := 12 * FDrawLineCount;
  Width := 388;
  FOffsetHeight := 0;
  FOnItemClick := nil;
  FOnItemMouseDown := nil;
  FOnItemMouseMove := nil;
  FOnItemMouseUp := nil;

  FTopIndex := 0;
  FTopCount := 3;
  FOnChange := nil;
  FAutoScroll := False;
  FOffSetX := 0;
  FTopIndex := 0;
  FFontBackTransparent := False;
  FStrings := TDxList.Create;
  FTopStrings := TDxList.Create;
end;

destructor TDxChatMemo.Destroy;
begin
  FStrings.Free;
  FTopStrings.Free;
  inherited Destroy;
end;

procedure TDxChatMemo.DoScroll(Value: Integer);
var
  I, nHeight: Integer;
  List: TDxList;
  Lines: TDxLines;
begin
  if Position - Value <= 0 then begin
    FTopIndex := 0;
    FOffsetHeight := 0;
  end else begin
    FOffsetHeight := Position - Value;
    List := TDxList(FTopStrings);
    for I := 0 to TopCount - 1 do begin
      Lines := TDxLines(List.Objects[I]);
      if FOffsetHeight <= 0 then begin
        FTopIndex := 0;
        Exit;
      end;
      //if FOffsetHeight >= Lines.Height then
      Dec(FOffsetHeight, Lines.Height);
      //else nHeight := 0;
    end;

    List := TDxList(FStrings);
    for I := 0 to List.Count - 1 do begin
      Lines := TDxLines(List.Objects[I]);
      if FOffsetHeight <= 0 then begin
        FTopIndex := I;
        Exit;
      end;
      //if FOffsetHeight >= Lines.Height then
      Dec(FOffsetHeight, Lines.Height);
      //else nHeight := 0;
    end;
  end;
end;

function TDxChatMemo.GetTopCount: Integer;
begin
  if csDesigning in ComponentState then begin
    Result := FTopCount;
  end else begin
    Result := Min(FTopStrings.Count, FTopCount);
  end;
end;

procedure TDxChatMemo.SetStrings(Value: TStrings);
var
  I: Integer;
begin
  FStrings.Clear;
  FStrings.AddStrings(Value);
  MaxValue := (TopCount + FStrings.Count) * ItemHeight;
end;

procedure TDxChatMemo.SetTopStrings(Value: TStrings);
begin
  FTopStrings.Clear;
  FTopStrings.AddStrings(Value);
  MaxValue := (TopCount + FStrings.Count) * ItemHeight;
end;

function TDxChatMemo.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin
  Result := False;
  if inherited KeyDown(Key, Shift) then begin
    Result := True;
    case Key of
      VK_UP: Previous;
      VK_DOWN: Next;
      VK_PRIOR: if Position >= Height then Position := Position - Height else Position := 0;
      VK_NEXT: if Position + Height < MaxValue then Position := Position + Height else Position := MaxValue;
    end;
  end;
end;

procedure TDxChatMemo.LoadFromFile(const FileName: string);
var
  I: Integer;
  Lines: TDxLines;
begin
  FStrings.LoadFromFile(FileName);
  for I := 0 to FStrings.Count - 1 do begin
    Lines := TDxLines.Create(FStrings);
    FStrings.Objects[I] := Lines;
    Lines.Add(FStrings.Strings[I]);
    Lines.Height := ItemHeight;
    Lines.Width := ImageCanvas.TextWidth(FStrings.Strings[I]);
  end;
  FTopStrings.Clear;
  MaxValue := FStrings.Count * ItemHeight;
end;

function TDxChatMemo.GetClientWidth: Integer;
begin
  if ShowScroll then
    Result := Width - ScrollSize - ImageCanvas.TextWidth('0')
  else
    Result := Width - ImageCanvas.TextWidth('0');
end;

function TDxChatMemo.GetClientHeight: Integer;
begin
  Result := Height;
end;

function TDxChatMemo.Insert(Index: Integer; const S: string; FC, BC: TColor): TDxLines;
var
  TextWidth: Integer;

  I, Len, ALine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  TextWidth := ClientWidth;

  sText := S;
  Lines := nil;
  if ImageCanvas.TextWidth(sText) > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp);
      if ALine > TextWidth then begin
        List := TDxList(FStrings);
        if Lines = nil then begin
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        Lines.Insert(Index, Temp);
        Lines.TimeTick := GetTickCount;
        List.Strings[Index] := List.Strings[Index] + Temp;
        ViewItem := pTViewItem(Lines.Objects[Index]);

        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');
        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);

        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;

        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FStrings);
      if Lines = nil then begin
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      Lines.Insert(Index, Temp);
      ViewItem := pTViewItem(Lines.Objects[Index]);
      List.Strings[Index] := List.Strings[Index] + Temp;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);
      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then begin
      Inc(Index);
      Insert(Index, ' ' + sText, FC, BC);
    end;
  end else begin
    List := TDxList(FStrings);
    if Lines = nil then begin
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    Lines.Insert(Index, sText);
    List.Strings[Index] := List.Strings[Index] + sText;
    ViewItem := pTViewItem(Lines.Objects[Index]);
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);
    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

function TDxChatMemo.Add(const S: string; FC, BC: TColor; ALines: TDxLines): TDxLines;
var
  TextWidth: Integer;

  I, Len, ALine, nLine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  TextWidth := ClientWidth;
  nLine := 0;
  sText := S;
  Lines := ALines;

  if Lines <> nil then
    nLine := Lines.Width;

  if nLine >= TextWidth then begin
    if Lines <> nil then
      MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    nLine := 0;
    Lines := nil;
  end;

  if ImageCanvas.TextWidth(sText) + nLine > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp) + nLine;
      if ALine >= TextWidth then begin
        List := TDxList(FStrings);
        if Lines = nil then begin
          nLine := 0;
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        ViewItem := Lines.AddItem(Temp, nil);
        List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
        Lines.TimeTick := GetTickCount;
        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');

        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);
        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;
        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FStrings);
      if Lines = nil then begin
        nLine := 0;
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      ViewItem := Lines.AddItem(Temp, nil);
      List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
      Lines.TimeTick := GetTickCount;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);
      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then
      Add(' ' + sText, FC, BC);
  end else begin
    List := TDxList(FStrings);
    if Lines = nil then begin
      nLine := 0;
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    ViewItem := Lines.AddItem(sText, nil);
    List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + sText;
    Lines.TimeTick := GetTickCount;
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);
    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

function TDxChatMemo.Add(const S: string; FC, BC: TColor): TDxLines;
var
  TextWidth: Integer;

  I, Len, ALine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  TextWidth := ClientWidth;

  sText := S;
  Lines := nil;
  if ImageCanvas.TextWidth(sText) > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp);
      if ALine > TextWidth then begin
        List := TDxList(FStrings);
        if Lines = nil then begin
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        ViewItem := Lines.AddItem(Temp, nil);
        List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
        Lines.TimeTick := GetTickCount;
        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');

        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);
        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;
        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FStrings);
      if Lines = nil then begin
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      ViewItem := Lines.AddItem(Temp, nil);
      List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
      Lines.TimeTick := GetTickCount;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);
      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then
      Add(' ' + sText, FC, BC);
  end else begin
    List := TDxList(FStrings);
    if Lines = nil then begin
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    ViewItem := Lines.AddItem(sText, nil);
    List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + sText;
    Lines.TimeTick := GetTickCount;
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);
    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

procedure TDxChatMemo.Delete(Index: Integer);
begin
  FStrings.Delete(Index);
  MaxValue := MaxValue - ItemHeight;
end;

function TDxChatMemo.InsertTop(Index: Integer; const S: string; FC, BC: TColor; TimeOut: Integer): TDxLines;
var
  TextWidth: Integer;

  I, Len, ALine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  TextWidth := ClientWidth;

  sText := S;
  Lines := nil;
  if ImageCanvas.TextWidth(sText) > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp);
      if ALine > TextWidth then begin
        List := TDxList(FTopStrings);
        if Lines = nil then begin
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        Lines.Insert(Index, Temp);
        List.Strings[Index] := List.Strings[Index] + Temp;
        Lines.TimeTick := GetTickCount + TimeOut * 1000;
        ViewItem := pTViewItem(Lines.Objects[Index]);

        ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');
        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);
        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;
        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FTopStrings);
      if Lines = nil then begin
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      Lines.Insert(Index, Temp);
      List.Strings[Index] := List.Strings[Index] + Temp;
      Lines.TimeTick := GetTickCount + TimeOut * 1000;
      ViewItem := pTViewItem(Lines.Objects[Index]);
      ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);

      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);

      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then begin
      Inc(Index);
      InsertTop(Index, ' ' + sText, FC, BC, TimeOut);
    end;
  end else begin
    List := TDxList(FTopStrings);
    if Lines = nil then begin
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    Lines.Insert(Index, sText);
    Lines.TimeTick := GetTickCount + TimeOut * 1000;
    List.Strings[Index] := List.Strings[Index] + sText;
    ViewItem := pTViewItem(Lines.Objects[Index]);
    ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);
    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

function TDxChatMemo.AddTop(const S: string; FC, BC: TColor; TimeOut: Integer; ALines: TDxLines): TDxLines;
var
  TextWidth: Integer;

  I, Len, ALine, nLine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  TextWidth := ClientWidth;
  nLine := 0;
  sText := S;
  Lines := ALines;

  if Lines <> nil then
    nLine := Lines.Width;

  if nLine >= TextWidth then begin
    if Lines <> nil then
      MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    nLine := 0;
    Lines := nil;
  end;

  if ImageCanvas.TextWidth(sText) + nLine > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp) + nLine;
      if ALine >= TextWidth then begin
        List := TDxList(FTopStrings);
        if Lines = nil then begin
          nLine := 0;
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        ViewItem := Lines.AddItem(Temp, nil);
        List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
        Lines.TimeTick := GetTickCount + TimeOut * 1000;
        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');

        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);
        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;
        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FTopStrings);
      if Lines = nil then begin
        nLine := 0;
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      ViewItem := Lines.AddItem(Temp, nil);
      List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
      Lines.TimeTick := GetTickCount + TimeOut * 1000;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);
      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then
      AddTop(' ' + sText, FC, BC, TimeOut);
  end else begin
    List := TDxList(FTopStrings);
    if Lines = nil then begin
      nLine := 0;
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    ViewItem := Lines.AddItem(sText, nil);
    List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + sText;
    Lines.TimeTick := GetTickCount + TimeOut * 1000;
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);
    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

function TDxChatMemo.AddTop(const S: string; FC, BC: TColor; TimeOut: Integer): TDxLines;
var
  TextWidth: Integer;
  I, Len, ALine: integer;
  sText, DLine, Temp: string;

  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  //DebugOut('TDxChatMemo.AddTop:'+S+' '+IntToStr(TimeOut));
  TextWidth := ClientWidth;

  sText := S;
  Lines := nil;
  if ImageCanvas.TextWidth(sText) > TextWidth then begin
    Len := Length(sText);
    Temp := '';
    I := 1;
    while True do begin
      if I > Len then break;
      if Byte(sText[I]) >= 128 then begin
        Temp := Temp + sText[I];
        Inc(I);
        if I <= Len then Temp := Temp + sText[I]
        else break;
      end else
        Temp := Temp + sText[I];

      ALine := ImageCanvas.TextWidth(Temp);
      if ALine > TextWidth then begin
        List := TDxList(FTopStrings);
        if Lines = nil then begin
          Lines := TDxLines.Create(List);
          List.AddObject('', Lines);
        end;
        ViewItem := Lines.AddItem(Temp, nil);
        Lines.TimeTick := GetTickCount + TimeOut * 1000;
        List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
        ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
        ViewItem.Transparent := False;
        ViewItem.Caption := Temp;
        ViewItem.Color.Up.Color := FC;
        ViewItem.Color.Up.BColor := BC;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
        ViewItem.Color.Down.Assign(ViewItem.Color.Up);
        ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
        ViewItem.Width := ImageCanvas.TextWidth(Temp);
        ViewItem.Height := ImageCanvas.TextHeight('0');
        Lines.Width := Lines.Width + ViewItem.Width;
        Lines.Height := Max(Lines.Height, ViewItem.Height);
        sText := Copy(sText, I + 1, Len - i);
        Temp := '';
        MaxValue := MaxValue + Lines.Height;
        if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
          Position := Position + Lines.Height;
        end;
        if (Assigned(FOnChange)) then FOnChange(Self);
        break;
      end;
      Inc(I);
    end;

    if Temp <> '' then begin
      List := TDxList(FTopStrings);
      if Lines = nil then begin
        Lines := TDxLines.Create(List);
        List.AddObject('', Lines);
      end;
      ViewItem := Lines.AddItem(Temp, nil);
      Lines.TimeTick := GetTickCount + TimeOut * 1000;
      List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + Temp;
      ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
      ViewItem.Transparent := False;
      ViewItem.Caption := Temp;
      ViewItem.Color.Up.Color := FC;
      ViewItem.Color.Up.BColor := BC;
      ViewItem.Color.Up.Bold := False;
      ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
      ViewItem.Color.Down.Assign(ViewItem.Color.Up);
      ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
      ViewItem.Width := ImageCanvas.TextWidth(Temp);
      ViewItem.Height := ImageCanvas.TextHeight('0');
      Lines.Width := Lines.Width + ViewItem.Width;
      Lines.Height := Max(Lines.Height, ViewItem.Height);
      sText := '';
      MaxValue := MaxValue + Lines.Height;
      if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
        Position := Position + Lines.Height;
      end;
      if (Assigned(FOnChange)) then FOnChange(Self);
    end;

    if sText <> '' then
      AddTop(' ' + sText, FC, BC, TimeOut);
  end else begin
    List := TDxList(FTopStrings);
    if Lines = nil then begin
      Lines := TDxLines.Create(List);
      List.AddObject('', Lines);
    end;
    ViewItem := Lines.AddItem(sText, nil);
    List.Strings[List.Count - 1] := List.Strings[List.Count - 1] + sText;
    ViewItem.TimeTick := GetTickCount + TimeOut * 1000;
    ViewItem.Transparent := False;
    ViewItem.Caption := sText;
    ViewItem.Color.Up.Color := FC;
    ViewItem.Color.Up.BColor := BC;
    ViewItem.Color.Up.Bold := False;
    ViewItem.Color.Hot.Assign(ViewItem.Color.Up);
    ViewItem.Color.Down.Assign(ViewItem.Color.Up);
    ViewItem.Color.Disabled.Assign(ViewItem.Color.Up);
    ViewItem.Width := ImageCanvas.TextWidth(sText);
    ViewItem.Height := ImageCanvas.TextHeight('0');
    Lines.Width := Lines.Width + ViewItem.Width;
    Lines.Height := Max(Lines.Height, ViewItem.Height);

    MaxValue := MaxValue + Lines.Height;
    if FAutoScroll and (MaxValue > Height - RemoveSize) then begin
      Position := Position + Lines.Height;
    end;
    if (Assigned(FOnChange)) then FOnChange(Self);
  end;
  Result := Lines;
end;

procedure TDxChatMemo.DeleteTop(Index: Integer);
begin
  FTopStrings.Delete(Index);
  MaxValue := MaxValue - ItemHeight;
  if FAutoScroll then Previous;
end;

procedure TDxChatMemo.Clear;
begin
  FStrings.Clear;
  FTopStrings.Clear;
  MaxValue := 0;
  if FAutoScroll then Previous;
end;

procedure TDxChatMemo.InitButton;
var
  I, II: Integer;
  ViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
begin
  List := TDxList(FTopStrings);
  for I := 0 to List.Count - 1 do begin
    Lines := TDxLines(List.Objects[I]);
    for II := 0 to Lines.Count - 1 do begin
      ViewItem := Lines.Items[II];
      ViewItem.Down := False;
      ViewItem.Move := False;
    end;
  end;

  List := TDxList(FStrings);
  for I := 0 to List.Count - 1 do begin
    Lines := TDxLines(List.Objects[I]);
    for II := 0 to Lines.Count - 1 do begin
      ViewItem := Lines.Items[II];
      ViewItem.Down := False;
      ViewItem.Move := False;
    end;
  end;
end;

function TDxChatMemo.Click(X, Y: Integer): Boolean;
var
  I: Integer;
  nHeight: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nTop: Integer;
  nIndex: Integer;
  vRect: TRect;
  ViewItem: pTViewItem;
  SelViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
  SelLines: TDxLines;
  S: string;
begin
  Result := False;
  InitButton;
  //DebugOutStr(Format('TDxChatMemo.Click1: Left:%d,Top:%d,X:%d,Y:%d', [Left, Top, X, Y]));
  if inherited Click(X, Y) then begin
    Result := True;
    if (not Background) { and (MouseCaptureControl = Self)} then begin
      vRect := Bounds(Left, Top, Width, Height);
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
    //DebugOutStr(Format('TDxChatMemo.Click2: Left:%d,Top:%d,X:%d,Y:%d', [Left, Top, X, Y]));
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin

      end else begin
        //vRect := Bounds(Left, Top, Width, Height);

        S := '';
        SelLines := nil;
        SelViewItem := nil;
        nIndex := -1;
        nTop := 0;
        nHeight := (Y - vRect.Top);
        List := TDxList(FTopStrings);
        for I := 0 to TopCount - 1 do begin
          Lines := TDxLines(List.Objects[I]);
          nTop := nTop + Lines.Height;
          if nTop >= nHeight then begin
            SelLines := Lines;
            S := List.Strings[I];
            nIndex := I;
            break;
          end;
        end;

        if SelLines = nil then begin
          nTop := 0;
          List := TDxList(FStrings);
          for I := FTopIndex to List.Count - 1 do begin
            Lines := TDxLines(List.Objects[I]);
            nTop := nTop + Lines.Height;
            if nTop >= nHeight then begin
              SelLines := Lines;
              S := List.Strings[I];
              nIndex := I;
              break;
            end;
          end;
        end;

        if SelLines <> nil then begin
          nLeft := 0;
          nWidth := X - vRect.Left;
          for I := 0 to SelLines.Count - 1 do begin
            ViewItem := SelLines.Items[I];
            nLeft := nLeft + ViewItem.Width;
            if nLeft >= nWidth then begin
              SelViewItem := ViewItem;
              break;
            end;
          end;
          if SelViewItem = nil then begin
            if SelLines.Count > 0 then
              SelViewItem := SelLines.Items[0];
          end;
          if SelViewItem <> nil then begin
            case SelViewItem.Style of
              bsRadio: begin
                  for I := 0 to SelLines.Count - 1 do begin
                    SelLines.Items[I].Checked := False;
                  end;
                  SelViewItem.Checked := True;
                end;
              bsCheckBox: begin
                  SelViewItem.Checked := not SelViewItem.Checked;
                end;
            end;
            if Assigned(FOnItemClick) then
              FOnItemClick(Self, S, SelLines, SelViewItem);
          end;
        end;
      end;
    end;
  end;
end;

function TDxChatMemo.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nHeight: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nTop: Integer;
  nIndex: Integer;
  vRect: TRect;
  ViewItem: pTViewItem;
  SelViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
  SelLines: TDxLines;
  S: string;
  AOnMouseDown: TOnMouseDown;

begin
  Result := False;
  AOnMouseDown := OnMouseDown;
  OnMouseDown := nil;
  InitButton;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseDown) and (not FNextMouseDown) and (not FBarMouseDown) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        S := '';
        SelLines := nil;
        SelViewItem := nil;
        nIndex := -1;
        nTop := 0;
        nHeight := (Y - vRect.Top);
        List := TDxList(FTopStrings);
        for I := 0 to TopCount - 1 do begin
          Lines := TDxLines(List.Objects[I]);
          nTop := nTop + Lines.Height;
          if nTop >= nHeight then begin
            SelLines := Lines;
            S := List.Strings[I];
            nIndex := I;
            break;
          end;
        end;

        if SelLines = nil then begin
          nTop := 0;
          List := TDxList(FStrings);
          for I := FTopIndex to List.Count - 1 do begin
            Lines := TDxLines(List.Objects[I]);
            nTop := nTop + Lines.Height;
            if nTop >= nHeight then begin
              SelLines := Lines;
              S := List.Strings[I];
              nIndex := I;
              break;
            end;
          end;
        end;

        if SelLines <> nil then begin
          nLeft := 0;
          nWidth := X - vRect.Left;
          for I := 0 to SelLines.Count - 1 do begin
            ViewItem := SelLines.Items[I];
            nLeft := nLeft + ViewItem.Width;
            if nLeft >= nWidth then begin
              SelViewItem := ViewItem;
              break;
            end;
          end;
          if SelViewItem = nil then begin
            if SelLines.Count > 0 then
              SelViewItem := SelLines.Items[0];
          end;

          if SelViewItem <> nil then begin
            SelViewItem.Down := True;
            SelViewItem.Move := False;
          end;

          if Assigned(FOnItemMouseDown) then
            FOnItemMouseDown(Self, S, SelLines, SelViewItem);
        end;
      end;
    end;
    OnMouseDown := AOnMouseDown;
    if Assigned(OnMouseDown) then
      OnMouseDown(Self, Button, Shift, X, Y);
  end;
  OnMouseDown := AOnMouseDown;
end;

function TDxChatMemo.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nHeight: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nTop: Integer;
  nIndex: Integer;
  vRect: TRect;
  ViewItem: pTViewItem;
  SelViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
  SelLines: TDxLines;
  S: string;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if Result { and (MouseCaptureControl = Self)} then begin
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseMove) and (not FNextMouseMove) and (not FBarMouseMove) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        SelLines := nil;
        SelViewItem := nil;
        nIndex := -1;
        S := '';
        nTop := 0;
        nHeight := (Y - vRect.Top);
        List := TDxList(FTopStrings);
        for I := 0 to TopCount - 1 do begin
          Lines := TDxLines(List.Objects[I]);
          nTop := nTop + Lines.Height;
          if nTop >= nHeight then begin
            SelLines := Lines;
            S := List.Strings[I];
            nIndex := I;
            break;
          end;
        end;

        if SelLines = nil then begin
          nTop := 0;
          List := TDxList(FStrings);
          for I := FTopIndex to List.Count - 1 do begin
            Lines := TDxLines(List.Objects[I]);
            nTop := nTop + Lines.Height;
            if nTop >= nHeight then begin
              SelLines := Lines;
              S := List.Strings[I];
              nIndex := I;
              break;
            end;
          end;
        end;

        if SelLines <> nil then begin
          nLeft := 0;
          nWidth := X - vRect.Left;
          for I := 0 to SelLines.Count - 1 do begin
            ViewItem := SelLines.Items[I];
            nLeft := nLeft + ViewItem.Width;
            if nLeft >= nWidth then begin
              SelViewItem := ViewItem;
              break;
            end;
          end;
          if SelViewItem = nil then begin
            if SelLines.Count > 0 then
              SelViewItem := SelLines.Items[0];
          end;
          if SelViewItem <> nil then begin
          //SelViewItem.Down := True;
            SelViewItem.Move := True;
          end;
        {for I := 0 to SelLines.Count - 1 do begin
          //SelLines.Items[I].Down := True;
          SelLines.Items[I].Move := True;
        end;}
          if Assigned(FOnItemMouseMove) then
            FOnItemMouseMove(Self, S, SelLines, SelViewItem);
        end;
      end;
    end;
  end;
end;

function TDxChatMemo.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nHeight: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nTop: Integer;
  nIndex: Integer;
  vRect: TRect;
  ViewItem: pTViewItem;
  SelViewItem: pTViewItem;
  List: TDxList;
  Lines: TDxLines;
  SelLines: TDxLines;
  S: string;
begin
  Result := False;
  ReleaseDCapture;
  Downed := False;
  InitButton;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    if (not Background) and InRange(X, Y) then begin
      Downed := False;
      Result := True;

      vRect := Bounds(Left, Top, Width, Height);
      SelLines := nil;
      SelViewItem := nil;
      nIndex := -1;
      S := '';
      nTop := 0;
      nHeight := (Y - vRect.Top);
      List := TDxList(FTopStrings);
      for I := 0 to TopCount - 1 do begin
        Lines := TDxLines(List.Objects[I]);
        nTop := nTop + Lines.Height;
        if nTop >= nHeight then begin
          SelLines := Lines;
          nIndex := I;
          S := List.Strings[I];
          break;
        end;
      end;

      if SelLines = nil then begin
        nTop := 0;
        List := TDxList(FStrings);
        for I := FTopIndex to List.Count - 1 do begin
          Lines := TDxLines(List.Objects[I]);
          nTop := nTop + Lines.Height;
          if nTop >= nHeight then begin
            SelLines := Lines;
            nIndex := I;
            S := List.Strings[I];
            break;
          end;
        end;
      end;

      if SelLines <> nil then begin
        nLeft := 0;
        nWidth := X - vRect.Left;
        for I := 0 to SelLines.Count - 1 do begin
          ViewItem := SelLines.Items[I];
          nLeft := nLeft + ViewItem.Width;
          if nLeft >= nWidth then begin
            SelViewItem := ViewItem;
            break;
          end;
        end;
        if SelViewItem = nil then begin
          if SelLines.Count > 0 then
            SelViewItem := SelLines.Items[0];
        end;
        for I := 0 to SelLines.Count - 1 do begin
          SelLines.Items[I].Down := False;
          SelLines.Items[I].Move := False;
        end;
        if Assigned(FOnItemMouseMove) then
          FOnItemMouseMove(Self, S, SelLines, SelViewItem);
      end;

    end;
  end;
end;

procedure TDxChatMemo.DirectPaint(dsurface: TDxSurface);
var
  I, II, III, nIndex, nDrawHeight, nLeft, nTop, nX, nY, nOffsetX, nOffsetY: Integer;
  FaceIndex: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;

  Surface: TDxSurface;
  D: TDxSurface;
  vtRect: TRect;
  vbRect: TRect;
  PaintRect: TRect;

  ViewItem: pTViewItem;

  OldSize: Integer;

  List: TDxList;
  Lines: TDxLines;
  Font: TDxFont;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);

  Surface := TDxSurface.Create(ImageCanvas.DDraw);
  Surface.SetSize(Width, Height);

  nDrawHeight := 0;

  nLeft := OffSetX;
  nTop := OffSetY;
  nY := nTop;
  nHeight := 0;
  List := TDxList(FTopStrings);
  for I := 0 to TopCount - 1 do begin
    Lines := TDxLines(List.Objects[I]);
    nX := nLeft;
    nHeight := ItemHeight;
    for II := 0 to Lines.Count - 1 do begin
      ViewItem := Lines.Items[II];
      nHeight := Max(nHeight, ViewItem.Height);
      if ViewItem.Image <> nil then begin
        FaceIndex := -1;
        if ViewItem.Style = bsButton then begin
          if ViewItem.Down then begin
            FaceIndex := ViewItem.ImageIndex.Down;
            if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
              FaceIndex := ViewItem.ImageIndex.Up;
          end else
            if ViewItem.Move then begin
            FaceIndex := ViewItem.ImageIndex.Hot;
            if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
              FaceIndex := ViewItem.ImageIndex.Up;
          end else
            FaceIndex := ViewItem.ImageIndex.Up;
        end else begin
          if ViewItem.Move then begin
            if ViewItem.Checked then
              FaceIndex := ViewItem.ImageIndex.Down
            else
              if ViewItem.ImageIndex.Hot >= 0 then
              FaceIndex := ViewItem.ImageIndex.Hot
            else
              FaceIndex := ViewItem.ImageIndex.Up;
          end else begin
            if ViewItem.Checked then
              FaceIndex := ViewItem.ImageIndex.Down
            else
              FaceIndex := ViewItem.ImageIndex.Up;
          end;
        end;

        if FaceIndex >= 0 then begin
          D := ViewItem.Image.Images[FaceIndex];
          if D <> nil then begin
            Surface.Draw(nX, nY {+ (ItemHeight - D.Height) div 2}, D.ClientRect, D); //, False
          end;
        end;
      end;

      if ViewItem.ShowCaption and (ViewItem.Caption <> '') then begin
        OldSize := ImageFont.Size;
        if ViewItem.Down then begin
          Font := ViewItem.Color.Down;
        end else
          if ViewItem.Move then begin
          Font := ViewItem.Color.Hot;
        end else begin
          Font := ViewItem.Color.Up;
        end;

        nOffsetX := 0;
        nOffsetY := 0;
        if ViewItem.Style = bsButton then begin
          if ViewItem.Down then begin
            Font := ViewItem.Color.Down;
            nOffsetX := ViewItem.OffsetX;
            nOffsetY := ViewItem.OffsetY;
          end else
            if ViewItem.Move then begin
            Font := ViewItem.Color.Hot;
          end else
            Font := ViewItem.Color.Up;
        end else begin
          if ViewItem.Move then begin
            if ViewItem.Checked then
              Font := ViewItem.Color.Down
            else
              if ViewItem.ImageIndex.Hot >= 0 then
              Font := ViewItem.Color.Hot
            else
              Font := ViewItem.Color.Up;
          end else begin
            if ViewItem.Checked then
              Font := ViewItem.Color.Down
            else
              Font := ViewItem.Color.Up;
          end;
        end;

        SetImageFont(Font.Size, fsBold in Font.Style);

        if Font.Name <> '' then
          SetImageFont(GetFontIndex(Font.Name), Font.Size, fsBold in Font.Style);

        if Font.Bold then
          Surface.BoldTextOut(nX + nOffsetX, nY + nOffsetY + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style)
        else
          Surface.TextOut(nX + nOffsetX, nY + nOffsetY + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style, FFontBackTransparent);
        SetImageFont(OldSize);
      end;

      Inc(nX, ViewItem.Width);
    end;
    Inc(nY, nHeight);
    Inc(nDrawHeight, nHeight);
    if nDrawHeight >= ClientHeight then break;
  end;

  if nDrawHeight < ClientHeight then begin
    //nY := nTop;
    List := TDxList(FStrings);
    for I := FTopIndex to List.Count - 1 do begin
      Lines := TDxLines(List.Objects[I]);
      nX := nLeft;
      nHeight := ItemHeight;
      for II := 0 to Lines.Count - 1 do begin
        ViewItem := Lines.Items[II];
        nHeight := Max(nHeight, ViewItem.Height);
        if ViewItem.Image <> nil then begin
          FaceIndex := -1;
          if ViewItem.Style = bsButton then begin
            if ViewItem.Down then begin
              FaceIndex := ViewItem.ImageIndex.Down;
              if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                FaceIndex := ViewItem.ImageIndex.Up;
            end else
              if ViewItem.Move then begin
              FaceIndex := ViewItem.ImageIndex.Hot;
              if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
                FaceIndex := ViewItem.ImageIndex.Up;
            end else
              FaceIndex := ViewItem.ImageIndex.Up;
          end else begin
            if ViewItem.Move then begin
              if ViewItem.Checked then
                FaceIndex := ViewItem.ImageIndex.Down
              else
                if ViewItem.ImageIndex.Hot >= 0 then
                FaceIndex := ViewItem.ImageIndex.Hot
              else
                FaceIndex := ViewItem.ImageIndex.Up;
            end else begin
              if ViewItem.Checked then
                FaceIndex := ViewItem.ImageIndex.Down
              else
                FaceIndex := ViewItem.ImageIndex.Up;
            end;
          end;

          if FaceIndex >= 0 then begin
            D := ViewItem.Image.Images[FaceIndex];
            if D <> nil then begin
              Surface.Draw(nX, nY {+ (ItemHeight - D.Height) div 2}, D.ClientRect, D); //, False
            end;
          end;
        end;

        if ViewItem.ShowCaption and (ViewItem.Caption <> '') then begin
          OldSize := ImageFont.Size;

          if ViewItem.Down then begin
            Font := ViewItem.Color.Down;
          end else
            if ViewItem.Move then begin
            Font := ViewItem.Color.Hot;
          end else begin
            Font := ViewItem.Color.Up;
          end;

          nOffsetX := 0;
          nOffsetY := 0;

          if ViewItem.Style = bsButton then begin
            if ViewItem.Down then begin
              Font := ViewItem.Color.Down;
              nOffsetX := ViewItem.OffsetX;
              nOffsetY := ViewItem.OffsetY;
            end else
              if ViewItem.Move then begin
              Font := ViewItem.Color.Hot;
            end else
              Font := ViewItem.Color.Up;
          end else begin
            if ViewItem.Move then begin
              if ViewItem.Checked then
                Font := ViewItem.Color.Down
              else
                if ViewItem.ImageIndex.Hot >= 0 then
                Font := ViewItem.Color.Hot
              else
                Font := ViewItem.Color.Up;
            end else begin
              if ViewItem.Checked then
                Font := ViewItem.Color.Down
              else
                Font := ViewItem.Color.Up;
            end;
          end;

          SetImageFont(Font.Size, fsBold in Font.Style);

          if Font.Name <> '' then
            SetImageFont(GetFontIndex(Font.Name), Font.Size, fsBold in Font.Style);

          if Font.Bold then
            Surface.BoldTextOut(nX + nOffsetX, nY + nOffsetY + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style)
          else
            Surface.TextOut(nX + nOffsetX, nY + nOffsetY + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style, FFontBackTransparent);
          SetImageFont(OldSize);
        end;

        Inc(nX, ViewItem.Width);
      end;
      Inc(nY, nHeight);
      Inc(nDrawHeight, nHeight);
      if nDrawHeight >= ClientHeight then break;
    end;
  end;

  if ImageIndex.Image <> nil then begin
    if ImageIndex.Up >= 0 then begin
      D := ImageIndex.Image.Images[ImageIndex.Up];
      if D <> nil then begin
        Surface.Draw(0, 0, D, False);
      end;
    end;
  end;

  if FShowScroll then begin
    if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
      D := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
      if D <> nil then begin
        Surface.StretchDraw(Bounds(Surface.Width - D.Width, 0, D.Width, Surface.Height), D, False);
        Surface.Draw(Surface.Width - D.Width, 0, Bounds(0, 0, D.Width, 16), D, False);
        Surface.Draw(Surface.Width - D.Width, Surface.Height - 16, Bounds(0, D.Height - 16, D.Width, D.Height), D, False);
      end;
    end;

    if FPrevImageIndex.Image <> nil then begin
      nIndex := -1;
      if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then nIndex := FPrevImageIndex.Down
      else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then nIndex := FPrevImageIndex.Hot
      else if (FPrevImageIndex.Up >= 0) then nIndex := FPrevImageIndex.Up;
      if (nIndex >= 0) then begin
        D := FPrevImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, 1, D.ClientRect, D);
        end;
      end;
    end;

    if FNextImageIndex.Image <> nil then begin
      nIndex := -1;
      if FNextMouseDown and (FNextImageIndex.Down >= 0) then nIndex := FNextImageIndex.Down
      else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then nIndex := FNextImageIndex.Hot
      else if (FNextImageIndex.Up >= 0) then nIndex := FNextImageIndex.Up;

      if nIndex >= 0 then begin
        D := FNextImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, (Surface.Height - D.Height - 1), D.ClientRect, D);
        end;
      end;
    end;

    if FBarImageIndex.Image <> nil then begin
      nIndex := -1;
      if FBarMouseDown and (FBarImageIndex.Down >= 0) then nIndex := FBarImageIndex.Down
      else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then nIndex := FBarImageIndex.Hot
      else if (FBarImageIndex.Up >= 0) then nIndex := FBarImageIndex.Up;

      if nIndex >= 0 then begin
        D := FBarImageIndex.Image.Images[nIndex];
        if D <> nil then begin

          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, FBarTop, D.ClientRect, D);
        end;
      end;
    end;
  end;

  //vtRect := VirtualRect;
  //PaintRect := ReallyRect(vbRect, vtRect);

  dsurface.Draw(SurfaceX(Left), SurfaceY(Top), Surface.ClientRect, Surface, True);
  Surface.Free;
end;

{------------------------------------------------------------------------------}

{------------------------------------------------------------------------------}

constructor TDxListItem.Create;
begin
  inherited;
  FItemList := nil;
end;

destructor TDxListItem.Destroy;
var
  I: Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited;
end;

function TDxListItem.AddItem(const S: string; AObject: TObject): pTViewItem;
begin
  AddObject(S, AObject);
  Result := @FItemList[Length(FItemList) - 1];
end;

function TDxListItem.AddObject(const S: string; AObject: TObject): Integer;
var
  ViewItem: pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  ViewItem := @FItemList[Length(FItemList) - 1];
  ViewItem.Down := False;
  ViewItem.Move := False;
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  Result := inherited AddObject(S, AObject);
end;

procedure TDxListItem.Clear;
var
  I: Integer;
begin
  for I := 0 to Length(FItemList) - 1 do begin
    FItemList[I].Color.Free;
    FItemList[I].ImageIndex.Free;
  end;
  SetLength(FItemList, 0);
  FItemList := nil;
  inherited Clear;
end;

procedure TDxListItem.Delete(Index: Integer);
var
  I: Integer;
begin
  FItemList[Index].Color.Free;
  FItemList[Index].ImageIndex.Free;
  for I := 0 to Length(FItemList) - 2 do begin
    FItemList[I] := FItemList[I + 1];
  end;
  SetLength(FItemList, Length(FItemList) - 1);
  inherited Delete(Index);
end;

procedure TDxListItem.InsertObject(Index: Integer; const S: string;
  AObject: TObject);
var
  I: Integer;
  ViewItem: pTViewItem;
begin
  SetLength(FItemList, Length(FItemList) + 1);
  for I := Length(FItemList) - 1 downto Index do begin
    FItemList[I] := FItemList[I - 1];
  end;
  ViewItem := @FItemList[Index];
  ViewItem.Down := False;
  ViewItem.Move := False;
  ViewItem.Caption := S;
  ViewItem.Data := nil;
  ViewItem.Image := nil;
  ViewItem.Style := bsButton;
  ViewItem.Checked := False;
  ViewItem.Color := TDxCaptionColor.Create;
  ViewItem.ImageIndex := TDxImageIndex.Create;
  inherited InsertObject(Index, S, AObject);
end;

function TDxListItem.GetItem(Index: Integer): pTViewItem;
begin
  Result := @FItemList[Index];
end;
{------------------------------------------------------------------------------}

constructor TDxListView.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);

  FLines := TList.Create;
  FColCount := 3;
  HeaderHeight := 36;
  FDrawLineCount := (Height - HeaderHeight) div ItemHeight;
  SetLength(ColWidths, FColCount);
  ColWidths[0] := Width div 3;
  ColWidths[1] := ColWidths[0];
  ColWidths[2] := ColWidths[0];
end;

destructor TDxListView.Destroy;
var
  I: Integer;
begin
  for I := 0 to FLines.Count - 1 do
    TDxListItem(FLines[I]).Free;
  FLines.Free;
  inherited Destroy;
end;

procedure TDxListView.Clear;
var
  I: Integer;
begin
  for I := 0 to FLines.Count - 1 do
    TDxListItem(FLines[I]).Free;
  FLines.Clear;
end;

function TDxListView.GetCount: Integer;
begin
  Result := FLines.Count;
end;

procedure TDxListView.FillItemMouse;
var
  I, II: Integer;
  ListItem: TDxListItem;
begin
  for I := 0 to Count - 1 do begin
    ListItem := Items[I];
    for II := 0 to ListItem.Count - 1 do begin
      ListItem.Items[II].Down := False;
      ListItem.Items[II].Move := False;
    end;
  end;
end;

function TDxListView.Click(X, Y: Integer): Boolean;
var
  I: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nCol: Integer;
  nRow: Integer;
  vRect: TRect;
  ViewItem: pTViewItem;
  ListItem: TDxListItem;
begin
  Result := False;
  //DebugOutStr(Format('TDxListView.Click1: Left:%d,Top:%d,X:%d,Y:%d', [Left, Top, X, Y]));
  if inherited Click(X, Y) then begin
    Result := True;
    if (not Background) { and (MouseCaptureControl = Self)} then begin
      vRect := Bounds(Left, Top, Width, Height);
      vRect.Left := vRect.Right - FScrollSize;
      vRect.Right := vRect.Left + FScrollSize;

    //DebugOutStr(Format('TDxListView.Click2: Left:%d,Top:%d,X:%d,Y:%d', [Left, Top, X, Y]));
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin

      end else begin
        vRect := Bounds(Left, Top, Width, Height);
        nCol := 0;
        nLeft := 0;
        nWidth := X - vRect.Left;
        for I := 0 to Length(ColWidths) - 1 do begin
          nLeft := nLeft + ColWidths[I];
          if nLeft >= nWidth then begin
            nCol := I;
            break;
          end;
        end;
    //nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight + Position div ItemHeight;
        nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight;
        if nRow <= FDrawLineCount then begin
          nRow := nRow + Position div ItemHeight;
          if (nRow >= 0) and (nRow < FLines.Count) then begin
            ListItem := Items[nRow];
            if (nCol >= 0) and (nCol < ListItem.Count) then begin
              ViewItem := ListItem.Items[nCol];
              case ViewItem.Style of
                bsRadio: begin
                    for I := 0 to ListItem.Count - 1 do begin
                      ListItem.Items[I].Checked := False;
                    end;
                    ViewItem.Checked := True;
                  end;
                bsCheckBox: begin
                    ViewItem.Checked := not ViewItem.Checked;
                  end;
              end;
              if Assigned(FOnListItemClick) then
                FOnListItemClick(Self, nRow, nCol, ListItem, ViewItem);
            end;
          end;
        end;
      end;
    end;
  end;
end;

function TDxListView.InRange(X, Y: Integer): Boolean;
var
  nRow: Integer;
  boInrange: Boolean;
  vRect: TRect;
begin
  boInrange := False;
  vRect := Bounds(Left, Top, Width, Height);
  if PointInRect(Point(X, Y), vRect) then begin

    boInrange := True;

    if ShowScroll then begin
      if (X <= Left + Width - FScrollSize) and (Y >= Top + Height - FBottomHeight) then
        boInrange := False;
    end else begin
      if (Y >= Top + Height - FBottomHeight) then
        boInrange := False;
    end;

    if boInrange then begin
      nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight;
      boInrange := nRow <= FDrawLineCount;
      if not boInrange then begin
        if ShowScroll then begin
          vRect.Left := vRect.Right - FScrollSize;
          vRect.Right := vRect.Left + FScrollSize;
          boInrange := PointInRect(Point(X, Y), vRect);
        end;
      end;
    end;
    if Assigned(OnInRealArea) then begin
      vRect := Bounds(Left, Top, Width, Height);
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end else Result := False;
end;

function TDxListView.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nCol: Integer;
  nRow: Integer;
  vRect: TRect;

  ListItem: TDxListItem;
  AOnMouseDown: TOnMouseDown;
begin
  Result := False;
  AOnMouseDown := OnMouseDown;
  OnMouseDown := nil;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseDown) and (not FNextMouseDown) and (not FBarMouseDown) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        nCol := 0;
        nLeft := 0;
        nWidth := X - vRect.Left;
        for I := 0 to Length(ColWidths) - 1 do begin
          nLeft := nLeft + ColWidths[I];
          if nLeft >= nWidth then begin
            nCol := I;
            break;
          end;
        end;

        nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight;
        if nRow <= FDrawLineCount then begin
          nRow := nRow + Position div ItemHeight;
          if (nRow >= 0) and (nRow < FLines.Count) then begin
            ListItem := Items[nRow];
            if (nCol >= 0) and (nCol < ListItem.Count) then begin
              ListItem.Items[nCol].Down := True;
              ListItem.Items[nCol].Move := False;
            end;
          end;
        end;
      end;
    end;
    OnMouseDown := AOnMouseDown;
    if Assigned(OnMouseDown) then
      OnMouseDown(Self, Button, Shift, X, Y);
  end;
  OnMouseDown := AOnMouseDown;
end;

function TDxListView.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nCol: Integer;
  nRow: Integer;
  vRect: TRect;
  ListItem: TDxListItem;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if Result and (MouseCaptureControl = Self) then begin

    FillItemMouse;
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseMove) and (not FNextMouseMove) and (not FBarMouseMove) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        nCol := 0;
        nLeft := 0;
        nWidth := X - vRect.Left;
        for I := 0 to Length(ColWidths) - 1 do begin
          nLeft := nLeft + ColWidths[I];
          if nLeft >= nWidth then begin
            nCol := I;
            break;
          end;
        end;

        nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight;
        if nRow <= FDrawLineCount then begin
          nRow := nRow + Position div ItemHeight;
          if (nRow >= 0) and (nRow < FLines.Count) then begin
            ListItem := Items[nRow];
            if (nCol >= 0) and (nCol < ListItem.Count) then begin
          //ListItem.Items[nCol].Down := True;
              ListItem.Items[nCol].Move := True;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function TDxListView.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nCol: Integer;
  nRow: Integer;
  vRect: TRect;
  ListItem: TDxListItem;
begin
  Result := False;
  ReleaseDCapture;
  Downed := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    if (not Background) and InRange(X, Y) then begin
      Downed := False;
      Result := True;
      FillItemMouse;
      vRect := Bounds(Left, Top, Width, Height);
      nCol := 0;
      nLeft := 0;
      nWidth := X - vRect.Left;
      for I := 0 to Length(ColWidths) - 1 do begin
        nLeft := nLeft + ColWidths[I];
        if nLeft >= nWidth then begin
          nCol := I;
          break;
        end;
      end;
    //nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight + Position div ItemHeight;
      nRow := (Y - vRect.Top - HeaderHeight) div ItemHeight;
      if nRow <= FDrawLineCount then begin
        nRow := nRow + Position div ItemHeight;
        if (nRow >= 0) and (nRow < FLines.Count) then begin
          ListItem := Items[nRow];
          if (nCol >= 0) and (nCol < ListItem.Count) then begin
            ListItem.Items[nCol].Down := False;
            ListItem.Items[nCol].Move := False;
          end;
        end;
      end;
    end;
  end;
end;

procedure TDxListView.DirectPaint(dsurface: TDxSurface);
var
  I, II, III, nIndex, nCount, nLeft, nTop, n01: Integer;
  FaceIndex: Integer;
  nHeight: Integer;
  nMaxValue: Integer;
  nBarTop: Integer;

  Surface: TDxSurface;
  D: TDxSurface;
  vtRect: TRect;
  vbRect: TRect;
  PaintRect: TRect;

  ListItem: TDxListItem;
  ViewItem: pTViewItem;

  OldSize: Integer;

  Font: TDxFont;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);

  Surface := TDxSurface.Create(ImageCanvas.DDraw);
  Surface.SetSize(Width, Height);
  //Surface.FillRectAlpha(Surface.ClientRect,clRed,100);
  n01 := 0;
  nTop := HeaderHeight;
  nIndex := Position div ItemHeight;
  nMaxValue := Min((Height - HeaderHeight) div ItemHeight, FDrawLineCount);
  for I := nIndex to FLines.Count - 1 do begin
    ListItem := FLines[I];
    nCount := Min(ListItem.Count, FColCount);
    nLeft := OffSetX;
    for II := 0 to nCount - 1 do begin
      ViewItem := ListItem.Items[II];
      if II > 0 then
        nLeft := nLeft + ColWidths[II - 1];

      if ViewItem.Image <> nil then begin
        FaceIndex := -1;
        if ViewItem.Style = bsButton then begin
          if ViewItem.Down then begin
            FaceIndex := ViewItem.ImageIndex.Down;
            if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
              FaceIndex := ViewItem.ImageIndex.Up;
          end else
            if ViewItem.Move then begin
            FaceIndex := ViewItem.ImageIndex.Hot;
            if (FaceIndex < 0) and (ViewItem.ImageIndex.Up >= 0) then
              FaceIndex := ViewItem.ImageIndex.Up;
          end else
            FaceIndex := ViewItem.ImageIndex.Up;
        end else begin
          if ViewItem.Move then begin
            if ViewItem.Checked then
              FaceIndex := ViewItem.ImageIndex.Down
            else
              if ViewItem.ImageIndex.Hot >= 0 then
              FaceIndex := ViewItem.ImageIndex.Hot
            else
              FaceIndex := ViewItem.ImageIndex.Up;
          end else begin
            if ViewItem.Checked then
              FaceIndex := ViewItem.ImageIndex.Down
            else
              FaceIndex := ViewItem.ImageIndex.Up;
          end;
        end;

        if FaceIndex >= 0 then begin
          D := ViewItem.Image.Images[FaceIndex];
          if D <> nil then begin
            Surface.Draw(nLeft + (ColWidths[II] - D.Width) div 2, nTop + (ItemHeight - D.Height) div 2, D.ClientRect, D, False);
          end;
        end;
      end;

      if ViewItem.Caption <> '' then begin
        OldSize := ImageFont.Size;
        if ViewItem.Down then begin
          Font := ViewItem.Color.Down;
        end else
          if ViewItem.Move then begin
          Font := ViewItem.Color.Hot;
        end else begin
          Font := ViewItem.Color.Up;
        end;

        SetImageFont(Font.Size, fsBold in Font.Style);

        if Font.Name <> '' then
          SetImageFont(GetFontIndex(Font.Name), Font.Size, fsBold in Font.Style);

        if Font.Bold then
          Surface.BoldTextOut(nLeft, nTop + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style)
        else
          Surface.TextOut(nLeft, nTop + (ItemHeight - Surface.TextHeight('0')) div 2, ViewItem.Caption, Font.Color, Font.BColor, Font.Style);

        SetImageFont(OldSize);
      end;
    end;
    Inc(n01);
    Inc(nTop, ItemHeight);
    if n01 >= nMaxValue then break;
  end;

  if ImageIndex.Image <> nil then begin
    if ImageIndex.Up >= 0 then begin
      D := ImageIndex.Image.Images[ImageIndex.Up];
      if D <> nil then begin
        Surface.Draw(0, 0, D);
      end;
    end;
  end;

  if FShowScroll then begin
      //画滚动条
    if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
      D := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
      if D <> nil then begin
        Surface.StretchDraw(Bounds(Surface.Width - D.Width, 0, D.Width, Surface.Height), D, False);
        Surface.Draw(Surface.Width - D.Width, 0, Bounds(0, 0, D.Width, 16), D, False);
        Surface.Draw(Surface.Width - D.Width, Surface.Height - 16, Bounds(0, D.Height - 16, D.Width, D.Height), D, False);
      end;
    end;

    if FPrevImageIndex.Image <> nil then begin
      nIndex := -1;
      if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then nIndex := FPrevImageIndex.Down
      else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then nIndex := FPrevImageIndex.Hot
      else if (FPrevImageIndex.Up >= 0) then nIndex := FPrevImageIndex.Up;
      if (nIndex >= 0) then begin
        D := FPrevImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, 1, D.ClientRect, D);
        end;
      end;
    end;

    if FNextImageIndex.Image <> nil then begin
      nIndex := -1;
      if FNextMouseDown and (FNextImageIndex.Down >= 0) then nIndex := FNextImageIndex.Down
      else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then nIndex := FNextImageIndex.Hot
      else if (FNextImageIndex.Up >= 0) then nIndex := FNextImageIndex.Up;

      if nIndex >= 0 then begin
        D := FNextImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, (Surface.Height - D.Height - 1), D.ClientRect, D);
        end;
      end;
    end;

    if FBarImageIndex.Image <> nil then begin
      nIndex := -1;
      if FBarMouseDown and (FBarImageIndex.Down >= 0) then nIndex := FBarImageIndex.Down
      else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then nIndex := FBarImageIndex.Hot
      else if (FBarImageIndex.Up >= 0) then nIndex := FBarImageIndex.Up;

      if nIndex >= 0 then begin
        D := FBarImageIndex.Image.Images[nIndex];
        if D <> nil then begin

          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, FBarTop, D.ClientRect, D);
        end;
      end;
    end;
  end;

    //vtRect := VirtualRect;
    //PaintRect := ReallyRect(vbRect, vtRect);
  dsurface.Draw(SurfaceX(Left), SurfaceY(Top), Surface.ClientRect, Surface, True);
  Surface.Free;
end;

function TDxListView.GetViewItem(Index: Integer): TDxListItem;
begin
  Result := FLines[Index];
end;

function TDxListView.GetColWidth(Index: Integer): Integer;
begin
  Result := ColWidths[Index];
end;

procedure TDxListView.SetColWidth(Index: Integer; Value: Integer);
begin
  if ColWidths[Index] <> Value then begin
    ColWidths[Index] := Value;
  end;
end;

procedure TDxListView.SetColCount(Value: Integer);
var
  I: Integer;
begin
  if FColCount <> Value then begin
    FColCount := Max(Value, 0);
    SetLength(ColWidths, FColCount);
    for I := 0 to FColCount - 1 do
      ColWidths[I] := Width div FColCount;
  end;
end;

procedure TDxListView.DoScroll(Value: Integer);
begin

end;

function TDxListView.Add: TDxListItem;
begin
  Result := TDxListItem.Create;
  FLines.Add(Result);
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
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
    TDxTreeNode(FList.Items[I]).Free;
  FList.Free;
  CaptionColor.Free;
  inherited;
end;

procedure TDxTreeNode.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do begin
    TDxTreeNode(FList.Items[I]).Free;
  end;
  FList.Clear;
  Owner.RefTreeNodeList();
end;

procedure TDxTreeNode.Add(Item: TDxTreeNode);
begin
  FList.Add(Item);
  Item.Owner := Owner;
  Item.Parent := Self;
  Item.Expand := True;
  if (Item.Parent = nil) or Item.Parent.Expand then
    Owner.RefTreeNodeList();
end;

procedure TDxTreeNode.Delete(Item: TDxTreeNode);
begin
  FList.Remove(Item);
end;

procedure TDxTreeNode.SetCaption(Value: string);
begin
  if FCaption <> Value then
    FCaption := Value;
end;

procedure TDxTreeNode.SetExpand(Value: Boolean);
var
  I: Integer;
  TreeNode: TDxTreeNode;
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
        //TreeNode.Expand := Value;
        TreeNode.Checked := Value;
      end;
    end;
    Owner.RefTreeNodeList();
  end;
end;

function TDxTreeNode.GetLevel: Integer;
var
  P: TDxTreeNode;
begin
  Result := 0;
  P := Parent;
  while True do begin
    if P = nil then break;
    P := P.Parent;
    Inc(Result);
  end;
end;

procedure TDxTreeNode.SetChecked(Value: Boolean);
var
  I: Integer;
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

function TDxTreeNode.GetItem(Index: Integer): TDxTreeNode;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TDxTreeNode.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TDxTreeNode.IndexOf(Item: TDxTreeNode): Integer;
begin
  Result := FList.IndexOf(Item);
end;

function TDxTreeNode.ExpandCount: Integer;
var
  I: Integer;
  TreeNode: TDxTreeNode;
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

procedure TDxTreeNode.Add(TreeNodeList: TList);
var
  I: Integer;
  TreeNode: TDxTreeNode;
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

constructor TDxTreeView.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FList := TList.Create;
  TreeNodeList := TList.Create;
  FOnSelect := nil;
  FShowButton := True;
end;

destructor TDxTreeView.Destroy();
var
  I: Integer;
  TreeNode: TDxTreeNode;
begin
  for I := 0 to FList.Count - 1 do begin
    Items[I].Free;
  end;
  FList.Free;
  TreeNodeList.Free;
  inherited Destroy();
end;

procedure TDxTreeView.Clear;
var
  I: Integer;
  TreeNode: TDxTreeNode;
begin
  for I := 0 to FList.Count - 1 do begin
    Items[I].Free;
  end;
  FList.Clear;
  Position := 0;
  MaxValue := 16;
  RefTreeNodeList();
end;

function TDxTreeView.GetItem(Index: Integer): TDxTreeNode;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

function TDxTreeView.GetCount: Integer;
begin
  Result := FList.Count;
end;

procedure TDxTreeView.Add(Item: TDxTreeNode);
begin
  FList.Add(Item);
  Item.Owner := Self;
  Item.Parent := nil;
  //Item.Expand := True;
  RefTreeNodeList();
end;

procedure TDxTreeView.Delete(Item: TDxTreeNode);
begin
  FList.Remove(Item);
end;

function TDxTreeView.IndexOf(Item: TDxTreeNode): Integer;
var
  D, P: TDxTreeNode;
begin
  Result := -1;
  if Item.Parent = nil then
    Result := FList.IndexOf(Item)
  else begin
    D := Item;
    P := D.Parent;

    Result := P.List.IndexOf(D) + 1;

    D := P;
    P := P.Parent;

    while True do begin
      if P.Parent = nil then begin
        Inc(Result, FList.IndexOf(P) + 1);
        break;
      end else begin
        Inc(Result, P.Count);
      end;
      D := P;
      P := P.Parent;
      if P = nil then break;
    end;
  end;
end;

procedure TDxTreeView.RefTreeNodeList();
var
  I: Integer;
  TreeNode: TDxTreeNode;
begin
  TreeNodeList.Clear;
  for I := 0 to FList.Count - 1 do begin
    TreeNode := Items[I];
    TreeNode.Add(TreeNodeList);
    if not TreeNode.Expand then
      TreeNodeList.Add(TreeNode);
  end;
  MaxValue := TreeNodeList.Count * ItemHeight + ItemHeight;
end;

function TDxTreeView.Get(Index: Integer): TDxTreeNode;
begin
  if (Index >= 0) and (Index < TreeNodeList.Count) then
    Result := TreeNodeList[Index]
  else
    Result := nil;
end;

procedure TDxTreeView.DoScroll(Value: Integer);
begin

end;

function TDxTreeView.Click(X, Y: Integer): Boolean;
var
  I: Integer;
  nWidth: Integer;
  nLeft: Integer;
  nRow: Integer;
  vRect: TRect;
  TreeNode: TDxTreeNode;
begin
  Result := False;
  if inherited Click(X, Y) then begin
    Result := True;
    vRect := Bounds(Left, Top, Width, Height);
    vRect.Left := vRect.Right - FScrollSize;
    vRect.Right := vRect.Left + FScrollSize;
    if PointInRect(Point(X, Y), vRect) then begin

    end else begin

      vRect := Bounds(Left, Top, Width, Height);
      nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
      TreeNode := Indexs[nRow];

      if TreeNode <> nil then begin
        nLeft := (TreeNode.Level + 1) * ImageCanvas.TextWidth('0') * 2;
        vRect.Left := vRect.Left + nLeft;
        vRect.Right := vRect.Right - FScrollSize;
        if PointInRect(Point(X, Y), vRect) then begin
          TreeNode.Expand := not TreeNode.Expand;
          RefTreeNodeList();
          case TreeNode.Style of
            bsRadio: begin
                if not TreeNode.Checked then begin
                  for I := 0 to TreeNodeList.Count - 1 do begin
                    TDxTreeNode(TreeNodeList.Items[I]).Checked := False;
                  end;
                  TreeNode.Checked := True;
                end;
              end;
            bsCheckBox: begin
                TreeNode.Checked := not TreeNode.Checked;
              end;
          end;
          if Assigned(FOnSelect) then
            FOnSelect(Self, TreeNode);
        end;
      end;
    end;
  end;
end;

function TDxTreeView.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nLeft: Integer;
  nRow: Integer;
  vRect: TRect;
  TreeNode: TDxTreeNode;
begin
  Result := False;
  if inherited MouseDown(Button, Shift, X, Y) then begin
    Downed := True;
    Result := True;
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseDown) and (not FNextMouseDown) and (not FBarMouseDown) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        RefTreeNodeList();
        nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
        TreeNode := Indexs[nRow];

        if TreeNode <> nil then begin
          nLeft := (TreeNode.Level + 1) * ImageCanvas.TextWidth('0') * 2;
          vRect.Left := vRect.Left + nLeft;
          vRect.Right := vRect.Right - FScrollSize;
          if PointInRect(Point(X, Y), vRect) then begin
            for I := 0 to TreeNodeList.Count - 1 do begin
              TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
              TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
            end;
            TreeNode.MouseDowned := True;
            TreeNode.MouseMoveed := False;
            Exit;
          end;
        end;
      end;
    end;
  end;
  for I := 0 to TreeNodeList.Count - 1 do begin
    TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
    TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
  end;
end;

function TDxTreeView.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nLeft: Integer;
  nRow: Integer;
  vRect: TRect;
  TreeNode: TDxTreeNode;
begin
  Result := inherited MouseMove(Shift, X, Y);
  if (not Background) and (not Result) then begin
    if MouseCaptureControl = Self then
      if InRange(X, Y) then Downed := True
      else Downed := False;
  end;

  if Result or Downed then begin
    vRect := Bounds(Left, Top, Width, Height);
    if (not FPrevMouseMove) and (not FNextMouseMove) and (not FBarMouseMove) then begin
      if ShowScroll then begin
        vRect.Left := vRect.Right - FScrollSize;
        vRect.Right := vRect.Left + FScrollSize;
      end;
      if ShowScroll and PointInRect(Point(X, Y), vRect) then begin
        Result := True;
      end else begin
        Result := True;
        vRect := Bounds(Left, Top, Width, Height);

        nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
        TreeNode := Indexs[nRow];

        if TreeNode <> nil then begin
          nLeft := (TreeNode.Level + 1) * ImageCanvas.TextWidth('0') * 2;
          vRect.Left := vRect.Left + nLeft;
          vRect.Right := vRect.Right - FScrollSize;
          if PointInRect(Point(X, Y), vRect) then begin
          //TreeNode.MouseDowned := True;
            for I := 0 to TreeNodeList.Count - 1 do begin
            //TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
              TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
            end;
            TreeNode.MouseMoveed := True;
            Exit;
          end;
        end;
      end;
    end;
  end;
  for I := 0 to TreeNodeList.Count - 1 do begin
            //TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
    TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
  end;
end;

function TDxTreeView.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
  nLeft: Integer;
  nRow: Integer;
  vRect: TRect;
  TreeNode: TDxTreeNode;
begin
  Result := False;
  if inherited MouseUp(Button, Shift, X, Y) then begin
    Downed := False;
    Result := True;
    vRect := Bounds(Left, Top, Width, Height);

    nRow := (Y - vRect.Top) div ItemHeight + Position div ItemHeight;
    TreeNode := Indexs[nRow];

    if TreeNode <> nil then begin
      nLeft := (TreeNode.Level + 1) * ImageCanvas.TextWidth('0') * 2;
      vRect.Left := vRect.Left + nLeft;
      vRect.Right := vRect.Right - FScrollSize;
      if PointInRect(Point(X, Y), vRect) then begin
        Downed := False;
        //MouseMoveed := False;
        for I := 0 to TreeNodeList.Count - 1 do begin
          TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
          TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
        end;
        Exit;
      end;
    end;

  //MouseMoveed := False;
    for I := 0 to TreeNodeList.Count - 1 do begin
      TDxTreeNode(TreeNodeList.Items[I]).MouseDowned := False;
      TDxTreeNode(TreeNodeList.Items[I]).MouseMoveed := False;
    end;
  end else begin
    ReleaseCapture;
    Downed := False;

  end;
end;

procedure TDxTreeView.DirectPaint(dsurface: TDxSurface);
var
  I, II, III, nIndex, nCount, nLeft, nTop: Integer;

  X1, Y1, X2, Y2, nHeight, nWidth: Integer;

  FaceIndex: Integer;

  nMaxValue: Integer;
  nBarTop: Integer;

  Surface: TDxSurface;
  D: TDxSurface;
  vtRect: TRect;
  vbRect: TRect;
  PaintRect: TRect;
  TreeNode: TDxTreeNode;
  OldSize: Integer;
  Font: TDxFont;
begin
  if Assigned(FOnDirectPaint) then
    FOnDirectPaint(Self, dsurface)
  else
    if WLib <> nil then begin
    d := WLib.Images[FaceIndex];
    if d <> nil then begin
      dsurface.Draw(SurfaceX(Left), SurfaceY(Top), d.ClientRect, d, True);
    end;
  end;

  for I := 0 to DControls.Count - 1 do
    if TDControl(DControls[I]).Visible then
      TDControl(DControls[I]).DirectPaint(dsurface);

  Surface := TDxSurface.Create(ImageCanvas.DDraw);
  Surface.SetSize(Width, Height);

  nTop := FOffSetY;
  nIndex := Position div ItemHeight;
  nMaxValue := (Height - FOffSetY) div ItemHeight;

  nCount := Min(nMaxValue, TreeNodeList.Count - 1);
  for I := nIndex to nCount do begin
    TreeNode := TreeNodeList[I];

    if FShowButton then begin
      if TreeNode.Expand then begin //→
        nHeight := FOffSetX;
        nWidth := FOffSetX - 3;
        X1 := TreeNode.Level * FOffSetX * 2 + FOffSetX; //TreeNode.Level * nWidth;
        Y1 := nTop + (ItemHeight - nHeight) div 2;
        X2 := X1 + nWidth;
        Y2 := nTop + ItemHeight div 2;
        for II := Y1 to Y1 + nHeight do
          Surface.Line(X1, II, X2, Y2, 4760808);
      end else begin //↓
        nWidth := FOffSetX + 1;
        nHeight := FOffSetX - 4;

        X1 := TreeNode.Level * FOffSetX * 2 + FOffSetX; //TreeNode.Level * nWidth;
        Y1 := nTop + (ItemHeight - nHeight) div 2;
        X2 := X1 + nWidth div 2;
        Y2 := nTop + ItemHeight div 2 + nHeight;
        for II := X1 to X1 + nWidth - 1 do
          Surface.Line(II, Y1, X2, Y2, 4760808);
      end;
    end;

    if FShowButton then begin
      nLeft := TreeNode.Level * FOffSetX * 2 + FOffSetX * 2 + FOffSetX div 2;
    end else begin
      nLeft := TreeNode.Level * FOffSetX * 2 + FOffSetX;
    end;

    if TreeNode.Caption <> '' then begin
      OldSize := ImageFont.Size;
      if TreeNode.Checked then begin
        Font := TreeNode.CaptionColor.Down;
      end else begin
        if TreeNode.MouseDowned then begin
          Font := TreeNode.CaptionColor.Down;
        end else
          if TreeNode.MouseMoveed then begin
          Font := TreeNode.CaptionColor.Hot;
        end else begin
          Font := TreeNode.CaptionColor.Up;
        end;
      end;
      SetImageFont(Font.Size, fsBold in Font.Style);

      if Font.Name <> '' then
        SetImageFont(GetFontIndex(Font.Name), Font.Size, fsBold in Font.Style);

      if Font.Bold then
        Surface.BoldTextOut(nLeft, nTop + (ItemHeight - Surface.TextHeight('0')) div 2, TreeNode.Caption, Font.Color, Font.BColor, Font.Style)
      else
        Surface.TextOut(nLeft, nTop + (ItemHeight - Surface.TextHeight('0')) div 2, TreeNode.Caption, Font.Color, Font.BColor, Font.Style);

      SetImageFont(OldSize);
    end;
    Inc(nTop, ItemHeight);
  end;

  if ImageIndex.Image <> nil then begin
    if ImageIndex.Up >= 0 then begin
      D := ImageIndex.Image.Images[ImageIndex.Up];
      if D <> nil then begin
        Surface.Draw(0, 0, D, False);
      end;
    end;
  end;

  if FShowScroll then begin
    if (FScrollImageIndex.Image <> nil) and (FScrollImageIndex.Up >= 0) then begin
      D := FScrollImageIndex.Image.Images[FScrollImageIndex.Up];
      if D <> nil then begin
        Surface.StretchDraw(Bounds(Surface.Width - D.Width, 0, D.Width, Surface.Height), D, False);
        Surface.Draw(Surface.Width - D.Width, 0, Bounds(0, 0, D.Width, 16), D, False);
        Surface.Draw(Surface.Width - D.Width, Surface.Height - 16, Bounds(0, D.Height - 16, D.Width, D.Height), D, False);
      end;
    end;

    if FPrevImageIndex.Image <> nil then begin
      nIndex := -1;
      if FPrevMouseDown and (FPrevImageIndex.Down >= 0) then nIndex := FPrevImageIndex.Down
      else if FPrevMouseMove and (FPrevImageIndex.Hot >= 0) then nIndex := FPrevImageIndex.Hot
      else if (FPrevImageIndex.Up >= 0) then nIndex := FPrevImageIndex.Up;
      if (nIndex >= 0) then begin
        D := FPrevImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, 1, D.ClientRect, D);
        end;
      end;
    end;

    if FNextImageIndex.Image <> nil then begin
      nIndex := -1;
      if FNextMouseDown and (FNextImageIndex.Down >= 0) then nIndex := FNextImageIndex.Down
      else if FNextMouseMove and (FNextImageIndex.Hot >= 0) then nIndex := FNextImageIndex.Hot
      else if (FNextImageIndex.Up >= 0) then nIndex := FNextImageIndex.Up;

      if nIndex >= 0 then begin
        D := FNextImageIndex.Image.Images[nIndex];
        if D <> nil then begin
          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, (Surface.Height - D.Height - 1), D.ClientRect, D);
        end;
      end;
    end;

    if FBarImageIndex.Image <> nil then begin
      nIndex := -1;
      if FBarMouseDown and (FBarImageIndex.Down >= 0) then nIndex := FBarImageIndex.Down
      else if FBarMouseMove and (FBarImageIndex.Hot >= 0) then nIndex := FBarImageIndex.Hot
      else if (FBarImageIndex.Up >= 0) then nIndex := FBarImageIndex.Up;

      if nIndex >= 0 then begin
        D := FBarImageIndex.Image.Images[nIndex];
        if D <> nil then begin

          Surface.Draw((Surface.Width - FScrollSize) + (FScrollSize - D.Width) div 2 + 1, FBarTop, D.ClientRect, D);
        end;
      end;
    end;
  end;

  //vtRect := VirtualRect;
  //PaintRect := ReallyRect(vbRect, vtRect);

  dsurface.Draw(SurfaceX(Left), SurfaceY(Top), Surface.ClientRect, Surface, True);
  Surface.Free;

end;
{--------------------- TDWinManager --------------------------}


constructor TDWinManager.Create;
begin
  DWinList := TList.Create;
  MouseCaptureControl := nil;
  FocusedControl := nil;
  MouseWheelControl := nil;
end;

destructor TDWinManager.Destroy;
begin
  inherited Destroy;
end;

procedure TDWinManager.ClearAll;
begin
  DWinList.Clear;
end;

procedure TDWinManager.AddDControl(dcon: TDControl; Visible: Boolean);
begin
  dcon.Visible := Visible;
  DWinList.Add(dcon);
end;

procedure TDWinManager.DelDControl(dcon: TDControl);
var
  I: Integer;
begin
  for I := 0 to DWinList.Count - 1 do
    if DWinList[I] = dcon then begin
      DWinList.Delete(I);
      Break;
    end;
end;

function TDWinManager.KeyPress(var Key: Char): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then
    ActiveMenu := nil;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then
    ModalDWindow := nil;

  if (FocusedControl <> nil) and FocusedControl.Visible and (not FocusedControl.Enabled) then
    ReleaseDFocus;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := KeyPress(Key);
      Exit;
    end else
      ActiveMenu := nil;
    Key := #0;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        Result := KeyPress(Key);
      Exit;
    end else
      ModalDWindow := nil;
    Key := #0;

  end;

  if FocusedControl <> nil then begin
    if FocusedControl.Visible then begin
      Result := FocusedControl.KeyPress(Key);
    end else
      ReleaseDFocus;
  end;
   {for i:=0 to DWinList.Count-1 do begin
      if TDControl(DWinList[i]).Visible then begin
         if TDControl(DWinList[i]).KeyPress (Key) then begin
            Result := TRUE;
            break;
         end;
      end;
   end; }
end;

function TDWinManager.KeyDown(var Key: Word; Shift: TShiftState): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then
    ActiveMenu := nil;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then
    ModalDWindow := nil;

  if (FocusedControl <> nil) and FocusedControl.Visible and (not FocusedControl.Enabled) then
    ReleaseDFocus;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := KeyDown(Key, Shift);
      Exit;
    end else ActiveMenu := nil;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        Result := KeyDown(Key, Shift);
      Exit;
    end else ModalDWindow := nil;
  end;
  if FocusedControl <> nil then begin
     //DebugOutStr(FocusedControl.Name);
    if FocusedControl.Visible then
      Result := FocusedControl.KeyDown(Key, Shift)
    else
      ReleaseDFocus;
  end;
   {for i:=0 to DWinList.Count-1 do begin
      if TDControl(DWinList[i]).Visible then begin
         if TDControl(DWinList[i]).KeyDown (Key, Shift) then begin
            Result := TRUE;
            break;
         end;
      end;
   end; }
end;

function TDWinManager.MouseWheelDown(Shift: TShiftState; MousePos: TPoint): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then
    ActiveMenu := nil;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then
    ModalDWindow := nil;


  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        MouseWheelDown(Shift, MousePos);
      Result := True;
      Exit;
    end else ActiveMenu := nil;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        MouseWheelDown(Shift, MousePos);
      Result := True;
      Exit;
    end;
  end;

  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := MouseWheelDown(Shift, MousePos);
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).MouseWheelDown(Shift, MousePos) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.MouseWheelUp(Shift: TShiftState; MousePos: TPoint): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then
    ActiveMenu := nil;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then
    ModalDWindow := nil;


  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        MouseWheelUp(Shift, MousePos);
      Result := True;
      Exit;
    end else ActiveMenu := nil;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        MouseWheelUp(Shift, MousePos);
      Result := True;
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := MouseWheelUp(Shift, MousePos);
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).MouseWheelUp(Shift, MousePos) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.MouseMove(Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then begin
    ActiveMenu.Hide;
    ActiveMenu := nil;
  end;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then begin
    ModalDWindow := nil;
  end;

  if (MouseCaptureControl <> nil) and MouseCaptureControl.Visible and (not MouseCaptureControl.Enabled) then
    MouseCaptureControl := nil;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := MouseMove(Shift, LocalX(X), LocalY(Y));
      //Result := True;
      //Exit;
      if Result then Exit else begin
       { ActiveMenu.Hide;
        ActiveMenu := nil;  }
      end;
    end else begin
      ActiveMenu.Hide;
      ActiveMenu := nil;
    end;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        MouseMove(Shift, LocalX(X), LocalY(Y));
      Result := True;
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := MouseMove(Shift, LocalX(X), LocalY(Y));
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).MouseMove(Shift, X, Y) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then begin
    ActiveMenu.Hide;
    ActiveMenu := nil;
  end;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then begin
    ModalDWindow := nil;
  end;

  if (MouseCaptureControl <> nil) and MouseCaptureControl.Visible and (not MouseCaptureControl.Enabled) then
    MouseCaptureControl := nil;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := MouseDown(Button, Shift, LocalX(X), LocalY(Y));
      if Result then Exit else begin
        ActiveMenu.Hide;
        ActiveMenu := nil;
      end;
    end else begin
      ActiveMenu.Hide;
      ActiveMenu := nil;
    end;
  end;

  if ModalDWindow <> nil then begin

    if ModalDWindow.Visible then begin
      with ModalDWindow do
        MouseDown(Button, Shift, LocalX(X), LocalY(Y));

      Result := True;
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := MouseDown(Button, Shift, LocalX(X), LocalY(Y));
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).MouseDown(Button, Shift, X, Y) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := True;

  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then begin
    ActiveMenu.Hide;
    ActiveMenu := nil;
  end;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then begin
    ModalDWindow := nil;
  end;

  if (MouseCaptureControl <> nil) and MouseCaptureControl.Visible and (not MouseCaptureControl.Enabled) then
    MouseCaptureControl := nil;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := MouseUp(Button, Shift, LocalX(X), LocalY(Y));
      if Result then Exit else begin
        {ActiveMenu.Hide;
        ActiveMenu := nil;  }
      end;
    end else begin
      ActiveMenu.Hide;
      ActiveMenu := nil;
    end;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        Result := MouseUp(Button, Shift, LocalX(X), LocalY(Y));
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := MouseUp(Button, Shift, LocalX(X), LocalY(Y));
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).MouseUp(Button, Shift, X, Y) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.DblClick(X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := True;
  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then begin
    ActiveMenu.Hide;
    ActiveMenu := nil;
  end;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then begin
    ModalDWindow := nil;
  end;

  if (MouseCaptureControl <> nil) and MouseCaptureControl.Visible and (not MouseCaptureControl.Enabled) then
    MouseCaptureControl := nil;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := DblClick(LocalX(X), LocalY(Y));
      Result := True;
      if Result then Exit else begin
        ActiveMenu.Hide;
        ActiveMenu := nil;
      end;
    end else begin
      ActiveMenu.Hide;
      ActiveMenu := nil;
    end;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        Result := DblClick(LocalX(X), LocalY(Y));
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := DblClick(LocalX(X), LocalY(Y));
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).DblClick(X, Y) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

function TDWinManager.Click(X, Y: Integer): Boolean;
var
  I: Integer;
begin
  Result := True;
  if (ActiveMenu <> nil) and ActiveMenu.Visible and (not ActiveMenu.Enabled) then begin
    ActiveMenu.Hide;
    ActiveMenu := nil;
  end;

  if (ModalDWindow <> nil) and ModalDWindow.Visible and (not ModalDWindow.Enabled) then begin
    ModalDWindow := nil;
  end;

  if (MouseCaptureControl <> nil) and MouseCaptureControl.Visible and (not MouseCaptureControl.Enabled) then
    MouseCaptureControl := nil;

  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then begin
      with ActiveMenu do
        Result := Click(LocalX(X), LocalY(Y));
      if Result then Exit else begin
        if MouseCaptureControl <> ActiveMenu.DControl then begin
          ActiveMenu.Hide;
          ActiveMenu := nil;
        end;
      end;
    end else begin
      ActiveMenu.Hide;
      ActiveMenu := nil;
    end;
  end;

  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then begin
      with ModalDWindow do
        Result := Click(LocalX(X), LocalY(Y));
      Exit;
    end else ModalDWindow := nil;
  end;
  if MouseCaptureControl <> nil then begin
    with MouseCaptureControl do
      Result := Click(LocalX(X), LocalY(Y));
  end else
    for I := 0 to DWinList.Count - 1 do begin
      if TDControl(DWinList[I]).Visible and TDControl(DWinList[I]).Enabled then begin
        if TDControl(DWinList[I]).Click(X, Y) then begin
          Result := True;
          Break;
        end;
      end;
    end;
end;

procedure TDWinManager.Process;
var
  I: Integer;
begin
  for I := 0 to DWinList.Count - 1 do begin
    if TDControl(DWinList[I]).Visible then begin
      TDControl(DWinList[I]).Process;
    end;
  end;
  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then
      with ModalDWindow do
        Process;
  end;
  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then
      with ActiveMenu do begin
        Process;
      end;
  end;
end;

procedure TDWinManager.DirectPaint(dsurface: TDxSurface);
var
  I: Integer;
begin
  for I := 0 to DWinList.Count - 1 do begin
    if TDControl(DWinList[I]).Visible then begin
      TDControl(DWinList[I]).DirectPaint(dsurface);
    end;
  end;
  if ModalDWindow <> nil then begin
    if ModalDWindow.Visible then
      with ModalDWindow do
        DirectPaint(dsurface);
  end;
  if ActiveMenu <> nil then begin
    if ActiveMenu.Visible then
      with ActiveMenu do begin
        DirectPaint(dsurface);
      end;
  end;
end;

initialization
  begin
    DWinMan := TDWinManager.Create;
    MouseCaptureControl := nil; //mouse message
    FocusedControl := nil; //Key message
    MainWinHandle := 0;
    ModalDWindow := nil;
    MouseMoveControl := nil;
    MouseDownControl := nil;
    ActiveMenu := nil;
    LabelClickTimeTick := GetTickCount;
  end;

end.

