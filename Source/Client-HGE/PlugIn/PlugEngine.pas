unit PlugEngine;

interface
uses
  Windows,
  SysUtils,
  Classes,
  Controls,
  Graphics,
  Dialogs,
  HGE,
  HGEFontEx,
  HGECanvas,
  GameImages,
  Wil,
  Wis,
  Wzl,
  Pak,
  Actor,
  Grobal2,
  EDcode,
  Grids,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxLine,
  DxControls,
  DxComponents,
  SDK,
  Math,
  DxImageEdit,
  DxTrackBar,
  DropItemsMgr;

{$IF Enabled_PlugEngine = 1}

type
  TImagesAPI = record
    GetHandle:function(const FileName:PChar):THandle; stdcall; //获取WIL文件
    Count:function(FileHandle:THandle):Integer; stdcall; //wil图片数
    Read:function(FileHandle:THandle; Index:Integer; var X, Y:Integer):TTexture; stdcall; //获取图片纹理
    Clear:procedure(FileHandle:THandle); stdcall;
  end;
  pTImagesAPI = ^TImagesAPI;

  TTextureAPI = record
    Width:function(Texture:TTexture):Integer; stdcall;
    Height:function(Texture:TTexture):Integer; stdcall;
    Pixels:function(Texture:TTexture; X, Y:Integer):Cardinal; stdcall;
    Lock:function(Texture:TTexture; Rect:TRect; out Bits:Pointer; out Pitch:Integer; ReadOnly:Boolean):Boolean; stdcall;
    Unlock:procedure(Texture:TTexture); stdcall;
  end;
  pTTextureAPI = ^TTextureAPI;

  TDControl = record
    Create:function(Parent:TDxControl; InterfaceType:TGuiType):TDxControl; stdcall;
    InterfaceType:function(D:TDxControl):TGuiType; stdcall; //类型
    Name:function(D:TDxControl):PChar; stdcall; //名称
    Left:function(D:TDxControl):Integer; stdcall; //左
    Top:function(D:TDxControl):Integer; stdcall; //上
    Width:function(D:TDxControl):Integer; stdcall; //宽
    Height:function(D:TDxControl):Integer; stdcall; //高
    Tag:function(D:TDxControl):Integer; stdcall; //标志
    Visible:function(D:TDxControl):Boolean; stdcall; //是否可见
    Enabled:function(D:TDxControl):Boolean; stdcall; //是否可用
    Floating:function(D:TDxControl):Boolean; stdcall; //是否可以移动
    ParentMove:function(D:TDxControl):Boolean; stdcall; //Parent是否可以移动
    EnableFocus:function(D:TDxControl):Boolean; stdcall; //是否可以设置焦点
    AutoSize:function(D:TDxControl):Boolean; stdcall; //是否根据图片尺寸自动调整尺寸
    DrawBorder:function(D:TDxControl):Boolean; stdcall; //绘制边框
    Caption:function(D:TDxControl):PChar; stdcall; //标题
    Alignment:function(D:TDxControl):TAlignment; stdcall; //标题显示位置
    Transparent:function(D:TDxControl):Boolean; stdcall; //是否透明
    BackgroundColor:function(D:TDxControl):TColor; stdcall; //背景色
    PopupMenu:function(D:TDxControl):TDxControl; stdcall; //弹出菜单
    VisibleRect:procedure(D:TDxControl; var Value:TRect); stdcall; //组件可见矩形
    VirtualRect:procedure(D:TDxControl; var Value:TRect); stdcall; //组件真正矩形

    OnShow:function(D:TDxControl):Pointer; stdcall; //显示触发事件
    OnHide:function(D:TDxControl):Pointer; stdcall; //隐藏触发事件
    OnKeyDown:function(D:TDxControl):Pointer; stdcall; //按键按下事件
    OnKeyPress:function(D:TDxControl):Pointer; stdcall; //按键事件
    OnKeyUp:function(D:TDxControl):Pointer; stdcall; //按键弹起事件
    OnClick:function(D:TDxControl):Pointer; stdcall; //单击事件
    OnDblClick:function(D:TDxControl):Pointer; stdcall; //双击事件
    OnMouseDown:function(D:TDxControl):Pointer; stdcall; //鼠标按下事件
    OnMouseMove:function(D:TDxControl):Pointer; stdcall; //鼠标移动事件
    OnMouseUp:function(D:TDxControl):Pointer; stdcall; //鼠标弹起事件
    OnMouseEnter:function(D:TDxControl):Pointer; stdcall; //鼠标进入事件
    OnMouseLeave:function(D:TDxControl):Pointer; stdcall; //鼠标离开事件
    OnInRealArea:function(D:TDxControl):Pointer; stdcall; //检测鼠标坐标事件
    OnPaint:function(D:TDxControl):Pointer; stdcall; //绘制事件
    OnStartPaint:function(D:TDxControl):Pointer; stdcall; //开始绘制事件
    OnStartSubPaint:function(D:TDxControl):Pointer; stdcall; //开始绘制子控件事件
    OnStopPaint:function(D:TDxControl):Pointer; stdcall; //绘制结束事件
    OnPress:function(D:TDxControl):Pointer; stdcall; //运行事件

    //------------------------------------------------------------------------------
    SetName:procedure(D:TDxControl; Value:PChar); stdcall; //名称
    SetLeft:procedure(D:TDxControl; Value:Integer); stdcall;
    SetTop:procedure(D:TDxControl; Value:Integer); stdcall;
    SetWidth:procedure(D:TDxControl; Value:Integer); stdcall;
    SetHeight:procedure(D:TDxControl; Value:Integer); stdcall;
    SetTag:procedure(D:TDxControl; Value:Integer); stdcall;
    SetVisible:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetEnabled:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetFloating:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetParentMove:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetEnableFocus:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetAutoSize:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetDrawBorder:procedure(D:TDxControl; Value:Boolean); stdcall; //绘制边框
    SetCaption:procedure(D:TDxControl; Value:PChar); stdcall;
    SetAlignment:procedure(D:TDxControl; Value:TAlignment); stdcall;
    SetTransparent:procedure(D:TDxControl; Value:Boolean); stdcall; //是否透明
    SetBackgroundColor:procedure(D:TDxControl; Value:TColor); stdcall; //背景色
    SetPopupMenu:procedure(D:TDxControl; Value:TDxControl); stdcall;

    SetDefaultBorderColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认边框颜色
    SetDefaultBorderBold:procedure(D:TDxControl; Value:Boolean); stdcall; //默认边框是否加粗
    SetMouseMoveBorderColor:procedure(D:TDxControl; Value:TColor); stdcall; //边框鼠标移动颜色
    SetMouseMoveBorderBold:procedure(D:TDxControl; Value:Boolean); stdcall; //边框鼠标移动是否加粗
    SetMouseDownBorderColor:procedure(D:TDxControl; Value:TColor); stdcall; //边框鼠标按下颜色
    SetMouseDownBorderBold:procedure(D:TDxControl; Value:Boolean); stdcall; //边框鼠标按下是否加粗
    SetDisabledBorderColor:procedure(D:TDxControl; Value:TColor); stdcall; //边框不可用时颜色
    SetDisabledBorderBold:procedure(D:TDxControl; Value:Boolean); stdcall; //边框不可用时是否加粗

    SetImages:procedure(D:TDxControl; Value:THandle); stdcall;
    SetDefaultImageIndex:procedure(D:TDxControl; Value:Integer); stdcall; //默认图库编号
    SetMouseMoveImageIndex:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标移动图库编号
    SetMouseDownImageIndex:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标按下图库编号
    SetDisabledImageIndex:procedure(D:TDxControl; Value:Integer); stdcall; //不可用时图库编号

    SetOnShow:procedure(Value:TMethod); stdcall;
    SetOnHide:procedure(Value:TMethod); stdcall;
    SetOnKeyDown:procedure(Value:TMethod); stdcall;
    SetOnKeyPress:procedure(Value:TMethod); stdcall;
    SetOnKeyUp:procedure(Value:TMethod); stdcall;
    SetOnClick:procedure(Value:TMethod); stdcall;
    SetOnDblClick:procedure(Value:TMethod); stdcall;
    SetOnMouseDown:procedure(Value:TMethod); stdcall;
    SetOnMouseMove:procedure(Value:TMethod); stdcall;
    SetOnMouseUp:procedure(Value:TMethod); stdcall;
    SetOnMouseEnter:procedure(Value:TMethod); stdcall;
    SetOnMouseLeave:procedure(Value:TMethod); stdcall;
    SetOnInRealArea:procedure(Value:TMethod); stdcall;
    SetOnPaint:procedure(Value:TMethod); stdcall;
    SetOnStartPaint:procedure(Value:TMethod); stdcall;
    SetOnStartSubPaint:procedure(Value:TMethod); stdcall;
    SetOnStopPaint:procedure(Value:TMethod); stdcall;
    SetOnPress:procedure(Value:TMethod); stdcall;
    //-----------------------------------------------------------------------------
    Parent:function(D:TDxControl):TDxControl; stdcall; //显示在主控件 为NIL时为主表面
    SetParent:procedure(D, Value:TDxControl); stdcall; //设置显示在主控件 为NIL时为主表面
    ControlCount:function(D:TDxControl):Integer; stdcall; //子控件数
    Controls:function(D:TDxControl; Index:Integer):TDxControl; stdcall; //获取子控件

    SetFocus:procedure(D:TDxControl); stdcall; //设置为焦点
    BringToFront:procedure(D:TDxControl); stdcall; //显示到最上层
    InRange:function(D:TDxControl; X, Y:Integer):Boolean; stdcall; //检测鼠标是否在范围

    // add chongchong 2014-04-14
    GetImages:function(D:TDxControl):THandle; stdcall;
    GetDefaultImageIndex:function(D:TDxControl):Integer; stdcall; //默认图库编号
    GetMouseMoveImageIndex:function(D:TDxControl):Integer; stdcall; //鼠标移动图库编号
    GetMouseDownImageIndex:function(D:TDxControl):Integer; stdcall; //鼠标按下图库编号
    GetDisabledImageIndex:function(D:TDxControl):Integer; stdcall; //不可用时图库编号
  end;
  pTDControl = ^TDControl;

  TDWindow = record
    //-----------------------------------------------------------------------------
    SetIsBringToFront:procedure(D:TDxControl; Value:Boolean); stdcall; //单击窗体是否显示到最上层
    //-----------------------------------------------------------------------------
    ShowModalEx:function(D:TDxControl):Integer; stdcall;
    ShowModalA:function(D:TDxControl):Integer; stdcall;
    ShowModalB:function(Value:TMethod):Integer; stdcall;
  end;
  pTDWindow = ^TDWindow;

  TDButton = record
    Style:function(D:TDxControl):TButtonStyle; stdcall; //按钮样式
    Checked:function(D:TDxControl):Boolean; stdcall;
    //-----------------------------------------------------------------------------
    SetDefaultCaptionFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
    SetDefaultCaptionFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
    SetDefaultCaptionFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
    SetDefaultCaptionFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
    SetDefaultCaptionFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //默认字体是否描边
    SetDefaultCaptionFontName:procedure(D:TDxControl; Value:PChar); stdcall; //默认字体名称

    SetMouseMoveCaptionFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
    SetMouseMoveCaptionFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
    SetMouseMoveCaptionFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
    SetMouseMoveCaptionFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
    SetMouseMoveCaptionFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标移动字体是否描边
    SetMouseMoveCaptionFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称

    SetMouseDownCaptionFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
    SetMouseDownCaptionFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
    SetMouseDownCaptionFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
    SetMouseDownCaptionFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
    SetMouseDownCaptionFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标按下字体是否描边
    SetMouseDownCaptionFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称

    SetDisabledCaptionFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
    SetDisabledCaptionFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
    SetDisabledCaptionFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
    SetDisabledCaptionFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
    SetDisabledCaptionFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //不可用时字体是否描边
    SetDisabledCaptionFontName:procedure(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称

    SetCheckedCaptionFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //勾选时标题字体颜色
    SetCheckedCaptionFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //勾选时标题字体描边颜色
    SetCheckedCaptionFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //勾选时字体样式
    SetCheckedCaptionFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //勾选时字体尺寸
    SetCheckedCaptionFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //勾选时字体是否描边
    SetCheckedCaptionFontName:procedure(D:TDxControl; Value:PChar); stdcall; //勾选时字体名称

    SetStyle:procedure(D:TDxControl; Value:TButtonStyle); stdcall; //按钮样式
    SetChecked:procedure(D:TDxControl; Value:Boolean); stdcall;
    SetCaptionDownOffsetX:procedure(D:TDxControl; Value:Integer); stdcall; //按下后标题偏移X
    SetCaptionDownOffsetY:procedure(D:TDxControl; Value:Integer); stdcall; //按下后标题偏移Y
    SetButtonDownOffsetX:procedure(D:TDxControl; Value:Integer); stdcall; //按下后偏移X
    SetButtonDownOffsetY:procedure(D:TDxControl; Value:Integer); stdcall; //按下后偏移Y
    SetClickCount:procedure(D:TDxControl; Value:TClickSound); stdcall; //按钮点击的声音
  end;
  pTDButton = ^TDButton;

  TDEdit = record
    Text:function(D:TDxControl):PChar; stdcall;
    Value:function(D:TDxControl):Integer; stdcall;
    ReadOnly:function(D:TDxControl):Boolean; stdcall; //是否只读
    MaxLength:function(D:TDxControl):Integer; stdcall; //最大长度
    SelectedColor:function(D:TDxControl):TColor; stdcall; //光标颜色
    SelBackColor:function(D:TDxControl):TColor; stdcall; //选择字体背景色
    SelFontColor:function(D:TDxControl):TColor; stdcall; //选择字体色
    PasswordChar:function(D:TDxControl):Char; stdcall;
    AllowSelect:function(D:TDxControl):Boolean; stdcall; //是否允许选择
    AllowPaste:function(D:TDxControl):Boolean; stdcall; //是否允许粘贴
    InValue:function(D:TDxControl):TInValue; stdcall; //允许输入控制
    TabOrder:function(D:TDxControl):Integer; stdcall;
    OnChange:function(D:TDxControl):Pointer; stdcall; //输入框改变触发
    //-----------------------------------------------------------------------------
    SetText:procedure(D:TDxControl; Value:PChar); stdcall;
    SetValue:procedure(D:TDxControl; Value:Integer); stdcall;
    SetReadOnly:procedure(D:TDxControl; Value:Boolean); stdcall; //是否只读
    SetMaxLength:procedure(D:TDxControl; Value:Integer); stdcall; //最大长度
    SetSelectedColor:procedure(D:TDxControl; Value:TColor); stdcall; //光标颜色
    SetSelBackColor:procedure(D:TDxControl; Value:TColor); stdcall; //选择字体背景色
    SetSelFontColor:procedure(D:TDxControl; Value:TColor); stdcall; //选择字体色
    SetPasswordChar:procedure(D:TDxControl; Value:Char); stdcall;
    SetAllowSelect:procedure(D:TDxControl; Value:Boolean); stdcall; //是否允许选择
    SetAllowPaste:procedure(D:TDxControl; Value:Boolean); stdcall; //是否允许粘贴
    SetInValue:procedure(D:TDxControl; Value:TInValue); stdcall; //允许输入控制
    SetTabOrder:procedure(D:TDxControl; Value:Integer); stdcall;
    SetOnChange:procedure(Value:TMethod); stdcall;
    //-----------------------------------------------------------------------------
  end;
  pTDEdit = ^TDEdit;

  TDGrid = record
    ColCount:function(D:TDxControl):Integer; stdcall; //列数
    RowCount:function(D:TDxControl):Integer; stdcall; //组数
    ColWidth:function(D:TDxControl):Integer; stdcall; //列宽
    RowHeight:function(D:TDxControl):Integer; stdcall; //组高

    OnGridSelect:function(D:TDxControl):Pointer; stdcall; //选择事件
    OnGridMouseMove:function(D:TDxControl):Pointer; stdcall; //鼠标移动事件
    OnGridPaint:function(D:TDxControl):Pointer; stdcall; //绘制事件
    //-----------------------------------------------------------------------------

    SetColCount:procedure(D:TDxControl; Value:Integer); stdcall; //列数
    SetRowCount:procedure(D:TDxControl; Value:Integer); stdcall; //组数
    SetColWidth:procedure(D:TDxControl; Value:Integer); stdcall; //列宽
    SetRowHeight:procedure(D:TDxControl; Value:Integer); stdcall; //组高

    SetOnGridSelect:procedure(Value:TMethod); stdcall; //选择事件
    SetOnGridMouseMove:procedure(Value:TMethod); stdcall; //鼠标移动事件
    SetOnGridPaint:procedure(Value:TMethod); stdcall; //绘制事件

    DrawGridItem:procedure(ARect:TRect; Item:pTClientItem; Effect:Pointer); stdcall; //绘制网格物品
  end;
  pTDGrid = ^TDGrid;

  TDComboBox = record
    ItemIndex:function(D:TDxControl):Integer; stdcall; //当前选择行
    Items:function(D:TDxControl):TStringList; stdcall; //列表
    Text:function(D:TDxControl):PChar; stdcall;
    OnSelect:function(D:TDxControl):Pointer; stdcall;

    //-----------------------------------------------------------------------------
    SetShowButton:procedure(D:TDxControl; Value:Boolean); stdcall; //是否显示按钮
    SetItemIndex:procedure(D:TDxControl; Value:Integer); stdcall; //当前选择行
    SetButtonColor:procedure(D:TDxControl; Value:TColor); stdcall; //按钮颜色
    SetItems:procedure(D:TDxControl; Value:TStringList); stdcall; //列表
    SetText:procedure(D:TDxControl; Value:PChar); stdcall;
    SetOnSelect:procedure(Value:TMethod); stdcall;

    SetDefaultTextFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
    SetDefaultTextFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
    SetDefaultTextFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
    SetDefaultTextFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
    SetDefaultTextFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //默认是否描边
    SetDefaultTextFontName:procedure(D:TDxControl; Value:PChar); stdcall; //默认字体名称

    SetMouseMoveTextFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
    SetMouseMoveTextFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
    SetMouseMoveTextFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
    SetMouseMoveTextFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
    SetMouseMoveTextFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标移动字体是否描边
    SetMouseMoveTextFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称

    SetMouseDownTextFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
    SetMouseDownTextFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
    SetMouseDownTextFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
    SetMouseDownTextFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
    SetMouseDownTextFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标按下字体是否描边
    SetMouseDownTextFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称

    SetDisabledTextFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
    SetDisabledTextFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
    SetDisabledTextFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
    SetDisabledTextFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
    SetDisabledTextFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //不可用时字体是否描边
    SetDisabledTextFontName:procedure(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称
  end;
  pTDComboBox = ^TDComboBox;

  //TMenuList=TObject;

  TItemMenuListAPI = record //TDxItemMenuList 相关API
    Count:function(List:TDxItemMenuList):Integer; stdcall;
    Add:procedure(List:TDxItemMenuList; S:PChar); stdcall;
    AddObject:procedure(List:TDxItemMenuList; S:PChar; AObject:TObject); stdcall;
    Get:function(List:TDxItemMenuList; Index:Integer):PChar; stdcall;
    GetObject:function(List:TDxItemMenuList; Index:Integer):TObject; stdcall;
    Delete:procedure(List:TDxItemMenuList; Index:Integer); stdcall;
    Clear:procedure(List:TDxItemMenuList); stdcall;
    {GetEnabled: function(List: TDxItemMenuList; Index: Integer): Boolean; stdcall;
    GetVisible: function(List: TDxItemMenuList; Index: Integer): Boolean; stdcall;
    GetChecked: function(List: TDxItemMenuList; Index: Integer): Boolean; stdcall;
    SetEnabled: procedure(List: TDxItemMenuList; Index: Integer; Value: Boolean); stdcall;
    SetVisible: procedure(List: TDxItemMenuList; Index: Integer; Value: Boolean); stdcall;
    SetChecked: procedure(List: TDxItemMenuList; Index: Integer; Value: Boolean); stdcall;}
  end;

  TDPopupMenu = record
    ItemIndex:function(D:TDxControl):Integer; stdcall; //选择行
    ItemHeight:function(D:TDxControl):Integer; stdcall; //组高
    Items:function(D:TDxControl):TDxItemMenuList; stdcall; //列表
    //-----------------------------------------------------------------------------
    SetSelectColor:procedure(D:TDxControl; Value:TColor); stdcall; //选择颜色
    SetItemIndex:procedure(D:TDxControl; Value:Integer); stdcall; //选择行
    SetItemHeight:procedure(D:TDxControl; Value:Integer); stdcall; //组高
    SetAlpha:procedure(D:TDxControl; Value:Byte); stdcall; //背景透明度

    SetDefaultItemFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
    SetDefaultItemFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
    SetDefaultItemFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
    SetDefaultItemFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
    SetDefaultItemFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //默认字体是否描边
    SetDefaultItemFontName:procedure(D:TDxControl; Value:PChar); stdcall; //默认字体名称

    SetMouseMoveItemFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
    SetMouseMoveItemFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
    SetMouseMoveItemFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
    SetMouseMoveItemFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
    SetMouseMoveItemFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标移动字体是否描边
    SetMouseMoveItemFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称

    SetMouseDownItemFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
    SetMouseDownItemFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
    SetMouseDownItemFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
    SetMouseDownItemFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
    SetMouseDownItemFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //鼠标按下字体是否描边
    SetMouseDownItemFontName:procedure(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称

    SetDisabledItemFontFColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
    SetDisabledItemFontBColor:procedure(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
    SetDisabledItemFontStyle:procedure(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
    SetDisabledItemFontSize:procedure(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
    SetDisabledItemFontBold:procedure(D:TDxControl; Value:Boolean); stdcall; //不可用时字体是否描边
    SetDisabledItemFontName:procedure(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称
  end;
  pTDPopupMenu = ^TDPopupMenu;

  TInterfaceAPI = record //界面操作API
    DControl:TDControl;
    DWindow:TDWindow;
    DButton:TDButton;
    DEdit:TDEdit;
    DGrid:TDGrid;
    DComboBox:TDComboBox;
    DPopupMenu:TDPopupMenu;
  end;
  pTInterfaceAPI = ^TInterfaceAPI;

  TGameInterfaceAPI = record //客户端内部界面
    DMainMenu:function:TDxControl; stdcall; //右键弹出菜单
    DLoginDlg:function:TDxControl; stdcall; //登录背景窗口
    DRandomCodeDlg:function:TDxControl; stdcall; //随机码窗口
    DLogin:function:TDxControl; stdcall; //登录对话框窗口
    DNewAccount:function:TDxControl; stdcall; //注册帐号窗口
    DChgPw:function:TDxControl; stdcall; //修改密码窗口
    DSelServerDlg:function:TDxControl; stdcall; //选择服务器背景窗口
    DServerDlg:function:TDxControl; stdcall; //选择服务器窗口
    DDoorDlg:function:TDxControl; stdcall; //开门时的背景窗口
    DSelectChr:function:TDxControl; stdcall; //选择角色背景窗口
    DCreateChr:function:TDxControl; stdcall; //创建角色窗口
    DDeleteHumanDlg:function:TDxControl; stdcall; //恢复角色窗口
    DNoticeDlg:function:TDxControl; stdcall; //公告窗口
    DMerchantDlg:function:TDxControl; stdcall; //NPC对话框
    DBottomLeft:function:TDxControl; stdcall; //游戏界面左
    DBottomCenter:function:TDxControl; stdcall; //游戏界面中
    DBottomRight:function:TDxControl; stdcall; //游戏界面右
    DItemBag:function:TDxControl; stdcall; //包裹窗口
    DStateWin:function:TDxControl; stdcall; //人物属性窗口 自己
    DUserState1:function:TDxControl; stdcall; //人物属性窗口 查看对方
    DHeroStateWin:function:TDxControl; stdcall; //英雄属性窗口
    DHeroStateDlg:function:TDxControl; stdcall; //英雄状态窗口
    DHeroItemBag:function:TDxControl; stdcall; //英雄包裹窗口
    DMenuDlg:function:TDxControl; stdcall; // NPC列表框
    DSellDlg:function:TDxControl; stdcall; //OK框
    DDealDlg:function:TDxControl; stdcall; //交易对话框 自己
    DDealRemoteDlg:function:TDxControl; stdcall; //交易对话框 对方
    DShopDlg:function:TDxControl; stdcall; //商铺窗口
    DGroupDlg:function:TDxControl; stdcall; //组队窗口
    DRankingDlg:function:TDxControl; stdcall; //排行榜窗口
    DGuildDlg:function:TDxControl; stdcall; //行会窗口
    DGuildEditNotice:function:TDxControl; stdcall; //行会编辑窗口
    DAdjustAbility:function:TDxControl; stdcall; //附加属性窗口
    DMissionDlg:function:TDxControl; stdcall; //任务日记窗口
  end;
  pTGameInterfaceAPI = ^TGameInterfaceAPI;

  TListAPI = record
    Create:function:TList; stdcall;
    Free:procedure(List:TList); stdcall;
    Count:function(List:TList):Integer; stdcall;
    Add:procedure(List:TList; Item:Pointer); stdcall;
    Insert:procedure(List:TList; Index:Integer; Item:Pointer); stdcall;
    Get:function(List:TList; Index:Integer):Pointer; stdcall;
    Delete:procedure(List:TList; Index:Integer); stdcall;
    Clear:procedure(List:TList); stdcall;
  end;

  TStringListAPI = record
    Create:function:TStringList; stdcall;
    Free:procedure(List:TStringList); stdcall;
    Count:function(List:TStringList):Integer; stdcall;
    Add:procedure(List:TStringList; S:PChar); stdcall;
    AddObject:procedure(List:TStringList; S:PChar; AObject:TObject); stdcall;
    Insert:procedure(List:TStringList; Index:Integer; S:PChar; AObject:TObject); stdcall;
    Get:function(List:TStringList; Index:Integer):PChar; stdcall;
    GetObject:function(List:TStringList; Index:Integer):TObject; stdcall;
    Delete:procedure(List:TStringList; Index:Integer); stdcall;
    Clear:procedure(List:TStringList); stdcall;
  end;

  //------------------------------------------------------------------------------
  // 地面上某个点的物品列表 2019-12-23
  TPointDropItemListAPI = record
    Count:function(DropItemList:TPointDropItemList):Integer; stdcall;
    Get:function(DropItemList:TPointDropItemList; Index:Integer):PTDropItem; stdcall;
    X:function(DropItemList:TPointDropItemList):Word; stdcall;
    Y:function(DropItemList:TPointDropItemList):Word; stdcall;
  end;

  // 地面物品列表 2019-12-23
  TDropItemsMgrAPI = record
    Lock:procedure(DropItemsMgr:TDropItemsMgr); stdcall;
    UnLock:procedure(DropItemsMgr:TDropItemsMgr); stdcall;

    Count:function(DropItemsMgr:TDropItemsMgr):Integer; stdcall;
    Get:function(DropItemsMgr:TDropItemsMgr; Index:Integer):TPointDropItemList; stdcall;

    GetItemByID:function(DropItemsMgr:TDropItemsMgr; ID:Integer):pTDropItem; stdcall;
    GetItemListByPoint:function(DropItemsMgr:TDropItemsMgr; X, Y:Word):TPointDropItemList; stdcall;
    GetItemListIndexByY:function(DropItemsMgr:TDropItemsMgr; Y:Word):Integer; stdcall;
  end;
  //------------------------------------------------------------------------------

  TDrawAPI = record //绘制图片相关API
    Draw:procedure(X, Y:Integer; SrcRect:TRect; Texture:TTexture; BlendMode:Integer); stdcall;
    DrawColor:procedure(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; BlendMode:Integer); stdcall;
    StretchDraw:procedure(DestRect, SrcRect:TRect; Texture:TTexture; BlendMode:Integer); stdcall; //缩放模式绘制
    DrawAlpha:procedure(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Alpha:Byte; BlendMode:Integer); stdcall;
    DrawColorAlpha:procedure(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; BlendMode:Integer); stdcall;
    DrawBlend:procedure(X, Y:Integer; SrcRect:TRect; Texture:TTexture); stdcall; //魔法效果绘制
    DrawEffect:procedure(GameImages, nImageIndex, X, Y:Integer; SrcRect:TRect; boEffect:Boolean; BlendMode:Integer); stdcall; // 绘制加亮和灰度效果  boEffect=True加亮  boEffect=False灰度

    FrameRect:procedure(Rect:TRect; Color:TColor; BlendMode:Integer); stdcall; //绘制矩形框
    FillRect:procedure(Rect:TRect; Color:TColor; BlendMode:Integer); stdcall; //绘制矩形框且填冲该矩形
    FillRectAlpha:procedure(DestRect:TRect; Color:TColor; Alpha:Byte; BlendMode:Integer); stdcall;
    Line:procedure(Pt1, Pt2:TPoint; Color:TColor; BlendMode:Integer); stdcall; //画线
    FillTri:procedure(p1, p2, p3:TPoint; c1, c2, c3:TColor; BlendMode:Integer); stdcall; //画三角形
    Circle:procedure(X, Y, Radius:Single; Color:TColor; Filled:Boolean; BlendMode:Integer); stdcall; //画圆
    CurrentFont:function:THGEFont; stdcall; //当前默认字体
    FindFont:function(FontName:PChar; FontSize:Integer; FontStyles:TFontStyles):THGEFont; stdcall;
    TextRect:procedure(HGEFont:THGEFont; X, Y:Integer; SrcRect:TRect; Text:PChar; Color:TColor; BlendMode:Integer); stdcall;
    TextOut:procedure(HGEFont:THGEFont; X, Y:Integer; Text:PChar; Color:TColor; BlendMode:Integer); stdcall;
    BoldTextOut:procedure(HGEFont:THGEFont; X, Y:Integer; Text:PChar; FColor, BColor:TColor); stdcall;
    TextWidth:function(HGEFont:THGEFont; Text:PChar):Integer; stdcall;
    TextHeight:function(HGEFont:THGEFont; Text:PChar):Integer; stdcall;
  end;
  pTDrawAPI = ^TDrawAPI;

  TSocketAPI = record //数据发送
    SendSocket:procedure(S:PChar); stdcall;
    SendClientMessage:procedure(Msg:Word; Recog:Int64; param, tag, series:Word; S:PChar); stdcall; // 2021-01-06 changed
    SendLogin:procedure(uid, passwd:PChar); stdcall; //登录
    SendSelectServer:procedure(sServerName:PChar); stdcall; //选择服务器
    SendQueryChr:procedure; stdcall; //查询角色
    SendSelChr:procedure(sChrName:PChar); stdcall; //选择角色
    SendSay:procedure(S:PChar); stdcall;
    Close:procedure; stdcall; //断开连接
  end;
  pTSocketAPI = ^TSocketAPI;

  pTList = ^TList;

  TGameAPI = record
    ClientPath:function:PChar; stdcall; //登录器路径
    ClientName:function:PChar; stdcall; //登录器名称
    MySelf:function:TActor; stdcall; //自己
    MyHero:function:TActor; stdcall; //我的英雄
    MagicList:function:TList; stdcall; //技能列表
    MagicNGList:function:TList; stdcall; //内功技能列表
    ContinuousMagicList:function:TList; stdcall; //连击技能列表

    HeroMagicList:function:TList; stdcall; //英雄魔法列表
    HeroMagicNGList:function:TList; stdcall; //英雄内功技能列表
    HeroContinuousMagicList:function:TList; stdcall; //英雄连击技能列表

    GroupMembers:function:TList; stdcall; //组列表
    DropedItemList:function:TDropItemsMgr; stdcall; //地面物品列表 ### 2019-12-23
    MenuItemList:function:TList; stdcall; //当前NPC出售物品列表
    ActorList:function:TList; stdcall; //角色列表
    ScreenXYfromMCXY:procedure(cx, cy:Integer; var sx, sY:Integer); stdcall; //地图坐标转换屏幕坐标
    CXYfromMouseXY:procedure(mx, my:Integer; var ccx, ccy:Integer); stdcall; //屏幕坐标转换地图坐标
    FindActor1:function(nRecogId:Int64):TActor; stdcall; //查找角色 2020-01-11 64位支持
    FindActor2:function(sName:PChar):TActor; stdcall;
    FindActorXY1:function(X, Y:Integer):TActor; stdcall;
    FindActorXY2:function(X, Y:Integer; Actor:TActor):TActor; stdcall;
    CanWalk:function(mx, my:Integer):Boolean; stdcall;
    CanRun:function(sx, sY, ex, ey:Integer):Boolean; stdcall;
    CanHorseRun:function(sx, sY, ex, ey:Integer):Boolean; stdcall;
    GetRGB:function(c256:Byte):Integer; stdcall; //获取颜色
    DebugOutStr:procedure(Msg:PChar; boWriteDate:Boolean); stdcall; //写入日记
    AppLogout:procedure; stdcall; //小退
    AppExit:procedure; stdcall; //退出游戏
    DMessageDlg:function(Msg:PChar; DlgButtons:TMsgDlgButtons):TModalResult; stdcall; //弹出对话框
    AddChatBoardString:procedure(Msg:PChar; FColor, BColor:Byte); stdcall; //聊天框显示信息
    AddTopChatBoardString:procedure(Msg:PChar; FColor, BColor:Byte; TimeOut:Integer); stdcall; //聊天框固顶信息
    AddMoveMsg:procedure(Msg:PChar; FColor, BColor:Byte; nX, nY, nCount:Integer); stdcall; //滚动信息
    ShowHint:procedure(X, Y:Integer; Msg:PChar; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean); stdcall; //显示悬浮框
    ShowMouseItemInfo:procedure(Actor:TActor; MouseItem:pTClientItem; X, Y:Integer; Secret {是否是神秘装备}:Boolean; ShowTzItemDesc {套装备注类型 0=自己 1=英雄 2=查看的人其他人}:Byte; DrawUp:Boolean; DrawLeft:Boolean); stdcall; //悬浮框显示装备信息
    ClearHint:procedure; stdcall; //清除悬浮框
    DlgEditText:function:PChar; stdcall; //DMessageDlg 对话框用户输入的信息
    PlaySound:procedure(idx:Integer); stdcall;
    PlaySoundA:procedure(sFileName:PChar; LoopCount:Integer); stdcall;
    ItemClickSound:procedure(StdItem:TStdItem); stdcall;
    ItemBag:function:pTClientBagItems; stdcall; //包裹物品指针
    HeroItemBag:function:pTClientHeroBagItems; stdcall; //英雄包裹物品指针
    UseItems:function:pTUseItems; stdcall; //身上装备指针
    HeroUseItems:function:pTUseItems; stdcall; //英雄身上装备指针

    JewelryBoxItems:function:pTJewelryBoxItems; stdcall; // 首饰盒物品指针
    HeroJewelryBoxItems:function:PTJewelryBoxItems; stdcall; // 英雄首饰盒物品指针
    GodBlessItems:function:PTGodBlessItems; stdcall; // 神佑盒物品指针
    HeroGodBlessItems:function:PTGodBlessItems; stdcall; // 英雄神佑盒物品指针
    ItemBoxItems:function:PTItemBoxItems; stdcall; // 自定义OK框物品指针

    UserState1:function:pTUserStateInfo; stdcall; // 查看的别人身上装备指针
    boItemMoving:function:Boolean; stdcall;
    MovingItem:function:Pointer; stdcall; //pTMovingItem; //当前正在移动的物品指针
    WaitingUseItem:function:Pointer; stdcall; //pTMovingItem;
    SellDlgItem:function:Pointer; stdcall; //pTMovingItem; 当前OK框物品
    nTargetX:function:PInteger; stdcall; //目标座标
    nTargetY:function:PInteger; stdcall; //目标座标
    TargetCret:function:pTActor; stdcall;
    FocusCret:function:TActor; stdcall;
    MagicTarget:function:pTActor; stdcall;
    Gold:function:Integer; stdcall; //金币数量
    GameGold:function:Integer; stdcall; //元宝数量
    GamePoint:function:Integer; stdcall; //游戏点数量
    GloryPoint:function:Integer; stdcall; //荣誉
    GameDiamond:function:Integer; stdcall; //金刚石
    GameGird:function:Integer; stdcall; //灵符
    GameGlory:function:Integer; stdcall; //荣誉
    LoyaltyPoint:function:Integer; stdcall; //忠诚度
    GameGoldName:function:PChar; stdcall; //元宝名称
    GamePointName:function:PChar; stdcall; //游戏点名称
    GameDiamondName:function:PChar; stdcall; //金刚石名称
    GameGirdName:function:PChar; stdcall; //灵符名称
    EncodeBuffer:function(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;
    DecodeBuffer:function(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;
    FullScreenDrawScene:procedure(Value:Boolean); stdcall; //全屏绘制场景
    AreaStateValue:function:Boolean; stdcall; //当前是在攻城区域
    MapTitle:function:PChar; stdcall; //当前地图名称
    EatItem:procedure(idx:Integer); stdcall; //使用物品
    HeroEatItem:procedure(idx:Integer); stdcall; //英雄使用物品
    ServerImageList:function:TStringList; stdcall; //M2列表信息2的WIL列表
    ClassDlg:function:TObject; stdcall;

    SetMovingItem:procedure(Item:Pointer); stdcall; //pTMovingItem; //当前正在移动的物品
    SetWaitingUseItem:procedure(Item:Pointer); stdcall; //pTMovingItem;
    SetSellDlgItem:procedure(Item:pTClientItem); stdcall; //pTMovingItem; 当前OK框物品

    SetItemBag:procedure(Index:Integer; Item:pTClientItem); stdcall; //包裹物品
    SetHeroItemBag:procedure(Index:Integer; Item:pTClientItem); stdcall; //英雄包裹物品
    SetUseItems:procedure(Index:Integer; Item:pTClientItem); stdcall; //身上装备
    SetHeroUseItems:procedure(Index:Integer; Item:pTClientItem); stdcall; //英雄身上装备

    SetJewelryBoxItems:procedure(Index:Integer; Item:pTClientItem); stdcall; // 首饰盒物品指针
    SetHeroJewelryBoxItems:procedure(Index:Integer; Item:pTClientItem); stdcall; // 英雄首饰盒物品指针
    SetGodBlessItems:procedure(Index:Integer; Item:pTClientItem); stdcall; // 神佑盒物品指针
    SetHeroGodBlessItems:procedure(Index:Integer; Item:pTClientItem); stdcall; // 英雄神佑盒 物品指针
    SetItemBoxItems:procedure(Index:Integer; Item:pTClientItem); stdcall; // 自定义OK框物品指针

    SetUserState1:procedure(UserStateInfo:pTUserStateInfo); stdcall; //查看的别人身上装备
    SetItemMoving:procedure(Value:Boolean); stdcall;
    Reserveds:array[0..99] of Integer;
  end;

  TInitialize = procedure(Handle:THandle; FirstInit:Boolean; WindowMode:Boolean; ScreenWidth, ScreenHeight:Word; ClientVersion:TClientVersion); stdcall;
  TStartPro = procedure; stdcall;
  TFormKeyDown = procedure(Sender:TObject; var Key:Word; Shift:TShiftState); stdcall;
  TFormKeyPress = procedure(Sender:TObject; var Key:Char); stdcall;
  TFormMouseDown = procedure(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer); stdcall;
  TFormMouseMove = procedure(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
  TDecodeMessagePacket = procedure(DefMsg:pTDefaultMessage; sData:PChar); stdcall;

  TObjectAction = procedure(Actor:TActor); stdcall;
  TTActor_DrawChr = procedure(Actor:TActor; dx, dy:Integer; blend:Boolean; boFlag:Boolean); stdcall;

  THookAPI = record
    GetHookInitialize:function:TInitialize; stdcall;
    GetHookFinalize:function:TStartPro; stdcall;
    GetHookFormKeyDown:function:TFormKeyDown; stdcall;
    GetHookFormKeyPress:function:TFormKeyPress; stdcall;
    GetHookFormMouseDown:function:TFormMouseDown; stdcall;
    GetHookFormMouseMove:function:TFormMouseMove; stdcall;
    GetHookDecodeMessagePacketStart:function:TDecodeMessagePacket; stdcall;
    GetHookDecodeMessagePacketStop:function:TDecodeMessagePacket; stdcall;
    GetHookDecodeMessagePacket:function:TDecodeMessagePacket; stdcall;

    GetHookDrawScene1:function:TStartPro; stdcall;
    GetHookDrawScene2:function:TStartPro; stdcall;
    GetHookDrawScene3:function:TStartPro; stdcall;
    GetHookDrawScene4:function:TStartPro; stdcall;

    GetHookTActor_FeatureChanged:function:TObjectAction; stdcall;
    GetHookTActor_CalcActorFrame:function:TObjectAction; stdcall;
    GetHookTActor_DrawChr1:function:TTActor_DrawChr; stdcall;
    GetHookTActor_DrawChr2:function:TTActor_DrawChr; stdcall;

    GetHookTHumActor_CalcActorFrame:function:TObjectAction; stdcall;
    GetHookTHumActor_DrawChr1:function:TTActor_DrawChr; stdcall;
    GetHookTHumActor_DrawChr2:function:TTActor_DrawChr; stdcall;
    GetHookTHumActor_DrawChr3:function:TTActor_DrawChr; stdcall;
    GetHookTHumActor_DrawChr4:function:TTActor_DrawChr; stdcall;

    //------------------------------------------------------------------------------
    SetHookInitialize:procedure(Value:TInitialize); stdcall;
    SetHookFinalize:procedure(Value:TStartPro); stdcall;
    SetHookFormKeyDown:procedure(Value:TFormKeyDown); stdcall;
    SetHookFormKeyPress:procedure(Value:TFormKeyPress); stdcall;
    SetHookFormMouseDown:procedure(Value:TFormMouseDown); stdcall;
    SetHookFormMouseMove:procedure(Value:TFormMouseMove); stdcall;
    SetHookDecodeMessagePacketStart:procedure(Value:TDecodeMessagePacket); stdcall;
    SetHookDecodeMessagePacketStop:procedure(Value:TDecodeMessagePacket); stdcall;
    SetHookDecodeMessagePacket:procedure(Value:TDecodeMessagePacket); stdcall;

    SetHookDrawScene1:procedure(Value:TStartPro); stdcall;
    SetHookDrawScene2:procedure(Value:TStartPro); stdcall;
    SetHookDrawScene3:procedure(Value:TStartPro); stdcall;
    SetHookDrawScene4:procedure(Value:TStartPro); stdcall;

    SetHookTActor_FeatureChanged:procedure(Value:TObjectAction); stdcall;
    SetHookTActor_CalcActorFrame:procedure(Value:TObjectAction); stdcall;
    SetHookTActor_DrawChr1:procedure(Value:TTActor_DrawChr); stdcall;
    SetHookTActor_DrawChr2:procedure(Value:TTActor_DrawChr); stdcall;

    SetHookTHumActor_CalcActorFrame:procedure(Value:TObjectAction); stdcall;
    SetHookTHumActor_DrawChr1:procedure(Value:TTActor_DrawChr); stdcall;
    SetHookTHumActor_DrawChr2:procedure(Value:TTActor_DrawChr); stdcall;
    SetHookTHumActor_DrawChr3:procedure(Value:TTActor_DrawChr); stdcall;
    SetHookTHumActor_DrawChr4:procedure(Value:TTActor_DrawChr); stdcall;

    Reserveds:array[0..99] of Integer;
  end;

  TActorAPI = record
    m_wAppearance:function(Actor:TActor):PWord; stdcall;
    m_nRecogId:function(Actor:TActor):PInt64; stdcall; //角色标识 2020-01-11 64位支持
    m_nCurrX:function(Actor:TActor):PInteger; stdcall; //当前所在地图座标X
    m_nCurrY:function(Actor:TActor):PInteger; stdcall; //当前所在地图座标Y
    m_btDir:function(Actor:TActor):PByte; stdcall; //当前站立方向
    m_btSex:function(Actor:TActor):PByte; stdcall; //性别
    m_btRace:function(Actor:TActor):PByte; stdcall; //怪物DB库的RaceImg
    m_btHair:function(Actor:TActor):PByte; stdcall; //头发类型
    m_wDress:function(Actor:TActor):PWord; stdcall; //衣服类型
    m_wWeapon:function(Actor:TActor):PWord; stdcall; //武器类型
    m_btJob:function(Actor:TActor):PByte; stdcall; //职业 0:武士  1:法师  2:道士
    m_btCaseltGuild:function(Actor:TActor):PByte; stdcall; //1=沙行会成员 //2=沙行会掌门
    m_sDescUserName:function(Actor:TActor):PChar; stdcall; //人物封号
    m_sUserName:function(Actor:TActor):PChar; stdcall; //名称
    m_nNameColor:function(Actor:TActor):PInteger; stdcall; //名称颜色
    m_Abil:function(Actor:TActor):pTAbility; stdcall; //属性
    m_boOpenShop:function(Actor:TActor):Boolean; stdcall; //是否在摆摊

    m_nSayX:function(Actor:TActor):Integer; stdcall;
    m_nSayY:function(Actor:TActor):Integer; stdcall;
    m_nShiftX:function(Actor:TActor):Integer; stdcall;
    m_nShiftY:function(Actor:TActor):Integer; stdcall;
    m_nTargetX:function(Actor:TActor):PInteger; stdcall;
    m_nTargetY:function(Actor:TActor):PInteger; stdcall;
    m_nTargetRecog:function(Actor:TActor):PInt64; stdcall; // 2020-01-11 64位支持
    m_boCobweb:function(Actor:TActor):PBoolean; stdcall; //网罩住了
    m_boCanDraw:function(Actor:TActor):Boolean; stdcall; //该角色是否可以绘制
    m_nBagCount:function(Actor:TActor):Integer; stdcall; //包裹最大数

    m_btColor:function(Actor:TActor):Byte; stdcall; //脚本命令修改的身体颜色
    m_nState:function(Actor:TActor):PInteger; stdcall; //人物中毒麻痹等
    //------------------------------------------------------------------------------
    m_nBodyOffset:function(Actor:TActor):PInteger; stdcall;
    m_boUseMagic:function(Actor:TActor):PBoolean; stdcall;
    m_nCurrentFrame:function(Actor:TActor):PInteger; stdcall;
    m_nStartFrame:function(Actor:TActor):PInteger; stdcall;
    m_nEndFrame:function(Actor:TActor):PInteger; stdcall;
    m_dwFrameTime:function(Actor:TActor):PLongWord; stdcall;
    m_dwStartTime:function(Actor:TActor):PLongWord; stdcall;

    Reserveds:array[0..99] of Integer;
  end;

  TClientAPI = record
    ListAPI:TListAPI;
    StringListAPI:TStringListAPI;
    ItemMenuListAPI:TItemMenuListAPI;
    TextureAPI:TTextureAPI;
    ImagesAPI:TImagesAPI; //读取WIL API
    InterfaceAPI:TInterfaceAPI; //游戏界面API
    DrawAPI:TDrawAPI; //绘制API
    ActorAPI:TActorAPI; //角色相关API
    SocketAPI:TSocketAPI;
    HookAPI:THookAPI; //HookAPI
    GameAPI:TGameAPI;
    GameInterfaceAPI:TGameInterfaceAPI; //

    PointDropItemList:TPointDropItemListAPI; // +++ 地面上某个点的物品列表 2019-12-23
    DropItemsMgr:TDropItemsMgrAPI; // +++ 地面物品列表管理 2019-12-23

    Reserveds:array[0..88] of Integer; // ### 减少11个预留值 2019-12-23
  end;
  pTClientAPI = ^TClientAPI;

  TPlugInfo = record
    Module:THandle;
    MemoryStream:TMemoryStream;
    MD5:string[32];
  end;
  pTPlugInfo = ^TPlugInfo;

  TPlugInit = function(ClientAPI:pTClientAPI; APISize:Integer):Integer; stdcall;

  TPlugInManage = class
    PlugList:TStringList;
    PlugNameList:TStringList;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadPlugIn();
    procedure UnLoadPlugIn();
  end;

function TGameAPI_EncodeBuffer(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;

function TGameAPI_DecodeBuffer(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;

procedure TGameAPI_FullScreenDrawScene(boFullScreen:Boolean); stdcall;

var
  ClientAPI:TClientAPI;
  PlugInManage:TPlugInManage;

  HookInitialize:TInitialize = nil;
  HookFinalize:TStartPro = nil;
  HookFormKeyDown:TFormKeyDown = nil;
  HookFormKeyPress:TFormKeyPress = nil;
  HookFormMouseDown:TFormMouseDown = nil;
  HookFormMouseMove:TFormMouseMove = nil;
  HookDecodeMessagePacketStart:TDecodeMessagePacket = nil;
  HookDecodeMessagePacketStop:TDecodeMessagePacket = nil;
  HookDecodeMessagePacket:TDecodeMessagePacket = nil;

  HookDrawScene1:TStartPro = nil;
  HookDrawScene2:TStartPro = nil;
  HookDrawScene3:TStartPro = nil;
  HookDrawScene4:TStartPro = nil;
  HookTActor_FeatureChanged:TObjectAction = nil;
  HookTActor_CalcActorFrame:TObjectAction = nil;
  HookTActor_DrawChr1:TTActor_DrawChr = nil;
  HookTActor_DrawChr2:TTActor_DrawChr = nil;

  HookTHumActor_CalcActorFrame:TObjectAction = nil;
  HookTHumActor_DrawChr1:TTActor_DrawChr = nil;
  HookTHumActor_DrawChr2:TTActor_DrawChr = nil;
  HookTHumActor_DrawChr3:TTActor_DrawChr = nil;
  HookTHumActor_DrawChr4:TTActor_DrawChr = nil;

  {$IFEND}

implementation

{$IF Enabled_PlugEngine = 1}

uses ClMain,
  MShare,
  FState,
  SerialWindowsDlg,
  SoundUtil,
  MD5Util,
  ClFunc;

function TGameImages_GetHandle(const FileName:PChar):THandle; stdcall;
var
  sFileName:string;
  GameImages:TGameImages;
begin
  sFileName := FileName;
  GameImages := GetGameImages(sFileName);
  if GameImages = nil then begin
    GameImages := CreateGameImages(sFileName);
    GameImages.Initialize;
    g_ImageEvent.AddDynamic(GameImages);
  end;
  Result := Integer(GameImages);
end;

function TGameImages_Count(FileHandle:THandle):Integer; stdcall;
begin
  Result := TGameImages(FileHandle).ImageCount;
end;

function TGameImages_Read(FileHandle:THandle; Index:Integer; var X, Y:Integer):TTexture; stdcall;
begin
  Result := TGameImages(FileHandle).GetCachedImage(Index, X, Y);
end;

procedure TGameImages_Clear(FileHandle:THandle); stdcall;
begin
  TGameImages(FileHandle).ClearCache;
end;

//------------------------------------------------------------------------------

function TTextureAPI_Width(Texture:TTexture):Integer; stdcall;
begin
  Result := Texture.Width;
end;

function TTextureAPI_Height(Texture:TTexture):Integer; stdcall;
begin
  Result := Texture.Height;
end;

function TTextureAPI_Pixels(Texture:TTexture; X, Y:Integer):Cardinal; stdcall;
begin
  Result := Texture.Pixels[X, Y];
end;

function TTextureAPI_Lock(Texture:TTexture; Rect:TRect; out Bits:Pointer; out Pitch:Integer; ReadOnly:Boolean):Boolean; stdcall;
begin
  Result := Texture.Lock(Rect, Bits, Pitch, ReadOnly);
end;

procedure TTextureAPI_Unlock(Texture:TTexture); stdcall;
begin
  Texture.Unlock;
end;
//------------------------------------------------------------------------------

function TListAPI_Create:TList; stdcall;
begin
  Result := TList.Create;
end;

procedure TListAPI_Free(List:TList); stdcall;
begin
  List.Free;
end;

function TListAPI_Count(List:TList):Integer; stdcall;
begin
  Result := List.Count;
end;

procedure TListAPI_Add(List:TList; Item:Pointer); stdcall;
begin
  List.Add(Item);
end;

procedure TListAPI_Insert(List:TList; Index:Integer; Item:Pointer); stdcall;
begin
  List.Insert(Index, Item);
end;

function TListAPI_Get(List:TList; Index:Integer):Pointer; stdcall;
begin
  Result := List.Items[Index];
end;

procedure TListAPI_Delete(List:TList; Index:Integer); stdcall;
begin
  List.Delete(Index);
end;

procedure TListAPI_Clear(List:TList); stdcall;
begin
  List.Clear;
end;

//------------------------------------------------------------------------------

function TStringListAPI_Create:TStringList; stdcall;
begin
  Result := TStringList.Create;
end;

procedure TStringListAPI_Free(List:TStringList); stdcall;
begin
  List.Free;
end;

function TStringListAPI_Count(List:TStringList):Integer; stdcall;
begin
  Result := List.Count;
end;

procedure TStringListAPI_Add(List:TStringList; S:PChar); stdcall;
begin
  List.Add(S);
end;

procedure TStringListAPI_AddObject(List:TStringList; S:PChar; AObject:TObject); stdcall;
begin
  List.AddObject(S, AObject);
end;

procedure TStringListAPI_Insert(List:TStringList; Index:Integer; S:PChar; AObject:TObject); stdcall;
begin
  List.InsertObject(Index, S, AObject);
end;

function TStringListAPI_Get(List:TStringList; Index:Integer):PChar; stdcall;
begin
  Result := PChar(List.Strings[Index]);
end;

function TStringListAPI_GetObject(List:TStringList; Index:Integer):TObject; stdcall;
begin
  Result := List.Objects[Index];
end;

procedure TStringListAPI_Delete(List:TStringList; Index:Integer); stdcall;
begin
  List.Delete(Index);
end;

procedure TStringListAPI_Clear(List:TStringList); stdcall;
begin
  List.Clear;
end;

//------------------------------------------------------------------------------

function TPointDropItemListAPI_Count(DropItemList:TPointDropItemList):Integer; stdcall;
begin
  Result := DropItemList.Count;
end;

function TPointDropItemListAPI_Get(DropItemList:TPointDropItemList; Index:Integer):PTDropItem; stdcall;
begin
  Result := DropItemList.Items[Index];
end;

function TPointDropItemListAPI_X(DropItemList:TPointDropItemList):Word; stdcall;
begin
  Result := DropItemList.X;
end;

function TPointDropItemListAPI_Y(DropItemList:TPointDropItemList):Word; stdcall;
begin
  Result := DropItemList.Y;
end;

//------------------------------------------------------------------------------

// 地面物品列表 2019-12-23

procedure TDropItemsMgrAPI_Lock(DropItemsMgr:TDropItemsMgr); stdcall;
begin
  DropItemsMgr.Lock;
end;

procedure TDropItemsMgrAPI_UnLock(DropItemsMgr:TDropItemsMgr); stdcall;
begin
  DropItemsMgr.UnLock;
end;

function TDropItemsMgrAPI_Count(DropItemsMgr:TDropItemsMgr):Integer; stdcall;
begin
  Result := DropItemsMgr.Count;
end;

function TDropItemsMgrAPI_Get(DropItemsMgr:TDropItemsMgr; Index:Integer):TPointDropItemList; stdcall;
begin
  Result := DropItemsMgr.Items[Index];
end;

function TDropItemsMgrAPI_GetItemByID(DropItemsMgr:TDropItemsMgr; ID:Integer):pTDropItem; stdcall;
begin
  Result := DropItemsMgr.GetItemByID(ID);
end;

function TDropItemsMgrAPI_GetItemListByPoint(DropItemsMgr:TDropItemsMgr; X, Y:Word):TPointDropItemList; stdcall;
begin
  Result := DropItemsMgr.GetItemListByPoint(X, Y);
end;

function TDropItemsMgrAPI_GetItemListIndexByY(DropItemsMgr:TDropItemsMgr; Y:Word):Integer; stdcall;
begin
  Result := DropItemsMgr.GetItemListIndexByY(Y);
end;
//------------------------------------------------------------------------------

procedure TDrawAPI_Draw(X, Y:Integer; SrcRect:TRect; Texture:TTexture; BlendMode:Integer); stdcall;
begin
  GameCanvas.Draw(X, Y, SrcRect, Texture, BlendMode);
end;

procedure TDrawAPI_DrawColor(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; BlendMode:Integer); stdcall;
begin
  GameCanvas.DrawColor(X, Y, SrcRect, Texture, Color, BlendMode);
end;

procedure TDrawAPI_StretchDraw(DestRect, SrcRect:TRect; Texture:TTexture; BlendMode:Integer); stdcall; //缩放模式绘制
begin
  GameCanvas.StretchDraw(DestRect, SrcRect, Texture, BlendMode);
end;

procedure TDrawAPI_DrawAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Alpha:Byte; BlendMode:Integer); stdcall;
begin
  GameCanvas.DrawAlpha(X, Y, SrcRect, Texture, Alpha, BlendMode);
end;

procedure TDrawAPI_DrawColorAlpha(X, Y:Integer; SrcRect:TRect; Texture:TTexture; Color:TColor; Alpha:Byte; BlendMode:Integer); stdcall;
begin
  GameCanvas.DrawColorAlpha(X, Y, SrcRect, Texture, Color, Alpha, BlendMode);
end;

procedure TDrawAPI_DrawBlend(X, Y:Integer; SrcRect:TRect; Texture:TTexture); stdcall; //魔法效果绘制
begin
  GameCanvas.DrawBlend(X, Y, SrcRect, Texture);
end;

procedure TDrawAPI_DrawEffect(GameImages, nImageIndex, X, Y:Integer; SrcRect:TRect; boEffect:Boolean; BlendMode:Integer); stdcall; // 绘制加亮和灰度效果  boEffect=True加亮  btEffect=1灰度
var
  Texture:TTexture;
begin
  if boEffect then
    Texture := TGameImages(GameImages).Brights[nImageIndex]
  else
    Texture := TGameImages(GameImages).Grays[nImageIndex];
  if Texture <> nil then
    GameCanvas.Draw(X, Y, SrcRect, Texture, BlendMode);
end;

procedure TDrawAPI_FrameRect(Rect:TRect; Color:TColor; BlendMode:Integer); stdcall; //绘制矩形框
begin
  GameCanvas.FrameRect(Rect, Color, 0, BlendMode);
end;

procedure TDrawAPI_FillRect(Rect:TRect; Color:TColor; BlendMode:Integer); stdcall; //绘制矩形框且填冲该矩形
begin
  GameCanvas.FillRect(Rect, Color, 0, BlendMode);
end;

procedure TDrawAPI_FillRectAlpha(DestRect:TRect; Color:TColor; Alpha:Byte; BlendMode:Integer); stdcall;
begin
  GameCanvas.FillRectAlpha(DestRect, Color, Alpha, 0, BlendMode);
end;

procedure TDrawAPI_Line(Pt1, Pt2:TPoint; Color:TColor; BlendMode:Integer); stdcall; //画线
begin
  GameCanvas.Line(Pt1, Pt2, Color, 0, BlendMode);
end;

procedure TDrawAPI_FillTri(p1, p2, p3:TPoint; c1, c2, c3:TColor; BlendMode:Integer); stdcall; //画三角形
begin
  GameCanvas.FillTri(p1, p2, p3, c1, c2, c3, 0, BlendMode);
end;

procedure TDrawAPI_Circle(X, Y, Radius:Single; Color:TColor; Filled:Boolean; BlendMode:Integer); stdcall; //画圆
begin
  GameCanvas.Circle(X, Y, Radius, Color, Filled, 0, BlendMode);
end;

function TDrawAPI_CurrentFont:THGEFont; stdcall; //当前默认字体
begin
  Result := CurrentFont;
end;

function TDrawAPI_FindFont(FontName:PChar; FontSize:Integer; FontStyles:TFontStyles):THGEFont; stdcall;
begin
  Result := TextureFonts.FindFont(FontName, FontSize, FontStyles);
end;

procedure TDrawAPI_TextRect(HGEFont:THGEFont; X, Y:Integer; SrcRect:TRect; Text:PChar; Color:TColor; BlendMode:Integer); stdcall;
begin
  HGEFont.TextRect(X, Y, SrcRect, Text, Color, BlendMode);
end;

procedure TDrawAPI_TextOut(HGEFont:THGEFont; X, Y:Integer; Text:PChar; Color:TColor; BlendMode:Integer); stdcall;
begin
  HGEFont.TextOut(X, Y, Text, Color, BlendMode);
end;

procedure TDrawAPI_BoldTextOut(HGEFont:THGEFont; X, Y:Integer; Text:PChar; FColor, BColor:TColor); stdcall;
begin
  BoldTextOut(HGEFont, X, Y, Text, FColor, BColor);
end;

function TDrawAPI_TextWidth(HGEFont:THGEFont; Text:PChar):Integer; stdcall;
begin
  Result := HGEFont.TextWidth(string(Text));
end;

function TDrawAPI_TextHeight(HGEFont:THGEFont; Text:PChar):Integer; stdcall;
begin
  Result := HGEFont.TextHeight(string(Text));
end;
//------------------------------------------------------------------------------

function TGameInterfaceAPI_DMainMenu:TDxControl; stdcall; //右键弹出菜单
begin
  Result := FrmDlg.DMainMenu;
end;

function TGameInterfaceAPI_DLoginDlg:TDxControl; stdcall; //登录背景窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DLoginDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DLoginDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DLoginDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DRandomCodeDlg:TDxControl; stdcall; //随机码窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DRandomCodeDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DRandomCodeDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DRandomCodeDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DLogin:TDxControl; stdcall; //登录对话框窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DLogin
      {
    else if FrmDlg is TMirsWindows then
      Result := nil                                                                                   //TMirsWindows(FrmDlg).DLogin
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DLogin
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DNewAccount:TDxControl; stdcall; //注册帐号窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DNewAccount
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DNewAccount
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DNewAccount
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DChgPw:TDxControl; stdcall; //修改密码窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DChgPw
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DChgPw
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DChgPw
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DSelServerDlg:TDxControl; stdcall; //选择服务器背景窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DSelServerDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DSelServerDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DSelServerDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DServerDlg:TDxControl; stdcall; //选择服务器窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DServerDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DServerDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DServerDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DDoorDlg:TDxControl; stdcall; //开门时的背景窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DSelServerDlg
  else
    Result := nil;
end;

function TGameInterfaceAPI_DSelectChr:TDxControl; stdcall; //选择角色背景窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DSelectChr
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DSelectChr
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DSelectChr
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DCreateChr:TDxControl; stdcall; //创建角色窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DCreateChr
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DCreateChr
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DCreateChr
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DDeleteHumanDlg:TDxControl; stdcall; //恢复角色窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DDeleteHumanDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DDeleteHumanDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DDeleteHumanDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DNoticeDlg:TDxControl; stdcall; //公告窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DNoticeDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DNoticeDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DNoticeDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DMerchantDlg:TDxControl; stdcall; //NPC对话框
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMerchantDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DMerchantDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DMerchantDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DBottomLeft:TDxControl; stdcall; //游戏界面左
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMainBottomDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DBottomLeft
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DBottomLeft
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DBottomCenter:TDxControl; stdcall; //游戏界面中
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMainBottomDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DBottomCenter
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DBottomCenter
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DBottomRight:TDxControl; stdcall; //游戏界面右
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMainBottomDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DBottomRight
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DBottomRight
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DItemBag:TDxControl; stdcall; //包裹窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DItemBag
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DItemBag
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DItemBag
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DStateWin:TDxControl; stdcall; //人物属性窗口 自己
begin
  if FrmDlg is TSerialWindows then begin
    if (g_ClientVersion in [cvSerial, cvMirSequel, cvMirNewUI205]) and (not g_ClientConfig.boUseOldSerialWindows) then
      Result := TSerialWindows(FrmDlg).NewStateWindows.DStateWin
    else
      Result := TSerialWindows(FrmDlg).DStateWin;
  end
    {
    else if FrmDlg is TMirsWindows then
    begin
      Result := TMirsWindows(FrmDlg).DStateWin
    end
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DStateWin
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DUserState1:TDxControl; stdcall; //人物属性窗口 查看对方
begin
  if FrmDlg is TSerialWindows then begin
    if (g_ClientVersion in [cvSerial, cvMirSequel, cvMirNewUI205]) and (not g_ClientConfig.boUseOldSerialWindows) then
      Result := TSerialWindows(FrmDlg).NewStateWindows.DUserState1
    else
      Result := TSerialWindows(FrmDlg).DUserState1;
  end
    {
    else if FrmDlg is TMirsWindows then
    begin
      Result := TMirsWindows(FrmDlg).DUserState1
    end
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DUserState1
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DHeroStateWin:TDxControl; stdcall; //英雄属性窗口
begin
  if FrmDlg is TSerialWindows then begin
    if (g_ClientVersion in [cvSerial, cvMirSequel, cvMirNewUI205]) and (not g_ClientConfig.boUseOldSerialWindows) then
      Result := TSerialWindows(FrmDlg).NewStateWindows.DHeroStateWin
    else
      Result := TSerialWindows(FrmDlg).DHeroStateWin;
  end
    {
    else if FrmDlg is TMirsWindows then
    begin
      Result := TMirsWindows(FrmDlg).DHeroStateWin
    end
    else if FrmDlg is TReturnWindows then
      Result := nil                                                                                   //TReturnWindows(FrmDlg).DHeroStateWin
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DHeroStateDlg:TDxControl; stdcall; //英雄状态窗口
begin
  if FrmDlg is TSerialWindows then begin
    if (g_ClientVersion in [cvSerial, cvMirSequel, cvMirNewUI205]) and (not g_ClientConfig.boUseOldSerialWindows) then
      Result := TSerialWindows(FrmDlg).DHeroStateDlg
    else
      Result := TSerialWindows(FrmDlg).DHeroStateDlg185;
  end
    {
    else if FrmDlg is TMirsWindows then
    begin
      Result := TMirsWindows(FrmDlg).DHeroStateDlg
    end
    else if FrmDlg is TReturnWindows then
      Result := nil                                                                                   //TReturnWindows(FrmDlg).DHeroStateDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DHeroItemBag:TDxControl; stdcall; //英雄包裹窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DHeroItemBag
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DHeroItemBag
    else if FrmDlg is TReturnWindows then
      Result := nil                                                                                   //TReturnWindows(FrmDlg).DHeroItemBag
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DMenuDlg:TDxControl; stdcall; // NPC列表框
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMenuDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DMenuDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DMenuDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DSellDlg:TDxControl; stdcall; //OK框
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DSellDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DSellDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DSellDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DDealDlg:TDxControl; stdcall; //交易对话框 自己
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DDealDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DDealDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DDealDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DDealRemoteDlg:TDxControl; stdcall; //交易对话框 对方
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DDealRemoteDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DDealRemoteDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DDealRemoteDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DShopDlg:TDxControl; stdcall; //商铺窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DShopDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DShopDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DShopDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DGroupDlg:TDxControl; stdcall; //组队窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DGroupDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DGroupDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DGroupDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DRankingDlg:TDxControl; stdcall; //排行榜窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DRankingDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DRankingDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DRankingDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DGuildDlg:TDxControl; stdcall; //行会窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DGuildDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DGuildDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DGuildDlg
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DGuildEditNotice:TDxControl; stdcall; //行会编辑窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DGuildEditNotice
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DGuildEditNotice
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DGuildEditNotice
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DAdjustAbility:TDxControl; stdcall; //附加属性窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DAdjustAbility
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DAdjustAbility
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DAdjustAbility
    }
  else
    Result := nil;
end;

function TGameInterfaceAPI_DMissionDlg:TDxControl; stdcall; //任务日记窗口
begin
  if FrmDlg is TSerialWindows then
    Result := TSerialWindows(FrmDlg).DMissionDlg
      {
    else if FrmDlg is TMirsWindows then
      Result := TMirsWindows(FrmDlg).DMissionDlg
    else if FrmDlg is TReturnWindows then
      Result := TReturnWindows(FrmDlg).DMissionDlg
    }
  else
    Result := nil;
end;
//------------------------------------------------------------------------------

procedure TSocketAPI_SendSocket(S:PChar); stdcall;
begin
  frmMain.SendSocket(S);
end;

// 2021-01-06 changed

procedure TSocketAPI_SendClientMessage(Msg:Word; Recog:Int64; param, tag, series:Word; S:PChar); stdcall;
var
  DefMsg:TDefaultMessage;
  sSendText:string;
begin
  DefMsg := MakeDefaultMsg(Msg, Recog, param, tag, series);
  sSendText := EncodeMessage(DefMsg);
  if S <> nil then
    sSendText := sSendText + EncodeString(string(S));
  frmMain.SendSocket(sSendText);
end;

procedure TSocketAPI_SendLogin(uid, passwd:PChar); stdcall;
begin
  frmMain.SendLogin(uid, passwd);
end;

procedure TSocketAPI_SendSelectServer(sServerName:PChar); stdcall;
begin
  frmMain.SendSelectServer(sServerName);
end;

procedure TSocketAPI_SendSay(S:PChar); stdcall;
begin
  frmMain.SendSay(S);
end;

procedure TSocketAPI_SendQueryChr(); stdcall;
begin
  frmMain.SendQueryChr();
end;

procedure TSocketAPI_SendSelChr(sChrName:PChar); stdcall;
begin
  frmMain.SendSelChr(sChrName);
end;

procedure TSocketAPI_Close; stdcall; //断开连接
begin
  frmMain.CSocket.Close;
end;
//------------------------------------------------------------------------------

function TGameAPI_AreaStateValue:Boolean; stdcall; //当前是在攻城区域
begin
  Result := (g_nAreaStateValue and $04) <> 0;
end;

function TGameAPI_MapTitle:PChar; stdcall; //当前地图名称
begin
  Result := PChar(g_sMapTitle);
end;

procedure TGameAPI_EatItem(Idx:Integer); stdcall;
begin
  frmMain.EatItem(Idx);
end;

procedure TGameAPI_HeroEatItem(Idx:Integer); stdcall;
begin
  frmMain.HeroEatItem(Idx);
end;

function TGameAPI_SellDlgItem:Pointer; stdcall; // 当前OK框物品
begin
  Result := @g_SellDlgItem;
end;

function TGameAPI_ClientPath:PChar; stdcall; //登录器路径
begin
  Result := PChar(g_sSelfFilePath);
end;

function TGameAPI_ClientName:PChar; stdcall; //登录器名称
begin
  Result := PChar(ExtractFileName(g_sSelfFileName));
end;

function TGameAPI_ServerImageList:TStringList; stdcall;
begin
  Result := g_EffectImageList;
end;

function TGameAPI_ClassDlg:TObject; stdcall;
begin
  Result := FrmDlg;
end;

procedure TGameAPI_SetMovingItem(Item:Pointer); stdcall; //pTMovingItem; //当前正在移动的物品
begin
  g_MovingItem := pTMovingItem(Item)^;
end;

procedure TGameAPI_SetWaitingUseItem(Item:Pointer); stdcall; //pTMovingItem;
begin
  g_WaitingUseItem := pTMovingItem(Item)^;
end;

procedure TGameAPI_SetSellDlgItem(Item:pTClientItem); stdcall; //pTMovingItem; 当前OK框物品
begin
  g_SellDlgItem := Item^;
end;

procedure TGameAPI_SetItemBag(Index:Integer; Item:pTClientItem); stdcall; //包裹物品
begin
  if (Index >= Low(g_ItemArr)) and (Index <= GetMaxBagCount - 1) then
    g_ItemArr[Index] := Item^;
end;

procedure TGameAPI_SetHeroItemBag(Index:Integer; Item:pTClientItem); stdcall; //英雄包裹物品
begin
  if (Index >= Low(g_HeroItemArr)) and (Index <= High(g_HeroItemArr)) then
    g_HeroItemArr[Index] := Item^;
end;

procedure TGameAPI_SetUseItems(Index:Integer; Item:pTClientItem); stdcall; //身上装备
begin
  if (Index >= Low(g_UseItems)) and (Index <= High(g_UseItems)) then
    g_UseItems[Index] := Item^;
end;

procedure TGameAPI_SetHeroUseItems(Index:Integer; Item:pTClientItem); stdcall; //英雄身上装备
begin
  if (Index >= Low(g_HeroUseItems)) and (Index <= High(g_HeroUseItems)) then
    g_HeroUseItems[Index] := Item^;
end;

procedure TGameAPI_SetJewelryBoxItems(Index:Integer; Item:pTClientItem); stdcall; // 首饰盒物品指针
begin
  if (Index >= Low(g_JewelryBoxItems)) and (Index <= High(g_JewelryBoxItems)) then
    g_JewelryBoxItems[Index] := Item^;
end;

procedure TGameAPI_SetHeroJewelryBoxItems(Index:Integer; Item:pTClientItem); stdcall; // 英雄首饰盒物品指针
begin
  if (Index >= Low(g_HeroJewelryBoxItems)) and (Index <= High(g_HeroJewelryBoxItems)) then
    g_HeroJewelryBoxItems[Index] := Item^;
end;

procedure TGameAPI_SetGodBlessItems(Index:Integer; Item:pTClientItem); stdcall; // 神佑盒物品指针
begin
  if (Index >= Low(g_GodBlessItems)) and (Index <= High(g_GodBlessItems)) then
    g_GodBlessItems[Index] := Item^;
end;

procedure TGameAPI_SetHeroGodBlessItems(Index:Integer; Item:pTClientItem); stdcall; // 英雄神佑盒物品指针
begin
  if (Index >= Low(g_HeroGodBlessItems)) and (Index <= High(g_HeroGodBlessItems)) then
    g_HeroGodBlessItems[Index] := Item^;
end;

procedure TGameAPI_SetItemBoxItems(Index:Integer; Item:pTClientItem); stdcall; // 自定义OK框物品指针
begin
  if (Index >= Low(g_ItemBoxItems)) and (Index <= High(g_ItemBoxItems)) then
    g_ItemBoxItems[Index] := Item^;
end;

procedure TGameAPI_SetUserState1(UserStateInfo:pTUserStateInfo); stdcall; //查看的别人身上装备
begin
  g_UserState1 := UserStateInfo^;
end;

procedure TGameAPI_SetItemMoving(Value:Boolean); stdcall;
begin
  g_boItemMoving := Value;

end;

function TGameAPI_MySelf:TActor; stdcall;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Result := g_MySelf;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_MyHero:TActor; stdcall;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Result := g_MyHero;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_MagicList:TList; stdcall;
begin
  Result := g_MagicList;
end;

function TGameAPI_MagicNGList:TList; stdcall;
begin
  Result := g_MagicNGList;
end;

function TGameAPI_ContinuousMagicList:TList; stdcall;
begin
  Result := g_ContinuousMagicList;
end;

function TGameAPI_HeroMagicList:TList; stdcall;
begin
  Result := g_HeroMagicList;
end;

function TGameAPI_HeroMagicNGList:TList; stdcall;
begin
  Result := g_HeroMagicNGList;
end;

function TGameAPI_HeroContinuousMagicList:TList; stdcall;
begin
  Result := g_HeroContinuousMagicList;
end;

function TGameAPI_GroupMembers:TList; stdcall;
begin
  Result := g_GroupMembers;
end;

function TGameAPI_DropedItemList:TDropItemsMgr; stdcall;
begin
  Result := g_DropItemsMgr;
end;

function TGameAPI_MenuItemList:TList; stdcall;
begin
  Result := g_MenuItemList;
end;

function TGameAPI_ActorList:TList; stdcall;
begin
  Result := PlayScene.m_ActorList;
end;

procedure TGameAPI_ScreenXYfromMCXY(cx, cy:Integer; var sx, sY:Integer); stdcall; //地图坐标转换屏幕坐标
begin
  PlayScene.ScreenXYfromMCXY(cx, cy, sx, sY);
end;

procedure TGameAPI_CXYfromMouseXY(mx, my:Integer; var ccx, ccy:Integer); stdcall; //屏幕坐标转换地图坐标
begin
  PlayScene.CXYfromMouseXY(mx, my, ccx, ccy);
end;

function TGameAPI_FindActor1(nRecogId:Int64):TActor; stdcall; // 2020-01-11改64位支持
begin
  Result := PlayScene.FindActor(nRecogId);
end;

function TGameAPI_FindActor2(sName:PChar):TActor; stdcall;
begin
  Result := PlayScene.FindActor(sName);
end;

function TGameAPI_FindActorXY1(X, Y:Integer):TActor; stdcall;
begin
  Result := PlayScene.FindActorXY(X, Y);
end;

function TGameAPI_FindActorXY2(X, Y:Integer; Actor:TActor):TActor; stdcall;
begin
  Result := PlayScene.FindActorXY(X, Y, Actor);
end;

function TGameAPI_CanWalk(mx, my:Integer):Boolean; stdcall;
begin
  Result := PlayScene.CanWalkEx(mx, my);
end;

function TGameAPI_CanRun(sx, sY, ex, ey:Integer):Boolean; stdcall;
begin
  Result := PlayScene.CanRun(sx, sY, ex, ey);
end;

function TGameAPI_CanHorseRun(sx, sY, ex, ey:Integer):Boolean; stdcall;
begin
  Result := PlayScene.CanHorseRun(sx, sY, ex, ey);
end;

function TGameAPI_GetRGB(c256:Byte):Integer; stdcall; //获取颜色
begin
  Result := GetRGB(c256);
end;

procedure TGameAPI_DebugOutStr(Msg:PChar; boWriteDate:Boolean); stdcall; //写入日记
begin
  DebugOutStr(Msg, boWriteDate);
end;

procedure TGameAPI_AppLogout; stdcall; //小退
begin
  g_IsWaitLogout := True;
  frmMain.RzToolButtonBackGameClick(frmMain);
  frmMain.Logout; //小退
end;

procedure TGameAPI_AppExit; stdcall; //退出游戏
begin
  frmMain.RzToolButtonBackGameClick(frmMain);
  frmMain.Close;
end;

function TGameAPI_DMessageDlg(Msg:PChar; DlgButtons:TMsgDlgButtons):TModalResult; stdcall; //弹出对话框
begin
  //showmessage('TGameAPI_DMessageDlg '+ Msg);
  Result := FrmDlg.DMessageDlg(Msg, DlgButtons);
end;

procedure TGameAPI_AddChatBoardString(Msg:PChar; FColor, BColor:Byte); stdcall; //聊天框显示信息
begin
  DScreen.AddChatBoardString(Msg, GetRGB(FColor), GetRGB(BColor));
end;

procedure TGameAPI_AddTopChatBoardString(Msg:PChar; FColor, BColor:Byte; TimeOut:Integer); stdcall; //聊天框固顶信息
begin
  DScreen.AddTopChatBoardString(Msg, GetRGB(FColor), GetRGB(BColor), TimeOut);
end;

procedure TGameAPI_AddMoveMsg(Msg:PChar; FColor, BColor:Byte; nX, nY, nCount:Integer); stdcall; //滚动信息
begin
  DScreen.AddMoveMsg(Msg, FColor, BColor, nY, nCount);
end;

procedure TGameAPI_ShowHint(X, Y:Integer; Msg:PChar; Color:TColor; DrawUp:Boolean; DrawLeft:Boolean; ShowBackground:Boolean); stdcall; //显示悬浮框
begin
  HintWindows.Show(X, Y, Msg, Color, DrawUp, DrawLeft, ShowBackground);
end;

procedure TGameAPI_ShowMouseItemInfo(Actor:TActor; MouseItem:pTClientItem; X, Y:Integer; Secret {是否是神秘装备}:Boolean; ShowTzItemDesc {套装备注类型 0=自己 1=英雄 2=查看的人其他人}:Byte; DrawUp:Boolean; DrawLeft:Boolean); stdcall; //悬浮框显示装备信息
begin
  FrmDlg.ShowMouseItemInfo(Actor, MouseItem, X, Y, Secret, ShowTzItemDesc, DrawUp, DrawLeft);
end;

procedure TGameAPI_ClearHint; stdcall; //清除悬浮框
begin
  HintWindows.Clear;
end;

function TGameAPI_DlgEditText:PChar; stdcall; //DMessageDlg 对话框用户输入的信息
begin
  Result := PChar(FrmDlg.DlgEditText);
end;

procedure TGameAPI_PlaySound(idx:Integer); stdcall;
begin
  PlaySound(idx);
end;

procedure TGameAPI_PlaySoundA(sFileName:PChar; LoopCount:Integer); stdcall;
begin
  PlaySound(sFileName, LoopCount);
end;

procedure TGameAPI_ItemClickSound(StdItem:TStdItem); stdcall;
begin
  ItemClickSound(StdItem);
end;

function TGameAPI_ItemBag:pTClientBagItems; stdcall; //包裹物品指针
begin
  Result := @g_ItemArr;
end;

function TGameAPI_HeroItemBag:pTClientHeroBagItems; stdcall; //英雄包裹物品指针
begin
  Result := @g_HeroItemArr;
end;

function TGameAPI_UseItems:pTUseItems; stdcall; //身上装备指针
begin
  Result := @g_UseItems;
end;

function TGameAPI_HeroUseItems:pTUseItems; stdcall; //英雄身上装备指针
begin
  Result := @g_HeroUseItems;
end;

function TGameAPI_JewelryBoxItems:pTJewelryBoxItems; stdcall // 首饰盒物品指针
begin
  Result := @g_JewelryBoxItems;
end;

function TGameAPI_HeroJewelryBoxItems:pTJewelryBoxItems; stdcall // 英雄首饰盒物品指针
begin
  Result := @g_HeroJewelryBoxItems;
end;

function TGameAPI_GodBlessItems:PTGodBlessItems; stdcall // 神佑盒物品指针
begin
  Result := @g_GodBlessItems;
end;

function TGameAPI_HeroGodBlessItems:PTGodBlessItems; stdcall // 英雄神佑盒物品指针
begin
  Result := @g_HeroGodBlessItems;
end;

function TGameAPI_ItemBoxItems:PTItemBoxItems; stdcall // 自定义OK框物品指针
begin
  Result := @g_ItemBoxItems;
end;

function TGameAPI_UserState1:pTUserStateInfo; stdcall; //查看的别人身上装备指针
begin
  Result := @g_UserState1;
end;

function TGameAPI_MovingItem:Pointer; stdcall; //pTMovingItem; //当前正在移动的物品指针
begin
  Result := @g_MovingItem;
end;

function TGameAPI_boItemMoving:Boolean; stdcall;
begin
  Result := g_boItemMoving;
end;

function TGameAPI_WaitingUseItem:Pointer; stdcall; //pTMovingItem;
begin
  Result := @g_WaitingUseItem;
end;

function TGameAPI_nTargetX:PInteger; stdcall; //目标座标
begin
  Result := @g_nTargetX;
end;

function TGameAPI_nTargetY:PInteger; stdcall; //目标座标
begin
  Result := @g_nTargetY;
end;

function TGameAPI_TargetCret:pTActor; stdcall;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Result := @g_TargetCret;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_FocusCret:TActor; stdcall;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Result := g_FocusCret;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_MagicTarget:pTActor; stdcall;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  Result := @g_MagicTarget;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_Gold:Integer; stdcall; //金币数量
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  if g_MySelf <> nil then
    Result := g_MySelf.m_nGold
  else
    Result := 0;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}

end;

function TGameAPI_GameGold:Integer; stdcall; //元宝数量
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  if g_MySelf <> nil then
    Result := g_MySelf.m_nGameGold
  else
    Result := 0;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_GamePoint:Integer; stdcall; //游戏点数量
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  if g_MySelf <> nil then
    Result := g_MySelf.m_nGamePoint
  else
    Result := 0;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_GloryPoint:Integer; stdcall; //荣誉
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  if g_MySelf <> nil then
    Result := g_MySelf.m_nGloryPoint
  else
    Result := 0;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}
end;

function TGameAPI_GameDiamond:Integer; stdcall; //金刚石
begin
  Result := g_nGameDiamond;
end;

function TGameAPI_GameGird:Integer; stdcall; //灵符
begin
  Result := g_nGameGird;
end;

function TGameAPI_GameGlory:Integer; stdcall; //荣誉
begin
  Result := g_nGameGlory;
end;

function TGameAPI_LoyaltyPoint:Integer; stdcall; //忠诚度
begin
  Result := g_nLoyaltyPoint;
end;

function TGameAPI_GameGoldName:PChar; stdcall;
begin
  Result := PChar(g_sGameGoldName);
end;

function TGameAPI_GamePointName:PChar; stdcall;
begin
  Result := PChar(g_sGamePointName);
end;

function TGameAPI_GameDiamondName:PChar; stdcall;
begin
  Result := PChar(g_sGameDiamondName);
end;

function TGameAPI_GameGirdName:PChar; stdcall;
begin
  Result := PChar(g_sGameGirdName);
end;

function TGameAPI_EncodeBuffer(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;
var
  Len:Integer;
begin
  Len := GetEncodeSize(InBytes);
  Result := Encode6BitBuf(InData, OutData, InBytes, Len);
end;

function TGameAPI_DecodeBuffer(InData:PChar; InBytes:Integer; OutData:PChar):Integer; stdcall;
var
  Len:Integer;
begin
  Len := GetDecodeSize(InBytes);
  Result := Decode6BitBuf(InData, OutData, InBytes, Len);
end;

procedure TGameAPI_FullScreenDrawScene(boFullScreen:Boolean);
begin
  g_boFullScreenDrawScene := boFullScreen;
  if g_boFullScreenDrawScene then begin
    MAPSURFACEHEIGHT := SCREENHEIGHT;
  end
  else begin
    MAPSURFACEHEIGHT := SCREENHEIGHT - 150;
  end;
  //MAPSURFACEHEIGHT := Min(MAPSURFACEHEIGHT + SceneHeight, SCREENHEIGHT);
end;
//------------------------------------------------------------------------------

function TDControl_Create(Parent:TDxControl; InterfaceType:TGuiType):TDxControl; stdcall;
var
  DxControl, AOwner:TDxControl;
begin
  if Parent = nil then
    AOwner := FrmDlg.DBackground
  else
    AOwner := Parent;
  DxControl := nil;
  case InterfaceType of
    t_Form:DxControl := TDxImageForm.Create(AOwner);
    t_FormShape:DxControl := TDxImageFormShape.Create(AOwner);
    t_Button:DxControl := TDxImageButton.Create(AOwner);
    t_Edit:DxControl := TDxEdit.Create(AOwner);
    t_ImageEdit:DxControl := TDxImageEdit.Create(AOwner);
    t_Label:DxControl := TDxLabel.Create(AOwner);
    t_Grid:DxControl := TDxImageGrid.Create(AOwner);
    t_ScrollBox:DxControl := TDxScrollBox.Create(AOwner);
    t_ChatMemo:DxControl := TDxChatMemo.Create(AOwner);
    t_ListView:DxControl := TDxListView.Create(AOwner);
    t_TreeView:DxControl := TDxTreeView.Create(AOwner);
    t_PopupMenu:DxControl := TDxPopupMenu.Create(AOwner);
    t_TabSheet:DxControl := TDxTabSheet.Create(AOwner);
    t_PageControl:DxControl := TDxPageControl.Create(AOwner);
    t_ComboBox:DxControl := TDxComboBox.Create(AOwner);
    t_Line:DxControl := TDxLine.Create(AOwner);
    t_TrackBar:DxControl := TDxTrackBar.Create(AOwner);
  end;
  if DxControl <> nil then begin
    DxControl.GuiType := InterfaceType;
    DxControl.Designing := False;
    DxControl.OnGetImage := AOwner.OnGetImage;
    if InterfaceType in [t_Label, t_Button] then
      TDxImageButton(DxControl).OnClickSound := FrmDlg.DLoginNewClickSound;
  end;
  Result := DxControl;
end;

function TDControl_InterfaceType(D:TDxControl):TGuiType; stdcall; //类型
begin
  Result := D.GuiType;
end;

function TDControl_Name(D:TDxControl):PChar; stdcall; //名称
begin
  Result := PChar(D.Name);
end;

function TDControl_Left(D:TDxControl):Integer; stdcall; //左
begin
  Result := D.Left;
end;

function TDControl_Top(D:TDxControl):Integer; stdcall; //上
begin
  Result := D.Top;
end;

function TDControl_Width(D:TDxControl):Integer; stdcall; //宽
begin
  Result := D.Width;
end;

function TDControl_Height(D:TDxControl):Integer; stdcall; //高
begin
  Result := D.Height;
end;

function TDControl_Tag(D:TDxControl):Integer; stdcall; //标志
begin
  Result := D.Tag;
end;

function TDControl_Visible(D:TDxControl):Boolean; stdcall; //是否可见
begin
  Result := D.Visible;

end;

function TDControl_Enabled(D:TDxControl):Boolean; stdcall; //是否可用
begin
  Result := D.Enabled;
end;

function TDControl_Floating(D:TDxControl):Boolean; stdcall; //是否可以移动
begin
  Result := D.Floating;
end;

function TDControl_ParentMove(D:TDxControl):Boolean; stdcall; //Parent是否可以移动
begin
  Result := D.OwnerMove;
end;

function TDControl_EnableFocus(D:TDxControl):Boolean; stdcall; //是否可以设置焦点
begin
  Result := D.EnableFocus;
end;

function TDControl_AutoSize(D:TDxControl):Boolean; stdcall; //是否根据图片尺寸自动调整尺寸
begin
  Result := D.AutoSize;
end;

function TDControl_DrawBorder(D:TDxControl):Boolean; stdcall; //绘制边框
begin
  Result := D.DrawBorder;
end;

function TDControl_Caption(D:TDxControl):PChar; stdcall; //标题
begin
  Result := PChar(D.Caption);
end;

function TDControl_Alignment(D:TDxControl):TAlignment; stdcall; //标题显示位置
begin
  Result := D.Alignment;
end;

function TDControl_Transparent(D:TDxControl):Boolean; stdcall; //是否透明
begin
  Result := D.Transparent;
end;

function TDControl_BackgroundColor(D:TDxControl):TColor; stdcall; //背景色
begin
  Result := D.BackgroundColor;
end;

function TDControl_PopupMenu(D:TDxControl):TDxControl; stdcall; //弹出菜单
begin
  Result := D.PopupMenu;
end;

procedure TDControl_VisibleRect(D:TDxControl; var Value:TRect); stdcall; //组件可见矩形
begin
  Value := D.VisibleRect;
end;

procedure TDControl_VirtualRect(D:TDxControl; var Value:TRect); stdcall; //组件真实矩形
begin
  Value := D.VirtualRect;
end;

function TDControl_DefaultBorderColor(D:TDxControl):TColor; stdcall; //边框默认颜色
begin
  Result := D.BorderColor.Up.Color;
end;

function TDControl_DefaultBorderBold(D:TDxControl):Boolean; stdcall; //边框默认是否加粗
begin
  Result := D.BorderColor.Up.Bold;
end;

function TDControl_MouseMoveBorderColor(D:TDxControl):TColor; stdcall; //边框鼠标移动颜色
begin
  Result := D.BorderColor.Hot.Color;
end;

function TDControl_MouseMoveBorderBold(D:TDxControl):Boolean; stdcall; //边框鼠标移动是否加粗
begin
  Result := D.BorderColor.Hot.Bold;
end;

function TDControl_MouseDownBorderColor(D:TDxControl):TColor; stdcall; //边框鼠标按下颜色
begin
  Result := D.BorderColor.Down.Color;
end;

function TDControl_MouseDownBorderBold(D:TDxControl):Boolean; stdcall; //边框鼠标按下是否加粗
begin
  Result := D.BorderColor.Down.Bold;
end;

function TDControl_DisabledBorderColor(D:TDxControl):TColor; stdcall; //边框不可用时颜色
begin
  Result := D.BorderColor.Disabled.Color;
end;

function TDControl_DisabledBorderBold(D:TDxControl):Boolean; stdcall; //边框不可用时是否加粗
begin
  Result := D.BorderColor.Disabled.Bold;
end;

function TDControl_Images(D:TDxControl):THandle; stdcall; //图库
begin
  Result := THandle(D.ImageIndex.Image);
end;

function TDControl_DefaultImageIndex(D:TDxControl):Integer; stdcall; //默认图库编号
begin
  Result := D.ImageIndex.Up;
end;

function TDControl_MouseMoveImageIndex(D:TDxControl):Integer; stdcall; //鼠标移动图库编号
begin
  Result := D.ImageIndex.Hot;
end;

function TDControl_MouseDownImageIndex(D:TDxControl):Integer; stdcall; //鼠标按下图库编号
begin
  Result := D.ImageIndex.Down;
end;

function TDControl_DisabledImageIndex(D:TDxControl):Integer; stdcall; //不可用时图库编号
begin
  Result := D.ImageIndex.Disabled;
end;

function TDControl_OnShow(D:TDxControl):Pointer; stdcall; //显示触发事件
begin
  Result := TMethod(D.OnShow).Code;
end;

function TDControl_OnHide(D:TDxControl):Pointer; stdcall; //隐藏触发事件
begin
  Result := TMethod(D.OnHide).Code;
end;

function TDControl_OnKeyDown(D:TDxControl):Pointer; stdcall; //按键按下事件
begin
  Result := TMethod(D.OnKeyDown).Code;
end;

function TDControl_OnKeyPress(D:TDxControl):Pointer; stdcall; //按键事件
begin
  Result := TMethod(D.OnKeyPress).Code;
end;

function TDControl_OnKeyUp(D:TDxControl):Pointer; stdcall; //按键弹起事件
begin
  Result := TMethod(D.OnKeyUp).Code;
end;

function TDControl_OnClick(D:TDxControl):Pointer; stdcall; //单击事件
begin
  Result := TMethod(D.OnClick).Code;
end;

function TDControl_OnDblClick(D:TDxControl):Pointer; stdcall; //双击事件
begin
  Result := TMethod(D.OnDblClick).Code;
end;

function TDControl_OnMouseDown(D:TDxControl):Pointer; stdcall; //鼠标按下事件
begin
  Result := TMethod(D.OnMouseDown).Code;
end;

function TDControl_OnMouseMove(D:TDxControl):Pointer; stdcall; //鼠标移动事件
begin
  Result := TMethod(D.OnMouseMove).Code;
end;

function TDControl_OnMouseUp(D:TDxControl):Pointer; stdcall; //鼠标弹起事件
begin
  Result := TMethod(D.OnMouseUp).Code;
end;

function TDControl_OnMouseEnter(D:TDxControl):Pointer; stdcall; //鼠标进入事件
begin
  Result := TMethod(D.OnMouseEnter).Code;
end;

function TDControl_OnMouseLeave(D:TDxControl):Pointer; stdcall; //鼠标离开事件
begin
  Result := TMethod(D.OnMouseLeave).Code;
end;

function TDControl_OnInRealArea(D:TDxControl):Pointer; stdcall; //检测鼠标坐标事件
begin
  Result := TMethod(D.OnInRealArea).Code;
end;

function TDControl_OnPaint(D:TDxControl):Pointer; stdcall; //绘制事件
begin
  Result := TMethod(D.OnPaint).Code;
end;

function TDControl_OnStartPaint(D:TDxControl):Pointer; stdcall; //开始绘制事件
begin
  Result := TMethod(D.OnStartPaint).Code;
end;

function TDControl_OnStartSubPaint(D:TDxControl):Pointer; stdcall; //开始绘制子控件事件
begin
  Result := TMethod(D.OnStartSubPaint).Code;
end;

function TDControl_OnStopPaint(D:TDxControl):Pointer; stdcall; //绘制结束事件
begin
  Result := TMethod(D.OnStopPaint).Code;
end;

function TDControl_OnPress(D:TDxControl):Pointer; stdcall; //运行事件
begin
  Result := TMethod(D.OnUpDate).Code;
end;

//------------------------------------------------------------------------------

procedure TDControl_SetName(D:TDxControl; Value:PChar); stdcall; //名称
begin
  D.Name := Value;
end;

procedure TDControl_SetLeft(D:TDxControl; Value:Integer); stdcall;
begin
  D.Left := Value;
end;

procedure TDControl_SetTop(D:TDxControl; Value:Integer); stdcall;
begin
  D.Top := Value;
end;

procedure TDControl_SetWidth(D:TDxControl; Value:Integer); stdcall;
begin
  D.Width := Value;
end;

procedure TDControl_SetHeight(D:TDxControl; Value:Integer); stdcall;
begin
  D.Height := Value;
end;

procedure TDControl_SetTag(D:TDxControl; Value:Integer); stdcall;
begin
  D.Tag := Value;
end;

procedure TDControl_SetVisible(D:TDxControl; Value:Boolean); stdcall;
begin
  D.Visible := Value;
end;

procedure TDControl_SetEnabled(D:TDxControl; Value:Boolean); stdcall;
begin
  D.Enabled := Value;
end;

procedure TDControl_SetFloating(D:TDxControl; Value:Boolean); stdcall;
begin
  D.Floating := Value;
end;

procedure TDControl_SetParentMove(D:TDxControl; Value:Boolean); stdcall;
begin
  D.OwnerMove := Value;
end;

procedure TDControl_SetEnableFocus(D:TDxControl; Value:Boolean); stdcall;
begin
  D.EnableFocus := Value;
end;

procedure TDControl_SetAutoSize(D:TDxControl; Value:Boolean); stdcall;
begin
  D.AutoSize := Value;
end;

procedure TDControl_SetDrawBorder(D:TDxControl; Value:Boolean); stdcall; //绘制边框
begin
  D.DrawBorder := Value;
end;

procedure TDControl_SetCaption(D:TDxControl; Value:PChar); stdcall;
begin
  D.Caption := Value;
end;

procedure TDControl_SetAlignment(D:TDxControl; Value:TAlignment); stdcall;
begin
  D.Alignment := Value;
end;

procedure TDControl_SetTransparent(D:TDxControl; Value:Boolean); stdcall; //是否透明
begin
  D.Transparent := Value;
end;

procedure TDControl_SetBackgroundColor(D:TDxControl; Value:TColor); stdcall; //背景色
begin
  D.BackgroundColor := Value;
end;

procedure TDControl_SetPopupMenu(D:TDxControl; Value:TDxControl); stdcall;
begin
  D.PopupMenu := Value;
end;

procedure TDControl_SetDefaultBorderColor(D:TDxControl; Value:TColor); stdcall; //默认边框颜色
begin
  D.BorderColor.Up.Color := Value;
end;

procedure TDControl_SetDefaultBorderBold(D:TDxControl; Value:Boolean); stdcall; //默认边框是否加粗
begin
  D.BorderColor.Up.Bold := Value;
end;

procedure TDControl_SetMouseMoveBorderColor(D:TDxControl; Value:TColor); stdcall; //边框鼠标移动颜色
begin
  D.BorderColor.Hot.Color := Value;
end;

procedure TDControl_SetMouseMoveBorderBold(D:TDxControl; Value:Boolean); stdcall; //边框鼠标移动是否加粗
begin
  D.BorderColor.Hot.Bold := Value;
end;

procedure TDControl_SetMouseDownBorderColor(D:TDxControl; Value:TColor); stdcall; //边框鼠标按下颜色
begin
  D.BorderColor.Down.Color := Value;
end;

procedure TDControl_SetMouseDownBorderBold(D:TDxControl; Value:Boolean); stdcall; //边框鼠标按下是否加粗
begin
  D.BorderColor.Down.Bold := Value;
end;

procedure TDControl_SetDisabledBorderColor(D:TDxControl; Value:TColor); stdcall; //边框不可用时颜色
begin
  D.BorderColor.Disabled.Color := Value;
end;

procedure TDControl_SetDisabledBorderBold(D:TDxControl; Value:Boolean); stdcall; //边框不可用时是否加粗
begin
  D.BorderColor.Disabled.Bold := Value;
end;

procedure TDControl_SetImages(D:TDxControl; Value:THandle); stdcall;
begin
  D.ImageIndex.Image := TGameImages(Value);
end;

procedure TDControl_SetDefaultImageIndex(D:TDxControl; Value:Integer); stdcall; //默认图库编号
begin
  D.ImageIndex.Up := Value;
end;

procedure TDControl_SetMouseMoveImageIndex(D:TDxControl; Value:Integer); stdcall; //鼠标移动图库编号
begin
  D.ImageIndex.Hot := Value;
end;

procedure TDControl_SetMouseDownImageIndex(D:TDxControl; Value:Integer); stdcall; //鼠标按下图库编号
begin
  D.ImageIndex.Down := Value;
end;

procedure TDControl_SetDisabledImageIndex(D:TDxControl; Value:Integer); stdcall; //不可用时图库编号
begin
  D.ImageIndex.Disabled := Value;
end;

function TDControl_GetImages(D:TDxControl):THandle; stdcall;
begin
  Result := THandle(D.ImageIndex.Image);
end;

function TDControl_GetDefaultImageIndex(D:TDxControl):Integer; stdcall; //默认图库编号
begin
  Result := D.ImageIndex.Up;
end;

function TDControl_GetMouseMoveImageIndex(D:TDxControl):Integer; stdcall; //鼠标移动图库编号
begin
  Result := D.ImageIndex.Hot;
end;

function TDControl_GetMouseDownImageIndex(D:TDxControl):Integer; stdcall; //鼠标按下图库编号
begin
  Result := D.ImageIndex.Down;
end;

function TDControl_GetDisabledImageIndex(D:TDxControl):Integer; stdcall; //不可用时图库编号
begin
  Result := D.ImageIndex.Disabled;
end;

procedure TDControl_SetOnShow(Value:TMethod); stdcall;
var
  OnShow:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnShow := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnShow := nil
    else
      D.OnShow := OnShow;
  end;
end;

procedure TDControl_SetOnHide(Value:TMethod); stdcall;
var
  OnHide:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnHide := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnHide := nil
    else
      D.OnHide := OnHide;
  end;
end;

procedure TDControl_SetOnKeyDown(Value:TMethod); stdcall;
var
  OnKeyDown:TKeyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnKeyDown := TKeyEvent(Value);
    if Value.Code = nil then
      D.OnKeyDown := nil
    else
      D.OnKeyDown := OnKeyDown;
  end;
end;

procedure TDControl_SetOnKeyPress(Value:TMethod); stdcall;
var
  OnKeyPress:TKeyPressEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnKeyPress := TKeyPressEvent(Value);
    if Value.Code = nil then
      D.OnKeyPress := nil
    else
      D.OnKeyPress := OnKeyPress;
  end;
end;

procedure TDControl_SetOnKeyUp(Value:TMethod); stdcall;
var
  OnKeyUp:TKeyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnKeyUp := TKeyEvent(Value);
    if Value.Code = nil then
      D.OnKeyUp := nil
    else
      D.OnKeyUp := OnKeyUp;
  end;
end;

procedure TDControl_SetOnClick(Value:TMethod); stdcall;
var
  OnClick:TOnClickEx;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnClick := TOnClickEx(Value);
    if Value.Code = nil then
      D.OnClick := nil
    else
      D.OnClick := OnClick;
  end;
end;

procedure TDControl_SetOnDblClick(Value:TMethod); stdcall;
var
  OnDblClick:TOnClickEx;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnDblClick := TOnClickEx(Value);
    if Value.Code = nil then
      D.OnDblClick := nil
    else
      D.OnDblClick := OnDblClick;
  end;
end;

procedure TDControl_SetOnMouseDown(Value:TMethod); stdcall;
var
  OnMouseDown:TMouseEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnMouseDown := TMouseEvent(Value);
    if Value.Code = nil then
      D.OnMouseDown := nil
    else
      D.OnMouseDown := OnMouseDown;
  end;
end;

procedure TDControl_SetOnMouseMove(Value:TMethod); stdcall;
var
  OnMouseMove:TMouseMoveEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnMouseMove := TMouseMoveEvent(Value);
    if Value.Code = nil then
      D.OnMouseMove := nil
    else
      D.OnMouseMove := OnMouseMove;
  end;
end;

procedure TDControl_SetOnMouseUp(Value:TMethod); stdcall;
var
  OnMouseUp:TMouseEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnMouseUp := TMouseEvent(Value);
    if Value.Code = nil then
      D.OnMouseUp := nil
    else
      D.OnMouseUp := OnMouseUp;
  end;
end;

procedure TDControl_SetOnMouseEnter(Value:TMethod); stdcall;
var
  OnMouseEnter:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnMouseEnter := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnMouseEnter := nil
    else
      D.OnMouseEnter := OnMouseEnter;
  end;
end;

procedure TDControl_SetOnMouseLeave(Value:TMethod); stdcall;
var
  OnMouseLeave:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnMouseLeave := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnMouseLeave := nil
    else
      D.OnMouseLeave := OnMouseLeave;
  end;
end;

procedure TDControl_SetOnInRealArea(Value:TMethod); stdcall;
var
  OnInRealArea:TOnInRealArea;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnInRealArea := TOnInRealArea(Value);
    if Value.Code = nil then
      D.OnInRealArea := nil
    else
      D.OnInRealArea := OnInRealArea;
  end;
end;

procedure TDControl_SetOnPaint(Value:TMethod); stdcall;
var
  OnPaint:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnPaint := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnPaint := nil
    else
      D.OnPaint := OnPaint;
  end;
end;

procedure TDControl_SetOnStartPaint(Value:TMethod); stdcall;
var
  OnStartPaint:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnStartPaint := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnStartPaint := nil
    else
      D.OnStartPaint := OnStartPaint;
  end;
end;

procedure TDControl_SetOnStartSubPaint(Value:TMethod); stdcall;
var
  OnStartSubPaint:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnStartSubPaint := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnStartSubPaint := nil
    else
      D.OnStartSubPaint := OnStartSubPaint;
  end;
end;

procedure TDControl_SetOnStopPaint(Value:TMethod); stdcall;
var
  OnStopPaint:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnStopPaint := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnStopPaint := nil
    else
      D.OnStopPaint := OnStopPaint;
  end;
end;

procedure TDControl_SetOnPress(Value:TMethod); stdcall;
var
  OnUpDate:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnUpDate := TNotifyEvent(Value);
    if Value.Code = nil then
      D.OnUpDate := nil
    else
      D.OnUpDate := OnUpDate;
  end;
end;
//-----------------------------------------------------------------------------

function TDControl_Parent(D:TDxControl):TDxControl; stdcall; //显示在主控件 为NIL时为主表面
begin
  Result := TDxControl(D.Owner);
end;

procedure TDControl_SetParent(D, Value:TDxControl); stdcall; //设置显示在主控件 为NIL时为主表面
begin
  if D.Owner <> nil then begin
    D.Owner.RemoveComponent(D);
  end;
  Value.InserComponent(D);
end;

function TDControl_ControlCount(D:TDxControl):Integer; stdcall; //子控件数
begin
  if D = nil then
    Result := FrmDlg.DBackground.ControlCount
  else
    Result := D.ControlCount;
end;

function TDControl_Controls(D:TDxControl; Index:Integer):TDxControl; stdcall; //获取子控件
begin
  if D = nil then
    Result := FrmDlg.DBackground.Control[Index]
  else
    Result := D.Control[Index];
end;

procedure TDControl_SetFocus(D:TDxControl); stdcall; //设置为焦点
begin
  D.SetFocus;
end;

procedure TDControl_BringToFront(D:TDxControl); stdcall; //显示到最上层
begin
  D.BringToFront;
end;

function TDControl_InRange(D:TDxControl; X, Y:Integer):Boolean; stdcall; //检测鼠标是否在范围
begin
  Result := D.InRange(X, Y);
end;

//------------------------------------------------------------------------------

function TDButton_DefaultCaptionFontFColor(D:TDxControl):TColor; stdcall; //默认标题字体颜色
begin
  Result := TDxImageButton(D).CaptionColor.Up.Color;
end;

function TDButton_DefaultCaptionFontBColor(D:TDxControl):TColor; stdcall; //默认标题字体描边颜色
begin
  Result := TDxImageButton(D).CaptionColor.Up.BColor;
end;

function TDButton_DefaultCaptionFontStyle(D:TDxControl):TFontStyles; stdcall; //默认字体样式
begin
  Result := TDxImageButton(D).CaptionColor.Up.Style;
end;

function TDButton_DefaultCaptionFontSize(D:TDxControl):Integer; stdcall; //默认字体尺寸
begin
  Result := TDxImageButton(D).CaptionColor.Up.Size;
end;

function TDButton_DefaultCaptionFontBold(D:TDxControl):Boolean; stdcall; //默认是否描边
begin
  Result := TDxImageButton(D).CaptionColor.Up.Bold;
end;

function TDButton_DefaultCaptionFontName(D:TDxControl):PChar; stdcall; //默认字体名称
begin
  Result := PChar(TDxImageButton(D).CaptionColor.Up.Name);
end;

function TDButton_MouseMoveCaptionFontFColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体颜色
begin
  Result := TDxImageButton(D).CaptionColor.Hot.Color;
end;

function TDButton_MouseMoveCaptionFontBColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体描边颜色
begin
  Result := TDxImageButton(D).CaptionColor.Hot.Color;
end;

function TDButton_MouseMoveCaptionFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标移动字体样式
begin
  Result := TDxImageButton(D).CaptionColor.Hot.Style;
end;

function TDButton_MouseMoveCaptionFontSize(D:TDxControl):Integer; stdcall; //鼠标移动字体尺寸
begin
  Result := TDxImageButton(D).CaptionColor.Hot.Size;
end;

function TDButton_MouseMoveCaptionFontBold(D:TDxControl):Boolean; stdcall; //鼠标移动是否描边
begin
  Result := TDxImageButton(D).CaptionColor.Hot.Bold;
end;

function TDButton_MouseMoveCaptionFontName(D:TDxControl):PChar; stdcall; //鼠标移动字体名称
begin
  Result := PChar(TDxImageButton(D).CaptionColor.Hot.Name);
end;

function TDButton_MouseDownCaptionFontFColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体颜色
begin
  Result := TDxImageButton(D).CaptionColor.Down.Color;
end;

function TDButton_MouseDownCaptionFontBColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体描边颜色
begin
  Result := TDxImageButton(D).CaptionColor.Down.BColor;
end;

function TDButton_MouseDownCaptionFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标按下字体样式
begin
  Result := TDxImageButton(D).CaptionColor.Down.Style;
end;

function TDButton_MouseDownCaptionFontSize(D:TDxControl):Integer; stdcall; //鼠标按下字体尺寸
begin
  Result := TDxImageButton(D).CaptionColor.Down.Size;
end;

function TDButton_MouseDownCaptionFontBold(D:TDxControl):Boolean; stdcall; //鼠标按下是否描边
begin
  Result := TDxImageButton(D).CaptionColor.Down.Bold;
end;

function TDButton_MouseDownCaptionFontName(D:TDxControl):PChar; stdcall; //鼠标按下字体名称
begin
  Result := PChar(TDxImageButton(D).CaptionColor.Down.Name);
end;

function TDButton_DisabledCaptionFontFColor(D:TDxControl):TColor; stdcall; //不可用时标题字体颜色
begin
  Result := TDxImageButton(D).CaptionColor.Disabled.Color;
end;

function TDButton_DisabledCaptionFontBColor(D:TDxControl):TColor; stdcall; //不可用时标题字体描边颜色
begin
  Result := TDxImageButton(D).CaptionColor.Disabled.BColor;
end;

function TDButton_DisabledCaptionFontStyle(D:TDxControl):TFontStyles; stdcall; //不可用时字体样式
begin
  Result := TDxImageButton(D).CaptionColor.Disabled.Style;
end;

function TDButton_DisabledCaptionFontSize(D:TDxControl):Integer; stdcall; //不可用时字体尺寸
begin
  Result := TDxImageButton(D).CaptionColor.Disabled.Size;
end;

function TDButton_DisabledCaptionFontBold(D:TDxControl):Boolean; stdcall; //不可用时是否描边
begin
  Result := TDxImageButton(D).CaptionColor.Disabled.Bold;
end;

function TDButton_DisabledCaptionFontName(D:TDxControl):PChar; stdcall; //不可用时字体名称
begin
  Result := PChar(TDxImageButton(D).CaptionColor.Disabled.Name);
end;

function TDButton_CheckedCaptionFontFColor(D:TDxControl):TColor; stdcall; //勾选时标题字体颜色
begin
  Result := TDxImageButton(D).CaptionColor.Checked.Color;
end;

function TDButton_CheckedCaptionFontBColor(D:TDxControl):TColor; stdcall; //勾选时标题字体描边颜色
begin
  Result := TDxImageButton(D).CaptionColor.Checked.BColor;
end;

function TDButton_CheckedCaptionFontStyle(D:TDxControl):TFontStyles; stdcall; //勾选时字体样式
begin
  Result := TDxImageButton(D).CaptionColor.Checked.Style;
end;

function TDButton_CheckedCaptionFontSize(D:TDxControl):Integer; stdcall; //勾选时字体尺寸
begin
  Result := TDxImageButton(D).CaptionColor.Checked.Size;
end;

function TDButton_CheckedCaptionFontBold(D:TDxControl):Boolean; stdcall; //勾选时是否描边
begin
  Result := TDxImageButton(D).CaptionColor.Checked.Bold;
end;

function TDButton_CheckedCaptionFontName(D:TDxControl):PChar; stdcall; //勾选时字体名称
begin
  Result := PChar(TDxImageButton(D).CaptionColor.Checked.Name);
end;

procedure TDButton_SetDefaultCaptionFontFColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
begin
  TDxImageButton(D).CaptionColor.Up.Color := Value;
end;

procedure TDButton_SetDefaultCaptionFontBColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
begin
  TDxImageButton(D).CaptionColor.Up.BColor := Value;
end;

procedure TDButton_SetDefaultCaptionFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
begin
  TDxImageButton(D).CaptionColor.Up.Style := Value;
end;

procedure TDButton_SetDefaultCaptionFontSize(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
begin
  TDxImageButton(D).CaptionColor.Up.Size := Value;
end;

procedure TDButton_SetDefaultCaptionFontBold(D:TDxControl; Value:Boolean); stdcall; //默认是否描边
begin
  TDxImageButton(D).CaptionColor.Up.Bold := Value;
end;

procedure TDButton_SetDefaultCaptionFontName(D:TDxControl; Value:PChar); stdcall; //默认字体名称
begin
  TDxImageButton(D).CaptionColor.Up.Name := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
begin
  TDxImageButton(D).CaptionColor.Hot.Color := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
begin
  TDxImageButton(D).CaptionColor.Hot.BColor := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
begin
  TDxImageButton(D).CaptionColor.Hot.Style := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
begin
  TDxImageButton(D).CaptionColor.Hot.Size := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标移动是否描边
begin
  TDxImageButton(D).CaptionColor.Hot.Bold := Value;
end;

procedure TDButton_SetMouseMoveCaptionFontName(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称
begin
  TDxImageButton(D).CaptionColor.Hot.Name := Value;
end;

procedure TDButton_SetMouseDownCaptionFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
begin
  TDxImageButton(D).CaptionColor.Down.Color := Value;
end;

procedure TDButton_SetMouseDownCaptionFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
begin
  TDxImageButton(D).CaptionColor.Down.BColor := Value;
end;

procedure TDButton_SetMouseDownCaptionFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
begin
  TDxImageButton(D).CaptionColor.Down.Style := Value;
end;

procedure TDButton_SetMouseDownCaptionFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
begin
  TDxImageButton(D).CaptionColor.Down.Size := Value;
end;

procedure TDButton_SetMouseDownCaptionFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标按下是否描边
begin
  TDxImageButton(D).CaptionColor.Down.Bold := Value;
end;

procedure TDButton_SetMouseDownCaptionFontName(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称
begin
  TDxImageButton(D).CaptionColor.Down.Name := Value;
end;

procedure TDButton_SetDisabledCaptionFontFColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
begin
  TDxImageButton(D).CaptionColor.Disabled.Color := Value;
end;

procedure TDButton_SetDisabledCaptionFontBColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
begin
  TDxImageButton(D).CaptionColor.Disabled.BColor := Value;
end;

procedure TDButton_SetDisabledCaptionFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
begin
  TDxImageButton(D).CaptionColor.Disabled.Style := Value;
end;

procedure TDButton_SetDisabledCaptionFontSize(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
begin
  TDxImageButton(D).CaptionColor.Disabled.Size := Value;
end;

procedure TDButton_SetDisabledCaptionFontBold(D:TDxControl; Value:Boolean); stdcall; //不可用时是否描边
begin
  TDxImageButton(D).CaptionColor.Disabled.Bold := Value;
end;

procedure TDButton_SetDisabledCaptionFontName(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称
begin
  TDxImageButton(D).CaptionColor.Disabled.Name := Value;
end;

procedure TDButton_SetCheckedCaptionFontFColor(D:TDxControl; Value:TColor); stdcall; //勾选时标题字体颜色
begin
  TDxImageButton(D).CaptionColor.Checked.Color := Value;
end;

procedure TDButton_SetCheckedCaptionFontBColor(D:TDxControl; Value:TColor); stdcall; //勾选时标题字体描边颜色
begin
  TDxImageButton(D).CaptionColor.Checked.BColor := Value;
end;

procedure TDButton_SetCheckedCaptionFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //勾选时字体样式
begin
  TDxImageButton(D).CaptionColor.Checked.Style := Value;
end;

procedure TDButton_SetCheckedCaptionFontSize(D:TDxControl; Value:Integer); stdcall; //勾选时字体尺寸
begin
  TDxImageButton(D).CaptionColor.Checked.Size := Value;
end;

procedure TDButton_SetCheckedCaptionFontBold(D:TDxControl; Value:Boolean); stdcall; //勾选时是否描边
begin
  TDxImageButton(D).CaptionColor.Checked.Bold := Value;
end;

procedure TDButton_SetCheckedCaptionFontName(D:TDxControl; Value:PChar); stdcall; //勾选时字体名称
begin
  TDxImageButton(D).CaptionColor.Checked.Name := Value;
end;

function TDButton_Style(D:TDxControl):TButtonStyle; stdcall; //按钮样式
begin
  Result := TDxImageButton(D).Style;
end;

function TDButton_Checked(D:TDxControl):Boolean; stdcall;
begin
  Result := TDxImageButton(D).Checked;
end;

function TDButton_CaptionDownOffsetX(D:TDxControl):Integer; stdcall; //按下后标题偏移X
begin
  Result := TDxImageButton(D).CaptionDownOffsetX;
end;

function TDButton_CaptionDownOffsetY(D:TDxControl):Integer; stdcall; //按下后标题偏移Y
begin
  Result := TDxImageButton(D).CaptionDownOffsetY;
end;

function TDButton_ButtonDownOffsetX(D:TDxControl):Integer; stdcall; //按下后偏移X
begin
  Result := TDxImageButton(D).ButtonDownOffsetX;
end;

function TDButton_ButtonDownOffsetY(D:TDxControl):Integer; stdcall; //按下后偏移Y
begin
  Result := TDxImageButton(D).ButtonDownOffsetY;
end;

function TDButton_ClickCount(D:TDxControl):TClickSound; stdcall; //按钮点击的声音
begin
  Result := TDxImageButton(D).ClickCount;
end;

procedure TDButton_SetStyle(D:TDxControl; Value:TButtonStyle); stdcall; //按钮样式
begin
  TDxImageButton(D).Style := Value;
end;

procedure TDButton_SetChecked(D:TDxControl; Value:Boolean); stdcall;
begin
  TDxImageButton(D).Checked := Value;
end;

procedure TDButton_SetCaptionDownOffsetX(D:TDxControl; Value:Integer); stdcall; //按下后标题偏移X
begin
  TDxImageButton(D).CaptionDownOffsetX := Value;
end;

procedure TDButton_SetCaptionDownOffsetY(D:TDxControl; Value:Integer); stdcall; //按下后标题偏移Y
begin
  TDxImageButton(D).CaptionDownOffsetY := Value;
end;

procedure TDButton_SetButtonDownOffsetX(D:TDxControl; Value:Integer); stdcall; //按下后偏移X
begin
  TDxImageButton(D).ButtonDownOffsetX := Value;
end;

procedure TDButton_SetButtonDownOffsetY(D:TDxControl; Value:Integer); stdcall; //按下后偏移Y
begin
  TDxImageButton(D).ButtonDownOffsetY := Value;
end;

procedure TDButton_SetClickCount(D:TDxControl; Value:TClickSound); stdcall; //按钮点击的声音
begin
  TDxImageButton(D).ClickCount := Value;
end;
//------------------------------------------------------------------------------

function TDWindow_IsBringToFront(D:TDxControl):Boolean; stdcall; //单击窗体是否显示到最上层
begin
  Result := TDxImageForm(D).IsBringToFront;
end;
//-----------------------------------------------------------------------------

procedure TDWindow_SetIsBringToFront(D:TDxControl; Value:Boolean); stdcall; //单击窗体是否显示到最上层
begin
  TDxImageForm(D).IsBringToFront := Value;
end;
//-----------------------------------------------------------------------------

function TDWindow_ShowModalEx(D:TDxControl):Integer; stdcall;
begin
  Result := TDxImageForm(D).ShowModalEx;
end;

function TDWindow_ShowModalA(D:TDxControl):Integer; stdcall;
begin
  Result := TDxImageForm(D).ShowModal;
end;

function TDWindow_ShowModalB(Value:TMethod):Integer; stdcall;
var
  NotifyEvent:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    NotifyEvent := TNotifyEvent(Value);
    Result := TDxImageForm(D).ShowModal(NotifyEvent);
  end else begin
    Result := mrNone; //HZQ 20230525
  end;
end;
//------------------------------------------------------------------------------

function TDEdit_Text(D:TDxControl):PChar; stdcall;
begin
  Result := PChar(TDxEdit(D).Text);
end;

function TDEdit_Value(D:TDxControl):Integer; stdcall;
begin
  Result := TDxEdit(D).Value;
end;

function TDEdit_ReadOnly(D:TDxControl):Boolean; stdcall; //是否只读
begin
  Result := TDxEdit(D).ReadOnly;
end;

function TDEdit_MaxLength(D:TDxControl):Integer; stdcall; //最大长度
begin
  Result := TDxEdit(D).MaxLength;
end;

function TDEdit_SelectedColor(D:TDxControl):TColor; stdcall; //光标颜色
begin
  Result := TDxEdit(D).SelectedColor;
end;

function TDEdit_SelBackColor(D:TDxControl):TColor; stdcall; //选择字体背景色
begin
  Result := TDxEdit(D).SelBackColor;
end;

function TDEdit_SelFontColor(D:TDxControl):TColor; stdcall; //选择字体色
begin
  Result := TDxEdit(D).SelFontColor;
end;

function TDEdit_PasswordChar(D:TDxControl):Char; stdcall;
begin
  Result := TDxEdit(D).PasswordChar;
end;

function TDEdit_AllowSelect(D:TDxControl):Boolean; stdcall; //是否允许选择
begin
  Result := TDxEdit(D).AllowSelect;
end;

function TDEdit_AllowPaste(D:TDxControl):Boolean; stdcall; //是否允许粘贴
begin
  Result := TDxEdit(D).AllowPaste;
end;

function TDEdit_InValue(D:TDxControl):TInValue; stdcall; //允许输入控制
begin
  Result := TDxEdit(D).InValue;
end;

function TDEdit_TabOrder(D:TDxControl):Integer; stdcall;
begin
  Result := TDxEdit(D).TabOrder;
end;

function TDEdit_OnChange(D:TDxControl):Pointer; stdcall; //输入框改变触发
begin
  Result := TMethod(TDxEdit(D).OnChange).Code;
end;
//-----------------------------------------------------------------------------

procedure TDEdit_SetText(D:TDxControl; Value:PChar); stdcall;
begin
  TDxEdit(D).Text := Value;
end;

procedure TDEdit_SetValue(D:TDxControl; Value:Integer); stdcall;
begin
  TDxEdit(D).Value := Value;
end;

procedure TDEdit_SetReadOnly(D:TDxControl; Value:Boolean); stdcall; //是否只读
begin
  TDxEdit(D).ReadOnly := Value;
end;

procedure TDEdit_SetMaxLength(D:TDxControl; Value:Integer); stdcall; //最大长度
begin
  TDxEdit(D).MaxLength := Value;
end;

procedure TDEdit_SetSelectedColor(D:TDxControl; Value:TColor); stdcall; //光标颜色
begin
  TDxEdit(D).SelectedColor := Value;
end;

procedure TDEdit_SetSelBackColor(D:TDxControl; Value:TColor); stdcall; //选择字体背景色
begin
  TDxEdit(D).SelBackColor := Value;
end;

procedure TDEdit_SetSelFontColor(D:TDxControl; Value:TColor); stdcall; //选择字体色
begin
  TDxEdit(D).SelFontColor := Value;
end;

procedure TDEdit_SetPasswordChar(D:TDxControl; Value:Char); stdcall;
begin
  TDxEdit(D).PasswordChar := Value;
end;

procedure TDEdit_SetAllowSelect(D:TDxControl; Value:Boolean); stdcall; //是否允许选择
begin
  TDxEdit(D).AllowSelect := Value;
end;

procedure TDEdit_SetAllowPaste(D:TDxControl; Value:Boolean); stdcall; //是否允许粘贴
begin
  TDxEdit(D).AllowPaste := Value;
end;

procedure TDEdit_SetInValue(D:TDxControl; Value:TInValue); stdcall; //允许输入控制
begin
  TDxEdit(D).InValue := Value;
end;

procedure TDEdit_SetTabOrder(D:TDxControl; Value:Integer); stdcall;
begin
  TDxEdit(D).TabOrder := Value;
end;

procedure TDEdit_SetOnChange(Value:TMethod); stdcall;
var
  OnChange:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnChange := TNotifyEvent(Value);
    if Value.Code = nil then
      TDxEdit(D).OnChange := nil
    else
      TDxEdit(D).OnChange := OnChange;
  end;
end;
//-----------------------------------------------------------------------------

function TDGrid_ColCount(D:TDxControl):Integer; stdcall; //列数
begin
  Result := TDxImageGrid(D).ColCount;
end;

function TDGrid_RowCount(D:TDxControl):Integer; stdcall; //组数
begin
  Result := TDxImageGrid(D).RowCount;
end;

function TDGrid_ColWidth(D:TDxControl):Integer; stdcall; //列宽
begin
  Result := TDxImageGrid(D).ColWidth;
end;

function TDGrid_RowHeight(D:TDxControl):Integer; stdcall; //组高
begin
  Result := TDxImageGrid(D).RowHeight;
end;

function TDGrid_OnGridSelect(D:TDxControl):Pointer; stdcall; //选择事件
begin
  Result := TMethod(TDxImageGrid(D).OnGridSelect).Code;
end;

function TDGrid_OnGridMouseMove(D:TDxControl):Pointer; stdcall; //鼠标移动事件
begin
  Result := TMethod(TDxImageGrid(D).OnGridMouseMove).Code;
end;

function TDGrid_OnGridPaint(D:TDxControl):Pointer; stdcall; //绘制事件
begin
  Result := TMethod(TDxImageGrid(D).OnGridPaint).Code;
end;
//-----------------------------------------------------------------------------

procedure TDGrid_SetColCount(D:TDxControl; Value:Integer); stdcall; //列数
begin
  TDxImageGrid(D).ColCount := Value;
end;

procedure TDGrid_SetRowCount(D:TDxControl; Value:Integer); stdcall; //组数
begin
  TDxImageGrid(D).RowCount := Value;
end;

procedure TDGrid_SetColWidth(D:TDxControl; Value:Integer); stdcall; //列宽
begin
  TDxImageGrid(D).ColWidth := Value;
end;

procedure TDGrid_SetRowHeight(D:TDxControl; Value:Integer); stdcall; //组高
begin
  TDxImageGrid(D).RowHeight := Value;
end;

procedure TDGrid_SetOnGridSelect(Value:TMethod); stdcall; //选择事件
var
  OnGridSelect:TOnGridSelect;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnGridSelect := TOnGridSelect(Value);
    if Value.Code = nil then
      TDxImageGrid(D).OnGridSelect := nil
    else
      TDxImageGrid(D).OnGridSelect := OnGridSelect;
  end;
end;

procedure TDGrid_SetOnGridMouseMove(Value:TMethod); stdcall; //鼠标移动事件
var
  OnGridMouseMove:TOnGridMove;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnGridMouseMove := TOnGridMove(Value);
    if Value.Code = nil then
      TDxImageGrid(D).OnGridMouseMove := nil
    else
      TDxImageGrid(D).OnGridMouseMove := OnGridMouseMove;
  end;
end;

procedure TDGrid_SetOnGridPaint(Value:TMethod); stdcall; //绘制事件
var
  OnGridPaint:TOnGridPaint;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnGridPaint := TOnGridPaint(Value);
    if Value.Code = nil then
      TDxImageGrid(D).OnGridPaint := nil
    else
      TDxImageGrid(D).OnGridPaint := OnGridPaint;
  end;
end;

procedure TDGrid_DrawGridItem(ARect:TRect; Item:pTClientItem; Effect:Pointer); stdcall; //绘制事件
begin
  FrmDlg.DrawGridItem(ARect, Item, Effect);
end;
//-----------------------------------------------------------------------------

function TDComboBox_ShowButton(D:TDxControl):Boolean; stdcall; //是否显示按钮
begin
  Result := TDxComboBox(D).ShowButton;
end;

function TDComboBox_ItemIndex(D:TDxControl):Integer; stdcall; //当前选择行
begin
  Result := TDxComboBox(D).ItemIndex;
end;

function TDComboBox_ButtonColor(D:TDxControl):TColor; stdcall; //按钮颜色
begin
  Result := TDxComboBox(D).ButtonColor;
end;

function TDComboBox_Items(D:TDxControl):TStringList; stdcall; //列表
begin
  Result := TStringList(TDxComboBox(D).Items);
end;

function TDComboBox_Text(D:TDxControl):PChar; stdcall;
begin
  Result := PChar(TDxComboBox(D).Text);
end;

function TDComboBox_OnSelect(D:TDxControl):Pointer; stdcall;
begin
  Result := TMethod(TDxComboBox(D).OnSelect).Code;
end;

function TDComboBox_DefaultTextFontFColor(D:TDxControl):TColor; stdcall; //默认标题字体颜色
begin
  Result := TDxComboBox(D).TextColor.Up.Color;
end;

function TDComboBox_DefaultTextFontBColor(D:TDxControl):TColor; stdcall; //默认标题字体描边颜色
begin
  Result := TDxComboBox(D).TextColor.Up.BColor;
end;

function TDComboBox_DefaultTextFontStyle(D:TDxControl):TFontStyles; stdcall; //默认字体样式
begin
  Result := TDxComboBox(D).TextColor.Up.Style;
end;

function TDComboBox_DefaultTextFontSize(D:TDxControl):Integer; stdcall; //默认字体尺寸
begin
  Result := TDxComboBox(D).TextColor.Up.Size;
end;

function TDComboBox_DefaultTextFontBold(D:TDxControl):Boolean; stdcall; //默认是否描边
begin
  Result := TDxComboBox(D).TextColor.Up.Bold;
end;

function TDComboBox_DefaultTextFontName(D:TDxControl):PChar; stdcall; //默认字体名称
begin
  Result := PChar(TDxComboBox(D).TextColor.Up.Name);
end;

function TDComboBox_MouseMoveTextFontFColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体颜色
begin
  Result := TDxComboBox(D).TextColor.Hot.Color;
end;

function TDComboBox_MouseMoveTextFontBColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体描边颜色
begin
  Result := TDxComboBox(D).TextColor.Hot.BColor;
end;

function TDComboBox_MouseMoveTextFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标移动字体样式
begin
  Result := TDxComboBox(D).TextColor.Hot.Style;
end;

function TDComboBox_MouseMoveTextFontSize(D:TDxControl):Integer; stdcall; //鼠标移动字体尺寸
begin
  Result := TDxComboBox(D).TextColor.Hot.Size;
end;

function TDComboBox_MouseMoveTextFontBold(D:TDxControl):Boolean; stdcall; //鼠标移动是否描边
begin
  Result := TDxComboBox(D).TextColor.Hot.Bold;
end;

function TDComboBox_MouseMoveTextFontName(D:TDxControl):PChar; stdcall; //鼠标移动字体名称
begin
  Result := PChar(TDxComboBox(D).TextColor.Hot.Name);
end;

function TDComboBox_MouseDownTextFontFColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体颜色
begin
  Result := TDxComboBox(D).TextColor.Down.Color;
end;

function TDComboBox_MouseDownTextFontBColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体描边颜色
begin
  Result := TDxComboBox(D).TextColor.Down.BColor;
end;

function TDComboBox_MouseDownTextFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标按下字体样式
begin
  Result := TDxComboBox(D).TextColor.Down.Style;
end;

function TDComboBox_MouseDownTextFontSize(D:TDxControl):Integer; stdcall; //鼠标按下字体尺寸
begin
  Result := TDxComboBox(D).TextColor.Down.Size;
end;

function TDComboBox_MouseDownTextFontBold(D:TDxControl):Boolean; stdcall; //鼠标按下是否描边
begin
  Result := TDxComboBox(D).TextColor.Down.Bold;
end;

function TDComboBox_MouseDownTextFontName(D:TDxControl):PChar; stdcall; //鼠标按下字体名称
begin
  Result := PChar(TDxComboBox(D).TextColor.Down.Name);
end;

function TDComboBox_DisabledTextFontFColor(D:TDxControl):TColor; stdcall; //不可用时标题字体颜色
begin
  Result := TDxComboBox(D).TextColor.Disabled.Color;
end;

function TDComboBox_DisabledTextFontBColor(D:TDxControl):TColor; stdcall; //不可用时标题字体描边颜色
begin
  Result := TDxComboBox(D).TextColor.Disabled.BColor;
end;

function TDComboBox_DisabledTextFontStyle(D:TDxControl):TFontStyles; stdcall; //不可用时字体样式
begin
  Result := TDxComboBox(D).TextColor.Disabled.Style;
end;

function TDComboBox_DisabledTextFontSize(D:TDxControl):Integer; stdcall; //不可用时字体尺寸
begin
  Result := TDxComboBox(D).TextColor.Disabled.Size;
end;

function TDComboBox_DisabledTextFontBold(D:TDxControl):Boolean; stdcall; //不可用时是否描边
begin
  Result := TDxComboBox(D).TextColor.Disabled.Bold;
end;

function TDComboBox_DisabledTextFontName(D:TDxControl):PChar; stdcall; //不可用时字体名称
begin
  Result := PChar(TDxComboBox(D).TextColor.Disabled.Name);
end;

//-----------------------------------------------------------------------------

procedure TDComboBox_SetShowButton(D:TDxControl; Value:Boolean); stdcall; //是否显示按钮
begin
  TDxComboBox(D).ShowButton := Value;
end;

procedure TDComboBox_SetItemIndex(D:TDxControl; Value:Integer); stdcall; //当前选择行
begin
  TDxComboBox(D).ItemIndex := Value;
end;

procedure TDComboBox_SetButtonColor(D:TDxControl; Value:TColor); stdcall; //按钮颜色
begin
  TDxComboBox(D).ButtonColor := Value;
end;

procedure TDComboBox_SetItems(D:TDxControl; Value:TStringList); stdcall; //列表
begin
  TDxComboBox(D).Items := Value;
end;

procedure TDComboBox_SetText(D:TDxControl; Value:PChar); stdcall;
begin
  TDxComboBox(D).Text := Value;
end;

procedure TDComboBox_SetOnSelect(Value:TMethod); stdcall;
var
  OnSelect:TNotifyEvent;
  D:TDxControl;
begin
  D := TDxControl(Value.Data);
  if D <> nil then begin
    OnSelect := TNotifyEvent(Value);
    if Value.Code = nil then
      TDxComboBox(D).OnSelect := nil
    else
      TDxComboBox(D).OnSelect := OnSelect;
  end;
end;

procedure TDComboBox_SetDefaultTextFontFColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
begin
  TDxComboBox(D).TextColor.Up.Color := Value;
end;

procedure TDComboBox_SetDefaultTextFontBColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
begin
  TDxComboBox(D).TextColor.Up.BColor := Value;
end;

procedure TDComboBox_SetDefaultTextFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
begin
  TDxComboBox(D).TextColor.Up.Style := Value;
end;

procedure TDComboBox_SetDefaultTextFontSize(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
begin
  TDxComboBox(D).TextColor.Up.Size := Value;
end;

procedure TDComboBox_SetDefaultTextFontBold(D:TDxControl; Value:Boolean); stdcall; //默认是否描边
begin
  TDxComboBox(D).TextColor.Up.Bold := Value;
end;

procedure TDComboBox_SetDefaultTextFontName(D:TDxControl; Value:PChar); stdcall; //默认字体名称
begin
  TDxComboBox(D).TextColor.Up.Name := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
begin
  TDxComboBox(D).TextColor.Hot.Color := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
begin
  TDxComboBox(D).TextColor.Hot.BColor := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
begin
  TDxComboBox(D).TextColor.Hot.Style := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
begin
  TDxComboBox(D).TextColor.Hot.Size := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标移动是否描边
begin
  TDxComboBox(D).TextColor.Hot.Bold := Value;
end;

procedure TDComboBox_SetMouseMoveTextFontName(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称
begin
  TDxComboBox(D).TextColor.Hot.Name := Value;
end;

procedure TDComboBox_SetMouseDownTextFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
begin
  TDxComboBox(D).TextColor.Down.Color := Value;
end;

procedure TDComboBox_SetMouseDownTextFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
begin
  TDxComboBox(D).TextColor.Down.BColor := Value;
end;

procedure TDComboBox_SetMouseDownTextFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
begin
  TDxComboBox(D).TextColor.Down.Style := Value;
end;

procedure TDComboBox_SetMouseDownTextFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
begin
  TDxComboBox(D).TextColor.Down.Size := Value;
end;

procedure TDComboBox_SetMouseDownTextFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标按下是否描边
begin
  TDxComboBox(D).TextColor.Down.Bold := Value;
end;

procedure TDComboBox_SetMouseDownTextFontName(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称
begin
  TDxComboBox(D).TextColor.Down.Name := Value;
end;

procedure TDComboBox_SetDisabledTextFontFColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
begin
  TDxComboBox(D).TextColor.Disabled.Color := Value;
end;

procedure TDComboBox_SetDisabledTextFontBColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
begin
  TDxComboBox(D).TextColor.Disabled.BColor := Value;
end;

procedure TDComboBox_SetDisabledTextFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
begin
  TDxComboBox(D).TextColor.Disabled.Style := Value;
end;

procedure TDComboBox_SetDisabledTextFontSize(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
begin
  TDxComboBox(D).TextColor.Disabled.Size := Value;
end;

procedure TDComboBox_SetDisabledTextFontBold(D:TDxControl; Value:Boolean); stdcall; //不可用时是否描边
begin
  TDxComboBox(D).TextColor.Disabled.Bold := Value;
end;

procedure TDComboBox_SetDisabledTextFontName(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称
begin
  TDxComboBox(D).TextColor.Disabled.Name := Value;
end;
//------------------------------------------------------------------------------

function TDPopupMenu_SelectColor(D:TDxControl):TColor; stdcall; //选择颜色
begin
  Result := TDxPopupMenu(D).SelectColor;
end;

function TDPopupMenu_ItemIndex(D:TDxControl):Integer; stdcall; //选择行
begin
  Result := TDxPopupMenu(D).ItemIndex;
end;

function TDPopupMenu_ItemHeight(D:TDxControl):Integer; stdcall; //组高
begin
  Result := TDxPopupMenu(D).ItemHeight;
end;

function TDPopupMenu_Items(D:TDxControl):TDxItemMenuList; stdcall;
begin
  Result := TDxPopupMenu(D).Items;
end;

function TDPopupMenu_Alpha(D:TDxControl):Byte; stdcall; //背景透明度
begin
  Result := TDxPopupMenu(D).Alpha;
end;

function TDPopupMenu_DefaultItemFontFColor(D:TDxControl):TColor; stdcall; //默认标题字体颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Up.Color;
end;

function TDPopupMenu_DefaultItemFontBColor(D:TDxControl):TColor; stdcall; //默认标题字体描边颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Up.BColor;
end;

function TDPopupMenu_DefaultItemFontStyle(D:TDxControl):TFontStyles; stdcall; //默认字体样式
begin
  Result := TDxPopupMenu(D).ItemColor.Up.Style;
end;

function TDPopupMenu_DefaultItemFontSize(D:TDxControl):Integer; stdcall; //默认字体尺寸
begin
  Result := TDxPopupMenu(D).ItemColor.Up.Size;
end;

function TDPopupMenu_DefaultItemFontBold(D:TDxControl):Boolean; stdcall; //默认是否描边
begin
  Result := TDxPopupMenu(D).ItemColor.Up.Bold;
end;

function TDPopupMenu_DefaultItemFontName(D:TDxControl):PChar; stdcall; //默认字体名称
begin
  Result := PChar(TDxPopupMenu(D).ItemColor.Up.Name);
end;

function TDPopupMenu_MouseMoveItemFontFColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Hot.Color;
end;

function TDPopupMenu_MouseMoveItemFontBColor(D:TDxControl):TColor; stdcall; //鼠标移动标题字体描边颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Hot.BColor;
end;

function TDPopupMenu_MouseMoveItemFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标移动字体样式
begin
  Result := TDxPopupMenu(D).ItemColor.Hot.Style;
end;

function TDPopupMenu_MouseMoveItemFontSize(D:TDxControl):Integer; stdcall; //鼠标移动字体尺寸
begin
  Result := TDxPopupMenu(D).ItemColor.Hot.Size;
end;

function TDPopupMenu_MouseMoveItemFontBold(D:TDxControl):Boolean; stdcall; //鼠标移动是否描边
begin
  Result := TDxPopupMenu(D).ItemColor.Hot.Bold;
end;

function TDPopupMenu_MouseMoveItemFontName(D:TDxControl):PChar; stdcall; //鼠标移动字体名称
begin
  Result := PChar(TDxPopupMenu(D).ItemColor.Hot.Name);
end;

function TDPopupMenu_MouseDownItemFontFColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Down.Color;
end;

function TDPopupMenu_MouseDownItemFontBColor(D:TDxControl):TColor; stdcall; //鼠标按下标题字体描边颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Down.BColor;
end;

function TDPopupMenu_MouseDownItemFontStyle(D:TDxControl):TFontStyles; stdcall; //鼠标按下字体样式
begin
  Result := TDxPopupMenu(D).ItemColor.Down.Style;
end;

function TDPopupMenu_MouseDownItemFontSize(D:TDxControl):Integer; stdcall; //鼠标按下字体尺寸
begin
  Result := TDxPopupMenu(D).ItemColor.Down.Size;
end;

function TDPopupMenu_MouseDownItemFontBold(D:TDxControl):Boolean; stdcall; //鼠标按下是否描边
begin
  Result := TDxPopupMenu(D).ItemColor.Down.Bold;
end;

function TDPopupMenu_MouseDownItemFontName(D:TDxControl):PChar; stdcall; //鼠标按下字体名称
begin
  Result := PChar(TDxPopupMenu(D).ItemColor.Down.Name);
end;

function TDPopupMenu_DisabledItemFontFColor(D:TDxControl):TColor; stdcall; //不可用时标题字体颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Disabled.Color;
end;

function TDPopupMenu_DisabledItemFontBColor(D:TDxControl):TColor; stdcall; //不可用时标题字体描边颜色
begin
  Result := TDxPopupMenu(D).ItemColor.Disabled.BColor;
end;

function TDPopupMenu_DisabledItemFontStyle(D:TDxControl):TFontStyles; stdcall; //不可用时字体样式
begin
  Result := TDxPopupMenu(D).ItemColor.Disabled.Style;
end;

function TDPopupMenu_DisabledItemFontSize(D:TDxControl):Integer; stdcall; //不可用时字体尺寸
begin
  Result := TDxPopupMenu(D).ItemColor.Disabled.Size;
end;

function TDPopupMenu_DisabledItemFontBold(D:TDxControl):Boolean; stdcall; //不可用时是否描边
begin
  Result := TDxPopupMenu(D).ItemColor.Disabled.Bold;
end;

function TDPopupMenu_DisabledItemFontName(D:TDxControl):PChar; stdcall; //不可用时字体名称
begin
  Result := PChar(TDxPopupMenu(D).ItemColor.Disabled.Name);
end;

//-----------------------------------------------------------------------------

procedure TDPopupMenu_SetSelectColor(D:TDxControl; Value:TColor); stdcall; //选择颜色
begin
  TDxPopupMenu(D).SelectColor := Value;
end;

procedure TDPopupMenu_SetItemIndex(D:TDxControl; Value:Integer); stdcall; //选择行
begin
  TDxPopupMenu(D).ItemIndex := Value;
end;

procedure TDPopupMenu_SetItemHeight(D:TDxControl; Value:Integer); stdcall; //组高
begin
  TDxPopupMenu(D).ItemHeight := Value;
end;

procedure TDPopupMenu_SetAlpha(D:TDxControl; Value:Byte); stdcall; //背景透明度
begin
  TDxPopupMenu(D).Alpha := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontFColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体颜色
begin
  TDxPopupMenu(D).ItemColor.Up.Color := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontBColor(D:TDxControl; Value:TColor); stdcall; //默认标题字体描边颜色
begin
  TDxPopupMenu(D).ItemColor.Up.BColor := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //默认字体样式
begin
  TDxPopupMenu(D).ItemColor.Up.Style := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontSize(D:TDxControl; Value:Integer); stdcall; //默认字体尺寸
begin
  TDxPopupMenu(D).ItemColor.Up.Size := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontBold(D:TDxControl; Value:Boolean); stdcall; //默认是否描边
begin
  TDxPopupMenu(D).ItemColor.Up.Bold := Value;
end;

procedure TDPopupMenu_SetDefaultItemFontName(D:TDxControl; Value:PChar); stdcall; //默认字体名称
begin
  TDxPopupMenu(D).ItemColor.Up.Name := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体颜色
begin
  TDxPopupMenu(D).ItemColor.Hot.Color := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标移动标题字体描边颜色
begin
  TDxPopupMenu(D).ItemColor.Hot.BColor := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标移动字体样式
begin
  TDxPopupMenu(D).ItemColor.Hot.Style := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标移动字体尺寸
begin
  TDxPopupMenu(D).ItemColor.Hot.Size := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标移动是否描边
begin
  TDxPopupMenu(D).ItemColor.Hot.Bold := Value;
end;

procedure TDPopupMenu_SetMouseMoveItemFontName(D:TDxControl; Value:PChar); stdcall; //鼠标移动字体名称
begin
  TDxPopupMenu(D).ItemColor.Hot.Name := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontFColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体颜色
begin
  TDxPopupMenu(D).ItemColor.Down.Color := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontBColor(D:TDxControl; Value:TColor); stdcall; //鼠标按下标题字体描边颜色
begin
  TDxPopupMenu(D).ItemColor.Down.BColor := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //鼠标按下字体样式
begin
  TDxPopupMenu(D).ItemColor.Down.Style := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontSize(D:TDxControl; Value:Integer); stdcall; //鼠标按下字体尺寸
begin
  TDxPopupMenu(D).ItemColor.Down.Size := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontBold(D:TDxControl; Value:Boolean); stdcall; //鼠标按下是否描边
begin
  TDxPopupMenu(D).ItemColor.Down.Bold := Value;
end;

procedure TDPopupMenu_SetMouseDownItemFontName(D:TDxControl; Value:PChar); stdcall; //鼠标按下字体名称
begin
  TDxPopupMenu(D).ItemColor.Down.Name := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontFColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体颜色
begin
  TDxPopupMenu(D).ItemColor.Disabled.Color := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontBColor(D:TDxControl; Value:TColor); stdcall; //不可用时标题字体描边颜色
begin
  TDxPopupMenu(D).ItemColor.Disabled.BColor := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontStyle(D:TDxControl; Value:TFontStyles); stdcall; //不可用时字体样式
begin
  TDxPopupMenu(D).ItemColor.Disabled.Style := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontSize(D:TDxControl; Value:Integer); stdcall; //不可用时字体尺寸
begin
  TDxPopupMenu(D).ItemColor.Disabled.Size := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontBold(D:TDxControl; Value:Boolean); stdcall; //不可用时是否描边
begin
  TDxPopupMenu(D).ItemColor.Disabled.Bold := Value;
end;

procedure TDPopupMenu_SetDisabledItemFontName(D:TDxControl; Value:PChar); stdcall; //不可用时字体名称
begin
  TDxPopupMenu(D).ItemColor.Disabled.Name := Value;
end;
//------------------------------------------------------------------------------

function TItemMenuListAPI_Count(List:TDxItemMenuList):Integer; stdcall;
begin
  Result := List.Count;
end;

procedure TItemMenuListAPI_Add(List:TDxItemMenuList; S:PChar); stdcall;
begin
  List.Add(S);
end;

procedure TItemMenuListAPI_AddObject(List:TDxItemMenuList; S:PChar; AObject:TObject); stdcall;
begin
  List.AddObject(S, AObject);
end;

function TItemMenuListAPI_Get(List:TDxItemMenuList; Index:Integer):PChar; stdcall;
begin
  Result := PChar(List.Strings[Index]);
end;

function TItemMenuListAPI_GetObject(List:TDxItemMenuList; Index:Integer):TObject; stdcall;
begin
  Result := pTDxItemMenu(List.Objects[Index]).AObject;
end;

procedure TItemMenuListAPI_Delete(List:TDxItemMenuList; Index:Integer); stdcall;
begin
  List.Delete(Index);
end;

procedure TItemMenuListAPI_Clear(List:TDxItemMenuList); stdcall;
begin
  List.Clear;
end;

function TActorAPI_m_wAppearance(Actor:TActor):PWord; stdcall;
begin
  Result := @Actor.m_wAppearance;
end;

function TActorAPI_m_nRecogId(Actor:TActor):PInt64; stdcall; //角色标识 2021-01-11 64位支持
begin
  Result := @Actor.m_nRecogId;
end;

function TActorAPI_m_nCurrX(Actor:TActor):PInteger; stdcall; //当前所在地图座标X
begin
  Result := nil; //0 HZQ //@Actor.m_nCurrX;
end;

function TActorAPI_m_nCurrY(Actor:TActor):PInteger; stdcall; //当前所在地图座标Y
begin
  Result := nil; //0; HZQ //@Actor.m_nCurrY;
end;

function TActorAPI_m_btDir(Actor:TActor):PByte; stdcall; //当前站立方向
begin
  Result := @Actor.m_btDir;
end;

function TActorAPI_m_btSex(Actor:TActor):PByte; stdcall; //性别
begin
  Result := @Actor.m_btSex;
end;

function TActorAPI_m_btRace(Actor:TActor):PByte; stdcall; //怪物DB库的RaceImg
begin
  Result := @Actor.m_btRace;
end;

function TActorAPI_m_btHair(Actor:TActor):PByte; stdcall; //头发类型
begin
  Result := @Actor.m_btHair;
end;

function TActorAPI_m_wDress(Actor:TActor):PWord; stdcall; //衣服类型
begin
  Result := @Actor.m_wDress;
end;

function TActorAPI_m_wWeapon(Actor:TActor):PWord; stdcall; //武器类型
begin
  Result := @Actor.m_wWeapon;
end;

function TActorAPI_m_btJob(Actor:TActor):PByte; stdcall; //职业 0:武士  1:法师  2:道士
begin
  Result := @Actor.m_btJob;
end;

function TActorAPI_m_btCaseltGuild(Actor:TActor):PByte; stdcall; //1=沙行会成员 //2=沙行会掌门
begin
  Result := @Actor.m_btCaseltGuild;
end;

function TActorAPI_m_sDescUserName(Actor:TActor):PChar; stdcall; //人物封号
begin
  Result := PChar(Actor.m_sDescUserName);
end;

function TActorAPI_m_sUserName(Actor:TActor):PChar; stdcall; //名称
begin
  Result := PChar(Actor.m_sUserName);
end;

function TActorAPI_m_nNameColor(Actor:TActor):PInteger; stdcall; //名称颜色
begin
  Result := @Actor.m_nNameColor;
end;

function TActorAPI_m_Abil(Actor:TActor):pTAbility; stdcall; //属性
begin
  Result := @Actor.m_Abil;
end;

function TActorAPI_m_boOpenShop(Actor:TActor):Boolean; stdcall; //是否在摆摊
begin
  Result := Actor.m_boShopStall;
end;

function TActorAPI_m_nSayX(Actor:TActor):Integer; stdcall;
begin
  Result := Actor.m_nSayX;
end;

function TActorAPI_m_nSayY(Actor:TActor):Integer; stdcall;
begin
  Result := Actor.m_nSayY;
end;

function TActorAPI_m_nShiftX(Actor:TActor):Integer; stdcall;
begin
  Result := Actor.m_nShiftX;
end;

function TActorAPI_m_nShiftY(Actor:TActor):Integer; stdcall;
begin
  Result := Actor.m_nShiftY;
end;

function TActorAPI_m_nTargetX(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nTargetX;
end;

function TActorAPI_m_nTargetY(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nTargetY;
end;

function TActorAPI_m_nTargetRecog(Actor:TActor):PInt64; stdcall; // 2021-01-11 64位支持
begin
  Result := @Actor.m_nTargetRecog;
end;

function TActorAPI_m_boCobweb(Actor:TActor):PBoolean; stdcall; //网罩住了
begin
  Result := @Actor.m_boCobweb;
end;

function TActorAPI_m_boCanDraw(Actor:TActor):Boolean; stdcall; //该角色是否可以绘制
begin
  Result := Actor.m_boCanDraw;
end;

function TActorAPI_m_nBagCount(Actor:TActor):Integer; stdcall; //包裹最大数
begin
  if Actor = g_MyHero then
    Result := THeroActor(Actor).m_nBagCount
  else
    Result := ALL_BAG_ITEM_COUNT;
end;

function TActorAPI_m_btColor(Actor:TActor):Byte; stdcall; //脚本命令修改的身体颜色
begin
  Result := Actor.m_btBodyColor;
end;

function TActorAPI_m_nState(Actor:TActor):PInteger; stdcall; //人物中毒麻痹等
begin
  Result := @Actor.m_nState;
end;
//------------------------------------------------------------------------------

function TActorAPI_m_nBodyOffset(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nBodyOffset;
end;

function TActorAPI_m_boUseMagic(Actor:TActor):PBoolean; stdcall;
begin
  Result := @Actor.m_boUseMagic;
end;

function TActorAPI_m_nCurrentFrame(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nCurrentFrame;
end;

function TActorAPI_m_nStartFrame(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nStartFrame;
end;

function TActorAPI_m_nEndFrame(Actor:TActor):PInteger; stdcall;
begin
  Result := @Actor.m_nEndFrame;
end;

function TActorAPI_m_dwFrameTime(Actor:TActor):PLongWord; stdcall;
begin
  Result := @Actor.m_dwFrameTime;
end;

function TActorAPI_m_dwStartTime(Actor:TActor):PLongWord; stdcall;
begin
  Result := @Actor.m_dwStartTime;
end;
//------------------------------------------------------------------------------

function THookAPI_GetHookInitialize:TInitialize; stdcall;
begin
  Result := HookInitialize;
end;

function THookAPI_GetHookFinalize:TStartPro; stdcall;
begin
  Result := HookFinalize;
end;

function THookAPI_GetHookFormKeyDown:TFormKeyDown; stdcall;
begin
  Result := HookFormKeyDown;
end;

function THookAPI_GetHookFormKeyPress:TFormKeyPress; stdcall;
begin
  Result := HookFormKeyPress;
end;

function THookAPI_GetHookFormMouseDown:TFormMouseDown; stdcall;
begin
  Result := HookFormMouseDown;
end;

function THookAPI_GetHookFormMouseMove:TFormMouseMove; stdcall;
begin
  Result := HookFormMouseMove;
end;

function THookAPI_GetHookDecodeMessagePacketStart:TDecodeMessagePacket; stdcall;
begin
  Result := HookDecodeMessagePacketStart;
end;

function THookAPI_GetHookDecodeMessagePacketStop:TDecodeMessagePacket; stdcall;
begin
  Result := HookDecodeMessagePacketStop;
end;

function THookAPI_GetHookDecodeMessagePacket:TDecodeMessagePacket; stdcall;
begin
  Result := HookDecodeMessagePacket;
end;

function THookAPI_GetHookDrawScene1:TStartPro; stdcall;
begin
  Result := HookDrawScene1;
end;

function THookAPI_GetHookDrawScene2:TStartPro; stdcall;
begin
  Result := HookDrawScene2;
end;

function THookAPI_GetHookDrawScene3:TStartPro; stdcall;
begin
  Result := HookDrawScene3;
end;

function THookAPI_GetHookDrawScene4:TStartPro; stdcall;
begin
  Result := HookDrawScene4;
end;

function THookAPI_GetHookTActor_FeatureChanged:TObjectAction; stdcall;
begin
  Result := HookTActor_FeatureChanged;
end;

function THookAPI_GetHookTActor_CalcActorFrame:TObjectAction; stdcall;
begin
  Result := HookTActor_CalcActorFrame;
end;

function THookAPI_GetHookTActor_DrawChr1:TTActor_DrawChr; stdcall;
begin
  Result := HookTActor_DrawChr1;
end;

function THookAPI_GetHookTActor_DrawChr2:TTActor_DrawChr; stdcall;
begin
  Result := HookTActor_DrawChr2;
end;

function THookAPI_GetHookTHumActor_CalcActorFrame:TObjectAction; stdcall;
begin
  Result := HookTHumActor_CalcActorFrame;
end;

function THookAPI_GetHookTHumActor_DrawChr1:TTActor_DrawChr; stdcall;
begin
  Result := HookTHumActor_DrawChr1;
end;

function THookAPI_GetHookTHumActor_DrawChr2:TTActor_DrawChr; stdcall;
begin
  Result := HookTHumActor_DrawChr2;
end;

function THookAPI_GetHookTHumActor_DrawChr3:TTActor_DrawChr; stdcall;
begin
  Result := HookTHumActor_DrawChr3;
end;

function THookAPI_GetHookTHumActor_DrawChr4:TTActor_DrawChr; stdcall;
begin
  Result := HookTHumActor_DrawChr4;
end;
//------------------------------------------------------------------------------

procedure THookAPI_SetHookInitialize(Value:TInitialize); stdcall;
begin
  HookInitialize := Value;
end;

procedure THookAPI_SetHookFinalize(Value:TStartPro); stdcall;
begin
  HookFinalize := Value;
end;

procedure THookAPI_SetHookFormKeyDown(Value:TFormKeyDown); stdcall;
begin
  HookFormKeyDown := Value;
end;

procedure THookAPI_SetHookFormKeyPress(Value:TFormKeyPress); stdcall;
begin
  HookFormKeyPress := Value;
end;

procedure THookAPI_SetHookFormMouseDown(Value:TFormMouseDown); stdcall;
begin
  HookFormMouseDown := Value;
end;

procedure THookAPI_SetHookFormMouseMove(Value:TFormMouseMove); stdcall;
begin
  HookFormMouseMove := Value;
end;

procedure THookAPI_SetHookDecodeMessagePacketStart(Value:TDecodeMessagePacket); stdcall;
begin
  HookDecodeMessagePacketStart := Value;
end;

procedure THookAPI_SetHookDecodeMessagePacketStop(Value:TDecodeMessagePacket); stdcall;
begin
  HookDecodeMessagePacketStop := Value;
end;

procedure THookAPI_SetHookDecodeMessagePacket(Value:TDecodeMessagePacket); stdcall;
begin
  HookDecodeMessagePacket := Value;
end;

procedure THookAPI_SetHookDrawScene1(Value:TStartPro); stdcall;
begin
  HookDrawScene1 := Value;
end;

procedure THookAPI_SetHookDrawScene2(Value:TStartPro); stdcall;
begin
  HookDrawScene2 := Value;
end;

procedure THookAPI_SetHookDrawScene3(Value:TStartPro); stdcall;
begin
  HookDrawScene3 := Value;
end;

procedure THookAPI_SetHookDrawScene4(Value:TStartPro); stdcall;
begin
  HookDrawScene4 := Value;
end;

procedure THookAPI_SetHookTActor_FeatureChanged(Value:TObjectAction); stdcall;
begin
  HookTActor_FeatureChanged := Value;
end;

procedure THookAPI_SetHookTActor_CalcActorFrame(Value:TObjectAction); stdcall;
begin
  HookTActor_CalcActorFrame := Value;
end;

procedure THookAPI_SetHookTActor_DrawChr1(Value:TTActor_DrawChr); stdcall;
begin
  HookTActor_DrawChr1 := Value;
end;

procedure THookAPI_SetHookTActor_DrawChr2(Value:TTActor_DrawChr); stdcall;
begin
  HookTActor_DrawChr2 := Value;
end;

procedure THookAPI_SetHookTHumActor_CalcActorFrame(Value:TObjectAction); stdcall;
begin
  HookTHumActor_CalcActorFrame := Value;
end;

procedure THookAPI_SetHookTHumActor_DrawChr1(Value:TTActor_DrawChr); stdcall;
begin
  HookTHumActor_DrawChr1 := Value;
end;

procedure THookAPI_SetHookTHumActor_DrawChr2(Value:TTActor_DrawChr); stdcall;
begin
  HookTHumActor_DrawChr2 := Value;
end;

procedure THookAPI_SetHookTHumActor_DrawChr3(Value:TTActor_DrawChr); stdcall;
begin
  HookTHumActor_DrawChr3 := Value;
end;

procedure THookAPI_SetHookTHumActor_DrawChr4(Value:TTActor_DrawChr); stdcall;
begin
  HookTHumActor_DrawChr4 := Value;
end;
//------------------------------------------------------------------------------

procedure InitializeAPI;
begin
  FillChar(ClientAPI, SizeOf(TClientAPI), #0);
  ClientAPI.ImagesAPI.GetHandle := TGameImages_GetHandle;
  ClientAPI.ImagesAPI.Count := TGameImages_Count;
  ClientAPI.ImagesAPI.Read := TGameImages_Read;
  ClientAPI.ImagesAPI.Clear := TGameImages_Clear;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.ListAPI.Create := TListAPI_Create;
  ClientAPI.ListAPI.Free := TListAPI_Free;
  ClientAPI.ListAPI.Count := TListAPI_Count;
  ClientAPI.ListAPI.Add := TListAPI_Add;
  ClientAPI.ListAPI.Insert := TListAPI_Insert;
  ClientAPI.ListAPI.Get := TListAPI_Get;
  ClientAPI.ListAPI.Delete := TListAPI_Delete;
  ClientAPI.ListAPI.Clear := TListAPI_Clear;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.StringListAPI.Create := TStringListAPI_Create;
  ClientAPI.StringListAPI.Free := TStringListAPI_Free;
  ClientAPI.StringListAPI.Count := TStringListAPI_Count;
  ClientAPI.StringListAPI.Add := TStringListAPI_Add;
  ClientAPI.StringListAPI.AddObject := TStringListAPI_AddObject;
  ClientAPI.StringListAPI.Insert := TStringListAPI_Insert;
  ClientAPI.StringListAPI.Get := TStringListAPI_Get;
  ClientAPI.StringListAPI.GetObject := TStringListAPI_GetObject;
  ClientAPI.StringListAPI.Delete := TStringListAPI_Delete;
  ClientAPI.StringListAPI.Clear := TStringListAPI_Clear;

  ClientAPI.ItemMenuListAPI.Count := TItemMenuListAPI_Count;
  ClientAPI.ItemMenuListAPI.Add := TItemMenuListAPI_Add;
  ClientAPI.ItemMenuListAPI.AddObject := TItemMenuListAPI_AddObject;
  ClientAPI.ItemMenuListAPI.Get := TItemMenuListAPI_Get;
  ClientAPI.ItemMenuListAPI.GetObject := TItemMenuListAPI_GetObject;
  ClientAPI.ItemMenuListAPI.Delete := TItemMenuListAPI_Delete;
  ClientAPI.ItemMenuListAPI.Clear := TItemMenuListAPI_Clear;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.TextureAPI.Width := TTextureAPI_Width;
  ClientAPI.TextureAPI.Height := TTextureAPI_Height;
  ClientAPI.TextureAPI.Pixels := TTextureAPI_Pixels;
  ClientAPI.TextureAPI.Lock := TTextureAPI_Lock;
  ClientAPI.TextureAPI.Unlock := TTextureAPI_Unlock;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.DrawAPI.Draw := TDrawAPI_Draw;
  ClientAPI.DrawAPI.DrawColor := TDrawAPI_DrawColor;
  ClientAPI.DrawAPI.StretchDraw := TDrawAPI_StretchDraw;
  ClientAPI.DrawAPI.DrawAlpha := TDrawAPI_DrawAlpha;
  ClientAPI.DrawAPI.DrawColorAlpha := TDrawAPI_DrawColorAlpha;
  ClientAPI.DrawAPI.DrawBlend := TDrawAPI_DrawBlend;
  ClientAPI.DrawAPI.DrawEffect := TDrawAPI_DrawEffect;
  ClientAPI.DrawAPI.FrameRect := TDrawAPI_FrameRect;
  ClientAPI.DrawAPI.FillRect := TDrawAPI_FillRect;
  ClientAPI.DrawAPI.FillRectAlpha := TDrawAPI_FillRectAlpha;
  ClientAPI.DrawAPI.Line := TDrawAPI_Line;
  ClientAPI.DrawAPI.FillTri := TDrawAPI_FillTri;
  ClientAPI.DrawAPI.Circle := TDrawAPI_Circle;
  ClientAPI.DrawAPI.CurrentFont := TDrawAPI_CurrentFont;
  ClientAPI.DrawAPI.FindFont := TDrawAPI_FindFont;
  ClientAPI.DrawAPI.TextRect := TDrawAPI_TextRect;
  ClientAPI.DrawAPI.TextOut := TDrawAPI_TextOut;
  ClientAPI.DrawAPI.BoldTextOut := TDrawAPI_BoldTextOut;
  ClientAPI.DrawAPI.TextWidth := TDrawAPI_TextWidth;
  ClientAPI.DrawAPI.TextHeight := TDrawAPI_TextHeight;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.GameInterfaceAPI.DMainMenu := TGameInterfaceAPI_DMainMenu;
  ClientAPI.GameInterfaceAPI.DLoginDlg := TGameInterfaceAPI_DLoginDlg; //登录背景窗口
  ClientAPI.GameInterfaceAPI.DRandomCodeDlg := TGameInterfaceAPI_DRandomCodeDlg; //随机码窗口
  ClientAPI.GameInterfaceAPI.DLogin := TGameInterfaceAPI_DLogin; //登录对话框窗口
  ClientAPI.GameInterfaceAPI.DNewAccount := TGameInterfaceAPI_DNewAccount; //注册帐号窗口
  ClientAPI.GameInterfaceAPI.DChgPw := TGameInterfaceAPI_DChgPw; //修改密码窗口
  ClientAPI.GameInterfaceAPI.DSelServerDlg := TGameInterfaceAPI_DSelServerDlg; //选择服务器背景窗口
  ClientAPI.GameInterfaceAPI.DServerDlg := TGameInterfaceAPI_DServerDlg; //选择服务器窗口
  ClientAPI.GameInterfaceAPI.DDoorDlg := TGameInterfaceAPI_DDoorDlg; //开门时的背景窗口
  ClientAPI.GameInterfaceAPI.DSelectChr := TGameInterfaceAPI_DSelectChr; //选择角色背景窗口
  ClientAPI.GameInterfaceAPI.DCreateChr := TGameInterfaceAPI_DCreateChr; //创建角色窗口
  ClientAPI.GameInterfaceAPI.DDeleteHumanDlg := TGameInterfaceAPI_DDeleteHumanDlg; //恢复角色窗口
  ClientAPI.GameInterfaceAPI.DNoticeDlg := TGameInterfaceAPI_DNoticeDlg; //公告窗口
  ClientAPI.GameInterfaceAPI.DMerchantDlg := TGameInterfaceAPI_DMerchantDlg; //NPC对话框
  ClientAPI.GameInterfaceAPI.DBottomLeft := TGameInterfaceAPI_DBottomLeft; //游戏界面左
  ClientAPI.GameInterfaceAPI.DBottomCenter := TGameInterfaceAPI_DBottomCenter; //游戏界面中
  ClientAPI.GameInterfaceAPI.DBottomRight := TGameInterfaceAPI_DBottomRight; //游戏界面右
  ClientAPI.GameInterfaceAPI.DItemBag := TGameInterfaceAPI_DItemBag; //包裹窗口
  ClientAPI.GameInterfaceAPI.DStateWin := TGameInterfaceAPI_DStateWin; //人物属性窗口 自己
  ClientAPI.GameInterfaceAPI.DUserState1 := TGameInterfaceAPI_DUserState1; //人物属性窗口 查看对方
  ClientAPI.GameInterfaceAPI.DHeroStateWin := TGameInterfaceAPI_DHeroStateWin; //英雄属性窗口
  ClientAPI.GameInterfaceAPI.DHeroStateDlg := TGameInterfaceAPI_DHeroStateDlg; //英雄状态窗口
  ClientAPI.GameInterfaceAPI.DHeroItemBag := TGameInterfaceAPI_DHeroItemBag; //英雄包裹窗口
  ClientAPI.GameInterfaceAPI.DMenuDlg := TGameInterfaceAPI_DMenuDlg; // NPC列表框
  ClientAPI.GameInterfaceAPI.DSellDlg := TGameInterfaceAPI_DSellDlg; //OK框
  ClientAPI.GameInterfaceAPI.DDealDlg := TGameInterfaceAPI_DDealDlg; //交易对话框 自己
  ClientAPI.GameInterfaceAPI.DDealRemoteDlg := TGameInterfaceAPI_DDealRemoteDlg; //交易对话框 对方
  ClientAPI.GameInterfaceAPI.DShopDlg := TGameInterfaceAPI_DShopDlg; //商铺窗口
  ClientAPI.GameInterfaceAPI.DGroupDlg := TGameInterfaceAPI_DGroupDlg; //组队窗口
  ClientAPI.GameInterfaceAPI.DRankingDlg := TGameInterfaceAPI_DRankingDlg; //排行榜窗口
  ClientAPI.GameInterfaceAPI.DGuildDlg := TGameInterfaceAPI_DGuildDlg; //行会窗口
  ClientAPI.GameInterfaceAPI.DGuildEditNotice := TGameInterfaceAPI_DGuildEditNotice; //行会编辑窗口
  ClientAPI.GameInterfaceAPI.DAdjustAbility := TGameInterfaceAPI_DAdjustAbility; //附加属性窗口
  ClientAPI.GameInterfaceAPI.DMissionDlg := TGameInterfaceAPI_DMissionDlg; //任务日记窗口
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.SocketAPI.SendSocket := TSocketAPI_SendSocket;
  ClientAPI.SocketAPI.SendClientMessage := TSocketAPI_SendClientMessage;
  ClientAPI.SocketAPI.SendLogin := TSocketAPI_SendLogin; //登录
  ClientAPI.SocketAPI.SendSelectServer := TSocketAPI_SendSelectServer; //现在服务器
  ClientAPI.SocketAPI.SendQueryChr := TSocketAPI_SendQueryChr; //查询角色
  ClientAPI.SocketAPI.SendSelChr := TSocketAPI_SendSelChr; //选择角色

  ClientAPI.SocketAPI.SendSay := TSocketAPI_SendSay;
  ClientAPI.SocketAPI.Close := TSocketAPI_Close;
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.GameAPI.ClientPath := TGameAPI_ClientPath; //登录器路径
  ClientAPI.GameAPI.ClientName := TGameAPI_ClientName; //登录器名称
  ClientAPI.GameAPI.MySelf := TGameAPI_MySelf;
  ClientAPI.GameAPI.MyHero := TGameAPI_MyHero;
  ClientAPI.GameAPI.MagicList := TGameAPI_MagicList; //技能列表
  ClientAPI.GameAPI.MagicNGList := TGameAPI_MagicNGList; //内功技能列表
  ClientAPI.GameAPI.ContinuousMagicList := TGameAPI_ContinuousMagicList; //连击技能列表

  ClientAPI.GameAPI.HeroMagicList := TGameAPI_HeroMagicList;
  ClientAPI.GameAPI.HeroMagicNGList := TGameAPI_HeroMagicNGList; //英雄内功技能列表
  ClientAPI.GameAPI.HeroContinuousMagicList := TGameAPI_HeroContinuousMagicList; //英雄连击技能列表

  ClientAPI.GameAPI.GroupMembers := TGameAPI_GroupMembers; //组列表
  ClientAPI.GameAPI.DropedItemList := TGameAPI_DropedItemList; //地面物品列表
  ClientAPI.GameAPI.MenuItemList := TGameAPI_MenuItemList; //当前NPC出售物品列表

  ClientAPI.GameAPI.ActorList := TGameAPI_ActorList;
  ClientAPI.GameAPI.ScreenXYfromMCXY := TGameAPI_ScreenXYfromMCXY; //地图坐标转换屏幕坐标
  ClientAPI.GameAPI.CXYfromMouseXY := TGameAPI_CXYfromMouseXY; //屏幕坐标转换地图坐标
  ClientAPI.GameAPI.FindActor1 := TGameAPI_FindActor1;
  ClientAPI.GameAPI.FindActor2 := TGameAPI_FindActor2;
  ClientAPI.GameAPI.FindActorXY1 := TGameAPI_FindActorXY1;
  ClientAPI.GameAPI.FindActorXY2 := TGameAPI_FindActorXY2;
  ClientAPI.GameAPI.CanWalk := TGameAPI_CanWalk;
  ClientAPI.GameAPI.CanRun := TGameAPI_CanRun;
  ClientAPI.GameAPI.CanHorseRun := TGameAPI_CanHorseRun;
  ClientAPI.GameAPI.GetRGB := TGameAPI_GetRGB;
  ClientAPI.GameAPI.DebugOutStr := TGameAPI_DebugOutStr; //写入日记
  ClientAPI.GameAPI.AppLogout := TGameAPI_AppLogout; //小退
  ClientAPI.GameAPI.AppExit := TGameAPI_AppExit; //退出游戏
  ClientAPI.GameAPI.DMessageDlg := TGameAPI_DMessageDlg; //弹出对话框
  ClientAPI.GameAPI.AddChatBoardString := TGameAPI_AddChatBoardString; //聊天框显示信息
  ClientAPI.GameAPI.AddTopChatBoardString := TGameAPI_AddTopChatBoardString; //聊天框固顶信息
  ClientAPI.GameAPI.AddMoveMsg := TGameAPI_AddMoveMsg; //滚动信息
  ClientAPI.GameAPI.ShowHint := TGameAPI_ShowHint; //显示悬浮框
  ClientAPI.GameAPI.ShowMouseItemInfo := TGameAPI_ShowMouseItemInfo; //悬浮框显示装备信息
  ClientAPI.GameAPI.ClearHint := TGameAPI_ClearHint; //清除悬浮框
  ClientAPI.GameAPI.DlgEditText := TGameAPI_DlgEditText;
  ClientAPI.GameAPI.PlaySound := TGameAPI_PlaySound;
  ClientAPI.GameAPI.PlaySoundA := TGameAPI_PlaySoundA;
  ClientAPI.GameAPI.ItemClickSound := TGameAPI_ItemClickSound;
  ClientAPI.GameAPI.ItemBag := TGameAPI_ItemBag; //包裹物品指针
  ClientAPI.GameAPI.HeroItemBag := TGameAPI_HeroItemBag; //英雄包裹物品指针
  ClientAPI.GameAPI.UseItems := TGameAPI_UseItems; //身上装备指针
  ClientAPI.GameAPI.HeroUseItems := TGameAPI_HeroUseItems; //英雄身上装备指针

  ClientAPI.GameAPI.JewelryBoxItems := TGameAPI_JewelryBoxItems; // 首饰盒物品指针
  ClientAPI.GameAPI.HeroJewelryBoxItems := TGameAPI_HeroJewelryBoxItems; // 英雄首饰盒物品指针
  // 神佑盒物品指针
  ClientAPI.GameAPI.GodBlessItems := TGameAPI_GodBlessItems; // 英雄神佑盒物品指针
  ClientAPI.GameAPI.HeroGodBlessItems := TGameAPI_HeroGodBlessItems;

  ClientAPI.GameAPI.ItemBoxItems := TGameAPI_ItemBoxItems; // 自定义OK框物品指针

  ClientAPI.GameAPI.UserState1 := TGameAPI_UserState1; //查看的别人身上装备指针
  ClientAPI.GameAPI.boItemMoving := TGameAPI_boItemMoving;
  ClientAPI.GameAPI.MovingItem := TGameAPI_MovingItem; //pTMovingItem; //当前正在移动的物品指针
  ClientAPI.GameAPI.WaitingUseItem := TGameAPI_WaitingUseItem; //pTMovingItem;
  ClientAPI.GameAPI.SellDlgItem := TGameAPI_SellDlgItem; //pTMovingItem; 当前OK框物品
  ClientAPI.GameAPI.nTargetX := TGameAPI_nTargetX; //目标座标
  ClientAPI.GameAPI.nTargetY := TGameAPI_nTargetY; //目标座标
  ClientAPI.GameAPI.TargetCret := TGameAPI_TargetCret;
  ClientAPI.GameAPI.FocusCret := TGameAPI_FocusCret;
  ClientAPI.GameAPI.MagicTarget := TGameAPI_MagicTarget;

  ClientAPI.GameAPI.Gold := TGameAPI_Gold; //金币数量
  ClientAPI.GameAPI.GameGold := TGameAPI_GameGold; //元宝数量
  ClientAPI.GameAPI.GamePoint := TGameAPI_GamePoint; //游戏点数量
  ClientAPI.GameAPI.GloryPoint := TGameAPI_GloryPoint; //荣誉
  ClientAPI.GameAPI.GameDiamond := TGameAPI_GameDiamond; //金刚石
  ClientAPI.GameAPI.GameGird := TGameAPI_GameGird; //灵符
  ClientAPI.GameAPI.GameGlory := TGameAPI_GameGlory; //荣誉
  ClientAPI.GameAPI.LoyaltyPoint := TGameAPI_LoyaltyPoint; //忠诚度
  ClientAPI.GameAPI.GameGoldName := TGameAPI_GameGoldName; //元宝名称
  ClientAPI.GameAPI.GamePointName := TGameAPI_GamePointName; //游戏点名称
  ClientAPI.GameAPI.GameDiamondName := TGameAPI_GameDiamondName; //金刚石名称
  ClientAPI.GameAPI.GameGirdName := TGameAPI_GameGirdName; //灵符名称
  ClientAPI.GameAPI.EncodeBuffer := TGameAPI_EncodeBuffer;
  ClientAPI.GameAPI.DecodeBuffer := TGameAPI_DecodeBuffer;
  ClientAPI.GameAPI.FullScreenDrawScene := TGameAPI_FullScreenDrawScene;
  ClientAPI.GameAPI.AreaStateValue := TGameAPI_AreaStateValue; //当前是在攻城区域
  ClientAPI.GameAPI.MapTitle := TGameAPI_MapTitle; //当前地图名称
  ClientAPI.GameAPI.EatItem := TGameAPI_EatItem; //使用物品
  ClientAPI.GameAPI.HeroEatItem := TGameAPI_HeroEatItem; //英雄使用物品
  ClientAPI.GameAPI.ServerImageList := TGameAPI_ServerImageList;
  ClientAPI.GameAPI.ClassDlg := TGameAPI_ClassDlg;
  ClientAPI.GameAPI.SetMovingItem := TGameAPI_SetMovingItem;
  ClientAPI.GameAPI.SetWaitingUseItem := TGameAPI_SetWaitingUseItem;
  ClientAPI.GameAPI.SetSellDlgItem := TGameAPI_SetSellDlgItem;
  ClientAPI.GameAPI.SetItemBag := TGameAPI_SetItemBag;
  ClientAPI.GameAPI.SetHeroItemBag := TGameAPI_SetHeroItemBag;
  ClientAPI.GameAPI.SetUseItems := TGameAPI_SetUseItems;
  ClientAPI.GameAPI.SetHeroUseItems := TGameAPI_SetHeroUseItems;

  ClientAPI.GameAPI.SetJewelryBoxItems := TGameAPI_SetJewelryBoxItems; // 首饰盒物品指针
  ClientAPI.GameAPI.SetHeroJewelryBoxItems := TGameAPI_SetHeroJewelryBoxItems; // 英雄首饰盒物品指针
  ClientAPI.GameAPI.SetGodBlessItems := TGameAPI_SetGodBlessItems; // 神佑盒物品指针
  ClientAPI.GameAPI.SetHeroGodBlessItems := TGameAPI_SetHeroGodBlessItems; // 英雄神佑盒物品指针
  ClientAPI.GameAPI.SetItemBoxItems := TGameAPI_SetItemBoxItems; // 自定义OK框物品指针

  ClientAPI.GameAPI.SetUserState1 := TGameAPI_SetUserState1;
  ClientAPI.GameAPI.SetItemMoving := TGameAPI_SetItemMoving;

  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  //------------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DControl.Create := TDControl_Create;
  ClientAPI.InterfaceAPI.DControl.InterfaceType := TDControl_InterfaceType;
  ClientAPI.InterfaceAPI.DControl.Name := TDControl_Name;
  ClientAPI.InterfaceAPI.DControl.Left := TDControl_Left;
  ClientAPI.InterfaceAPI.DControl.Top := TDControl_Top;
  ClientAPI.InterfaceAPI.DControl.Width := TDControl_Width;
  ClientAPI.InterfaceAPI.DControl.Height := TDControl_Height;
  ClientAPI.InterfaceAPI.DControl.Tag := TDControl_Tag;
  ClientAPI.InterfaceAPI.DControl.Visible := TDControl_Visible;
  ClientAPI.InterfaceAPI.DControl.Enabled := TDControl_Enabled;
  ClientAPI.InterfaceAPI.DControl.Floating := TDControl_Floating;
  ClientAPI.InterfaceAPI.DControl.ParentMove := TDControl_ParentMove;
  ClientAPI.InterfaceAPI.DControl.EnableFocus := TDControl_EnableFocus;
  ClientAPI.InterfaceAPI.DControl.AutoSize := TDControl_AutoSize;
  ClientAPI.InterfaceAPI.DControl.DrawBorder := TDControl_DrawBorder;
  ClientAPI.InterfaceAPI.DControl.Caption := TDControl_Caption;
  ClientAPI.InterfaceAPI.DControl.Alignment := TDControl_Alignment;
  ClientAPI.InterfaceAPI.DControl.Transparent := TDControl_Transparent;
  ClientAPI.InterfaceAPI.DControl.BackgroundColor := TDControl_BackgroundColor;
  ClientAPI.InterfaceAPI.DControl.PopupMenu := TDControl_PopupMenu;
  ClientAPI.InterfaceAPI.DControl.VisibleRect := TDControl_VisibleRect;
  ClientAPI.InterfaceAPI.DControl.VirtualRect := TDControl_VirtualRect;

  { ClientAPI.InterfaceAPI.DControl.DefaultBorderColor := TDControl_DefaultBorderColor;
   ClientAPI.InterfaceAPI.DControl.DefaultBorderBold := TDControl_DefaultBorderBold;
   ClientAPI.InterfaceAPI.DControl.MouseMoveBorderColor := TDControl_MouseMoveBorderColor;
   ClientAPI.InterfaceAPI.DControl.MouseMoveBorderBold := TDControl_MouseMoveBorderBold;
   ClientAPI.InterfaceAPI.DControl.MouseDownBorderColor := TDControl_MouseDownBorderColor;
   ClientAPI.InterfaceAPI.DControl.MouseDownBorderBold := TDControl_MouseDownBorderBold;
   ClientAPI.InterfaceAPI.DControl.DisabledBorderColor := TDControl_DisabledBorderColor;
   ClientAPI.InterfaceAPI.DControl.DisabledBorderBold := TDControl_DisabledBorderBold;

   ClientAPI.InterfaceAPI.DControl.Images := TDControl_Images;
   ClientAPI.InterfaceAPI.DControl.DefaultImageIndex := TDControl_DefaultImageIndex;
   ClientAPI.InterfaceAPI.DControl.MouseMoveImageIndex := TDControl_MouseMoveImageIndex;
   ClientAPI.InterfaceAPI.DControl.MouseDownImageIndex := TDControl_MouseDownImageIndex;
   ClientAPI.InterfaceAPI.DControl.DisabledImageIndex := TDControl_DisabledImageIndex; }

  ClientAPI.InterfaceAPI.DControl.OnShow := TDControl_OnShow;
  ClientAPI.InterfaceAPI.DControl.OnHide := TDControl_OnHide;
  ClientAPI.InterfaceAPI.DControl.OnKeyDown := TDControl_OnKeyDown;
  ClientAPI.InterfaceAPI.DControl.OnKeyPress := TDControl_OnKeyPress;
  ClientAPI.InterfaceAPI.DControl.OnKeyUp := TDControl_OnKeyUp;
  ClientAPI.InterfaceAPI.DControl.OnClick := TDControl_OnClick;
  ClientAPI.InterfaceAPI.DControl.OnDblClick := TDControl_OnDblClick;
  ClientAPI.InterfaceAPI.DControl.OnMouseDown := TDControl_OnMouseDown;
  ClientAPI.InterfaceAPI.DControl.OnMouseMove := TDControl_OnMouseMove;
  ClientAPI.InterfaceAPI.DControl.OnMouseUp := TDControl_OnMouseUp;
  ClientAPI.InterfaceAPI.DControl.OnMouseEnter := TDControl_OnMouseEnter;
  ClientAPI.InterfaceAPI.DControl.OnMouseLeave := TDControl_OnMouseLeave;
  ClientAPI.InterfaceAPI.DControl.OnInRealArea := TDControl_OnInRealArea;
  ClientAPI.InterfaceAPI.DControl.OnPaint := TDControl_OnPaint;
  ClientAPI.InterfaceAPI.DControl.OnStartPaint := TDControl_OnStartPaint;
  ClientAPI.InterfaceAPI.DControl.OnStartSubPaint := TDControl_OnStartSubPaint;
  ClientAPI.InterfaceAPI.DControl.OnStopPaint := TDControl_OnStopPaint;
  ClientAPI.InterfaceAPI.DControl.OnPress := TDControl_OnPress;

  //------------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DControl.SetName := TDControl_SetName;
  ClientAPI.InterfaceAPI.DControl.SetLeft := TDControl_SetLeft;
  ClientAPI.InterfaceAPI.DControl.SetTop := TDControl_SetTop;
  ClientAPI.InterfaceAPI.DControl.SetWidth := TDControl_SetWidth;
  ClientAPI.InterfaceAPI.DControl.SetHeight := TDControl_SetHeight;
  ClientAPI.InterfaceAPI.DControl.SetTag := TDControl_SetTag;
  ClientAPI.InterfaceAPI.DControl.SetVisible := TDControl_SetVisible;
  ClientAPI.InterfaceAPI.DControl.SetEnabled := TDControl_SetEnabled;
  ClientAPI.InterfaceAPI.DControl.SetFloating := TDControl_SetFloating;
  ClientAPI.InterfaceAPI.DControl.SetParentMove := TDControl_SetParentMove;
  ClientAPI.InterfaceAPI.DControl.SetEnableFocus := TDControl_SetEnableFocus;
  ClientAPI.InterfaceAPI.DControl.SetAutoSize := TDControl_SetAutoSize;
  ClientAPI.InterfaceAPI.DControl.SetDrawBorder := TDControl_SetDrawBorder;
  ClientAPI.InterfaceAPI.DControl.SetCaption := TDControl_SetCaption;
  ClientAPI.InterfaceAPI.DControl.SetAlignment := TDControl_SetAlignment;
  ClientAPI.InterfaceAPI.DControl.SetTransparent := TDControl_SetTransparent;
  ClientAPI.InterfaceAPI.DControl.SetBackgroundColor := TDControl_SetBackgroundColor;
  ClientAPI.InterfaceAPI.DControl.SetPopupMenu := TDControl_SetPopupMenu;

  ClientAPI.InterfaceAPI.DControl.SetDefaultBorderColor := TDControl_SetDefaultBorderColor;
  ClientAPI.InterfaceAPI.DControl.SetDefaultBorderBold := TDControl_SetDefaultBorderBold;
  ClientAPI.InterfaceAPI.DControl.SetMouseMoveBorderColor := TDControl_SetMouseMoveBorderColor;
  ClientAPI.InterfaceAPI.DControl.SetMouseMoveBorderBold := TDControl_SetMouseMoveBorderBold;
  ClientAPI.InterfaceAPI.DControl.SetMouseDownBorderColor := TDControl_SetMouseDownBorderColor;
  ClientAPI.InterfaceAPI.DControl.SetMouseDownBorderBold := TDControl_SetMouseDownBorderBold;
  ClientAPI.InterfaceAPI.DControl.SetDisabledBorderColor := TDControl_SetDisabledBorderColor;
  ClientAPI.InterfaceAPI.DControl.SetDisabledBorderBold := TDControl_SetDisabledBorderBold;

  ClientAPI.InterfaceAPI.DControl.SetImages := TDControl_SetImages;
  ClientAPI.InterfaceAPI.DControl.SetDefaultImageIndex := TDControl_SetDefaultImageIndex;
  ClientAPI.InterfaceAPI.DControl.SetMouseMoveImageIndex := TDControl_SetMouseMoveImageIndex;
  ClientAPI.InterfaceAPI.DControl.SetMouseDownImageIndex := TDControl_SetMouseDownImageIndex;
  ClientAPI.InterfaceAPI.DControl.SetDisabledImageIndex := TDControl_SetDisabledImageIndex;

  ClientAPI.InterfaceAPI.DControl.GetImages := TDControl_GetImages;
  ClientAPI.InterfaceAPI.DControl.GetDefaultImageIndex := TDControl_GetDefaultImageIndex;
  ClientAPI.InterfaceAPI.DControl.GetMouseMoveImageIndex := TDControl_GetMouseMoveImageIndex;
  ClientAPI.InterfaceAPI.DControl.GetMouseDownImageIndex := TDControl_GetMouseDownImageIndex;
  ClientAPI.InterfaceAPI.DControl.GetDisabledImageIndex := TDControl_GetDisabledImageIndex;

  ClientAPI.InterfaceAPI.DControl.SetOnShow := TDControl_SetOnShow;
  ClientAPI.InterfaceAPI.DControl.SetOnHide := TDControl_SetOnHide;
  ClientAPI.InterfaceAPI.DControl.SetOnKeyDown := TDControl_SetOnKeyDown;
  ClientAPI.InterfaceAPI.DControl.SetOnKeyPress := TDControl_SetOnKeyPress;
  ClientAPI.InterfaceAPI.DControl.SetOnKeyUp := TDControl_SetOnKeyUp;
  ClientAPI.InterfaceAPI.DControl.SetOnClick := TDControl_SetOnClick;
  ClientAPI.InterfaceAPI.DControl.SetOnDblClick := TDControl_SetOnDblClick;
  ClientAPI.InterfaceAPI.DControl.SetOnMouseDown := TDControl_SetOnMouseDown;
  ClientAPI.InterfaceAPI.DControl.SetOnMouseMove := TDControl_SetOnMouseMove;
  ClientAPI.InterfaceAPI.DControl.SetOnMouseUp := TDControl_SetOnMouseUp;
  ClientAPI.InterfaceAPI.DControl.SetOnMouseEnter := TDControl_SetOnMouseEnter;
  ClientAPI.InterfaceAPI.DControl.SetOnMouseLeave := TDControl_SetOnMouseLeave;
  ClientAPI.InterfaceAPI.DControl.SetOnInRealArea := TDControl_SetOnInRealArea;
  ClientAPI.InterfaceAPI.DControl.SetOnPaint := TDControl_SetOnPaint;
  ClientAPI.InterfaceAPI.DControl.SetOnStartPaint := TDControl_SetOnStartPaint;
  ClientAPI.InterfaceAPI.DControl.SetOnStartSubPaint := TDControl_SetOnStartSubPaint;
  ClientAPI.InterfaceAPI.DControl.SetOnStopPaint := TDControl_SetOnStopPaint;
  ClientAPI.InterfaceAPI.DControl.SetOnPress := TDControl_SetOnPress;
  //-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DControl.Parent := TDControl_Parent;
  ClientAPI.InterfaceAPI.DControl.SetParent := TDControl_SetParent;
  ClientAPI.InterfaceAPI.DControl.ControlCount := TDControl_ControlCount;
  ClientAPI.InterfaceAPI.DControl.Controls := TDControl_Controls;

  ClientAPI.InterfaceAPI.DControl.SetFocus := TDControl_SetFocus;
  ClientAPI.InterfaceAPI.DControl.BringToFront := TDControl_BringToFront;
  ClientAPI.InterfaceAPI.DControl.InRange := TDControl_InRange;

  //-----------------------------------------------------------------------------
    //ClientAPI.InterfaceAPI.DWindow.IsBringToFront := TDWindow_IsBringToFront;
  //-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DWindow.SetIsBringToFront := TDWindow_SetIsBringToFront;
  //-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DWindow.ShowModalEx := TDWindow_ShowModalEx;
  ClientAPI.InterfaceAPI.DWindow.ShowModalA := TDWindow_ShowModalA;
  ClientAPI.InterfaceAPI.DWindow.ShowModalB := TDWindow_ShowModalB;

  //-----------------------------------------------------------------------------

  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontFColor := TDButton_SetDefaultCaptionFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontBColor := TDButton_SetDefaultCaptionFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontStyle := TDButton_SetDefaultCaptionFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontSize := TDButton_SetDefaultCaptionFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontBold := TDButton_SetDefaultCaptionFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DButton.SetDefaultCaptionFontName := TDButton_SetDefaultCaptionFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontFColor := TDButton_SetMouseMoveCaptionFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontBColor := TDButton_SetMouseMoveCaptionFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontStyle := TDButton_SetMouseMoveCaptionFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontSize := TDButton_SetMouseMoveCaptionFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontBold := TDButton_SetMouseMoveCaptionFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DButton.SetMouseMoveCaptionFontName := TDButton_SetMouseMoveCaptionFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontFColor := TDButton_SetMouseDownCaptionFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontBColor := TDButton_SetMouseDownCaptionFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontStyle := TDButton_SetMouseDownCaptionFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontSize := TDButton_SetMouseDownCaptionFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontBold := TDButton_SetMouseDownCaptionFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DButton.SetMouseDownCaptionFontName := TDButton_SetMouseDownCaptionFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontFColor := TDButton_SetDisabledCaptionFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontBColor := TDButton_SetDisabledCaptionFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontStyle := TDButton_SetDisabledCaptionFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontSize := TDButton_SetDisabledCaptionFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontBold := TDButton_SetDisabledCaptionFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DButton.SetDisabledCaptionFontName := TDButton_SetDisabledCaptionFontName; //不可用时字体名称

  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontFColor := TDButton_SetCheckedCaptionFontFColor; //勾选时标题字体颜色
  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontBColor := TDButton_SetCheckedCaptionFontBColor; //勾选时标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontStyle := TDButton_SetCheckedCaptionFontStyle; //勾选时字体样式
  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontSize := TDButton_SetCheckedCaptionFontSize; //勾选时字体尺寸
  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontBold := TDButton_SetCheckedCaptionFontBold; //勾选时是否描边
  ClientAPI.InterfaceAPI.DButton.SetCheckedCaptionFontName := TDButton_SetCheckedCaptionFontName; //勾选时字体名称

  {ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontFColor := TDButton_DefaultCaptionFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontBColor := TDButton_DefaultCaptionFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontStyle := TDButton_DefaultCaptionFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontSize := TDButton_DefaultCaptionFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontBold := TDButton_DefaultCaptionFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DButton.DefaultCaptionFontName := TDButton_DefaultCaptionFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontFColor := TDButton_MouseMoveCaptionFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontBColor := TDButton_MouseMoveCaptionFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontStyle := TDButton_MouseMoveCaptionFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontSize := TDButton_MouseMoveCaptionFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontBold := TDButton_MouseMoveCaptionFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DButton.MouseMoveCaptionFontName := TDButton_MouseMoveCaptionFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontFColor := TDButton_MouseDownCaptionFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontBColor := TDButton_MouseDownCaptionFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontStyle := TDButton_MouseDownCaptionFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontSize := TDButton_MouseDownCaptionFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontBold := TDButton_MouseDownCaptionFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DButton.MouseDownCaptionFontName := TDButton_MouseDownCaptionFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontFColor := TDButton_DisabledCaptionFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontBColor := TDButton_DisabledCaptionFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontStyle := TDButton_DisabledCaptionFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontSize := TDButton_DisabledCaptionFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontBold := TDButton_DisabledCaptionFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DButton.DisabledCaptionFontName := TDButton_DisabledCaptionFontName; //不可用时字体名称

  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontFColor := TDButton_CheckedCaptionFontFColor; //勾选时标题字体颜色
  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontBColor := TDButton_CheckedCaptionFontBColor; //勾选时标题字体描边颜色
  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontStyle := TDButton_CheckedCaptionFontStyle; //勾选时字体样式
  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontSize := TDButton_CheckedCaptionFontSize; //勾选时字体尺寸
  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontBold := TDButton_CheckedCaptionFontBold; //勾选时是否描边
  ClientAPI.InterfaceAPI.DButton.CheckedCaptionFontName := TDButton_CheckedCaptionFontName; //勾选时字体名称 }

  ClientAPI.InterfaceAPI.DButton.Style := TDButton_Style; //按钮样式
  ClientAPI.InterfaceAPI.DButton.Checked := TDButton_Checked;
  {ClientAPI.InterfaceAPI.DButton.CaptionDownOffsetX := TDButton_CaptionDownOffsetX; //按下后标题偏移X
  ClientAPI.InterfaceAPI.DButton.CaptionDownOffsetY := TDButton_CaptionDownOffsetY; //按下后标题偏移Y
  ClientAPI.InterfaceAPI.DButton.ButtonDownOffsetX := TDButton_ButtonDownOffsetX; //按下后偏移X
  ClientAPI.InterfaceAPI.DButton.ButtonDownOffsetY := TDButton_ButtonDownOffsetY; //按下后偏移Y
  ClientAPI.InterfaceAPI.DButton.ClickCount := TDButton_ClickCount; //按钮点击的声音}

  ClientAPI.InterfaceAPI.DButton.SetStyle := TDButton_SetStyle; //按钮样式
  ClientAPI.InterfaceAPI.DButton.SetChecked := TDButton_SetChecked;
  ClientAPI.InterfaceAPI.DButton.SetCaptionDownOffsetX := TDButton_SetCaptionDownOffsetX; //按下后标题偏移X
  ClientAPI.InterfaceAPI.DButton.SetCaptionDownOffsetY := TDButton_SetCaptionDownOffsetY; //按下后标题偏移Y
  ClientAPI.InterfaceAPI.DButton.SetButtonDownOffsetX := TDButton_SetButtonDownOffsetX; //按下后偏移X
  ClientAPI.InterfaceAPI.DButton.SetButtonDownOffsetY := TDButton_SetButtonDownOffsetY; //按下后偏移Y
  ClientAPI.InterfaceAPI.DButton.SetClickCount := TDButton_SetClickCount; //按钮点击的声音

  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DEdit.Text := TDEdit_Text;
  ClientAPI.InterfaceAPI.DEdit.Value := TDEdit_Value;
  ClientAPI.InterfaceAPI.DEdit.ReadOnly := TDEdit_ReadOnly; //是否只读
  ClientAPI.InterfaceAPI.DEdit.MaxLength := TDEdit_MaxLength; //最大长度
  ClientAPI.InterfaceAPI.DEdit.SelectedColor := TDEdit_SelectedColor; //光标颜色
  ClientAPI.InterfaceAPI.DEdit.SelBackColor := TDEdit_SelBackColor; //选择字体背景色
  ClientAPI.InterfaceAPI.DEdit.SelFontColor := TDEdit_SelFontColor; //选择字体色
  ClientAPI.InterfaceAPI.DEdit.PasswordChar := TDEdit_PasswordChar;
  ClientAPI.InterfaceAPI.DEdit.AllowSelect := TDEdit_AllowSelect; //是否允许选择
  ClientAPI.InterfaceAPI.DEdit.AllowPaste := TDEdit_AllowPaste; //是否允许粘贴
  ClientAPI.InterfaceAPI.DEdit.InValue := TDEdit_InValue; //允许输入控制
  ClientAPI.InterfaceAPI.DEdit.TabOrder := TDEdit_TabOrder;
  ClientAPI.InterfaceAPI.DEdit.OnChange := TDEdit_OnChange; //输入框改变触发
  //-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DEdit.SetText := TDEdit_SetText;
  ClientAPI.InterfaceAPI.DEdit.SetValue := TDEdit_SetValue;
  ClientAPI.InterfaceAPI.DEdit.SetReadOnly := TDEdit_SetReadOnly; //是否只读
  ClientAPI.InterfaceAPI.DEdit.SetMaxLength := TDEdit_SetMaxLength; //最大长度
  ClientAPI.InterfaceAPI.DEdit.SetSelectedColor := TDEdit_SetSelectedColor; //光标颜色
  ClientAPI.InterfaceAPI.DEdit.SetSelBackColor := TDEdit_SetSelBackColor; //选择字体背景色
  ClientAPI.InterfaceAPI.DEdit.SetSelFontColor := TDEdit_SetSelFontColor; //选择字体色
  ClientAPI.InterfaceAPI.DEdit.SetPasswordChar := TDEdit_SetPasswordChar;
  ClientAPI.InterfaceAPI.DEdit.SetAllowSelect := TDEdit_SetAllowSelect; //是否允许选择
  ClientAPI.InterfaceAPI.DEdit.SetAllowPaste := TDEdit_SetAllowPaste; //是否允许粘贴
  ClientAPI.InterfaceAPI.DEdit.SetInValue := TDEdit_SetInValue; //允许输入控制
  ClientAPI.InterfaceAPI.DEdit.SetTabOrder := TDEdit_SetTabOrder;
  ClientAPI.InterfaceAPI.DEdit.SetOnChange := TDEdit_SetOnChange;
  //-----------------------------------------------------------------------------

  ClientAPI.InterfaceAPI.DGrid.ColCount := TDGrid_ColCount; //列数
  ClientAPI.InterfaceAPI.DGrid.RowCount := TDGrid_RowCount; //组数
  ClientAPI.InterfaceAPI.DGrid.ColWidth := TDGrid_ColWidth; //列宽
  ClientAPI.InterfaceAPI.DGrid.RowHeight := TDGrid_RowHeight; //组高

  ClientAPI.InterfaceAPI.DGrid.OnGridSelect := TDGrid_OnGridSelect; //选择事件
  ClientAPI.InterfaceAPI.DGrid.OnGridMouseMove := TDGrid_OnGridMouseMove; //鼠标移动事件
  ClientAPI.InterfaceAPI.DGrid.OnGridPaint := TDGrid_OnGridPaint; //绘制事件
  //-----------------------------------------------------------------------------

  ClientAPI.InterfaceAPI.DGrid.SetColCount := TDGrid_SetColCount; //列数
  ClientAPI.InterfaceAPI.DGrid.SetRowCount := TDGrid_SetRowCount; //组数
  ClientAPI.InterfaceAPI.DGrid.SetColWidth := TDGrid_SetColWidth; //列宽
  ClientAPI.InterfaceAPI.DGrid.SetRowHeight := TDGrid_SetRowHeight; //组高

  ClientAPI.InterfaceAPI.DGrid.SetOnGridSelect := TDGrid_SetOnGridSelect; //选择事件
  ClientAPI.InterfaceAPI.DGrid.SetOnGridMouseMove := TDGrid_SetOnGridMouseMove; //鼠标移动事件
  ClientAPI.InterfaceAPI.DGrid.SetOnGridPaint := TDGrid_SetOnGridPaint; //绘制事件
  ClientAPI.InterfaceAPI.DGrid.DrawGridItem := TDGrid_DrawGridItem; //绘制网格物品
  //-----------------------------------------------------------------------------
    //ClientAPI.InterfaceAPI.DComboBox.ShowButton := TDComboBox_ShowButton; //是否显示按钮
  ClientAPI.InterfaceAPI.DComboBox.ItemIndex := TDComboBox_ItemIndex; //当前选择行
  //ClientAPI.InterfaceAPI.DComboBox.ButtonColor := TDComboBox_ButtonColor; //按钮颜色
  ClientAPI.InterfaceAPI.DComboBox.Items := TDComboBox_Items; //列表
  ClientAPI.InterfaceAPI.DComboBox.Text := TDComboBox_Text;
  ClientAPI.InterfaceAPI.DComboBox.OnSelect := TDComboBox_OnSelect;

  {ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontFColor := TDComboBox_DefaultTextFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontBColor := TDComboBox_DefaultTextFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontStyle := TDComboBox_DefaultTextFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontSize := TDComboBox_DefaultTextFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontBold := TDComboBox_DefaultTextFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DComboBox.DefaultTextFontName := TDComboBox_DefaultTextFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontFColor := TDComboBox_MouseMoveTextFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontBColor := TDComboBox_MouseMoveTextFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontStyle := TDComboBox_MouseMoveTextFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontSize := TDComboBox_MouseMoveTextFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontBold := TDComboBox_MouseMoveTextFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DComboBox.MouseMoveTextFontName := TDComboBox_MouseMoveTextFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontFColor := TDComboBox_MouseDownTextFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontBColor := TDComboBox_MouseDownTextFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontStyle := TDComboBox_MouseDownTextFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontSize := TDComboBox_MouseDownTextFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontBold := TDComboBox_MouseDownTextFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DComboBox.MouseDownTextFontName := TDComboBox_MouseDownTextFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontFColor := TDComboBox_DisabledTextFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontBColor := TDComboBox_DisabledTextFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontStyle := TDComboBox_DisabledTextFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontSize := TDComboBox_DisabledTextFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontBold := TDComboBox_DisabledTextFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DComboBox.DisabledTextFontName := TDComboBox_DisabledTextFontName; //不可用时字体名称 }

//-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DComboBox.SetShowButton := TDComboBox_SetShowButton; //是否显示按钮
  ClientAPI.InterfaceAPI.DComboBox.SetItemIndex := TDComboBox_SetItemIndex; //当前选择行
  ClientAPI.InterfaceAPI.DComboBox.SetButtonColor := TDComboBox_SetButtonColor; //按钮颜色
  ClientAPI.InterfaceAPI.DComboBox.SetItems := TDComboBox_SetItems; //列表
  ClientAPI.InterfaceAPI.DComboBox.SetText := TDComboBox_SetText;
  ClientAPI.InterfaceAPI.DComboBox.SetOnSelect := TDComboBox_SetOnSelect;

  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontFColor := TDComboBox_SetDefaultTextFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontBColor := TDComboBox_SetDefaultTextFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontStyle := TDComboBox_SetDefaultTextFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontSize := TDComboBox_SetDefaultTextFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontBold := TDComboBox_SetDefaultTextFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DComboBox.SetDefaultTextFontName := TDComboBox_SetDefaultTextFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontFColor := TDComboBox_SetMouseMoveTextFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontBColor := TDComboBox_SetMouseMoveTextFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontStyle := TDComboBox_SetMouseMoveTextFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontSize := TDComboBox_SetMouseMoveTextFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontBold := TDComboBox_SetMouseMoveTextFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DComboBox.SetMouseMoveTextFontName := TDComboBox_SetMouseMoveTextFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontFColor := TDComboBox_SetMouseDownTextFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontBColor := TDComboBox_SetMouseDownTextFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontStyle := TDComboBox_SetMouseDownTextFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontSize := TDComboBox_SetMouseDownTextFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontBold := TDComboBox_SetMouseDownTextFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DComboBox.SetMouseDownTextFontName := TDComboBox_SetMouseDownTextFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontFColor := TDComboBox_SetDisabledTextFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontBColor := TDComboBox_SetDisabledTextFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontStyle := TDComboBox_SetDisabledTextFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontSize := TDComboBox_SetDisabledTextFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontBold := TDComboBox_SetDisabledTextFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DComboBox.SetDisabledTextFontName := TDComboBox_SetDisabledTextFontName; //不可用时字体名称
  //-----------------------------------------------------------------------------
    //ClientAPI.InterfaceAPI.DPopupMenu.SelectColor := TDPopupMenu_SelectColor; //选择颜色
  ClientAPI.InterfaceAPI.DPopupMenu.ItemIndex := TDPopupMenu_ItemIndex; //选择行
  ClientAPI.InterfaceAPI.DPopupMenu.ItemHeight := TDPopupMenu_ItemHeight; //组高
  ClientAPI.InterfaceAPI.DPopupMenu.Items := TDPopupMenu_Items;
  //ClientAPI.InterfaceAPI.DPopupMenu.Alpha := TDPopupMenu_Alpha; //背景透明度

  {ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontFColor := TDPopupMenu_DefaultItemFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontBColor := TDPopupMenu_DefaultItemFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontStyle := TDPopupMenu_DefaultItemFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontSize := TDPopupMenu_DefaultItemFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontBold := TDPopupMenu_DefaultItemFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.DefaultItemFontName := TDPopupMenu_DefaultItemFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontFColor := TDPopupMenu_MouseMoveItemFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontBColor := TDPopupMenu_MouseMoveItemFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontStyle := TDPopupMenu_MouseMoveItemFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontSize := TDPopupMenu_MouseMoveItemFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontBold := TDPopupMenu_MouseMoveItemFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.MouseMoveItemFontName := TDPopupMenu_MouseMoveItemFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontFColor := TDPopupMenu_MouseDownItemFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontBColor := TDPopupMenu_MouseDownItemFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontStyle := TDPopupMenu_MouseDownItemFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontSize := TDPopupMenu_MouseDownItemFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontBold := TDPopupMenu_MouseDownItemFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.MouseDownItemFontName := TDPopupMenu_MouseDownItemFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontFColor := TDPopupMenu_DisabledItemFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontBColor := TDPopupMenu_DisabledItemFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontStyle := TDPopupMenu_DisabledItemFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontSize := TDPopupMenu_DisabledItemFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontBold := TDPopupMenu_DisabledItemFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.DisabledItemFontName := TDPopupMenu_DisabledItemFontName; //不可用时字体名称}

//-----------------------------------------------------------------------------
  ClientAPI.InterfaceAPI.DPopupMenu.SetSelectColor := TDPopupMenu_SetSelectColor; //选择颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetItemIndex := TDPopupMenu_SetItemIndex; //选择行
  ClientAPI.InterfaceAPI.DPopupMenu.SetItemHeight := TDPopupMenu_SetItemHeight; //组高
  ClientAPI.InterfaceAPI.DPopupMenu.SetAlpha := TDPopupMenu_SetAlpha; //背景透明度

  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontFColor := TDPopupMenu_SetDefaultItemFontFColor; //默认标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontBColor := TDPopupMenu_SetDefaultItemFontBColor; //默认标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontStyle := TDPopupMenu_SetDefaultItemFontStyle; //默认字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontSize := TDPopupMenu_SetDefaultItemFontSize; //默认字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontBold := TDPopupMenu_SetDefaultItemFontBold; //默认是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.SetDefaultItemFontName := TDPopupMenu_SetDefaultItemFontName; //默认字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontFColor := TDPopupMenu_SetMouseMoveItemFontFColor; //鼠标移动标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontBColor := TDPopupMenu_SetMouseMoveItemFontBColor; //鼠标移动标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontStyle := TDPopupMenu_SetMouseMoveItemFontStyle; //鼠标移动字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontSize := TDPopupMenu_SetMouseMoveItemFontSize; //鼠标移动字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontBold := TDPopupMenu_SetMouseMoveItemFontBold; //鼠标移动是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseMoveItemFontName := TDPopupMenu_SetMouseMoveItemFontName; //鼠标移动字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontFColor := TDPopupMenu_SetMouseDownItemFontFColor; //鼠标按下标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontBColor := TDPopupMenu_SetMouseDownItemFontBColor; //鼠标按下标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontStyle := TDPopupMenu_SetMouseDownItemFontStyle; //鼠标按下字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontSize := TDPopupMenu_SetMouseDownItemFontSize; //鼠标按下字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontBold := TDPopupMenu_SetMouseDownItemFontBold; //鼠标按下是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.SetMouseDownItemFontName := TDPopupMenu_SetMouseDownItemFontName; //鼠标按下字体名称

  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontFColor := TDPopupMenu_SetDisabledItemFontFColor; //不可用时标题字体颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontBColor := TDPopupMenu_SetDisabledItemFontBColor; //不可用时标题字体描边颜色
  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontStyle := TDPopupMenu_SetDisabledItemFontStyle; //不可用时字体样式
  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontSize := TDPopupMenu_SetDisabledItemFontSize; //不可用时字体尺寸
  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontBold := TDPopupMenu_SetDisabledItemFontBold; //不可用时是否描边
  ClientAPI.InterfaceAPI.DPopupMenu.SetDisabledItemFontName := TDPopupMenu_SetDisabledItemFontName; //不可用时字体名称

  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------
  ClientAPI.ActorAPI.m_wAppearance := TActorAPI_m_wAppearance;
  ClientAPI.ActorAPI.m_nRecogId := TActorAPI_m_nRecogId; //角色标识
  ClientAPI.ActorAPI.m_nCurrX := TActorAPI_m_nCurrX; //当前所在地图座标X
  ClientAPI.ActorAPI.m_nCurrY := TActorAPI_m_nCurrY; //当前所在地图座标Y
  ClientAPI.ActorAPI.m_btDir := TActorAPI_m_btDir; //当前站立方向
  ClientAPI.ActorAPI.m_btSex := TActorAPI_m_btSex; //性别
  ClientAPI.ActorAPI.m_btRace := TActorAPI_m_btRace; //怪物DB库的RaceImg
  ClientAPI.ActorAPI.m_btHair := TActorAPI_m_btHair; //头发类型
  ClientAPI.ActorAPI.m_wDress := TActorAPI_m_wDress; //衣服类型
  ClientAPI.ActorAPI.m_wWeapon := TActorAPI_m_wWeapon; //武器类型
  ClientAPI.ActorAPI.m_btJob := TActorAPI_m_btJob; //职业 0:武士  1:法师  2:道士
  ClientAPI.ActorAPI.m_btCaseltGuild := TActorAPI_m_btCaseltGuild; //1=沙行会成员 //2=沙行会掌门
  ClientAPI.ActorAPI.m_sDescUserName := TActorAPI_m_sDescUserName; //人物封号
  ClientAPI.ActorAPI.m_sUserName := TActorAPI_m_sUserName; //名称
  ClientAPI.ActorAPI.m_nNameColor := TActorAPI_m_nNameColor; //名称颜色
  ClientAPI.ActorAPI.m_Abil := TActorAPI_m_Abil; //属性
  ClientAPI.ActorAPI.m_boOpenShop := TActorAPI_m_boOpenShop;
  ClientAPI.ActorAPI.m_nSayX := TActorAPI_m_nSayX;
  ClientAPI.ActorAPI.m_nSayY := TActorAPI_m_nSayY;
  ClientAPI.ActorAPI.m_nShiftX := TActorAPI_m_nShiftX;
  ClientAPI.ActorAPI.m_nShiftY := TActorAPI_m_nShiftY;
  ClientAPI.ActorAPI.m_nTargetX := TActorAPI_m_nTargetX;
  ClientAPI.ActorAPI.m_nTargetY := TActorAPI_m_nTargetY;
  ClientAPI.ActorAPI.m_nTargetRecog := TActorAPI_m_nTargetRecog;
  ClientAPI.ActorAPI.m_boCobweb := TActorAPI_m_boCobweb; //网罩住了
  ClientAPI.ActorAPI.m_boCanDraw := TActorAPI_m_boCanDraw; //该角色是否可以绘制
  ClientAPI.ActorAPI.m_nBagCount := TActorAPI_m_nBagCount; //包裹最大数
  ClientAPI.ActorAPI.m_btColor := TActorAPI_m_btColor;
  ClientAPI.ActorAPI.m_nState := TActorAPI_m_nState;
  //------------------------------------------------------------------------------
  ClientAPI.ActorAPI.m_nBodyOffset := TActorAPI_m_nBodyOffset;
  ClientAPI.ActorAPI.m_boUseMagic := TActorAPI_m_boUseMagic;
  ClientAPI.ActorAPI.m_nCurrentFrame := TActorAPI_m_nCurrentFrame;
  ClientAPI.ActorAPI.m_nStartFrame := TActorAPI_m_nStartFrame;
  ClientAPI.ActorAPI.m_nEndFrame := TActorAPI_m_nEndFrame;
  ClientAPI.ActorAPI.m_dwFrameTime := TActorAPI_m_dwFrameTime;
  ClientAPI.ActorAPI.m_dwStartTime := TActorAPI_m_dwStartTime;

  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------
  //-----------------------------------------------------------------------------

  ClientAPI.HookAPI.GetHookInitialize := THookAPI_GetHookInitialize;
  ClientAPI.HookAPI.GetHookFinalize := THookAPI_GetHookFinalize;
  ClientAPI.HookAPI.GetHookFormKeyDown := THookAPI_GetHookFormKeyDown;
  ClientAPI.HookAPI.GetHookFormKeyPress := THookAPI_GetHookFormKeyPress;
  ClientAPI.HookAPI.GetHookFormMouseDown := THookAPI_GetHookFormMouseDown;
  ClientAPI.HookAPI.GetHookFormMouseMove := THookAPI_GetHookFormMouseMove;
  ClientAPI.HookAPI.GetHookDecodeMessagePacketStart := THookAPI_GetHookDecodeMessagePacketStart;
  ClientAPI.HookAPI.GetHookDecodeMessagePacketStop := THookAPI_GetHookDecodeMessagePacketStop;
  ClientAPI.HookAPI.GetHookDecodeMessagePacket := THookAPI_GetHookDecodeMessagePacket;

  ClientAPI.HookAPI.GetHookDrawScene1 := THookAPI_GetHookDrawScene1;
  ClientAPI.HookAPI.GetHookDrawScene2 := THookAPI_GetHookDrawScene2;
  ClientAPI.HookAPI.GetHookDrawScene3 := THookAPI_GetHookDrawScene3;
  ClientAPI.HookAPI.GetHookDrawScene4 := THookAPI_GetHookDrawScene4;

  ClientAPI.HookAPI.GetHookTActor_FeatureChanged := THookAPI_GetHookTActor_FeatureChanged;
  ClientAPI.HookAPI.GetHookTActor_CalcActorFrame := THookAPI_GetHookTActor_CalcActorFrame;
  ClientAPI.HookAPI.GetHookTActor_DrawChr1 := THookAPI_GetHookTActor_DrawChr1;
  ClientAPI.HookAPI.GetHookTActor_DrawChr2 := THookAPI_GetHookTActor_DrawChr2;

  ClientAPI.HookAPI.GetHookTHumActor_CalcActorFrame := THookAPI_GetHookTHumActor_CalcActorFrame;
  ClientAPI.HookAPI.GetHookTHumActor_DrawChr1 := THookAPI_GetHookTHumActor_DrawChr1;
  ClientAPI.HookAPI.GetHookTHumActor_DrawChr2 := THookAPI_GetHookTHumActor_DrawChr2;
  ClientAPI.HookAPI.GetHookTHumActor_DrawChr3 := THookAPI_GetHookTHumActor_DrawChr3;
  ClientAPI.HookAPI.GetHookTHumActor_DrawChr4 := THookAPI_GetHookTHumActor_DrawChr4;
  //------------------------------------------------------------------------------
  ClientAPI.HookAPI.SetHookInitialize := THookAPI_SetHookInitialize;
  ClientAPI.HookAPI.SetHookFinalize := THookAPI_SetHookFinalize;
  ClientAPI.HookAPI.SetHookFormKeyDown := THookAPI_SetHookFormKeyDown;
  ClientAPI.HookAPI.SetHookFormKeyPress := THookAPI_SetHookFormKeyPress;
  ClientAPI.HookAPI.SetHookFormMouseDown := THookAPI_SetHookFormMouseDown;
  ClientAPI.HookAPI.SetHookFormMouseMove := THookAPI_SetHookFormMouseMove;
  ClientAPI.HookAPI.SetHookDecodeMessagePacketStart := THookAPI_SetHookDecodeMessagePacketStart;
  ClientAPI.HookAPI.SetHookDecodeMessagePacketStop := THookAPI_SetHookDecodeMessagePacketStop;
  ClientAPI.HookAPI.SetHookDecodeMessagePacket := THookAPI_SetHookDecodeMessagePacket;

  ClientAPI.HookAPI.SetHookDrawScene1 := THookAPI_SetHookDrawScene1;
  ClientAPI.HookAPI.SetHookDrawScene2 := THookAPI_SetHookDrawScene2;
  ClientAPI.HookAPI.SetHookDrawScene3 := THookAPI_SetHookDrawScene3;
  ClientAPI.HookAPI.SetHookDrawScene4 := THookAPI_SetHookDrawScene4;

  ClientAPI.HookAPI.SetHookTActor_FeatureChanged := THookAPI_SetHookTActor_FeatureChanged;
  ClientAPI.HookAPI.SetHookTActor_CalcActorFrame := THookAPI_SetHookTActor_CalcActorFrame;
  ClientAPI.HookAPI.SetHookTActor_DrawChr1 := THookAPI_SetHookTActor_DrawChr1;
  ClientAPI.HookAPI.SetHookTActor_DrawChr2 := THookAPI_SetHookTActor_DrawChr2;

  ClientAPI.HookAPI.SetHookTHumActor_CalcActorFrame := THookAPI_SetHookTHumActor_CalcActorFrame;
  ClientAPI.HookAPI.SetHookTHumActor_DrawChr1 := THookAPI_SetHookTHumActor_DrawChr1;
  ClientAPI.HookAPI.SetHookTHumActor_DrawChr2 := THookAPI_SetHookTHumActor_DrawChr2;
  ClientAPI.HookAPI.SetHookTHumActor_DrawChr3 := THookAPI_SetHookTHumActor_DrawChr3;
  ClientAPI.HookAPI.SetHookTHumActor_DrawChr4 := THookAPI_SetHookTHumActor_DrawChr4;

  ClientAPI.PointDropItemList.Count := TPointDropItemListAPI_Count;
  ClientAPI.PointDropItemList.Get := TPointDropItemListAPI_Get;
  ClientAPI.PointDropItemList.X := TPointDropItemListAPI_X;
  ClientAPI.PointDropItemList.Y := TPointDropItemListAPI_Y;

  ClientAPI.DropItemsMgr.Lock := TDropItemsMgrAPI_Lock;
  ClientAPI.DropItemsMgr.UnLock := TDropItemsMgrAPI_UnLock;
  ClientAPI.DropItemsMgr.Count := TDropItemsMgrAPI_Count;
  ClientAPI.DropItemsMgr.Get := TDropItemsMgrAPI_Get;
  ClientAPI.DropItemsMgr.GetItemByID := TDropItemsMgrAPI_GetItemByID;
  ClientAPI.DropItemsMgr.GetItemListByPoint := TDropItemsMgrAPI_GetItemListByPoint;
  ClientAPI.DropItemsMgr.GetItemListIndexByY := TDropItemsMgrAPI_GetItemListIndexByY;
end;

constructor TPlugInManage.Create();
begin
  inherited;
  PlugList := TStringList.Create;
  PlugNameList := TStringList.Create;
end;

destructor TPlugInManage.Destroy;
begin
  UnLoadPlugIn();
  PlugList.Free;
  PlugNameList.Free;
  inherited;
end;

procedure TPlugInManage.LoadPlugIn();
var
  I:Integer;
  sPlugLibName:string;
  sPlugLibFileName:string;
  Init:TPlugInit;
  PlugInfo:pTPlugInfo;
  PLUGAPISIZE:Integer;
begin
  for I := 0 to PlugNameList.Count - 1 do begin
    sPlugLibName := Trim(PlugNameList.Strings[I]);
    if (sPlugLibName = '') or (sPlugLibName[1] = ';') then Continue;
    sPlugLibFileName := g_sSelfFilePath + 'PlugIn\' + sPlugLibName;
    if FileExists(sPlugLibFileName) then begin
      New(PlugInfo);
      PlugInfo.Module := 0;
      PlugInfo.MemoryStream := TMemoryStream.Create;
      try
        PlugInfo.MemoryStream.LoadFromFile(sPlugLibFileName);
        PlugInfo.MemoryStream.Position := 0;
      except
        PlugInfo.MemoryStream.Clear;
      end;

      if PlugInfo.MemoryStream.Size <= 0 then begin
        PlugInfo.MemoryStream.Free;
        Dispose(PlugInfo);
        DebugOutStr('[Exception] LoadPlugIn 1 ' + sPlugLibFileName);
        Continue;
      end;

      PlugInfo.Module := LoadLibrary(PChar(sPlugLibFileName)); //FreeLibrary
      if PlugInfo.Module <= 32 then begin
        DebugOutStr('[Exception] LoadPlugIn 2 ' + sPlugLibFileName);
        PlugInfo.MemoryStream.Free;
        Dispose(PlugInfo);
        Continue;
      end;
      Init := GetProcAddress(PlugInfo.Module, 'CInit');
      if @Init = nil then begin
        DebugOutStr('[Exception] LoadPlugIn 3 ' + sPlugLibFileName);
        FreeLibrary(PlugInfo.Module);
        PlugInfo.MemoryStream.Free;
        Dispose(PlugInfo);
        Continue;
      end;

      PLUGAPISIZE := Init(@ClientAPI, SizeOf(TClientAPI));
      if PLUGAPISIZE <> SizeOf(TClientAPI) then begin
        DebugOutStr('[Exception] LoadPlugIn 4 ' + sPlugLibFileName + ' PLUGAPISIZE:' + IntToStr(PLUGAPISIZE) + ' SizeOf(TClientAPI):' + IntToStr(SizeOf(TClientAPI)));

        FreeLibrary(PlugInfo.Module);

        PlugInfo.MemoryStream.Free;
        Dispose(PlugInfo);
        Continue;
      end;
      PlugInfo.MD5 := RivestFile(sPlugLibFileName);
      PlugList.AddObject(sPlugLibFileName, TObject(PlugInfo));
    end;
  end;
  // LoadList.Free;
 //end;
end;

procedure TPlugInManage.UnLoadPlugIn();
var
  I:Integer;
  PFunc:procedure(); stdcall;
  PlugInfo:pTPlugInfo;
begin
  for I := 0 to PlugList.Count - 1 do begin
    PlugInfo := pTPlugInfo(PlugList.Objects[I]);
    PFunc := GetProcAddress(PlugInfo.Module, 'CUnInit');
    if @PFunc <> nil then PFunc();
    PlugInfo.MemoryStream.Free;
    FreeLibrary(PlugInfo.Module);
    Dispose(PlugInfo);
  end;
  PlugList.Clear;
end;

initialization
  InitializeAPI;

  {$IFEND}

end.
