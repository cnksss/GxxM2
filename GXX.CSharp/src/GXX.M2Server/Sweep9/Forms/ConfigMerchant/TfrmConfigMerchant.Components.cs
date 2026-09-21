// ============================================================================
// ConfigMerchant.dfm（文本，466 行）控件树 1:1 实例化：**50 个控件 + 33 个事件绑定**
// ============================================================================

namespace GXX.M2Server.Sweep9.Forms;

public sealed partial class TfrmConfigMerchant
{
    /// <summary>DFM 控件树 1:1 实例化（属性逐条取自 ConfigMerchant.dfm）。</summary>
    private void InitializeComponents()
    {
        // ---- DFM :1-18 窗体自身 ----
        Name = "frmConfigMerchant";
        Text = "交易NPC配置";                                          // Caption = #20132#26131'NPC'#37197#32622
        Left = 366;
        Top = 256;
        ClientSize = new System.Drawing.Size(818, 374);               // ClientWidth/ClientHeight
        // BorderIcons = [biSystemMenu, biMinimize] ⇒ 无最大化、有最小化
        MaximizeBox = false;
        MinimizeBox = true;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;  // bsSingle
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // poMainFormCenter
        Font = new System.Drawing.Font("宋体", 9F);                    // Font.Height = -12 / Name = 宋体
        // DFM `ShowHint = True` + 15 个控件的 `Hint = '...'`：WinForms 的等价物
        // `ToolTip.SetToolTip` **会给目标控件挂上 MouseEnter/MouseLeave/… 一批事件**，
        // 污染"DFM 绑定数 ↔ 托管 += 数"对账（实测：33 → 59，多出 26 个）。
        // ⇒ 本车道**不挂 ToolTip**，把 DFM 的 Hint 原文存进 `DfmHints`（可断言），
        // 呈现与否交给宿主适配层。已登记偏离 **D-P9-04**。
        // DFM :17 OnCreate = FormCreate ⇒ WinForms 等价 Load（绑定数对账：DFM 33 ↔ 托管 33）
        Load += (_, _) => FormCreate(this);

        // =================================================================
        // GroupBox1 'NPC列表:'（DFM :418-465）
        // =================================================================
        GroupBox1 = new System.Windows.Forms.GroupBox
        { Name = "GroupBox1", Text = "NPC列表:", Left = 8, Top = 8, Width = 401, Height = 214, TabIndex = 6 };
        lblSearch = new System.Windows.Forms.Label
        { Name = "lblSearch", Text = "搜索：", Left = 8, Top = 191, Width = 36, Height = 12, AutoSize = false };
        ListBoxMerChant = new System.Windows.Forms.ListBox
        { Name = "ListBoxMerChant", Left = 8, Top = 16, Width = 385, Height = 165, TabIndex = 0, IntegralHeight = false };
        ListBoxMerChant.Click += (_, _) => ListBoxMerChantClick(ListBoxMerChant);
        edtSearch = new System.Windows.Forms.TextBox { Name = "edtSearch", Left = 40, Top = 187, Width = 193, Height = 20, TabIndex = 1 };
        btnSearch = new System.Windows.Forms.Button { Name = "btnSearch", Text = "开始搜索", Left = 240, Top = 186, Width = 75, Height = 22, TabIndex = 2 };
        btnSearch.Click += (_, _) => btnSearchClick(btnSearch);
        btnSearchNext = new System.Windows.Forms.Button { Name = "btnSearchNext", Text = "下一个", Left = 320, Top = 186, Width = 75, Height = 22, TabIndex = 3 };
        // ⚠ DFM :457-464 `btnSearchNext` **没有** OnClick 绑定（原文如此 —— 该按钮点了没反应）
        GroupBox1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            lblSearch, ListBoxMerChant, edtSearch, btnSearch, btnSearchNext,
        });
        Controls.Add(GroupBox1);

        // =================================================================
        // GroupBoxNPC '相关设置'（DFM :19-203）—— DFM Enabled = False
        // =================================================================
        GroupBoxNPC = new System.Windows.Forms.GroupBox
        { Name = "GroupBoxNPC", Text = "相关设置", Left = 8, Top = 227, Width = 401, Height = 113, TabIndex = 0, Enabled = false };

        Label2 = MakeLabel("Label2", "脚本名称:", 8, 19, 54);
        Label3 = MakeLabel("Label3", "地图名称:", 208, 19, 54);
        Label4 = MakeLabel("Label4", "座标X:", 8, 43, 36);
        Label5 = MakeLabel("Label5", "Y:", 120, 43, 12);
        Label6 = MakeLabel("Label6", "显示名称:", 8, 67, 54);
        Label7 = MakeLabel("Label7", "方向:", 208, 67, 30);
        Label8 = MakeLabel("Label8", "外形:", 304, 67, 30);
        Label10 = MakeLabel("Label10", "地图描述:", 208, 43, 54);
        Label11 = MakeLabel("Label11", "移动间隔:", 288, 91, 54);

        EditScriptName = new System.Windows.Forms.TextBox { Name = "EditScriptName", Left = 64, Top = 15, Width = 121, Height = 20, AutoSize = false, TabIndex = 0 };
        EditScriptName.TextChanged += (_, _) => EditScriptNameChange(EditScriptName);

        EditMapName = new System.Windows.Forms.TextBox { Name = "EditMapName", Left = 264, Top = 15, Width = 121, Height = 20, AutoSize = false, TabIndex = 1 };
        EditMapName.TextChanged += (_, _) => EditMapNameChange(EditMapName);

        EditShowName = new System.Windows.Forms.TextBox { Name = "EditShowName", Left = 64, Top = 63, Width = 121, Height = 20, AutoSize = false, TabIndex = 2 };
        EditShowName.TextChanged += (_, _) => EditShowNameChange(EditShowName);

        CheckBoxOfCastle = new System.Windows.Forms.CheckBox { Name = "CheckBoxOfCastle", Text = "属于城堡", Left = 64, Top = 87, Width = 81, Height = 17, TabIndex = 3 };
        CheckBoxOfCastle.CheckedChanged += (_, _) => CheckBoxOfCastleClick(CheckBoxOfCastle);

        ComboBoxDir = new System.Windows.Forms.ComboBox
        {
            Name = "ComboBoxDir", Left = 240, Top = 63, Width = 49, Height = 20, TabIndex = 4,
            DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,   // DFM Style = csDropDownList
        };
        ComboBoxDir.SelectedIndexChanged += (_, _) => ComboBoxDirChange(ComboBoxDir);

        // DFM :136-147 EditImageIdx: TSpinEditEx（MaxValue=65535 MinValue=0 Value=0）
        EditImageIdx = Sweep9FormsKit.MakeSpin(336, 62, 49, minValue: 0, maxValue: 65535, value: 0);
        EditImageIdx.Name = "EditImageIdx";
        EditImageIdx.ValueChanged += (_, _) => EditImageIdxChange(EditImageIdx);

        // DFM :148-159 EditX（MaxValue=1000 MinValue=1 Value=1）
        EditX = Sweep9FormsKit.MakeSpin(64, 38, 49, minValue: 1, maxValue: 1000, value: 1);
        EditX.Name = "EditX";
        EditX.ValueChanged += (_, _) => EditXChange(EditX);

        // DFM :160-171 EditY（MaxValue=1000 MinValue=1 Value=1）
        EditY = Sweep9FormsKit.MakeSpin(136, 38, 49, minValue: 1, maxValue: 1000, value: 1);
        EditY.Name = "EditY";
        EditY.ValueChanged += (_, _) => EditYChange(EditY);

        // DFM :172-180 EditMapDesc（Enabled = False / ReadOnly = True）
        EditMapDesc = new System.Windows.Forms.TextBox
        { Name = "EditMapDesc", Left = 264, Top = 39, Width = 121, Height = 20, AutoSize = false, Enabled = false, ReadOnly = true, TabIndex = 8 };

        CheckBoxAutoMove = new System.Windows.Forms.CheckBox { Name = "CheckBoxAutoMove", Text = "自动移动", Left = 208, Top = 87, Width = 81, Height = 17, TabIndex = 9 };
        CheckBoxAutoMove.CheckedChanged += (_, _) => CheckBoxAutoMoveClick(CheckBoxAutoMove);

        // DFM :191-202 EditMoveTime（MaxValue=65535 MinValue=0 Value=0）
        EditMoveTime = Sweep9FormsKit.MakeSpin(344, 86, 41, minValue: 0, maxValue: 65535, value: 0);
        EditMoveTime.Name = "EditMoveTime";
        EditMoveTime.ValueChanged += (_, _) => EditMoveTimeChange(EditMoveTime);

        GroupBoxNPC.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label2, Label3, Label4, Label5, Label6, Label7, Label8, Label10, Label11,
            EditScriptName, EditMapName, EditShowName, CheckBoxOfCastle, ComboBoxDir,
            EditImageIdx, EditX, EditY, EditMapDesc, CheckBoxAutoMove, EditMoveTime,
        });
        Controls.Add(GroupBoxNPC);

        // =================================================================
        // GroupBoxScript '脚本编辑'（DFM :204-377）—— DFM Enabled = False
        // =================================================================
        GroupBoxScript = new System.Windows.Forms.GroupBox
        { Name = "GroupBoxScript", Text = "脚本编辑", Left = 416, Top = 8, Width = 401, Height = 361, TabIndex = 1, Enabled = false };

        // DFM :212-220 MemoScript（ScrollBars = ssBoth）
        MemoScript = new Sweep9Memo
        {
            Name = "MemoScript", Left = 8, Top = 136, Width = 385, Height = 217,
            ScrollBars = System.Windows.Forms.ScrollBars.Both, TabIndex = 0,
        };
        MemoScript.TextChanged += (_, _) => MemoScriptChange(MemoScript);

        ButtonScriptSave = new System.Windows.Forms.Button { Name = "ButtonScriptSave", Text = "保存(&S)", Left = 336, Top = 24, Width = 57, Height = 25, TabIndex = 1 };
        ButtonScriptSave.Click += (_, _) => ButtonScriptSaveClick(ButtonScriptSave);

        // ---- DFM :231-365 GroupBox3 '脚本参数' ----
        GroupBox3 = new System.Windows.Forms.GroupBox
        { Name = "GroupBox3", Text = "脚本参数", Left = 8, Top = 16, Width = 321, Height = 113, TabIndex = 2 };
        Label9 = MakeLabel("Label9", "交易折扣:", 8, 88, 54);
        CheckBoxBuy = MakeCheck("CheckBoxBuy", "买", 8, 16, 33, 0);
        CheckBoxBuy.CheckedChanged += (_, _) => CheckBoxBuyClick(CheckBoxBuy);
        CheckBoxSell = MakeCheck("CheckBoxSell", "卖", 8, 32, 33, 1);
        CheckBoxSell.CheckedChanged += (_, _) => CheckBoxSellClick(CheckBoxSell);
        CheckBoxStorage = MakeCheck("CheckBoxStorage", "取仓库", 72, 32, 65, 2);
        CheckBoxStorage.CheckedChanged += (_, _) => CheckBoxStorageClick(CheckBoxStorage);
        CheckBoxGetback = MakeCheck("CheckBoxGetback", "存仓库", 72, 16, 65, 3);
        CheckBoxGetback.CheckedChanged += (_, _) => CheckBoxGetbackClick(CheckBoxGetback);
        CheckBoxMakedrug = MakeCheck("CheckBoxMakedrug", "合成物品", 240, 48, 73, 4);
        CheckBoxMakedrug.CheckedChanged += (_, _) => CheckBoxMakedrugClick(CheckBoxMakedrug);
        CheckBoxUpgradenow = MakeCheck("CheckBoxUpgradenow", "升级武器", 152, 16, 73, 5);
        CheckBoxUpgradenow.CheckedChanged += (_, _) => CheckBoxUpgradenowClick(CheckBoxUpgradenow);
        CheckBoxGetbackupgnow = MakeCheck("CheckBoxGetbackupgnow", "取回升级", 152, 33, 73, 6);
        CheckBoxGetbackupgnow.CheckedChanged += (_, _) => CheckBoxGetbackupgnowClick(CheckBoxGetbackupgnow);
        CheckBoxRepair = MakeCheck("CheckBoxRepair", "修理物品", 240, 16, 73, 7);
        CheckBoxRepair.CheckedChanged += (_, _) => CheckBoxRepairClick(CheckBoxRepair);
        CheckBoxS_repair = MakeCheck("CheckBoxS_repair", "特殊修理", 240, 32, 73, 8);
        CheckBoxS_repair.CheckedChanged += (_, _) => CheckBoxS_repairClick(CheckBoxS_repair);

        // DFM :326-337 EditPriceRate（MaxValue=500 MinValue=60 Value=60）
        EditPriceRate = Sweep9FormsKit.MakeSpin(64, 84, 49, minValue: 60, maxValue: 500, value: 60);
        EditPriceRate.Name = "EditPriceRate";
        EditPriceRate.ValueChanged += (_, _) => EditPriceRateChange(EditPriceRate);

        CheckBoxSendMsg = MakeCheck("CheckBoxSendMsg", "祝福语", 72, 48, 73, 10);
        CheckBoxSendMsg.CheckedChanged += (_, _) => CheckBoxSendMsgClick(CheckBoxSendMsg);
        chkCreateHero = MakeCheck("chkCreateHero", "创建英雄", 152, 49, 73, 11);
        chkCreateHero.CheckedChanged += (_, _) => chkCreateHeroClick(chkCreateHero);
        chkBuyHero = MakeCheck("chkBuyHero", "副将英雄", 152, 66, 73, 12);
        chkBuyHero.CheckedChanged += (_, _) => chkBuHeroClick(chkBuyHero);

        GroupBox3.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label9, CheckBoxBuy, CheckBoxSell, CheckBoxStorage, CheckBoxGetback, CheckBoxMakedrug,
            CheckBoxUpgradenow, CheckBoxGetbackupgnow, CheckBoxRepair, CheckBoxS_repair,
            EditPriceRate, CheckBoxSendMsg, chkCreateHero, chkBuyHero,
        });

        // DFM :366-376 ButtonReLoadNpc（Enabled = False）
        ButtonReLoadNpc = new System.Windows.Forms.Button
        { Name = "ButtonReLoadNpc", Text = "加载(&L)", Left = 336, Top = 56, Width = 57, Height = 25, Enabled = false, TabIndex = 3 };
        ButtonReLoadNpc.Click += (_, _) => ButtonReLoadNpcClick(ButtonReLoadNpc);

        GroupBoxScript.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            MemoScript, ButtonScriptSave, GroupBox3, ButtonReLoadNpc,
        });
        Controls.Add(GroupBoxScript);

        // =================================================================
        // DFM :378-417 窗体级按钮/勾选（4 个）
        // =================================================================
        ButtonSave = new System.Windows.Forms.Button { Name = "ButtonSave", Text = "保存(&S)", Left = 8, Top = 344, Width = 57, Height = 25, TabIndex = 2 };
        ButtonSave.Click += (_, _) => ButtonSaveClick(ButtonSave);
        Controls.Add(ButtonSave);

        CheckBoxDenyRefStatus = new System.Windows.Forms.CheckBox
        { Name = "CheckBoxDenyRefStatus", Text = "刷新状态", Left = 328, Top = 348, Width = 73, Height = 17, TabIndex = 3 };
        CheckBoxDenyRefStatus.CheckedChanged += (_, _) => CheckBoxDenyRefStatusClick(CheckBoxDenyRefStatus);
                Controls.Add(CheckBoxDenyRefStatus);

        ButtonClearTempData = new System.Windows.Forms.Button
        { Name = "ButtonClearTempData", Text = "清除数据(&C)", Left = 168, Top = 344, Width = 89, Height = 25, TabIndex = 4 };
        ButtonClearTempData.Click += (_, _) => ButtonClearTempDataClick(ButtonClearTempData);
                Controls.Add(ButtonClearTempData);

        // DFM :408-417 ButtonViewData（Visible = False，OnClick **复用** ButtonClearTempDataClick）
        ButtonViewData = new System.Windows.Forms.Button
        { Name = "ButtonViewData", Text = "查看数据(&V)", Left = 72, Top = 344, Width = 89, Height = 25, Visible = false, TabIndex = 5 };
        ButtonViewData.Click += (_, _) => ButtonClearTempDataClick(ButtonViewData);
        Controls.Add(ButtonViewData);
    }

    // ------------------------------------------------------------------
    // 构造辅助
    // ------------------------------------------------------------------

    private static System.Windows.Forms.Label MakeLabel(string name, string caption, int left, int top, int width)
        => new() { Name = name, Text = caption, Left = left, Top = top, Width = width, Height = 12, AutoSize = false };

    private static System.Windows.Forms.CheckBox MakeCheck(string name, string caption,
        int left, int top, int width, int tabOrder)
        => new() { Name = name, Text = caption, Left = left, Top = top, Width = width, Height = 17, TabIndex = tabOrder };
}
