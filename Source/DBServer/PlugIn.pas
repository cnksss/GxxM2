unit PlugIn;

interface
uses
  Windows, Classes, SysUtils, Forms, Grobal2, SDK, HUtil32;
type
  TFunctionValue = function: Integer; stdcall;
  TConvert = procedure(OldData, NewData: Pointer) stdcall;

  TPlugInfo = record
    DllName: string;
    sDesc: string;
    Module: THandle;
    OldVersion: TFunctionValue;
    NewVersion: TFunctionValue;
    OldVersionSize: TFunctionValue;
    NewVersionSize: TFunctionValue;
    Convert: TConvert;
  end;
  pTPlugInfo = ^TPlugInfo;

  TPlugInManage = class
    PlugList: TList;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadPlugIn();
    procedure UnLoadPlugIn();
  end;
implementation

uses DBShare;

{ TPlugIn }

constructor TPlugInManage.Create;
begin
  PlugList := TList.Create;
end;

destructor TPlugInManage.Destroy;
begin
  if PlugList.Count > 0 then 
  UnLoadPlugIn();
  PlugList.Free;
  inherited;
end;

procedure TPlugInManage.LoadPlugIn;
var
  I: Integer;
  LoadList: TStringList;
  sPlugFileName: string;
  sPlugLibName: string;
  sPlugLibFileName: string;
  Moudle: THandle;
  PlugInfo: pTPlugInfo;
begin
  sPlugFileName := g_sFilePath + 'PlugList.txt';
  if FileExists(sPlugFileName) then begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sPlugFileName);
    for I := 0 to LoadList.Count - 1 do begin
      sPlugLibName := Trim(LoadList.Strings[I]);
      if (sPlugLibName = '') or (sPlugLibName[1] = ';') then Continue;
      sPlugLibFileName := g_sFilePath + sPlugLibName;
      if FileExists(sPlugLibFileName) then begin
        Moudle := LoadLibrary(PChar(sPlugLibFileName)); //FreeLibrary
        if Moudle > 32 then begin
          New(PlugInfo);
          PlugInfo.DllName := sPlugLibFileName;
          PlugInfo.Module := Moudle;
          PlugInfo.OldVersion := GetProcAddress(Moudle, 'OldVersion');
          PlugInfo.NewVersion := GetProcAddress(Moudle, 'NewVersion');
          PlugInfo.OldVersionSize := GetProcAddress(Moudle, 'OldVersionSize');
          PlugInfo.NewVersionSize := GetProcAddress(Moudle, 'NewVersionSize');
          PlugInfo.Convert := GetProcAddress(Moudle, 'Convert');
          if (@PlugInfo.OldVersion <> nil) and
            (@PlugInfo.NewVersion <> nil) and
            (@PlugInfo.OldVersionSize <> nil) and
            (@PlugInfo.NewVersionSize <> nil) and
            (@PlugInfo.Convert <> nil)
            then begin
            PlugList.Add(PlugInfo);
          end else begin
            Dispose(PlugInfo);
            FreeLibrary(Moudle);
          end;
        end;
      end;
    end;
    LoadList.Free;
  end;
end;

procedure TPlugInManage.UnLoadPlugIn;
var
  I: Integer;
  Module: THandle;
begin
  for I := 0 to PlugList.Count - 1 do begin
    Module := pTPlugInfo(PlugList.Items[I]).Module;
    Dispose(pTPlugInfo(PlugList.Items[I]));
    FreeLibrary(Module);
  end;
  PlugList.Clear;
end;

initialization

finalization

end.

