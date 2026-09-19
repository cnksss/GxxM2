// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzRadChk.pas' rev: 35.00 (Windows)

#ifndef RzradchkHPP
#define RzradchkHPP

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
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <Vcl.ActnList.hpp>
#include <Vcl.ImgList.hpp>
#include <Vcl.Menus.hpp>
#include <System.Actions.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzradchk
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCheckedActionLink;
class DELPHICLASS TRzCustomGlyphButton;
class DELPHICLASS TRzRadioButton;
class DELPHICLASS TRzCustomCheckBox;
class DELPHICLASS TRzCheckBox;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzCheckedActionLink : public Vcl::Controls::TWinControlActionLink
{
	typedef Vcl::Controls::TWinControlActionLink inherited;
	
protected:
	TRzCustomGlyphButton* FClient;
	virtual void __fastcall AssignClient(System::TObject* AClient);
	virtual bool __fastcall IsCheckedLinked();
	virtual void __fastcall SetChecked(bool Value);
public:
	/* TBasicActionLink.Create */ inline __fastcall virtual TRzCheckedActionLink(System::TObject* AClient) : Vcl::Controls::TWinControlActionLink(AClient) { }
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzCheckedActionLink() { }
	
};


typedef System::TMetaClass* TRzCheckedActionLinkClass;

class PASCALIMPLEMENTATION TRzCustomGlyphButton : public Rzbutton::TRzCustomButton
{
	typedef Rzbutton::TRzCustomButton inherited;
	
private:
	System::Classes::TAlignment FAlignment;
	bool FAutoSize;
	bool FWordWrap;
	System::Uitypes::TColor FFrameColor;
	int FNumGlyphs;
	Vcl::Graphics::TBitmap* FCustomGlyphs;
	Vcl::Imglist::TCustomImageList* FCustomGlyphImages;
	Vcl::Imglist::TChangeLink* FCustomGlyphImagesChangeLink;
	bool FUseCustomGlyphs;
	System::Uitypes::TColor FTransparentColor;
	System::Uitypes::TColor FWinMaskColor;
	System::Uitypes::TColor FFillColor;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FDisabledColor;
	bool FReadOnly;
	bool FReadOnlyColorOnFocus;
	System::Uitypes::TColor FReadOnlyColor;
	int FGlyphWidth;
	int FGlyphHeight;
	int FThemedGlyphWidth;
	int FThemedGlyphHeight;
	bool FTabOnEnter;
	Rzcommon::TRzButtonHotTrackStyle FHotTrackStyle;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	bool __fastcall IsCheckedStored();
	void __fastcall CustomGlyphsChanged(System::TObject* Sender);
	void __fastcall CustomGlyphImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	bool FClicksDisabled;
	Vcl::Graphics::TBitmap* FBackgroundBmp;
	bool FUsingMouse;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall AdjustBounds();
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	DYNAMIC Vcl::Controls::TControlActionLinkClass __fastcall GetActionLinkClass();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall GetGlyphPosition(int &X, int &Y);
	virtual void __fastcall UpdateGlyphDimensions() = 0 ;
	virtual void __fastcall SelectGlyph(Vcl::Graphics::TBitmap* Glyph) = 0 ;
	virtual void __fastcall UpdateDisplay();
	virtual void __fastcall RepaintDisplay();
	virtual System::Types::TRect __fastcall GetHotTrackRect();
	virtual void __fastcall BlendButtonFrame(Vcl::Graphics::TBitmap* Glyph);
	virtual void __fastcall DrawGlyph(Vcl::Graphics::TCanvas* ACanvas);
	void __fastcall UpdateUseCustomGlyphs();
	virtual void __fastcall Paint();
	virtual void __fastcall ExtractGlyph(int Index, Vcl::Graphics::TBitmap* Bitmap, Vcl::Graphics::TBitmap* Source, int W, int H);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetAlignment(System::Classes::TLeftRight Value);
	virtual void __fastcall SetAutoSize(bool Value);
	virtual bool __fastcall GetChecked();
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetCustomGlyphs(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetCustomGlyphImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetFillColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetUseCustomGlyphs(bool Value);
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTransparentColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetWinMaskColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetWordWrap(bool Value);
	__property System::Classes::TLeftRight Alignment = {read=FAlignment, write=SetAlignment, default=1};
	__property bool AutoSize = {read=FAutoSize, write=SetAutoSize, default=1};
	__property bool Checked = {read=GetChecked, write=SetChecked, stored=IsCheckedStored, default=0};
	__property Vcl::Graphics::TBitmap* CustomGlyphs = {read=FCustomGlyphs, write=SetCustomGlyphs};
	__property Vcl::Imglist::TCustomImageList* CustomGlyphImages = {read=FCustomGlyphImages, write=SetCustomGlyphImages};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FillColor = {read=FFillColor, write=SetFillColor, default=-16777211};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzButtonHotTrackStyle HotTrackStyle = {read=FHotTrackStyle, write=FHotTrackStyle, default=0};
	__property bool ReadOnly = {read=FReadOnly, write=SetReadOnly, default=0};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, default=-16777192};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=SetTransparentColor, default=32896};
	__property bool UseCustomGlyphs = {read=FUseCustomGlyphs, write=SetUseCustomGlyphs, default=0};
	__property System::Uitypes::TColor WinMaskColor = {read=FWinMaskColor, write=SetWinMaskColor, default=65280};
	__property bool WordWrap = {read=FWordWrap, write=SetWordWrap, default=0};
	
