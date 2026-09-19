using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList2.pas TFrmViewList2 控件树（批次J57 第二片）：物品规则页（40 复选框 + 拍卖价格三控件）、
/// 用户命令页、宝箱页、套装组页（16 标志 + 19 比率 + 20 数值 + 2 单选）、WIL 名单页、技能威力页。
/// 布局按 ViewList2.dfm 的分组与可见性 1:1（第 10 个规则复选框 Visible=False 但参与 Delphi 循环）。
/// </summary>
public sealed partial class ViewList2Form
{
    private static readonly (int Tag, string Caption)[] ItemRuleDefs =
    {
        (0, "禁止丢弃"),
        (1, "禁止交易"),
        (2, "禁止存仓"),
        (3, "禁止修理"),
        (4, "禁止出售"),
        (5, "上线消失"),
        (6, "死亡必爆"),
        (7, "禁止英雄装备"),
        (8, "禁止寄售"),
        (9, "禁止存个人商店"),
        (10, "禁止挑战"),
        (11, "禁止宝石升级"),
        (12, "怪物掉落提示"),
        (13, "永不掉落"),
        (14, "禁止商铺打折"),
        (15, "禁止英雄包裹"),
        (16, "英雄物品"),
        (17, "禁止武器升级"),
        (18, "死亡消失"),
        (19, "挖取提示"),
        (20, "禁止捡起"),
        (21, "触发提示"),
        (22, "下线必掉"),
        (23, "宝箱提示"),
        (24, "丢弃消失"),
        (25, "触发ID"),
        (26, "人物掉落提示"),
        (27, "允许拍卖"),
        (28, "怪物掉落触发"),
        (29, "宝箱触发"),
        (30, "禁止透视"),
        (31, "禁止宠物包裹"),
        (39, "人物掉落触发"),
    };

    private static readonly (int Tag, string Caption)[] GroupItemFlagDefs =
    {
        (1, "麻痹"), (2, "防麻痹"), (3, "蛛网"), (4, "防蛛网"),
        (5, "魔道麻痹"), (6, "防魔道麻痹"), (7, "冰冻"), (8, "防冰冻"),
        (9, "护身"), (10, "防护身"), (11, "重生"), (12, "防重生"),
        (13, "吸血"), (14, "防吸血"), (15, "隐身"), (16, "防隐身"),
    };

    private static readonly (int Tag, string Caption)[] GroupItemExtraFlagDefs =
    {
        (16, "冰冻"), (17, "防冰冻"), (18, "蛛网"), (19, "防蛛网"), (20, "魔道麻痹"),
    };

    private static readonly (int Tag, string Name)[] GroupItemRateDefs =
    {
        (0, "EditGroupItemHPRate"), (1, "EditGroupItemMPRate"), (2, "EditGroupItemACRate"),
        (3, "EditGroupItemMACRate"), (4, "EditGroupItemDCRate"), (5, "EditGroupItemMCRate"),
        (6, "EditGroupItemSCRate"), (7, "EditHitPointRate"), (8, "EditSpeedPointRate"),
        (9, "EditAntiMagicRate"), (10, "EditAntiPoisonRate"), (11, "EditPoisonRecoverRate"),
        (12, "EditHealthRecoverRate"), (13, "EditSpellRecoverRate"), (14, "EditGroupItemACRate2"),
        (15, "EditGroupItemMACRate2"), (16, "EditGroupItemDCRate2"), (17, "EditGroupItemMCRate2"),
        (18, "EditGroupItemSCRate2"),
    };

    private static readonly (int Tag, string Name)[] GroupItemValueDefs =
    {
        (0, "seGroupItemHPValue"), (1, "seGroupItemMPValue"), (2, "seGroupItemACValue"),
        (3, "seGroupItemMACValue"), (4, "seGroupItemDCValue"), (5, "seGroupItemMCValue"),
        (6, "seGroupItemSCValue"), (7, "seHitPointValue"), (8, "seSpeedPointValue"),
        (9, "seAntiMagicValue"), (10, "seAntiPoisonValue"), (11, "sePoisonRecoverValue"),
        (12, "seHealthRecoverValue"), (13, "seSpellRecoverValue"), (14, "seGroupItemACValue2"),
        (15, "seGroupItemMACValue2"), (16, "seGroupItemDCValue2"), (17, "seGroupItemMCValue2"),
        (18, "seGroupItemSCValue2"), (19, "SpinEditEx6"),
    };

    private static System.Windows.Forms.NumericUpDown NewSpin(decimal min = -1000000000, decimal max = 1000000000)
        => new() { Minimum = min, Maximum = max, Width = 70 };

    private void InitializeResidualSections()
    {
        // ---------------- 物品规则页（TabSheet4） ----------------
        // Delphi: ListBoxItemRuleList 为 MultiSelect，OnClick 触发（WinForms 用 ItemSelectionChanged +
        // SelectedIndexChanged 会重复；此处只挂 Click 语义的选中变化，不挂 IndexChanged，避免重复回填）
        ListBoxItemRuleList = new System.Windows.Forms.ListBox { Width = 129, Height = 401, SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended };
        ListBoxItemList2 = new System.Windows.Forms.ListBox { Width = 145, Height = 401, SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended };
        EditRuleItemName = new System.Windows.Forms.TextBox();
        GroupBoxItemRule = new System.Windows.Forms.GroupBox { Text = "物品规则" };
        for (int i = 0; i < ItemRuleDefs.Length; i++)
        {
            var (tag, caption) = ItemRuleDefs[i];
            var chk = new System.Windows.Forms.CheckBox { Text = caption, Tag = tag, AutoSize = true, Visible = tag != 0 };
            if (tag == 0)
                ItemRuleHiddenCheck = chk;
            ItemRuleChecks[tag] = chk;
        }
        cbbAuctionPricesType = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        // Delphi FormCreate（4945-4950）：五档拍卖货币，ItemIndex = 0
        cbbAuctionPricesType.Items.Add(M2Config.sGameGoldName);
        cbbAuctionPricesType.Items.Add(M2Config.sGamePointName);
        cbbAuctionPricesType.Items.Add("金币");
        cbbAuctionPricesType.Items.Add(M2Config.sGameDiamondName);
        cbbAuctionPricesType.Items.Add(M2Config.sGameGirdName);
        cbbAuctionPricesType.SelectedIndexChanged += (_, _) => CbbAuctionPricesTypeChange();
        seAuctionPrices_Min = NewSpin(0, uint.MaxValue);
        seAuctionPrices_Max = NewSpin(0, uint.MaxValue);
        seAuctionPrices_Min.ValueChanged += (_, _) => SeAuctionPricesMinChange();
        seAuctionPrices_Max.ValueChanged += (_, _) => SeAuctionPricesMaxChange();

        ButtonItemRuleSelAll = new System.Windows.Forms.Button { Text = "全部选中" };
        ButtonItemRuleSelAll.Click += (_, _) => ButtonItemRuleSelAllClick();
        ButtonItemRuleNotSelAll = new System.Windows.Forms.Button { Text = "全部不选" };
        ButtonItemRuleNotSelAll.Click += (_, _) => ButtonItemRuleNotSelAllClick();
        ButtonItemRuleAdd = new System.Windows.Forms.Button();
        ButtonItemRuleAdd.Click += (_, _) => ButtonItemRuleAddClick();
        ButtonItemRuleDel = new System.Windows.Forms.Button { Enabled = false };
        ButtonItemRuleDel.Click += (_, _) => ButtonItemRuleDelClick();
        ButtonItemRuleAddAll = new System.Windows.Forms.Button();
        ButtonItemRuleAddAll.Click += (_, _) => ButtonItemRuleAddAllClick();
        ButtonItemRuleDelAll = new System.Windows.Forms.Button();
        ButtonItemRuleDelAll.Click += (_, _) => ButtonItemRuleDelAllClick();
        ButtonItemRuleChg = new System.Windows.Forms.Button { Enabled = false };
        ButtonItemRuleChg.Click += (_, _) => ButtonItemRuleChgClick();
        ButtonItemRuleSave = new System.Windows.Forms.Button { Enabled = false };
        ButtonItemRuleSave.Click += (_, _) => { ViewList2State.g_ItemRules.SaveToEnvirFile(); ButtonItemRuleSave.Enabled = false; };

        var rulePage = new System.Windows.Forms.TabPage("物品规则");
        var ruleGroup = new System.Windows.Forms.GroupBox { Text = "物品规则", Width = 700, Height = 260 };
        for (int i = 0; i < ItemRuleDefs.Length; i++)
        {
            var chk = ItemRuleChecks[ItemRuleDefs[i].Tag]!;
            chk.Left = 8 + (i % 4) * 170;
            chk.Top = 18 + (i / 4) * 22;
            ruleGroup.Controls.Add(chk);
        }
        ruleGroup.Controls.Add(ButtonItemRuleSelAll);
        ruleGroup.Controls.Add(ButtonItemRuleNotSelAll);
        rulePage.Controls.Add(ListBoxItemRuleList);
        rulePage.Controls.Add(ListBoxItemList2);
        rulePage.Controls.Add(EditRuleItemName);
        rulePage.Controls.Add(ruleGroup);
        rulePage.Controls.Add(cbbAuctionPricesType);
        rulePage.Controls.Add(seAuctionPrices_Min);
        rulePage.Controls.Add(seAuctionPrices_Max);
        rulePage.Controls.Add(ButtonItemRuleAdd);
        rulePage.Controls.Add(ButtonItemRuleDel);
        rulePage.Controls.Add(ButtonItemRuleAddAll);
        rulePage.Controls.Add(ButtonItemRuleDelAll);
        rulePage.Controls.Add(ButtonItemRuleChg);
        rulePage.Controls.Add(ButtonItemRuleSave);

        // ---------------- 用户命令页 ----------------
        ListBoxUserCommand = new System.Windows.Forms.ListBox();
        ListBoxUserCommand.SelectedIndexChanged += (_, _) => ListBoxUserCommandClick();
        EditCommandName = new System.Windows.Forms.TextBox();
        EditCommandIdx = NewSpin(0, 100000);
        LabelMsg = new System.Windows.Forms.Label { AutoSize = true };
        ButtonUserCommandAdd = new System.Windows.Forms.Button();
        ButtonUserCommandAdd.Click += (_, _) => ButtonUserCommandAddClick();
        ButtonUserCommandDel = new System.Windows.Forms.Button { Enabled = false };
        ButtonUserCommandDel.Click += (_, _) => ButtonUserCommandDelClick();
        ButtonUserCommandSave = new System.Windows.Forms.Button { Enabled = false };
        ButtonUserCommandSave.Click += (_, _) => { ViewList2State.g_UserCmds.SaveToFile(); ButtonUserCommandSave.Enabled = false; };

        var cmdPage = new System.Windows.Forms.TabPage("用户命令");
        cmdPage.Controls.Add(ListBoxUserCommand);
        cmdPage.Controls.Add(EditCommandName);
        cmdPage.Controls.Add(EditCommandIdx);
        cmdPage.Controls.Add(LabelMsg);
        cmdPage.Controls.Add(ButtonUserCommandAdd);
        cmdPage.Controls.Add(ButtonUserCommandDel);
        cmdPage.Controls.Add(ButtonUserCommandSave);

        // ---------------- 宝箱页 ----------------
        ListBoxitemList1 = new System.Windows.Forms.ListBox();
        ListBoxBoxItem = new System.Windows.Forms.ListBox();
        ListBoxBoxItem.SelectedIndexChanged += (_, _) => ListBoxBoxItemClick();
        GroupBoxBoxItem = new System.Windows.Forms.GroupBox { Text = "宝箱物品" };
        ListBoxGiveItem = new System.Windows.Forms.ListBox();
        ListBoxGiveItem.SelectedIndexChanged += (_, _) => BoxItemListClick(0);
        ListBoxCenterItem = new System.Windows.Forms.ListBox();
        ListBoxCenterItem.SelectedIndexChanged += (_, _) => BoxItemListClick(1);
        ListBoxNoGiveItem = new System.Windows.Forms.ListBox();
        ListBoxNoGiveItem.SelectedIndexChanged += (_, _) => BoxItemListClick(2);
        ListBoxEndNoGiveItem = new System.Windows.Forms.ListBox();
        ListBoxEndNoGiveItem.SelectedIndexChanged += (_, _) => BoxItemListClick(3);
        ComboBoxBoxItemType = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        ComboBoxBoxItemType.Items.AddRange(new object[] { "可得", "不可得", "中间一格", "永不可得" });
        edtBoxItemName = new System.Windows.Forms.TextBox();
        cbbOtherItem = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbOtherItem.SelectedIndexChanged += (_, _) => CbbOtherItemChange();
        seBoxItemCount = NewSpin(0, 100000);
        seBoxItemCount.ValueChanged += (_, _) => SeBoxItemCountChange();
        btnAddBoxItem = new System.Windows.Forms.Button { Enabled = false };
        btnAddBoxItem.Click += (_, _) => BtnAddBoxItemClick();
        btnDelBoxItem = new System.Windows.Forms.Button { Enabled = false };
        btnDelBoxItem.Click += (_, _) => BtnDelBoxItemClick();
        btnSaveBoxItem = new System.Windows.Forms.Button { Enabled = false };
        btnSaveBoxItem.Click += (_, _) => BtnSaveBoxItemClick();
        chkNext = new System.Windows.Forms.CheckBox { Text = "循环" };
        chkNext.CheckedChanged += (_, _) => ChkNextClick();
        seCount = NewSpin(0, 255);
        seCount.ValueChanged += (_, _) => BoxSetFieldChanged("Count");
        seGold = NewSpin();
        seGold.ValueChanged += (_, _) => BoxSetFieldChanged("Gold");
        seGameGold = NewSpin();
        seGameGold.ValueChanged += (_, _) => BoxSetFieldChanged("GameGold");
        seAddGold = NewSpin();
        seAddGold.ValueChanged += (_, _) => BoxSetFieldChanged("AddGold");
        seAddGameGold = NewSpin();
        seAddGameGold.ValueChanged += (_, _) => BoxSetFieldChanged("AddGameGold");
        seEndGold = NewSpin();
        seEndGold.ValueChanged += (_, _) => BoxSetFieldChanged("EndGold");
        seEndGameGold = NewSpin();
        seEndGameGold.ValueChanged += (_, _) => BoxSetFieldChanged("EndGameGold");

        var boxPage = new System.Windows.Forms.TabPage("宝箱");
        boxPage.Controls.Add(ListBoxitemList1);
        boxPage.Controls.Add(ListBoxBoxItem);
        boxPage.Controls.Add(GroupBoxBoxItem);
        GroupBoxBoxItem.Controls.Add(ListBoxGiveItem);
        GroupBoxBoxItem.Controls.Add(ListBoxCenterItem);
        GroupBoxBoxItem.Controls.Add(ListBoxNoGiveItem);
        GroupBoxBoxItem.Controls.Add(ListBoxEndNoGiveItem);
        boxPage.Controls.Add(ComboBoxBoxItemType);
        boxPage.Controls.Add(edtBoxItemName);
        boxPage.Controls.Add(cbbOtherItem);
        boxPage.Controls.Add(seBoxItemCount);
        boxPage.Controls.Add(btnAddBoxItem);
        boxPage.Controls.Add(btnDelBoxItem);
        boxPage.Controls.Add(btnSaveBoxItem);
        boxPage.Controls.Add(chkNext);
        boxPage.Controls.Add(seCount);
        boxPage.Controls.Add(seGold);
        boxPage.Controls.Add(seGameGold);
        boxPage.Controls.Add(seAddGold);
        boxPage.Controls.Add(seAddGameGold);
        boxPage.Controls.Add(seEndGold);
        boxPage.Controls.Add(seEndGameGold);

        // ---------------- 套装组页 ----------------
        ListViewGroupItemList = new System.Windows.Forms.ListView { View = System.Windows.Forms.View.Details, FullRowSelect = true };
        ListViewGroupItemList.Columns.Add("套装编号", 60);
        ListViewGroupItemList.Columns.Add("套装说明", 90);
        ListViewGroupItemList.Columns.Add("数量", 60);
        ListViewGroupItemList.Columns.Add("套装物品", 300);
        ListViewGroupItemList.SelectedIndexChanged += (_, _) => ListViewGroupItemListClick();
        GroupBoxGroupItem = new System.Windows.Forms.GroupBox { Text = "附加属性设置" };
        for (int i = 0; i < GroupItemFlagDefs.Length; i++)
        {
            var (tag, caption) = GroupItemFlagDefs[i];
            var chk = new System.Windows.Forms.CheckBox { Text = caption, Tag = tag, AutoSize = true };
            GroupItemChecks[tag] = chk;
            GroupBoxGroupItem.Controls.Add(chk);
        }
        for (int i = 0; i < GroupItemExtraFlagDefs.Length; i++)
        {
            var (tag, caption) = GroupItemExtraFlagDefs[i];
            var chk = new System.Windows.Forms.CheckBox { Text = caption, Tag = tag, AutoSize = true };
            GroupItemExtraChecks[tag] = chk;
            GroupBoxGroupItem.Controls.Add(chk);
        }
        for (int i = 0; i < GroupItemRateDefs.Length; i++)
        {
            var (tag, name) = GroupItemRateDefs[i];
            var spin = NewSpin();
            spin.Name = name;
            spin.Tag = tag;
            spin.ValueChanged += (_, _) => { if (SelGroupItem != null) SelGroupItem.FLD_RATE[tag] = (int)spin.Value; };
            GroupItemRates[tag] = spin;
            GroupBoxGroupItem.Controls.Add(spin);
        }
        for (int i = 0; i < GroupItemValueDefs.Length; i++)
        {
            var (tag, name) = GroupItemValueDefs[i];
            var spin = NewSpin();
            spin.Name = name;
            spin.Tag = tag;
            spin.ValueChanged += (_, _) => { if (SelGroupItem != null) SelGroupItem.FLD_VALUE[tag] = (int)spin.Value; };
            GroupItemValues[tag] = spin;
            GroupBoxGroupItem.Controls.Add(spin);
        }
        EditGroupItemIndex = NewSpin(0, 100000);
        EditGroupItemHint = new System.Windows.Forms.TextBox();
        EditGroupItemName = new System.Windows.Forms.TextBox();
        EditGroupItemCount = NewSpin(0, 100000);
        EditGroupItemDesc = new System.Windows.Forms.TextBox();
        RadioButtonRate = new System.Windows.Forms.RadioButton { Text = "先加百分比再加属性点", Checked = true };
        RadioButtonValue = new System.Windows.Forms.RadioButton { Text = "先加属性点再加百分比" };
        RadioButtonValue.CheckedChanged += (_, _) => { if (RadioButtonValue.Checked) M2Config.boGroupItemRule = true; else if (RadioButtonRate.Checked) M2Config.boGroupItemRule = false; };
        ButtonGroupItemAdd = new System.Windows.Forms.Button();
        ButtonGroupItemAdd.Click += (_, _) => ButtonGroupItemAddClick();
        ButtonGroupItemDel = new System.Windows.Forms.Button { Enabled = false };
        ButtonGroupItemDel.Click += (_, _) => ButtonGroupItemDelClick();
        ButtonGroupItemChg = new System.Windows.Forms.Button { Enabled = false };
        ButtonGroupItemChg.Click += (_, _) => ButtonGroupItemChgClick();
        ButtonGroupItemSave = new System.Windows.Forms.Button { Enabled = false };
        ButtonGroupItemSave.Click += (_, _) => ButtonGroupItemSaveClick();
        ButtonGroupItemSkillPower = new System.Windows.Forms.Button { Text = "套装技能威力百分比", Enabled = false };
        ButtonGroupItemSkillPower.Click += (_, _) => ButtonGroupItemSkillPowerClick();

        var groupPage = new System.Windows.Forms.TabPage("套装组");
        groupPage.Controls.Add(ListViewGroupItemList);
        groupPage.Controls.Add(GroupBoxGroupItem);
        groupPage.Controls.Add(EditGroupItemIndex);
        groupPage.Controls.Add(EditGroupItemHint);
        groupPage.Controls.Add(EditGroupItemName);
        groupPage.Controls.Add(EditGroupItemCount);
        groupPage.Controls.Add(EditGroupItemDesc);
        groupPage.Controls.Add(RadioButtonRate);
        groupPage.Controls.Add(RadioButtonValue);
        groupPage.Controls.Add(ButtonGroupItemAdd);
        groupPage.Controls.Add(ButtonGroupItemDel);
        groupPage.Controls.Add(ButtonGroupItemChg);
        groupPage.Controls.Add(ButtonGroupItemSave);
        groupPage.Controls.Add(ButtonGroupItemSkillPower);

        // ---------------- WIL 名单页 ----------------
        ListBoxWilNameList = new System.Windows.Forms.ListBox();
        ListBoxWilNameList.SelectedIndexChanged += (_, _) => ListBoxWilNameListClick();
        EditWilName = new System.Windows.Forms.TextBox();
        LabelFileIndex = new System.Windows.Forms.Label { AutoSize = true };
        btnWilNameUP = new System.Windows.Forms.Button { Enabled = false };
        btnWilNameUP.Click += (_, _) => BtnWilNameUpClick();
        btnWilNameDown = new System.Windows.Forms.Button { Enabled = false };
        btnWilNameDown.Click += (_, _) => BtnWilNameDownClick();
        btnWilAdd = new System.Windows.Forms.Button();
        btnWilAdd.Click += (_, _) => BtnWilAddClick();
        btnWilDel = new System.Windows.Forms.Button { Enabled = false };
        btnWilDel.Click += (_, _) => BtnWilDelClick();
        btnWilSave = new System.Windows.Forms.Button { Enabled = false };
        btnWilSave.Click += (_, _) => BtnWilSaveClick();
        btnWilEdit = new System.Windows.Forms.Button { Enabled = false };
        btnWilEdit.Click += (_, _) => BtnWilEditClick();
        ButtonSendEffectImageList = new System.Windows.Forms.Button { Text = "下发特效列表" };
        ButtonSendEffectImageList.Click += (_, _) => ButtonSendEffectImageListClick();

        // cbbEffectFileIndex1/2/3/5 + cbbAddEffectFileIndex（Delphi 五个特效文件下拉）
        for (int i = 0; i < 5; i++)
        {
            var cbb = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
            EffectFileIndexCombos.Add(cbb);
        }

        var wilPage = new System.Windows.Forms.TabPage("WIL 名单");
        wilPage.Controls.Add(ListBoxWilNameList);
        wilPage.Controls.Add(EditWilName);
        wilPage.Controls.Add(LabelFileIndex);
        wilPage.Controls.Add(btnWilNameUP);
        wilPage.Controls.Add(btnWilNameDown);
        wilPage.Controls.Add(btnWilAdd);
        wilPage.Controls.Add(btnWilDel);
        wilPage.Controls.Add(btnWilSave);
        wilPage.Controls.Add(btnWilEdit);
        wilPage.Controls.Add(ButtonSendEffectImageList);
        foreach (var cbb in EffectFileIndexCombos)
            wilPage.Controls.Add(cbb);

        // ---------------- 技能威力页 ----------------
        ListBoxitemList5 = new System.Windows.Forms.ListBox();
        ListBoxitemList5.SelectedIndexChanged += (_, _) => ListBoxitemList5Click();
        ListBoxSkillPowerItem = new System.Windows.Forms.ListBox();
        ListBoxSkillPowerItem.SelectedIndexChanged += (_, _) => ListBoxSkillPowerItemClick();
        GroupBoxSkillPowerItem = new System.Windows.Forms.GroupBox { Text = "技能威力百分比设置" };
        ButtonAddSkillPowerItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonAddSkillPowerItem.Click += (_, _) => ButtonAddSkillPowerItemClick();
        ButtonChgSkillPowerItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonChgSkillPowerItem.Click += (_, _) => ButtonChgSkillPowerItemClick();
        ButtonDelSkillPowerItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonDelSkillPowerItem.Click += (_, _) => ButtonDelSkillPowerItemClick();
        ButtonSaveSkillPowerItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonSaveSkillPowerItem.Click += (_, _) => ButtonSaveSkillPowerItemClick();
        chkSkillPowerItemUseHum = new System.Windows.Forms.CheckBox { Text = "对人有效" };
        chkSkillPowerItemUseHum.CheckedChanged += (_, _) => ChkSkillPowerItemUseChanged();
        chkSkillPowerItemUseMon = new System.Windows.Forms.CheckBox { Text = "对怪有效" };
        chkSkillPowerItemUseMon.CheckedChanged += (_, _) => ChkSkillPowerItemUseChanged();

        var powerPage = new System.Windows.Forms.TabPage("装备技能威力");
        powerPage.Controls.Add(ListBoxitemList5);
        powerPage.Controls.Add(ListBoxSkillPowerItem);
        powerPage.Controls.Add(GroupBoxSkillPowerItem);
        powerPage.Controls.Add(ButtonAddSkillPowerItem);
        powerPage.Controls.Add(ButtonChgSkillPowerItem);
        powerPage.Controls.Add(ButtonDelSkillPowerItem);
        powerPage.Controls.Add(ButtonSaveSkillPowerItem);
        powerPage.Controls.Add(chkSkillPowerItemUseHum);
        powerPage.Controls.Add(chkSkillPowerItemUseMon);

        // ---------------- 物品过滤/备注复选框（宿主页） ----------------
        chkSendFilterItemList = new System.Windows.Forms.CheckBox { Text = "下发送过滤物品列表" };
        chkSendFilterItemList.CheckedChanged += (_, _) => ChkSendFilterItemListClick();
        chkSendItemDescList = new System.Windows.Forms.CheckBox { Text = "下发送物品备注列表" };
        chkSendItemDescList.CheckedChanged += (_, _) => ChkSendItemDescListClick();
        chkSendItemDescTopList = new System.Windows.Forms.CheckBox { Text = "下发送物品置顶备注列表" };
        chkSendTzItemDescList = new System.Windows.Forms.CheckBox { Text = "下发送套装物品备注列表" };
        chkSendTzItemDescList.CheckedChanged += (_, _) => ChkSendTzItemDescListClick();
        chkSingleHint = new System.Windows.Forms.CheckBox { Text = "单件装备属性单独显示" };
        chkSingleHint.CheckedChanged += (_, _) => ChkSingleHintClick();
        chkTZSupportRenameItem = new System.Windows.Forms.CheckBox { Text = "套装支持改名物品" };
        chkTZSupportRenameItem.CheckedChanged += (_, _) => ChkRenameFlagsChanged();
        chkDescSupportRenamItem = new System.Windows.Forms.CheckBox { Text = "备注支持改名物品" };
        chkDescSupportRenamItem.CheckedChanged += (_, _) => ChkRenameFlagsChanged();
        chkNoRenameDescReadDefault = new System.Windows.Forms.CheckBox { Text = "未改名物品读取默认备注" };
        chkNoRenameDescReadDefault.CheckedChanged += (_, _) => ChkRenameFlagsChanged();
        chkEnablePlayerUseClientPickItems = new System.Windows.Forms.CheckBox { Text = "玩家使用客户端物品列表" };
        chkEnablePlayerUseClientPickItems.CheckedChanged += (_, _) => ChkEnablePlayerUseClientPickItemsClick();
        chkEnableHeroUseClientPickItems = new System.Windows.Forms.CheckBox { Text = "英雄使用客户端物品列表" };
        chkEnableHeroUseClientPickItems.CheckedChanged += (_, _) => ChkEnableHeroUseClientPickItemsClick();
        chkEnabledBuyShopItemGive = new System.Windows.Forms.CheckBox { Text = "购买商店物品赠送" };
        chkEnabledBuyShopItemGive.CheckedChanged += (_, _) => ChkEnabledBuyShopItemGiveClick();

        var optPage = new System.Windows.Forms.TabPage("过滤与备注");
        optPage.Controls.Add(chkSendFilterItemList);
        optPage.Controls.Add(chkSendItemDescList);
        optPage.Controls.Add(chkSendItemDescTopList);
        optPage.Controls.Add(chkSendTzItemDescList);
        optPage.Controls.Add(chkSingleHint);
        optPage.Controls.Add(chkTZSupportRenameItem);
        optPage.Controls.Add(chkDescSupportRenamItem);
        optPage.Controls.Add(chkNoRenameDescReadDefault);
        optPage.Controls.Add(chkEnablePlayerUseClientPickItems);
        optPage.Controls.Add(chkEnableHeroUseClientPickItems);
        optPage.Controls.Add(chkEnabledBuyShopItemGive);

        PageControlResidual = new System.Windows.Forms.TabControl { Width = 900, Height = 520 };
        PageControlResidual.TabPages.Add(rulePage);
        PageControlResidual.TabPages.Add(cmdPage);
        PageControlResidual.TabPages.Add(boxPage);
        PageControlResidual.TabPages.Add(groupPage);
        PageControlResidual.TabPages.Add(wilPage);
        PageControlResidual.TabPages.Add(powerPage);
        PageControlResidual.TabPages.Add(optPage);
        Controls.Add(PageControlResidual);
    }

    /// <summary>第二片控件宿主（PageControl，与第一片 PageControlShop 并列，Delphi PageControl 等效）。</summary>
    public System.Windows.Forms.TabControl PageControlResidual = null!;


    /// <summary>
    /// Open 第二片（ViewList2.pas 1322-1624 的余下部分）：物品填充、套装威力表头、按钮初态、
    /// 配置回显、四类清单重装。
    /// </summary>
    /// <param name="stdItemNames">UserEngine.StdItemList 物品名快照（与 StdMode/Shape 并行）。</param>
    public void OpenResidualSections(
        IReadOnlyList<(string Name, byte StdMode, ushort Shape)> stdItemList)
    {
        boOpened = false;
        Array.Clear(FAcutionPricesLimeMin);
        Array.Clear(FAcutionPricesLimeMax);
        Array.Clear(ViewList2State.SelAttackSkillPercent);
        Array.Clear(ViewList2State.SelDefenseSkillPercent);

        ListBoxitemList1.Items.Clear();
        ListBoxBoxItem.Items.Clear();
        ListBoxitemList5.Items.Clear();

        foreach (var (name, stdMode, shape) in stdItemList)
        {
            ListBoxitemList1.Items.Add(name);
            ListBoxItemList2.Items.Add(name);
            ListBoxitemList5.Items.Add(name);
            if (stdMode == 31 && shape is >= 15 and <= 49)
                ListBoxBoxItem.Items.Add(name);
        }

        // 其他物品下拉（经验/声望/金刚石/灵符）
        OtherItemNames.Clear();
        OtherItemNames.Add("经验");
        OtherItemNames.Add(M2Config.sCreditPointName);
        OtherItemNames.Add(M2Config.sGameDiamondName);
        OtherItemNames.Add(M2Config.sGameGirdName);
        cbbOtherItem.Items.Clear();
        foreach (var n in OtherItemNames)
            cbbOtherItem.Items.Add(n);

        RefSkillPowerList();
        ListBoxWilNameList.Items.Clear();
        foreach (var n in ViewList2State.g_EffectImageList)
            ListBoxWilNameList.Items.Add(n);

        for (int i = 0; i < EffectFileIndexCombos.Count; i++)
        {
            var cbb = EffectFileIndexCombos[i];
            cbb.Items.Clear();
            cbb.Items.Add("关闭特效");
            foreach (var n in ViewList2State.g_EffectImageList)
                cbb.Items.Add(n);
        }

        btnAddBoxItem.Enabled = false;
        btnDelBoxItem.Enabled = false;
        btnSaveBoxItem.Enabled = false;

        ButtonAddSkillPowerItem.Enabled = false;
        ButtonChgSkillPowerItem.Enabled = false;
        ButtonDelSkillPowerItem.Enabled = false;
        ButtonSaveSkillPowerItem.Enabled = false;

        RadioButtonValue.Checked = M2Config.boGroupItemRule;
        // WinForms 同值赋值不触发 CheckedChanged（Delphi OnClick 语义）；显式回写保证配置与控件一致
        M2Config.boGroupItemRule = RadioButtonValue.Checked;

        chkSkillPowerItemUseHum.Checked = M2Config.boSkillPowerItemUseHum;
        chkSkillPowerItemUseMon.Checked = M2Config.boSkillPowerItemUseMon;

        RefShopList();
        RefFilterMsgList();
        RefItemRuleList();
        RefUserCommandList();
        RefGroupItemList();

        ButtonDelShopItem.Enabled = false;
        ButtonShopChgItem.Enabled = false;
        ButtonShopSaveItem.Enabled = false;

        ButtonMsgFilterChg.Enabled = false;
        ButtonMsgFilterDel.Enabled = false;
        ButtonMsgFilterSave.Enabled = false;

        ButtonItemRuleChg.Enabled = false;
        ButtonItemRuleDel.Enabled = false;
        ButtonItemRuleSave.Enabled = false;

        ButtonUserCommandDel.Enabled = false;
        ButtonUserCommandSave.Enabled = false;

        chkEnabledBuyShopItemGive.Checked = M2Config.boEnabledBuyShopItemGive;

        // {$IF NEED_KEY <> 2} 分支：VMProtect 键控（g_nKey_UseClientPickItems == 1 时才可勾选）
        chkEnablePlayerUseClientPickItems.Enabled = false;
        chkEnableHeroUseClientPickItems.Enabled = false;
        if (g_nKey_UseClientPickItems == 1)
        {
            chkEnablePlayerUseClientPickItems.Enabled = true;
            chkEnableHeroUseClientPickItems.Enabled = true;
            chkEnablePlayerUseClientPickItems.Checked = M2Config.boEnablePlayerUseClientPickItems;
            chkEnableHeroUseClientPickItems.Checked = M2Config.boEnableHeroUseClientPickItems;
        }

        chkTZSupportRenameItem.Checked = M2Config.boTZSupportRenameItem;
        chkDescSupportRenamItem.Checked = M2Config.boDescSupportRenamItem;
        chkNoRenameDescReadDefault.Checked = M2Config.boNoRenameDescReadDefault;

        chkSendFilterItemList.Checked = M2Config.boSendFilterItemList;
        chkSendItemDescList.Checked = M2Config.boSendItemDescList;
        chkSendItemDescTopList.Checked = M2Config.boSendItemDescTopList;
        chkSendTzItemDescList.Checked = M2Config.boSendTzItemDescList;
        chkSingleHint.Checked = M2Config.boSingleHint;

        SkillPowerGrid[0, 0] = "技能名称";
        SkillPowerGrid[1, 0] = "增加技能伤害百分比";
        SkillPowerGrid[2, 0] = "增加技能防御百分比";

        ViewList2State.g_ItemRules.GetStdItemIdxHandler = GetStdItemIdxHandler;   // UserEngine.GetStdItemIdx 等效接缝
        ItemRuleFormCreate();
        boOpened = true;
    }

    /// <summary>
    /// Open 的技能威力网格装填（1571-1620）：DEF_MAGIC_COUNT(250) + CUSTOM_MAGIC_COUNT(300) = 550 行。
    /// FindMagic 接缝 + 自定义技能保护模式后缀。
    /// </summary>
    public void FillSkillPowerGrid(
        Func<int, string?> findMagicName,
        Func<int, bool> isCustomMagicProtect)
    {
        for (int i = 0; i < 250; i++)
        {
            string? name = findMagicName(i + 1);
            SkillPowerGrid[0, i + 1] = name ?? "";
            SkillPowerGrid[1, i + 1] = "0";
            SkillPowerGrid[2, i + 1] = "0";
            if (name != null && !TGroupItems.IsValidMagicInSkillPowerItem(i + 1))
                SkillPowerGrid[0, i + 1] = name + "[无效]";
        }
        for (int i = 250; i < 550; i++)
        {
            int magicId = GXX.Core.Protocol.Grobal2Const.CUSTOM_MAGIC_START_ID + i - 250;
            string? name = findMagicName(magicId);
            SkillPowerGrid[0, i + 1] = name ?? "";
            SkillPowerGrid[1, i + 1] = "0";
            SkillPowerGrid[2, i + 1] = "0";
            if (name != null && isCustomMagicProtect(magicId))
                SkillPowerGrid[0, i + 1] = name + "[无效]";
        }
    }
}
