using System.Linq;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 端到端工作流（跨方法族）：覆盖"打开 → 新增 → 改参数 → 存盘 → 重开"整条路径，
/// 用于抓单方法测试看不出的时序/状态残留问题。
///
/// ⚠ 无头 UI：全程 `DoOpen(showModal:false)`；弹窗与输入全部走注入接缝（绝不弹真实模态框）。
/// </summary>
[Collection("GamePetsSerial")]
public sealed class GamePetsWorkflowTests : GamePetsTestBase
{
    private string PetConfigFile => Path.Combine(Dir, "GamePetConfigs.txt");

    [Fact]
    public void Workflow_OpenAddSaveReopen_ConfigSurvivesAndIsListed()
    {
        // ① 首次打开（空配置）
        //   ⚠ 测试基类构造期已跑过一次 DoOpen（决定"基线"）；这里先清掉它给
        //     lstMonsterList 留下的残留项，使本测试从干净状态开始（原文 DoOpen 不清该列表，
        //     故重复打开会累加 —— 见 Workflow_ReopenTwice_MonsterListAccumulatesButCombosDoNot）。
        Form.lstMonsterList.Items.Clear();
        Form.MonsterListHandler = MonsterTable;
        Form.EffectImageListHandler = () => new List<string> { "a.wil", "b.wil", "c.wil" };
        Form.DoOpen(showModal: false);
        Assert.Empty(Form.lstGamePets.Items);
        Assert.Equal(2, Form.lstMonsterList.Items.Count);
        // cbbPetShowFile1 在 DoOpen 里 :269 被 Clear → 精确等于"根据Appr计算" + 3 素材
        Assert.Equal(4, Form.cbbPetShowFile1.Items.Count);

        // ② 双击选怪 → 填名
        Form.lstMonsterList.SelectedIndex = 0;
        Form.RaiseMonsterListDblClick();
        Assert.Equal("普通动物", Form.edtPetName.Text);

        // ③ 设参数后新增
        Form.sePetCaptureRate.Value = 30;
        Form.seHPScale.Value = 70;
        Form.chkLevelDifference.Checked = false;
        Form.cbbPetShowFile1.SelectedIndex = 2;
        Form.sePetShowStart1.Value = 111;
        Form.cbbPetAddHPType.SelectedIndex = 1;   // 百分比
        Form.sePetAddHP.Value = 55;
        Form.RaiseAddPet();
        Assert.Empty(Messages);

        var cfg = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal("普通动物", cfg.Name);
        Assert.Equal(30, cfg.CaptureRate);
        Assert.Equal(70, cfg.HPScale);
        Assert.False(cfg.EnabledLevelDifference);
        Assert.Equal(2, cfg.ShowFile1);
        Assert.Equal(111, cfg.ShowStart1);
        Assert.True(cfg.IsAddHPRate);
        Assert.Equal(55, cfg.AddHP);

        // ④ 存宠物配置
        Form.RaiseSavePet();

        // ⑤ 改一个全局参数并存参数
        Form.RaiseParamHandler(nameof(Form.chkOpenGamePet), true);
        Form.RaiseSpinHandler(nameof(Form.seGamePetMaxCount), 5);
        Form.RaiseSavePetParams();
        Assert.Equal(1, SendServerConfigCount);
        Assert.Equal(true, WroteBool.Single(x => x.Key == "OpenGamePet").Value);
        Assert.Equal(5, WroteInt.Single(x => x.Key == "GamePetMaxCount").Value);

        // ⑥ 清空内存 + 重新装载 → 配置必须原样回来
        GamePetsState.ClearGamePetsConfig();
        Assert.True(GamePetsState.LoadGamePetsConfig());
        var reloaded = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal("普通动物", reloaded.Name);
        Assert.Equal(30, reloaded.CaptureRate);
        Assert.Equal(70, reloaded.HPScale);
        Assert.False(reloaded.EnabledLevelDifference);
        Assert.Equal(2, reloaded.ShowFile1);
        Assert.Equal(111, reloaded.ShowStart1);
        Assert.True(reloaded.IsAddHPRate);
        Assert.Equal(55, reloaded.AddHP);

        // ⑦ 重开窗体 → 列表出现该宠物并被自动选中回填
        //   ⚠ 原文行为：DoOpen **不**清空 lstMonsterList（只清两个素材下拉与经验计划下拉），
        //     故重复 DoOpen 会把怪物名**累加**（生产中该窗体只 Open 一次）。
        //     此处只断言"被过滤后的名字确实出现过"与"没有漏进来被排除的种族"，
        //     不锁定具体条数（条数取决于本测试里 DoOpen 的调用次数）。
        Form.DoOpen(showModal: false);

        var monsterNames = Form.lstMonsterList.Items.Cast<object>().Select(o => (string)o).ToHashSet();
        Assert.Contains("普通动物", monsterNames);
        Assert.Contains("大怪", monsterNames);
        Assert.DoesNotContain("人形怪", monsterNames);
        Assert.DoesNotContain("弓箭手", monsterNames);
        Assert.DoesNotContain("练功师", monsterNames);
        Assert.DoesNotContain("低于动物", monsterNames);

        Assert.Single(Form.lstGamePets.Items);
        Assert.Equal("普通动物", (string)Form.lstGamePets.Items[0]);
        Assert.Same(reloaded, Form.SelGamePetConfig);
        Assert.Equal(30, (int)Form.sePetCaptureRate.Value);
        Assert.Equal(111, (int)Form.sePetShowStart1.Value);
    }

    [Fact]
    public void Workflow_AddEditDeleteSave_LeavesEmptyConfigFile()
    {
        OpenWith();

        // 增
        Form.edtPetName.Text = "临时怪";
        Form.RaiseAddPet();
        Assert.Single(GamePetsState.g_GamePetConfigList);

        // 改
        Form.edtPetName.Text = "改名怪";
        Form.RaiseEditPet();
        Assert.Equal("改名怪", GamePetsState.g_GamePetConfigList[0].Name);

        // 删（选中项即刚改过的那只）
        Form.RaiseDelPet();
        Assert.Empty(GamePetsState.g_GamePetConfigList);
        Assert.Empty(Form.lstGamePets.Items);

        // 存盘 → count=0
        Form.RaiseSavePet();
        Assert.Contains("count=0", File.ReadAllText(PetConfigFile));
    }

    [Fact]
    public void Workflow_ChangeExpSchemeThenSaveExp_PersistsToExpIniAndTable()
    {
        OpenWith();
        var expWrites = new List<(string Section, string Key, string Value)>();

        // ① 选 s_2Mult 并确认（设 SelectedIndex 已触发一次，再显式触发一次）
        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_2Mult;
        for (int i = 0; i <= 1000; i++)
            M2Config.dwPetNeedExps[i] = 1_000_000u * (uint)i;
        Form.RaiseLevelExpClick();

        // ② 保存经验
        WroteExp.Clear();
        Form.RaiseSaveExp();

        Assert.Equal("500000", WroteExp.Single(e => e.Key == "Level1").Value);   // 1,000,000 / 2
        Assert.Equal("500000000", WroteExp.Single(e => e.Key == "Level1000").Value);
        _ = expWrites;
    }

    [Fact]
    public void Workflow_ReopenTwice_MonsterListAccumulatesButCombosDoNot()
    {
        Form.MonsterListHandler = MonsterTable;
        Form.EffectImageListHandler = () => new List<string> { "a.wil" };
        int baseline = Form.lstMonsterList.Items.Count;   // 构造期已跑过一次 DoOpen

        Form.DoOpen(showModal: false);
        Form.DoOpen(showModal: false);

        // ⚠ 原文 DoOpen **不**清理 lstMonsterList（只清两个素材下拉与经验计划下拉），
        //   故重复打开会**累加**怪物名 —— 这是原文行为（生产中该窗体只打开一次）。
        Assert.Equal(baseline + 4, Form.lstMonsterList.Items.Count);   // 每批 2 项 × 2 次
        // 素材下拉与经验计划下拉有 Clear → 不累加
        Assert.Equal(2, Form.cbbPetShowFile1.Items.Count);
        Assert.Equal(19, Form.cbbLevelExp.Items.Count);
    }

    [Fact]
    public void Workflow_LevelExpThenParamsSave_BothDirtyFlagsClearedIndependently()
    {
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_5Mult;
        Form.RaiseLevelExpClick();
        Assert.True(Form.BoModValued);

        Form.btnSaveExp.Enabled = true;
        Form.RaiseSaveExp();
        Assert.False(Form.BoModValued);                 // ModValue 的脏标记被清
        Assert.False(Form.btnSaveExp.Enabled);
        Assert.True(Form.btnSavePetParams.Enabled);     // 参数保存按钮**不受** btnSaveExp 影响

        Form.RaiseSavePetParams();
        Assert.False(Form.btnSavePetParams.Enabled);
    }

    [Fact]
    public void Workflow_MultiPetSaveAndReload_PreservesOrder()
    {
        OpenWith();
        foreach (var name in new[] { "第一", "第二", "第三" })
        {
            Form.edtPetName.Text = name;
            Form.RaiseAddPet();
        }
        Form.RaiseSavePet();

        GamePetsState.ClearGamePetsConfig();
        Assert.True(GamePetsState.LoadGamePetsConfig());

        Assert.Equal(new[] { "第一", "第二", "第三" },
            GamePetsState.g_GamePetConfigList.Select(c => c.Name));
        var ini = new TFastIniFile(PetConfigFile);
        Assert.Equal(3, ini.ReadInteger("setup", "count", -1));
        Assert.Equal("第一", ini.ReadString("pet1", "name", ""));
        Assert.Equal("第三", ini.ReadString("pet3", "name", ""));
    }
}