public:
	__fastcall virtual TRzCustomGlyphButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomGlyphButton();
	DYNAMIC System::Classes::TAlignment __fastcall GetControlsAlignment();
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomGlyphButton(HWND ParentWindow) : Rzbutton::TRzCustomButton(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
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


class PASCALIMPLEMENTATION TRzRadioButton : public TRzCustomGlyphButton
{
	typedef TRzCustomGlyphButton inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FChecked;
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogKey(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDblClk(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall BMGetCheck(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall ChangeState();
	virtual void __fastcall UpdateGlyphDimensions();
	virtual void __fastcall SelectGlyph(Vcl::Graphics::TBitmap* Glyph);
	virtual System::Uitypes::TColor __fastcall GetStyleFontColor(bool Enabled);
	virtual void __fastcall BlendButtonFrame(Vcl::Graphics::TBitmap* Glyph);
	virtual bool __fastcall GetChecked();
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetReadOnly(bool Value);
	
public:
	__fastcall virtual TRzRadioButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzRadioButton();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Action;
	__property Align = {default=0};
	__property Alignment = {default=1};
	__property AlignmentVertical = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property Caption = {default=0};
	__property Checked = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property CustomGlyphs;
	__property CustomGlyphImages;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FillColor = {default=-16777211};
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property Height;
	__property HelpContext = {default=0};
	__property HighlightColor = {default=-16777203};
	__property Hint = {default=0};
	__property HotTrack = {default=0};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property HotTrackStyle = {default=0};
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property TextStyle = {default=0};
	__property ThemeAware = {default=1};
	__property Transparent = {default=0};
	__property TransparentColor = {default=32896};
	__property UseCustomGlyphs = {default=0};
	__property Visible = {default=1};
	__property Width;
	__property WinMaskColor = {default=65280};
	__property WordWrap = {default=0};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzRadioButton(HWND ParentWindow) : TRzCustomGlyphButton(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzCustomCheckBox : public TRzCustomGlyphButton
{
	typedef TRzCustomGlyphButton inherited;
	
private:
	bool FAllowGrayed;
	Vcl::Stdctrls::TCheckBoxState FState;
	bool FKeyToggle;
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	MESSAGE void __fastcall BMGetCheck(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall ChangeState();
	virtual void __fastcall UpdateGlyphDimensions();
	virtual void __fastcall SelectGlyph(Vcl::Graphics::TBitmap* Glyph);
	virtual System::Uitypes::TColor __fastcall GetStyleFontColor(bool Enabled);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall DoExit();
	virtual bool __fastcall GetChecked();
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetState(Vcl::Stdctrls::TCheckBoxState Value);
	__property bool AllowGrayed = {read=FAllowGrayed, write=FAllowGrayed, default=0};
	__property Vcl::Stdctrls::TCheckBoxState State = {read=FState, write=SetState, nodefault};
	
public:
	__fastcall virtual TRzCustomCheckBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomCheckBox();
	void __fastcall InitState(Vcl::Stdctrls::TCheckBoxState Value);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomCheckBox(HWND ParentWindow) : TRzCustomGlyphButton(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzCheckBox : public TRzCustomCheckBox
{
	typedef TRzCustomCheckBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Action;
	__property Align = {default=0};
	__property Alignment = {default=1};
	__property AlignmentVertical = {default=0};
	__property AllowGrayed = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property Caption = {default=0};
	__property Checked = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property CustomGlyphs;
	__property CustomGlyphImages;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FillColor = {default=-16777211};
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property Height;
	__property HelpContext = {default=0};
	__property HighlightColor = {default=-16777203};
	__property Hint = {default=0};
	__property HotTrack = {default=0};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property HotTrackStyle = {default=0};
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property ShowHint;
	__property State;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TextStyle = {default=0};
	__property ThemeAware = {default=1};
	__property Transparent = {default=0};
	__property TransparentColor = {default=32896};
	__property UseCustomGlyphs = {default=0};
	__property Visible = {default=1};
	__property Width;
	__property WinMaskColor = {default=65280};
	__property WordWrap = {default=0};
	__property OnClick;
	__property OnContextPopup;
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
	/* TRzCustomCheckBox.Create */ inline __fastcall virtual TRzCheckBox(System::Classes::TComponent* AOwner) : TRzCustomCheckBox(AOwner) { }
	/* TRzCustomCheckBox.Destroy */ inline __fastcall virtual ~TRzCheckBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckBox(HWND ParentWindow) : TRzCustomCheckBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Int8 DefaultGlyphWidth = System::Int8(0xd);
static const System::Int8 DefaultGlyphHeight = System::Int8(0xd);
static const System::Int8 DefaultNumGlyphs_RadioButton = System::Int8(0x6);
static const System::Int8 DefaultNumGlyphs_CheckBox = System::Int8(0x9);
}	/* namespace Rzradchk */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZRADCHK)
using namespace Rzradchk;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzradchkHPP
