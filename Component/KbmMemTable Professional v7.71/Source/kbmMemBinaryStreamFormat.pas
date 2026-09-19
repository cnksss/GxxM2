unit kbmMemBinaryStreamFormat;

interface

{$include kbmMemTable.inc}

// =========================================================================
// Binary stream format for kbmMemTable
//
// Copyright 1999-2013 Kim Bo Madsen/Components4Developers
// All rights reserved.
//
// Please refer to kbmMemTable.pas for license agreement.
//
// History.
// Per v. 3.00, the stream formats will each have their own history.
//
// 3.00a alpha
//       Initial v. 3.00 binary stream format release based on the sources of v. 2.53b.
//
// 3.00b beta
//       Fixed Floating point error in calculation of progress.
//       Bug reported by Fred Schetterer (yahoogroups@shaw.ca)
//
// 3.00c beta
//       Fixed bug not setting record flag to record being part of table.
//       This would result in massive memory leaks.
//
// 3.00  Final
//       Added BufferSize property (default 16384) which controls the internal
//       read and write buffer size. Suggested by Ken Schafer (prez@write-brain.com)

uses
  kbmMemTable,
  kbmMemTypes,
  Classes,DB,
{$include kbmMemRes.inc}
  SysUtils;

type
  TkbmStreamFlagUsingIndex  = (sfSaveUsingIndex);
  TkbmStreamFlagUsingIndexs = set of TkbmStreamFlagUsingIndex;

  TkbmStreamFlagDataTypeHeader = (sfSaveDataTypeHeader,sfLoadDataTypeHeader);
  TkbmStreamFlagDataTypeHeaders = set of TkbmStreamFlagDataTypeHeader;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCustomBinaryStreamFormat = class(TkbmCustomStreamFormat)
  private
     Writer:TWriter;
     Reader:TReader;

     FUsingIndex:TkbmStreamFlagUsingIndexs;
     FDataTypeHeader:TkbmStreamFlagDataTypeHeaders;
     FBuffSize:LongInt;

     FileVersion:integer;
     InitIndexDef:boolean;

     ProgressCnt:integer;
     StreamSize:longint;
     procedure SetBuffSize(ABuffSize:LongInt);
  protected
     function GetVersion:string; override;

     procedure BeforeSave(ADataset:TkbmCustomMemTable); override;
     procedure SaveDef(ADataset:TkbmCustomMemTable); override;
     procedure SaveData(ADataset:TkbmCustomMemTable); override;
     procedure AfterSave(ADataset:TkbmCustomMemTable); override;

     procedure BeforeLoad(ADataset:TkbmCustomMemTable); override;
     procedure LoadDef(ADataset:TkbmCustomMemTable); override;
     procedure LoadData(ADataset:TkbmCustomMemTable); override;
     procedure AfterLoad(ADataset:TkbmCustomMemTable); override;

     procedure DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation); override;
     
     property sfUsingIndex:TkbmStreamFlagUsingIndexs read FUsingIndex write FUsingIndex;
     property sfDataTypeHeader:TkbmStreamFlagDataTypeHeaders read FDataTypeHeader write FDataTypeHeader;
     property BufferSize:LongInt read FBuffSize write SetBuffSize;
  public
     constructor Create(AOwner:TComponent); override;
  end;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmBinaryStreamFormat = class(TkbmCustomBinaryStreamFormat)
  published
     property Version;
     property sfUsingIndex;
     property sfData;
     property sfCalculated;
     property sfLookup;
     property sfNonVisible;
     property sfBlobs;
     property sfDef;
     property sfIndexDef;
     property sfFiltered;
     property sfIgnoreRange;
     property sfIgnoreMasterDetail;
     property sfDeltas;
     property sfDontFilterDeltas;
     property sfAppend;
     property sfFieldKind;
     property sfFromStart;
     property sfDataTypeHeader;
     property sfDisplayWidth;

     property OnBeforeLoad;
     property OnAfterLoad;
     property OnBeforeSave;
     property OnAfterSave;
     property OnCompress;
     property OnDeCompress;

     property BufferSize;
  end;

implementation

const
  // Binary file magic word.
  kbmBinaryMagic = '@@BINARY@@';

  // Current file versions. V. 1.xx file versions are considered 100, 2.xx are considered 2xx etc.
  kbmBinaryFileVersion = 400;
  kbmDeltaVersion = 200;

