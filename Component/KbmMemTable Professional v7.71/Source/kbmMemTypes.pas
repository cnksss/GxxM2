unit kbmMemTypes;

// =========================================================================
// kbmMemTypes - Types.
//
// Copyright 1999-2013 Kim Bo Madsen/Components4Developers DK
// All rights reserved.

interface

{$I kbmMemTable.inc}

uses DB
{$IFDEF DOTNET}
  ,System.Runtime.InteropServices
{$ENDIF}
  ;

type
  PkbmRecord    = ^TkbmRecord;

{$IFDEF LEVEL16}
  TkbmNativeInt = nativeint;
  TkbmNativeUInt = nativeuint;
{$ELSE}
  TkbmNativeInt = integer;
  TkbmNativeUInt = cardinal;
{$ENDIF}
  PkbmNativeInt = ^TkbmNativeInt;
  PkbmNativeUInt = ^TkbmNativeUInt;

{$IFDEF LEVEL23} // XE8
  TkbmLongInt = FixedInt;
{$ELSE}
  TkbmLongInt = longint;
{$ENDIF}
  PkbmLongInt = ^TkbmLongInt;

  TkbmRecord=record
{$IFDEF DO_CHECKRECORD}
      StartIdent:TkbmNativeInt;
{$ENDIF}

      RecordNo: TkbmNativeInt;      // Will be set on every single getrecord call.
      RecordID: TkbmNativeInt;
      UniqueRecordID: TkbmNativeInt;

      Flag:byte;              // Record flags.
      UpdateStatus:TUpdateStatus;

      TransactionLevel:integer;
      Tag:TkbmNativeInt;
      PrevRecordVersion:PkbmRecord;

      // Data starts at place pointed at by data, right after the end of TkbmRecord.
      Data:PByte;

{$IFDEF DO_CHECKRECORD}
      EndIdent:TkbmNativeInt;
{$ENDIF}
  end;

{
  Internal Data layout:
+------------+------------------------+-----------------------+------------------+----------------------+
| TkbmRecord | FIXED LENGTH DATA      | CALCULATED FIELDS     |Bookmark arrays   | VARIABLE LENGTH PTRS |
|            | FFixedRecordSize bytes | FCalcRecordSize bytes |FBookmarkArraySize| FVarLengthRecordSize |
+------------+------------------------+-----------------------+------------------+----------------------+
             ^                        ^                       ^                  ^
             GetFieldPointer          StartCalculated         StartBookmarks     StartVarLength

Blobsfields in the internal buffer are pointers to the blob data.
}

{$IFNDEF FPC}
  PDateTimeRec=^TDateTimeRec;
  PWordBool=^WordBool;
{$ENDIF}

  TkbmDataEventInfo = TkbmNativeInt;
{$IFDEF NEXTGEN}
  PkbmInternalAddRecord = TRecBuf;
  PkbmInternalInitRecord = TRecBuf;
  PkbmInternalSetToRecord = TRecBuf;
  PkbmAllocRecordBuffer = TRecBuf;
  PkbmFreeRecordBuffer = TRecBuf;
  PkbmSetFieldData = Pointer;
  PkbmGetRecord = TRecBuf;
  PkbmRecordBuffer = TRecBuf;
  PkbmGetBookmarkFlag = TRecBuf;
  PkbmSetBookmarkFlag = TRecBuf;
  PkbmGetBookmarkData = TRecBuf;
  PkbmSetBookmarkData = TRecBuf;
  PkbmInternalBookmarkValid = TBookmark;
  PkbmInternalGotoBookmark = TBookmark;
  PkbmClearCalcFields = TRecBuf;
  PkbmGetCalcFields = TRecBuf;
  PkbmCalculateFields = TRecBuf;
  PBookmarkFlag = ^TBookmarkFlag;
  PSetFieldDataBuffer = TValueBuffer;
  PGetFieldDataBuffer = TValueBuffer;
{$ELSE}
 {$IFDEF FPC}
  PkbmInternalAddRecord = Pointer;
  PkbmInternalInitRecord = TRecordBuffer;
  PkbmInternalSetToRecord = TRecordBuffer;
  PkbmAllocRecordBuffer = TRecordBuffer;
  PkbmFreeRecordBuffer = TRecordBuffer;
  PkbmSetFieldData = Pointer;
  PkbmGetRecord = TRecordBuffer;
  PkbmRecordBuffer = TRecordBuffer;
  PkbmGetBookmarkFlag = TRecordBuffer;
  PkbmSetBookmarkFlag = TRecordBuffer;
  PkbmGetBookmarkData = TRecordBuffer;
  PkbmSetBookmarkData = TRecordBuffer;
  PkbmInternalBookmarkValid = Pointer;
  PkbmInternalGotoBookmark = Pointer;
  PkbmClearCalcFields = TRecordBuffer;
  PkbmGetCalcFields = TRecordBuffer;
  PkbmCalculateFields = TRecordBuffer;
  PBookmarkFlag = ^TBookmarkFlag;
  PSetFieldDataBuffer = pointer;
  PGetFieldDataBuffer = pointer;
 {$ELSE}
  {$IFDEF LEVEL17}
  PkbmInternalAddRecord = TRecordBuffer;
  {$ELSE}
  PkbmInternalAddRecord = Pointer;
  {$ENDIF}
  {$IFDEF LEVEL18}
  PkbmCalculateFields = NativeInt;
  PkbmGetCalcFields = NativeInt;
  PkbmClearCalcFields = NativeInt;
  PkbmGetRecord = NativeInt;
  {$ELSE}
  PkbmCalculateFields = PByte;
  PkbmGetCalcFields = PByte;
  PkbmClearCalcFields = PByte;
  PkbmGetRecord = TRecordBuffer;
  {$ENDIF}
  PkbmInternalInitRecord = TRecordBuffer;
  PkbmInternalSetToRecord = TRecordBuffer;
  PkbmAllocRecordBuffer = TRecordBuffer;
  PkbmFreeRecordBuffer = TRecordBuffer;
  PkbmRecordBuffer = PByte;
  PkbmGetBookmarkFlag = TRecordBuffer;
  PkbmSetBookmarkFlag = TRecordBuffer;
  PkbmGetBookmarkData = TRecordBuffer;
  PkbmSetBookmarkData = TRecordBuffer;
  PkbmInternalBookmarkValid = Pointer;
  PkbmInternalGotoBookmark = Pointer;
  PBookmarkFlag = ^TBookmarkFlag;
  {$IFDEF LEVEL17}
  PSetFieldDataBuffer = TValueBuffer;
  PGetFieldDataBuffer = TValueBuffer;
  {$ELSE}
  PSetFieldDataBuffer = pointer;
  PGetFieldDataBuffer = pointer;
  {$ENDIF}
 {$ENDIF}
{$ENDIF}

implementation

end.
