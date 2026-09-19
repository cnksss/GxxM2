unit RouteManage;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls;

type
  TfrmRouteManage = class(TForm)
    GroupBox1: TGroupBox;
    ListViewRoute: TListView;
    ButtonEdit: TButton;
    ButtonDelete: TButton;
    ButtonOK: TButton;
    ButtonAddRoute: TButton;
    procedure ButtonDeleteClick(Sender: TObject);
    procedure ListViewRouteClick(Sender: TObject);
    procedure ButtonEditClick(Sender: TObject);
    procedure ButtonAddRouteClick(Sender: TObject);
  private
    { Private declarations }

    procedure RefShowRoute;
  public
    procedure Open;
    { Public declarations }
  end;

var
  frmRouteManage: TfrmRouteManage;

implementation

uses DBShare, RouteEdit;

{$R *.dfm}

{ TfrmRouteManage }

procedure TfrmRouteManage.Open;
begin
  RefShowRoute();

  ShowModal;
end;

procedure TfrmRouteManage.RefShowRoute;
var
  i, ii: Integer;
  ListItem: TListItem;
  RouteInfo: pTRouteInfo;
  sGameGate: string;
begin
  ListViewRoute.Clear;
  ButtonEdit.Enabled := False;
  ButtonDelete.Enabled := False;
  for i := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    RouteInfo := @g_RouteInfo[i];
    if RouteInfo.nGateCount = 0 then break;
    sGameGate := '';
    ListItem := ListViewRoute.Items.Add;
    ListItem.Data := RouteInfo;
    ListItem.Caption := IntToStr(i);
    ListItem.SubItems.Add(RouteInfo.sSelGateIP);
    ListItem.SubItems.Add(IntToStr(RouteInfo.nGateCount));
    for ii := 0 to RouteInfo.nGateCount - 1 do
    begin
      sGameGate := format('%s %s:%d ', [sGameGate, RouteInfo.sGameGateIP[ii], RouteInfo.nGameGatePort[ii]]);
    end;
    ListItem.SubItems.Add(sGameGate);
  end;
end;


procedure TfrmRouteManage.ListViewRouteClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := ListViewRoute.Selected;
  if ListItem = nil then Exit;
  ButtonEdit.Enabled := True;
  ButtonDelete.Enabled := True;
end;

procedure TfrmRouteManage.ButtonDeleteClick(Sender: TObject);
var
  ii: Integer;
  ListItem: TListItem;
  RouteInfo: pTRouteInfo;
begin
  ListItem := ListViewRoute.Selected;
  if ListItem = nil then Exit;
  RouteInfo := ListItem.Data;
  RouteInfo.nGateCount := 0;
  RouteInfo.sSelGateIP := '';

  for ii := Low(RouteInfo.sGameGateIP) to High(RouteInfo.sGameGateIP) do
  begin
    RouteInfo.sGameGateIP[ii] := '';
    RouteInfo.nGameGatePort[ii] := 0;
  end;
  RouteInfo.RunGate2List.Clear;
  RefShowRoute();
end;


procedure TfrmRouteManage.ButtonEditClick(Sender: TObject);
var
  ListItem: TListItem;
  RouteInfo: pTRouteInfo;
begin
  ListItem := ListViewRoute.Selected;
  if ListItem = nil then Exit;
  RouteInfo := ListItem.Data;
  if ShowFrmRouteEdit(RouteInfo, True) then
  begin
    RefShowRoute();
  end;
end;

procedure TfrmRouteManage.ButtonAddRouteClick(Sender: TObject);
var
  RouteInfo: pTRouteInfo;
  nNulIdx: Integer;
begin
  nNulIdx := ListViewRoute.Items.Count;
  if nNulIdx >= 20 then
  begin
    MessageBox(Handle, '路由条数已经达到指定数量,不能再增加路由！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;
  RouteInfo := @g_RouteInfo[nNulIdx];
  if ShowFrmRouteEdit(RouteInfo, False) then
  begin
    RefShowRoute();
  end;
end;

end.
