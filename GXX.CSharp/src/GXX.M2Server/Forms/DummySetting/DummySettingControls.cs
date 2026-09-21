// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmDummySetting.dfm（34,189 字节）
//  本文件：DFM 控件树 1:1（79 个控件 + 窗体自身属性）+ 事件绑定。
//
//  ★ 控件类型选择（与既有车道一致，非本车道发明）：
//    · `TSpinEditEx`（SpinEditEx.pas 未移植）→ WinForms `NumericUpDown`，并用
//      **DFM 的 `MinValue`/`MaxValue`/`Value` 逐条设定**（值见本文件每条注释；
//      DFM 中 `MaxValue = 0` 表示不钳制 ⇒ 映射为 `int.MaxValue`）。
//    · `TListBox` → WinForms `ListBox`（`SelectionMode` 按 DFM `MultiSelect` 设定）。
//    · `TPageControl`/`TTabSheet` → WinForms `TabControl`/`TabPage`。
//    · `TLabel`/`TBevel`/`TGroupBox` → 同名 WinForms 控件。
//
//  ★ 与 `TFrmDummySetting` 分工：本类只承载**控件身份**（让 `TFrmDummySetting` 的
//    事件处理器可以逐字写 `Ct.lstDummyList.ItemIndex` 而**无需 STA 窗体句柄**，
//    从而可在普通 xUnit 用例里直调处理器 —— 这正是"事件处理器直调 + 决策镜像"
//    规程所要求的形态）。窗体本身仍是 `System.Windows.Forms.Form`（生产可显示）。
// ============================================================================

using System;
using System.Collections.Generic;

namespace GXX.M2Server.Forms.DummySetting;

/// <summary>
/// `uFrmDummySetting.dfm` 控件树 1:1（字段名、层级、顺序与 DFM 一致）。
/// </summary>
public sealed class DummySettingControls
{
    // ---- DFM :18-27 页签容器 ----
    /// <summary>DFM :18 `pgcMain: TPageControl`（`ActivePage = ts1`）。</summary>
    public System.Windows.Forms.TabControl pgcMain = null!;
    /// <summary>DFM :27 `ts1: TTabSheet`。</summary>
    public System.Windows.Forms.TabPage ts1 = null!;
    /// <summary>DFM :197 `ts2: TTabSheet`。</summary>
    public System.Windows.Forms.TabPage ts2 = null!;
    /// <summary>DFM :819 `ts3: TTabSheet`。</summary>
    public System.Windows.Forms.TabPage ts3 = null!;
    /// <summary>DFM :938 `ts4: TTabSheet`。</summary>
    public System.Windows.Forms.TabPage ts4 = null!;
    /// <summary>DFM :1025 `ts5: TTabSheet`。</summary>
    public System.Windows.Forms.TabPage ts5 = null!;

    // ---- DFM :29-35 / :49-89 / :90-195 ts1 ----
    /// <summary>DFM :29 `GroupBox1: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    /// <summary>DFM :36 `lstDummyList: TListBox`（`MultiSelect = True`）。</summary>
    public System.Windows.Forms.ListBox lstDummyList = null!;
    /// <summary>DFM :49 `GroupBox2: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox2 = null!;
    /// <summary>DFM :55 `Label1: TLabel`。</summary>
    public System.Windows.Forms.Label Label1 = null!;
    /// <summary>DFM :62 `edtDummyName: TEdit`。</summary>
    public System.Windows.Forms.TextBox edtDummyName = null!;
    /// <summary>DFM :70 `btnDummyAdd: TButton`。</summary>
    public System.Windows.Forms.Button btnDummyAdd = null!;
    /// <summary>DFM :79 `btnDummyDel: TButton`（DFM `Enabled = False`）。</summary>
    public System.Windows.Forms.Button btnDummyDel = null!;
    /// <summary>DFM :90 `GroupBox3: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox3 = null!;
    /// <summary>DFM :97 `Label2: TLabel`。</summary>
    public System.Windows.Forms.Label Label2 = null!;
    /// <summary>DFM :104 `Label4: TLabel`。</summary>
    public System.Windows.Forms.Label Label4 = null!;
    /// <summary>DFM :111 `Label3: TLabel`。</summary>
    public System.Windows.Forms.Label Label3 = null!;
    /// <summary>DFM :118 `Label31: TLabel`。</summary>
    public System.Windows.Forms.Label Label31 = null!;
    /// <summary>DFM :125 `Label6: TLabel`。</summary>
    public System.Windows.Forms.Label Label6 = null!;
    /// <summary>DFM :132 `seDummyHomeX: TSpinEditEx`（Min=1 Max=2000 Value=10）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHomeX = null!;
    /// <summary>DFM :143 `seDummyHomeY: TSpinEditEx`（Min=1 Max=2000 Value=10）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHomeY = null!;
    /// <summary>DFM :154 `edtDummyHomeMap: TEdit`（:162 `OnChange = edtDummyHomeMapChange`；Text 初值 `'3'`）。</summary>
    public System.Windows.Forms.TextBox edtDummyHomeMap = null!;
    /// <summary>DFM :164 `seDummyLogonTime: TSpinEditEx`（Min=1 Max=2000 Value=10）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyLogonTime = null!;
    /// <summary>DFM :175 `chkDummyLogonRand: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyLogonRand = null!;
    /// <summary>DFM :186 `btnDummyLogon: TButton`（DFM `Enabled = False`）。</summary>
    public System.Windows.Forms.Button btnDummyLogon = null!;

    // ---- DFM :200-816 ts2 ----
    /// <summary>DFM :200 `GroupBox5: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox5 = null!;
    /// <summary>DFM :207 `CheckBoxDummyAutoRepairItem: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox CheckBoxDummyAutoRepairItem = null!;
    /// <summary>DFM :217 `GroupBox6: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox6 = null!;
    /// <summary>DFM :224 `Label7: TLabel`。</summary>
    public System.Windows.Forms.Label Label7 = null!;
    /// <summary>DFM :231 `Label8: TLabel`。</summary>
    public System.Windows.Forms.Label Label8 = null!;
    /// <summary>DFM :238 `Label9: TLabel`。</summary>
    public System.Windows.Forms.Label Label9 = null!;
    /// <summary>DFM :245 `EditDummyWarrorAttackTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyWarrorAttackTime = null!;
    /// <summary>DFM :256 `EditDummyTaoistAttackTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyTaoistAttackTime = null!;
    /// <summary>DFM :267 `EditDummyWizardAttackTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyWizardAttackTime = null!;
    /// <summary>DFM :279 `GroupBox7: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox7 = null!;
    /// <summary>DFM :286 `Label10: TLabel`。</summary>
    public System.Windows.Forms.Label Label10 = null!;
    /// <summary>DFM :293 `Label11: TLabel`。</summary>
    public System.Windows.Forms.Label Label11 = null!;
    /// <summary>DFM :300 `Label12: TLabel`。</summary>
    public System.Windows.Forms.Label Label12 = null!;
    /// <summary>DFM :307 `EditDummyWarrorWalkTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyWarrorWalkTime = null!;
    /// <summary>DFM :318 `EditDummyWizardWalkTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyWizardWalkTime = null!;
    /// <summary>DFM :329 `EditDummyTaoistWalkTime: TSpinEditEx`（Min=10 Max=10000）。</summary>
    public System.Windows.Forms.NumericUpDown EditDummyTaoistWalkTime = null!;
    /// <summary>DFM :341 `GroupBox8: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox8 = null!;
    /// <summary>DFM :348 `CheckBoxDummyAutoRecallHero: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox CheckBoxDummyAutoRecallHero = null!;
    /// <summary>DFM :358 `GroupBox9: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox9 = null!;
    /// <summary>DFM :365 `Label13: TLabel`。</summary>
    public System.Windows.Forms.Label Label13 = null!;
    /// <summary>DFM :372 `Label14: TLabel`。</summary>
    public System.Windows.Forms.Label Label14 = null!;
    /// <summary>DFM :379 `Label15: TLabel`。</summary>
    public System.Windows.Forms.Label Label15 = null!;
    /// <summary>DFM :386 `Bevel1: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel1 = null!;
    /// <summary>DFM :393 `Label16: TLabel`。</summary>
    public System.Windows.Forms.Label Label16 = null!;
    /// <summary>DFM :400 `Label17: TLabel`。</summary>
    public System.Windows.Forms.Label Label17 = null!;
    /// <summary>DFM :407 `Label18: TLabel`。</summary>
    public System.Windows.Forms.Label Label18 = null!;
    /// <summary>DFM :414 `Bevel5: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel5 = null!;
    /// <summary>DFM :421 `Label19: TLabel`。</summary>
    public System.Windows.Forms.Label Label19 = null!;
    /// <summary>DFM :428 `Label20: TLabel`。</summary>
    public System.Windows.Forms.Label Label20 = null!;
    /// <summary>DFM :435 `Bevel3: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel3 = null!;
    /// <summary>DFM :442 `seDummyHPTime_Warrior: TSpinEditEx`（Min=0 Max=0 ⇒ 不钳制）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHPTime_Warrior = null!;
    /// <summary>DFM :456 `seDummyMPTime_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyMPTime_Warrior = null!;
    /// <summary>DFM :470 `seDummyHPTime_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHPTime_DF = null!;
    /// <summary>DFM :484 `seDummyHPBase_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHPBase_DF = null!;
    /// <summary>DFM :498 `seDummyHPBase_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHPBase_Warrior = null!;
    /// <summary>DFM :512 `seDummyMPBase_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyMPBase_Warrior = null!;
    /// <summary>DFM :526 `seDummyMPTime_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyMPTime_DF = null!;
    /// <summary>DFM :540 `seDummyMPBase_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyMPBase_DF = null!;
    /// <summary>DFM :555 `GroupBox10: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox10 = null!;
    /// <summary>DFM :562 `Label21: TLabel`。</summary>
    public System.Windows.Forms.Label Label21 = null!;
    /// <summary>DFM :569 `Label22: TLabel`。</summary>
    public System.Windows.Forms.Label Label22 = null!;
    /// <summary>DFM :576 `Label23: TLabel`。</summary>
    public System.Windows.Forms.Label Label23 = null!;
    /// <summary>DFM :583 `Label24: TLabel`。</summary>
    public System.Windows.Forms.Label Label24 = null!;
    /// <summary>DFM :590 `Bevel2: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel2 = null!;
    /// <summary>DFM :597 `Label25: TLabel`。</summary>
    public System.Windows.Forms.Label Label25 = null!;
    /// <summary>DFM :604 `Label26: TLabel`。</summary>
    public System.Windows.Forms.Label Label26 = null!;
    /// <summary>DFM :611 `Bevel4: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel4 = null!;
    /// <summary>DFM :618 `Label27: TLabel`。</summary>
    public System.Windows.Forms.Label Label27 = null!;
    /// <summary>DFM :625 `Bevel6: TBevel`。</summary>
    public System.Windows.Forms.Label Bevel6 = null!;
    /// <summary>DFM :632 `Label28: TLabel`。</summary>
    public System.Windows.Forms.Label Label28 = null!;
    /// <summary>DFM :639 `seDummyHeroHPTime_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroHPTime_Warrior = null!;
    /// <summary>DFM :653 `seDummyHeroMPTime_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroMPTime_Warrior = null!;
    /// <summary>DFM :667 `seDummyHeroHPTime_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroHPTime_DF = null!;
    /// <summary>DFM :681 `seDummyHeroMPTime_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroMPTime_DF = null!;
    /// <summary>DFM :695 `seDummyHeroHPBase_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroHPBase_DF = null!;
    /// <summary>DFM :709 `seDummyHeroMPBase_DF: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroMPBase_DF = null!;
    /// <summary>DFM :723 `seDummyHeroHPBase_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroHPBase_Warrior = null!;
    /// <summary>DFM :737 `seDummyHeroMPBase_Warrior: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seDummyHeroMPBase_Warrior = null!;
    /// <summary>DFM :752 `grp14: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox grp14 = null!;
    /// <summary>DFM :759 `Label29: TLabel`。</summary>
    public System.Windows.Forms.Label Label29 = null!;
    /// <summary>DFM :766 `Label30: TLabel`。</summary>
    public System.Windows.Forms.Label Label30 = null!;
    /// <summary>DFM :773 `chkDummyAutoAddHP: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyAutoAddHP = null!;
    /// <summary>DFM :782 `seDummyAddHPPercent: TSpinEditEx`（Min=1 Max=100 Value=60）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyAddHPPercent = null!;
    /// <summary>DFM :795 `chkDummyAutoAddMP: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyAutoAddMP = null!;
    /// <summary>DFM :804 `seDummyAddMPPercent: TSpinEditEx`（Min=1 Max=100 Value=60）。</summary>
    public System.Windows.Forms.NumericUpDown seDummyAddMPPercent = null!;

    // ---- DFM :822-935 ts3 ----
    /// <summary>DFM :822 `GroupBox11: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox11 = null!;
    /// <summary>DFM :829 `chkDisDummyRun: TCheckBox`（原文语义**取反**：Checked 表示"禁止跑动"）。</summary>
    public System.Windows.Forms.CheckBox chkDisDummyRun = null!;
    /// <summary>DFM :839 `chkDummyRunHum: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyRunHum = null!;
    /// <summary>DFM :849 `chkDummyRunMon: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyRunMon = null!;
    /// <summary>DFM :859 `chkDummyRunNpc: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyRunNpc = null!;
    /// <summary>DFM :869 `chkDummyRunGuard: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyRunGuard = null!;
    /// <summary>DFM :879 `chkDummySafeArea: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummySafeArea = null!;
    /// <summary>DFM :888 `chkDummySafeAreaDisNpcRun: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummySafeAreaDisNpcRun = null!;
    /// <summary>DFM :897 `chkSafeAreaDisShopStallDummyRun: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkSafeAreaDisShopStallDummyRun = null!;
    /// <summary>DFM :906 `chkSafeAreaDisOffLineDummyRun: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkSafeAreaDisOffLineDummyRun = null!;
    /// <summary>DFM :915 `chkDummyWarDisHumRun: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyWarDisHumRun = null!;
    /// <summary>DFM :925 `chkDummyWarHreoRun: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkDummyWarHreoRun = null!;

    // ---- DFM :941-1023 ts4 ----
    /// <summary>DFM :941 `GroupBox12: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox12 = null!;
    /// <summary>DFM :948 `lstDisableMoveMap: TListBox`（DFM **未设** `MultiSelect` ⇒ 单选）。</summary>
    public System.Windows.Forms.ListBox lstDisableMoveMap = null!;
    /// <summary>DFM :959 `btnDisableMoveMapAdd: TButton`。</summary>
    public System.Windows.Forms.Button btnDisableMoveMapAdd = null!;
    /// <summary>DFM :968 `btnDisableMoveMapDelete: TButton`（DFM `Enabled = False`）。</summary>
    public System.Windows.Forms.Button btnDisableMoveMapDelete = null!;
    /// <summary>DFM :978 `btnDisableMoveMapAddAll: TButton`。</summary>
    public System.Windows.Forms.Button btnDisableMoveMapAddAll = null!;
    /// <summary>DFM :987 `btnDisableMoveMapDeleteAll: TButton`。</summary>
    public System.Windows.Forms.Button btnDisableMoveMapDeleteAll = null!;
    /// <summary>DFM :996 `btnDisableMoveMapSave: TButton`。</summary>
    public System.Windows.Forms.Button btnDisableMoveMapSave = null!;
    /// <summary>DFM :1006 `GroupBox13: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox13 = null!;
    /// <summary>DFM :1013 `lstMapList: TListBox`（`MultiSelect = True`）。</summary>
    public System.Windows.Forms.ListBox lstMapList = null!;

    // ---- DFM :1028-1111 ts5 ----
    /// <summary>DFM :1028 `GroupBox14: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox14 = null!;
    /// <summary>DFM :1035 `lstNoAttackMonList: TListBox`（DFM **未设** `MultiSelect` ⇒ 单选）。</summary>
    public System.Windows.Forms.ListBox lstNoAttackMonList = null!;
    /// <summary>DFM :1046 `btnNoAttackMonDel: TButton`（DFM `Enabled = False`）。</summary>
    public System.Windows.Forms.Button btnNoAttackMonDel = null!;
    /// <summary>DFM :1056 `btnNoAttackMonAddAll: TButton`。</summary>
    public System.Windows.Forms.Button btnNoAttackMonAddAll = null!;
    /// <summary>DFM :1065 `btnNoAttackMonDelAll: TButton`。</summary>
    public System.Windows.Forms.Button btnNoAttackMonDelAll = null!;
    /// <summary>DFM :1074 `btnNoAttackMonSave: TButton`。</summary>
    public System.Windows.Forms.Button btnNoAttackMonSave = null!;
    /// <summary>DFM :1084 `GroupBox15: TGroupBox`。</summary>
    public System.Windows.Forms.GroupBox GroupBox15 = null!;
    /// <summary>DFM :1091 `lstMonList: TListBox`（`MultiSelect = True`）。</summary>
    public System.Windows.Forms.ListBox lstMonList = null!;
    /// <summary>DFM :1102 `btnNoAttackMonAdd: TButton`。</summary>
    public System.Windows.Forms.Button btnNoAttackMonAdd = null!;

    // ---- DFM :1113-1131 窗体底部按钮（**不在任何 TabSheet 内**） ----
    /// <summary>DFM :1113 `ButtonDummySave: TButton`（DFM 未设 Enabled ⇒ 默认 True）。</summary>
    public System.Windows.Forms.Button ButtonDummySave = null!;
    /// <summary>DFM :1122 `btn1: TButton`（「默认」按钮）。</summary>
    public System.Windows.Forms.Button btn1 = null!;
}