type
  TkbmProtCustomMemTable = class(TkbmCustomMemTable);
  TkbmProtCommon = class(TkbmCommon);

function TkbmCustomBinaryStreamFormat.GetVersion:string;
begin
     Result:='4.00';
end;

constructor TkbmCustomBinaryStreamFormat.Create(AOwner:TComponent);
begin
     inherited;
     FUsingIndex:=[sfSaveUsingIndex];
     FDataTypeHeader:=[sfSaveDataTypeHeader,sfLoadDataTypeHeader];
     FBuffSize:=16384;
end;

procedure TkbmCustomBinaryStreamFormat.SetBuffSize(ABuffSize:LongInt);
begin
     if ABuffSize<16384 then ABuffSize:=16384;
     FBuffSize:=ABuffSize;
end;

procedure TkbmCustomBinaryStreamFormat.BeforeSave(ADataset:TkbmCustomMemTable);
begin
     inherited;

     Writer:=TWriter.Create(WorkStream,FBuffSize);
{$IFDEF FPC}
     Writer.Write(FilerSignature, SizeOf(FilerSignature));
{$ELSE}
     Writer.WriteSignature;
{$ENDIF}

{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
     Writer.WriteInteger(kbmBinaryFileVersion);
{$ENDIF}
end;

procedure TkbmCustomBinaryStreamFormat.AfterSave(ADataset:TkbmCustomMemTable);
begin
{$IFNDEF FPC}
     Writer.FlushBuffer;
{$ENDIF}
     Writer.Free;
     Writer:=nil;
     ADataset.OverrideActiveRecordBuffer:=nil;
     inherited;
end;

procedure TkbmCustomBinaryStreamFormat.SaveDef(ADataset:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
   nf:TkbmNativeInt;
begin
     // Write fielddefinitions.
     nf:=ADataSet.FieldCount;

     Writer.WriteListBegin;
     if (sfSaveDef in sfDef) then
     begin
          for i:=0 to nf-1 do
          begin
               if SaveFields[i]>=0 then
               begin
                    Writer.WriteString(ADataSet.Fields[i].FieldName);
                    Writer.WriteString(FieldTypeNames[ADataSet.Fields[i].DataType]);
                    Writer.WriteInteger(ADataSet.Fields[i].Size);
                    Writer.WriteString(ADataSet.Fields[i].DisplayName);
{$IFDEF FPC}
                    Writer.WriteString('');
{$ELSE}
                    Writer.WriteString(ADataSet.Fields[i].EditMask);
{$ENDIF}
                    Writer.WriteInteger(ADataSet.Fields[i].DisplayWidth);
                    Writer.WriteBoolean(ADataSet.Fields[i].Required);
                    Writer.WriteBoolean(ADataSet.Fields[i].ReadOnly);

                    // New for 2.50i BinaryFileVersion 250
                    if sfSaveFieldKind in sfFieldKind then
                       Writer.WriteString(FieldKindNames[ord(ADataSet.Fields[i].FieldKind)])
                    else
                        Writer.WriteString(FieldKindNames[0]); //fkData.

                    // New for 2.50o2 BinaryFileVersion 251
                    Writer.WriteString(ADataSet.Fields[i].DefaultExpression);
               end;
          end;
     end;
     Writer.WriteListEnd;

{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
     // Save index definitions.
     Writer.WriteListBegin;
     if sfSaveIndexDef in sfIndexDef then
     begin
          for i:=0 to ADataSet.IndexDefs.Count-1 do
              with ADataSet.IndexDefs.Items[i] do
              begin
                   Writer.WriteString(Name);
                   Writer.WriteString(Fields);
                   Writer.WriteString(DisplayName);
                   Writer.WriteBoolean(ixDescending in Options);
                   Writer.WriteBoolean(ixCaseInSensitive in Options);
                   Writer.WriteBoolean(ixNonMaintained in Options);
                   Writer.WriteBoolean(ixUnique in Options);
              end;
     end;
     Writer.WriteListEnd;
{$ENDIF}
end;

procedure TkbmCustomBinaryStreamFormat.SaveData(ADataset:TkbmCustomMemTable);
var
   i,j,cnt:TkbmNativeInt;
   nf:TkbmNativeInt;
   Accept:boolean;
   NewestVersion:boolean;
   pRec:PkbmRecord;
   UsingIndex:boolean;
begin
     // Write fielddefinitions.
     nf:=ADataSet.FieldCount;

     // Write datatypes as a kind of header.
     if sfSaveDataTypeHeader in sfDataTypeHeader then
     begin
          // Count number of fields actually saved.
          j:=0;
          for i:=0 to nf-1 do
              if SaveFields[i]>=0 then inc(j);

          // Start writing header.
          Writer.WriteListBegin;
          Writer.WriteInteger(j);
          for i:=0 to nf-1 do
          begin
               if SaveFields[i]>=0 then
                  Writer.WriteInteger(ord(ADataSet.Fields[i].DataType));
          end;
          Writer.WriteListEnd;
     end;

     // Write all records
     ADataSet.SaveCount := 0;
     ADataSet.SavedCompletely:=true;
     Writer.WriteListBegin;

     // Check if to write according to current index or not.
     UsingIndex:=sfSaveUsingIndex in FUsingIndex;
     if UsingIndex then
        cnt:=ADataSet.CurIndex.References.Count
     else
         cnt:=TkbmProtCommon(ADataSet.Common).FRecords.Count;

     for j:=0 to cnt-1 do
     begin
          // Check if to save more.
          if (ADataSet.SaveLimit>0) and (ADataSet.SaveCount>=ADataSet.SaveLimit) then
          begin
               ADataSet.SavedCompletely:=false;
               break;
          end;

          // Check if to invoke progress event if any.
          if (j mod 100)=0 then ADataSet.Progress(trunc((j/cnt)*100),mtpcSave);

          // Setup which record to look at.
          if UsingIndex then
             ADataSet.OverrideActiveRecordBuffer:=PkbmRecord(ADataSet.CurIndex.References.Items[j])
          else
             ADataSet.OverrideActiveRecordBuffer:=PkbmRecord(TkbmProtCommon(ADataSet.Common).FRecords.Items[j]);
          if (ADataSet.OverrideActiveRecordBuffer=nil) then continue;

          // Calculate fields.
          ADataSet.__ClearCalcFields(ADataSet.OverrideActiveRecordBuffer);
          ADataSet.__GetCalcFields(ADataSet.OverrideActiveRecordBuffer);

          // Check filter of record.
          Accept:=ADataSet.FilterRecord(ADataSet.OverrideActiveRecordBuffer,false);
          if not Accept then continue;

          // Check accept of saving this record.
          Accept:=true;
          if Assigned(ADataSet.OnSaveRecord) then ADataSet.OnSaveRecord(ADataset,Accept);
          if not Accept then continue;

          // Write current record.
          NewestVersion:=true;
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}
          // New for v. 2.24.

          if (not (sfSaveData in sfData)) and (ADataSet.OverrideActiveRecordBuffer^.UpdateStatus=usUnmodified) then continue;

          // New for v. 2.30b
          if (not (sfSaveDontFilterDeltas in sfDontFilterDeltas)) and (ADataSet.OverrideActiveRecordBuffer^.UpdateStatus=usDeleted) then
          begin
               // Make sure record has not been inserted and deleted again.
               pRec:=ADataSet.OverrideActiveRecordBuffer^.PrevRecordVersion;
               while pRec^.PrevRecordVersion<>nil do pRec:=pRec^.PrevRecordVersion;
               if pRec^.UpdateStatus=usInserted then continue;
          end;

          // Write record versions in a list starting with Updatestatus.
          Writer.WriteListBegin;
          while ADataSet.OverrideActiveRecordBuffer<>nil do
          begin
               Writer.WriteInteger(ord(ADataSet.OverrideActiveRecordBuffer^.UpdateStatus));
 {$ENDIF}
{$ENDIF}
               for i:=0 to nf-1 do
               begin
                    if SaveFields[i]>=0 then
                    begin
                         if NewestVersion and Assigned(ADataSet.OnSaveField) then ADataSet.OnSaveField(ADataset,i,ADataSet.Fields[i]);

{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}
  {$IFNDEF BINARY_FILE_230_COMPATIBILITY}
                         Writer.WriteBoolean(ADataSet.Fields[i].IsNull);
                         if not ADataSet.Fields[i].IsNull then
                         begin
  {$ENDIF}
 {$ENDIF}
{$ENDIF}
                              case ADataSet.Fields[i].DataType of
                                   ftBoolean : Writer.WriteBoolean(ADataSet.Fields[i].AsBoolean);

                                   ftLargeInt: Writer.WriteFloat(ADataSet.Fields[i].AsFloat);
                                   ftWideString: Writer.WriteString(string(UTF8Encode(ADataSet.Fields[i].AsWideString)));

                                   ftSmallInt,
                                   ftInteger,
                                   ftWord,
                                   ftAutoInc : Writer.WriteInteger(ADataSet.Fields[i].AsInteger);

                                   ftFloat : Writer.WriteFloat(ADataSet.Fields[i].AsFloat);

                                   ftBCD,
                                   ftFMTBcd,
                                   ftCurrency : Writer.WriteFloat(ADataSet.Fields[i].AsCurrency);

                                   ftDate,
                                   ftTime,ftDateTime: Writer.WriteFloat(ADataSet.Fields[i].AsFloat);
                              else
                                  Writer.WriteString(ADataSet.Fields[i].AsString);
                              end;
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}
  {$IFNDEF BINARY_FILE_230_COMPATIBILITY}
                         end;
  {$ENDIF}
 {$ENDIF}
{$ENDIF}
                    end;
               end;
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}                         // New for v. 2.24.

               // Only write newest version (current data).
               if not (sfSaveDeltas in sfDeltas) then break;

               // Prepare writing next older version of record.
               ADataSet.OverrideActiveRecordBuffer:=ADataSet.OverrideActiveRecordBuffer^.PrevRecordVersion;
               NewestVersion:=false;
          end;
          Writer.WriteListEnd;
 {$ENDIF}
{$ENDIF}

          // Increment save count.
          ADataSet.SaveCount:=ADataSet.SaveCount + 1;
     end;
     Writer.WriteListEnd;
