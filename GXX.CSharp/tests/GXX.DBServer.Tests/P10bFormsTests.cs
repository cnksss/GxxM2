using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GXX.DBServer;
using GXX.DBServer.Forms2;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// p10-m2-misc 车道（DBServer 侧）：`uFrmHumanExport.pas`（151 行）与 `CreateChr.pas`（69 行）1:1 移植的用例集。
///
/// <para>
/// DFM 对账口径（§37.3 计数取证）：
/// </para>
/// <list type="bullet">
/// <item>`uFrmHumanExport.dfm`（**文本 DFM**，2,542 B）—— 逐行清点 `object` 节点 **10** 个
///   （GroupBox1 / Label1 / Label2 / seLimitCount / seMinLevel / btnHumanExport /
///     GroupBox2 / btnMobileNumberExport / rbAllMobile / rbBindMobile），
///   `On*` 绑定 **2** 条（btnHumanExport.OnClick、btnMobileNumberExport.OnClick）；</item>
/// <item>`CreateChr.dfm`（**二进制 DFM**，944 B，回读 `Source/**` 手工解码，§41.3-1）—— 控件 **8** 个
///   （Label1 / Label2 / Label3 / EdUserId / EdChrName / BitBtnOK / BitBtnCancel / EditSelectID），
///   绑定 **1** 条（根 `OnShow = FormShow`；解码偏移 @310，值 `vaIdent:FormShow`）。</item>
/// </list>
/// <para>
/// 三条对账断言都在本文件里机械执行：
/// ① DFM 控件名集合 == 托管 public 控件字段名集合；
/// ② 控件声明数 == 实例化数 == 挂到 Controls 树上的数量；
/// ③ DFM 绑定数 == <see cref="P10bDfmRecon.CountBoundEventsDeep"/>（走 Component.Events + 静态键，§41.3-2）。
/// </para>
/// </summary>
public class P10bFormsTests : IDisposable
{
    private readonly string _dir;
    private readonly Func<string, string, uint, int> _savedMessageBox = UiSeam.MessageBox;

    public P10bFormsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx-p10b-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        DBServerForms2Ui.ResetForTests();
        SelectClientRoleDbSeam.Reset();
    }

    public void Dispose()
    {
        UiSeam.MessageBox = _savedMessageBox;
        DBServerForms2Ui.ResetForTests();
        SelectClientRoleDbSeam.Reset();
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string Tmp(string name) => Path.Combine(_dir, name);

    private static string ReadGbk(string path)
        => Encoding.GetEncoding(936).GetString(File.ReadAllBytes(path));

    /// <summary>注入一个记录弹窗的 UiSeam，并返回记录列表。</summary>
    private static List<(string Text, string Caption, uint Flags)> CaptureMessageBox()
    {
        var log = new List<(string, string, uint)>();
        UiSeam.MessageBox = (text, caption, flags) =>
        {
            log.Add((text, caption, flags));
            return TMsgBox.IDOK;
        };
        return log;
    }

    // =====================================================================================
    // DFM 对账（uFrmHumanExport）
    // =====================================================================================

    private static readonly string[] HumanExportDfmControls =
    {
        "GroupBox1", "Label1", "Label2", "seLimitCount", "seMinLevel", "btnHumanExport",
        "GroupBox2", "btnMobileNumberExport", "rbAllMobile", "rbBindMobile",
    };

    [Fact]
    public void HumanExport_DfmControlInventory_MatchesDeclaredFields()
    {
        var declared = P10bDfmRecon.DeclaredControlFields(typeof(TFrmHumanExport))
            .Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

        Assert.Equal(10, HumanExportDfmControls.Length);                       // DFM object 节点数
        Assert.Equal(HumanExportDfmControls.OrderBy(n => n, StringComparer.Ordinal).ToArray(), declared);
    }

    [Fact]
    public void HumanExport_ControlDeclaration_Instantiation_Parenting_AllTen()
    {
        using var form = new TFrmHumanExport();
        Assert.Equal(10, P10bDfmRecon.DeclaredControlFields(typeof(TFrmHumanExport)).Count);
        Assert.Equal(10, P10bDfmRecon.InstantiatedControlCount(form));
        // 只数"声明字段里的控件"是否挂进了 Controls 树：直接递归会把 NumericUpDown 的匿名内部
        // 子控件（UpDownEdit/UpDownButtons）也算进来（10 → 14），那是假红（§41.3-2 同类陷阱）。
        Assert.Equal(10, P10bDfmRecon.ParentedDeclaredControlCount(form));
    }

    [Fact]
    public void HumanExport_DfmBindingCount_TwoClickHandlers()
    {
        using var form = new TFrmHumanExport();
        Assert.True(2 == P10bDfmRecon.CountBoundEventsDeep(form),
            "绑定数应为 2，实际 " + P10bDfmRecon.CountBoundEventsDeep(form) +
            "；明细：" + P10bDfmRecon.DescribeBoundEventsDeep(form));
    }

    [Fact]
    public void HumanExport_KeyDfmProperties_PortFaithfully()
    {
        using var form = new TFrmHumanExport();
        Assert.Equal("导出人物数据", form.Text);
        Assert.Equal(new System.Drawing.Size(299, 161), form.ClientSize);
        Assert.Equal("导出离线挂机人物", form.GroupBox1.Text);
        Assert.Equal("导出人物手机号", form.GroupBox2.Text);
        Assert.Equal(100, form.seLimitCount.Value);
        Assert.Equal(20, form.seMinLevel.Value);
        Assert.True(form.rbBindMobile.Checked);
        Assert.False(form.rbAllMobile.Checked);
        Assert.Equal("限制导出数量：", form.Label1.Text);
        Assert.Equal("最低人物等级：", form.Label2.Text);
        Assert.Equal("导出已绑定的手机号", form.rbBindMobile.Text);
    }

    // =====================================================================================
    // TFrmHumanExport 行为
    // =====================================================================================

    [Fact]
    public void HumanExport_WritesTsvOfRoleNameAndAccount()
    {
        var db = new P10bFakeHumanDb();
        db.Roles.Add(new TSerarchRoleData { RoleName = "英雄A", Account = "accA" });
        db.Roles.Add(new TSerarchRoleData { RoleName = "英雄B", Account = "accB" });
        SelectClientRoleDbSeam.HumanDB = db;

        string target = Tmp("AutoLoadOffline.txt");
        var dlg = new P10bFakeSaveDialog { SelectedPath = target };
        DBServerForms2Ui.CreateSaveDialog = () => dlg;
        var messages = CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.seLimitCount.Value = 30;
        form.seMinLevel.Value = 5;
        form.btnHumanExportClick(null);

        Assert.Equal(new[] { (30, 5) }, db.SearchByLevelCalls.ToArray());
        Assert.Equal("英雄A\taccA\r\n英雄B\taccB\r\n", ReadGbk(target));
        Assert.Equal(1, dlg.ExecuteCount);
        Assert.Equal("导出离线挂机人物", dlg.TitleAtExecute);
        Assert.Equal("AutoLoadOffline|*.txt", dlg.FilterAtExecute);
        Assert.Equal("AutoLoadOffline.txt", dlg.FileNameAtExecute);      // Execute 之前设置的三项
        Assert.Single(messages);
        Assert.Equal(target, messages[0].Text);
        Assert.Equal("提示信息", messages[0].Caption);
        Assert.Equal(TMsgBox.MB_ICONQUESTION, messages[0].Flags);
        Assert.True(form.btnHumanExport.Enabled);       // finally 复位
    }

    [Fact]
    public void HumanExport_NoRoles_WritesEmptyFile()
    {
        SelectClientRoleDbSeam.HumanDB = new P10bFakeHumanDb();
        string target = Tmp("empty.txt");
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = target };
        CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.btnHumanExportClick(null);

        Assert.Equal("", ReadGbk(target));
    }

    [Fact]
    public void HumanExport_AppendsTxtExtension_WhenMissing()
    {
        SelectClientRoleDbSeam.HumanDB = new P10bFakeHumanDb();
        string chosen = Tmp("noext_end");
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = chosen };
        var messages = CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.btnHumanExportClick(null);

        // ExtractFileExt('…noext_end') = '' ≠ '.txt' ⇒ ChangeFileExt 追加 '.txt'
        Assert.True(File.Exists(chosen + ".txt"));
        Assert.False(File.Exists(chosen));
        Assert.Equal(chosen + ".txt", messages[0].Text);
    }

    [Fact]
    public void HumanExport_UppercaseTxtExtension_IsRewrittenToLowercase_OriginalFlaw()
    {
        // ★ 原文如此（uFrmHumanExport.pas:85）：判据是 `ExtractFileExt(FileName) <> '.txt'`（**大小写敏感**）
        //   ⇒ 用户选了 `X.TXT` 也会被改写成 `X.txt`（在大小写不敏感的文件系统上落到同一个文件，
        //     在大小写敏感的文件系统上则是另一个文件）。
        SelectClientRoleDbSeam.HumanDB = new P10bFakeHumanDb();
        string chosen = Tmp("UP.TXT");
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = chosen };
        CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.btnHumanExportClick(null);

        Assert.True(File.Exists(Tmp("UP.txt")));
    }

    [Fact]
    public void HumanExport_CancelSaveDialog_WritesNothing()
    {
        SelectClientRoleDbSeam.HumanDB = new P10bFakeHumanDb();
        var dlg = new P10bFakeSaveDialog { ExecuteResult = false, SelectedPath = Tmp("never.txt") };
        DBServerForms2Ui.CreateSaveDialog = () => dlg;
        var messages = CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.btnHumanExportClick(null);

        Assert.False(File.Exists(Tmp("never.txt")));
        Assert.Empty(messages);
        Assert.True(form.btnHumanExport.Enabled);
    }

    [Fact]
    public void HumanExport_SaveFailure_ShowsErrorBoxAndSwallowsException()
    {
        SelectClientRoleDbSeam.HumanDB = new P10bFakeHumanDb();
        // 目标落在一个**不存在的目录**里 ⇒ SaveToFile 抛（原文 except 捕获后只弹错误框，异常不外泄）。
        // 注意不能传 _dir（目录本身）：ExtractFileExt 会返回 ''、ChangeFileExt 追加 '.txt'，
        // 结果变成一个可写文件路径，反而不会抛。
        string bad = Path.Combine(_dir, "missing-dir", "x.txt");
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = bad };
        var messages = CaptureMessageBox();

        using var form = new TFrmHumanExport();
        var ex = Record.Exception(() => form.btnHumanExportClick(null));

        Assert.Null(ex);
        Assert.Single(messages);
        Assert.Equal("错误信息", messages[0].Caption);
        Assert.Equal(TMsgBox.MB_ICONERROR, messages[0].Flags);
        Assert.True(form.btnHumanExport.Enabled);       // finally 仍然复位
    }

    [Fact]
    public void HumanExport_UnwiredRoleDb_ThrowsLoudly()
    {
        // 原文 g_RoleDB.HumanDB 为 nil 时会 AV；托管接缝改成可读异常（登记 D-P10-11）
        SelectClientRoleDbSeam.HumanDB = null;
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = Tmp("x.txt") };
        using var form = new TFrmHumanExport();
        Assert.Throws<NotSupportedException>(() => form.btnHumanExportClick(null));
        Assert.True(form.btnHumanExport.Enabled);
    }

    [Fact]
    public void HumanExport_MobileExport_PassesBindFlagAndWritesLines()
    {
        var db = new P10bFakeHumanDb();
        db.Mobiles.Add("13800000001");
        db.Mobiles.Add("13800000002");
        SelectClientRoleDbSeam.HumanDB = db;

        string target = Tmp("手机号码.txt");
        var dlg = new P10bFakeSaveDialog { SelectedPath = target };
        DBServerForms2Ui.CreateSaveDialog = () => dlg;
        var messages = CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.btnMobileNumberExportClick(null);

        Assert.Equal(new[] { true }, db.GetMobileNumbersCalls.ToArray());    // rbBindMobile.Checked 默认真
        Assert.Equal("13800000001\r\n13800000002\r\n", ReadGbk(target));
        Assert.Equal("导出人物手机号码", dlg.TitleAtExecute);
        Assert.Equal("文本文件(*.txt)|*.txt", dlg.FilterAtExecute);
        Assert.Equal("手机号码.txt", dlg.FileNameAtExecute);
        Assert.Equal(target, messages[0].Text);
        Assert.True(form.btnMobileNumberExport.Enabled);
    }

    [Fact]
    public void HumanExport_MobileExport_AllMobileFlagIsForwarded()
    {
        var db = new P10bFakeHumanDb();
        SelectClientRoleDbSeam.HumanDB = db;
        DBServerForms2Ui.CreateSaveDialog = () => new P10bFakeSaveDialog { SelectedPath = Tmp("m.txt") };
        CaptureMessageBox();

        using var form = new TFrmHumanExport();
        form.rbAllMobile.Checked = true;               // 与 rbBindMobile 互斥（同容器）
        Assert.False(form.rbBindMobile.Checked);
        form.btnMobileNumberExportClick(null);

        Assert.Equal(new[] { false }, db.GetMobileNumbersCalls.ToArray());
    }

    [Fact]
    public void ShowFrmHumanExport_ShowsModalAndDisposes()
    {
        Form? shown = null;
        DBServerForms2Ui.ShowModal = f => { shown = f; return DialogResult.Cancel; };

        HumanExportUnit.ShowFrmHumanExport();

        Assert.NotNull(shown);
        Assert.IsType<TFrmHumanExport>(shown);
        Assert.True(shown!.IsDisposed);                // finally 里 Free
    }

    // =====================================================================================
    // DFM 对账（CreateChr）
    // =====================================================================================

    private static readonly string[] CreateChrDfmControls =
    {
        "Label1", "Label2", "Label3", "EdUserId", "EdChrName", "BitBtnOK", "BitBtnCancel", "EditSelectID",
    };

    [Fact]
    public void CreateChr_DfmControlInventory_MatchesDeclaredFields()
    {
        var declared = P10bDfmRecon.DeclaredControlFields(typeof(TFrmCreateChr))
            .Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

        Assert.Equal(8, CreateChrDfmControls.Length);                          // 二进制 DFM 解出的 object 节点数
        Assert.Equal(CreateChrDfmControls.OrderBy(n => n, StringComparer.Ordinal).ToArray(), declared);
    }

    [Fact]
    public void CreateChr_ControlDeclaration_Instantiation_Parenting_AllEight()
    {
        using var form = new TFrmCreateChr();
        Assert.Equal(8, P10bDfmRecon.DeclaredControlFields(typeof(TFrmCreateChr)).Count);
        Assert.Equal(8, P10bDfmRecon.InstantiatedControlCount(form));
        Assert.Equal(8, P10bDfmRecon.ParentedDeclaredControlCount(form));
    }

    [Fact]
    public void CreateChr_DfmBindingCount_OneShowHandler()
    {
        using var form = new TFrmCreateChr();
        Assert.True(1 == P10bDfmRecon.CountBoundEventsDeep(form),
            "绑定数应为 1，实际 " + P10bDfmRecon.CountBoundEventsDeep(form) +
            "；明细：" + P10bDfmRecon.DescribeBoundEventsDeep(form));
    }

    [Fact]
    public void CreateChr_KeyDfmProperties_PortFaithfully()
    {
        using var form = new TFrmCreateChr();
        Assert.Equal("创建新人物", form.Text);
        Assert.Equal(new System.Drawing.Size(250, 130), form.ClientSize);
        Assert.Equal("登录帐号:", form.Label1.Text);
        Assert.Equal("人物名称:", form.Label2.Text);
        Assert.Equal("选择ID:", form.Label3.Text);
        Assert.Equal("确定(&O)", form.BitBtnOK.Text);
        Assert.Equal("取消(&C)", form.BitBtnCancel.Text);
        Assert.Equal("0", form.EditSelectID.Text);              // DFM: EditSelectID.Text = '0'
        Assert.Equal(new System.Drawing.Point(23, 90), form.BitBtnOK.Location);
        Assert.Equal(new System.Drawing.Size(93, 31), form.BitBtnOK.Size);
        Assert.Equal(DialogResult.OK, form.BitBtnOK.DialogResult);
        Assert.Equal(DialogResult.Cancel, form.BitBtnCancel.DialogResult);
    }

    // =====================================================================================
    // TFrmCreateChr 行为
    // =====================================================================================

    [Fact]
    public void CreateChr_ShowModalCancel_ReturnsFalseAndKeepsEmptyFields()
    {
        using var form = new TFrmCreateChr();
        form.ShowModalHandler = () => DialogResult.Cancel;

        Assert.False(form.IncputChrInfo());
        Assert.Equal("", form.sUserId);
        Assert.Equal("", form.sChrName);
        Assert.Equal(0, form.nSelectID);                        // Delphi 字段默认 0，取消时不改
    }

    [Fact]
    public void CreateChr_Ok_TrimsTextsAndParsesSelectId()
    {
        using var form = new TFrmCreateChr();
        form.ShowModalHandler = () =>
        {
            form.EdUserId.Text = "  acc  ";
            form.EdChrName.Text = "  英雄  ";
            form.EditSelectID.Text = " 5 ";
            return DialogResult.OK;
        };

        Assert.True(form.IncputChrInfo());
        Assert.Equal("acc", form.sUserId);
        Assert.Equal("英雄", form.sChrName);
        Assert.Equal(5, form.nSelectID);
    }

    [Fact]
    public void CreateChr_Ok_DefaultSelectIdZero_IsAccepted()
    {
        using var form = new TFrmCreateChr();
        form.ShowModalHandler = () => DialogResult.OK;
        Assert.True(form.IncputChrInfo());
        Assert.Equal(0, form.nSelectID);
    }

    [Fact]
    public void CreateChr_InvalidSelectId_ShowsBoxAndReturnsFalse()
    {
        using var form = new TFrmCreateChr();
        var messages = CaptureMessageBox();
        form.ShowModalHandler = () =>
        {
            form.EditSelectID.Text = "abc";                     // StrToIntDef(..., -1) ⇒ -1
            return DialogResult.OK;
        };

        Assert.False(form.IncputChrInfo());
        Assert.Equal(-1, form.nSelectID);
        Assert.Single(messages);
        Assert.Equal("选择ID输入不正确！！！", messages[0].Text);
        Assert.Equal("确认信息", messages[0].Caption);
        Assert.Equal(TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION, messages[0].Flags);
    }

    [Fact]
    public void CreateChr_NegativeSelectId_ShowsBoxAndReturnsFalse()
    {
        using var form = new TFrmCreateChr();
        var messages = CaptureMessageBox();
        form.ShowModalHandler = () =>
        {
            form.EditSelectID.Text = "-3";
            return DialogResult.OK;
        };

        Assert.False(form.IncputChrInfo());
        Assert.Equal(-3, form.nSelectID);
        Assert.Single(messages);
    }

    [Fact]
    public void CreateChr_PrefillsEditsFromPublicFields_ThenClearsThem_OriginalFlaw()
    {
        // ★ 原文如此（CreateChr.pas:44-49 → :54-55）：IncputChrInfo **先清空** sUserId/sChrName，
        //   再在 GetInputInfo 里用这两个字段回填编辑框 ⇒ 回填进去的**永远是空串**，
        //   "预置初值"这条设计意图在原文里已经失效（照抄 + 差异断言锁定）。
        using var form = new TFrmCreateChr();
        form.sUserId = "presetUser";
        form.sChrName = "presetName";
        string seenUser = "", seenName = "";
        form.ShowModalHandler = () =>
        {
            seenUser = form.EdUserId.Text;
            seenName = form.EdChrName.Text;
            return DialogResult.Cancel;
        };

        Assert.False(form.IncputChrInfo());
        Assert.Equal("", seenUser);
        Assert.Equal("", seenName);
    }

    [Fact]
    public void CreateChr_FormShow_DoesNotThrowWithoutHandle()
    {
        using var form = new TFrmCreateChr();
        var ex = Record.Exception(() => form.FormShow(null));
        Assert.Null(ex);
    }

    [Fact]
    public void CreateChr_GlobalInstanceField_IsPublicAndSettable()
    {
        // CreateChr.pas:31 var FrmCreateChr（DBServer.dpr:37 Application.CreateForm）
        using var form = new TFrmCreateChr();
        TFrmCreateChr.FrmCreateChr = form;
        try
        {
            Assert.Same(form, TFrmCreateChr.FrmCreateChr);
        }
        finally
        {
            TFrmCreateChr.FrmCreateChr = null;
        }
    }

    // =====================================================================================
    // 接缝自身
    // =====================================================================================

    [Fact]
    public void FileUtils_ExtractAndChangeExt_MatchDelphiSemantics()
    {
        Assert.Equal(".txt", P10bFileUtils.ExtractFileExt(@"C:\a\b.txt"));
        Assert.Equal("", P10bFileUtils.ExtractFileExt(@"C:\a\b"));
        Assert.Equal("", P10bFileUtils.ExtractFileExt(@"C:\a.b\c"));       // '.' 在路径分隔符之前 ⇒ 无扩展名
        Assert.Equal(@"C:\a\b.log", P10bFileUtils.ChangeFileExt(@"C:\a\b.txt", ".log"));
        Assert.Equal(@"C:\a\b.log", P10bFileUtils.ChangeFileExt(@"C:\a\b", ".log"));
    }

    [Fact]
    public void SaveDialogSeam_PortsTitleFilterFileName()
    {
        var dlg = new TSaveDialogSeam { Title = "t", Filter = "f|*.x", FileName = "a.txt" };
        Assert.Equal("t", dlg.Title);
        Assert.Equal("f|*.x", dlg.Filter);
        Assert.Equal("a.txt", dlg.FileName);
    }

    [Fact]
    public void EventCounter_IsNotZero_GuardAgainstFalseGreen()
    {
        // §41.3-2 的"假绿"哨兵：反射数 field-like 事件字段在 .NET 8 上会得 0，
        // 这里断言两条路都仍在工作 —— 若 CountBoundEvents 因框架改名而失效，本用例会先红。
        using var form = new TFrmHumanExport();
        Assert.True(P10bDfmRecon.CountBoundEvents(form) >= 0);
        int n = 0;
        var probe = new Button();
        probe.Click += (s, e) => n++;
        Assert.Equal(1, P10bDfmRecon.CountBoundEvents(probe));
    }
}
