unit uFrmClientPlugManager;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, EDCode, CheckUnit;

type
  TFrmClientPlugManager = class(TForm)
    ListBoxPlugin: TListBox;
    ButtonRef: TButton;
    procedure ButtonRefClick(Sender: TObject);
  private
    { Private declarations }
  public
    procedure Open;
  end;

procedure LoadPlugClientFiles();

implementation

uses M2Share, MD5Util;
{$R *.dfm}

procedure LoadPlugClientFiles();
var
  I: Integer;
  sFileName, S: string;
  PlugClientInfo: pTPlugClientInfo;

  procedure SearchFiles(SearchDirectory: string);
  var
    Info: TsearchRec;
    nFileCount: Integer;

    function IsDir: Boolean;
    begin
      with Info do
        Result := (Name <> '.') and (Name <> '..') and ((Attr and faDirectory) = faDirectory);
    end;

    function IsFile: Boolean;
    begin
      Result := (not((Info.Attr and faDirectory) = faDirectory)) and (Info.Name <> '.') and (Info.Name <> '..');
    end;

  begin
    nFileCount := 0;
    SearchDirectory := IncludeTrailingPathDelimiter(SearchDirectory);
    try
      if FindFirst(SearchDirectory + '*.dll', faAnyFile, Info) = 0 then
      begin
        if IsFile then
        begin
          New(PlugClientInfo);
          PlugClientInfo.sFileName := SearchDirectory + Info.Name;
          PlugClientInfo.sMD5 := RivestFile(PlugClientInfo.sFileName);
          g_PlugClientList.Add(PlugClientInfo);
        end;
      end;

      while (FindNext(Info) = 0) and (not Application.Terminated) do
      begin
        Inc(nFileCount);
        if nFileCount mod 100 = 0 then
        begin
          Application.ProcessMessages;
          // Sleep(1);
        end;

        if IsFile then
        begin
          New(PlugClientInfo);
          PlugClientInfo.sFileName := SearchDirectory + Info.Name;
          PlugClientInfo.sMD5 := RivestFile(PlugClientInfo.sFileName);
          g_PlugClientList.Add(PlugClientInfo);
        end;
      end;
    finally
      FindClose(Info);
    end;
  end;

begin
  for I := 0 to g_PlugClientList.Count - 1 do
  begin
    Dispose(pTPlugClientInfo(g_PlugClientList.Items[I]));
  end;
  g_PlugClientList.Clear;
  g_PlugFileMD5ListText := '';
  g_PlugFileMD5ListTextLen := 0;
  g_PlugFileMD5ListTextCRC := 0;

  sFileName := ExtractFilePath(Application.ExeName) + 'PlugClient';
  if not DirectoryExists(sFileName) then
  begin
    CreateDir(sFileName);
  end;
  SearchFiles(sFileName);

  S := '';
  for I := 0 to g_PlugClientList.Count - 1 do
  begin
    PlugClientInfo := g_PlugClientList.Items[I];

    if I = g_PlugClientList.Count - 1 then
      S := S + PlugClientInfo.sMD5
    else
      S := S + PlugClientInfo.sMD5 + sLineBreak;;
  end;

  if Length(S) > 0 then
  begin
    g_PlugFileMD5ListTextLen := Length(S);
    g_PlugFileMD5ListText := zEncodeString(S);
    g_PlugFileMD5ListTextCRC := BufferCrc(PAnsiChar(g_PlugFileMD5ListText), Length(g_PlugFileMD5ListText));
  end;
end;

procedure TFrmClientPlugManager.Open;
var
  I: Integer;
  PlugClientInfo: pTPlugClientInfo;
begin
  ListBoxPlugin.Clear;
  for I := 0 to g_PlugClientList.Count - 1 do
  begin
    PlugClientInfo := g_PlugClientList.Items[I];
    ListBoxPlugin.Items.AddObject(PlugClientInfo.sFileName, TObject(PlugClientInfo));
  end;
  ShowModal;
end;

procedure TFrmClientPlugManager.ButtonRefClick(Sender: TObject);
var
  I: Integer;
  PlugClientInfo: pTPlugClientInfo;
begin
  LoadPlugClientFiles();
  ListBoxPlugin.Clear;
  for I := 0 to g_PlugClientList.Count - 1 do
  begin
    PlugClientInfo := g_PlugClientList.Items[I];
    ListBoxPlugin.Items.AddObject(PlugClientInfo.sFileName, TObject(PlugClientInfo));
  end;
  UserEngine.SendPlugClientList();
end;

end.
