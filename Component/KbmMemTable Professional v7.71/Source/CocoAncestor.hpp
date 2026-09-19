// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'CocoAncestor.pas' rev: 30.00 (Windows)

#ifndef CocoancestorHPP
#define CocoancestorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <kbmClasses.hpp>
#include <System.Generics.Defaults.hpp>
#include <System.Generics.Collections.hpp>
#include <System.SysUtils.hpp>

//-- user supplied -----------------------------------------------------------

namespace Cocoancestor
{
//-- forward type declarations -----------------------------------------------
struct TSymbolRec;
struct TStartTableEntry;
class DELPHICLASS TStartTable;
class DELPHICLASS TBaseScanner;
class DELPHICLASS TCocoRGrammar;
class DELPHICLASS TLiteralEqualityComparer;
class DELPHICLASS TCocoRScanner;
class DELPHICLASS TResorceSystem;
//-- type declarations -------------------------------------------------------
typedef System::WideChar CocoChar;

typedef System::DynamicArray<System::Classes::TBits*> TSetArray;

typedef TSymbolRec *PSymbol;

struct DECLSPEC_DRECORD TSymbolRec
{
public:
	int Beg;
	int Len;
	int Line;
	int Col;
	int Id;
};


typedef System::WideChar __fastcall (__closure *TGetCH)(int &pos);

typedef void __fastcall (__closure *TErrorEvent)(System::TObject* Sender, int ErrorType, int ErrorCode, int line, int col, const System::UnicodeString Msg, const System::UnicodeString data);

#pragma pack(push,1)
struct DECLSPEC_DRECORD TStartTableEntry
{
public:
	System::Word Ch;
	System::Word State;
};
#pragma pack(pop)


typedef System::DynamicArray<TStartTableEntry> _TStartTable__1;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TStartTable : public System::TObject
{
	typedef System::TObject inherited;
	
	
private:
	typedef System::StaticArray<_TStartTable__1, 128> _TStartTable__2;
	
	
public:
	int operator[](const int aCh) { return States[aCh]; }
	
private:
	_TStartTable__2 fEnterPoints;
	int __fastcall getState(const int aCh);
	void __fastcall setState(const int aCh, const int Value);
	
public:
	__fastcall TStartTable(void);
	void __fastcall FillRange(int start_, int end_, int value);
	__property int States[const int aCh] = {read=getState, write=setState/*, default*/};
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TStartTable(void) { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TBaseScanner : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	int fSourceBegin;
	int fSourceLen;
	int fStartLine;
	int fStartCol;
	
protected:
	System::UnicodeString fSource;
	bool fUTF8;
	bool fCaseInsensitive;
	virtual void __fastcall setCaseInsensitive(const bool Value);
	virtual void __fastcall setUTF8(const bool Value);
	System::UnicodeString __fastcall GetText(int pos, int len);
	
public:
	int noSym;
	virtual void __fastcall Reset(void) = 0 ;
	virtual void __fastcall Get(TSymbolRec &sym) = 0 ;
	virtual void __fastcall SetSource(const System::UnicodeString Src, PSymbol pos = (PSymbol)(0x0));
	virtual void __fastcall SetSourceFile(const System::UnicodeString FileName);
	void __fastcall GetSourcePosition(TSymbolRec &pos);
	virtual void __fastcall GetPosition(TSymbolRec &pos) = 0 ;
	virtual void __fastcall GotoPosition(TSymbolRec &pos) = 0 ;
	System::WideChar * __fastcall getSymbolPtr(PSymbol pos);
	System::UnicodeString __fastcall getSymbolText(PSymbol pos);
	bool __fastcall GetLine(int &pos, System::UnicodeString &line);
	__property bool CaseInsensitive = {read=fCaseInsensitive, write=setCaseInsensitive, nodefault};
	__property bool UTF8 = {read=fUTF8, write=setUTF8, nodefault};
	__property System::UnicodeString Source = {read=fSource};
	__property int SourceBegin = {read=fSourceBegin, nodefault};
	__property int SourceLen = {read=fSourceLen, nodefault};
	__property int StartLine = {read=fStartLine, nodefault};
	__property int StartCol = {read=fStartCol, nodefault};
public:
	/* TComponent.Create */ inline __fastcall virtual TBaseScanner(System::Classes::TComponent* AOwner) : System::Classes::TComponent(AOwner) { }
	/* TComponent.Destroy */ inline __fastcall virtual ~TBaseScanner(void) { }
	
};


class PASCALIMPLEMENTATION TCocoRGrammar : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
	
private:
	typedef System::DynamicArray<TSymbolRec> _TCocoRGrammar__1;
	
	
private:
	TBaseScanner* fScanner;
	int fLookAheadCount;
	int fLookBackCount;
	int fLookAroundGap;
	int fCurSymbolIndex;
	int fNextSymbolIndex;
	int fRealAheadCount;
	_TCocoRGrammar__1 fSymbols;
	int fErrDist;
	TErrorEvent fOnError;
	int fErrorCount;
	int fWarnCount;
	TSetArray fSymSets;
	int __fastcall SymbolOffset(int Idx);
	System::UnicodeString __fastcall getLexName(const int Index);
	System::UnicodeString __fastcall getLexString(const int Index);
	bool __fastcall GetSuccessful(void);
	PSymbol __fastcall getSymbol(const int Ind);
	PSymbol __fastcall getCurSymbol(void);
	int __fastcall getCurLine(void);
	PSymbol __fastcall getNextSymbol(void);
	void __fastcall setCurSymbolIndex(const int Value);
	int __fastcall getCurrentInputSymbol(void);
	
protected:
	__property int CurSymbolIndex = {read=fCurSymbolIndex, write=setCurSymbolIndex, nodefault};
	
public:
	__classmethod void __fastcall ClearSymSet(TSetArray arr);
	__classmethod void __fastcall InitSymSets(TSetArray &aSymSets, int *vals, const int vals_High);
	__classmethod System::UnicodeString __fastcall DequotedStr(const System::UnicodeString str);
	void __fastcall LookAroundGap(System::Word Back, System::Word Ahead);
	virtual TBaseScanner* __fastcall CreateScanner(void);
	virtual System::UnicodeString __fastcall TokenToString(int n);
	void __fastcall DoError(System::TObject* Sender, int ErrorType, int ErrorCode, int line, int col, const System::UnicodeString Data);
	virtual System::UnicodeString __fastcall ErrorMessage(int ErrorType, int ErrorCode, const System::UnicodeString data);
	virtual System::UnicodeString __fastcall ErrorPrefix(int ErrorType);
	void __fastcall Get(void);
	virtual void __fastcall ProcessPragmas(void);
	virtual void __fastcall CheckHomograph(int &sym);
	void __fastcall RestoreHomograph(int Id);
	void __fastcall Expect(int n);
	bool __fastcall InSet(int Symbol, int SetIndex);
	void __fastcall ExpectWeak(int n, int expectedSetIndex);
	bool __fastcall WeakSeparator(int n, int sySuccIdx, int iterSuccIdx);
	virtual void __fastcall SetSource(const System::UnicodeString Src);
	virtual void __fastcall SetSourceFileName(const System::UnicodeString Filename);
	System::UnicodeString __fastcall Bookmark(void);
	void __fastcall GotoBookmark(const System::UnicodeString aBookmark);
	void __fastcall SemError(const int errNo, const System::UnicodeString Data = System::UnicodeString());
	void __fastcall SemErrorInLine(const int errNo, const int line, const System::UnicodeString Data = System::UnicodeString());
	void __fastcall SynError(const int errNo);
	void __fastcall SynErrorExpect(const int aId);
	void __fastcall Warning(int line, const System::UnicodeString Msg);
	virtual void __fastcall Reinit(void);
	virtual bool __fastcall Execute(void);
	__fastcall virtual TCocoRGrammar(System::Classes::TComponent* AOwner);
	__property int ErrorCount = {read=fErrorCount, write=fErrorCount, nodefault};
	__property int WarnCount = {read=fWarnCount, write=fWarnCount, nodefault};
	__property TBaseScanner* Scanner = {read=fScanner, write=fScanner};
	__property int LookBackCount = {read=fLookBackCount, nodefault};
	__property int LookAheadCount = {read=fLookAheadCount, nodefault};
	__property int CurrentInputSymbol = {read=getCurrentInputSymbol, nodefault};
	__property int ErrDist = {read=fErrDist, nodefault};
	__property PSymbol CurSymbol = {read=getCurSymbol};
	__property int CurLine = {read=getCurLine, nodefault};
	__property PSymbol NextSymbol = {read=getNextSymbol};
	__property PSymbol Symbols[const int Ind] = {read=getSymbol};
	__property System::UnicodeString LexName = {read=getLexName, index=0};
	__property System::UnicodeString LexString = {read=getLexString, index=0};
	__property System::UnicodeString LexNames[const int Ind] = {read=getLexName};
	__property System::UnicodeString LexStrings[const int Ind] = {read=getLexString};
	__property bool Successful = {read=GetSuccessful, nodefault};
	__property TSetArray SymSets = {read=fSymSets, write=fSymSets};
	
__published:
	__property TErrorEvent OnError = {read=fOnError, write=fOnError};
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TCocoRGrammar(void) { }
	
};


typedef System::Generics::Collections::TDictionary__2<System::UnicodeString,int>* TLiteralList;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TLiteralEqualityComparer : public System::Generics::Defaults::TEqualityComparer__1<System::UnicodeString>
{
	typedef System::Generics::Defaults::TEqualityComparer__1<System::UnicodeString> inherited;
	
public:
	virtual bool __fastcall Equals(const System::UnicodeString Left, const System::UnicodeString Right)/* overload */;
	virtual int __fastcall GetHashCode(const System::UnicodeString Value)/* overload */;
public:
	/* TObject.Create */ inline __fastcall TLiteralEqualityComparer(void) : System::Generics::Defaults::TEqualityComparer__1<System::UnicodeString>() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TLiteralEqualityComparer(void) { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TCocoRScanner : public TBaseScanner
{
	typedef TBaseScanner inherited;
	
private:
	int fTabLength;
	TGetCH fGetCh;
	System::WideChar fCurrInputCh;
	int fSymbolStartPos;
	int fBufferPosition;
	int fNextPosition;
	int fCurrLine;
	int fCurrCol;
	System::Generics::Collections::TDictionary__2<System::UnicodeString,int>* fLiterals;
	TStartTable* fStartState;
	void __fastcall AdjustGetter(void);
	
protected:
	virtual void __fastcall setCaseInsensitive(const bool Value);
	virtual void __fastcall setUTF8(const bool Value);
	
public:
	__fastcall virtual TCocoRScanner(System::Classes::TComponent* AOwner);
	virtual void __fastcall Reset(void);
	virtual void __fastcall GetPosition(TSymbolRec &pos);
	virtual void __fastcall GotoPosition(TSymbolRec &pos);
	void __fastcall BeginContext(TSymbolRec &pos);
	void __fastcall EndContext(TSymbolRec &pos);
	System::WideChar __fastcall CharAt(int &pos);
	System::WideChar __fastcall CharUTF8At(int &pos);
	System::WideChar __fastcall CapChAt(int &pos);
	System::WideChar __fastcall CapChUTF8At(int &pos);
	void __fastcall NextCh(void);
	virtual void __fastcall Get(TSymbolRec &sym);
	virtual void __fastcall ScanSym(int state, int &sym);
	virtual bool __fastcall SkipComments(int ind);
	void __fastcall SkipTo(const System::UnicodeString Str);
	virtual void __fastcall SkipIgnoreSet(void);
	void __fastcall SkipCommentTo(const System::UnicodeString str);
	void __fastcall SkipNestedComment(const System::UnicodeString str1, const System::UnicodeString str2);
	void __fastcall CheckLiteral(int &Sym);
	__property TGetCH GetCh = {read=fGetCh};
	__property int BufferPosition = {read=fBufferPosition, write=fBufferPosition, nodefault};
	__property System::WideChar CurrInputCh = {read=fCurrInputCh, nodefault};
	__property int CurrLine = {read=fCurrLine, nodefault};
	__property int CurrCol = {read=fCurrCol, nodefault};
	__property int TabLength = {read=fTabLength, write=fTabLength, nodefault};
	__property TStartTable* StartState = {read=fStartState, write=fStartState};
	__property System::Generics::Collections::TDictionary__2<System::UnicodeString,int>* Literals = {read=fLiterals, write=fLiterals};
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TCocoRScanner(void) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TResorceSystem : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	virtual System::UnicodeString __fastcall AbsoluteURL(const System::UnicodeString BaseURL, const System::UnicodeString RelativeURL) = 0 ;
	virtual bool __fastcall ResourceExists(const System::UnicodeString URL, const System::UnicodeString mime = System::UnicodeString()) = 0 ;
	virtual bool __fastcall ResourceAge(const System::UnicodeString URL, /* out */ System::TDateTime &ResDateTime, const System::UnicodeString mime = System::UnicodeString()) = 0 ;
	virtual System::Classes::TStream* __fastcall ResourceForReading(const System::UnicodeString URL, const System::UnicodeString mime = System::UnicodeString()) = 0 ;
	virtual System::UnicodeString __fastcall GetResString(const System::UnicodeString URL, const System::UnicodeString mime = System::UnicodeString()) = 0 ;
	virtual System::Classes::TStream* __fastcall ResourceForWriting(const System::UnicodeString URL, const System::UnicodeString mime = System::UnicodeString()) = 0 ;
public:
	/* TObject.Create */ inline __fastcall TResorceSystem(void) : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TResorceSystem(void) { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
static const System::Int8 minErrDist = System::Int8(0x2);
static const System::Int8 LookBackDefaultCount = System::Int8(0x1);
static const System::Int8 LookAheadDefaultCount = System::Int8(0x3);
static const System::Int8 etSyntax = System::Int8(0x0);
static const System::Int8 etSymantic = System::Int8(0x1);
static const System::Int8 etWarning = System::Int8(0x2);
static const System::Int8 _EOFSYMB = System::Int8(0x0);
extern DELPHI_PACKAGE System::UnicodeString LineBreak;
extern DELPHI_PACKAGE int LineBreakLen;
extern DELPHI_PACKAGE TResorceSystem* ResorceSystem;
extern DELPHI_PACKAGE System::UnicodeString __fastcall LoadFileToString(const System::UnicodeString FileName);
extern DELPHI_PACKAGE System::Generics::Collections::TDictionary__2<System::UnicodeString,int>* __fastcall CreateLiterals(const bool ACaseSensitive, System::UnicodeString const *Names, const int Names_High, int const *Ids, const int Ids_High);
extern DELPHI_PACKAGE void __fastcall ClearSymbol(PSymbol pos);
}	/* namespace Cocoancestor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_COCOANCESTOR)
using namespace Cocoancestor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// CocoancestorHPP
