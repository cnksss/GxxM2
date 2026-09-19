// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzGrafx.pas' rev: 35.00 (Windows)

#ifndef RzgrafxHPP
#define RzgrafxHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <System.SysUtils.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzgrafx
{
//-- forward type declarations -----------------------------------------------
struct TDWordColor;
struct TDWordWord;
struct TTrap;
//-- type declarations -------------------------------------------------------
typedef System::Int8 TSmoothFactor;

struct DECLSPEC_DRECORD TDWordColor
{
public:
	System::Word ColorFract;
	System::Byte Color;
	System::Byte Pad;
};


struct DECLSPEC_DRECORD TDWordWord
{
public:
	System::Word FractPart;
	System::Word WordPart;
};


enum DECLSPEC_DENUM TTrapBevelStyle : unsigned char { bsNone, bsLowered, bsRaised };

enum DECLSPEC_DENUM TTrapDirect : unsigned char { tdLargeToSmall, tdSmallToLarge };

enum DECLSPEC_DENUM TTrapShape : unsigned char { tsLeft, tsCenter, tsRight };

struct DECLSPEC_DRECORD TTrap
{
public:
	System::Types::TPoint UpperLeft;
	System::Types::TPoint UpperRight;
	System::Types::TPoint LowerRight;
	System::Types::TPoint LowerLeft;
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE void __fastcall DrawFullTransparentBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* OrigBitmap, const System::Types::TRect &Rect, const System::Types::TRect &Src, System::Uitypes::TColor TransparentColor);
extern DELPHI_PACKAGE void __fastcall DrawTransparentBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* OrigBitmap, const System::Types::TRect &Rect, const System::Types::TRect &Src, System::Uitypes::TColor TransparentColor);
extern DELPHI_PACKAGE void __fastcall TileTransparentBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect, System::Uitypes::TColor TransparentColor);
extern DELPHI_PACKAGE void __fastcall StretchTransparentBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect, System::Uitypes::TColor TransparentColor);
extern DELPHI_PACKAGE void __fastcall FlipBitmap(Vcl::Graphics::TBitmap* Bitmap);
extern DELPHI_PACKAGE void __fastcall TileBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect);
extern DELPHI_PACKAGE void __fastcall TileRealizedBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect);
extern DELPHI_PACKAGE void __fastcall CenterBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect);
extern DELPHI_PACKAGE void __fastcall StretchBitmap(Vcl::Graphics::TCanvas* Canvas, Vcl::Graphics::TBitmap* Bitmap, const System::Types::TRect &Rect);
extern DELPHI_PACKAGE void __fastcall BitFillBlit(HDC DC, int X0, int Y0, int W0, int H0, int W, int H, unsigned ROP);
extern DELPHI_PACKAGE void __fastcall BoxBitMirrorBlit(HDC DC, int X0, int Y0, int W0, int H0, unsigned ROP);
extern DELPHI_PACKAGE void __fastcall PaintGradient(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, Rzcommon::TGradientDirection GradDir, System::Uitypes::TColor ColorStart, System::Uitypes::TColor ColorStop, TSmoothFactor SmoothFactor = (TSmoothFactor)(0x1));
extern DELPHI_PACKAGE void __fastcall FillGrad(HDC DC, int AX, int AY, int L, System::Uitypes::TColor ColorStart, System::Uitypes::TColor ColorStop, TSmoothFactor SmoothFactor = (TSmoothFactor)(0x1));
extern DELPHI_PACKAGE void __fastcall FillGradRect(HDC DC, int AX, int AY, int AX0, int AY0, System::Uitypes::TColor ColorStart, System::Uitypes::TColor ColorStop, TSmoothFactor SmoothFactor = (TSmoothFactor)(0x1));
extern DELPHI_PACKAGE void __fastcall FillGradDiag(HDC DC, int AW, int AH, System::Uitypes::TColor ColorStart, System::Uitypes::TColor ColorStop, TSmoothFactor SmoothFactor = (TSmoothFactor)(0x1));
extern DELPHI_PACKAGE void __fastcall DrawTrapezoidBorder(Vcl::Graphics::TCanvas* Canvas, const TTrap &Trap, TTrapBevelStyle Style, int BevelWidth);
extern DELPHI_PACKAGE void __fastcall SetTrapezoid(TTrap &Trap, const System::Types::TRect &Rect, int TrapMin, TTrapShape TrapShape, Rzcommon::TDirection Direct, TTrapDirect TrapDirect);
extern DELPHI_PACKAGE TTrap __fastcall AdjustTrapezoid(TTrap &Trap, const System::Types::TRect &Rect, Rzcommon::TDirection Direct);
extern DELPHI_PACKAGE System::Types::TPoint __fastcall Intersect(const System::Types::TPoint &Pt1, const System::Types::TPoint &Pt2, const System::Types::TPoint &Pt3, Rzcommon::TDirection Direct);
extern DELPHI_PACKAGE void __fastcall InflateTrapezoid(TTrap &Trap, int Amount);
extern DELPHI_PACKAGE void __fastcall DrawParentImage(Vcl::Controls::TControl* Control, Vcl::Graphics::TCanvas* Dest, bool InvalidateParent = false)/* overload */;
extern DELPHI_PACKAGE void __fastcall DrawParentImage(Vcl::Controls::TControl* Control, HDC DC, bool InvalidateParent = false)/* overload */;
}	/* namespace Rzgrafx */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZGRAFX)
using namespace Rzgrafx;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzgrafxHPP
