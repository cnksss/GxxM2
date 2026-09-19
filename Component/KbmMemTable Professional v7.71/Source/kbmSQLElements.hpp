// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmSQLElements.pas' rev: 30.00 (Windows)

#ifndef KbmsqlelementsHPP
#define KbmsqlelementsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <System.SysUtils.hpp>
#include <Data.DB.hpp>
#include <kbmMemTable.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmsqlelements
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmSQLCustomDBAPI_;
class DELPHICLASS TkbmSQLCustomNode;
class DELPHICLASS TkbmSQLNodes;
class DELPHICLASS TkbmSQLBinaryNode;
class DELPHICLASS TkbmSQLUnaryNode;
class DELPHICLASS TkbmSQLBetweenNode;
class DELPHICLASS TkbmSQLInNode;
class DELPHICLASS TkbmSQLFieldNode;
class DELPHICLASS TkbmSQLFieldNodes;
class DELPHICLASS TkbmSQLGroupFieldNode;
class DELPHICLASS TkbmSQLVariableNode;
class DELPHICLASS TkbmSQLAggregateNode;
class DELPHICLASS TkbmSQLFunctionNode;
class DELPHICLASS TkbmSQLCustomValueNode;
class DELPHICLASS TkbmSQLValueNode;
class DELPHICLASS TkbmSQLConstNode;
class DELPHICLASS TkbmSQLTable;
class DELPHICLASS TkbmSQLTables;
class DELPHICLASS TkbmSQLCustomOperation;
class DELPHICLASS TkbmSQLCustomSelectOperation;
class DELPHICLASS TkbmSQLSelectOperation;
class DELPHICLASS TkbmSQLUpdateOperation;
class DELPHICLASS TkbmSQLDeleteOperation;
class DELPHICLASS TkbmSQLInsertOperation;
class DELPHICLASS TkbmSQLEvaluationOperation;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TkbmSQLBinaryNodeOperator : unsigned char { ebAdd, ebSub, ebMul, ebDiv, ebAnd, ebOr, ebXor, ebMod, ebIDiv, ebLike, ebEqual, ebNotEqual, ebLess, ebGreater, ebLessEqual, ebGreaterEqual, ebConcat };

enum DECLSPEC_DENUM TkbmSQLUnaryNodeOperator : unsigned char { euNot, euNegate };

enum DECLSPEC_DENUM TkbmSQLAggregateFunction : unsigned char { safUNKNOWN, safCOUNT, safMAX, safMIN, safAVG, safSUM, safSTDDEV };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLCustomDBAPI_ : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	/* TObject.Create */ inline __fastcall TkbmSQLCustomDBAPI_(void) : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLCustomDBAPI_(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLCustomNode : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	int FID;
	System::UnicodeString FUniqueName;
	System::UnicodeString FAlias;
	System::UnicodeString FDescription;
	Data::Db::TFieldType FDataType;
	bool FNonScalar;
	TkbmSQLNodes* FNodes;
	bool FExecuted;
	TkbmSQLCustomOperation* FOperation;
	Data::Db::TField* FDestinationField;
	Data::Db::TField* FSourceField;
	System::TObject* FParent;
	
protected:
	virtual int __fastcall GetExpectedWidth(void);
	virtual int __fastcall GetWidth(void);
	virtual bool __fastcall GetNonScalar(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLCustomNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	virtual System::Variant __fastcall Execute(void) = 0 ;
	__property System::UnicodeString UniqueName = {read=FUniqueName, write=FUniqueName};
	virtual System::UnicodeString __fastcall GetUniqueName(void);
	__property System::UnicodeString Alias = {read=FAlias, write=FAlias};
	__property System::UnicodeString Description = {read=FDescription, write=FDescription};
	__property int ID = {read=FID, write=FID, nodefault};
	__property Data::Db::TFieldType DataType = {read=GetDataType, write=FDataType, nodefault};
	__property int Width = {read=GetWidth, nodefault};
	__property bool NonScalar = {read=GetNonScalar, nodefault};
	__property TkbmSQLCustomOperation* Operation = {read=FOperation};
	__property TkbmSQLNodes* Nodes = {read=FNodes, write=FNodes};
	__property System::TObject* Parent = {read=FParent, write=FParent};
	__property Data::Db::TField* DestinationField = {read=FDestinationField, write=FDestinationField};
	__property Data::Db::TField* SourceField = {read=FSourceField, write=FSourceField};
	__property bool Executed = {read=FExecuted, nodefault};
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLCustomNode(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLNodes : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	TkbmSQLCustomNode* operator[](const int AIndex) { return Node[AIndex]; }
	
private:
	System::Classes::TList* FList;
	bool FOwnsNodes;
	TkbmSQLCustomNode* FParent;
	
protected:
	TkbmSQLCustomNode* __fastcall GetNode(const int AIndex);
	void __fastcall SetNode(const int AIndex, TkbmSQLCustomNode* const ANode);
	
public:
	__fastcall virtual TkbmSQLNodes(const bool AOwnsNodes);
	__fastcall virtual ~TkbmSQLNodes(void);
	TkbmSQLCustomNode* __fastcall GetByUniqueName(System::UnicodeString AUniqueName);
	TkbmSQLCustomNode* __fastcall GetByAlias(System::UnicodeString AAlias);
	TkbmSQLFieldNode* __fastcall GetByFieldName(const System::UnicodeString AName);
	int __fastcall IndexOf(TkbmSQLCustomNode* const ANode);
	void __fastcall Add(TkbmSQLCustomNode* const ANode, const bool AIgnoreIfExists = false)/* overload */;
	void __fastcall Add(TkbmSQLNodes* const ANodes, const bool AIgnoreIfExists = false)/* overload */;
	void __fastcall Insert(const int APos, TkbmSQLCustomNode* const ANode);
	void __fastcall Delete(const int APos)/* overload */;
	void __fastcall Delete(TkbmSQLCustomNode* const ANode)/* overload */;
	void __fastcall Replace(TkbmSQLCustomNode* const AOldNode, TkbmSQLCustomNode* const ANewNode, const bool AFreeOldNode = false);
	int __fastcall Count(void);
	void __fastcall ClearExecuted(void);
	void __fastcall Clear(void);
	__property TkbmSQLCustomNode* Node[const int AIndex] = {read=GetNode, write=SetNode/*, default*/};
	__property TkbmSQLCustomNode* Parent = {read=FParent, write=FParent};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLBinaryNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	TkbmSQLCustomNode* FLeftNode;
	TkbmSQLCustomNode* FRightNode;
	TkbmSQLBinaryNodeOperator FOperator;
	
protected:
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLBinaryNode(TkbmSQLCustomOperation* AOperation, TkbmSQLBinaryNodeOperator AOperator)/* overload */;
	__fastcall virtual ~TkbmSQLBinaryNode(void);
	virtual System::Variant __fastcall Execute(void);
	__property TkbmSQLCustomNode* LeftNode = {read=FLeftNode, write=FLeftNode};
	__property TkbmSQLCustomNode* RightNode = {read=FRightNode, write=FRightNode};
	__property TkbmSQLBinaryNodeOperator Operator = {read=FOperator, nodefault};
public:
	/* TkbmSQLCustomNode.Create */ inline __fastcall virtual TkbmSQLBinaryNode(TkbmSQLCustomOperation* AOperation)/* overload */ : TkbmSQLCustomNode(AOperation) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLUnaryNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	TkbmSQLCustomNode* FRightNode;
	TkbmSQLUnaryNodeOperator FOperator;
	
protected:
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLUnaryNode(TkbmSQLCustomOperation* AOperation, TkbmSQLUnaryNodeOperator AOperator);
	__fastcall virtual ~TkbmSQLUnaryNode(void);
	virtual System::Variant __fastcall Execute(void);
	__property TkbmSQLCustomNode* RightNode = {read=FRightNode, write=FRightNode};
	__property TkbmSQLUnaryNodeOperator Operator = {read=FOperator, nodefault};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLBetweenNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	TkbmSQLCustomNode* FLeftNode;
	TkbmSQLCustomNode* FRangeLow;
	TkbmSQLCustomNode* FRangeHigh;
	
protected:
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLBetweenNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	__fastcall virtual ~TkbmSQLBetweenNode(void);
	virtual System::Variant __fastcall Execute(void);
	__property TkbmSQLCustomNode* LeftNode = {read=FLeftNode, write=FLeftNode};
	__property TkbmSQLCustomNode* RangeLow = {read=FRangeLow, write=FRangeLow};
	__property TkbmSQLCustomNode* RangeHigh = {read=FRangeHigh, write=FRangeHigh};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLInNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	TkbmSQLCustomNode* FLeftNode;
	TkbmSQLNodes* FNodes;
	
protected:
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLInNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	__fastcall virtual ~TkbmSQLInNode(void);
	virtual System::Variant __fastcall Execute(void);
	__property TkbmSQLCustomNode* LeftNode = {read=FLeftNode, write=FLeftNode};
	__property TkbmSQLNodes* Nodes = {read=FNodes};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TkbmSQLFieldNodeType : unsigned char { ntField, ntWildCard, ntRowID, ntRecNo };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLFieldNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	System::UnicodeString FFieldName;
	Data::Db::TField* FOriginatingField;
	System::UnicodeString FTableName;
	TkbmSQLTable* FTable;
	bool FDescending;
	System::UnicodeString FIndexName;
	TkbmSQLFieldNodeType FFieldType;
	bool FOrderBy;
	bool FGroupBy;
	
protected:
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLFieldNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	__fastcall virtual ~TkbmSQLFieldNode(void);
	virtual System::Variant __fastcall Execute(void);
	__property System::UnicodeString FieldName = {read=FFieldName, write=FFieldName};
	__property System::UnicodeString TableName = {read=FTableName, write=FTableName};
	__property System::UnicodeString IndexName = {read=FIndexName, write=FIndexName};
	__property bool Descending = {read=FDescending, write=FDescending, nodefault};
	__property TkbmSQLFieldNodeType FieldType = {read=FFieldType, write=FFieldType, nodefault};
	__property TkbmSQLTable* Table = {read=FTable, write=FTable};
	__property Data::Db::TField* OriginatingField = {read=FOriginatingField, write=FOriginatingField};
	__property bool OrderBy = {read=FOrderBy, write=FOrderBy, nodefault};
	__property bool GroupBy = {read=FGroupBy, write=FGroupBy, nodefault};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLFieldNodes : public TkbmSQLNodes
{
	typedef TkbmSQLNodes inherited;
	
protected:
	virtual void __fastcall SetFieldNode(int AIndex, TkbmSQLFieldNode* ANode);
	virtual TkbmSQLFieldNode* __fastcall GetFieldNode(int AIndex);
	
public:
	__property TkbmSQLFieldNode* FieldNodes[int AIndex] = {read=GetFieldNode, write=SetFieldNode};
public:
	/* TkbmSQLNodes.Create */ inline __fastcall virtual TkbmSQLFieldNodes(const bool AOwnsNodes) : TkbmSQLNodes(AOwnsNodes) { }
	/* TkbmSQLNodes.Destroy */ inline __fastcall virtual ~TkbmSQLFieldNodes(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLGroupFieldNode : public TkbmSQLFieldNode
{
	typedef TkbmSQLFieldNode inherited;
	
public:
	/* TkbmSQLFieldNode.Create */ inline __fastcall virtual TkbmSQLGroupFieldNode(TkbmSQLCustomOperation* AOperation)/* overload */ : TkbmSQLFieldNode(AOperation) { }
	/* TkbmSQLFieldNode.Destroy */ inline __fastcall virtual ~TkbmSQLGroupFieldNode(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLVariableNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	System::UnicodeString FVariableName;
	
protected:
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLVariableNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	__fastcall virtual ~TkbmSQLVariableNode(void);
	virtual System::Variant __fastcall Execute(void);
	virtual bool __fastcall IsVariableValid(void);
	__property System::UnicodeString VariableName = {read=FVariableName, write=FVariableName};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLAggregateNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	TkbmSQLCustomNode* FExpressionNode;
	bool FDistinct;
	
protected:
	TkbmSQLAggregateFunction FAggrFunction;
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	virtual Kbmmemtable::TkbmifoOptions __fastcall GetFieldOptions(void);
	virtual System::UnicodeString __fastcall GetFieldModifier(void);
	
public:
	__fastcall virtual TkbmSQLAggregateNode(TkbmSQLCustomOperation* AOperation)/* overload */;
	__fastcall virtual ~TkbmSQLAggregateNode(void);
	virtual System::UnicodeString __fastcall GetUniqueName(void);
	virtual System::Variant __fastcall Execute(void);
	__property TkbmSQLCustomNode* ExpressionNode = {read=FExpressionNode, write=FExpressionNode};
	__property bool Distinct = {read=FDistinct, write=FDistinct, nodefault};
	__property TkbmSQLAggregateFunction AggrFunction = {read=FAggrFunction, write=FAggrFunction, nodefault};
	__property Kbmmemtable::TkbmifoOptions FieldOptions = {read=GetFieldOptions, nodefault};
	__property System::UnicodeString FieldModifier = {read=GetFieldModifier};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TkbmSQLFunctionSituation : unsigned char { fsWidth, fsExecute, fsDataType };

typedef bool __fastcall (*TkbmSQLCustomFunction)(TkbmSQLCustomOperation* AOperation, TkbmSQLFunctionSituation ASituation, TkbmSQLNodes* AArgs, System::Variant &AResult);

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLFunctionNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
private:
	System::UnicodeString FFunctionName;
	void *FFunction;
	TkbmSQLNodes* FArgs;
	
protected:
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLFunctionNode(TkbmSQLCustomOperation* AOperation, System::UnicodeString AFunctionName, TkbmSQLNodes* AArgs)/* overload */;
	__fastcall virtual ~TkbmSQLFunctionNode(void);
	virtual System::Variant __fastcall Execute(void);
	virtual bool __fastcall IsFunctionValid(void);
	__property System::UnicodeString FunctionName = {read=FFunctionName};
	__property TkbmSQLNodes* Args = {read=FArgs};
public:
	/* TkbmSQLCustomNode.Create */ inline __fastcall virtual TkbmSQLFunctionNode(TkbmSQLCustomOperation* AOperation)/* overload */ : TkbmSQLCustomNode(AOperation) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLCustomValueNode : public TkbmSQLCustomNode
{
	typedef TkbmSQLCustomNode inherited;
	
protected:
	virtual System::Variant __fastcall GetValue(void) = 0 ;
	virtual void __fastcall SetValue(const System::Variant &AValue);
	
public:
	virtual System::Variant __fastcall Execute(void);
	__property System::Variant Value = {read=GetValue, write=SetValue};
public:
	/* TkbmSQLCustomNode.Create */ inline __fastcall virtual TkbmSQLCustomValueNode(TkbmSQLCustomOperation* AOperation)/* overload */ : TkbmSQLCustomNode(AOperation) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLCustomValueNode(void) { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TkbmSQLValueType : unsigned char { evtTimeStamp, evtNull };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLValueNode : public TkbmSQLCustomValueNode
{
	typedef TkbmSQLCustomValueNode inherited;
	
private:
	TkbmSQLValueType FValueType;
	
protected:
	virtual System::Variant __fastcall GetValue(void);
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLValueNode(TkbmSQLCustomOperation* AOperation, TkbmSQLValueType AValueType);
	__property TkbmSQLValueType ValueType = {read=FValueType, nodefault};
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLValueNode(void) { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TkbmSQLConstNode : public TkbmSQLCustomValueNode
{
	typedef TkbmSQLCustomValueNode inherited;
	
private:
	System::Variant FValue;
	
protected:
	virtual System::Variant __fastcall GetValue(void);
	virtual void __fastcall SetValue(const System::Variant &AValue);
	virtual int __fastcall GetWidth(void);
	virtual Data::Db::TFieldType __fastcall GetDataType(void);
	
public:
	__fastcall virtual TkbmSQLConstNode(TkbmSQLCustomOperation* AOperation, const System::Variant &AValue);
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLConstNode(void) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLTable : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::UnicodeString FName;
	System::UnicodeString FAlias;
	Kbmmemtable::TkbmCustomMemTable* FDataset;
	TkbmSQLCustomDBAPI_* FAPI;
	int FAffectedRows;
	TkbmSQLTable* FBaseTable;
	
protected:
	virtual void __fastcall SetName(System::UnicodeString AName);
	virtual void __fastcall SetAlias(System::UnicodeString AAlias);
	System::UnicodeString __fastcall GetAliasOrName(void);
	
public:
	__fastcall virtual TkbmSQLTable(void);
	__int64 __fastcall GetCurrentRowID(void);
	int __fastcall GetCurrentRecNo(void);
	__property System::UnicodeString AliasOrName = {read=GetAliasOrName};
	__property System::UnicodeString Name = {read=FName, write=SetName};
	__property System::UnicodeString Alias = {read=FAlias, write=SetAlias};
	__property Kbmmemtable::TkbmCustomMemTable* Dataset = {read=FDataset, write=FDataset};
	__property TkbmSQLCustomDBAPI_* API = {read=FAPI, write=FAPI};
	__property int AffectedRows = {read=FAffectedRows, write=FAffectedRows, nodefault};
	__property TkbmSQLTable* BaseTable = {read=FBaseTable, write=FBaseTable};
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLTable(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLTables : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	TkbmSQLTable* operator[](int AIndex) { return Tables[AIndex]; }
	
private:
	System::Classes::TList* FList;
	bool FOwnsTables;
	
protected:
	virtual int __fastcall GetCount(void);
	virtual TkbmSQLTable* __fastcall GetTable(int AIndex);
	TkbmSQLTable* __fastcall GetTableByName(System::UnicodeString AName);
	TkbmSQLTable* __fastcall GetTableByAlias(System::UnicodeString AAlias);
	
public:
	__fastcall virtual TkbmSQLTables(const bool AOwnTables);
	__fastcall virtual ~TkbmSQLTables(void);
	void __fastcall Clear(void);
	void __fastcall Add(TkbmSQLTable* ATable)/* overload */;
	void __fastcall Add(System::UnicodeString AName, Kbmmemtable::TkbmCustomMemTable* ADataset)/* overload */;
	void __fastcall AddUnique(TkbmSQLTable* ATable);
	void __fastcall Delete(TkbmSQLTable* ATable)/* overload */;
	void __fastcall Delete(System::UnicodeString AName)/* overload */;
	TkbmSQLTables* __fastcall GetTablesContainingFieldName(System::UnicodeString AFieldName);
	__property TkbmSQLTable* Tables[int AIndex] = {read=GetTable/*, default*/};
	__property TkbmSQLTable* TableByName[System::UnicodeString AName] = {read=GetTableByName};
	__property TkbmSQLTable* TableByAlias[System::UnicodeString AAlias] = {read=GetTableByAlias};
	__property int Count = {read=GetCount, nodefault};
	__property bool OwnsTables = {read=FOwnsTables, nodefault};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TkbmSQLOperationType : unsigned char { sotSELECT, sotUPDATE, sotINSERT, sotDELETE, sotEVALUATE };

enum DECLSPEC_DENUM TkbmSQLOperationParseState : unsigned char { sopsDefault, sopsSearchCondition, sopsHavingCondition, sopsSelection, sopsUpdate, sopsInsert, sopsEvaluation, sopsFilter };

enum DECLSPEC_DENUM TkbmSQLOperationFlag : unsigned char { sopfNoBooleanExpressions, sopfOnlyNumericExpressions };

typedef System::Set<TkbmSQLOperationFlag, TkbmSQLOperationFlag::sopfNoBooleanExpressions, TkbmSQLOperationFlag::sopfOnlyNumericExpressions> TkbmSQLOperationFlags;

enum DECLSPEC_DENUM TkbmSQLExpressionOption : unsigned char { seoOnlyNumericExpressions };

typedef System::Set<TkbmSQLExpressionOption, TkbmSQLExpressionOption::seoOnlyNumericExpressions, TkbmSQLExpressionOption::seoOnlyNumericExpressions> TkbmSQLExpressionOptions;

typedef TkbmSQLCustomNode* __fastcall (__closure *TkbmSQLTraverseFunction)(TkbmSQLCustomNode* const AParent, TkbmSQLCustomNode* const ANode);

typedef bool __fastcall (__closure *TkbmSQLOnGetVariableValue)(TkbmSQLCustomNode* const ANode, const System::UnicodeString AVariableName, System::Variant &AValue);

typedef bool __fastcall (__closure *TkbmSQLOnGetVariableMetaData)(TkbmSQLCustomNode* const ANode, const System::UnicodeString AVariableName, int &AWidth, Data::Db::TFieldType &ADataType);

typedef bool __fastcall (__closure *TkbmSQLOnGetFunction)(TkbmSQLCustomNode* const ANode, const System::UnicodeString AFunctionName, const System::UnicodeString AFunctionGroup, TkbmSQLCustomFunction &AFunction);

class PASCALIMPLEMENTATION TkbmSQLCustomOperation : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	TkbmSQLOperationType FType;
	System::TObject* FOwner;
	TkbmSQLFieldNodes* FRefSourceFields;
	TkbmSQLTables* FRefSourceTables;
	TkbmSQLFieldNodes* FRefSearchFields;
	TkbmSQLFieldNodes* FRefHavingFields;
	TkbmSQLFieldNodes* FRefWildcards;
	TkbmSQLOperationParseState FParseState;
	TkbmSQLOperationFlags FFlags;
	System::TObject* FContext;
	TkbmSQLOnGetVariableValue FOnGetVariableValue;
	TkbmSQLOnGetVariableMetaData FOnGetVariableMetaData;
	TkbmSQLOnGetFunction FOnGetFunction;
	int FNextID;
	
protected:
	virtual TkbmSQLCustomNode* __fastcall GetFieldByAlias(const System::UnicodeString AAlias);
	void __fastcall ReplaceParentRef(System::TObject* const AParent, TkbmSQLCustomNode* const AOldNode, TkbmSQLCustomNode* const ANewNode);
	
public:
	System::Sysutils::TFormatSettings FormatSettings;
	__fastcall virtual TkbmSQLCustomOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLCustomOperation(void);
	virtual void __fastcall FixupRefSourceFields(void);
	virtual int __fastcall GetErrorColOffset(void);
	virtual void __fastcall OptimizeNodeTree(TkbmSQLCustomNode* &ANode);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	int __fastcall GetNextID(void);
	__property System::TObject* Owner = {read=FOwner};
	__property TkbmSQLOperationType OpType = {read=FType, nodefault};
	__property TkbmSQLFieldNodes* RefSourceFields = {read=FRefSourceFields};
	__property TkbmSQLTables* RefSourceTables = {read=FRefSourceTables};
	__property TkbmSQLFieldNodes* RefSearchFields = {read=FRefSearchFields};
	__property TkbmSQLFieldNodes* RefHavingFields = {read=FRefHavingFields};
	__property TkbmSQLFieldNodes* RefWildcards = {read=FRefWildcards};
	__property TkbmSQLOperationParseState ParseState = {read=FParseState, write=FParseState, nodefault};
	__property TkbmSQLOperationFlags Flags = {read=FFlags, write=FFlags, nodefault};
	__property System::TObject* Context = {read=FContext, write=FContext};
	__property TkbmSQLOnGetVariableValue OnGetVariableValue = {read=FOnGetVariableValue, write=FOnGetVariableValue};
	__property TkbmSQLOnGetVariableMetaData OnGetVariableMetaData = {read=FOnGetVariableMetaData, write=FOnGetVariableMetaData};
	__property TkbmSQLOnGetFunction OnGetFunction = {read=FOnGetFunction, write=FOnGetFunction};
};


class PASCALIMPLEMENTATION TkbmSQLCustomSelectOperation : public TkbmSQLCustomOperation
{
	typedef TkbmSQLCustomOperation inherited;
	
private:
	TkbmSQLNodes* FSelection;
	TkbmSQLNodes* FAggregates;
	TkbmSQLTables* FTables;
	TkbmSQLCustomNode* FCondition;
	bool FDistinct;
	TkbmSQLNodes* FOrder;
	TkbmSQLNodes* FGroup;
	TkbmSQLCustomNode* FHavingCondition;
	TkbmSQLCustomNode* FLimit;
	TkbmSQLCustomNode* FOffset;
	Kbmmemtable::TkbmCustomMemTable* FResultTable;
	
protected:
	bool __fastcall GetIsAggregate(void);
	bool __fastcall GetIsDistinctAggregate(void);
	virtual TkbmSQLCustomNode* __fastcall GetFieldByAlias(const System::UnicodeString AAlias);
	
public:
	__fastcall virtual TkbmSQLCustomSelectOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLCustomSelectOperation(void);
	virtual void __fastcall FixupSelectionWildcardNodes(void);
	virtual void __fastcall FixupRefSourceFields(void);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	__property TkbmSQLNodes* Selection = {read=FSelection};
	__property TkbmSQLNodes* Aggregates = {read=FAggregates};
	__property TkbmSQLTables* Tables = {read=FTables};
	__property TkbmSQLCustomNode* Condition = {read=FCondition, write=FCondition};
	__property TkbmSQLNodes* Order = {read=FOrder};
	__property TkbmSQLNodes* Group = {read=FGroup};
	__property TkbmSQLCustomNode* HavingCondition = {read=FHavingCondition, write=FHavingCondition};
	__property TkbmSQLCustomNode* Limit = {read=FLimit, write=FLimit};
	__property TkbmSQLCustomNode* Offset = {read=FOffset, write=FOffset};
	__property Kbmmemtable::TkbmCustomMemTable* ResultTable = {read=FResultTable, write=FResultTable};
	__property bool IsAggregate = {read=GetIsAggregate, nodefault};
	__property bool IsDistinctAggregate = {read=GetIsDistinctAggregate, nodefault};
	__property bool Distinct = {read=FDistinct, write=FDistinct, nodefault};
};


class PASCALIMPLEMENTATION TkbmSQLSelectOperation : public TkbmSQLCustomSelectOperation
{
	typedef TkbmSQLCustomSelectOperation inherited;
	
public:
	/* TkbmSQLCustomSelectOperation.Create */ inline __fastcall virtual TkbmSQLSelectOperation(System::TObject* AOwner) : TkbmSQLCustomSelectOperation(AOwner) { }
	/* TkbmSQLCustomSelectOperation.Destroy */ inline __fastcall virtual ~TkbmSQLSelectOperation(void) { }
	
};


class PASCALIMPLEMENTATION TkbmSQLUpdateOperation : public TkbmSQLCustomOperation
{
	typedef TkbmSQLCustomOperation inherited;
	
private:
	TkbmSQLTable* FTable;
	TkbmSQLFieldNodes* FFields;
	TkbmSQLNodes* FValues;
	TkbmSQLCustomNode* FCondition;
	
public:
	__fastcall virtual TkbmSQLUpdateOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLUpdateOperation(void);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	__property TkbmSQLTable* Table = {read=FTable, write=FTable};
	__property TkbmSQLFieldNodes* Fields = {read=FFields};
	__property TkbmSQLNodes* Values = {read=FValues};
	__property TkbmSQLCustomNode* Condition = {read=FCondition, write=FCondition};
};


class PASCALIMPLEMENTATION TkbmSQLDeleteOperation : public TkbmSQLCustomOperation
{
	typedef TkbmSQLCustomOperation inherited;
	
private:
	TkbmSQLTable* FTable;
	TkbmSQLCustomNode* FCondition;
	
public:
	__fastcall virtual TkbmSQLDeleteOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLDeleteOperation(void);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	__property TkbmSQLTable* Table = {read=FTable, write=FTable};
	__property TkbmSQLCustomNode* Condition = {read=FCondition, write=FCondition};
};


class PASCALIMPLEMENTATION TkbmSQLInsertOperation : public TkbmSQLCustomOperation
{
	typedef TkbmSQLCustomOperation inherited;
	
private:
	TkbmSQLTable* FTable;
	TkbmSQLFieldNodes* FFields;
	TkbmSQLNodes* FValues;
	
public:
	__fastcall virtual TkbmSQLInsertOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLInsertOperation(void);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	__property TkbmSQLTable* Table = {read=FTable, write=FTable};
	__property TkbmSQLFieldNodes* Fields = {read=FFields};
	__property TkbmSQLNodes* Values = {read=FValues};
};


class PASCALIMPLEMENTATION TkbmSQLEvaluationOperation : public TkbmSQLCustomOperation
{
	typedef TkbmSQLCustomOperation inherited;
	
private:
	TkbmSQLCustomNode* FEvaluation;
	TkbmSQLFieldNodes* FFields;
	
public:
	__fastcall virtual TkbmSQLEvaluationOperation(System::TObject* AOwner);
	__fastcall virtual ~TkbmSQLEvaluationOperation(void);
	virtual int __fastcall GetErrorColOffset(void);
	virtual void __fastcall Prepare(void);
	virtual void __fastcall Optimize(void);
	virtual void __fastcall Execute(void);
	virtual System::Variant __fastcall Evaluate(void);
	__property TkbmSQLFieldNodes* Fields = {read=FFields};
	__property TkbmSQLCustomNode* Evaluation = {read=FEvaluation, write=FEvaluation};
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE Data::Db::TFieldType __fastcall kbmSQLMergeDataTypes(Data::Db::TFieldType ALeft, Data::Db::TFieldType ARight);
extern DELPHI_PACKAGE double __fastcall kbmSQLStrToFloat(System::UnicodeString AString);
}	/* namespace Kbmsqlelements */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMSQLELEMENTS)
using namespace Kbmsqlelements;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmsqlelementsHPP
