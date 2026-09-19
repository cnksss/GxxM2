// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBTrak.pas' rev: 35.00 (Windows)

#ifndef RzdbtrakHPP
#define RzdbtrakHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <RzTrkBar.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Data.DB.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbtrak
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzTrackValueList;
class DELPHICLASS TRzDBTrackBar;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzTrackValueList : public System::Classes::TStringList
{
	typedef System::Classes::TStringList inherited;
	
private:
	TRzDBTrackBar* FTrackBar;
	
public:
	virtual int __fastcall Add(const System::UnicodeString Value);
	virtual void __fastcall Clear();
	virtual void __fastcall Delete(int Index);
	virtual void __fastcall Insert(int Index, const System::UnicodeString Value);
public:
	/* TStringList.Create */ inline __fastcall TRzTrackValueList()/* overload */ : System::Classes::TStringList() { }
	/* TStringList.Create */ inline __fastcall TRzTrackValueList(bool OwnsObjects)/* overload */ : System::Classes::TStringList(OwnsObjects) { }
	/* TStringList.Create */ inline __fastcall TRzTrackValueList(System::WideChar QuoteChar, System::WideChar Delimiter)/* overload */ : System::Classes::TStringList(QuoteChar, Delimiter) { }
	/* TStringList.Create */ inline __fastcall TRzTrackValueList(System::WideChar QuoteChar, System::WideChar Delimiter, System::Classes::TStringsOptions Options)/* overload */ : System::Classes::TStringList(QuoteChar, Delimiter, Options) { }
	/* TStringList.Create */ inline __fastcall TRzTrackValueList(System::Types::TDuplicates Duplicates, bool Sorted, bool CaseSensitive)/* overload */ : System::Classes::TStringList(Duplicates, Sorted, CaseSensitive) { }
	/* TStringList.Destroy */ inline __fastcall virtual ~TRzTrackValueList() { }
	
};


class PASCALIMPLEMENTATION TRzDBTrackBar : public Rztrkbar::TRzTrackBar
{
	typedef Rztrkbar::TRzTrackBar inherited;
	
private:
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	System::UnicodeString FValue;
	System::Classes::TStrings* FValues;
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual bool __fastcall CanInternalChange(int NewPos);
	virtual Data::Db::TField* __fastcall GetField();
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	virtual System::UnicodeString __fastcall GetPositionValue(int Index);
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall SetValue(const System::UnicodeString Value);
	virtual void __fastcall SetValues(System::Classes::TStrings* Value);
	__property Vcl::Dbctrls::TFieldDataLink* DataLink = {read=FDataLink};
	
public:
	__fastcall virtual TRzDBTrackBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBTrackBar();
	__property Data::Db::TField* Field = {read=GetField};
	__property System::UnicodeString Value = {read=GetValue, write=SetValue};
	
__published:
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Classes::TStrings* Values = {read=FValues, write=SetValues};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBTrackBar(HWND ParentWindow) : Rztrkbar::TRzTrackBar(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbtrak */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBTRAK)
using namespace Rzdbtrak;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbtrakHPP
