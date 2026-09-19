unit ConfigShare;

interface
uses
  Windows,
  SysUtils,
  Classes;
const
  CONFIGFILE = 'Config\%s.%s.set';
  BOSSCONFIGFILE = 'Config\%s.%s.Boss.set';
  GJMONCONFIGFILE = 'Config\%s.%s.GjMon.set';
  GJNAGICCONFIGFILE1 = 'Config\%s.%s.GjMagic1.set';
  GJNAGICCONFIGFILE2 = 'Config\%s.%s.GjMagic2.set';
  GJNAGICCONFIGFILE3 = 'Config\%s.%s.GjMagic3.set';
  MAGIC_ICONS_INI_FILE = 'Config\%s.%s.MagicIcons';
  NOTESFILE = 'Config\%s.%s.notes';

type

  TShortcutKey = record // 快捷键
    Use:Boolean;
    Key:Word;
    Shift:TShiftState;
  end;
  pTShortcutKey = ^TShortcutKey;

  TShortcutKeys = array[0..15] of TShortcutKey;
var
  g_sPlugServerName:string = '';
  g_sPlugUserName:string = '';
  g_ShortcutKeys:TShortcutKeys;

function FindBagItemName(sItemName:string):Integer;
function FindHeroBagItemName(sItemName:string):Integer;
function FindHumBindHPItemIndex():Integer; overload;
function FindHumBindHPItemIndex(sItemName:string):Integer; overload;
function FindHeroBindHPItemIndex():Integer; overload;
function FindHeroBindHPItemIndex(sItemName:string):Integer; overload;
function FindHumUnBindHPItemIndex():Integer; overload;
function FindHumUnBindHPItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;
function FindHeroUnBindHPItemIndex():Integer; overload;
function FindHeroUnBindHPItemIndex(sItemName:string):Integer; overload;

function FindHumBindSpecialItemIndex():Integer; overload;
function FindHumBindSpecialItemIndex(sItemName:string):Integer; overload;
function FindHeroBindSpecialItemIndex():Integer; overload;
function FindHeroBindSpecialItemIndex(sItemName:string):Integer; overload;
function FindHumUnBindSpecialItemIndex():Integer; overload;
function FindHumUnBindSpecialItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;
function FindHeroUnBindSpecialItemIndex():Integer; overload;
function FindHeroUnBindSpecialItemIndex(sItemName:string):Integer; overload;

function FindHumBindMPItemIndex():Integer; overload;
function FindHumBindMPItemIndex(sItemName:string):Integer; overload;
function FindHeroBindMPItemIndex():Integer; overload;
function FindHeroBindMPItemIndex(sItemName:string):Integer; overload;
function FindHumUnBindMPItemIndex():Integer; overload;
function FindHumUnBindMPItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;
function FindHeroUnBindMPItemIndex():Integer; overload;
function FindHeroUnBindMPItemIndex(sItemName:string):Integer; overload;
function FindHumUnBindBookItemIndex(sItemName:string):Integer;
function FindHumBindBookItemIndex(sItemName:string):Integer;

function FindHumHPItemIndex():Integer; overload;
function FindHumHPItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;
function FindHumMPItemIndex():Integer; overload;
function FindHumMPItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;

function FindHeroHPItemIndex():Integer; overload;
function FindHeroHPItemIndex(sItemName:string):Integer; overload;
function FindHeroMPItemIndex():Integer; overload;
function FindHeroMPItemIndex(sItemName:string):Integer; overload;

function FindHumSpecialItemIndex():Integer; overload;
function FindHumSpecialItemIndex(sItemName:string; OnEqualName:Boolean = False):Integer; overload;
function FindHeroSpecialItemIndex():Integer; overload;
function FindHeroSpecialItemIndex(sItemName:string):Integer; overload;
function FindHumBookItemIndex(sItemName:string):Integer;

function FindHumBindItemIndex(sItemName:string):Integer;
function FindHeroBindItemIndex(sItemName:string):Integer;

function GetKeyDownStr(Key:Word; Shift:TShiftState; IncludeFN:Boolean = False):string;

implementation
uses Grobal2,
  MShare;

function GetKeyDownStr(Key:Word; Shift:TShiftState; IncludeFN:Boolean):string;

function GetKey(Key:Word):string;
  begin
    if ((Key >= 48) and (Key <= 57)) or ((Key >= 65) and (Key <= 90)) or
      ((Key >= 96) and (Key <= 105)) then begin
      Result := Chr(Key);
    end;
    if Key = VK_TAB then begin
      Result := 'Tab';
    end;
    if Key = VK_SCROLL then begin
      Result := 'Scroll';
    end;
    if Key in [VK_F1..VK_F12] then begin
      case Key of
        VK_F1:Result := 'F1';
        VK_F2:Result := 'F2';
        VK_F3:Result := 'F3';
        VK_F4:Result := 'F4';
        VK_F5:Result := 'F5';
        VK_F6:Result := 'F6';
        VK_F7:Result := 'F7';
        VK_F8:Result := 'F8';
        VK_F9:Result := 'F9';
        VK_F10:Result := 'F10';
        VK_F11:Result := 'F11';
        VK_F12:Result := 'F12';
      end;
    end;
    if (Key >= 186) and (Key <= 222) then {// 其他键} begin
      case Key of
        186:Result := ';';
        187:Result := '=';
        188:Result := ',';
        189:Result := '-';
        190:Result := '.';
        191:Result := '/';
        192:Result := '`';
        219:Result := '[';
        220:Result := '\';
        221:Result := ']';
        222:Result := Char(27);
      end;
    end;

    if (Key >= 8) and (Key <= 46) then {// 方向键} begin
      case Key of
        8:Result := '退格';
        9:Result := 'Tab';
        13:Result := 'Enter';
        32:Result := '空格';
        33:Result := 'PageUp';
        34:Result := 'PageDown';
        35:Result := 'End';
        36:Result := 'Home';
        45:Result := 'Insert';
        46:Result := 'Delete';
      end;
    end;
  end;
var
  IsFN:Boolean;
begin
  Result := '';

  IsFN := False;
  if (Key >= VK_F1) and (Key <= VK_F12) then begin
    if IncludeFN then
      IsFN := True
    else
      IsFN := False;
  end;

  if (Key <> VK_MENU) and (Key <> VK_CONTROL) and (Key <> VK_SHIFT) and (Key <> VK_TAB) and (Key <> VK_SCROLL) and
    (((Key >= 48) and (Key <= 57)) or
    ((Key >= 65) and (Key <= 90)) or
    ((Key >= 96) and (Key <= 105)) or
    IsFN or
    (Key >= 186) and (Key <= 222)
    ) then begin

    if ssShift in Shift then begin
      Result := 'Shift+';
    end
    else if ssAlt in Shift then begin
      Result := 'Alt+';
    end
    else if ssCtrl in Shift then begin
      Result := 'Ctrl+';
    end
    else if ssLeft in Shift then begin
      Result := 'Left+';
    end
    else if ssRight in Shift then begin
      Result := 'Right+';
    end
    else if ssMiddle in Shift then begin
      Result := 'Middle+';
    end
    else if ssDouble in Shift then begin
      Result := 'Double+';
    end;
  end;

  Result := Result + GetKey(Key);
end;

function FindBagItemName(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_ItemArr) to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
        Result := I;
        Break;
      end;
    end;
  end;
end;

function FindHeroBagItemName(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if g_HeroItemArr[I].s.Name <> '' then begin
      if CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0 then begin
        Result := I;
        Break;
      end;
    end;
  end;
end;

function FindHumBindHPItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
      // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
      if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) and (g_ItemArr[I].s.Reserved = 0) then begin
        for II := 0 to g_UnbindItemList.Count - 1 do begin
          UnBindItem := g_UnbindItemList.Items[II];
          if (UnBindItem.UnBindItemType = t_HP) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
            Result := I;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHumBindHPItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正内挂吃药会吃31类卷轴的问题chongchong 2013-10-27
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) and (g_ItemArr[I].s.Reserved = 0) then begin
            if (UnBindItem.UnBindItemType = t_HP) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHeroBindHPItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
        // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
        if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
          for II := 0 to g_UnbindItemList.Count - 1 do begin
            UnBindItem := g_UnbindItemList.Items[II];
            if (UnBindItem.UnBindItemType = t_HP) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function FindHeroBindHPItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for II := 0 to g_UnbindItemList.Count - 1 do begin
        UnBindItem := g_UnbindItemList.Items[II];
        if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
          for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
            if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
              if (UnBindItem.UnBindItemType = t_HP) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
                Result := I;
                Exit;
              end;
            end;
          end;
          Exit;
        end;
      end;
    end;
  end;
end;

function FindHumBindMPItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
      // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
      if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) and (g_ItemArr[I].s.Reserved = 0) then begin
        for II := 0 to g_UnbindItemList.Count - 1 do begin
          UnBindItem := g_UnbindItemList.Items[II];
          if (UnBindItem.UnBindItemType = t_MP) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
            Result := I;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHumBindMPItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(sItemName, UnBindItem.sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (UnBindItem.UnBindItemType = t_MP) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHeroBindMPItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
        // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
        if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
          for II := 0 to g_UnbindItemList.Count - 1 do begin
            UnBindItem := g_UnbindItemList.Items[II];
            if (UnBindItem.UnBindItemType = t_MP) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function FindHeroBindMPItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for II := 0 to g_UnbindItemList.Count - 1 do begin
        UnBindItem := g_UnbindItemList.Items[II];
        if (CompareText(sItemName, UnBindItem.sItemName) = 0) then begin
          for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
            if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
              if (UnBindItem.UnBindItemType = t_MP) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
                Result := I;
                Exit;
              end;
            end;
          end;
          Exit;
        end;
      end;
    end;
  end;
end;

function FindHumBindSpecialItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
      // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
      if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) and (g_ItemArr[I].s.Reserved = 0) then begin
        for II := 0 to g_UnbindItemList.Count - 1 do begin
          UnBindItem := g_UnbindItemList.Items[II];
          if (UnBindItem.UnBindItemType = t_Special) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
            Result := I;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHumBindSpecialItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (UnBindItem.UnBindItemType = t_Special) and (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHeroBindSpecialItemIndex():Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
        // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
        if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
          for II := 0 to g_UnbindItemList.Count - 1 do begin
            UnBindItem := g_UnbindItemList.Items[II];
            if (UnBindItem.UnBindItemType = t_Special) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function FindHeroBindSpecialItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if (g_MyHero <> nil) then begin
    if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
      for II := 0 to g_UnbindItemList.Count - 1 do begin
        UnBindItem := g_UnbindItemList.Items[II];
        if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
          for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
            if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
              if (UnBindItem.UnBindItemType = t_Special) and (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
                Result := I;
                Exit;
              end;
            end;
          end;
          Exit;
        end;
      end;
    end;
  end;
end;

function FindHumUnBindHPItemIndex():Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape in [0, 1]) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindHPItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindHPItemIndex():Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if g_HeroItemArr[I].s.Name <> '' then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape = 0) and (g_HeroItemArr[I].s.AC1 > 0) and (g_HeroItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindHPItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0 then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape = 0) and (g_HeroItemArr[I].s.AC1 > 0) and (g_HeroItemArr[I].s.MAC1 = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindSpecialItemIndex():Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape > 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape > 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindSpecialItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape > 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape > 0) and (g_ItemArr[I].s.AC1 > 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindSpecialItemIndex():Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if g_HeroItemArr[I].s.Name <> '' then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape > 0) and (g_HeroItemArr[I].s.AC1 > 0) and (g_HeroItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindSpecialItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0 then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape > 0) and (g_HeroItemArr[I].s.AC1 > 0) and (g_HeroItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindMPItemIndex():Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}
  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 = 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
  for I := Low(g_ItemArr) to 5 do begin
    if g_ItemArr[I].s.Name <> '' then begin
      if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 = 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindMPItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 = 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
  for I := Low(g_ItemArr) to 5 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if OnEqualName then begin
        Result := I;
        Exit;
      end
      else if (g_ItemArr[I].s.StdMode = 0) and (g_ItemArr[I].s.Shape = 0) and (g_ItemArr[I].s.AC1 = 0) and (g_ItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindMPItemIndex():Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if g_HeroItemArr[I].s.Name <> '' then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape = 0) and (g_HeroItemArr[I].s.AC1 = 0) and (g_HeroItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroUnBindMPItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0 then begin
      if (g_HeroItemArr[I].s.StdMode = 0) and (g_HeroItemArr[I].s.Shape = 0) and (g_HeroItemArr[I].s.AC1 = 0) and (g_HeroItemArr[I].s.MAC1 > 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumUnBindBookItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}
  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if (g_ItemArr[I].s.Name <> '') and (CompareText(g_ItemArr[I].s.Name, sItemName) = 0) then begin
      Result := I;
      Exit;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if (g_ItemArr[I].s.Name <> '') and (CompareText(g_ItemArr[I].s.Name, sItemName) = 0) then begin
      Result := I;
      Exit;
    end;
  end;
end;

function FindHumBindBookItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正吃回城卷会吃掉卷轴 + (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHumBindItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正吃回城卷会吃掉卷轴 + (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHeroBindItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTUnBindItem;
begin
  Result := -1;
  if g_MyHero = nil then Exit;
  if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
          // 修正吃回城卷会吃掉卷轴 + (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
          if (g_HeroItemArr[I].s.Name <> '') and (g_HeroItemArr[I].s.StdMode = 31) and (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (g_HeroItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
        Exit;
      end;
    end;
  end;
end;

function FindHumHPItemIndex():Integer;
begin
  Result := FindHumUnBindHPItemIndex;
  if Result < 0 then
    Result := FindHumBindHPItemIndex;
end;

function FindHumHPItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
begin
  Result := FindHumUnBindHPItemIndex(sItemName, OnEqualName);
  if Result < 0 then
    Result := FindHumBindHPItemIndex(sItemName);
end;

function FindHumMPItemIndex():Integer;
begin
  Result := FindHumUnBindMPItemIndex;
  if Result < 0 then
    Result := FindHumBindMPItemIndex;
end;

function FindHumMPItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
begin
  Result := FindHumUnBindMPItemIndex(sItemName);
  if Result < 0 then
    Result := FindHumBindMPItemIndex(sItemName);
end;

function FindHeroHPItemIndex():Integer;
begin
  Result := FindHeroUnBindHPItemIndex;
  if Result < 0 then
    Result := FindHeroBindHPItemIndex;
end;

function FindHeroHPItemIndex(sItemName:string):Integer;
begin
  Result := FindHeroUnBindHPItemIndex(sItemName);
  if Result < 0 then
    Result := FindHeroBindHPItemIndex(sItemName);
end;

function FindHeroMPItemIndex():Integer;
begin
  Result := FindHeroUnBindMPItemIndex;
  if Result < 0 then
    Result := FindHeroBindMPItemIndex;
end;

function FindHeroMPItemIndex(sItemName:string):Integer;
begin
  Result := FindHeroUnBindMPItemIndex(sItemName);
  if Result < 0 then
    Result := FindHeroBindMPItemIndex(sItemName);
end;

function FindHumSpecialItemIndex():Integer;
begin
  Result := FindHumUnBindSpecialItemIndex;
  if Result < 0 then
    Result := FindHumBindSpecialItemIndex;
end;

function FindHumSpecialItemIndex(sItemName:string; OnEqualName:Boolean):Integer;
begin
  Result := FindHumUnBindSpecialItemIndex(sItemName, OnEqualName);
  if Result < 0 then
    Result := FindHumBindSpecialItemIndex(sItemName);
end;

function FindHeroSpecialItemIndex():Integer;
begin
  Result := FindHeroUnBindSpecialItemIndex;
  if Result < 0 then
    Result := FindHeroBindSpecialItemIndex;
end;

function FindHeroSpecialItemIndex(sItemName:string):Integer;
begin
  Result := FindHeroUnBindSpecialItemIndex(sItemName);
  if Result < 0 then
    Result := FindHeroBindSpecialItemIndex(sItemName);
end;

function FindHumBookItemIndex(sItemName:string):Integer;
begin
  Result := FindHumUnBindBookItemIndex(sItemName);
  if Result < 0 then
    Result := FindHumBindBookItemIndex(sItemName);
end;

end.
