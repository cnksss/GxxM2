{ ******************************************************* }
{ }
{ 宝箱单元 }
{ }
{ 单元编写：piaoyun }
{ 编写日期：2013-08-23 }
{ }
{ ******************************************************* }

unit Boxs;

interface

uses
  Windows, Classes, SysUtils, Grobal2, HUtil32;

type
  // 宝箱物品参数
  TBoxItem = record
    ItemName: string[50];
    ItemCount: Integer;
  end;

  pTBoxItem = ^TBoxItem;

  // 宝箱专用链表
  TBoxList = class(TList)
  private
  public
    constructor Create();
    destructor Destroy; override;
    procedure Clear; override;
    function GetByName(ItemName: string): pTBoxItem; overload;
    function GetByName(ItemName: string; ItemCount: Integer): pTBoxItem; overload;
    function DeleteByName(ItemName: string): Boolean; overload;
    function DeleteByName(ItemName: string; ItemCount: Integer): Boolean; overload;
  end;

  // 宝箱配置
  TBoxSet = record
    boNext: Boolean;
    nGold: Integer;
    nGameGold: Integer;
    nAddGold: Integer;
    nAddGameGold: Integer;
    nEndGold: Integer;
    nEndGameGold: Integer;
    nCount: Byte;
    nNowGold: Integer;
    nNowGameGold: Integer;
    nNowCount: Byte;
  end;

  // 宝箱箱体结构
  TBox = record
    BoxSet: TBoxSet;
    Name: string;
    // Shape: Integer;
    Source: Integer;
    Give: TBoxList; // 可得
    Center: TBoxList; // 中间
    NoGive: TBoxList; // 不可得
    EndNoGive: TBoxList; // 永远不可得
  end;

  pTBox = ^TBox;

  // 宝箱类 -- 完成游戏中宝箱相关功能
  TBoxsList = class
  private
    // 宝箱链表--保存所有宝箱
    BoxList: TList;
    // 获取宝箱-Idx编号
    function GetBox(Idx: Integer): pTBox;
    // 获取宝箱数量
    function GetCount: Integer;
  public
    constructor Create();
    destructor Destroy; override;
    // 根据数据库Source字段获取对应宝箱
    function Find(Source: Integer): pTBox; overload;
    // 根据名字获取对应宝箱
    function Find(ItemName: string): pTBox; overload;
    // 添加宝箱
    // function Add(Source: Integer; ItemName: string): pTBox;
    // 删除宝箱
    // function Delete(Source: Integer; ItemName: string): Boolean;
    // 获取宝箱物品
    function GetBoxsItem(nListIdx: Integer; nIdx: Byte; MakeIndex: Integer; ClientItem: pTClientItem): TBoxSet;
    // 加载宝箱物品到链表
    function LoadBoxsItemList(const sFileName: string; Idx: Integer): Boolean;
    // 从文件载入宝箱配置
    procedure LoadFromFile();
    // 保存宝箱配置到文件
    procedure SaveToFile;
    property Items[Index: Integer]: pTBox read GetBox;
    property Count: Integer read GetCount;
  end;

implementation

uses
  M2Share;

{ TBoxList }

constructor TBoxList.Create();
begin
  inherited Create;
end;

destructor TBoxList.Destroy;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    Dispose(pTBoxItem(Items[I]));
  inherited;
end;

procedure TBoxList.Clear;
begin
  { for I := 0 to Count - 1 do
    Dispose(pTBoxItem(Items[I])); }
  inherited Clear;
end;

function TBoxList.GetByName(ItemName: string): pTBoxItem;
var
  I: Integer;
begin
  Result := nil;
  for I := 0 to Count - 1 do
  begin
    if CompareText(pTBoxItem(Items[I]).ItemName, ItemName) = 0 then
    begin
      Result := Items[I];
      Break;
    end;
  end;
end;

function TBoxList.GetByName(ItemName: string; ItemCount: Integer): pTBoxItem;
var
  I: Integer;
begin
  Result := nil;
  for I := 0 to Count - 1 do
  begin
    if (CompareText(pTBoxItem(Items[I]).ItemName, ItemName) = 0) and (pTBoxItem(Items[I]).ItemCount = ItemCount) then
    begin
      Result := Items[I];
      Break;
    end;
  end;
end;

function TBoxList.DeleteByName(ItemName: string): Boolean;
var
  I: Integer;
  BoxItem: pTBoxItem;
begin
  Result := False;
  for I := 0 to Count - 1 do
  begin
    if CompareText(pTBoxItem(Items[I]).ItemName, ItemName) = 0 then
    begin
      Result := True;
      BoxItem := Items[I];
      Dispose(BoxItem);
      Delete(I);
      Break;
    end;
  end;
end;

function TBoxList.DeleteByName(ItemName: string; ItemCount: Integer): Boolean;
var
  I: Integer;
  BoxItem: pTBoxItem;
begin
  Result := False;
  for I := 0 to Count - 1 do
  begin
    if (CompareText(pTBoxItem(Items[I]).ItemName, ItemName) = 0) and (pTBoxItem(Items[I]).ItemCount = ItemCount) then
    begin
      Result := True;
      BoxItem := Items[I];
      Dispose(BoxItem);
      Delete(I);
      Break;
    end;
  end;
end;

{ TBoxsList }

constructor TBoxsList.Create();
begin
  BoxList := TList.Create;
end;

destructor TBoxsList.Destroy;
var
  I: Integer;
  Box: pTBox;
begin
  for I := 0 to BoxList.Count - 1 do
  begin
    Box := BoxList.Items[I];
    Box.Give.Free;
    Box.Center.Free;
    Box.NoGive.Free;
    Box.EndNoGive.Free;
    Dispose(Box);
  end;
  BoxList.Free;
  inherited Destroy;
end;

function TBoxsList.Find(ItemName: string): pTBox;
var
  I: Integer;
begin
  Result := nil;
  for I := 0 to BoxList.Count - 1 do
  begin
    if CompareText(pTBox(BoxList.Items[I]).Name, ItemName) = 0 then
    begin
      Result := pTBox(BoxList.Items[I]);
      Exit;
    end;
  end;
end;

function TBoxsList.Find(Source: Integer): pTBox;
var
  I: Integer;
begin
  Result := nil;
  for I := 0 to BoxList.Count - 1 do
  begin
    if pTBox(BoxList.Items[I]).Source = Source then
    begin
      Result := pTBox(BoxList.Items[I]);
      Break;
    end;
  end;
end;

function TBoxsList.GetCount: Integer;
begin
  Result := BoxList.Count;
end;

function TBoxsList.GetBox(Idx: Integer): pTBox;
begin
  Result := BoxList.Items[Idx];
end;

{ function TBoxsList.Add(Source: Integer; ItemName: string): pTBox;
  var
  Box: pTBox;
  begin
  Result := nil;
  if Source in [15..49] then
  begin
  Box := Find(Source);
  if Box = nil then
  begin
  New(Box);
  Box.Name := ItemName;
  Box.Source := Source;
  Box.Give := TBoxList.Create;
  Box.Center := TBoxList.Create;
  Box.NoGive := TBoxList.Create;
  Box.EndNoGive := TBoxList.Create;
  BoxList.Add(Box);
  end;

  Result := Box;
  end;
  end;

  function TBoxsList.Delete(Source: Integer; ItemName: string): Boolean;
  var
  I: Integer;
  Box: pTBox;
  begin
  Result := False;
  for I := 0 to BoxList.Count - 1 do
  begin
  Box := pTBox(BoxList.Items[I]);
  if Box.Source = Source then
  begin
  Result := True;
  BoxList.Delete(I);
  Box.Give.Free;
  Box.Center.Free;
  Box.NoGive.Free;
  Box.EndNoGive.Free;
  Dispose(Box);
  break;
  end;
  end;
  end;
}

procedure TBoxsList.LoadFromFile();
var
  sFileName, sBoxFileName: string;
  TempList: TStringList;
  I, Idx, nStr: Integer;
  Box: pTBox;
begin
  Idx := 0;
  for I := 0 to BoxList.Count - 1 do
  begin
    Box := BoxList.Items[I];
    Box.Give.Free;
    Box.Center.Free;
    Box.NoGive.Free;
    Box.EndNoGive.Free;
    Dispose(Box);
  end;
  BoxList.Clear;

  if not DirectoryExists(g_Config.sBoxsDir) then
  begin // 目录不存在,则创建
    CreateDir(g_Config.sBoxsDir);
  end;

  if not FileExists(g_Config.sBoxsFile) then
  begin // BoxsList.txt文件不存在,则创建文件
    TempList := TStringList.Create;
    TempList.Add(';宝箱设置:StdMode=31 Shape=15--19(15=檀木宝箱,16=紫铜宝箱,17=白银宝箱,18=赤金宝箱,19=黄金宝箱 20-24=扩展的5个宝箱)');
    TempList.Add(';钥匙设置:StdMode=40 Shape=15--24');
    TempList.Add(';Source值对应宝箱开启对应的X.txt 例：赤金宝箱Source值为4，开启后对应4.txt的物品');
    TempList.SaveToFile(g_Config.sBoxsFile);
    TempList.Free;
  end;

  sFileName := g_Config.sBoxsFile;
  if not FileExists(sFileName) then
    Exit;

  TempList := TStringList.Create;
  try
    TempList.LoadFromFile(sFileName);
    for I := 0 to TempList.Count - 1 do
    begin
      nStr := Str_ToInt(TempList[I], -1);
      if nStr > -1 then
      begin
        if Idx = nStr then
        begin
          sBoxFileName := Format('%s%d.txt', [g_Config.sBoxsDir, nStr]);
          if not LoadBoxsItemList(sBoxFileName, nStr) then
          begin
            // Result := -(nStr + 1);
            Exit;
          end;
          Inc(Idx);
        end
        else
        begin
          // Result := -(nStr + 1);
          Exit;
        end;
      end;
    end;
  finally
    TempList.Free;
  end;
end;

function TBoxsList.LoadBoxsItemList(const sFileName: string; Idx: Integer): Boolean;
var
  TempList: TStringList;
  I, nIdx, nCount: Integer;
  Box: pTBox;
  sMsg, sName, sCount, sTemp: string;
  BoxItem: pTBoxItem;
begin
  Result := False;
  if FileExists(sFileName) then
  begin
    TempList := TStringList.Create;
    New(Box);
    Box.Name := ''; // StdItem.Name;
    // Box.Shape := 0; //StdItem.Shape;
    Box.Source := Idx;
    Box.Give := TBoxList.Create;
    Box.Center := TBoxList.Create;
    Box.NoGive := TBoxList.Create;
    Box.EndNoGive := TBoxList.Create;
    Box.BoxSet.nNowGold := 0;
    Box.BoxSet.nNowGameGold := 0;
    Box.BoxSet.nNowCount := 0;
    try
      TempList.LoadFromFile(sFileName);
      if TempList.Count > 0 then
      begin
        sMsg := TempList[0];
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.boNext := sName = '1';
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nGameGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nAddGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nAddGameGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nEndGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nEndGameGold := Str_ToInt(sName, -1);
        sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
        Box.BoxSet.nCount := Str_ToInt(sName, -1);
        {
          sGameDiamondName: '金刚石';                                                                     // 金刚石
          sGameGirdName: '灵符';                                                                          // 灵符
          sCreditPointName: '声望';                                                                       // 声望
        }
        for I := 1 to TempList.Count - 1 do
        begin
          sMsg := TempList[I];
          if (Length(sMsg) > 0) and (sMsg[1] <> ';') then
          begin
            sMsg := GetValidStr3(sMsg, sName, [' ', '/', #9]);
            sTemp := GetValidStr3(sMsg, sMsg, [' ', '/', #9]);

            // 宝箱物品支持物品数量 #  chongchong 2014-01-06
            // nCount := 0;
            if CompareLStr(sName, '经验' + '(', Length('经验') + 1) then
            begin
              ArrestStringEx(sName, '(', ')', sCount);
              nCount := Str_ToInt(sCount, 0);
              sName := '经验';
            end
            else if CompareLStr(sName, g_Config.sCreditPointName + '(', Length(g_Config.sCreditPointName) + 1) then
            begin
              ArrestStringEx(sName, '(', ')', sCount);
              nCount := Str_ToInt(sCount, 0);
              sName := g_Config.sCreditPointName;
            end
            else if CompareLStr(sName, g_Config.sGameDiamondName + '(', Length(g_Config.sGameDiamondName) + 1) then
            begin
              ArrestStringEx(sName, '(', ')', sCount);
              nCount := Str_ToInt(sCount, 0);
              sName := g_Config.sGameDiamondName;
            end
            else
            begin
              if UserEngine.GetStdItem(sName) = nil then
              begin
                MainOutMessage(Format('宝箱[%d] 物品[%s] 不存在.', [Idx, sName]), False);
                Continue;
              end;

              // 宝箱物品支持物品数量 +  chongchong 2014-01-06
              nCount := Str_ToInt(sTemp, 1);
            end;
            // 宝物类型
            nIdx := Str_ToInt(sMsg, 0);
            New(BoxItem);
            BoxItem.ItemName := sName;
            // BoxItem.ItemRate := 0;
            BoxItem.ItemCount := nCount;
            case nIdx of
              0:
                Box.Give.Add(BoxItem);
              1:
                Box.NoGive.Add(BoxItem);
              2:
                Box.Center.Add(BoxItem);
            else
              // Dispose(BoxItem);
              Box.EndNoGive.Add(BoxItem);
            end;
          end;
        end;
      end;
    finally
      TempList.Free;
    end;

    if BoxList.Add(Box) = Idx then
      Result := True;
  end;
end;

procedure TBoxsList.SaveToFile;

  function MakeNewName(BoxItem: pTBoxItem): string;
  var
    sItemName: string;
    sItemCount: Integer;
  const
    CON_NEWNAME = '%s(%d)';
  begin
    sItemName := BoxItem.ItemName;
    sItemCount := BoxItem.ItemCount;

    if CompareLStr(sItemName, '经验', Length('经验')) then
    begin
      Result := Format(CON_NEWNAME, [sItemName, sItemCount])
    end
    else if CompareLStr(sItemName, g_Config.sCreditPointName, Length(g_Config.sCreditPointName)) then
    begin
      Result := Format(CON_NEWNAME, [sItemName, sItemCount])
    end
    else if CompareLStr(sItemName, g_Config.sGameDiamondName, Length(g_Config.sGameDiamondName)) then
    begin
      Result := Format(CON_NEWNAME, [sItemName, sItemCount])
    end
    else
    begin
      // 宝箱物品支持物品数量 +  chongchong 2014-01-06
      Result := sItemName;
    end;
  end;

var
  I, II: Integer;
  BoxItem: pTBoxItem;
  Box: pTBox;
  SaveList: TStringList;
  sFileName: string;
  s: string;
const
  FileName = '%s%d.txt';
  CON_BOXSET = '%S'#9'%d'#9'%d'#9'%d'#9'%d'#9'%d'#9'%d'#9'%d'#9;
begin
  // sFileName := g_Config.sEnvirDir + 'BoxsList.txt';
  SaveList := TStringList.Create;
  try
    for I := 0 to BoxList.Count - 1 do
    begin
      Box := BoxList.Items[I];
      sFileName := Format(FileName, [g_Config.sBoxsDir, Box.Source]);
      SaveList.Clear;

      /// //////////////////////////宝箱设置/////////////////////////////////////
      s := Format(CON_BOXSET, [IntToStr(Integer(Box.BoxSet.boNext)), Box.BoxSet.nGold, Box.BoxSet.nGameGold, Box.BoxSet.nAddGold,
        Box.BoxSet.nAddGameGold, Box.BoxSet.nEndGold, Box.BoxSet.nEndGameGold, Box.BoxSet.nCount]);
      SaveList.Add(s);

      /// //////////////////////////宝箱详细/////////////////////////////////////
      for II := 0 to Box.Give.Count - 1 do
      begin
        BoxItem := Box.Give.Items[II];
        SaveList.Add(MakeNewName(BoxItem) + #9 + '0' + #9 + IntToStr(BoxItem.ItemCount));
      end;
      for II := 0 to Box.NoGive.Count - 1 do
      begin
        BoxItem := Box.NoGive.Items[II];
        SaveList.Add(MakeNewName(BoxItem) + #9 + '1' + #9 + IntToStr(BoxItem.ItemCount));
      end;
      for II := 0 to Box.Center.Count - 1 do
      begin
        BoxItem := Box.Center.Items[II];
        SaveList.Add(MakeNewName(BoxItem) + #9 + '2' + #9 + IntToStr(BoxItem.ItemCount));
      end;
      for II := 0 to Box.EndNoGive.Count - 1 do
      begin
        BoxItem := Box.EndNoGive.Items[II];
        SaveList.Add(MakeNewName(BoxItem) + #9 + '3' + #9 + IntToStr(BoxItem.ItemCount));
      end;
      SaveList.SaveToFile(sFileName);
    end;
  finally
    SaveList.Free;
  end;
end;

function TBoxsList.GetBoxsItem(nListIdx: Integer; nIdx: Byte; MakeIndex: Integer; ClientItem: pTClientItem): TBoxSet;
var
  List: TBoxList;
  Idx: Integer;
  sName: string;
  StdItem: pTStdItem;
  Box: pTBox;
  BoxItem: pTBoxItem;
begin
  try
    ZeroMemory(ClientItem, SizeOf(TClientItem));
    if nListIdx < BoxList.Count then
    begin
      Box := BoxList.Items[nListIdx];
      Result := Box.BoxSet;
      case nIdx of
        0:
          List := Box.Give;
        1:
          List := Box.NoGive;
        2:
          List := Box.Center;
      else
        List := Box.EndNoGive;
      end;

      if List.Count > 0 then
      begin
        Idx := Random(List.Count);
        BoxItem := List.Items[Idx];
        sName := BoxItem.ItemName;
        ClientItem.MakeIndex := MakeIndex; // 修改 chongchongn 2015-06-10 ClientItem.MakeIndex := nIdx;

        if CompareStr(sName, '经验') = 0 then
        begin
          ClientItem.s.Name := sName;
          ClientItem.s.StdMode := 255;
          ClientItem.s.Looks := 1186;
          ClientItem.s.Color := 255;
          ClientItem.s.Price := BoxItem.ItemCount;
        end
        else if CompareStr(sName, '声望') = 0 then
        begin
          ClientItem.s.Name := g_Config.sCreditPointName;
          ClientItem.s.StdMode := 255;
          ClientItem.s.Looks := 1185;
          ClientItem.s.Color := 255;
          ClientItem.s.Price := BoxItem.ItemCount;
        end
        else if CompareStr(sName, '金刚石') = 0 then
        begin
          ClientItem.s.Name := g_Config.sGameDiamondName;
          ClientItem.s.StdMode := 255;
          ClientItem.s.Looks := 1187;
          ClientItem.s.Color := 255;
          ClientItem.s.Price := BoxItem.ItemCount;
        end
        else
        begin
          StdItem := UserEngine.GetStdItem(sName);
          if StdItem <> nil then
          begin
            ClientItem.s := StdItem^;
            { StdItem.GetStandardItem(ClientItem.S);
              ClientItem.Desc := StdItem.sDesc;
              ClientItem.Shine := 0; }
          end;
        end;

        // 宝箱叠加物品数量错误 chongchong 2014-06-03
        if CheckOverLapItem(@ClientItem.s) then
        begin
          ClientItem.Dura := 0;
          ClientItem.DuraMax := ClientItem.s.DuraMax;

          // 宝箱物品支持物品数量 +  chongchong 2014-01-06
          ClientItem.s.Price := BoxItem.ItemCount;
        end
        else
        begin
          ClientItem.Dura := ClientItem.s.DuraMax;
          ClientItem.DuraMax := ClientItem.s.DuraMax;

          // 宝箱物品支持物品数量 +  chongchong 2014-01-06
          ClientItem.s.Price := BoxItem.ItemCount;
        end;
      end;
    end;
  except
    MainOutMessage('[Exception] TBoxsList:GetBoxsItem');
  end;
end;

end.
