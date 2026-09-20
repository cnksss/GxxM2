// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本文件承载该单元的**外部依赖接缝**（均在独占区 GXX.Client/GUI/GameConfig/MirReturn*.cs 内）：
//   MirReturnCheckedMap                      原文 TConfigChecked 缺失成员的显式映射（原文 844/859/1661）
//   IMirReturnConfigDlgControls              控件访问面**基础部分**（行模型/Lock/焦点等"带操作"的成员）
//   MirReturnConfigDlgControlsStub           纯内存默认实现（无 WinForms 依赖）
//   MirReturnGlobalSeam                      原文 MShare/ConfigShare/ClMain 侧全局量
//   MirReturnIniFile / MirReturnIniFileStub  IniFiles.pas 的 TIniFile 接缝
//   MirReturnMessageSeam                     FrmDlg.DMessageDlg 的消息框接缝（无头安全）
//
// ★ 控件取值面属性的**主体**在脚本生成文件 MirReturnControls.g.cs /
//   MirReturnControlsStub.g.cs（原文 10-328 的 319 个控件字段 + 1986-2136 的 77 条 RefConfig 赋值），
//   本文件只保留"生成器无法表达"的部分（方法族：Add/Lock/Clear/LoadFromFile/Set* 与焦点三段）。
//
// ★ 命名冲突规避（工程已发生 5 次跨车道类型重名事故）：
//   本文件**不**声明 TDxEdit/TDxListView/TDxScrollBox/TDxImageForm/TDxChatMemo/TDxLabel/
//   TDxImageButton/TDxTrackBar 等任何 DxComponent 类型 —— 原文控件全部以**取值面属性**表达
//   （只暴露 Checked/Value/ItemIndex/Text/Lines/Enabled/Visible/SetFocus 这些原语），
//   因此不与 src/GXX.Client/DxComponent/**、src/GXX.Client/LoadDx/**、
//   src/GXX.Client/GUI/{Share,Mir,DxComponent}/** 里同名类型产生 CS0101/CS0104。

using System;
using System.Collections.Generic;

namespace GXX.Client.GUI.GameConfig.Seams;

/// <summary>
/// MirReturnConfigDlg.pas 独有的 <c>TConfigChecked</c> 成员。
///
/// **原文不一致（本车道核实）**：<c>Source/Client-HGE/GameConfig/Common/GameConfigDlg.pas:26-249</c>
/// 的 <c>TConfigChecked</c> **缺**下列 7 个 MirReturnConfigDlg.pas 直接使用的成员：
/// <c>ckAutoTakeOnItem</c>(844/1588/2057)、<c>ckMovePick</c>(859/1661)、
/// <c>ckShowItemName</c>(1460/1990)、<c>ckShowFilterItem</c>(1464/1991)、
/// <c>ckItemHint</c>(1468/1992)、<c>ckGJ_DFAvoid</c>(1833/2128)、<c>ckAutoChangePoison</c>(1592/2058)。
/// 同名枚举在套件内存在多个变体（Mir/MirJSY/MirReturn/Mirs 各自引用的成员集合都不同），
/// 与 MirsConfigDlg 车道（见 <see cref="MirsConfigCheckedAlias"/>）遇到的是同一类问题。
///
/// 处理方式与 MirsConfigDlg **完全一致**（不改基线、不臆造枚举顺序）：
/// 定义独立扩展枚举 + 显式映射表；无任何语义等价成员者分配**超出真实枚举范围的合成槽位**。
/// </summary>
public enum MirReturnCheckedAlias
{
    /// <summary>原文 844/1588/2057。注释原文即"毒符互换"。</summary>
    ckAutoTakeOnItem = 0,
    /// <summary>原文 859/1661。检出枚举中无等价成员（见合成槽位说明）。</summary>
    ckMovePick = 1,
    /// <summary>原文 1460/1990（PlugCheckBoxShowItemName）。</summary>
    ckShowItemName = 2,
    /// <summary>原文 1464/1991（PlugCheckBoxShowFilterItem）。</summary>
    ckShowFilterItem = 3,
    /// <summary>原文 1468/1992（PlugCheckBoxItemHint）。</summary>
    ckItemHint = 4,
    /// <summary>原文 1833/2128（PlugCheckBoxDFAvoid）。</summary>
    ckGJ_DFAvoid = 5,
    /// <summary>原文 1592/2058（PlugCheckBoxAutoChangePoison）。</summary>
    ckAutoChangePoison = 6,
}

/// <summary>
/// MirReturnConfigDlg 的 <c>TConfigChecked</c> 缺失成员 → 检出枚举成员的**显式映射表**。
/// 全部映射以"同名/同义控件 + 原文注释"为依据，并在注释里给出原文行号。
/// </summary>
public static class MirReturnCheckedMap
{
    /// <summary>ckAutoTakeOnItem → ckAutoCHangePoison（原文注释："毒符互换"；MirsConfigDlg 车道同一裁定）</summary>
    public const TConfigChecked ckAutoTakeOnItem = TConfigChecked.ckAutoCHangePoison;

    /// <summary>ckShowItemName → ckShowMonName（检出枚举无"显示物品名"位；与 MirsConfigDlg 同一裁定）</summary>
    public const TConfigChecked ckShowItemName = TConfigChecked.ckShowMonName;

    /// <summary>ckShowFilterItem → ckShowMonName（检出枚举无"过滤物品显示"位）</summary>
    public const TConfigChecked ckShowFilterItem = TConfigChecked.ckShowMonName;

    /// <summary>ckItemHint → ckShowHPLabel（原文 GameConfigDlg.pas:28 注释即把 PlugCheckBoxItemHint 标在 ckShowHPLabel 上）</summary>
    public const TConfigChecked ckItemHint = TConfigChecked.ckShowHPLabel;

    /// <summary>ckGJ_DFAvoid → ckNotParaly（道法"不躲避"与"防止石化"同族；检出枚举无 DFAvoid 位）</summary>
    public const TConfigChecked ckGJ_DFAvoid = TConfigChecked.ckNotParaly;

    /// <summary>ckAutoChangePoison → ckAutoCHangePoison（同义成员）</summary>
    public const TConfigChecked ckAutoChangePoison = TConfigChecked.ckAutoCHangePoison;

    /// <summary>
    /// ckMovePick 在检出枚举中**没有任何语义等价成员**（它原义是"移动捡取"，
    /// 与 ckAutoPickUpItem"自动捡取"不同源）。若强行映射到 ckAutoPickUpItem，
    /// 构造函数第 859 行的 <c>FConfigCheckeds[ckMovePick] := False</c> 会把第 823 行
    /// 刚置 True 的 ckAutoPickUpItem 覆盖掉 —— 与原意不符。
    /// 因此分配**超出真实枚举范围的合成槽位**（数组长度 = HighOrdinal + 2）。
    /// </summary>
    public const int ckMovePickSyntheticIndex = TConfigCheckedBounds.HighOrdinal + 1;

    /// <summary>ckMovePick → 合成槽位（见 <see cref="ckMovePickSyntheticIndex"/>），本身不是 TConfigChecked。</summary>
    public static int ckMovePickIndex => ckMovePickSyntheticIndex;

    /// <summary>映射依据说明（供测试逐条断言）。</summary>
    public static string Describe(MirReturnCheckedAlias a) => a switch
    {
        MirReturnCheckedAlias.ckAutoTakeOnItem =>
            "ckAutoTakeOnItem -> ckAutoCHangePoison (MirReturnConfigDlg.pas:844/1588/2057)",
        MirReturnCheckedAlias.ckMovePick =>
            "ckMovePick -> 合成槽位 index=" + ckMovePickSyntheticIndex + "（无等价成员，MirReturnConfigDlg.pas:859/1661）",
        MirReturnCheckedAlias.ckShowItemName =>
            "ckShowItemName -> ckShowMonName (MirReturnConfigDlg.pas:1460/1990)",
        MirReturnCheckedAlias.ckShowFilterItem =>
            "ckShowFilterItem -> ckShowMonName (MirReturnConfigDlg.pas:1464/1991)",
        MirReturnCheckedAlias.ckItemHint =>
            "ckItemHint -> ckShowHPLabel (MirReturnConfigDlg.pas:1468/1992)",
        MirReturnCheckedAlias.ckGJ_DFAvoid =>
            "ckGJ_DFAvoid -> ckNotParaly (MirReturnConfigDlg.pas:1833/2128)",
        MirReturnCheckedAlias.ckAutoChangePoison =>
            "ckAutoChangePoison -> ckAutoCHangePoison (MirReturnConfigDlg.pas:1592/2058)",
        _ => "",
    };
}

/// <summary>
/// 接缝：<c>TMirReturnConfigDlg</c> 的界面控件访问面——**基础部分**。
///
/// 原文这些控件全部是 DxComponent 自绘控件（行 10-328 声明），该控件库在托管侧尚无
/// 可复用的真实实现（同名类型分散在 LoadDx / GUI.Share / GUI.Mir 三处且互相冲突）。
/// 按规程定义最小接缝：只暴露配置逻辑真正读写的原语成员，控件对象由宿主/测试注入。
///
/// **取值面属性**（<c>PlugXxx.Checked/Value/ItemIndex/Text/Enabled</c> → 同名 C# 属性）
/// 在生成文件 <c>MirReturnControls.g.cs</c> 的 <see cref="IMirReturnConfigDlgControlsExt"/> 里；
/// 本接口只保留"带操作"的成员（行模型 Add/Set*/Lock/Clear、列表 Load/Save、焦点三段）。
/// </summary>
public partial interface IMirReturnConfigDlgControls
{
    // ---- 容器/页控制 ----
    /// <summary>接缝：<c>PlugConfigDlg:TDxImageForm</c> 是否存在（原文以 <c>&lt;&gt; nil</c> 判定）。</summary>
    object PlugConfigDlg { get; }
    /// <summary>接缝：<c>PlugConfigDlg.Visible</c>（原文 900/913）。</summary>
    bool PlugConfigDlgVisible { get; set; }
    /// <summary>接缝：<c>PlugPageControlConfig.Width</c>（原文 1007）。</summary>
    int PlugPageControlConfigWidth { get; }
    /// <summary>接缝：<c>PlugPageControlConfig.Height</c>（原文 1007）。</summary>
    int PlugPageControlConfigHeight { get; }
    /// <summary>接缝：<c>PlugPageControlConfig.ActivePageIndex</c>（原文 1018）。</summary>
    int PlugPageControlConfigActivePageIndex { get; set; }

    // ---- 焦点三段（910-927 / 1018-1026） ----
    /// <summary>接缝：<c>PlugCheckBoxShowHPLabel.Visible</c>。</summary>
    bool PlugCheckBoxShowHPLabelVisible { get; }
    /// <summary>接缝：<c>PlugCheckBoxShowHPLabel.SetFocus</c>。</summary>
    void PlugCheckBoxShowHPLabelSetFocus();
    /// <summary>接缝：<c>PlugCheckBoxNumberLable.Visible</c>。</summary>
    bool PlugCheckBoxNumberLableVisible { get; }
    /// <summary>接缝：<c>PlugCheckBoxNumberLable.SetFocus</c>。</summary>
    void PlugCheckBoxNumberLableSetFocus();
    /// <summary>接缝：<c>PlugCheckBoxJobAndLevel.Visible</c>。</summary>
    bool PlugCheckBoxJobAndLevelVisible { get; }
    /// <summary>接缝：<c>PlugCheckBoxJobAndLevel.SetFocus</c>。</summary>
    void PlugCheckBoxJobAndLevelSetFocus();

    // ---- 物品过滤页行模型（PlugMemoConfig2:TDxListView，原文 1095-1157 / 1365-1425 / 5322-5383） ----
    /// <summary>接缝：<c>PlugMemoConfig2.Clear</c>。</summary>
    void PlugMemoConfig2Clear();
    /// <summary>接缝：<c>PlugMemoConfig2.ColCount</c>（ClearShowItem 在 1085 置 6）。</summary>
    int PlugMemoConfig2ColCount { get; set; }
    /// <summary>接缝：<c>PlugMemoConfig2.Last</c>（原文 5288/5302）。</summary>
    void PlugMemoConfig2Last();
    /// <summary>接缝：<c>PlugMemoConfig2.Lock</c>。</summary>
    void PlugMemoConfig2Lock();
    /// <summary>接缝：<c>PlugMemoConfig2.UnLock</c>。</summary>
    void PlugMemoConfig2UnLock();
    /// <summary>接缝：<c>PlugMemoConfig2.Add</c>（返回新行下标）。</summary>
    int PlugMemoConfig2Add();
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].AddItem('', nil)</c>（返回新列下标）。</summary>
    int PlugMemoConfig2AddItem(int rowIndex);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Caption</c>。</summary>
    void PlugMemoConfig2SetItemCaption(int rowIndex, int colIndex, string caption);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Data</c>。</summary>
    void PlugMemoConfig2SetItemData(int rowIndex, int colIndex, uint data);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Style</c>。</summary>
    void PlugMemoConfig2SetItemStyle(int rowIndex, int colIndex, int style);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Alignment</c>。</summary>
    void PlugMemoConfig2SetItemAlignment(int rowIndex, int colIndex, int alignment);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Color.{Up,Hot,Down}.Color</c>。</summary>
    void PlugMemoConfig2SetItemColor(int rowIndex, int colIndex, string state, int color);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].ImageIndex.ImageType</c>。</summary>
    void PlugMemoConfig2SetItemImageType(int rowIndex, int colIndex, int imageType);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].ImageIndex.{Up,Down}</c>。</summary>
    void PlugMemoConfig2SetItemImageIndex(int rowIndex, int colIndex, string state, int value);
    /// <summary>接缝：<c>PlugMemoConfig2.Items[R].Items[C].Checked</c>。</summary>
    void PlugMemoConfig2SetItemChecked(int rowIndex, int colIndex, bool value);
    /// <summary>接缝：<c>PlugMemoConfig2.Count</c>（行数；DTO 用，与生成属性同源）。</summary>
    int PlugMemoConfig2Count { get; }

    // ---- 快捷键标签（原文 1969-1980） ----
    /// <summary>接缝：<c>PlugMemoConfig6LabelKeyBoard1..12.Caption</c>。</summary>
    void SetLabelKeyBoard(int index1to12, string caption);
    /// <summary>接缝：<c>PlugMemoConfig4Label3/4.Caption</c>（原文 1867/1869/1874/1876）。</summary>
    void SetMemoConfig4Label(int index3or4, string caption);

    // ---- 超药索引族（原文 2495-2538 的 9 路 Sender 判等；索引形式便于与原文分支对照） ----
    /// <summary>接缝：<c>PlugCheckBoxUseSuperMedicaItemName0..8.Checked</c> 的索引读取。</summary>
    bool PlugCheckBoxUseSuperMedicaItemName(int index);
    /// <summary>接缝：<c>PlugCheckBoxUseSuperMedicaItemName0..8.Checked</c> 的索引写入。</summary>
    void SetPlugCheckBoxUseSuperMedicaItemName(int index, bool value);
    /// <summary>接缝：<c>PlugEditSuperMedicaHP0..8.Value</c> 的索引读取。</summary>
    int PlugEditSuperMedicaHP(int index);
    /// <summary>接缝：<c>PlugEditSuperMedicaHPTime0..8.Value</c> 的索引读取。</summary>
    int PlugEditSuperMedicaHPTime(int index);
    /// <summary>接缝：<c>PlugEditSuperMedicaMP0..8.Value</c> 的索引读取。</summary>
    int PlugEditSuperMedicaMP(int index);
    /// <summary>接缝：<c>PlugEditSuperMedicaMPTime0..8.Value</c> 的索引读取。</summary>
    int PlugEditSuperMedicaMPTime(int index);
    /// <summary>接缝：<c>PlugEditSuperMedicaHPTimeN.Value := …</c>（原文 2371 的回写）。</summary>
    void SetPlugEditSuperMedicaHPTime(int index, int value);
    /// <summary>接缝：<c>PlugEditSuperMedicaMPTimeN.Value := …</c>（原文 2485 的回写）。</summary>
    void SetPlugEditSuperMedicaMPTime(int index, int value);
    /// <summary>接缝：<c>PlugEditSuperMedicaHPN.Value := …</c>（原文 5913-5920，**控件下标 0..7**）。</summary>
    void SetPlugEditSuperMedicaHP(int index, int value);
    /// <summary>接缝：<c>PlugEditSuperMedicaMPN.Value := …</c>（原文 5931-5938，**控件下标 0..7**）。</summary>
    void SetPlugEditSuperMedicaMP(int index, int value);

    // ---- 自动练功下拉（原文 979-990 / 4464-4470） ----
    /// <summary>接缝：<c>PlugComboBoxAutoMagic.ItemIndex</c> 的**可空**访问（Delphi 里 -1 = 无选中）。</summary>
    int PlugComboBoxAutoMagicItemIndex { get; set; }
    /// <summary>接缝：<c>PlugComboBoxAutoMagic.Items.Count</c>。</summary>
    int PlugComboBoxAutoMagicItemsCount { get; }
    /// <summary>接缝：<c>PlugComboBoxAutoMagic.Items.Clear</c>。</summary>
    void PlugComboBoxAutoMagicItemsClear();
    /// <summary>接缝：<c>PlugComboBoxAutoMagic.Items.AddObject(name, obj)</c>。</summary>
    void PlugComboBoxAutoMagicItemsAddObject(string name, object obj);
    /// <summary>接缝：<c>PlugComboBoxAutoMagic.Items.Objects[index]</c>。</summary>
    object PlugComboBoxAutoMagicItemsObject(int index);

    // ---- 挂机动作下拉条目文本（原文 4781-4785，来源 g_GJActionMode.Text） ----
    /// <summary>接缝：<c>PlugComboBoxNoRedPoisonValue.Items.Text</c>。</summary>
    string PlugComboBoxNoRedPoisonValueItemsText { get; set; }
    /// <summary>接缝：<c>PlugComboBoxNoBluePoisonValue.Items.Text</c>。</summary>
    string PlugComboBoxNoBluePoisonValueItemsText { get; set; }
    /// <summary>接缝：<c>PlugComboBoxNoDuFuValue.Items.Text</c>。</summary>
    string PlugComboBoxNoDuFuValueItemsText { get; set; }
    /// <summary>接缝：<c>PlugComboBoxBagFullValue.Items.Text</c>。</summary>
    string PlugComboBoxBagFullValueItemsText { get; set; }
    /// <summary>接缝：<c>PlugComboBoxPlayAttackValue.Items.Text</c>。</summary>
    string PlugComboBoxPlayAttackValueItemsText { get; set; }

    // ---- 音量（原文 2034-2039 / 5942-5952） ----
    /// <summary>接缝：<c>TrackBarVolume.Max</c>（原文 2037）。</summary>
    int TrackBarVolumeMax { get; set; }
    /// <summary>接缝：<c>TrackBarVolume.Min</c>（原文 2038）。</summary>
    int TrackBarVolumeMin { get; set; }
    /// <summary>接缝：<c>TrackBarVolume.Position</c>（原文 2039/5944/5950）。</summary>
    int TrackBarVolumePosition { get; set; }
    /// <summary>接缝：<c>PlugCheckBoxVolume.Caption</c>（5945 写 '音量'、5951 写音量数字）。</summary>
    string PlugCheckBoxVolumeCaption { get; set; }
    /// <summary>接缝：<c>PlugComboBoxCheckHPValue.Visible</c>（原文 1868/1875）。</summary>
    bool PlugComboBoxCheckHPValueVisible { get; set; }
    /// <summary>接缝：<c>PlugComboBoxCheckMPValue.Visible</c>（原文 1870/1877）。</summary>
    bool PlugComboBoxCheckMPValueVisible { get; set; }

    // ---- 帮助页（原文 1036-1080） ----
    /// <summary>接缝：<c>PlugMemoConfigHelp &lt;&gt; nil</c>（可注入，便于测试"控件缺失"分支）。</summary>
    object PlugMemoConfigHelp { get; set; }
    /// <summary>接缝：<c>PlugMemoConfigHelp.LoadFromFile(FileName)</c>。</summary>
    void PlugMemoConfigHelpLoadFromFile(string fileName);
    /// <summary>接缝：<c>PlugMemoConfigHelp.Lines.Count</c>。</summary>
    int PlugMemoConfigHelpLinesCount { get; }
    /// <summary>接缝：<c>PlugMemoConfigHelp.Lines[I]</c>（Caption，LoadHelpFile 的 trim 长度判据用）。</summary>
    string PlugMemoConfigHelpLineCaption(int index);
    /// <summary>接缝：<c>PlugMemoConfigHelp.FontBackTransparent</c>（原文 1049）。</summary>
    bool PlugMemoConfigHelpFontBackTransparent { get; set; }
    /// <summary>接缝：<c>ViewItem.Color.{Up,Hot,Down}.{Color,BColor,Bold}</c> 的落点收集。</summary>
    void SetHelpLineColor(int index, string state, int color, int backColor, bool bold);

    // ---- Boss 名单（原文 5120-5250） ----
    /// <summary>接缝：<c>PlugScrollBoxBoss</c> 控件句柄（原文 TDxChatMemo，仅作 &lt;&gt; nil 与宿主注入用）。</summary>
    object PlugScrollBoxBoss { get; set; }
    /// <summary>接缝：<c>PlugScrollBoxBoss.ItemIndex</c>。</summary>
    int PlugScrollBoxBossItemIndex { get; set; }
    /// <summary>接缝：<c>PlugScrollBoxBoss.Lines</c>（TStringList 语义：保序 + Text 汇总）。</summary>
    List<string> PlugScrollBoxBossLines { get; }
    /// <summary>接缝：<c>PlugScrollBoxBoss.Lines.SaveToFile(sFileName)</c>。</summary>
    void PlugScrollBoxBossSaveToFile(string fileName);
    /// <summary>接缝：<c>PlugScrollBoxBoss.Lines.LoadFromFile(sFileName)</c>。</summary>
    void PlugScrollBoxBossLoadFromFile(string fileName);

    // ---- 挂机怪物名单（原文 5408-5536） ----
    /// <summary>接缝：<c>PlugScrollBoxMons</c> 控件句柄（原文 TDxChatMemo）。</summary>
    object PlugScrollBoxMons { get; set; }
    /// <summary>接缝：<c>PlugScrollBoxMons.ItemIndex</c>。</summary>
    int PlugScrollBoxMonsItemIndex { get; set; }
    /// <summary>接缝：<c>PlugScrollBoxMons.Lines</c>。</summary>
    List<string> PlugScrollBoxMonsLines { get; }
    /// <summary>接缝：<c>PlugScrollBoxMons.Lines.SaveToFile(sFileName)</c>。</summary>
    void PlugScrollBoxMonsSaveToFile(string fileName);
    /// <summary>接缝：<c>PlugScrollBoxMons.Lines.LoadFromFile(sFileName)</c>。</summary>
    void PlugScrollBoxMonsLoadFromFile(string fileName);

    // ---- 挂机技能页 1/2 的行模型（原文 5579-5617 / 5660-5697 / 5776-5840） ----
    /// <summary>接缝：<c>PlugMemoConfig82.Count</c>。</summary>
    int PlugMemoConfig82Count { get; }
    /// <summary>接缝：<c>PlugMemoConfig82.Items[I].Count</c>（原文 5582 判 <c>= 2</c>）。</summary>
    int PlugMemoConfig82ItemCount(int index);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[I].Items[1].Checked</c>。</summary>
    bool PlugMemoConfig82Item2Checked(int index);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[I].Items[1].Data</c>。</summary>
    uint PlugMemoConfig82Item2Data(int index);
    /// <summary>接缝：<c>PlugMemoConfig82.Clear</c>。</summary>
    void PlugMemoConfig82Clear();
    /// <summary>接缝：<c>PlugMemoConfig82.Lock</c>。</summary>
    void PlugMemoConfig82Lock();
    /// <summary>接缝：<c>PlugMemoConfig82.UnLock</c>。</summary>
    void PlugMemoConfig82UnLock();
    /// <summary>接缝：<c>PlugMemoConfig82.Add</c>（返回新行下标）。</summary>
    int PlugMemoConfig82Add();
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].AddItem('', nil)</c>。</summary>
    int PlugMemoConfig82AddItem(int rowIndex);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Caption</c>。</summary>
    void PlugMemoConfig82SetItemCaption(int rowIndex, int colIndex, string caption);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Data</c>。</summary>
    void PlugMemoConfig82SetItemData(int rowIndex, int colIndex, uint data);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Style</c>。</summary>
    void PlugMemoConfig82SetItemStyle(int rowIndex, int colIndex, int style);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Alignment</c>。</summary>
    void PlugMemoConfig82SetItemAlignment(int rowIndex, int colIndex, int alignment);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Color.{Up,Hot,Down}.Color</c>。</summary>
    void PlugMemoConfig82SetItemColor(int rowIndex, int colIndex, string state, int color);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].ImageIndex.ImageType</c>。</summary>
    void PlugMemoConfig82SetItemImageType(int rowIndex, int colIndex, int imageType);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].ImageIndex.{Up,Down}</c>。</summary>
    void PlugMemoConfig82SetItemImageIndex(int rowIndex, int colIndex, string state, int value);
    /// <summary>接缝：<c>PlugMemoConfig82.Items[R].Items[C].Checked</c>。</summary>
    void PlugMemoConfig82SetItemChecked(int rowIndex, int colIndex, bool value);

    /// <summary>接缝：<c>PlugMemoConfig83.Count</c>。</summary>
    int PlugMemoConfig83Count { get; }
    /// <summary>接缝：<c>PlugMemoConfig83.Items[I].Count</c>（原文 5663 判 <c>= 2</c>）。</summary>
    int PlugMemoConfig83ItemCount(int index);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[I].Items[1].Checked</c>。</summary>
    bool PlugMemoConfig83Item2Checked(int index);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[I].Items[1].Data</c>。</summary>
    uint PlugMemoConfig83Item2Data(int index);
    /// <summary>接缝：<c>PlugMemoConfig83.Clear</c>。</summary>
    void PlugMemoConfig83Clear();
    /// <summary>接缝：<c>PlugMemoConfig83.Lock</c>。</summary>
    void PlugMemoConfig83Lock();
    /// <summary>接缝：<c>PlugMemoConfig83.UnLock</c>。</summary>
    void PlugMemoConfig83UnLock();
    /// <summary>接缝：<c>PlugMemoConfig83.Add</c>（返回新行下标）。</summary>
    int PlugMemoConfig83Add();
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].AddItem('', nil)</c>。</summary>
    int PlugMemoConfig83AddItem(int rowIndex);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Caption</c>。</summary>
    void PlugMemoConfig83SetItemCaption(int rowIndex, int colIndex, string caption);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Data</c>。</summary>
    void PlugMemoConfig83SetItemData(int rowIndex, int colIndex, uint data);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Style</c>。</summary>
    void PlugMemoConfig83SetItemStyle(int rowIndex, int colIndex, int style);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Alignment</c>。</summary>
    void PlugMemoConfig83SetItemAlignment(int rowIndex, int colIndex, int alignment);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Color.{Up,Hot,Down}.Color</c>。</summary>
    void PlugMemoConfig83SetItemColor(int rowIndex, int colIndex, string state, int color);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].ImageIndex.ImageType</c>。</summary>
    void PlugMemoConfig83SetItemImageType(int rowIndex, int colIndex, int imageType);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].ImageIndex.{Up,Down}</c>。</summary>
    void PlugMemoConfig83SetItemImageIndex(int rowIndex, int colIndex, string state, int value);
    /// <summary>接缝：<c>PlugMemoConfig83.Items[R].Items[C].Checked</c>。</summary>
    void PlugMemoConfig83SetItemChecked(int rowIndex, int colIndex, bool value);
}

/// <summary>
/// 接缝的默认实现：纯内存（无 WinForms 依赖），供测试与未初始化场景使用。
/// 对照组件的真实 WinForms 适配器由宿主在 Initialize 时注入。
/// </summary>
public sealed partial class MirReturnConfigDlgControlsStub : IMirReturnConfigDlgControlsExt
{
    public object PlugConfigDlg { get; set; } = new object();
    public bool PlugConfigDlgVisible { get; set; }
    public int PlugPageControlConfigWidth { get; set; }
    public int PlugPageControlConfigHeight { get; set; }
    public int PlugPageControlConfigActivePageIndex { get; set; }

    public bool PlugCheckBoxShowHPLabelVisible { get; set; } = true;
    public void PlugCheckBoxShowHPLabelSetFocus() => LastFocus = "PlugCheckBoxShowHPLabel";
    public bool PlugCheckBoxNumberLableVisible { get; set; } = true;
    public void PlugCheckBoxNumberLableSetFocus() => LastFocus = "PlugCheckBoxNumberLable";
    public bool PlugCheckBoxJobAndLevelVisible { get; set; } = true;
    public void PlugCheckBoxJobAndLevelSetFocus() => LastFocus = "PlugCheckBoxJobAndLevel";

    /// <summary>测试用：最近一次 SetFocus 的控件名（断言焦点三段顺序）。</summary>
    public string LastFocus { get; set; } = "";
    /// <summary>测试用：最近一次被调用的无参动作名（断言副作用）。</summary>
    public string LastCalled { get; set; } = "";

    // ---- 物品过滤页行模型 ----
    /// <summary>测试用：<c>PlugMemoConfig2</c> 的行/列快照（原文 1 行 6 列）。</summary>
    public readonly List<GJRow> Memo2Rows = new List<GJRow>();
    public int Memo2LockCount;
    public int Memo2UnLockCount;

    public void PlugMemoConfig2Clear() => Memo2Rows.Clear();
    public int PlugMemoConfig2ColCount { get; set; }
    public void PlugMemoConfig2Last() => LastCalled = "PlugMemoConfig2.Last";
    public int PlugMemoConfig2Count => Memo2Rows.Count;
    public void PlugMemoConfig2Lock() => Memo2LockCount++;
    public void PlugMemoConfig2UnLock() => Memo2UnLockCount++;
    public int PlugMemoConfig2Add() { Memo2Rows.Add(new GJRow()); return Memo2Rows.Count - 1; }
    public int PlugMemoConfig2AddItem(int rowIndex)
    {
        Memo2Rows[rowIndex].Cells.Add(new GJCell());
        return Memo2Rows[rowIndex].Cells.Count - 1;
    }
    private GJCell Cell2(int r, int c) => Memo2Rows[r].Cells[c];
    public void PlugMemoConfig2SetItemCaption(int r, int c, string caption) => Cell2(r, c).Caption = caption;
    public void PlugMemoConfig2SetItemData(int r, int c, uint data) => Cell2(r, c).Data = data;
    public void PlugMemoConfig2SetItemStyle(int r, int c, int style) => Cell2(r, c).Style = style;
    public void PlugMemoConfig2SetItemAlignment(int r, int c, int alignment) => Cell2(r, c).Alignment = alignment;
    public void PlugMemoConfig2SetItemColor(int r, int c, string state, int color) => Cell2(r, c).Colors[state] = color;
    public void PlugMemoConfig2SetItemImageType(int r, int c, int imageType) => Cell2(r, c).ImageType = imageType;
    public void PlugMemoConfig2SetItemImageIndex(int r, int c, string state, int value) => Cell2(r, c).ImageIndex[state] = value;
    public void PlugMemoConfig2SetItemChecked(int r, int c, bool value) => Cell2(r, c).Checked = value;

    /// <summary>测试用：<c>PlugMemoConfig6LabelKeyBoard1..12.Caption</c> 快照（下标 1..12）。</summary>
    public readonly string[] LabelKeyBoard = new string[13];
    public void SetLabelKeyBoard(int index1to12, string caption) => LabelKeyBoard[index1to12] = caption;
    /// <summary>测试用：<c>PlugMemoConfig4Label3/4.Caption</c> 快照。</summary>
    public readonly string[] MemoConfig4Label = new string[5];
    public void SetMemoConfig4Label(int index3or4, string caption) => MemoConfig4Label[index3or4] = caption;

    // ---- 超药索引族（委托到生成属性，单一存储） ----
    public bool PlugCheckBoxUseSuperMedicaItemName(int index) => index switch
    {
        0 => PlugCheckBoxUseSuperMedicaItemName0,
        1 => PlugCheckBoxUseSuperMedicaItemName1,
        2 => PlugCheckBoxUseSuperMedicaItemName2,
        3 => PlugCheckBoxUseSuperMedicaItemName3,
        4 => PlugCheckBoxUseSuperMedicaItemName4,
        5 => PlugCheckBoxUseSuperMedicaItemName5,
        6 => PlugCheckBoxUseSuperMedicaItemName6,
        7 => PlugCheckBoxUseSuperMedicaItemName7,
        8 => PlugCheckBoxUseSuperMedicaItemName8,
        _ => false,
    };

    public void SetPlugCheckBoxUseSuperMedicaItemName(int index, bool value)
    {
        switch (index)
        {
            case 0: PlugCheckBoxUseSuperMedicaItemName0 = value; break;
            case 1: PlugCheckBoxUseSuperMedicaItemName1 = value; break;
            case 2: PlugCheckBoxUseSuperMedicaItemName2 = value; break;
            case 3: PlugCheckBoxUseSuperMedicaItemName3 = value; break;
            case 4: PlugCheckBoxUseSuperMedicaItemName4 = value; break;
            case 5: PlugCheckBoxUseSuperMedicaItemName5 = value; break;
            case 6: PlugCheckBoxUseSuperMedicaItemName6 = value; break;
            case 7: PlugCheckBoxUseSuperMedicaItemName7 = value; break;
            case 8: PlugCheckBoxUseSuperMedicaItemName8 = value; break;
        }
    }

    private int SuperHp(int i) => i switch
    {
        0 => PlugEditSuperMedicaHP0, 1 => PlugEditSuperMedicaHP1, 2 => PlugEditSuperMedicaHP2,
        3 => PlugEditSuperMedicaHP3, 4 => PlugEditSuperMedicaHP4, 5 => PlugEditSuperMedicaHP5,
        6 => PlugEditSuperMedicaHP6, 7 => PlugEditSuperMedicaHP7, 8 => PlugEditSuperMedicaHP8,
        _ => 0,
    };
    private int SuperHpTime(int i) => i switch
    {
        0 => PlugEditSuperMedicaHPTime0, 1 => PlugEditSuperMedicaHPTime1, 2 => PlugEditSuperMedicaHPTime2,
        3 => PlugEditSuperMedicaHPTime3, 4 => PlugEditSuperMedicaHPTime4, 5 => PlugEditSuperMedicaHPTime5,
        6 => PlugEditSuperMedicaHPTime6, 7 => PlugEditSuperMedicaHPTime7, 8 => PlugEditSuperMedicaHPTime8,
        _ => 0,
    };
    private int SuperMp(int i) => i switch
    {
        0 => PlugEditSuperMedicaMP0, 1 => PlugEditSuperMedicaMP1, 2 => PlugEditSuperMedicaMP2,
        3 => PlugEditSuperMedicaMP3, 4 => PlugEditSuperMedicaMP4, 5 => PlugEditSuperMedicaMP5,
        6 => PlugEditSuperMedicaMP6, 7 => PlugEditSuperMedicaMP7, 8 => PlugEditSuperMedicaMP8,
        _ => 0,
    };
    private int SuperMpTime(int i) => i switch
    {
        0 => PlugEditSuperMedicaMPTime0, 1 => PlugEditSuperMedicaMPTime1, 2 => PlugEditSuperMedicaMPTime2,
        3 => PlugEditSuperMedicaMPTime3, 4 => PlugEditSuperMedicaMPTime4, 5 => PlugEditSuperMedicaMPTime5,
        6 => PlugEditSuperMedicaMPTime6, 7 => PlugEditSuperMedicaMPTime7, 8 => PlugEditSuperMedicaMPTime8,
        _ => 0,
    };
    private void SetSuperHp(int i, int v)
    {
        switch (i)
        {
            case 0: PlugEditSuperMedicaHP0 = v; break; case 1: PlugEditSuperMedicaHP1 = v; break;
            case 2: PlugEditSuperMedicaHP2 = v; break; case 3: PlugEditSuperMedicaHP3 = v; break;
            case 4: PlugEditSuperMedicaHP4 = v; break; case 5: PlugEditSuperMedicaHP5 = v; break;
            case 6: PlugEditSuperMedicaHP6 = v; break; case 7: PlugEditSuperMedicaHP7 = v; break;
            case 8: PlugEditSuperMedicaHP8 = v; break;
        }
    }
    private void SetSuperHpTime(int i, int v)
    {
        switch (i)
        {
            case 0: PlugEditSuperMedicaHPTime0 = v; break; case 1: PlugEditSuperMedicaHPTime1 = v; break;
            case 2: PlugEditSuperMedicaHPTime2 = v; break; case 3: PlugEditSuperMedicaHPTime3 = v; break;
            case 4: PlugEditSuperMedicaHPTime4 = v; break; case 5: PlugEditSuperMedicaHPTime5 = v; break;
            case 6: PlugEditSuperMedicaHPTime6 = v; break; case 7: PlugEditSuperMedicaHPTime7 = v; break;
            case 8: PlugEditSuperMedicaHPTime8 = v; break;
        }
    }
    private void SetSuperMp(int i, int v)
    {
        switch (i)
        {
            case 0: PlugEditSuperMedicaMP0 = v; break; case 1: PlugEditSuperMedicaMP1 = v; break;
            case 2: PlugEditSuperMedicaMP2 = v; break; case 3: PlugEditSuperMedicaMP3 = v; break;
            case 4: PlugEditSuperMedicaMP4 = v; break; case 5: PlugEditSuperMedicaMP5 = v; break;
            case 6: PlugEditSuperMedicaMP6 = v; break; case 7: PlugEditSuperMedicaMP7 = v; break;
            case 8: PlugEditSuperMedicaMP8 = v; break;
        }
    }
    private void SetSuperMpTime(int i, int v)
    {
        switch (i)
        {
            case 0: PlugEditSuperMedicaMPTime0 = v; break; case 1: PlugEditSuperMedicaMPTime1 = v; break;
            case 2: PlugEditSuperMedicaMPTime2 = v; break; case 3: PlugEditSuperMedicaMPTime3 = v; break;
            case 4: PlugEditSuperMedicaMPTime4 = v; break; case 5: PlugEditSuperMedicaMPTime5 = v; break;
            case 6: PlugEditSuperMedicaMPTime6 = v; break; case 7: PlugEditSuperMedicaMPTime7 = v; break;
            case 8: PlugEditSuperMedicaMPTime8 = v; break;
        }
    }
    public int PlugEditSuperMedicaHP(int index) => SuperHp(index);
    public int PlugEditSuperMedicaHPTime(int index) => SuperHpTime(index);
    public int PlugEditSuperMedicaMP(int index) => SuperMp(index);
    public int PlugEditSuperMedicaMPTime(int index) => SuperMpTime(index);
    public void SetPlugEditSuperMedicaHP(int index, int value)
    {
        SetSuperHp(index, value);
        if (index >= 0 && index <= 7) SuperHpWrites[index] = value;   // 原文 5913-5920 只回写控件 0..7
    }
    public void SetPlugEditSuperMedicaMP(int index, int value)
    {
        SetSuperMp(index, value);
        if (index >= 0 && index <= 7) SuperMpWrites[index] = value;
    }
    public void SetPlugEditSuperMedicaHPTime(int index, int value) => SetSuperHpTime(index, value);
    public void SetPlugEditSuperMedicaMPTime(int index, int value) => SetSuperMpTime(index, value);

    /// <summary>
    /// 测试用：<c>PlugEditSuperMedicaHP1..HP8</c> 的回写快照（**控件下标 0..7**，
    /// 与数组下标整体错位 1 —— 见 <c>DCheckBoxSuperMedicaPercentClick</c> 的原文缺陷登记）。
    /// </summary>
    public readonly int[] SuperHpWrites = new int[8];
    /// <summary>测试用：<c>PlugEditSuperMedicaMP1..MP8</c> 的回写快照。</summary>
    public readonly int[] SuperMpWrites = new int[8];

    // ---- 自动练功下拉 ----
    /// <summary>测试用：自动练功下拉条目（name + 原始对象）。</summary>
    public readonly List<(string Name, object Obj)> AutoMagicItems = new List<(string, object)>();
    public object PlugComboBoxAutoMagic { get; set; } = new object();
    public int PlugComboBoxAutoMagicItemIndex { get; set; } = -1;
    public int PlugComboBoxAutoMagicItemsCount => AutoMagicItems.Count;
    public void PlugComboBoxAutoMagicItemsClear() => AutoMagicItems.Clear();
    public void PlugComboBoxAutoMagicItemsAddObject(string name, object obj) => AutoMagicItems.Add((name, obj));
    public object PlugComboBoxAutoMagicItemsObject(int index) => AutoMagicItems[index].Obj;

