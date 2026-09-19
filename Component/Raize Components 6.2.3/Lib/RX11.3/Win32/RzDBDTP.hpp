// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBDTP.pas' rev: 35.00 (Windows)

#ifndef RzdbdtpHPP
#define RzdbdtpHPP

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
#include <RzDTP.hpp>
#include <RzCommon.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.ComCtrls.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbdtp
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBDateTimePicker;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBDateTimePicker : public Rzdtp::TRzDateTimePicker
{
	typedef Rzdtp::TRzDateTimePicker inherited;
	
private:
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	Rzdtp::TRzDateTimePicker* FPaintControl;
	void __fastcall ActiveChangeHandler(System::TObject* Sender);
	void __fastcall DataChangeHandler(System::TObject* Sender);
	void __fastcall UpdateDataHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify(Winapi::Messages::TWMNotify &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall CheckFieldType(const System::UnicodeString Value);
	virtual void __fastcall DataChange();
	virtual void __fastcall UpdateData();
	virtual void __fastcall ActiveChange();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual Data::Db::TField* __fastcall GetField();
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	
public:
	__fastcall virtual TRzDBDateTimePicker(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBDateTimePicker();
	virtual bool __fastcall UpdateAction(System::Classes::TBasicAction* Action);
	__property Data::Db::TField* Field = {read=GetField};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBDateTimePicker(HWND ParentWindow) : Rzdtp::TRzDateTimePicker(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbdtp */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBDTP)
using namespace Rzdbdtp;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbdtpHPP
