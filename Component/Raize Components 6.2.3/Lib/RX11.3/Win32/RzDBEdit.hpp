// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBEdit.pas' rev: 35.00 (Windows)

#ifndef RzdbeditHPP
#define RzdbeditHPP

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
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzCommon.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzPopups.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCGrids.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Menus.hpp>
#include <System.MaskUtils.hpp>
#include <Vcl.ComCtrls.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbedit
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPaintEdit;
class DELPHICLASS TRzDBEdit;
class DELPHICLASS TRzDBNumericEdit;
class DELPHICLASS TRzDBExpandEdit;
class DELPHICLASS TRzDBDateTimeEdit;
class DELPHICLASS TRzPaintMemo;
class DELPHICLASS TRzDBMemo;
class DELPHICLASS TRzDBRichEdit;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzPaintEdit : public Vcl::Extctrls::TCustomPanel
{
	typedef Vcl::Extctrls::TCustomPanel inherited;
	
private:
	TRzDBEdit* FEditControl;
	
protected:
	virtual void __fastcall Paint();
	
public:
	__fastcall virtual TRzPaintEdit(System::Classes::TComponent* AOwner);
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzPaintEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPaintEdit(HWND ParentWindow) : Vcl::Extctrls::TCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBEdit : public Rzedit::TRzCustomEdit
{
	typedef Rzedit::TRzCustomEdit inherited;
	
private:
	bool FFocused;
	TRzPaintEdit* FPaintControl;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	System::Classes::TAlignment FAlignment;
	void __fastcall ActiveChangeHandler(System::TObject* Sender);
	void __fastcall DataChangeHandler(System::TObject* Sender);
	void __fastcall EditingChangeHandler(System::TObject* Sender);
	void __fastcall UpdateDataHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMCut(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaste(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMClear(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMUndo(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	bool FOverControl;
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall RepaintFrame();
	virtual System::UnicodeString __fastcall GetRightJustifiedText();
	virtual void __fastcall AdjustEditRect();
	virtual System::Types::TRect __fastcall GetEditRect();
	void __fastcall ResetMaxLength();
	virtual bool __fastcall EditCanModify();
	virtual void __fastcall Reset();
	virtual void __fastcall ActiveChanged();
	virtual void __fastcall DataChanged();
	virtual void __fastcall EditingChanged();
	virtual void __fastcall UpdateData();
	virtual System::UnicodeString __fastcall GetDisplayString();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual Data::Db::TField* __fastcall GetField();
	virtual void __fastcall SetFocused(bool Value);
	virtual bool __fastcall ReadOnlyValue();
	HIDESBASE bool __fastcall GetReadOnly();
	HIDESBASE void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	__property Vcl::Dbctrls::TFieldDataLink* DataLink = {read=FDataLink};
	
public:
	__fastcall virtual TRzDBEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBEdit();
	DYNAMIC bool __fastcall ExecuteAction(System::Classes::TBasicAction* Action);
	virtual bool __fastcall UpdateAction(System::Classes::TBasicAction* Action);
	DYNAMIC bool __fastcall UseRightToLeftAlignment();
	__property Data::Db::TField* Field = {read=GetField};
	__property Text;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property Align = {default=0};
	__property Alignment = {default=0};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzDBEdit(HWND ParentWindow) : Rzedit::TRzCustomEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBNumericEdit : public TRzDBEdit
{
	typedef TRzDBEdit inherited;
	
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
	Rzedit::TRzRangeErrorEvent FOnRangeError;
	bool __fastcall GetCalculatorVisible();
	void __fastcall SetCalculatorVisible(bool Value);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual bool __fastcall IsValidChar(System::WideChar Key);
	virtual System::UnicodeString __fastcall FormatText(const System::Extended Value);
	virtual System::Extended __fastcall EvaluateText();
	virtual void __fastcall DisplayCalculator();
	virtual System::UnicodeString __fastcall GetDisplayString();
	virtual void __fastcall DataChanged();
	virtual void __fastcall UpdateData();
	DYNAMIC void __fastcall CloseUp();
	DYNAMIC void __fastcall DropDown();
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
	__fastcall virtual TRzDBNumericEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBNumericEdit();
	__property __int64 IntValue = {read=GetIntValue, write=SetIntValue};
	__property bool Modified = {read=FModified, nodefault};
	
__published:
	__property bool AllowBlank = {read=FAllowBlank, write=FAllowBlank, default=1};
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
	__property Rzedit::TRzRangeErrorEvent OnRangeError = {read=FOnRangeError, write=FOnRangeError};
	__property Alignment = {default=1};
	__property FlatButtonColor = {default=-16777201};
	__property FlatButtons = {default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBNumericEdit(HWND ParentWindow) : TRzDBEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBExpandEdit : public TRzDBEdit
{
	typedef TRzDBEdit inherited;
	
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
	__fastcall virtual TRzDBExpandEdit(System::Classes::TComponent* AOwner);
	
__published:
	__property int ExpandedWidth = {read=FExpandedWidth, write=SetExpandedWidth, nodefault};
	__property Rzcommon::TExpandOnType ExpandOn = {read=FExpandOn, write=SetExpandOn, default=2};
public:
	/* TRzDBEdit.Destroy */ inline __fastcall virtual ~TRzDBExpandEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBExpandEdit(HWND ParentWindow) : TRzDBEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBDateTimeEdit : public TRzDBEdit
{
	typedef TRzDBEdit inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	Rzedit::TRzDTEditType FEditType;
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
	Rzedit::TRzDateTimeChangeEvent FOnDateTimeChange;
	Rzpopups::TRzGetWeekNumberEvent FOnGetWeekNumber;
	Rzpopups::TRzViewDateChangeEvent FOnViewDateChange;
	Rzpopups::TRzInvalidDateEvent FOnInvalidDate;
	Rzpopups::TRzInvalidTimeEvent FOnInvalidTime;
	bool __fastcall StoreMinDate();
	bool __fastcall StoreMaxDate();
	void __fastcall CheckDateTimeChange();
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	bool __fastcall CanEditData();
	void __fastcall SetDateTime();
	void __fastcall UpdateText();
	int __fastcall DateInRange(System::TDateTime Value);
	void __fastcall SetRange(System::TDate MinValue, System::TDate MaxValue);
	virtual void __fastcall DisplayCalendar();
	virtual void __fastcall DisplayTimePicker();
	virtual void __fastcall DataChanged();
	virtual void __fastcall UpdateData();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall CloseUp();
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall DateTimeChange();
	DYNAMIC void __fastcall InvalidDate(bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewDate);
	DYNAMIC void __fastcall InvalidTime(bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewTime);
	virtual System::TDate __fastcall GetDate();
	virtual void __fastcall SetDate(System::TDate Value);
	virtual System::TDateTime __fastcall GetAsDateTime();
	virtual void __fastcall SetAsDateTime(System::TDateTime Value);
	virtual bool __fastcall StoreDate();
	virtual void __fastcall SetMinDate(System::TDateTime Value);
	virtual void __fastcall SetMaxDate(System::TDateTime Value);
	virtual void __fastcall SetEditType(Rzedit::TRzDTEditType Value);
	virtual void __fastcall SetFormat(const System::UnicodeString Value);
	virtual System::TTime __fastcall GetTime();
	virtual void __fastcall SetTime(System::TTime Value);
	virtual bool __fastcall StoreTime();
	virtual void __fastcall SetClockFaceColors(Rzpopups::TRzClockFaceColors* Value);
	virtual void __fastcall SetCalendarColors(Rzpopups::TRzCalendarColors* Value);
	
public:
	__fastcall virtual TRzDBDateTimeEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBDateTimeEdit();
	virtual void __fastcall Clear();
	unsigned __fastcall DaysToBitmask(System::Byte *Days, const int Days_High);
	void __fastcall AdjustYear(int DeltaYears);
	void __fastcall AdjustMonth(int DeltaMonths);
	void __fastcall AdjustDay(int DeltaDays);
	void __fastcall AdjustHour(__int64 DeltaHours);
	void __fastcall AdjustMinute(__int64 DeltaMinutes);
	bool __fastcall ReformatDateTime();
	__property System::TDateTime AsDateTime = {read=GetAsDateTime, write=SetAsDateTime, stored=false};
	
__published:
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
	__property Rzedit::TRzDTEditType EditType = {read=FEditType, write=SetEditType, nodefault};
	__property System::UnicodeString Format = {read=FFormat, write=SetFormat};
	__property bool UseFormatToParse = {read=FUseFormatToParse, write=FUseFormatToParse, default=0};
	__property System::Uitypes::TColor PopupButtonColor = {read=FPopupButtonColor, write=FPopupButtonColor, default=-16777201};
	__property System::Uitypes::TColor PopupButtonFontColor = {read=FPopupButtonFontColor, write=FPopupButtonFontColor, default=-16777208};
	__property int PopupHeight = {read=FPopupHeight, write=FPopupHeight, default=0};
	__property int PopupWidth = {read=FPopupWidth, write=FPopupWidth, default=0};
	__property Rzpopups::TRzGetBoldDaysEvent OnGetBoldDays = {read=FOnGetBoldDays, write=FOnGetBoldDays};
	__property Rzpopups::TRzGetDayFormatEvent OnGetDayFormat = {read=FOnGetDayFormat, write=FOnGetDayFormat};
	__property Rzedit::TRzDateTimeChangeEvent OnDateTimeChange = {read=FOnDateTimeChange, write=FOnDateTimeChange};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzDBDateTimeEdit(HWND ParentWindow) : TRzDBEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzPaintMemo : public TRzPaintEdit
{
	typedef TRzPaintEdit inherited;
	
private:
	TRzDBMemo* FEditControl;
	
protected:
	virtual void __fastcall Paint();
	
public:
	__fastcall virtual TRzPaintMemo(System::Classes::TComponent* AOwner);
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzPaintMemo() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPaintMemo(HWND ParentWindow) : TRzPaintEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBMemo : public Vcl::Dbctrls::TDBMemo
{
	typedef Vcl::Dbctrls::TDBMemo inherited;
	
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
	TRzPaintMemo* FPaintControl;
	Rzedit::TLineColChangeEvent FOnLineColChange;
	Rzedit::TClipboardChangeEvent FOnClipboardChange;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
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
	__fastcall virtual TRzDBMemo(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBMemo();
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
	__property Rzedit::TLineColChangeEvent OnLineColChange = {read=FOnLineColChange, write=FOnLineColChange};
	__property Rzedit::TClipboardChangeEvent OnClipboardChange = {read=FOnClipboardChange, write=FOnClipboardChange};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBMemo(HWND ParentWindow) : Vcl::Dbctrls::TDBMemo(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBRichEdit : public Vcl::Dbctrls::TDBRichEdit
{
	typedef Vcl::Dbctrls::TDBRichEdit inherited;
	
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
	Rzedit::TLineColChangeEvent FOnLineColChange;
	Rzedit::TClipboardChangeEvent FOnClipboardChange;
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
	virtual void __fastcall DestroyWnd();
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
	__fastcall virtual TRzDBRichEdit(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBRichEdit();
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
	__property Rzedit::TLineColChangeEvent OnLineColChange = {read=FOnLineColChange, write=FOnLineColChange};
	__property Rzedit::TClipboardChangeEvent OnClipboardChange = {read=FOnClipboardChange, write=FOnClipboardChange};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBRichEdit(HWND ParentWindow) : Vcl::Dbctrls::TDBRichEdit(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbedit */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBEDIT)
using namespace Rzdbedit;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbeditHPP
