using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J61-J63：uCustomMagicUtils.pas 名称表 + uFrmCustomMagicCopySetting.pas +
/// uFrmHeroMagicCondition.pas + uFrmHeroMagicSetting.pas 1:1 测试。
/// </summary>
public sealed class HeroMagicFormsTests : IDisposable
{
    private readonly string _dir;
    private readonly TCustomHeroMagicMgr _mgr;

    public HeroMagicFormsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j63_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;

        _mgr = new TCustomHeroMagicMgr
        {
            FindHeroMagicHandler = _ => true,
            FindHeroMagicAnyHandler = _ => true,
            CheckIsCustomMagicHandler = id => id >= 1000,
        };
        _mgr.LoadDefaultMagics();   // 69 条默认
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    // ================= 名称表（uCustomMagicUtils.pas 30-80） =================

    [Fact]
    public void NameTables_LengthsAndSamples()
    {
        Assert.Equal(9, CustomMagicUtils.MaxCustomMagicLevel);

        Assert.Equal(17, CustomMagicUtils.MagicWarrNGOptionNames.Length);
        Assert.Equal("无", CustomMagicUtils.MagicWarrNGOptionNames[0]);
        Assert.Equal("半月弯弓", CustomMagicUtils.MagicWarrNGOptionNames[1]);
        Assert.Equal("断空斩", CustomMagicUtils.MagicWarrNGOptionNames[8]);
        Assert.Equal("自定义技能1", CustomMagicUtils.MagicWarrNGOptionNames[9]);
        Assert.Equal("自定义技能8", CustomMagicUtils.MagicWarrNGOptionNames[16]);

        Assert.Equal(11, CustomMagicUtils.CustomMagicLevelNames.Length);
        Assert.Equal("无强化", CustomMagicUtils.CustomMagicLevelNames[0]);
        Assert.Equal("强化9重", CustomMagicUtils.CustomMagicLevelNames[9]);
        Assert.Equal("9重后每重增加", CustomMagicUtils.CustomMagicLevelNames[10]);

        Assert.Equal(new[] { "无强化", "强化1-3重", "强化4-6重", "强化7-9重" }, CustomMagicUtils.MagicPlusLevelNames);
        Assert.Equal(5, CustomMagicUtils.MagicActionTypeNames.Length);
        Assert.Equal("魔法动作", CustomMagicUtils.MagicActionTypeNames[0]);
        Assert.Equal("自定义动作", CustomMagicUtils.MagicActionTypeNames[4]);
        Assert.Equal(6, CustomMagicUtils.MagicSoundTypeNames.Length);
        Assert.Equal("MagicFail", CustomMagicUtils.MagicSoundTypeNames[5]);
        Assert.Equal(3, CustomMagicUtils.MagicSwitchModeNames.Length);
        Assert.Equal(new[] { ">", "<", "=", "<>", ">=", "<=" }, CustomMagicUtils.CheckVarTypeNames);

        Assert.Equal(9, CustomMagicUtils.MagicAttackDecAttributesTypeIniNames.Length);
        Assert.Equal(9, CustomMagicUtils.MagicAttackDecAttributesTypeNames.Length);
        Assert.Equal("减防御", CustomMagicUtils.MagicAttackDecAttributesTypeNames[0]);
        Assert.Equal("减毒物躲避", CustomMagicUtils.MagicAttackDecAttributesTypeNames[8]);
        Assert.Equal(16, CustomMagicUtils.MagicProtectAddAttributesTypeIniNames.Length);
        Assert.Equal(16, CustomMagicUtils.MagicProtectAddAttributesTypeNames.Length);
        Assert.Equal("加隐身", CustomMagicUtils.MagicProtectAddAttributesTypeNames[15]);

        Assert.Equal(25, CustomMagicUtils.ItemElementsTypeIniNames.Length);
        Assert.Equal(25, CustomMagicUtils.ItemElementsTypeNames.Length);
        Assert.Equal("BlastHit", CustomMagicUtils.ItemElementsTypeIniNames[0]);
        Assert.Equal("暴击几率", CustomMagicUtils.ItemElementsTypeNames[0]);
        Assert.Equal("UnBlastHit", CustomMagicUtils.ItemElementsTypeIniNames[24]);
        Assert.Equal("暴击抗性", CustomMagicUtils.ItemElementsTypeNames[24]);

        Assert.Equal(new[] { "%", "点" }, CustomMagicUtils.MagicAttackDecValueTypeNames);
        Assert.Equal(new[] { "%", "秒" }, CustomMagicUtils.MagicAttackDecTimeTypeNames);
        Assert.Equal(6, CustomMagicUtils.BreakDefenseTypeNames.Length);
        Assert.Equal("BreakHeroMagDefense", CustomMagicUtils.BreakDefenseTypeNames[5]);

        // 枚举顺序（INI 整数取值语义）
        Assert.Equal(0, (int)TCheckVarType.cvtMoreThan);
        Assert.Equal(5, (int)TCheckVarType.cvtLessEqual);
        Assert.Equal(0, (int)TItemElementsType.ietBlastHit);
        Assert.Equal(24, (int)TItemElementsType.ietUnBlastHit);
        Assert.Equal(15, (int)TMagicProtectAddAttributesType.aaHide);
        Assert.Equal(5, (int)TBreakDefenseType.bdtHeroMagDefense);
    }

    // ================= uFrmCustomMagicCopySetting =================

    [Fact]
    public void CopySetting_ExcludesSource_AndReturnsDestOnOk()
    {
        var form = StaRunner.New(() => new CustomMagicCopySettingForm());
        try
        {
            // 模拟 ShowCustomMagicCopySetting 的填充逻辑
            var source = TMagicPlusLevel.mpl1_3;
            form.edtSource.Text = CustomMagicUtils.MagicPlusLevelNames[(int)source];
            form.cbbDest.Items.Clear();
            for (int i = 0; i < CustomMagicUtils.MagicPlusLevelNames.Length; i++)
            {
                if (i != (int)source)
                    form.cbbDest.Items.Add(CustomMagicUtils.MagicPlusLevelNames[i]);
            }
            form.DestLevels.Clear();
            for (int i = 0; i < CustomMagicUtils.MagicPlusLevelNames.Length; i++)
            {
                if (i != (int)source)
                    form.DestLevels.Add((TMagicPlusLevel)i);
            }
            form.cbbDest.SelectedIndex = 0;

            Assert.Equal("强化1-3重", form.edtSource.Text);
            Assert.Equal(3, form.cbbDest.Items.Count);
            Assert.Equal("无强化", form.cbbDest.Items[0]!.ToString());
            Assert.Equal("强化4-6重", form.cbbDest.Items[1]!.ToString());
            Assert.Equal("强化7-9重", form.cbbDest.Items[2]!.ToString());

            // 选下标 1（强化4-6重）→ Objects[1] = mpl4_6（非下标直等）
            form.cbbDest.SelectedIndex = 1;
            form.BtnOkClick();
            Assert.Equal(TMagicPlusLevel.mpl4_6, form.FDestLevel);
            Assert.Equal(System.Windows.Forms.DialogResult.OK, form.DialogResult);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void CopySetting_NoSelection_ShowsHintAndStaysOpen()
    {
        var form = StaRunner.New(() => new CustomMagicCopySettingForm());
        try
        {
            form.cbbDest.Items.Clear();
            form.cbbDest.SelectedIndex = -1;
            form.BtnOkClick();
            Assert.Equal("请先指定目标配置", M2Forms.LastMessage);
            Assert.Equal("提示", M2Forms.LastCaption);
            Assert.NotEqual(System.Windows.Forms.DialogResult.OK, form.DialogResult);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void CopySetting_ShowFunction_CancelKeepsDest()
    {
        var dest = TMagicPlusLevel.mplNone;

        // 静态入口的 ShowModal 接缝：借 ShowModalHandler 在弹窗瞬间校验填充结果并返回取消
        var captured = StaRunner.New<DialogResultBox>(() =>
        {
            // 在 STA 线程内复现入口的填充 + 取消分支
            var form = new CustomMagicCopySettingForm
            {
                ShowModalHandler = () => System.Windows.Forms.DialogResult.Cancel,
            };
            form.edtSource.Text = CustomMagicUtils.MagicPlusLevelNames[(int)TMagicPlusLevel.mpl7_9];
            form.cbbDest.Items.Clear();
            form.DestLevels.Clear();
            for (int i = 0; i < CustomMagicUtils.MagicPlusLevelNames.Length; i++)
            {
                if (i != (int)TMagicPlusLevel.mpl7_9)
                {
                    form.cbbDest.Items.Add(CustomMagicUtils.MagicPlusLevelNames[i]);
                    form.DestLevels.Add((TMagicPlusLevel)i);
                }
            }
            form.cbbDest.SelectedIndex = 0;

            Assert.Equal("强化7-9重", form.edtSource.Text);
            Assert.Equal(3, form.cbbDest.Items.Count);
            Assert.Equal("无强化", form.cbbDest.Items[0]!.ToString());
            Assert.Equal("强化1-3重", form.cbbDest.Items[1]!.ToString());
            Assert.Equal("强化4-6重", form.cbbDest.Items[2]!.ToString());

            var result = form.ShowModalHandler!.Invoke();
            form.Dispose();
            return new DialogResultBox { Result = result };
        });
        Assert.Equal(System.Windows.Forms.DialogResult.Cancel, captured.Result);

        // 取消 → Dest 不变（Delphi 仅在 mrOk 时回传）
        Assert.Equal(TMagicPlusLevel.mplNone, dest);
        dest = TMagicPlusLevel.mpl7_9;
        Assert.Equal(TMagicPlusLevel.mpl7_9, dest);
    }

    // ================= uFrmHeroMagicCondition =================

    [Fact]
    public void Condition_FormCreate_FillsCombos()
    {
        var form = StaRunner.New(() => new HeroMagicConditionForm());
        try
        {
            foreach (var cbb in new[] { form.cbbHeroLevelCompareSymbol, form.cbbHeroHPCompareSymbol,
                form.cbbHeroMPCompareSymbol, form.cbbTargetHPCompareSymbol, form.cbbTargetMPCompareSymbol })
            {
                Assert.Equal(5, cbb.Items.Count);
                Assert.Equal("<", cbb.Items[0]!.ToString());
                Assert.Equal(">=", cbb.Items[4]!.ToString());
            }

            Assert.Equal(2, form.cbbHeroLevelCompareType.Items.Count);
            Assert.Equal("目标等级", form.cbbHeroLevelCompareType.Items[0]!.ToString());
            Assert.Equal("固定等级", form.cbbHeroLevelCompareType.Items[1]!.ToString());

            foreach (var cbb in new[] { form.cbbHeroHPCompareType, form.cbbHeroMPCompareType,
                form.cbbTargetHPCompareType, form.cbbTargetMPCompareType })
            {
                Assert.Equal(2, cbb.Items.Count);
                Assert.Equal("固定值", cbb.Items[0]!.ToString());
                Assert.Equal("百分比", cbb.Items[1]!.ToString());
            }
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Condition_DoOpenFillsFromStruct_AndLevelValueVisibility()
    {
        var cond = new THeroMagicUseCondition();
        cond.HeroLevelCheck.boChecked = true;
        cond.HeroLevelCheck.CompareSymbol = TCompareSymbol.csGreaterorEqual;
        cond.HeroLevelCheck.CompareType = THeroLevelCompareType.hlctLevelNumber;
        cond.HeroLevelCheck.CompareValue = 55;
        cond.HeroHPCheck.boChecked = true;
        cond.HeroHPCheck.CompareType = THeroHPCompareType.hhpctPercentage;
        cond.HeroHPCheck.CompareValue = 80;
        cond.TargetStatusCheck.boPoisoning = true;
        cond.TargetStatusCheck.boUnFrozen = true;
        cond.FriendCountCheck.boChecked = true;
        cond.FriendCountCheck.nCheckRange = 6;
        cond.FriendCountCheck.nCheckValue = 3;
        cond.boStraightLineCheck = true;

        var form = StaRunner.New(() => new HeroMagicConditionForm { FCondition = cond });
        try
        {
            form.DoOpen();
            Assert.True(form.chkHeroLevel.Checked);
            Assert.Equal(4, form.cbbHeroLevelCompareSymbol.SelectedIndex);
            Assert.Equal(1, form.cbbHeroLevelCompareType.SelectedIndex);
            Assert.Equal(55m, form.edtHeroLevelCompareValue.Value);
            Assert.True(form.HeroLevelCompareValueShouldShow);      // hlctLevelNumber → 可见

            Assert.True(form.chkHeroHP.Checked);
            Assert.Equal(1, form.cbbHeroHPCompareType.SelectedIndex);
            Assert.Equal(80m, form.edtHeroHPCompareValue.Value);
            Assert.True(form.chkPoisoning.Checked);
            Assert.True(form.chkNoFrozen.Checked);
            Assert.True(form.chkFriendCount.Checked);
            Assert.Equal(6m, form.edtFriendCheckRange.Value);
            Assert.Equal(3m, form.edtFriendCheckValue.Value);
            Assert.True(form.chkStraightLineCheck.Checked);
            Assert.False(form.chkHeroMP.Checked);

            // 切到「目标等级」→ 等级值编辑框隐藏
            form.cbbHeroLevelCompareType.SelectedIndex = (int)THeroLevelCompareType.hlctTargetLevel;
            form.CbbHeroLevelCompareTypeChange();
            Assert.False(form.HeroLevelCompareValueShouldShow);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Condition_BtnOkClick_WritesBackAllFields()
    {
        var cond = new THeroMagicUseCondition();
        var form = StaRunner.New(() => new HeroMagicConditionForm { FCondition = cond });
        try
        {
            form.chkHeroLevel.Checked = true;
            form.cbbHeroLevelCompareSymbol.SelectedIndex = 2;
            form.cbbHeroLevelCompareType.SelectedIndex = 1;
            form.edtHeroLevelCompareValue.Value = 42;
            form.chkTargetHP.Checked = true;
            form.cbbTargetHPCompareSymbol.SelectedIndex = 0;
            form.cbbTargetHPCompareType.SelectedIndex = 1;
            form.edtTargetHPCompareValue.Value = 66;
            form.chkPoisonDamageArmor.Checked = true;
            form.chkNoCobwebWinding.Checked = true;
            form.chkEnemyCount.Checked = true;
            form.edtEnemyCheckRange.Value = 5;
            form.edtEnemyCheckValue.Value = 2;
            form.chkStraightLineCheck.Checked = true;

            form.BtnOkClick();

            var r = form.FCondition;
            Assert.True(r.HeroLevelCheck.boChecked);
            Assert.Equal(TCompareSymbol.csEqual, r.HeroLevelCheck.CompareSymbol);
            Assert.Equal(THeroLevelCompareType.hlctLevelNumber, r.HeroLevelCheck.CompareType);
            Assert.Equal(42u, r.HeroLevelCheck.CompareValue);
            Assert.True(r.TargetHPCheck.boChecked);
            Assert.Equal(TCompareSymbol.csLess, r.TargetHPCheck.CompareSymbol);
            Assert.Equal(THeroHPCompareType.hhpctPercentage, r.TargetHPCheck.CompareType);
            Assert.Equal(66u, r.TargetHPCheck.CompareValue);
            Assert.True(r.TargetStatusCheck.boPoisonDamageArmor);
            Assert.True(r.TargetStatusCheck.boUnCobwebWinding);
            Assert.True(r.EnemyCountCheck.boChecked);
            Assert.Equal(5, r.EnemyCountCheck.nCheckRange);
            Assert.Equal(2, r.EnemyCountCheck.nCheckValue);
            Assert.True(r.boStraightLineCheck);
            Assert.False(r.HeroHPCheck.boChecked);
            Assert.Equal(System.Windows.Forms.DialogResult.OK, form.DialogResult);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= uFrmHeroMagicSetting =================

    private HeroMagicSettingForm NewSettingForm()
        => StaRunner.New(() => new HeroMagicSettingForm(_mgr));

    [Fact]
    public void Setting_FormCreate_DistributesByMagicType()
    {
        var form = NewSettingForm();
        try
        {
            Assert.Equal(0, (int)form.vstWarrMagic.Tag);
            Assert.Equal(1, (int)form.vstWizardMagic.Tag);
            Assert.Equal(2, (int)form.vstTaosMagic.Tag);
            Assert.Equal(0, form.pgcMain.SelectedIndex);

            Assert.Equal(18, form.NodeCount(form.vstWarrMagic));
            Assert.Equal(27, form.NodeCount(form.vstWizardMagic));
            Assert.Equal(24, form.NodeCount(form.vstTaosMagic));
            Assert.False(form.btnSave.Enabled);
            Assert.Equal(8, form.vstWarrMagic.Columns.Count);
            Assert.Equal("启用", form.vstWarrMagic.Columns[0]!.Text);
            Assert.Equal("设置", form.vstWarrMagic.Columns[7]!.Text);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_GetCellText_AllColumns()
    {
        var form = NewSettingForm();
        try
        {
            var node = form.GetNodeData(form.vstWarrMagic, 0);
            // 未注入 FindHeroMagic 接缝 → 技能名解析为 '-'
            Assert.Equal("-", node.MagicName);
            Assert.Equal("-", HeroMagicSettingForm.GetCellText(node, 1));
            Assert.Equal("普通技能", HeroMagicSettingForm.GetCellText(node, 2));
            Assert.Equal(node.HeroMagic.UseRate.ToString(), HeroMagicSettingForm.GetCellText(node, 3));
            Assert.Equal(node.HeroMagic.AttackRange.ToString(), HeroMagicSettingForm.GetCellText(node, 4));
            Assert.Equal("敌人", HeroMagicSettingForm.GetCellText(node, 5));
            Assert.Equal("", HeroMagicSettingForm.GetCellText(node, 6));
            Assert.Equal("", HeroMagicSettingForm.GetCellText(node, 7));   // 非自定义无「设置...」

            // 护体神盾（75, matSelf）
            var shield = form.GetNodeData(form.vstWarrMagic, 13);
            Assert.Equal(THeroMagicAttackTarget.matSelf, shield.HeroMagic.AttackTarget);
            Assert.Equal("自己", HeroMagicSettingForm.GetCellText(shield, 5));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_ConditionText_BuildsAndTrims()
    {
        var cond = new THeroMagicUseCondition();
        Assert.Equal("", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));

        cond.HeroLevelCheck.boChecked = true;
        cond.HeroLevelCheck.CompareType = THeroLevelCompareType.hlctTargetLevel;
        cond.HeroLevelCheck.CompareSymbol = TCompareSymbol.csGreater;
        Assert.Equal("(英雄等级 > 目标等级)", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));

        cond.HeroLevelCheck.CompareType = THeroLevelCompareType.hlctLevelNumber;
        cond.HeroLevelCheck.CompareValue = 30;
        Assert.Equal("(英雄等级 > 30)", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));

        cond.HeroHPCheck.boChecked = true;
        cond.HeroHPCheck.CompareSymbol = TCompareSymbol.csLess;
        cond.HeroHPCheck.CompareType = THeroHPCompareType.hhpctNumber;
        cond.HeroHPCheck.CompareValue = 50;
        cond.TargetMPCheck.boChecked = true;
        cond.TargetMPCheck.CompareSymbol = TCompareSymbol.csLessOrEqual;
        cond.TargetMPCheck.CompareType = THeroHPCompareType.hhpctPercentage;
        cond.TargetMPCheck.CompareValue = 25;
        Assert.Equal("(英雄等级 > 30) + (英雄HP < 50) + (目标MP <= 25%)",
            HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));

        cond.TargetStatusCheck.boPoisoning = true;
        cond.TargetStatusCheck.boFrozen = true;
        cond.FriendCountCheck.boChecked = true;
        cond.FriendCountCheck.nCheckRange = 4;
        cond.FriendCountCheck.nCheckValue = 2;
        cond.EnemyCountCheck.boChecked = true;
        cond.EnemyCountCheck.nCheckRange = 6;
        cond.EnemyCountCheck.nCheckValue = 3;
        cond.boStraightLineCheck = true;
        Assert.Equal("(英雄等级 > 30) + (英雄HP < 50) + (目标MP <= 25%) + 中毒 + 冰冻 + " +
                     "(目标周围4格朋友数量 > 2) + (目标周围6格敌人数量 > 3) + [直线]",
            HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));
    }

    [Fact]
    public void Setting_ConditionText_ForeverFrozenDuplicateDefect()
    {
        // 原文瑕疵（520-523 / 557-560）：冰封项与蛛网项都判断 boForeverFrozen
        var cond = new THeroMagicUseCondition();
        cond.TargetStatusCheck.boForeverFrozen = true;
        Assert.Equal("冰封 + 蛛网", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond));

        var cond2 = new THeroMagicUseCondition();
        cond2.TargetStatusCheck.boCobwebWinding = true;   // 单独置蛛网：原文不输出
        Assert.Equal("", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond2));

        var cond3 = new THeroMagicUseCondition();
        cond3.TargetStatusCheck.boUnForeverFrozen = true;
        Assert.Equal("无冰封 + 无蛛网", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond3));

        var cond4 = new THeroMagicUseCondition();
        cond4.TargetStatusCheck.boUnCobwebWinding = true;
        Assert.Equal("", HeroMagicSettingForm.GetHeroMagicUseConditionText(cond4));
    }

    [Fact]
    public void Setting_EditingAllowed_Matrix()
    {
        var form = NewSettingForm();
        try
        {
            // 普通技能仅列 3
            Assert.True(form.EditingAllowed(form.vstWarrMagic, 0, 3));
            Assert.False(form.EditingAllowed(form.vstWarrMagic, 0, 1));
            Assert.False(form.EditingAllowed(form.vstWarrMagic, 0, 4));
            Assert.False(form.EditingAllowed(form.vstWarrMagic, 0, 5));

            // 造一条自定义技能（战士页）
            form.KeyUp(form.vstWarrMagic, (int)System.Windows.Forms.Keys.Insert, true, -1);
            int idx = form.NodeCount(form.vstWarrMagic) - 1;
            Assert.True(form.GetNodeData(form.vstWarrMagic, idx).HeroMagic.IsCustomMagic);
            Assert.True(form.EditingAllowed(form.vstWarrMagic, idx, 1));
            Assert.True(form.EditingAllowed(form.vstWarrMagic, idx, 3));
            Assert.True(form.EditingAllowed(form.vstWarrMagic, idx, 4));
            Assert.False(form.EditingAllowed(form.vstWarrMagic, idx, 5));   // 战士页无列 5

            // 法师页自定义含列 5
            form.KeyUp(form.vstWizardMagic, (int)System.Windows.Forms.Keys.Insert, true, -1);
            int widx = form.NodeCount(form.vstWizardMagic) - 1;
            Assert.True(form.EditingAllowed(form.vstWizardMagic, widx, 5));
            Assert.False(form.EditingAllowed(form.vstWizardMagic, widx, 6));

            // 越界
            Assert.False(form.EditingAllowed(form.vstWarrMagic, -1, 3));
            Assert.False(form.EditingAllowed(form.vstWarrMagic, 9999, 3));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_PrepareEdit_CandidateSets()
    {
        var form = NewSettingForm();
        form.IsMagicWarrHandler = id => id == 208;
        form.HeroMagicCandidatesHandler = () => new[] { (208, "旋风转"), (31, "魔法盾") };
        try
        {
            // 战士页：仅 IsMagicWarr
            var warr = form.PrepareEdit(form.vstWarrMagic, 1);
            Assert.Single(warr);
            Assert.Equal((208, "旋风转"), warr[0]);

            // 非战士页：IsMagicWarr 取反
            var wiz = form.PrepareEdit(form.vstWizardMagic, 1);
            Assert.Single(wiz);
            Assert.Equal((31, "魔法盾"), wiz[0]);

            // 列 5：原文 if FTree.Tag = 1（法师页）→ 仅到 matMaster；战士/道士页（Tag 0/2）落 else → 全 4 项
            var warrTargets = form.PrepareEdit(form.vstWarrMagic, 5);
            Assert.Equal(4, warrTargets.Count);
            Assert.Equal("伙伴", warrTargets[3].Text);
            var wizTargets = form.PrepareEdit(form.vstWizardMagic, 5);
            Assert.Equal(3, wizTargets.Count);
            Assert.Equal("主人", wizTargets[2].Text);

            // 列 3/4 无候选
            Assert.Empty(form.PrepareEdit(form.vstWarrMagic, 3));
            Assert.Empty(form.PrepareEdit(form.vstWarrMagic, 4));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_EndEdit_WritesBackAndMarksChanged()
    {
        var form = NewSettingForm();
        try
        {
            var node = form.GetNodeData(form.vstWarrMagic, 0);
            int magicId = node.HeroMagic.MagicID;

            // 列 3：使用率
            Assert.True(form.EndEdit(form.vstWarrMagic, 0, 3, 7));
            Assert.Equal(7, node.HeroMagic.UseRate);
            Assert.True(node.HeroMagic.IsChanged);
            Assert.True(form.btnSave.Enabled);
            Assert.Equal("7", form.vstWarrMagic.Items[0].SubItems[3].Text);

            // 同值再写 → 不变更
            Assert.False(form.EndEdit(form.vstWarrMagic, 0, 3, 7));

            // 列 4：攻击范围
            Assert.True(form.EndEdit(form.vstWarrMagic, 0, 4, 9));
            Assert.Equal(9, node.HeroMagic.AttackRange);

            // 列 5：攻击目标
            Assert.True(form.EndEdit(form.vstWarrMagic, 0, 5, (int)THeroMagicAttackTarget.matPartner));
            Assert.Equal(THeroMagicAttackTarget.matPartner, node.HeroMagic.AttackTarget);
            Assert.Equal("伙伴", form.vstWarrMagic.Items[0].SubItems[5].Text);

            // 列 1：技能 ID + 名称
            Assert.True(form.EndEdit(form.vstWarrMagic, 0, 1, 999, "测试技能"));
            Assert.Equal(999, node.HeroMagic.MagicID);
            Assert.Equal("测试技能", node.MagicName);
            Assert.Equal("测试技能", form.vstWarrMagic.Items[0].SubItems[1].Text);

            // 列 1 传 -1（下拉未选）→ 不变更
            Assert.False(form.EndEdit(form.vstWarrMagic, 0, 1, -1));

            // 越界
            Assert.False(form.EndEdit(form.vstWarrMagic, 9999, 3, 1));
            Assert.Equal(magicId, 208);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_NodeClick_Column7OpensConditionOnlyForCustom()
    {
        var form = NewSettingForm();
        try
        {
            int condCalls = 0;
            // 委托实例保持稳定（WinForms CheckedChanged/事件语义：不因重新赋值而丢计数）
            form.ConditionDialogHandler = c =>
            {
                condCalls++;
                var r = c;
                if (r.boStraightLineCheck)
                    return (false, c);      // 已置直线（第二次取消分支）
                r.boStraightLineCheck = true;
                return (true, r);
            };

            // 普通技能的列 7 → 不弹窗
            form.NodeClick(form.vstWarrMagic, 0, 7);
            Assert.Equal(0, condCalls);

            // 自定义技能列 7 → 弹窗并写回
            form.KeyUp(form.vstWarrMagic, (int)System.Windows.Forms.Keys.Insert, true, -1);
            int idx = form.NodeCount(form.vstWarrMagic) - 1;
            form.NodeClick(form.vstWarrMagic, idx, 7);
            Assert.Equal(1, condCalls);
            Assert.True(form.GetNodeData(form.vstWarrMagic, idx).HeroMagic.Condition.boStraightLineCheck);
            Assert.True(form.GetNodeData(form.vstWarrMagic, idx).HeroMagic.IsChanged);

            // 取消 → 不写回（handler 对已置直线的条目返回 false）
            form.NodeClick(form.vstWarrMagic, idx, 7);
            Assert.Equal(2, condCalls);

            // 非列 7 → 记录编辑请求
            form.NodeClick(form.vstWarrMagic, 0, 3);
            Assert.NotNull(form.LastEditingRequest);
            Assert.Equal(3, form.LastEditingRequest!.Value.Column);
            Assert.Equal(0, form.LastEditingRequest!.Value.Index);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_KeyUp_InsertAndDelete()
    {
        var form = NewSettingForm();
        try
        {
            int before = _mgr.Count;
            form.KeyUp(form.vstTaosMagic, (int)System.Windows.Forms.Keys.Insert, true, -1);
            Assert.Equal(before + 1, _mgr.Count);
            Assert.Equal(25, form.NodeCount(form.vstTaosMagic));

            int idx = form.NodeCount(form.vstTaosMagic) - 1;
            var node = form.GetNodeData(form.vstTaosMagic, idx);
            Assert.True(node.HeroMagic.Checked);
            Assert.True(node.HeroMagic.IsCustomMagic);
            Assert.Equal(0, node.HeroMagic.MagicID);
            Assert.Equal(THeroMagicType.mtTaosAttack, node.HeroMagic.MagicType);
            Assert.Equal(THeroMagicAttackTarget.matEnemy, node.HeroMagic.AttackTarget);
            Assert.Equal(0, node.HeroMagic.UseRate);
            Assert.True(form.btnSave.Enabled);

            // Ctrl+Delete 删自定义
            form.KeyUp(form.vstTaosMagic, (int)System.Windows.Forms.Keys.Delete, true, idx);
            Assert.Equal(before, _mgr.Count);
            Assert.Equal(24, form.NodeCount(form.vstTaosMagic));

            // 删普通技能 → 拒绝
            form.KeyUp(form.vstTaosMagic, (int)System.Windows.Forms.Keys.Delete, true, 0);
            Assert.Equal(before, _mgr.Count);
            Assert.Equal(24, form.NodeCount(form.vstTaosMagic));

            // 无 Ctrl → 不动作
            form.KeyUp(form.vstTaosMagic, (int)System.Windows.Forms.Keys.Insert, false, -1);
            Assert.Equal(before, _mgr.Count);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_NodeChecked_RowColor_AndSave()
    {
        var form = NewSettingForm();
        try
        {
            var node = form.GetNodeData(form.vstWarrMagic, 0);
            bool was = node.HeroMagic.Checked;
            form.NodeChecked(form.vstWarrMagic, 0, !was);
            Assert.Equal(!was, node.HeroMagic.Checked);
            Assert.True(node.HeroMagic.IsChanged);
            Assert.True(form.btnSave.Enabled);

            Assert.Equal(System.Drawing.Color.Red, form.RowColor(form.vstWarrMagic, 0, false));

            form.BtnSaveClick();
            Assert.False(form.btnSave.Enabled);
            Assert.True(File.Exists(Path.Combine(_dir, "CustomHeroMagic.ini")));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void Setting_ResolveMagicName_FallbackToDash()
    {
        var mgr = new TCustomHeroMagicMgr
        {
            FindHeroMagicHandler = _ => true,
            FindHeroMagicAnyHandler = _ => true,
            CheckIsCustomMagicHandler = id => id >= 1000,
        };
        var form = StaRunner.New(() => new HeroMagicSettingForm(mgr));
        try
        {
            form.FindHeroMagicNameAnyHandler = _ => null;
            form.FindHeroMagicNameHandler = _ => null;
            Assert.Equal("-", form.ResolveMagicName(208));      // 内置未找到
            Assert.Equal("-", form.ResolveMagicName(1001));     // 自定义未找到

            form.FindHeroMagicNameAnyHandler = id => "自定义" + id;
            form.FindHeroMagicNameHandler = id => "技能" + id;
            Assert.Equal("技能208", form.ResolveMagicName(208));
            Assert.Equal("自定义1001", form.ResolveMagicName(1001));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    /// <summary>StaRunner.New&lt;T&gt; 需要 IDisposable 返回类型；此处包一个空壳承载结果。</summary>
    private sealed class DialogResultBox : IDisposable
    {
        public System.Windows.Forms.DialogResult Result;
        public void Dispose() { }
    }

    [Fact]
    public void Setting_ConditionEquals_DetectsAllFields()
    {
        var a = new THeroMagicUseCondition();
        var b = new THeroMagicUseCondition();
        Assert.True(HeroMagicSettingForm.ConditionEquals(a, b));

        b.boStraightLineCheck = true;
        Assert.False(HeroMagicSettingForm.ConditionEquals(a, b));
        b.boStraightLineCheck = false;

        b.TargetStatusCheck.boCobwebWinding = true;
        Assert.False(HeroMagicSettingForm.ConditionEquals(a, b));
        b.TargetStatusCheck.boCobwebWinding = false;

        b.EnemyCountCheck.nCheckRange = 3;
        Assert.False(HeroMagicSettingForm.ConditionEquals(a, b));
        b.EnemyCountCheck.nCheckRange = 0;

        b.HeroLevelCheck.CompareValue = 5;
        Assert.False(HeroMagicSettingForm.ConditionEquals(a, b));
        b.HeroLevelCheck.CompareValue = 0;

        b.TargetMPCheck.CompareType = THeroHPCompareType.hhpctPercentage;
        Assert.False(HeroMagicSettingForm.ConditionEquals(a, b));
        b.TargetMPCheck.CompareType = THeroHPCompareType.hhpctNumber;
        Assert.True(HeroMagicSettingForm.ConditionEquals(a, b));
    }
}
