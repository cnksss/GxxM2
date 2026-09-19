unit GBDEtoSqlite;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Mask, RzEdit, RzBtnEdt, ParadoxDataSet, ParadoxConv,
  ASGSQLite3, DB, FileCtrl, ADODB;

type
  TFrmBDEToSqlite = class(TForm)
    lbl1: TLabel;
    lbl2: TLabel;
    lbl3: TLabel;
    btnSrc: TRzButtonEdit;
    btnDest: TRzButtonEdit;
    FBtnbtn1: TButton;
    CheckBox1: TCheckBox;
    procedure FBtnbtn1Click(Sender: TObject);
    procedure btnSrcButtonClick(Sender: TObject);
    procedure btnDestButtonClick(Sender: TObject);
  private
    { Private declarations }
    procedure DateSetToSqlite(TableName: string; DataSet: TDataSet; SqliteDB: TASQLite3DB);
    function GetFieldNameInGomArray(FieldName, TableName: string): string;
  public
    procedure Open(FileDir: string);
    { Public declarations }
  end;
const
  STD_FIELDCOUNT = 33;
  MAG_FIELDCOUNT = 1;
  MON_FIELDCOUNT = 1;
  
  GOM_FIELDNAME_STD: array[0..STD_FIELDCOUNT] of string = ('Expand1', 'Expand2', 'Expand3', 'Expand4', 'Expand5', 'Value1', 'Value2', 'Value3', 'Value4', 'Value5', 'Value6',
   'Value7', 'Value8', 'Value9', 'Value10', 'Value11', 'Value12', 'Value13', 'Value14', 'Value15',
    'Value16', 'Value17', 'Value18', 'Value19', 'Value20', 'Value21', 'Value22', 'Value23', 'Value24', 'Value25', 'Job', 'Horse',
     'InsuranceCurrency', 'InsuranceGold');

  GEE_FIELDNAME_STD: array[0..STD_FIELDCOUNT] of string = ('Expand1', 'Expand2', 'Expand3', 'Expand4', 'Expand5', 'element', 'element1', 'element2', 'element3', 'element4', 'element5', 'element6',
   'element7', 'element8', 'element9', 'element10', 'element11', 'element12', 'element13', 'element14', 'element15',
    'element16', 'element17', 'element18', 'element19', 'element20', 'element21', 'element22', 'element23', 'element24', 'Light', 'Horse',
     'InsuranceCurrency', 'InsuranceGold');
                                            
  GOM_FIELDNAME_MAG: array[0..MAG_FIELDCOUNT] of string = ('CanUpgrade', 'MaxUpgradeLv');
  GEE_FIELDNAME_MAG: array[0..MAG_FIELDCOUNT] of string = ('CanUpgrade', 'MaxUpgradeLv');

  GOM_FIELDNAME_MON: array[0..MON_FIELDCOUNT] of string = ('ExploreItem', 'DisableSimpleActor');
  GEE_FIELDNAME_MON: array[0..MON_FIELDCOUNT] of string = ('ExploreItem', 'DisableSimpleActor');

var
  FrmBDEToSqlite: TFrmBDEToSqlite;
  StdCheckFieldName: array[0..STD_FIELDCOUNT] of Boolean;
  MagCheckFieldName: array[0..MAG_FIELDCOUNT] of Boolean;
  MonCheckFieldName: array[0..MON_FIELDCOUNT] of Boolean;
  
  boACC: Boolean = False;
implementation

{$R *.dfm}

{ TFrmBDEToSqlite }
procedure TFrmBDEToSqlite.btnDestButtonClick(Sender: TObject);
var
  SaveDlg: TSaveDialog;
begin
  SaveDlg := TSaveDialog.Create(nil);
  try
    SaveDlg.FileName := btnDest.Text;

    SaveDlg.Filter := 'Sqlite数据库文件(*.db)|*.db';
    SaveDlg.Title := '保存sqlite数据库文件';
    if SaveDlg.Execute then
    begin
      btnDest.Text := SaveDlg.FileName;
    end;
  finally
    SaveDlg.Free;
  end;
end;

procedure TFrmBDEToSqlite.btnSrcButtonClick(Sender: TObject);
var
  Dir: string;
begin
  Dir := btnSrc.Text;
  if SelectDirectory('请选择BDE数据库目录', '', Dir) then
  begin
    btnSrc.Text := Dir;
  end;
end;

procedure TFrmBDEToSqlite.DateSetToSqlite(TableName: string; DataSet: TDataSet;
  SqliteDB: TASQLite3DB);
var
  I: Integer;
  Field: TField;
  S1, S2, S3, sFieldName: string;
  sm: TSQLStatement;
  FloatField: TFloatField;
  I64: Int64;
  nIdx: Integer;
  nSetExpand1: Integer;
begin
  if DataSet.FieldCount = 0 then Exit;
  S1 := '';
  S2 := '';
  S3 := '';
  nSetExpand1 := 0;
  nIdx := 0;
  for I := 0 to DataSet.FieldCount - 1 do //首先建表
  begin
    Field := DataSet.Fields[I];
    sFieldName := GetFieldNameInGomArray(Field.FieldName, TableName);
    if (Field.DataType = ftString) or (Field is TWideStringField) then
    begin
      S1 := S1 + Format('"%s" TEXT(%d),', [sFieldName, TStringField(Field).Size]);
      S2 := S2 + sFieldName + ',';
      S3 := S3 + '?' + ',';
    end
    else if Field is TIntegerField then
    begin
      S1 := S1 + Format('"%s" INTEGER,', [sFieldName]);
      S2 := S2 + sFieldName + ',';
      S3 := S3 + '?' + ',';
    end
    else if Field is TFloatField then
    begin
      S1 := S1 + Format('"%s" INTEGER,', [sFieldName]);
      S2 := S2 + sFieldName + ',';
      S3 := S3 + '?' + ',';
    end
    else
    begin
      raise Exception.Create('不识别的字段类型(' + Field.ClassName + '): ' + Field.FieldName);
    end;
  end;
  //创建表=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
  S1 := Copy(S1, 1, Length(S1) - 1);
  S1 := 'CREATE TABLE "' + TableName + '" ( ' + S1 + ');';

  SqliteDB.Exec('DROP TABLE IF EXISTS "' + TableName + '";'  + sLineBreak + S1);

  S2 := Copy(S2, 1, Length(S2) - 1);
  S3 := Copy(S3, 1, Length(S3) - 1);

  //处理记录开始=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
  sm := SqliteDB.Statements.AddSQLStatement('insert_record');
  try
    sm.Sql :=
      'insert into ' + TableName + '(' + S2 + ') values(' + S3 + ');';
    sm.Prepare;

    DataSet.First;
    while not DataSet.Eof do //遍历每条记录赋值到SQLite里
    begin
      sm.Reset;
      for I := 0 to DataSet.FieldCount - 1 do
      begin
        Field := DataSet.Fields[I];
        if (Field.DataType = ftString) or (Field is TWideStringField) then
        begin
          sm.OrderBindText(Field.AsString);
        end
        else
        if Field is TIntegerField then
        begin
          sm.OrderBindInt(Field.AsInteger);
        end
        else if Field is TFloatField then
        begin
          I64 := Round(Field.AsFloat);
          sm.OrderBindInt64(I64);
        end;
      end;
      sm.Step;

      DataSet.Next;
    end;
  finally
    SqliteDB.Statements.Clear;
  end;
end;

procedure TFrmBDEToSqlite.FBtnbtn1Click(Sender: TObject);
var
  FileDir, FileName: string;
  DataSetSrc: TParadoxDataSet;
  Sqlite3DB: TASQLite3DB;
  ADODataSet: TADODataSet;
  ADOConnection: TADOConnection;
begin
  if Length(btnSrc.Text) = 0 then
  begin
    ShowMessage('源数据库目录不能为空');
    btnSrc.SetFocus;
    Exit;
  end;

  if not DirectoryExists(btnSrc.Text) then
  begin
    ShowMessage('源数据库目录不存在');
    btnSrc.SetFocus;
    Exit;
  end;

  if Length(btnDest.Text) = 0 then
  begin
    ShowMessage('目标数据库不能为空');
    btnDest.SetFocus;
    Exit;
  end;

  if FileExists(btnSrc.Text + '\HeroDB.MDB') then
  begin
    boACC := True;
  end;

  FillChar(StdCheckFieldName, SizeOf(StdCheckFieldName), False); 
  FillChar(MagCheckFieldName, SizeOf(MagCheckFieldName), False);
  FillChar(MonCheckFieldName, SizeOf(MonCheckFieldName), False);

  FileDir := IncludeTrailingPathDelimiter(btnSrc.Text);
  CheckBox1.Enabled := False;
  FBtnbtn1.Enabled := False;
  Sqlite3DB := TASQLite3DB.Create(nil);
  try
    Sqlite3DB.Database := btnDest.Text;
    Sqlite3DB.MustExist := False;
    Sqlite3DB.CharacterEncoding := 'utf8';
    Sqlite3DB.Connected := True;

    Sqlite3DB.Exec('PRAGMA synchronous = OFF;');
    if boACC then
    begin
      ADOConnection := TADOConnection.Create(nil);
      ADOConnection.ConnectionString := 'Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{D:\test.MDB} + btnSrc.Text + '\HeroDB.MDB' + ';Persist Security Info=False';
      ADOConnection.Connected := True;
      ADODataSet := TADODataSet.Create(nil);
      ADODataSet.Connection := ADOConnection;
      try
        with ADODataSet do
        begin
          Close;
          CommandType := cmdText;
          CommandText := 'SELECT * FROM StdItems';
          Open;
        end;
        DateSetToSqlite('StdItems', ADODataSet, Sqlite3DB);
        with ADODataSet do
        begin
          Close;
          CommandType := cmdText;
          CommandText := 'SELECT * FROM Magic';
          Open;
        end;
        DateSetToSqlite('Magic', ADODataSet, Sqlite3DB);
        with ADODataSet do
        begin
          Close;
          CommandType := cmdText;
          CommandText := 'SELECT * FROM Monster';
          Open;
        end;
        DateSetToSqlite('Monster', ADODataSet, Sqlite3DB);
      finally
        ADOConnection.Free;
        ADODataSet.Free;
      end;
    end
    else
    begin
      DataSetSrc := TParadoxDataSet.Create(nil);
      try
        DataSetSrc.TableName := FileDir + 'StdItems.DB';
        DataSetSrc.Open;
        DateSetToSqlite('StdItems', DataSetSrc, Sqlite3DB);

        DataSetSrc.Close;
        DataSetSrc.TableName := FileDir + 'Magic.DB';
        DataSetSrc.Open;
        DateSetToSqlite('Magic', DataSetSrc, Sqlite3DB);

        DataSetSrc.Close;
        DataSetSrc.TableName := FileDir + 'Monster.DB';
        DataSetSrc.Open;
        DateSetToSqlite('Monster', DataSetSrc, Sqlite3DB);
      finally
        DataSetSrc.Free;
      end;
    end;
  finally
    Sqlite3DB.Free;    
    CheckBox1.Enabled := True;
    FBtnbtn1.Enabled := True;
  end;
  ModalResult := mrOk;
  ShowMessage('数据库转换完成');
end;

function TFrmBDEToSqlite.GetFieldNameInGomArray(FieldName,
  TableName: string): string;
var
  I: Integer;
begin
  Result := FieldName;
end;

procedure TFrmBDEToSqlite.Open(FileDir: string);
var
  sDir: string;
begin
  sDir := IncludeTrailingPathDelimiter(FileDir);
  btnSrc.EditText := sDir + 'Mud2\DB\';
  btnDest.EditText := sDir + 'Mud2\DB\BmM2.db';
  ModalResult := mrNone;
  ShowModal;
end;

end.
