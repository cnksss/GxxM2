// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzBHints.pas' rev: 35.00 (Windows)

#ifndef RzbhintsHPP
#define RzbhintsHPP

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
#include <Vcl.Forms.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <RzGrafx.hpp>
#include <System.Types.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbhints
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzBalloonBitmaps;
class DELPHICLASS TRzCustomHintWindow;
class DELPHICLASS TRzBalloonHints;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzHintCorner : unsigned char { hcLowerRight, hcLowerLeft, hcUpperLeft, hcUpperRight, hcNone };

typedef void __fastcall (__closure *TSetHintWinSizeEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* Canvas, int &Width, int &Height, System::UnicodeString Hint, TRzHintCorner Corner);

typedef void __fastcall (__closure *TSetHintRectEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* Canvas, System::Types::TRect &Rect, System::UnicodeString Hint, TRzHintCorner Corner);

typedef TRzBalloonBitmaps* *PRzBalloonBitmaps;

class PASCALIMPLEMENTATION TRzBalloonBitmaps : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	Vcl::Graphics::TBitmap* FLowerRight;
	Vcl::Graphics::TBitmap* FLowerLeft;
	Vcl::Graphics::TBitmap* FUpperLeft;
	Vcl::Graphics::TBitmap* FUpperRight;
	System::Uitypes::TColor FTransparentColor;
	System::Classes::TNotifyEvent FOnChange;
	
protected:
	virtual void __fastcall SetLowerRight(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetLowerLeft(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetUpperLeft(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetUpperRight(Vcl::Graphics::TBitmap* Value);
	
public:
	__fastcall TRzBalloonBitmaps();
	__fastcall virtual ~TRzBalloonBitmaps();
	
__published:
	__property Vcl::Graphics::TBitmap* LowerRight = {read=FLowerRight, write=SetLowerRight};
	__property Vcl::Graphics::TBitmap* LowerLeft = {read=FLowerLeft, write=SetLowerLeft};
	__property Vcl::Graphics::TBitmap* UpperLeft = {read=FUpperLeft, write=SetUpperLeft};
	__property Vcl::Graphics::TBitmap* UpperRight = {read=FUpperRight, write=SetUpperRight};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=FTransparentColor, nodefault};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
};


class PASCALIMPLEMENTATION TRzCustomHintWindow : public Vcl::Controls::THintWindow
{
	typedef Vcl::Controls::THintWindow inherited;
	
private:
	bool FHintActive;
	Vcl::Forms::TApplication* FApplication;
	TRzBalloonHints* FBalloonHints;
	TRzBalloonBitmaps* *FBitmaps;
	System::UnicodeString FCaption;
	int FCaptionWidth;
	System::Uitypes::TColor FColor;
	System::Uitypes::TColor FFrameColor;
	Vcl::Controls::THintInfo FHintInfo;
	Vcl::Graphics::TFont* FFont;
	System::Classes::TAlignment FAlignment;
	TRzHintCorner FCorner;
	bool FShowBalloon;
	int FCenterThreshold;
	TRzHintCorner FDrawCorner;
	System::Types::TRect FHintRect;
	TSetHintWinSizeEvent FOnSetHintWinSize;
	TSetHintRectEvent FOnSetHintRect;
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCCalcSize(Winapi::Messages::TWMNCCalcSize &Msg);
	
protected:
	virtual void __fastcall Paint();
	virtual void __fastcall NCPaint(HDC DC);
	HRGN __fastcall GetHintRegion();
	void __fastcall AdjustBoundsForPointer(System::Types::TRect &Bounds, bool Shrink);
	void __fastcall DrawHintText(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);
	void __fastcall GetPointerCoordinates(const System::Types::TRect &Bounds, const System::Types::TRect &HintRect, System::Types::TPoint &Pt1, System::Types::TPoint &Pt2, System::Types::TPoint &Pt3, System::Types::TRect &FillLineRect);
	void __fastcall DrawHint(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);
	void __fastcall DrawBitmapBalloon(Vcl::Graphics::TCanvas* Canvas);
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &msg);
	HIDESBASE void __fastcall SetColor(System::Uitypes::TColor Value);
	HIDESBASE void __fastcall SetFont(Vcl::Graphics::TFont* Value);
	
public:
	__fastcall virtual TRzCustomHintWindow(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomHintWindow();
	virtual void __fastcall ActivateHint(const System::Types::TRect &Rect, const System::UnicodeString AHint);
	void __fastcall DoShowHint(System::UnicodeString &HintStr, bool &CanShow, Vcl::Controls::THintInfo &HintInfo);
	void __fastcall BitmapChanged(System::TObject* Sender);
	__property PRzBalloonBitmaps Bitmaps = {read=FBitmaps, write=FBitmaps};
	
__published:
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, nodefault};
	__property int CaptionWidth = {read=FCaptionWidth, write=FCaptionWidth, nodefault};
	__property System::UnicodeString Caption = {read=FCaption, write=FCaption};
	__property int CenterThreshold = {read=FCenterThreshold, write=FCenterThreshold, default=80};
	__property Vcl::Graphics::TFont* Font = {read=FFont, write=SetFont};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=FFrameColor, nodefault};
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=FAlignment, nodefault};
	__property TRzHintCorner Corner = {read=FCorner, write=FCorner, nodefault};
	__property bool ShowBalloon = {read=FShowBalloon, write=FShowBalloon, nodefault};
	__property TSetHintWinSizeEvent OnSetHintWinSize = {read=FOnSetHintWinSize, write=FOnSetHintWinSize};
	__property TSetHintRectEvent OnSetHintRect = {read=FOnSetHintRect, write=FOnSetHintRect};
	__property Canvas;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomHintWindow(HWND ParentWindow) : Vcl::Controls::THintWindow(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzBalloonHints : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	TRzCustomHintWindow* FHintWindow;
	Vcl::Controls::THintWindowClass FOrigHintWindowClass;
	TRzBalloonBitmaps* FBitmaps;
	int FCaptionWidth;
	System::Uitypes::TColor FColor;
	System::Uitypes::TColor FFrameColor;
	Vcl::Graphics::TFont* FFont;
	System::Classes::TAlignment FAlignment;
	TRzHintCorner FCorner;
	bool FShowBalloon;
	int FHintPause;
	int FHintShortPause;
	int FCenterThreshold;
	TSetHintWinSizeEvent FOnSetHintWinSize;
	TSetHintRectEvent FOnSetHintRect;
	Vcl::Forms::TShowHintEvent FOnShowHint;
	void __fastcall FontChanged(System::TObject* Sender);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall SetFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetCenterThreshold(int Value);
	virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionWidth(int Value);
	virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetCorner(TRzHintCorner Value);
	virtual void __fastcall SetShowBalloon(bool Value);
	virtual void __fastcall SetOnSetHintWinSize(TSetHintWinSizeEvent Value);
	virtual void __fastcall SetOnSetHintRect(TSetHintRectEvent Value);
	virtual int __fastcall GetHintPause();
	virtual void __fastcall SetHintPause(int Value);
	virtual int __fastcall GetHintShortPause();
	virtual void __fastcall SetHintShortPause(int Value);
	
public:
	__fastcall virtual TRzBalloonHints(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzBalloonHints();
	void __fastcall MakeConnection(TRzCustomHintWindow* HintWindow);
	void __fastcall BreakConnection(TRzCustomHintWindow* HintWindow);
	__property TRzCustomHintWindow* HintWindow = {read=FHintWindow};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=0};
	__property TRzBalloonBitmaps* Bitmaps = {read=FBitmaps, write=FBitmaps};
	__property int CaptionWidth = {read=FCaptionWidth, write=SetCaptionWidth, default=100};
	__property int CenterThreshold = {read=FCenterThreshold, write=SetCenterThreshold, default=80};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, default=-16777192};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777193};
	__property TRzHintCorner Corner = {read=FCorner, write=SetCorner, default=3};
	__property Vcl::Graphics::TFont* Font = {read=FFont, write=SetFont};
	__property int HintPause = {read=GetHintPause, write=SetHintPause, default=500};
	__property int HintShortPause = {read=GetHintShortPause, write=SetHintShortPause, default=0};
	__property bool ShowBalloon = {read=FShowBalloon, write=SetShowBalloon, default=1};
	__property TSetHintWinSizeEvent OnSetHintWinSize = {read=FOnSetHintWinSize, write=SetOnSetHintWinSize};
	__property TSetHintRectEvent OnSetHintRect = {read=FOnSetHintRect, write=SetOnSetHintRect};
	__property Vcl::Forms::TShowHintEvent OnShowHint = {read=FOnShowHint, write=FOnShowHint};
};


//-- var, const, procedure ---------------------------------------------------
static const System::Int8 MinWindowWidth = System::Int8(0x37);
static const System::Int8 MouseAdj = System::Int8(0x2);
}	/* namespace Rzbhints */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBHINTS)
using namespace Rzbhints;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbhintsHPP
