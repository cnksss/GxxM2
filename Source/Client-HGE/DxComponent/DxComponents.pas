unit DxComponents;

interface
uses
  Types,
  Classes,
  SysUtils,
  StdCtrls,
  Graphics,
  Controls,
  ComCtrls,
  Grids,
  MsCTF,
  imm,
  Forms;

type
  TSaveUIColor = record
    Up:TColor;
    Hot:TColor;
    Down:TColor;
    Disabled:TColor;
  end;

type
  // 176   185   英雄版  连击版   传奇续章     {外传     归来}
  TClientVersion = (cv176, cv185, cvHero, cvSerial, cvMirSequel, {cvMirs, cvMirReturn,} {cvMirReturn2} cvMirNewUI205);
  TReferenceX = (rxLeft, rxCenter, rxRight);

  TButtonAnimationShowType = (astAlwaysShow, astNormalShow, astHotShow, astDownShow, astCheckShow, astEnableShow, astDisableShow);

  // 1      2          3        4       5        6        7         8            9            10            11            12         13         14     15
  TGuiType = (t_None, t_Form, t_Button, t_Edit, t_Label, t_Grid,
    t_ScrollBox, t_ChatMemo, t_PopupMenu, t_ComboBox, t_PageControl, t_TabSheet,
    t_TreeView, t_ListView, t_Line, t_FormShape, t_ImageEdit,
    t_TrackBar, t_MainBottomForm, t_MagicBall, t_SexPanel, t_GroupAttackProgress, t_ImageProgress, t_SwitchButton); //

  // 添加自定义图库支持 piaoyun 2013-10-24
  TImageType = (Prguse_wil, Prguse2_wil, Prguse3_wil, ChrSel_wil, Prguse_16_wil, Prguse2_16_wil, Prguse3_16_wil, ChrSel_16_wil,
    UI_wil, UI1_wil, UI2_wil, Prguse_wis, Prguse2_wis, Prguse3_wis, NewopUI_Pak, UI3_wil, UIN_wil, NSelect_wil, UICommon_wil,
    NewUI1_PAK, NewUI2_PAK, NewUI3_PAK, NewUI4_PAK, NewUI5_PAK, Mobile_Pak);
  //  {$IFDEF BEIJING}, Mobile_Pak{$ENDIF});

  TMouseEvents = set of TMouseButton;

  TMagicBallType = (mbtHPMP, mbtHP, mbtMP);
  TMagicBallValueAlignment = (mbaLeft, mbaRight, mbaTop, mbaBottom);
  TProgressValueType = (vtNone, vtValueAndMax, vtValue, vtPercentage);
  TDrawAligment = (daFill, daBottom);

  TLineStyle = (lsNone, lsHorizontal, lsVertical, lsCircle, lsTriangle);
  TInValue = (vInteger, vString);
  TButtonStyle = (bsButton, bsRadio, bsCheckBox);
  TClickSound = (csNone, csStone, csGlass, csNorm);

  TNotifyEvent = procedure(Sender:TObject) of object; stdcall;
  TMouseEvent = procedure(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer) of object; stdcall;
  TMouseMoveEvent = procedure(Sender:TObject; Shift:TShiftState; X, Y:Integer) of object; stdcall;
  TKeyEvent = procedure(Sender:TObject; var Key:Word; Shift:TShiftState) of object; stdcall;
  TKeyPressEvent = procedure(Sender:TObject; var Key:Char) of object; stdcall;

  TChangeQueryEvent = procedure(Sender:TObject; var CanClose:Boolean) of object; stdcall;
  TGetHumAbilityEvent = procedure(Sender:TObject; var Job:Byte; var Level, HP, MaxHP, MP, MaxMP:LongWord) of object; stdcall;
  TAfterDrawMagicBallAreaEvent = procedure(Sender:TObject; IsHP:Boolean; Rect:TRect) of object; stdcall;

  TGetGroupAttackProgressEvent = procedure(Sender:TObject; var IsShowGroupAttack, IsShowContinueAttack:Boolean; var nGroupAttackProgress:Integer; var IsContinueFlash:Boolean) of object; stdcall;

  TAnimationFrameChangedEvent = procedure(Sender:TObject; AnimationIndex:Integer; PlayCount:Integer; Frame:Integer) of object; stdcall;

  TItemClickEvent = procedure(Sender:TObject; ItemName:string; MakeIndex:Integer; R:TRect) of object; stdcall;

  TOnClickSound = procedure(Sender:TObject; ClickSound:TClickSound) of object; stdcall;

  TOnClickEx = procedure(Sender:TObject; X, Y:Integer) of object; stdcall;
  TOnInRealArea = procedure(Sender:TObject; X, Y:Integer; var IsRealArea:Boolean) of object; stdcall;
  TOnGetImage = procedure(Sender:TObject; ImageType:TImageType; var AImage:TObject) of object;
  TOnGridMove = procedure(Sender:TObject; ACol, ARow:Integer; Shift:TShiftState) of object; stdcall;
  TOnGridSelect = procedure(Sender:TObject; ACol, ARow:Integer; Button:TMouseButton; Shift:TShiftState) of object; stdcall;
  TOnGridPaint = procedure(Sender:TObject; ACol, ARow:Integer; Rect:TRect; State:TGridDrawState) of object; stdcall;
  TOnViewItemPaint = procedure(Sender:TObject; ACol, ARow:Integer; Rect:TRect; ViewItem:Pointer; var PaintOverride:Boolean) of object; stdcall;
  TOnListItem = procedure(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer) of object; stdcall;

  TOnListItemMouseDown = procedure(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer; Select:Boolean) of object; stdcall;
  TOnListItemMouseMove = procedure(Sender:TObject; Shift:TShiftState; X, Y:Integer; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer; Select:Boolean) of object; stdcall;

  TTreeNodeEventEvent = procedure(Sender:TObject; TreeNode:TObject) of object; stdcall;

  TAlignEx = (alxNone, alxTop, alxBottom, alxLeft, alxRight, alxClient, alxTopLeft, alxTopRight, alxBottomLeft, alxBottomRight);

  TSelection = record
    StartPos, EndPos:Integer;
  end;

  TGuiFileHeader = packed record
    sDesc:string[$23];
    ClientVersion:TClientVersion;
    Reserve:Byte;
    GroupCount:Word;
    nGuiVersion:Integer;
    nCount:Integer;
    dCreateDate:TDateTime;
  end;
  pTGuiFileHeader = ^TGuiFileHeader;

  TDebugOutPro = procedure(Msg:string);

  TGuiFont = record
    Color:TColor;
    BColor:TColor;
    Style:TFontStyles;
    Size:Integer;
    Bold:Boolean;
    NameLen:Integer;
  end;

  TGuiCaptionColor = record
    Up:TGuiFont;
    Hot:TGuiFont;
    Down:TGuiFont;
    Disabled:TGuiFont;
  end;

  TGuiImageIndex = record
    Image:TImageType;
    Up:Integer;
    Hot:Integer;
    Down:Integer;
    Disabled:Integer;
  end;

  TGuiImageIndex_Form = record
    Image:TImageType;
    Up:Integer;
    OffsetX:Integer;
    OffsetY:Integer;
  end;

  TGuiImageIndex_Button = record
    Image:TImageType;
    Up:Integer;
    Hot:Integer;
    Down:Integer;
    Disabled:Integer;
    Checked:Integer;
  end;

  TGuiHeader = record
    Gui:TGuiType;
    Left:Integer;
    Top:Integer;
    Width:Integer;
    Height:Integer;
    Enabled:Boolean;
    Visible:Boolean;

    Transparent:Boolean;
    EnableFocus:Boolean;
    Floating:Boolean;
    OwnerMove:Boolean;

    MouseEvents:TMouseEvents;

    NameLen:Integer;
    Background:Integer;
    Count:Integer;
  end;
  pTGuiHeader = ^TGuiHeader;

  TGuiHeaderAdd = record
    ShowNameLen:Integer;
    ReferenceX:TReferenceX;
    AdjustYByHeight:Boolean;
    TopAlignment:Boolean;
    Reserverd:Byte;
    HintTextLen:Word;
    Reserverd2:Word;
    Reserve:array[0..8] of Integer;
  end;

  TGuiImageForm = record
    AutoSize:Boolean;
    ImageIndex:TGuiImageIndex;
    Center:Boolean;
  end;
  pTGuiImageForm = ^TGuiImageForm;

  TGuiImageForm_New = record
    AutoSize:Boolean;
    ImageIndex:TGuiImageIndex;
    Center:Boolean;
    BackgroundAlpha:Byte;
    BackgroundColor:TColor;
    Reserve:array[0..3] of Integer;
  end;
  pTGuiImageForm_New = ^TGuiImageForm_New;

  TGuiAnimation = record
    ImageType:TImageType;
    DrawBeforeDef:Boolean;
    Reserve:Word;

    StartIndex:Integer;
    EndIndex:Integer;
    FrameTime:Integer;
    PlayCount:Integer;

    OffsetX:Integer;
    OffsetY:Integer;

    UseImageOffset:Boolean;
    OutsideAreaDraw:Boolean;
    Draw:Boolean;
    BlendDraw:Boolean;
  end;

  TGuiImageForm_New2 = record
    AutoSize:Boolean;
    ImageIndex:TGuiImageIndex;
    Center:Boolean;
    BackgroundAlpha:Byte;
    BackgroundColor:TColor;

    Animation1:TGuiAnimation;
    Animation2:TGuiAnimation;
    Animation3:TGuiAnimation;

    ImageOffsetX:Integer;
    ImageOffsetY:Integer;
    Reserve:array[0..17] of Integer;
  end;
  pTGuiImageForm_New2 = ^TGuiImageForm_New2;

  TGuiImageForm_New3 = record
    AutoSize:Boolean;
    ImageIndex:TGuiImageIndex_Form;
    Center:Boolean;
    BackgroundAlpha:Byte;
    BackgroundColor:TColor;

    Animation1:TGuiAnimation;
    Animation2:TGuiAnimation;
    Animation3:TGuiAnimation;
    Reserve:array[0..19] of Integer;
  end;
  pTGuiImageForm_New3 = ^TGuiImageForm_New3;

  TGuiFormShapeInfo = record
    ImageType:TImageType; // 图库
    ImageIndex:Integer; // 图片序号
    Draw:Boolean; // 是否绘制
    Stretch:Boolean; // 缩放绘制模式
    Center:Boolean; // 绘制在中间
    BlendMode:Integer;
    Align:TAlignEx;
    SrcRect:TRect; // 需要绘制图片矩形
    DestRect:TRect; // 绘制在目标矩形
  end;
  pTGuiFormShapeInfo = ^TGuiFormShapeInfo;
  TGuiFormShapeInfoArray = array[0..8 - 1] of TGuiFormShapeInfo;

  TGuiImageFormShape = record
    AutoSize:Boolean;
    ImageIndex:TGuiImageIndex;
    Center:Boolean;

    ImageIndexs:TGuiFormShapeInfoArray;
  end;
  pTGuiImageFormShape = ^TGuiImageFormShape;

  TGuiImageButton = record
    Alignment:TAlignment;
    ImageIndex:TGuiImageIndex;
    AutoSize:Boolean;
    CaptionColor:TGuiCaptionColor;
    Checked:Boolean;
    ClickCount:TClickSound;
    Style:TButtonStyle;

    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;
    CaptionLen:Integer;
  end;
  pTGuiImageButton = ^TGuiImageButton;

  TGuiImageButton_New2 = record
    Alignment:TAlignment;
    ImageIndex:TGuiImageIndex_Button;
    AutoSize:Boolean;
    CaptionColor:TGuiCaptionColor;
    Checked:Boolean;
    ClickCount:TClickSound;
    Style:TButtonStyle;

    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;

    CaptionOffsetX:Integer;
    CaptionOffsetY:Integer;

    CaptionLen:Integer;
  end;
  pTGuiImageButton_New2 = ^TGuiImageButton_New2;

  TGuiButtonAnimation = record
    ImageType:TImageType;
    DrawBeforeDef:Boolean;
    Reserve:Word;

    ShowType:TButtonAnimationShowType;

    StartIndex:Integer;
    EndIndex:Integer;
    FrameTime:Integer;
    PlayCount:Integer;

    OffsetX:Integer;
    OffsetY:Integer;
    UseImageOffset:Boolean;

    OutsideAreaDraw:Boolean;
    Draw:Boolean;
    BlendDraw:Boolean;
  end;

  TGuiImageButton_New3 = record
    Alignment:TAlignment;
    ImageIndex:TGuiImageIndex_Button;
    AutoSize:Boolean;
    CaptionColor:TGuiCaptionColor;
    Checked:Boolean;
    ClickCount:TClickSound;
    Style:TButtonStyle;

    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;

    CaptionOffsetX:Integer;
    CaptionOffsetY:Integer;

    Animation:TGuiButtonAnimation;

    CaptionLen:Integer;
  end;
  pTGuiImageButton_New3 = ^TGuiImageButton_New3;

  TGuiTrackBar = record
    ImageIndex:TGuiImageIndex;
    SliderIndex:TGuiImageIndex;
    AutoSize:Boolean;
    Min:Integer;
    Max:Integer;
    Position:Integer;
  end;

  TGuiMainBottomCenter = record
    Height:Integer;
    MinHeight:Integer;
    MaxHeight:Integer;

    OffsetLeft:Integer;
    OffsetRight:Integer;

    DragHeightOffsetY:Integer;
    DragHeightSize:Integer;

    AutoStretchSize:Boolean;
    StretchImageFillCenterAlpha:Byte;
    StretchImageFillCenterColor:TColor;
    StretchImageFillCenterExpandHorz:Integer;
    StretchImageFillCenterExpandVert:Integer;

    StretchImageType:TImageType;
    StretchImageUpLeft:Integer;
    StretchImageUp:Integer;
    StretchImageUpRight:Integer;
    StretchImageLeft:Integer;
    StretchImageRight:Integer;
    StretchImageDownLeft:Integer;
    StretchImageDown:Integer;
    StretchImageDownRight:Integer;

    FillImageType:TImageType;
    FillImageIndex:Integer;

    Reserve:array[0..2] of Integer;
  end;

  TGuiMainBottomForm = record
    LeftImageType:TImageType;
    LeftImageIndex:Integer;
    RightImageType:TImageType;
    RightImageIndex:Integer;

    Center:TGuiMainBottomCenter;
    Reserve:array[0..3] of Integer;
  end;

  TGuiMainBottomFormAnimation = record
    ImageType:TImageType;
    DrawBeforeDef:Boolean;
    Reserve:Word;

    StartIndex:Integer;
    EndIndex:Integer;
    FrameTime:Integer;
    PlayCount:Integer;

    OffsetX:Integer;
    OffsetY:Integer;
    UseImageOffset:Boolean;

    OutsideAreaDraw:Boolean;
    Draw:Boolean;
    BlendDraw:Boolean;

    HorzAlignment:TAlignment;
    VertAlignment:TVerticalAlignment;
    AdjustYByHeight:Boolean;

    Reseved:array[0..16] of Byte;
  end;

  TGuiMainBottomForm_New = record
    LeftImageType:TImageType;
    LeftImageIndex:Integer;
    RightImageType:TImageType;
    RightImageIndex:Integer;

    Center:TGuiMainBottomCenter;
    Animation1:TGuiMainBottomFormAnimation;
    Animation2:TGuiMainBottomFormAnimation;
    Animation3:TGuiMainBottomFormAnimation;
    Animation4:TGuiMainBottomFormAnimation;
    Reserve:array[0..3] of Integer;
  end;

  TGuiMainBottomForm_New2 = record
    LeftImageType:TImageType;
    LeftImageIndex:Integer;
    RightImageType:TImageType;
    RightImageIndex:Integer;
    BottomImageType:TImageType;
    BottomImageIndex:Integer;

    Center:TGuiMainBottomCenter;
    Animation1:TGuiMainBottomFormAnimation;
    Animation2:TGuiMainBottomFormAnimation;
    Animation3:TGuiMainBottomFormAnimation;
    Animation4:TGuiMainBottomFormAnimation;
    Reserve:array[0..3] of Integer;
  end;

  TGuiMagicBall = record
    BallType:TMagicBallType;
    ValueAlignment:TMagicBallValueAlignment;

    Overall_ImageType:TImageType;

    Overall_EmptyHPMP:Integer;
    Overall_FullHPMP:Integer;

    Overall_EmptyHP:Integer;
    Overall_FullHP:Integer;

    Overall_Splite:Integer;
    Overall_MiddleZoneWidth:Integer;

    Alone_ImageType:TImageType;
    Alone_Empty:Integer;
    Alone_Full:Integer;

    Reserve:array[0..9] of Integer;
  end;

  TGuiMagicBall2 = record
    BallType:TMagicBallType;
    ValueAlignment:TMagicBallValueAlignment;

    Overall_ImageType:TImageType;

    Overall_EmptyHPMP:Integer;
    Overall_FullHPMP:Integer;

    Overall_EmptyHP:Integer;
    Overall_FullHP:Integer;

    Overall_Splite:Integer;
    Overall_MiddleZoneWidth:Integer;

    Overall_EffectDrawBlend:Boolean;
    Overall_EffectImageType:TImageType;
    Overall_EffectHPMPStart:Integer;
    Overall_EffectHPStart:Integer;
    Overall_EffectImageCount:Integer;
    Overall_EffectPlayInterval:Integer;

    Alone_ImageType:TImageType;
    Alone_Empty:Integer;
    Alone_Full:Integer;

    Alone_EffectDrawBlend:Boolean;
    Alone_EffectImageType:TImageType;
    Alone_EffectStart:Integer;
    Alone_EffectImageCount:Integer;
    Alone_EffectPlayInterval:Integer;

    Reserve:array[0..9] of Integer;
  end;

  TGuiSexPanel = record
    IsMale:Boolean;
    UseSettign2:Boolean;

    ImageType:TImageType;
    Male:Integer;
    Female:Integer;

    ImageType2:TImageType;
    Male2:Integer;
    Female2:Integer;

    Reserve:array[0..7] of Integer;
  end;

  TGuiGroupAttackProgressSetting = record
    ImageType:TImageType;
    Background:Integer;
    Progress:Integer;
    FlashStart:Integer;
    FlashEnd:Integer;
    FlashInterval:Integer;
    OffsetX1:Integer;
    OffsetY1:Integer;
    OffsetX2:Integer;
    OffsetY2:Integer;
    OffsetX3:Integer;
    OffsetY3:Integer;
  end;

  TGuiGroupAttackProgress = record
    ProgressAlignment:TMagicBallValueAlignment;
    Settings:array[0..2] of TGuiGroupAttackProgressSetting;
    Reserve:array[0..13] of Integer;
  end;

  TGuiImageProgress = record
    AutoSize:Boolean;

    ImageType:TImageType;

    ImageBG:Integer;
    ImageProgress:Integer;
    ImageProgressX:Integer;
    ImageProgressY:Integer;

    ValueType:TProgressValueType;
    ValueSplite:string[20];

    ValueAlignment:TAlignment;
    ValuePrefix:string[60];
    ValueSuffix:string[60];

    Max:LongWord;
    Min:LongWord;
    Value:LongWord;

    Font:TGuiFont;

    Reserve:array[0..7] of Integer;
  end;

  TGuiSwitchButtonSetting = record
    ImageIndex:TGuiImageIndex;
    CaptionColor:TGuiCaptionColor;
    ClickSound:TClickSound;
    Alignment:TAlignment;

    CaptionOffsetX:Integer;
    CaptionOffsetY:Integer;

    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;
    ButtonDownOffsetX:Integer;
    ButtonDownOffsetY:Integer;

    DrawAligment:TDrawAligment;
    CaptionLen:Integer;

    Reserve:array[0..7] of Integer;
  end;

  TGuiSwitchButton = record
    AutoSize:Boolean;
    CloseSetting:TGuiSwitchButtonSetting;
    OpenSetting:TGuiSwitchButtonSetting;

    Reserve:array[0..7] of Integer;
  end;

  TGuiLabel = record
    AutoSize:Boolean;
    DrawBorder:Boolean;
    BackgroundColor:TColor;
    BorderColor:TGuiCaptionColor;
    CaptionColor:TGuiCaptionColor;
    ClickCount:TClickSound;
    Style:TButtonStyle;
    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;
    CaptionLen:Integer;
  end;
  pTGuiLabel = ^TGuiLabel;

  TGuiLabel_New = record
    AutoSize:Boolean;
    Alignment:TAlignment;
    DrawBorder:Boolean;
    BackgroundColor:TColor;
    BorderColor:TGuiCaptionColor;
    CaptionColor:TGuiCaptionColor;
    ClickCount:TClickSound;
    Style:TButtonStyle;
    CaptionDownOffsetX:Integer;
    CaptionDownOffsetY:Integer;
    CaptionLen:Integer;
  end;
  pTGuiLabel_New = ^TGuiLabel_New;

  TGuiEdit = record
    DrawBorder:Boolean;
    SelectedColor:TColor;
    SelBackColor:TColor;
    SelFontColor:TColor;
    BackgroundColor:TColor;
    FontColor:TGuiFont;
    BorderColor:TGuiCaptionColor;

    ReadOnly:Boolean;
    MaxLength:Integer;

    InValue:TInValue;
    PasswordChar:Char;
    AllowSelect:Boolean;
    AllowPaste:Boolean;
    TabOrder:Integer;
    TextLen:Integer;
  end;
  pTGuiEdit = ^TGuiEdit;

  TGuiImageEdit = record
    DrawBorder:Boolean;
    SelectedColor:TColor;
    SelBackColor:TColor;
    SelFontColor:TColor;

    BackgroundColor:TColor;
    DisableBackgroundColor:TColor;
    HintTextFont:TGuiFont;
    HintTextAlignment:TAlignment;

    FontColor:TGuiFont;
    BorderColor:TGuiCaptionColor;

    ReadOnly:Boolean;
    MaxLength:Integer;

    InValue:TInValue;
    PasswordChar:Char;
    AllowSelect:Boolean;
    AllowPaste:Boolean;
    TabOrder:Integer;
    TextLen:Integer;

    HintTextLen:Integer;
    Reserve:array[0..5] of Integer;
  end;
  pTGuiImageEdit = ^TGuiImageEdit;

  TBackgroundImage = record
    ImageType:TImageType; // 图库
    BlendMode:Boolean; // 透明绘制
    OutsideAreaDraw:Boolean; // 区域外绘制
    Reserve:Boolean;
    ImageIndex:Integer; // 图片序号
    OffsetX:Integer;
    OffsetY:Integer;
  end;

  TGuiImageEdit_New = record
    DrawBorder:Boolean;
    SelectedColor:TColor;
    SelBackColor:TColor;
    SelFontColor:TColor;

    BackgroundColor:TColor;
    BackgroundColorAlpha:Byte;
    BackgroundImage:TBackgroundImage;

    DisableHideCtrl:Boolean;
    DisableBackgroundTransparent:Boolean;
    DisableBackgroundColor:TColor;
    DisableBackgroundAlpha:Byte;
    DisableBackgroundImage:TBackgroundImage;

    HintTextFont:TGuiFont;
    HintTextAlignment:TAlignment;

    FontColor:TGuiFont;
    BorderColor:TGuiCaptionColor;

    ReadOnly:Boolean;
    MaxLength:Integer;

    InValue:TInValue;
    PasswordChar:Char;
    AllowSelect:Boolean;
    AllowPaste:Boolean;
    TabOrder:Integer;
    TextLen:Integer;

    HintTextLen:Integer;

    Reserve:array[0..9] of Integer;
  end;
  pTGuiImageEdit_New = ^TGuiImageEdit_New;

  TGuiImageCheckBox = record
    Button:TGuiImageButton;
    Checked:Boolean;
  end;
  pTGuiImageCheckBox = ^TGuiImageCheckBox;

  TGuiImageGrid = record
    ColCount:Integer;
    RowCount:Integer;
    ColWidth:Integer;
    RowHeight:Integer;
    ViewTopLine:Integer;
  end;
  pTGuiImageGrid = ^TGuiImageGrid;

  TGuiMemo = record
    ImageIndex:TGuiImageIndex;
    ScrollImageIndex:TGuiImageIndex;
    PrevImageIndex:TGuiImageIndex;
    NextImageIndex:TGuiImageIndex;
    BarImageIndex:TGuiImageIndex;

    ShowScroll:Boolean;
    ItemHeight:Integer;
    ItemIndex:Integer;
    ScrollBars:TScrollStyle;
    ScrollSize:Integer;

    ExpandSize:Integer;
    Position:Integer;
    VisibleItemCount:Integer;

    ShowButton:Boolean;
    OffSetX:Integer;
    OffSetY:Integer;

    ColCount:Integer; // TDxListView   有效
    ShowItemCount:Integer; // TDxListView   有效
    ShowGridLine:Boolean; // TDxListView   有效
    GridLineColor:TColor; // TDxListView  有效
    CheckItemControlSize:Boolean;
    Reserve:Integer;
  end;
  pTGuiMemo = ^TGuiMemo;

  TGuiMemo_New = record
    ImageIndex:TGuiImageIndex;
    ScrollImageIndex:TGuiImageIndex;
    PrevImageIndex:TGuiImageIndex;
    NextImageIndex:TGuiImageIndex;
    BarImageIndex:TGuiImageIndex;

    ShowScroll:Boolean;
    ItemHeight:Integer;
    ItemIndex:Integer;
    ScrollBars:TScrollStyle;
    ScrollSize:Integer;

    ExpandSize:Integer;
    Position:Integer;
    VisibleItemCount:Integer;

    ShowButton:Boolean;
    OffSetX:Integer;
    OffSetY:Integer;

    ColCount:Integer; // TDxListView   有效
    ShowItemCount:Integer; // TDxListView   有效
    ShowGridLine:Boolean; // TDxListView   有效
    GridLineColor:TColor; // TDxListView  有效
    CheckItemControlSize:Boolean;
    BackGroupColor:Integer;

    FontBackTransparent:Boolean;
    FontLen:Integer;
    FontSize:Integer;
    FontStroke:Boolean;

    Reserve:array[0..9] of Integer;
  end;
  pTGuiMemo_New = ^TGuiMemo_New;

  TGuiViewField = record
    Color:TGuiCaptionColor;
    Alignment:TAlignment;
    CaptionLen:Integer;
  end;
  pTGuiViewField = ^TGuiViewField;

  TGuiPopupMenu = record
    DrawBorder:Boolean;
    SelectColor:TColor;
    BackgroundColor:TColor;
    ItemColor:TGuiCaptionColor;
    BorderColor:TGuiCaptionColor;
    ItemHeight:Integer;
    ItemIndex:Integer;
    ItemTextLen:Integer;
  end;
  pTGuiPopupMenu = ^TGuiPopupMenu;

  TGuiTabSheet = record
    OffSetX:Integer;
    OffSetY:Integer;
    HideTable:Boolean;
    Reserve1:array[0..2] of Byte;
    Reserve2:Integer;
    CaptionColor:TGuiCaptionColor;
    ImageIndex:TGuiImageIndex;
    CaptionLen:Integer;
  end;
  pTGuiTabSheet = ^TGuiTabSheet;

  TGuiPageControl = record
    ShowButton:Boolean;
    ClientLeft:Integer;
    ClientTop:Integer;
    ClientWidth:Integer;
    ClientHeight:Integer;
    TabPosition:TTabPosition;
    PageCount:Integer;
    ActivePageIndex:Integer;
    ButtonWidth:Integer;
    ButtonHeight:Integer;
    OffSetX:Integer;
    OffSetY:Integer;
  end;
  pTGuiPageControl = ^TGuiPageControl;

  TGuiPageControl_New = record
    ShowButton:Boolean;
    ClientLeft:Integer;
    ClientTop:Integer;
    ClientWidth:Integer;
    ClientHeight:Integer;
    TabPosition:TTabPosition;
    PageCount:Integer;
    ActivePageIndex:Integer;
    ButtonWidth:Integer;
    ButtonHeight:Integer;
    OffSetX:Integer;
    OffSetY:Integer;

    CaptionOffsetX:Integer;
    CaptionOffsetY:Integer;
    DownCaptionOffsetX:Integer;
    DownCaptionOffsetY:Integer;

    ReverseDrawButton:Boolean; // // 逆序绘制Page按钮
    Reserve1:array[0..2] of Byte;

    Reserve2:array[0..18] of Integer;
  end;
  pTGuiPageControl_New = ^TGuiPageControl_New;

  TGuiComboBox = record
    GuiPopupMenu:TGuiPopupMenu;
    DrawBorder:Boolean;
    ButtonColor:TColor;
    BackgroundColor:TColor;
    TextColor:TGuiCaptionColor;
    BorderColor:TGuiCaptionColor;
    TextLen:Integer;
    ItemLen:Integer;
  end;
  pTGuiComboBox = ^TGuiComboBox;

  TGuiLine = record
    LineColor:TGuiCaptionColor;
    LineStyle:TLineStyle;
  end;
  pTGuiLine = ^TGuiLine;

function ShortRect(const Rect1, Rect2:TRect):TRect;
function LongRect(const Rect1, Rect2:TRect):TRect;
function MoveRect(const Rect:TRect; const Point:TPoint):TRect;
function PointInRect(const Point:TPoint; const Rect:TRect):Boolean;
function ShrinkRect(const Rect:TRect; const hIn, vIn:Integer):TRect;

function RectInRect(const Rect1, Rect2:TRect):Boolean;

function OverlapRect(const Rect1, Rect2:TRect):Boolean;
procedure DebugOut(Msg:string);

function GetDefaultCursor:Integer;

var
  g_CustomCursorIndex:Integer = -1;

  OnDebugOut:TDebugOutPro = nil;

  g_IMM32DLL:THandle = 0;
  g_ImmGetOpenStatus:function(hImc:HIMC):Boolean stdcall = nil;

procedure OpenIme;
procedure CloseIme;

implementation

uses Math;

procedure DebugOut(Msg:string);
begin
  if Assigned(OnDebugOut) then OnDebugOut(Msg);
end;
// ---------------------------------------------------------------------------

function ShortRect(const Rect1, Rect2:TRect):TRect;
begin
  Result.Left := Max(Rect1.Left, Rect2.Left);
  Result.Top := Max(Rect1.Top, Rect2.Top);
  Result.Right := Min(Rect1.Right, Rect2.Right);
  Result.Bottom := Min(Rect1.Bottom, Rect2.Bottom);
