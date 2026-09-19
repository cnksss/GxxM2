unit ItmUnit;

interface

uses
  Windows, Classes, SysUtils, Grobal2, Math;

type
  TItemUnit = class
  private
    function GetRandomRange(nCount, nRate: Integer): Integer;
  public
    constructor Create();
    destructor Destroy; override;
    procedure GetItemAddValue(UserItem: pTUserItem; var StdItem: TStdItem);
    procedure ItemNewAbilRandomUpgrade(UserItem: pTUserItem; btWhere: Byte); // 新属性

    procedure RandomUpgradeWeapon(UserItem: pTUserItem);
    procedure RandomUpgradeDress(UserItem: pTUserItem);
    procedure RandomUpgrade19(UserItem: pTUserItem);
    procedure RandomUpgrade202124(UserItem: pTUserItem);
    procedure RandomUpgrade26(UserItem: pTUserItem);
    procedure RandomUpgrade22(UserItem: pTUserItem);
    procedure RandomUpgrade23(UserItem: pTUserItem);
    procedure RandomUpgradeHelMet(UserItem: pTUserItem);
    procedure UnknowHelmet(UserItem: pTUserItem);
    procedure UnknowRing(UserItem: pTUserItem);
    procedure UnknowNecklace(UserItem: pTUserItem);

    procedure RandomUpgradeFashionDress(UserItem: pTUserItem);
    procedure RandomUpgradeFashionWeapon(UserItem: pTUserItem);
    procedure RandomUpgradeHorse(UserItem: pTUserItem);
    procedure RandomUpgradeDrum(UserItem: pTUserItem);
    procedure RandomUpgradeShield(UserItem: pTUserItem);

    // 20080503  鞋子腰带极品
    procedure RandomUpgradeBoots(UserItem: pTUserItem);
  end;

implementation

uses
  HUtil32, M2Share;

{ TItemUnit }

constructor TItemUnit.Create;
begin
end;

destructor TItemUnit.Destroy;
begin
  inherited;
end;

function TItemUnit.GetRandomRange(nCount, nRate: Integer): Integer; // 00494794
var
  I: Integer;
begin
  Result := 0;
  for I := 0 to nCount - 1 do
    if Random(nRate) = 0 then
      Inc(Result);
end;

procedure TItemUnit.ItemNewAbilRandomUpgrade(UserItem: pTUserItem; btWhere: Byte); // 新属性
var
  nIndex, nValue: Integer;
  nMaxIndex: Integer;
begin
  if btWhere in [Low(g_Config.ItemNewAbil)..High(g_Config.ItemNewAbil) { 0..12 } ] then
  begin
    nMaxIndex := High(g_Config.ItemNewAbil[0]);
    if g_Config.ItemNewAbil[btWhere][High(g_Config.ItemNewAbil[0]) { 12 } ] then
    begin // 随机选择一个
      nIndex := Random(nMaxIndex);
      if (nIndex >= 0) and (nIndex <= nMaxIndex - 1) and (Random(g_Config.ItemNewAbilAddRate[btWhere]) = 0) then
      begin
        if nIndex in [1, 7, 8, 10, 22] then
        begin
          nValue := GetRandomRange(g_Config.ItemNewAbilAddValueMaxLimit2[btWhere], g_Config.ItemNewAbilAddValueRate[btWhere]);
          UserItem.btNewValue[nIndex] := _MIN(nValue + 1, g_Config.ItemNewAbilAddValueMaxLimit2[btWhere]);
        end
        else
        begin
          nValue := GetRandomRange(g_Config.ItemNewAbilAddValueMaxLimit[btWhere], g_Config.ItemNewAbilAddValueRate[btWhere]);
          UserItem.btNewValue[nIndex] := _MIN(nValue + 1, g_Config.ItemNewAbilAddValueMaxLimit[btWhere]);
        end;
      end;
    end
    else
    begin
      for nIndex := 0 to nMaxIndex - 1 do
      begin
        if g_Config.ItemNewAbil[btWhere][nIndex] then
        begin
          if (Random(g_Config.ItemNewAbilAddRate[btWhere]) = 0) then
          begin
            if nIndex in [1, 7, 8, 10, 22] then
            begin
              nValue := GetRandomRange(g_Config.ItemNewAbilAddValueMaxLimit2[btWhere], g_Config.ItemNewAbilAddValueRate[btWhere]);
              UserItem.btNewValue[nIndex] := _MIN(nValue + 1, g_Config.ItemNewAbilAddValueMaxLimit2[btWhere]);
            end
            else
            begin
              nValue := GetRandomRange(g_Config.ItemNewAbilAddValueMaxLimit[btWhere], g_Config.ItemNewAbilAddValueRate[btWhere]);
              UserItem.btNewValue[nIndex] := _MIN(nValue + 1, g_Config.ItemNewAbilAddValueMaxLimit[btWhere]);
            end;
          end;
        end;
      end;
    end;
  end;
end;

procedure TItemUnit.RandomUpgradeWeapon(UserItem: pTUserItem); // 随机升级武器
var
  nC, n10, n14: Integer;
begin
  nC := GetRandomRange(g_Config.nWeaponDCAddValueMaxLimit, g_Config.nWeaponDCAddValueRate);
  if (g_Config.nWeaponDCAddRate > 0) and (Random(g_Config.nWeaponDCAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nWeaponDCAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nWeaponDCAddValueMaxLimit; // 限制上限
  end;

  nC := GetRandomRange(g_Config.nWeaponHitSpeedAddValueMaxLimit, g_Config.nWeaponHitSpeedAddValueRate); // 武器攻击速度
  if (g_Config.nWeaponHitSpeedAddRate > 0) and (Random(g_Config.nWeaponHitSpeedAddRate) = 0) then
  begin
    n14 := (nC + 1) div 3;
    if n14 > 0 then
    begin
      UserItem.btValue[6] := n14;
      { TODO -ochongchong -c新增 : 去掉极品武器有一定的机率攻击速度加10 【2013-08-26】 }
      {
        if Random(3) <> 0 then
        begin
        UserItem.btValue[6] := n14;
        end else
        begin
        UserItem.btValue[6] := n14 + 10;
        end;
      }
    end;
  end;

  nC := GetRandomRange(g_Config.nWeaponMCAddValueMaxLimit, g_Config.nWeaponMCAddValueRate);
  if (g_Config.nWeaponMCAddRate > 0) and (Random(g_Config.nWeaponMCAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nWeaponMCAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nWeaponMCAddValueMaxLimit; // 20080724 限制上限
  end;

  nC := GetRandomRange(g_Config.nWeaponSCAddValueMaxLimit, g_Config.nWeaponSCAddValueRate);
  if (g_Config.nWeaponSCAddRate > 0) and (Random(g_Config.nWeaponSCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nWeaponSCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nWeaponSCAddValueMaxLimit; // 20080724 限制上限
  end;

  nC := GetRandomRange(12, 15);
  if Random(15) = 0 then
  begin
    UserItem.btValue[5] := nC div 2 + 1;
  end;

  nC := GetRandomRange(12, 12);
  if Random(3) < 2 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;

  nC := GetRandomRange(12, 15);
  if Random(10) = 0 then
  begin
    UserItem.btValue[7] := nC div 2 + 1;
  end;
end;

procedure TItemUnit.RandomUpgradeFashionWeapon(UserItem: pTUserItem); // 随机升级武器
var
  nC, n10, n14: Integer;
begin
  nC := GetRandomRange(g_Config.nFashionWeaponDCAddValueMaxLimit, g_Config.nFashionWeaponDCAddValueRate);
  if (g_Config.nFashionWeaponDCAddRate > 0) and (Random(g_Config.nFashionWeaponDCAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nFashionWeaponDCAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nFashionWeaponDCAddValueMaxLimit; // 限制上限
  end;

  nC := GetRandomRange(g_Config.nFashionWeaponHitSpeedAddValueMaxLimit, g_Config.nFashionWeaponHitSpeedAddValueRate);
    // 武器攻击速度
  if (g_Config.nFashionWeaponHitSpeedAddRate > 0) and (Random(g_Config.nFashionWeaponHitSpeedAddRate) = 0) then
  begin
    n14 := (nC + 1) div 3;
    if n14 > 0 then
    begin
      UserItem.btValue[6] := n14;
    end;
  end;

  nC := GetRandomRange(g_Config.nFashionWeaponMCAddValueMaxLimit, g_Config.nFashionWeaponMCAddValueRate);
  if (g_Config.nFashionWeaponMCAddRate > 0) and (Random(g_Config.nFashionWeaponMCAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nFashionWeaponMCAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nFashionWeaponMCAddValueMaxLimit; // 20080724 限制上限
  end;

  nC := GetRandomRange(g_Config.nFashionWeaponSCAddValueMaxLimit, g_Config.nFashionWeaponSCAddValueRate);
  if (g_Config.nFashionWeaponSCAddRate > 0) and (Random(g_Config.nFashionWeaponSCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nFashionWeaponSCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nFashionWeaponSCAddValueMaxLimit; // 20080724 限制上限
  end;

  nC := GetRandomRange(12, 15);
  if Random(15) = 0 then
  begin
    UserItem.btValue[5] := nC div 2 + 1;
  end;

  nC := GetRandomRange(12, 12);
  if Random(3) < 2 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;

  nC := GetRandomRange(12, 15);
  if Random(10) = 0 then
  begin
    UserItem.btValue[7] := nC div 2 + 1;
  end
end;

procedure TItemUnit.RandomUpgradeDress(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nDressACAddValueMaxLimit, g_Config.nDressACAddValueRate);
  if (g_Config.nDressACAddRate > 0) and (Random(g_Config.nDressACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nDressACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nDressACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDressMACAddValueMaxLimit, g_Config.nDressMACAddValueRate);
  if (g_Config.nDressMACAddRate > 0) and (Random(g_Config.nDressMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nDressMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nDressMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDressDCAddValueMaxLimit, g_Config.nDressDCAddValueRate);
  if (g_Config.nDressDCAddRate > 0) and (Random(g_Config.nDressDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nDressDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nDressDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDressMCAddValueMaxLimit, g_Config.nDressMCAddValueRate);
  if (g_Config.nDressMCAddRate > 0) and (Random(g_Config.nDressMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nDressMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nDressMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDressSCAddValueMaxLimit, g_Config.nDressSCAddValueRate);
  if (g_Config.nDressSCAddRate > 0) and (Random(g_Config.nDressSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nDressSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nDressSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(8) < 6 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

// 时装衣服 piaoyun 2013-10-27

procedure TItemUnit.RandomUpgradeFashionDress(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nFashionDressACAddValueMaxLimit, g_Config.nFashionDressACAddValueRate);
  if (g_Config.nFashionDressACAddRate > 0) and (Random(g_Config.nFashionDressACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nFashionDressACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nFashionDressACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nFashionDressMACAddValueMaxLimit, g_Config.nFashionDressMACAddValueRate);
  if (g_Config.nFashionDressMACAddRate > 0) and (Random(g_Config.nFashionDressMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nFashionDressMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nFashionDressMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nFashionDressDCAddValueMaxLimit, g_Config.nFashionDressDCAddValueRate);
  if (g_Config.nFashionDressDCAddRate > 0) and (Random(g_Config.nFashionDressDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nFashionDressDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nFashionDressDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nFashionDressMCAddValueMaxLimit, g_Config.nFashionDressMCAddValueRate);
  if (g_Config.nFashionDressMCAddRate > 0) and (Random(g_Config.nFashionDressMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nFashionDressMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nFashionDressMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nFashionDressSCAddValueMaxLimit, g_Config.nFashionDressSCAddValueRate);
  if (g_Config.nFashionDressSCAddRate > 0) and (Random(g_Config.nFashionDressSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nFashionDressSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nFashionDressSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(8) < 6 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

// 马牌 piaoyun 2013-10-27

procedure TItemUnit.RandomUpgradeHorse(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nHorseACAddValueMaxLimit, g_Config.nHorseACAddValueRate);
  if (g_Config.nHorseACAddRate > 0) and (Random(g_Config.nHorseACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nHorseACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nHorseACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHorseMACAddValueMaxLimit, g_Config.nHorseMACAddValueRate);
  if (g_Config.nHorseMACAddRate > 0) and (Random(g_Config.nHorseMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nHorseMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nHorseMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHorseDCAddValueMaxLimit, g_Config.nHorseDCAddValueRate);
  if (g_Config.nHorseDCAddRate > 0) and (Random(g_Config.nHorseDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nHorseDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nHorseDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHorseMCAddValueMaxLimit, g_Config.nHorseMCAddValueRate);
  if (g_Config.nHorseMCAddRate > 0) and (Random(g_Config.nHorseMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nHorseMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nHorseMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHorseSCAddValueMaxLimit, g_Config.nHorseSCAddValueRate);
  if (g_Config.nHorseSCAddRate > 0) and (Random(g_Config.nHorseSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nHorseSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nHorseSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(8) < 6 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

// 军鼓 piaoyun 2013-10-27

procedure TItemUnit.RandomUpgradeDrum(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nDrumACAddValueMaxLimit, g_Config.nDrumACAddValueRate);
  if (g_Config.nDrumACAddRate > 0) and (Random(g_Config.nDrumACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nDrumACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nDrumACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDrumMACAddValueMaxLimit, g_Config.nDrumMACAddValueRate);
  if (g_Config.nDrumMACAddRate > 0) and (Random(g_Config.nDrumMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nDrumMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nDrumMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDrumDCAddValueMaxLimit, g_Config.nDrumDCAddValueRate);
  if (g_Config.nDrumDCAddRate > 0) and (Random(g_Config.nDrumDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nDrumDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nDrumDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDrumMCAddValueMaxLimit, g_Config.nDrumMCAddValueRate);
  if (g_Config.nDrumMCAddRate > 0) and (Random(g_Config.nDrumMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nDrumMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nDrumMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nDrumSCAddValueMaxLimit, g_Config.nDrumSCAddValueRate);
  if (g_Config.nDrumSCAddRate > 0) and (Random(g_Config.nDrumSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nDrumSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nDrumSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(8) < 6 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

// 盾牌 piaoyun 2013-10-27

procedure TItemUnit.RandomUpgradeShield(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nShieldACAddValueMaxLimit, g_Config.nShieldACAddValueRate);
  if (g_Config.nShieldACAddRate > 0) and (Random(g_Config.nShieldACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nShieldACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nShieldACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nShieldMACAddValueMaxLimit, g_Config.nShieldMACAddValueRate);
  if (g_Config.nShieldMACAddRate > 0) and (Random(g_Config.nShieldMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nShieldMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nShieldMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nShieldDCAddValueMaxLimit, g_Config.nShieldDCAddValueRate);
  if (g_Config.nShieldDCAddRate > 0) and (Random(g_Config.nShieldDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nShieldDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nShieldDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nShieldMCAddValueMaxLimit, g_Config.nShieldMCAddValueRate);
  if (g_Config.nShieldMCAddRate > 0) and (Random(g_Config.nShieldMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nShieldMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nShieldMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nShieldSCAddValueMaxLimit, g_Config.nShieldSCAddValueRate);
  if (g_Config.nShieldSCAddRate > 0) and (Random(g_Config.nShieldSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nShieldSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nShieldSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(8) < 6 then
  begin
    n10 := (nC + 1) * 2000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

procedure TItemUnit.RandomUpgrade202124(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nNeckLace202124ACAddValueMaxLimit, g_Config.nNeckLace202124ACAddValueRate);
  if (g_Config.nNeckLace202124ACAddRate > 0) and (Random(g_Config.nNeckLace202124ACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nNeckLace202124ACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nNeckLace202124ACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace202124MACAddValueMaxLimit, g_Config.nNeckLace202124MACAddValueRate);
  if (g_Config.nNeckLace202124MACAddRate > 0) and (Random(g_Config.nNeckLace202124MACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nNeckLace202124MACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nNeckLace202124MACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace202124DCAddValueMaxLimit, g_Config.nNeckLace202124DCAddValueRate);
  if (g_Config.nNeckLace202124DCAddRate > 0) and (Random(g_Config.nNeckLace202124DCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nNeckLace202124DCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nNeckLace202124DCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace202124MCAddValueMaxLimit, g_Config.nNeckLace202124MCAddValueRate);
  if (g_Config.nNeckLace202124MCAddRate > 0) and (Random(g_Config.nNeckLace202124MCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nNeckLace202124MCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nNeckLace202124MCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace202124SCAddValueMaxLimit, g_Config.nNeckLace202124SCAddValueRate);
  if (g_Config.nNeckLace202124SCAddRate > 0) and (Random(g_Config.nNeckLace202124SCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nNeckLace202124SCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nNeckLace202124SCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(20) < 15 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

procedure TItemUnit.RandomUpgrade26(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nArmRing26ACAddValueMaxLimit, g_Config.nArmRing26ACAddValueRate);
  if (g_Config.nArmRing26ACAddRate > 0) and (Random(g_Config.nArmRing26ACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nArmRing26ACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nArmRing26ACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nArmRing26MACAddValueMaxLimit, g_Config.nArmRing26MACAddValueRate);
  if (g_Config.nArmRing26MACAddRate > 0) and (Random(g_Config.nArmRing26MACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nArmRing26MACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nArmRing26MACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nArmRing26DCAddValueMaxLimit, g_Config.nArmRing26DCAddValueRate);
  if (g_Config.nArmRing26DCAddRate > 0) and (Random(g_Config.nArmRing26DCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nArmRing26DCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nArmRing26DCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nArmRing26MCAddValueMaxLimit, g_Config.nArmRing26MCAddValueRate);
  if (g_Config.nArmRing26MCAddRate > 0) and (Random(g_Config.nArmRing26MCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nArmRing26MCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nArmRing26MCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nArmRing26SCAddValueMaxLimit, g_Config.nArmRing26SCAddValueRate);
  if (g_Config.nArmRing26SCAddRate > 0) and (Random(g_Config.nArmRing26SCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nArmRing26SCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nArmRing26SCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(20) < 15 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;
// 随机升级-分类19物品(幸运类项链)

procedure TItemUnit.RandomUpgrade19(UserItem: pTUserItem); // 00494D60
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nNeckLace19ACAddValueMaxLimit, g_Config.nNeckLace19ACAddValueRate);
  if (g_Config.nNeckLace19ACAddRate > 0) and (Random(g_Config.nNeckLace19ACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nNeckLace19ACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nNeckLace19ACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace19MACAddValueMaxLimit, g_Config.nNeckLace19MACAddValueRate);
  if (g_Config.nNeckLace19MACAddRate > 0) and (Random(g_Config.nNeckLace19MACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nNeckLace19MACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nNeckLace19MACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace19DCAddValueMaxLimit, g_Config.nNeckLace19DCAddValueRate);
  if (g_Config.nNeckLace19DCAddRate > 0) and (Random(g_Config.nNeckLace19DCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nNeckLace19DCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nNeckLace19DCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace19MCAddValueMaxLimit, g_Config.nNeckLace19MCAddValueRate);
  if (g_Config.nNeckLace19MCAddRate > 0) and (Random(g_Config.nNeckLace19MCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nNeckLace19MCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nNeckLace19MCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nNeckLace19SCAddValueMaxLimit, g_Config.nNeckLace19SCAddValueRate);
  if (g_Config.nNeckLace19SCAddRate > 0) and (Random(g_Config.nNeckLace19SCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nNeckLace19SCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nNeckLace19SCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 10);
  if Random(4) < 3 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

procedure TItemUnit.RandomUpgrade22(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nRing22ACAddValueMaxLimit, g_Config.nRing22ACAddValueRate);
  if (g_Config.nRing22ACAddRate > 0) and (Random(g_Config.nRing22ACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nRing22ACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nRing22ACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing22MACAddValueMaxLimit, g_Config.nRing22MACAddValueRate);
  if (g_Config.nRing22MACAddRate > 0) and (Random(g_Config.nRing22MACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nRing22MACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nRing22MACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing22DCAddValueMaxLimit, g_Config.nRing22DCAddValueRate);
  if (g_Config.nRing22DCAddRate > 0) and (Random(g_Config.nRing22DCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nRing22DCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nRing22DCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing22MCAddValueMaxLimit, g_Config.nRing22MCAddValueRate);
  if (g_Config.nRing22MCAddRate > 0) and (Random(g_Config.nRing22MCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nRing22MCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nRing22MCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing22SCAddValueMaxLimit, g_Config.nRing22SCAddValueRate);
  if (g_Config.nRing22SCAddRate > 0) and (Random(g_Config.nRing22SCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nRing22SCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nRing22SCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(4) < 3 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

procedure TItemUnit.RandomUpgrade23(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nRing23ACAddValueMaxLimit, g_Config.nRing23ACAddValueRate);
  if (g_Config.nRing23ACAddRate > 0) and (Random(g_Config.nRing23ACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nRing23ACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nRing23ACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing23MACAddValueMaxLimit, g_Config.nRing23MACAddValueRate);
  if (g_Config.nRing23MACAddRate > 0) and (Random(g_Config.nRing23MACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nRing23MACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nRing23MACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing23DCAddValueMaxLimit, g_Config.nRing23DCAddValueRate);
  if (g_Config.nRing23DCAddRate > 0) and (Random(g_Config.nRing23DCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nRing23DCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nRing23DCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing23MCAddValueMaxLimit, g_Config.nRing23MCAddValueRate);
  if (g_Config.nRing23MCAddRate > 0) and (Random(g_Config.nRing23MCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nRing23MCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nRing23MCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nRing23SCAddValueMaxLimit, g_Config.nRing23SCAddValueRate);
  if (g_Config.nRing23SCAddRate > 0) and (Random(g_Config.nRing23SCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nRing23SCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nRing23SCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(4) < 3 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;
// 头盔,斗笠 极品属性

procedure TItemUnit.RandomUpgradeHelMet(UserItem: pTUserItem); // 00495110
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nHelMetACAddValueMaxLimit, g_Config.nHelMetACAddValueRate);
  if (g_Config.nHelMetACAddRate > 0) and (Random(g_Config.nHelMetACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1;
    if UserItem.btValue[0] > g_Config.nHelMetACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nHelMetACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHelMetMACAddValueMaxLimit, g_Config.nHelMetMACAddValueRate);
  if (g_Config.nHelMetMACAddRate > 0) and (Random(g_Config.nHelMetMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1;
    if UserItem.btValue[1] > g_Config.nHelMetMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nHelMetMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHelMetDCAddValueMaxLimit, g_Config.nHelMetDCAddValueRate);
  if (g_Config.nHelMetDCAddRate > 0) and (Random(g_Config.nHelMetDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1;
    if UserItem.btValue[2] > g_Config.nHelMetDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nHelMetDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHelMetMCAddValueMaxLimit, g_Config.nHelMetMCAddValueRate);
  if (g_Config.nHelMetMCAddRate > 0) and (Random(g_Config.nHelMetMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1;
    if UserItem.btValue[3] > g_Config.nHelMetMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nHelMetMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nHelMetSCAddValueMaxLimit, g_Config.nHelMetSCAddValueRate);
  if (g_Config.nHelMetSCAddRate > 0) and (Random(g_Config.nHelMetSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1;
    if UserItem.btValue[4] > g_Config.nHelMetSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nHelMetSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(4) < 3 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;


// 20080503  鞋子腰带极品

procedure TItemUnit.RandomUpgradeBoots(UserItem: pTUserItem);
var
  nC, n10: Integer;
begin
  nC := GetRandomRange(g_Config.nBootsACAddValueMaxLimit, g_Config.nBootsACAddValueRate);
  if (g_Config.nBootsACAddRate > 0) and (Random(g_Config.nBootsACAddRate) = 0) then
  begin
    UserItem.btValue[0] := nC + 1; // 防御
    if UserItem.btValue[0] > g_Config.nBootsACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nBootsACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nBootsMACAddValueMaxLimit, g_Config.nBootsMACAddValueRate);
  if (g_Config.nBootsMACAddRate > 0) and (Random(g_Config.nBootsMACAddRate) = 0) then
  begin
    UserItem.btValue[1] := nC + 1; // 魔御
    if UserItem.btValue[1] > g_Config.nBootsMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nBootsMACAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nBootsDCAddValueMaxLimit, g_Config.nBootsDCAddValueRate);
  if (g_Config.nBootsDCAddRate > 0) and (Random(g_Config.nBootsDCAddRate) = 0) then
  begin
    UserItem.btValue[2] := nC + 1; // 攻击力
    if UserItem.btValue[2] > g_Config.nBootsDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nBootsDCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nBootsMCAddValueMaxLimit, g_Config.nBootsMCAddValueRate);
  if (g_Config.nBootsMCAddRate > 0) and (Random(g_Config.nBootsMCAddRate) = 0) then
  begin
    UserItem.btValue[3] := nC + 1; // 魔法
    if UserItem.btValue[3] > g_Config.nBootsMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nBootsMCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(g_Config.nBootsSCAddValueMaxLimit, g_Config.nBootsSCAddValueRate);
  if (g_Config.nBootsSCAddRate > 0) and (Random(g_Config.nBootsSCAddRate) = 0) then
  begin
    UserItem.btValue[4] := nC + 1; // 道术
    if UserItem.btValue[4] > g_Config.nBootsSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nBootsSCAddValueMaxLimit; // 20080724 限制上线
  end;

  nC := GetRandomRange(6, 12);
  if Random(4) < 3 then
  begin
    n10 := (nC + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + n10);
    UserItem.Dura := _MIN(65000, UserItem.Dura + n10);
  end;
end;

procedure TItemUnit.UnknowHelmet(UserItem: pTUserItem); // 神秘头盔
var
  nC, nRandPoint, n14: Integer;
begin
  nRandPoint := GetRandomRange(g_Config.nUnknowHelMetACAddValueMaxLimit, g_Config.nUnknowHelMetACAddRate);
  if nRandPoint > 0 then
  begin
    UserItem.btValue[0] := nRandPoint;
    if UserItem.btValue[0] > g_Config.nUnknowHelMetACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nUnknowHelMetACAddValueMaxLimit; // 20080724 限制上线
  end;
  n14 := nRandPoint;
  nRandPoint := GetRandomRange(g_Config.nUnknowHelMetMACAddValueMaxLimit, g_Config.nUnknowHelMetMACAddRate);
  if nRandPoint > 0 then
  begin
    UserItem.btValue[1] := nRandPoint;
    if UserItem.btValue[1] > g_Config.nUnknowHelMetMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nUnknowHelMetMACAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, nRandPoint);
  nRandPoint := GetRandomRange(g_Config.nUnknowHelMetDCAddValueMaxLimit, g_Config.nUnknowHelMetDCAddRate);
  if nRandPoint > 0 then
  begin
    UserItem.btValue[2] := nRandPoint;
    if UserItem.btValue[2] > g_Config.nUnknowHelMetDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nUnknowHelMetDCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, nRandPoint);
  nRandPoint := GetRandomRange(g_Config.nUnknowHelMetMCAddValueMaxLimit, g_Config.nUnknowHelMetMCAddRate);
  if nRandPoint > 0 then
  begin
    UserItem.btValue[3] := nRandPoint;
    if UserItem.btValue[3] > g_Config.nUnknowHelMetMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nUnknowHelMetMCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, nRandPoint);
  nRandPoint := GetRandomRange(g_Config.nUnknowHelMetSCAddValueMaxLimit, g_Config.nUnknowHelMetSCAddRate);
  if nRandPoint > 0 then
  begin
    UserItem.btValue[4] := nRandPoint;
    if UserItem.btValue[4] > g_Config.nUnknowHelMetSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nUnknowHelMetSCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, nRandPoint);
  nRandPoint := GetRandomRange(6, 30);
  if nRandPoint > 0 then
  begin
    nC := (nRandPoint + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + nC);
    UserItem.Dura := _MIN(65000, UserItem.Dura + nC);
  end;
  if Random(30) = 0 then
    UserItem.btValue[7] := 1;
  UserItem.btValue[8] := 1;
  if n14 >= 3 then
  begin
    if UserItem.btValue[0] >= 5 then
    begin
      UserItem.btValue[5] := 1;
      UserItem.btValue[6] := UserItem.btValue[0] * 3 + 25;
      Exit;
    end;
    if UserItem.btValue[2] >= 2 then
    begin
      UserItem.btValue[5] := 1;
      UserItem.btValue[6] := UserItem.btValue[2] * 4 + 35;
      Exit;
    end;
    if UserItem.btValue[3] >= 2 then
    begin
      UserItem.btValue[5] := 2;
      UserItem.btValue[6] := UserItem.btValue[3] * 2 + 18;
      Exit;
    end;
    if UserItem.btValue[4] >= 2 then
    begin
      UserItem.btValue[5] := 3;
      UserItem.btValue[6] := UserItem.btValue[4] * 2 + 18;
      Exit;
    end;
    UserItem.btValue[6] := n14 * 2 + 18;
  end;
end;

procedure TItemUnit.UnknowRing(UserItem: pTUserItem); // 神秘戒指
var
  nC, n10, n14: Integer;
begin
  n10 := GetRandomRange(g_Config.nUnknowRingACAddValueMaxLimit, g_Config.nUnknowRingACAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[0] := n10;
    if UserItem.btValue[0] > g_Config.nUnknowRingACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nUnknowRingACAddValueMaxLimit; // 20080724 限制上线
  end;
  n14 := n10;
  n10 := GetRandomRange(g_Config.nUnknowRingMACAddValueMaxLimit, g_Config.nUnknowRingMACAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[1] := n10;
    if UserItem.btValue[1] > g_Config.nUnknowRingMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nUnknowRingMACAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);

  n10 := GetRandomRange(g_Config.nUnknowRingDCAddValueMaxLimit, g_Config.nUnknowRingDCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[2] := n10;
    if UserItem.btValue[2] > g_Config.nUnknowRingDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nUnknowRingDCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(g_Config.nUnknowRingMCAddValueMaxLimit, g_Config.nUnknowRingMCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[3] := n10;
    if UserItem.btValue[3] > g_Config.nUnknowRingMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nUnknowRingMCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(g_Config.nUnknowRingSCAddValueMaxLimit, g_Config.nUnknowRingSCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[4] := n10;
    if UserItem.btValue[4] > g_Config.nUnknowRingSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nUnknowRingSCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(6, 30);
  if n10 > 0 then
  begin
    nC := (n10 + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + nC);
    UserItem.Dura := _MIN(65000, UserItem.Dura + nC);
  end;
  if Random(30) = 0 then
    UserItem.btValue[7] := 1;
  UserItem.btValue[8] := 1;
  if n14 >= 3 then
  begin
    if UserItem.btValue[2] >= 3 then
    begin
      UserItem.btValue[5] := 1;
      UserItem.btValue[6] := UserItem.btValue[2] * 3 + 25;
      Exit;
    end;
    if UserItem.btValue[3] >= 3 then
    begin
      UserItem.btValue[5] := 2;
      UserItem.btValue[6] := UserItem.btValue[3] * 2 + 18;
      Exit;
    end;
    if UserItem.btValue[4] >= 3 then
    begin
      UserItem.btValue[5] := 3;
      UserItem.btValue[6] := UserItem.btValue[4] * 2 + 18;
      Exit;
    end;
    UserItem.btValue[6] := n14 * 2 + 18;
  end;
end;

procedure TItemUnit.UnknowNecklace(UserItem: pTUserItem); // 神秘腰带
var
  nC, n10, n14: Integer;
begin
  n10 := GetRandomRange(g_Config.nUnknowNecklaceACAddValueMaxLimit, g_Config.nUnknowNecklaceACAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[0] := n10;
    if UserItem.btValue[0] > g_Config.nUnknowNecklaceACAddValueMaxLimit then
      UserItem.btValue[0] := g_Config.nUnknowNecklaceACAddValueMaxLimit; // 20080724 限制上线
  end;
  n14 := n10;
  n10 := GetRandomRange(g_Config.nUnknowNecklaceMACAddValueMaxLimit, g_Config.nUnknowNecklaceMACAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[1] := n10;
    if UserItem.btValue[1] > g_Config.nUnknowNecklaceMACAddValueMaxLimit then
      UserItem.btValue[1] := g_Config.nUnknowNecklaceMACAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(g_Config.nUnknowNecklaceDCAddValueMaxLimit, g_Config.nUnknowNecklaceDCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[2] := n10;
    if UserItem.btValue[2] > g_Config.nUnknowNecklaceDCAddValueMaxLimit then
      UserItem.btValue[2] := g_Config.nUnknowNecklaceDCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(g_Config.nUnknowNecklaceMCAddValueMaxLimit, g_Config.nUnknowNecklaceMCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[3] := n10;
    if UserItem.btValue[3] > g_Config.nUnknowNecklaceMCAddValueMaxLimit then
      UserItem.btValue[3] := g_Config.nUnknowNecklaceMCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(g_Config.nUnknowNecklaceSCAddValueMaxLimit, g_Config.nUnknowNecklaceSCAddRate);
  if n10 > 0 then
  begin
    UserItem.btValue[4] := n10;
    if UserItem.btValue[4] > g_Config.nUnknowNecklaceSCAddValueMaxLimit then
      UserItem.btValue[4] := g_Config.nUnknowNecklaceSCAddValueMaxLimit; // 20080724 限制上线
  end;
  Inc(n14, n10);
  n10 := GetRandomRange(6, 30);
  if n10 > 0 then
  begin
    nC := (n10 + 1) * 1000;
    UserItem.DuraMax := _MIN(65000, UserItem.DuraMax + nC);
    UserItem.Dura := _MIN(65000, UserItem.Dura + nC);
  end;
  if Random(30) = 0 then
    UserItem.btValue[7] := 1;
  UserItem.btValue[8] := 1;
  if n14 >= 2 then
  begin
    if UserItem.btValue[0] >= 3 then
    begin
      UserItem.btValue[5] := 1;
      UserItem.btValue[6] := UserItem.btValue[0] * 3 + 25;
      Exit;
    end;
    if UserItem.btValue[2] >= 2 then
    begin
      UserItem.btValue[5] := 1;
      UserItem.btValue[6] := UserItem.btValue[2] * 3 + 30;
      Exit;
    end;
    if UserItem.btValue[3] >= 2 then
    begin
      UserItem.btValue[5] := 2;
      UserItem.btValue[6] := UserItem.btValue[3] * 2 + 20;
      Exit;
    end;
    if UserItem.btValue[4] >= 2 then
    begin
      UserItem.btValue[5] := 3;
      UserItem.btValue[6] := UserItem.btValue[4] * 2 + 20;
      Exit;
    end;
    UserItem.btValue[6] := n14 * 2 + 18;
  end;
end;

// 物品附加属性-极品属性

procedure TItemUnit.GetItemAddValue(UserItem: pTUserItem; var StdItem: TStdItem);
var
  Mac2: Integer;
  V: LongWord;
  MaxValue: LongWord;
begin
  MaxValue := High(Integer);
  case StdItem.StdMode of
    5, 6, 68, 69: // 时装武器[68,69]
      begin
        V := LongWord(StdItem.DC1) + UserItem.btValue[9];
        StdItem.DC1 := Min(V, MaxValue);

        V := LongWord(StdItem.DC2) + UserItem.btValue[0];
        StdItem.DC2 := Min(V, MaxValue);

        V := LongWord(StdItem.MC1) + UserItem.btValue[11];
        StdItem.MC1 := Min(V, MaxValue);

        V := LongWord(StdItem.MC2) + UserItem.btValue[1];
        StdItem.MC2 := Min(V, MaxValue);

        V := LongWord(StdItem.SC1) + UserItem.btValue[12];
        StdItem.SC1 := Min(V, MaxValue);

        V := LongWord(StdItem.SC2) + UserItem.btValue[2];
        StdItem.SC2 := Min(V, MaxValue);

        // 诅咒
        V := LongWord(StdItem.AC1) + UserItem.btValue[3];
        StdItem.AC1 := Min(V, MaxValue);

        V := LongWord(StdItem.AC2) + UserItem.btValue[5];
        StdItem.AC2 := Min(V, MaxValue);

        { TODO -ochongchong -c修改 : 极品属性加攻击速度错误 【2013-08-28】 }
        // StdItem.MAC := MakeLong(LoWord(StdItem.MAC) + UserItem.btValue[4], HiWord(StdItem.MAC) + UserItem.btValue[6]);
        Mac2 := StdItem.Mac2;
        if UserItem.btValue[6] > 0 then
        begin
          if Mac2 = 0 then
          begin
            V := LongWord(10) + UserItem.btValue[6];
            Mac2 := Min(V, MaxValue)
          end
          else if Mac2 < 10 then
          begin
            Mac2 := Mac2 - UserItem.btValue[6];
            if Mac2 < 0 then
            begin
              V := LongWord(10) + Abs(Mac2);
              Mac2 := Min(V, MaxValue);
            end;
          end
          else // if Mac2 >= 10 then
          begin
            V := LongWord(Mac2) + UserItem.btValue[6];
            Mac2 := Min(V, MaxValue);
          end
        end;

        // 幸运
        V := LongWord(StdItem.MAC1) + UserItem.btValue[4];
        StdItem.MAC1 := Min(V, MaxValue);

        // 修改幸运和诅咒分开显示 2020-05-13 00:09:23
        // 修正幸运和诅咒不会抵消 2019-01-30 17:55:53
        {
          if (StdItem.AC1 <> 0) and (StdItem.MAC1 <> 0) then
          begin
          if StdItem.AC1 >= StdItem.MAC1 then
          begin
          StdItem.AC1 := StdItem.AC1 - StdItem.MAC1;
          StdItem.MAC1 := 0;
          end
          else
          begin
          StdItem.MAC1 := StdItem.MAC1 - StdItem.AC1;
          StdItem.AC1 := 0;
          end;
          end;
        }

        StdItem.Mac2 := Mac2;

        if Byte(UserItem.btValue[7] - 1) < 10 then
        begin // 神圣
          StdItem.Source := UserItem.btValue[7];
        end;
        if UserItem.btValue[10] <> 0 then
          StdItem.Reserved := StdItem.Reserved or 1;
      end;
    10, 11, 66, 67:
      begin
        StdItem.AC1 := StdItem.AC1;
        V := LongWord(StdItem.AC2) + UserItem.btValue[0];
        StdItem.AC2 := Min(V, MaxValue);

        StdItem.MAC1 := StdItem.MAC1;
        V := LongWord(StdItem.Mac2) + UserItem.btValue[1];
        StdItem.Mac2 := Min(V, MaxValue);

        V := LongWord(StdItem.DC1) + UserItem.btValue[9];
        StdItem.DC1 := Min(V, MaxValue);

        V := LongWord(StdItem.DC2) + UserItem.btValue[2];
        StdItem.DC2 := Min(V, MaxValue);

        V := LongWord(StdItem.MC1) + UserItem.btValue[11];
        StdItem.MC1 := Min(V, MaxValue);

        V := LongWord(StdItem.MC2) + UserItem.btValue[3];
        StdItem.MC2 := Min(V, MaxValue);

        V := LongWord(StdItem.SC1) + UserItem.btValue[12];
        StdItem.SC1 := Min(V, MaxValue);

        V := LongWord(StdItem.SC2) + UserItem.btValue[4];
        StdItem.SC2 := Min(V, MaxValue);
      end;
    // 修复斗笠[16]、马牌[28]、军鼓[65]、盾牌[12]、时装衣服[66,67] 极品属性 piaoyun 2013-10-27
    15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 75..90:
      begin
        StdItem.AC1 := StdItem.AC1;
        V := LongWord(StdItem.AC2) + UserItem.btValue[0];
        StdItem.AC2 := Min(V, MaxValue);

        StdItem.MAC1 := StdItem.MAC1;
        V := LongWord(StdItem.Mac2) + UserItem.btValue[1];
        StdItem.Mac2 := Min(V, MaxValue);

        V := LongWord(StdItem.DC1) + UserItem.btValue[9];
        StdItem.DC1 := Min(V, MaxValue);

        V := LongWord(StdItem.DC2) + UserItem.btValue[2];
        StdItem.DC2 := Min(V, MaxValue);

        V := LongWord(StdItem.MC1) + UserItem.btValue[11];
        StdItem.MC1 := Min(V, MaxValue);

        V := LongWord(StdItem.MC2) + UserItem.btValue[3];
        StdItem.MC2 := Min(V, MaxValue);

        V := LongWord(StdItem.SC1) + UserItem.btValue[12];
        StdItem.SC1 := Min(V, MaxValue);

        V := LongWord(StdItem.SC2) + UserItem.btValue[4];
        StdItem.SC2 := Min(V, MaxValue);

        if UserItem.btValue[5] > 0 then
        begin
          StdItem.Need := UserItem.btValue[5];
        end;
        if UserItem.btValue[6] > 0 then
        begin
          StdItem.NeedLevel := UserItem.btValue[6];
        end;
      end;
  end;

  {
    StdItem.btUpgradeCount := UserItem.btUpgradeCount;                                                // 升级次数
    StdItem.btHeroM2Light := UserItem.btHeroM2Light;                                                  // HeroM2 SetItemsLight
    StdItem.IsBind := UserItem.boIsBind;

    Move(UserItem.btValue, StdItem.btValue, SizeOf(UserItem.btValue));
    Move(UserItem.btNewValue, StdItem.NewValue, SizeOf(UserItem.btNewValue));
  }

  if CheckOverLapItem(@StdItem) then // 计算重叠物品重量
    StdItem.Weight := OverLapItemWeight(@StdItem, UserItem.Dura);

  if UserItem.wEffect > 0 then // 读取物品特效
    SetItemEffect(UserItem.wEffect, @StdItem);

  if StdItem.Color = 0 then
    StdItem.Color := 255;

  if UserItem.btColor > 0 then
    StdItem.Color := UserItem.btColor;

  if StdItem.Need in [101, 102, 103] then
  begin // 限时物品
    if UserItem.boStartTime then
      StdItem.NeedLevel := UserItem.nLimitTime;
  end;

  if UserItem.wNewLooks > 0 then
  begin
    StdItem.Looks := UserItem.wNewLooks;
  end;

  if UserItem.wNewShape > 0 then
  begin
    StdItem.Shape := UserItem.wNewShape;
  end;
end;

end.

