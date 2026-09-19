using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J42：MonsterConfig.pas 攻击配置逐控件编辑处理器 + tmrFlash/LoadStdItems/KeyDown 1:1 测试。</summary>
public sealed class MonsterConfigAttackEditsTests : IDisposable
{
    private readonly string _dir;
    private readonly MonsterConfigForm _form;
    private readonly TCustomMonsterConfig _cfg;

    public MonsterConfigAttackEditsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j42_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetMonsterConfigSliceDefaults();
        M2Config.ResetMonsterSlice2Defaults();
        DropLimitGlobals.ResetForTests();
        M2Config.sItemDropLimit = Path.Combine(_dir, "ItemDropLimit") + "\\";
        M2Config.sItemDropLogDir = Path.Combine(_dir, "ItemDropLimit", "DropLog") + "\\";
        M2Config.sSmartMonsterDir = Path.Combine(_dir, "SmartMonster") + "\\";
        M2Config.sCustomMonsterClientConfigFileName = "";
        DropLimitGlobals.g_DropLimitMgr.LoadConfig();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _cfg = new TCustomMonsterConfig("白野猪", 156, 10);
        _form = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string> { "火球术", "治愈术" };
            f.MonsterListHandler = () => new List<string> { "鸡", "稻草人" };
            f.CustomMonsterListHandler = () => new List<string>();
            f.AllItemsHandler = () => new List<string>();
            f.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { _cfg };
            f.EffectImageListHandler = () => new List<string> { "eff1", "eff2", "eff3" };
            f.StdItemListHandler = () => new List<(string, int)>
            {
                ("木剑", 5), ("布衣", 10), ("金创药", 2), ("头盔", 15), ("斗笠", 16), ("鼓", 65), ("盾牌", 12), ("毒药", 31), ("戒指", 22),
            };
            f.InputQueryHandler = (_, _) => false;
            f.Open(showModal: false);
            f.VstCustomMonster.Items[0].Selected = true;
            f.VstCustomMonsterNodeClick();
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        DropLimitGlobals.ResetForTests();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void ClientGroup_SpinAndComboHandlers_WriteConfig()
    {
        // 切攻击组 1（下标 1 → ClientConfigIndex=1，级联回填已还原状态）
        _form.Cbbs["cbbClientAttackConfig"].SelectedIndex = 1;
        Assert.Equal(1, _form.ClientConfigIndex);
        Assert.False(_cfg.IsChanged); // 回填级联被 SetMonsterConfigChanged(old) 还原

        // Spin：Value 赋值触发 OnChange（VCL 同义）→ 写回 + 改标志
        _form.Spins["seClientFlyStartIndex"].Value = 5;
        Assert.Equal(5, _cfg.ClientAttackConfigs[1].Fly_StartIndex);
        Assert.True(_cfg.IsChanged);
        Assert.True(_form.btnSave.Enabled);

        _form.Spins["seClientFlyPlayTime"].Value = 200;
        Assert.Equal(200, _cfg.ClientAttackConfigs[1].Fly_PlayTime);
        // 组合框：ItemIndex-1 语义
        _form.Cbbs["cbbClientFlyFile"].SelectedIndex = 3;
        Assert.Equal(2, _cfg.ClientAttackConfigs[1].Fly_File);
        _form.Cbbs["cbbClientFlyDrawMode"].SelectedIndex = 1;
        Assert.Equal(TCustomDrawMode.mdmNormal, _cfg.ClientAttackConfigs[1].Fly_DrawMode);

        // 切组后写回落在新组
        _form.Cbbs["cbbClientAttackConfig"].SelectedIndex = 3;
        _form.Spins["seClientSelfPlayTime"].Value = 300;
        Assert.Equal(300, _cfg.ClientAttackConfigs[3].Self_PlayTime);
        Assert.Equal(100, _cfg.ClientAttackConfigs[1].Self_PlayTime); // 组 1 不受影响
    }

    [Fact]
    public void ClientGroup_CheckHandlers_VclOnClickSemantics()
    {
        // VCL OnClick 语义：回填经 SetChk 程序赋值不触发处理器 → 状态不被污染
        _form.VstCustomMonster.Items[0].Selected = false;
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.False(_cfg.IsChanged);
        Assert.Equal(1, _cfg.ClientAttackConfigs[0].Fly_CalcDir);
        // 直接赋值（未压制）→ 事件触发 → 处理器写回 + 改标志
        _form.Chks["chkClientFlyCalcDir"].Checked = false;
        Assert.Equal(0, _cfg.ClientAttackConfigs[0].Fly_CalcDir);
        Assert.True(_cfg.IsChanged);
        Assert.True(_form.btnSave.Enabled);
    }

    [Fact]
    public void ServerGroup_Handlers_WriteConfig()
    {
        // 切服务端攻击组 2
        _form.Cbbs["cbbServerAttackConfig"].SelectedIndex = 2;
        Assert.Equal(2, _form.ServerConfigIndex);
        var sc = _cfg.MonsterServerConfigs[2];

        _form.Spins["seAttackDelayTime"].Value = 650;
        Assert.Equal(650, sc.AttackDelayTime);
        _form.Cbbs["cbbAttackMode"].SelectedIndex = 1;
        Assert.Equal(TCustomAttackMode.mamFar, sc.AttackMode);
        _form.Cbbs["cbbOperateMode"].SelectedIndex = 1;
        Assert.Equal(TCustomOperateMode.momProtect, sc.OperateMode);
        Assert.True(_form.GrpProtectVisible); // 保护面板切换
        _form.Chks["chkAttackTeleportAttack"].Checked = true;
        _form.ServerEditHandlers["chkAttackTeleportAttack"]();
        Assert.True(sc.AttackTeleportAttack);

        // 附加状态 Tag 驱动组（0..11）
        _form.Chks["chkAdditional4"].Checked = true;
        _form.ServerEditHandlers["chkAdditional4"]();
        Assert.True(sc.Additionals[4].Checked);
        _form.Spins["seAdditionalRate4"].Value = 9;
        Assert.Equal(9, sc.Additionals[4].Rate);
        _form.Spins["seAdditionalTime11"].Value = 7;
        Assert.Equal(7, sc.Additionals[11].Time);
        _form.Spins["seAdditionaHP0"].Value = 12;
        Assert.Equal(12, sc.AdditionalHP0);

        // 召唤怪物（edtCallMonster TextChanged 程序赋值即触发，VCL OnChange 同义）
        _form.Edits["edtCallMonster1"].Text = "黑野猪";
        Assert.Equal("黑野猪", sc.CallMonsters[0]);
        _form.Spins["seCallMonsterNum1"].Value = 3;
        Assert.Equal(3, sc.CallMonsterNums[0]);

        // 保护组
        _form.Chks["chkProtectAddHP"].Checked = true;
        _form.ServerEditHandlers["chkProtectAddHP"]();
        Assert.True(sc.ProtectAddHP);
        _form.Spins["seProtectAddHPPercent"].Value = 40;
        Assert.Equal(40, sc.ProtectAddHPPercent);
        _form.Spins["seProtectTargetRange"].Value = 6;
        Assert.Equal(6, sc.ProtectTargetRange);

        // 移动目标
        _form.Chks["chkMoveTarget"].Checked = true;
        _form.ServerEditHandlers["chkMoveTarget"]();
        Assert.True(sc.MoveTarget);
    }

    [Fact]
    public void ServerGroup_Guard_NoConfig_NoWrite()
    {
        // 处理器在未选中怪物/攻击组时（nil 语义）不写不置标
        _form.FCurrentMonsterCustomConfig = null;
        _form.Spins["seAttackDelayTime"].Value = 123;
        _form.ServerEditHandlers["seAttackDelayTime"](); // 守卫早退
        Assert.Equal(400, _cfg.MonsterServerConfigs[0].AttackDelayTime); // 默认不变
        Assert.False(_cfg.IsChanged);
    }

    [Fact]
    public void BaseGroup_Handlers_WriteConfig()
    {
        _form.Spins["seViewRange"].Value = 11;
        Assert.Equal(11, _cfg.ServerBaseConfig.ViewRange);
        _form.Cbbs["cbbMonsterType"].SelectedIndex = 2;
        Assert.Equal(TMonsterType.mtDigUP, _cfg.ServerBaseConfig.MonsterType);
        _form.Spins["seProtectRange"].Value = 5;
        Assert.Equal(5, _cfg.ServerBaseConfig.ProtectRange);
        _form.Chks["chkNoAttack"].Checked = true;
        _form.BaseEditHandlers["chkNoAttack"]();
        Assert.True(_cfg.ServerBaseConfig.NoAttack);

        _form.Cbbs["cbbHPFile"].SelectedIndex = 0; // 首项 '根据Appr计算' → HPFile=-1
        Assert.Equal(-1, _cfg.ClientBaseConfig.HPFile);
        _form.Cbbs["cbbHPFile"].SelectedIndex = 1;
        Assert.Equal(0, _cfg.ClientBaseConfig.HPFile);
        _form.Spins["seHPOffsetX"].Value = 9;
        Assert.Equal(9, _cfg.ClientBaseConfig.HPOffsetX);

        _form.Edits["edtSoundNormal"].Text = "mon-wz";
        Assert.Equal("mon-wz", _cfg.ClientBaseConfig.Sounds[0].Value);
        _form.Edits["edtSoundAttack6"].Text = "atk6";
        Assert.Equal("atk6", _cfg.ClientBaseConfig.Sounds[10].Value);
        _form.Chks["chkDieNoCalcDir"].Checked = true;
        _form.BaseEditHandlers["chkDieNoCalcDir"]();
        Assert.Equal(1, _cfg.ClientBaseConfig.DieNoCalcDir);
    }

    [Fact]
    public void Batch_And_Calc_Handlers()
    {
        // 批量动作/特效（ItemIndex-1 → 全 12 动作）
        _form.Cbbs["cbbBatchAction"].SelectedIndex = 3;
        for (int i = 0; i < 12; i++)
            Assert.Equal(2, _cfg.ClientActions[i].ActionFile);
        _form.Cbbs["cbbBatchEffect"].SelectedIndex = 1;
        for (int i = 0; i < 12; i++)
            Assert.Equal(0, _cfg.ClientActions[i].EffectFile);
        Assert.True(_cfg.IsChanged);

        // 计算图片位：站/走/普攻/被击/死亡 = 基址 + Offset×80，其余不动
        for (int i = 0; i < 12; i++)
            _cfg.ClientActions[i].StartIndex = -1;
        _form.Spins["seClientStartIndex"].Value = 100;
        _form.BtnCalcStartIndexClick();
        Assert.Equal(100, _cfg.ClientActions[0].StartIndex);  // matStand +0
        Assert.Equal(180, _cfg.ClientActions[1].StartIndex);  // matWalk +1
        Assert.Equal(260, _cfg.ClientActions[2].StartIndex);  // matDefAttack +2
        Assert.Equal(340, _cfg.ClientActions[3].StartIndex);  // matStruck +3
        Assert.Equal(420, _cfg.ClientActions[4].StartIndex);  // matDie +4
        Assert.Equal(-1, _cfg.ClientActions[5].StartIndex);   // matStoneRevive 不计
        Assert.Equal(-1, _cfg.ClientActions[6].StartIndex);   // matAttack1 不计

        // 负基址 → 该五动作 -1
        _form.Spins["seClientEffectIndex"].Value = -5;
        _form.BtnCalcEffectIndexClick();
        Assert.Equal(-1, _cfg.ClientActions[0].EffectIndex);
        Assert.Equal(-1, _cfg.ClientActions[4].EffectIndex);

        // 正特效基址
        _form.Spins["seClientEffectIndex"].Value = 60;
        _form.BtnCalcEffectIndexClick();
        Assert.Equal(60, _cfg.ClientActions[0].EffectIndex);
        Assert.Equal(380, _cfg.ClientActions[4].EffectIndex);
    }

    [Fact]
    public void LoadStdItems_Filters()
    {
        _form.LoadStdItems(); // -1 全量
        Assert.Equal(9, _form.lstItemList.Items.Count);
        _form.LoadStdItems(1); // 武器：StdMode 5/6
        Assert.Equal(new List<string> { "木剑" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        _form.LoadStdItems(0); // 衣服：10/11
        Assert.Equal(new List<string> { "布衣" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        _form.LoadStdItems(13); // 斗笠：16
        Assert.Equal(new List<string> { "斗笠" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        _form.LoadStdItems(14); // 鼓：65
        Assert.Equal(new List<string> { "鼓" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        _form.LoadStdItems(100); // 0..3 或 25
        Assert.Equal(new List<string> { "金创药" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        // else 分支（Tag=999 → CheckUserItems 等效）：不入任何槽位的物品保留
        // 金创药(2) 不入任何槽 → 保留；毒药(31) 同 → 保留
        _form.LoadStdItems(999);
        Assert.Equal(new List<string> { "金创药", "毒药" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
        // 菜单互斥勾选
        _form.MenuItemShowAllClick(4);
        Assert.Equal(4, _form.CurrentFilterTag);
        Assert.Equal(new List<string> { "头盔" }, _form.lstItemList.Items.Cast<object>().Select(o => o.ToString()!).ToList());
    }

    [Fact]
    public void LstItemListDblClick_WritesSlotEdit_And_Flash()
    {
        // 人形怪配置（双击需 SelMonsterConfig）
        M2Config.PlayMonsterConfigs["稻草人"] = new TPlayMonsterConfig { Name = "稻草人" };
        _form.ListBoxMonsterList.SelectedIndex = 1;
        _form.ListBoxMonsterListClick(_form);
        Assert.NotNull(_form.SelMonsterConfig);

        _form.MenuItemShowAllClick(1); // 武器过滤
        _form.lstItemList.SelectedIndex = 0; // 木剑(5) → U_WEAPON → 槽 1
        _form.LstItemListDblClick();
        Assert.Equal("木剑", _form.EquipEdits[1].Text);
        Assert.True(_form.TmrFlashEnabled);
        Assert.Same(_form.EquipEdits[1], _form.FlashEdit);

        // 闪烁：黄↔白交替，10 次后复位停表
        var window = System.Drawing.SystemColors.Window;
        Assert.Equal(window, _form.FlashEdit!.BackColor);
        _form.TmrFlashTick();
        Assert.Equal(System.Drawing.Color.Yellow, _form.FlashEdit.BackColor);
        _form.TmrFlashTick();
        Assert.Equal(window, _form.FlashEdit.BackColor);
        for (int i = 0; i < 8; i++)
            _form.TmrFlashTick();
        Assert.Null(_form.FlashEdit); // Tag 计满 10 → 复位
        Assert.False(_form.TmrFlashEnabled);
        Assert.Equal(window, _form.EquipEdits[1].BackColor);

        // 戒指（22）→ GetTakeOnPosition=7 → 左右手对话框
        M2Config.PlayMonsterConfigs["稻草人"].UseItems[7] = "";
        _form.LoadStdItems(7);
        _form.lstItemList.SelectedIndex = 0; // 戒指
        _form.LeftRightDialogHandler = () => 1;
        _form.LstItemListDblClick();
        Assert.Equal("戒指", _form.EquipEdits[8].Text); // 右戒指
        Assert.Equal(1, MonsterConfigForm.LeftRightIndex);
        _form.lstItemList.SelectedIndex = 0;
        _form.LeftRightDialogHandler = () => 0;
        _form.LstItemListDblClick();
        Assert.Equal("戒指", _form.EquipEdits[7].Text); // 左戒指
        // 未选中人形怪 → 早退（FlashEdit 保持原状）
        _form.SelMonsterConfig = null;
        var prevFlash = _form.FlashEdit;
        _form.LstItemListDblClick();
        Assert.Same(prevFlash, _form.FlashEdit);
    }

    [Fact]
    public void Three_KeyDowns_ExactMatchSearch()
    {
        // 物品查找（lstItemList）
        _form.LoadStdItems();
        bool asked = false;
        _form.InputQueryHandler = (caption, prompt) =>
        {
            asked = caption == "物品查找" && prompt == "输入物品名称:";
            _form.LastInputQueryText = "盾牌";
            return true;
        };
        _form.LstItemListCtrlF();
        Assert.True(asked);
        Assert.Equal("盾牌", _form.lstItemList.Items[_form.lstItemList.SelectedIndex]?.ToString() ?? "");
        // 魔法查找
        _form.InputQueryHandler = (caption, prompt) =>
        {
            Assert.Equal("魔法查找", caption);
            _form.LastInputQueryText = "治愈术";
            return true;
        };
        _form.ListBoxMagicListCtrlF();
        Assert.Equal("治愈术", _form.ListBoxMagicList.Items[_form.ListBoxMagicList.SelectedIndex]?.ToString() ?? "");
        // 人形怪查找
        _form.InputQueryHandler = (caption, prompt) =>
        {
            Assert.Equal("人形怪查找", caption);
            _form.LastInputQueryText = "鸡";
            return true;
        };
        _form.ListBoxMonsterListCtrlF();
        Assert.Equal(0, _form.ListBoxMonsterList.SelectedIndex);
        // 未命中保持 + 取消不动 + 空关键字不动
        _form.ListBoxMonsterList.SelectedIndex = -1;
        _form.InputQueryHandler = (_, _) => true;
        _form.LastInputQueryText = "不存在的";
        _form.ListBoxMonsterListCtrlF();
        Assert.Equal(-1, _form.ListBoxMonsterList.SelectedIndex);
        _form.InputQueryHandler = (_, _) => false;
        _form.ListBoxMonsterListCtrlF();
        Assert.Equal(-1, _form.ListBoxMonsterList.SelectedIndex);
    }
}
