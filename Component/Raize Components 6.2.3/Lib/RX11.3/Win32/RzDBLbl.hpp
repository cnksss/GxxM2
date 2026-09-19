// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBLbl.pas' rev: 35.00 (Windows)

#ifndef RzdblblHPP
#define RzdblblHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Data.DBConsts.hpp>
#include <Vcl.VDBConsts.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzLabel.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdblbl
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBLabel;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBLabel : public Rzlabel::TRzLabel
{
	typedef Rzlabel::TRzLabel inherited;
	
private:
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	void __fastcall DataChange(System::TObject* Sender);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Paint();
	System::UnicodeString __fastcall GetFieldText();
	virtual System::UnicodeString __fastcall GetLabelText();
	virtual void __fastcall SetAutoSize(bool Value);
	virtual Data::Db::TField* __fastcall GetField();
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	
public:
	__fastcall virtual TRzDBLabel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBLabel();
	__property Data::Db::TField* Field = {read=GetField};
	
__published:
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property AutoSize = {default=0};
	__property Caption = {stored=false, default=0};
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdblbl */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBLBL)
using namespace Rzdblbl;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdblblHPP
