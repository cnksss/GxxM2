using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// FunctionConfig.pas TfrmFunctionConfig 巨片拆分（批次J22 第一片，15801 行）：
/// 密码保护页全部处理器（EnablePasswordLock/LockLogin 两级级联 + 13 锁定动作 + 错误次数 + 保存）
/// 与常规页第一组（RefGeneral 九名称颜色 + 饥饿系统 + 装备刻名前缀过滤 + 保存全键）1:1。
/// 其余页（Skill/UpgradeWeapon/Master/MonUpgrade/HeroOption/OffLine/MyShop 等）随后续巨片接入。
/// </summary>
public sealed class FunctionConfigForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;
    private bool boSendServerConfig;

    // ---- 密码保护页 ----
    public System.Windows.Forms.CheckBox CheckBoxEnablePasswordLock = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockGetBackItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockDealItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockDropItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockUseItem = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockLogin = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockWalk = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockRun = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockHit = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockSpell = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockSendMsg = null!;
    public System.Windows.Forms.CheckBox CheckBoxLockInObMode = null!;
    public System.Windows.Forms.CheckBox chkLockChallenge = null!;
    public System.Windows.Forms.CheckBox chkLockSummonHero = null!;
    public System.Windows.Forms.CheckBox chkLockShop = null!;
    public System.Windows.Forms.CheckBox chkLockStall = null!;
    public System.Windows.Forms.NumericUpDown EditErrorPasswordCount = null!;
    public System.Windows.Forms.Button ButtonPasswordLockSave = null!;

    // ---- 常规页 ----
    public System.Windows.Forms.CheckBox CheckBoxHungerSystem = null!;
    public System.Windows.Forms.CheckBox CheckBoxHungerDecHP = null!;
    public System.Windows.Forms.CheckBox CheckBoxHungerDecPower = null!;
    public System.Windows.Forms.NumericUpDown EditPKFlagNameColor = null!;
    public System.Windows.Forms.NumericUpDown EditPKLevel1NameColor = null!;
    public System.Windows.Forms.NumericUpDown EditPKLevel2NameColor = null!;
    public System.Windows.Forms.NumericUpDown EditAllyAndGuildNameColor = null!;
    public System.Windows.Forms.NumericUpDown EditWarGuildNameColor = null!;
    public System.Windows.Forms.NumericUpDown EditInFreePKAreaNameColor = null!;
    public System.Windows.Forms.NumericUpDown EditMerchantNameColor = null!;
    public System.Windows.Forms.NumericUpDown seMerchant273NameColor = null!;
    public System.Windows.Forms.NumericUpDown seGuardNameColor = null!;
    public System.Windows.Forms.CheckBox CheckBoxItemName = null!;
    public System.Windows.Forms.TextBox EditItemName = null!;
    public System.Windows.Forms.Button ButtonGeneralSave = null!;

    // ---- 其余页保存按钮（ModValue/uModValue 联动；处理器随巨片接入） ----
    // （ButtonSkillSave/ButtonUpgradeWeaponSave 已随批次J26、ButtonMasterSave/MakeMineSave/
    //   WinLotterySave 已随批次J27 接入真实控件）
    public System.Windows.Forms.Button btnBonusAbilofSave = null!;
    public System.Windows.Forms.Button btnOther3 = null!;

    /// <summary>GetNameInFilterList 接缝（Delphi M2Share 非法字符过滤；测试注入）。</summary>
    public Func<string, bool>? GetNameInFilterListHandler;

    // ---- Skill 技能页控件（批次J26，头部区段） ----
    public System.Windows.Forms.CheckBox CheckBoxLimitSwordLong = null!;
    public System.Windows.Forms.NumericUpDown EditSwordLongPowerRate = null!;
    public System.Windows.Forms.NumericUpDown seFireBoomRage = null!;
    public System.Windows.Forms.NumericUpDown seSnowWindRange = null!;
    public System.Windows.Forms.NumericUpDown seElecBlizzardRange = null!;
    public System.Windows.Forms.NumericUpDown seMagTurnUndeadLevel = null!;
    public System.Windows.Forms.NumericUpDown EditMagTammingLevel = null!;
    public System.Windows.Forms.NumericUpDown EditMagicAttackRage = null!;
    public System.Windows.Forms.NumericUpDown EditAmyOunsulPoint = null!;
    public System.Windows.Forms.CheckBox CheckBoxFireCrossInSafeZone = null!;
    public System.Windows.Forms.CheckBox chkSkill41MbAttackPlayObject = null!;
    public System.Windows.Forms.NumericUpDown EditBoneFammCount = null!;
    public System.Windows.Forms.TextBox EditBoneFammName = null!;
    public System.Windows.Forms.NumericUpDown EditDogzCount = null!;
    public System.Windows.Forms.TextBox EditDogzName = null!;
    public System.Windows.Forms.DataGridView GridBoneFamm = null!;
    public System.Windows.Forms.DataGridView GridDogz = null!;
    public System.Windows.Forms.Button ButtonSkillSave = null!;

    // ---- UpgradeWeapon 升级武器页控件（批次J26） ----
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponDCRate = null!;
    public System.Windows.Forms.TextBox EditUpgradeWeaponDCRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponDCTwoPointRate = null!;
    public System.Windows.Forms.TextBox EditUpgradeWeaponDCTwoPointRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponDCThreePointRate = null!;
    public System.Windows.Forms.TextBox EditUpgradeWeaponDCThreePointRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponMCRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponMCTwoPointRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponMCThreePointRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponSCRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponSCTwoPointRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarUpgradeWeaponSCThreePointRate = null!;
    public System.Windows.Forms.NumericUpDown EditUpgradeWeaponMaxPoint = null!;
    public System.Windows.Forms.NumericUpDown EditUpgradeWeaponPrice = null!;
    public System.Windows.Forms.NumericUpDown EditUPgradeWeaponGetBackTime = null!;
    public System.Windows.Forms.NumericUpDown EditClearExpireUpgradeWeaponDays = null!;
    public System.Windows.Forms.CheckBox CheckBoxWeaponUpgradeFailNotDelete = null!;
    public System.Windows.Forms.Button ButtonUpgradeWeaponSave = null!;
    public System.Windows.Forms.Button ButtonUpgradeWeaponDefaulf = null!;

    /// <summary>UserEngine.GetMonRace 接缝（Delphi 怪物名数据库校验；测试注入，UserEngine 批次接入真实）。</summary>
    public Func<string, int>? GetMonRaceHandler;

    // ---- Master 拜师页控件（批次J27） ----
    public System.Windows.Forms.NumericUpDown EditMasterOKLevel = null!;
    public System.Windows.Forms.NumericUpDown EditMasterOKCreditPoint = null!;
    public System.Windows.Forms.NumericUpDown EditMasterOKBonusPoint = null!;
    public System.Windows.Forms.Button ButtonMasterSave = null!;

    // ---- MakeMine 挖矿页控件（批次J27） ----
    public System.Windows.Forms.NumericUpDown EditMakeMineHitRate = null!;
    public System.Windows.Forms.NumericUpDown EditMakeMineRate = null!;
    public System.Windows.Forms.NumericUpDown EditStoneTypeRate = null!;
    public System.Windows.Forms.NumericUpDown EditStoneTypeRateMin = null!;
    public System.Windows.Forms.NumericUpDown EditGoldStoneMin = null!;
    public System.Windows.Forms.NumericUpDown EditGoldStoneMax = null!;
    public System.Windows.Forms.NumericUpDown EditSilverStoneMin = null!;
    public System.Windows.Forms.NumericUpDown EditSilverStoneMax = null!;
    public System.Windows.Forms.NumericUpDown EditSteelStoneMin = null!;
    public System.Windows.Forms.NumericUpDown EditSteelStoneMax = null!;
    public System.Windows.Forms.NumericUpDown EditBlackStoneMin = null!;
    public System.Windows.Forms.NumericUpDown EditBlackStoneMax = null!;
    public System.Windows.Forms.NumericUpDown EditStoneMinDura = null!;
    public System.Windows.Forms.NumericUpDown EditStoneGeneralDuraRate = null!;
    public System.Windows.Forms.NumericUpDown EditStoneAddDuraRate = null!;
    public System.Windows.Forms.NumericUpDown EditStoneAddDuraMax = null!;
    public System.Windows.Forms.Button ButtonMakeMineSave = null!;

    // ---- WinLottery 赌博页控件（批次J27） ----
    public System.Windows.Forms.NumericUpDown EditWinLottery1Gold = null!;
    public System.Windows.Forms.NumericUpDown EditWinLottery2Gold = null!;
    public System.Windows.Forms.NumericUpDown EditWinLottery3Gold = null!;
    public System.Windows.Forms.NumericUpDown EditWinLottery4Gold = null!;
    public System.Windows.Forms.NumericUpDown EditWinLottery5Gold = null!;
    public System.Windows.Forms.NumericUpDown EditWinLottery6Gold = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLotteryRate = null!;
    public System.Windows.Forms.TextBox EditWinLotteryRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery1Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery1Max = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery2Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery2Max = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery3Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery3Max = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery4Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery4Max = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery5Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery5Max = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWinLottery6Max = null!;
    public System.Windows.Forms.TextBox EditWinLottery6Max = null!;
    public System.Windows.Forms.Button ButtonWinLotterySave = null!;
    public System.Windows.Forms.Button ButtonWinLotteryDefaulf = null!;

    // ---- ReNewLevel 转生页控件（批次J28） ----
    public System.Windows.Forms.NumericUpDown[] EditReNewNameColorCells = null!;
    public System.Windows.Forms.NumericUpDown EditReNewNameColorTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxReNewChangeColor = null!;
    public System.Windows.Forms.CheckBox CheckBoxReNewLevelClearExp = null!;
    public System.Windows.Forms.Button ButtonReNewLevelSave = null!;
    public System.Windows.Forms.Button ButtonMonUpgradeSave = null!;
    public System.Windows.Forms.Button ButtonOffLineSave = null!;
    public System.Windows.Forms.Button ButtonMyShopSave = null!;
    public System.Windows.Forms.Button ButtonHeroOptionSave = null!;
    public System.Windows.Forms.Button ButtonOther = null!;

    // ---- HeroOption 英雄选项页控件（批次J31） ----
    public System.Windows.Forms.CheckBox CheckBoxHeroGetAllExp = null!;
    public System.Windows.Forms.CheckBox CheckBoxHumanGetAllExp = null!;
    public System.Windows.Forms.NumericUpDown EditHeroKillMonExpRate = null!;
    public System.Windows.Forms.CheckBox CheckBoxAllowCopySelf = null!;
    public System.Windows.Forms.NumericUpDown EditLimitExpLevel = null!;
    public System.Windows.Forms.NumericUpDown EditRecallHeroTime = null!;
    public System.Windows.Forms.NumericUpDown EditHeroWarrorAttackTime = null!;
    public System.Windows.Forms.DataGridView GridHeroExp = null!;
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.NumericUpDown> HeroOptionSpins = new();

    // ---- Other 其他页控件（批次J31） ----
    public System.Windows.Forms.CheckBox CheckBoxDeleteItemDuraZero = null!;
    public System.Windows.Forms.TextBox EditMysteriousManName = null!;
    public System.Windows.Forms.NumericUpDown EditMaxLuckMaxPower = null!;
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.Control> OtherControls = new();

    // ---- BonusAbilof 属性点页控件（批次J31） ----
    public System.Windows.Forms.NumericUpDown EditBonusAbilofWarrDC = null!;
    public System.Windows.Forms.NumericUpDown EditBonusAbilofTaosSpeed = null!;
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.NumericUpDown> BonusAbilofSpins = new();
    public System.Windows.Forms.Button ButtonBonusAbilofSave = null!;

    // ---- Other3 页控件（批次J31） ----
    public System.Windows.Forms.NumericUpDown EditRevivalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxRevivalTouch = null!;
    public System.Windows.Forms.NumericUpDown EditStarBaseNum = null!;
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.Control> Other3Controls = new();
    public System.Windows.Forms.CheckBox CheckBoxSaveRevivalTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxJewelryDecDura = null!;
    public System.Windows.Forms.CheckBox CheckBoxCloseNPCNoItemMsg = null!;
    public System.Windows.Forms.CheckBox CheckBoxDisableMoveParalysisHuman = null!;
    public System.Windows.Forms.NumericUpDown EditStarLineMaxCount = null!;
    public System.Windows.Forms.Button ButtonOther3 = null!;

    public System.Windows.Forms.Button ButtonSpiritMutinySave = null!;
    public System.Windows.Forms.Button ButtonMonSayMsgSave = null!;
    public System.Windows.Forms.Button ButtonWeaponMakeLuckSave = null!;

    // ---- SpiritMutiny/MonSayMsg/WeaponMakeLuck 页控件（批次J29） ----
    public System.Windows.Forms.CheckBox CheckBoxSpiritMutiny = null!;
    public System.Windows.Forms.NumericUpDown EditSpiritMutinyTime = null!;
    public System.Windows.Forms.NumericUpDown EditSpiritPowerRate = null!;
    public System.Windows.Forms.CheckBox CheckBoxMonSayMsg = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeUnLuckRate = null!;
    public System.Windows.Forms.TextBox EditWeaponMakeUnLuckRate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeLuckPoint1 = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeLuckPoint2 = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeLuckPoint3 = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeLuckPoint2Rate = null!;
    public System.Windows.Forms.NumericUpDown ScrollBarWeaponMakeLuckPoint3Rate = null!;
    public System.Windows.Forms.Button ButtonWeaponMakeLuckDefaulf = null!;

    // ---- OffLine 离线页控件（批次J30） ----
    public System.Windows.Forms.CheckBox CheckBoxOffLineLoginSafeArea = null!;
    public System.Windows.Forms.NumericUpDown EditOffLineLoginMapName = null!;
    public System.Windows.Forms.TextBox EditSetOffLineLoginMapName = null!;
    public System.Windows.Forms.CheckBox CheckBoxMonNoAttackOffLinePlayer = null!;

    // ---- MyShop 个人商铺页控件（批次J30） ----
    public System.Windows.Forms.NumericUpDown EditMaxMyShopSellingItemCount = null!;
    public System.Windows.Forms.NumericUpDown EditMaxMyShopStorageItemCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxOfflineCloseMyShop = null!;
    public System.Windows.Forms.CheckBox CheckBoxProhibitModifyPrices = null!;
    public System.Windows.Forms.CheckBox CheckBoxUseHeroM2Shop = null!;
    public System.Windows.Forms.CheckBox CheckBoxInfinityStorage = null!;
    public System.Windows.Forms.NumericUpDown EditInfinityStorageCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxMyShopGold = null!;
    public System.Windows.Forms.CheckBox CheckBoxMyShopGameGold = null!;
    public System.Windows.Forms.CheckBox CheckBoxMyShopGameDiamond = null!;
    public System.Windows.Forms.CheckBox CheckBoxMyShopGameGird = null!;
    public System.Windows.Forms.CheckBox CheckBoxMyShopGamePoint = null!;
    public System.Windows.Forms.CheckBox CheckBoxOpenSelfShop = null!;
    public System.Windows.Forms.CheckBox CheckBoxSafeZoneShop = null!;
    public System.Windows.Forms.CheckBox CheckBoxMapShop = null!;
    public System.Windows.Forms.CheckBox CheckBoxShopStallCanNotAttack = null!;
    public System.Windows.Forms.NumericUpDown EditSellOffGoldTaxRate = null!;
    public System.Windows.Forms.NumericUpDown EditSellOffGameGoldTaxRate = null!;
    public System.Windows.Forms.NumericUpDown EditSellOffGameDiamondTaxRate = null!;
    public System.Windows.Forms.NumericUpDown EditSellOffGameGirdTaxRate = null!;
    public System.Windows.Forms.NumericUpDown EditSellOffGamePointTaxRate = null!;
    public System.Windows.Forms.CheckBox CheckBoxShopHeadPic = null!;

    // ---- MonUpgrade 宝宝升级页控件（批次J28） ----
    public System.Windows.Forms.NumericUpDown[] EditMonUpgradeColorCells = null!;
    public System.Windows.Forms.NumericUpDown[] EditMonUpgradeKillCountCells = null!;
    public System.Windows.Forms.NumericUpDown EditMonUpLvNeedKillBase = null!;
    public System.Windows.Forms.NumericUpDown EditMonUpLvRate = null!;
    public System.Windows.Forms.CheckBox CheckBoxMasterDieMutiny = null!;
    public System.Windows.Forms.NumericUpDown EditMasterDieMutinyRate = null!;
    public System.Windows.Forms.NumericUpDown EditMasterDieMutinyPower = null!;
    public System.Windows.Forms.NumericUpDown EditMasterDieMutinySpeed = null!;
    public System.Windows.Forms.CheckBox CheckBoxBBMonAutoChangeColor = null!;
    public System.Windows.Forms.NumericUpDown EditBBMonAutoChangeColorTime = null!;

    public FunctionConfigForm()
    {
        InitializeComponent();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, int left, System.Windows.Forms.Control parent, int max = 255)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = caption, Left = left, Top = top + 4, AutoSize = true });
        var edit = new System.Windows.Forms.NumericUpDown { Left = left + 150, Top = top, Width = 80, Maximum = max };
        parent.Controls.Add(edit);
        return edit;
    }

    private System.Windows.Forms.CheckBox MkChk(string caption, int left, int top, System.Windows.Forms.Control parent, Action<object?> handler)
    {
        var chk = new System.Windows.Forms.CheckBox { Text = caption, Left = left, Top = top, AutoSize = true };
        chk.Click += (s, e) => handler(s);
        parent.Controls.Add(chk);
        return chk;
    }

    /// <summary>滚动条伴随只读编辑框（Delphi ScrollBar OnChange → Edit.Text 镜像）。</summary>
    private System.Windows.Forms.TextBox AddMirror(System.Windows.Forms.Control parent, System.Windows.Forms.NumericUpDown scroll)
    {
        var edit = new System.Windows.Forms.TextBox { Left = scroll.Left + scroll.Width + 6, Top = scroll.Top, Width = 40, ReadOnly = true };
        parent.Controls.Add(edit);
        return edit;
    }

    /// <summary>召唤宝宝升级表网格（Delphi StringGrid 4 列 ×10 行）。</summary>
    private System.Windows.Forms.DataGridView MakeRecallGrid(System.Windows.Forms.Control parent, string title, int left, int top)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = title, Left = left, Top = top - 20, AutoSize = true });
        var grid = new System.Windows.Forms.DataGridView
        {
            Left = left,
            Top = top,
            Width = 320,
            Height = 180,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
        };
        grid.Columns.Add("HumLevel", "等级");
        grid.Columns.Add("MonName", "名字");
        grid.Columns.Add("Count", "数量");
        grid.Columns.Add("Level", "等保");
        grid.Columns[0].Width = 60;
        grid.Columns[1].Width = 140;
        grid.Rows.Add(10);
        parent.Controls.Add(grid);
        return grid;
    }

    private void InitializeComponent()
    {
        Text = "功能设置";
        Width = 700;
        Height = 540;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var tabs = new System.Windows.Forms.TabControl { Left = 8, Top = 8, Width = 670, Height = 450 };
        Controls.Add(tabs);
        var generalTab = new System.Windows.Forms.TabPage { Text = "常规" };
        tabs.TabPages.Add(generalTab);
        var lockTab = new System.Windows.Forms.TabPage { Text = "密码保护" };
        tabs.TabPages.Add(lockTab);
        var skillTab = new System.Windows.Forms.TabPage { Text = "技能" };
        tabs.TabPages.Add(skillTab);
        var upgradeTab = new System.Windows.Forms.TabPage { Text = "升级武器" };
        tabs.TabPages.Add(upgradeTab);
        var masterTab = new System.Windows.Forms.TabPage { Text = "拜师" };
        tabs.TabPages.Add(masterTab);
        var mineTab = new System.Windows.Forms.TabPage { Text = "挖矿" };
        tabs.TabPages.Add(mineTab);
        var lotteryTab = new System.Windows.Forms.TabPage { Text = "赌博" };
        tabs.TabPages.Add(lotteryTab);
        var renewTab = new System.Windows.Forms.TabPage { Text = "转生" };
        tabs.TabPages.Add(renewTab);
        var monUpTab = new System.Windows.Forms.TabPage { Text = "宝宝升级" };
        tabs.TabPages.Add(monUpTab);
        var offlineTab = new System.Windows.Forms.TabPage { Text = "离线" };
        tabs.TabPages.Add(offlineTab);
        var shopTab = new System.Windows.Forms.TabPage { Text = "个人商铺" };
        tabs.TabPages.Add(shopTab);
        var heroOptTab = new System.Windows.Forms.TabPage { Text = "英雄选项" };
        tabs.TabPages.Add(heroOptTab);
        var otherTab = new System.Windows.Forms.TabPage { Text = "其他" };
        tabs.TabPages.Add(otherTab);
        var bonusTab = new System.Windows.Forms.TabPage { Text = "属性点" };
        tabs.TabPages.Add(bonusTab);
        var other3Tab = new System.Windows.Forms.TabPage { Text = "扩展" };
        tabs.TabPages.Add(other3Tab);
        var mutinyTab = new System.Windows.Forms.TabPage { Text = "叛变" };
        tabs.TabPages.Add(mutinyTab);
        var sayTab = new System.Windows.Forms.TabPage { Text = "怪物发言" };
        tabs.TabPages.Add(sayTab);
        var luckTab = new System.Windows.Forms.TabPage { Text = "武器炼制" };
        tabs.TabPages.Add(luckTab);

        // ---- 常规页 ----
        var gbHunger = new System.Windows.Forms.GroupBox { Text = "饥饿系统", Left = 8, Top = 4, Width = 320, Height = 120 };
        generalTab.Controls.Add(gbHunger);
        CheckBoxHungerSystem = new System.Windows.Forms.CheckBox { Text = "启用饥饿系统", Left = 12, Top = 18, AutoSize = true };
        CheckBoxHungerSystem.Click += (s, e) => CheckBoxHungerSystemClick(s);
        gbHunger.Controls.Add(CheckBoxHungerSystem);
        CheckBoxHungerDecHP = new System.Windows.Forms.CheckBox { Text = "饥饿减 HP", Left = 24, Top = 44, AutoSize = true, Enabled = false };
        CheckBoxHungerDecHP.Click += (s, e) => CheckBoxHungerDecHPClick(s);
        gbHunger.Controls.Add(CheckBoxHungerDecHP);
        CheckBoxHungerDecPower = new System.Windows.Forms.CheckBox { Text = "饥饿减体力", Left = 24, Top = 70, AutoSize = true, Enabled = false };
        CheckBoxHungerDecPower.Click += (s, e) => CheckBoxHungerDecPowerClick(s);
        gbHunger.Controls.Add(CheckBoxHungerDecPower);

        var gbColor = new System.Windows.Forms.GroupBox { Text = "名称颜色", Left = 8, Top = 128, Width = 320, Height = 250 };
        generalTab.Controls.Add(gbColor);
        EditPKFlagNameColor = MakeSpin("PK标志颜色:", 16, 10, gbColor);
        EditPKLevel1NameColor = MakeSpin("红名一级颜色:", 42, 10, gbColor);
        EditPKLevel2NameColor = MakeSpin("红名二级颜色:", 68, 10, gbColor);
        EditAllyAndGuildNameColor = MakeSpin("行会联盟颜色:", 94, 10, gbColor);
        EditWarGuildNameColor = MakeSpin("敌对行会颜色:", 120, 10, gbColor);
        EditInFreePKAreaNameColor = MakeSpin("自由PK区颜色:", 146, 10, gbColor);
        EditMerchantNameColor = MakeSpin("商人颜色:", 172, 10, gbColor);
        seMerchant273NameColor = MakeSpin("商人273颜色:", 198, 10, gbColor);
        seGuardNameColor = MakeSpin("守卫颜色:", 224, 10, gbColor);

        var gbItemName = new System.Windows.Forms.GroupBox { Text = "装备刻名", Left = 336, Top = 4, Width = 320, Height = 90 };
        generalTab.Controls.Add(gbItemName);
        CheckBoxItemName = new System.Windows.Forms.CheckBox { Text = "启用刻名前缀", Left = 12, Top = 18, AutoSize = true };
        gbItemName.Controls.Add(CheckBoxItemName);
        gbItemName.Controls.Add(new System.Windows.Forms.Label { Text = "前缀:", Left = 12, Top = 50, AutoSize = true });
        EditItemName = new System.Windows.Forms.TextBox { Left = 60, Top = 46, Width = 200 };
        gbItemName.Controls.Add(EditItemName);

        ButtonGeneralSave = new System.Windows.Forms.Button { Text = "保存常规(&G)", Left = 336, Top = 104, Width = 110, Height = 26, Enabled = false };
        ButtonGeneralSave.Click += (s, e) => ButtonGeneralSaveClick(s);
        generalTab.Controls.Add(ButtonGeneralSave);

        // ---- 密码保护页 ----
        var gbLock = new System.Windows.Forms.GroupBox { Text = "密码保护系统", Left = 8, Top = 4, Width = 320, Height = 380 };
        lockTab.Controls.Add(gbLock);
        CheckBoxEnablePasswordLock = new System.Windows.Forms.CheckBox { Text = "启用密码保护", Left = 12, Top = 18, AutoSize = true };
        CheckBoxEnablePasswordLock.Click += (s, e) => CheckBoxEnablePasswordLockClick(s);
        gbLock.Controls.Add(CheckBoxEnablePasswordLock);
        CheckBoxLockGetBackItem = new System.Windows.Forms.CheckBox { Text = "锁定取仓库", Left = 24, Top = 44, AutoSize = true, Enabled = false };
        CheckBoxLockGetBackItem.Click += (s, e) => CheckBoxLockGetBackItemClick(s);
        gbLock.Controls.Add(CheckBoxLockGetBackItem);
        CheckBoxLockLogin = new System.Windows.Forms.CheckBox { Text = "登录即锁定", Left = 24, Top = 70, AutoSize = true, Enabled = false };
        CheckBoxLockLogin.Click += (s, e) => CheckBoxLockLoginClick(s);
        gbLock.Controls.Add(CheckBoxLockLogin);
        CheckBoxLockWalk = new System.Windows.Forms.CheckBox { Text = "锁定走", Left = 36, Top = 96, AutoSize = true, Enabled = false };
        CheckBoxLockWalk.Click += (s, e) => CheckBoxLockWalkClick(s);
        gbLock.Controls.Add(CheckBoxLockWalk);
        CheckBoxLockRun = new System.Windows.Forms.CheckBox { Text = "锁定跑", Left = 36, Top = 122, AutoSize = true, Enabled = false };
        CheckBoxLockRun.Click += (s, e) => CheckBoxLockRunClick(s);
        gbLock.Controls.Add(CheckBoxLockRun);
        CheckBoxLockHit = new System.Windows.Forms.CheckBox { Text = "锁定攻击", Left = 36, Top = 148, AutoSize = true, Enabled = false };
        CheckBoxLockHit.Click += (s, e) => CheckBoxLockHitClick(s);
        gbLock.Controls.Add(CheckBoxLockHit);
        CheckBoxLockSpell = new System.Windows.Forms.CheckBox { Text = "锁定魔法", Left = 36, Top = 174, AutoSize = true, Enabled = false };
        CheckBoxLockSpell.Click += (s, e) => CheckBoxLockSpellClick(s);
        gbLock.Controls.Add(CheckBoxLockSpell);
        CheckBoxLockSendMsg = new System.Windows.Forms.CheckBox { Text = "锁定发信息", Left = 36, Top = 200, AutoSize = true, Enabled = false };
        CheckBoxLockSendMsg.Click += (s, e) => CheckBoxLockSendMsgClick(s);
        gbLock.Controls.Add(CheckBoxLockSendMsg);
        CheckBoxLockInObMode = new System.Windows.Forms.CheckBox { Text = "锁定隐身", Left = 36, Top = 226, AutoSize = true, Enabled = false };
        CheckBoxLockInObMode.Click += (s, e) => CheckBoxLockInObModeClick(s);
        gbLock.Controls.Add(CheckBoxLockInObMode);
        CheckBoxLockDealItem = new System.Windows.Forms.CheckBox { Text = "锁定交易", Left = 36, Top = 252, AutoSize = true, Enabled = false };
        CheckBoxLockDealItem.Click += (s, e) => CheckBoxLockDealItemClick(s);
        gbLock.Controls.Add(CheckBoxLockDealItem);
        CheckBoxLockDropItem = new System.Windows.Forms.CheckBox { Text = "锁定扔物", Left = 36, Top = 278, AutoSize = true, Enabled = false };
        CheckBoxLockDropItem.Click += (s, e) => CheckBoxLockDropItemClick(s);
        gbLock.Controls.Add(CheckBoxLockDropItem);
        CheckBoxLockUseItem = new System.Windows.Forms.CheckBox { Text = "锁定用物", Left = 36, Top = 304, AutoSize = true, Enabled = false };
        CheckBoxLockUseItem.Click += (s, e) => CheckBoxLockUseItemClick(s);
        gbLock.Controls.Add(CheckBoxLockUseItem);

        var gbLock2 = new System.Windows.Forms.GroupBox { Text = "锁定扩展", Left = 336, Top = 4, Width = 320, Height = 160 };
        lockTab.Controls.Add(gbLock2);
        chkLockChallenge = new System.Windows.Forms.CheckBox { Text = "禁止挑战", Left = 12, Top = 18, AutoSize = true, Enabled = false };
        chkLockChallenge.Click += (s, e) => chkLockChallengeClick(s);
        gbLock2.Controls.Add(chkLockChallenge);
        chkLockSummonHero = new System.Windows.Forms.CheckBox { Text = "禁止召唤英雄", Left = 12, Top = 44, AutoSize = true, Enabled = false };
        chkLockSummonHero.Click += (s, e) => chkLockSummonHeroClick(s);
        gbLock2.Controls.Add(chkLockSummonHero);
        chkLockShop = new System.Windows.Forms.CheckBox { Text = "禁止商铺", Left = 12, Top = 70, AutoSize = true, Enabled = false };
        chkLockShop.Click += (s, e) => chkLockShopClick(s);
        gbLock2.Controls.Add(chkLockShop);
        chkLockStall = new System.Windows.Forms.CheckBox { Text = "禁止摆摊", Left = 12, Top = 96, AutoSize = true, Enabled = false };
        chkLockStall.Click += (s, e) => chkLockStallClick(s);
        gbLock2.Controls.Add(chkLockStall);

        var gbErr = new System.Windows.Forms.GroupBox { Text = "密码错误", Left = 336, Top = 168, Width = 320, Height = 70 };
        lockTab.Controls.Add(gbErr);
        EditErrorPasswordCount = MakeSpin("错误次数上限:", 16, 10, gbErr, 1000);

        ButtonPasswordLockSave = new System.Windows.Forms.Button { Text = "保存密码保护(&P)", Left = 336, Top = 244, Width = 130, Height = 26, Enabled = false };
        ButtonPasswordLockSave.Click += (s, e) => ButtonPasswordLockSaveClick(s);
        lockTab.Controls.Add(ButtonPasswordLockSave);

        // ---- 技能页（批次J26 头部区段） ----
        var skillGb1 = new System.Windows.Forms.GroupBox { Text = "攻击技能", Left = 8, Top = 4, Width = 320, Height = 190 };
        skillTab.Controls.Add(skillGb1);
        CheckBoxLimitSwordLong = MkChk("限制刺杀剑术", 12, 18, skillGb1, CheckBoxLimitSwordLongClick);
        EditSwordLongPowerRate = MakeSpin("刺杀威力倍率:", 44, 10, skillGb1, 65535);
        EditSwordLongPowerRate.ValueChanged += (s, e) => EditSwordLongPowerRateChange(s);
        seFireBoomRage = MakeSpin("爆裂火焰范围:", 70, 10, skillGb1, 65535);
        seFireBoomRage.ValueChanged += (s, e) => seFireBoomRageChange(s);
        seSnowWindRange = MakeSpin("冰咆哮范围:", 96, 10, skillGb1, 65535);
        seSnowWindRange.ValueChanged += (s, e) => seSnowWindRangeChange(s);
        seElecBlizzardRange = MakeSpin("地狱雷光范围:", 122, 10, skillGb1, 65535);
        seElecBlizzardRange.ValueChanged += (s, e) => seElecBlizzardRangeChange(s);
        EditMagicAttackRage = MakeSpin("魔法攻击范围:", 148, 10, skillGb1, 65535);
        EditMagicAttackRage.ValueChanged += (s, e) => EditMagicAttackRageChange(s);
        chkSkill41MbAttackPlayObject = MkChk("噬血术可攻击玩家", 150, 148, skillGb1, chkSkill41MbAttackPlayObjectClick);

        var skillGb2 = new System.Windows.Forms.GroupBox { Text = "施毒/圣言/诱惑", Left = 336, Top = 4, Width = 320, Height = 190 };
        skillTab.Controls.Add(skillGb2);
        EditAmyOunsulPoint = MakeSpin("施毒术点数:", 16, 10, skillGb2, 65535);
        EditAmyOunsulPoint.ValueChanged += (s, e) => EdiAmyOunsulPointChange(s);
        CheckBoxFireCrossInSafeZone = MkChk("安全区禁火咒", 150, 20, skillGb2, CheckBoxFireCrossInSafeZoneClick);
        seMagTurnUndeadLevel = MakeSpin("圣言怪等级:", 42, 10, skillGb2, 65535);
        seMagTurnUndeadLevel.ValueChanged += (s, e) => seMagTurnUndeadLevelChange(s);
        EditMagTammingLevel = MakeSpin("诱惑怪等级:", 68, 10, skillGb2, 65535);
        EditMagTammingLevel.ValueChanged += (s, e) => EditMagTammingLevelChange(s);

        var skillGb3 = new System.Windows.Forms.GroupBox { Text = "召唤宝宝", Left = 8, Top = 198, Width = 320, Height = 210 };
        skillTab.Controls.Add(skillGb3);
        skillGb3.Controls.Add(new System.Windows.Forms.Label { Text = "骷髅名:", Left = 10, Top = 20, AutoSize = true });
        EditBoneFammName = new System.Windows.Forms.TextBox { Left = 100, Top = 16, Width = 120 };
        EditBoneFammName.TextChanged += (s, e) => EditBoneFammNameChange(s);
        skillGb3.Controls.Add(EditBoneFammName);
        EditBoneFammCount = MakeSpin("骷髅数量:", 42, 10, skillGb3, 100);
        EditBoneFammCount.ValueChanged += (s, e) => EditBoneFammCountChange(s);
        skillGb3.Controls.Add(new System.Windows.Forms.Label { Text = "神兽名:", Left = 10, Top = 76, AutoSize = true });
        EditDogzName = new System.Windows.Forms.TextBox { Left = 100, Top = 72, Width = 120 };
        EditDogzName.TextChanged += (s, e) => EditDogzNameChange(s);
        skillGb3.Controls.Add(EditDogzName);
        EditDogzCount = MakeSpin("神兽数量:", 98, 10, skillGb3, 100);
        EditDogzCount.ValueChanged += (s, e) => EditDogzCountChange(s);

        GridBoneFamm = MakeRecallGrid(skillTab, "骷髅升级表（等级/名字/数量/等保）", 8, 412);
        GridBoneFamm.CellEndEdit += (s, e) => GridBoneFammSetEditText(s, e.RowIndex);
        GridDogz = MakeRecallGrid(skillTab, "神兽升级表（等级/名字/数量/等保）", 336, 412);
        GridDogz.CellEndEdit += (s, e) => GridDogzSetEditText(s, e.RowIndex);

        ButtonSkillSave = new System.Windows.Forms.Button { Text = "保存技能(&K)", Left = 8, Top = 660, Width = 110, Height = 26, Enabled = false };
        ButtonSkillSave.Click += (s, e) => ButtonSkillSaveClick(s);
        skillTab.Controls.Add(ButtonSkillSave);

        // ---- 升级武器页（批次J26） ----
        var upGb1 = new System.Windows.Forms.GroupBox { Text = "升级几率（1..1000）", Left = 8, Top = 4, Width = 648, Height = 170 };
        upgradeTab.Controls.Add(upGb1);
        ScrollBarUpgradeWeaponDCRate = MakeSpin("武器攻DC率:", 16, 10, upGb1, 1000);
        ScrollBarUpgradeWeaponDCRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponDCRateChange(s);
        EditUpgradeWeaponDCRate = AddMirror(upGb1, ScrollBarUpgradeWeaponDCRate);
        ScrollBarUpgradeWeaponDCTwoPointRate = MakeSpin("攻二点率:", 42, 10, upGb1, 1000);
        ScrollBarUpgradeWeaponDCTwoPointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponDCTwoPointRateChange(s);
        EditUpgradeWeaponDCTwoPointRate = AddMirror(upGb1, ScrollBarUpgradeWeaponDCTwoPointRate);
        ScrollBarUpgradeWeaponDCThreePointRate = MakeSpin("攻三点率:", 68, 10, upGb1, 1000);
        ScrollBarUpgradeWeaponDCThreePointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponDCThreePointRateChange(s);
        EditUpgradeWeaponDCThreePointRate = AddMirror(upGb1, ScrollBarUpgradeWeaponDCThreePointRate);
        ScrollBarUpgradeWeaponMCRate = MakeSpin("武器魔MC率:", 16, 220, upGb1, 1000);
        ScrollBarUpgradeWeaponMCRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponMCRateChange(s);
        ScrollBarUpgradeWeaponMCTwoPointRate = MakeSpin("魔二点率:", 42, 220, upGb1, 1000);
        ScrollBarUpgradeWeaponMCTwoPointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponMCTwoPointRateChange(s);
        ScrollBarUpgradeWeaponMCThreePointRate = MakeSpin("魔三点率:", 68, 220, upGb1, 1000);
        ScrollBarUpgradeWeaponMCThreePointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponMCThreePointRateChange(s);
        ScrollBarUpgradeWeaponSCRate = MakeSpin("武器道SC率:", 16, 430, upGb1, 1000);
        ScrollBarUpgradeWeaponSCRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponSCRateChange(s);
        ScrollBarUpgradeWeaponSCTwoPointRate = MakeSpin("道二点率:", 42, 430, upGb1, 1000);
        ScrollBarUpgradeWeaponSCTwoPointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponSCTwoPointRateChange(s);
        ScrollBarUpgradeWeaponSCThreePointRate = MakeSpin("道三点率:", 68, 430, upGb1, 1000);
        ScrollBarUpgradeWeaponSCThreePointRate.ValueChanged += (s, e) => ScrollBarUpgradeWeaponSCThreePointRateChange(s);

        var upGb2 = new System.Windows.Forms.GroupBox { Text = "升级参数", Left = 8, Top = 178, Width = 648, Height = 130 };
        upgradeTab.Controls.Add(upGb2);
        EditUpgradeWeaponMaxPoint = MakeSpin("升级总点数:", 16, 10, upGb2, 65535);
        EditUpgradeWeaponMaxPoint.ValueChanged += (s, e) => EditUpgradeWeaponMaxPointChange(s);
        EditUpgradeWeaponPrice = MakeSpin("升级费用:", 42, 10, upGb2, 2000000000);
        EditUpgradeWeaponPrice.ValueChanged += (s, e) => EditUpgradeWeaponPriceChange(s);
        EditUPgradeWeaponGetBackTime = MakeSpin("取回时间(秒):", 68, 10, upGb2, 2000000000);
        EditUPgradeWeaponGetBackTime.ValueChanged += (s, e) => EditUPgradeWeaponGetBackTimeChange(s);
        EditClearExpireUpgradeWeaponDays = MakeSpin("过期清理(天):", 94, 10, upGb2, 65535);
        EditClearExpireUpgradeWeaponDays.ValueChanged += (s, e) => EditClearExpireUpgradeWeaponDaysChange(s);
        CheckBoxWeaponUpgradeFailNotDelete = MkChk("失败不删除武器", 200, 20, upGb2, CheckBoxWeaponUpgradeFailNotDeleteClick);

        ButtonUpgradeWeaponSave = new System.Windows.Forms.Button { Text = "保存升级(&U)", Left = 8, Top = 314, Width = 110, Height = 26, Enabled = false };
        ButtonUpgradeWeaponSave.Click += (s, e) => ButtonUpgradeWeaponSaveClick(s);
        upgradeTab.Controls.Add(ButtonUpgradeWeaponSave);
        ButtonUpgradeWeaponDefaulf = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 128, Top = 314, Width = 90, Height = 26 };
        ButtonUpgradeWeaponDefaulf.Click += (s, e) => ButtonUpgradeWeaponDefaulfClick(s);
        upgradeTab.Controls.Add(ButtonUpgradeWeaponDefaulf);

        // ---- 拜师页（批次J27） ----
        var masterGb = new System.Windows.Forms.GroupBox { Text = "拜师参数", Left = 8, Top = 4, Width = 320, Height = 150 };
        masterTab.Controls.Add(masterGb);
        EditMasterOKLevel = MakeSpin("拜师等级:", 16, 10, masterGb, 65535);
        EditMasterOKLevel.ValueChanged += (s, e) => EditMasterOKLevelChange(s);
        EditMasterOKCreditPoint = MakeSpin("师傅声望:", 42, 10, masterGb, 2000000000);
        EditMasterOKCreditPoint.ValueChanged += (s, e) => EditMasterOKCreditPointChange(s);
        EditMasterOKBonusPoint = MakeSpin("师傅点数:", 68, 10, masterGb, 2000000000);
        EditMasterOKBonusPoint.ValueChanged += (s, e) => EditMasterOKBonusPointChange(s);
        ButtonMasterSave = new System.Windows.Forms.Button { Text = "保存拜师(&M)", Left = 8, Top = 160, Width = 110, Height = 26, Enabled = false };
        ButtonMasterSave.Click += (s, e) => ButtonMasterSaveClick(s);
        masterTab.Controls.Add(ButtonMasterSave);

        // ---- 挖矿页（批次J27） ----
        var mineGb = new System.Windows.Forms.GroupBox { Text = "挖矿/矿石", Left = 8, Top = 4, Width = 648, Height = 260 };
        mineTab.Controls.Add(mineGb);
        EditMakeMineHitRate = MakeSpin("挖矿命中率:", 16, 10, mineGb, 65535);
        EditMakeMineHitRate.ValueChanged += (s, e) => EditMakeMineHitRateChange(s);
        EditMakeMineRate = MakeSpin("挖矿率:", 42, 10, mineGb, 65535);
        EditMakeMineRate.ValueChanged += (s, e) => EditMakeMineRateChange(s);
        EditStoneTypeRate = MakeSpin("矿石类型率:", 68, 10, mineGb, 65535);
        EditStoneTypeRate.ValueChanged += (s, e) => EditStoneTypeRateChange(s);
        EditStoneTypeRateMin = MakeSpin("类型率下限:", 94, 10, mineGb, 65535);
        EditStoneTypeRateMin.ValueChanged += (s, e) => EditStoneTypeRateMinChange(s);
        EditGoldStoneMin = MakeSpin("金矿下限:", 120, 10, mineGb, 65535);
        EditGoldStoneMin.ValueChanged += (s, e) => EditGoldStoneMinChange(s);
        EditGoldStoneMax = MakeSpin("金矿上限:", 146, 10, mineGb, 65535);
        EditGoldStoneMax.ValueChanged += (s, e) => EditGoldStoneMaxChange(s);
        EditSilverStoneMin = MakeSpin("银矿下限:", 172, 10, mineGb, 65535);
        EditSilverStoneMin.ValueChanged += (s, e) => EditSilverStoneMinChange(s);
        EditSilverStoneMax = MakeSpin("银矿上限:", 198, 10, mineGb, 65535);
        EditSilverStoneMax.ValueChanged += (s, e) => EditSilverStoneMaxChange(s);
        EditSteelStoneMin = MakeSpin("铁矿下限:", 16, 330, mineGb, 65535);
        EditSteelStoneMin.ValueChanged += (s, e) => EditSteelStoneMinChange(s);
        EditSteelStoneMax = MakeSpin("铁矿上限:", 42, 330, mineGb, 65535);
        EditSteelStoneMax.ValueChanged += (s, e) => EditSteelStoneMaxChange(s);
        EditBlackStoneMin = MakeSpin("黑矿下限:", 68, 330, mineGb, 65535);
        EditBlackStoneMin.ValueChanged += (s, e) => EditBlackStoneMinChange(s);
        EditBlackStoneMax = MakeSpin("黑矿上限:", 94, 330, mineGb, 65535);
        EditBlackStoneMax.ValueChanged += (s, e) => EditBlackStoneMaxChange(s);
        EditStoneMinDura = MakeSpin("矿石最小持久:", 120, 330, mineGb, 2000000000);
        EditStoneMinDura.ValueChanged += (s, e) => EditStoneMinDuraChange(s);
        EditStoneGeneralDuraRate = MakeSpin("普通持久率:", 146, 330, mineGb, 2000000000);
        EditStoneGeneralDuraRate.ValueChanged += (s, e) => EditStoneGeneralDuraRateChange(s);
        EditStoneAddDuraRate = MakeSpin("持久增加率:", 172, 330, mineGb, 2000000000);
        EditStoneAddDuraRate.ValueChanged += (s, e) => EditStoneAddDuraRateChange(s);
        EditStoneAddDuraMax = MakeSpin("持久增加上限:", 198, 330, mineGb, 2000000000);
        EditStoneAddDuraMax.ValueChanged += (s, e) => EditStoneAddDuraMaxChange(s);

        ButtonMakeMineSave = new System.Windows.Forms.Button { Text = "保存挖矿(&N)", Left = 8, Top = 270, Width = 110, Height = 26, Enabled = false };
        ButtonMakeMineSave.Click += (s, e) => ButtonMakeMineSaveClick(s);
        mineTab.Controls.Add(ButtonMakeMineSave);

        // ---- 赌博页（批次J27） ----
        var lotGb1 = new System.Windows.Forms.GroupBox { Text = "奖金", Left = 8, Top = 4, Width = 320, Height = 210 };
        lotteryTab.Controls.Add(lotGb1);
        EditWinLottery1Gold = MakeSpin("一等奖金:", 16, 10, lotGb1, 2000000000);
        EditWinLottery1Gold.ValueChanged += (s, e) => EditWinLottery1GoldChange(s);
        EditWinLottery2Gold = MakeSpin("二等奖金:", 42, 10, lotGb1, 2000000000);
        EditWinLottery2Gold.ValueChanged += (s, e) => EditWinLottery2GoldChange(s);
        EditWinLottery3Gold = MakeSpin("三等奖金:", 68, 10, lotGb1, 2000000000);
        EditWinLottery3Gold.ValueChanged += (s, e) => EditWinLottery3GoldChange(s);
        EditWinLottery4Gold = MakeSpin("四等奖金:", 94, 10, lotGb1, 2000000000);
        EditWinLottery4Gold.ValueChanged += (s, e) => EditWinLottery4GoldChange(s);
        EditWinLottery5Gold = MakeSpin("五等奖金:", 120, 10, lotGb1, 2000000000);
        EditWinLottery5Gold.ValueChanged += (s, e) => EditWinLottery5GoldChange(s);
        EditWinLottery6Gold = MakeSpin("六等奖金:", 146, 10, lotGb1, 2000000000);
        EditWinLottery6Gold.ValueChanged += (s, e) => EditWinLottery6GoldChange(s);

        var lotGb2 = new System.Windows.Forms.GroupBox { Text = "中奖区间", Left = 336, Top = 4, Width = 320, Height = 260 };
        lotteryTab.Controls.Add(lotGb2);
        lotGb2.Controls.Add(new System.Windows.Forms.Label { Text = "总税率:", Left = 10, Top = 20, AutoSize = true });
        ScrollBarWinLotteryRate = new System.Windows.Forms.NumericUpDown { Left = 110, Top = 16, Width = 80, Minimum = 1, Maximum = 100000 };
        ScrollBarWinLotteryRate.ValueChanged += (s, e) => ScrollBarWinLotteryRateChange(s);
        lotGb2.Controls.Add(ScrollBarWinLotteryRate);
        EditWinLotteryRate = new System.Windows.Forms.TextBox { Left = 200, Top = 16, Width = 70, ReadOnly = true };
        lotGb2.Controls.Add(EditWinLotteryRate);
        ScrollBarWinLottery1Max = MakeSpin("一等奖上界:", 46, 10, lotGb2, 100000);
        ScrollBarWinLottery1Max.Minimum = 1;
        ScrollBarWinLottery1Max.ValueChanged += (s, e) => ScrollBarWinLottery1MaxChange(s);
        EditWinLottery1Max = AddMirror(lotGb2, ScrollBarWinLottery1Max);
        ScrollBarWinLottery2Max = MakeSpin("二等奖上界:", 72, 10, lotGb2, 100000);
        ScrollBarWinLottery2Max.Minimum = 1;
        ScrollBarWinLottery2Max.ValueChanged += (s, e) => ScrollBarWinLottery2MaxChange(s);
        EditWinLottery2Max = AddMirror(lotGb2, ScrollBarWinLottery2Max);
        ScrollBarWinLottery3Max = MakeSpin("三等奖上界:", 98, 10, lotGb2, 100000);
        ScrollBarWinLottery3Max.Minimum = 1;
        ScrollBarWinLottery3Max.ValueChanged += (s, e) => ScrollBarWinLottery3MaxChange(s);
        EditWinLottery3Max = AddMirror(lotGb2, ScrollBarWinLottery3Max);
        ScrollBarWinLottery4Max = MakeSpin("四等奖上界:", 124, 10, lotGb2, 100000);
        ScrollBarWinLottery4Max.Minimum = 1;
        ScrollBarWinLottery4Max.ValueChanged += (s, e) => ScrollBarWinLottery4MaxChange(s);
        EditWinLottery4Max = AddMirror(lotGb2, ScrollBarWinLottery4Max);
        ScrollBarWinLottery5Max = MakeSpin("五等奖上界:", 150, 10, lotGb2, 100000);
        ScrollBarWinLottery5Max.Minimum = 1;
        ScrollBarWinLottery5Max.ValueChanged += (s, e) => ScrollBarWinLottery5MaxChange(s);
        EditWinLottery5Max = AddMirror(lotGb2, ScrollBarWinLottery5Max);
        ScrollBarWinLottery6Max = MakeSpin("六等奖上界:", 176, 10, lotGb2, 100000);
        ScrollBarWinLottery6Max.Minimum = 1;
        ScrollBarWinLottery6Max.ValueChanged += (s, e) => ScrollBarWinLottery6MaxChange(s);
        EditWinLottery6Max = AddMirror(lotGb2, ScrollBarWinLottery6Max);

        ButtonWinLotterySave = new System.Windows.Forms.Button { Text = "保存赌博(&L)", Left = 8, Top = 270, Width = 110, Height = 26, Enabled = false };
        ButtonWinLotterySave.Click += (s, e) => ButtonWinLotterySaveClick(s);
        lotteryTab.Controls.Add(ButtonWinLotterySave);
        ButtonWinLotteryDefaulf = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 128, Top = 270, Width = 90, Height = 26 };
        ButtonWinLotteryDefaulf.Click += (s, e) => ButtonWinLotteryDefaulfClick(s);
        lotteryTab.Controls.Add(ButtonWinLotteryDefaulf);

        // ---- 转生页（批次J28） ----
        var renewGb = new System.Windows.Forms.GroupBox { Text = "转生名字颜色（10 阶）", Left = 8, Top = 4, Width = 648, Height = 100 };
        renewTab.Controls.Add(renewGb);
        EditReNewNameColorCells = new System.Windows.Forms.NumericUpDown[10];
        for (int i = 0; i < 10; i++)
        {
            int idx = i;
            renewGb.Controls.Add(new System.Windows.Forms.Label { Text = (i + 1) + "阶:", Left = 12 + (i % 5) * 128, Top = 24 + (i / 5) * 44, AutoSize = true });
            var spin = new System.Windows.Forms.NumericUpDown { Left = 52 + (i % 5) * 128, Top = 20 + (i / 5) * 44, Width = 70, Maximum = 255 };
            spin.ValueChanged += (s, e) => EditReNewNameColorNChange(idx);
            renewGb.Controls.Add(spin);
            EditReNewNameColorCells[idx] = spin;
        }
        EditReNewNameColorTime = MakeSpin("变色间隔(秒):", 110, 10, renewTab, 2000000000);
        EditReNewNameColorTime.ValueChanged += (s, e) => EditReNewNameColorTimeChange(s);
        CheckBoxReNewChangeColor = MkChk("转生变色", 200, 114, renewTab, CheckBoxReNewChangeColorClick);
        CheckBoxReNewLevelClearExp = MkChk("转生清经验", 200, 140, renewTab, CheckBoxReNewLevelClearExpClick);
        ButtonReNewLevelSave = new System.Windows.Forms.Button { Text = "保存转生(&R)", Left = 8, Top = 170, Width = 110, Height = 26, Enabled = false };
        ButtonReNewLevelSave.Click += (s, e) => ButtonReNewLevelSaveClick(s);
        renewTab.Controls.Add(ButtonReNewLevelSave);

        // ---- 宝宝升级页（批次J28） ----
        var monGb1 = new System.Windows.Forms.GroupBox { Text = "宝宝颜色（0..9 级）", Left = 8, Top = 4, Width = 648, Height = 100 };
        monUpTab.Controls.Add(monGb1);
        EditMonUpgradeColorCells = new System.Windows.Forms.NumericUpDown[10];
        for (int i = 0; i < 10; i++)
        {
            int idx = i;
            monGb1.Controls.Add(new System.Windows.Forms.Label { Text = i + "级:", Left = 12 + (i % 5) * 128, Top = 24 + (i / 5) * 44, AutoSize = true });
            var spin = new System.Windows.Forms.NumericUpDown { Left = 52 + (i % 5) * 128, Top = 20 + (i / 5) * 44, Width = 70, Maximum = 255 };
            spin.ValueChanged += (s, e) => EditMonUpgradeColorNChange(idx);
            monGb1.Controls.Add(spin);
            EditMonUpgradeColorCells[idx] = spin;
        }

        var monGb2 = new System.Windows.Forms.GroupBox { Text = "升级所需杀怪数（KillCount1..7）", Left = 8, Top = 108, Width = 648, Height = 70 };
        monUpTab.Controls.Add(monGb2);
        EditMonUpgradeKillCountCells = new System.Windows.Forms.NumericUpDown[7];
        for (int i = 0; i < 7; i++)
        {
            int idx = i;
            monGb2.Controls.Add(new System.Windows.Forms.Label { Text = (i + 1) + ":", Left = 12 + i * 90, Top = 28, AutoSize = true });
            var spin = new System.Windows.Forms.NumericUpDown { Left = 30 + i * 90, Top = 24, Width = 60, Maximum = 2000000000 };
            spin.ValueChanged += (s, e) => EditMonUpgradeKillCountNChange(idx);
            monGb2.Controls.Add(spin);
            EditMonUpgradeKillCountCells[idx] = spin;
        }

        var monGb3 = new System.Windows.Forms.GroupBox { Text = "升级参数", Left = 8, Top = 182, Width = 648, Height = 100 };
        monUpTab.Controls.Add(monGb3);
        EditMonUpLvNeedKillBase = MakeSpin("基础杀怪数:", 16, 10, monGb3, 2000000000);
        EditMonUpLvNeedKillBase.ValueChanged += (s, e) => EditMonUpLvNeedKillBaseChange(s);
        EditMonUpLvRate = MakeSpin("升级率:", 42, 10, monGb3, 65535);
        EditMonUpLvRate.ValueChanged += (s, e) => EditMonUpLvRateChange(s);
        CheckBoxMasterDieMutiny = MkChk("主人死亡叛变", 200, 20, monGb3, CheckBoxMasterDieMutinyClick);
        EditMasterDieMutinyRate = MakeSpin("叛变率:", 42, 200, monGb3, 65535);
        EditMasterDieMutinyRate.ValueChanged += (s, e) => EditMasterDieMutinyRateChange(s);
        EditMasterDieMutinyPower = MakeSpin("叛变威力:", 68, 200, monGb3, 65535);
        EditMasterDieMutinyPower.ValueChanged += (s, e) => EditMasterDieMutinyPowerChange(s);
        EditMasterDieMutinySpeed = MakeSpin("叛变速度:", 68, 440, monGb3, 65535);
        EditMasterDieMutinySpeed.ValueChanged += (s, e) => EditMasterDieMutinySpeedChange(s);

        var monGb4 = new System.Windows.Forms.GroupBox { Text = "自动变色", Left = 8, Top = 286, Width = 648, Height = 70 };
        monUpTab.Controls.Add(monGb4);
        CheckBoxBBMonAutoChangeColor = MkChk("宝宝自动变色", 12, 20, monGb4, CheckBoxBBMonAutoChangeColorClick);
        EditBBMonAutoChangeColorTime = MakeSpin("变色间隔(秒):", 46, 10, monGb4, 2000000000);
        EditBBMonAutoChangeColorTime.ValueChanged += (s, e) => EditBBMonAutoChangeColorTimeChange(s);

        ButtonMonUpgradeSave = new System.Windows.Forms.Button { Text = "保存宝宝升级(&G)", Left = 8, Top = 362, Width = 130, Height = 26, Enabled = false };
        ButtonMonUpgradeSave.Click += (s, e) => ButtonMonUpgradeSaveClick(s);
        monUpTab.Controls.Add(ButtonMonUpgradeSave);

        // ---- 离线页（批次J30） ----
        var offGb = new System.Windows.Forms.GroupBox { Text = "离线挂机", Left = 8, Top = 4, Width = 400, Height = 170 };
        offlineTab.Controls.Add(offGb);
        CheckBoxOffLineLoginSafeArea = MkChk("离线登录仅安全区", 12, 18, offGb, CheckBoxOffLineLoginSafeAreaClick);
        EditOffLineLoginMapName = MakeSpin("离线登录地图号:", 44, 10, offGb, 255);
        EditOffLineLoginMapName.ValueChanged += (s, e) => EditOffLineLoginMapNameChange(s);
        offGb.Controls.Add(new System.Windows.Forms.Label { Text = "离线登录地图:", Left = 10, Top = 76, AutoSize = true });
        EditSetOffLineLoginMapName = new System.Windows.Forms.TextBox { Left = 160, Top = 72, Width = 80 };
        EditSetOffLineLoginMapName.TextChanged += (s, e) => EditSetOffLineLoginMapNameChange(s);
        offGb.Controls.Add(EditSetOffLineLoginMapName);
        CheckBoxMonNoAttackOffLinePlayer = MkChk("怪物不攻击脱机人物", 12, 102, offGb, CheckBoxMonNoAttackOffLinePlayerClick);
        ButtonOffLineSave = new System.Windows.Forms.Button { Text = "保存离线(&O)", Left = 8, Top = 180, Width = 110, Height = 26, Enabled = false };
        ButtonOffLineSave.Click += (s, e) => ButtonOffLineSaveClick(s);
        offlineTab.Controls.Add(ButtonOffLineSave);

        // ---- 个人商铺页（批次J30） ----
        var shopGb1 = new System.Windows.Forms.GroupBox { Text = "商铺容量/开关", Left = 8, Top = 4, Width = 400, Height = 230 };
        shopTab.Controls.Add(shopGb1);
        EditMaxMyShopSellingItemCount = MakeSpin("在售物品上限:", 16, 10, shopGb1, 65535);
        EditMaxMyShopSellingItemCount.ValueChanged += (s, e) => EditMaxMyShopSellingItemCountChange(s);
        EditMaxMyShopStorageItemCount = MakeSpin("仓库物品上限:", 42, 10, shopGb1, 65535);
        EditMaxMyShopStorageItemCount.ValueChanged += (s, e) => EditMaxMyShopStorageItemCountChange(s);
        CheckBoxOfflineCloseMyShop = MkChk("离线关闭商铺", 260, 20, shopGb1, CheckBoxOfflineCloseMyShopClick);
        CheckBoxProhibitModifyPrices = MkChk("禁止修改价格", 12, 72, shopGb1, CheckBoxProhibitModifyPricesClick);
        CheckBoxUseHeroM2Shop = MkChk("英雄可用商铺", 200, 72, shopGb1, CheckBoxUseHeroM2ShopClick);
        CheckBoxInfinityStorage = MkChk("启用自定义仓库", 12, 98, shopGb1, CheckBoxInfinityStorageClick);
        EditInfinityStorageCount = MakeSpin("仓库容量:", 122, 10, shopGb1, 65535);
        EditInfinityStorageCount.ValueChanged += (s, e) => EditInfinityStorageCountChange(s);
        CheckBoxShopStallCanNotAttack = MkChk("摆摊无敌", 260, 72, shopGb1, CheckBoxShopStallCanNotAttackClick);
        CheckBoxShopHeadPic = MkChk("头顶商店图标", 12, 150, shopGb1, CheckBoxShopHeadPicClick);

        var shopGb2 = new System.Windows.Forms.GroupBox { Text = "货币/摆摊", Left = 8, Top = 238, Width = 648, Height = 160 };
        shopTab.Controls.Add(shopGb2);
        CheckBoxMyShopGold = MkChk("允许金币", 12, 16, shopGb2, CheckBoxMyShopGoldClick);
        CheckBoxMyShopGameGold = MkChk("允许元宝", 130, 16, shopGb2, CheckBoxMyShopGameGoldClick);
        CheckBoxMyShopGameDiamond = MkChk("允许金刚石", 248, 16, shopGb2, CheckBoxMyShopGameDiamondClick);
        CheckBoxMyShopGameGird = MkChk("允许灵符", 12, 42, shopGb2, CheckBoxMyShopGameGirdClick);
        CheckBoxMyShopGamePoint = MkChk("允许游戏点", 130, 42, shopGb2, CheckBoxMyShopGamePointClick);
        CheckBoxOpenSelfShop = MkChk("开启摆摊", 12, 68, shopGb2, CheckBoxOpenSelfShopClick);
        CheckBoxSafeZoneShop = MkChk("仅安全区摆摊", 130, 68, shopGb2, CheckBoxSafeZoneShopClick);
        CheckBoxMapShop = MkChk("仅指定地图摆摊", 248, 68, shopGb2, CheckBoxMapShopClick);
        EditSellOffGoldTaxRate = MakeSpin("金币税:", 94, 10, shopGb2, 65535);
        EditSellOffGoldTaxRate.ValueChanged += (s, e) => EditSellOffGoldTaxRateChange(s);
        EditSellOffGameGoldTaxRate = MakeSpin("元宝税:", 94, 210, shopGb2, 65535);
        EditSellOffGameGoldTaxRate.ValueChanged += (s, e) => EditSellOffGameGoldTaxRateChange(s);
        EditSellOffGameDiamondTaxRate = MakeSpin("金刚石税:", 120, 10, shopGb2, 65535);
        EditSellOffGameDiamondTaxRate.ValueChanged += (s, e) => EditSellOffGameDiamondTaxRateChange(s);
        EditSellOffGameGirdTaxRate = MakeSpin("灵符税:", 120, 210, shopGb2, 65535);
        EditSellOffGameGirdTaxRate.ValueChanged += (s, e) => EditSellOffGameGirdTaxRateChange(s);
        EditSellOffGamePointTaxRate = MakeSpin("游戏点税:", 146, 210, shopGb2, 65535);
        EditSellOffGamePointTaxRate.ValueChanged += (s, e) => EditSellOffGamePointTaxRateChange(s);

        ButtonMyShopSave = new System.Windows.Forms.Button { Text = "保存商铺(&H)", Left = 8, Top = 404, Width = 110, Height = 26, Enabled = false };
        ButtonMyShopSave.Click += (s, e) => ButtonMyShopSaveClick(s);
        shopTab.Controls.Add(ButtonMyShopSave);

        // ---- 英雄选项页（批次J31） ----
        var heroGb = new System.Windows.Forms.GroupBox { Text = "英雄经验/参数", Left = 8, Top = 4, Width = 648, Height = 170 };
        heroOptTab.Controls.Add(heroGb);
        CheckBoxHeroGetAllExp = MkChk("英雄获取全部经验", 12, 16, heroGb, CheckBoxHeroGetAllExpClick);
        CheckBoxHumanGetAllExp = MkChk("人物获取全部经验", 200, 16, heroGb, CheckBoxHumanGetAllExpClick);
        EditHeroKillMonExpRate = MakeSpin("英雄杀怪经验率:", 42, 10, heroGb, 2000000000);
        EditHeroKillMonExpRate.ValueChanged += (s, e) => EditHeroKillMonExpRateChange(s);
        CheckBoxAllowCopySelf = MkChk("允许分身", 200, 46, heroGb, CheckBoxAllowCopySelfClick);
        EditLimitExpLevel = MakeSpin("限制经验等级:", 68, 10, heroGb, 65535);
        EditLimitExpLevel.ValueChanged += (s, e) => EditLimitExpLevelChange(s);
        EditRecallHeroTime = MakeSpin("召唤英雄间隔:", 94, 10, heroGb, 2000000000);
        EditRecallHeroTime.ValueChanged += (s, e) => EditRecallHeroTimeChange(s);
        EditHeroWarrorAttackTime = MakeSpin("英雄攻击间隔:", 120, 10, heroGb, 2000000000);
        EditHeroWarrorAttackTime.ValueChanged += (s, e) => EditHeroWarrorAttackTimeChange(s);

        heroOptTab.Controls.Add(new System.Windows.Forms.Label { Text = "英雄升级经验表（等级/经验值）", Left = 8, Top = 180, AutoSize = true });
        GridHeroExp = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 200,
            Width = 648,
            Height = 380,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
        };
        GridHeroExp.Columns.Add("Level", "等级");
        GridHeroExp.Columns.Add("Exp", "经验值");
        GridHeroExp.Columns[0].Width = 80;
        GridHeroExp.Columns[0].ReadOnly = true;
        GridHeroExp.Columns[1].Width = 500;
        GridHeroExp.Rows.Add(1000);
        for (int i = 1; i <= 1000; i++)
            GridHeroExp.Rows[i - 1].Cells[0].Value = i;
        heroOptTab.Controls.Add(GridHeroExp);

        ButtonHeroOptionSave = new System.Windows.Forms.Button { Text = "保存英雄(&E)", Left = 8, Top = 586, Width = 110, Height = 26, Enabled = false };
        ButtonHeroOptionSave.Click += (s, e) => ButtonHeroOptionSaveClick(s);
        heroOptTab.Controls.Add(ButtonHeroOptionSave);

        // ---- 其他页（批次J31） ----
        var otherGb = new System.Windows.Forms.GroupBox { Text = "其他参数", Left = 8, Top = 4, Width = 648, Height = 260 };
        otherTab.Controls.Add(otherGb);
        CheckBoxDeleteItemDuraZero = MkChk("删除零持久物品", 12, 16, otherGb, CheckBoxDeleteItemDuraZeroClick);
        otherGb.Controls.Add(new System.Windows.Forms.Label { Text = "神秘人名:", Left = 10, Top = 46, AutoSize = true });
        EditMysteriousManName = new System.Windows.Forms.TextBox { Left = 110, Top = 42, Width = 120 };
        EditMysteriousManName.TextChanged += (s, e) => EditMysteriousManNameChange(s);
        otherGb.Controls.Add(EditMysteriousManName);
        EditMaxLuckMaxPower = MakeSpin("幸运最高威力:", 70, 10, otherGb, 65535);
        EditMaxLuckMaxPower.ValueChanged += (s, e) => EditMaxLuckMaxPowerChange(s);
        var ebQueryBagItemsTime = MakeSpin("查询背包间隔:", 96, 10, otherGb, 65535);
        OtherControls["QueryBagItemsTime"] = ebQueryBagItemsTime;
        ebQueryBagItemsTime.ValueChanged += (s, e) => EditQueryBagItemsTimeChange(ebQueryBagItemsTime);
        otherGb.Height = 200;

        ButtonOther = new System.Windows.Forms.Button { Text = "保存其他(&O)", Left = 8, Top = 274, Width = 110, Height = 26, Enabled = false };
        ButtonOther.Click += (s, e) => ButtonOtherClick(s);
        otherTab.Controls.Add(ButtonOther);

        // ---- 属性点页（批次J31） ----
        ButtonBonusAbilofSave = new System.Windows.Forms.Button { Text = "保存属性点(&B)", Left = 8, Top = 340, Width = 120, Height = 26, Enabled = false };
        ButtonBonusAbilofSave.Click += (s, e) => ButtonBonusAbilofSaveClick(s);
        bonusTab.Controls.Add(ButtonBonusAbilofSave);
        BuildBonusAbilofControls(bonusTab);

        // ---- 扩展页（批次J31） ----
        var other3Gb = new System.Windows.Forms.GroupBox { Text = "复活/星岩/首饰盒/行会", Left = 8, Top = 4, Width = 648, Height = 200 };
        other3Tab.Controls.Add(other3Gb);
        EditRevivalTime = MakeSpin("复活间隔(秒):", 16, 10, other3Gb, 2000000000);
        EditRevivalTime.ValueChanged += (s, e) => EditRevivalTimeChange(s);
        CheckBoxRevivalTouch = MkChk("触发复活脚本", 200, 20, other3Gb, CheckBoxRevivalTouchClick);
        CheckBoxSaveRevivalTime = MkChk("保存复活时间", 380, 20, other3Gb, CheckBoxSaveRevivalTimeClick);
        EditStarBaseNum = MakeSpin("星岩基数:", 46, 10, other3Gb, 65535);
        EditStarBaseNum.ValueChanged += (s, e) => EditStarBaseNumChange(s);
        EditStarLineMaxCount = MakeSpin("星岩连线数:", 46, 210, other3Gb, 65535);
        EditStarLineMaxCount.ValueChanged += (s, e) => EditStarLineMaxCountChange(s);
        CheckBoxJewelryDecDura = MkChk("首饰盒掉持久", 12, 76, other3Gb, CheckBoxJewelryDecDuraClick);
        CheckBoxCloseNPCNoItemMsg = MkChk("关闭 NPC 无货提示", 200, 76, other3Gb, CheckBoxCloseNPCNoItemMsgClick);
        CheckBoxDisableMoveParalysisHuman = MkChk("麻痹禁传送", 12, 102, other3Gb, CheckBoxDisableMoveParalysisHumanClick);

        ButtonOther3 = new System.Windows.Forms.Button { Text = "保存扩展(&3)", Left = 8, Top = 210, Width = 110, Height = 26, Enabled = false };
        ButtonOther3.Click += (s, e) => ButtonOther3Click(s);
        other3Tab.Controls.Add(ButtonOther3);

        // ---- 叛变页（批次J29） ----
        var mutinyGb = new System.Windows.Forms.GroupBox { Text = "神器叛变", Left = 8, Top = 4, Width = 320, Height = 150 };
        mutinyTab.Controls.Add(mutinyGb);
        CheckBoxSpiritMutiny = MkChk("启用叛变", 12, 18, mutinyGb, CheckBoxSpiritMutinyClick);
        EditSpiritMutinyTime = MakeSpin("叛变时间(分):", 44, 10, mutinyGb, 65535);
        EditSpiritMutinyTime.ValueChanged += (s, e) => EditSpiritMutinyTimeChange(s);
        EditSpiritPowerRate = MakeSpin("叛变威力倍率:", 70, 10, mutinyGb, 65535);
        EditSpiritPowerRate.ValueChanged += (s, e) => EditSpiritPowerRateChange(s);
        ButtonSpiritMutinySave = new System.Windows.Forms.Button { Text = "保存叛变(&V)", Left = 8, Top = 160, Width = 110, Height = 26, Enabled = false };
        ButtonSpiritMutinySave.Click += (s, e) => ButtonSpiritMutinySaveClick(s);
        mutinyTab.Controls.Add(ButtonSpiritMutinySave);

        // ---- 怪物发言页（批次J29） ----
        var sayGb = new System.Windows.Forms.GroupBox { Text = "怪物发言", Left = 8, Top = 4, Width = 320, Height = 90 };
        sayTab.Controls.Add(sayGb);
        CheckBoxMonSayMsg = MkChk("启用怪物发言", 12, 20, sayGb, CheckBoxMonSayMsgClick);
        ButtonMonSayMsgSave = new System.Windows.Forms.Button { Text = "保存发言(&Y)", Left = 8, Top = 100, Width = 110, Height = 26, Enabled = false };
        ButtonMonSayMsgSave.Click += (s, e) => ButtonMonSayMsgSaveClick(s);
        sayTab.Controls.Add(ButtonMonSayMsgSave);

        // ---- 武器炼制页（批次J29） ----
        var luckGb = new System.Windows.Forms.GroupBox { Text = "武器炼 luck", Left = 8, Top = 4, Width = 648, Height = 190 };
        luckTab.Controls.Add(luckGb);
        ScrollBarWeaponMakeUnLuckRate = MakeSpin("不幸率:", 16, 10, luckGb, 1000);
        ScrollBarWeaponMakeUnLuckRate.ValueChanged += (s, e) => ScrollBarWeaponMakeUnLuckRateChange(s);
        EditWeaponMakeUnLuckRate = AddMirror(luckGb, ScrollBarWeaponMakeUnLuckRate);
        ScrollBarWeaponMakeLuckPoint1 = MakeSpin("一点 luck:", 42, 10, luckGb, 1000);
        ScrollBarWeaponMakeLuckPoint1.ValueChanged += (s, e) => ScrollBarWeaponMakeLuckPoint1Change(s);
        ScrollBarWeaponMakeLuckPoint2 = MakeSpin("二点 luck:", 68, 10, luckGb, 1000);
        ScrollBarWeaponMakeLuckPoint2.ValueChanged += (s, e) => ScrollBarWeaponMakeLuckPoint2Change(s);
        ScrollBarWeaponMakeLuckPoint3 = MakeSpin("三点 luck:", 94, 10, luckGb, 1000);
        ScrollBarWeaponMakeLuckPoint3.ValueChanged += (s, e) => ScrollBarWeaponMakeLuckPoint3Change(s);
        ScrollBarWeaponMakeLuckPoint2Rate = MakeSpin("二点比率:", 42, 330, luckGb, 1000);
        ScrollBarWeaponMakeLuckPoint2Rate.ValueChanged += (s, e) => ScrollBarWeaponMakeLuckPoint2RateChange(s);
        ScrollBarWeaponMakeLuckPoint3Rate = MakeSpin("三点比率:", 68, 330, luckGb, 1000);
        ScrollBarWeaponMakeLuckPoint3Rate.ValueChanged += (s, e) => ScrollBarWeaponMakeLuckPoint3RateChange(s);

        ButtonWeaponMakeLuckSave = new System.Windows.Forms.Button { Text = "保存炼制(&W)", Left = 8, Top = 200, Width = 110, Height = 26, Enabled = false };
        ButtonWeaponMakeLuckSave.Click += (s, e) => ButtonWeaponMakeLuckSaveClick(s);
        luckTab.Controls.Add(ButtonWeaponMakeLuckSave);
        ButtonWeaponMakeLuckDefaulf = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 128, Top = 200, Width = 90, Height = 26 };
        ButtonWeaponMakeLuckDefaulf.Click += (s, e) => ButtonWeaponMakeLuckDefaulfClick(s);
        luckTab.Controls.Add(ButtonWeaponMakeLuckDefaulf);

        // 其余页保存按钮（禁用态占位，处理器随巨片拆分接入）
        // （ButtonSkillSave/ButtonUpgradeWeaponSave 已随批次J26 接入真实控件）
        btnBonusAbilofSave = Hidden();
        btnOther3 = Hidden();
    }

    private System.Windows.Forms.Button Hidden() => new() { Left = 0, Top = 0, Width = 0, Height = 0, Enabled = false };

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        ButtonPasswordLockSave.Enabled = true;
        ButtonGeneralSave.Enabled = true;
        ButtonSkillSave.Enabled = true;
        ButtonUpgradeWeaponSave.Enabled = true;
        ButtonMasterSave.Enabled = true;
        ButtonMakeMineSave.Enabled = true;
        ButtonWinLotterySave.Enabled = true;
        ButtonReNewLevelSave.Enabled = true;
        ButtonMonUpgradeSave.Enabled = true;
        ButtonSpiritMutinySave.Enabled = true;
        ButtonMonSayMsgSave.Enabled = true;
        ButtonHeroOptionSave.Enabled = true;
        ButtonOffLineSave.Enabled = true;
        ButtonMyShopSave.Enabled = true;
        ButtonOther.Enabled = true;
        btnBonusAbilofSave.Enabled = true;
        btnOther3.Enabled = true;
        ButtonWeaponMakeLuckSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        ButtonPasswordLockSave.Enabled = false;
        ButtonGeneralSave.Enabled = false;
        ButtonSkillSave.Enabled = false;
        ButtonUpgradeWeaponSave.Enabled = false;
        ButtonMasterSave.Enabled = false;
        ButtonMakeMineSave.Enabled = false;
        ButtonWinLotterySave.Enabled = false;
        ButtonReNewLevelSave.Enabled = false;
        ButtonMonUpgradeSave.Enabled = false;
        ButtonSpiritMutinySave.Enabled = false;
        ButtonMonSayMsgSave.Enabled = false;
        ButtonHeroOptionSave.Enabled = false;
        ButtonOffLineSave.Enabled = false;
        ButtonMyShopSave.Enabled = false;
        ButtonOther.Enabled = false;
        btnBonusAbilofSave.Enabled = false;
        boSendServerConfig = false;
        btnOther3.Enabled = false;
        ButtonWeaponMakeLuckSave.Enabled = false;
    }

    /// <summary>FunctionConfigControlChanging 1:1（Delphi var AllowChange → 返回值）。</summary>
    public bool FunctionConfigControlChanging()
    {
        if (boModValued)
        {
            if (M2Forms.MessageBox("参数设置已经被修改，是否确认不保存修改的设置？", "确认信息",
                    M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
            {
                uModValue();
            }
            else
                return false;
        }
        return true;
    }

    /// <summary>Delphi DoOpen() 子集（Skill/升级武器/魔兽/英雄等页加载随巨片拆分接入）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        RefGeneral();
        CheckBoxHungerSystem.Checked = M2Config.boHungerSystem;
        CheckBoxHungerDecHP.Checked = M2Config.boHungerDecHP;
        CheckBoxHungerDecPower.Checked = M2Config.boHungerDecPower;
        CheckBoxHungerSystemClick(CheckBoxHungerSystem);

        CheckBoxEnablePasswordLock.Checked = M2Config.boPasswordLockSystem;
        CheckBoxLockGetBackItem.Checked = M2Config.boLockGetBackItemAction;
        CheckBoxLockDealItem.Checked = M2Config.boLockDealAction;
        CheckBoxLockDropItem.Checked = M2Config.boLockDropAction;
        CheckBoxLockWalk.Checked = M2Config.boLockWalkAction;
        CheckBoxLockRun.Checked = M2Config.boLockRunAction;
        CheckBoxLockHit.Checked = M2Config.boLockHitAction;
        CheckBoxLockSpell.Checked = M2Config.boLockSpellAction;
        CheckBoxLockSendMsg.Checked = M2Config.boLockSendMsgAction;
        CheckBoxLockInObMode.Checked = M2Config.boLockInObModeAction;

        CheckBoxLockLogin.Checked = M2Config.boLockHumanLogin;
        CheckBoxLockUseItem.Checked = M2Config.boLockUserItemAction;
        chkLockChallenge.Checked = M2Config.boLockChallenge;
        chkLockSummonHero.Checked = M2Config.boLockSummonHero;
        chkLockShop.Checked = M2Config.boLockShop;
        chkLockStall.Checked = M2Config.boLockStall;

        CheckBoxEnablePasswordLockClick(CheckBoxEnablePasswordLock);
        CheckBoxLockLoginClick(CheckBoxLockLogin);

        EditErrorPasswordCount.Value = M2Config.nPasswordErrorCountLock;
        RefMagicSkill();
        RefUpgradeWeapon();
        RefMasterMineLottery();
        RefReNewMonUpgrade();
        RefMutinyMsgLuck();
        RefOffLineMyShop();
        RefFinalPages();
        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    /// <summary>英雄选项/其他/属性点/扩展页回显（批次J31）。</summary>
    public void RefFinalPages()
    {
        CheckBoxHeroGetAllExp.Checked = M2Config.boHeroGetAllExp;
        CheckBoxHumanGetAllExp.Checked = M2Config.boHumanGetAllExp;
        EditHeroKillMonExpRate.Value = M2Config.nHeroKillMonExpRate;
        CheckBoxAllowCopySelf.Checked = M2Config.boAllowCopySelf;
        EditLimitExpLevel.Value = M2Config.nLimitExpLevel;
        EditRecallHeroTime.Value = M2Config.nRecallHeroTime;
        EditHeroWarrorAttackTime.Value = M2Config.dwHeroWarrorAttackTime;
        for (int i = 1; i <= 1000; i++)
            GridHeroExp.Rows[i - 1].Cells[1].Value = M2Config.dwHeroNeedExps[i].ToString();

        CheckBoxDeleteItemDuraZero.Checked = M2Config.boDeleteItemDuraZero;
        EditMysteriousManName.Text = M2Config.sMysteriousManName;
        EditMaxLuckMaxPower.Value = M2Config.nMaxLuckMaxPower;
        ((System.Windows.Forms.NumericUpDown)OtherControls["QueryBagItemsTime"]).Value = M2Config.nQueryBagItemsTime;

        EditBonusAbilofWarrDC.Value = M2Config.BonusAbilofWarr.DC;
        EditBonusAbilofTaosSpeed.Value = M2Config.BonusAbilofTaos.Speed;

        EditRevivalTime.Value = M2Config.dwRevivalTime / 1000;
        CheckBoxRevivalTouch.Checked = M2Config.boRevivalTouch;
        EditStarBaseNum.Value = M2Config.nStarBaseNum;
    }

    /// <summary>离线/商铺页回显（批次J30）。</summary>
    public void RefOffLineMyShop()
    {
        CheckBoxOffLineLoginSafeArea.Checked = M2Config.boOffLineLoginSafeArea;
        EditOffLineLoginMapName.Value = M2Config.btOffLineLoginMapName;
        EditSetOffLineLoginMapName.Text = M2Config.sSetOffLineLoginMapName;
        CheckBoxMonNoAttackOffLinePlayer.Checked = M2Config.boMonNoAttackOffLinePlayer;

        EditMaxMyShopSellingItemCount.Value = M2Config.nMaxMyShopSellingItemCount;
        EditMaxMyShopStorageItemCount.Value = M2Config.nMaxMyShopStorageItemCount;
        CheckBoxOfflineCloseMyShop.Checked = M2Config.boOfflineCloseMyShop;
        CheckBoxProhibitModifyPrices.Checked = M2Config.boProhibitModifyPrices;
        CheckBoxUseHeroM2Shop.Checked = M2Config.boUseHeroM2Shop;
        CheckBoxInfinityStorage.Checked = M2Config.boInfinityStorage;
        EditInfinityStorageCount.Value = M2Config.nInfinityStorageCount;
        CheckBoxMyShopGold.Checked = M2Config.boMyShopGold;
        CheckBoxMyShopGameGold.Checked = M2Config.boMyShopGameGold;
        CheckBoxMyShopGameDiamond.Checked = M2Config.boMyShopGameDiamond;
        CheckBoxMyShopGameGird.Checked = M2Config.boMyShopGameGird;
        CheckBoxMyShopGamePoint.Checked = M2Config.boMyShopGamePoint;
        CheckBoxOpenSelfShop.Checked = M2Config.boOpenSelfShop;
        CheckBoxSafeZoneShop.Checked = M2Config.boSafeZoneShop;
        CheckBoxMapShop.Checked = M2Config.boMapShop;
        CheckBoxShopStallCanNotAttack.Checked = M2Config.boShopStallCanNotAttack;
        EditSellOffGoldTaxRate.Value = M2Config.dwSellOffGoldTaxRate;
        EditSellOffGameGoldTaxRate.Value = M2Config.dwSellOffGameGoldTaxRate;
        EditSellOffGameDiamondTaxRate.Value = M2Config.dwSellOffGameDiamondTaxRate;
        EditSellOffGameGirdTaxRate.Value = M2Config.dwSellOffGameGirdTaxRate;
        EditSellOffGamePointTaxRate.Value = M2Config.dwSellOffGamePointTaxRate;
        CheckBoxShopHeadPic.Checked = M2Config.boShopHeadPic;
    }

    /// <summary>叛变/怪物发言/炼 luck 页回显（批次J29；RefSpiritMutiny/RefWeaponMakeLuck 1:1）。</summary>
    public void RefMutinyMsgLuck()
    {
        CheckBoxSpiritMutiny.Checked = M2Config.boSpiritMutiny;
        EditSpiritMutinyTime.Value = M2Config.dwSpiritMutinyTime / (60 * 1000);
        EditSpiritPowerRate.Value = M2Config.nSpiritPowerRate;
        CheckBoxSpiritMutinyClick(CheckBoxSpiritMutiny);

        CheckBoxMonSayMsg.Checked = M2Config.boMonSayMsg;

        ScrollBarWeaponMakeUnLuckRate.Value = Math.Clamp(M2Config.nWeaponMakeUnLuckRate, 1, 1000);
        EditWeaponMakeUnLuckRate.Text = M2Config.nWeaponMakeUnLuckRate.ToString();
        ScrollBarWeaponMakeLuckPoint1.Value = M2Config.nWeaponMakeLuckPoint1;
        ScrollBarWeaponMakeLuckPoint2.Value = M2Config.nWeaponMakeLuckPoint2;
        ScrollBarWeaponMakeLuckPoint3.Value = M2Config.nWeaponMakeLuckPoint3;
        ScrollBarWeaponMakeLuckPoint2Rate.Value = M2Config.nWeaponMakeLuckPoint2Rate;
        ScrollBarWeaponMakeLuckPoint3Rate.Value = M2Config.nWeaponMakeLuckPoint3Rate;
    }

    /// <summary>转生/宝宝升级页回显（批次J28；RefMonUpgrade 1:1 含 Mutiny 级联）。</summary>
    public void RefReNewMonUpgrade()
    {
        for (int i = 0; i < 10; i++)
            EditReNewNameColorCells[i].Value = M2Config.ReNewNameColor[i];
        EditReNewNameColorTime.Value = M2Config.dwReNewNameColorTime / 1000;
        CheckBoxReNewChangeColor.Checked = M2Config.boReNewChangeColor;
        CheckBoxReNewLevelClearExp.Checked = M2Config.boReNewLevelClearExp;

        for (int i = 0; i < 10; i++)
            EditMonUpgradeColorCells[i].Value = M2Config.SlaveColor[i];
        for (int i = 0; i < 7; i++)
            EditMonUpgradeKillCountCells[i].Value = M2Config.MonUpLvNeedKillCount[i];
        EditMonUpLvNeedKillBase.Value = M2Config.nMonUpLvNeedKillBase;
        EditMonUpLvRate.Value = M2Config.nMonUpLvRate;
        CheckBoxMasterDieMutiny.Checked = M2Config.boMasterDieMutiny;
        EditMasterDieMutinyRate.Value = M2Config.nMasterDieMutinyRate;
        EditMasterDieMutinyPower.Value = M2Config.nMasterDieMutinyPower;
        EditMasterDieMutinySpeed.Value = M2Config.nMasterDieMutinySpeed;
        CheckBoxMasterDieMutinyClick(CheckBoxMasterDieMutiny);
        CheckBoxBBMonAutoChangeColor.Checked = M2Config.boBBMonAutoChangeColor;
        EditBBMonAutoChangeColorTime.Value = M2Config.dwBBMonAutoChangeColorTime / 1000;
    }

    /// <summary>Master/MakeMine/WinLottery 页回显（批次J27；RefWinLottery 1:1 含级联 Min/Max）。</summary>
    public void RefMasterMineLottery()
    {
        EditMasterOKLevel.Value = M2Config.nMasterOKLevel;
        EditMasterOKCreditPoint.Value = M2Config.nMasterOKCreditPoint;
        EditMasterOKBonusPoint.Value = M2Config.nMasterOKBonusPoint;

        EditMakeMineHitRate.Value = M2Config.nMakeMineHitRate;
        EditMakeMineRate.Value = M2Config.nMakeMineRate;
        EditStoneTypeRate.Value = M2Config.nStoneTypeRate;
        EditStoneTypeRateMin.Value = M2Config.nStoneTypeRateMin;
        EditGoldStoneMin.Value = M2Config.nGoldStoneMin;
        EditGoldStoneMax.Value = M2Config.nGoldStoneMax;
        EditSilverStoneMin.Value = M2Config.nSilverStoneMin;
        EditSilverStoneMax.Value = M2Config.nSilverStoneMax;
        EditSteelStoneMin.Value = M2Config.nSteelStoneMin;
        EditSteelStoneMax.Value = M2Config.nSteelStoneMax;
        EditBlackStoneMin.Value = M2Config.nBlackStoneMin;
        EditBlackStoneMax.Value = M2Config.nBlackStoneMax;
        EditStoneMinDura.Value = M2Config.nStoneMinDura;
        EditStoneGeneralDuraRate.Value = M2Config.nStoneGeneralDuraRate;
        EditStoneAddDuraRate.Value = M2Config.nStoneAddDuraRate;
        EditStoneAddDuraMax.Value = M2Config.nStoneAddDuraMax;

        ScrollBarWinLotteryRate.Maximum = 100000;
        ScrollBarWinLotteryRate.Value = Math.Clamp(M2Config.nWinLotteryRate, 1, 100000);
        EditWinLotteryRate.Text = M2Config.nWinLotteryRate.ToString();
        ScrollBarWinLottery1Max.Maximum = M2Config.nWinLotteryRate;
        ScrollBarWinLottery1Max.Minimum = Math.Clamp(M2Config.nWinLottery1Min, 1, 100000);
        ScrollBarWinLottery2Max.Maximum = Math.Clamp(M2Config.nWinLottery1Max, 1, 100000);
        ScrollBarWinLottery2Max.Minimum = Math.Clamp(M2Config.nWinLottery2Min, 1, 100000);
        ScrollBarWinLottery3Max.Maximum = Math.Clamp(M2Config.nWinLottery2Max, 1, 100000);
        ScrollBarWinLottery3Max.Minimum = Math.Clamp(M2Config.nWinLottery3Min, 1, 100000);
        ScrollBarWinLottery4Max.Maximum = Math.Clamp(M2Config.nWinLottery3Max, 1, 100000);
        ScrollBarWinLottery4Max.Minimum = Math.Clamp(M2Config.nWinLottery4Min, 1, 100000);
        ScrollBarWinLottery5Max.Maximum = Math.Clamp(M2Config.nWinLottery4Max, 1, 100000);
        ScrollBarWinLottery5Max.Minimum = Math.Clamp(M2Config.nWinLottery5Min, 1, 100000);
        ScrollBarWinLottery6Max.Maximum = Math.Clamp(M2Config.nWinLottery5Max, 1, 100000);
        ScrollBarWinLottery6Max.Minimum = Math.Clamp(M2Config.nWinLottery6Min, 1, 100000);
        ScrollBarWinLottery1Max.Value = Math.Clamp(M2Config.nWinLottery1Max, ScrollBarWinLottery1Max.Minimum, ScrollBarWinLottery1Max.Maximum);
        ScrollBarWinLottery2Max.Value = Math.Clamp(M2Config.nWinLottery2Max, ScrollBarWinLottery2Max.Minimum, ScrollBarWinLottery2Max.Maximum);
        ScrollBarWinLottery3Max.Value = Math.Clamp(M2Config.nWinLottery3Max, ScrollBarWinLottery3Max.Minimum, ScrollBarWinLottery3Max.Maximum);
        ScrollBarWinLottery4Max.Value = Math.Clamp(M2Config.nWinLottery4Max, ScrollBarWinLottery4Max.Minimum, ScrollBarWinLottery4Max.Maximum);
        ScrollBarWinLottery5Max.Value = Math.Clamp(M2Config.nWinLottery5Max, ScrollBarWinLottery5Max.Minimum, ScrollBarWinLottery5Max.Maximum);
        ScrollBarWinLottery6Max.Value = Math.Clamp(M2Config.nWinLottery6Max, ScrollBarWinLottery6Max.Minimum, ScrollBarWinLottery6Max.Maximum);
        EditWinLottery1Gold.Value = M2Config.nWinLottery1Gold;
        EditWinLottery2Gold.Value = M2Config.nWinLottery2Gold;
        EditWinLottery3Gold.Value = M2Config.nWinLottery3Gold;
        EditWinLottery4Gold.Value = M2Config.nWinLottery4Gold;
        EditWinLottery5Gold.Value = M2Config.nWinLottery5Gold;
        EditWinLottery6Gold.Value = M2Config.nWinLottery6Gold;
    }

    /// <summary>RefMagicSkill 头部区段 1:1（RefMagicSkill 对应片段）。</summary>
    public void RefMagicSkill()
    {
        EditSwordLongPowerRate.Value = M2Config.nSwordLongPowerRate;
        CheckBoxLimitSwordLong.Checked = M2Config.boLimitSwordLong;
        seFireBoomRage.Value = M2Config.nFireBoomRage;
        seSnowWindRange.Value = M2Config.nSnowWindRange;
        seElecBlizzardRange.Value = M2Config.nElecBlizzardRange;
        seMagTurnUndeadLevel.Value = M2Config.nMagTurnUndeadLevel;
        EditMagicAttackRage.Value = M2Config.nMagicAttackRage;
        EditAmyOunsulPoint.Value = M2Config.nAmyOunsulPoint;
        CheckBoxFireCrossInSafeZone.Checked = M2Config.boDisableInSafeZoneFireCross;
        chkSkill41MbAttackPlayObject.Checked = M2Config.boSkill41MbAttackPlayObject;
        EditMagTammingLevel.Value = M2Config.nMagTammingLevel;

        EditBoneFammName.Text = M2Config.sBoneFamm;
        EditBoneFammCount.Value = M2Config.nBoneFammCount;
        for (int i = 0; i < 10; i++)
        {
            if (M2Config.BoneFammArray[i].nHumLevel <= 0)
                break;
            GridBoneFamm.Rows[i].Cells[0].Value = M2Config.BoneFammArray[i].nHumLevel.ToString();
            GridBoneFamm.Rows[i].Cells[1].Value = M2Config.BoneFammArray[i].sMonName;
            GridBoneFamm.Rows[i].Cells[2].Value = M2Config.BoneFammArray[i].nCount.ToString();
            GridBoneFamm.Rows[i].Cells[3].Value = M2Config.BoneFammArray[i].nLevel.ToString();
        }
        EditDogzName.Text = M2Config.sDogz;
        EditDogzCount.Value = M2Config.nDogzCount;
        for (int i = 0; i < 10; i++)
        {
            if (M2Config.DogzArray[i].nHumLevel <= 0)
                break;
            GridDogz.Rows[i].Cells[0].Value = M2Config.DogzArray[i].nHumLevel.ToString();
            GridDogz.Rows[i].Cells[1].Value = M2Config.DogzArray[i].sMonName;
            GridDogz.Rows[i].Cells[2].Value = M2Config.DogzArray[i].nCount.ToString();
            GridDogz.Rows[i].Cells[3].Value = M2Config.DogzArray[i].nLevel.ToString();
        }
    }

    /// <summary>RefUpgradeWeapon 1:1。</summary>
    public void RefUpgradeWeapon()
    {
        ScrollBarUpgradeWeaponDCRate.Value = Math.Clamp(M2Config.nUpgradeWeaponDCRate, 1, 1000);
        ScrollBarUpgradeWeaponDCTwoPointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponDCTwoPointRate, 1, 1000);
        ScrollBarUpgradeWeaponDCThreePointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponDCThreePointRate, 1, 1000);
        ScrollBarUpgradeWeaponMCRate.Value = Math.Clamp(M2Config.nUpgradeWeaponMCRate, 1, 1000);
        ScrollBarUpgradeWeaponMCTwoPointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponMCTwoPointRate, 1, 1000);
        ScrollBarUpgradeWeaponMCThreePointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponMCThreePointRate, 1, 1000);
        ScrollBarUpgradeWeaponSCRate.Value = Math.Clamp(M2Config.nUpgradeWeaponSCRate, 1, 1000);
        ScrollBarUpgradeWeaponSCTwoPointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponSCTwoPointRate, 1, 1000);
        ScrollBarUpgradeWeaponSCThreePointRate.Value = Math.Clamp(M2Config.nUpgradeWeaponSCThreePointRate, 1, 1000);
        EditUpgradeWeaponMaxPoint.Value = M2Config.nUpgradeWeaponMaxPoint;
        EditUpgradeWeaponPrice.Value = M2Config.nUpgradeWeaponPrice;
        EditUPgradeWeaponGetBackTime.Value = M2Config.dwUPgradeWeaponGetBackTime / 1000;
        EditClearExpireUpgradeWeaponDays.Value = M2Config.nClearExpireUpgradeWeaponDays;
        CheckBoxWeaponUpgradeFailNotDelete.Checked = M2Config.boWeaponUpgradeFailNotDelete;
    }

    public void RefGeneral()
    {
        EditPKFlagNameColor.Value = M2Config.btPKFlagNameColor;
        EditPKLevel1NameColor.Value = M2Config.btPKLevel1NameColor;
        EditPKLevel2NameColor.Value = M2Config.btPKLevel2NameColor;
        EditAllyAndGuildNameColor.Value = M2Config.btAllyAndGuildNameColor;
        EditWarGuildNameColor.Value = M2Config.btWarGuildNameColor;
        EditInFreePKAreaNameColor.Value = M2Config.btInFreePKAreaNameColor;
        EditMerchantNameColor.Value = M2Config.btMerchantNameColor;
        seMerchant273NameColor.Value = M2Config.btMerchant273NameColor;
        seGuardNameColor.Value = M2Config.btGuardNameColor;
    }

    // ---- 密码保护页 ----

    public void CheckBoxEnablePasswordLockClick(object? sender)
    {
        if (CheckBoxEnablePasswordLock.Checked)
        {
            CheckBoxLockGetBackItem.Enabled = true;
            CheckBoxLockLogin.Enabled = true;
        }
        else
        {
            CheckBoxLockGetBackItem.Checked = false;
            CheckBoxLockLogin.Checked = false;

            CheckBoxLockGetBackItem.Enabled = false;
            CheckBoxLockLogin.Enabled = false;
        }
        if (!boOpened)
            return;
        M2Config.boPasswordLockSystem = CheckBoxEnablePasswordLock.Checked;
        ModValue();
    }

    public void CheckBoxLockGetBackItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockGetBackItemAction = CheckBoxLockGetBackItem.Checked;
        ModValue();
    }

    public void CheckBoxLockDealItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockDealAction = CheckBoxLockDealItem.Checked;
        ModValue();
    }

    public void CheckBoxLockDropItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockDropAction = CheckBoxLockDropItem.Checked;
        ModValue();
    }

    public void CheckBoxLockUseItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockUserItemAction = CheckBoxLockUseItem.Checked;
        ModValue();
    }

    public void CheckBoxLockLoginClick(object? sender)
    {
        if (CheckBoxLockLogin.Checked)
        {
            CheckBoxLockWalk.Enabled = true;
            CheckBoxLockRun.Enabled = true;
            CheckBoxLockHit.Enabled = true;
            CheckBoxLockSpell.Enabled = true;
            CheckBoxLockInObMode.Enabled = true;
            CheckBoxLockSendMsg.Enabled = true;
            CheckBoxLockDealItem.Enabled = true;
            CheckBoxLockDropItem.Enabled = true;
            CheckBoxLockUseItem.Enabled = true;

            chkLockChallenge.Enabled = true;
            chkLockSummonHero.Enabled = true;
            chkLockShop.Enabled = true;
            chkLockStall.Enabled = true;
        }
        else
        {
            CheckBoxLockWalk.Checked = false;
            CheckBoxLockRun.Checked = false;
            CheckBoxLockHit.Checked = false;
            CheckBoxLockSpell.Checked = false;
            CheckBoxLockInObMode.Checked = false;
            CheckBoxLockSendMsg.Checked = false;
            CheckBoxLockDealItem.Checked = false;
            CheckBoxLockDropItem.Checked = false;
            CheckBoxLockUseItem.Checked = false;
            chkLockChallenge.Checked = false;
            chkLockSummonHero.Checked = false;
            chkLockShop.Checked = false;
            chkLockStall.Checked = false;

            CheckBoxLockWalk.Enabled = false;
            CheckBoxLockRun.Enabled = false;
            CheckBoxLockHit.Enabled = false;
            CheckBoxLockSpell.Enabled = false;
            CheckBoxLockInObMode.Enabled = false;
            CheckBoxLockSendMsg.Enabled = false;
            CheckBoxLockDealItem.Enabled = false;
            CheckBoxLockDropItem.Enabled = false;
            CheckBoxLockUseItem.Enabled = false;
            chkLockChallenge.Enabled = false;
            chkLockSummonHero.Enabled = false;
            chkLockShop.Enabled = false;
            chkLockStall.Enabled = false;
        }
        if (!boOpened)
            return;
        M2Config.boLockHumanLogin = CheckBoxLockLogin.Checked;
        ModValue();
    }

    public void CheckBoxLockWalkClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockWalkAction = CheckBoxLockWalk.Checked;
        ModValue();
    }

    public void CheckBoxLockRunClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockRunAction = CheckBoxLockRun.Checked;
        ModValue();
    }

    public void CheckBoxLockHitClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockHitAction = CheckBoxLockHit.Checked;
        ModValue();
    }

    public void CheckBoxLockSpellClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockSpellAction = CheckBoxLockSpell.Checked;
        ModValue();
    }

    public void CheckBoxLockSendMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockSendMsgAction = CheckBoxLockSendMsg.Checked;
        ModValue();
    }

    public void CheckBoxLockInObModeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockInObModeAction = CheckBoxLockInObMode.Checked;
        ModValue();
    }

    public void chkLockChallengeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockChallenge = chkLockChallenge.Checked;
        ModValue();
    }

    public void chkLockSummonHeroClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockSummonHero = chkLockSummonHero.Checked;
        ModValue();
    }

    public void chkLockShopClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockShop = chkLockShop.Checked;
        ModValue();
    }

    public void chkLockStallClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLockStall = chkLockStall.Checked;
        ModValue();
    }

    public void EditErrorPasswordCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nPasswordErrorCountLock = (int)EditErrorPasswordCount.Value;
        ModValue();
    }

    /// <summary>Delphi 原文：CheckBoxErrorCountKickClick 同样写 nPasswordErrorCountLock（原文复制粘贴保留）。</summary>
    public void CheckBoxErrorCountKickClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nPasswordErrorCountLock = (int)EditErrorPasswordCount.Value;
        ModValue();
    }

    public void ButtonPasswordLockSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "PasswordLockSystem", M2Config.boPasswordLockSystem);
        Config.WriteBool("Setup", "PasswordLockDealAction", M2Config.boLockDealAction);
        Config.WriteBool("Setup", "PasswordLockDropAction", M2Config.boLockDropAction);
        Config.WriteBool("Setup", "PasswordLockGetBackItemAction", M2Config.boLockGetBackItemAction);
        Config.WriteBool("Setup", "PasswordLockWalkAction", M2Config.boLockWalkAction);
        Config.WriteBool("Setup", "PasswordLockRunAction", M2Config.boLockRunAction);
        Config.WriteBool("Setup", "PasswordLockHitAction", M2Config.boLockHitAction);
        Config.WriteBool("Setup", "PasswordLockSpellAction", M2Config.boLockSpellAction);
        Config.WriteBool("Setup", "PasswordLockSendMsgAction", M2Config.boLockSendMsgAction);
        Config.WriteBool("Setup", "PasswordLockInObModeAction", M2Config.boLockInObModeAction);
        Config.WriteBool("Setup", "PasswordLockUserItemAction", M2Config.boLockUserItemAction);

        Config.WriteBool("Setup", "LockChallenge", M2Config.boLockChallenge);
        // 禁止挑战
        Config.WriteBool("Setup", "LockSummonHero", M2Config.boLockSummonHero);
        // 禁止召唤英雄
        Config.WriteBool("Setup", "LockShop", M2Config.boLockShop);
        // 禁止商铺
        Config.WriteBool("Setup", "LockStall", M2Config.boLockStall);
        // 禁止摆摊

        Config.WriteBool("Setup", "PasswordLockHumanLogin", M2Config.boLockHumanLogin);
        Config.WriteInteger("Setup", "PasswordErrorCountLock", M2Config.nPasswordErrorCountLock);

        if (boSendServerConfig)
            GameConfigState.SendServerConfig();

        uModValue();
    }

    // ---- 常规页 ----

    public void CheckBoxHungerSystemClick(object? sender)
    {
        if (CheckBoxHungerSystem.Checked)
        {
            CheckBoxHungerDecHP.Enabled = true;
            CheckBoxHungerDecPower.Enabled = true;
        }
        else
        {
            CheckBoxHungerDecHP.Checked = false;
            CheckBoxHungerDecPower.Checked = false;
            CheckBoxHungerDecHP.Enabled = false;
            CheckBoxHungerDecPower.Enabled = false;
        }
        if (!boOpened)
            return;
        M2Config.boHungerSystem = CheckBoxHungerSystem.Checked;
        ModValue();
    }

    public void CheckBoxHungerDecHPClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHungerDecHP = CheckBoxHungerDecHP.Checked;
        ModValue();
    }

    public void CheckBoxHungerDecPowerClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHungerDecPower = CheckBoxHungerDecPower.Checked;
        ModValue();
    }

    public void ButtonGeneralSaveClick(object? sender)
    {
        if (CheckBoxItemName.Checked && (GetNameInFilterListHandler?.Invoke(EditItemName.Text) ?? false))
        {
            M2Forms.MessageBox("装备刻名自定义前缀包含非法字符", "提示", M2Forms.MB_OK);
            if (EditItemName.CanFocus)
                EditItemName.Focus();
            return;
        }

        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "HungerSystem", M2Config.boHungerSystem);
        Config.WriteBool("Setup", "HungerDecHP", M2Config.boHungerDecHP);
        Config.WriteBool("Setup", "HungerDecPower", M2Config.boHungerDecPower);

        Config.WriteInteger("Setup", "PKFlagNameColor", M2Config.btPKFlagNameColor);
        Config.WriteInteger("Setup", "AllyAndGuildNameColor", M2Config.btAllyAndGuildNameColor);
        Config.WriteInteger("Setup", "WarGuildNameColor", M2Config.btWarGuildNameColor);
        Config.WriteInteger("Setup", "InFreePKAreaNameColor", M2Config.btInFreePKAreaNameColor);
        Config.WriteInteger("Setup", "PKLevel1NameColor", M2Config.btPKLevel1NameColor);
        Config.WriteInteger("Setup", "PKLevel2NameColor", M2Config.btPKLevel2NameColor);
        Config.WriteInteger("Setup", "MerchantNameColor", M2Config.btMerchantNameColor);
        Config.WriteInteger("Setup", "Merchant273NameColor", M2Config.btMerchant273NameColor);
        Config.WriteInteger("Setup", "GuardNameColor", M2Config.btGuardNameColor);

        Config.WriteInteger("Setup", "HPRockRate", M2Config.nHPRockRate);
        Config.WriteInteger("Setup", "HPRockType", M2Config.nHPRockType);
        Config.WriteInteger("Setup", "HPRockTime", M2Config.nHPRockTime);
        Config.WriteInteger("Setup", "HPRockAddType", M2Config.nHPRockAddType);
        Config.WriteInteger("Setup", "HPRockAddValue", M2Config.nHPRockAddValue);
        Config.WriteInteger("Setup", "HPRockDecValue", M2Config.nHPRockDecValue);

        Config.WriteInteger("Setup", "MPRockRate", M2Config.nMPRockRate);
        Config.WriteInteger("Setup", "MPRockType", M2Config.nMPRockType);
        Config.WriteInteger("Setup", "MPRockTime", M2Config.nMPRockTime);
        Config.WriteInteger("Setup", "MPRockAddType", M2Config.nMPRockAddType);
        Config.WriteInteger("Setup", "MPRockAddValue", M2Config.nMPRockAddValue);
        Config.WriteInteger("Setup", "MPRockDecValue", M2Config.nMPRockDecValue);

        Config.WriteInteger("Setup", "HMPRockRate", M2Config.nHMPRockRate);
        Config.WriteInteger("Setup", "HMPRockType", M2Config.nHMPRockType);
        Config.WriteInteger("Setup", "HMPRockTime", M2Config.nHMPRockTime);
        Config.WriteInteger("Setup", "HMPRockAddType", M2Config.nHMPRockAddType);
        Config.WriteInteger("Setup", "HMPRockAddValue", M2Config.nHMPRockAddValue);
        Config.WriteInteger("Setup", "HMPRockDecValue", M2Config.nHMPRockDecValue);
        Config.WriteBool("Setup", "HMPUse2Times", M2Config.boHMPUse2Times);
        Config.WriteInteger("Setup", "HMPDivDura", M2Config.nHMPDivDura);

        Config.WriteBool("Setup", "DropOverLapItem", M2Config.boDropOverLapItem);
        Config.WriteBool("Setup", "OpenMapEvent", M2Config.boOpenMapEvent);
        uModValue();
        GameConfigState.SendServerConfig();
    }

    // ================= Skill 技能页（批次J26 头部区段） =================

    public void EditMagicAttackRageChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagicAttackRage = (int)EditMagicAttackRage.Value;
        ModValue();
    }

    public void EditBoneFammCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nBoneFammCount = (int)EditBoneFammCount.Value;
        ModValue();
    }

    public void EditDogzCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nDogzCount = (int)EditDogzCount.Value;
        ModValue();
    }

    public void CheckBoxLimitSwordLongClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boLimitSwordLong = CheckBoxLimitSwordLong.Checked;
        ModValue();
    }

    public void EditSwordLongPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSwordLongPowerRate = (int)EditSwordLongPowerRate.Value;
        ModValue();
    }

    public void EditBoneFammNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sBoneFamm = EditBoneFammName.Text.Trim();
        ModValue();
    }

    public void EditDogzNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sDogz = EditDogzName.Text.Trim();
        ModValue();
    }

    public void seFireBoomRageChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nFireBoomRage = (int)seFireBoomRage.Value;
        ModValue();
    }

    public void seSnowWindRangeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSnowWindRange = (int)seSnowWindRange.Value;
        ModValue();
    }

    public void seElecBlizzardRangeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nElecBlizzardRange = (int)seElecBlizzardRange.Value;
        ModValue();
    }

    public void seMagTurnUndeadLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagTurnUndeadLevel = (int)seMagTurnUndeadLevel.Value;
        ModValue();
    }

    public void EdiAmyOunsulPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nAmyOunsulPoint = (int)EditAmyOunsulPoint.Value;
        ModValue();
    }

    public void CheckBoxFireCrossInSafeZoneClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableInSafeZoneFireCross = CheckBoxFireCrossInSafeZone.Checked;
        ModValue();
    }

    public void chkSkill41MbAttackPlayObjectClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSkill41MbAttackPlayObject = chkSkill41MbAttackPlayObject.Checked;
        ModValue();
    }

    public void EditMagTammingLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagTammingLevel = (int)EditMagTammingLevel.Value;
        ModValue();
    }

    public void GridBoneFammSetEditText(object? sender, int rowIndex)
    {
        if (!boOpened)
            return;
        WriteRecallRow(M2Config.BoneFammArray, GridBoneFamm, rowIndex);
        ModValue();
    }

    public void GridDogzSetEditText(object? sender, int rowIndex)
    {
        if (!boOpened)
            return;
        WriteRecallRow(M2Config.DogzArray, GridDogz, rowIndex);
        ModValue();
    }

    private static void WriteRecallRow((int nHumLevel, string sMonName, int nLevel, int nCount)[] array, System.Windows.Forms.DataGridView grid, int row)
    {
        if (row < 0 || row >= array.Length)
            return;
        array[row].nHumLevel = GXX.Core.Rtl.DelphiRTL.StrToIntDef(grid.Rows[row].Cells[0].Value?.ToString() ?? "", 0);
        array[row].sMonName = grid.Rows[row].Cells[1].Value?.ToString() ?? "";
        array[row].nCount = GXX.Core.Rtl.DelphiRTL.StrToIntDef(grid.Rows[row].Cells[2].Value?.ToString() ?? "", 0);
        array[row].nLevel = GXX.Core.Rtl.DelphiRTL.StrToIntDef(grid.Rows[row].Cells[3].Value?.ToString() ?? "", 0);
    }

    private static void WriteRecallArrayKeys(GXX.Core.Util.TFastIniFile config, string setupKey, string namesKey,
        (int nHumLevel, string sMonName, int nLevel, int nCount)[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].nHumLevel <= 0)
            {
                config.WriteInteger("Setup", setupKey + "HumLevel" + i, 0);
                config.WriteString("Names", namesKey + i, "");
                config.WriteInteger("Setup", setupKey + "Count" + i, 0);
                config.WriteInteger("Setup", setupKey + "Level" + i, 0);
            }
            else
            {
                config.WriteInteger("Setup", setupKey + "HumLevel" + i, array[i].nHumLevel);
                config.WriteString("Names", namesKey + i, array[i].sMonName);
                config.WriteInteger("Setup", setupKey + "Count" + i, array[i].nCount);
                config.WriteInteger("Setup", setupKey + "Level" + i, array[i].nLevel);
            }
        }
    }

    /// <summary>ButtonSkillSaveClick 头部区段 1:1（名字/数量/数组 + GetMonRace 校验链 + 对应 INI 键；
    /// Skill60..117/CopySelf/SuperShiled 等尾部键组随其余控件接入后补全）。</summary>
    public void ButtonSkillSaveClick(object? sender)
    {
        M2Config.sBoneFamm = EditBoneFammName.Text.Trim();
        M2Config.sDogz = EditDogzName.Text.Trim();

        int monRace = GetMonRaceHandler?.Invoke(M2Config.sBoneFamm) ?? 1;
        if (monRace <= 0)
        {
            M2Forms.MessageBox("怪物名称设置错误！！！\r\n道士技能宝宝名字在数据库中不存在", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditBoneFammName.CanFocus)
                EditBoneFammName.Focus();
            return;
        }
        monRace = GetMonRaceHandler?.Invoke(M2Config.sDogz) ?? 1;
        if (monRace <= 0)
        {
            M2Forms.MessageBox("怪物名称设置错误！！！\r\n道士技能宝宝名字在数据库中不存在", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (EditDogzName.CanFocus)
                EditDogzName.Focus();
            return;
        }

        var Config = M2ShareState.ConfigIni;
        WriteRecallArrayKeys(Config, "BoneFamm", "BoneFamm", M2Config.BoneFammArray);
        WriteRecallArrayKeys(Config, "Dogz", "Dogz", M2Config.DogzArray);
        WriteRecallArrayKeys(Config, "BigDogz", "BigDogz", M2Config.BigDogzArray);
        WriteRecallArrayKeys(Config, "MonthSpirit", "MonthSpirit", M2Config.MonthSpiritArray);

        Config.WriteBool("Setup", "LimitSwordLong", M2Config.boLimitSwordLong);
        Config.WriteInteger("Setup", "SwordLongPowerRate", M2Config.nSwordLongPowerRate);
        Config.WriteInteger("Setup", "SkillYedoPowerRate", M2Config.nSkillYedoPowerRate);
        Config.WriteInteger("Setup", "BoneFammCount", M2Config.nBoneFammCount);
        Config.WriteString("Names", "BoneFamm", M2Config.sBoneFamm);
        Config.WriteInteger("Setup", "DogzCount", M2Config.nDogzCount);
        Config.WriteString("Names", "Dogz", M2Config.sDogz);
        Config.WriteInteger("Setup", "BigDogzCount", M2Config.nBigDogzCount);
        Config.WriteString("Names", "BigDogz", M2Config.sBigDogz);
        Config.WriteString("Names", "MonthSpirit", M2Config.sMonthSpirit);
        Config.WriteInteger("Setup", "MonthSpiritCount", M2Config.nMonthSpiritCount);
        Config.WriteInteger("Setup", "MonthSpiritAttackRange", M2Config.nMonthSpiritAttackRange);
        Config.WriteInteger("Setup", "FireBoomRage", M2Config.nFireBoomRage);
        Config.WriteInteger("Setup", "FireBoomRagePowerRate", M2Config.nFireBoomRagePowerRate);
        Config.WriteInteger("Setup", "SnowWindRange", M2Config.nSnowWindRange);
        Config.WriteInteger("Setup", "SnowWindPowerRate", M2Config.nSnowWindPowerRate);
        Config.WriteInteger("Setup", "SnowwindWaitTime", M2Config.nSnowwindWaitTime);
        Config.WriteInteger("Setup", "ElecBlizzardRange", M2Config.nElecBlizzardRange);
        Config.WriteInteger("Setup", "ElecBlizzardPowerRate", M2Config.nElecBlizzardPowerRate);
        Config.WriteInteger("Setup", "AmyOunsulPoint", M2Config.nAmyOunsulPoint);
        Config.WriteInteger("Setup", "AmyOunsulTimeRate", M2Config.nAmyOunsulTimeRate);
        Config.WriteInteger("Setup", "AmyOunsulMaxTime", M2Config.nAmyOunsulMaxTime);
        Config.WriteBool("Setup", "ShowYouPoisoned", M2Config.boShowYouPoisoned);
        Config.WriteInteger("Setup", "MagicValidTimeRate", M2Config.nMagDelayTimeDoubly);
        Config.WriteInteger("Setup", "NearAttackPowerRate", M2Config.nNearAttackPowerRate);
        Config.WriteBool("Setup", "DisableInSafeZoneFireCross", M2Config.boDisableInSafeZoneFireCross);
        Config.WriteBool("Setup", "Skill41MbAttackPlayObject", M2Config.boSkill41MbAttackPlayObject);
        uModValue();
    }

    // ================= UpgradeWeapon 升级武器页（批次J26） =================

    public void ScrollBarUpgradeWeaponDCRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarUpgradeWeaponDCRate.Value;
        EditUpgradeWeaponDCRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponDCRate = nPostion;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponDCTwoPointRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarUpgradeWeaponDCTwoPointRate.Value;
        EditUpgradeWeaponDCTwoPointRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponDCTwoPointRate = nPostion;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponDCThreePointRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarUpgradeWeaponDCThreePointRate.Value;
        EditUpgradeWeaponDCThreePointRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponDCThreePointRate = nPostion;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponMCRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponMCRate = (int)ScrollBarUpgradeWeaponMCRate.Value;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponMCTwoPointRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponMCTwoPointRate = (int)ScrollBarUpgradeWeaponMCTwoPointRate.Value;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponMCThreePointRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponMCThreePointRate = (int)ScrollBarUpgradeWeaponMCThreePointRate.Value;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponSCRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponSCRate = (int)ScrollBarUpgradeWeaponSCRate.Value;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponSCTwoPointRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponSCTwoPointRate = (int)ScrollBarUpgradeWeaponSCTwoPointRate.Value;
        ModValue();
    }

    public void ScrollBarUpgradeWeaponSCThreePointRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponSCThreePointRate = (int)ScrollBarUpgradeWeaponSCThreePointRate.Value;
        ModValue();
    }

    public void EditUpgradeWeaponMaxPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponMaxPoint = (int)EditUpgradeWeaponMaxPoint.Value;
        ModValue();
    }

    public void EditUpgradeWeaponPriceChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nUpgradeWeaponPrice = (int)EditUpgradeWeaponPrice.Value;
        ModValue();
    }

    public void EditUPgradeWeaponGetBackTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwUPgradeWeaponGetBackTime = (uint)EditUPgradeWeaponGetBackTime.Value * 1000;
        ModValue();
    }

    public void EditClearExpireUpgradeWeaponDaysChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nClearExpireUpgradeWeaponDays = (int)EditClearExpireUpgradeWeaponDays.Value;
        ModValue();
    }

    public void CheckBoxWeaponUpgradeFailNotDeleteClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boWeaponUpgradeFailNotDelete = CheckBoxWeaponUpgradeFailNotDelete.Checked;
        ModValue();
    }

    public void ButtonUpgradeWeaponSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "UpgradeWeaponMaxPoint", M2Config.nUpgradeWeaponMaxPoint);
        Config.WriteInteger("Setup", "UpgradeWeaponPrice", M2Config.nUpgradeWeaponPrice);
        Config.WriteInteger("Setup", "ClearExpireUpgradeWeaponDays", M2Config.nClearExpireUpgradeWeaponDays);
        Config.WriteInteger("Setup", "UPgradeWeaponGetBackTime", (int)M2Config.dwUPgradeWeaponGetBackTime);
        Config.WriteInteger("Setup", "UpgradeWeaponDCRate", M2Config.nUpgradeWeaponDCRate);
        Config.WriteInteger("Setup", "UpgradeWeaponDCTwoPointRate", M2Config.nUpgradeWeaponDCTwoPointRate);
        Config.WriteInteger("Setup", "UpgradeWeaponDCThreePointRate", M2Config.nUpgradeWeaponDCThreePointRate);
        Config.WriteInteger("Setup", "UpgradeWeaponMCRate", M2Config.nUpgradeWeaponMCRate);
        Config.WriteInteger("Setup", "UpgradeWeaponMCTwoPointRate", M2Config.nUpgradeWeaponMCTwoPointRate);
        Config.WriteInteger("Setup", "UpgradeWeaponMCThreePointRate", M2Config.nUpgradeWeaponMCThreePointRate);
        Config.WriteInteger("Setup", "UpgradeWeaponSCRate", M2Config.nUpgradeWeaponSCRate);
        Config.WriteInteger("Setup", "UpgradeWeaponSCTwoPointRate", M2Config.nUpgradeWeaponSCTwoPointRate);
        Config.WriteInteger("Setup", "UpgradeWeaponSCThreePointRate", M2Config.nUpgradeWeaponSCThreePointRate);
        Config.WriteBool("Setup", "WeaponUpgradeFailNotDelete", M2Config.boWeaponUpgradeFailNotDelete);
        uModValue();
    }

    public void ButtonUpgradeWeaponDefaulfClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2Config.nUpgradeWeaponMaxPoint = 20;
        M2Config.nUpgradeWeaponPrice = 10000;
        M2Config.nClearExpireUpgradeWeaponDays = 8;
        M2Config.dwUPgradeWeaponGetBackTime = 60 * 60 * 1000;
        M2Config.nUpgradeWeaponDCRate = 100;
        M2Config.nUpgradeWeaponDCTwoPointRate = 30;
        M2Config.nUpgradeWeaponDCThreePointRate = 200;
        M2Config.nUpgradeWeaponMCRate = 100;
        M2Config.nUpgradeWeaponMCTwoPointRate = 30;
        M2Config.nUpgradeWeaponMCThreePointRate = 200;
        M2Config.nUpgradeWeaponSCRate = 100;
        M2Config.nUpgradeWeaponSCTwoPointRate = 30;
        M2Config.nUpgradeWeaponSCThreePointRate = 200;
        M2Config.boWeaponUpgradeFailNotDelete = false;
        RefUpgradeWeapon();
        ModValue(); // Delphi 由 Position 赋值触发 OnChange 等效产生 ModValue（WinForms 同值赋值不触发）
    }

    // ================= Master 拜师页（批次J27） =================

    public void EditMasterOKLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterOKLevel = (int)EditMasterOKLevel.Value;
        ModValue();
    }

    public void EditMasterOKCreditPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterOKCreditPoint = (int)EditMasterOKCreditPoint.Value;
        ModValue();
    }

    public void EditMasterOKBonusPointChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterOKBonusPoint = (int)EditMasterOKBonusPoint.Value;
        ModValue();
    }

    public void ButtonMasterSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MasterOKLevel", M2Config.nMasterOKLevel);
        Config.WriteInteger("Setup", "MasterOKCreditPoint", M2Config.nMasterOKCreditPoint);
        Config.WriteInteger("Setup", "MasterOKBonusPoint", M2Config.nMasterOKBonusPoint);
        Config.WriteInteger("Setup", "MasterCount", M2Config.nMasterCount);
        uModValue();
    }

    // ================= MakeMine 挖矿页（批次J27） =================

    private void WriteMineField(System.Windows.Forms.NumericUpDown edit, Action<int> set)
    {
        if (!boOpened)
            return;
        set((int)edit.Value);
        ModValue();
    }

    public void EditMakeMineHitRateChange(object? sender) => WriteMineField(EditMakeMineHitRate, v => M2Config.nMakeMineHitRate = v);
    public void EditMakeMineRateChange(object? sender) => WriteMineField(EditMakeMineRate, v => M2Config.nMakeMineRate = v);
    public void EditStoneTypeRateChange(object? sender) => WriteMineField(EditStoneTypeRate, v => M2Config.nStoneTypeRate = v);
    public void EditStoneTypeRateMinChange(object? sender) => WriteMineField(EditStoneTypeRateMin, v => M2Config.nStoneTypeRateMin = v);
    public void EditGoldStoneMinChange(object? sender) => WriteMineField(EditGoldStoneMin, v => M2Config.nGoldStoneMin = v);
    public void EditGoldStoneMaxChange(object? sender) => WriteMineField(EditGoldStoneMax, v => M2Config.nGoldStoneMax = v);
    public void EditSilverStoneMinChange(object? sender) => WriteMineField(EditSilverStoneMin, v => M2Config.nSilverStoneMin = v);
    public void EditSilverStoneMaxChange(object? sender) => WriteMineField(EditSilverStoneMax, v => M2Config.nSilverStoneMax = v);
    public void EditSteelStoneMinChange(object? sender) => WriteMineField(EditSteelStoneMin, v => M2Config.nSteelStoneMin = v);
    public void EditSteelStoneMaxChange(object? sender) => WriteMineField(EditSteelStoneMax, v => M2Config.nSteelStoneMax = v);
    public void EditBlackStoneMinChange(object? sender) => WriteMineField(EditBlackStoneMin, v => M2Config.nBlackStoneMin = v);
    public void EditBlackStoneMaxChange(object? sender) => WriteMineField(EditBlackStoneMax, v => M2Config.nBlackStoneMax = v);
    public void EditStoneMinDuraChange(object? sender) => WriteMineField(EditStoneMinDura, v => M2Config.nStoneMinDura = v);
    public void EditStoneGeneralDuraRateChange(object? sender) => WriteMineField(EditStoneGeneralDuraRate, v => M2Config.nStoneGeneralDuraRate = v);
    public void EditStoneAddDuraRateChange(object? sender) => WriteMineField(EditStoneAddDuraRate, v => M2Config.nStoneAddDuraRate = v);
    public void EditStoneAddDuraMaxChange(object? sender) => WriteMineField(EditStoneAddDuraMax, v => M2Config.nStoneAddDuraMax = v);

    public void ButtonMakeMineSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MakeMineHitRate", M2Config.nMakeMineHitRate);
        Config.WriteInteger("Setup", "MakeMineRate", M2Config.nMakeMineRate);
        Config.WriteInteger("Setup", "StoneTypeRate", M2Config.nStoneTypeRate);
        Config.WriteInteger("Setup", "StoneTypeRateMin", M2Config.nStoneTypeRateMin);
        Config.WriteInteger("Setup", "GoldStoneMin", M2Config.nGoldStoneMin);
        Config.WriteInteger("Setup", "GoldStoneMax", M2Config.nGoldStoneMax);
        Config.WriteInteger("Setup", "SilverStoneMin", M2Config.nSilverStoneMin);
        Config.WriteInteger("Setup", "SilverStoneMax", M2Config.nSilverStoneMax);
        Config.WriteInteger("Setup", "SteelStoneMin", M2Config.nSteelStoneMin);
        Config.WriteInteger("Setup", "SteelStoneMax", M2Config.nSteelStoneMax);
        Config.WriteInteger("Setup", "BlackStoneMin", M2Config.nBlackStoneMin);
        Config.WriteInteger("Setup", "BlackStoneMax", M2Config.nBlackStoneMax);
        Config.WriteInteger("Setup", "StoneMinDura", M2Config.nStoneMinDura);
        Config.WriteInteger("Setup", "StoneGeneralDuraRate", M2Config.nStoneGeneralDuraRate);
        Config.WriteInteger("Setup", "StoneAddDuraRate", M2Config.nStoneAddDuraRate);
        Config.WriteInteger("Setup", "StoneAddDuraMax", M2Config.nStoneAddDuraMax);
        uModValue();
    }

    // ================= WinLottery 赌博页（批次J27） =================

    public void EditWinLottery1GoldChange(object? sender) => WriteMineField(EditWinLottery1Gold, v => M2Config.nWinLottery1Gold = v);
    public void EditWinLottery2GoldChange(object? sender) => WriteMineField(EditWinLottery2Gold, v => M2Config.nWinLottery2Gold = v);
    public void EditWinLottery3GoldChange(object? sender) => WriteMineField(EditWinLottery3Gold, v => M2Config.nWinLottery3Gold = v);
    public void EditWinLottery4GoldChange(object? sender) => WriteMineField(EditWinLottery4Gold, v => M2Config.nWinLottery4Gold = v);
    public void EditWinLottery5GoldChange(object? sender) => WriteMineField(EditWinLottery5Gold, v => M2Config.nWinLottery5Gold = v);
    public void EditWinLottery6GoldChange(object? sender) => WriteMineField(EditWinLottery6Gold, v => M2Config.nWinLottery6Gold = v);

    public void ScrollBarWinLotteryRateChange(object? sender)
    {
        int nPostion = (int)ScrollBarWinLotteryRate.Value;
        EditWinLotteryRate.Text = nPostion.ToString();
        if (!boOpened)
            return;
        ScrollBarWinLottery1Max.Maximum = nPostion;
        M2Config.nWinLotteryRate = nPostion;
        ModValue();
    }

    public void ScrollBarWinLottery1MaxChange(object? sender)
    {
        int nPostion = (int)ScrollBarWinLottery1Max.Value;
        EditWinLottery1Max.Text = M2Config.nWinLottery1Min + "-" + M2Config.nWinLottery1Max;
        if (!boOpened)
            return;
        M2Config.nWinLottery1Max = nPostion;
        ScrollBarWinLottery2Max.Maximum = nPostion - 1;
        ScrollBarWinLotteryRate.Minimum = Math.Min(nPostion, ScrollBarWinLotteryRate.Maximum);
        EditWinLottery1Max.Text = M2Config.nWinLottery1Min + "-" + M2Config.nWinLottery1Max;
        ModValue();
    }

    public void ScrollBarWinLottery2MaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWinLottery2Max = (int)ScrollBarWinLottery2Max.Value;
        ModValue();
    }

    public void ScrollBarWinLottery3MaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWinLottery3Max = (int)ScrollBarWinLottery3Max.Value;
        ModValue();
    }

    public void ScrollBarWinLottery4MaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWinLottery4Max = (int)ScrollBarWinLottery4Max.Value;
        ModValue();
    }

    public void ScrollBarWinLottery5MaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWinLottery5Max = (int)ScrollBarWinLottery5Max.Value;
        ModValue();
    }

    public void ScrollBarWinLottery6MaxChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWinLottery6Max = (int)ScrollBarWinLottery6Max.Value;
        ModValue();
    }

    public void ButtonWinLotterySaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "WinLottery1Gold", M2Config.nWinLottery1Gold);
        Config.WriteInteger("Setup", "WinLottery2Gold", M2Config.nWinLottery2Gold);
        Config.WriteInteger("Setup", "WinLottery3Gold", M2Config.nWinLottery3Gold);
        Config.WriteInteger("Setup", "WinLottery4Gold", M2Config.nWinLottery4Gold);
        Config.WriteInteger("Setup", "WinLottery5Gold", M2Config.nWinLottery5Gold);
        Config.WriteInteger("Setup", "WinLottery6Gold", M2Config.nWinLottery6Gold);
        Config.WriteInteger("Setup", "WinLottery1Min", M2Config.nWinLottery1Min);
        Config.WriteInteger("Setup", "WinLottery1Max", M2Config.nWinLottery1Max);
        Config.WriteInteger("Setup", "WinLottery2Min", M2Config.nWinLottery2Min);
        Config.WriteInteger("Setup", "WinLottery2Max", M2Config.nWinLottery2Max);
        Config.WriteInteger("Setup", "WinLottery3Min", M2Config.nWinLottery3Min);
        Config.WriteInteger("Setup", "WinLottery3Max", M2Config.nWinLottery3Max);
        Config.WriteInteger("Setup", "WinLottery4Min", M2Config.nWinLottery4Min);
        Config.WriteInteger("Setup", "WinLottery4Max", M2Config.nWinLottery4Max);
        Config.WriteInteger("Setup", "WinLottery5Min", M2Config.nWinLottery5Min);
        Config.WriteInteger("Setup", "WinLottery5Max", M2Config.nWinLottery5Max);
        Config.WriteInteger("Setup", "WinLottery6Min", M2Config.nWinLottery6Min);
        Config.WriteInteger("Setup", "WinLottery6Max", M2Config.nWinLottery6Max);
        Config.WriteInteger("Setup", "WinLotteryRate", M2Config.nWinLotteryRate);
        uModValue();
    }

    public void ButtonWinLotteryDefaulfClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2Config.nWinLottery1Gold = 1000000;
        M2Config.nWinLottery2Gold = 200000;
        M2Config.nWinLottery3Gold = 100000;
        M2Config.nWinLottery4Gold = 10000;
        M2Config.nWinLottery5Gold = 1000;
        M2Config.nWinLottery6Gold = 500;
        M2Config.nWinLottery6Min = 1;
        M2Config.nWinLottery6Max = 4999;
        M2Config.nWinLottery5Min = 14000;
        M2Config.nWinLottery5Max = 15999;
        M2Config.nWinLottery4Min = 16000;
        M2Config.nWinLottery4Max = 16149;
        M2Config.nWinLottery3Min = 16150;
        M2Config.nWinLottery3Max = 16169;
        M2Config.nWinLottery2Min = 16170;
        M2Config.nWinLottery2Max = 16179;
        M2Config.nWinLottery1Min = 16180;
        M2Config.nWinLottery1Max = 16185;
        M2Config.nWinLotteryRate = 30000;
        RefMasterMineLottery();
    }

    // ================= ReNewLevel 转生页（批次J28） =================

    public void EditReNewNameColorNChange(int index)
    {
        if (!boOpened)
            return;
        M2Config.ReNewNameColor[index] = (byte)EditReNewNameColorCells[index].Value;
        ModValue();
    }

    // Delphi 逐控件处理器 1:1（EditReNewNameColor1..10Change 均转调本泛化实现）
    public void EditReNewNameColor1Change(object? sender) => EditReNewNameColorNChange(0);
    public void EditReNewNameColor2Change(object? sender) => EditReNewNameColorNChange(1);
    public void EditReNewNameColor3Change(object? sender) => EditReNewNameColorNChange(2);
    public void EditReNewNameColor4Change(object? sender) => EditReNewNameColorNChange(3);
    public void EditReNewNameColor5Change(object? sender) => EditReNewNameColorNChange(4);
    public void EditReNewNameColor6Change(object? sender) => EditReNewNameColorNChange(5);
    public void EditReNewNameColor7Change(object? sender) => EditReNewNameColorNChange(6);
    public void EditReNewNameColor8Change(object? sender) => EditReNewNameColorNChange(7);
    public void EditReNewNameColor9Change(object? sender) => EditReNewNameColorNChange(8);
    public void EditReNewNameColor10Change(object? sender) => EditReNewNameColorNChange(9);

    public void EditReNewNameColorTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwReNewNameColorTime = (uint)EditReNewNameColorTime.Value * 1000;
        ModValue();
    }

    public void CheckBoxReNewChangeColorClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boReNewChangeColor = CheckBoxReNewChangeColor.Checked;
        ModValue();
    }

    public void CheckBoxReNewLevelClearExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boReNewLevelClearExp = CheckBoxReNewLevelClearExp.Checked;
        ModValue();
    }

    public void ButtonReNewLevelSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        for (int i = 0; i < M2Config.ReNewNameColor.Length; i++)
            Config.WriteInteger("Setup", "ReNewNameColor" + i, M2Config.ReNewNameColor[i]);
        Config.WriteInteger("Setup", "ReNewNameColorTime", (int)M2Config.dwReNewNameColorTime);
        Config.WriteBool("Setup", "ReNewChangeColor", M2Config.boReNewChangeColor);
        Config.WriteBool("Setup", "ReNewLevelClearExp", M2Config.boReNewLevelClearExp);
        uModValue();
    }

    // ================= MonUpgrade 宝宝升级页（批次J28） =================

    public void EditMonUpgradeColorNChange(int index)
    {
        if (!boOpened)
            return;
        M2Config.SlaveColor[index] = (byte)EditMonUpgradeColorCells[index].Value;
        ModValue();
    }

    public void EditMonUpgradeColor0Change(object? sender) => EditMonUpgradeColorNChange(0);
    public void EditMonUpgradeColor1Change(object? sender) => EditMonUpgradeColorNChange(1);
    public void EditMonUpgradeColor2Change(object? sender) => EditMonUpgradeColorNChange(2);
    public void EditMonUpgradeColor3Change(object? sender) => EditMonUpgradeColorNChange(3);
    public void EditMonUpgradeColor4Change(object? sender) => EditMonUpgradeColorNChange(4);
    public void EditMonUpgradeColor5Change(object? sender) => EditMonUpgradeColorNChange(5);
    public void EditMonUpgradeColor6Change(object? sender) => EditMonUpgradeColorNChange(6);
    public void EditMonUpgradeColor7Change(object? sender) => EditMonUpgradeColorNChange(7);
    public void EditMonUpgradeColor8Change(object? sender) => EditMonUpgradeColorNChange(8);
    public void EditMonUpgradeColor9Change(object? sender) => EditMonUpgradeColorNChange(9);

    public void EditMonUpgradeKillCountNChange(int index)
    {
        if (!boOpened)
            return;
        M2Config.MonUpLvNeedKillCount[index] = (int)EditMonUpgradeKillCountCells[index].Value;
        ModValue();
    }

    public void EditMonUpgradeKillCount1Change(object? sender) => EditMonUpgradeKillCountNChange(0);
    public void EditMonUpgradeKillCount2Change(object? sender) => EditMonUpgradeKillCountNChange(1);
    public void EditMonUpgradeKillCount3Change(object? sender) => EditMonUpgradeKillCountNChange(2);
    public void EditMonUpgradeKillCount4Change(object? sender) => EditMonUpgradeKillCountNChange(3);
    public void EditMonUpgradeKillCount5Change(object? sender) => EditMonUpgradeKillCountNChange(4);
    public void EditMonUpgradeKillCount6Change(object? sender) => EditMonUpgradeKillCountNChange(5);
    public void EditMonUpgradeKillCount7Change(object? sender) => EditMonUpgradeKillCountNChange(6);

    public void EditMonUpLvNeedKillBaseChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMonUpLvNeedKillBase = (int)EditMonUpLvNeedKillBase.Value;
        ModValue();
    }

    public void EditMonUpLvRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMonUpLvRate = (int)EditMonUpLvRate.Value;
        ModValue();
    }

    public void CheckBoxMasterDieMutinyClick(object? sender)
    {
        if (CheckBoxMasterDieMutiny.Checked)
        {
            EditMasterDieMutinyRate.Enabled = true;
            EditMasterDieMutinyPower.Enabled = true;
            EditMasterDieMutinySpeed.Enabled = true;
        }
        else
        {
            EditMasterDieMutinyRate.Enabled = false;
            EditMasterDieMutinyPower.Enabled = false;
            EditMasterDieMutinySpeed.Enabled = false;
        }
        if (!boOpened)
            return;
        M2Config.boMasterDieMutiny = CheckBoxMasterDieMutiny.Checked;
        ModValue();
    }

    public void EditMasterDieMutinyRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterDieMutinyRate = (int)EditMasterDieMutinyRate.Value;
        ModValue();
    }

    public void EditMasterDieMutinyPowerChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterDieMutinyPower = (int)EditMasterDieMutinyPower.Value;
        ModValue();
    }

    public void EditMasterDieMutinySpeedChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMasterDieMutinySpeed = (int)EditMasterDieMutinySpeed.Value;
        ModValue();
    }

    public void CheckBoxBBMonAutoChangeColorClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boBBMonAutoChangeColor = CheckBoxBBMonAutoChangeColor.Checked;
        ModValue();
    }

    public void EditBBMonAutoChangeColorTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwBBMonAutoChangeColorTime = (uint)EditBBMonAutoChangeColorTime.Value * 1000;
        ModValue();
    }

    public void ButtonMonUpgradeSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MonUpLvNeedKillBase", M2Config.nMonUpLvNeedKillBase);
        Config.WriteInteger("Setup", "MonUpLvRate", M2Config.nMonUpLvRate);
        for (int i = 0; i < M2Config.MonUpLvNeedKillCount.Length; i++)
            Config.WriteInteger("Setup", "MonUpLvNeedKillCount" + i, M2Config.MonUpLvNeedKillCount[i]);
        for (int i = 0; i < M2Config.SlaveColor.Length; i++)
            Config.WriteInteger("Setup", "SlaveColor" + i, M2Config.SlaveColor[i]);
        Config.WriteBool("Setup", "MasterDieMutiny", M2Config.boMasterDieMutiny);
        Config.WriteInteger("Setup", "MasterDieMutinyRate", M2Config.nMasterDieMutinyRate);
        Config.WriteInteger("Setup", "MasterDieMutinyPower", M2Config.nMasterDieMutinyPower);
        Config.WriteInteger("Setup", "MasterDieMutinyPower", M2Config.nMasterDieMutinySpeed); // Delphi 原文重复键名取 Speed 值，保留
        Config.WriteBool("Setup", "BBMonAutoChangeColor", M2Config.boBBMonAutoChangeColor);
        Config.WriteInteger("Setup", "BBMonAutoChangeColorTime", (int)M2Config.dwBBMonAutoChangeColorTime);
        Config.WriteInteger("Setup", "SlavePowerRate", M2Config.nSlavePowerRate);
        Config.WriteBool("Setup", "MasterRoyaltyDie", M2Config.boMasterRoyaltyDie);
        Config.WriteBool("Setup", "MasterRoyaltyFullHP", M2Config.boMasterRoyaltyFullHP);
        Config.WriteBool("Setup", "SlaveRelaxCanStruck", M2Config.boSlaveRelaxCanStruck);
        Config.WriteBool("Setup", "SlaveNotAttackHuman", M2Config.boSlaveNotAttackHuman);
        Config.WriteBool("Setup", "SlaveNotAttackHero", M2Config.boSlaveNotAttackHero);
        Config.WriteBool("Setup", "SlaveLockTarget", M2Config.boSlaveLockTarget);
        Config.WriteBool("Setup", "SlaveNoLockHuman", M2Config.boSlaveNoLockHuman);
        Config.WriteInteger("Setup", "Slave9HP", M2Config.nSlave9HP);
        Config.WriteInteger("Setup", "Slave9AC", M2Config.nSlave9AC);
        Config.WriteInteger("Setup", "Slave9MAC", M2Config.nSlave9MAC);
        Config.WriteInteger("Setup", "Slave9DC", M2Config.nSlave9DC);
        Config.WriteInteger("Setup", "Slave9MoveSpeed", M2Config.nSlave9MoveSpeed);
        uModValue();
    }

    // ================= SpiritMutiny 叛变页（批次J29） =================

    public void CheckBoxSpiritMutinyClick(object? sender)
    {
        if (CheckBoxSpiritMutiny.Checked)
        {
            EditSpiritMutinyTime.Enabled = true;
        }
        else
        {
            EditSpiritMutinyTime.Enabled = false;
            EditSpiritPowerRate.Enabled = false;
        }
        if (!boOpened)
            return;
        M2Config.boSpiritMutiny = CheckBoxSpiritMutiny.Checked;
        ModValue();
    }

    public void EditSpiritMutinyTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwSpiritMutinyTime = (uint)EditSpiritMutinyTime.Value * 60 * 1000;
        ModValue();
    }

    public void EditSpiritPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nSpiritPowerRate = (int)EditSpiritPowerRate.Value;
        ModValue();
    }

    public void ButtonSpiritMutinySaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "SpiritMutiny", M2Config.boSpiritMutiny);
        Config.WriteInteger("Setup", "SpiritMutinyTime", (int)M2Config.dwSpiritMutinyTime);
        Config.WriteInteger("Setup", "SpiritPowerRate", M2Config.nSpiritPowerRate);
        uModValue();
    }

    // ================= MonSayMsg 怪物发言页（批次J29） =================

    public void CheckBoxMonSayMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boMonSayMsg = CheckBoxMonSayMsg.Checked;
        ModValue();
    }

    public void ButtonMonSayMsgSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "MonSayMsg", M2Config.boMonSayMsg);
        uModValue();
    }

    // ================= WeaponMakeLuck 武器炼制页（批次J29） =================

    public void ScrollBarWeaponMakeUnLuckRateChange(object? sender)
    {
        int nInteger = (int)ScrollBarWeaponMakeUnLuckRate.Value;
        EditWeaponMakeUnLuckRate.Text = nInteger.ToString();
        if (!boOpened)
            return;
        M2Config.nWeaponMakeUnLuckRate = nInteger;
        ModValue();
    }

    public void ScrollBarWeaponMakeLuckPoint1Change(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWeaponMakeLuckPoint1 = (int)ScrollBarWeaponMakeLuckPoint1.Value;
        ModValue();
    }

    public void ScrollBarWeaponMakeLuckPoint2Change(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWeaponMakeLuckPoint2 = (int)ScrollBarWeaponMakeLuckPoint2.Value;
        ModValue();
    }

    public void ScrollBarWeaponMakeLuckPoint3Change(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWeaponMakeLuckPoint3 = (int)ScrollBarWeaponMakeLuckPoint3.Value;
        ModValue();
    }

    public void ScrollBarWeaponMakeLuckPoint2RateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWeaponMakeLuckPoint2Rate = (int)ScrollBarWeaponMakeLuckPoint2Rate.Value;
        ModValue();
    }

    public void ScrollBarWeaponMakeLuckPoint3RateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nWeaponMakeLuckPoint3Rate = (int)ScrollBarWeaponMakeLuckPoint3Rate.Value;
        ModValue();
    }

    public void ButtonWeaponMakeLuckSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "WeaponMakeUnLuckRate", M2Config.nWeaponMakeUnLuckRate);
        Config.WriteInteger("Setup", "WeaponMakeLuckPoint1", M2Config.nWeaponMakeLuckPoint1);
        Config.WriteInteger("Setup", "WeaponMakeLuckPoint2", M2Config.nWeaponMakeLuckPoint2);
        Config.WriteInteger("Setup", "WeaponMakeLuckPoint3", M2Config.nWeaponMakeLuckPoint3);
        Config.WriteInteger("Setup", "WeaponMakeLuckPoint2Rate", M2Config.nWeaponMakeLuckPoint2Rate);
        Config.WriteInteger("Setup", "WeaponMakeLuckPoint3Rate", M2Config.nWeaponMakeLuckPoint3Rate);
        uModValue();
    }

    public void ButtonWeaponMakeLuckDefaulfClick(object? sender)
    {
        if (M2Forms.MessageBox("是否确认恢复默认设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) != M2Forms.IDYES)
        {
            return;
        }
        M2Config.nWeaponMakeUnLuckRate = 20;
        M2Config.nWeaponMakeLuckPoint1 = 1;
        M2Config.nWeaponMakeLuckPoint2 = 3;
        M2Config.nWeaponMakeLuckPoint3 = 7;
        M2Config.nWeaponMakeLuckPoint2Rate = 6;
        M2Config.nWeaponMakeLuckPoint3Rate = 40;
        RefMutinyMsgLuck();
        ModValue(); // VCL Position 赋值触发 OnChange 的等效 ModValue（WinForms 同值赋值不触发）
    }

    // ================= OffLine 离线页（批次J30） =================

    public void CheckBoxOffLineLoginSafeAreaClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boOffLineLoginSafeArea = CheckBoxOffLineLoginSafeArea.Checked;
        ModValue();
    }

    public void EditOffLineLoginMapNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btOffLineLoginMapName = (byte)EditOffLineLoginMapName.Value;
        ModValue();
    }

    public void EditSetOffLineLoginMapNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sSetOffLineLoginMapName = EditSetOffLineLoginMapName.Text.Trim();
        ModValue();
    }

    public void CheckBoxMonNoAttackOffLinePlayerClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boMonNoAttackOffLinePlayer = CheckBoxMonNoAttackOffLinePlayer.Checked;
        ModValue();
    }

    public void ButtonOffLineSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "OffLineLoginSafeArea", M2Config.boOffLineLoginSafeArea);
        Config.WriteInteger("Setup", "OffLineLoginMapName", M2Config.btOffLineLoginMapName);
        Config.WriteString("Setup", "SetOffLineLoginMapName", M2Config.sSetOffLineLoginMapName);
        Config.WriteBool("Setup", "MonNoAttackOffLinePlayer", M2Config.boMonNoAttackOffLinePlayer);
        uModValue();
    }

    // ================= MyShop 个人商铺页（批次J30） =================

    public void EditMaxMyShopSellingItemCountChange(object? sender) => WriteMineField(EditMaxMyShopSellingItemCount, v => M2Config.nMaxMyShopSellingItemCount = v);
    public void EditMaxMyShopStorageItemCountChange(object? sender) => WriteMineField(EditMaxMyShopStorageItemCount, v => M2Config.nMaxMyShopStorageItemCount = v);
    public void CheckBoxOfflineCloseMyShopClick(object? sender) { if (boOpened) { M2Config.boOfflineCloseMyShop = CheckBoxOfflineCloseMyShop.Checked; ModValue(); } }
    public void CheckBoxProhibitModifyPricesClick(object? sender) { if (boOpened) { M2Config.boProhibitModifyPrices = CheckBoxProhibitModifyPrices.Checked; ModValue(); } }
    public void CheckBoxUseHeroM2ShopClick(object? sender) { if (boOpened) { M2Config.boUseHeroM2Shop = CheckBoxUseHeroM2Shop.Checked; ModValue(); } }
    public void CheckBoxInfinityStorageClick(object? sender) { if (boOpened) { M2Config.boInfinityStorage = CheckBoxInfinityStorage.Checked; ModValue(); } }
    public void EditInfinityStorageCountChange(object? sender) => WriteMineField(EditInfinityStorageCount, v => M2Config.nInfinityStorageCount = v);
    public void CheckBoxMyShopGoldClick(object? sender) { if (boOpened) { M2Config.boMyShopGold = CheckBoxMyShopGold.Checked; ModValue(); } }
    public void CheckBoxMyShopGameGoldClick(object? sender) { if (boOpened) { M2Config.boMyShopGameGold = CheckBoxMyShopGameGold.Checked; ModValue(); } }
    public void CheckBoxMyShopGameDiamondClick(object? sender) { if (boOpened) { M2Config.boMyShopGameDiamond = CheckBoxMyShopGameDiamond.Checked; ModValue(); } }
    public void CheckBoxMyShopGameGirdClick(object? sender) { if (boOpened) { M2Config.boMyShopGameGird = CheckBoxMyShopGameGird.Checked; ModValue(); } }
    public void CheckBoxMyShopGamePointClick(object? sender) { if (boOpened) { M2Config.boMyShopGamePoint = CheckBoxMyShopGamePoint.Checked; ModValue(); } }
    public void CheckBoxOpenSelfShopClick(object? sender) { if (boOpened) { M2Config.boOpenSelfShop = CheckBoxOpenSelfShop.Checked; ModValue(); } }
    public void CheckBoxSafeZoneShopClick(object? sender) { if (boOpened) { M2Config.boSafeZoneShop = CheckBoxSafeZoneShop.Checked; ModValue(); } }
    public void CheckBoxMapShopClick(object? sender) { if (boOpened) { M2Config.boMapShop = CheckBoxMapShop.Checked; ModValue(); } }
    public void CheckBoxShopStallCanNotAttackClick(object? sender) { if (boOpened) { M2Config.boShopStallCanNotAttack = CheckBoxShopStallCanNotAttack.Checked; ModValue(); } }
    public void EditSellOffGoldTaxRateChange(object? sender) => WriteMineField(EditSellOffGoldTaxRate, v => M2Config.dwSellOffGoldTaxRate = (uint)v);
    public void EditSellOffGameGoldTaxRateChange(object? sender) => WriteMineField(EditSellOffGameGoldTaxRate, v => M2Config.dwSellOffGameGoldTaxRate = (uint)v);
    public void EditSellOffGameDiamondTaxRateChange(object? sender) => WriteMineField(EditSellOffGameDiamondTaxRate, v => M2Config.dwSellOffGameDiamondTaxRate = (uint)v);
    public void EditSellOffGameGirdTaxRateChange(object? sender) => WriteMineField(EditSellOffGameGirdTaxRate, v => M2Config.dwSellOffGameGirdTaxRate = (uint)v);
    public void EditSellOffGamePointTaxRateChange(object? sender) => WriteMineField(EditSellOffGamePointTaxRate, v => M2Config.dwSellOffGamePointTaxRate = (uint)v);
    public void CheckBoxShopHeadPicClick(object? sender) { if (boOpened) { M2Config.boShopHeadPic = CheckBoxShopHeadPic.Checked; ModValue(); } }

    public void ButtonMyShopSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MaxMyShopSellingItemCount", M2Config.nMaxMyShopSellingItemCount);
        Config.WriteInteger("Setup", "MaxMyShopStorageItemCount", M2Config.nMaxMyShopStorageItemCount);
        Config.WriteBool("Setup", "OfflineCloseMyShop", M2Config.boOfflineCloseMyShop);
        Config.WriteBool("Setup", "ProhibitModifyPrices", M2Config.boProhibitModifyPrices);
        Config.WriteBool("Setup", "UseHeroM2Shop", M2Config.boUseHeroM2Shop);
        Config.WriteBool("Setup", "InfinityStorage", M2Config.boInfinityStorage);
        Config.WriteInteger("Setup", "InfinityStorageCount", M2Config.nInfinityStorageCount);
        Config.WriteBool("Setup", "MyShopGold", M2Config.boMyShopGold);
        Config.WriteBool("Setup", "MyShopGameGold", M2Config.boMyShopGameGold);
        Config.WriteBool("Setup", "MyShopGameDiamond", M2Config.boMyShopGameDiamond);
        Config.WriteBool("Setup", "MyShopGameGird", M2Config.boMyShopGameGird);
        Config.WriteBool("Setup", "MyShopGamePoint", M2Config.boMyShopGamePoint);
        Config.WriteBool("Setup", "OpenSelfShop", M2Config.boOpenSelfShop);
        Config.WriteBool("Setup", "SafeZoneShop", M2Config.boSafeZoneShop);
        Config.WriteBool("Setup", "MapShop", M2Config.boMapShop);
        Config.WriteBool("Setup", "ShopStallCanNotAttack", M2Config.boShopStallCanNotAttack);
        Config.WriteInteger("Setup", "SellOffGoldTaxRate", (int)M2Config.dwSellOffGoldTaxRate);
        Config.WriteInteger("Setup", "SellOffGameGoldTaxRate", (int)M2Config.dwSellOffGameGoldTaxRate);
        Config.WriteInteger("Setup", "SellOffGameDiamondTaxRate", (int)M2Config.dwSellOffGameDiamondTaxRate);
        Config.WriteInteger("Setup", "SellOffGameGirdTaxRate", (int)M2Config.dwSellOffGameGirdTaxRate);
        Config.WriteInteger("Setup", "SellOffGamePointTaxRate", (int)M2Config.dwSellOffGamePointTaxRate);
        Config.WriteBool("Setup", "ShopHeadPic", M2Config.boShopHeadPic);
        uModValue();
    }

    // ================= HeroOption/Other/BonusAbilof/Other3 页（批次J31） =================

    /// <summary>属性点三职业 27 项控件构建（BonusAbilofWarr/Wizard/Taos × DC/MC/SC/AC/MAC/HP/MP/Hit/Speed）。</summary>
    private void BuildBonusAbilofControls(System.Windows.Forms.TabPage bonusTab)
    {
        var defs = new (string Name, string Cap, TBonusAbil Abil, int Col, int Row)[]
        {
            ("WarrDC", "战士DC:", M2Config.BonusAbilofWarr, 0, 0),
            ("WarrMC", "战士MC:", M2Config.BonusAbilofWarr, 0, 1),
            ("WarrSC", "战士SC:", M2Config.BonusAbilofWarr, 0, 2),
            ("WarrAC", "战士AC:", M2Config.BonusAbilofWarr, 0, 3),
            ("WarrMAC", "战士MAC:", M2Config.BonusAbilofWarr, 0, 4),
            ("WarrHP", "战士HP:", M2Config.BonusAbilofWarr, 0, 5),
            ("WarrMP", "战士MP:", M2Config.BonusAbilofWarr, 0, 6),
            ("WarrHit", "战士准确:", M2Config.BonusAbilofWarr, 0, 7),
            ("WarrSpeed", "战士速度:", M2Config.BonusAbilofWarr, 0, 8),
            ("WizardDC", "法师DC:", M2Config.BonusAbilofWizard, 1, 0),
            ("WizardMC", "法师MC:", M2Config.BonusAbilofWizard, 1, 1),
            ("WizardSC", "法师SC:", M2Config.BonusAbilofWizard, 1, 2),
            ("WizardAC", "法师AC:", M2Config.BonusAbilofWizard, 1, 3),
            ("WizardMAC", "法师MAC:", M2Config.BonusAbilofWizard, 1, 4),
            ("WizardHP", "法师HP:", M2Config.BonusAbilofWizard, 1, 5),
            ("WizardMP", "法师MP:", M2Config.BonusAbilofWizard, 1, 6),
            ("WizardHit", "法师准确:", M2Config.BonusAbilofWizard, 1, 7),
            ("WizardSpeed", "法师速度:", M2Config.BonusAbilofWizard, 1, 8),
            ("TaosDC", "道士DC:", M2Config.BonusAbilofTaos, 2, 0),
            ("TaosMC", "道士MC:", M2Config.BonusAbilofTaos, 2, 1),
            ("TaosSC", "道士SC:", M2Config.BonusAbilofTaos, 2, 2),
            ("TaosAC", "道士AC:", M2Config.BonusAbilofTaos, 2, 3),
            ("TaosMAC", "道士MAC:", M2Config.BonusAbilofTaos, 2, 4),
            ("TaosHP", "道士HP:", M2Config.BonusAbilofTaos, 2, 5),
            ("TaosMP", "道士MP:", M2Config.BonusAbilofTaos, 2, 6),
            ("TaosHit", "道士准确:", M2Config.BonusAbilofTaos, 2, 7),
            ("TaosSpeed", "道士速度:", M2Config.BonusAbilofTaos, 2, 8),
        };
        foreach (var (name, cap, abil, col, row) in defs)
        {
            var abilLocal = abil;
            var spin = new System.Windows.Forms.NumericUpDown { Left = 20 + col * 220, Top = 20 + row * 34, Width = 90, Minimum = -2000000000, Maximum = 2000000000 };
            spin.Value = BonusReadValue(abilLocal, name);
            spin.ValueChanged += (s, e) =>
            {
                if (!boOpened)
                    return;
                BonusWriteValue(abilLocal, name, (int)spin.Value);
                ModValue();
            };
            bonusTab.Controls.Add(new System.Windows.Forms.Label { Text = cap, Left = spin.Left - 90, Top = spin.Top + 4, AutoSize = true });
            bonusTab.Controls.Add(spin);
            BonusAbilofSpins[name] = spin;
        }
        EditBonusAbilofWarrDC = BonusAbilofSpins["WarrDC"];
        EditBonusAbilofTaosSpeed = BonusAbilofSpins["TaosSpeed"];
    }

    private static int BonusReadValue(TBonusAbil abil, string name) => name switch
    {
        "WarrDC" or "WizardDC" or "TaosDC" => abil.DC,
        "WarrMC" or "WizardMC" or "TaosMC" => abil.MC,
        "WarrSC" or "WizardSC" or "TaosSC" => abil.SC,
        "WarrAC" or "WizardAC" or "TaosAC" => abil.AC,
        "WarrMAC" or "WizardMAC" or "TaosMAC" => abil.MAC,
        "WarrHP" or "WizardHP" or "TaosHP" => abil.HP,
        "WarrMP" or "WizardMP" or "TaosMP" => abil.MP,
        "WarrHit" or "WizardHit" or "TaosHit" => abil.Hit,
        _ => abil.Speed,
    };

    private static void BonusWriteValue(TBonusAbil abil, string name, int v)
    {
        switch (name)
        {
            case "WarrDC" or "WizardDC" or "TaosDC": abil.DC = v; break;
            case "WarrMC" or "WizardMC" or "TaosMC": abil.MC = v; break;
            case "WarrSC" or "WizardSC" or "TaosSC": abil.SC = v; break;
            case "WarrAC" or "WizardAC" or "TaosAC": abil.AC = v; break;
            case "WarrMAC" or "WizardMAC" or "TaosMAC": abil.MAC = v; break;
            case "WarrHP" or "WizardHP" or "TaosHP": abil.HP = v; break;
            case "WarrMP" or "WizardMP" or "TaosMP": abil.MP = v; break;
            case "WarrHit" or "WizardHit" or "TaosHit": abil.Hit = v; break;
            default: abil.Speed = v; break;
        }
    }

    public void CheckBoxHeroGetAllExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHeroGetAllExp = CheckBoxHeroGetAllExp.Checked;
        ModValue();
    }

    public void CheckBoxHumanGetAllExpClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boHumanGetAllExp = CheckBoxHumanGetAllExp.Checked;
        ModValue();
    }

    public void EditHeroKillMonExpRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nHeroKillMonExpRate = (int)EditHeroKillMonExpRate.Value;
        ModValue();
    }

    public void CheckBoxAllowCopySelfClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boAllowCopySelf = CheckBoxAllowCopySelf.Checked;
        ModValue();
    }

    public void EditLimitExpLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nLimitExpLevel = (int)EditLimitExpLevel.Value;
        ModValue();
    }

    public void EditRecallHeroTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nRecallHeroTime = (int)EditRecallHeroTime.Value;
        ModValue();
    }

    public void EditHeroWarrorAttackTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwHeroWarrorAttackTime = (uint)EditHeroWarrorAttackTime.Value;
        ModValue();
    }

    /// <summary>ButtonHeroOptionSaveClick 1:1（网格逐行 1..High(LongWord) 校验（三个感叹号）、HeroExp 节
    /// Level1..1000 写 Exps.ini、双获取经验开关、AllowCopySelf 一值写三键、英雄六时间写人物键名原文瑕疵）。</summary>
    public void ButtonHeroOptionSaveClick(object? sender)
    {
        var needExps = new uint[1001];
        for (int i = 1; i <= 1000; i++)
        {
            var cell = GridHeroExp.Rows[i - 1].Cells[1].Value?.ToString() ?? "";
            long dwExp = GXX.Core.Rtl.DelphiRTL.StrToInt64Def(cell, 0);
            if (dwExp <= 0 || dwExp > uint.MaxValue)
            {
                M2Forms.MessageBox($"等级 {i} 升级经验设置错误！！！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
                if (GridHeroExp.IsHandleCreated)
                    GridHeroExp.CurrentCell = GridHeroExp.Rows[i - 1].Cells[1];
                return;
            }
            needExps[i] = (uint)dwExp;
        }
        Array.Copy(needExps, M2Config.dwHeroNeedExps, 1001);

        var Config = M2ShareState.ConfigIni;
        var ExpConfig = M2ShareState.ExpConfigIni;
        for (int i = 1; i <= 1000; i++)
            ExpConfig.WriteString("HeroExp", "Level" + i, M2Config.dwHeroNeedExps[i].ToString());
        Config.WriteBool("Setup", "HeroGetAllExp", M2Config.boHeroGetAllExp);
        Config.WriteBool("Setup", "HumanGetAllExp", M2Config.boHumanGetAllExp);
        Config.WriteInteger("Setup", "HeroKillMonExpRate", M2Config.nHeroKillMonExpRate);
        Config.WriteInteger("Setup", "HeroNotKillMonExpRate", M2Config.nHeroNotKillMonExpRate);
        Config.WriteBool("Setup", "AllowCopySelf", M2Config.boAllowCopySelf);
        Config.WriteBool("Setup", "HeroUseBagItem", M2Config.boAllowCopySelf); // Delphi 原文三键同值
        Config.WriteBool("Setup", "MonUseBagItem", M2Config.boAllowCopySelf);
        Config.WriteInteger("Setup", "LimitExpLevel", M2Config.nLimitExpLevel);
        Config.WriteInteger("Setup", "LimitExpValue", M2Config.nLimitExpValue);
        // 原文瑕疵：英雄攻/走间隔写人物键名（WarrorAttackTime 等）取 Hero 值
        Config.WriteInteger("Setup", "WarrorAttackTime", (int)M2Config.dwHeroWarrorAttackTime);
        Config.WriteInteger("Setup", "WizardAttackTime", (int)M2Config.dwHeroWizardAttackTime);
        Config.WriteInteger("Setup", "TaoistAttackTime", (int)M2Config.dwHeroTaoistAttackTime);
        Config.WriteInteger("Setup", "WarrorWalkTime", (int)M2Config.dwHeroWarrorWalkTime);
        Config.WriteInteger("Setup", "WizardWalkTime", (int)M2Config.dwHeroWizardWalkTime);
        Config.WriteInteger("Setup", "TaoistWalkTime", (int)M2Config.dwHeroTaoistWalkTime);
        Config.WriteInteger("Setup", "HeroAvoidTime", (int)M2Config.dwHeroAvoidTime);
        Config.WriteBool("Setup", "HeroHitCmp", M2Config.boHeroHitCmp);
        Config.WriteInteger("Setup", "WarrCmpInvTime", M2Config.nWarrCmpInvTime);
        Config.WriteInteger("Setup", "RecallHeroTime", M2Config.nRecallHeroTime);
        Config.WriteInteger("Setup", "RecallDeputyHeroTime", M2Config.nRecallDeputyHeroTime);
        Config.WriteInteger("Setup", "ClearHeroGhostTick", M2Config.nClearHeroGhostTick);
        Config.WriteInteger("Setup", "HeroNameColor", M2Config.btHeroNameColor);
        Config.WriteBool("Setup", "HeroShowMasterName", M2Config.boHeroShowMasterName);
        Config.WriteInteger("Setup", "HeroNeedMagicItem", M2Config.nHeroNeedMagicItem);
        Config.WriteBool("Setup", "HeroPickUpItem", M2Config.boHeroPickUpItem);
        Config.WriteBool("Setup", "WarrorAttack", M2Config.boWarrorAttack);
        uModValue();
    }

    public void CheckBoxDeleteItemDuraZeroClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDeleteItemDuraZero = CheckBoxDeleteItemDuraZero.Checked;
        ModValue();
    }

    public void EditMysteriousManNameChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sMysteriousManName = EditMysteriousManName.Text;
        ModValue();
    }

    public void EditMaxLuckMaxPowerChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxLuckMaxPower = (int)EditMaxLuckMaxPower.Value;
        ModValue();
    }

    public void EditQueryBagItemsTimeChange(System.Windows.Forms.NumericUpDown edit)
    {
        if (!boOpened)
            return;
        M2Config.nQueryBagItemsTime = (int)edit.Value;
        ModValue();
    }

    public void ButtonOtherClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "DeleteItemDuraZero", M2Config.boDeleteItemDuraZero);
        Config.WriteString("Setup", "MysteriousManName", M2Config.sMysteriousManName);
        Config.WriteInteger("Setup", "DamageItemDuraRate", M2Config.nDamageItemDuraRate);
        Config.WriteBool("Setup", "DuraChangeLight", M2Config.boDuraChangeLight);
        Config.WriteBool("Setup", "PoisonWeaponCanMagicAttack", M2Config.boPoisonWeaponCanMagicAttack);
        Config.WriteBool("Setup", "PoisonWeaponCanHitAllTarget", M2Config.boPoisonWeaponCanHitAllTarget);
        Config.WriteInteger("Setup", "QueryBagItemsTime", M2Config.nQueryBagItemsTime);
        Config.WriteBool("Setup", "ShowRefreshBagMsg", M2Config.boShowRefreshBagMsg);
        Config.WriteInteger("Setup", "MaxLuckMaxPower", M2Config.nMaxLuckMaxPower);
        Config.WriteBool("Setup", "LuckUseNewAlgorism", M2Config.boLuckUseNewAlgorism);
        Config.WriteBool("Setup", "HongMoSuiteWithPower", M2Config.boHongMoSuiteWithPower);
        Config.WriteBool("Setup", "GroupReCallNotInSafeZone", M2Config.boGroupReCallNotInSafeZone);
        Config.WriteBool("Setup", "NewHumanAttatckMode_HAM_PEACE", M2Config.boNewHumanAttatckMode_HAM_PEACE);
        Config.WriteBool("Setup", "AutoGroupMaster", M2Config.boAutoGroupMaster);
        Config.WriteBool("Setup", "GuardNotAttackPlayMoster", M2Config.boGuardNotAttackPlayMoster);
        Config.WriteBool("Setup", "WarNoDropUseItem", M2Config.boWarNoDropUseItem);
        Config.WriteInteger("Setup", "HongMoSuiteRateChange", M2Config.nHongMoSuiteRateChange);
        Config.WriteBool("Setup", "ShowMysteriousMan", M2Config.boShowMysteriousMan);
        Config.WriteInteger("Setup", "LimitScriptGotoCount", M2Config.nLimitScriptGotoCount);
        Config.WriteBool("Setup", "M2CacheRankData", M2Config.boM2CacheRankData);
        Config.WriteBool("Setup", "GroupUseOldMode", M2Config.boGroupUseOldMode);
        uModValue();
    }

    public void EditBonusAbilofWarrDCChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.BonusAbilofWarr.DC = (int)EditBonusAbilofWarrDC.Value;
        ModValue();
    }

    public void EditBonusAbilofTaosSpeedChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.BonusAbilofTaos.Speed = (int)EditBonusAbilofTaosSpeed.Value;
        ModValue();
    }

    public void ButtonBonusAbilofSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        var warr = M2Config.BonusAbilofWarr;
        var wiz = M2Config.BonusAbilofWizard;
        var taos = M2Config.BonusAbilofTaos;
        Config.WriteInteger("Setup", "BonusAbilofWarrDC", warr.DC);
        Config.WriteInteger("Setup", "BonusAbilofWarrMC", warr.MC);
        Config.WriteInteger("Setup", "BonusAbilofWarrSC", warr.SC);
        Config.WriteInteger("Setup", "BonusAbilofWarrAC", warr.AC);
        Config.WriteInteger("Setup", "BonusAbilofWarrMAC", warr.MAC);
        Config.WriteInteger("Setup", "BonusAbilofWarrHP", warr.HP);
        Config.WriteInteger("Setup", "BonusAbilofWarrMP", warr.MP);
        Config.WriteInteger("Setup", "BonusAbilofWarrHit", warr.Hit);
        Config.WriteInteger("Setup", "BonusAbilofWarrSpeed", warr.Speed);
        Config.WriteInteger("Setup", "BonusAbilofWizardDC", wiz.DC);
        Config.WriteInteger("Setup", "BonusAbilofWizardMC", wiz.MC);
        Config.WriteInteger("Setup", "BonusAbilofWizardSC", wiz.SC);
        Config.WriteInteger("Setup", "BonusAbilofWizardAC", wiz.AC);
        Config.WriteInteger("Setup", "BonusAbilofWizardMAC", wiz.MAC);
        Config.WriteInteger("Setup", "BonusAbilofWizardHP", wiz.HP);
        Config.WriteInteger("Setup", "BonusAbilofWizardMP", wiz.MP);
        Config.WriteInteger("Setup", "BonusAbilofWizardHit", wiz.Hit);
        Config.WriteInteger("Setup", "BonusAbilofWizardSpeed", wiz.Speed);
        Config.WriteInteger("Setup", "BonusAbilofTaosDC", taos.DC);
        Config.WriteInteger("Setup", "BonusAbilofTaosMC", taos.MC);
        Config.WriteInteger("Setup", "BonusAbilofTaosSC", taos.SC);
        Config.WriteInteger("Setup", "BonusAbilofTaosAC", taos.AC);
        Config.WriteInteger("Setup", "BonusAbilofTaosMAC", taos.MAC);
        Config.WriteInteger("Setup", "BonusAbilofTaosHP", taos.HP);
        Config.WriteInteger("Setup", "BonusAbilofTaosMP", taos.MP);
        Config.WriteInteger("Setup", "BonusAbilofTaosHit", taos.Hit);
        Config.WriteInteger("Setup", "BonusAbilofTaosSpeed", taos.Speed);
        uModValue();
    }

    public void CheckBoxRevivalTouchClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boRevivalTouch = CheckBoxRevivalTouch.Checked;
        ModValue();
    }

    public void EditRevivalTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwRevivalTime = (uint)EditRevivalTime.Value * 1000;
        ModValue();
    }

    public void EditStarBaseNumChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStarBaseNum = (int)EditStarBaseNum.Value;
        ModValue();
    }

    public void EditStarLineMaxCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nStarLineMaxCount = (int)EditStarLineMaxCount.Value;
        ModValue();
    }

    public void CheckBoxJewelryDecDuraClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boJewelryDecDura = CheckBoxJewelryDecDura.Checked;
        ModValue();
    }

    public void CheckBoxCloseNPCNoItemMsgClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCloseNPCNoItemMsg = CheckBoxCloseNPCNoItemMsg.Checked;
        ModValue();
    }

    public void CheckBoxDisableMoveParalysisHumanClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableMoveParalysisHuman = CheckBoxDisableMoveParalysisHuman.Checked;
        ModValue();
    }

    public void CheckBoxSaveRevivalTimeClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSaveRevivalTime = CheckBoxSaveRevivalTime.Checked;
        ModValue();
    }

    public void ButtonOther3Click(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "RevivalTime", (int)M2Config.dwRevivalTime);
        Config.WriteBool("Setup", "RevivalTouch", M2Config.boRevivalTouch);
        Config.WriteBool("Setup", "SaveRevivalTime", M2Config.boSaveRevivalTime);
        Config.WriteBool("Setup", "FBExitCreaterOffline", M2Config.boFBExitCreaterOffline);
        Config.WriteBool("Setup", "FBDisableDelay30s", M2Config.boFBDisableDelay30s);
        Config.WriteBool("Setup", "JewelryCalcBasicAbilitys", M2Config.boJewelryCalcBasicAbilitys);
        Config.WriteBool("Setup", "JewelryCalcGroupAbilitys", M2Config.boJewelryCalcGroupAbilitys);
        Config.WriteBool("Setup", "JewelryDecDura", M2Config.boJewelryDecDura);
        Config.WriteString("Setup", "JewelryBoxHint", M2Config.sJewelryBoxHint);
        Config.WriteBool("Setup", "OpenNewGuild", M2Config.boOpenNewGuildTemp);
        Config.WriteBool("Setup", "NoShowNewGuildHumanCount", M2Config.boNoShowNewGuildHumanCount);
        Config.WriteBool("Setup", "CloseNPCNoItemMsg", M2Config.boCloseNPCNoItemMsg);
        Config.WriteBool("Setup", "ChangeUseItemNameByPlayName", M2Config.boChangeUseItemNameByPlayName);
        Config.WriteString("Setup", "ChangeUseItemName", M2Config.sChangeUseItemName);
        Config.WriteInteger("Setup", "StarBaseNum", M2Config.nStarBaseNum);
        Config.WriteInteger("Setup", "StarLineMaxCount", M2Config.nStarLineMaxCount);
        Config.WriteBool("Setup", "DisableDuFuTakeArmRingL", M2Config.boDisableDuFuTakeArmRingL);
        Config.WriteBool("Setup", "RecordBeadExp", M2Config.boRecordBeadExp);
        Config.WriteBool("Setup", "DisableMoveParalysisHuman", M2Config.boDisableMoveParalysisHuman);
        uModValue();
    }
}
