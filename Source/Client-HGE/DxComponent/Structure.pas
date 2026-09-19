unit Structure;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, RzTreeVw, DxControls, DxComponents, Menus, Main, Magnetic, Clipbrd, StreamClipbrd;

type
  TStructureDlg = class(TForm)
    TreeView: TRzTreeView;
    PopupMenu: TPopupMenu;
    Menu_Add: TMenuItem;
    Menu_Delete: TMenuItem;
    Menu_Out: TMenuItem;
    Menu_IN: TMenuItem;
    procedure TreeViewClick(Sender: TObject);
    procedure Menu_DeleteClick(Sender: TObject);
    procedure Menu_AddClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure TreeViewKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure TreeViewDblClick(Sender: TObject);
  private

  public
    procedure WndProc(var Msg_: TMessage); override;
  end;

var
  StructureDlg: TStructureDlg;

implementation
uses DxPageControl, DxImageButton, DxEdit, DxLabel, DxImageGrid, DxPopupMenu, DxComboBox, Share, Objects;

{$R *.dfm}
var
  MagneticWndProc: TSubClass_Proc;
  MagneticAddWindow: Boolean;
// procedure to subclass form's window procedure for magnetic effect.

procedure TStructureDlg.WndProc(var Msg_: TMessage);
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

procedure TStructureDlg.TreeViewClick(Sender: TObject);
var
  D, DxControl: TDxControl;
  TreeNode: TTreeNode;
  MainTreeNode: TTreeNode;
  AOwner: TDxControl;
begin
  TreeNode := TreeView.Selected;
  if (TreeNode = nil) then begin
    Menu_Delete.Enabled := False;
    Exit;
  end;
  Menu_Delete.Enabled := True;
  if (TreeNode.Parent = nil) then begin
    g_SelectComponent := nil;
    g_Background := TDxControlEngine(TreeNode.Data);
    {if Assigned(g_Background.OnClick) then begin
      g_Background.OnClick(g_Background, 0, 0);
    end;}
    Caption := TreeNode.Text;
  end else begin
    if g_Background <> nil then begin
      g_SelectComponent := nil;
      DxControl := TDxControl(TreeNode.Data);
      g_SelectComponent := DxControl;
      MainTreeNode := TreeNode;
      while True do begin
        if (MainTreeNode.Parent = nil) then break
        else MainTreeNode := MainTreeNode.Parent;
      end;
      if MainTreeNode <> nil then begin
        if (MainTreeNode.Parent = nil) then begin
          g_Background := TDxControlEngine(MainTreeNode.Data);
        end;
      end;

      D := DxControl;
      while True do begin
        if D is TDxTabSheet then
          if TDxTabSheet(D).Owner is TDxPageControl then
            TDxPageControl(TDxTabSheet(D).Owner).ActivePage := TDxTabSheet(D);
        D := TDxControl(D.Owner);
        if D = nil then break;
      end;

      if Assigned(DxControl.OnClick) then begin
        DxControl.OnClick(DxControl, 0, 0);
      end;
    end;
  end;
end;

procedure TStructureDlg.Menu_DeleteClick(Sender: TObject);
var
  TreeNode: TTreeNode;
  DxControl: TDxControl;
begin
  TreeNode := TreeView.Selected;
  if (TreeNode = nil) then begin
    Menu_Delete.Enabled := False;
    Exit;
  end;
  if (TreeNode.Parent = nil) then begin
    DxControl := TDxControl(TreeNode.Data);
    TreeNode := TTreeNode(DxControl.Data);
    TreeView.Items.Delete(TreeNode);
    ObjectsDlg.JvInspector.Clear;

    if g_Background = DxControl then
      g_Background := nil;

    FrmMain.DelSubMenu(DxControl);
    FreeAndNil(DxControl);
  end else begin
    DxControl := TDxControl(TreeNode.Data);
    TreeNode := TTreeNode(DxControl.Data);
    TreeView.Items.Delete(TreeNode);
    ObjectsDlg.JvInspector.Clear;

    if g_SelectComponent = DxControl then
      g_SelectComponent := nil;
    FreeAndNil(DxControl);
  end;


 { if (TreeNode.Parent = nil) then begin
    ControlEngine := TDxControlEngine(TreeNode.Data);
    TreeNode := TTreeNode(ControlEngine.Data);
    TreeView.Items.Delete(TreeNode);
    ObjectsDlg.JvInspector.Clear;

    if g_Background = ControlEngine then
      g_Background := nil;

    FrmMain.DelSubMenu(ControlEngine);
    FreeAndNil(ControlEngine);
  end else begin
    DxControl := TDxControl(TreeNode.Data);
    TreeNode := TTreeNode(DxControl.Data);
    TreeView.Items.Delete(TreeNode);
    ObjectsDlg.JvInspector.Clear;

    if g_SelectComponent = DxControl then
      g_SelectComponent := nil;

    FreeAndNil(DxControl);
  end;    }
  Menu_Delete.Enabled := False;
  TreeView.PopupMenu := nil;
  TreeView.PopupMenu := PopupMenu;
end;

procedure TStructureDlg.Menu_AddClick(Sender: TObject);
var
  TreeNode: TTreeNode;
  Background: TDxControlEngine;
begin
  Inc(g_nBackground);
  Background := TDxControlEngine.Create();
  Background.Name := Background.Name; // + IntToStr(g_nBackground);
  Background.Width := FrmMain.ClientWidth;
  Background.Height := FrmMain.ClientHeight;
  Background.OnMouseDown := FrmMain.OnDxControlMouseDown;
  Background.OnClick := FrmMain.OnDxControlClick;
  Background.OnDblClick := FrmMain.OnDxControlDblClick;
  FrmMain.AddSubMenu(Background);
  TreeNode := TreeView.Items.AddObject(nil, Background.Name, Background);
  Background.Data := TreeNode;
end;

procedure TStructureDlg.FormCreate(Sender: TObject);
begin
  MagneticAddWindow := False;
  if not Assigned(MagneticWndProc) then
    if Assigned(MagneticWnd) and (not MagneticAddWindow) then begin
      MagneticAddWindow := True;
      MagneticWnd.AddWindow(Self.Handle, FrmMain.Handle, MagneticWndProc);
    end;
end;

procedure TStructureDlg.TreeViewKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
var
  Component: TComponent;
  DxControl: TDxControl;
  ControlEngine: TDxControlEngine;
  TreeNode, ChildTreeNode: TTreeNode;
  MemoryStream: TMemoryStream;
begin
  {TreeNode := TreeView.Selected;
  if (TreeNode = nil) then begin
    //Menu_Delete.Enabled := False;
    Exit;
  end;
  Component := nil;
  ControlEngine := nil;
  DxControl := nil;
  if (TreeNode.Parent = nil) then begin
    ControlEngine := TDxControlEngine(TreeNode.Data);
    TreeNode := TTreeNode(ControlEngine.Data);
    Component := ControlEngine;
  end else begin
    DxControl := TDxControl(TreeNode.Data);
    TreeNode := TTreeNode(DxControl.Data);
    Component := DxControl;
  end;

  if (ssCtrl in Shift) and (Component <> nil) then begin
    case Key of
      Byte('C'): begin
          if DxControl then begin
            MemoryStream := TMemoryStream.Create;
            FrmMain.SaveControl(MemoryStream, DxControl);
            MemoryStream.Position := 0;
            Clipboard.Clear;

            StreamSaveToClipboard(MemoryStream);
            MemoryStream.Free();
          end;
        end;
      Byte('X'): begin
          MemoryStream := TMemoryStream.Create;
          FrmMain.SaveControl(MemoryStream, DxControl);
          MemoryStream.Position := 0;
          Clipboard.Clear;

          StreamSaveToClipboard(MemoryStream);
          MemoryStream.Free();

          TreeNode := TTreeNode(DxControl.Data);
          StructureDlg.TreeView.Items.Delete(TreeNode);
          ObjectsDlg.JvInspector.Clear;
          FreeAndNil(DxControl);
        end;
      Byte('Z'): ;
      Byte('V'): begin
          if not ((DxControl is TDxImageButton) or
            (DxControl is TDxEdit) or
            (DxControl is TDxLabel) or
            (DxControl is TDxImageGrid) or
            (DxControl is TDxPopupMenu) or
            (DxControl is TDxComboBox)) then begin
            MemoryStream := TMemoryStream.Create;
            StreamLoadFromClipboard(MemoryStream);
            MemoryStream.Position := 0;
            FrmMain.LoadControl(MemoryStream, DxControl);
            MemoryStream.Free;
          end;
        end;
      Byte('A'): ;
    end;
  end;}
end;

procedure TStructureDlg.TreeViewDblClick(Sender: TObject);
var
  DxControl: TDxControl;
  TreeNode: TTreeNode;
begin
  TreeNode := TreeView.Selected;
  if (TreeNode = nil) then begin
    Menu_Delete.Enabled := False;
    Exit;
  end;
  Menu_Delete.Enabled := True;
  if (TreeNode.Parent = nil) then begin
    g_Background := TDxControlEngine(TreeNode.Data);
    {ObjectsDlg.AddComponent(g_Background);
    {if Assigned(g_Background.OnClick) then begin
      g_Background.OnClick(g_Background, 0, 0);
    end;}
    Caption := TreeNode.Text;
  end else begin
    if g_Background <> nil then begin
      DxControl := TDxControl(TreeNode.Data);
{$IF  CLIENTEXE = 0}
      ObjectsDlg.AddComponent(DxControl);
{$IFEND}
      if Assigned(DxControl.OnClick) then begin
        DxControl.OnClick(DxControl, 0, 0);
      end;
    end;
  end;
end;

end.

