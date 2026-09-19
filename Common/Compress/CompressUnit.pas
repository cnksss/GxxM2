unit CompressUnit;

interface
uses ZLibEx, RLEUnit;

function CompressBuffer(CompType: Integer; InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer): Integer;
procedure DecompressBuffer(CompType: Integer; InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer);
implementation

function CompressBuffer(CompType: Integer; InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer): Integer;
begin
  Result := Width * Height * BytesPerPixel;
  case CompType of
    1: Result := EncodeRLE(InData, OutData, Width, Height, BytesPerPixel);
    2: CompressBufZ(InData, Width * Height * BytesPerPixel, OutData, Result);
  else
    Move(InData^, OutData^, Width * Height * BytesPerPixel);
  end;
end;

procedure DecompressBuffer(CompType: Integer; InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer);
var
  OutBytes: Integer;
begin
  case CompType of
    1: DecodeRLE(InData, OutData, Width, Height, BytesPerPixel);
    2: DecompressBufZ(InData, Width * Height * BytesPerPixel, 0, OutData, OutBytes);
  else
    Move(InData^, OutData^, Width * Height * BytesPerPixel);
  end;
end;

end.
