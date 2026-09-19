// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmSQLMemTableAPI.pas' rev: 30.00 (Windows)

#ifndef KbmsqlmemtableapiHPP
#define KbmsqlmemtableapiHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <kbmSQLDBAPI.hpp>
#include <kbmSQLElements.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmsqlmemtableapi
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmSQLMemTableAPI;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLMemTableAPI : public Kbmsqldbapi::TkbmSQLCustomDatasetAPI
{
	typedef Kbmsqldbapi::TkbmSQLCustomDatasetAPI inherited;
	
protected:
	virtual System::UnicodeString __fastcall GetIndexNameForField(Kbmsqlelements::TkbmSQLFieldNode* const AField);
	
public:
	virtual bool __fastcall LocateFirst(Kbmsqlelements::TkbmSQLTable* const ATable, Kbmsqlelements::TkbmSQLCustomNode* const ACondition);
	virtual bool __fastcall LocateNext(Kbmsqlelements::TkbmSQLTable* const ATable, Kbmsqlelements::TkbmSQLCustomNode* const ACondition);
public:
	/* TObject.Create */ inline __fastcall TkbmSQLMemTableAPI(void) : Kbmsqldbapi::TkbmSQLCustomDatasetAPI() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLMemTableAPI(void) { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Kbmsqlmemtableapi */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMSQLMEMTABLEAPI)
using namespace Kbmsqlmemtableapi;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmsqlmemtableapiHPP
