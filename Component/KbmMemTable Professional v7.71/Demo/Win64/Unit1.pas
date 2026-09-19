unit Unit1;

interface

{$I kbmMemTable.inc}

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  Db, Grids, DBGrids, ExtCtrls, DBCtrls, TeeProcs, TeEngine,
  Chart, StdCtrls, Series, DBChart, kbmMemTable,
  ComCtrls, kbmCompress, Mask, kbmMemCSVStreamFormat,
  kbmMemBinaryStreamFormat

{$ifdef LEVEL6}
  ,variants
{$endif}
;

type
  // An example on how to create a deltahandler.
  TDemoDeltaHandler = class(TkbmCustomDeltaHandler)
  protected
     procedure InsertRecord(var Retry:boolean; var State:TUpdateStatus); override;
     procedure DeleteRecord(var Retry:boolean; var State:TUpdateStatus); override;
     procedure ModifyRecord(var Retry:boolean; var State:TUpdateStatus); override;
  end;

  // The standard form definition.
  TForm1 = class(TForm)
    Panel1: TPanel;
    DBGrid1: TDBGrid;
    DBNavigator1: TDBNavigator;
    DataSource1: TDataSource;
    kbmMemTable1: TkbmMemTable;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    Button6: TButton;
    Button7: TButton;
    Button8: TButton;
    Button1: TButton;
    Panel2: TPanel;
    DBImage1: TDBImage;
    DBMemo1: TDBMemo;
    TabSheet2: TTabSheet;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    TabSheet3: TTabSheet;
    Button11: TButton;
    Label10: TLabel;
    btnLocatePeriod: TButton;
    eSearch: TEdit;
    btnLocateValue: TButton;
    chbCaseInsensitive: TCheckBox;
    chbPartialKey: TCheckBox;
    btnLookupCalc: TButton;
    btnLocateCalc: TButton;
    Label12: TLabel;
    Label13: TLabel;
    Label14: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    eResult: TEdit;
    chbDescending: TCheckBox;
    Label19: TLabel;
    lblLZH: TLabel;
    PageControl2: TPageControl;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    Button2: TButton;
    Label2: TLabel;
    Label3: TLabel;
    Button3: TButton;
    Label4: TLabel;
    Button4: TButton;
    Button5: TButton;
    Label5: TLabel;
    Memo1: TMemo;
    Panel4: TPanel;
    BinarySave: TCheckBox;
    cbSortField: TComboBox;
    Label24: TLabel;
    btnFindNearest: TButton;
    eRecordCount: TEdit;
    Label25: TLabel;
    Label26: TLabel;
    lRecNo: TLabel;
    TabSheet7: TTabSheet;
    btnGetBookmark: TButton;
    Label11: TLabel;
    btnGotoBookmark: TButton;
    Label29: TLabel;
    TabSheet8: TTabSheet;
    btnRebuildIdx: TButton;
    Label30: TLabel;
    Label31: TLabel;
    cbIndexes: TComboBox;
    chbEnableIndexes: TCheckBox;
    btnAddIndex: TButton;
    Label32: TLabel;
    btnDeleteIndex: TButton;
    Label33: TLabel;
    chbSaveIndexDef: TCheckBox;
    Panel6: TPanel;
    DBEdit1: TDBEdit;
    Label34: TLabel;
    chbRandomColor: TCheckBox;
    chbColorUnique: TCheckBox;
    chbColorDescending: TCheckBox;
    Label35: TLabel;
    lOldValue: TLabel;
    TabSheet10: TTabSheet;
    Button14: TButton;
    Label36: TLabel;
    Label37: TLabel;
    chbVersionAll: TCheckBox;
    Label38: TLabel;
    Button15: TButton;
    chbSaveDeltas: TCheckBox;
    Button16: TButton;
    Label39: TLabel;
    TabSheet11: TTabSheet;
    Button17: TButton;
    Button18: TButton;
    Label40: TLabel;
    Label41: TLabel;
    TabSheet12: TTabSheet;
    Button19: TButton;
    Button20: TButton;
    Button21: TButton;
    Label42: TLabel;
    lTransactionLevel: TLabel;
    Label43: TLabel;
    Label44: TLabel;
    Label45: TLabel;
    chbGenerateMemos: TCheckBox;
    TabSheet13: TTabSheet;
    Label28: TLabel;
    eFilter: TEdit;
    TableFilteredCheckBox: TCheckBox;
    btnSetFilter: TButton;
    Label22: TLabel;
    btnSetRange: TButton;
    btnCancelRange: TButton;
    Label23: TLabel;
    Panel7: TPanel;
    Label46: TLabel;
    lProgress: TLabel;
    Button22: TButton;
    Button13: TButton;
    Button10: TButton;
    Label47: TLabel;
    Button23: TButton;
    chbNoQuotes: TCheckBox;
    Button24: TButton;
    Label48: TLabel;
    sfBinary: TkbmBinaryStreamFormat;
    sfCSV: TkbmCSVStreamFormat;
    sfBinaryWithDeltas: TkbmBinaryStreamFormat;
    Button25: TButton;
    Button26: TButton;
    TabSheet9: TTabSheet;
    btnMoveRecord: TButton;
    btnCopyRecord: TButton;
    eRowNum: TEdit;
    Label49: TLabel;
    chbIgnoreMDSave: TCheckBox;
    chbIgnoreRangeOnSave: TCheckBox;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure MemTable1CalcFields(DataSet: TDataSet);
    procedure Button4Click(Sender: TObject);
    procedure Button5Click(Sender: TObject);
    procedure Button6Click(Sender: TObject);
    procedure Button7Click(Sender: TObject);
    procedure Button8Click(Sender: TObject);
    procedure Button11Click(Sender: TObject);
    procedure kbmMemTable1FilterRecord(DataSet: TDataSet;
      var Accept: Boolean);
    procedure TableFilteredCheckBoxClick(Sender: TObject);
    procedure btnLocatePeriodClick(Sender: TObject);
    procedure btnLocateCalcClick(Sender: TObject);
    procedure btnLocateValueClick(Sender: TObject);
    procedure btnLookupCalcClick(Sender: TObject);
    procedure TabSheet3Enter(Sender: TObject);
    procedure btnSetRangeClick(Sender: TObject);
    procedure btnCancelRangeClick(Sender: TObject);
    procedure btnFindNearestClick(Sender: TObject);
    procedure kbmMemTable1AfterScroll(DataSet: TDataSet);
    procedure btnGetBookmarkClick(Sender: TObject);
    procedure btnGotoBookmarkClick(Sender: TObject);
    procedure btnRebuildIdxClick(Sender: TObject);
    procedure TabSheet8Enter(Sender: TObject);
    procedure cbIndexesChange(Sender: TObject);
    procedure chbEnableIndexesClick(Sender: TObject);
    procedure btnAddIndexClick(Sender: TObject);
    procedure btnDeleteIndexClick(Sender: TObject);
    procedure kbmMemTable1BytesFieldGetText(Sender: TField;
      var Text: String; DisplayText: Boolean);
    procedure kbmMemTable1BytesFieldSetText(Sender: TField;
      const Text: String);
    procedure btnSetFilterClick(Sender: TObject);
    procedure kbmMemTable1AfterEdit(DataSet: TDataSet);
    procedure Button14Click(Sender: TObject);
    procedure Button15Click(Sender: TObject);
    procedure chbVersionAllClick(Sender: TObject);
    procedure BinarySaveClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure Button16Click(Sender: TObject);
    procedure Button17Click(Sender: TObject);
    procedure Button18Click(Sender: TObject);
    procedure Button19Click(Sender: TObject);
    procedure Button20Click(Sender: TObject);
    procedure Button21Click(Sender: TObject);
    procedure kbmMemTable1Progress(DataSet: TDataSet; Percentage: Integer;
      Code: TkbmProgressCode);
    procedure Button22Click(Sender: TObject);
    procedure Button13Click(Sender: TObject);
    procedure Button10Click(Sender: TObject);
    procedure Button23Click(Sender: TObject);
    procedure Button24Click(Sender: TObject);
    procedure Button25Click(Sender: TObject);
    procedure Button26Click(Sender: TObject);
    procedure btnMoveRecordClick(Sender: TObject);
    procedure btnCopyRecordClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
    CalcField:TStringField;
    bm:TBookmark;
    DeltaHandler:TDemoDeltaHandler;
    SnapShot:variant;
  end;

var
  Form1: TForm1;

implementation

type
   TkbmProtCustomStreamFormat = class(TkbmCustomStreamFormat);

{$R *.DFM}

// An example on how to create a deltahandler.
procedure TDemoDeltaHandler.ModifyRecord(var Retry:boolean; var State:TUpdateStatus);
var
   i:integer;
   s1,s2,sv:string;
   v:variant;
begin
     s1:='';
     s2:='';
     for i:=0 to FieldCount-1 do
     begin
          if (not (Fields[i].DataType in kbmBinaryTypes)) or (Fields[i].DataType=ftMemo) then
          begin
               v:=Values[i];
               if (VarIsNull(v)) then sv:='<NULL>'
               else sv:=v;
               s1:=s1+sv+' ';

               v:=OrigValues[i];
               if (VarIsNull(v)) then sv:='<NULL>'
               else sv:=v;
               s2:=s2+sv+' ';
          end;
     end;
     ShowMessage(Format('Modified record (%s) to (%s)',[s2,s1]));
end;

procedure TDemoDeltaHandler.InsertRecord(var Retry:boolean; var State:TUpdateStatus);
var
   i:integer;
   s,sv:string;
   v:variant;
begin
     s:='';
     for i:=0 to FieldCount-1 do
     begin
          v:=Values[i];
          if not (Fields[i].DataType in kbmBinaryTypes) then
          begin
               if (VarIsNull(v)) then sv:='<NULL>'
               else sv:=v;
               s:=s+sv+' ';
          end;
     end;
     ShowMessage(Format('Inserted record (%s)',[s]));
end;

procedure TDemoDeltaHandler.DeleteRecord(var Retry:boolean; var State:TUpdateStatus);
var
   i:integer;
   s,sv:string;
   v:variant;
begin
     s:='';
     for i:=0 to FieldCount-1 do
     begin
          v:=Values[i];
          if not (Fields[i].DataType in kbmBinaryTypes) then
          begin
               if (VarIsNull(v)) then sv:='<NULL>'
               else sv:=v;
               s:=s+sv+' ';
          end;
     end;
     ShowMessage(Format('Deleted record (%s)',[s]));
end;

// ****************************************************************************
// The following code illustrates an example on creating a TkbmMemTable
// on the fly in runtime code.
//
//
//  // Create the memorytable object, and set a few optionel stuff.
//  kbmMemTable1 := TkbmMemTable.Create(Self); //Owner is Self. It will be auto-destroyed.
//  kbmMemTable1.SortOptions := [];                                           // Optional.
//  kbmMemTable1.PersistentFile := 'testfil.txt';                             // Optional.
//  kbmMemTable1.OnCompress := kbmMemTable1Compress;                          // Optional.
//  kbmMemTable1.OnDecompress := kbmMemTable1Decompress;                      // Optional.
//  kbmMemTable1.OnCompressBlobStream := kbmMemTable1CompressBlobStream;      // Optional.
//  kbmMemTable1.OnDecompressBlobStream := kbmMemTable1DecompressBlobStream;  // Optional.
//  kbmMemTable1.OnCalcFields := MemTable1CalcFields;                         // Optional.
//  kbmMemTable1.OnFilterRecord := kbmMemTable1FilterRecord;                  // Optional.
//  kbmMemTable1.MasterSource := Nil;                                         // Optional.
//
//  //Now, creating the field defs.                                           // Similar required.
//  kbmMemTable1.FieldDefs.Clear; //We dont need this line, but it does not hurt either.
//  kbmMemTable1.FieldDefs.Add('Period', ftInteger, 0, False);
//  kbmMemTable1.FieldDefs.Add('Value', ftInteger, 0, False);
//  kbmMemTable1.FieldDefs.Add('Color', ftInteger, 0, False);
//  kbmMemTable1.FieldDefs.Add('Calc', FtString, 20, False);
//  kbmMemTable1.FieldDefs.Add('Date', ftDate, 0, False);
//
//  // Define index fields.                                                   // Optional.
//  kbmMemTable1.IndexDefs.Add('Index1','Value',[]);
//
//  // Finally create the table according to definitions.                     // Required.
//  kbmMemTable1.CreateTable;
//
//  //Since this is a run-time created one, we have to assign the following here.
//  DataSource1.DataSet := kbmMemTable1;
//
//  // Optionel. IndexFields and SortFields must be assigned AFTER CreateTable
//  kbmMemTable1.IndexFields := 'Value';
//  kbmMemTable1.SortFields := 'Value';
//


procedure TForm1.Button1Click(Sender: TObject);
var
   i,j:integer;
begin
     j:=strtoint(eRecordCount.text);
     with kbmMemTable1 do
     begin
          Close;
          DisableControls;
          try
             Open;
             for i:=1 to j do
             begin
//OutputDebugString(Pchar('i='+inttostr(i)));
                  Append;
                  FieldByName('PERIOD').asinteger:=i;
                  FieldByName('VALUE').asinteger:=(j-i) * 2;
                  if chbRandomColor.Checked then
                     FieldByName('COLOR').asinteger:=Random(j)
                  else
                     FieldByName('COLOR').asinteger:=i*4;
                  FieldByName('Date').AsDateTime:=Now+i-1;
                  FieldByName('String').AsString:='String:'+inttostr(i);
{$IFDEF LEVEL4}
                  FieldByName('WideString').Value:='WideString:'+inttostr(i);
{$ENDIF}

                  if chbGenerateMemos.Checked then
                     FieldByName('Memo').AsString:='This is a memo'+#10+DateTimeToStr(Now)+' '+inttostr(i);

                  Post;
             end;

             // Check if not updated indexes, rebuild and reenable updates of the indexes.
             if EnableIndexes=false then
             begin
                  // Rebuild indexes.
                  UpdateIndexes;
                  EnableIndexes:=true;
                  chbEnableIndexes.Checked:=true;
             end;
          finally
             EnableControls;
          end;
     end;
end;

procedure TForm1.Button2Click(Sender: TObject);
var
   fmt:TkbmCustomStreamFormat;
begin
     if BinarySave.Checked then
        fmt:=sfBinary
     else
         fmt:=sfCSV;


     with TkbmProtCustomStreamFormat(fmt) do
     begin
          if chbSaveIndexDef.Checked then
             sfIndexDef:=[sfSaveIndexDef]
          else
              sfIndexDef:=[];

          if chbSaveDeltas.Checked then
          begin
               sfDeltas:=[sfSaveDeltas];
               sfDontFilterDeltas:=[sfSaveDontFilterDeltas];
          end
          else
          begin
               sfDeltas:=[];
               sfDontFilterDeltas:=[];
          end;

          if chbIgnoreMDSave.Checked then
             sfIgnoreMasterDetail:=[sfSaveIgnoreMasterDetail]
          else
              sfIgnoreMasterDetail:=[];

          if chbIgnoreRangeOnSave.Checked then
             sfIgnoreRange:=[sfSaveIgnoreRange]
          else
              sfIgnoreRange:=[];

          if chbNoQuotes.Checked then
          begin
               sfCSV.CSVQuote:=#0;
               sfCSV.CSVRecordDelimiter:=#0;
          end
          else
          begin
               sfCSV.CSVQuote:='"';
               sfCSV.CSVRecordDelimiter:=',';
          end;
     end;

     if BinarySave.Checked then
        kbmMemTable1.SaveToFileViaFormat('c:\temp\test.bin',fmt)
     else
        kbmMemTable1.SaveToFileViaFormat('c:\temp\test.csv',fmt);
end;

procedure TForm1.Button3Click(Sender: TObject);
begin
     if chbNoQuotes.Checked then
     begin
          sfCSV.CSVQuote:=#0;
          sfCSV.CSVRecordDelimiter:=#0;
     end
     else
     begin
          sfCSV.CSVQuote:='"';
          sfCSV.CSVRecordDelimiter:=',';
     end;

     if BinarySave.Checked then
        kbmMemTable1.LoadFromFileViaFormat('c:\temp\test.bin',sfBinary)
     else
        kbmMemTable1.LoadFromFileViaFormat('c:\temp\test.csv',sfCSV);
     DBImage1.DataField:='graphic';
     DBImage1.DataSource:=DataSource1;
     DBMemo1.DataField:='Notes';
     DBMemo1.DataSource:=DataSource1;
end;

procedure TForm1.MemTable1CalcFields(DataSet: TDataSet);
var
   i:integer;
   s:string;
begin
     if kbmMemTable1.Fields[0].IsNull then
        kbmMemTable1.FieldByName('CALC').AsString:='NULL'
     else
     begin
          i:=kbmMemTable1.Fields[0].AsInteger;
          s:=LongMonthNames[(i mod 12) + 1];
          kbmMemTable1.Fieldbyname('CALC').AsString := kbmMemTable1.Fields[0].AsString + '-' + s;
     end;
end;

procedure TForm1.Button4Click(Sender: TObject);
begin
     Memo1.Lines.Text:=kbmMemTable1.CommaText;
end;

procedure TForm1.Button5Click(Sender: TObject);
begin
     kbmMemTable1.CommaText:=Memo1.Lines.Text;
end;

// Dynamically define a set of fields.
{$DEFINE CALC}
procedure TForm1.Button6Click(Sender: TObject);
begin
     with kbmMemTable1 do
     begin
          Close;
          MasterSource:=nil;

          // Define data fields.
          with kbmMemTable1.FieldDefs do
          begin
               Clear;
               Add('Period', ftInteger, 0, false);
               Add('VALUE', ftInteger, 0, false);
               Add('String', ftString, 30, false);
               Add('BytesField', ftBytes, 20, false);
               Add('Color', ftInteger, 0, false);
               Add('Date', ftDate, 0, false);
               Add('Bool', ftBoolean, 0, false);
               Add('Memo', ftMemo, 0, false);
               Add('AutoInc', ftAutoInc,0,false);
               Add('Currency',ftFloat,0,false);
//               Add('Currency',ftCurrency,0,false);
{$IFDEF LEVEL4}
               Add('WideString',ftWideString,40,false);
{$ENDIF}
          end;

          // Define index fields.
          with kbmMemTable1.IndexDefs do
          begin
               Clear;
//               Add('Period','PERIOD',[]);
//               Add('Index1','VALUE',[ixdescending]);
//               Add('StringIndex','String',[]);
//               Add('combined','PERIOD;VALUE',[]);
               Add('descending','PERIOD',[ixDescending]);
//               Add('Index2','Color;Period',[]);
          end;

          // Create the table according to definitions.
          CreateTable;

          TCurrencyField(FieldByName('Currency')).DisplayFormat:='$###0.00';

          // Setup eventhandlers for dynamically created bytefield.
          with FieldByName('BytesField') do
          begin
               OnSetText:=kbmMemTable1BytesFieldSetText;
               OnGetText:=kbmMemTable1BytesFieldGetText;
          end;

          // Define sorting and index.
          IndexFieldNames := 'VALUE';
     end;

{$IFDEF CALC}
     // Define calculated field.
     CalcField:=TStringField.Create(self);
     CalcField.FieldKind:=fkCalculated;
     CalcField.Size:=20;
     CalcField.FieldName:='CALC';
     CalcField.DataSet:=kbmMemTable1;
{$ENDIF}

     // Setup other dataaware controls.
     DBEdit1.DataSource:=DataSource1;
     DBEdit1.DataField:='BytesField';
     DBMemo1.DataSource:=DataSource1;
     DBMemo1.DataField:='Memo';
end;

procedure TForm1.Button7Click(Sender: TObject);
begin
     kbmMemTable1.Open;
end;

procedure TForm1.Button8Click(Sender: TObject);
begin
     kbmMemTable1.Close;
end;

procedure TForm1.Button11Click(Sender: TObject);
var
   Options:TkbmMemTableCompareOptions;
begin
     Options := [];
     if chbDescending.Checked then Options:=Options + [mtcoDescending];
     if chbCaseInsensitive.Checked then Options:=Options + [mtcoCaseInsensitive];
     kbmMemTable1.SortOn(cbSortField.Text, Options);
end;

procedure TForm1.kbmMemTable1FilterRecord(DataSet: TDataSet; var Accept: Boolean);
begin
     // Example of runtime fast filtering.  Only select records where period>10.
     //     Accept:=DataSet.FieldByName('Period').AsInteger > 10;
end;

procedure TForm1.TableFilteredCheckBoxClick(Sender: TObject);
begin
     DataSource1.DataSet.Filtered:=TCheckBox(Sender).Checked;
end;

procedure TForm1.btnLocatePeriodClick(Sender: TObject);
begin
     if DataSource1.DataSet.Locate('PERIOD', eSearch.Text, []) then
        ShowMessage('Found')
     else
         ShowMessage('Not found');
end;

procedure TForm1.btnLocateCalcClick(Sender: TObject);
var
   Options:TLocateOptions;
begin
     Options:=[];
     if chbCaseInsensitive.Checked then Include(Options,loCaseInsensitive);
     if chbPartialKey.Checked then Include(Options,loPartialKey);
     DataSource1.DataSet.Locate('CALC', eSearch.Text, Options);
end;

procedure TForm1.btnLocateValueClick(Sender: TObject);
begin
     DataSource1.DataSet.Locate('VALUE', eSearch.Text, []);
end;

procedure TForm1.btnLookupCalcClick(Sender: TObject);
begin
     eResult.Text:=VarToStr(DataSource1.DataSet.Lookup('PERIOD', eSearch.Text, 'CALC'));
end;

procedure TForm1.btnMoveRecordClick(Sender: TObject);
begin
     kbmMemTable1.IndexName:='';
     kbmMemTable1.MoveRecord(kbmMemTable1.RecNo,strtoint(eRowNum.Text));
     kbmMemTable1.Refresh;
end;

procedure TForm1.TabSheet3Enter(Sender: TObject);
var
   i:integer;
begin
     // Build list of fields to sort on.
     cbSortField.Clear;
     for i:=0 to kbmMemTable1.FieldCount-1 do
         cbSortField.Items.Add(kbmMemTable1.Fields[i].FieldName);
     cbSortField.ItemIndex:=0;
end;

procedure TForm1.btnSetRangeClick(Sender: TObject);
begin
     kbmMemTable1.IndexName:='PERIOD';
     kbmMemTable1.SetRange([51],[69]);
end;

procedure TForm1.btnCancelRangeClick(Sender: TObject);
begin
     kbmMemTable1.CancelRange;
end;

procedure TForm1.btnCopyRecordClick(Sender: TObject);
begin
     kbmMemTable1.IndexName:='';
     kbmMemTable1.CopyRecord(kbmMemTable1.RecNo,strtoint(eRowNum.Text));
     kbmMemTable1.Refresh;
end;

procedure TForm1.btnFindNearestClick(Sender: TObject);
begin
     kbmMemTable1.IndexFieldNames:='VALUE';
     kbmMemtable1.FindNearest([eSearch.Text]);
end;

procedure TForm1.kbmMemTable1AfterScroll(DataSet: TDataSet);
begin
     LRecNo.caption:=IntToStr(DataSet.RecNo)+'/'+IntToStr(DataSet.RecordCount);
end;

procedure TForm1.btnGetBookmarkClick(Sender: TObject);
begin
     bm:=kbmMemTable1.GetBookmark;
end;

procedure TForm1.btnGotoBookmarkClick(Sender: TObject);
begin
     kbmMemTable1.GotoBookmark(bm);
end;

procedure TForm1.btnRebuildIdxClick(Sender: TObject);
begin
     kbmMemTable1.Indexes.ReBuildAll;
end;

procedure TForm1.TabSheet8Enter(Sender: TObject);
var
   i:integer;
begin
     // Build list of indexes currently available.
     cbIndexes.Clear;
     cbIndexes.Items.Add('');
     for i:=0 to kbmMemTable1.IndexDefs.Count-1 do
         cbIndexes.Items.Add(kbmMemTable1.IndexDefs.Items[i].Name);
end;

procedure TForm1.cbIndexesChange(Sender: TObject);
begin
     kbmMemTable1.IndexName:=cbIndexes.Text;
end;

procedure TForm1.chbEnableIndexesClick(Sender: TObject);
begin
     kbmMemTable1.EnableIndexes:=chbEnableIndexes.Checked;
end;

procedure TForm1.btnAddIndexClick(Sender: TObject);
var
   io:TIndexOptions;
begin
     io:=[];
     if chbColorUnique.checked then io:=io+[ixUnique];
     if chbColorDescending.checked then io:=io+[ixDescending];
     kbmMemTable1.AddIndex('testindex','COLOR',io);

     // Update combobox list of indexes.
     TabSheet8Enter(nil);
end;

procedure TForm1.btnDeleteIndexClick(Sender: TObject);
begin
     kbmMemTable1.DeleteIndex('testindex');

     // Update combobox list of indexes.
     TabSheet8Enter(nil);
end;

procedure TForm1.kbmMemTable1BytesFieldGetText(Sender: TField;
  var Text: String; DisplayText: Boolean);
var
   len:integer;
   h,l:byte;
   s:string;
begin
     if Sender.IsNull then Text:='(NULL)'
     else
     begin
          s:=Sender.AsString;
          h:=ord(s[1]);
          l:=ord(s[2]);
          len:=h*256 + l;
          Text:=Copy(s,3,len);
     end;
end;

procedure TForm1.kbmMemTable1BytesFieldSetText(Sender: TField;
  const Text: String);
var
   len:integer;
   h,l:byte;
begin
     len:=length(Text);
     if len=0 then Sender.Clear
     else
     begin
          h:=len div 256;
          l:=len mod 256;
          Sender.AsString:=chr(h)+chr(l)+Text;
     end;
end;

procedure TForm1.btnSetFilterClick(Sender: TObject);
begin
     DataSource1.DataSet.Filter:=eFilter.Text;
     TableFilteredCheckBox.Visible:=trim(DataSource1.DataSet.Filter)<>'';
end;

procedure TForm1.kbmMemTable1AfterEdit(DataSet: TDataSet);
var
   fld:TField;
begin
     fld:=DataSet.FindField('VALUE');
     if fld<>nil then lOldValue.Caption:=fld.OldValue;
end;

procedure TForm1.Button14Click(Sender: TObject);
begin
     kbmMemTable1.CheckPoint;
end;

procedure TForm1.Button15Click(Sender: TObject);
begin
     kbmMemTable1.SaveToFileViaFormat('c:\deltas.dat',sfBinaryWithDeltas);
end;

procedure TForm1.chbVersionAllClick(Sender: TObject);
begin
     if chbVersionAll.checked then
        kbmMemTable1.VersioningMode:=mtvmAllSinceCheckPoint
     else
        kbmMemTable1.VersioningMode:=mtvm1SinceCheckPoint;
     if kbmMemTable1.Active then kbmMemTable1.CheckPoint;
end;

procedure TForm1.BinarySaveClick(Sender: TObject);
begin
     chbSaveDeltas.Enabled:=BinarySave.Checked;
     if not chbSaveDeltas.Enabled then chbSaveDeltas.Checked:=false;
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
     // Create the demo delta handler.
     DeltaHandler:=TDemoDeltaHandler.Create(nil);
     kbmMemTable1.DeltaHandler:=DeltaHandler;
end;

procedure TForm1.FormDestroy(Sender: TObject);
begin
     DeltaHandler.Free;
end;

procedure TForm1.Button16Click(Sender: TObject);
begin
     if assigned(kbmMemTable1.DeltaHandler) then kbmMemTable1.DeltaHandler.Resolve;
     kbmMemTable1.CheckPoint;
end;

procedure TForm1.Button17Click(Sender: TObject);
begin
     SnapShot:=kbmMemTable1.AllData;
end;

procedure TForm1.Button18Click(Sender: TObject);
begin
     kbmMemTable1.AllData:=SnapShot;
end;

procedure TForm1.Button19Click(Sender: TObject);
begin
     kbmMemTable1.VersioningMode:=mtvmAllSinceCheckPoint;
     kbmMemTable1.StartTransaction;
     lTransactionLevel.caption:=inttostr(kbmMemTable1.TransactionLevel);
end;

procedure TForm1.Button20Click(Sender: TObject);
begin
     DBGrid1.SelectedRows.Clear;
     kbmMemTable1.Commit;
     lTransactionLevel.caption:=inttostr(kbmMemTable1.TransactionLevel);
end;

procedure TForm1.Button21Click(Sender: TObject);
begin
     kbmMemTable1.Rollback;
     lTransactionLevel.caption:=inttostr(kbmMemTable1.TransactionLevel);
end;

procedure TForm1.kbmMemTable1Progress(DataSet: TDataSet;
  Percentage: Integer; Code: TkbmProgressCode);
const
   ProgressStrings:array [0..ord(mtpcSort)] of string = (
     'Load','Save','Empty','Pack','Checkpoint','Search','Copy','Update','Sort');
begin
     lProgress.Caption:=ProgressStrings[ord(Code)]+' '+inttostr(Percentage)+'%';
     lProgress.Refresh;
end;

procedure TForm1.Button22Click(Sender: TObject);
begin
     DBMemo1.DataSource.DataSet.Edit;
     DBMemo1.Field.Clear;
end;

procedure TForm1.Button13Click(Sender: TObject);
var
   i:integer;
begin
     for i:=0 to 100 do
     begin
          kbmMemTable1.AppendRecord([i,100-i,'S_'+inttostr(i*2)]);
     end;
end;

procedure TForm1.Button10Click(Sender: TObject);
begin
     kbmMemTable1.RecNo:=10;
end;

procedure TForm1.Button23Click(Sender: TObject);
begin
     kbmMemTable1.IndexFieldNames:='VALUE';
     if kbmMemtable1.FindKey([eSearch.Text]) then
        ShowMessage('Found')
     else
         ShowMessage('Not Found');
end;

procedure TForm1.Button24Click(Sender: TObject);
var
   io:TIndexOptions;
begin
     io:=[];
     if chbColorUnique.checked then io:=io+[ixUnique];
     if chbColorDescending.checked then io:=io+[ixDescending];
     kbmMemTable1.AddFilteredIndex('filteredindex','PERIOD',io,'Period<70',[]{$IFNDEF LEVEL5},nil{$ENDIF});

     // Update combobox list of indexes.
     TabSheet8Enter(nil);
end;

procedure TForm1.Button25Click(Sender: TObject);
begin
     kbmMemTable1.PackTable;
end;

procedure TForm1.Button26Click(Sender: TObject);
begin
     kbmMemTable1.EmptyTable;
end;

end.

