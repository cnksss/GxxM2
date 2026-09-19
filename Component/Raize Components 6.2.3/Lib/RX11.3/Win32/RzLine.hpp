// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLine.pas' rev: 35.00 (Windows)

#ifndef RzlineHPP
#define RzlineHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Menus.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzline
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLine;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzLineSlope : unsigned char { lsDown, lsUp };

enum DECLSPEC_DENUM TRzShowArrows : unsigned char { saNone, saStart, saEnd, saBoth };

class PASCALIMPLEMENTATION TRzLine : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	int FBorderWidth;
	System::Types::TPoint FStartPoint;
	System::Types::TPoint FEndPoint;
	System::Uitypes::TColor FLineColor;
	TRzLineSlope FLineSlope;
	Vcl::Graphics::TPenStyle FLineStyle;
	int FLineWidth;
	int FArrowLength;
	TRzShowArrows FShowArrows;
	HIDESBASE MESSAGE void __fastcall CMHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Paint();
	void __fastcall UpdateSize();
	DYNAMIC void __fastcall Resize();
	int __fastcall GetYFromX(int X, const System::Types::TPoint &Offset);
	bool __fastcall PointOnLine(const System::Types::TPoint &P);
	void __fastcall SetEndPoints();
	virtual void __fastcall SetArrowLength(int Value);
	virtual void __fastcall SetLineColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetLineSlope(TRzLineSlope Value);
	virtual void __fastcall SetLineStyle(Vcl::Graphics::TPenStyle Value);
	virtual void __fastcall SetLineWidth(int Value);
	virtual void __fastcall SetShowArrows(TRzShowArrows Value);
	
public:
	__fastcall virtual TRzLine(System::Classes::TComponent* AOwner);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property int ArrowLength = {read=FArrowLength, write=SetArrowLength, default=10};
	__property System::Uitypes::TColor LineColor = {read=FLineColor, write=SetLineColor, default=-16777208};
	__property TRzLineSlope LineSlope = {read=FLineSlope, write=SetLineSlope, default=0};
	__property Vcl::Graphics::TPenStyle LineStyle = {read=FLineStyle, write=SetLineStyle, default=0};
	__property int LineWidth = {read=FLineWidth, write=SetLineWidth, default=1};
	__property TRzShowArrows ShowArrows = {read=FShowArrows, write=SetShowArrows, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Caption = {default=0};
	__property Constraints;
	__property DragKind = {default=0};
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Font;
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TGraphicControl.Destroy */ inline __fastcall virtual ~TRzLine() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzline */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLINE)
using namespace Rzline;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlineHPP
