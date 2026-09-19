// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBSpin.pas' rev: 35.00 (Windows)

#ifndef RzdbspinHPP
#define RzdbspinHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Vcl.Graphics.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <RzDBEdit.hpp>
#include <RzSpnEdt.hpp>
#include <RzEdit.hpp>
#include <Vcl.Mask.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbspin
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBSpinEdit;
class DELPHICLASS TRzDBSpinner;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBSpinEdit : public Rzdbedit::TRzDBEdit
{
	typedef Rzdbedit::TRzDBEdit inherited;
	
private:
	bool FAllowKeyEdit;
	bool FAllowBlank;
	System::Extended FBlankValue;
	Rzspnedt::TRzSpinButtons* FButtons;
	int FButtonWidth;
	bool FCheckRange;
	System::Byte FDecimals;
	System::Extended FIncrement;
	bool FIntegersOnly;
	System::Extended FMin;
	System::Extended FMax;
	System::Extended FPageSize;
	System::Uitypes::TColor FFlatButtonColor;
	bool FInternalUpdate;
	Rzspnedt::TSpinChangingEvent FOnChanging;
	Rzspnedt::TSpinButtonEvent FOnButtonClick;
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
	bool __fastcall CanEditData();
	bool __fastcall IsCustomDownGlyph();
	bool __fastcall IsCustomUpGlyph();
	virtual void __fastcall ReadOnlyChanged();
	virtual void __fastcall ResizeButtons();
	virtual void __fastcall AdjustEditRect();
	virtual System::Types::TRect __fastcall GetEditRect();
	virtual void __fastcall SetEditRect();
	System::UnicodeString __fastcall CleanUpText();
	virtual bool __fastcall IsValidChar(System::WideChar Key);
	virtual void __fastcall UpRightClickHandler(System::TObject* Sender);
	virtual void __fastcall DownLeftClickHandler(System::TObject* Sender);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC bool __fastcall CanChange(System::Extended NewValue);
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall DoButtonClick(Rzspnedt::TSpinButtonType S);
	virtual void __fastcall IncValue(const System::Extended Amount);
	virtual void __fastcall DecValue(const System::Extended Amount);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetButtonWidth(int Value);
	virtual void __fastcall SetDecimals(System::Byte Value);
	virtual void __fastcall SetIntegersOnly(bool Value);
	virtual Rzbutton::TRzControlButton* __fastcall GetButton(int Index);
	virtual Vcl::Graphics::TBitmap* __fastcall GetButtonUpGlyph();
	virtual void __fastcall SetButtonUpGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetButtonUpNumGlyphs();
	virtual void __fastcall SetButtonUpNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual Vcl::Graphics::TBitmap* __fastcall GetButtonDownGlyph();
	virtual void __fastcall SetButtonDownGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TNumGlyphs __fastcall GetButtonDownNumGlyphs();
	virtual void __fastcall SetButtonDownNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual Rzspnedt::TSpinDirection __fastcall GetDirection();
	virtual void __fastcall SetDirection(Rzspnedt::TSpinDirection Value);
	virtual void __fastcall SetFlatButtons(bool Value);
	virtual Rzcommon::TOrientation __fastcall GetOrientation();
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetCheckRange(bool Value);
	virtual void __fastcall SetMin(const System::Extended Value);
	virtual void __fastcall SetMax(const System::Extended Value);
	virtual __int64 __fastcall GetIntValue();
	virtual void __fastcall SetIntValue(__int64 Value);
	virtual System::Extended __fastcall GetValue();
	virtual System::Extended __fastcall CheckValue(const System::Extended Value);
	virtual void __fastcall SetValue(const System::Extended Value);
	bool __fastcall StoreIncrement();
	bool __fastcall StorePageSize();
	
public:
	__fastcall virtual TRzDBSpinEdit(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	__property Rzspnedt::TRzSpinButtons* Buttons = {read=FButtons};
	__property Rzbutton::TRzControlButton* DownLeftButton = {read=GetButton, index=1};
	__property Rzbutton::TRzControlButton* UpRightButton = {read=GetButton, index=2};
	__property __int64 IntValue = {read=GetIntValue, write=SetIntValue};
	
__published:
	__property bool AllowBlank = {read=FAllowBlank, write=FAllowBlank, default=1};
	__property System::Extended BlankValue = {read=FBlankValue, write=FBlankValue};
	__property bool AllowKeyEdit = {read=FAllowKeyEdit, write=FAllowKeyEdit, default=0};
	__property Vcl::Graphics::TBitmap* ButtonDownGlyph = {read=GetButtonDownGlyph, write=SetButtonDownGlyph, stored=IsCustomDownGlyph};
	__property Vcl::Buttons::TNumGlyphs ButtonDownNumGlyphs = {read=GetButtonDownNumGlyphs, write=SetButtonDownNumGlyphs, stored=IsCustomDownGlyph, nodefault};
	__property Vcl::Graphics::TBitmap* ButtonUpGlyph = {read=GetButtonUpGlyph, write=SetButtonUpGlyph, stored=IsCustomUpGlyph};
	__property Vcl::Buttons::TNumGlyphs ButtonUpNumGlyphs = {read=GetButtonUpNumGlyphs, write=SetButtonUpNumGlyphs, stored=IsCustomUpGlyph, nodefault};
	__property int ButtonWidth = {read=FButtonWidth, write=SetButtonWidth, default=17};
	__property bool CheckRange = {read=FCheckRange, write=SetCheckRange, default=0};
	__property System::Byte Decimals = {read=FDecimals, write=SetDecimals, default=0};
	__property Rzspnedt::TSpinDirection Direction = {read=GetDirection, write=SetDirection, default=0};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=FFlatButtonColor, default=-16777201};
	__property System::Extended Increment = {read=FIncrement, write=FIncrement, stored=StoreIncrement};
	__property bool IntegersOnly = {read=FIntegersOnly, write=SetIntegersOnly, default=1};
	__property System::Extended Max = {read=FMax, write=SetMax, stored=true};
	__property System::Extended Min = {read=FMin, write=SetMin, stored=true};
	__property Rzcommon::TOrientation Orientation = {read=GetOrientation, write=SetOrientation, default=1};
	__property System::Extended PageSize = {read=FPageSize, write=FPageSize, stored=StorePageSize};
	__property System::Extended Value = {read=GetValue, write=SetValue};
	__property Rzspnedt::TSpinChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property Rzspnedt::TSpinButtonEvent OnButtonClick = {read=FOnButtonClick, write=FOnButtonClick};
	__property Enabled = {default=1};
	__property FlatButtons = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
public:
	/* TRzDBEdit.Destroy */ inline __fastcall virtual ~TRzDBSpinEdit() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBSpinEdit(HWND ParentWindow) : Rzdbedit::TRzDBEdit(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBSpinner : public Rzspnedt::TRzSpinner
{
	typedef Rzspnedt::TRzSpinner inherited;
	
private:
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	Rzspnedt::TRzSpinner* FPaintControl;
	bool FBeepOnInvalidKey;
	void __fastcall DataChangeHandler(System::TObject* Sender);
	void __fastcall UpdateDataHandler(System::TObject* Sender);
	void __fastcall ActiveChangeHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall CheckFieldType(const System::UnicodeString Value);
	virtual void __fastcall DecValue(int Amount);
	virtual void __fastcall IncValue(int Amount);
	virtual void __fastcall DataChange();
	virtual void __fastcall UpdateData();
	virtual void __fastcall ActiveChange();
	void __fastcall InvalidKeyPressed();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual Data::Db::TField* __fastcall GetField();
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	__property Data::Db::TField* Field = {read=GetField};
	__property Vcl::Dbctrls::TFieldDataLink* DataLink = {read=FDataLink};
	
public:
	__fastcall virtual TRzDBSpinner(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBSpinner();
	
__published:
	__property bool BeepOnInvalidKey = {read=FBeepOnInvalidKey, write=FBeepOnInvalidKey, default=1};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBSpinner(HWND ParentWindow) : Rzspnedt::TRzSpinner(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::ResourceString _sRzInvalidFieldType;
#define Rzdbspin_sRzInvalidFieldType System::LoadResourceString(&Rzdbspin::_sRzInvalidFieldType)
}	/* namespace Rzdbspin */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBSPIN)
using namespace Rzdbspin;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbspinHPP
