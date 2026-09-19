unit kbmMemDesLazStd; 

interface

uses
  kbmMemRegLaz, LazarusPackageIntf;

implementation

procedure Register; 
begin
  RegisterUnit('kbmMemRegLaz',@kbmMemRegLaz.Register); 
end; 

initialization
  RegisterPackage('kbmMemDesLazStd',@Register); 
end.