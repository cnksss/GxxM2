// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSpnEdt.pas' rev: 35.00 (Windows)

#ifndef RzspnedtHPP
#define RzspnedtHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <RzButton.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzspnedt
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzRapidFireButton;
class DELPHICLASS TRzSpinButtons;
class DELPHICLASS TRzSpinEdit;
class DELPHICLASS TRzSpinner;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzRapidFireStyle : unsigned char { rfFocusRect, rfAllowTimer };

typedef System::Set<TRzRapidFireStyle, TRzRapidFireStyle::rfFocusRect, TRzRapidFireStyle::rfAllowTimer> TRzRapidFireStyles;

class PASCALIMPLEMENTATION TRzRapidFireButton : public Vcl::Buttons::TSpeedButton
{
	typedef Vcl::Buttons::TSpeedButton inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	System::Word FInitialDelay;
	System::Word FDelay;
	TRzRapidFireStyles FStyle;
	Rzcommon::TRzScrollStyle FScrollStyle;
	void __fastcall TimerExpired(System::TObject* Sender);
	
protected:
	virtual void __fastcall Paint();
	void __fastcall AssignArrowGlyph();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetScrollStyle(Rzcommon::TRzScrollStyle Value);
	
public:
	__fastcall virtual TRzRapidFireButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzRapidFireButton();
	__property TRzRapidFireStyles Style = {read=FStyle, write=FStyle, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Word Delay = {read=FDelay, write=FDelay, default=100};
	__property System::Word InitialDelay = {read=FInitialDelay, write=FInitialDelay, default=400};
	__property Rzcommon::TRzScrollStyle ScrollStyle = {read=FScrollStyle, write=SetScrollStyle, default=0};
};


enum DECLSPEC_DENUM TSpinDirection : unsigned char { sdUpDown, sdLeftRight };

enum DECLSPEC_DENUM TSpinButtonType : unsigned char { sbUp, sbDown };

typedef void __fastcall (__closure *TSpinChangingEvent)(System::TObject* Sender, System::Extended NewValue, bool &AllowChange);

typedef void __fastcall (__closure *TSpinButtonEvent)(System::TObject* Sender, TSpinButtonType Button);

class PASCALIMPLEMENTATION TRzSpinButtons : public Vcl::Controls::TWinControl
{
	typedef Vcl::Controls::TWinControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	TSpinDirection FDirection;
	Rzcommon::TOrientation FOrientation;
	Rzbutton::TRzControlButton* FUpRightButton;
	Rzbutton::TRzControlButton* FDownLeftButton;
	Rzbutton::TRzControlButton* FFocusedButton;
	Vcl::Controls::TWinControl* FFocusControl;
	bool FFlat;
	System::Classes::TNotifyEvent FOnUpRightClick;
	System::Classes::TNotifyEvent FOnDownLeftClick;
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	bool FCustomUpRightGlyph;
	bool FCustomDownLeftGlyph;
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual Rzbutton::TRzControlButton* __fastcall CreateButton();
	virtual void __fastcall BtnClickHandler(System::TObject* Sender);
	virtual void __fastcall BtnMouseDownHandler(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall BtnGlyphChangeHandler(System::TObject* Sender);
	virtual void __fastcall AdjustDimensions(int &W, int &H);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall UpRightClick();
	DYNAMIC void __fastcall DownLeftClick();
	virtual void __fastcall SetDirection(TSpinDirection Value);
	virtual bool __fastcall GetAllEnabled();
	virtual void __fastcall SetAllEnabled(bool Value);
	virtual System::Uitypes::TColor __fastcall GetColor();
	HIDESBASE virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlat(bool Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	System::Word __fastcall GetInitialDelay();
	void __fastcall SetInitialDelay(System::Word Value);
	System::Word __fastcall GetDelay();
	void __fastcall SetDelay(System::Word Value);
	virtual Vcl::Graphics::TBitmap* __fastcall GetUpRightGlyph();
	virtual void __fastcall SetUpRightGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Graphics::TBitmap* __fastcall GetDownLeftGlyph();
	virtual void __fastcall SetDownLeftGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetUpRightNumGlyphs();
	virtual void __fastcall SetUpRightNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetDownLeftNumGlyphs();
	virtual void __fastcall SetDownLeftNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	
public:
	__fastcall virtual TRzSpinButtons(System::Classes::TComponent* AOwner);
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	__property Rzbutton::TRzControlButton* UpRightButton = {read=FUpRightButton};
	__property Rzbutton::TRzControlButton* DownLeftButton = {read=FDownLeftButton};
	__property bool CustomUpRightGlyph = {read=FCustomUpRightGlyph, nodefault};
	__property bool CustomDownLeftGlyph = {read=FCustomDownLeftGlyph, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor Color = {read=GetColor, write=SetColor, default=-16777201};
	__property System::Word Delay = {read=GetDelay, write=SetDelay, default=100};
	__property TSpinDirection Direction = {read=FDirection, write=SetDirection, default=0};
	__property bool Enabled = {read=GetAllEnabled, write=SetAllEnabled, default=1};
	__property bool Flat = {read=FFlat, write=SetFlat, default=0};
	__property Vcl::Controls::TWinControl* FocusControl = {read=FFocusControl, write=FFocusControl};
	__property Vcl::Graphics::TBitmap* GlyphUpRight = {read=GetUpRightGlyph, write=SetUpRightGlyph, stored=FCustomUpRightGlyph};
	__property Vcl::Graphics::TBitmap* GlyphDownLeft = {read=GetDownLeftGlyph, write=SetDownLeftGlyph, stored=FCustomDownLeftGlyph};
	__property System::Word InitialDelay = {read=GetInitialDelay, write=SetInitialDelay, default=400};
	__property Vcl::Buttons::TNumGlyphs NumGlyphsUpRight = {read=GetUpRightNumGlyphs, write=SetUpRightNumGlyphs, stored=FCustomUpRightGlyph, default=2};
	__property Vcl::Buttons::TNumGlyphs NumGlyphsDownLeft = {read=GetDownLeftNumGlyphs, write=SetDownLeftNumGlyphs, stored=FCustomDownLeftGlyph, default=2};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=1};
	__property System::Classes::TNotifyEvent OnDownLeftClick = {read=FOnDownLeftClick, write=FOnDownLeftClick};
	__property System::Classes::TNotifyEvent OnUpRightClick = {read=FOnUpRightClick, write=FOnUpRightClick};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property ParentColor = {default=1};
	__property ParentCtl3D = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Visible = {default=1};
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSpinButtons(HWND ParentWindow) : Vcl::Controls::TWinControl(ParentWindow) { }
	/* TWinControl.Destroy */ inline __fastcall virtual ~TRzSpinButtons() { }
	
};


class PASCALIMPLEMENTATION TRzSpinEdit : public Rzedit::TRzCustomEdit
{
	typedef Rzedit::TRzCustomEdit inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAllowKeyEdit;
	bool FAllowBlank;
	System::Extended FBlankValue;
	TRzSpinButtons* FButtons;
	int FButtonWidth;
	bool FCheckRange;
	System::Byte FDecimals;
	System::Extended FIncrement;
	bool FIntegersOnly;
	System::Extended FMin;
	System::Extended FMax;
	System::Extended FPageSize;
	System::Uitypes::TColor FFlatButtonColor;
	TSpinChangingEvent FOnChanging;
	TSpinButtonEvent FOnButtonClick;
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaste(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMCut(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	bool __fastcall IsCustomDownGlyph();
	bool __fastcall IsCustomUpGlyph();
	virtual void __fastcall ReadOnlyChanged();
	virtual void __fastcall ResizeButtons();
	virtual System::Types::TRect __fastcall GetEditRect();
	virtual void __fastcall SetEditRect();
	virtual bool __fastcall IsValidChar(System::WideChar Key);
	virtual void __fastcall UpRightClickHandler(System::TObject* Sender);
	virtual void __fastcall DownLeftClickHandler(System::TObject* Sender);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC bool __fastcall CanChange(System::Extended NewValue);
	DYNAMIC void __fastcall DoButtonClick(TSpinButtonType S);
	virtual void __fastcall IncValue(const System::Extended Amount);
	virtual void __fastcall DecValue(const System::Extended Amount);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetButtonWidth(int Value);
	virtual void __fastcall SetDecimals(System::Byte Value);
	virtual void __fastcall SetIntegersOnly(bool Value);
	virtual Rzbutton::TRzControlButton* __fastcall GetButton(int Index);
	virtual Vcl::Graphics::TBitmap* __fastcall GetButtonUpGlyph();
	virtual void __fastcall SetButtonUpGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetButtonUpNumGlyphs();
	virtual void __fastcall SetButtonUpNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual Vcl::Graphics::TBitmap* __fastcall GetButtonDownGlyph();
	virtual void __fastcall SetButtonDownGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetButtonDownNumGlyphs();
	virtual void __fastcall SetButtonDownNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual TSpinDirection __fastcall GetDirection();
	virtual void __fastcall SetDirection(TSpinDirection Value);
	virtual void __fastcall SetFlatButtons(bool Value);
	virtual Rzcommon::TOrientation __fastcall GetOrientation();
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetCheckRange(bool Value);
	virtual void __fastcall SetMin(const System::Extended Value);
	virtual void __fastcall SetMax(const System::Extended Value);
	virtual __int64 __fastcall GetIntValue();
	virtual void __fastcall SetIntValue(__int64 Value);
	virtual System::Extended __fastcall GetValue();
	virtual System::Extended __fastcall CheckValue(const System::Extended Value);
	virtual void __fastcall SetValue(const System::Extended Value);
	bool __fastcall StoreIncrement();
	bool __fastcall StorePageSize();
	
public:
	__fastcall virtual TRzSpinEdit(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	__property TRzSpinButtons* Buttons = {read=FButtons};
	__property Rzbutton::TRzControlButton* DownLeftButton = {read=GetButton, index=1};
	__property Rzbutton::TRzControlButton* UpRightButton = {read=GetButton, index=2};
	__property __int64 IntValue = {read=GetIntValue, write=SetIntValue};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowBlank = {read=FAllowBlank, write=FAllowBlank, default=0};
	__property System::Extended BlankValue = {read=FBlankValue, write=FBlankValue};
	__property bool AllowKeyEdit = {read=FAllowKeyEdit, write=FAllowKeyEdit, default=0};
	__property Vcl::Graphics::TBitmap* ButtonDownGlyph = {read=GetButtonDownGlyph, write=SetButtonDownGlyph, stored=IsCustomDownGlyph};
	__property Vcl::Buttons::TNumGlyphs ButtonDownNumGlyphs = {read=GetButtonDownNumGlyphs, write=SetButtonDownNumGlyphs, stored=IsCustomDownGlyph, nodefault};
	__property Vcl::Graphics::TBitmap* ButtonUpGlyph = {read=GetButtonUpGlyph, write=SetButtonUpGlyph, stored=IsCustomUpGlyph};
	__property Vcl::Buttons::TNumGlyphs ButtonUpNumGlyphs = {read=GetButtonUpNumGlyphs, write=SetButtonUpNumGlyphs, stored=IsCustomUpGlyph, nodefault};
	__property int ButtonWidth = {read=FButtonWidth, write=SetButtonWidth, default=17};
	__property bool CheckRange = {read=FCheckRange, write=SetCheckRange, default=0};
	__property System::Byte Decimals = {read=FDecimals, write=SetDecimals, default=0};
	__property TSpinDirection Direction = {read=GetDirection, write=SetDirection, default=0};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=FFlatButtonColor, default=-16777201};
	__property System::Extended Increment = {read=FIncrement, write=FIncrement, stored=StoreIncrement};
	__property bool IntegersOnly = {read=FIntegersOnly, write=SetIntegersOnly, default=1};
	__property System::Extended Max = {read=FMax, write=SetMax, stored=true};
	__property System::Extended Min = {read=FMin, write=SetMin, stored=true};
	__property Rzcommon::TOrientation Orientation = {read=GetOrientation, write=SetOrientation, default=1};
	__property System::Extended PageSize = {read=FPageSize, write=FPageSize, stored=StorePageSize};
	__property System::Extended Value = {read=GetValue, write=SetValue};
	__property TSpinChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property TSpinButtonEvent OnButtonClick = {read=FOnButtonClick, write=FOnButtonClick};
	__property Align = {default=0};
	__property Alignment = {default=1};
	__property Anchors = {default=3};
	__property AutoSelect = {default=1};
	__property AutoSize = {default=1};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatButtons = {default=0};
	__property Font;
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FrameHotColor = {default=-16777200};
	__property FrameHotTrack = {default=0};
	__property FrameHotStyle = {default=10};
	__property FrameSides = {default=15};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property HideSelection = {default=1};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property OEMConvert = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PasswordChar = {default=0};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TextHint = {default=0};
	__property TextHintVisibleOnFocus = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzSpinEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSpinEdit(HWND ParentWindow) : Rzedit::TRzCustomEdit(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzSpinnerArea : unsigned char { saValue, saMinusButton, saPlusButton };

typedef void __fastcall (__closure *TRzSpinnerEvent)(System::TObject* Sender, int NewValue, bool &AllowChange);

typedef void __fastcall (__closure *TRzSpinnerIncDecEvent)(System::TObject* Sender, int Amount);

class PASCALIMPLEMENTATION TRzSpinner : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	int FValue;
	int FMin;
	int FMax;
	int FIncrement;
	int FPageSize;
	bool FCheckRange;
	bool FTabOnEnter;
	bool FShowHexValue;
	System::UnicodeString FHexValuePrefix;
	System::Uitypes::TColor FFrameColor;
	System::Uitypes::TColor FButtonColor;
	int FButtonWidth;
	bool FShowFocusRect;
	bool FMinusBtnDown;
	bool FPlusBtnDown;
	TRzSpinnerArea FLastHotTrackArea;
	bool FOverPlusButton;
	bool FOverMinusButton;
	bool FThemeAware;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	System::Word FInitialDelay;
	System::Word FDelay;
	Vcl::Graphics::TBitmap* FGlyphBitmap;
	Vcl::Imglist::TCustomImageList* FImages;
	System::StaticArray<System::Uitypes::TImageIndex, 2> FImageIndexes;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Classes::TNotifyEvent FOnChange;
	TRzSpinnerEvent FOnChanging;
	TRzSpinnerIncDecEvent FOnIncrement;
	TRzSpinnerIncDecEvent FOnDecrement;
	void __fastcall TimerExpired(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Paint();
	virtual void __fastcall DrawFrame();
	virtual void __fastcall DrawValue();
	virtual void __fastcall DrawButton(TRzSpinnerArea Area, bool Down, const System::Types::TRect &Bounds);
	bool __fastcall UseCustomImages(TRzSpinnerArea Area);
	void __fastcall CalcCenterOffsets(const System::Types::TRect &Bounds, int &L, int &T);
	void __fastcall CheckMinSize();
	void __fastcall CheckHotTracking(const System::Types::TPoint &P);
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall DecValue(int Amount);
	virtual void __fastcall IncValue(int Amount);
	bool __fastcall ShowFocus();
	System::Types::TPoint __fastcall CursorPosition();
	TRzSpinnerArea __fastcall HitTest(const System::Types::TPoint &P)/* overload */;
	TRzSpinnerArea __fastcall HitTest(int X, int Y)/* overload */;
	DYNAMIC void __fastcall Change();
	DYNAMIC bool __fastcall CanChange(int NewValue);
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonWidth(int Value);
	virtual void __fastcall SetCheckRange(bool Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual System::Uitypes::TImageIndex __fastcall GetImageIndex(int PropIndex);
	virtual void __fastcall SetImageIndex(int PropIndex, System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetMax(int Value);
	virtual void __fastcall SetMin(int Value);
	virtual int __fastcall CheckValue(int Value);
	virtual void __fastcall SetValue(int Value);
	virtual void __fastcall SetShowHexValue(bool Value);
	virtual void __fastcall SetHexValuePrefix(const System::UnicodeString Value);
	bool __fastcall StoreHexValuePrefix();
	virtual void __fastcall SetThemeAware(bool Value);
	
public:
	__fastcall virtual TRzSpinner(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzSpinner();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=SetButtonColor, default=-16777201};
	__property int ButtonWidth = {read=FButtonWidth, write=SetButtonWidth, default=18};
	__property bool CheckRange = {read=FCheckRange, write=SetCheckRange, default=0};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Vcl::Graphics::TBitmap* GlyphMinus = {read=FGlyphBitmap, stored=false};
	__property Vcl::Graphics::TBitmap* GlyphPlus = {read=FGlyphBitmap, stored=false};
	__property System::UnicodeString HexValuePrefix = {read=FHexValuePrefix, write=SetHexValuePrefix, stored=StoreHexValuePrefix};
	__property int Increment = {read=FIncrement, write=FIncrement, default=1};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property System::Uitypes::TImageIndex ImageIndexMinus = {read=GetImageIndex, write=SetImageIndex, index=1, default=-1};
	__property System::Uitypes::TImageIndex ImageIndexPlus = {read=GetImageIndex, write=SetImageIndex, index=2, default=-1};
	__property int Max = {read=FMax, write=SetMax, default=100};
	__property int Min = {read=FMin, write=SetMin, default=0};
	__property int PageSize = {read=FPageSize, write=FPageSize, default=10};
	__property bool ShowHexValue = {read=FShowHexValue, write=SetShowHexValue, default=0};
	__property bool ShowFocusRect = {read=FShowFocusRect, write=FShowFocusRect, default=1};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property int Value = {read=FValue, write=SetValue, default=0};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property TRzSpinnerEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property TRzSpinnerIncDecEvent OnIncrement = {read=FOnIncrement, write=FOnIncrement};
	__property TRzSpinnerIncDecEvent OnDecrement = {read=FOnDecrement, write=FOnDecrement};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Color = {default=-16777211};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property Height = {default=21};
	__property HelpContext = {default=0};
	__property Hint = {default=0};
	__property ParentColor = {default=0};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=85};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSpinner(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
private:
	void *__IRzCustomFramingNotification;	// Rzcommon::IRzCustomFramingNotification 
	
public:
	#if defined(MANAGED_INTERFACE_OPERATORS)
	// {64B1C9EA-C954-428A-95C4-4EA2EB0F1E16}
	operator Rzcommon::_di_IRzCustomFramingNotification()
	{
		Rzcommon::_di_IRzCustomFramingNotification intf;
		this->GetInterface(intf);
		return intf;
	}
	#else
	operator Rzcommon::IRzCustomFramingNotification*(void) { return (Rzcommon::IRzCustomFramingNotification*)&__IRzCustomFramingNotification; }
	#endif
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzspnedt */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSPNEDT)
using namespace Rzspnedt;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzspnedtHPP
