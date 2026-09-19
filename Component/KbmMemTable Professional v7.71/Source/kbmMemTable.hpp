// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemTable.pas' rev: 30.00 (Windows)

#ifndef KbmmemtableHPP
#define KbmmemtableHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Data.DB.hpp>
#include <Data.DBCommon.hpp>
#include <Winapi.Windows.hpp>
#include <System.SyncObjs.hpp>
#include <System.Masks.hpp>
#include <System.Variants.hpp>
#include <Data.FmtBcd.hpp>
#include <Data.SqlTimSt.hpp>
#include <kbmMemTypes.hpp>
#include <kbmString.hpp>
#include <kbmList.hpp>
#include <kbmMove.hpp>
#include <System.AnsiStrings.hpp>

//-- user supplied -----------------------------------------------------------
typedef Set<Db::TFieldType, Db::ftUnknown, Db::ftSingle>  TkbmFieldTypes;

namespace Kbmmemtable
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmExprParser;
class DELPHICLASS EMemTableError;
class DELPHICLASS EMemTableFatalError;
class DELPHICLASS EMemTableInvalidRecord;
class DELPHICLASS EMemTableIndexError;
class DELPHICLASS EMemTableDupKey;
class DELPHICLASS EMemTableFilterError;
class DELPHICLASS EMemTableLocaleError;
class DELPHICLASS EMemTableInvalidLocale;
struct TkbmBookmark;
struct TkbmUserBookmark;
class DELPHICLASS TkbmFieldList;
class DELPHICLASS TkbmIndex;
class DELPHICLASS TkbmIndexes;
class DELPHICLASS TkbmCustomStreamFormat;
class DELPHICLASS TkbmStreamFormat;
class DELPHICLASS TkbmCommon;
class DELPHICLASS TkbmMasterDataLink;
class DELPHICLASS TkbmCustomMemTable;
class DELPHICLASS TkbmMemTable;
class DELPHICLASS TkbmBlobStream;
class DELPHICLASS TkbmCustomDeltaHandler;
//-- type declarations -------------------------------------------------------
typedef System::TMetaClass* TkbmCustomMemTableClass;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmExprParser : public Data::Dbcommon::TExprParser
{
	typedef Data::Dbcommon::TExprParser inherited;
	
private:
	TkbmCustomMemTable* FDataset;
	
public:
	__fastcall TkbmExprParser(TkbmCustomMemTable* DataSet, const System::UnicodeString Text, Data::Db::TFilterOptions Options);
	bool __fastcall Evaluate(void);
public:
	/* TExprParser.Destroy */ inline __fastcall virtual ~TkbmExprParser(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableError : public Data::Db::EDatabaseError
{
	typedef Data::Db::EDatabaseError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableError(const System::UnicodeString Msg) : Data::Db::EDatabaseError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : Data::Db::EDatabaseError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableError(NativeUInt Ident)/* overload */ : Data::Db::EDatabaseError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableError(System::PResStringRec ResStringRec)/* overload */ : Data::Db::EDatabaseError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : Data::Db::EDatabaseError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : Data::Db::EDatabaseError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableError(const System::UnicodeString Msg, int AHelpContext) : Data::Db::EDatabaseError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : Data::Db::EDatabaseError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableError(NativeUInt Ident, int AHelpContext)/* overload */ : Data::Db::EDatabaseError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : Data::Db::EDatabaseError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : Data::Db::EDatabaseError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : Data::Db::EDatabaseError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableError(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableFatalError : public EMemTableError
{
	typedef EMemTableError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableFatalError(const System::UnicodeString Msg) : EMemTableError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableFatalError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableFatalError(NativeUInt Ident)/* overload */ : EMemTableError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableFatalError(System::PResStringRec ResStringRec)/* overload */ : EMemTableError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableFatalError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableFatalError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableFatalError(const System::UnicodeString Msg, int AHelpContext) : EMemTableError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableFatalError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableFatalError(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableFatalError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableFatalError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableFatalError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableFatalError(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableInvalidRecord : public EMemTableFatalError
{
	typedef EMemTableFatalError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableInvalidRecord(const System::UnicodeString Msg) : EMemTableFatalError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableInvalidRecord(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableFatalError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableInvalidRecord(NativeUInt Ident)/* overload */ : EMemTableFatalError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableInvalidRecord(System::PResStringRec ResStringRec)/* overload */ : EMemTableFatalError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableInvalidRecord(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableFatalError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableInvalidRecord(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableFatalError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableInvalidRecord(const System::UnicodeString Msg, int AHelpContext) : EMemTableFatalError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableInvalidRecord(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableFatalError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableInvalidRecord(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableFatalError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableInvalidRecord(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableFatalError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableInvalidRecord(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableFatalError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableInvalidRecord(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableFatalError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableInvalidRecord(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableIndexError : public EMemTableError
{
	typedef EMemTableError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableIndexError(const System::UnicodeString Msg) : EMemTableError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableIndexError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableIndexError(NativeUInt Ident)/* overload */ : EMemTableError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableIndexError(System::PResStringRec ResStringRec)/* overload */ : EMemTableError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableIndexError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableIndexError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableIndexError(const System::UnicodeString Msg, int AHelpContext) : EMemTableError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableIndexError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableIndexError(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableIndexError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableIndexError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableIndexError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableIndexError(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableDupKey : public EMemTableError
{
	typedef EMemTableError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableDupKey(const System::UnicodeString Msg) : EMemTableError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableDupKey(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableDupKey(NativeUInt Ident)/* overload */ : EMemTableError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableDupKey(System::PResStringRec ResStringRec)/* overload */ : EMemTableError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableDupKey(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableDupKey(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableDupKey(const System::UnicodeString Msg, int AHelpContext) : EMemTableError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableDupKey(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableDupKey(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableDupKey(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableDupKey(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableDupKey(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableDupKey(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableFilterError : public EMemTableError
{
	typedef EMemTableError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableFilterError(const System::UnicodeString Msg) : EMemTableError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableFilterError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableFilterError(NativeUInt Ident)/* overload */ : EMemTableError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableFilterError(System::PResStringRec ResStringRec)/* overload */ : EMemTableError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableFilterError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableFilterError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableFilterError(const System::UnicodeString Msg, int AHelpContext) : EMemTableError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableFilterError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableFilterError(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableFilterError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableFilterError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableFilterError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableFilterError(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableLocaleError : public EMemTableError
{
	typedef EMemTableError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableLocaleError(const System::UnicodeString Msg) : EMemTableError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableLocaleError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableLocaleError(NativeUInt Ident)/* overload */ : EMemTableError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableLocaleError(System::PResStringRec ResStringRec)/* overload */ : EMemTableError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableLocaleError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableLocaleError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableLocaleError(const System::UnicodeString Msg, int AHelpContext) : EMemTableError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableLocaleError(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableLocaleError(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableLocaleError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableLocaleError(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableLocaleError(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableLocaleError(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMemTableInvalidLocale : public EMemTableLocaleError
{
	typedef EMemTableLocaleError inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMemTableInvalidLocale(const System::UnicodeString Msg) : EMemTableLocaleError(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMemTableInvalidLocale(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High) : EMemTableLocaleError(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableInvalidLocale(NativeUInt Ident)/* overload */ : EMemTableLocaleError(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMemTableInvalidLocale(System::PResStringRec ResStringRec)/* overload */ : EMemTableLocaleError(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableInvalidLocale(NativeUInt Ident, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableLocaleError(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMemTableInvalidLocale(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High)/* overload */ : EMemTableLocaleError(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMemTableInvalidLocale(const System::UnicodeString Msg, int AHelpContext) : EMemTableLocaleError(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMemTableInvalidLocale(const System::UnicodeString Msg, System::TVarRec const *Args, const int Args_High, int AHelpContext) : EMemTableLocaleError(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableInvalidLocale(NativeUInt Ident, int AHelpContext)/* overload */ : EMemTableLocaleError(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMemTableInvalidLocale(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : EMemTableLocaleError(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableInvalidLocale(System::PResStringRec ResStringRec, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableLocaleError(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMemTableInvalidLocale(NativeUInt Ident, System::TVarRec const *Args, const int Args_High, int AHelpContext)/* overload */ : EMemTableLocaleError(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMemTableInvalidLocale(void) { }
	
};

#pragma pack(pop)

struct DECLSPEC_DRECORD TkbmBookmark
{
public:
	Kbmmemtypes::TkbmRecord *Bookmark;
	Data::Db::TBookmarkFlag Flag;
	NativeInt RecordID;
};


typedef TkbmBookmark *PkbmBookmark;

struct DECLSPEC_DRECORD TkbmUserBookmark
{
public:
	Kbmmemtypes::TkbmRecord *Bookmark;
	NativeInt DataID;
	NativeInt RecordID;
};


typedef TkbmUserBookmark *PkbmUserBookmark;

enum DECLSPEC_DENUM TkbmifoOption : unsigned char { mtifoDescending, mtifoCaseInsensitive, mtifoPartial, mtifoIgnoreNull, mtifoIgnoreLocale, mtifoAggregate, mtifoAggSum, mtifoAggMin, mtifoAggMax, mtifoAggCount, mtifoAggAvg, mtifoAggStdDev, mtifoAggUsr1, mtifoAggUsr2, mtifoAggUsr3, mtifoIgnoreNonSpace, mtifoIgnoreKanatype, mtifoIgnoreSymbols, mtifoIgnoreWidth, mtifoExtract, mtifoAsDate, mtifoAsTime, mtifoAsDateTime, mtifoNullFirst };

typedef System::Set<TkbmifoOption, TkbmifoOption::mtifoDescending, TkbmifoOption::mtifoNullFirst> TkbmifoOptions;

enum DECLSPEC_DENUM TkbmMemTableStorageType : unsigned char { mtstDataSet, mtstStream, mtstBinaryStream, mtstFile, mtstBinaryFile };

enum DECLSPEC_DENUM TkbmMemTableUpdateFlag : unsigned char { mtufEdit, mtufAppend, mtufDontClear };

typedef System::Set<TkbmMemTableUpdateFlag, TkbmMemTableUpdateFlag::mtufEdit, TkbmMemTableUpdateFlag::mtufDontClear> TkbmMemTableUpdateFlags;

enum DECLSPEC_DENUM TkbmMemTableCompareOption : unsigned char { mtcoDescending, mtcoCaseInsensitive, mtcoPartialKey, mtcoIgnoreNullKey, mtcoIgnoreLocale, mtcoUnique, mtcoNonMaintained, mtcoNullFirst };

typedef System::Set<TkbmMemTableCompareOption, TkbmMemTableCompareOption::mtcoDescending, TkbmMemTableCompareOption::mtcoNullFirst> TkbmMemTableCompareOptions;

enum DECLSPEC_DENUM TkbmMemTableCopyTableOption : unsigned char { mtcpoStructure, mtcpoOnlyActiveFields, mtcpoProperties, mtcpoLookup, mtcpoCalculated, mtcpoAppend, mtcpoFieldIndex, mtcpoDontDisableIndexes, mtcpoIgnoreErrors, mtcpoLookupAsData, mtcpoCalculatedAsData, mtcpoStringAsWideString, mtcpoWideStringUTF8 };

typedef System::Set<TkbmMemTableCopyTableOption, TkbmMemTableCopyTableOption::mtcpoStructure, TkbmMemTableCopyTableOption::mtcpoWideStringUTF8> TkbmMemTableCopyTableOptions;

typedef void __fastcall (__closure *TkbmOnFilterIndex)(Data::Db::TDataSet* DataSet, TkbmIndex* Index, bool &Accept);

typedef void __fastcall (__closure *TkbmOnUserAggregate)(Data::Db::TDataSet* DataSet, const int UserFunction, const int Count, const double Value, double &Accumulator);

typedef System::PByte PkbmVarLength;

typedef System::PByte *PPkbmVarLength;

enum DECLSPEC_DENUM TkbmIndexType : unsigned char { mtitNonSorted, mtitSorted };

enum DECLSPEC_DENUM TkbmSearchType : unsigned char { mtstFirst, mtstLast, mtstNearestBefore, mtstNearestAfter };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmFieldList : public System::TObject
{
	typedef System::TObject inherited;
	
	
private:
	typedef System::DynamicArray<int> _TkbmFieldList__1;
	
	typedef System::DynamicArray<int> _TkbmFieldList__2;
	
	typedef System::DynamicArray<TkbmifoOptions> _TkbmFieldList__3;
	
	typedef System::DynamicArray<Data::Db::TField*> _TkbmFieldList__4;
	
	
private:
	int FCount;
	int FPrimaryCount;
	
public:
	int FieldCount;
	_TkbmFieldList__1 FieldNo;
	_TkbmFieldList__2 FieldOfs;
	_TkbmFieldList__3 Options;
	Data::Db::TLocateOptions LocateOptions;
	_TkbmFieldList__4 Fields;
	__fastcall virtual TkbmFieldList(void);
	__fastcall virtual ~TkbmFieldList(void);
	int __fastcall Add(TkbmCustomMemTable* const ADataSet, Data::Db::TField* const AField, const TkbmifoOptions AValue);
	virtual void __fastcall Clear(void);
	int __fastcall IndexOf(Data::Db::TField* Item);
	void __fastcall AssignTo(TkbmFieldList* AFieldList);
	void __fastcall MergeOptionsTo(TkbmFieldList* AFieldList);
	void __fastcall DefineAdditionalOrderFields(TkbmCustomMemTable* ADataSet, TkbmFieldList* AFieldList);
	void __fastcall ClearOptions(void);
	System::UnicodeString __fastcall GetAsString(void);
	void __fastcall SetOptions(TkbmCustomMemTable* ADataSet, TkbmifoOption AOptions, System::UnicodeString AFieldNames);
	Data::Db::TField* __fastcall FindField(const System::UnicodeString AFieldName);
	bool __fastcall StartsWith(TkbmFieldList* AList, const bool ASameCase, const bool AOnlyPrimary);
	bool __fastcall IsEqualTo(TkbmFieldList* AList, const bool ASameCase, const bool AOnlyPrimary);
	void __fastcall Build(TkbmCustomMemTable* ADataset, const System::UnicodeString AFieldNames, const bool AAggregateFieldNaming = false);
	__property int Count = {read=FCount, nodefault};
	__property int PrimaryCount = {read=FPrimaryCount, nodefault};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TkbmIndex : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	bool FBeingFreed;
	TkbmIndexes* FDependantIndexes;
	Kbmlist::TkbmList* FReferences;
	System::UnicodeString FName;
	TkbmCustomMemTable* FDataSet;
	System::UnicodeString FIndexFields;
	TkbmFieldList* FIndexFieldList;
	TkbmMemTableCompareOptions FIndexOptions;
	Kbmmemtypes::TkbmRecord *FRangeStartKey;
	Kbmmemtypes::TkbmRecord *FRangeEndKey;
	bool FSorted;
	bool FDirty;
	TkbmIndexType FType;
	bool FRowOrder;
	bool FIsFiltered;
	bool FIsRange;
	bool FEnabled;
	Data::Db::TUpdateStatusSet FUpdateStatus;
	TkbmExprParser* FFilterParser;
	TkbmOnFilterIndex FFilterFunc;
	TkbmIndex* FBaseIndex;
	void __fastcall InternalSwap(const int I, const int J);
	void __fastcall InternalInsertionSort(const int Lo, const int Hi);
	void __fastcall InternalFastQuickSort(const int L, const int R);
	void __fastcall SetEnabled(bool AValue);
	
protected:
	void __fastcall SetRangeStartKey(Kbmmemtypes::PkbmRecord AKeyRecord);
	void __fastcall SetRangeEndKey(Kbmmemtypes::PkbmRecord AKeyRecord);
	bool __fastcall GetIsOrdered(void);
	bool __fastcall GetIsFullScope(void);
	int __fastcall CompareRecords(TkbmFieldList* const AFieldList, const Kbmmemtypes::PkbmRecord AKeyRecord, const Kbmmemtypes::PkbmRecord ARecord, const bool APrimaryOnly, const bool ASortCompare, const bool APartial, const bool AUniqueConstraint);
	void __fastcall FastQuickSort(const int L, const int R);
	NativeInt __fastcall BinarySearchRecordID(NativeInt FirstNo, NativeInt LastNo, const NativeInt RecordID, const bool Desc, NativeInt &Index);
	NativeInt __fastcall SequentialSearchRecordID(const NativeInt FirstNo, const NativeInt LastNo, const NativeInt RecordID, NativeInt &Index);
	NativeInt __fastcall BinarySearch(TkbmFieldList* FieldList, NativeInt FirstNo, NativeInt LastNo, const Kbmmemtypes::PkbmRecord KeyRecord, const TkbmSearchType SearchType, const bool PrimaryOnly, const bool RespectFilter, NativeInt &Index, bool &Found);
	NativeInt __fastcall SequentialSearch(TkbmFieldList* FieldList, const NativeInt FirstNo, const NativeInt LastNo, const Kbmmemtypes::PkbmRecord KeyRecord, const TkbmSearchType SearchType, const bool PrimaryOnly, const bool RespectFilter, const bool SearchOrdered, NativeInt &Index, bool &Found);
	int __fastcall FindRecordNumber(const Kbmmemtypes::PkbmRecord ARecord);
	bool __fastcall Filter(const Kbmmemtypes::PkbmRecord ARecord, const bool ACheckRange);
	
public:
	__fastcall virtual TkbmIndex(TkbmIndex* ABase, System::UnicodeString Name, TkbmCustomMemTable* ADataSet, System::UnicodeString Fields, TkbmMemTableCompareOptions Options, TkbmIndexType IndexType)/* overload */;
	__fastcall virtual TkbmIndex(TkbmIndex* ABase, Data::Db::TIndexDef* IndexDef, TkbmCustomMemTable* ADataSet)/* overload */;
	__fastcall virtual ~TkbmIndex(void);
	NativeInt __fastcall Search(TkbmFieldList* FieldList, Kbmmemtypes::PkbmRecord KeyRecord, const TkbmSearchType SearchType, const bool PrimaryOnly, const bool RespectFilter, NativeInt &Index, bool &Found);
	NativeInt __fastcall SearchRecord(Kbmmemtypes::PkbmRecord KeyRecord, NativeInt &Index, const bool PrimaryOnly, const bool RespectFilter);
	NativeInt __fastcall SearchRecordID(NativeInt RecordID, NativeInt &Index);
	bool __fastcall IsDependingOn(TkbmIndex* AIndex);
	void __fastcall Clear(void);
	void __fastcall Load(void);
	void __fastcall ReSort(void);
	void __fastcall Rebuild(void);
	void __fastcall RebuildDepending(void);
	__property TkbmIndexes* DependantIndexes = {read=FDependantIndexes};
	__property TkbmIndex* Base = {read=FBaseIndex};
	__property bool Enabled = {read=FEnabled, write=SetEnabled, nodefault};
	__property bool IsSorted = {read=FSorted, write=FSorted, nodefault};
	__property bool IsDirty = {read=FDirty, write=FDirty, nodefault};
	__property bool IsOrdered = {read=GetIsOrdered, nodefault};
	__property bool IsFiltered = {read=FIsFiltered, write=FIsFiltered, nodefault};
	__property bool IsRange = {read=FIsRange, write=FIsRange, nodefault};
	__property bool IsFullScope = {read=GetIsFullScope, nodefault};
	__property TkbmIndexType IndexType = {read=FType, write=FType, nodefault};
	__property TkbmMemTableCompareOptions IndexOptions = {read=FIndexOptions, write=FIndexOptions, nodefault};
	__property System::UnicodeString IndexFields = {read=FIndexFields, write=FIndexFields};
	__property TkbmFieldList* IndexFieldList = {read=FIndexFieldList, write=FIndexFieldList};
	__property TkbmCustomMemTable* Dataset = {read=FDataSet, write=FDataSet};
	__property System::UnicodeString Name = {read=FName, write=FName};
	__property Kbmlist::TkbmList* References = {read=FReferences, write=FReferences};
	__property bool IsRowOrder = {read=FRowOrder, write=FRowOrder, nodefault};
	__property Data::Db::TUpdateStatusSet UpdateStatus = {read=FUpdateStatus, write=FUpdateStatus, nodefault};
	__property Kbmmemtypes::PkbmRecord RangeStartKey = {read=FRangeStartKey, write=SetRangeStartKey};
	__property Kbmmemtypes::PkbmRecord RangeEndKey = {read=FRangeEndKey, write=SetRangeEndKey};
};


enum DECLSPEC_DENUM TkbmIndexUpdateHow : unsigned char { mtiuhInsert, mtiuhEdit, mtiuhDelete, mtiuhAppend };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmIndexes : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	bool FInClear;
	System::Classes::TStringList* FIndexes;
	TkbmIndex* FRowOrderIndex;
	TkbmCustomMemTable* FDataSet;
	
protected:
	virtual void __fastcall IndexFreed(TkbmIndex* AIndex);
	
public:
	__fastcall virtual TkbmIndexes(TkbmCustomMemTable* ADataSet);
	__fastcall virtual ~TkbmIndexes(void);
	void __fastcall Clear(void);
	void __fastcall Add(Data::Db::TIndexDef* const IndexDef);
	void __fastcall AddIndex(TkbmIndex* const Index);
	void __fastcall DeleteIndex(TkbmIndex* const Index);
	void __fastcall ReBuild(const System::UnicodeString IndexName);
	void __fastcall Delete(const System::UnicodeString IndexName);
	TkbmIndex* __fastcall Get(const System::UnicodeString IndexName);
	TkbmIndex* __fastcall GetIndex(const int Ordinal);
	void __fastcall Empty(const System::UnicodeString IndexName);
	TkbmIndex* __fastcall GetByFieldNames(System::UnicodeString FieldNames);
	void __fastcall EmptyAll(void);
	void __fastcall ReBuildAll(void);
	void __fastcall MarkAllDirty(void);
	void __fastcall CheckRecordUniqueness(const Kbmmemtypes::PkbmRecord ARecord, const Kbmmemtypes::PkbmRecord ActualRecord);
	void __fastcall ReflectToIndexes(const TkbmIndexUpdateHow How, const Kbmmemtypes::PkbmRecord OldRecord, const Kbmmemtypes::PkbmRecord NewRecord, const NativeInt RecordPos, const bool DontVersion);
	NativeInt __fastcall Search(TkbmFieldList* const FieldList, const Kbmmemtypes::PkbmRecord KeyRecord, const TkbmSearchType SearchType, const bool PrimaryOnly, const bool RespectFilter, const bool AutoAddIdx, NativeInt &Index, bool &Found);
	int __fastcall Count(void);
};

#pragma pack(pop)

enum DECLSPEC_DENUM TkbmVersioningMode : unsigned char { mtvm1SinceCheckPoint, mtvmAllSinceCheckPoint };

enum DECLSPEC_DENUM TkbmProgressCode : unsigned char { mtpcLoad, mtpcSave, mtpcEmpty, mtpcPack, mtpcCheckPoint, mtpcSearch, mtpcCopy, mtpcUpdate, mtpcSort };

typedef System::Set<TkbmProgressCode, TkbmProgressCode::mtpcLoad, TkbmProgressCode::mtpcSort> TkbmProgressCodes;

enum DECLSPEC_DENUM TkbmState : unsigned char { mtstBrowse, mtstLoad, mtstSave, mtstEmpty, mtstPack, mtstCheckPoint, mtstSearch, mtstUpdate, mtstSort };

enum DECLSPEC_DENUM TkbmPerformance : unsigned char { mtpfFast, mtpfBalanced, mtpfSmall };

typedef void __fastcall (__closure *TkbmOnProgress)(Data::Db::TDataSet* DataSet, int Percentage, TkbmProgressCode Code);

typedef void __fastcall (__closure *TkbmOnLoadRecord)(Data::Db::TDataSet* DataSet, bool &Accept);

typedef void __fastcall (__closure *TkbmOnLoadField)(Data::Db::TDataSet* DataSet, int FieldNo, Data::Db::TField* Field);

typedef void __fastcall (__closure *TkbmOnSaveRecord)(Data::Db::TDataSet* DataSet, bool &Accept);

typedef void __fastcall (__closure *TkbmOnSaveField)(Data::Db::TDataSet* DataSet, int FieldNo, Data::Db::TField* Field);

typedef void __fastcall (__closure *TkbmOnCompressField)(Data::Db::TDataSet* DataSet, Data::Db::TField* Field, const System::PByte Buffer, int &Size, System::PByte &ResultBuffer);

typedef void __fastcall (__closure *TkbmOnDecompressField)(Data::Db::TDataSet* DataSet, Data::Db::TField* Field, const System::PByte Buffer, int &Size, System::PByte &ResultBuffer);

typedef void __fastcall (__closure *TkbmOnCompareFields)(Data::Db::TDataSet* DataSet, Data::Db::TField* AFld, void * KeyField, void * AField, Data::Db::TFieldType FieldType, TkbmifoOptions Options, bool &FullCompare, int &Result);

typedef void __fastcall (__closure *TkbmOnSave)(Data::Db::TDataSet* DataSet, TkbmMemTableStorageType StorageType, System::Classes::TStream* Stream);

typedef void __fastcall (__closure *TkbmOnLoad)(Data::Db::TDataSet* DataSet, TkbmMemTableStorageType StorageType, System::Classes::TStream* Stream);

typedef void __fastcall (__closure *TkbmOnSetupField)(Data::Db::TDataSet* DataSet, Data::Db::TField* Field, System::Byte &FieldFlags);

typedef void __fastcall (__closure *TkbmOnSetupFieldProperties)(Data::Db::TDataSet* DataSet, Data::Db::TField* Field);

typedef int TkbmLocaleID;

enum DECLSPEC_DENUM TkbmStreamFlagData : unsigned char { sfSaveData, sfLoadData };

enum DECLSPEC_DENUM TkbmStreamFlagCalculated : unsigned char { sfSaveCalculated, sfLoadCalculated };

enum DECLSPEC_DENUM TkbmStreamFlagLookup : unsigned char { sfSaveLookup, sfLoadLookup };

enum DECLSPEC_DENUM TkbmStreamFlagNonVisible : unsigned char { sfSaveNonVisible, sfLoadNonVisible };

enum DECLSPEC_DENUM TkbmStreamFlagBlobs : unsigned char { sfSaveBlobs, sfLoadBlobs };

enum DECLSPEC_DENUM TkbmStreamFlagDef : unsigned char { sfSaveDef, sfLoadDef, sfAutoLayoutDef, sfLoadByFieldNo };

enum DECLSPEC_DENUM TkbmStreamFlagIndexDef : unsigned char { sfSaveIndexDef, sfLoadIndexDef };

enum DECLSPEC_DENUM TkbmStreamFlagFiltered : unsigned char { sfSaveFiltered };

enum DECLSPEC_DENUM TkbmStreamFlagIgnoreRange : unsigned char { sfSaveIgnoreRange };

enum DECLSPEC_DENUM TkbmStreamFlagIgnoreMasterDetail : unsigned char { sfSaveIgnoreMasterDetail };

enum DECLSPEC_DENUM TkbmStreamFlagDeltas : unsigned char { sfSaveDeltas, sfLoadDeltas };

enum DECLSPEC_DENUM TkbmStreamFlagDontFilterDeltas : unsigned char { sfSaveDontFilterDeltas };

enum DECLSPEC_DENUM TkbmStreamFlagAppend : unsigned char { sfSaveAppend, sfSaveInsert };

enum DECLSPEC_DENUM TkbmStreamFlagFieldKind : unsigned char { sfSaveFieldKind, sfLoadFieldKind };

enum DECLSPEC_DENUM TkbmStreamFlagFromStart : unsigned char { sfLoadFromStart };

enum DECLSPEC_DENUM TkbmStreamFlagDisplayWidth : unsigned char { sfLoadDetermineWidth };

typedef System::Set<TkbmStreamFlagData, TkbmStreamFlagData::sfSaveData, TkbmStreamFlagData::sfLoadData> TkbmStreamFlagsData;

typedef System::Set<TkbmStreamFlagCalculated, TkbmStreamFlagCalculated::sfSaveCalculated, TkbmStreamFlagCalculated::sfLoadCalculated> TkbmStreamFlagsCalculated;

typedef System::Set<TkbmStreamFlagLookup, TkbmStreamFlagLookup::sfSaveLookup, TkbmStreamFlagLookup::sfLoadLookup> TkbmStreamFlagsLookup;

typedef System::Set<TkbmStreamFlagNonVisible, TkbmStreamFlagNonVisible::sfSaveNonVisible, TkbmStreamFlagNonVisible::sfLoadNonVisible> TkbmStreamFlagsNonVisible;

typedef System::Set<TkbmStreamFlagBlobs, TkbmStreamFlagBlobs::sfSaveBlobs, TkbmStreamFlagBlobs::sfLoadBlobs> TkbmStreamFlagsBlobs;

typedef System::Set<TkbmStreamFlagDef, TkbmStreamFlagDef::sfSaveDef, TkbmStreamFlagDef::sfLoadByFieldNo> TkbmStreamFlagsDef;

typedef System::Set<TkbmStreamFlagIndexDef, TkbmStreamFlagIndexDef::sfSaveIndexDef, TkbmStreamFlagIndexDef::sfLoadIndexDef> TkbmStreamFlagsIndexDef;

typedef System::Set<TkbmStreamFlagFiltered, TkbmStreamFlagFiltered::sfSaveFiltered, TkbmStreamFlagFiltered::sfSaveFiltered> TkbmStreamFlagsFiltered;

typedef System::Set<TkbmStreamFlagIgnoreRange, TkbmStreamFlagIgnoreRange::sfSaveIgnoreRange, TkbmStreamFlagIgnoreRange::sfSaveIgnoreRange> TkbmStreamFlagsIgnoreRange;

typedef System::Set<TkbmStreamFlagIgnoreMasterDetail, TkbmStreamFlagIgnoreMasterDetail::sfSaveIgnoreMasterDetail, TkbmStreamFlagIgnoreMasterDetail::sfSaveIgnoreMasterDetail> TkbmStreamFlagsIgnoreMasterDetail;

typedef System::Set<TkbmStreamFlagDeltas, TkbmStreamFlagDeltas::sfSaveDeltas, TkbmStreamFlagDeltas::sfLoadDeltas> TkbmStreamFlagsDeltas;

typedef System::Set<TkbmStreamFlagDontFilterDeltas, TkbmStreamFlagDontFilterDeltas::sfSaveDontFilterDeltas, TkbmStreamFlagDontFilterDeltas::sfSaveDontFilterDeltas> TkbmStreamFlagsDontFilterDeltas;

typedef System::Set<TkbmStreamFlagAppend, TkbmStreamFlagAppend::sfSaveAppend, TkbmStreamFlagAppend::sfSaveInsert> TkbmStreamFlagsAppend;

typedef System::Set<TkbmStreamFlagFieldKind, TkbmStreamFlagFieldKind::sfSaveFieldKind, TkbmStreamFlagFieldKind::sfLoadFieldKind> TkbmStreamFlagsFieldKind;

typedef System::Set<TkbmStreamFlagFromStart, TkbmStreamFlagFromStart::sfLoadFromStart, TkbmStreamFlagFromStart::sfLoadFromStart> TkbmStreamFlagsFromStart;

typedef System::Set<TkbmStreamFlagDisplayWidth, TkbmStreamFlagDisplayWidth::sfLoadDetermineWidth, TkbmStreamFlagDisplayWidth::sfLoadDetermineWidth> TkbmStreamFlagsDisplayWidth;

typedef void __fastcall (__closure *TkbmOnCompress)(TkbmCustomMemTable* Dataset, System::Classes::TStream* UnCompressedStream, System::Classes::TStream* CompressedStream);

typedef void __fastcall (__closure *TkbmOnDeCompress)(TkbmCustomMemTable* Dataset, System::Classes::TStream* CompressedStream, System::Classes::TStream* DeCompressedStream);

enum DECLSPEC_DENUM TkbmDetermineLoadFieldsSituation : unsigned char { dlfBeforeLoad, dlfAfterLoadDef };

class PASCALIMPLEMENTATION TkbmCustomStreamFormat : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
	
private:
	typedef System::DynamicArray<int> _TkbmCustomStreamFormat__1;
	
	typedef System::DynamicArray<int> _TkbmCustomStreamFormat__2;
	
	
private:
	System::Classes::TStream* FOrigStream;
	System::Classes::TStream* FWorkStream;
	System::DynamicArray<System::Byte> FBookmark;
	TkbmOnCompress FOnCompress;
	TkbmOnDeCompress FOnDecompress;
	bool FWasFiltered;
	bool FWasRangeActive;
	bool FWasMasterLinkUsed;
	bool FWasEnableIndexes;
	bool FWasPersistent;
	TkbmStreamFlagsData FsfData;
	TkbmStreamFlagsCalculated FsfCalculated;
	TkbmStreamFlagsLookup FsfLookup;
	TkbmStreamFlagsNonVisible FsfNonVisible;
	TkbmStreamFlagsBlobs FsfBlobs;
	TkbmStreamFlagsDef FsfDef;
	TkbmStreamFlagsIndexDef FsfIndexDef;
	TkbmStreamFlagsFiltered FsfFiltered;
	TkbmStreamFlagsIgnoreRange FsfIgnoreRange;
	TkbmStreamFlagsIgnoreMasterDetail FsfIgnoreMasterDetail;
	TkbmStreamFlagsDeltas FsfDeltas;
	TkbmStreamFlagsDontFilterDeltas FsfDontFilterDeltas;
	TkbmStreamFlagsAppend FsfAppend;
	TkbmStreamFlagsFieldKind FsfFieldKind;
	TkbmStreamFlagsFromStart FsfFromStart;
	TkbmStreamFlagsDisplayWidth FsfDisplayWidth;
	System::Classes::TNotifyEvent FOnBeforeSave;
	System::Classes::TNotifyEvent FOnAfterSave;
	System::Classes::TNotifyEvent FOnBeforeLoad;
	System::Classes::TNotifyEvent FOnAfterLoad;
	System::Sysutils::TFormatSettings FFormatSettings;
	void __fastcall SetVersion(System::UnicodeString AVersion);
	
protected:
	_TkbmCustomStreamFormat__1 SaveFields;
	_TkbmCustomStreamFormat__1 LoadFields;
	_TkbmCustomStreamFormat__2 LoadFieldWidths;
	void __fastcall SetIgnoreAutoIncPopulation(TkbmCustomMemTable* ADataset, bool Value);
	virtual System::UnicodeString __fastcall GetVersion(void);
	virtual void __fastcall DetermineSaveFields(TkbmCustomMemTable* ADataset);
	virtual void __fastcall BeforeSave(TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveDef(TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveData(TkbmCustomMemTable* ADataset);
	virtual void __fastcall Save(TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterSave(TkbmCustomMemTable* ADataset);
	virtual void __fastcall DetermineLoadFieldIDs(TkbmCustomMemTable* ADataset, System::Classes::TStringList* AList, TkbmDetermineLoadFieldsSituation Situation);
	virtual void __fastcall DetermineLoadFields(TkbmCustomMemTable* ADataset, TkbmDetermineLoadFieldsSituation Situation);
	virtual void __fastcall DetermineLoadFieldIndex(TkbmCustomMemTable* ADataset, System::UnicodeString ID, int FieldCount, int OrigIndex, int &NewIndex, TkbmDetermineLoadFieldsSituation Situation);
	virtual void __fastcall BeforeLoad(TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadDef(TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadData(TkbmCustomMemTable* ADataset);
	virtual void __fastcall Load(TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterLoad(TkbmCustomMemTable* ADataset);
	virtual void __fastcall Reposition(TkbmCustomMemTable* ADataset);
	virtual void __fastcall Refresh(TkbmCustomMemTable* ADataset);
	__property System::Classes::TStream* WorkStream = {read=FWorkStream, write=FWorkStream};
	__property System::Classes::TStream* OrigStream = {read=FOrigStream, write=FOrigStream};
	__property TkbmStreamFlagsData sfData = {read=FsfData, write=FsfData, nodefault};
	__property TkbmStreamFlagsCalculated sfCalculated = {read=FsfCalculated, write=FsfCalculated, nodefault};
	__property TkbmStreamFlagsLookup sfLookup = {read=FsfLookup, write=FsfLookup, nodefault};
	__property TkbmStreamFlagsNonVisible sfNonVisible = {read=FsfNonVisible, write=FsfNonVisible, nodefault};
	__property TkbmStreamFlagsBlobs sfBlobs = {read=FsfBlobs, write=FsfBlobs, nodefault};
	__property TkbmStreamFlagsDef sfDef = {read=FsfDef, write=FsfDef, nodefault};
	__property TkbmStreamFlagsIndexDef sfIndexDef = {read=FsfIndexDef, write=FsfIndexDef, nodefault};
	__property TkbmStreamFlagsFiltered sfFiltered = {read=FsfFiltered, write=FsfFiltered, nodefault};
	__property TkbmStreamFlagsIgnoreRange sfIgnoreRange = {read=FsfIgnoreRange, write=FsfIgnoreRange, nodefault};
	__property TkbmStreamFlagsIgnoreMasterDetail sfIgnoreMasterDetail = {read=FsfIgnoreMasterDetail, write=FsfIgnoreMasterDetail, nodefault};
	__property TkbmStreamFlagsDeltas sfDeltas = {read=FsfDeltas, write=FsfDeltas, nodefault};
	__property TkbmStreamFlagsDontFilterDeltas sfDontFilterDeltas = {read=FsfDontFilterDeltas, write=FsfDontFilterDeltas, nodefault};
	__property TkbmStreamFlagsAppend sfAppend = {read=FsfAppend, write=FsfAppend, nodefault};
	__property TkbmStreamFlagsFieldKind sfFieldKind = {read=FsfFieldKind, write=FsfFieldKind, nodefault};
	__property TkbmStreamFlagsFromStart sfFromStart = {read=FsfFromStart, write=FsfFromStart, nodefault};
	__property TkbmStreamFlagsDisplayWidth sfDisplayWidth = {read=FsfDisplayWidth, write=FsfDisplayWidth, nodefault};
	__property System::UnicodeString Version = {read=GetVersion, write=SetVersion};
	__property System::Classes::TNotifyEvent OnBeforeSave = {read=FOnBeforeSave, write=FOnBeforeSave};
	__property System::Classes::TNotifyEvent OnAfterSave = {read=FOnAfterSave, write=FOnAfterSave};
	__property System::Classes::TNotifyEvent OnBeforeLoad = {read=FOnBeforeLoad, write=FOnBeforeLoad};
	__property System::Classes::TNotifyEvent OnAfterLoad = {read=FOnAfterLoad, write=FOnAfterLoad};
	__property TkbmOnCompress OnCompress = {read=FOnCompress, write=FOnCompress};
	__property TkbmOnDeCompress OnDeCompress = {read=FOnDecompress, write=FOnDecompress};
	
public:
	__fastcall virtual TkbmCustomStreamFormat(System::Classes::TComponent* AOwner);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property System::Sysutils::TFormatSettings FormatSettings = {read=FFormatSettings, write=FFormatSettings};
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmCustomStreamFormat(void) { }
	
};


class PASCALIMPLEMENTATION TkbmStreamFormat : public TkbmCustomStreamFormat
{
	typedef TkbmCustomStreamFormat inherited;
	
__published:
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
	__property Version = {default=0};
	__property OnBeforeLoad;
	__property OnAfterLoad;
	__property OnBeforeSave;
	__property OnAfterSave;
	__property OnCompress;
	__property OnDeCompress;
	
public:
	__property FormatSettings;
public:
	/* TkbmCustomStreamFormat.Create */ inline __fastcall virtual TkbmStreamFormat(System::Classes::TComponent* AOwner) : TkbmCustomStreamFormat(AOwner) { }
	
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmStreamFormat(void) { }
	
};


enum DECLSPEC_DENUM TkbmCompareHow : unsigned char { chBreakNE, chBreakLT, chBreakGT, chBreakLTE, chBreakGTE };

typedef System::Syncobjs::TCriticalSection TkbmCS;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmCommon : public System::TObject
{
	typedef System::TObject inherited;
	
	
private:
	typedef System::DynamicArray<int> _TkbmCommon__1;
	
	typedef System::DynamicArray<System::Byte> _TkbmCommon__2;
	
	
protected:
	bool FStandalone;
	Kbmlist::TkbmList* FRecords;
	int FFieldCount;
	_TkbmCommon__1 FFieldOfs;
	_TkbmCommon__2 FFieldFlags;
	int FLanguageID;
	int FSubLanguageID;
	int FSortID;
	NativeInt FDataID;
	int FLocaleID;
	int FBookmarkArraySize;
	int FFixedRecordSize;
	int FTotalRecordSize;
	int FDataRecordSize;
	int FCalcRecordSize;
	int FVarLengthRecordSize;
	int FStartCalculated;
	int FStartBookmarks;
	int FStartVarLength;
	int FVarLengthCount;
	bool FIsDataModified;
	int FAutoIncMin;
	int FAutoIncMax;
	NativeInt FDeletedCount;
	TkbmPerformance FPerformance;
	int FAttachMaxCount;
	Kbmlist::TkbmList* FDeletedRecords;
	TkbmVersioningMode FVersioningMode;
	bool FEnableVersioning;
	int FTransactionLevel;
	bool FThreadProtected;
	System::Classes::TList* FAttachedTables;
	TkbmCustomMemTable* FOwner;
	Kbmmemtypes::PkbmRecord __fastcall _InternalCopyRecord(Kbmmemtypes::PkbmRecord SourceRecord, bool CopyVarLengths);
	void __fastcall _InternalCopyVarLength(Kbmmemtypes::PkbmRecord SourceRecord, Kbmmemtypes::PkbmRecord DestRecord, Data::Db::TField* Field);
	void __fastcall _InternalCopyVarLengths(Kbmmemtypes::PkbmRecord SourceRec, Kbmmemtypes::PkbmRecord DestRec);
	void __fastcall _InternalMoveRecord(Kbmmemtypes::PkbmRecord SourceRecord, Kbmmemtypes::PkbmRecord DestRecord);
	void __fastcall _InternalTransferRecord(Kbmmemtypes::PkbmRecord SourceRecord, Kbmmemtypes::PkbmRecord DestRecord);
	void __fastcall _InternalFreeRecordVarLengths(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall _InternalClearRecord(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall _InternalAppendRecord(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall _InternalDeleteRecord(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall _InternalPackRecords(void);
	void __fastcall _InternalEmpty(void);
	int __fastcall _InternalCompareRecords(TkbmFieldList* const AFieldList, const int AMaxFields, const Kbmmemtypes::PkbmRecord AKeyRecord, const Kbmmemtypes::PkbmRecord ARecord, const bool AIgnoreNull, const bool APartial, const bool AUniqueConstraint, const TkbmCompareHow AHow);
	void __fastcall SetStandalone(bool Value);
	bool __fastcall GetStandalone(void);
	void __fastcall SetAutoIncMin(NativeInt Value);
	NativeInt __fastcall GetAutoIncMin(void);
	void __fastcall SetAutoIncMax(NativeInt Value);
	NativeInt __fastcall GetAutoIncMax(void);
	void __fastcall SetPerformance(TkbmPerformance Value);
	TkbmPerformance __fastcall GetPerformance(void);
	void __fastcall SetVersioningMode(TkbmVersioningMode Value);
	TkbmVersioningMode __fastcall GetVersioningMode(void);
	void __fastcall SetEnableVersioning(bool Value);
	bool __fastcall GetEnableVersioning(void);
	void __fastcall SetCapacity(NativeInt Value);
	NativeInt __fastcall GetCapacity(void);
	int __fastcall GetTransactionLevel(void);
	bool __fastcall GetIsDataModified(void);
	void __fastcall SetIsDataModified(bool Value);
	void __fastcall ClearModifiedFlags(void);
	bool __fastcall GetModifiedFlag(int i);
	void __fastcall SetModifiedFlag(int i, bool Value);
	int __fastcall GetAttachMaxCount(void);
	void __fastcall SetAttachMaxCount(int Value);
	int __fastcall GetAttachCount(void);
	void __fastcall EnsureFields(const int ACount);
	void __fastcall SetRecordID(NativeInt ARecordID);
	void __fastcall SetUniqueRecordID(NativeInt ARecordID);
	void __fastcall SetDeletedCount(NativeInt ACount);
	int __fastcall GetLanguageID(void);
	void __fastcall SetLanguageID(int Value);
	int __fastcall GetSortID(void);
	void __fastcall SetSortID(int Value);
	int __fastcall GetSubLanguageID(void);
	void __fastcall SetSubLanguageID(int Value);
	int __fastcall GetLocaleID(void);
	void __fastcall SetLocaleID(int Value);
	void __fastcall CalcLocaleID(void);
	NativeInt __fastcall GetUniqueDataID(void);
	
public:
	System::Syncobjs::TCriticalSection* FLock;
	NativeInt FUniqueRecordID;
	NativeInt FRecordID;
	NativeInt __fastcall GetDeletedRecordsCount(void);
	int __fastcall GetFieldSize(Data::Db::TFieldType FieldType, int Size);
	int __fastcall GetFieldDataOffset(Data::Db::TField* Field);
	System::PByte __fastcall GetFieldPointer(Kbmmemtypes::PkbmRecord ARecord, Data::Db::TField* Field);
	Kbmmemtypes::PkbmRecord __fastcall _InternalAllocRecord(void);
	void __fastcall _InternalFreeRecord(Kbmmemtypes::PkbmRecord ARecord, bool FreeVarLengths, bool FreeVersions);
	__fastcall TkbmCommon(TkbmCustomMemTable* AOwner);
	__fastcall virtual ~TkbmCommon(void);
	void __fastcall Lock(void);
	void __fastcall Unlock(void);
	bool __fastcall GetFieldIsVarLength(Data::Db::TFieldType FieldType, int Size);
	void * __fastcall CompressFieldBuffer(Data::Db::TField* Field, const void * Buffer, int &Size);
	void * __fastcall DecompressFieldBuffer(Data::Db::TField* Field, const void * Buffer, int &Size);
	void __fastcall DeAttachAllTables(TkbmCustomMemTable* AExceptTable);
	void __fastcall AttachTable(TkbmCustomMemTable* ATable);
	void __fastcall DeAttachTable(TkbmCustomMemTable* ATable);
	void __fastcall LayoutRecord(const int AFieldCount);
	void __fastcall AppendRecord(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall DeleteRecord(Kbmmemtypes::PkbmRecord ARecord);
	void __fastcall PackRecords(void);
	NativeInt __fastcall RecordCount(void);
	NativeInt __fastcall DeletedRecordCount(void);
	void __fastcall Rollback(void);
	void __fastcall Commit(void);
	void __fastcall Undo(Kbmmemtypes::PkbmRecord ARecord);
	bool __fastcall IsAnyTableActive(void);
	void __fastcall CloseTables(TkbmCustomMemTable* Caller);
	void __fastcall RefreshTables(TkbmCustomMemTable* Caller);
	void __fastcall ResyncTables(void);
	void __fastcall EmptyTables(void);
	void __fastcall RebuildIndexes(void);
	void __fastcall MarkIndexesDirty(void);
	void __fastcall UpdateIndexes(void);
	void __fastcall ClearIndexes(void);
	void __fastcall ReflectToIndexes(TkbmCustomMemTable* const Caller, const TkbmIndexUpdateHow How, const Kbmmemtypes::PkbmRecord OldRecord, const Kbmmemtypes::PkbmRecord NewRecord, const int RecordPos, const bool DontVersion);
	void __fastcall IncTransactionLevel(void);
	void __fastcall DecTransactionLevel(void);
	__property int DataRecordSize = {read=FDataRecordSize, nodefault};
	__property Kbmlist::TkbmList* Records = {read=FRecords};
	__property NativeInt RecordID = {read=FRecordID, write=SetRecordID, nodefault};
	__property NativeInt UniqueRecordID = {read=FUniqueRecordID, write=SetUniqueRecordID, nodefault};
	__property NativeInt DeletedCount = {read=FDeletedCount, write=SetDeletedCount, nodefault};
	__property int AttachMaxCount = {read=GetAttachMaxCount, write=SetAttachMaxCount, nodefault};
	__property int AttachCount = {read=GetAttachCount, nodefault};
	__property bool Standalone = {read=GetStandalone, write=SetStandalone, nodefault};
	__property NativeInt AutoIncMin = {read=GetAutoIncMin, write=SetAutoIncMin, nodefault};
	__property NativeInt AutoIncMax = {read=GetAutoIncMax, write=SetAutoIncMax, nodefault};
	__property TkbmPerformance Performance = {read=GetPerformance, write=SetPerformance, nodefault};
	__property TkbmVersioningMode VersioningMode = {read=GetVersioningMode, write=SetVersioningMode, nodefault};
	__property bool EnableVersioning = {read=GetEnableVersioning, write=SetEnableVersioning, nodefault};
	__property NativeInt Capacity = {read=GetCapacity, write=SetCapacity, nodefault};
	__property bool IsDataModified = {read=GetIsDataModified, write=SetIsDataModified, nodefault};
	__property int TransactionLevel = {read=GetTransactionLevel, nodefault};
	__property bool FieldModified[int i] = {read=GetModifiedFlag, write=SetModifiedFlag};
	__property int LanguageID = {read=GetLanguageID, write=SetLanguageID, nodefault};
	__property int SortID = {read=GetSortID, write=SetSortID, nodefault};
	__property int SubLanguageID = {read=GetSubLanguageID, write=SetSubLanguageID, nodefault};
	__property int LocaleID = {read=GetLocaleID, write=SetLocaleID, nodefault};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TkbmMasterDataLink : public Data::Db::TMasterDataLink
{
	typedef Data::Db::TMasterDataLink inherited;
	
protected:
	virtual void __fastcall RecordChanged(Data::Db::TField* Field);
public:
	/* TMasterDataLink.Create */ inline __fastcall TkbmMasterDataLink(Data::Db::TDataSet* DataSet) : Data::Db::TMasterDataLink(DataSet) { }
	/* TMasterDataLink.Destroy */ inline __fastcall virtual ~TkbmMasterDataLink(void) { }
	
};


typedef System::TMetaClass* TkbmMasterDataLinkClass;

class PASCALIMPLEMENTATION TkbmCustomMemTable : public Data::Db::TDataSet
{
	typedef Data::Db::TDataSet inherited;
	
protected:
	int FTableID;
	TkbmIndexes* FIndexes;
	TkbmCustomStreamFormat* FDefaultFormat;
	TkbmCustomStreamFormat* FCommaTextFormat;
	TkbmCustomStreamFormat* FPersistentFormat;
	TkbmCustomStreamFormat* FFormFormat;
	TkbmCustomStreamFormat* FAllDataFormat;
	Kbmmemtypes::TkbmRecord *FFilterRecord;
	Kbmmemtypes::TkbmRecord *FKeyRecord;
	System::StaticArray<Kbmmemtypes::PkbmRecord, 4> FKeyBuffers;
	bool FIgnoreReadOnly;
	bool FIgnoreAutoIncPopulation;
	Data::Db::TIndexDefs* FIndexDefs;
	TkbmIndex* FCurIndex;
	TkbmIndex* FSortIndex;
	TkbmIndex* FDetailIndex;
	TkbmIndex* FRangeIndex;
	TkbmIndex* FGroupIndex;
	bool FGroupIndexOwned;
	bool FEnableIndexes;
	bool FAutoAddIndexes;
	bool FDesignActivation;
	bool FInterceptActive;
	bool FAutoUpdateFieldVariables;
	TkbmState FState;
	TkbmExprParser* FFilterParser;
	Data::Db::TFilterOptions FFilterOptions;
	bool FMasterDetailRelationChanged;
	TkbmMasterDataLink* FMasterLink;
	bool FMasterLinkUsed;
	bool FIsOpen;
	NativeInt FRecNo;
	NativeInt FReposRecNo;
	NativeInt FInsertRecNo;
	bool FBeforeCloseCalled;
	bool FDuringAfterOpen;
	NativeInt FLoadLimit;
	NativeInt FLoadCount;
	bool FLoadedCompletely;
	NativeInt FSaveLimit;
	NativeInt FSaveCount;
	bool FSavedCompletely;
	TkbmCustomDeltaHandler* FDeltaHandler;
	Kbmmemtypes::TkbmRecord *FOverrideActiveRecordBuffer;
	Data::Db::TUpdateStatusSet FStatusFilter;
	TkbmCustomMemTable* FAttachedTo;
	bool FAttachedAutoRefresh;
	Data::Db::TField* FAutoIncField;
	bool FRecalcOnFetch;
	bool FReadOnly;
	bool FPersistent;
	System::Sysutils::TFileName FPersistentFile;
	bool FPersistentSaved;
	bool FPersistentBackup;
	System::UnicodeString FPersistentBackupExt;
	bool FStoreDataOnForm;
	System::Classes::TMemoryStream* FTempDataStorage;
	System::UnicodeString FDummyStr;
	TkbmFieldList* FDetailIndexList;
	TkbmFieldList* FIndexList;
	bool FRecalcOnIndex;
	System::UnicodeString FIndexFieldNames;
	System::UnicodeString FDetailFieldNames;
	System::UnicodeString FIndexName;
	System::UnicodeString FSortFieldNames;
	bool FAutoReposition;
	bool FRangeIgnoreNullKeyValues;
	System::UnicodeString FSortedOn;
	TkbmMemTableCompareOptions FSortOptions;
	TkbmOnCompareFields FOnCompareFields;
	TkbmOnSave FOnSave;
	TkbmOnLoad FOnLoad;
	TkbmProgressCodes FProgressFlags;
	TkbmOnProgress FOnProgress;
	TkbmOnLoadRecord FOnLoadRecord;
	TkbmOnSaveRecord FOnSaveRecord;
	TkbmOnLoadField FOnLoadField;
	TkbmOnSaveField FOnSaveField;
	TkbmOnCompress FOnCompressBlobStream;
	TkbmOnDeCompress FOnDecompressBlobStream;
	TkbmOnSetupField FOnSetupField;
	TkbmOnSetupFieldProperties FOnSetupFieldProperties;
	TkbmOnCompressField FOnCompressField;
	TkbmOnDecompressField FOnDecompressField;
	Data::Db::TDataSetNotifyEvent FBeforeInsert;
	TkbmOnFilterIndex FOnFilterIndex;
	TkbmOnUserAggregate FOnUserAggregate;
	bool FIsFiltered;
	System::Sysutils::TFormatSettings FFormatSettings;
	TkbmCommon* FCommon;
	__classmethod virtual TkbmMasterDataLinkClass __fastcall GetMasterDataLinkClass();
	void __fastcall _InternalBeforeInsert(Data::Db::TDataSet* DataSet);
	Kbmmemtypes::PkbmRecord __fastcall GetActiveRecord(void);
	virtual void __fastcall _InternalFirst(void);
	virtual void __fastcall _InternalLast(void);
	virtual bool __fastcall _InternalNext(bool ForceUseFilter);
	virtual bool __fastcall _InternalPrior(bool ForceUseFilter);
	virtual void __fastcall IndexFreed(TkbmIndex* AIndex);
	void __fastcall FreeGroupIndex(void);
	void __fastcall CreateGroupIndex(System::UnicodeString AGroupFields);
	void __fastcall FreeDetailIndex(void);
	void __fastcall CreateDetailIndex(void);
	void __fastcall SetMasterFields(const System::UnicodeString Value);
	void __fastcall SetDetailFields(const System::UnicodeString Value);
	System::UnicodeString __fastcall GetMasterFields(void);
	void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	void __fastcall FreeRangeIndex(void);
	void __fastcall CreateRangeIndex(System::UnicodeString AFieldNames = System::UnicodeString());
	bool __fastcall GetRangeActive(void);
	virtual void __fastcall SetIsFiltered(void);
	__property bool IsFiltered = {read=FIsFiltered, nodefault};
	void __fastcall BuildFilter(TkbmExprParser* &AFilterParser, System::UnicodeString AFilter, Data::Db::TFilterOptions AFilterOptions);
	void __fastcall FreeFilter(TkbmExprParser* &AFilterParser);
	void __fastcall DrawAutoInc(Kbmmemtypes::PkbmRecord ARecord = (Kbmmemtypes::PkbmRecord)(0x0));
	void __fastcall PostAutoInc(Kbmmemtypes::PkbmRecord ARecord = (Kbmmemtypes::PkbmRecord)(0x0));
	System::UnicodeString __fastcall GetVersion(void);
	void __fastcall SetIndexFieldNames(System::UnicodeString FieldNames);
	void __fastcall SetIndexName(System::UnicodeString IndexName);
	void __fastcall SetIndexDefs(Data::Db::TIndexDefs* Value);
	void __fastcall SetCommaText(System::UnicodeString AString);
	System::UnicodeString __fastcall GetCommaText(void);
	TkbmIndex* __fastcall GetIndexByName(System::UnicodeString IndexName);
	Data::Db::TField* __fastcall GetIndexField(int Index);
	void __fastcall SetIndexField(int Index, Data::Db::TField* Value);
	void __fastcall SetAttachedTo(TkbmCustomMemTable* Value);
	void __fastcall SetRecordTag(NativeInt Value);
	NativeInt __fastcall GetRecordTag(void);
	bool __fastcall GetIsVersioning(void);
	void __fastcall SetStatusFilter(const Data::Db::TUpdateStatusSet Value);
	void __fastcall SetDeltaHandler(TkbmCustomDeltaHandler* AHandler);
	void __fastcall SetAllData(const System::Variant &AVariant);
	System::Variant __fastcall GetAllData(void);
	NativeInt __fastcall GetAutoIncValue(void);
	NativeInt __fastcall GetAutoIncMin(void);
	void __fastcall SetAutoIncMinValue(NativeInt AValue);
	void __fastcall SetAutoUpdateFieldVariables(bool AValue);
	TkbmPerformance __fastcall GetPerformance(void);
	void __fastcall SetPerformance(TkbmPerformance AValue);
	TkbmVersioningMode __fastcall GetVersioningMode(void);
	void __fastcall SetVersioningMode(TkbmVersioningMode AValue);
	bool __fastcall GetEnableVersioning(void);
	void __fastcall SetEnableVersioning(bool AValue);
	bool __fastcall GetStandalone(void);
	void __fastcall SetStandalone(bool AValue);
	NativeInt __fastcall GetCapacity(void);
	void __fastcall SetCapacity(NativeInt AValue);
	bool __fastcall GetIsDataModified(void);
	void __fastcall SetIsDataModified(bool AValue);
	int __fastcall GetAttachMaxCount(void);
	void __fastcall SetAttachMaxCount(int AValue);
	int __fastcall GetAttachCount(void);
	bool __fastcall GetModifiedFlags(int i);
	TkbmIndexes* __fastcall GetIndexes(void);
	int __fastcall GetTransactionLevel(void);
	NativeInt __fastcall GetDeletedRecordsCount(void);
	int __fastcall GetLanguageID(void);
	void __fastcall SetLanguageID(int Value);
	int __fastcall GetSortID(void);
	void __fastcall SetSortID(int Value);
	int __fastcall GetSubLanguageID(void);
	void __fastcall SetSubLanguageID(int Value);
	int __fastcall GetLocaleID(void);
	void __fastcall SetLocaleID(int Value);
	virtual void __fastcall SetActive(bool Value);
	virtual void __fastcall DoCheckInActive(void);
	virtual void __fastcall RebuildFieldLists(void);
	virtual void __fastcall InternalOpen(void);
	virtual void __fastcall InternalClose(void);
	virtual void __fastcall InternalFirst(void);
	virtual void __fastcall InternalLast(void);
	virtual void __fastcall InternalAddRecord(System::PByte Buffer, bool Append)/* overload */;
	virtual void __fastcall InternalDelete(void);
	virtual void __fastcall InternalInitRecord(System::PByte Buffer)/* overload */;
	virtual void __fastcall InternalPost(void);
	virtual void __fastcall InternalCancel(void);
	virtual void __fastcall InternalEdit(void);
	virtual void __fastcall InternalInsert(void);
	virtual void __fastcall InternalInitFieldDefs(void);
	virtual void __fastcall InternalInitFieldDefsOnOpen(void);
	virtual void __fastcall InternalCreateFieldsOnOpen(void);
	virtual void __fastcall InternalSetToRecord(System::PByte Buffer)/* overload */;
	virtual void __fastcall CheckActive(void);
	virtual void __fastcall CheckInactive(void);
	virtual void __fastcall DoBeforeClose(void);
	virtual void __fastcall DoBeforeOpen(void);
	virtual void __fastcall DoAfterOpen(void);
	virtual void __fastcall DoAfterPost(void);
	virtual void __fastcall DoAfterDelete(void);
	virtual void __fastcall DoOnNewRecord(void);
	virtual void __fastcall DoBeforePost(void);
	virtual void __fastcall DoOnFilterRecord(Data::Db::TDataSet* ADataset, bool &AFiltered);
	virtual bool __fastcall IsCursorOpen(void);
	virtual bool __fastcall GetCanModify(void);
	virtual System::Word __fastcall GetRecordSize(void);
	virtual int __fastcall GetRecordCount(void);
	virtual System::PByte __fastcall AllocRecordBuffer(void);
	virtual void __fastcall FreeRecordBuffer(System::PByte &Buffer);
	virtual void __fastcall CloseBlob(Data::Db::TField* Field);
	virtual void __fastcall SetFieldData(Data::Db::TField* Field, System::DynamicArray<System::Byte> Buffer)/* overload */;
	virtual Data::Db::TGetResult __fastcall GetRecord(NativeInt Buffer, Data::Db::TGetMode GetMode, bool DoCheck)/* overload */;
	virtual bool __fastcall FindRecord(bool Restart, bool GoForward);
	virtual int __fastcall GetRecNo(void);
	virtual void __fastcall SetRecNo(int Value);
	virtual bool __fastcall GetIsIndexField(Data::Db::TField* Field);
	virtual Data::Db::TBookmarkFlag __fastcall GetBookmarkFlag(System::PByte Buffer)/* overload */;
	virtual void __fastcall SetBookmarkFlag(System::PByte Buffer, Data::Db::TBookmarkFlag Value)/* overload */;
	virtual void __fastcall GetBookmarkData(System::PByte Buffer, System::DynamicArray<System::Byte> Data)/* overload */;
	virtual void __fastcall SetBookmarkData(System::PByte Buffer, System::DynamicArray<System::Byte> Data)/* overload */;
	virtual void __fastcall InternalGotoBookmark(void * Bookmark)/* overload */;
	virtual void __fastcall InternalHandleException(void);
	virtual Data::Db::TDataSource* __fastcall GetDataSource(void);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall SetFiltered(bool Value);
	virtual void __fastcall SetFilterText(const System::UnicodeString Value);
	void __fastcall SetLoadedCompletely(bool Value);
	void __fastcall SetTableState(TkbmState AValue);
	void __fastcall CreateFieldDefs(void);
	virtual void __fastcall SetOnFilterRecord(const Data::Db::TFilterRecordEvent Value);
	virtual void __fastcall ClearCalcFields(NativeInt Buffer)/* overload */;
	virtual void __fastcall DataEvent(Data::Db::TDataEvent Event, NativeInt Info);
	virtual void __fastcall Loaded(void);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	void __fastcall ReadData(System::Classes::TStream* Stream);
	void __fastcall WriteData(System::Classes::TStream* Stream);
	void __fastcall InternalEmptyTable(void);
	void __fastcall PopulateField(Kbmmemtypes::PkbmRecord ARecord, Data::Db::TField* Field, const System::Variant &AValue);
	void __fastcall PopulateRecord(Kbmmemtypes::PkbmRecord ARecord, System::UnicodeString Fields, const System::Variant &Values);
	void __fastcall PopulateVarLength(Kbmmemtypes::PkbmRecord ARecord, Data::Db::TField* Field, const void *Buffer, int Size);
	bool __fastcall InternalBookmarkValid(void * Bookmark);
	void __fastcall PrepareKeyRecord(int KeyRecordType, bool Clear);
	bool __fastcall FilterExpression(Kbmmemtypes::PkbmRecord ARecord, TkbmExprParser* AFilterParser);
	virtual void __fastcall MasterChanged(System::TObject* Sender);
	virtual void __fastcall MasterDisabled(System::TObject* Sender);
	virtual void __fastcall InternalSaveToStreamViaFormat(System::Classes::TStream* AStream, TkbmCustomStreamFormat* AFormat);
	virtual void __fastcall InternalLoadFromStreamViaFormat(System::Classes::TStream* AStream, TkbmCustomStreamFormat* AFormat);
	NativeInt __fastcall UpdateRecords(Data::Db::TDataSet* Source, Data::Db::TDataSet* Destination, System::UnicodeString KeyFields, NativeInt Count, TkbmMemTableUpdateFlags Flags, System::UnicodeString Mapping);
	int __fastcall LocateRecord(const System::UnicodeString KeyFields, const System::Variant &KeyValues, Data::Db::TLocateOptions Options);
	bool __fastcall CheckAutoInc(void);
	virtual void __fastcall SetBlockReadSize(int Value);
	__property System::Sysutils::TFormatSettings FormatSettings = {read=FFormatSettings, write=FFormatSettings};
	
public:
	int __fastcall __CalcFieldsSize(void);
	void __fastcall __ClearBuffers(void);
	void __fastcall __ClearCalcFields(Kbmmemtypes::PkbmRecord Buffer);
	void __fastcall __GetCalcFields(Kbmmemtypes::PkbmRecord Buffer);
	void __fastcall __SetBlockReadSize(int Value);
	Data::Db::TDataSetState __fastcall __SetTempState(const Data::Db::TDataSetState Value);
	void __fastcall __RestoreState(const Data::Db::TDataSetState Value);
	__fastcall virtual TkbmCustomMemTable(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TkbmCustomMemTable(void);
	virtual bool __fastcall BookmarkValid(System::DynamicArray<System::Byte> Bookmark);
	virtual int __fastcall CompareBookmarks(System::DynamicArray<System::Byte> Bookmark1, System::DynamicArray<System::Byte> Bookmark2);
	virtual bool __fastcall GetFieldData(Data::Db::TField* Field, System::DynamicArray<System::Byte> &Buffer)/* overload */;
	virtual Data::Db::TUpdateStatus __fastcall UpdateStatus(void);
	virtual System::Classes::TStream* __fastcall CreateBlobStream(Data::Db::TField* Field, Data::Db::TBlobStreamMode Mode);
	virtual bool __fastcall IsSequenced(void);
	void __fastcall SavePersistent(void);
	void __fastcall LoadPersistent(void);
	__classmethod virtual System::UnicodeString __fastcall GetAggregateFieldName(const System::UnicodeString AFieldName, const TkbmifoOptions AOptions);
	__classmethod virtual TkbmifoOptions __fastcall GetAggregateFieldOption(const System::UnicodeString AAggregateFunction);
	__classmethod virtual TkbmifoOptions __fastcall GetExtractFieldOption(const System::UnicodeString AExtractOption);
	void __fastcall ClearModified(void);
	void __fastcall DestroyIndexes(void);
	void __fastcall CreateIndexes(void);
	bool __fastcall FilterRecord(Kbmmemtypes::PkbmRecord ARecord, bool ForceUseFilter);
	bool __fastcall IsMasterDetailActive(void);
	void __fastcall SwitchToIndex(TkbmIndex* Index);
	Data::Db::TField* __fastcall CreateFieldAs(Data::Db::TField* Field, const bool AForceAsData = false);
	bool __fastcall MoveRecord(NativeInt Source, NativeInt Destination);
	bool __fastcall CopyRecord(NativeInt Source, NativeInt Destination);
	bool __fastcall MoveCurRecord(NativeInt Destination);
	System::Variant __fastcall GetVersionFieldData(Data::Db::TField* Field, int Version);
	Data::Db::TUpdateStatus __fastcall GetVersionStatus(int Version);
	int __fastcall GetVersionCount(void);
	System::Variant __fastcall SetVersionFieldData(Data::Db::TField* Field, int AVersion, const System::Variant &AValue);
	Data::Db::TUpdateStatus __fastcall SetVersionStatus(int AVersion, Data::Db::TUpdateStatus AUpdateStatus);
	void __fastcall ResetAutoInc(void);
	virtual void __fastcall Progress(int Pct, TkbmProgressCode Code);
	virtual NativeInt __fastcall CopyRecords(Data::Db::TDataSet* Source, Data::Db::TDataSet* Destination, NativeInt Count, bool IgnoreErrors, System::UnicodeString Mapping, bool WideStringAsUTF8);
	void __fastcall AssignRecord(Data::Db::TDataSet* Source, Data::Db::TDataSet* Destination);
	virtual void __fastcall Lock(void);
	virtual void __fastcall Unlock(void);
	void __fastcall UpdateFieldVariables(void);
	void __fastcall CopyFieldProperties(Data::Db::TField* Source, Data::Db::TField* Destination);
	void __fastcall CopyFieldsProperties(Data::Db::TDataSet* Source, Data::Db::TDataSet* Destination);
	bool __fastcall Exists(void);
	void __fastcall CreateTable(void);
	void __fastcall EmptyTable(void);
	void __fastcall CreateTableAs(Data::Db::TDataSet* Source, TkbmMemTableCopyTableOptions CopyOptions);
	void __fastcall DeleteTable(void);
	void __fastcall PackTable(void);
	TkbmIndex* __fastcall AddIndex(const System::UnicodeString Name, const System::UnicodeString Fields, Data::Db::TIndexOptions Options)/* overload */;
	TkbmIndex* __fastcall AddIndex(const System::UnicodeString Name, const System::UnicodeString Fields, Data::Db::TIndexOptions Options, Data::Db::TUpdateStatusSet AUpdateStatus)/* overload */;
	TkbmIndex* __fastcall AddIndex2(const System::UnicodeString Name, const System::UnicodeString Fields, TkbmMemTableCompareOptions Options)/* overload */;
	TkbmIndex* __fastcall AddIndex2(const System::UnicodeString Name, const System::UnicodeString Fields, TkbmMemTableCompareOptions Options, Data::Db::TUpdateStatusSet AUpdateStatus)/* overload */;
	TkbmIndex* __fastcall AddFilteredIndex(const System::UnicodeString Name, const System::UnicodeString Fields, Data::Db::TIndexOptions Options, System::UnicodeString Filter, Data::Db::TFilterOptions FilterOptions, TkbmOnFilterIndex FilterFunc = 0x0)/* overload */;
	TkbmIndex* __fastcall AddFilteredIndex(const System::UnicodeString Name, const System::UnicodeString Fields, Data::Db::TIndexOptions Options, Data::Db::TUpdateStatusSet AUpdateStatus, System::UnicodeString Filter, Data::Db::TFilterOptions FilterOptions, TkbmOnFilterIndex FilterFunc = 0x0)/* overload */;
	TkbmIndex* __fastcall AddFilteredIndex2(const System::UnicodeString Name, const System::UnicodeString Fields, TkbmMemTableCompareOptions Options, System::UnicodeString Filter, Data::Db::TFilterOptions FilterOptions, TkbmOnFilterIndex FilterFunc = 0x0)/* overload */;
	TkbmIndex* __fastcall AddFilteredIndex2(const System::UnicodeString Name, const System::UnicodeString Fields, TkbmMemTableCompareOptions Options, Data::Db::TUpdateStatusSet AUpdateStatus, System::UnicodeString Filter, Data::Db::TFilterOptions FilterOptions, TkbmOnFilterIndex FilterFunc = 0x0)/* overload */;
	void __fastcall DeleteIndex(const System::UnicodeString Name);
	void __fastcall UpdateIndexes(void);
	int __fastcall IndexFieldCount(void);
	virtual void __fastcall StartTransaction(void);
	virtual void __fastcall Commit(void);
	virtual void __fastcall Rollback(void);
	bool __fastcall TestFilter(const System::UnicodeString AFilter, Data::Db::TFilterOptions AFilterOptions);
	void __fastcall Undo(void);
	void __fastcall LoadFromFile(const System::UnicodeString FileName);
	void __fastcall LoadFromStream(System::Classes::TStream* Stream);
	void __fastcall LoadFromFileViaFormat(const System::UnicodeString FileName, TkbmCustomStreamFormat* AFormat);
	void __fastcall LoadFromStreamViaFormat(System::Classes::TStream* Stream, TkbmCustomStreamFormat* AFormat);
	void __fastcall SaveToFile(const System::UnicodeString FileName);
	void __fastcall SaveToStream(System::Classes::TStream* Stream);
	void __fastcall SaveToFileViaFormat(const System::UnicodeString FileName, TkbmCustomStreamFormat* AFormat);
	void __fastcall SaveToStreamViaFormat(System::Classes::TStream* Stream, TkbmCustomStreamFormat* AFormat);
	virtual void __fastcall LoadFromDataSet(Data::Db::TDataSet* Source, TkbmMemTableCopyTableOptions CopyOptions, System::UnicodeString Mapping = System::UnicodeString());
	virtual void __fastcall SaveToDataSet(Data::Db::TDataSet* Destination, TkbmMemTableCopyTableOptions CopyOptions = TkbmMemTableCopyTableOptions() , System::UnicodeString Mapping = System::UnicodeString());
	virtual void __fastcall UpdateToDataSet(Data::Db::TDataSet* Destination, System::UnicodeString KeyFields, TkbmMemTableUpdateFlags Flags, System::UnicodeString Mapping = System::UnicodeString())/* overload */;
	virtual void __fastcall UpdateToDataSet(Data::Db::TDataSet* Destination, System::UnicodeString KeyFields, System::UnicodeString Mapping = System::UnicodeString())/* overload */;
	void __fastcall SortDefault(void);
	void __fastcall Sort(TkbmMemTableCompareOptions Options);
	void __fastcall SortOn(const System::UnicodeString FieldNames, TkbmMemTableCompareOptions Options);
	virtual System::Variant __fastcall Lookup(const System::UnicodeString KeyFields, const System::Variant &KeyValues, const System::UnicodeString ResultFields);
	System::Variant __fastcall LookupByIndex(const System::UnicodeString IndexName, const System::Variant &KeyValues, const System::UnicodeString ResultFields, bool RespFilter);
	virtual bool __fastcall Locate(const System::UnicodeString KeyFields, const System::Variant &KeyValues, Data::Db::TLocateOptions Options);
	void __fastcall SetKey(void);
	void __fastcall EditKey(void);
	bool __fastcall GotoNearest(void);
	bool __fastcall GotoKey(void);
	bool __fastcall FindKey(System::TVarRec const *KeyValues, const int KeyValues_High);
	bool __fastcall FindNearest(System::TVarRec const *KeyValues, const int KeyValues_High);
	virtual int __fastcall DeleteRecords(void);
	void __fastcall ApplyRange(void);
	void __fastcall CancelRange(void);
	void __fastcall SetRange(System::TVarRec const *StartValues, const int StartValues_High, System::TVarRec const *EndValues, const int EndValues_High)/* overload */;
	void __fastcall SetRange(const System::UnicodeString AFields, System::Variant const *StartValues, const int StartValues_High, System::Variant const *EndValues, const int EndValues_High)/* overload */;
	int __fastcall DeleteRange(const System::UnicodeString AFields, System::Variant const *StartValues, const int StartValues_High, System::Variant const *EndValues, const int EndValues_High);
	void __fastcall SetRangeStart(void);
	void __fastcall SetRangeEnd(void);
	void __fastcall EditRangeStart(void);
	void __fastcall EditRangeEnd(void);
	void __fastcall CheckPoint(const bool AForce = false, const bool AMarkAsInserted = false);
	Data::Db::TUpdateStatus __fastcall CheckPointRecord(int RecordIndex, const bool AForce = false, const bool AMarkAsInserted = false);
	System::Variant __fastcall GetRows(int Rows, const System::Variant &Start, const System::Variant &Fields);
	void __fastcall Reset(void);
	virtual void __fastcall GotoCurrent(TkbmCustomMemTable* DataSet);
	virtual void __fastcall GroupBy(TkbmCustomMemTable* ADestDataset, System::UnicodeString AGroupFields, System::UnicodeString AAggregateFields)/* overload */;
	virtual void __fastcall GroupBy(TkbmCustomMemTable* ADestDataset, TkbmFieldList* ASourceGroupFieldList, TkbmFieldList* ASourceAggregateFieldList, TkbmFieldList* ADestAggregateFieldList)/* overload */;
	virtual System::Variant __fastcall Aggregate(System::UnicodeString AAggregateFields);
	int __fastcall Extract(System::UnicodeString AExtractFields, System::Classes::TStrings* AStringList, System::UnicodeString AFormat = System::UnicodeString());
	__property Kbmmemtypes::PkbmRecord OverrideActiveRecordBuffer = {read=FOverrideActiveRecordBuffer, write=FOverrideActiveRecordBuffer};
	__property bool PersistentSaved = {read=FPersistentSaved, write=FPersistentSaved, stored=false, nodefault};
	__property TkbmCustomMemTable* AttachedTo = {read=FAttachedTo, write=SetAttachedTo};
	__property bool AttachedAutoRefresh = {read=FAttachedAutoRefresh, write=FAttachedAutoRefresh, nodefault};
	__property TkbmPerformance Performance = {read=GetPerformance, write=SetPerformance, default=0};
	__property Filtered = {default=0};
	__property Filter = {default=0};
	__property TkbmIndex* CurIndex = {read=FCurIndex};
	__property bool IgnoreAutoIncPopulation = {read=FIgnoreAutoIncPopulation, write=FIgnoreAutoIncPopulation, nodefault};
	__property int AttachMaxCount = {read=GetAttachMaxCount, write=SetAttachMaxCount, nodefault};
	__property int AttachCount = {read=GetAttachCount, nodefault};
	__property bool DesignActivation = {read=FDesignActivation, write=FDesignActivation, nodefault};
	__property int LanguageID = {read=GetLanguageID, write=SetLanguageID, nodefault};
	__property int SortID = {read=GetSortID, write=SetSortID, nodefault};
	__property int SubLanguageID = {read=GetSubLanguageID, write=SetSubLanguageID, nodefault};
	__property int LocaleID = {read=GetLocaleID, write=SetLocaleID, nodefault};
	__property TkbmCommon* Common = {read=FCommon};
	__property NativeInt AutoIncValue = {read=GetAutoIncValue, nodefault};
	__property NativeInt AutoIncMinValue = {read=GetAutoIncMin, write=SetAutoIncMinValue, default=0};
	__property bool AutoUpdateFieldVariables = {read=FAutoUpdateFieldVariables, write=SetAutoUpdateFieldVariables, nodefault};
	__property System::Variant AllData = {read=GetAllData, write=SetAllData};
	__property bool StoreDataOnForm = {read=FStoreDataOnForm, write=FStoreDataOnForm, default=0};
	__property System::UnicodeString CommaText = {read=GetCommaText, write=SetCommaText};
	__property NativeInt Capacity = {read=GetCapacity, write=SetCapacity, nodefault};
	__property NativeInt DeletedRecordsCount = {read=GetDeletedRecordsCount, nodefault};
	__property System::UnicodeString IndexFieldNames = {read=FIndexFieldNames, write=SetIndexFieldNames};
	__property System::UnicodeString IndexName = {read=FIndexName, write=SetIndexName};
	__property bool EnableIndexes = {read=FEnableIndexes, write=FEnableIndexes, default=1};
	__property bool AutoAddIndexes = {read=FAutoAddIndexes, write=FAutoAddIndexes, default=0};
	__property bool AutoReposition = {read=FAutoReposition, write=FAutoReposition, default=0};
	__property System::UnicodeString SortFields = {read=FSortFieldNames, write=FSortFieldNames};
	__property TkbmMemTableCompareOptions SortOptions = {read=FSortOptions, write=FSortOptions, nodefault};
	__property bool ReadOnly = {read=FReadOnly, write=FReadOnly, default=0};
	__property bool Standalone = {read=GetStandalone, write=SetStandalone, default=0};
	__property bool IgnoreReadOnly = {read=FIgnoreReadOnly, write=FIgnoreReadOnly, default=0};
	__property bool RangeActive = {read=GetRangeActive, nodefault};
	__property bool RangeIgnoreNullKeyValues = {read=FRangeIgnoreNullKeyValues, write=FRangeIgnoreNullKeyValues, default=1};
	__property System::Sysutils::TFileName PersistentFile = {read=FPersistentFile, write=FPersistentFile};
	__property bool Persistent = {read=FPersistent, write=FPersistent, default=0};
	__property bool PersistentBackup = {read=FPersistentBackup, write=FPersistentBackup, nodefault};
	__property System::UnicodeString PersistentBackupExt = {read=FPersistentBackupExt, write=FPersistentBackupExt};
	__property TkbmProgressCodes ProgressFlags = {read=FProgressFlags, write=FProgressFlags, nodefault};
	__property NativeInt LoadLimit = {read=FLoadLimit, write=FLoadLimit, default=-1};
	__property NativeInt LoadCount = {read=FLoadCount, write=FLoadCount, nodefault};
	__property bool LoadedCompletely = {read=FLoadedCompletely, write=FLoadedCompletely, nodefault};
	__property NativeInt SaveLimit = {read=FSaveLimit, write=FSaveLimit, default=-1};
	__property NativeInt SaveCount = {read=FSaveCount, write=FSaveCount, nodefault};
	__property bool SavedCompletely = {read=FSavedCompletely, write=FSavedCompletely, nodefault};
	__property bool RecalcOnFetch = {read=FRecalcOnFetch, write=FRecalcOnFetch, default=1};
	__property bool IsFieldModified[int i] = {read=GetModifiedFlags};
	__property bool EnableVersioning = {read=GetEnableVersioning, write=SetEnableVersioning, default=0};
	__property TkbmVersioningMode VersioningMode = {read=GetVersioningMode, write=SetVersioningMode, default=0};
	__property bool IsVersioning = {read=GetIsVersioning, nodefault};
	__property Data::Db::TUpdateStatusSet StatusFilter = {read=FStatusFilter, write=SetStatusFilter, nodefault};
	__property TkbmCustomDeltaHandler* DeltaHandler = {read=FDeltaHandler, write=SetDeltaHandler};
	__property TkbmIndexes* Indexes = {read=GetIndexes};
	__property TkbmIndex* IndexByName[System::UnicodeString IndexName] = {read=GetIndexByName};
	__property Data::Db::TIndexDefs* IndexDefs = {read=FIndexDefs, write=SetIndexDefs};
	__property Data::Db::TField* IndexFields[int Index] = {read=GetIndexField, write=SetIndexField};
	__property bool RecalcOnIndex = {read=FRecalcOnIndex, write=FRecalcOnIndex, default=0};
	__property Data::Db::TFilterOptions FilterOptions = {read=FFilterOptions, write=FFilterOptions, nodefault};
	__property System::UnicodeString DetailFields = {read=FDetailFieldNames, write=SetDetailFields};
	__property System::UnicodeString MasterFields = {read=GetMasterFields, write=SetMasterFields};
	__property Data::Db::TDataSource* MasterSource = {read=GetDataSource, write=SetDataSource};
	__property NativeInt RecordTag = {read=GetRecordTag, write=SetRecordTag, nodefault};
	__property System::UnicodeString Version = {read=GetVersion, write=FDummyStr};
	__property bool IsDataModified = {read=GetIsDataModified, write=SetIsDataModified, nodefault};
	__property int TransactionLevel = {read=GetTransactionLevel, nodefault};
	__property TkbmState TableState = {read=FState, write=FState, nodefault};
	__property TkbmCustomStreamFormat* DefaultFormat = {read=FDefaultFormat, write=FDefaultFormat};
	__property TkbmCustomStreamFormat* CommaTextFormat = {read=FCommaTextFormat, write=FCommaTextFormat};
	__property TkbmCustomStreamFormat* PersistentFormat = {read=FPersistentFormat, write=FPersistentFormat};
	__property TkbmCustomStreamFormat* FormFormat = {read=FFormFormat, write=FFormFormat};
	__property TkbmCustomStreamFormat* AllDataFormat = {read=FAllDataFormat, write=FAllDataFormat};
	__property TkbmOnLoadRecord OnLoadRecord = {read=FOnLoadRecord, write=FOnLoadRecord};
	__property TkbmOnLoadField OnLoadField = {read=FOnLoadField, write=FOnLoadField};
	__property TkbmOnSaveRecord OnSaveRecord = {read=FOnSaveRecord, write=FOnSaveRecord};
	__property TkbmOnSaveField OnSaveField = {read=FOnSaveField, write=FOnSaveField};
	__property TkbmOnCompress OnCompressBlobStream = {read=FOnCompressBlobStream, write=FOnCompressBlobStream};
	__property TkbmOnDeCompress OnDecompressBlobStream = {read=FOnDecompressBlobStream, write=FOnDecompressBlobStream};
	__property TkbmOnSetupField OnSetupField = {read=FOnSetupField, write=FOnSetupField};
	__property TkbmOnSetupFieldProperties OnSetupFieldProperties = {read=FOnSetupFieldProperties, write=FOnSetupFieldProperties};
	__property TkbmOnCompressField OnCompressField = {read=FOnCompressField, write=FOnCompressField};
	__property TkbmOnDecompressField OnDecompressField = {read=FOnDecompressField, write=FOnDecompressField};
	__property TkbmOnSave OnSave = {read=FOnSave, write=FOnSave};
	__property TkbmOnLoad OnLoad = {read=FOnLoad, write=FOnLoad};
	__property TkbmOnProgress OnProgress = {read=FOnProgress, write=FOnProgress};
	__property TkbmOnCompareFields OnCompareFields = {read=FOnCompareFields, write=FOnCompareFields};
	__property TkbmOnFilterIndex OnFilterIndex = {read=FOnFilterIndex, write=FOnFilterIndex};
	__property TkbmOnUserAggregate OnUserAggregate = {read=FOnUserAggregate, write=FOnUserAggregate};
	__property BeforeOpen;
	__property AfterOpen;
	__property BeforeClose;
	__property AfterClose;
	__property Data::Db::TDataSetNotifyEvent BeforeInsert = {read=FBeforeInsert, write=FBeforeInsert};
	__property AfterInsert;
	__property BeforeEdit;
	__property AfterEdit;
	__property BeforePost;
	__property AfterPost;
	__property BeforeCancel;
	__property AfterCancel;
	__property BeforeDelete;
	__property AfterDelete;
	__property BeforeScroll;
	__property AfterScroll;
	__property OnCalcFields;
	__property OnDeleteError;
	__property OnEditError;
	__property OnFilterRecord;
	__property OnNewRecord;
	__property OnPostError;
	__property Active = {default=0};
	/* Hoisted overloads: */
	
protected:
	inline void __fastcall  InternalAddRecord(NativeInt Buffer, bool Append){ Data::Db::TDataSet::InternalAddRecord(Buffer, Append); }
	inline void __fastcall  InternalAddRecord _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (void * Buffer, bool Append){ Data::Db::TDataSet::InternalAddRecord(Buffer, Append); }
	inline void __fastcall  InternalInitRecord(NativeInt Buffer){ Data::Db::TDataSet::InternalInitRecord(Buffer); }
	inline void __fastcall  InternalSetToRecord(NativeInt Buffer){ Data::Db::TDataSet::InternalSetToRecord(Buffer); }
	inline void __fastcall  SetFieldData(Data::Db::TField* Field, System::DynamicArray<System::Byte> Buffer, bool NativeFormat){ Data::Db::TDataSet::SetFieldData(Field, Buffer, NativeFormat); }
	inline void __fastcall  SetFieldData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (Data::Db::TField* Field, void * Buffer){ Data::Db::TDataSet::SetFieldData(Field, Buffer); }
	inline void __fastcall  SetFieldData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (Data::Db::TField* Field, void * Buffer, bool NativeFormat){ Data::Db::TDataSet::SetFieldData(Field, Buffer, NativeFormat); }
	inline Data::Db::TGetResult __fastcall  GetRecord(System::PByte Buffer, Data::Db::TGetMode GetMode, bool DoCheck){ return Data::Db::TDataSet::GetRecord(Buffer, GetMode, DoCheck); }
	inline Data::Db::TBookmarkFlag __fastcall  GetBookmarkFlag(NativeInt Buffer){ return Data::Db::TDataSet::GetBookmarkFlag(Buffer); }
	inline void __fastcall  SetBookmarkFlag(NativeInt Buffer, Data::Db::TBookmarkFlag Value){ Data::Db::TDataSet::SetBookmarkFlag(Buffer, Value); }
	inline void __fastcall  GetBookmarkData(NativeInt Buffer, System::DynamicArray<System::Byte> Data){ Data::Db::TDataSet::GetBookmarkData(Buffer, Data); }
	inline void __fastcall  GetBookmarkData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (System::PByte Buffer, void * Data){ Data::Db::TDataSet::GetBookmarkData(Buffer, Data); }
	inline void __fastcall  SetBookmarkData(NativeInt Buffer, System::DynamicArray<System::Byte> Data){ Data::Db::TDataSet::SetBookmarkData(Buffer, Data); }
	inline void __fastcall  SetBookmarkData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (System::PByte Buffer, void * Data){ Data::Db::TDataSet::SetBookmarkData(Buffer, Data); }
	inline void __fastcall  InternalGotoBookmark(System::DynamicArray<System::Byte> Bookmark){ Data::Db::TDataSet::InternalGotoBookmark(Bookmark); }
	inline void __fastcall  ClearCalcFields _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (System::PByte Buffer){ Data::Db::TDataSet::ClearCalcFields(Buffer); }
	
public:
	inline bool __fastcall  GetFieldData(int FieldNo, System::DynamicArray<System::Byte> &Buffer){ return Data::Db::TDataSet::GetFieldData(FieldNo, Buffer); }
	inline bool __fastcall  GetFieldData(Data::Db::TField* Field, System::DynamicArray<System::Byte> &Buffer, bool NativeFormat){ return Data::Db::TDataSet::GetFieldData(Field, Buffer, NativeFormat); }
	inline bool __fastcall  GetFieldData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (Data::Db::TField* Field, void * Buffer){ return Data::Db::TDataSet::GetFieldData(Field, Buffer); }
	inline bool __fastcall  GetFieldData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (int FieldNo, void * Buffer){ return Data::Db::TDataSet::GetFieldData(FieldNo, Buffer); }
	inline bool __fastcall  GetFieldData _DEPRECATED_ATTRIBUTE1("Use overloaded method instead") (Data::Db::TField* Field, void * Buffer, bool NativeFormat){ return Data::Db::TDataSet::GetFieldData(Field, Buffer, NativeFormat); }
	
};


class PASCALIMPLEMENTATION TkbmMemTable : public TkbmCustomMemTable
{
	typedef TkbmCustomMemTable inherited;
	
public:
	__property IgnoreReadOnly = {default=0};
	
__published:
	__property Active = {default=0};
	__property DesignActivation;
	__property AttachedTo;
	__property AttachedAutoRefresh;
	__property AttachMaxCount;
	__property AutoIncMinValue = {default=0};
	__property AutoCalcFields = {default=1};
	__property FieldDefs;
	__property Filtered = {default=0};
	__property DeltaHandler;
	__property EnableIndexes = {default=1};
	__property AutoAddIndexes = {default=0};
	__property AutoReposition = {default=0};
	__property IndexFieldNames = {default=0};
	__property IndexName = {default=0};
	__property IndexDefs;
	__property RecalcOnIndex = {default=0};
	__property RecalcOnFetch = {default=1};
	__property SortFields = {default=0};
	__property SortOptions;
	__property ReadOnly = {default=0};
	__property Performance = {default=0};
	__property Standalone = {default=0};
	__property PersistentFile = {default=0};
	__property StoreDataOnForm = {default=0};
	__property Persistent = {default=0};
	__property PersistentBackup;
	__property PersistentBackupExt = {default=0};
	__property ProgressFlags;
	__property LoadLimit = {default=-1};
	__property LoadedCompletely;
	__property SaveLimit = {default=-1};
	__property SavedCompletely;
	__property EnableVersioning = {default=0};
	__property VersioningMode = {default=0};
	__property Filter = {default=0};
	__property FilterOptions;
	__property MasterFields = {default=0};
	__property DetailFields = {default=0};
	__property MasterSource;
	__property Version = {default=0};
	__property LanguageID;
	__property SortID;
	__property SubLanguageID;
	__property LocaleID;
	__property DefaultFormat;
	__property CommaTextFormat;
	__property PersistentFormat;
	__property AllDataFormat;
	__property FormFormat;
	__property RangeIgnoreNullKeyValues = {default=1};
	
public:
	__property System::Sysutils::TFormatSettings FormatSettings = {read=FFormatSettings, write=FFormatSettings};
	
__published:
	__property OnProgress;
	__property OnLoadRecord;
	__property OnLoadField;
	__property OnSaveRecord;
	__property OnSaveField;
	__property OnCompressBlobStream;
	__property OnDecompressBlobStream;
	__property OnSetupField;
	__property OnSetupFieldProperties;
	__property OnCompressField;
	__property OnDecompressField;
	__property OnSave;
	__property OnLoad;
	__property OnCompareFields;
	__property OnFilterIndex;
	__property OnUserAggregate;
	__property BeforeOpen;
	__property AfterOpen;
	__property BeforeClose;
	__property AfterClose;
	__property BeforeInsert;
	__property AfterInsert;
	__property BeforeEdit;
	__property AfterEdit;
	__property BeforePost;
	__property AfterPost;
	__property BeforeCancel;
	__property AfterCancel;
	__property BeforeDelete;
	__property AfterDelete;
	__property BeforeScroll;
	__property AfterScroll;
	__property BeforeRefresh;
	__property AfterRefresh;
	__property OnCalcFields;
	__property OnDeleteError;
	__property OnEditError;
	__property OnFilterRecord;
	__property OnNewRecord;
	__property OnPostError;
public:
	/* TkbmCustomMemTable.Create */ inline __fastcall virtual TkbmMemTable(System::Classes::TComponent* AOwner) : TkbmCustomMemTable(AOwner) { }
	/* TkbmCustomMemTable.Destroy */ inline __fastcall virtual ~TkbmMemTable(void) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmBlobStream : public System::Classes::TMemoryStream
{
	typedef System::Classes::TMemoryStream inherited;
	
private:
	Kbmmemtypes::TkbmRecord *FWorkBuffer;
	Kbmmemtypes::TkbmRecord *FTableRecord;
	Data::Db::TBlobField* FField;
	TkbmCustomMemTable* FDataSet;
	Data::Db::TBlobStreamMode FMode;
	int FFieldNo;
	bool FModified;
	System::Byte *FpWorkBufferField;
	System::Byte *FpTableRecordField;
	System::PByte *FpWorkBufferBlob;
	System::PByte *FpTableRecordBlob;
	void __fastcall ReadBlobData(void);
	void __fastcall WriteBlobData(void);
	
public:
	__fastcall TkbmBlobStream(Data::Db::TBlobField* Field, Data::Db::TBlobStreamMode Mode);
	__fastcall virtual ~TkbmBlobStream(void);
	virtual int __fastcall Write(const void *Buffer, int Count)/* overload */;
	void __fastcall Truncate(void);
	/* Hoisted overloads: */
	
public:
	inline int __fastcall  Write(const System::DynamicArray<System::Byte> Buffer, int Offset, int Count){ return System::Classes::TMemoryStream::Write(Buffer, Offset, Count); }
	inline int __fastcall  Write(const System::DynamicArray<System::Byte> Buffer, int Count){ return System::Classes::TStream::Write(Buffer, Count); }
	
};

#pragma pack(pop)

typedef void __fastcall (__closure *TkbmDeltaHandlerGetValue)(TkbmCustomDeltaHandler* ADeltaHandler, Data::Db::TField* AField, System::Variant &AValue);

class PASCALIMPLEMENTATION TkbmCustomDeltaHandler : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TkbmDeltaHandlerGetValue FOnGetValue;
	TkbmCustomMemTable* FDataSet;
	void __fastcall CheckDataSet(void);
	System::Variant __fastcall GetValues(int Index);
	System::Variant __fastcall GetOrigValues(int Index);
	int __fastcall GetFieldCount(void);
	System::UnicodeString __fastcall GetFieldNames(int Index);
	Data::Db::TField* __fastcall GetFields(int Index);
	System::Variant __fastcall GetOrigValuesByName(System::UnicodeString Name);
	System::Variant __fastcall GetValuesByName(System::UnicodeString Name);
	NativeInt __fastcall GetRecordNo(void);
	NativeInt __fastcall GetUniqueRecordID(void);
	
protected:
	Kbmmemtypes::TkbmRecord *FPRecord;
	Kbmmemtypes::TkbmRecord *FPOrigRecord;
	virtual void __fastcall BeforeRecord(void);
	virtual void __fastcall InsertRecord(bool &Retry, Data::Db::TUpdateStatus &State);
	virtual void __fastcall DeleteRecord(bool &Retry, Data::Db::TUpdateStatus &State);
	virtual void __fastcall ModifyRecord(bool &Retry, Data::Db::TUpdateStatus &State);
	virtual void __fastcall UnmodifiedRecord(bool &Retry, Data::Db::TUpdateStatus &State);
	virtual void __fastcall AfterRecord(void);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	
public:
	virtual void __fastcall Resolve(void);
	__property TkbmCustomMemTable* DataSet = {read=FDataSet, write=FDataSet};
	__property int FieldCount = {read=GetFieldCount, nodefault};
	__property System::Variant OrigValues[int i] = {read=GetOrigValues};
	__property System::Variant Values[int i] = {read=GetValues};
	__property System::Variant OrigValuesByName[System::UnicodeString Name] = {read=GetOrigValuesByName};
	__property System::Variant ValuesByName[System::UnicodeString Name] = {read=GetValuesByName};
	__property System::UnicodeString FieldNames[int i] = {read=GetFieldNames};
	__property Data::Db::TField* Fields[int i] = {read=GetFields};
	__property NativeInt RecNo = {read=GetRecordNo, nodefault};
	__property NativeInt UniqueRecID = {read=GetUniqueRecordID, nodefault};
	
__published:
	__property TkbmDeltaHandlerGetValue OnGetValue = {read=FOnGetValue, write=FOnGetValue};
public:
	/* TComponent.Create */ inline __fastcall virtual TkbmCustomDeltaHandler(System::Classes::TComponent* AOwner) : System::Classes::TComponent(AOwner) { }
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmCustomDeltaHandler(void) { }
	
};


typedef System::StaticArray<System::UnicodeString, 5> Kbmmemtable__12;

//-- var, const, procedure ---------------------------------------------------
#define KBMMEMTABLE_EDITION L"Professional"
#define KBMMEMTABLE_VERSION L"7.70.00"
static const System::Int8 kbmkbMin = System::Int8(0x0);
static const System::Int8 kbmkbKey = System::Int8(0x0);
static const System::Int8 kbmkbRangeStart = System::Int8(0x1);
static const System::Int8 kbmkbRangeEnd = System::Int8(0x2);
static const System::Int8 kbmkbMasterDetail = System::Int8(0x3);
static const System::Int8 kbmkbMax = System::Int8(0x3);
static const System::Int8 kbmffIndirect = System::Int8(0x1);
static const System::Int8 kbmffCompress = System::Int8(0x2);
static const System::Int8 kbmffModified = System::Int8(0x4);
static const int kbmRecordIdent = int(0x6a1b2c3e);
static const System::Int8 kbmBookmarkCurrent = System::Int8(0x0);
static const System::Int8 kbmBookmarkFirst = System::Int8(0x1);
static const System::Int8 kbmBookmarkLast = System::Int8(0x2);
static const unsigned kbmGetRowsRest = unsigned(0xffffffff);
static const System::Int8 kbmffNull = System::Int8(0x0);
static const System::Int8 kbmffUnknown = System::Int8(0x1);
static const System::Int8 kbmffData = System::Int8(0x2);
#define kbmRowOrderIndex L"__MT__ROWORDER_"
#define kbmDetailIndex L"__MT__DETAIL_"
#define kbmGroupIndex L"__MT__GROUP_"
#define kbmDefSortIndex L"__MT__DEFSORT_"
#define kbmDefaultIndex L"__MT__DEFAULT_"
#define kbmRangeIndex L"__MT__RANGE_"
#define kbmAutoIndex L"__MT__AUTO_"
static const System::Int8 kbmrfInTable = System::Int8(0x1);
static const System::Int8 kbmrfDontCheckPoint = System::Int8(0x2);
extern DELPHI_PACKAGE TkbmFieldTypes kbmSupportedFieldTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmStringTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmBinaryTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmBlobTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmNonBlobTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmIndexableTypes;
extern DELPHI_PACKAGE TkbmFieldTypes kbmVarLengthNonBlobTypes;
#define NullVarLength (System::PByte)(0)
extern DELPHI_PACKAGE Kbmmemtable__12 FieldKindNames;
extern DELPHI_PACKAGE System::Variant __fastcall StreamToVariant(System::Classes::TStream* stream);
extern DELPHI_PACKAGE void __fastcall VariantToStream(const System::Variant &AVariant, System::Classes::TStream* stream);
extern DELPHI_PACKAGE void __fastcall CopyFieldDefs(Data::Db::TFieldDefs* Source, Data::Db::TFieldDefs* Dest);
extern DELPHI_PACKAGE int __fastcall CompareFields(const void * KeyField, const void * AField, const Data::Db::TFieldType FieldType, const int LocaleID, const TkbmifoOptions IndexFieldOptions, bool &FullCompare);
extern DELPHI_PACKAGE TkbmMemTableCompareOptions __fastcall IndexOptions2CompareOptions(Data::Db::TIndexOptions AOptions);
extern DELPHI_PACKAGE Data::Db::TIndexOptions __fastcall CompareOptions2IndexOptions(TkbmMemTableCompareOptions AOptions);
}	/* namespace Kbmmemtable */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMTABLE)
using namespace Kbmmemtable;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmemtableHPP
