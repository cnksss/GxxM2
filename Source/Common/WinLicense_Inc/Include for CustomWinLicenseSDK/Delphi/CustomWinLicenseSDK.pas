unit CustomWinlicenseSDK;

{$ALIGN ON}
{$MINENUMSIZE 4}

interface

uses
  Windows;

type  
sLicenseFeatures = record 

     cb: Integer; 
     NumDays: Integer; 
     NumExec: Integer; 
     ExpDate: SYSTEMTIME; 
     CountryId: Integer; 
     Runtime: Integer; 
     GlobalMinutes: Integer; 
     InstallDate: SYSTEMTIME; 
     NetInstances: Integer; 
     EmbedLicenseInfoInKey: Integer; 
     EmbedCreationDate: Integer 

end; 
 
function WLCustomGenTrialExtensionFileKey(TrialHash:PChar; Level:Integer; NumDays:Integer; NumExec:Integer; NewDate:Pointer; NumMinutes:Integer; TimeRuntime:Integer; BufferOut:PChar):Integer; stdcall;
function WLCustomGenTrialExtensionRegistryKey(TrialHash:PChar; Level:Integer; NumDays:Integer; NumExec:Integer; NewDate:Pointer; NumMinutes:Integer; TimeRuntime:Integer; pKeyName:PChar; pKeyValueName:PChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenPassword(PasswordHash:PChar; Name:PChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseFileKey(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseFileKeyW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseFileKeyEx(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseFileKeyExW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseRegistryKey(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; KeyName:PChar; KeyValueName:PChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseRegistryKeyW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; KeyName:PWideChar; KeyValueName:PWideChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseRegistryKeyEx(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; var LicenseFeatures: sLicenseFeatures; KeyName:PChar; KeyValueName:PChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseRegistryKeyExW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; var LicenseFeatures: sLicenseFeatures; KeyName:PWideChar; KeyValueName:PWideChar; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseTextKey(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; BufferOut:PWideChar):Integer; stdcall;
function WLCustomGenLicenseTextKeyW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; CountryId:Integer; Runtime:Integer; GlobalMinutes:Integer; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseTextKeyEx(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseTextKeyExW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PWideChar):Integer; stdcall;
function WLCustomGenLicenseSmartKey(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseSmartKeyW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; NumDays:Integer; NumExec:Integer; NewDate:Pointer; BufferOut:PWideChar):Integer; stdcall;
function WLCustomGenLicenseDynSmartKey(LicenseHash:PChar; UserName:PChar; Organization:PChar; CustomData:PChar; MachineID:PChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PChar):Integer; stdcall;
function WLCustomGenLicenseDynSmartKeyW(LicenseHash:PWideChar; UserName:PWideChar; Organization:PWideChar; CustomData:PWideChar; MachineID:PWideChar; var LicenseFeatures: sLicenseFeatures; BufferOut:PWideChar):Integer; stdcall;

implementation

const
  WinlicenseDLL = 'CustomWinlicenseSDK.DLL';

function WLCustomGenTrialExtensionFileKey; external WinlicenseDLL name 'WLCustomGenTrialExtensionFileKey';
function WLCustomGenTrialExtensionRegistryKey; external WinlicenseDLL name 'WLCustomGenTrialExtensionRegistryKey';
function WLCustomGenPassword; external WinlicenseDLL name 'WLCustomGenPassword';
function WLCustomGenLicenseFileKey; external WinlicenseDLL name 'WLCustomGenLicenseFileKey';
function WLCustomGenLicenseFileKeyW; external WinlicenseDLL name 'WLCustomGenLicenseFileKeyW';
function WLCustomGenLicenseFileKeyEx; external WinlicenseDLL name 'WLCustomGenLicenseFileKeyEx';
function WLCustomGenLicenseRegistryKeyEx; external WinlicenseDLL name 'WLCustomGenLicenseRegistryKeyEx';
function WLCustomGenLicenseTextKeyEx; external WinlicenseDLL name 'WLCustomGenLicenseTextKeyEx';
function WLCustomGenLicenseRegistryKey; external WinlicenseDLL name 'WLCustomGenLicenseRegistryKey';
function WLCustomGenLicenseRegistryKeyW; external WinlicenseDLL name 'WLCustomGenLicenseRegistryKeyW';
function WLCustomGenLicenseTextKey; external WinlicenseDLL name 'WLCustomGenLicenseTextKey';
function WLCustomGenLicenseTextKeyW; external WinlicenseDLL name 'WLCustomGenLicenseTextKey';
function WLCustomGenLicenseSmartKey; external WinlicenseDLL name 'WLCustomGenLicenseSmartKey';
function WLCustomGenLicenseSmartKeyW; external WinlicenseDLL name 'WLCustomGenLicenseSmartKeyW';
function WLCustomGenLicenseFileKeyExW; external WinlicenseDLL name 'WLCustomGenLicenseFileKeyExW';
function WLCustomGenLicenseRegistryKeyExW; external WinlicenseDLL name 'WLCustomGenLicenseRegistryKeyExW';
function WLCustomGenLicenseTextKeyExW; external WinlicenseDLL name 'WLCustomGenLicenseTextKeyExW';
function WLCustomGenLicenseDynSmartKey; external WinlicenseDLL name 'WLCustomGenLicenseDynSmartKey';
function WLCustomGenLicenseDynSmartKeyW; external WinlicenseDLL name 'WLCustomGenLicenseDynSmartKeyW';

end.

