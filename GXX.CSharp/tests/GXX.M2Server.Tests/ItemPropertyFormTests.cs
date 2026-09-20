using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms.ItemProperty;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：uFrmCustomItemProperty.pas 窗体 1:1 测试。
///
/// 覆盖：类构造（DFM OnCreate = FormCreate）、120 个控件的下标映射（chk01..60 / edtShowName01..60）、
/// FormCreate 回填、btnOKClick 写回 + Config INI + CRC 分支、btnOK2Click + TStrings.Text 往返、
/// 6 个事件处理器、ShowFrmCustomItemProperty 的 boStartReady 门控、DFM 事件布线表。
/// </summary>
[Collection("ItemPropertyLane")]
public sealed class ItemPropertyFormTests : ItemPropertyTestBase
{
    // ------------------------------------------------------------------
    // 构造 / DFM OnCreate
    // ------------------------------------------------------------------

    [Fact]
    public void Ctor_TriggersFormCreateViaDfmOnCreate()
    {
        FillChecksAllTrue();
        FillBindNamesDistinct();

        var frm = new TFrmCustomItemProperty();          // 原文 :170 Create(nil) → DFM:17 OnCreate

        Action formCreate = frm.FormCreate;
        Assert.Equal(formCreate, frm.self.OnCreate);
        for (int i = 1; i <= 60; i++)
        {
            Assert.True(frm.chk[i].Checked);
            Assert.Equal("名称" + i.ToString("00"), frm.edtShowName[i].Text);
        }
    }

    [Fact]
    public void ControlArrays_HaveIndexZeroUnused()
    {
        var frm = new TFrmCustomItemProperty();
        Assert.Equal(61, frm.chk.Length);                 // Delphi 下界 1 → 下标 0 不用
        Assert.Equal(61, frm.edtShowName.Length);
        Assert.Null(frm.chk[0]);
        Assert.Null(frm.edtShowName[0]);
    }

    [Fact]
    public void AliasFields_PointToSameSeamObjectAsArray()
    {
        var frm = new TFrmCustomItemProperty();
        Assert.Same(frm.chk[1], frm.chk01);
        Assert.Same(frm.chk[60], frm.chk60);
        Assert.Same(frm.edtShowName[1], frm.edtShowName01);
        Assert.Same(frm.edtShowName[60], frm.edtShowName60);
    }

    [Fact]
    public void Ctor_UnwiredSeams_ThrowsLoudlyInsteadOfSilentlySucceeding()
    {
        CustomItemPropertyGlobals.Reset();               // 撤掉接线
        var ex = Assert.Throws<InvalidOperationException>(() => new TFrmCustomItemProperty());
        Assert.Contains("接缝未接线", ex.Message);
    }

    // ------------------------------------------------------------------
    // FormCreate（原文 :178-306）
    // ------------------------------------------------------------------

    [Fact]
    public void FormCreate_BackfillsAllSixtyCheckBoxesByIndex()
    {
        for (int i = 1; i <= 60; i++)
            Checks[i] = i % 2 == 0;                      // 交错，任何下标错位都会暴露
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            Assert.Equal(i % 2 == 0, frm.chk[i].Checked);
    }

    [Fact]
    public void FormCreate_BackfillsAllSixtyEditBoxesByIndex()
    {
        FillBindNamesDistinct();
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            Assert.Equal("名称" + i.ToString("00"), frm.edtShowName[i].Text);
    }

    [Fact]
    public void FormCreate_DisablesBothOkButtons()
    {
        FillChecksAllTrue();
        var frm = new TFrmCustomItemProperty();
        Assert.False(frm.btnOK.Enabled);                 // :302
        Assert.False(frm.btnOK2.Enabled);                // :305
    }

    [Fact]
    public void FormCreate_LoadsTextVarListIntoCaretMemo()
    {
        TextVarList.Add("第一行");                       // 原文 :304 mmoVar.Text := ...Text
        TextVarList.Add("第二行");
        var frm = new TFrmCustomItemProperty();
        Assert.Equal("第一行\r\n第二行\r\n", frm.mmoVar.Text);
    }

    [Fact]
    public void FormCreate_EmptyTextVarList_YieldsEmptyMemo()
    {
        var frm = new TFrmCustomItemProperty();
        Assert.Equal("", frm.mmoVar.Text);
    }

    // ------------------------------------------------------------------
    // WireDfmEvents（DFM 事件布线 1:1）
    // ------------------------------------------------------------------

