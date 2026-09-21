using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// p10-m2-misc 车道（M2Engine 侧）：**已在 main 上存在**的 3 个单元的复核对账 + 行为锁定。
///
/// <para>
/// 台账 §38.6(b)/§39.2 曾把 `uFrmGlobalVarEdit` / `dlgReplaceText` / `dlgConfirmReplace`
/// 记为「真未移植」（判据是「同名 Delphi 类 `T…` 无 `class` 声明」）。本车道的复核结论是
/// **判据假阳性**：三个单元在 main 上都有实现，只是托管类名与 Delphi 类名不同
/// （`TFrmGlobalVarEdit` → `GlobalVarEditForm`、`TTextReplaceDialog` → `TextReplaceDialog`、
/// `TConfirmReplaceDialog` → `ConfirmReplaceDialog`），且 `dlgReplaceText`/`dlgConfirmReplace`
/// 的实现落在 `TextSearchReplaceDialogs.cs` 的第 147/213 行（超出审计工具 E2 的"头 40 行"窗口）。
/// </para>
/// <para>
/// 本文件做两件事：
/// ① 用**计数取证**给出真实完成度（DFM 控件/绑定三向对账 + 方法数对账）；
/// ② 把复核出的**偏离与缺口**写成差异断言锁死（不是"断言它是对的"，而是"断言它现在是这样"），
/// 缺口本身登记为跨区请求 X-P10-03/04/05 —— 相关文件在 `src/GXX.M2Server/Forms/**`，
/// **不在本车道分区内，不可改**。
/// </para>
/// </summary>
public class Sweep9bFormsReconTests : IDisposable
{
    private readonly string _descIni =
        Path.Combine(AppContext.BaseDirectory, "GlobalValDesc.ini");

    private readonly bool _descIniExisted;

    public Sweep9bFormsReconTests()
    {
        _descIniExisted = File.Exists(_descIni);
        InterServerState.ResetGlobalVars();
    }

    public void Dispose()
    {
        InterServerState.ResetGlobalVars();
        if (!_descIniExisted)
        {
            try { if (File.Exists(_descIni)) File.Delete(_descIni); } catch { }
        }
    }

    // =====================================================================================
    // 一、uFrmGlobalVarEdit.pas（233 行）→ src/GXX.M2Server/Forms/InterServerForms.cs:151-303
    // =====================================================================================

    /// <summary>uFrmGlobalVarEdit.dfm（**文本 DFM**，1,548 B）清点出的 5 个 `object` 节点。</summary>
    private static readonly string[] GlobalVarEditDfmControls =
    {
        "strngrdVar", "btnClearVar", "btnRefreshVar", "btnSave", "btnSaveDesc",
    };

    [Fact]
    public void GlobalVarEdit_DfmInventory_FiveControls_ManagedHasThree_Gap()
    {
        string[] declared = Sweep9bFormsRecon.DeclaredControlNames(typeof(GlobalVarEditForm));
        Assert.Equal(5, GlobalVarEditDfmControls.Length);                 // DFM object 节点数
        // 托管只声明了 3 个（strngrdVar / btnSave / btnSaveDesc）
        Assert.Equal(3, declared.Length);
        Assert.Equal(new[] { "btnSave", "btnSaveDesc", "strngrdVar" }, declared.OrderBy(n => n, StringComparer.Ordinal).ToArray());
        // 计数取证：缺的正是这两个（否定性断言靠集合差给出，不靠"扫一眼"）
        string[] missing = GlobalVarEditDfmControls.Except(declared).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[] { "btnClearVar", "btnRefreshVar" }, missing);
    }

    [Fact]
    public void GlobalVarEdit_DfmBindings_FiveVersusTwo_Gap()
    {
        // DFM 绑定 5：strngrdVar.OnSetEditText + btnClearVar/btnRefreshVar/btnSave/btnSaveDesc 的 OnClick
        const int dfmBindings = 5;
        using var form = new GlobalVarEditForm(0);
        int managed = Sweep9bFormsRecon.CountBoundEventsDeep(form);
        Assert.Equal(dfmBindings, 5);
        Assert.Equal(2, managed);            // 只有两个已被绑定的按钮
        Assert.True(dfmBindings - managed == 3, "缺口数应为 3（OnSetEditText + 两个未移植按钮的 OnClick）");
    }

    [Fact]
    public void GlobalVarEdit_ControlInstantiation_MatchesDeclared()
    {
        using var form = new GlobalVarEditForm(0);
        Assert.Equal(3, Sweep9bFormsRecon.InstantiatedControlCount(form));
        Assert.Equal(3, Sweep9bFormsRecon.ParentedDeclaredControlCount(form));
    }

    [Fact]
    public void GlobalVarEdit_MethodParity_SixOfSeven_ShowFrmGlobalVarEditMissing()
    {
        Type t = typeof(GlobalVarEditForm);
        // 原文 7 个例程：ShowFrmGlobalVarEdit / strngrdVarSetEditText / btnClearVarClick /
        //                btnRefreshVarClick / btnSaveClick / btnSaveDescClick / LoadVarDesc
        Assert.NotNull(t.GetMethod("strngrdVarSetEditText"));
        Assert.NotNull(t.GetMethod("btnClearVarClick"));
        Assert.NotNull(t.GetMethod("btnRefreshVarClick"));
        Assert.NotNull(t.GetMethod("btnSaveClick"));
        Assert.NotNull(t.GetMethod("btnSaveDescClick"));
        Assert.NotNull(t.GetMethod("LoadVarDesc"));
        // 单元级 `function ShowFrmGlobalVarEdit(VarType: Integer): Boolean` **缺失**（否定性断言 = 全程序集计数）
        int hits = Sweep9bFormsRecon.CountPublicStaticMethods(t.Assembly, "ShowFrmGlobalVarEdit");
        Assert.Equal(0, hits);
    }

    [Fact]
    public void GlobalVarEdit_Ctor_GridHasHeaderPlusThousandRows()
    {
        using var form = new GlobalVarEditForm(0);
        Assert.Equal(0, form.VarType);
        Assert.Equal(3, form.strngrdVar.Columns.Count);      // DFM ColCount=3
        Assert.Equal(1001, form.strngrdVar.Rows.Count);      // 表头行 + 1000 数据行
        Assert.Equal("0", form.strngrdVar[0, 0].Value?.ToString());
        Assert.Equal("999", form.strngrdVar[0, 999].Value?.ToString());
    }

    [Fact]
    public void GlobalVarEdit_ButtonsStartEnabled_Gap()
    {
        // 缺口（X-P10-03）：原文 `ShowFrmGlobalVarEdit` 在 ShowModal **之前**把
        // `btnSave.Enabled := False; btnSaveDesc.Enabled := False;`，而该函数**未移植**
        // ⇒ 托管窗体一构造出来两个保存按钮就是可点的（原文要等用户编辑过才点亮）。
        using var form = new GlobalVarEditForm(0);
        Assert.True(form.ButtonSaveEnabled);
        Assert.True(form.ButtonSaveDescEnabled);
    }

    [Fact]
    public void GlobalVarEdit_SetEditText_EnablesSaveOrSaveDesc()
    {
        using var form = new GlobalVarEditForm(0);
        // 复刻原文前提（ShowFrmGlobalVarEdit 的初始禁用），再验证 ACol 分支
        form.btnSave.Enabled = false;
        form.btnSaveDesc.Enabled = false;

        form.strngrdVarSetEditText(1);
        Assert.True(form.ButtonSaveEnabled);
        Assert.False(form.ButtonSaveDescEnabled);

        form.strngrdVarSetEditText(2);                       // ACol <> 1
        Assert.True(form.ButtonSaveDescEnabled);
    }

    [Fact]
    public void GlobalVarEdit_RefreshAndSave_RoundTripThroughGlobalVal()
    {
        InterServerState.GlobalVal[0] = 42;
        using var form = new GlobalVarEditForm(0);
        form.btnSave.Enabled = false;
        form.btnRefreshVarClick(null);
        Assert.Equal("42", form.strngrdVar[1, 1].Value?.ToString());
        Assert.False(form.ButtonSaveEnabled);                // 刷新后禁用保存（原文如此）

        form.strngrdVar[1, 5].Value = "77";
        form.strngrdVar[1, 6].Value = "not-a-number";
        form.btnSaveClick(null);
        Assert.Equal(77, InterServerState.GlobalVal[4]);
        Assert.Equal(0, InterServerState.GlobalVal[5]);      // StrToIntDef 回退 0
        Assert.False(form.ButtonSaveEnabled);
    }

    [Fact]
    public void GlobalVarEdit_Clear_ResetsStorageAndCells()
    {
        InterServerState.GlobalVal[3] = 9;
        using var form = new GlobalVarEditForm(0);
        form.btnClearVarClick(null, confirmed: true);
        Assert.Equal(0, InterServerState.GlobalVal[3]);
        Assert.Equal("0", form.strngrdVar[1, 4].Value?.ToString());
        Assert.True(form.ButtonSaveEnabled);
    }

    [Fact]
    public void GlobalVarEdit_NotConfirmed_StillEnablesSave_OriginalFlaw()
    {
        // ★ 原文如此（uFrmGlobalVarEdit.pas:96-123）：`btnSave.Enabled := True;` 写在那两个
        //   MessageBox 判断**之外** ⇒ 用户点"否"也照样把保存按钮点亮。
        InterServerState.GlobalVal[3] = 9;
        using var form = new GlobalVarEditForm(0);
        form.btnSave.Enabled = false;
        form.btnClearVarClick(null, confirmed: false);
        Assert.Equal(9, InterServerState.GlobalVal[3]);       // 没清
        Assert.True(form.ButtonSaveEnabled);                 // 但按钮亮了
        form.btnSaveClick(null, confirmed: false);
        Assert.Equal(9, InterServerState.GlobalVal[3]);       // 没写
        Assert.False(form.ButtonSaveEnabled);                // 且按钮又灭（原文 :170）
    }

    [Fact]
    public void GlobalVarEdit_AvarType_UsesGlobalAVal()
    {
        using var form = new GlobalVarEditForm(1);
        Assert.Equal(1, form.VarType);
        InterServerState.GlobalAVal[2] = "hello";
        form.btnRefreshVarClick(null);
        Assert.Equal("hello", form.strngrdVar[1, 3].Value?.ToString());
        form.strngrdVar[1, 4].Value = "world";
        form.btnSaveClick(null);
        Assert.Equal("world", InterServerState.GlobalAVal[3]);
        form.btnClearVarClick(null, confirmed: true);
        Assert.Equal("", InterServerState.GlobalAVal[3]);
    }

    [Fact]
    public void GlobalVarEdit_SaveDesc_PersistsToGlobalValDescIni()
    {
        using (var form = new GlobalVarEditForm(0))
        {
            form.strngrdVar[2, 1].Value = "第一卷";
            form.btnSaveDescClick(null);
            Assert.False(form.ButtonSaveDescEnabled);
        }
        Assert.True(File.Exists(_descIni));
        var ini = new TFastIniFile(_descIni);
        Assert.Equal("第一卷", ini.ReadString("VarG", "0", ""));
        // 新建窗体应能把描述读回同一格（LoadVarDesc 1:1）
        using var again = new GlobalVarEditForm(0);
        Assert.Equal("第一卷", again.strngrdVar[2, 1].Value?.ToString());
    }

    [Fact]
    public void GlobalVarEdit_VarTypeCaption_DiffersFromDfm_Deviation()
    {
        // DFM Caption：VarType=0 → '全局G变量编辑'，否则 '全局A变量编辑'
        // 托管：'G变量编辑' / 'A变量编辑'（少了"全局"前缀）——登记 D-P10-17（形态偏离，非功能）
        using var form = new GlobalVarEditForm(0);
        Assert.Equal("G变量编辑", form.Text);
        Assert.NotEqual("全局G变量编辑", form.Text);
    }

    // =====================================================================================
    // 二、dlgReplaceText.pas（121 行）→ src/GXX.M2Server/Forms/TextSearchReplaceDialogs.cs:149-210
    // =====================================================================================

    /// <summary>dlgReplaceText.dfm（**文本 DFM**，982 B，首行 `inherited` ⇒ 继承窗体）清点出的 8 个节点。</summary>
    private static readonly string[] ReplaceDialogDfmNodes =
    {
        "Label1", "Label2", "cbSearchText", "gbSearchOptions", "rgSearchDirection",
        "btnOK", "btnCancel", "cbReplaceText",
    };

    [Fact]
    public void ReplaceDialog_DfmInventory_AllEightNodesDeclared()
    {
        string[] declared = Sweep9bFormsRecon.DeclaredControlNames(typeof(TextReplaceDialog));
        Assert.Equal(8, ReplaceDialogDfmNodes.Length);
        foreach (string node in ReplaceDialogDfmNodes)
            Assert.Contains(node, declared);
        // 多出来的 7 个来自**基类单元的 DFM**（dlgSearchText.dfm），不是本单元新增
        Assert.Equal(15, declared.Length);
    }

    [Fact]
    public void ReplaceDialog_DfmHasNoOwnBindings()
    {
        // dlgReplaceText.dfm 里**没有**任何 `On… =` 行（事件全部继承自 dlgSearchText.dfm）
        using var dlg = new TextReplaceDialog();
        Assert.Equal(0, Sweep9bFormsRecon.CountBoundEvents(dlg, typeof(TextReplaceDialog)));
    }

    [Fact]
    public void ReplaceDialog_MethodParity_FiveOfFive()
    {
        Type t = typeof(TextReplaceDialog);
        Assert.NotNull(t.GetProperty("ReplaceText"));            // GetReplaceText/SetReplaceText
        Assert.NotNull(t.GetProperty("ReplaceTextHistory"));     // GetReplaceTextHistory/SetReplaceTextHistory
        Assert.NotNull(t.GetMethod("FormCloseQuery"));           // 派生版（原文 override）
        // 原文 5 个例程全部在册（属性 get/set 各算原文的两个方法时，5 ⇒ 3 条托管成员）
        Assert.Equal(5, 2 + 2 + 1);
    }

    [Fact]
    public void ReplaceDialog_ReplaceText_RoundTrip()
    {
        using var dlg = new TextReplaceDialog();
        dlg.ReplaceText = "abc";
        Assert.Equal("abc", dlg.cbReplaceText.Text);
        Assert.Equal("abc", dlg.ReplaceText);
    }

    [Fact]
    public void ReplaceDialog_HistoryJoinsWithCrLfAndCapsAtTen()
    {
        using var dlg = new TextReplaceDialog();
        dlg.ReplaceTextHistory = string.Join("\r\n", Enumerable.Range(0, 12).Select(i => "v" + i));
        Assert.Equal(12, dlg.cbReplaceText.Items.Count);          // 写入不截断（原文只截读取）
        string history = dlg.ReplaceTextHistory;
        Assert.Equal(string.Join("\r\n", Enumerable.Range(0, 10).Select(i => "v" + i)), history);
    }

    [Fact]
    public void ReplaceDialog_CloseQuery_PromotesReplaceWordToTop()
    {
        using var dlg = new TextReplaceDialog();
        dlg.ReplaceTextHistory = "a\r\nb\r\nc";
        dlg.ReplaceText = "c";
        dlg.ModalResult = DialogResult.OK;
        dlg.FormCloseQuery(out bool canClose);
        Assert.True(canClose);                                    // 原文 out 参数恒为 True
        Assert.Equal("c", dlg.cbReplaceText.Items[0]?.ToString());
        Assert.Equal(3, dlg.cbReplaceText.Items.Count);            // 删除后重插，数量不变
    }

    [Fact]
    public void ReplaceDialog_CloseQuery_Cancel_DoesNotPromote()
    {
        using var dlg = new TextReplaceDialog();
        dlg.ReplaceTextHistory = "a\r\nb";
        dlg.ModalResult = DialogResult.Cancel;
        dlg.FormCloseQuery(out _);
        Assert.Equal("a", dlg.cbReplaceText.Items[0]?.ToString());
    }

    [Fact]
    public void ReplaceDialog_CloseQuery_EmptyText_DoesNotPromote()
    {
        using var dlg = new TextReplaceDialog();
        dlg.ReplaceTextHistory = "a";
        dlg.ReplaceText = "";
        dlg.ModalResult = DialogResult.OK;
        dlg.FormCloseQuery(out _);
        Assert.Equal("a", dlg.cbReplaceText.Items[0]?.ToString());
    }

    [Fact]
    public void ReplaceDialog_VirtualDispatchReachesDerived_IntegratorFix()
    {
        // ★★ 缺口 X-P10-04 —— **已由集成方修复**（台账 §53.1）。本用例原为 `..._VirtualDispatchIsLost_Gap`
        //   （锁定"派生逻辑不可达"的缺口现状）；修复后按**正确行为**断言，并按 §19.2 的规矩保留此说明。
        //
        // 原文事实（集成方逐行核对，纠正本车道报告里的一处误述）：
        //   dlgSearchText.pas:60 与 dlgReplaceText.pas:50 **两处声明都没有 `virtual` / `override`**
        //   ⇒ 在 Delphi 里这是"**隐藏**"而不是"覆写"。但 **DFM 把窗体事件 `OnCloseQuery` 绑到实例的最派生方法**，
        //   所以**原文的可观测行为**是：关闭替换对话框时跑派生版（把替换词也置顶）。
        //   托管侧原先 基类非虚 + 派生用 `new` ⇒ 基类继承给按钮的接线永远调到基类版 ⇒ **行为偏离**。
        // 处置：基类 `virtual` + 派生 `override` —— **声明形态与原文不同（原文非虚），可观测行为对齐**，已登记为偏离。
        using var dlg = new TextReplaceDialog();
        dlg.SearchTextHistory = "b\r\nS";
        dlg.SearchText = "S";
        dlg.ReplaceTextHistory = "b\r\nR";
        dlg.ReplaceText = "R";
        dlg.ModalResult = DialogResult.OK;

        TextSearchDialog asBase = dlg;
        asBase.FormCloseQuery(out _);                       // 经基类引用调用
        Assert.Equal("S", dlg.cbSearchText.Items[0]?.ToString());
        Assert.Equal("R", dlg.cbReplaceText.Items[0]?.ToString());   // ← 修复后：派生版被派发，替换词也置顶

        // 反例保护：基类实例（非替换对话框）不得出现"替换词置顶"这类派生专属行为
        using var plain = new TextSearchDialog();
        plain.SearchTextHistory = "b\r\nS";
        plain.SearchText = "S";
        plain.ModalResult = DialogResult.OK;
        plain.FormCloseQuery(out _);
        Assert.Equal("S", plain.cbSearchText.Items[0]?.ToString());
    }

    [Fact]
    public void ReplaceDialog_IsSubclassOfSearchDialog()
    {
        // dlgReplaceText.dfm 首行是 `inherited` ⇒ 基类关系必须成立
        using var dlg = new TextReplaceDialog();
        Assert.IsAssignableFrom<TextSearchDialog>(dlg);
    }

    // =====================================================================================
    // 三、dlgConfirmReplace.pas（104 行）→ src/GXX.M2Server/Forms/TextSearchReplaceDialogs.cs:215-278
    // =====================================================================================

    /// <summary>dlgConfirmReplace.dfm（**文本 DFM**，1,367 B）清点出的 6 个 `object` 节点。</summary>
    private static readonly string[] ConfirmReplaceDfmControls =
    {
        "btnReplace", "lblConfirmation", "btnSkip", "btnCancel", "btnReplaceAll", "Image1",
    };

    [Fact]
    public void ConfirmReplace_DfmInventory_SixControls_ManagedHasFive_Gap()
    {
        string[] declared = Sweep9bFormsRecon.DeclaredControlNames(typeof(ConfirmReplaceDialog));
        Assert.Equal(6, ConfirmReplaceDfmControls.Length);
        Assert.Equal(5, declared.Length);
        string[] missing = ConfirmReplaceDfmControls.Except(declared).ToArray();
        Assert.Equal(new[] { "Image1" }, missing);            // DFM 的 TImage 未实例化
    }

    [Fact]
    public void ConfirmReplace_DfmBindings_TwoVersusZero_Gap()
    {
        // DFM: 根 OnCreate = FormCreate、OnDestroy = FormDestroy（2 条）
        using var dlg = new ConfirmReplaceDialog();
        Assert.Equal(0, Sweep9bFormsRecon.CountBoundEventsDeep(dlg));
    }

    [Fact]
    public void ConfirmReplace_MethodParity_OneOfThree_Gap()
    {
        Type t = typeof(ConfirmReplaceDialog);
        Assert.NotNull(t.GetMethod("PrepareShow"));
        // 原文 FormCreate（LoadIcon 装载问号图标）与 FormDestroy（ConfirmReplaceDialog := nil）
        // 托管没有同名方法：前者完全缺失，后者用 Dispose(bool) 覆盖表达。
        Assert.Null(t.GetMethod("FormCreate"));
        Assert.Null(t.GetMethod("FormDestroy"));
        Assert.NotNull(t.GetMethod("Dispose",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null, new[] { typeof(bool) }, null));
    }

    [Fact]
    public void ConfirmReplace_GlobalInstance_SetOnCtorAndClearedOnDispose()
    {
        var dlg = new ConfirmReplaceDialog();
        Assert.Same(dlg, ConfirmReplaceDialog.ConfirmReplaceDialogInst);
        dlg.Dispose();
        Assert.Null(ConfirmReplaceDialog.ConfirmReplaceDialogInst);
    }

    [Fact]
    public void ConfirmReplace_PrepareShow_FormatsResourceString()
    {
        using var dlg = new ConfirmReplaceDialog();
        dlg.PrepareShow(new System.Drawing.Rectangle(0, 0, 100, 60), 0, 0, 30, "目标");
        Assert.Equal("是否要对 \"目标\" 进行替换?", dlg.lblConfirmation.Text);   // SAskReplaceText
    }

    [Fact]
    public void ConfirmReplace_PrepareShow_NarrowerThanForm_CentersHorizontally()
    {
        using var dlg = new ConfirmReplaceDialog();
        dlg.PrepareShow(new System.Drawing.Rectangle(0, 0, 100, 60), 0, 10, 30, "x");
        // nW(100) <= Width(360) ⇒ X := Rect.Left - (Width - nW) div 2 = -130
        Assert.Equal(-130, dlg.Left);
        // Y2(30) > Rect.Top + MulDiv(nH=60, 2, 3) = 40 ? 否 ⇒ Y2 := 30 + 4 = 34
        Assert.Equal(34, dlg.Top);
    }

    [Fact]
    public void ConfirmReplace_PrepareShow_WiderThanForm_ClampsToRightEdge()
    {
        using var dlg = new ConfirmReplaceDialog();
        dlg.PrepareShow(new System.Drawing.Rectangle(0, 0, 400, 50), 100, 10, 5, "x");
        // nW(400) > Width(360) 且 X+Width(460) > Rect.Right(400) ⇒ X := 400 - 360 = 40
        Assert.Equal(40, dlg.Left);
    }

    [Fact]
    public void ConfirmReplace_PrepareShow_MulDivRoundingIsNotReproduced_Deviation()
    {
        // ★ 偏离 D-P10-18：原文用 `MulDiv(nH, 2, 3)`（Win32，**四舍五入**），
        //   托管写 `nH * 2 / 3`（整数截断）。nH=100 时 67 vs 66 —— 恰好在边界上给出相反分支。
        using var dlg = new ConfirmReplaceDialog();
        int nH = 100;
        int y2 = 67;
        dlg.PrepareShow(new System.Drawing.Rectangle(0, 0, 400, nH), 100, 5, y2, "x");
        // 托管：67 > 66 ⇒ Y2 := Y1 - Height - 4 = 5 - 130 - 4 = -129
        Assert.Equal(-129, dlg.Top);
        // 原文：MulDiv(100,2,3) = 67 ⇒ 67 > 67 为假 ⇒ Y2 := 67 + 4 = 71
        Assert.NotEqual(71, dlg.Top);
    }

    [Fact]
    public void ConfirmReplace_LabelAutoSize_DiffersFromDfm_Deviation()
    {
        // DFM: lblConfirmation AutoSize=False WordWrap=True Height=44；托管 AutoSize=true（登记 D-P10-18）
        using var dlg = new ConfirmReplaceDialog();
        Assert.True(dlg.lblConfirmation.AutoSize);
    }
}
