// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmSQLFuncAPI.pas' rev: 30.00 (Windows)

#ifndef KbmsqlfuncapiHPP
#define KbmsqlfuncapiHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Data.DB.hpp>
#include <System.Classes.hpp>
#include <kbmSQLElements.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmsqlfuncapi
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmSQLFunctionRegistration;
class DELPHICLASS TkbmSQLFunctionRegistrations;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLFunctionRegistration : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::UnicodeString FGroup;
	Kbmsqlelements::TkbmSQLCustomFunction FFunction;
	System::UnicodeString FFunctionName;
	bool FEnabled;
	
public:
	__fastcall virtual TkbmSQLFunctionRegistration(System::UnicodeString AGroup, System::UnicodeString AFunctionName, Kbmsqlelements::TkbmSQLCustomFunction AFunction, bool AEnabled);
	__fastcall virtual ~TkbmSQLFunctionRegistration(void);
	__property System::UnicodeString FunctionName = {read=FFunctionName};
	__property System::UnicodeString Group = {read=FGroup};
	__property bool Enabled = {read=FEnabled, write=FEnabled, nodefault};
	__property Kbmsqlelements::TkbmSQLCustomFunction _Function = {read=FFunction};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLFunctionRegistrations : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::Classes::TList* FList;
	
public:
	__fastcall virtual TkbmSQLFunctionRegistrations(void);
	__fastcall virtual ~TkbmSQLFunctionRegistrations(void);
	void __fastcall Clear(void);
	void __fastcall RegisterFunction(System::UnicodeString AGroup, System::UnicodeString AFunctionName, Kbmsqlelements::TkbmSQLCustomFunction AFunction);
	void __fastcall DisableGroup(System::UnicodeString AGroup);
	void __fastcall EnableGroup(System::UnicodeString AGroup);
	void __fastcall DisableFunction(System::UnicodeString AFunctionName);
	void __fastcall EnableFunction(System::UnicodeString AFunctionName);
	TkbmSQLFunctionRegistration* __fastcall GetFunctionReg(System::UnicodeString AFunctionName);
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE TkbmSQLFunctionRegistrations* kbmSQLFunctionRegistrations;
extern DELPHI_PACKAGE void __fastcall kbmSQLCheckArgs(Kbmsqlelements::TkbmSQLNodes* const AArgs, const int ACnt, const bool AMinimum = false);
}	/* namespace Kbmsqlfuncapi */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMSQLFUNCAPI)
using namespace Kbmsqlfuncapi;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmsqlfuncapiHPP
