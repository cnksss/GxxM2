unit GHeroDBConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, StdCtrls, Mask, RzEdit, RzBtnEdt, ShlObj, ComObj, ActiveX;

type
  TFrmHeroDB = class(TForm)
    PageControl: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    TabSheet4: TTabSheet;
    ButtonClose: TButton;
    Label1: TLabel;
    EditHeroDB: TEdit;
    Label2: TLabel;
    EditHeroDBPath: TRzButtonEdit;
    ButtonSaveHeroDBConfig: TButton;
    GroupBox1: TGroupBox;
    ListBoxStdItems: TListBox;
    ButtonCreateStdItemsField: TButton;
    GroupBox2: TGroupBox;
    ListBoxMonster: TListBox;
    GroupBox3: TGroupBox;
    ListBoxMagic: TListBox;
    MemoLog: TMemo;
    MemoLog1: TMemo;
    MemoLog2: TMemo;
    MemoLog3: TMemo;
    ButtonMonsterField: TButton;
    ButtonMagicField: TButton;
    procedure EditHeroDBPathButtonClick(Sender: TObject);
    procedure ButtonSaveHeroDBConfigClick(Sender: TObject);
    procedure ButtonCloseClick(Sender: TObject);
    procedure ButtonCreateStdItemsFieldClick(Sender: TObject);
    procedure ButtonMagicFieldClick(Sender: TObject);
    procedure ButtonMonsterFieldClick(Sender: TObject);
  private
    { Private declarations }
  public
    procedure Open;
    function CheckHeroDB: Boolean;
  end;

implementation
uses GHeroDB, GShare;
{$R *.dfm}

function SelectDirCB(Wnd: HWND; uMsg: UINT; lParam, lpData: lParam): Integer stdcall;
begin
  if (uMsg = BFFM_INITIALIZED) and (lpData <> 0) then
    SendMessage(Wnd, BFFM_SETSELECTION, Integer(True), lpData);
  Result := 0;
end;

function SelectDirectory(const Caption: string; const Root: WideString;
  var Directory: string; Owner: Thandle): Boolean;
var
  WindowList: Pointer;
  BrowseInfo: TBrowseInfo;
  Buffer: PChar;
  RootItemIDList, ItemIDList: PItemIDList;
  ShellMalloc: IMalloc;
  IDesktopFolder: IShellFolder;
  Eaten, Flags: LongWord;
begin
  Result := False;
  if not DirectoryExists(Directory) then
    Directory := '';
  FillChar(BrowseInfo, SizeOf(BrowseInfo), 0);
  if (ShGetMalloc(ShellMalloc) = S_OK) and (ShellMalloc <> nil) then begin
    Buffer := ShellMalloc.Alloc(MAX_PATH);
    try
      RootItemIDList := nil;
      if Root <> '' then begin
        SHGetDesktopFolder(IDesktopFolder);
        IDesktopFolder.ParseDisplayName(Application.Handle, nil,
          POleStr(Root), Eaten, RootItemIDList, Flags);
      end;
      with BrowseInfo do begin
        hwndOwner := Owner;
        pidlRoot := RootItemIDList;
        pszDisplayName := Buffer;
        lpszTitle := PChar(Caption);
        ulFlags := BIF_RETURNONLYFSDIRS + BIF_USENEWUI;
        if Directory <> '' then begin
          lpfn := SelectDirCB;
          lParam := Integer(PChar(Directory));
        end;
      end;
      WindowList := DisableTaskWindows(0);
      try
        ItemIDList := ShBrowseForFolder(BrowseInfo);
      finally
        EnableTaskWindows(WindowList);
      end;
      Result := ItemIDList <> nil;
      if Result then begin
        ShGetPathFromIDList(ItemIDList, Buffer);
        ShellMalloc.Free(ItemIDList);
        Directory := Buffer;
      end;
    finally
      ShellMalloc.Free(Buffer);
    end;
  end;
end;

procedure TFrmHeroDB.EditHeroDBPathButtonClick(Sender: TObject);
var
  sFilePath: string;
begin
  if SelectDirectory('请选择数据库目录' + #13#10 + '一般在D:\MirServer\Mud2\DB', '', sFilePath, Handle) then begin
    if (sFilePath <> '') and (sFilePath[Length(sFilePath)] = '\') then
      sFilePath := Copy(sFilePath, 1, Length(sFilePath) - 1);

    MemoLog.Font.Color := clRed;
    MemoLog.Clear;
    if not FileExists(sFilePath + '\StdItems.DB') then
      MemoLog.Lines.Add('当前目录中没有发现“StdItems.DB”');
    if not FileExists(sFilePath + '\Monster.DB') then
      MemoLog.Lines.Add('当前目录中没有发现“Monster.DB”');
    if not FileExists(sFilePath + '\Magic.DB') then
      MemoLog.Lines.Add('当前目录中没有发现“Magic.DB”');
    EditHeroDBPath.Text := sFilePath;
  end;
end;

procedure TFrmHeroDB.ButtonSaveHeroDBConfigClick(Sender: TObject);
var
  sFilePath: string;
  HeroDB: THeroDB;
begin
  MemoLog.Font.Color := clRed;
  MemoLog.Clear;
  g_sHeroDBName := Trim(EditHeroDB.Text);
  sFilePath := Trim(EditHeroDBPath.Text);
  if (sFilePath <> '') and (sFilePath[Length(sFilePath)] = '\') then
    sFilePath := Copy(sFilePath, 1, Length(sFilePath) - 1);

  if not FileExists(sFilePath + '\StdItems.DB') then
    MemoLog.Lines.Add('当前目录中没有发现“StdItems.DB”');
  if not FileExists(sFilePath + '\Monster.DB') then
    MemoLog.Lines.Add('当前目录中没有发现“Monster.DB”');
  if not FileExists(sFilePath + '\Magic.DB') then
    MemoLog.Lines.Add('当前目录中没有发现“Magic.DB”');
  if MemoLog.Lines.Count > 0 then Exit;

  HeroDB := THeroDB.Create;
  HeroDB.SaveHeroDBConfigFile(g_sHeroDBName, sFilePath);
  HeroDB.Free;

  g_IniConf.WriteString('GameConf', 'HeroDBName', g_sHeroDBName);

  g_boHeroDBOK := not CheckHeroDB;
  if g_boHeroDBOK then begin
    Application.MessageBox('数据库更新成功！！！', '提示信息', MB_OK + MB_ICONWARNING);
    Close;
  end;
end;

function TFrmHeroDB.CheckHeroDB: Boolean;
const
  sMagicNeed = 'NeedL%d';
  sMagicTrain = 'L%dTrain';
var
  I: Integer;
  HeroDB: THeroDB;
  sMagicNeedFieldName: string;
  sMagicTrainFieldName: string;
begin
  MemoLog.Lines.Clear;
  MemoLog.Font.Color := clRed;
  MemoLog1.Lines.Clear;
  MemoLog1.Font.Color := clRed;
  MemoLog2.Lines.Clear;
  MemoLog2.Font.Color := clRed;
  MemoLog3.Lines.Clear;
  MemoLog3.Font.Color := clRed;
  ListBoxStdItems.Clear;
  ListBoxMonster.Clear;
  ListBoxMagic.Clear;
  TabSheet1.TabVisible := False;
  TabSheet2.TabVisible := False;
  TabSheet3.TabVisible := False;
  TabSheet4.TabVisible := False;

  HeroDB := THeroDB.Create;
  if not HeroDB.HeroDBExist(g_sHeroDBName) then
  begin
    MemoLog.Lines.Add(g_sHeroDBName + '配置错误！');
    TabSheet1.TabVisible := True;
  end;

  if not TabSheet1.TabVisible then begin
    if not HeroDB.TableExist(g_sHeroDBName, 'StdItems') then
    begin
      MemoLog.Lines.Add(g_sHeroDBName + '配置错误,没有发现“StdItems.DB”！');
      TabSheet1.TabVisible := True;
    end;
    if not HeroDB.TableExist(g_sHeroDBName, 'Monster') then
    begin
      MemoLog.Lines.Add(g_sHeroDBName + '配置错误,没有发现“Monster.DB”！');
      TabSheet1.TabVisible := True;
    end;
    if not HeroDB.TableExist(g_sHeroDBName, 'Magic') then
    begin
      MemoLog.Lines.Add(g_sHeroDBName + '配置错误,没有发现“Magic.DB”！');
      TabSheet1.TabVisible := True;
    end;
  end;

  if not TabSheet1.TabVisible then
  begin
    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Color') then
    begin
      ListBoxStdItems.Items.Add('Color');
      TabSheet2.TabVisible := True;
    end;
    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'OverLap') then
    begin
      ListBoxStdItems.Items.Add('OverLap');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'HP') then
    begin
      ListBoxStdItems.Items.Add('HP');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'MP') then
    begin
      ListBoxStdItems.Items.Add('MP');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Light') then
    begin
      ListBoxStdItems.Items.Add('Light');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Horse') then
    begin
      ListBoxStdItems.Items.Add('Horse');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Element') then
    begin
      ListBoxStdItems.Items.Add('Element');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Expand1') then
    begin
      ListBoxStdItems.Items.Add('Expand1');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Expand2') then
    begin
      ListBoxStdItems.Items.Add('Expand2');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Expand3') then
    begin
      ListBoxStdItems.Items.Add('Expand3');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Expand4') then
    begin
      ListBoxStdItems.Items.Add('Expand4');
      TabSheet2.TabVisible := True;
    end;


    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Expand5') then
    begin
      ListBoxStdItems.Items.Add('Expand5');
      TabSheet2.TabVisible := True;
    end;

    for I := 1 to 24 do
    begin
      if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'Element' + IntToStr(I)) then
      begin
        ListBoxStdItems.Items.Add('Element' + IntToStr(I));
        TabSheet2.TabVisible := True;
      end;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'InsuranceCurrency') then
    begin
      ListBoxStdItems.Items.Add('InsuranceCurrency');
      TabSheet2.TabVisible := True;
    end;

    if not HeroDB.FieldExist(g_sHeroDBName, 'StdItems', 'InsuranceGold') then
    begin
      ListBoxStdItems.Items.Add('InsuranceGold');
      TabSheet2.TabVisible := True;
    end;

    if not TabSheet2.TabVisible then
    begin
      // 添加新字段 piaoyun 2013-12-20
      if not HeroDB.FieldExist(g_sHeroDBName, 'Monster', 'AttackState') then
      begin
        ListBoxMonster.Items.Add('AttackState');
        TabSheet3.TabVisible := True;
      end;

      if not HeroDB.FieldExist(g_sHeroDBName, 'Monster', 'ExploreItem') then
      begin
        ListBoxMonster.Items.Add('ExploreItem');
        TabSheet3.TabVisible := True;
      end;

      if not HeroDB.FieldExist(g_sHeroDBName, 'Monster', 'AttackSource') then
      begin
        ListBoxMonster.Items.Add('AttackSource');
        TabSheet3.TabVisible := True;
      end;

      if not HeroDB.FieldExist(g_sHeroDBName, 'Monster', 'DisableSimpleActor') then
      begin
        ListBoxMonster.Items.Add('DisableSimpleActor');
        TabSheet3.TabVisible := True;
      end;

      if not TabSheet3.Visible then
      begin
        for I := 0 to 14 do begin
          sMagicNeedFieldName := Format('NeedL%d', [I + 1]);
          sMagicTrainFieldName := Format('L%dTrain', [I + 1]);
          if not HeroDB.FieldExist(g_sHeroDBName, 'Magic', sMagicNeedFieldName) then
          begin
            ListBoxMagic.Items.Add(sMagicNeedFieldName);
            TabSheet4.TabVisible := True;
          end;
          if not HeroDB.FieldExist(g_sHeroDBName, 'Magic', sMagicTrainFieldName) then
          begin
            ListBoxMagic.Items.Add(sMagicTrainFieldName);
            TabSheet4.TabVisible := True;
          end;
        end;
        if not HeroDB.FieldExist(g_sHeroDBName, 'Magic', 'MaxTrainLv') then
        begin
          ListBoxMagic.Items.Add('MaxTrainLv');
          TabSheet4.TabVisible := True;
        end;
        if not HeroDB.FieldExist(g_sHeroDBName, 'Magic', 'CanUpgrade') then
        begin
          ListBoxMagic.Items.Add('CanUpgrade');
          TabSheet4.TabVisible := True;
        end;

        // 添加新字段 piaoyun 2013-12-20
        if not HeroDB.FieldExist(g_sHeroDBName, 'Magic', 'MaxUpgradeLv') then
        begin
          ListBoxMagic.Items.Add('MaxUpgradeLv');
          TabSheet4.TabVisible := True;
        end;
      end;
    end;
  end;
  Result := (TabSheet1.TabVisible or TabSheet2.TabVisible or TabSheet3.TabVisible or TabSheet4.TabVisible);
  HeroDB.Free;
end;

procedure TFrmHeroDB.Open;
begin
  EditHeroDB.Text := g_sHeroDBName;
  EditHeroDBPath.Text := g_sGameDirectory + 'Mud2\DB';
  Self.ShowModal;
end;

procedure TFrmHeroDB.ButtonCloseClick(Sender: TObject);
begin
  g_boHeroDBOK := not CheckHeroDB;
  Close;
end;

procedure TFrmHeroDB.ButtonCreateStdItemsFieldClick(Sender: TObject);
var
  I: Integer;
  HeroDB: THeroDB;
begin
  ButtonCreateStdItemsField.Enabled := False;
  MemoLog1.Lines.Clear;
  MemoLog1.Font.Color := clRed;
  HeroDB := THeroDB.Create;
  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Color', 255, 2) then
    MemoLog1.Lines.Add('Color字段创建成功')
  else
    MemoLog1.Lines.Add('Color字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'OverLap', 0, 2) then
    MemoLog1.Lines.Add('OverLap字段创建成功')
  else
    MemoLog1.Lines.Add('OverLap字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'HP', 0, 4) then
    MemoLog1.Lines.Add('HP字段创建成功')
  else
    MemoLog1.Lines.Add('HP字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'MP', 0, 4) then
    MemoLog1.Lines.Add('MP字段创建成功')
  else
    MemoLog1.Lines.Add('MP字段创建失败');


  // 自动创建缺少的Light字段 piaoyun 2013-08-17
  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Light', 0, 4) then
    MemoLog1.Lines.Add('Light字段创建成功')
  else
    MemoLog1.Lines.Add('Light字段创建失败');

  // 自动创建缺少的Light字段 piaoyun 2013-11-19
  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Horse', 0, 4) then
    MemoLog1.Lines.Add('Horse字段创建成功')
  else
    MemoLog1.Lines.Add('Horse字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Element', 0, 2) then
    MemoLog1.Lines.Add('Element字段创建成功')
  else
    MemoLog1.Lines.Add('Element字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Expand1', 0, 4) then
    MemoLog1.Lines.Add('Expand1字段创建成功')
  else
    MemoLog1.Lines.Add('Expand1字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Expand2', 0, 4) then
    MemoLog1.Lines.Add('Expand2字段创建成功')
  else
    MemoLog1.Lines.Add('Expand2字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Expand3', 0, 4) then
    MemoLog1.Lines.Add('Expand3字段创建成功')
  else
    MemoLog1.Lines.Add('Expand3字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Expand4', 0, 4) then
    MemoLog1.Lines.Add('Expand4字段创建成功')
  else
    MemoLog1.Lines.Add('Expand4字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Expand5', 0, 4) then
    MemoLog1.Lines.Add('Expand5字段创建成功')
  else
    MemoLog1.Lines.Add('Expand5字段创建失败');

    
  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'InsuranceCurrency', 0, 4) then
    MemoLog1.Lines.Add('InsuranceCurrency字段创建成功')
  else
    MemoLog1.Lines.Add('InsuranceCurrency字段创建失败');

  if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'InsuranceGold', 0, 4) then
    MemoLog1.Lines.Add('InsuranceGold字段创建成功')
  else
    MemoLog1.Lines.Add('InsuranceGold字段创建失败');

  for I := 1 to 24 do
  begin
    if HeroDB.CreateField(g_sHeroDBName, 'StdItems', 'Element' + IntToStr(I), 0, 2) then
      MemoLog1.Lines.Add('Element' + IntToStr(I) + '字段创建成功')
    else
      MemoLog1.Lines.Add('Element' + IntToStr(I) + '字段创建失败');
  end;

  {if HeroDB.ModifyField(g_sHeroDBName, 'StdItems', 'Name', 30) then
    MemoLog1.Lines.Add('Name字段长度修改成功')
  else
    MemoLog1.Lines.Add('Name字段长度修改失败'); }

  HeroDB.Free;
  ButtonCreateStdItemsField.Enabled := True;
  g_boHeroDBOK := not CheckHeroDB;
  if g_boHeroDBOK then begin
    Application.MessageBox('数据库更新成功！！！', '提示信息', MB_OK + MB_ICONWARNING);
    Close;
  end;
end;

procedure TFrmHeroDB.ButtonMagicFieldClick(Sender: TObject);
var
  I, nValue: Integer;
  HeroDB: THeroDB;
  sFieldName: string;
begin
  ButtonMagicField.Enabled := False;
  MemoLog3.Lines.Clear;
  MemoLog3.Font.Color := clRed;

  HeroDB := THeroDB.Create;
  for I := 0 to ListBoxMagic.Count - 1 do begin
    sFieldName := ListBoxMagic.Items.Strings[I];
    if SameText(sFieldName, 'CanUpgrade') or SameText(sFieldName, 'MaxUpgradeLv') then
      nValue := 0
    else if sFieldName[1] = 'M' then
      nValue := 3
    else if sFieldName[1] = 'N' then
      nValue := 20
    else
      nValue := 200;

    if HeroDB.CreateField(g_sHeroDBName, 'Magic', sFieldName, nValue, 4) then
      MemoLog3.Lines.Add(sFieldName + '字段创建成功')
    else
      MemoLog3.Lines.Add(sFieldName + '字段创建失败');
  end;

  HeroDB.Free;
  ButtonMagicField.Enabled := True;
  g_boHeroDBOK := not CheckHeroDB;
  if g_boHeroDBOK then
  begin
    Application.MessageBox('数据库更新成功！！！', '提示信息', MB_OK + MB_ICONWARNING);
    Close;
  end;
end;

procedure TFrmHeroDB.ButtonMonsterFieldClick(Sender: TObject);
var
  I, nValue: Integer;
  HeroDB: THeroDB;
  sFieldName: string;
begin
  ButtonMonsterField.Enabled := False;
  MemoLog2.Lines.Clear;
  MemoLog2.Font.Color := clRed;

  HeroDB := THeroDB.Create;
  for I := 0 to ListBoxMonster.Count - 1 do
  begin
    sFieldName := ListBoxMonster.Items.Strings[I];
    nValue := 0;
    if HeroDB.CreateField(g_sHeroDBName, 'Monster', sFieldName, nValue, 4) then
      MemoLog2.Lines.Add(sFieldName + '字段创建成功')
    else
      MemoLog2.Lines.Add(sFieldName + '字段创建失败');
  end;

  HeroDB.Free;
  ButtonMonsterField.Enabled := True;
  g_boHeroDBOK := not CheckHeroDB;
  if g_boHeroDBOK then
  begin
    Application.MessageBox('数据库更新成功！！！', '提示信息', MB_OK + MB_ICONWARNING);
    Close;
  end;
end;

end.