end;
// ---------------------------------------------------------------------------

function LongRect(const Rect1, Rect2:TRect):TRect;
begin
  Result.Left := Min(Rect1.Left, Rect2.Left);
  Result.Top := Min(Rect1.Top, Rect2.Top);
  Result.Right := Max(Rect1.Right, Rect2.Right);
  Result.Bottom := Max(Rect1.Bottom, Rect2.Bottom);
end;
// ---------------------------------------------------------------------------

function MoveRect(const Rect:TRect; const Point:TPoint):TRect;
begin
  Result.Left := Rect.Left + Point.X;
  Result.Top := Rect.Top + Point.Y;
  Result.Right := Rect.Right + Point.X;
  Result.Bottom := Rect.Bottom + Point.Y;
end;
// ---------------------------------------------------------------------------

function PointInRect(const Point:TPoint; const Rect:TRect):Boolean;
begin
  Result := (Point.X >= Rect.Left) and (Point.X <= Rect.Right) and
    (Point.Y >= Rect.Top) and (Point.Y <= Rect.Bottom);
end;

// ---------------------------------------------------------------------------

function ShrinkRect(const Rect:TRect; const hIn, vIn:Integer):TRect;
begin
  Result.Left := Rect.Left + hIn;
  Result.Top := Rect.Top + vIn;
  Result.Right := Rect.Right - hIn;
  Result.Bottom := Rect.Bottom - vIn;
end;

// ---------------------------------------------------------------------------

function RectInRect(const Rect1, Rect2:TRect):Boolean;
begin
  Result := (Rect1.Left >= Rect2.Left) and (Rect1.Right <= Rect2.Right) and
    (Rect1.Top >= Rect2.Top) and (Rect1.Bottom <= Rect2.Bottom);
end;

// ---------------------------------------------------------------------------

function OverlapRect(const Rect1, Rect2:TRect):Boolean;
begin
  Result := (Rect1.Left < Rect2.Right) and (Rect1.Right > Rect2.Left) and
    (Rect1.Top < Rect2.Bottom) and (Rect1.Bottom > Rect2.Top);
end;

// ---------------------------------------------------------------------------

function GetImeOpen(hWnd:HIMC):Boolean;
var
  IMC:HIMC;
begin
  Result := False;
  if g_IMM32DLL = 0 then Exit;
  IMC := ImmGetContext(hWnd);
  if IMC = 0 then Exit;
  if @g_ImmGetOpenStatus = nil then Exit;
  Result := g_ImmGetOpenStatus(IMC);
end;

procedure SetImeModeEx(hWnd:Cardinal; Mode:TImeMode);
const
  InputSourceModeMap:array[imSAlpha..imHanguel] of InputScope = // flags in use are all < 255
  ({ imSAlpha: } IS_ALPHANUMERIC_HALFWIDTH,
    { imAlpha:  } IS_ALPHANUMERIC_FULLWIDTH,
    { imHira:   } IS_HIRAGANA,
    { imSKata:  } IS_KATAKANA_HALFWIDTH,
    { imKata:   } IS_KATAKANA_FULLWIDTH,
    { imChinese } IS_DEFAULT,
    { imSHanguel} IS_HANJA_HALFWIDTH,
    { imHanguel } IS_HANJA_FULLWIDTH);
  ConversionStatusModeMap:array[imSAlpha..imHanguel] of Byte = // flags in use are all < 255
  ({ imSAlpha: } IME_CMODE_ALPHANUMERIC,
    { imAlpha:  } IME_CMODE_ALPHANUMERIC or IME_CMODE_FULLSHAPE,
    { imHira:   } IME_CMODE_NATIVE or IME_CMODE_FULLSHAPE,
    { imSKata:  } IME_CMODE_NATIVE or IME_CMODE_KATAKANA,
    { imKata:   } IME_CMODE_NATIVE or IME_CMODE_KATAKANA or IME_CMODE_FULLSHAPE,
    { imChinese:} IME_CMODE_NATIVE or IME_CMODE_FULLSHAPE,
    { imSHanguel} IME_CMODE_NATIVE,
    { imHanguel } IME_CMODE_NATIVE or IME_CMODE_FULLSHAPE);
var
  IMC:HIMC;
  Conv, Sent:DWORD;
begin
  if Mode = imDontCare then Exit;
  if g_IMM32DLL = 0 then Exit; // No IMM32.DLL. done.

  if Mode = imDisable then begin
    ImmAssociateContextEx(hWnd, 0, 0);
    Exit;
  end;

  ImmAssociateContextEx(hWnd, 0, $0010);
  if Mode in [imOpen, imClose] then begin
    IMC := ImmGetContext(hWnd);
    if IMC = 0 then Exit;
    ImmGetConversionStatus(IMC, Conv, Sent);
    ImmSetOpenStatus(IMC, Mode = imOpen);
    ImmSetConversionStatus(IMC, Conv, Sent);
    ImmReleaseContext(hWnd, IMC);
    Exit;
  end;

  if IsMSCTFAvailable then
    SetInputScope(hWnd, INputSourceModeMap[Mode])
  else begin
    IMC := ImmGetContext(hWnd);
    if IMC = 0 then Exit;

    ImmSetOpenStatus(IMC, TRUE);
    ImmGetConversionStatus(IMC, Conv, Sent);
    Conv := Conv and (not (IME_CMODE_LANGUAGE or IME_CMODE_FULLSHAPE)) or ConversionStatusModeMap[Mode];
    ImmSetConversionStatus(IMC, Conv, Sent);
    ImmReleaseContext(hWnd, IMC);
  end;
end;

// 打开输入法

procedure OpenIme;
begin
  if @g_ImmGetOpenStatus = nil then Exit;
  {
    if not GetImeOpen(Handle) then
    begin
      SetImeMode(Handle, imOpen);
    end;
  }
  if not GetImeOpen(Application.MainForm.Handle) then begin
    SetImeModeEx(Application.MainForm.Handle, imOpen);
  end;
end;

procedure CloseIme;
begin
  if @g_ImmGetOpenStatus = nil then Exit;
  {
    if GetImeOpen(Handle) then
    begin
      SetImeMode(Handle, imClose);
    end;
  }

  if GetImeOpen(Application.MainForm.Handle) then begin
    SetImeModeEx(Application.MainForm.Handle, imDisable);
  end;
end;

function GetDefaultCursor:Integer;
begin
  if g_CustomCursorIndex <> -1 then
    Result := g_CustomCursorIndex
  else
    Result := crDefault;
end;

end.
