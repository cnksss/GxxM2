unit FrmWizard;

interface

uses
  Windows, ExptIntf, ToolIntf, EditIntf, DesignIntf;

type
  TFrmCustomTitleExpert = class(TIExpert)
    function GetName: string; override;
    function GetComment: string; override;
    function GetGlyph: HICON; override;
    function GetStyle: TExpertStyle; override;
    function GetState: TExpertState; override;
    function GetIDString: string; override;
    function GetAuthor: string; override;
    function GetPage: string; override;
    function GetMenuText: string; override;
    procedure Execute; override;
  end;

implementation

{ TMyFormExpert }

procedure TFrmCustomTitleExpert.Execute;
var
  ModuleName, FormName, FileName: string;
  ModIntf: TIModuleInterface;
begin
  ToolServices.GetNewModuleAndClassName('CustomStyleForm', ModuleName, FormName, FileName);
  ModIntf := ToolServices.CreateModuleEx(FileName, FormName, 'CustomStyleForm', '', nil, nil,
    [cmNewForm, cmAddToProject, cmUnNamed]);
  ModIntf.ShowSource;
  ModIntf.ShowForm;
  ModIntf.Release;
end;

function TFrmCustomTitleExpert.GetName: string;
begin
  Result := 'TCustomStyleForm';
end;

function TFrmCustomTitleExpert.GetComment: string;
begin
  Result := 'custom form';
end;

function TFrmCustomTitleExpert.GetStyle: TExpertStyle;
begin
  Result := esForm;
end;

function TFrmCustomTitleExpert.GetGlyph: HICON;
begin
  Result := LoadIcon(HInstance, '');
end;

function TFrmCustomTitleExpert.GetState: TExpertState;
begin
  Result := [esEnabled];
end;

function TFrmCustomTitleExpert.GetAuthor: string;
begin
  Result := 'chongchong';
end;

function TFrmCustomTitleExpert.GetIDString: string;
begin
  Result := 'TFrmCustomTitleExpert.Expert';
end;

function TFrmCustomTitleExpert.GetMenuText: string;
begin
  Result := '';
end;

function TFrmCustomTitleExpert.GetPage: string;
begin
  Result := 'Forms';
end;

end.