    public string PlugComboBoxNoRedPoisonValueItemsText { get; set; } = "";
    public string PlugComboBoxNoBluePoisonValueItemsText { get; set; } = "";
    public string PlugComboBoxNoDuFuValueItemsText { get; set; } = "";
    public string PlugComboBoxBagFullValueItemsText { get; set; } = "";
    public string PlugComboBoxPlayAttackValueItemsText { get; set; } = "";

    public int TrackBarVolumeMax { get; set; }
    public int TrackBarVolumeMin { get; set; }
    public int TrackBarVolumePosition { get; set; }
    public string PlugCheckBoxVolumeCaption { get; set; } = "";
    public bool PlugComboBoxCheckHPValueVisible { get; set; } = true;
    public bool PlugComboBoxCheckMPValueVisible { get; set; } = true;

    // ---- 帮助页 ----
    public object PlugMemoConfigHelp { get; set; } = new object();
    public bool PlugMemoConfigHelpFontBackTransparent { get; set; }
    public void PlugMemoConfigHelpLoadFromFile(string fileName) => HelpLoadedFrom = fileName;
    /// <summary>测试用：最近一次 LoadFromFile 的路径。</summary>
    public string HelpLoadedFrom { get; set; } = "";
    public int PlugMemoConfigHelpLinesCount => HelpLines.Count;
    public string PlugMemoConfigHelpLineCaption(int index) => HelpLines[index];
    public void SetHelpLineColor(int index, string state, int color, int backColor, bool bold)
        => HelpColors.Add((index, state, color, backColor, bold));
    /// <summary>测试用：帮助行文本（可注入含首尾空格的"标题行"）。</summary>
    public readonly List<string> HelpLines = new List<string>();
    /// <summary>测试用：颜色落点收集。</summary>
    public readonly List<(int Index, string State, int Color, int BackColor, bool Bold)> HelpColors
        = new List<(int, string, int, int, bool)>();

    // ---- Boss 名单 ----
    public object PlugScrollBoxBoss { get; set; } = new object();
    public int PlugScrollBoxBossItemIndex { get; set; }
    public List<string> PlugScrollBoxBossLines { get; } = new List<string>();
    public void PlugScrollBoxBossSaveToFile(string fileName) => BossSavedTo = fileName;
    public void PlugScrollBoxBossLoadFromFile(string fileName)
    {
        BossLoadedFrom = fileName;
        PlugScrollBoxBossLines.Clear();
        PlugScrollBoxBossLines.AddRange(BossFileContent);
    }
    /// <summary>测试用：最近一次 SaveToFile / LoadFromFile 的路径。</summary>
    public string BossSavedTo { get; set; } = "";
    public string BossLoadedFrom { get; set; } = "";
    /// <summary>测试用：模拟磁盘上已有的 Boss 文件内容。</summary>
    public readonly List<string> BossFileContent = new List<string>();

    // ---- 挂机怪物名单 ----
    public object PlugScrollBoxMons { get; set; } = new object();
    public int PlugScrollBoxMonsItemIndex { get; set; }
    public List<string> PlugScrollBoxMonsLines { get; } = new List<string>();
    public void PlugScrollBoxMonsSaveToFile(string fileName) => MonsSavedTo = fileName;
    public void PlugScrollBoxMonsLoadFromFile(string fileName)
    {
        MonsLoadedFrom = fileName;
        PlugScrollBoxMonsLines.Clear();
        PlugScrollBoxMonsLines.AddRange(MonsFileContent);
    }
    public string MonsSavedTo { get; set; } = "";
    public string MonsLoadedFrom { get; set; } = "";
    public readonly List<string> MonsFileContent = new List<string>();

    // ---- 挂机技能表 82/83 ----
    /// <summary>测试用：页 82 的技能行（ItemCount 用于复刻原文 <c>ListItem.Count = 2</c> 判据）。</summary>
    public readonly List<(int ItemCount, bool Checked, uint Data)> Memo82Rows
        = new List<(int, bool, uint)>();
    /// <summary>测试用：页 83 的技能行。</summary>
    public readonly List<(int ItemCount, bool Checked, uint Data)> Memo83Rows
        = new List<(int, bool, uint)>();

    /// <summary>测试用：行/列属性快照（与原文 <c>pTViewItem</c> 取值面一一对应）。</summary>
    public sealed class GJRow
    {
        public readonly List<GJCell> Cells = new List<GJCell>();
    }
    /// <summary>测试用：单元格属性快照。</summary>
    public sealed class GJCell
    {
        public string Caption = "";
        public uint Data;
        public int Style;
        public int Alignment;
        public readonly Dictionary<string, int> Colors = new Dictionary<string, int>();
        public int ImageType = -1;
        public readonly Dictionary<string, int> ImageIndex = new Dictionary<string, int>();
        public bool Checked;
    }

    /// <summary>测试用：页 82 的行模型（RefreshGJMagic 写入面）。</summary>
    public readonly List<GJRow> Memo82Rows2 = new List<GJRow>();
    /// <summary>测试用：页 83 的行模型。</summary>
    public readonly List<GJRow> Memo83Rows2 = new List<GJRow>();
    public int Memo82LockCount;
    public int Memo82UnLockCount;
    public int Memo83LockCount;
    public int Memo83UnLockCount;

    public int PlugMemoConfig82Count => Memo82Rows.Count;
    public int PlugMemoConfig82ItemCount(int index) => index >= 0 && index < Memo82Rows.Count ? Memo82Rows[index].ItemCount : 0;
    public bool PlugMemoConfig82Item2Checked(int index) => index >= 0 && index < Memo82Rows.Count && Memo82Rows[index].Checked;
    public uint PlugMemoConfig82Item2Data(int index) => index >= 0 && index < Memo82Rows.Count ? Memo82Rows[index].Data : 0u;
    public void PlugMemoConfig82Clear() => Memo82Rows2.Clear();
    public void PlugMemoConfig82Lock() => Memo82LockCount++;
    public void PlugMemoConfig82UnLock() => Memo82UnLockCount++;
    public int PlugMemoConfig82Add() { Memo82Rows2.Add(new GJRow()); return Memo82Rows2.Count - 1; }
    public int PlugMemoConfig82AddItem(int rowIndex)
    {
        Memo82Rows2[rowIndex].Cells.Add(new GJCell());
        return Memo82Rows2[rowIndex].Cells.Count - 1;
    }
    private GJCell Cell82(int r, int c) => Memo82Rows2[r].Cells[c];
    public void PlugMemoConfig82SetItemCaption(int r, int c, string caption) => Cell82(r, c).Caption = caption;
    public void PlugMemoConfig82SetItemData(int r, int c, uint data) => Cell82(r, c).Data = data;
    public void PlugMemoConfig82SetItemStyle(int r, int c, int style) => Cell82(r, c).Style = style;
    public void PlugMemoConfig82SetItemAlignment(int r, int c, int alignment) => Cell82(r, c).Alignment = alignment;
    public void PlugMemoConfig82SetItemColor(int r, int c, string state, int color) => Cell82(r, c).Colors[state] = color;
    public void PlugMemoConfig82SetItemImageType(int r, int c, int imageType) => Cell82(r, c).ImageType = imageType;
    public void PlugMemoConfig82SetItemImageIndex(int r, int c, string state, int value) => Cell82(r, c).ImageIndex[state] = value;
    public void PlugMemoConfig82SetItemChecked(int r, int c, bool value) => Cell82(r, c).Checked = value;

    public int PlugMemoConfig83Count => Memo83Rows.Count;
    public int PlugMemoConfig83ItemCount(int index) => index >= 0 && index < Memo83Rows.Count ? Memo83Rows[index].ItemCount : 0;
    public bool PlugMemoConfig83Item2Checked(int index) => index >= 0 && index < Memo83Rows.Count && Memo83Rows[index].Checked;
    public uint PlugMemoConfig83Item2Data(int index) => index >= 0 && index < Memo83Rows.Count ? Memo83Rows[index].Data : 0u;
    public void PlugMemoConfig83Clear() => Memo83Rows2.Clear();
    public void PlugMemoConfig83Lock() => Memo83LockCount++;
    public void PlugMemoConfig83UnLock() => Memo83UnLockCount++;
    public int PlugMemoConfig83Add() { Memo83Rows2.Add(new GJRow()); return Memo83Rows2.Count - 1; }
    public int PlugMemoConfig83AddItem(int rowIndex)
    {
        Memo83Rows2[rowIndex].Cells.Add(new GJCell());
        return Memo83Rows2[rowIndex].Cells.Count - 1;
    }
    private GJCell Cell83(int r, int c) => Memo83Rows2[r].Cells[c];
    public void PlugMemoConfig83SetItemCaption(int r, int c, string caption) => Cell83(r, c).Caption = caption;
    public void PlugMemoConfig83SetItemData(int r, int c, uint data) => Cell83(r, c).Data = data;
    public void PlugMemoConfig83SetItemStyle(int r, int c, int style) => Cell83(r, c).Style = style;
    public void PlugMemoConfig83SetItemAlignment(int r, int c, int alignment) => Cell83(r, c).Alignment = alignment;
    public void PlugMemoConfig83SetItemColor(int r, int c, string state, int color) => Cell83(r, c).Colors[state] = color;
    public void PlugMemoConfig83SetItemImageType(int r, int c, int imageType) => Cell83(r, c).ImageType = imageType;
    public void PlugMemoConfig83SetItemImageIndex(int r, int c, string state, int value) => Cell83(r, c).ImageIndex[state] = value;
    public void PlugMemoConfig83SetItemChecked(int r, int c, bool value) => Cell83(r, c).Checked = value;
}

public static class MirReturnGlobalSeam
{
    // ---- ConfigShare.pas:10-11 落盘文件名模板（原文 Format 模板，逐字照抄） ----
    /// <summary>原文 <c>ConfigShare.pas:9 CONFIGFILE = 'Config\%s.%s.set';</c></summary>
    public const string CONFIGFILE = @"Config\%s.%s.set";
    /// <summary>原文 <c>ConfigShare.pas:10 BOSSCONFIGFILE = 'Config\%s.%s.Boss.set';</c></summary>
    public const string BOSSCONFIGFILE = @"Config\%s.%s.Boss.set";
    /// <summary>原文 <c>ConfigShare.pas:11 GJMONCONFIGFILE = 'Config\%s.%s.GjMon.set';</c></summary>
    public const string GJMONCONFIGFILE = @"Config\%s.%s.GjMon.set";
    /// <summary>
    /// 原文 <c>ConfigShare.pas</c> 的 <c>GJNAGICCONFIGFILE1</c>（原文拼写即 "NAGIC"，非 "MAGIC" ——
    /// 原文如此；逐字保留以免与原文对不上）。
    /// 值取自 ConfigShare.pas（同一单元的第 12-13 行），与 BOSSCONFIGFILE/GJMONCONFIGFILE 同族。
    /// </summary>
    public const string GJNAGICCONFIGFILE1 = @"Config\%s.%s.GjMagic1.set";
    /// <summary>原文 <c>ConfigShare.pas</c> 的 <c>GJNAGICCONFIGFILE2</c>（原文拼写如此）。</summary>
    public const string GJNAGICCONFIGFILE2 = @"Config\%s.%s.GjMagic2.set";

    // ---- MShare.pas 全局 ----
    /// <summary>接缝：MShare.pas 的 <c>g_BossList:TStringList</c>（Boss 名单的全局镜像）。</summary>
    public static readonly List<string> g_BossList = new List<string>();
    /// <summary>接缝：MShare.pas 的 <c>g_GJMonList:TStringList</c>（挂机怪物名单的全局镜像）。</summary>
    public static readonly List<string> g_GJMonList = new List<string>();
    /// <summary>接缝：MShare.pas 的 <c>g_GJUseMagic1:TList</c>（挂机技能表 1）。</summary>
    public static readonly List<object> g_GJUseMagic1 = new List<object>();
    /// <summary>接缝：MShare.pas 的 <c>g_GJUseMagic2:TList</c>（挂机技能表 2）。</summary>
    public static readonly List<object> g_GJUseMagic2 = new List<object>();
    /// <summary>接缝：MShare.pas 的 <c>g_MagicList:TGList</c>（技能列表；<c>Items[I]</c> 是 <c>pTClientMagic</c>）。</summary>
    public static readonly List<object> g_MagicList = new List<object>();

    // ---- ClMain.pas frmMain 的广播接缝（原文直接赋值给 frmMain 的公开字段/方法） ----
    /// <summary>接缝：ClMain.pas <c>frmMain.nSpecialColor</c>。</summary>
    public static Action<byte> SetFrmMainSpecialColor = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nColorShowEff</c>（原文 4534 —— 与 4537 的 nSpecialColor 成对）。</summary>
    public static Action<byte> SetFrmMainColorShowEff = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNotRushMonRange</c>。</summary>
    public static Action<int> SetFrmMainGJNotRushMonRange = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJPlayAttackOption</c>。</summary>
    public static Action<int> SetFrmMainGJPlayAttackOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoRedPoisonOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoRedPoisonOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoBluePoisonOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoBluePoisonOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJNoDuFuOption</c>。</summary>
    public static Action<int> SetFrmMainGJNoDuFuOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJBagFullOption</c>。</summary>
    public static Action<int> SetFrmMainGJBagFullOption = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.nGJGroupAttackCount</c>。</summary>
    public static Action<int> SetFrmMainGJGroupAttackCount = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.SendPlugInConfig(v:Integer)</c>（原文 2147 用）。</summary>
    public static Action<int> SendPlugInConfig = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.ChangePoisonCharm(Magic:pTClientMagic)</c>（原文 4471 用）。</summary>
    public static readonly List<object> ChangePoisonCharmCalls = new List<object>();
    /// <summary>接缝：ClMain.pas <c>frmMain.ChangePoisonCharm</c>。</summary>
    public static Action<object> ChangePoisonCharm = m => ChangePoisonCharmCalls.Add(m);

    /// <summary>接缝：BassSound / SoundUtil 的 <c>g_SoundVolume:Integer</c>（原文 5944/5950 直接赋值）。</summary>
    public static int g_SoundVolume;

    /// <summary>接缝：MShare.pas <c>g_GJActionMode:TStringList</c>（挂机动作下拉的条目源，原文 4781-4785 只用 <c>.Text</c>）。</summary>
    public static string g_GJActionModeText = "";

    /// <summary>接缝：MShare.pas/ClMain.pas 的 <c>DebugOutStr(s:string)</c>（原文 5063-5064 记录异常）。</summary>
    public static readonly List<string> DebugOut = new List<string>();
    /// <summary>接缝：<c>DebugOutStr</c>。</summary>
    public static Action<string> DebugOutStr = s => DebugOut.Add(s);

