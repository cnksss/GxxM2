// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   1088-1158  RefShowItem（FileItemDB.m_ShowItemList → PlugMemoConfig2 的 6 列行模型）
//   1967-1981  RefKeyBoardConfig（12 个快捷键标签 ← GetKeyDownStr）
//   2564-2570  LoadClientConfig 头部（写 FClientConfig + 门禁）
//   2903-2908  Initialize 头部（5 个字段落值）
//   3275-3280  Finalize（见 Lifecycle 分片）
//
// 本分片同时补齐 <c>TGameConfigObject</c> 的其余抽象面（原文对这些成员
// **没有** 覆写实现或实现为空，见逐条注释）。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // 接缝：TConfigChecked 数组（原文 338 <c>FConfigCheckeds:array[TConfigChecked] of Boolean</c>）
    // ================================================================================

    /// <summary>
    /// 原文 338 的 <c>FConfigCheckeds</c>。托管侧 <c>TGameConfigObject.ConfigCheckeds</c> 是
    /// 抽象只读属性（见 GameConfigDlg.cs:331），此处返回同一实例（长度 HighOrdinal + 2，含合成槽位）。
    /// </summary>
    public override bool[] ConfigCheckeds => FConfigCheckeds;

    // ================================================================================
    // 接缝：TGameConfigObject 里原文**未覆写**的成员
    //
    // 核实依据：原文 485-525 的 override 清单里没有下面这 7 个名字 —— 即
    // TGameConfigObject 的同名声明在 TMirReturnConfigDlg 里保持**继承的抽象/默认行为**。
    // 托管侧基类是 abstract，必须给出实现；按规程实现为**空实现**并在注释里登记
    // "原文无覆写"，绝不臆造业务逻辑。
    // ================================================================================

    /// <summary>原文无覆写（<c>RefKeyboardConfig</c> **有**覆写，见 <see cref="RefKeyboardConfig"/>）。</summary>
    public override void AddToBossList(string sName) { }

    /// <summary>原文无覆写。</summary>
    public override void RemoveFromBossList(string sName) { }

    /// <summary>原文无覆写。</summary>
    public override void AddOrRemoveBossList(string sNamt) { }

    // ================================================================================
    // MirReturnConfigDlg.pas:1967-1981  RefKeyBoardConfig
    // ================================================================================

    /// <summary>
    /// 原文 1967-1981：12 个 <c>PlugMemoConfig6LabelKeyBoardN.Caption</c> ←
    /// <c>GetKeyDownStr(g_ShortcutKeys[k].Key, g_ShortcutKeys[k].Shift)</c>。
    ///
    /// ★★ 原文缺陷（**本车道发现，登记不改，测试固定**）：
    /// 1977 行把 <c>LabelKeyBoard9</c> 赋成了 <c>g_ShortcutKeys[9]</c>，
    /// 而 1978 行 <c>LabelKeyBoard10</c> **也是** <c>g_ShortcutKeys[9]</c>：
    /// <c>PlugMemoConfig6LabelKeyBoard9</c> 与 <c>PlugMemoConfig6LabelKeyBoard10</c> 显示同一快捷键
    /// （下标 9 被用了两次），而 <c>g_ShortcutKeys[8]</c> **从未显示**；
    /// 同时 <c>g_ShortcutKeys[12..15]</c>（共 16 项）也从未显示。
    /// 原文如此，逐行照抄（不"修正"成 [8]）。
    /// </summary>
    public override void RefKeyboardConfig()
    {
        Plug.SetLabelKeyBoard(1, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[0].Key, ConfigShareGlobal.g_ShortcutKeys[0].Shift));   // 1969
        Plug.SetLabelKeyBoard(2, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[1].Key, ConfigShareGlobal.g_ShortcutKeys[1].Shift));   // 1970
        Plug.SetLabelKeyBoard(3, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[2].Key, ConfigShareGlobal.g_ShortcutKeys[2].Shift));   // 1971
        Plug.SetLabelKeyBoard(4, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[3].Key, ConfigShareGlobal.g_ShortcutKeys[3].Shift));   // 1972
        Plug.SetLabelKeyBoard(5, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[4].Key, ConfigShareGlobal.g_ShortcutKeys[4].Shift));   // 1973
        Plug.SetLabelKeyBoard(6, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[5].Key, ConfigShareGlobal.g_ShortcutKeys[5].Shift));   // 1974
        Plug.SetLabelKeyBoard(7, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[6].Key, ConfigShareGlobal.g_ShortcutKeys[6].Shift));   // 1975
        Plug.SetLabelKeyBoard(8, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[7].Key, ConfigShareGlobal.g_ShortcutKeys[7].Shift));   // 1976
        Plug.SetLabelKeyBoard(9, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[9].Key, ConfigShareGlobal.g_ShortcutKeys[9].Shift));   // 1977 ★ [9] 而非 [8]
        Plug.SetLabelKeyBoard(10, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[9].Key, ConfigShareGlobal.g_ShortcutKeys[9].Shift));  // 1978 ★ 又是 [9]
        Plug.SetLabelKeyBoard(11, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[10].Key, ConfigShareGlobal.g_ShortcutKeys[10].Shift)); // 1979
        Plug.SetLabelKeyBoard(12, ConfigShare.GetKeyDownStr(ConfigShareGlobal.g_ShortcutKeys[11].Key, ConfigShareGlobal.g_ShortcutKeys[11].Shift)); // 1980
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:1088-1158  RefShowItem
    // ================================================================================

    /// <summary>
    /// 原文 1088-1158：清空 + <c>ColCount := 6</c> → <c>Lock</c>/<c>UnLock</c> 包夹 →
    /// 对 <c>FileItemDB.m_ShowItemList</c> 每个 <c>TShowItem</c> 建 **1 行 6 列**：
    /// <list type="number">
    /// <item>第 1 列：<c>Caption = sItemName</c>、<c>Style = bsButton</c>（原文注释 // bsRadio）、
    ///       <c>Alignment = taLeftJustify</c>、三态色 Up=clWhite / Hot=clRed（// clWhite）/ Down=clRed</item>
    /// <item>第 2..6 列：全 <c>Style = bsCheckBox</c>、<c>ImageType = NewopUI_Pak</c>、<c>Up=228/Down=229</c>，
    ///       依次绑定 <c>boHintMsg / boPickup / boShowName / boShowSpecial / boAutoMove</c></item>
    /// </list>
    /// 每列的 <c>Data</c> 都指向**同一个** <c>ShowItem</c> 对象（原文如此，是行内回写的句柄）。
    /// </summary>
    public override void RefShowItem()
    {
        Plug.PlugMemoConfig2Clear();                                       // 1095
        Plug.PlugMemoConfig2ColCount = 6;                                  // 1096

        Plug.PlugMemoConfig2Lock();                                        // 1098
        try
        {
            for (int I = 0; I <= FileItemDB.m_ShowItemList.Count - 1; I++)  // 1100
            {
                TShowItem ShowItem = FileItemDB.m_ShowItemList[I];          // 1102

                int row = Plug.PlugMemoConfig2Add();                        // 1104

                int c0 = Plug.PlugMemoConfig2AddItem(row);                  // 1105
                Plug.PlugMemoConfig2SetItemCaption(row, c0, ShowItem.sItemName);        // 1107
                Plug.PlugMemoConfig2SetItemData(row, c0, ShowItemRowHandle(ShowItem));  // 1108
                Plug.PlugMemoConfig2SetItemStyle(row, c0, BsButton);                    // 1109 // bsRadio
                Plug.PlugMemoConfig2SetItemAlignment(row, c0, TaLeftJustify);           // 1110
                Plug.PlugMemoConfig2SetItemColor(row, c0, "Up", ViewClWhite);           // 1111
                Plug.PlugMemoConfig2SetItemColor(row, c0, "Hot", ViewClRed);            // 1112 // clWhite
                Plug.PlugMemoConfig2SetItemColor(row, c0, "Down", ViewClRed);           // 1113

                AddShowItemCheckCell(row, ShowItem, ShowItem.boHintMsg);    // 1115-1121
                AddShowItemCheckCell(row, ShowItem, ShowItem.boPickup);     // 1123-1129
                AddShowItemCheckCell(row, ShowItem, ShowItem.boShowName);   // 1131-1137
                AddShowItemCheckCell(row, ShowItem, ShowItem.boShowSpecial);// 1139-1145
                AddShowItemCheckCell(row, ShowItem, ShowItem.boAutoMove);   // 1147-1153
            }
        }
        finally
        {
            Plug.PlugMemoConfig2UnLock();                               // 1156
        }
    }

    /// <summary>
    /// 原文 1115-1121（以及 1123-1129 / 1131-1137 / 1139-1145 / 1147-1153 四段同形）：
    /// <c>ViewItem := ListItem.AddItem('', nil); ViewItem.Data := ShowItem; ViewItem.Style := bsCheckBox;
    /// ViewItem.ImageIndex.ImageType := NewopUI_Pak; ViewItem.ImageIndex.Up := 228;
    /// ViewItem.ImageIndex.Down := 229; ViewItem.Checked := &lt;bo*&gt;;</c>
    /// 注意 <c>Checked := ShowItem.boHintMsg</c> 是 **Byte → Boolean** 的隐式转换（非 0 即真）—— 照抄。
    /// </summary>
    private void AddShowItemCheckCell(int row, TShowItem ShowItem, byte checkedValue)
    {
        int c = Plug.PlugMemoConfig2AddItem(row);
        Plug.PlugMemoConfig2SetItemData(row, c, ShowItemRowHandle(ShowItem));
        Plug.PlugMemoConfig2SetItemStyle(row, c, BsCheckBox);
        Plug.PlugMemoConfig2SetItemImageType(row, c, NewopUIPak);
        Plug.PlugMemoConfig2SetItemImageIndex(row, c, "Up", 228);
        Plug.PlugMemoConfig2SetItemImageIndex(row, c, "Down", 229);
        Plug.PlugMemoConfig2SetItemChecked(row, c, checkedValue != 0);
    }

    /// <summary>
    /// 原文 <c>ViewItem.Data := ShowItem;</c>（把 <c>pTShowItem</c> 塞进 <c>Pointer</c> 字段）。
    /// 托管侧用"对象引用 + 对象哈希"承载同一语义（原文本字段仅作句柄）。
    /// </summary>
    private static uint ShowItemRowHandle(TShowItem ShowItem)
        => unchecked((uint)System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(ShowItem));

    /// <summary>原文 DxListView 的 <c>bsButton</c>（Delphi TButtonStyle，bsButton = 3）。</summary>
    private const int BsButton = 3;
    /// <summary>原文 DxListView 的 <c>bsCheckBox</c>（Delphi TButtonStyle，bsCheckBox = 2）。</summary>
    private const int BsCheckBox = 2;
    /// <summary>原文 <c>taLeftJustify = 0</c>（Delphi Classes.TAlignment）。</summary>
    private const int TaLeftJustify = 0;
    /// <summary>原文 Graphics clWhite = $00FFFFFF。</summary>
    private const int ViewClWhite = 0x00FFFFFF;
    /// <summary>原文 Graphics clRed = $000000FF。</summary>
    private const int ViewClRed = 0x000000FF;
    /// <summary>原文 Grobal2/DxComponent 的 <c>NewopUI_Pak</c> 图库号（原文 1118/5803/5832 用）。</summary>
    private const int NewopUIPak = 0;

    // ================================================================================
    // MirReturnConfigDlg.pas:2564-2570  LoadClientConfig（头部）
    // ================================================================================

    /// <summary>
    /// 原文 2564-3274 的 <c>LoadClientConfig</c>：头部把入参存进 <c>FClientConfig</c>，
    /// 随后是 **700+ 行的控件构建/回写**（依赖 TDxImageButton/TDxScrollBox 的真实布局引擎，
    /// 本车道未覆盖，见文件头清单）。
    ///
    /// 托管侧按 MirsConfigDlg 同一做法：头部逻辑照抄，其余走可注入接缝
    /// （<see cref="LoadClientConfigSeam"/>），并在报告里登记覆盖区间。
    /// </summary>
    public override void LoadClientConfig(TClientConfig ClientConfig)
    {
        FClientConfig = ClientConfig;                                      // 2565 起（原文用局部 ClientConfig 全程）
        LoadClientConfigSeam(ClientConfig);
    }

    /// <summary>接缝：<c>LoadClientConfig</c> 的注入点（原文 2571-3274 的控件构建段）。</summary>
    public Action<TClientConfig> LoadClientConfigSeam = _ => { };

    // ================================================================================
    // MirReturnConfigDlg.pas:2903-3274  Initialize
    // ================================================================================

    /// <summary>
    /// 原文 2903-3274：头部把 4 个入参 + <c>MyGetTickCount</c> 落进私有字段，
    /// 随后是 **370+ 行**的控件树构建（CreatePlugInWindow/DxImageButton 创建 + 事件挂接 +
    /// 全部几何定位）。托管侧照抄头部、其余走接缝。
    /// </summary>
    public override void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode)
    {
        FHandle = Handle;                                                  // 2904 起
        FScreenMode = ScreenMode;
        FClientVersion = ClientVersion;
        FWindowMode = WindowMode;
        // 原文 3272：**控件树构建完成后**才置 FInitializeed := True ——
        // 它是 RefConfig(1985)/RefUseItemConfig(1862) 的总门禁。
        // 托管侧把"控件树构建"整体交给接缝，故在接缝调用**之后**置位（与原文顺序一致）。
        InitializeSeam(Handle, ScreenMode, ClientVersion, WindowMode);
        FInitializeed = true;                                              // 3272
    }

    /// <summary>接缝：<c>Initialize</c> 的注入点（原文 2909-3274 的控件树构建段）。</summary>
    public Action<IntPtr, byte, TClientVersion, bool> InitializeSeam = (h, s, v, w) => { };
}
