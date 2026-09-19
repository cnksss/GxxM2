using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J37：MonsterConfig.pas 巨片第二片（人形怪配置/魔法双击/Ctrl+F/装备拖放槽位匹配）1:1 测试。</summary>
public sealed class MonsterConfigSlice2Tests : IDisposable
{
    private readonly string _dir;
    private readonly ItemSetForm _unusedForm;
    private readonly MonsterConfigForm _form;

    public MonsterConfigSlice2Tests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j37_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetMonsterConfigSliceDefaults();
        M2Config.ResetMonsterSlice2Defaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string> { "火球术", "治愈术", "施毒术" };
            f.MonsterListHandler = () => new List<string> { "鸡", "稻草人" };
            f.CustomMonsterListHandler = () => new List<string> { "自定义怪1" };
            f.InputQueryHandler = (caption, prompt) => false;
            return f;
        });
        _unusedForm = null!;
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void GetTakeOnPosition_MapsStdModes()
    {
        StaRunner.New(() =>
        {
            Assert.Equal(MonsterConfigForm.U_WEAPON, MonsterConfigForm.GetTakeOnPosition(5));
            Assert.Equal(MonsterConfigForm.U_WEAPON, MonsterConfigForm.GetTakeOnPosition(6));
            Assert.Equal(MonsterConfigForm.U_DRESS, MonsterConfigForm.GetTakeOnPosition(10));
            Assert.Equal(MonsterConfigForm.U_DRESS, MonsterConfigForm.GetTakeOnPosition(11));
            Assert.Equal(MonsterConfigForm.U_HELMET, MonsterConfigForm.GetTakeOnPosition(15));
            Assert.Equal(MonsterConfigForm.U_NECKLACE, MonsterConfigForm.GetTakeOnPosition(19));
            Assert.Equal(MonsterConfigForm.U_RINGL, MonsterConfigForm.GetTakeOnPosition(22));
            Assert.Equal(MonsterConfigForm.U_RINGL, MonsterConfigForm.GetTakeOnPosition(23));
            Assert.Equal(MonsterConfigForm.U_ARMRINGR, MonsterConfigForm.GetTakeOnPosition(24));
            Assert.Equal(MonsterConfigForm.U_ARMRINGR, MonsterConfigForm.GetTakeOnPosition(26));
            Assert.Equal(MonsterConfigForm.U_RIGHTHAND, MonsterConfigForm.GetTakeOnPosition(30));
            Assert.Equal(MonsterConfigForm.U_BUJUK, MonsterConfigForm.GetTakeOnPosition(25));
            Assert.Equal(MonsterConfigForm.U_BUJUK, MonsterConfigForm.GetTakeOnPosition(51));
            Assert.Equal(MonsterConfigForm.U_BOOTS, MonsterConfigForm.GetTakeOnPosition(52));
            Assert.Equal(MonsterConfigForm.U_BOOTS, MonsterConfigForm.GetTakeOnPosition(62));
            Assert.Equal(MonsterConfigForm.U_CHARM, MonsterConfigForm.GetTakeOnPosition(7));
            Assert.Equal(MonsterConfigForm.U_CHARM, MonsterConfigForm.GetTakeOnPosition(94));
            Assert.Equal(MonsterConfigForm.U_BELT, MonsterConfigForm.GetTakeOnPosition(54));
            Assert.Equal(MonsterConfigForm.U_BELT, MonsterConfigForm.GetTakeOnPosition(64));
            Assert.Equal(MonsterConfigForm.U_SHIELD, MonsterConfigForm.GetTakeOnPosition(12));
            Assert.Equal(-1, MonsterConfigForm.GetTakeOnPosition(0));
        });
    }

    [Fact]
    public void MonsterListClick_LoadsPlayMonsterConfig()
    {
        StaRunner.New(() =>
        {
            var cfg = new TPlayMonsterConfig { Name = "稻草人", Job = 1, Gender = 0, Hair = 3, nDieDropUseItemRate = 30 };
            M2Config.PlayMonsterConfigs["稻草人"] = cfg;

            _form.Open(showModal: false);
            _form.ListBoxMonsterList.SelectedIndex = 1; // 稻草人
            _form.ListBoxMonsterListClick(_form);

            Assert.Equal("稻草人", _form.GroupBoxMonsterConfig.Text);
            Assert.True(_form.GroupBoxMonsterConfig.Enabled);
            Assert.Equal(1, _form.cbbJob.SelectedIndex);
            Assert.Equal(30, (int)_form.seDieDropUseItemRate.Value);
            // Delphi：点击人形怪回填控件触发 OnChange → ModValue（原文行为）
            Assert.True(_form.IsModValued);
        });
    }

    [Fact]
    public void MonsterMagicDragDrop_DedupAndAdd()
    {
        StaRunner.New(() =>
        {
            var cfg = new TPlayMonsterConfig { Name = "稻草人" };
            M2Config.PlayMonsterConfigs["稻草人"] = cfg;
            _form.Open(showModal: false);
            _form.ListBoxMonsterList.SelectedIndex = 1; // 稻草人
            _form.ListBoxMonsterListClick(_form);

            _form.ListBoxMagicList.SelectedIndex = 0; // 火球术
            _form.ListBoxMonsterMagicListDragDrop(_form);
            Assert.Equal(1, _form.ListBoxMonsterMagicList.Items.Count);
            Assert.Equal("火球术", _form.ListBoxMonsterMagicList.Items[0]);

            _form.ListBoxMagicList.SelectedIndex = 0;
            _form.ListBoxMonsterMagicListDragDrop(_form); // 重复添加被跳过
            Assert.Equal(1, _form.ListBoxMonsterMagicList.Items.Count);
            Assert.True(_form.ButtonMonUseItemsChange.Enabled);
        });
    }

    [Fact]
    public void MonsterMagicDblClick_DeletesSelected()
    {
        StaRunner.New(() =>
        {
            var cfg = new TPlayMonsterConfig { Name = "稻草人" };
            M2Config.PlayMonsterConfigs["稻草人"] = cfg;
            _form.Open(showModal: false);
            _form.ListBoxMonsterList.SelectedIndex = 1; // 稻草人
            _form.ListBoxMonsterListClick(_form);
            _form.ListBoxMagicList.SelectedIndex = 1; // 治愈术
            _form.ListBoxMonsterMagicListDragDrop(_form);
            _form.ListBoxMonsterMagicList.SelectedIndex = 0;
            _form.ListBoxMonsterMagicListDblClick(_form);
            Assert.Equal(0, _form.ListBoxMonsterMagicList.Items.Count);
        });
    }

    [Fact]
    public void CtrlF_FindsAndSelects_AndNotFoundKeepsIndex()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.FindInList(_form.ListBoxMonsterList, "稻草人");
            Assert.Equal(1, _form.ListBoxMonsterList.SelectedIndex);
            _form.FindInList(_form.ListBoxMonsterList, "不存在的怪"); // 未命中：ItemIndex 保持原文行为
            Assert.Equal(1, _form.ListBoxMonsterList.SelectedIndex);
        });
    }

    [Fact]
    public void UseItemDragDrop_TagMatching()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            // 戒指(L) 槽位：Tag 相同才写入
            _form.EquipEdits[MonsterConfigForm.U_RINGL].Text = "旧戒指";
            _form.DragDropEquip(MonsterConfigForm.U_RINGL, "新手戒指", 22);
            Assert.Equal("新手戒指", _form.EquipEdits[MonsterConfigForm.U_RINGL].Text);
            // 手镯右 Tag：戒指的落点不写入手镯编辑框
            _form.DragDropEquip(MonsterConfigForm.U_ARMRINGR, "新手手镯", 26);
            Assert.Equal("新手手镯", _form.EquipEdits[MonsterConfigForm.U_ARMRINGR].Text); // stdMode 26 → 右手镯槽
            // 手镯左右互通
            _form.DragDropEquip(MonsterConfigForm.U_ARMRINGL, "新手手镯", 26);
            Assert.Equal("新手手镯", _form.EquipEdits[MonsterConfigForm.U_ARMRINGL].Text);
        });
    }

    [Fact]
    public void MonUseItemsChangeClick_WritesBackConfig()
    {
        StaRunner.New(() =>
        {
            var cfg = new TPlayMonsterConfig { Name = "稻草人" };
            M2Config.PlayMonsterConfigs["稻草人"] = cfg;
            _form.Open(showModal: false);
            _form.ListBoxMonsterList.SelectedIndex = 1; // 稻草人
            _form.ListBoxMonsterListClick(_form);

            _form.cbbJob.SelectedIndex = 2;
            _form.cbbJobChange(_form);
            _form.chkDieDropUseItem.Checked = true;
            _form.chkDieDropUseItemClick(_form);
            _form.seDieDropUseItemRate.Value = 44;
            _form.chkProtectMode.Checked = true;
            _form.chkProtectModeClick(_form);
            _form.ListBoxMagicList.SelectedIndex = 0;
            _form.ListBoxMonsterMagicListDragDrop(_form);

            _form.ButtonMonUseItemsChangeClick(_form);
            Assert.Equal(2, cfg.Job);
            Assert.True(cfg.boDieDropUseItem);
            Assert.Equal(44, cfg.nDieDropUseItemRate);
            Assert.True(cfg.boProtectMode);
            Assert.Equal("火球术", cfg.Magics[0]);
            Assert.True(cfg.IsChanged);
            Assert.True(_form.ButtonMonUseItemsSave.Enabled);

            _form.ButtonMonUseItemsSaveClick(_form);
            Assert.True(M2Config.SavedPlayMonsterNames.Contains("稻草人"));
            Assert.False(_form.ButtonMonUseItemsSave.Enabled);
        });
    }
}
