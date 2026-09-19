unit FilterTexts;

interface

uses
  Windows, Classes, SysUtils;

type
  TFilterMsg = record
    Msg: string;
    Replace: string;
  end;

  pTFilterMsg = ^TFilterMsg;

  TFilterTexts = class
  private
    FRecordCount: Integer;
    FList: TList;
    function TrimAll(Text: string): string;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTFilterMsg;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadFromFile;
    procedure SaveToFile;
    function Filter(sMsg: string; var sNewMsg: string): Boolean;
    function Find(sMsg: string): Boolean;
    function Add(sMsg, sReplaceMsg: string): Boolean;
    function Delete(sMsg: string): Boolean;
    property Items[Index: Integer]: pTFilterMsg read GetItems;
    property Count: Integer read GetCount;
    property RecordCount: Integer read FRecordCount;
  end;

implementation

uses
  StrUtils, M2Share, HUtil32;

const
  TextChars = [#32 .. #255];

function GetWebSubAddr(sDomainName: string; var sName: string): string;
begin
  Result := sDomainName;
  // sAddr := GetWebAddr(sAddr);
  if Pos('.', sDomainName) > 0 then
    Result := Copy(sDomainName, Pos('.', sDomainName) + 1, Length(sDomainName));
  sName := Copy(Result, 1, Pos('.', Result) - 1);
  // Result := sAddr;
end;

constructor TFilterTexts.Create();
begin
  FList := TList.Create;
  FRecordCount := 0;
end;

destructor TFilterTexts.Destroy;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(pTFilterMsg(FList.Items[I]));
  end;
  FList.Free;
  inherited;
end;

function TFilterTexts.TrimAll(Text: string): string;
var
  I: Integer;
begin
  Text := Trim(Text);
  for I := Length(Text) downto 1 do
  begin
{$IF CompilerVersion >= 22}
    if (not CharInSet(Text[I], TextChars)) then
      SysTem.Delete(Text, I, 1);
{$ELSE}
    if (not(Text[I] in TextChars)) then
      SysTem.Delete(Text, I, 1);
{$IFEND}
  end;
  Result := Text;
end;

function TFilterTexts.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TFilterTexts.GetItems(Index: Integer): pTFilterMsg;
begin
  Result := FList.Items[Index];
end;

function TFilterTexts.Find(sMsg: string): Boolean;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
begin
  Result := False;
  sMsg := TrimAll(sMsg);
  for I := 0 to FList.Count - 1 do
  begin
    FilterMsg := pTFilterMsg(FList.Items[I]);
    if (CompareText(sMsg, FilterMsg.Msg) = 0) then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TFilterTexts.Filter(sMsg: string; var sNewMsg: string): Boolean;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
  IsSameText: Boolean;
begin
  Result := False;
  sNewMsg := sMsg;
  // try
  sNewMsg := TrimAll(sNewMsg);
  if CanFilterMsg(sNewMsg) then
  begin
    if (CompareText(sNewMsg, '$') = 0) or AnsiContainsText(sNewMsg, '$') then
    begin
      Result := True;
      if (Length(sNewMsg) = 1) then
      begin
        sNewMsg := '';
      end
      else
      begin
        sNewMsg := AnsiReplaceText(sNewMsg, '$', '');
      end;
      Exit;
    end;

    for I := 0 to FList.Count - 1 do
    begin
      FilterMsg := pTFilterMsg(FList.Items[I]);
      IsSameText := SameText(sNewMsg, FilterMsg.Msg);
      if IsSameText or AnsiContainsText(sNewMsg, FilterMsg.Msg) then
      begin
        Result := True;
        if FilterMsg.Replace = '' then
        begin
          sNewMsg := '';
        end
        else
        begin
          if IsSameText then
          begin
            sNewMsg := FilterMsg.Replace;
          end
          else
          begin
            sNewMsg := AnsiReplaceText(sNewMsg, FilterMsg.Msg, FilterMsg.Replace);
          end;
        end;
        if sNewMsg = '' then
          Break;
        // Break;
      end;
    end;
  end;
  // except
  // MainOutMessage('TFilterTexts.Filter:'+IntToStr(nError));
  // end;
end;

function TFilterTexts.Add(sMsg, sReplaceMsg: string): Boolean;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
begin
  Result := False;
  sMsg := TrimAll(sMsg);
  for I := 0 to FList.Count - 1 do
  begin
    FilterMsg := pTFilterMsg(FList.Items[I]);
    if (CompareText(sMsg, FilterMsg.Msg) = 0) then
    begin
      Exit;
    end;
  end;
  if CanFilterMsg(sMsg) then
  begin
    New(FilterMsg);
    FilterMsg.Msg := sMsg;
    FilterMsg.Replace := sReplaceMsg;
    FList.Add(FilterMsg);
    Inc(FRecordCount);
    Result := True;
  end;
end;

function TFilterTexts.Delete(sMsg: string): Boolean;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    FilterMsg := pTFilterMsg(FList.Items[I]);
    if (CompareText(sMsg, FilterMsg.Msg) = 0) then
    begin
      Dec(FRecordCount);
      FList.Delete(I);
      Dispose(FilterMsg);
      Result := True;
      Break;
    end;
  end;
end;

procedure TFilterTexts.LoadFromFile;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
  LoadList: TStringList;
  sFileName: string;
  sLineText: string;
  sMsg: string;
  sReplace: string;
begin
  sFileName := g_Config.sEnvirDir + 'FilterMsgList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sMsg, [#9]);
        sLineText := GetValidStr3(sLineText, sReplace, [#9]);
        if CanFilterMsg(sMsg) then
        begin
          New(FilterMsg);
          FilterMsg.Msg := sMsg;
          FilterMsg.Replace := sReplace;
          FList.Add(FilterMsg);
          Inc(FRecordCount);
        end;
      end;
    end;
    LoadList.Free;
  end;
end;

procedure TFilterTexts.SaveToFile;
var
  I: Integer;
  FilterMsg: pTFilterMsg;
  SaveList: TStringList;
  sFileName: string;
begin
  SaveList := TStringList.Create;
  for I := 0 to FList.Count - 1 do
  begin
    FilterMsg := pTFilterMsg(FList.Items[I]);
    SaveList.Add(FilterMsg.Msg + #9 + FilterMsg.Replace);
  end;
  sFileName := g_Config.sEnvirDir + 'FilterMsgList.txt';
  try
    SaveList.SaveToFile(sFileName);
  except

  end;
  SaveList.Free;
end;

end.
