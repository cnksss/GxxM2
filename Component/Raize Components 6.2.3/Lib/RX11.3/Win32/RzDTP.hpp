// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDTP.pas' rev: 35.00 (Windows)

#ifndef RzdtpHPP
#define RzdtpHPP

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
#include <Vcl.ComCtrls.hpp>
#include <Winapi.CommCtrl.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdtp
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzMonthCalColors;
class DELPHICLASS TRzDateTimePicker;
//-- type declarations -------------------------------------------------------
typedef System::Int8 TRzDTPDropRange;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzMonthCalColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzDateTimePicker* Owner;
	System::Uitypes::TColor FBackColor;
	System::Uitypes::TColor FTextColor;
	System::Uitypes::TColor FTitleBackColor;
	System::Uitypes::TColor FTitleTextColor;
	System::Uitypes::TColor FMonthBackColor;
	System::Uitypes::TColor FTrailingTextColor;
	
public:
	__fastcall TRzMonthCalColors(TRzDateTimePicker* AOwner);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	void __fastcall SetColor(int Index, System::Uitypes::TColor Value);
	void __fastcall SetAllColors();
	
__published:
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetColor, index=0, default=-16777211};
	__property System::Uitypes::TColor TextColor = {read=FTextColor, write=SetColor, index=1, default=-16777208};
	__property System::Uitypes::TColor TitleBackColor = {read=FTitleBackColor, write=SetColor, index=2, default=-16777214};
	__property System::Uitypes::TColor TitleTextColor = {read=FTitleTextColor, write=SetColor, index=3, default=-16777207};
	__property System::Uitypes::TColor MonthBackColor = {read=FMonthBackColor, write=SetColor, index=4, default=-16777211};
	__property System::Uitypes::TColor TrailingTextColor = {read=FTrailingTextColor, write=SetColor, index=5, default=8421504};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzMonthCalColors() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDateTimePicker _DEPRECATED_ATTRIBUTE0  : public Vcl::Comctrls::TDateTimePicker
{
	typedef Vcl::Comctrls::TDateTimePicker inherited;
	
private:
	bool FFlatButtons _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFlatButtonColor _DEPRECATED_ATTRIBUTE0 ;
	bool FUpdatingColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FDisabledColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFocusColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FNormalColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameColor _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TRzFrameController* FFrameController _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameHotColor _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameHotTrack _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameHotStyle _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TSides FFrameSides _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameStyle _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameVisible _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFramingPreference FFramingPreference _DEPRECATED_ATTRIBUTE0 ;
	bool FShowToday _DEPRECATED_ATTRIBUTE0 ;
	bool FShowTodayCircle _DEPRECATED_ATTRIBUTE0 ;
	bool FShowWeekNumbers _DEPRECATED_ATTRIBUTE0 ;
	TRzDTPDropRange FDropColumns _DEPRECATED_ATTRIBUTE0 ;
	TRzDTPDropRange FDropRows _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FFormat _DEPRECATED_ATTRIBUTE0 ;
	TRzMonthCalColors* FInternalCalColors _DEPRECATED_ATTRIBUTE0 ;
	void __fastcall ReadOldFrameFlatProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNotify &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo _DEPRECATED_ATTRIBUTE0 ;
	Vcl::Graphics::TCanvas* FCanvas _DEPRECATED_ATTRIBUTE0 ;
	bool FInControl _DEPRECATED_ATTRIBUTE0 ;
	bool FOverControl _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FSaveFormat _DEPRECATED_ATTRIBUTE0 ;
	virtual void __fastcall CreateWnd _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall DestroyWnd _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall DefineProperties _DEPRECATED_ATTRIBUTE0 (System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Notification _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall UpdateFrame _DEPRECATED_ATTRIBUTE0 (bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame _DEPRECATED_ATTRIBUTE0 ();
	virtual System::Types::TRect __fastcall GetEditRect _DEPRECATED_ATTRIBUTE0 ();
	virtual System::UnicodeString __fastcall GetRightJustifiedText _DEPRECATED_ATTRIBUTE0 ();
	HIDESBASE void __fastcall SetCalColors _DEPRECATED_ATTRIBUTE0 (TRzMonthCalColors* Value);
	virtual void __fastcall SetFlatButtons _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFlatButtonColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	HIDESBASE virtual void __fastcall SetFormat _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString Value);
	bool __fastcall StoreColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall StoreFocusColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall NotUsingController _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDisabledColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController _DEPRECATED_ATTRIBUTE0 (Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFrameHotStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides _DEPRECATED_ATTRIBUTE0 (Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFramingPreference _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFramingPreference Value);
	__property Vcl::Graphics::TCanvas* Canvas = {read=FCanvas};
	
public:
	__fastcall virtual TRzDateTimePicker _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDateTimePicker _DEPRECATED_ATTRIBUTE0 ();
	virtual bool __fastcall UseThemes _DEPRECATED_ATTRIBUTE0 ();
	HIDESBASE void __fastcall PaintTo _DEPRECATED_ATTRIBUTE0 (HDC DC, int X, int Y);
	bool __fastcall UpdateCalColors _DEPRECATED_ATTRIBUTE0 (int ColorIndex, System::Uitypes::TColor ColorValue);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzMonthCalColors* CalColors = {read=FInternalCalColors, write=SetCalColors};
	__property Color _DEPRECATED_ATTRIBUTE0  = {stored=StoreColor, default=-16777211};
	__property TRzDTPDropRange DropColumns = {read=FDropColumns, write=FDropColumns, default=1};
	__property TRzDTPDropRange DropRows = {read=FDropRows, write=FDropRows, default=1};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=SetFlatButtonColor, stored=NotUsingController, default=-16777201};
	__property bool FlatButtons = {read=FFlatButtons, write=SetFlatButtons, stored=NotUsingController, default=0};
	__property System::UnicodeString Format = {read=FFormat, write=SetFormat};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=NotUsingController, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=NotUsingController, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=NotUsingController, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=NotUsingController, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=NotUsingController, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=NotUsingController, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, default=1};
	__property bool ShowToday = {read=FShowToday, write=FShowToday, default=0};
	__property bool ShowTodayCircle = {read=FShowTodayCircle, write=FShowTodayCircle, default=0};
	__property bool ShowWeekNumbers = {read=FShowWeekNumbers, write=FShowWeekNumbers, default=0};
	__property FirstDayOfWeek _DEPRECATED_ATTRIBUTE0  = {default=7};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDateTimePicker(HWND ParentWindow) : Vcl::Comctrls::TDateTimePicker(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdtp */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDTP)
using namespace Rzdtp;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdtpHPP
