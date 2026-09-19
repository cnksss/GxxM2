unit uFrmStorageItemsView;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, StdCtrls;

type
  TFrmStorageItemsView = class(TForm)
    grp1: TGroupBox;
    lstUsers: TListBox;
    grp2: TGroupBox;
    lvItems: TListView;
    lbl1: TLabel;
    edtUser: TEdit;
    btnSearch: TButton;
    btnDel: TButton;
    btnDelAll: TButton;
    lbl2: TLabel;
    procedure lstUsersClick(Sender: TObject);
    procedure btnDelClick(Sender: TObject);
    procedure btnDelAllClick(Sender: TObject);
    procedure lvItemsEditing(Sender: TObject; Item: TListItem;
      var AllowEdit: Boolean);
    procedure lvItemsClick(Sender: TObject);
    procedure btnSearchClick(Sender: TObject);
  private
    { Private declarations }

    FUserName: string;
  public
    { Public declarations }
  end;

  procedure ShowFrmStorageItemsView;
  
implementation

uses
  Grobal2, M2Share, ObjPlayer;

procedure ShowFrmStorageItemsView;
var
  Form: TFrmStorageItemsView;
  I: Integer;
  SL: TStringList;
begin
  Form := TFrmStorageItemsView.Create(nil);
  try
    Form.FUserName := '';
    SL := TStringList.Create;
    try
      g_M2DataDB.StorageDB.GetAllHumans(SL);
      for I := 0 to SL.Count - 1 do
      begin
        Form.lstUsers.Items.Add(SL[I]);
      end;
    finally
      SL.Free;
    end;

    Form.btnDel.Enabled  := (Form.lvItems.ItemIndex >= 0) and (Form.lvItems.ItemIndex < Form.lvItems.Items.Count);
    Form.btnDelAll.Enabled := Form.lvItems.Items.Count > 0;
    Form.ShowModal;
  finally
    Form.Free;
  end;
end;

{$R *.dfm}

procedure TFrmStorageItemsView.lstUsersClick(Sender: TObject);
var
  ItemList: TList;
  TempList: TList;
  I: Integer;
  Player: TPlayObject;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  ListItem: TListItem;
  StorageID: Integer;
begin
  lvItems.Clear;

  if (lstUsers.ItemIndex >= 0) and (lstUsers.ItemIndex < lstUsers.Items.Count) then
  begin
    ItemList := TList.Create;
    try
      FUserName := lstUsers.Items[lstUsers.ItemIndex];
      Player := UserEngine.GetPlayObject(FUserName);
      if Player <> nil then
        TempList := Player.m_BigStorageItemList
      else
      begin
        g_M2DataDB.StorageDB.LoadStorageItems(FUserName, ItemList, StorageID);
        TempList := ItemList;
      end;

      for I := 0 to TempList.Count - 1 do
      begin
        UserItem := TempList.Items[I];
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);

        ListItem := lvItems.Items.Add;
        ListItem.Caption := IntToStr(I + 1);

        if StdItem <> nil then
          ListItem.SubItems.Add(StdItem.Name)
        else
          ListItem.SubItems.Add('');

        ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
        ListItem.SubItems.Add(IntToStr(UserItem.wIndex + 1));
        ListItem.SubItems.Add(IntToStr(UserItem.Dura));
        ListItem.SubItems.Add(IntToStr(UserItem.DuraMax));
      end;

      for I := 0 to ItemList.Count - 1 do
      begin
        UserItem := ItemList.Items[I];
        Dispose(UserItem);
      end;
    finally
      ItemList.Free;
    end;
  end;

  btnDel.Enabled := (lvItems.ItemIndex >= 0) and (lvItems.ItemIndex < lvItems.Items.Count);
  btnDelAll.Enabled := lvItems.Items.Count > 0;
end;

procedure TFrmStorageItemsView.btnDelClick(Sender: TObject);
var
  ItemList: TList;
  TempList: TList;
  I: Integer;
  Player: TPlayObject;
  UserItem: pTUserItem;
  nMakeIndex: Integer;
  StorageID: Integer;
begin
  if (lvItems.ItemIndex >= 0) and (lvItems.ItemIndex < lvItems.Items.Count) then
  begin
    nMakeIndex := StrToIntDef(lvItems.Items[lvItems.ItemIndex].SubItems[1], 0);
    lvItems.DeleteSelected;
    ItemList := TList.Create;
    try
      FUserName := lstUsers.Items[lstUsers.ItemIndex];
      Player := UserEngine.GetPlayObject(FUserName);
      if Player <> nil then
        TempList := Player.m_BigStorageItemList
      else
      begin
        g_M2DataDB.StorageDB.LoadStorageItems(FUserName, ItemList, StorageID);
        TempList := ItemList;
      end;

      for I := 0 to TempList.Count - 1 do
      begin
        UserItem := TempList.Items[I];
        if UserItem.MakeIndex = nMakeIndex then
        begin
          TempList.Delete(I);
          
          g_M2DataDB.StorageDB.DeleteStorageItem(0, FUserName, UserItem.MakeIndex, UserItem.wIndex);

          Dispose(UserItem);
          Break;
        end;
      end;

      for I := 0 to ItemList.Count - 1 do
      begin
        UserItem := ItemList.Items[I];
        Dispose(UserItem);
      end;
    finally
      ItemList.Free;
    end;
  end;

  btnDel.Enabled := (lvItems.ItemIndex >= 0) and (lvItems.ItemIndex < lvItems.Items.Count);
  btnDelAll.Enabled := lvItems.Items.Count > 0;
end;

procedure TFrmStorageItemsView.btnDelAllClick(Sender: TObject);
var
  ItemList: TList;
  TempList: TList;
  I: Integer;
  Player: TPlayObject;
  UserItem: pTUserItem;
  StorageID: Integer;
begin
  if (lvItems.Items.Count > 0) then
  begin
    if Application.MessageBox('是否确定删除全部物品！', '提示', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      lvItems.Clear;
      ItemList := TList.Create;
      try
        FUserName := lstUsers.Items[lstUsers.ItemIndex];
        Player := UserEngine.GetPlayObject(FUserName);
        if Player <> nil then
          TempList := Player.m_BigStorageItemList
        else
        begin
          g_M2DataDB.StorageDB.LoadStorageItems(FUserName, ItemList, StorageID);
          TempList := ItemList;
        end;

        for I := 0 to TempList.Count - 1 do
        begin
          UserItem := TempList.Items[I];
          Dispose(UserItem);
        end;
        TempList.Clear;

        g_M2DataDB.StorageDB.ClearStorageItem(0, FUserName);
      finally
        ItemList.Free;
      end;
    end;
  end;

  btnDel.Enabled := (lvItems.ItemIndex >= 0) and (lvItems.ItemIndex < lvItems.Items.Count);
  btnDelAll.Enabled := lvItems.Items.Count > 0;
end;

procedure TFrmStorageItemsView.lvItemsEditing(Sender: TObject;
  Item: TListItem; var AllowEdit: Boolean);
begin
  AllowEdit := False;
end;

procedure TFrmStorageItemsView.lvItemsClick(Sender: TObject);
begin
  btnDel.Enabled := (lvItems.ItemIndex >= 0) and (lvItems.ItemIndex < lvItems.Items.Count);
end;

procedure TFrmStorageItemsView.btnSearchClick(Sender: TObject);
var
  I: Integer;
begin
  if Length(edtUser.Text) > 0 then
  begin
    for I := 0 to lstUsers.Items.Count - 1 do
    begin
      if SameText(lstUsers.Items[I], edtUser.Text) then
      begin
        lstUsers.ItemIndex := I;
        lstUsers.OnClick(lstUsers);
        Break;
      end;
    end;
  end;
end;

end.