end;

procedure TkbmCustomBinaryStreamFormat.BeforeLoad(ADataset:TkbmCustomMemTable);
{$IFDEF FPC}
var
  Signature: LongInt;
{$ENDIF}
begin
     inherited;

     StreamSize:=WorkStream.Size;
     ProgressCnt:=0;

     Reader:=TReader.Create(WorkStream,FBuffSize);
{$IFDEF FPC}
     Reader.Read(Signature,4);
     if Signature<>LongInt(FilerSignature) then
        raise EReadError.Create('Stream format error');
{$ELSE}
     Reader.ReadSignature;
{$ENDIF}

     InitIndexDef:=false;

{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
     if Reader.NextValue = vaList then       // A hack since vaList only exists in >= v. 2.xx.
       FileVersion := 100
     else
       FileVersion:=Reader.ReadInteger;
     if FileVersion>kbmBinaryFileVersion then // Newer version than what I can handle.
        raise EReadError.Create('Newer kbmMemTable binary stream version ('+inttostr(FileVersion)+') cant be read by this version of the software ('+inttostr(kbmBinaryFileVersion)+').');

{$ELSE}
     FileVersion:=0;
{$ENDIF}
end;

procedure TkbmCustomBinaryStreamFormat.AfterLoad(ADataset:TkbmCustomMemTable);
begin
     Reader.Free;

     // Now create indexes as defined.
     if InitIndexDef then ADataset.CreateIndexes;
     ADataset.OverrideActiveRecordBuffer:=nil;
     inherited;
end;

procedure TkbmCustomBinaryStreamFormat.DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation);
begin
     NewIndex:=OrigIndex;
end;

procedure TkbmCustomBinaryStreamFormat.LoadDef(ADataset:TkbmCustomMemTable);
var
   i:integer;
   FName,KName,TName,DName,EMask,DExpr:string;
   FSize,DSize:integer;
   REQ,RO:boolean;
   FT:TFieldType;
   FK:TFieldKind;
   InitTableDef:boolean;
   ld,ldidx:boolean;
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
   ioptions:TIndexOptions;
   FFields:string;
{$ENDIF}
  aField:TField;
  aIndexDef:TIndexDef;
begin
     if (StreamSize = 0) then exit;
     ld:=sfLoadDef in sfDef;
     ldidx:=sfLoadIndexDef in sfIndexDef;

     // Read all definitions if any saved.
     InitTableDef:=false;
     InitIndexDef:=false;
     try
        Reader.ReadListBegin;

        while not(Reader.EndofList) do
        begin
             // Clear previous setup if not cleared yet.
             if not InitTableDef then
             begin
                  if ld then
                  begin
{$IFDEF LEVEL21}
                       ADataset.Fields.LifeCycles:=[lcAutomatic];
{$ENDIF}
                       ADataSet.Close;
                       ADataSet.FieldDefs.clear;
                       ADataSet.DeleteTable;
                       ADataSet.Fields.Clear;
                  end;
                  InitTableDef:=true;
             end;

             // read field definition.
             FName := Reader.ReadString;
             TName := Reader.ReadString;
             FSize := Reader.ReadInteger;
             DName := Reader.ReadString;
             EMask := Reader.ReadString;
             DSize := Reader.ReadInteger;
             REQ := Reader.ReadBoolean;
             RO := Reader.ReadBoolean;
             if FileVersion>=250 then KName:=Reader.ReadString
             else KName:=FieldKindNames[0]; // fkData
             if FileVersion>=251 then DExpr:=Reader.ReadString
             else DExpr:='';

             // Find fieldtype from fieldtypename.
             for i:=0 to ord(High(FieldTypeNames)) do
                 if FieldTypeNames[TFieldType(i)]=TName then break;
             FT:=TFieldType(i);
             if not (FT in kbmSupportedFieldTypes) then
                raise EMemTableError.Create(Format(kbmUnknownFieldErr1,[TName]));

             // Find fieldkind from fieldkindname.
             FK:=fkData;
             for i:=0 to ord(High(FieldKindNames)) do
                 if FieldKindNames[i]=KName then
                 begin
                      FK:=TFieldKind(i);
                      break;
                 end;

            if ld then
            begin
                 // Add field definition.
                 ADataSet.FieldDefs.Add(FName,FT,FSize,REQ);

                 // Setup other properties.
                 i:=ADataSet.FieldDefs.IndexOf(FName);

                 AField := ADataSet.FieldDefs.Items[i].CreateField(ADataset);
                 AField.FieldKind:=FK;
                 AField.DisplayLabel:=DName;
{$IFNDEF FPC}
                 AField.EditMask:=EMask;
{$ENDIF}
                 AField.ReadOnly:=RO;
                 AField.DisplayWidth:=DSize;
                 AField.DefaultExpression:=DExpr;
             end;
        end;
        Reader.ReadListEnd;

        // Indexes introduced in file version 2.00
        if FileVersion>=200 then
        begin
             // Read all index definitions if any saved.
             Reader.ReadListBegin;

             while not(Reader.EndofList) do
             begin
                  // Clear previous setup if not cleared yet.
                  if not InitIndexDef then
                  begin
                       if ld and ldidx then
                       begin
                            ADataSet.DestroyIndexes;
                            ADataSet.IndexDefs.Clear;
                       end;
                       InitIndexDef:=true;
                  end;

                  // read index definition.
                  FName := Reader.ReadString;
                  FFields := Reader.ReadString;
                  DName := Reader.ReadString;

                  ioptions:=[];
                  if Reader.ReadBoolean then ioptions:=ioptions+[ixDescending];
                  if Reader.ReadBoolean then ioptions:=ioptions+[ixCaseInSensitive];

                  if Reader.ReadBoolean then ioptions:=ioptions+[ixNonMaintained];
                  if Reader.ReadBoolean then ioptions:=ioptions+[ixUnique];

                  // Add index definition.
                  if ld and ldidx then
                  begin
                       aIndexDef := ADataSet.IndexDefs.AddIndexDef;
                       aIndexDef.Name:=FName;
                       aIndexDef.Fields:=FFields;
                       aIndexDef.Options:=ioptions;
{$IFNDEF FPC}
                       aIndexDef.DisplayName:=DName;
{$ENDIF}
                  end;
             end;
             Reader.ReadListEnd;
        end;
     finally
        if InitTableDef then ADataSet.Open;
     end;
     if not (ld and ldidx) then InitIndexDef:=false;
end;

procedure TkbmCustomBinaryStreamFormat.LoadData(ADataset:TkbmCustomMemTable);
   procedure SkipField(AFieldType:TFieldType);
   begin
        case AFieldType of
          ftBoolean :   Reader.ReadBoolean;

          ftLargeInt:   Reader.ReadFloat;
          ftWideString: Reader.ReadString;

          ftSmallInt,
          ftInteger,
          ftWord :      Reader.ReadInteger;

          ftAutoInc :   Reader.ReadInteger;

          ftFloat :     Reader.ReadFloat;

          ftBCD,
          ftCurrency :  Reader.ReadFloat;

          ftFMTBCD   :  if FileVersion>=400 then
                           Reader.ReadFloat
                        else
                            Reader.ReadString;
          ftDate,
          ftTime,
          ftDateTime : Reader.ReadFloat;
        else
          Reader.ReadString;
        end;
   end;
var
   i,j,k:TkbmNativeInt;
   nf:TkbmNativeInt;
   Accept:boolean;
   bNull:boolean;
   bWidths:boolean;
   NewestVersion:boolean;
   pRec:PkbmRecord;
   ApproxRecs:integer;
   fc,fno:integer;
   ftypes:array of TFieldType;
{$IFDEF NEXTGEN}
   ws:string;
{$ELSE}
   ws:WideString;
{$ENDIF}
   s:string;
begin
     if (StreamSize = 0) then exit;
     bWidths:=sfLoadDetermineWidth in sfDisplayWidth;

     ADataSet.__SetTempState(dsinsert);
     try
        ADataSet.ResetAutoInc;

        // Try to determine approx how many records in stream + add some slack.
        if ADataSet.RecordSize>0 then
        begin
             ApproxRecs:=StreamSize div ADataSet.Common.DataRecordSize;
             ApproxRecs:=ApproxRecs + (ApproxRecs div 50) + ADataSet.RecordCount;
        end
        else
            ApproxRecs:=0;

        nf:=length(LoadFields);

        // Load datatypes from header.
        fc:=0;
        if sfLoadDataTypeHeader in sfDataTypeHeader then
        begin
             Reader.ReadListBegin;
             fc:=Reader.ReadInteger;
             SetLength(ftypes,fc);
             for i:=0 to fc-1 do
                 ftypes[i]:=TFieldType(Reader.ReadInteger);
             Reader.ReadListEnd;
        end;

        // Read all records.
        ADataSet.LoadCount:=0;
        ADataSet.LoadedCompletely:=true;

        if ApproxRecs>0 then ADataSet.Common.Records.Capacity:=ApproxRecs; // For speed reason try to preallocate room for all records.

        Reader.ReadListBegin;
        while not(Reader.EndofList) do
        begin
             // Show progress.
             inc(ProgressCnt);
             ProgressCnt:=ProgressCnt mod 100;
             if (ProgressCnt=0) then
                ADataSet.Progress(trunc((WorkStream.Position / StreamSize) * 100),mtpcLoad);

             if (ADataSet.LoadLimit>0) and (ADataSet.LoadCount>=ADataSet.LoadLimit) then
             begin
                  ADataSet.LoadedCompletely:=false;
                  break;
             end;

             pRec:=ADataSet.Common._InternalAllocRecord;
             ADataSet.OverrideActiveRecordBuffer:=pRec;

             NewestVersion:=true;
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}                         // New for v. 2.24.

             // Loop for all versions of record if versioning is used (2.30 and forth).
             if FileVersion>=230 then Reader.ReadListBegin;
             while true do
             begin
                  if FileVersion>=230 then ADataSet.OverrideActiveRecordBuffer^.UpdateStatus:=TUpdateStatus(Reader.ReadInteger);
 {$ENDIF}
{$ENDIF}

                  // Read fields for current record version.
                  fno:=0;
                  for i:=0 to nf-1 do
                  begin
                       if LoadFields[i]<0 then
                       begin
                            // Check if to skip.
                            if fc>0 then SkipField(ftypes[fno]);
                            continue;
                       end;
                       j:=LoadFields[i];

                       //2.50i                          if Fields[i].FieldKind<>fkData then continue;

{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}
  {$IFNDEF BINARY_FILE_230_COMPATIBILITY}                     // New for v. 2.49.
                       // Check if null values saved in binary file.
                       if (FileVersion>=249) then
                          bNull:=Reader.ReadBoolean
                       else
  {$ENDIF}
 {$ENDIF}
{$ENDIF}
                          bNull:=false;

                       // Check if null field.
                       if bNull then
                          ADataSet.Fields[j].Clear
                       else
                       begin
                            // Not null, load data.
                            case ADataSet.Fields[j].DataType of
                                 ftBoolean : ADataSet.Fields[j].AsBoolean := Reader.ReadBoolean;

                                 ftLargeInt: ADataSet.Fields[j].AsFloat:=Reader.ReadFloat;
                                 ftWideString:
                                    begin
                                         ws:=Reader.ReadString;
                                         ADataSet.Fields[j].Value:=ws;
                                         if bWidths then
                                         begin
                                              k:=length(ws);
                                              if k>LoadFieldWidths[i] then
                                                 LoadFieldWidths[i]:=k;
                                         end;
                                    end;

                                 ftSmallInt,
                                 ftInteger,
                                 ftWord : ADataSet.Fields[j].AsInteger := Reader.ReadInteger;

                                 ftAutoInc :begin
                                                 ADataSet.Fields[j].AsInteger:=Reader.ReadInteger;
                                                 if ADataSet.Common.AutoIncMax<ADataSet.Fields[j].AsInteger then
                                                    ADataSet.Common.AutoIncMax:=ADataSet.Fields[j].AsInteger;
                                            end;

                                 ftFloat : ADataSet.Fields[j].AsFloat := Reader.ReadFloat;

                                 ftBCD,
                                 ftCurrency : ADataSet.Fields[j].AsCurrency := Reader.ReadFloat;

                                 ftFMTBCD   : if FileVersion>=400 then
                                                 ADataSet.Fields[j].AsCurrency := Reader.ReadFloat
                                              else
                                              begin
                                                   s:=Reader.ReadString;
                                                   ADataSet.Fields[j].AsString:=s;
                                              end;

                                 ftDate,
                                 ftTime,
                                 ftDateTime : ADataSet.Fields[j].AsFloat:=Reader.ReadFloat;
                            else
                                begin
                                     s:=Reader.ReadString;
                                     ADataSet.Fields[j].AsString:=s;
                                     if bWidths then
                                     begin
                                          k:=length(s);
                                          if k>LoadFieldWidths[i] then
                                             LoadFieldWidths[i]:=k;
                                     end;
                                end;
                            end;
                       end;

                       if NewestVersion and Assigned(ADataSet.OnLoad) then ADataSet.OnLoadField(ADataset,i,ADataSet.Fields[i]);
{$IFNDEF BINARY_FILE_1XX_COMPATIBILITY}
 {$IFNDEF BINARY_FILE_200_COMPATIBILITY}                         // New for v. 2.24.
                  end;

                  // Previous file versions didnt contain versions, so just break loop.
                  if FileVersion<230 then break;

                  // Prepare for reading next version if any. (introduced in v. 2.30)
                  if Reader.EndOfList then break;

                  // Prepare next version.
                  NewestVersion:=false;
                  ADataSet.OverrideActiveRecordBuffer^.PrevRecordVersion:=ADataSet.Common._InternalAllocRecord;
                  ADataSet.OverrideActiveRecordBuffer:=ADataSet.OverrideActiveRecordBuffer^.PrevRecordVersion;
             end;
             if FileVersion>=230 then Reader.ReadListEnd;
 {$ENDIF}
{$ENDIF}

             Accept:=true;
             if Assigned(ADataSet.OnLoadRecord) then ADataSet.OnLoadRecord(ADataset,Accept);
             if Accept then
             begin
                  pRec^.RecordID:=ADataset.Common.RecordID;
                  ADataSet.Common.RecordID:=ADataSet.Common.RecordID+1;
                  pRec^.UniqueRecordID:=ADataSet.Common.UniqueRecordID;
                  ADataSet.Common.UniqueRecordID:=ADataSet.Common.UniqueRecordID+1;
                  pRec^.Flag:=kbmrfInTable;
                  ADataSet.Common.Records.Add(pRec);
                  if pRec^.UpdateStatus=usDeleted then
                     ADataSet.Common.deletedCount:=ADataSet.Common.DeletedCount+1;
                  ADataSet.LoadCount:=ADataSet.LoadCount+1;
             end
             else
                 ADataSet.Common._InternalFreeRecord(pRec,true,true);
        end;
        Reader.ReadListEnd;
     finally
        ADataSet.__RestoreState(dsBrowse);
     end;
end;

end.
