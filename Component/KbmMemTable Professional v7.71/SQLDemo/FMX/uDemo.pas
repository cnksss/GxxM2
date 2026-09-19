unit uDemo;

interface

uses
  System.SysUtils, System.Types, System.UITypes, System.Classes, System.Variants,
  FMX.Types, FMX.Controls, FMX.Forms, FMX.Dialogs, FMX.Layouts, FMX.Memo,
  Data.DB, kbmMemTable, Data.Bind.EngExt, Fmx.Bind.DBEngExt,
  Data.Bind.Components, Data.Bind.DBScope, FMX.Grid, Fmx.Bind.Editors,
  Data.Bind.DBLinks, Fmx.Bind.DBLinks, kbmMemSQL, kbmSQLMemTableAPI,
  kbmMemCSVStreamFormat;

type
  TForm1 = class(TForm)
    mtTable1: TkbmMemTable;
    mtTable1fld1: TStringField;
    mtTable1fld2: TIntegerField;
    mtTable1fld3: TIntegerField;
    mtTable1fld4: TStringField;
    dsTable1: TDataSource;
    mtTable1fld5: TIntegerField;
    mSQL: TMemo;
    bsTable1: TBindScopeDB;
    BindingsList1: TBindingsList;
    Panel1: TPanel;
    Splitter1: TSplitter;
    Label1: TLabel;
    sgRawData: TStringGrid;
    sgResult: TStringGrid;
    Label2: TLabel;
    BindDBGridLink1: TBindDBGridLink;
    bsResult: TBindScopeDB;
    Panel2: TPanel;
    btnExecute: TButton;
    dsResult: TDataSource;
    Panel3: TPanel;
    Memo2: TMemo;
    BindDBGridLink2: TBindDBGridLink;
    kbmCSVStreamFormat1: TkbmCSVStreamFormat;
    btnSaveResult: TButton;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure btnExecuteClick(Sender: TObject);
    procedure btnSaveResultClick(Sender: TObject);
  private
    { Private declarations }
    FSQL:TkbmMemSQL;
  public
    { Public declarations }
    procedure BuildSampleData(ATable:TkbmMemTable);
  end;

var
  Form1: TForm1;

implementation

{$R *.fmx}

uses
   kbmSQLElements;

procedure TForm1.FormCreate(Sender: TObject);
begin
     FSQL:=TkbmMemSQL.Create(nil);
end;

procedure TForm1.FormDestroy(Sender: TObject);
begin
     FSQL.Free;
end;

procedure TForm1.btnExecuteClick(Sender: TObject);
var
   i:integer;
   fld:TkbmSQLField;
   tbl:TkbmSQLTable;
begin
     dsResult.Dataset:=nil; // Disconnect from dataset or alternatively disablecontrols on kbmMemSQL to make FireMonkey perform.

     BuildSampleData(mtTable1);

     // Add tables that the SQL is supposed to access.
     FSQL.Tables.Clear;
     FSQL.Tables.Add('table1',mtTable1);

     // Execute the SQL.
     try
        FSQL.ExecSQL(mSQL.Text);

        // Show the result.
        dsResult.DataSet:=FSQL;
     finally
        // Dump our parse tree as we know it.
//        with Memo2.Lines do
//        begin
//             Clear;
//             Add('Operation');
//             Add('----------');
//             Add('');
//             case FSQL.Parser.Operation.OpType of
//                  sotSELECT: Add('SELECT');
//                  sotDELETE: Add('DELETE');
//                  sotINSERT: Add('INSERT');
//                  sotUPDATE: Add('UPDATE');
//             else
//                  Add('Unknown');
//             end;
//
//             for i:=0 to FSQL.Parser.Operation.RefTables.Count-1 do
//             begin
//                  Add('Table '+Inttostr(i));
//                  tbl:=FSQL.Parser.Operation.RefTables[i];
//                  Add('  '+tbl.Name+' as '+tbl.Alias);
//             end;
//
//             for i:=0 to FSQL.Parser.Operation.RefFields.Count-1 do
//             begin
//                  Add('Included field '+Inttostr(i));
//                  fld:=FSQL.Parser.Operation.RefFields[i];
//                  Add('  '+fld.Name+' as '+fld.Alias);
//             end;
//
//             for i:=0 to FSQL.Parser.Operation.RefSearchFields.Count-1 do
//             begin
//                  Add('Searched field '+Inttostr(i));
//                  fld:=FSQL.Parser.Operation.RefSearchFields[i];
//                  Add('  '+fld.Name+' as '+fld.Alias);
//             end;
//        end;
     end;
end;

procedure TForm1.btnSaveResultClick(Sender: TObject);
begin
     FSQL.SaveToFileViaFormat('resultCSV',kbmCSVStreamFormat1);
end;

procedure TForm1.BuildSampleData(ATable:TkbmMemTable);
var
   i:integer;
begin
     ATable.Close;
     ATable.CreateTable;
     ATable.DisableControls;
     try
        ATable.Open;
        for i:=0 to 1000 do
        begin
             ATable.Append;
             ATable.FieldByName('fld1').AsString:='STR'+inttostr(i div 3);
             ATable.FieldByName('fld2').AsInteger:=i;
             ATable.FieldByName('fld3').AsInteger:=999-i;
             ATable.FieldByName('fld4').AsString:='STR'+inttostr(999-i);
             ATable.FieldByName('fld5').AsInteger:=Random(2)+5;;
             ATable.Post;
        end;
     finally
        ATable.EnableControls;
     end;
end;

end.
