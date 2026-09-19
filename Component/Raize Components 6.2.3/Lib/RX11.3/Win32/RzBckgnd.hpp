// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzBckgnd.pas' rev: 35.00 (Windows)

#ifndef RzbckgndHPP
#define RzbckgndHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Buttons.hpp>
#include <RzCommon.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzGrafx.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbckgnd
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomBackground;
class DELPHICLASS TRzBackground;
class DELPHICLASS TRzSeparator;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TImageStyle : unsigned char { isCenter, isClip, isFill, isStretch, isTiled };

class PASCALIMPLEMENTATION TRzCustomBackground : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	bool FActive;
	bool FMDIActive;
	Rzcommon::TFrameStyleEx FFrameStyle;
	System::Uitypes::TColor FFrameColor;
	System::Uitypes::TColor FColorStart;
	System::Uitypes::TColor FColorStartDisp;
	System::Byte FStartRed;
	System::Byte FStartGreen;
	System::Byte FStartBlue;
	System::Uitypes::TColor FColorStop;
	System::Uitypes::TColor FColorStopDisp;
	System::Byte FStopRed;
	System::Byte FStopGreen;
	System::Byte FStopBlue;
	Rzcommon::TGradientDirection FGradientDirection;
	Rzgrafx::TSmoothFactor FGradientSmoothFactor;
	bool FShowGradient;
	Vcl::Graphics::TPicture* FImage;
	bool FShowImage;
	TImageStyle FImageStyle;
	Vcl::Graphics::TPicture* FTexture;
	bool FShowTexture;
	void *FClientInstance;
	HWND FClientHandle;
	void *FDefClientProc;
	void __fastcall EraseMDIClientBkgrnd(Winapi::Messages::TMessage &Msg);
	void __fastcall SubClassMDIClient(Vcl::Forms::TForm* AOwner);
	void __fastcall TextureChangeHandler(System::TObject* Sender);
	void __fastcall ImageChangeHandler(System::TObject* Sender);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	virtual void __fastcall Loaded();
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* AParent);
	virtual void __fastcall InvalidateBackground();
	virtual void __fastcall Paint();
	virtual void __fastcall PaintGradient(Vcl::Graphics::TCanvas* ACanvas);
	virtual void __fastcall PaintImage(Vcl::Graphics::TCanvas* ACanvas, Vcl::Graphics::TPicture* Picture);
	virtual void __fastcall PaintTexture(Vcl::Graphics::TCanvas* ACanvas, Vcl::Graphics::TPicture* Picture);
	virtual void __fastcall SetActive(bool Value);
	virtual Vcl::Controls::TAlign __fastcall GetAlign();
	HIDESBASE virtual void __fastcall SetAlign(Vcl::Controls::TAlign Value);
	virtual void __fastcall SetColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetGradientSmoothFactor(Rzgrafx::TSmoothFactor Value);
	virtual void __fastcall SetImage(Vcl::Graphics::TPicture* Value);
	virtual void __fastcall SetImageStyle(TImageStyle Value);
	virtual void __fastcall SetShowGradient(bool Value);
	virtual void __fastcall SetShowImage(bool Value);
	virtual void __fastcall SetShowTexture(bool Value);
	virtual void __fastcall SetTexture(Vcl::Graphics::TPicture* Value);
	virtual bool __fastcall GetVisible();
	HIDESBASE virtual void __fastcall SetVisible(bool Value);
	__property bool Active = {read=FActive, write=SetActive, nodefault};
	__property Vcl::Controls::TAlign Align = {read=GetAlign, write=SetAlign, nodefault};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, default=0};
	__property System::Uitypes::TColor GradientColorStart = {read=FColorStartDisp, write=SetColorStart, default=-16777190};
	__property System::Uitypes::TColor GradientColorStop = {read=FColorStopDisp, write=SetColorStop, default=-16777214};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=2};
	__property Rzgrafx::TSmoothFactor GradientSmoothFactor = {read=FGradientSmoothFactor, write=SetGradientSmoothFactor, default=1};
	__property Vcl::Graphics::TPicture* Image = {read=FImage, write=SetImage};
	__property TImageStyle ImageStyle = {read=FImageStyle, write=SetImageStyle, stored=true, nodefault};
	__property bool ShowGradient = {read=FShowGradient, write=SetShowGradient, stored=true, nodefault};
	__property bool ShowImage = {read=FShowImage, write=SetShowImage, stored=true, nodefault};
	__property bool ShowTexture = {read=FShowTexture, write=SetShowTexture, stored=true, nodefault};
	__property Vcl::Graphics::TPicture* Texture = {read=FTexture, write=SetTexture, stored=true};
	__property bool Visible = {read=GetVisible, write=SetVisible, default=1};
	
public:
	__fastcall virtual TRzCustomBackground(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomBackground();
};


class PASCALIMPLEMENTATION TRzBackground : public TRzCustomBackground
{
	typedef TRzCustomBackground inherited;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Active;
	__property Align;
	__property Anchors = {default=3};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FrameColor = {default=-16777200};
	__property FrameStyle = {default=0};
	__property GradientColorStart = {default=-16777190};
	__property GradientColorStop = {default=-16777214};
	__property GradientDirection = {default=2};
	__property GradientSmoothFactor = {default=1};
	__property Image;
	__property ImageStyle;
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowGradient;
	__property ShowHint;
	__property ShowImage;
	__property ShowTexture;
	__property Texture;
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
public:
	/* TRzCustomBackground.Create */ inline __fastcall virtual TRzBackground(System::Classes::TComponent* AOwner) : TRzCustomBackground(AOwner) { }
	/* TRzCustomBackground.Destroy */ inline __fastcall virtual ~TRzBackground() { }
	
};


enum DECLSPEC_DENUM TRzHighlightLocation : unsigned char { hlCenter, hlUpperLeft, hlLowerRight };

class PASCALIMPLEMENTATION TRzSeparator : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::Uitypes::TColor FHighlightColor;
	TRzHighlightLocation FHighlightLocation;
	Rzcommon::TOrientation FOrientation;
	bool FShowGradient;
	Rzcommon::TGradientDirection FGradientDirection;
	System::Uitypes::TColor FStartColor;
	System::Uitypes::TColor FStopColor;
	bool FSetInitialSizeRequired;
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall Resize();
	virtual void __fastcall Paint();
	void __fastcall UpdateGradientDetails();
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHighlightLocation(TRzHighlightLocation Value);
	virtual void __fastcall SetShowGradient(bool Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	
public:
	__fastcall virtual TRzSeparator(System::Classes::TComponent* AOwner);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property TRzHighlightLocation HighlightLocation = {read=FHighlightLocation, write=SetHighlightLocation, default=0};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property bool ShowGradient = {read=FShowGradient, write=SetShowGradient, stored=true, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Color = {stored=true, default=-16777211};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Height = {default=2};
	__property ParentColor = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Visible = {default=1};
	__property Width = {default=200};
	__property OnClick;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
public:
	/* TGraphicControl.Destroy */ inline __fastcall virtual ~TRzSeparator() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzbckgnd */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBCKGND)
using namespace Rzbckgnd;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbckgndHPP
