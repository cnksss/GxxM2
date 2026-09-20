// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmMainGamePets.pas（1,353 行，GBK）
//  类型：TFrmGamePets 事件处理器族（:392-1350）与 DoOpen（:241-390）
//  本文件按原文出现顺序逐方法 1:1 移植；方法名、字段名、分支顺序照抄。
//  原文笔误/冗余保留并注释 `// 原文如此（<文件>:<行>）`。
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.GamePets;

public sealed partial class GamePetsForm
{
    /// <summary>弹窗接缝转调：优先本窗体注入，否则走既有 `M2Forms.MessageBox`（测试可注入不弹窗）。</summary>
    private int ShowMessage(string text, string caption, int flags)
        => MessageBoxHandler != null
            ? MessageBoxHandler(text, caption, flags)
            : M2Forms.MessageBox(text, caption, flags);

    private static int Idx(System.Windows.Forms.ComboBox cb) => cb.SelectedIndex;

    // ========================================================================
    //  :241-390 DoOpen
    // ========================================================================

    /// <summary>
    /// `TFrmGamePets.DoOpen`（:241-390）1:1。
    /// </summary>
    /// <param name="showModal">Delphi 尾部 `ShowModal`（:389）；无头测试传 false。</param>
    public void DoOpen(bool showModal = true)
    {
        int I;
        boOpened = false;

        // {$IF MULTI_THREAD = 1}（:248-266）：原文按编译开关包裹加锁
        if (g_MultiThreadRun)
            MonsterListLockR?.Invoke(11);                       // :250 LockR(11)
        try
        {
            var monsters = MonsterListHandler?.Invoke() ?? (IReadOnlyList<(string sName, byte btRace)>)Array.Empty<(string, byte)>();
            for (I = 0; I <= monsters.Count - 1; I++)
            {
                (string sName, byte btRace) MonInfo = monsters[I];

                // :257-258：btRace >= RC_ANIMAL 且**不**属于这 7 个种族/值
                if ((MonInfo.btRace >= Grobal2Const.RC_ANIMAL) && !(MonInfo.btRace == Grobal2Const.RC_PLAYMOSTER     // 人形怪
                    || MonInfo.btRace == Grobal2Const.RC_ARCHERGUARD                                                  // 弓箭手
                    || MonInfo.btRace == Grobal2Const.RC_MOVE_ARCHERGUARD                                             // 巡回弓箭手
                    || MonInfo.btRace == Grobal2Const.RC_TRUCKOBJECT                                                  // 押镖车
                    || MonInfo.btRace == 55                                                                           // 练功师
                    || MonInfo.btRace == 110
                    || MonInfo.btRace == 111))                                                                        // 沙巴克城墙
                    lstMonsterList.Items.Add(MonInfo.sName);
            }
        }
        finally
        {
            if (g_MultiThreadRun)
                MonsterListUnLockR?.Invoke();                   // :264 UnLockR
        }

        // :269-270 原文在此**显式清空**两个下拉（重复 DoOpen 不累加）
        cbbPetShowFile1.Items.Clear();
        cbbPetShowFile2.Items.Clear();
        cbbPetShowFile1.Items.Add("根据Appr计算");                // :271
        cbbPetShowFile2.Items.Add("根据Appr计算");                // :272
        var imgs = EffectImageListHandler?.Invoke() ?? (IReadOnlyList<string>)Array.Empty<string>();
        for (I = 0; I <= imgs.Count - 1; I++)                   // :273
        {
            cbbPetShowFile1.Items.Add(imgs[I]);                 // :275
            cbbPetShowFile2.Items.Add(imgs[I]);                 // :276
        }

        // :279-297 19 项经验计划（顺序与 TLevelExpScheme 枚举值一致）
        cbbLevelExp.Items.Clear();
        cbbLevelExp.Items.Add("原始经验值");
        cbbLevelExp.Items.Add("标准经验值");
        cbbLevelExp.Items.Add("当前1/2倍经验");
        cbbLevelExp.Items.Add("当前1/5倍经验");
        cbbLevelExp.Items.Add("当前1/8倍经验");
        cbbLevelExp.Items.Add("当前1/10倍经验");
        cbbLevelExp.Items.Add("当前1/20倍经验");
        cbbLevelExp.Items.Add("当前1/30倍经验");
        cbbLevelExp.Items.Add("当前1/40倍经验");
        cbbLevelExp.Items.Add("当前1/50倍经验");
        cbbLevelExp.Items.Add("当前1/60倍经验");
        cbbLevelExp.Items.Add("当前1/70倍经验");
        cbbLevelExp.Items.Add("当前1/80倍经验");
        cbbLevelExp.Items.Add("当前1/90倍经验");
        cbbLevelExp.Items.Add("当前1/100倍经验");
        cbbLevelExp.Items.Add("当前1/150倍经验");
        cbbLevelExp.Items.Add("当前1/200倍经验");
        cbbLevelExp.Items.Add("当前1/250倍经验");
        cbbLevelExp.Items.Add("当前1/300倍经验");

        // :299-302 GridLevelExp 列宽 + 表头
        // 原文 GridLevelExp 行数由 DFM 静态给定（1 表头 + 1000 级）；重复 DoOpen 不会再加表头。
        GridLevelExp.Columns[0].Width = 50;
        GridLevelExp.Columns[1].Width = 200;
        if (GridLevelExp.Rows.Count == 0)
            GridLevelExp.Rows.Add("等级", "经验值");              // Cells[0,0]='等级'; Cells[1,0]='经验值'
        else
        {
            GridLevelExp.Rows[0].Cells[0].Value = "等级";
            GridLevelExp.Rows[0].Cells[1].Value = "经验值";
        }

        // :304-308 逐行回填等级与经验（行 I 对应 dwPetNeedExps[I]，1-based）
        EnsureGridRows(MAXCHANGELEVEL);
        for (I = 1; I <= GridLevelExp.RowCount - 1; I++)
        {
            GridLevelExp.Rows[I].Cells[0].Value = GXX.Core.Rtl.DelphiRTL.IntToStr(I);
            GridLevelExp.Rows[I].Cells[1].Value = GXX.Core.Rtl.DelphiRTL.IntToStr(M2Config.dwPetNeedExps[I]);
        }

        sePetHighLevel.Value = M2Config.nPetHighLevel;                                   // :310
        sePetHighLevelGetExp.Value = M2Config.nPetHighLevelGetExp;                       // :311

        chkPetFixExp.Checked = M2Config.boPetUseFixExp;                                  // :313
        sePetBaseExp.Value = M2Config.nPetBaseExp;                                       // :314
        sePetAddExp.Value = M2Config.nPetAddExp;                                         // :315

        RefreshGamePetConfigList();                                                      // :317

        if (lstGamePets.Items.Count > 0)                                                 // :319-323
        {
            lstGamePets.SelectedIndex = 0;
            lstGamePetsClick(lstGamePets);                                               // :322 显式触发 OnClick
        }

        chkOpenGamePet.Checked = M2Config.boOpenGamePet;                                 // :325
        chkEnabledPetAttack.Checked = M2Config.boEnabledPetAttack;                       // :326
        chkDisableMonAttackPet.Checked = M2Config.boDisableMonAttackPet;                 // :327
        chkDisableAllAttackPet.Checked = M2Config.boDisableAllAttackPet;                 // :328
        chkEnabledPetPickup.Checked = M2Config.boEnabledPetPickup;                       // :329
        chkPetOnlyPickMonsterItem.Checked = M2Config.boPetOnlyPickMonsterItem;           // :330
        chkPetPickupToMaster.Checked = M2Config.boPetPickupToMaster;                     // :331
        chkPetPickupFullToMaster.Checked = M2Config.boPetPickupFullToMaster;             // :332
        chkPetPickupFullToMaster.Enabled = !M2Config.boPetPickupToMaster;                // :333
        chkPetQuickPickup.Checked = M2Config.boPetQuickPickup;                           // :334
        chkPetRangePickup.Checked = M2Config.boPetRangePickup;                           // :335
        sePetPickupRange.Value = M2Config.btPetPickupRange;                              // :336

        chkPetNoEntity.Checked = M2Config.boPetNoEntity;                                 // :338
        chkPetSleepControlBySlave.Checked = M2Config.boPetSleepControlBySlave;           // :339
        chkPetNoShowHPProgress.Checked = M2Config.boPetNoShowHPProgress;                 // :340

        // {$IF NEED_KEY = 1}（:345-356）：VMProtect 键控分支
        // 决策抽成纯函数（见 ShouldShowClientPickItems），便于无头断言。
        ApplyClientPickItemsVisibility();                                                // :346-353

        sePetAbilToMasterRate.Value = M2Config.nPetAbilToMasterRate;                     // :361
        chkPetHPToMaster.Checked = M2Config.boPetHPToMaster;                             // :362
        chkPetDCToMaster.Checked = M2Config.boPetDCToMaster;                             // :363 原文如此（:363 实为 MC，见报告）
        chkPetMCToMaster.Checked = M2Config.boPetMCToMaster;                             // :364
        chkPetSCToMaster.Checked = M2Config.boPetSCToMaster;                             // :365
        chkPetACToMaster.Checked = M2Config.boPetACToMaster;                             // :366
        chkPetMACToMaster.Checked = M2Config.boPetMACToMaster;                           // :367

        sePetUseItemIntervalTime.Value = M2Config.dwPetUseItemIntervalTime;              // :369
        chkCapturePetNeedItem.Checked = M2Config.boCapturePetNeedItem;                   // :370
        chkCaptureOKDecDura.Checked = M2Config.boCaptureOKDecDura;                       // :371

        chkPetShowMasterName.Checked = M2Config.boPetShowMasterName;                     // :373
        sePetNameColor.Value = M2Config.btPetNameColor;                                  // :374
        edtPetSuffixName.Text = M2Config.sPetSuffixName;                                 // :375

        seGamePetMaxCount.Value = M2Config.nGamePetMaxCount;                             // :377
        seGamePetNameCount.Value = M2Config.nGamePetNameCount;                           // :378
        seGamePetRecallTime.Value = M2Config.nGamePetRecallTime;                         // :379

        chkGamePetKillMonTrigger.Checked = M2Config.boGamePetKillMonTrigger;             // :381

        pgcMain.SelectedIndex = 0;                                                       // :383

        btnSavePet.Enabled = false;                                                      // :385
        btnSavePetParams.Enabled = false;                                                // :386

        boOpened = true;                                                                 // :388
        if (showModal)
            ShowDialog();                                                                // :389 ShowModal
    }

    /// <summary>
    /// :345-356 `{$IF NEED_KEY = 1}` 的 VMProtect 键控决策，抽成**纯函数**。
    /// 返回 true = 勾选并保持可见（key == 1）；false = 不勾选并 `Visible := False`。
    /// </summary>
    /// <remarks>
    /// ⚠ 无头限制：WinForms 的 `Control.Visible` 是**计算值**（受父 TabPage 是否被真正显示影响），
    /// 无头环境下恒为 false，无法用它断言。故把"该分支取哪条"抽成本函数单独测，
    /// 窗体侧只做 `Visibility = 本决策` 的机械赋值。
    /// </remarks>
    public static bool ShouldShowClientPickItems(int key) => key == 1;

    /// <summary>`ApplyClientPickItemsVisibility`（:346-353）1:1。</summary>
    private void ApplyClientPickItemsVisibility()
    {
        if (ShouldShowClientPickItems(g_nKey_UseClientPickItems))
        {
            chkEnablePetUseClientPickItems.Checked = M2Config.boEnablePetUseClientPickItems; // :348
        }
        else
        {
            chkEnablePetUseClientPickItems.Visible = false;                              // :352
        }
    }

    /// <summary>
    /// 把 `GridLevelExp` 补足到 `RowCount == 1 + MAXCHANGELEVEL`（原文 DFM 里已静态设好 1001 行）。
    /// WinForms DataGridView 无静态行，故按需补齐；只增不减，与原文固定行数等价。
    /// </summary>
    private void EnsureGridRows(int total)
    {
        while (GridLevelExp.Rows.Count < 1 + total)
            GridLevelExp.Rows.Add();
    }

    // ========================================================================
    //  :392-457 lstGamePetsClick
    // ========================================================================

    /// <summary>`TFrmGamePets.lstGamePetsClick`（:392-457）1:1：回填选中宠物配置到右侧控件。</summary>
    private void lstGamePetsClick(object Sender)
    {
        if (lstGamePets.Items.Count > 0)
        {
            btnEditPet.Enabled = true;                                          // :396
            btnDelPet.Enabled = true;                                           // :397

            SelGamePetConfig = GamePetsState.g_GamePetConfigList[lstGamePets.SelectedIndex]; // :399

            edtPetName.Text = SelGamePetConfig.Name;                            // :401
            sePetCaptureRate.Value = SelGamePetConfig.CaptureRate;              // :402

            chkLevelDifference.Checked = SelGamePetConfig.EnabledLevelDifference; // :404
            seLevelDifference.Value = SelGamePetConfig.LevelDifference;        // :405
            seHPScale.Value = SelGamePetConfig.HPScale;                         // :406

            cbbPetShowFile1.SelectedIndex = SelGamePetConfig.ShowFile1;         // :408
            sePetShowStart1.Value = SelGamePetConfig.ShowStart1;                // :409
            sePetShowCount1.Value = SelGamePetConfig.ShowCount1;                // :410
            sePetShowTime1.Value = SelGamePetConfig.ShowTime1;                  // :411
            sePetShowOffsetX1.Value = SelGamePetConfig.ShowOffsetX1;            // :412
            sePetShowOffsetY1.Value = SelGamePetConfig.ShowOffsetY1;            // :413

            cbbPetShowFile2.SelectedIndex = SelGamePetConfig.ShowFile2;         // :415
            sePetShowStart2.Value = SelGamePetConfig.ShowStart2;                // :416
            sePetShowCount2.Value = SelGamePetConfig.ShowCount2;                // :417
            sePetShowTime2.Value = SelGamePetConfig.ShowTime2;                  // :418
            sePetShowOffsetX2.Value = SelGamePetConfig.ShowOffsetX2;            // :419
            sePetShowOffsetY2.Value = SelGamePetConfig.ShowOffsetY2;            // :420

            sePetAddHP.Value = SelGamePetConfig.AddHP;                          // :422
            cbbPetAddHPType.SelectedIndex = SelGamePetConfig.IsAddHPRate ? 1 : 0; // :423 Integer(Boolean)

            sePetAddDC1.Value = SelGamePetConfig.AddDC1;                        // :425
            cbbPetAddDCType1.SelectedIndex = SelGamePetConfig.IsAddDC1Rate ? 1 : 0; // :426
            sePetAddDC2.Value = SelGamePetConfig.AddDC2;                        // :427
            cbbPetAddDCType2.SelectedIndex = SelGamePetConfig.IsAddDC2Rate ? 1 : 0; // :428

            sePetAddMC1.Value = SelGamePetConfig.AddMC1;                        // :430
            cbbPetAddMCType1.SelectedIndex = SelGamePetConfig.IsAddMC1Rate ? 1 : 0; // :431
            sePetAddMC2.Value = SelGamePetConfig.AddMC2;                        // :432
            cbbPetAddMCType2.SelectedIndex = SelGamePetConfig.IsAddMC2Rate ? 1 : 0; // :433

            sePetAddSC1.Value = SelGamePetConfig.AddSC1;                        // :435
            cbbPetAddSCType1.SelectedIndex = SelGamePetConfig.IsAddSC1Rate ? 1 : 0; // :436
            sePetAddSC2.Value = SelGamePetConfig.AddSC2;                        // :437
            cbbPetAddSCType2.SelectedIndex = SelGamePetConfig.IsAddSC2Rate ? 1 : 0; // :438

            sePetAddAC1.Value = SelGamePetConfig.AddAC1;                        // :440
            cbbPetAddACType1.SelectedIndex = SelGamePetConfig.IsAddAC1Rate ? 1 : 0; // :441
            sePetAddAC2.Value = SelGamePetConfig.AddAC2;                        // :442
            cbbPetAddACType2.SelectedIndex = SelGamePetConfig.IsAddAC2Rate ? 1 : 0; // :443

            sePetAddMAC1.Value = SelGamePetConfig.AddMAC1;                      // :445
            cbbPetAddMACType1.SelectedIndex = SelGamePetConfig.IsAddMAC1Rate ? 1 : 0; // :446
            sePetAddMAC2.Value = SelGamePetConfig.AddMAC2;                      // :447
            cbbPetAddMACType2.SelectedIndex = SelGamePetConfig.IsAddMAC2Rate ? 1 : 0; // :448
        }
        else
        {
            btnEditPet.Enabled = false;                                         // :452
            btnDelPet.Enabled = false;                                          // :453

            SelGamePetConfig = null;                                            // :455
        }
    }

    // ========================================================================
    //  :459-519 btnEditPetClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnEditPetClick`（:459-519）1:1。</summary>
    private void btnEditPetClick(object Sender)
    {
        if (SelGamePetConfig == null)                                           // :461
        {
            ShowMessage("请选择一个需要修改的怪物！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR); // :463
            SetFocusProbe?.Invoke("lstGamePets");                               // :464 lstGamePets.SetFocus
            return;                                                             // :465 Exit
        }

        SelGamePetConfig.Name = edtPetName.Text;                                // :468
        SelGamePetConfig.CaptureRate = (int)sePetCaptureRate.Value;             // :469

        SelGamePetConfig.EnabledLevelDifference = chkLevelDifference.Checked;   // :471
        SelGamePetConfig.LevelDifference = (int)seLevelDifference.Value;        // :472
        SelGamePetConfig.HPScale = (int)seHPScale.Value;                        // :473

        SelGamePetConfig.ShowFile1 = Idx(cbbPetShowFile1);                      // :475
        SelGamePetConfig.ShowStart1 = (int)sePetShowStart1.Value;               // :476
        SelGamePetConfig.ShowCount1 = (int)sePetShowCount1.Value;               // :477
        SelGamePetConfig.ShowTime1 = (int)sePetShowTime1.Value;                 // :478
        SelGamePetConfig.ShowOffsetX1 = (int)sePetShowOffsetX1.Value;           // :479
        SelGamePetConfig.ShowOffsetY1 = (int)sePetShowOffsetY1.Value;           // :480

        SelGamePetConfig.ShowFile2 = Idx(cbbPetShowFile2);                      // :482
        SelGamePetConfig.ShowStart2 = (int)sePetShowStart2.Value;               // :483
        SelGamePetConfig.ShowCount2 = (int)sePetShowCount2.Value;               // :484
        SelGamePetConfig.ShowTime2 = (int)sePetShowTime2.Value;                 // :485
        SelGamePetConfig.ShowOffsetX2 = (int)sePetShowOffsetX2.Value;           // :486
        SelGamePetConfig.ShowOffsetY2 = (int)sePetShowOffsetY2.Value;           // :487

        SelGamePetConfig.AddHP = (int)sePetAddHP.Value;                         // :489
        SelGamePetConfig.IsAddHPRate = Idx(cbbPetAddHPType) == 1;               // :490

        SelGamePetConfig.AddDC1 = (int)sePetAddDC1.Value;                       // :492
        SelGamePetConfig.IsAddDC1Rate = Idx(cbbPetAddDCType1) == 1;             // :493
        SelGamePetConfig.AddDC2 = (int)sePetAddDC2.Value;                       // :494
        SelGamePetConfig.IsAddDC2Rate = Idx(cbbPetAddDCType2) == 1;             // :495

        SelGamePetConfig.AddMC1 = (int)sePetAddMC1.Value;                       // :497
        SelGamePetConfig.IsAddMC1Rate = Idx(cbbPetAddMCType1) == 1;             // :498
        SelGamePetConfig.AddMC2 = (int)sePetAddMC2.Value;                       // :499
        SelGamePetConfig.IsAddMC2Rate = Idx(cbbPetAddMCType2) == 1;             // :500

        SelGamePetConfig.AddSC1 = (int)sePetAddSC1.Value;                       // :502
        SelGamePetConfig.IsAddSC1Rate = Idx(cbbPetAddSCType1) == 1;             // :503
        SelGamePetConfig.AddSC2 = (int)sePetAddSC2.Value;                       // :504
        SelGamePetConfig.IsAddSC2Rate = Idx(cbbPetAddSCType2) == 1;             // :505

        SelGamePetConfig.AddAC1 = (int)sePetAddAC1.Value;                       // :507
        SelGamePetConfig.IsAddAC1Rate = Idx(cbbPetAddACType1) == 1;             // :508
        SelGamePetConfig.AddAC2 = (int)sePetAddAC2.Value;                       // :509
        SelGamePetConfig.IsAddAC2Rate = Idx(cbbPetAddACType2) == 1;             // :510

        SelGamePetConfig.AddMAC1 = (int)sePetAddMAC1.Value;                     // :512
        SelGamePetConfig.IsAddMAC1Rate = Idx(cbbPetAddMACType1) == 1;           // :513
        SelGamePetConfig.AddMAC2 = (int)sePetAddMAC2.Value;                     // :514
        SelGamePetConfig.IsAddMAC2Rate = Idx(cbbPetAddMACType2) == 1;           // :515

        RefreshGamePetConfigList();                                             // :517
        btnSavePet.Enabled = true;                                              // :518
    }

    // ========================================================================
    //  :521-532 RefreshGamePetConfigList（见 GamePetsForm.cs）

    // ========================================================================
    //  :534-612 btnAddPetClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnAddPetClick`（:534-612）1:1。</summary>
    private void btnAddPetClick(object Sender)
    {
        TGamePetConfig GamePetConfig;
        string S = edtPetName.Text.Trim();                                      // :539 Trim

        if (S.Length == 0)                                                      // :540 Length(S) = 0
        {
            ShowMessage("请输入怪物名称！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR); // :542
            SetFocusProbe?.Invoke("edtPetName");                                // :543 edtPetName.SetFocus
            return;                                                             // :544 Exit
        }

        if (lstGamePets.Items.IndexOf(S) >= 0)                                  // :547 IndexOf
        {
            // 原文 PChar('怪物 ' + S + ' 已经存在！')（:549）
            ShowMessage("怪物 " + S + " 已经存在！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            SetFocusProbe?.Invoke("edtPetName");                                // :550 edtPetName.SetFocus
            return;                                                             // :551 Exit
        }

        GamePetConfig = new TGamePetConfig();                                   // :554 New(GamePetConfig)
        GamePetsState.g_GamePetConfigList.Add(GamePetConfig);                   // :555

        GamePetConfig.Name = edtPetName.Text;                                   // :557 原文如此：用 .Text 而非 Trim 后的 S
        GamePetConfig.CaptureRate = (int)sePetCaptureRate.Value;                // :558

        GamePetConfig.EnabledLevelDifference = chkLevelDifference.Checked;      // :560
        GamePetConfig.LevelDifference = (int)seLevelDifference.Value;           // :561
        GamePetConfig.HPScale = (int)seHPScale.Value;                           // :562

        GamePetConfig.ShowFile1 = Idx(cbbPetShowFile1);                         // :564
        GamePetConfig.ShowStart1 = (int)sePetShowStart1.Value;                  // :565
        GamePetConfig.ShowCount1 = (int)sePetShowCount1.Value;                  // :566
        GamePetConfig.ShowTime1 = (int)sePetShowTime1.Value;                    // :567
        GamePetConfig.ShowOffsetX1 = (int)sePetShowOffsetX1.Value;              // :568
        GamePetConfig.ShowOffsetY1 = (int)sePetShowOffsetY1.Value;              // :569

        GamePetConfig.ShowFile2 = Idx(cbbPetShowFile2);                         // :571
        GamePetConfig.ShowStart2 = (int)sePetShowStart2.Value;                  // :572
        GamePetConfig.ShowCount2 = (int)sePetShowCount2.Value;                  // :573
        GamePetConfig.ShowTime2 = (int)sePetShowTime2.Value;                    // :574
        GamePetConfig.ShowOffsetX2 = (int)sePetShowOffsetX2.Value;              // :575
        GamePetConfig.ShowOffsetY2 = (int)sePetShowOffsetY2.Value;              // :576

        GamePetConfig.AddHP = (int)sePetAddHP.Value;                            // :578
        GamePetConfig.IsAddHPRate = Idx(cbbPetAddHPType) == 1;                  // :579

        GamePetConfig.AddDC1 = (int)sePetAddDC1.Value;                          // :581
        GamePetConfig.IsAddDC1Rate = Idx(cbbPetAddDCType1) == 1;                // :582
        GamePetConfig.AddDC2 = (int)sePetAddDC2.Value;                          // :583
        GamePetConfig.IsAddDC2Rate = Idx(cbbPetAddDCType2) == 1;                // :584

        GamePetConfig.AddMC1 = (int)sePetAddMC1.Value;                          // :586
        GamePetConfig.IsAddMC1Rate = Idx(cbbPetAddMCType1) == 1;                // :587
        GamePetConfig.AddMC2 = (int)sePetAddMC2.Value;                          // :588
        GamePetConfig.IsAddMC2Rate = Idx(cbbPetAddMCType2) == 1;                // :589

        GamePetConfig.AddSC1 = (int)sePetAddSC1.Value;                          // :591
        GamePetConfig.IsAddSC1Rate = Idx(cbbPetAddSCType1) == 1;                // :592
        GamePetConfig.AddSC2 = (int)sePetAddSC2.Value;                          // :593
        GamePetConfig.IsAddSC2Rate = Idx(cbbPetAddSCType2) == 1;                // :594

        GamePetConfig.AddAC1 = (int)sePetAddAC1.Value;                          // :596
        GamePetConfig.IsAddAC1Rate = Idx(cbbPetAddACType1) == 1;                // :597
        GamePetConfig.AddAC2 = (int)sePetAddAC2.Value;                          // :598
        GamePetConfig.IsAddAC2Rate = Idx(cbbPetAddACType2) == 1;                // :599

        GamePetConfig.AddMAC1 = (int)sePetAddMAC1.Value;                        // :601
        GamePetConfig.IsAddMAC1Rate = Idx(cbbPetAddMACType1) == 1;              // :602
        GamePetConfig.AddMAC2 = (int)sePetAddMAC2.Value;                        // :603
        GamePetConfig.IsAddMAC2Rate = Idx(cbbPetAddMACType2) == 1;              // :604

        RefreshGamePetConfigList();                                             // :606

        lstGamePets.SelectedIndex = lstGamePets.Items.Count - 1;                // :608
        SelGamePetConfig = GamePetConfig;                                       // :609

        btnSavePet.Enabled = true;                                              // :611
    }

    // ========================================================================
    //  :614-648 btnDelPetClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnDelPetClick`（:614-648）1:1。</summary>
    private void btnDelPetClick(object Sender)
    {
        bool IsOK;

        if (SelGamePetConfig == null)                                           // :619
        {
            // 原文如此：提示文本是"物品"而非"怪物"（:621）
            ShowMessage("请选择一个需要修改的物品！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            SetFocusProbe?.Invoke("lstGamePets");                               // :622 lstGamePets.SetFocus
            return;                                                             // :623 Exit
        }

        IsOK = false;                                                           // :626
        for (int I = 0; I <= GamePetsState.g_GamePetConfigList.Count - 1; I++)
        {
            if (ReferenceEquals(SelGamePetConfig, GamePetsState.g_GamePetConfigList[I])) // :629 指针相等
            {
                // 原文 Dispose(SelGamePetConfig)（:631）；托管侧引用类型交给 GC
                GamePetsState.g_GamePetConfigList.RemoveAt(I);                  // :632 Delete(I)

                IsOK = true;                                                    // :634
                break;                                                          // :635
            }
        }

        if (!IsOK)                                                              // :639
        {
            ShowMessage("删除失败！", "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR); // :641
        }
        else
        {
            SelGamePetConfig = null;                                            // :645
            RefreshGamePetConfigList();                                         // :646
        }
    }

    // ========================================================================
    //  :650-654 btnSavePetClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnSavePetClick`（:650-654）1:1。</summary>
    private void btnSavePetClick(object Sender)
    {
        GamePetsState.SaveGamePetsConfig();                                     // :652
        btnSavePet.Enabled = false;                                             // :653
    }

    // ========================================================================
    //  :656-662 lstMonsterListDblClick
    // ========================================================================

    /// <summary>`TFrmGamePets.lstMonsterListDblClick`（:656-662）1:1。</summary>
    private void lstMonsterListDblClick(object Sender)
    {
        if (lstMonsterList.SelectedIndex >= 0)                                  // :658
        {
            edtPetName.Text = (string)lstMonsterList.Items[lstMonsterList.SelectedIndex]; // :660
        }
    }

    // ========================================================================
    //  :664-669 ModValue
    // ========================================================================

    /// <summary>`TFrmGamePets.ModValue`（:664-669）1:1：置脏并放开两个保存按钮。</summary>
    private void ModValue()
    {
        boModValued = true;                                                     // :666
        btnSavePet.Enabled = true;                                              // :667
        btnSavePetParams.Enabled = true;                                        // :668
    }

    // ========================================================================
    //  :671-698 lstMonsterListKeyDown
    // ========================================================================

    /// <summary>
    /// `TFrmGamePets.lstMonsterListKeyDown`（:671-698）1:1。
    /// 原文 `case Key of Word('F'): ...` —— **只有** Ctrl+F 一个分支（无 else）。
    /// </summary>
    /// <param name="Key">Delphi `var Key: Word`；置 0 = 吞掉按键。</param>
    private void lstMonsterListKeyDown(object Sender, ushort Key, bool ssCtrl)
    {
        // 原文 Word('F') = 70
        if (Key != 70)
            return;

        if (ssCtrl)                                                             // :679 ssCtrl in Shift
        {
            Key = 0;                                                            // :681 原文是 var 参数（托管侧仅局部置 0）
            string sMonName = "";                                               // :682
            var iq = InputQueryHandler?.Invoke("怪物查找", "输入怪物名称:", sMonName)
                     ?? (Ok: false, Value: sMonName);                           // :683 InputQuery
            if (!iq.Ok)
                return;                                                         // :684 Exit
            sMonName = iq.Value;
            if (sMonName == "")                                                 // :685
                return;                                                         // :686 Exit
            for (int I = 0; I <= lstMonsterList.Items.Count - 1; I++)
            {
                if ((string)lstMonsterList.Items[I] == sMonName)                // :689
                {
                    lstMonsterList.SelectedIndex = I;                           // :691
                    break;                                                      // :692
                }
            }
        }
    }

    // ========================================================================
    //  :700-772 纯布尔开关处理器族（统一形状：boOpened 早退 → 写 g_Config → ModValue）
    // ========================================================================

    /// <summary>`chkOpenGamePetClick`（:700-706）。</summary>
    private void chkOpenGamePetClick(object Sender)
    {
        if (!boOpened) return;                                                  // :702-703
        M2Config.boOpenGamePet = chkOpenGamePet.Checked;                        // :704
        ModValue();                                                             // :705
    }

    /// <summary>`chkPetNoEntityClick`（:708-714）。</summary>
    private void chkPetNoEntityClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetNoEntity = chkPetNoEntity.Checked;
        ModValue();
    }

    /// <summary>`chkPetNoShowHPProgressClick`（:716-722）。</summary>
    private void chkPetNoShowHPProgressClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetNoShowHPProgress = chkPetNoShowHPProgress.Checked;
        ModValue();
    }

    /// <summary>`chkPetSleepControlBySlaveClick`（:724-730）。</summary>
    private void chkPetSleepControlBySlaveClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetSleepControlBySlave = chkPetSleepControlBySlave.Checked;
        ModValue();
    }

    /// <summary>`chkEnabledPetAttackClick`（:732-738）。</summary>
    private void chkEnabledPetAttackClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boEnabledPetAttack = chkEnabledPetAttack.Checked;
        ModValue();
    }

    /// <summary>`chkEnabledPetPickupClick`（:740-746）。</summary>
    private void chkEnabledPetPickupClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boEnabledPetPickup = chkEnabledPetPickup.Checked;
        ModValue();
    }

    /// <summary>`chkPetOnlyPickMonsterItemClick`（:748-754）。</summary>
    private void chkPetOnlyPickMonsterItemClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetOnlyPickMonsterItem = chkPetOnlyPickMonsterItem.Checked;
        ModValue();
    }

    /// <summary>`chkPetPickupToMasterClick`（:756-764）：额外联动 `chkPetPickupFullToMaster.Enabled`。</summary>
    private void chkPetPickupToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetPickupToMaster = chkPetPickupToMaster.Checked;            // :760
        ModValue();                                                             // :761

        chkPetPickupFullToMaster.Enabled = !M2Config.boPetPickupToMaster;       // :763
    }

    /// <summary>`chkPetPickupFullToMasterClick`（:766-772）。</summary>
    private void chkPetPickupFullToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetPickupFullToMaster = chkPetPickupFullToMaster.Checked;
        ModValue();
    }

    // ========================================================================
    //  :774-828 叠加给主人开关/Spin 族
    // ========================================================================

    /// <summary>`sePetAbilToMasterRateChange`（:774-780）。</summary>
    private void sePetAbilToMasterRateChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nPetAbilToMasterRate = (int)sePetAbilToMasterRate.Value;       // :778
        ModValue();
    }

    /// <summary>`chkPetHPToMasterClick`（:782-788）。</summary>
    private void chkPetHPToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetHPToMaster = chkPetHPToMaster.Checked;
        ModValue();
    }

    /// <summary>`chkPetDCToMasterClick`（:790-796）。</summary>
    private void chkPetDCToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetDCToMaster = chkPetDCToMaster.Checked;
        ModValue();
    }

    /// <summary>`chkPetMCToMasterClick`（:798-804）。</summary>
    private void chkPetMCToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetMCToMaster = chkPetMCToMaster.Checked;
        ModValue();
    }

    /// <summary>`chkPetSCToMasterClick`（:806-812）。</summary>
    private void chkPetSCToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetSCToMaster = chkPetSCToMaster.Checked;
        ModValue();
    }

    /// <summary>`chkPetACToMasterClick`（:814-820）。</summary>
    private void chkPetACToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetACToMaster = chkPetACToMaster.Checked;
        ModValue();
    }

    /// <summary>`chkPetMACToMasterClick`（:822-828）。</summary>
    private void chkPetMACToMasterClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetMACToMaster = chkPetMACToMaster.Checked;
        ModValue();
    }

    // ========================================================================
    //  :830-912 btnSavePetParamsClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnSavePetParamsClick`（:830-912）1:1：30 个 INI 键写入 + 下发。</summary>
    private void btnSavePetParamsClick(object Sender)
    {
        // 开启宠物系统
        WriteBool("OpenGamePet", M2Config.boOpenGamePet);                       // :833

        // 允许宠物攻击
        WriteBool("EnabledPetAttack", M2Config.boEnabledPetAttack);             // :836

        WriteBool("DisableMonAttackPet", M2Config.boDisableMonAttackPet);       // :838
        WriteBool("DisableAllAttackPet", M2Config.boDisableAllAttackPet);       // :839

        // 允许宠物捡物
        WriteBool("EnabledPetPickup", M2Config.boEnabledPetPickup);             // :842

        WriteBool("PetOnlyPickMonsterItem", M2Config.boPetOnlyPickMonsterItem); // :844

        // 宠物直接捡物到主人背包
        WriteBool("PetPickupToMaster", M2Config.boPetPickupToMaster);           // :847

        // 宠物包满时捡到物品放主人包裹
        WriteBool("PetPickupFullToMaster", M2Config.boPetPickupFullToMaster);   // :850

        WriteBool("PetQuickPickup", M2Config.boPetQuickPickup);                 // :852

        WriteBool("PetRangePickup", M2Config.boPetRangePickup);                 // :854
        WriteInteger("PetPickupRange", M2Config.btPetPickupRange);              // :855

        // 宝宝无实体
        WriteBool("PetNoEntity", M2Config.boPetNoEntity);                       // :858

        WriteBool("PetNoShowHPProgress", M2Config.boPetNoShowHPProgress);       // :860

        // 宠物休息受宝宝控制
        WriteBool("PetSleepControlBySlave", M2Config.boPetSleepControlBySlave); // :863

        // 宠物叠加属性给主人倍率
        WriteInteger("PetAbilToMasterRate", M2Config.nPetAbilToMasterRate);     // :866

        // 叠加HP给主人
        WriteBool("PetHPToMaster", M2Config.boPetHPToMaster);                   // :869

        // 叠加攻击给主人
        WriteBool("PetDCToMaster", M2Config.boPetDCToMaster);                   // :872

        // 叠加魔法给主人
        WriteBool("PetMCToMaster", M2Config.boPetMCToMaster);                   // :875

        // 叠加道术给主人
        WriteBool("PetSCToMaster", M2Config.boPetSCToMaster);                   // :878

        // 叠加防御给主人
        WriteBool("PetACToMaster", M2Config.boPetACToMaster);                   // :881

        // 叠加魔防给主人
        WriteBool("PetMACToMaster", M2Config.boPetMACToMaster);                 // :884

        WriteInteger("PetUseItemIntervalTime", (int)M2Config.dwPetUseItemIntervalTime); // :886
        WriteBool("CapturePetNeedItem", M2Config.boCapturePetNeedItem);         // :887
        WriteBool("CaptureOKDecDura", M2Config.boCaptureOKDecDura);             // :888

        WriteBool("PetShowMasterName", M2Config.boPetShowMasterName);           // :890
        WriteInteger("PetNameColor", M2Config.btPetNameColor);                  // :891
        WriteString("PetSuffixName", M2Config.sPetSuffixName);                  // :892

        WriteInteger("GamePetMaxCount", M2Config.nGamePetMaxCount);             // :894
        WriteInteger("GamePetNameCount", M2Config.nGamePetNameCount);           // :895

        WriteInteger("GamePetRecallTime", M2Config.nGamePetRecallTime);         // :897

        WriteBool("GamePetKillMonTrigger", M2Config.boGamePetKillMonTrigger);   // :899

        // VMProtect 键控：**只有** g_nKey == 1 才写这一键（:903-906）
        if (g_nKey_UseClientPickItems == 1)
        {
            WriteBool("EnablePetUseClientPickItems", M2Config.boEnablePetUseClientPickItems); // :905
        }

        SendServerConfigHandler?.Invoke();                                      // :910 UserEngine.SendServerConfig
        btnSavePetParams.Enabled = false;                                       // :911
    }

    private void WriteBool(string key, bool value) => WriteBoolHandler?.Invoke(key, value);
    private void WriteInteger(string key, int value) => WriteIntegerHandler?.Invoke(key, value);
    private void WriteString(string key, string value) => WriteStringHandler?.Invoke(key, value);

    // ========================================================================
    //  :914-1140 cbbLevelExpClick（19 分支经验计划）
    // ========================================================================

    /// <summary>`HIGH_VALUE`（:916 常量 4200000000）。</summary>
    private const long HIGH_VALUE = 4200000000L;

    /// <summary>
    /// `TFrmGamePets.cbbLevelExpClick`（:914-1140）1:1。
    /// 19 个 case 分支全部展开（原文逐分支重复的 `div N` 循环，此处逐字保留展开形式）。
    /// </summary>
    private void cbbLevelExpClick(object Sender)
    {
        if (!boOpened) return;                                                  // :925-926

        // :927 MB_YESNO + MB_ICONQUESTION；= IDNO 则退出
        if (ShowMessage("升级经验计划设置的经验将立即生效，是否确认使用此经验计划？", "确认信息",
                M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDNO)
        {
            return;                                                             // :929 Exit
        }

        TLevelExpScheme LevelExpScheme = (TLevelExpScheme)cbbLevelExp.SelectedIndex; // :933

        switch (LevelExpScheme)
        {
            case TLevelExpScheme.s_OldLevelExp:                                 // :935
                // 原文 g_Config.dwPetNeedExps := g_dwOldNeedExps（整表赋值）
                CopyOldNeedExpsToPet();
                break;

            case TLevelExpScheme.s_StdLevelExp:                                 // :937-963
            {
                bool IsHighLongWord = false;                                    // :939
                CopyOldNeedExpsToPet();                                         // :940
                // :941 dwOneLevelExp := 4000000000 div (High(dwPetNeedExps){ div 2});
                //      High(TLevelNeedExp) = MAXCHANGELEVEL = 1000（被注释掉的 div 2 不参与）
                uint dwOneLevelExp = (uint)(4000000000L / MAXCHANGELEVEL);
                for (int I = 1; I <= MAXCHANGELEVEL; I++)
                {
                    if ((26 + I) > MAXCHANGELEVEL)                              // :944
                        break;                                                  // :945

                    uint dwExp;
                    if (IsHighLongWord)                                         // :947
                        dwExp = (uint)HIGH_VALUE;                               // :948
                    else
                    {
                        long Int64Value = (long)dwOneLevelExp * I;              // :951
                        if (Int64Value >= HIGH_VALUE)                           // :952
                        {
                            IsHighLongWord = true;                              // :954
                            Int64Value = HIGH_VALUE;                            // :955
                        }
                        dwExp = (uint)Int64Value;                               // :957
                    }
                    if (dwExp == 0)                                             // :959
                        dwExp = 1;                                              // :960
                    M2Config.dwPetNeedExps[26 + I] = dwExp;                     // :961
                }
                break;
            }

            // :964-1133 十七个纯倍率分支（div 2/5/8/10/20/30/40/50/60/70/80/90/100/150/200/250/300）
            case TLevelExpScheme.s_2Mult: DividePetNeedExps(2); break;
            case TLevelExpScheme.s_5Mult: DividePetNeedExps(5); break;
            case TLevelExpScheme.s_8Mult: DividePetNeedExps(8); break;
            case TLevelExpScheme.s_10Mult: DividePetNeedExps(10); break;
            case TLevelExpScheme.s_20Mult: DividePetNeedExps(20); break;
            case TLevelExpScheme.s_30Mult: DividePetNeedExps(30); break;
            case TLevelExpScheme.s_40Mult: DividePetNeedExps(40); break;
            case TLevelExpScheme.s_50Mult: DividePetNeedExps(50); break;
            case TLevelExpScheme.s_60Mult: DividePetNeedExps(60); break;
            case TLevelExpScheme.s_70Mult: DividePetNeedExps(70); break;
            case TLevelExpScheme.s_80Mult: DividePetNeedExps(80); break;
            case TLevelExpScheme.s_90Mult: DividePetNeedExps(90); break;
            case TLevelExpScheme.s_100Mult: DividePetNeedExps(100); break;
            case TLevelExpScheme.s_150Mult: DividePetNeedExps(150); break;
            case TLevelExpScheme.s_200Mult: DividePetNeedExps(200); break;
            case TLevelExpScheme.s_250Mult: DividePetNeedExps(250); break;
            case TLevelExpScheme.s_300Mult: DividePetNeedExps(300); break;
        }

        EnsureGridRows(MAXCHANGELEVEL);
        for (int I = 1; I <= GridLevelExp.RowCount - 1; I++)                    // :1135-1138
        {
            GridLevelExp.Rows[I].Cells[1].Value = GXX.Core.Rtl.DelphiRTL.IntToStr(M2Config.dwPetNeedExps[I]);
        }
        ModValue();                                                             // :1139
    }

    /// <summary>
    /// :964-1133 十七个倍率分支共用的循环体 1:1：
    /// `for I := 1 to MAXCHANGELEVEL do begin dwExp := dwPetNeedExps[I] div N; if dwExp = 0 then dwExp := 1;
    /// dwPetNeedExps[I] := dwExp; end;`
    /// ⚠ 原文是**逐分支复制 17 遍**的相同代码；此处抽为一个私有方法，循环体逐字一致。
    /// </summary>
    private static void DividePetNeedExps(uint divisor)
    {
        for (int I = 1; I <= MAXCHANGELEVEL; I++)
        {
            uint dwExp = M2Config.dwPetNeedExps[I] / divisor;
            if (dwExp == 0)
                dwExp = 1;
            M2Config.dwPetNeedExps[I] = dwExp;
        }
    }

    /// <summary>
    /// :936/:940 `g_Config.dwPetNeedExps := g_dwOldNeedExps`（TLevelNeedExp 整表赋值）。
    /// `M2Config.OldNeedExps`（`Engine/ExpTables.g.cs:6`）= 原文 `g_dwOldNeedExps`（索引 0 未用）。
    /// </summary>
    private static void CopyOldNeedExpsToPet()
        => M2Config.OldNeedExps.AsSpan().CopyTo(M2Config.dwPetNeedExps);

    // ========================================================================
    //  :1142-1147 GridLevelExpSetEditText
    // ========================================================================

    /// <summary>`TFrmGamePets.GridLevelExpSetEditText`（:1142-1147）1:1：仅置脏。</summary>
    private void GridLevelExpSetEditText(object Sender, int ACol, int ARow)
    {
        if (!boOpened) return;                                                  // :1144-1145
        ModValue();                                                             // :1146
    }

    // ========================================================================
    //  :1149-1182 btnSaveExpClick
    // ========================================================================

    /// <summary>`TFrmGamePets.btnSaveExpClick`（:1149-1182）1:1。</summary>
    private void btnSaveExpClick(object Sender)
    {
        EnsureGridRows(MAXCHANGELEVEL);
        for (int I = 1; I <= GridLevelExp.RowCount - 1; I++)                    // :1155
        {
            long dwExp = StrToInt64Def(GridLevelExp.Rows[I].Cells[1].Value, 0); // :1157
            if (dwExp > uint.MaxValue)                                          // :1158 High(LongWord)
            {
                // 原文 PChar('等级 ' + IntToStr(I) + ' 升级经验设置错误！')（:1160）
                ShowMessage("等级 " + GXX.Core.Rtl.DelphiRTL.IntToStr(I) + " 升级经验设置错误！",
                    "错误信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
                GridLevelExpLocateRowHandler?.Invoke(I);                        // :1161-1162 Row := I; SetFocus
                return;                                                         // :1163 Exit
            }
            M2Config.dwPetNeedExps[I] = (uint)dwExp;                            // :1165
        }

        WriteBool("PetUseFixExp", M2Config.boPetUseFixExp);                     // :1169
        WriteInteger("PetBaseExp", M2Config.nPetBaseExp);                       // :1170
        WriteInteger("PetAddExp", M2Config.nPetAddExp);                         // :1171
        WriteInteger("PetHighLevel", M2Config.nPetHighLevel);                   // :1172
        WriteInteger("PetHighLevelGetExp", M2Config.nPetHighLevelGetExp);       // :1173

        // :1175-1178 for I := Low(dwPetNeedExps) to High(dwPetNeedExps)
        //   Low(TLevelNeedExp) = 1，High = MAXCHANGELEVEL = 1000
        for (int I = 1; I <= MAXCHANGELEVEL; I++)
        {
            ExpConfigWriteStringHandler?.Invoke("GamePetExp", "Level" + GXX.Core.Rtl.DelphiRTL.IntToStr(I),
                GXX.Core.Rtl.DelphiRTL.IntToStr(M2Config.dwPetNeedExps[I]));
        }

        boModValued = false;                                                    // :1180
        btnSaveExp.Enabled = false;                                             // :1181
    }

    /// <summary>`SysUtils.StrToInt64Def(S, 0)`：无法解析返回 0（不是抛异常）。</summary>
    private static long StrToInt64Def(object value, long def)
    {
        string s = value?.ToString()?.Trim() ?? "";
        if (s.Length == 0) return def;
        return long.TryParse(s, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out long v) ? v : def;
    }

    // ========================================================================
    //  :1184-1350 经验页/全局页剩余开关与 Spin 处理器
    // ========================================================================

    /// <summary>`sePetHighLevelChange`（:1184-1190）。</summary>
    private void sePetHighLevelChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nPetHighLevel = (int)sePetHighLevel.Value;
        ModValue();
    }

    /// <summary>`sePetHighLevelGetExpChange`（:1192-1198）。</summary>
    private void sePetHighLevelGetExpChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nPetHighLevelGetExp = (int)sePetHighLevelGetExp.Value;
        ModValue();
    }

    /// <summary>`chkPetFixExpClick`（:1200-1206）。</summary>
    private void chkPetFixExpClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetUseFixExp = chkPetFixExp.Checked;
        ModValue();
    }

    /// <summary>`sePetBaseExpChange`（:1208-1214）。</summary>
    private void sePetBaseExpChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nPetBaseExp = (int)sePetBaseExp.Value;
        ModValue();
    }

    /// <summary>`sePetAddExpChange`（:1216-1222）。</summary>
    private void sePetAddExpChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nPetAddExp = (int)sePetAddExp.Value;
        ModValue();
    }

    /// <summary>`chkPetShowMasterNameClick`（:1224-1230）。</summary>
    private void chkPetShowMasterNameClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetShowMasterName = chkPetShowMasterName.Checked;
        ModValue();
    }

    /// <summary>`sePetNameColorChange`（:1232-1238）：`Byte(sePetNameColor.Value)` 显式截断。</summary>
    private void sePetNameColorChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.btPetNameColor = (byte)sePetNameColor.Value;                   // :1236 Byte(...)
        ModValue();
    }

    /// <summary>`edtPetSuffixNameChange`（:1240-1246）：写入前 `Trim`。</summary>
    private void edtPetSuffixNameChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.sPetSuffixName = edtPetSuffixName.Text.Trim();                 // :1244 Trim
        ModValue();
    }

    /// <summary>`sePetUseItemIntervalTimeChange`（:1248-1254）。</summary>
    private void sePetUseItemIntervalTimeChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.dwPetUseItemIntervalTime = (uint)sePetUseItemIntervalTime.Value; // :1252
        ModValue();
    }

    /// <summary>`chkCapturePetNeedItemClick`（:1256-1262）。</summary>
    private void chkCapturePetNeedItemClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boCapturePetNeedItem = chkCapturePetNeedItem.Checked;
        ModValue();
    }

    /// <summary>`chkCaptureOKDecDuraClick`（:1264-1270）。</summary>
    private void chkCaptureOKDecDuraClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boCaptureOKDecDura = chkCaptureOKDecDura.Checked;
        ModValue();
    }

    /// <summary>`seGamePetMaxCountChange`（:1272-1278）。</summary>
    private void seGamePetMaxCountChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nGamePetMaxCount = (int)seGamePetMaxCount.Value;
        ModValue();
    }

    /// <summary>`seGamePetNameCountChange`（:1280-1286）。</summary>
    private void seGamePetNameCountChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nGamePetNameCount = (int)seGamePetNameCount.Value;
        ModValue();
    }

    /// <summary>`seGamePetRecallTimeChange`（:1288-1294）。</summary>
    private void seGamePetRecallTimeChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.nGamePetRecallTime = (int)seGamePetRecallTime.Value;
        ModValue();
    }

    /// <summary>`chkDisableMonAttackPetClick`（:1296-1302）。</summary>
    private void chkDisableMonAttackPetClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boDisableMonAttackPet = chkDisableMonAttackPet.Checked;
        ModValue();
    }

    /// <summary>`chkDisableAllAttackPetClick`（:1304-1310）。</summary>
    private void chkDisableAllAttackPetClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boDisableAllAttackPet = chkDisableAllAttackPet.Checked;
        ModValue();
    }

    /// <summary>`chkPetQuickPickupClick`（:1312-1318）。</summary>
    private void chkPetQuickPickupClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetQuickPickup = chkPetQuickPickup.Checked;
        ModValue();
    }

    /// <summary>`chkPetRangePickupClick`（:1320-1326）。</summary>
    private void chkPetRangePickupClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boPetRangePickup = chkPetRangePickup.Checked;
        ModValue();
    }

    /// <summary>`sePetPickupRangeChange`（:1328-1334）。</summary>
    private void sePetPickupRangeChange(object Sender)
    {
        if (!boOpened) return;
        M2Config.btPetPickupRange = (byte)sePetPickupRange.Value;               // :1332
        ModValue();
    }

    /// <summary>`chkEnablePetUseClientPickItemsClick`（:1336-1342）。</summary>
    private void chkEnablePetUseClientPickItemsClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boEnablePetUseClientPickItems = chkEnablePetUseClientPickItems.Checked;
        ModValue();
    }

    /// <summary>`chkGamePetKillMonTriggerClick`（:1344-1350）。</summary>
    private void chkGamePetKillMonTriggerClick(object Sender)
    {
        if (!boOpened) return;
        M2Config.boGamePetKillMonTrigger = chkGamePetKillMonTrigger.Checked;
        ModValue();
    }
}
