unit kbmSQLStdFunc;

// =========================================================================
// A kbmMemTable based SQL implementation.
//
// Copyright 2007-2011 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com

interface

{$I kbmMemTable.inc}

implementation

uses
   DB,
   Math,
   Classes,
   SysUtils,
   kbmMemSQL,
   kbmSQLParser,
   kbmSQLFuncAPI,
   kbmSQLElements
{$ifdef LEVEL6}
   ,Variants
{$endif}
   ;

// Math

function SQLSin(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Sin(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLCos(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Cos(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLTan(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Tan(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLLog(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Log10(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLLog2(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Log2(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLExp(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Exp(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLSqrt(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Sqrt(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLSqr(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Sqr(AResult);
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLRoot(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 v:=AArgs.Node[1].Execute;
                 if (not VarIsNull(AResult)) and (not VarIsNull(v)) then
                    AResult:=Power(AResult,1.0 / v)
                 else
                     AResult:=null;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLPow(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 v:=AArgs.Node[1].Execute;
                 if (not VarIsNull(AResult)) and (not VarIsNull(v)) then
                    AResult:=Power(AResult,v)
                 else
                     AResult:=null;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLTrunc(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Trunc(double(AResult));
            end;
        fsDataType:
            AResult:=ftLargeInt;
     end;
     Result:=true;
end;

function SQLFrac(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Frac(double(AResult));
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLMod(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 v:=AArgs.Node[1].Execute;
                 if (not VarIsNull(AResult)) and (not VarIsNull(v)) then
                    AResult:=AResult mod v
                 else
                     AResult:=null;
            end;
        fsDataType:
            AResult:=ftInteger;
     end;
     Result:=true;
end;

function SQLDiv(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 v:=AArgs.Node[1].Execute;
                 if (not VarIsNull(AResult)) and (not VarIsNull(v)) then
                    AResult:=AResult div v
                 else
                     AResult:=null;
            end;
        fsDataType:
            AResult:=ftInteger;
     end;
     Result:=true;
end;

function SQLAbs(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=abs(AResult);
            end;
        fsDataType:
            AResult:=AArgs.Node[0].DataType;
     end;
     Result:=true;
end;

function SQLMax(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   i:integer;
   r,d:double;
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1,true);
     r:=MinDouble;
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 for i:=0 to AArgs.Count-1 do
                 begin
                      v:=AArgs.Node[i].Execute;
                      if VarIsNull(v) then
                      begin
                           Result:=null;
                           exit;
                      end;
                      d:=v;
                      if d>r then
                         r:=d;
                 end;
                 AResult:=r;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLMin(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   i:integer;
   r,d:double;
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1,true);
     r:=MaxDouble;
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 for i:=0 to AArgs.Count-1 do
                 begin
                      v:=AArgs.Node[i].Execute;
                      if VarIsNull(v) then
                      begin
                           Result:=null;
                           exit;
                      end;
                      d:=v;
                      if d<r then
                         r:=d;
                 end;
                 AResult:=r;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLAvg(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   i:integer;
   r,d:double;
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1,true);
     r:=0;
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 for i:=0 to AArgs.Count-1 do
                 begin
                      v:=AArgs.Node[i].Execute;
                      if VarIsNull(v) then
                      begin
                           Result:=null;
                           exit;
                      end;
                      d:=v;
                      r:=r+d;
                 end;
                 AResult:=r / AArgs.Count;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLSum(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   i:integer;
   r,d:double;
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1,true);
     r:=0;
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 for i:=0 to AArgs.Count-1 do
                 begin
                      v:=AArgs.Node[i].Execute;
                      if VarIsNull(v) then
                      begin
                           Result:=null;
                           exit;
                      end;
                      d:=v;
                      r:=r+d;
                 end;
                 AResult:=r;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

// String

function SQLChr(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   i:integer;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=1;
        fsExecute:
          begin
               AResult:=AArgs.Node[0].Execute;
               if not VarIsNull(AResult) then
               begin
                    i:=AResult;
                    AResult:=Char(i);
               end;
          end;
        fsDataType:
            AResult:=ftWideString;
     end;
     Result:=true;
end;

function SQLUpper(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=UpperCase(AResult);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLLower(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=LowerCase(AResult);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLTrim(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Trim(AResult);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLLength(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=Length(AResult);
            end;
        fsDataType:
            AResult:=ftInteger;
     end;
     Result:=true;
end;

function SQLMid(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   s:string;
   p,l:integer;
   v1,v2,v3:variant;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,3);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=null;
                 v1:=AArgs.Node[0].Execute;
                 if VarIsNull(v1) then
                    exit;
                 v2:=AArgs.Node[1].Execute;
                 if VarIsNull(v2) then
                    exit;
                 v3:=AArgs.Node[2].Execute;
                 if VarIsNull(v3) then
                    exit;
                 s:=v1;
                 p:=v2;
                 l:=v3;
                 AResult:=Copy(s,p,l);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLLeft(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   s:string;
   l:integer;
   v1,v2:variant;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=null;
                 v1:=AArgs.Node[0].Execute;
                 if VarIsNull(v1) then
                    exit;
                 v2:=AArgs.Node[1].Execute;
                 if VarIsNull(v2) then
                    exit;
                 s:=v1;
                 l:=v2;
                 AResult:=Copy(s,1,l);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLRight(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   s:string;
   l,l1:integer;
   v1,v2:variant;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,2);
     case ASituation of
        fsWidth:
            AResult:=AArgs.Node[0].Width;
        fsExecute:
            begin
                 AResult:=null;
                 v1:=AArgs.Node[0].Execute;
                 if VarIsNull(v1) then
                    exit;
                 v2:=AArgs.Node[1].Execute;
                 if VarIsNull(v2) then
                    exit;
                 s:=v1;
                 l:=v2;
                 l1:=length(s);
                 AResult:=Copy(s,l1-l+1,l1);
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLLeftPad(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   s,c:string;
   l,l1:integer;
   v1,v2,v3:variant;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,3);
     case ASituation of
        fsWidth:
            begin
                 AResult:=AArgs.Node[0].Width;
                 l:=AArgs.Node[2].Execute;
                 if l>AResult then
                    AResult:=l;
            end;
        fsExecute:
            begin
                 AResult:=null;
                 v1:=AArgs.Node[0].Execute;
                 if VarIsNull(v1) then
                    exit;
                 v2:=AArgs.Node[1].Execute;
                 if VarIsNull(v2) then
                    exit;
                 v3:=AArgs.Node[2].Execute;
                 if VarIsNull(v3) then
                    exit;
                 s:=v1;
                 c:=v2;
                 l:=v3;
                 l1:=length(s);
                 if l>l1 then
                    AResult:=StringOfChar(c[1],l-l1)+s
                 else
                     AResult:=s;
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLRightPad(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   s,c:string;
   l,l1:integer;
   v1,v2,v3:variant;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,3);
     case ASituation of
        fsWidth:
            begin
                 AResult:=AArgs.Node[0].Width;
                 l:=AArgs.Node[2].Execute;
                 if l>AResult then
                    AResult:=l;
            end;
        fsExecute:
            begin
                 AResult:=null;
                 v1:=AArgs.Node[0].Execute;
                 if VarIsNull(v1) then
                    exit;
                 v2:=AArgs.Node[1].Execute;
                 if VarIsNull(v2) then
                    exit;
                 v3:=AArgs.Node[2].Execute;
                 if VarIsNull(v3) then
                    exit;
                 s:=v1;
                 c:=v2;
                 l:=v3;
                 l1:=length(s);
                 if l>l1 then
                    AResult:=s+StringOfChar(c[1],l-l1)
                 else
                     AResult:=s;
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

// Date/time

function SQLNow(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,0);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            AResult:=now;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLDate(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                    AResult:=trunc(double(AResult));
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLTime(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 AResult:=AResult-trunc(double(AResult));
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLYear(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   year,month,day:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeDate(AResult,year,month,day);
                      AResult:=year;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLMonth(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   year,month,day:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeDate(AResult,year,month,day);
                      AResult:=month;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLDay(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   year,month,day:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeDate(AResult,year,month,day);
                      AResult:=day;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLHour(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   hour,minute,sec,msec:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeTime(AResult,hour,minute,sec,msec);
                      AResult:=hour;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLMinute(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   hour,minute,sec,msec:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeTime(AResult,hour,minute,sec,msec);
                      AResult:=minute;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLSecond(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   hour,minute,sec,msec:word;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 AResult:=AArgs.Node[0].Execute;
                 if not VarIsNull(AResult) then
                 begin
                      DecodeTime(AResult,hour,minute,sec,msec);
                      AResult:=sec;
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;


function SQLTimeString(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
   s:string;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,1,false);
     case ASituation of
        fsWidth:
            begin
                 case AArgs.Node[0].DataType of
                     ftDate,
                     ftDateTime,
                     ftTimeStamp,
                     ftFloat:
                       begin
                         s:=TimeToStr(now
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                                         );
                         AResult:=length(s);
                         Result:=true;
                       end;
                 end;
            end;
        fsExecute:
            begin
                 v:=AArgs.Node[0].Execute;
                 case VarType(v) of
                      varDate,
                      varDouble,
                      varSingle:
                        begin
                             AResult:=TimeToStr(v
{$IFDEF LEVEL9}
                                ,AOperation.FormatSettings
{$ENDIF}
                                );
                             Result:=true;
                        end;
                 end;
            end;
        fsDataType:
            begin
                 AResult:=ftString;
                 Result:=true;
            end;
     end;
end;

function SQLDateString(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
   s:string;
begin
     Result:=false;
     kbmSQLCheckArgs(AArgs,1,false);
     case ASituation of
        fsWidth:
            begin
                 case AArgs.Node[0].DataType of
                     ftDate,
                     ftDateTime,
                     ftTimeStamp,
                     ftFloat:
                       begin
                         s:=DateToStr(now
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                                         );
                         AResult:=length(s);
                         Result:=true;
                       end;
                 end;
            end;
        fsExecute:
            begin
                 v:=AArgs.Node[0].Execute;
                 case VarType(v) of
                      varDate,
                      varDouble,
                      varSingle:
                        begin
                             AResult:=DateToStr(v
{$IFDEF LEVEL9}
                                ,AOperation.FormatSettings
{$ENDIF}
                                );
                             Result:=true;
                        end;
                 end;
            end;
        fsDataType:
            begin
                 AResult:=ftString;
                 Result:=true;
            end;
     end;
end;

function SQLCastToNumber(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 v:=AArgs.Node[0].Execute;
                 case VarType(v) of
                      varString,
                      varOleStr
{$IFDEF LEVEL13}
                      ,varUString
{$ENDIF}
                      :
                        AResult:=strtofloat(v
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                           );
                 else
                     AResult:=v;
                 end;
            end;
        fsDataType:
            AResult:=ftFloat;
     end;
     Result:=true;
end;

function SQLCastToDateTime(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            begin
                 v:=AArgs.Node[0].Execute;
                 case VarType(v) of
                      varString,
                      varOleStr
{$IFDEF LEVEL13}
                      ,varUString
{$ENDIF}
                      :
                        AResult:=StrToDateTime(string(v)
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                           );
                      varNull:
                         AResult:=v;
                 else
                     AResult:=VarToDateTime(v);
                 end;
            end;
        fsDataType:
            AResult:=ftDateTime;
     end;
     Result:=true;
end;

function SQLCastToString(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   v:variant;
   s:string;
begin
     kbmSQLCheckArgs(AArgs,1,true);
     case ASituation of
        fsWidth:
            begin
                 if AArgs.Count>1 then
                 begin
                      AResult:=AArgs.Node[1].Execute;
                      Result:=true;
                      exit;
                 end;

                 case AArgs.Node[0].DataType of
                     ftDate,
                     ftDateTime,
                     ftTimeStamp:
                       begin
                         s:=DateTimeToStr(now
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                                         );
                         AResult:=length(s);
                       end;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                     ftLongWord,
{$ENDIF}
                     ftInteger:
                       AResult:=length(IntToStr(MaxInt));
                     ftWord:
                       AResult:=5;
                     ftLargeint:
                       AResult:=length(IntToStr(MaxInt))*2;
                     ftMemo,
                     ftFmtMemo,
                     ftString:
                       AResult:=AArgs.Node[0].Width;
                     ftGUID:
                       AResult:=38;
                 else
                     AResult:=10;
                 end;
            end;
        fsExecute:
            begin
                 v:=AArgs.Node[0].Execute;
                 case VarType(v) of
                      varDate:
                        AResult:=DateTimeToStr(v
{$IFDEF LEVEL9}
                           ,AOperation.FormatSettings
{$ENDIF}
                           );
                      varNull:
                         AResult:=v;
                 else
                     AResult:=VarToStr(v);
                 end;
            end;
        fsDataType:
            AResult:=ftString;
     end;
     Result:=true;
end;

function SQLIf(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
var
   vCondition:variant;
   bCondition:boolean;
begin
     kbmSQLCheckArgs(AArgs,3,true);
     case ASituation of
        fsWidth:
           begin
                AResult:=Max(AArgs.Node[1].Width,AArgs.Node[2].Width);
           end;
        fsExecute:
           begin
                vCondition:=AArgs.Node[0].Execute;
                if VarIsNull(vCondition) then
                begin
                     if AArgs.Count>=4 then
                        AResult:=AArgs.Node[3].Execute
                     else
                         AResult:=null;
                end
                else
                begin
                     bCondition:=vCondition; // Make sure its downtrimmed instead of uptrimmed to comparing two floating point values.
                     if bCondition then
                        AResult:=AArgs.Node[1].Execute
                     else
                         AResult:=AArgs.Node[2].Execute;
                end;
           end;
        fsDataType:
           begin
                AResult:=kbmSQLMergeDataTypes(AArgs.Node[1].DataType,AArgs.Node[2].DataType);
           end;

     end;
     Result:=true;
end;

function SQLIsNull(const AOperation:TkbmSQLCustomOperation; const ASituation:TkbmSQLFunctionSituation; const AArgs:TkbmSQLNodes; var AResult:variant):boolean;
begin
     kbmSQLCheckArgs(AArgs,1);
     case ASituation of
        fsWidth:
            AResult:=0;
        fsExecute:
            AResult:=VarIsNull(AArgs.Node[0].Execute);
        fsDataType:
            AResult:=ftBoolean;
     end;
     Result:=true;
end;

initialization

   kbmSQLFunctionRegistrations.RegisterFunction('MATH.TRIG','SIN',@SQLSin);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH.TRIG','COS',@SQLCos);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH.TRIG','TAN',@SQLTan);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH.LOG','LOG',@SQLLog);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH.LOG','LOG2',@SQLLog2);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH.LOG','EXP',@SQLExp);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','TRUNC',@SQLTrunc);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','FRAC',@SQLFrac);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','MOD',@SQLMod);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','DIV',@SQLDiv);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','SQRT',@SQLSqrt);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','SQR',@SQLSqr);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','ROOT',@SQLRoot);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','MIN',@SQLMin);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','MAX',@SQLMax);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','AVG',@SQLAvg);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','SUM',@SQLSum);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','ABS',@SQLAbs);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','POW',@SQLPow);
   kbmSQLFunctionRegistrations.RegisterFunction('MATH','ROOT',@SQLRoot);

   kbmSQLFunctionRegistrations.RegisterFunction('STRING','UPPER',@SQLUpper);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','LOWER',@SQLLower);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','TRIM',@SQLTrim);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','MID',@SQLMid);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','LEFT',@SQLLeft);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','RIGHT',@SQLRight);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','LENGTH',@SQLLength);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','LEFTPAD',@SQLLeftPad);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','RIGHTPAD',@SQLRightPad);
   kbmSQLFunctionRegistrations.RegisterFunction('STRING','CHR',@SQLChr);

   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','NOW',@SQLNow);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','DATE',@SQLDate);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','TIME',@SQLTime);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','YEAR',@SQLYear);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','MONTH',@SQLMonth);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','DAY',@SQLDay);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','HOURS',@SQLHour);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','MINUTES',@SQLMinute);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','SECONDS',@SQLSecond);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','DATESTRING',@SQLDateString);
   kbmSQLFunctionRegistrations.RegisterFunction('DATETIME','TIMESTRING',@SQLTimeString);

   kbmSQLFunctionRegistrations.RegisterFunction('CAST','CASTTODATETIME',@SQLCastToDateTime);
   kbmSQLFunctionRegistrations.RegisterFunction('CAST','CASTTOSTRING',@SQLCastToString);
   kbmSQLFunctionRegistrations.RegisterFunction('CAST','CASTTONUMBER',@SQLCastToNumber);

   kbmSQLFunctionRegistrations.RegisterFunction('CONDITIONAL','IF',@SQLIf);
   kbmSQLFunctionRegistrations.RegisterFunction('CONDITIONAL','ISNULL',@SQLIsNull);
end.