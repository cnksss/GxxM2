using System;
using System.Collections.Generic;
using System.Globalization;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList2.pas TFrmViewList2 第三片（批次J57）：用户命令页、宝箱页、套装组页、WIL 名单页、
/// 装备技能威力页与物品过滤/备注复选框 1:1。
/// </summary>
public sealed partial class ViewList2Form
{
    // ==================== 用户命令页 ====================

    /// <summary>RefUserCommandList（1200-1209）。</summary>
    public void RefUserCommandList()
    {
        ListBoxUserCommand.Items.Clear();
        foreach (var (cmd, _) in ViewList2State.g_UserCmds.Snapshot())
            ListBoxUserCommand.Items.Add(cmd);
    }

    /// <summary>ListBoxUserCommandClick（2384-2399）：回填命令名/编号与提示标签。</summary>
    public void ListBoxUserCommandClick()
    {
        int index = ListBoxUserCommand.SelectedIndex;
        if (index >= 0)
        {
            string cmd = ListBoxUserCommand.Items[index]?.ToString() ?? "";
            int idx = ViewList2State.g_UserCmds.Get(cmd);
            EditCommandName.Text = cmd;
            EditCommandIdx.Value = Math.Clamp(idx, EditCommandIdx.Minimum, EditCommandIdx.Maximum);
            ButtonUserCommandDel.Enabled = true;
            LabelMsg.Text = string.Format(CultureInfo.InvariantCulture,
                "输入“@{0}”触发 QFunction-0.txt 脚本中的 [@UserCmd{1}] 字段", cmd, idx);
        }
        else
        {
            ButtonUserCommandDel.Enabled = false;
            LabelMsg.Text = "";
        }
    }

    /// <summary>ButtonUserCommandAddClick（2401-2429）。</summary>
    public void ButtonUserCommandAddClick()
    {
        string sCmdName = EditCommandName.Text.Trim();
        int nCmdIndex = (int)EditCommandIdx.Value;
        if (ViewList2State.g_UserCmds.Find(sCmdName))
        {
            M2Forms.ErrorBox("此命令名称已经存在！");
            EditCommandName.Focus();
            return;
        }
        if (ViewList2State.g_UserCmds.Find(nCmdIndex))
        {
            M2Forms.ErrorBox("此命令编号已经存在！");
            EditCommandIdx.Focus();
            return;
        }
        if (ViewList2State.g_UserCmds.Add(sCmdName, nCmdIndex))
        {
            RefUserCommandList();
            ButtonUserCommandSave.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("增加失败！");
        }
    }

    /// <summary>ButtonUserCommandDelClick（2431-2460）：先按名删，失败再按编号删。</summary>
    public void ButtonUserCommandDelClick()
    {
        string sCmdName = EditCommandName.Text.Trim();
        int nCmdIndex = (int)EditCommandIdx.Value;
        if (ViewList2State.g_UserCmds.Find(sCmdName))
        {
            if (ViewList2State.g_UserCmds.Delete(sCmdName))
            {
                RefUserCommandList();
                ButtonUserCommandSave.Enabled = true;
                ButtonUserCommandDel.Enabled = false;
            }
            else if (ViewList2State.g_UserCmds.Delete(nCmdIndex))
            {
                RefUserCommandList();
                ButtonUserCommandSave.Enabled = true;
                ButtonUserCommandDel.Enabled = false;
            }
            else
            {
                M2Forms.ErrorBox("删除失败！");
            }
        }
    }

    // ==================== 宝箱页 ====================

    /// <summary>RefBoxList（1154-1198）：四类物品列表 + 8 项宝箱配置回填。</summary>
    public void RefBoxList(int source)
    {
        ListBoxGiveItem.Items.Clear();
        ListBoxCenterItem.Items.Clear();
        ListBoxNoGiveItem.Items.Clear();
        ListBoxEndNoGiveItem.Items.Clear();
        var box = ViewList2State.g_BoxsList.Find(source);
        if (box != null)
        {
            chkNext.Checked = box.BoxSet.boNext;
            seGold.Value = ClampI(box.BoxSet.nGold, seGold);
            seGameGold.Value = ClampI(box.BoxSet.nGameGold, seGameGold);
            seAddGold.Value = ClampI(box.BoxSet.nAddGold, seAddGold);
            seAddGameGold.Value = ClampI(box.BoxSet.nAddGameGold, seAddGameGold);
            seEndGold.Value = ClampI(box.BoxSet.nEndGold, seEndGold);
            seEndGameGold.Value = ClampI(box.BoxSet.nEndGameGold, seEndGameGold);
            seCount.Value = ClampI(box.BoxSet.nCount, seCount);

            for (int i = 0; i < box.Give.Count; i++)
                ListBoxGiveItem.Items.Add(box.Give.GetItems(i).ItemName);
            for (int i = 0; i < box.Center.Count; i++)
                ListBoxCenterItem.Items.Add(box.Center.GetItems(i).ItemName);
            for (int i = 0; i < box.NoGive.Count; i++)
                ListBoxNoGiveItem.Items.Add(box.NoGive.GetItems(i).ItemName);
            // 永不可得物品 piaoyun 2013-08-23
            for (int i = 0; i < box.EndNoGive.Count; i++)
                ListBoxEndNoGiveItem.Items.Add(box.EndNoGive.GetItems(i).ItemName);
        }
    }

    private static decimal ClampI(int value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp((decimal)value, control.Minimum, control.Maximum);

    /// <summary>ListBoxBoxItemClick（1770-1795）：选中宝箱 → 标题 + 列表回填 + 按钮态。</summary>
    public void ListBoxBoxItemClick()
    {
        int index = ListBoxBoxItem.SelectedIndex;
        if (index >= 0)
        {
            // Delphi: g_BoxsList.Find(pTStdItem(Items.Objects[Index]).Source)
            int source = index;
            SelBox = ViewList2State.g_BoxsList.Find(source);
            if (SelBox != null)
            {
                GroupBoxBoxItem.Text = SelBox.Name;
                RefBoxList(SelBox.Source);
                btnAddBoxItem.Enabled = true;
                btnDelBoxItem.Enabled = false;
            }
            else
            {
                SelBox = null;
                btnAddBoxItem.Enabled = false;
                btnDelBoxItem.Enabled = false;
            }
        }
        else
        {
            SelBox = null;
            btnAddBoxItem.Enabled = false;
            btnDelBoxItem.Enabled = false;
        }
    }

    /// <summary>btnAddBoxItemClick（1797-1867）1:1。</summary>
    public void BtnAddBoxItemClick()
    {
        if (SelBox == null)
        {
            M2Forms.ErrorBox("请在宝箱列表中选择一个物品！");
            ListBoxBoxItem.Focus();
            return;
        }

        // 宝箱物品支持物品数量 + chongchong 2014-01-06
        int nItemCount = (int)seBoxItemCount.Value;
        string sItemName;
        if (edtBoxItemName.Text.Trim().Length > 0
            && edtBoxItemName.Text.Trim() == (cbbOtherItem.SelectedIndex >= 0 ? cbbOtherItem.Items[cbbOtherItem.SelectedIndex]?.ToString() : null))
        {
            sItemName = edtBoxItemName.Text.Trim();
        }
        else
        {
            if (ListBoxitemList1.SelectedIndex < 0)
            {
                M2Forms.ErrorBox("请在物品列表中选择一个物品！");
                ListBoxitemList1.Focus();
                return;
            }
            sItemName = (ListBoxitemList1.Items[ListBoxitemList1.SelectedIndex]?.ToString() ?? "").Trim();
        }

        if (ComboBoxBoxItemType.SelectedIndex is < 0 or > 3)
        {
            M2Forms.ErrorBox("请选择物品种类！");
            ComboBoxBoxItemType.Focus();
            return;
        }

        TBoxList boxList = ComboBoxBoxItemType.SelectedIndex switch
        {
            0 => SelBox.Give,      // 可得
            1 => SelBox.NoGive,    // 不可得
            2 => SelBox.Center,    // 中间一格
            _ => SelBox.EndNoGive, // 永不可得
        };

        if (boxList.GetByName(sItemName, nItemCount) != null)
        {
            M2Forms.ErrorBox("该物品已经在列表中了！");
            edtBoxItemName.Focus();
            return;
        }

        boxList.Add(new TBoxItem { ItemName = sItemName, ItemCount = nItemCount });
        RefBoxList(SelBox.Source);
        btnSaveBoxItem.Enabled = true;
    }

    /// <summary>btnDelBoxItemClick（1869-1941）1:1。</summary>
    public void BtnDelBoxItemClick()
    {
        if (ComboBoxBoxItemType.SelectedIndex is < 0 or > 3)
        {
            M2Forms.ErrorBox("请选择物品种类！");
            ComboBoxBoxItemType.Focus();
            return;
        }
        if (SelBox == null)
        {
            M2Forms.ErrorBox("请选择一个宝箱！");
            ListBoxBoxItem.Focus();
            return;
        }

        TBoxList boxList;
        TBoxItem? boxItem = null;
        switch (ComboBoxBoxItemType.SelectedIndex)
        {
            case 0:
                boxList = SelBox.Give;
                if (ListBoxGiveItem.SelectedIndex >= 0)
                    boxItem = boxList.GetByName(ListBoxGiveItem.Items[ListBoxGiveItem.SelectedIndex]?.ToString() ?? "");
                break;
            case 1:
                boxList = SelBox.NoGive;
                if (ListBoxNoGiveItem.SelectedIndex >= 0)
                    boxItem = boxList.GetByName(ListBoxNoGiveItem.Items[ListBoxNoGiveItem.SelectedIndex]?.ToString() ?? "");
                break;
            case 2:
                boxList = SelBox.Center;
                if (ListBoxCenterItem.SelectedIndex >= 0)
                    boxItem = boxList.GetByName(ListBoxCenterItem.Items[ListBoxCenterItem.SelectedIndex]?.ToString() ?? "");
                break;
            default:
                boxList = SelBox.EndNoGive;
                if (ListBoxEndNoGiveItem.SelectedIndex >= 0)
                    boxItem = boxList.GetByName(ListBoxEndNoGiveItem.Items[ListBoxEndNoGiveItem.SelectedIndex]?.ToString() ?? "");
                break;
        }

        string sItemName = boxItem?.ItemName ?? "";
        int nItemCount = boxItem?.ItemCount ?? 0;

        if (sItemName.Length == 0)
        {
            M2Forms.ErrorBox("请选择要删除的物品！");
            return;
        }
        boxList.DeleteByName(sItemName, nItemCount);
        RefBoxList(SelBox.Source);
        btnSaveBoxItem.Enabled = true;
        btnDelBoxItem.Enabled = false;
    }

    /// <summary>btnSaveBoxItemClick（1943-1947）。</summary>
    public void BtnSaveBoxItemClick()
    {
        ViewList2State.g_BoxsList.SaveToFile();
        btnSaveBoxItem.Enabled = false;
    }

    /// <summary>四类列表 Click（3959/3981/4003/4634）：回填 edtBoxItemName + 数量 + 类型 + 删除使能。</summary>
    public void BoxItemListClick(int listKind)
    {
        System.Windows.Forms.ListBox listBox = listKind switch
        {
            0 => ListBoxGiveItem,
            1 => ListBoxCenterItem,
            2 => ListBoxNoGiveItem,
            _ => ListBoxEndNoGiveItem,
        };
        int index = listBox.SelectedIndex;
        if (index >= 0)
        {
            var boxItem = SelBox != null
                ? (listKind switch
                {
                    0 => SelBox.Give.GetByName(listBox.Items[index]?.ToString() ?? ""),
                    1 => SelBox.Center.GetByName(listBox.Items[index]?.ToString() ?? ""),
                    2 => SelBox.NoGive.GetByName(listBox.Items[index]?.ToString() ?? ""),
                    _ => SelBox.EndNoGive.GetByName(listBox.Items[index]?.ToString() ?? ""),
                })
                : null;
            if (boxItem != null)
            {
                edtBoxItemName.Text = boxItem.ItemName;
                // Delphi: seBoxItemCount.OnChange := nil; Value := ...; OnChange := seBoxItemCountChange
                SeBoxItemCountSetValue(boxItem.ItemCount);
            }
            ComboBoxBoxItemType.SelectedIndex = listKind switch { 0 => 0, 1 => 2, 2 => 1, _ => 3 };
            btnDelBoxItem.Enabled = true;
        }
        else
        {
            btnDelBoxItem.Enabled = false;
        }
    }

    /// <summary>seBoxItemCount 赋值（抑制 OnChange 的 Delphi 语义等效）。</summary>
    public bool SuppressSeBoxItemCountChange;

    public void SeBoxItemCountSetValue(int value)
    {
        SuppressSeBoxItemCountChange = true;
        seBoxItemCount.Value = ClampI(value, seBoxItemCount);
        SuppressSeBoxItemCountChange = false;
    }

    /// <summary>cbbOtherItemChange（4656-4659）。</summary>
    public void CbbOtherItemChange()
    {
        if (cbbOtherItem.SelectedIndex >= 0)
            edtBoxItemName.Text = cbbOtherItem.Items[cbbOtherItem.SelectedIndex]?.ToString() ?? "";
    }

    /// <summary>seBoxItemCountChange（4785-）：把数量写回当前列表现有项。</summary>
    public void SeBoxItemCountChange()
    {
        if (SuppressSeBoxItemCountChange)
            return;
        int listKind = ComboBoxBoxItemType.SelectedIndex;
        if (listKind is < 0 or > 3)
            return;
        var listBox = listKind switch { 0 => ListBoxGiveItem, 1 => ListBoxNoGiveItem, 2 => ListBoxCenterItem, _ => ListBoxEndNoGiveItem };
        int index = listBox.SelectedIndex;
        if (index < 0 || SelBox == null)
            return;
        var boxList = listKind switch { 0 => SelBox.Give, 1 => SelBox.NoGive, 2 => SelBox.Center, _ => SelBox.EndNoGive };
        var item = boxList.GetByName(listBox.Items[index]?.ToString() ?? "");
        if (item != null)
            item.ItemCount = (int)seBoxItemCount.Value;
    }

    /// <summary>chkNextClick（4624-4632）：联动 seCount 并使能 + 写回。</summary>
    public void ChkNextClick()
    {
        seCount.Enabled = chkNext.Checked;
        if (SelBox != null)
        {
            SelBox.BoxSet.boNext = chkNext.Checked;
            btnSaveBoxItem.Enabled = true;
        }
    }

    /// <summary>宝箱 8 项配置 Change（4661-4724）：写回 SelBox.BoxSet 并使能保存。</summary>
    public void BoxSetFieldChanged(string field)
    {
        if (SelBox == null)
            return;
        switch (field)
        {
            case "Count": SelBox.BoxSet.nCount = (byte)seCount.Value; break;
            case "Gold": SelBox.BoxSet.nGold = (int)seGold.Value; break;
            case "GameGold": SelBox.BoxSet.nGameGold = (int)seGameGold.Value; break;
            case "AddGold": SelBox.BoxSet.nAddGold = (int)seAddGold.Value; break;
            case "AddGameGold": SelBox.BoxSet.nAddGameGold = (int)seAddGameGold.Value; break;
            case "EndGold": SelBox.BoxSet.nEndGold = (int)seEndGold.Value; break;
            case "EndGameGold": SelBox.BoxSet.nEndGameGold = (int)seEndGameGold.Value; break;
        }
        btnSaveBoxItem.Enabled = true;
    }

    // ==================== 套装组页 ====================

    /// <summary>RefGroupItemList（1211-1244）：四列（编号/说明/数量/物品名|）重装并保持选中。</summary>
    public void RefGroupItemList()
    {
        int nIndex = GetSelectedItem(ListViewGroupItemList)?.Index ?? -1;
        ListViewGroupItemList.Items.Clear();
        for (int i = 0; i < ViewList2State.g_GroupItems.Count; i++)
        {
            var groupItem = ViewList2State.g_GroupItems.GetItems(i);
            var item = ListViewGroupItemList.Items.Add(groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture));
            item.Tag = groupItem;
            item.SubItems.Add(groupItem.FLD_DESC);
            item.SubItems.Add(groupItem.FLD_COUNT.ToString(CultureInfo.InvariantCulture));
            string sItemName = "";
            for (int ii = 0; ii < groupItem.FLD_ITEMNAMES.Count; ii++)
                sItemName += groupItem.FLD_ITEMNAMES[ii] + "|";
            item.SubItems.Add(sItemName);
        }
        if (nIndex >= 0 && nIndex < ListViewGroupItemList.Items.Count)
            ListViewGroupItemList.Items[nIndex].Selected = true;
    }

    /// <summary>ButtonGroupItemAddClick（2462-2558）1:1。</summary>
    public void ButtonGroupItemAddClick()
    {
        if (ViewList2State.g_GroupItems.FindIndex((int)EditGroupItemIndex.Value))
        {
            M2Forms.ErrorBox("套装编号已经存在，请重新输入！");
            EditGroupItemIndex.Focus();
            return;
        }
        if (EditGroupItemDesc.Text.Length == 0)
        {
            M2Forms.ErrorBox("请输入套装说明！");
            EditGroupItemDesc.Focus();
            return;
        }
        if (EditGroupItemName.Text.Length == 0)
        {
            M2Forms.ErrorBox("请输入套装物品！");
            EditGroupItemName.Focus();
            return;
        }
        if (EditGroupItemCount.Value <= 0)
        {
            M2Forms.ErrorBox("套装数量输入不正确！");
            EditGroupItemCount.Focus();
            return;
        }

        var groupItem = new TGroupItemModel
        {
            FLD_INDEX = (int)EditGroupItemIndex.Value,
            FLD_COUNT = (int)EditGroupItemCount.Value,
            FLD_DESC = EditGroupItemDesc.Text,
            FLD_HINTMSG = EditGroupItemHint.Text.Trim(),
        };
        foreach (var name in TGroupItems.ExtractStrings('|', EditGroupItemName.Text.Trim()))
            groupItem.FLD_ITEMNAMES.Add(name);
        TGroupItems.TrimStringList(groupItem.FLD_ITEMNAMES);

        ReadGroupItemFlags(groupItem);
        ReadGroupItemRates(groupItem);
        ReadGroupItemValues(groupItem);

        if (ViewList2State.g_GroupItems.Add(groupItem))
        {
            var item = ListViewGroupItemList.Items.Add(groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture));
            item.Tag = groupItem;
            item.SubItems.Add(groupItem.FLD_DESC);
            item.SubItems.Add(groupItem.FLD_COUNT.ToString(CultureInfo.InvariantCulture));
            string sItemName = "";
            for (int i = 0; i < groupItem.FLD_ITEMNAMES.Count; i++)
                sItemName += groupItem.FLD_ITEMNAMES[i] + "|";
            item.SubItems.Add(sItemName);

            ViewList2State.SelGroupItem = groupItem;
            SelGroupItem = groupItem;
            item.Selected = true;
            ButtonGroupItemSave.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("增加失败！");
        }
    }

    /// <summary>SelGroupItem（窗体字段，与 ViewList2State.SelGroupItem 同步）。</summary>
    public TGroupItemModel? SelGroupItem;

    private void ReadGroupItemFlags(TGroupItemModel groupItem)
    {
        for (int i = 1; i <= 16; i++)
        {
            var chk = GroupItemChecks[i];
            if (chk != null)
                groupItem.FLD_FLAG[i] = chk.Checked;
        }
        for (int i = 16; i <= 20; i++)
        {
            var chk = GroupItemExtraChecks[i];
            if (chk != null)
                groupItem.FLD_FLAG[i] = chk.Checked;
        }
    }

    private void ReadGroupItemRates(TGroupItemModel groupItem)
    {
        for (int i = 0; i <= 18; i++)
        {
            var spin = GroupItemRates[i];
            if (spin != null)
                groupItem.FLD_RATE[i] = (int)spin.Value;
        }
    }

    private void ReadGroupItemValues(TGroupItemModel groupItem)
    {
        for (int i = 0; i <= 19; i++)
        {
            var spin = GroupItemValues[i];
            if (spin != null)
                groupItem.FLD_VALUE[i] = (int)spin.Value;
        }
    }

    /// <summary>选中套装行（Delphi ListViewGroupItemListClick；同索引重复赋值不触发 WinForms 事件，故显式入口）。</summary>
    public void SelectGroupItemRow(int index)
    {
        if (index >= 0 && index < ListViewGroupItemList.Items.Count)
        {
            foreach (System.Windows.Forms.ListViewItem it in ListViewGroupItemList.Items)
                it.Selected = false;
            ListViewGroupItemList.Items[index].Selected = true;
            ListViewGroupItemList.FocusedItem = ListViewGroupItemList.Items[index];
        }
        ListViewGroupItemListClick();
    }

    /// <summary>ListViewGroupItemListClick（2560-2621）：回填全部控件并使能删/改/技能威力。</summary>
    public void ListViewGroupItemListClick()
    {
        var listItem = GetSelectedItem(ListViewGroupItemList);
        if (listItem?.Tag is TGroupItemModel groupItem)
        {
            SelGroupItem = groupItem;
            ViewList2State.SelGroupItem = groupItem;
            EditGroupItemIndex.Value = Math.Clamp(groupItem.FLD_INDEX, EditGroupItemIndex.Minimum, EditGroupItemIndex.Maximum);
            EditGroupItemCount.Value = Math.Clamp(groupItem.FLD_COUNT, EditGroupItemCount.Minimum, EditGroupItemCount.Maximum);
            EditGroupItemHint.Text = groupItem.FLD_HINTMSG;
            EditGroupItemDesc.Text = groupItem.FLD_DESC;
            Array.Copy(groupItem.AttackSkillPercent, ViewList2State.SelAttackSkillPercent, 115);
            Array.Copy(groupItem.DefenseSkillPercent, ViewList2State.SelDefenseSkillPercent, 115);
            string sItemName = "";
            for (int i = 0; i < groupItem.FLD_ITEMNAMES.Count; i++)
                sItemName += groupItem.FLD_ITEMNAMES[i] + "|";
            EditGroupItemName.Text = sItemName;

            for (int i = 1; i <= 16; i++)
            {
                var chk = GroupItemChecks[i];
                if (chk != null)
                    chk.Checked = groupItem.FLD_FLAG[i];
            }
            for (int i = 16; i <= 20; i++)
            {
                var chk = GroupItemExtraChecks[i];
                if (chk != null)
                    chk.Checked = groupItem.FLD_FLAG[i];
            }
            for (int i = 0; i <= 18; i++)
            {
                var spin = GroupItemRates[i];
                if (spin != null)
                    spin.Value = Math.Clamp((decimal)groupItem.FLD_RATE[i], spin.Minimum, spin.Maximum);
            }
            for (int i = 0; i <= 19; i++)
            {
                var spin = GroupItemValues[i];
                if (spin != null)
                    spin.Value = Math.Clamp((decimal)groupItem.FLD_VALUE[i], spin.Minimum, spin.Maximum);
            }

            ButtonGroupItemDel.Enabled = true;
            ButtonGroupItemChg.Enabled = true;
            ButtonGroupItemSkillPower.Enabled = true;
        }
        else
        {
            SelGroupItem = null;
            ViewList2State.SelGroupItem = null;
            ButtonGroupItemDel.Enabled = false;
            ButtonGroupItemChg.Enabled = false;
            ButtonGroupItemSkillPower.Enabled = false;
        }
    }

    /// <summary>ButtonGroupItemDelClick（2623-2640）。</summary>
    public void ButtonGroupItemDelClick()
    {
        if (SelGroupItem == null)
            return;
        ButtonGroupItemDel.Enabled = false;
        if (ViewList2State.g_GroupItems.Delete(SelGroupItem))
        {
            SelGroupItem = null;
            ViewList2State.SelGroupItem = null;
            RefGroupItemList();
            ButtonGroupItemChg.Enabled = false;
            ButtonGroupItemSave.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("删除失败！");
        }
    }

    /// <summary>ButtonGroupItemChgClick（2642-2727）：技能威力先行回写，再改字段与列表行。</summary>
    public void ButtonGroupItemChgClick()
    {
        if (SelGroupItem == null)
        {
            M2Forms.MessageBox("请选择要修改的套装！", "提示信息", M2Forms.MB_OK | M2Forms.MB_ICONQUESTION);
            return;
        }

        Array.Copy(ViewList2State.SelAttackSkillPercent, SelGroupItem.AttackSkillPercent, 115);
        Array.Copy(ViewList2State.SelDefenseSkillPercent, SelGroupItem.DefenseSkillPercent, 115);
        ReadGroupItemFlags(SelGroupItem);
        ReadGroupItemRates(SelGroupItem);
        ReadGroupItemValues(SelGroupItem);

        if (EditGroupItemDesc.Text.Length == 0)
        {
            M2Forms.ErrorBox("请输入套装说明！");
            EditGroupItemDesc.Focus();
            return;
        }
        if (EditGroupItemName.Text.Length == 0)
        {
            M2Forms.ErrorBox("请输入套装物品！");
            EditGroupItemName.Focus();
            return;
        }
        if (EditGroupItemCount.Value <= 0)
        {
            M2Forms.ErrorBox("套装数量输入不正确！");
            EditGroupItemCount.Focus();
            return;
        }

        SelGroupItem.FLD_COUNT = (int)EditGroupItemCount.Value;
        SelGroupItem.FLD_DESC = EditGroupItemDesc.Text;
        SelGroupItem.FLD_HINTMSG = EditGroupItemHint.Text.Trim();
        SelGroupItem.FLD_ITEMNAMES.Clear();
        foreach (var name in TGroupItems.ExtractStrings('|', EditGroupItemName.Text.Trim()))
            SelGroupItem.FLD_ITEMNAMES.Add(name);
        TGroupItems.TrimStringList(SelGroupItem.FLD_ITEMNAMES);

        var listItem = GetSelectedItem(ListViewGroupItemList);
        if (listItem != null)
        {
            listItem.Text = SelGroupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture);
            listItem.SubItems[0].Text = SelGroupItem.FLD_DESC;
            listItem.SubItems[1].Text = SelGroupItem.FLD_COUNT.ToString(CultureInfo.InvariantCulture);
            string sItemName = "";
            for (int i = 0; i < SelGroupItem.FLD_ITEMNAMES.Count; i++)
                sItemName += SelGroupItem.FLD_ITEMNAMES[i] + "|";
            listItem.SubItems[2].Text = sItemName;
        }

        ButtonGroupItemSave.Enabled = true;
    }

    /// <summary>ButtonGroupItemSkillPowerClick（4483-4495）：弹出技能威力窗，确认后回写。</summary>
    public void ButtonGroupItemSkillPowerClick()
    {
        if (SelGroupItem == null)
            return;
        if (ShowGroupItemSkillPowerHandler?.Invoke() ?? false)
        {
            Array.Copy(ViewList2State.SelAttackSkillPercent, SelGroupItem.AttackSkillPercent, 115);
            Array.Copy(ViewList2State.SelDefenseSkillPercent, SelGroupItem.DefenseSkillPercent, 115);
            ButtonGroupItemSave.Enabled = true;
        }
    }

    /// <summary>RadioButtonRate/RadioButtonValue 保存分组（组保存 ButtonGroupItemSaveClick 4147）。</summary>
    public void ButtonGroupItemSaveClick()
    {
        ViewList2State.g_GroupItems.SaveToFile();
        ButtonGroupItemSave.Enabled = false;
    }

    // ==================== WIL 名单页 ====================

    /// <summary>btnWilAddClick（2923-2951）：空名与 CompareText 重名双校验。</summary>
    public void BtnWilAddClick()
    {
        string sWilName = EditWilName.Text.Trim();
        if (sWilName.Length == 0)
        {
            M2Forms.ErrorBox("请输入WIL文件名称！");
            EditWilName.Focus();
            return;
        }
        for (int i = 0; i < ListBoxWilNameList.Items.Count; i++)
        {
            if (string.Compare(ListBoxWilNameList.Items[i]?.ToString(), sWilName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                M2Forms.ErrorBox("此WIL文件名称已经在列表中了！");
                EditWilName.Focus();
                return;
            }
        }
        ListBoxWilNameList.Items.Add(sWilName);
        btnWilSave.Enabled = true;
    }

    /// <summary>btnWilNameUPClick（2953-2969）。</summary>
    public void BtnWilNameUpClick()
    {
        int itemIndex = ListBoxWilNameList.SelectedIndex;
        if (itemIndex > 0)
        {
            string sWilName = ListBoxWilNameList.Items[itemIndex]?.ToString() ?? "";
            ListBoxWilNameList.Items.RemoveAt(itemIndex);
            ListBoxWilNameList.Items.Insert(itemIndex - 1, sWilName);
            ListBoxWilNameList.SelectedIndex = itemIndex - 1;
            UpdateWilButtons();
            btnWilSave.Enabled = true;
        }
    }

    /// <summary>btnWilNameDownClick（2971-2987）。</summary>
    public void BtnWilNameDownClick()
    {
        int itemIndex = ListBoxWilNameList.SelectedIndex;
        if (itemIndex >= 0 && itemIndex < ListBoxWilNameList.Items.Count - 1)
        {
            string sWilName = ListBoxWilNameList.Items[itemIndex]?.ToString() ?? "";
            ListBoxWilNameList.Items.RemoveAt(itemIndex);
            ListBoxWilNameList.Items.Insert(itemIndex + 1, sWilName);
            ListBoxWilNameList.SelectedIndex = itemIndex + 1;
            UpdateWilButtons();
            btnWilSave.Enabled = true;
        }
    }

    private void UpdateWilButtons()
    {
        int idx = ListBoxWilNameList.SelectedIndex;
        btnWilNameDown.Enabled = idx >= 0 && idx < ListBoxWilNameList.Items.Count - 1;
        btnWilNameUP.Enabled = idx > 0;
    }

    /// <summary>ListBoxWilNameListClick（2989-3005）。</summary>
    public void ListBoxWilNameListClick()
    {
        int idx = ListBoxWilNameList.SelectedIndex;
        if (idx >= 0)
        {
            UpdateWilButtons();
            LabelFileIndex.Text = (ListBoxWilNameList.Items[idx]?.ToString() ?? "") + " 编号:" + idx.ToString(CultureInfo.InvariantCulture);
            EditWilName.Text = ListBoxWilNameList.Items[idx]?.ToString() ?? "";
            btnWilDel.Enabled = true;
            btnWilEdit.Enabled = true;
        }
        else
        {
            btnWilDel.Enabled = false;
            btnWilEdit.Enabled = false;
        }
    }

    /// <summary>btnWilSaveClick（3007-3032）：g_EffectImageList 同步 + 5 个下拉重建 + 落盘。</summary>
    public void BtnWilSaveClick()
    {
        ViewList2State.g_EffectImageList.Clear();
        for (int i = 0; i < ListBoxWilNameList.Items.Count; i++)
            ViewList2State.g_EffectImageList.Add(ListBoxWilNameList.Items[i]?.ToString() ?? "");

        for (int i = 0; i < EffectFileIndexCombos.Count; i++)
        {
            var cbb = EffectFileIndexCombos[i];
            cbb.Items.Clear();
            cbb.Items.Add("关闭特效");
            foreach (var name in ViewList2State.g_EffectImageList)
                cbb.Items.Add(name);
        }

        SaveEffectImageListHandler?.Invoke();
        btnWilSave.Enabled = false;
    }

    /// <summary>5 个特效文件下拉（cbbEffectFileIndex1/2/3/5 + cbbAddEffectFileIndex）。</summary>
    public readonly List<System.Windows.Forms.ComboBox> EffectFileIndexCombos = new();

    /// <summary>SaveEffectImageList 接缝。</summary>
    public Action? SaveEffectImageListHandler;

    /// <summary>ButtonSendEffectImageListClick（3034-3037）。</summary>
    public void ButtonSendEffectImageListClick() => SendEffectImageListHandler?.Invoke();

    /// <summary>btnWilDelClick（3039-3049）。</summary>
    public void BtnWilDelClick()
    {
        int idx = ListBoxWilNameList.SelectedIndex;
        if (idx >= 0)
        {
            ListBoxWilNameList.Items.RemoveAt(idx);
            btnWilDel.Enabled = false;
            btnWilSave.Enabled = true;
            UpdateWilButtons();
        }
    }

    /// <summary>btnWilEditClick（5026-）：就地改写选中项。</summary>
    public void BtnWilEditClick()
    {
        int idx = ListBoxWilNameList.SelectedIndex;
        if (idx < 0)
            return;
        string sWilName = EditWilName.Text.Trim();
        if (sWilName.Length == 0)
            return;
        for (int i = 0; i < ListBoxWilNameList.Items.Count; i++)
        {
            if (i != idx && string.Compare(ListBoxWilNameList.Items[i]?.ToString(), sWilName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                M2Forms.ErrorBox("此WIL文件名称已经在列表中了！");
                return;
            }
        }
        ListBoxWilNameList.Items[idx] = sWilName;
        LabelFileIndex.Text = sWilName + " 编号:" + idx.ToString(CultureInfo.InvariantCulture);
        btnWilSave.Enabled = true;
    }

    // ==================== 装备技能威力页 ====================

    /// <summary>ListBoxitemList5Click（4497-4500）。</summary>
    public void ListBoxitemList5Click() => ButtonAddSkillPowerItem.Enabled = true;

    /// <summary>ListBoxSkillPowerItemClick（4502-4521）：标题 + 115 行网格回填。</summary>
    public void ListBoxSkillPowerItemClick()
    {
        SelSkillPowerItem = null;
        ButtonDelSkillPowerItem.Enabled = false;
        int index = ListBoxSkillPowerItem.SelectedIndex;
        if (index >= 0)
        {
            string name = ListBoxSkillPowerItem.Items[index]?.ToString() ?? "";
            GroupBoxSkillPowerItem.Text = name + "的技能威力百分比设置";
            var skillPowerItem = g_SkillPowerItemList.GetSkillPowerItem(name);
            if (skillPowerItem != null)
            {
                for (int i = 1; i <= TGroupItems.SKILL_POWER_MAGIC_COUNT; i++)
                {
                    SkillPowerGrid[1, i] = skillPowerItem.AttackSkillPercent[i].ToString(CultureInfo.InvariantCulture);
                    SkillPowerGrid[2, i] = skillPowerItem.DefenseSkillPercent[i].ToString(CultureInfo.InvariantCulture);
                }
                SelSkillPowerItem = skillPowerItem;
                ButtonDelSkillPowerItem.Enabled = true;
            }
        }
    }

    /// <summary>ButtonDelSkillPowerItemClick（4523-4551）。</summary>
    public void ButtonDelSkillPowerItemClick()
    {
        if (SelSkillPowerItem == null)
            return;
        for (int i = 0; i < g_SkillPowerItemList.Count; i++)
        {
            if (ReferenceEquals(g_SkillPowerItemList.GetObjects(i), SelSkillPowerItem))
            {
                SelSkillPowerItem = null;
                g_SkillPowerItemList.DeleteAt(i);
                RefSkillPowerList();
                ButtonDelSkillPowerItem.Enabled = false;
                ButtonSaveSkillPowerItem.Enabled = true;
                break;
            }
        }
    }

    /// <summary>ButtonChgSkillPowerItemClick（4553-4567）：网格 → 选中项。</summary>
    public void ButtonChgSkillPowerItemClick()
    {
        if (SelSkillPowerItem == null)
            return;
        for (int i = 1; i <= TGroupItems.SKILL_POWER_MAGIC_COUNT; i++)
        {
            SelSkillPowerItem.AttackSkillPercent[i] = (byte)ParseGrid(SkillPowerGrid[1, i]);
            SelSkillPowerItem.DefenseSkillPercent[i] = (byte)ParseGrid(SkillPowerGrid[2, i]);
        }
        ButtonChgSkillPowerItem.Enabled = false;
        ButtonSaveSkillPowerItem.Enabled = true;
    }

    private static int ParseGrid(string? value)
        => int.TryParse((value ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;

    /// <summary>ButtonSaveSkillPowerItemClick（4569-4573）。</summary>
    public void ButtonSaveSkillPowerItemClick()
    {
        ButtonSaveSkillPowerItem.Enabled = false;
        g_SkillPowerItemList.SaveToFile();
    }

    /// <summary>ButtonAddSkillPowerItemClick（4575-4617）：重名拒绝 + 按网格新建。</summary>
    public void ButtonAddSkillPowerItemClick()
    {
        int index = ListBoxitemList5.SelectedIndex;
        if (index < 0)
            return;
        string sItemName = ListBoxitemList5.Items[index]?.ToString() ?? "";
        if (g_SkillPowerItemList.GetSkillPowerItem(sItemName) != null)
        {
            M2Forms.MessageBox("该物品已经在装备技能威力列表中了！", "提示信息", M2Forms.MB_OK | M2Forms.MB_ICONQUESTION);
            return;
        }

        var skillPowerItem = new TSkillPowerItem();
        for (int i = 1; i <= TGroupItems.SKILL_POWER_MAGIC_COUNT; i++)
        {
            skillPowerItem.AttackSkillPercent[i] = (byte)ParseGrid(SkillPowerGrid[1, i]);
            skillPowerItem.DefenseSkillPercent[i] = (byte)ParseGrid(SkillPowerGrid[2, i]);
        }
        g_SkillPowerItemList.Add(sItemName, skillPowerItem);
        RefSkillPowerList();
        ButtonSaveSkillPowerItem.Enabled = true;
    }

    /// <summary>g_SkillPowerItemList → ListBoxSkillPowerItem（Clear + AddStrings）。</summary>
    public void RefSkillPowerList()
    {
        ListBoxSkillPowerItem.Items.Clear();
        foreach (var name in g_SkillPowerItemList.Snapshot())
            ListBoxSkillPowerItem.Items.Add(name);
    }

    /// <summary>StringGridSkillPowerSetEditText（4619-4622）。</summary>
    public void StringGridSkillPowerSetEditText()
        => ButtonChgSkillPowerItem.Enabled = SelSkillPowerItem != null;

    /// <summary>chkSkillPowerItemUseHum/MonClick（4814-4824）：写配置。</summary>
    public void ChkSkillPowerItemUseChanged()
    {
        M2Config.boSkillPowerItemUseHum = chkSkillPowerItemUseHum.Checked;
        M2Config.boSkillPowerItemUseMon = chkSkillPowerItemUseMon.Checked;
    }

    // ==================== 物品过滤/备注复选框 ====================

    /// <summary>chkSendFilterItemListClick（4726-4730）。</summary>
    public void ChkSendFilterItemListClick()
    {
        M2Config.boSendFilterItemList = chkSendFilterItemList.Checked;
        WriteBoolHandler?.Invoke("Setup", "SendFilterItemList", M2Config.boSendFilterItemList);
    }

    /// <summary>chkEnablePlayerUseClientPickItemsClick（4732-4748）：boOpened 门控 + 键控写盘。</summary>
    public void ChkEnablePlayerUseClientPickItemsClick()
    {
        if (!boOpened)
            return;
        M2Config.boEnablePlayerUseClientPickItems = chkEnablePlayerUseClientPickItems.Checked;
        if (g_nKey_UseClientPickItems == 1)
            WriteBoolHandler?.Invoke("Setup", "EnablePlayerUseClientPickItems", M2Config.boEnablePlayerUseClientPickItems);
    }

    /// <summary>chkEnableHeroUseClientPickItemsClick（4750-4764）。</summary>
    public void ChkEnableHeroUseClientPickItemsClick()
    {
        if (!boOpened)
            return;
        M2Config.boEnableHeroUseClientPickItems = chkEnableHeroUseClientPickItems.Checked;
        if (g_nKey_UseClientPickItems == 1)
            WriteBoolHandler?.Invoke("Setup", "EnableHeroUseClientPickItems", M2Config.boEnableHeroUseClientPickItems);
    }

    /// <summary>chkSendItemDescListClick（4766-4770）。</summary>
    public void ChkSendItemDescListClick()
    {
        M2Config.boSendItemDescList = chkSendItemDescList.Checked;
        WriteBoolHandler?.Invoke("Setup", "SendItemDescList", M2Config.boSendItemDescList);
    }

    /// <summary>chkSendTzItemDescListClick（4772-4776）。</summary>
    public void ChkSendTzItemDescListClick()
    {
        M2Config.boSendTzItemDescList = chkSendTzItemDescList.Checked;
        WriteBoolHandler?.Invoke("Setup", "SendTzItemDescList", M2Config.boSendTzItemDescList);
    }

    /// <summary>chkSingleHintClick（4778-4783）：写配置 + 下发。</summary>
    public void ChkSingleHintClick()
    {
        M2Config.boSingleHint = chkSingleHint.Checked;
        WriteBoolHandler?.Invoke("Setup", "SingleHint", M2Config.boSingleHint);
        SendServerConfigHandler?.Invoke();
    }

    /// <summary>chkEnabledBuyShopItemGiveClick（4903-4908）：写配置 + 下发。</summary>
    public void ChkEnabledBuyShopItemGiveClick()
    {
        M2Config.boEnabledBuyShopItemGive = chkEnabledBuyShopItemGive.Checked;
        WriteBoolHandler?.Invoke("Setup", "EnabledBuyShopItemGive", M2Config.boEnabledBuyShopItemGive);
        SendServerConfigHandler?.Invoke();
    }

    /// <summary>chkTZSupportRenameItem/Desc/NoRename 三开关（5087-5106）。</summary>
    public void ChkRenameFlagsChanged()
    {
        M2Config.boTZSupportRenameItem = chkTZSupportRenameItem.Checked;
        M2Config.boDescSupportRenamItem = chkDescSupportRenamItem.Checked;
        M2Config.boNoRenameDescReadDefault = chkNoRenameDescReadDefault.Checked;
    }

    // ==================== Ctrl+F 物品查找 ====================

    /// <summary>ListBoxitemListKeyDown（1984-2011）：Ctrl+F 弹输入框并精确匹配定位。</summary>
    public void ListBoxitemListKeyDown(System.Windows.Forms.ListBox listBox, int key, bool ctrl)
    {
        if (key != (int)System.Windows.Forms.Keys.F || !ctrl)
            return;
        var (ok, value) = InputQueryHandler?.Invoke("物品查找", "输入物品名称:", "") ?? (false, "");
        if (!ok)
            return;
        string sItemName = value;
        if (sItemName.Length == 0)
            return;
        for (int i = 0; i < listBox.Items.Count; i++)
        {
            if (listBox.Items[i]?.ToString() == sItemName)
            {
                listBox.SelectedIndex = i;
                break;
            }
        }
    }

    /// <summary>ButtonShopRefreshClick 之外的商店保存（ButtonShopSaveItemClick 4097-4101）。</summary>
    public void ButtonShopSaveItemClick()
    {
        g_SndaShopList.SaveToFile();
        ButtonShopSaveItem.Enabled = false;
    }
}
