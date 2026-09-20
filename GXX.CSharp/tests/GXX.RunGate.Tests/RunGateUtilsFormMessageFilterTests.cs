using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmMessageFilter.pas（233 行）的纯逻辑与窗体测试。
/// 重点（原文缺陷，全部照抄并固定行为）：
///   D1 `btnEditClick` 未选中项时 `sInputText` 保持 '' → 走"请输入正确的文本！！！"分支（**不弹输入框**）；
///   D2 `btnDelClick` 删除后按 `nSelectIndex >= Count` 决定 ItemIndex；
///   D3 5 个单选的 Tag = 0..4 与 TFilterSayMsgMode 一一对应；
///   D4 `Open` 的 case 无 else（越界值 → 5 个单选都不勾）；
///   D5 `chkFilterSayMsgClick` 直接改写全局并把 btnOK 置 True。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormMessageFilterTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormMessageFilterTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p2rg_form_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        FormGlobals.ResetForTest();
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string IniPath => Path.Combine(_dir, "Config.ini");
    private string WordFile => Path.Combine(_dir, "WordFilter.txt");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    // ---------------- OpenList / Open（原 :68-96）----------------

    [Fact]
    public void OpenList_从g_WordFilterList按序取出()
    {
        FormGlobals.g_WordFilterList.Add("bad1");
        FormGlobals.g_WordFilterList.Add("bad2");
        var list = MessageFilterLogic.OpenList();
        Assert.Equal(new[] { "bad1", "bad2" }, list);
    }

    [Fact]
    public void Open_五个单选按模式勾选_Tag与枚举值一一对应()
    {
        FormGlobals.g_FilterSayMsgMode = TFilterSayMsgMode.fsmmClose;
        using var f = new FrmMessageFilter();
        f.Open();
        Assert.True(f.rbConnClose.Checked);       // fsmmClose = 2
        Assert.False(f.rbAllBlock.Checked);
        Assert.False(f.rbSelfBolck.Checked);
        Assert.False(f.rbDisMsg.Checked);
        Assert.False(f.rbDisMsgorSys.Checked);

        // Tag 与枚举整数一一对应（原 :218 `TFilterSayMsgMode(RadioButton.Tag)`）
        Assert.Equal((int)TFilterSayMsgMode.fsmmAllBlock, (int)f.rbAllBlock.Tag);
        Assert.Equal((int)TFilterSayMsgMode.fsmmSelfBolck, (int)f.rbSelfBolck.Tag);
        Assert.Equal((int)TFilterSayMsgMode.fsmmClose, (int)f.rbConnClose.Tag);
        Assert.Equal((int)TFilterSayMsgMode.fsmmDisMsg, (int)f.rbDisMsg.Tag);
        Assert.Equal((int)TFilterSayMsgMode.fsmmDisMsgorSys, (int)f.rbDisMsgorSys.Tag);
    }

    [Fact]
    public void Open_越界模式时五个单选全不勾_case无else()
    {
        FormGlobals.g_FilterSayMsgMode = (TFilterSayMsgMode)99;
        using var f = new FrmMessageFilter();
        f.Open();
        Assert.False(f.rbAllBlock.Checked);
        Assert.False(f.rbSelfBolck.Checked);
        Assert.False(f.rbConnClose.Checked);
        Assert.False(f.rbDisMsg.Checked);
        Assert.False(f.rbDisMsgorSys.Checked);
    }

    [Fact]
    public void Open_回填列表_禁用删改按钮_并同步开关与警告文本()
    {
        FormGlobals.g_WordFilterList.Add("x");
        FormGlobals.g_WordFilterList.Add("y");
        FormGlobals.g_boFilterSayMsg = true;
        FormGlobals.g_WarnSayMsg = "警告A";
        FormGlobals.g_boFilterSayTriggerScript = true;

        using var f = new FrmMessageFilter();
        f.Open();

        Assert.Equal(2, f.lstFilterText.Items.Count);
        Assert.Equal("x", f.lstFilterText.Items[0]);
        Assert.Equal("y", f.lstFilterText.Items[1]);
        Assert.True(f.chkFilterSayMsg.Checked);
        Assert.Equal("警告A", f.edtWarnSayMsg.Text);
        Assert.True(f.chkFilterSayTriggerScript.Checked);
    }

    [Fact]
    public void Open_调用chkFilterSayMsgClick后_删改按钮被强制False确定被强制True()
    {
        using var f = new FrmMessageFilter();
        f.btnDel.Enabled = true;
        f.btnEdit.Enabled = true;
        f.btnOK.Enabled = false;

        f.Open();

        Assert.False(f.btnDel.Enabled);      // 原 :200-201
        Assert.False(f.btnEdit.Enabled);
        Assert.True(f.btnOK.Enabled);        // 原 :202
    }

    // ---------------- chkFilterSayMsgClick（原 :197-211）----------------

    [Fact]
    public void CheckFilterSayMsgClick_写回全局并返回控件使能值()
    {
        MessageFilterLogic.CheckFilterSayMsgClick(true, out bool listEnabled, out bool addEnabled, out bool editDel);
        Assert.True(FormGlobals.g_boFilterSayMsg);      // 副作用（原 :199）
        Assert.True(listEnabled);
        Assert.True(addEnabled);
        Assert.False(editDel);                          // 原 :200-201 恒 False

        MessageFilterLogic.CheckFilterSayMsgClick(false, out listEnabled, out addEnabled, out editDel);
        Assert.False(FormGlobals.g_boFilterSayMsg);
        Assert.False(listEnabled);
        Assert.False(addEnabled);
        Assert.False(editDel);
    }

    [Fact]
    public void 窗体_chkFilterSayMsgClick把六组控件一起使能或禁用()
    {
        using var f = new FrmMessageFilter();

        f.chkFilterSayMsg.Checked = true;
        f.chkFilterSayMsg_Click(f, EventArgs.Empty);
        Assert.True(f.lstFilterText.Enabled);
        Assert.True(f.rbAllBlock.Enabled);
        Assert.True(f.rbSelfBolck.Enabled);
        Assert.True(f.rbConnClose.Enabled);
        Assert.True(f.rbDisMsg.Enabled);
        Assert.True(f.rbDisMsgorSys.Enabled);
        Assert.True(f.edtWarnSayMsg.Enabled);
        Assert.True(f.btnAdd.Enabled);
        Assert.True(f.btnOK.Enabled);

        f.chkFilterSayMsg.Checked = false;
        f.chkFilterSayMsg_Click(f, EventArgs.Empty);
        Assert.False(f.lstFilterText.Enabled);
        Assert.False(f.btnAdd.Enabled);
        Assert.False(f.edtWarnSayMsg.Enabled);
        Assert.True(f.btnOK.Enabled);        // 原 :202 无条件 True
    }

    // ---------------- EditText（原 :135-152，含 D1）----------------

    [Fact]
    public void EditText_未选中项_不弹输入框直接报错_D1()
    {
        bool inputQueryCalled = false;
        var handler = new MessageBoxSeam.InputQueryHandler((string c, string p, ref string v) => { inputQueryCalled = true; return true; });

        bool ok = MessageFilterLogic.EditText(-1, 3, "", handler, out string newText, out string errorMessage);

        Assert.False(ok);                                            // 原 :148 Exit
        Assert.False(inputQueryCalled);                              // ★ 关键：不弹输入框
        Assert.Equal("", newText);
        Assert.Equal("请输入正确的文本！！！", errorMessage);          // 原 :147
    }

    [Fact]
    public void EditText_选中且用户取消_直接返回不报错()
    {
        var handler = new MessageBoxSeam.InputQueryHandler((string c, string p, ref string v) => false);

        bool ok = MessageFilterLogic.EditText(1, 3, "old", handler, out string newText, out string errorMessage);

        Assert.False(ok);                  // 原 :142 Exit
        Assert.Equal("请输入正确的文本！！！", errorMessage == "" ? "请输入正确的文本！！！" : errorMessage);  // no-op 保护
        Assert.Equal("", errorMessage);     // 取消不弹错
    }

    [Fact]
    public void EditText_选中且输入空串_报错()
    {
        var handler = new MessageBoxSeam.InputQueryHandler((string c, string p, ref string v) => { v = ""; return true; });

        bool ok = MessageFilterLogic.EditText(1, 3, "old", handler, out string newText, out string errorMessage);

        Assert.False(ok);
        Assert.Equal("", newText);
        Assert.Equal("请输入正确的文本！！！", errorMessage);     // 原 :147
    }

    [Fact]
    public void EditText_选中且输入新文本_成功()
    {
        var handler = new MessageBoxSeam.InputQueryHandler((string c, string p, ref string v) =>
        {
            Assert.Equal("增加过滤文字", c);                 // 原 :142
            Assert.Equal("请输入新的文字:", p);
            Assert.Equal("old", v);                          // 传入的是当前项文本（原 :141）
            v = "new";
            return true;
        });

        bool ok = MessageFilterLogic.EditText(1, 3, "old", handler, out string newText, out string errorMessage);

        Assert.True(ok);
        Assert.Equal("new", newText);
        Assert.Equal("", errorMessage);
    }

    [Fact]
    public void EditText_项下标等于Count也视为未选中()
    {
        bool called = false;
        var handler = new MessageBoxSeam.InputQueryHandler((string c, string p, ref string v) => { called = true; return true; });
        bool ok = MessageFilterLogic.EditText(3, 3, "x", handler, out _, out string err);
        Assert.False(ok);
        Assert.False(called);
        Assert.Equal("请输入正确的文本！！！", err);
    }

    // ---------------- DeleteSelection（原 :175-195，含 D2）----------------

    [Fact]
    public void DeleteSelection_删最后一项_选中前移一位()
    {
        // Count=3，删 index 2（最后一项）→ 删后 Count=2，nSelectIndex(2) >= 2 → newIndex = 1
        MessageFilterLogic.DeleteSelection(2, 3, out int newIndex, out bool disable, out bool deleted);
        Assert.True(deleted);
        Assert.Equal(1, newIndex);
        Assert.False(disable);
    }

    [Fact]
    public void DeleteSelection_删中间项_保持同一下标_指向原下一项()
    {
        // Count=3，删 index 1 → 删后 Count=2，1 >= 2 为假 → newIndex = 1（原本的下一项左移过来）
        MessageFilterLogic.DeleteSelection(1, 3, out int newIndex, out bool disable, out bool deleted);
        Assert.True(deleted);
        Assert.Equal(1, newIndex);
        Assert.False(disable);
    }

    [Fact]
    public void DeleteSelection_删唯一一项_NewIndex为负1触发禁用()
    {
        MessageFilterLogic.DeleteSelection(0, 1, out int newIndex, out bool disable, out bool deleted);
        Assert.True(deleted);
        Assert.Equal(-1, newIndex);      // 0 >= 0 → -1
        Assert.True(disable);            // 原 :190-194
    }

    [Fact]
    public void DeleteSelection_未选中任何项且列表非空_仍按公式算出NewIndex()
    {
        // nSelectIndex = -1（未选中）→ 未删除 → Count 不变 3 → -1 >= 3 假 → newIndex = -1 → 禁用
        MessageFilterLogic.DeleteSelection(-1, 3, out int newIndex, out bool disable, out bool deleted);
        Assert.False(deleted);
        Assert.Equal(-1, newIndex);
        Assert.True(disable);
    }

    [Fact]
    public void DeleteSelection_空列表()
    {
        MessageFilterLogic.DeleteSelection(0, 0, out int newIndex, out bool disable, out bool deleted);
        Assert.False(deleted);
        Assert.Equal(-1, newIndex);      // 0 >= 0 → -1
        Assert.True(disable);
    }

    // ---------------- ButtonOK（原 :108-133）----------------

    [Fact]
    public void ButtonOK_写回词表文件与INI四键()
    {
        var v = new MessageFilterValues
        {
            FilterTexts = new List<string> { "f1", "f2" },
            FilterSayMsg = true,
            FilterSayMsgMode = TFilterSayMsgMode.fsmmAllBlock,
            WarnSayMsg = "警告",
            FilterSayTriggerScript = true
        };
        MessageFilterLogic.ButtonOK(v, IniPath, WordFile);

        Assert.Equal(2, FormGlobals.g_WordFilterList.Count);
        Assert.Equal("f1", FormGlobals.g_WordFilterList[0]);
        Assert.True(File.Exists(WordFile));

        string expected =
            "[GameGate]\r\n" +
            "FilterSayMsg=1\r\n" +
            "FilterSayMsgMode=0\r\n" +
            "WarnSayMsg=警告\r\n" +
            "FilterSayTriggerScript=1\r\n" +
            "\r\n";
        Assert.Equal(expected, IniText);
    }

    [Fact]
    public void ButtonOK_模式写出枚举整数值()
    {
        MessageFilterLogic.ButtonOK(new MessageFilterValues
        {
            FilterTexts = new List<string>(),
            FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsgorSys
        }, IniPath, WordFile);

        Assert.Contains("FilterSayMsgMode=4\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void ButtonOK_清空原词表后写新的()
    {
        FormGlobals.g_WordFilterList.Add("old");
        MessageFilterLogic.ButtonOK(new MessageFilterValues { FilterTexts = new List<string> { "new" } },
                                    IniPath, WordFile);
        Assert.Equal(1, FormGlobals.g_WordFilterList.Count);
        Assert.Equal("new", FormGlobals.g_WordFilterList[0]);
    }

    [Fact]
    public void ButtonOK_空文件名不抛异常()
    {
        var ex = Record.Exception(() => MessageFilterLogic.ButtonOK(
            new MessageFilterValues { FilterTexts = new List<string> { "a" } }, "", ""));
        Assert.Null(ex);
    }

    // ---------------- 窗体（DFM 对齐 + 端到端）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmMessageFilter();

        Assert.Equal("消息文字过滤设置", f.Text);                    // DFM: Caption
        Assert.Equal(427, f.ClientSize.Width);                       // DFM: ClientWidth=427
        Assert.Equal(249, f.ClientSize.Height);                      // DFM: ClientHeight=249
        Assert.True(f.MinimizeBox);                                  // BorderIcons 含 biMinimize
        Assert.False(f.MaximizeBox);

        Assert.NotNull(f.Label1); Assert.NotNull(f.lstFilterText); Assert.NotNull(f.btnAdd);
        Assert.NotNull(f.btnDel); Assert.NotNull(f.btnOK); Assert.NotNull(f.btnEdit);
        Assert.NotNull(f.GroupBox2); Assert.NotNull(f.Label2); Assert.NotNull(f.chkFilterSayMsg);
        Assert.NotNull(f.rbAllBlock); Assert.NotNull(f.rbSelfBolck); Assert.NotNull(f.rbConnClose);
        Assert.NotNull(f.rbDisMsg); Assert.NotNull(f.rbDisMsgorSys);
        Assert.NotNull(f.chkFilterSayTriggerScript); Assert.NotNull(f.edtWarnSayMsg);

        Assert.Equal("过滤文本:", f.Label1.Text);
        Assert.Equal("增加(&A)", f.btnAdd.Text);
        Assert.Equal("删除(&D)", f.btnDel.Text);
        Assert.Equal("确定(&O)", f.btnOK.Text);
        Assert.Equal("修改(&M)", f.btnEdit.Text);
        Assert.Equal("过滤选项", f.GroupBox2.Text);
        Assert.Equal("开启文字过滤", f.chkFilterSayMsg.Text);
        Assert.Equal("整句使用警告文本替换", f.rbAllBlock.Text);
        Assert.Equal("特征字用警告文本替换", f.rbSelfBolck.Text);
        Assert.Equal("发现过滤文字掉线处理", f.rbConnClose.Text);
        Assert.Equal("发现过滤文字丢包处理", f.rbDisMsg.Text);
        Assert.Equal("发现过滤文字警告处理", f.rbDisMsgorSys.Text);
        Assert.Equal("触发脚本 [@RungateMsgFilter]", f.chkFilterSayTriggerScript.Text);
        Assert.Equal("警告内容:", f.Label2.Text);
    }

    [Fact]
    public void 窗体_btnOK_Click读列表与当前全局值写盘()
    {
        FormGlobals.g_sIniFileName = IniPath;
        FormGlobals.g_sWordFilterFileName = WordFile;
        FormGlobals.g_boFilterSayMsg = true;
        FormGlobals.g_FilterSayMsgMode = TFilterSayMsgMode.fsmmSelfBolck;
        FormGlobals.g_WarnSayMsg = "W";
        FormGlobals.g_boFilterSayTriggerScript = false;

        using var f = new FrmMessageFilter();
        f.lstFilterText.Items.Add("a");
        f.lstFilterText.Items.Add("b");
        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
        Assert.Equal(2, FormGlobals.g_WordFilterList.Count);
        Assert.Contains("FilterSayMsgMode=1\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("WarnSayMsg=W\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_btnEdit_Click未选中时弹错误不弹输入框()
    {
        var shown = new List<string>();
        int inputCalls = 0;
        var originalShow = MessageBoxSeam.Show;
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { inputCalls++; return true; };

            using var f = new FrmMessageFilter();
            f.lstFilterText.Items.Add("a");
            f.lstFilterText.SelectedIndex = -1;

            f.btnEdit_Click(f, EventArgs.Empty);

            Assert.Equal(0, inputCalls);                            // ★ D1：不弹输入框
            Assert.Single(shown);
            Assert.Equal("请输入正确的文本！！！", shown[0]);
        }
        finally
        {
            MessageBoxSeam.Show = originalShow;
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_btnEdit_Click选中且输入新值则覆盖该项()
    {
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "NEW"; return true; };

            using var f = new FrmMessageFilter();
            f.lstFilterText.Items.Add("a");
            f.lstFilterText.Items.Add("b");
            f.lstFilterText.SelectedIndex = 1;
            Assert.Equal("b", f.lstFilterText.Items[1]);   // 编辑前

            f.btnEdit_Click(f, EventArgs.Empty);

            Assert.Equal("NEW", f.lstFilterText.Items[1]);   // 原 :151 覆盖该项
            Assert.Equal("a", f.lstFilterText.Items[0]);
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_btnAdd_Click输入空串报错_输入合法则追加()
    {
        var shown = new List<string>();
        var originalShow = MessageBoxSeam.Show;
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmMessageFilter();

            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = ""; return true; };
            f.btnAdd_Click(f, EventArgs.Empty);
            Assert.Single(shown);
            Assert.Equal("请输入正确的文本！！！", shown[0]);          // 原 :169
            Assert.Equal(0, f.lstFilterText.Items.Count);

            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "ok"; return true; };
            f.btnAdd_Click(f, EventArgs.Empty);
            Assert.Equal(1, f.lstFilterText.Items.Count);
            Assert.Equal("ok", f.lstFilterText.Items[0]);
        }
        finally
        {
            MessageBoxSeam.Show = originalShow;
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_btnAdd_Click取消则不追加不报错()
    {
        var shown = new List<string>();
        var originalShow = MessageBoxSeam.Show;
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => false;

            using var f = new FrmMessageFilter();
            f.btnAdd_Click(f, EventArgs.Empty);

            Assert.Empty(shown);
            Assert.Equal(0, f.lstFilterText.Items.Count);
        }
        finally
        {
            MessageBoxSeam.Show = originalShow;
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_btnDel_Click删中间项后选中原位置的下一项()
    {
        using var f = new FrmMessageFilter();
        f.lstFilterText.Items.AddRange(new object[] { "a", "b", "c" });
        f.lstFilterText.SelectedIndex = 1;

        f.btnDel_Click(f, EventArgs.Empty);

        Assert.Equal(2, f.lstFilterText.Items.Count);
        Assert.Equal("a", f.lstFilterText.Items[0]);
        Assert.Equal("c", f.lstFilterText.Items[1]);
        Assert.Equal(1, f.lstFilterText.SelectedIndex);      // 指向 "c"
    }

    [Fact]
    public void 窗体_btnDel_Click删唯一项后禁用删改按钮()
    {
        using var f = new FrmMessageFilter();
        f.lstFilterText.Items.Add("only");
        f.lstFilterText.SelectedIndex = 0;
        f.btnDel.Enabled = true;
        f.btnEdit.Enabled = true;

        f.btnDel_Click(f, EventArgs.Empty);

        Assert.Equal(0, f.lstFilterText.Items.Count);
        Assert.False(f.btnDel.Enabled);                     // 原 :192
        Assert.False(f.btnEdit.Enabled);                    // 原 :193
    }

    [Fact]
    public void 窗体_btnDel_Click未选中时不抛异常()
    {
        using var f = new FrmMessageFilter();
        f.lstFilterText.Items.AddRange(new object[] { "a", "b" });
        f.lstFilterText.SelectedIndex = -1;
        f.btnDel.Enabled = true;

        var ex = Record.Exception(() => f.btnDel_Click(f, EventArgs.Empty));
        Assert.Null(ex);
        Assert.Equal(2, f.lstFilterText.Items.Count);       // 未删除
        Assert.False(f.btnDel.Enabled);                      // newIndex = -1 → 禁用
    }

    [Fact]
    public void 窗体_五个单选共用一个处理器并按Tag写模式()
    {
        // 原 :213-219：5 个单选的 OnClick 都指向同一个 `rbAllBlockClick`，
        // 处理体用 `Sender as TRadioButton` 取 Tag。托管侧同样只有一个处理器，
        // 用各自的 RadioButton 作为 sender 调用即可（等价于原文的事件分发）。
        using var f = new FrmMessageFilter();

        f.rbAllBlock_Click(f.rbDisMsgorSys, EventArgs.Empty);
        Assert.Equal(TFilterSayMsgMode.fsmmDisMsgorSys, FormGlobals.g_FilterSayMsgMode);

        f.rbAllBlock_Click(f.rbAllBlock, EventArgs.Empty);
        Assert.Equal(TFilterSayMsgMode.fsmmAllBlock, FormGlobals.g_FilterSayMsgMode);

        f.rbAllBlock_Click(f.rbSelfBolck, EventArgs.Empty);
        Assert.Equal(TFilterSayMsgMode.fsmmSelfBolck, FormGlobals.g_FilterSayMsgMode);

        f.rbAllBlock_Click(f.rbConnClose, EventArgs.Empty);
        Assert.Equal(TFilterSayMsgMode.fsmmClose, FormGlobals.g_FilterSayMsgMode);

        f.rbAllBlock_Click(f.rbDisMsg, EventArgs.Empty);
        Assert.Equal(TFilterSayMsgMode.fsmmDisMsg, FormGlobals.g_FilterSayMsgMode);
    }

    [Fact]
    public void 窗体_rbAllBlock_Click的sender非RadioButton时静默忽略()
    {
        using var f = new FrmMessageFilter();
        FormGlobals.g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;

        var ex = Record.Exception(() => f.rbAllBlock_Click(f, EventArgs.Empty));   // sender 是 Form，不是 RadioButton
        Assert.Null(ex);
        Assert.Equal(TFilterSayMsgMode.fsmmDisMsg, FormGlobals.g_FilterSayMsgMode);   // 未改动
    }

    [Fact]
    public void 窗体_edtWarnSayMsgChange与脚本开关即时写全局()
    {
        using var f = new FrmMessageFilter();
        f.edtWarnSayMsg.Text = "T";
        f.edtWarnSayMsg_Change(f, EventArgs.Empty);
        Assert.Equal("T", FormGlobals.g_WarnSayMsg);          // 原 :223

        f.chkFilterSayTriggerScript.Checked = true;
        f.chkFilterSayTriggerScript_Click(f, EventArgs.Empty);
        Assert.True(FormGlobals.g_boFilterSayTriggerScript);   // 原 :229
    }

    [Fact]
    public void 窗体_lstFilterTextClick选中后启用删改按钮()
    {
        using var f = new FrmMessageFilter();
        f.lstFilterText.Items.Add("a");
        f.btnDel.Enabled = false;
        f.btnEdit.Enabled = false;

        f.lstFilterText.SelectedIndex = 0;
        f.lstFilterText_Click(f, EventArgs.Empty);

        Assert.True(f.btnDel.Enabled);      // 原 :103
        Assert.True(f.btnEdit.Enabled);     // 原 :104
    }
}
