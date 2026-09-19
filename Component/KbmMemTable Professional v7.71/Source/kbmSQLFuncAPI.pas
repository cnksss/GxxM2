unit kbmSQLFuncAPI;

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

uses
    DB,
    Classes,
{$IFDEF NEXTGEN}
    System.Generics.Collections,
{$ENDIF}
    kbmSQLElements;

{$WARN UNSAFE_CAST OFF}

type
   TkbmSQLFunctionRegistration = class
   private
      FGroup:string;
      FFunction:TkbmSQLCustomFunction;
      FFunctionName:string;
      FEnabled:boolean;
   public
      constructor Create(AGroup:string; AFunctionName:string; AFunction:TkbmSQLCustomFunction; AEnabled:boolean = true); virtual;
      destructor Destroy; override;
      property FunctionName:string read FFunctionName;
      property Group:string read FGroup;
      property Enabled:boolean read FEnabled write FEnabled;
      property _Function:TkbmSQLCustomFunction read FFunction;
   end;

   TkbmSQLFunctionRegistrations = class
   private
{$IFDEF NEXTGEN}
      FList:TList<TkbmSQLFunctionRegistration>;
{$ELSE}
      FList:TList;
{$ENDIF}
   public
      constructor Create; virtual;
      destructor Destroy; override;

      procedure Clear;
      procedure RegisterFunction(AGroup:string; AFunctionName:string; AFunction:TkbmSQLCustomFunction);
      procedure DisableGroup(AGroup:string);
      procedure EnableGroup(AGroup:string);
      procedure DisableFunction(AFunctionName:string);
      procedure EnableFunction(AFunctionName:string);
      function GetFunctionReg(AFunctionName:string):TkbmSQLFunctionRegistration;
   end;

   procedure kbmSQLCheckArgs(const AArgs:TkbmSQLNodes; const ACnt:integer; const AMinimum:boolean = false);

var
   kbmSQLFunctionRegistrations:TkbmSQLFunctionRegistrations;

implementation

uses
   SysUtils;

procedure kbmSQLCheckArgs(const AArgs:TkbmSQLNodes; const ACnt:integer; const AMinimum:boolean = false);
var
   n:integer;
begin
     n:=AArgs.Count;
     if AMinimum then
     begin
          if n<ACnt then
             raise Exception.Create('Invalid number of arguments. Expected minimum '+inttostr(ACnt));
     end
     else
         if n<>ACnt then
            raise Exception.Create('Invalid number of arguments. Expected '+inttostr(ACnt));
end;

constructor TkbmSQLFunctionRegistration.Create(AGroup:string; AFunctionName:string; AFunction:TkbmSQLCustomFunction; AEnabled:boolean = true);
begin
     inherited Create;
     FGroup:=UpperCase(AGroup);
     FFunctionName:=UpperCase(AFunctionName);
     FFunction:=AFunction;
     FEnabled:=AEnabled;
end;

destructor TkbmSQLFunctionRegistration.Destroy;
begin
     inherited Destroy;
end;

constructor TkbmSQLFunctionRegistrations.Create;
begin
     inherited Create;
{$IFDEF NEXTGEN}
     FList:=TList<TkbmSQLFunctionRegistration>.Create;
{$ELSE}
     FList:=TList.Create;
{$ENDIF}
end;

destructor TkbmSQLFunctionRegistrations.Destroy;
begin
     Clear;
     FList.Free;
     inherited Destroy;
end;

procedure TkbmSQLFunctionRegistrations.Clear;
{$IFNDEF NEXTGEN}
var
   i:integer;
{$ENDIF}
begin
{$IFNDEF NEXTGEN}
     for i:=0 to FList.Count-1 do
         TkbmSQLFunctionRegistration(FList.Items[i]).Free;
{$ENDIF}
     FList.Clear;
end;

procedure TkbmSQLFunctionRegistrations.RegisterFunction(AGroup:string; AFunctionName:string; AFunction:TkbmSQLCustomFunction);
var
   reg:TkbmSQLFunctionRegistration;
begin
     reg:=GetFunctionReg(AFunctionName);
     if reg=nil then
        FList.Add(TkbmSQLFunctionRegistration.Create(AGroup,AFunctionName,AFunction));
end;

procedure TkbmSQLFunctionRegistrations.DisableGroup(AGroup:string);
var
   reg:TkbmSQLFunctionRegistration;
   i,l:integer;
begin
     AGroup:=UpperCase(AGroup);
     l:=Length(AGroup);
     for i:=0 to FList.Count-1 do
     begin
          reg:=TkbmSQLFunctionRegistration(FList.Items[i]);
          if copy(reg.FGroup,1,l) = AGroup then
             reg.FEnabled:=false;
     end;
end;

procedure TkbmSQLFunctionRegistrations.EnableGroup(AGroup:string);
var
   reg:TkbmSQLFunctionRegistration;
   i,l:integer;
begin
     AGroup:=UpperCase(AGroup);
     l:=Length(AGroup);
     for i:=0 to FList.Count-1 do
     begin
          reg:=TkbmSQLFunctionRegistration(FList.Items[i]);
          if copy(reg.FGroup,1,l) = AGroup then
             reg.FEnabled:=true;
     end;
end;

procedure TkbmSQLFunctionRegistrations.DisableFunction(AFunctionName:string);
var
   reg:TkbmSQLFunctionRegistration;
begin
     reg:=GetFunctionReg(AFunctionName);
     if reg=nil then
        raise Exception.Create('Function '+AFunctionName+' not registered.');
     reg.FEnabled:=false;
end;

procedure TkbmSQLFunctionRegistrations.EnableFunction(AFunctionName:string);
var
   reg:TkbmSQLFunctionRegistration;
begin
     reg:=GetFunctionReg(AFunctionName);
     if reg=nil then
        raise Exception.Create('Function '+AFunctionName+' not registered.');
     reg.FEnabled:=true;
end;

function TkbmSQLFunctionRegistrations.GetFunctionReg(AFunctionName:string):TkbmSQLFunctionRegistration;
var
   i:integer;
begin
     AFunctionName:=UpperCase(AFunctionName);
     for i:=0 to FList.Count-1 do
     begin
          Result:=TkbmSQLFunctionRegistration(FList.Items[i]);
          if Result.FFunctionName = AFunctionName then
             exit;
     end;
     Result:=nil;
end;


initialization
   kbmSQLFunctionRegistrations:=TkbmSQLFunctionRegistrations.Create;

finalization
   kbmSQLFunctionRegistrations.Free;

end.
