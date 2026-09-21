// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   6329-6625  LoadConfigFile
//   6626-6876  SaveConfigFile
//   1309-1393  RefreshUnBindItemList
//   1284-1302  RefreshMySelfMagicList
//
// ⏳ 进度：RefreshUnBindItemList / RefreshMySelfMagicList 已 1:1 移入（见下）；
//   LoadConfigFile / SaveConfigFile 是**最大的两块**（各约 300/250 行）——
//   骨架已就位，逐行体见报告 §未完成/阻塞项。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    // ================================================================================
    // 1284-1393  绑定物品/药品下拉的刷新
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 1284-1302：<c>RefreshMySelfMagicList</c>。
    /// <c>if (g_MySelf &lt;&gt; nil) then</c> →
    ///   记住 <c>PlugComboBoxAutoMagic.ItemIndex</c> → <c>Items.Clear</c> →
    ///   把 <c>g_MagicList</c> 逐项 <c>AddObject(魔法名, 魔法对象)</c> →
    ///   **下标回落**：原下标仍在范围内则还原，否则置 -1；
    /// 最后**无条件** <c>RefreshGJMagic;</c>（在 if 之外 —— 原文如此）。
    /// </summary>
    public override void RefreshMySelfMagicList()
    {
        if (MirActorSeam.MySelfExists())                            // 1288
        {
            int nItemIndex = PlugCtl.PlugComboBoxAutoMagic.ItemIndex;        // 1289
            PlugCtl.PlugComboBoxAutoMagic.Items.Clear();                     // 1290
            for (int I = 0; I <= MirConfigGlobalSeam.g_MagicList.Count - 1; I++)   // 1291
            {
                object magic = MirConfigGlobalSeam.g_MagicList[I];           // 1292
                PlugCtl.PlugComboBoxAutoMagic.Items.AddObject(
                    MirMagicSeam.GetMagicName(magic), magic);                // 1292-1293
            }
            if ((nItemIndex >= 0) && (nItemIndex < PlugCtl.PlugComboBoxAutoMagic.Items.Count))   // 1295
                PlugCtl.PlugComboBoxAutoMagic.ItemIndex = nItemIndex;        // 1296
            else
                PlugCtl.PlugComboBoxAutoMagic.ItemIndex = -1;                // 1298
        }

        RefreshGJMagic();                                                    // 1301
    }

    /// <summary>
    /// 原文 1309-1392：<c>RefreshUnBindItemList</c> —— 按**两个保护开关的组合**把
    /// "小退 / 自定义书类物品" 灌进 HP/MP 保护下拉。四分支逐字照抄：
    ///  ① 两个开关都开 → 清空 + 下标归 -1 + <c>g_Config.Check{HP,MP}Values := -1</c> + 清文本 + <c>Enabled := False</c>；
    ///  ② 只开 <c>boCloseBookProtect</c> → 启用，只塞 <c>'小退'</c>（SameText 命中项）；
    ///  ③ 只开 <c>boCloseLogoutProtect</c> → 启用，塞**非** <c>'小退'</c> 的项，
    ///     再把 <c>g_CustomUnbindItemList</c> 里 <c>t_Book</c> 且**下拉里还没有**的名字补进去；
    ///  ④ 都不开 → <c>Items.Text := g_NGProtectItems.Text</c>（整表），
    ///     若末项是 <c>'小退'</c> 则删掉并记 <c>IsDelExit</c>，补自定义书类，最后把 <c>'小退'</c> 追加回末尾。
    /// ★ 原文缺陷（登记不改）：③ 分支**不**做 <c>'小退'</c> 的删除/回补，
    ///   与 ④ 分支的"删了再加"顺序不同 —— 照抄。
    /// </summary>
    public override void RefreshUnBindItemList()
    {
        var g_ClientConfig = MirConfigGlobalSeam.g_ClientConfig;

        if (g_ClientConfig.boCloseBookProtect && g_ClientConfig.boCloseLogoutProtect)   // 1315
        {
            PlugCtl.PlugComboBoxCheckHPValue.Items.Clear();                    // 1316
            PlugCtl.PlugComboBoxCheckMPValue.Items.Clear();                    // 1317

            PlugCtl.PlugComboBoxCheckHPValue.ItemIndex = -1;                   // 1319
            PlugCtl.PlugComboBoxCheckMPValue.ItemIndex = -1;                   // 1320

            g_Config.CheckHpValues[g_Config.MedicaMode] = -1;                  // 1322
            g_Config.CheckMpValues[g_Config.MedicaMode] = -1;                  // 1323

            PlugCtl.PlugComboBoxCheckHPValue.Text = "";                        // 1325
            PlugCtl.PlugComboBoxCheckMPValue.Text = "";                        // 1326

            PlugCtl.PlugComboBoxCheckHPValue.Enabled = false;                  // 1328
            PlugCtl.PlugComboBoxCheckMPValue.Enabled = false;                  // 1329
        }
        else if (g_ClientConfig.boCloseBookProtect)                            // 1330
        {
            PlugCtl.PlugComboBoxCheckHPValue.Enabled = true;                   // 1331
            PlugCtl.PlugComboBoxCheckMPValue.Enabled = true;                   // 1332

            PlugCtl.PlugComboBoxCheckHPValue.Items.Clear();                    // 1334
            PlugCtl.PlugComboBoxCheckMPValue.Items.Clear();                    // 1335

            for (int I = 0; I <= MirConfigGlobalSeam.g_NGProtectItems.Count - 1; I++)   // 1337
            {
                if (MirConfigGlobalSeam.SameText(MirConfigGlobalSeam.g_NGProtectItems[I], "小退"))   // 1338
                {
                    PlugCtl.PlugComboBoxCheckHPValue.Items.Add(MirConfigGlobalSeam.g_NGProtectItems[I]);   // 1339
                    PlugCtl.PlugComboBoxCheckMPValue.Items.Add(MirConfigGlobalSeam.g_NGProtectItems[I]);   // 1340
                }
            }
        }
        else if (g_ClientConfig.boCloseLogoutProtect)                          // 1343
        {
            PlugCtl.PlugComboBoxCheckHPValue.Enabled = true;                   // 1344
            PlugCtl.PlugComboBoxCheckMPValue.Enabled = true;                   // 1345

            PlugCtl.PlugComboBoxCheckHPValue.Items.Clear();                    // 1347
            PlugCtl.PlugComboBoxCheckMPValue.Items.Clear();                    // 1348

            for (int I = 0; I <= MirConfigGlobalSeam.g_NGProtectItems.Count - 1; I++)   // 1350
            {
                if (!MirConfigGlobalSeam.SameText(MirConfigGlobalSeam.g_NGProtectItems[I], "小退"))   // 1351
                {
                    PlugCtl.PlugComboBoxCheckHPValue.Items.Add(MirConfigGlobalSeam.g_NGProtectItems[I]);   // 1352
                    PlugCtl.PlugComboBoxCheckMPValue.Items.Add(MirConfigGlobalSeam.g_NGProtectItems[I]);   // 1353
                }
            }

            for (int I = 0; I <= MirConfigGlobalSeam.g_CustomUnbindItemList.Count - 1; I++)   // 1357
            {
                var BindItem = (IMirCustomBindItem)MirConfigGlobalSeam.g_CustomUnbindItemList[I];                  // 1358
                if (BindItem.UnBindItemType == TUnBindItemType.t_Book)                          // 1359
                {
                    if (PlugCtl.PlugComboBoxCheckHPValue.Items.IndexOf(BindItem.sItemName) < 0)   // 1360
                    {
                        PlugCtl.PlugComboBoxCheckHPValue.Items.Add(BindItem.sItemName);           // 1361
                        PlugCtl.PlugComboBoxCheckMPValue.Items.Add(BindItem.sItemName);           // 1362
                    }
                }
            }
        }
        else                                                                    // 1366
        {
            PlugCtl.PlugComboBoxCheckHPValue.Items.Text = MirConfigGlobalSeam.g_NGProtectItems.Text;   // 1367
            PlugCtl.PlugComboBoxCheckMPValue.Items.Text = MirConfigGlobalSeam.g_NGProtectItems.Text;   // 1368

            bool IsDelExit = false;                                             // 1370
            if ((MirConfigGlobalSeam.g_NGProtectItems.Count > 0) &&
                MirConfigGlobalSeam.SameText(
                    MirConfigGlobalSeam.g_NGProtectItems[MirConfigGlobalSeam.g_NGProtectItems.Count - 1], "小退"))   // 1371
            {
                PlugCtl.PlugComboBoxCheckHPValue.Items.Delete(PlugCtl.PlugComboBoxCheckHPValue.Items.Count - 1);   // 1372
                PlugCtl.PlugComboBoxCheckMPValue.Items.Delete(PlugCtl.PlugComboBoxCheckMPValue.Items.Count - 1);   // 1373
                IsDelExit = true;                                              // 1374
            }

            for (int I = 0; I <= MirConfigGlobalSeam.g_CustomUnbindItemList.Count - 1; I++)   // 1377
            {
                var BindItem = (IMirCustomBindItem)MirConfigGlobalSeam.g_CustomUnbindItemList[I];                  // 1378
                if (BindItem.UnBindItemType == TUnBindItemType.t_Book)                          // 1379
                {
                    if (PlugCtl.PlugComboBoxCheckHPValue.Items.IndexOf(BindItem.sItemName) < 0)   // 1380
                    {
                        PlugCtl.PlugComboBoxCheckHPValue.Items.Add(BindItem.sItemName);           // 1381
                        PlugCtl.PlugComboBoxCheckMPValue.Items.Add(BindItem.sItemName);           // 1382
                    }
                }
            }

            if (IsDelExit)                                                      // 1387
            {
                PlugCtl.PlugComboBoxCheckHPValue.Items.Add("小退");               // 1388
                PlugCtl.PlugComboBoxCheckMPValue.Items.Add("小退");               // 1389
            }
        }
    }

    // ================================================================================
    // 6329-6876  配置文件读写（本单元最大的两块）
    // ⏳ 骨架
    // ================================================================================

    /// <summary>原文 6329-6625：<c>LoadConfigFile</c>（逐字段 ReadInteger/ReadBool/ReadString）。</summary>
    public void LoadConfigFile() => NotPorted(nameof(LoadConfigFile), 6329);

    /// <summary>原文 6626-6876：<c>SaveConfigFile</c>（逐字段 WriteInteger/WriteBool/WriteString）。</summary>
    public void SaveConfigFile() => NotPorted(nameof(SaveConfigFile), 6626);
}

/// <summary>
/// 接缝：<c>pTClientMagic</c> 的取名字器（原文 <c>pTClientMagic(...).Def.sMagicName</c>）。
/// 与 MirReturnConfigGlobalSeam.GetMagicName 同一做法，避免复制第二份实现。
/// </summary>
public static class MirMagicSeam
{
    /// <summary>接缝：<c>pTClientMagic.Def.sMagicName</c>。</summary>
    public static Func<object, string> GetMagicName = _ => "";
    /// <summary>接缝：<c>pTClientMagic.Def.wMagicId</c>。</summary>
    public static Func<object, uint> GetMagicId = _ => 0u;
}
