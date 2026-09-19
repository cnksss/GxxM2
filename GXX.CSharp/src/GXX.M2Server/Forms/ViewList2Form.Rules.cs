using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList2.pas TFrmViewList2 第二片（批次J57）：物品规则页（41 位标志复选框组 + 拍卖价格区间 5 档 +
/// 增/删/改/全增/全删/全部选中/全部不选/批量右键 + 保存落盘 ItemRuleList.txt）、用户命令页
/// （g_UserCmds 增删与保存 UserCmd.txt）、宝箱页（g_BoxsList 四类物品列表 + 8 项配置 + 保存）、
/// 套装组页（g_GroupItems 索引/说明/数量/物品名 + 16 标志 + 20 比率 + 20 数值 + 技能威力入口）、
/// WIL 名单页（g_EffectImageList 增删上下移保存）、技能威力物品页（g_SkillPowerItemList）。
/// </summary>
public sealed partial class ViewList2Form
{
    // ==================== 物品规则页（TabSheet4） ====================

    public System.Windows.Forms.ListBox ListBoxItemRuleList = null!;
    public System.Windows.Forms.ListBox ListBoxItemList2 = null!;
    public System.Windows.Forms.TextBox EditRuleItemName = null!;
    public System.Windows.Forms.GroupBox GroupBoxItemRule = null!;
    public System.Windows.Forms.ComboBox cbbAuctionPricesType = null!;
    public System.Windows.Forms.NumericUpDown seAuctionPrices_Min = null!;
    public System.Windows.Forms.NumericUpDown seAuctionPrices_Max = null!;
    public System.Windows.Forms.Button ButtonItemRuleSelAll = null!;
    public System.Windows.Forms.Button ButtonItemRuleNotSelAll = null!;
    public System.Windows.Forms.Button ButtonItemRuleAdd = null!;
    public System.Windows.Forms.Button ButtonItemRuleDel = null!;
    public System.Windows.Forms.Button ButtonItemRuleAddAll = null!;
    public System.Windows.Forms.Button ButtonItemRuleDelAll = null!;
    public System.Windows.Forms.Button ButtonItemRuleChg = null!;
    public System.Windows.Forms.Button ButtonItemRuleSave = null!;

    /// <summary>GroupBoxItemRule 内 40 个复选框（下标=Tag，未用的 Tag 为 null）。</summary>
    public readonly System.Windows.Forms.CheckBox?[] ItemRuleChecks = new System.Windows.Forms.CheckBox?[41];

    /// <summary>Delphi 设计期 Visible=False 的第 10 个复选框（Tag=0，Delphi 仍参与循环）。</summary>
    public System.Windows.Forms.CheckBox? ItemRuleHiddenCheck;

    /// <summary>SelItemRule（pTItemRule）。</summary>
    public TItemRule? SelItemRule;



    /// <summary>FAcutionPricesLime（5 档拍卖价格区间，Delphi on-stack record 等效）。</summary>
    public readonly uint[] FAcutionPricesLimeMin = new uint[5];
    public readonly uint[] FAcutionPricesLimeMax = new uint[5];

    /// <summary>UserEngine.GetStdItemIdx 接缝（物品规则按索引定位）。</summary>
    public Func<string, int>? GetStdItemIdxHandler;

    // ==================== 用户命令页（TabSheet5） ====================

    public System.Windows.Forms.ListBox ListBoxUserCommand = null!;
    public System.Windows.Forms.TextBox EditCommandName = null!;
    public System.Windows.Forms.NumericUpDown EditCommandIdx = null!;
    public System.Windows.Forms.Label LabelMsg = null!;
    public System.Windows.Forms.Button ButtonUserCommandAdd = null!;
    public System.Windows.Forms.Button ButtonUserCommandDel = null!;
    public System.Windows.Forms.Button ButtonUserCommandSave = null!;

    // ==================== 宝箱页（TabSheet2） ====================

    public System.Windows.Forms.ListBox ListBoxitemList1 = null!;
    public System.Windows.Forms.ListBox ListBoxBoxItem = null!;
    public System.Windows.Forms.GroupBox GroupBoxBoxItem = null!;
    public System.Windows.Forms.ListBox ListBoxGiveItem = null!;
    public System.Windows.Forms.ListBox ListBoxCenterItem = null!;
    public System.Windows.Forms.ListBox ListBoxNoGiveItem = null!;
    public System.Windows.Forms.ListBox ListBoxEndNoGiveItem = null!;
    public System.Windows.Forms.ComboBox ComboBoxBoxItemType = null!;
    public System.Windows.Forms.TextBox edtBoxItemName = null!;
    public System.Windows.Forms.ComboBox cbbOtherItem = null!;
    public System.Windows.Forms.NumericUpDown seBoxItemCount = null!;
    public System.Windows.Forms.Button btnAddBoxItem = null!;
    public System.Windows.Forms.Button btnDelBoxItem = null!;
    public System.Windows.Forms.Button btnSaveBoxItem = null!;
    public System.Windows.Forms.CheckBox chkNext = null!;
    public System.Windows.Forms.NumericUpDown seCount = null!;
    public System.Windows.Forms.NumericUpDown seGold = null!;
    public System.Windows.Forms.NumericUpDown seGameGold = null!;
    public System.Windows.Forms.NumericUpDown seAddGold = null!;
    public System.Windows.Forms.NumericUpDown seAddGameGold = null!;
    public System.Windows.Forms.NumericUpDown seEndGold = null!;
    public System.Windows.Forms.NumericUpDown seEndGameGold = null!;

    /// <summary>SelBox（pTBox）。</summary>
    public TBox? SelBox;

    /// <summary>cbbOtherItem 特殊物品项（经验/声望/金刚石/灵符/游戏点）。</summary>
    public readonly List<string> OtherItemNames = new();

    // ==================== 套装组页（TabSheet6/TabSheet13） ====================

    public System.Windows.Forms.ListView ListViewGroupItemList = null!;
    public System.Windows.Forms.GroupBox GroupBoxGroupItem = null!;
    public System.Windows.Forms.NumericUpDown EditGroupItemIndex = null!;
    public System.Windows.Forms.TextBox EditGroupItemHint = null!;
    public System.Windows.Forms.TextBox EditGroupItemName = null!;
    public System.Windows.Forms.NumericUpDown EditGroupItemCount = null!;
    public System.Windows.Forms.TextBox EditGroupItemDesc = null!;
    public System.Windows.Forms.RadioButton RadioButtonRate = null!;
    public System.Windows.Forms.RadioButton RadioButtonValue = null!;
    public System.Windows.Forms.Button ButtonGroupItemAdd = null!;
    public System.Windows.Forms.Button ButtonGroupItemDel = null!;
    public System.Windows.Forms.Button ButtonGroupItemChg = null!;
    public System.Windows.Forms.Button ButtonGroupItemSave = null!;
    public System.Windows.Forms.Button ButtonGroupItemSkillPower = null!;

    /// <summary>GroupBoxGroupItem 内 16 个标志复选框（下标=Tag 1..16）。</summary>
    public readonly System.Windows.Forms.CheckBox?[] GroupItemChecks = new System.Windows.Forms.CheckBox?[17];

    /// <summary>GroupBoxGroupItem 内 5 个扩展复选框（Tag 16..20，与标志同名不同位）。</summary>
    public readonly System.Windows.Forms.CheckBox?[] GroupItemExtraChecks = new System.Windows.Forms.CheckBox?[21];

    /// <summary>TabSheetRate 内 19 个比率控件（下标=Tag 0..18）。</summary>
    public readonly System.Windows.Forms.NumericUpDown?[] GroupItemRates = new System.Windows.Forms.NumericUpDown?[19];

    /// <summary>TabSheetValue 内 20 个数值控件（下标=Tag 0..19）。</summary>
    public readonly System.Windows.Forms.NumericUpDown?[] GroupItemValues = new System.Windows.Forms.NumericUpDown?[20];

    // ==================== WIL 名单页（TabSheet8） ====================

    public System.Windows.Forms.ListBox ListBoxWilNameList = null!;
    public System.Windows.Forms.TextBox EditWilName = null!;
    public System.Windows.Forms.Label LabelFileIndex = null!;
    public System.Windows.Forms.Button btnWilNameUP = null!;
    public System.Windows.Forms.Button btnWilNameDown = null!;
    public System.Windows.Forms.Button btnWilAdd = null!;
    public System.Windows.Forms.Button btnWilDel = null!;
    public System.Windows.Forms.Button btnWilSave = null!;
    public System.Windows.Forms.Button btnWilEdit = null!;
    public System.Windows.Forms.Button ButtonSendEffectImageList = null!;

    /// <summary>UserEngine.SendEffectImageList 接缝。</summary>
    public Action? SendEffectImageListHandler;

    // ==================== 装备技能威力页 ====================

    public System.Windows.Forms.ListBox ListBoxitemList5 = null!;
    public System.Windows.Forms.ListBox ListBoxSkillPowerItem = null!;
    public System.Windows.Forms.GroupBox GroupBoxSkillPowerItem = null!;
    public System.Windows.Forms.Button ButtonAddSkillPowerItem = null!;
    public System.Windows.Forms.Button ButtonChgSkillPowerItem = null!;
    public System.Windows.Forms.Button ButtonDelSkillPowerItem = null!;
    public System.Windows.Forms.Button ButtonSaveSkillPowerItem = null!;
    public System.Windows.Forms.CheckBox chkSkillPowerItemUseHum = null!;
    public System.Windows.Forms.CheckBox chkSkillPowerItemUseMon = null!;

    /// <summary>SelSkillPowerItem（pTSkillPowerItem）。</summary>
    public TSkillPowerItem? SelSkillPowerItem;

    /// <summary>g_SkillPowerItemList（Delphi M2Share 单元全局，窗体持有便于测试）。</summary>
    public readonly TSkillPowerItemList g_SkillPowerItemList = new();

    /// <summary>
    /// StringGridSkillPower 的当前单元格文本（[0..2, 0..550]，行 0 为表头）。
    /// Delphi RowCount = DEF_MAGIC_COUNT + CUSTOM_MAGIC_COUNT = 550，行号 = 技能 ID
    /// （自定义技能 = 1000 + 行内偏移）。
    /// </summary>
    public readonly string[,] SkillPowerGrid = new string[3, TGroupItems.SKILL_POWER_MAGIC_COUNT + 1];

    /// <summary>ShowFrmGroupItemSkillPower 接缝（返回 true 等效 mrOK）。</summary>
    public Func<bool>? ShowGroupItemSkillPowerHandler;

    // ==================== 物品过滤/备注页（复选框） ====================

    public System.Windows.Forms.CheckBox chkSendFilterItemList = null!;
    public System.Windows.Forms.CheckBox chkSendItemDescList = null!;
    public System.Windows.Forms.CheckBox chkSendItemDescTopList = null!;
    public System.Windows.Forms.CheckBox chkSendTzItemDescList = null!;
    public System.Windows.Forms.CheckBox chkSingleHint = null!;
    public System.Windows.Forms.CheckBox chkTZSupportRenameItem = null!;
    public System.Windows.Forms.CheckBox chkDescSupportRenamItem = null!;
    public System.Windows.Forms.CheckBox chkNoRenameDescReadDefault = null!;
    public System.Windows.Forms.CheckBox chkEnablePlayerUseClientPickItems = null!;
    public System.Windows.Forms.CheckBox chkEnableHeroUseClientPickItems = null!;
    public System.Windows.Forms.CheckBox chkEnabledBuyShopItemGive = null!;

    /// <summary>VMProtect 键控 g_nKey_UseClientPickItems（校验通过=1 才可勾选与写盘）。</summary>
    public int g_nKey_UseClientPickItems = 1;

    /// <summary>boOpened（Open 期间与之后为 True；未打开时部分处理器早退）。</summary>
    public bool boOpened;

    /// <summary>Config.WriteBool 接缝。</summary>
    public Action<string, string, bool>? WriteBoolHandler;

    /// <summary>UserEngine.SendServerConfig 接缝。</summary>
    public Action? SendServerConfigHandler;

    /// <summary>InputQuery 接缝（Ctrl+F 物品查找）。</summary>
    public Func<string, string, string, (bool Ok, string Value)>? InputQueryHandler;

    // ========================================================================
    //  物品规则
    // ========================================================================

    /// <summary>FormCreate（4910-4951）：构建 40 复选框 → 右键菜单（批量设置/批量取消）+ 拍卖货币 5 项。</summary>
    public void ItemRuleFormCreate()
    {
        ItemRuleMenuItems.Clear();
        for (int tag = 0; tag <= 40; tag++)
        {
            var chk = ItemRuleChecks[tag];
            if (chk == null || chk.Tag is not int t || t < 0)
                continue;
            ItemRuleMenuItems.Add((chk.Text, t));
        }

        cbbAuctionPricesType.Items.Clear();
        cbbAuctionPricesType.Items.Add(M2Config.sGameGoldName);
        cbbAuctionPricesType.Items.Add(M2Config.sGamePointName);
        cbbAuctionPricesType.Items.Add("金币");
        cbbAuctionPricesType.Items.Add(M2Config.sGameDiamondName);
        cbbAuctionPricesType.Items.Add(M2Config.sGameGirdName);
        cbbAuctionPricesType.SelectedIndex = 0;
    }

    /// <summary>pmRuleList 菜单项（Caption + Tag）。</summary>
    public readonly List<(string Caption, int Tag)> ItemRuleMenuItems = new();

    /// <summary>RefItemRuleList（1081-1097）：规则列表重装。</summary>
    public void RefItemRuleList()
    {
        ListBoxItemRuleList.Items.Clear();
        for (int i = 0; i < ViewList2State.g_ItemRules.Count; i++)
        {
            var itemRule = ViewList2State.g_ItemRules.GetItems(i);
            ListBoxItemRuleList.Items.Add(itemRule.ItemName);
        }
    }

    /// <summary>
    /// 选中规则行（Delphi ListBoxItemRuleListClick 的调用路径；WinForms 同索引重复赋值不触发
    /// SelectedIndexChanged，故显式暴露入口，与真实点击等价）。
    /// </summary>
    public void SelectItemRuleRow(int index)
    {
        if (index >= 0 && index < ListBoxItemRuleList.Items.Count)
        {
            ListBoxItemRuleList.ClearSelected();
            ListBoxItemRuleList.SetSelected(index, true);
            ListBoxItemRuleListClick();
        }
    }

    /// <summary>ListBoxItemRuleListClick（2116-2151）：选中回填 41 标志与拍卖价格区间，使能改/删。</summary>
    public void ListBoxItemRuleListClick()
    {
        int index = ListBoxItemRuleList.SelectedIndex;
        if (index >= 0)
        {
            SelItemRule = ViewList2State.g_ItemRules.GetItems(index);
            EditRuleItemName.Text = SelItemRule.ItemName;
            for (int i = 0; i <= 40; i++)
            {
                var chk = ItemRuleChecks[i];
                if (chk != null)
                {
                    chk.Checked = SelItemRule.FlagArray[i];
                }
            }

            cbbAuctionPricesType.SelectedIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                FAcutionPricesLimeMin[i] = SelItemRule.PricesMin[i];
                FAcutionPricesLimeMax[i] = SelItemRule.PricesMax[i];
            }


            seAuctionPrices_Min.Value = ClampU(FAcutionPricesLimeMin[cbbAuctionPricesType.SelectedIndex], seAuctionPrices_Min);
            seAuctionPrices_Max.Value = ClampU(FAcutionPricesLimeMax[cbbAuctionPricesType.SelectedIndex], seAuctionPrices_Max);

            ButtonItemRuleChg.Enabled = true;
            ButtonItemRuleDel.Enabled = true;
        }
        else
        {
            SelItemRule = null;
            ButtonItemRuleChg.Enabled = false;
            ButtonItemRuleDel.Enabled = false;
        }
    }

    private static decimal ClampU(uint value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp((decimal)value, control.Minimum, control.Maximum);

    /// <summary>ButtonItemRuleSelAllClick（2153-2167）1:1：仅使能项置真。</summary>
    public void ButtonItemRuleSelAllClick()
    {
        for (int i = 0; i <= 40; i++)
        {
            var chk = ItemRuleChecks[i];
            if (chk != null && chk.Enabled)
                chk.Checked = true;
        }
    }

    /// <summary>ButtonItemRuleNotSelAllClick（2169-2183）1:1。</summary>
    public void ButtonItemRuleNotSelAllClick()
    {
        for (int i = 0; i <= 40; i++)
        {
            var chk = ItemRuleChecks[i];
            if (chk != null && chk.Enabled)
                chk.Checked = false;
        }
    }

    /// <summary>ButtonItemRuleAddClick（2185-2254）：多选逐个添加；空名/重名/区间倒置三校验。</summary>
    public void ButtonItemRuleAddClick()
    {
        for (int j = 0; j < ListBoxItemList2.Items.Count; j++)
        {
            if (!ListBoxItemList2.GetSelected(j))
                continue;
            string sItemName = ListBoxItemList2.Items[j]?.ToString() ?? "";
            if (sItemName.Length == 0)
            {
                M2Forms.ErrorBox("请输入物品名称！");
                EditRuleItemName.Focus();
                return;
            }
            if (ViewList2State.g_ItemRules.Find(sItemName) != null)
            {
                M2Forms.ErrorBox("物品 " + sItemName + " 已经存在！");
                EditRuleItemName.Focus();
                continue;
            }
            SyncAuctionPricesFromControls();
            if (!ValidateAuctionPrices())
                return;

            var flagArray = new TFlagArray();
            for (int i = 0; i <= 40; i++)
            {
                var chk = ItemRuleChecks[i];
                if (chk != null)
                    flagArray[i] = chk.Checked;
            }

            // Delphi: g_ItemRules.Add(sItemName, FlagArray, @FAcutionPricesLime)
            // Delphi 的 GetStdItemIdx 来自 UserEngine 全局；托管侧由窗体接缝注入到规则表
            ViewList2State.g_ItemRules.GetStdItemIdxHandler = GetStdItemIdxHandler;
            TItemRule? itemRule = ViewList2State.g_ItemRules.Add(sItemName, flagArray,
                (FAcutionPricesLimeMin, FAcutionPricesLimeMax));
            if (itemRule != null)
            {
                ListBoxItemRuleList.Items.Add(itemRule.ItemName);
                ButtonItemRuleSave.Enabled = true;
                SelItemRule = itemRule;
                ListBoxItemRuleList.SelectedIndex = ListBoxItemRuleList.Items.Count - 1;
            }
            else
            {
                M2Forms.ErrorBox("增加失败！");
            }
        }
    }

    /// <summary>
    /// 把 seAuctionPrices_Min/Max 当前值同步进 FAcutionPricesLime（等效 Delphi 的 OnChange 写回；
    /// WinForms 无句柄时 ValueChanged 不触发，故在取用处显式同步）。
    /// </summary>
    public void SyncAuctionPricesFromControls()
    {
        int idx = cbbAuctionPricesType.SelectedIndex;
        if (idx < 0 || idx > 4)
            return;
        FAcutionPricesLimeMin[idx] = (uint)seAuctionPrices_Min.Value;
        FAcutionPricesLimeMax[idx] = (uint)seAuctionPrices_Max.Value;
    }

    /// <summary>区间倒置校验（Min/Max 均非 0 且 Min &gt; Max → 弹窗 + 回填 + 焦点 + 早退）。</summary>
    private bool ValidateAuctionPrices()
    {
        for (int i = 0; i < 5; i++)
        {
            if (FAcutionPricesLimeMin[i] != 0 && FAcutionPricesLimeMax[i] != 0 && FAcutionPricesLimeMin[i] > FAcutionPricesLimeMax[i])
            {
                cbbAuctionPricesType.SelectedIndex = i;
                M2Forms.ErrorBox("拍卖最低价不能大于最高价！");
                seAuctionPrices_Min.Value = ClampU(FAcutionPricesLimeMin[i], seAuctionPrices_Min);
                seAuctionPrices_Max.Value = ClampU(FAcutionPricesLimeMax[i], seAuctionPrices_Max);
                seAuctionPrices_Max.Focus();
                return false;
            }
        }
        return true;
    }

    /// <summary>ButtonItemRuleChgClick（2256-2298）：写入选中规则的标志与价格区间。</summary>
    public void ButtonItemRuleChgClick()
    {
        if (SelItemRule == null)
        {
            M2Forms.ErrorBox("请选择一个需要修改的物品！");
            ListBoxItemRuleList.Focus();
            return;
        }
        SyncAuctionPricesFromControls();
        if (!ValidateAuctionPrices())
            return;

        for (int i = 0; i <= 40; i++)
        {
            var chk = ItemRuleChecks[i];
            if (chk != null)
                SelItemRule.FlagArray[i] = chk.Checked;
        }
        for (int i = 0; i < 5; i++)
        {
            SelItemRule.PricesMin[i] = FAcutionPricesLimeMin[i];
            SelItemRule.PricesMax[i] = FAcutionPricesLimeMax[i];
        }
        ButtonItemRuleSave.Enabled = true;
    }

    /// <summary>ButtonItemRuleAddAllClick（2300-2351）：全表批量添加（已存在的跳过）。</summary>
    public void ButtonItemRuleAddAllClick()
    {
        // Delphi 原文 sItemName 未初始化 → ''（空串）；Find('') 在 GetStdItemIdx 失败时返回 nil，故不死循环
        string sItemName = "";
        if (ViewList2State.g_ItemRules.Find(sItemName) != null)
        {
            M2Forms.ErrorBox("此物品已经存在！");
            EditRuleItemName.Focus();
            return;
        }
        ViewList2State.g_ItemRules.GetStdItemIdxHandler = GetStdItemIdxHandler;
        SyncAuctionPricesFromControls();   // Delphi: ButtonItemRuleAddAllClick 也在 Add 前用 FAcutionPricesLime
        if (!ValidateAuctionPrices())
            return;

        var flagArray = new TFlagArray();
        for (int i = 0; i <= 40; i++)
        {
            var chk = ItemRuleChecks[i];
            if (chk != null)
                flagArray[i] = chk.Checked;
        }

        for (int i = 0; i < ListBoxItemList2.Items.Count; i++)
        {
            string name = ListBoxItemList2.Items[i]?.ToString() ?? "";
            if (ViewList2State.g_ItemRules.Find(name) == null)
            {
                var added = ViewList2State.g_ItemRules.Add(name, flagArray, (FAcutionPricesLimeMin, FAcutionPricesLimeMax));

            }
        }
        RefItemRuleList();
        ButtonItemRuleSave.Enabled = true;
    }

    /// <summary>ButtonItemRuleDelAllClick（2353-2361）。</summary>
    public void ButtonItemRuleDelAllClick()
    {
        ButtonItemRuleDelAll.Enabled = false;
        SelItemRule = null;
        ViewList2State.g_ItemRules.Clear();
        RefItemRuleList();
        ButtonItemRuleDelAll.Enabled = true;
        ButtonItemRuleSave.Enabled = true;
    }

    /// <summary>ButtonItemRuleDelClick（2363-2382）。</summary>
    public void ButtonItemRuleDelClick()
    {
        if (SelItemRule == null)
        {
            M2Forms.ErrorBox("请选择一个需要修改的物品！");
            ListBoxItemRuleList.Focus();
            return;
        }
        if (ViewList2State.g_ItemRules.Delete(SelItemRule.ItemName))
        {
            SelItemRule = null;
            RefItemRuleList();
            ButtonItemRuleSave.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("删除失败！");
        }
    }

    /// <summary>cbbAuctionPricesTypeChange（5113-5118）。</summary>
    public void CbbAuctionPricesTypeChange()
    {
        // Delphi ComboBox 空白时 ItemIndex = -1 会数组越界（设计期已 ItemIndex=0）；
        // 托管侧以 0 兜底等价值，避免未打开窗体时 OnChange 早触发。
        int idx = cbbAuctionPricesType.SelectedIndex;
        if (idx < 0) idx = 0;
        if (idx > 4) return;
        seAuctionPrices_Min.Value = ClampU(FAcutionPricesLimeMin[idx], seAuctionPrices_Min);
        seAuctionPrices_Max.Value = ClampU(FAcutionPricesLimeMax[idx], seAuctionPrices_Max);
    }

    /// <summary>seAuctionPrices_MinChange（5120-5123）。</summary>
    public void SeAuctionPricesMinChange()
    {
        int idx = cbbAuctionPricesType.SelectedIndex;
        if (idx < 0 || idx > 4) return;
        FAcutionPricesLimeMin[idx] = (uint)seAuctionPrices_Min.Value;
    }

    /// <summary>seAuctionPrices_MaxChange（5125-5128）。</summary>
    public void SeAuctionPricesMaxChange()
    {
        int idx = cbbAuctionPricesType.SelectedIndex;
        if (idx < 0 || idx > 4) return;
        FAcutionPricesLimeMax[idx] = (uint)seAuctionPrices_Max.Value;
    }

    /// <summary>OnItemRuleBathSettingClick（4953-4984）：Tag = Index*1000 + Value。</summary>
    public void OnItemRuleBathSettingClick(int tag)
    {
        int value = tag % 1000;
        int index = (tag - value) / 1000;
        if (index < 0 || index >= 41)
            return;

        for (int i = 0; i < ListBoxItemRuleList.Items.Count; i++)
        {
            if (!ListBoxItemRuleList.GetSelected(i))
                continue;
            var itemRule = ViewList2State.g_ItemRules.Find(ListBoxItemRuleList.Items[i]?.ToString() ?? "");
            if (itemRule != null)
                itemRule.FlagArray[index] = value != 0;
        }
        RefItemRuleList();
        ButtonItemRuleSave.Enabled = false;
        ViewList2State.g_ItemRules.SaveToEnvirFile();
    }
}
