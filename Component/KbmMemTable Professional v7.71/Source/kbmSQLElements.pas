unit kbmSQLElements;

// =========================================================================
// A kbmMemTable based SQL implementation.
//
// Copyright 2007-2013 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com

interface

{$I kbmMemTable.inc}

uses
   Classes,
{$IFDEF NEXTGEN}
   System.Generics.Collections,
{$ENDIF}
   SysUtils,
   DB,
   kbmMemTable;

type
   TkbmSQLTable = class;
   TkbmSQLCustomNode = class;
   TkbmSQLCustomOperation = class;
   TkbmSQLFieldNode = class;

   TkbmSQLBinaryNodeOperator = (ebAdd,ebSub,ebMul,ebDiv,ebAnd,ebOr,ebXor,ebMod,ebIDiv,
                                ebLike,ebEqual,ebNotEqual,ebLess,ebGreater,ebLessEqual,ebGreaterEqual,ebConcat);
   TkbmSQLUnaryNodeOperator = (euNot,euNegate);

   TkbmSQLAggregateFunction = (safUNKNOWN,safCOUNT,safMAX,safMIN,safAVG,safSUM,safSTDDEV);

   TkbmSQLCustomDBAPI_ = class
   end;

   TkbmSQLNodes = class;

   TkbmSQLCustomNode = class
   private
      FID:integer;
      FUniqueName:string;
      FAlias:string;
      FDescription:string;
      FDataType:TFieldType;
      FNonScalar:boolean;
      FNodes:TkbmSQLNodes;
      FExecuted:boolean;

{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FOperation:TkbmSQLCustomOperation;
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FDestinationField:TField;
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FSourceField:TField;
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FParent:TObject;
   protected
      function GetExpectedWidth:integer; virtual;
      function GetWidth:integer; virtual;
      function GetNonScalar:boolean; virtual;
      function GetDataType:TFieldType; virtual;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); overload; virtual;

      function Execute:variant; virtual; abstract;
      property UniqueName:string read FUniqueName write FUniqueName;
      function GetUniqueName:string; virtual;
      property Alias:string read FAlias write FAlias;
      property Description:string read FDescription write FDescription;
      property ID:integer read FID write FID;
      property DataType:TFieldType read GetDataType write FDataType;
      property Width:integer read GetWidth;
      property NonScalar:boolean read GetNonScalar;
      property Operation:TkbmSQLCustomOperation read FOperation;
      property Nodes:TkbmSQLNodes read FNodes write FNodes;
      property Parent:TObject read FParent write FParent;
      property DestinationField:TField read FDestinationField write FDestinationField;
      property SourceField:TField read FSourceField write FSourceField;
      property Executed:boolean read FExecuted;
   end;

   TkbmSQLNodes = class
   private
{$IFDEF NEXTGEN}
      FList:TList<TkbmSQLCustomNode>;
{$ELSE}
      FList:TList;
{$ENDIF}
      FOwnsNodes:boolean;
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FParent:TkbmSQLCustomNode;
   protected
      function GetNode(const AIndex:integer):TkbmSQLCustomNode;
      procedure SetNode(const AIndex:integer; const ANode:TkbmSQLCustomNode);
   public
      constructor Create(const AOwnsNodes:boolean); virtual;
      destructor Destroy; override;

      function GetByUniqueName(AUniqueName:string):TkbmSQLCustomNode;
      function GetByAlias(AAlias:string):TkbmSQLCustomNode;
      function GetByFieldName(const AName:string):TkbmSQLFieldNode;
      function IndexOf(const ANode:TkbmSQLCustomNode):integer;
      procedure Add(const ANode:TkbmSQLCustomNode; const AIgnoreIfExists:boolean = false); overload;
      procedure Add(const ANodes:TkbmSQLNodes; const AIgnoreIfExists:boolean = false); overload;
      procedure Insert(const APos:integer; const ANode:TkbmSQLCustomNode);
      procedure Delete(const APos:integer); overload;
      procedure Delete(const ANode:TkbmSQLCustomNode); overload;
      procedure Replace(const AOldNode,ANewNode:TkbmSQLCustomNode; const AFreeOldNode:boolean = false);
      function Count:integer;
      procedure ClearExecuted;
      procedure Clear;

      property Node[const AIndex:integer]:TkbmSQLCustomNode read GetNode write SetNode; default;
      property Parent:TkbmSQLCustomNode read FParent write FParent;
   end;

   TkbmSQLBinaryNode = class(TkbmSQLCustomNode)
   private
      FLeftNode,FRightNode:TkbmSQLCustomNode;
      FOperator:TkbmSQLBinaryNodeOperator;
   protected
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation; AOperator:TkbmSQLBinaryNodeOperator); overload; virtual;
      destructor Destroy; override;

      function Execute:variant; override;

      property LeftNode:TkbmSQLCustomNode read FLeftNode write FLeftNode;
      property RightNode:TkbmSQLCustomNode read FRightNode write FRightNode;
      property Operator:TkbmSQLBinaryNodeOperator read FOperator;
   end;


   TkbmSQLUnaryNode = class(TkbmSQLCustomNode)
   private
      FRightNode:TkbmSQLCustomNode;
      FOperator:TkbmSQLUnaryNodeOperator;
   protected
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation; AOperator:TkbmSQLUnaryNodeOperator); virtual;
      destructor Destroy; override;

      function Execute:variant; override;

      property RightNode:TkbmSQLCustomNode read FRightNode write FRightNode;
      property Operator:TkbmSQLUnaryNodeOperator read FOperator;
   end;

   TkbmSQLBetweenNode = class(TkbmSQLCustomNode)
   private
      FLeftNode:TkbmSQLCustomNode;
      FRangeLow:TkbmSQLCustomNode;
      FRangeHigh:TkbmSQLCustomNode;
   protected
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); override;
      destructor Destroy; override;

      function Execute:variant; override;

      property LeftNode:TkbmSQLCustomNode read FLeftNode write FLeftNode;
      property RangeLow:TkbmSQLCustomNode read FRangeLow write FRangeLow;
      property RangeHigh:TkbmSQLCustomNode read FRangeHigh write FRangeHigh;
   end;

   TkbmSQLInNode = class(TkbmSQLCustomNode)
   private
      FLeftNode:TkbmSQLCustomNode;
      FNodes:TkbmSQLNodes;
   protected
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); override;
      destructor Destroy; override;

      function Execute:variant; override;

      property LeftNode:TkbmSQLCustomNode read FLeftNode write FLeftNode;
      property Nodes:TkbmSQLNodes read FNodes;
   end;

   TkbmSQLFieldNodeType = (ntField,ntWildCard,ntRowID,ntRecNo);
   TkbmSQLFieldNode = class(TkbmSQLCustomNode)
   private
      FFieldName:string;
      FOriginatingField:TField;
      FTableName:string;
      FTable:TkbmSQLTable;
      FDescending:boolean;
      FIndexName:string;
      FFieldType:TkbmSQLFieldNodeType;
      FOrderBy:boolean;
      FGroupBy:boolean;
   protected
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); override;
      destructor Destroy; override;

      function Execute:variant; override;

      property FieldName:string read FFieldName write FFieldname;
      property TableName:string read FTableName write FTableName;
      property IndexName:string read FIndexName write FIndexName;
      property Descending:boolean read FDescending write FDescending;
      property FieldType:TkbmSQLFieldNodeType read FFieldType write FFieldType;
      property Table:TkbmSQLTable read FTable write FTable;
      property OriginatingField:TField read FOriginatingField write FOriginatingField;
      property OrderBy:boolean read FOrderBy write FOrderBy;
      property GroupBy:boolean read FGroupBy write FGroupBy;
   end;

   TkbmSQLFieldNodes = class(TkbmSQLNodes)
   protected
      procedure SetFieldNode(AIndex:integer; ANode:TkbmSQLFieldNode); virtual;
      function GetFieldNode(AIndex:integer):TkbmSQLFieldNode; virtual;
   public
      property FieldNodes[AIndex:integer]:TkbmSQLFieldNode read GetFieldNode write SetFieldNode;
   end;

   TkbmSQLGroupFieldNode = class(TkbmSQLFieldNode);

   TkbmSQLVariableNode = class(TkbmSQLCustomNode)
   private
      FVariableName:string;
   protected
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); override;
      destructor Destroy; override;

      function Execute:variant; override;
      function IsVariableValid:boolean; virtual;

      property VariableName:string read FVariableName write FVariableName;
   end;

   TkbmSQLAggregateNode = class(TkbmSQLCustomNode)
   private
      FExpressionNode:TkbmSQLCustomNode;
      FDistinct:boolean;
   protected
      FAggrFunction:TkbmSQLAggregateFunction;
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
      function GetFieldOptions:TkbmifoOptions; virtual;
      function GetFieldModifier:string; virtual;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation); override;
      destructor Destroy; override;

      function GetUniqueName:string; override;

      function Execute:variant; override;

      property ExpressionNode:TkbmSQLCustomNode read FExpressionNode write FExpressionNode;
      property Distinct:boolean read FDistinct write FDistinct;
      property AggrFunction:TkbmSQLAggregateFunction read FAggrFunction write FAggrFunction;
      property FieldOptions:TkbmifoOptions read GetFieldOptions;
      property FieldModifier:string read GetFieldModifier;
   end;

   TkbmSQLFunctionSituation = (fsWidth,fsExecute,fsDataType);

   TkbmSQLCustomFunction = function (AOperation:TkbmSQLCustomOperation; ASituation:TkbmSQLFunctionSituation; AArgs:TkbmSQLNodes; var AResult:variant):boolean;

   TkbmSQLFunctionNode = class(TkbmSQLCustomNode)
   private
      FFunctionName:string;
      FFunction:pointer;
      FArgs:TkbmSQLNodes;
   protected
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation; AFunctionName:string; AArgs:TkbmSQLNodes); overload; virtual;
      destructor Destroy; override;

      function Execute:variant; override;
      function IsFunctionValid:boolean; virtual;

      property FunctionName:string read FFunctionName;
      property Args:TkbmSQLNodes read FArgs;
   end;

   TkbmSQLCustomValueNode = class(TkbmSQLCustomNode)
   protected
      function GetValue:variant; virtual; abstract;
      procedure SetValue(AValue:variant); virtual;
   public
      function Execute:variant; override;

      property Value:variant read GetValue write SetValue;
   end;

   TkbmSQLValueType = (evtTimeStamp,evtNull);

   TkbmSQLValueNode = class(TkbmSQLCustomValueNode)
   private
      FValueType:TkbmSQLValueType;
   protected
      function GetValue:variant; override;
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation; AValueType:TkbmSQLValueType); virtual;

      property ValueType:TkbmSQLValueType read FValueType;
   end;

   TkbmSQLConstNode = class(TkbmSQLCustomValueNode)
   private
      FValue:variant;
   protected
      function GetValue:variant; override;
      procedure SetValue(AValue:variant); override;
      function GetWidth:integer; override;
      function GetDataType:TFieldType; override;
   public
      constructor Create(AOperation:TkbmSQLCustomOperation; AValue:variant); virtual;
   end;

   TkbmSQLTable = class
   private
      FName:string;
      FAlias:string;
      FDataset:TkbmCustomMemTable;
      FAPI:TkbmSQLCustomDBAPI_;
      FAffectedRows:integer;
      FBaseTable:TkbmSQLTable;
   protected
      procedure SetName(AName:string); virtual;
      procedure SetAlias(AAlias:string); virtual;
      function GetAliasOrName:string;

   public
      constructor Create; virtual;

      function GetCurrentRowID:int64;
      function GetCurrentRecNo:integer;

      property AliasOrName:string read GetAliasOrName;
      property Name:string read FName write SetName;
      property Alias:string read FAlias write SetAlias;
      property Dataset:TkbmCustomMemTable read FDataset write FDataset;
      property API:TkbmSQLCustomDBAPI_ read FAPI write FAPI;
      property AffectedRows:integer read FAffectedRows write FAffectedRows;
      property BaseTable:TkbmSQLTable read FBaseTable write FBaseTable;
   end;

   TkbmSQLTables = class
   private
{$IFDEF NEXTGEN}
      FList:TList<TkbmSQLTable>;
{$ELSE}
      FList:TList;
      FOwnsTables:boolean;
{$ENDIF}
   protected
      function GetCount:integer; virtual;
      function GetTable(AIndex:integer):TkbmSQLTable; virtual;
      function GetTableByName(AName:string):TkbmSQLTable;
      function GetTableByAlias(AAlias:string):TkbmSQLTable;
   public
      constructor Create(const AOwnTables:boolean); virtual;
      destructor Destroy; override;
      procedure Clear;
      procedure Add(ATable:TkbmSQLTable); overload;
      procedure Add(AName:string; ADataset:TkbmCustomMemTable); overload;
      procedure AddUnique(ATable:TkbmSQLTable);
      procedure Delete(ATable:TkbmSQLTable); overload;
      procedure Delete(AName:string); overload;
      function GetTablesContainingFieldName(AFieldName:string):TkbmSQLTables;

      property Tables[AIndex:integer]:TkbmSQLTable read GetTable; default;
      property TableByName[AName:string]:TkbmSQLTable read GetTableByName;
      property TableByAlias[AAlias:string]:TkbmSQLTable read GetTableByAlias;
      property Count:integer read GetCount;
{$IFNDEF NEXTGEN}
      property OwnsTables:boolean read FOwnsTables;
{$ENDIF}
   end;

   TkbmSQLOperationType = (sotSELECT,sotUPDATE,sotINSERT,sotDELETE,sotEVALUATE);

   TkbmSQLOperationParseState = (sopsDefault,sopsSearchCondition,sopsHavingCondition,sopsSelection,sopsUpdate,sopsInsert,sopsEvaluation,sopsFilter);
   TkbmSQLOperationFlag = (sopfNoBooleanExpressions,sopfOnlyNumericExpressions);
   TkbmSQLOperationFlags = set of TkbmSQLOperationFlag;

   TkbmSQLExpressionOption = (seoOnlyNumericExpressions);
   TkbmSQLExpressionOptions = set of TkbmSQLExpressionOption;

   TkbmSQLTraverseFunction = function(const AParent:TkbmSQLCustomNode; const ANode:TkbmSQLCustomNode):TkbmSQLCustomNode of object;
   TkbmSQLOnGetVariableValue = function(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AValue:variant):boolean of object;
   TkbmSQLOnGetVariableMetaData = function(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AWidth:integer; var ADataType:TFieldType):boolean of object;
   TkbmSQLOnGetFunction = function(const ANode:TkbmSQLCustomNode; const AFunctionName:string; const AFunctionGroup:string; var AFunction:TkbmSQLCustomFunction):boolean of object;

   TkbmSQLCustomOperation = class
   private
      FType:TkbmSQLOperationType;
      FOwner:TObject;
      FRefSourceFields:TkbmSQLFieldNodes;
      FRefSourceTables:TkbmSQLTables;
      FRefSearchFields:TkbmSQLFieldNodes;
      FRefHavingFields:TkbmSQLFieldNodes;
      FRefWildcards:TkbmSQLFieldNodes;
      FParseState:TkbmSQLOperationParseState;
      FFlags:TkbmSQLOperationFlags;
      FContext:TObject;
      FOnGetVariableValue:TkbmSQLOnGetVariableValue;
      FOnGetVariableMetaData:TkbmSQLOnGetVariableMetaData;
      FOnGetFunction:TkbmSQLOnGetFunction;
      FNextID:integer;
   protected
      function GetFieldByAlias(const AAlias:string):TkbmSQLCustomNode; virtual;
      procedure ReplaceParentRef(const AParent:TObject; const AOldNode,ANewNode:TkbmSQLCustomNode);
   public
{$IFDEF LEVEL9}
      FormatSettings:TFormatSettings;
{$ENDIF}

      constructor Create(AOwner:TObject); virtual;
      destructor Destroy; override;

      procedure FixupRefSourceFields; virtual;

      function GetErrorColOffset:integer; virtual;
      procedure OptimizeNodeTree(var ANode:TkbmSQLCustomNode); virtual;

      procedure Prepare; virtual;
      procedure Optimize; virtual;
      procedure Execute; virtual;
      function GetNextID:integer;

      property Owner:TObject read FOwner;
      property OpType:TkbmSQLOperationType read FType;
      property RefSourceFields:TkbmSQLFieldNodes read FRefSourceFields;
      property RefSourceTables:TkbmSQLTables read FRefSourceTables;
      property RefSearchFields:TkbmSQLFieldNodes read FRefSearchFields;
      property RefHavingFields:TkbmSQLFieldNodes read FRefHavingFields;
      property RefWildcards:TkbmSQLFieldNodes read FRefWildcards;
      property ParseState:TkbmSQLOperationParseState read FParseState write FParseState;
      property Flags:TkbmSQLOperationFlags read FFlags write FFlags;
      property Context:TObject read FContext write FContext;
      property OnGetVariableValue:TkbmSQLOnGetVariableValue read FOnGetVariableValue write FOnGetVariableValue;
      property OnGetVariableMetaData:TkbmSQLOnGetVariableMetaData read FOnGetVariableMetaData write FOnGetVariableMetaData;
      property OnGetFunction:TkbmSQLOnGetFunction read FOnGetFunction write FOnGetFunction;
   end;

   TkbmSQLCustomSelectOperation = class(TkbmSQLCustomOperation)
   private
      FSelection:TkbmSQLNodes;
      FAggregates:TkbmSQLNodes;
      FTables:TkbmSQLTables;
      FCondition:TkbmSQLCustomNode;
      FDistinct:boolean;
      FOrder:TkbmSQLNodes;
      FGroup:TkbmSQLNodes;
      FHavingCondition:TkbmSQLCustomNode;
      FLimit:TkbmSQLCustomNode;
      FOffset:TkbmSQLCustomNode;
      FResultTable:TkbmCustomMemTable;
   protected
      function GetIsAggregate:boolean;
      function GetIsDistinctAggregate:boolean;
      function GetFieldByAlias(const AAlias:string):TkbmSQLCustomNode; override;
   public
      constructor Create(AOwner:TObject); override;
      destructor Destroy; override;

      procedure FixupSelectionWildcardNodes; virtual;
      procedure FixupRefSourceFields; override;

      procedure Prepare; override;
      procedure Optimize; override;
      procedure Execute; override;

      property Selection:TkbmSQLNodes read FSelection;
      property Aggregates:TkbmSQLNodes read FAggregates;
      property Tables:TkbmSQLTables read FTables;
      property Condition:TkbmSQLCustomNode read FCondition write FCondition;
      property Order:TkbmSQLNodes read FOrder;
      property Group:TkbmSQLNodes read FGroup;
      property HavingCondition:TkbmSQLCustomNode read FHavingCondition write FHavingCondition;
      property Limit:TkbmSQLCustomNode read FLimit write FLimit;
      property Offset:TkbmSQLCustomNode read FOffset write FOffset;
      property ResultTable:TkbmCustomMemTable read FResultTable write FResultTable;
      property IsAggregate:boolean read GetIsAggregate;
      property IsDistinctAggregate:boolean read GetIsDistinctAggregate;
      property Distinct:boolean read FDistinct write FDistinct;
   end;

   TkbmSQLSelectOperation = class(TkbmSQLCustomSelectOperation);

   TkbmSQLUpdateOperation = class(TkbmSQLCustomOperation)
   private
      FTable:TkbmSQLTable;
      FFields:TkbmSQLFieldNodes;
      FValues:TkbmSQLNodes;
      FCondition:TkbmSQLCustomNode;
   public
      constructor Create(AOwner:TObject); override;
      destructor Destroy; override;

      procedure Prepare; override;
      procedure Optimize; override;
      procedure Execute; override;

      property Table:TkbmSQLTable read FTable write FTable;
      property Fields:TkbmSQLFieldNodes read FFields;
      property Values:TkbmSQLNodes read FValues;
      property Condition:TkbmSQLCustomNode read FCondition write FCondition;
   end;

   TkbmSQLDeleteOperation = class(TkbmSQLCustomOperation)
   private
      FTable:TkbmSQLTable;
      FCondition:TkbmSQLCustomNode;
   public
      constructor Create(AOwner:TObject); override;
      destructor Destroy; override;

      procedure Prepare; override;
      procedure Optimize; override;
      procedure Execute; override;

      property Table:TkbmSQLTable read FTable write FTable;
      property Condition:TkbmSQLCustomNode read FCondition write FCondition;
   end;

   TkbmSQLInsertOperation = class(TkbmSQLCustomOperation)
   private
      FTable:TkbmSQLTable;
      FFields:TkbmSQLFieldNodes;
      FValues:TkbmSQLNodes;
   public
      constructor Create(AOwner:TObject); override;
      destructor Destroy; override;

      procedure Prepare; override;
      procedure Optimize; override;
      procedure Execute; override;

      property Table:TkbmSQLTable read FTable write FTable;
      property Fields:TkbmSQLFieldNodes read FFields;
      property Values:TkbmSQLNodes read FValues;
   end;

   TkbmSQLEvaluationOperation = class(TkbmSQLCustomOperation)
   private
      FEvaluation:TkbmSQLCustomNode;
      FFields:TkbmSQLFieldNodes;
   public
      constructor Create(AOwner:TObject); override;
      destructor Destroy; override;

      function GetErrorColOffset:integer; override;

      procedure Prepare; override;
      procedure Optimize; override;
      procedure Execute; override;
      function Evaluate:variant; virtual;

      property Fields:TkbmSQLFieldNodes read FFields;
      property Evaluation:TkbmSQLCustomNode read FEvaluation write FEvaluation;
   end;

   function kbmSQLMergeDataTypes(ALeft:TFieldType; ARight:TFieldType):TFieldType;
   function kbmSQLStrToFloat(AString:string):double;

implementation

uses Math,
{$IFNDEF FPC}
     Masks,
{$ELSE}
     StrUtils,
{$ENDIF}
     Variants,
     kbmMemTypes,
     kbmMemSQL,
     kbmSQLDBAPI,
     kbmSQLFuncAPI;

function kbmSQLMergeDataTypes(ALeft:TFieldType; ARight:TFieldType):TFieldType;
 const
     DTOrder : array [TFieldType] of integer = (
       999, // ftUnknown,
       210, // ftString,
       20,  // ftSmallint,
       25,  // ftInteger,
       22,  // ftWord,
       10,  // ftBoolean,
       100, // ftFloat,
       105, // ftCurrency,
       110, // ftBCD,
       300, // ftDate,
       310, // ftTime,
       320, // ftDateTime,
       420, // ftBytes,
       400, // ftVarBytes,
       26,  // ftAutoInc,
       499, // ftBlob,
       160, // ftMemo,
       490, // ftGraphic,
       170, // ftFmtMemo,
       491, // ftParadoxOle,
       -1,  // ftDBaseOle,
       493, // ftTypedBinary,
       -1,  // ftCursor,
       208, // ftFixedChar,
       220, // ftWideString,
       27,  // ftLargeint,
       -1,  // ftADT,
       -1,  // ftArray,
       -1,  // ftReference,
       -1,  // ftDataSet,
       -1,  // ftOraBlob,
       299, // ftOraClob,
       -1,  // ftVariant,
       -1,  // ftInterface,
       -1,  // ftIDispatch,
       205, // ftGuid,
       315, // ftTimeStamp,
       115  // ftFMTBcd
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
       ,209 // ftFixedWideChar,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
       ,161 // ftWideMemo,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
       ,-1  // ftOraTimeStamp,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
       ,-1  // ftOraInterval
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
       ,23  // ftLongWord
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
       ,26  // ftShortint
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
       ,11  // ftByte
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
       ,110 // ftExtended
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_CONNECTION}
       ,-1  // ftConnection
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_PARAMS}
       ,-1  // ftParams
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_STREAM}
       ,-1  // ftStream
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_TIMESTAMPOFFSET}
       ,-1  // ftTimeStampOffset
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_OBJECT}
       ,-1  // ftObject
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
       ,90  // ftSingle
{$ENDIF}
     );
var
   dtl,dtr:integer;
begin
     if ALeft=ARight then
     begin
          Result:=ALeft;
          exit;
     end;

     dtl:=DTOrder[ALeft];
     dtr:=DTOrder[ARight];
     if dtl<dtr then
        Result:=ARight
     else
         Result:=ALeft;
end;

function kbmSQLStrToFloat(AString:string):double;
{$IFDEF LEVEL9}
var
   fmt:TFormatSettings;
begin
   fmt.DecimalSeparator:='.';
   Result:=StrToFloat(AString,fmt);
{$ELSE}
var
   i:integer;
   c:char;
begin
     for i:=1 to length(AString) do
     begin
          c:=AString[i];
          if (c=',') or (c='.') then
          begin
               AString[i]:=DecimalSeperator;
               break;
          end;
     end;
     Result:=StrToFloat(AString);
{$ENDIF}
end;

// ------------------------

constructor TkbmSQLCustomNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create;
     FOperation:=AOperation;
     FID:=FOperation.GetNextID;
     FDestinationField:=nil;
     FSourceField:=nil;
     FNodes:=nil;
     FParent:=nil;
     FExecuted:=false;
end;

function TkbmSQLCustomNode.GetUniqueName:string;
begin
     if FAlias<>'' then
        Result:=FAlias
     else if FUniqueName<>'' then
        Result:=FUniqueName
     else
         Result:='F'+inttostr(FID);
end;

function TkbmSQLCustomNode.GetNonScalar:boolean;
begin
     Result:=FNonScalar;
end;

function TkbmSQLCustomNode.GetExpectedWidth:integer;
begin
     case GetDataType of
          ftFloat,
          ftCurrency,
          ftBCD: Result:=15; 
          ftLargeInt: Result:=15;
          ftInteger: Result:=10; 
          ftSmallint: Result:=5; 
          ftWord: Result:=5; 
          ftBoolean: Result:=5; 
          ftDate: Result:=10; 
          ftTime: Result:=8; 
          ftDateTime: Result:=19; 
     else
          Result:=GetWidth;
     end;
end;

function TkbmSQLCustomNode.GetWidth:integer;
begin
     Result:=0;
end;

function TkbmSQLCustomNode.GetDataType:TFieldType;
begin
     Result:=FDataType;
end;

// ------------------------

constructor TkbmSQLNodes.Create(const AOwnsNodes:boolean);
begin
     inherited Create;
{$IFDEF NEXTGEN}
     FList:=TList<TkbmSQLCustomNode>.Create;
{$ELSE}
     FList:=TList.Create;
{$ENDIF}
     FOwnsNodes:=AOwnsNodes;
     FParent:=nil;
end;

destructor TkbmSQLNodes.Destroy;
begin
     Clear;
     FList.Free;
     FList:=nil;
     inherited Destroy;
end;

function TkbmSQLNodes.GetByUniqueName(AUniqueName:string):TkbmSQLCustomNode;
var
   i:integer;
begin
     AUniqueName:=UpperCase(AUniqueName);
     for i:=0 to Count-1 do
     begin
          Result:=TkbmSQLCustomNode(FList.Items[i]);
          if UpperCase(Result.GetUniqueName)=AUniqueName then
             exit;
     end;
     Result:=nil;
end;

function TkbmSQLNodes.GetByAlias(AAlias:string):TkbmSQLCustomNode;
var
   i:integer;
begin
     AAlias:=UpperCase(AAlias);
     for i:=0 to Count-1 do
     begin
          Result:=TkbmSQLCustomNode(FList.Items[i]);
          if UpperCase(Result.Alias)=AAlias then
             exit;
     end;
     Result:=nil;
end;

function TkbmSQLNodes.GetByFieldName(const AName:string):TkbmSQLFieldNode;
var
   n:TkbmSQLCustomNode;
   i,j:integer;
begin
     j:=FList.Count-1;
     for i:=0 to j do
     begin
          n:=FList[i];
          if n is TkbmSQLFieldNode then
          begin
               Result:=TkbmSQLFieldNode(n);
               if Result.FieldName=AName then
                  exit;
          end;
     end;
     Result:=nil;
end;

function TkbmSQLNodes.IndexOf(const ANode:TkbmSQLCustomNode):integer;
begin
     Result:=FList.IndexOf(ANode);
end;

procedure TkbmSQLNodes.Add(const ANode:TkbmSQLCustomNode; const AIgnoreIfExists:boolean = false);
begin
     if (not AIgnoreIfExists) or (GetByUniqueName(ANode.GetUniqueName)=nil) then
     begin
          if FOwnsNodes then
          begin
               ANode.Parent:=self;
               ANode.Nodes:=self;
          end;
          FList.Add(ANode);
     end;
end;

procedure TkbmSQLNodes.Add(const ANodes:TkbmSQLNodes; const AIgnoreIfExists:boolean = false);
var
   i:integer;
begin
     for i:=0 to ANodes.Count-1 do
         Add(ANodes.Node[i],AIgnoreIfExists);
end;

procedure TkbmSQLNodes.Insert(const APos:integer; const ANode:TkbmSQLCustomNode);
begin
     FList.Insert(APos,ANode);
     if FOwnsNodes then
     begin
          ANode.Parent:=self;
          ANode.Nodes:=self;
     end;
end;

procedure TkbmSQLNodes.Delete(const APos:integer);
begin
     FList.Delete(APos);
end;

procedure TkbmSQLNodes.Delete(const ANode:TkbmSQLCustomNode);
var
   i:integer;
begin
     i:=FList.IndexOf(ANode);
     if i>=0 then
        FList.Delete(i);
end;

procedure TkbmSQLNodes.Replace(const AOldNode,ANewNode:TkbmSQLCustomNode; const AFreeOldNode:boolean = false);
var
   i:integer;
begin
     i:=FList.IndexOf(AOldNode);
     if i>=0 then
     begin
{$IFNDEF NEXTGEN}
          if AFreeOldNode then
             AOldNode.Free;
{$ENDIF}
          FList.Items[i]:=ANewNode;
     end;
end;

function TkbmSQLNodes.Count:integer;
begin
     Result:=FList.Count;
end;

function TkbmSQLNodes.GetNode(const AIndex:integer):TkbmSQLCustomNode;
begin
     Result:=TkbmSQLCustomNode(FList.Items[AIndex]);
end;

procedure TkbmSQLNodes.SetNode(const AIndex:integer; const ANode:TkbmSQLCustomNode);
begin
     FList.Items[AIndex]:=ANode;
     if FOwnsNodes then
     begin
          ANode.Parent:=self;
          ANode.Nodes:=self;
     end;
end;

procedure TkbmSQLNodes.ClearExecuted;
var
   i:integer;
begin
     for i:=0 to FList.Count-1 do
         TkbmSQLCustomNode(FList.Items[i]).FExecuted:=false;
end;

procedure TkbmSQLNodes.Clear;
{$IFNDEF NEXTGEN}
var
   i:integer;
{$ENDIF}
begin
{$IFNDEF NEXTGEN}
     if FOwnsNodes then
        for i:=0 to FList.Count-1 do
            TkbmSQLCustomNode(FList.Items[i]).Free;
{$ENDIF}
     FList.Clear;
end;

// ------------------------

procedure TkbmSQLFieldNodes.SetFieldNode(AIndex:integer; ANode:TkbmSQLFieldNode);
begin
     FList[AIndex]:=ANode;
end;

function TkbmSQLFieldNodes.GetFieldNode(AIndex:integer):TkbmSQLFieldNode;
var
   n:TkbmSQLCustomNode;
begin
     n:=FList[AIndex];
     if n is TkbmSQLFieldNode then
        Result:=TkbmSQLFieldNode(n)
     else
         Result:=nil;
end;

// ------------------------

constructor TkbmSQLBinaryNode.Create(AOperation:TkbmSQLCustomOperation; AOperator:TkbmSQLBinaryNodeOperator);
begin
     inherited Create(AOperation);
     FOperator:=AOperator;
     FLeftNode:=nil;
     FRightNode:=nil;
end;

destructor TkbmSQLBinaryNode.Destroy;
begin
     if FLeftNode<>nil then
        FLeftNode.Free;
     FLeftNode:=nil;

     if FRightNode<>nil then
        FRightNode.Free;
     FRightNode:=nil;

     inherited Destroy;
end;

function TkbmSQLBinaryNode.GetWidth:integer;
begin
     if (FOperator=ebAdd) and (FLeftNode.DataType in [ftString,ftWideString,ftFixedChar]) then
        Result:=FLeftNode.GetExpectedWidth+FRightNode.GetExpectedWidth
     else if (FOperator=ebConcat) then
        Result:=FLeftNode.GetExpectedWidth+FRightNode.GetExpectedWidth
     else
         Result:=Max(FLeftNode.GetExpectedWidth,FRightNode.GetExpectedWidth);
end;

function TkbmSQLBinaryNode.GetDataType:TFieldType;
begin
     if not Assigned(FLeftNode) then
     begin
          Result:=ftUnknown;
          exit;
     end;
     if not Assigned(FRightNode) then
     begin
          Result:=ftUnknown;
          exit;
     end;

     case FOperator of
         ebDiv: Result:=ftFloat;
         ebAdd,
         ebSub,
         ebMul,
         ebAnd,
         ebOr,
         ebXor,
         ebMod,
         ebIDiv: Result:=kbmSQLMergeDataTypes(FLeftNode.DataType,FRightNode.DataType);
         ebEqual,
         ebNotEqual,
         ebLess,
         ebGreater,
         ebLessEqual,
         ebGreaterEqual: Result:=ftBoolean;
         ebConcat: Result:=ftString;
     else
         raise Exception.Create('Unsupported binary operator');
     end;
end;

function TkbmSQLBinaryNode.Execute:variant;
var
   vLeft,vRight:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     if not Assigned(FLeftNode) then
        raise Exception.Create('Left operand not assigned in binary Node');
     if not Assigned(FRightNode) then
        raise Exception.Create('Right operand not assigned in binary Node');

     // Handle comparison with NULL (IS NULL or IS NOT NULL) specially
     if FRightNode is TkbmSQLValueNode then
     begin
          Result:=false;
          if TkbmSQLValueNode(FRightNode).ValueType=evtNull then
          begin
               vLeft:=FLeftNode.Execute;
               case FOperator of
                    ebEqual:        Result:=VarIsNull(vLeft);
                    ebNotEqual:     Result:=not VarIsNull(vLeft);
               end;
          end;
          exit;
     end;

     // In remaining, all null results in failed comparison.
     vLeft:=FLeftNode.Execute;
     if VarIsNull(vLeft) then
     begin
          Result:=Null;
          exit;
     end;
     vRight:=FRightNode.Execute;
     if VarIsNull(vRight) then
     begin
          Result:=Null;
          exit;
     end;

     // Check if to convert to string to allow 5+"a" -> "5a".
     if (VarIsStr(vLeft) or VarIsStr(vRight)) then
     begin
           vLeft:=VarAsType(vLeft,varString);
           vRight:=VarAsType(vRight,varString);
     end;

     // Got non null values, compare.
     case FOperator of
         ebAdd:          Result:=vLeft + vRight;
         ebSub:          Result:=vLeft - vRight;
         ebMul:          Result:=vLeft * vRight;
         ebDiv:          Result:=vLeft / vRight;
         ebAnd:
                         if VarType(vLeft)=vtBoolean then
                            Result:=boolean(vLeft) AND boolean(vRight)
                         else
                             Result:=integer(vLeft) AND integer(vRight);
         ebMod:          Result:=vLeft MOD vRight;
         ebIDiv:         Result:=vLeft DIV vRight;
         ebOr:
                         if VarType(vLeft)=vtBoolean then
                            Result:=boolean(vLeft) OR boolean(vRight)
                         else
                             Result:=integer(vLeft) OR integer(vRight);
         ebXor:
                         if VarType(vLeft)=vtBoolean then
                            Result:=boolean(vLeft) XOR boolean(vRight)
                         else
                             Result:=integer(vLeft) XOR integer(vRight);
         ebEqual:        Result:=vLeft = vRight;
         ebNotEqual:     Result:=vLeft <> vRight;
         ebLess:         Result:=vLeft < vRight;
         ebGreater:      Result:=vLeft > vRight;
         ebLessEqual:    Result:=vLeft <= vRight;
         ebGreaterEqual: Result:=vLeft >= vRight;
         ebConcat:       Result:=String(vLeft)+String(vRight);
         ebLike:
{$IFDEF FPC}
                         Result:=IsWild(vLeft,vRight,false);
{$ELSE}
                         Result:=MatchesMask(vLeft,vRight);
{$ENDIF}
     else
         raise Exception.Create('Unsupported binary operator');
     end;
end;

// ------------------------

constructor TkbmSQLUnaryNode.Create(AOperation:TkbmSQLCustomOperation; AOperator:TkbmSQLUnaryNodeOperator);
begin
     inherited Create(AOperation);
     FOperator:=AOperator;
     FRightNode:=nil;
end;

destructor TkbmSQLUnaryNode.Destroy;
begin
     if FRightNode<>nil then
        FRightNode.Free;
     FRightNode:=nil;
     inherited Destroy;
end;

function TkbmSQLUnaryNode.GetWidth:integer;
begin
     Result:=FRightNode.Width;
end;

function TkbmSQLUnaryNode.GetDataType:TFieldType;
begin
     if not Assigned(FRightNode) then
     begin
          Result:=ftUnknown;
          exit;
     end;

     case FOperator of
         euNot:    Result:=ftBoolean;
         euNegate: Result:=FRightNode.DataType;
     else
         raise Exception.Create('Unsupported unary operator');
     end;
end;

function TkbmSQLUnaryNode.Execute:variant;
var
   v:variant;
   b:boolean;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     if not Assigned(FRightNode) then
        raise Exception.Create('Right operand not assigned in unary Node');

     v:=FRightNode.Execute;
     if VarIsNull(v) then
     begin
          Result:=Null;
          exit;
     end;

     case FOperator of
         euNot:
            begin
                 b:=v;
                 Result:=NOT b;
            end;
         euNegate: Result:=-v;
     else
         raise Exception.Create('Unsupported unary operator');
     end;
end;

// ------------------------

constructor TkbmSQLBetweenNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create(AOperation);
     FRangeLow:=nil;
     FRangeHigh:=nil;
end;

destructor TkbmSQLBetweenNode.Destroy;
begin
     if FLeftNode<>nil then
        FLeftNode.Free;
     FLeftNode:=nil;
     if FRangeLow<>nil then
        FRangeLow.Free;
     FRangeLow:=nil;
     if FRangeHigh<>nil then
        FRangeHigh.Free;
     FRangeHigh:=nil;
     inherited Destroy;
end;

function TkbmSQLBetweenNode.GetDataType:TFieldType;
begin
     Result:=ftBoolean;
end;

function TkbmSQLBetweenNode.Execute:variant;
var
   v,vLow,vHigh:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     v:=FLeftNode.Execute;
     if VarIsNull(v) then
     begin
          Result:=Null;
          exit;
     end;

     vLow:=FRangeLow.Execute;
     if VarIsNull(vLow) then
     begin
          Result:=Null;
          exit;
     end;

     vHigh:=FRangeHigh.Execute;
     if VarIsNull(vHigh) then
     begin
          Result:=Null;
          exit;
     end;

     Result:=(v>=vLow) and (v<=vHigh);
end;

// ------------------------

constructor TkbmSQLInNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create(AOperation);
     FLeftNode:=nil;
     FNodes:=TkbmSQLNodes.Create(true);
     FNodes.Parent:=self;
end;

destructor TkbmSQLInNode.Destroy;
begin
     if FLeftNode<>nil then
        FLeftNode.Free;
     FLeftNode:=nil;
     FNodes.Free;
     inherited Destroy;
end;

function TkbmSQLInNode.GetDataType:TFieldType;
begin
     Result:=ftBoolean;
end;

function TkbmSQLInNode.Execute:variant;
var
   i:integer;
   v:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     v:=LeftNode.Execute;
     if VarIsNull(v) then
     begin
          Result:=Null;
          exit;
     end;

     for i:=0 to FNodes.Count-1 do
     begin
          if v=FNodes[i].Execute then
          begin
               Result:=true;
               exit;
          end;
     end;
     Result:=false;
end;

// ------------------------

constructor TkbmSQLFieldNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create(AOperation);
end;

destructor TkbmSQLFieldNode.Destroy;
begin
     inherited Destroy;
end;

function TkbmSQLFieldNode.GetWidth:integer;
begin
     if FOriginatingField<>nil then
        Result:=FOriginatingField.Size
     else
        Result:=0;
end;

function TkbmSQLFieldNode.GetDataType:TFieldType;
var
   s:string;
begin
     if FFieldType in [ntRowID,ntRecNo] then
        Result:=ftLargeint
     else if FOriginatingField=nil then
     begin
          if Tablename<>'' then
             s:='('+TableName+')'
          else if FTable<>nil then
             s:='('+FTable.Name+')'
          else
              s:='';
          raise Exception.Create('Field '+FieldName+' not found in table '+s);
     end
     else
         Result:=FOriginatingField.DataType;
end;

function TkbmSQLFieldNode.Execute:variant;
var
   s:string;
begin
     FExecuted:=true;
     case FieldType of
        ntField:
          begin
               if FSourceField<>nil then
               begin
                    Result:=FSourceField.Value;
                    exit;
               end;

               if FOriginatingField=nil then
               begin
                    if TableName<>'' then
                       s:=TableName
                    else if FTable<>nil then
                       s:=FTable.Name
                    else
                        s:='<Unknown>';
                    raise Exception.Create('Field '+FieldName+' not found in table ('+s+')');
               end
               else
                   Result:=FOriginatingField.AsVariant;
          end;

        ntRowID:
          begin
               if FTable=nil then
                  raise Exception.Create('Table undefined');
               Result:=FTable.GetCurrentRowID;
          end;

        ntRecNo:
          begin
               if FTable=nil then
                  raise Exception.Create('Table undefined');
               Result:=FTable.GetCurrentRecNo;
          end;
     end;

end;

// ------------------------

constructor TkbmSQLVariableNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create(AOperation);
end;

destructor TkbmSQLVariableNode.Destroy;
begin
     inherited Destroy;
end;

function TkbmSQLVariableNode.GetWidth:integer;
var
   ft:TFieldType;
begin
     if Assigned(Operation.OnGetVariableMetaData) then
        if Operation.OnGetVariableMetaData(self,VariableName,Result,ft) then
           exit;
     Result:=0;
end;

function TkbmSQLVariableNode.GetDataType:TFieldType;
var
   w:integer;
begin
     if Assigned(Operation.OnGetVariableMetaData) then
        if Operation.OnGetVariableMetaData(self,VariableName,w,Result) then
           exit;
     Result:=ftUnknown;
end;

function TkbmSQLVariableNode.IsVariableValid:boolean;
var
   w:integer;
   dt:TFieldType;
begin
     if Assigned(Operation.OnGetVariableMetaData) then
        Result:=Operation.OnGetVariableMetaData(self,VariableName,w,dt)
     else
         Result:=true; // Assume valid for now.
end;

function TkbmSQLVariableNode.Execute:variant;
begin
     FExecuted:=true;
     if Assigned(Operation.OnGetVariableValue) then
        if Operation.OnGetVariableValue(self,VariableName,Result) then
           exit;
     raise Exception.Create('Variable '+VariableName+' not available.');
end;

// ------------------------

constructor TkbmSQLAggregateNode.Create(AOperation:TkbmSQLCustomOperation);
begin
     inherited Create(AOperation);
     FExpressionNode:=nil;
     FAggrFunction:=safUNKNOWN;
end;

destructor TkbmSQLAggregateNode.Destroy;
begin
     if FExpressionNode<>nil then
        FExpressionNode.Free;
     FExpressionNode:=nil;
     inherited Destroy;
end;

function TkbmSQLAggregateNode.GetWidth:integer;
begin
     Result:=FExpressionNode.GetWidth;
end;

function TkbmSQLAggregateNode.GetDataType:TFieldType;
begin
     if FAggrFunction=safCOUNT then
        Result:=ftInteger
     else if FAggrFunction=safAVG then
        Result:=ftFloat
     else if FAggrFunction=safSTDDEV then
        Result:=ftFloat
     else if (FExpressionNode<>nil) then
        Result:=FExpressionNode.DataType
     else
         Result:=ftUnknown;
end;

function TkbmSQLAggregateNode.GetUniqueName:string;
var
   s:string;
begin
     if Alias<>'' then
        Result:=Alias
     else
     begin
          if (FExpressionNode<>nil) then
             s:=FExpressionNode.GetUniqueName
          else
              s:=inherited GetUniqueName;

          Result:=TkbmCustomMemTable.GetAggregateFieldName(s,FieldOptions);
     end;

     // Check if this is an intermediate value, generate a temporary name.
     if Parent is TkbmSQLNodes then
     begin
          if TkbmSQLNodes(Parent).Parent<>nil then
             Result:='_'+Result;
     end
     else if Parent is TkbmSQLCustomNode then
        Result:='_'+Result;
end;

function TkbmSQLAggregateNode.GetFieldOptions:TkbmifoOptions;
begin
     case FAggrFunction of
          safCOUNT:    Result:=[mtifoAggregate,mtifoAggCount];
          safMAX:      Result:=[mtifoAggregate,mtifoAggMax];
          safMIN:      Result:=[mtifoAggregate,mtifoAggMin];
          safAVG:      Result:=[mtifoAggregate,mtifoAggAvg];
          safSUM:      Result:=[mtifoAggregate,mtifoAggSum];
          safSTDDEV:   Result:=[mtifoAggregate,mtifoAggStdDev];
     else
         Result:=[];
     end;
end;

function TkbmSQLAggregateNode.GetFieldModifier:string;
begin
     case FAggrFunction of
          safCOUNT:    Result:='COUNT';
          safMAX:      Result:='MAX';
          safMIN:      Result:='MIN';
          safAVG:      Result:='AVG';
          safSUM:      Result:='SUM';
          safSTDDEV:   Result:='STDDEV';
     else
         Result:='';
     end;
end;

function TkbmSQLAggregateNode.Execute:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
        Result:=FSourceField.Value
     else if FAggrFunction=safCOUNT then
          Result:=1
     else
         Result:=FExpressionNode.Execute;
end;

// ------------------------

procedure TkbmSQLCustomValueNode.SetValue(AValue:variant);
begin
     raise Exception.Create('Setting value not supported.');
end;

function TkbmSQLCustomValueNode.Execute:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     Result:=GetValue;
end;

// ------------------------

constructor TkbmSQLValueNode.Create(AOperation:TkbmSQLCustomOperation; AValueType:TkbmSQLValueType);
begin
     inherited Create(AOperation);
     FValueType:=AValueType;
end;

function TkbmSQLValueNode.GetWidth:integer;
begin
     case FValueType of
         evtTimeStamp: Result:=20;
     else
         Result:=0;
     end;
end;

function TkbmSQLValueNode.GetDataType:TFieldType;
begin
     case FValueType of
         evtTimeStamp: Result:=ftDateTime;
     else
         Result:=ftUnknown;
     end;
end;

function TkbmSQLValueNode.GetValue:variant;
begin
     case FValueType of
         evtTimeStamp: Result:=Now;
         evtNull:      Result:=Null;
     end;
end;

// ------------------------

constructor TkbmSQLConstNode.Create(AOperation:TkbmSQLCustomOperation; AValue:variant);
begin
     inherited Create(AOperation);
     Value:=AValue;
end;

function TkbmSQLConstNode.GetWidth:integer;
begin
     case VarType(FValue) of
{$IFDEF LEVEL13}
       varUString,
{$ENDIF}
       varString,
       varOleStr  : Result:=length(FValue);
     else
       Result:=0;
     end;
end;

function TkbmSQLConstNode.GetDataType:TFieldType;
begin
     case VarType(FValue) of
{$ifdef LEVEL13}      // Delphi 2009 and up
       varUString,
{$endif}
       varString: Result:=ftString;
       varDouble,
       varCurrency,
       varSingle: Result:=ftFloat;
       varByte,
       varShortInt,
       varWord,
       varLongWord,
       varSmallint,
       varInteger: Result:=ftInteger;
       varBoolean: Result:=ftBoolean;
     else
       Result:=ftUnknown;
     end;
end;

function TkbmSQLConstNode.GetValue:variant;
begin
     Result:=FValue;
end;

procedure TkbmSQLConstNode.SetValue(AValue:variant);
begin
     FValue:=AValue;
end;

// ------------------------

constructor TkbmSQLFunctionNode.Create(AOperation:TkbmSQLCustomOperation; AFunctionName:string; AArgs:TkbmSQLNodes);
var
   reg:TkbmSQLFunctionRegistration;
   fct:TkbmSQLCustomFunction;
   sGrp:string;
begin
     inherited Create(AOperation);
     FFunctionName:=AFunctionName;
     reg:=kbmSQLFunctionRegistrations.GetFunctionReg(AFunctionName);
     if reg=nil then
     begin
          fct:=nil;
          sGrp:='';
     end
     else
     begin
          fct:=reg._Function;
          sGrp:=reg.Group;
     end;
     if Assigned(FOperation.FOnGetFunction) then
     begin
          if not FOperation.FOnGetFunction(self,AFunctionName,sGrp,fct) then
             fct:=nil;
     end;
     if (reg<>nil) and (not reg.Enabled) then
        fct:=nil;

     FFunction:=@fct;
     FArgs:=AArgs;
     AArgs.Parent:=self;
end;

destructor TkbmSQLFunctionNode.Destroy;
begin
     if FArgs<>nil then
        FArgs.Free;
     FArgs:=nil;
     inherited Destroy;
end;

function TkbmSQLFunctionNode.GetWidth:integer;
var
   v:variant;
begin
     if FFunction=nil then
        raise Exception.Create('Undefined function: '+FunctionName);
     if not TkbmSQLCustomFunction(FFunction)(FOperation,fsWidth,FArgs,v) then
        raise Exception.Create('Custom function failed: '+FunctionName);
     Result:=v;
end;

function TkbmSQLFunctionNode.GetDataType:TFieldType;
var
   v:variant;
begin
     if FFunction=nil then
     begin
          Result:=ftUnknown;
          exit;
     end;
     if not TkbmSQLCustomFunction(FFunction)(FOperation,fsDataType,FArgs,v) then
     begin
          Result:=ftUnknown;
          exit;
     end;
     Result:=v;
end;

function TkbmSQLFunctionNode.IsFunctionValid:boolean;
begin
     if FFunction=nil then
        Result:=false
     else
         Result:=true;
end;

function TkbmSQLFunctionNode.Execute:variant;
begin
     FExecuted:=true;
     if FSourceField<>nil then
     begin
          Result:=FSourceField.Value;
          exit;
     end;

     if FFunction=nil then
        raise Exception.Create('Undefined function: '+FunctionName);
     if not TkbmSQLCustomFunction(FFunction)(FOperation,fsExecute,FArgs,Result) then
        raise Exception.Create('Custom function failed: '+FunctionName);
end;

// -------------------------------

constructor TkbmSQLTable.Create;
begin
     inherited Create;
     FAlias:='';
     FName:='';
     FAPI:=nil;
     FAffectedRows:=0;
end;

procedure TkbmSQLTable.SetName(AName:string);
begin
     FName:=AName;
end;

procedure TkbmSQLTable.SetAlias(AAlias:string);
begin
     FAlias:=AAlias;
end;

function TkbmSQLTable.GetAliasOrName:string;
begin
     if FAlias<>'' then
        Result:=FAlias
     else
         Result:=FName;
end;

function TkbmSQLTable.GetCurrentRowID:int64;
begin
     if FDataSet=nil then
        raise Exception.Create('Dataset undefined');
     Result:=PkbmRecord(FDataSet.ActiveBuffer).UniqueRecordID;
end;

function TkbmSQLTable.GetCurrentRecNo:integer;
begin
     if FDataSet=nil then
        raise Exception.Create('Dataset undefined');
     Result:=PkbmRecord(FDataSet.ActiveBuffer).RecordNo;
end;

// ------------------------

constructor TkbmSQLTables.Create(const AOwnTables:boolean);
begin
     inherited Create;
{$IFDEF NEXTGEN}
     FList:=TList<TkbmSQLTable>.Create;
{$ELSE}
     FList:=TList.Create;
     FOwnsTables:=AOwnTables;
{$ENDIF}
end;

destructor TkbmSQLTables.Destroy;
begin
     Clear;
     FList.Free;
     inherited Destroy;
end;

procedure TkbmSQLTables.Clear;
{$IFNDEF NEXTGEN}
var
   i:integer;
{$ENDIF}
begin
{$IFNDEF NEXTGEN}
     if FOwnsTables then
        for i:=0 to FList.Count-1 do
            TkbmSQLTable(FList.items[i]).Free;
{$ENDIF}
     FList.Clear;
end;

function TkbmSQLTables.GetTable(AIndex:integer):TkbmSQLTable;
begin
     Result:=TkbmSQLTable(FList.items[AIndex]);
end;

function TkbmSQLTables.GetTableByName(AName:string):TkbmSQLTable;
var
   i:integer;
   t:TkbmSQLTable;
begin
     AName:=UpperCase(AName);
     for i:=0 to FList.Count-1 do
     begin
          t:=TkbmSQLTable(FList.items[i]);
          if UpperCase(t.Name)=AName then
          begin
               Result:=t;
               exit;
          end;
     end;
     Result:=nil;
end;

function TkbmSQLTables.GetTableByAlias(AAlias:string):TkbmSQLTable;
var
   i:integer;
   t:TkbmSQLTable;
begin
     AAlias:=UpperCase(AAlias);
     for i:=0 to FList.Count-1 do
     begin
          t:=TkbmSQLTable(FList.items[i]);
          if UpperCase(t.Alias)=AAlias then
          begin
               Result:=t;
               exit;
          end;
     end;
     Result:=nil;
end;

procedure TkbmSQLTables.Add(ATable:TkbmSQLTable);
begin
     FList.Add(ATable);
end;

procedure TkbmSQLTables.AddUnique(ATable:TkbmSQLTable);
begin
     if ATable.Alias<>'' then
     begin
          if GetTableByAlias(ATable.Alias)<>nil then
             raise Exception.Create('Table alias '''+ATable.Alias+''' already used.');
     end
     else if GetTableByName(ATable.Name)<>nil then
         raise Exception.Create('Table '''+ATable.Name+''' already referenced.');
     FList.Add(ATable);
end;

procedure TkbmSQLTables.Add(AName:string; ADataset:TkbmCustomMemTable);
var
   tbl:TkbmSQLTable;
begin
     tbl:=TkbmSQLTable.Create;
     tbl.Name:=AName;
     tbl.Dataset:=ADataset;
     FList.Add(tbl);
end;

procedure TkbmSQLTables.Delete(ATable:TkbmSQLTable);
var
   i:integer;
begin
     i:=FList.IndexOf(ATable);
     if i>=0 then
        FList.Delete(i);
end;

procedure TkbmSQLTables.Delete(AName:string);
var
   i:integer;
   tbl:TkbmSQLTable;
begin
     AName:=UpperCase(AName);
     for i:=0 to FList.Count-1 do
     begin
          tbl:=TkbmSQLTable(FList.Items[i]);
          if UpperCase(tbl.Name)=AName then
          begin
               FList.Delete(i);
               tbl.Free;
               exit;
          end;
     end;
end;

function TkbmSQLTables.GetTablesContainingFieldName(AFieldName:string):TkbmSQLTables;
var
   i:integer;
   tbl:TkbmSQLTable;
begin
     AFieldName:=UpperCase(AFieldName);
     Result:=TkbmSQLTables.Create(false);
     for i:=0 to FList.Count-1 do
     begin
          tbl:=TkbmSQLTable(FList.Items[i]);
          if tbl.Dataset.FindField(AFieldName)<>nil then
             Result.Add(tbl);
     end;
end;

function TkbmSQLTables.GetCount:integer;
begin
     Result:=FList.Count;
end;

// -------------------------------

constructor TkbmSQLCustomOperation.Create(AOwner:TObject);
begin
     inherited Create;
     FOwner:=AOwner;
     FRefSourceFields:=TkbmSQLFieldNodes.Create(false);
     FRefSourceTables:=TkbmSQLTables.Create(false);
     FRefSearchFields:=TkbmSQLFieldNodes.Create(false);
     FRefHavingFields:=TkbmSQLFieldNodes.Create(false);
     FRefWildcards:=TkbmSQLFieldNodes.Create(false);
     FParseState:=sopsDefault;
     FOnGetVariableValue:=nil;
     FOnGetVariableMetaData:=nil;
     FOnGetFunction:=nil;
     FContext:=nil;
     FFlags:=[];
end;

destructor TkbmSQLCustomOperation.Destroy;
begin
     FRefWildcards.Free;
     FRefSearchFields.Free;
     FRefHavingFields.Free;
     FRefSourceFields.Free;
     FRefSourceTables.Free;
     inherited Destroy;
end;

function TkbmSQLCustomOperation.GetFieldByAlias(const AAlias:string):TkbmSQLCustomNode;
begin
     Result:=nil;
end;

function TkbmSQLCustomOperation.GetErrorColOffset:integer;
begin
     Result:=0;
end;

procedure TkbmSQLCustomOperation.FixupRefSourceFields;
var
   i:integer;
   na,n:TkbmSQLCustomNode;
   fld:TkbmSQLFieldNode;
   bPrefixTableToField:boolean;
   ts:TkbmSQLTables;
   un:string;
begin
     for i:=0 to FRefSourceFields.Count-1 do
     begin
          n:=FRefSourceFields[i];
          if not (n is TkbmSQLFieldNode) then
             continue;

          fld:=TkbmSQLFieldNode(n);

          // Figure out which table field refers to.
          bPrefixTableToField:=true;
          if fld.TableName='' then
          begin
               // Not specifically defined table. Find out.
               bPrefixTableToField:=false;

               if fld.FieldType<>ntField then
               begin
                    if RefSourceTables.Count>1 then
                       raise Exception.Create('Ambiguous wildcard/RecNo/RecID table reference.');
                    fld.Table:=RefSourceTables.Tables[0];
               end
               else
               begin
                    // Check if using alias.
                    na:=GetFieldByAlias(fld.FieldName);
                    if na is TkbmSQLFieldNode then
                    begin
                         fld.FieldName:=TkbmSQLFieldNode(na).FieldName;
                         ts:=RefSourceTables.GetTablesContainingFieldName(fld.FieldName);
                    end
                    else
                        ts:=RefSourceTables.GetTablesContainingFieldName(fld.FieldName);
                    if ts<>nil then
                       try
                          if ts.Count=1 then
                             fld.Table:=ts.Tables[0]
                          else if ts.Count>1 then
                             raise Exception.Create('Ambiguous Field name '''+fld.FieldName+'''')
                          else if not (fld.OrderBy or fld.GroupBy) then
                             raise Exception.Create('Field '''+fld.FieldName+''' not existing in any referenced table');
                       finally
                          ts.Free;
                       end;
               end;
          end
          else
          begin
               // Table specified, look it up.
               if fld.Table=nil then
                  fld.Table:=RefSourceTables.TableByName[fld.TableName];
               if fld.Table=nil then
                  fld.Table:=RefSourceTables.TableByAlias[fld.TableName];
               if fld.Table=nil then
                  raise Exception.Create('Table '''+fld.TableName+''' by field '''+fld.FieldName+''' is unavailable.');
          end;

          // Check if to pick relevant field name.
          if fld.FieldType=ntWildcard then
             fld.FieldName:=fld.Table.Dataset.Fields[0].FieldName;

          // Generate unique field names.
          if fld.Alias<>'' then
             un:=fld.Alias
          else if bPrefixTableToField then
              un:=fld.TableName+'_'+fld.FieldName
          else
              un:=fld.FieldName;

//          // Check if already exists unique name, add id.
//          if FRefSourceFields.GetByUniqueName(un)<>nil then
//             un:=un+'_'+inttostr(fld.ID);

          fld.UniqueName:=un;

          // Setup reference to actual field in table.
          if fld.Table<>nil then
             fld.OriginatingField:=fld.Table.Dataset.FindField(fld.FieldName);
     end;
end;

function TkbmSQLCustomOperation.GetNextID:integer;
begin
     Result:=FNextID;
     inc(FNextID);
end;

procedure TkbmSQLCustomOperation.Prepare;
begin
end;

procedure TkbmSQLCustomOperation.Optimize;
begin
end;

procedure TkbmSQLCustomOperation.Execute;
begin
end;

procedure TkbmSQLCustomOperation.ReplaceParentRef(const AParent:TObject; const AOldNode,ANewNode:TkbmSQLCustomNode);
begin
     if AParent is TkbmSQLNodes then
        TkbmSQLNodes(AParent).Replace(AOldNode,ANewNode,false)
     else if AParent is TkbmSQLBinaryNode then
     begin
          if TkbmSQLBinaryNode(AParent).LeftNode = AOldNode then
             TkbmSQLBinaryNode(AParent).LeftNode:=ANewNode;
          if TkbmSQLBinaryNode(AParent).RightNode = AOldNode then
             TkbmSQLBinaryNode(AParent).RightNode:=ANewNode;
     end
     else if AParent is TkbmSQLUnaryNode then
     begin
          if TkbmSQLUnaryNode(AParent).RightNode = AOldNode then
             TkbmSQLUnaryNode(AParent).RightNode:=ANewNode;
     end
     else if AParent is TkbmSQLBetweenNode then
     begin
          if TkbmSQLBetweenNode(AParent).LeftNode = AOldNode then
             TkbmSQLBetweenNode(AParent).LeftNode:=ANewNode;
          if TkbmSQLBetweenNode(AParent).RangeLow = AOldNode then
             TkbmSQLBetweenNode(AParent).RangeLow:=ANewNode;
          if TkbmSQLBetweenNode(AParent).RangeHigh = AOldNode then
             TkbmSQLBetweenNode(AParent).RangeHigh:=ANewNode;
     end
     else if AParent is TkbmSQLInNode then
     begin
          if TkbmSQLInNode(AParent).LeftNode = AOldNode then
             TkbmSQLInNode(AParent).LeftNode:=ANewNode;
          TkbmSQLInNode(AParent).Nodes.Replace(AOldNode,ANewNode,false);
     end
     else if AParent is TkbmSQLAggregateNode then
     begin
          if TkbmSQLAggregateNode(AParent).ExpressionNode = AOldNode then
             TkbmSQLAggregateNode(AParent).ExpressionNode:=ANewNode;
     end;
end;

procedure TkbmSQLCustomOperation.OptimizeNodeTree(var ANode:TkbmSQLCustomNode);
var
   i:integer;
   bnode:TkbmSQLBinaryNode;
   unode:TkbmSQLUnaryNode;
   btnode:TkbmSQLBetweenNode;
   innode:TkbmSQLInNode;
   newnode:TkbmSQLConstNode;
   fctnode:TkbmSQLFunctionNode;
   aggnode:TkbmSQLAggregateNode;
   n:TkbmSQLCustomNode;
   v:variant;
   b:boolean;
begin
     if ANode=nil then
        exit;

     // Check if binary operation of two const nodes?
     if ANode is TkbmSQLBinaryNode then
     begin
          bnode:=TkbmSQLBinaryNode(ANode);
          if (bnode.FLeftNode=nil) or (bnode.FLeftNode.NonScalar) then
             raise Exception.Create('Left node is non scalar.');
          if (bnode.FRightNode=nil) or (bnode.FRightNode.NonScalar) then
             raise Exception.Create('Right node is non scalar.');
          OptimizeNodeTree(bnode.FLeftNode);
          OptimizeNodeTree(bnode.FRightNode);
          if (bnode.LeftNode is TkbmSQLConstNode) and (bnode.RightNode is TkbmSQLConstNode) then
          begin
               v:=bnode.Execute;
               newnode:=TkbmSQLConstNode.Create(bnode.FOperation,v);
               newnode.FID:=bnode.FID;
               newnode.FAlias:=bnode.FAlias;
               newnode.FDescription:=bnode.FDescription;
               newnode.Parent:=bnode.Parent;
               ReplaceParentRef(bnode.Parent,bnode,newnode);
               bnode.Free;
               ANode:=newnode;
          end;
     end
     else if ANode is TkbmSQLUnaryNode then
     begin
          unode:=TkbmSQLUnaryNode(ANode);
          if (unode.FRightNode=nil) or (unode.FRightNode.NonScalar) then
             raise Exception.Create('Right node is non scalar.');
          OptimizeNodeTree(unode.FRightNode);
          if unode.RightNode is TkbmSQLConstNode then
          begin
               v:=unode.Execute;
               newnode:=TkbmSQLConstNode.Create(unode.FOperation,v);
               newnode.FID:=unode.FID;
               newnode.FAlias:=unode.FAlias;
               newnode.FDescription:=unode.FDescription;
               newnode.Parent:=unode.Parent;
               ReplaceParentRef(unode.Parent,unode,newnode);
               unode.Free;
               ANode:=newnode;
          end;
     end
     else if ANode is TkbmSQLBetweenNode then
     begin
          btnode:=TkbmSQLBetweenNode(ANode);
          if (btnode.FLeftNode=nil) or (btnode.FLeftNode.NonScalar) then
             raise Exception.Create('Left node is non scalar.');
          if (btnode.FRangeLow=nil) or (btnode.FRangeLow.NonScalar) then
             raise Exception.Create('Low boundary range node is non scalar.');
          if (btnode.FRangeHigh=nil) or (btnode.FRangeHigh.NonScalar) then
             raise Exception.Create('High boundary range node is non scalar.');
          OptimizeNodeTree(btnode.FLeftNode);
          OptimizeNodeTree(btnode.FRangeLow);
          OptimizeNodeTree(btnode.FRangeHigh);
          if (btnode.FLeftNode is TkbmSQLConstNode) and (btnode.RangeHigh is TkbmSQLConstNode) and (btnode.RangeLow is TkbmSQLConstNode) then
          begin
               v:=btnode.Execute;
               newnode:=TkbmSQLConstNode.Create(btnode.FOperation,v);
               newnode.FID:=btnode.FID;
               newnode.FAlias:=btnode.FAlias;
               newnode.FDescription:=btnode.FDescription;
               ReplaceParentRef(btnode.Parent,btnode,newnode);
               btnode.Free;
               ANode:=newnode;
          end;
     end
     else if ANode is TkbmSQLInNode then
     begin
          innode:=TkbmSQLInNode(ANode);
          if (innode.FLeftNode=nil) or (innode.FLeftNode.NonScalar) then
             raise Exception.Create('Left node is non scalar.');
          OptimizeNodeTree(innode.FLeftNode);
          b:=true;
          for i:=innode.Nodes.Count-1 downto 0 do
          begin
               n:=innode.Nodes[i];
               OptimizeNodeTree(n);
               if not (n is TkbmSQLConstNode) then
                  b:=false;
          end;
          if (innode.FLeftNode is TkbmSQLConstNode) and b then
          begin
               v:=innode.Execute;
               newnode:=TkbmSQLConstNode.Create(innode.FOperation,v);
               newnode.FID:=innode.FID;
               newnode.FAlias:=innode.FAlias;
               newnode.FDescription:=innode.FDescription;
               newnode.Parent:=innode.Parent;
               ReplaceParentRef(innode.Parent,innode,newnode);
               innode.Free;
               ANode:=newnode;
          end;
     end
     else if ANode is TkbmSQLFunctionNode then
     begin
          fctnode:=TkbmSQLFunctionNode(ANode);
          for i:=fctnode.Args.Count-1 downto 0 do
          begin
              n:=fctnode.Args[i];
              OptimizeNodeTree(n);
          end;
          if (fctNode.DataType = ftVariant) then
          begin
               // Find more open field type matching arguments.
               if not TkbmSQLCustomFunction(fctNode.FFunction)(self,fsDataType,fctNode.FArgs,v) then
                  raise Exception.Create('Custom function failed datatype request: '+fctNode.FunctionName);
               fctNode.FDataType:=TFieldType(v);
          end;
     end
     else if ANode is TkbmSQLAggregateNode then
     begin
          aggnode:=TkbmSQLAggregateNode(ANode);
          n:=aggnode.ExpressionNode;
          OptimizeNodeTree(n);
     end;
end;

// -------------------------------

constructor TkbmSQLUpdateOperation.Create(AOwner:TObject);
begin
     inherited Create(AOwner);
     FType:=sotUPDATE;
     FFields:=TkbmSQLFieldNodes.Create(true);
     FValues:=TkbmSQLNodes.Create(true);
     FTable:=nil;
     FCondition:=nil;
end;

destructor TkbmSQLUpdateOperation.Destroy;
begin
     if FCondition<>nil then
        FCondition.Free;
     FCondition:=nil;
     FFields.Free;
     FValues.Free;
     if FTable<>nil then
        FTable.Free;
     FTable:=nil;
     inherited Destroy;
end;

procedure TkbmSQLUpdateOperation.Prepare;
begin
end;

procedure TkbmSQLUpdateOperation.Optimize;
var
   i:integer;
   n:TkbmSQLCustomNode;
begin
     OptimizeNodeTree(FCondition);
     for i:=FValues.Count-1 downto 0 do
     begin
          n:=FValues.Node[i];
          OptimizeNodeTree(n);
     end;
end;

procedure TkbmSQLUpdateOperation.Execute;
var
   dbapi:TkbmSQLCustomDBAPI;
begin
     dbapi:=kbmSQLDBAPIRegistrations.GetAPI(FTable.Dataset.ClassType);
     dbapi.Update(FTable.Dataset,self);
end;

// -------------------------------

constructor TkbmSQLDeleteOperation.Create(AOwner:TObject);
begin
     inherited Create(AOwner);
     FType:=sotDELETE;
     FCondition:=nil;
     FTable:=nil;
end;

destructor TkbmSQLDeleteOperation.Destroy;
begin
     if FCondition<>nil then
        FCondition.Free;
     FCondition:=nil;
     if FTable<>nil then
        FTable.Free;
     FTable:=nil;
     inherited Destroy;
end;

procedure TkbmSQLDeleteOperation.Prepare;
begin
end;

procedure TkbmSQLDeleteOperation.Optimize;
begin
     OptimizeNodeTree(FCondition);
end;

procedure TkbmSQLDeleteOperation.Execute;
var
   dbapi:TkbmSQLCustomDBAPI;
begin
     dbapi:=kbmSQLDBAPIRegistrations.GetAPI(FTable.Dataset.ClassType);
     dbapi.Delete(FTable.Dataset,self);
end;

// -------------------------------

constructor TkbmSQLInsertOperation.Create(AOwner:TObject);
begin
     inherited Create(AOwner);
     FType:=sotINSERT;
     FTable:=nil;
     FFields:=TkbmSQLFieldNodes.Create(true);
     FValues:=TkbmSQLNodes.Create(true);
end;

destructor TkbmSQLInsertOperation.Destroy;
begin
     FValues.Free;
     FFields.Free;
     if FTable<>nil then
        FTable.Free;
     FTable:=nil;
     inherited Destroy;
end;

procedure TkbmSQLInsertOperation.Prepare;
begin
end;

procedure TkbmSQLInsertOperation.Optimize;
var
   i:integer;
   n:TkbmSQLCustomNode;
begin
     for i:=FValues.Count-1 downto 0 do
     begin
          n:=FValues.Node[i];
          OptimizeNodeTree(n);
     end;
end;

procedure TkbmSQLInsertOperation.Execute;
var
   dbapi:TkbmSQLCustomDBAPI;
begin
     dbapi:=kbmSQLDBAPIRegistrations.GetAPI(FTable.Dataset.ClassType);
     dbapi.Insert(FTable.Dataset,self);
end;

// -------------------------------

constructor TkbmSQLEvaluationOperation.Create(AOwner:TObject);
begin
     inherited Create(AOwner);
     FType:=sotEVALUATE;
     FFields:=TkbmSQLFieldNodes.Create(true);
     FEvaluation:=nil;
end;

destructor TkbmSQLEvaluationOperation.Destroy;
begin
     if FEvaluation<>nil then
        FEvaluation.Free;
     FEvaluation:=nil;
     FFields.Free;
     inherited Destroy;
end;

function TkbmSQLEvaluationOperation.GetErrorColOffset:integer;
begin
     Result:=5; // EVAL<space> and CALC<space>
end;

procedure TkbmSQLEvaluationOperation.Prepare;
begin
end;

procedure TkbmSQLEvaluationOperation.Optimize;
begin
     OptimizeNodeTree(FEvaluation);
end;

procedure TkbmSQLEvaluationOperation.Execute;
begin
end;

function TkbmSQLEvaluationOperation.Evaluate:variant;
begin
     if FEvaluation<>nil then
        Result:=FEvaluation.Execute
     else
         Result:=null;
end;

// -------------------------------

constructor TkbmSQLCustomSelectOperation.Create(AOwner:TObject);
begin
     inherited Create(AOwner);
     FSelection:=TkbmSQLNodes.Create(true);
     FAggregates:=TkbmSQLNodes.Create(false);
     FTables:=TkbmSQLTables.Create(true);
     FCondition:=nil;
     FOrder:=TkbmSQLNodes.Create(true);
     FGroup:=TkbmSQLNodes.Create(true);
     FHavingCondition:=nil;
     FLimit:=nil;
     FOffset:=nil;
     FDistinct:=false;
end;

destructor TkbmSQLCustomSelectOperation.Destroy;
begin
     FGroup.Free;
     FOrder.Free;
     if FCondition<>nil then
        FCondition.Free;
     FCondition:=nil;
     if FHavingCondition<>nil then
        FHavingCondition.Free;
     FHavingCondition:=nil;
     if FLimit<>nil then
        FLimit.Free;
     FLimit:=nil;
     if FOffset<>nil then
        FOffset.Free;
     FOffset:=nil;
     FTables.Free;
     FAggregates.Free;
     FSelection.Free;
     inherited Destroy;
end;

function TkbmSQLCustomSelectOperation.GetIsAggregate:boolean;
begin
     Result:=FAggregates.Count>0;
end;

function TkbmSQLCustomSelectOperation.GetIsDistinctAggregate:boolean;
var
   i:integer;
   n:TkbmSQLCustomNode;
begin
     Result:=false;
     for i:=0 to FAggregates.Count-1 do
     begin
          n:=FAggregates.Node[i];
          if n is TkbmSQLAggregateNode then
          begin
               if TkbmSQLAggregateNode(n).Distinct then
               begin
                    Result:=true;
                    break;
               end;
          end;
     end;
end;

function TkbmSQLCustomSelectOperation.GetFieldByAlias(const AAlias:string):TkbmSQLCustomNode;
begin
     Result:=FSelection.GetByAlias(AAlias);
end;

// Replace all Wildcard fieldnodes with relevant single fields.
// Eg.. Select * from -> Select f1,f2... from
procedure TkbmSQLCustomSelectOperation.FixupSelectionWildcardNodes;
var
   i,j:integer;
   n:TkbmSQLCustomNode;
   fn,newfn:TkbmSQLFieldNode;
   tblref:TDataset;
   fldref:TField;
begin
     for i:=0 to FSelection.Count-1 do
     begin
          n:=FSelection.Node[i];

          // Check if wild card node.
          if (n is TkbmSQLFieldNode) and (TkbmSQLFieldNode(n).FieldType=ntWildcard) then
          begin
               fn:=TkbmSQLFieldNode(n);

               FSelection.Delete(i);
               j:=FRefSourceFields.IndexOf(fn);
               FRefSourceFields.Delete(j);

               try
                  tblref:=fn.Table.Dataset;
                  for j:=tblref.FieldCount-1 downto 0 do
                  begin
                       fldref:=tblref.Fields[j];
                       if fldref.FieldKind=fkData then
                       begin
                            newfn:=TkbmSQLFieldNode.Create(self);
                            newfn.FieldName:=fldref.FieldName;
                            newfn.Tablename:=fn.Tablename;
                            newfn.Table:=fn.Table;
                            newfn.OriginatingField:=fn.Table.Dataset.FindField(fn.FieldName);
                            Selection.Insert(i,newfn);
                            RefSourceFields.Insert(i,newfn);
                       end;
                  end;
               finally
                  fn.Free;
               end;
          end;
     end;
end;

procedure TkbmSQLCustomSelectOperation.FixupRefSourceFields;
begin
     // Fixup table references.
     inherited FixupRefSourceFields;
end;

procedure TkbmSQLCustomSelectOperation.Prepare;
begin
end;

procedure TkbmSQLCustomSelectOperation.Optimize;
var
   i:integer;
   n:TkbmSQLCustomNode;
begin
     OptimizeNodeTree(FCondition);
     OptimizeNodeTree(FHavingCondition);
     OptimizeNodeTree(FLimit);
     OptimizeNodeTree(FOffset);
     for i:=FSelection.Count-1 downto 0 do
     begin
          n:=FSelection.Node[i];
          OptimizeNodeTree(n);
     end;
     for i:=FOrder.Count-1 downto 0 do
     begin
          n:=FOrder.Node[i];
          OptimizeNodeTree(n);
     end;
     for i:=FGroup.Count-1 downto 0 do
     begin
          n:=FGroup.Node[i];
          OptimizeNodeTree(n);
     end;
end;

procedure TkbmSQLCustomSelectOperation.Execute;
var
   dbapi:TkbmSQLCustomDBAPI;
   clt:TClass;
begin
     clt:=FTables.Tables[0].Dataset.ClassType;
     dbapi:=kbmSQLDBAPIRegistrations.GetAPI(clt);
     if dbapi=nil then
        raise Exception.Create('No DB API installed for table of class: '+clt.ClassName);


     dbapi.Select(FTables.Tables[0].Dataset,FResultTable,self);
end;

// --------------------------------------------

end.
