// ============================================================================
// ConfigMerchant.pas（654 行）1:1 测试
//   DFM 对账（§37.3 计数取证）：控件 **50** / 事件绑定 **33** / 处理器方法 **40**
//   ★ 重点原始缺陷：RefListBoxMerChant 不清空列表、LoadScriptFile 少写 2 个开关、
//     ChangeScriptAllowAction 的插入顺序、EditPriceRateChange 不判空、
//     ButtonScriptSaveClick 不判 SelMerchant、btnSearchClick 不 Break
// ============================================================================

using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Npc;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsConfigMerchantTests : IDisposable
{
    private readonly string Dir;
    private readonly List<TMerchant> Merchants = new();
    private readonly List<int> LockIds = new();
    private readonly List<(string Text, string Caption, int Flags)> Messages = new();
    private TfrmConfigMerchant Form = null!;

    public Sweep9FormsConfigMerchantTests()
    {
        Dir = Path.Combine(Path.GetTempPath(), "p9merchant_" + Guid.NewGuid().ToString("N")) + "\\";
        Directory.CreateDirectory(Dir);
        M2Config.sEnvirDir = Dir;
        SweepSeam.ResetDefaults();
        Sweep9FormsMessageBoxSeam.Reset();
        Sweep9FormsMessageBoxSeam.UiEnabled = false;        // ★ 无头
        Sweep9FormsConfigMerchantGlobals.Reset();

        Form = new TfrmConfigMerchant();
        Form.MerchantListHandler = () => Merchants;
        Form.MerchantListLockR = id => LockIds.Add(id);
        Form.MerchantListUnLockR = () => LockIds.Add(-1);
        Form.MessageBoxHandler = (t, c, f) => { Messages.Add((t, c, f)); return M2Forms.IDYES; };
    }

    public void Dispose()
    {
        Form.Dispose();
        Sweep9FormsConfigMerchantGlobals.Reset();
        SweepSeam.ResetDefaults();
        Sweep9FormsMessageBoxSeam.Reset();
        try { Directory.Delete(Dir, true); } catch { /* best effort */ }
    }

    private TMerchant AddMerchant(string name, string mapName, int x = 0, int y = 0,
        string script = "", string mapDesc = "", sbyte flag = 0, ushort appr = 0)
    {
        var m = new TMerchant();
        m.m_sCharName = name;
        m.m_sMapName = mapName;
        m.m_nCurrX = x;
        m.m_nCurrY = y;
        m.m_sScript = script;
        m.m_nFlag = flag;
        m.m_wAppr = appr;
        m.m_PEnvir = new TEnvirnoment { sMapDesc = mapDesc };
        Merchants.Add(m);
        return m;
    }

    // ==================================================================
    // DFM 对账（计数取证）
    // ==================================================================

    [Fact]
    public void DfmReconcile_ControlCount_Is50()
    {
        Assert.Equal(50, Sweep9FormsReconcile.CountControlsExcludingForm(Form));
    }

    [Fact]
    public void DfmReconcile_EventBindingCount_Is33()
    {
        // DFM：根节点 OnCreate（1）+ 32 个控件绑定（其中 ButtonViewData 与
        // ButtonClearTempData **共用** ButtonClearTempDataClick） = 33
        Assert.Equal(33, Sweep9FormsReconcile.CountEventBindings(Form));
        Assert.Equal(32, Sweep9FormsReconcile.DfmControls(Form)
            .Sum(c => Sweep9FormsReconcile.CountEventBindingsOn(c)));
    }

    [Fact]
    public void DfmReconcile_MethodCount_Is39_OfWhich32AreDfmHandlers()
    {
        // 原文唯一过程 39 个（类声明 :63-94 的 32 个事件处理器 + 6 个 private 辅助 + Open）
        var original = new[]
        {
            "ModValue", "uModValue", "Open", "ClearMerchantData", "RefListBoxMerChant",
            "LoadScriptFile", "ChangeScriptAllowAction",
            "ListBoxMerChantClick", "FormCreate", "ButtonSaveClick", "ButtonClearTempDataClick",
            "CheckBoxDenyRefStatusClick", "EditXChange", "EditYChange", "EditShowNameChange",
            "EditImageIdxChange", "EditScriptNameChange", "EditMapNameChange", "ComboBoxDirChange",
            "CheckBoxOfCastleClick", "CheckBoxAutoMoveClick", "EditMoveTimeChange",
            "CheckBoxBuyClick", "CheckBoxSellClick", "CheckBoxGetbackClick", "CheckBoxStorageClick",
            "CheckBoxUpgradenowClick", "CheckBoxGetbackupgnowClick", "CheckBoxRepairClick",
            "CheckBoxS_repairClick", "CheckBoxMakedrugClick", "CheckBoxSendMsgClick",
            "EditPriceRateChange", "ButtonScriptSaveClick", "ButtonReLoadNpcClick",
            "MemoScriptChange", "chkCreateHeroClick", "chkBuHeroClick", "btnSearchClick",
        };
        Assert.Equal(39, original.Length);

        var declared = typeof(TfrmConfigMerchant)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Assert.Equal(original.OrderBy(n => n, StringComparer.Ordinal).ToArray(), declared);

        // 其中 32 个是 DFM 事件处理器（.pas:63-94 的类声明清单）
        Assert.Equal(32, original.Count(n =>
            n.EndsWith("Click", StringComparison.Ordinal) || n.EndsWith("Change", StringComparison.Ordinal)
            || n == "FormCreate"));
    }

    [Fact]
    public void DfmReconcile_FormProperties_MatchDfm()
    {
        Assert.Equal("frmConfigMerchant", Form.Name);
        Assert.Equal("交易NPC配置", Form.Text);
        Assert.Equal(366, Form.Left);
        Assert.Equal(256, Form.Top);
        Assert.Equal(818, Form.ClientSize.Width);
        Assert.Equal(374, Form.ClientSize.Height);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedSingle, Form.FormBorderStyle);
        Assert.Equal(System.Windows.Forms.FormStartPosition.CenterParent, Form.StartPosition);
        Assert.False(Form.MaximizeBox);
        Assert.True(Form.MinimizeBox);
    }

    [Fact]
    public void DfmReconcile_KeyControlGeometryAndCaptions()
    {
        Assert.Equal("NPC列表:", Form.GroupBox1.Text);
        Assert.Equal(new System.Drawing.Rectangle(8, 8, 401, 214),
            new System.Drawing.Rectangle(Form.GroupBox1.Left, Form.GroupBox1.Top, Form.GroupBox1.Width, Form.GroupBox1.Height));
        Assert.Equal("相关设置", Form.GroupBoxNPC.Text);
        Assert.False(Form.GroupBoxNPC.Enabled);              // DFM Enabled = False
        Assert.Equal("脚本编辑", Form.GroupBoxScript.Text);
        Assert.False(Form.GroupBoxScript.Enabled);           // DFM Enabled = False
        Assert.Equal("脚本参数", Form.GroupBox3.Text);

        // 两个"保存(&S)"按钮标题相同（DFM :227/:384）
        Assert.Equal("保存(&S)", Form.ButtonSave.Text);
        Assert.Equal("保存(&S)", Form.ButtonScriptSave.Text);
        Assert.Equal("加载(&L)", Form.ButtonReLoadNpc.Text);
        Assert.False(Form.ButtonReLoadNpc.Enabled);
        Assert.Equal("清除数据(&C)", Form.ButtonClearTempData.Text);
        Assert.Equal("查看数据(&V)", Form.ButtonViewData.Text);
        Assert.False(Form.ButtonViewData.Visible);           // DFM Visible = False
        Assert.Equal("开始搜索", Form.btnSearch.Text);
        Assert.Equal("下一个", Form.btnSearchNext.Text);

        // 复选框标题（含 CheckBoxGetback/Storage 的"存/取"顺序照抄 DFM）
        Assert.Equal("属于城堡", Form.CheckBoxOfCastle.Text);
        Assert.Equal("自动移动", Form.CheckBoxAutoMove.Text);
        Assert.Equal("买", Form.CheckBoxBuy.Text);
        Assert.Equal("卖", Form.CheckBoxSell.Text);
        Assert.Equal("取仓库", Form.CheckBoxStorage.Text);   // DFM :268（坐标 72,32）
        Assert.Equal("存仓库", Form.CheckBoxGetback.Text);   // DFM :277（坐标 72,16）
        Assert.Equal("合成物品", Form.CheckBoxMakedrug.Text);
        Assert.Equal("升级武器", Form.CheckBoxUpgradenow.Text);
        Assert.Equal("取回升级", Form.CheckBoxGetbackupgnow.Text);
        Assert.Equal("修理物品", Form.CheckBoxRepair.Text);
        Assert.Equal("特殊修理", Form.CheckBoxS_repair.Text);
        Assert.Equal("祝福语", Form.CheckBoxSendMsg.Text);
        Assert.Equal("创建英雄", Form.chkCreateHero.Text);
        Assert.Equal("副将英雄", Form.chkBuyHero.Text);
        Assert.Equal("刷新状态", Form.CheckBoxDenyRefStatus.Text);
    }

    [Fact]
    public void DfmReconcile_EditMapDesc_IsDisabledAndReadOnly()
    {
        Assert.False(Form.EditMapDesc.Enabled);     // DFM Enabled = False
        Assert.True(Form.EditMapDesc.ReadOnly);     // DFM ReadOnly = True
    }

    [Fact]
    public void DfmReconcile_SpinEdits_MatchDfmBounds()
    {
        // TSpinEditEx 的 MinValue/MaxValue/Value 逐条来自 DFM
        Assert.Equal(0, Form.EditImageIdx.Minimum);
        Assert.Equal(65535, Form.EditImageIdx.Maximum);
        Assert.Equal(0, Form.EditImageIdx.Value);
        Assert.Equal(1, Form.EditX.Minimum);
        Assert.Equal(1000, Form.EditX.Maximum);
        Assert.Equal(1, Form.EditX.Value);
        Assert.Equal(1, Form.EditY.Minimum);
        Assert.Equal(1000, Form.EditY.Maximum);
        Assert.Equal(1, Form.EditY.Value);
        Assert.Equal(0, Form.EditMoveTime.Minimum);
        Assert.Equal(65535, Form.EditMoveTime.Maximum);
        Assert.Equal(0, Form.EditMoveTime.Value);
        Assert.Equal(60, Form.EditPriceRate.Minimum);
        Assert.Equal(500, Form.EditPriceRate.Maximum);
        Assert.Equal(60, Form.EditPriceRate.Value);
        // ComboBoxDir：Style = csDropDownList
        Assert.Equal(System.Windows.Forms.ComboBoxStyle.DropDownList, Form.ComboBoxDir.DropDownStyle);
        // MemoScript：ScrollBars = ssBoth
        Assert.Equal(System.Windows.Forms.ScrollBars.Both, Form.MemoScript.ScrollBars);
    }

    [Fact]
    public void DfmReconcile_BtnSearchNext_HasNoBinding_OriginalBehaviour()
    {
        // ★ 否定性断言（计数取证）：`btnSearchNext` 在 DFM 里**没有** OnClick 绑定
        Assert.False(Sweep9FormsReconcile.IsBound(Form.btnSearchNext, "Click"));
        Assert.False(Sweep9FormsReconcile.IsBound(Form.edtSearch, "TextChanged"));
        Assert.True(Sweep9FormsReconcile.IsBound(Form.btnSearch, "Click"));
        Assert.True(Sweep9FormsReconcile.IsBound(Form.ButtonViewData, "Click"));   // 复用 ClearTempData 处理器
    }

    [Fact]
    public void DfmReconcile_ButtonViewData_SharesHandlerWithClearTempData()
    {
        // 两次绑定指向同一个处理器 ⇒ 行为一致（都会弹确认框）。
        // 绑定事实由计数取证（`IsBound`）；行为由直调同一处理器取证
        // （`ButtonViewData.Visible = False` ⇒ WinForms `PerformClick` 因 `CanSelect` 为假不会触发）。
        Assert.True(Sweep9FormsReconcile.IsBound(Form.ButtonViewData, "Click"));
        Messages.Clear();
        Form.ButtonClearTempDataClick(Form.ButtonViewData);
        Assert.Single(Messages);
        Assert.Equal("是否确认清除NPC临时数据？", Messages[0].Text);
    }

    // ==================================================================
    // ModValue / uModValue / FormCreate
    // ==================================================================

    [Fact]
    public void ModValueAndUModValue_ToggleBothButtons()
    {
        // 偏离 D-P9-05：WinForms `Control.Enabled` 与**父链**求与，而 VCL 只看控件自身。
        //   `ButtonScriptSave` 在 DFM `Enabled = False` 的 `GroupBoxScript` 内 ⇒ 必须先把
        //   父组启用（真实交互路径：选中 NPC 后 `ListBoxMerChantClick` 会启用它）。
        Assert.False(Form.ButtonScriptSave.Enabled);        // 父组未启用时读不到 true
        Form.GroupBoxScript.Enabled = true;

        Form.ModValue();
        Assert.True(Form.ButtonSave.Enabled);
        Assert.True(Form.ButtonScriptSave.Enabled);

        Form.uModValue();
        Assert.False(Form.ButtonSave.Enabled);
        Assert.False(Form.ButtonScriptSave.Enabled);
    }

    [Fact]
    public void EnabledReadback_FollowsParentChain_DeviationFromVcl()
    {
        // 偏离 D-P9-05 取证：把 `ButtonScriptSave.Enabled` 置 true（= 原文 ModValue 做的事），
        // 但父组仍为 DFM 的 Enabled = False ⇒ WinForms 读回 false（VCL 会读回 true）。
        Form.ButtonScriptSave.Enabled = true;
        Assert.False(Form.ButtonScriptSave.Enabled);
        Form.GroupBoxScript.Enabled = true;
        Assert.True(Form.ButtonScriptSave.Enabled);
    }

    [Fact]
    public void FormCreate_PopulatesComboBoxDirWithEightItems()
    {
        Form.FormCreate();
        Assert.Equal(8, Form.ComboBoxDir.Items.Count);
        Assert.Equal(new[] { "0", "1", "2", "3", "4", "5", "6", "7" },
            Form.ComboBoxDir.Items.Cast<object>().Select(o => o.ToString()!).ToArray());
    }

    // ==================================================================
    // Open / RefListBoxMerChant
    // ==================================================================

    [Fact]
    public void Open_ResetsStateAndLocksWithId13()
    {
        Form.CheckBoxDenyRefStatus.Checked = true;
        AddMerchant("NPC1", "0", 100, 200);

        Form.Open();

        Assert.False(Form.CheckBoxDenyRefStatus.Checked);
        Assert.Null(Form.SelMerchant);
        Assert.True(Form.boOpened);                         // 末尾置 True
        Assert.Equal(new[] { 13, -1 }, LockIds);            // RefListBoxMerChant 用 13
        Assert.Equal(1, Sweep9FormsMessageBoxSeam.ShowModalCount);
        Assert.False(Form.ButtonSave.Enabled);
    }

    [Fact]
    public void RefListBoxMerChant_FormatsItems_AndKeepsObjectCarriers()
    {
        var m = AddMerchant("张三", "3", 330, 331);
        Form.RefListBoxMerChant();

        Assert.Single(Form.ListBoxMerChant.Items);
        Assert.Equal("张三 - 3 (330:331)", Form.ListBoxMerChant.Items[0]);
        Assert.Same(m, Form.ItemObjects[0]);
    }

    [Fact]
    public void RefListBoxMerChant_SkipsTripleZeroEntry()
    {
        AddMerchant("隐藏", "0", 0, 0);
        Form.RefListBoxMerChant();
        Assert.Empty(Form.ListBoxMerChant.Items);
    }

    [Fact]
    public void RefListBoxMerChant_DoesNotClear_AccumulatesAcrossCalls_OriginalBehaviour()
    {
        // ★ 原文缺陷 1：`RefListBoxMerChant` **不清空** ListBoxMerChant ⇒ 反复调会累积。
        AddMerchant("NPC1", "3", 1, 1);
        Form.RefListBoxMerChant();
        Form.RefListBoxMerChant();
        Assert.Equal(2, Form.ListBoxMerChant.Items.Count);
        Assert.Equal(2, Form.ItemObjects.Count);
    }

    [Fact]
    public void Open_Twice_AccumulatesListItems_OriginalBehaviour()
    {
        AddMerchant("NPC1", "3", 1, 1);
        Form.Open();
        Form.Open();
        Assert.Equal(2, Form.ListBoxMerChant.Items.Count);
    }

    // ==================================================================
    // ListBoxMerChantClick（回填）
    // ==================================================================

    [Fact]
    public void ListBoxMerChantClick_BackfillsAllControls()
    {
        var m = AddMerchant("李四", "5", 100, 200, "merchant", "地图描述", 3, 42);
        m.m_dwMoveTime = 7;
        m.m_nPriceRate = 80;
        m.m_boCastle = true;
        m.m_boCanMove = true;
        m.m_boBuy = true;
        m.m_boSell = true;
        m.m_boGetback = true;
        m.m_boStorage = true;
        m.m_boUpgradenow = true;
        m.m_boGetBackupgnow = true;
        m.m_boRepair = true;
        m.m_boS_repair = true;
        m.m_boMakeDrug = true;
        m.m_boSendmsg = true;
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.ListBoxMerChant.SelectedIndex = 0;

        Form.ListBoxMerChantClick(Form.ListBoxMerChant);

        Assert.Same(m, Form.SelMerchant);
        Assert.Equal("merchant", Form.EditScriptName.Text);
        Assert.Equal("5", Form.EditMapName.Text);
        Assert.Equal("地图描述", Form.EditMapDesc.Text);
        Assert.Equal(100, (int)Form.EditX.Value);
        Assert.Equal(200, (int)Form.EditY.Value);
        Assert.Equal("李四", Form.EditShowName.Text);
        Assert.Equal(3, Form.ComboBoxDir.SelectedIndex);
        Assert.Equal(42, (int)Form.EditImageIdx.Value);
        Assert.True(Form.CheckBoxOfCastle.Checked);
        Assert.True(Form.CheckBoxAutoMove.Checked);
        Assert.Equal(7, (int)Form.EditMoveTime.Value);
        Assert.True(Form.CheckBoxBuy.Checked);
        Assert.True(Form.CheckBoxSendMsg.Checked);
        Assert.Equal(80, (int)Form.EditPriceRate.Value);
        Assert.True(Form.GroupBoxNPC.Enabled);
        Assert.True(Form.GroupBoxScript.Enabled);
        Assert.True(Form.boOpened);
        Assert.False(Form.ButtonReLoadNpc.Enabled);
    }

    [Fact]
    public void ListBoxMerChantClick_NoSelection_ReturnsEarly()
    {
        AddMerchant("NPC1", "3", 1, 1);
        Form.FormCreate();
        Form.ListBoxMerChant.SelectedIndex = -1;

        Form.ListBoxMerChantClick(Form.ListBoxMerChant);

        Assert.Null(Form.SelMerchant);
        Assert.False(Form.boOpened);            // :242 已置 False 后 Exit
    }

    [Fact]
    public void ListBoxMerChantClick_ComboBoxOutOfRange_IsSilentlyMinusOne()
    {
        // ★ §21.3 陷阱：原文 `ComboBoxDir.ItemIndex := m_nFlag`（ShortInt），
        //   超出 0..7 时 Delphi **静默置 -1**（WinForms 直译会抛）。
        var m = AddMerchant("NPC", "3", 1, 1);
        m.m_nFlag = 9;
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.ListBoxMerChant.SelectedIndex = 0;

        Form.ListBoxMerChantClick(Form.ListBoxMerChant);

        Assert.Equal(-1, Form.ComboBoxDir.SelectedIndex);
        Assert.Equal(9, m.m_nFlag);             // 回填不改 m_nFlag（只有 Change 处理器才写）
    }

    [Fact]
    public void ListBoxMerChantClick_BackfillDoesNotDirtyButtons()
    {
        // boOpened = False 期间的回填不置脏（各 Change 处理器早退）
        AddMerchant("NPC", "3", 5, 6);
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.ListBoxMerChant.SelectedIndex = 0;

        Form.ListBoxMerChantClick(Form.ListBoxMerChant);
        Form.uModValue();                        // 清掉再触发一次"用户级"改动
        Form.EditX.Value = 7;                    // 触发 EditXChange（boOpened = True）

        Assert.True(Form.ButtonSave.Enabled);
    }

    // ==================================================================
    // 各 Change/Click 处理器
    // ==================================================================

    [Fact]
    public void ChangeHandlers_RequireBoOpenedAndSelMerchant()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = false;
        Form.EditX.Value = 50;
        Form.EditY.Value = 60;
        Form.EditShowName.Text = "改";
        Form.EditScriptName.Text = "s";
        Form.EditMapName.Text = "m";
        Form.EditMoveTime.Value = 9;
        Form.EditImageIdx.Value = 11;
        Form.CheckBoxOfCastle.Checked = true;
        Form.CheckBoxAutoMove.Checked = true;
        Form.CheckBoxBuy.Checked = true;
        // boOpened = False ⇒ 一个字段都不许被改
        Assert.Equal(1, m.m_nCurrX);
        Assert.Equal(1, m.m_nCurrY);
        Assert.Equal("NPC", m.m_sCharName);
        Assert.Equal("", m.m_sScript);
        Assert.Equal("3", m.m_sMapName);
        Assert.Equal(0u, m.m_dwMoveTime);
        Assert.Equal(0, (int)m.m_wAppr);
        Assert.False(m.m_boCastle);
        Assert.False(m.m_boBuy);
    }

    [Fact]
    public void EditHandlers_TrimAndWriteBack()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;

        Form.EditShowName.Text = "  名字  ";    // TextChanged 直接触发处理器
        Form.EditScriptName.Text = "  sc  ";
        Form.EditMapName.Text = "  map  ";

        Assert.Equal("名字", m.m_sCharName);     // 原文 Trim
        Assert.Equal("sc", m.m_sScript);
        Assert.Equal("map", m.m_sMapName);
        Assert.True(Form.ButtonSave.Enabled);
    }

    [Fact]
    public void NumericHandlers_WriteBackWithNarrowing()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;

        Form.EditX.Value = 12;
        Form.EditY.Value = 34;
        Form.EditImageIdx.Value = 65;
        Form.EditMoveTime.Value = 77;

        Assert.Equal(12, m.m_nCurrX);
        Assert.Equal(34, m.m_nCurrY);
        Assert.Equal((ushort)65, m.m_wAppr);
        Assert.Equal(77u, m.m_dwMoveTime);
    }

    [Fact]
    public void ComboBoxDirChange_WritesSelectedIndex()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;
        Form.FormCreate();

        Form.ComboBoxDir.SelectedIndex = 4;

        Assert.Equal(4, m.m_nFlag);
    }

    [Fact]
    public void CheckBoxDenyRefStatusClick_DoesNotDirtyButtons_NorCheckBoOpened()
    {
        // ★ 原文缺陷 8：唯一不判 boOpened、也不调 ModValue 的处理器
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = false;
        Form.uModValue();

        Form.CheckBoxDenyRefStatus.Checked = true;

        Assert.True(m.m_boDenyRefStatus);            // 仍然写进去了
        Assert.False(Form.ButtonSave.Enabled);       // 但不置脏
        Assert.False(Form.ButtonScriptSave.Enabled);
    }

    [Fact]
    public void CheckBoxDenyRefStatusClick_NullMerchant_NoThrow()
    {
        Form.SelMerchant = null;
        Form.CheckBoxDenyRefStatus.Checked = true;   // 不应抛
        Assert.True(Form.CheckBoxDenyRefStatus.Checked);
    }

    [Fact]
    public void All12CheckBoxHandlers_WriteTheirOwnField_AndDirty()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;
        Form.FormCreate();
        Form.MemoScript.Lines.Add("(header)");
        Form.MemoScript.Lines.Add("%100");

        Form.CheckBoxOfCastle.Checked = true;
        Form.CheckBoxAutoMove.Checked = true;
        Form.CheckBoxBuy.Checked = true;
        Form.CheckBoxSell.Checked = true;
        Form.CheckBoxGetback.Checked = true;
        Form.CheckBoxStorage.Checked = true;
        Form.CheckBoxUpgradenow.Checked = true;
        Form.CheckBoxGetbackupgnow.Checked = true;
        Form.CheckBoxRepair.Checked = true;
        Form.CheckBoxS_repair.Checked = true;
        Form.CheckBoxMakedrug.Checked = true;
        Form.CheckBoxSendMsg.Checked = true;
        Form.chkCreateHero.Checked = true;
        Form.chkBuyHero.Checked = true;

        Assert.True(m.m_boCastle);
        Assert.True(m.m_boCanMove);
        Assert.True(m.m_boBuy);
        Assert.True(m.m_boSell);
        Assert.True(m.m_boGetback);
        Assert.True(m.m_boStorage);
        Assert.True(m.m_boUpgradenow);
        Assert.True(m.m_boGetBackupgnow);
        Assert.True(m.m_boRepair);
        Assert.True(m.m_boS_repair);
        Assert.True(m.m_boMakeDrug);
        Assert.True(m.m_boSendmsg);
        Assert.True(m.m_boCreateHeroName);
        Assert.True(m.m_boBuHero);
        Assert.True(Form.ButtonSave.Enabled);
    }

    [Fact]
    public void ChangeScriptAllowAction_Writes13FlagsWithHeroInMiddle()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        m.m_boBuy = true;
        m.m_boSell = true;
        m.m_boMakeDrug = true;
        m.m_boStorage = true;
        m.m_boGetback = true;
        m.m_boUpgradenow = true;
        m.m_boGetBackupgnow = true;
        m.m_boRepair = true;
        m.m_boS_repair = true;
        m.m_boSendmsg = true;
        m.m_boCreateHeroName = true;
        m.m_boBuHero = true;
        m.m_boArmRemoveStone = true;

        Form.SelMerchant = m;
        Form.MemoScript.Lines.Add("old");

        Form.ChangeScriptAllowAction();

        // ★ 原文缺陷 3：CreateHero/CreateDeputy 插在 SendMsg 与 ArmRemoveStone 之间
        Assert.Equal(
            "(@buy @sell @makedrug @storage @getback @upgradenow @getbackupgnow @repair @s_repair " +
            "@@sendmsg @@CreateHero @@BuHero @armremovestone )",
            Form.MemoScript.Lines[0]);
    }

    [Fact]
    public void ChangeScriptAllowAction_EmptyMemo_ReturnsEarly()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        m.m_boBuy = true;
        Form.SelMerchant = m;
        Form.MemoScript.Lines.Clear();

        Form.ChangeScriptAllowAction();          // 原文 :454 Lines.Count <= 0 ⇒ Exit

        Assert.Equal(0, Form.MemoScript.Lines.Count);
    }

    [Fact]
    public void ChangeScriptAllowAction_NullMerchant_ReturnsEarly()
    {
        Form.SelMerchant = null;
        Form.MemoScript.Lines.Add("x");
        Form.ChangeScriptAllowAction();
        Assert.Equal("x", Form.MemoScript.Lines[0]);
    }

    // ==================================================================
    // LoadScriptFile
    // ==================================================================

    [Fact]
    public void LoadScriptFile_NullMerchant_ReturnsEarly()
    {
        Form.SelMerchant = null;
        Form.LoadScriptFile();
        Assert.Empty(Form.MemoScript.Lines.ToArray());
    }

    [Fact]
    public void LoadScriptFile_Writes11FlagsHeader_WithoutHeroFlags_OriginalBehaviour()
    {
        // ★ 原文缺陷 2：`LoadScriptFile` 只写 11 个开关（**不含** CreateHero/CreateDeputy）
        var m = AddMerchant("NPC", "3", 1, 1);
        m.m_boBuy = true;
        m.m_boSell = true;
        m.m_boMakeDrug = true;
        m.m_boStorage = true;
        m.m_boGetback = true;
        m.m_boUpgradenow = true;
        m.m_boGetBackupgnow = true;
        m.m_boRepair = true;
        m.m_boS_repair = true;
        m.m_boSendmsg = true;
        m.m_boCreateHeroName = true;             // 打开窗口时这两项**不会**出现在头部行
        m.m_boBuHero = true;
        m.m_boArmRemoveStone = true;
        m.m_nPriceRate = 88;
        m.m_ItemTypeList.Add(1);
        m.m_ItemTypeList.Add(30);

        Form.SelMerchant = m;
        Form.LoadScriptFile();

        var lines = Form.MemoScript.Lines.ToArray();
        Assert.Equal(
            "(@buy @sell @makedrug @storage @getback @upgradenow @getbackupgnow @repair @s_repair " +
            "@@sendmsg @armremovestone )",
            lines[0]);
        Assert.DoesNotContain("CreateHero", lines[0]);
        Assert.DoesNotContain("BuHero", lines[0]);
        Assert.Equal("%88", lines[1]);
        Assert.Equal("+1", lines[2]);
        Assert.Equal("+30", lines[3]);
        Assert.True(Form.MemoScript.DfmVisibleStored);
    }

    [Fact]
    public void LoadScriptFile_ReadsScriptFile_WithNoHeaderGate()
    {
        // 脚本文件：';' 注释行丢弃；遇到 '[' 或 '#' 之后才开始收录
        var m = AddMerchant("NPC", "3", 1, 1, script: "sc");
        Directory.CreateDirectory(Dir + "Market_Def");
        File.WriteAllBytes(Dir + "Market_Def\\sc-3.txt", GXX.Core.EncodingInit.GBK.GetBytes(
            ";comment\r\nBEFORE\r\n[section]\r\nAFTER1\r\nAFTER2\r\n"));
        Form.SelMerchant = m;

        Form.LoadScriptFile();

        var lines = Form.MemoScript.Lines.ToArray();
        // 0 = 头部行、1 = '%100'、2.. = 文件内容（BEFORE 在 [section] 之前 ⇒ 被丢掉）
        Assert.True(lines.Length >= 5);
        Assert.Equal("[section]", lines[2]);
        Assert.Equal("AFTER1", lines[3]);
        Assert.Equal("AFTER2", lines[4]);
        Assert.DoesNotContain("BEFORE", lines);
    }

    [Fact]
    public void LoadScriptFile_HashHeader_AlsoTurnsOnNoHeader()
    {
        var m = AddMerchant("NPC", "3", 1, 1, script: "sc");
        Directory.CreateDirectory(Dir + "Market_Def");
        File.WriteAllBytes(Dir + "Market_Def\\sc-3.txt",
            GXX.Core.EncodingInit.GBK.GetBytes("#head\r\nX\r\n"));
        Form.SelMerchant = m;
        Form.LoadScriptFile();
        Assert.Contains("X", Form.MemoScript.Lines.ToArray());
    }

    [Fact]
    public void LoadScriptFile_MemoVisibleToggledBackToTrue()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.MemoScript.Visible = false;
        Form.LoadScriptFile();
        Assert.True(Form.MemoScript.DfmVisibleStored);       // :447 末尾置 True
    }

    // ==================================================================
    // EditPriceRateChange / MemoScriptChange
    // ==================================================================

    [Fact]
    public void EditPriceRateChange_UpdatesLine1()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;
        Form.MemoScript.Lines.Add("(header)");
        Form.MemoScript.Lines.Add("%100");

        Form.EditPriceRate.Value = 123;

        Assert.Equal(123, m.m_nPriceRate);
        Assert.Equal("%123", Form.MemoScript.Lines[1]);
    }

    [Fact]
    public void EditPriceRateChange_WithoutEnoughLines_Throws_OriginalBehaviour()
    {
        // ★ 原文缺陷 4：`:583` 直接写 `Lines[1]`，不判空 ⇒ 行数 < 2 时抛
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.boOpened = true;
        Form.MemoScript.Lines.Clear();

        Assert.ThrowsAny<ArgumentOutOfRangeException>(() => Form.EditPriceRateChange(Form.EditPriceRate));
    }

    [Fact]
    public void MemoScriptChange_MarksDirtyOnlyWhenOpened()
    {
        var m = AddMerchant("NPC", "3", 1, 1);
        Form.SelMerchant = m;
        Form.uModValue();

        Form.boOpened = false;
        Form.MemoScript.Lines.Add("x");
        Assert.False(Form.ButtonSave.Enabled);

        Form.boOpened = true;
        Form.MemoScript.Lines.Add("y");
        Assert.True(Form.ButtonSave.Enabled);
    }

    // ==================================================================
    // ButtonSaveClick
    // ==================================================================

    [Fact]
    public void ButtonSaveClick_WritesTabSeparatedMerchantFile()
    {
        var m = AddMerchant("张三", "3", 330, 331, script: "Market_Def\\npc\\a");
        m.m_nFlag = 2;
        m.m_wAppr = 17;
        m.m_boCastle = true;
        m.m_boCanMove = true;
        m.m_dwMoveTime = 42;
        AddMerchant("跳过", "0", 5, 5);                  // m_sMapName = '0' ⇒ Continue

        Form.ButtonSaveClick(Form.ButtonSave);

        Assert.Equal(new[] { 11, -1 }, LockIds);
        var text = GXX.Core.EncodingInit.GBK.GetString(File.ReadAllBytes(Dir + "Merchant.txt"));
        var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
        // ★ Script 里的 '\' 被替换成 '/'（原文 :184 StringReplace）
        Assert.Equal("Market_Def/npc/a\t3\t330\t331\t张三\t2\t17\t1\t1\t42", lines[0]);
    }

    [Fact]
    public void ButtonSaveClick_NoMerchants_WritesEmptyFile()
    {
        Form.ButtonSaveClick(Form.ButtonSave);
        Assert.True(File.Exists(Dir + "Merchant.txt"));
        Assert.Equal("", GXX.Core.EncodingInit.GBK.GetString(File.ReadAllBytes(Dir + "Merchant.txt")));
        Assert.False(Form.ButtonSave.Enabled);           // 末尾 uModValue
    }

    [Fact]
    public void ButtonSaveClick_UnwiredMerchantList_Throws()
    {
        Form.MerchantListHandler = null;
        var ex = Assert.Throws<InvalidOperationException>(() => Form.ButtonSaveClick(Form.ButtonSave));
        Assert.Contains("m_MerchantList", ex.Message);
    }

    // ==================================================================
    // ClearMerchantData / ButtonClearTempDataClick
    // ==================================================================

    [Fact]
    public void ButtonClearTempDataClick_Confirmed_ClearsEveryMerchant()
    {
        var a = AddMerchant("A", "3", 1, 1);
        var b = AddMerchant("B", "4", 2, 2);
        a.m_GoodsList.Add(new List<object>());
        b.m_GoodsList.Add(new List<object>());

        Form.ButtonClearTempDataClick(Form.ButtonClearTempData);

        Assert.Single(Messages);
        Assert.Equal(M2Forms.MB_YESNO + M2Forms.MB_ICONQUESTION, Messages[0].Flags);
        Assert.Equal("确认信息", Messages[0].Caption);
        Assert.Equal(new[] { 12, -1 }, LockIds);          // ClearMerchantData 用 12
        Assert.Empty(a.m_GoodsList);                      // TMerchant.ClearData 清货单/价格表
        Assert.Empty(b.m_GoodsList);
    }

    [Fact]
    public void ButtonClearTempDataClick_Declined_DoesNotClear()
    {
        var a = AddMerchant("A", "3", 1, 1);
        a.m_GoodsList.Add(new List<object>());
        Form.MessageBoxHandler = (t, c, f) => { Messages.Add((t, c, f)); return M2Forms.IDNO; };

        Form.ButtonClearTempDataClick(Form.ButtonClearTempData);

        Assert.Single(a.m_GoodsList);                     // 未确认 ⇒ 未被清
        Assert.Empty(LockIds);
    }

    // ==================================================================
    // ButtonScriptSaveClick / ButtonReLoadNpcClick
    // ==================================================================

    [Fact]
    public void ButtonScriptSaveClick_WritesMemoToMarketDefPath()
    {
        Directory.CreateDirectory(Dir + "Market_Def");
        var m = AddMerchant("NPC", "3", 1, 1, script: "sc");
        Form.SelMerchant = m;
        Form.GroupBoxScript.Enabled = true;               // 绕过 D-P9-05 的父链求与
        Form.MemoScript.Lines.Add("(header)");
        Form.MemoScript.Lines.Add("%100");

        Form.ButtonScriptSaveClick(Form.ButtonScriptSave);

        var path = Dir + "Market_Def\\sc-3.txt";
        Assert.True(File.Exists(path));
        var text = GXX.Core.EncodingInit.GBK.GetString(File.ReadAllBytes(path));
        Assert.Contains("(header)", text);
        Assert.False(Form.ButtonScriptSave.Enabled);      // uModValue
        Assert.True(Form.ButtonReLoadNpc.Enabled);        // :594
    }

    [Fact]
    public void ButtonScriptSaveClick_NullMerchant_Throws_OriginalBehaviour()
    {
        // ★ 原文缺陷 5：不判 SelMerchant = nil ⇒ 直接解引用
        Form.SelMerchant = null;
        Assert.Throws<NullReferenceException>(() => Form.ButtonScriptSaveClick(Form.ButtonScriptSave));
    }

    [Fact]
    public void ButtonReLoadNpcClick_ClearsScriptAndReloads()
    {
        var m = AddMerchant("NPC", "3", 1, 1, script: "sc");
        m.m_boBuy = true;
        m.m_boSell = true;
        m.m_ItemTypeList.Add(7);
        m.m_sScript = "sc";
        Form.SelMerchant = m;
        Form.GroupBoxScript.Enabled = true;               // 绕过 D-P9-05 的父链求与
        Form.ButtonReLoadNpc.Enabled = true;

        Form.ButtonReLoadNpcClick(Form.ButtonReLoadNpc);

        Assert.False(m.m_boBuy);                     // ClearScript（virtual/override ⇒ 虚分派）
        Assert.False(m.m_boSell);
        Assert.Empty(m.m_ItemTypeList);              // LoadNpcScript 先清类型表
        Assert.False(Form.ButtonReLoadNpc.Enabled);
    }

    [Fact]
    public void ButtonReLoadNpcClick_NullMerchant_ReturnsEarly()
    {
        Form.SelMerchant = null;
        Form.GroupBoxScript.Enabled = true;          // 绕过 D-P9-05 的父链求与
        Form.ButtonReLoadNpc.Enabled = true;
        Form.ButtonReLoadNpcClick(Form.ButtonReLoadNpc);
        Assert.True(Form.ButtonReLoadNpc.Enabled);   // 未走到 :603
    }

    // ==================================================================
    // btnSearchClick
    // ==================================================================

    [Fact]
    public void BtnSearchClick_FindsCaseInsensitiveMatch()
    {
        AddMerchant("AbcNpc", "3", 1, 1);
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.edtSearch.Text = "abc";

        Form.btnSearchClick(Form.btnSearch);

        Assert.Equal(0, Form.ListBoxMerChant.SelectedIndex);
        Assert.NotNull(Form.SelMerchant);            // OnClick 直调 ⇒ 已回填
        Assert.Equal("AbcNpc", Form.EditShowName.Text);
    }

    [Fact]
    public void BtnSearchClick_MultipleMatches_StopsAtLast_NoBreak()
    {
        // ★ 原文缺陷 6：命中后不 Break ⇒ 最终停在**最后一个**匹配
        AddMerchant("NPC1", "3", 1, 1);
        AddMerchant("NPC2", "4", 2, 2);
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.edtSearch.Text = "npc";

        Form.btnSearchClick(Form.btnSearch);

        Assert.Equal(1, Form.ListBoxMerChant.SelectedIndex);
        Assert.Equal("NPC2", Form.EditShowName.Text);
    }

    [Fact]
    public void BtnSearchClick_EmptyText_NoOp()
    {
        AddMerchant("NPC1", "3", 1, 1);
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.edtSearch.Text = "   ";                 // Trim 后为空

        Form.btnSearchClick(Form.btnSearch);

        Assert.Equal(-1, Form.ListBoxMerChant.SelectedIndex);
        Assert.Null(Form.SelMerchant);
    }

    [Fact]
    public void BtnSearchClick_NoMatch_NoSelectionChange()
    {
        AddMerchant("NPC1", "3", 1, 1);
        Form.FormCreate();
        Form.RefListBoxMerChant();
        Form.edtSearch.Text = "zzz";

        Form.btnSearchClick(Form.btnSearch);

        Assert.Equal(-1, Form.ListBoxMerChant.SelectedIndex);
    }

    // ==================================================================
    // 全局 / 补成员
    // ==================================================================

    [Fact]
    public void GlobalFormVariable_IsNullUntilWired()
    {
        Assert.Null(Sweep9FormsConfigMerchantGlobals.frmConfigMerchant);
        var f = new TfrmConfigMerchant();
        Sweep9FormsConfigMerchantGlobals.frmConfigMerchant = f;
        Assert.Same(f, Sweep9FormsConfigMerchantGlobals.frmConfigMerchant);
        f.Dispose();
    }

    [Fact]
    public void TCreatureGapMember_DenyRefStatus_ExistsAndDefaultsFalse()
    {
        // 本车道按 §19.6 用 partial 补齐的缺口成员（ObjBase.pas:340）
        var m = new TMerchant();
        Assert.False(m.m_boDenyRefStatus);
        m.m_boDenyRefStatus = true;
        Assert.True(m.m_boDenyRefStatus);
    }
}
