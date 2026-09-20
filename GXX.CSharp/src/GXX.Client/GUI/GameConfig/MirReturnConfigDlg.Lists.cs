// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   5116-5132  DMemoBossListClick（选中行 → PlugEditBoss.Text + 两按钮 Enabled）
//   5134-5147  CheckBossNameExists（SameText + CurIndex 排除；**大小写不敏感**）
//   5149-5169  DBtnBossAddClick（空名校验 → 查重 → 追加 → 立即落盘）
//   5171-5179  DBtnBossDelClick
//   5181-5201  DBtnBossModifyClick（含 5195 <c>Lines := Lines</c> 的"自赋值触发变更"怪招）
//   5203-5251  SaveOrLoadBossList
//   5253-5260  DEditSpecialColorChange
//   5262-5290  DBtnDiyAddClick
//   5292-5304  DBtnDiyDelClick
//   5306-5397  DBtnDiyMyLoadOrSaveClick
//   5399-5402  DBtnGJPageControlClick
//   5404-5420  DMemoGJMonListClick
//   5421-5434  CheckGJMonNameExists
//   5436-5456  DBtnGJMonNameAddClick
//   5458-5466  DBtnGJMonNameDelClick
//   5468-5487  DBtnGJMonNameEditClick
//   5489-5537  SaveOrLoadGJMonList
//   5539-5618  SaveOrLoadGJMagicList1（GJNAGICCONFIGFILE1，原文拼写 "NAGIC"）
//   5620-5698  SaveOrLoadGJMagicList2（GJNAGICCONFIGFILE2）

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // MirReturnConfigDlg.pas:5116-5147  Boss 列表：选中 / 查重
    // ================================================================================

    /// <summary>
    /// 原文 5116-5132：
    /// <c>if (ItemIndex &gt;= 0) and (ItemIndex &lt;= Lines.Count - 1) then</c>
    /// 回填 <c>PlugEditBoss.Text</c> + 打开 Modify/Del 按钮；否则清空文本 + 关闭两按钮。
    /// 原文 5119 有一行被注释掉的 <c>ScrollMouseDown</c>（原文如此）。
    /// </summary>
    public void DMemoBossListClick()
    {
        //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;（原文 5119 —— 原文如此）
        if ((Plug.PlugScrollBoxBossItemIndex >= 0) &&
            (Plug.PlugScrollBoxBossItemIndex <= Plug.PlugScrollBoxBossLines.Count - 1))   // 5120
        {
            Plug.PlugEditBoss = Plug.PlugScrollBoxBossLines[Plug.PlugScrollBoxBossItemIndex]; // 5122
            Plug.PlugBtnBossModify = true;                          // 5123
            Plug.PlugBtnBossDel = true;                             // 5124
        }
        else
        {
            Plug.PlugEditBoss = "";                                    // 5128
            Plug.PlugBtnBossModify = false;                         // 5129
            Plug.PlugBtnBossDel = false;                            // 5130
        }
    }

    /// <summary>
    /// 原文 5134-5147：
    /// <c>Result := False; for I := 0 to Lines.Count - 1 do
    /// if SameText(Name, Lines[I]) and (CurIndex &lt;&gt; I) then begin Result := True; Exit; end;</c>
    ///
    /// 语义要点（1:1）：
    /// <list type="bullet">
    /// <item><c>SameText</c> = **大小写不敏感**（托管侧 OrdinalIgnoreCase）</item>
    /// <item>形参 <c>CurIndex</c> 的原默认值是 <c>-1</c>（原文 415 行声明），
    ///       传 -1 时"排除自己"这一条恒成立（因为 I 从 0 起）</item>
    /// <item>命中即 <c>Exit</c> —— 只判第一次命中，但结果与全扫一致</item>
    /// </list>
    /// </summary>
    public bool CheckBossNameExists(string Name, int CurIndex = -1)
    {
        bool Result = false;                                               // 5138
        for (int I = 0; I <= Plug.PlugScrollBoxBossLines.Count - 1; I++)
        {
            if (SameText(Name, Plug.PlugScrollBoxBossLines[I]) && (CurIndex != I))   // 5141
            {
                Result = true;                                             // 5143
                return Result;                                             // 5144 Exit
            }
        }
        return Result;
    }

    /// <summary>Delphi <c>SameText</c>（AnsiCompareText，大小写不敏感）。</summary>
    internal static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    // ================================================================================
    // MirReturnConfigDlg.pas:5149-5201  Boss 增/删/改
    // ================================================================================

    /// <summary>
    /// 原文 5149-5169：
    /// <c>BossName := Trim(PlugEditBoss.Text);</c> →
    /// 空则 <c>DMessageDlg('Boss名不能为空', [mbOk])</c> 并 Exit →
    /// 不存在则 <c>Lines.Add(BossName); SaveOrLoadBossList(True);</c>
    /// 否则 <c>DMessageDlg('Boss名已存在，添加失败', [mbOk])</c>。
    /// 注意原文 5162-5165 有一层**多余的 begin/end**（只有一条有效语句 + 一行注释）—— 原文如此。
    /// </summary>
    public void DBtnBossAddClick()
    {
        string BossName = (Plug.PlugEditBoss ?? "").Trim();            // 5153
        if (BossName.Length == 0)                                          // 5154
        {
            MirReturnMessageSeam.DMessageDlg("Boss名不能为空");            // 5156
            return;                                                        // 5157 Exit
        }
        if (!CheckBossNameExists(BossName))                                // 5159
        {
            Plug.PlugScrollBoxBossLines.Add(BossName);                     // 5161
            {
                SaveOrLoadBossList(true);                                  // 5163
                //PlugScrollBoxBoss.ItemIndex := PlugScrollBoxBoss.Lines.Count - 1;（5164 —— 原文如此）
            }
        }
        else
            MirReturnMessageSeam.DMessageDlg("Boss名已存在，添加失败");     // 5168
    }

    /// <summary>
    /// 原文 5171-5179：仅当 <c>ItemIndex</c> 在界内才 <c>Lines.Delete(ItemIndex); PlugEditBoss.Text := ''; SaveOrLoadBossList(True);</c>
    /// （**不**重置 <c>ItemIndex</c> —— 原文如此）。
    /// </summary>
    public void DBtnBossDelClick()
    {
        if ((Plug.PlugScrollBoxBossItemIndex >= 0) &&
            (Plug.PlugScrollBoxBossItemIndex <= Plug.PlugScrollBoxBossLines.Count - 1))   // 5173
        {
            Plug.PlugScrollBoxBossLines.RemoveAt(Plug.PlugScrollBoxBossItemIndex);        // 5175
            Plug.PlugEditBoss = "";                                    // 5176
            SaveOrLoadBossList(true);                                      // 5177
        }
    }

    /// <summary>
    /// 原文 5181-5201：
    /// 空名检查同 Add；然后 <c>if not CheckBossNameExists(BossName, PlugScrollBoxBoss.ItemIndex) then</c>
    /// 就地改写该行 → 5195 的 <c>PlugScrollBoxBoss.Lines := PlugScrollBoxBoss.Lines;</c>
    /// （**自赋值**，只为触发控件的数据变更通知）→ 落盘；否则"修改失败"。
    ///
    /// ★ 原文缺陷（登记不改）：若 <c>ItemIndex = -1</c>（无选中），
    /// <c>Lines[-1]</c> 在 Delphi 里抛 <c>EStringListError</c>；
    /// 而 <c>CheckBossNameExists(name, -1)</c> 此时**恒返回 False**（无排除项），
    /// 于是必然走到 5194 的越界写。托管侧保留同一路径（由宿主控件抛出），
    /// 但把"越界"显式实现为 <see cref="ArgumentOutOfRangeException"/> 以免静默写坏数据。
    /// </summary>
    public void DBtnBossModifyClick()
    {
        string BossName = (Plug.PlugEditBoss ?? "").Trim();            // 5186
        if (BossName.Length == 0)                                          // 5187
        {
            MirReturnMessageSeam.DMessageDlg("Boss名不能为空");            // 5189
            return;                                                        // 5190 Exit
        }
        if (!CheckBossNameExists(BossName, Plug.PlugScrollBoxBossItemIndex))   // 5192
        {
            // 5194：PlugScrollBoxBoss.Lines[PlugScrollBoxBoss.ItemIndex] := BossName
            if (Plug.PlugScrollBoxBossItemIndex < 0 ||
                Plug.PlugScrollBoxBossItemIndex > Plug.PlugScrollBoxBossLines.Count - 1)
                throw new ArgumentOutOfRangeException(
                    nameof(Plug.PlugScrollBoxBossItemIndex),
                    "MirReturnConfigDlg.pas:5194 原文在 ItemIndex=-1 时会抛 EStringListError（原文如此）");
            Plug.PlugScrollBoxBossLines[Plug.PlugScrollBoxBossItemIndex] = BossName;

            // 5195：PlugScrollBoxBoss.Lines := PlugScrollBoxBoss.Lines;（自赋值，触发变更通知 —— 原文如此）
            SetBossLinesNoOp();

            SaveOrLoadBossList(true);                                      // 5197
        }
        else
            MirReturnMessageSeam.DMessageDlg("Boss名已存在，修改失败");     // 5200
    }

    /// <summary>原文 5195 的 <c>Lines := Lines</c> 自赋值（托管侧 List 无此语义，留空并登记）。</summary>
    private void SetBossLinesNoOp() { }

    // ================================================================================
    // MirReturnConfigDlg.pas:5203-5251  SaveOrLoadBossList
    // ================================================================================

    /// <summary>
    /// 原文 5203-5251。落盘模板 <c>BOSSCONFIGFILE = 'Config\%s.%s.Boss.set'</c>。
    /// IsSave：<c>Lines.SaveToFile</c>；否则 <c>Lines.Clear</c> + <c>FileExists</c> 才 <c>LoadFromFile</c> +
    /// <c>ItemIndex := -1</c> + 关闭两个按钮。
    /// **两分支末尾都**执行 <c>g_BossList.Text := PlugScrollBoxBoss.Lines.Text;</c>（5250）—— 原文如此。
    /// </summary>
    public void SaveOrLoadBossList(bool IsSave)
    {
        string sDirectory = PrepareConfigDirectory();                      // 5208-5209
        _ = sDirectory;

        RefreshPlugUserNameFromMySelf();                                   // 5211-5231

        string sFileName = ConfigFileName(MirReturnGlobalSeam.BOSSCONFIGFILE);   // 5233
        if (IsSave)                                                        // 5234
        {
            Plug.PlugScrollBoxBossSaveToFile(sFileName);              // 5236
            //PlugScrollBoxBoss.Position := 0;（5237 —— 原文如此）
        }
        else
        {
            Plug.PlugScrollBoxBossLines.Clear();                           // 5241
            if (MirReturnGlobalSeam.FileExists(sFileName))                 // 5242
                Plug.PlugScrollBoxBossLoadFromFile(sFileName);        // 5243
            //PlugScrollBoxBoss.Position := 0;（5244 —— 原文如此）
            Plug.PlugScrollBoxBossItemIndex = -1;                          // 5245
            Plug.PlugBtnBossModify = false;                         // 5246
            Plug.PlugBtnBossDel = false;                            // 5247
        }

        MirReturnGlobalSeam.g_BossList.Clear();                            // 5250
        MirReturnGlobalSeam.g_BossList.AddRange(Plug.PlugScrollBoxBossLines);
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5253-5260  DEditSpecialColorChange
    // ================================================================================

    /// <summary>
    /// 原文 5253-5260：
    /// 5255 有**一行被注释掉的旧逻辑**（<c>g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;</c>）
    /// —— 原文如此，保留。
    /// 然后 <c>if Value &gt; 255 then Value := 255;</c>（**只夹上界、无下界**）→
    /// <c>g_Config.nSpecialColor := ...; frmMain.nSpecialColor := ...;
    /// PlugLabelSpecialColor.CaptionColor.Up.Color := GetRGB(Value);</c>
    /// </summary>
    public void DEditSpecialColorChange()
    {
        //g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;（原文 5255 —— 原文如此）
        if (Plug.PlugEditSpecialColor > 255)                               // 5256
            Plug.PlugEditSpecialColor = 255;
        g_Config.nSpecialColor = (byte)Plug.PlugEditSpecialColor;          // 5257
        MirReturnGlobalSeam.SetFrmMainSpecialColor(g_Config.nSpecialColor); // 5258
        MirReturnGlobalSeam.SetLabelSpecialColorUp(                        // 5259
            MirReturnGlobalSeam.GetRGB(Plug.PlugEditSpecialColor));
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5262-5304  DIY 增/删
    // ================================================================================

    /// <summary>
    /// 原文 5262-5290：
    /// <c>sItemName := PlugEditSpecialName.Text;</c> → <c>FileItemDB.Find</c> 命中则**什么都不做**
    /// （★ 原文**没有 else 分支的提示**，且 in 外**没有 else**）→ 否则 <c>New(ShowItem)</c>、
    /// 逐字段赋值（ItemType=i_diy、sItemType='自定类'、四个 bo* 全 False）→
    /// <c>FileItemDB.Add(ShowItem)</c> → 再 <c>New(FileItem); FileItem^ := ShowItem^;
    /// FileItemDB.m_FileItemList.Add(FileItem)</c>（**同一份数据进两个列表**）→
    /// <c>PlugComboBoxItemStdMode.ItemIndex := 8; DComboBoxItemStdModeSelect(Sender); PlugMemoConfig2.Last;</c>
    /// </summary>
    public void DBtnDiyAddClick()
    {
        string sItemName = Plug.PlugEditSpecialName;                   // 5268
        TShowItem ShowItem = FileItemDB.Find(sItemName);                   // 5269
        if (ShowItem == null)                                              // 5270
        {
            ShowItem = new TShowItem();                                    // 5272 New(ShowItem)
            ShowItem.ItemType = TItemType.i_diy;                           // 5273 //GetItemType(sItemType);
            ShowItem.sItemType = "自定类";                                 // 5274 //sItemType;
            ShowItem.sItemName = sItemName;                                // 5275
            ShowItem.boHintMsg = 0;                                        // 5276 False
            ShowItem.boPickup = 0;                                         // 5277 False
            ShowItem.boShowName = 0;                                       // 5278 False
            ShowItem.boShowSpecial = 0;                                    // 5279 False
            //m_ShowItemList.Add(ShowItem);（5280 —— 原文如此）
            FileItemDB.Add(ShowItem);                                      // 5281
            TShowItem FileItem = new TShowItem();                          // 5282 New(FileItem)
            FileItem.AssignFrom(ShowItem);                                 // 5283 FileItem^ := ShowItem^
            FileItemDB.m_FileItemList.Add(FileItem);                       // 5284
            //FileItemDB.SaveToFile;（5285 —— 原文如此）
            Plug.PlugComboBoxItemStdMode = 8;                     // 5286
            DComboBoxItemStdModeSelect();                                  // 5287
            Plug.PlugMemoConfig2Last();                                    // 5288
        }
    }

    /// <summary>
    /// 原文 5292-5304：<c>sItemName &lt;&gt; ''</c> 时
    /// <c>FileItemDB.Del(sItemName); PlugComboBoxItemStdMode.ItemIndex := 8;
    /// DComboBoxItemStdModeSelect(Sender); PlugMemoConfig2.Last;</c>
    /// （★ **不做 Find 预判**，直接 Del —— 与 Add 的不对称，原文如此）。
    /// </summary>
    public void DBtnDiyDelClick()
    {
        string sItemName = Plug.PlugEditSpecialName;                   // 5296
        if (sItemName != "")                                               // 5297
        {
            FileItemDB.Del(sItemName);                                     // 5299
            Plug.PlugComboBoxItemStdMode = 8;                     // 5300
            DComboBoxItemStdModeSelect();                                  // 5301
            Plug.PlugMemoConfig2Last();                                    // 5302
        }
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5399-5402  DBtnGJPageControlClick
    // ================================================================================

    /// <summary>原文 5399-5402：<c>PlugMemoConfig8Page.ActivePageIndex := (Sender as TDxImageButton).Tag;</c></summary>
    public void DBtnGJPageControlClick(int SenderTag)
    {
        Plug.PlugMemoConfig8Page = SenderTag;               // 5401
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5404-5434  挂机怪物名单：选中 / 查重
    // ================================================================================

    /// <summary>原文 5404-5420：与 <see cref="DMemoBossListClick"/> 同形，作用于 Mons 组控件。</summary>
    public void DMemoGJMonListClick()
    {
        //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;（原文 5407 —— 原文如此）
        if ((Plug.PlugScrollBoxMonsItemIndex >= 0) &&
            (Plug.PlugScrollBoxMonsItemIndex <= Plug.PlugScrollBoxMonsLines.Count - 1))   // 5408
        {
            Plug.PlugEditMonName = Plug.PlugScrollBoxMonsLines[Plug.PlugScrollBoxMonsItemIndex]; // 5410
            Plug.PlugButtonMonNameEdit = true;                      // 5411
            Plug.PlugButtonMonNameDel = true;                       // 5412
        }
        else
        {
            Plug.PlugEditMonName = "";                                 // 5416
            Plug.PlugButtonMonNameEdit = false;                     // 5417
            Plug.PlugButtonMonNameDel = false;                      // 5418
        }
    }

    /// <summary>原文 5421-5434：与 <see cref="CheckBossNameExists"/> 逐字同形（只是换成 Mons 列表）。</summary>
    public bool CheckGJMonNameExists(string Name, int CurIndex = -1)
    {
        bool Result = false;                                               // 5425
        for (int I = 0; I <= Plug.PlugScrollBoxMonsLines.Count - 1; I++)
        {
            if (SameText(Name, Plug.PlugScrollBoxMonsLines[I]) && (CurIndex != I))   // 5428
            {
                Result = true;                                             // 5430
                return Result;                                             // 5431 Exit
            }
        }
        return Result;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5436-5487  挂机怪物名单 增/删/改
    // ================================================================================

    /// <summary>原文 5436-5456：与 <c>DBtnBossAddClick</c> 同形，提示文案为 '怪物名不能为空'/'怪物名已存在，添加失败'。</summary>
    public void DBtnGJMonNameAddClick()
    {
        string MonName = (Plug.PlugEditMonName ?? "").Trim();          // 5440
        if (MonName.Length == 0)                                           // 5441
        {
            MirReturnMessageSeam.DMessageDlg("怪物名不能为空");            // 5443
            return;                                                        // 5444 Exit
        }
        if (!CheckGJMonNameExists(MonName))                                // 5446
        {
            Plug.PlugScrollBoxMonsLines.Add(MonName);                      // 5448
            {
                SaveOrLoadGJMonList(true);                                 // 5450
                //PlugScrollBoxMons.ItemIndex := PlugScrollBoxMons.Lines.Count - 1;（5451 —— 原文如此）
            }
        }
        else
            MirReturnMessageSeam.DMessageDlg("怪物名已存在，添加失败");     // 5455
    }

    /// <summary>原文 5458-5466：与 <c>DBtnBossDelClick</c> 同形。</summary>
    public void DBtnGJMonNameDelClick()
    {
        if ((Plug.PlugScrollBoxMonsItemIndex >= 0) &&
            (Plug.PlugScrollBoxMonsItemIndex <= Plug.PlugScrollBoxMonsLines.Count - 1))   // 5460
        {
            Plug.PlugScrollBoxMonsLines.RemoveAt(Plug.PlugScrollBoxMonsItemIndex);        // 5462
            Plug.PlugEditMonName = "";                                 // 5463
            SaveOrLoadGJMonList(true);                                     // 5464
        }
    }

    /// <summary>
    /// 原文 5468-5487：与 <c>DBtnBossModifyClick</c> 同形（含 5482 的 <c>Lines := Lines</c> 自赋值），
    /// 提示文案为 '怪物名不能为空'/'怪物名已存在，修改失败'。
    /// </summary>
    public void DBtnGJMonNameEditClick()
    {
        string MonName = (Plug.PlugEditMonName ?? "").Trim();          // 5473
        if (MonName.Length == 0)                                           // 5474
        {
            MirReturnMessageSeam.DMessageDlg("怪物名不能为空");            // 5476
            return;                                                        // 5477 Exit
        }
        if (!CheckGJMonNameExists(MonName, Plug.PlugScrollBoxMonsItemIndex))   // 5479
        {
            if (Plug.PlugScrollBoxMonsItemIndex < 0 ||
                Plug.PlugScrollBoxMonsItemIndex > Plug.PlugScrollBoxMonsLines.Count - 1)
                throw new ArgumentOutOfRangeException(
                    nameof(Plug.PlugScrollBoxMonsItemIndex),
                    "MirReturnConfigDlg.pas:5481 原文在 ItemIndex=-1 时会抛 EStringListError（原文如此）");
            Plug.PlugScrollBoxMonsLines[Plug.PlugScrollBoxMonsItemIndex] = MonName;   // 5481
            SetMonsLinesNoOp();                                            // 5482
            SaveOrLoadGJMonList(true);                                     // 5483
        }
        else
            MirReturnMessageSeam.DMessageDlg("怪物名已存在，修改失败");     // 5486
    }

    /// <summary>原文 5482 的 <c>Lines := Lines</c> 自赋值（托管侧 List 无此语义，留空并登记）。</summary>
    private void SetMonsLinesNoOp() { }

    // ================================================================================
    // MirReturnConfigDlg.pas:5489-5537  SaveOrLoadGJMonList
    // ================================================================================

    /// <summary>原文 5489-5537：与 <c>SaveOrLoadBossList</c> 同形，模板 <c>GJMONCONFIGFILE</c>，末尾写 <c>g_GJMonList</c>。</summary>
    public void SaveOrLoadGJMonList(bool IsSave)
    {
        string sDirectory = PrepareConfigDirectory();                      // 5494-5495
        _ = sDirectory;

        RefreshPlugUserNameFromMySelf();                                   // 5497-5517

        string sFileName = ConfigFileName(MirReturnGlobalSeam.GJMONCONFIGFILE);   // 5519
        if (IsSave)                                                        // 5520
        {
            Plug.PlugScrollBoxMonsSaveToFile(sFileName);              // 5522
            //PlugScrollBoxMons.Position := 0;（5523 —— 原文如此）
        }
        else
        {
            Plug.PlugScrollBoxMonsLines.Clear();                           // 5527
            if (MirReturnGlobalSeam.FileExists(sFileName))                 // 5528
                Plug.PlugScrollBoxMonsLoadFromFile(sFileName);        // 5529
            //PlugScrollBoxMons.Position := 0;（5530 —— 原文如此）
            Plug.PlugScrollBoxMonsItemIndex = -1;                          // 5531
            Plug.PlugButtonMonNameEdit = false;                     // 5532
            Plug.PlugButtonMonNameDel = false;                      // 5533
        }

        MirReturnGlobalSeam.g_GJMonList.Clear();                           // 5536
        MirReturnGlobalSeam.g_GJMonList.AddRange(Plug.PlugScrollBoxMonsLines);
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5539-5698  SaveOrLoadGJMagicList1 / 2
    // ================================================================================

    /// <summary>
    /// 原文 5539-5618。落盘模板 <c>GJNAGICCONFIGFILE1</c>（★ 原文拼写 "NAGIC"，非 "MAGIC"）。
    ///
    /// **语义要点（1:1，且两处易错）**：
    /// <list type="bullet">
    /// <item>无论 IsSave 与否，**先无条件** <c>g_GJUseMagic1.Clear;</c>（5574）</item>
    /// <item>IsSave：遍历 <c>PlugMemoConfig82</c>，只取 <c>ListItem.Count = 2</c> 的行
    ///       （**恰好两列**，多/少都不取），看第 2 列（下标 1）的 <c>Checked</c>，
    ///       写入 <c>g_GJUseMagic1</c> 与文本行 <c>IntToStr(Integer(ViewItem.Data))</c></item>
    /// <item>Load：先 <c>PlugMemoConfig82.Clear;</c>，**然后**才 <c>if not FileExists then Exit;</c>
    ///       —— 即"文件不存在"时控件仍被清空，但 <c>g_GJUseMagic1</c> 停在已 Clear 的空表</item>
    /// <item>Load 的 <c>StrToIntDef(SL[I], 0)</c> 与 <c>if Value &lt;&gt; 0</c> 过滤：
    ///       **值为 0 的行被静默丢弃**（原文如此）</item>
    /// </list>
    /// </summary>
    public void SaveOrLoadGJMagicList1(bool IsSave)
    {
        string sDirectory = PrepareConfigDirectory();                      // 5548-5549
        _ = sDirectory;

        RefreshPlugUserNameFromMySelf();                                   // 5551-5571

        string sFileName = ConfigFileName(MirReturnGlobalSeam.GJNAGICCONFIGFILE1);   // 5573
        MirReturnGlobalSeam.g_GJUseMagic1.Clear();                         // 5574 ★ 无条件

        if (IsSave)                                                        // 5575
        {
            var SL = new List<string>();                                   // 5577
            try
            {
                for (int I = 0; I <= Plug.PlugMemoConfig82Count - 1; I++)  // 5579
                {
                    if (Plug.PlugMemoConfig82ItemCount(I) == 2)            // 5582 ListItem.Count = 2
                    {
                        if (Plug.PlugMemoConfig82Item2Checked(I))          // 5585 ViewItem.Checked
                        {
                            MirReturnGlobalSeam.g_GJUseMagic1.Add(         // 5587
                                (object)Plug.PlugMemoConfig82Item2Data(I));
                            SL.Add(MirReturnGlobalSeam.IntToStr(           // 5588
                                unchecked((int)Plug.PlugMemoConfig82Item2Data(I))));
                        }
                    }
                }
                MirReturnGlobalSeam.SaveTextFile(sFileName, SL);           // 5592 SL.SaveToFile
            }
            finally
            {
                // 5594：SL.Free（托管侧无需释放）
            }
        }
        else
        {
            Plug.PlugMemoConfig82Clear();                                  // 5599
            if (!MirReturnGlobalSeam.FileExists(sFileName)) return;        // 5600 Exit

            List<string> SL = MirReturnGlobalSeam.LoadTextFile(sFileName); // 5604
            try
            {
                for (int I = 0; I <= SL.Count - 1; I++)                    // 5606
                {
                    int Value = MirReturnGlobalSeam.StrToIntDef(SL[I], 0); // 5608
                    if (Value != 0)                                        // 5609
                    {
                        MirReturnGlobalSeam.g_GJUseMagic1.Add((object)unchecked((uint)Value));   // 5611 Pointer(Value)
                    }
                }
            }
            finally
            {
                // 5615：SL.Free
            }
        }
    }

    /// <summary>
    /// 原文 5620-5698：与 <c>SaveOrLoadGJMagicList1</c> 逐字同形，换成
    /// <c>GJNAGICCONFIGFILE2</c> / <c>PlugMemoConfig83</c> / <c>g_GJUseMagic2</c>。
    /// ★ 唯一结构差异：Load 分支里 <c>SL</c> 的创建位置（5682）在 <c>Exit</c> 之后 —— 语义等价。
    /// </summary>
    public void SaveOrLoadGJMagicList2(bool IsSave)
    {
        string sDirectory = PrepareConfigDirectory();                      // 5629-5630
        _ = sDirectory;

        RefreshPlugUserNameFromMySelf();                                   // 5632-5652

        string sFileName = ConfigFileName(MirReturnGlobalSeam.GJNAGICCONFIGFILE2);   // 5654
        MirReturnGlobalSeam.g_GJUseMagic2.Clear();                         // 5655 ★ 无条件

        if (IsSave)                                                        // 5656
        {
            var SL = new List<string>();                                   // 5658
            try
            {
                for (int I = 0; I <= Plug.PlugMemoConfig83Count - 1; I++)  // 5660
                {
                    if (Plug.PlugMemoConfig83ItemCount(I) == 2)           // 5663 ListItem.Count = 2
                    {
                        if (Plug.PlugMemoConfig83Item2Checked(I))          // 5666 ViewItem.Checked
                        {
                            MirReturnGlobalSeam.g_GJUseMagic2.Add(         // 5668
                                (object)Plug.PlugMemoConfig83Item2Data(I));
                            SL.Add(MirReturnGlobalSeam.IntToStr(           // 5669
                                unchecked((int)Plug.PlugMemoConfig83Item2Data(I))));
                        }
                    }
                }
                MirReturnGlobalSeam.SaveTextFile(sFileName, SL);           // 5673
            }
            finally
            {
                // 5675：SL.Free
            }
        }
        else
        {
            Plug.PlugMemoConfig83Clear();                                  // 5680
            if (!MirReturnGlobalSeam.FileExists(sFileName)) return;        // 5681 Exit
            List<string> SL = MirReturnGlobalSeam.LoadTextFile(sFileName); // 5684
            try
            {
                for (int I = 0; I <= SL.Count - 1; I++)                    // 5686
                {
                    int Value = MirReturnGlobalSeam.StrToIntDef(SL[I], 0); // 5688
                    if (Value != 0)                                        // 5689
                    {
                        MirReturnGlobalSeam.g_GJUseMagic2.Add((object)unchecked((uint)Value));   // 5691
                    }
                }
            }
            finally
            {
                // 5695：SL.Free
            }
        }
    }
}