    [Fact]
    public void DfmWiring_AllSixtyCheckBoxesShareChk01Click()
    {
        var frm = new TFrmCustomItemProperty();
        Action expected = frm.chk01Click;
        for (int i = 1; i <= 60; i++)
        {
            var handler = frm.chk[i].OnClick;
            Assert.NotNull(handler);
            Assert.Equal(expected, handler);
        }
    }

    [Fact]
    public void DfmWiring_AllSixtyEditsShareEdtShowName01Change()
    {
        var frm = new TFrmCustomItemProperty();
        Action expected = frm.edtShowName01Change;
        for (int i = 1; i <= 60; i++)
        {
            var handler = frm.edtShowName[i].OnChange;
            Assert.NotNull(handler);
            Assert.Equal(expected, handler);
        }
    }

    [Fact]
    public void DfmWiring_ButtonsAndMemoHandlers()
    {
        var frm = new TFrmCustomItemProperty();
        Assert.NotNull(frm.btnOK.OnClick);                // DFM:1266
        Assert.NotNull(frm.btnOK2.OnClick);               // DFM:1315
        Assert.NotNull(frm.mmoVar.OnChange);              // DFM:1326
        Assert.NotNull(frm.mmoVar.OnKeyUp);               // DFM:1327
        Assert.NotNull(frm.mmoVar.OnMouseDown);           // DFM:1328
    }

    [Fact]
    public void DfmWiring_InvokingEachBoxWiring_EnablesBtnOk()
    {
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
        {
            frm.btnOK.Enabled = false;
            frm.chk[i].OnClick!();
            Assert.True(frm.btnOK.Enabled);
            frm.btnOK.Enabled = false;
            frm.edtShowName[i].OnChange!();
            Assert.True(frm.btnOK.Enabled);
        }
    }

    [Fact]
    public void DfmWiring_MemoWiring_DrivesBtnOk2AndLineLabel()
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.OnChange!();
        Assert.True(frm.btnOK2.Enabled);

        frm.mmoVar.CaretPosY = 2;
        frm.mmoVar.OnKeyUp!();
        Assert.Equal("当前行：3", frm.lblLineNum.Caption);

        frm.mmoVar.CaretPosY = 0;
        frm.mmoVar.OnMouseDown!();
        Assert.Equal("当前行：1", frm.lblLineNum.Caption);
    }

    [Fact]
    public void DfmWiring_IsIdempotent()
    {
        var frm = new TFrmCustomItemProperty();
        frm.WireDfmEvents();
        frm.WireDfmEvents();
        Action chkHandler = frm.chk01Click;
        Action edtHandler = frm.edtShowName01Change;
        Assert.Equal(chkHandler, frm.chk[37].OnClick);
        Assert.Equal(edtHandler, frm.edtShowName[37].OnChange);
    }

    // ------------------------------------------------------------------
    // 6 个事件处理器
    // ------------------------------------------------------------------

    [Fact]
    public void Chk01Click_EnablesBtnOkOnly()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK.Enabled = false;
        frm.btnOK2.Enabled = false;
        frm.chk01Click();                                 // :452-455
        Assert.True(frm.btnOK.Enabled);
        Assert.False(frm.btnOK2.Enabled);
    }

    [Fact]
    public void Chk01Click_IsIdempotent()
    {
        var frm = new TFrmCustomItemProperty();
        frm.chk01Click();
        frm.chk01Click();
        Assert.True(frm.btnOK.Enabled);
    }

    [Fact]
    public void EdtShowName01Change_EnablesBtnOkOnly()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK.Enabled = false;
        frm.edtShowName01Change();                        // :457-461
        Assert.True(frm.btnOK.Enabled);
        Assert.False(frm.btnOK2.Enabled);
    }

    [Fact]
    public void EdtShowName01Change_IsIdempotent()
    {
        var frm = new TFrmCustomItemProperty();
        frm.edtShowName01Change();
        frm.edtShowName01Change();
        Assert.True(frm.btnOK.Enabled);
    }

    [Fact]
    public void MmoVarChange_EnablesBtnOk2Only()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK.Enabled = false;
        frm.btnOK2.Enabled = false;
        frm.mmoVarChange();                               // :463-466
        Assert.True(frm.btnOK2.Enabled);
        Assert.False(frm.btnOK.Enabled);
    }

    [Fact]
    public void MmoVarChange_IsIdempotent()
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVarChange();
        frm.mmoVarChange();
        Assert.True(frm.btnOK2.Enabled);
    }

    [Theory]
    [InlineData(0, "当前行：1")]
    [InlineData(3, "当前行：4")]
    [InlineData(59, "当前行：60")]
    public void MmoVarKeyUp_WritesOneBasedLineNumber(int caretY, string expected)
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.CaretPosY = caretY;
        frm.mmoVarKeyUp();                                // :486-490
        Assert.Equal(expected, frm.lblLineNum.Caption);
    }

    [Theory]
    [InlineData(0, "当前行：1")]
    [InlineData(3, "当前行：4")]
    [InlineData(59, "当前行：60")]
    public void MmoVarMouseDown_WritesOneBasedLineNumber(int caretY, string expected)
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.CaretPosY = caretY;
        frm.mmoVarMouseDown();                            // :492-496
        Assert.Equal(expected, frm.lblLineNum.Caption);
    }

    [Fact]
    public void MmoVarKeyUpAndMouseDown_IgnoreTheirArguments()
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.CaretPosY = 7;
        frm.mmoVarKeyUp(null, 0xFFFF, new object());
        Assert.Equal("当前行：8", frm.lblLineNum.Caption);
        frm.mmoVarMouseDown(null, 2, new object(), 123, 456);
        Assert.Equal("当前行：8", frm.lblLineNum.Caption);
    }

    // ------------------------------------------------------------------
    // btnOKClick（原文 :308-450）
    // ------------------------------------------------------------------

    [Fact]
    public void BtnOKClick_WritesBackAllSixtyChecksByIndex()
    {
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            frm.chk[i].Checked = i % 3 == 0;             // 交错，任何下标错位都会暴露
        frm.btnOKClick();
        for (int i = 1; i <= 60; i++)
            Assert.Equal(i % 3 == 0, Checks[i]);
    }

    [Fact]
    public void BtnOKClick_WritesBackAllSixtyBindNamesByIndex()
    {
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            frm.edtShowName[i].Text = "edit" + i.ToString("00");
        frm.btnOKClick();
        for (int i = 1; i <= 60; i++)
            Assert.Equal("edit" + i.ToString("00"), BindNames[i]);
    }

    [Fact]
    public void BtnOKClick_WritesConfigIniForEveryIndexInRange()
    {
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
        {
            frm.chk[i].Checked = i == 7;
            frm.edtShowName[i].Text = "bind" + i.ToString("00");
        }
        frm.btnOKClick();                                 // :437/:438

        var cfg = M2ShareState.ConfigIni;
        Assert.True(cfg.ReadBool("Setup", "CustomItemPropertyCheck7", false));
        Assert.False(cfg.ReadBool("Setup", "CustomItemPropertyCheck8", true));
        Assert.Equal("bind01", cfg.ReadString("Setup", "CustomItemPropertyBindName1", ""));
        Assert.Equal("bind60", cfg.ReadString("Setup", "CustomItemPropertyBindName60", ""));
        Assert.True(cfg.ValueExists("Setup", "CustomItemPropertyCheck60"));
        Assert.True(cfg.ValueExists("Setup", "CustomItemPropertyBindName1"));
    }

    [Fact]
    public void BtnOKClick_ConfigKeysUseOneBasedIndicesOnly()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOKClick();
        var cfg = M2ShareState.ConfigIni;
        Assert.False(cfg.ValueExists("Setup", "CustomItemPropertyCheck0"));    // Low = 1
        Assert.False(cfg.ValueExists("Setup", "CustomItemPropertyCheck61"));   // High = 60
        Assert.True(cfg.ValueExists("Setup", "CustomItemPropertyCheck1"));
        Assert.True(cfg.ValueExists("Setup", "CustomItemPropertyCheck60"));
    }

    [Fact]
    public void BtnOKClick_ConfigIsFlushedToDisk()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOKClick();
        string file = Path.Combine(Dir, M2ShareState.sConfigFileName);
        Assert.True(File.Exists(file));                   // 偏离 D-P8-1：补 UpdateFile 后立即落盘
        Assert.Contains("CustomItemPropertyCheck1=", File.ReadAllText(file));
    }

    [Fact]
    public void BtnOKClick_UnchangedCrc_DoesNotSend()
    {
        var frm = new TFrmCustomItemProperty();
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig = () => RebuildCalls++;   // CRC 不变
        frm.btnOKClick();                                 // :441-447
        Assert.Equal(1, RebuildCalls);
        Assert.Equal(0, SendConfigCalls);
    }

    [Fact]
    public void BtnOKClick_ChangedCrc_SendsConfigOnce()
    {
        var frm = new TFrmCustomItemProperty();
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig =
            () => { RebuildCalls++; CustomItemPropertyGlobals.g_CustomItemPropertyCRC = 0xA5A5A5A5u; };
        frm.btnOKClick();
        Assert.Equal(1, RebuildCalls);
        Assert.Equal(1, SendConfigCalls);
    }

    [Fact]
    public void BtnOKClick_SameCrcValueWrittenByRebuild_StillDoesNotSend()
    {
        var frm = new TFrmCustomItemProperty();
        CustomItemPropertyGlobals.g_CustomItemPropertyCRC = 42u;
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig =
            () => CustomItemPropertyGlobals.g_CustomItemPropertyCRC = 42u;                   // 值相同 → 不发送
        frm.btnOKClick();
        Assert.Equal(0, SendConfigCalls);
    }

    [Fact]
    public void BtnOKClick_DisablesBtnOkAtTheEnd()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK.Enabled = true;
        frm.btnOKClick();
        Assert.False(frm.btnOK.Enabled);                  // :449
    }

    [Fact]
    public void BtnOKClick_UnwiredRebuild_ThrowsAndLeavesBtnOkEnabled()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK.Enabled = true;
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig = null;
        var ex = Assert.Throws<InvalidOperationException>(frm.btnOKClick);
        Assert.Contains("接缝未接线", ex.Message);
        Assert.True(frm.btnOK.Enabled);                   // 异常在 :449 之前抛出 —— 与原文一致
    }

    [Fact]
    public void BtnOKClick_RepeatedCalls_RebuildEachTime()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOKClick();
        frm.btnOKClick();
        Assert.Equal(2, RebuildCalls);
        Assert.Equal(0, SendConfigCalls);
    }

    // ------------------------------------------------------------------
    // btnOK2Click（原文 :468-484）
    // ------------------------------------------------------------------

    [Fact]
    public void BtnOK2Click_WritesMemoTextIntoTextVarList()
    {
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.Text = "a\r\nb\r\n";
        frm.btnOK2Click();                                // :472
        Assert.Equal(2, TextVarList.Count);
        Assert.Equal("a", TextVarList[0]);
        Assert.Equal("b", TextVarList[1]);
    }

    [Fact]
    public void BtnOK2Click_EmptyMemo_ClearsTextVarList()
    {
        TextVarList.Add("stale");
        var frm = new TFrmCustomItemProperty();
        frm.mmoVar.Text = "";
        frm.btnOK2Click();
        Assert.Equal(0, TextVarList.Count);
    }

    [Fact]
    public void BtnOK2Click_AlwaysCallsSave()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK2Click();                                // :476
        Assert.Equal(1, SaveCalls);
    }

    [Fact]
    public void BtnOK2Click_UnchangedCrc_DoesNotSend()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK2Click();                                // Save 不改 CRC → :478 为假
        Assert.Equal(0, SendTextVarListCalls);
    }

    [Fact]
    public void BtnOK2Click_ChangedCrc_SendsTextVarListOnce()
    {
        var frm = new TFrmCustomItemProperty();
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList =
            () => CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC = 7u;     // 模拟 CRC 变化
        frm.btnOK2Click();
        Assert.Equal(1, SendTextVarListCalls);
    }

    [Fact]
    public void BtnOK2Click_SameCrcValueWrittenBySave_StillDoesNotSend()
    {
        var frm = new TFrmCustomItemProperty();
        CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC = 9u;
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList =
            () => CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC = 9u;
        frm.btnOK2Click();
        Assert.Equal(0, SendTextVarListCalls);
    }

    [Fact]
    public void BtnOK2Click_DisablesBtnOk2AtTheEnd()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK2.Enabled = true;
        frm.btnOK2Click();
        Assert.False(frm.btnOK2.Enabled);                 // :483
    }

    [Fact]
    public void BtnOK2Click_UnwiredSave_ThrowsAndLeavesBtnOk2Enabled()
    {
        var frm = new TFrmCustomItemProperty();
        frm.btnOK2.Enabled = true;
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList = null;
        var ex = Assert.Throws<InvalidOperationException>(frm.btnOK2Click);
        Assert.Contains("接缝未接线", ex.Message);
        Assert.True(frm.btnOK2.Enabled);                  // 异常在 :483 之前抛出
    }

    // ------------------------------------------------------------------
    // ShowFrmCustomItemProperty（原文 :159、:165-176）
    // ------------------------------------------------------------------

    [Fact]
    public void ShowFrm_WhenNotStartReady_DoesNothing()
    {
        CustomItemPropertyGlobals.boStartReady = false;   // 原文 :169 if not boStartReady then Exit
        FillChecksAllTrue();
        TFrmCustomItemProperty.ShowFrmCustomItemProperty();
        Assert.Equal(0, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void ShowFrm_WhenStartReady_CreatesFormAndCallsShowModalOnce()
    {
        CustomItemPropertyGlobals.boStartReady = true;
        FillBindNamesDistinct();
        TFrmCustomItemProperty.ShowFrmCustomItemProperty();
        Assert.Equal(1, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void ShowFrm_HeadlessUiEnabled_StillCountsWithoutBlocking()
    {
        CustomItemPropertyGlobals.boStartReady = true;
        CustomItemPropertyMessageBoxSeam.UiEnabled = true;   // 生产标志；本接缝不创建窗口，故不挂死
        TFrmCustomItemProperty.ShowFrmCustomItemProperty();
        Assert.Equal(1, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void ShowFrm_StartReadyButSeamsUnwired_Throws()
    {
        CustomItemPropertyGlobals.boStartReady = true;
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks = null!;
        Assert.Throws<InvalidOperationException>(TFrmCustomItemProperty.ShowFrmCustomItemProperty);
    }

    [Fact]
    public void ShowModal_DelegatesToSeam()
    {
        var frm = new TFrmCustomItemProperty();
        frm.ShowModal();
        Assert.Equal(1, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    // ------------------------------------------------------------------
    // 端到端工作流（原文真实使用路径）
    // ------------------------------------------------------------------

    [Fact]
    public void Workflow_LoadEditCheckUncheckSave_RoundTripsThroughIni()
    {
        FillChecksAllTrue();
        FillBindNamesDistinct();
        TextVarList.Add("var1");

        // 1) 打开窗体（原文 ShowFrmCustomItemProperty + FormCreate）
        CustomItemPropertyGlobals.boStartReady = true;
        TFrmCustomItemProperty.ShowFrmCustomItemProperty();
        Assert.Equal(1, CustomItemPropertyMessageBoxSeam.ShowModalCalls);

        // 2) 用户改控件（原文 chk01Click / edtShowName01Change / mmoVarChange）
        var frm = new TFrmCustomItemProperty();
        frm.chk[3].Checked = false;
        frm.edtShowName[3].Text = "改过的名字";
        frm.mmoVar.Text = "var1\r\nvar2\r\n";
        frm.chk01Click();
        frm.mmoVarChange();
        Assert.True(frm.btnOK.Enabled);
        Assert.True(frm.btnOK2.Enabled);

        // 3) 保存（原文 btnOKClick / btnOK2Click）：CRC 变化 → 各自发送一次
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig =
            () => CustomItemPropertyGlobals.g_CustomItemPropertyCRC = 0x11223344u;
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList =
            () => CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC = 0x55667788u;
        frm.btnOKClick();
        frm.btnOK2Click();

        Assert.Equal(1, SendConfigCalls);
        Assert.Equal(1, SendTextVarListCalls);
        Assert.False(Checks[3]);
        Assert.Equal("改过的名字", BindNames[3]);
        Assert.Equal(2, TextVarList.Count);
        Assert.Equal("var2", TextVarList[1]);
        Assert.Equal(1,
            M2ShareState.ConfigIni.ReadInteger("Setup", "CustomItemPropertyCheck1", -1));
        Assert.Equal("改过的名字",
            M2ShareState.ConfigIni.ReadString("Setup", "CustomItemPropertyBindName3", ""));
        Assert.False(frm.btnOK.Enabled);
        Assert.False(frm.btnOK2.Enabled);
    }

    [Fact]
    public void Workflow_DistinctIndexValuesSurviveTwoFullRounds()
    {
        // 两轮"回填→改→写回"必须稳定；任何跨下标串写都会在第 2 轮暴露
        var frm = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            frm.edtShowName[i].Text = "X" + i;
        frm.btnOKClick();

        var frm2 = new TFrmCustomItemProperty();
        for (int i = 1; i <= 60; i++)
            Assert.Equal("X" + i, frm2.edtShowName[i].Text);

        for (int i = 1; i <= 60; i++)
            frm2.chk[i].Checked = true;
        frm2.btnOKClick();
        Assert.All(Enumerable.Range(1, 60), i => Assert.True(Checks[i]));
    }

    [Fact]
    public void UnrelatedSeamValues_DoNotLeakBetweenForms()
    {
        var a = new TFrmCustomItemProperty();
        a.chk[5].Checked = true;
        a.btnOKClick();
        var b = new TFrmCustomItemProperty();
        Assert.True(b.chk[5].Checked);                    // 通过全局回填
        Assert.False(b.chk[6].Checked);
    }
}
