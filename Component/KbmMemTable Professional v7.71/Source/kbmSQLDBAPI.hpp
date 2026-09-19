// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmSQLDBAPI.pas' rev: 30.00 (Windows)

#ifndef KbmsqldbapiHPP
#define KbmsqldbapiHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Data.DB.hpp>
#include <kbmSQLElements.hpp>
#include <kbmMemTable.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmsqldbapi
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TkbmSQLDBAPIRegistration;
class DELPHICLASS TkbmSQLDBAPIRegistrations;
class DELPHICLASS TkbmSQLCustomDBAPI;
class DELPHICLASS TkbmSQLCustomDatasetAPI;
//-- type declarations -------------------------------------------------------
typedef System::TMetaClass* TkbmSQLCustomDBAPIClass;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLDBAPIRegistration : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::TClass FDataset;
	TkbmSQLCustomDBAPI* FDatasetAPI;
	
public:
	__fastcall virtual TkbmSQLDBAPIRegistration(const System::TClass ADataset, const TkbmSQLCustomDBAPIClass ADatasetAPIClass);
	__fastcall virtual ~TkbmSQLDBAPIRegistration(void);
	__property System::TClass Dataset = {read=FDataset};
	__property TkbmSQLCustomDBAPI* DatasetAPI = {read=FDatasetAPI};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLDBAPIRegistrations : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::Classes::TList* FList;
	
public:
	__fastcall virtual TkbmSQLDBAPIRegistrations(void);
	__fastcall virtual ~TkbmSQLDBAPIRegistrations(void);
	void __fastcall Clear(void);
	void __fastcall RegisterAPI(System::TClass ADatasetClass, TkbmSQLCustomDBAPIClass ADatasetAPIClass);
	TkbmSQLCustomDBAPI* __fastcall GetAPI(System::TClass ADataset);
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLCustomDBAPI : public Kbmsqlelements::TkbmSQLCustomDBAPI_
{
	typedef Kbmsqlelements::TkbmSQLCustomDBAPI_ inherited;
	
protected:
	virtual Data::Db::TField* __fastcall CreateField(System::Classes::TComponent* const AOwner, const Data::Db::TFieldType ADataType);
	virtual void __fastcall CheckNonAggregateFieldsInGroupClause(Kbmsqlelements::TkbmSQLNodes* const ASelection, Kbmsqlelements::TkbmSQLNodes* const AGroup);
	virtual void __fastcall CreateSelectionFields(Kbmmemtable::TkbmCustomMemTable* const ATable, Kbmsqlelements::TkbmSQLNodes* const ASelection, const bool ACreateGroupFields, const bool AUseAggregateSources);
	virtual void __fastcall CreateDistinctIndex(Kbmmemtable::TkbmCustomMemTable* const ATable, Kbmsqlelements::TkbmSQLNodes* const ADistinct);
	virtual void __fastcall AppendResultRecord(Kbmmemtable::TkbmCustomMemTable* const AResultTable, Kbmsqlelements::TkbmSQLNodes* const ASelection, const bool AIncludeGroupFields)/* overload */;
	virtual void __fastcall ApplyGroupBy(Kbmmemtable::TkbmCustomMemTable* const ASourceTable, Kbmmemtable::TkbmCustomMemTable* const AResultTable, Kbmsqlelements::TkbmSQLNodes* const ASelection, Kbmsqlelements::TkbmSQLNodes* const AGroup, Kbmsqlelements::TkbmSQLNodes* const AAggregates);
	virtual void __fastcall ApplyOrderBy(Kbmmemtable::TkbmCustomMemTable* const AResultTable, Kbmsqlelements::TkbmSQLNodes* const AOrder);
	virtual void __fastcall ApplyOffsetAndLimit(Kbmmemtable::TkbmCustomMemTable* const AResultTable, Kbmsqlelements::TkbmSQLCustomNode* const AOffset, Kbmsqlelements::TkbmSQLCustomNode* const ALimit);
	virtual void __fastcall DetermineUsableIndexesForRefFields(Kbmsqlelements::TkbmSQLCustomOperation* const AOperation);
	virtual void __fastcall SetupSourceFields(Kbmsqlelements::TkbmSQLNodes* const ANodes, Data::Db::TDataSet* const ADataset);
	virtual void __fastcall SetupSourceField(Kbmsqlelements::TkbmSQLCustomNode* const ANode, Data::Db::TDataSet* const ADataset);
	virtual void __fastcall Search(Kbmmemtable::TkbmCustomMemTable* const ASourceTable, Kbmmemtable::TkbmCustomMemTable* const AResultTable, Kbmsqlelements::TkbmSQLCustomSelectOperation* const AOperation, Kbmsqlelements::TkbmSQLNodes* const ASelection, Kbmsqlelements::TkbmSQLCustomNode* const ACondition, const bool ACopyDirectlyFromSource);
	virtual System::UnicodeString __fastcall GetIndexNameForField(Kbmsqlelements::TkbmSQLFieldNode* const AField) = 0 ;
	virtual void __fastcall FixupTemporaryResultFields(Kbmmemtable::TkbmCustomMemTable* const ATempResultTable, Kbmsqlelements::TkbmSQLCustomSelectOperation* const AOperation, Kbmsqlelements::TkbmSQLFieldNodes* const AFields);
	
public:
	virtual bool __fastcall LocateFirst(Kbmsqlelements::TkbmSQLTable* const ATable, Kbmsqlelements::TkbmSQLCustomNode* const ACondition);
	virtual bool __fastcall LocateNext(Kbmsqlelements::TkbmSQLTable* const ATable, Kbmsqlelements::TkbmSQLCustomNode* const ACondition);
	virtual bool __fastcall Insert(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLInsertOperation* const AOperation) = 0 ;
	virtual bool __fastcall Delete(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLDeleteOperation* const AOperation) = 0 ;
	virtual bool __fastcall Update(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLUpdateOperation* const AOperation) = 0 ;
	virtual bool __fastcall Select(System::TObject* const ADataset, Kbmmemtable::TkbmCustomMemTable* const AResultDataset, Kbmsqlelements::TkbmSQLCustomSelectOperation* const AOperation) = 0 ;
public:
	/* TObject.Create */ inline __fastcall TkbmSQLCustomDBAPI(void) : Kbmsqlelements::TkbmSQLCustomDBAPI_() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLCustomDBAPI(void) { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TkbmSQLCustomDatasetAPI : public TkbmSQLCustomDBAPI
{
	typedef TkbmSQLCustomDBAPI inherited;
	
public:
	virtual bool __fastcall Insert(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLInsertOperation* const AOperation);
	virtual bool __fastcall Delete(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLDeleteOperation* const AOperation);
	virtual bool __fastcall Update(System::TObject* const ADataset, Kbmsqlelements::TkbmSQLUpdateOperation* const AOperation);
	virtual bool __fastcall Select(System::TObject* const ADataset, Kbmmemtable::TkbmCustomMemTable* const AResultDataset, Kbmsqlelements::TkbmSQLCustomSelectOperation* const AOperation);
public:
	/* TObject.Create */ inline __fastcall TkbmSQLCustomDatasetAPI(void) : TkbmSQLCustomDBAPI() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TkbmSQLCustomDatasetAPI(void) { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
#define KBMSQL_UNIQUE_INDEXNAME L"__KBMSQL_UNIQUE"
#define KBMSQL_ORDERBY_INDEXNAME L"__KBMSQL_ORDERBY"
extern DELPHI_PACKAGE TkbmSQLDBAPIRegistrations* kbmSQLDBAPIRegistrations;
}	/* namespace Kbmsqldbapi */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMSQLDBAPI)
using namespace Kbmsqldbapi;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmsqldbapiHPP
