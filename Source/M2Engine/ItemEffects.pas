unit ItemEffects;

interface

uses
  Windows, Classes, SysUtils;

type
  TItemEffect = record
    Index: Word;

    FileIndex1: SmallInt; // 内观
    FileIndex2: SmallInt; // 外观
    FileIndex3: SmallInt; // 包裹
    FileIndex4: SmallInt; // 外观附加
    FileIndex5: SmallInt; // 地面特效

    StartIndex1: Word; // 内观
    StartIndex2: Word; // 外观
    StartIndex3: Word; // 包裹
    StartIndex4: Word; // 外观附加
    StartIndex5: Word; // 地面特效

    ImageCount1: Word; // 内观
    ImageCount2: Word; // 外观
    ImageCount3: Word; // 包裹
    ImageCount4: Word; // 外观附加
    ImageCount5: Word; // 地面特效

    Time1: Word; // 内观
    Time2: Word; // 外观
    Time3: Word; // 包裹
    Time4: Word; // 外观附加
    Time5: Word; // 地面特效

    OffSetX1: SmallInt; // 内观
    OffSetY1: SmallInt; // 内观

    OffSetX2: SmallInt; // 外观 - 废字段
    OffSetY2: SmallInt; // 外观 - 废字段

    OffSetX3: SmallInt; // 包裹
    OffSetY3: SmallInt; // 包裹

    OffSetX5: SmallInt; // 地面特效
    OffSetY5: SmallInt; // 地面特效

    NoBlendMode2: Boolean; // 外观 - 不透明模式
    NoSex2: Boolean; // 外观 - 不分男女
    // DressEffect: Boolean;                                                                         // 外观 - 衣服特效

    DrawCenter1: Boolean; // 居中对齐 内观
    DrawCenter3: Boolean; // 居中对齐 包裹
    DrawCenter5: Boolean; // 居中对齐 地面特效

    NoBlendMode1: Boolean; // 不透明模式 内观
    NoBlendMode3: Boolean; // 不透明模式 包裹
    NoBlendMode5: Boolean; // 不透明模式 地面特效

    boEfectBelowItem1: Boolean; // 物效在物品底层播放 内观
    boEfectBelowItem5: Boolean; // 物效在物品底层播放 地面特效

    AddEffectDrawOrder: Integer; // 附加特效绘制顺序
    AddEffectNoBlendMode: Boolean; // 附加特效普通绘制
    AddEffectDrawCenter: Boolean; // 附加特效居中绘制

    EffectDesc: string;
  end;

  pTItemEffect = ^TItemEffect;

  TItemEffects = class
  private
    FRecordCount: Integer;
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTItemEffect;
  public
    constructor Create();
    destructor Destroy; override;
    function Find(const Index: Integer): Boolean;
    function Get(const Index: Integer): pTItemEffect;
    function Add(ItemEffect: pTItemEffect): Boolean;
    function Delete(ItemEffect: pTItemEffect): Boolean;
    function DeleteIndex(Index: Integer): Boolean;
    procedure LoadFromFile;
    procedure SaveToFile;
    property Items[Index: Integer]: pTItemEffect read GetItems;
    property Count: Integer read GetCount;
    property RecordCount: Integer read FRecordCount;
  end;

implementation

uses
  M2Share, HUtil32;

constructor TItemEffects.Create();
begin
  FList := TList.Create;
  FRecordCount := 0;
end;

destructor TItemEffects.Destroy;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(pTItemEffect(FList.Items[I]));
  end;
  FList.Free;
  inherited;
end;

function TItemEffects.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TItemEffects.GetItems(Index: Integer): pTItemEffect;
begin
  Result := FList.Items[Index];
end;

procedure TItemEffects.LoadFromFile;
var
  I: Integer;
  sFileName: string;
  sLineText: string;
  sIndex: string;
  nIndex: Integer;
  sFileIndex1: string; // 内观
  sFileIndex2: string; // 外观
  sFileIndex3: string; // 包裹
  sFileIndex4: string;
  sFileIndex5: string; // 地上

  sOffSet1: string; // 内观
  sOffSet2: string; // 外观
  sOffSet3: string; // 包裹
  sOffset4: string;
  sOffset5: string; // 地上

  sImageCount1: string; // 内观
  sImageCount2: string; // 外观
  sImageCount3: string; // 包裹
  sImageCount4: string;
  sImageCount5: string; // 地上

  sTime1: string; // 内观
  sTime2: string; // 外观
  sTime3: string; // 包裹
  sTime4: string;
  sTime5: string; // 地上

  sOffSetX1: string; // 内观
  sOffSetY1: string; // 内观

  sOffSetX2: string; // 外观
  sOffSetY2: string; // 外观

  sOffSetX3: string; // 包裹
  sOffSetY3: string; // 包裹

  sOffSetX5: string; // 地上
  sOffSetY5: string; // 地上

  sNoBlendMode1, sNoBlendMode2, sNoBlendMode3, sNoBlendMode5, sEfectBelowItem1, sEfectBelowItem5: string;
  sNoSex2: string;
  sDrawCenter1: string;
  sDrawCenter3: string;
  sDrawCenter5: string;
  sAddEffectDrawOrder: string;
  sAddEffectNoBlendMode: string;
  sAddEffectDrawCenter: string;
  FileIndex1: Integer; // 内观
  FileIndex2: Integer; // 外观
  FileIndex3: Integer; // 包裹
  FileIndex4: Integer;
  FileIndex5: Integer;
  OffSet1: Integer; // 内观
  OffSet2: Integer; // 外观
  OffSet3: Integer; // 包裹
  Offset4: Integer;
  ImageCount1: Integer; // 内观
  ImageCount2: Integer; // 外观
  ImageCount3: Integer; // 包裹
  ImageCount4: Integer;
  Time1: Integer; // 内观
  Time2: Integer; // 外观
  Time3: Integer; // 包裹
  Time4: Integer;
  OffSetX1: Integer; // 内观
  OffSetY1: Integer; // 内观

  OffSetX2: Integer; // 外观
  OffSetY2: Integer; // 外观

  OffSetX3: Integer; // 包裹
  OffSetY3: Integer; // 包裹

  LoadList: TStringList;
  ItemEffect: pTItemEffect;
begin
  FRecordCount := 0;
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(pTItemEffect(FList.Items[I]));
  end;
  FList.Clear;

  sFileName := g_Config.sEnvirDir + 'EffectList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sIndex, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sFileIndex1, [' ', #9]); // 内观
        sLineText := GetValidStr3(sLineText, sFileIndex2, [' ', #9]); // 外观
        sLineText := GetValidStr3(sLineText, sFileIndex3, [' ', #9]); // 包裹

        sLineText := GetValidStr3(sLineText, sOffSet1, [' ', #9]); // 内观
        sLineText := GetValidStr3(sLineText, sOffSet2, [' ', #9]); // 外观
        sLineText := GetValidStr3(sLineText, sOffSet3, [' ', #9]); // 包裹

        sLineText := GetValidStr3(sLineText, sImageCount1, [' ', #9]); // 内观
        sLineText := GetValidStr3(sLineText, sImageCount2, [' ', #9]); // 外观
        sLineText := GetValidStr3(sLineText, sImageCount3, [' ', #9]); // 包裹

        sLineText := GetValidStr3(sLineText, sTime1, [' ', #9]); // 内观
        sLineText := GetValidStr3(sLineText, sTime2, [' ', #9]); // 外观
        sLineText := GetValidStr3(sLineText, sTime3, [' ', #9]); // 包裹

        sLineText := GetValidStr3(sLineText, sOffSetX1, [' ', #9]); // 内观
        sLineText := GetValidStr3(sLineText, sOffSetY1, [' ', #9]); // 内观

        sLineText := GetValidStr3(sLineText, sOffSetX2, [' ', #9]); // 外观
        sLineText := GetValidStr3(sLineText, sOffSetY2, [' ', #9]); // 外观

        sLineText := GetValidStr3(sLineText, sOffSetX3, [' ', #9]); // 包裹
        sLineText := GetValidStr3(sLineText, sOffSetY3, [' ', #9]); // 包裹

        sLineText := GetValidStr3(sLineText, sNoBlendMode2, [' ', #9]); // 不透明绘制
        sLineText := GetValidStr3(sLineText, sNoSex2, [' ', #9]); // 不分男女

        sLineText := GetValidStr3(sLineText, sDrawCenter1, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sDrawCenter3, [' ', #9]);

        sLineText := GetValidStr3(sLineText, sFileIndex4, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAddEffectDrawOrder, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sOffset4, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sImageCount4, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sTime4, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAddEffectNoBlendMode, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAddEffectDrawCenter, [' ', #9]);

        sLineText := GetValidStr3(sLineText, sNoBlendMode1, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sNoBlendMode3, [' ', #9]);

        sLineText := GetValidStr3(sLineText, sEfectBelowItem1, [' ', #9]);

        sLineText := GetValidStr3(sLineText, sFileIndex5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sOffset5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sImageCount5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sTime5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sOffSetX5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sOffSetY5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sDrawCenter5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sNoBlendMode5, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sEfectBelowItem5, [' ', #9]);

        nIndex := StrToIntDef(sIndex, -1);
        FileIndex1 := StrToIntDef(sFileIndex1, -1); // 内观
        FileIndex2 := StrToIntDef(sFileIndex2, -1); // 外观
        FileIndex3 := StrToIntDef(sFileIndex3, -1); // 包裹

        OffSet1 := StrToIntDef(sOffSet1, -1); // 内观
        OffSet2 := StrToIntDef(sOffSet2, -1); // 外观
        OffSet3 := StrToIntDef(sOffSet3, -1); // 包裹

        ImageCount1 := StrToIntDef(sImageCount1, -1); // 内观
        ImageCount2 := StrToIntDef(sImageCount2, -1); // 外观
        ImageCount3 := StrToIntDef(sImageCount3, -1); // 包裹

        Time1 := StrToIntDef(sTime1, 1); // 内观
        Time2 := StrToIntDef(sTime2, 1); // 外观
        Time3 := StrToIntDef(sTime3, 1); // 包裹

        OffSetX1 := StrToIntDef(sOffSetX1, -1); // 内观
        OffSetY1 := StrToIntDef(sOffSetY1, -1); // 内观

        OffSetX2 := StrToIntDef(sOffSetX2, -1); // 外观
        OffSetY2 := StrToIntDef(sOffSetY2, -1); // 外观

        OffSetX3 := StrToIntDef(sOffSetX3, -1); // 包裹
        OffSetY3 := StrToIntDef(sOffSetY3, -1); // 包裹

        // 附加外观
        FileIndex4 := StrToIntDef(sFileIndex4, -1);
        Offset4 := StrToIntDef(sOffset4, -1);
        ImageCount4 := StrToIntDef(sImageCount4, 0);
        Time4 := StrToIntDef(sTime4, 1);

        FileIndex5 := StrToIntDef(sFileIndex5, -1);
        // 修正添加特效物品后，什么都没有指定，重启M2后没有了 2020-03-14 22:27:47
        if (nIndex > 0)
        { and ((FileIndex1 >= 0) or (FileIndex2 >= 0) or (FileIndex3 >= 0) or (FileIndex4 >= 0) or (FileIndex5 >= 0)) } then
        begin
          New(ItemEffect);
          FillChar(ItemEffect^, SizeOf(TItemEffect), 0);
          ItemEffect.Index := nIndex;

          ItemEffect.FileIndex1 := FileIndex1; // 内观
          ItemEffect.FileIndex2 := FileIndex2; // 外观
          ItemEffect.FileIndex3 := FileIndex3; // 包裹
          ItemEffect.FileIndex4 := FileIndex4;

          ItemEffect.StartIndex1 := OffSet1; // 内观
          ItemEffect.StartIndex2 := OffSet2; // 外观
          ItemEffect.StartIndex3 := OffSet3; // 包裹
          ItemEffect.StartIndex4 := Offset4;

          ItemEffect.ImageCount1 := ImageCount1; // 内观
          ItemEffect.ImageCount2 := ImageCount2; // 外观
          ItemEffect.ImageCount3 := ImageCount3; // 包裹
          ItemEffect.ImageCount4 := ImageCount4;

          ItemEffect.Time1 := Time1; // 内观
          ItemEffect.Time2 := Time2; // 外观
          ItemEffect.Time3 := Time3; // 包裹
          ItemEffect.Time4 := Time4;

          ItemEffect.OffSetX1 := OffSetX1; // 内观
          ItemEffect.OffSetY1 := OffSetY1; // 内观

          ItemEffect.OffSetX2 := OffSetX2; // 外观
          ItemEffect.OffSetY2 := OffSetY2; // 外观

          ItemEffect.OffSetX3 := OffSetX3; // 包裹
          ItemEffect.OffSetY3 := OffSetY3; // 包裹

          ItemEffect.NoBlendMode2 := StrToIntDef(sNoBlendMode2, 0) <> 0;
          ItemEffect.NoSex2 := StrToIntDef(sNoSex2, 0) <> 0;

          ItemEffect.DrawCenter1 := StrToIntDef(sDrawCenter1, 0) <> 0;
          ItemEffect.DrawCenter3 := StrToIntDef(sDrawCenter3, 0) <> 0;

          ItemEffect.NoBlendMode1 := StrToIntDef(sNoBlendMode1, 0) <> 0;
          ItemEffect.NoBlendMode3 := StrToIntDef(sNoBlendMode3, 0) <> 0;

          ItemEffect.boEfectBelowItem1 := StrToIntDef(sEfectBelowItem1, 0) <> 0;
          ItemEffect.boEfectBelowItem5 := StrToIntDef(sEfectBelowItem5, 0) <> 0;

          ItemEffect.AddEffectDrawOrder := StrToIntDef(sAddEffectDrawOrder, 0);
          ItemEffect.AddEffectNoBlendMode := StrToIntDef(sAddEffectNoBlendMode, 0) <> 0;
          ItemEffect.AddEffectDrawCenter := StrToIntDef(sAddEffectDrawCenter, 0) <> 0;

          ItemEffect.EffectDesc := sLineText;

          ItemEffect.FileIndex5 := FileIndex5;
          ItemEffect.StartIndex5 := StrToIntDef(sOffset5, 0);
          ItemEffect.ImageCount5 := StrToIntDef(sImageCount5, 1);
          ItemEffect.Time5 := StrToIntDef(sTime5, 1);
          ItemEffect.OffSetX5 := StrToIntDef(sOffSetX5, 0);
          ItemEffect.OffSetY5 := StrToIntDef(sOffSetY5, 0);
          ItemEffect.DrawCenter5 := StrToIntDef(sDrawCenter5, 0) <> 0;
          ItemEffect.NoBlendMode5 := StrToIntDef(sNoBlendMode5, 0) <> 0;

          // 加载时去掉重复 2020-03-15 14:25:03
          if not Add(ItemEffect) then
          begin
            Dispose(ItemEffect);
          end;

          // FList.Add(ItemEffect);
          // Inc(FRecordCount);
        end;
      end;
    end;
    LoadList.Free;
  end;
end;

procedure TItemEffects.SaveToFile;
var
  I: Integer;
  sFileName: string;
  SaveList: TStringList;
  ItemEffect: pTItemEffect;
begin
  SaveList := TStringList.Create;
  try
    for I := 0 to FList.Count - 1 do
    begin
      ItemEffect := FList.Items[I];

      SaveList.Add(IntToStr(ItemEffect.Index) + #9 + IntToStr(ItemEffect.FileIndex1) + #9 + IntToStr(ItemEffect.FileIndex2) + #9 +
        IntToStr(ItemEffect.FileIndex3) + #9 + IntToStr(ItemEffect.StartIndex1) + #9 + IntToStr(ItemEffect.StartIndex2) + #9 +
        IntToStr(ItemEffect.StartIndex3) + #9 + IntToStr(ItemEffect.ImageCount1) + #9 + IntToStr(ItemEffect.ImageCount2) + #9 +
        IntToStr(ItemEffect.ImageCount3) + #9 + IntToStr(ItemEffect.Time1) + #9 + IntToStr(ItemEffect.Time2) + #9 +
        IntToStr(ItemEffect.Time3) + #9 + IntToStr(ItemEffect.OffSetX1) + #9 + IntToStr(ItemEffect.OffSetY1) + #9 +
        IntToStr(ItemEffect.OffSetX2) + #9 + IntToStr(ItemEffect.OffSetY2) + #9 + IntToStr(ItemEffect.OffSetX3) + #9 +
        IntToStr(ItemEffect.OffSetY3) + #9 + IntToStr(Integer(ItemEffect.NoBlendMode2)) + #9 + IntToStr(Integer(ItemEffect.NoSex2)
        ) + #9 + IntToStr(Integer(ItemEffect.DrawCenter1)) + #9 + IntToStr(Integer(ItemEffect.DrawCenter3)) + #9 +
        IntToStr(ItemEffect.FileIndex4) + #9 + IntToStr(ItemEffect.AddEffectDrawOrder) + #9 + IntToStr(ItemEffect.StartIndex4) +
        #9 + IntToStr(ItemEffect.ImageCount4) + #9 + IntToStr(ItemEffect.Time4) + #9 +
        IntToStr(Integer(ItemEffect.AddEffectNoBlendMode)) + #9 + IntToStr(Integer(ItemEffect.AddEffectDrawCenter)) + #9 +
        IntToStr(Integer(ItemEffect.NoBlendMode1)) + #9 + IntToStr(Integer(ItemEffect.NoBlendMode3)) + #9 +
        IntToStr(Integer(ItemEffect.boEfectBelowItem1)) + #9 + IntToStr(ItemEffect.FileIndex5) + #9 +
        IntToStr(ItemEffect.StartIndex5) + #9 + IntToStr(ItemEffect.ImageCount5) + #9 + IntToStr(ItemEffect.Time5) + #9 +
        IntToStr(ItemEffect.OffSetX5) + #9 + IntToStr(ItemEffect.OffSetY5) + #9 + IntToStr(Integer(ItemEffect.DrawCenter5)) + #9 +
        IntToStr(Integer(ItemEffect.NoBlendMode5)) + #9 + IntToStr(Integer(ItemEffect.boEfectBelowItem5)) + #9 +
        ItemEffect.EffectDesc);
    end;

    sFileName := g_Config.sEnvirDir + 'EffectList.txt';
    try
      SaveList.SaveToFile(sFileName);
    except
    end;
  finally
    SaveList.Free;
  end;
end;

function TItemEffects.Find(const Index: Integer): Boolean;
var
  I: Integer;
  ItemEffect: pTItemEffect;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    ItemEffect := pTItemEffect(FList.Items[I]);
    if (ItemEffect.Index = Index) then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TItemEffects.Get(const Index: Integer): pTItemEffect;
var
  I: Integer;
  ItemEffect: pTItemEffect;
begin
  Result := nil;
  for I := 0 to FList.Count - 1 do
  begin
    ItemEffect := pTItemEffect(FList.Items[I]);
    if (ItemEffect.Index = Index) then
    begin
      Result := ItemEffect;
      Break;
    end;
  end;
end;

function TItemEffects.Add(ItemEffect: pTItemEffect): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    if pTItemEffect(FList.Items[I]).Index = ItemEffect.Index then
    begin
      Exit;
    end;
  end;

  FList.Add(ItemEffect);
  Inc(FRecordCount);
  Result := True;
end;

function TItemEffects.Delete(ItemEffect: pTItemEffect): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    if FList.Items[I] = ItemEffect then
    begin
      FList.Delete(I);
      Dispose(ItemEffect);
      Dec(FRecordCount);
      Result := True;
      Break;
    end;
  end;
end;

function TItemEffects.DeleteIndex(Index: Integer): Boolean;
begin
  Result := False;
  if (Index >= 0) and (Index <= FList.Count - 1) then
  begin
    Dispose(pTItemEffect(FList.Items[Index]));
    FList.Delete(Index);
    Dec(FRecordCount);
    Result := True;
  end;
end;

end.
