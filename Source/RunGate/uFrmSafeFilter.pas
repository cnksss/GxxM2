unit uFrmSafeFilter;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, JSocket, WinSock, Menus, Spin, IniFiles, ComCtrls,
  SpinEditEx, IOCPUtils, IocpTcpServer, MirClientContext;

type
  TFrmSafeFilter = class(TForm)
    grp1: TGroupBox;
    Label4: TLabel;
    lstActive: TListBox;
    GroupBox1: TGroupBox;
    LabelTempList: TLabel;
    Label1: TLabel;
    Label23: TLabel;
    lstTemp: TListBox;
    lstBlock: TListBox;
    lstIpSection: TListBox;
    grp2: TGroupBox;
    Label2: TLabel;
    Label3: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    seMaxConnect: TSpinEdit;
    seKeepConnectTimeOut: TSpinEdit;
    GroupBox3: TGroupBox;
    rbAddBlockList: TRadioButton;
    rbAddTempList: TRadioButton;
    rbDisConnect: TRadioButton;
    GroupBox4: TGroupBox;
    Label6: TLabel;
    seMaxClientPacketSize: TSpinEdit;
    Label7: TLabel;
    btnOK: TButton;
    GroupBox2: TGroupBox;
    Label11: TLabel;
    Label12: TLabel;
    seAttackTick: TSpinEdit;
    seAttackCount: TSpinEdit;
    Label22: TLabel;
    Label24: TLabel;
    seIPCountLimit1: TSpinEditEx;
    seIPCountLimit2: TSpinEditEx;
    seIPCountLimitTime1: TSpinEditEx;
    seIPCountLimitTime2: TSpinEditEx;
    GroupBox6: TGroupBox;
    Label25: TLabel;
    trckbrDefenseLevel: TTrackBar;
    chkDefenseToLevel1: TCheckBox;
    chkAutoClearTemp: TCheckBox;
    seAutoClearTemp: TSpinEditEx;
    chkResotreDefense: TCheckBox;
    seResotreDefense: TSpinEditEx;
    seDefenseToLevel1: TSpinEditEx;
    chkAddAllToTemp: TCheckBox;
    seAddAllToTemp: TSpinEditEx;
    GroupBox5: TGroupBox;
    Label13: TLabel;
    Label14: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    seSayMaxLen: TSpinEdit;
    seSayTime: TSpinEdit;
    seSayMaxCount: TSpinEdit;
    seSayDisableTime: TSpinEdit;
    chkSayMsgControl: TCheckBox;
    pmActive: TPopupMenu;
    mniActiveRefesh: TMenuItem;
    mniActiveSort: TMenuItem;
    N3: TMenuItem;
    mniActiveAddToTemp: TMenuItem;
    mniActiveAddAllToTemp: TMenuItem;
    N2: TMenuItem;
    mniActiveAddToBlock: TMenuItem;
    mniActiveAddAllToBlock: TMenuItem;
    N1: TMenuItem;
    mniActiveKick: TMenuItem;
    pmTemp: TPopupMenu;
    mniTempRefresh: TMenuItem;
    mniTempSort: TMenuItem;
    mniN4: TMenuItem;
    mniTempAdd: TMenuItem;
    mniTempDelete: TMenuItem;
    mniTempClear: TMenuItem;
    mniN5: TMenuItem;
    mniTempAddToBlock: TMenuItem;
    mniTempAddAllToBlock: TMenuItem;
    pmBlock: TPopupMenu;
    mniBlockRefresh: TMenuItem;
    mniBlockSort: TMenuItem;
    mniN6: TMenuItem;
    mniBlockAdd: TMenuItem;
    mniBlockDelete: TMenuItem;
    mniBlockClear: TMenuItem;
    mniN7: TMenuItem;
    mniBlockAddToTemp: TMenuItem;
    mniBlockAddAllToTemp: TMenuItem;
    pmIpSection: TPopupMenu;
    mniIpSectionSort: TMenuItem;
    mniIpSectionAdd: TMenuItem;
    mniIpSectionDel: TMenuItem;
    chkLostLine: TCheckBox;
    grp3: TGroupBox;
    lstTempMac: TListBox;
    lstBlockMac: TListBox;
    lbl1: TLabel;
    Label5: TLabel;
    pmTempMac: TPopupMenu;
    mniTempMacRefresh: TMenuItem;
    mniTempMacSort: TMenuItem;
    MenuItem3: TMenuItem;
    mniTempMacAdd: TMenuItem;
    mniTempMacDelete: TMenuItem;
    mniTempMacClear: TMenuItem;
    MenuItem7: TMenuItem;
    mniTempMacAddToBlock: TMenuItem;
    mniTempMacAddAllToBlock: TMenuItem;
    pmBlockMac: TPopupMenu;
    mniBlockMacRefresh: TMenuItem;
    mniBlockMacSort: TMenuItem;
    MenuItem4: TMenuItem;
    mniBlockMacAdd: TMenuItem;
    mniBlockMacDelete: TMenuItem;
    mniBlockMacClear: TMenuItem;
    MenuItem9: TMenuItem;
    mniBlockMacAddToTempMac: TMenuItem;
    mniBlockMacAddAllToTempMac: TMenuItem;
    grp5: TGroupBox;
    lbl3: TLabel;
    chkOpenCheckClient: TCheckBox;
    cbbCheckClientFailBlockMode: TComboBox;
    mniN8: TMenuItem;
    mniActiveAddAllNoUserToTemp: TMenuItem;
    mniActiveAddAllNoUserToBlock: TMenuItem;
    grp4: TGroupBox;
    chkCheckClientPacketLegal: TCheckBox;
    Label8: TLabel;
    seMaxClientPacketCount: TSpinEdit;
    seCheckClientPacketCount: TSpinEditEx;
    lbl2: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure mniTempSortClick(Sender: TObject);
    procedure mniTempDeleteClick(Sender: TObject);
    procedure mniBlockSortClick(Sender: TObject);
    procedure mniBlockAddToTempClick(Sender: TObject);
    procedure mniBlockDeleteClick(Sender: TObject);
    procedure pmTempPopup(Sender: TObject);
    procedure pmBlockPopup(Sender: TObject);
    procedure mniTempRefreshClick(Sender: TObject);
    procedure mniBlockRefreshClick(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure mniTempAddClick(Sender: TObject);
    procedure mniBlockAddClick(Sender: TObject);
    procedure chkSayMsgControlClick(Sender: TObject);
    procedure trckbrDefenseLevelChange(Sender: TObject);
    procedure mniTempClearClick(Sender: TObject);
    procedure mniTempAddToBlockClick(Sender: TObject);
    procedure mniTempAddAllToBlockClick(Sender: TObject);
    procedure mniBlockClearClick(Sender: TObject);
    procedure mniBlockAddAllToTempClick(Sender: TObject);
    procedure mniActiveRefeshClick(Sender: TObject);
    procedure mniActiveSortClick(Sender: TObject);
    procedure mniActiveAddToTempClick(Sender: TObject);
    procedure mniActiveAddAllToTempClick(Sender: TObject);
    procedure mniActiveAddToBlockClick(Sender: TObject);
    procedure mniActiveKickClick(Sender: TObject);
    procedure pmActiveChange(Sender: TObject; Source: TMenuItem;
      Rebuild: Boolean);
    procedure mniActiveAddAllToBlockClick(Sender: TObject);
    procedure mniIpSectionAddClick(Sender: TObject);
    procedure mniIpSectionDelClick(Sender: TObject);
    procedure mniIpSectionSortClick(Sender: TObject);
    procedure pmIpSectionPopup(Sender: TObject);
    procedure lstActiveKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure mniTempMacRefreshClick(Sender: TObject);
    procedure mniTempMacSortClick(Sender: TObject);
    procedure mniTempMacAddClick(Sender: TObject);
    procedure mniTempMacDeleteClick(Sender: TObject);
    procedure mniTempMacClearClick(Sender: TObject);
    procedure mniTempMacAddToBlockClick(Sender: TObject);
    procedure mniBlockMacRefreshClick(Sender: TObject);
    procedure mniBlockMacSortClick(Sender: TObject);
    procedure mniBlockMacAddClick(Sender: TObject);
    procedure mniBlockMacDeleteClick(Sender: TObject);
    procedure mniBlockMacClearClick(Sender: TObject);
    procedure mniBlockMacAddToTempMacClick(Sender: TObject);
    procedure mniBlockMacAddAllToTempMacClick(Sender: TObject);
    procedure pmTempMacPopup(Sender: TObject);
    procedure pmBlockMacPopup(Sender: TObject);
    procedure mniActiveAddAllNoUserToTempClick(Sender: TObject);
    procedure mniActiveAddAllNoUserToBlockClick(Sender: TObject);
    procedure mniTempMacAddAllToBlockClick(Sender: TObject);
  private
    procedure Open;
    procedure UpdateHints;
    procedure ErrMessage(MsgStr: string);
    function QuestionMessage(const MsgStr: string; MsgTitle: string = ''): Boolean;
    { Private declarations }
  public
    { Public declarations }
  end;

  function ShowFrmSafeFilter(MainForm: TForm): Boolean;

implementation

uses
  GateShare, HUtil32;

{$R *.dfm}

function GetAveCharSize(Canvas: TCanvas): TPoint;
var
  I: Integer;
  Buffer: array[0..51] of Char;
begin
  for I := 0 to 25 do Buffer[I] := Chr(I + Ord('A'));
  for I := 0 to 25 do Buffer[I + 26] := Chr(I + Ord('a'));
  GetTextExtentPoint(Canvas.Handle, Buffer, 52, TSize(Result));
  Result.X := Result.X div 52;
end;

function InputQueryEx(const ACaption, APrompt, AHint: string;
  var Value: string): Boolean;
const
  FORM_WIDTH = 280;
var
  Form: TForm;
  Prompt: TLabel;
  Edit: TEdit;
  DialogUnits: TPoint;
  ButtonTop, ButtonWidth, ButtonHeight: Integer;
begin
  Result := False;
  Form := TForm.Create(Application);
  with Form do
  begin
    try
      Canvas.Font := Font;
      DialogUnits := GetAveCharSize(Canvas);
      BorderStyle := bsDialog;
      Caption := ACaption;
      ClientWidth := MulDiv(FORM_WIDTH, DialogUnits.X, 4);
      Position := poScreenCenter;
      Prompt := TLabel.Create(Form);
      with Prompt do
      begin
        Parent := Form;
        Caption := APrompt;
        Left := MulDiv(8, DialogUnits.X, 4);
        Top := MulDiv(8, DialogUnits.Y, 8);
        Constraints.MaxWidth := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        WordWrap := True;
      end;
      Edit := TEdit.Create(Form);
      with Edit do
      begin
        Parent := Form;
        Left := Prompt.Left;
        Top := Prompt.Top + Prompt.Height + 5;
        Width := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        MaxLength := 255;
        Text := Value;
        SelectAll;
      end;

      ButtonTop := Edit.Top + Edit.Height + 8;
      ButtonWidth := MulDiv(50, DialogUnits.X, 4);
      ButtonHeight := MulDiv(14, DialogUnits.Y, 8);

      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '确定';
        ModalResult := mrOk;
        Default := True;
        Left := Edit.Left + Edit.Width - ButtonWidth * 2 - 6;
        Top := ButtonTop;
        Width := ButtonWidth;
        Height := ButtonHeight;
      end;

      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '取消';
        ModalResult := mrCancel;
        Cancel := True;
        Left := Edit.Left + Edit.Width - ButtonWidth;
        Top := ButtonTop;
        Width := ButtonWidth;
        Height := ButtonHeight;

        Form.ClientHeight := Top + Height + 10;
      end;

      Prompt := TLabel.Create(Form);
      with Prompt do
      begin
        Parent := Form;
        Caption := AHint;
        Font.Color := clBlue;
        Left := Edit.Left;
        Top := ButtonTop + (ButtonHeight - Height) div 2;
        Constraints.MaxWidth := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        WordWrap := True;
      end;

      if ShowModal = mrOk then
      begin
        Value := Edit.Text;
        Result := True;
      end;
    finally
      Form.Free;
    end;
  end;
end;


function ShowFrmSafeFilter(MainForm: TForm): Boolean;
var
  FrmSafeFilter: TFrmSafeFilter;
begin
  FrmSafeFilter := TFrmSafeFilter.Create(nil);
  try
    FrmSafeFilter.Left := MainForm.Left + (MainForm.Width - FrmSafeFilter.Width) div 2;
    FrmSafeFilter.Top := MainForm.Top + (MainForm.Height - FrmSafeFilter.Height) div 2;
    FrmSafeFilter.Open;
    Result := FrmSafeFilter.ShowModal = mrOk;
  finally
    FrmSafeFilter.Free;
  end;
end;

procedure TFrmSafeFilter.Open;
var
  I: Integer;
  sIPaddr: string;
  IPSection: PTIPSection;
  List: TList;
  Context: TMirClientContext;
begin
  lstActive.Clear;
  lstTemp.Clear;
  lstBlock.Clear;
  lstIpSection.Clear;
  lstTempMac.Clear;
  lstBlockMac.Clear;

  g_TempIPList.Lock;
  try
    for I := 0 to g_TempIPList.Count - 1 do
      lstTemp.Items.Add(StrPas(inet_ntoa(TInAddr(g_TempIPList.Items[I].nIPaddr))));
  finally
    g_TempIPList.UnLock;
  end;

  g_BlockIPList.Lock;
  try
    for I := 0 to g_BlockIPList.Count - 1 do
      lstBlock.Items.Add(StrPas(inet_ntoa(TInAddr(g_BlockIPList.Items[I].nIPaddr))));
  finally
    g_BlockIPList.UnLock;
  end;

  List := TList.Create;
  try
    TIOCPClientContextPool.Instance.GetOnlineContextList(List);

    for I := List.Count - 1 downto 0 do
    begin
      Context := TMirClientContext(List.Items[I]);

      {
      sUserName := Format('%-20s', [Context.sChrName]);
      sIPaddr := Context.RemoteAddr;
      if sIPaddr <> '' then
        lstActive.Items.AddObject(sUserName + sIPaddr, TObject(List.Items[I]));
      }

      sIPaddr := Context.RemoteAddr;
      if sIPaddr <> '' then
      begin
        sIPaddr := Format('%-18s', [sIPaddr]);
        lstActive.Items.AddObject(sIPaddr + Context.sChrName, TObject(List.Items[I]));
      end;
    end;
  finally
    List.Free;
  end;

  g_IPSectionList.Lock;
  try
    for I := 0 to g_IPSectionList.Count - 1 do
    begin
      IPSection := g_IPSectionList.Items[I];
      lstIpSection.AddItem(Long2IP(IPSection.nBeginAddr) + ' - ' + Long2IP(IPSection.nEndAddr), TObject(IPSection));
    end;
  finally
    g_IPSectionList.UnLock;
  end;

  g_TempMacList.Lock;
  try
    for I := 0 to g_TempMacList.Count - 1 do
    begin
      lstTempMac.Items.Add(g_TempMacList.Strings[I]);
    end;
  finally
    g_TempMacList.UnLock;
  end;

  g_BlockMacList.Lock;
  try
    for I := 0 to g_BlockMacList.Count - 1 do
    begin
      lstBlockMac.Items.Add(g_BlockMacList.Strings[I]);
    end;
  finally
    g_BlockMacList.UnLock;
  end;

  seMaxConnect.Value := g_nMaxConnOfIPaddr;
  case g_BlockMethod of
    bmDisconnect: rbDisConnect.Checked := True;
    bmTempBlock:  rbAddTempList.Checked := True;
    bmBlockList:  rbAddBlockList.Checked := True;
  end;
  
  seAttackTick.Value := g_dwAttackTick;
  seAttackCount.Value := g_nAttackCount;
  seMaxClientPacketSize.Value := g_nMaxClientPacketSize;
  seMaxClientPacketCount.Value := g_nMaxClientPacketCount;
  chkLostLine.Checked := g_boKickOverPacketSize;
  seKeepConnectTimeOut.Value := g_dwKeepConnectTimeOut;

  chkCheckClientPacketLegal.Checked := g_boCheckClientPacketLegal;
  seCheckClientPacketCount.Value := g_nCheckClientPacketCount;

  {----------------------------- chongchong 2013-09-01 ----------------------}
  chkSayMsgControl.Checked := g_boSayMsgControl;
  chkSayMsgControlClick(chkSayMsgControl);
  seSayMaxLen.Value := g_dwSayMaxLen;
  seSayTime.Value := g_dwSayTime;
  seSayMaxCount.Value := g_dwSayMaxCount;
  seSayDisableTime.Value := g_dwSayDisableTime;

  seIPCountLimitTime1.Value := g_dwIPCountLimitTime1;
  seIPCountLimit1.Value := g_dwIPCountLimit1;
  seIPCountLimitTime2.Value := g_dwIPCountLimitTime2;
  seIPCountLimit2.Value := g_dwIPCountLimit2;

  trckbrDefenseLevel.Position := g_dwDefenseLevel;
  chkDefenseToLevel1.Checked := g_boDefenseToLevel1;
  seDefenseToLevel1.Value := g_dwDefenseToLevel1;
  chkResotreDefense.Checked := g_boResotreDefense;
  seResotreDefense.Value := g_dwResotreDefense;
  chkAutoClearTemp.Checked := g_boAutoClearTemp;
  seAutoClearTemp.Value := g_dwAutoClearTemp;
  chkAddAllToTemp.Checked := g_boAddAllToTemp;
  seAddAllToTemp.Value := g_dwAddAllToTemp;

  chkOpenCheckClient.Checked := g_boOpenCheckClient;
  cbbCheckClientFailBlockMode.ItemIndex := Integer(g_CheckClientFailBlockMethod);
end;


procedure TFrmSafeFilter.FormCreate(Sender: TObject);
begin
  lstTemp.Clear;
  lstBlock.Clear;
end;

procedure TFrmSafeFilter.mniTempSortClick(Sender: TObject);
begin
  lstTemp.Sorted := True;
end;

procedure TFrmSafeFilter.mniTempDeleteClick(Sender: TObject);
begin
  if (lstTemp.ItemIndex >= 0) and (lstTemp.ItemIndex < lstTemp.Items.Count) then
  begin
    g_TempIPList.Lock;
    try
      g_TempIPList.Delete(lstTemp.Items[lstTemp.ItemIndex]);
    finally
      g_TempIPList.UnLock;
    end;
    lstTemp.Items.Delete(lstTemp.ItemIndex);
  end;
end;

procedure TFrmSafeFilter.mniTempClearClick(Sender: TObject);
begin
  g_TempIPList.Lock;
  try
    g_TempIPList.Clear;
  finally
    g_TempIPList.UnLock;
  end;

  lstTemp.Clear;
end;

procedure TFrmSafeFilter.mniTempAddToBlockClick(Sender: TObject);
var
  sIPAddr: string;
begin
  if (lstTemp.ItemIndex >= 0) and (lstTemp.ItemIndex < lstTemp.Items.Count) then
  begin
    sIPAddr := lstTemp.Items[lstTemp.ItemIndex];

    g_TempIPList.Lock;
    try
      g_TempIPList.Delete(sIPAddr);
    finally
      g_TempIPList.UnLock;
    end;

    lstTemp.Items.Delete(lstTemp.ItemIndex);
    lstBlock.Items.Add(sIPAddr);

    AddBlockIP(sIPAddr);

    SaveBlockIPList;
  end;
end;

procedure TFrmSafeFilter.mniTempAddAllToBlockClick(Sender: TObject);
var
  sIPAddr: string;
  I: Integer;
begin
  for I := 0 to lstTemp.Items.Count - 1 do
  begin
    sIPAddr := lstTemp.Items[lstTemp.ItemIndex];
    lstBlock.Items.Add(sIPAddr);

    AddBlockIP(sIPAddr);
  end;

  g_TempIPList.Clear;
  lstTemp.Clear;

  SaveBlockIPList;
end;

procedure TFrmSafeFilter.mniBlockSortClick(Sender: TObject);
begin
  lstBlock.Sorted := True;
end;

procedure TFrmSafeFilter.mniBlockAddToTempClick(Sender: TObject);
var
  sIPaddr: string;
begin
  if (lstBlock.ItemIndex >= 0) and (lstBlock.ItemIndex < lstBlock.Items.Count) then
  begin
    sIPaddr := lstBlock.Items[lstBlock.ItemIndex];
    lstBlock.Items.Delete(lstBlock.ItemIndex);

    g_BlockIPList.Lock;
    try
      g_BlockIPList.Delete(sIPaddr);
    finally
      g_BlockIPList.UnLock;
    end;
    lstTemp.Items.Add(sIPaddr);

    AddTempBlockIP(sIPaddr);
    SaveBlockIPList;
  end;
end;

procedure TFrmSafeFilter.mniBlockDeleteClick(Sender: TObject);
var
  sIPaddr: string;
begin
  if (lstBlock.ItemIndex >= 0) and (lstBlock.ItemIndex < lstBlock.Items.Count) then
  begin
    sIPaddr := lstBlock.Items.Strings[lstBlock.ItemIndex];
    lstBlock.Items.Delete(lstBlock.ItemIndex);

    g_BlockIPList.Lock;
    try
      g_BlockIPList.Delete(sIPaddr);
    finally
      g_BlockIPList.UnLock;
    end;
  end;
end;

procedure TFrmSafeFilter.mniBlockClearClick(Sender: TObject);
begin
  g_BlockIPList.Clear;
  lstBlock.Clear;
end;

procedure TFrmSafeFilter.mniBlockAddAllToTempClick(Sender: TObject);
var
  sIPaddr: string;
  I: Integer;
begin
  for I := 0 to lstBlock.Items.Count - 1 do
  begin
    sIPaddr := lstBlock.Items[I];
    lstTemp.Items.Add(sIPaddr);
    AddTempBlockIP(sIPaddr);
  end;

  g_BlockIPList.Clear;
  lstBlock.Clear;

  SaveBlockIPList;
end;

procedure TFrmSafeFilter.pmTempPopup(Sender: TObject);
var
  boCheck: Boolean;
begin
  mniTempSort.Enabled := lstTemp.Items.Count > 0;
  mniTempClear.Enabled := mniTempSort.Enabled;
  mniTempAddAllToBlock.Enabled := mniTempSort.Enabled;

  boCheck := (lstTemp.ItemIndex >= 0) and (lstTemp.ItemIndex < lstTemp.Items.Count);
  mniTempDelete.Enabled := boCheck;
  mniTempAddToBlock.Enabled := boCheck;
end;

procedure TFrmSafeFilter.pmBlockPopup(Sender: TObject);
var
  boCheck: Boolean;
begin
  mniBlockSort.Enabled := lstBlock.Items.Count > 0;
  mniBlockClear.Enabled := mniBlockSort.Enabled;
  mniBlockAddAllToTemp.Enabled := mniBlockSort.Enabled;

  boCheck := (lstBlock.ItemIndex >= 0) and (lstBlock.ItemIndex < lstBlock.Items.Count);
  mniBlockDelete.Enabled := boCheck;
  mniBlockAddToTemp.Enabled := boCheck;
end;

procedure TFrmSafeFilter.mniTempRefreshClick(Sender: TObject);
var
  I: Integer;
begin
  lstTemp.Clear;
  g_TempIPList.Lock;
  try
    for I := 0 to g_TempIPList.Count - 1 do
    begin
      lstTemp.Items.Add(StrPas(inet_ntoa(TInAddr(pTSockaddr(g_TempIPList.Items[I]).nIPaddr))));
    end;
  finally
    g_TempIPList.UnLock;
  end;
end;

procedure TFrmSafeFilter.mniBlockRefreshClick(Sender: TObject);
var
  I: Integer;
begin
  lstBlock.Clear;
  g_BlockIPList.Lock;
  try
    for I := 0 to g_BlockIPList.Count - 1 do
    begin
      lstBlock.Items.Add(StrPas(inet_ntoa(TInAddr(pTSockaddr(g_BlockIPList.Items[I]).nIPaddr))));
    end;
  finally
    g_BlockIPList.UnLock;
  end;
end;

procedure TFrmSafeFilter.btnOKClick(Sender: TObject);
var
  IniFile: TIniFile;
begin
  if rbDisConnect.Checked then
    g_BlockMethod := bmDisconnect
  else if rbAddTempList.Checked then
    g_BlockMethod := bmTempBlock
  else
    g_BlockMethod := bmBlockList;

  g_nMaxClientPacketSize := seMaxClientPacketSize.Value;
  g_nMaxClientPacketCount := seMaxClientPacketCount.Value;
  g_boKickOverPacketSize := chkLostLine.Checked;

  g_boCheckClientPacketLegal := chkCheckClientPacketLegal.Checked;
  g_nCheckClientPacketCount := seCheckClientPacketCount.Value;

  g_dwAttackTick := seAttackTick.Value;
  g_nAttackCount := seAttackCount.Value;
  g_nMaxConnOfIPaddr := seMaxConnect.Value;
  g_dwKeepConnectTimeOut := seKeepConnectTimeOut.Value;

  g_dwIPCountLimitTime1 := seIPCountLimitTime1.Value;
  g_dwIPCountLimit1 := seIPCountLimit1.Value;
  g_dwIPCountLimitTime2 := seIPCountLimitTime2.Value;
  g_dwIPCountLimit2 := seIPCountLimit2.Value;

  g_dwSayMaxLen := seSayMaxLen.Value;
  g_dwSayTime := seSayTime.Value;
  g_dwSayMaxCount := seSayMaxCount.Value;
  g_dwSayDisableTime := seSayDisableTime.Value;

  g_dwDefenseLevel := trckbrDefenseLevel.Position;

  g_boDefenseToLevel1 := chkDefenseToLevel1.Checked;
  g_dwDefenseToLevel1 := seDefenseToLevel1.Value;

  g_boResotreDefense := chkResotreDefense.Checked;
  g_dwResotreDefense := seResotreDefense.Value;

  g_boAutoClearTemp := chkAutoClearTemp.Checked;
  g_dwAutoClearTemp := seAutoClearTemp.Value;

  g_boAddAllToTemp := chkAddAllToTemp.Checked;
  g_dwAddAllToTemp := seAddAllToTemp.Value;

  g_boOpenCheckClient := chkOpenCheckClient.Checked;
  g_CheckClientFailBlockMethod := TBlockIPMethod(cbbCheckClientFailBlockMode.ItemIndex);

  IniFile := TIniFile.Create(g_sIniFileName);
  IniFile.WriteInteger(GateClass, 'AttackTick', g_dwAttackTick);
  IniFile.WriteInteger(GateClass, 'AttackCount', g_nAttackCount);
  IniFile.WriteInteger(GateClass, 'MaxConnOfIPaddr', g_nMaxConnOfIPaddr);
  IniFile.WriteInteger(GateClass, 'BlockMethod', Integer(g_BlockMethod));
  IniFile.WriteInteger(GateClass, 'MaxClientPacketSize', g_nMaxClientPacketSize);
  IniFile.WriteInteger(GateClass, 'MaxClientPacketCount', g_nMaxClientPacketCount);
  IniFile.WriteInteger(GateClass, 'MaxClientMsgCount', nMaxClientMsgCount);
  IniFile.WriteBool(GateClass, 'KickOverPacket', g_boKickOverPacketSize);

  IniFile.WriteBool(GateClass, 'CheckClientPacketLegal', g_boCheckClientPacketLegal);
  IniFile.WriteInteger(GateClass, 'CheckClientPacketCount', g_nCheckClientPacketCount);

  // chongchong 2013-09-01
  IniFile.WriteInteger(GateClass, 'KeepConnectTimeOut', g_dwKeepConnectTimeOut);

  // 是否开启发言控制
  IniFile.WriteBool(GateClass, 'SayMsgControl', g_boSayMsgControl);

  // 发言文字最大长度
  IniFile.WriteInteger(GateClass, 'SayMaxLen', g_dwSayMaxLen);

  // 发言时间间隔
  IniFile.WriteInteger(GateClass, 'SayTime', g_dwSayTime);

  // 发言次数
  IniFile.WriteInteger(GateClass, 'SayMaxCount', g_dwSayMaxCount);

  // 禁言时间
  IniFile.WriteInteger(GateClass, 'SayDisableTime', g_dwSayDisableTime);

  IniFile.WriteInteger(GateClass, 'IPCountLimitTime1', g_dwIPCountLimitTime1);
  IniFile.WriteInteger(GateClass, 'IPCountLimit1', g_dwIPCountLimit1);
  IniFile.WriteInteger(GateClass, 'IPCountLimitTime2', g_dwIPCountLimitTime2);
  IniFile.WriteInteger(GateClass, 'IPCountLimit2', g_dwIPCountLimit2);

  // 防御等级
  IniFile.WriteInteger(GateClass, 'DefenseLevel', g_dwDefenseLevel);

  // 受攻击防御调为1级
  IniFile.WriteBool(GateClass, 'IsDefenseToLevel1', g_boDefenseToLevel1);

  // 受攻击防御调为1级 (攻击次数)
  IniFile.WriteInteger(GateClass, 'DefenseToLevel1', g_dwDefenseToLevel1);

  // 无攻击还原防御等级
  IniFile.WriteBool(GateClass, 'IsResotreDefense', g_boResotreDefense);

  // 无攻击还原防御等级 (120秒后)
  IniFile.WriteInteger(GateClass, 'ResotreDefense', g_dwResotreDefense);

  // 清除动态过滤列表
  IniFile.WriteBool(GateClass, 'IsAutoClearTemp', g_boAutoClearTemp);

  // 清除动态过滤列表 (自动清除间隔120秒)
  IniFile.WriteInteger(GateClass, 'AutoClearTemp', g_dwAutoClearTemp);

  // 连接加入到动态过滤
  IniFile.WriteBool(GateClass, 'IsAddAllToTemp', g_boAddAllToTemp);

  // 连接加入到动态过滤 (连接数)
  IniFile.WriteInteger(GateClass, 'AddAllToTemp', g_dwAddAllToTemp);

  IniFile.WriteBool(GateClass, 'OpenCheckClient', g_boOpenCheckClient);

  IniFile.WriteInteger(GateClass, 'CheckClientFailBlockMethod', Integer(g_CheckClientFailBlockMethod));

  IniFile.Free;

  Close;
end;

procedure TFrmSafeFilter.mniTempAddClick(Sender: TObject);
var
  sIPaddress: string;
begin
  sIPaddress := '';
  if not InputQueryEx('永久IP过滤', '请输入一个新的IP地址: ', '如：202.103.100.20', sIPaddress) then Exit;
  if not IsIPaddr(sIPaddress) then
  begin
    ErrMessage('输入的地址格式错误！');
    Exit;
  end;
  lstTemp.Items.Add(sIPaddress);

  g_TempIPList.Lock;
  try
    g_TempIPList.Add(sIPaddress);
  finally
    g_TempIPList.UnLock;
  end;
end;

procedure TFrmSafeFilter.mniBlockAddClick(Sender: TObject);
var
  sIPaddress: string;
begin
  sIPaddress := '';
  if not InputQueryEx('永久IP过滤', '请输入一个新的IP地址: ', '如：202.103.100.20', sIPaddress) then Exit;
  if not IsIPaddr(sIPaddress) then
  begin
    ErrMessage('输入的IP地址错误');
    Exit;
  end;
  lstBlock.Items.Add(sIPaddress);
  g_BlockIPList.Lock;
  try
    g_BlockIPList.Add(sIPaddress);
  finally
    g_BlockIPList.UnLock;
  end;
  SaveBlockIPList();
end;

procedure TFrmSafeFilter.chkSayMsgControlClick(Sender: TObject);
begin
  g_boSayMsgControl := chkSayMsgControl.Checked;
  seSayMaxLen.Enabled := g_boSayMsgControl;
  seSayTime.Enabled := g_boSayMsgControl;
  seSayMaxCount.Enabled := g_boSayMsgControl;
  seSayDisableTime.Enabled := g_boSayMsgControl;
end;

procedure TFrmSafeFilter.trckbrDefenseLevelChange(Sender: TObject);
begin
  UpdateHints;
end;

procedure TFrmSafeFilter.UpdateHints;
begin
  trckbrDefenseLevel.Hint := '调整范围在：1-10。分别是：严格-宽松。' +
    '等级为0时关闭攻击防御！当前等级：' + IntToStr(trckbrDefenseLevel.Position);

  chkDefenseToLevel1.Hint := '被攻击' + IntToStr(seDefenseToLevel1.Value) +
    '次后，如果你的防御等级不是1级，程序将自动调整你的防御等级为1级';
  seDefenseToLevel1.Hint := chkDefenseToLevel1.Hint;

  seResotreDefense.Hint := '每隔' + IntToStr(seResotreDefense.Value) +
    '秒自动清除动态过滤列表中的IP';
  chkResotreDefense.Hint := seResotreDefense.Hint;

  chkAutoClearTemp.Hint := '在没有攻击后，等待' + IntToStr(seAutoClearTemp.Value) +
    '秒程序将防御等级还原成你最初的设置';
  seAutoClearTemp.Hint := chkAutoClearTemp.Hint;

  chkAddAllToTemp.Hint := '当连接数达到' + IntToStr(seAddAllToTemp.Value) +
    '时，将链接列表的所有IP加入动态过滤';
  seAddAllToTemp.Hint := chkAddAllToTemp.Hint;
end;

procedure TFrmSafeFilter.mniActiveRefeshClick(Sender: TObject);
var
  I: Integer;
  List: TList;
  sIPaddr: string;
  Context: TMirClientContext;
begin
  lstActive.Clear;
  List := TList.Create;
  try
    TIOCPClientContextPool.Instance.GetOnlineContextList(List);

    for I := List.Count - 1 downto 0 do
    begin
      Context := TMirClientContext(List.Items[I]);

      {
      sUserName := Format('%-20s', [Context.sChrName]);
      sIPaddr := Context.RemoteAddr;
      if sIPaddr <> '' then
        lstActive.Items.AddObject(sUserName + sIPaddr, TObject(List.Items[I]));
      }

      sIPaddr := Context.RemoteAddr;
      if sIPaddr <> '' then
      begin
        sIPaddr := Format('%-18s', [sIPaddr]);
        lstActive.Items.AddObject(sIPaddr + Context.sChrName, TObject(List.Items[I]));
      end;
    end;
  finally
    List.Free;
  end;
end;

procedure TFrmSafeFilter.mniActiveSortClick(Sender: TObject);
begin
  lstActive.Sorted := True;
end;

function GetIPAddrFromActiveItem(S: string): string;
begin
  Result := Trim(Copy(S, 1, 18));
end;

function GetUserNameFromActiveItem(S: string): string;
begin
  Result := Copy(S, 19, MaxInt);
end;

procedure TFrmSafeFilter.mniActiveAddToTempClick(Sender: TObject);
var
  sIPaddr: string;
  sMsg, sTitle: string;
  Context: TIOCPClientContext;
begin
  if (lstActive.ItemIndex >= 0) and (lstActive.ItemIndex < lstActive.Items.Count) then
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[lstActive.ItemIndex]);

    sMsg := '将此IP加入到过态过滤列表后，此IP建立的所有连接将被强行中断，是否继续？';
    sTitle := '确认信息 - ' + sIPaddr;
    if Application.MessageBox(PChar(sMsg), PChar(sTitle), MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then Exit;

    lstTemp.Items.Add(sIPaddr);
    AddTempBlockIP(sIPaddr);

    Context := TIOCPClientContext(lstActive.Items.Objects[lstActive.ItemIndex]);
    if SameText(Context.RemoteAddr, sIPaddr) then
      Context.Close;
    mniActiveRefeshClick(Self);
  end;
end;

procedure TFrmSafeFilter.mniActiveAddAllToTempClick(Sender: TObject);
var
  I: Integer;
  sIPaddr: string;
  Context: TIOCPClientContext;
begin
  if not QuestionMessage('全部加入到过态过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？') then Exit;

  for I := 0 to lstActive.Items.Count - 1 do
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[I]);

    lstTemp.Items.Add(sIPaddr);
    AddTempBlockIP(sIPaddr);
    Context := TIOCPClientContext(lstActive.Items.Objects[I]);
    if SameText(Context.RemoteAddr, sIPaddr) then
      Context.Close;
  end;

  mniActiveRefeshClick(Self);
end;

procedure TFrmSafeFilter.mniActiveAddToBlockClick(Sender: TObject);
var
  sIPaddr: string;
  sMsg, sTitle: string;
  Context: TIOCPClientContext;
begin
  if (lstActive.ItemIndex >= 0) and (lstActive.ItemIndex < lstActive.Items.Count) then
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[lstActive.ItemIndex]);

    sMsg := '将此IP加入到永久过滤列表后，此IP建立的所有连接将被强行中断，是否继续？';
    sTitle := '确认信息 - ' + sIPaddr;
    if not QuestionMessage(sMsg, sTitle) then Exit;

    lstBlock.Items.Add(sIPaddr);
    AddBlockIP(sIPaddr);
    Context := TIOCPClientContext(lstActive.Items.Objects[lstActive.ItemIndex]);

    if SameText(Context.RemoteAddr, sIPaddr) then
      Context.Close;

    mniActiveRefeshClick(Self);
    SaveBlockIPList;
  end;
end;

procedure TFrmSafeFilter.mniActiveAddAllToBlockClick(Sender: TObject);
var
  I: Integer;
  sIPaddr: string;
  sMsg: string;
  Context: TIOCPClientContext;
begin
  sMsg := '全部加入到永久过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？';
  if not QuestionMessage(sMsg) then Exit;

  for I := 0 to lstActive.Items.Count - 1 do
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[I]);
    lstBlock.Items.Add(sIPaddr);
    AddBlockIP(sIPaddr);
    Context := TIOCPClientContext(lstActive.Items.Objects[I]);
    if SameText(Context.RemoteAddr, sIPaddr) then
      Context.Close;
  end;

  mniActiveRefeshClick(Self);
  SaveBlockIPList;
end;

procedure TFrmSafeFilter.mniActiveAddAllNoUserToTempClick(Sender: TObject);
var
  I: Integer;
  sUserName, sIPaddr: string;
  sMsg: string;
  Context: TIOCPClientContext;
begin
  sMsg := '全部加入到过态过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？';
  if not QuestionMessage(sMsg) then Exit;

  for I := 0 to lstActive.Items.Count - 1 do
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[I]);
    sUserName := GetUserNameFromActiveItem(lstActive.Items.Strings[I]);

    if Length(sUserName) = 0 then
    begin
      lstTemp.Items.Add(sIPaddr);
      AddTempBlockIP(sIPaddr);
      Context := TIOCPClientContext(lstActive.Items.Objects[I]);
      if SameText(Context.RemoteAddr, sIPaddr) then
        Context.Close;
    end;
  end;

  mniActiveRefeshClick(Self);
end;

procedure TFrmSafeFilter.mniActiveAddAllNoUserToBlockClick(
  Sender: TObject);
var
  I: Integer;
  sUserName, sIPaddr: string;
  sMsg: string;
  Context: TIOCPClientContext;
begin
  sMsg := '全部加入到永久过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？';
  if not QuestionMessage(sMsg) then Exit;

  for I := 0 to lstActive.Items.Count - 1 do
  begin
    sIPaddr := GetIPAddrFromActiveItem(lstActive.Items.Strings[I]);
    sUserName := GetUserNameFromActiveItem(lstActive.Items.Strings[I]);

    if Length(sUserName) = 0 then
    begin
      lstBlock.Items.Add(sIPaddr);
      AddBlockIP(sIPaddr);
      Context := TIOCPClientContext(lstActive.Items.Objects[I]);
      if SameText(Context.RemoteAddr, sIPaddr) then
        Context.Close;
    end;
  end;

  mniActiveRefeshClick(Self);
  SaveBlockIPList;
end;

procedure TFrmSafeFilter.mniActiveKickClick(Sender: TObject);
var
  S, sUserName, sIPaddr: string;
begin
  if (lstActive.ItemIndex >= 0) and (lstActive.ItemIndex < lstActive.Items.Count) then
  begin
    S := lstActive.Items.Strings[lstActive.ItemIndex];
    sIPaddr := GetIPAddrFromActiveItem(S);
    sUserName := GetUserNameFromActiveItem(S);

    if QuestionMessage('是否确认将此连接断开？', '确认信息 - ' + sIPaddr + ' [' + sUserName + ']') then
    begin
      TIOCPClientContext(lstActive.Items.Objects[lstActive.ItemIndex]).Close;
      mniActiveRefeshClick(Self);
    end;
  end;
end;

procedure TFrmSafeFilter.pmActiveChange(Sender: TObject;
  Source: TMenuItem; Rebuild: Boolean);
var
  boCheck: Boolean;
begin
  mniActiveSort.Enabled := lstActive.Items.Count > 0;
  mniActiveAddAllToTemp.Enabled := mniActiveSort.Enabled;
  mniActiveAddAllToBlock.Enabled := mniActiveSort.Enabled;

  boCheck := (lstActive.ItemIndex >= 0) and (lstActive.ItemIndex < lstActive.Items.Count);
  mniActiveAddToTemp.Enabled := boCheck;
  mniActiveAddToBlock.Enabled := boCheck;
  mniActiveKick.Enabled := boCheck;
end;

procedure TFrmSafeFilter.mniIpSectionAddClick(Sender: TObject);
var
  sIPaddress: string;
  nBeginaddr, nEndaddr: LongWord;
  IPSection: PTIPSection;
begin
  sIPaddress := '';
  if not InputQueryEx('过滤IP段信息', '请输入起始IP地址: ', '如：202.103.100.1', sIPaddress) then Exit;
  nBeginaddr := IP2Long(sIPaddress);
  if nBeginaddr = LongWord(INADDR_NONE) then
  begin
    Application.MessageBox('输入的地址格式不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  if not InputQueryEx('过滤IP段信息', '请输入结束IP地址: ', '如：202.103.100.100', sIPaddress) then Exit;
  nEndaddr := IP2Long(sIPaddress);
  if nEndaddr = LongWord(INADDR_NONE) then
  begin
    Application.MessageBox('输入的地址格式不正确！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  if nEndaddr >= nBeginaddr then
  begin
    New(IPSection);
    IPSection.nBeginAddr := nBeginaddr;
    IPSection.nEndAddr := nEndaddr;
    g_IPSectionList.Add(IPSection);
    lstIpSection.AddItem(Long2IP(nBeginAddr) + ' - ' + Long2IP(nEndAddr), TObject(IPSection));
    SaveIPSectionList;
  end
  else
    Application.MessageBox('结束地址不能小于开始地址', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TFrmSafeFilter.mniIpSectionDelClick(Sender: TObject);
var
  I: Integer;
  IPSection: pTIPSection;
begin
  if (lstIpSection.ItemIndex >= 0) and (lstIpSection.ItemIndex < lstIpSection.Items.Count) then
  begin
    IPSection := pTIPSection(lstIpSection.Items.Objects[lstIpSection.ItemIndex]);
    lstIpSection.Items.Delete(lstIpSection.ItemIndex);

    g_IPSectionList.Lock;
    try
      for I := g_IPSectionList.Count - 1 downto 0 do
      begin
        if pTIPSection(g_IPSectionList.Items[I]) = IPSection then
        begin
          g_IPSectionList.Delete(I);
          Dispose(IPSection);
          Break;
        end;
      end;
    finally
      g_IPSectionList.Unlock;
    end;
  end;
  SaveIPSectionList;
end;

procedure TFrmSafeFilter.mniIpSectionSortClick(Sender: TObject);
begin
  lstIpSection.Sorted := True;
end;

procedure TFrmSafeFilter.pmIpSectionPopup(Sender: TObject);
var
  boCheck: Boolean;
begin
  mniIpSectionSort.Enabled := lstIpSection.Items.Count > 0;

  boCheck := (lstIpSection.ItemIndex >= 0) and (lstIpSection.ItemIndex < lstIpSection.Items.Count);
  mniIpSectionDel.Enabled := boCheck;
end;

procedure TFrmSafeFilter.ErrMessage(MsgStr: string);
begin
  Application.MessageBox(PChar(MsgStr), '错误', MB_OK or MB_ICONERROR)
end;

function TFrmSafeFilter.QuestionMessage(const MsgStr: string; MsgTitle: string): Boolean;
begin
  if MsgTitle = '' then
    MsgTitle := '询问';
  Result := Application.MessageBox(PChar(MsgStr), PChar(MsgTitle),
    MB_OKCANCEL + MB_ICONQUESTION) = ID_OK;
end;

procedure TFrmSafeFilter.lstActiveKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
var
  I: Integer;
  S1, S2, StrInput: string;
  IPList: TListBox;
begin
  if not (Sender is TListBox) then Exit;
  IPList := Sender as TListBox;

  if (ssCtrl in Shift) and (Key = Ord('F')) then
  begin
    if (Sender = lstTempMac) or (Sender = lstBlockMac) then
    begin
      S1 := '输入MAC';
      S2 := '请输入要查找的MAC地址'
    end
    else
    begin
      S1 := '输入IP';
      S2 := '请输入要查找的IP地址'
    end;
    if InputQuery(S1, S2, StrInput) then
    begin
      for I := 0 to IPList.Items.Count - 1 do
      begin
        if SameText(StrInput, IPList.Items[I]) then
        begin
          IPList.ItemIndex := I;
          Break;
        end;
      end;
    end;
  end;
end;

procedure TFrmSafeFilter.mniTempMacRefreshClick(Sender: TObject);
var
  I: Integer;
begin
  lstTempMac.Clear;
  g_TempMacList.Lock;
  try
    for I := 0 to g_TempMacList.Count - 1 do
    begin
      lstTempMac.Items.Add(g_TempMacList.Strings[I]);
    end;
  finally
    g_TempMacList.UnLock;
  end;
end;

procedure TFrmSafeFilter.mniTempMacSortClick(Sender: TObject);
begin
  lstTempMac.Sorted := True;
end;

procedure TFrmSafeFilter.mniTempMacAddClick(Sender: TObject);
var
  sMac: string;
begin
  sMac := '';
  if not InputQueryEx('动态MAC过滤', '请输入一个新的MAC地址: ', '', sMac) then Exit;

  g_TempMacList.Lock;
  try
    if g_TempMacList.IndexOf(sMac) < 0 then
    begin
      g_TempMacList.Add(sMac);
      lstTempMAC.Items.Add(sMac);
    end;
  finally
    g_TempMacList.UnLock;
  end;
end;

procedure TFrmSafeFilter.mniTempMacDeleteClick(Sender: TObject);
var
  Index: Integer;
begin
  if (lstTempMac.ItemIndex >= 0) and (lstTempMac.ItemIndex < lstTempMac.Items.Count) then
  begin
    g_TempMacList.Lock;
    try
      Index := g_TempMacList.IndexOf(lstTempMac.Items[lstTempMac.ItemIndex]);
      if Index >= 0 then
        g_TempMacList.Delete(Index);
    finally
      g_TempMacList.UnLock;
    end;
    lstTempMac.Items.Delete(lstTempMac.ItemIndex);
  end;
end;

procedure TFrmSafeFilter.mniTempMacClearClick(Sender: TObject);
begin
  g_TempMacList.Lock;
  try
    g_TempMacList.Clear;
  finally
    g_TempMacList.UnLock;
  end;

  lstTempMac.Clear;
end;

procedure TFrmSafeFilter.mniTempMacAddToBlockClick(Sender: TObject);
var
  sMac: string;
begin
  if (lstTempMac.ItemIndex >= 0) and (lstTempMac.ItemIndex < lstTempMac.Items.Count) then
  begin
    sMac := lstTempMac.Items[lstTempMac.ItemIndex];

    g_TempMacList.Lock;
    try
      g_TempMacList.Delete(lstTempMac.ItemIndex);
    finally
      g_TempMacList.UnLock;
    end;

    lstTempMac.Items.Delete(lstTempMac.ItemIndex);
    lstBlockMac.Items.Add(sMac);

    AddBlockMac(sMac);

    SaveBlockMacList;
  end;
end;

procedure TFrmSafeFilter.mniTempMacAddAllToBlockClick(Sender: TObject);
var
  sMac: string;
  I: Integer;
begin
  for I := 0 to lstTempMac.Items.Count - 1 do
  begin
    sMac := lstTempMac.Items[I];
    lstBlockMac.Items.Add(sMac);

    AddBlockMac(sMac);
  end;

  g_TempMacList.Clear;
  lstTempMac.Clear;

  SaveBlockMacList;
end;

procedure TFrmSafeFilter.mniBlockMacRefreshClick(Sender: TObject);
var
  I: Integer;
begin
  lstBlockMac.Clear;
  g_BlockMacList.Lock;
  try
    for I := 0 to g_BlockMacList.Count - 1 do
    begin
      lstBlockMac.Items.Add(g_BlockMacList.Strings[I]);
    end;
  finally
    g_BlockMacList.UnLock;
  end;
end;

procedure TFrmSafeFilter.mniBlockMacSortClick(Sender: TObject);
begin
  lstBlockMac.Sorted := True;
end;

procedure TFrmSafeFilter.mniBlockMacAddClick(Sender: TObject);
var
  sMac: string;
begin
  sMac := '';
  if not InputQueryEx('永久MAC过滤', '请输入一个新的MAC地址: ', '', sMac) then Exit;

  g_BlockMacList.Lock;
  try
    if g_BlockMacList.IndexOf(sMac) < 0 then
    begin
      g_BlockMacList.Add(sMac);
      lstBlockMac.Items.Add(sMac);
    end;
  finally
    g_BlockMacList.UnLock;
  end;
  
  SaveBlockMacList();
end;

procedure TFrmSafeFilter.mniBlockMacDeleteClick(Sender: TObject);
var
  Index: Integer;
begin
  if (lstBlockMac.ItemIndex >= 0) and (lstBlockMac.ItemIndex < lstBlockMac.Items.Count) then
  begin
    g_BlockMacList.Lock;
    try
      Index := g_BlockMacList.IndexOf(lstBlockMac.Items[lstBlockMac.ItemIndex]);

      if Index >= 0 then
        g_BlockMacList.Delete(Index);
    finally
      g_BlockMacList.UnLock;
    end;

    lstBlockMac.Items.Delete(lstBlockMac.ItemIndex);
    SaveBlockMacList();
  end;
end;

procedure TFrmSafeFilter.mniBlockMacClearClick(Sender: TObject);
begin
  g_BlockMacList.Clear;
  lstBlockMac.Clear;
  SaveBlockMacList();
end;

procedure TFrmSafeFilter.mniBlockMacAddToTempMacClick(Sender: TObject);
var
  sMac: string;
begin
  if (lstBlockMac.ItemIndex >= 0) and (lstBlockMac.ItemIndex < lstBlockMac.Items.Count) then
  begin
    sMac := lstBlockMac.Items[lstBlockMac.ItemIndex];

    g_BlockMacList.Lock;
    try
      g_BlockMacList.Delete(lstBlockMac.ItemIndex);
    finally
      g_BlockMacList.UnLock;
    end;
    lstTempMac.Items.Add(sMac);
    lstBlockMac.Items.Delete(lstBlockMac.ItemIndex);

    AddTempBlockMac(sMac);
    SaveBlockMacList;
  end;
end;

procedure TFrmSafeFilter.mniBlockMacAddAllToTempMacClick(Sender: TObject);
var
  sMac: string;
  I: Integer;
begin
  for I := 0 to lstBlockMac.Items.Count - 1 do
  begin
    sMac := lstBlockMac.Items[I];
    lstTempMac.Items.Add(sMac);
    AddTempBlockMac(sMac);
  end;

  g_BlockMacList.Clear;
  lstBlockMac.Clear;

  SaveBlockMacList;
end;

procedure TFrmSafeFilter.pmTempMacPopup(Sender: TObject);
var
  boCheck: Boolean;
begin
  mniTempMacSort.Enabled := lstTempMac.Items.Count > 0;
  mniTempMacClear.Enabled := mniTempMacSort.Enabled;
  mniTempMacAddAllToBlock.Enabled := mniTempMacSort.Enabled;

  boCheck := (lstTempMac.ItemIndex >= 0) and (lstTempMac.ItemIndex < lstTempMac.Items.Count);
  mniTempMacDelete.Enabled := boCheck;
  mniTempMacAddToBlock.Enabled := boCheck;
end;

procedure TFrmSafeFilter.pmBlockMacPopup(Sender: TObject);
var
  boCheck: Boolean;
begin
  mniBlockMacSort.Enabled := lstBlockMac.Items.Count > 0;
  mniBlockMacClear.Enabled := mniBlockMacSort.Enabled;
  mniBlockMacAddAllToTempMac.Enabled := mniBlockMacSort.Enabled;

  boCheck := (lstBlockMac.ItemIndex >= 0) and (lstBlockMac.ItemIndex < lstBlockMac.Items.Count);
  mniBlockMacDelete.Enabled := boCheck;
  mniBlockMacAddToTempMac.Enabled := boCheck;
end;

end.

