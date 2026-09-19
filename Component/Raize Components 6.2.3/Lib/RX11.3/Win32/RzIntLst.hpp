// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzIntLst.pas' rev: 35.00 (Windows)

#ifndef RzintlstHPP
#define RzintlstHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <System.SysUtils.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzintlst
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS EOutOfRange;
class DELPHICLASS TRzIntegerList;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION EOutOfRange : public System::Sysutils::EListError
{
	typedef System::Sysutils::EListError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EOutOfRange(const System::UnicodeString Msg) : System::Sysutils::EListError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EOutOfRange(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::EListError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EOutOfRange(NativeUInt Ident)/* overload */ : System::Sysutils::EListError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EOutOfRange(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::EListError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EOutOfRange(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::EListError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EOutOfRange(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::EListError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EOutOfRange(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::EListError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EOutOfRange(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::EListError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EOutOfRange(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::EListError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EOutOfRange(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::EListError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EOutOfRange(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::EListError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EOutOfRange(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::EListError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EOutOfRange() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzIntegerList : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
public:
	int operator[](int Index) { return this->Items[Index]; }
	
private:
	System::Classes::TList* FList;
	System::Types::TDuplicates FDuplicates;
	int FMin;
	int FMax;
	bool FSorted;
	void __fastcall ReadMin(System::Classes::TReader* Reader);
	void __fastcall WriteMin(System::Classes::TWriter* Writer);
	void __fastcall ReadMax(System::Classes::TReader* Reader);
	void __fastcall WriteMax(System::Classes::TWriter* Writer);
	void __fastcall ReadIntegers(System::Classes::TReader* Reader);
	void __fastcall WriteIntegers(System::Classes::TWriter* Writer);
	void __fastcall SetSorted(bool Value);
	void __fastcall QuickSort(int L, int R);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual bool __fastcall Find(int N, int &Index);
	int __fastcall GetCount();
	int __fastcall GetItem(int Index);
	virtual void __fastcall SetItem(int Index, int Value);
	void __fastcall SetMin(int Value);
	void __fastcall SetMax(int Value);
	virtual void __fastcall Sort();
	
public:
	__fastcall TRzIntegerList();
	__fastcall virtual ~TRzIntegerList();
	virtual int __fastcall Add(int Value);
	virtual void __fastcall AddIntegers(TRzIntegerList* List);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	virtual void __fastcall Clear();
	virtual void __fastcall Delete(int Index);
	HIDESBASE bool __fastcall Equals(TRzIntegerList* List);
	virtual void __fastcall Exchange(int Index1, int Index2);
	virtual int __fastcall IndexOf(int N);
	virtual void __fastcall Insert(int Index, int Value);
	virtual void __fastcall InsertNumber(int Index, int Value);
	virtual void __fastcall Move(int CurIndex, int NewIndex);
	__property System::Types::TDuplicates Duplicates = {read=FDuplicates, write=FDuplicates, nodefault};
	__property int Count = {read=GetCount, nodefault};
	__property int Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property int Min = {read=FMin, write=SetMin, nodefault};
	__property int Max = {read=FMax, write=SetMax, nodefault};
	__property bool Sorted = {read=FSorted, write=SetSorted, nodefault};
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzintlst */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZINTLST)
using namespace Rzintlst;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzintlstHPP
