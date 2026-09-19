// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemSQL.pas' rev: 30.00 (Windows)

#ifndef KbmmemsqlHPP
#define KbmmemsqlHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Data.DB.hpp>
#include <kbmMemTable.hpp>
#include <System.Classes.hpp>
#include <System.SysUtils.hpp>
#include <kbmSQLParser.hpp>
#include <kbmSQLElements.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmmemsql
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmMemSQL;
//-- type declarations -------------------------------------------------------
typedef bool __fastcall (__closure *TkbmSQLOnError)(System::TObject* Sender, int AErrorType, int AErrorCode, int ALine, int ACol, const System::UnicodeString AMsg, const System::UnicodeString AData);

class PASCALIMPLEMENTATION TkbmMemSQL : public Kbmmemtable::TkbmCustomMemTable
{
	typedef Kbmmemtable::TkbmCustomMemTable inherited;
	
private:
	Kbmsqlparser::TkbmSQLParser* FParser;
	Kbmsqlelements::TkbmSQLTables* FTables;
	System::TObject* FContext;
	Kbmsqlelements::TkbmSQLOnGetVariableValue FOnGetVariableValue;
	Kbmsqlelements::TkbmSQLOnGetVariableMetaData FOnGetVariableMetaData;
	Kbmsqlelements::TkbmSQLOnGetFunction FOnGetFunction;
	TkbmSQLOnError FOnError;
	
protected:
	virtual void __fastcall ResetAffectedRows(void);
	virtual void __fastcall FixupRefSourceTables(void);
	virtual void __fastcall DoOnError(System::TObject* Sender, int AErrorType, int AErrorCode, int ALine, int ACol, const System::UnicodeString AMsg, const System::UnicodeString AData);
	
public:
	void __fastcall ExecSQL(System::UnicodeString ASQL);
	System::Variant __fastcall Evaluate(const System::UnicodeString AExpression, const bool ASyntaxCheckOnly = false, const Kbmsqlelements::TkbmSQLExpressionOptions AOptions = (Kbmsqlelements::TkbmSQLExpressionOptions() << Kbmsqlelements::TkbmSQLExpressionOption::seoOnlyNumericExpressions ));
	System::Variant __fastcall Calculate(const System::UnicodeString AExpression, const bool ASyntaxCheckOnly = false, const Kbmsqlelements::TkbmSQLExpressionOptions AOptions = (Kbmsqlelements::TkbmSQLExpressionOptions() << Kbmsqlelements::TkbmSQLExpressionOption::seoOnlyNumericExpressions ));
	__fastcall virtual TkbmMemSQL(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TkbmMemSQL(void);
	__property System::TObject* Context = {read=FContext, write=FContext};
	__property Kbmsqlelements::TkbmSQLTables* Tables = {read=FTables};
	__property Kbmsqlparser::TkbmSQLParser* Parser = {read=FParser};
	__property FormatSettings;
	
__published:
	__property Kbmsqlelements::TkbmSQLOnGetVariableValue OnGetVariableValue = {read=FOnGetVariableValue, write=FOnGetVariableValue};
	__property Kbmsqlelements::TkbmSQLOnGetVariableMetaData OnGetVariableMetaData = {read=FOnGetVariableMetaData, write=FOnGetVariableMetaData};
	__property Kbmsqlelements::TkbmSQLOnGetFunction OnGetFunction = {read=FOnGetFunction, write=FOnGetFunction};
	__property TkbmSQLOnError OnError = {read=FOnError, write=FOnError};
};


//-- var, const, procedure ---------------------------------------------------
#define TkbmMemSQLNumericVarTypes (System::Set<System::Byte, 0, 255>() << 0x2 << 0x3 << 0x4 << 0x5 << 0x6 << 0x7 << 0xb << 0x10 << 0x11 << 0x12 << 0x13 << 0x14 << 0x15 )
#define TkbmMemSQLNumericFieldTypes (System::Set<Data::Db::TFieldType, Data::Db::TFieldType::ftUnknown, Data::Db::TFieldType::ftSingle>() << Data::Db::TFieldType::ftSmallint << Data::Db::TFieldType::ftInteger << Data::Db::TFieldType::ftWord << Data::Db::TFieldType::ftBoolean << Data::Db::TFieldType::ftFloat << Data::Db::TFieldType::ftCurrency << Data::Db::TFieldType::ftDate << Data::Db::TFieldType::ftTime << Data::Db::TFieldType::ftDateTime << Data::Db::TFieldType::ftAutoInc << Data::Db::TFieldType::ftLargeint << Data::Db::TFieldType::ftTimeStamp << Data::Db::TFieldType::ftLongWord << Data::Db::TFieldType::ftShortint << Data::Db::TFieldType::ftByte << Data::Db::TFieldType::ftExtended << Data::Db::TFieldType::ftSingle )
}	/* namespace Kbmmemsql */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMSQL)
using namespace Kbmmemsql;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmemsqlHPP
