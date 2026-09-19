unit ViewHeroRcd;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics,
  Controls, Forms, Dialogs, StdCtrls, TabNotBk, Grids, ExtCtrls, Buttons,
  ComCtrls, Grobal2, DBShare, HUtil32;
type
  TFrmHeroFDBViewer = class(TForm)
    PageControlHero: TPageControl;
    TabSheet3: TTabSheet;
    HumanGrid: TStringGrid;
    TabSheet4: TTabSheet;
    PageControlHeroJob0: TPageControl;
    TabSheet7: TTabSheet;
    UserItemGrid0: TStringGrid;
    TabSheet8: TTabSheet;
    BagItemGrid0: TStringGrid;
    TabSheet9: TTabSheet;
    UseMagicGrid0: TStringGrid;
    TabSheet5: TTabSheet;
    PageControlHeroJob1: TPageControl;
    TabSheet10: TTabSheet;
    UserItemGrid1: TStringGrid;
    TabSheet11: TTabSheet;
    BagItemGrid1: TStringGrid;
    TabSheet12: TTabSheet;
    UseMagicGrid1: TStringGrid;
    TabSheet6: TTabSheet;
    PageControlHeroJob2: TPageControl;
    TabSheet13: TTabSheet;
    UserItemGrid2: TStringGrid;
    TabSheet14: TTabSheet;
    BagItemGrid2: TStringGrid;
    TabSheet15: TTabSheet;
    UseMagicGrid2: TStringGrid;
    TabSheet1: TTabSheet;
    UseSuccessiveMagicGrid: TStringGrid;
    procedure FormCreate(Sender: TObject);
  private
    procedure sub_49A0C0();
    procedure sub_49A9DC();
    procedure sub_49AB10();
    procedure InitUserItemGrid;
    procedure ShowUserItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);
    procedure ShowBagItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);
    procedure ShowUserItems();
    procedure ShowBagItems();
    procedure ShowUseMagic();
    procedure ShowHumanInfo();
    function GetUserItemGrid(Job: Integer): TStringGrid;
    function GetBagItemGrid(Job: Integer): TStringGrid;
    function GetUseMagicGrid(Job: Integer): TStringGrid;
  public
    n2F8: Integer;
    s2FC: string;
    //HeroRecord: THeroFileDataInfo;
    procedure ShowHumData();

    property UserItemGrid[Job: Integer]: TStringGrid read GetUserItemGrid;
    property BagItemGrid[Job: Integer]: TStringGrid read GetBagItemGrid;
    property UseMagicGrid[Job: Integer]: TStringGrid read GetUseMagicGrid;
  end;

var
  FrmHeroFDBViewer: TFrmHeroFDBViewer;

implementation
{$R *.DFM}

procedure TFrmHeroFDBViewer.FormCreate(Sender: TObject);
begin
  sub_49A0C0();
  sub_49A9DC();
  sub_49AB10();
  InitUserItemGrid;
end;

procedure TFrmHeroFDBViewer.ShowHumData();
begin
  ShowHumanInfo();
  ShowUserItems();
  ShowBagItems();
  ShowUseMagic();
  PageControlHero.ActivePageIndex := 0;
  PageControlHeroJob0.ActivePageIndex := 0;
  PageControlHeroJob1.ActivePageIndex := 0;
  PageControlHeroJob2.ActivePageIndex := 0;
end;

function TFrmHeroFDBViewer.GetUserItemGrid(Job: Integer): TStringGrid;
begin
  case Job of
    1: Result := UserItemGrid1;
    2: Result := UserItemGrid2;
  else
    Result := UserItemGrid0;
  end;
end;

function TFrmHeroFDBViewer.GetBagItemGrid(Job: Integer): TStringGrid;
begin
  case Job of
    1: Result := BagItemGrid1;
    2: Result := BagItemGrid2;
  else
    Result := BagItemGrid0;
  end;
end;

function TFrmHeroFDBViewer.GetUseMagicGrid(Job: Integer): TStringGrid;
begin
  case Job of
    1: Result := UseMagicGrid1;
    2: Result := UseMagicGrid2;
  else
    Result := UseMagicGrid0;
  end;
end;

procedure TFrmHeroFDBViewer.sub_49A0C0();
begin
  HumanGrid.Cells[0, 1] := '索引号';
  HumanGrid.Cells[1, 1] := '名称';
  HumanGrid.Cells[2, 1] := '地图';
  HumanGrid.Cells[3, 1] := 'CX';
  HumanGrid.Cells[4, 1] := 'CY';
  HumanGrid.Cells[5, 1] := '方向';
  HumanGrid.Cells[6, 1] := '职业';
  HumanGrid.Cells[7, 1] := '性别';
  HumanGrid.Cells[8, 1] := '头发';
  HumanGrid.Cells[9, 1] := '金币数';
  HumanGrid.Cells[10, 1] := '主人名称';
  HumanGrid.Cells[11, 1] := 'Home';

  HumanGrid.Cells[0, 3] := 'HomeX';
  HumanGrid.Cells[1, 3] := 'HomeY';
  HumanGrid.Cells[2, 3] := '等级';
  HumanGrid.Cells[3, 3] := 'AC';
  HumanGrid.Cells[4, 3] := 'MAC';
  HumanGrid.Cells[5, 3] := 'Reserved1';
  HumanGrid.Cells[6, 3] := 'DC/1';
  HumanGrid.Cells[7, 3] := 'DC/2';
  HumanGrid.Cells[8, 3] := 'MC/1';
  HumanGrid.Cells[9, 3] := 'MC/2';
  HumanGrid.Cells[10, 3] := 'SC/1';
  HumanGrid.Cells[11, 3] := 'SC/2';

  HumanGrid.Cells[0, 5] := 'Reserved2';
  HumanGrid.Cells[1, 5] := 'HP';
  HumanGrid.Cells[2, 5] := 'MaxHP';
  HumanGrid.Cells[3, 5] := 'MP';
  HumanGrid.Cells[4, 5] := 'MaxMP';
  HumanGrid.Cells[5, 5] := 'Reserved2';
  HumanGrid.Cells[6, 5] := '当前经验';
  HumanGrid.Cells[7, 5] := '升级经验';
  HumanGrid.Cells[8, 5] := 'PK点数';
  HumanGrid.Cells[9, 5] := '忠诚度';
  HumanGrid.Cells[10, 5] := '登录帐号';
  HumanGrid.Cells[11, 5] := '最后登录时间';

  HumanGrid.Cells[0, 7] := '修炼内功';
  HumanGrid.Cells[1, 7] := '修炼心法';
  HumanGrid.Cells[2, 7] := '内功等级';
  HumanGrid.Cells[3, 7] := '当前内力值';
  HumanGrid.Cells[4, 7] := '内力值上限';
  HumanGrid.Cells[5, 7] := '当前内功经验';
  HumanGrid.Cells[6, 7] := '内功最高经验';
  HumanGrid.Cells[7, 7] := '酒量';
  HumanGrid.Cells[8, 7] := '酒量上限';
  HumanGrid.Cells[9, 7] := '药力值';
  HumanGrid.Cells[10, 7] := '药力值上限';
  HumanGrid.Cells[11, 7] := '醉酒度';
end;

procedure TFrmHeroFDBViewer.InitUserItemGrid;
var
  I: Integer;
begin
  for I := 0 to 2 do
  begin
    UserItemGrid[I].Cells[0, 0] := '物品位置';
    UserItemGrid[I].Cells[1, 0] := '物品ID';
    UserItemGrid[I].Cells[2, 0] := '物品号';
    UserItemGrid[I].Cells[3, 0] := '持久';
    UserItemGrid[I].Cells[4, 0] := '物品名称';
    UserItemGrid[I].Cells[0, 1] := '衣服';
    UserItemGrid[I].Cells[0, 2] := '武器';
    UserItemGrid[I].Cells[0, 3] := '照明物';
    UserItemGrid[I].Cells[0, 4] := '项链';
    UserItemGrid[I].Cells[0, 5] := '头盔';
    UserItemGrid[I].Cells[0, 6] := '左手镯';
    UserItemGrid[I].Cells[0, 7] := '右手镯';
    UserItemGrid[I].Cells[0, 8] := '左戒指';
    UserItemGrid[I].Cells[0, 9] := '右戒指';
    UserItemGrid[I].Cells[0, 10] := '物品';
    UserItemGrid[I].Cells[0, 11] := '腰带';
    UserItemGrid[I].Cells[0, 12] := '鞋子';
    UserItemGrid[I].Cells[0, 13] := '宝石';
    UserItemGrid[I].Cells[0, 14] := '斗笠';
    UserItemGrid[I].Cells[0, 15] := '军鼓';
  end;
end;

procedure TFrmHeroFDBViewer.sub_49A9DC();
var
  I: Integer;
begin
  for I := 0 to 2 do
  begin
    BagItemGrid[I].Cells[0, 0] := '物品号';
    BagItemGrid[I].Cells[1, 0] := '物品ID';
    BagItemGrid[I].Cells[2, 0] := '物品号';
    BagItemGrid[I].Cells[3, 0] := '持久';
    BagItemGrid[I].Cells[4, 0] := '物品名称';
  end;
end;

procedure TFrmHeroFDBViewer.sub_49AB10();
var
  I: Integer;
begin
  for I := 0 to 2 do
  begin
    UseMagicGrid[I].Cells[0, 0] := '技能ID';
    UseMagicGrid[I].Cells[1, 0] := '快捷键';
    UseMagicGrid[I].Cells[2, 0] := '修练状态';
    UseMagicGrid[I].Cells[3, 0] := '技能名称';
    UseMagicGrid[I].Cells[4, 0] := '技能类型';
  end;
  UseSuccessiveMagicGrid.Cells[0, 0] := '技能ID';
  UseSuccessiveMagicGrid.Cells[1, 0] := '快捷键';
  UseSuccessiveMagicGrid.Cells[2, 0] := '修练状态';
  UseSuccessiveMagicGrid.Cells[3, 0] := '技能名称';
  UseSuccessiveMagicGrid.Cells[4, 0] := '技能类型';
end;

procedure TFrmHeroFDBViewer.ShowBagItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);
begin
  if Item.wIndex > 0 then
  begin
    BagItemGrid[nJob].Cells[0, nIndex] := sName;
    BagItemGrid[nJob].Cells[1, nIndex] := IntToStr(Item.MakeIndex);
    BagItemGrid[nJob].Cells[2, nIndex] := IntToStr(Item.wIndex);
    BagItemGrid[nJob].Cells[3, nIndex] := IntToStr(Item.Dura) + '/' + IntToStr(Item.DuraMax);
    BagItemGrid[nJob].Cells[4, nIndex] := GetStdItemName(Item.wIndex);
  end
  else
  begin
    BagItemGrid[nJob].Cells[0, nIndex] := sName;
    BagItemGrid[nJob].Cells[1, nIndex] := '';
    BagItemGrid[nJob].Cells[2, nIndex] := '';
    BagItemGrid[nJob].Cells[3, nIndex] := '';
    BagItemGrid[nJob].Cells[4, nIndex] := '';
  end;
end;

procedure TFrmHeroFDBViewer.ShowUserItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);
begin
  if Item.wIndex > 0 then
  begin
    UserItemGrid[nJob].Cells[1, nIndex] := IntToStr(Item.MakeIndex);
    UserItemGrid[nJob].Cells[2, nIndex] := IntToStr(Item.wIndex);
    UserItemGrid[nJob].Cells[3, nIndex] := IntToStr(Item.Dura) + '/' + IntToStr(Item.DuraMax);
    UserItemGrid[nJob].Cells[4, nIndex] := GetStdItemName(Item.wIndex);
  end
  else
  begin
    UserItemGrid[nJob].Cells[1, nIndex] := '';
    UserItemGrid[nJob].Cells[2, nIndex] := '';
    UserItemGrid[nJob].Cells[3, nIndex] := '';
    UserItemGrid[nJob].Cells[4, nIndex] := '';
  end;
end;

procedure TFrmHeroFDBViewer.ShowHumanInfo();
begin
(*
var
  HumData: pTHeroDataPublic;
begin
  HumData := @HeroRecord.PublicData;
  HumanGrid.Cells[0, 2] := IntToStr(n2F8);
  HumanGrid.Cells[1, 2] := HumData.sChrName;
  HumanGrid.Cells[2, 2] := HumData.sCurMap;
  HumanGrid.Cells[3, 2] := IntToStr(HumData.wCurX);
  HumanGrid.Cells[4, 2] := IntToStr(HumData.wCurY);
  HumanGrid.Cells[5, 2] := IntToStr(HumData.btDir);
  HumanGrid.Cells[6, 2] := IntToStr(HumData.btJob);
  HumanGrid.Cells[7, 2] := IntToStr(HumData.btSex);
  HumanGrid.Cells[8, 2] := IntToStr(HumData.btHair);
  HumanGrid.Cells[9, 2] := IntToStr(HumData.nGold);
  HumanGrid.Cells[10, 2] := HumData.sMasterName;
  HumanGrid.Cells[11, 2] := HumData.sHomeMap;

  HumanGrid.Cells[0, 4] := IntToStr(HumData.wHomeX);
  HumanGrid.Cells[1, 4] := IntToStr(HumData.wHomeY);
  HumanGrid.Cells[2, 4] := IntToStr(HumData.Abil.Level);
  HumanGrid.Cells[3, 4] := IntToStr(HumData.Abil.AC1) + '-' + IntToStr(HumData.Abil.AC2);
  HumanGrid.Cells[4, 4] := IntToStr(HumData.Abil.MAC1) + '-' + IntToStr(HumData.Abil.MAC2);
  //  HumanGrid.Cells[5,4]:=IntToStr(HumData.Abil.bt49);
  HumanGrid.Cells[6, 4] := IntToStr(HumData.Abil.DC1);
  HumanGrid.Cells[7, 4] := IntToStr(HumData.Abil.DC2);
  HumanGrid.Cells[8, 4] := IntToStr(HumData.Abil.MC1);
  HumanGrid.Cells[9, 4] := IntToStr(HumData.Abil.MC2);
  HumanGrid.Cells[10, 4] := IntToStr(HumData.Abil.SC1);
  HumanGrid.Cells[11, 4] := IntToStr(HumData.Abil.SC2);
  // HumanGrid.Cells[0,6]:=IntToStr(HumData.Abil.bt48);
  HumanGrid.Cells[1, 6] := IntToStr(HumData.Abil.HP);
  HumanGrid.Cells[2, 6] := IntToStr(HumData.Abil.HP);
  HumanGrid.Cells[3, 6] := IntToStr(HumData.Abil.MaxMP);
  HumanGrid.Cells[4, 6] := IntToStr(HumData.Abil.MaxMP);
  // HumanGrid.Cells[5,6]:=IntToStr(HumData.Abil.bt48);
  HumanGrid.Cells[6, 6] := IntToStr(HumData.Abil.Exp);
  HumanGrid.Cells[7, 6] := IntToStr(HumData.Abil.MaxExp);
  HumanGrid.Cells[8, 6] := IntToStr(HumData.nPKPoint);
  HumanGrid.Cells[9, 6] := Format('%.2f', [HumData.rLoyalPoint]) + '%';
  HumanGrid.Cells[10, 6] := HumData.sAccount;
  HumanGrid.Cells[11, 6] := DateTimeToStr(HeroRecord.Header.dCreateDate);

  HumanGrid.Cells[0, 8] := BooleanToStr(HumData.boTrainingNG);
  HumanGrid.Cells[1, 8] := BooleanToStr(HumData.boTrainingXF);
  HumanGrid.Cells[2, 8] := IntToStr(HumData.AbilNG.Level);
  HumanGrid.Cells[3, 8] := IntToStr(HumData.AbilNG.NH);
  HumanGrid.Cells[4, 8] := IntToStr(HumData.AbilNG.MaxNH);
  HumanGrid.Cells[5, 8] := IntToStr(HumData.AbilNG.Exp);
  HumanGrid.Cells[6, 8] := IntToStr(HumData.AbilNG.MaxExp);
  HumanGrid.Cells[7, 8] := IntToStr(HumData.Alcohol.Alcohol);
  HumanGrid.Cells[8, 8] := IntToStr(HumData.Alcohol.MaxAlcohol);
  HumanGrid.Cells[9, 8] := IntToStr(HumData.Alcohol.MedicineValue);
  HumanGrid.Cells[10, 8] := IntToStr(HumData.Alcohol.MaxMedicineValue);
  HumanGrid.Cells[11, 8] := IntToStr(HumData.Alcohol.WineDrinkValue);
*)
end;

procedure TFrmHeroFDBViewer.ShowBagItems();
var
  I, II, III: Integer;
begin
  (*
  for I := 0 to 2 do
  begin
    for II := 1 to BagItemGrid[I].RowCount - 1 do
    begin
      for III := 0 to BagItemGrid[I].ColCount - 1 do
      begin
        BagItemGrid[I].Cells[III, II] := '';
      end;
    end;
  end;
  for I := Low(HeroRecord.PrivateDatas) to High(HeroRecord.PrivateDatas) do
  begin
    for II := Low(HeroRecord.PrivateDatas[I].BagItems) to High(HeroRecord.PrivateDatas[I].BagItems) do
    begin
      ShowBagItem(II + 1, I, IntToStr(II + 1), HeroRecord.PrivateDatas[I].BagItems[II]);
    end;
  end;
  *)
end;

procedure TFrmHeroFDBViewer.ShowUserItems();
var
  I, II, III: Integer;
begin
  (*
  for I := 0 to 2 do
  begin
    for II := 1 to UserItemGrid[I].RowCount - 1 do
    begin
      for III := 1 to UserItemGrid[I].ColCount - 1 do
      begin
        UserItemGrid[I].Cells[III, II] := '';
      end;
    end;
  end;
  for I := Low(HeroRecord.PrivateDatas) to High(HeroRecord.PrivateDatas) do
  begin
    ShowUserItem(1, I, '衣服', HeroRecord.PrivateDatas[I].HumItems[0]);
    ShowUserItem(2, I, '武器', HeroRecord.PrivateDatas[I].HumItems[1]);
    ShowUserItem(3, I, '照明物', HeroRecord.PrivateDatas[I].HumItems[2]);
    ShowUserItem(4, I, '项链', HeroRecord.PrivateDatas[I].HumItems[3]);
    ShowUserItem(5, I, '头盔', HeroRecord.PrivateDatas[I].HumItems[4]);
    ShowUserItem(6, I, '左手镯', HeroRecord.PrivateDatas[I].HumItems[5]);
    ShowUserItem(7, I, '右手镯', HeroRecord.PrivateDatas[I].HumItems[6]);
    ShowUserItem(8, I, '左戒指', HeroRecord.PrivateDatas[I].HumItems[7]);
    ShowUserItem(9, I, '右戒指', HeroRecord.PrivateDatas[I].HumItems[8]);
    ShowUserItem(10, I, '物品', HeroRecord.PrivateDatas[I].HumItems[9]);
    ShowUserItem(11, I, '腰带', HeroRecord.PrivateDatas[I].HumItems[10]);
    ShowUserItem(12, I, '鞋子', HeroRecord.PrivateDatas[I].HumItems[11]);
    ShowUserItem(13, I, '宝石', HeroRecord.PrivateDatas[I].HumItems[12]);
    ShowUserItem(14, I, '斗笠', HeroRecord.PrivateDatas[I].HumItems[13]);
  end;
  *)
end;

procedure TFrmHeroFDBViewer.ShowUseMagic();
var
  I, II, III: Integer;
begin
  (*
  for I := 0 to 2 do
  begin
    for II := 1 to UseMagicGrid[I].RowCount - 1 do
    begin
      for III := 0 to UseMagicGrid[I].ColCount - 1 do
      begin
        UseMagicGrid[I].Cells[III, II] := '';
      end;
    end;
  end;
  for I := Low(HeroRecord.PrivateDatas) to High(HeroRecord.PrivateDatas) do
  begin
    for II := Low(HeroRecord.PrivateDatas[I].HumMagics) to High(HeroRecord.PrivateDatas[I].HumMagics) do
    begin
      if HeroRecord.PrivateDatas[I].HumMagics[I].wMagIdx <= 0 then Break;
      UseMagicGrid[I].Cells[0, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumMagics[II].wMagIdx);
      UseMagicGrid[I].Cells[1, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumMagics[II].btKey);
      UseMagicGrid[I].Cells[2, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumMagics[II].nTranPoint);
      UseMagicGrid[I].Cells[3, II + 1] := GetMagicName(HeroRecord.PrivateDatas[I].HumMagics[II].wMagIdx, HeroRecord.PrivateDatas[I].HumMagics[II].MagicAttr);
      UseMagicGrid[I].Cells[4, II + 1] := GetMagicTypeString(HeroRecord.PrivateDatas[I].HumMagics[II].MagicAttr);
    end;
    for II := Low(HeroRecord.PrivateDatas[I].HumNGMagics) to High(HeroRecord.PrivateDatas[I].HumNGMagics) do
    begin
      if HeroRecord.PrivateDatas[I].HumNGMagics[I].wMagIdx <= 0 then Break;
      UseMagicGrid[I].Cells[0, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumNGMagics[II].wMagIdx);
      UseMagicGrid[I].Cells[1, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumNGMagics[II].btKey);
      UseMagicGrid[I].Cells[2, II + 1] := IntToStr(HeroRecord.PrivateDatas[I].HumNGMagics[II].nTranPoint);
      UseMagicGrid[I].Cells[3, II + 1] := GetMagicName(HeroRecord.PrivateDatas[I].HumNGMagics[II].wMagIdx, HeroRecord.PrivateDatas[I].HumNGMagics[II].MagicAttr);
      UseMagicGrid[I].Cells[4, II + 1] := GetMagicTypeString(HeroRecord.PrivateDatas[I].HumNGMagics[II].MagicAttr);
    end;
  end;

  for II := 1 to UseSuccessiveMagicGrid.RowCount - 1 do
  begin
    for III := 0 to UseSuccessiveMagicGrid.ColCount - 1 do
    begin
      UseSuccessiveMagicGrid.Cells[III, II] := '';
    end;
  end;
  for II := Low(HeroRecord.PublicData.HumContinuousMagics) to High(HeroRecord.PublicData.HumContinuousMagics) do
  begin
    if HeroRecord.PublicData.HumContinuousMagics[II].wMagIdx <= 0 then Break;
    UseSuccessiveMagicGrid.Cells[0, II + 1] := IntToStr(HeroRecord.PublicData.HumContinuousMagics[II].wMagIdx);
    UseSuccessiveMagicGrid.Cells[1, II + 1] := IntToStr(HeroRecord.PublicData.HumContinuousMagics[II].btKey);
    UseSuccessiveMagicGrid.Cells[2, II + 1] := IntToStr(HeroRecord.PublicData.HumContinuousMagics[II].nTranPoint);
    UseSuccessiveMagicGrid.Cells[3, II + 1] := GetMagicName(HeroRecord.PublicData.HumContinuousMagics[II].wMagIdx, HeroRecord.PublicData.HumContinuousMagics[II].MagicAttr);
    UseSuccessiveMagicGrid.Cells[4, II + 1] := GetMagicTypeString(HeroRecord.PublicData.HumContinuousMagics[II].MagicAttr);
  end;
  *)
end;

end.
