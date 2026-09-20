// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  本文件：FormCreate（:229-325）+ 全部 54 个事件处理器 + ModValue/uModValue
//          （:327-1078），按原文出现顺序逐方法 1:1 移植。
//  方法名 / 字段名 / 分支顺序 / 边界行为照抄；原文笔误与冗余保留并注释
//  `// 原文如此（<文件>:<行>）`。
//
//  ★ 可见性：原文这些过程都在 `TFrmDummySetting` 的 published 面（由 DFM 事件绑定），
//    托管侧落为 `public` 以便单元测试**直调事件处理器**
//    （任务书硬性要求 3 的"事件处理器直调 + 决策镜像"），与既有 `GamePetsForm` 家族一致。
//
//  ★ 原文**实际存在但 DFM 未绑定**的处理器：
//    `edtDummyHomeMapChange`（:408-411，空体）在 DFM :154-162 上**没有** OnChange
//    （DFM 该控件只有 Left/Top/…/TabOrder/Text，**无** `OnChange = edtDummyHomeMapChange`），
//    即原文里这个事件**永不触发**。1:1 保留空体 + 本注释。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.DummySetting;

public sealed partial class TFrmDummySetting
{
    // ========================================================================
    //  纯逻辑（UI 取值/写回规则；任务书硬性要求 3：单独测）
    // ========================================================================

    /// <summary>
    /// Delphi `TSpinEditEx.Value := V` 的**钳制**语义：低于 `MinValue` 静默取 `MinValue`、
    /// 高于 `MaxValue` 静默取 `MaxValue`（不抛异常）。
    /// <para>
    /// 抽为纯函数以便单测（原文在 `FormCreate`/`btn1Click` 共 40+ 处写 `SpinEditEx.Value`）。
    /// `maxValue == 0` 表示 DFM 的"不钳制"（`seDummyHPTime_Warrior` 族：DFM `MinValue=0 MaxValue=0`）。
    /// </para>
    /// </summary>
    public static int ClampSpinValue(int value, int minValue, int maxValue)
    {
        int hi = maxValue == 0 ? int.MaxValue : maxValue;
        if (value < minValue) return minValue;
        if (value > hi) return hi;
        return value;
    }

    /// <summary>
    /// `chkDisDummyRunClick`（:616-663）的**取值/写回规则**抽成纯逻辑（可单测）：
    /// <para>
    /// `boChecked := not chkDisDummyRun.Checked`（:620，**取反**）。
    /// `boChecked = True` ⇒ 8 个从属勾选 `Checked := False; Enabled := False`
    /// （:623-645，顺序见原文；**唯一例外**是末尾的 `chkSafeAreaDisOffLineDummyRun`，
    /// 原文先 `Enabled` 后 `Checked` —— :644-645）。
    /// `boChecked = False` ⇒ 只把 8 个从属的 `Enabled := True`（:649-657），
    /// **不动 `Checked`**（原文本分支确实不回填勾选状态）。
    /// </para>
    /// </summary>
    public static void ApplyDisableDummyRun(DummySettingControls ct, bool chkDisDummyRunChecked)
    {
        bool boChecked = !chkDisDummyRunChecked;        // :620
        if (boChecked)
        {
            // :623-642
            ct.chkDummyRunHum.Checked = false;
            ct.chkDummyRunHum.Enabled = false;

            ct.chkDummyRunMon.Checked = false;
            ct.chkDummyRunMon.Enabled = false;

            ct.chkDummyRunNpc.Checked = false;
            ct.chkDummyRunNpc.Enabled = false;

            ct.chkDummyRunGuard.Checked = false;
            ct.chkDummyRunGuard.Enabled = false;

            ct.chkDummySafeArea.Checked = false;
            ct.chkDummySafeArea.Enabled = false;

            ct.chkDummySafeAreaDisNpcRun.Checked = false;
            ct.chkDummySafeAreaDisNpcRun.Enabled = false;

            ct.chkSafeAreaDisShopStallDummyRun.Checked = false;
            ct.chkSafeAreaDisShopStallDummyRun.Enabled = false;

            // 原文如此（:644-645）：**Enabled 在前、Checked 在后**，与上面 7 个相反
            ct.chkSafeAreaDisOffLineDummyRun.Enabled = false;
            ct.chkSafeAreaDisOffLineDummyRun.Checked = false;
        }
        else
        {
            // :649-657（只置 Enabled，不回填 Checked）
            ct.chkDummyRunHum.Enabled = true;
            ct.chkDummyRunMon.Enabled = true;
            ct.chkDummyRunNpc.Enabled = true;
            ct.chkDummyRunGuard.Enabled = true;

            ct.chkDummySafeArea.Enabled = true;
            ct.chkSafeAreaDisShopStallDummyRun.Enabled = true;
            ct.chkSafeAreaDisOffLineDummyRun.Enabled = true;
            ct.chkDummySafeAreaDisNpcRun.Enabled = true;
        }
    }

    /// <summary>
    /// `btnDisableMoveMapAddClick`（:919-940）的**取值/写回规则**抽成纯逻辑（可单测）。
    /// <para>
    /// 原文 `:924 if lstMapList.Items.Count >= 0 then` —— **恒真**（Count 不可能 &lt; 0）。
    /// 原文如此（uFrmDummySetting.pas:924）。1:1 保留该恒真外层 `if`。
    /// </para>
    /// <para>
    /// 内层 `:932 if lstDisableMoveMap.Items.IndexOf(sMapName) < 0 then` ——
    /// **去重**：已在目标列表里的名字**不**重复添加（大小写敏感，`TStrings.IndexOf` 语义）。
    /// </para>
    /// <returns>`true` = 外层恒真 `if` 已进入（对应原文 :937 置脏）。</returns>
    /// </summary>
    public static bool AddSelectedMaps(DummySettingControls ct)
    {
        if (ct.lstMapList.Items.Count >= 0)                 // :924 原文如此：恒真
        {
            for (int I = 0; I <= ct.lstMapList.Items.Count - 1; I++)   // :926
            {
                if (!IsSelected(ct.lstMapList, I))          // :928
                    continue;                               // :929

                string sMapName = ct.lstMapList.Items[I].ToString()!;   // :931
                if (ct.lstDisableMoveMap.Items.IndexOf(sMapName) < 0)   // :932
                {
                    ct.lstDisableMoveMap.Items.Add(sMapName);           // :934
                }
            }
            return true;                                    // :937 FIsDummyDisableMoveMapChanged := True
        }
        return false;
    }

    /// <summary>
    /// `btnNoAttackMonAddClick`（:1003-1025）的**取值/写回规则**抽成纯逻辑（可单测）。
    /// 与 <see cref="AddSelectedMaps"/> 同形，但：
    /// `:1008 if lstMonList.Items.Count >= 0`（同样**恒真**，原文如此）、
    /// `:1016 if lstNoAttackMonList.Items.IndexOf(sItemName) < 0`（去重）。
    /// </summary>
    /// <returns>`true` = 外层恒真 `if` 已进入（对应原文 :1023 置脏）。</returns>
    public static bool AddSelectedMons(DummySettingControls ct)
    {
        if (ct.lstMonList.Items.Count >= 0)                 // :1008 原文如此：恒真
        {
            for (int I = 0; I <= ct.lstMonList.Items.Count - 1; I++)   // :1010
            {
                if (!IsSelected(ct.lstMonList, I))          // :1012
                    continue;                               // :1013

                string sItemName = ct.lstMonList.Items[I].ToString()!;  // :1015
                if (ct.lstNoAttackMonList.Items.IndexOf(sItemName) < 0) // :1016
                {
                    ct.lstNoAttackMonList.Items.Add(sItemName);         // :1018
                }
            }
            return true;                                    // :1023 FIsDummyNoActiveAttackMonChanged := True
        }
        return false;
    }

    // ========================================================================
    //  :229-325 FormCreate
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.FormCreate`（:229-325）1:1。
    /// <para>
    /// ★ Delphi `TSpinEditEx.Value := X` 在 X 与当前值不同时会**触发 OnChange**
    /// （托管侧 WinForms `NumericUpDown.ValueChanged` 同样触发）⇒ 本方法里
    /// 40+ 处回填会连带触发各自的 `*Change` 处理器（把同一个值写回 `g_Config` + `ModValue`）。
    /// 原文如此（uFrmDummySetting.pas:239-283 对 `g_Config` 是**同值回写**，故无副作用差异），
    /// 托管侧 1:1 保留该连带触发。
    /// </para>
    /// </summary>
    public void FormCreate()
    {
        // :235-236
        Ct.lstDummyList.Items.Clear();
        Ct.lstDummyList.Items.AddRange(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));

        // :238-242
        Ct.edtDummyHomeMap.Text = M2Config.sDummyHomeMap;
        Ct.seDummyHomeX.Value = ClampSpinValue(M2Config.nDummyHomeX, 1, 2000);
        Ct.seDummyHomeY.Value = ClampSpinValue(M2Config.nDummyHomeY, 1, 2000);
        Ct.seDummyLogonTime.Value = ClampSpinValue(M2Config.nDummyLogonTime, 1, 2000);
        Ct.chkDummyLogonRand.Checked = M2Config.boDummyLogonRand;

        // :244-245
        Ct.CheckBoxDummyAutoRepairItem.Checked = M2Config.boDummyAutoRepairItem;
        Ct.CheckBoxDummyAutoRecallHero.Checked = M2Config.boDummyAutoRecallHero;

        // :247-251
        Ct.chkDummyAutoAddHP.Checked = M2Config.boDummyAutoAddHP;
        Ct.seDummyAddHPPercent.Value = ClampSpinValue(M2Config.nDummyAddHPPercent, 1, 100);

        Ct.chkDummyAutoAddMP.Checked = M2Config.boDummyAutoAddMP;
        Ct.seDummyAddMPPercent.Value = ClampSpinValue(M2Config.nDummyAddMPPercent, 1, 100);

        // :253-263
        Ct.seDummyHPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHPTime_Warrior, 0, 0);
        Ct.seDummyHPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHPBase_Warrior, 0, 0);

        Ct.seDummyMPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyMPTime_Warrior, 0, 0);
        Ct.seDummyMPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyMPBase_Warrior, 0, 0);

        Ct.seDummyHPTime_DF.Value = ClampSpinValue(M2Config.nDummyHPTime_DF, 0, 0);
        Ct.seDummyHPBase_DF.Value = ClampSpinValue(M2Config.nDummyHPBase_DF, 0, 0);

        Ct.seDummyMPTime_DF.Value = ClampSpinValue(M2Config.nDummyMPTime_DF, 0, 0);
        Ct.seDummyMPBase_DF.Value = ClampSpinValue(M2Config.nDummyMPBase_DF, 0, 0);

        // :265-275
        Ct.seDummyHeroHPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroHPTime_Warrior, 0, 0);
        Ct.seDummyHeroHPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroHPBase_Warrior, 0, 0);

        Ct.seDummyHeroMPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroMPTime_Warrior, 0, 0);
        Ct.seDummyHeroMPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroMPBase_Warrior, 0, 0);

        Ct.seDummyHeroHPTime_DF.Value = ClampSpinValue(M2Config.nDummyHeroHPTime_DF, 0, 0);
        Ct.seDummyHeroHPBase_DF.Value = ClampSpinValue(M2Config.nDummyHeroHPBase_DF, 0, 0);

        Ct.seDummyHeroMPTime_DF.Value = ClampSpinValue(M2Config.nDummyHeroMPTime_DF, 0, 0);
        Ct.seDummyHeroMPBase_DF.Value = ClampSpinValue(M2Config.nDummyHeroMPBase_DF, 0, 0);

        // :277-283
        Ct.EditDummyWarrorAttackTime.Value = ClampSpinValue(M2Config.dwDummyWarrorAttackTime, 10, 10000);
        Ct.EditDummyWizardAttackTime.Value = ClampSpinValue(M2Config.dwDummyWizardAttackTime, 10, 10000);
        Ct.EditDummyTaoistAttackTime.Value = ClampSpinValue(M2Config.dwDummyTaoistAttackTime, 10, 10000);

        Ct.EditDummyWarrorWalkTime.Value = ClampSpinValue(M2Config.dwDummyWarrorWalkTime, 10, 10000);
        Ct.EditDummyWizardWalkTime.Value = ClampSpinValue(M2Config.dwDummyWizardWalkTime, 10, 10000);
        Ct.EditDummyTaoistWalkTime.Value = ClampSpinValue(M2Config.dwDummyTaoistWalkTime, 10, 10000);

        // :285-296
        Ct.chkDisDummyRun.Checked = !M2Config.boDiableDummyRun;
        Ct.chkDummyRunHum.Checked = M2Config.boDummyRunHum;
        Ct.chkDummyRunMon.Checked = M2Config.boDummyRunMon;
        Ct.chkDummyRunNpc.Checked = M2Config.boDummyRunNpc;
        Ct.chkDummyRunGuard.Checked = M2Config.boDummyRunGuard;
        Ct.chkDummySafeArea.Checked = M2Config.boDummySafeAreaLimited;
        Ct.chkDummyWarDisHumRun.Checked = M2Config.boDummyWarDisHumRun;
        Ct.chkDummyWarHreoRun.Checked = M2Config.boDummyWarHreoRun;
        Ct.chkDummyWarHreoRun.Enabled = M2Config.boDummyWarDisHumRun;       // :293
        Ct.chkDummySafeAreaDisNpcRun.Checked = M2Config.boDummySafeAreaDisNpcRun;
        Ct.chkSafeAreaDisShopStallDummyRun.Checked = M2Config.boSafeAreaDisShopStallDummyRun;
        Ct.chkSafeAreaDisOffLineDummyRun.Checked = M2Config.boSafeAreaDisOffLineDummyRun;

        // :298-299
        Ct.pgcMain.SelectedIndex = 0;                                       // 原文 ActivePageIndex := 0
        Ct.btnDummyLogon.Enabled = false;

        // :301-305
        var mapEnvir = MapListHandler?.Invoke();
        if (mapEnvir != null)
        {
            for (int I = 0; I <= mapEnvir.Count - 1; I++)
            {
                TEnvirnoment Envir = mapEnvir[I];
                Ct.lstMapList.Items.Add(Envir.sMapName);
            }
        }

        // :307-318（{$IF MULTI_THREAD = 1} 包裹）
        if (g_MultiThreadRun)
            MonsterListLockR?.Invoke(6);                                    // :308 LockR(6)
        try
        {
            var monsters = MonsterListHandler?.Invoke();
            if (monsters != null)
            {
                for (int I = 0; I <= monsters.Count - 1; I++)
                {
                    (string sName, byte btRace) MonInfo = monsters[I];
                    // :313 原文 lstMonList.Items.AddObject(MonInfo.sName, TObject(MonInfo))
                    // 托管侧 AddObject 的 Objects[] 载体**全树 0 消费者**，故只保留 sName。
                    Ct.lstMonList.Items.Add(MonInfo.sName);
                }
            }
        }
        finally
        {
            if (g_MultiThreadRun)
                MonsterListUnLockR?.Invoke();                               // :317 UnLockR
        }

        // :320-321
        // 原文如此（uFrmDummySetting.pas:320-321）：两个都是 `Items.Assign` ——
        // `TListBox.Items` 在 Delphi 里**没有** Assign 方法（`TStrings.Assign` 才是），
        // 这两行**无法按字面编译**；原意显然是「把全局列表拷进控件」。
        // 托管侧按语义等价实现为「清空后逐项添加」。
        Ct.lstDisableMoveMap.Items.Clear();
        Ct.lstDisableMoveMap.Items.AddRange(DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
        Ct.lstNoAttackMonList.Items.Clear();
        Ct.lstNoAttackMonList.Items.AddRange(DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));

        // :323-324
        FIsDummyDisableMoveMapChanged = false;
        FIsDummyNoActiveAttackMonChanged = false;
    }

    // ========================================================================
    //  :327-343 lstDummyListClick
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.lstDummyListClick`（:327-343）1:1。
    /// 原文 `TListBox.ItemIndex` 的越界语义（Delphi 恒为 -1 或有效索引，不可能越界），
    /// 托管 `ListBox.SelectedIndex` 同语义 ⇒ 无需 §21.3 的 `SetItemIndex` 垫片；
    /// 但原文 :332 仍显式写了 `(ItemIndex &gt;= 0) and (ItemIndex &lt; Items.Count)` 双条件，1:1 保留。
    /// </summary>
    public void lstDummyListClick(System.Windows.Forms.ListBox lstDummyList)
    {
        int ItemIndex = lstDummyList.SelectedIndex;                          // :331
        if (ItemIndex >= 0 && ItemIndex < lstDummyList.Items.Count)          // :332
        {
            Ct.btnDummyLogon.Enabled = true;                                // :334
            Ct.btnDummyDel.Enabled = true;                                  // :335
            Ct.edtDummyName.Text = lstDummyList.Items[ItemIndex].ToString()!;   // :336（原文无行尾分号）
        }
        else
        {
            Ct.btnDummyLogon.Enabled = false;                               // :340
            Ct.btnDummyDel.Enabled = false;                                 // :341
        }
    }

    // ========================================================================
    //  :345-374 btnDummyLogonClick
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.btnDummyLogonClick`（:345-374）1:1。
    /// <para>
    /// ⚠ 原文 :350 / :356-358 **全部读 `g_Config.*` 而不是控件值**
    /// （`edtDummyHomeMap`/`seDummyHomeX/Y` 只在 `*Change` 里写 `g_Config`）；1:1 保留。
    /// 差异断言见测试 `BtnDummyLogon_UsesConfigNotControls`。
    /// </para>
    /// <para>
    /// ★ `DummyLogon` 在原文是**栈上局部记录**（`DummyLogon: TDummyLogon`，:348 **未** `New`），
    /// 托管 `TDummyLogon` 是 `class` ⇒ 每轮循环 new 一个（等价于原文每轮覆写同栈槽 +
    /// `AddDummyLogon` 内部按值拷贝；见 `UsrEngn.pas:4307-4322`）。
    /// </para>
    /// </summary>
    public void btnDummyLogonClick(System.Windows.Forms.Button btnDummyLogon)
    {
        // :350-355
        if (FindMap(M2Config.sDummyHomeMap) == null)
        {
            ShowMessageBox("出生地图设置错误！", "错误信息", M2Forms.MB_OK + M2Forms.MB_ICONERROR);   // :352
            ProbeSetFocus(nameof(Ct.edtDummyHomeMap));                              // :353 edtDummyHomeMap.SetFocus
            return;
        }

        // :356-358
        string sMapName = M2Config.sDummyHomeMap;
        int nX = M2Config.nDummyHomeX;
        int nY = M2Config.nDummyHomeY;

        // :360-372
        for (int Index = 0; Index <= Ct.lstDummyList.Items.Count - 1; Index++)
        {
            if (IsSelected(Ct.lstDummyList, Index))                             // :362
            {
                // 原文 :364 被注释掉的 MainOutMessage 调试行（1:1 保留为注释）
                // MainOutMessage('ListBoxAIList.Selected[Index]1:' + ListBoxAIList.Items.Strings[Index]);
                string sCharName = Ct.lstDummyList.Items[Index].ToString()!;
                if (GetPlayObject(sCharName) == null && !FindDummyLogon(sCharName))   // :365
                {
                    var DummyLogon = new TDummyLogon { sCharName = sCharName };      // :368
                    DummyLogon.sMapName = sMapName;
                    DummyLogon.nX = nX;
                    DummyLogon.nY = nY;
                    AddDummyLogonHandler?.Invoke(DummyLogon);                       // :369
                }
            }
        }
        btnDummyLogon.Enabled = false;                                          // :373
    }

    // ========================================================================
    //  :376-393 btnDummyAddClick
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.btnDummyAddClick`（:376-393）1:1。
    /// 原文分支：`Trim` 后非空 **且** `not GetDummyNameList(sName)`（大小写无关去重）。
    /// </summary>
    public void btnDummyAddClick(System.Windows.Forms.Button btnDummyAdd)
    {
        string sName = DelphiRTL.Trim(Ct.edtDummyName.Text);                    // :380
        if (sName != "" && !DummySettingState.GetDummyNameList(sName))          // :381
        {
            DummySettingState.Lock();                           // :383
            try
            {
                DummySettingState.g_DummyNameList.Add(sName);                   // :385
                Ct.lstDummyList.Items.Clear();                                  // :386
                Ct.lstDummyList.Items.AddRange(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));   // :387
            }
            finally
            {
                DummySettingState.UnLock();                     // :389
            }
            ModValue();                                                         // :391
        }
    }

    // ========================================================================
    //  :395-406 btnDummyDelClick
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.btnDummyDelClick`（:395-406）1:1。
    /// 原文 `lstDummyList.DeleteSelected`（:397）删**全部选中**行，随后用控件列表
    /// **反向覆盖**全局列表（:400-401 `Clear` + `AddStrings(lstDummyList.Items)`）。
    /// </summary>
    public void btnDummyDelClick(System.Windows.Forms.Button btnDummyDel)
    {
        DeleteSelectedRows(Ct.lstDummyList);                                    // :397 lstDummyList.DeleteSelected
        DummySettingState.Lock();                               // :398
        try
        {
            DummySettingState.g_DummyNameList.Clear();                          // :400
            foreach (var item in Ct.lstDummyList.Items)                         // :401 AddStrings(lstDummyList.Items)
                DummySettingState.g_DummyNameList.Add(item.ToString()!);
        }
        finally
        {
            DummySettingState.UnLock();                         // :403
        }
        ModValue();                                                             // :405
    }

    // ========================================================================
    //  :408-411 edtDummyHomeMapChange（原文空体）
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.edtDummyHomeMapChange`（:408-411）1:1 —— **空体**（原文只有 `//`）。
    /// <para>
    /// ⚠ 该处理器在 `uFrmDummySetting.dfm` 里**没有任何控件绑定 `OnChange`**
    /// （DFM :154-162 `edtDummyHomeMap` 无 `OnChange` 行）⇒ 原文中**永不触发**。
    /// 1:1 保留（含空体），并在 DFM 控件树上照原文"不绑定"以免改变行为。
    /// </para>
    /// </summary>
    public void edtDummyHomeMapChange(System.Windows.Forms.TextBox edtDummyHomeMap)
    {
        // :410 原文如此：方法体只有一行注释 `//`，无任何语句
    }

    // ========================================================================
    //  :413-429 seDummyHomeXChange / seDummyHomeYChange / seDummyLogonTimeChange
    // ========================================================================

    /// <summary>`TFrmDummySetting.seDummyHomeXChange`（:413-417）1:1。</summary>
    public void seDummyHomeXChange(System.Windows.Forms.NumericUpDown seDummyHomeX)
    {
        M2Config.nDummyHomeX = (int)seDummyHomeX.Value;                         // :415
        ModValue();                                                             // :416
    }

    /// <summary>`TFrmDummySetting.seDummyHomeYChange`（:419-423）1:1。</summary>
    public void seDummyHomeYChange(System.Windows.Forms.NumericUpDown seDummyHomeY)
    {
        M2Config.nDummyHomeY = (int)seDummyHomeY.Value;                         // :421
        ModValue();                                                             // :422
    }

    /// <summary>`TFrmDummySetting.seDummyLogonTimeChange`（:425-429）1:1。</summary>
    public void seDummyLogonTimeChange(System.Windows.Forms.NumericUpDown seDummyLogonTime)
    {
        M2Config.nDummyLogonTime = (int)seDummyLogonTime.Value;                 // :427
        ModValue();                                                             // :428
    }

    // ========================================================================
    //  :431-550 ButtonDummySaveClick
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.ButtonDummySaveClick`（:431-550）1:1。
    /// <para>
    /// 分支顺序逐字保留：
    ///   ① `:435` `edtDummyHomeMap.Text = ''` ⇒ 弹窗 + `SetFocus` + `Exit`
    ///   ② `:441` `g_Config.sDummyHomeMap := Trim(edtDummyHomeMap.Text)`（**先落配置再验图**）
    ///   ③ `:443` `g_MapManager.FindMap(...) = nil` ⇒ 弹窗 + `SetFocus` + `Exit`
    ///      —— 注意此时 `g_Config.sDummyHomeMap` **已被写入**（原文如此，失败时**不回滚**）
    ///   ④ 52 个 `Config.Write*('Setup', ...)`
    ///   ⑤ `SaveDummyNameList`
    ///   ⑥ 条件块 `FIsDummyDisableMoveMapChanged` ⇒ 覆写全局 + `Sorted := True` + `SaveDummyDisableMoveMap()`
    ///   ⑦ 条件块 `FIsDummyNoActiveAttackMonChanged` ⇒ 同上 + `SaveDummyNoActiveAttackMonList()`
    ///   ⑧ `uModValue()`
    /// </para>
    /// </summary>
    public void ButtonDummySaveClick(System.Windows.Forms.Button ButtonDummySave)
    {
        // :435-440
        if (Ct.edtDummyHomeMap.Text == "")
        {
            ShowMessageBox("出生地图设置错误！", "错误信息", M2Forms.MB_OK + M2Forms.MB_ICONERROR);   // :437
            ProbeSetFocus(nameof(Ct.edtDummyHomeMap));                              // :438
            return;
        }
        M2Config.sDummyHomeMap = DelphiRTL.Trim(Ct.edtDummyHomeMap.Text);          // :441

        // :443-448
        if (FindMap(M2Config.sDummyHomeMap) == null)
        {
            ShowMessageBox("出生地图设置错误！", "错误信息", M2Forms.MB_OK + M2Forms.MB_ICONERROR);   // :445
            ProbeSetFocus(nameof(Ct.edtDummyHomeMap));                              // :446
            return;
        }

        // :450-451 原文注释掉的两行（1:1 保留为注释）
        // g_Config.sDummyConfigListFileName := Trim(EditDummyConfigListFileName.Text);
        // g_Config.sDummyHeroConfigListFileName := Trim(EditDummyHeroConfigListFileName.Text);

        WriteBool("DummyLogonRand", M2Config.boDummyLogonRand);                 // :452

        WriteInteger("DummyLogonTime", M2Config.nDummyLogonTime);               // :454
        WriteInteger("DummyHomeX", M2Config.nDummyHomeX);                       // :455
        WriteInteger("DummyHomeY", M2Config.nDummyHomeY);                       // :456
        WriteString("DummyHomeMap", M2Config.sDummyHomeMap);                    // :457
        // :458-459 原文注释掉的两行
        // Config.WriteString('Setup', 'DummyConfigListFileName', g_Config.sDummyConfigListFileName);
        // Config.WriteString('Setup', 'DummyHeroConfigListFileName', g_Config.sDummyHeroConfigListFileName);

        WriteBool("DummyAutoRepairItem", M2Config.boDummyAutoRepairItem);       // :461
        WriteBool("DummyAutoRecallHero", M2Config.boDummyAutoRecallHero);       // :462
        WriteInteger("DummyWarrorAttackTime", M2Config.dwDummyWarrorAttackTime);   // :463
        WriteInteger("DummyWizardAttackTime", M2Config.dwDummyWizardAttackTime);   // :464
        WriteInteger("DummyTaoistAttackTime", M2Config.dwDummyTaoistAttackTime);   // :465
        WriteInteger("DummyWarrorWalkTime", M2Config.dwDummyWarrorWalkTime);       // :466
        WriteInteger("DummyWizardWalkTime", M2Config.dwDummyWizardWalkTime);       // :467
        WriteInteger("DummyTaoistWalkTime", M2Config.dwDummyTaoistWalkTime);       // :468

        WriteBool("DummyAutoAddHP", M2Config.boDummyAutoAddHP);                 // :470
        WriteInteger("DummyAddHPPercent", M2Config.nDummyAddHPPercent);         // :471

        WriteBool("DummyAutoAddMP", M2Config.boDummyAutoAddMP);                 // :473
        WriteInteger("DummyAddMPPercent", M2Config.nDummyAddMPPercent);         // :474

        WriteInteger("DummyHPTime_Warrior", M2Config.nDummyHPTime_Warrior);     // :476
        WriteInteger("DummyHPBase_Warrior", M2Config.nDummyHPBase_Warrior);     // :477

        WriteInteger("DummyMPTime_Warrior", M2Config.nDummyMPTime_Warrior);     // :479
        WriteInteger("DummyMPBase_Warrior", M2Config.nDummyMPBase_Warrior);     // :480

        WriteInteger("DummyHPTime_DF", M2Config.nDummyHPTime_DF);               // :482
        WriteInteger("DummyHPBase_DF", M2Config.nDummyHPBase_DF);               // :483

        WriteInteger("DummyMPTime_DF", M2Config.nDummyMPTime_DF);               // :485
        WriteInteger("DummyMPBase_DF", M2Config.nDummyMPBase_DF);               // :486

        WriteInteger("DummyHeroHPTime_Warrior", M2Config.nDummyHeroHPTime_Warrior);   // :488
        WriteInteger("DummyHeroHPBase_Warrior", M2Config.nDummyHeroHPBase_Warrior);   // :489

        WriteInteger("DummyHeroMPTime_Warrior", M2Config.nDummyHeroMPTime_Warrior);   // :491
        WriteInteger("DummyHeroMPBase_Warrior", M2Config.nDummyHeroMPBase_Warrior);   // :492

        WriteInteger("DummyHeroHPTime_DF", M2Config.nDummyHeroHPTime_DF);       // :494
        WriteInteger("DummyHeroHPBase_DF", M2Config.nDummyHeroHPBase_DF);       // :495

        WriteInteger("DummyHeroMPTime_DF", M2Config.nDummyHeroMPTime_DF);       // :497
        WriteInteger("DummyHeroMPBase_DF", M2Config.nDummyHeroMPBase_DF);       // :498

        WriteBool("DiableDummyRun", M2Config.boDiableDummyRun);                 // :500（原文如此：拼写 Diable）
        WriteBool("DummyRunHum", M2Config.boDummyRunHum);                       // :501
        WriteBool("DummyRunMon", M2Config.boDummyRunMon);                       // :502
        WriteBool("DummyRunNpc", M2Config.boDummyRunNpc);                       // :503
        WriteBool("DummyRunGuard", M2Config.boDummyRunGuard);                   // :504
        WriteBool("DummyWarDisHumRun", M2Config.boDummyWarDisHumRun);           // :505
        WriteBool("DummyWarHreoRun", M2Config.boDummyWarHreoRun);               // :506
        WriteBool("DummySafeAreaLimited", M2Config.boDummySafeAreaLimited);     // :507
        WriteBool("DummySafeAreaDisNpcRun", M2Config.boDummySafeAreaDisNpcRun); // :508
        WriteBool("SafeAreaDisShopStallDummyRun", M2Config.boSafeAreaDisShopStallDummyRun);   // :509
        WriteBool("SafeAreaDisOffLineDummyRun", M2Config.boSafeAreaDisOffLineDummyRun);       // :510

        DummySettingState.SaveDummyNameList();                                  // :512

        // :514-530
        if (FIsDummyDisableMoveMapChanged)
        {
            DummySettingState.Lock();                           // :516
            try
            {
                DummySettingState.g_DummyDisableMoveMapList.Clear();            // :518
                for (int I = 0; I <= Ct.lstDisableMoveMap.Items.Count - 1; I++)   // :519
                {
                    // 原文如此（:521）：该行**无行尾分号**（Pascal 允许 `end` 前最后一条语句省分号）
                    DummySettingState.g_DummyDisableMoveMapList.Add(Ct.lstDisableMoveMap.Items[I].ToString()!);
                }

                DummySettingState.g_DummyDisableMoveMapList.Sorted = true;      // :524
            }
            finally
            {
                DummySettingState.UnLock();                     // :526
            }

            DummySettingState.SaveDummyDisableMoveMap();                        // :529
        }

        // :532-547
        if (FIsDummyNoActiveAttackMonChanged)
        {
            DummySettingState.Lock();                           // :534
            try
            {
                DummySettingState.g_DummyNoActiveAttackMonList.Clear();         // :536
                for (int I = 0; I <= Ct.lstNoAttackMonList.Items.Count - 1; I++)   // :537
                {
                    DummySettingState.g_DummyNoActiveAttackMonList.Add(Ct.lstNoAttackMonList.Items[I].ToString()!);   // :539（有分号）
                }

                DummySettingState.g_DummyNoActiveAttackMonList.Sorted = true;   // :542
            }
            finally
            {
                DummySettingState.UnLock();                     // :544
            }
            DummySettingState.SaveDummyNoActiveAttackMonList();                 // :546
        }

        uModValue();                                                            // :549
    }

    // ========================================================================
    //  :552-614 btn1Click（「默认」按钮）
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.btn1Click`（:552-614）1:1 —— 把 20 个回血/回蓝参数恢复默认，
    /// 再**回填 16 个 SpinEdit**（:590-612），最后 `ModValue()`（:613）。
    /// <para>
    /// 注意：本方法**不回填** `seDummyAddHPPercent`/`seDummyAddMPPercent`，
    /// 也**不动** `boDummyAutoAddHP/MP` —— 只动 HP/MP 的 Time/Base。原文如此。
    /// </para>
    /// </summary>
    public void btn1Click(System.Windows.Forms.Button btn1)
    {
        // :554-561（原文注释写在每个赋值**之后**的下一行，缩进 4 空格 —— 1:1 保留位置）
        M2Config.nDummyHPTime_Warrior = 350;
        // 假人－战士职业回血速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHPTime_DF = 350;
        // 假人－道法职业回血速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroHPTime_Warrior = 350;
        // 假人英雄－战士职业回血速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroHPTime_DF = 350;
        // 假人英雄－道法职业回血速度 - 2013-07-16  (+ chongchong)

        // :563-570
        M2Config.nDummyMPTime_Warrior = 800;
        // 假人－战士职业回蓝速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyMPTime_DF = 800;
        // 假人－道法职业回蓝速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroMPTime_Warrior = 800;
        // 假人英雄－战士职业回蓝速度 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroMPTime_DF = 800;
        // 假人英雄－道法职业回蓝速度 - 2013-07-16  (+ chongchong)

        // :572-579
        M2Config.nDummyHPBase_Warrior = 75;
        // 假人－战士职业回血基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHPBase_DF = 75;
        // 假人－道法职业回血基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroHPBase_Warrior = 75;
        // 假人英雄－战士职业回血基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroHPBase_DF = 75;
        // 假人英雄－道法职业回血基数 - 2013-07-16  (+ chongchong)

        // :581-588
        M2Config.nDummyMPBase_Warrior = 18;
        // 假人－战士职业回蓝基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyMPBase_DF = 18;
        // 假人－道法职业回蓝基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroMPBase_Warrior = 18;
        // 假人英雄－战士职业回蓝基数 - 2013-07-16  (+ chongchong)
        M2Config.nDummyHeroMPBase_DF = 18;
        // 假人英雄－道法职业回蓝基数 - 2013-07-16  (+ chongchong)

        // :590-612（回填 16 个 SpinEdit；触发各自 OnChange ⇒ 同值回写 g_Config）
        Ct.seDummyHPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHPTime_Warrior, 0, 0);
        Ct.seDummyHPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHPBase_Warrior, 0, 0);

        Ct.seDummyMPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyMPTime_Warrior, 0, 0);
        Ct.seDummyMPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyMPBase_Warrior, 0, 0);

        Ct.seDummyHPTime_DF.Value = ClampSpinValue(M2Config.nDummyHPTime_DF, 0, 0);
        Ct.seDummyHPBase_DF.Value = ClampSpinValue(M2Config.nDummyHPBase_DF, 0, 0);

        Ct.seDummyMPTime_DF.Value = ClampSpinValue(M2Config.nDummyMPTime_DF, 0, 0);
        Ct.seDummyMPBase_DF.Value = ClampSpinValue(M2Config.nDummyMPBase_DF, 0, 0);

        Ct.seDummyHeroHPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroHPTime_Warrior, 0, 0);
        Ct.seDummyHeroHPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroHPBase_Warrior, 0, 0);

        Ct.seDummyHeroMPTime_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroMPTime_Warrior, 0, 0);
        Ct.seDummyHeroMPBase_Warrior.Value = ClampSpinValue(M2Config.nDummyHeroMPBase_Warrior, 0, 0);

        Ct.seDummyHeroHPTime_DF.Value = ClampSpinValue(M2Config.nDummyHeroHPTime_DF, 0, 0);
        Ct.seDummyHeroHPBase_DF.Value = ClampSpinValue(M2Config.nDummyHeroHPBase_DF, 0, 0);

        Ct.seDummyHeroMPTime_DF.Value = ClampSpinValue(M2Config.nDummyHeroMPTime_DF, 0, 0);
        Ct.seDummyHeroMPBase_DF.Value = ClampSpinValue(M2Config.nDummyHeroMPBase_DF, 0, 0);
        ModValue();                                                             // :613
    }

    // ========================================================================
    //  :616-663 chkDisDummyRunClick
    // ========================================================================

    /// <summary>`TFrmDummySetting.chkDisDummyRunClick`（:616-663）1:1。取值/写回规则见
    /// <see cref="ApplyDisableDummyRun"/>（纯逻辑，单独测）。</summary>
    public void chkDisDummyRunClick(System.Windows.Forms.CheckBox chkDisDummyRun)
    {
        bool boChecked = !chkDisDummyRun.Checked;                               // :620
        ApplyDisableDummyRun(Ct, chkDisDummyRun.Checked);                       // :621-658

        M2Config.boDiableDummyRun = boChecked;                                  // :660

        ModValue();                                                             // :662
    }

    // ========================================================================
    //  :665-901 单字段处理器族（写 g_Config + ModValue）
    // ========================================================================

    /// <summary>`TFrmDummySetting.EditDummyWarrorAttackTimeChange`（:665-669）1:1。</summary>
    public void EditDummyWarrorAttackTimeChange(System.Windows.Forms.NumericUpDown EditDummyWarrorAttackTime)
    {
        M2Config.dwDummyWarrorAttackTime = (int)EditDummyWarrorAttackTime.Value;    // :667
        ModValue();                                                                 // :668
    }

    /// <summary>`TFrmDummySetting.chkDummyAutoAddHPClick`（:671-675）1:1。</summary>
    public void chkDummyAutoAddHPClick(System.Windows.Forms.CheckBox chkDummyAutoAddHP)
    {
        M2Config.boDummyAutoAddHP = chkDummyAutoAddHP.Checked;                  // :673
        ModValue();                                                             // :674
    }

    /// <summary>`TFrmDummySetting.seDummyAddHPPercentChange`（:677-681）1:1。</summary>
    public void seDummyAddHPPercentChange(System.Windows.Forms.NumericUpDown seDummyAddHPPercent)
    {
        M2Config.nDummyAddHPPercent = (int)seDummyAddHPPercent.Value;           // :679
        ModValue();                                                             // :680
    }

    /// <summary>`TFrmDummySetting.chkDummyAutoAddMPClick`（:683-687）1:1。</summary>
    public void chkDummyAutoAddMPClick(System.Windows.Forms.CheckBox chkDummyAutoAddMP)
    {
        M2Config.boDummyAutoAddMP = chkDummyAutoAddMP.Checked;                  // :685
        ModValue();                                                             // :686
    }

    /// <summary>`TFrmDummySetting.seDummyAddMPPercentChange`（:689-693）1:1。</summary>
    public void seDummyAddMPPercentChange(System.Windows.Forms.NumericUpDown seDummyAddMPPercent)
    {
        M2Config.nDummyAddMPPercent = (int)seDummyAddMPPercent.Value;           // :691
        ModValue();                                                             // :692
    }

    /// <summary>`TFrmDummySetting.seDummyHPTime_WarriorChange`（:695-699）1:1。</summary>
    public void seDummyHPTime_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHPTime_Warrior)
    {
        M2Config.nDummyHPTime_Warrior = (int)seDummyHPTime_Warrior.Value;       // :697
        ModValue();                                                             // :698
    }

    /// <summary>`TFrmDummySetting.seDummyHPBase_WarriorChange`（:701-705）1:1。</summary>
    public void seDummyHPBase_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHPBase_Warrior)
    {
        M2Config.nDummyHPBase_Warrior = (int)seDummyHPBase_Warrior.Value;       // :703
        ModValue();                                                             // :704
    }

    /// <summary>`TFrmDummySetting.seDummyMPTime_WarriorChange`（:707-711）1:1。</summary>
    public void seDummyMPTime_WarriorChange(System.Windows.Forms.NumericUpDown seDummyMPTime_Warrior)
    {
        M2Config.nDummyMPTime_Warrior = (int)seDummyMPTime_Warrior.Value;       // :709
        ModValue();                                                             // :710
    }

    /// <summary>`TFrmDummySetting.seDummyMPBase_WarriorChange`（:713-717）1:1。</summary>
    public void seDummyMPBase_WarriorChange(System.Windows.Forms.NumericUpDown seDummyMPBase_Warrior)
    {
        M2Config.nDummyMPBase_Warrior = (int)seDummyMPBase_Warrior.Value;       // :715
        ModValue();                                                             // :716
    }

    /// <summary>`TFrmDummySetting.seDummyHPTime_DFChange`（:719-723）1:1。</summary>
    public void seDummyHPTime_DFChange(System.Windows.Forms.NumericUpDown seDummyHPTime_DF)
    {
        M2Config.nDummyHPTime_DF = (int)seDummyHPTime_DF.Value;                 // :721
        ModValue();                                                             // :722
    }

    /// <summary>`TFrmDummySetting.seDummyHPBase_DFChange`（:725-729）1:1。</summary>
    public void seDummyHPBase_DFChange(System.Windows.Forms.NumericUpDown seDummyHPBase_DF)
    {
        M2Config.nDummyHPBase_DF = (int)seDummyHPBase_DF.Value;                 // :727
        ModValue();                                                             // :728
    }

    /// <summary>`TFrmDummySetting.seDummyMPTime_DFChange`（:731-735）1:1。</summary>
    public void seDummyMPTime_DFChange(System.Windows.Forms.NumericUpDown seDummyMPTime_DF)
    {
        M2Config.nDummyMPTime_DF = (int)seDummyMPTime_DF.Value;                 // :733
        ModValue();                                                             // :734
    }

    /// <summary>`TFrmDummySetting.seDummyMPBase_DFChange`（:737-741）1:1。</summary>
    public void seDummyMPBase_DFChange(System.Windows.Forms.NumericUpDown seDummyMPBase_DF)
    {
        M2Config.nDummyMPBase_DF = (int)seDummyMPBase_DF.Value;                 // :739
        ModValue();                                                             // :740
    }

    /// <summary>`TFrmDummySetting.seDummyHeroHPTime_WarriorChange`（:743-747）1:1。</summary>
    public void seDummyHeroHPTime_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHeroHPTime_Warrior)
    {
        M2Config.nDummyHeroHPTime_Warrior = (int)seDummyHeroHPTime_Warrior.Value;   // :745
        ModValue();                                                                 // :746
    }

    /// <summary>`TFrmDummySetting.seDummyHeroHPBase_WarriorChange`（:749-753）1:1。</summary>
    public void seDummyHeroHPBase_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHeroHPBase_Warrior)
    {
        M2Config.nDummyHeroHPBase_Warrior = (int)seDummyHeroHPBase_Warrior.Value;   // :751
        ModValue();                                                                 // :752
    }

    /// <summary>`TFrmDummySetting.seDummyHeroMPTime_WarriorChange`（:755-759）1:1。</summary>
    public void seDummyHeroMPTime_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHeroMPTime_Warrior)
    {
        M2Config.nDummyHeroMPTime_Warrior = (int)seDummyHeroMPTime_Warrior.Value;   // :757
        ModValue();                                                                 // :758
    }

    /// <summary>`TFrmDummySetting.seDummyHeroMPBase_WarriorChange`（:761-765）1:1。</summary>
    public void seDummyHeroMPBase_WarriorChange(System.Windows.Forms.NumericUpDown seDummyHeroMPBase_Warrior)
    {
        M2Config.nDummyHeroMPBase_Warrior = (int)seDummyHeroMPBase_Warrior.Value;   // :763
        ModValue();                                                                 // :764
    }

    /// <summary>`TFrmDummySetting.seDummyHeroHPTime_DFChange`（:767-771）1:1。</summary>
    public void seDummyHeroHPTime_DFChange(System.Windows.Forms.NumericUpDown seDummyHeroHPTime_DF)
    {
        M2Config.nDummyHeroHPTime_DF = (int)seDummyHeroHPTime_DF.Value;         // :769
        ModValue();                                                             // :770
    }

    /// <summary>`TFrmDummySetting.seDummyHeroHPBase_DFChange`（:773-777）1:1。</summary>
    public void seDummyHeroHPBase_DFChange(System.Windows.Forms.NumericUpDown seDummyHeroHPBase_DF)
    {
        M2Config.nDummyHeroHPBase_DF = (int)seDummyHeroHPBase_DF.Value;         // :775
        ModValue();                                                             // :776
    }

    /// <summary>`TFrmDummySetting.seDummyHeroMPTime_DFChange`（:779-783）1:1。</summary>
    public void seDummyHeroMPTime_DFChange(System.Windows.Forms.NumericUpDown seDummyHeroMPTime_DF)
    {
        M2Config.nDummyHeroMPTime_DF = (int)seDummyHeroMPTime_DF.Value;         // :781
        ModValue();                                                             // :782
    }

    /// <summary>`TFrmDummySetting.seDummyHeroMPBase_DFChange`（:785-789）1:1。</summary>
    public void seDummyHeroMPBase_DFChange(System.Windows.Forms.NumericUpDown seDummyHeroMPBase_DF)
    {
        M2Config.nDummyHeroMPBase_DF = (int)seDummyHeroMPBase_DF.Value;         // :787
        ModValue();                                                             // :788
    }

    /// <summary>`TFrmDummySetting.CheckBoxDummyAutoRepairItemClick`（:791-795）1:1。</summary>
    public void CheckBoxDummyAutoRepairItemClick(System.Windows.Forms.CheckBox CheckBoxDummyAutoRepairItem)
    {
        M2Config.boDummyAutoRepairItem = CheckBoxDummyAutoRepairItem.Checked;   // :793
        ModValue();                                                             // :794
    }

    /// <summary>`TFrmDummySetting.CheckBoxDummyAutoRecallHeroClick`（:797-801）1:1。</summary>
    public void CheckBoxDummyAutoRecallHeroClick(System.Windows.Forms.CheckBox CheckBoxDummyAutoRecallHero)
    {
        M2Config.boDummyAutoRecallHero = CheckBoxDummyAutoRecallHero.Checked;   // :799
        ModValue();                                                             // :800
    }

    /// <summary>`TFrmDummySetting.EditDummyWarrorWalkTimeChange`（:803-807）1:1。</summary>
    public void EditDummyWarrorWalkTimeChange(System.Windows.Forms.NumericUpDown EditDummyWarrorWalkTime)
    {
        M2Config.dwDummyWarrorWalkTime = (int)EditDummyWarrorWalkTime.Value;    // :805
        ModValue();                                                             // :806
    }

    /// <summary>`TFrmDummySetting.EditDummyWizardWalkTimeChange`（:809-813）1:1。</summary>
    public void EditDummyWizardWalkTimeChange(System.Windows.Forms.NumericUpDown EditDummyWizardWalkTime)
    {
        M2Config.dwDummyWizardWalkTime = (int)EditDummyWizardWalkTime.Value;    // :811
        ModValue();                                                             // :812
    }

    /// <summary>`TFrmDummySetting.EditDummyTaoistWalkTimeChange`（:815-819）1:1。</summary>
    public void EditDummyTaoistWalkTimeChange(System.Windows.Forms.NumericUpDown EditDummyTaoistWalkTime)
    {
        M2Config.dwDummyTaoistWalkTime = (int)EditDummyTaoistWalkTime.Value;    // :817
        ModValue();                                                             // :818
    }

    /// <summary>`TFrmDummySetting.chkDummyRunHumClick`（:821-825）1:1。</summary>
    public void chkDummyRunHumClick(System.Windows.Forms.CheckBox chkDummyRunHum)
    {
        M2Config.boDummyRunHum = chkDummyRunHum.Checked;                        // :823
        ModValue();                                                             // :824
    }

    /// <summary>`TFrmDummySetting.chkDummyRunMonClick`（:827-831）1:1。</summary>
    public void chkDummyRunMonClick(System.Windows.Forms.CheckBox chkDummyRunMon)
    {
        M2Config.boDummyRunMon = chkDummyRunMon.Checked;                        // :829
        ModValue();                                                             // :830
    }

    /// <summary>`TFrmDummySetting.chkDummyRunNpcClick`（:833-837）1:1。</summary>
    public void chkDummyRunNpcClick(System.Windows.Forms.CheckBox chkDummyRunNpc)
    {
        M2Config.boDummyRunNpc = chkDummyRunNpc.Checked;                        // :835
        ModValue();                                                             // :836
    }

    /// <summary>`TFrmDummySetting.chkDummyRunGuardClick`（:839-843）1:1。</summary>
    public void chkDummyRunGuardClick(System.Windows.Forms.CheckBox chkDummyRunGuard)
    {
        M2Config.boDummyRunGuard = chkDummyRunGuard.Checked;                    // :841
        ModValue();                                                             // :842
    }

    /// <summary>`TFrmDummySetting.chkDummySafeAreaClick`（:845-849）1:1。</summary>
    public void chkDummySafeAreaClick(System.Windows.Forms.CheckBox chkDummySafeArea)
    {
        M2Config.boDummySafeAreaLimited = chkDummySafeArea.Checked;             // :847
        ModValue();                                                             // :848
    }

    /// <summary>
    /// `TFrmDummySetting.chkDummyWarDisHumRunClick`（:851-859）1:1。
    /// <para>
    /// ★ 原文 :856 `g_Config.boDummyWarHreoRun := chkDummyWarHreoRun.Enabled and chkDummyWarHreoRun.Checked`
    /// —— **先取 `Enabled` 再与 `Checked` 逻辑与**（不是直接取 `Checked`）。
    /// 即：当本勾选**取消**时 `chkDummyWarHreoRun.Enabled = False` ⇒ `boDummyWarHreoRun` 被**强制置 False**
    /// （即使子勾选仍是勾上的）。1:1 保留（差异断言见测试）。
    /// </para>
    /// </summary>
    public void chkDummyWarDisHumRunClick(System.Windows.Forms.CheckBox chkDummyWarDisHumRun)
    {
        System.Console.Error.WriteLine($"[DIAG-WD] enter checked={chkDummyWarDisHumRun.Checked} hreoBefore={Ct.chkDummyWarHreoRun.Checked} hreoEnBefore={Ct.chkDummyWarHreoRun.Enabled}");
        M2Config.boDummyWarDisHumRun = chkDummyWarDisHumRun.Checked;            // :853

        Ct.chkDummyWarHreoRun.Enabled = chkDummyWarDisHumRun.Checked;           // :855
        System.Console.Error.WriteLine($"[DIAG-WD] after855 hreo={Ct.chkDummyWarHreoRun.Checked} hreoEn={Ct.chkDummyWarHreoRun.Enabled}");
        Ct.chkDummyWarHreoRun.Checked = true;
        System.Console.Error.WriteLine($"[DIAG-WD] forceTrue hreo={Ct.chkDummyWarHreoRun.Checked}");
        M2Config.boDummyWarHreoRun = Ct.chkDummyWarHreoRun.Enabled && Ct.chkDummyWarHreoRun.Checked;   // :856
        System.Console.Error.WriteLine($"[DIAG-WD] exit hreo={Ct.chkDummyWarHreoRun.Checked} hreoEn={Ct.chkDummyWarHreoRun.Enabled}");

        ModValue();                                                             // :858
    }

    /// <summary>`TFrmDummySetting.chkDummyWarHreoRunClick`（:861-865）1:1。</summary>
    public void chkDummyWarHreoRunClick(System.Windows.Forms.CheckBox chkDummyWarHreoRun)
    {
        System.Console.Error.WriteLine($"[DIAG] hreoClick checked={chkDummyWarHreoRun.Checked}");
        M2Config.boDummyWarHreoRun = chkDummyWarHreoRun.Checked;                // :863
        ModValue();                                                             // :864
    }

    /// <summary>`TFrmDummySetting.chkDummySafeAreaDisNpcRunClick`（:867-871）1:1。</summary>
    public void chkDummySafeAreaDisNpcRunClick(System.Windows.Forms.CheckBox chkDummySafeAreaDisNpcRun)
    {
        M2Config.boDummySafeAreaDisNpcRun = chkDummySafeAreaDisNpcRun.Checked;  // :869
        ModValue();                                                             // :870
    }

    /// <summary>`TFrmDummySetting.chkSafeAreaDisShopStallDummyRunClick`（:873-877）1:1。</summary>
    public void chkSafeAreaDisShopStallDummyRunClick(System.Windows.Forms.CheckBox chkSafeAreaDisShopStallDummyRun)
    {
        M2Config.boSafeAreaDisShopStallDummyRun = chkSafeAreaDisShopStallDummyRun.Checked;   // :875
        ModValue();                                                                          // :876
    }

    /// <summary>`TFrmDummySetting.chkSafeAreaDisOffLineDummyRunClick`（:879-883）1:1。</summary>
    public void chkSafeAreaDisOffLineDummyRunClick(System.Windows.Forms.CheckBox chkSafeAreaDisOffLineDummyRun)
    {
        M2Config.boSafeAreaDisOffLineDummyRun = chkSafeAreaDisOffLineDummyRun.Checked;   // :881
        ModValue();                                                                      // :882
    }

    /// <summary>`TFrmDummySetting.EditDummyWizardAttackTimeChange`（:885-889）1:1。</summary>
    public void EditDummyWizardAttackTimeChange(System.Windows.Forms.NumericUpDown EditDummyWizardAttackTime)
    {
        M2Config.dwDummyWizardAttackTime = (int)EditDummyWizardAttackTime.Value;   // :887
        ModValue();                                                                // :888
    }

    /// <summary>`TFrmDummySetting.EditDummyTaoistAttackTimeChange`（:891-895）1:1。</summary>
    public void EditDummyTaoistAttackTimeChange(System.Windows.Forms.NumericUpDown EditDummyTaoistAttackTime)
    {
        M2Config.dwDummyTaoistAttackTime = (int)EditDummyTaoistAttackTime.Value;   // :893
        ModValue();                                                                // :894
    }

    /// <summary>`TFrmDummySetting.chkDummyLogonRandClick`（:897-901）1:1。</summary>
    public void chkDummyLogonRandClick(System.Windows.Forms.CheckBox chkDummyLogonRand)
    {
        M2Config.boDummyLogonRand = chkDummyLogonRand.Checked;                  // :899
        ModValue();                                                             // :900
    }

    // ========================================================================
    //  :903-911 ModValue / uModValue
    // ========================================================================

    /// <summary>
    /// `TFrmDummySetting.ModValue`（:903-906）1:1 —— 置脏：启用「保存」按钮。
    /// <para>
    /// 原文 `ButtonDummySave.Enabled := True` ⇒ 托管 `Button.Enabled = true`。
    /// 原文**不**调用 `Application.ProcessMessages`，也**不**关窗；托管侧同样不。
    /// </para>
    /// </summary>
    public void ModValue()
    {
        Ct.ButtonDummySave.Enabled = true;                                      // :905
    }

    /// <summary>
    /// `TFrmDummySetting.uModValue`（:908-911）1:1 —— 清脏：禁用「保存」按钮。
    /// </summary>
    public void uModValue()
    {
        Ct.ButtonDummySave.Enabled = false;                                     // :910
    }

    // ========================================================================
    //  :913-953 禁止移动地图列表
    // ========================================================================

    /// <summary>`TFrmDummySetting.lstDisableMoveMapClick`（:913-917）1:1。</summary>
    public void lstDisableMoveMapClick(System.Windows.Forms.ListBox lstDisableMoveMap)
    {
        if (lstDisableMoveMap.SelectedIndex >= 0)                               // :915
            Ct.btnDisableMoveMapDelete.Enabled = true;                          // :916
    }

    /// <summary>`TFrmDummySetting.btnDisableMoveMapAddClick`（:919-940）1:1。
    /// 取值/写回规则见 <see cref="AddSelectedMaps"/>（纯逻辑，单独测）。</summary>
    public void btnDisableMoveMapAddClick(System.Windows.Forms.Button btnDisableMoveMapAdd)
    {
        if (AddSelectedMaps(Ct))
        {
            FIsDummyDisableMoveMapChanged = true;                               // :937
            ModValue();                                                         // :938
        }
    }

    /// <summary>`TFrmDummySetting.btnDisableMoveMapDeleteClick`（:942-953）1:1。</summary>
    public void btnDisableMoveMapDeleteClick(System.Windows.Forms.Button btnDisableMoveMapDelete)
    {
        if (Ct.lstDisableMoveMap.SelectedIndex >= 0)                            // :944
        {
            // 原文 :946 `lstDisableMoveMap.Items.Delete(lstDisableMoveMap.ItemIndex)`
            // ⇒ 删除**当前选中行**（不是全部选中）。
            Ct.lstDisableMoveMap.Items.RemoveAt(Ct.lstDisableMoveMap.SelectedIndex);
            FIsDummyDisableMoveMapChanged = true;                               // :947
            ModValue();                                                         // :948
        }

        if (Ct.lstDisableMoveMap.SelectedIndex < 0)                             // :951
            Ct.btnDisableMoveMapDelete.Enabled = false;                         // :952
    }

    /// <summary>`TFrmDummySetting.btnDisableMoveMapAddAllClick`（:955-966）1:1。
    /// 注意原文 :960 循环取 `lstMapList.Items.Count`（**不是** `lstDisableMoveMap`），
    /// 且 :959 先 `Clear` 目标列表 ⇒ 结果是 `lstMapList` 的**全量副本**。</summary>
    public void btnDisableMoveMapAddAllClick(System.Windows.Forms.Button btnDisableMoveMapAddAll)
    {
        Ct.lstDisableMoveMap.Items.Clear();                                     // :959
        for (int I = 0; I <= Ct.lstMapList.Items.Count - 1; I++)                // :960
        {
            Ct.lstDisableMoveMap.Items.Add(Ct.lstMapList.Items[I]);             // :962
        }
        ModValue();                                                             // :964
        FIsDummyDisableMoveMapChanged = true;                                   // :965（原文 ModValue 在前）
    }

    /// <summary>`TFrmDummySetting.btnDisableMoveMapDeleteAllClick`（:968-974）1:1。</summary>
    public void btnDisableMoveMapDeleteAllClick(System.Windows.Forms.Button btnDisableMoveMapDeleteAll)
    {
        Ct.lstDisableMoveMap.Items.Clear();                                     // :970
        Ct.btnDisableMoveMapDelete.Enabled = false;                             // :971
        FIsDummyDisableMoveMapChanged = true;                                   // :972
        ModValue();                                                             // :973（原文置脏在 ModValue 之前）
    }

    /// <summary>
    /// `TFrmDummySetting.btnDisableMoveMapSaveClick`（:976-995）1:1。
    /// 与 `ButtonDummySaveClick` 的 :514-530 块**同体**（同样 `Items.Strings[I]`、
    /// 同样 `Sorted := True`），但**不置脏**而是**清脏**（:993 `FIsDummyDisableMoveMapChanged := False`）。
    /// 另：原文 :985 该行**无行尾分号**（`end` 前最后一条语句），而 :1069 同型位置**有分号** —— 原文如此。
    /// </summary>
    public void btnDisableMoveMapSaveClick(System.Windows.Forms.Button btnDisableMoveMapSave)
    {
        DummySettingState.Lock();                               // :980
        try
        {
            DummySettingState.g_DummyDisableMoveMapList.Clear();                // :982
            for (int I = 0; I <= Ct.lstDisableMoveMap.Items.Count - 1; I++)      // :983
            {
                // 原文如此（:985）：该行**无行尾分号**
                DummySettingState.g_DummyDisableMoveMapList.Add(Ct.lstDisableMoveMap.Items[I].ToString()!);
            }

            DummySettingState.g_DummyDisableMoveMapList.Sorted = true;          // :988
        }
        finally
        {
            DummySettingState.UnLock();                         // :990
        }
        DummySettingState.SaveDummyDisableMoveMap();                            // :992
        FIsDummyDisableMoveMapChanged = false;                                  // :993
    }

    // ========================================================================
    //  :997-1078 不主动攻击怪物列表
    // ========================================================================

    /// <summary>`TFrmDummySetting.lstNoAttackMonListClick`（:997-1001）1:1。</summary>
    public void lstNoAttackMonListClick(System.Windows.Forms.ListBox lstNoAttackMonList)
    {
        if (lstNoAttackMonList.SelectedIndex >= 0)                              // :999
            Ct.btnNoAttackMonDel.Enabled = true;                                // :1000
    }

    /// <summary>`TFrmDummySetting.btnNoAttackMonAddClick`（:1003-1025）1:1。
    /// 取值/写回规则见 <see cref="AddSelectedMons"/>（纯逻辑，单独测）。</summary>
    public void btnNoAttackMonAddClick(System.Windows.Forms.Button btnNoAttackMonAdd)
    {
        if (AddSelectedMons(Ct))
        {
            ModValue();                                                         // :1022（原文 ModValue 在前）
            FIsDummyNoActiveAttackMonChanged = true;                            // :1023
        }
    }

    /// <summary>`TFrmDummySetting.btnNoAttackMonDelClick`（:1027-1037）1:1。</summary>
    public void btnNoAttackMonDelClick(System.Windows.Forms.Button btnNoAttackMonDel)
    {
        if (Ct.lstNoAttackMonList.SelectedIndex >= 0)                           // :1029
        {
            Ct.lstNoAttackMonList.Items.RemoveAt(Ct.lstNoAttackMonList.SelectedIndex);   // :1031
            ModValue();                                                         // :1032（原文 ModValue 在置脏前）
            FIsDummyNoActiveAttackMonChanged = true;                            // :1033
        }
        if (Ct.lstNoAttackMonList.SelectedIndex < 0)                            // :1035
            Ct.btnNoAttackMonDel.Enabled = false;                               // :1036
    }

    /// <summary>`TFrmDummySetting.btnNoAttackMonAddAllClick`（:1039-1050）1:1。</summary>
    public void btnNoAttackMonAddAllClick(System.Windows.Forms.Button btnNoAttackMonAddAll)
    {
        Ct.lstNoAttackMonList.Items.Clear();                                    // :1043
        for (int I = 0; I <= Ct.lstMonList.Items.Count - 1; I++)                // :1044
        {
            Ct.lstNoAttackMonList.Items.Add(Ct.lstMonList.Items[I]);            // :1046
        }
        ModValue();                                                             // :1048
        FIsDummyNoActiveAttackMonChanged = true;                                // :1049
    }

    /// <summary>`TFrmDummySetting.btnNoAttackMonDelAllClick`（:1052-1058）1:1。</summary>
    public void btnNoAttackMonDelAllClick(System.Windows.Forms.Button btnNoAttackMonDelAll)
    {
        Ct.lstNoAttackMonList.Items.Clear();                                    // :1054
        Ct.btnNoAttackMonDel.Enabled = false;                                   // :1055
        ModValue();                                                             // :1056（原文 ModValue 在置脏前）
        FIsDummyNoActiveAttackMonChanged = true;                                // :1057
    }

    /// <summary>
    /// `TFrmDummySetting.btnNoAttackMonSaveClick`（:1060-1078）1:1。
    /// 与 `btnDisableMoveMapSaveClick` 同形（同样**清脏**），但此处 `:1069` 该行**有分号** —— 原文如此。
    /// </summary>
    public void btnNoAttackMonSaveClick(System.Windows.Forms.Button btnNoAttackMonSave)
    {
        DummySettingState.Lock();                               // :1064
        try
        {
            DummySettingState.g_DummyNoActiveAttackMonList.Clear();             // :1066
            for (int I = 0; I <= Ct.lstNoAttackMonList.Items.Count - 1; I++)     // :1067
            {
                DummySettingState.g_DummyNoActiveAttackMonList.Add(Ct.lstNoAttackMonList.Items[I].ToString()!);   // :1069（有分号）
            }

            DummySettingState.g_DummyNoActiveAttackMonList.Sorted = true;       // :1072
        }
        finally
        {
            DummySettingState.UnLock();                         // :1074
        }
        DummySettingState.SaveDummyNoActiveAttackMonList();                     // :1076
        FIsDummyNoActiveAttackMonChanged = false;                               // :1077
    }
}