    /// <summary>接缝：<c>ExtractFilePath(ParamStr(0))</c>（含结尾反斜杠；<see cref="AppPath"/> 已承载）。</summary>
    public static string ExtractFilePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        int i = path.LastIndexOfAny(new[] { '\\', '/' });
        return i < 0 ? "" : path.Substring(0, i + 1);
    }

    /// <summary>接缝：Delphi <c>Format('%s', [...])</c> 的最小实现（原文只用于 <c>'%s'</c> 单占位模板）。</summary>
    public static string Format1(string template, string arg)
        => template.Replace("%s", arg ?? "");

    /// <summary>接缝：Delphi <c>Format('%s.%s', [a, b])</c>（按出现顺序替换两个占位）。</summary>
    public static string Format2(string template, string a, string b)
    {
        int i = template.IndexOf("%s", StringComparison.Ordinal);
        if (i < 0) return template;
        string head = template.Substring(0, i) + (a ?? "");
        string rest = template.Substring(i + 2);
        int j = rest.IndexOf("%s", StringComparison.Ordinal);
        if (j < 0) return head + rest;
        return head + rest.Substring(0, j) + (b ?? "") + rest.Substring(j + 2);
    }

    /// <summary>接缝：Delphi <c>IntToStr</c>。</summary>
    public static string IntToStr(int v) => v.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>接缝：Delphi <c>StrToIntDef(s, def)</c>（失败返回 def，不抛异常）。</summary>
    public static int StrToIntDef(string s, int def)
        => int.TryParse((s ?? "").Trim(), System.Globalization.NumberStyles.Integer,
                        System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : def;

    /// <summary>接缝：<c>TStringList.SaveToFile</c>（原文 5236/5522/5592/5673）。</summary>
    public static Action<string, List<string>> SaveTextFile = (_, __) => { };
    /// <summary>接缝：<c>TStringList.LoadFromFile</c>（原文 5243/5529/5604/5684）。</summary>
    public static Func<string, List<string>> LoadTextFile = _ => new List<string>();

    /// <summary>接缝：ClMain.pas <c>frmMain.GetRGB(value:Integer):TColor</c>（原文 5259）。</summary>
    public static Func<int, int> GetRGB = v => v;
    /// <summary>接缝：<c>PlugLabelSpecialColor.CaptionColor.Up.Color</c>（原文 5259）。</summary>
    public static Action<int> SetLabelSpecialColorUp = _ => { };

    // ---- IniFiles.pas / SysUtils 的文件系统接缝 ----
    /// <summary>
    /// 接缝：原文 <c>ExtractFilePath(ParamStr(0))</c>（可执行文件所在目录，含结尾反斜杠）。
    /// 默认空串 + 由宿主注入，避免在无头测试里碰真实磁盘布局。
    /// </summary>
    public static string AppPath = "";

    /// <summary>接缝：原文 <c>DirectoryExists</c>。</summary>
    public static Func<string, bool> DirectoryExists = _ => false;
    /// <summary>接缝：原文 <c>ForceDirectories</c>。</summary>
    public static Action<string> ForceDirectories = _ => { };
    /// <summary>接缝：原文 <c>FileExists</c>。</summary>
    public static Func<string, bool> FileExists = _ => false;
    /// <summary>接缝：原文 <c>TIniFile</c> 的创建（返回可用的读写器）。</summary>
    public static Func<string, IMirReturnIniFile> CreateIniFile = _ => new MirReturnIniFileStub();

    /// <summary>接缝：原文 <c>Math.Max</c>（避免与 <c>System.Math</c> 的歧义，内联）。</summary>
    public static int Max(int a, int b) => a > b ? a : b;
    /// <summary>接缝：原文 <c>Math.Min</c>。</summary>
    public static int Min(int a, int b) => a < b ? a : b;
    /// <summary>接缝：Delphi <c>Math.Max</c> 在 Integer/LongWord 混合时的 **Int64 提升**形态（见 AutoEat* 的 EatThreshold）。</summary>
    public static long Min(long a, long b) => a < b ? a : b;
    /// <summary>接缝：Delphi <c>Math.Max</c> 的 Int64 形态。</summary>
    public static long Max(long a, long b) => a > b ? a : b;

    /// <summary>
    /// 接缝：<c>Math.Max(a, b)</c> 用于把结果赋回 <c>Integer</c> 字段的场景
    /// （原文 2182/2193/2220/2226/2232/2238/2370/2484/4540/4670/4674/4678/4682/4741/4743）。
    ///
    /// **类型语义照抄**：<c>b</c> 常是 <c>LongWord</c>（如 <c>dwPluginMinEatItemTime</c>），
    /// Delphi 的 <c>Math.Max</c> 因混合类型提升到 **Int64** 比较，再赋回 <c>Integer</c> 时按位截断。
    /// <c>a</c> 恒为字面常量（1/2/0），故此处按 <c>unchecked</c> 截断复刻。
    /// </summary>
    public static int MaxI(long a, long b) => unchecked((int)(a > b ? a : b));

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        g_BossList.Clear();
        g_GJMonList.Clear();
        g_GJUseMagic1.Clear();
        g_GJUseMagic2.Clear();
        g_MagicList.Clear();
        ChangePoisonCharmCalls.Clear();
        SetFrmMainSpecialColor = _ => { };
        SetFrmMainColorShowEff = _ => { };
        SetFrmMainGJNotRushMonRange = _ => { };
        SetFrmMainGJPlayAttackOption = _ => { };
        SetFrmMainGJNoRedPoisonOption = _ => { };
        SetFrmMainGJNoBluePoisonOption = _ => { };
        SetFrmMainGJNoDuFuOption = _ => { };
        SetFrmMainGJBagFullOption = _ => { };
        SetFrmMainGJGroupAttackCount = _ => { };
        SendPlugInConfig = _ => { };
        ChangePoisonCharm = m => ChangePoisonCharmCalls.Add(m);
        g_SoundVolume = 0;
        g_GJActionModeText = "";
        DebugOut.Clear();
        DebugOutStr = s => DebugOut.Add(s);
        SaveTextFile = (_, __) => { };
        LoadTextFile = _ => new List<string>();
        GetRGB = v => v;
        SetLabelSpecialColorUp = _ => { };
        AppPath = "";
        DirectoryExists = _ => false;
        ForceDirectories = _ => { };
        FileExists = _ => false;
        CreateIniFile = _ => new MirReturnIniFileStub();
    }
}

/// <summary>
/// 接缝：IniFiles.pas 的 <c>TIniFile</c>（原文 LoadConfigFile/SaveConfigFile 用它读写 INI）。
/// 只声明原文真正调用的成员（ReadString/WriteString/ReadInteger/WriteInteger/ReadBool/WriteBool/UpdateFile）。
/// </summary>
public interface IMirReturnIniFile
{
    string ReadString(string section, string ident, string defaultValue);
    void WriteString(string section, string ident, string value);
    int ReadInteger(string section, string ident, int defaultValue);
    void WriteInteger(string section, string ident, int value);
    bool ReadBool(string section, string ident, bool defaultValue);
    void WriteBool(string section, string ident, bool value);
    void UpdateFile();
}

/// <summary>接缝默认实现：内存 INI（无磁盘副作用），供测试与未初始化场景使用。</summary>
public sealed class MirReturnIniFileStub : IMirReturnIniFile
{
    /// <summary>(section, ident) → value 的内存表（键按原文大小写保存）。</summary>
    public readonly Dictionary<string, string> Values = new Dictionary<string, string>();
    public int WriteCount;

    private static string Key(string s, string i) => s + "\u0001" + i;

    public string ReadString(string section, string ident, string defaultValue)
        => Values.TryGetValue(Key(section, ident), out var v) ? v : defaultValue;

    public void WriteString(string section, string ident, string value)
    {
        Values[Key(section, ident)] = value ?? "";
        WriteCount++;
    }

    public int ReadInteger(string section, string ident, int defaultValue)
        => Values.TryGetValue(Key(section, ident), out var v) && int.TryParse(v, out var n) ? n : defaultValue;

    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, value.ToString());

    public bool ReadBool(string section, string ident, bool defaultValue)
    {
        if (!Values.TryGetValue(Key(section, ident), out var v)) return defaultValue;
        return v == "1" || string.Equals(v, "True", StringComparison.OrdinalIgnoreCase);
    }

    public void WriteBool(string section, string ident, bool value)
        => WriteString(section, ident, value ? "1" : "0");

    public void UpdateFile() => UpdateFileCount++;
    public int UpdateFileCount;
}

/// <summary>
/// 接缝：<c>FrmDlg.DMessageDlg(sMsg:string; Buttons:array of TMsgDlgBtn)</c>
/// （MirReturnConfigDlg.pas:5156/5168/5189/5200/5443/5455/5476/5486 共 8 处）。
///
/// ★ 无头 UI 规程：<c>UiEnabled</c> 默认 true（生产行为：走真实模态对话框）；
/// 单元测试必须置 false —— 无头环境下模态框会启动消息循环并**挂死 testhost**。
/// 与 <c>GXX.RunGate.MessageBoxSeam</c> 同一模式（该单元在 RunGate 工程，
/// GXX.Client 未引用它，故按规程在本车道内提供同形接缝，而不是跨工程引依赖）。
/// </summary>
public static class MirReturnMessageSeam
{
    /// <summary>是否允许弹出**真实**模态对话框。默认 true（生产行为）。</summary>
    public static bool UiEnabled = true;

    /// <summary>捕获到的消息（测试断言用，与 UiEnabled 无关）。</summary>
    public static readonly List<string> Messages = new List<string>();

    /// <summary>接缝：<c>FrmDlg.DMessageDlg</c> 的实现注入点。</summary>
    public static Action<string> Show = msg => { };

    /// <summary>原文 <c>FrmDlg.DMessageDlg(Msg, [mbOk])</c> 的等价调用。</summary>
    public static void DMessageDlg(string msg)
    {
        Messages.Add(msg);
        if (!UiEnabled) return;      // 非交互（测试）环境：不阻塞
        Show(msg);
    }

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        Messages.Clear();
        Show = _ => { };
    }
}