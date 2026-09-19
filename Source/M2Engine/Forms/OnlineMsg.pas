unit OnlineMsg;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, Grids, ComCtrls, SpinEditEx,
  ExtCtrls, M2Definition, Vcl.Samples.Spin;

type
  TfrmOnlineMsg = class(TForm)
    btnSend: TButton;
    pgc1: TPageControl;
    ts1: TTabSheet;
    ts2: TTabSheet;
    StringGrid: TStringGrid;
    ButtonAdd: TButton;
    ButtonDelete: TButton;
    MemoMsg: TMemo;
    lbl1: TLabel;
    grp1: TGroupBox;
    chkDisableTrading: TCheckBox;
    chkDisableRepair: TCheckBox;
    chkDisableSaveToStorage: TCheckBox;
    chkDisableGetFromStorage: TCheckBox;
    chkDisableBuy: TCheckBox;
    chkDisableSell: TCheckBox;
    chkDisableDropItem: TCheckBox;
    chkDisableUseNpc: TCheckBox;
    grp2: TGroupBox;
    chkAutoRun: TCheckBox;
    lbl2: TLabel;
    seAutRunInterval: TSpinEditEx;
    Label2: TLabel;
    Label1: TLabel;
    ComboBoxMsg: TComboBox;
    lbl3: TLabel;
    tmrRun: TTimer;
    lbl4: TLabel;
    chkDisableChallenge: TCheckBox;
    chkDisableShop: TCheckBox;
    procedure ComboBoxMsgKeyPress(Sender: TObject; var Key: Char);
    procedure ComboBoxMsgChange(Sender: TObject);
    procedure StringGridClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure ButtonAddClick(Sender: TObject);
    procedure StringGridDblClick(Sender: TObject);
    procedure ButtonDeleteClick(Sender: TObject);
    procedure MemoMsgChange(Sender: TObject);
    procedure btnSendClick(Sender: TObject);
    procedure tmrRunTimer(Sender: TObject);
  private
    StrList: TStringList;
    StrListFile: string;

    FSendMsg: string;
    { Private declarations }
  public
    { Public declarations }
  end;

procedure ShowFrmOnlineMsg;

implementation

uses
  UsrEngn, M2Share;

{$R *.dfm}

procedure ShowFrmOnlineMsg;
var
  frmOnlineMsg: TfrmOnlineMsg;
begin
  frmOnlineMsg := TfrmOnlineMsg.Create(nil);
  try
    frmOnlineMsg.tmrRun.Enabled := False;
    frmOnlineMsg.ShowModal;
  finally
    g_OnlineMsgControl.boDisableTrading := False;
    g_OnlineMsgControl.boDisableRepair := False;
    g_OnlineMsgControl.boDisableBuy := False;
    g_OnlineMsgControl.boDisableSell := False;
    g_OnlineMsgControl.boDisableSaveToStorage := False;
    g_OnlineMsgControl.boDisableGetFromStorage := False;
    g_OnlineMsgControl.boDisableDropItem := False;
    g_OnlineMsgControl.boDisableUseNpc := False;
    g_OnlineMsgControl.boDisableChallenge := False;
    frmOnlineMsg.Free;
  end;
end;

procedure TfrmOnlineMsg.ComboBoxMsgKeyPress(Sender: TObject; var Key: Char);
var
  Msg: string;
begin
  try
    case Ord(Key) of
      13:
        begin
          Msg := ComboBoxMsg.Text;
          if Trim(Msg) <> '' then
          begin
            if ComboBoxMsg.Items.Count = 0 then
              ComboBoxMsg.Items.Add(Msg);
            ComboBoxMsg.Items.Insert(1, Msg);
            UserEngine.SendBroadCastMsgExt(Msg, t_System);
            MemoMsg.Lines.Add(g_Config.sSysMsgPreFix + Msg);
          end;
          ComboBoxMsg.ItemIndex := 0;
          ComboBoxMsg.Text := '';
          ButtonAdd.Enabled := False;
        end;
    end;
  finally
  end;
end;

procedure TfrmOnlineMsg.ComboBoxMsgChange(Sender: TObject);
begin
  try
    if ComboBoxMsg.Items.Count > 20 then
      ComboBoxMsg.Items.Delete(19);
    if Trim(ComboBoxMsg.Text) <> '' then
      ButtonAdd.Enabled := True
    else
      ButtonAdd.Enabled := False;
  finally

  end;
end;

procedure TfrmOnlineMsg.StringGridClick(Sender: TObject);
begin
  try
    if StringGrid.Col >= 0 then
      ButtonDelete.Enabled := True;
  finally
  end;
end;

procedure TfrmOnlineMsg.FormCreate(Sender: TObject);
begin
  StrListFile := '.\MsgList.txt';
  StrList := TStringList.Create;
  if FileExists(StrListFile) then
  begin
    StrList.LoadFromFile(StrListFile);
    StringGrid.RowCount := StrList.Count;
    StringGrid.Cols[0] := StrList;
  end
  else
  begin
    StrList.SaveToFile(StrListFile);
  end;
  MemoMsg.Clear;

  chkDisableTrading.Checked := g_OnlineMsgControl.boSaveDisableTrading;
  chkDisableRepair.Checked := g_OnlineMsgControl.boSaveDisableRepair;
  chkDisableBuy.Checked := g_OnlineMsgControl.boSaveDisableBuy;
  chkDisableSell.Checked := g_OnlineMsgControl.boSaveDisableSell;
  chkDisableSaveToStorage.Checked := g_OnlineMsgControl.boSaveDisableSaveToStorage;
  chkDisableGetFromStorage.Checked := g_OnlineMsgControl.boSaveDisableGetFromStorage;
  chkDisableDropItem.Checked := g_OnlineMsgControl.boSaveDisableDropItem;
  chkDisableUseNpc.Checked := g_OnlineMsgControl.boSaveDisableUseNpc;
  chkDisableChallenge.Checked := g_OnlineMsgControl.boSaveDisableChallenge;
  chkDisableShop.Checked := g_OnlineMsgControl.boSaveDisableShop;

  {
    g_OnlineMsgControl.boDisableTrading := g_OnlineMsgControl.boSaveDisableTrading;
    g_OnlineMsgControl.boDisableRepair := g_OnlineMsgControl.boSaveDisableRepair ;
    g_OnlineMsgControl.boDisableBuy := g_OnlineMsgControl.boSaveDisableBuy;
    g_OnlineMsgControl.boDisableSell := g_OnlineMsgControl.boSaveDisableSell;
    g_OnlineMsgControl.boDisableSaveToStorage := g_OnlineMsgControl.boSaveDisableSaveToStorage;
    g_OnlineMsgControl.boDisableGetFromStorage := g_OnlineMsgControl.boSaveDisableGetFromStorage;
    g_OnlineMsgControl.boDisableDropItem := g_OnlineMsgControl.boSaveDisableDropItem;
    g_OnlineMsgControl.boDisableUseNpc := g_OnlineMsgControl.boSaveDisableUseNpc;
    g_OnlineMsgControl.boDisableChallenge := g_OnlineMsgControl.boSaveDisableChallenge;
  }
end;

procedure TfrmOnlineMsg.ButtonAddClick(Sender: TObject);
var
  Msg: string;
begin
  Msg := Trim(ComboBoxMsg.Text);
  if Msg <> '' then
  begin
    StrList.Add(Msg);
  end;
  StringGrid.RowCount := StrList.Count;
  StringGrid.Cols[0] := StrList;
  ButtonAdd.Enabled := False;
  StrList.SaveToFile(StrListFile);
end;

procedure TfrmOnlineMsg.StringGridDblClick(Sender: TObject);
begin
  ComboBoxMsg.Text := StrList.Strings[StringGrid.Row];
  ComboBoxMsg.SetFocus;
end;

procedure TfrmOnlineMsg.ButtonDeleteClick(Sender: TObject);
begin
  if StringGrid.RowCount = 1 then
  begin
    ButtonDelete.Enabled := False;
    Exit;
  end;
  StrList.Delete(StringGrid.Row);
  StringGrid.RowCount := StrList.Count;
  StringGrid.Cols[0] := StrList;
  StrList.SaveToFile(StrListFile);
end;

procedure TfrmOnlineMsg.MemoMsgChange(Sender: TObject);
begin
  if MemoMsg.Lines.Count > 80 then
  begin
    MemoMsg.Lines.Clear;
  end;
end;

procedure TfrmOnlineMsg.btnSendClick(Sender: TObject);
var
  Msg: string;
begin
  g_OnlineMsgControl.boSaveDisableTrading := chkDisableTrading.Checked;
  g_OnlineMsgControl.boSaveDisableRepair := chkDisableRepair.Checked;
  g_OnlineMsgControl.boSaveDisableBuy := chkDisableBuy.Checked;
  g_OnlineMsgControl.boSaveDisableSell := chkDisableSell.Checked;
  g_OnlineMsgControl.boSaveDisableSaveToStorage := chkDisableSaveToStorage.Checked;
  g_OnlineMsgControl.boSaveDisableGetFromStorage := chkDisableGetFromStorage.Checked;
  g_OnlineMsgControl.boSaveDisableDropItem := chkDisableDropItem.Checked;
  g_OnlineMsgControl.boSaveDisableUseNpc := chkDisableUseNpc.Checked;
  g_OnlineMsgControl.boSaveDisableChallenge := chkDisableChallenge.Checked;
  g_OnlineMsgControl.boSaveDisableShop := chkDisableShop.Checked;

  Config.WriteBool('OnlineMsgControl', 'DisableTrading', g_OnlineMsgControl.boSaveDisableTrading);
  Config.WriteBool('OnlineMsgControl', 'DisableRepair', g_OnlineMsgControl.boSaveDisableRepair);
  Config.WriteBool('OnlineMsgControl', 'DisableBuy', g_OnlineMsgControl.boSaveDisableBuy);
  Config.WriteBool('OnlineMsgControl', 'DisableSell', g_OnlineMsgControl.boSaveDisableSell);
  Config.WriteBool('OnlineMsgControl', 'DisableSaveToStorage', g_OnlineMsgControl.boSaveDisableSaveToStorage);
  Config.WriteBool('OnlineMsgControl', 'DisableGetFromStorage', g_OnlineMsgControl.boSaveDisableGetFromStorage);
  Config.WriteBool('OnlineMsgControl', 'DisableDropItem', g_OnlineMsgControl.boSaveDisableDropItem);
  Config.WriteBool('OnlineMsgControl', 'DisableUseNpc', g_OnlineMsgControl.boSaveDisableUseNpc);
  Config.WriteBool('OnlineMsgControl', 'DisableChallenge', g_OnlineMsgControl.boSaveDisableChallenge);
  Config.WriteBool('OnlineMsgControl', 'DisableShop', g_OnlineMsgControl.boSaveDisableShop);

  Msg := ComboBoxMsg.Text;
  if Length(Trim(Msg)) = 0 then
    Exit;

  if ComboBoxMsg.Items.Count = 0 then
    ComboBoxMsg.Items.Add(Msg)
  else if ComboBoxMsg.Items.IndexOf(Msg) < 0 then
    ComboBoxMsg.Items.Insert(1, Msg)
  else
    ComboBoxMsg.ItemIndex := ComboBoxMsg.Items.IndexOf(Msg);

  g_OnlineMsgControl.boDisableTrading := chkDisableTrading.Checked;
  g_OnlineMsgControl.boDisableRepair := chkDisableRepair.Checked;
  g_OnlineMsgControl.boDisableBuy := chkDisableBuy.Checked;
  g_OnlineMsgControl.boDisableSell := chkDisableSell.Checked;
  g_OnlineMsgControl.boDisableSaveToStorage := chkDisableSaveToStorage.Checked;
  g_OnlineMsgControl.boDisableGetFromStorage := chkDisableGetFromStorage.Checked;
  g_OnlineMsgControl.boDisableDropItem := chkDisableDropItem.Checked;
  g_OnlineMsgControl.boDisableUseNpc := chkDisableUseNpc.Checked;
  g_OnlineMsgControl.boDisableChallenge := chkDisableChallenge.Checked;

  FSendMsg := Msg;
  UserEngine.SendBroadCastMsgExt(FSendMsg, t_System);
  MemoMsg.Lines.Add(g_Config.sSysMsgPreFix + FSendMsg);

  if chkAutoRun.Checked and (seAutRunInterval.Value > 0) then
  begin
    tmrRun.Interval := seAutRunInterval.Value * 1000;
    tmrRun.Enabled := True;
  end
  else
  begin
    tmrRun.Enabled := False;
  end;
end;

procedure TfrmOnlineMsg.tmrRunTimer(Sender: TObject);
begin
  UserEngine.SendBroadCastMsgExt(FSendMsg, t_System);
  MemoMsg.Lines.Add(g_Config.sSysMsgPreFix + FSendMsg);
end;

end.
