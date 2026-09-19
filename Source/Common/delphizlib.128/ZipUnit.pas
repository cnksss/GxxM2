unit ZipUnit;

interface
uses SysUtils, Classes, ZLibEx;

procedure ZipCompressBuffer(const InBuf: Pointer; InBytes: Integer;
  out OutBuf: Pointer; out OutBytes: Integer; Level: ZLibEx.TZCompressionLevel = zcDefault);

procedure ZipDecompressBuffer(const InBuf: Pointer; InBytes: Integer;
  out OutBuf: Pointer; out OutBytes: Integer);

procedure ZipCompressStream(const InStream, OutStream: TStream; Level: ZLibEx.TZCompressionLevel = zcDefault; OnProgress: TNotifyEvent = nil);
procedure ZipDecompressStream(const InStream, OutStream: TStream; OnProgress: TNotifyEvent = nil);
implementation

procedure ZipDecompressStream(const InStream, OutStream: TStream; OnProgress: TNotifyEvent);
var
  zStream: TZDecompressionStream;
begin
  zStream := TZDecompressionStream.Create(InStream);
  try
    zStream.OnProgress := OnProgress;
    OutStream.CopyFrom(zStream, 0);
  finally
    zStream.Free;
  end;
end;

procedure ZipCompressStream(const InStream, OutStream: TStream; Level: ZLibEx.TZCompressionLevel; OnProgress: TNotifyEvent);
var
  zStream: TZCompressionStream;
begin
  zStream := TZCompressionStream.Create(OutStream, Level);
  try
    zStream.OnProgress := OnProgress;
    zStream.CopyFrom(InStream, InStream.Size);
  finally
    zStream.Free;
  end;
end;

procedure ZipCompressBuffer(const InBuf: Pointer; InBytes: Integer;
  out OutBuf: Pointer; out OutBytes: Integer; Level: TZCompressionLevel);
begin
  ZCompress(InBuf, InBytes, OutBuf, OutBytes, Level);
end;

procedure ZipDecompressBuffer(const InBuf: Pointer; InBytes: Integer;
  out OutBuf: Pointer; out OutBytes: Integer);
begin
  ZDecompress(InBuf, InBytes, OutBuf, OutBytes, 0);
end;

end.
