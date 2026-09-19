unit ClFunc;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  Controls,
  HGECanvas,
  HGEFontEx,
  DxComponents,
  Grobal2,
  ExtCtrls,
  HUtil32,
  EDcode;

const
  DR_0 = 0;
  DR_1 = 1;
  DR_2 = 2;
  DR_3 = 3;
  DR_4 = 4;
  DR_5 = 5;
  DR_6 = 6;
  DR_7 = 7;
  DR_8 = 8;
  DR_9 = 9;
  DR_10 = 10;
  DR_11 = 11;
  DR_12 = 12;
  DR_13 = 13;
  DR_14 = 14;
  DR_15 = 15;

var
  DropItems:TList; // lsit of TClientItem

procedure BoldTextOut(X, Y:Integer; Str:string; FColor:TColor; BColor:TColor = clBlack; Alpha:Byte = 255); overload;

procedure BoldTextOut(HGEFont:THGEFont; X, Y:Integer; Str:string; FColor:TColor; BColor:TColor = clBlack; Alpha:Byte = 255); overload;
// 绘制文字，不使用阴影 piaoyun 2013-08-02

procedure BoldTextOutEx(HGEFont:THGEFont; X, Y:Integer; Str:string; FColor:TColor; BColor:TColor = clBlack; Alpha:Byte = 255);

function fmstr(Str:string; len:Integer):string;

function GetGoldStr(gold:LongWord):string;

procedure ClearBag;

function AddItemBag(cu:TClientItem; ReSetItem1_6:Boolean = True):Boolean;

function FindMagic(sMagicName:string):PTClientMagic;

function AddBetterItem(BagItem:PTClientItem):Boolean;

function CompareItem(BagItem, UseItem:PTClientItem; btJob:Byte):Boolean; //人物背包物品对比

function CheckItemNeed(BagItem:PTClientItem):Boolean;

function UpdateItemBag(cu:TClientItem; var OldDrua:Integer; IsCompareName:Boolean = True):Boolean;

procedure ArrangeItembag(DoSort:Boolean = False);

procedure ClearPetBag;

function AddPetItemBag(cu:TClientItem):Boolean;

function DelItemBag(iname:string; iindex:Integer):Boolean;

function UpdatePetItemBag(cu:TClientItem):Boolean;

procedure ArrangePetItembag;

procedure ArrangeSellPlayerItembag;

procedure ArrangeSellPlayerPetItembag;

function AddSellPlayerPetItemBag(cu:TClientItem):Boolean;

procedure ClearHeroBag;

function AddHeroItemBag(cu:TClientItem):Boolean;

function UpdateHeroItemBag(cu:TClientItem):Boolean;

function DelHeroItemBag(iname:string; iindex:Integer):Boolean;

procedure ArrangeHeroItembag;

procedure AddDropItem(ci:TClientItem);

function GetDropItem(iname:string; MakeIndex:Integer):PTClientItem;

function DelDropItem(iname:string; MakeIndex:Integer):Boolean;

procedure AddDealItem(ci:TClientItem);

procedure DelDealItem(ci:TClientItem);

procedure MoveDealItemToBag;

procedure AddDealRemoteItem(ci:TClientItem);

procedure DelDealRemoteItem(MakeIndex:Integer; ItemName:string);

function GetDistance(sx, sy, dx, dy:Integer):Integer;

procedure GetNextPosXY(dir:byte; var X, Y:Integer);

procedure GetNextRunXY(dir:byte; var X, Y:Integer);

procedure GetNextHorseRunXY(dir:byte; var X, Y:Integer);

function GetNextDirection(sx, sy, dx, dy:Integer):byte;

function GetBack(dir:Integer):Integer;

procedure GetBackPosition(sx, sy, dir:Integer; var newx, newy:Integer);

procedure GetFrontPosition(sx, sY, dir:Integer; var NewX, NewY:Integer); overload;

procedure GetFrontPosition(sx, sY, dir, nFlag:Integer; var NewX, NewY:Integer); overload;

function GetFlyDirection(sx, sy, ttx, tty:Integer):Integer;

function GetFlyDirection16(sx, sy, ttx, tty:Integer):Integer;

function PrivDir(ndir:Integer):Integer;

function NextDir(ndir:Integer):Integer;

function GetTakeOnPosition(StdMode, Shape:Integer):Integer;

function IsKeyPressed(Key:byte):Boolean;

procedure AddChangeFace(recogid:Int64);

procedure DelChangeFace(recogid:Int64);

function IsChangingFace(recogid:Int64):Boolean;

procedure AddChallengeItem(ci:TClientItem);

procedure DelChallengeItem(MakeIndex:Integer; ItemName:string);

procedure MoveChallengeItemToBag;

procedure ClearChallengeItem;

procedure AddChallengeRemoteItem(ci:TClientItem);

procedure DelChallengeRemoteItem(MakeIndex:Integer; ItemName:string);

procedure HeroM2AddUserShopItem(cu:TClientItem);

procedure HeroM2DelUserShopItem(nMakeIndex:Integer);

procedure HeroM2AddRemoteUserShopItem(cu:TClientItem);

procedure HeroM2DelRemoteUserShopItem(nMakeIndex:Integer);

function IsMobileNumber(num:string):boolean;

implementation

uses
  ClMain,
  MShare,
  SDK,
  Math;

function IsMobileNumber(num:string):boolean;
var
  i:Int64;
begin
  Result := False;
  if length(trim(num)) <> 11 then
    Exit;
  if num[1] <> '1' then
    Exit;
  i := StrToInt64Def(num, 0);
  if i <> 0 then
    Result := True
end;

function fmstr(Str:string; len:Integer):string;
var
  I:Integer;
begin
  try
    Result := Str + ' ';
    for I := 1 to len - Length(Str) - 1 do
      Result := Result + ' ';
  except
    Result := Str + ' ';
  end;
end;

function GetGoldStr(gold:LongWord):string;
var
  I, n:Integer;
  Str:string;
begin
  Str := IntToStr(gold);
  n := 0;
  Result := '';
  for I := Length(Str) downto 1 do begin
    if n = 3 then begin
      Result := Str[I] + ',' + Result;
      n := 1;
    end
    else begin
      Result := Str[I] + Result;
      Inc(n);
    end;
  end;
end;

procedure ClearHeroBag;
var
  I:Integer;
begin
  for I := 0 to Length(g_HeroItemArr) - 1 do
    g_HeroItemArr[I].S.Name := '';
end;

function AddHeroItemBag(cu:TClientItem):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := 0 to Length(g_HeroItemArr) - 1 do begin
    if (g_HeroItemArr[I].MakeIndex = cu.MakeIndex) and (g_HeroItemArr[I].S.Name = cu.S.Name) then begin
      Exit;
    end;
  end;
  for I := 0 to Length(g_HeroItemArr) - 1 do begin
    if g_HeroItemArr[I].S.Name = '' then begin
      g_HeroItemArr[I] := cu;
      Result := True;
      Break;
    end;
  end;
  ArrangeHeroItembag;
end;

function UpdateHeroItemBag(cu:TClientItem):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := Length(g_HeroItemArr) - 1 downto 0 do begin
    if (g_HeroItemArr[I].S.Name = cu.S.Name) and (g_HeroItemArr[I].MakeIndex = cu.MakeIndex) then begin
      g_HeroItemArr[I] := cu;
      Result := True;
      Break;
    end;
  end;
end;

function DelHeroItemBag(iname:string; iindex:Integer):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := Length(g_HeroItemArr) - 1 downto 0 do begin
    if (g_HeroItemArr[I].S.Name = iname) and (g_HeroItemArr[I].MakeIndex = iindex) then begin
      g_HeroItemArr[I].S.Name := '';
      // SafeFillChar(g_HeroItemArr[I], SizeOf(TClientItem), #0);
      Result := True;
      Break;
    end;
  end;
  ArrangeHeroItembag;
end;

procedure ArrangeHeroItembag;
var
  I, k:Integer;
begin
  // DScreen.AddChatBoardString('ArrangeHeroItembag' , clRed, clBlue);
  for I := 0 to Length(g_HeroItemArr) - 1 do begin
    if g_HeroItemArr[I].S.Name <> '' then begin
      for k := I + 1 to Length(g_HeroItemArr) - 1 do begin
        if (g_HeroItemArr[I].S.Name = g_HeroItemArr[k].S.Name) and (g_HeroItemArr[I].MakeIndex = g_HeroItemArr[k].MakeIndex) then begin
          g_HeroItemArr[k].S.Name := '';
          // SafeFillChar(g_HeroItemArr[k], SizeOf(TClientItem), #0);
        end;
      end;
      if (g_HeroItemArr[I].S.Name = g_MovingItem.Item.S.Name) and (g_HeroItemArr[I].MakeIndex = g_MovingItem.Item.MakeIndex) then begin
        g_MovingItem.Index := 0;
        g_MovingItem.Item.S.Name := '';
      end;
    end;
  end;
end;

procedure ClearBag;
var
  I:Integer;
begin
  for I := 0 to Length(g_ItemArr) - 1 do
    g_ItemArr[I].S.Name := '';
end;

function CheckItemNeed(BagItem:PTClientItem):Boolean;
var
  LoNeedValue, HiNeedValue:Word;
begin
  Result := False;
  LoNeedValue := LoWord(BagItem.S.NeedLevel);
  HiNeedValue := HiWord(BagItem.S.NeedLevel);
  case BagItem.S.Need of
    0, 18, 22:begin
        Result := Integer(g_MySelf.m_Abil.Level) >= BagItem.S.NeedLevel;
      end;
    14:begin
        Result := Integer(g_MySelf.m_Abil.Level) = BagItem.S.NeedLevel;
      end;
    1, 19, 23:begin
        Result := g_MySelf.m_Abil.DC2 >= BagItem.S.NeedLevel;
      end;
    2, 20, 24:begin
        Result := g_MySelf.m_Abil.MC2 >= BagItem.S.NeedLevel;
      end;
    3, 21, 25:begin
        Result := g_MySelf.m_Abil.SC2 >= BagItem.S.NeedLevel;
      end;
    4:begin
        Result := g_MySelf.m_btReLevel >= BagItem.S.NeedLevel;
      end;
    40:begin
        Result := (g_MySelf.m_btReLevel >= LoNeedValue) and (g_MySelf.m_Abil.Level >= HiNeedValue);
      end;
    41:begin
        Result := (g_MySelf.m_btReLevel >= LoNeedValue) and (g_MySelf.m_Abil.DC2 >= HiNeedValue);
      end;
    42:begin
        Result := (g_MySelf.m_btReLevel >= LoNeedValue) and (g_MySelf.m_Abil.MC2 >= HiNeedValue);
      end;
    43:begin
        Result := (g_MySelf.m_btReLevel >= LoNeedValue) and (g_MySelf.m_Abil.SC2 >= HiNeedValue);
      end;
    44:begin
        Result := (g_MySelf.m_btReLevel >= LoNeedValue) and (g_MySelf.m_Abil.CreditPoint >= HiNeedValue);
      end;
    5:begin
        Result := g_MySelf.m_Abil.CreditPoint >= BagItem.S.NeedLevel;
      end;
    50:begin
        Result := (g_MySelf.m_Abil.Level >= LoNeedValue) and (g_MySelf.m_Abil.CreditPoint >= HiNeedValue);
      end;
    51:begin
        Result := (g_MySelf.m_Abil.DC2 >= LoNeedValue) and (g_MySelf.m_Abil.CreditPoint >= HiNeedValue);
      end;
    52:begin
        Result := (g_MySelf.m_Abil.MC2 >= LoNeedValue) and (g_MySelf.m_Abil.CreditPoint >= HiNeedValue);
      end;
    53:begin
        Result := (g_MySelf.m_Abil.SC2 >= LoNeedValue) and (g_MySelf.m_Abil.CreditPoint >= HiNeedValue);
      end;
    10:begin
        Result := (g_MySelf.m_btJob = LoNeedValue) and (g_MySelf.m_Abil.Level >= HiNeedValue);
      end;
    11:begin
        Result := (g_MySelf.m_btJob = LoNeedValue) and (g_MySelf.m_Abil.DC2 >= HiNeedValue);
      end;
    12:begin
        Result := (g_MySelf.m_btJob = LoNeedValue) and (g_MySelf.m_Abil.MC2 >= HiNeedValue);
      end;
    13:begin
        Result := (g_MySelf.m_btJob = LoNeedValue) and (g_MySelf.m_Abil.SC2 >= HiNeedValue);
      end;
  end;
end;

function CompareItem(BagItem, UseItem:PTClientItem; btJob:Byte):Boolean; //人物背包物品对比
begin
  Result := False;

  if Length(UseItem.s.Name) > 0 then begin
    case g_ClientConfig.btBagFastItemCompareMode of
      1:begin
          if BagItem.s.NeedLevel > UseItem.s.NeedLevel then
            Result := True;
        end;
      2:begin
          if BagItem.s.Weight > UseItem.s.Weight then
            Result := True;
        end;
      else
        if btJob = 0 then begin
          if ((BagItem.s.DC1 = UseItem.s.DC1) and (BagItem.s.DC2 > UseItem.s.DC2)) or ((BagItem.s.DC1 > UseItem.s.DC1) and (BagItem.s.DC2 = UseItem.s.DC2)) or ((BagItem.s.DC1 > UseItem.s.DC1) and (BagItem.s.DC2 > UseItem.s.DC2)) or (BagItem.s.DC2 > UseItem.s.DC2) or ((BagItem.s.DC1 = UseItem.s.DC1) and (BagItem.s.DC2 = UseItem.s.DC2) and (((BagItem.s.AC1 = UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 >= UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)))) then begin
            Result := True;
          end;
        end
        else if btJob = 1 then begin
          if ((BagItem.s.MC1 = UseItem.s.MC1) and (BagItem.s.MC2 > UseItem.s.MC2)) or ((BagItem.s.MC1 > UseItem.s.MC1) and (BagItem.s.MC2 = UseItem.s.MC2)) or ((BagItem.s.MC1 > UseItem.s.MC1) and (BagItem.s.MC2 > UseItem.s.MC2)) or (BagItem.s.MC2 > UseItem.s.MC2) or ((BagItem.s.MC1 = UseItem.s.MC1) and (BagItem.s.MC2 = UseItem.s.MC2) and (((BagItem.s.AC1 = UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 = UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)))) then begin
            Result := True;
          end;
        end
        else begin
          if ((BagItem.s.SC1 = UseItem.s.SC1) and (BagItem.s.SC2 > UseItem.s.SC2)) or ((BagItem.s.SC1 > UseItem.s.SC1) and (BagItem.s.SC2 = UseItem.s.SC2)) or ((BagItem.s.SC1 > UseItem.s.SC1) and (BagItem.s.SC2 > UseItem.s.SC2)) or (BagItem.s.SC2 > UseItem.s.SC2) or ((BagItem.s.SC1 = UseItem.s.SC1) and (BagItem.s.SC2 = UseItem.s.SC2) and (((BagItem.s.AC1 = UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 = UseItem.s.AC2)) or ((BagItem.s.AC1 > UseItem.s.AC1) and (BagItem.s.AC2 > UseItem.s.AC2)))) then begin
            Result := True;
          end;
        end;
    end;
  end
  else begin
    Result := True;
  end;
end;

function FindMagic(sMagicName:string):PTClientMagic;
var
  I:Integer;
  pm:PTClientMagic;
begin
  Result := nil;
  g_MagicList.Lock;
  try
    for I := 0 to g_MagicList.Count - 1 do begin
      pm := PTClientMagic(g_MagicList[I]);
      if SameText(pm.Def.sMagicName, sMagicName) then begin
        Result := pm;
        Break;
      end;
    end;
  finally
    g_MagicList.UnLock;
  end;
end;

function AddBetterItem(BagItem:PTClientItem):Boolean;
var
  boShowUP, Flag:Boolean;
  I, nWhere, nWhere1, nWhere2:Integer;
  UseItem:PTClientItem;
begin
  boShowUP := False; //HZQ 20230525
  if (Length(BagItem.s.Name) > 0) and (not (BagItem.s.StdMode in [93 {宠物加负重}])) then begin
    //boShowUP := False; //HZQ 20230525

    Flag := False;
    nWhere := GetTakeOnPosition(BagItem.S.StdMode, BagItem.S.Shape);

    if nWhere = -1 then begin
      if (BagItem.S.StdMode = 4) and (g_EatingItem.S.Shape < 100) and (FindMagic(BagItem.S.Name) = nil) then begin
        Result := True;
        g_BetterItemList.Insert(0, BagItem);
      end else begin
        Result := False; //HZQ 20230525
      end;
      Exit;
    end;

    UseItem := nil;
    //HZQ 20230525 右戒指笔误修复
    if (g_MySelf.m_nJewelryBoxStatus = jbsOpen) and (BagItem.s.OverLap and 2 <> 0)
      and (nWhere in [U_HELMET {头盔}, U_NECKLACE {项链}, U_ARMRINGL {左手镯}, U_ARMRINGR {右手镯}, U_RINGL {左戒指}, U_RINGR {右戒指}]) then begin
      if BagItem.s.Expand1 in [1..6] then begin
        UseItem := @g_JewelryBoxItems[BagItem.s.Expand1 - 1];
      end else if BagItem.s.Expand1 in [0, 7] then begin
        for I := Low(g_JewelryBoxItems) to High(g_JewelryBoxItems) do begin
          UseItem := @g_JewelryBoxItems[I];
          boShowUP := CompareItem(BagItem, UseItem, g_MySelf.m_btJob);
          if boShowUP then begin
            UseItem := nil;
            Break;
          end;
        end;
      end;
    end else if g_MySelf.m_boShowGodBless and (BagItem.s.OverLap and 4 <> 0) then begin
      if BagItem.s.Expand1 in [1..12] then begin
        if (g_GodBlessItemsState[BagItem.s.Expand1 - 1] = 1) then begin
          UseItem := @g_GodBlessItems[BagItem.s.Expand1 - 1];
        end;
      end else if BagItem.s.Expand1 in [13] then begin
        for I := Low(g_GodBlessItems) to High(g_GodBlessItems) do begin
          UseItem := @g_GodBlessItems[I];
          boShowUP := CompareItem(BagItem, UseItem, g_MySelf.m_btJob);
          if boShowUP then begin
            UseItem := nil;
            Break;
          end;
        end;
      end;
    end;

    if UseItem <> nil then begin
      boShowUP := CompareItem(BagItem, UseItem, g_MySelf.m_btJob);
    end;

    if (not boShowUP) and ((BagItem.S.OverLap = 0) or (BagItem.S.OverLap and 1 = 1)) then begin
      case nWhere of
        U_DRESS, U_FASHIONDRESS:begin
            if not ((nWhere = U_FASHIONDRESS) and (g_ClientConfig.boUseOldSerialWindows)) then begin
              Flag := True;
            end;
          end;
        U_WEAPON:
          Flag := True;
        U_NECKLACE:
          Flag := True;
        U_RIGHTHAND:
          Flag := True;
        U_HELMET:
          Flag := True;
        U_RINGR, U_RINGL:
          Flag := True;
        U_ARMRINGR, U_ARMRINGL:
          Flag := True;

        { 1.76界面下面一排不允许装备 chongchong 2013-09-16 }
        U_BUJUK:begin
            if g_ClientConfig.boUseOldSerialWindows and (g_ClientConfig.boStateWindowsType = 0) then begin
              Flag := not g_ClientConfig.boDisableDuFuTakeArmRingL;
              if Flag then
                nWhere := U_ARMRINGL;
            end
            else begin
              Flag := True;
            end;
          end;
        U_BELT:
          Flag := (not g_ClientConfig.boUseOldSerialWindows) or (g_ClientConfig.boStateWindowsType <> 0); // True;
        U_BOOTS:
          Flag := (not g_ClientConfig.boUseOldSerialWindows) or (g_ClientConfig.boStateWindowsType <> 0); // True;
        U_CHARM:
          Flag := (not g_ClientConfig.boUseOldSerialWindows) or (g_ClientConfig.boStateWindowsType <> 0); // True;

        U_HAT:
          Flag := True;
        U_DRUM:
          Flag := (not g_ClientConfig.boUseOldSerialWindows);
        U_HORSE:begin
            Flag := True;
            if g_ClientConfig.boUseOldSerialWindows then begin
              nWhere := U_RIGHTHAND;
            end;
          end;
        U_SHIELD:
          Flag := True; // 盾牌 chongchong 2013-09-16
        U_JADE:
          Flag := (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONWEAPON:
          Flag := (not g_ClientConfig.boUseOldSerialWindows);

        U_FASHIONNECKLACE:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONHELMET:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONARMRINGR, U_FASHIONARMRINGL:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONRINGL:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONRIGHTHAND:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONBELT:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONBOOTS:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
        U_FASHIONCHARM:
          Flag := g_ClientConfig.boFashionJewelryOpen and (not g_ClientConfig.boUseOldSerialWindows);
      end;

      if Flag then begin
        nWhere1 := nWhere;
        nWhere2 := -1;

        if nWhere in [U_RINGR, U_RINGL] then begin
          nWhere1 := U_RINGR;
          nWhere2 := U_RINGL;
        end else if nWhere in [U_ARMRINGR, U_ARMRINGL] then begin
          if BagItem.s.StdMode = 25 then begin
            nWhere1 := U_ARMRINGL
          end else begin
            nWhere1 := U_ARMRINGR;
            nWhere2 := U_ARMRINGL;
          end;
        end else if nWhere in [U_FASHIONRINGR, U_FASHIONRINGL] then begin
          nWhere1 := U_FASHIONRINGR;
          nWhere2 := U_FASHIONRINGL;
        end else if nWhere in [U_FASHIONARMRINGR, U_FASHIONARMRINGL] then begin
          nWhere1 := U_FASHIONARMRINGR;
          nWhere2 := U_FASHIONARMRINGL;
        end;

        if nWhere1 > -1 then begin
          UseItem := @g_UseItems[nWhere1];
          boShowUP := CompareItem(BagItem, UseItem, g_MySelf.m_btJob);
        end;

        if (not boShowUP) and (nWhere2 > -1) then begin
          UseItem := @g_UseItems[nWhere2];
          boShowUP := CompareItem(BagItem, UseItem, g_MySelf.m_btJob);
        end;
      end;
    end;
  end;

  Result := boShowUP;
  if not boShowUP then Exit;
  for I := 0 to g_BetterItemList.Count - 1 do begin
    UseItem := g_BetterItemList.Items[I];
    if (UseItem.MakeIndex = BagItem.MakeIndex) and (UseItem.s.Name = BagItem.s.Name) then
      Exit;
  end;
  //  g_BetterItemList.Add(BagItem);
  g_BetterItemList.Insert(0, BagItem);
end;

function AddItemBag(cu:TClientItem; ReSetItem1_6:Boolean {是否重新设置下面一排6个快捷包裹栏}):Boolean;
var
  I, MinCount:Integer;
begin
  {$IF PRIVATE_CLIENT = 1}
  {.$I AddVmpFeatureCode_M.inc}
  {$IFEND}

  Result := FALSE;
  for I := 0 to GetMaxBagCount - 1 do begin
    if (g_ItemArr[I].MakeIndex = cu.MakeIndex) and (g_ItemArr[I].S.Name = cu.S.Name) then begin
      Exit;
    end;
  end;

  if cu.S.Name = '' then
    Exit;

  if not g_boBagLoaded then begin
    // 先看看用户记录的数据中，快捷栏的物品放置顺序
    if ((cu.S.StdMode <= 3) and (cu.s.Source <> -1)) or ((cu.s.StdMode = 31) and (cu.s.Source = -2)) or (cu.s.StdMode = 49) {聚灵珠让放到快捷栏} or (cu.s.StdMode = 95) {攻击类物品放到快捷栏} then begin
      MinCount := Min(g_SaveMyBagItemList.Count, 6);
      for I := 0 to MinCount - 1 do begin
        if cu.MakeIndex = Integer(g_SaveMyBagItemList.Items[I]) then begin
          g_ItemArr[I] := cu;
          Result := True;
          Exit;
        end;
      end;
    end;

    // 快捷物品栏没有人为摆放物品
    if g_SaveMyBagItemList.Count = 0 then begin
      if ReSetItem1_6 then begin
        // 太阳水捆药绳不放到最下一排中
        if ((cu.S.StdMode <= 3) and (cu.s.Source <> -1)) or ((cu.s.StdMode = 31) and (cu.s.Source = -2)) or (cu.s.StdMode = 49) {聚灵珠让放到快捷栏} or (cu.s.StdMode = 95) {攻击类物品放到快捷栏} then begin
          for I := 0 to 5 do
            if g_ItemArr[I].S.Name = '' then begin
              g_ItemArr[I] := cu;
              Result := True;
              Exit;
            end;
        end;
      end
      else begin
        for I := Low(g_SaveItem1_6) to High(g_SaveItem1_6) do begin
          if (g_SaveItem1_6[I].s.Name <> '') and SameText(g_SaveItem1_6[I].s.Name, cu.s.Name) and (g_SaveItem1_6[I].MakeIndex = cu.MakeIndex) then begin
            g_ItemArr[I] := cu;
            Result := True;
            Exit;
          end;
        end;
      end;
    end;

    for I := 6 to g_SaveMyBagItemList.Count - 1 do begin
      if (cu.MakeIndex = Integer(g_SaveMyBagItemList.Items[I])) and (g_ItemArr[I].S.Name = '') then begin
        g_ItemArr[I] := cu;
        Result := True;
        Exit;
      end;
    end;

    for I := 6 to GetMaxBagCount - 1 do begin
      if g_ItemArr[I].S.Name = '' then begin
        g_ItemArr[I] := cu;
        Result := True;
        Break;
      end;
    end;
  end
  else begin
    if ReSetItem1_6 then begin
      // 太阳水捆药绳不放到最下一排中
      if ((cu.S.StdMode <= 3) and (cu.s.Source <> -1)) or ((cu.s.StdMode = 31) and (cu.s.Source = -2)) or (cu.s.StdMode = 49) {聚灵珠让放到快捷栏} or (cu.s.StdMode = 95) {攻击类物品放到快捷栏} then begin
        if Length(g_EatingItem.s.Name) > 0 then begin
          for I := 0 to 5 do begin
            if (g_ItemArr[I].S.Name = '') and (I = g_EatingItemIndex) and (cu.MakeIndex = g_EatingItem.MakeIndex) then begin
              g_ItemArr[I] := cu;
              Result := True;
              Exit;
            end;
          end;

          for I := 0 to 5 do begin
            if (g_ItemArr[I].S.Name = '') and (g_ItemArr[I].MakeIndex <> g_EatingItem.MakeIndex) then begin
              g_ItemArr[I] := cu;
              Result := True;
              //OutputDebugString(PChar(IntToStr(I)));
              Exit;
            end;
          end;
        end
        else begin
          for I := 0 to 5 do begin
            if (g_ItemArr[I].S.Name = '') then begin
              g_ItemArr[I] := cu;
              //OutputDebugString(PChar(IntToStr(I)));
              Result := True;
              Exit;
            end;
          end;
        end;
      end;
    end
    else begin
      for I := Low(g_SaveItem1_6) to High(g_SaveItem1_6) do begin
        if (g_SaveItem1_6[I].s.Name <> '') and SameText(g_SaveItem1_6[I].s.Name, cu.s.Name) and (g_SaveItem1_6[I].MakeIndex = cu.MakeIndex) then begin
          g_ItemArr[I] := cu;
          Result := True;
          Exit;
        end;
      end;
    end;

    for I := 6 to GetMaxBagCount - 1 do begin
      if g_ItemArr[I].S.Name = '' then begin
        g_ItemArr[I] := cu;
        Result := True;
        Break;
      end;
    end;
    ArrangeItembag;
  end;
end;

procedure ArrangePetItembag;
var
  I, k:Integer;
begin
  // DScreen.AddChatBoardString('ArrangeHeroItembag' , clRed, clBlue);
  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if g_PetItemArr[I].S.Name <> '' then begin
      for k := I + 1 to Length(g_PetItemArr) - 1 do begin
        if (g_PetItemArr[I].S.Name = g_PetItemArr[k].S.Name) and (g_PetItemArr[I].MakeIndex = g_PetItemArr[k].MakeIndex) then begin
          g_PetItemArr[k].S.Name := '';
          // SafeFillChar(g_HeroItemArr[k], SizeOf(TClientItem), #0);
        end;
      end;
      if (g_PetItemArr[I].S.Name = g_MovingItem.Item.S.Name) and (g_PetItemArr[I].MakeIndex = g_MovingItem.Item.MakeIndex) then begin
        g_MovingItem.Index := 0;
        g_MovingItem.Item.S.Name := '';
      end;
    end;
  end;
end;

procedure ClearPetBag;
var
  I:Integer;
begin
  for I := 0 to Length(g_PetItemArr) - 1 do
    g_PetItemArr[I].S.Name := '';
end;

function AddPetItemBag(cu:TClientItem):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if (g_PetItemArr[I].MakeIndex = cu.MakeIndex) and (g_PetItemArr[I].S.Name = cu.S.Name) then begin
      Exit;
    end;
  end;

  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if g_PetItemArr[I].S.Name = '' then begin
      g_PetItemArr[I] := cu;
      Result := True;
      Break;
    end;
  end;

  ArrangePetItembag;
end;

function UpdatePetItemBag(cu:TClientItem):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := MAX_GAMEPET_BAG_COUNT - 1 downto 0 do begin
    if (g_PetItemArr[I].S.Name = cu.S.Name) and (g_PetItemArr[I].MakeIndex = cu.MakeIndex) then begin
      g_PetItemArr[I] := cu;
      Result := True;
      Break;
    end;
  end;
end;

function UpdateItemBag(cu:TClientItem; var OldDrua:Integer; IsCompareName:Boolean = True):Boolean;
var
  I:Integer;
begin
  Result := False;
  I := GetMaxBagCount - 1;
  // 改For为While; delphi编译器Bug chongchong 2014-01-07

  if IsCompareName then begin
    while I >= 0 do begin
      if (SameText(g_ItemArr[I].S.Name, cu.S.Name) or SameText(g_ItemArr[I].S.DBName, cu.S.Name)) and (Length(g_ItemArr[I].S.Name) <> 0) and (g_ItemArr[I].MakeIndex = cu.MakeIndex) then begin
        OldDrua := g_ItemArr[I].Dura;
        g_ItemArr[I] := cu;
        Result := True;
        Break;
      end;
      Dec(I);
    end;
  end
  else begin
    // 修改，当捡到装备改名，成这个鸟样了 chongchong 2015-12-03
    while I >= 0 do begin
      if (g_ItemArr[I].MakeIndex = cu.MakeIndex) and (Length(g_ItemArr[I].S.Name) <> 0) then begin
        OldDrua := g_ItemArr[I].Dura;
        g_ItemArr[I] := cu;
        Result := True;
        Break;
      end;
      Dec(I);
    end;
  end;
end;

function DelItemBag(iname:string; iindex:Integer):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  I := GetMaxBagCount - 1;
  while I >= 0 do begin
    if (g_ItemArr[I].S.Name = iname) and (g_ItemArr[I].MakeIndex = iindex) then begin
      g_ItemArr[I].S.Name := '';
      // SafeFillChar(g_ItemArr[I], SizeOf(TClientItem), #0);
      Result := True;
      Break;
    end;
    Dec(I);
  end;
  ArrangeItembag;
end;

procedure ArrangeItembag(DoSort:Boolean);
var
  I, k:Integer;

  procedure QuickSortBagItem(L, R:Integer);
  var
    I, J:Integer;
    P, T:TClientItem;
  begin
    repeat
      I := L;
      J := R;
      P := g_ItemArr[L + (R - L) div 2];
      repeat
        while AnsiCompareText(g_ItemArr[I].s.Name, P.s.Name) > 0 do
          Inc(I);
        while AnsiCompareText(g_ItemArr[J].s.Name, P.s.Name) < 0 do
          Dec(J);
        if I <= J then begin
          T := g_ItemArr[I];
          g_ItemArr[I] := g_ItemArr[J];
          g_ItemArr[J] := T;
          Inc(I);
          Dec(J);
        end;
      until I > J;
      if L < J then
        QuickSortBagItem(L, J);
      L := I;
    until I >= R;
  end;

begin
  for I := 0 to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].S.Name <> '' then begin
      // 同一物品不允许显示两次
      for k := I + 1 to GetMaxBagCount - 1 do begin
        if (g_ItemArr[I].S.Name = g_ItemArr[k].S.Name) and (g_ItemArr[I].MakeIndex = g_ItemArr[k].MakeIndex) then begin
          g_ItemArr[k].S.Name := '';
          // SafeFillChar(g_ItemArr[k], SizeOf(TClientItem), #0);
        end;
      end;

      // 如果正在移动的是包裹中的物品，取消移动物品
      if (g_ItemArr[I].S.Name = g_MovingItem.Item.S.Name) and (g_ItemArr[I].MakeIndex = g_MovingItem.Item.MakeIndex) then begin
        g_MovingItem.Index := 0;
        g_MovingItem.Item.S.Name := '';
      end;
    end;
  end;

  if DoSort then
    QuickSortBagItem(6, GetMaxBagCount - 1);
end;

procedure ArrangeSellPlayerItembag;

  procedure QuickSortBagItem(L, R:Integer);
  var
    I, J:Integer;
    P, T:TClientItem;
  begin
    repeat
      I := L;
      J := R;
      P := g_SellPlayerItemArr[L + (R - L) div 2];
      repeat
        while AnsiCompareText(g_SellPlayerItemArr[I].s.Name, P.s.Name) > 0 do
          Inc(I);
        while AnsiCompareText(g_SellPlayerItemArr[J].s.Name, P.s.Name) < 0 do
          Dec(J);
        if I <= J then begin
          T := g_SellPlayerItemArr[I];
          g_SellPlayerItemArr[I] := g_SellPlayerItemArr[J];
          g_SellPlayerItemArr[J] := T;
          Inc(I);
          Dec(J);
        end;
      until I > J;
      if L < J then
        QuickSortBagItem(L, J);
      L := I;
    until I >= R;
  end;

begin
  if Length(g_SellPlayerItemArr) > 1 then begin
    QuickSortBagItem(0, Length(g_SellPlayerItemArr) - 1);
  end;
end;

procedure ArrangeSellPlayerPetItembag;
var
  I, k:Integer;
begin
  // DScreen.AddChatBoardString('ArrangeHeroItembag' , clRed, clBlue);
  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if g_SellPlayerPetItemArr[I].S.Name <> '' then begin
      for k := I + 1 to Length(g_SellPlayerPetItemArr) - 1 do begin
        if (g_SellPlayerPetItemArr[I].S.Name = g_SellPlayerPetItemArr[k].S.Name) and (g_SellPlayerPetItemArr[I].MakeIndex = g_SellPlayerPetItemArr[k].MakeIndex) then begin
          g_SellPlayerPetItemArr[k].S.Name := '';
          // SafeFillChar(g_HeroItemArr[k], SizeOf(TClientItem), #0);
        end;
      end;
    end;
  end;
end;

function AddSellPlayerPetItemBag(cu:TClientItem):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if (g_SellPlayerPetItemArr[I].MakeIndex = cu.MakeIndex) and (g_SellPlayerPetItemArr[I].S.Name = cu.S.Name) then begin
      Exit;
    end;
  end;

  for I := 0 to MAX_GAMEPET_BAG_COUNT - 1 do begin
    if g_SellPlayerPetItemArr[I].S.Name = '' then begin
      g_SellPlayerPetItemArr[I] := cu;
      Result := True;
      Break;
    end;
  end;
end;

{----------------------------------------------------------}

procedure AddDropItem(ci:TClientItem);
var
  pc:PTClientItem;
begin
  New(pc);
  pc^ := ci;
  DropItems.Add(pc);
end;

function GetDropItem(iname:string; MakeIndex:Integer):PTClientItem;
var
  I:Integer;
begin
  Result := nil;
  for I := 0 to DropItems.Count - 1 do begin
    if (PTClientItem(DropItems[I]).S.Name = iname) and (PTClientItem(DropItems[I]).MakeIndex = MakeIndex) then begin
      Result := PTClientItem(DropItems[I]);
      Break;
    end;
  end;
end;

function DelDropItem(iname:string; MakeIndex:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to DropItems.Count - 1 do begin
    if (PTClientItem(DropItems[I]).S.Name = iname) and (PTClientItem(DropItems[I]).MakeIndex = MakeIndex) then begin
      Result := True;
      Dispose(PTClientItem(DropItems[I]));
      DropItems.Delete(I);
      Break;
    end;
  end;
end;

{----------------------------------------------------------}

procedure AddDealItem(ci:TClientItem);
var
  I:Integer;
begin
  for I := 0 to 10 - 1 do begin
    if g_DealItems[I].S.Name = '' then begin
      g_DealItems[I] := ci;
      Break;
    end;
  end;
end;

procedure DelDealItem(ci:TClientItem);
var
  I:Integer;
begin
  for I := 0 to 10 - 1 do begin
    if (g_DealItems[I].S.Name = ci.S.Name) and (g_DealItems[I].MakeIndex = ci.MakeIndex) then begin
      FillChar(g_DealItems[I], SizeOf(TClientItem), #0);
      Break;
    end;
  end;
end;

procedure MoveDealItemToBag;
var
  I:Integer;
begin
  for I := 0 to 10 - 1 do begin
    if g_DealItems[I].S.Name <> '' then
      AddItemBag(g_DealItems[I]);
  end;
  FillChar(g_DealItems, SizeOf(TClientItem) * 10, #0);
end;

procedure AddDealRemoteItem(ci:TClientItem);
var
  I:Integer;
begin
  for I := 0 to 20 - 1 do begin
    if g_DealRemoteItems[I].S.Name = '' then begin
      g_DealRemoteItems[I] := ci;
      Break;
    end;
  end;
end;

procedure DelDealRemoteItem(MakeIndex:Integer; ItemName:string);
var
  I:Integer;
begin
  for I := 0 to 20 - 1 do begin
    if SameText(g_DealRemoteItems[I].S.Name, ItemName) and (g_DealRemoteItems[I].MakeIndex = MakeIndex) then begin
      FillChar(g_DealRemoteItems[I], SizeOf(TClientItem), #0);
      Break;
    end;
  end;
end;
{----------------------------------------------------------}

procedure AddChallengeItem(ci:TClientItem);
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if g_ChallengeItems[I].S.Name = '' then begin
      g_ChallengeItems[I] := ci;
      Break;
    end;
  end;
end;

procedure DelChallengeItem(MakeIndex:Integer; ItemName:string);
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if (g_ChallengeItems[I].MakeIndex = MakeIndex) and SameText(g_ChallengeItems[I].S.Name, ItemName) then begin
      FillChar(g_ChallengeItems[I], SizeOf(TClientItem), #0);
      Break;
    end;
  end;
end;

procedure MoveChallengeItemToBag;
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if g_ChallengeItems[I].S.Name <> '' then begin
      AddItemBag(g_ChallengeItems[I]);
      g_ChallengeItems[I].S.Name := '';
    end;
  end;
end;

procedure ClearChallengeItem;
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if g_ChallengeItems[I].S.Name <> '' then begin
      g_ChallengeItems[I].S.Name := '';
    end;
  end;
end;

procedure AddChallengeRemoteItem(ci:TClientItem);
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if g_ChallengeRemoteItems[I].S.Name = '' then begin
      g_ChallengeRemoteItems[I] := ci;
      Break;
    end;
  end;
end;

procedure DelChallengeRemoteItem(MakeIndex:Integer; ItemName:string);
var
  I:Integer;
begin
  for I := 0 to 4 - 1 do begin
    if (g_ChallengeRemoteItems[I].MakeIndex = MakeIndex) and SameText(g_ChallengeRemoteItems[I].S.Name, ItemName) then begin
      FillChar(g_ChallengeRemoteItems[I], SizeOf(TClientItem), #0);
      Break;
    end;
  end;
end;
{----------------------------------------------------------}

procedure HeroM2AddUserShopItem(cu:TClientItem);
var
  I:Integer;
begin
  for I := 0 to Length(g_HeroM2ShopItems) - 1 do begin
    if g_HeroM2ShopItems[I].S.Name = '' then begin
      g_HeroM2ShopItems[I] := cu;
      break;
    end;
  end;
end;

procedure HeroM2DelUserShopItem(nMakeIndex:Integer);
var
  I:Integer;
begin
  for I := 0 to Length(g_HeroM2ShopItems) - 1 do begin
    if g_HeroM2ShopItems[I].MakeIndex = nMakeIndex then begin
      g_HeroM2ShopItems[I].S.Name := '';
    end;
  end;
end;

procedure HeroM2AddRemoteUserShopItem(cu:TClientItem);
var
  I:Integer;
begin
  for I := 0 to Length(g_HeroM2ShopRemoteItems) - 1 do begin
    if g_HeroM2ShopRemoteItems[I].S.Name = '' then begin
      g_HeroM2ShopRemoteItems[I] := cu;
      break;
    end;
  end;
end;

procedure HeroM2DelRemoteUserShopItem(nMakeIndex:Integer);
var
  I:Integer;
begin
  for I := 0 to Length(g_HeroM2ShopRemoteItems) - 1 do begin
    if g_HeroM2ShopRemoteItems[I].MakeIndex = nMakeIndex then begin
      g_HeroM2ShopRemoteItems[I].S.Name := '';
    end;
  end;
end;
// -------------------------------------------------------------------------------

function GetDistance(sx, sy, dx, dy:Integer):Integer;
begin
  Result := _MAX(abs(sx - dx), abs(sy - dy));
end;

procedure GetNextPosXY(dir:byte; var X, Y:Integer);
begin
  case dir of
    DR_UP:begin
        X := X;
        Y := Y - 1;
      end;
    DR_UPRIGHT:begin
        X := X + 1;
        Y := Y - 1;
      end;
    DR_RIGHT:begin
        X := X + 1;
        Y := Y;
      end;
    DR_DOWNRIGHT:begin
        X := X + 1;
        Y := Y + 1;
      end;
    DR_DOWN:begin
        X := X;
        Y := Y + 1;
      end;
    DR_DOWNLEFT:begin
        X := X - 1;
        Y := Y + 1;
      end;
    DR_LEFT:begin
        X := X - 1;
        Y := Y;
      end;
    DR_UPLEFT:begin
        X := X - 1;
        Y := Y - 1;
      end;
  end;
end;

procedure GetNextRunXY(dir:byte; var X, Y:Integer);
var
  Offset:Integer;
begin
  // 骑马一步三格 chongchong 2013-10-16
  if (g_MySelf.m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then
    Offset := 3
  else
    Offset := 2;

  case dir of
    DR_UP:begin
        X := X;
        Y := Y - Offset;
      end;
    DR_UPRIGHT:begin
        X := X + Offset;
        Y := Y - Offset;
      end;
    DR_RIGHT:begin
        X := X + Offset;
        Y := Y;
      end;
    DR_DOWNRIGHT:begin
        X := X + Offset;
        Y := Y + Offset;
      end;
    DR_DOWN:begin
        X := X;
        Y := Y + Offset;
      end;
    DR_DOWNLEFT:begin
        X := X - Offset;
        Y := Y + Offset;
      end;
    DR_LEFT:begin
        X := X - Offset;
        Y := Y;
      end;
    DR_UPLEFT:begin
        X := X - Offset;
        Y := Y - Offset;
      end;
  end;
end;

procedure GetNextHorseRunXY(dir:byte; var X, Y:Integer);
begin
  case dir of
    DR_UP:begin
        X := X;
        Y := Y - 3;
      end;
    DR_UPRIGHT:begin
        X := X + 3;
        Y := Y - 3;
      end;
    DR_RIGHT:begin
        X := X + 3;
        Y := Y;
      end;
    DR_DOWNRIGHT:begin
        X := X + 3;
        Y := Y + 3;
      end;
    DR_DOWN:begin
        X := X;
        Y := Y + 3;
      end;
    DR_DOWNLEFT:begin
        X := X - 3;
        Y := Y + 3;
      end;
    DR_LEFT:begin
        X := X - 3;
        Y := Y;
      end;
    DR_UPLEFT:begin
        X := X - 3;
        Y := Y - 3;
      end;
  end;
end;

function GetNextDirection(sx, sy, dx, dy:Integer):byte;
var
  flagx, flagy:Integer;
begin
  Result := DR_DOWN;
  if sx < dx then
    flagx := 1
  else if sx = dx then
    flagx := 0
  else
    flagx := -1;
  if abs(sy - dy) > 2 then
    if (sx >= dx - 1) and (sx <= dx + 1) then
      flagx := 0;

  if sy < dy then
    flagy := 1
  else if sy = dy then
    flagy := 0
  else
    flagy := -1;
  if abs(sx - dx) > 2 then
    if (sy > dy - 1) and (sy <= dy + 1) then
      flagy := 0;

  if (flagx = 0) and (flagy = -1) then
    Result := DR_UP;
  if (flagx = 1) and (flagy = -1) then
    Result := DR_UPRIGHT;
  if (flagx = 1) and (flagy = 0) then
    Result := DR_RIGHT;
  if (flagx = 1) and (flagy = 1) then
    Result := DR_DOWNRIGHT;
  if (flagx = 0) and (flagy = 1) then
    Result := DR_DOWN;
  if (flagx = -1) and (flagy = 1) then
    Result := DR_DOWNLEFT;
  if (flagx = -1) and (flagy = 0) then
    Result := DR_LEFT;
  if (flagx = -1) and (flagy = -1) then
    Result := DR_UPLEFT;
end;

function GetBack(dir:Integer):Integer;
begin
  Result := DR_UP;
  case dir of
    DR_UP:
      Result := DR_DOWN;
    DR_DOWN:
      Result := DR_UP;
    DR_LEFT:
      Result := DR_RIGHT;
    DR_RIGHT:
      Result := DR_LEFT;
    DR_UPLEFT:
      Result := DR_DOWNRIGHT;
    DR_UPRIGHT:
      Result := DR_DOWNLEFT;
    DR_DOWNLEFT:
      Result := DR_UPRIGHT;
    DR_DOWNRIGHT:
      Result := DR_UPLEFT;
  end;
end;

procedure GetBackPosition(sx, sy, dir:Integer; var newx, newy:Integer);
begin
  newx := sx;
  newy := sy;
  case dir of
    DR_UP:
      newy := newy + 1;
    DR_DOWN:
      newy := newy - 1;
    DR_LEFT:
      newx := newx + 1;
    DR_RIGHT:
      newx := newx - 1;
    DR_UPLEFT:begin
        newx := newx + 1;
        newy := newy + 1;
      end;
    DR_UPRIGHT:begin
        newx := newx - 1;
        newy := newy + 1;
      end;
    DR_DOWNLEFT:begin
        newx := newx + 1;
        newy := newy - 1;
      end;
    DR_DOWNRIGHT:begin
        newx := newx - 1;
        newy := newy - 1;
      end;
  end;
end;

procedure GetFrontPosition(sx, sY, dir:Integer; var NewX, NewY:Integer);
begin
  NewX := sx;
  NewY := sY;
  case dir of
    DR_UP:
      NewY := NewY - 1;
    DR_DOWN:
      NewY := NewY + 1;
    DR_LEFT:
      NewX := NewX - 1;
    DR_RIGHT:
      NewX := NewX + 1;
    DR_UPLEFT:begin
        NewX := NewX - 1;
        NewY := NewY - 1;
      end;
    DR_UPRIGHT:begin
        NewX := NewX + 1;
        NewY := NewY - 1;
      end;
    DR_DOWNLEFT:begin
        NewX := NewX - 1;
        NewY := NewY + 1;
      end;
    DR_DOWNRIGHT:begin
        NewX := NewX + 1;
        NewY := NewY + 1;
      end;
  end;
end;

procedure GetFrontPosition(sx, sY, dir, nFlag:Integer; var NewX, NewY:Integer);
begin
  NewX := sx;
  NewY := sY;
  case dir of
    DR_UP:
      NewY := NewY - nFlag;
    DR_DOWN:
      NewY := NewY + nFlag;
    DR_LEFT:
      NewX := NewX - nFlag;
    DR_RIGHT:
      NewX := NewX + nFlag;
    DR_UPLEFT:begin
        NewX := NewX - nFlag;
        NewY := NewY - nFlag;
      end;
    DR_UPRIGHT:begin
        NewX := NewX + nFlag;
        NewY := NewY - nFlag;
      end;
    DR_DOWNLEFT:begin
        NewX := NewX - nFlag;
        NewY := NewY + nFlag;
      end;
    DR_DOWNRIGHT:begin
        NewX := NewX + nFlag;
        NewY := NewY + nFlag;
      end;
  end;
end;

function GetFlyDirection(sx, sy, ttx, tty:Integer):Integer;
var
  fx, fy:Real;
begin
  fx := ttx - sx;
  fy := tty - sy;
  Result := DR_DOWN;
  if fx = 0 then begin
    if fy < 0 then
      Result := DR_UP
    else
      Result := DR_DOWN;
    Exit;
  end;
  if fy = 0 then begin
    if fx < 0 then
      Result := DR_LEFT
    else
      Result := DR_RIGHT;
    Exit;
  end;
  if (fx > 0) and (fy < 0) then begin
    if - fy > fx * 2.5 then
      Result := DR_UP
    else if - fy < fx / 3 then
      Result := DR_RIGHT
    else
      Result := DR_UPRIGHT;
  end;
  if (fx > 0) and (fy > 0) then begin
    if fy < fx / 3 then
      Result := DR_RIGHT
    else if fy > fx * 2.5 then
      Result := DR_DOWN
    else
      Result := DR_DOWNRIGHT;
  end;
  if (fx < 0) and (fy > 0) then begin
    if fy < -fx / 3 then
      Result := DR_LEFT
    else if fy > -fx * 2.5 then
      Result := DR_DOWN
    else
      Result := DR_DOWNLEFT;
  end;
  if (fx < 0) and (fy < 0) then begin
    if - fy > -fx * 2.5 then
      Result := DR_UP
    else if - fy < -fx / 3 then
      Result := DR_LEFT
    else
      Result := DR_UPLEFT;
  end;
end;

function GetFlyDirection16(sx, sy, ttx, tty:Integer):Integer;
var
  fx, fy:Real;
begin
  fx := ttx - sx;
  fy := tty - sy;
  Result := 0;
  if fx = 0 then begin
    if fy < 0 then
      Result := 0
    else
      Result := 8;
    Exit;
  end;
  if fy = 0 then begin
    if fx < 0 then
      Result := 12
    else
      Result := 4;
    Exit;
  end;
  if (fx > 0) and (fy < 0) then begin
    Result := 4;
    if - fy > fx / 4 then
      Result := 3;
    if - fy > fx / 1.9 then
      Result := 2;
    if - fy > fx * 1.4 then
      Result := 1;
    if - fy > fx * 4 then
      Result := 0;
  end;
  if (fx > 0) and (fy > 0) then begin
    Result := 4;
    if fy > fx / 4 then
      Result := 5;
    if fy > fx / 1.9 then
      Result := 6;
    if fy > fx * 1.4 then
      Result := 7;
    if fy > fx * 4 then
      Result := 8;
  end;
  if (fx < 0) and (fy > 0) then begin
    Result := 12;
    if fy > -fx / 4 then
      Result := 11;
    if fy > -fx / 1.9 then
      Result := 10;
    if fy > -fx * 1.4 then
      Result := 9;
    if fy > -fx * 4 then
      Result := 8;
  end;
  if (fx < 0) and (fy < 0) then begin
    Result := 12;
    if - fy > -fx / 4 then
      Result := 13;
    if - fy > -fx / 1.9 then
      Result := 14;
    if - fy > -fx * 1.4 then
      Result := 15;
    if - fy > -fx * 4 then
      Result := 0;
  end;
end;

function PrivDir(ndir:Integer):Integer;
begin
  if ndir - 1 < 0 then
    Result := 7
  else
    Result := ndir - 1;
end;

function NextDir(ndir:Integer):Integer;
begin
  if ndir + 1 > 7 then
    Result := 0
  else
    Result := ndir + 1;
end;

procedure BoldTextOut(HGEFont:THGEFont; X, Y:Integer; Str:string; FColor:TColor; BColor:TColor; Alpha:Byte);
begin
  HGEFont.TextOut(X - 1, Y, Str, BColor, 2, Alpha);
  HGEFont.TextOut(X + 1, Y, Str, BColor, 2, Alpha);
  HGEFont.TextOut(X, Y - 1, Str, BColor, 2, Alpha);
  HGEFont.TextOut(X, Y + 1, Str, BColor, 2, Alpha);
  HGEFont.TextOut(X, Y, Str, FColor, 2, Alpha);
end;

procedure BoldTextOutEx(HGEFont:THGEFont; X, Y:Integer; Str:string; FColor:TColor; BColor:TColor; Alpha:Byte);
begin
  HGEFont.TextOut(X, Y, Str, BColor, 2, Alpha);
  HGEFont.TextOut(X, Y, Str, FColor, 2, Alpha);
end;

procedure BoldTextOut(X, Y:Integer; Str:string; FColor:TColor; BColor:TColor; Alpha:Byte);
begin
  if (CurrentFont <> nil) then begin
    CurrentFont.TextOut(X - 1, Y, Str, BColor, 2, Alpha);
    CurrentFont.TextOut(X + 1, Y, Str, BColor, 2, Alpha);
    CurrentFont.TextOut(X, Y - 1, Str, BColor, 2, Alpha);
    CurrentFont.TextOut(X, Y + 1, Str, BColor, 2, Alpha);
    CurrentFont.TextOut(X, Y, Str, FColor, 2, Alpha);
  end;
end;

function GetTakeOnPosition(StdMode, Shape:Integer):Integer;
begin
  Result := -1;
  case StdMode of // StdMode
    5, 6:
      Result := U_WEAPON; // 武器
    10, 11:
      Result := U_DRESS;
    15:
      Result := U_HELMET;
    19, 20, 21:
      Result := U_NECKLACE;
    22, 23:
      Result := U_RINGL;
    24, 26:
      Result := U_ARMRINGR;
    29 {天使翅膀}, 30:
      Result := U_RIGHTHAND;
    25, 51, 96, 97:begin
        if g_ClientVersion > cv176 then
          Result := U_BUJUK
        else
          Result := U_ARMRINGL; // 符
      end;
    52, 62:begin
        if g_ClientVersion > cv176 then
          Result := U_BOOTS; // 鞋
      end;
    7, 53, 63, 94:begin
        if g_ClientVersion > cv176 then
          Result := U_CHARM
        else
          Result := U_ARMRINGL; // 宝石
      end;
    54, 64:begin
        if g_ClientVersion > cv176 then
          Result := U_BELT; // 腰带
      end;
    16:
      Result := U_HAT;
    65:
      Result := U_DRUM; // 鼓
    28:begin
        if g_ClientVersion > cv176 then
          Result := U_HORSE; // 马牌 chongchong 2013-10-12
      end;
    12:
      Result := U_SHIELD; // 盾牌 chongchong 2013-09-16
    66, 67:
      Result := U_FASHIONDRESS;
    68, 69:
      Result := U_FASHIONWEAPON;
    // 70..74: 称号
    75, 76, 77:
      Result := U_FASHIONNECKLACE; // 时装项链
    78:
      Result := U_FASHIONHELMET; // 时装头盔
    79, 80:
      Result := U_FASHIONARMRINGL; // 时装手镯
    81, 82:
      Result := U_FASHIONRINGL; // 时装戒指
    83:
      Result := U_FASHIONRIGHTHAND; // 时装照明物品
    84, 85:
      Result := U_FASHIONBELT; // 时装腰带
    86, 87:
      Result := U_FASHIONBOOTS; // 时装鞋
    88, 89:
      Result := U_FASHIONCHARM; // 时装宝石
    90:
      Result := U_JADE;
  end;
end;

function IsKeyPressed(Key:byte):Boolean;
var
  keyvalue:TKeyBoardState;
begin
  Result := FALSE;
  FillChar(keyvalue, SizeOf(TKeyBoardState), #0);
  if GetKeyboardState(keyvalue) then
    if (keyvalue[Key] and $80) <> 0 then
      Result := True;
end;

procedure AddChangeFace(recogid:Int64);
var
  I64:PInt64;
begin
  New(I64);
  I64^ := recogid;
  g_ChangeFaceReadyList.Add(I64);
end;

procedure DelChangeFace(recogid:Int64);
var
  I:Integer;
  I64:PInt64;
begin
  for I := 0 to g_ChangeFaceReadyList.Count - 1 do begin
    I64 := PInt64(g_ChangeFaceReadyList[I]);
    if I64^ = recogid then begin
      Dispose(I64);
      g_ChangeFaceReadyList.Delete(I);
      Break;
    end;
  end;
end;

function IsChangingFace(recogid:Int64):Boolean;
var
  I:Integer;
begin
  Result := FALSE;
  for I := 0 to g_ChangeFaceReadyList.Count - 1 do begin
    if PInt64(g_ChangeFaceReadyList[I])^ = recogid then begin
      Result := True;
      Break;
    end;
  end;
end;

initialization
  DropItems := TList.Create;

finalization
  DropItems.Free;

end.
