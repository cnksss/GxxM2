unit LoginDlg;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Mask, RzEdit, RzBtnEdt, ExtCtrls, ComCtrls, ShlObj, ComObj,
  ActiveX, RzPanel, RzDlgBtn, RzRadGrp, DxComponents;

type
  TFrmLogin = class(TForm)
    DialogButtons: TRzDialogButtons;
    Label1: TLabel;
    EditGamePath: TRzButtonEdit;
    RadioGroup: TRzRadioGroup;
    CheckBoxD3DFormat: TCheckBox;
    procedure EditGamePathButtonClick(Sender: TObject);
    procedure DialogButtonsClickOk(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure DialogButtonsClickCancel(Sender: TObject);
    procedure RadioGroupClick(Sender: TObject);
    procedure CheckBoxD3DFormatClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  FrmLogin: TFrmLogin;

implementation
uses GameImages, IniFiles, Share;
{$R *.dfm}
//_____________________________________________________________________//

function SelectDirCB(Wnd: Hwnd; uMsg: UINT; LPARAM, lpData: LPARAM): Integer stdcall;
begin
  if (uMsg = BFFM_INITIALIZED) and (lpData <> 0) then
    SendMessage(Wnd, BFFM_SETSELECTION, Integer(True), lpData);
  Result := 0;
end;

function SelectDirectory(const Caption: string; const Root: WideString;
  var Directory: string; Owner: THandle): Boolean;
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
        ulFlags := BIF_RETURNONLYFSDIRS;
        if Directory <> '' then begin
          lpfn := SelectDirCB;
          LPARAM := Integer(PChar(Directory));
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

procedure TFrmLogin.EditGamePathButtonClick(Sender: TObject);
begin
  EditGamePath.Text := g_sMirDataDirectory;
  if not SelectDirectory('请选择传奇客户端“Legend of mir2”目录', '', g_sMirDataDirectory, Handle) then begin
    //ModalResult := 0;
    Close;
    ModalResult := mrNo;
    Exit;
  end;
  EditGamePath.Text := g_sMirDataDirectory;
end;

procedure TFrmLogin.DialogButtonsClickOk(Sender: TObject);
var
  I: Integer;
  IniFile: TIniFile;
  FileNameList: TStringList;
  sFileName: string;
begin
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');
  g_sMirDataDirectory := Trim(EditGamePath.Text);
  //IniFile.WriteString('Setup', 'Directory', g_sMirDataDirectory);
  IniFile.WriteInteger('Setup', 'ClientVersion', Integer(g_ClientVersion));
  IniFile.WriteBool('Setup', 'D3DFormat', g_boD3DFormat);
  FileNameList := TStringList.Create;
  IniFile.ReadSection('FileNames', FileNameList);
  for I := 0 to FileNameList.Count - 1 do begin
    sFileName := IniFile.ReadString('FileNames', FileNameList.Strings[I], '');
    if sFileName <> '' then
      g_FileNameList.AddObject(sFileName, TObject(I));
  end;
  FileNameList.Free;

  g_MirDataDirectoryList[g_ClientVersion] := g_sMirDataDirectory;
  for I := 0 to Length(g_MirDataDirectoryList) - 1 do begin
    IniFile.WriteString('Directory', IntToStr(I), g_MirDataDirectoryList[TClientVersion(I)]);
  end;

  IniFile.Free;
  //ModalResult := mrYes;
  Close;
  ModalResult := mrYes;
end;

procedure TFrmLogin.FormCreate(Sender: TObject);
var
  I: Integer;
  IniFile: TIniFile;
begin
  ModalResult := mrNo;
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');
  //g_sMirDataDirectory := IniFile.ReadString('Setup', 'Directory', g_sMirDataDirectory);
  g_ClientVersion := TClientVersion(IniFile.ReadInteger('Setup', 'ClientVersion', Integer(g_ClientVersion)));
  g_boD3DFormat := IniFile.ReadBool('Setup', 'D3DFormat', g_boD3DFormat);
  for I := 0 to Length(g_MirDataDirectoryList) - 1 do begin
    g_MirDataDirectoryList[TClientVersion(I)] := IniFile.ReadString('Directory', IntToStr(I), g_sMirDataDirectory);
  end;
  g_sMirDataDirectory := g_MirDataDirectoryList[g_ClientVersion];
  EditGamePath.Text := g_sMirDataDirectory;
  RadioGroup.ItemIndex := Integer(g_ClientVersion);
  CheckBoxD3DFormat.Checked := g_boD3DFormat;
  IniFile.Free;
end;

procedure TFrmLogin.DialogButtonsClickCancel(Sender: TObject);
begin
  Close;
  ModalResult := mrNo;
end;

procedure TFrmLogin.RadioGroupClick(Sender: TObject);
begin
  g_ClientVersion := TClientVersion(RadioGroup.ItemIndex);
  EditGamePath.Text := g_MirDataDirectoryList[g_ClientVersion];
end;

procedure TFrmLogin.CheckBoxD3DFormatClick(Sender: TObject);
begin
  g_boD3DFormat := CheckBoxD3DFormat.Checked;
end;

end.

