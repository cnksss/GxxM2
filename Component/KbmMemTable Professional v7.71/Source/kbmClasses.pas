unit kbmClasses;

// =========================================================================
// kbmMemTypes - Types.
//
// Copyright 1999-2014 Kim Bo Madsen/Components4Developers DK
// All rights reserved.

{$IFDEF FPC}
 {$I kbmFPC.inc}
{$ENDIF}

interface

uses
  Classes, SysUtils
{$IFDEF _FPC}
  ,LCLProc
  ,fgl
{$ENDIF}
  ;

{$IFDEF _FPC}
type
  TDictionary<TKey,TData> = class(TFPGMap<TKey,TData>)
  public
     function TryGetValue(const Key:TKey; out Value:TData):boolean;
  end;
{$ENDIF}

implementation

{$IFDEF _FPC}
function TDictionary<TKey,TData>.TryGetValue(const Key:TKey; out Value:TData):boolean;
var
   i:integer;
s:string;
begin
s:='';
for i:=0 to Count-1 do
begin
     s:=s+','+Keys[i];
end;
     i:=IndexOf(UpperCase(Key));
     Result:=i>=0;
     if Result then
        Value:=Data[i];
end;
{$ENDIF}

end.

