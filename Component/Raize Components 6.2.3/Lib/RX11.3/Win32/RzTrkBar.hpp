// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzTrkBar.pas' rev: 35.00 (Windows)

#ifndef RztrkbarHPP
#define RztrkbarHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Themes.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztrkbar
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzTrackBar;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TBitmapType : unsigned char { btThumb, btHot, btDisabled, btMask };

enum DECLSPEC_DENUM TThumbStyle : unsigned char { tsBox, tsCustom, tsMixer, tsPointer, tsFlat, tsXPPointer, tsXPBox };

enum DECLSPEC_DENUM TTickStyle : unsigned char { tkStandard, tkOwnerDraw };

enum DECLSPEC_DENUM TPointerType : unsigned char { ptStandard, ptWin95 };

enum DECLSPEC_DENUM TTrackClickBehavior : unsigned char { tcbNone, tcbMoveByPage, tcbMoveToTick };

typedef void __fastcall (__closure *TDrawTickEvent)(TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);

typedef void __fastcall (__closure *TRzDrawTrackEvent)(TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);

class PASCALIMPLEMENTATION TRzTrackBar : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	int FBorderWidth;
	int FMax;
	int FMin;
	Rzcommon::TOrientation FOrientation;
	System::Word FPageSize;
	int FPosition;
	TTickStyle FTickStyle;
	System::Uitypes::TColor FTickColor;
	Rzcommon::TPositiveInteger FTickStep;
	System::Uitypes::TColor FHighlightColor;
	System::Uitypes::TColor FHotTrackColor;
	Rzcommon::TRzHotTrackColorType FHotTrackColorType;
	bool FShowTicks;
	bool FSliding;
	bool FPaging;
	bool FPagingDirInc;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	int FLastX;
	int FLastY;
	bool FTabOnEnter;
	int FThumbCenterOffsetX;
	int FThumbCenterOffsetY;
	int FThumbHeight;
	System::Types::TRect FThumbRct;
	TThumbStyle FThumbStyle;
	int FThumbWidth;
	int FHalfWidth;
	TPointerType FPointerType;
	System::Uitypes::TColor FTrackColor;
	System::Word FTrackOffset;
	System::Types::TRect FTrackRct;
	Rzcommon::TFrameStyleEx FTrackFrame;
	System::Uitypes::TColor FTrackFrameColor;
	System::Word FTrackWidth;
	bool FShowFocusRect;
	TTrackClickBehavior FTrackClickBehavior;
	Vcl::Graphics::TBitmap* FThumbBmp;
	Vcl::Graphics::TBitmap* FHotThumbBmp;
	Vcl::Graphics::TBitmap* FMaskBmp;
	Vcl::Graphics::TBitmap* FDisabledBmp;
	Vcl::Graphics::TBitmap* FBackgroundBmp;
	bool FUpdateBg;
	Vcl::Graphics::TBitmap* FCustomThumb;
	bool FUseHotThumb;
	bool FTransparent;
	bool FThemeAware;
	HICON FHorzCursor;
	HICON FVertCursor;
	System::Classes::TNotifyEvent FOnChange;
	Rzcommon::TPositionChangingEvent FOnChanging;
	TRzDrawTrackEvent FOnDrawTrack;
	TDrawTickEvent FOnDrawTick;
	void __fastcall TimerExpired(System::TObject* Sender);
	void __fastcall CustomThumbChanged(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	virtual void __fastcall CreateWnd();
	void __fastcall LoadThumbBitmaps();
	bool __fastcall ShowFocus();
	System::Types::TPoint __fastcall CursorPosition();
	bool __fastcall UseThemes();
	Vcl::Themes::TThemedElementDetails __fastcall GetThumbDetails();
	virtual void __fastcall DrawTrack(Vcl::Graphics::TCanvas* ACanvas);
	virtual void __fastcall DrawTicks(Vcl::Graphics::TCanvas* ACanvas);
	virtual void __fastcall DrawThumb(Vcl::Graphics::TCanvas* ACanvas);
	virtual void __fastcall Paint();
	void __fastcall UpdateBackground();
	void __fastcall MoveThumbToPoint(int X, int Y);
	DYNAMIC void __fastcall Change();
	DYNAMIC bool __fastcall CanChange(int NewPos);
	virtual bool __fastcall CanInternalChange(int NewPos);
	DYNAMIC void __fastcall DoDrawTrack(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);
	DYNAMIC void __fastcall DrawTick(Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetCustomThumb(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotTrackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotTrackColorType(Rzcommon::TRzHotTrackColorType Value);
	virtual void __fastcall SetMax(int Value);
	virtual void __fastcall SetMin(int Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetPointerType(TPointerType Value);
	virtual void __fastcall SetPosition(int Value);
	virtual void __fastcall SetShowTicks(bool Value);
	virtual void __fastcall SetThumbStyle(TThumbStyle Value);
	virtual void __fastcall SetTickColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTickStep(Rzcommon::TPositiveInteger Value);
	virtual void __fastcall SetTickStyle(TTickStyle Value);
	virtual void __fastcall SetTrackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTrackOffset(System::Word Value);
	virtual void __fastcall SetTrackFrame(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetTrackFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTrackWidth(System::Word Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetThemeAware(bool Value);
	
public:
	__fastcall virtual TRzTrackBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTrackBar();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Graphics::TBitmap* CustomThumb = {read=FCustomThumb, write=SetCustomThumb};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property System::Uitypes::TColor HotTrackColor = {read=FHotTrackColor, write=SetHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType HotTrackColorType = {read=FHotTrackColorType, write=SetHotTrackColorType, default=1};
	__property int Max = {read=FMax, write=SetMax, default=10};
	__property int Min = {read=FMin, write=SetMin, default=0};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property System::Word PageSize = {read=FPageSize, write=FPageSize, default=1};
	__property TPointerType PointerType = {read=FPointerType, write=SetPointerType, default=0};
	__property int Position = {read=FPosition, write=SetPosition, nodefault};
	__property bool ShowFocusRect = {read=FShowFocusRect, write=FShowFocusRect, default=1};
	__property bool ShowTicks = {read=FShowTicks, write=SetShowTicks, default=1};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property TThumbStyle ThumbStyle = {read=FThumbStyle, write=SetThumbStyle, default=3};
	__property System::Uitypes::TColor TickColor = {read=FTickColor, write=SetTickColor, default=-16777200};
	__property Rzcommon::TPositiveInteger TickStep = {read=FTickStep, write=SetTickStep, default=1};
	__property TTickStyle TickStyle = {read=FTickStyle, write=SetTickStyle, default=0};
	__property TTrackClickBehavior TrackClickBehavior = {read=FTrackClickBehavior, write=FTrackClickBehavior, default=1};
	__property System::Uitypes::TColor TrackColor = {read=FTrackColor, write=SetTrackColor, default=-16777194};
	__property System::Word TrackOffset = {read=FTrackOffset, write=SetTrackOffset, default=20};
	__property Rzcommon::TFrameStyle TrackFrame = {read=FTrackFrame, write=SetTrackFrame, default=8};
	__property System::Uitypes::TColor TrackFrameColor = {read=FTrackFrameColor, write=SetTrackFrameColor, default=-16777200};
	__property System::Word TrackWidth = {read=FTrackWidth, write=SetTrackWidth, default=8};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property Rzcommon::TPositionChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property TRzDrawTrackEvent OnDrawTrack = {read=FOnDrawTrack, write=FOnDrawTrack};
	__property TDrawTickEvent OnDrawTick = {read=FOnDrawTick, write=FOnDrawTick};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Color = {default=-16777211};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Height = {default=35};
	__property HelpContext = {default=0};
	__property Hint = {default=0};
	__property ParentColor = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=200};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTrackBar(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rztrkbar */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTRKBAR)
using namespace Rztrkbar;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztrkbarHPP
