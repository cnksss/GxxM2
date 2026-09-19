unit UserCmds;

interface

uses
  Windows, Classes, SysUtils, ObjPlayer;

type
  TUserCmds = class
  private
    FRecordCount: Integer;
    FCmdList: TStringList;

    function GetCount: Integer;
    function GetObjects(Index: Integer): TObject;
    function GetStrings(Index: Integer): string;
  public
    constructor Create();
    destructor Destroy; override;

    procedure LoadFromFile;
    procedure SaveToFile;
    function Get(sCmd: string): Integer;
    function Find(sCmd: string): Boolean; overload;
    function Find(nIndex: Integer): Boolean; overload;
    function Add(sCmd: string; nIndex: Integer): Boolean;
    function Delete(sCmd: string): Boolean; overload;
    function Delete(nIndex: Integer): Boolean; overload;
    procedure GotoLable(PlayObject: TPlayObject; Index: Integer); overload;
    function GotoLable(PlayObject: TPlayObject; sCmd: string): Boolean; overload;

    property Strings[Index: Integer]: string read GetStrings;
    property Objects[Index: Integer]: TObject read GetObjects;
    property Count: Integer read GetCount;
    property RecordCount: Integer read FRecordCount;
  end;

implementation

uses
  M2Share, HUtil32;

constructor TUserCmds.Create();
begin
  FCmdList := TStringList.Create;
  FRecordCount := 0;
end;

destructor TUserCmds.Destroy;
begin
  FCmdList.Free;
  inherited;
end;

function TUserCmds.GetCount: Integer;
begin
  Result := FCmdList.Count;
end;

function TUserCmds.GetObjects(Index: Integer): TObject;
begin
  Result := FCmdList.Objects[Index];
end;

function TUserCmds.GetStrings(Index: Integer): string;
begin
  Result := FCmdList.Strings[Index];
end;

procedure TUserCmds.LoadFromFile;
var
  I: Integer;
  sFileName: string;
  sLineText: string;
  sCmdName: string;
  sIndex: string;
  nIndex: Integer;
  LoadList: TStringList;
begin
  FRecordCount := 0;
  FCmdList.Clear;
  sFileName := g_Config.sEnvirDir + 'UserCmd.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := LoadList.Strings[I];
        if (sLineText <> '') and (sLineText[1] <> ';') then
        begin
          sLineText := GetValidStr3(sLineText, sCmdName, [' ', #9]);
          sLineText := GetValidStr3(sLineText, sIndex, [' ', #9]);
          nIndex := StrToIntDef(sIndex, -1);
          if (sCmdName <> '') and (nIndex >= 0) then
          begin
            Inc(FRecordCount);
            FCmdList.AddObject(sCmdName, TObject(nIndex));
          end;
        end;
      end;
    finally
      LoadList.Free;
    end;
  end;
end;

procedure TUserCmds.SaveToFile;
var
  I: Integer;
  sFileName: string;
  SaveList: TStringList;
begin
  SaveList := TStringList.Create;
  for I := 0 to FCmdList.Count - 1 do
  begin
    SaveList.Add(FCmdList.Strings[I] + #9 + IntToStr(Integer(FCmdList.Objects[I])));
  end;
  sFileName := g_Config.sEnvirDir + 'UserCmd.txt';
  try
    SaveList.SaveToFile(sFileName);
  finally
    SaveList.Free;
  end;

end;

function TUserCmds.Add(sCmd: string; nIndex: Integer): Boolean;
begin
  FCmdList.AddObject(sCmd, TObject(nIndex));
  Inc(FRecordCount);
  Result := True;
end;

function TUserCmds.Delete(sCmd: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FCmdList.Count - 1 do
  begin
    if (CompareText(sCmd, FCmdList.Strings[I]) = 0) then
    begin
      FCmdList.Delete(I);
      Dec(FRecordCount);
      Result := True;
      Break;
    end;
  end;
end;

function TUserCmds.Delete(nIndex: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FCmdList.Count - 1 do
  begin
    if nIndex = Integer(FCmdList.Objects[I]) then
    begin
      FCmdList.Delete(I);
      Dec(FRecordCount);
      Result := True;
      Break;
    end;
  end;
end;

function TUserCmds.Get(sCmd: string): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to FCmdList.Count - 1 do
  begin
    if CompareText(sCmd, FCmdList.Strings[I]) = 0 then
    begin
      Result := Integer(FCmdList.Objects[I]);
      Break;
    end;
  end;
end;

function TUserCmds.Find(sCmd: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FCmdList.Count - 1 do
  begin
    if CompareText(sCmd, FCmdList.Strings[I]) = 0 then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TUserCmds.Find(nIndex: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FCmdList.Count - 1 do
  begin
    if nIndex = Integer(FCmdList.Objects[I]) then
    begin
      Result := True;
      Break;
    end;
  end;
end;

procedure TUserCmds.GotoLable(PlayObject: TPlayObject; Index: Integer);
begin
  if (Index >= 0) and (g_FunctionNPC <> nil) then
  begin
    PlayObject.m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(PlayObject, '@UserCmd' + IntToStr(Index), False);
  end;
end;

function TUserCmds.GotoLable(PlayObject: TPlayObject; sCmd: string): Boolean;
var
  Index: Integer;
begin
  Result := False;
  Index := Get(sCmd);
  if (Index >= 0) and (g_FunctionNPC <> nil) then
  begin
    PlayObject.m_nScriptGotoCount := 0;
    g_FunctionNPC.GotoLable(PlayObject, '@UserCmd' + IntToStr(Index), False);
    Result := True;
  end;
end;

end.
