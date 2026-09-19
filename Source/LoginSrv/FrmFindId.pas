unit FrmFindId;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics,
  Controls, Forms, Dialogs, StdCtrls, ExtCtrls, Grids, AccountDB, Grobal2;
type
  TFrmFindUserId = class(TForm)
    IdGrid: TStringGrid;
    Panel1: TPanel;
    edtFindAccount: TEdit;
    Label1: TLabel;
    btnFindAccount: TButton;
    Button1: TButton;
    BtnEdit: TButton;
    Button2: TButton;

    procedure FormCreate(Sender: TObject);
    procedure btnFindAccountClick(Sender: TObject);
    procedure Button1Click(Sender: TObject);
    procedure BtnEditClick(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure edtFindAccountKeyPress(Sender: TObject; var Key: Char);
  private
    procedure RefChrGrid(nIndex: Integer; AccountInfo: pTAccountInfo);
    { Private declarations }
  public
    { Public declarations }
  end;

var
  FrmFindUserId: TFrmFindUserId;

implementation

uses EditUserInfo, LMain, MasSock, LSShare;



{$R *.DFM}
//00467AB4

procedure TFrmFindUserId.edtFindAccountKeyPress(Sender: TObject; var Key: Char);
var
  sAccount: string;
  //n08, nIndex: Integer;
  AccountInfo: TAccountInfo;
begin
  if Key <> #13 then Exit;
  sAccount := Trim(edtFindAccount.Text);
  IdGrid.RowCount := 1;

  if g_AccountDB.GetAccount(sAccount, AccountInfo) then
  begin
    RefChrGrid(-1, @AccountInfo);
  end;
end;

procedure TFrmFindUserId.FormCreate(Sender: TObject);
begin
  IdGrid.RowCount := 2;
  IdGrid.Cells[0, 0] := '帐号';
  IdGrid.Cells[1, 0] := '密码';
  IdGrid.Cells[2, 0] := '用户名称';
  IdGrid.Cells[3, 0] := '身份证号';
  IdGrid.Cells[4, 0] := '生日';
  IdGrid.Cells[5, 0] := '问题一';
  IdGrid.Cells[6, 0] := '答案一';
  IdGrid.Cells[7, 0] := '问题二';
  IdGrid.Cells[8, 0] := '答案二';
  IdGrid.Cells[9, 0] := '电话';
  IdGrid.Cells[10, 0] := '移动电话';
  IdGrid.Cells[11, 0] := '备注信息';
  //IdGrid.Cells[12, 0] := '备注二';
  IdGrid.Cells[12, 0] := '创建时间';
  IdGrid.Cells[13, 0] := '最后登录时间';
  IdGrid.Cells[14, 0] := '电子邮箱';
end;

procedure TFrmFindUserId.btnFindAccountClick(Sender: TObject);
var
  sAccount: string;
  I: Integer;
  AccountInfo: pTAccountInfo;
  AccountList: TAccountList;
begin
  sAccount := Trim(edtFindAccount.Text);
  if sAccount = '' then exit;

  try
    IdGrid.RowCount := 1;
    AccountList := TAccountList.Create;
    try
      if g_AccountDB.FindAccount(sAccount, AccountList) > 0 then
      begin
        for I := 0 to AccountList.Count - 1 do
        begin
          AccountInfo := AccountList.Items[I];
          RefChrGrid(-1, AccountInfo);
        end;
      end;
    finally
      AccountList.Free;
    end;
  except
    MainOutMessage('TFrmFindUserId.BtnFindAllClick');
  end;
end;

procedure TFrmFindUserId.Button1Click(Sender: TObject);
begin
  FrmMasSoc.LoadServerAddr();
end;

procedure TFrmFindUserId.BtnEditClick(Sender: TObject);
var
  nRow: Integer;
  sAccount: string;
  AccountInfo: TAccountInfo;
resourcestring
  sEditAccount = 'ch2';
begin
  nRow := IdGrid.Row;
  if nRow <= 0 then Exit;
  sAccount := IdGrid.Cells[0, nRow];
  if sAccount = '' then Exit;

  if g_AccountDB.GetAccount(sAccount, AccountInfo) then
  begin
    if FrmUserInfoEdit.InputAccountInfo(False, AccountInfo) then
    begin
      if g_AccountDB.UpdateAccount(AccountInfo, ufAllField) then
      begin
        WriteLogMsg(sEditAccount, AccountInfo);
      end;
    end;
  end;
end;

procedure TFrmFindUserId.Button2Click(Sender: TObject);
var
  AccountInfo: TAccountInfo;
  sAccount: string;
resourcestring
  sAddAccount = 'ch2';
  sMakingIDSuccess = '创建帐号成功: %s';
begin
  FillChar(AccountInfo, SizeOf(AccountInfo), #0);
  if FrmUserInfoEdit.InputAccountInfo(True, AccountInfo) and (Length(AccountInfo.AccountName) >= MIN_ACCOUNT_LEN) then
  begin
    if not g_AccountDB.CheckAccountExists(AccountInfo.AccountName) then
    begin
      sAccount := AccountInfo.AccountName;
      if g_AccountDB.AddAccount(AccountInfo) then
      begin
        MainOutMessage(format(sMakingIDSuccess, [sAccount]));
        WriteLogMsg(sAddAccount, AccountInfo);
      end;
    end
    else
    begin
      ShowMessage('帐号名重复');
    end;
  end;
end;

procedure TFrmFindUserId.RefChrGrid(nIndex: Integer; AccountInfo: pTAccountInfo);
var
  nRow: integer;
begin
  try
    if nIndex <= 0 then
    begin
      IdGrid.RowCount := IdGrid.RowCount + 1;
      IdGrid.FixedRows := 1;
      nRow := IdGrid.RowCount - 1;
    end
    else
    begin
      nRow := nIndex;
    end;
    IdGrid.Cells[0, nRow] := AccountInfo.AccountName;
    IdGrid.Cells[1, nRow] := AccountInfo.Password;
    IdGrid.Cells[2, nRow] := AccountInfo.UserName;
    IdGrid.Cells[3, nRow] := AccountInfo.IDCard;
    IdGrid.Cells[4, nRow] := AccountInfo.BirthDay;
    IdGrid.Cells[5, nRow] := AccountInfo.Questions1;
    IdGrid.Cells[6, nRow] := AccountInfo.Answers1;
    IdGrid.Cells[7, nRow] := AccountInfo.Questions2;
    IdGrid.Cells[8, nRow] := AccountInfo.Answers2;
    IdGrid.Cells[9, nRow] := AccountInfo.Phone;
    IdGrid.Cells[10, nRow] := AccountInfo.MobilePhone;
    IdGrid.Cells[11, nRow] := AccountInfo.Memo;
    //IdGrid.Cells[12, nRow] := DBRecord.UserEntryAdd.sMemo2;
    IdGrid.Cells[12, nRow] := IntToStr(AccountInfo.CreateDate);
    IdGrid.Cells[13, nRow] := IntToStr(AccountInfo.LoginDate);
    IdGrid.Cells[14, nRow] := AccountInfo.Mail;
  except

  end;
end;

end.
