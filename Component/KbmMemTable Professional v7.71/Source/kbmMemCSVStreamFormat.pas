unit kbmMemCSVStreamFormat;

interface

{$include kbmMemTable.inc}

{$IFDEF LEVEL18}
 {$ZEROBASEDSTRINGS OFF}
{$ENDIF}

// =========================================================================
// CSV stream format for kbmMemTable v. 3.xx+
//
// Copyright 1999-2013 Kim Bo Madsen/Components4Developers
// All rights reserved.
//
// Please refer to kbmMemTable.pas for license agreement.
//
// History.
// Per v. 3.00, the stream formats will each have their own history.
//

//=============================================================================
// Remove the remark on the next lines if to keep CSV file compatibility
// between different versions of TkbmMemTable.
//{$define CSV_FILE_1XX_COMPATIBILITY}
//=============================================================================

// History.
// Per v. 3.00, the stream formats will have their own history.
//
// 3.00a alpha
//       Initial v. 3.00 CSV stream format release based on the sources of v. 2.53b.
//
// 3.00b alpha 11. Aug. 2001
//       Fixed loading CSV with CSVQuote=#0 and CSVRecordDelimiter=#0.
//       Fixed not allowing CSVFieldDelimiter=#0.
//       Bugs reported by Dave (rave154@yahoo.co.uk)
//
// 3.00c alpha 12. Aug. 2001
//       Added support for the Assign method.
//
// 3.00d alpha 20. Sep. 2001
//       Changed GetWord to automatically detect and accept unquoted fields.
//       Contribution by Georg Zimmer (gzimmer@empoweryourfirm.com).
//
// 3.00e alpha 17. Nov. 2001
//       Fixed LoadDef not deciphering the field definition list correctly
//       todo with field kind and default expression.
//
// 3.00f beta 30. Jan. 2002
//       Fixed bug reading CSV files with blobs.
//       Reason was a faulty GetWord algorithm.
//
// 3.00f9 beta 25. Feb. 2002
//       Added OnFormatLoadField and OnFormatSaveField event for reformatting of data.
//       Added sfQuoteOnlyStrings flag for selecting to only quote string/binary fields during save.
//       Published sfNoHeader and completed support for it. It controls if a header should be saved or loaded.
//
// 3.00 Final 14. Jun. 2002
//       Changed version status to final.
//
// 3.01 7. Aug. 2002
//      Fixed problem not loading last field if not quoted. Bug reported by several.
//
// 3.09 20. Apr. 2003
//      Fixed problem not loading last field in certain cases.
//
// 3.10 23. Jul. 2007
//      Added support for ignoring commented lines starting with CommentChar.
//      Added support for changing escape character % to something else via EscapeChar.
//      Added OnGetLine event.
//      Added support for skipping empty lines instead of simply stopping import.
//      Added support for loading CSV files without knowing structure.
//        Fields will be defined either based on names in the first line, or as Field_x.
//        Each field will be defined as a 255 (default) character stringfield why small performance mode
//        is an advantage.
//      Added DefaultFieldStringSize (default 255).

uses
  kbmMemTable,
  kbmMemTypes,
  Classes,
  DB,
{$IFNDEF FPC}
  DBCommon,
{$ENDIF}
{$IFDEF LEVEL18}
  System.Character,
{$ENDIF}
{$include kbmMemRes.inc}
  SysUtils;

type
  TkbmStreamFlagLocalFormat   = (sfSaveLocalFormat,sfLoadLocalFormat
       ,sfLoadAsASCII
{$IFDEF LEVEL16}
       ,sfLoadAsANSI
{$ENDIF}
     );
  TkbmStreamFlagNoHeader      = (sfSaveNoHeader,sfLoadNoHeader);
  TkbmStreamFlagQuoteOnlyStrings = (sfSaveQuoteOnlyStrings);
  TkbmStreamFlagPlaceholders  = (sfSavePlaceholders);
  TkbmStreamFlagAutoInc       = (sfLoadGenerateAutoInc);

  TkbmStreamFlagsLocalFormat  = set of TkbmStreamFlagLocalFormat;
  TkbmStreamFlagsNoHeader     = set of TkbmStreamFlagNoHeader;
  TkbmStreamFlagsPlaceHolders = set of TkbmStreamFlagPlaceHolders;
  TkbmStreamFlagsQuoteOnlyStrings = set of TkbmStreamFlagQuoteOnlyStrings;
  TkbmStreamFlagsAutoInc  = set of TkbmStreamFlagAutoInc;

  TkbmOnFormatLoadField = procedure(Sender:TObject; Field:TField; var Null:boolean; var Data:string) of object;
  TkbmOnFormatSaveField = procedure(Sender:TObject; Field:TField; var Null:boolean; var Data:string) of object;

  TkbmOnGetLine = procedure(Sender:TObject; var ALine:string; var AEOF:boolean) of object;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCustomCSVStreamFormat = class(TkbmCustomStreamFormat)
  private
     FDataset:TkbmCustomMemTable;
     FOnFormatLoadField:TkbmOnFormatLoadField;
     FOnFormatSaveField:TkbmOnFormatSaveField;
     FOnGetLine:TkbmOnGetLine;

     FBuf:TBytes;
     FBufidx:integer;
     FBufSize:integer;

     FRawLine:TBytes;
     FLine,FWord:string;
     Flptr,Felptr:integer;

     FProgressCnt:integer;
     FStreamSize:longint;
     FStartPosition:longint;

     FCommentChar:Char;
     FEscapeChar:Char;
     FDefaultStringFieldSize:integer;
     FCSVQuote:Char;
     FCSVFieldDelimiter:Char;
     FCSVRecordDelimiter:Char;
     FCSVTrueString,FCSVFalseString:string;
     FsfLocalFormat:TkbmStreamFlagsLocalFormat;
     FsfNoHeader:TkbmStreamFlagsNoHeader;
     FsfPlaceHolders:TkbmStreamFlagsPlaceHolders;
     FsfQuoteOnlyStrings:TkbmStreamFlagsQuoteOnlyStrings;
     FsfAutoInc:TkbmStreamFlagsAutoInc;

     FLocalFormat:TFormatSettings;
     FUseFieldDisplayName:boolean;

     procedure SetCSVFieldDelimiter(Value:Char);
  protected
     FDefLoaded:boolean;

     function GetChunk:boolean; virtual;
     function GetLine:boolean; virtual;
     function GetWord(var null:boolean):string; virtual;
     procedure WriteString(const AString:string); virtual;

     function GetVersion:string; override;

     procedure BeforeSave(ADataset:TkbmCustomMemTable); override;
     procedure SaveDef(ADataset:TkbmCustomMemTable); override;
     procedure SaveData(ADataset:TkbmCustomMemTable); override;
     procedure AfterSave(ADataset:TkbmCustomMemTable); override;

     procedure DetermineLoadFieldIDs(ADataset:TkbmCustomMemTable; AList:TStringList; Situation:TkbmDetermineLoadFieldsSituation); override;
     procedure DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation); override;
     procedure BeforeLoad(ADataset:TkbmCustomMemTable); override;
     procedure LoadDef(ADataset:TkbmCustomMemTable); override;
     procedure LoadData(ADataset:TkbmCustomMemTable); override;
     procedure AfterLoad(ADataset:TkbmCustomMemTable); override;

     function GetFieldHeaderText(const AField:TField ):string;
     procedure LoadFieldIDs(const ADataset:TkbmCustomMemTable; const AList:TStringList; const AUseFieldDisplayName:boolean); virtual;
  public
     property OnFormatLoadField:TkbmOnFormatLoadField read FOnFormatLoadField write FOnFormatLoadField;
     property OnFormatSaveField:TkbmOnFormatSaveField read FOnFormatSaveField write FOnFormatSaveField;
     property OnGetLine:TkbmOnGetLine read FOnGetLine write FOnGetLine;
     property CommentChar:Char read FCommentChar write FCommentChar;
     property EscapeChar:Char read FEscapeChar write FEscapeChar;
     property DefaultStringFieldSize:integer read FDefaultStringFieldSize write FDefaultStringFieldSize;
     property CSVQuote:Char read FCSVQuote write FCSVQuote;
     property CSVFieldDelimiter:Char read FCSVFieldDelimiter write SetCSVFieldDelimiter;
     property CSVRecordDelimiter:Char read FCSVRecordDelimiter write FCSVRecordDelimiter;
     property CSVTrueString:string read FCSVTrueString write FCSVTrueString;
     property CSVFalseString:string read FCSVFalseString write FCSVFalseString;
     property sfLocalFormat:TkbmStreamFlagsLocalFormat read FsfLocalFormat write FsfLocalFormat;
     property sfNoHeader:TkbmStreamFlagsNoHeader read FsfNoHeader write FsfNoHeader;
     property sfPlaceHolders:TkbmStreamFlagsPlaceHolders read FsfPlaceHolders write FsfPlaceHolders;
     property sfQuoteOnlyStrings:TkbmStreamFlagsQuoteOnlyStrings read FsfQuoteOnlyStrings write FsfQuoteOnlyStrings;
     property sfAutoInc:TkbmStreamFlagsAutoInc read FsfAutoInc write FsfAutoInc;
     property UseFieldDisplayName:boolean read FUseFieldDisplayName write FUseFieldDisplayName;

  public
     constructor Create(AOwner:TComponent); override;
     procedure Assign(Source:TPersistent); override;
  end;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCSVStreamFormat = class(TkbmCustomCSVStreamFormat)
  published
     property CommentChar;
     property EscapeChar;
     property DefaultStringFieldSize;
     property CSVQuote;
     property CSVFieldDelimiter;
     property CSVRecordDelimiter;
     property CSVTrueString;
     property CSVFalseString;
     property sfLocalFormat;
     property sfQuoteOnlyStrings;
     property sfNoHeader;
     property Version;

     property sfData;
     property sfCalculated;
     property sfLookup;
     property sfNonVisible;
     property sfBlobs;
     property sfDef;
     property sfIndexDef;
     property sfPlaceHolders;
     property sfFiltered;
     property sfIgnoreRange;
     property sfIgnoreMasterDetail;
     property sfDeltas;
     property sfDontFilterDeltas;
     property sfAppend;
     property sfFieldKind;
     property sfFromStart;
     property sfDisplayWidth;
     property sfAutoInc;

     property OnFormatLoadField;
     property OnFormatSaveField;
     property OnGetLine;
     property OnBeforeLoad;
     property OnAfterLoad;
     property OnBeforeSave;
     property OnAfterSave;
     property OnCompress;
     property OnDeCompress;

{$IFNDEF KBMMEMTABLE_DONT_SUPPORT_PUBLISHED_RECORDS}
     property FormatSettings;
{$ENDIF}
     property UseFieldDisplayName;
  end;

  function StringToCodedString(const Source:string; const EscapeChar:Char):string;
  function CodedStringToString(const Source:string; const EscapeChar:Char):string;
  function BufferToBase64(const Source:TBytes):TBytes;
  function Base64ToBuffer(const Source:TBytes):TBytes;

implementation

const
  // Table definition magic words.
  kbmTableDefMagicStart:string = '@@TABLEDEF START@@';
  kbmTableDefMagicEnd:string = '@@TABLEDEF END@@';

  // Index definition magic words.
  kbmIndexDefMagicStart:string = '@@INDEXDEF START@@';
  kbmIndexDefMagicEnd:string = '@@INDEXDEF END@@';

  // File version magic word.
  kbmFileVersionMagic:string = '@@FILE VERSION@@';

  // Current file versions. V. 1.xx file versions are considered 100, 2.xx are considered 2xx etc.
  kbmCSVFileVersion = 251;

  CSVBUFSIZE=1024; //8192;

type
  TkbmProtCustomMemTable = class(TkbmCustomMemTable);
  TkbmProtCommon = class(TkbmCommon);

// General procedures
// ************************************************************


// Code special characters (LF,CR,%,#0)
// CR (#13) -> %c
// LF (#10) -> %n
// #0 -> %0
// % -> %%
function StringToCodedString(const Source:string; const EscapeChar:Char):string;
var
   i,j:TkbmNativeInt;
   l:TkbmNativeInt;
begin
     // Count CR/LF.
     l:=0;
     for i:=1 to length(Source) do
         if
{$IFDEF FPC}
            (Source[i] in [#13,#10,#0])
{$ELSE}
 {$IFDEF LEVEL18}
            Source[i].IsInArray([#13,#10,#0])
 {$ELSE}
            CharInSet(Source[i],[#13,#10,#0])
 {$ENDIF}
{$ENDIF}
            or (Source[i]=EscapeChar) then inc(l);

     // If no special characters, return the original string.
     if l=0 then
     begin
          Result:=Source;
          exit;
     end;

     // If any special characters, make room for them.
     SetLength(Result,length(Source)+l);

     // Code special characters.
     j:=1;
     for i:=1 to length(Source) do
         case Source[i] of
              #13: begin
                        Result[j]:=EscapeChar; inc(j);
                        Result[j]:='c'; inc(j);
                   end;
              #10: begin
                        Result[j]:=EscapeChar; inc(j);
                        Result[j]:='n'; inc(j);
                   end;
              #0:  begin
                        Result[j]:=EscapeChar; inc(j);
                        Result[j]:='0'; inc(j);
                   end;
              else
                  if Source[i]=EscapeChar then
                  begin
                       Result[j]:=EscapeChar; inc(j);
                       Result[j]:=EscapeChar; inc(j);
                  end
                  else
                  begin
                       Result[j]:=Source[i];
                       inc(j);
                  end;
         end;
end;

{$R-}

// Decode special characters (LF,CR,%,#0)
// %c -> CR (#13)
// %n -> LF (#10)
// %% -> %
// %0 -> #0
function CodedStringToString(const Source:string; const EscapeChar:Char):string;
var
   i,j:TkbmNativeInt;
begin
     SetLength(Result,length(Source));

     // Code special characters.
     i:=1;
     j:=1;
     while true do
     begin
          if i>length(Source) then break;
          if Source[i]=EscapeChar then
          begin
               inc(i);
               if i>length(Source) then break;
               case Source[i] of
                    'c': Result[j]:=#13;
                    'n': Result[j]:=#10;
                    '0': Result[j]:=#0;
               else
                   if Source[i]=EscapeChar then
                      Result[j]:=EscapeChar;
               end;
               inc(j);
          end
          else
          begin
               Result[j]:=Source[i];
               inc(j);
          end;
          inc(i);
     end;

     // Cut result string to right length.
     if i<>j then SetLength(Result,j-1);
end;

// Code a buffer as BASE 64.
function BufferToBase64(const Source:TBytes):TBytes;
var
   Act: Word;
   I,Len:TkbmNativeInt;
   Bits,P:integer;
begin
     Bits:=0;
     Len:=(Length(Source)*4+2) div 3;
     if Len>0 then
     begin
          SetLength(Result,Len);
	        P:=0;
	        Act:=0;
	        for I:=0 to Len-2 do
          begin
	             if Bits<6 then
               begin
	                  Act:=(Act shr 6) or (Source[P] shl Bits);
		                Inc(P);
		                Inc(Bits,2);
               end
               else
               begin
	                  Dec(Bits,6);
		                Act:=Act shr 6;
               end;
	             Result[I]:=byte(Act and 63+32);
          end;
	        Result[Len-1]:=byte(Act shr 6+32);
     end
     else
         SetLength(Result,0);
end;

// Decode BASE64 string.
function Base64ToBuffer(const Source:TBytes):TBytes;
var
   Act:Word;
   I,Len:TkbmNativeInt;
   Bits,P:integer;
begin
     Len:=(Length(Source)*3) div 4;
     SetLength(Result,Len);
     Bits:=0;
     Act:=0;
     P:=0;
     for I:=0 to system.Length(Source)-1 do
     begin
          Act:=Act or (Ord(Source[I])-32) shl Bits;
	        if Bits>=2 then
          begin
	             Result[P]:=byte(Act and $FF);
	             Inc(P);
	             Act:=Act shr 8;
	             Dec(Bits,2);
          end
          else
	            Inc(Bits,6);
     end;
end;

// Quote a string.
function QuoteString(const Source:string; Quote:char):string;
begin
     if Quote=#0 then Result:=Source
     else Result:=AnsiQuotedStr(Source,Quote);
end;

// Extract a quoted string.
function ExtractQuoteString(const Source:string; Quote:char):string;
begin
     if Quote=#0 then
     begin
          Result:=Source;
          exit;
     end;
     Result:=AnsiDequotedStr(Source,Quote);
end;

// TKbmCustomCSVStreamFormat
//*******************************************************************

constructor TkbmCustomCSVStreamFormat.Create(AOwner:TComponent);
begin
     inherited;
     FEscapeChar:='%';
     FCommentChar:=#0;
     FDefaultStringFieldSize:=255;
     FCSVQuote:='"';
     FCSVFieldDelimiter:=',';
     FCSVRecordDelimiter:=',';
     FCSVTrueString:='True';
     FCSVFalseString:='False';
     FsfLocalFormat:=[];
     FsfPlaceHolders:=[];
     FUseFieldDisplayName:=false;
end;

function TkbmCustomCSVStreamFormat.GetVersion:string;
begin
     Result:='3.10';
end;

procedure TkbmCUstomCSVStreamFormat.Assign(Source:TPersistent);
begin
     if Source is TkbmCustomCSVStreamFormat then
     begin
          CSVQuote:=TkbmCustomCSVStreamFormat(Source).CSVQuote;
          CSVFieldDelimiter:=TkbmCustomCSVStreamFormat(Source).CSVFieldDelimiter;
          CSVRecordDelimiter:=TkbmCustomCSVStreamFormat(Source).CSVRecordDelimiter;
          CSVTrueString:=TkbmCustomCSVStreamFormat(Source).CSVTrueString;
          CSVFalseString:=TkbmCustomCSVStreamFormat(Source).CSVFalseString;
          sfLocalFormat:=TkbmCustomCSVStreamFormat(Source).sfLocalFormat;
          sfPlaceHolders:=TkbmCustomCSVStreamFormat(Source).sfPlaceHolders;
     end;
     inherited;
end;

procedure TkbmCustomCSVStreamFormat.SetCSVFieldDelimiter(Value:Char);
begin
     if Value<>#0 then FCSVFieldDelimiter:=Value;
end;

procedure TkbmCustomCSVStreamFormat.WriteString(const AString:string);
var
   l:integer;
   b:{$IFDEF FPC}UTF8String{$ELSE}TBytes{$ENDIF};
const
   CRLF:array[0..1] of byte=(13,10);
begin
{$IFDEF FPC}
     b:=UTF8Encode(AString);
{$ELSE}
     b:=TEncoding.UTF8.GetBytes(AString);
{$ENDIF}
     l:=length(b);
     WorkStream.WriteBuffer({$IFDEF FPC}b[1]{$ELSE}b[0]{$ENDIF},l);
     l:=length(CRLF);
     WorkStream.WriteBuffer(CRLF[0],l);
end;

procedure TkbmCustomCSVStreamFormat.BeforeSave(ADataset:TkbmCustomMemTable);
begin
     // Check if trying to save deltas in CSV format. Not supported.
     if sfSaveDeltas in sfDeltas then
        raise EMemTableError.Create(kbmSavingDeltasBinary);

     inherited;

     // Setup standard layout for data.
     FLocalFormat:=self.FormatSettings;
     if not (sfSaveLocalFormat in FsfLocalFormat) then
     begin
          FLocalFormat.DateSeparator:='/';
          FLocalFormat.TimeSeparator:=':';
          FLocalFormat.ThousandSeparator:=',';
          FLocalFormat.DecimalSeparator:='.';
          FLocalFormat.ShortDateFormat:='dd/mm/yyyy';
          FLocalFormat.ShortTimeFormat:='HH:nn:ss';
          FLocalFormat.LongTimeFormat:='HH:nn:ss';
          FLocalFormat.CurrencyString:='';
          FLocalFormat.CurrencyFormat:=0;
          FLocalFormat.NegCurrFormat:=1;
     end;
end;

procedure TkbmCustomCSVStreamFormat.AfterSave(ADataset:TkbmCustomMemTable);
begin
     inherited;
end;

procedure TkbmCustomCSVStreamFormat.SaveDef(ADataset:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
   nf:TkbmNativeInt;
   s:string;
begin
     if not (sfSaveDef in sfDef) then exit;

     // Setup flags for fields to save.
     nf:=ADataSet.Fieldcount;

{$IFNDEF CSV_FILE_1XX_COMPATIBILITY}
     // Write file version.
     s:=QuoteString(kbmFileVersionMagic,FCSVQuote)
        +FCSVFieldDelimiter
        +QuoteString(IntToStr(kbmCSVFileVersion),FCSVQuote);
     WriteString(s);
{$ENDIF}

     // Write header.
     s:=QuoteString(kbmTableDefMagicStart,FCSVQuote);
     WriteString(s);

     // Write fielddefinitions.
     for i:=0 to nf-1 do
     begin
          if (SaveFields[i]>=0) or (sfSavePlaceHolders in sfPlaceHolders) then
          begin
               s:=ADataSet.Fields[i].FieldName+'='+
                   FieldTypeNames[ADataSet.Fields[i].DataType]+','+
                   inttostr(ADataSet.Fields[i].Size)+','+
                   QuoteString(ADataSet.Fields[i].DisplayName,'"')+','+
{$IFDEF FPC}
                   '"",'+
{$ELSE}
                   QuoteString(ADataSet.Fields[i].EditMask,'"')+','+
{$ENDIF}
                   inttostr(ADataSet.Fields[i].DisplayWidth);
               if ADataSet.Fields[i].Required then s:=s+',REQ';
               if ADataSet.Fields[i].ReadOnly then s:=s+',RO';
               if not ADataSet.Fields[i].Visible then s:=s+',INV';
               if sfSaveFieldKind in sfFieldKind then
                  s:=s+','+FieldKindNames[ord(ADataSet.Fields[i].FieldKind)]
               else
                   s:=s+','+FieldKindNames[0];
               s:=s+','+QuoteString(ADataSet.Fields[i].DefaultExpression,'"');
               s:=QuoteString(s,FCSVQuote);
               WriteString(s);
          end;
     end;

{$IFNDEF CSV_FILE_1XX_COMPATIBILITY}
     // Check if to write index definitions.
     if sfSaveIndexDef in sfIndexDef then
     begin
          // Write header.
          s:=QuoteString(kbmIndexDefMagicStart,FCSVQuote);
          WriteString(s);

          // Write indexdefinitions.
          for i:=0 to ADataSet.IndexDefs.count-1 do
              with ADataSet.IndexDefs.Items[i] do
              begin
                   s:=Name+'='+
                      QuoteString(Fields,FCSVQuote)+','+
                      QuoteString(DisplayName,FCSVQuote);
                   if ixDescending in Options then s:=s+',DESC';
                   if ixCaseInsensitive in Options then s:=s+',CASE';
                   if ixNonMaintained in Options then s:=s+',NONMT';
                   if ixUnique in Options then s:=s+',UNIQ';
                   s:=QuoteString(s,FCSVQuote);
                   WriteString(s);
              end;

          // Write footer.
          s:=QuoteString(kbmIndexDefMagicEnd,FCSVQuote);
          WriteString(s);
     end;
{$ENDIF}

     // Write footer.
     s:=QuoteString(kbmTableDefMagicEnd,FCSVQuote);
     WriteString(s);
end;

procedure TkbmCustomCSVStreamFormat.SaveData(ADataset:TkbmCustomMemTable);
var
   i,j:TkbmNativeInt;
   nf:TkbmNativeInt;
   s,s1,a:string;
   Accept:boolean;
   null:boolean;
   dt:TFieldType;
{$IFDEF FPC}
   b:TBytes;
   a1:AnsiString;
{$ENDIF}
begin
     ADataSet.SaveCount := 0;

     // Setup flags for fields to save.
     nf:=ADataSet.Fieldcount;

     // Save header.
     if not (sfSaveNoHeader in sfNoHeader) then
     begin
          // Write all field display names in CSV format.
          s:='';
          a:='';
          for i:=0 to nf-1 do
          begin
               if (SaveFields[i]>=0) or (sfSavePlaceHolders in sfPlaceHolders) then
               begin
                    s:=s+a+QuoteString(GetFieldHeaderText(ADataSet.Fields[i]),FCSVQuote);
                    a:=FCSVFieldDelimiter;
               end;
          end;
          if FCSVRecordDelimiter <> #0 then s:=s+FCSVRecordDelimiter;
          WriteString(s);
     end;

     // Write all records in CSV format ordered by current index.
     if sfSaveData in sfData then
     begin
          try
             ADataSet.SavedCompletely := True;

             for j:=0 to ADataSet.CurIndex.References.Count-1 do
             begin
                  // Check if to save more.
                  if (ADataSet.SaveLimit>0) and (ADataSet.SaveCount>=ADataSet.SaveLimit) then
                  begin
                       ADataSet.SavedCompletely:=false;
                       break;
                  end;

                  // Check if to invoke progress event if any.
                  if (j mod 100)=0 then ADataSet.Progress(trunc((j/ADataSet.CurIndex.References.count)*100),mtpcSave);

                  // Setup which record to work on.
                  ADataSet.OverrideActiveRecordBuffer:=PkbmRecord(ADataSet.CurIndex.References.Items[j]);
                  if ADataSet.OverrideActiveRecordBuffer=nil then continue;

                  // Calculate fields.
                  ADataSet.__ClearCalcFields(ADataSet.OverrideActiveRecordBuffer);
                  ADataSet.__GetCalcFields(ADataSet.OverrideActiveRecordBuffer);

                  // Check filter of record.
                  Accept:=ADataSet.FilterRecord(ADataSet.OverrideActiveRecordBuffer,false);
                  if not Accept then continue;

                  // Check if to accept that record for save.
                  Accept:=true;
                  if Assigned(ADataSet.OnSaveRecord) then ADataSet.OnSaveRecord(ADataset,Accept);
                  if not Accept then continue;

                  // Write current record.
                  s:='';
                  a:='';
                  for i:=0 to nf-1 do
                  begin
                       if SaveFields[i]>=0 then
                       begin
                            if Assigned(ADataSet.OnSaveField) then
                               ADataSet.OnSaveField(ADataset,i,ADataSet.Fields[i]);

                            dt:=ADataSet.Fields[i].DataType;
                            null:=ADataSet.Fields[i].IsNull;
                            if null then
                                s1:=''
                            else if dt in kbmStringTypes then
                                s1:=StringToCodedString(ADataSet.Fields[i].AsString,FEscapeChar)
                            else if dt in kbmBinaryTypes then
                            begin
                              {$IFDEF FPC}
                                b:=BufferToBase64(ADataSet.Fields[i].AsBytes);
                                SetString(a1,PAnsiChar(@b[0]),Length(b));
                                s1:=a1;
                              {$ELSE}
                                s1:=TEncoding.ASCII.GetString(BufferToBase64(ADataSet.Fields[i].AsBytes))
                              {$ENDIF}
                            end
                            else if dt=ftBoolean then
                            begin
                                 with TBooleanField(ADataSet.Fields[i]) do
                                      if Value then
                                         s1:=FCSVTrueString
                                      else
                                          s1:=FCSVFalseString;
                                 end
                            else if dt in [ftFloat,
                                           ftCurrency,
                                           ftBCD
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                          ,ftSingle
{$ENDIF}
{$IFDEF LEVEL6}
                                          ,ftFMTBCD
{$ENDIF}
                                          ] then
                                s1:=FloatToStr(ADataSet.Fields[i].AsFloat,FLocalFormat)

{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                            else if dt=ftExtended then
                                s1:=FloatToStr(ADataSet.Fields[i].AsExtended,FLocalFormat)
{$ENDIF}
                            else if dt=ftDate then
                                s1:=DateToStr(ADataSet.Fields[i].AsDateTime,FLocalFormat)
                            else if dt=ftTime then
                                s1:=TimeToStr(ADataSet.Fields[i].AsDateTime,FLocalFormat)
                            else if dt=ftDateTime then
                                s1:=DateTimeToStr(ADataSet.Fields[i].AsDateTime,FLocalFormat)
                            else
                                s1:=ADataSet.Fields[i].AsString;

                            if assigned(FOnFormatSaveField) then
                               FOnFormatSaveField(self,ADataSet.Fields[i],null,s1);

                            if null then
                               s:=s+a
                            else if ((sfSaveQuoteOnlyStrings in sfQuoteOnlyStrings) and
                                     (not (ADataSet.Fields[i].DataType in kbmStringTypes+kbmBinaryTypes))) then
                               s:=s+a+s1
                            else
                               s:=s+a+QuoteString(s1,FCSVQuote);
                            a:=FCSVFieldDelimiter;
                       end
                       else if sfSavePlaceHolders in sfPlaceHolders then
                       begin
                            s:=s+a;
                            a:=FCSVFieldDelimiter;
                       end;
                  end;

                  // Add record delimiter.
                  if FCSVRecordDelimiter <> #0 then s:=s+FCSVRecordDelimiter;

                  // Write line.
                  WriteString(s);

                  // Increment savecounter.
                  ADataSet.SaveCount:=ADataSet.SaveCount+1;
             end;

          finally
             ADataSet.OverrideActiveRecordBuffer:=nil;
          end;
     end;
end;

function TkbmCustomCSVStreamFormat.GetChunk:boolean;
//  function ToHex:string;
//  var
//     i,n:integer;
//  begin
//       n:=length(FBuf);
//       Result:='';
//       for i:=0 to n-1 do
//           Result:=Result+inttostr(i)+'='+IntToHex(FBuf[i],2)+'('+chr(FBuf[i])+') ';
//  end;
begin
     SetLength(FBuf,CSVBUFSIZE);
     FBufSize:=WorkStream.Read(FBuf[0],CSVBUFSIZE);
     Result:=FBufSize>0;
     FBufidx:=0;

     // Show progress.
     inc(FProgressCnt);
     FProgressCnt:=FProgressCnt mod 100;
     if (FProgressCnt=0) then
         FDataset.Progress(trunc((WorkStream.Position / FStreamSize) * 100),mtpcLoad);
end;

function TkbmCustomCSVStreamFormat.GetLine:boolean;
var
  bEOF:boolean;
  ep,sp:integer;
{$IFDEF FPC}
  a:AnsiString;
{$ENDIF}

   procedure AddToLine;
   var
      n,l:integer;
   begin
        // Add collected data until now, to line.
        n:=ep-sp;
        if n>0 then
        begin
             l:=length(FRawLine);
             SetLength(FRawLine,n+l);
             Move(FBuf[sp],FRawLine[l],n);
        end;
   end;

   procedure DoGetChunk;
   begin
        AddToLine;

        // Get next chunk of data and check if EOF.
        if GetChunk then
        begin
             // Restart pointers to new data in buffer.
             sp:=FBufidx;
             ep:=FBufidx;
        end
        else
            bEOF:=true;
   end;

begin
     // Cut out a line.
     bEOF:=false;
     SetLength(FRawLine,0);
     FLine:='';
     sp:=FBufidx;
     ep:=FBufidx;
     while true do
     begin

          // Check if need another chunk.
          if FBufidx>=FBufSize then
          begin
               DoGetChunk;
               if bEOF then
                  break;
               continue;
          end
          else
          begin
               // Still enough data.

               // Check if found end of line (EOL) character.
               if (FBuf[FBufidx]) in [0,10,13] then
               begin
                    // Check if there is one more, skip it.
                    inc(FBufidx);
                    if FBufidx>=FBufSize then
                    begin
                         DoGetChunk;
                         if bEOF then
                            break;
                    end
                    else
                        // Add collected data to line.
                        AddToLine;

                    // Check if 2nd EOL character, skip it.
                    if (FBuf[FBufidx]) in [0,10,13] then
                       inc(FBufidx);

                    // Prepare for next line.
                    sp:=FBufidx;

{$IFDEF FPC}
                    SetString(a,@FRawLine[0],length(FRawLine));
                    if (sfLoadAsASCII in FsfLocalFormat) then
                       FLine:=a
                    else
                       FLine:=UTF8Decode(UTF8String(a));
{$ELSE}
                    if (sfLoadAsASCII in FsfLocalFormat) then
                       FLine:=TEncoding.ASCII.GetString(FRawLine)
 {$IFDEF LEVEL16}
                    else if (sfLoadAsANSI in FsfLocalFormat) then
                         FLine:=TEncoding.ANSI.GetString(FRawLine)
 {$ENDIF}
                    else
                        FLine:=TEncoding.UTF8.GetString(FRawLine);
{$ENDIF}

                    // Check if user validation event.
                    if Assigned(FOnGetLine) then
                    begin
                         FOnGetLine(self,FLine,bEOF);
                         if bEOF then
                            break;
                    end;

                    // Check if comment line.
                    if (FCommentChar<>#0) and
                       (length(FLine)>0) and
                       (FLine[1]=FCommentChar) then
                    begin
                         FLine:='';
                         continue;
                    end;
                    break;
               end;
          end;

          // Prepare to look at next char.
          Inc(FBufidx);
          ep:=FBufidx;
     end;

     Flptr:=1;
     Felptr:=Length(FLine);
//outputDebugstring(PChar('LINE) '+FLine));
     Result:=(not bEOF);
end;

function TkbmCustomCSVStreamFormat.GetWord(var null:boolean):string;
var
   sptr:integer;
   TmpStr:string;
   wasquoted,quoted:boolean;
begin
    Result:='';

    sptr:=Flptr;     // Fsptr=start of word. Flptr=current point in line; Felptr=end of line
    null:=false;
    quoted:=false;
    wasquoted:=false;

    while true do
    begin
        // Check for end of line (implicit record delimiter)
        if Flptr>Felptr then
        begin
             TmpStr:=Copy(FLine,sptr,Flptr-sptr);
             Result:=Result+TmpStr;
             break;
        end;

        // Check if quote.
        if FLine[Flptr]=FCSVQuote then
        begin
             // Check if next character is also a quote. Then its a quote that is part of text.
             if quoted and (Flptr<Felptr) and (FLine[Flptr+1]=FCSVQuote) then
                inc(Flptr)
             else
                 quoted:=not quoted;

             // Remember if the text was quoted. Makes a difference in detecting null value.
             if quoted then
                wasquoted:=true;

             // Copy text until quote to result.
             TmpStr:=Copy(FLine,sptr,Flptr-sptr);
             Result:=Result+TmpStr;
             inc(Flptr);
             sptr:=Flptr;
             continue;
        end;

        // Check for field delimiter.
        if FLine[Flptr]=FCSVFieldDelimiter then
        begin
             // If not quoted, then we have got a field.
             if not quoted then
             begin
                  TmpStr:=Copy(FLine,sptr,Flptr-sptr);
                  Result:=Result+TmpStr;
                  inc(Flptr);
                  break;
             end;
             inc(Flptr);
             continue;
        end;

        // Check for record delimiter.
        if FLine[Flptr]=FCSVRecordDelimiter then
        begin
             // If not quoted, then we have got a field (end of record).
             if not quoted then
             begin
                  TmpStr:=Copy(FLine,sptr,Flptr-sptr);
                  Result:=Result+TmpStr;
                  inc(Flptr);
                  break;
             end;
             inc(Flptr);
             continue;
        end;

        // Normal text.
        inc(Flptr);
    end;

    // Check if null value.
    null:=(not wasquoted) and (length(Result)=0);
end;

procedure TkbmCustomCSVStreamFormat.BeforeLoad(ADataset:TkbmCustomMemTable);
begin
     FDefLoaded:=false;
     inherited;

     // Allocate space for a buffer.
     SetLength(FBuf,CSVBUFSIZE);

     // Still nothing in the buffer to handle.
     FDataset:=ADataset;
     FBufSize:=0;
     FStreamSize:=WorkStream.Size;
     FStartPosition:=WorkStream.Position;
     FProgressCnt:=0;

     FLocalFormat := Self.FormatSettings;

     // Check if to load in local format.
     if not (sfLoadLocalFormat in sfLocalFormat) then
     begin
          FLocalFormat.DateSeparator:='/';
          FLocalFormat.TimeSeparator:=':';
          FLocalFormat.ThousandSeparator:=',';
          FLocalFormat.DecimalSeparator:='.';
          FLocalFormat.ShortDateFormat:='dd/mm/yyyy';
          FLocalFormat.ShortTimeFormat:='HH:nn:ss';
          FLocalFormat.LongTimeFormat:='HH:nn:ss';
          FLocalFormat.CurrencyString:='';
          FLocalFormat.CurrencyFormat:=0;
          FLocalFormat.NegCurrFormat:=1;
     end;
end;

procedure TkbmCustomCSVStreamFormat.AfterLoad(ADataset:TkbmCustomMemTable);
begin
     SetLength(FBuf,0);

     inherited;
end;

procedure TkbmCustomCSVStreamFormat.DetermineLoadFieldIDs(ADataset:TkbmCustomMemTable; AList:TStringList; Situation:TkbmDetermineLoadFieldsSituation);
var
   s:string;
   null:boolean;
begin
     // Dont try to get fields display names if def not yet loaded.
     if (Situation<>dlfAfterLoadDef) then
     begin
          LoadFieldIDs(ADataset,AList,UseFieldDisplayName);
          exit;
     end;

     if (sfLoadByFieldNo in sfDef) then
     begin
          LoadFieldIDs(ADataset,AList,false);
          exit;
     end;

     if (sfAutoLayoutDef in sfDef) then
     begin
          LoadFieldIDs(ADataset,AList,UseFieldDisplayName);
          exit;
     end;

     if (FLine='') or (sfLoadNoHeader in sfNoHeader) then
     begin
          LoadFieldIDs(ADataset,AList,UseFieldDisplayName);
          exit;
     end;

     // Determine which fields is present in the stream.
     // Line has already been populated by LoadDef.
     AList.Clear;

     Flptr:=1;
     Felptr:=length(FLine);
     while (Flptr<=Felptr) do
     begin
          // Get DisplayName for field.
          s:=GetWord(null);
          AList.Add(s);
     end;
end;

procedure TkbmCustomCSVStreamFormat.DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation);
var
   i:TkbmNativeInt;
   s:string;
begin
     // If determined not to load field, dont.
     if (Situation<>dlfAfterLoadDef) then exit;

     // Dont want to worry about case.
     s:=UpperCase(ID);

     // Find Field index in dataset
     for i:=0 to ADataset.FieldCount-1 do
     begin
          if (UpperCase(ADataset.Fields[i].DisplayName)=s) then
          begin
               NewIndex:=i;
               exit;
          end;
     end;

     NewIndex:=-1;
end;

procedure TkbmCustomCSVStreamFormat.LoadDef(ADataset:TkbmCustomMemTable);
  procedure AddField(AName:string);
  var
     fd:TFieldDef;
  begin
       if AName='' then
          AName:='Field_'+inttostr(ADataSet.FieldCount+1);

       // Create fielddef.
       fd:=ADataSet.FieldDefs.AddFieldDef;
       fd.Name:=AName;
       fd.DataType:=ftString;
       fd.Size:=FDefaultStringFieldSize;
       fd.Required:=false;

       // Create field.
       fd.CreateField(ADataset);
  end;
var
   s:string;
   ld,ald,ldidx,hdrline:boolean;
   i:TkbmNativeInt;
   slist:TStringList;
   DuringTableDef,DuringIndexDef:boolean;
   null:boolean;
   FName,KName,TName,DName,EMask,DExpr:string;
   FSize,DSize:integer;
   REQ,RO,INV,CASEIN,NONMT,DESC,UNIQ:boolean;
   FT:TFieldType;
   FK:TFieldKind;
   FFields:string;
   ioptions:TIndexOptions;
   AIndexDef:TIndexDef;
   fd:TFieldDef;
   AField:TField;
begin
     if (FStreamSize = 0) then exit;
     ald:=sfAutoLayoutDef in sfDef;
     ld:=sfLoadDef in sfDef;
     ldidx:=sfLoadIndexDef in sfIndexDef;
     hdrline:=not (sfLoadNoHeader in sfNoHeader);

     // Check if to autolayoutdef based on CSV contents.
     if ald then
     begin
          ADataSet.FieldDefs.Clear;
          ADataSet.DeleteTable;
{$IFDEF LEVEL21}
          ADataset.Fields.LifeCycles:=[lcAutomatic];
{$ENDIF}

          if not GetLine then
             exit;
          if FLine='' then
             exit;

          // Build definitions based on the datafields.
          Flptr:=1;
          while (Flptr<=Felptr) do
          begin
               s:=GetWord(null);

               // If a headerline is available, use it to define the fieldname.
               if hdrline then
                  AddField(s)
               else
                   AddField('');
          end;
          ADataSet.Open;

          // Reset so data loader is rereading this line again.
          if not hdrline then
             Flptr:=1;

          FDefLoaded:=true;
          exit;
     end;

     // Read all definition lines in CSV format.
     slist:=TStringList.Create;
     DuringTableDef:=false;
     DuringIndexDef:=false;
     try
        while true do
        begin
             if not GetLine then
                break;
             if FLine='' then
                break;

             // Read magic words if any.
             FWord:=GetWord(null);

{$IFNDEF CSV_FILE_1XX_COMPATIBILITY}
             if FWord=kbmFileVersionMagic then
             begin
                  FWord:=GetWord(null);
//                     FileVersion:=StrToInt(Word);
                  continue;
             end
             else
{$ENDIF}

             if FWord=kbmTableDefMagicStart then
             begin
                  DuringTableDef:=true;
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
                  continue;
             end

             // End of table definition?
             else if FWord=kbmTableDefMagicEnd then
             begin
                  DuringTableDef:=false;
                  ADataSet.Open;
                  continue;
             end

             // Start of index definitions?
             else if FWord=kbmIndexDefMagicStart then
             begin
                  if ld and ldidx then
                  begin
                       ADataSet.DestroyIndexes;
                       ADataSet.IndexDefs.Clear;
                  end;
                  DuringIndexDef:=true;
                  continue;
             end

             // End of index definitions?
             else if FWord=kbmIndexDefMagicEnd then
             begin
                  DuringIndexDef:=false;
                  if ld and ldidx then ADataSet.CreateIndexes;
                  continue;
             end;

             // If not during table definitions then its the header. Break.
             if not DuringTableDef then break;

             // If its an index definition.
             if DuringIndexDef then
             begin
                  if ld and ldidx then
                  begin
                       FLine:=ExtractQuoteString(FLine,FCSVQuote);
                       i:=pos('=',FLine);
                       slist.CommaText:=copy(FLine,i+1,length(FLine));
                       FName:=copy(FLine,1,i-1);
                       FFields:=slist.Strings[0];
                       DName:=slist.Strings[1];
                       CASEIN:=pos(',CASE',FLine)<>0;
                       NONMT:=pos(',NONMT',FLine)<>0;
                       DESC:=pos(',DESC',FLine)<>0;
                       UNIQ:=pos(',UNIQ',FLine)<>0;

                       // Add field definition.
                       ioptions:=[];
                       if CASEIN then ioptions:=ioptions+[ixCaseInSensitive];
                       if DESC then ioptions:=ioptions+[ixDescending];
                       if UNIQ then ioptions:=ioptions+[ixUnique];
                       if NONMT then ioptions:=ioptions+[ixNonMaintained];
                       AIndexDef := ADataSet.IndexDefs.AddIndexDef;
                       AIndexDef.Name:=FName;
                       AIndexDef.Fields:=FFields;
                       AIndexDef.Options:=ioptions;
{$IFNDEF FPC}
                       AIndexDef.DisplayName:=DName;
{$ENDIF}
                  end;
                  continue;
             end;

             // Otherwise its a field definition. Break the line apart.
             if ld then
             begin
                  FLine:=ExtractQuoteString(FLine,FCSVQuote);
                  i:=pos('=',FLine);
                  slist.CommaText:=copy(FLine,i+1,length(FLine));
                  FName:=copy(FLine,1,i-1);
                  TName:=slist.Strings[0];
                  FSize:=strtoint(slist.Strings[1]);
                  DName:=slist.Strings[2];
                  EMask:=slist.Strings[3];
                  DSize:=strtoint(slist.Strings[4]);
                  REQ:=pos(',REQ',FLine)<>0;
                  RO:=pos(',RO',FLine)<>0;
                  INV:=pos(',INV',FLine)<>0;
                  i:=slist.Count;
                  DExpr:='';
                  if i>6 then
                  begin
                       DExpr:=slist.Strings[i-1];
                       dec(i);
                  end;
                  if i>5 then
                     KName:=slist.Strings[i-1]
                  else
                      KName:=FieldKindNames[0]; // fkData.

                  // Find fieldtype from fieldtypename.
                  for i:=0 to ord(High(FieldTypeNames)) do
                      if SameText(FieldTypeNames[TFieldType(i)],TName) then
                         break;
                  FT:=TFieldType(i);
                  if not (FT in kbmSupportedFieldTypes) then
                     raise EMemTableError.Create(Format(kbmUnknownFieldErr1+kbmUnknownFieldErr2,[TName,FWord]));

                  // Check if autoinc field in stream, use data from stream.
                  if FT=ftAutoInc then
                     SetIgnoreAutoIncPopulation(ADataset,true);

                  // If fieldkind specified, find fieldkind.
                  FK:=fkData;
                  for i:=0 to ord(High(FieldKindNames)) do
                      if SameText(FieldKindNames[i],KName) then
                      begin
                           FK:=TFieldKind(i);
                           break;
                      end;

                  // Add field definition.
                  fd:=ADataSet.FieldDefs.AddFieldDef;
                  fd.Name:=FName;
                  fd.DataType:=FT;
                  fd.Size:=FSize;
                  fd.Required:=REQ;

                  // Setup other properties.
                  AField:=fd.CreateField(ADataset);
                  AField.FieldKind:=FK;
                  AField.DisplayLabel:=DName;
{$IFNDEF FPC}
                  AField.EditMask:=EMask;
{$ENDIF}
                  AField.ReadOnly:=RO;
                  AField.DisplayWidth:=DSize;
                  AField.DefaultExpression:=DExpr;
                  AField.Visible:=not INV;
             end;
        end;
     finally
        slist.free;
     end;
     FDefLoaded:=true;
end;

function TkbmCustomCSVStreamFormat.GetFieldHeaderText(const AField:TField):string;
begin
     if UseFieldDisplayName then
        Result:=AField.DisplayName
     else
         Result:=AField.FieldName;
end;

procedure TkbmCustomCSVStreamFormat.LoadFieldIDs(const ADataset:TkbmCustomMemTable; const AList:TStringList; const AUseFieldDisplayName:boolean);
var
   i:TkbmNativeInt;
begin
     AList.Clear;
     for i:=0 to ADataset.FieldCount-1 do
         if AUseFieldDisplayName then
            AList.Add(ADataset.Fields[i].DisplayName)
         else
             AList.Add(ADataset.Fields[i].FieldName);
end;

procedure TkbmCustomCSVStreamFormat.LoadData(ADataset:TkbmCustomMemTable);
var
   i,j,k:integer;
   nf:integer;
   null:boolean;
   bWidths:boolean;
   s:string;
   Accept:boolean;
   LoadLine:boolean;
   bEOF:boolean;
   dt:TFieldType;
   ai:boolean;
begin
     if (FStreamSize = 0) then exit;
     if not (sfLoadData in sfData) then exit;

     ai:=sfLoadGenerateAutoInc in sfAutoInc;
     if ai then
        SetIgnoreAutoIncPopulation(ADataset,false);

     // Check if data line already loaded.
     LoadLine:=not (sfLoadNoHeader in sfNoHeader);
     bWidths:=sfLoadDetermineWidth in sfDisplayWidth;

     ADataSet.ResetAutoInc;

     // Read all lines in CSV format.
     ADataSet.LoadCount:=0;
     ADataSet.LoadedCompletely:=true;

     bEOF:=false;
     while not bEOF do
     begin
          if (ADataSet.LoadLimit>0) and (ADataSet.LoadCount>=ADataSet.LoadLimit) then
          begin
               ADataSet.LoadedCompletely:=false;
               break;
          end;

          if LoadLine then
             bEOF:=not GetLine;
          LoadLine:=true;
          if FLine='' then
             continue;
          Flptr:=1;            // Make sure word pointer is at start of line.

          ADataSet.append;

          i:=0;
          nf:=length(LoadFields);
          while (Flptr<=Felptr) and (i<nf) do
          begin
               j:=LoadFields[i];
               s:=GetWord(null);
               if j>=0 then
               begin
                    if not (sfLoadFieldKind in sfFieldKind) then
                    begin
                         if ADataSet.Fields[j].FieldKind<>fkData then
                         begin
                              inc(i);
                              continue;
                         end;
                    end;

                    if assigned(FOnFormatLoadField) then
                       FOnFormatLoadField(self,ADataSet.Fields[j],null,s);

                    dt:=ADataSet.Fields[j].DataType;
                    if (dt<>ftAutoInc) or (not ai) then
                    begin
                         if null then
                            ADataSet.Fields[j].Clear
                         else if dt in kbmStringTypes then
                         begin
                              s:=CodedStringToString(s,FEscapeChar);
                              ADataSet.Fields[j].AsString:=s;
                              if bWidths then
                              begin
                                   k:=length(s);
                                   if k>LoadFieldWidths[i] then
                                      LoadFieldWidths[i]:=k;
                              end;
                         end
                         else if dt in kbmBinaryTypes then
{$IFDEF FPC}
                              ADataSet.Fields[j].AsBytes:=Base64ToBuffer(TBytes(s))
{$ELSE}
                              ADataSet.Fields[j].AsBytes:=Base64ToBuffer(TEncoding.ASCII.GetBytes(s))
{$ENDIF}

                         else if dt = ftBoolean then
                         begin
                              if s=FCSVTrueString then
                                 TBooleanField(ADataSet.Fields[j]).Value:=true
                              else
                                  TBooleanField(ADataSet.Fields[j]).Value:=false;
                         end
                         else if dt in [ftFloat,
                                        ftCurrency,
                                        ftBCD
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                       ,ftSingle
{$ENDIF}
{$IFDEF LEVEL6}
                                       ,ftFMTBCD
{$ENDIF}                                       ] then
                             ADataSet.Fields[j].AsFloat:=StrToFloat(s,FLocalFormat)
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                         else if dt=ftExtended then
                             ADataSet.Fields[j].AsExtended:=StrToFloat(s,FLocalFormat)
{$ENDIF}
                         else if dt=ftDate then
                                  ADataSet.Fields[i].AsDateTime:=StrToDate(s,FLocalFormat)
                         else if dt=ftTime then
                                  ADataSet.Fields[i].AsDateTime:=StrToTime(s,FLocalFormat)
                         else if dt=ftDateTime then
                                  ADataSet.Fields[i].AsDateTime:=StrToDateTime(s,FLocalFormat)
                         else
                             ADataSet.Fields[j].AsString:=s;
                    end;

                    if Assigned(ADataSet.OnLoadField) then ADataSet.OnLoadField(ADataSet,j,ADataSet.Fields[j]);
               end;
               inc(i);
          end;

          Accept:=true;
          if Assigned(ADataSet.OnLoadRecord) then ADataSet.OnLoadRecord(ADataset,Accept);
          if Accept then
          begin
               ADataSet.Post;
               ADataSet.LoadCount:=ADataSet.LoadCount+1;
          end
          else
              ADataSet.Cancel;
     end;
end;

// -----------------------------------------------------------------------------------
// Registration for Delphi 3 / C++ Builder 3
// -----------------------------------------------------------------------------------

end.

