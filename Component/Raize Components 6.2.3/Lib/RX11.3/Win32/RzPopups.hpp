// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPopups.pas' rev: 35.00 (Windows)

#ifndef RzpopupsHPP
#define RzpopupsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <RzPanel.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzpopups
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCalendarColors;
class DELPHICLASS ERzCalendarError;
class DELPHICLASS TRzCalendar;
class DELPHICLASS TRzClockFaceColors;
class DELPHICLASS TRzTimePicker;
class DELPHICLASS TRzCalculatorColors;
class DELPHICLASS TRzCalculator;
class DELPHICLASS TRzCustomPopup;
class DELPHICLASS TRzPopupPanel;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCalendarColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	System::Uitypes::TColor FDays;
	System::Uitypes::TColor FFillDays;
	System::Uitypes::TColor FDaysOfWeek;
	System::Uitypes::TColor FLines;
	System::Uitypes::TColor FSelectedDateBack;
	System::Uitypes::TColor FSelectedDateFore;
	System::Uitypes::TColor FTodaysDateFrame;
	TRzCalendar* FCalendar;
	
protected:
	virtual System::Uitypes::TColor __fastcall GetColor(int Index);
	virtual void __fastcall SetColor(int Index, System::Uitypes::TColor Value);
	
public:
	__fastcall TRzCalendarColors(TRzCalendar* ACalendar);
	__fastcall virtual ~TRzCalendarColors();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property TRzCalendar* Calendar = {read=FCalendar};
	__property System::Uitypes::TColor Colors[int Index] = {read=GetColor, write=SetColor};
	
__published:
	__property System::Uitypes::TColor Days = {read=GetColor, write=SetColor, index=0, default=-16777208};
	__property System::Uitypes::TColor FillDays = {read=GetColor, write=SetColor, index=1, default=-16777200};
	__property System::Uitypes::TColor DaysOfWeek = {read=GetColor, write=SetColor, index=2, default=-16777208};
	__property System::Uitypes::TColor Lines = {read=GetColor, write=SetColor, index=3, default=-16777200};
	__property System::Uitypes::TColor SelectedDateBack = {read=GetColor, write=SetColor, index=4, default=-16777203};
	__property System::Uitypes::TColor SelectedDateFore = {read=GetColor, write=SetColor, index=5, default=-16777202};
	__property System::Uitypes::TColor TodaysDateFrame = {read=GetColor, write=SetColor, index=6, default=128};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzCalendarElement : unsigned char { ceYear, ceMonth, ceArrows, ceWeekNumbers, ceDaysOfWeek, ceFillDays, ceTodayButton, ceClearButton };

typedef System::Set<TRzCalendarElement, TRzCalendarElement::ceYear, TRzCalendarElement::ceClearButton> TRzCalendarElements;

enum DECLSPEC_DENUM TRzCalendarArea : unsigned char { caYear, caMonth, caLeftArrow, caRightArrow, caWeekNumbers, caDays, caDaysOfWeek, caFillDays, caTodayButton, caClearButton };

typedef System::StaticArray<System::Types::TRect, 10> TRzCalendarAreas;

enum DECLSPEC_DENUM TRzFirstDayOfWeek : unsigned char { fdowMonday, fdowTuesday, fdowWednesday, fdowThursday, fdowFriday, fdowSaturday, fdowSunday, fdowLocale };

typedef void __fastcall (__closure *TRzGetBoldDaysEvent)(System::TObject* Sender, System::Word Year, System::Word Month, unsigned &Bitmask);

typedef void __fastcall (__closure *TRzGetDayFormatEvent)(System::TObject* Sender, System::TDateTime DayDate, System::Word Year, System::Word Month, System::Word Day, System::Uitypes::TColor &DayColor, System::Uitypes::TColor &DayFontColor, System::Uitypes::TFontStyles &DayFontStyle);

typedef void __fastcall (__closure *TRzGetWeekNumberEvent)(System::TObject* Sender, System::TDateTime WeekDate, int &WeekNumber);

typedef void __fastcall (__closure *TRzViewDateChangeEvent)(System::TObject* Sender, System::TDateTime ViewDate);

typedef void __fastcall (__closure *TRzInvalidDateEvent)(System::TObject* Sender, bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewDate);

typedef void __fastcall (__closure *TRzInvalidTimeEvent)(System::TObject* Sender, bool &KeepFocused, bool &KeepInvalidText, System::TDateTime &NewTime);

typedef void __fastcall (__closure *TRzCalendarTodayEvent)(System::TObject* Sender, bool &AllowChange);

typedef void __fastcall (__closure *TRzCalendarClearEvent)(System::TObject* Sender, bool &AllowClear);

#pragma pack(push,4)
class PASCALIMPLEMENTATION ERzCalendarError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall ERzCalendarError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall ERzCalendarError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall ERzCalendarError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall ERzCalendarError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzCalendarError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzCalendarError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall ERzCalendarError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall ERzCalendarError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzCalendarError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzCalendarError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzCalendarError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzCalendarError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~ERzCalendarError() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCalendar : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	System::Byte FFirstDay;
	TRzFirstDayOfWeek FFirstDayOfWeek;
	TRzCalendarElements FElements;
	TRzCalendarArea FPressedArea;
	TRzCalendarArea FOverArea;
	bool FShowDays;
	bool FIsPopup;
	System::Types::TRect FMouseOverRect;
	System::Types::TPoint FCharSize;
	System::Types::TPoint FCellSize;
	System::TDateTime FDate;
	System::TDateTime FMinDate;
	System::TDateTime FMaxDate;
	Vcl::Extctrls::TTimer* FTodayTimer;
	System::TDateTime FToday;
	System::TDateTime FViewDate;
	System::TDateTime FFirstDateInGrid;
	bool FForceUpdate;
	bool FClearClicked;
	bool FIgnoreClick;
	System::Uitypes::TColor FButtonColor;
	System::Uitypes::TColor FButtonFontColor;
	Vcl::Controls::TCaption FCaptionClearBtn;
	int FClearBtnWidth;
	Vcl::Controls::TCaption FCaptionTodayBtn;
	int FTodayBtnWidth;
	TRzCalendarColors* FCalendarColors;
	int FCounter;
	Vcl::Menus::TPopupMenu* FMonthPopupMenu;
	Vcl::Controls::TWinControl* FYearSpinner;
	bool FYearSpinnerVisible;
	bool FThemeAware;
	System::Classes::TNotifyEvent FOnChange;
	TRzGetBoldDaysEvent FOnGetBoldDays;
	TRzGetDayFormatEvent FOnGetDayFormat;
	TRzGetWeekNumberEvent FOnGetWeekNumber;
	TRzViewDateChangeEvent FOnViewDateChange;
	TRzCalendarTodayEvent FOnToday;
	TRzCalendarClearEvent FOnClear;
	bool __fastcall StoreMinDate();
	bool __fastcall StoreMaxDate();
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	MESSAGE void __fastcall WMTimer(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	void __fastcall TodayTimerExpired(System::TObject* Sender);
	
protected:
	virtual void __fastcall CreateHandle();
	virtual void __fastcall CreateWnd();
	bool __fastcall ShowFocus();
	virtual void __fastcall Paint();
	System::Uitypes::TColor __fastcall GetButtonFontColor();
	int __fastcall GetMaxShortDayNameLength();
	void __fastcall CalcAreas(TRzCalendarAreas &Areas);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	void __fastcall AdjustForFont();
	void __fastcall CalcFontSize();
	virtual void __fastcall ConstrainedResize(int &MinWidth, int &MinHeight, int &MaxWidth, int &MaxHeight);
	virtual void __fastcall SetDate(System::TDateTime Value);
	int __fastcall DateInRange(System::TDateTime Value);
	bool __fastcall ViewMonthInRange(System::TDateTime Value);
	void __fastcall SetRange(System::TDate MinValue, System::TDate MaxValue);
	void __fastcall CreateMonthPopupMenu();
	void __fastcall DisplayMonthPopupMenu(int X, int Y);
	void __fastcall MonthPopupClickHandler(System::TObject* Sender);
	void __fastcall HideYearSpinner();
	void __fastcall ShowYearSpinner();
	void __fastcall YearSpinnerDecrementHandler(System::TObject* Sender, int Amount);
	void __fastcall YearSpinnerIncrementHandler(System::TObject* Sender, int Amount);
	void __fastcall UpdateHighlight(int X, int Y);
	System::TDateTime __fastcall InternalHitTest(const System::Types::TRect &R, const System::Types::TPoint &P);
	void __fastcall StartTimer();
	void __fastcall TimerExpired();
	void __fastcall StopTimer();
	HIDESBASEDYNAMIC void __fastcall Changed();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall DblClick();
	DYNAMIC void __fastcall GetBoldDays(System::Word Year, System::Word Month, unsigned &Bitmask);
	DYNAMIC bool __fastcall GetDayFormat(System::TDateTime DayDate, System::Word Year, System::Word Month, System::Word Day, System::Uitypes::TColor &DayColor, System::Uitypes::TColor &DayFontColor, System::Uitypes::TFontStyles &DayFontStyle);
	DYNAMIC void __fastcall ViewDateChange(System::TDateTime ViewDate);
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC bool __fastcall CanClear();
	DYNAMIC bool __fastcall CanGoToToday();
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCalendarColors(TRzCalendarColors* Value);
	virtual void __fastcall SetCaptionClearBtn(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetCaptionTodayBtn(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetDateProperty(System::TDateTime Value);
	virtual void __fastcall SetMinDate(System::TDateTime Value);
	virtual void __fastcall SetMaxDate(System::TDateTime Value);
	virtual void __fastcall SetFirstDayOfWeek(TRzFirstDayOfWeek Value);
	virtual void __fastcall SetElements(TRzCalendarElements Value);
	virtual void __fastcall SetOverArea(TRzCalendarArea Value);
	virtual void __fastcall SetPressedArea(TRzCalendarArea Value);
	virtual void __fastcall SetShowDays(bool Value);
	virtual void __fastcall SetViewDate(System::TDateTime Value);
	virtual void __fastcall SetThemeAware(bool Value);
	
public:
	__fastcall virtual TRzCalendar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCalendar();
	virtual bool __fastcall CanAutoSize(int &NewWidth, int &NewHeight);
	unsigned __fastcall DaysToBitmask(System::Byte *Days, const int Days_High);
	System::TDateTime __fastcall HitTest(int X, int Y);
	bool __fastcall IsClear();
	void __fastcall Clear();
	void __fastcall Today();
	__property System::TDateTime ViewDate = {read=FViewDate, write=SetViewDate};
	__property bool IsPopup = {write=FIsPopup, nodefault};
	__property bool ClearClicked = {read=FClearClicked, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=SetButtonColor, default=-16777201};
	__property System::Uitypes::TColor ButtonFontColor = {read=FButtonFontColor, write=SetButtonFontColor, default=-16777208};
	__property TRzCalendarColors* CalendarColors = {read=FCalendarColors, write=SetCalendarColors};
	__property Vcl::Controls::TCaption CaptionClearBtn = {read=FCaptionClearBtn, write=SetCaptionClearBtn};
	__property Vcl::Controls::TCaption CaptionTodayBtn = {read=FCaptionTodayBtn, write=SetCaptionTodayBtn};
	__property System::TDateTime Date = {read=FDate, write=SetDateProperty};
	__property System::TDateTime MinDate = {read=FMinDate, write=SetMinDate, stored=StoreMinDate};
	__property System::TDateTime MaxDate = {read=FMaxDate, write=SetMaxDate, stored=StoreMaxDate};
	__property TRzFirstDayOfWeek FirstDayOfWeek = {read=FFirstDayOfWeek, write=SetFirstDayOfWeek, default=7};
	__property TRzCalendarElements Elements = {read=FElements, write=SetElements, default=247};
	__property bool ShowDays = {read=FShowDays, write=SetShowDays, default=1};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property TRzGetBoldDaysEvent OnGetBoldDays = {read=FOnGetBoldDays, write=FOnGetBoldDays};
	__property TRzGetDayFormatEvent OnGetDayFormat = {read=FOnGetDayFormat, write=FOnGetDayFormat};
	__property TRzGetWeekNumberEvent OnGetWeekNumber = {read=FOnGetWeekNumber, write=FOnGetWeekNumber};
	__property TRzViewDateChangeEvent OnViewDateChange = {read=FOnViewDateChange, write=FOnViewDateChange};
	__property TRzCalendarTodayEvent OnToday = {read=FOnToday, write=FOnToday};
	__property TRzCalendarClearEvent OnClear = {read=FOnClear, write=FOnClear};
	__property Align = {default=0};
	__property Alignment = {default=2};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=8};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property Font;
	__property FrameController;
	__property FrameControllerNotifications = {default=65535};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEndDock;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCalendar(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzClockFaceColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	System::Uitypes::TColor FFace;
	System::Uitypes::TColor FHands;
	System::Uitypes::TColor FNumbers;
	System::Uitypes::TColor FHourTicks;
	System::Uitypes::TColor FMinuteTicks;
	TRzTimePicker* FTimePicker;
	
protected:
	virtual System::Uitypes::TColor __fastcall GetColor(int Index);
	virtual void __fastcall SetColor(int Index, System::Uitypes::TColor Value);
	
public:
	__fastcall TRzClockFaceColors(TRzTimePicker* ATimePicker);
	__fastcall virtual ~TRzClockFaceColors();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property TRzTimePicker* TimePicker = {read=FTimePicker};
	__property System::Uitypes::TColor Colors[int Index] = {read=GetColor, write=SetColor};
	
__published:
	__property System::Uitypes::TColor Face = {read=GetColor, write=SetColor, index=0, default=-16777201};
	__property System::Uitypes::TColor Hands = {read=GetColor, write=SetColor, index=1, default=-16777208};
	__property System::Uitypes::TColor Numbers = {read=GetColor, write=SetColor, index=2, default=-16777208};
	__property System::Uitypes::TColor HourTicks = {read=GetColor, write=SetColor, index=3, default=-16777200};
	__property System::Uitypes::TColor MinuteTicks = {read=GetColor, write=SetColor, index=4, default=-16777208};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzTimePicker : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	System::TTime FTime;
	bool FTimeIsPM;
	bool FRestrictMinutes;
	int FRestrictMinutesBy;
	bool FForceUpdate;
	bool FIsPopup;
	System::Types::TRect FTimeRect;
	System::Types::TRect FClockRect;
	int FRadius;
	System::Types::TPoint FClockCenter;
	System::Types::TRect FAMRect;
	System::Types::TRect FPMRect;
	System::Types::TRect FSetRect;
	System::Types::TPoint FCharSize;
	bool FMouseOverAM;
	bool FMouseOverPM;
	bool FMouseOverSet;
	bool FMouseOverClock;
	bool FPressingLeft;
	bool FPressingRight;
	System::UnicodeString FFormat;
	Vcl::Controls::TCaption FCaptionAM;
	Vcl::Controls::TCaption FCaptionPM;
	Vcl::Controls::TCaption FCaptionSet;
	System::Uitypes::TColor FButtonColor;
	System::Uitypes::TColor FButtonFontColor;
	TRzClockFaceColors* FClockFaceColors;
	bool FShowSetButton;
	bool FShowTime;
	bool FShowHowToUseHint;
	System::UnicodeString FHowToUseMsg;
	Vcl::Controls::THintWindow* FHintWnd;
	Vcl::Extctrls::TTimer* FTimer;
	int FLastMinute;
	bool FThemeAware;
	System::Classes::TNotifyEvent FOnChange;
	System::Classes::TNotifyEvent FOnSetBtnClick;
	System::Classes::TNotifyEvent FOnSetTime;
	void __fastcall CheckHintWindowHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Paint();
	System::Uitypes::TColor __fastcall GetButtonFontColor();
	virtual void __fastcall DrawClock(const System::Types::TRect &Bounds, const System::Types::TPoint &CenterPoint, int Radius);
	void __fastcall CalcRects();
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	void __fastcall AdjustForFont();
	void __fastcall CalcFontSize();
	virtual void __fastcall ConstrainedResize(int &MinWidth, int &MinHeight, int &MaxWidth, int &MaxHeight);
	System::Extended __fastcall NormalizedArcTan(const System::Extended Y, const System::Extended X);
	int __fastcall GetHourFromXY(int X, int Y);
	int __fastcall GetMinuteFromXY(int X, int Y, bool Restrict);
	System::Types::TRect __fastcall CalcHintRect(int MaxWidth, const System::UnicodeString HintStr, Vcl::Controls::THintWindow* HintWnd);
	void __fastcall DoHint(int X, int Y);
	void __fastcall ReleaseHintWindow();
	void __fastcall ChangeToAM();
	void __fastcall ChangeToPM();
	HIDESBASEDYNAMIC void __fastcall Changed();
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall SetBtnClick();
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionAM(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetCaptionPM(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetCaptionSet(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetClockFaceColors(TRzClockFaceColors* Value);
	virtual void __fastcall SetFormat(const System::UnicodeString Value);
	void __fastcall SetHour(int Value);
	void __fastcall SetMinutes(int Value);
	virtual void __fastcall SetTime(System::TTime Value);
	virtual void __fastcall SetShowSetButton(bool Value);
	virtual void __fastcall SetShowTime(bool Value);
	virtual void __fastcall SetThemeAware(bool Value);
	
public:
	__fastcall virtual TRzTimePicker(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTimePicker();
	virtual bool __fastcall CanAutoSize(int &NewWidth, int &NewHeight);
	void __fastcall AdjustHour(__int64 DeltaHours);
	void __fastcall AdjustMinute(__int64 DeltaMinutes);
	bool __fastcall IsClear();
	void __fastcall Clear();
	__property bool IsPopup = {write=FIsPopup, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=SetButtonColor, default=-16777201};
	__property System::Uitypes::TColor ButtonFontColor = {read=FButtonFontColor, write=SetButtonFontColor, default=-16777208};
	__property Vcl::Controls::TCaption CaptionAM = {read=FCaptionAM, write=SetCaptionAM};
	__property Vcl::Controls::TCaption CaptionPM = {read=FCaptionPM, write=SetCaptionPM};
	__property Vcl::Controls::TCaption CaptionSet = {read=FCaptionSet, write=SetCaptionSet};
	__property TRzClockFaceColors* ClockFaceColors = {read=FClockFaceColors, write=SetClockFaceColors};
	__property System::UnicodeString Format = {read=FFormat, write=SetFormat};
	__property System::UnicodeString HowToUseMsg = {read=FHowToUseMsg, write=FHowToUseMsg};
	__property int RestrictMinutesBy = {read=FRestrictMinutesBy, write=FRestrictMinutesBy, default=5};
	__property bool RestrictMinutes = {read=FRestrictMinutes, write=FRestrictMinutes, default=0};
	__property bool ShowHowToUseHint = {read=FShowHowToUseHint, write=FShowHowToUseHint, default=1};
	__property bool ShowSetButton = {read=FShowSetButton, write=SetShowSetButton, default=0};
	__property bool ShowTime = {read=FShowTime, write=SetShowTime, default=1};
	__property System::TTime Time = {read=FTime, write=SetTime};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property System::Classes::TNotifyEvent OnSetBtnClick = {read=FOnSetBtnClick, write=FOnSetBtnClick};
	__property System::Classes::TNotifyEvent OnSetTime = {read=FOnSetTime, write=FOnSetTime};
	__property Align = {default=0};
	__property Alignment = {default=2};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=4};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property Font;
	__property FrameController;
	__property FrameControllerNotifications = {default=65535};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEndDock;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTimePicker(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCalculatorColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	System::Uitypes::TColor FNumberFont;
	System::Uitypes::TColor FNumberBtns;
	System::Uitypes::TColor FOperatorFont;
	System::Uitypes::TColor FOperatorBtns;
	System::Uitypes::TColor FCommandFont;
	System::Uitypes::TColor FCommandBtns;
	System::Uitypes::TColor FDisplayFont;
	System::Uitypes::TColor FDisplay;
	TRzCalculator* FCalculator;
	
protected:
	virtual System::Uitypes::TColor __fastcall GetColor(int Index);
	virtual void __fastcall SetColor(int Index, System::Uitypes::TColor Value);
	
public:
	__fastcall TRzCalculatorColors(TRzCalculator* ACalculator);
	__fastcall virtual ~TRzCalculatorColors();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property TRzCalculator* Calculator = {read=FCalculator};
	__property System::Uitypes::TColor Colors[int Index] = {read=GetColor, write=SetColor};
	
__published:
	__property System::Uitypes::TColor NumberFont = {read=GetColor, write=SetColor, index=0, default=-16777208};
	__property System::Uitypes::TColor NumberBtns = {read=GetColor, write=SetColor, index=1, default=-16777201};
	__property System::Uitypes::TColor OperatorFont = {read=GetColor, write=SetColor, index=2, default=-16777208};
	__property System::Uitypes::TColor OperatorBtns = {read=GetColor, write=SetColor, index=3, default=-16777201};
	__property System::Uitypes::TColor CommandFont = {read=GetColor, write=SetColor, index=4, default=-16777208};
	__property System::Uitypes::TColor CommandBtns = {read=GetColor, write=SetColor, index=5, default=-16777201};
	__property System::Uitypes::TColor DisplayFont = {read=GetColor, write=SetColor, index=6, default=-16777208};
	__property System::Uitypes::TColor Display = {read=GetColor, write=SetColor, index=7, default=-16777211};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzCalculatorArea : unsigned char { ccBtn0, ccBtn1, ccBtn2, ccBtn3, ccBtn4, ccBtn5, ccBtn6, ccBtn7, ccBtn8, ccBtn9, ccDecimal, ccAdd, ccSubtract, ccMultiply, ccDivide, ccClear, ccEqual, ccSet, ccValue };

typedef System::StaticArray<System::Types::TRect, 19> TRzCalculatorAreas;

enum DECLSPEC_DENUM TRzCalculatorOperation : unsigned char { calcNone, calcAdd, calcSubtract, calcMultiply, calcDivide };

enum DECLSPEC_DENUM TRzCalculatorUpdateDisplay : unsigned char { udNumber, udResult };

class PASCALIMPLEMENTATION TRzCalculator : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	TRzCalculatorArea FPressedArea;
	TRzCalculatorArea FOverArea;
	bool FIsPopup;
	System::Types::TRect FMouseOverRect;
	System::Types::TPoint FCharSize;
	System::Extended FNumber;
	int FDecimalPlaces;
	TRzCalculatorOperation FCurrentOperation;
	System::Extended FResult;
	bool FFirstDigit;
	bool FFirstNumber;
	bool FShowResult;
	bool FErrorOccurred;
	bool FBoldButtons;
	TRzCalculatorColors* FCalculatorColors;
	bool FThemeAware;
	System::Classes::TNotifyEvent FOnChange;
	System::Classes::TNotifyEvent FOnSetBtnClick;
	int __fastcall GetIntResult();
	void __fastcall SetIntResult(int Value);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	virtual void __fastcall CreateHandle();
	virtual void __fastcall DrawDisplayBox(const System::Types::TRect &Bounds, System::Uitypes::TColor FrameColor);
	System::Uitypes::TColor __fastcall GetButtonColor(TRzCalculatorArea Area);
	System::Uitypes::TColor __fastcall GetButtonFontColor(TRzCalculatorArea Area);
	virtual void __fastcall DrawCalcButton(const System::Types::TRect &Bounds, TRzCalculatorArea Area);
	virtual void __fastcall Paint();
	void __fastcall CalcAreas(TRzCalculatorAreas &Areas);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	void __fastcall AdjustForFont();
	void __fastcall CalcFontSize();
	virtual void __fastcall ConstrainedResize(int &MinWidth, int &MinHeight, int &MaxWidth, int &MaxHeight);
	void __fastcall PrepareForUserInput();
	void __fastcall PrepareForOperator();
	void __fastcall UpdateDisplay(TRzCalculatorUpdateDisplay Update);
	void __fastcall ShowError();
	void __fastcall AppendDigit(int N);
	void __fastcall DecimalPressed();
	void __fastcall OperationPressed(TRzCalculatorOperation Operation);
	void __fastcall EqualPressed();
	void __fastcall CalculateNewResult();
	virtual void __fastcall SetResult(System::Extended Value);
	HIDESBASEDYNAMIC void __fastcall Changed();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall SetBtnClick();
	virtual void __fastcall SetBoldButtons(bool Value);
	virtual void __fastcall SetCalculatorColors(TRzCalculatorColors* Value);
	virtual void __fastcall SetOverArea(TRzCalculatorArea Value);
	virtual void __fastcall SetPressedArea(TRzCalculatorArea Value);
	virtual void __fastcall SetThemeAware(bool Value);
	
public:
	__fastcall virtual TRzCalculator(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCalculator();
	virtual bool __fastcall CanAutoSize(int &NewWidth, int &NewHeight);
	TRzCalculatorArea __fastcall HitTest(int X, int Y);
	void __fastcall Clear();
	__property bool IsPopup = {write=FIsPopup, nodefault};
	__property int IntResult = {read=GetIntResult, write=SetIntResult, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool BoldButtons = {read=FBoldButtons, write=SetBoldButtons, default=0};
	__property TRzCalculatorColors* CalculatorColors = {read=FCalculatorColors, write=SetCalculatorColors};
	__property System::Extended Result = {read=FResult, write=SetResult};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property System::Classes::TNotifyEvent OnSetBtnClick = {read=FOnSetBtnClick, write=FOnSetBtnClick};
	__property Align = {default=0};
	__property Alignment = {default=2};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=8};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property Font;
	__property FrameController;
	__property FrameControllerNotifications = {default=65535};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEndDock;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCalculator(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzCustomPopup : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	Vcl::Controls::TControl* FPopupControl;
	
protected:
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall Paint();
	
public:
	__fastcall virtual TRzCustomPopup(System::Classes::TComponent* AOwner);
	__property Vcl::Controls::TControl* PopupControl = {read=FPopupControl};
	
__published:
	__property BevelWidth = {default=1};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=6};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
public:
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzCustomPopup() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomPopup(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzPopupPanel : public TRzCustomPopup
{
	typedef TRzCustomPopup inherited;
	
private:
	Vcl::Controls::TWinControl* FPopup;
	System::Classes::TNotifyEvent FOnClose;
	System::Classes::TNotifyEvent FOnPopup;
	
protected:
	virtual void __fastcall DestroyWnd();
	DYNAMIC void __fastcall DoClose();
	DYNAMIC void __fastcall DoPopup();
	bool __fastcall GetActive();
	
public:
	__fastcall virtual TRzPopupPanel(System::Classes::TComponent* AOwner);
	void __fastcall Close(System::TObject* Sender = (System::TObject*)(0x0));
	bool __fastcall Popup(Vcl::Controls::TControl* PopupControl)/* overload */;
	bool __fastcall Popup(Vcl::Controls::TControl* PopupControl, int X, int Y)/* overload */;
	__property bool Active = {read=GetActive, nodefault};
	
__published:
	__property System::Classes::TNotifyEvent OnClose = {read=FOnClose, write=FOnClose};
	__property System::Classes::TNotifyEvent OnPopup = {read=FOnPopup, write=FOnPopup};
	__property Alignment = {default=1};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property ParentBiDiMode = {default=1};
	__property Color = {default=-16777201};
	__property Ctl3D;
	__property Enabled = {default=1};
	__property Font;
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property OnContextPopup;
public:
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzPopupPanel() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPopupPanel(HWND ParentWindow) : TRzCustomPopup(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::ResourceString _sRzHowToSelectTime;
#define Rzpopups_sRzHowToSelectTime System::LoadResourceString(&Rzpopups::_sRzHowToSelectTime)
extern DELPHI_PACKAGE System::ResourceString _sRzCaptionAM;
#define Rzpopups_sRzCaptionAM System::LoadResourceString(&Rzpopups::_sRzCaptionAM)
extern DELPHI_PACKAGE System::ResourceString _sRzCaptionPM;
#define Rzpopups_sRzCaptionPM System::LoadResourceString(&Rzpopups::_sRzCaptionPM)
extern DELPHI_PACKAGE System::ResourceString _sRzCaptionSet;
#define Rzpopups_sRzCaptionSet System::LoadResourceString(&Rzpopups::_sRzCaptionSet)
extern DELPHI_PACKAGE System::ResourceString _sRzCaptionClearBtn;
#define Rzpopups_sRzCaptionClearBtn System::LoadResourceString(&Rzpopups::_sRzCaptionClearBtn)
extern DELPHI_PACKAGE System::ResourceString _sRzCaptionTodayBtn;
#define Rzpopups_sRzCaptionTodayBtn System::LoadResourceString(&Rzpopups::_sRzCaptionTodayBtn)
extern DELPHI_PACKAGE System::ResourceString _sRzDateMax;
#define Rzpopups_sRzDateMax System::LoadResourceString(&Rzpopups::_sRzDateMax)
extern DELPHI_PACKAGE System::ResourceString _sRzDateMin;
#define Rzpopups_sRzDateMin System::LoadResourceString(&Rzpopups::_sRzDateMin)
extern DELPHI_PACKAGE System::ResourceString _sRzDateRange;
#define Rzpopups_sRzDateRange System::LoadResourceString(&Rzpopups::_sRzDateRange)
extern DELPHI_PACKAGE bool __fastcall StrToDateEx(const System::UnicodeString S, System::TDateTime &DateTime, Rzcommon::TRzTwoDigitYearConverter TwoDigitYearConverter = (Rzcommon::TRzTwoDigitYearConverter)(0x0));
extern DELPHI_PACKAGE bool __fastcall StrToTimeEx(const System::UnicodeString S, System::TDateTime &DateTime, const System::UnicodeString ParsingFormat = System::UnicodeString());
}	/* namespace Rzpopups */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPOPUPS)
using namespace Rzpopups;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzpopupsHPP
