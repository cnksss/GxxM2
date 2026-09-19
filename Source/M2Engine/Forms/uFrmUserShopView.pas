unit uFrmUserShopView;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls, M2Share, M2DataCommon;

type
  TFrmUserShopView = class(TForm)
    lvItems: TListView;
    lbl1: TLabel;
    edtUser: TEdit;
    btnSearch: TButton;
    lbl2: TLabel;
    edtShopName: TEdit;
    btnRename: TButton;
    procedure FormCreate(Sender: TObject);
    procedure lvItemsSelectItem(Sender: TObject; Item: TListItem;
      Selected: Boolean);
    procedure btnRenameClick(Sender: TObject);
    procedure btnSearchClick(Sender: TObject);
  private
    { Private declarations }

    FSelectShopID: Integer;
    FSelectUserName: string;
    FSelectShopName: string;
    FSelectItemIndex: Integer;
  public
    { Public declarations }
  end;

  procedure ShowFrmUserShopView;

implementation

{$R *.dfm}

procedure ShowFrmUserShopView;
var
  FrmUserShopView: TFrmUserShopView;
begin
  FrmUserShopView := TFrmUserShopView.Create(nil);
  try
    FrmUserShopView.ShowModal;
  finally
    FrmUserShopView.Free;
  end;
end;

procedure TFrmUserShopView.FormCreate(Sender: TObject);
var
  UserShopList: TUserShopList;
  I: Integer;
  UserShop: pTUserShop;
  ListItem: TListItem;
begin
  UserShopList := TUserShopList.Create;
  try
    g_M2DataDB.UserShopDB.GetAllShopEx(UserShopList);

    for I := 0 to UserShopList.Count - 1 do
    begin
      UserShop := UserShopList.Items[I];

      ListItem := lvItems.Items.Add;
      ListItem.Caption := IntToStr(UserShop.ShopID);
      ListItem.SubItems.Add(UserShop.sMasterName);
      ListItem.SubItems.Add(UserShop.sShopName);
      ListItem.SubItems.Add(FormatDateTime('yyyy/mm/dd', UserShop.dCreateDate));
    end;
  finally
    UserShopList.Free;
  end;

  FSelectShopID := -1;
  FSelectUserName := '';
  FSelectShopName := '';
  FSelectItemIndex := -1;
end;

procedure TFrmUserShopView.lvItemsSelectItem(Sender: TObject;
  Item: TListItem; Selected: Boolean);
begin
  if Selected then
  begin
    FSelectShopID := StrToIntDef(Item.Caption, 0);
    FSelectUserName := Item.SubItems[0];
    FSelectShopName := Item.SubItems[1];
    FSelectItemIndex := Item.Index;

    edtShopName.Text := FSelectShopName;
    edtShopName.Enabled := True;
    btnRename.Enabled := True;
  end
  else
  begin
    FSelectShopID := -1;
    FSelectUserName := '';
    FSelectShopName := '';
    FSelectItemIndex := -1;

    edtShopName.Text := '';
    edtShopName.Enabled := False;
    btnRename.Enabled := False;
  end;
end;

procedure TFrmUserShopView.btnRenameClick(Sender: TObject);
var
  NewShopName: string;
begin
  NewShopName := Trim(edtShopName.Text);

  if g_M2DataDB.UserShopDB.ShopNameExists(NewShopName) then
  begin
    ShowMessage(NewShopName + ' 店铺名已被占用');
    Exit;
  end
  else
  begin
    if GetNameInFilterList(NewShopName) then
    begin                                                                                       // 检测英雄名称是否有非法字符
      ShowMessage('商店名称包含禁止的字符');
      Exit;
    end;

    if Application.MessageBox(PChar(Format('是否将用户 %s 的商铺 "%s" 改名为 "%s"', [FSelectUserName, FSelectShopName, NewShopName])), '确认改名', MB_YESNO or MB_ICONQUESTION) = idYes then
    begin
      if g_M2DataDB.UserShopDB.ShopRename(FSelectShopID, NewShopName) then
      begin
        lvItems.Items[FSelectItemIndex].SubItems[1] := NewShopName;
        FSelectShopName := NewShopName;
        ShowMessage('店铺改名成功');
      end;
    end;
  end;
end;

procedure TFrmUserShopView.btnSearchClick(Sender: TObject);
var
  I: Integer;
begin
  if Length(edtUser.Text) = 0 then Exit;

  for I := 0 to lvItems.Items.Count - 1 do
  begin
    if SameText(lvItems.Items[I].SubItems[0], edtUser.Text) then
    begin
      lvItems.Items[I].Selected := True;
      lvItems.ItemIndex := I;
      lvItems.SetFocus;
      Break;
    end;
  end;

end;

end.

