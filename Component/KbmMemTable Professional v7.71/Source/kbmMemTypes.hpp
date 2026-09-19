// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemTypes.pas' rev: 30.00 (Windows)

#ifndef KbmmemtypesHPP
#define KbmmemtypesHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Data.DB.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmmemtypes
{
//-- forward type declarations -----------------------------------------------
struct TkbmRecord;
//-- type declarations -------------------------------------------------------
typedef TkbmRecord *PkbmRecord;

typedef NativeInt TkbmNativeInt;

typedef NativeUInt TkbmNativeUInt;

typedef NativeInt *PkbmNativeInt;

typedef NativeUInt *PkbmNativeUInt;

typedef int TkbmLongInt;

typedef int *PkbmLongInt;

struct DECLSPEC_DRECORD TkbmRecord
{
public:
	NativeInt RecordNo;
	NativeInt RecordID;
	NativeInt UniqueRecordID;
	System::Byte Flag;
	Data::Db::TUpdateStatus UpdateStatus;
	int TransactionLevel;
	NativeInt Tag;
	TkbmRecord *PrevRecordVersion;
	System::Byte *Data;
};


typedef Data::Db::TDateTimeRec *PDateTimeRec;

typedef System::WordBool *PWordBool;

typedef NativeInt TkbmDataEventInfo;

typedef System::PByte PkbmInternalAddRecord;

typedef NativeInt PkbmCalculateFields;

typedef NativeInt PkbmGetCalcFields;

typedef NativeInt PkbmClearCalcFields;

typedef NativeInt PkbmGetRecord;

typedef System::PByte PkbmInternalInitRecord;

typedef System::PByte PkbmInternalSetToRecord;

typedef System::PByte PkbmAllocRecordBuffer;

typedef System::PByte PkbmFreeRecordBuffer;

typedef System::PByte PkbmRecordBuffer;

typedef System::PByte PkbmGetBookmarkFlag;

typedef System::PByte PkbmSetBookmarkFlag;

typedef System::PByte PkbmGetBookmarkData;

typedef System::PByte PkbmSetBookmarkData;

typedef void * PkbmInternalBookmarkValid;

typedef void * PkbmInternalGotoBookmark;

typedef Data::Db::TBookmarkFlag *PBookmarkFlag;

typedef System::DynamicArray<System::Byte> PSetFieldDataBuffer;

typedef System::DynamicArray<System::Byte> PGetFieldDataBuffer;

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Kbmmemtypes */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMTYPES)
using namespace Kbmmemtypes;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmemtypesHPP
