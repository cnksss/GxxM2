unit ClientModules;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, ExtCtrls;

type
  TftmClientModules = class(TForm)
    Panel1: TPanel;
    ButtonModuleAdd: TButton;
    ButtonModuleDel: TButton;
    ButtonModuleSave: TButton;
    Label1: TLabel;
    Label2: TLabel;
    EditFileName: TEdit;
    EditMD5: TEdit;
    ButtonLoad: TButton;
    CheckBoxGetCheckModule: TCheckBox;
    PageControl: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    ListViewClientModule: TListView;
    ListViewClientBlackModule: TListView;
    RadioGroupModule: TRadioGroup;
    CheckBoxClientAddModule: TCheckBox;
    RadioButtonWhiteModule: TRadioButton;
    RadioButtonBlackModule: TRadioButton;
    procedure ButtonModuleAddClick(Sender: TObject);
    procedure ButtonModuleSaveClick(Sender: TObject);
    procedure ListViewClientModuleClick(Sender: TObject);
    procedure ButtonModuleDelClick(Sender: TObject);
    procedure ButtonLoadClick(Sender: TObject);
    procedure CheckBoxGetCheckModuleClick(Sender: TObject);
    procedure CheckBoxClientAddModuleClick(Sender: TObject);
    procedure ListViewClientBlackModuleClick(Sender: TObject);
    procedure RadioGroupModuleClick(Sender: TObject);
  private
    boOpened: Boolean;
    procedure RefModuleList;
  public
    procedure Open;
  end;

var
  ftmClientModules: TftmClientModules;

implementation

uses
  M2Share;
{$R *.dfm}

var
  SelModuleInfo: pTModuleInfo = nil;

procedure TftmClientModules.RefModuleList;
var
  I: Integer;
  ModuleInfo: pTModuleInfo;
  ListItem: TListItem;
begin
  ListViewClientModule.Clear;
  for I := 0 to g_ModuleList.Count - 1 do
  begin
    ModuleInfo := g_ModuleList.Items[I];
    ListItem := ListViewClientModule.Items.Add;
    ListItem.Caption := IntToStr(I);
    ListItem.Data := ModuleInfo;
    if ModuleInfo.boMode then
      ListItem.SubItems.AddObject('远程添加', TObject(ModuleInfo))
    else
      ListItem.SubItems.AddObject('本地添加', TObject(ModuleInfo));
    ListItem.SubItems.AddObject(ModuleInfo.sMD5, TObject(ModuleInfo));
    ListItem.SubItems.Add(ModuleInfo.sFileName);
  end;

  ListViewClientBlackModule.Clear;
  for I := 0 to g_BlackModuleList.Count - 1 do
  begin
    ModuleInfo := g_BlackModuleList.Items[I];
    ListItem := ListViewClientBlackModule.Items.Add;
    ListItem.Caption := IntToStr(I);
    ListItem.Data := ModuleInfo;
    if ModuleInfo.boMode then
      ListItem.SubItems.AddObject('远程添加', TObject(ModuleInfo))
    else
      ListItem.SubItems.AddObject('本地添加', TObject(ModuleInfo));
    ListItem.SubItems.AddObject(ModuleInfo.sMD5, TObject(ModuleInfo));
    ListItem.SubItems.Add(ModuleInfo.sFileName);
  end;
end;

procedure TftmClientModules.Open;
begin
  boOpened := False;
  CheckBoxGetCheckModule.Checked := g_Config.boClientCheckModule;
  CheckBoxClientAddModule.Checked := g_Config.boClientAddModule;
  if g_Config.boAddModuleList then
    RadioGroupModule.ItemIndex := 1
  else
    RadioGroupModule.ItemIndex := 0;

  ButtonModuleSave.Enabled := False;
  ButtonModuleDel.Enabled := False;
  SelModuleInfo := nil;
  RefModuleList;
  boOpened := True;
  PageControl.ActivePageIndex := 0;
  ShowModal;
end;

procedure TftmClientModules.ButtonModuleAddClick(Sender: TObject);
var
  I: Integer;
  ModuleInfo: pTModuleInfo;
  sMD5: string;
  sFileName: string;
begin
  sMD5 := LowerCase(Trim(EditMD5.Text));
  sFileName := Trim(EditFileName.Text);
  if Length(sMD5) <> 32 then
  begin
    Application.MessageBox('请输入正确的MD5值！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;

  if RadioButtonWhiteModule.Checked then
  begin
    for I := 0 to g_ModuleList.Count - 1 do
    begin
      ModuleInfo := g_ModuleList.Items[I];
      if ModuleInfo.sMD5 = sMD5 then
      begin
        Application.MessageBox('你输入的MD5值已经在白名单！', '错误信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    for I := g_BlackModuleList.Count - 1 downto 0 do
    begin
      ModuleInfo := g_BlackModuleList.Items[I];
      if ModuleInfo.sMD5 = sMD5 then
      begin
        g_BlackModuleList.Delete(I);
        Dispose(ModuleInfo);
      end;
    end;
  end
  else
  begin
    for I := 0 to g_BlackModuleList.Count - 1 do
    begin
      ModuleInfo := g_BlackModuleList.Items[I];
      if ModuleInfo.sMD5 = sMD5 then
      begin
        Application.MessageBox('你输入的MD5值已经在黑名单！', '错误信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    for I := g_ModuleList.Count - 1 downto 0 do
    begin
      ModuleInfo := g_ModuleList.Items[I];
      if ModuleInfo.sMD5 = sMD5 then
      begin
        g_ModuleList.Delete(I);
        Dispose(ModuleInfo);
      end;
    end;
  end;

  New(ModuleInfo);
  ModuleInfo.sMD5 := sMD5;
  ModuleInfo.sFileName := sFileName;
  if RadioButtonWhiteModule.Checked then
  begin
    g_ModuleList.Add(ModuleInfo);
  end
  else
  begin
    g_BlackModuleList.Add(ModuleInfo);
  end;
  RefModuleList;
  ButtonModuleSave.Enabled := True;
end;

procedure TftmClientModules.ButtonModuleSaveClick(Sender: TObject);
begin
  SaveClientModules();
  SaveClientBlackModules();
  ButtonModuleSave.Enabled := False;
end;

procedure TftmClientModules.ListViewClientModuleClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := ListViewClientModule.Selected;
  if ListItem = nil then
  begin
    ButtonModuleDel.Enabled := False;
    SelModuleInfo := nil;
    Exit;
  end;
  SelModuleInfo := pTModuleInfo(ListItem.SubItems.Objects[0]);
  EditFileName.Text := SelModuleInfo.sFileName;
  EditMD5.Text := SelModuleInfo.sMD5;
  ButtonModuleDel.Enabled := SelModuleInfo <> nil;
  RadioButtonWhiteModule.Checked := True;
end;

procedure TftmClientModules.ButtonModuleDelClick(Sender: TObject);
var
  I: Integer;
  boDelete: Boolean;
begin
  boDelete := False;
  if SelModuleInfo <> nil then
  begin
    for I := 0 to g_ModuleList.Count - 1 do
    begin
      if SelModuleInfo = g_ModuleList.Items[I] then
      begin
        SelModuleInfo := nil;
        Dispose(pTModuleInfo(g_ModuleList.Items[I]));
        g_ModuleList.Delete(I);
        boDelete := True;
        break;
      end;
    end;

    for I := 0 to g_BlackModuleList.Count - 1 do
    begin
      if SelModuleInfo = g_BlackModuleList.Items[I] then
      begin
        SelModuleInfo := nil;
        Dispose(pTModuleInfo(g_BlackModuleList.Items[I]));
        g_BlackModuleList.Delete(I);
        boDelete := True;
        break;
      end;
    end;
  end;
  if boDelete then
  begin
    RefModuleList;
    ButtonModuleSave.Enabled := True;
  end;
  ButtonModuleDel.Enabled := False;
end;

procedure TftmClientModules.ButtonLoadClick(Sender: TObject);
begin
  LoadClientModules();
  LoadClientBlackModules();
  ButtonModuleSave.Enabled := False;
  ButtonModuleDel.Enabled := False;
  SelModuleInfo := nil;
  RefModuleList;
end;

procedure TftmClientModules.CheckBoxGetCheckModuleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boClientCheckModule := CheckBoxGetCheckModule.Checked;
  Config.WriteBool('Setup', 'ClientCheckModule', g_Config.boClientCheckModule);
  UserEngine.SendClientModules();
end;

procedure TftmClientModules.CheckBoxClientAddModuleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boClientAddModule := CheckBoxClientAddModule.Checked;
  Config.WriteBool('Setup', 'ClientAddModule', g_Config.boClientAddModule);
  UserEngine.SendClientModules();
end;

procedure TftmClientModules.ListViewClientBlackModuleClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := ListViewClientBlackModule.Selected;
  if ListItem = nil then
  begin
    ButtonModuleDel.Enabled := False;
    SelModuleInfo := nil;
    Exit;
  end;
  SelModuleInfo := pTModuleInfo(ListItem.SubItems.Objects[0]);
  EditFileName.Text := SelModuleInfo.sFileName;
  EditMD5.Text := SelModuleInfo.sMD5;
  ButtonModuleDel.Enabled := SelModuleInfo <> nil;
  RadioButtonBlackModule.Checked := True;
end;

procedure TftmClientModules.RadioGroupModuleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAddModuleList := RadioGroupModule.ItemIndex = 1;
  Config.WriteBool('Setup', 'AddModuleList', g_Config.boAddModuleList);
end;

end.

