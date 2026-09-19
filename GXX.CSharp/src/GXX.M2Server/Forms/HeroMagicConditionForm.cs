using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// uFrmHeroMagicCondition.pas 1:1（批次J62）：英雄技能使用条件设置对话框。
/// DoOpen 回填 5 组「等级/HP/MP/目标HP/目标MP」比较 + 14 个目标状态开关 +
/// 友/敌数量检查 + 直线检查；btnOKClick 反向回写；FormCreate 填 5 个比较符下拉、
/// 1 个等级比较类型下拉、4 个 HP/MP 比较类型下拉；
/// cbbHeroLevelCompareTypeChange：等级值编辑框仅在「固定等级」时可见。
/// </summary>
public sealed class HeroMagicConditionForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.GroupBox grp1 = null!;
    public System.Windows.Forms.GroupBox grp2 = null!;
    public System.Windows.Forms.GroupBox grp3 = null!;

    // ---- 五组比较（等级 / 英雄HP / 英雄MP / 目标HP / 目标MP）----
    public System.Windows.Forms.CheckBox chkHeroLevel = null!;
    public System.Windows.Forms.ComboBox cbbHeroLevelCompareSymbol = null!;
    public System.Windows.Forms.ComboBox cbbHeroLevelCompareType = null!;
    public System.Windows.Forms.NumericUpDown edtHeroLevelCompareValue = null!;

    public System.Windows.Forms.CheckBox chkHeroHP = null!;
    public System.Windows.Forms.ComboBox cbbHeroHPCompareSymbol = null!;
    public System.Windows.Forms.ComboBox cbbHeroHPCompareType = null!;
    public System.Windows.Forms.NumericUpDown edtHeroHPCompareValue = null!;

    public System.Windows.Forms.CheckBox chkHeroMP = null!;
    public System.Windows.Forms.ComboBox cbbHeroMPCompareSymbol = null!;
    public System.Windows.Forms.ComboBox cbbHeroMPCompareType = null!;
    public System.Windows.Forms.NumericUpDown edtHeroMPCompareValue = null!;

    public System.Windows.Forms.CheckBox chkTargetHP = null!;
    public System.Windows.Forms.ComboBox cbbTargetHPCompareSymbol = null!;
    public System.Windows.Forms.ComboBox cbbTargetHPCompareType = null!;
    public System.Windows.Forms.NumericUpDown edtTargetHPCompareValue = null!;

    public System.Windows.Forms.CheckBox chkTargetMP = null!;
    public System.Windows.Forms.ComboBox cbbTargetMPCompareSymbol = null!;
    public System.Windows.Forms.ComboBox cbbTargetMPCompareType = null!;
    public System.Windows.Forms.NumericUpDown edtTargetMPCompareValue = null!;

    // ---- 目标状态（7 个「需具备」+ 7 个「需不具备」）----
    public System.Windows.Forms.CheckBox chkPoisonDamageArmor = null!;
    public System.Windows.Forms.CheckBox chkPoisonDecHealth = null!;
    public System.Windows.Forms.CheckBox chkPoisoning = null!;
    public System.Windows.Forms.CheckBox chkPoisonStone = null!;
    public System.Windows.Forms.CheckBox chkFrozen = null!;
    public System.Windows.Forms.CheckBox chkForeverFrozen = null!;
    public System.Windows.Forms.CheckBox chkCobwebWinding = null!;

    public System.Windows.Forms.CheckBox chkNoPoisonDamageArmor = null!;
    public System.Windows.Forms.CheckBox chkNoPoisonDecHealth = null!;
    public System.Windows.Forms.CheckBox chkNoPoisoning = null!;
    public System.Windows.Forms.CheckBox chkNoPoisonStone = null!;
    public System.Windows.Forms.CheckBox chkNoFrozen = null!;
    public System.Windows.Forms.CheckBox chkNoForeverFrozen = null!;
    public System.Windows.Forms.CheckBox chkNoCobwebWinding = null!;

    // ---- 数量检查与直线检查 ----
    public System.Windows.Forms.CheckBox chkFriendCount = null!;
    public System.Windows.Forms.NumericUpDown edtFriendCheckRange = null!;
    public System.Windows.Forms.NumericUpDown edtFriendCheckValue = null!;
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label Label1 = null!;

    public System.Windows.Forms.CheckBox chkEnemyCount = null!;
    public System.Windows.Forms.NumericUpDown edtEnemyCheckRange = null!;
    public System.Windows.Forms.NumericUpDown edtEnemyCheckValue = null!;

    public System.Windows.Forms.CheckBox chkStraightLineCheck = null!;

    public System.Windows.Forms.Button btnOK = null!;

    /// <summary>FCondition：当前编辑的条件记录。</summary>
    public THeroMagicUseCondition FCondition;

    /// <summary>ShowModal 接缝（测试可跳过真实模态）。</summary>
    public Func<System.Windows.Forms.DialogResult>? ShowModalHandler;

    public HeroMagicConditionForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private static System.Windows.Forms.NumericUpDown NewSpin()
        => new() { Minimum = 0, Maximum = uint.MaxValue, Width = 90 };

    private static System.Windows.Forms.ComboBox NewCombo()
        => new() { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 90 };

    private void InitializeComponent()
    {
        Text = "技能使用条件";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ClientSize = new System.Drawing.Size(640, 470);

        grp1 = new System.Windows.Forms.GroupBox { Text = "条件", Left = 8, Top = 4, Width = 620, Height = 250 };

        // 五行五列布局（编号/使能/比较符/类型/值）
        string[] rowLabels = { "英雄等级", "英雄HP", "英雄MP", "目标HP", "目标MP" };
        chkHeroLevel = NewChk(grp1, rowLabels[0], 12, 24);
        cbbHeroLevelCompareSymbol = NewCombo(); cbbHeroLevelCompareSymbol.SetBounds(120, 22, 60, 22);
        cbbHeroLevelCompareType = NewCombo(); cbbHeroLevelCompareType.SetBounds(188, 22, 96, 22);
        cbbHeroLevelCompareType.SelectedIndexChanged += (_, _) => CbbHeroLevelCompareTypeChange();
        edtHeroLevelCompareValue = NewSpin(); edtHeroLevelCompareValue.SetBounds(292, 22, 90, 22);

        chkHeroHP = NewChk(grp1, rowLabels[1], 12, 66);
        cbbHeroHPCompareSymbol = NewCombo(); cbbHeroHPCompareSymbol.SetBounds(120, 64, 60, 22);
        cbbHeroHPCompareType = NewCombo(); cbbHeroHPCompareType.SetBounds(188, 64, 96, 22);
        edtHeroHPCompareValue = NewSpin(); edtHeroHPCompareValue.SetBounds(292, 64, 90, 22);

        chkHeroMP = NewChk(grp1, rowLabels[2], 12, 108);
        cbbHeroMPCompareSymbol = NewCombo(); cbbHeroMPCompareSymbol.SetBounds(120, 106, 60, 22);
        cbbHeroMPCompareType = NewCombo(); cbbHeroMPCompareType.SetBounds(188, 106, 96, 22);
        edtHeroMPCompareValue = NewSpin(); edtHeroMPCompareValue.SetBounds(292, 106, 90, 22);

        chkTargetHP = NewChk(grp1, rowLabels[3], 12, 150);
        cbbTargetHPCompareSymbol = NewCombo(); cbbTargetHPCompareSymbol.SetBounds(120, 148, 60, 22);
        cbbTargetHPCompareType = NewCombo(); cbbTargetHPCompareType.SetBounds(188, 148, 96, 22);
        edtTargetHPCompareValue = NewSpin(); edtTargetHPCompareValue.SetBounds(292, 148, 90, 22);

        chkTargetMP = NewChk(grp1, rowLabels[4], 12, 192);
        cbbTargetMPCompareSymbol = NewCombo(); cbbTargetMPCompareSymbol.SetBounds(120, 190, 60, 22);
        cbbTargetMPCompareType = NewCombo(); cbbTargetMPCompareType.SetBounds(188, 190, 96, 22);
        edtTargetMPCompareValue = NewSpin(); edtTargetMPCompareValue.SetBounds(292, 190, 90, 22);

        grp1.Controls.Add(cbbHeroLevelCompareSymbol);
        grp1.Controls.Add(cbbHeroLevelCompareType);
        grp1.Controls.Add(edtHeroLevelCompareValue);
        grp1.Controls.Add(cbbHeroHPCompareSymbol);
        grp1.Controls.Add(cbbHeroHPCompareType);
        grp1.Controls.Add(edtHeroHPCompareValue);
        grp1.Controls.Add(cbbHeroMPCompareSymbol);
        grp1.Controls.Add(cbbHeroMPCompareType);
        grp1.Controls.Add(edtHeroMPCompareValue);
        grp1.Controls.Add(cbbTargetHPCompareSymbol);
        grp1.Controls.Add(cbbTargetHPCompareType);
        grp1.Controls.Add(edtTargetHPCompareValue);
        grp1.Controls.Add(cbbTargetMPCompareSymbol);
        grp1.Controls.Add(cbbTargetMPCompareType);
        grp1.Controls.Add(edtTargetMPCompareValue);

        grp2 = new System.Windows.Forms.GroupBox { Text = "目标状态", Left = 8, Top = 258, Width = 452, Height = 204 };
        chkPoisonDamageArmor = NewChk(grp2, "红毒", 12, 22);
        chkPoisonDecHealth = NewChk(grp2, "绿毒", 12, 46);
        chkPoisoning = NewChk(grp2, "中毒", 12, 70);
        chkPoisonStone = NewChk(grp2, "石化", 12, 94);
        chkFrozen = NewChk(grp2, "冰冻", 12, 118);
        chkForeverFrozen = NewChk(grp2, "永恒冰冻", 12, 142);
        chkCobwebWinding = NewChk(grp2, "蛛网", 12, 166);

        chkNoPoisonDamageArmor = NewChk(grp2, "防红毒", 152, 22);
        chkNoPoisonDecHealth = NewChk(grp2, "防绿毒", 152, 46);
        chkNoPoisoning = NewChk(grp2, "防中毒", 152, 70);
        chkNoPoisonStone = NewChk(grp2, "防石化", 152, 94);
        chkNoFrozen = NewChk(grp2, "防冰冻", 152, 118);
        chkNoForeverFrozen = NewChk(grp2, "防永恒冰冻", 152, 142);
        chkNoCobwebWinding = NewChk(grp2, "防蛛网", 152, 166);

        chkStraightLineCheck = NewChk(grp2, "直线检查", 292, 22);

        grp3 = new System.Windows.Forms.GroupBox { Text = "数量检查", Left = 468, Top = 258, Width = 160, Height = 204 };
        lbl1 = new System.Windows.Forms.Label { Text = "范围", Left = 12, Top = 22, AutoSize = true };
        Label1 = new System.Windows.Forms.Label { Text = "数量", Left = 12, Top = 54, AutoSize = true };

        chkFriendCount = NewChk(grp3, "友方", 12, 84);
        edtFriendCheckRange = NewSpin(); edtFriendCheckRange.SetBounds(12, 108, 130, 22);
        edtFriendCheckValue = NewSpin(); edtFriendCheckValue.SetBounds(12, 134, 130, 22);

        chkEnemyCount = NewChk(grp3, "敌方", 12, 160);
        edtEnemyCheckRange = NewSpin(); edtEnemyCheckRange.SetBounds(72, 158, 70, 22);
        edtEnemyCheckValue = NewSpin(); edtEnemyCheckValue.SetBounds(72, 182, 70, 22);
        grp3.Controls.Add(lbl1);
        grp3.Controls.Add(Label1);
        grp3.Controls.Add(edtFriendCheckRange);
        grp3.Controls.Add(edtFriendCheckValue);
        grp3.Controls.Add(edtEnemyCheckRange);
        grp3.Controls.Add(edtEnemyCheckValue);

        btnOK = new System.Windows.Forms.Button { Text = "确定", Left = 548, Top = 470, Width = 80, Height = 26 };
        btnOK.Click += (_, _) => BtnOkClick();

        Controls.Add(grp1);
        Controls.Add(grp2);
        Controls.Add(grp3);
        Controls.Add(btnOK);
        AcceptButton = btnOK;
    }

    private static System.Windows.Forms.CheckBox NewChk(System.Windows.Forms.Control parent, string text, int left, int top)
    {
        var chk = new System.Windows.Forms.CheckBox { Text = text, Left = left, Top = top, AutoSize = true };
        parent.Controls.Add(chk);
        return chk;
    }

    /// <summary>FormCreate（203-242）：填比较符/等级比较类型/HP·MP 比较类型四类下拉。</summary>
    public void FormCreate()
    {
        var symbolCombos = new[]
        {
            cbbHeroLevelCompareSymbol, cbbHeroHPCompareSymbol, cbbHeroMPCompareSymbol,
            cbbTargetHPCompareSymbol, cbbTargetMPCompareSymbol,
        };
        foreach (var cbb in symbolCombos)
            cbb.Items.Clear();
        for (int i = 0; i < CustomHeroMagicState.CompareSymbolNames.Length; i++)
        {
            foreach (var cbb in symbolCombos)
                cbb.Items.Add(CustomHeroMagicState.CompareSymbolNames[i]);
        }

        cbbHeroLevelCompareType.Items.Clear();
        for (int i = 0; i < CustomHeroMagicState.HeroLevelCompareTypeNames.Length; i++)
            cbbHeroLevelCompareType.Items.Add(CustomHeroMagicState.HeroLevelCompareTypeNames[i]);

        var hpTypeCombos = new[] { cbbHeroHPCompareType, cbbHeroMPCompareType, cbbTargetHPCompareType, cbbTargetMPCompareType };
        foreach (var cbb in hpTypeCombos)
            cbb.Items.Clear();
        for (int i = 0; i < CustomHeroMagicState.HeroHPCompareTypeNames.Length; i++)
        {
            foreach (var cbb in hpTypeCombos)
                cbb.Items.Add(CustomHeroMagicState.HeroHPCompareTypeNames[i]);
        }
    }

    /// <summary>
    /// 等级值编辑框的可见性判定（hlctLevelNumber ⇔ 可见）。
    /// 句柄未创建时 WinForms 的 Visible getter 恒为 false，故另存判定结果供测试与逻辑共用。
    /// </summary>
    public bool HeroLevelCompareValueShouldShow
        => (THeroLevelCompareType)cbbHeroLevelCompareType.SelectedIndex == THeroLevelCompareType.hlctLevelNumber;

    /// <summary>cbbHeroLevelCompareTypeChange（244-248）：仅「固定等级」时显示等级值编辑框。</summary>
    public void CbbHeroLevelCompareTypeChange()
        => edtHeroLevelCompareValue.Visible = HeroLevelCompareValueShouldShow;

    /// <summary>DoOpen（91-145）1:1：条件 → 控件回填。</summary>
    public void DoOpen()
    {
        chkHeroLevel.Checked = FCondition.HeroLevelCheck.boChecked;
        cbbHeroLevelCompareSymbol.SelectedIndex = (int)FCondition.HeroLevelCheck.CompareSymbol;
        cbbHeroLevelCompareType.SelectedIndex = (int)FCondition.HeroLevelCheck.CompareType;
        edtHeroLevelCompareValue.Value = Clamp(FCondition.HeroLevelCheck.CompareValue, edtHeroLevelCompareValue);

        edtHeroLevelCompareValue.Visible = HeroLevelCompareValueShouldShow;

        chkHeroHP.Checked = FCondition.HeroHPCheck.boChecked;
        cbbHeroHPCompareSymbol.SelectedIndex = (int)FCondition.HeroHPCheck.CompareSymbol;
        cbbHeroHPCompareType.SelectedIndex = (int)FCondition.HeroHPCheck.CompareType;
        edtHeroHPCompareValue.Value = Clamp(FCondition.HeroHPCheck.CompareValue, edtHeroHPCompareValue);

        chkHeroMP.Checked = FCondition.HeroMPCheck.boChecked;
        cbbHeroMPCompareSymbol.SelectedIndex = (int)FCondition.HeroMPCheck.CompareSymbol;
        cbbHeroMPCompareType.SelectedIndex = (int)FCondition.HeroMPCheck.CompareType;
        edtHeroMPCompareValue.Value = Clamp(FCondition.HeroMPCheck.CompareValue, edtHeroMPCompareValue);

        chkTargetHP.Checked = FCondition.TargetHPCheck.boChecked;
        cbbTargetHPCompareSymbol.SelectedIndex = (int)FCondition.TargetHPCheck.CompareSymbol;
        cbbTargetHPCompareType.SelectedIndex = (int)FCondition.TargetHPCheck.CompareType;
        edtTargetHPCompareValue.Value = Clamp(FCondition.TargetHPCheck.CompareValue, edtTargetHPCompareValue);

        chkTargetMP.Checked = FCondition.TargetMPCheck.boChecked;
        cbbTargetMPCompareSymbol.SelectedIndex = (int)FCondition.TargetMPCheck.CompareSymbol;
        cbbTargetMPCompareType.SelectedIndex = (int)FCondition.TargetMPCheck.CompareType;
        edtTargetMPCompareValue.Value = Clamp(FCondition.TargetMPCheck.CompareValue, edtTargetMPCompareValue);

        chkPoisonDamageArmor.Checked = FCondition.TargetStatusCheck.boPoisonDamageArmor;
        chkPoisonDecHealth.Checked = FCondition.TargetStatusCheck.boPoisonDecHealth;
        chkPoisoning.Checked = FCondition.TargetStatusCheck.boPoisoning;
        chkPoisonStone.Checked = FCondition.TargetStatusCheck.boPoisonStone;
        chkFrozen.Checked = FCondition.TargetStatusCheck.boFrozen;
        chkForeverFrozen.Checked = FCondition.TargetStatusCheck.boForeverFrozen;
        chkCobwebWinding.Checked = FCondition.TargetStatusCheck.boCobwebWinding;

        chkNoPoisonDamageArmor.Checked = FCondition.TargetStatusCheck.boUnPoisonDamageArmor;
        chkNoPoisonDecHealth.Checked = FCondition.TargetStatusCheck.boUnPoisonDecHealth;
        chkNoPoisoning.Checked = FCondition.TargetStatusCheck.boUnPoisoning;
        chkNoPoisonStone.Checked = FCondition.TargetStatusCheck.boUnPoisonStone;
        chkNoFrozen.Checked = FCondition.TargetStatusCheck.boUnFrozen;
        chkNoForeverFrozen.Checked = FCondition.TargetStatusCheck.boUnForeverFrozen;
        chkNoCobwebWinding.Checked = FCondition.TargetStatusCheck.boUnCobwebWinding;

        chkFriendCount.Checked = FCondition.FriendCountCheck.boChecked;
        edtFriendCheckRange.Value = ClampSigned(FCondition.FriendCountCheck.nCheckRange, edtFriendCheckRange);
        edtFriendCheckValue.Value = ClampSigned(FCondition.FriendCountCheck.nCheckValue, edtFriendCheckValue);

        chkEnemyCount.Checked = FCondition.EnemyCountCheck.boChecked;
        edtEnemyCheckRange.Value = ClampSigned(FCondition.EnemyCountCheck.nCheckRange, edtEnemyCheckRange);
        edtEnemyCheckValue.Value = ClampSigned(FCondition.EnemyCountCheck.nCheckValue, edtEnemyCheckValue);

        chkStraightLineCheck.Checked = FCondition.boStraightLineCheck;
    }

    private static decimal Clamp(uint value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp((decimal)value, control.Minimum, control.Maximum);

    private static decimal ClampSigned(int value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp((decimal)value, control.Minimum, control.Maximum);

    /// <summary>btnOKClick（147-201）1:1：控件 → 条件回写并 mrOk。</summary>
    public void BtnOkClick()
    {
        FCondition.HeroLevelCheck.boChecked = chkHeroLevel.Checked;
        FCondition.HeroLevelCheck.CompareSymbol = (TCompareSymbol)cbbHeroLevelCompareSymbol.SelectedIndex;
        FCondition.HeroLevelCheck.CompareType = (THeroLevelCompareType)cbbHeroLevelCompareType.SelectedIndex;
        FCondition.HeroLevelCheck.CompareValue = (uint)edtHeroLevelCompareValue.Value;

        FCondition.HeroHPCheck.boChecked = chkHeroHP.Checked;
        FCondition.HeroHPCheck.CompareSymbol = (TCompareSymbol)cbbHeroHPCompareSymbol.SelectedIndex;
        FCondition.HeroHPCheck.CompareType = (THeroHPCompareType)cbbHeroHPCompareType.SelectedIndex;
        FCondition.HeroHPCheck.CompareValue = (uint)edtHeroHPCompareValue.Value;

        FCondition.HeroMPCheck.boChecked = chkHeroMP.Checked;
        FCondition.HeroMPCheck.CompareSymbol = (TCompareSymbol)cbbHeroMPCompareSymbol.SelectedIndex;
        FCondition.HeroMPCheck.CompareType = (THeroHPCompareType)cbbHeroMPCompareType.SelectedIndex;
        FCondition.HeroMPCheck.CompareValue = (uint)edtHeroMPCompareValue.Value;

        FCondition.TargetHPCheck.boChecked = chkTargetHP.Checked;
        FCondition.TargetHPCheck.CompareSymbol = (TCompareSymbol)cbbTargetHPCompareSymbol.SelectedIndex;
        FCondition.TargetHPCheck.CompareType = (THeroHPCompareType)cbbTargetHPCompareType.SelectedIndex;
        FCondition.TargetHPCheck.CompareValue = (uint)edtTargetHPCompareValue.Value;

        FCondition.TargetMPCheck.boChecked = chkTargetMP.Checked;
        FCondition.TargetMPCheck.CompareSymbol = (TCompareSymbol)cbbTargetMPCompareSymbol.SelectedIndex;
        FCondition.TargetMPCheck.CompareType = (THeroHPCompareType)cbbTargetMPCompareType.SelectedIndex;
        FCondition.TargetMPCheck.CompareValue = (uint)edtTargetMPCompareValue.Value;

        FCondition.TargetStatusCheck.boPoisonDamageArmor = chkPoisonDamageArmor.Checked;
        FCondition.TargetStatusCheck.boPoisonDecHealth = chkPoisonDecHealth.Checked;
        FCondition.TargetStatusCheck.boPoisoning = chkPoisoning.Checked;
        FCondition.TargetStatusCheck.boPoisonStone = chkPoisonStone.Checked;
        FCondition.TargetStatusCheck.boFrozen = chkFrozen.Checked;
        FCondition.TargetStatusCheck.boForeverFrozen = chkForeverFrozen.Checked;
        FCondition.TargetStatusCheck.boCobwebWinding = chkCobwebWinding.Checked;

        FCondition.TargetStatusCheck.boUnPoisonDamageArmor = chkNoPoisonDamageArmor.Checked;
        FCondition.TargetStatusCheck.boUnPoisonDecHealth = chkNoPoisonDecHealth.Checked;
        FCondition.TargetStatusCheck.boUnPoisoning = chkNoPoisoning.Checked;
        FCondition.TargetStatusCheck.boUnPoisonStone = chkNoPoisonStone.Checked;
        FCondition.TargetStatusCheck.boUnFrozen = chkNoFrozen.Checked;
        FCondition.TargetStatusCheck.boUnForeverFrozen = chkNoForeverFrozen.Checked;
        FCondition.TargetStatusCheck.boUnCobwebWinding = chkNoCobwebWinding.Checked;

        FCondition.FriendCountCheck.boChecked = chkFriendCount.Checked;
        FCondition.FriendCountCheck.nCheckRange = (int)edtFriendCheckRange.Value;
        FCondition.FriendCountCheck.nCheckValue = (int)edtFriendCheckValue.Value;

        FCondition.EnemyCountCheck.boChecked = chkEnemyCount.Checked;
        FCondition.EnemyCountCheck.nCheckRange = (int)edtEnemyCheckRange.Value;
        FCondition.EnemyCountCheck.nCheckValue = (int)edtEnemyCheckValue.Value;

        FCondition.boStraightLineCheck = chkStraightLineCheck.Checked;

        DialogResult = System.Windows.Forms.DialogResult.OK;
    }

    /// <summary>
    /// ShowFrmHeroMagicCondition（75-87）1:1：DoOpen 回填 → ShowModal；
    /// 确认时 btnOKClick 已就地改写 Condition（Delphi 传引用语义），返回是否确认。
    /// </summary>
    public static bool ShowFrmHeroMagicCondition(ref THeroMagicUseCondition condition)
    {
        var form = new HeroMagicConditionForm { FCondition = condition };
        try
        {
            form.DoOpen();
            var result = form.ShowModalHandler?.Invoke() ?? form.ShowDialog();
            bool ok = result == System.Windows.Forms.DialogResult.OK;
            if (ok)
                condition = form.FCondition;
            return ok;
        }
        finally
        {
            form.Dispose();
        }
    }
}
