// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemCSVStreamFormat.pas' rev: 30.00 (Windows)

#ifndef KbmmemcsvstreamformatHPP
#define KbmmemcsvstreamformatHPP

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
#include <Data.DBCommon.hpp>
#include <System.Character.hpp>
#include <kbmMemResEng.hpp>
#include <System.SysUtils.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmmemcsvstreamformat
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmCustomCSVStreamFormat;
class DELPHICLASS TkbmCSVStreamFormat;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TkbmStreamFlagLocalFormat : unsigned char { sfSaveLocalFormat, sfLoadLocalFormat, sfLoadAsASCII, sfLoadAsANSI };

enum DECLSPEC_DENUM TkbmStreamFlagNoHeader : unsigned char { sfSaveNoHeader, sfLoadNoHeader };

enum DECLSPEC_DENUM TkbmStreamFlagQuoteOnlyStrings : unsigned char { sfSaveQuoteOnlyStrings };

enum DECLSPEC_DENUM TkbmStreamFlagPlaceholders : unsigned char { sfSavePlaceholders };

enum DECLSPEC_DENUM TkbmStreamFlagAutoInc : unsigned char { sfLoadGenerateAutoInc };

typedef System::Set<TkbmStreamFlagLocalFormat, TkbmStreamFlagLocalFormat::sfSaveLocalFormat, TkbmStreamFlagLocalFormat::sfLoadAsANSI> TkbmStreamFlagsLocalFormat;

typedef System::Set<TkbmStreamFlagNoHeader, TkbmStreamFlagNoHeader::sfSaveNoHeader, TkbmStreamFlagNoHeader::sfLoadNoHeader> TkbmStreamFlagsNoHeader;

typedef System::Set<TkbmStreamFlagPlaceholders, TkbmStreamFlagPlaceholders::sfSavePlaceholders, TkbmStreamFlagPlaceholders::sfSavePlaceholders> TkbmStreamFlagsPlaceHolders;

typedef System::Set<TkbmStreamFlagQuoteOnlyStrings, TkbmStreamFlagQuoteOnlyStrings::sfSaveQuoteOnlyStrings, TkbmStreamFlagQuoteOnlyStrings::sfSaveQuoteOnlyStrings> TkbmStreamFlagsQuoteOnlyStrings;

typedef System::Set<TkbmStreamFlagAutoInc, TkbmStreamFlagAutoInc::sfLoadGenerateAutoInc, TkbmStreamFlagAutoInc::sfLoadGenerateAutoInc> TkbmStreamFlagsAutoInc;

typedef void __fastcall (__closure *TkbmOnFormatLoadField)(System::TObject* Sender, Data::Db::TField* Field, bool &Null, System::UnicodeString &Data);

typedef void __fastcall (__closure *TkbmOnFormatSaveField)(System::TObject* Sender, Data::Db::TField* Field, bool &Null, System::UnicodeString &Data);

typedef void __fastcall (__closure *TkbmOnGetLine)(System::TObject* Sender, System::UnicodeString &ALine, bool &AEOF);

class PASCALIMPLEMENTATION TkbmCustomCSVStreamFormat : public Kbmmemtable::TkbmCustomStreamFormat
{
	typedef Kbmmemtable::TkbmCustomStreamFormat inherited;
	
private:
	Kbmmemtable::TkbmCustomMemTable* FDataset;
	TkbmOnFormatLoadField FOnFormatLoadField;
	TkbmOnFormatSaveField FOnFormatSaveField;
	TkbmOnGetLine FOnGetLine;
	System::DynamicArray<System::Byte> FBuf;
	int FBufidx;
	int FBufSize;
	System::DynamicArray<System::Byte> FRawLine;
	System::UnicodeString FLine;
	System::UnicodeString FWord;
	int Flptr;
	int Felptr;
	int FProgressCnt;
	int FStreamSize;
	int FStartPosition;
	System::WideChar FCommentChar;
	System::WideChar FEscapeChar;
	int FDefaultStringFieldSize;
	System::WideChar FCSVQuote;
	System::WideChar FCSVFieldDelimiter;
	System::WideChar FCSVRecordDelimiter;
	System::UnicodeString FCSVTrueString;
	System::UnicodeString FCSVFalseString;
	TkbmStreamFlagsLocalFormat FsfLocalFormat;
	TkbmStreamFlagsNoHeader FsfNoHeader;
	TkbmStreamFlagsPlaceHolders FsfPlaceHolders;
	TkbmStreamFlagsQuoteOnlyStrings FsfQuoteOnlyStrings;
	TkbmStreamFlagsAutoInc FsfAutoInc;
	System::Sysutils::TFormatSettings FLocalFormat;
	bool FUseFieldDisplayName;
	void __fastcall SetCSVFieldDelimiter(System::WideChar Value);
	
protected:
	bool FDefLoaded;
	virtual bool __fastcall GetChunk(void);
	virtual bool __fastcall GetLine(void);
	virtual System::UnicodeString __fastcall GetWord(bool &null);
	virtual void __fastcall WriteString(const System::UnicodeString AString);
	virtual System::UnicodeString __fastcall GetVersion(void);
	virtual void __fastcall BeforeSave(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveDef(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall SaveData(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterSave(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall DetermineLoadFieldIDs(Kbmmemtable::TkbmCustomMemTable* ADataset, System::Classes::TStringList* AList, Kbmmemtable::TkbmDetermineLoadFieldsSituation Situation);
	virtual void __fastcall DetermineLoadFieldIndex(Kbmmemtable::TkbmCustomMemTable* ADataset, System::UnicodeString ID, int FieldCount, int OrigIndex, int &NewIndex, Kbmmemtable::TkbmDetermineLoadFieldsSituation Situation);
	virtual void __fastcall BeforeLoad(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadDef(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall LoadData(Kbmmemtable::TkbmCustomMemTable* ADataset);
	virtual void __fastcall AfterLoad(Kbmmemtable::TkbmCustomMemTable* ADataset);
	System::UnicodeString __fastcall GetFieldHeaderText(Data::Db::TField* const AField);
	virtual void __fastcall LoadFieldIDs(Kbmmemtable::TkbmCustomMemTable* const ADataset, System::Classes::TStringList* const AList, const bool AUseFieldDisplayName);
	
public:
	__property TkbmOnFormatLoadField OnFormatLoadField = {read=FOnFormatLoadField, write=FOnFormatLoadField};
	__property TkbmOnFormatSaveField OnFormatSaveField = {read=FOnFormatSaveField, write=FOnFormatSaveField};
	__property TkbmOnGetLine OnGetLine = {read=FOnGetLine, write=FOnGetLine};
	__property System::WideChar CommentChar = {read=FCommentChar, write=FCommentChar, nodefault};
	__property System::WideChar EscapeChar = {read=FEscapeChar, write=FEscapeChar, nodefault};
	__property int DefaultStringFieldSize = {read=FDefaultStringFieldSize, write=FDefaultStringFieldSize, nodefault};
	__property System::WideChar CSVQuote = {read=FCSVQuote, write=FCSVQuote, nodefault};
	__property System::WideChar CSVFieldDelimiter = {read=FCSVFieldDelimiter, write=SetCSVFieldDelimiter, nodefault};
	__property System::WideChar CSVRecordDelimiter = {read=FCSVRecordDelimiter, write=FCSVRecordDelimiter, nodefault};
	__property System::UnicodeString CSVTrueString = {read=FCSVTrueString, write=FCSVTrueString};
	__property System::UnicodeString CSVFalseString = {read=FCSVFalseString, write=FCSVFalseString};
	__property TkbmStreamFlagsLocalFormat sfLocalFormat = {read=FsfLocalFormat, write=FsfLocalFormat, nodefault};
	__property TkbmStreamFlagsNoHeader sfNoHeader = {read=FsfNoHeader, write=FsfNoHeader, nodefault};
	__property TkbmStreamFlagsPlaceHolders sfPlaceHolders = {read=FsfPlaceHolders, write=FsfPlaceHolders, nodefault};
	__property TkbmStreamFlagsQuoteOnlyStrings sfQuoteOnlyStrings = {read=FsfQuoteOnlyStrings, write=FsfQuoteOnlyStrings, nodefault};
	__property TkbmStreamFlagsAutoInc sfAutoInc = {read=FsfAutoInc, write=FsfAutoInc, nodefault};
	__property bool UseFieldDisplayName = {read=FUseFieldDisplayName, write=FUseFieldDisplayName, nodefault};
	__fastcall virtual TkbmCustomCSVStreamFormat(System::Classes::TComponent* AOwner);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmCustomCSVStreamFormat(void) { }
	
};


class PASCALIMPLEMENTATION TkbmCSVStreamFormat : public TkbmCustomCSVStreamFormat
{
	typedef TkbmCustomCSVStreamFormat inherited;
	
__published:
	__property CommentChar;
	__property EscapeChar;
	__property DefaultStringFieldSize;
	__property CSVQuote;
	__property CSVFieldDelimiter;
	__property CSVRecordDelimiter;
	__property CSVTrueString = {default=0};
	__property CSVFalseString = {default=0};
	__property sfLocalFormat;
	__property sfQuoteOnlyStrings;
	__property sfNoHeader;
	__property Version = {default=0};
	__property sfData;
	__property sfCalculated;
	__property sfLookup;
	__property sfNonVisible;
	__property sfBlobs;
	__property sfDef;
	__property sfIndexDef;
	__property sfPlaceHolders;
	__property sfFiltered;
	__property sfIgnoreRange;
	__property sfIgnoreMasterDetail;
	__property sfDeltas;
	__property sfDontFilterDeltas;
	__property sfAppend;
	__property sfFieldKind;
	__property sfFromStart;
	__property sfDisplayWidth;
	__property sfAutoInc;
	__property OnFormatLoadField;
	__property OnFormatSaveField;
	__property OnGetLine;
	__property OnBeforeLoad;
	__property OnAfterLoad;
	__property OnBeforeSave;
	__property OnAfterSave;
	__property OnCompress;
	__property OnDeCompress;
	
public:
	__property FormatSettings;
	
__published:
	__property UseFieldDisplayName;
public:
	/* TkbmCustomCSVStreamFormat.Create */ inline __fastcall virtual TkbmCSVStreamFormat(System::Classes::TComponent* AOwner) : TkbmCustomCSVStreamFormat(AOwner) { }
	
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmCSVStreamFormat(void) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::UnicodeString __fastcall StringToCodedString(const System::UnicodeString Source, const System::WideChar EscapeChar);
extern DELPHI_PACKAGE System::UnicodeString __fastcall CodedStringToString(const System::UnicodeString Source, const System::WideChar EscapeChar);
extern DELPHI_PACKAGE System::DynamicArray<System::Byte> __fastcall BufferToBase64(const System::DynamicArray<System::Byte> Source);
extern DELPHI_PACKAGE System::DynamicArray<System::Byte> __fastcall Base64ToBuffer(const System::DynamicArray<System::Byte> Source);
}	/* namespace Kbmmemcsvstreamformat */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMCSVSTREAMFORMAT)
using namespace Kbmmemcsvstreamformat;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmemcsvstreamformatHPP
