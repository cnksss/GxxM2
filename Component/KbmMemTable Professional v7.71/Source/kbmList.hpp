// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmList.pas' rev: 30.00 (Windows)

#ifndef KbmlistHPP
#define KbmlistHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmlist
{
//-- forward type declarations -----------------------------------------------
struct TkbmArrayList;
class DELPHICLASS TkbmList;
//-- type declarations -------------------------------------------------------
typedef System::StaticArray<void *, 134217727> TkbmPointerArray;

typedef TkbmPointerArray *PkbmPointerArray;

struct DECLSPEC_DRECORD TkbmArrayList
{
public:
	TkbmPointerArray *FArray;
	int FCapacity;
};


typedef TkbmArrayList *PkbmArrayList;

typedef System::StaticArray<TkbmArrayList, 16777216> TkbmArrayLists;

typedef TkbmArrayLists *PkbmArrayLists;

typedef System::Word TkbmArrayCount;

typedef System::Word *PkbmArrayCount;

typedef System::StaticArray<System::Word, 16777216> TkbmArrayCounts;

typedef TkbmArrayCounts *PkbmArrayCounts;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmList : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	void * operator[](int AIndex) { return Items[AIndex]; }
	
private:
	TkbmArrayLists *FLists;
	TkbmArrayCounts *FListCounts;
	int FListCount;
	int FTotalCount;
	int FCapacity;
	int FHalfTotalCount;
	int FMaxSubListSize;
	
protected:
	void * __fastcall GetItem(int AIndex);
	void __fastcall SetItem(int AIndex, const void * Aitem);
	void __fastcall Grow(const PkbmArrayList AList);
	void __fastcall SetCapacity(const int ACapacity);
	void __fastcall SetListCapacity(const PkbmArrayList AList, const int ACapacity);
	
public:
	__fastcall TkbmList(void);
	__fastcall virtual ~TkbmList(void);
	int __fastcall Add(const void * AItem);
	void __fastcall Clear(void);
	void __fastcall Delete(int AIndex);
	int __fastcall IndexOf(const void * AItem);
	void __fastcall Insert(int AIndex, const void * AItem);
	void __fastcall Pack(void);
	__property int Capacity = {read=FCapacity, write=SetCapacity, nodefault};
	__property int Count = {read=FTotalCount, nodefault};
	__property void * Items[int AIndex] = {read=GetItem, write=SetItem/*, default*/};
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
static const int kbmMaxListSize = int(0x7ffffff);
}	/* namespace Kbmlist */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMLIST)
using namespace Kbmlist;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmlistHPP
