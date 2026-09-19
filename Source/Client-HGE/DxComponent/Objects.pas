unit Objects;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, JvComponentBase, JvInspector, JvExControls, JvComponent,
  TypInfo, JvInspExtraEditors, DxControls, DxComponents, Share, Magnetic, Main;

type
  TShowComponent = class(TThread)
  private
    AddComponentTick: LongWord;
    Component: TComponent;
    ComponentList: TList;
    CriticalSection: TRTLCriticalSection;
    procedure UpdateComponent;
  protected
    procedure Execute; override;
  public
    constructor Create;
    destructor Destroy; override;
    procedure AddComponent(Component: TComponent);
  end;

  TObjectsDlg = class(TForm)
    JvInspector: TJvInspector;
    procedure JvInspectorDataValueChanged(Sender: TObject;
      Data: TJvCustomInspectorData);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
  private
    ShowComponent: TShowComponent;
  public
    procedure AddComponent(Component: TComponent);
    procedure WndProc(var Msg_: TMessage); override;
  end;

var
  ObjectsDlg: TObjectsDlg;

implementation
uses Structure;
{$R *.dfm}
var
  MagneticWndProc: TSubClass_Proc;
  MagneticAddWindow: Boolean;

  SelComponent: TComponent;
// procedure to subclass form's window procedure for magnetic effect.
{------------------------------------------------------------------------------}

constructor TShowComponent.Create;
begin
  inherited Create(True);
  InitializeCriticalSection(CriticalSection);
  ComponentList := TList.Create;
  AddComponentTick := GetTickCount;
  Resume;
end;

destructor TShowComponent.Destroy;
begin
  ComponentList.Free;
  DeleteCriticalSection(CriticalSection);
  inherited Destroy;
end;

procedure TShowComponent.UpdateComponent;
begin
{$IF  CLIENTEXE = 0}
  if SelComponent <> Component then begin
    SelComponent := Component;
    ObjectsDlg.JvInspector.Clear;
    ObjectsDlg.JvInspector.AddComponent(Component, Component.ClassName + '::' + Component.Name);
  end;
{$IFEND}

end;

procedure TShowComponent.Execute;

begin
  while not Terminated do begin
    Component := nil;
    if GetTickCount - AddComponentTick > 100 then begin
      AddComponentTick := GetTickCount;
      EnterCriticalSection(CriticalSection);
      try
        if ComponentList.Count > 0 then begin
          Component := TComponent(ComponentList.Items[ComponentList.Count - 1]);
          ComponentList.Clear;
        end;
      finally
        LeaveCriticalSection(CriticalSection);
      end;
      if Component <> nil then begin
        Synchronize(UpdateComponent);
      end;
    end;

    Sleep(1);
  end;
end;

procedure TShowComponent.AddComponent(Component: TComponent);
begin
  EnterCriticalSection(CriticalSection);
  try
    ComponentList.Add(Component);
  finally
    LeaveCriticalSection(CriticalSection);
  end;
end;

{------------------------------------------------------------------------------}

procedure TObjectsDlg.WndProc(var Msg_: TMessage);
var
  Handled: boolean;
begin
  if not Assigned(MagneticWndProc) then
  begin
    inherited WndProc(Msg_);
    exit;
  end;

  if (Msg_.Msg = WM_SYSCOMMAND) or (Msg_.Msg = WM_ENTERSIZEMOVE) or (Msg_.Msg = WM_EXITSIZEMOVE) or
    (Msg_.Msg = WM_WINDOWPOSCHANGED) or (Msg_.Msg = WM_COMMAND) then
  begin
    inherited WndProc(Msg_);
    MagneticWndProc(Self.Handle, Msg_.Msg, Msg_, Handled);
  end else if (Msg_.Msg = WM_MOVING) or (Msg_.Msg = WM_SIZING) then
  begin
    MagneticWndProc(Self.Handle, Msg_.Msg, Msg_, Handled);
    if not Handled then
      inherited WndProc(Msg_);

  end else
    inherited WndProc(Msg_);
end;

procedure TObjectsDlg.AddComponent(Component: TComponent);
begin
{$IF  CLIENTEXE = 0}
  //ShowComponent.AddComponent(Component);
  if SelComponent <> Component then begin
    SelComponent := Component;
    JvInspector.Clear;
    JvInspector.AddComponent(Component, Component.ClassName + '::' + Component.Name);
  end;
{$IFEND}
end;

procedure TObjectsDlg.JvInspectorDataValueChanged(Sender: TObject;
  Data: TJvCustomInspectorData);
begin
  if (Data.Name = 'Name') and (Data is TJvInspectorPropData) then begin
    if TJvInspectorPropData(Data).Instance is TComponent then begin
      TTreeNode(TDxControl(TJvInspectorPropData(Data).Instance).Data).Text := TDxControl(TJvInspectorPropData(Data).Instance).Name;
    end;
  end;
end;

procedure TObjectsDlg.FormCreate(Sender: TObject);
begin
  MagneticAddWindow := False;
  //ShowComponent := TShowComponent.Create;
  if not Assigned(MagneticWndProc) then
    if Assigned(MagneticWnd) and (not MagneticAddWindow) then begin
      MagneticAddWindow := True;
      MagneticWnd.AddWindow(Self.Handle, FrmMain.Handle, MagneticWndProc);
    end;

end;

procedure TObjectsDlg.FormDestroy(Sender: TObject);
begin
  //ShowComponent.Terminate;
  //ShowComponent.Free;
end;

initialization
  //TJvInspectorAlignItem.RegisterAsDefaultItem;
  TJvInspectorAnchorsItem.RegisterAsDefaultItem;
  TJvInspectorColorItem.RegisterAsDefaultItem;
  //TJvInspectorTImageIndexItem.RegisterAsDefaultItem;

end.

