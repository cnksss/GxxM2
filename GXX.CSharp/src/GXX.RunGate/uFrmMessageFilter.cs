using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmMessageFilter.pas 1:1 转换（Source\RunGate\uFrmMessageFilter.pas，233 行 / LF 232）。
// 布局真源：Source\RunGate\uFrmMessageFilter.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：消息文字过滤设置 —— 过滤词列表编辑 + 5 种过滤模式单选 + 警告文本 + 触发脚本开关。
//
// ★ 原文缺陷（照抄并在测试中固定行为）：
//   D1. `btnEditClick`（原 :135-152）**缺少"未选中项"的保护**：
//         if (ItemIndex >= 0) and (ItemIndex < Items.Count) then
//           begin sInputText := Items[ItemIndex]; if not InputQuery(...) then Exit; end;
//         // ← 若条件为假（ItemIndex = -1），sInputText 保持**未初始化**（Delphi 局部 string 初值为 ''）
//         if sInputText = '' then 弹窗并 Exit;
//         lstFilterText.Items[lstFilterText.ItemIndex] := sInputText;   // ← ItemIndex = -1 → 运行时越界
//       即：**未选中任何项时双击/点"修改"不会走 InputQuery，而是先弹"请输入正确的文本！！！"**（因为 sInputText=''）。
//       只有 `InputQuery` 返回 True 且用户输入空串时才会再弹一次。本移植用 `string sInputText = "";` 复刻该行为，
//       并把"未选中"分支显式前置判断（等价于原行为：弹窗 + Exit）。
//   D2. `btnDelClick`（原 :175-195）删除后按"删除位置"重设 ItemIndex：
//         if nSelectIndex >= Count then ItemIndex := nSelectIndex - 1 else ItemIndex := nSelectIndex;
//       被删的是最后一项时 nSelectIndex == Count → ItemIndex := Count - 1；
//       不是最后一项时 ItemIndex := nSelectIndex（指向**原本的下一项**，因为列表左移了）。
//       二者数值在"删最后一项"时**相同**，但在"删中间项且删除后 Count 变化"的边界上语义不同 —— 差异断言见测试。
//   D3. `rbAllBlockClick`（原 :213-219）用 `RadioButton.Tag` 反查模式：
//       DFM 里 rbAllBlock **没有 Tag 属性**（默认 0），rbSelfBolck Tag=1，rbConnClose Tag=2，
//       rbDisMsg Tag=3，rbDisMsgorSys Tag=4 —— 与 TFilterSayMsgMode 的枚举值一一对应。
//   D4. `Open`（原 :84-90）用 `case` **没有 else 分支**：若 g_FilterSayMsgMode 是越界值，
//       5 个单选按钮都不勾选（不是默认选中第一个）。
//   D5. `chkFilterSayMsgClick`（原 :197-211）会**直接改写全局 g_boFilterSayMsg**（不是只在 OK 时写），
//       同时无条件把 btnEdit/btnDel 置 False、把 btnOK 置 True（即使没做任何修改）。
// =====================================================================================

/// <summary>uFrmMessageFilter.pas 的界面值载体。</summary>
public class MessageFilterValues
{
    /// <summary>原 :83 `chkFilterSayMsg.Checked`。</summary>
    public bool FilterSayMsg;

    /// <summary>原 :84-90 `g_FilterSayMsgMode` 对应的单选选择。</summary>
    public TFilterSayMsgMode FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;

    /// <summary>原 :93 `edtWarnSayMsg.Text`。</summary>
    public string WarnSayMsg = "";

    /// <summary>原 :95 `chkFilterSayTriggerScript.Checked`。</summary>
    public bool FilterSayTriggerScript;

    /// <summary>原 :74-76 `lstFilterText.Items` 的全部条目。</summary>
    public List<string> FilterTexts = new List<string>();
}

/// <summary>uFrmMessageFilter.pas 的非 UI 逻辑（可单测）。</summary>
public static class MessageFilterLogic
{
    /// <summary>原 :72-79 `TFrmMessageFilter.Open` 的列表刷新部分（g_WordFilterList → 列表）。</summary>
    public static List<string> OpenList()
    {
        var list = new List<string>();
        FormGlobals.g_WordFilterList.Lock();                                 // 原 :72
        try
        {
            for (int i = 0; i < FormGlobals.g_WordFilterList.Count; i++)     // 原 :75-76
                list.Add(FormGlobals.g_WordFilterList[i]);
        }
        finally
        {
            FormGlobals.g_WordFilterList.UnLock();                           // 原 :78
        }
        return list;
    }

    /// <summary>原 :81-95 的其余初始化（按钮使能 + 单选 + 唤一次 chkFilterSayMsgClick + 警告文本 + 脚本开关）。</summary>
    public static MessageFilterValues Open()
    {
        var v = new MessageFilterValues();
        v.FilterTexts = OpenList();                                          // 原 :74-76

        // 原 :81-82：btnDel.Enabled := False; btnEdit.Enabled := False;
        v.FilterSayMsg = FormGlobals.g_boFilterSayMsg;                       // 原 :83
        v.FilterSayMsgMode = FormGlobals.g_FilterSayMsgMode;                 // 原 :84-90
        v.WarnSayMsg = FormGlobals.g_WarnSayMsg;                             // 原 :93
        v.FilterSayTriggerScript = FormGlobals.g_boFilterSayTriggerScript;   // 原 :95
        return v;
    }

    /// <summary>
    /// 原 :197-211 `chkFilterSayMsgClick` 的**控件使能决策**（纯逻辑）。
    /// 返回"被 enable/disable 的控件组"：`listEnabled` = 过滤词列表与 5 个单选的使能值，
    /// `btnAddEnabled` = 增加按钮使能，`editDelEnabled` = 修改/删除按钮使能（恒 False）。
    /// ★ 原文还会**写回 g_boFilterSayMsg**（副作用），此处由调用方显式完成。
    /// </summary>
    public static void CheckFilterSayMsgClick(bool @checked, out bool listEnabled, out bool btnAddEnabled,
                                              out bool editDelEnabled)
    {
        FormGlobals.g_boFilterSayMsg = @checked;      // 原 :199（副作用，原文如此）
        editDelEnabled = false;                       // 原 :200-201 btnEdit/btnDel.Enabled := False
        listEnabled = @checked;                       // 原 :203-208 lstFilterText/rb*/edtWarnSayMsg.Enabled := g_boFilterSayMsg
        btnAddEnabled = @checked;                     // 原 :210 btnAdd.Enabled := g_boFilterSayMsg
    }

    /// <summary>
    /// 原 :108-133 `btnOKClick`：列表 → `g_WordFilterList`（Clear 后逐条 Add，然后 SaveToFile），
    /// 再写 INI 的 4 个键。`IniFile.Free` 在原文**没有 try..finally**（原 :123-130），
    /// 托管侧同样放在最后一行。
    /// </summary>
    public static void ButtonOK(MessageFilterValues v, string iniFileName, string wordFilterFileName)
    {
        FormGlobals.g_WordFilterList.Lock();                                       // 原 :113
        try
        {
            FormGlobals.g_WordFilterList.Clear();                                   // 原 :115
            foreach (string s in v.FilterTexts)                                     // 原 :116-117
                FormGlobals.g_WordFilterList.Add(s);
        }
        finally
        {
            FormGlobals.g_WordFilterList.UnLock();                                  // 原 :119
        }
        FormGlobals.g_WordFilterList.SaveToFile(wordFilterFileName);                // 原 :121

        var ini = new TIniFileEx(iniFileName);                                      // 原 :123
        ini.WriteBool(RunGateConst.GateClass, "FilterSayMsg", v.FilterSayMsg ? (byte)1 : (byte)0);          // 原 :124
        ini.WriteInteger(RunGateConst.GateClass, "FilterSayMsgMode", (int)v.FilterSayMsgMode);              // 原 :125
        ini.WriteString(RunGateConst.GateClass, "WarnSayMsg", v.WarnSayMsg);                                // 原 :126
        ini.WriteBool(RunGateConst.GateClass, "FilterSayTriggerScript", v.FilterSayTriggerScript ? (byte)1 : (byte)0); // 原 :128
        ini.Dispose();                                                              // 原 :130 IniFile.Free
    }

    /// <summary>
    /// 原 :135-152 `btnEditClick` 的文本决策（含 D1 缺陷复刻）。
    /// <paramref name="itemIndex"/> = 当前选中项（-1 表示未选中）；
    /// <paramref name="sourceText"/> = 选中项的当前文本（原 :141 `Items[ItemIndex]`）；
    /// <paramref name="inputQuery"/> 为输入框接缝，签名 (caption, prompt, ref value) → bool。
    /// 返回：`false` 表示应 Exit（不修改列表）；`true` 时用 <paramref name="newText"/> 覆盖 itemIndex 项。
    /// 不管哪个分支，只要弹了错都会把文本写进 <paramref name="errorMessage"/>（便于测试断言）。
    /// </summary>
    public static bool EditText(int itemIndex, int itemCount, string sourceText,
                                MessageBoxSeam.InputQueryHandler inputQuery,
                                out string newText, out string errorMessage)
    {
        // ★ D1：Delphi 局部 `sInputText: string;` 初值为 ''，未进入 if 分支时它就是 ''，
        //    于是下面 `if sInputText = ''` 成立 → 弹 '请输入正确的文本！！！' 并 Exit。
        newText = "";                                                               // 原 :137（未初始化 → ''）
        errorMessage = "";

        if (itemIndex >= 0 && itemIndex < itemCount)                                // 原 :139
        {
            newText = sourceText ?? "";                                             // 原 :141
            if (!inputQuery("增加过滤文字", "请输入新的文字:", ref newText))        // 原 :142
                return false;                                                       // 原 :142 `if not ... then Exit`
        }

        if (newText == "")                                                          // 原 :145
        {
            errorMessage = "请输入正确的文本！！！";                                 // 原 :147
            return false;                                                           // 原 :148 Exit
        }

        // 原 :151 lstFilterText.Items[lstFilterText.ItemIndex] := sInputText;
        // ★ 未选中（itemIndex = -1）时原文会在此越界（ItemIndex = -1）——
        //    该分支在本实现里已被上面的 `newText == ""` 拦截，等价于原文"先弹窗再 Exit"。
        return true;
    }

    /// <summary>
    /// 原 :175-195 `btnDelClick` 的"删除后选中项"决策（纯函数，含 D2 边界）。
    /// 输入：删除前的选中索引与条数；输出：删除后应设的 ItemIndex 以及是否应禁用两个按钮。
    /// 原文用**删除后**的 `Items.Count` 做 `nSelectIndex >= Count` 比较，故这里显式用 `itemCountAfter`。
    /// </summary>
    public static void DeleteSelection(int nSelectIndex, int itemCountBefore, out int newItemIndex,
                                      out bool disableEditDel, out bool deleted)
    {
        deleted = nSelectIndex >= 0 && nSelectIndex < itemCountBefore;               // 原 :180-183
        int itemCountAfter = deleted ? itemCountBefore - 1 : itemCountBefore;

        if (nSelectIndex >= itemCountAfter)                                         // 原 :185
            newItemIndex = nSelectIndex - 1;                                        // 原 :186
        else
            newItemIndex = nSelectIndex;                                            // 原 :188

        disableEditDel = newItemIndex < 0;                                          // 原 :190-194
    }
}

/// <summary>原 :53-66 `function ShowFrmMessageFilter(MainForm: TForm): Boolean;`。</summary>
public static class MessageFilterUnit
{
    public static bool ShowFrmMessageFilter(Form mainForm)
    {
        using var form = new FrmMessageFilter();
        if (mainForm != null)                                                              // 原 :59-60
        {
            form.Left = mainForm.Left + (mainForm.Width - form.Width) / 2;
            form.Top = mainForm.Top + (mainForm.Height - form.Height) / 2;
        }
        form.Open();                                                                       // 原 :61
        return form.ShowDialog() == DialogResult.OK;                                       // 原 :62
    }
}

/// <summary>原 uFrmMessageFilter.pas:10-42 `TFrmMessageFilter`（DFM: uFrmMessageFilter.dfm）。</summary>
public class FrmMessageFilter : Form
{
    // DFM: FrmMessageFilter Left=488 Top=246 BorderIcons=[biSystemMenu,biMinimize] BorderStyle=bsDialog
    //      Caption='消息文字过滤设置' ClientHeight=249 ClientWidth=427 Font.Charset=ANSI_CHARSET
    //      Font.Height=-12 Font.Name='宋体' PixelsPerInch=96
    public Label Label1;              // DFM: Label1 Left=8 Top=8 Width=54 Height=12 Caption='过滤文本:'
    public ListBox lstFilterText;     // DFM: lstFilterText Left=8 Top=24 Width=153 Height=217 ItemHeight=12 TabOrder=0 OnClick/OnDblClick
    public Button btnAdd;             // DFM: btnAdd Left=168 Top=216 Width=59 Height=25 Caption='增加(&A)' TabOrder=1 OnClick=btnAddClick
    public Button btnDel;             // DFM: btnDel Left=232 Top=216 Width=59 Height=25 Caption='删除(&D)' TabOrder=2 OnClick=btnDelClick
    public Button btnOK;              // DFM: btnOK Left=360 Top=216 Width=59 Height=25 Caption='确定(&O)' TabOrder=3 OnClick=btnOKClick
    public Button btnEdit;            // DFM: btnEdit Left=296 Top=216 Width=59 Height=25 Caption='修改(&M)' TabOrder=4 OnClick=btnEditClick
    public GroupBox GroupBox2;        // DFM: GroupBox2 Left=168 Top=19 Width=251 Height=192 Caption='过滤选项' TabOrder=5
    public Label Label2;              // DFM: Label2 Left=18 Top=168 Width=54 Height=12 Caption='警告内容:'
    public CheckBox chkFilterSayMsg;  // DFM: chkFilterSayMsg Left=8 Top=17 Width=97 Height=17 Caption='开启文字过滤' ShowHint=True TabOrder=0 OnClick=chkFilterSayMsgClick
    public RadioButton rbAllBlock;    // DFM: rbAllBlock Left=16 Top=37 Width=145 Height=17 Caption='整句使用警告文本替换' Tag=(未设→0) TabOrder=1 OnClick=rbAllBlockClick
    public RadioButton rbSelfBolck;   // DFM: rbSelfBolck Left=16 Top=57 Width=145 Height=17 Caption='特征字用警告文本替换' Tag=1 TabOrder=2 OnClick=rbAllBlockClick
    public RadioButton rbConnClose;   // DFM: rbConnClose Left=16 Top=78 Width=145 Height=17 Caption='发现过滤文字掉线处理' Tag=2 TabOrder=3 OnClick=rbAllBlockClick
    public RadioButton rbDisMsg;      // DFM: rbDisMsg Left=16 Top=98 Width=145 Height=17 Caption='发现过滤文字丢包处理' Tag=3 TabOrder=4 OnClick=rbAllBlockClick
    public RadioButton rbDisMsgorSys; // DFM: rbDisMsgorSys Left=16 Top=118 Width=145 Height=17 Caption='发现过滤文字警告处理' Tag=4 TabOrder=5 OnClick=rbAllBlockClick
    public CheckBox chkFilterSayTriggerScript; // DFM: chkFilterSayTriggerScript Left=17 Top=141 Width=200 Height=17 Caption='触发脚本 [@RungateMsgFilter]' TabOrder=6 OnClick=chkFilterSayTriggerScriptClick
    public TextBox edtWarnSayMsg;     // DFM: edtWarnSayMsg Left=72 Top=164 Width=171 Height=20 TabOrder=7 OnChange=edtWarnSayMsgChange

    public FrmMessageFilter()
    {
        // DFM: FrmMessageFilter Caption='消息文字过滤设置' BorderStyle=bsDialog BorderIcons=[biSystemMenu,biMinimize]
        Text = "消息文字过滤设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = true;                                  // BorderIcons 含 biMinimize
        MaximizeBox = false;
        Location = new Point(488, 246);
        ClientSize = new Size(427, 249);
        Font = new Font("宋体", 9F);                          // Font.Height=-12 Font.Name='宋体'

        // DFM: Label1 Left=8 Top=8 Width=54 Height=12 Caption='过滤文本:'
        Label1 = new Label { Left = 8, Top = 8, Width = 54, Height = 12, Text = "过滤文本:" };
        // DFM: lstFilterText Left=8 Top=24 Width=153 Height=217 ItemHeight=12 TabOrder=0
        lstFilterText = new ListBox { Left = 8, Top = 24, Width = 153, Height = 217, TabIndex = 0 };
        // DFM: btnAdd Left=168 Top=216 Width=59 Height=25 Caption='增加(&A)' TabOrder=1
        btnAdd = new Button { Left = 168, Top = 216, Width = 59, Height = 25, Text = "增加(&A)", TabIndex = 1 };
        // DFM: btnDel Left=232 Top=216 Width=59 Height=25 Caption='删除(&D)' TabOrder=2
        btnDel = new Button { Left = 232, Top = 216, Width = 59, Height = 25, Text = "删除(&D)", TabIndex = 2 };
        // DFM: btnOK Left=360 Top=216 Width=59 Height=25 Caption='确定(&O)' TabOrder=3
        btnOK = new Button { Left = 360, Top = 216, Width = 59, Height = 25, Text = "确定(&O)", TabIndex = 3 };
        // DFM: btnEdit Left=296 Top=216 Width=59 Height=25 Caption='修改(&M)' TabOrder=4
        btnEdit = new Button { Left = 296, Top = 216, Width = 59, Height = 25, Text = "修改(&M)", TabIndex = 4 };

        // DFM: GroupBox2 Left=168 Top=19 Width=251 Height=192 Caption='过滤选项' TabOrder=5
        GroupBox2 = new GroupBox { Left = 168, Top = 19, Width = 251, Height = 192, Text = "过滤选项", TabIndex = 5 };
        // DFM: Label2 Left=18 Top=168 Width=54 Height=12 Caption='警告内容:'
        Label2 = new Label { Left = 18, Top = 168, Width = 54, Height = 12, Text = "警告内容:" };
        // DFM: chkFilterSayMsg Left=8 Top=17 Width=97 Height=17 Caption='开启文字过滤' ShowHint=True TabOrder=0
        chkFilterSayMsg = new CheckBox { Left = 8, Top = 17, Width = 97, Height = 17, Text = "开启文字过滤", TabIndex = 0 };
        // DFM: rbAllBlock Left=16 Top=37 Width=145 Height=17 Caption='整句使用警告文本替换'（Tag 未设 → 0）TabOrder=1
        rbAllBlock = new RadioButton { Left = 16, Top = 37, Width = 145, Height = 17, Text = "整句使用警告文本替换", TabIndex = 1, Tag = 0 };
        // DFM: rbSelfBolck Left=16 Top=57 Width=145 Height=17 Caption='特征字用警告文本替换' Tag=1 TabOrder=2
        rbSelfBolck = new RadioButton { Left = 16, Top = 57, Width = 145, Height = 17, Text = "特征字用警告文本替换", TabIndex = 2, Tag = 1 };
        // DFM: rbConnClose Left=16 Top=78 Width=145 Height=17 Caption='发现过滤文字掉线处理' Tag=2 TabOrder=3
        rbConnClose = new RadioButton { Left = 16, Top = 78, Width = 145, Height = 17, Text = "发现过滤文字掉线处理", TabIndex = 3, Tag = 2 };
        // DFM: rbDisMsg Left=16 Top=98 Width=145 Height=17 Caption='发现过滤文字丢包处理' Tag=3 TabOrder=4
        rbDisMsg = new RadioButton { Left = 16, Top = 98, Width = 145, Height = 17, Text = "发现过滤文字丢包处理", TabIndex = 4, Tag = 3 };
        // DFM: rbDisMsgorSys Left=16 Top=118 Width=145 Height=17 Caption='发现过滤文字警告处理' Tag=4 TabOrder=5
        rbDisMsgorSys = new RadioButton { Left = 16, Top = 118, Width = 145, Height = 17, Text = "发现过滤文字警告处理", TabIndex = 5, Tag = 4 };
        // DFM: chkFilterSayTriggerScript Left=17 Top=141 Width=200 Height=17 Caption='触发脚本 [@RungateMsgFilter]' TabOrder=6
        chkFilterSayTriggerScript = new CheckBox { Left = 17, Top = 141, Width = 200, Height = 17, Text = "触发脚本 [@RungateMsgFilter]", TabIndex = 6 };
        // DFM: edtWarnSayMsg Left=72 Top=164 Width=171 Height=20 TabOrder=7 OnChange=edtWarnSayMsgChange
        edtWarnSayMsg = new TextBox { Left = 72, Top = 164, Width = 171, Height = 20, TabIndex = 7 };

        GroupBox2.Controls.AddRange(new Control[] { Label2, chkFilterSayMsg, rbAllBlock, rbSelfBolck,
                                                    rbConnClose, rbDisMsg, rbDisMsgorSys,
                                                    chkFilterSayTriggerScript, edtWarnSayMsg });
        Controls.AddRange(new Control[] { Label1, lstFilterText, btnAdd, btnDel, btnOK, btnEdit, GroupBox2 });

        // DFM 的事件接线（OnClick/OnDblClick/OnChange）
        lstFilterText.Click += (s, e) => lstFilterText_Click(s, e);
        lstFilterText.DoubleClick += (s, e) => lstFilterTextDblClick(s, e);
        btnAdd.Click += (s, e) => btnAdd_Click(s, e);
        btnDel.Click += (s, e) => btnDel_Click(s, e);
        btnOK.Click += (s, e) => btnOK_Click(s, e);
        btnEdit.Click += (s, e) => btnEdit_Click(s, e);
        rbAllBlock.Click += (s, e) => rbAllBlock_Click(s, e);
        rbSelfBolck.Click += (s, e) => rbAllBlock_Click(s, e);
        rbConnClose.Click += (s, e) => rbAllBlock_Click(s, e);
        rbDisMsg.Click += (s, e) => rbAllBlock_Click(s, e);
        rbDisMsgorSys.Click += (s, e) => rbAllBlock_Click(s, e);
        chkFilterSayMsg.Click += (s, e) => chkFilterSayMsg_Click(s, e);
        chkFilterSayTriggerScript.Click += (s, e) => chkFilterSayTriggerScript_Click(s, e);
        edtWarnSayMsg.TextChanged += (s, e) => edtWarnSayMsg_Change(s, e);
    }

    /// <summary>原 uFrmMessageFilter.pas:68-96 `TFrmMessageFilter.Open`。</summary>
    public void Open()
    {
        var v = MessageFilterLogic.Open();

        lstFilterText.Items.Clear();                                      // 原 :74
        foreach (string s in v.FilterTexts) lstFilterText.Items.Add(s);   // 原 :75-76

        btnDel.Enabled = false;                                           // 原 :81
        btnEdit.Enabled = false;                                          // 原 :82
        chkFilterSayMsg.Checked = v.FilterSayMsg;                         // 原 :83
        switch (v.FilterSayMsgMode)                                       // 原 :84-90（无 else 分支 → D4）
        {
            case TFilterSayMsgMode.fsmmAllBlock: rbAllBlock.Checked = true; break;
            case TFilterSayMsgMode.fsmmSelfBolck: rbSelfBolck.Checked = true; break;
            case TFilterSayMsgMode.fsmmClose: rbConnClose.Checked = true; break;
            case TFilterSayMsgMode.fsmmDisMsg: rbDisMsg.Checked = true; break;
            case TFilterSayMsgMode.fsmmDisMsgorSys: rbDisMsgorSys.Checked = true; break;
        }

        chkFilterSayMsg_Click(chkFilterSayMsg, EventArgs.Empty);          // 原 :92
        edtWarnSayMsg.Text = v.WarnSayMsg;                                // 原 :93
        chkFilterSayTriggerScript.Checked = v.FilterSayTriggerScript;     // 原 :95
    }

    /// <summary>原 uFrmMessageFilter.pas:98-106 `lstFilterTextClick`。</summary>
    public void lstFilterText_Click(object sender, EventArgs e)
    {
        if (lstFilterText.SelectedIndex >= 0 && lstFilterText.SelectedIndex < lstFilterText.Items.Count)   // 原 :100-101
        {
            btnDel.Enabled = true;                                        // 原 :103
            btnEdit.Enabled = true;                                       // 原 :104
        }
    }

    /// <summary>原 uFrmMessageFilter.pas:108-133 `btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        var v = new MessageFilterValues
        {
            FilterTexts = new List<string>(),
            FilterSayMsg = FormGlobals.g_boFilterSayMsg,          // 原文 OK 时**不重读控件**，用的是 chkFilterSayMsgClick 已写回的值
            FilterSayMsgMode = FormGlobals.g_FilterSayMsgMode,
            WarnSayMsg = FormGlobals.g_WarnSayMsg,
            FilterSayTriggerScript = FormGlobals.g_boFilterSayTriggerScript
        };
        for (int i = 0; i < lstFilterText.Items.Count; i++)                   // 原 :116-117
            v.FilterTexts.Add(lstFilterText.Items[i].ToString());

        MessageFilterLogic.ButtonOK(v, FormGlobals.g_sIniFileName, FormGlobals.g_sWordFilterFileName);
        DialogResult = DialogResult.OK;                                       // 原 :132 ModalResult := mrOk
    }

    /// <summary>原 uFrmMessageFilter.pas:135-152 `btnEditClick`。</summary>
    public void btnEdit_Click(object sender, EventArgs e)
    {
        int idx = lstFilterText.SelectedIndex;                                // 原 :139 ItemIndex
        string source = idx >= 0 && idx < lstFilterText.Items.Count
            ? lstFilterText.Items[idx].ToString() : "";                       // 原 :141

        bool ok = MessageFilterLogic.EditText(idx, lstFilterText.Items.Count, source,
            MessageBoxSeam.InputQueryWithValue, out string newText, out string errorMessage);
        if (errorMessage != "")
            MessageBoxSeam.ShowError(errorMessage, "错误信息");                // 原 :147

        if (!ok) return;                                                       // 原 :142 / :148 Exit
        lstFilterText.Items[idx] = newText;                                    // 原 :151
    }

    /// <summary>原 uFrmMessageFilter.pas:154-158 `lstFilterTextDblClick` → 直接转发给 btnEditClick。</summary>
    public void lstFilterTextDblClick(object sender, EventArgs e) => btnEdit_Click(sender, e);   // 原 :157

    /// <summary>原 uFrmMessageFilter.pas:160-173 `btnAddClick`。</summary>
    public void btnAdd_Click(object sender, EventArgs e)
    {
        string sInputText = "";                                                          // 原 :162
        if (!MessageBoxSeam.InputQueryWithValue("增加过滤文字", "请输入新的文字:", ref sInputText))   // 原 :165
            return;                                                                      // 原 :165 Exit

        if (sInputText == "")                                                            // 原 :167
        {
            MessageBoxSeam.ShowError("请输入正确的文本！！！", "错误信息");                // 原 :169
            return;                                                                      // 原 :170 Exit
        }
        lstFilterText.Items.Add(sInputText);                                             // 原 :172
    }

    /// <summary>原 uFrmMessageFilter.pas:175-195 `btnDelClick`。</summary>
    public void btnDel_Click(object sender, EventArgs e)
    {
        int nSelectIndex = lstFilterText.SelectedIndex;                                   // 原 :179
        int countBefore = lstFilterText.Items.Count;
        if (nSelectIndex >= 0 && nSelectIndex < countBefore)                              // 原 :180
            lstFilterText.Items.RemoveAt(nSelectIndex);                                   // 原 :182 Delete(nSelectIndex)

        // 原 :185-188 的判定用的是**删除后**的 Items.Count
        MessageFilterLogic.DeleteSelection(nSelectIndex, countBefore,
            out int newItemIndex, out bool disableEditDel, out _);

        lstFilterText.SelectedIndex = newItemIndex;                                       // 原 :186/188

        if (disableEditDel)                                                               // 原 :190-194
        {
            btnDel.Enabled = false;                                                       // 原 :192
            btnEdit.Enabled = false;                                                      // 原 :193
        }
    }

    /// <summary>原 uFrmMessageFilter.pas:197-211 `chkFilterSayMsgClick`（含写回全局的副作用）。</summary>
    public void chkFilterSayMsg_Click(object sender, EventArgs e)
    {
        // 原 :199 g_boFilterSayMsg := chkFilterSayMsg.Checked;（由 CheckFilterSayMsgClick 内完成）
        MessageFilterLogic.CheckFilterSayMsgClick(chkFilterSayMsg.Checked,
            out bool listEnabled, out bool btnAddEnabled, out bool editDelEnabled);

        btnEdit.Enabled = editDelEnabled;          // 原 :200
        btnDel.Enabled = editDelEnabled;           // 原 :201
        btnOK.Enabled = true;                      // 原 :202

        lstFilterText.Enabled = listEnabled;       // 原 :203
        rbAllBlock.Enabled = listEnabled;          // 原 :204
        rbSelfBolck.Enabled = listEnabled;         // 原 :205
        rbConnClose.Enabled = listEnabled;         // 原 :206
        rbDisMsg.Enabled = listEnabled;            // 原 :207
        rbDisMsgorSys.Enabled = listEnabled;       // 原 :208
        edtWarnSayMsg.Enabled = listEnabled;       // 原 :209
        btnAdd.Enabled = btnAddEnabled;            // 原 :210
    }

    /// <summary>原 uFrmMessageFilter.pas:213-219 `rbAllBlockClick`（5 个单选共用，靠 Tag 反查模式）。</summary>
    public void rbAllBlock_Click(object sender, EventArgs e)
    {
        if (sender is RadioButton rb)                                    // 原 :217 TRadioButton(Sender)
            FormGlobals.g_FilterSayMsgMode = (TFilterSayMsgMode)rb.Tag; // 原 :218
    }

    /// <summary>原 uFrmMessageFilter.pas:221-224 `edtWarnSayMsgChange`。</summary>
    public void edtWarnSayMsg_Change(object sender, EventArgs e)
        => FormGlobals.g_WarnSayMsg = edtWarnSayMsg.Text;                // 原 :223

    /// <summary>原 uFrmMessageFilter.pas:226-230 `chkFilterSayTriggerScriptClick`。</summary>
    public void chkFilterSayTriggerScript_Click(object sender, EventArgs e)
        => FormGlobals.g_boFilterSayTriggerScript = chkFilterSayTriggerScript.Checked;   // 原 :229
}
