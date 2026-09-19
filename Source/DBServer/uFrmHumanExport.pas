unit uFrmHumanExport;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, Grobal2, HUtil32, MudUtil, RoleDB;

type

  TFrmHumanExport = class(TForm)
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    seLimitCount: TSpinEdit;
    seMinLevel: TSpinEdit;
    btnHumanExport: TButton;
    GroupBox2: TGroupBox;
    btnMobileNumberExport: TButton;
    rbAllMobile: TRadioButton;
    rbBindMobile: TRadioButton;
    procedure btnHumanExportClick(Sender: TObject);
    procedure btnMobileNumberExportClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmHumanExport;

implementation

uses DBShare;

{$R *.dfm}

procedure ShowFrmHumanExport;
var
  FrmHumanExport: TFrmHumanExport;
begin
  FrmHumanExport := TFrmHumanExport.Create(nil);
  try
    FrmHumanExport.ShowModal;
  finally
    FrmHumanExport.Free;
  end;
end;

procedure TFrmHumanExport.btnHumanExportClick(Sender: TObject);
var
  I: Integer;
  TempRoleList: TSerarchRoleList;
  RoleData: PTSerarchRoleData;

  SaveDialog: TSaveDialog;
  FileName: string;
  SL: TStringList;
begin
  btnHumanExport.Enabled := False;

  SL := TStringList.Create;
  try
    TempRoleList := TSerarchRoleList.Create;
    try
      g_RoleDB.HumanDB.SearchByLevel(seLimitCount.Value, seMinLevel.Value, TempRoleList);
      for I := 0 to TempRoleList.Count - 1 do
      begin
        RoleData := TempRoleList.Items[I];

        SL.Add(RoleData.RoleName + #9 + RoleData.Account);
      end;
    finally
      TempRoleList.Free;
    end;

    SaveDialog := TSaveDialog.Create(nil);
    try
      SaveDialog.Title := '导出离线挂机人物名称';
      SaveDialog.Filter := 'AutoLoadOffline|*.txt';
      SaveDialog.FileName := 'AutoLoadOffline.txt';
      if SaveDialog.Execute then
      begin
        FileName := SaveDialog.FileName;
        if ExtractFileExt(FileName) <> '.txt' then
          FileName := ChangeFileExt(FileName, '.txt');

        try
          SL.SaveToFile(FileName);
          Application.MessageBox(PChar(FileName), '提示信息', MB_ICONQUESTION);
        except
          on e: Exception do
          begin
            Application.MessageBox(PChar(e.Message), '错误信息', MB_ICONERROR);
          end;
        end;
      end;
    finally
      SaveDialog.Free;
    end;

  finally
    SL.Free;
    btnHumanExport.Enabled := True;
  end;
end;

procedure TFrmHumanExport.btnMobileNumberExportClick(Sender: TObject);
var
  //I: Integer;
  SaveDialog: TSaveDialog;
  FileName: string;
  SL: TStringList;
begin
  btnMobileNumberExport.Enabled := False;

  SL := TStringList.Create;
  try
    g_RoleDB.HumanDB.GetMobileNumbers(rbBindMobile.Checked, SL);

    SaveDialog := TSaveDialog.Create(nil);
    try
      SaveDialog.Title := '导出人物手机号码';
      SaveDialog.Filter := '文本文件(*.txt)|*.txt';
      SaveDialog.FileName := '手机号码.txt';
      if SaveDialog.Execute then begin
        FileName := SaveDialog.FileName;
        if ExtractFileExt(FileName) <> '.txt' then begin
          FileName := ChangeFileExt(FileName, '.txt');
        end;

        try
          SL.SaveToFile(FileName);
          Application.MessageBox(PChar(FileName), '提示信息', MB_ICONQUESTION);
        except
          on e: Exception do begin
            Application.MessageBox(PChar(e.Message), '错误信息', MB_ICONERROR);
          end;
        end;
      end;
    finally
      SaveDialog.Free;
    end;

  finally
    SL.Free;
    btnMobileNumberExport.Enabled := True;
  end;
end;

end.
