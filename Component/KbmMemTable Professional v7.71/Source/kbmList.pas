unit kbmList;

// =========================================================================
// kbmList - a hyper performance TList
//
// Copyright 1999-2014 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// LICENSE AGREEMENT
//
// This unit is not open source or freeware. Its part of the kbmMW package
// not the kbmMemTable package.
//
// All its contents is confidential, may not be discussed in public
// Any knowledge you may have gained from it may not be used for making
// products competing with C4D products.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com
// Further you must have read the license agreement in the top of this
// unit and accepted it.

interface

{$I kbmMemTable.inc}

//---------------------------------------------------------------
// Comment the next line to use less optimized list handling.
{$DEFINE USE_FAST_LIST}
//---------------------------------------------------------------

{$IFDEF LEVEL7}
 {$WARNINGS OFF}
{$ENDIF}

const
  kbmMaxListSize = MaxInt div 16;

type
  TkbmPointerArray = array[0..kbmMaxListSize-1] of Pointer;
  PkbmPointerArray = ^TkbmPointerArray;

{$IFNDEF USE_FAST_LIST}
  TkbmList = class
  private
     FList:PkbmPointerArray;
     FCount:integer;
     FCapacity:integer;
  protected
     function GetItem(const AIndex:integer):pointer;
     procedure SetItem(const AIndex:integer; const Aitem:pointer);
     procedure Grow;
     procedure SetCapacity(const ACapacity:integer);
  public
     destructor Destroy; override;
     function Add(const AItem:pointer):integer;
     procedure Clear;
     procedure Delete(const AIndex:integer);
     function IndexOf(const AItem:pointer):integer;
     procedure Insert(const AIndex:integer; const AItem:pointer);
     procedure Pack;
     property Capacity:integer read FCapacity write SetCapacity;
     property Count:integer read FCount;
     property Items[const AIndex:integer]:pointer read GetItem write SetItem; default;
     property List:PkbmPointerArray read FList;
  end;

{$ELSE}
  TkbmArrayList = record
     FArray:PkbmPointerArray;
     FCapacity:integer;
  end;
  PkbmArrayList = ^TkbmArrayList;
  TkbmArrayLists = array[0..$FFFFFF] of TkbmArrayList;
  PkbmArrayLists = ^TkbmArrayLists;
  TkbmArrayCount = word;
  PkbmArrayCount = ^TkbmArrayCount;
  TkbmArrayCounts = array[0..$FFFFFF] of TkbmArrayCount;
  PkbmArrayCounts = ^TkbmArrayCounts;

  TkbmList = class
  private
     // Sublists.
     FLists:PkbmArrayLists;

     // For speed reasons all sublist counts are in a seperate word array.
     FListCounts:PkbmArrayCounts;

     // Number of sublists.
     FListCount:integer;

     // Number of records totally in all sublists (=TList.Count).
     FTotalCount:integer;

     // Used for setting capacity.
     FCapacity:integer;

     // Half the size of TotalCount. Maintained for performance reasons.
     FHalfTotalCount:integer;

     FMaxSubListSize:integer;
  protected
     function GetItem(AIndex:integer):pointer;
     procedure SetItem(AIndex:integer; const Aitem:pointer);
     procedure Grow(const AList:PkbmArrayList);
     procedure SetCapacity(const ACapacity:integer);
     procedure SetListCapacity(const AList:PkbmArrayList; const ACapacity:integer);
  public
     constructor Create;
     destructor Destroy; override;
     function Add(const AItem:pointer):integer;
     procedure Clear;
     procedure Delete(AIndex:integer);
     function IndexOf(const AItem:pointer):integer;
     procedure Insert(AIndex:integer; const AItem:pointer);
     procedure Pack;
     property Capacity:integer read FCapacity write SetCapacity;
     property Count:integer read FTotalCount;
     property Items[AIndex:integer]:pointer read GetItem write SetItem; default;
  end;
{$ENDIF}

implementation

uses kbmMove;

const
   MAX_SUBLIST_SIZE = 2000;

// -----------------------------------------------------------------------------------
// TkbmList
// -----------------------------------------------------------------------------------
{$IFNDEF USE_FAST_LIST}
destructor TkbmList.Destroy;
begin
     Clear;
end;

function TkbmList.GetItem(const AIndex:integer):pointer;
begin
     Result:=FList^[AIndex];
end;

procedure TkbmList.SetItem(const AIndex:integer; const AItem:pointer);
begin
     FList^[AIndex]:=AItem;
end;

procedure TkbmList.Grow;
begin
     if FCapacity<1000 then
        SetCapacity(FCapacity+100)
     else if FCapacity<10000 then
         SetCapacity(FCapacity+1000)
     else
         SetCapacity(FCapacity+10000);
end;

procedure TkbmList.SetCapacity(const ACapacity:integer);
begin
     if ACapacity<>FCapacity then
     begin
          ReallocMem(FList,ACapacity*SizeOf(Pointer));
          FCapacity:=ACapacity;
     end;
end;

function TkbmList.Add(const AItem:pointer):integer;
begin
     Result:=FCount;
     if Result=FCapacity then Grow;
     FList^[Result]:=AItem;
     inc(FCount);
end;

procedure TkbmList.Clear;
begin
     SetCapacity(0);
     FCount:=0;
end;

procedure TkbmList.Delete(const AIndex:integer);
begin
     dec(FCount);
     if AIndex<FCount then
        kbmMemMove(FList^[AIndex+1],FList^[AIndex],(FCount-AIndex)*sizeof(pointer));
end;

function TkbmList.IndexOf(const AItem:pointer):integer;
var
   i:integer;
begin
     i:=0;
     while (i<FCount) and (FList^[i]<>AItem) do inc(i);
     if i=FCount then
        Result:=-1
     else
         Result:=i;
end;

procedure TkbmList.Insert(const AIndex:integer; const AItem:pointer);
begin
     if FCount=FCapacity then Grow;
     if AIndex<FCount then
        kbmMemMove(FList^[AIndex],FList^[AIndex+1],(FCount-AIndex)*sizeof(pointer));
     FList^[AIndex]:=AItem;
     inc(FCount);
end;

procedure TkbmList.Pack;
var
   i,j:integer;
begin
     j:=0;
     for i:=0 to FCount-1 do
     begin
          if FList^[i]<>nil then inc(j);
          FList^[j]:=FList^[i];
     end;
     FCount:=j;
end;
{$ELSE}

constructor TkbmList.Create;
begin
     inherited;
     FLists:=nil;
     FListCount:=0;
     FTotalCount:=0;
     FMaxSubListSize:=3000;
end;

destructor TkbmList.Destroy;
begin
     Clear;
end;

function TkbmList.GetItem(AIndex:integer):pointer;
var
   i{,n}:integer;
   pal:PkbmArrayList;
   pac:PkbmArrayCount;
begin
     // Check if only one sublist at the moment.
{     if FListCount=1 then
     begin
          pal:=@FLists[0];
          Result:=pal^.FArray^[AIndex];
          exit;
     end;
}
     // Check if to look for list forwards or backwards.
     if (AIndex<FHalfTotalCount) then
     begin
          i:=0;
          pac:=@FListCounts[0];
          while i<FListCount do
          begin
//               n:=pac^;
               if AIndex<pac^ then
               begin
                    pal:=@FLists[i];
                    Result:=pal^.FArray^[AIndex];
                    exit;
               end
               else
                   dec(AIndex,pac^);
               inc(pac);
               inc(i);
          end;
     end
     else
     begin
          i:=FListCount-1;
          AIndex:=FTotalCount-AIndex;
          pac:=@FListCounts[i];
          while i>=0 do
          begin
//               n:=pac^;
               if AIndex<=pac^ then
               begin
                    pal:=@FLists[i];
                    Result:=pal^.FArray^[pac^-AIndex];
                    exit;
               end
               else
                   dec(AIndex,pac^);
               dec(pac);
               dec(i);
          end;
     end;
     Result:=nil;
end;

procedure TkbmList.SetItem(AIndex:integer; const AItem:pointer);
var
   i,n:integer;
   pal:PkbmArrayList;
   pac:PkbmArrayCount;
begin
     // Check if only one sublist at the moment.
{     if FListCount=1 then
     begin
          pal:=@FLists[0];
          pal^.FArray^[AIndex]:=AItem;
          exit;
     end;
}

     // Check if to look for list forwards or backwards.
     if (AIndex>FHalfTotalCount) then
     begin
          i:=FListCount-1;
          pac:=@FListCounts[i];
          AIndex:=FTotalCount-AIndex;
          while i>=0 do
          begin
               n:=pac^;
               if AIndex<=n then
               begin
                    pal:=@FLists[i];
                    pal^.FArray^[n-AIndex]:=AItem;
                    exit;
               end
               else
                   dec(AIndex,n);
               dec(pac);
               dec(i);
          end;
     end
     else
     begin
          i:=0;
          pac:=@FListCounts[0];
          while i<FListCount do
          begin
               n:=pac^;
               if AIndex<n then
               begin
                    pal:=@FLists[i];
                    pal^.FArray^[AIndex]:=AItem;
                    exit;
               end
               else
                   dec(AIndex,n);
               inc(pac);
               inc(i);
          end;
     end;
end;

procedure TkbmList.Grow(const AList:PkbmArrayList);
begin
     with AList^ do
     begin
          if FCapacity<1000 then
             SetListCapacity(AList,FCapacity+100)
          else if FCapacity<10000 then
             SetListCapacity(AList,FCapacity+1000);
     end;
     if FTotalCount<=100000 then
        FMaxSubListSize:=2000
     else
        FMaxSubListSize:=5000;
end;

procedure TkbmList.SetListCapacity(const AList:PkbmArrayList; const ACapacity:integer);
begin
     with AList^ do
     begin
          if ACapacity<>FCapacity then
          begin
               ReallocMem(FArray,ACapacity*SizeOf(Pointer));
               FCapacity:=ACapacity;
          end;
     end;
end;

procedure TkbmList.SetCapacity(const ACapacity:integer);
begin
     FCapacity:=ACapacity;
     if FCapacity<FTotalCount then FCapacity:=FTotalCount;
     if FCapacity<=100000 then
        FMaxSubListSize:=3000
     else
        FMaxSubListSize:=6000;
end;

function TkbmList.Add(const AItem:pointer):integer;
var
   pal:PkbmArrayList;
   pac:PkbmArrayCount;
begin
     // Check if no array list defined.
     if FListCount=0 then
     begin
          ReallocMem(FListCounts,sizeof(TkbmArrayCount));
          ReallocMem(FLists,sizeof(TkbmArrayList));
          pac:=@FListCounts[0];
          pac^:=0;
          pal:=@FLists[0];
          pal^.FArray:=nil;
          pal^.FCapacity:=0;
          inc(FListCount);
     end
     else
     begin
          // Get the last sublist.
          pal:=@FLists[FListCount-1];
          pac:=@FListCounts[FListCount-1];
     end;

     // Check if to grow list.
     if pac^=pal^.FCapacity then
     begin
          // If too big allocate a new list.
          if pac^>=MAX_SUBLIST_SIZE then
          begin
               ReallocMem(FListCounts,(FListCount+1)*sizeof(TkbmArrayCount));
               ReallocMem(FLists,(FListCount+1)*sizeof(TkbmArrayList));
               pac:=@FListCounts[FListCount];
               pal:=@FLists[FListCount];

               pac^:=0;
               pal^.FArray:=nil;
               pal^.FCapacity:=0;
               inc(FListCount);
          end;

          // Grow sublist to have place for extra elements.
          Grow(pal);
     end;

     // Now add the item to the sublist.
     Result:=FTotalCount;
     pal^.FArray^[pac^]:=AItem;
     inc(pac^);
     inc(FTotalCount);
     FHalfTotalCount:=FTotalCount div 2;
end;

procedure TkbmList.Clear;
var
   i:integer;
   pal:PkbmArrayList;
begin
     pal:=@FLists[0];
     for i:=0 to FListCount-1 do
     begin
          SetListCapacity(pal,0);
          inc(pal);
     end;
     if FListCount>0 then
     begin
          ReallocMem(FListCounts,0);
          ReallocMem(FLists,0);
     end;
     FListCount:=0;
     FTotalCount:=0;
     FHalfTotalCount:=0;
end;

procedure TkbmList.Delete(AIndex:integer);
var
   i,n:integer;
   pal:PkbmArrayList;
   pac:PkbmArrayCount;
begin
     // Look for sublist.
     if (AIndex>FHalfTotalCount) then
     begin
          i:=FListCount-1;
          AIndex:=FTotalCount-AIndex;
          pac:=@FListCounts[i];
          while i>=0 do
          begin
               n:=pac^;
               if AIndex<=n then
               begin
                    AIndex:=n-AIndex;
                    break;
               end;

               dec(AIndex,n);
               dec(pac);
               dec(i);
          end
     end
     else
     begin
          i:=0;
          pac:=@FListCounts[0];
          while i<FListCount do
          begin
               n:=pac^;
               if AIndex<n then
                  break
               else
                   dec(AIndex,n);
               inc(pac);
               inc(i);
          end;
     end;
     pal:=@FLists[i];

     // Look for sublist.
     dec(FTotalCount);
     FHalfTotalCount:=FTotalCount div 2;
     dec(pac^);
     with pal^ do
     begin
          // If this sublist is empty then remove it from sublist list.
          if pac^=0 then
          begin
               // Clear the sublist.
               SetListCapacity(pal,0);

               // Check if only sublist, then simply clear out list.
               if FListCount=1 then
               begin
                    ReallocMem(FListCounts,0);
                    ReallocMem(FLists,0);
                    FListCount:=0;
                    FTotalCount:=0;
                    exit;
               end;

               // Rearrange the sublists.
               kbmMemMove(FLists^[i+1],FLists^[i],(FListCount-i)*sizeof(TkbmArrayList));
               kbmMemMove(FListCounts^[i+1],FListCounts^[i],(FListCount-i)*sizeof(TkbmArrayCount));
               dec(FListCount);
               ReallocMem(FLists,FListCount*sizeof(TkbmArrayList));
               ReallocMem(FListCounts,FListCount*sizeof(TkbmArrayCount));
               exit;
          end;

          // If it was not empty, shrink it.
          if AIndex<pac^ then
             kbmMemMove(FArray^[AIndex+1],FArray^[AIndex],(pac^-AIndex)*sizeof(pointer));
     end;
end;

function TkbmList.IndexOf(const AItem:pointer):integer;
var
   i,j,n,m:integer;
   pal:PkbmArrayList;
   pac:PkbmArrayCount;
begin
     // Look through all sublists.
     Result:=-1;
     n:=0;
     pal:=@FLists^[0];
     pac:=@FListCounts^[0];
     for i:=0 to FListCount-1 do
     begin
          m:=pac^;
          with pal^ do
          begin
               j:=0;
               while (j<m) and (FArray^[j]<>AItem) do inc(j);
               if j<m then
               begin
                    Result:=n+j;
                    exit;
               end;
               inc(n,j);
          end;
          inc(pal);
          inc(pac);
     end;
end;

procedure TkbmList.Insert(AIndex:integer; const AItem:pointer);
var
   i,n:integer;
   pal,pal2:PkbmArrayList;
   pac,pac2:PkbmArrayCount;
begin
     if AIndex>=FTotalCount then
     begin
          Add(AItem);
          exit;
     end;
     if AIndex<0 then AIndex:=0;

     // Check if no array list defined.
     if FListCount=0 then
     begin
          ReallocMem(FListCounts,sizeof(TkbmArrayCount));
          ReallocMem(FLists,sizeof(TkbmArrayList));
          pac:=@FListCounts[0];
          pal:=@FLists[0];

          pac^:=0;
          pal^.FArray:=nil;
          pal^.FCapacity:=0;
          inc(FListCount);
          i:=0;
     end
     else
     begin
          // Look for sublist.
          if (AIndex>FHalfTotalCount) then
          begin
               i:=FListCount-1;
               AIndex:=FTotalCount-AIndex;
               pac:=@FListCounts[i];
               while i>=0 do
               begin
                    n:=pac^;
                    if AIndex<=n then
                    begin
                         AIndex:=n-AIndex;
                         break;
                    end
                    else
                        dec(AIndex,n);
                    dec(pac);
                    dec(i);
               end
          end
          else
          begin
               i:=0;
               pac:=@FListCounts[0];
               while i<FListCount do
               begin
                    n:=pac^;
                    if AIndex<n then
                       break
                    else
                        dec(AIndex,n);
                    inc(pac);
                    inc(i);
               end;
          end;
          pal:=@FLists[i];
     end;

     // Check if to grow list.
     if pac^=pal^.FCapacity then
     begin
          // If too big, split sublist into two sublists around the insertion area.
          // Insert the new sublist before the old one.
          if pac^>=MAX_SUBLIST_SIZE then
          begin
               // Insert new sublist after the current sublist.
               ReallocMem(FListCounts,(FListCount+1)*sizeof(TkbmArrayCount));
               ReallocMem(FLists,(FListCount+1)*sizeof(TkbmArrayList));
               kbmMemMove(FListCounts^[i+1],FListCounts^[i+2],(FListCount-i-1)*sizeof(TkbmArrayCount));
               kbmMemMove(FLists^[i+1],FLists^[i+2],(FListCount-i-1)*sizeof(TkbmArrayList));
               inc(FListCount);

               // Clear the newly created sublist.
               pac2:=@FListCounts^[i+1];
               pal2:=@FLists^[i+1];

               pac2^:=0;
               pal2^.FArray:=nil;
               pal2^.FCapacity:=0;

               // Get access to the old list again.
               pac:=@FListCounts^[i];
               pal:=@FLists^[i];

               // Allocate room in the new list for all entries starting from the insertion point.
               n:=pac^-AIndex;
               SetListCapacity(pal2,n);

               // Move entries after the insertion place to the new list.
               kbmMemMove(pal^.FArray^[AIndex],pal2.FArray^[0],n*sizeof(pointer));

               // Recalculate number of entries in each list.
               pac2^:=n;
               pac^:=AIndex;

               // Add the new item to the old list where there is now room.
               pal^.FArray^[AIndex]:=AItem;
               inc(pac^);
               inc(FTotalCount);
               FHalfTotalCount:=FTotalCount div 2;
               exit;
          end
          else
              // Grow sublist to have place for extra elements.
              Grow(pal);
     end;

     // Now insert the item to the sublist.
     with pal^ do
     begin
          if AIndex<pac^ then
             kbmMemMove(FArray^[AIndex],FArray^[AIndex+1],(pac^-AIndex)*sizeof(pointer));
          FArray^[AIndex]:=AItem;
          inc(pac^);
     end;
     inc(FTotalCount);
     FHalfTotalCount:=FTotalCount div 2;
end;

procedure TkbmList.Pack;
//var
//   i,j:integer;
begin
{
     j:=0;
     for i:=0 to FCount-1 do
     begin
          if FList^[i]<>nil then inc(j);
          FList^[j]:=FList^[i];
     end;
     FCount:=j;
}
end;
{$ENDIF}

end.
