unit kbmSQLDBAPI;

// =========================================================================
// A kbmMemTable based SQL implementation.
//
// Copyright 2007-2014 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com

interface

{$include kbmMemTable.inc}

uses
    Classes,
{$IFDEF NEXTGEN}
    System.Generics.Collections,
{$ENDIF}
    DB,
    kbmSQLElements,
    kbmMemTable;

{$WARN UNSAFE_CAST OFF}

const
   KBMSQL_UNIQUE_INDEXNAME = '__KBMSQL_UNIQUE';
   KBMSQL_ORDERBY_INDEXNAME = '__KBMSQL_ORDERBY';

type
   TkbmSQLCustomDBAPI = class;
   TkbmSQLCustomDBAPIClass = class of TkbmSQLCustomDBAPI;

   TkbmSQLDBAPIRegistration = class
   private
      FDataset:TClass;
      FDatasetAPI:TkbmSQLCustomDBAPI;
   public
      constructor Create(const ADataset:TClass; const ADatasetAPIClass:TkbmSQLCustomDBAPIClass); virtual;
      destructor Destroy; override;
      property Dataset:TClass read FDataset;
      property DatasetAPI:TkbmSQLCustomDBAPI read FDatasetAPI;
   end;

   TkbmSQLDBAPIRegistrations = class
   private
{$IFDEF NEXTGEN}
      FList:TList<TkbmSQLDBAPIRegistration>;
{$ELSE}
      FList:TList;
{$ENDIF}
   public
      constructor Create; virtual;
      destructor Destroy; override;

      procedure Clear;
      procedure RegisterAPI(ADatasetClass:TClass; ADatasetAPIClass:TkbmSQLCustomDBAPIClass);
      function GetAPI(ADataset:TClass):TkbmSQLCustomDBAPI;
   end;

   TkbmSQLCustomDBAPI = class(TkbmSQLCustomDBAPI_)
   protected
      function CreateField(const AOwner:TComponent; const ADataType:TFieldType):TField; virtual;
      procedure CheckNonAggregateFieldsInGroupClause(const ASelection:TkbmSQLNodes; const AGroup:TkbmSQLNodes); virtual;
      procedure CreateSelectionFields(const ATable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const ACreateGroupFields:boolean; const AUseAggregateSources:boolean); virtual;
      procedure CreateDistinctIndex(const ATable:TkbmCustomMemTable; const ADistinct:TkbmSQLNodes); virtual;
      procedure AppendResultRecord(const AResultTable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const AIncludeGroupFields:boolean); overload; virtual;
      procedure ApplyGroupBy(const ASourceTable,AResultTable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const AGroup:TkbmSQLNodes; const AAggregates:TkbmSQLNodes); virtual;
      procedure ApplyOrderBy(const AResultTable:TkbmCustomMemTable; const AOrder:TkbmSQLNodes); virtual;
      procedure ApplyOffsetAndLimit(const AResultTable:TkbmCustomMemTable; const AOffset:TkbmSQLCustomNode; const ALimit:TkbmSQLCustomNode); virtual;
      procedure DetermineUsableIndexesForRefFields(const AOperation:TkbmSQLCustomOperation); virtual;
      procedure SetupSourceFields(const ANodes:TkbmSQLNodes; const ADataset:TDataset); virtual;
      procedure SetupSourceField(const ANode:TkbmSQLCustomNode; const ADataset:TDataset); virtual;
      procedure Search(const ASourceTable,AResultTable:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation; const ASelection:TkbmSQLNodes; const ACondition:TkbmSQLCustomNode; const ACopyDirectlyFromSource:boolean); virtual;
      function GetIndexNameForField(const AField:TkbmSQLFieldNode):string; virtual; abstract;
      procedure FixupTemporaryResultFields(const ATempResultTable:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation; const AFields:TkbmSQLFieldNodes); virtual;

   public
      function LocateFirst(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean; virtual;
      function LocateNext(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean; virtual;

      function Insert(const ADataset:TObject; const AOperation:TkbmSQLInsertOperation):boolean; virtual; abstract;
      function Delete(const ADataset:TObject; const AOperation:TkbmSQLDeleteOperation):boolean; virtual; abstract;
      function Update(const ADataset:TObject; const AOperation:TkbmSQLUpdateOperation):boolean; virtual; abstract;
      function Select(const ADataset:TObject; const AResultDataset:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation):boolean; virtual; abstract;
   end;

   TkbmSQLCustomDatasetAPI = class(TkbmSQLCustomDBAPI)
   public
      function Insert(const ADataset:TObject; const AOperation:TkbmSQLInsertOperation):boolean; override;
      function Delete(const ADataset:TObject; const AOperation:TkbmSQLDeleteOperation):boolean; override;
      function Update(const ADataset:TObject; const AOperation:TkbmSQLUpdateOperation):boolean; override;
      function Select(const ADataset:TObject; const AResultDataset:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation):boolean; override;
   end;

var
   kbmSQLDBAPIRegistrations:TkbmSQLDBAPIRegistrations;

implementation

uses
    Variants,
    SysUtils,
    kbmMemSQL;


constructor TkbmSQLDBAPIRegistration.Create(const ADataset:TClass; const ADatasetAPIClass:TkbmSQLCustomDBAPIClass);
begin
     inherited Create;
     FDataset:=ADataset;
     FDatasetAPI:=ADatasetAPIClass.Create;
end;

destructor TkbmSQLDBAPIRegistration.Destroy;
begin
     FDatasetAPI.Free;
     inherited Destroy;
end;

constructor TkbmSQLDBAPIRegistrations.Create;
begin
     inherited Create;
{$IFDEF NEXTGEN}
     FList:=TList<TkbmSQLDBAPIRegistration>.Create;
{$ELSE}
     FList:=TList.Create;
{$ENDIF}
end;

destructor TkbmSQLDBAPIRegistrations.Destroy;
begin
     Clear;
     FList.Free;
     inherited Destroy;
end;

procedure TkbmSQLDBAPIRegistrations.Clear;
{$IFNDEF NEXTGEN}
var
   i:integer;
{$ENDIF}
begin
{$IFNDEF NEXTGEN}
     for i:=0 to FList.Count-1 do
         TkbmSQLCustomDBAPI(FList.Items[i]).Free;
{$ENDIF}
     FList.Clear;
end;

procedure TkbmSQLDBAPIRegistrations.RegisterAPI(ADatasetClass:TClass; ADatasetAPIClass:TkbmSQLCustomDBAPIClass);
begin
     if GetAPI(ADatasetClass)<>nil then
        raise Exception.Create('Database API already registered for '+ADatasetClass.ClassName);
     FList.Add(TkbmSQLDBAPIRegistration.Create(ADatasetClass,ADatasetAPIClass));
end;

function TkbmSQLDBAPIRegistrations.GetAPI(ADataset:TClass):TkbmSQLCustomDBAPI;
var
   i:integer;
   reg:TkbmSQLDBAPIRegistration;
begin
     for i:=0 to FList.Count-1 do
     begin
          reg:=TkbmSQLDBAPIRegistration(FList.Items[i]);
          if (ADataset=reg.FDataset) or (ADataset.InheritsFrom(reg.FDataset)) then
          begin
               Result:=reg.FDatasetAPI;
               exit;
          end;
     end;
     Result:=nil;
end;


procedure TkbmSQLCustomDBAPI.FixupTemporaryResultFields(const ATempResultTable:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation; const AFields:TkbmSQLFieldNodes);
var
   i:integer;
   n:TkbmSQLCustomNode;
   fld:TkbmSQLFieldNode;
begin
     // Match up the parser fields with the defined having fields.
     for i:=0 to AFields.Count-1 do
     begin
          n:=AFields[i];
          if not (n is TkbmSQLFieldNode) then
             continue;

          fld:=TkbmSQLFieldNode(n);

          // Check if field havnt prefixed tablename. Then only one table must be specified.
          if (fld.Tablename='') then
          begin
               if AOperation.RefSourceTables.Count>1 then
                  raise Exception.Create('Field '''+fld.FieldName+''' must be prefixed with tablename as multiple tables has been specified.');
               fld.TableName:=AOperation.RefSourceTables[0].Name;
          end;

          // Find relevant table for field.
          fld.Table:=AOperation.RefSourceTables.TableByName[fld.TableName];
          if fld.Table=nil then
             raise Exception.Create('Table referenced by field '''+fld.FieldName+''' is unavailable.');

          fld.OriginatingField:=ATempResultTable.FindField(fld.FieldName);
     end;
end;

function TkbmSQLCustomDBAPI.CreateField(const AOwner:TComponent; const ADataType:TFieldType):TField;
{
  const
     FTClass : array [TFieldType] of TFieldClass = (
       nil,              // ftUnknown,
       TStringField,     // ftString,
       TSmallIntField,   // ftSmallint,
       TIntegerField,    // ftInteger,
       TWordField,       // ftWord,
       TBooleanField,    // ftBoolean,
       TFloatField,      // ftFloat,
       TCurrencyField,   // ftCurrency,
       TBCDField,        // ftBCD,
       TDateField,       // ftDate,
       TTimeField,       // ftTime,
       TDateTimeField,   // ftDateTime,
       TBytesField,      // ftBytes,
       TVarBytesField,   // ftVarBytes,
       TAutoIncField,    // ftAutoInc,
       TBlobField,       // ftBlob,
       TMemoField,       // ftMemo,
       TGraphicField,    // ftGraphic,
       TFmtMemoField,    // ftFmtMemo,
       TParadoxOLEField, // ftParadoxOle,
       nil,              // ftDBaseOle,
       TTypedBinaryField,// ftTypedBinary,
       nil,              // ftCursor,
       TFixedCharField,  // ftFixedChar,
       TWideStringField, // ftWideString,
       TLargeIntField,   // ftLargeint,
       nil,              // ftADT,
       nil,              // ftArray,
       nil,              // ftReference,
       nil,              // ftDataSet,
       nil,              // ftOraBlob,
       TOraClobField,    // ftOraClob,
       nil,              // ftVariant,
       nil,              // ftInterface,
       nil,              // ftIDispatch,
       TGUIDField,       // ftGuid,
       TTimeStampField,  // ftTimeStamp,
       TFMTBCDField      // ftFMTBcd
     );
}
var
   fc:TFieldClass;
begin
     fc:=DefaultFieldClasses[ADataType];
     if fc=nil then
        raise Exception.Create('Unsupported field type ('+inttostr(ord(ADataType))+').');
     Result:=fc.Create(AOwner);
end;

procedure TkbmSQLCustomDBAPI.CheckNonAggregateFieldsInGroupClause(const ASelection:TkbmSQLNodes; const AGroup:TkbmSQLNodes);
begin
     // TODO.
end;

procedure TkbmSQLCustomDBAPI.CreateSelectionFields(const ATable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const ACreateGroupFields:boolean; const AUseAggregateSources:boolean);
var
   i:integer;
   node:TkbmSQLCustomNode;
   fld:TkbmSQLFieldNode;
   ffld:TField;
   dt:TFieldType;
begin
     ATable.DeleteTable;

     for i:=0 to ASelection.Count-1 do
     begin
          node:=ASelection[i];

          if (node is TkbmSQLGroupFieldNode) and (not ACreateGroupFields) then
             continue
          else if (node is TkbmSQLAggregateNode) and ACreateGroupFields then
          begin
               if AUseAggregateSources then
                  dt:=TkbmSQLAggregateNode(node).ExpressionNode.DataType
               else
                   dt:=TkbmSQLAggregateNode(node).DataType;
               ffld:=CreateField(ATable,dt);
               ffld.FieldName:=node.GetUniqueName;
               ffld.FieldKind:=fkData;
               ffld.DisplayLabel:=node.Description;
               if (node.DataType in kbmStringTypes) or (node.DataType in kbmBlobTypes) then
                  ffld.Size:=node.Width;
               ffld.DataSet:=ATable;
               node.DestinationField:=ffld;
          end
          else if node is TkbmSQLFieldNode then
          begin
               ffld:=nil; // To satisfy compiler warning.
               fld:=TkbmSQLFieldNode(node);
               case fld.FieldType of
                  ntField:
                    ffld:=CreateField(ATable,fld.OriginatingField.DataType);
                  ntRowID,
                  ntRecNo:
                    ffld:=CreateField(ATable,ftLargeint);
                  ntWildCard:
                    raise Exception.Create('Internal error 1');
               end;

               ffld.FieldName:=fld.UniqueName;
               ffld.FieldKind:=fkData;
               ffld.DisplayLabel:=node.Description;
               ffld.Size:=node.Width;
               ffld.DataSet:=ATable;
               node.DestinationField:=ffld;
          end
          else
          begin
               ffld:=CreateField(ATable,node.DataType);
               ffld.FieldName:=node.GetUniqueName;
               ffld.FieldKind:=fkData;
               ffld.DisplayLabel:=node.Description;
               if (node.DataType in kbmStringTypes) or (node.DataType in kbmBlobTypes) then
                  ffld.Size:=node.Width;
               ffld.DataSet:=ATable;
               node.DestinationField:=ffld;
          end;
     end;

     ATable.ResetAutoInc;
end;

procedure TkbmSQLCustomDBAPI.CreateDistinctIndex(const ATable:TkbmCustomMemTable; const ADistinct:TkbmSQLNodes);
var
   i:integer;
   s,a:string;
   n:TkbmSQLCustomNode;
begin
     s:='';
     a:='';
     for i:=0 to ADistinct.Count-1 do
     begin
          n:=ADistinct.Node[i];
          s:=s+a+n.GetUniqueName;
          a:=';';
     end;
     ATable.AddIndex(KBMSQL_UNIQUE_INDEXNAME,s,[ixUnique]);
end;

procedure TkbmSQLCustomDBAPI.DetermineUsableIndexesForRefFields(const AOperation:TkbmSQLCustomOperation);
var
   i:integer;
   n:TkbmSQLCustomNode;
   fld:TkbmSQLFieldNode;
begin
     for i:=0 to AOperation.RefSourceFields.Count-1 do
     begin
          n:=AOperation.RefSourceFields[i];
          if not (n is TkbmSQLFieldNode) then
             continue;

          fld:=TkbmSQLFieldNode(n);
          if fld.OriginatingField=nil then
             continue;
          fld.IndexName:=GetIndexNameForField(fld);
     end;
end;

procedure TkbmSQLCustomDBAPI.SetupSourceFields(const ANodes:TkbmSQLNodes; const ADataset:TDataset);
var
   i:integer;
begin
     for i:=0 to ANodes.Count-1 do
         SetupSourceField(ANodes.Node[i],ADataset);
end;

procedure TkbmSQLCustomDBAPI.SetupSourceField(const ANode:TkbmSQLCustomNode; const ADataset:TDataset);
var
   fnode:TkbmSQLFieldNode;
   aggnode:TkbmSQLAggregateNode;
   s:string;
begin
     // Point aggregate nodes to result fields.
     if (ANode is TkbmSQLFieldNode) then
     begin
          fnode:=TkbmSQLFieldNode(ANode);
          s:=fnode.GetUniqueName;
          ANode.SourceField:=ADataset.FindField(s);
          if ANode.SourceField=nil then
             ANode.SourceField:=ADataset.FindField(fnode.FieldName);
     end
     else if (ANode is TkbmSQLAggregateNode) then
     begin
          aggnode:=TkbmSQLAggregateNode(ANode);
          s:=aggnode.GetUniqueName;
          ANode.SourceField:=ADataset.FindField(s);
     end
     else if ANode is TkbmSQLUnaryNode then
        SetupSourceField(TkbmSQLUnaryNode(ANode).RightNode,ADataset)
     else if ANode is TkbmSQLBinaryNode then
     begin
          SetupSourceField(TkbmSQLBinaryNode(ANode).LeftNode,ADataset);
          SetupSourceField(TkbmSQLBinaryNode(ANode).RightNode,ADataset);
     end
     else if ANode is TkbmSQLBetweenNode then
     begin
          SetupSourceField(TkbmSQLBetweenNode(ANode).LeftNode,ADataset);
          SetupSourceField(TkbmSQLBetweenNode(ANode).RangeLow,ADataset);
          SetupSourceField(TkbmSQLBetweenNode(ANode).RangeHigh,ADataset);
     end
     else if ANode is TkbmSQLInNode then
     begin
          SetupSourceField(TkbmSQLInNode(ANode).LeftNode,ADataset);
          SetupSourceFields(TkbmSQLInNode(ANode).Nodes,ADataset);
     end
     else if ANode is TkbmSQLFunctionNode then
          SetupSourceFields(TkbmSQLFunctionNode(ANode).Args,ADataset);
end;

procedure TkbmSQLCustomDBAPI.AppendResultRecord(const AResultTable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const AIncludeGroupFields:boolean);
var
   i:integer;
   node:TkbmSQLCustomNode;
   fld:TField;
begin
     AResultTable.Append;
     for i:=0 to ASelection.Count-1 do
     begin
          node:=ASelection[i];
          if (node is TkbmSQLGroupFieldNode) and (not AIncludeGroupFields) then
             continue;
          fld:=node.DestinationField;
          if fld=nil then
             raise Exception.Create('Corrupted selection field reference: '+node.GetUniqueName);
          fld.Value:=node.Execute;
     end;
     try
        AResultTable.Post;
     except
        on E: EMemTableDupKey do
           AResultTable.Cancel;
        on E: Exception do raise;
     end;
end;

function TkbmSQLCustomDBAPI.LocateFirst(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean;
begin
     Result:=false;
end;

function TkbmSQLCustomDBAPI.LocateNext(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean;
begin
     Result:=false;
end;

procedure TkbmSQLCustomDBAPI.Search(const ASourceTable,AResultTable:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation; const ASelection:TkbmSQLNodes; const ACondition:TkbmSQLCustomNode; const ACopyDirectlyFromSource:boolean);
var
   tbl:TDataset;
   v:variant;
   exec:boolean;
begin
     tbl:=ASourceTable;
     tbl.DisableControls;
     AResultTable.DisableControls;
     try
        tbl.First;
        try
           while not tbl.Eof do
           begin
                exec:=ACondition=nil;
                if not exec then
                begin
                     v:=ACondition.Execute;
                     exec:=(not VarIsNull(v)) and (v);
                end;
                if exec then
                begin
                     if ACopyDirectlyFromSource then
                        AppendResultRecord(AResultTable,ASelection,false)
                     else
                         AppendResultRecord(AResultTable,ASelection,AOperation.Group.Count>0);
                end;
                tbl.Next;
           end;
        except
           AResultTable.Cancel;
           raise;
        end;
     finally
        tbl.EnableControls;
        AResultTable.EnableControls;
     end;
end;

procedure TkbmSQLCustomDBAPI.ApplyGroupBy(const ASourceTable,AResultTable:TkbmCustomMemTable; const ASelection:TkbmSQLNodes; const AGroup:TkbmSQLNodes; const AAggregates:TkbmSQLNodes);
var
   i:integer;
   node:TkbmSQLCustomNode;
   fnode:TkbmSQLFieldNode;
   anode:TkbmSQLAggregateNode;
   sGroupFields,sFields:string;
   s,a:string;
   gbsrcflist,
   aggsrcflist,aggdstflist:TkbmFieldList;
begin
     CreateSelectionFields(AResultTable,ASelection,false,false);
     AResultTable.Open;

     // Build group fields.
     sGroupFields:='';
     a:='';
     for i:=0 to AGroup.Count-1 do
     begin
          node:=AGroup.Node[i];
          if node is TkbmSQLFieldNode then
          begin
               fnode:=TkbmSQLFieldNode(node);
               sGroupFields:=sGroupFields+a+fnode.GetUniqueName;
               a:=';';
          end;
     end;
     a:='';

     // Build aggregate fields.
     sFields:='';
     for i:=0 to ASelection.Count-1 do
     begin
          // Point aggregate nodes to result fields.
          node:=ASelection.Node[i];
          s:=node.GetUniqueName;
          if node is TkbmSQLAggregateNode then
          begin
               anode:=TkbmSQLAggregateNode(node);
               s:=s+':'+anode.FieldModifier;
          end;
          sFields:=sFields+a+s;
          a:=';';
     end;

     gbsrcflist:=TkbmFieldList.Create;
     aggsrcflist:=TkbmFieldList.Create;
     aggdstflist:=TkbmFieldList.Create;
     try
        // Build list of fields to group on (source).
        gbsrcflist.Build(ASourceTable,sGroupFields);

        // Build list of fields to aggregate on.
        aggsrcflist.Build(ASourceTable,sFields);
        aggdstflist.Build(AResultTable,sFields);

        ASourceTable.GroupBy(AResultTable,gbsrcflist,aggsrcflist,aggdstflist);
     finally
        gbsrcflist.Free;
        aggsrcflist.Free;
        aggdstflist.Free;
     end;
end;

procedure TkbmSQLCustomDBAPI.ApplyOrderBy(const AResultTable:TkbmCustomMemTable; const AOrder:TkbmSQLNodes);
var
   i:integer;
   node,node2:TkbmSQLCustomNode;
   fnode,fnode2:TkbmSQLFieldNode;
   s,a:string;
begin
     if AOrder.Count<1 then
        exit;
     s:='';
     a:='';
     for i:=0 to AOrder.Count-1 do
     begin
          node:=AOrder.Node[i];
          if node is TkbmSQLFieldNode then
          begin
               fnode:=TkbmSQLFieldNode(node);

               // Lookup referenced field.
               node2:=TkbmSQLSelectOperation(fnode.Operation).Selection.GetByFieldName(fnode.FieldName);
               if node2 is TkbmSQLFieldNode then
                  fnode2:=TkbmSQLFieldNode(node2)
               else
                   fnode2:=fnode;
               s:=s+a+fnode2.UniqueName;
               if fnode.Descending then
                  s:=s+':D';
               a:=';';
          end;
     end;
     AResultTable.AddIndex(KBMSQL_ORDERBY_INDEXNAME,s,[]);
     AResultTable.IndexName:=KBMSQL_ORDERBY_INDEXNAME;
end;

procedure TkbmSQLCustomDBAPI.ApplyOffsetAndLimit(const AResultTable:TkbmCustomMemTable; const AOffset:TkbmSQLCustomNode; const ALimit:TkbmSQLCustomNode);
var
   n:integer;
begin
     if (AOffset<>nil) or (ALimit<>nil) then
        AResultTable.DisableControls;
     try
        if AOffset<>nil then
        begin
             n:=trunc(double(AOffset.Execute));
             while (n>0) and (AResultTable.RecordCount>0) do
             begin
                  AResultTable.First;
                  AResultTable.Delete;
                  dec(n);
             end;
        end;
        if ALimit<>nil then
        begin
             n:=trunc(double(ALimit.Execute));
             while AResultTable.RecordCount>n do
             begin
                  AResultTable.Last;
                  AResultTable.Delete;
             end;
        end;
     finally
        if (AOffset<>nil) or (ALimit<>nil) then
           AResultTable.EnableControls;
     end;
end;

function TkbmSQLCustomDatasetAPI.Insert(const ADataset:TObject; const AOperation:TkbmSQLInsertOperation):boolean;
var
   i:integer;
   tbl:TDataset;
begin
     tbl:=TDataset(ADataset);

     // Locate registered table.
     if AOperation.Fields.Count<>AOperation.Values.Count then
        raise Exception.Create('Values dont match fields.');

     Result:=false;
     tbl.DisableControls;
     try
        tbl.Insert;
        try
           for i:=0 to AOperation.Fields.Count-1 do
               AOperation.Fields.FieldNodes[i].OriginatingField.Value:=AOperation.Values.Node[i].Execute;
           tbl.Post;
           AOperation.Table.BaseTable.AffectedRows:=1;
           Result:=true;
        except
           tbl.Cancel;
           raise;
        end;
     finally
        tbl.EnableControls;
     end;
end;

function TkbmSQLCustomDatasetAPI.Delete(const ADataset:TObject; const AOperation:TkbmSQLDeleteOperation):boolean;
var
   tbl:TDataset;
   n:integer;
   exec:boolean;
   v:variant;
begin
     // Locate registered table.
     tbl:=TDataset(ADataset);

     n:=0;
     Result:=false;
     tbl.DisableControls;
     try
        tbl.First;
        try
           while not tbl.Eof do
           begin
                exec:=AOperation.Condition=nil;
                if not exec then
                begin
                     v:=AOperation.Condition.Execute;
                     exec:=(not VarIsNull(v)) and (v);
                end;
                if exec then
                begin
                     tbl.Delete;
                     inc(n);
                end
                else
                    tbl.Next;
           end;
           Result:=true;
        except
           tbl.Cancel;
           raise;
        end;
     finally
        AOperation.Table.BaseTable.AffectedRows:=n;
        tbl.EnableControls;
     end;
end;

function TkbmSQLCustomDatasetAPI.Update(const ADataset:TObject; const AOperation:TkbmSQLUpdateOperation):boolean;
var
   n:integer;
   i:integer;
   exec:boolean;
   tbl:TDataset;
   v:variant;
begin
     // Locate registered table.
     tbl:=TDataset(ADataset);

     if AOperation.Fields.Count<>AOperation.Values.Count then
        raise Exception.Create('Values dont match fields.');

     n:=0;
     Result:=false;
     tbl.DisableControls;
     try
        tbl.First;
        try
           while not tbl.Eof do
           begin
                exec:=AOperation.Condition=nil;
                if not exec then
                begin
                     v:=AOperation.Condition.Execute;
                     exec:=(not VarIsNull(v)) and (v);
                end;
                if exec then
                begin
                     tbl.Edit;
                     for i:=0 to AOperation.Fields.Count-1 do
                         AOperation.Fields.FieldNodes[i].OriginatingField.Value:=AOperation.Values.Node[i].Execute;
                     tbl.Post;
                     inc(n);
                end;
                tbl.Next;
           end;
           Result:=true;
        except
           tbl.Cancel;
           raise;
        end;
     finally
        AOperation.Table.BaseTable.AffectedRows:=n;
        tbl.EnableControls;
     end;
end;

function TkbmSQLCustomDatasetAPI.Select(const ADataset:TObject; const AResultDataset:TkbmCustomMemTable; const AOperation:TkbmSQLCustomSelectOperation):boolean;
var
   tTempResult,tGroup,tHaving:TkbmMemTable;
   nAggSelection,nSelection:TkbmSQLNodes;
begin
     // Check if group by/aggregation. Then operate via intermediate table.
     if AOperation.IsAggregate then
     begin
          tTempResult:=TkbmMemTable.Create(nil);
          tGroup:=TkbmMemTable.Create(nil);
          tHaving:=TkbmMemTable.Create(nil);
          nAggSelection:=TkbmSQLNodes.Create(false);
          nSelection:=TkbmSQLNodes.Create(false);
          try
             // Sanity check.
             CheckNonAggregateFieldsInGroupClause(AOperation.Group,AOperation.Selection);

             // Create list of selection nodes that are relevant in aggregation.
             nAggSelection.Add(AOperation.Aggregates,true);
             nAggSelection.Add(AOperation.Group,true);

             // Create group table.
             CreateSelectionFields(tGroup,nAggSelection,true,true);

             // Make distinct if requested.
             if AOperation.IsDistinctAggregate then
                CreateDistinctIndex(tGroup,AOperation.Aggregates);

             // Open result table.
             tGroup.Open;

             // Determine indexes matching referenced fields.
             DetermineUsableIndexesForRefFields(AOperation);

             // Look for records matching condition and output selection.
             Search(AOperation.Tables.Tables[0].Dataset,tGroup,AOperation,nAggSelection,AOperation.Condition,false);

             // Apply group by directly if no having condition.
             if AOperation.HavingCondition<>nil then
             begin
                  ApplyGroupBy(tGroup,tHaving,nAggSelection,AOperation.Group,AOperation.Aggregates);

                  // Create fields
                  CreateSelectionFields(tTempResult,AOperation.Selection,false,false);
                  tTempResult.Open;

                  // Apply having.
                  SetupSourceFields(AOperation.Selection,tHaving);
                  SetupSourceField(AOperation.HavingCondition,tHaving);
                  Search(tHaving,tTempResult,AOperation,AOperation.Selection,AOperation.HavingCondition,false);
             end
             else
                 // Apply group by.
                 ApplyGroupBy(tGroup,tTempResult,nAggSelection,AOperation.Group,AOperation.Aggregates);

             // Do final calculations on aggregated data.
             CreateSelectionFields(AResultDataset,AOperation.Selection,true,false);
             AResultDataset.Open;
             SetupSourceFields(AOperation.Selection,tTempResult);
             SetupSourceFields(AOperation.Aggregates,tTempResult);
             Search(tTempResult,AResultDataset,AOperation,AOperation.Selection,nil,true);

          finally
             nSelection.Free;
             nAggSelection.Free;
             tGroup.Free;
             tHaving.Free;
             tTempResult.Free;
          end;
     end
     else
     begin
          // Create result table.
          CreateSelectionFields(AResultDataset,AOperation.Selection,false,false);

          // Make distinct if requested.
          if AOperation.Distinct then
             CreateDistinctIndex(AResultDataset,AOperation.Selection);

          // Open result table.
          AResultDataset.Open;

          // Determine indexes matching referenced fields.
          DetermineUsableIndexesForRefFields(AOperation);

          // Look for records matching condition and output selection.
          Search(AOperation.Tables.Tables[0].Dataset,AResultDataset,AOperation,AOperation.Selection,AOperation.Condition,false);
     end;

     // Add appropriate index to handle order by.
     ApplyOrderBy(AResultDataset,AOperation.Order);

     // Check if offset given, drop all before offset.
     ApplyOffsetAndLimit(AResultDataset,AOperation.Offset,AOperation.Limit);

     Result:=true;
end;



initialization
   kbmSQLDBAPIRegistrations:=TkbmSQLDBAPIRegistrations.Create;

finalization
   kbmSQLDBAPIRegistrations.Free;

end.
