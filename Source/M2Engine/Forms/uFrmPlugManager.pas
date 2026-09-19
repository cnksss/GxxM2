unit uFrmPlugManager;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, VirtualTrees, ExtCtrls, M2Share, PluginManager, Menus;

type
  TFrmPlugManager = class(TForm)
    grpPlugInfo: TGroupBox;
    mmoPlugInfo: TMemo;
    grpPlugList: TGroupBox;
    vstPlug: TVirtualStringTree;
    btnLoadPlug: TButton;
    btnUnloadPlug: TButton;
    pmPlugList: TPopupMenu;
    mniLoadPlug: TMenuItem;
    mniUnloadPlug: TMenuItem;
    pnlSpliter: TPanel;
    procedure FormCreate(Sender: TObject);
    procedure vstPlugGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstPlugFreeNode(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstPlugFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
    procedure pmPlugListPopup(Sender: TObject);
    procedure mniLoadPlugClick(Sender: TObject);
    procedure mniUnloadPlugClick(Sender: TObject);
    procedure vstPlugBeforeItemErase(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect;
      var ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure vstPlugPaintText(Sender: TBaseVirtualTree; const TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      TextType: TVSTTextType);
  private
    { Private declarations }

    procedure RefreshPlugInfo(Plugin: TPlugin);
  public
    { Public declarations }
  end;

implementation

{$R *.dfm}

type
  PNodeData = ^TNodeData;

  TNodeData = record
    ID: Integer;
    PlugName: string;
    Plugin: TPlugin;
  end;

procedure TFrmPlugManager.FormCreate(Sender: TObject);
var
  I: Integer;
  Node: PVirtualNode;
  NodeData: PNodeData;
begin
  vstPlug.NodeDataSize := SizeOf(TNodeData);

  for I := 0 to g_PluginManager.PlugList.Count - 1 do
  begin
    Node := vstPlug.AddChild(nil);
    NodeData := vstPlug.GetNodeData(Node);

    NodeData.ID := I;
    NodeData.PlugName := g_PluginManager.PlugList.Strings[I];
    NodeData.Plugin := TPlugin(g_PluginManager.PlugList.Objects[I]);
  end;

  mmoPlugInfo.Clear;
end;

procedure TFrmPlugManager.vstPlugGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  case Column of
    0:
      CellText := IntToStr(Node.Index + 1);
    1:
      CellText := NodeData.PlugName;
  end;
end;

procedure TFrmPlugManager.vstPlugFreeNode(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  NodeData.PlugName := '';
end;

procedure TFrmPlugManager.vstPlugFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
var
  NodeData: PNodeData;
begin
  mmoPlugInfo.Clear;

  if Node = nil then
  begin
    btnLoadPlug.Enabled := False;
    btnUnloadPlug.Enabled := False;
    Exit;
  end;

  NodeData := Sender.GetNodeData(Node);

  btnLoadPlug.Enabled := NodeData.Plugin = nil;
  btnUnloadPlug.Enabled := (NodeData.Plugin <> nil) and (not NodeData.Plugin.IsSysDef);

  if NodeData.Plugin = nil then
    Exit;

  RefreshPlugInfo(NodeData.Plugin);
end;

procedure TFrmPlugManager.pmPlugListPopup(Sender: TObject);
var
  NodeData: PNodeData;
begin
  if vstPlug.FocusedNode = nil then
  begin
    mniLoadPlug.Enabled := False;
    mniUnloadPlug.Enabled := False;
  end
  else
  begin
    NodeData := vstPlug.GetNodeData(vstPlug.FocusedNode);
    mniLoadPlug.Enabled := (NodeData.Plugin = nil);
    mniUnloadPlug.Enabled := (NodeData.Plugin <> nil) and (not NodeData.Plugin.IsSysDef);
  end;
end;

procedure TFrmPlugManager.mniLoadPlugClick(Sender: TObject);
var
  NodeData: PNodeData;
begin
  if vstPlug.FocusedNode = nil then
    Exit;

  NodeData := vstPlug.GetNodeData(vstPlug.FocusedNode);

  if NodeData.Plugin <> nil then
    Exit;

  NodeData.Plugin := g_PluginManager.LoadPlugin(NodeData.ID);
  g_PluginManager.PlugList.Objects[NodeData.ID] := NodeData.Plugin;

  if NodeData.Plugin = nil then
    Exit;

  btnLoadPlug.Enabled := NodeData.Plugin = nil;
  btnUnloadPlug.Enabled := (NodeData.Plugin <> nil) and (not NodeData.Plugin.IsSysDef);

  vstPlug.InvalidateNode(vstPlug.FocusedNode);

  if NodeData.Plugin = nil then
  begin
    mmoPlugInfo.Clear;
  end
  else
  begin
    RefreshPlugInfo(NodeData.Plugin);
  end;
end;

procedure TFrmPlugManager.mniUnloadPlugClick(Sender: TObject);
var
  NodeData: PNodeData;
begin
  if vstPlug.FocusedNode = nil then
    Exit;

  NodeData := vstPlug.GetNodeData(vstPlug.FocusedNode);

  if (NodeData.Plugin = nil) or (NodeData.Plugin.IsSysDef) then
    Exit;

  g_PluginManager.Remove(NodeData.Plugin);

  NodeData.Plugin.Free;
  NodeData.Plugin := nil;

  g_PluginManager.PlugList.Objects[NodeData.ID] := nil;

  btnLoadPlug.Enabled := True;
  btnUnloadPlug.Enabled := False;

  vstPlug.InvalidateNode(vstPlug.FocusedNode);
  mmoPlugInfo.Clear;
end;

procedure TFrmPlugManager.RefreshPlugInfo(Plugin: TPlugin);
var
  S: string;
begin
  mmoPlugInfo.Lines.Add(Plugin.PlugDesc);

  S := Plugin.GetRegisterMenus;
  if Length(S) > 0 then
  begin
    mmoPlugInfo.Lines.Add(sLineBreak + sLineBreak);
    mmoPlugInfo.Lines.Add('--------------------------------------------------------------');
    mmoPlugInfo.Lines.Add('注册菜单:');
    mmoPlugInfo.Lines.Add(S);
  end;

  S := g_PluginManager.GetHookConditionCmdList(Plugin);
  if Length(S) > 0 then
  begin
    mmoPlugInfo.Lines.Add(sLineBreak + sLineBreak);
    mmoPlugInfo.Lines.Add('--------------------------------------------------------------');
    mmoPlugInfo.Lines.Add('自定义NPC检测命令:');
    mmoPlugInfo.Lines.Add(S);
  end;

  S := g_PluginManager.GetHookActionCmdList(Plugin);
  if Length(S) > 0 then
  begin
    mmoPlugInfo.Lines.Add(sLineBreak + sLineBreak);
    mmoPlugInfo.Lines.Add('--------------------------------------------------------------');
    mmoPlugInfo.Lines.Add('自定义NPC执行命令:');
    mmoPlugInfo.Lines.Add(S);
  end;
end;

procedure TFrmPlugManager.vstPlugBeforeItemErase(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  ItemRect: TRect; var ItemColor: TColor; var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    ItemColor := clCream;
    EraseAction := eaColor;
  end;
end;

procedure TFrmPlugManager.vstPlugPaintText(Sender: TBaseVirtualTree; const TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; TextType: TVSTTextType);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData.Plugin = nil then
    TargetCanvas.Font.Color := clRed
  else if NodeData.Plugin.IsSysDef then
    TargetCanvas.Font.Color := clBlue;
end;

end.
