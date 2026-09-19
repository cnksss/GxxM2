{
  // 修复StringList读取无bom表的Utf8文件时乱码
}

unit StringListHelper;

interface

uses
  System.SysUtils, System.Classes, EncodingHelper;

type
  TStringListHelper = class helper for TStringList
    procedure LoadFromFile(const FileName: string); overload; virtual;
    procedure LoadFromStream(Stream: TStream; Encoding: TEncoding); overload; virtual;
  end;

implementation

{ TStringListHelper }

procedure TStringListHelper.LoadFromFile(const FileName: string);
var
  Stream: TStream;
begin
  Stream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyWrite);
  try
    LoadFromStream(Stream, nil);
  finally
    Stream.Free;
  end;
end;

procedure TStringListHelper.LoadFromStream(Stream: TStream;
  Encoding: TEncoding);
var
  Size: Integer;
  Buffer: TBytes;
begin
  BeginUpdate;
  try
    Size := Stream.Size - Stream.Position;
    SetLength(Buffer, Size);
    Stream.Read(Buffer, 0, Size);
    Size := TEncoding.GetBufferEncoding(Buffer, Encoding, DefaultEncoding);
    SetEncoding(Encoding); // Keep Encoding in case the stream is saved
    SetTextStr(Encoding.GetString(Buffer, Size, Length(Buffer) - Size));
  finally
    EndUpdate;
  end;
end;

end.
