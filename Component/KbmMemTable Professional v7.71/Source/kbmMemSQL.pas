unit kbmMemSQL;

// =========================================================================
// A kbmMemTable based SQL implementation.
//
// Copyright 2007-2012 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com
//
// INSTALLATION
// ------------
// After installation of the source on disk, make sure that
// Delphi/C++Builder is able to locate the kbmMemTable.inc file from
// the kbmMemTable source directory. Its a requirement for compilation.
//
// 1.00.00 beta Jan 28. 2011
//        Published first release of kbmSQL.
//        Please see demo for sample of how to use.
//        It require kbmMemTable v. 7.01.00 or later.
//        Currently supports:
//           SELECT, UPDATE, INSERT, DELETE
//           ORDER BY (DESC), GROUP BY, aggregates COUNT, MIN, MAX, SUM, AVG
//           AS aliasing, LIKE, BETWEEN, IN, <,>,<=,>=,<>, NOT, IS NULL, IS NOT NULL,
//           Complex calculations incl. combinations of +, -, *, /, (, ) and
//            expression functions incl:
//             SIN,COS,TAN,LOG,LOG2,TRUNC,SQR,UPPER,LOWER,TRIM,NOW,DATE,TIME,
//             YEAR,MONTH,DAY,HOURS,MINUTES,SECONDS
//           It supports registration of custom expression functions, and multiple
//           tables with table aliases.
//        It does currently not support joins, sub selects or HAVING.
//        Further, except for GROUP BY and ORDER BY, it does not utilize
//        defined indexes at this point.
//
// 1.01.00 beta Aug 23. 2011
//        Added support for using ' or " for string literals (before only ' was supported).
//        Fixed to accept floating point values using both . and , for decimal comma.
//        Fixed BETWEEN.
//        Added support for providing descriptive text for fields using AS. Eg.
//          SELECT fld as somefld ("this is some field") from sometable
//        Added LIKE support.
//
// 7.10.00 Merged kbmSQL into kbmMemTable. Please see history in kbmMemTable.pas for further history info.

interface

{$I kbmMemTable.inc}

uses
  db,
  kbmMemTable,
  Classes,
  SysUtils,
  kbmSQLParser,
  kbmSQLElements;

const
   TkbmMemSQLNumericVarTypes = [varShortInt,varByte,varWord,varLongWord,varInt64,varUInt64,
                                varSmallint,varInteger,varSingle,varDouble,varCurrency,varDate,varBoolean];
   TkbmMemSQLNumericFieldTypes = [ftSmallint,ftInteger,ftWord,ftBoolean,ftFloat,ftCurrency,ftDate,ftTime,ftDateTime,
                                  ftAutoInc,ftLargeint,ftTimeStamp
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                                  ,ftLongWord
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                                  ,ftShortint
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                                  ,ftByte
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                                  ,ftExtended
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                  ,ftSingle
{$ENDIF}
                                  ];

type
   TkbmSQLOnError = function (Sender:TObject; AErrorType,AErrorCode,ALine,ACol:integer; const AMsg,AData:string):boolean of object;


{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
   TkbmMemSQL = class(TkbmCustomMemTable)
   private
      FParser:TkbmSQLParser;
      FTables:TkbmSQLTables;
      FContext:TObject;
      FOnGetVariableValue:TkbmSQLOnGetVariableValue;
      FOnGetVariableMetaData:TkbmSQLOnGetVariableMetaData;
      FOnGetFunction:TkbmSQLOnGetFunction;
      FOnError:TkbmSQLOnError;
   protected
      procedure ResetAffectedRows; virtual;
      procedure FixupRefSourceTables; virtual;
      procedure DoOnError(Sender:TObject; AErrorType,AErrorCode,ALine,ACol:integer; const AMsg,AData:string); virtual;
   public

      procedure ExecSQL(ASQL:string);
      function Evaluate(const AExpression:string; const ASyntaxCheckOnly:boolean = false; const AOptions:TkbmSQLExpressionOptions=[seoOnlyNumericExpressions]):variant;
      function Calculate(const AExpression:string; const ASyntaxCheckOnly:boolean = false; const AOptions:TkbmSQLExpressionOptions=[seoOnlyNumericExpressions]):variant;

      constructor Create(AOwner:TComponent); override;
      destructor Destroy; override;

      property Context:TObject read FContext write FContext;
      property Tables:TkbmSQLTables read FTables;
      property Parser:TkbmSQLParser read FParser;
{$IFDEF LEVEL9}
      property FormatSettings;
{$ENDIF}
   published
      property OnGetVariableValue:TkbmSQLOnGetVariableValue read FOnGetVariableValue write FOnGetVariableValue;
      property OnGetVariableMetaData:TkbmSQLOnGetVariableMetaData read FOnGetVariableMetaData write FOnGetVariableMetaData;
      property OnGetFunction:TkbmSQLOnGetFunction read FOnGetFunction write FOnGetFunction;
      property OnError:TkbmSQLOnError read FOnError write FOnError;
   end;

implementation

uses
    Variants,
    kbmSQLDBAPI;

constructor TkbmMemSQL.Create(AOwner:TComponent);
begin
     inherited Create(AOwner);
     FTables:=TkbmSQLTables.Create(true);
     FParser:=TkbmSQLParser.Create(self);
     FParser.OnError:=DoOnError;
end;

destructor TkbmMemSQL.Destroy;
begin
     FParser.Free;
     FTables.Free;
     inherited Destroy;
end;

procedure TkbmMemSQL.DoOnError(Sender:TObject; AErrorType,AErrorCode,ALine,ACol:integer; const AMsg,AData:string);
var
   s:string;
   b:boolean;
begin
     // Adjust error column.
     if FParser.Operation<>nil then
        ACol:=ACol-FParser.Operation.GetErrorColOffset;

     if Assigned(FOnError) then
        b:=FOnError(self,AErrorType,AErrorCode,ALine,ACol,AMsg,AData)
     else
         b:=false;
     if not b then
     begin
          s:=AMsg+#13#10+'Line: '+inttostr(ALine)+' Col:'+inttostr(ACol)+' ('+AData+')';
          raise Exception.Create(s);
     end;
end;

procedure TkbmMemSQL.ResetAffectedRows;
var
   i:integer;
begin
     for i:=0 to FTables.Count-1 do
         FTables[i].AffectedRows:=0;
end;

procedure TkbmMemSQL.FixupRefSourceTables;
var
   i:integer;
   tbl1,tbl2:TkbmSQLTable;
begin
     // Match up the parser tables with the defined tables.
     for i:=0 to FParser.Operation.RefSourceTables.Count-1 do
     begin
          tbl1:=FParser.Operation.RefSourceTables[i];
          tbl2:=Tables.TableByName[tbl1.Name];
          if tbl2=nil then
             tbl2:=Tables.TableByAlias[tbl1.Alias];
          if tbl2=nil then
             raise Exception.Create('Table identified in SQL by '''+tbl1.AliasOrName+''' is unavailable.');

          tbl1.BaseTable:=tbl2;
          tbl1.Dataset:=tbl2.Dataset;
          tbl1.API:=kbmSQLDBAPIRegistrations.GetAPI(tbl1.Dataset.ClassType);
     end;
end;

procedure TkbmMemSQL.ExecSQL(ASQL:string);
var
   so:TkbmSQLSelectOperation;
begin
     Reset;
     FParser.Init;
     FParser.Context:=FContext;
     FParser.OnGetVariableMetaData:=FOnGetVariableMetaData;
     FParser.OnGetVariableValue:=FOnGetVariableValue;
     FParser.OnGetFunction:=FOnGetFunction;
     ASQL:=trim(ASQL);
     if ASQL='' then
        exit;

     FParser.SetSource(ASQL);
     if not FParser.Execute then
        exit;
     ResetAffectedRows;
     FixupRefSourceTables;
     FParser.Operation.FixupRefSourceFields;
     if FParser.Operation is TkbmSQLSelectOperation then
     begin
          TkbmSQLSelectOperation(FParser.Operation).FixupSelectionWildcardNodes;
          FParser.Operation.FixupRefSourceFields;
          so:=TkbmSQLSelectOperation(FParser.Operation);
          so.ResultTable:=self;
     end;
     FParser.Operation.Optimize;
     FParser.Operation.Execute;
end;

function TkbmMemSQL.Evaluate(const AExpression:string; const ASyntaxCheckOnly:boolean = false; const AOptions:TkbmSQLExpressionOptions=[seoOnlyNumericExpressions]):variant;
var
   eo:TkbmSQLEvaluationOperation;
   vt:TVarType;
begin
     Reset;

     FParser.Init;
     FParser.Context:=FContext;
     FParser.OnGetVariableMetaData:=FOnGetVariableMetaData;
     FParser.OnGetVariableValue:=FOnGetVariableValue;
     FParser.OnGetFunction:=FOnGetFunction;
     FParser.ExpressionOptions:=AOptions;
     FParser.SetSource('EVAL '+AExpression+';');
     if not FParser.Execute then
     begin
          if ASyntaxCheckOnly then
             Result:=false
          else
              Result:=Null;
          exit;
     end;
     if not (FParser.Operation is TkbmSQLEvaluationOperation) then
     begin
          if ASyntaxCheckOnly then
          begin
               Result:=false;
               exit;
          end
          else
              raise Exception.Create('Expression didnt result in an evaluation operation.');
     end;
     eo:=TkbmSQLEvaluationOperation(FParser.Operation);
     eo.Optimize;

     if ASyntaxCheckOnly then
     begin
          if (seoOnlyNumericExpressions in AOptions) and (not (eo.Evaluation.DataType in TkbmMemSQLNumericFieldTypes)) then
          begin
               Result:=false;
               exit;
          end;

          Result:=true;
          exit;
     end;

     Result:=eo.Evaluate;

     if (seoOnlyNumericExpressions in AOptions) then
     begin
          vt:=VarType(Result);
          if not (vt in TkbmMemSQLNumericVarTypes) then
             raise Exception.Create('Expression didnt result in a numeric result.');
     end;
end;

function TkbmMemSQL.Calculate(const AExpression:string; const ASyntaxCheckOnly:boolean = false; const AOptions:TkbmSQLExpressionOptions=[seoOnlyNumericExpressions]):variant;
var
   eo:TkbmSQLEvaluationOperation;
   vt:TVarType;
begin
     Reset;

     FParser.Init;
     FParser.Context:=FContext;
     FParser.OnGetVariableMetaData:=FOnGetVariableMetaData;
     FParser.OnGetVariableValue:=FOnGetVariableValue;
     FParser.OnGetFunction:=FOnGetFunction;
     FParser.ExpressionOptions:=AOptions;
     FParser.SetSource('CALC '+AExpression+';');
     if not FParser.Execute then
     begin
          if ASyntaxCheckOnly then
             Result:=false
          else
              Result:=Null;
          exit;
     end;
     if not (FParser.Operation is TkbmSQLEvaluationOperation) then
     begin
          if ASyntaxCheckOnly then
          begin
               Result:=false;
               exit;
          end
          else
              raise Exception.Create('Expression didnt result in an evaluation operation.');
     end;
     eo:=TkbmSQLEvaluationOperation(FParser.Operation);
     eo.Optimize;

     if ASyntaxCheckOnly then
     begin
          if (seoOnlyNumericExpressions in AOptions) and (not (eo.Evaluation.DataType in TkbmMemSQLNumericFieldTypes)) then
          begin
               Result:=false;
               exit;
          end;

          Result:=true;
          exit;
     end;

     Result:=eo.Evaluate;

     if (seoOnlyNumericExpressions in AOptions) then
     begin
          vt:=VarType(Result);
          if not (vt in TkbmMemSQLNumericVarTypes) then
             raise Exception.Create('Expression didnt result in a numeric result.');
     end;
end;


end.
