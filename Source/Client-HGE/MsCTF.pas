{*******************************************************}
{                                                       }
{           CodeGear Delphi Runtime Library             }
{ Copyright(c) 2015-2017 Embarcadero Technologies, Inc. }
{              All rights reserved                      }
{                                                       }
{*******************************************************}

Unit MsCTF;

interface

uses
  Windows,
  ActiveX;

type
  InputScope = (
    // common input scopes
    IS_DEFAULT = 0,
    IS_URL = 1,
    IS_FILE_FULLFILEPATH = 2,
    IS_FILE_FILENAME = 3,
    IS_EMAIL_USERNAME = 4,
    IS_EMAIL_SMTPEMAILADDRESS = 5,
    IS_LOGINNAME = 6,
    IS_PERSONALNAME_FULLNAME = 7,
    IS_PERSONALNAME_PREFIX = 8,
    IS_PERSONALNAME_GIVENNAME = 9,
    IS_PERSONALNAME_MIDDLENAME = 10,
    IS_PERSONALNAME_SURNAME = 11,
    IS_PERSONALNAME_SUFFIX = 12,
    IS_ADDRESS_FULLPOSTALADDRESS = 13,
    IS_ADDRESS_POSTALCODE = 14,
    IS_ADDRESS_STREET = 15,
    IS_ADDRESS_STATEORPROVINCE = 16,
    IS_ADDRESS_CITY = 17,
    IS_ADDRESS_COUNTRYNAME = 18,
    IS_ADDRESS_COUNTRYSHORTNAME = 19,
    IS_CURRENCY_AMOUNTANDSYMBOL = 20,
    IS_CURRENCY_AMOUNT = 21,
    IS_DATE_FULLDATE = 22,
    IS_DATE_MONTH = 23,
    IS_DATE_DAY = 24,
    IS_DATE_YEAR = 25,
    IS_DATE_MONTHNAME = 26,
    IS_DATE_DAYNAME = 27,
    IS_DIGITS = 28,
    IS_NUMBER = 29,
    IS_ONECHAR = 30,
    IS_PASSWORD = 31,
    IS_TELEPHONE_FULLTELEPHONENUMBER = 32,
    IS_TELEPHONE_COUNTRYCODE = 33,
    IS_TELEPHONE_AREACODE = 34,
    IS_TELEPHONE_LOCALNUMBER = 35,
    IS_TIME_FULLTIME = 36,
    IS_TIME_HOUR = 37,
    IS_TIME_MINORSEC = 38,
    IS_NUMBER_FULLWIDTH = 39,
    IS_ALPHANUMERIC_HALFWIDTH = 40,
    IS_ALPHANUMERIC_FULLWIDTH = 41,
    IS_CURRENCY_CHINESE = 42,
    IS_BOPOMOFO = 43,
    IS_HIRAGANA = 44,
    IS_KATAKANA_HALFWIDTH = 45,
    IS_KATAKANA_FULLWIDTH = 46,
    IS_HANJA = 47,
    IS_HANJA_HALFWIDTH = 48,
    IS_HANJA_FULLWIDTH = 49,
    IS_SEARCH = 50,
    IS_FORMULA = 51,

    // special input scopes for ITfInputScope
    IS_PHRASELIST = -1,
    IS_REGULAREXPRESSION = -2,
    IS_SRGS = -3,
    IS_XML = -4,
    IS_ENUMSTRING = -5
    );

  TInputScope = InputScope;
  PInputScope = ^TInputScope;

  //
  // MSCTF entry
  //
function SetInputScope(H:HWND; inputscope:TInputScope):HRESULT; stdcall;

function IsMSCTFAvailable:Boolean;

implementation

uses
  SysUtils;

var
  // To disable TSF, you can set 0 to this global control variable.
  MsCTFHandle:THandle = 0;

const
  MsCTFModName = 'Msctf.dll';

function SetInputScope(H:HWND; inputscope:TInputScope):HRESULT; stdcall
var
  SetInputScopeProc:function(H:HWND; inputscope:TInputScope):HRESULT; stdcall;
begin
  @SetInputScopeProc := nil;
  if not Assigned(SetInputScopeProc) and (MsCTFHandle <> 0) then
    @SetInputScopeProc := GetProcAddress(MsCTFHandle, 'SetInputScope');
  if Assigned(SetInputScopeProc) then
    Result := SetInputScopeProc(H, inputscope)
  else
    Result := E_FAIL;
end;

function IsMSCTFAvailable:Boolean;
begin
  Result := MsCTFHandle <> 0;
end;

procedure InitMSCTF;
begin
  if MsCTFHandle = 0 then
    MsCTFHandle := SafeLoadLibrary(MsCTFModName);
end;

procedure DoneMSCTF;
begin
  if MsCTFHandle <> 0 then
    FreeLibrary(MsCTFHandle);
end;

initialization
  InitMSCTF;

finalization
  DoneMSCTF;

end.
