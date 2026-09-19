unit StreamClipbrd;

interface
uses
  SysUtils, Classes, Windows, Clipbrd;
procedure CopyStreamToClipboard(fmt: Cardinal; S: TStream);
procedure CopyStreamFromClipboard(fmt: Cardinal; S: TStream);
procedure SaveClipboardFormat(fmt: Word; writer: TWriter);
procedure LoadClipboardFormat(reader: TReader);
procedure SaveClipboard(S: TStream);
procedure LoadClipboard(S: TStream);

procedure StreamSaveToClipboard(S: TStream);
procedure StreamLoadFromClipboard(S: TStream);
implementation

procedure CopyStreamToClipboard(fmt: Cardinal; S: TStream);
var
  hMem: THandle;
  pMem: Pointer;
begin
  Assert(Assigned(S));
  S.Position := 0;
  hMem := GlobalAlloc(GHND or GMEM_DDESHARE, S.Size);
  if hMem <> 0 then
  begin
    pMem := GlobalLock(hMem);
    if pMem <> nil then
    begin
      try
        S.Read(pMem^, S.Size);
        S.Position := 0;
      finally
        GlobalUnlock(hMem);
      end;
      Clipboard.Open;
      try
        Clipboard.SetAsHandle(fmt, hMem);
      finally
        Clipboard.Close;
      end;
    end { If }
    else
    begin
      GlobalFree(hMem);
      OutOfMemoryError;
    end;
  end { If }
  else
    OutOfMemoryError;
end; { CopyStreamToClipboard }

procedure CopyStreamFromClipboard(fmt: Cardinal; S: TStream);
var
  hMem: THandle;
  pMem: Pointer;
begin
  Assert(Assigned(S));
  hMem := Clipboard.GetAsHandle(fmt);
  if hMem <> 0 then
  begin
    pMem := GlobalLock(hMem);
    if pMem <> nil then
    begin
      try
        S.Write(pMem^, GlobalSize(hMem));
        S.Position := 0;
      finally
        GlobalUnlock(hMem);
      end;
    end { If }
    else
      raise Exception.Create('CopyStreamFromClipboard: could not lock global handle ' +
        'obtained from clipboard!');
  end; { If }
end; { CopyStreamFromClipboard }

procedure SaveClipboardFormat(fmt: Word; writer: TWriter);
var
  fmtname: array[0..128] of Char;
  ms: TMemoryStream;
begin
  Assert(Assigned(writer));
  if 0 = GetClipboardFormatName(fmt, fmtname, SizeOf(fmtname)) then
    fmtname[0] := #0;
  ms := TMemoryStream.Create;
  try
    CopyStreamFromClipboard(fmt, ms);
    if ms.Size > 0 then
    begin
      writer.WriteInteger(fmt);
      writer.WriteString(fmtname);
      writer.WriteInteger(ms.Size);
      writer.Write(ms.Memory^, ms.Size);
    end; { If }
  finally
    ms.Free
  end; { Finally }
end; { SaveClipboardFormat }

procedure LoadClipboardFormat(reader: TReader);
var
  fmt: Integer;
  fmtname: string;
  Size: Integer;
  ms: TMemoryStream;
begin
  Assert(Assigned(reader));
  fmt := reader.ReadInteger;
  fmtname := reader.ReadString;
  Size := reader.ReadInteger;
  ms := TMemoryStream.Create;
  try
    ms.Size := Size;
    reader.Read(ms.memory^, Size);
    if Length(fmtname) > 0 then
      fmt := RegisterCLipboardFormat(PChar(fmtname));
    if fmt <> 0 then
      CopyStreamToClipboard(fmt, ms);
  finally
    ms.Free;
  end; { Finally }
end; { LoadClipboardFormat }

procedure SaveClipboard(S: TStream);
var
  writer: TWriter;
  i: Integer;
begin
  Assert(Assigned(S));
  writer := TWriter.Create(S, 4096);
  try
    Clipboard.Open;
    try
      writer.WriteListBegin;
      for i := 0 to Clipboard.formatcount - 1 do
        SaveClipboardFormat(Clipboard.Formats[i], writer);
      writer.WriteListEnd;
    finally
      Clipboard.Close;
    end; { Finally }
  finally
    writer.Free
  end; { Finally }
end; { SaveClipboard }

procedure LoadClipboard(S: TStream);
var
  reader: TReader;
begin
  Assert(Assigned(S));
  reader := TReader.Create(S, 4096);
  try
    Clipboard.Open;
    try
      clipboard.Clear;
      reader.ReadListBegin;
      while not reader.EndOfList do
        LoadClipboardFormat(reader);
      reader.ReadListEnd;
    finally
      Clipboard.Close;
    end; { Finally }
  finally
    reader.Free
  end; { Finally }
end; { LoadClipboard }


const
  CF_MYFORMAT = 55555;
//将内容写入剪贴板中

procedure StreamSaveToClipboard(S: TStream);
var
  hbuf: THandle;
  bufptr: Pointer;
begin
  hbuf := GlobalAlloc(GMEM_MOVEABLE, S.Size);
  try
    bufptr := GlobalLock(hbuf);
    try
      //Move(mstream.Memory^, bufptr^, S.Size);
      S.Read(bufptr^, S.Size);
      Clipboard.SetAsHandle(CF_MYFORMAT, hbuf);
    finally
      GlobalUnlock(hbuf);
    end;
  except
    GlobalFree(hbuf);
    raise;
  end;
end;

//请注意不要将分配的全局缓冲区释放，这个工作由剪贴板来完成，在读出数据中
//你应该将它复制后处理。

//将剪贴板内容读出来

procedure StreamLoadFromClipboard(S: TStream);
var
  hbuf: THandle;
  bufptr: Pointer;
begin
  hbuf := Clipboard.GetAsHandle(CF_MYFORMAT);
  if hbuf <> 0 then begin
    bufptr := GlobalLock(hbuf);
    if bufptr <> nil then begin
      try
        S.WriteBuffer(bufptr^, GlobalSize(hbuf));
        S.Position := 0;
      finally
        GlobalUnlock(hbuf);
      end;
    end;
  end;
end;

end.
