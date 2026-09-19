unit FilterItems;

interface
uses
  Classes,
  SysUtils,
  Graphics,
  GameConfigDlg;

type
  TFileItemDB = class
    m_FileItemList:TList;
    m_ShowItemList:TList; // THashedStringlist;//TStringList;
  private
  public
    constructor Create();
    destructor Destroy; override;
    function Find(sItemName:string):pTShowItem;
    procedure Get(sItemType:string; var ItemList:TList); overload;
    procedure Get(ItemType:TItemType; var ItemList:TList); overload;
    function Add(ShowItem:pTShowItem):Boolean;
    function Del(sItemName:string):Boolean;
    procedure Hint(const sItemName:string; nX, nY:Integer);
    procedure LoadFormList(StringList:TStrings; FromSystem:Boolean);

    procedure LoadFormFile;
    procedure SaveToFile;

    procedure ImportFormFile(FileName:string);
    procedure ExportToFile(FileName:string);

    procedure ImportFormStrings(StringList:TStrings);
    procedure ExportToStrings(StringList:TStrings);

    procedure BackUp;
  end;

var
  g_FileItemDB:TFileItemDB;
  g_IsClientPickItemsChanged:Boolean = False;
  g_UploadPickItemsTick:LongWord = 0;
  g_UploadPickItemsHash:LongWord = 0;

function GetActorDir(nX, nY:Integer):string;
function GetSelectString(bo:Boolean):string;

implementation

uses
  MShare,
  ConfigShare,
  ClFunc,
  HUtil32;

function GetSelectString(bo:Boolean):string;
begin
  if bo then
    Result := '√'
  else
    Result := '';
end;

function GetItemTypeName(ItemType:TItemType):string;
begin
  case ItemType of
    i_Other:Result := '其它类';
    i_HPMPDurg:Result := '药品类';
    i_Dress:Result := '服装类';
    i_Weapon:Result := '武器类';
    i_Jewelry:Result := '首饰类';
    i_Decoration:Result := '饰品类';
    i_Decorate:Result := '装饰类';
    i_diy:Result := '自定类';
  end;
end;

function GetItemType(ItemType:string):TItemType;
begin
  Result := i_diy;
  if ItemType = '其它类' then Result := i_Other;
  if ItemType = '药品类' then Result := i_HPMPDurg;
  if ItemType = '服装类' then Result := i_Dress;
  if ItemType = '武器类' then Result := i_Weapon;
  if ItemType = '首饰类' then Result := i_Jewelry;
  if ItemType = '饰品类' then Result := i_Decoration;
  if ItemType = '装饰类' then Result := i_Decorate;
  if ItemType = '自定类' then Result := i_diy;
end;

function GetActorDir(nX, nY:Integer):string;
var
  ndir:Integer;
begin
  Result := '';
  if (g_MySelf = nil) then Exit;
  ndir := GetNextDirection(g_MySelf.m_nCurrX, g_MySelf.m_nCurrY, nX, nY);
  case ndir of
    0:Result := '↑';
    1:Result := '↗';
    2:Result := '→';
    3:Result := '↘';
    4:Result := '↓';
    5:Result := '↙';
    6:Result := '←';
    7:Result := '↖';
  end;
end;

constructor TFileItemDB.Create();
begin
  m_FileItemList := TList.Create;
  m_ShowItemList := TList.Create;
end;

destructor TFileItemDB.Destroy;
var
  I:Integer;
begin
  for I := 0 to m_ShowItemList.Count - 1 do begin
    Dispose(pTShowItem(m_ShowItemList.Items[I]));
  end;
  m_ShowItemList.Free;

  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-7-17】
      for I := 0 to m_FileItemList.Count - 1 do begin
        Dispose(m_FileItemList.Items[I]);
      end;
  }
  for I := 0 to m_FileItemList.Count - 1 do begin
    Dispose(pTShowItem(m_FileItemList.Items[I]));
  end;

  m_FileItemList.Free;
  inherited;
end;

// 加载客户端过滤物品提示列表增加支持特殊物品 piaoyun 2013-09-09

procedure TFileItemDB.LoadFormFile;
var
  sDirectory:string;
  sFileName:string;
  LoadList:TStringList;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then CreateDir(sDirectory);

  if g_MySelf <> nil then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := sDirectory + g_sPlugServerName + '.' + g_sPlugUserName + '.ItemFilter.dat';
  ;

  // sFileName := 'Config\%s.%s.ItemFilter.dat', [g_sServerName, sUserName]);

  // g_ClientFunction.AddChatBoardString(PChar('LoadFormFile:' + sFileName), clyellow, clBlue);

  if FileExists(sFileName) then begin
    LoadList := TStringList.Create;
    try
      try
        LoadList.LoadFromFile(sFileName);
        LoadFormList(LoadList, False);
      except
      end;
    finally
      LoadList.Free;
    end;
  end;
end;

procedure TFileItemDB.LoadFormList(StringList:TStrings; FromSystem:Boolean);
var
  nIndex, nItemType:Integer;
  sLineText, sItemName, sItemType, sHint, sPickUp, sShowName, sShowSpecial, sAutoMove:string;
  ShowItem:pTShowItem;
  FileItem:pTShowItem;
begin
  (*
  for I := 0 to m_ShowItemList.Count - 1 do
  begin
    Dispose(pTShowItem(m_ShowItemList.Items[I]));
  end;
  m_ShowItemList.Clear;

  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-7-17】}
  for I := 0 to m_FileItemList.Count - 1 do
  begin
    Dispose(pTShowItem(m_FileItemList.Items[I]));
  end;
  m_FileItemList.Clear;
  *)

  for nIndex := 0 to StringList.Count - 1 do begin
    sLineText := Trim(StringList.Strings[nIndex]);
    if sLineText = '' then Continue;
    if (sLineText <> '') and (sLineText[1] = ';') then Continue;
    sLineText := GetValidStr3(sLineText, sItemType, [',', #9]);
    sLineText := GetValidStr3(sLineText, sItemName, [',', #9]);
    sLineText := GetValidStr3(sLineText, sHint, [',', #9]);
    sLineText := GetValidStr3(sLineText, sPickUp, [',', #9]);
    sLineText := GetValidStr3(sLineText, sShowName, [',', #9]);
    sLineText := GetValidStr3(sLineText, sShowSpecial, [',', #9]);
    sLineText := GetValidStr3(sLineText, sAutoMove, [',', #9]);
    nItemType := StrToIntDef(sItemType, -1);
    if (sItemName <> '') and
      (nItemType >= Integer(Low(TItemType))) and
      (nItemType <= Integer(High(TItemType))) then begin
      ShowItem := Find(sItemName);
      if ShowItem <> nil then begin
        ShowItem.ItemType := TItemType(nItemType);
        ShowItem.sItemType := GetItemTypeName(ShowItem.ItemType);
        ShowItem.sItemName := sItemName;
        if not FromSystem then begin
          ShowItem.boHintMsg := sHint = '1';
          ShowItem.boPickup := sPickUp = '1';
          ShowItem.boShowName := sShowName = '1';
          ShowItem.boShowSpecial := sShowSpecial = '1';
          ShowItem.boAutoMove := sAutoMove = '1';
        end;
        ShowItem.boFromSystem := FromSystem;
      end
      else begin
        // 没有找到物品则是自定义物品 piaoyun 2013-09-09
        New(ShowItem);
        ShowItem.ItemType := TItemType(nItemType);
        ShowItem.sItemType := GetItemTypeName(ShowItem.ItemType);
        ShowItem.sItemName := sItemName;
        ShowItem.boHintMsg := sHint = '1';
        ShowItem.boPickup := sPickUp = '1';
        ShowItem.boShowName := sShowName = '1';
        ShowItem.boShowSpecial := sShowSpecial = '1';
        ShowItem.boAutoMove := sAutoMove = '1';
        ShowItem.boFromSystem := FromSystem;
        //m_ShowItemList.Add(ShowItem);
        Add(ShowItem);
        New(FileItem);
        FileItem^ := ShowItem^;
        m_FileItemList.Add(FileItem);
      end;
    end;
  end;
end;

procedure TFileItemDB.BackUp;
var
  I:Integer;
begin
  for I := 0 to m_FileItemList.Count - 1 do
    pTShowItem(m_ShowItemList.Items[I])^ := pTShowItem(m_FileItemList.Items[I])^;
end;

// 客户端过滤物品提示列表增加特殊物品提示 piaoyun 2013-09-09

procedure TFileItemDB.SaveToFile;
var
  I:Integer;
  sDirectory:string;
  sFileName:string;
  SaveList:TStringList;
  FileItem:pTShowItem;
  ShowItem:pTShowItem;
begin
  if (g_MySelf = nil) then Exit;
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then CreateDir(sDirectory);
  if g_MySelf <> nil then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := sDirectory + Format('%s.%s.ItemFilter.dat', [g_sPlugServerName, g_sPlugUserName]);

  SaveList := TStringList.Create;
  for I := 0 to m_FileItemList.Count - 1 do begin
    FileItem := m_FileItemList.Items[I];
    ShowItem := Find(FileItem.sItemName);
    // chongchong 2018-02-12 去劫持
    {$I VMProtectBegin.inc}
    if ShowItem <> nil then begin
      SaveList.Add(Format('%d,%s,%d,%d,%d,%d,%d', [Integer(ShowItem.ItemType), ShowItem.sItemName,
        BoolToInt(ShowItem.boHintMsg), BoolToInt(ShowItem.boPickup), BoolToInt(ShowItem.boShowName),
          BoolToInt(ShowItem.boShowSpecial), BoolToInt(ShowItem.boAutoMove)]));
    end;
    {$I VMProtectEnd.inc}
  end;
  try
    SaveList.SaveToFile(sFileName);
  except

  end;
  SaveList.Free;
end;

procedure TFileItemDB.ImportFormFile(FileName:string);
var
  LoadList:TStringList;
begin
  if FileExists(FileName) then begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(FileName);
      ImportFormStrings(LoadList);
    finally
      LoadList.Free;
    end;
  end;
end;

procedure TFileItemDB.ExportToFile(FileName:string);
var
  SaveList:TStringList;
begin
  SaveList := TStringList.Create;
  ExportToStrings(SaveList);
  try
    SaveList.SaveToFile(FileName);
  except

  end;
  SaveList.Free;
end;

procedure TFileItemDB.ImportFormStrings(StringList:TStrings);
var
  I:Integer;
begin
  try
    for I := 0 to m_ShowItemList.Count - 1 do begin
      Dispose(pTShowItem(m_ShowItemList.Items[I]));
    end;
    m_ShowItemList.Clear;

    for I := 0 to m_FileItemList.Count - 1 do begin
      Dispose(pTShowItem(m_FileItemList.Items[I]));
    end;
    m_FileItemList.Clear;

    LoadFormList(StringList, False);
  except
  end;
end;

procedure TFileItemDB.ExportToStrings(StringList:TStrings);
var
  I:Integer;
  FileItem:pTShowItem;
  ShowItem:pTShowItem;
begin
  for I := 0 to m_FileItemList.Count - 1 do begin
    FileItem := m_FileItemList.Items[I];
    ShowItem := Find(FileItem.sItemName);
    if ShowItem <> nil then begin
      // chongchong 2018-02-12 去劫持
      {$I VMProtectBegin.inc}
      StringList.Add(Format('%d,%s,%d,%d,%d,%d,%d', [Integer(ShowItem.ItemType), ShowItem.sItemName,
        BoolToInt(ShowItem.boHintMsg), BoolToInt(ShowItem.boPickup), BoolToInt(ShowItem.boShowName),
          BoolToInt(ShowItem.boShowSpecial), BoolToInt(ShowItem.boAutoMove)]));
      {$I VMProtectEnd.inc}
    end;
  end;
end;

procedure TFileItemDB.Get(sItemType:string; var ItemList:TList);
var
  I:Integer;
  ShowItem:pTShowItem;
begin
  if ItemList = nil then Exit;
  for I := 0 to m_ShowItemList.Count - 1 do begin
    ShowItem := pTShowItem(m_ShowItemList.Items[I]);
    if (sItemType = '(全部分类)') or (ShowItem.sItemType = sItemType) then begin
      ItemList.Add(ShowItem);
    end;
  end;
end;

procedure TFileItemDB.Get(ItemType:TItemType; var ItemList:TList);
var
  I:Integer;
  ShowItem:pTShowItem;
begin
  if ItemList = nil then Exit;
  for I := 0 to m_ShowItemList.Count - 1 do begin
    ShowItem := pTShowItem(m_ShowItemList.Items[I]);
    if (ItemType = i_All) or (ShowItem.ItemType = ItemType) then begin
      ItemList.Add(ShowItem);
    end;
  end;
end;

function TFileItemDB.Add(ShowItem:pTShowItem):Boolean;
begin
  Result := False;
  if Find(ShowItem.sItemName) <> nil then Exit;
  m_ShowItemList.Add(ShowItem);
  Result := True;
end;

function TFileItemDB.Del(sItemName:string):Boolean;
var
  ShowItem:pTShowItem;
  I:Integer;
begin
  Result := False;
  for I := 0 to m_ShowItemList.Count - 1 do begin
    ShowItem := pTShowItem(m_ShowItemList.Items[I]);
    if (not ShowItem.boFromSystem) and (CompareText(ShowItem.sItemName, sItemName) = 0) then begin
      Dispose(ShowItem);
      m_ShowItemList.Delete(i);
      // 可能m_FileItemList链表需要再遍历一次 piaoyun 2013-09-10
      Dispose(pTShowItem(m_FileItemList.Items[I]));
      m_FileItemList.Delete(i);
      Result := True;
      Break;
    end;
  end;
end;

function TFileItemDB.Find(sItemName:string):pTShowItem;
var
  ShowItem:pTShowItem;
  I:Integer;
begin
  Result := nil;
  for I := 0 to m_ShowItemList.Count - 1 do begin
    ShowItem := pTShowItem(m_ShowItemList.Items[I]);
    if CompareText(ShowItem.sItemName, sItemName) = 0 then begin
      Result := ShowItem;
      Break;
    end;
  end;
end;

procedure TFileItemDB.Hint(const sItemName:string; nX, nY:Integer);
var
  ShowItem:pTShowItem;
  nCurrX, nCurrY:Integer;
  sHint, sPosition, sDir:string;
begin
  ShowItem := Find(sItemName);
  if (g_MySelf <> nil) and (ShowItem <> nil) and ShowItem.boHintMsg then begin
    nCurrX := g_MySelf.m_nCurrX;
    nCurrY := g_MySelf.m_nCurrY;

    case GetNextDirection(nCurrX, nCurrY, nX, nY) of
      0:begin
          sPosition := '上';
          sDir := '↑';
        end;
      1:begin
          sPosition := '右上';
          sDir := '↗';
        end;

      2:begin
          sPosition := '右';
          sDir := '→';
        end;
      3:begin
          sPosition := '右下';
          sDir := '↘';
        end;
      4:begin
          sPosition := '下';
          sDir := '↓';
        end;
      5:begin
          sPosition := '左下';
          sDir := '↙';
        end;
      6:begin
          sPosition := '左';
          sDir := '←';
        end;
      7:begin
          sPosition := '左上';
          sDir := '↖';
        end;
    end;
    sHint := '发现[' + sItemName + ']，方位:' + sPosition + sDir + '，坐标:(' + Format('%d,%d', [nX, nY]) + ').';
    DScreen.AddChatBoardString(sHint, $00FFFF, clBlue);
  end;
end;

initialization
  g_FileItemDB := TFileItemDB.Create;

finalization
  g_FileItemDB.Free;

end.
