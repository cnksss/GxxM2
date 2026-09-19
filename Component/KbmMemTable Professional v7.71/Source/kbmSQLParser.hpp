// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmSQLParser.pas' rev: 30.00 (Windows)

#ifndef KbmsqlparserHPP
#define KbmsqlparserHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <CocoAncestor.hpp>
#include <kbmSQLElements.hpp>
#include <System.Variants.hpp>
#include <Data.DB.hpp>
#include <System.SysUtils.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmsqlparser
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmSQLParserScanner;
class DELPHICLASS TkbmSQLParser;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TkbmSQLParserScanner : public Cocoancestor::TCocoRScanner
{
	typedef Cocoancestor::TCocoRScanner inherited;
	
public:
	virtual void __fastcall SkipIgnoreSet(void);
	virtual void __fastcall ScanSym(int state, int &sym);
	virtual bool __fastcall SkipComments(int ind);
public:
	/* TCocoRScanner.Create */ inline __fastcall virtual TkbmSQLParserScanner(System::Classes::TComponent* AOwner) : Cocoancestor::TCocoRScanner(AOwner) { }
	
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TkbmSQLParserScanner(void) { }
	
};


class PASCALIMPLEMENTATION TkbmSQLParser : public Cocoancestor::TCocoRGrammar
{
	typedef Cocoancestor::TCocoRGrammar inherited;
	
private:
	int FSubSelectDepth;
	Kbmsqlelements::TkbmSQLCustomOperation* FOperation;
	Kbmsqlelements::TkbmSQLExpressionOptions FExpressionOptions;
	System::TObject* FContext;
	Kbmsqlelements::TkbmSQLOnGetVariableMetaData FOnGetVariableMetaData;
	Kbmsqlelements::TkbmSQLOnGetVariableValue FOnGetVariableValue;
	Kbmsqlelements::TkbmSQLOnGetFunction FOnGetFunction;
	System::UnicodeString __fastcall UnQuoteSQLString(const System::UnicodeString AString);
	System::UnicodeString __fastcall UnQuoteIdentifier(const System::UnicodeString AString);
	Kbmsqlelements::TkbmSQLCustomNode* __fastcall AppendBinaryNode(Kbmsqlelements::TkbmSQLCustomOperation* AOperation, const Kbmsqlelements::TkbmSQLBinaryNodeOperator AOperator, Kbmsqlelements::TkbmSQLCustomNode* ALeftNode, Kbmsqlelements::TkbmSQLCustomNode* ARightNode);
	__property int SubSelectDepth = {read=FSubSelectDepth, write=FSubSelectDepth, nodefault};
	
protected:
	void __fastcall _kbmSQLParser(void);
	void __fastcall _SelectSQL(Kbmsqlelements::TkbmSQLSelectOperation* &statement);
	void __fastcall _UpdateSQL(Kbmsqlelements::TkbmSQLUpdateOperation* &statement);
	void __fastcall _InsertSQL(Kbmsqlelements::TkbmSQLInsertOperation* &statement);
	void __fastcall _DeleteSQL(Kbmsqlelements::TkbmSQLDeleteOperation* &statement);
	void __fastcall _Evaluate(Kbmsqlelements::TkbmSQLEvaluationOperation* &statement);
	void __fastcall _SelectStmt(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _UpdateStmt(Kbmsqlelements::TkbmSQLUpdateOperation* statement);
	void __fastcall _InsertStmt(Kbmsqlelements::TkbmSQLInsertOperation* statement);
	void __fastcall _DeleteStmt(Kbmsqlelements::TkbmSQLDeleteOperation* statement);
	void __fastcall _EvaluateStmt(Kbmsqlelements::TkbmSQLEvaluationOperation* statement);
	void __fastcall _FilterStmt(Kbmsqlelements::TkbmSQLEvaluationOperation* statement);
	void __fastcall _UnqualifiedTable(Kbmsqlelements::TkbmSQLTable* &table);
	void __fastcall _UpdateFieldList(Kbmsqlelements::TkbmSQLUpdateOperation* statement);
	void __fastcall _WhereClause(Kbmsqlelements::TkbmSQLCustomNode* &condition);
	void __fastcall _UpdateField(Kbmsqlelements::TkbmSQLUpdateOperation* statement);
	void __fastcall _QualifiedField(Kbmsqlelements::TkbmSQLFieldNode* &node);
	void __fastcall _Expression(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _ItemSeparator(void);
	void __fastcall _OpenParens(void);
	void __fastcall _InsertFieldList(Kbmsqlelements::TkbmSQLFieldNodes* fields);
	void __fastcall _CloseParens(void);
	void __fastcall _ExpressionList(Kbmsqlelements::TkbmSQLNodes* nodes);
	void __fastcall _SelectionClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _FromClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _GroupByClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _HavingClause(Kbmsqlelements::TkbmSQLCustomNode* &condition);
	void __fastcall _OrderByClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _LimitClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _OffsetClause(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _SelectionFieldList(Kbmsqlelements::TkbmSQLNodes* fields);
	void __fastcall _FromTableList(Kbmsqlelements::TkbmSQLTables* tables);
	void __fastcall _QualifiedTable(Kbmsqlelements::TkbmSQLTable* &table);
	void __fastcall _Table(Kbmsqlelements::TkbmSQLTable* &table);
	void __fastcall _SearchCondition(Kbmsqlelements::TkbmSQLCustomNode* &condition);
	void __fastcall _OrderByFldList(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _GroupByFldList(Kbmsqlelements::TkbmSQLSelectOperation* statement);
	void __fastcall _MathTerm(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _SelectionExpr(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _FunctionExpr(Kbmsqlelements::TkbmSQLCustomNode* &exprnode);
	void __fastcall _ConstExpr(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _Null(void);
	void __fastcall _True(void);
	void __fastcall _False(void);
	void __fastcall _FieldExpr(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _Variable(Kbmsqlelements::TkbmSQLVariableNode* &node);
	void __fastcall _SimpleVariable(Kbmsqlelements::TkbmSQLVariableNode* &node);
	void __fastcall _VariableExpr(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _SimpleVariableExpr(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _OrderByField(Kbmsqlelements::TkbmSQLFieldNode* &node);
	void __fastcall _GroupByField(Kbmsqlelements::TkbmSQLFieldNode* &node);
	void __fastcall _AndOrTerm(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _BinaryExpression(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _Factor(Kbmsqlelements::TkbmSQLCustomNode* &node);
	void __fastcall _Term(Kbmsqlelements::TkbmSQLCustomNode* &node);
	
public:
	System::Sysutils::TFormatSettings FormatSettings;
	void __fastcall Init(void);
	virtual void __fastcall ProcessPragmas(void);
	virtual System::UnicodeString __fastcall ErrorMessage(int ErrorType, int ErrorCode, const System::UnicodeString data);
	virtual System::UnicodeString __fastcall TokenToString(int n);
	virtual Cocoancestor::TBaseScanner* __fastcall CreateScanner(void);
	virtual bool __fastcall Execute(void);
	__fastcall virtual TkbmSQLParser(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TkbmSQLParser(void);
	__property Kbmsqlelements::TkbmSQLCustomOperation* Operation = {read=FOperation, write=FOperation};
	__property Kbmsqlelements::TkbmSQLExpressionOptions ExpressionOptions = {read=FExpressionOptions, write=FExpressionOptions, nodefault};
	__property System::TObject* Context = {read=FContext, write=FContext};
	__property Kbmsqlelements::TkbmSQLOnGetVariableMetaData OnGetVariableMetaData = {read=FOnGetVariableMetaData, write=FOnGetVariableMetaData};
	__property Kbmsqlelements::TkbmSQLOnGetVariableValue OnGetVariableValue = {read=FOnGetVariableValue, write=FOnGetVariableValue};
	__property Kbmsqlelements::TkbmSQLOnGetFunction OnGetFunction = {read=FOnGetFunction, write=FOnGetFunction};
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Kbmsqlparser */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMSQLPARSER)
using namespace Kbmsqlparser;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmsqlparserHPP
