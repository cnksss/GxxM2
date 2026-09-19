// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBBnEd.pas' rev: 35.00 (Windows)

#ifndef RzdbbnedHPP
#define RzdbbnedHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Buttons.hpp>
#include <RzBtnEdt.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Vcl.Graphics.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Menus.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <RzDBEdit.hpp>
#include <RzEdit.hpp>
#include <Vcl.Mask.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbbned
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBButtonEdit;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBButtonEdit : public Rzdbedit::TRzDBEdit
{
	typedef Rzdbedit::TRzDBEdit inherited;
	
private:
	bool FInternalUpdate;
	bool FAllowKeyEdit;
	Rzbtnedt::TRzButtonPair* FButtons;
	System::Uitypes::TColor FFlatButtonColor;
	Rzbtnedt::TButtonKind FAltBtnKind;
	Rzbtnedt::TButtonKind FButtonKind;
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
	virtual void __fastcall AdjustEditRect();
	virtual System::Types::TRect __fastcall GetEditRect();
	virtual void __fastcall SetEditRect();
	virtual void __fastcall AltBtnClickHandler(System::TObject* Sender);
	virtual void __fastcall ButtonClickHandler(System::TObject* Sender);
	DYNAMIC void __fastcall AltBtnClick();
	DYNAMIC void __fastcall ButtonClick();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
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
	virtual Rzbtnedt::TButtonKind __fastcall GetButtonKind(int Index);
	virtual void __fastcall SetButtonKind(int Index, Rzbtnedt::TButtonKind Value);
	virtual System::Classes::TShortCut __fastcall GetButtonShortCut(int Index);
	virtual void __fastcall SetButtonShortCut(int Index, System::Classes::TShortCut Value);
	virtual bool __fastcall GetButtonVisible(int Index);
	virtual void __fastcall SetButtonVisible(int Index, bool Value);
	virtual int __fastcall GetButtonWidth(int Index);
	virtual void __fastcall SetButtonWidth(int Index, int Value);
	
public:
	__fastcall virtual TRzDBButtonEdit(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	__property Rzbtnedt::TRzButtonPair* Buttons = {read=FButtons};
	__property Rzbutton::TRzControlButton* AltBtn = {read=GetButton, index=1};
	__property Rzbutton::TRzControlButton* Button = {read=GetButton, index=2};
	
__published:
	__property bool AllowKeyEdit = {read=FAllowKeyEdit, write=FAllowKeyEdit, default=1};
	__property Vcl::Graphics::TBitmap* AltBtnGlyph = {read=GetAltBtnGlyph, write=SetAltBtnGlyph, stored=IsCustomAltBtnGlyph};
	__property Vcl::Graphics::TBitmap* ButtonGlyph = {read=GetButtonGlyph, write=SetButtonGlyph, stored=IsCustomButtonGlyph};
	__property Vcl::Buttons::TNumGlyphs AltBtnNumGlyphs = {read=GetButtonNumGlyphs, write=SetButtonNumGlyphs, index=1, default=2};
	__property Vcl::Buttons::TNumGlyphs ButtonNumGlyphs = {read=GetButtonNumGlyphs, write=SetButtonNumGlyphs, index=2, default=2};
	__property System::UnicodeString AltBtnHint = {read=GetButtonHint, write=SetButtonHint, index=1};
	__property System::UnicodeString ButtonHint = {read=GetButtonHint, write=SetButtonHint, index=2};
	__property Rzbtnedt::TButtonKind AltBtnKind = {read=GetButtonKind, write=SetButtonKind, index=1, default=1};
	__property Rzbtnedt::TButtonKind ButtonKind = {read=GetButtonKind, write=SetButtonKind, index=2, default=1};
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
	__property Enabled = {default=1};
	__property FlatButtons = {default=0};
public:
	/* TRzDBEdit.Destroy */ inline __fastcall virtual ~TRzDBButtonEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBButtonEdit(HWND ParentWindow) : Rzdbedit::TRzDBEdit(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbbned */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBBNED)
using namespace Rzdbbned;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbbnedHPP
