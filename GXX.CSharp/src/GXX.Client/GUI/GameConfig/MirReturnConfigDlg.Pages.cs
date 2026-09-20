// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   1354-1427  DComboBoxItemStdModeSelect（物品类别下拉 → PlugMemoConfig2 的 6 列行模型）
//   1431-1435  DComboBoxColorShow
//   5306-5397  DBtnDiyMyLoadOrSaveClick（读取/保存"自定义物品过滤"，含 5316/5391 的 Sender 判等）
//   5700-5705  见 Lifecycle 分片
//   5776-5840  RefreshGJMagic（两页技能表，各 1 行 2 列）
//
// 三处"6 列行"（RefShowItem 1104-1153 / DComboBoxItemStdModeSelect 1372-1421 /
// DBtnDiyMyLoadOrSaveClick 5331-5380）在原文里是**逐字重复的三份**；
// 托管侧收敛为 <see cref="BuildShowItemRow"/> 一个私有方法，
// 每个调用点都注明原文行号，语义与三份重复完全一致。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // MirReturnConfigDlg.pas:1354-1427  DComboBoxItemStdModeSelect
    // ================================================================================

    /// <summary>
    /// 原文 1354-1427：<c>FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List)</c> →
    /// 清空 + <c>ColCount := 6</c> → <c>Lock</c>/<c>UnLock</c> 包夹 → 逐项建 1 行 6 列
    /// （与 <see cref="RefShowItem"/> 的行结构**逐字相同**）。
    ///
    /// ★ 注意原文把 <c>ItemIndex</c> **硬转**成 <c>TItemType</c>：<c>ItemIndex = -1</c>（无选中）时
    /// 得到枚举值 -1，<c>FileItemDB.Get</c> 的输出取决于其内部 <c>case</c> 的 default 分支。
    /// 照抄（不做范围夹紧）。
    /// </summary>
    public void DComboBoxItemStdModeSelect()
    {
        var List = new List<TShowItem>();                                   // 1363 TList.Create
        FileItemDB.Get((TItemType)Plug.PlugComboBoxItemStdMode, List);   // 1364
        Plug.PlugMemoConfig2Clear();                                        // 1365
        Plug.PlugMemoConfig2ColCount = 6;                                   // 1366
        Plug.PlugMemoConfig2Lock();                                         // 1367
        try
        {
            for (int I = 0; I <= List.Count - 1; I++)                       // 1369
            {
                BuildShowItemRow(List[I]);                                  // 1372-1421
            }
        }
        finally
        {
            Plug.PlugMemoConfig2UnLock();                               // 1424
        }
        // 1426：List.Free（托管侧无需释放）
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:1431-1435  DComboBoxColorShow（BOSS 颜色事件 piaoyun 2013-09-09）
    // ================================================================================

    /// <summary>原文 1431-1435：<c>g_Config.nColorShowEff := PlugComboBoxColorShow.ItemIndex;</c> + frmMain 同步。</summary>
    public void DComboBoxColorShow()
    {
        g_Config.nColorShowEff = (byte)Plug.PlugComboBoxColorShow;   // 1433
        MirReturnGlobalSeam.SetFrmMainColorShowEff(g_Config.nColorShowEff);   // 1434
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5306-5397  DBtnDiyMyLoadOrSaveClick
    // ================================================================================

    /// <summary>
    /// 原文 5306-5397（两个分支的 Sender 判等：<c>PlugBtnDiyLoad</c> / <c>plugBtnDiySave</c>
    /// —— ★ 原文第二处的小写 <c>plugBtnDiySave</c> 是**标识符大小写不一致**，Delphi 大小写不敏感
    /// 所以是同一个控件；托管侧统一为 <see cref="MirReturnDiyButtonId.PlugBtnDiySave"/> 并在此登记）。
    ///
    /// Load 分支：<c>FileItemDB.LoadFormFile(True)</c> → <c>Get(TItemType(ItemIndex), List)</c> →
    /// 重建 6 列行模型（5331-5380，与 RefShowItem 同形）→ 聊天栏提示。
    /// Save 分支：<c>FileItemDB.SaveToFile(True)</c> → 聊天栏提示。
    ///
    /// ★ 两处 <c>AddChatBoardString</c> 用的颜色是 **<c>GetRGB(219)</c>**（不是固定常量）——
    /// 原文如此，托管侧保留 <c>GetRGB</c> 调用点。
    /// </summary>
    public void DBtnDiyMyLoadOrSaveClick(MirReturnDiyButtonId Sender)
    {
        if (Sender == MirReturnDiyButtonId.PlugBtnDiyLoad)                  // 5316
        {
            FileItemDB.LoadFormFile();                                      // 5318 LoadFormFile(True)

            var List = new List<TShowItem>();                               // 5320
            FileItemDB.Get((TItemType)Plug.PlugComboBoxItemStdMode, List);   // 5321
            Plug.PlugMemoConfig2Clear();                                    // 5322
            Plug.PlugMemoConfig2ColCount = 6;                               // 5323

            Plug.PlugMemoConfig2Lock();                                     // 5325
            try
            {
                for (int I = 0; I <= List.Count - 1; I++)                   // 5327
                {
                    BuildShowItemRow(List[I]);                              // 5331-5380
                }
            }
            finally
            {
                Plug.PlugMemoConfig2UnLock();                           // 5383
            }

            string S = "读取上次保存的配置成功.如果没获取到请检查选择分区名是否一致.";   // 5386
            ChatBoardSeam.AddChatBoardString(S, MirReturnGlobalSeam.GetRGB(219), ChatClWhite);   // 5387

            // 5389：List.Free
        }
        else if (Sender == MirReturnDiyButtonId.PlugBtnDiySave)             // 5391
        {
            FileItemDB.SaveToFile();                                        // 5393 SaveToFile(True)
            string S = "保存成功，下次进入游戏本区所有玩家只需点读取即可获取本次保存的设置.";   // 5394
            ChatBoardSeam.AddChatBoardString(S, MirReturnGlobalSeam.GetRGB(219), ChatClWhite);   // 5395
        }
    }

    /// <summary>
    /// 原文 1104-1153（RefShowItem）/ 1372-1421（DComboBoxItemStdModeSelect）/
    /// 5331-5380（DBtnDiyMyLoadOrSaveClick）**三处逐字重复**的"1 行 6 列"构建。
    /// 语义见 <see cref="RefShowItem"/> 的注释。
    /// </summary>
    private int BuildShowItemRow(TShowItem ShowItem)
    {
        int row = Plug.PlugMemoConfig2Add();

        int c0 = Plug.PlugMemoConfig2AddItem(row);
        Plug.PlugMemoConfig2SetItemCaption(row, c0, ShowItem.sItemName);
        Plug.PlugMemoConfig2SetItemData(row, c0, ShowItemRowHandle(ShowItem));
        Plug.PlugMemoConfig2SetItemStyle(row, c0, BsButton);                        // // bsRadio
        Plug.PlugMemoConfig2SetItemAlignment(row, c0, TaLeftJustify);
        Plug.PlugMemoConfig2SetItemColor(row, c0, "Up", ViewClWhite);
        Plug.PlugMemoConfig2SetItemColor(row, c0, "Hot", ViewClRed);                // // clWhite
        Plug.PlugMemoConfig2SetItemColor(row, c0, "Down", ViewClRed);

        AddShowItemCheckCell(row, ShowItem, ShowItem.boHintMsg);
        AddShowItemCheckCell(row, ShowItem, ShowItem.boPickup);
        AddShowItemCheckCell(row, ShowItem, ShowItem.boShowName);
        AddShowItemCheckCell(row, ShowItem, ShowItem.boShowSpecial);
        AddShowItemCheckCell(row, ShowItem, ShowItem.boAutoMove);

        return row;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5776-5840  RefreshGJMagic
    // ================================================================================

    /// <summary>
    /// 原文 5776-5840：把 <c>g_MagicList</c> 逐项灌进**两页**技能表（82 / 83），
    /// 每项建 **1 行 2 列**：
    /// <list type="number">
    /// <item>第 1 列：<c>Caption = Magic.Def.sMagicName</c>、<c>Data = Magic.Def.wMagicId</c>、
    ///       <c>Style = bsButton</c>（原文注释 // bsRadio）、<c>Alignment = taLeftJustify</c>、
    ///       Up=clWhite / Hot=clRed（// clWhite）/ Down=clRed</item>
    /// <item>第 2 列：<c>Data = wMagicId</c>、<c>Style = bsCheckBox</c>、
    ///       <c>ImageType = NewopUI_Pak</c>、<c>Up=228/Down=229</c>、
    ///       <c>Checked := g_GJUseMagicN.IndexOf(Pointer(wMagicId)) &lt;&gt; -1</c></item>
    /// </list>
    /// 两页各自 <c>Clear</c> + <c>Lock</c>/<c>UnLock</c>。
    ///
    /// ★ 5835 行末尾有一个**多余的分号**（<c>… &lt;&gt; -1;;</c>）—— 原文如此，语义无影响。
    /// ★ <c>wMagicId</c> 是 <c>Word</c>（16 位），<c>IndexOf</c> 找的是 <c>Pointer(wMagicId)</c>
    ///   即**把 Word 当指针值**比较 —— 照抄（托管侧同样按 uint 值比较）。
    /// </summary>
    private void RefreshGJMagicImpl()
    {
        // ---- 第 1 页（PlugMemoConfig82） ----
        Plug.PlugMemoConfig82Clear();                                      // 5783
        Plug.PlugMemoConfig82Lock();                                       // 5784
        try
        {
            for (int I = 0; I <= MirReturnConfigGlobalSeam.g_MagicList.Count - 1; I++)   // 5786
            {
                object Magic = MirReturnConfigGlobalSeam.g_MagicList[I];   // 5788
                uint magicId = MirReturnConfigGlobalSeam.GetMagicId(Magic);

                int row = Plug.PlugMemoConfig82Add();                      // 5790

                int c0 = Plug.PlugMemoConfig82AddItem(row);                // 5791
                Plug.PlugMemoConfig82SetItemCaption(row, c0, MirReturnConfigGlobalSeam.GetMagicName(Magic));  // 5792
                Plug.PlugMemoConfig82SetItemData(row, c0, magicId);         // 5793
                Plug.PlugMemoConfig82SetItemStyle(row, c0, BsButton);       // 5794 // bsRadio
                Plug.PlugMemoConfig82SetItemAlignment(row, c0, TaLeftJustify);   // 5795
                Plug.PlugMemoConfig82SetItemColor(row, c0, "Up", ViewClWhite);   // 5796
                Plug.PlugMemoConfig82SetItemColor(row, c0, "Hot", ViewClRed);    // 5797 // clWhite
                Plug.PlugMemoConfig82SetItemColor(row, c0, "Down", ViewClRed);   // 5798

                int c1 = Plug.PlugMemoConfig82AddItem(row);                // 5800
                Plug.PlugMemoConfig82SetItemData(row, c1, magicId);         // 5801
                Plug.PlugMemoConfig82SetItemStyle(row, c1, BsCheckBox);     // 5802
                Plug.PlugMemoConfig82SetItemImageType(row, c1, NewopUIPak); // 5803
                Plug.PlugMemoConfig82SetItemImageIndex(row, c1, "Up", 228); // 5804
                Plug.PlugMemoConfig82SetItemImageIndex(row, c1, "Down", 229);   // 5805
                Plug.PlugMemoConfig82SetItemChecked(row, c1,                 // 5806
                    MirReturnGlobalSeam.g_GJUseMagic1.Contains((object)magicId));
            }
        }
        finally
        {
            Plug.PlugMemoConfig82UnLock();                              // 5809
        }

        // ---- 第 2 页（PlugMemoConfig83） ----
        Plug.PlugMemoConfig83Clear();                                      // 5812
        Plug.PlugMemoConfig83Lock();                                       // 5813
        try
        {
            for (int I = 0; I <= MirReturnConfigGlobalSeam.g_MagicList.Count - 1; I++)   // 5815
            {
                object Magic = MirReturnConfigGlobalSeam.g_MagicList[I];   // 5817
                uint magicId = MirReturnConfigGlobalSeam.GetMagicId(Magic);

                int row = Plug.PlugMemoConfig83Add();                      // 5819

                int c0 = Plug.PlugMemoConfig83AddItem(row);                // 5820
                Plug.PlugMemoConfig83SetItemCaption(row, c0, MirReturnConfigGlobalSeam.GetMagicName(Magic));  // 5821
                Plug.PlugMemoConfig83SetItemData(row, c0, magicId);         // 5822
                Plug.PlugMemoConfig83SetItemStyle(row, c0, BsButton);       // 5823 // bsRadio
                Plug.PlugMemoConfig83SetItemAlignment(row, c0, TaLeftJustify);   // 5824
                Plug.PlugMemoConfig83SetItemColor(row, c0, "Up", ViewClWhite);   // 5825
                Plug.PlugMemoConfig83SetItemColor(row, c0, "Hot", ViewClRed);    // 5826 // clWhite
                Plug.PlugMemoConfig83SetItemColor(row, c0, "Down", ViewClRed);   // 5827

                int c1 = Plug.PlugMemoConfig83AddItem(row);                // 5829
                Plug.PlugMemoConfig83SetItemData(row, c1, magicId);         // 5830
                Plug.PlugMemoConfig83SetItemStyle(row, c1, BsCheckBox);     // 5831
                Plug.PlugMemoConfig83SetItemImageType(row, c1, NewopUIPak); // 5832
                Plug.PlugMemoConfig83SetItemImageIndex(row, c1, "Up", 228); // 5833
                Plug.PlugMemoConfig83SetItemImageIndex(row, c1, "Down", 229);   // 5834
                Plug.PlugMemoConfig83SetItemChecked(row, c1,                 // 5835 ★ 原文此句末尾为 ";;"（多余分号）
                    MirReturnGlobalSeam.g_GJUseMagic2.Contains((object)magicId));
            }
        }
        finally
        {
            Plug.PlugMemoConfig83UnLock();                              // 5838
        }
    }
}

/// <summary>
/// 接缝：<c>DBtnDiyMyLoadOrSaveClick</c> 的 Sender 判等（原文 5316 / 5391）。
/// ★ 原文第二处写作小写 <c>plugBtnDiySave</c>（Delphi 大小写不敏感，同一控件）—— 已登记。
/// </summary>
public enum MirReturnDiyButtonId
{
    None = 0,
    PlugBtnDiyLoad,
    /// <summary>原文 5391 写作 <c>plugBtnDiySave</c>（小写首字母）—— 原文如此。</summary>
    PlugBtnDiySave,
}
