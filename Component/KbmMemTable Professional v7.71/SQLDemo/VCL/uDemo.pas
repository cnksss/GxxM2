unit uDemo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, DB, Grids, DBGrids, ExtCtrls, ComCtrls,
  kbmMemTable, kbmMemSQL, kbmSQLMemTableAPI, kbmSQLStdFunc,
  kbmSQLElements;

type
  TForm1 = class(TForm)
    mtTable1: TkbmMemTable;
    mtTable1fld1: TStringField;
    mtTable1fld2: TIntegerField;
    mtTable1fld3: TIntegerField;
    mtTable1fld4: TStringField;
    mtTable1fld5: TIntegerField;
    PageControl1: TPageControl;
    tsDescription: TTabSheet;
    tsSQLSamples: TTabSheet;
    Memo1: TMemo;
    Panel1: TPanel;
    Button1: TButton;
    mSQL: TMemo;
    Panel2: TPanel;
    Label1: TLabel;
    dbgTable1: TDBGrid;
    Label2: TLabel;
    DBGrid2: TDBGrid;
    Splitter1: TSplitter;
    Splitter2: TSplitter;
    Memo2: TMemo;
    dsTable1: TDataSource;
    dsResult: TDataSource;
    PageControl2: TPageControl;
    tsData: TTabSheet;
    tsParseTree: TTabSheet;
    tvParse: TTreeView;
    btnRefreshParseTree: TButton;
    Button2: TButton;
    tsLog: TTabSheet;
    mLog: TMemo;
    Button3: TButton;
    PageControl3: TPageControl;
    tsTable1: TTabSheet;
    tsTable2: TTabSheet;
    tsTable3: TTabSheet;
    dbgTable2: TDBGrid;
    dbgTable3: TDBGrid;
    mtTable2: TkbmMemTable;
    mtTable2fld1: TStringField;
    mtTable2fld2: TIntegerField;
    mtTable2fld3: TIntegerField;
    mtTable2fld4: TStringField;
    mtTable2fld5: TIntegerField;
    dsTable2: TDataSource;
    mtTable3: TkbmMemTable;
    mtTable3fld1: TStringField;
    mtTable3fld2: TIntegerField;
    mtTable3fld3: TIntegerField;
    mtTable3fld4: TStringField;
    mtTable3fld5: TIntegerField;
    dsTable3: TDataSource;
    mtTable1fld6: TIntegerField;
    mtTable2fld6: TIntegerField;
    mtTable3fld6: TIntegerField;
    tsDebugDataset: TTabSheet;
    DBGrid1: TDBGrid;
    dsIntermediate: TDataSource;
    tsEvaluationSamples: TTabSheet;
    mEvaluation: TMemo;
    btnEvaluate: TButton;
    btnCalculate: TButton;
    Label3: TLabel;
    eEvalResult: TEdit;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    Label13: TLabel;
    Label14: TLabel;
    mtTable1fld7: TIntegerField;
    mtTable2fld7: TIntegerField;
    mtTable3fld7: TIntegerField;
    procedure Button1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure btnRefreshParseTreeClick(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure mSQLClick(Sender: TObject);
    procedure btnEvaluateClick(Sender: TObject);
    procedure btnCalculateClick(Sender: TObject);
  private
    { Private declarations }
    FSQL:TkbmMemSQL;

    function OnGetVariableValue(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AVariableValue:variant):boolean;
    function OnGetVariableMetaData(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AVariableWidth:integer; var AVariableDataType:TFieldType):boolean;

    procedure ExecuteSQL(const ASQL:string);

    procedure BuildSampleData(ATable:TkbmMemTable);

    procedure AddSubElements(const AParentNode:TTreeNode; AElements:TkbmSQLNodes);
    procedure AddSubElement(const AParentNode:TTreeNode; AElement:TkbmSQLCustomNode);
    procedure BuildParseTree;
    procedure SelectCurrentLine;
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

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
             ATable.FieldByName('fld5').AsInteger:=Random(2)+5;
             ATable.FieldByName('fld6').AsInteger:=Random(100);
             if random(100)>5 then
                ATable.FieldByName('fld7').AsInteger:=ATable.FieldByName('fld6').AsInteger;
             ATable.Post;
        end;
     finally
        ATable.EnableControls;
     end;
end;

procedure TForm1.btnCalculateClick(Sender: TObject);
var
   i:integer;
begin
     i:=mEvaluation.Perform(EM_LINEFROMCHAR,mEvaluation.SelStart,0);
     eEvalResult.Text:=FSQL.Calculate(mEvaluation.Lines[i]);
end;

procedure TForm1.btnEvaluateClick(Sender: TObject);
var
   i:integer;
begin
     i:=mEvaluation.Perform(EM_LINEFROMCHAR,mEvaluation.SelStart,0);
     eEvalResult.Text:=FSQL.Evaluate(mEvaluation.Lines[i]);
end;

procedure TForm1.btnRefreshParseTreeClick(Sender: TObject);
begin
     BuildParseTree;
end;

procedure TForm1.AddSubElements(const AParentNode:TTreeNode; AElements:TkbmSQLNodes);
var
   i:integer;
   n:TkbmSQLCustomNode;
begin
     for i:=0 to AElements.Count-1 do
     begin
          n:=AElements.Node[i];
          AddSubElement(AParentNode,n);
     end;
end;

procedure TForm1.AddSubElement(const AParentNode:TTreeNode; AElement:TkbmSQLCustomNode);
var
   tn,tn1:TTreeNode;
   s:string;
begin
     if AElement is TkbmSQLBinaryNode then
     begin
          case TkbmSQLBinaryNode(AElement).Operator of
               ebAdd: s:='Add';
               ebConcat: s:='Concat';
               ebSub: s:='Sub';
               ebMul: s:='Mul';
               ebDiv: s:='Div';
               ebAnd: s:='And';
               ebOr: s:='Or';
               ebXor: s:='XOr';
               ebMod: s:='Mod';
               ebIDiv: s:='IDiv';
               ebLike: s:='LIKE';
               ebEqual: s:='=';
               ebNotEqual: s:='<>';
               ebLess: s:='<';
               ebGreater: s:='>';
               ebLessEqual: s:='<=';
               ebGreaterEqual: s:='>=';
          else
              s:='Unknown';
          end;
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName+' '+s);
          tn1:=tvParse.Items.AddChild(tn,'Left');
          AddSubElement(tn1,TkbmSQLBinaryNode(AElement).LeftNode);
          tn1:=tvParse.Items.AddChild(tn,'Right');
          AddSubElement(tn1,TkbmSQLBinaryNode(AElement).RightNode);
     end
     else if AElement is TkbmSQLUnaryNode then
     begin
          case TkbmSQLUnaryNode(AElement).Operator of
               euNot: s:='Not';
               euNegate: s:='Negate';
          else
              s:='Unknown';
          end;
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName+' '+s);
          tn1:=tvParse.Items.AddChild(tn,'Right');
          AddSubElement(tn1,TkbmSQLUnaryNode(AElement).RightNode);
     end
     else if AElement is TkbmSQLBetweenNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tn1:=tvParse.Items.AddChild(tn,'Left');
          AddSubElement(tn1,TkbmSQLBetweenNode(AElement).LeftNode);
          tn1:=tvParse.Items.AddChild(tn,'Range low');
          AddSubElement(tn1,TkbmSQLBetweenNode(AElement).RangeLow);
          tn1:=tvParse.Items.AddChild(tn,'Range high');
          AddSubElement(tn1,TkbmSQLBetweenNode(AElement).RangeHigh);
     end
     else if AElement is TkbmSQLInNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tn1:=tvParse.Items.AddChild(tn,'Left');
          AddSubElement(tn1,TkbmSQLInNode(AElement).LeftNode);
          tn1:=tvParse.Items.AddChild(tn,'In');
          AddSubElements(tn1,TkbmSQLInNode(AElement).Nodes);
     end
     else if AElement is TkbmSQLFieldNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tvParse.Items.AddChild(tn,'Type='+TkbmSQLFieldNode(AElement).ClassName);
          tvParse.Items.AddChild(tn,'Name='+TkbmSQLFieldNode(AElement).FieldName);
          tvParse.Items.AddChild(tn,'Alias='+TkbmSQLFieldNode(AElement).Alias);
          tvParse.Items.AddChild(tn,'Description='+TkbmSQLFieldNode(AElement).Description);
     end
     else if AElement is TkbmSQLVariableNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tvParse.Items.AddChild(tn,'Type='+TkbmSQLVariableNode(AElement).ClassName);
          tvParse.Items.AddChild(tn,'VariableName='+TkbmSQLVariableNode(AElement).VariableName);
          tvParse.Items.AddChild(tn,'Alias='+TkbmSQLVariableNode(AElement).Alias);
          tvParse.Items.AddChild(tn,'Description='+TkbmSQLVariableNode(AElement).Description);
     end
     else if AElement is TkbmSQLGroupFieldNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tvParse.Items.AddChild(tn,'Name='+TkbmSQLGroupFieldNode(AElement).FieldName);
          tvParse.Items.AddChild(tn,'Alias='+TkbmSQLGroupFieldNode(AElement).Alias);
          tvParse.Items.AddChild(tn,'Description='+TkbmSQLGroupFieldNode(AElement).Description);
     end
     else if AElement is TkbmSQLAggregateNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          if TkbmSQLAggregateNode(AElement).Distinct then
             s:=' DISTINCT'
          else
              s:='';

          tvParse.Items.AddChild(tn,'Aggregation='+TkbmSQLAggregateNode(AElement).FieldModifier+s);
          tn1:=tvParse.Items.AddChild(tn,'Expression');
          AddSubElement(tn1,TkbmSQLAggregateNode(AElement).ExpressionNode);
     end
     else if AElement is TkbmSQLFunctionNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tvParse.Items.AddChild(tn,'Function='+TkbmSQLFunctionNode(AElement).FunctionName);
          tn1:=tvParse.Items.AddChild(tn,'Arguments');
          AddSubElements(tn1,TkbmSQLFunctionNode(AElement).Args);
     end
     else if AElement is TkbmSQLConstNode then
     begin
          tn:=tvParse.Items.AddChild(AParentNode,AElement.ClassName);
          tvParse.Items.AddChild(tn,'Alias='+TkbmSQLConstNode(AElement).Alias);
          tvParse.Items.AddChild(tn,'Value='+String(TkbmSQLConstNode(AElement).Value));
     end;
end;

procedure TForm1.BuildParseTree;
var
   tn,tn1:TTreeNode;
   op:TkbmSQLCustomOperation;
begin
     tvParse.Items.Clear;

     op:=FSQL.Parser.Operation;
     tn:=tvParse.Items.Add(nil,op.ClassName);
     if op is TkbmSQLSelectOperation then
     begin
          tn1:=tvParse.Items.AddChild(tn,'Field expressions');
          AddSubElements(tn1,(TkbmSQLSelectOperation(op).Selection));
          tn1:=tvParse.Items.AddChild(tn,'Condition');
          AddSubElement(tn1,(TkbmSQLSelectOperation(op).Condition));
          tn1:=tvParse.Items.AddChild(tn,'Group expression');
          AddSubElements(tn1,(TkbmSQLSelectOperation(op).Group));
          if TkbmSQLSelectOperation(op).HavingCondition<>nil then
          begin
               tn1:=tvParse.Items.AddChild(tn,'Having expression');
               AddSubElement(tn1,(TkbmSQLSelectOperation(op).HavingCondition));
          end;
          tn1:=tvParse.Items.AddChild(tn,'Order expression');
          AddSubElements(tn1,(TkbmSQLSelectOperation(op).Order));
     end;
     tvParse.FullExpand;
end;

function TForm1.OnGetVariableValue(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AVariableValue:variant):boolean;
var
   s:string;
begin
     s:=UpperCase(AVariableName);
     if s='VAR1' then
     begin
          AVariableValue:='The red fox';
          Result:=true;
          exit;
     end;
     if s='VAR2' then
     begin
          AVariableValue:=1234.567;
          Result:=true;
          exit;
     end;
     Result:=false;
end;

function TForm1.OnGetVariableMetaData(const ANode:TkbmSQLCustomNode; const AVariableName:string; var AVariableWidth:integer; var AVariableDataType:TFieldType):boolean;
var
   s:string;
begin
     s:=UpperCase(AVariableName);
     if s='VAR1' then
     begin
          AVariableWidth:=11;
          AVariableDataType:=ftString;
          Result:=true;
          exit;
     end;
     if s='VAR2' then
     begin
          AVariableWidth:=10;
          AVariableDataType:=ftFloat;
          Result:=true;
          exit;
     end;
     Result:=false;
end;

procedure TForm1.ExecuteSQL(const ASQL:string);
var
   i:integer;
   fld:TkbmSQLFieldNode;
   tbl:TkbmSQLTable;
begin
     mLog.Lines.Add('Executing: '+ASQL);

     BuildSampleData(mtTable1);
     BuildSampleData(mtTable2);
     BuildSampleData(mtTable3);

     // Add tables that the SQL is supposed to access.
     FSQL.Tables.Clear;
     FSQL.Tables.Add('table1',mtTable1);
     FSQL.Tables.Add('table2',mtTable2);
     FSQL.Tables.Add('table3',mtTable3);

     // Execute the SQL.
     try
        FSQL.ExecSQL(ASQL);
        dsResult.DataSet:=FSQL;
     finally
        // Dump our parse tree as we know it.
        with Memo2.Lines do
        begin
             Clear;
             Add('Operation');
             Add('----------');
             Add('');
             if FSQL.Parser.Operation<>nil then
             begin

                  case FSQL.Parser.Operation.OpType of
                       sotSELECT: Add('SELECT');
                       sotDELETE: Add('DELETE');
                       sotINSERT: Add('INSERT');
                       sotUPDATE: Add('UPDATE');
                  else
                       Add('Unknown');
                  end;

                  for i:=0 to FSQL.Parser.Operation.RefSourceTables.Count-1 do
                  begin
                       Add('Table '+Inttostr(i));
                       tbl:=FSQL.Parser.Operation.RefSourceTables[i];
                       Add('  '+tbl.Name+' as '+tbl.Alias);
                  end;

                  for i:=0 to FSQL.Parser.Operation.RefSourceFields.Count-1 do
                  begin
                       Add('Included field '+Inttostr(i));
                       fld:=FSQL.Parser.Operation.RefSourceFields.FieldNodes[i];
                       Add('  '+fld.FieldName+' as '+fld.Alias);
                  end;

                  for i:=0 to FSQL.Parser.Operation.RefSearchFields.Count-1 do
                  begin
                       Add('Searched field '+Inttostr(i));
                       fld:=FSQL.Parser.Operation.RefSearchFields.FieldNodes[i];
                       Add('  '+fld.FieldName+' as '+fld.Alias);
                  end;
             end
             else
                 Add('NOT DETERMINED');
        end;
     end;
end;



procedure TForm1.Button1Click(Sender: TObject);
begin
     ExecuteSQL(mSQL.Lines[0]);
end;

procedure TForm1.Button2Click(Sender: TObject);
var
   i:integer;
begin
     for i:=0 to mSQL.Lines.Count-1 do
         ExecuteSQL(mSQL.Lines[i]);
end;

procedure TForm1.Button3Click(Sender: TObject);
var
   i:integer;
begin
     i:=mSQL.Perform(EM_LINEFROMCHAR,mSQL.SelStart,0);
     ExecuteSQL(mSQL.Lines[i]);
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
     ReportMemoryLeaksOnShutdown:=true;
     FSQL:=TkbmMemSQL.Create(nil);
     FSQL.OnGetVariableValue:=OnGetVariableValue;

     // This event is only necessary to set if the variable is going to be part of a field result.
     FSQL.OnGetVariableMetaData:=OnGetVariableMetaData;
end;

procedure TForm1.FormDestroy(Sender: TObject);
begin
     FSQL.Free;
end;

procedure TForm1.SelectCurrentLine;
var
   i:integer;
begin
     with mSQL do
     begin
          i:=Perform(EM_LINEFROMCHAR,SelStart,0);
          SelStart:=Perform(EM_LINEINDEX,i,0);
          SelLength:=Length(Lines[i]);
   end;
end;

procedure TForm1.mSQLClick(Sender: TObject);
begin
     SelectCurrentLine;
end;

end.
