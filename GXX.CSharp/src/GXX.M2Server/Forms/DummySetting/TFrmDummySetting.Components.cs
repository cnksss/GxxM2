// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmDummySetting.dfm（34,189 字节）
//  本文件：DFM 控件树实例化（`Text`/`ClientSize`/页签/每个控件的
//          `Left/Top/Width/Height/MinValue/MaxValue/Value/Enabled/MultiSelect`）
//          + 事件绑定（绑定到的正是 `TFrmDummySetting.Handlers.cs` 里逐字对应的处理器）。
//
//  DFM 窗体属性（:1-17）：
//    BorderStyle = bsDialog / BorderWidth = 5 / Caption = '假人设置'
//    ClientHeight = 371 / ClientWidth = 451 / Position = poMainFormCenter
// ============================================================================

namespace GXX.M2Server.Forms.DummySetting;

public sealed partial class TFrmDummySetting
{
    /// <summary>
    /// DFM 控件树 1:1 实例化。控件坐标/尺寸/边界值全部取自 `uFrmDummySetting.dfm`。
    /// </summary>
    private void InitializeComponents()
    {
        // ---- DFM :1-17 窗体自身 ----
        Text = "假人设置";                                             // DFM Caption
        ClientSize = new System.Drawing.Size(451, 371);                 // DFM ClientWidth/ClientHeight
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;   // DFM BorderStyle = bsDialog
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;  // DFM Position = poMainFormCenter
        MaximizeBox = false;
        MinimizeBox = false;

        // ---- DFM :18-26 pgcMain（ActivePage = ts1 ⇒ 索引 0） ----
        Ct.pgcMain = new System.Windows.Forms.TabControl { Left = 0, Top = 0, Width = 451, Height = 371 };
        Ct.ts1 = new System.Windows.Forms.TabPage { Text = "假人名单", Left = 4, Top = 22, Width = 443, Height = 345 };
        Ct.ts2 = new System.Windows.Forms.TabPage { Text = "假人设置", Left = 4, Top = 22, Width = 443, Height = 345 };
        Ct.ts3 = new System.Windows.Forms.TabPage { Text = "跑动限制", Left = 4, Top = 22, Width = 443, Height = 345 };
        Ct.ts4 = new System.Windows.Forms.TabPage { Text = "禁止移动地图", Left = 4, Top = 22, Width = 443, Height = 345 };
        Ct.ts5 = new System.Windows.Forms.TabPage { Text = "不主动攻击怪物", Left = 4, Top = 22, Width = 443, Height = 345 };
        Ct.pgcMain.TabPages.AddRange(new System.Windows.Forms.TabPage[] { Ct.ts1, Ct.ts2, Ct.ts3, Ct.ts4, Ct.ts5 });
        Ct.pgcMain.SelectedIndex = 0;                                   // DFM ActivePage = ts1
        Controls.Add(Ct.pgcMain);

        // =====================================================================
        //  ts1（DFM :29-195）
        // =====================================================================

        Ct.GroupBox1 = new System.Windows.Forms.GroupBox { Left = 8, Top = 8, Width = 196, Height = 299 };
        Ct.lstDummyList = new System.Windows.Forms.ListBox
        {
            Left = 8, Top = 16, Width = 180, Height = 277,
            SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended,   // DFM MultiSelect = True
            IntegralHeight = false,
        };
        Ct.lstDummyList.Click += (_, _) => lstDummyListClick(Ct.lstDummyList);
        Ct.GroupBox1.Controls.Add(Ct.lstDummyList);
        Ct.ts1.Controls.Add(Ct.GroupBox1);

        Ct.GroupBox2 = new System.Windows.Forms.GroupBox { Left = 208, Top = 8, Width = 232, Height = 108 };
        Ct.Label1 = new System.Windows.Forms.Label { Left = 8, Top = 20, Width = 60, Height = 13, Text = "假人名单" };
        Ct.edtDummyName = new System.Windows.Forms.TextBox { Left = 8, Top = 40, Width = 145 };
        Ct.btnDummyAdd = new System.Windows.Forms.Button { Left = 163, Top = 14, Width = 57, Height = 25, Text = "增加(&A)" };
        Ct.btnDummyAdd.Click += (_, _) => btnDummyAddClick(Ct.btnDummyAdd);
        Ct.btnDummyDel = new System.Windows.Forms.Button { Left = 163, Top = 40, Width = 57, Height = 25, Text = "删除(&D)", Enabled = false };
        Ct.btnDummyDel.Click += (_, _) => btnDummyDelClick(Ct.btnDummyDel);
        Ct.GroupBox2.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label1, Ct.edtDummyName, Ct.btnDummyAdd, Ct.btnDummyDel,
        });
        Ct.ts1.Controls.Add(Ct.GroupBox2);

        Ct.GroupBox3 = new System.Windows.Forms.GroupBox { Left = 208, Top = 120, Width = 232, Height = 108 };
        Ct.Label2 = new System.Windows.Forms.Label { Left = 8, Top = 16, Width = 60, Height = 13, Text = "出生地图" };
        Ct.Label4 = new System.Windows.Forms.Label { Left = 8, Top = 64, Width = 60, Height = 13, Text = "出生Y" };
        Ct.Label3 = new System.Windows.Forms.Label { Left = 8, Top = 40, Width = 60, Height = 13, Text = "出生X" };
        Ct.Label31 = new System.Windows.Forms.Label { Left = 8, Top = 88, Width = 60, Height = 13, Text = "登录时间" };
        Ct.Label6 = new System.Windows.Forms.Label { Left = 145, Top = 88, Width = 60, Height = 13, Text = "随机时间" };
        Ct.seDummyHomeX = MakeSpin(71, 40, 70, 1, 2000, 10);            // DFM Min=1 Max=2000 Value=10
        Ct.seDummyHomeX.ValueChanged += (_, _) => seDummyHomeXChange(Ct.seDummyHomeX);
        Ct.seDummyHomeY = MakeSpin(71, 64, 70, 1, 2000, 10);            // DFM Min=1 Max=2000 Value=10
        Ct.seDummyHomeY.ValueChanged += (_, _) => seDummyHomeYChange(Ct.seDummyHomeY);
        Ct.edtDummyHomeMap = new System.Windows.Forms.TextBox { Left = 71, Top = 16, Width = 150, Text = "3" };
        // DFM :162 `OnChange = edtDummyHomeMapChange` ⇒ **确实绑定**（原文处理器 :408-411 是空体）
        Ct.edtDummyHomeMap.TextChanged += (_, _) => edtDummyHomeMapChange(Ct.edtDummyHomeMap);
        Ct.seDummyLogonTime = MakeSpin(71, 88, 70, 1, 2000, 10);        // DFM Min=1 Max=2000 Value=10
        Ct.seDummyLogonTime.ValueChanged += (_, _) => seDummyLogonTimeChange(Ct.seDummyLogonTime);
        Ct.chkDummyLogonRand = new System.Windows.Forms.CheckBox { Left = 145, Top = 86, Width = 80, Text = "随机" };
        Ct.chkDummyLogonRand.CheckedChanged += (_, _) => chkDummyLogonRandClick(Ct.chkDummyLogonRand);
        Ct.GroupBox3.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label2, Ct.Label4, Ct.Label3, Ct.Label31, Ct.Label6,
            Ct.seDummyHomeX, Ct.seDummyHomeY, Ct.edtDummyHomeMap, Ct.seDummyLogonTime, Ct.chkDummyLogonRand,
        });
        Ct.ts1.Controls.Add(Ct.GroupBox3);

        Ct.btnDummyLogon = new System.Windows.Forms.Button { Left = 357, Top = 229, Width = 81, Height = 25, Text = "登录(&L)", Enabled = false };
        Ct.btnDummyLogon.Click += (_, _) => btnDummyLogonClick(Ct.btnDummyLogon);
        Ct.ts1.Controls.Add(Ct.btnDummyLogon);

        // =====================================================================
        //  ts2（DFM :200-816）
        // =====================================================================

        Ct.GroupBox5 = new System.Windows.Forms.GroupBox { Left = 8, Top = 8, Width = 208, Height = 45 };
        Ct.CheckBoxDummyAutoRepairItem = new System.Windows.Forms.CheckBox { Left = 8, Top = 16, Width = 150, Text = "自动修理装备" };
        Ct.CheckBoxDummyAutoRepairItem.CheckedChanged += (_, _) => CheckBoxDummyAutoRepairItemClick(Ct.CheckBoxDummyAutoRepairItem);
        Ct.GroupBox5.Controls.Add(Ct.CheckBoxDummyAutoRepairItem);
        Ct.ts2.Controls.Add(Ct.GroupBox5);

        Ct.GroupBox6 = new System.Windows.Forms.GroupBox { Left = 8, Top = 56, Width = 208, Height = 92 };
        Ct.Label7 = new System.Windows.Forms.Label { Left = 8, Top = 20, Width = 30, Height = 13, Text = "战士" };
        Ct.Label8 = new System.Windows.Forms.Label { Left = 8, Top = 44, Width = 30, Height = 13, Text = "法师" };
        Ct.Label9 = new System.Windows.Forms.Label { Left = 8, Top = 68, Width = 30, Height = 13, Text = "道士" };
        Ct.EditDummyWarrorAttackTime = MakeSpin(42, 16, 75, 10, 10000, 10);
        Ct.EditDummyWarrorAttackTime.ValueChanged += (_, _) => EditDummyWarrorAttackTimeChange(Ct.EditDummyWarrorAttackTime);
        Ct.EditDummyTaoistAttackTime = MakeSpin(42, 64, 75, 10, 10000, 10);
        Ct.EditDummyTaoistAttackTime.ValueChanged += (_, _) => EditDummyTaoistAttackTimeChange(Ct.EditDummyTaoistAttackTime);
        Ct.EditDummyWizardAttackTime = MakeSpin(42, 40, 75, 10, 10000, 10);
        Ct.EditDummyWizardAttackTime.ValueChanged += (_, _) => EditDummyWizardAttackTimeChange(Ct.EditDummyWizardAttackTime);
        Ct.GroupBox6.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label7, Ct.Label8, Ct.Label9,
            Ct.EditDummyWarrorAttackTime, Ct.EditDummyTaoistAttackTime, Ct.EditDummyWizardAttackTime,
        });
        Ct.ts2.Controls.Add(Ct.GroupBox6);

        Ct.GroupBox7 = new System.Windows.Forms.GroupBox { Left = 8, Top = 152, Width = 208, Height = 92 };
        Ct.Label10 = new System.Windows.Forms.Label { Left = 8, Top = 20, Width = 30, Height = 13, Text = "战士" };
        Ct.Label11 = new System.Windows.Forms.Label { Left = 8, Top = 44, Width = 30, Height = 13, Text = "法师" };
        Ct.Label12 = new System.Windows.Forms.Label { Left = 8, Top = 68, Width = 30, Height = 13, Text = "道士" };
        Ct.EditDummyWarrorWalkTime = MakeSpin(42, 16, 75, 10, 10000, 10);
        Ct.EditDummyWarrorWalkTime.ValueChanged += (_, _) => EditDummyWarrorWalkTimeChange(Ct.EditDummyWarrorWalkTime);
        Ct.EditDummyWizardWalkTime = MakeSpin(42, 40, 75, 10, 10000, 10);
        Ct.EditDummyWizardWalkTime.ValueChanged += (_, _) => EditDummyWizardWalkTimeChange(Ct.EditDummyWizardWalkTime);
        Ct.EditDummyTaoistWalkTime = MakeSpin(42, 64, 75, 10, 10000, 10);
        Ct.EditDummyTaoistWalkTime.ValueChanged += (_, _) => EditDummyTaoistWalkTimeChange(Ct.EditDummyTaoistWalkTime);
        Ct.GroupBox7.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label10, Ct.Label11, Ct.Label12,
            Ct.EditDummyWarrorWalkTime, Ct.EditDummyWizardWalkTime, Ct.EditDummyTaoistWalkTime,
        });
        Ct.ts2.Controls.Add(Ct.GroupBox7);

        Ct.GroupBox8 = new System.Windows.Forms.GroupBox { Left = 8, Top = 248, Width = 208, Height = 45 };
        Ct.CheckBoxDummyAutoRecallHero = new System.Windows.Forms.CheckBox { Left = 8, Top = 16, Width = 150, Text = "自动召唤英雄" };
        Ct.CheckBoxDummyAutoRecallHero.CheckedChanged += (_, _) => CheckBoxDummyAutoRecallHeroClick(Ct.CheckBoxDummyAutoRecallHero);
        Ct.GroupBox8.Controls.Add(Ct.CheckBoxDummyAutoRecallHero);
        Ct.ts2.Controls.Add(Ct.GroupBox8);

        Ct.GroupBox9 = new System.Windows.Forms.GroupBox { Left = 222, Top = 8, Width = 216, Height = 178 };
        Ct.Label13 = new System.Windows.Forms.Label { Left = 8, Top = 18, Width = 30, Height = 13, Text = "速度" };
        Ct.Label14 = new System.Windows.Forms.Label { Left = 8, Top = 42, Width = 30, Height = 13, Text = "基数" };
        Ct.Label15 = new System.Windows.Forms.Label { Left = 8, Top = 66, Width = 30, Height = 13, Text = "速度" };
        Ct.Bevel1 = MakeBevel(78, 16, 1, 86);
        Ct.Label16 = new System.Windows.Forms.Label { Left = 84, Top = 18, Width = 40, Height = 13, Text = "战士HP" };
        Ct.Label17 = new System.Windows.Forms.Label { Left = 148, Top = 18, Width = 40, Height = 13, Text = "战士MP" };
        Ct.Label18 = new System.Windows.Forms.Label { Left = 84, Top = 66, Width = 40, Height = 13, Text = "道法HP" };
        Ct.Bevel5 = MakeBevel(140, 16, 1, 86);
        Ct.Label19 = new System.Windows.Forms.Label { Left = 148, Top = 66, Width = 40, Height = 13, Text = "道法MP" };
        Ct.Label20 = new System.Windows.Forms.Label { Left = 8, Top = 90, Width = 30, Height = 13, Text = "基数" };
        Ct.Bevel3 = MakeBevel(78, 88, 1, 86);
        Ct.seDummyHPTime_Warrior = MakeSpin(91, 16, 53, 0, 0, 5);       // DFM Min=0 Max=0（不钳制）Value=5
        Ct.seDummyHPTime_Warrior.ValueChanged += (_, _) => seDummyHPTime_WarriorChange(Ct.seDummyHPTime_Warrior);
        Ct.seDummyMPTime_Warrior = MakeSpin(155, 16, 53, 0, 0, 5);
        Ct.seDummyMPTime_Warrior.ValueChanged += (_, _) => seDummyMPTime_WarriorChange(Ct.seDummyMPTime_Warrior);
        Ct.seDummyHPTime_DF = MakeSpin(91, 66, 53, 0, 0, 5);
        Ct.seDummyHPTime_DF.ValueChanged += (_, _) => seDummyHPTime_DFChange(Ct.seDummyHPTime_DF);
        Ct.seDummyHPBase_DF = MakeSpin(91, 90, 53, 0, 0, 0);
        Ct.seDummyHPBase_DF.ValueChanged += (_, _) => seDummyHPBase_DFChange(Ct.seDummyHPBase_DF);
        Ct.seDummyHPBase_Warrior = MakeSpin(91, 42, 53, 0, 0, 0);
        Ct.seDummyHPBase_Warrior.ValueChanged += (_, _) => seDummyHPBase_WarriorChange(Ct.seDummyHPBase_Warrior);
        Ct.seDummyMPBase_Warrior = MakeSpin(155, 42, 53, 0, 0, 0);
        Ct.seDummyMPBase_Warrior.ValueChanged += (_, _) => seDummyMPBase_WarriorChange(Ct.seDummyMPBase_Warrior);
        Ct.seDummyMPTime_DF = MakeSpin(155, 66, 53, 0, 0, 5);
        Ct.seDummyMPTime_DF.ValueChanged += (_, _) => seDummyMPTime_DFChange(Ct.seDummyMPTime_DF);
        Ct.seDummyMPBase_DF = MakeSpin(155, 90, 53, 0, 0, 0);
        Ct.seDummyMPBase_DF.ValueChanged += (_, _) => seDummyMPBase_DFChange(Ct.seDummyMPBase_DF);
        Ct.GroupBox9.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label13, Ct.Label14, Ct.Label15, Ct.Bevel1, Ct.Label16, Ct.Label17, Ct.Label18,
            Ct.Bevel5, Ct.Label19, Ct.Label20, Ct.Bevel3,
            Ct.seDummyHPTime_Warrior, Ct.seDummyMPTime_Warrior, Ct.seDummyHPTime_DF, Ct.seDummyHPBase_DF,
            Ct.seDummyHPBase_Warrior, Ct.seDummyMPBase_Warrior, Ct.seDummyMPTime_DF, Ct.seDummyMPBase_DF,
        });
        Ct.ts2.Controls.Add(Ct.GroupBox9);

        Ct.GroupBox10 = new System.Windows.Forms.GroupBox { Left = 222, Top = 190, Width = 216, Height = 178 };
        Ct.Label21 = new System.Windows.Forms.Label { Left = 8, Top = 18, Width = 30, Height = 13, Text = "速度" };
        Ct.Label22 = new System.Windows.Forms.Label { Left = 8, Top = 42, Width = 30, Height = 13, Text = "基数" };
        Ct.Label23 = new System.Windows.Forms.Label { Left = 8, Top = 66, Width = 30, Height = 13, Text = "速度" };
        Ct.Label24 = new System.Windows.Forms.Label { Left = 8, Top = 90, Width = 30, Height = 13, Text = "基数" };
        Ct.Bevel2 = MakeBevel(78, 16, 1, 86);
        Ct.Label25 = new System.Windows.Forms.Label { Left = 84, Top = 18, Width = 55, Height = 13, Text = "英雄战士HP" };
        Ct.Label26 = new System.Windows.Forms.Label { Left = 148, Top = 18, Width = 55, Height = 13, Text = "英雄战士MP" };
        Ct.Bevel4 = MakeBevel(140, 16, 1, 86);
        Ct.Label27 = new System.Windows.Forms.Label { Left = 84, Top = 66, Width = 55, Height = 13, Text = "英雄道法HP" };
        Ct.Bevel6 = MakeBevel(78, 88, 1, 86);
        Ct.Label28 = new System.Windows.Forms.Label { Left = 148, Top = 66, Width = 55, Height = 13, Text = "英雄道法MP" };
        Ct.seDummyHeroHPTime_Warrior = MakeSpin(91, 16, 53, 0, 0, 5);
        Ct.seDummyHeroHPTime_Warrior.ValueChanged += (_, _) => seDummyHeroHPTime_WarriorChange(Ct.seDummyHeroHPTime_Warrior);
        Ct.seDummyHeroMPTime_Warrior = MakeSpin(155, 16, 53, 0, 0, 5);
        Ct.seDummyHeroMPTime_Warrior.ValueChanged += (_, _) => seDummyHeroMPTime_WarriorChange(Ct.seDummyHeroMPTime_Warrior);
        Ct.seDummyHeroHPTime_DF = MakeSpin(91, 66, 53, 0, 0, 5);
        Ct.seDummyHeroHPTime_DF.ValueChanged += (_, _) => seDummyHeroHPTime_DFChange(Ct.seDummyHeroHPTime_DF);
        Ct.seDummyHeroMPTime_DF = MakeSpin(155, 66, 53, 0, 0, 5);
        Ct.seDummyHeroMPTime_DF.ValueChanged += (_, _) => seDummyHeroMPTime_DFChange(Ct.seDummyHeroMPTime_DF);
        Ct.seDummyHeroHPBase_DF = MakeSpin(91, 90, 53, 0, 0, 0);
        Ct.seDummyHeroHPBase_DF.ValueChanged += (_, _) => seDummyHeroHPBase_DFChange(Ct.seDummyHeroHPBase_DF);
        Ct.seDummyHeroMPBase_DF = MakeSpin(155, 90, 53, 0, 0, 0);
        Ct.seDummyHeroMPBase_DF.ValueChanged += (_, _) => seDummyHeroMPBase_DFChange(Ct.seDummyHeroMPBase_DF);
        Ct.seDummyHeroHPBase_Warrior = MakeSpin(91, 42, 53, 0, 0, 0);
        Ct.seDummyHeroHPBase_Warrior.ValueChanged += (_, _) => seDummyHeroHPBase_WarriorChange(Ct.seDummyHeroHPBase_Warrior);
        Ct.seDummyHeroMPBase_Warrior = MakeSpin(155, 42, 53, 0, 0, 0);
        Ct.seDummyHeroMPBase_Warrior.ValueChanged += (_, _) => seDummyHeroMPBase_WarriorChange(Ct.seDummyHeroMPBase_Warrior);
        Ct.GroupBox10.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label21, Ct.Label22, Ct.Label23, Ct.Label24, Ct.Bevel2, Ct.Label25, Ct.Label26,
            Ct.Bevel4, Ct.Label27, Ct.Bevel6, Ct.Label28,
            Ct.seDummyHeroHPTime_Warrior, Ct.seDummyHeroMPTime_Warrior, Ct.seDummyHeroHPTime_DF,
            Ct.seDummyHeroMPTime_DF, Ct.seDummyHeroHPBase_DF, Ct.seDummyHeroMPBase_DF,
            Ct.seDummyHeroHPBase_Warrior, Ct.seDummyHeroMPBase_Warrior,
        });
        Ct.ts2.Controls.Add(Ct.GroupBox10);

        Ct.grp14 = new System.Windows.Forms.GroupBox { Left = 8, Top = 300, Width = 430, Height = 45 };
        Ct.Label29 = new System.Windows.Forms.Label { Left = 8, Top = 18, Width = 70, Height = 13, Text = "自动加血蓝" };
        Ct.Label30 = new System.Windows.Forms.Label { Left = 160, Top = 18, Width = 70, Height = 13, Text = "自动加蓝" };
        Ct.chkDummyAutoAddHP = new System.Windows.Forms.CheckBox { Left = 8, Top = 14, Width = 70, Text = "自动加血" };
        Ct.chkDummyAutoAddHP.CheckedChanged += (_, _) => chkDummyAutoAddHPClick(Ct.chkDummyAutoAddHP);
        Ct.seDummyAddHPPercent = MakeSpin(82, 16, 42, 1, 100, 60);      // DFM Min=1 Max=100 Value=60
        Ct.seDummyAddHPPercent.ValueChanged += (_, _) => seDummyAddHPPercentChange(Ct.seDummyAddHPPercent);
        Ct.chkDummyAutoAddMP = new System.Windows.Forms.CheckBox { Left = 160, Top = 14, Width = 70, Text = "自动加蓝" };
        Ct.chkDummyAutoAddMP.CheckedChanged += (_, _) => chkDummyAutoAddMPClick(Ct.chkDummyAutoAddMP);
        Ct.seDummyAddMPPercent = MakeSpin(231, 16, 42, 1, 100, 60);      // DFM Min=1 Max=100 Value=60
        Ct.seDummyAddMPPercent.ValueChanged += (_, _) => seDummyAddMPPercentChange(Ct.seDummyAddMPPercent);
        Ct.grp14.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.Label29, Ct.Label30, Ct.chkDummyAutoAddHP, Ct.seDummyAddHPPercent,
            Ct.chkDummyAutoAddMP, Ct.seDummyAddMPPercent,
        });
        Ct.ts2.Controls.Add(Ct.grp14);

        // =====================================================================
        //  ts3（DFM :822-935）
        // =====================================================================

        Ct.GroupBox11 = new System.Windows.Forms.GroupBox { Left = 8, Top = 8, Width = 430, Height = 200 };
        Ct.chkDisDummyRun = new System.Windows.Forms.CheckBox { Left = 8, Top = 16, Width = 120, Text = "禁止假人跑动" };
        Ct.chkDisDummyRun.CheckedChanged += (_, _) => chkDisDummyRunClick(Ct.chkDisDummyRun);
        Ct.chkDummyRunHum = new System.Windows.Forms.CheckBox { Left = 8, Top = 44, Width = 120, Text = "可穿人物" };
        Ct.chkDummyRunHum.CheckedChanged += (_, _) => chkDummyRunHumClick(Ct.chkDummyRunHum);
        Ct.chkDummyRunMon = new System.Windows.Forms.CheckBox { Left = 8, Top = 72, Width = 120, Text = "可穿怪物" };
        Ct.chkDummyRunMon.CheckedChanged += (_, _) => chkDummyRunMonClick(Ct.chkDummyRunMon);
        Ct.chkDummyRunNpc = new System.Windows.Forms.CheckBox { Left = 8, Top = 100, Width = 120, Text = "可穿NPC" };
        Ct.chkDummyRunNpc.CheckedChanged += (_, _) => chkDummyRunNpcClick(Ct.chkDummyRunNpc);
        Ct.chkDummyRunGuard = new System.Windows.Forms.CheckBox { Left = 8, Top = 128, Width = 120, Text = "可穿守卫" };
        Ct.chkDummyRunGuard.CheckedChanged += (_, _) => chkDummyRunGuardClick(Ct.chkDummyRunGuard);
        Ct.chkDummySafeArea = new System.Windows.Forms.CheckBox { Left = 8, Top = 156, Width = 150, Text = "安全区限制跑动" };
        Ct.chkDummySafeArea.CheckedChanged += (_, _) => chkDummySafeAreaClick(Ct.chkDummySafeArea);
        Ct.chkDummySafeAreaDisNpcRun = new System.Windows.Forms.CheckBox { Left = 160, Top = 44, Width = 190, Text = "安全区禁止穿NPC" };
        Ct.chkDummySafeAreaDisNpcRun.CheckedChanged += (_, _) => chkDummySafeAreaDisNpcRunClick(Ct.chkDummySafeAreaDisNpcRun);
        Ct.chkSafeAreaDisShopStallDummyRun = new System.Windows.Forms.CheckBox { Left = 160, Top = 72, Width = 190, Text = "安全区禁止穿摆摊" };
        Ct.chkSafeAreaDisShopStallDummyRun.CheckedChanged += (_, _) => chkSafeAreaDisShopStallDummyRunClick(Ct.chkSafeAreaDisShopStallDummyRun);
        Ct.chkSafeAreaDisOffLineDummyRun = new System.Windows.Forms.CheckBox { Left = 160, Top = 100, Width = 190, Text = "安全区禁止穿离线" };
        Ct.chkSafeAreaDisOffLineDummyRun.CheckedChanged += (_, _) => chkSafeAreaDisOffLineDummyRunClick(Ct.chkSafeAreaDisOffLineDummyRun);
        Ct.chkDummyWarDisHumRun = new System.Windows.Forms.CheckBox { Left = 160, Top = 128, Width = 190, Text = "攻城区允许穿英雄" };
        Ct.chkDummyWarDisHumRun.CheckedChanged += (_, _) => chkDummyWarDisHumRunClick(Ct.chkDummyWarDisHumRun);
        Ct.chkDummyWarHreoRun = new System.Windows.Forms.CheckBox { Left = 160, Top = 156, Width = 190, Text = "攻城区允许穿英雄(子)" };
        Ct.chkDummyWarHreoRun.CheckedChanged += (_, _) => chkDummyWarHreoRunClick(Ct.chkDummyWarHreoRun);
        Ct.GroupBox11.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.chkDisDummyRun, Ct.chkDummyRunHum, Ct.chkDummyRunMon, Ct.chkDummyRunNpc, Ct.chkDummyRunGuard,
            Ct.chkDummySafeArea, Ct.chkDummySafeAreaDisNpcRun, Ct.chkSafeAreaDisShopStallDummyRun,
            Ct.chkSafeAreaDisOffLineDummyRun, Ct.chkDummyWarDisHumRun, Ct.chkDummyWarHreoRun,
        });
        Ct.ts3.Controls.Add(Ct.GroupBox11);

        // =====================================================================
        //  ts4（DFM :941-1023）
        // =====================================================================

        Ct.GroupBox12 = new System.Windows.Forms.GroupBox { Left = 8, Top = 8, Width = 260, Height = 299 };
        Ct.lstDisableMoveMap = new System.Windows.Forms.ListBox
        {
            Left = 8, Top = 16, Width = 150, Height = 277, IntegralHeight = false,
        };
        Ct.lstDisableMoveMap.Click += (_, _) => lstDisableMoveMapClick(Ct.lstDisableMoveMap);
        Ct.btnDisableMoveMapAdd = new System.Windows.Forms.Button { Left = 184, Top = 14, Width = 73, Height = 25, Text = "增加(&A)" };
        Ct.btnDisableMoveMapAdd.Click += (_, _) => btnDisableMoveMapAddClick(Ct.btnDisableMoveMapAdd);
        Ct.btnDisableMoveMapDelete = new System.Windows.Forms.Button { Left = 184, Top = 40, Width = 73, Height = 25, Text = "删除(&D)", Enabled = false };
        Ct.btnDisableMoveMapDelete.Click += (_, _) => btnDisableMoveMapDeleteClick(Ct.btnDisableMoveMapDelete);
        Ct.btnDisableMoveMapAddAll = new System.Windows.Forms.Button { Left = 184, Top = 66, Width = 73, Height = 25, Text = "全部增加" };
        Ct.btnDisableMoveMapAddAll.Click += (_, _) => btnDisableMoveMapAddAllClick(Ct.btnDisableMoveMapAddAll);
        Ct.btnDisableMoveMapDeleteAll = new System.Windows.Forms.Button { Left = 184, Top = 92, Width = 73, Height = 25, Text = "全部删除" };
        Ct.btnDisableMoveMapDeleteAll.Click += (_, _) => btnDisableMoveMapDeleteAllClick(Ct.btnDisableMoveMapDeleteAll);
        Ct.btnDisableMoveMapSave = new System.Windows.Forms.Button { Left = 184, Top = 118, Width = 73, Height = 25, Text = "保存(&S)" };
        Ct.btnDisableMoveMapSave.Click += (_, _) => btnDisableMoveMapSaveClick(Ct.btnDisableMoveMapSave);
        Ct.GroupBox12.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.lstDisableMoveMap, Ct.btnDisableMoveMapAdd, Ct.btnDisableMoveMapDelete,
            Ct.btnDisableMoveMapAddAll, Ct.btnDisableMoveMapDeleteAll, Ct.btnDisableMoveMapSave,
        });
        Ct.ts4.Controls.Add(Ct.GroupBox12);

        Ct.GroupBox13 = new System.Windows.Forms.GroupBox { Left = 276, Top = 8, Width = 162, Height = 299 };
        Ct.lstMapList = new System.Windows.Forms.ListBox
        {
            Left = 8, Top = 16, Width = 150, Height = 275,
            SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended,   // DFM MultiSelect = True
            IntegralHeight = false,
        };
        Ct.GroupBox13.Controls.Add(Ct.lstMapList);
        Ct.ts4.Controls.Add(Ct.GroupBox13);

        // =====================================================================
        //  ts5（DFM :1028-1111）
        // =====================================================================

        Ct.GroupBox14 = new System.Windows.Forms.GroupBox { Left = 8, Top = 8, Width = 260, Height = 299 };
        Ct.lstNoAttackMonList = new System.Windows.Forms.ListBox
        {
            Left = 8, Top = 16, Width = 150, Height = 278, IntegralHeight = false,
        };
        Ct.lstNoAttackMonList.Click += (_, _) => lstNoAttackMonListClick(Ct.lstNoAttackMonList);
        Ct.btnNoAttackMonDel = new System.Windows.Forms.Button { Left = 184, Top = 40, Width = 73, Height = 25, Text = "删除(&D)", Enabled = false };
        Ct.btnNoAttackMonDel.Click += (_, _) => btnNoAttackMonDelClick(Ct.btnNoAttackMonDel);
        Ct.btnNoAttackMonAddAll = new System.Windows.Forms.Button { Left = 184, Top = 66, Width = 73, Height = 25, Text = "全部增加" };
        Ct.btnNoAttackMonAddAll.Click += (_, _) => btnNoAttackMonAddAllClick(Ct.btnNoAttackMonAddAll);
        Ct.btnNoAttackMonDelAll = new System.Windows.Forms.Button { Left = 184, Top = 92, Width = 73, Height = 25, Text = "全部删除" };
        Ct.btnNoAttackMonDelAll.Click += (_, _) => btnNoAttackMonDelAllClick(Ct.btnNoAttackMonDelAll);
        Ct.btnNoAttackMonSave = new System.Windows.Forms.Button { Left = 184, Top = 118, Width = 73, Height = 25, Text = "保存(&S)" };
        Ct.btnNoAttackMonSave.Click += (_, _) => btnNoAttackMonSaveClick(Ct.btnNoAttackMonSave);
        Ct.GroupBox14.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Ct.lstNoAttackMonList, Ct.btnNoAttackMonDel, Ct.btnNoAttackMonAddAll,
            Ct.btnNoAttackMonDelAll, Ct.btnNoAttackMonSave,
        });
        Ct.ts5.Controls.Add(Ct.GroupBox14);

        Ct.GroupBox15 = new System.Windows.Forms.GroupBox { Left = 276, Top = 8, Width = 162, Height = 299 };
        Ct.lstMonList = new System.Windows.Forms.ListBox
        {
            Left = 8, Top = 16, Width = 150, Height = 278,
            SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended,   // DFM MultiSelect = True
            IntegralHeight = false,
        };
        Ct.GroupBox15.Controls.Add(Ct.lstMonList);
        Ct.ts5.Controls.Add(Ct.GroupBox15);

        Ct.btnNoAttackMonAdd = new System.Windows.Forms.Button { Left = 268, Top = 312, Width = 73, Height = 25, Text = "增加(&A)" };
        Ct.btnNoAttackMonAdd.Click += (_, _) => btnNoAttackMonAddClick(Ct.btnNoAttackMonAdd);
        Ct.ts5.Controls.Add(Ct.btnNoAttackMonAdd);

        // ---- DFM :1113-1131 窗体底部按钮（**直接在窗体上**，不在 TabSheet 内） ----
        Ct.ButtonDummySave = new System.Windows.Forms.Button { Left = 376, Top = 343, Width = 75, Height = 25, Text = "保存(&S)" };
        Ct.ButtonDummySave.Click += (_, _) => ButtonDummySaveClick(Ct.ButtonDummySave);
        Controls.Add(Ct.ButtonDummySave);

        Ct.btn1 = new System.Windows.Forms.Button { Left = 290, Top = 343, Width = 75, Height = 25, Text = "默认" };
        Ct.btn1.Click += (_, _) => btn1Click(Ct.btn1);
        Controls.Add(Ct.btn1);
    }

    /// <summary>
    /// DFM `TSpinEditEx` → WinForms `NumericUpDown` 的公共构造。
    /// <paramref name="maxValue"/> 为 0 时按"不钳制"处理（DFM 中 `MaxValue = 0` 的语义）。
    /// </summary>
    private static System.Windows.Forms.NumericUpDown MakeSpin(int left, int top, int width,
                                                               int minValue, int maxValue, int value)
    {
        var spin = new System.Windows.Forms.NumericUpDown
        {
            Left = left,
            Top = top,
            Width = width,
            Minimum = minValue,
            Maximum = maxValue == 0 ? int.MaxValue : maxValue,
            Value = value,
        };
        return spin;
    }

    /// <summary>DFM `TBevel`（装饰线）→ 1px 高的 WinForms `Label`（无行为，仅占位）。</summary>
    private static System.Windows.Forms.Label MakeBevel(int left, int top, int width, int height)
        => new() { Left = left, Top = top, Width = width, Height = height, BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D };
}
