// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzGrids.pas' rev: 35.00 (Windows)

#ifndef RzgridsHPP
#define RzgridsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Grids.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzgrids
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzStringGrid;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzStringGrid : public Vcl::Grids::TStringGrid
{
	typedef Vcl::Grids::TStringGrid inherited;
	
private:
	System::Uitypes::TColor FLineColor;
	System::Uitypes::TColor FFixedLineColor;
	bool FUseDrawingStyle;
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
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Graphics::TCanvas* FCanvas;
	bool FOverControl;
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual Vcl::Grids::TInplaceEdit* __fastcall CreateEditor();
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	HIDESBASE bool __fastcall IsActiveControl();
	virtual void __fastcall Paint();
	HIDESBASE void __fastcall GridRectToScreenRect(const Vcl::Grids::TGridRect &GridRect, System::Types::TRect &ScreenRect, bool IncludeLine);
	virtual Vcl::Grids::TGridRect __fastcall GetFocusSelection();
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
	virtual void __fastcall SetFixedLineColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetLineColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetUseDrawingStyle(bool Value);
	
public:
	__fastcall virtual TRzStringGrid(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzStringGrid();
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
	__property System::Uitypes::TColor FixedLineColor = {read=FFixedLineColor, write=SetFixedLineColor, default=-16777200};
	__property System::Uitypes::TColor LineColor = {read=FLineColor, write=SetLineColor, default=-16777201};
	__property bool UseDrawingStyle = {read=FUseDrawingStyle, write=SetUseDrawingStyle, default=1};
	__property DefaultColWidth = {default=60};
	__property DefaultRowHeight = {default=18};
	__property Height = {default=125};
	__property Width = {default=325};
	__property OnResize;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzStringGrid(HWND ParentWindow) : Vcl::Grids::TStringGrid(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzgrids */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZGRIDS)
using namespace Rzgrids;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzgridsHPP
