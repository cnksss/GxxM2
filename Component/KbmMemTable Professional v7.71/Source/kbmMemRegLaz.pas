unit kbmMemRegLaz;

{$mode objfpc}{$H+}

interface

uses
  Classes,
  SysUtils,
  LResources,
  kbmMemTable,
  kbmMemCSVStreamFormat,
  kbmMemBinaryStreamFormat;

procedure Register;

implementation

procedure Register;
begin
     RegisterComponents('kbmMemTable', [TkbmMemTable,TkbmBinaryStreamFormat,TkbmCSVStreamFormat]);
end;

initialization
  {$I kbmMemTable.lrs}

end.

