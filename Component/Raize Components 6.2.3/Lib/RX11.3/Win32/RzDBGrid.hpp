// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBGrid.pas' rev: 35.00 (Windows)

#ifndef RzdbgridHPP
#define RzdbgridHPP

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
#include <Vcl.DBGrids.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbgrid
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBGridQuickCompare;
class DELPHICLASS TRzDBGrid;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzQuickCompareOperation : unsigned char { qcoEqual, qcoLessThan, qcoLessThanOrEqual, qcoGreaterThan, qcoGreaterThanOrEqual, qcoNotEqual };

class PASCALIMPLEMENTATION TRzDBGridQuickCompare : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzDBGrid* FGrid;
	bool FActive;
	System::UnicodeString FFieldName;
	System::Variant FFieldValue;
	TRzQuickCompareOperation FOperation;
	System::Uitypes::TColor FColor;
	System::Uitypes::TColor FFontColor;
	
protected:
	void __fastcall SetActive(bool Value);
	void __fastcall SetFieldName(const System::UnicodeString Value);
	void __fastcall SetFieldValue(const System::Variant &Value);
	bool __fastcall StoreFieldValue();
	void __fastcall SetOperation(TRzQuickCompareOperation Value);
	void __fastcall SetColor(System::Uitypes::TColor Value);
	void __fastcall SetFontColor(System::Uitypes::TColor Value);
	
public:
	__fastcall TRzDBGridQuickCompare();
	__property TRzDBGrid* Grid = {read=FGrid};
	
__published:
	__property bool Active = {read=FActive, write=SetActive, default=0};
	__property System::UnicodeString FieldName = {read=FFieldName, write=SetFieldName};
	__property System::Variant FieldValue = {read=FFieldValue, write=SetFieldValue, stored=StoreFieldValue};
	__property TRzQuickCompareOperation Operation = {read=FOperation, write=SetOperation, default=0};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, default=10053222};
	__property System::Uitypes::TColor FontColor = {read=FFontColor, write=SetFontColor, default=16777215};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzDBGridQuickCompare() { }
	
};


class PASCALIMPLEMENTATION TRzDBGrid : public Vcl::Dbgrids::TDBGrid
{
	typedef Vcl::Dbgrids::TDBGrid inherited;
	
private:
	System::Uitypes::TColor FLineColor;
	System::Uitypes::TColor FFixedLineColor;
	TRzDBGridQuickCompare* FQuickCompare;
	bool FAltRowShading;
	System::Uitypes::TColor FAltRowShadingColor;
	bool FAltRowShadingFixed;
	bool FLocalDefaultDrawing;
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
	HIDESBASE bool __fastcall AcquireFocus();
	HIDESBASE MESSAGE void __fastcall WMVScroll(Winapi::Messages::TWMScroll &Msg);
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
	DYNAMIC void __fastcall DrawColumnCell(const System::Types::TRect &Rect, int DataCol, Vcl::Dbgrids::TColumn* Column, Vcl::Grids::TGridDrawState State);
	virtual void __fastcall Scroll(int Distance);
	virtual void __fastcall UpdateScrollBar();
	DYNAMIC void __fastcall Resize();
	HIDESBASE bool __fastcall IsActiveControl();
	virtual void __fastcall Paint();
	HIDESBASE void __fastcall GridRectToScreenRect(const Vcl::Grids::TGridRect &GridRect, System::Types::TRect &ScreenRect, bool IncludeLine);
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
	virtual void __fastcall SetQuickCompare(TRzDBGridQuickCompare* Value);
	void __fastcall SetAltRowShading(bool Value);
	void __fastcall SetAltRowShadingColor(System::Uitypes::TColor Value);
	void __fastcall SetAltRowShadingFixed(bool Value);
	
public:
	__fastcall virtual TRzDBGrid(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBGrid();
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
	__property TRzDBGridQuickCompare* QuickCompare = {read=FQuickCompare, write=SetQuickCompare};
	__property bool AltRowShading = {read=FAltRowShading, write=SetAltRowShading, default=0};
	__property System::Uitypes::TColor AltRowShadingColor = {read=FAltRowShadingColor, write=SetAltRowShadingColor, default=15463915};
	__property bool AltRowShadingFixed = {read=FAltRowShadingFixed, write=SetAltRowShadingFixed, default=1};
	__property bool DefaultDrawing = {read=FLocalDefaultDrawing, write=FLocalDefaultDrawing, default=1};
	__property Height = {default=125};
	__property Width = {default=325};
	__property OnResize;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBGrid(HWND ParentWindow) : Vcl::Dbgrids::TDBGrid(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Uitypes::TColor clFadedGreen = System::Uitypes::TColor(15463915);
static const System::Uitypes::TColor clBlueGray = System::Uitypes::TColor(10053222);
}	/* namespace Rzdbgrid */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBGRID)
using namespace Rzdbgrid;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbgridHPP
