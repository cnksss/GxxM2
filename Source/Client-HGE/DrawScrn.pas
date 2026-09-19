unit DrawScrn;

interface

uses
  Windows,
  Messages,
  SysUtils,
  StrUtils,
  Classes,
  Graphics,
  Controls,
  Forms,
  DIB,
  Dialogs,
  HGE,
  IntroScn,
  Actor,
  SDK,
  ClFunc,
  HUtil32,
  Grobal2,
  HGEFontEx,
  DxCanvas,
  HGECanvas,
  DxComponents,
  GameImages,
  dxMemo;

const
  MAXSYSLINE = 8;

  BOTTOMBOARD = 1;
  VIEWCHATLINE = 9;

  HEALTHBAR_BLACK = 0;
  HEALTHBAR_RED = 1;
  AREASTATEICONBASE = 150;

  {
  var
    HintWindow_BorderSpace: Integer = 8;
  }

type
  // 滚动消息方向修改 piaoyun 2013-08-02
  TMoveMessageOrientation = (mbHorizontal {水平}, mbVertical {垂直}, mbNone {不滚动});

  // 滚动消息结构体定义
  TMoveMsg = record
    Text:TStringLineEx;
    //FColor, BColor: Byte;
    Alpha:Byte;
    Count:Integer;
    Y:Integer;
    Width:Integer;
    Height:Integer;
    Orientation:TMoveMessageOrientation;

    ShowFrame:Boolean;
    FrameColor:Byte;
    btFontSize:Byte;
    boFontBold:Boolean;

    nMarqueeTime:Integer;
  end;
  pTMoveMsg = ^TMoveMsg;

  TDelayMsg = record
    RecogId:Int64;
    Msg:string;
    Time:LongWord;
    FColor, BColor:TColor;
    X:Integer;
    Tokens:TStringLineEx;
  end;
  pTDelayMsg = ^TDelayMsg;

  // 提示信息结构体
  THintMsg = record
    Msg:string;
    FColor:TColor;
  end;
  pTHintMsg = ^THintMsg;

  THintWindowInfo = record
    Visible:Boolean;
    HintList:TList;
  end;
  pTHintWindowInfo = ^THintWindowInfo;

  // 系统消息结构体定义
  TSysMsg = record
    Time:LongWord;
    FColor, BColor:TColor;
  end;
  pTSysMsg = ^TSysMsg;

  THintLines = class;
  THintMessage = class(TObject)
  private
    FWidth:Integer;
    FHeight:Integer;
    FOwner:THintLines;
  public
    constructor Create(AOwner:THintLines); virtual;
    procedure Initialize; virtual;
    procedure Paint(OwnerRect, DestRect, HintWinRect:TRect); virtual;

    property Owner:THintLines read FOwner;
    property Width:Integer read FWidth write FWidth;
    property Height:Integer read FHeight write FHeight;
  end;

  THintImage = class(THintMessage)
  private
    FGameImages:TGameImages;
    FImageIndex:Integer;

    FOffsetX:Integer;
    FOffsetY:Integer;
    FShowBG:Integer;
    FFixedHeight:Integer;

    FIsVerticalAlignTop:Boolean;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property ImageIndex:Integer read FImageIndex write FImageIndex;

    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
  end;

  TWinHintImage = class(THintMessage)
  private
    FGameImages:TGameImages;
    FImageIndex:Integer;

    FOffsetX:Integer;
    FOffsetY:Integer;
  protected //不确定是否会影响到API接口，移动到protected 消除警告
    FShowBG:Integer;
    FFixedHeight:Integer;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property ImageIndex:Integer read FImageIndex write FImageIndex;

    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
  end;

  TLineBGHintImage = class(THintMessage)
  private
    FTextureWidth:Integer;
    FGameImages:TGameImages;
    FImageIndex:Integer;

    FOffsetX:Integer;
    FOffsetY:Integer;
  protected //不确定是否会影响到API接口，移动到protected 消除警告
    FShowBG:Integer;
    FFixedHeight:Integer;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property ImageIndex:Integer read FImageIndex write FImageIndex;

    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
  end;

  TFiexdHeightLine = class(THintMessage)
  end;

  THintPlayImage = class(THintMessage)
  private
    FGameImages:TGameImages;
    FImageIndex:Integer;
    FPlayCount:Integer;
    FPlayTime:Integer;
    FPlayTick:Cardinal;
    FPlayImageIndex:Integer;

    FOffsetX:Integer;
    FOffsetY:Integer;
    FBlendDraw:Boolean;

    FFixedWidth:Integer;
    FFixedHeight:Integer;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    property GameImages:TGameImages read FGameImages write FGameImages;
    property ImageIndex:Integer read FImageIndex write FImageIndex;

    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
    property PlayCount:Integer read FPlayCount write FPlayCount;
    property PlayTime:Integer read FPlayTime write FPlayTime;
    property BlendDraw:Boolean read FBlendDraw write FBlendDraw;
  end;

  THintPlayImageEx = class(THintPlayImage)
  private
    FIncSpacing:Integer;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
  end;

  THintItemProgress = class(THintMessage)
  private
    FTextColor:TColor;
    FText:string;
    FGameImages:TGameImages;
    FProgressIndex:Integer;
    FPlayCount:Integer;

    FMaxValue:Integer;
    FCurValue:Integer;

    FPlayTick:Cardinal;
    FPlayImageIndex:Integer;

    FOffsetX:Integer;
    FOffsetY:Integer;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    property GameImages:TGameImages read FGameImages write FGameImages;

    property TextColor:TColor read FTextColor write FTextColor;
    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
    property PlayCount:Integer read FPlayCount write FPlayCount;
    property ProgressIndex:Integer read FProgressIndex write FProgressIndex;
    property MaxValue:Integer read FMaxValue write FMaxValue;
    property CurValue:Integer read FCurValue write FCurValue;
  end;

  THintText = class(THintMessage)
  private
    FCaption:string;
    FColor:TColor;
    FSize:Integer;
    FStyle:TFontStyles;
    FIsStroke:Boolean;
    FFontName:string;

    procedure SetSize(Value:Integer);
    procedure SetStyle(Value:TFontStyles);
    procedure SetIsStroke(Value:Boolean);
  protected
    procedure SetCaption(Value:string);
  public
    constructor Create(AOwner:THintLines); override;
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;

    property Caption:string read FCaption write FCaption;
    property Color:TColor read FColor write FColor;
    property Size:Integer read FSize write SetSize;
    property Style:TFontStyles read FStyle write SetStyle;
    property IsStroke:Boolean read FIsStroke write SetIsStroke;
  end;

  THintFixedWidthText = class(THintText)
  private
    FMinWidth:Integer;
  public
    procedure Initialize; override;
  end;

  TCountdownText = class(THintMessage)
  private
    FCountdownValue:Integer;
    FStartTick:LongWord;
    FTextColor:TColor;
    FOffsetX:Integer;
    FOffsetY:Integer;

    function GetShowText:string;
  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
  end;

  THintImageNumber = class(THintMessage)
  private
    FGameImages:TGameImages;
    FNumberIndex:Integer;
    FNumberSpace:Integer;
    FNumberValue:string;

    FOffsetX:Integer;
    FOffsetY:Integer;
  protected

  public
    procedure Initialize; override;
    procedure Paint(OwnerRect, PaintRect, HintWinRect:TRect); override;
    {
    property GameImages: TGameImages read FGameImages write FGameImages;
    property NumberIndex: Integer read FNumberIndex write FNumberIndex;
    property NumberSpace: Integer read FNumberSpace write FNumberSpace;
    property NumberValue: string read FNumberValue write FNumberValue;

    property OffsetX: Integer read FOffsetX write FOffsetX;
    property OffsetY: Integer read FOffsetY write FOffsetY;
    }
  end;

  THintLines = class
  private
    FList:TList;
    FOffsetX, FOffsetY:Integer;
    FWidth:Integer;
    FHeight:Integer;
    FMinWidth:Integer; // 扩展最底宽度(用于背景图) 2019-12-16 09:35:50
    FItemHeight:Integer;
    FAlignment:TAlignment;

    // ask 模式的图标加文字不好处理，只能这样先处理着，如果还有这样的，就用Group: Byte来整
    // 这个主要目地是为了处理对齐，单个偏移不行，会叠加，只有在对齐时整体偏移才可以
    FIsItemIconText:Boolean;

    procedure Put(Index:Integer; const S:string);
    function Get(Index:Integer):string;
    function GetCount:Integer;
    function GetObject(Index:Integer):THintMessage;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure Delete(Index:Integer);
    procedure Add(
      S:string;
      Color:TColor = clWhite;
      FontSize:Integer = 9;
      FontStyles:TFontStyles = [];
      IsStroke:Boolean = False;
      FontName:string = '');

    procedure Insert(Index:Integer;
      S:string;
      Color:TColor = clWhite;
      FontSize:Integer = 9;
      FontStyles:TFontStyles = [];
      IsStroke:Boolean = False;
      FontName:string = '');

    procedure AddFixedHeightLine(Height:Integer);

    procedure Paint(OwnerRect, PaintRect:TRect);
    procedure PaintWithoutWinHintImage(OwnerRect, PaintRect:TRect; var HaveWinHintImage:Boolean);
    procedure PaintWinHintImage(OwnerRect, PaintRect:TRect);
    property OffsetX:Integer read FOffsetX write FOffsetX;
    property OffsetY:Integer read FOffsetY write FOffsetY;
    property Count:Integer read GetCount;
    property Width:Integer read FWidth;
    property Height:Integer read FHeight;
    property MinWidth:Integer read FMinWidth;
    property ItemHeight:Integer read FItemHeight;
    property Alignment:TAlignment read FAlignment write FAlignment;
    property IsItemIconText:Boolean read FIsItemIconText write FIsItemIconText;
    property Objects[Index:Integer]:THintMessage read GetObject;
    property Strings[Index:Integer]:string read Get write Put;

    procedure GetSize;
  end;

  THintWindow = class(TList)
  private
    FVisible:Boolean;
    HintX, HintY, HintWidth, HintHeight:Integer;
    HintUp:Boolean;

    FShowBackground:Boolean;
    HintRect:TRect;
    procedure DrawBackground();
    procedure SetHintX(Value:Integer);
    procedure SetHintY(Value:Integer);
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear; override;

    procedure ShowColor(X, Y:Integer; Msg:string; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
    procedure Show(X, Y:Integer; DrawUp:Boolean = False; DrawLeft:Boolean = False; ShowBackground:Boolean = True); overload;
    procedure Show(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean = False; DrawLeft:Boolean = False; ShowBackground:Boolean = True); overload;
    procedure Draw();

    property Visible:Boolean read FVisible write FVisible;
    property Width:Integer read HintWidth write HintWidth;
    property Height:Integer read HintHeight write HintHeight;
    property X:Integer read HintX write SetHintX;
    property Y:Integer read HintY write SetHintY;

    procedure SetHintXExt(Value:Integer);
    procedure SetHintYExt(Value:Integer);
  end;

  THintWindows = class(TObject)
  private
    FWindows:TList;
    CriticalSection:TRTLCriticalSection;
    function GetCount:Integer;
    function GetItems(Index:Integer):THintWindow;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Initialize;
    procedure Finalize;
    procedure Draw();
    procedure Clear;
    procedure Add(HintWindow:THintWindow);
    procedure UpDate;
    procedure Show(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean = False; DrawLeft:Boolean = False; ShowBackground:Boolean = True);
    procedure ShowColor(X, Y:Integer; Msg:string; DrawUp:Boolean = False; DrawLeft:Boolean = False; ShowBackground:Boolean = True);

    property Count:Integer read GetCount;
    property Items[Index:Integer]:THintWindow read GetItems;
  end;

  TDrawScreenCenterMsg = class(TObject)
  private
    m_TextList:TGStringList;
    m_FColor:Byte;
    m_BColor:Byte;
    m_dwShowTime:LongWord;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte; nTime:Integer);
    procedure Draw();
    procedure Clear(Lock:Boolean = True);
  end;

  PDrawScreenNewMsgCacheText = ^TDrawScreenNewMsgCacheText;
  TDrawScreenNewMsgCacheText = record
    sMsg:string;
    FColor, BColor:Byte;
    FontSize:Byte;
    nX, nY, nCount:Integer;
    nAddTick:LongWord;
    nTime:Integer;
    nDrawType:Integer;
  end;

  // 新增加一个中心换行消息，位置由Y决定 piaoyun 2013-08-02
  TDrawScreenCenterNewlineMsg = class(TObject)
  private
    FCacheList:TList;

    m_ShowLines:TGStringList; // 信息内容链
    m_FColor:Byte; // 前景色
    m_BColor:Byte; // 背景色
    m_btFontSize:Byte; // 字体大小
    m_dwStartStandTick:LongWord; // 开始停留时间
    m_nX, m_nY:Integer; // Y坐标
    m_nCurY:Integer; // 当前Y坐标
    m_nState:Byte; // 当前状态
    m_nStandTime:Integer; // 停留时间
    m_nDrawType:Byte; // 绘制方式 0带透明框 1淡入淡出

    m_boShowOver:Boolean;

    procedure ClearTimeCache;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(const sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
    procedure Draw(boDrawBack:Boolean);
    procedure DrawEx();
    procedure ClearShowLines(Lock:Boolean = True);
    procedure ClearCacheLines;
  end;

  TDrawScreenNewMoveMsg = class(TObject)
  private
    FCacheList:TList;

    m_ShowLines:TGStringList; // 信息内容链
    m_FontSize:Byte;
    m_FColor, m_BColor:TColor; // 前景色/背景色

    m_TotalCount, m_CurrentCount:Integer; // 次数

    m_nX:Integer;
    m_Y0, m_Y1, m_Y2:Integer; // 三行文字的定位位置

    m_OffsetY1, m_OffsetY2:Integer; // 文字当前显示位置

    m_Alpha1, m_Alpha2:Byte; // 文字的透明度

    m_CurLineIndex:Integer;
    m_IsStayShow:Boolean; // 是否为停留显示 （不是滚动显示）

    m_StartStayTime:DWORD; // 开始停留时间
    m_LastTime:DWORD;

    m_LineHeight:Integer; // 每一行的高度
    m_StepMove:Integer; // 每一步移动几个点
    m_StepAlphaChange:Integer; // 每移动一步的透明度改变值

    m_HGEFont:THGEFont;

    m_ShowOver:Boolean;
  public
    constructor Create;
    destructor Destroy; override;
    procedure ClearShowLines;
    procedure ClearCacheLines;
    //SuperMOVEMSG 信息类型代码(0-1)0全局发送1发送给个人 字体颜色(0-255) 背景颜色(0-255) 字体大小 Y坐标 滚动次数 信息内容   | 换行符号
    procedure Add(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
    procedure Draw();
  end;

  TDrawDelayMsg = class(TObject)
  private
    m_dwDrawFrameCount:longword;
    m_MsgList:TGList;
    m_MoveDraw:Boolean; // SendCenterMsg只显示几行 chongchong 2014-07-05
    m_MoveOffset:Integer; // SendCenterMsg只显示几行 chongchong 2014-07-05
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(RecogId:Int64; sMsg:string; nTime:Integer; FColor, BColor:TColor; nX:Integer);
    procedure Draw();
    procedure Clear;
    procedure Delete(RecogId:Int64);
  end;

  TDrawScreenMoveMsg = class(TObject)
  private
    m_MsgList:TGList;
    m_nCurrMoveMsg:pTMoveMsg;
    m_dwMoveTick:LongWord;
    FOffSetX, FOffSetY, FTextSize, FMoveSize:Integer;
    DestRect:TRect;
    SrcRect:TRect;
    function GetTop:Integer;
    function GetCount:Integer;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer;
      boShowFrame:Boolean = True; btFrameColor:Byte = 190; btFontSize:Byte = 11;
      boFontBold:Boolean = True; nMarqueeTime:Integer = 60);
    procedure Draw();
    property Top:Integer read GetTop;
    procedure Initialize;
    procedure Finalize;
    procedure Update;
    function MoveOver:Boolean;
    property Count:Integer read GetCount;
  end;

  TScreenMoveMsgList = class(TObject)
  private
    m_MoveObjList:TGList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer;
      boShowFrame:Boolean = True; btFrameColor:Byte = 190; btFontSize:Byte = 11;
      boFontBold:Boolean = True; nMarqueeTime:Integer = 60);
    procedure Draw();
    procedure Initialize;
    procedure Finalize;
    procedure Update;
    procedure Clear;
  end;

  TScreenNewMoveMsgList = class(TObject)
  private
    FItemList:TGList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
    procedure Draw();
    procedure Update;
    procedure Clear;
  end;

  PMoveHintMsgRecord = ^TMoveHintMsgRecord;
  TMoveHintMsgRecord = record
    Msg:string;
    FColor:Byte; // 前景色
    BColor:Byte; // 背景色
    nX:Integer;
    nY:Integer;
    nYOffset:Integer;
    LastUpdateTick:LongWord;
  end;

  TMoveHintMsgList = class(TObject)
  private
    FItemList:TGList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte; nX, nY:Integer);
    procedure Draw();
    procedure Update;
    procedure Clear;
  end;

  TScreenNewLineMsgList = class(TObject)
  private
    FItemList:TGList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
    procedure Draw(boDrawBack:Boolean);
    procedure Update;
    procedure Clear;
  end;

  TDrawSysMsg = class(TObject)
    m_nX, m_nY:Integer;
    m_FColor, m_BColor:TColor;
  private
    m_MsgList:TGStringList;
  public
    m_boDownToUP:Boolean;
    m_boDrawBottom:Boolean;
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte);
    procedure Draw();
    procedure Clear;
  end;

  TDrawSysMsgEx = class(TObject)
    m_nX, m_nY:Integer;
    m_FColor, m_BColor:TColor;
  private
    m_boWantDelete:Boolean;
    m_boWantDeleteTick:LongWord;
    m_nOffsetY:Integer;
    m_dwOffsetTick:LongWord;
    m_boDownToUP:Boolean;

    m_MsgList:TGStringList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte);
    procedure Draw();
    procedure Clear;
  end;

  TDrawMoveHintMsg = class(TObject)
    m_nX, m_nY:Integer;
    m_FColor, m_BColor:TColor;
  private
    m_MsgList:TGStringList;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(sMsg:string; FColor, BColor:Byte);
    procedure Draw();
    procedure Clear;
  end;

  TDrawScreen = class(TObject)
  private
    m_dwFrameTime:longword;
    m_dwFrameCount:longword;

    m_SysMsgList:TGStringList;
    m_SysMsgListEx:TGStringList;
    m_boInitialize:Boolean;

    m_boShowLoginSceneShowRandomCodeDlg:Boolean;

    FScreenMoveMsgList:TScreenMoveMsgList; // 屏幕滚动消息链
    FScreenNewMoveMsgList:TScreenNewMoveMsgList;
    FScreenNewLineMsgList:TScreenNewLineMsgList;
    FMoveHintMsgList:TMoveHintMsgList;
  public
    CurrentScene:TScene;

    HintList:TStringList;
    HintX, HintY, HintWidth, HintHeight:Integer;
    HintUp:Boolean;
    HintColor:TColor;
    DrawDelayMsg:TDrawDelayMsg;
    DrawScreenCenterMsg:TDrawScreenCenterMsg;

    constructor Create;
    destructor Destroy; override;
    procedure KeyPress(var Key:Char);
    procedure KeyDown(var Key:Word; Shift:TShiftState);
    procedure MouseMove(Shift:TShiftState; X, Y:Integer);
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);

    procedure Initialize;
    procedure Finalize;
    procedure Update;
    procedure ChangeScene(SceneType:TSceneType);

    procedure AddSysMsg(Msg:string; FColor, Bcolor:Byte; X, Y:Integer; boDownToUP:Boolean = False; boDrawBottom:Boolean = False);
    //procedure AddSysMsgEx(Msg: string; FColor, Bcolor: Byte; X, Y: Integer; boDownToUP: Boolean = False);

    procedure AddChatBoardString(Str:string; FColor, Bcolor:Integer);
    procedure AddTopChatBoardString(Str:string; FColor, Bcolor, TimeOut:Integer);
    procedure ClearChatBoard;
    procedure AddMoveMsg(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer; boShowFrame:Boolean = True; btFrameColor:Byte = 190; btFontSize:Byte = 11; boFontBold:Boolean = True; nMarqueeTime:Integer = 60);
    procedure AddNewMoveMsg(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
    procedure AddNewLineMsg(sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
    procedure AddMoveHintMsg(sMsg:string; FColor, BColor:Byte; nX, nY:Integer);

    procedure ShowHint(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
    procedure ClearHint;

    procedure DrawScreen();
    procedure DrawMsg_TopLevel(); // 上层绘制  2018-09-27 18:40:19
    procedure DrawSysMsg_BottomLevel(); // 下层绘制 获得经验 2018-09-27 18:40:21
    procedure DrawHint();
    procedure DrawMove();
    procedure DrawMoveBefor();

    procedure SetShowLoginSceneShowRandomCodeDlg;
  end;

procedure ProcessHintText(S:string; Owner:THintLines; List:TList;
  Color:TColor = clWhite; FontSize:Integer = 9;
  FontStyles:TFontStyles = []; IsStroke:Boolean = False; FontName:string = '');

implementation

uses
  Math,
  ClMain,
  MShare,
  FState,
  GameConfigDlg;

constructor THintMessage.Create(AOwner:THintLines);
begin
  FWidth := 0;
  FHeight := 0;
  FOwner := AOwner;
end;

procedure THintMessage.Initialize;
begin
end;

procedure THintMessage.Paint(OwnerRect, DestRect, HintWinRect:TRect);
begin
end;
// ==============================================================================

constructor THintText.Create(AOwner:THintLines);
begin
  inherited Create(AOwner);
  FCaption := '';
  FColor := clWhite;
  FSize := 0;
  FStyle := [];
  FIsStroke := False;
  FFontName := '';
end;

procedure THintText.SetCaption(Value:string);
begin
  if FCaption <> Value then begin
    FCaption := Value;
  end;
  // Initialize;
end;

procedure THintText.SetSize(Value:Integer);
begin
  if FSize <> Value then begin
    FSize := Value;
  end;
  Initialize;
end;

procedure THintText.SetStyle(Value:TFontStyles);
begin
  if FStyle <> Value then begin
    FStyle := Value;
  end;
  // Initialize;
end;

procedure THintText.SetIsStroke(Value:Boolean);
begin
  if FIsStroke <> Value then begin
    FIsStroke := Value;
  end;
  // Initialize;
end;

procedure THintText.Initialize;
var
  HGEFont:THGEFont;
begin
  HGEFont := nil;

  if FFontName <> '' then
    HGEFont := TextureFonts.FindFont(FFontName, Size, FStyle);

  if HGEFont = nil then
    HGEFont := TextureFonts.FindFont(g_sCurFontName, Size, FStyle);

  if HGEFont <> nil then begin
    if FIsStroke then begin
      FHeight := HGEFont.TextHeight('Pp') + 2;
      FWidth := HGEFont.TextWidth(FCaption); //  + 2 去掉这个，不然 {攻击|247}: 2-3 这样的对不齐
    end
    else begin
      FHeight := HGEFont.TextHeight('Pp');
      FWidth := HGEFont.TextWidth(FCaption);
    end;
  end;
end;

procedure THintText.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  ARect:TRect;
  HGEFont:THGEFont;
  d:TTexture;
begin
  if FCaption <> '' then begin
    if FCaption = '-' then begin
      ARect := Bounds(OwnerRect.Left + 4 + 2, OwnerRect.Top + Owner.OffsetY + (OwnerRect.Bottom - OwnerRect.Top - 2) div 2, OwnerRect.Right - OwnerRect.Left - 8, 2);

      // 修改悬浮框中的分界线路径放到NewopUI.pak的00046中
      d := g_WNewopUIImages.Images[46];
      if d = nil then
        GameCanvas.FillRect(ARect, $FF8C715A, $FF8C715A, clBlack1, clBlack1)
      else begin
        if d.Width >= ARect.Right - ARect.Left then
          GameCanvas.Draw(ARect.Left, ARect.Top, Rect(0, 0, ARect.Right - ARect.Left, d.Height), d)

          // 当分隔条长度不够时，拉升绘制分隔条 chongchong 2015-11-11
        else
          GameCanvas.StretchDraw(Rect(ARect.Left, ARect.Top, ARect.Right, ARect.Top + d.Height), d)
      end;
    end else begin
      HGEFont := nil;

      if FFontName <> '' then
        HGEFont := TextureFonts.FindFont(FFontName, Size, FStyle);

      if HGEFont = nil then
        HGEFont := TextureFonts.FindFont(g_sCurFontName, Size, FStyle);

      if HGEFont <> nil then begin
        if FIsStroke then
          BoldTextOut(HGEFont, PaintRect.Left + Owner.OffsetX, PaintRect.Top + Owner.OffsetY + (PaintRect.Bottom - PaintRect.Top - HGEFont.TextHeight(FCaption)) div 2, FCaption, FColor, clBlack)
        else
          HGEFont.TextOut(PaintRect.Left + Owner.OffsetX, PaintRect.Top + Owner.OffsetY + (PaintRect.Bottom - PaintRect.Top - HGEFont.TextHeight(FCaption)) div 2, FCaption, FColor);
      end;
    end;
  end;
end;

{ THintFixedWidthText }

procedure THintFixedWidthText.Initialize;
begin
  inherited;
  if FMinWidth >= FWidth then
    FWidth := FMinWidth;
end;

// ------------------------------------------------------------------------------

function TCountdownText.GetShowText:string;
var
  nTemp, nDay, nHour, sMin, sSec:Integer;
begin
  if FCountdownValue <= 0 then
    Result := '0秒'
  else begin
    nTemp := FCountdownValue;
    nDay := 0;
    nHour := 0;
    sMin := 0;

    if nTemp >= 86400 {60 * 60 * 24} then begin
      nDay := FCountdownValue div 86400;
      nTemp := FCountdownValue mod 86400;
    end;

    if nTemp >= 3600 then begin
      nHour := nTemp div 3600;
      nTemp := nTemp mod 3600;
    end;

    if nTemp >= 60 then begin
      sMin := nTemp div 60;
      nTemp := nTemp mod 60;
    end;

    sSec := nTemp;

    Result := '';
    if nDay > 0 then begin
      Result := Format('%d天%d时%d分%d秒', [nDay, nHour, sMin, sSec]);
    end
    else if nHour > 0 then begin
      Result := Format('%d时%d分%d秒', [nHour, sMin, sSec]);
    end
    else if sMin > 0 then begin
      Result := Format('%d分%d秒', [sMin, sSec]);
    end
    else
      Result := Format('%d秒', [sSec]);
  end;
end;

procedure TCountdownText.Initialize;
var
  HGEFont:THGEFont;
begin
  HGEFont := TextureFonts.FindFont(g_sCurFontName, 9, []);
  if HGEFont <> nil then begin
    FHeight := HGEFont.TextHeight('Pp') + 2;
    FWidth := HGEFont.TextWidth(GetShowText + '      ') + 2;
  end;
end;

procedure TCountdownText.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  HGEFont:THGEFont;
  S:string;
begin
  if FCountdownValue > 0 then begin
    if MyGetTickCount - FStartTick >= 1000 then begin
      FStartTick := MyGetTickCount;
      Dec(FCountdownValue);
    end;
  end;

  HGEFont := TextureFonts.FindFont(g_sCurFontName, 9, []);

  if HGEFont <> nil then begin
    S := GetShowText;
    BoldTextOut(HGEFont, PaintRect.Left + Owner.OffsetX + FOffsetX, PaintRect.Top + Owner.OffsetY + (PaintRect.Bottom - PaintRect.Top - HGEFont.TextHeight(S)) div 2 + FOffsetY, S, FTextColor, clBlack)
  end;
end;

//--------------------------------------------------------------------------------------------------------------
{ THintImage }

procedure THintImage.Initialize;
var
  Texture:TTexture;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];

    if (Texture <> nil) then begin
      FWidth := Texture.Width;
      if FFixedHeight <= 0 then
        FHeight := Texture.Height
      else
        FHeight := FFixedHeight;
    end;

    if FShowBG <> 0 then begin
      if FShowBG > 0 then
        Texture := g_WNewopUIImages.Images[250]
      else
        Texture := g_WNewopUIImages.Images[251];

      if (Texture <> nil) then begin
        FWidth := Max(FWidth, Texture.Width);

        if FFixedHeight <= 0 then
          FHeight := Max(FHeight, Texture.Height)
        else
          FHeight := FFixedHeight;
      end;
    end;
  end;
end;

procedure THintImage.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  Texture:TTexture;
  X, Y, H:Integer;
begin
  if FGameImages <> nil then begin
    H := 0;
    if FShowBG <> 0 then begin
      if FShowBG > 0 then
        Texture := g_WNewopUIImages.Images[250]
      else
        Texture := g_WNewopUIImages.Images[251];

      if Texture <> nil then begin
        X := PaintRect.Left + Owner.OffsetX + (FWidth - Texture.Width) div 2 + FOffsetX;

        H := Texture.Height;

        if FIsVerticalAlignTop then
          Y := PaintRect.Top + Owner.OffsetY + FOffsetY
        else
          Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2;
        GameCanvas.Draw(X, Y, Texture);
      end;
    end;

    Texture := FGameImages.Images[FImageIndex];
    if Texture <> nil then begin
      X := PaintRect.Left + Owner.OffsetX + (FWidth - Texture.Width) div 2 + FOffsetX;

      if FIsVerticalAlignTop then begin
        if H = 0 then
          Y := PaintRect.Top + Owner.OffsetY + FOffsetY
        else
          Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (H - Texture.Height) div 2;
      end
      else
        Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2;
      GameCanvas.Draw(X, Y, Texture);
    end;
  end;
end;

{ TWinHintImage }

procedure TWinHintImage.Initialize;
//var
  //Texture: TTexture;
begin
  FWidth := 0;
  FHeight := 0;

  // 不要这个，不然装备名居中对齐不行 2019-10-21 17:59:44
  {
  if FGameImages <> nil then
  begin
    Texture := FGameImages.Images[FImageIndex];
    if (Texture <> nil) then
    begin
      FWidth := Texture.Width;
    end;
  end;
  }
end;

procedure TWinHintImage.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  Texture:TTexture;
  X, Y:Integer;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];
    if Texture <> nil then begin
      X := HintWinRect.Left + (HintWinRect.Right - HintWinRect.Left - Texture.Width) + FOffsetX;
      Y := HintWinRect.Top + FOffsetY;
      GameCanvas.Draw(X, Y, Texture);
    end;
  end;
end;

{ TLineBGHintImage }

procedure TLineBGHintImage.Initialize;
var
  Texture:TTexture;
begin
  FWidth := 0;
  FTextureWidth := 0;

  // 不要这个，不然装备名居中对齐不行 2019-10-21 17:59:44
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];
    if (Texture <> nil) then begin
      FHeight := Texture.Height;
      FTextureWidth := Texture.Width;
    end;
  end;
end;

procedure TLineBGHintImage.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  Texture:TTexture;
  X, Y:Integer;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];
    if Texture <> nil then begin
      (*
      X := PaintRect.Left + Owner.OffsetX {+ (FWidth - Texture.Width)} + FOffsetX;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2;
      R := Rect(0, 0, OwnerRect.Right - OwnerRect.Left - 1, Texture.Height);
      GameCanvas.Draw(X, Y, R, Texture, Blend_Default);
      *)

      X := HintWinRect.Left + g_ClientConfig.HintWindowBorderWidth.Left;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2;

      GameCanvas.Draw(X, Y, Texture);
    end;
  end;
end;

{ THintPlayImage }

procedure THintPlayImage.Initialize;
begin
  FWidth := FFixedWidth;
  FHeight := FFixedHeight;
end;

procedure THintPlayImage.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  Texture:TTexture;
  X, Y:Integer;
  Pt:TPoint;
  CurTick:Cardinal;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.GetCachedImage(FPlayImageIndex, Pt.X, Pt.Y);
    if Texture <> nil then begin
      // 修正物品和对应的特效对不上 chongchong 2015-04-11
      X := PaintRect.Left + Owner.OffsetX + FOffsetX + Pt.X;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY + Pt.Y {+ (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2};

      if FBlendDraw then
        GameCanvas.DrawBlend(X, Y, Texture)
      else
        GameCanvas.Draw(X, Y, Texture)
    end;

    CurTick := MyGetTickCount;
    if CurTick - FPlayTick > Cardinal(FPlayTime) then begin
      FPlayTick := CurTick;
      Inc(FPlayImageIndex);
      if FPlayImageIndex - FImageIndex + 1 > FPlayCount then
        FPlayImageIndex := FImageIndex;
    end;
  end;
end;

// ------------------------------------------------------------------------------

{ THintPlayImageEx }

procedure THintPlayImageEx.Initialize;
var
  Texture:TTexture;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FImageIndex];
    if Texture <> nil then begin
      FWidth := Texture.Width + FIncSpacing;
      FHeight := Texture.Height;
    end;
  end;
end;

procedure THintPlayImageEx.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  Texture:TTexture;
  X, Y:Integer;
  CurTick:Cardinal;
begin
  if FGameImages <> nil then begin
    Texture := FGameImages.Images[FPlayImageIndex];
    if Texture <> nil then begin
      // 修正物品和对应的特效对不上 chongchong 2015-04-11
      X := PaintRect.Left + Owner.OffsetX + FOffsetX;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY {+ (OwnerRect.Bottom - OwnerRect.Top - Texture.Height) div 2};

      if FBlendDraw then
        GameCanvas.DrawBlend(X, Y, Texture)
      else
        GameCanvas.Draw(X, Y, Texture)
    end;

    CurTick := MyGetTickCount;
    if CurTick - FPlayTick > Cardinal(FPlayTime) then begin
      FPlayTick := CurTick;
      Inc(FPlayImageIndex);
      if FPlayImageIndex - FImageIndex + 1 > FPlayCount then
        FPlayImageIndex := FImageIndex;
    end;
  end;
end;

// ------------------------------------------------------------------------------

{ THintItemProgress }

procedure THintItemProgress.Initialize;
var
  Texture:TTexture;
begin
  if (FGameImages <> nil) {and (FPlayCount > 0)} then begin
    if FProgressIndex = 1 then
      Texture := FGameImages.Images[640]
    else
      Texture := FGameImages.Images[620];

    if (Texture <> nil) then begin
      FWidth := Texture.Width + 2;
      FHeight := Texture.Height
    end;
  end
  else begin
    FWidth := 0;
    FHeight := 0;
  end;
end;

procedure THintItemProgress.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  TextureBG, Texture:TTexture;
  X, Y, TextW, TextH:Integer;
  CurTick:Cardinal;
  R:TRect;
begin
  if FGameImages <> nil then begin
    if FProgressIndex = 1 then
      TextureBG := FGameImages.Images[640]
    else
      TextureBG := FGameImages.Images[620];

    if (TextureBG <> nil) and (FPlayCount > 0) then begin
      // 修正物品和对应的特效对不上 chongchong 2015-04-11
      X := PaintRect.Left + Owner.OffsetX + FOffsetX;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY;

      GameCanvas.Draw(X, Y, TextureBG);
    end;

    if FMaxValue = 0 then Exit;

    if (FCurValue > 0) and (FPlayCount > 0) then begin
      if FPlayCount >= 10 then begin
        CurTick := MyGetTickCount;
        if CurTick - FPlayTick > 500 then begin
          FPlayTick := CurTick;
          Inc(FPlayImageIndex);
          if FPlayImageIndex > FPlayCount - 10 then
            FPlayImageIndex := 0;
        end;

        if FProgressIndex = 1 then
          Texture := FGameImages.Images[650 + FPlayImageIndex]
        else
          Texture := FGameImages.Images[630 + FPlayImageIndex];

        if Texture <> nil then begin
          // 修正物品和对应的特效对不上 chongchong 2015-04-11
          if TextureBG <> nil then begin
            X := PaintRect.Left + Owner.OffsetX + FOffsetX + (TextureBG.Width - Texture.Width) div 2;
            Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (TextureBG.Height - Texture.Height) div 2;
          end
          else begin
            X := PaintRect.Left + Owner.OffsetX + FOffsetX;
            Y := PaintRect.Top + Owner.OffsetY + FOffsetY;
          end;

          R := Rect(0, 0, Texture.Width, Texture.Height);
          R.Right := R.Left + Round((R.Right - R.Left) / FMaxValue * FCurValue);

          GameCanvas.Draw(X, Y, R, Texture);
        end;
      end
      else begin
        if FProgressIndex = 1 then
          Texture := FGameImages.Images[640 + FPlayCount]
        else
          Texture := FGameImages.Images[620 + FPlayCount];

        if Texture <> nil then begin
          // 修正物品和对应的特效对不上 chongchong 2015-04-11
          if TextureBG <> nil then begin
            X := PaintRect.Left + Owner.OffsetX + FOffsetX + (TextureBG.Width - Texture.Width) div 2;
            Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (TextureBG.Height - Texture.Height) div 2;
          end
          else begin
            X := PaintRect.Left + Owner.OffsetX + FOffsetX;
            Y := PaintRect.Top + Owner.OffsetY + FOffsetY;
          end;

          R := Rect(0, 0, Texture.Width, Texture.Height);
          R.Right := R.Left + Round((R.Right - R.Left) / FMaxValue * FCurValue);

          GameCanvas.Draw(X, Y, R, Texture);
        end;
      end;
    end;

    if (TextureBG <> nil) and (Length(FText) > 0) then begin
      TextW := CurrentFont.TextWidth(FText);
      TextH := g_CurrentFontHeight;

      // 修正物品和对应的特效对不上 chongchong 2015-04-11
      X := PaintRect.Left + Owner.OffsetX + FOffsetX + (TextureBG.Width - TextW) div 2;
      Y := PaintRect.Top + Owner.OffsetY + FOffsetY + (TextureBG.Height - TextH) div 2;

      BoldTextOut(X, Y, FText, FTextColor);
    end;
  end;
end;

{ THintImageNumber }

procedure THintImageNumber.Initialize;
var
  Texture:TTexture;
  II, nIndex:Integer;
begin
  FWidth := 0;
  FHeight := 0;
  if (FGameImages <> nil) and (FNumberIndex >= 0) and (FNumberIndex <= 9) and (Length(FNumberValue) > 0) then begin
    for II := 1 to Length(FNumberValue) do begin
      nIndex := 1230 + (FNumberIndex * 10) + StrToInt(FNumberValue[II]);
      Texture := FGameImages.Images[nIndex];
      if Texture <> nil then begin
        if FHeight < Texture.Height then FHeight := Texture.Height;
        FWidth := FWidth + Texture.Width + FNumberSpace;
      end;
    end;
  end;
end;

procedure THintImageNumber.Paint(OwnerRect, PaintRect, HintWinRect:TRect);
var
  PX, PY, Index, I:Integer;
  Texture:TTexture;
begin
  PX := PaintRect.Left + Owner.OffsetX + FOffsetX;
  PY := PaintRect.Top + Owner.OffsetY + FOffsetY;

  if (FGameImages <> nil) and (Length(FNumberValue) > 0) then begin
    for I := 1 to Length(FNumberValue) do begin
      Index := 1230 + (FNumberIndex * 10) + StrToInt(FNumberValue[I]);
      Texture := FGameImages.Images[Index];
      if Texture <> nil then begin
        GameCanvas.Draw(PX, PY + (Height - Texture.Height) div 2, Texture);
        PX := PX + Texture.Width + FNumberSpace;
      end;
    end;
  end;
end;

// ------------------------------------------------------------------------------

constructor THintLines.Create;
begin
  inherited;
  FWidth := 0;
  FHeight := 0;
  FItemHeight := 0;
  FOffsetX := g_ClientConfig.HintWindowBorderWidth.Left;
  FOffsetY := g_ClientConfig.HintWindowBorderWidth.Top;
  FAlignment := taLeftJustify;
  FIsItemIconText := False;
  FList := TList.Create;
end;

destructor THintLines.Destroy;
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do begin
    THintMessage(FList.Items[I]).Free;
  end;
  FList.Free;
  inherited;
end;

procedure THintLines.GetSize;
var
  I:Integer;
  HintMessage:THintMessage;
  MaxWidth:Integer;
begin
  FWidth := 0;
  FHeight := 0;
  MaxWidth := 0;

  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);
    HintMessage.Initialize;
    FHeight := Max(FHeight, HintMessage.Height);
    FWidth := FWidth + HintMessage.Width;

    if HintMessage is TLineBGHintImage then begin
      FMinWidth := TLineBGHintImage(HintMessage).FTextureWidth;
    end;
  end;

  if FWidth < MaxWidth then begin
    FWidth := MaxWidth;
  end;

  if CurrentFont <> nil then begin
    if FWidth <= 0 then
      FWidth := CurrentFont.TextWidth('0');

    if FHeight <= 0 then
      FHeight := g_CurrentFontHeight;
  end;
  FItemHeight := FHeight + 2;
end;

function THintLines.Get(Index:Integer):string;
var
  HintMessage:THintMessage;
begin
  if (Index >= 0) and (Index < FList.Count) then begin
    HintMessage := THintMessage(FList.Items[Index]);
    if HintMessage is THintText then
      Result := THintText(HintMessage).Caption
    else
      Result := '';
  end
  else
    Result := '';
end;

function THintLines.GetCount:Integer;
begin
  Result := FList.Count;
end;

procedure THintLines.Put(Index:Integer; const S:string);
var
  HintMessage:THintMessage;
begin
  if (Index >= 0) and (Index < FList.Count) then begin
    HintMessage := THintMessage(FList.Items[Index]);
    if HintMessage is THintText then
      THintText(HintMessage).Caption := S;
  end;
end;

// 提示窗口支持自定义图片显示 chongchong 2015-01-10

procedure ProcessHintText(S:string; Owner:THintLines; List:TList; Color:TColor; FontSize:Integer;
  FontStyles:TFontStyles; IsStroke:Boolean; FontName:string);

  procedure NewHintText(Text:WideString);
  var
    S1, S2, S3, S4:WideString;
    Index1, Index2, Index3, nColor:Integer;
    boCustomColorText:Boolean;
    HintText:THintText;
  begin
    if Length(Text) = 0 then begin
      HintText := THintText.Create(Owner);
      HintText.Size := FontSize;
      HintText.Style := FontStyles;
      HintText.Color := Color;
      HintText.IsStroke := IsStroke;
      HintText.Caption := Text;
      HintText.FFontName := FontName;
      List.Add(HintText);
      Exit;
    end;

    Index1 := Pos(WideString('{'), Text); //Pos('{', Text); HEZUQING 解决重载的问题  20230423

    if Index1 > 0 then
      Index2 := Pos(WideString('}'), Text) //Pos('}', Text) HEZUQING 解决重载的问题  20230423
    else
      Index2 := 0;

    while (Index1 > 0) and (Index2 > 0) and (Text <> '') do begin
      S1 := Copy(Text, 1, Index1 - 1);
      S2 := Copy(Text, Index1 + 1, Index2 - Index1 - 1);

      if Length(S1) > 0 then begin
        HintText := THintText.Create(Owner);
        HintText.Size := FontSize;
        HintText.Style := FontStyles;
        HintText.Color := Color;
        HintText.IsStroke := IsStroke;
        HintText.Caption := S1;
        HintText.FFontName := FontName;
        List.Add(HintText);
      end;

      boCustomColorText := False;
      if Length(S2) > 0 then begin
        Index3 := Pos('|', S2);
        if Index3 > 0 then begin
          S3 := Copy(S2, 1, Index3 - 1);
          S4 := Copy(S2, Index3 + 1, MaxInt);

          nColor := StrToIntDef(S4, -1);
          if (nColor >= 0) and (nColor <= 255) then begin
            boCustomColorText := True;

            if Length(S3) > 0 then begin
              HintText := THintText.Create(Owner);
              HintText.Size := FontSize;
              HintText.Style := FontStyles;
              HintText.Color := GetRGB(nColor);
              HintText.IsStroke := IsStroke;
              HintText.Caption := S3;
              HintText.FFontName := FontName;
              List.Add(HintText);
            end;
          end;
        end;
      end;

      if not boCustomColorText then begin
        HintText := THintText.Create(Owner);
        HintText.Size := FontSize;
        HintText.Style := FontStyles;
        HintText.Color := Color;
        HintText.IsStroke := IsStroke;
        HintText.Caption := '{' + S2 + '}';
        HintText.FFontName := FontName;
        List.Add(HintText);
      end;

      Text := Copy(Text, Index2 + 1, MaxInt);
      Index1 := Pos('{', Text);
      if Index1 > 0 then
        Index2 := Pos('}', Text)
      else
        Index2 := 0;
    end;

    if Length(Text) > 0 then begin
      HintText := THintText.Create(Owner);
      HintText.Size := FontSize;
      HintText.Style := FontStyles;
      HintText.Color := Color;
      HintText.IsStroke := IsStroke;
      HintText.Caption := Text;
      HintText.FFontName := FontName;
      List.Add(HintText);
    end;
  end;

  procedure NewFixedWidthHintText(Text:WideString; FixedWidth:Integer; AColor:Integer);
  var
    //S1, S2, S3, S4: WideString;
    //Index1, Index2, Index3, nColor: Integer;
    //boCustomColorText: Boolean;
    HintText:THintText;
  begin
    HintText := THintFixedWidthText.Create(Owner);
    THintFixedWidthText(HintText).FMinWidth := FixedWidth;
    HintText.Size := FontSize;
    HintText.Style := FontStyles;
    if (AColor >= 0) and (AColor <= 255) then
      HintText.Color := GetRGB(AColor)
    else
      HintText.Color := Color;
    HintText.IsStroke := IsStroke;
    HintText.Caption := Text;
    HintText.FFontName := FontName;
    List.Add(HintText);
  end;

  procedure NewHintImage(Text:string; AGameImages:TGameImages; AImageIndex:Integer; OffsetX, OffsetY:Integer; ShowBG:Integer; FixedHeight:Integer; IsVerticalAlignTop:Boolean);
  var
    HintImage:THintImage;
  begin
    if AGameImages <> nil then begin
      HintImage := THintImage.Create(Owner);
      HintImage.FIsVerticalAlignTop := IsVerticalAlignTop;
      HintImage.FGameImages := AGameImages;
      HintImage.FImageIndex := AImageIndex;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      HintImage.FShowBG := ShowBG;
      HintImage.FFixedHeight := FixedHeight;

      List.Add(HintImage);
    end;
  end;

  procedure NewHintImageNumber(Text:string; AGameImages:TGameImages; NumberIndex, NumberSpace, OffsetX, OffsetY:Integer);
  var
    HintImage:THintImageNumber;
  begin
    if AGameImages <> nil then begin
      HintImage := THintImageNumber.Create(Owner);
      HintImage.FGameImages := AGameImages;
      HintImage.FNumberIndex := NumberIndex;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      HintImage.FNumberSpace := NumberSpace;
      HintImage.FNumberValue := Text;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintPlayImage(Text:string; AGameImages:TGameImages; AImageIndex, APlayCount, APlayTime:Integer; OffsetX, OffsetY:Integer; IsBlendDraw:Boolean; FixedWith, FixedHeight:Integer);
  var
    HintImage:THintPlayImage;
  begin
    if AGameImages <> nil then begin
      HintImage := THintPlayImage.Create(Owner);
      HintImage.FGameImages := AGameImages;
      HintImage.FImageIndex := AImageIndex;
      HintImage.FPlayImageIndex := AImageIndex;
      HintImage.FPlayCount := APlayCount;
      HintImage.FPlayTime := APlayTime;
      HintImage.FPlayTick := MyGetTickCount;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      HintImage.FBlendDraw := IsBlendDraw;
      HintImage.FFixedWidth := FixedWith;
      HintImage.FFixedHeight := FixedHeight;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintWinImage(Text:string; AGameImages:TGameImages; AImageIndex:Integer; OffsetX, OffsetY:Integer; HorzAligment, VertAligment:Integer);
  var
    HintImage:TWinHintImage;
  begin
    if AGameImages <> nil then begin
      HintImage := TWinHintImage.Create(Owner);
      HintImage.FGameImages := AGameImages;
      HintImage.FImageIndex := AImageIndex;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintLineBGImage(Text:string; AGameImages:TGameImages; AImageIndex:Integer; OffsetX, OffsetY:Integer; HorzAligment, VertAligment:Integer);
  var
    HintImage:TLineBGHintImage;
  begin
    if AGameImages <> nil then begin
      HintImage := TLineBGHintImage.Create(Owner);
      HintImage.FGameImages := AGameImages;
      HintImage.FImageIndex := AImageIndex;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintPlayImageEx(Text:string; AGameImages:TGameImages; AImageIndex, APlayCount, APlayTime:Integer; OffsetX, OffsetY:Integer; IsBlendDraw:Boolean; IncSpacing:Integer);
  var
    HintImage:THintPlayImageEx;
  begin
    if AGameImages <> nil then begin
      HintImage := THintPlayImageEx.Create(Owner);
      HintImage.FGameImages := AGameImages;
      HintImage.FImageIndex := AImageIndex;
      HintImage.FPlayImageIndex := AImageIndex;
      HintImage.FPlayCount := APlayCount;
      HintImage.FPlayTime := APlayTime;
      HintImage.FPlayTick := MyGetTickCount;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      HintImage.FBlendDraw := IsBlendDraw;
      HintImage.FIncSpacing := IncSpacing;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintItemProgress(AColor:Byte; Text:string; AGameImages:TGameImages; AProgressIndex, APlayCount, AMaxValue, ACurValue:Integer; OffsetX, OffsetY:Integer);
  var
    HintImage:THintItemProgress;
  begin
    if AGameImages <> nil then begin
      HintImage := THintItemProgress.Create(Owner);
      HintImage.TextColor := GetRGB(AColor);
      HintImage.FText := Text;
      HintImage.FGameImages := AGameImages;
      HintImage.FProgressIndex := AProgressIndex;
      HintImage.FPlayCount := APlayCount;
      HintImage.FMaxValue := AMaxValue;
      HintImage.FCurValue := ACurValue;
      HintImage.FOffsetX := OffsetX;
      HintImage.FOffsetY := OffsetY;
      HintImage.FPlayTick := MyGetTickCount;
      HintImage.FPlayImageIndex := 0;
      List.Add(HintImage);
    end;
  end;

  procedure NewHintCountdownText(AMaxValue:Integer; AColor:Byte; OffsetX, OffsetY:Integer);
  var
    HintImage:TCountdownText;
  begin
    HintImage := TCountdownText.Create(Owner);
    HintImage.FCountdownValue := AMaxValue;
    HintImage.FStartTick := MyGetTickCount;
    HintImage.FTextColor := GetRGB(AColor);
    HintImage.FOffsetX := OffsetX;
    HintImage.FOffsetY := OffsetY;
    List.Add(HintImage);
  end;

  function CheckHintImage(Text:string; var AGameImages:TGameImages; var ImageIndex, OffsetX, OffsetY:Integer;
    var ShowBG:Integer; var FixedHeight:Integer; var IsVerticalAlignTop:Boolean; var IsPlayImg, IsNewopUIPlayImg:Boolean; var PlayCount, PlayTime:Integer;
    var IsBlendDraw:Boolean; var IsItemProgress:Boolean; var IsImgNumber:Boolean; var MaxValue, CurValue:Integer; var TextColor:Byte; var ShowText:string;
    var IsCountdown:Boolean; var IncSpacing:Integer; var IsWinImage, IsLineBGImage, IsFixedTextWidth:Boolean):Boolean;
  var
    Temp:string;
    SName, S1, S2, S3, S4, S5, S6, S7, S8, S9:string;
    I1, I2:Integer;
  begin
    Result := False; //'<Img:N:F:X:Y>'
    AGameImages := nil;

    if Length(Text) > 2 then begin
      Temp := Copy(Text, 2, Length(Text) - 2);
      Temp := GetValidStr3_Ex(Temp, SName, ':');
      Temp := GetValidStr3_Ex(Temp, S1, ':');
      Temp := GetValidStr3_Ex(Temp, S2, ':');
      Temp := GetValidStr3_Ex(Temp, S3, ':');
      Temp := GetValidStr3_Ex(Temp, S4, ':');
      Temp := GetValidStr3_Ex(Temp, S5, ':');
      Temp := GetValidStr3_Ex(Temp, S6, ':');
      Temp := GetValidStr3_Ex(Temp, S7, ':');
      Temp := GetValidStr3_Ex(Temp, S8, ':');
      Temp := GetValidStr3_Ex(Temp, S9, ':');

      I1 := StrToIntDef(S1, -1);
      I2 := StrToIntDef(S2, -1);
      IsPlayImg := False;
      IsNewopUIPlayImg := False;
      ShowBG := 0;
      FixedHeight := 0;
      IsVerticalAlignTop := False;

      IsBlendDraw := True;
      IsItemProgress := False;
      IsImgNumber := False;
      IsCountdown := False;
      IncSpacing := 0;
      IsWinImage := False;
      IsLineBGImage := False;
      IsFixedTextWidth := False;

      // <img: xxxx
      if SameText(SName, 'TextW') and (I1 >= 0) then begin
        IsFixedTextWidth := True;
        OffsetX := I1;
        OffsetY := I2;

        Temp := Copy(Text, 2, Length(Text) - 2);
        Temp := GetValidStr3_Ex(Temp, SName, ':');
        Temp := GetValidStr3_Ex(Temp, S1, ':');
        ShowText := GetValidStr3_Ex(Temp, S2, ':');

        Result := True;
      end
      else if SameText(SName, 'Img') and (I2 >= 0) then begin
        if (I1 >= 0) then begin
          Result := True;

          if I2 < g_EffectImageList.Count then
            AGameImages := TGameImages(g_EffectImageList.Objects[I2]);

          ImageIndex := I1;
          OffsetX := StrToIntDef(S3, 0);
          OffsetY := StrToIntDef(S4, 0);
          ShowBG := StrToIntDef(S5, 0);
          FixedHeight := StrToIntDef(S6, 0);
          IsVerticalAlignTop := StrToIntDef(S7, 0) = 1;
        end;
      end
      else if SameText(SName, 'Looks') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WBagItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
        ShowBG := StrToIntDef(S4, 0);
        FixedHeight := StrToIntDef(S5, 0);
        IsVerticalAlignTop := StrToIntDef(S6, 0) = 1;
      end
      else if SameText(SName, 'DnItems') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WDnItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
        ShowBG := StrToIntDef(S4, 0);
        FixedHeight := StrToIntDef(S5, 0);
        IsVerticalAlignTop := StrToIntDef(S6, 0) = 1;
      end
      else if SameText(SName, 'StateItem') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WStateItemImages.Looks[I1];
        ImageIndex := I1 mod 10000;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
        ShowBG := StrToIntDef(S4, 0);
        FixedHeight := StrToIntDef(S5, 0);
        IsVerticalAlignTop := StrToIntDef(S6, 0) = 1;
      end

        //格式: <NewopUIPlay:N:C:T:X:Y:M>
        //N表示播放开始图片,C表示播放张数,T表示播放速度(毫秒),X是横向坐标,Y是纵向坐标;M绘制模式
      else if SameText(SName, 'NewopPlayImg') and (I2 >= 0) then begin
        Result := True;
        IsNewopUIPlayImg := True;

        PlayCount := I2;
        PlayTime := StrToIntDef(S3, 0);

        if PlayTime <= 0 then
          PlayTime := 100;

        if PlayCount > 0 then
          AGameImages := g_WNewopUIImages;

        ImageIndex := I1;

        OffsetX := StrToIntDef(S4, 0);
        OffsetY := StrToIntDef(S5, 0);
        IsBlendDraw := StrToIntDef(S6, 0) <> 0;
        IncSpacing := StrToIntDef(S7, 0);
      end

      else if SameText(SName, 'NewopUI') and (I1 >= 0) then begin
        Result := True;
        AGameImages := g_WNewopUIImages;
        ImageIndex := I1;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
      end

        // 提示窗图片（位置随提示窗变化，不随行）
      else if SameText(SName, 'WinNewopUI') and (I1 >= 0) then begin
        Result := True;
        IsWinImage := True;
        AGameImages := g_WNewopUIImages;
        ImageIndex := I1;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
        MaxValue := StrToIntDef(S4, 0); // 水平对齐(0:左;1:中;1:右)
        CurValue := StrToIntDef(S5, 0); // 垂直对齐(0:上;1:中;1:下)
      end

        // 提示窗图片（位置随提示窗变化，不随行）
      else if SameText(SName, 'LineNewopUI') and (I1 >= 0) then begin
        Result := True;
        IsLineBGImage := True;
        AGameImages := g_WNewopUIImages;
        ImageIndex := I1;
        OffsetX := StrToIntDef(S2, 0);
        OffsetY := StrToIntDef(S3, 0);
        MaxValue := StrToIntDef(S4, 0); // 水平对齐(0:左;1:中;1:右)
        CurValue := StrToIntDef(S5, 0); // 垂直对齐(0:上;1:中;1:下)
      end

        //格式: <PlayImg:F:N:C:T:X:Y:M/@Label>
        //F表示WIL文件序号,N表示播放开始图片,C表示播放张数,T表示播放速度(毫秒),X是横向坐标,Y是纵向坐标;M绘制模式
      else if SameText(SName, 'PlayImg') and (I1 >= 0) and (I2 >= 0) then begin
        Result := True;
        IsPlayImg := True;

        PlayCount := StrToIntDef(S3, 0);
        PlayTime := StrToIntDef(S4, 0);

        if PlayTime <= 0 then
          PlayTime := 100;

        if (I1 < g_EffectImageList.Count) and (PlayCount > 0) then
          AGameImages := TGameImages(g_EffectImageList.Objects[I1]);

        ImageIndex := I2;

        OffsetX := StrToIntDef(S5, 0);
        OffsetY := StrToIntDef(S6, 0);
        IsBlendDraw := StrToIntDef(S7, 0) <> 0;
        FixedHeight := StrToIntDef(S8, 0);
        IncSpacing := StrToIntDef(S9, 0);
      end

        //格式: <ItemProgress:N:C:M:V:X:Y:S/@Label>
        //N表示进度条序号; C表示进度条显示图片数量；M:进度条最大值; V:当前值; X,Y: 坐标偏移；R: 显示值颜色; S: 进度条显示的值
      else if SameText(SName, 'ItemProgress') and (I1 >= 0) and (I2 >= 0) then begin
        Result := True;
        IsItemProgress := True;

        AGameImages := g_WNewopUIImages;
        ImageIndex := I1;
        PlayCount := I2;

        MaxValue := StrToIntDef(S3, 0);
        CurValue := StrToIntDef(S4, 0);

        OffsetX := StrToIntDef(S5, 0);
        OffsetY := StrToIntDef(S6, 0);
        TextColor := StrToIntDef(S7, 0);
        ShowText := S8;
      end
      else if SameText(SName, 'ImgNum') and (I1 >= 0) and (I1 <= 9) and (I2 > 0) then begin
        Result := True;
        IsImgNumber := True;

        AGameImages := g_WNewopUIImages;

        ImageIndex := I1; // 数字类型
        ShowText := IntToStr(I2); // 数字值
        PlayCount := StrToIntDef(S3, 0); // 字符间隔
        OffsetX := StrToIntDef(S4, 0); // X
        OffsetY := StrToIntDef(S5, 0); // Y
      end
      else if SameText(SName, 'Countdown') and (I1 >= 0) then begin
        Result := True;
        IsCountdown := True;
        MaxValue := I1;
        TextColor := StrToIntDef(S2, 0);
        OffsetX := StrToIntDef(S3, 0);
        OffsetY := StrToIntDef(S4, 0);
      end;
    end;
  end;
var
  Index1, Index2:Integer;
  StrB, StrC:string;
  OX, OY:Integer;
  GameImages:TGameImages;
  GameImageIndex:Integer;
  IsVerticalAlignTop, IsPlayImg, IsNewopUIPlayImg, IsBlendDraw, IsItemProgress, IsImgNumber, IsCountdown, IsWinImage, IsLineBGImage, IsFixedTextWidth:Boolean;
  ShowBG, FixedHeight, PlayCount, PlayTime, IncSpacing:Integer;

  MaxValue, CurValue:Integer;
  ShowText:string;
  btColor:Byte;
begin
  Index1 := Pos('<', S);
  if Index1 = 0 then begin
    NewHintText(S);
  end
  else begin
    while S <> '' do begin
      StrB := StrB + Copy(S, 1, Index1 - 1);
      S := Copy(S, Index1, MAXINT);

      Index2 := Pos('>', S);
      if Index2 = 0 then begin
        NewHintText(StrB + S);
        Break;
      end
      else begin
        IsPlayImg := False;
        IsNewopUIPlayImg := False;
        IsBlendDraw := False;
        IsItemProgress := False;
        IsImgNumber := False;
        IsCountdown := False;
        IsFixedTextWidth := False;

        IsVerticalAlignTop := False;

        StrC := Copy(S, 1, Index2);
        if not CheckHintImage(StrC, GameImages, GameImageIndex, OX, OY, ShowBG, FixedHeight, IsVerticalAlignTop, IsPlayImg, IsNewopUIPlayImg, PlayCount, PlayTime, IsBlendDraw,
          IsItemProgress, IsImgNumber, MaxValue, CurValue, btColor, ShowText, IsCountdown, IncSpacing, IsWinImage, IsLineBGImage, IsFixedTextWidth) then
          StrB := StrB + StrC
        else begin
          if Length(StrB) > 0 then
            NewHintText(StrB);

          if IsFixedTextWidth then
            NewFixedWidthHintText(ShowText, OX, OY)
          else if IsPlayImg then
            NewHintPlayImage(StrC, GameImages, GameImageIndex, PlayCount, PlayTime, OX, OY, IsBlendDraw, IncSpacing, FixedHeight)
          else if IsWinImage then
            NewHintWinImage(StrC, GameImages, GameImageIndex, OX, OY, MaxValue, CurValue)
          else if IsLineBGImage then
            NewHintLineBGImage(StrC, GameImages, GameImageIndex, OX, OY, MaxValue, CurValue)
          else if IsNewopUIPlayImg then
            NewHintPlayImageEx(StrC, GameImages, GameImageIndex, PlayCount, PlayTime, OX, OY, IsBlendDraw, IncSpacing)
          else if IsItemProgress then
            NewHintItemProgress(btColor, ShowText, GameImages, GameImageIndex, PlayCount, MaxValue, CurValue, OX, OY)
          else if IsImgNumber then
            NewHintImageNumber(ShowText, GameImages, GameImageIndex, PlayCount, OX, OY)
          else if IsCountdown then begin
            NewHintCountdownText(MaxValue, btColor, OX, OY);
          end
          else
            NewHintImage(StrC, GameImages, GameImageIndex, OX, OY, ShowBG, FixedHeight, IsVerticalAlignTop);

          StrB := '';
          StrC := '';
        end;

        S := Copy(S, Index2 + 1, MaxInt);
        Index1 := Pos('<', S);
        if Index1 = 0 then begin
          S := StrB + S;
          if S <> '' then
            NewHintText(S);
          Break;
        end;
      end;
    end;
  end;

end;

procedure THintLines.Add(S:string; Color:TColor;
  FontSize:Integer; FontStyles:TFontStyles; IsStroke:Boolean; FontName:string);
var
  I:Integer;
  List:TList;
begin
  List := TList.Create;
  try
    ProcessHintText(S, Self, List, Color, FontSize, FontStyles, IsStroke, FontName);

    for I := 0 to List.Count - 1 do begin
      FList.Add(List[I]);
    end;
  finally
    List.Free;
  end;

  GetSize;
end;

procedure THintLines.AddFixedHeightLine(Height:Integer);
var
  Line:TFiexdHeightLine;
begin
  Line := TFiexdHeightLine.Create(Self);
  Line.FHeight := Height;
  FList.Add(Line);
  GetSize;
end;

procedure THintLines.Insert(Index:Integer; S:string; Color:TColor;
  FontSize:Integer; FontStyles:TFontStyles; IsStroke:Boolean; FontName:string);
var
  I:Integer;
  List:TList;
begin
  List := TList.Create;
  try
    ProcessHintText(S, Self, List, Color, FontSize, FontStyles, IsStroke, FontName);

    for I := List.Count - 1 downto 0 do begin
      FList.Insert(Index, List[I]);
    end;
  finally
    List.Free;
  end;

  GetSize;
end;

procedure THintLines.Clear;
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do begin
    THintMessage(FList.Items[I]).Free;
  end;
  FList.Clear;
end;

procedure THintLines.Delete(Index:Integer);
begin
  if (Index >= 0) and (Index < FList.Count) then begin
    THintMessage(FList.Items[Index]).Free;
    FList.Delete(Index);
  end;
end;

function THintLines.GetObject(Index:Integer):THintMessage;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := THintMessage(FList.Items[Index])
  else
    Result := nil;
end;

procedure THintLines.Paint(OwnerRect, PaintRect:TRect);
var
  I, nX:Integer;
  HintMessage:THintMessage;
begin
  nX := PaintRect.Left;
  // 将带播放的特效放在最前面放 chongchong 2015-01-22
  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);
    if (HintMessage is THintPlayImage) or (HintMessage is THintPlayImageEx) or (HintMessage is THintImage) then begin
      HintMessage.Paint(PaintRect, Bounds(nX, PaintRect.Top, HintMessage.Width, ItemHeight), OwnerRect);
    end;
    Inc(nX, HintMessage.Width);
  end;

  nX := PaintRect.Left;
  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);
    if not ((HintMessage is THintPlayImage) or (HintMessage is THintPlayImageEx) or (HintMessage is THintImage)) then begin
      HintMessage.Paint(PaintRect, Bounds(nX, PaintRect.Top, HintMessage.Width, ItemHeight), OwnerRect);
    end;
    Inc(nX, HintMessage.Width);
  end;
end;

procedure THintLines.PaintWithoutWinHintImage(OwnerRect, PaintRect:TRect; var HaveWinHintImage:Boolean);
var
  I, nX:Integer;
  HintMessage:THintMessage;
begin
  nX := PaintRect.Left;
  // 将带播放的特效放在最前面放 chongchong 2015-01-22
  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);

    if HintMessage is TWinHintImage then begin
      HaveWinHintImage := True;
    end
    else if (HintMessage is THintPlayImage) or (HintMessage is THintPlayImageEx) or (HintMessage is THintImage) then begin
      HintMessage.Paint(PaintRect, Bounds(nX, PaintRect.Top, HintMessage.Width, ItemHeight), OwnerRect);
    end;
    Inc(nX, HintMessage.Width);
  end;

  nX := PaintRect.Left;
  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);
    if not ((HintMessage is THintPlayImage) or (HintMessage is THintPlayImageEx) or (HintMessage is THintImage) or (HintMessage is TWinHintImage)) then begin
      HintMessage.Paint(PaintRect, Bounds(nX, PaintRect.Top, HintMessage.Width, ItemHeight), OwnerRect);
    end;
    Inc(nX, HintMessage.Width);
  end;
end;

procedure THintLines.PaintWinHintImage(OwnerRect, PaintRect:TRect);
var
  I, nX:Integer;
  HintMessage:THintMessage;
begin
  nX := PaintRect.Left;
  // 将带播放的特效放在最前面放 chongchong 2015-01-22
  for I := 0 to FList.Count - 1 do begin
    HintMessage := THintMessage(FList.Items[I]);

    if HintMessage is TWinHintImage then begin
      HintMessage.Paint(PaintRect, Bounds(nX, PaintRect.Top, HintMessage.Width, ItemHeight), OwnerRect);
    end;
    Inc(nX, HintMessage.Width);
  end;
end;

// ------------------------------------------------------------------------------

constructor THintWindow.Create;
begin
  inherited Create;
  FVisible := True;
end;

destructor THintWindow.Destroy;
begin
  Clear;
  inherited Destroy;
end;

procedure THintWindow.Clear;
var
  I:Integer;
begin
  FVisible := False;
  for I := 0 to Count - 1 do begin
    THintLines(Items[I]).Free;
  end;
  inherited Clear();
end;

procedure THintWindow.DrawBackground();
var
  SourceRect:TRect;
  nWidth, nHeight:Integer;
  d:TTexture;
  Color:TColor;
begin
  SourceRect := Bounds(HintRect.Left + 2, HintRect.Top + 2, HintRect.Right - HintRect.Left - 2, HintRect.Bottom - HintRect.Top - 2);
  // SourceRect := Bounds(HintRect.Left + 2, HintRect.Top + 2, HintRect.Right - HintRect.Left - 4, HintRect.Bottom - HintRect.Top - 4);

  Color := GetRGB(g_ClientConfig.btHintWindowbackgroundColor);
  GameCanvas.FillRectAlpha(SourceRect, Color, g_ClientConfig.btHintWindowbackgroundAlpha); // $00005E5E   $00006C6C  GetRGB(18)
  SourceRect := HintRect;
  if FShowBackground and g_ClientConfig.boShowHintWindowFrame then begin
    nWidth := SourceRect.Right - SourceRect.Left;
    nHeight := SourceRect.Bottom - SourceRect.Top;
    d := g_WNewopUIImages.Images[44]; // 横
    if d <> nil then begin
      GameCanvas.StretchDraw(Bounds(HintRect.Left + 2, HintRect.Top + 2, nWidth, d.Height), d);
      GameCanvas.StretchDraw(Bounds(HintRect.Left + 2, HintRect.Top + 2 + (nHeight - d.Height) + 1, nWidth, d.Height), d);
    end;
    d := g_WNewopUIImages.Images[45]; // 竖
    if d <> nil then begin
      GameCanvas.StretchDraw(Bounds(HintRect.Left + 2, HintRect.Top + 2, d.Width, nHeight), d);
      GameCanvas.StretchDraw(Bounds(HintRect.Left + 2 + (nWidth - d.Width) + 1, HintRect.Top + 2, d.Width, nHeight), d);
    end;

    d := g_WNewopUIImages.Images[42]; // 左上角
    if d <> nil then
      GameCanvas.Draw(HintRect.Left + 2, HintRect.Top + 2, d);

    d := g_WNewopUIImages.Images[40]; // 右上角
    if d <> nil then
      GameCanvas.Draw(HintRect.Left + 2 + (nWidth - d.Width), HintRect.Top + 2, d);

    d := g_WNewopUIImages.Images[43]; // 左下角
    if d <> nil then
      GameCanvas.Draw(HintRect.Left + 2, HintRect.Top + 2 + (nHeight - d.Height), d);

    d := g_WNewopUIImages.Images[41]; // 右下角
    if d <> nil then
      GameCanvas.Draw(HintRect.Left + 2 + (nWidth - d.Width), HintRect.Top + 2 + (nHeight - d.Height), d);
  end;
end;

procedure THintWindow.ShowColor(X, Y:Integer; Msg:string; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
var
  nPos:Integer;
  sMsg:string;
  sColor:string;
  HintLines:THintLines;
  Color:TColor;
begin
  if Msg = '' then Exit;

  HintWidth := 0;
  HintHeight := 0;
  while True do begin
    if Msg = '' then Break;
    Msg := GetValidStr3_Ex(Msg, sMsg, '\');
    if sMsg <> '' then begin
      Color := clWhite;
      nPos := Pos('/', sMsg);
      if nPos > 0 then begin
        sColor := Copy(sMsg, 1, nPos - 1);
        sMsg := Copy(sMsg, nPos + 1, Length(sMsg) - nPos);
        Color := GetRGB(StrToIntDef(sColor, 255));
      end;
      HintLines := THintLines.Create;

      // 默认为加描边 chongchong 2017-04-16
      HintLines.Add(sMsg, Color, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True));
      Add(HintLines);
      HintWidth := Max(HintLines.Width, HintWidth);
      HintHeight := HintHeight + HintLines.ItemHeight;
    end;
  end;

  HintHeight := HintHeight + g_ClientConfig.HintWindowBorderWidth.Top + g_ClientConfig.HintWindowBorderWidth.Bottom - 5;

  HintX := X;
  HintY := Y;

  HintUp := DrawUp;

  HintWidth := HintWidth + g_ClientConfig.HintWindowBorderWidth.Left + g_ClientConfig.HintWindowBorderWidth.Right - 6; // 24;

  if HintUp then HintY := HintY - HintHeight;
  if DrawLeft then HintX := HintX - HintWidth;
  if HintX < 0 then HintX := 0;
  if HintX + HintWidth > SCREENWIDTH then HintX := SCREENWIDTH - HintWidth;
  if HintY < 0 then HintY := 0;
  if HintY + HintHeight > SCREENHEIGHT then HintY := SCREENHEIGHT - HintHeight;

  HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));

  FShowBackground := ShowBackground;
end;

procedure THintWindow.Show(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
var
  sMsg:string;
  HintLines:THintLines;
begin
  if Msg = '' then Exit;

  HintWidth := 0;
  HintHeight := 0;
  while True do begin
    if Msg = '' then Break;
    Msg := GetValidStr3_Ex(Msg, sMsg, '\');
    if sMsg <> '' then begin
      HintLines := THintLines.Create;
      HintLines.Add(sMsg, Color, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
      Add(HintLines);
      HintWidth := Max(HintLines.Width, HintWidth);
      HintHeight := HintHeight + HintLines.ItemHeight;
    end;
  end;

  HintHeight := HintHeight + g_ClientConfig.HintWindowBorderWidth.Top + g_ClientConfig.HintWindowBorderWidth.Bottom - 5;

  HintX := X;
  HintY := Y;

  HintUp := DrawUp;

  HintWidth := HintWidth + g_ClientConfig.HintWindowBorderWidth.Left + g_ClientConfig.HintWindowBorderWidth.Right - 6; // 24;

  if HintUp then HintY := HintY - HintHeight;
  if DrawLeft then HintX := HintX - HintWidth;
  if HintX < 0 then HintX := 0;
  if HintX + HintWidth > SCREENWIDTH then HintX := SCREENWIDTH - HintWidth;
  if HintY < 0 then HintY := 0;
  if HintY + HintHeight > SCREENHEIGHT then HintY := SCREENHEIGHT - HintHeight;

  HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));

  FShowBackground := ShowBackground;
end;

procedure THintWindow.Show(X, Y:Integer; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
var
  I:Integer;
  HintLines:THintLines;
begin
  HintX := X;
  HintY := Y;
  HintWidth := 0;
  HintHeight := 0;
  HintUp := DrawUp;

  for I := 0 to Count - 1 do begin
    HintLines := THintLines(Items[I]);
    HintWidth := Max(HintLines.Width, HintWidth);
    HintHeight := HintHeight + HintLines.ItemHeight;

    if HintLines.MinWidth > 0 then begin
      HintWidth := Max(HintLines.MinWidth, HintWidth);
    end;
  end;

  HintWidth := HintWidth + g_ClientConfig.HintWindowBorderWidth.Left + g_ClientConfig.HintWindowBorderWidth.Right - 6; // Max(HintWidth + 24, 120);
  HintHeight := HintHeight + g_ClientConfig.HintWindowBorderWidth.Top + g_ClientConfig.HintWindowBorderWidth.Bottom - 5;

  if HintUp then HintY := HintY - HintHeight;
  if DrawLeft then HintX := HintX - HintWidth;
  if HintX < 0 then HintX := 0;
  if HintX + HintWidth > SCREENWIDTH then HintX := SCREENWIDTH - HintWidth;
  if HintY < 0 then HintY := 0;
  if HintY + HintHeight > SCREENHEIGHT then HintY := SCREENHEIGHT - HintHeight;

  HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));

  FShowBackground := ShowBackground;
end;

procedure THintWindow.SetHintX(Value:Integer);
begin
  if HintX <> Value then begin
    HintX := Value;
    if HintX < 0 then HintX := 0;
    HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));
  end;
end;

procedure THintWindow.SetHintY(Value:Integer);
begin
  if HintY <> Value then begin
    HintY := Value;
    if HintY < 0 then HintY := 0;
    HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));
  end;
end;

procedure THintWindow.SetHintXExt(Value:Integer);
begin
  if HintX <> Value then begin
    HintX := Value;
    HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));
  end;
end;

procedure THintWindow.SetHintYExt(Value:Integer);
begin
  if HintY <> Value then begin
    HintY := Value;
    HintRect := Bounds(HintX, HintY, Max(HintWidth, 20), Max(HintHeight, 20));
  end;
end;

procedure THintWindow.Draw();
var
  I:Integer;
  nY:Integer;
  HintLines:THintLines;
  PaintRect:TRect;
  LineRect:TRect;
  HintIconWidth:Integer;
  HaveWinHintImage:Boolean;
begin
  DrawBackground();
  PaintRect := HintRect;

  // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
  if PaintRect.Left > SCREENWIDTH then Exit;
  if PaintRect.Top > SCREENHEIGHT then Exit;

  HintIconWidth := 0;
  for I := 0 to Count - 1 do begin
    HintLines := THintLines(Items[I]);

    if HintLines.IsItemIconText then
      HintIconWidth := Max(HintIconWidth, HintLines.Width);
  end;

  HaveWinHintImage := False;
  nY := PaintRect.Top;
  for I := 0 to Count - 1 do begin
    HintLines := THintLines(Items[I]);

    case HintLines.Alignment of
      taLeftJustify:begin
          LineRect := Bounds(PaintRect.Left, nY, PaintRect.Right - PaintRect.Left, HintLines.ItemHeight);
        end;
      taRightJustify:begin
          if HintLines.IsItemIconText then begin
            LineRect := Bounds(PaintRect.Left, nY, HintIconWidth, HintLines.ItemHeight);
            OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintIconWidth - g_ClientConfig.HintWindowBorderWidth.Right - 6), 0);
          end
          else begin
            LineRect := Bounds(PaintRect.Left, nY, HintLines.Width, HintLines.ItemHeight);
            OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintLines.Width - g_ClientConfig.HintWindowBorderWidth.Right - 6), 0);
          end
        end;
      taCenter:begin
          if HintLines.IsItemIconText then begin
            LineRect := Bounds(PaintRect.Left, nY, HintIconWidth, HintLines.ItemHeight);
            OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintIconWidth - g_ClientConfig.HintWindowBorderWidth.Right - 6) div 2, 0);
          end
          else begin
            LineRect := Bounds(PaintRect.Left, nY, HintLines.Width, HintLines.ItemHeight);
            OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintLines.Width - g_ClientConfig.HintWindowBorderWidth.Right - 6) div 2, 0);
          end;
        end;
    end;

    HintLines.PaintWithoutWinHintImage(PaintRect, LineRect, HaveWinHintImage);
    nY := nY + HintLines.ItemHeight;

    // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
    if nY > SCREENHEIGHT then Break;
  end;

  // <WinNewopUI:259这种最上层绘制
  if HaveWinHintImage then begin
    nY := PaintRect.Top;
    for I := 0 to Count - 1 do begin
      HintLines := THintLines(Items[I]);

      case HintLines.Alignment of
        taLeftJustify:begin
            LineRect := Bounds(PaintRect.Left, nY, PaintRect.Right - PaintRect.Left, HintLines.ItemHeight);
          end;
        taRightJustify:begin
            if HintLines.IsItemIconText then begin
              LineRect := Bounds(PaintRect.Left, nY, HintIconWidth, HintLines.ItemHeight);
              OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintIconWidth - g_ClientConfig.HintWindowBorderWidth.Right - 6), 0);
            end
            else begin
              LineRect := Bounds(PaintRect.Left, nY, HintLines.Width, HintLines.ItemHeight);
              OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintLines.Width - g_ClientConfig.HintWindowBorderWidth.Right - 6), 0);
            end
          end;
        taCenter:begin
            if HintLines.IsItemIconText then begin
              LineRect := Bounds(PaintRect.Left, nY, HintIconWidth, HintLines.ItemHeight);
              OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintIconWidth - g_ClientConfig.HintWindowBorderWidth.Right - 6) div 2, 0);
            end
            else begin
              LineRect := Bounds(PaintRect.Left, nY, HintLines.Width, HintLines.ItemHeight);
              OffsetRect(LineRect, (PaintRect.Right - PaintRect.Left - HintLines.Width - g_ClientConfig.HintWindowBorderWidth.Right - 6) div 2, 0);
            end;
          end;
      end;

      HintLines.PaintWinHintImage(PaintRect, LineRect);
      nY := nY + HintLines.ItemHeight;

      // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
      if nY > SCREENHEIGHT then Break;
    end;
  end;
end;
{-------------------------------------------------------------------------------}

constructor THintWindows.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
  FWindows := TList.Create;
end;

destructor THintWindows.Destroy;
var
  I:Integer;
begin
  for I := 0 to FWindows.Count - 1 do begin
    THintWindow(FWindows.Items[I]).Free;
  end;
  FWindows.Free;
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

procedure THintWindows.Initialize;
var
  I:Integer;
begin
  EnterCriticalSection(CriticalSection);
  try
    for I := 0 to FWindows.Count - 1 do begin
      THintWindow(FWindows.Items[I]).Free;
    end;
    FWindows.Clear;
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.Finalize;
var
  I:Integer;
begin
  EnterCriticalSection(CriticalSection);
  try
    for I := 0 to FWindows.Count - 1 do begin
      THintWindow(FWindows.Items[I]).Free;
    end;
    FWindows.Clear;
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.ShowColor(X, Y:Integer; Msg:string; DrawUp:Boolean = False; DrawLeft:Boolean = False; ShowBackground:Boolean = True);
var
  I:Integer;
  HintWindow:THintWindow;
begin
  EnterCriticalSection(CriticalSection);
  try
    for I := 0 to FWindows.Count - 1 do begin
      THintWindow(FWindows.Items[I]).Visible := False;
    end;

    HintWindow := THintWindow.Create;
    HintWindow.ShowColor(X, Y, Msg, DrawUp, DrawLeft, ShowBackground);
    FWindows.Add(HintWindow);
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.Show(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
var
  I:Integer;
  HintWindow:THintWindow;
begin
  EnterCriticalSection(CriticalSection);
  try
    for I := 0 to FWindows.Count - 1 do begin
      THintWindow(FWindows.Items[I]).Visible := False;
    end;

    HintWindow := THintWindow.Create;
    HintWindow.Show(X, Y, Msg, Color, DrawUp, DrawLeft, ShowBackground);
    FWindows.Add(HintWindow);
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.Draw();
var
  I:Integer;
  HintWindow:THintWindow;
begin
  EnterCriticalSection(CriticalSection);
  try
    if FWindows.Count > 0 then begin
      HintWindow := THintWindow(FWindows.Items[FWindows.Count - 1]);
      if HintWindow.Visible then begin // 检测是不是有新的 悬浮显示 如果有把不显示的删除 否则会闪烁
        for I := FWindows.Count - 1 downto 0 do begin
          HintWindow := THintWindow(FWindows.Items[I]);
          if not HintWindow.Visible then begin
            FWindows.Delete(I);
            HintWindow.Free;
          end;
        end;
      end;

      for I := 0 to FWindows.Count - 1 do begin
        HintWindow := THintWindow(FWindows.Items[I]);
        HintWindow.Draw;
      end;

      for I := FWindows.Count - 1 downto 0 do begin
        HintWindow := THintWindow(FWindows.Items[I]);
        if not HintWindow.Visible then begin
          FWindows.Delete(I);
          HintWindow.Free;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.Clear;
var
  I:Integer;
  HintWindow:THintWindow;
begin
  EnterCriticalSection(CriticalSection);
  try
    for I := FWindows.Count - 1 downto 0 do begin
      HintWindow := THintWindow(FWindows.Items[I]);
      HintWindow.Free;
    end;
    g_LastHintMakeIndex := -1;
    FWindows.Clear;
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.Add(HintWindow:THintWindow);
begin
  EnterCriticalSection(CriticalSection);
  try
    FWindows.Add(HintWindow);
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

function THintWindows.GetCount:Integer;
begin
  EnterCriticalSection(CriticalSection);
  try
    Result := FWindows.Count;
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

function THintWindows.GetItems(Index:Integer):THintWindow;
begin
  EnterCriticalSection(CriticalSection);
  try
    Result := FWindows.Items[Index];
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

procedure THintWindows.UpDate;
// var
// I: Integer;
begin
  {EnterCriticalSection(CriticalSection);
  try
    for I := FWindows.Count - 1 downto 0 do begin
      if not THintWindow(FWindows.Items[I]).Visible then begin
        THintWindow(FWindows.Items[I]).Free;
        FWindows.Delete(I);
      end;
    end;
  finally
    LeaveCriticalSection(CriticalSection);
  end;}
end;

{-------------------------------------------------------------------------------}

constructor TDrawSysMsg.Create;
begin
  m_MsgList := TGStringList.Create;
  m_nX := 30;
  m_nY := 40;
  m_FColor := GetRGB(2);
  m_BColor := GetRGB(0);

  m_boDownToUP := False;
  m_boDrawBottom := False;
end;

destructor TDrawSysMsg.Destroy;
var
  I:Integer;
begin
  for I := 0 to m_MsgList.Count - 1 do begin
    Dispose(pTSysMsg(m_MsgList.Objects[I]));
  end;
  m_MsgList.Free;
end;

procedure TDrawSysMsg.Add(sMsg:string; FColor, BColor:Byte);
var
  SysMsg:pTSysMsg;
begin
  m_MsgList.Lock;
  try
    if m_MsgList.Count >= 10 then begin
      Dispose(pTSysMsg(m_MsgList.Objects[0]));
      m_MsgList.Delete(0);
    end;
    New(SysMsg);
    SysMsg.Time := MyGetTickCount;
    SysMsg.FColor := GetRGB(FColor);
    SysMsg.BColor := GetRGB(BColor);
    m_MsgList.AddObject(sMsg, TObject(SysMsg));
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawSysMsg.Draw();
var
  I, sx, sy:Integer;
  SysMsg:pTSysMsg;
begin
  if (g_MySelf = nil) then Exit;
  m_MsgList.Lock;
  try
    if m_MsgList.Count > 0 then begin
      sx := m_nX;
      sy := m_nY;
      for I := 0 to m_MsgList.Count - 1 do begin
        SysMsg := pTSysMsg(m_MsgList.Objects[I]);
        BoldTextOut(sx, sy, m_MsgList[I], SysMsg.FColor, SysMsg.BColor);

        if m_boDownToUP then
          Dec(sy, 16)
        else
          Inc(sy, 16);
      end;
      if MyGetTickCount - pTSysMsg(m_MsgList.Objects[0]).Time >= 3000 then begin
        Dispose(pTSysMsg(m_MsgList.Objects[0]));
        m_MsgList.Delete(0);
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawSysMsg.Clear;
begin
  m_MsgList.Lock;
  try
    m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;

{-------------------------------------------------------------------------------}

constructor TDrawSysMsgEx.Create;
begin
  m_MsgList := TGStringList.Create;
  m_nX := 30;
  m_nY := 40;
  m_FColor := GetRGB(2);
  m_BColor := GetRGB(0);

  m_boWantDelete := False;
  m_boWantDeleteTick := MyGetTickCount;
  m_nOffsetY := 0;
  m_dwOffsetTick := MyGetTickCount;
  m_boDownToUP := False;
end;

destructor TDrawSysMsgEx.Destroy;
var
  I:Integer;
begin
  for I := 0 to m_MsgList.Count - 1 do begin
    Dispose(pTSysMsg(m_MsgList.Objects[I]));
  end;
  m_MsgList.Free;
end;

procedure TDrawSysMsgEx.Add(sMsg:string; FColor, BColor:Byte);
var
  SysMsg:pTSysMsg;
begin
  m_MsgList.Lock;
  try
    if m_MsgList.Count >= 10 then begin
      Dispose(pTSysMsg(m_MsgList.Objects[0]));
      m_MsgList.Delete(0);
    end;
    New(SysMsg);
    SysMsg.Time := MyGetTickCount;
    SysMsg.FColor := GetRGB(FColor);
    SysMsg.BColor := GetRGB(BColor);
    m_MsgList.AddObject(sMsg, TObject(SysMsg));
  finally
    m_MsgList.UnLock;
  end;
end;

(*
procedure TDrawSysMsgEx.Draw();
var
  I, sx, sy, OffsetY: Integer;
  SysMsg: pTSysMsg;
  TempTime: LongWord;
  Alpha: Byte;

  nShowTime, nDeleteTime, nMinShowTime, OffsetY_Step: Integer;
  nShowCount: Integer;
begin
  if (g_MySelf = nil) then Exit;
  m_MsgList.Lock;
  try
    if m_MsgList.Count > 0 then
    begin
      if m_MsgList.Count > 10 then
      begin
        nShowTime := 500;            // 显示多少时间隐藏
        nMinShowTime := 100;          // 前一条删除后，本条最小显示多长时间
        nDeleteTime := 900;          // 过多少时间删除
      end
      else if m_MsgList.Count > 5 then
      begin
        nShowTime := 1000;            // 显示多少时间隐藏
        nMinShowTime := 100;          // 前一条删除后，本条最小显示多长时间
        nDeleteTime := 1400;          // 过多少时间删除
      end
      else
      begin
        nShowTime := 2000;            // 显示多少时间隐藏
        nMinShowTime := 300;          // 前一条删除后，本条最小显示多长时间
        nDeleteTime := 2800;          // 过多少时间删除
      end;

      OffsetY_Step := (nDeleteTime - nShowTime) div 16;

      if m_MsgList.Count > 5 then
      begin
        TempTime := MyGetTickCount - pTSysMsg(m_MsgList.Objects[0]).Time;
        if TempTime < nShowTime - nMinShowTime then
        begin
          pTSysMsg(m_MsgList.Objects[0]).Time := MyGetTickCount - nShowTime + nMinShowTime;
        end;
      end;

      OffsetY := 0;
      TempTime := MyGetTickCount - pTSysMsg(m_MsgList.Objects[0]).Time;
      if TempTime >= nDeleteTime then
      begin
        Dispose(pTSysMsg(m_MsgList.Objects[0]));
        m_MsgList.Delete(0);

        if m_MsgList.Count > 0 then
        begin
          TempTime := MyGetTickCount - pTSysMsg(m_MsgList.Objects[0]).Time;
          if TempTime >= nShowTime then
          begin
            pTSysMsg(m_MsgList.Objects[0]).Time := MyGetTickCount - nShowTime + nMinShowTime;
          end;
        end;
      end
      else if TempTime >= nShowTime then
      begin
        OffsetY := (TempTime - nShowTime) div OffsetY_Step;
      end;

      nShowCount := 0;
      sx := m_nX;
      sy := m_nY + OffsetY;
      for I := Max(m_MsgList.Count - 5, 0) to m_MsgList.Count - 1 do
      begin
        SysMsg := pTSysMsg(m_MsgList.Objects[I]);

        if I = 0 then
          Alpha := 255 - OffsetY * 15
        else
          Alpha := 255;

        BoldTextOut(sx, sy, m_MsgList[I], SysMsg.FColor, SysMsg.BColor, Alpha);

        Dec(sy, 16); // 经验显示要改成向上滚动 chongchong 2018-05-28

        Inc(nShowCount);
        if nShowCount > 5 then Break;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;
*)

procedure TDrawSysMsgEx.Draw();
var
  I, sx, sy:Integer;
  SysMsg:pTSysMsg;
  TempTime:LongWord;
  Alpha:Integer;

  nShowTime, nStepTime:Cardinal; //HZQ 20230524 Integer;
  nShowCount:Integer;
begin
  if (g_MySelf = nil) then Exit;
  m_MsgList.Lock;
  try
    if m_MsgList.Count > 0 then begin
      if m_MsgList.Count > 8 then begin
        nShowTime := 700; // 显示多少时间隐藏
        nStepTime := 20; // 多长时间渐移一个像素
        //StayTime := 0;
      end else if m_MsgList.Count > 5 then begin
        nShowTime := 1500; // 显示多少时间隐藏
        nStepTime := 30; // 多长时间渐移一个像素
        //StayTime := 200;
      end else begin
        nShowTime := 2000; // 显示多少时间隐藏
        nStepTime := 50; // 多长时间渐移一个像素
        //StayTime := 500;
      end;

      SysMsg := pTSysMsg(m_MsgList.Objects[0]);
      if m_MsgList.Count > 5 then begin
        if not m_boWantDelete then begin
          m_boWantDelete := True;
          m_boWantDeleteTick := MyGetTickCount;
          m_nOffsetY := 0;
          m_dwOffsetTick := MyGetTickCount;
        end;
      end;

      if m_boWantDelete then begin
        if (m_nOffsetY >= 16) then begin
          Dispose(SysMsg);
          m_MsgList.Delete(0);

          m_boWantDelete := False;
          m_nOffsetY := 0;
          m_dwOffsetTick := MyGetTickCount;

          if m_MsgList.Count > 0 then begin
            SysMsg := pTSysMsg(m_MsgList.Objects[0]);
            TempTime := MyGetTickCount - SysMsg.Time;
            if TempTime >= nShowTime then begin
              //SysMsg.Time := MyGetTickCount - nShowTime + nMinShowTime;
              m_boWantDelete := True;
              m_boWantDeleteTick := MyGetTickCount;
              m_nOffsetY := 0;
              m_dwOffsetTick := MyGetTickCount;
            end;
          end;
        end else begin
          //if MyGetTickCount - m_boWantDeleteTick >= StayTime then
          begin
            if MyGetTickCount - m_dwOffsetTick >= nStepTime then begin
              Inc(m_nOffsetY);
              m_dwOffsetTick := MyGetTickCount;
            end;
          end;
        end;
      end else begin
        SysMsg := pTSysMsg(m_MsgList.Objects[0]);
        TempTime := MyGetTickCount - SysMsg.Time;
        if TempTime >= nShowTime then begin
          m_boWantDelete := True;
          m_boWantDeleteTick := MyGetTickCount;
          m_nOffsetY := 0;
          m_dwOffsetTick := MyGetTickCount;
        end;
      end;

      nShowCount := 0;
      sx := m_nX;

      if m_boDownToUP then
        sy := m_nY + m_nOffsetY
      else
        sy := m_nY - m_nOffsetY;

      for I := 0 to m_MsgList.Count - 1 do begin
        SysMsg := pTSysMsg(m_MsgList.Objects[I]);

        if (I = 0) and (m_nOffsetY > 0) then
          Alpha := 180 - m_nOffsetY * 15
        else
          Alpha := 255;

        if Alpha < 0 then Alpha := 0;

        BoldTextOut(sx, sy, m_MsgList[I], SysMsg.FColor, SysMsg.BColor, Alpha);

        if m_boDownToUP then
          Dec(sy, 16) // 经验显示要改成向上滚动 chongchong 2018-05-28
        else
          Inc(sy, 16);

        Inc(nShowCount);
        if nShowCount > 5 then Break;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawSysMsgEx.Clear;
begin
  m_MsgList.Lock;
  try
    m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;

{-------------------------------------------------------------------------------}

constructor TDrawMoveHintMsg.Create;
begin
  m_MsgList := TGStringList.Create;
  m_nX := 30;
  m_nY := 40;
  m_FColor := GetRGB(2);
  m_BColor := GetRGB(0);
end;

destructor TDrawMoveHintMsg.Destroy;
var
  I:Integer;
begin
  for I := 0 to m_MsgList.Count - 1 do begin
    Dispose(pTSysMsg(m_MsgList.Objects[I]));
  end;
  m_MsgList.Free;
end;

procedure TDrawMoveHintMsg.Add(sMsg:string; FColor, BColor:Byte);
var
  SysMsg:pTSysMsg;
begin
  m_MsgList.Lock;
  try
    if m_MsgList.Count >= 10 then begin
      Dispose(pTSysMsg(m_MsgList.Objects[0]));
      m_MsgList.Delete(0);
    end;
    New(SysMsg);
    SysMsg.Time := MyGetTickCount;
    SysMsg.FColor := GetRGB(FColor);
    SysMsg.BColor := GetRGB(BColor);
    m_MsgList.AddObject(sMsg, TObject(SysMsg));
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawMoveHintMsg.Draw();
var
  I, sx, sy:Integer;
  SysMsg:pTSysMsg;
begin
  if (g_MySelf = nil) then Exit;
  m_MsgList.Lock;
  try
    if m_MsgList.Count > 0 then begin
      sx := m_nX;
      sy := m_nY;
      for I := 0 to m_MsgList.Count - 1 do begin
        SysMsg := pTSysMsg(m_MsgList.Objects[I]);
        BoldTextOut(sx, sy, m_MsgList[I], SysMsg.FColor, SysMsg.BColor);
        Inc(sy, 16);
      end;
      if MyGetTickCount - pTSysMsg(m_MsgList.Objects[0]).Time >= 3000 then begin
        Dispose(pTSysMsg(m_MsgList.Objects[0]));
        m_MsgList.Delete(0);
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawMoveHintMsg.Clear;
begin
  m_MsgList.Lock;
  try
    m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;
{-------------------------------------------------------------------------------}

constructor TDrawScreenCenterMsg.Create;
begin
  inherited Create;
  m_TextList := TGStringList.Create;
  m_FColor := 255;
  m_BColor := 0;
end;

destructor TDrawScreenCenterMsg.Destroy;
begin
  Clear(False);
  m_TextList.Free;
  inherited Destroy;
end;

procedure TDrawScreenCenterMsg.Clear(Lock:Boolean);
var
  I:Integer;
  TokenLine:TStringLineEx;
begin
  if Lock then
    m_TextList.Lock;

  for I := 0 to m_TextList.Count - 1 do begin
    TokenLine := TStringLineEx(m_TextList.Objects[I]);
    TokenLine.Free;
  end;
  m_TextList.Clear;

  if Lock then
    m_TextList.UnLock;
end;

procedure TDrawScreenCenterMsg.Add(sMsg:string; FColor, BColor:Byte; nTime:Integer);
var
  wText:WideString;
  sText:string;
  //FontSize: Integer;
  I:Integer;
  // FontCharset: TFontCharset;
  HGEFont:THGEFont;
  TokenLines:TList;
  TokenLine:TStringLineEx;
begin
  // DScreen.AddChatBoardString('TDrawScreenCenterMsg.Add '+sMsg, GetRGB(255), GetRGB(253));
  m_TextList.Lock;
  try
    m_dwShowTime := MyGetTickCount + Cardinal(nTime) * 1000;
    m_FColor := FColor;
    m_BColor := BColor;

    Clear(False);

    HGEFont := TextureFonts.FindFont(g_sCurFontName, 20);
    if HGEFont <> nil then begin
      //nTextWidth := HGEFont.TextWidth(sMsg);
      wText := sMsg;
      {
      if nTextWidth > SCREENWIDTH then
      begin
        sText := '';
        I := 1;
        while True do
        begin
      // for I := 1 to Length(wText) do begin
          if HGEFont.TextWidth(sText + wText[I]) <= SCREENWIDTH then
          begin
            sText := sText + wText[I];
          end
          else
          begin
            m_TextList.Add(sText);
            sText := '';
            Continue;
          end;
          Inc(I);
          if I > Length(wText) then break;
        end;
        if sText <> '' then m_TextList.Add(sText);
      end
      else
      begin
        m_TextList.Add(sMsg);
      end;
      }
      TokenLines := TList.Create;
      try
        GetTextListEx(HGEFont, wText, GetRGB(m_FColor), GetRGB(m_BColor), TokenLines, clNone, SCREENWIDTH - 20);

        for I := 0 to TokenLines.Count - 1 do begin
          TokenLine := TokenLines.Items[I];
          sText := GetStrinLineExText(TokenLine);
          m_TextList.AddObject(sText, TokenLine);
        end;
      finally
        TokenLines.Free;
      end;
    end;
  finally
    m_TextList.UnLock;
  end;
end;

procedure TDrawScreenCenterMsg.Draw();
var
  I, J, nX, nY, nTextHeight:Integer;
  HGEFont:THGEFont;

  TokenLine:TStringLineEx;
  Token:PStringToken;
  S:string;
begin
  m_TextList.Lock;
  try
    if m_TextList.Count > 0 then begin
      if MyGetTickCount < m_dwShowTime then begin
        HGEFont := TextureFonts.FindFont(g_sCurFontName, 20);
        if HGEFont <> nil then begin
          nTextHeight := HGEFont.TextHeight('Pp');

          nY := (SCREENHEIGHT - m_TextList.Count * nTextHeight) div 2;

          for I := 0 to m_TextList.Count - 1 do begin
            {
            BoldTextOut(HGEFont, (SCREENWIDTH - HGEFont.TextWidth(m_TextList.Strings[I])) div 2, nY,
              m_TextList.Strings[I], GetRGB(m_FColor), GetRGB(m_BColor));
            }

            TokenLine := TStringLineEx(m_TextList.Objects[I]);
            S := GetStrinLineExText(TokenLine);
            nX := (SCREENWIDTH - HGEFont.TextWidth(S)) div 2;
            for J := 0 to TokenLine.Count - 1 do begin
              Token := TokenLine.Tokens[J];
              BoldTextOut(HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor);
              nX := nX + HGEFont.TextWidth(Token.Text);
            end;

            Inc(nY, nTextHeight);
          end;
        end;
      end
      else begin
        Clear(False);
      end;
    end;
  finally
    m_TextList.UnLock;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDrawDelayMsg.Create;
begin
  m_dwDrawFrameCount := MyGetTickCount;
  m_MsgList := TGList.Create;
  m_MoveDraw := False;
  m_MoveOffset := 0;
end;

destructor TDrawDelayMsg.Destroy;
begin
  Clear;
  m_MsgList.Free;
  inherited Destroy;
end;

procedure TDrawDelayMsg.Clear;
var
  I:Integer;
  DelayMsg:pTDelayMsg;
begin
  m_MsgList.Lock;
  try
    for I := 0 to m_MsgList.Count - 1 do begin
      DelayMsg := pTDelayMsg(m_MsgList.Items[I]);
      DelayMsg.Tokens.Free;
      Dispose(DelayMsg);
    end;
    m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawDelayMsg.Delete(RecogId:Int64);
var
  I:Integer;
  DelayMsg:pTDelayMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      DelayMsg := pTDelayMsg(m_MsgList.Items[I]);
      if DelayMsg.RecogId = RecogId then begin
        m_MsgList.Delete(I);
        DelayMsg.Tokens.Free;
        Dispose(DelayMsg);
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawDelayMsg.Add(RecogId:Int64; sMsg:string; nTime:Integer; FColor, BColor:TColor; nX:Integer);
var
  I:Integer;
  DelayMsg:pTDelayMsg;
begin
  m_MsgList.Lock;
  try
    for I := 0 to m_MsgList.Count - 1 do begin
      DelayMsg := m_MsgList.Items[I];
      if DelayMsg.RecogId = RecogId then begin
        DelayMsg.RecogId := RecogId;
        DelayMsg.Msg := sMsg;
        DelayMsg.Time := MyGetTickCount + Cardinal(nTime) * 1000;
        DelayMsg.FColor := FColor;
        DelayMsg.BColor := BColor;
        DelayMsg.X := nX;
        if DelayMsg.Tokens <> nil then
          DelayMsg.Tokens.Clear;
        GetTextListEx(sMsg, FColor, BColor, DelayMsg.Tokens);

        Exit;
      end;
    end;

    New(DelayMsg);
    DelayMsg.RecogId := RecogId;
    DelayMsg.Msg := sMsg;
    DelayMsg.Time := MyGetTickCount + Cardinal(nTime) * 1000;
    DelayMsg.FColor := FColor;
    DelayMsg.BColor := BColor;
    DelayMsg.X := nX;
    DelayMsg.Tokens := TStringLineEx.Create;
    GetTextListEx(sMsg, FColor, BColor, DelayMsg.Tokens);
    m_MsgList.Add(DelayMsg);

    m_MoveDraw := True;
    m_MoveOffset := 0;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawDelayMsg.Draw();

  function GetTimeStr(nTime:Integer):string;
  var
    nMin:Integer;
    nSec:Integer;
  begin
    nSec := nTime div 1000;
    nMin := nSec div 60;
    if nMin > 0 then begin
      nSec := nSec mod 60;
      Result := IntToStr(nMin) + '分' + IntToStr(nSec) + '秒';
    end
    else begin
      Result := IntToStr(nSec) + '秒';
    end;
  end;
const
  SHOW_LINE_COUNT = 5;
var
  I, J, Y, StartIndex, EndIndex, OffsetX:Integer;
  sMsg:string;
  DelayMsg:pTDelayMsg;
  // HGEFont: THGEFont;
  Token:PStringToken;
  CurTick:DWORD;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      DelayMsg := pTDelayMsg(m_MsgList.Items[I]);
      if MyGetTickCount > DelayMsg.Time then begin
        m_MsgList.Delete(I);
        DelayMsg.Tokens.Free;
        Dispose(DelayMsg);
      end;
    end;

    // SendCenterMsg只显示几行
    if m_MsgList.Count <= SHOW_LINE_COUNT then m_MoveDraw := False;
  finally
    m_MsgList.UnLock;
  end;

  Y := SCREENHEIGHT - 230;
  if m_MoveDraw then begin
    Inc(m_MoveOffset);
    if m_MoveOffset >= 14 then begin
      m_MoveOffset := 0;
      m_MoveDraw := False;
    end;
    Y := Y + m_MoveOffset;
  end;

  // HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]); //fsBold  g_sCurFontName
  if CurrentFont <> nil then begin
    m_MsgList.Lock;
    try
      if m_MoveDraw then
        EndIndex := m_MsgList.Count - 2
      else
        EndIndex := m_MsgList.Count - 1;
      StartIndex := Max(EndIndex - SHOW_LINE_COUNT - 1, 0);

      for I := StartIndex to EndIndex do begin
        DelayMsg := pTDelayMsg(m_MsgList.Items[I]);

        //sMsg := DelayMsg.Msg;
        sMsg := GetStrinLineExText(DelayMsg.Tokens);

        CurTick := MyGetTickCount;
        if Pos('%d', sMsg) > 0 then
          sMsg := AnsiReplaceText(sMsg, '%d', GetTimeStr(DelayMsg.Time - CurTick));
        if Pos('%s', sMsg) > 0 then
          sMsg := AnsiReplaceText(sMsg, '%s', GetTimeStr(DelayMsg.Time - CurTick));
        // sMsg := Format(sMsg, [GetTimeStr(DelayMsg.Time - MyGetTickCount)]);

        if DelayMsg.X <= 0 then begin
          DelayMsg.X := (SCREENWIDTH - CurrentFont.TextWidth(sMsg)) div 2;
          if DelayMsg.X <= 0 then DelayMsg.X := 1;
        end;

        {
        if m_MoveDraw and (I = StartIndex) then
          BoldTextOut(CurrentFont, DelayMsg.X, Y, sMsg, DelayMsg.FColor, DelayMsg.BColor, 255 - m_MoveOffset * 18)
        else
          BoldTextOut(CurrentFont, DelayMsg.X, Y, sMsg, DelayMsg.FColor, DelayMsg.BColor);
        }
        OffsetX := 0;
        for J := 0 to DelayMsg.Tokens.Count - 1 do begin
          Token := DelayMsg.Tokens.Tokens[J];
          sMsg := Token.Text;

          if Pos('%d', sMsg) > 0 then
            sMsg := AnsiReplaceText(sMsg, '%d', GetTimeStr(DelayMsg.Time - CurTick));
          if Pos('%s', sMsg) > 0 then
            sMsg := AnsiReplaceText(sMsg, '%s', GetTimeStr(DelayMsg.Time - CurTick));

          if m_MoveDraw and (I = StartIndex) then
            BoldTextOut(CurrentFont, DelayMsg.X + OffsetX, Y, sMsg, Token.FColor, Token.BColor, 255 - m_MoveOffset * 18)
          else
            BoldTextOut(CurrentFont, DelayMsg.X + OffsetX, Y, sMsg, Token.FColor, Token.BColor);

          OffsetX := OffsetX + CurrentFont.TextWidth(sMsg);
        end;

        Dec(Y, g_CurrentFontHeight + 2);
      end;
    finally
      m_MsgList.UnLock;
    end;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDrawScreenMoveMsg.Create;
begin
  m_MsgList := TGList.Create;
  m_nCurrMoveMsg := nil;
end;

destructor TDrawScreenMoveMsg.Destroy;
var
  I:Integer;
  MoveMsg:pTMoveMsg;
begin
  for I := 0 to m_MsgList.Count - 1 do begin
    MoveMsg := m_MsgList.Items[I];
    if MoveMsg = m_nCurrMoveMsg then
      m_nCurrMoveMsg := nil;

    MoveMsg.Text.Free;

    Dispose(MoveMsg);
  end;
  m_MsgList.Free;

  if (m_nCurrMoveMsg <> nil) then begin
    m_nCurrMoveMsg.Text.Free;
    Dispose(m_nCurrMoveMsg);
  end;
  inherited;
end;

procedure TDrawScreenMoveMsg.Initialize;
begin

end;

procedure TDrawScreenMoveMsg.Finalize;
begin

end;

function TDrawScreenMoveMsg.GetTop:Integer;
begin
  Result := DestRect.Top;
end;

function TDrawScreenMoveMsg.GetCount:Integer;
begin
  m_MsgList.Lock;
  Result := m_MsgList.Count;
  m_MsgList.UnLock;
end;

function TDrawScreenMoveMsg.MoveOver:Boolean;
begin
  m_MsgList.Lock;
  Result := (m_MsgList.Count <= 0) and (m_nCurrMoveMsg = nil);
  m_MsgList.UnLock;
end;

{ TODO -ochongchong  -c新增 : 让聊天框的文字支持自定义颜色 --- 仅单行 【2013-08-27】 }
// {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}

procedure GetTextListEx2(const Text:WideString; const FColor, BColor:TColor; TokenLine:TStringLineEx);
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
    FoundText, FoundText2:WideString;
  begin
    PWChar := PWideChar(Text);
    Inc(PWChar, Index);
    FoundIndex := Pos(WideString('}'), WideString(PWChar)); //HZQ, 修复重载到Ansi版的的Pos导致的计算错误，为毛要转换为PWideChar?
    if FoundIndex = 0 then begin
      Result := False;
      Exit;
    end;

    Len := FoundIndex + 1;

    // 取{}中间一部分 aabbcc|100:200:0
    FoundText := Copy(Text, Index + 1, FoundIndex - 1);
    FoundText2 := UpperCase(FoundText);
    FoundIndex := Pos('/SCOLOR=', FoundText2);
    if FoundIndex = 0 then begin
      Result := False;
      Exit;
    end;
    StringToken.TokenType := tt_Item;
    StringToken.Text := Copy(FoundText, 1, FoundIndex - 1);
    FoundText := Copy(FoundText, FoundIndex + Length('/SCOLOR='), MaxInt);
    StringToken.Flag := 0;
    StringToken.FColor := StrToIntDef(Trim(FoundText), FColor);
    StringToken.BColor := BColor;

    Result := True;
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
    end else begin
      if not ProcessCustomColor(OutToken, CustomTextLen) then begin
        // 如果不符合定义颜色的格式，则照常处理
        if DoProcessText then Break;
      end else begin
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

procedure TDrawScreenMoveMsg.Add(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer;
  boShowFrame:Boolean; btFrameColor:Byte; btFontSize:Byte; boFontBold:Boolean;
  nMarqueeTime:Integer);
var
  MoveMsg:pTMoveMsg;
  HGEFont:THGEFont;
  I:Integer;
  S:string;
  Token:PStringToken;
begin
  m_MsgList.Lock;
  try
    //HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);

    if boFontBold then
      HGEFont := TextureFonts.FindFont(g_sCurFontName, btFontSize, [fsBold])
    else
      HGEFont := TextureFonts.FindFont(g_sCurFontName, btFontSize, []);

    if HGEFont <> nil then begin
      New(MoveMsg);
      MoveMsg.Text := TStringLineEx.Create;

      MoveMsg.Count := nCount;
      MoveMsg.Y := nY;
      GetTextListEx2(sMsg, FColor, BColor, MoveMsg.Text);

      S := '';
      for I := 0 to MoveMsg.Text.Count - 1 do begin
        Token := MoveMsg.Text.Tokens[I];
        S := S + Token.Text;
      end;

      MoveMsg.Width := HGEFont.TextWidth(S);
      MoveMsg.Height := HGEFont.TextHeight(S);

      {
      MoveMsg.Text := sMsg;
      MoveMsg.FColor := FColor;
      MoveMsg.BColor := BColor;

      MoveMsg.Width := HGEFont.TextWidth(MoveMsg.Text);
      MoveMsg.Height := HGEFont.TextHeight(MoveMsg.Text);
      }

      MoveMsg.Orientation := mbHorizontal;
      MoveMsg.ShowFrame := boShowFrame;
      MoveMsg.FrameColor := btFrameColor;
      MoveMsg.btFontSize := btFontSize;
      MoveMsg.boFontBold := boFontBold;
      MoveMsg.nMarqueeTime := nMarqueeTime;

      DestRect.Top := nY;
      m_MsgList.Insert(0, MoveMsg);
    end;

  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawScreenMoveMsg.Update;
var
  BackgroundHeight:Integer;
begin
  m_MsgList.Lock;
  try
    if (m_nCurrMoveMsg <> nil) and (m_nCurrMoveMsg.Count <= 0) then begin
      try
        m_nCurrMoveMsg.Text.Free;
        Dispose(m_nCurrMoveMsg);
      except
        m_nCurrMoveMsg := nil;
      end;
      m_nCurrMoveMsg := nil;
    end;

    if (m_nCurrMoveMsg = nil) and (m_MsgList.Count > 0) then begin
      m_nCurrMoveMsg := m_MsgList.Items[0];
      m_MsgList.Delete(0);
      FMoveSize := 0;

      //BackgroundHeight := 30;
      BackgroundHeight := m_nCurrMoveMsg.Height + m_nCurrMoveMsg.Height div 4;

      case m_nCurrMoveMsg.Orientation of
        mbHorizontal:begin
            FOffSetX := SCREENWIDTH - 80;
            FOffSetY := m_nCurrMoveMsg.Y + (BackgroundHeight - m_nCurrMoveMsg.Height) div 2;
            FTextSize := m_nCurrMoveMsg.Width;

            DestRect.Left := 80;
            DestRect.Top := m_nCurrMoveMsg.Y; // FOffSetY;
            DestRect.Right := DestRect.Left + SCREENWIDTH - 80 * 2;
            DestRect.Bottom := DestRect.Top + BackgroundHeight; // m_nCurrMoveMsg.Height;

            SrcRect := Bounds(0, 0, 0, m_nCurrMoveMsg.Height);
          end;
        mbVertical:begin
            FOffSetX := 80;
            FOffSetY := m_nCurrMoveMsg.Y + (BackgroundHeight - m_nCurrMoveMsg.Height) div 2;
            FTextSize := m_nCurrMoveMsg.Height;

            DestRect.Left := 80;
            DestRect.Top := m_nCurrMoveMsg.Y; // FOffSetY;
            DestRect.Right := DestRect.Left + SCREENWIDTH - 80 * 2;
            DestRect.Bottom := DestRect.Top + BackgroundHeight; // m_nCurrMoveMsg.Height;
            SrcRect := Bounds(0, 0, m_nCurrMoveMsg.Width, m_nCurrMoveMsg.Height);
          end;
      end;
    end;
    if (m_nCurrMoveMsg <> nil) then begin
      // SENDMOVEMSG滚动速度快 chongchong 2013-11-13
      BackgroundHeight := m_nCurrMoveMsg.Height + m_nCurrMoveMsg.Height div 4;
      if MyGetTickCount - m_dwMoveTick > Cardinal(m_nCurrMoveMsg.nMarqueeTime) then begin // 原值 18
        m_dwMoveTick := MyGetTickCount;
        Inc(FMoveSize, 2);
        case m_nCurrMoveMsg.Orientation of
          mbHorizontal:begin
              if FMoveSize >= m_nCurrMoveMsg.Width + (DestRect.Right - DestRect.Left) then begin
                Dec(m_nCurrMoveMsg.Count);

                FMoveSize := 0;
                FOffSetX := SCREENWIDTH - 80;
                FOffSetY := m_nCurrMoveMsg.Y + (BackgroundHeight - m_nCurrMoveMsg.Height) div 2;
                FTextSize := m_nCurrMoveMsg.Width;

                DestRect.Left := 80;
                DestRect.Top := m_nCurrMoveMsg.Y; // FOffSetY;
                DestRect.Right := DestRect.Left + SCREENWIDTH - 80 * 2;
                DestRect.Bottom := DestRect.Top + BackgroundHeight; // m_nCurrMoveMsg.Height;

                SrcRect := Bounds(0, 0, 0, m_nCurrMoveMsg.Height);
              end
              else begin
                if FOffSetX > 80 then
                  Dec(FOffSetX, 2);
                if FMoveSize >= DestRect.Right - DestRect.Left then begin
                  SrcRect := Bounds(FMoveSize - (DestRect.Right - DestRect.Left), 0, Min(m_nCurrMoveMsg.Width, (DestRect.Right - DestRect.Left)), m_nCurrMoveMsg.Height);
                end
                else begin
                  SrcRect := Bounds(0, 0, Min(m_nCurrMoveMsg.Width, FMoveSize), m_nCurrMoveMsg.Height);
                end;
              end;
            end;
          mbVertical:begin
              if FMoveSize >= m_nCurrMoveMsg.Height then begin
                Dec(m_nCurrMoveMsg.Count);

                FMoveSize := 0;
                FOffSetX := 80;
                FOffSetY := m_nCurrMoveMsg.Y + (BackgroundHeight - m_nCurrMoveMsg.Height) div 2;
                FTextSize := m_nCurrMoveMsg.Height;

                DestRect.Left := 80;
                DestRect.Top := m_nCurrMoveMsg.Y; // FOffSetY;
                DestRect.Right := DestRect.Left + SCREENWIDTH - 80 * 2;
                DestRect.Bottom := DestRect.Top + BackgroundHeight; // m_nCurrMoveMsg.Height;
                SrcRect := Bounds(0, 0, m_nCurrMoveMsg.Width, m_nCurrMoveMsg.Height);
              end
              else begin

              end;
            end;
        end;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TDrawScreenMoveMsg.Draw();
var
  HGEFont:THGEFont;
  I:Integer;
  ImageInfo:TImageInfo;
  Token:PStringToken;
  nX:Integer;

  PaintRect:TRect;
  TextWidth:Integer;
begin
  m_MsgList.Lock;
  try
    if (m_nCurrMoveMsg <> nil) then begin
      //HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);
      if m_nCurrMoveMsg.boFontBold then
        HGEFont := TextureFonts.FindFont(g_sCurFontName, m_nCurrMoveMsg.btFontSize, [fsBold])
      else
        HGEFont := TextureFonts.FindFont(g_sCurFontName, m_nCurrMoveMsg.btFontSize, []);

      if HGEFont <> nil then begin
        if m_nCurrMoveMsg.ShowFrame then
          GameCanvas.FillRectAlpha(DestRect, GetRGB(m_nCurrMoveMsg.FrameColor), 100);

        nX := 0;
        PaintRect := SrcRect;
        for I := 0 to m_nCurrMoveMsg.Text.Count - 1 do begin
          Token := m_nCurrMoveMsg.Text.Tokens[I];
          ImageInfo := HGEFont.GetImageInfo(Token.Text);

          HGEFont.TextRect(nX + FOffSetX - 1, FOffSetY, PaintRect, ImageInfo.ImageIndexs, GetRGB(Token.BColor));
          HGEFont.TextRect(nX + FOffSetX + 1, FOffSetY, PaintRect, ImageInfo.ImageIndexs, GetRGB(Token.BColor));
          HGEFont.TextRect(nX + FOffSetX, FOffSetY - 1, PaintRect, ImageInfo.ImageIndexs, GetRGB(Token.BColor));
          HGEFont.TextRect(nX + FOffSetX, FOffSetY + 1, PaintRect, ImageInfo.ImageIndexs, GetRGB(Token.BColor));
          HGEFont.TextRect(nX + FOffSetX, FOffSetY, PaintRect, ImageInfo.ImageIndexs, GetRGB(Token.FColor));

          TextWidth := HGEFont.TextWidth(Token.Text);

          if PaintRect.Left <= TextWidth then begin
            nX := nX + TextWidth - PaintRect.Left;
            PaintRect.Left := 0;
            PaintRect.Right := PaintRect.Right - TextWidth - PaintRect.Left;
          end else if PaintRect.Left >= TextWidth then begin
            PaintRect.Left := PaintRect.Left - TextWidth;
            PaintRect.Right := PaintRect.Right - TextWidth;
          end;
        end;
        // HGEFont.TextRect(FOffSetX, FOffSetY, SrcRect, ImageInfo.ImageIndexs, GetRGB(m_nCurrMoveMsg.FColor));
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

constructor TScreenMoveMsgList.Create;
begin
  m_MoveObjList := TGList.Create;
end;

destructor TScreenMoveMsgList.Destroy;
var
  I:Integer;
begin
  m_MoveObjList.Lock;
  try
    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Free();
    end;
  finally
    m_MoveObjList.UnLock;
  end;
  m_MoveObjList.Free;
  inherited;
end;

procedure TScreenMoveMsgList.Add(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer;
  boShowFrame:Boolean; btFrameColor:Byte; btFontSize:Byte; boFontBold:Boolean;
  nMarqueeTime:Integer);
var
  I:Integer;
  boFind:Boolean;
  DrawScreenMoveMsg:TDrawScreenMoveMsg;
begin
  m_MoveObjList.Lock;
  try
    boFind := False;
    for I := 0 to m_MoveObjList.Count - 1 do begin
      DrawScreenMoveMsg := TDrawScreenMoveMsg(m_MoveObjList.Items[I]);
      if DrawScreenMoveMsg.Top = nY then begin
        DrawScreenMoveMsg.Add(sMsg, FColor, BColor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
        boFind := True;
        break;
      end;
    end;
    if not boFind then begin
      DrawScreenMoveMsg := TDrawScreenMoveMsg.Create;
      DrawScreenMoveMsg.Add(sMsg, FColor, BColor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
      m_MoveObjList.Add(DrawScreenMoveMsg);
    end;
  finally
    m_MoveObjList.UnLock;
  end;
end;

procedure TScreenMoveMsgList.Draw();
var
  I:Integer;
begin
  m_MoveObjList.Lock;
  try
    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Draw();
    end;
  finally
    m_MoveObjList.UnLock;
  end;
end;

procedure TScreenMoveMsgList.Update;
var
  I:Integer;
  DrawScreenMoveMsg:TDrawScreenMoveMsg;
begin
  m_MoveObjList.Lock;
  try
    for I := m_MoveObjList.Count - 1 downto 0 do begin
      DrawScreenMoveMsg := TDrawScreenMoveMsg(m_MoveObjList.Items[I]);
      if DrawScreenMoveMsg.MoveOver then begin
        m_MoveObjList.Delete(I);
        DrawScreenMoveMsg.Free;
      end;
    end;

    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Update;
    end;
  finally
    m_MoveObjList.UnLock;
  end;
end;

procedure TScreenMoveMsgList.Initialize;
var
  I:Integer;
begin
  m_MoveObjList.Lock;
  try
    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Initialize;
    end;
  finally
    m_MoveObjList.UnLock;
  end;
end;

procedure TScreenMoveMsgList.Finalize;
var
  I:Integer;
begin
  m_MoveObjList.Lock;
  try
    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Finalize;
    end;
  finally
    m_MoveObjList.UnLock;
  end;
end;

procedure TScreenMoveMsgList.Clear;
var
  I:Integer;
begin
  m_MoveObjList.Lock;
  try
    for I := 0 to m_MoveObjList.Count - 1 do begin
      TDrawScreenMoveMsg(m_MoveObjList.Items[I]).Free;
    end;
    m_MoveObjList.Clear;
  finally
    m_MoveObjList.UnLock;
  end;
end;

// ==============================================================================

constructor TScreenNewMoveMsgList.Create;
begin
  FItemList := TGList.Create;
end;

destructor TScreenNewMoveMsgList.Destroy;
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenMoveMsg(FItemList.Items[I]).Free();
    end;
  finally
    FItemList.UnLock;
  end;
  FItemList.Free;
  inherited;
end;

procedure TScreenNewMoveMsgList.Add(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
var
  I:Integer;
  boFind:Boolean;
  MoveMsg:TDrawScreenNewMoveMsg;
begin
  FItemList.Lock;
  try
    boFind := False;
    for I := 0 to FItemList.Count - 1 do begin
      MoveMsg := TDrawScreenNewMoveMsg(FItemList.Items[I]);
      if (MoveMsg.m_Y1 = nY) and (MoveMsg.m_nX = nX) then begin
        MoveMsg.Add(sMsg, FColor, BColor, FontSize, nX, nY, nCount);
        boFind := True;
        break;
      end;
    end;
    if not boFind then begin
      MoveMsg := TDrawScreenNewMoveMsg.Create;
      MoveMsg.Add(sMsg, FColor, BColor, FontSize, nX, nY, nCount);
      FItemList.Add(MoveMsg);
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewMoveMsgList.Draw();
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenNewMoveMsg(FItemList.Items[I]).Draw();
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewMoveMsgList.Update;
var
  I:Integer;
  MoveMsg:TDrawScreenNewMoveMsg;
begin
  FItemList.Lock;
  try
    for I := FItemList.Count - 1 downto 0 do begin
      MoveMsg := TDrawScreenNewMoveMsg(FItemList.Items[I]);
      if MoveMsg.m_ShowOver then begin
        FItemList.Delete(I);
        MoveMsg.Free;
      end;
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewMoveMsgList.Clear;
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenNewMoveMsg(FItemList.Items[I]).Free;
    end;
    FItemList.Clear;
  finally
    FItemList.UnLock;
  end;

end;
// ==============================================================================

constructor TMoveHintMsgList.Create;
begin
  FItemList := TGList.Create;
end;

destructor TMoveHintMsgList.Destroy;
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin

    end;
  finally
    FItemList.UnLock;
  end;
  FItemList.Free;
  inherited;
end;

procedure TMoveHintMsgList.Add(sMsg:string; FColor, BColor:Byte; nX, nY:Integer);
var
  MsgRecord:PMoveHintMsgRecord;
begin
  FItemList.Lock;
  try
    New(MsgRecord);
    MsgRecord.Msg := sMsg;
    MsgRecord.FColor := FColor;
    MsgRecord.BColor := BColor;

    if nX = 0 then begin
      MsgRecord.nX := g_nMoveMouseX;

      if FrmDlg.DMerchantDlg.Visible then begin
        MsgRecord.nX := MsgRecord.nX - FrmDlg.DMerchantDlg.VirtualRect.Left;
      end;
    end
    else begin
      MsgRecord.nX := nX;
    end;

    if nY = 0 then begin
      MsgRecord.nY := g_nMoveMouseY - g_CurrentFontHeight - 5;

      if FrmDlg.DMerchantDlg.Visible then begin
        MsgRecord.nY := MsgRecord.nY - FrmDlg.DMerchantDlg.VirtualRect.Top;
      end;
    end
    else begin
      MsgRecord.nY := nY + 60;
    end;

    MsgRecord.nYOffset := 0;
    MsgRecord.LastUpdateTick := MyGetTickCount;

    FItemList.Add(MsgRecord);
  finally
    FItemList.UnLock;
  end;
end;

procedure TMoveHintMsgList.Draw();
var
  I, J:Integer;
  ImageInfo:TImageInfo;
  HGEFont:THGEFont;
  MsgRecord:PMoveHintMsgRecord;

  SL:TStringList;
  S, S2:string;
  Count:Integer;

  oX, oY:Integer;
begin
  HGEFont := TextureFonts.FindFont(g_sCurFontName, 9);
  if HGEFont = nil then Exit;

  if FrmDlg.DMerchantDlg.Visible then begin
    oX := FrmDlg.DMerchantDlg.VirtualRect.Left;
    oY := FrmDlg.DMerchantDlg.VirtualRect.Top;
  end else begin
    oX := 0; //HZQ 20230525 补全初始化
    oy := 0;
  end;

  FItemList.Lock;
  try
    if FItemList.Count > 0 then begin //HZQ 20230525 先判断Count大于0再去构造TStringList
      SL := TStringList.Create;
      try
        for I := 0 to FItemList.Count - 1 do begin
          MsgRecord := FItemList.Items[I];

          SL.Clear;

          Count := 0;
          S2 := MsgRecord.Msg;
          while True do begin
            S2 := GetValidStr3_Ex(S2, S, '\');

            if Length(S) = 0 then Break;
            SL.Add(S);
            Inc(Count);
            if Count >= 10 then Break;
          end;

          for J := 0 to SL.Count - 1 do begin
            S := SL.Strings[J];

            ImageInfo := HGEFont.GetImageInfo(S);

            HGEFont.TextOut(oX + MsgRecord.nX - 1, oY + MsgRecord.nY - MsgRecord.nYOffset - (g_CurrentFontHeight + 2) * (SL.Count - 1 - J), ImageInfo.ImageIndexs, GetRGB(MsgRecord.BColor));
            HGEFont.TextOut(oX + MsgRecord.nX + 1, oY + MsgRecord.nY - MsgRecord.nYOffset - (g_CurrentFontHeight + 2) * (SL.Count - 1 - J), ImageInfo.ImageIndexs, GetRGB(MsgRecord.BColor));
            HGEFont.TextOut(oX + MsgRecord.nX, oY + MsgRecord.nY - 1 - MsgRecord.nYOffset - (g_CurrentFontHeight + 2) * (SL.Count - 1 - J), ImageInfo.ImageIndexs, GetRGB(MsgRecord.BColor));
            HGEFont.TextOut(oX + MsgRecord.nX, oY + MsgRecord.nY + 1 - MsgRecord.nYOffset - (g_CurrentFontHeight + 2) * (SL.Count - 1 - J), ImageInfo.ImageIndexs, GetRGB(MsgRecord.BColor));
            HGEFont.TextOut(oX + MsgRecord.nX, oY + MsgRecord.nY - MsgRecord.nYOffset - (g_CurrentFontHeight + 2) * (SL.Count - 1 - J), ImageInfo.ImageIndexs, GetRGB(MsgRecord.FColor));
          end;
        end;
      finally
        SL.Free;
      end;
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TMoveHintMsgList.Update;
var
  I:Integer;
  MsgRecord:PMoveHintMsgRecord;
begin
  FItemList.Lock;
  try
    for I := FItemList.Count - 1 downto 0 do begin
      MsgRecord := FItemList.Items[I];

      if MsgRecord.nYOffset >= 40 then begin
        if MyGetTickCount - MsgRecord.LastUpdateTick >= 1500 then begin
          Dispose(MsgRecord);
          FItemList.Delete(I);
        end;
      end
      else if MyGetTickCount - MsgRecord.LastUpdateTick >= 10 then begin
        MsgRecord.LastUpdateTick := MyGetTickCount;
        Inc(MsgRecord.nYOffset, 2);
      end;
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TMoveHintMsgList.Clear;
var
  I:Integer;
  MsgRecord:PMoveHintMsgRecord;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      MsgRecord := FItemList.Items[I];
      Dispose(MsgRecord);
    end;
    FItemList.Clear;
  finally
    FItemList.UnLock;
  end;
end;

// ==============================================================================

constructor TScreenNewLineMsgList.Create;
begin
  FItemList := TGList.Create;
end;

destructor TScreenNewLineMsgList.Destroy;
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenCenterNewlineMsg(FItemList.Items[I]).Free();
    end;
  finally
    FItemList.UnLock;
  end;
  FItemList.Free;
  inherited;
end;

procedure TScreenNewLineMsgList.Add(sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
var
  I:Integer;
  boFind:Boolean;
  MoveMsg:TDrawScreenCenterNewlineMsg;
begin
  FItemList.Lock;
  try
    boFind := False;
    for I := 0 to FItemList.Count - 1 do begin
      MoveMsg := TDrawScreenCenterNewlineMsg(FItemList.Items[I]);
      if (MoveMsg.m_nY = nY) and (MoveMsg.m_nX = nX) then begin
        MoveMsg.ClearTimeCache;
        MoveMsg.Add(sMsg, FColor, BColor, FontSize, nX, nY, nTime, nDrawType);
        boFind := True;
        break;
      end;
    end;
    if not boFind then begin
      MoveMsg := TDrawScreenCenterNewlineMsg.Create;
      MoveMsg.Add(sMsg, FColor, BColor, FontSize, nX, nY, nTime, nDrawType);
      FItemList.Add(MoveMsg);
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewLineMsgList.Draw(boDrawBack:Boolean);
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenCenterNewlineMsg(FItemList.Items[I]).Draw(boDrawBack);
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewLineMsgList.Update;
var
  I:Integer;
  MoveMsg:TDrawScreenCenterNewlineMsg;
begin
  FItemList.Lock;
  try
    for I := FItemList.Count - 1 downto 0 do begin
      MoveMsg := TDrawScreenCenterNewlineMsg(FItemList.Items[I]);
      if MoveMsg.m_boShowOver then begin
        FItemList.Delete(I);
        MoveMsg.Free;
      end;
    end;
  finally
    FItemList.UnLock;
  end;
end;

procedure TScreenNewLineMsgList.Clear;
var
  I:Integer;
begin
  FItemList.Lock;
  try
    for I := 0 to FItemList.Count - 1 do begin
      TDrawScreenCenterNewlineMsg(FItemList.Items[I]).Free;
    end;
    FItemList.Clear;
  finally
    FItemList.UnLock;
  end;
end;
// ==============================================================================

constructor TDrawScreen.Create;
begin
  CurrentScene := nil;
  m_dwFrameTime := MyGetTickCount;
  m_dwFrameCount := 0;
  m_SysMsgList := TGStringList.Create;
  m_SysMsgListEx := TGStringList.Create;

  HintList := TStringList.Create;
  FScreenMoveMsgList := TScreenMoveMsgList.Create;
  FScreenNewMoveMsgList := TScreenNewMoveMsgList.Create;
  FScreenNewLineMsgList := TScreenNewLineMsgList.Create;

  DrawScreenCenterMsg := TDrawScreenCenterMsg.Create;
  DrawDelayMsg := TDrawDelayMsg.Create;

  FMoveHintMsgList := TMoveHintMsgList.Create;

  m_boShowLoginSceneShowRandomCodeDlg := False;
end;

destructor TDrawScreen.Destroy;
var
  I:Integer;
begin
  for I := 0 to m_SysMsgList.Count - 1 do begin
    TDrawSysMsg(m_SysMsgList.Objects[I]).Free;
  end;
  m_SysMsgList.Free;

  for I := 0 to m_SysMsgListEx.Count - 1 do begin
    TDrawSysMsgEx(m_SysMsgListEx.Objects[I]).Free;
  end;
  m_SysMsgListEx.Free;

  HintList.Free;
  DrawScreenCenterMsg.Free;
  DrawDelayMsg.Free;
  FScreenMoveMsgList.Free;
  FScreenNewMoveMsgList.Free;
  FScreenNewLineMsgList.Free;

  FMoveHintMsgList.Free;
  inherited;
end;

procedure TDrawScreen.Update;
begin
  FScreenMoveMsgList.Update;
  FScreenNewMoveMsgList.Update;
  FScreenNewLineMsgList.Update;
  HintWindows.Update;
  FMoveHintMsgList.Update;
end;

procedure TDrawScreen.Initialize;
begin
  FScreenMoveMsgList.Initialize;
  m_boInitialize := True;
end;

procedure TDrawScreen.Finalize;
begin
  m_boInitialize := False;
  FScreenMoveMsgList.Finalize;
end;

procedure TDrawScreen.KeyPress(var Key:Char);
begin
  if CurrentScene <> nil then
    CurrentScene.KeyPress(Key);
end;

procedure TDrawScreen.KeyDown(var Key:Word; Shift:TShiftState);
begin
  if CurrentScene <> nil then
    CurrentScene.KeyDown(Key, Shift);
end;

procedure TDrawScreen.MouseMove(Shift:TShiftState; X, Y:Integer);
begin
  if CurrentScene <> nil then
    CurrentScene.MouseMove(Shift, X, Y);
end;

procedure TDrawScreen.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  if CurrentScene <> nil then
    CurrentScene.MouseDown(Button, Shift, X, Y);
end;

procedure TDrawScreen.ChangeScene(SceneType:TSceneType);
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_CriticalSection);
  try
    {$IFEND}
    if CurrentScene <> nil then
      CurrentScene.CloseScene;
    case SceneType of
      stWelcome:CurrentScene := WelcomeScene;
      stLogin:CurrentScene := LoginScene;
      stSelectCountry:;
      stSelectChr:begin
          //if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
          CurrentScene := SelectChrScene;
        end;
      stNewChr:;
      stLoading:begin
          //if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
        end;
      stLoginNotice:begin
          //if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
          CurrentScene := LoginNoticeScene;
        end;
      stPlayGame:begin
          //if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
          CurrentScene := PlayScene;
        end;
    end;

    if CurrentScene <> nil then begin
      CurrentScene.OpenScene;
      if (CurrentScene = LoginScene) and (m_boShowLoginSceneShowRandomCodeDlg) then begin
        FrmDlg.OpenDRandomCodeDlg;
      end;
    end;
    {$IF IsMultiThreadRender = 1}
  finally
    LeaveCriticalSection(g_CriticalSection);
  end;
  {$IFEND}
end;

procedure TDrawScreen.AddSysMsg(Msg:string; FColor, Bcolor:Byte; X, Y:Integer; boDownToUP:Boolean; boDrawBottom:Boolean);
var
  I:Integer;
  DrawSysMsg, CurDrawSysMsg:TDrawSysMsg;
begin
  m_SysMsgList.Lock;
  try
    CurDrawSysMsg := nil;
    for I := 0 to m_SysMsgList.Count - 1 do begin
      DrawSysMsg := TDrawSysMsg(m_SysMsgList.Objects[I]);
      if (DrawSysMsg.m_nX = X) and (DrawSysMsg.m_nY = Y) and
        (DrawSysMsg.m_boDownToUP = boDownToUP) and (DrawSysMsg.m_boDrawBottom = boDrawBottom) then begin
        CurDrawSysMsg := DrawSysMsg;
        break;
      end;
    end;
    if CurDrawSysMsg = nil then begin
      CurDrawSysMsg := TDrawSysMsg.Create;
      CurDrawSysMsg.m_nX := X;
      CurDrawSysMsg.m_nY := Y;
      CurDrawSysMsg.m_boDownToUP := boDownToUP;
      CurDrawSysMsg.m_boDrawBottom := boDrawBottom;
      m_SysMsgList.AddObject('', CurDrawSysMsg);
    end;
    CurDrawSysMsg.Add(Msg, FColor, Bcolor);
  finally
    m_SysMsgList.UnLock;
  end;
end;

(*
procedure TDrawScreen.AddSysMsg(Msg: string; FColor, Bcolor: Byte; X, Y: Integer; boDownToUP: Boolean);
var
  I: Integer;
  DrawSysMsg, CurDrawSysMsg: TDrawSysMsgEx;
begin
  m_SysMsgListEx.Lock;
  try
    CurDrawSysMsg := nil;
    for I := 0 to m_SysMsgListEx.Count - 1 do
    begin
      DrawSysMsg := TDrawSysMsgEx(m_SysMsgListEx.Objects[I]);
      if (DrawSysMsg.m_nX = X) and (DrawSysMsg.m_nY = Y) and (DrawSysMsg.m_boDownToUP = boDownToUP) then
      begin
        CurDrawSysMsg := DrawSysMsg;
        break;
      end;
    end;
    if CurDrawSysMsg = nil then
    begin
      CurDrawSysMsg := TDrawSysMsgEx.Create;
      CurDrawSysMsg.m_nX := X;
      CurDrawSysMsg.m_nY := Y;
      CurDrawSysMsg.m_boDownToUP := boDownToUP;
      m_SysMsgListEx.AddObject('', CurDrawSysMsg);
    end;
    CurDrawSysMsg.Add(Msg, FColor, Bcolor);
  finally
    m_SysMsgListEx.UnLock;
  end;
end;

procedure TDrawScreen.AddSysMsgEx(Msg: string; FColor, Bcolor: Byte; X, Y: Integer; boDownToUP: Boolean);
var
  I: Integer;
  DrawSysMsg, CurDrawSysMsg: TDrawSysMsgEx;
begin
  m_SysMsgListEx.Lock;
  try
    CurDrawSysMsg := nil;
    for I := 0 to m_SysMsgListEx.Count - 1 do
    begin
      DrawSysMsg := TDrawSysMsgEx(m_SysMsgListEx.Objects[I]);
      if (DrawSysMsg.m_nX = X) and (DrawSysMsg.m_nY = Y) and (DrawSysMsg.m_boDownToUP = boDownToUP) then
      begin
        CurDrawSysMsg := DrawSysMsg;
        break;
      end;
    end;
    if CurDrawSysMsg = nil then
    begin
      CurDrawSysMsg := TDrawSysMsgEx.Create;
      CurDrawSysMsg.m_nX := X;
      CurDrawSysMsg.m_nY := Y;
      CurDrawSysMsg.m_boDownToUP := boDownToUP;
      m_SysMsgListEx.AddObject('', CurDrawSysMsg);
    end;
    CurDrawSysMsg.Add(Msg, FColor, Bcolor);
  finally
    m_SysMsgListEx.UnLock;
  end;
end;
*)

procedure TDrawScreen.AddChatBoardString(Str:string; FColor, Bcolor:Integer);
begin
  FrmDlg.DChatMemo.Add(Str, FColor, Bcolor);
end;

procedure TDrawScreen.AddTopChatBoardString(Str:string; FColor, BColor, TimeOut:Integer); // 显示在聊天框顶部的信息
begin
  FrmDlg.DChatMemo.AddTop(Str, FColor, BColor, TimeOut);
end;

procedure TDrawScreen.AddMoveMsg(sMsg:string; FColor, BColor:Byte; nY, nCount:Integer; boShowFrame:Boolean; btFrameColor:Byte; btFontSize:Byte; boFontBold:Boolean; nMarqueeTime:Integer);
begin
  FScreenMoveMsgList.Add(sMsg, FColor, BColor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
end;

procedure TDrawScreen.AddNewMoveMsg(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
begin
  FScreenNewMoveMsgList.Add(sMsg, FColor, BColor, FontSize, nX, nY, nCount);
end;

procedure TDrawScreen.AddNewLineMsg(sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
begin
  FScreenNewLineMsgList.Add(sMsg, FColor, BColor, FontSize, nX, nY, nTime, nDrawType);
end;

procedure TDrawScreen.AddMoveHintMsg(sMsg:string; FColor, BColor:Byte; nX, nY:Integer);
begin
  FMoveHintMsgList.Add(sMsg, FColor, BColor, nX, nY);
end;

procedure TDrawScreen.ShowHint(X, Y:Integer; Msg:string; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean);
begin
  // ClearHint;
  HintWindows.Show(X, Y, Msg, Color, DrawUp, DrawLeft, ShowBackground);

  {HintX := X;
  HintY := Y;
  HintWidth := 0;
  HintHeight := 0;
  HintUp := drawup;
  HintColor := Color;
  while True do begin
    if Str = '' then Break;
    Str := GetValidStr3_Ex(Str, Data, '\');
    w := CurrentFont.TextWidth(Data) + 4 * 2;
    if w > HintWidth then HintWidth := w;
    if Data <> '' then
      HintList.Add(Data)
  end;
  HintHeight := (g_CurrentFontHeight + 1) * HintList.Count + 3 * 2;
  if HintUp then
    HintY := HintY - HintHeight;}
end;

procedure TDrawScreen.ClearHint;
begin
  HintList.Clear;
end;

procedure TDrawScreen.ClearChatBoard;
var
  I:Integer;
begin
  FScreenMoveMsgList.Clear;
  FScreenNewMoveMsgList.Clear;
  FScreenNewLineMsgList.Clear;

  DrawScreenCenterMsg.Clear;
  DrawDelayMsg.Clear;

  m_SysMsgList.Lock;
  try
    for I := 0 to m_SysMsgList.Count - 1 do begin
      TDrawSysMsg(m_SysMsgList.Objects[I]).Free;
    end;
    m_SysMsgList.Clear;
  finally
    m_SysMsgList.UnLock;
  end;

  m_SysMsgListEx.Lock;
  try
    for I := 0 to m_SysMsgListEx.Count - 1 do begin
      TDrawSysMsgEx(m_SysMsgListEx.Objects[I]).Free;
    end;
    m_SysMsgListEx.Clear;
  finally
    m_SysMsgListEx.UnLock;
  end;
end;

procedure TDrawScreen.DrawScreen();
var
  I, Line, k:Integer;
  Str, s1:string;
  d:TTexture;
  MySelf:TActor;
  FocusCret:TActor;
  MyHero:TActor;
  boShowNumber:Boolean;
begin
  Str := '';
  Line := 1;
  if g_MySelf <> nil then begin
    k := 0;

    for I := 0 to 15 do begin
      // 地图右上角战斗，安全标记 显示错误 ($01 shr I) 改为 ($01 shl I) chongchong 2014-01-07
      if g_nAreaStateValue and ($01 shl I) <> 0 then begin
        d := g_WMainImages.Images[AREASTATEICONBASE + I];
        if d <> nil then begin
          k := k + d.Width;
          GameCanvas.Draw(SCREENWIDTH - k, 0, d.ClientRect, d);
        end;
      end;
    end;

    if PlugInEnabled and g_ClientConfig.boShowGreenHint and g_ConfigDlg.ConfigCheckeds[ckShowGreenHint] then begin
      {$IF IsMultiThreadRender = 1}
      EnterCriticalSection(g_ActorLock);
      {$IFEND}
      MySelf := g_MySelf;
      FocusCret := g_FocusCret;
      MyHero := g_MyHero;
      {$IF IsMultiThreadRender = 1}
      LeaveCriticalSection(g_ActorLock);
      {$IFEND}
      if (MySelf <> nil) and (not PlayScene.IsValidActorEx(MySelf)) then begin
        MySelf := nil;
      end;
      if (FocusCret <> nil) and (not PlayScene.IsValidActorEx(FocusCret)) then begin
        FocusCret := nil;
      end;
      if (MyHero <> nil) and (not PlayScene.IsValidActorEx(MyHero)) then begin
        MyHero := nil;
      end;

      if MySelf <> nil then begin
        if not g_ClientConfig.boGreenHintNewStyle then begin
          Str :=
            '等级: ' + IntToStr(MySelf.m_Abil.Level) + ' 经验(' + IntToStr(MySelf.m_Abil.Exp) + '/' + IntToStr(MySelf.m_Abil.MaxExp) + ')' +
            ' 负重: ' + IntToStr(MySelf.m_Abil.Weight) + '/' + IntToStr(MySelf.m_Abil.MaxWeight) +
            ' ' + g_sGoldName + ': ' + IntToStr(MySelf.m_nGold) +
            ' ' + g_sGameGoldName + ': ' + IntToStr(MySelf.m_nGameGold) +
            ' 鼠标: ' + IntToStr(g_nMouseCurrX) + ':' + IntToStr(g_nMouseCurrY) + '(' + IntToStr(g_nMouseX) + ':' + IntToStr(g_nMouseY) + ')';
        end else begin
          Str :=
            //'等级: ' + IntToStr(MySelf.m_Abil.Level) + {' 经验(' + IntToStr(MySelf.m_Abil.Exp) + '/' + IntToStr(MySelf.m_Abil.MaxExp) + ')' +}
          //' ' + g_sGoldName + ': ' + IntToStr(MySelf.m_nGold) +
          //' ' + g_sGameGoldName + ': ' + IntToStr(MySelf.m_nGameGold) +
          ' 防御: ' + IntToStr(g_MySelf.m_Abil.AC1) + '-' + IntToStr(g_MySelf.m_Abil.AC2) +
            ' 魔防: ' + IntToStr(g_MySelf.m_Abil.MAC1) + '-' + IntToStr(g_MySelf.m_Abil.MAC2) +
            ' 攻击: ' + IntToStr(g_MySelf.m_Abil.DC1) + '-' + IntToStr(g_MySelf.m_Abil.DC2) +
            ' 魔法: ' + IntToStr(g_MySelf.m_Abil.MC1) + '-' + IntToStr(g_MySelf.m_Abil.MC2) +
            ' 道术: ' + IntToStr(g_MySelf.m_Abil.SC1) + '-' + IntToStr(g_MySelf.m_Abil.SC2) +
            ' 鼠标: ' + IntToStr(g_nMouseCurrX) + ':' + IntToStr(g_nMouseCurrY) + '(' + IntToStr(g_nMouseX) + ':' + IntToStr(g_nMouseY) + ')';
        end;
      end;

      if (FocusCret <> nil) then begin
        boShowNumber := False;
        if (FocusCret.m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
          if (g_ClientConfig.boHumStruckShowNumber and FocusCret.m_boStruckShowNumber) or (not g_ClientConfig.boHumStruckShowNumber) or (FocusCret = g_MySelf) or FocusCret.m_boOpenHealth then begin
            if (FocusCret.m_Abil.MaxHP > 0) then begin
              boShowNumber := True;
            end;
          end;
        end
        else begin
          if (g_ClientConfig.boMonStruckShowNumber and FocusCret.m_boStruckShowNumber) or (not g_ClientConfig.boMonStruckShowNumber) or FocusCret.m_boOpenHealth then begin
            if (FocusCret.m_btRace <> RC_MERCHANT) and (FocusCret.m_Abil.MaxHP > 0) then begin
              boShowNumber := True;
            end;
          end;
        end;

        if boShowNumber then
          s1 := '目标: ' + FocusCret.m_sUserName + '(' + IntToStr(FocusCret.m_Abil.HP) + '/' + IntToStr(FocusCret.m_Abil.MaxHP) + ')'
        else
          s1 := '目标: ' + FocusCret.m_sUserName + '(0/0)';
      end
      else begin
        s1 := '目标: -/-';
      end;

      if CurrentFont.TextWidth(Str + s1 + ' ') > SCREENWIDTH then begin
        Str := Str + #13 + s1;
        Inc(Line);
      end
      else
        Str := Str + ' ' + s1;

      if MyHero <> nil then begin
        s1 := Format('我的英雄: %s(%d/%d)', [MyHero.m_sUserName, MyHero.m_nCurrX, MyHero.m_nCurrY]);
        if Pos(#13, Str) <= 0 then begin
          if CurrentFont.TextWidth(Str + s1 + ' ') > SCREENWIDTH then begin
            Str := Str + #13 + s1;
            Inc(Line);
          end
          else
            Str := Str + ' ' + s1;
        end
        else
          Str := Str + ' ' + s1;
      end;
      BoldTextOut(2, 0, Str, clLime);
    end;

    if (g_nAreaStateValue and $04) <> 0 then begin
      if Length(Str) > 0 then begin
        BoldTextOut(2, g_CurrentFontHeight * Line, '攻城区域', clWhite)
      end
      else
        BoldTextOut(2, 0, '攻城区域', clWhite)
    end;
  end;
end;

// 显示左上角信息文字

procedure TDrawScreen.DrawMsg_TopLevel();
var
  I:Integer;
  DrawSysMsg:TDrawSysMsg;
begin
  if (not m_boInitialize) or (g_MySelf = nil) then Exit;
  if CurrentScene = PlayScene then begin
    m_SysMsgList.Lock;
    try
      for I := 0 to m_SysMsgList.Count - 1 do begin
        DrawSysMsg := TDrawSysMsg(m_SysMsgList.Objects[I]);

        if not DrawSysMsg.m_boDrawBottom then DrawSysMsg.Draw;
      end;
    finally
      m_SysMsgList.UnLock;
    end;

    m_SysMsgListEx.Lock;
    try
      for I := 0 to m_SysMsgListEx.Count - 1 do begin
        TDrawSysMsgEx(m_SysMsgListEx.Objects[I]).Draw;
      end;
    finally
      m_SysMsgListEx.UnLock;
    end;

    DrawDelayMsg.Draw();
    DrawScreenCenterMsg.Draw();

    FMoveHintMsgList.Draw();
  end;
end;

procedure TDrawScreen.DrawSysMsg_BottomLevel();
var
  I:Integer;
  DrawSysMsg:TDrawSysMsg;
begin
  if (not m_boInitialize) or (g_MySelf = nil) then Exit;
  if CurrentScene = PlayScene then begin
    m_SysMsgList.Lock;
    try
      for I := 0 to m_SysMsgList.Count - 1 do begin
        DrawSysMsg := TDrawSysMsg(m_SysMsgList.Objects[I]);

        if DrawSysMsg.m_boDrawBottom then DrawSysMsg.Draw;
      end;
    finally
      m_SysMsgList.UnLock;
    end;
  end;
end;

procedure TDrawScreen.DrawMove();
begin
  if (not m_boInitialize) or (g_MySelf = nil) then Exit;
  if CurrentScene = PlayScene then begin
    FScreenMoveMsgList.Draw();
    FScreenNewMoveMsgList.Draw();
    FScreenNewLineMsgList.Draw(True);
  end;
end;

procedure TDrawScreen.DrawMoveBefor();
begin
  if (not m_boInitialize) or (g_MySelf = nil) then Exit;
  if CurrentScene = PlayScene then begin
    //    FScreenMoveMsgList.Draw();
    //    FScreenNewMoveMsgList.Draw();
    FScreenNewLineMsgList.Draw(False);
  end;
end;

procedure TDrawScreen.DrawHint();
begin

end;

{ TDrawScreenCenterNewlineMsg }

procedure TDrawScreenCenterNewlineMsg.ClearTimeCache;
var
  I:Integer;
  CacheText:PDrawScreenNewMsgCacheText;
begin
  for I := FCacheList.Count - 1 downto 0 do begin
    CacheText := FCacheList.Items[I];
    if tick_diff(CacheText.nAddTick, MyGetTickCount) >= Cardinal(CacheText.nTime + 8000) then begin
      Dispose(CacheText);
      FCacheList.Delete(I);
    end;
  end;
end;

procedure TDrawScreenCenterNewlineMsg.Add(const sMsg:string; FColor, BColor,
  FontSize:Byte; nX, nY, nTime, nDrawType:Integer);
var
  I:Integer;
  slLines:TStringList;
  HGEFont:THGEFont;

  TokenLine:TStringLineEx;
  CacheText:PDrawScreenNewMsgCacheText;
  sTemp:string;
begin
  if Length(sMsg) = 0 then Exit;

  if m_ShowLines.Count > 0 then begin
    New(CacheText);

    CacheText.sMsg := sMsg;
    CacheText.FColor := FColor;
    CacheText.BColor := BColor;
    CacheText.FontSize := FontSize;
    CacheText.nX := nX;
    CacheText.nY := nY;
    CacheText.nTime := nTime;
    CacheText.nDrawType := nDrawType;
    CacheText.nAddTick := MyGetTickCount;

    FCacheList.Add(CacheText);
    Exit;
  end;

  m_ShowLines.Lock;
  try
    m_nCurY := 0;
    m_nState := 0;
    m_nStandTime := nTime * 1000;
    m_FColor := FColor;
    m_BColor := BColor;
    m_btFontSize := FontSize;
    m_nY := ny;
    m_nX := nX;
    m_nDrawType := nDrawType;

    //ClearShowLines(False);

    HGEFont := TextureFonts.FindFont(g_sCurFontName, m_btFontSize);
    if HGEFont <> nil then begin
      sTemp := StringReplace(sMsg, '||', sLineBreak, [rfReplaceAll]);
      slLines := TStringList.Create;
      try
        slLines.Text := sTemp;
        for I := 0 to slLines.Count - 1 do begin
          TokenLine := TStringLineEx.Create;
          GetTextListEx(slLines[I], GetRGB(m_FColor), GetRGB(m_BColor), TokenLine);
          m_ShowLines.AddObject(slLines[I], TokenLine);
        end;
      finally
        slLines.Free;
      end;
    end;
  finally
    m_ShowLines.UnLock;
  end;
end;

procedure TDrawScreenCenterNewlineMsg.ClearShowLines(Lock:Boolean);
var
  I:Integer;
  TokenLine:TStringLineEx;
begin
  if Lock then
    m_ShowLines.Lock;

  for I := 0 to m_ShowLines.Count - 1 do begin
    TokenLine := TStringLineEx(m_ShowLines.Objects[I]);
    TokenLine.Free;
  end;
  m_ShowLines.Clear;

  if Lock then
    m_ShowLines.UnLock;
end;

procedure TDrawScreenCenterNewlineMsg.ClearCacheLines;
var
  I:Integer;
  CacheText:PDrawScreenNewMsgCacheText;
begin
  for I := 0 to FCacheList.Count - 1 do begin
    CacheText := FCacheList.Items[I];
    Dispose(CacheText);
  end;

  FCacheList.Clear;
end;

constructor TDrawScreenCenterNewlineMsg.Create;
begin
  inherited Create;
  m_ShowLines := TGStringList.Create;
  m_FColor := 255;
  m_BColor := 0;
  m_btFontSize := 20;
  m_nStandTime := 10;
  m_nDrawType := 0;

  m_boShowOver := False;

  FCacheList := TList.Create;
end;

destructor TDrawScreenCenterNewlineMsg.Destroy;
begin
  ClearShowLines;
  m_ShowLines.Free;

  ClearCacheLines;
  FCacheList.Free;
  inherited Destroy;
end;

procedure TDrawScreenCenterNewlineMsg.Draw(boDrawBack:Boolean);
var
  sText:string;
  I, J, nX, nY, nTextHeight:Integer;
  HGEFont:THGEFont;

  TokenLine:TStringLineEx;
  Token:PStringToken;
  CacheText:PDrawScreenNewMsgCacheText;

  nCode, nDrawType:Integer;
begin
  nCode := 0;

  if (m_nDrawType div 100 = 0) and not boDrawBack then Exit;

  nDrawType := m_nDrawType mod 100;
  try
    if nDrawType = 1 then begin
      nCode := 1;
      DrawEx;
      Exit;
    end;

    nCode := 2;
    m_ShowLines.Lock;
    try
      if m_ShowLines.Count = 0 then Exit;

      nCode := 3;
      if m_nState = 0 then begin
        m_dwStartStandTick := MyGetTickCount;
        m_nState := 1;
      end;

      nCode := 4;
      if MyGetTickCount - m_dwStartStandTick >= Cardinal(m_nStandTime) then begin
        nCode := 5;
        ClearShowLines(False);

        nCode := 6;
        if FCacheList.Count > 0 then begin
          nCode := 7;
          CacheText := FCacheList.Items[0];
          Add(CacheText.sMsg, CacheText.FColor, CacheText.BColor, CacheText.FontSize, CacheText.nX, CacheText.nY, CacheText.nTime, CacheText.nDrawType);

          nCode := 8;
          Dispose(CacheText);
          FCacheList.Delete(0);
        end
        else begin
          m_boShowOver := True;
        end;

        Exit;
      end;

      nCode := 9;
      // 加粗
      HGEFont := TextureFonts.FindFont(g_sCurFontName, m_btFontSize, [fsBold]);

      nCode := 10;
      nTextHeight := HGEFont.TextHeight('Pp');
      nY := m_ShowLines.Count * (nTextHeight + 2);

      nCode := 11;
      if nDrawType = 0 then
        // 绘制透明矩形框
        GameCanvas.FillRectAlpha(Rect(80, m_nY, SCREENWIDTH - 80, m_nY + nY + 4), GetRGB(190), 100);

      nCode := 12;
      if HGEFont <> nil then begin
        nTextHeight := HGEFont.TextHeight('Pp');
        //nY := (SCREENHEIGHT - m_ShowLines.Count * nTextHeight) div 2;
        nY := m_nY + 4;
        for I := 0 to m_ShowLines.Count - 1 do begin
          nCode := 13;
          {
          sText := m_ShowLines.Strings[I];
          nX := (SCREENWIDTH - HGEFont.TextWidth(sText)) div 2;
          BoldTextOut(HGEFont, nX, nY, sText, GetRGB(m_FColor), GetRGB(m_BColor));
          }

          TokenLine := TStringLineEx(m_ShowLines.Objects[I]);

          nCode := 14;
          sText := GetStrinLineExText(TokenLine);
          if m_nX = 0 then
            nX := (SCREENWIDTH - HGEFont.TextWidth(sText)) div 2
          else
            nX := m_nX;

          nCode := 15;
          for J := 0 to TokenLine.Count - 1 do begin
            Token := TokenLine.Tokens[J];
            BoldTextOut(HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor);
            nX := nX + HGEFont.TextWidth(Token.Text);
          end;

          nCode := 16;
          Inc(nY, nTextHeight + 2);
        end;
      end;
    finally
      m_ShowLines.UnLock;
    end;
  except
    DebugOutStr('[Exception] TDrawScreenCenterNewlineMsg::Draw Error; Code = ' + IntToStr(nCode));
  end;
end;

procedure TDrawScreenCenterNewlineMsg.DrawEx;
var
  I, J, nX, nY, nTextHeight:Integer;
  HGEFont:THGEFont;
  nTempAlpha:Byte;

  TokenLine:TStringLineEx;
  Token:PStringToken;
  sText:string;
  CacheText:PDrawScreenNewMsgCacheText;
begin
  m_ShowLines.Lock;
  try
    if m_ShowLines.Count = 0 then Exit;

    {if MyGetTickCount >= m_dwShowTime then
    begin
      m_ShowLines.Clear;
      Exit;
    end;}

    // 加粗
    HGEFont := TextureFonts.FindFont(g_sCurFontName, m_btFontSize, [fsBold]);

    nTextHeight := HGEFont.TextHeight('Pp');
    nY := m_ShowLines.Count * (nTextHeight + 2);

    if (m_nState = 0) and (m_nCurY = 0) then
      m_nCurY := m_nY + nY * 1
    else
      Dec(m_nCurY, 1);

    if (m_nCurY <= m_nY) and (m_nState = 0) then begin
      m_nState := 1;
      m_nCurY := m_nY;
      // 开始停留
      m_dwStartStandTick := MyGetTickCount;
    end;

    if m_nState <> 1 then begin
      nTempAlpha := 255 - Abs(Round((m_nCurY - m_nY) / (1 * nY) * 255));
      if (m_nState = 2) and (m_nCurY < m_nY - nY) then begin
        //m_boShowOver := True;

        ClearShowLines(False);

        if FCacheList.Count = 0 then begin
          m_boShowOver := True
        end else begin
          while FCacheList.Count > 0 do begin
            CacheText := FCacheList.Items[0];
            if tick_diff(CacheText.nAddTick, MyGetTickCount) >= Cardinal(CacheText.nTime + 5000) then begin
              Dispose(CacheText);
              FCacheList.Delete(0);

              if FCacheList.Count = 0 then
                m_boShowOver := True;
            end else begin
              Add(CacheText.sMsg, CacheText.FColor, CacheText.BColor, CacheText.FontSize, CacheText.nX, CacheText.nY, CacheText.nTime, CacheText.nDrawType);

              Dispose(CacheText);
              FCacheList.Delete(0);
              Break;
            end;
          end;
        end;

        Exit;
      end;
    end else begin
      m_nCurY := m_nY;
      nTempAlpha := 255;
      if MyGetTickCount - m_dwStartStandTick >= Cardinal(m_nStandTime) then begin
        m_nState := 2;
      end;
    end;

    // 绘制透明矩形框
    // GameCanvas.FillRectAlpha(Rect(80, m_nY, SCREENWIDTH - 80, m_nY + nY + 4), GetRGB(190), 100);

    if HGEFont <> nil then begin
      nTextHeight := HGEFont.TextHeight('Pp');
      //nY := (SCREENHEIGHT - m_ShowLines.Count * nTextHeight) div 2;
      nY := m_nCurY;
      for I := 0 to m_ShowLines.Count - 1 do begin
        {
        nX := (SCREENWIDTH - HGEFont.TextWidth(m_ShowLines.Strings[I])) div 2;
        if m_nState = 1 then
          BoldTextOut(HGEFont, nX, nY, m_ShowLines.Strings[I], GetRGB(m_FColor), GetRGB(m_BColor), nTempAlpha)
        else
          BoldTextOutEx(HGEFont, nX, nY, m_ShowLines.Strings[I], GetRGB(m_FColor), GetRGB(m_BColor), nTempAlpha);
        }

        TokenLine := TStringLineEx(m_ShowLines.Objects[I]);
        sText := GetStrinLineExText(TokenLine);

        if m_nX = 0 then
          nX := (SCREENWIDTH - HGEFont.TextWidth(sText)) div 2
        else
          nX := m_nX;

        for J := 0 to TokenLine.Count - 1 do begin
          Token := TokenLine.Tokens[J];
          if m_nState = 1 then
            BoldTextOut(HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor, nTempAlpha)
          else
            BoldTextOutEx(HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor, nTempAlpha);
          nX := nX + HGEFont.TextWidth(Token.Text);
        end;

        Inc(nY, nTextHeight + 2);
      end;
    end;
  finally
    m_ShowLines.UnLock;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDrawScreenNewMoveMsg.Create;
begin
  m_ShowLines := TGStringList.Create;
  m_ShowOver := False;
  FCacheList := TList.Create;
end;

destructor TDrawScreenNewMoveMsg.Destroy;
begin
  ClearShowLines;
  m_ShowLines.Free;

  ClearCacheLines;
  FCacheList.Free;
  inherited;
end;

procedure TDrawScreenNewMoveMsg.ClearShowLines;
var
  I:Integer;
  TokenLine:TStringLineEx;
begin
  for I := 0 to m_ShowLines.Count - 1 do begin
    TokenLine := TStringLineEx(m_ShowLines.Objects[I]);
    TokenLine.Free;
  end;

  m_ShowLines.Clear;
end;

procedure TDrawScreenNewMoveMsg.ClearCacheLines;
var
  I:Integer;
  CacheText:PDrawScreenNewMsgCacheText;
begin
  for I := 0 to FCacheList.Count - 1 do begin
    CacheText := FCacheList.Items[I];
    Dispose(CacheText);
  end;

  FCacheList.Clear;
end;

procedure TDrawScreenNewMoveMsg.Add(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer);
var
  I:Integer;
  slLines:TStringList;
  TokenLine:TStringLineEx;

  CacheText:PDrawScreenNewMsgCacheText;
begin
  if Length(sMsg) = 0 then Exit;

  slLines := TStringList.Create;
  m_ShowLines.Lock;
  try
    if m_ShowLines.Count > 0 then begin
      New(CacheText);
      CacheText.sMsg := sMsg;
      CacheText.FColor := FColor;
      CacheText.BColor := BColor;
      CacheText.FontSize := FontSize;
      CacheText.nX := nX;
      CacheText.nY := nY;
      CacheText.nCount := nCount;

      FCacheList.Add(CacheText);
      Exit;
    end;

    m_HGEFont := TextureFonts.FindFont(g_sCurFontName, FontSize, [fsBold]);
    if m_HGEFont = nil then Exit;

    //ClearShowLines;

    m_FontSize := FontSize;
    m_FColor := GetRGB(FColor);
    m_BColor := GetRGB(BColor);
    m_TotalCount := nCount;
    m_CurrentCount := 1; // 次数
    m_nX := nX;

    m_LineHeight := m_HGEFont.TextHeight('文字') + 2;
    m_Y0 := nY - m_LineHeight;
    m_Y1 := nY;
    m_Y2 := nY + m_LineHeight;
    m_StepMove := 2;
    m_StepAlphaChange := Round(250 / (m_LineHeight / m_StepMove));

    // 拆分每行
    {
    slLines.Delimiter := '|';
    slLines.DelimitedText := sMsg;
    for i := 0 to slLines.Count - 1 do
      m_ShowLines.Add(slLines[i]);
    }

    sMsg := StringReplace(sMsg, '||', sLineBreak, [rfReplaceAll]);
    slLines.Text := sMsg;
    for I := 0 to slLines.Count - 1 do begin
      TokenLine := TStringLineEx.Create;
      GetTextListEx(slLines[I], m_FColor, m_BColor, TokenLine);
      m_ShowLines.AddObject(slLines[I], TokenLine);
    end;

    m_CurLineIndex := 0;
    m_IsStayShow := True; // 是否为停留显示 （不是滚动显示）
    m_StartStayTime := MyGetTickCount; // 开始停留时间
  finally
    m_ShowLines.UnLock;
    slLines.Free;
  end;
end;

procedure TDrawScreenNewMoveMsg.Draw();
var
  I, nX, nY:Integer;
  S:string;
  TokenLine:TStringLineEx;
  Token:PStringToken;

  CacheText:PDrawScreenNewMsgCacheText;
begin
  if m_HGEFont = nil then Exit;
  m_ShowLines.Lock;
  try
    if m_IsStayShow then begin
      if (MyGetTickCount - m_StartStayTime <= 2000) then begin
        if (m_CurLineIndex > m_ShowLines.Count - 1) then begin
          if (m_CurrentCount < m_TotalCount) then begin
            Inc(m_CurrentCount);
            m_CurLineIndex := 0;
          end
          else begin
            //m_ShowOver := True;

            ClearShowLines;

            if FCacheList.Count = 0 then
              m_ShowOver := True
            else begin
              while FCacheList.Count > 0 do begin
                CacheText := FCacheList.Items[0];
                if tick_diff(CacheText.nAddTick, MyGetTickCount) >= Cardinal(CacheText.nTime + 5000) then begin
                  Dispose(CacheText);
                  FCacheList.Delete(0);

                  if FCacheList.Count = 0 then
                    m_ShowOver := True;
                end else begin
                  Add(CacheText.sMsg, CacheText.FColor, CacheText.BColor, CacheText.FontSize, CacheText.nX, CacheText.nY, CacheText.nCount);

                  Dispose(CacheText);
                  FCacheList.Delete(0);
                  Break;
                end;
              end;
            end;

            Exit;
          end;
        end;

        {
        S := m_ShowLines.Strings[m_CurLineIndex];
        nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2;
        nY := m_Y1;
        BoldTextOut(m_HGEFont, nX, nY, S, m_FColor, m_BColor);
        }

        TokenLine := TStringLineEx(m_ShowLines.Objects[m_CurLineIndex]);
        S := GetStrinLineExText(TokenLine);
        if m_nX = 0 then
          nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2
        else
          nX := m_nX;
        nY := m_Y1;
        for I := 0 to TokenLine.Count - 1 do begin
          Token := TokenLine.Tokens[I];
          BoldTextOut(m_HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor);
          nX := nX + m_HGEFont.TextWidth(Token.Text);
        end;
      end
      else begin
        m_IsStayShow := False;

        m_OffsetY1 := m_Y1;
        m_OffsetY2 := m_Y2;

        m_Alpha1 := 250;
        m_Alpha2 := 0;

        m_LastTime := MyGetTickCount - 1000;
      end;

      Exit;
    end;

    if MyGetTickCount - m_LastTime >= 100 then begin
      m_OffsetY1 := m_OffsetY1 - m_StepMove;
      m_Alpha1 := m_Alpha1 - m_StepAlphaChange;

      m_OffsetY2 := m_OffsetY2 - m_StepMove;
      m_Alpha2 := m_Alpha2 + m_StepAlphaChange;

      if (m_OffsetY1 <= m_Y0) or (m_OffsetY2 <= m_Y1) then begin
        m_OffsetY1 := m_Y0;
        m_Alpha1 := 0;

        m_OffsetY2 := m_Y1;
        m_Alpha2 := 255;

        Inc(m_CurLineIndex);

        m_IsStayShow := True;
        m_StartStayTime := MyGetTickCount;
        Exit;
      end;
      m_LastTime := MyGetTickCount;
    end;

    if m_CurLineIndex > m_ShowLines.Count - 1 then Exit;

    {
    S := m_ShowLines.Strings[m_CurLineIndex];
    nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2;
    nY := m_OffsetY1;
    BoldTextOutEx(m_HGEFont, nX, nY, S, m_FColor, m_BColor, m_Alpha1);
    }

    TokenLine := TStringLineEx(m_ShowLines.Objects[m_CurLineIndex]);
    S := GetStrinLineExText(TokenLine);
    if m_nX = 0 then
      nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2
    else
      nX := m_nX;
    nY := m_OffsetY1;
    for I := 0 to TokenLine.Count - 1 do begin
      Token := TokenLine.Tokens[I];
      BoldTextOutEx(m_HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor, m_Alpha1);
      nX := nX + m_HGEFont.TextWidth(Token.Text);
    end;

    if m_CurLineIndex + 1 <= m_ShowLines.Count - 1 then begin
      {
      S := m_ShowLines.Strings[m_CurLineIndex + 1];
      nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2;
      nY := m_OffsetY2;
      BoldTextOutEx(m_HGEFont, nX, nY, S, m_FColor, m_BColor, m_Alpha2);
      }

      TokenLine := TStringLineEx(m_ShowLines.Objects[m_CurLineIndex + 1]);
      S := GetStrinLineExText(TokenLine);
      if m_nX = 0 then
        nX := (SCREENWIDTH - m_HGEFont.TextWidth(S)) div 2
      else
        nX := m_nX;
      nY := m_OffsetY2;
      for I := 0 to TokenLine.Count - 1 do begin
        Token := TokenLine.Tokens[I];
        BoldTextOutEx(m_HGEFont, nX, nY, Token.Text, Token.FColor, Token.BColor, m_Alpha2);
        nX := nX + m_HGEFont.TextWidth(Token.Text);
      end;
    end;
  finally
    m_ShowLines.UnLock;
  end;
end;

procedure TDrawScreen.SetShowLoginSceneShowRandomCodeDlg;
begin
  m_boShowLoginSceneShowRandomCodeDlg := True;
end;

end.
