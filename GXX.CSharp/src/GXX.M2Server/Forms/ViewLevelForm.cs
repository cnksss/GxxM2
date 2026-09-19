using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewLevel.pas TfrmViewLevel 1:1 转换（等级/基础属性查看器：系统默认公式 + 自定义表）。
/// RecalcHuman 等效链：RecalcLevelAbilitys(True) → RecalcAbilitys（装备叠加，装备系统批次接入）→ HasLevelUp。
/// </summary>
public sealed class ViewLevelForm : System.Windows.Forms.Form
{
    /// <summary>Delphi PlayObject（临时查看对象）。</summary>
    public readonly TPlayObject PlayObject = new();
    /// <summary>Delphi HeroObject（临时查看英雄）。</summary>
    public readonly TPlayObject HeroObject = new();

    public System.Windows.Forms.RadioButton rbDef = null!;
    public System.Windows.Forms.RadioButton rbCustom = null!;
    public System.Windows.Forms.ComboBox cbbDefUserType = null!;
    public System.Windows.Forms.ComboBox cbbDefJob = null!;
    public System.Windows.Forms.NumericUpDown seDefLevel = null!;
    public System.Windows.Forms.DataGridView GridHumanInfo = null!;
    public System.Windows.Forms.Button btnSave = null!;

    public ViewLevelForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "等级属性查看";
        Width = 480;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        rbDef = new System.Windows.Forms.RadioButton { Text = "系统默认", Left = 10, Top = 10, AutoSize = true, Checked = true };
        rbCustom = new System.Windows.Forms.RadioButton { Text = "自定义", Left = 100, Top = 10, AutoSize = true };
        rbDef.Click += (s, e) => rbDefClick(s);
        rbCustom.Click += (s, e) => rbCustomClick(s);
        Controls.Add(rbDef);
        Controls.Add(rbCustom);

        var lbType = new System.Windows.Forms.Label { Text = "类型:", Left = 10, Top = 38, AutoSize = true };
        cbbDefUserType = new System.Windows.Forms.ComboBox { Left = 60, Top = 34, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbDefUserType.Items.AddRange(new object[] { "玩家", "英雄" });
        cbbDefUserType.SelectedIndexChanged += (s, e) => cbbDefUserTypeChange(s);
        var lbJob = new System.Windows.Forms.Label { Text = "职业:", Left = 170, Top = 38, AutoSize = true };
        cbbDefJob = new System.Windows.Forms.ComboBox { Left = 220, Top = 34, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbDefJob.Items.AddRange(new object[] { "战士", "法师", "道士" });
        cbbDefJob.SelectedIndexChanged += (s, e) => cbbDefJobChange(s);
        Controls.Add(lbType);
        Controls.Add(cbbDefUserType);
        Controls.Add(lbJob);
        Controls.Add(cbbDefJob);

        var lbLevel = new System.Windows.Forms.Label { Text = "等级:", Left = 10, Top = 66, AutoSize = true };
        seDefLevel = new System.Windows.Forms.NumericUpDown { Left = 60, Top = 62, Width = 90, Minimum = 0, Maximum = 65535 };
        seDefLevel.ValueChanged += (s, e) => seDefLevelChange(s);
        Controls.Add(lbLevel);
        Controls.Add(seDefLevel);

        GridHumanInfo = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 96,
            Width = 450,
            Height = 320,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            ColumnHeadersVisible = false
        };
        GridHumanInfo.Columns.Add("k", "属性");
        GridHumanInfo.Columns.Add("v", "值");
        GridHumanInfo.Columns[0].Width = 150;
        GridHumanInfo.Columns[1].Width = 280;
        GridHumanInfo.Rows.Add(11);
        string[] keys = { "升级经验", "防御力", "魔御力", "攻击力", "魔法力", "道术力", "生命值", "魔法值", "背包重量", "穿戴重量", "腕力重量" };
        for (int i = 0; i < 11; i++)
            GridHumanInfo[0, i].Value = keys[i];
        Controls.Add(GridHumanInfo);

        btnSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 8, Top = 424, Width = 90, Height = 26, Enabled = false };
        btnSave.Click += (s, e) => btnSaveClick(s);
        Controls.Add(btnSave);
    }

    // ================= Delphi 1:1 =================

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool showModal = true)
    {
        if (M2ShareAbilConfig.g_BaseAbilConfig.UseDefault)
            rbDef.Checked = true;
        else
            rbCustom.Checked = true;

        PlayObject.m_wAbil.Level = 1;
        PlayObject.m_btJob = 0;
        PlayObject.m_sMapName = "0";
        PlayObject.m_nCurrX = 330;
        PlayObject.m_nCurrY = 266;

        HeroObject.m_wAbil.Level = 1;
        HeroObject.m_btJob = 0;
        HeroObject.m_sMapName = "0";
        HeroObject.m_nCurrX = 330;
        HeroObject.m_nCurrY = 266;

        seDefLevel.Value = 1;
        cbbDefUserType.SelectedIndex = 0;
        cbbDefJob.SelectedIndex = 0;
        RefView();
        btnSave.Enabled = false;
        if (showModal)
            ShowDialog();
    }

    /// <summary>RecalcHuman 等效链（RecalcAbilitys 装备叠加随装备系统批次接入；
    /// HasLevelUp 的 MaxExp 依赖 GetLevelExp（ExpCalc 表达式批次接入））。</summary>
    public void RecalcHuman()
    {
        PlayObject.RecalcLevelAbilitys(true);
        HasLevelUpEquivalent(PlayObject);
        HeroObject.RecalcLevelAbilitys(true);
        HasLevelUpEquivalent(HeroObject);
    }

    private static void HasLevelUpEquivalent(TPlayObject obj)
    {
        obj.m_wAbil.MaxExp = AbilRecalc.GetLevelExp(obj, obj.m_wAbil.Level); // HasLevelUp 首行
        obj.RecalcLevelAbilitys(false);
        // RecalcAbilitys（装备/物品属性叠加）随装备系统批次接入
    }

    private static TCreature CurrentViewObject(ViewLevelForm form)
        => form.cbbDefUserType.SelectedIndex == 1 ? form.HeroObject : form.PlayObject;

    public void RefView()
    {
        RecalcHuman();
        var obj = CurrentViewObject(this);
        GridHumanInfo[1, 0].Value = obj.m_wAbil.MaxExp.ToString();
        GridHumanInfo[1, 1].Value = obj.m_wAbil.AC1 + "/" + obj.m_wAbil.AC2;
        GridHumanInfo[1, 2].Value = obj.m_wAbil.MAC1 + "/" + obj.m_wAbil.MAC2;
        GridHumanInfo[1, 3].Value = obj.m_wAbil.DC1 + "/" + obj.m_wAbil.DC2;
        GridHumanInfo[1, 4].Value = obj.m_wAbil.MC1 + "/" + obj.m_wAbil.MC2;
        GridHumanInfo[1, 5].Value = obj.m_wAbil.SC1 + "/" + obj.m_wAbil.SC2;
        GridHumanInfo[1, 6].Value = obj.m_wAbil.HP + "/" + obj.m_wAbil.MaxHP;
        GridHumanInfo[1, 7].Value = obj.m_wAbil.MP + "/" + obj.m_wAbil.MaxMP;
        GridHumanInfo[1, 8].Value = obj.m_wAbil.Weight + "/" + obj.m_wAbil.MaxWeight;
        GridHumanInfo[1, 9].Value = obj.m_wAbil.WearWeight + "/" + obj.m_wAbil.MaxWearWeight;
        GridHumanInfo[1, 10].Value = obj.m_wAbil.HandWeight + "/" + obj.m_wAbil.MaxHandWeight;
    }

    public void cbbDefJobChange(object? sender)
    {
        PlayObject.m_btJob = (byte)cbbDefJob.SelectedIndex;
        HeroObject.m_btJob = (byte)cbbDefJob.SelectedIndex;
        RefView();
    }

    public void cbbDefUserTypeChange(object? sender)
    {
        RefView();
    }

    public void seDefLevelChange(object? sender)
    {
        if (seDefLevel.Value < 1)
            seDefLevel.Value = 1;
        PlayObject.m_wAbil.Level = (uint)seDefLevel.Value;
        HeroObject.m_wAbil.Level = (uint)seDefLevel.Value;
        RefView();
    }

    /// <summary>Delphi lstDefLevelsClick（等级列表行 → seDefLevel）。</summary>
    public void lstDefLevelsClick(int itemIndex)
    {
        seDefLevel.Value = itemIndex + 1;
    }

    public void rbDefClick(object? sender)
    {
        M2ShareAbilConfig.g_BaseAbilConfig.UseDefault = true;
        btnSave.Enabled = true;
        RefView();
    }

    public void rbCustomClick(object? sender)
    {
        M2ShareAbilConfig.g_BaseAbilConfig.UseDefault = false;
        btnSave.Enabled = true;
        RefView();
    }

    /// <summary>Delphi btnSaveClick 核心格式：[Setup] UseDefault + [Hum_n] AutoCalcLevel1000 +
    /// AC1_1..MaxHandWeight_1000 + Add 增量键（写入 BaseAbil.txt）。</summary>
    public void btnSaveClick(object? sender)
    {
        const string ValuesTrue = "1";
        const string ValuesFalse = "0";
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        var ini = new GXX.Core.Util.TStringList();
        ini.Add("[Setup]");
        ini.Add("UseDefault=" + (cfg.UseDefault ? ValuesTrue : ValuesFalse));

        for (int nJob = THumBaseAbil.JOB_WARR; nJob <= THumBaseAbil.JOB_TAOS; nJob++)
        {
            ini.Add("");
            ini.Add("[Hum_" + nJob + "]");
            ini.Add("AutoCalcLevel1000=" + (cfg.HumAbil[nJob].AutoCalcLevel1000 ? ValuesTrue : ValuesFalse));

            for (int nLevel = 0; nLevel < cfg.HumAbil[nJob].Base.Length; nLevel++)
            {
                var baseAbil = cfg.HumAbil[nJob].Base[nLevel];
                string sLevel = (nLevel + 1).ToString();
                ini.Add("AC1_" + sLevel + "=" + baseAbil.AC1);
                ini.Add("AC2_" + sLevel + "=" + baseAbil.AC2);
                ini.Add("MAC1_" + sLevel + "=" + baseAbil.MAC1);
                ini.Add("MAC2_" + sLevel + "=" + baseAbil.MAC2);
                ini.Add("DC1_" + sLevel + "=" + baseAbil.DC1);
                ini.Add("DC2_" + sLevel + "=" + baseAbil.DC2);
                ini.Add("MC1_" + sLevel + "=" + baseAbil.MC1);
                ini.Add("MC2_" + sLevel + "=" + baseAbil.MC2);
                ini.Add("SC1_" + sLevel + "=" + baseAbil.SC1);
                ini.Add("SC2_" + sLevel + "=" + baseAbil.SC2);
                ini.Add("MaxHP_" + sLevel + "=" + baseAbil.MaxHP);
                ini.Add("MaxMP_" + sLevel + "=" + baseAbil.MaxMP);
                ini.Add("MaxWeight_" + sLevel + "=" + baseAbil.MaxWeight);
                ini.Add("MaxWearWeight_" + sLevel + "=" + baseAbil.MaxWearWeight);
                ini.Add("MaxHandWeight_" + sLevel + "=" + baseAbil.MaxHandWeight);
            }

            ini.Add("");
            var addAbil = cfg.HumAbil[nJob].Add;
            ini.Add("AddAC1=" + addAbil.AC1);
            ini.Add("AddAC2=" + addAbil.AC2);
            ini.Add("AddMAC1=" + addAbil.MAC1);
            ini.Add("AddMAC2=" + addAbil.MAC2);
            ini.Add("AddDC1=" + addAbil.DC1);
            ini.Add("AddDC2=" + addAbil.DC2);
            ini.Add("AddMC1=" + addAbil.MC1);
            ini.Add("AddMC2=" + addAbil.MC2);
            ini.Add("AddSC1=" + addAbil.SC1);
            ini.Add("AddSC2=" + addAbil.SC2);
            ini.Add("AddMaxHP=" + addAbil.MaxHP);
            ini.Add("AddMaxMP=" + addAbil.MaxMP);
            ini.Add("AddMaxWeight=" + addAbil.MaxWeight);
            ini.Add("AddMaxWearWeight=" + addAbil.MaxWearWeight);
            ini.Add("AddMaxHandWeight=" + addAbil.MaxHandWeight);
        }

        ini.SaveToFile("BaseAbil.txt");
        btnSave.Enabled = false;
    }
}
