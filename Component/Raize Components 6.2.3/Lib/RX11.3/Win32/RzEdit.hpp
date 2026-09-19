// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzEdit.pas' rev: 35.00 (Windows)

#ifndef RzeditHPP
#define RzeditHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzCommon.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Mask.hpp>
#include <RzPopups.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Themes.hpp>
#include <System.MaskUtils.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzedit
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomEdit;
class DELPHICLASS TRzEditStyleHook;
class DELPHICLASS TRzEdit;
class DELPHICLASS TRzNumericEdit;
class DELPHICLASS TRzMaskEdit;
class DELPHICLASS TRzExpandEdit;
class DELPHICLASS TRzColorEdit;
class DELPHICLASS TRzDateTimeEdit;
class DELPHICLASS TRzMemo;
class DELPHICLASS TRzRichEdit;
class DELPHICLASS TRzHotKeyEdit;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzValidateErrorEvent)(System::TObject* Sender, System::UnicodeString &ErrorMsg);

class PASCALIMPLEMENTATION TRzCustomEdit : public Vcl::Mask::TCustomMaskEdit
{
	typedef Vcl::Mask::TCustomMaskEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	System::Classes::TAlignment FAlignment;
	bool FTabOnEnter;
	bool FBeepOnInvalidKey;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	System::Uitypes::TColor FFrameHotColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	Rzcommon::TFramingPreference FFramingPreference;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	bool FTextHintVisibleOnFocus;
	TRzValidateErrorEvent FOnValidateError;
	bool FDropButtonVisible;
	bool FShowDropButton;
	bool FMouseOverButton;
	bool FFlatButtons;
	System::Uitypes::TColor FFlatButtonColor;
	bool FDroppedDown;
	Vcl::Buttons::TButtonState FButtonState;
	System::Classes::TNotifyEvent FOnDropDown;
	System::Classes::TNotifyEvent FOnCloseUp;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	void __fastcall SetTextHintVisibleOnFocus(bool Value);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCCalcSize(Winapi::Messages::TWMNCCalcSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonUp(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDblClk(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMRButtonDown(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Graphics::TCanvas* FCanvas;
	bool FInControl;
	bool FOverControl;
	bool FFrameVisible;
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DoSetTextHint(const System::UnicodeString Value);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	virtual System::Types::TRect __fastcall GetEditRect();
	virtual System::UnicodeString __fastcall GetRightJustifiedText();
	virtual void __fastcall ValidateError();
	virtual void __fastcall HideButton();
	virtual void __fastcall ShowButton();
	void __fastcall RepaintButton();
	virtual void __fastcall DrawButton(Vcl::Graphics::TCanvas* Canvas, System::Types::TRect &R);
	void __fastcall MouseCancel();
	void __fastcall DoDropDown();
	void __fastcall InvalidKeyPressed();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall CloseUp();
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	HIDESBASE virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetButtonState(Vcl::Buttons::TButtonState Value);
	virtual void __fastcall SetDropButtonVisible(bool Value);
	virtual void __fastcall SetShowDropButton(bool Value);
	virtual void __fastcall SetFlatButtons(bool Value);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreReadOnlyColor();
	bool __fastcall StoreReadOnlyColorOnFocus();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	virtual bool __fastcall ReadOnlyValue();
	virtual void __fastcall ReadOnlyChanged();
	HIDESBASE bool __fastcall GetReadOnly();
	HIDESBASE void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	__property Vcl::Graphics::TCanvas* Canvas = {read=FCanvas};
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=0};
	__property bool BeepOnInvalidKey = {read=FBeepOnInvalidKey, write=FBeepOnInvalidKey, default=1};
	__property Vcl::Buttons::TButtonState ButtonState = {read=FButtonState, write=SetButtonState, nodefault};
	__property bool DropButtonVisible = {read=FDropButtonVisible, write=SetDropButtonVisible, default=0};
	__property bool ShowDropButton = {read=FShowDropButton, write=SetShowDropButton, default=0};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=FFlatButtonColor, stored=StoreFlatButtonColor, default=-16777201};
	__property bool FlatButtons = {read=FFlatButtons, write=SetFlatButtons, stored=StoreFlatButtons, default=0};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
	__property bool TextHintVisibleOnFocus = {read=FTextHintVisibleOnFocus, write=SetTextHintVisibleOnFocus, default=0};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, stored=StoreReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, stored=StoreReadOnlyColorOnFocus, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property System::Classes::TNotifyEvent OnCloseUp = {read=FOnCloseUp, write=FOnCloseUp};
	__property System::Classes::TNotifyEvent OnDropDown = {read=FOnDropDown, write=FOnDropDown};
	__property TRzValidateErrorEvent OnValidateError = {read=FOnValidateError, write=FOnValidateError};
	
public:
	__fastcall virtual TRzCustomEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomEdit();
	virtual bool __fastcall UseThemes();
	DYNAMIC System::Classes::TAlignment __fastcall GetControlsAlignment();
	bool __fastcall UseRightToLeftLayout();
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomEdit(HWND ParentWindow) : Vcl::Mask::TCustomMaskEdit(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzEditStyleHook : public Vcl::Stdctrls::TEditStyleHook
{
	typedef Vcl::Stdctrls::TEditStyleHook inherited;
	
private:
	HIDESBASE MESSAGE void __fastcall WMNCCalcSize(Winapi::Messages::TWMNCCalcSize &Msg);
	
protected:
	virtual void __fastcall PaintNC(Vcl::Graphics::TCanvas* Canvas);
	
public:
	__fastcall virtual TRzEditStyleHook(Vcl::Controls::TWinControl* AControl);
public:
	/* TMouseTrackControlStyleHook.Destroy */ inline __fastcall virtual ~TRzEditStyleHook() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzEdit : public TRzCustomEdit
{
	typedef TRzCustomEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
protected:
	HIDESBASE Vcl::Controls::TCaption __fastcall GetText();
	HIDESBASE void __fastcall SetText(const Vcl::Controls::TCaption Value);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Controls::TCaption Text = {read=GetText, write=SetText};
	__property Align = {default=0};
	__property Alignment = {default=0};
	__property Anchors = {default=3};
	__property AutoSelect = {default=1};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
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
	__property MaxLength = {default=0};
	__property OEMConvert = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
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
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomEdit.Create */ inline __fastcall virtual TRzEdit(System::Classes::TComponent* AOwner) : TRzCustomEdit(AOwner) { }
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzEdit(HWND ParentWindow) : TRzCustomEdit(ParentWindow) { }
	
};


typedef void __fastcall (__closure *TRzRangeErrorEvent)(System::TObject* Sender, System::Extended EnteredValue, System::Extended AdjustedValue, bool &AutoCorrect);

class PASCALIMPLEMENTATION TRzNumericEdit : public TRzEdit
{
	typedef TRzEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	bool FAllowBlank;
	bool FAllowScientificNotation;
	System::Extended FBlankValue;
	bool FCheckRange;
	bool FIntegersOnly;
	System::Extended FMin;
	System::Extended FMax;
	System::UnicodeString FDisplayFormat;
	System::Extended FFieldValue;
	bool FModified;
	bool FCalculatorBoldButtons;
	Rzpopups::TRzCalculatorColors* FCalculatorColors;
	bool FDecimalPressed;
	System::Classes::TAlignment FPopupAlignment;
	int FPopupHeight;
	int FPopupWidth;
	TRzRangeErrorEvent FOnRangeError;
	bool __fastcall GetCalculatorVisible();
	void __fastcall SetCalculatorVisible(bool Value);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaste(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMCut(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	bool FLoading;
	virtual void __fastcall Loaded();
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual bool __fastcall IsValidChar(System::WideChar Key);
	virtual System::UnicodeString __fastcall FormatText(const System::Extended Value);
	System::UnicodeString __fastcall CleanupQuotedText(const System::UnicodeString S);
	virtual System::Extended __fastcall EvaluateText();
	virtual void __fastcall DisplayCalculator();
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall RangeError(System::Extended EnteredValue, System::Extended AdjustedValue, bool &AutoCorrect);
	virtual void __fastcall SetIntegersOnly(bool Value);
	virtual void __fastcall SetMin(const System::Extended Value);
	virtual void __fastcall SetMax(const System::Extended Value);
	virtual __int64 __fastcall GetIntValue();
	virtual void __fastcall SetIntValue(__int64 Value);
	virtual System::Extended __fastcall GetValue();
	virtual System::Extended __fastcall CheckValue(const System::Extended Value, bool &KeepFocusOnEdit);
	virtual void __fastcall SetValue(const System::Extended Value);
	virtual void __fastcall SetDisplayFormat(System::UnicodeString FormatString);
	virtual void __fastcall SetCalculatorColors(Rzpopups::TRzCalculatorColors* Value);
	
public:
	__fastcall virtual TRzNumericEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzNumericEdit();
	__property __int64 IntValue = {read=GetIntValue, write=SetIntValue};
	__property bool Modified = {read=FModified, nodefault};
	
__published:
	__property bool AllowBlank = {read=FAllowBlank, write=FAllowBlank, default=0};
	__property bool AllowScientificNotation = {read=FAllowScientificNotation, write=FAllowScientificNotation, default=1};
	__property System::Extended BlankValue = {read=FBlankValue, write=FBlankValue};
	__property bool CalculatorVisible = {read=GetCalculatorVisible, write=SetCalculatorVisible, default=0};
	__property bool CalculatorBoldButtons = {read=FCalculatorBoldButtons, write=FCalculatorBoldButtons, default=0};
	__property Rzpopups::TRzCalculatorColors* CalculatorColors = {read=FCalculatorColors, write=SetCalculatorColors};
	__property bool CheckRange = {read=FCheckRange, write=FCheckRange, default=0};
	__property bool IntegersOnly = {read=FIntegersOnly, write=SetIntegersOnly, default=1};
	__property System::Extended Max = {read=FMax, write=SetMax};
	__property System::Extended Min = {read=FMin, write=SetMin};
	__property System::UnicodeString DisplayFormat = {read=FDisplayFormat, write=SetDisplayFormat};
	__property System::Classes::TAlignment PopupAlignment = {read=FPopupAlignment, write=FPopupAlignment, default=1};
	__property int PopupHeight = {read=FPopupHeight, write=FPopupHeight, default=0};
	__property int PopupWidth = {read=FPopupWidth, write=FPopupWidth, default=0};
	__property System::Extended Value = {read=GetValue, write=SetValue};
	__property TRzRangeErrorEvent OnRangeError = {read=FOnRangeError, write=FOnRangeError};
	__property Alignment = {default=1};
	__property BeepOnInvalidKey = {default=1};
	__property FlatButtonColor = {default=-16777201};
	__property FlatButtons = {default=0};
	__property Text = {stored=false};
	__property TextHint = {default=0};
	__property TextHintVisibleOnFocus = {default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzNumericEdit(HWND ParentWindow) : TRzEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzMaskEdit : public TRzCustomEdit
{
	typedef TRzCustomEdit inherited;
	
protected:
	virtual System::UnicodeString __fastcall GetRightJustifiedText();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Alignment = {default=0};
	__property Anchors = {default=3};
	__property AutoSelect = {default=1};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property EditMask = {default=0};
	__property FlatButtonColor = {default=-16777201};
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
	__property MaxLength = {default=0};
	__property OEMConvert = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
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
	__property Text;
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
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnValidateError;
public:
	/* TRzCustomEdit.Create */ inline __fastcall virtual TRzMaskEdit(System::Classes::TComponent* AOwner) : TRzCustomEdit(AOwner) { }
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzMaskEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzMaskEdit(HWND ParentWindow) : TRzCustomEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzExpandEdit : public TRzEdit
{
	typedef TRzEdit inherited;
	
private:
	int FExpandedWidth;
	bool FExpanded;
	int FOrigWidth;
	Rzcommon::TExpandOnType FExpandOn;
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMRButtonUp(Winapi::Messages::TWMMouse &Msg);
	
protected:
	virtual void __fastcall SetExpandedWidth(int Value);
	virtual void __fastcall SetExpandOn(Rzcommon::TExpandOnType Value);
	
public:
	__fastcall virtual TRzExpandEdit(System::Classes::TComponent* AOwner);
	
__published:
	__property int ExpandedWidth = {read=FExpandedWidth, write=SetExpandedWidth, nodefault};
	__property Rzcommon::TExpandOnType ExpandOn = {read=FExpandOn, write=SetExpandOn, default=2};
public:
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzExpandEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzExpandEdit(HWND ParentWindow) : TRzEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzColorEdit : public TRzCustomEdit
{
	typedef TRzCustomEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	System::Uitypes::TColor FCustomColor;
	System::Uitypes::TColor FDefaultColor;
	Vcl::Dialogs::TColorDialogOptions FColorDlgOptions;
	Rzcommon::TRzCustomColors* FCustomColors;
	System::UnicodeString FCustomColorCaption;
	System::UnicodeString FNoColorCaption;
	System::UnicodeString FDefaultColorCaption;
	bool FShowCustomColor;
	bool FShowNoColor;
	bool FShowDefaultColor;
	bool FShowSystemColors;
	System::Uitypes::TColor FSelectedColor;
	bool FShowColorHints;
	System::Classes::TAlignment FPopupAlignment;
	System::Uitypes::TColor FPopupButtonColor;
	System::Uitypes::TColor FPopupButtonFontColor;
	int FPopupHeight;
	int FPopupWidth;
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DisplayColorPicker();
	DYNAMIC void __fastcall DropDown();
	virtual void __fastcall SetCustomColors(Rzcommon::TRzCustomColors* Value);
	virtual void __fastcall SetSelectedColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzColorEdit(System::Classes::TComponent* AOwner);
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString CustomColorCaption = {read=FCustomColorCaption, write=FCustomColorCaption};
	__property System::Uitypes::TColor CustomColor = {read=FCustomColor, write=FCustomColor, default=16777215};
	__property Rzcommon::TRzCustomColors* CustomColors = {read=FCustomColors, write=SetCustomColors};
	__property Vcl::Dialogs::TColorDialogOptions ColorDlgOptions = {read=FColorDlgOptions, write=FColorDlgOptions, default=1};
	__property System::Uitypes::TColor DefaultColor = {read=FDefaultColor, write=FDefaultColor, default=-16777203};
	__property System::UnicodeString DefaultColorCaption = {read=FDefaultColorCaption, write=FDefaultColorCaption};
	__property bool ShowColorHints = {read=FShowColorHints, write=FShowColorHints, default=1};
	__property System::UnicodeString NoColorCaption = {read=FNoColorCaption, write=FNoColorCaption};
	__property System::Classes::TAlignment PopupAlignment = {read=FPopupAlignment, write=FPopupAlignment, default=1};
	__property System::Uitypes::TColor PopupButtonColor = {read=FPopupButtonColor, write=FPopupButtonColor, default=-16777201};
	__property System::Uitypes::TColor PopupButtonFontColor = {read=FPopupButtonFontColor, write=FPopupButtonFontColor, default=-16777208};
	__property int PopupHeight = {read=FPopupHeight, write=FPopupHeight, default=0};
	__property int PopupWidth = {read=FPopupWidth, write=FPopupWidth, default=0};
	__property System::Uitypes::TColor SelectedColor = {read=FSelectedColor, write=SetSelectedColor, default=536870911};
	__property bool ShowNoColor = {read=FShowNoColor, write=FShowNoColor, default=0};
	__property bool ShowCustomColor = {read=FShowCustomColor, write=FShowCustomColor, default=0};
	__property bool ShowDefaultColor = {read=FShowDefaultColor, write=FShowDefaultColor, default=0};
	__property bool ShowSystemColors = {read=FShowSystemColors, write=FShowSystemColors, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DropButtonVisible = {default=1};
	__property Enabled = {default=1};
	__property FlatButtonColor = {default=-16777201};
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
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDropDown;
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
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzColorEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzColorEdit(HWND ParentWindow) : TRzCustomEdit(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzDTEditType : unsigned char { etDate, etTime };

typedef void __fastcall (__closure *TRzDateTimeChangeEvent)(System::TObject* Sender, System::TDateTime DateTime);

class PASCALIMPLEMENTATION TRzDateTimeEdit : public TRzCustomEdit
{
	typedef TRzCustomEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	TRzDTEditType FEditType;
	System::TDateTime FLastDateTime;
	System::TDateTime FDateTime;
	System::TDateTime FMinDate;
	System::TDateTime FMaxDate;
	System::UnicodeString FFormat;
	bool FUseFormatToParse;
	bool FUpdating;
	bool FTimeHasBeenSet;
	bool FSettingTime;
	bool FTimePicked;
	bool FDateHasBeenSet;
	bool FSettingDate;
	bool FDatePicked;
	bool FValidating;
	Rzcommon::TRzTwoDigitYearConverter FTwoDigitYearConverter;
	Rzpopups::TRzCalendarElements FCalendarElements;
	Rzpopups::TRzCalendarColors* FCalendarColors;
	System::UnicodeString FCaptionClearBtn;
	System::UnicodeString FCaptionTodayBtn;
	Rzpopups::TRzFirstDayOfWeek FFirstDayOfWeek;
	Rzpopups::TRzClockFaceColors* FClockFaceColors;
	System::UnicodeString FCaptionAM;
	System::UnicodeString FCaptionPM;
	System::UnicodeString FCaptionSet;
	bool FRestrictMinutes;
	bool FShowHowToUseHint;
	System::UnicodeString FHowToUseMsg;
	System::Classes::TAlignment FPopupAlignment;
	System::Uitypes::TColor FPopupButtonColor;
	System::Uitypes::TColor FPopupButtonFontColor;
	int FPopupHeight;
	int FPopupWidth;
	Rzpopups::TRzGetBoldDaysEvent FOnGetBoldDays;
	Rzpopups::TRzGetDayFormatEvent FOnGetDayFormat;
	TRzDateTimeChangeEvent FOnDateTimeChange;
	Rzpopups::TRzGetWeekNumberEvent FOnGetWeekNumber;
	Rzpopups::TRzViewDateChangeEvent FOnViewDateChange;
	Rzpopups::TRzInvalidDateEvent FOnInvalidDate;
	Rzpopups::TRzInvalidTimeEvent FOnInvalidTime;
	bool __fastcall StoreMinDate();
	bool __fastcall StoreMaxDate();
	void __fastcall CheckDateTimeChange();
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	void __fastcall SetDateTime();
	void __fastcall UpdateText();
	int __fastcall DateInRange(System::TDateTime Value);
	void __fastcall SetRange(System::TDate MinValue, System::TDate MaxValue);
	virtual void __fastcall DisplayCalendar();
	virtual void __fastcall DisplayTimePicker();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall DateTimeChange();
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall InvalidDate(bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewDate);
	DYNAMIC void __fastcall InvalidTime(bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewTime);
	virtual System::TDate __fastcall GetDate();
	virtual void __fastcall SetDate(System::TDate Value);
	virtual System::TDateTime __fastcall GetAsDateTime();
	virtual void __fastcall SetAsDateTime(System::TDateTime Value);
	virtual bool __fastcall StoreDate();
	virtual void __fastcall SetMinDate(System::TDateTime Value);
	virtual void __fastcall SetMaxDate(System::TDateTime Value);
	virtual void __fastcall SetEditType(TRzDTEditType Value);
	virtual void __fastcall SetFormat(const System::UnicodeString Value);
	virtual System::TTime __fastcall GetTime();
	virtual void __fastcall SetTime(System::TTime Value);
	virtual bool __fastcall StoreTime();
	virtual void __fastcall SetClockFaceColors(Rzpopups::TRzClockFaceColors* Value);
	virtual void __fastcall SetCalendarColors(Rzpopups::TRzCalendarColors* Value);
	HIDESBASE Vcl::Controls::TCaption __fastcall GetText();
	HIDESBASE void __fastcall SetText(const Vcl::Controls::TCaption Value);
	
public:
	__fastcall virtual TRzDateTimeEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDateTimeEdit();
	virtual void __fastcall Clear();
	unsigned __fastcall DaysToBitmask(System::Byte *Days, const int Days_High);
	void __fastcall AdjustYear(int DeltaYears);
	void __fastcall AdjustMonth(int DeltaMonths);
	void __fastcall AdjustDay(int DeltaDays);
	void __fastcall AdjustHour(__int64 DeltaHours);
	void __fastcall AdjustMinute(__int64 DeltaMinutes);
	void __fastcall ReformatDateTime();
	__property Vcl::Controls::TCaption Text = {read=GetText, write=SetText};
	__property System::TDateTime AsDateTime = {read=GetAsDateTime, write=SetAsDateTime, stored=false};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Rzpopups::TRzCalendarColors* CalendarColors = {read=FCalendarColors, write=SetCalendarColors};
	__property Rzpopups::TRzCalendarElements CalendarElements = {read=FCalendarElements, write=FCalendarElements, default=247};
	__property System::UnicodeString CaptionTodayBtn = {read=FCaptionTodayBtn, write=FCaptionTodayBtn};
	__property System::UnicodeString CaptionClearBtn = {read=FCaptionClearBtn, write=FCaptionClearBtn};
	__property System::UnicodeString CaptionAM = {read=FCaptionAM, write=FCaptionAM};
	__property System::UnicodeString CaptionPM = {read=FCaptionPM, write=FCaptionPM};
	__property System::UnicodeString CaptionSet = {read=FCaptionSet, write=FCaptionSet};
	__property Rzcommon::TRzTwoDigitYearConverter TwoDigitYearConverter = {read=FTwoDigitYearConverter, write=FTwoDigitYearConverter, default=0};
	__property Rzpopups::TRzClockFaceColors* ClockFaceColors = {read=FClockFaceColors, write=SetClockFaceColors};
	__property System::TDate Date = {read=GetDate, write=SetDate, stored=StoreDate};
	__property System::TDateTime MinDate = {read=FMinDate, write=SetMinDate, stored=StoreMinDate};
	__property System::TDateTime MaxDate = {read=FMaxDate, write=SetMaxDate, stored=StoreMaxDate};
	__property Rzpopups::TRzFirstDayOfWeek FirstDayOfWeek = {read=FFirstDayOfWeek, write=FFirstDayOfWeek, default=7};
	__property System::UnicodeString HowToUseMsg = {read=FHowToUseMsg, write=FHowToUseMsg};
	__property bool RestrictMinutes = {read=FRestrictMinutes, write=FRestrictMinutes, default=0};
	__property bool ShowHowToUseHint = {read=FShowHowToUseHint, write=FShowHowToUseHint, default=1};
	__property System::TTime Time = {read=GetTime, write=SetTime, stored=StoreTime};
	__property TRzDTEditType EditType = {read=FEditType, write=SetEditType, nodefault};
	__property System::UnicodeString Format = {read=FFormat, write=SetFormat};
	__property bool UseFormatToParse = {read=FUseFormatToParse, write=FUseFormatToParse, default=0};
	__property System::Classes::TAlignment PopupAlignment = {read=FPopupAlignment, write=FPopupAlignment, default=1};
	__property System::Uitypes::TColor PopupButtonColor = {read=FPopupButtonColor, write=FPopupButtonColor, default=-16777201};
	__property System::Uitypes::TColor PopupButtonFontColor = {read=FPopupButtonFontColor, write=FPopupButtonFontColor, default=-16777208};
	__property int PopupHeight = {read=FPopupHeight, write=FPopupHeight, default=0};
	__property int PopupWidth = {read=FPopupWidth, write=FPopupWidth, default=0};
	__property Rzpopups::TRzGetBoldDaysEvent OnGetBoldDays = {read=FOnGetBoldDays, write=FOnGetBoldDays};
	__property Rzpopups::TRzGetDayFormatEvent OnGetDayFormat = {read=FOnGetDayFormat, write=FOnGetDayFormat};
	__property TRzDateTimeChangeEvent OnDateTimeChange = {read=FOnDateTimeChange, write=FOnDateTimeChange};
	__property Rzpopups::TRzGetWeekNumberEvent OnGetWeekNumber = {read=FOnGetWeekNumber, write=FOnGetWeekNumber};
	__property Rzpopups::TRzViewDateChangeEvent OnViewDateChange = {read=FOnViewDateChange, write=FOnViewDateChange};
	__property Rzpopups::TRzInvalidDateEvent OnInvalidDate = {read=FOnInvalidDate, write=FOnInvalidDate};
	__property Rzpopups::TRzInvalidTimeEvent OnInvalidTime = {read=FOnInvalidTime, write=FOnInvalidTime};
	__property Align = {default=0};
	__property Alignment = {default=0};
	__property Anchors = {default=3};
	__property AutoSelect = {default=1};
	__property AutoSize = {default=1};
	__property BeepOnInvalidKey = {default=1};
	__property BorderStyle = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DropButtonVisible = {default=1};
	__property Enabled = {default=1};
	__property FlatButtonColor = {default=-16777201};
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
	__property MaxLength = {default=0};
	__property OEMConvert = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentDoubleBuffered = {default=1};
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
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDropDown;
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
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDateTimeEdit(HWND ParentWindow) : TRzCustomEdit(ParentWindow) { }
	
};


typedef void __fastcall (__closure *TLineColChangeEvent)(System::TObject* Sender, int Line, int Column);

typedef void __fastcall (__closure *TClipboardChangeEvent)(System::TObject* Sender, bool HasSelection, bool HasText);

class PASCALIMPLEMENTATION TRzMemo : public Vcl::Stdctrls::TMemo
{
	typedef Vcl::Stdctrls::TMemo inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	bool FTabOnEnter;
	TLineColChangeEvent FOnLineColChange;
	TClipboardChangeEvent FOnClipboardChange;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas;
	bool FOverControl;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall LineColChange();
	DYNAMIC void __fastcall ClipboardChange();
	DYNAMIC bool __fastcall DoMouseWheel(System::Classes::TShiftState Shift, int WheelDelta, const System::Types::TPoint &MousePos);
	virtual int __fastcall GetColumn();
	virtual void __fastcall SetColumn(int Value);
	virtual int __fastcall GetLine();
	virtual void __fastcall SetLine(int Value);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreReadOnlyColor();
	bool __fastcall StoreReadOnlyColorOnFocus();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	HIDESBASE bool __fastcall GetReadOnly();
	HIDESBASE void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	__property Vcl::Graphics::TCanvas* Canvas = {read=FCanvas};
	
public:
	__fastcall virtual TRzMemo(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzMemo();
	virtual bool __fastcall UseThemes();
	void __fastcall JumpTo(int ALine, int ACol);
	__property int Column = {read=GetColumn, write=SetColumn, nodefault};
	__property int Line = {read=GetLine, write=SetLine, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, stored=StoreReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, stored=StoreReadOnlyColorOnFocus, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property TLineColChangeEvent OnLineColChange = {read=FOnLineColChange, write=FOnLineColChange};
	__property TClipboardChangeEvent OnClipboardChange = {read=FOnClipboardChange, write=FOnClipboardChange};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzMemo(HWND ParentWindow) : Vcl::Stdctrls::TMemo(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzRichEdit : public Vcl::Comctrls::TRichEdit
{
	typedef Vcl::Comctrls::TRichEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	bool FTabOnEnter;
	TLineColChangeEvent FOnLineColChange;
	TClipboardChangeEvent FOnClipboardChange;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas;
	bool FOverControl;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall SelectionChange();
	DYNAMIC void __fastcall LineColChange();
	DYNAMIC void __fastcall ClipboardChange();
	DYNAMIC bool __fastcall DoMouseWheel(System::Classes::TShiftState Shift, int WheelDelta, const System::Types::TPoint &MousePos);
	virtual int __fastcall GetColumn();
	virtual void __fastcall SetColumn(int Value);
	virtual int __fastcall GetLine();
	virtual void __fastcall SetLine(int Value);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreReadOnlyColor();
	bool __fastcall StoreReadOnlyColorOnFocus();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	HIDESBASE bool __fastcall GetReadOnly();
	HIDESBASE void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	__property Vcl::Graphics::TCanvas* Canvas = {read=FCanvas};
	
public:
	__fastcall virtual TRzRichEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzRichEdit();
	virtual bool __fastcall UseThemes();
	void __fastcall JumpTo(int ALine, int ACol);
	System::UnicodeString __fastcall GetRtfData();
	__property int Column = {read=GetColumn, write=SetColumn, nodefault};
	__property int Line = {read=GetLine, write=SetLine, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, stored=StoreReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, stored=StoreReadOnlyColorOnFocus, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property TLineColChangeEvent OnLineColChange = {read=FOnLineColChange, write=FOnLineColChange};
	__property TClipboardChangeEvent OnClipboardChange = {read=FOnClipboardChange, write=FOnClipboardChange};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzRichEdit(HWND ParentWindow) : Vcl::Comctrls::TRichEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzHotKeyEdit : public Vcl::Comctrls::THotKey
{
	typedef Vcl::Comctrls::THotKey inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas;
	bool FOverControl;
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	__property Vcl::Graphics::TCanvas* Canvas = {read=FCanvas};
	
public:
	__fastcall virtual TRzHotKeyEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzHotKeyEdit();
	virtual bool __fastcall UseThemes();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzHotKeyEdit(HWND ParentWindow) : Vcl::Comctrls::THotKey(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzedit */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZEDIT)
using namespace Rzedit;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzeditHPP
