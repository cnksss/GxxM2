unit kbmSQLMemTableAPI;

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

interface

uses
   kbmSQLDBAPI,
   kbmSQLElements;

type
   TkbmSQLMemTableAPI = class(TkbmSQLCustomDatasetAPI)
   protected
      function GetIndexNameForField(const AField:TkbmSQLFieldNode):string; override;
   public
      function LocateFirst(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean; override;
      function LocateNext(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean; override;
//      function Aggregate(AField:TkbmSQLField; const AFunction:TkbmSQLAggregateFunction):variant; override;
   end;

implementation

uses
   DB,
   Classes,
   SysUtils,
   kbmMemTable,
   kbmMemTypes,
   kbmList,
   Math;

function TkbmSQLMemTableAPI.GetIndexNameForField(const AField:TkbmSQLFieldNode):string;
var
   mt:TkbmCustomMemTable;
   idx:TkbmIndex;
begin
     mt:=TkbmCustomMemTable(AField.Table.Dataset);
     idx:=mt.Indexes.GetByFieldNames(AField.FieldName);
     if idx<>nil then
        Result:=idx.Name
     else
         Result:='';
end;

function TkbmSQLMemTableAPI.LocateFirst(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean;
var
   ds:TkbmCustomMemTable;
begin
     ds:=TkbmCustomMemTable(ATable.Dataset);
     ds.First;
     if ACondition=nil then
     begin
          Result:=not ds.Eof;
          exit;
     end;

     Result:=false;
     while not ds.Eof do
     begin
          if ACondition.Execute=true then
          begin
               Result:=true;
               exit;
          end;
          ds.Next;
     end;
end;

function TkbmSQLMemTableAPI.LocateNext(const ATable:TkbmSQLTable; const ACondition:TkbmSQLCustomNode):boolean;
var
   ds:TkbmCustomMemTable;
begin
     ds:=TkbmCustomMemTable(ATable.Dataset);
     ds.Next;
     if ACondition=nil then
     begin
          Result:=not ds.Eof;
          exit;
     end;

     Result:=false;
     while not ds.Eof do
     begin
          if ACondition.Execute=true then
          begin
               Result:=true;
               exit;
          end;
          ds.Next;
     end;
end;

{
function TkbmSQLMemTableAPI.Aggregate(AField:TkbmSQLField; const AFunction:TkbmSQLAggregateFunction):variant;
var
   ds:TkbmCustomMemTable;
   idx:TkbmIndex;
   ref:TkbmList;
   i:integer;
   res:double;
   val:double;
   fld:TField;
begin
     ds:=TkbmCustomMemTable(AField.Table.Dataset);
     idx:=ds.CurIndex;
     ref:=idx.References;
     fld:=AField.OriginatingField;

     ds.Lock;
     try
        case AFunction of
             safCOUNT:
                begin
                     Result:=ref.Count;
                end;
             safMIN:
                begin
                     res:=MaxDouble;
                     for i:=0 to ref.Count-1 do
                     begin
                          ds.OverrideActiveRecordBuffer:=PkbmRecord(ref.Items[i]);
                          val:=fld.AsFloat;
                          if val<res then
                             res:=val;
                     end;
                     Result:=res;
                end;
             safMAX:
                begin
                     res:=-MaxDouble;
                     for i:=0 to ref.Count-1 do
                     begin
                          ds.OverrideActiveRecordBuffer:=PkbmRecord(ref.Items[i]);
                          val:=fld.AsFloat;
                          if val>res then
                             res:=val;
                     end;
                     Result:=res;
                end;
             safAVG:
                begin
                     res:=0;
                     for i:=0 to ref.Count-1 do
                     begin
                          ds.OverrideActiveRecordBuffer:=PkbmRecord(ref.Items[i]);
                          val:=fld.AsFloat;
                          res:=res+val;
                     end;
                     Result:=res / ref.Count;
                end;
             safSUM:
                begin
                     res:=0;
                     for i:=0 to ref.Count-1 do
                     begin
                          ds.OverrideActiveRecordBuffer:=PkbmRecord(ref.Items[i]);
                          val:=fld.AsFloat;
                          res:=res+val;
                     end;
                     Result:=res;
                end;
        else
            raise Exception.Create('Specific aggregation not supported.');
        end;
     finally
        ds.OverrideActiveRecordBuffer:=nil;
        ds.Unlock;
     end;
end;
}

initialization
   kbmSQLDBAPIRegistrations.RegisterAPI(TkbmCustomMemTable,TkbmSQLMemTableAPI);

end.
