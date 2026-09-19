// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemBinaryStreamFormat.pas' rev: 30.00 (Windows)

#ifndef KbmmembinarystreamformatHPP
#define KbmmembinarystreamformatHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <kbmMemTable.hpp>
#include <kbmMemTypes.hpp>
#include <System.Classes.hpp>
#include <Data.DB.hpp>
#include <kbmMemResEng.hpp>
#include <System.SysUtils.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmmembinarystreamformat
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmCustomBinaryStreamFormat;
class DELPHICLASS TkbmBinaryStreamFormat;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TkbmStreamFlagUsingIndex : unsigned char { sfSaveUsingIndex };

typedef System::Set<TkbmStreamFlagUsingIndex, TkbmStreamFlagUsingIndex::sfSaveUsingIndex, TkbmStreamFlagUsingIndex::sfSaveUsingIndex> TkbmStreamFlagUsingIndexs;

enum DECLSPEC_DENUM TkbmStreamFlagDataTypeHeader : unsigned char { sfSaveDataTypeHeader, sfLoadDataTypeHeader };

typedef System::Set<TkbmStreamFlagDataTypeHeader, TkbmStreamFlagDataTypeHeader::sfSaveDataTypeHeader, TkbmStreamFlagDataTypeHeader::sfLoadDataTypeHeader> TkbmStreamFlagDataTypeHeaders;

class PASCALIMPLEMENTATION TkbmCustomBinaryStreamFormat : public Kbmmemtable::TkbmCustomStreamFormat
{
	typedef Kbmmemtable::TkbmCustomStreamFormat inherited;
	
private:
	System::Classes::TWriter* Writer;
	System::Classes::TReader* Reader;
	TkbmStreamFlagUsingIndexs FUsingIndex;
	TkbmStreamFlagDataTypeHeaders FDataTypeHeader;
	int FBuffSize;
	int FileVersion;
	bool InitIndexDef;
	int ProgressCnt;
	int StreamSize;
	void __fastcall SetBuffSize(int ABuffSize);
	
protected:
	virtual System::UnicodeString __fastcall GetVersion(void);
	virtual void __fastcall BeforeSave(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveDef(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveData(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterSave(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall BeforeLoad(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadDef(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadData(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterLoad(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall DetermineLoadFieldIndex(Kbmmemtable::TkbmCustomMemTable* ADataset, System::UnicodeString ID, int FieldCount, int OrigIndex, int &NewIndex, Kbmmemtable::TkbmDetermineLoadFieldsSituation Situation);
	__property TkbmStreamFlagUsingIndexs sfUsingIndex = {read=FUsingIndex, write=FUsingIndex, nodefault};
	__property TkbmStreamFlagDataTypeHeaders sfDataTypeHeader = {read=FDataTypeHeader, write=FDataTypeHeader, nodefault};
	__property int BufferSize = {read=FBuffSize, write=SetBuffSize, nodefault};
	
public:
	__fastcall virtual TkbmCustomBinaryStreamFormat(System::Classes::TComponent* AOwner);
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmCustomBinaryStreamFormat(void) { }
	
};


class PASCALIMPLEMENTATION TkbmBinaryStreamFormat : public TkbmCustomBinaryStreamFormat
{
	typedef TkbmCustomBinaryStreamFormat inherited;
	
__published:
	__property Version = {default=0};
	__property sfUsingIndex;
	__property sfData;
	__property sfCalculated;
	__property sfLookup;
	__property sfNonVisible;
	__property sfBlobs;
	__property sfDef;
	__property sfIndexDef;
	__property sfFiltered;
	__property sfIgnoreRange;
	__property sfIgnoreMasterDetail;
	__property sfDeltas;
	__property sfDontFilterDeltas;
	__property sfAppend;
	__property sfFieldKind;
	__property sfFromStart;
	__property sfDataTypeHeader;
	__property sfDisplayWidth;
	__property OnBeforeLoad;
	__property OnAfterLoad;
	__property OnBeforeSave;
	__property OnAfterSave;
	__property OnCompress;
	__property OnDeCompress;
	__property BufferSize;
public:
	/* TkbmCustomBinaryStreamFormat.Create */ inline __fastcall virtual TkbmBinaryStreamFormat(System::Classes::TComponent* AOwner) : TkbmCustomBinaryStreamFormat(AOwner) { }
	
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmBinaryStreamFormat(void) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Kbmmembinarystreamformat */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMBINARYSTREAMFORMAT)
using namespace Kbmmembinarystreamformat;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmembinarystreamformatHPP
