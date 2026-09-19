unit DxControls;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  Forms,
  DxCanvas,
  DxComponents,
  GameImages,
  Clipbrd,
  HGE,
  HGEFontEx,
  HGECanvas,
  SDK;
const
  CLIENTEXE = 1;
type

  TDxControlEngine = class;
  TDxControl = class;

  TOnInsertControl = procedure(Sender:TObject; AControl:TDxControl) of object;
  TOnRemoveControl = procedure(Sender:TObject; AControl:TDxControl) of object;
  TOnFindActiveControl = function(Sender:TObject; X, Y:Integer):TDxControl of object;

  TOnGetItem = procedure(Sender:TObject; var DxControl:TDxControl; const S:string; FC, BC:TColor) of object;

  TDxFont = class(TPersistent)

  private
    FOnChange:TNotifyEvent;
    FColor:TColor;
    FBColor:TColor;
    FName:TFontName;
    FStyle:TFontStyles;
    FSize:Integer;
    FBold:Boolean;

    procedure SetColor(Value:TColor);
    procedure SetBColor(Value:TColor);
    procedure SetFontName(Value:TFontName);
    procedure SetSize(Value:Integer);
    procedure SetStyle(Value:TFontStyles);
    procedure SetBold(Value:Boolean);
  protected

    procedure Changed; // dynamic;
  public

    constructor Create;
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); // override;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
  published
    property Color:TColor read FColor write SetColor;
    property BColor:TColor read FBColor write SetBColor;
    property Name:TFontName read FName write SetFontName;
    property Size:Integer read FSize write SetSize;
    property Style:TFontStyles read FStyle write SetStyle;
    property Bold:Boolean read FBold write SetBold;
  end;

  TDxCaptionColor = class(TPersistent)

  private
    FOnChange:TNotifyEvent;
    FUp:TDxFont;
    FHot:TDxFont;
    FDown:TDxFont;
    FChecked:TDxFont;
    FDisabled:TDxFont;

    procedure SetUp(Value:TDxFont);
    procedure SetHot(Value:TDxFont);
    procedure SetDown(Value:TDxFont);
    procedure SetChecked(Value:TDxFont);
    procedure SetDisabled(Value:TDxFont);
    procedure FontChange(Sender:TObject); stdcall;
  protected

  public
    constructor Create;
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
  published
    property Up:TDxFont read FUp write SetUp;
    property Hot:TDxFont read FHot write SetHot;
    property Down:TDxFont read FDown write SetDown;
    property Checked:TDxFont read FChecked write SetChecked;
    property Disabled:TDxFont read FDisabled write SetDisabled;
  end;

  TDxBorderColor = class(TDxCaptionColor)
  public
    constructor Create;
  end;

  TDxImageIndex = class(TPersistent)
  private
    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;
    FUp:Integer;
    FHot:Integer;
    FDown:Integer;
    FChecked:Integer;
    FDisabled:Integer;
    FOffsetX:Integer;
    FOffsetY:Integer;
    FUpdateValue:Integer;

    procedure SetUp(Value:Integer);
    procedure SetHot(Value:Integer);
    procedure SetDown(Value:Integer);
    procedure SetChecked(Value:Integer);
    procedure SetDisabled(Value:Integer);
    procedure SetOffsetX(Value:Integer);
    procedure SetOffsetY(Value:Integer);
    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
    procedure SetImage(Value:TGameImages);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage write SetImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;

    procedure BeginUpdate;
    procedure EndUpdate;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Up:Integer read FUp write SetUp;
    property Hot:Integer read FHot write SetHot;
    property Down:Integer read FDown write SetDown;
    property Checked:Integer read FChecked write SetChecked;
    property Disabled:Integer read FDisabled write SetDisabled;
    property OffsetX:Integer read FOffsetX write SetOffsetX;
    property OffsetY:Integer read FOffsetY write SetOffsetY;
  end;
  {$IF CLIENTEXE = 1}
  TDxControl = class(TObject)
  {$ELSE}
  TDxControl = class(TComponent)
  {$IFEND}
    private
      FGuiType:TGuiType;
      FClientRect:TRect;
      FVisible:Boolean;
      FEnabled:Boolean;
      FOnResize:TNotifyEvent;
      FOnShow:TNotifyEvent;
      FOnHide:TNotifyEvent;

      // FOnGetImage: TOnGetImage;
      FOnKeyDown:TKeyEvent;
      FOnKeyPress:TKeyPressEvent;
      FOnKeyUp:TKeyEvent;
      FOnClick:TOnClickEx;
      FOnDblClick:TOnClickEx;
      FOnMouseDown:TMouseEvent;
      FOnMouseMove:TMouseMoveEvent;
      FOnMouseUp:TMouseEvent;

      FOnMove:TNotifyEvent;

      FDesigning:Boolean; // 是否在设计期

      FRootCtrl:TDxControlEngine;

      FPopupMenu:TDxControl;
      FEnableFocus:Boolean;

      FOnInRealArea:TOnInRealArea;
      FOnPaint:TNotifyEvent;
      FOnStartPaint:TNotifyEvent;
      FOnStartSubPaint:TNotifyEvent;
      FOnStopPaint:TNotifyEvent;

      FOnCreate:TNotifyEvent;
      FOnDestroy:TNotifyEvent;
      FOnMouseLeave:TNotifyEvent;
      FOnMouseEnter:TNotifyEvent;

      FOnFocused:TNotifyEvent;
      FOnUpDate:TNotifyEvent;

      FOnFindActiveControl:TOnFindActiveControl;
      FFloating:Boolean;
      FMoveRange:TRect;
      FTabOrder:Integer;
      FCaption:TCaption;
      FRawText:string;
      FHint:TCaption;
      FAutoSize:Boolean;
      FAlignment:TAlignment;
      FAlign:TAlign;

      FShowName:TCaption;
      FReferenceX:TReferenceX;
      FAdjustYByHeight:Boolean;
      FTopAlignment:Boolean;

      FTransparent:Boolean;
      FBackgroundColor:TColor;

      FImageIndex:TDxImageIndex; // 图片位置
      FBorderColor:TDxBorderColor; // 边框颜色
      FDrawBorder:Boolean; // 是否画边框

      FCenter:Boolean;
      FOwnerMove:Boolean;

      FMouseDownX, FMouseDownY:Integer;

      FMouseEvents:TMouseEvents;
      FCanMouse:Boolean;
      FModalControl:TDxControl;

      FBlendMode:Integer;
      FMouseDownBlendMode:Integer;
      FMouseMoveBlendMode:Integer;

      FDrawCaptionFont:TDxFont; // add chongchong 2013-07-13

      FAddData1:Integer;
      FAddData2:Integer;
      FAddData3:Integer;
      FAddData4:Integer;

      FEnableMouse:Boolean;
      FControlID:Integer;
      {$IF  CLIENTEXE = 1}
      FOwner:TDxControl;
      FName:TComponentName;
      FTag:Longint;
      FComponents:TList;
      function GeComponent(AIndex:Integer):TDxControl;
      function GeComponentCount:Integer;
      function GeComponentIndex:Integer;
      procedure Insert(AComponent:TDxControl);
      procedure Remove(AComponent:TDxControl);
      procedure SeComponentIndex(Value:Integer);
      {$IFEND}

      function GetControl(Index:Integer):TDxControl;
      function GetControlCount:Integer;

      procedure ToFront(Index:Integer);
      procedure ToBack(Index:Integer);

      function FindControl(const AName:string):TDxControl;
      function GetCtrl(AName:string):TDxControl;

      function GetRootCtrl():TDxControlEngine;
      function GetFocusedCtrl():TDxControl;

      procedure SetShowNameA(Value:TCaption);
      procedure SetReferenceX(Value:TReferenceX);
      procedure SetAdjustYByHeight(Value:Boolean);
      procedure SetTopAlignment(Value:Boolean);

      procedure SetCenterA(Value:Boolean);

      procedure ImageIndexChange(Sender:TObject); stdcall;

      procedure ReleaseControl;
      procedure SetScrollControl;

      procedure SetMousePoint(X, Y:Integer);

      function GetFocused:Boolean;

      function GetMouseMoveed:Boolean;
      function GetMouseDowned:Boolean;

      procedure SetAutoSize(Value:Boolean);
      procedure SetAlign(Value:TAlign);

      procedure SetDesigning(Value:Boolean); // 是否在设计期

      procedure SetMouseMoveed(Value:Boolean);
      procedure SetMouseDowned(Value:Boolean);

      procedure SetClientRect(const Value:TRect);
      function GetPosition(const Index:Integer):Integer;
      procedure SetPosition(const Index, Value:Integer);
      procedure SetVisible(const Value:Boolean);
      procedure SetEnabled(const Value:Boolean);

      procedure SetOwner(const Value:TDxControl);
    protected
      SpotX, SpotY:Integer;
      {$IF  CLIENTEXE = 1}
       //需要测试添加FAutoSizeSetFlag变量后是否会引起控件GUI内存结构，导致的控件加载的问题
      FAutoSizeSetFlag:Integer; //HZQ 20230609 测试用于解决微端模式下，Texture延迟加载导致的控件Size问题，
      procedure CheckAutoSize(); virtual; //HZQ 20230609 测试用于解决微端模式下，Texture延迟加载导致的控件Size问题，
      procedure Notification(AComponent:TDxControl; Operation:TOperation); virtual;
      procedure SetName(const NewName:TComponentName); virtual;
      {$IFEND}
      procedure SetCaptionV(Value:TCaption); virtual;
      procedure SetCaptionA(Value:TCaption); virtual;
      procedure DoResize(var NewRect:TRect); virtual;
      procedure DoShow(); virtual;
      procedure DoHide(); virtual;
      procedure DoEnable(); virtual;
      procedure DoDisable(); virtual;
      procedure DoFocused(); virtual;
      procedure DoUnFocused(); virtual;
      function GetOnGetImage():TOnGetImage; virtual;
      procedure SetOnGetImage(Value:TOnGetImage); virtual;
      function GetVirtualRect():TRect; virtual;
      function GetVisibleRect():TRect; virtual;

      procedure FocusSomething();

      function CanMove:Boolean; virtual;

      procedure DoMouseEnter; virtual;
      procedure DoMouseLeave; virtual;

      procedure DoClick(X, Y:Integer); virtual;
      procedure DoDblClick(X, Y:Integer); virtual;
      procedure DoMouseDown(); virtual;
      procedure DoMouseMove(); virtual;
      procedure DoMouseUp(); virtual;

      procedure DoUpdate(); virtual;
      procedure DoPaint(); virtual;

      procedure DoCaptionChange(); virtual;

      procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); virtual;

      procedure FormatCaption;
      property BlendMode:Integer read FBlendMode write FBlendMode;
      property MouseDownBlendMode:Integer read FMouseDownBlendMode write FMouseDownBlendMode;
      property MouseMoveBlendMode:Integer read FMouseMoveBlendMode write FMouseMoveBlendMode;

    public
      Data:Pointer;
      ReserveIndex:Integer;

      property AddData1:Integer read FAddData1 write FAddData1;
      property AddData2:Integer read FAddData2 write FAddData2;
      property AddData3:Integer read FAddData3 write FAddData3;
      property AddData4:Integer read FAddData4 write FAddData4;

      {$IF  CLIENTEXE = 1}
      constructor Create(AOwner:TDxControl); virtual;
      {$ELSE}
      constructor Create(AOwner:TComponent); override;
      {$IFEND}

      destructor Destroy; override;
      {$IF  CLIENTEXE = 1}
      procedure DestroyComponents;
      function FindComponent(const AName:string):TDxControl; overload;
      function FindComponent(const ID:Integer):TDxControl; overload;

      procedure InserComponent(AComponent:TDxControl);
      procedure RemoveComponent(AComponent:TDxControl);

      property Components[Index:Integer]:TDxControl read GeComponent;
      property ComponentCount:Integer read GeComponentCount;
      property ComponentIndex:Integer read GeComponentIndex write SeComponentIndex;
      property Owner:TDxControl read FOwner write SetOwner;
      {$IFEND}

      procedure Move(X, Y:Integer);

      procedure Show();
      procedure Hide();
      procedure Close();
      procedure MoveBy(dx, dy:Integer);
      procedure ResizeBy(dx, dy:Integer);
      procedure ApplyConstraint(const Constraint:TRect);

      procedure Repaint; virtual;
      procedure BringToFront(); virtual;
      procedure SentToBack(); virtual;
      procedure Initialize; virtual;
      procedure Finalize; virtual;

      procedure Assign(Source:TDxControl); virtual;

      function FindActiveControl(X, Y:Integer):TDxControl; virtual;

      function InRange(X, Y:Integer):Boolean; virtual;
      function CanDraw:Boolean; overload; virtual;
      function CanDraw(DestRect:TRect):Boolean; overload; virtual;

      function ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect:TRect; var nX, nY:Integer; Texture:TTexture = nil):TRect;
      function GetPaintRect(DestRect, SrcRect, vtRect, vbRect:TRect; var nX, nY:Integer):TRect;
      procedure DrawRectColor(DestRect:TRect;
        Texture:TTexture; Color:TColor; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRectColor(DestRect, SrcRect, vtRect, vbRect:TRect;
        Texture:TTexture; Color:TColor; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRectColorAlpha(DestRect:TRect;
        Texture:TTexture; Color:TColor; Alpha:Byte = 255; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRectColorAlpha(DestRect, vtRect, vbRect:TRect;
        Texture:TTexture; Color:TColor; Alpha:Byte = 255; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRect(DestRect:TRect;
        Texture:TTexture; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRect(DestRect, vtRect, vbRect:TRect;
        Texture:TTexture; ABlendMode:Integer = Blend_Default); overload;

      procedure DrawRect(nPX, nPY:Integer; DestRect, vtRect, vbRect:TRect;
        Texture:TTexture; ABlendMode:Integer = Blend_Default); overload;

      procedure FillRect(DestRect:TRect; Color:TColor); overload;
      procedure FillRectAlpha(DestRect:TRect; Color:TColor; Alpha:Byte = 255); overload;
      procedure FrameRect(DestRect:TRect; Color:TColor); overload;

      procedure FillRect(DestRect, vtRect, vbRect:TRect; Color:TColor); overload;
      procedure FillRectAlpha(DestRect, vtRect, vbRect:TRect; Color:TColor; Alpha:Byte = 255); overload;
      procedure FrameRect(DestRect, vtRect, vbRect:TRect; Color:TColor); overload;

      procedure DblClick(X, Y:Integer); virtual;

      procedure MouseEnter; virtual;
      procedure MouseLeave; virtual;

      procedure KeyDown(var Key:Word; Shift:TShiftState); virtual;
      procedure KeyPress(var Key:Char); virtual;
      procedure KeyUp(var Key:Word; Shift:TShiftState); virtual;
      procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); virtual;
      procedure MouseMove(Shift:TShiftState; X, Y:Integer); virtual;
      procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); virtual;
      procedure MouseWheelDown(Shift:TShiftState; MousePos:TPoint); virtual;
      procedure MouseWheelUp(Shift:TShiftState; MousePos:TPoint); virtual;
      procedure DoMove(); virtual;

      procedure SetFocus(); virtual;
      procedure SetCapture(); virtual;

      procedure Update(); virtual;
      procedure Paint; virtual;

      procedure DrawCaption(HGEFont:THGEFont; AFont:TDxFont;
        const ACaption:string; const ADestRect, AVisibleRect, AVirtualRect:TRect;
        X:Integer = 0; Y:Integer = 0); overload;

      procedure DrawCaption(HGEFont:THGEFont;
        const Color:TColor; const ACaption:string;
        const ADestRect:TRect;
        X:Integer = 0; Y:Integer = 0); overload;

      procedure DrawCaption(HGEFont:THGEFont; AFont:TDxFont; ATextImages:TImageInfos;
        const ADestRect, AVisibleRect, AVirtualRect:TRect;
        X:Integer = 0; Y:Integer = 0; ExpandLineHeight:Integer = 0); overload;

      procedure DrawCaption(HGEFont:THGEFont; AFont:TDxFont; AAlignment:TAlignment; ATextImages:TImageInfos;
        const ADestRect, AVisibleRect, AVirtualRect:TRect;
        X:Integer = 0; Y:Integer = 0; ExpandLineHeight:Integer = 0); overload;

      procedure SetCenter();
      procedure WidthCenter();
      procedure HeightCenter();
      function ReallyRect(AVisibleRect, AVirtualRect:TRect):TRect;

      property Align:TAlign read FAlign write SetAlign;
      property Alignment:TAlignment read FAlignment write FAlignment;
      property Caption:TCaption read FCaption write SetCaptionA;

      property RawText:string read FRawText;

      property AutoSize:Boolean read FAutoSize write SetAutoSize;
      property DrawBorder:Boolean read FDrawBorder write FDrawBorder;
      property BackgroundColor:TColor read FBackgroundColor write FBackgroundColor;

      property CanMouse:Boolean read FCanMouse;
      property GuiType:TGuiType read FGuiType write FGuiType;
      property ClientRect:TRect read FClientRect write SetClientRect;
      property VisibleRect:TRect read GetVisibleRect;
      property VirtualRect:TRect read GetVirtualRect;
      property ControlCount:Integer read GetControlCount;
      property Control[Index:Integer]:TDxControl read GetControl;
      property OnGetImage:TOnGetImage read GetOnGetImage write SetOnGetImage;
      property OnKeyDown:TKeyEvent read FOnKeyDown write FOnKeyDown;
      property OnKeyPress:TKeyPressEvent read FOnKeyPress write FOnKeyPress;
      property OnKeyUp:TKeyEvent read FOnKeyUp write FOnKeyUp;
      property OnClick:TOnClickEx read FOnClick write FOnClick;
      property OnDblClick:TOnClickEx read FOnDblClick write FOnDblClick;
      property OnMouseDown:TMouseEvent read FOnMouseDown write FOnMouseDown;
      property OnMouseMove:TMouseMoveEvent read FOnMouseMove write FOnMouseMove;
      property OnMouseUp:TMouseEvent read FOnMouseUp write FOnMouseUp;
      property OnFindActiveControl:TOnFindActiveControl read FOnFindActiveControl write FOnFindActiveControl;
      property OnMove:TNotifyEvent read FOnMove write FOnMove;

      property MoveRange:TRect read FMoveRange write FMoveRange;
      property Focused:Boolean read GetFocused;

      property RootCtrl:TDxControlEngine read FRootCtrl;
      property FocusedCtrl:TDxControl read GetFocusedCtrl;

      property Ctrl[AName:string]:TDxControl read GetCtrl;

      property PopupMenu:TDxControl read FPopupMenu write FPopupMenu;

      property Center:Boolean read FCenter write SetCenterA;
      property MouseMoveed:Boolean read GetMouseMoveed write SetMouseMoveed;
      property MouseDowned:Boolean read GetMouseDowned write SetMouseDowned;
      property OnCreate:TNotifyEvent read FOnCreate write FOnCreate;
      property OnDestroy:TNotifyEvent read FOnDestroy write FOnDestroy;
      property OnMouseEnter:TNotifyEvent read FOnMouseEnter write FOnMouseEnter;
      property OnMouseLeave:TNotifyEvent read FOnMouseLeave write FOnMouseLeave;

      property OnInRealArea:TOnInRealArea read FOnInRealArea write FOnInRealArea;
      property OnPaint:TNotifyEvent read FOnPaint write FOnPaint;
      property OnStartPaint:TNotifyEvent read FOnStartPaint write FOnStartPaint;
      property OnStartSubPaint:TNotifyEvent read FOnStartSubPaint write FOnStartSubPaint;

      property OnStopPaint:TNotifyEvent read FOnStopPaint write FOnStopPaint;
      property OnFocused:TNotifyEvent read FOnFocused write FOnFocused;
      property OnUpDate:TNotifyEvent read FOnUpDate write FOnUpDate;

      property OnResize:TNotifyEvent read FOnResize write FOnResize;
      property OnShow:TNotifyEvent read FOnShow write FOnShow;
      property OnHide:TNotifyEvent read FOnHide write FOnHide;

      {$IF CLIENTEXE = 0}
      property Owner;
      {$IFEND}
    published
      {$IF CLIENTEXE = 1}
      property Name:TComponentName read FName write SetName stored False;
      property Tag:Longint read FTag write FTag default 0;
      {$IFEND}

      property ShowName:TCaption read FShowName write SetShowNameA;
      property ReferenceX:TReferenceX read FReferenceX write SetReferenceX;
      property AdjustYByHeight:Boolean read FAdjustYByHeight write SetAdjustYByHeight;
      property TopAlignment:Boolean read FTopAlignment write SetTopAlignment;

      property TabOrder:Integer read FTabOrder write FTabOrder;
      property Left:Integer index 0 read FClientRect.Left write SetPosition;
      property Top:Integer index 1 read FClientRect.Top write SetPosition;
      property Width:Integer index 2 read GetPosition write SetPosition;
      property Height:Integer index 3 read GetPosition write SetPosition;

      property Visible:Boolean read FVisible write SetVisible;
      property Enabled:Boolean read FEnabled write SetEnabled;
      property EnableMouse:Boolean read FEnableMouse write FEnableMouse;

      property MouseEvents:TMouseEvents read FMouseEvents write FMouseEvents;
      property ImageIndex:TDxImageIndex read FImageIndex write FImageIndex;
      property BorderColor:TDxBorderColor read FBorderColor write FBorderColor;
      property Transparent:Boolean read FTransparent write FTransparent; // 是否透明

      property Designing:Boolean read FDesigning write SetDesigning; // 是否在设计期
      property EnableFocus:Boolean read FEnableFocus write FEnableFocus; // 是否允许设置焦点
      property Floating:Boolean read FFloating write FFloating; // 是否可以拖动
      property OwnerMove:Boolean read FOwnerMove write FOwnerMove;
      property Hint:TCaption read FHint write FHint;

      property ModalControl:TDxControl read FModalControl write FModalControl;
      property ControlID:Integer read FControlID write FControlID;
    end;

  TDxControlEngine = class(TDxControl)
  private
    FOnBackgroundClick:TNotifyEvent;
    FActiveControl:TDxControl;
    FActiveMenu:TDxControl;
    FImeWindow:TDxControl; // 输入法窗口
    FCandidateWindow:TDxControl; // 候选字窗口
    FFocusedControl:TDxControl;
    FMouseDownControl:TDxControl;
    FMouseMoveControl:TDxControl;
    FScrollControl:TDxControl;

    FModalForms:TGList;
    FTopForms:TList;
    FOnRepaint:TNotifyEvent;
    FCriticalSection:TRTLCriticalSection;

    FToFrontList:TGList;
    FToBackList:TGList;
  protected
    function GetModalFormEx:TDxControl; //HZQ 20230525 Private
  private
    function GetModalForm:TDxControl;
    procedure SetModalForm(Value:TDxControl);
    procedure SetActiveMenu(Value:TDxControl);
    procedure SetFocusedControl(Value:TDxControl);
    procedure SetMouseDownControl(Value:TDxControl);
    procedure SetMouseMoveControl(Value:TDxControl);
    procedure SetScrollControl(Value:TDxControl);
    procedure SetActiveControl(Value:TDxControl);
  protected
    procedure ExecutePostion; //HZQ 20230525 Private
  private
    procedure SetImeWindow(Value:TDxControl);
    procedure SetCandidateWindow(Value:TDxControl);
  protected
    procedure DoResize(var NewRect:TRect); override;
  public
    WantReturn:Boolean;
    constructor Create();
    destructor Destroy; override;
    procedure Repaint; override;

    procedure Initialize; override;
    procedure Finalize; override;
    function FindActiveControl(X, Y:Integer):TDxControl; override;
    function DblClick(X, Y:Integer):Boolean; // override;
    function KeyDown(var Key:Word; Shift:TShiftState):Boolean; // override;
    function KeyPress(var Key:Char):Boolean; // override;
    function KeyUp(var Key:Word; Shift:TShiftState):Boolean; // override;
    function MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer):Boolean; // override;
    function MouseMove(Shift:TShiftState; X, Y:Integer):Boolean; // override;
    function MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer):Boolean; // override;
    function MouseWheelDown(Shift:TShiftState; MousePos:TPoint):Boolean; // override;
    function MouseWheelUp(Shift:TShiftState; MousePos:TPoint):Boolean; // override;
    procedure Paint; override;
    procedure Update(); override;
    procedure DeleteModalForm(D:TDxControl);
    procedure Lock;
    procedure UnLock;

    procedure AddBringToFront(D:TDxControl);
    procedure AddSentToBack(D:TDxControl);

    property ModalForm:TDxControl read GetModalForm write SetModalForm;
    property ActiveMenu:TDxControl read FActiveMenu write SetActiveMenu;
    property ImeWindow:TDxControl read FImeWindow write SetImeWindow; // 输入法窗口
    property CandidateWindow:TDxControl read FCandidateWindow write SetCandidateWindow; // 候选字窗口

    property FocusedControl:TDxControl read FFocusedControl write SetFocusedControl;
    property MouseDownControl:TDxControl read FMouseDownControl write SetMouseDownControl;
    property MouseMoveControl:TDxControl read FMouseMoveControl write SetMouseMoveControl;
    property ScrollControl:TDxControl read FScrollControl write SetScrollControl;
    property ActiveControl:TDxControl read FActiveControl write SetActiveControl;

    property OnBackgroundClick:TNotifyEvent read FOnBackgroundClick write FOnBackgroundClick;
    property OnRepaint:TNotifyEvent read FOnRepaint write FOnRepaint;
  end;

procedure SetClipboardText(const Text:WideString);
function GetClipboardText:WideString;

function FindDxComponent(Master:{$IF CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}; const ClassName:string):Boolean;
procedure GetAllSubComponents(Ctrl:TDxControl; List:TList);

implementation
uses Math,
  DxPopupMenu,
  DxImageForm,
  DxMemo,
  DxEdit,
  DxImageEdit,
  MShare,
  HUtil32;
var
  ControlEngineList:TList;

procedure SetClipboardText(const Text:WideString);
var
  Count:Integer;
  Handle:HGLOBAL;
  Ptr:Pointer;
begin
  Count := (Length(Text) + 1) * SizeOf(WideChar);
  Handle := GlobalAlloc(GMEM_MOVEABLE, Count);
  try
    if Handle = 0 then RaiseLastOSError; // Win32Check(Handle<>0);
    Ptr := GlobalLock(Handle);
    //Win32Check(Assigned(Ptr));
    if not Assigned(Ptr) then RaiseLastOSError; //HZQ 20230525解决报W1002 特有平台问题

    Move(PWideChar(Text)^, Ptr^, Count);
    GlobalUnlock(Handle);
    Clipboard.SetAsHandle(CF_UNICODETEXT, Handle);
  except
    GlobalFree(Handle);
    raise;
  end;
end;

function GetClipboardText:WideString;
var
  Data:THandle;
begin
  Clipboard.Open;
  Data := GetClipboardData(CF_UNICODETEXT);
  try
    if Data <> 0 then
      Result := PWideChar(GlobalLock(Data))
    else
      Result := '';
  finally
    if Data <> 0 then GlobalUnlock(Data);
    Clipboard.Close;
  end;
end;

procedure DebugOutStr(Msg:string);
var
  flname:string;
  fhandle:TextFile;
begin
  flname := '.\Debug.txt';
  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end
  else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  Writeln(fhandle, TimeToStr(Time) + ' ' + Msg);
  CloseFile(fhandle);
end;

function FindDxComponent(Master:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}; const ClassName:string):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to Master.ComponentCount - 1 do begin
    if CompareText(Master.Components[I].Name, ClassName) = 0 then begin
      Result := True;
      Exit;
    end
    else begin
      if FindDxComponent(Master.Components[I], ClassName) then begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function MakeGuiName(Master:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}; const ClassName:string):string;
var
  Num:Integer;
begin
  if (Pos('tdx', LowerCase(ClassName)) = 1) then begin
    Result := Copy(ClassName, 4, Length(ClassName) - 3);
  end
  else
    Result := ClassName;

  Num := 1;
  while FindDxComponent(Master, Result + IntToStr(Num)) do
    Inc(Num);
  Result := Result + IntToStr(Num);

  {Num := 1;
  while (Master.FindComponent(Result + IntToStr(Num)) <> nil) do Inc(Num);
  Result := Result + IntToStr(Num); }
end;

// ---------------------------------------------------------------------------

constructor TDxFont.Create;
begin
  inherited;
  FColor := clWhite;
  FBColor := clBlack;
  FName := ''; // 宋体
  FStyle := [];
  FSize := 9;
  FBold := False;
end;

destructor TDxFont.Destroy;
begin
  inherited;
end;

procedure TDxFont.Changed;
begin
  if Assigned(FOnChange) then FOnChange(Self);
end;

procedure TDxFont.SetColor(Value:TColor);
begin
  if FColor <> Value then begin
    FColor := Value;
    Changed;
  end;
end;

procedure TDxFont.SetBColor(Value:TColor);
begin
  if FBColor <> Value then begin
    FBColor := Value;
    Changed;
  end;
end;

procedure TDxFont.SetFontName(Value:TFontName);
begin
  if FName <> Value then begin
    FName := Value;
    Changed;
  end;
end;

procedure TDxFont.SetSize(Value:Integer);
begin
  if FSize <> Value then begin
    FSize := Value;
    Changed;
  end;
end;

procedure TDxFont.SetStyle(Value:TFontStyles);
begin
  if FStyle <> Value then begin
    FStyle := Value;
    Changed;
  end;
end;

procedure TDxFont.SetBold(Value:Boolean);
begin
  if FBold <> Value then begin
    FBold := Value;
    Changed;
  end;
end;

procedure TDxFont.Assign(Source:TPersistent);
begin
  // inherited;
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
  FChecked := TDxFont.Create;
  FDisabled := TDxFont.Create;

  FUp.Bold := True;
  FHot.Bold := True;
  FDown.Bold := True;
  FChecked.Bold := True;
  FDisabled.Bold := True;

  FUp.Color := clWhite;
  FHot.Color := clWhite;
  FDown.Color := clWhite;
  FChecked.Color := clWhite;
  FDisabled.Color := clBtnFace;

  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-13】 }
  // FDisabled := TDxFont.Create;

  FUp.OnChange := FontChange;
  FHot.OnChange := FontChange;
  FDown.OnChange := FontChange;
  FChecked.OnChange := FontChange;
  FDisabled.OnChange := FontChange;
end;

destructor TDxCaptionColor.Destroy;
begin
  FUp.Free;
  FHot.Free;
  FDown.Free;
  FChecked.Free;
  FDisabled.Free;
  inherited Destroy;
end;

procedure TDxCaptionColor.Assign(Source:TPersistent);
begin
  // inherited;
  if Source is TDxCaptionColor then begin
    FUp.Assign(TDxCaptionColor(Source).Up);
    FHot.Assign(TDxCaptionColor(Source).Hot);
    FDown.Assign(TDxCaptionColor(Source).Down);
    FChecked.Assign(TDxCaptionColor(Source).Checked);
    FDisabled.Assign(TDxCaptionColor(Source).Disabled);
  end;
end;
{------------------------------------------------------------------------------}

procedure TDxCaptionColor.SetUp(Value:TDxFont);
begin
  FUp.Assign(Value);
end;

procedure TDxCaptionColor.SetHot(Value:TDxFont);
begin
  FHot.Assign(Value);
end;

procedure TDxCaptionColor.SetDown(Value:TDxFont);
begin
  FDown.Assign(Value);
end;

procedure TDxCaptionColor.SetChecked(Value:TDxFont);
begin
  FChecked.Assign(Value);
end;

procedure TDxCaptionColor.SetDisabled(Value:TDxFont);
begin
  FDisabled.Assign(Value);
end;

procedure TDxCaptionColor.FontChange(Sender:TObject);
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TDxBorderColor.Create;
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
end;
// ---------------------------------------------------------------------------

constructor TDxImageIndex.Create;
begin
  inherited;
  FImageType := Prguse_wil;
  FUp := -1;
  FDown := -1;
  FHot := -1;
  FChecked := -1;
  FDisabled := -1;
  FOnChange := nil;
  FOnGetImage := nil;
  FImage := nil;
  FUpdateValue := 0;
end;

procedure TDxImageIndex.BeginUpdate;
begin
  Inc(FUpdateValue);
end;

procedure TDxImageIndex.EndUpdate;
begin
  if FUpdateValue > 0 then
    Dec(FUpdateValue);

  if FUpdateValue = 0 then begin
    Changed;
  end;
end;

procedure TDxImageIndex.Changed;
begin
  if FUpdateValue > 0 then Exit;

  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TDxImageIndex.Assign(Source:TPersistent);
begin
  // inherited;
  if Source is TDxImageIndex then begin
    Image := TDxImageIndex(Source).Image;
    OnGetImage := TDxImageIndex(Source).OnGetImage;
    ImageType := TDxImageIndex(Source).ImageType;

    Up := TDxImageIndex(Source).Up;
    Down := TDxImageIndex(Source).Down;
    Hot := TDxImageIndex(Source).Hot;
    Checked := TDxImageIndex(Source).Checked;
    Disabled := TDxImageIndex(Source).Disabled;
    OffsetX := TDxImageIndex(Source).OffsetX;
    OffsetY := TDxImageIndex(Source).OffsetY;
    Changed;
  end;
end;
{------------------------------------------------------------------------------}

procedure TDxImageIndex.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TDxImageIndex.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TDxImageIndex.SetImage(Value:TGameImages);
begin
  if FImage <> Value then begin
    FImage := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetUp(Value:Integer);
begin
  if FUp <> Value then begin
    FUp := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetHot(Value:Integer);
begin
  if FHot <> Value then begin
    FHot := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetDown(Value:Integer);
begin
  if FDown <> Value then begin
    FDown := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetChecked(Value:Integer);
begin
  if FChecked <> Value then begin
    FChecked := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetDisabled(Value:Integer);
begin
  if FDisabled <> Value then begin
    FDisabled := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetOffsetX(Value:Integer);
begin
  if FOffsetX <> Value then begin
    FOffsetX := Value;
    Changed;
  end;
end;

procedure TDxImageIndex.SetOffsetY(Value:Integer);
begin
  if FOffsetY <> Value then begin
    FOffsetY := Value;
    Changed;
  end;
end;

{-------------------------------------------------------------------------------}

constructor TDxControlEngine.Create();

  function FindComponentName(Master:TComponent; const ClassName:string):Boolean;
  var
    I:Integer;
  begin
    Result := False;
    for I := 0 to Master.ComponentCount - 1 do begin
      if CompareText(Master.Components[I].Name, ClassName) = 0 then begin
        Result := True;
        Exit;
      end
      else begin
        if FindComponentName(Master.Components[I], ClassName) then begin
          Result := True;
          Exit;
        end;
      end;
    end;
    for I := 0 to ControlEngineList.Count - 1 do begin
      if CompareText(TDxControlEngine(ControlEngineList[I]).Name, ClassName) = 0 then begin
        Result := True;
        Exit;
      end;
    end;
  end;

  function MakeName(Master:TComponent; const ClassName:string):string;
  var
    Num:Integer;
  begin
    if (Pos('tdx', LowerCase(ClassName)) = 1) then begin
      Result := Copy(ClassName, 4, Length(ClassName) - 3);
    end
    else
      Result := ClassName;

    Num := 1;
    while (Master <> nil) and FindComponentName(Master, Result + IntToStr(Num)) do
      Inc(Num);

    Result := Result + IntToStr(Num);
  end;
begin
  InitializeCriticalSection(FCriticalSection);
  inherited Create(nil);
  FRootCtrl := Self;
  Name := MakeName(Application.MainForm, ClassName);
  FOnRepaint := nil;
  FActiveMenu := nil;
  FFocusedControl := nil;
  FMouseDownControl := nil;
  FMouseMoveControl := nil;
  FScrollControl := nil;
  FActiveControl := nil;
  FModalForms := TGList.Create;
  FTopForms := TList.Create;
  ControlEngineList.Add(Self);
  FToFrontList := TGList.Create;
  FToBackList := TGList.Create;
end;

destructor TDxControlEngine.Destroy;
begin
  DeleteCriticalSection(FCriticalSection);
  FModalForms.Free;
  FTopForms.Free;
  FToFrontList.Free;
  FToBackList.Free;
  ControlEngineList.Remove(Self);
  inherited Destroy;
end;

procedure TDxControlEngine.DoResize(var NewRect:TRect);
begin
  inherited;
end;

procedure TDxControlEngine.Initialize;
begin
  {$IF IsMultiThreadRender = 1}
  Lock;
  try
    {$IFEND}
    inherited;
    {$IF IsMultiThreadRender = 1}
  finally
    UnLock;
  end;
  {$IFEND}
end;

procedure TDxControlEngine.Finalize;
begin
  {$IF IsMultiThreadRender = 1}
  Lock;
  try
    {$IFEND}
    inherited;
    {$IF IsMultiThreadRender = 1}
  finally
    UnLock;
  end;
  {$IFEND}
end;

procedure TDxControlEngine.Repaint;
begin
  if Assigned(FOnRepaint) then
    FOnRepaint(Self);
end;

procedure TDxControlEngine.SetActiveControl(Value:TDxControl);
begin
  if FActiveControl <> Value then begin
    FActiveControl := Value;
  end;
end;

function TDxControlEngine.FindActiveControl(X, Y:Integer):TDxControl;
var
  I:Integer;
  D:TDxControl;
begin
  D := nil;
  for I := 0 to ControlCount - 1 do begin
    D := Control[I].FindActiveControl(X, Y);
    if D <> nil then break;
  end;
  Result := D;
end;

function TDxControlEngine.GetModalForm:TDxControl;
begin
  // FModalForms.Lock;
  // try
  Result := nil;
  if FModalForms.Count > 0 then
    Result := TDxControl(FModalForms.Items[0]);
  // finally
  // FModalForms.UnLock;
  // end;
end;

function TDxControlEngine.GetModalFormEx:TDxControl;
begin
  // FModalForms.Lock;
  // try
  Result := nil;
  if FModalForms.Count > 0 then
    Result := TDxControl(FModalForms.Items[0]);
  // finally
  // FModalForms.UnLock;
  // end;
end;

procedure TDxControlEngine.SetModalForm(Value:TDxControl);
var
  nIndex:Integer;
begin
  // FModalForms.Lock;
  // try
  nIndex := FModalForms.IndexOf(Value);
  if nIndex >= 0 then begin
    FModalForms.Delete(nIndex);
    FModalForms.Insert(0, Value);
  end
  else begin
    FModalForms.Insert(0, Value);
  end;
  // finally
  // FModalForms.UnLock;
 // end;
end;

procedure TDxControlEngine.DeleteModalForm(D:TDxControl);
var
  nIndex:Integer;
begin
  // FModalForms.Lock;
  // try
  nIndex := FModalForms.IndexOf(D);
  if nIndex > 0 then begin
    FModalForms.Delete(nIndex);
  end;
  // finally
    // FModalForms.UnLock;
  // end;
end;

procedure TDxControlEngine.SetImeWindow(Value:TDxControl);
begin
  if FImeWindow <> Value then begin
    if FImeWindow <> nil then
      FImeWindow.Visible := False;
    FImeWindow := Value;
  end;
end;

procedure TDxControlEngine.SetCandidateWindow(Value:TDxControl);
begin
  if FCandidateWindow <> Value then begin
    if FCandidateWindow <> nil then
      FCandidateWindow.Visible := False;
    FCandidateWindow := Value;
  end;
end;

procedure TDxControlEngine.SetActiveMenu(Value:TDxControl);
begin
  if FActiveMenu <> Value then begin
    if FActiveMenu <> nil then
      FActiveMenu.Visible := False;
    FActiveMenu := Value;
  end;
end;

procedure TDxControlEngine.SetFocusedControl(Value:TDxControl);
begin
  if FFocusedControl <> Value then begin
    if FFocusedControl <> nil then
      FFocusedControl.DoUnFocused;
    FFocusedControl := Value;
    if FFocusedControl <> nil then
      FFocusedControl.DoFocused;
  end;
end;

procedure TDxControlEngine.SetMouseDownControl(Value:TDxControl);
begin
  if FMouseDownControl <> Value then begin
    FMouseDownControl := Value;
  end;
end;

procedure TDxControlEngine.SetMouseMoveControl(Value:TDxControl);
begin
  if FMouseMoveControl <> Value then begin
    FMouseMoveControl := Value;
  end;
end;

procedure TDxControlEngine.SetScrollControl(Value:TDxControl);
begin
  if FScrollControl <> Value then begin
    FScrollControl := Value;
  end;
end;

procedure TDxControlEngine.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDxControlEngine.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TDxControlEngine.ExecutePostion;
begin
  FToBackList.Lock;
  try
    while FToBackList.Count > 0 do begin
      TDxControl(FToBackList.Items[0]).ComponentIndex := ComponentCount - 1;
      FToBackList.Delete(0);
    end;
  finally
    FToBackList.UnLock;
  end;

  FToFrontList.Lock;
  try
    while FToFrontList.Count > 0 do begin
      TDxControl(FToFrontList.Items[0]).ComponentIndex := 0;
      FToFrontList.Delete(0);
    end;
  finally
    FToFrontList.UnLock;
  end;
end;

procedure TDxControlEngine.AddBringToFront(D:TDxControl);
begin
  FToFrontList.Lock;
  try
    FToFrontList.Add(D);
  finally
    FToFrontList.UnLock;
  end;
end;

procedure TDxControlEngine.AddSentToBack(D:TDxControl);
begin
  FToBackList.Lock;
  try
    FToBackList.Add(D);
  finally
    FToBackList.UnLock;
  end;
end;

function TDxControlEngine.DblClick(X, Y:Integer):Boolean;
var
  D:TDxControl;
  AModalForm:TDxControl;
begin
  Result := False;
  if (ActiveMenu <> nil) and ActiveMenu.Visible and ActiveMenu.InRange(X, Y) then begin
    ActiveMenu.DblClick(X, Y);
    Result := True;
    Exit;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    D := AModalForm.FindActiveControl(X, Y);
    if D <> nil then begin
      D.DblClick(X, Y);
      if (D.ModalControl = nil) and (AModalForm <> FModalControl) then
        D.ModalControl := AModalForm; // 检测双击后 显示模式窗体 会产生重复  MouseDown
      Result := True;
      Exit;
    end;

    if AModalForm.InRange(X, Y) then begin
      AModalForm.DblClick(X, Y);
      Result := True;
    end;
    Exit;
  end;

  D := FindActiveControl(X, Y);
  if D <> nil then begin
    D.DblClick(X, Y);
    if FModalControl = nil then
      FModalControl := ModalForm; // 检测双击后 显示模式窗体 会产生重复  MouseDown
    Result := True;
    Exit;
  end;
end;

function TDxControlEngine.KeyDown(var Key:Word; Shift:TShiftState):Boolean;
var
  AModalForm:TDxControl;
begin
  Result := False;
  if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
    ActiveMenu.KeyDown(Key, Shift);
    Result := True;
    Exit;
  end;

  if FFocusedControl <> nil then begin
    FFocusedControl.KeyDown(Key, Shift);
    Result := True;
    Exit;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    AModalForm.KeyDown(Key, Shift);
    Result := True;
  end;
end;

function TDxControlEngine.KeyPress(var Key:Char):Boolean;
var
  AModalForm:TDxControl;
begin
  Result := False;
  if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
    ActiveMenu.KeyPress(Key);
    Result := True;
    Exit;
  end;
  if FFocusedControl <> nil then begin
    FFocusedControl.KeyPress(Key);
    Result := True;
    Exit;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    AModalForm.KeyPress(Key);
    Result := True;
  end;
end;

function TDxControlEngine.KeyUp(var Key:Word; Shift:TShiftState):Boolean;
var
  AModalForm:TDxControl;
begin
  Result := False;
  if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
    ActiveMenu.KeyUp(Key, Shift);
    Result := True;
    Exit;
  end;

  if FFocusedControl <> nil then begin
    FFocusedControl.KeyUp(Key, Shift);
    Result := True;
    Exit;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    AModalForm.KeyUp(Key, Shift);
    Result := True;
  end;
end;

function TDxControlEngine.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer):Boolean;
var
  D:TDxControl;
  AModalForm:TDxControl;
begin
  Result := False;
  if (FModalControl <> nil) and (not FModalControl.Visible) then begin // 双击产生的重复MouseDown
    FModalControl := nil;
    Result := True;
    Exit;
  end;

  if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
    if ActiveMenu.InRange(X, Y) then begin
      ActiveMenu.MouseDowned := True;
      ActiveMenu.SetMousePoint(X, Y);
      ActiveMenu.SetFocus;
      ActiveMenu.MouseDown(Button, Shift, X, Y);
      Result := True;
      Exit;
    end
    else begin
      ActiveMenu.Visible := False;
    end;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    D := AModalForm.FindActiveControl(X, Y);
    if D <> nil then begin

      if (D.ModalControl <> nil) and (not D.ModalControl.Visible) then begin // 双击产生的重复MouseDown
        D.ModalControl := nil;
        Result := True;
        Exit;
      end;

      D.MouseDowned := True;
      D.SetMousePoint(X, Y);
      D.SetFocus;
      D.MouseDown(Button, Shift, X, Y);
      Result := True;
      Exit;
    end;

    if AModalForm.InRange(X, Y) then begin
      AModalForm.MouseDowned := True;
      AModalForm.SetMousePoint(X, Y);
      AModalForm.SetFocus;
      AModalForm.MouseDown(Button, Shift, X, Y);
    end
    else begin
      if MouseDownControl <> nil then
        MouseDownControl.MouseDowned := False;
    end;
    Result := True;
    Exit;
  end;

  D := FindActiveControl(X, Y);
  if D <> nil then begin
    D.MouseDowned := True;
    D.SetMousePoint(X, Y);
    D.SetFocus;
    D.MouseDown(Button, Shift, X, Y);
    Result := True;
  end
  else begin
    if MouseDownControl <> nil then
      MouseDownControl.MouseDowned := False;

    if Assigned(FOnBackgroundClick) then begin
      WantReturn := False;
      FOnBackgroundClick(Self);
      if WantReturn then Result := True;
    end;

    if Assigned(OnMouseDown) then begin
      WantReturn := False;
      OnMouseDown(Self, Button, Shift, X, Y);
      if WantReturn then Result := True;
    end;
  end;
end;

function TDxControlEngine.MouseMove(Shift:TShiftState; X, Y:Integer):Boolean;
var
  D:TDxControl;
  AModalForm:TDxControl;
begin
  Result := False;
  if MouseDownControl <> nil then begin
    MouseDownControl.Move(X, Y);
    MouseDownControl.MouseMove(Shift, X, Y);
    Result := True;
    Exit;
  end;

  if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
    if ActiveMenu.InRange(X, Y) then begin
      ActiveMenu.MouseMoveed := True;
      ActiveMenu.MouseMove(Shift, X, Y);
      Result := True;
      Exit;
    end;
  end;
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    D := AModalForm.FindActiveControl(X, Y);
    if D <> nil then begin
      D.MouseMoveed := True;
      D.MouseMove(Shift, X, Y);
      Result := True;
      Exit;
    end;

    if AModalForm.InRange(X, Y) then begin
      AModalForm.MouseMoveed := True;
      AModalForm.MouseMove(Shift, X, Y);
    end
    else begin
      if MouseMoveControl <> nil then
        MouseMoveControl.MouseMoveed := False;
    end;
    Result := True;
    Exit;
  end;

  D := FindActiveControl(X, Y);
  if D <> nil then begin
    D.MouseMoveed := True;
    D.MouseMove(Shift, X, Y);
    Result := True;
    Exit;
  end;

  if MouseMoveControl <> nil then
    MouseMoveControl.MouseMoveed := False;

  if Assigned(OnMouseMove) then begin
    WantReturn := False;
    OnMouseMove(Self, Shift, X, Y);
    if WantReturn then Result := True;
  end;
end;

function TDxControlEngine.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer):Boolean;
var
  D:TDxControl;
  AModalForm:TDxControl;
begin
  Result := False;
  if MouseDownControl <> nil then begin
    if MouseDownControl.InRange(X, Y) then begin
      // MouseDownControl.MouseUp(Button, Shift, X, Y);
      // MouseDownControl.MouseDowned := False;
      D := MouseDownControl;
      MouseDownControl.MouseDowned := False;
      D.MouseUp(Button, Shift, X, Y);
    end
    else begin
      MouseDownControl.MouseDowned := False;
    end;
    Result := True;
    Exit;
  end;

  { if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
     if ActiveMenu.InRange(X, Y) and ActiveMenu.MouseDowned then begin
       // ActiveMenu.MouseDowned := False;
       ActiveMenu.MouseUp(Button, Shift, X, Y);
     end;
     Result := True;
     Exit;
   end;

   if MouseDownControl <> nil then begin
     if MouseDownControl.InRange(X, Y) then begin

       MouseDownControl.MouseUp(Button, Shift, X, Y);
       // MouseDownControl.MouseDowned := False;
       // D := MouseDownControl;
       // MouseDownControl.MouseDowned := False;
       // D.MouseUp(Button, Shift, X, Y);
     end else begin
       MouseDownControl.MouseDowned := False;
     end;
     Result := True;
     Exit;
   end;

   if (ModalForm <> nil) and ModalForm.Visible then begin
     if ModalForm.InRange(X, Y) and ModalForm.MouseDowned then begin
       // ModalForm.MouseDowned := False;
       ModalForm.MouseUp(Button, Shift, X, Y);
     end;
     Result := True;
     Exit;
   end;  }
  AModalForm := ModalForm;
  if (AModalForm <> nil) and AModalForm.Visible then begin
    if AModalForm.InRange(X, Y) and AModalForm.MouseDowned then begin
      D := MouseDownControl;
      MouseDownControl.MouseDowned := False;
      D.MouseUp(Button, Shift, X, Y);
    end;
    Result := True;
    Exit;
  end;

  if Assigned(OnMouseUp) then begin
    WantReturn := False;
    OnMouseUp(Self, Button, Shift, X, Y);
    if WantReturn then Result := True;
  end;

  if Assigned(OnClick) then begin
    WantReturn := False;
    OnClick(Self, X, Y);
    if WantReturn then Result := True;
  end;
end;

function TDxControlEngine.MouseWheelDown(Shift:TShiftState; MousePos:TPoint):Boolean;
begin
  Result := False;
  if (FScrollControl <> nil) and (FScrollControl is TDxScrollControl) then begin
    FScrollControl.MouseWheelDown(Shift, MousePos);
    Result := True;
  end;
end;

function TDxControlEngine.MouseWheelUp(Shift:TShiftState; MousePos:TPoint):Boolean;
begin
  Result := False;
  if (FScrollControl <> nil) and (FScrollControl is TDxScrollControl) then begin
    FScrollControl.MouseWheelUp(Shift, MousePos);
    Result := True;
  end;
end;

procedure TDxControlEngine.Update();
var
  I:Integer;
begin
  {$IF IsMultiThreadRender = 1}
  Lock;
  try
    {$IFEND}
    for I := 0 to ControlCount - 1 do
      if Control[I].Visible and Control[I].Enabled then
        Control[I].Update();
    {$IF IsMultiThreadRender = 1}
  finally
    UnLock;
  end;
  {$IFEND}
end;

procedure TDxControlEngine.Paint;
var
  I:Integer;
  D:TDxControl;
begin

  {$IF IsMultiThreadRender = 1}
  ExecutePostion;
  Lock;
  try
    {$IFEND}

    for I := ControlCount - 1 downto 0 do begin
      if Control[I].Visible then begin
        if (Control[I] = ActiveMenu) or (Control[I] = ModalForm) then Continue;
        Control[I].Paint;
      end;
    end;

    D := ModalForm;
    if (D <> nil) and D.Visible then
      D.Paint;

    if (ActiveMenu <> nil) and ActiveMenu.Visible then
      ActiveMenu.Paint;

    {$IF IsMultiThreadRender = 1}
  finally
    UnLock;
  end;
  {$IFEND}
end;

// ---------------------------------------------------------------------------
{$IF  CLIENTEXE = 1}

constructor TDxControl.Create(AOwner:TDxControl);
begin
  if AOwner <> nil then AOwner.InserComponent(Self);

  FClientRect := Bounds(0, 0, 0, 0);
  FVisible := True;
  FEnabled := True;
  FCanMouse := True;
  FEnableMouse := True;
  Data := nil;
  ReserveIndex := -1;
  FOnKeyDown := nil;
  FOnKeyPress := nil;
  FOnKeyUp := nil;
  FOnClick := nil;
  FOnDblClick := nil;
  FOnMouseDown := nil;
  FOnMouseMove := nil;
  FOnMouseUp := nil;

  FOnCreate := nil;
  FOnDestroy := nil;
  FOnInRealArea := nil;
  // FOnGetImage := nil;
  FOnFocused := nil;
  FOnMouseLeave := nil;
  FOnMouseEnter := nil;
  FOnUpDate := nil;
  FRootCtrl := nil;
  FModalControl := nil;
  FOnFindActiveControl := nil;
  if (AOwner <> nil) then begin
    if (AOwner is TDxControlEngine) then
      FRootCtrl := TDxControlEngine(AOwner)
    else
      FRootCtrl := TDxControl(AOwner).RootCtrl;
  end;

  {  if FRootCtrl <> nil then
      Name := MakeGuiName(FRootCtrl, ClassName)
    else
      Name := ClassName; }

    // DebugOutStr(Name);

  if Owner <> nil then begin
    FMoveRange := Bounds(0, 0, TDxControl(Owner).Width, TDxControl(Owner).Height);
  end
  else
    FMoveRange := Rect(0, 0, 800, 600);

  FDesigning := True;

  FFloating := False;
  FPopupMenu := nil;

  FMouseEvents := [mbLeft, mbRight, mbMiddle];

  FCaption := '';
  FShowName := '';

  FReferenceX := rxLeft;
  FAdjustYByHeight := False;
  FTopAlignment := False;

  FAutoSize := True;
  FAlignment := taLeftJustify;
  FTransparent := True;

  FBackgroundColor := clWhite;

  FDrawBorder := False;

  FEnableFocus := False;
  FTabOrder := 0;
  FTransparent := True;
  FMouseDownX := 0;
  FMouseDownY := 0;
  FAlign := alNone;
  FCenter := False;
  FOwnerMove := False;

  FBlendMode := Blend_Default;
  FMouseDownBlendMode := Blend_Default;
  FMouseMoveBlendMode := Blend_Default;

  FBorderColor := TDxBorderColor.Create;
  FImageIndex := TDxImageIndex.Create;
  FImageIndex.OnChange := ImageIndexChange;

  // add chongchong 2012-07-13
  FDrawCaptionFont := TDxFont.Create;

  FAddData1 := 0;
  FAddData2 := 0;
  FAddData3 := 0;
  FAddData4 := 0;
end;

{$ELSE}

constructor TDxControl.Create(AOwner:TComponent);
begin
  inherited Create(AOwner);
  FClientRect := Bounds(0, 0, 0, 0);
  FVisible := True;
  FEnabled := True;
  FEnableMouse := True;
  // FCanClick := True;
  Data := nil;
  ReserveIndex := -1;
  FOnKeyDown := nil;
  FOnKeyPress := nil;
  FOnKeyUp := nil;
  FOnClick := nil;
  FOnDblClick := nil;
  FOnMouseDown := nil;
  FOnMouseMove := nil;
  FOnMouseUp := nil;

  FOnCreate := nil;
  FOnDestroy := nil;
  FOnInRealArea := nil;
  FOnFocused := nil;
  FOnMouseLeave := nil;
  FOnMouseEnter := nil;
  FOnUpDate := nil;
  FRootCtrl := nil;
  FModalControl := nil;
  FOnFindActiveControl := nil;
  if (AOwner <> nil) then begin
    if (AOwner is TDxControlEngine) then
      FRootCtrl := TDxControlEngine(AOwner)
    else
      FRootCtrl := TDxControl(AOwner).RootCtrl;
  end;

  if FRootCtrl <> nil then
    Name := MakeGuiName(FRootCtrl, ClassName)
  else
    Name := ClassName;

  // DebugOutStr(Name);

  if Owner <> nil then begin
    FMoveRange := Bounds(0, 0, TDxControl(Owner).Width, TDxControl(Owner).Height);
  end
  else
    FMoveRange := Rect(0, 0, 800, 600);

  FDesigning := True;

  FFloating := False;
  FPopupMenu := nil;

  FMouseEvents := [mbLeft, mbRight, mbMiddle];

  FCaption := '';
  FShowName := '';

  FAutoSize := True;
  FAlignment := taLeftJustify;
  FTransparent := True;

  FBackgroundColor := clWhite;

  FDrawBorder := False;

  FEnableFocus := False;
  FTabOrder := 0;
  FTransparent := True;
  FMouseDownX := 0;
  FMouseDownY := 0;
  FAlign := alNone;
  FCenter := False;
  FOwnerMove := False;

  FBlendMode := Blend_Default;
  FMouseDownBlendMode := Blend_Default;
  FMouseMoveBlendMode := Blend_Default;

  FBorderColor := TDxBorderColor.Create;
  FImageIndex := TDxImageIndex.Create;
  FImageIndex.OnChange := ImageIndexChange;

  // add chongchong 2012-07-13
  FDrawCaptionFont := TDxFont.Create;

  FAddData1 := 0;
  FAddData2 := 0;
  FAddData3 := 0;
  FAddData4 := 0;
  FAllwayTopShow := False;
end;
{$IFEND}

destructor TDxControl.Destroy;
begin
  FBorderColor.Free;
  FImageIndex.Free;

  RootCtrl.DeleteModalForm(Self);

  if RootCtrl.ActiveMenu = Self then begin
    RootCtrl.ActiveMenu := nil;
  end;

  if RootCtrl.FocusedControl = Self then begin
    RootCtrl.FocusedControl := nil;
  end;
  if RootCtrl.ScrollControl = Self then begin
    RootCtrl.ScrollControl := nil;
  end;

  if RootCtrl.ModalControl = Self then begin
    RootCtrl.ModalControl := nil;
  end;

  if RootCtrl.MouseDownControl = Self then begin
    MouseDowned := False;
  end;

  if RootCtrl.MouseMoveControl = Self then begin
    MouseMoveed := False;
  end;

  {$IF  CLIENTEXE = 1}
  DestroyComponents;
  if FOwner <> nil then
    FOwner.RemoveComponent(Self);
  {$IFEND}

  FDrawCaptionFont.Free;
  inherited Destroy;
end;
{$IF  CLIENTEXE = 1}

procedure TDxControl.Insert(AComponent:TDxControl);
begin
  if FComponents = nil then FComponents := TList.Create;
  FComponents.Add(AComponent);
  AComponent.FOwner := Self;
end;

procedure TDxControl.Remove(AComponent:TDxControl);
begin
  AComponent.FOwner := nil;
  if FComponents <> nil then begin
    FComponents.Remove(AComponent);
    if FComponents.Count = 0 then begin
      FComponents.Free;
      FComponents := nil;
    end;
  end;
end;

procedure TDxControl.InserComponent(AComponent:TDxControl);
begin
  Insert(AComponent);
  Notification(AComponent, opInsert);
end;

procedure TDxControl.RemoveComponent(AComponent:TDxControl);
begin
  Notification(AComponent, opRemove);
  Remove(AComponent);
end;

procedure TDxControl.DestroyComponents;
var
  Instance:TDxControl;
begin
  while FComponents <> nil do begin
    Instance := FComponents.Last;
    RemoveComponent(Instance);
    Instance.DestroyComponents;
    Instance.Destroy;
  end;
end;

procedure TDxControl.Notification(AComponent:TDxControl;
  Operation:TOperation);
var
  I:Integer;
begin
  if FComponents <> nil then begin
    for I := 0 to FComponents.Count - 1 do begin
      TDxControl(FComponents[I]).Notification(AComponent, Operation);
    end;
  end;
end;

function TDxControl.FindComponent(const AName:string):TDxControl;
var
  I:Integer;
begin
  if (AName <> '') and (FComponents <> nil) then
    for I := 0 to FComponents.Count - 1 do begin
      Result := FComponents[I];
      if SameText(Result.FName, AName) then Exit;
    end;
  Result := nil;
end;

function TDxControl.FindComponent(const ID:Integer):TDxControl;
var
  I:Integer;
  D:TDxControl;
begin
  if (ID <> 0) and (FComponents <> nil) then
    for I := 0 to FComponents.Count - 1 do begin
      D := FComponents[I];
      if D.ControlID = ID then begin
        Result := D;
        Exit;
      end;

      Result := D.FindComponent(ID);
      if Result <> nil then Exit;
    end;
  Result := nil;
end;

//HZQ 20230609 解决延迟加载素材导致的AutoSize设置问题
procedure TDxControl.CheckAutoSize();
var
    d:TTexture;
begin
    if AutoSize and (FAutoSizeSetFlag = 0) then begin
        if (ImageIndex.Up >= 0) and (ImageIndex.Image <> nil) then begin
            d := ImageIndex.Image.images[ImageIndex.Up];
            if (d <> nil) and ( d.Width * d.Height >= 4) then begin
                Self.FClientRect := Bounds(Self.Left, Self.Top, d.Width, d.Height);
                FAutoSizeSetFlag := $FF;
            end;
        end;
    end;
end;

procedure TDxControl.SetName(const NewName:TComponentName);
begin
  if FName <> NewName then begin
    FName := NewName;
  end;
end;

function TDxControl.GeComponentIndex:Integer;
begin
  if (FOwner <> nil) and (FOwner.FComponents <> nil) then
    Result := FOwner.FComponents.IndexOf(Self)
  else
    Result := -1;
end;

function TDxControl.GeComponent(AIndex:Integer):TDxControl;
begin
  if FComponents <> nil then begin
    Result := FComponents[AIndex];
  end else begin
    Result := nil; //HZQ 20230525
  end;
end;

function TDxControl.GeComponentCount:Integer;
begin
  if FComponents <> nil then
    Result := FComponents.Count
  else
    Result := 0;
end;

procedure TDxControl.SeComponentIndex(Value:Integer);
var
  I, Count:Integer;
begin
  if FOwner <> nil then begin
    I := FOwner.FComponents.IndexOf(Self);
    if I >= 0 then begin
      Count := FOwner.FComponents.Count;
      if Value < 0 then Value := 0;
      if Value >= Count then Value := Count - 1;
      if Value <> I then begin
        FOwner.FComponents.Delete(I);
        FOwner.FComponents.Insert(Value, Self);
      end;
    end;
  end;
end;
{$IFEND}

procedure TDxControl.Initialize;
var
  I:Integer;
begin
  for I := 0 to ControlCount - 1 do
    Control[I].Initialize;
end;

procedure TDxControl.Finalize;
var
  I:Integer;
begin
  for I := 0 to ControlCount - 1 do
    Control[I].Finalize;
end;

function TDxControl.GetControl(Index:Integer):TDxControl;
begin
  if (Index >= 0) and (Index < ComponentCount) then
    Result := TDxControl(Components[Index])
  else
    Result := nil;
end;

function TDxControl.GetControlCount:Integer;
begin
  Result := ComponentCount;
end;
// ---------------------------------------------------------------------------

function TDxControl.GetVisibleRect():TRect;
var
  OwnerSurface:TRect;
  MySurface:TRect;
begin
  // if (Owner = nil) or (Owner is TForm) then    //TDxControlEngine
  if not (Owner is TDxControl) then begin
    Result := ClientRect;
    Exit;
  end;

  // Retreive owner VisibleRect
  OwnerSurface := TDxControl(Owner).VisibleRect;

  // Calculate our theoretical VisibleRect, in absolute space
  MySurface := MoveRect(ClientRect, TDxControl(Owner).VirtualRect.TopLeft);

  // The intersection of both rectangles is our result
  Result := ShortRect(MySurface, OwnerSurface);
end;

// ---------------------------------------------------------------------------

function TDxControl.GetVirtualRect():TRect;
begin
  // if (Owner = nil) or (Owner is TForm) then    //TDxControlEngine
  if not (Owner is TDxControl) then begin
    Result := ClientRect;
    Exit;
  end;

  Result := MoveRect(ClientRect, TDxControl(Owner).VirtualRect.TopLeft);
end;

function TDxControl.GetOnGetImage():TOnGetImage;
begin
  Result := FImageIndex.FOnGetImage;
end;

procedure TDxControl.SetOnGetImage(Value:TOnGetImage);
begin
  // FOnGetImage := Value;
  FImageIndex.OnGetImage := Value;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.FocusSomething();
var
  I:Integer;
begin
  if Visible and Enabled and EnableMouse then begin
    for I := 0 to ControlCount - 1 do
      if ((Control[I] is TDxEdit) or (Control[I] is TDxImageEdit)) and (Control[I].Visible) and (Control[I].Enabled) and Control[I].EnableFocus then begin
        Control[I].SetFocus;
        if FocusedCtrl <> nil then Break;
      end;
    if FocusedCtrl = nil then begin
      for I := 0 to ControlCount - 1 do
        if (Control[I].Visible) and (Control[I].Enabled) and Control[I].EnableFocus then begin
          Control[I].SetFocus;
          if FocusedCtrl <> nil then Break;
        end;
    end;

    if FocusedCtrl = nil then begin
      for I := 0 to ControlCount - 1 do begin
        Control[I].FocusSomething();
        if FocusedCtrl <> nil then Exit;
      end;
    end;
  end;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.ToFront(Index:Integer);
var
  Aux:TDxControl;
begin
  if RootCtrl <> nil then begin
    if (Index >= 0) and (Index < ComponentCount) then begin
      Aux := Control[Index];
      if (Aux <> nil) and (Aux.Owner <> nil) and (Aux.ComponentIndex <> 0) then begin
        if Aux.Owner = RootCtrl then begin
          {$IF IsMultiThreadRender = 1}
          RootCtrl.AddBringToFront(Aux);
          {$ELSE}
          Aux.ComponentIndex := 0;
          {$IFEND}
        end
        else
          Aux.ComponentIndex := 0;
        Repaint;
      end;
    end;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.ToBack(Index:Integer);
var
  Aux:TDxControl;
begin
  if RootCtrl <> nil then begin
    if (Index >= 0) and (Index < ComponentCount) then begin
      Aux := Control[Index];
      if (Aux <> nil) and (Aux.Owner <> nil) and (Aux.ComponentIndex <> ComponentCount - 1) then begin
        if Aux.Owner = RootCtrl then begin
          {$IF IsMultiThreadRender = 1}
          RootCtrl.AddSentToBack(Aux);
          {$ELSE}
          Aux.ComponentIndex := Aux.Owner.ComponentCount - 1;
          {$IFEND}
        end
        else
          Aux.ComponentIndex := ComponentCount - 1;
        Repaint;
      end;
    end;
  end;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.BringToFront();
begin
  if (Owner <> nil) and (Owner is TDxControl) then begin
    TDxControl(Owner).ToFront(ComponentIndex);
    Repaint;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SentToBack();
begin
  if (Owner <> nil) and (Owner is TDxControl) then begin
    TDxControl(Owner).ToBack(ComponentIndex);
    Repaint;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetClientRect(const Value:TRect);
var
  NewRect:TRect;
begin
  NewRect := Value;
  DoResize(NewRect);
  FClientRect := NewRect;
  Repaint;
  if (Assigned(FOnResize)) then FOnResize(Self);
end;

// ---------------------------------------------------------------------------

function TDxControl.GetPosition(const Index:Integer):Integer;
begin
  case Index of
    2:Result := FClientRect.Right - FClientRect.Left;
    3:Result := FClientRect.Bottom - FClientRect.Top;
    else
      Result := 0;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetPosition(const Index, Value:Integer);
var
  Aux:Integer;
  NewRect:TRect;
begin
  NewRect := FClientRect;
  case Index of
    0:begin
        Aux := NewRect.Right - NewRect.Left;
        NewRect.Left := Value;
        NewRect.Right := Value + Aux;
      end;
    1:begin
        Aux := NewRect.Bottom - NewRect.Top;
        NewRect.Top := Value;
        NewRect.Bottom := Value + Aux;
      end;
    2:NewRect.Right := NewRect.Left + Value;
    3:NewRect.Bottom := NewRect.Top + Value;
  end;

  DoResize(NewRect);
  FClientRect := NewRect;
  Repaint;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetVisible(const Value:Boolean);
begin
  if (FVisible <> Value) then begin
    FVisible := Value;

    if (FVisible) then begin
      DoShow();
      if (Assigned(FOnShow)) then FOnShow(Self);
    end
    else begin
      if (Assigned(FOnHide)) then FOnHide(Self);
      DoHide();
    end;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.Show();
begin
  Visible := True;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.Hide();
begin
  Visible := False;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.Close();
begin
  Visible := False;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetEnabled(const Value:Boolean);
begin
  if (FEnabled <> Value) then begin
    FEnabled := Value;
    if (FEnabled) then
      DoEnable()
    else
      DoDisable();
  end;
end;

procedure TDxControl.SetOwner(const Value:TDxControl);
begin
  if (Value <> nil) and (Value <> FOwner) then begin
    FOwner.Remove(Self);
    Value.Insert(Self);
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.MoveBy(dx, dy:Integer);
begin
  Left := FClientRect.Left + dx;
  Top := FClientRect.Top + dy;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.ResizeBy(dx, dy:Integer);
begin
  Width := Width + dx;
  Height := Height + dy;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.ApplyConstraint(const Constraint:TRect);
begin
  FClientRect := ShortRect(FClientRect, Constraint);
end;

procedure TDxControl.Assign(Source:TDxControl);
begin
  Left := Source.Left;
  Top := Source.Top;
  Width := Source.Width;
  Height := Source.Height;
  Visible := Source.Visible;
  Enabled := Source.Enabled;
  EnableMouse := Source.EnableMouse;

  OnGetImage := Source.OnGetImage;
  OnKeyDown := Source.OnKeyDown;
  OnKeyPress := Source.OnKeyPress;
  OnKeyUp := Source.OnKeyUp;
  OnClick := Source.OnClick;
  OnDblClick := Source.OnDblClick;
  OnMouseDown := Source.OnMouseDown;
  OnMouseMove := Source.OnMouseMove;
  OnMouseUp := Source.OnMouseUp;

  OnCreate := Source.OnCreate;
  OnDestroy := Source.OnDestroy;
  OnMouseEnter := Source.OnMouseEnter;
  OnMouseLeave := Source.OnMouseLeave;
  OnInRealArea := Source.OnInRealArea;
  OnPaint := Source.OnPaint;
  OnStartPaint := Source.OnStartPaint;
  OnStopPaint := Source.OnStopPaint;
  OnFocused := Source.OnFocused;

  OnResize := Source.OnResize;

  MouseEvents := Source.MouseEvents;
  ImageIndex.Assign(Source.ImageIndex);
  BorderColor.Assign(Source.BorderColor);
  Transparent := Source.Transparent; // 是否透明

  Designing := Source.Designing; // 是否在设计期
  EnableFocus := Source.EnableFocus; // 是否允许设置焦点
  Floating := Source.Floating; // 是否可以拖动
  OwnerMove := Source.OwnerMove;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.ImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if FAutoSize and (FImageIndex.Image <> nil) then begin
    nIndex := -1;
    if FImageIndex.Up >= 0 then
      nIndex := FImageIndex.Up
    else if FImageIndex.Hot >= 0 then
      nIndex := FImageIndex.Hot
    else if FImageIndex.Down >= 0 then
      nIndex := FImageIndex.Down
    else if FImageIndex.Checked >= 0 then
      nIndex := FImageIndex.Checked
    else if FImageIndex.Disabled >= 0 then
      nIndex := FImageIndex.Disabled;

    if nIndex >= 0 then begin
      Texture := FImageIndex.Image.Images[nIndex];
      if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
        Width := Texture.Width;
        Height := Texture.Height;
        if FCenter and (Owner <> nil) and (not Designing) then begin
          Left := (TDxControl(Owner).Width - Width) div 2;
          Top := (TDxControl(Owner).Height - Height) div 2;
        end;
      end;
    end;
  end;
end;

// ---------------------------------------------------------------------------

function TDxControl.FindControl(const AName:string):TDxControl;
begin
  //Result := nil; HZQ 20230525

  if (AName = LowerCase(Self.Name)) then begin
    Result := Self;
    Exit;
  end;

  Result := TDxControl(FindComponent(AName));
end;

// ---------------------------------------------------------------------------

function TDxControl.GetCtrl(AName:string):TDxControl;
begin
  AName := LowerCase(AName);
  Result := FindControl(AName);
end;

// ---------------------------------------------------------------------------
(* //HZQ 2020520 原函数，可能不会返回值，所以修改
function TDxControl.GetRootCtrl(): TDxControlEngine;
begin
  //
  if ((Owner <> nil) and (Owner is TDxControlEngine)) then begin
    Result := TDxControlEngine(Owner);
    Exit;
  end;

  while (Owner <> nil) and (Owner is TDxControl) do begin //所有的控件都时TDxControl
     Result := TDxControl(Owner).GetRootCtrl;
  end;
end;
*)

//HZQ 20230520 NEW FUNCTION

function TDxControl.GetRootCtrl():TDxControlEngine;
begin
  Result := nil;
  while (Owner <> nil) do begin
    if Owner is TDxControlEngine then begin
      Result := TDxControlEngine(Owner);
      Break;
    end else begin
      Result := TDxControl(Owner).GetRootCtrl;
    end;
  end;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.SetAlign(Value:TAlign);
begin
  if FAlign <> Value then begin
    FAlign := Value;
    if Owner <> nil then begin
      case FAlign of
        alNone:;
        alTop:begin
            Width := TDxControl(Owner).Width;
            Top := 0;
            Left := 0;
          end;
        alBottom:begin
            Width := TDxControl(Owner).Width;
            Top := TDxControl(Owner).Height - Height;
            Left := 0;
          end;
        alLeft:begin
            Height := TDxControl(Owner).Height;
            Top := 0;
            Left := 0;
          end;
        alRight:begin
            Height := TDxControl(Owner).Height;
            Top := 0;
            Left := TDxControl(Owner).Width - Width;
          end;
        alClient:begin
            Height := TDxControl(Owner).Height;
            Width := TDxControl(Owner).Width;
            Top := 0;
            Left := 0;
          end;
        alCustom:;
      end;
    end;
  end;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.SetAutoSize(Value:Boolean);
begin
  if FAutoSize <> Value then begin
    FAutoSize := Value;
    ImageIndexChange(Self);
    DoCaptionChange();
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetCaptionV(Value:TCaption);
begin
  if FCaption <> Value then begin
    FCaption := Value;
    DoCaptionChange();
  end;
end;

procedure TDxControl.SetCaptionA(Value:TCaption);
begin
  if FCaption <> Value then begin
    FCaption := Value;
    FRawText := Value;
    DoCaptionChange();
  end;
end;

procedure TDxControl.SetShowNameA(Value:TCaption);
begin
  if FShowName <> Value then begin
    FShowName := Value;
  end;
end;

procedure TDxControl.SetReferenceX(Value:TReferenceX);
begin
  if FReferenceX <> Value then
    FReferenceX := Value;
end;

procedure TDxControl.SetAdjustYByHeight(Value:Boolean);
begin
  if FAdjustYByHeight <> Value then
    FAdjustYByHeight := Value;
end;

procedure TDxControl.SetTopAlignment(Value:Boolean);
begin
  if FTopAlignment <> Value then
    FTopAlignment := Value;
end;

procedure TDxControl.DoResize(var NewRect:TRect);
begin
  if Owner <> nil then begin
    case FAlign of
      alNone:;
      alTop:begin
          NewRect.Left := 0;
          NewRect.Top := 0;
          NewRect.Right := NewRect.Left + TDxControl(Owner).Width;
          NewRect.Bottom := NewRect.Top + Height;
        end;
      alBottom:begin
          NewRect.Left := 0;
          NewRect.Top := TDxControl(Owner).Height - Height;
          NewRect.Right := NewRect.Left + TDxControl(Owner).Width;
          NewRect.Bottom := NewRect.Top + Height;
        end;
      alLeft:begin
          NewRect.Left := 0;
          NewRect.Top := 0;
          NewRect.Right := NewRect.Left + Width;
          NewRect.Bottom := NewRect.Top + TDxControl(Owner).Height;
        end;
      alRight:begin
          NewRect.Left := TDxControl(Owner).Width - Width;
          NewRect.Top := 0;
          NewRect.Right := NewRect.Left + Width;
          NewRect.Bottom := NewRect.Top + TDxControl(Owner).Height;
        end;
      alClient:begin
          NewRect.Left := 0;
          NewRect.Top := 0;
          NewRect.Right := NewRect.Left + TDxControl(Owner).Width;
          NewRect.Bottom := NewRect.Top + TDxControl(Owner).Height;
        end;
      alCustom:;
    end;
  end;
  Repaint;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.DoCaptionChange();
begin

end;

procedure TDxControl.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  if Assigned(FOnInRealArea) then begin
    FOnInRealArea(Self, X, Y, IsRealArea);
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.DoHide();
begin
  ReleaseControl;
  // DebugOutStr(Name);
  if (Owner <> nil) and (Owner is TDxControl) then begin
    TDxControl(Owner).FocusSomething();

    if (FocusedCtrl <> nil) and ((FocusedCtrl is TDxEdit) or (FocusedCtrl is TDxImageEdit)) and (FocusedCtrl.Visible) and (FocusedCtrl.Enabled) and FocusedCtrl.EnableFocus then
      OpenIme
    else
      CloseIme;
  end;
  Repaint;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.DoDisable();
begin
  ReleaseControl;
  if (Owner <> nil) and (Owner is TDxControl) then begin
    // 设置控件可见时不要调用父控件的 FocusSomething；2019-09-06 18:11:48
    if (TDxControl(Owner).FocusedCtrl = Self) and (not Visible) then
      TDxControl(Owner).FocusSomething();

    if (FocusedCtrl <> nil) and ((FocusedCtrl is TDxEdit) or (FocusedCtrl is TDxImageEdit)) and (FocusedCtrl.Visible) and (FocusedCtrl.Enabled) and FocusedCtrl.EnableFocus then
      OpenIme
    else
      CloseIme;
  end;
  Repaint;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.DoEnable();
begin
  FocusSomething();
  if (FocusedCtrl <> nil) and ((FocusedCtrl is TDxEdit) or (FocusedCtrl is TDxImageEdit)) and (FocusedCtrl.Visible) and (FocusedCtrl.Enabled) and FocusedCtrl.EnableFocus then
    OpenIme
  else
    CloseIme;
  Repaint;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.DoShow();
begin
  SetFocus;
  if RootCtrl.FocusedControl <> Self then begin
    FocusSomething();

    if (FocusedCtrl <> nil) and ((FocusedCtrl is TDxEdit) or (FocusedCtrl is TDxImageEdit)) and (FocusedCtrl.Visible) and (FocusedCtrl.Enabled) and FocusedCtrl.EnableFocus then
      OpenIme
    else
      CloseIme;
  end;
  Repaint;
end;

// ---------------------------------------------------------------------------

function TDxControl.GetFocusedCtrl():TDxControl;
begin
  Result := RootCtrl.FocusedControl;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetMouseMoveed(Value:Boolean);
var
  D:TDxControl;
begin
  if Value then begin
    if (RootCtrl.MouseMoveControl <> nil) and (RootCtrl.MouseMoveControl <> Self) then
      RootCtrl.MouseMoveControl.DoMouseLeave();

    D := RootCtrl.MouseMoveControl;
    RootCtrl.MouseMoveControl := Self;

    if D <> Self then
      DoMouseEnter;

    DoMouseMove;
  end
  else begin
    if RootCtrl.MouseMoveControl <> nil then
      RootCtrl.MouseMoveControl.DoMouseLeave();
    RootCtrl.MouseMoveControl := nil;
  end;
end;

procedure TDxControl.SetMouseDowned(Value:Boolean);
begin
  if Value then begin
    if (RootCtrl.MouseDownControl <> nil) and (RootCtrl.MouseDownControl <> Self) then
      RootCtrl.MouseDownControl.DoMouseUp;

    RootCtrl.MouseDownControl := Self;
    DoMouseDown;
  end
  else begin
    if RootCtrl.MouseDownControl <> nil then
      RootCtrl.MouseDownControl.DoMouseUp;
    RootCtrl.MouseDownControl := nil;
  end;
end;

function TDxControl.GetMouseDowned:Boolean;
begin
  Result := Self = RootCtrl.MouseDownControl;
end;

function TDxControl.GetMouseMoveed:Boolean;
begin
  Result := Self = RootCtrl.MouseMoveControl;
end;

function TDxControl.GetFocused():Boolean;
begin
  Result := Self = RootCtrl.FocusedControl;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.SetCenterA(Value:Boolean);
begin
  if FCenter <> Value then begin
    FCenter := Value;
    if FCenter and (Owner <> nil) and (not Designing) then begin
      Left := (TDxControl(Owner).Width - Width) div 2;
      Top := (TDxControl(Owner).Height - Height) div 2;
    end;
  end;
end;

procedure TDxControl.SetCenter();
begin
  if (Owner <> nil) then begin
    Left := (TDxControl(Owner).Width - Width) div 2;
    Top := (TDxControl(Owner).Height - Height) div 2;
  end;
end;

procedure TDxControl.WidthCenter();
begin
  if (Owner <> nil) then begin
    Left := (TDxControl(Owner).Width - Width) div 2;
  end;
end;

procedure TDxControl.HeightCenter();
begin
  if (Owner <> nil) then begin
    Top := (TDxControl(Owner).Height - Height) div 2;
  end;
end;

procedure TDxControl.SetFocus();
var
  D:TDxControl;
  boCanFocus:Boolean;
begin
  boCanFocus := True;
  D := Self;
  while True do begin
    if not (D.Visible and (D.Enabled or ((not D.Enabled) and D.Designing))) then begin
      boCanFocus := False;
      break
    end;

    if D.Owner = RootCtrl then begin
      if (D is TDxImageForm) and D.Visible and (D.Enabled or ((not D.Enabled) and D.Designing)) then
        D.BringToFront;
      break;
    end;
    D := TDxControl(D.Owner);
  end;

  if boCanFocus and (RootCtrl.ControlCount > 0) then begin
    if RootCtrl.Control[0].Visible and
      (RootCtrl.Control[0].Enabled or ((not RootCtrl.Control[0].Enabled) and RootCtrl.Control[0].Designing)) and

    RootCtrl.Control[0].CanDraw and
      (RootCtrl.Control[0] is TDxScrollControl) then
      RootCtrl.ScrollControl := RootCtrl.Control[0]
    else
      RootCtrl.Control[0].SetScrollControl;
  end;

  if boCanFocus and Visible and (Enabled or ((not Enabled) and Designing)) and EnableFocus then begin
    RootCtrl.FocusedControl := Self;
    if Assigned(FOnFocused) then
      FOnFocused(Self);
  end;
end;

procedure TDxControl.SetCapture();
begin
  if Visible and (Enabled or Designing) then begin
    RootCtrl.MouseDownControl := Self;
  end;
end;

procedure TDxControl.SetScrollControl;
var
  I:Integer;
begin
  if Visible and (Enabled or ((not Enabled) and Designing)) and CanDraw then begin
    if (Self is TDxScrollControl) then
      RootCtrl.ScrollControl := Self
    else begin
      for I := 0 to ControlCount - 1 do begin
        Control[I].SetScrollControl;
        if RootCtrl.ScrollControl = Control[I] then break;
      end;
    end;
  end;
end;

procedure TDxControl.ReleaseControl;
var
  I:Integer;
begin
  RootCtrl.DeleteModalForm(Self);

  if RootCtrl.ActiveMenu = Self then begin
    RootCtrl.ActiveMenu := nil;
  end;

  if RootCtrl.FocusedControl = Self then begin
    RootCtrl.FocusedControl := nil;
  end;

  if RootCtrl.ScrollControl = Self then begin
    RootCtrl.ScrollControl := nil;
  end;

  if RootCtrl.MouseDownControl = Self then begin
    MouseDowned := False;
    // MouseCaptureControl := nil;
  end;

  if RootCtrl.MouseMoveControl = Self then begin
    MouseMoveed := False;
    // MouseMoveControl := nil;
  end;

  for I := 0 to ControlCount - 1 do begin
    Control[I].ReleaseControl;
  end;
end;

procedure TDxControl.SetDesigning(Value:Boolean);
var
  Index:Integer;
begin
  if FDesigning <> Value then begin
    FDesigning := Value;
    for Index := 0 to ControlCount - 1 do begin
      Control[Index].Designing := Value;
    end;
  end;
end;

// ---------------------------------------------------------------------------

procedure TDxControl.FormatCaption;
var
  nIdx:Integer;
  sVariable, sName, sCaption, s14:string;
  nPos:Integer;
begin
  if FRawText = '' then Exit;
  sCaption := RawText;
  s14 := sCaption;
  if (Pos('<', RawText) > 0) and (Pos('#', RawText) > 0) then begin
    nPos := ArrestVariable(s14, '<', '#', '>', 1, sVariable);
    if sVariable = '' then Exit;
    if CompareLStr(sVariable, '#MONEY(', Length('#MONEY(')) then begin
      ArrestStringEx(sVariable, '(', ')', sName);
      nIdx := g_MoneyList.GetIndex(sName);
      if nIdx >= 0 then begin
        sCaption := sub_49ADB8(nPos, sCaption, '<' + sVariable + '>', IntToStr(Integer(g_MoneyList.Objects[nIdx])));
      end
      else begin
        sCaption := sub_49ADB8(nPos, sCaption, '<' + sVariable + '>', '0');
      end;
    end;
  end;
  SetCaptionV(sCaption);
end;

procedure TDxControl.DoUpdate();
begin
  FormatCaption();
  if Assigned(FOnUpDate) then FOnUpDate(Self);
end;

// ---------------------------------------------------------------------------

procedure TDxControl.DoPaint();
begin
  {$IF  CLIENTEXE = 1}
  CheckAutoSize(); //HZQ 20230609 解决AutoSize因为延迟加载导致的Width和Height未设置的问题
  {$IFEND}
end;

// ---------------------------------------------------------------------------

procedure TDxControl.Update();
var
  I:Integer;
begin
  if (Enabled or ((not Enabled) and Designing)) and Visible then
    DoUpdate();

  for I := 0 to ControlCount - 1 do
    Control[I].Update();
end;

// ---------------------------------------------------------------------------

function TDxControl.ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect:TRect; var nX, nY:Integer;
  Texture:TTexture):TRect;
var
  PaintRect:TRect;
  nLeft, nTop, nWidth, nHeight:Integer;
begin
  nX := -1;
  nY := -1;
  Result := Rect(0, 0, 0, 0);
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;
  if Texture <> nil then
    SrcRect := ShortRect(SrcRect, Texture.ClientRect);
  PaintRect := ShortRect(Bounds(SrcRect.Left + nLeft, SrcRect.Top + nTop, nWidth, nHeight), SrcRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

  // GameCanvas.DrawColor(nX, nY, PaintRect, Texture, Color);

  Result := PaintRect;
end;

function TDxControl.GetPaintRect(DestRect, SrcRect, vtRect, vbRect:TRect; var nX, nY:Integer):TRect;
var
  PaintRect:TRect;
  nLeft, nTop, nWidth, nHeight:Integer;
begin
  nX := -1;
  nY := -1;
  Result := Rect(0, 0, 0, 0);
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  PaintRect := ShortRect(DestRect, vbRect);
  nWidth := Min(PaintRect.Right - PaintRect.Left, SrcRect.Right - SrcRect.Left);
  nHeight := Min(PaintRect.Bottom - PaintRect.Top, SrcRect.Bottom - SrcRect.Top);

  {if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;}

  if (nHeight <= 0) or (nWidth <= 0) then Exit;

  PaintRect := ShortRect(Bounds(SrcRect.Left + nLeft, SrcRect.Top + nTop, nWidth, nHeight), SrcRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

  Result := PaintRect;
end;

function TDxControl.ReallyRect(AVisibleRect, AVirtualRect:TRect):TRect;
begin
  Result.Left := AVisibleRect.Left - AVirtualRect.Left;
  Result.Top := AVisibleRect.Top - AVirtualRect.Top;
  Result.Right := Result.Left + (AVisibleRect.Right - AVisibleRect.Left);
  Result.Bottom := Result.Top + (AVisibleRect.Bottom - AVisibleRect.Top);
end;
// ------------------------------------------------------------------------------

function TDxControl.CanDraw(DestRect:TRect):Boolean;
var
  AVisibleRect:TRect;
begin
  AVisibleRect := ShortRect(VisibleRect, DestRect);
  Result := (AVisibleRect.Right > AVisibleRect.Left) and (AVisibleRect.Bottom > AVisibleRect.Top);
end;
// ------------------------------------------------------------------------------

function TDxControl.CanDraw:Boolean;
var
  AVisibleRect:TRect;
begin
  AVisibleRect := VisibleRect;
  Result := (AVisibleRect.Right > AVisibleRect.Left) and (AVisibleRect.Bottom > AVisibleRect.Top);
end;

// ------------------------------------------------------------------------------

procedure TDxControl.FillRect(DestRect:TRect; Color:TColor);
begin
  FillRect(DestRect, VirtualRect, VisibleRect, Color);
end;
// ------------------------------------------------------------------------------

procedure TDxControl.FillRectAlpha(DestRect:TRect; Color:TColor; Alpha:Byte = 255);
begin
  FillRectAlpha(DestRect, VirtualRect, VisibleRect, Color, Alpha);
end;
// ------------------------------------------------------------------------------

procedure TDxControl.FrameRect(DestRect:TRect; Color:TColor);
begin
  FrameRect(DestRect, VirtualRect, VisibleRect, Color);
end;

// ------------------------------------------------------------------------------

procedure TDxControl.FillRect(DestRect, vtRect, vbRect:TRect; Color:TColor);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  PaintRect := ShortRect(DestRect, vtRect);
  if vbRect.Left > PaintRect.Left then
    nLeft := vbRect.Left - PaintRect.Left
  else
    nLeft := 0;

  nX := PaintRect.Left + nLeft;

  if vbRect.Top > PaintRect.Top then
    nTop := vbRect.Top - PaintRect.Top
  else
    nTop := 0;

  nY := PaintRect.Top + nTop;

  if (PaintRect.Bottom - PaintRect.Top <= nTop) or (PaintRect.Right - PaintRect.Left <= nLeft) then Exit;

  if vbRect.Right < PaintRect.Right then
    nWidth := vbRect.Right - PaintRect.Left
  else
    nWidth := PaintRect.Right - PaintRect.Left;

  if PaintRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - PaintRect.Top
  else
    nHeight := PaintRect.Bottom - PaintRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;
  PaintRect := Bounds(nX, nY, nWidth, nHeight);
  GameCanvas.FillRect(PaintRect, Color);
end;

procedure TDxControl.FillRectAlpha(DestRect, vtRect, vbRect:TRect; Color:TColor; Alpha:Byte);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Alpha >= 255 then
    FillRect(DestRect, vtRect, vbRect, Color)
  else begin
    PaintRect := ShortRect(DestRect, vtRect);
    if vbRect.Left > PaintRect.Left then
      nLeft := vbRect.Left - PaintRect.Left
    else
      nLeft := 0;

    nX := PaintRect.Left + nLeft;

    if vbRect.Top > PaintRect.Top then
      nTop := vbRect.Top - PaintRect.Top
    else
      nTop := 0;

    nY := PaintRect.Top + nTop;

    if (PaintRect.Bottom - PaintRect.Top <= nTop) or (PaintRect.Right - PaintRect.Left <= nLeft) then Exit;

    if vbRect.Right < PaintRect.Right then
      nWidth := vbRect.Right - PaintRect.Left
    else
      nWidth := PaintRect.Right - PaintRect.Left;

    if PaintRect.Bottom > vbRect.Bottom then
      nHeight := vbRect.Bottom - PaintRect.Top
    else
      nHeight := PaintRect.Bottom - PaintRect.Top;

    nWidth := nWidth - nLeft;
    nHeight := nHeight - nTop;

    if (nHeight <= 0) or (nWidth <= 0) then Exit;
    PaintRect := Bounds(nX, nY, nWidth, nHeight);
    GameCanvas.FillRectAlpha(PaintRect, Color, Alpha);
  end;
end;

procedure TDxControl.FrameRect(DestRect, vtRect, vbRect:TRect; Color:TColor);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  PaintRect := ShortRect(DestRect, vtRect);
  if vbRect.Left > PaintRect.Left then
    nLeft := vbRect.Left - PaintRect.Left
  else
    nLeft := 0;

  nX := PaintRect.Left + nLeft;

  if vbRect.Top > PaintRect.Top then
    nTop := vbRect.Top - PaintRect.Top
  else
    nTop := 0;

  nY := PaintRect.Top + nTop;

  if (PaintRect.Bottom - PaintRect.Top <= nTop) or (PaintRect.Right - PaintRect.Left <= nLeft) then Exit;

  if vbRect.Right < PaintRect.Right then
    nWidth := vbRect.Right - PaintRect.Left
  else
    nWidth := PaintRect.Right - PaintRect.Left;

  if PaintRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - PaintRect.Top
  else
    nHeight := PaintRect.Bottom - PaintRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;

  PaintRect := Bounds(nX, nY, nWidth, nHeight);

  if DestRect.Top >= vbRect.Top then // 上横
    GameCanvas.Line(
      Point(PaintRect.Left, PaintRect.Top),
      Point(PaintRect.Right, PaintRect.Top),
      Color);

  if DestRect.Right <= vbRect.Right then // 右竖
    GameCanvas.Line(
      Point(PaintRect.Right, PaintRect.Top),
      Point(PaintRect.Right, PaintRect.Bottom),
      Color);

  if DestRect.Bottom <= vbRect.Bottom then // 下横
    GameCanvas.Line(
      Point(PaintRect.Left, PaintRect.Bottom),
      Point(PaintRect.Right, PaintRect.Bottom),
      Color);

  if DestRect.Left >= vbRect.Left then // 左竖
    GameCanvas.Line(
      Point(PaintRect.Left, PaintRect.Top - 1),
      Point(PaintRect.Left, PaintRect.Bottom),
      Color);
end;

procedure TDxControl.DrawRect(DestRect:TRect;
  Texture:TTexture; ABlendMode:Integer);
var
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;

  PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

  GameCanvas.Draw(nX, nY, PaintRect, Texture, ABlendMode);
end;

procedure TDxControl.DrawRect(DestRect, vtRect, vbRect:TRect;
  Texture:TTexture; ABlendMode:Integer);
//var
//  PaintRect: TRect;
//  nX, nY, nLeft, nTop, nWidth, nHeight: Integer;
begin
  DrawRect(0, 0, DestRect, vtRect, vbRect, Texture, ABlendMode);
  //  if Texture.Width * Texture.Height <= 4 then Exit;
  //  // GameCanvas.Draw(DestRect.Left, DestRect.Top, Texture.ClientRect, Texture, ATransparent);
  //  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  //  // PaintRect := Texture.ClientRect;
  //
  //  if vbRect.Left > DestRect.Left then
  //    nLeft := vbRect.Left - DestRect.Left
  //  else
  //    nLeft := 0;
  //
  //  nX := DestRect.Left + nLeft;
  //
  //  if vbRect.Top > DestRect.Top then
  //    nTop := vbRect.Top - DestRect.Top
  //  else
  //    nTop := 0;
  //
  //  nY := DestRect.Top + nTop;
  //
  //  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;
  //
  //  if vbRect.Right < DestRect.Right then
  //    nWidth := vbRect.Right - DestRect.Left
  //  else
  //    nWidth := DestRect.Right - DestRect.Left;
  //
  //  if DestRect.Bottom > vbRect.Bottom then
  //    nHeight := vbRect.Bottom - DestRect.Top
  //  else
  //    nHeight := DestRect.Bottom - DestRect.Top;
  //
  //  nWidth := nWidth - nLeft;
  //  nHeight := nHeight - nTop;
  //
  //  if (nHeight <= 0) or (nWidth <= 0) then Exit;
  //
  //  PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);
  //
  //  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;
  //  // Canvas.Draw(nX, nY, Texture.ClientRect, Texture.Texture, ATransparent);
  //  GameCanvas.Draw(nX, nY, PaintRect, Texture, ABlendMode);
end;

procedure TDxControl.DrawRect(nPX, nPY:Integer; DestRect, vtRect, vbRect:TRect;
  Texture:TTexture; ABlendMode:Integer);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  // GameCanvas.Draw(DestRect.Left, DestRect.Top, Texture.ClientRect, Texture, ATransparent);
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  // PaintRect := Texture.ClientRect;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;

  PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;
  // Canvas.Draw(nX, nY, Texture.ClientRect, Texture.Texture, ATransparent);
  GameCanvas.Draw(nX + nPX, nY + nPY, PaintRect, Texture, ABlendMode);
end;

procedure TDxControl.DrawRectColorAlpha(DestRect:TRect;
  Texture:TTexture; Color:TColor; Alpha:Byte; ABlendMode:Integer);
var
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  if Alpha = 255 then
    DrawRectColor(DestRect, Texture, Color)
  else begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    if vbRect.Left > DestRect.Left then
      nLeft := vbRect.Left - DestRect.Left
    else
      nLeft := 0;

    nX := DestRect.Left + nLeft;

    if vbRect.Top > DestRect.Top then
      nTop := vbRect.Top - DestRect.Top
    else
      nTop := 0;

    nY := DestRect.Top + nTop;

    if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

    if vbRect.Right < DestRect.Right then
      nWidth := vbRect.Right - DestRect.Left
    else
      nWidth := DestRect.Right - DestRect.Left;

    if DestRect.Bottom > vbRect.Bottom then
      nHeight := vbRect.Bottom - DestRect.Top
    else
      nHeight := DestRect.Bottom - DestRect.Top;

    nWidth := nWidth - nLeft;
    nHeight := nHeight - nTop;

    if (nHeight <= 0) or (nWidth <= 0) then Exit;

    PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);
    // PaintRect := ShortRect(Bounds(nLeft, nTop, nWidth, nHeight), Texture.ClientRect);

    if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

    GameCanvas.DrawColorAlpha(nX, nY, PaintRect, Texture, Alpha, Color, ABlendMode);
  end;
end;

procedure TDxControl.DrawRectColorAlpha(DestRect, vtRect, vbRect:TRect;
  Texture:TTexture; Color:TColor; Alpha:Byte; ABlendMode:Integer);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  if Alpha = 255 then
    DrawRectColor(DestRect, Texture.ClientRect, vtRect, vbRect, Texture, Color)
  else begin
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

    if vbRect.Left > DestRect.Left then
      nLeft := vbRect.Left - DestRect.Left
    else
      nLeft := 0;

    nX := DestRect.Left + nLeft;

    if vbRect.Top > DestRect.Top then
      nTop := vbRect.Top - DestRect.Top
    else
      nTop := 0;

    nY := DestRect.Top + nTop;

    if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

    if vbRect.Right < DestRect.Right then
      nWidth := vbRect.Right - DestRect.Left
    else
      nWidth := DestRect.Right - DestRect.Left;

    if DestRect.Bottom > vbRect.Bottom then
      nHeight := vbRect.Bottom - DestRect.Top
    else
      nHeight := DestRect.Bottom - DestRect.Top;

    nWidth := nWidth - nLeft;
    nHeight := nHeight - nTop;

    if (nHeight <= 0) or (nWidth <= 0) then Exit;

    PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);
    // PaintRect := ShortRect(Bounds(nLeft, nTop, nWidth, nHeight), Texture.ClientRect);

    if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

    GameCanvas.DrawColorAlpha(nX, nY, PaintRect, Texture, Color, Alpha, ABlendMode);
  end;
end;

procedure TDxControl.DrawRectColor(DestRect:TRect;
  Texture:TTexture; Color:TColor; ABlendMode:Integer);
var
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;

  PaintRect := ShortRect(Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight), Texture.ClientRect);
  // PaintRect := ShortRect(Bounds(nLeft, nTop, nWidth, nHeight), Texture.ClientRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

  GameCanvas.DrawColor(nX, nY, PaintRect, Texture, Color, ABlendMode);
end;

procedure TDxControl.DrawRectColor(DestRect, SrcRect, vtRect, vbRect:TRect;
  Texture:TTexture; Color:TColor; ABlendMode:Integer);
var
  PaintRect:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then Exit;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

  if vbRect.Left > DestRect.Left then
    nLeft := vbRect.Left - DestRect.Left
  else
    nLeft := 0;

  nX := DestRect.Left + nLeft;

  if vbRect.Top > DestRect.Top then
    nTop := vbRect.Top - DestRect.Top
  else
    nTop := 0;

  nY := DestRect.Top + nTop;

  if (DestRect.Bottom - DestRect.Top <= nTop) or (DestRect.Right - DestRect.Left <= nLeft) then Exit;

  if vbRect.Right < DestRect.Right then
    nWidth := vbRect.Right - DestRect.Left
  else
    nWidth := DestRect.Right - DestRect.Left;

  if DestRect.Bottom > vbRect.Bottom then
    nHeight := vbRect.Bottom - DestRect.Top
  else
    nHeight := DestRect.Bottom - DestRect.Top;

  nWidth := nWidth - nLeft;
  nHeight := nHeight - nTop;

  if (nHeight <= 0) or (nWidth <= 0) then Exit;
  SrcRect := ShortRect(SrcRect, Texture.ClientRect);
  PaintRect := ShortRect(Bounds(SrcRect.Left + nLeft, SrcRect.Top + nTop, nWidth, nHeight), SrcRect);
  // PaintRect := ShortRect(Bounds(nLeft, nTop, nWidth, nHeight), Texture.ClientRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then Exit;

  GameCanvas.DrawColor(nX, nY, PaintRect, Texture, Color, ABlendMode);
end;

procedure TDxControl.DrawCaption(HGEFont:THGEFont;
  const Color:TColor; const ACaption:string;
  const ADestRect:TRect;
  X:Integer; Y:Integer);
var
  AVisibleRect, AVirtualRect:TRect;
begin
  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-13】 }
  (*
  AFont := TDxFont.Create;
  AFont.Color := Color;

  AVisibleRect := VisibleRect;
  AVirtualRect := VirtualRect;
  DrawCaption(HGEFont, AFont, ACaption, ADestRect, AVisibleRect, AVirtualRect, X, Y);
  AFont.Free;
  *)

  FDrawCaptionFont.Color := Color;

  AVisibleRect := VisibleRect;
  AVirtualRect := VirtualRect;
  DrawCaption(HGEFont, FDrawCaptionFont, ACaption, ADestRect, AVisibleRect, AVirtualRect, X, Y);
end;

procedure TDxControl.DrawCaption(HGEFont:THGEFont; AFont:TDxFont; AAlignment:TAlignment; ATextImages:TImageInfos;
  const ADestRect, AVisibleRect, AVirtualRect:TRect;
  X:Integer; Y:Integer; ExpandLineHeight:Integer);
var
  nLeft, nTop, nX, nY:Integer;
  PaintRect:TRect;
  DestRect:TRect;
begin
  if Length(ATextImages) > 0 then begin
    DestRect := ADestRect;
    PaintRect := Rect(0, 0, (DestRect.Right - DestRect.Left), (DestRect.Bottom - DestRect.Top));
    case AAlignment of
      taLeftJustify:begin
          nLeft := X;
          nTop := ((AVirtualRect.Bottom - AVirtualRect.Top) - (DestRect.Bottom - DestRect.Top)) div 2 + Y;
          DestRect := MoveRect(DestRect, Point(nLeft, nTop));
        end;
      taRightJustify:begin
          nLeft := (AVirtualRect.Right - AVirtualRect.Left) - (DestRect.Right - DestRect.Left) + X;
          nTop := ((AVirtualRect.Bottom - AVirtualRect.Top) - (DestRect.Bottom - DestRect.Top)) div 2 + Y;
          DestRect := MoveRect(DestRect, Point(nLeft, nTop));
        end;
      taCenter:begin
          nLeft := ((AVirtualRect.Right - AVirtualRect.Left) - (DestRect.Right - DestRect.Left)) div 2 + X;
          nTop := ((AVirtualRect.Bottom - AVirtualRect.Top) - (DestRect.Bottom - DestRect.Top)) div 2 + Y;
          DestRect := MoveRect(DestRect, Point(nLeft, nTop));
        end;
    end;

    PaintRect := ReallyPaintRect(DestRect, PaintRect, AVirtualRect, AVisibleRect, nX, nY);
    // PaintRect := GetPaintRect(DestRect, PaintRect, AVirtualRect, AVisibleRect, nX, nY);
    if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
      if AFont.Bold then begin
        HGEFont.TextRect(nX - 1, nY, PaintRect, ATextImages, AFont.BColor, 2, 255, ExpandLineHeight);
        HGEFont.TextRect(nX + 1, nY, PaintRect, ATextImages, AFont.BColor, 2, 255, ExpandLineHeight);
        HGEFont.TextRect(nX, nY - 1, PaintRect, ATextImages, AFont.BColor, 2, 255, ExpandLineHeight);
        HGEFont.TextRect(nX, nY + 1, PaintRect, ATextImages, AFont.BColor, 2, 255, ExpandLineHeight);
        HGEFont.TextRect(nX, nY, PaintRect, ATextImages, AFont.Color, 2, 255, ExpandLineHeight);
      end
      else begin
        HGEFont.TextRect(nX, nY, PaintRect, ATextImages, AFont.Color, 2, 255, ExpandLineHeight);
      end;
    end;
  end;
end;

procedure TDxControl.DrawCaption(HGEFont:THGEFont; AFont:TDxFont; ATextImages:TImageInfos;
  const ADestRect, AVisibleRect, AVirtualRect:TRect;
  X:Integer; Y:Integer; ExpandLineHeight:Integer);
begin
  DrawCaption(HGEFont, AFont, Alignment, ATextImages, ADestRect, AVisibleRect, AVirtualRect, X, Y, ExpandLineHeight);
end;

procedure TDxControl.DrawCaption(HGEFont:THGEFont;
  AFont:TDxFont; const ACaption:string;
  const ADestRect, AVisibleRect, AVirtualRect:TRect;
  X:Integer; Y:Integer);
var
  TextImages:TImageInfos;
  PaintRect:TRect;
begin
  if ACaption <> '' then begin
    if ACaption = '-' then begin
      PaintRect := AVirtualRect;
      PaintRect.Top := AVirtualRect.Top + (AVirtualRect.Bottom - AVirtualRect.Top) div 2;
      PaintRect.Bottom := PaintRect.Top + 1;
      FillRect(PaintRect, AVirtualRect, AVisibleRect, AFont.Color);
    end
    else begin
      TextImages := HGEFont.GetImageInfos(ACaption);
      DrawCaption(HGEFont, AFont, TextImages, ADestRect, AVisibleRect, AVirtualRect, X, Y);
    end;
  end;
end;

procedure TDxControl.Repaint;
begin
  RootCtrl.Repaint;
end;

// ------------------------------------------------------------------------------

procedure TDxControl.Paint;
var
  I:Integer;
  vtRect:TRect;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  if FDesigning then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, clRed);
  end;
  for I := ControlCount - 1 downto 0 do begin
    if Control[I].Visible then begin
      Control[I].Paint;
    end;
  end;
end;
// ---------------------------------------------------------------------------

procedure TDxControl.DoClick(X, Y:Integer);
begin
  if Assigned(OnClick) then
    OnClick(Self, X, Y);
  // DebugOut('Name:'+Name);
end;

// ---------------------------------------------------------------------------

procedure TDxControl.DoDblClick(X, Y:Integer);
begin
  if Assigned(OnDblClick) then
    OnDblClick(Self, X, Y);
end;

procedure TDxControl.DoMouseDown();
begin
  Repaint;
end;

procedure TDxControl.DoMouseMove();
begin
  Repaint;
end;

procedure TDxControl.DoMouseUp();
begin
  Repaint;
end;

procedure TDxControl.DoMouseEnter;
begin
  if Assigned(FOnMouseEnter) then
    FOnMouseEnter(Self);
end;

procedure TDxControl.DoMouseLeave;
begin
  if Assigned(FOnMouseLeave) then
    FOnMouseLeave(Self);
end;

procedure TDxControl.DoFocused();
begin

end;

procedure TDxControl.DoUnFocused();
begin

end;
// ---------------------------------------------------------------------------

procedure TDxControl.DblClick(X, Y:Integer);
begin
  DoDblClick(X, Y);
end;

procedure TDxControl.KeyDown(var Key:Word; Shift:TShiftState);
begin
  if Assigned(OnKeyDown) then
    OnKeyDown(Self, Key, Shift);
end;

procedure TDxControl.KeyPress(var Key:Char);
begin
  if Assigned(OnKeyPress) then
    OnKeyPress(Self, Key);
end;

procedure TDxControl.KeyUp(var Key:Word; Shift:TShiftState);
begin
  if Assigned(OnKeyUp) then
    OnKeyUp(Self, Key, Shift);
end;

function TDxControl.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  Texture:TTexture;
  FaceIndex:Integer;
  vRect:TRect;
begin
  Result := False;
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if FTransparent and (ImageIndex.Image <> nil) then begin
      FaceIndex := -1;

      if FImageIndex.Up >= 0 then
        FaceIndex := FImageIndex.Up
      else if FImageIndex.Hot >= 0 then
        FaceIndex := FImageIndex.Hot
      else if FImageIndex.Down >= 0 then
        FaceIndex := FImageIndex.Down
      else if FImageIndex.Checked >= 0 then
        FaceIndex := FImageIndex.Checked;

      if FaceIndex >= 0 then begin
        Texture := ImageIndex.Image.Images[FaceIndex];
        if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
          vRect := VirtualRect;
          boInrange := CheckTextureAlpha(Texture, X - vRect.Left, Y - vRect.Top); //
        end
        else
          boInrange := True;
      end;
    end;

    vRect := VirtualRect;
    DoOnInRealArea(X - vRect.Left, Y - vRect.Top, boInrange);

    Result := boInrange;
  end;
end;

procedure TDxControl.SetMousePoint(X, Y:Integer);
begin
  SpotX := X;
  SpotY := Y;
  FMouseDownX := X;
  FMouseDownY := X;
  if (Owner <> nil) and (Owner is TDxControl) then
    TDxControl(Owner).SetMousePoint(X, Y);
end;

procedure TDxControl.MouseEnter;
begin

end;

procedure TDxControl.MouseLeave;
begin

end;

function TDxControl.FindActiveControl(X, Y:Integer):TDxControl;
var
  I:Integer;
  D:TDxControl;
begin
  if Assigned(FOnFindActiveControl) then
    Result := FOnFindActiveControl(Self, X, Y)
  else begin
    Result := nil;
    if Visible and EnableMouse and (Enabled or ((not Enabled) and Designing)) and CanDraw and PointInRect(Point(X, Y), VisibleRect) then begin
      if InRange(X, Y) then Result := Self;
      for I := ControlCount - 1 downto 0 do begin
        D := Control[I].FindActiveControl(X, Y);
        if D <> nil then
          Result := D;
      end;
    end;
  end;
end;

procedure TDxControl.MouseWheelDown(Shift:TShiftState; MousePos:TPoint);
begin

end;

procedure TDxControl.MouseWheelUp(Shift:TShiftState; MousePos:TPoint);
begin

end;

procedure TDxControl.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  // FCanMouse := ((Button = mbLeft) and (mbLeft in FMouseEvents)) or
   // ((Button = mbRight) and (mbRight in FMouseEvents));

  if {FCanMouse and }  Assigned(OnMouseDown) then
    OnMouseDown(Self, Button, Shift, X, Y);
end;

function TDxControl.CanMove:Boolean;
begin
  Result := True;
end;

procedure TDxControl.Move(X, Y:Integer);
var
  al, at:Integer;
  Range:TRect;
begin
  if CanMove and (Floating or Designing) and
    ((SpotX <> X) or (SpotY <> Y)) and
    (X >= 0) and (Y >= 0) and
    (X <= RootCtrl.Width) and (Y <= RootCtrl.Height) then begin

    al := Left + (X - SpotX);
    at := Top + (Y - SpotY);
    if Owner <> nil then begin
      if Owner = RootCtrl then begin
        Range := RootCtrl.ClientRect;
      end
      else begin
        Range := TDxControl(Owner).MoveRange;
      end;
      if al + Width < Range.Left then al := Left;
      if al > Range.Right then al := Left;
      if at + Height < Range.Top then at := Top;
      if at > Range.Bottom then at := Top;

      {if al + Width < Range.Left then al := Range.Left - Width;
      if al > Range.Right then al := Range.Right;
      if at + Height < Range.Top then at := Range.Top - Height;
      if at + Height > Range.Bottom then at := Range.Bottom - Height;}
    end;
    Left := al;
    Top := at;
    SpotX := X;
    SpotY := Y;

    DoMove;

    Exit;
  end;
  if (not Designing) and CanMove and OwnerMove and
    (Owner <> nil) and (Owner is TDxControl) then
    TDxControl(Owner).Move(X, Y);
end;

procedure TDxControl.DoMove;
begin
  if Assigned(FOnMove) then
    FOnMove(Self);
end;

procedure TDxControl.MouseMove(Shift:TShiftState; X, Y:Integer);
begin
  // FCanMouse := ((ssLeft in Shift) and (mbLeft in FMouseEvents)) or
    // ((ssRight in Shift) and (mbRight in FMouseEvents));

  if {FCanMouse and}  Assigned(OnMouseMove) then
    OnMouseMove(Self, Shift, X, Y);
end;

procedure TDxControl.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  FCanMouse := ((Button = mbLeft) and (mbLeft in FMouseEvents)) or
    ((Button = mbRight) and (mbRight in FMouseEvents));

  if FCanMouse and Assigned(OnMouseUp) then
    OnMouseUp(Self, Button, Shift, X, Y);

  if FCanMouse then
    DoClick(X, Y);
end;

procedure GetAllSubComponents(Ctrl:TDxControl; List:TList);
var
  I:Integer;
begin
  for I := 0 to Ctrl.ComponentCount - 1 do begin
    List.Add(Ctrl.Components[I]);

    if Ctrl.Components[I].ComponentCount > 0 then
      GetAllSubComponents(Ctrl.Components[I], List);
  end;
end;

// ---------------------------------------------------------------------------
initialization
  ControlEngineList := TList.Create;
finalization
  ControlEngineList.Free;
end.
