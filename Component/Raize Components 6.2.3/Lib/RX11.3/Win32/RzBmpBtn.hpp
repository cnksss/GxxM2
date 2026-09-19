// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzBmpBtn.pas' rev: 35.00 (Windows)

#ifndef RzbmpbtnHPP
#define RzbmpbtnHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.Menus.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbmpbtn
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzButtonBitmaps;
class DELPHICLASS TRzBmpButton;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TButtonBorder : unsigned char { bbNone, bbSingle, bbButton };

enum DECLSPEC_DENUM TBtnSize : unsigned char { bszNeither, bszButtonToBitmap, bszStretchToButton, bszTileToButton };

class PASCALIMPLEMENTATION TRzButtonBitmaps : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	Vcl::Graphics::TBitmap* FUp;
	Vcl::Graphics::TBitmap* FUpAndFocused;
	Vcl::Graphics::TBitmap* FDisabled;
	Vcl::Graphics::TBitmap* FDown;
	Vcl::Graphics::TBitmap* FStayDown;
	Vcl::Graphics::TBitmap* FHot;
	System::Uitypes::TColor FTransparentColor;
	System::Classes::TNotifyEvent FOnChange;
	void __fastcall BitmapsChanged(System::TObject* Sender);
	
protected:
	virtual void __fastcall SetUp(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetUpAndFocused(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetDisabled(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetDown(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetStayDown(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetHot(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetTransparentColor(System::Uitypes::TColor Value);
	
public:
	__fastcall TRzButtonBitmaps();
	__fastcall virtual ~TRzButtonBitmaps();
	
__published:
	__property Vcl::Graphics::TBitmap* Disabled = {read=FDisabled, write=SetDisabled};
	__property Vcl::Graphics::TBitmap* Down = {read=FDown, write=SetDown};
	__property Vcl::Graphics::TBitmap* StayDown = {read=FStayDown, write=SetStayDown};
	__property Vcl::Graphics::TBitmap* Hot = {read=FHot, write=SetHot};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=SetTransparentColor, nodefault};
	__property Vcl::Graphics::TBitmap* Up = {read=FUp, write=SetUp};
	__property Vcl::Graphics::TBitmap* UpAndFocused = {read=FUpAndFocused, write=SetUpAndFocused};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
};


class PASCALIMPLEMENTATION TRzBmpButton : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	int FGroupIndex;
	TRzButtonBitmaps* FBitmaps;
	bool FDown;
	int FCaptionDownOffset;
	bool FDragging;
	bool FAllowAllUp;
	Vcl::Buttons::TButtonLayout FLayout;
	int FSpacing;
	int FMargin;
	Vcl::Buttons::TButtonStyle FButtonStyle;
	TButtonBorder FButtonBorder;
	TBtnSize FButtonSize;
	bool FShowFocus;
	bool FShowDownPattern;
	System::Uitypes::TColor FColor;
	bool FWordWrap;
	bool IsFocused;
	System::Uitypes::TModalResult FModalResult;
	void __fastcall BitmapChanged(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMLButtonDblClk(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMButtonExtPressed(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMSysColorChange(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	Vcl::Buttons::TButtonState FState;
	bool FMouseOverButton;
	virtual void __fastcall Loaded();
	DYNAMIC HPALETTE __fastcall GetPalette();
	virtual void __fastcall Paint();
	void __fastcall UpdateExclusive();
	void __fastcall CalcLayout(System::Types::TRect &TextBounds, System::Types::TRect &PaintRect, Vcl::Graphics::TBitmap* Bitmap);
	void __fastcall ClickButton(bool DoClick);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetAllowAllUp(bool Value);
	virtual void __fastcall SetButtonBorder(TButtonBorder Value);
	virtual void __fastcall SetButtonSize(TBtnSize Value);
	virtual void __fastcall SetButtonStyle(Vcl::Buttons::TButtonStyle Value);
	HIDESBASE virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetDown(bool Value);
	virtual void __fastcall SetCaptionDownOffset(int Value);
	virtual void __fastcall SetGroupIndex(int Value);
	virtual void __fastcall SetLayout(Vcl::Buttons::TButtonLayout Value);
	virtual void __fastcall SetMargin(int Value);
	virtual void __fastcall SetShowDownPattern(bool Value);
	virtual void __fastcall SetShowFocus(bool Value);
	virtual void __fastcall SetSpacing(int Value);
	virtual void __fastcall SetWordWrap(bool Value);
	
public:
	__fastcall virtual TRzBmpButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzBmpButton();
	DYNAMIC void __fastcall Click();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowAllUp = {read=FAllowAllUp, write=SetAllowAllUp, default=0};
	__property int CaptionDownOffset = {read=FCaptionDownOffset, write=SetCaptionDownOffset, default=1};
	__property System::Uitypes::TModalResult ModalResult = {read=FModalResult, write=FModalResult, default=0};
	__property int GroupIndex = {read=FGroupIndex, write=SetGroupIndex, default=0};
	__property bool Down = {read=FDown, write=SetDown, default=0};
	__property TRzButtonBitmaps* Bitmaps = {read=FBitmaps, write=FBitmaps};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, nodefault};
	__property TButtonBorder ButtonBorder = {read=FButtonBorder, write=SetButtonBorder, default=0};
	__property bool ShowDownPattern = {read=FShowDownPattern, write=SetShowDownPattern, default=1};
	__property bool ShowFocus = {read=FShowFocus, write=SetShowFocus, default=1};
	__property TBtnSize ButtonSize = {read=FButtonSize, write=SetButtonSize, default=1};
	__property Vcl::Buttons::TButtonStyle ButtonStyle = {read=FButtonStyle, write=SetButtonStyle, default=0};
	__property Vcl::Buttons::TButtonLayout Layout = {read=FLayout, write=SetLayout, default=0};
	__property int Margin = {read=FMargin, write=SetMargin, default=-1};
	__property int Spacing = {read=FSpacing, write=SetSpacing, default=4};
	__property bool WordWrap = {read=FWordWrap, write=SetWordWrap, default=0};
	__property Action;
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Caption = {default=0};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property Height = {default=30};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Visible = {default=1};
	__property Width = {default=80};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
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
	/* TWinControl.CreateParented */ inline __fastcall TRzBmpButton(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word cm_BmpButtonPressed = System::Word(0x2420);
}	/* namespace Rzbmpbtn */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBMPBTN)
using namespace Rzbmpbtn;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbmpbtnHPP
