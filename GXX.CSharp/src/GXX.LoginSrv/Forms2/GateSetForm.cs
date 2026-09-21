using System;
using System.Windows.Forms;
using GXX.Core.Rtl;
using GXX.Core.Util;

// 源单元：Source/LoginSrv/GateSet.pas（302 行）→ 本文件（1:1 移植）
// DFM：Source/LoginSrv/GateSet.dfm（6,155 字节，**二进制 DFM**）—— 按 §41.3-1 回读 `Source/**` 原始字节
//      手工解码（`_analysis/utf8_mirror` 里的副本已损坏，**不可用**）。
//      实测：25 字节头 `FF 0A 00` + 'TFRMGATESETTING' + 00 + `30 10` + Int32(0x17F2=6130)，
//      流从偏移 25 的 'TPF0' 起、解到 **6155 = 文件长度**（零残留）。
//      对账：DFM 控件 **49** / 托管字段 **49**（见类注释的分组清单）；
//      DFM 事件绑定 **6**（根 OnCreate/OnDestroy + BtnOk.OnClick + BtnChangeTitle.OnClick
//        + CbGateList.OnChange + CbServerList.OnChange）/ 托管 `+=` **6**。
// 方法对账：原文 **8**（FormCreate / FormDestroy / CbGateListChange / BtnOkClick /
//      BtnChangeTitleClick / CbServerListChange / RefRouteList / Open）→ 托管 **8**。
// 消费者：`LoginSrv.dpr:5 uses GateSet`；`LMain.pas:228-233 OpenRouteConfig` 是唯一调用点
//      （Create(nil) → Open → Free；同文件 :235-244 的另一版**整段被注释**，故无第二调用点）。

namespace GXX.LoginSrv.Forms2;

/// <summary>
/// `GateSet.pas:9-73 TFrmGateSetting`（DFM: GateSet.dfm，二进制）。
///
/// <para>DFM 控件 49 个（分组与 `GateSet.pas:10-58` 的声明顺序一致）：</para>
/// <list type="bullet">
/// <item>根级 5：BtnOk / BtnClose / GroupBox1 / GroupBox2 / GroupBox3；</item>
/// <item>GroupBox1（角色网关设置）34：Label5…Label18（14）+ CkGate1…CkGate10（10）+ EdGate1…EdGate10（10）；</item>
/// <item>GroupBox2（登录网关信息）4：Label4 / Label3 / EdPublicAddr / EdPrivateAddr；</item>
/// <item>GroupBox3（网关路由标识）6：Label2 / Label1 / BtnChangeTitle / CbGateList / CbServerList / EdTitle。</item>
/// </list>
/// </summary>
public class TFrmGateSetting : Form
{
    // ---- 根级 ----
    public Button BtnOk = null!;                 // DFM: TBitBtn
    public Button BtnClose = null!;              // DFM: TBitBtn, ModalResult=1 (mrOk)
    public GroupBox GroupBox1 = null!;
    public GroupBox GroupBox2 = null!;
    public GroupBox GroupBox3 = null!;

    // ---- GroupBox1 的 14 个 Label ----
    public Label Label5 = null!;
    public Label Label6 = null!;
    public Label Label7 = null!;
    public Label Label8 = null!;
    public Label Label9 = null!;
    public Label Label10 = null!;
    public Label Label11 = null!;
    public Label Label12 = null!;
    public Label Label13 = null!;
    public Label Label14 = null!;
    public Label Label15 = null!;
    public Label Label16 = null!;
    public Label Label17 = null!;
    public Label Label18 = null!;

    // ---- GroupBox1 的 10 + 10 ----
    public CheckBox CkGate1 = null!;
    public CheckBox CkGate2 = null!;
    public CheckBox CkGate3 = null!;
    public CheckBox CkGate4 = null!;
    public CheckBox CkGate5 = null!;
    public CheckBox CkGate6 = null!;
    public CheckBox CkGate7 = null!;
    public CheckBox CkGate8 = null!;
    public CheckBox CkGate9 = null!;
    public CheckBox CkGate10 = null!;
    public TextBox EdGate1 = null!;
    public TextBox EdGate2 = null!;
    public TextBox EdGate3 = null!;
    public TextBox EdGate4 = null!;
    public TextBox EdGate5 = null!;
    public TextBox EdGate6 = null!;
    public TextBox EdGate7 = null!;
    public TextBox EdGate8 = null!;
    public TextBox EdGate9 = null!;
    public TextBox EdGate10 = null!;

    // ---- GroupBox2 ----
    public TextBox EdPublicAddr = null!;
    public Label Label4 = null!;
    public Label Label3 = null!;
    public TextBox EdPrivateAddr = null!;

    // ---- GroupBox3 ----
    public ComboBox CbGateList = null!;
    public Label Label2 = null!;
    public Label Label1 = null!;
    public ComboBox CbServerList = null!;
    public TextBox EdTitle = null!;
    public Button BtnChangeTitle = null!;        // DFM: TSpeedButton（扁平）

    /// <summary>`GateSet.pas:66 EdGate: array[0..9] of TEdit`（**由 FormCreate 填充**，原文如此）。</summary>
    public readonly TextBox[] EdGate = new TextBox[GateSetConfigSeam.GateCount];

    /// <summary>`GateSet.pas:67 CkGate: array[0..9] of TCheckBox`（**由 FormCreate 填充**，原文如此）。</summary>
    public readonly CheckBox[] CkGate = new CheckBox[GateSetConfigSeam.GateCount];

    /// <summary>`GateSet.pas:76 var FrmGateSetting: TFrmGateSetting`（`LMain.pas:230` 赋值）。</summary>
    public static TFrmGateSetting? FrmGateSetting;

    /// <summary>
    /// DFM 的 `Hint` 承载（Delphi 由 `Application.Hint` + `ShowHint` 实现；WinForms 用 `ToolTip` 组件）。
    /// 公开只读以便单测断言"提示串确实挂上了"。
    /// </summary>
    public readonly ToolTip HintProvider = new();

    public TFrmGateSetting()
    {
        // DFM: FrmGateSetting Left=474 Top=250 BorderIcons=[biSystemMenu,biMinimize] BorderStyle=bsSingle
        //      Caption='网关路由配置' ClientHeight=348 ClientWidth=448 Position=poMainFormCenter
        //      ShowHint=True PixelsPerInch=96 TextHeight=12 Font.Charset=ANSI_CHARSET Font.Name='宋体'
        Text = "网关路由配置";
        ClientSize = new System.Drawing.Size(448, 348);
        Location = new System.Drawing.Point(474, 250);
        FormBorderStyle = FormBorderStyle.FixedSingle;      // bsSingle
        MaximizeBox = false;                               // BorderIcons 不含 biMaximize
        MinimizeBox = true;                                // BorderIcons 含 biMinimize
        StartPosition = FormStartPosition.CenterParent;    // poMainFormCenter
        // 装饰性属性按 §7 简化：Font.Charset/Name/Height、PixelsPerInch、TextHeight 未复刻。

        // DFM: BtnOk (64,320,105,25) Hint='将网关路由设置保存到配置文件中。' Caption='保存(&S)'
        //      TabOrder=0 OnClick=BtnOkClick Glyph.Data=<vaBinary 482> NumGlyphs=2
        BtnOk = new Button { Left = 64, Top = 320, Width = 105, Height = 25, Text = "保存(&S)", TabIndex = 0 };
        // DFM: BtnClose (336,320,75,25) Hint='退出网关路由配置' Caption='确定(&O)' ModalResult=1 TabOrder=1
        //      ★ 原文如此：关闭按钮的标题是「确定(&O)」，不是「取消」。
        BtnClose = new Button { Left = 336, Top = 320, Width = 75, Height = 25, Text = "确定(&O)", TabIndex = 1, DialogResult = DialogResult.OK };
        // Glyph.Data（482 字节位图）与 NumGlyphs=2 未复刻：TBitBtn 字形属装饰性资源（§7）。

        // DFM: GroupBox3 (8,8,433,73) Caption='网关路由标识' TabOrder=4
        GroupBox3 = new GroupBox { Left = 8, Top = 8, Width = 433, Height = 73, Text = "网关路由标识", TabIndex = 4 };
        // DFM: Label1 (8,20,54,12) '服务器名:'
        Label1 = new Label { Left = 8, Top = 20, Width = 54, Height = 12, Text = "服务器名:" };
        // DFM: Label2 (8,44,54,12) '路由标识:'
        Label2 = new Label { Left = 8, Top = 44, Width = 54, Height = 12, Text = "路由标识:" };
        // DFM: CbServerList (72,16,161,20) Style=csDropDownList ItemHeight=12 TabOrder=1 OnChange=CbServerListChange
        CbServerList = new ComboBox { Left = 72, Top = 16, Width = 161, Height = 20, TabIndex = 1, DropDownStyle = ComboBoxStyle.DropDownList, ItemHeight = 12 };
        // DFM: CbGateList (72,40,161,20) Style=csDropDownList ItemHeight=12 TabOrder=0 OnChange=CbGateListChange
        CbGateList = new ComboBox { Left = 72, Top = 40, Width = 161, Height = 20, TabIndex = 0, DropDownStyle = ComboBoxStyle.DropDownList, ItemHeight = 12 };
        // DFM: EdTitle (240,40,121,21) Font.Charset=DEFAULT_CHARSET Font.Color=clNavy Font.Height=-11
        //      Font.Name='MS Sans Serif' Font.Style=[] ParentFont=False TabOrder=2
        EdTitle = new TextBox { Left = 240, Top = 40, Width = 121, Height = 21, TabIndex = 2, Font = new System.Drawing.Font("MS Sans Serif", 8.25F) };
        EdTitle.ForeColor = System.Drawing.Color.Navy;      // Font.Color=clNavy（ParentFont=False 的显式覆盖）
        // DFM: BtnChangeTitle (368,40,57,22) Caption='修改(&M)' OnClick=BtnChangeTitleClick（TSpeedButton，无 TabOrder）
        BtnChangeTitle = new Button { Left = 368, Top = 40, Width = 57, Height = 22, Text = "修改(&M)", FlatStyle = FlatStyle.Flat, TabStop = false };

        // DFM: GroupBox2 (8,88,433,57) Caption='登录网关信息' TabOrder=3
        GroupBox2 = new GroupBox { Left = 8, Top = 88, Width = 433, Height = 57, Text = "登录网关信息", TabIndex = 3 };
        // DFM: Label3 (8,28,54,12) '内部地址:'
        Label3 = new Label { Left = 8, Top = 28, Width = 54, Height = 12, Text = "内部地址:" };
        // DFM: Label4 (200,28,54,12) '外部地址:'
        Label4 = new Label { Left = 200, Top = 28, Width = 54, Height = 12, Text = "外部地址:" };
        // DFM: EdPrivateAddr (72,24,121,20) Hint='登录网关连接到登录服务器IP地址。' TabOrder=1 Text='5.5.2.1'
        EdPrivateAddr = new TextBox { Left = 72, Top = 24, Width = 121, Height = 20, TabIndex = 1, Text = "5.5.2.1" };
        // DFM: EdPublicAddr (256,24,121,20) Hint='登录网关对外服务的IP地址。' TabOrder=0 Text='210.121.143.202'
        EdPublicAddr = new TextBox { Left = 256, Top = 24, Width = 121, Height = 20, TabIndex = 0, Text = "210.121.143.202" };

        // DFM: GroupBox1 (8,152,433,161) Caption='角色网关设置' TabOrder=2
        GroupBox1 = new GroupBox { Left = 8, Top = 152, Width = 433, Height = 161, Text = "角色网关设置", TabIndex = 2 };
        // DFM 表头：Label5(32,16)'角色网关:' Label6(168,16)'是否启用' Label7(240,16)'角色网关:' Label8(376,16)'是否启用'
        Label5 = new Label { Left = 32, Top = 16, Width = 54, Height = 12, Text = "角色网关:" };
        Label6 = new Label { Left = 168, Top = 16, Width = 48, Height = 12, Text = "是否启用" };
        Label7 = new Label { Left = 240, Top = 16, Width = 54, Height = 12, Text = "角色网关:" };
        Label8 = new Label { Left = 376, Top = 16, Width = 48, Height = 12, Text = "是否启用" };
        // DFM 行号：左列 Label9..Label13(15,36/60/84/108/132)='1'..'5'；右列 Label14..Label18(223,…)='6'..'10'
        Label9 = new Label { Left = 15, Top = 36, Width = 6, Height = 12, Text = "1" };
        Label10 = new Label { Left = 15, Top = 60, Width = 6, Height = 12, Text = "2" };
        Label11 = new Label { Left = 15, Top = 84, Width = 6, Height = 12, Text = "3" };
        Label12 = new Label { Left = 15, Top = 108, Width = 6, Height = 12, Text = "4" };
        Label13 = new Label { Left = 15, Top = 132, Width = 6, Height = 12, Text = "5" };
        Label14 = new Label { Left = 223, Top = 36, Width = 6, Height = 12, Text = "6" };
        Label15 = new Label { Left = 223, Top = 60, Width = 6, Height = 12, Text = "7" };
        Label16 = new Label { Left = 223, Top = 84, Width = 6, Height = 12, Text = "8" };
        Label17 = new Label { Left = 223, Top = 108, Width = 6, Height = 12, Text = "9" };
        Label18 = new Label { Left = 223, Top = 132, Width = 12, Height = 12, Text = "10" };

        // DFM: CkGate1..CkGate5 (184,32/56/80/104/128,15,17) Checked=True State=cbChecked
        //      CkGate6..CkGate10 (392,32/56/80/104/128,15,17) Checked=True State=cbChecked
        //      Caption 原文为 'CkGate1'（第 1 个）/ 'CheckBox1'（其余 9 个）—— **照抄**，不"美化"。
        CkGate1 = MakeGateCheck("CkGate1", 184, 32, 0);
        CkGate2 = MakeGateCheck("CheckBox1", 184, 56, 2);
        CkGate3 = MakeGateCheck("CheckBox1", 184, 80, 4);
        CkGate4 = MakeGateCheck("CheckBox1", 184, 104, 6);
        CkGate5 = MakeGateCheck("CheckBox1", 184, 128, 8);
        CkGate6 = MakeGateCheck("CheckBox1", 392, 32, 10);
        CkGate7 = MakeGateCheck("CheckBox1", 392, 56, 12);
        CkGate8 = MakeGateCheck("CheckBox1", 392, 80, 14);
        CkGate9 = MakeGateCheck("CheckBox1", 392, 104, 16);
        CkGate10 = MakeGateCheck("CheckBox1", 392, 128, 18);

        // DFM: EdGate1..EdGate5 (32,32/56/80/104/128,137,20) TabOrder 1/3/5/7/9，初值见下
        //      EdGate6..EdGate10 (240,32/56/80/104/128,137,20) TabOrder 11/13/15/17/19
        EdGate1 = MakeGateEdit(32, 32, 1, "210.121.143.202");
        EdGate2 = MakeGateEdit(32, 56, 3, "210.121.143.203");
        EdGate3 = MakeGateEdit(32, 80, 5, "210.121.143.203");
        EdGate4 = MakeGateEdit(32, 104, 7, "210.121.143.203");
        EdGate5 = MakeGateEdit(32, 128, 9, "210.121.143.203");
        EdGate6 = MakeGateEdit(240, 32, 11, "210.121.143.203");
        EdGate7 = MakeGateEdit(240, 56, 13, "210.121.143.203");
        EdGate8 = MakeGateEdit(240, 80, 15, "210.121.143.203");
        EdGate9 = MakeGateEdit(240, 104, 17, "210.121.143.203");
        EdGate10 = MakeGateEdit(240, 128, 19, "210.121.143.203");

        GroupBox1.Controls.AddRange(new Control[]
        {
            Label5, Label6, Label7, Label8, Label9, Label10, Label11, Label12, Label13,
            Label14, Label15, Label16, Label17, Label18,
            CkGate1, EdGate1, CkGate2, EdGate2, CkGate3, EdGate3, CkGate4, EdGate4, CkGate5, EdGate5,
            CkGate6, EdGate6, CkGate7, EdGate7, CkGate8, EdGate8, CkGate9, EdGate9, CkGate10, EdGate10,
        });
        GroupBox2.Controls.AddRange(new Control[] { Label3, Label4, EdPrivateAddr, EdPublicAddr });
        GroupBox3.Controls.AddRange(new Control[] { Label1, Label2, CbServerList, CbGateList, EdTitle, BtnChangeTitle });

        Controls.AddRange(new Control[] { GroupBox3, GroupBox2, GroupBox1, BtnOk, BtnClose });

        // DFM 提示串（Hint + ParentShowHint 默认真 + 根 ShowHint=True）
        HintProvider.SetToolTip(BtnOk, "将网关路由设置保存到配置文件中。");
        HintProvider.SetToolTip(BtnClose, "退出网关路由配置");
        foreach (TextBox ed in new[] { EdGate1, EdGate2, EdGate3, EdGate4, EdGate5, EdGate6, EdGate7, EdGate8, EdGate9, EdGate10 })
            HintProvider.SetToolTip(ed, "角色选择网关IP，及端口。");
        HintProvider.SetToolTip(EdPublicAddr, "登录网关对外服务的IP地址。");
        HintProvider.SetToolTip(EdPrivateAddr, "登录网关连接到登录服务器IP地址。");

        // DFM 绑定（6/6）
        Load += (s, e) => FormCreate(s);                    // 根 OnCreate = FormCreate
        FormClosed += (s, e) => FormDestroy(s);             // 根 OnDestroy = FormDestroy（Delphi OnDestroy ↔ FormClosed）
        BtnOk.Click += (s, e) => BtnOkClick(s);
        BtnChangeTitle.Click += (s, e) => BtnChangeTitleClick(s);
        // Delphi `TComboBox.OnChange`（csDropDownList ⇒ 只能选不能打字）↔ WinForms SelectedIndexChanged
        CbGateList.SelectedIndexChanged += (s, e) => CbGateListChange(s);
        CbServerList.SelectedIndexChanged += (s, e) => CbServerListChange(s);
    }

    private CheckBox MakeGateCheck(string caption, int left, int top, int tabIndex)
        => new CheckBox { Left = left, Top = top, Width = 15, Height = 17, Text = caption, Checked = true, TabIndex = tabIndex };

    private TextBox MakeGateEdit(int left, int top, int tabIndex, string text)
        => new TextBox { Left = left, Top = top, Width = 137, Height = 20, TabIndex = tabIndex, Text = text };

    /// <summary>
    /// `GateSet.pas:85-107 TFrmGateSetting.FormCreate` 1:1：把 10 个 `EdGateN` / `CkGateN`
    /// 逐个装进 `EdGate[0..9]` / `CkGate[0..9]` 数组（原文就是手写 20 行赋值）。
    /// </summary>
    public void FormCreate(object? Sender)
    {
        EdGate[0] = EdGate1;
        EdGate[1] = EdGate2;
        EdGate[2] = EdGate3;
        EdGate[3] = EdGate4;
        EdGate[4] = EdGate5;
        EdGate[5] = EdGate6;
        EdGate[6] = EdGate7;
        EdGate[7] = EdGate8;
        EdGate[8] = EdGate9;
        EdGate[9] = EdGate10;
        CkGate[0] = CkGate1;
        CkGate[1] = CkGate2;
        CkGate[2] = CkGate3;
        CkGate[3] = CkGate4;
        CkGate[4] = CkGate5;
        CkGate[5] = CkGate6;
        CkGate[6] = CkGate7;
        CkGate[7] = CkGate8;
        CkGate[8] = CkGate9;
        CkGate[9] = CkGate10;
    }

    /// <summary>`GateSet.pas:109-112 TFrmGateSetting.FormDestroy`：**空实现**（原文如此，逐字保留）。</summary>
    public void FormDestroy(object? Sender)
    {
    }

    /// <summary>
    /// `GateSet.pas:115-160 CbGateListChange` 1:1。
    ///
    /// <para>
    /// ★ 原文要点（照抄）：
    /// ① 从 `CbServerList` 取服务器名、从 `CbGateList` 取标题，**两者任一未选中就 Exit**；
    /// ② `sServerName` 取出来之后**从未被使用**（原文残留的死局部变量，逐字保留）；
    /// ③ 按标题在 `GateRoute[Low..High]`（**全 60 槽**）里线性找路由，找不到就 Exit；
    /// ④ 用选中的路由回填 `EdPrivateAddr`/`EdPublicAddr`，再无条件刷满 10 组
    ///   `EdGateN.Text = 'IP:Port'`（IP 为空则清空）与 `CkGateN.Checked`。
    /// </para>
    /// </summary>
    public void CbGateListChange(object? Sender)
    {
        int nSelServerIdx = P10cComboBox.GetItemIndex(CbServerList);
        if (nSelServerIdx < 0) return;                                       // exit
        string sServerName = (string)CbServerList.Items[nSelServerIdx];      // ★ 原文取而未用（照抄）
        int nSelTitlIdx = P10cComboBox.GetItemIndex(CbGateList);
        if (nSelTitlIdx < 0) return;                                         // exit
        string sTitle = (string)CbGateList.Items[nSelTitlIdx];
        EdTitle.Text = sTitle;
        int nSelRouteIdx = -1;
        for (int I = 0; I <= GateSetConfigSeam.GateRoute.Length - 1; I++)    // Low..High(GateRoute) = 0..59
        {
            if (GateSetConfigSeam.GateRoute[I].sTitle == sTitle)
            {
                nSelRouteIdx = I;
                break;
            }
        }
        if (nSelRouteIdx < 0) return;                                        // exit

        EdPrivateAddr.Text = GateSetConfigSeam.GateRoute[nSelRouteIdx].sRemoteAddr;
        EdPublicAddr.Text = GateSetConfigSeam.GateRoute[nSelRouteIdx].sPublicAddr;
        int nGateIdx = 0;
        while (true)
        {
            if (GateSetConfigSeam.GateRoute[nSelRouteIdx].Gate[nGateIdx].sIPaddr != "")
            {
                EdGate[nGateIdx].Text = GateSetConfigSeam.GateRoute[nSelRouteIdx].Gate[nGateIdx].sIPaddr
                    + ":" + DelphiRTL.IntToStr(GateSetConfigSeam.GateRoute[nSelRouteIdx].Gate[nGateIdx].nPort);
            }
            else
            {
                EdGate[nGateIdx].Text = "";
            }
            CkGate[nGateIdx].Checked = GateSetConfigSeam.GateRoute[nSelRouteIdx].Gate[nGateIdx].boEnable;
            nGateIdx++;
            if (nGateIdx >= GateSetConfigSeam.GateCount) break;
        }
    }

    /// <summary>
    /// `GateSet.pas:164-228 BtnOkClick` 1:1。
    ///
    /// <para>
    /// ★ 原文缺陷（逐条保留 + 差异断言锁定，见测试）：
    /// </para>
    /// <list type="number">
    /// <item>**校验循环不重置 `sIPaddr`/`sPort`**：`sGateAddr = ''` 时**不调用** `GetValidStr3`，
    ///   于是沿用上一轮的取值 ⇒「第 0 槽有效、后面留空」会**通过校验**（空槽被静默当作有效）；
    ///   只有第 0 槽本身为空时才会因初值 `sIPaddr = ''` 而 `Beep; exit`。</item>
    /// <item>**路由查找循环停在第 59 槽之前**：`if nGateIdx &gt;= 59 then break;` 是硬编码 59
    ///   （`High(GateRoute) = 59`）⇒ 索引 **59 号路由永远匹配不到**，且 59 号槽位的标题改了名字也找不到。</item>
    /// <item>`Beep` 之后直接 `exit`，**不弹任何提示**（用户只会听到一声响）。</item>
    /// <item>第 3 段写回循环**无条件**覆盖 10 组网关（含空槽 → 清空），即使该槽的 `EdGateN` 文本为空。</item>
    /// </list>
    /// </summary>
    public void BtnOkClick(object? Sender)
    {
        int nTitleIdx = P10cComboBox.GetItemIndex(CbGateList);
        if (nTitleIdx < 0) return;                                           // exit
        int nGateIdx = 0;
        string sIPaddr = "";                                                 // ★ 原文：循环外声明、**不逐轮重置**
        string sPort = "";
        while (true)
        {
            string sGateAddr = DelphiRTL.Trim(EdGate[nGateIdx].Text);
            if (sGateAddr != "")
            {
                sPort = HUtil32.GetValidStr3(sGateAddr, ref sIPaddr, new[] { ':' });
            }
            if ((sIPaddr == "") || (DelphiRTL.StrToIntDef(sPort, 0) == 0))
            {
                GateSetConfigSeam.Beep();
                return;                                                      // exit
            }
            nGateIdx++;
            if (nGateIdx >= GateSetConfigSeam.GateCount) break;
        }
        string sTitle = (string)CbGateList.Items[nTitleIdx];
        int nRouteIdx = -1;
        nGateIdx = 0;
        while (true)
        {
            if (GateSetConfigSeam.GateRoute[nGateIdx].sTitle == sTitle)
            {
                nRouteIdx = nGateIdx;
                break;
            }
            nGateIdx++;
            if (nGateIdx >= 59) break;                                       // ★ 原文硬编码 59（High=59）
        }
        if (nRouteIdx < 0) return;                                           // exit
        GateSetConfigSeam.GateRoute[nRouteIdx].sRemoteAddr = EdPrivateAddr.Text;
        GateSetConfigSeam.GateRoute[nRouteIdx].sPublicAddr = EdPublicAddr.Text;
        nGateIdx = 0;
        while (true)
        {
            sPort = HUtil32.GetValidStr3(DelphiRTL.Trim(EdGate[nGateIdx].Text), ref sIPaddr, new[] { ':' });
            if (sIPaddr != "")
            {
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].sIPaddr = sIPaddr;
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].nPort = DelphiRTL.StrToIntDef(sPort, 0);
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].boEnable = CkGate[nGateIdx].Checked;
            }
            else
            {
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].sIPaddr = "";
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].nPort = 0;
                GateSetConfigSeam.GateRoute[nRouteIdx].Gate[nGateIdx].boEnable = false;
            }
            nGateIdx++;
            if (nGateIdx >= GateSetConfigSeam.GateCount) break;
        }
        GateSetConfigSeam.SaveGateConfig();
    }

    /// <summary>
    /// `GateSet.pas:231-246 BtnChangeTitleClick` 1:1。
    ///
    /// <para>
    /// ★★ 原文缺陷（本单元最有价值的一条）：最后写回用的是
    /// <c>Config.GateRoute[nTitleIdx].sTitle := sTitle</c> —— **`nTitleIdx` 是"过滤后的下拉框序号"，
    /// 不是 `GateRoute` 的下标**。`CbServerListChange` 只把"当前服务器名"的路由塞进 `CbGateList`，
    /// 所以只要选中的服务器不是第 0 个服务器（或该服务器的第 1 条路由不在 `GateRoute[0]`），
    /// **改名就会写到另一条路由上**（把 A 路由的标题改成 B 的名字，而 B 的标题没变）。
    /// 下拉框里的显示改了、磁盘上的配置却改错了对象。
    /// </para>
    /// </summary>
    public void BtnChangeTitleClick(object? Sender)
    {
        int nTitleIdx = P10cComboBox.GetItemIndex(CbGateList);
        if (nTitleIdx < 0) return;                                           // exit
        string sEdTitle = DelphiRTL.Trim(EdTitle.Text);
        string sTitle = HUtil32.ReplaceChar(sEdTitle, ' ', '_');
        CbGateList.Items[nTitleIdx] = sTitle;
        GateSetConfigSeam.GateRoute[nTitleIdx].sTitle = sTitle;               // ★ 原文用下拉框序号索引 GateRoute
        P10cComboBox.SetItemIndex(CbGateList, nTitleIdx);
    }

    /// <summary>
    /// `GateSet.pas:249-268 CbServerListChange` 1:1：
    /// 清空 `CbGateList` → 按服务器名过滤填标题 → `ItemIndex := 0` → 转调 `CbGateListChange(Self)`。
    ///
    /// <para>
    /// ★ `ItemIndex := 0` 落在"`CbGateList` 可能为空"的路径上（该服务器一条路由都没有）——
    /// Delphi 走 `CB_SETCURSEL` 失败后**静默保持 -1**，WinForms 直接赋 `SelectedIndex = 0`
    /// 会抛 <see cref="ArgumentOutOfRangeException"/> ⇒ 用
    /// <see cref="P10cComboBox.SetItemIndex"/> 还原 Delphi 语义（§21.3 已知陷阱）。
    /// </para>
    /// </summary>
    public void CbServerListChange(object? Sender)
    {
        int nSelIdx = P10cComboBox.GetItemIndex(CbServerList);
        if (nSelIdx < 0) return;                                             // exit
        string sServerName = (string)CbServerList.Items[nSelIdx];
        CbGateList.Items.Clear();
        for (int I = 0; I <= GateSetConfigSeam.nRouteCount - 1; I++)
        {
            if (GateSetConfigSeam.GateRoute[I].sServerName == sServerName)
                CbGateList.Items.Add(GateSetConfigSeam.GateRoute[I].sTitle);
        }
        P10cComboBox.SetItemIndex(CbGateList, 0);
        CbGateListChange(this);                                              // CbGateListChange(Self)
    }

    /// <summary>
    /// `GateSet.pas:270-291 RefRouteList` 1:1：按 `nRouteCount` 收集**去重**的服务器名到 `CbServerList`，
    /// 选中第 0 项并转调 `CbServerListChange(Self)`。
    ///
    /// <para>
    /// ★ 原文缺陷：去重用的局部变量名叫 `boAdded`（"已加入"），但它的语义是**"尚未存在"**
    /// （初值 True、命中相同项时置 False、为 True 才 `Add`）—— 变量名与语义相反，逐字保留。
    /// 另：`nRouteCount` 若超过 60，`Config.GateRoute[I]` 在 Delphi 里是**无检查的越界读写**，
    /// 托管数组会抛 <see cref="IndexOutOfRangeException"/>（登记 D-P10-16）。
    /// </para>
    /// </summary>
    public void RefRouteList()
    {
        if (GateSetConfigSeam.nRouteCount <= 0) return;                      // exit
        CbServerList.Items.Clear();
        for (int I = 0; I <= GateSetConfigSeam.nRouteCount - 1; I++)
        {
            bool boAdded = true;                                             // ★ 名实相反（原文如此）
            for (int II = 0; II <= CbServerList.Items.Count - 1; II++)
            {
                if (GateSetConfigSeam.GateRoute[I].sServerName == (string)CbServerList.Items[II])
                    boAdded = false;                                          // ★ 原文无 break，逐字保留
            }
            if (boAdded) CbServerList.Items.Add(GateSetConfigSeam.GateRoute[I].sServerName);
        }
        P10cComboBox.SetItemIndex(CbServerList, 0);
        CbServerListChange(this);
    }

    /// <summary>`GateSet.pas:294-299 TFrmGateSetting.Open`：先 `RefRouteList`，再 `ShowModal = mrOK`。</summary>
    public bool Open()
    {
        RefRouteList();
        bool Result = false;
        if (GateSetUi.ShowModal(this) == DialogResult.OK) Result = true;      // mrOK = 1
        return Result;
    }
}
