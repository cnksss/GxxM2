// ============================================================================
// 源单元：Source/M2Engine/Forms/ConfigMerchant.pas（654 行，GBK）
// 同源 DFM：Source/M2Engine/Forms/ConfigMerchant.dfm（文本，466 行）
//   DFM 控件 **50** 个；事件绑定 **33** 个
//   （根节点 `OnCreate = FormCreate` + 32 个控件绑定；
//    注意 `ButtonViewData.OnClick = ButtonClearTempDataClick` 是**同一个处理器的第二次绑定**）。
//   窗体：Caption='交易NPC配置'、Left=366 Top=256、BorderStyle=bsSingle、
//         BorderIcons=[biSystemMenu,biMinimize]、ClientWidth=818 ClientHeight=374、
//         Position=poMainFormCenter、ShowHint=True、OnCreate=FormCreate。
//
// 方法清单（.pas 行号，共 40 个）：
//   ModValue :121      uModValue :127     Open :133        ClearMerchantData :198
//   RefListBoxMerChant :215   LoadScriptFile :381   ChangeScriptAllowAction :450
//   ListBoxMerChantClick :236  FormCreate :281  ButtonSaveClick :153
//   ButtonClearTempDataClick :145  CheckBoxDenyRefStatusClick :293
//   EditXChange :301  EditYChange :309  EditShowNameChange :317  EditImageIdxChange :325
//   EditScriptNameChange :333  EditMapNameChange :341  ComboBoxDirChange :349
//   CheckBoxOfCastleClick :357  CheckBoxAutoMoveClick :365  EditMoveTimeChange :373
//   CheckBox{Buy:487,Sell:496,Getback:505,Storage:514,Upgradenow:523,Getbackupgnow:532,
//             Repair:541,S_repair:550,Makedrug:559,SendMsg:568}Click
//   EditPriceRateChange :577  ButtonScriptSaveClick :587  ButtonReLoadNpcClick :597
//   MemoScriptChange :606  chkCreateHeroClick :613  chkBuHeroClick :623  btnSearchClick :633
//
// 原文 uses（:8-9 / :115-116）：ObjNpc, SpinEditEx, NpcCommon, Vcl.Samples.Spin, UsrEngn, M2Share。
//
// ★ 原文缺陷/怪癖（逐字保留 + 差异断言锁定）：
//   1. `RefListBoxMerChant`（:215-234）**不清空** `ListBoxMerChant` ⇒ 同一实例反复 `Open`
//      会把 NPC 条目**累积**（对比 `TFrmDummySetting.FormCreate` 是先 Clear 的）。原文如此。
//   2. `LoadScriptFile`（:381-448）的 `(` 参数行只写 **11** 个开关
//      （`m_boCreateHeroName` / `m_boBuHero` **不写**），
//      而 `ChangeScriptAllowAction`（:450-485）写 **13** 个（含这两个）。
//      ⇒ 打开窗口时头部行**不含**"创建英雄/副将英雄"，只要改动任一把开关就会被补上。
//   3. `ChangeScriptAllowAction` 的插入顺序：`CreateHero`/`CreateDeputy` 插在
//      `SendMsg` 与 `ArmRemoveStone` **之间**（:475-482），与 `LoadScriptFile` 的尾部顺序不同。
//   4. `EditPriceRateChange`（:577-585）直接写 `MemoScript.Lines[1]` —— **不判空**；
//      行数 < 2 时 Delphi 抛 `EStringListError`（托管侧同样抛）。原文如此。
//   5. `ButtonScriptSaveClick`（:587-595）**不判 `SelMerchant = nil`**（对比
//      `ButtonReLoadNpcClick` :599 判了）⇒ 未选中时会 nil 解引用。逐字保留。
//   6. `btnSearchClick`（:633-652）命中后**不 Break** ⇒ 多个匹配时最终停在**最后一个**。
//   7. `Open`（:133-143）里 `boOpened` 先置 False、末尾才置 True ⇒ `ShowModal` 期间为 True。
//   8. `CheckBoxDenyRefStatus`（刷新状态）**不经过** `ModValue`，也不受 `boOpened` 保护
//      （:293-299 只判 `SelMerchant <> nil`）⇒ 它是唯一"不置脏"的开关。
//
// ★ 事件映射（Delphi → WinForms）与 §37.2 机制 1：
//   Delphi `TCheckBox.OnClick` 在"点击但 Checked 未变"时**也**触发；WinForms 侧只能挂
//   `CheckedChanged`（状态**变化**才触发）。此处照 p8-m2-dummysetting 的既定处置照抄，
//   各复选框处理器体都只在事件里做同一件事，故行为差异仅限"无变化点击"这一情形。
//   Delphi `TEdit.OnChange`/`TSpinEditEx.OnChange`/`TComboBox.OnChange`/`TMemo.OnChange`
//   → `TextChanged`/`ValueChanged`/`SelectedIndexChanged`/`TextChanged`（**含
//   `MemoScript.Lines[i] := x` 触发的级联**，见 Sweep9MemoLines.Changed）。
// ============================================================================

using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Npc;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// 原文 `ConfigMerchant.pas:12-108 TfrmConfigMerchant = class(TForm)` 1:1。
/// <para>「工具 → 交易 NPC 配制」窗体：左侧 NPC 列表 / 右侧脚本编辑 + 脚本参数开关。</para>
/// </summary>
public sealed partial class TfrmConfigMerchant : System.Windows.Forms.Form
{
    // ==================================================================
    // DFM 控件字段（50 个；声明顺序 = DFM 顺序 = .pas:13-61）
    // ==================================================================

    /// <summary>DFM `GroupBoxNPC: TGroupBox`（`Caption = '相关设置'`，`Enabled = False`）。</summary>
    public System.Windows.Forms.GroupBox GroupBoxNPC = null!;
    /// <summary>DFM `Label2`（'脚本名称:'）。</summary>
    public System.Windows.Forms.Label Label2 = null!;
    /// <summary>DFM `Label3`（'地图名称:'）。</summary>
    public System.Windows.Forms.Label Label3 = null!;
    /// <summary>DFM `Label4`（'座标X:'）。</summary>
    public System.Windows.Forms.Label Label4 = null!;
    /// <summary>DFM `Label5`（'Y:'）。</summary>
    public System.Windows.Forms.Label Label5 = null!;
    /// <summary>DFM `Label6`（'显示名称:'）。</summary>
    public System.Windows.Forms.Label Label6 = null!;
    /// <summary>DFM `Label7`（'方向:'）。</summary>
    public System.Windows.Forms.Label Label7 = null!;
    /// <summary>DFM `Label8`（'外形:'）。</summary>
    public System.Windows.Forms.Label Label8 = null!;
    /// <summary>DFM `Label10`（'地图描述:'）。</summary>
    public System.Windows.Forms.Label Label10 = null!;
    /// <summary>DFM `Label11`（'移动间隔:'）。</summary>
    public System.Windows.Forms.Label Label11 = null!;
    /// <summary>DFM `EditScriptName: TEdit`（`OnChange = EditScriptNameChange`）。</summary>
    public System.Windows.Forms.TextBox EditScriptName = null!;
    /// <summary>DFM `EditMapName: TEdit`（`OnChange = EditMapNameChange`）。</summary>
    public System.Windows.Forms.TextBox EditMapName = null!;
    /// <summary>DFM `EditShowName: TEdit`（`OnChange = EditShowNameChange`）。</summary>
    public System.Windows.Forms.TextBox EditShowName = null!;
    /// <summary>DFM `CheckBoxOfCastle: TCheckBox`（`Caption = '属于城堡'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxOfCastle = null!;
    /// <summary>DFM `ComboBoxDir: TComboBox`（`Style = csDropDownList`）。</summary>
    public System.Windows.Forms.ComboBox ComboBoxDir = null!;
    /// <summary>DFM `EditImageIdx: TSpinEditEx`（Min=0 Max=65535 Value=0）。</summary>
    public System.Windows.Forms.NumericUpDown EditImageIdx = null!;
    /// <summary>DFM `EditX: TSpinEditEx`（Min=1 Max=1000 Value=1）。</summary>
    public System.Windows.Forms.NumericUpDown EditX = null!;
    /// <summary>DFM `EditY: TSpinEditEx`（Min=1 Max=1000 Value=1）。</summary>
    public System.Windows.Forms.NumericUpDown EditY = null!;
    /// <summary>DFM `EditMapDesc: TEdit`（`Enabled = False`，`ReadOnly = True`）。</summary>
    public System.Windows.Forms.TextBox EditMapDesc = null!;
    /// <summary>DFM `CheckBoxAutoMove: TCheckBox`（`Caption = '自动移动'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxAutoMove = null!;
    /// <summary>DFM `EditMoveTime: TSpinEditEx`（Min=0 Max=65535 Value=0）。</summary>
    public System.Windows.Forms.NumericUpDown EditMoveTime = null!;

    /// <summary>DFM `GroupBoxScript: TGroupBox`（`Caption = '脚本编辑'`，`Enabled = False`）。</summary>
    public System.Windows.Forms.GroupBox GroupBoxScript = null!;
    /// <summary>DFM `MemoScript: TMemo`（`ScrollBars = ssBoth`，`OnChange = MemoScriptChange`）。</summary>
    public Sweep9Memo MemoScript = null!;
    /// <summary>DFM `ButtonScriptSave: TButton`（`Caption = '保存(&S)'`）。</summary>
    public System.Windows.Forms.Button ButtonScriptSave = null!;
    /// <summary>DFM `GroupBox3: TGroupBox`（`Caption = '脚本参数'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox3 = null!;
    /// <summary>DFM `Label9`（'交易折扣:'）。</summary>
    public System.Windows.Forms.Label Label9 = null!;
    /// <summary>DFM `CheckBoxBuy: TCheckBox`（`Caption = '买'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxBuy = null!;
    /// <summary>DFM `CheckBoxSell: TCheckBox`（`Caption = '卖'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxSell = null!;
    /// <summary>DFM `CheckBoxStorage: TCheckBox`（`Caption = '取仓库'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxStorage = null!;
    /// <summary>DFM `CheckBoxGetback: TCheckBox`（`Caption = '存仓库'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxGetback = null!;
    /// <summary>DFM `CheckBoxMakedrug: TCheckBox`（`Caption = '合成物品'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxMakedrug = null!;
    /// <summary>DFM `CheckBoxUpgradenow: TCheckBox`（`Caption = '升级武器'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxUpgradenow = null!;
    /// <summary>DFM `CheckBoxGetbackupgnow: TCheckBox`（`Caption = '取回升级'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxGetbackupgnow = null!;
    /// <summary>DFM `CheckBoxRepair: TCheckBox`（`Caption = '修理物品'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxRepair = null!;
    /// <summary>DFM `CheckBoxS_repair: TCheckBox`（`Caption = '特殊修理'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxS_repair = null!;
    /// <summary>DFM `EditPriceRate: TSpinEditEx`（Min=60 Max=500 Value=60）。</summary>
    public System.Windows.Forms.NumericUpDown EditPriceRate = null!;
    /// <summary>DFM `CheckBoxSendMsg: TCheckBox`（`Caption = '祝福语'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxSendMsg = null!;
    /// <summary>DFM `chkCreateHero: TCheckBox`（`Caption = '创建英雄'`）。</summary>
    public System.Windows.Forms.CheckBox chkCreateHero = null!;
    /// <summary>DFM `chkBuyHero: TCheckBox`（`Caption = '副将英雄'`）。</summary>
    public System.Windows.Forms.CheckBox chkBuyHero = null!;
    /// <summary>DFM `ButtonReLoadNpc: TButton`（`Caption = '加载(&L)'`，`Enabled = False`）。</summary>
    public System.Windows.Forms.Button ButtonReLoadNpc = null!;

    /// <summary>DFM `ButtonSave: TButton`（`Caption = '保存(&S)'`）。</summary>
    public System.Windows.Forms.Button ButtonSave = null!;
    /// <summary>DFM `CheckBoxDenyRefStatus: TCheckBox`（`Caption = '刷新状态'`）。</summary>
    public System.Windows.Forms.CheckBox CheckBoxDenyRefStatus = null!;
    /// <summary>DFM `ButtonClearTempData: TButton`（`Caption = '清除数据(&C)'`）。</summary>
    public System.Windows.Forms.Button ButtonClearTempData = null!;
    /// <summary>DFM `ButtonViewData: TButton`（`Caption = '查看数据(&V)'`，`Visible = False`）。</summary>
    public System.Windows.Forms.Button ButtonViewData = null!;

    /// <summary>DFM `GroupBox1: TGroupBox`（`Caption = 'NPC列表:'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    /// <summary>DFM `lblSearch: TLabel`（'搜索：'）。</summary>
    public System.Windows.Forms.Label lblSearch = null!;
    /// <summary>DFM `ListBoxMerChant: TListBox`（`OnClick = ListBoxMerChantClick`）。</summary>
    public System.Windows.Forms.ListBox ListBoxMerChant = null!;
    /// <summary>DFM `edtSearch: TEdit`（**无任何事件绑定** —— 原文如此）。</summary>
    public System.Windows.Forms.TextBox edtSearch = null!;
    /// <summary>DFM `btnSearch: TButton`（`Caption = '开始搜索'`）。</summary>
    public System.Windows.Forms.Button btnSearch = null!;
    /// <summary>DFM `btnSearchNext: TButton`（`Caption = '下一个'`，**无 OnClick 绑定** —— 原文如此）。</summary>
    public System.Windows.Forms.Button btnSearchNext = null!;

    /// <summary>
    /// DFM 的 `Hint` 原文（15 条：窗体 `ShowHint = True`，各控件 `Hint = '...'`）。
    /// <para>
    /// **不挂 `ToolTip`**（那会给控件追加事件 ⇒ 破坏绑定数对账）—— 见偏离 **D-P9-04**。
    /// 键 = DFM 控件名。
    /// </para>
    /// </summary>
    public readonly Dictionary<string, string> DfmHints = new(StringComparer.Ordinal)
    {
        ["EditScriptName"] = "脚本文件名称。文件名称以此名字加地图名组合为实际文件名。",
        ["EditMapName"] = "地图名称。",
        ["CheckBoxOfCastle"] = "指定此NPC属于城堡管理，当攻城时NPC将停止营业。",
        ["ComboBoxDir"] = "默认站立方向。",
        ["EditImageIdx"] = "外观图形。",
        ["EditX"] = "当前座标X。",
        ["EditY"] = "当前座标Y。",
        ["CheckBoxAutoMove"] = "NPC会在地图进行随要移动",
        ["EditMoveTime"] = "随机移动间隔时间秒",
        ["ButtonScriptSave"] = "保存脚本文件。",
        ["EditPriceRate"] = "NPC交易时折扣，80为80%",
        ["ButtonReLoadNpc"] = "重新加载NPC脚本。",
        ["ButtonSave"] = "保存交易NPC设置",
        ["CheckBoxDenyRefStatus"] = "使用此方法，可以刷新NPC在游戏中的数据。打开此选项几秒后再关闭，NPC更改的参数就会在游戏中刷新。",
        ["ButtonClearTempData"] = "清除所有NPC的临时文件，包括临时价格表及交易物品库存，在NPC买卖物品有问题时可使用此功能清理。",
    };

    // ==================================================================
    // :96-97 private 字段
    // ==================================================================

    /// <summary>原文 `:96 SelMerchant: TMerchant;`（当前选中的交易 NPC；nil = 未选中）。</summary>
    public TMerchant? SelMerchant;

    /// <summary>原文 `:97 boOpened: Boolean;`（回填/加载期间为 False，避免把回填当成用户改动）。</summary>
    public bool boOpened;

    /// <summary>`ListBoxMerChant.Items.Objects[nSelIndex]`（:246）的载体镜像（与 `Items` 同序同长）。</summary>
    public readonly List<object?> ItemObjects = new();

    // ==================================================================
    // 接缝
    // ==================================================================

    /// <summary>
    /// 接缝：`UserEngine.m_MerchantList`（:165/:167/:192/:203/:205/:220/:222/:231）。
    /// <para>
    /// **未接线即抛**（§25.2）：本窗体的 `ButtonSaveClick` 会用它**重写 Merchant.txt** ——
    /// 若"未接线"被静默当成空表，会把整份 NPC 配置**覆盖成空文件**，属于灾难性静默失败。
    /// </para>
    /// </summary>
    public Func<IReadOnlyList<TMerchant>?>? MerchantListHandler;

    /// <summary>接缝：`UserEngine.m_MerchantList.LockR(ID)`（:165=11 / :203=12 / :220=13）。</summary>
    public Action<int>? MerchantListLockR;

    /// <summary>接缝：`UserEngine.m_MerchantList.UnLockR`。</summary>
    public Action? MerchantListUnLockR;

    /// <summary>
    /// 接缝：`Application.MessageBox(PChar(...), '确认信息', MB_YESNO + MB_ICONQUESTION)`（:147）。
    /// 默认转调既有 <see cref="M2Forms.MessageBox"/>（测试注入 ⇒ 不弹窗）。
    /// </summary>
    public Func<string, string, int, int>? MessageBoxHandler;

    /// <summary>`g_Config` 读取面（当前只需 `sEnvirDir`，既有 `M2Config` 已提供）。</summary>
    private string sEnvirDir => M2Config.sEnvirDir;

    /// <summary>`UserEngine.m_MerchantList`；未接线即抛。</summary>
    private IReadOnlyList<TMerchant> MerchantList => MerchantListHandler?.Invoke()
        ?? throw Sweep9FormsKit.NotWired(nameof(MerchantListHandler), "UsrEngn.pas UserEngine.m_MerchantList");

    private int ShowMessageBox(string text, string caption, int flags)
        => MessageBoxHandler != null ? MessageBoxHandler(text, caption, flags) : M2Forms.MessageBox(text, caption, flags);

    // ==================================================================
    // 构造（DFM 控件树 1:1）
    // ==================================================================

    /// <summary>构造 + 装载 DFM 控件树（DFM `OnCreate = FormCreate` 由 `Load` 触发）。</summary>
    public TfrmConfigMerchant()
    {
        InitializeComponents();
    }

    // ==================================================================
    // :121-131 ModValue / uModValue
    // ==================================================================

    /// <summary>原文 `:121-125 procedure TfrmConfigMerchant.ModValue;`。</summary>
    public void ModValue()
    {
        ButtonSave.Enabled = true;
        ButtonScriptSave.Enabled = true;
    }

    /// <summary>原文 `:127-131 procedure TfrmConfigMerchant.uModValue;`。</summary>
    public void uModValue()
    {
        ButtonSave.Enabled = false;
        ButtonScriptSave.Enabled = false;
    }

    // ==================================================================
    // :133-143 Open
    // ==================================================================

    /// <summary>原文 `:133-143 procedure TfrmConfigMerchant.Open;`。</summary>
    public void Open()
    {
        boOpened = false;                                      // :135
        uModValue();                                           // :136
        CheckBoxDenyRefStatus.Checked = false;                  // :137
        SelMerchant = null;                                    // :138
        RefListBoxMerChant();                                  // :139
        boOpened = true;                                       // :141
        Sweep9FormsMessageBoxSeam.ShowModal(this);              // :142
    }

    // ==================================================================
    // :145-151 ButtonClearTempDataClick
    // ==================================================================

    /// <summary>原文 `:145-151 procedure TfrmConfigMerchant.ButtonClearTempDataClick(Sender: TObject);`。</summary>
    public void ButtonClearTempDataClick(object? Sender)
    {
        // 原文 if Application.MessageBox(PChar('是否确认清除NPC临时数据？'), '确认信息',
        //        MB_YESNO + MB_ICONQUESTION) = mrYes then
        if (ShowMessageBox("是否确认清除NPC临时数据？", "确认信息", M2Forms.MB_YESNO + M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
        {
            ClearMerchantData();
        }
    }

    // ==================================================================
    // :153-196 ButtonSaveClick
    // ==================================================================

    /// <summary>原文 `:153-196 procedure TfrmConfigMerchant.ButtonSaveClick(Sender: TObject);`。</summary>
    public void ButtonSaveClick(object? Sender)
    {
        // 原文 var I: Integer; SaveList: TStringList; Merchant: TMerchant;
        //        sMerchantFile, sIsCastle, sCanMove, Script: string;
        string sMerchantFile, sIsCastle, sCanMove, Script;

        sMerchantFile = sEnvirDir + "Merchant.txt";            // :163
        var SaveList = new TStringList();                      // :164
        MerchantListLockR?.Invoke(11);                         // :165 LockR(11)
        try
        {
            // 原文 for I := 0 to UserEngine.m_MerchantList.Count - 1 do
            var list = MerchantList;
            for (int I = 0; I < list.Count; I++)
            {
                TMerchant Merchant = list[I];
                if (Merchant.m_sMapName == "0")                // :170
                    continue;

                if (Merchant.m_boCastle) sIsCastle = "1"; else sIsCastle = "0";   // :173-176
                if (Merchant.m_boCanMove) sCanMove = "1"; else sCanMove = "0";    // :177-180

                // 原文 { TODO -ochongchong -c修改 : NPC对应脚本中的\改为/ 【2013-08-28】 }
                //   Script := StringReplace(Script, '\', '/', [rfReplaceAll]);   （:184）
                Script = Merchant.m_sScript;
                Script = Script.Replace('\\', '/');

                // 原文 :186-188 制表符分隔的 10 段
                SaveList.Add(Script + "\t" + Merchant.m_sMapName + "\t" +
                    GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_nCurrX) + "\t" +
                    GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_nCurrY) + "\t" +
                    Merchant.m_sCharName + "\t" +
                    GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_nFlag) + "\t" +
                    GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_wAppr) + "\t" +
                    sIsCastle + "\t" + sCanMove + "\t" +
                    GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_dwMoveTime));
            }
            SaveList.SaveToFile(sMerchantFile);                // :190
        }
        finally
        {
            MerchantListUnLockR?.Invoke();                     // :192
        }
        // 原文 SaveList.Free;（托管侧由 GC 回收）
        uModValue();                                           // :195
    }

    // ==================================================================
    // :198-213 ClearMerchantData
    // ==================================================================

    /// <summary>原文 `:198-213 procedure TfrmConfigMerchant.ClearMerchantData;`。</summary>
    public void ClearMerchantData()
    {
        MerchantListLockR?.Invoke(12);                         // :203 LockR(12)
        try
        {
            var list = MerchantList;
            for (int I = 0; I < list.Count; I++)
            {
                TMerchant Merchant = list[I];
                Merchant.ClearData();                          // :208
            }
        }
        finally
        {
            MerchantListUnLockR?.Invoke();                     // :211
        }
    }

    // ==================================================================
    // :215-234 RefListBoxMerChant
    // ==================================================================

    /// <summary>
    /// 原文 `:215-234 procedure TfrmConfigMerchant.RefListBoxMerChant;`。
    /// <para>★ 原文**不清空** `ListBoxMerChant` ⇒ 反复调用会累积（原文如此）。</para>
    /// </summary>
    public void RefListBoxMerChant()
    {
        MerchantListLockR?.Invoke(13);                         // :220 LockR(13)
        try
        {
            var list = MerchantList;
            for (int I = 0; I < list.Count; I++)
            {
                TMerchant Merchant = list[I];
                if (Merchant.m_sMapName == "0" && Merchant.m_nCurrX == 0 && Merchant.m_nCurrY == 0)
                    continue;                                  // :226

                // 原文 :227-228
                ListBoxMerChant.Items.Add(Merchant.m_sCharName + " - " + Merchant.m_sMapName +
                    " (" + GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_nCurrX) +
                    ":" + GXX.Core.Rtl.DelphiRTL.IntToStr(Merchant.m_nCurrY) + ")");
                ItemObjects.Add(Merchant);                     // 原文 AddObject 的第二参
            }
        }
        finally
        {
            MerchantListUnLockR?.Invoke();                     // :231
        }
    }

    // ==================================================================
    // :236-279 ListBoxMerChantClick
    // ==================================================================

    /// <summary>原文 `:236-279 procedure TfrmConfigMerchant.ListBoxMerChantClick(Sender: TObject);`。</summary>
    public void ListBoxMerChantClick(object? Sender)
    {
        CheckBoxDenyRefStatus.Checked = false;                 // :240
        uModValue();                                           // :241
        boOpened = false;                                      // :242

        int nSelIndex = ListBoxMerChant.SelectedIndex;         // :243 ItemIndex
        if (nSelIndex < 0)                                     // :244
            return;                                            // 原文 Exit

        // 原文 SelMerchant := TMerchant(ListBoxMerChant.Items.Objects[nSelIndex]);
        SelMerchant = (TMerchant?)ItemObjects[nSelIndex];

        EditScriptName.Text = SelMerchant!.m_sScript;           // :247
        EditMapName.Text = SelMerchant.m_sMapName;              // :248
        // 原文 :249 `SelMerchant.m_PEnvir.sMapDesc` —— m_PEnvir 为 nil 时原文会 AV，此处同样解引用
        EditMapDesc.Text = SelMerchant.m_PEnvir!.sMapDesc;
        EditX.Value = SelMerchant.m_nCurrX;                     // :250
        EditY.Value = SelMerchant.m_nCurrY;                     // :251
        EditShowName.Text = SelMerchant.m_sCharName;            // :252
        // 原文 :253 `ComboBoxDir.ItemIndex := SelMerchant.m_nFlag;` —— nFlag 是 ShortInt，
        //   越界时 Delphi **静默置 -1**（《并行派发台账》§21.3）⇒ 用垫片复刻。
        Sweep9FormsKit.SetComboBoxItemIndex(ComboBoxDir, SelMerchant.m_nFlag);
        EditImageIdx.Value = SelMerchant.m_wAppr;               // :254
        CheckBoxOfCastle.Checked = SelMerchant.m_boCastle;       // :255
        CheckBoxAutoMove.Checked = SelMerchant.m_boCanMove;      // :256
        EditMoveTime.Value = SelMerchant.m_dwMoveTime;           // :257

        CheckBoxBuy.Checked = SelMerchant.m_boBuy;               // :259
        CheckBoxSell.Checked = SelMerchant.m_boSell;             // :260
        CheckBoxGetback.Checked = SelMerchant.m_boGetback;       // :261
        CheckBoxStorage.Checked = SelMerchant.m_boStorage;       // :262
        CheckBoxUpgradenow.Checked = SelMerchant.m_boUpgradenow; // :263
        CheckBoxGetbackupgnow.Checked = SelMerchant.m_boGetBackupgnow;   // :264
        CheckBoxRepair.Checked = SelMerchant.m_boRepair;         // :265
        CheckBoxS_repair.Checked = SelMerchant.m_boS_repair;     // :266
        CheckBoxMakedrug.Checked = SelMerchant.m_boMakeDrug;     // :267
        CheckBoxSendMsg.Checked = SelMerchant.m_boSendmsg;       // :268

        EditPriceRate.Value = SelMerchant.m_nPriceRate;          // :270
        MemoScript.Lines.Clear();                                // :271 MemoScript.Clear
        ButtonReLoadNpc.Enabled = false;                         // :272
        LoadScriptFile();                                        // :273

        GroupBoxNPC.Enabled = true;                              // :275
        GroupBoxScript.Enabled = true;                           // :276

        boOpened = true;                                         // :278
    }

    // ==================================================================
    // :281-291 FormCreate
    // ==================================================================

    /// <summary>原文 `:281-291 procedure TfrmConfigMerchant.FormCreate(Sender: TObject);`。</summary>
    public void FormCreate(object? Sender = null)
    {
        // 原文把 '0'..'7' 逐条 Add（未合并为循环 —— 托管侧也逐条照抄）
        ComboBoxDir.Items.Add("0");     // :283
        ComboBoxDir.Items.Add("1");
        ComboBoxDir.Items.Add("2");
        ComboBoxDir.Items.Add("3");
        ComboBoxDir.Items.Add("4");
        ComboBoxDir.Items.Add("5");
        ComboBoxDir.Items.Add("6");
        ComboBoxDir.Items.Add("7");     // :290
    }

    // ==================================================================
    // :293-299 CheckBoxDenyRefStatusClick
    // ==================================================================

    /// <summary>
    /// 原文 `:293-299 procedure TfrmConfigMerchant.CheckBoxDenyRefStatusClick(Sender: TObject);`。
    /// <para>★ 唯一**不判 `boOpened`、也不调 `ModValue`** 的处理器（:295 只判 `SelMerchant &lt;&gt; nil`）。</para>
    /// </summary>
    public void CheckBoxDenyRefStatusClick(object? Sender)
    {
        if (SelMerchant != null)
        {
            SelMerchant.m_boDenyRefStatus = CheckBoxDenyRefStatus.Checked;   // :297
        }
    }

    // ==================================================================
    // :301-379 各 Edit / CheckBox / ComboBox 的 Change/Click（14 个）
    // ==================================================================

    /// <summary>原文 `:301-307 EditXChange`。</summary>
    public void EditXChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_nCurrX = (int)EditX.Value;
        ModValue();
    }

    /// <summary>原文 `:309-315 EditYChange`。</summary>
    public void EditYChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_nCurrY = (int)EditY.Value;
        ModValue();
    }

    /// <summary>原文 `:317-323 EditShowNameChange`。</summary>
    public void EditShowNameChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_sCharName = GXX.Core.Rtl.DelphiRTL.Trim(EditShowName.Text);
        ModValue();
    }

    /// <summary>原文 `:325-331 EditImageIdxChange`。</summary>
    public void EditImageIdxChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_wAppr = (ushort)EditImageIdx.Value;        // Integer → Word
        ModValue();
    }

    /// <summary>原文 `:333-339 EditScriptNameChange`。</summary>
    public void EditScriptNameChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_sScript = GXX.Core.Rtl.DelphiRTL.Trim(EditScriptName.Text);
        ModValue();
    }

    /// <summary>原文 `:341-347 EditMapNameChange`。</summary>
    public void EditMapNameChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_sMapName = GXX.Core.Rtl.DelphiRTL.Trim(EditMapName.Text);
        ModValue();
    }

    /// <summary>原文 `:349-355 ComboBoxDirChange`。</summary>
    public void ComboBoxDirChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_nFlag = (sbyte)ComboBoxDir.SelectedIndex;   // Integer → ShortInt
        ModValue();
    }

    /// <summary>原文 `:357-363 CheckBoxOfCastleClick`。</summary>
    public void CheckBoxOfCastleClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boCastle = CheckBoxOfCastle.Checked;
        ModValue();
    }

    /// <summary>原文 `:365-371 CheckBoxAutoMoveClick`。</summary>
    public void CheckBoxAutoMoveClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boCanMove = CheckBoxAutoMove.Checked;
        ModValue();
    }

    /// <summary>原文 `:373-379 EditMoveTimeChange`。</summary>
    public void EditMoveTimeChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_dwMoveTime = (uint)EditMoveTime.Value;      // Integer → LongWord
        ModValue();
    }

    /// <summary>原文 `:487-494 CheckBoxBuyClick`。</summary>
    public void CheckBoxBuyClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boBuy = CheckBoxBuy.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:496-503 CheckBoxSellClick`。</summary>
    public void CheckBoxSellClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boSell = CheckBoxSell.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:505-512 CheckBoxGetbackClick`。</summary>
    public void CheckBoxGetbackClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boGetback = CheckBoxGetback.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:514-521 CheckBoxStorageClick`。</summary>
    public void CheckBoxStorageClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boStorage = CheckBoxStorage.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:523-530 CheckBoxUpgradenowClick`。</summary>
    public void CheckBoxUpgradenowClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boUpgradenow = CheckBoxUpgradenow.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:532-539 CheckBoxGetbackupgnowClick`。</summary>
    public void CheckBoxGetbackupgnowClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boGetBackupgnow = CheckBoxGetbackupgnow.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:541-548 CheckBoxRepairClick`。</summary>
    public void CheckBoxRepairClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boRepair = CheckBoxRepair.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:550-557 CheckBoxS_repairClick`。</summary>
    public void CheckBoxS_repairClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boS_repair = CheckBoxS_repair.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:559-566 CheckBoxMakedrugClick`。</summary>
    public void CheckBoxMakedrugClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boMakeDrug = CheckBoxMakedrug.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:568-575 CheckBoxSendMsgClick`。</summary>
    public void CheckBoxSendMsgClick(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boSendmsg = CheckBoxSendMsg.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:577-585 EditPriceRateChange`。</summary>
    public void EditPriceRateChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;

        SelMerchant.m_nPriceRate = (int)EditPriceRate.Value;
        // ★ 原文 :583 **不判空**：行数 < 2 时抛出（TStrings[1] 越界）。逐字保留。
        MemoScript.Lines[1] = "%" + GXX.Core.Rtl.DelphiRTL.IntToStr(SelMerchant.m_nPriceRate);
        ModValue();
    }

    /// <summary>原文 `:613-621 chkCreateHeroClick`。</summary>
    public void chkCreateHeroClick(object? Sender)
    {
        // 原文 { TODO -ochongchong -c新增 : 工具-交易npc配制 增加创建英雄 【2013-07-23】 }
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boCreateHeroName = chkCreateHero.Checked;
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:623-631 chkBuHeroClick`。</summary>
    public void chkBuHeroClick(object? Sender)
    {
        // 原文 { TODO -ochongchong -c新增 : 工具-交易npc配制 增加副将英雄 【2013-07-23】 }
        if (!boOpened || SelMerchant == null) return;
        SelMerchant.m_boBuHero = chkBuyHero.Checked;                   // ★ 读的是 chkBuyHero（原文如此）
        ModValue();
        ChangeScriptAllowAction();
    }

    /// <summary>原文 `:606-611 MemoScriptChange`（体内只有脏标志，原文如此）。</summary>
    public void MemoScriptChange(object? Sender)
    {
        if (!boOpened || SelMerchant == null) return;
        ModValue();
    }

    // ==================================================================
    // :381-448 LoadScriptFile
    // ==================================================================

    /// <summary>
    /// 原文 `:381-448 procedure TfrmConfigMerchant.LoadScriptFile;`。
    /// <para>★ 参数行只写 **11** 个开关（不含 `m_boCreateHeroName` / `m_boBuHero`）。</para>
    /// </summary>
    public void LoadScriptFile()
    {
        // 原文 var I: Integer; sScriptFile, LineText: string; LoadList: TStringList; boNoHeader: Boolean;
        string sScriptFile, LineText;
        bool boNoHeader;

        if (SelMerchant == null)                                   // :389
            return;

        sScriptFile = sEnvirDir + "Market_Def\\" + SelMerchant.m_sScript + "-" + SelMerchant.m_sMapName + ".txt";  // :391
        MemoScript.Visible = false;                                // :392
        LineText = "(";                                            // :393
        if (SelMerchant.m_boBuy) LineText = LineText + NpcProcessCmd.sNF_Buy + " ";
        if (SelMerchant.m_boSell) LineText = LineText + NpcProcessCmd.sNF_Sell + " ";
        if (SelMerchant.m_boMakeDrug) LineText = LineText + NpcProcessCmd.sNF_MakedUrg + " ";
        if (SelMerchant.m_boStorage) LineText = LineText + NpcProcessCmd.sNF_Storage + " ";
        if (SelMerchant.m_boGetback) LineText = LineText + NpcProcessCmd.sNF_Getback + " ";
        if (SelMerchant.m_boUpgradenow) LineText = LineText + NpcProcessCmd.sNF_UpgradeNow + " ";
        if (SelMerchant.m_boGetBackupgnow) LineText = LineText + NpcProcessCmd.sNF_GetBackupgNow + " ";
        if (SelMerchant.m_boRepair) LineText = LineText + NpcProcessCmd.sNF_Repair + " ";
        if (SelMerchant.m_boS_repair) LineText = LineText + NpcProcessCmd.sNF_SuperRepair + " ";
        if (SelMerchant.m_boSendmsg) LineText = LineText + NpcProcessCmd.sNF_SendMsg + " ";
        if (SelMerchant.m_boArmRemoveStone) LineText = LineText + NpcProcessCmd.sNF_ArmRemoveStone + " ";

        LineText = LineText + ")";                                 // :417
        MemoScript.Lines.Add(LineText);                            // :418
        LineText = "%" + GXX.Core.Rtl.DelphiRTL.IntToStr(SelMerchant.m_nPriceRate);   // :419
        MemoScript.Lines.Add(LineText);                            // :420

        // 原文 :421-425 逐条把 m_ItemTypeList 的**装箱 Integer** 展开成 '+N'
        for (int I = 0; I < SelMerchant.m_ItemTypeList.Count; I++)
        {
            LineText = "+" + GXX.Core.Rtl.DelphiRTL.IntToStr(Convert.ToInt32(SelMerchant.m_ItemTypeList[I]));
            MemoScript.Lines.Add(LineText);
        }

        if (SweepSeam.FileExists(sScriptFile))                      // :426
        {
            var LoadList = new TStringList();                      // :428
            LoadList.LoadFromFile(sScriptFile);                    // :429（原文无 try ⇒ 读失败即抛）
            boNoHeader = false;                                    // :430
            for (int I = 0; I < LoadList.Count; I++)               // :431
            {
                LineText = LoadList[I];                            // :433
                // 原文 if (LineText = '') or (LineText[1] = ';') then Continue;（:434）
                if (LineText == "" || LineText[0] == ';')
                    continue;

                // 原文 if (LineText[1] = '[') or (LineText[1] = '#') then boNoHeader := True;（:437）
                if (LineText[0] == '[' || LineText[0] == '#')
                    boNoHeader = true;
                if (boNoHeader)                                    // :439
                {
                    MemoScript.Lines.Add(LineText);                // :441
                }
            }
            // 原文 LoadList.Free;（:445，托管侧由 GC 回收）
        }
        MemoScript.Visible = true;                                 // :447
    }

    // ==================================================================
    // :450-485 ChangeScriptAllowAction
    // ==================================================================

    /// <summary>
    /// 原文 `:450-485 procedure TfrmConfigMerchant.ChangeScriptAllowAction;`。
    /// <para>★ 写 **13** 个开关：`CreateHero`/`CreateDeputy` 插在 `SendMsg` 与 `ArmRemoveStone` 之间。</para>
    /// </summary>
    public void ChangeScriptAllowAction()
    {
        // 原文 var LineText: string;
        string LineText;

        if (SelMerchant == null || MemoScript.Lines.Count <= 0)     // :454
            return;

        LineText = "(";                                            // :456
        if (SelMerchant.m_boBuy) LineText = LineText + NpcProcessCmd.sNF_Buy + " ";
        if (SelMerchant.m_boSell) LineText = LineText + NpcProcessCmd.sNF_Sell + " ";
        if (SelMerchant.m_boMakeDrug) LineText = LineText + NpcProcessCmd.sNF_MakedUrg + " ";
        if (SelMerchant.m_boStorage) LineText = LineText + NpcProcessCmd.sNF_Storage + " ";
        if (SelMerchant.m_boGetback) LineText = LineText + NpcProcessCmd.sNF_Getback + " ";
        if (SelMerchant.m_boUpgradenow) LineText = LineText + NpcProcessCmd.sNF_UpgradeNow + " ";
        if (SelMerchant.m_boGetBackupgnow) LineText = LineText + NpcProcessCmd.sNF_GetBackupgNow + " ";
        if (SelMerchant.m_boRepair) LineText = LineText + NpcProcessCmd.sNF_Repair + " ";
        if (SelMerchant.m_boS_repair) LineText = LineText + NpcProcessCmd.sNF_SuperRepair + " ";
        if (SelMerchant.m_boSendmsg) LineText = LineText + NpcProcessCmd.sNF_SendMsg + " ";
        if (SelMerchant.m_boCreateHeroName) LineText = LineText + NpcProcessCmd.sNF_CreateHero + " ";      // :477-478
        if (SelMerchant.m_boBuHero) LineText = LineText + NpcProcessCmd.sNF_CreateDeputy + " ";            // :479-480
        if (SelMerchant.m_boArmRemoveStone) LineText = LineText + NpcProcessCmd.sNF_ArmRemoveStone + " ";
        LineText = LineText + ")";                                 // :483
        MemoScript.Lines[0] = LineText;                            // :484
    }

    // ==================================================================
    // :587-604 ButtonScriptSaveClick / ButtonReLoadNpcClick
    // ==================================================================

    /// <summary>
    /// 原文 `:587-595 procedure TfrmConfigMerchant.ButtonScriptSaveClick(Sender: TObject);`。
    /// <para>★ 原文**不判 `SelMerchant = nil`**（:591 直接解引用）—— 逐字保留。</para>
    /// </summary>
    public void ButtonScriptSaveClick(object? Sender)
    {
        // 原文 sScriptFile := g_Config.sEnvirDir + 'Market_Def\' + SelMerchant.m_sScript + '-' +
        //                     SelMerchant.m_sMapName + '.txt';
        string sScriptFile = sEnvirDir + "Market_Def\\" + SelMerchant!.m_sScript + "-" + SelMerchant.m_sMapName + ".txt";
        MemoScript.Lines.SaveToFile(sScriptFile);                  // :592
        uModValue();                                               // :593
        ButtonReLoadNpc.Enabled = true;                            // :594
    }

    /// <summary>原文 `:597-604 procedure TfrmConfigMerchant.ButtonReLoadNpcClick(Sender: TObject);`。</summary>
    public void ButtonReLoadNpcClick(object? Sender)
    {
        if (SelMerchant == null)                                   // :599
            return;
        SelMerchant.ClearScript();                                 // :601（原文 virtual/override ⇒ 虚分派保留）
        SelMerchant.LoadNpcScript();                               // :602
        ButtonReLoadNpc.Enabled = false;                           // :603
    }

    // ==================================================================
    // :633-652 btnSearchClick
    // ==================================================================

    /// <summary>
    /// 原文 `:633-652 procedure TfrmConfigMerchant.btnSearchClick(Sender: TObject);`。
    /// <para>★ 命中后**不 Break** ⇒ 多个匹配时最终停在最后一个。</para>
    /// </summary>
    public void btnSearchClick(object? Sender)
    {
        // 原文 var I: Integer; S, S2: string;
        string S, S2;

        S = GXX.Core.Rtl.DelphiRTL.Trim(edtSearch.Text);           // :638
        S = GXX.Core.Rtl.DelphiRTL.LowerCase(S);                   // :639
        if (S.Length == 0)                                         // :640
            return;

        for (int I = 0; I < ListBoxMerChant.Items.Count; I++)      // :643
        {
            S2 = GXX.Core.Rtl.DelphiRTL.LowerCase(ListBoxMerChant.Items[I].ToString() ?? "");
            if (GXX.Core.Rtl.DelphiRTL.Pos(S, S2) > 0)             // :646
            {
                // 原文 :648 `ListBoxMerChant.ItemIndex := I;`（下标在范围内；垫片复刻 Delphi 语义）
                Sweep9FormsKit.SetListBoxItemIndex(ListBoxMerChant, I);
                // 原文 :649 `ListBoxMerChant.OnClick(ListBoxMerChant);` —— **直接调处理器**
                ListBoxMerChantClick(ListBoxMerChant);
            }
        }
    }
}

/// <summary>
/// 原文 `ConfigMerchant.pas:111 var frmConfigMerchant: TfrmConfigMerchant;`（单元级全局窗体变量）。
/// <para>创建点 `svMain.pas:2778`、`Open` `:2780`、释放 `:2782`（均不在本单元内）。</para>
/// </summary>
public static class Sweep9FormsConfigMerchantGlobals
{
    /// <summary>原文 `frmConfigMerchant`（未创建时为 <c>null</c> == 原文 nil）。</summary>
    public static TfrmConfigMerchant? frmConfigMerchant;

    /// <summary>测试隔离。</summary>
    public static void Reset() => frmConfigMerchant = null;
}
