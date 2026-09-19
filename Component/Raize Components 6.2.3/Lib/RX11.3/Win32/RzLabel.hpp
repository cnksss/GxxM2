// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLabel.pas' rev: 35.00 (Windows)

#ifndef RzlabelHPP
#define RzlabelHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <RzCommon.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlabel
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLabel;
class DELPHICLASS TRzURLLabel;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRotation : unsigned char { roNone, roFlat, roCurve };

enum DECLSPEC_DENUM TCenterPoint : unsigned char { cpUpperLeft, cpUpperCenter, cpUpperRight, cpLeftCenter, cpCenter, cpRightCenter, cpLowerLeft, cpLowerCenter, cpLowerRight };

enum DECLSPEC_DENUM TRzCondenseCaption : unsigned char { ccNone, ccAtEnd, ccWithinPath };

class PASCALIMPLEMENTATION TRzLabel : public Vcl::Stdctrls::TLabel
{
	typedef Vcl::Stdctrls::TLabel inherited;
	
private:
	int FAngle;
	System::Uitypes::TColor FBlinkColor;
	Rzcommon::TBlinkState FBlinkState;
	bool FBlinking;
	int FBevelWidth;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	int FBorderWidth;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	System::Uitypes::TColor FFontColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	TCenterPoint FCenterPoint;
	System::Uitypes::TColor FOrigColor;
	bool FInitColor;
	bool FFlyByEnabled;
	System::Uitypes::TColor FFlyByColor;
	bool FLightTextStyle;
	Rzcommon::TTextStyle FTextStyle;
	TRotation FRotation;
	System::Uitypes::TColor FHighlightColor;
	System::Uitypes::TColor FShadowColor;
	int FShadowDepth;
	int FTextMargin;
	TRzCondenseCaption FCondenseCaption;
	MESSAGE void __fastcall CMGetBlinking(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMBlink(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::Uitypes::TColor FCurrentColor;
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall Blink(Rzcommon::TBlinkState State);
	virtual System::UnicodeString __fastcall GetLabelText();
	virtual void __fastcall AdjustForBorders(System::Types::TRect &R);
	DYNAMIC void __fastcall AdjustBounds();
	System::Types::TPoint __fastcall CalcCenterPoint();
	System::Types::TPoint __fastcall CalcRadius();
	virtual void __fastcall FixClientRect(System::Types::TRect &Rect, bool ShrinkByBorder);
	void __fastcall DrawArcText();
	void __fastcall AdjustForLayout(unsigned Flags, System::Types::TRect &R);
	void __fastcall Draw3DText();
	virtual void __fastcall SetAngle(int Value);
	virtual void __fastcall SetBlinking(bool Value);
	virtual void __fastcall SetBlinkColor(System::Uitypes::TColor Value);
	virtual System::Word __fastcall GetBlinkIntervalOff();
	virtual void __fastcall SetBlinkIntervalOff(System::Word Value);
	virtual System::Word __fastcall GetBlinkIntervalOn();
	virtual void __fastcall SetBlinkIntervalOn(System::Word Value);
	virtual void __fastcall SetBevelWidth(int Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderWidth(int Value);
	virtual void __fastcall SetCondenseCaption(TRzCondenseCaption Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetCenterPoint(TCenterPoint Value);
	virtual void __fastcall SetFlyByColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlyByEnabled(bool Value);
	virtual Vcl::Stdctrls::TTextLayout __fastcall GetLayout();
	HIDESBASE virtual void __fastcall SetLayout(Vcl::Stdctrls::TTextLayout Value);
	virtual void __fastcall SetLightTextStyle(bool Value);
	virtual void __fastcall SetRotation(TRotation Value);
	virtual void __fastcall SetTextMargin(int Value);
	virtual void __fastcall SetTextStyle(Rzcommon::TTextStyle Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetShadowDepth(int Value);
	
public:
	__fastcall virtual TRzLabel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzLabel();
	__property Rzcommon::TBlinkState BlinkState = {read=FBlinkState, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property int Angle = {read=FAngle, write=SetAngle, default=0};
	__property bool Blinking = {read=FBlinking, write=SetBlinking, default=0};
	__property System::Uitypes::TColor BlinkColor = {read=FBlinkColor, write=SetBlinkColor, default=-16777203};
	__property System::Word BlinkIntervalOff = {read=GetBlinkIntervalOff, write=SetBlinkIntervalOff, default=500};
	__property System::Word BlinkIntervalOn = {read=GetBlinkIntervalOn, write=SetBlinkIntervalOn, default=500};
	__property int BevelWidth = {read=FBevelWidth, write=SetBevelWidth, default=1};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=0};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property int BorderWidth = {read=FBorderWidth, write=SetBorderWidth, default=0};
	__property TRzCondenseCaption CondenseCaption = {read=FCondenseCaption, write=SetCondenseCaption, default=0};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property TCenterPoint CenterPoint = {read=FCenterPoint, write=SetCenterPoint, default=4};
	__property System::Uitypes::TColor FlyByColor = {read=FFlyByColor, write=SetFlyByColor, default=-16777203};
	__property bool FlyByEnabled = {read=FFlyByEnabled, write=SetFlyByEnabled, default=0};
	__property Vcl::Stdctrls::TTextLayout Layout = {read=GetLayout, write=SetLayout, default=0};
	__property bool LightTextStyle = {read=FLightTextStyle, write=SetLightTextStyle, default=0};
	__property TRotation Rotation = {read=FRotation, write=SetRotation, default=0};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777196};
	__property System::Uitypes::TColor ShadowColor = {read=FShadowColor, write=SetShadowColor, default=-16777200};
	__property int ShadowDepth = {read=FShadowDepth, write=SetShadowDepth, default=2};
	__property int TextMargin = {read=FTextMargin, write=SetTextMargin, default=0};
	__property Rzcommon::TTextStyle TextStyle = {read=FTextStyle, write=SetTextStyle, default=0};
	__property Color = {default=-16777201};
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


class PASCALIMPLEMENTATION TRzURLLabel : public TRzLabel
{
	typedef TRzLabel inherited;
	
private:
	System::UnicodeString FURL;
	bool FVisited;
	System::Uitypes::TColor FUnvisitedColor;
	System::Uitypes::TColor FVisitedColor;
	
protected:
	virtual void __fastcall UpdateFontColor();
	virtual void __fastcall SetVisited(bool Value);
	virtual void __fastcall SetUnvisitedColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetVisitedColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzURLLabel(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall Click();
	
__published:
	__property System::UnicodeString URL = {read=FURL, write=FURL};
	__property System::Uitypes::TColor UnvisitedColor = {read=FUnvisitedColor, write=SetUnvisitedColor, default=-16777203};
	__property bool Visited = {read=FVisited, write=SetVisited, default=0};
	__property System::Uitypes::TColor VisitedColor = {read=FVisitedColor, write=SetVisitedColor, default=8388736};
	__property Cursor = {default=26000};
	__property TextStyle = {default=0};
public:
	/* TRzLabel.Destroy */ inline __fastcall virtual ~TRzURLLabel() { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word crRzHandPoint = System::Word(0x6590);
extern DELPHI_PACKAGE System::StaticArray<System::Word, 3> CondenseCaptionFlags;
}	/* namespace Rzlabel */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLABEL)
using namespace Rzlabel;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlabelHPP
