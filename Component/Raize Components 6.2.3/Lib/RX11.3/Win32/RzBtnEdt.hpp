// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzBtnEdt.pas' rev: 35.00 (Windows)

#ifndef RzbtnedtHPP
#define RzbtnedtHPP

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
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Buttons.hpp>
#include <RzEdit.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Mask.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbtnedt
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzButtonPair;
class DELPHICLASS TRzCustomButtonEdit;
class DELPHICLASS TRzButtonEdit;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TButtonKind : unsigned char { bkCustom, bkLookup, bkDropDown, bkCalendar, bkAccept, bkReject, bkFolder, bkFind, bkSearch };

class PASCALIMPLEMENTATION TRzButtonPair : public Vcl::Controls::TWinControl
{
	typedef Vcl::Controls::TWinControl inherited;
	
private:
	Rzbutton::TRzControlButton* FAltBtn;
	Rzbutton::TRzControlButton* FButton;
	Vcl::Controls::TWinControl* FFocusControl;
	bool FFlat;
	System::Classes::TNotifyEvent FOnAltBtnClick;
	System::Classes::TNotifyEvent FOnButtonClick;
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual Rzbutton::TRzControlButton* __fastcall CreateButton();
	virtual void __fastcall BtnClickHandler(System::TObject* Sender);
	virtual void __fastcall BtnMouseDownHandler(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall RepositionButtons();
	DYNAMIC void __fastcall AltBtnClick();
	DYNAMIC void __fastcall ButtonClick();
	virtual bool __fastcall GetAllEnabled();
	virtual void __fastcall SetAllEnabled(bool Value);
	virtual System::Uitypes::TColor __fastcall GetColor();
	HIDESBASE virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlat(bool Value);
	
public:
	__fastcall virtual TRzButtonPair(System::Classes::TComponent* AOwner);
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	__property Rzbutton::TRzControlButton* AltBtn = {read=FAltBtn};
	__property Rzbutton::TRzControlButton* Button = {read=FButton};
	
__published:
	__property System::Uitypes::TColor Color = {read=GetColor, write=SetColor, default=-16777201};
	__property bool Enabled = {read=GetAllEnabled, write=SetAllEnabled, default=1};
	__property bool Flat = {read=FFlat, write=SetFlat, default=0};
	__property Vcl::Controls::TWinControl* FocusControl = {read=FFocusControl, write=FFocusControl};
	__property System::Classes::TNotifyEvent OnAltBtnClick = {read=FOnAltBtnClick, write=FOnAltBtnClick};
	__property System::Classes::TNotifyEvent OnButtonClick = {read=FOnButtonClick, write=FOnButtonClick};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property ParentColor = {default=1};
	__property ParentCtl3D = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Visible = {default=1};
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzButtonPair(HWND ParentWindow) : Vcl::Controls::TWinControl(ParentWindow) { }
	/* TWinControl.Destroy */ inline __fastcall virtual ~TRzButtonPair() { }
	
};


class PASCALIMPLEMENTATION TRzCustomButtonEdit : public Rzedit::TRzEdit
{
	typedef Rzedit::TRzEdit inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAllowKeyEdit;
	TRzButtonPair* FButtons;
	System::Uitypes::TColor FFlatButtonColor;
	TButtonKind FAltBtnKind;
	TButtonKind FButtonKind;
	System::Classes::TShortCut FAltBtnShortCut;
	System::Classes::TShortCut FButtonShortCut;
	bool FShortCutPressed;
	bool FHideButtonsOnReadOnly;
	System::Classes::TNotifyEvent FOnAltBtnClick;
	System::Classes::TNotifyEvent FOnButtonClick;
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaste(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMCut(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	bool __fastcall IsCustomAltBtnGlyph();
	bool __fastcall IsCustomButtonGlyph();
	virtual void __fastcall ReadButtonShortCutText(System::Classes::TReader* Reader);
	virtual void __fastcall ReadAltBtnShortCutText(System::Classes::TReader* Reader);
	virtual void __fastcall WriteShortCutText(System::Classes::TWriter* Writer);
	virtual void __fastcall ReadOnlyChanged();
	virtual void __fastcall ResizeButtons();
	virtual System::Types::TRect __fastcall GetEditRect();
	virtual void __fastcall SetEditRect();
	virtual void __fastcall AltBtnClickHandler(System::TObject* Sender);
	virtual void __fastcall ButtonClickHandler(System::TObject* Sender);
	DYNAMIC void __fastcall AltBtnClick();
	DYNAMIC void __fastcall ButtonClick();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFlatButtons(bool Value);
	virtual void __fastcall SetHideButtonsOnReadOnly(bool Value);
	virtual Rzbutton::TRzControlButton* __fastcall GetButton(int Index);
	virtual Vcl::Graphics::TBitmap* __fastcall GetAltBtnGlyph();
	virtual void __fastcall SetAltBtnGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Graphics::TBitmap* __fastcall GetButtonGlyph();
	virtual void __fastcall SetButtonGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetButtonNumGlyphs(int Index);
	virtual void __fastcall SetButtonNumGlyphs(int Index, Vcl::Buttons::TNumGlyphs Value);
	virtual System::UnicodeString __fastcall GetButtonHint(int Index);
	virtual void __fastcall SetButtonHint(int Index, const System::UnicodeString Value);
	virtual TButtonKind __fastcall GetButtonKind(int Index);
	virtual void __fastcall SetButtonKind(int Index, TButtonKind Value);
	virtual System::Classes::TShortCut __fastcall GetButtonShortCut(int Index);
	virtual void __fastcall SetButtonShortCut(int Index, System::Classes::TShortCut Value);
	virtual bool __fastcall GetButtonVisible(int Index);
	virtual void __fastcall SetButtonVisible(int Index, bool Value);
	virtual int __fastcall GetButtonWidth(int Index);
	virtual void __fastcall SetButtonWidth(int Index, int Value);
	
public:
	__fastcall virtual TRzCustomButtonEdit(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	__property TRzButtonPair* Buttons = {read=FButtons};
	__property Rzbutton::TRzControlButton* AltBtn = {read=GetButton, index=1};
	__property Rzbutton::TRzControlButton* Button = {read=GetButton, index=2};
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowKeyEdit = {read=FAllowKeyEdit, write=FAllowKeyEdit, default=1};
	__property Vcl::Graphics::TBitmap* AltBtnGlyph = {read=GetAltBtnGlyph, write=SetAltBtnGlyph, stored=IsCustomAltBtnGlyph};
	__property Vcl::Graphics::TBitmap* ButtonGlyph = {read=GetButtonGlyph, write=SetButtonGlyph, stored=IsCustomButtonGlyph};
	__property Vcl::Buttons::TNumGlyphs AltBtnNumGlyphs = {read=GetButtonNumGlyphs, write=SetButtonNumGlyphs, index=1, default=2};
	__property Vcl::Buttons::TNumGlyphs ButtonNumGlyphs = {read=GetButtonNumGlyphs, write=SetButtonNumGlyphs, index=2, default=2};
	__property System::UnicodeString AltBtnHint = {read=GetButtonHint, write=SetButtonHint, index=1};
	__property System::UnicodeString ButtonHint = {read=GetButtonHint, write=SetButtonHint, index=2};
	__property TButtonKind AltBtnKind = {read=GetButtonKind, write=SetButtonKind, index=1, default=1};
	__property TButtonKind ButtonKind = {read=GetButtonKind, write=SetButtonKind, index=2, default=1};
	__property System::Classes::TShortCut AltBtnShortCut = {read=GetButtonShortCut, write=SetButtonShortCut, index=1, default=0};
	__property System::Classes::TShortCut ButtonShortCut = {read=GetButtonShortCut, write=SetButtonShortCut, index=2, default=115};
	__property bool AltBtnVisible = {read=GetButtonVisible, write=SetButtonVisible, index=1, default=0};
	__property bool ButtonVisible = {read=GetButtonVisible, write=SetButtonVisible, index=2, default=1};
	__property int AltBtnWidth = {read=GetButtonWidth, write=SetButtonWidth, index=1, default=17};
	__property int ButtonWidth = {read=GetButtonWidth, write=SetButtonWidth, index=2, default=17};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=FFlatButtonColor, default=-16777201};
	__property bool HideButtonsOnReadOnly = {read=FHideButtonsOnReadOnly, write=SetHideButtonsOnReadOnly, default=1};
	__property System::Classes::TNotifyEvent OnAltBtnClick = {read=FOnAltBtnClick, write=FOnAltBtnClick};
	__property System::Classes::TNotifyEvent OnButtonClick = {read=FOnButtonClick, write=FOnButtonClick};
	__property Alignment = {default=0};
	__property Enabled = {default=1};
public:
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzCustomButtonEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomButtonEdit(HWND ParentWindow) : Rzedit::TRzEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzButtonEdit : public TRzCustomButtonEdit
{
	typedef TRzCustomButtonEdit inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Alignment = {default=0};
	__property AllowKeyEdit = {default=1};
	__property AltBtnGlyph;
	__property ButtonGlyph;
	__property AltBtnNumGlyphs = {index=1, default=2};
	__property ButtonNumGlyphs = {index=2, default=2};
	__property AltBtnHint = {index=1, default=0};
	__property ButtonHint = {index=2, default=0};
	__property AltBtnKind = {index=1, default=1};
	__property ButtonKind = {index=2, default=1};
	__property AltBtnShortCut = {index=1, default=0};
	__property ButtonShortCut = {index=2, default=115};
	__property AltBtnVisible = {index=1, default=0};
	__property ButtonVisible = {index=2, default=1};
	__property AltBtnWidth = {index=1, default=17};
	__property ButtonWidth = {index=2, default=17};
	__property BeepOnInvalidKey = {default=1};
	__property Enabled = {default=1};
	__property FlatButtons = {default=0};
	__property FlatButtonColor = {default=-16777201};
	__property HideButtonsOnReadOnly = {default=1};
	__property Touch;
	__property OnAltBtnClick;
	__property OnButtonClick;
	__property OnGesture;
public:
	/* TRzCustomButtonEdit.Create */ inline __fastcall virtual TRzButtonEdit(System::Classes::TComponent* AOwner) : TRzCustomButtonEdit(AOwner) { }
	
public:
	/* TRzCustomEdit.Destroy */ inline __fastcall virtual ~TRzButtonEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzButtonEdit(HWND ParentWindow) : TRzCustomButtonEdit(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word um_PopupListKeyDown = System::Word(0xbd00);
static const System::Word um_PopupListKeyUP = System::Word(0xbd01);
extern DELPHI_PACKAGE Vcl::Graphics::TBitmap* __fastcall GetBtnEdtGlyph(TButtonKind Kind);
}	/* namespace Rzbtnedt */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBTNEDT)
using namespace Rzbtnedt;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbtnedtHPP
