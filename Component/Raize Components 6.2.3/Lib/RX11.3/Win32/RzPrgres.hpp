// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPrgres.pas' rev: 35.00 (Windows)

#ifndef RzprgresHPP
#define RzprgresHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzprgres
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomProgressBar;
class DELPHICLASS TRzProgressBar;
//-- type declarations -------------------------------------------------------
typedef System::Int8 TSegmentRange;

typedef int TUnsignedLongint;

typedef void __fastcall (__closure *TProgressChangeEvent)(System::TObject* Sender, int Percent);

class PASCALIMPLEMENTATION TRzCustomProgressBar : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	System::Uitypes::TColor FBackColor;
	System::Uitypes::TColor FBackColorStop;
	System::Uitypes::TColor FBarColor;
	System::Uitypes::TColor FBarColorStop;
	Rzcommon::TBarStyle FBarStyle;
	Vcl::Controls::TBevelWidth FBevelWidth;
	System::Uitypes::TColor FBorderColor;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Vcl::Controls::TBorderWidth FBorderWidth;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TGradientDirection FGradientDirection;
	System::Byte FInteriorOffset;
	TSegmentRange FNumSegments;
	Rzcommon::TOrientation FOrientation;
	TUnsignedLongint FPartsComplete;
	System::Word FPercent;
	bool FShowPercent;
	bool FShowParts;
	TUnsignedLongint FTotalParts;
	bool FThemeAware;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	TProgressChangeEvent FOnChange;
	HIDESBASE MESSAGE void __fastcall CMHitTest(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	DYNAMIC void __fastcall PercentChanged();
	virtual void __fastcall SetBackColor(System::Uitypes::TColor Value);
	void __fastcall SetBackColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarColor(System::Uitypes::TColor Value);
	void __fastcall SetBarColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarStyle(Rzcommon::TBarStyle Value);
	virtual void __fastcall SetBevelWidth(Vcl::Controls::TBevelWidth Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetInteriorOffset(System::Byte Value);
	virtual void __fastcall SetNumSegments(TSegmentRange Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetPartsComplete(TUnsignedLongint Value);
	virtual void __fastcall SetPercent(System::Word Value);
	virtual void __fastcall SetShowPercent(bool Value);
	virtual void __fastcall SetShowParts(bool Value);
	virtual void __fastcall SetTotalParts(TUnsignedLongint Value);
	virtual void __fastcall SetThemeAware(bool Value);
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetBackColor, default=16777215};
	__property System::Uitypes::TColor BackColorStop = {read=FBackColorStop, write=SetBackColorStop, default=-16777201};
	__property System::Uitypes::TColor BarColor = {read=FBarColor, write=SetBarColor, default=-16777203};
	__property System::Uitypes::TColor BarColorStop = {read=FBarColorStop, write=SetBarColorStop, default=-16777190};
	__property Rzcommon::TBarStyle BarStyle = {read=FBarStyle, write=SetBarStyle, default=0};
	__property Vcl::Controls::TBevelWidth BevelWidth = {read=FBevelWidth, write=SetBevelWidth, default=1};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=4};
	__property Vcl::Controls::TBorderWidth BorderWidth = {read=FBorderWidth, write=SetBorderWidth, nodefault};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=3};
	__property System::Byte InteriorOffset = {read=FInteriorOffset, write=SetInteriorOffset, nodefault};
	__property TSegmentRange NumSegments = {read=FNumSegments, write=SetNumSegments, default=20};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property TUnsignedLongint PartsComplete = {read=FPartsComplete, write=SetPartsComplete, nodefault};
	__property System::Word Percent = {read=FPercent, write=SetPercent, nodefault};
	__property bool ShowPercent = {read=FShowPercent, write=SetShowPercent, default=1};
	__property bool ShowParts = {read=FShowParts, write=SetShowParts, default=0};
	__property TUnsignedLongint TotalParts = {read=FTotalParts, write=SetTotalParts, nodefault};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property TProgressChangeEvent OnChange = {read=FOnChange, write=FOnChange};
	
public:
	__fastcall virtual TRzCustomProgressBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomProgressBar();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	bool __fastcall UseThemes();
	__property Align = {default=0};
	__property Font;
	__property Height = {default=24};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Visible = {default=1};
	__property Width = {default=200};
	__property OnClick;
	__property OnDblClick;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
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


class PASCALIMPLEMENTATION TRzProgressBar : public TRzCustomProgressBar
{
	typedef TRzCustomProgressBar inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
public:
	void __fastcall IncPartsByOne();
	void __fastcall IncParts(TUnsignedLongint N);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BackColor = {default=16777215};
	__property BackColorStop = {default=-16777201};
	__property BarColor = {default=-16777203};
	__property BarColorStop = {default=-16777190};
	__property BarStyle = {default=0};
	__property BevelWidth = {default=1};
	__property BorderColor = {default=-16777201};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=4};
	__property BorderWidth;
	__property Constraints;
	__property DragKind = {default=0};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property GradientDirection = {default=3};
	__property Height = {default=24};
	__property InteriorOffset;
	__property NumSegments = {default=20};
	__property Orientation = {default=0};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PartsComplete;
	__property Percent;
	__property PopupMenu;
	__property ShowHint;
	__property ShowParts = {default=0};
	__property ShowPercent = {default=1};
	__property ThemeAware = {default=1};
	__property TotalParts;
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=200};
	__property OnChange;
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnEndDock;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
public:
	/* TRzCustomProgressBar.Create */ inline __fastcall virtual TRzProgressBar(System::Classes::TComponent* AOwner) : TRzCustomProgressBar(AOwner) { }
	/* TRzCustomProgressBar.Destroy */ inline __fastcall virtual ~TRzProgressBar() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzprgres */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPRGRES)
using namespace Rzprgres;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzprgresHPP
