using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GXX.LoginSrv;
using GXX.LoginSrv.Forms2;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>
/// p10-m2-misc 车道（LoginSrv 侧）：`Source/LoginSrv/GateSet.pas`（302 行）1:1 移植的用例集。
///
/// <para>
/// DFM 对账口径（§37.3 计数取证）：`Source/LoginSrv/GateSet.dfm`（**二进制 DFM**，6,155 B，
/// 按 §41.3-1 回读 `Source/**` 并手工解码；25 字节头 + 'TPF0' 流，解到 6155 = 文件长度）
/// ⇒ 控件 **49**、事件绑定 **6**。
/// </para>
/// <para>
/// 三条对账断言：① 控件名集合；② 声明数 == 实例化数 == 挂树数；③ 绑定数 == 托管 `+=` 数。
/// </para>
/// </summary>
public class P10bGateSetTests : IDisposable
{
    private readonly string _dir;

    public P10bGateSetTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx-p10c-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        GateSetConfigSeam.ResetForTests();
    }

    public void Dispose()
    {
        GateSetConfigSeam.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { }
    }

    /// <summary>DFM 的 49 个 `object` 节点名（按 DFM 出现顺序）。</summary>
    private static readonly string[] DfmControls =
    {
        "BtnOk", "BtnClose",
        "GroupBox1",
        "Label5", "Label6", "Label7", "Label8",
        "Label9", "Label10", "Label11", "Label12", "Label13",
        "Label14", "Label15", "Label16", "Label17", "Label18",
        "CkGate1", "EdGate1", "CkGate2", "EdGate2", "CkGate3", "EdGate3", "CkGate4", "EdGate4",
        "CkGate5", "EdGate5", "CkGate6", "EdGate6", "CkGate7", "EdGate7", "CkGate8", "EdGate8",
        "CkGate9", "EdGate9", "CkGate10", "EdGate10",
        "GroupBox2", "Label4", "Label3", "EdPublicAddr", "EdPrivateAddr",
        "GroupBox3", "Label2", "Label1", "BtnChangeTitle", "CbGateList", "CbServerList", "EdTitle",
    };

    private static TFrmGateSetting NewForm()
    {
        var f = new TFrmGateSetting();
        f.FormCreate(null);          // 模拟 DFM 的 OnCreate（测试不建窗口句柄，Load 不会自动触发）
        return f;
    }

    private static void SetServer(int index, string serverName, string title, string remote = "", string publicAddr = "")
    {
        GateSetConfigSeam.GateRoute[index].sServerName = serverName;
        GateSetConfigSeam.GateRoute[index].sTitle = title;
        GateSetConfigSeam.GateRoute[index].sRemoteAddr = remote;
        GateSetConfigSeam.GateRoute[index].sPublicAddr = publicAddr;
    }

    // =====================================================================================
    // DFM 对账
    // =====================================================================================

    [Fact]
    public void GateSet_DfmControlInventory_MatchesDeclaredFields()
    {
        var declared = P10bGateSetRecon.DeclaredControlFields(typeof(TFrmGateSetting))
            .Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

        Assert.Equal(49, DfmControls.Length);
        Assert.Equal(DfmControls.OrderBy(n => n, StringComparer.Ordinal).ToArray(), declared);
    }

    [Fact]
    public void GateSet_ControlDeclaration_Instantiation_Parenting_AllFortyNine()
    {
        using var form = new TFrmGateSetting();
        Assert.Equal(49, P10bGateSetRecon.DeclaredControlFields(typeof(TFrmGateSetting)).Count);
        Assert.Equal(49, P10bGateSetRecon.InstantiatedControlCount(form));
        Assert.Equal(49, P10bGateSetRecon.ParentedDeclaredControlCount(form));
    }

    [Fact]
    public void GateSet_DfmBindingCount_SixHandlers()
    {
        using var form = new TFrmGateSetting();
        Assert.True(6 == P10bGateSetRecon.CountBoundEventsDeep(form),
            "绑定数应为 6，实际 " + P10bGateSetRecon.CountBoundEventsDeep(form) +
            "；明细：" + P10bGateSetRecon.DescribeBoundEventsDeep(form));
    }

    [Fact]
    public void GateSet_KeyDfmProperties_PortFaithfully()
    {
        using var form = new TFrmGateSetting();
        Assert.Equal("网关路由配置", form.Text);
        Assert.Equal(new System.Drawing.Size(448, 348), form.ClientSize);
        Assert.Equal("保存(&S)", form.BtnOk.Text);
        // ★ 原文如此：关闭按钮的标题是「确定(&O)」而不是「取消」，ModalResult=1(mrOk)
        Assert.Equal("确定(&O)", form.BtnClose.Text);
        Assert.Equal(DialogResult.OK, form.BtnClose.DialogResult);
        Assert.Equal("修改(&M)", form.BtnChangeTitle.Text);
        Assert.Equal(FlatStyle.Flat, form.BtnChangeTitle.FlatStyle);     // TSpeedButton
        Assert.Equal("网关路由标识", form.GroupBox3.Text);
        Assert.Equal("登录网关信息", form.GroupBox2.Text);
        Assert.Equal("角色网关设置", form.GroupBox1.Text);
        Assert.Equal("服务器名:", form.Label1.Text);
        Assert.Equal("路由标识:", form.Label2.Text);
        Assert.Equal("内部地址:", form.Label3.Text);
        Assert.Equal("外部地址:", form.Label4.Text);
        Assert.Equal("角色网关:", form.Label5.Text);
        Assert.Equal("是否启用", form.Label6.Text);
        Assert.Equal("10", form.Label18.Text);
        Assert.Equal(System.Drawing.Color.Navy, form.EdTitle.ForeColor);  // Font.Color=clNavy
        Assert.Equal(ComboBoxStyle.DropDownList, form.CbGateList.DropDownStyle);
        Assert.Equal(ComboBoxStyle.DropDownList, form.CbServerList.DropDownStyle);
    }

    [Fact]
    public void GateSet_CheckBoxCaptions_CopiedVerbatimFromDfm_OriginalFlaw()
    {
        // ★ 原文如此：第 1 个复选框的 Caption 是 'CkGate1'（控件名本身），
        //   其余 9 个都是 'CheckBox1' —— 从设计器拖出来后**从未改过**，照抄不"美化"。
        using var form = new TFrmGateSetting();
        Assert.Equal("CkGate1", form.CkGate1.Text);
        Assert.Equal("CheckBox1", form.CkGate2.Text);
        Assert.Equal("CheckBox1", form.CkGate10.Text);
        Assert.True(form.CkGate1.Checked);
        Assert.True(form.CkGate10.Checked);
    }

    [Fact]
    public void GateSet_HintsAreWired()
    {
        using var form = new TFrmGateSetting();
        Assert.Equal("将网关路由设置保存到配置文件中。", form.HintProvider.GetToolTip(form.BtnOk));
        Assert.Equal("退出网关路由配置", form.HintProvider.GetToolTip(form.BtnClose));
        Assert.Equal("角色选择网关IP，及端口。", form.HintProvider.GetToolTip(form.EdGate10));
        Assert.Equal("登录网关对外服务的IP地址。", form.HintProvider.GetToolTip(form.EdPublicAddr));
        Assert.Equal("登录网关连接到登录服务器IP地址。", form.HintProvider.GetToolTip(form.EdPrivateAddr));
    }

    [Fact]
    public void GateSet_DfmInitialTextValues()
    {
        using var form = new TFrmGateSetting();
        Assert.Equal("210.121.143.202", form.EdGate1.Text);
        Assert.Equal("210.121.143.203", form.EdGate10.Text);
        Assert.Equal("210.121.143.202", form.EdPublicAddr.Text);
        Assert.Equal("5.5.2.1", form.EdPrivateAddr.Text);
    }

    [Fact]
    public void GateSet_FormCreate_FillsGateArrays()
    {
        using var form = NewForm();
        Assert.Same(form.EdGate1, form.EdGate[0]);
        Assert.Same(form.EdGate10, form.EdGate[9]);
        Assert.Same(form.CkGate1, form.CkGate[0]);
        Assert.Same(form.CkGate10, form.CkGate[9]);
    }

    [Fact]
    public void GateSet_FormDestroy_IsEmpty_OriginalFlaw()
    {
        using var form = NewForm();
        var ex = Record.Exception(() => form.FormDestroy(null));
        Assert.Null(ex);      // 原文方法体为空
    }

    // =====================================================================================
    // RefRouteList / CbServerListChange / CbGateListChange
    // =====================================================================================

    [Fact]
    public void RefRouteList_CollectsDistinctServerNamesAndSelectsFirst()
    {
        SetServer(0, "S1", "R0", "10.0.0.1", "1.1.1.1");
        SetServer(1, "S1", "R1", "10.0.0.2", "1.1.1.2");
        SetServer(2, "S2", "R2", "10.0.0.3", "1.1.1.3");
        GateSetConfigSeam.nRouteCount = 3;

        using var form = NewForm();
        form.RefRouteList();

        Assert.Equal(new[] { "S1", "S2" }, form.CbServerList.Items.Cast<object>().Select(o => o.ToString()).ToArray());
        Assert.Equal(0, form.CbServerList.SelectedIndex);
        // CbServerListChange 被转调 ⇒ CbGateList 只放 S1 的两条路由
        Assert.Equal(new[] { "R0", "R1" }, form.CbGateList.Items.Cast<object>().Select(o => o.ToString()).ToArray());
        Assert.Equal(0, form.CbGateList.SelectedIndex);
        // CbGateListChange 被转调 ⇒ 回填 R0 的地址与 10 组网关
        Assert.Equal("R0", form.EdTitle.Text);
        Assert.Equal("10.0.0.1", form.EdPrivateAddr.Text);
        Assert.Equal("1.1.1.1", form.EdPublicAddr.Text);
    }

    [Fact]
    public void RefRouteList_ZeroRouteCount_LeavesCombosUntouched()
    {
        GateSetConfigSeam.nRouteCount = 0;
        using var form = NewForm();
        form.RefRouteList();
        Assert.Empty(form.CbServerList.Items);
        Assert.Empty(form.CbGateList.Items);
    }

    [Fact]
    public void CbServerListChange_ServerWithoutRoutes_EmptyGateListDoesNotThrow()
    {
        // §21.3 已知陷阱：`CbGateList.ItemIndex := 0` 落在空列表上。
        // Delphi 走 CB_SETCURSEL 失败后静默保持 -1；WinForms 直接赋 SelectedIndex=0 会抛
        // ⇒ P10cComboBox.SetItemIndex 还原 Delphi 语义。本用例就是那条路径的哨兵。
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;

        using var form = NewForm();
        form.CbServerList.Items.Add("S2");
        P10cComboBox.SetItemIndex(form.CbServerList, 0);

        var ex = Record.Exception(() => form.CbServerListChange(null));
        Assert.Null(ex);
        Assert.Empty(form.CbGateList.Items);
        Assert.Equal(-1, form.CbGateList.SelectedIndex);
    }

    [Fact]
    public void CbServerListChange_NotSelected_ExitsEarly()
    {
        GateSetConfigSeam.nRouteCount = 1;
        SetServer(0, "S1", "R0");
        using var form = NewForm();
        form.CbGateList.Items.Add("keep");
        form.CbServerListChange(null);                       // ItemIndex = -1 ⇒ exit
        Assert.Single(form.CbGateList.Items);
    }

    [Fact]
    public void CbGateListChange_FillsAllTenGateSlots()
    {
        SetServer(0, "S1", "R0", "REMOTE", "PUBLIC");
        GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr = "10.0.0.1";
        GateSetConfigSeam.GateRoute[0].Gate[0].nPort = 5500;
        GateSetConfigSeam.GateRoute[0].Gate[0].boEnable = true;
        GateSetConfigSeam.GateRoute[0].Gate[3].sIPaddr = "10.0.0.4";
        GateSetConfigSeam.GateRoute[0].Gate[3].nPort = 5600;
        GateSetConfigSeam.GateRoute[0].Gate[3].boEnable = false;
        GateSetConfigSeam.nRouteCount = 1;

        using var form = NewForm();
        form.CbServerList.Items.Add("S1");
        P10cComboBox.SetItemIndex(form.CbServerList, 0);
        form.CbGateListChange(null);

        Assert.Equal("10.0.0.1:5500", form.EdGate[0].Text);
        Assert.True(form.CkGate[0].Checked);
        Assert.Equal("", form.EdGate[1].Text);
        Assert.Equal("10.0.0.4:5600", form.EdGate[3].Text);
        Assert.False(form.CkGate[3].Checked);
        for (int i = 4; i < 10; i++) Assert.Equal("", form.EdGate[i].Text);
    }

    [Fact]
    public void CbGateListChange_TitleNotFoundInRouteTable_Exits()
    {
        GateSetConfigSeam.nRouteCount = 1;
        SetServer(0, "S1", "R0");
        using var form = NewForm();
        // 显式构造选中状态，避免依赖 WinForms "首个 Item 自动选中" 的副作用触发处理器
        form.CbServerList.Items.Add("S1");
        P10cComboBox.SetItemIndex(form.CbServerList, 0);
        form.CbGateList.Items.Clear();
        form.CbGateList.Items.Add("NOPE");
        P10cComboBox.SetItemIndex(form.CbGateList, 0);
        string before = form.EdPrivateAddr.Text;      // 上一步的 CbServerListChange 链条已按 R0 回填（R0 的 sRemoteAddr 为空）

        form.CbGateListChange(null);

        // EdTitle 在"按标题找路由"之前就被赋值（原文如此），但地址**不再回填**（保持调用前的值）
        Assert.Equal("NOPE", form.EdTitle.Text);
        Assert.Equal(before, form.EdPrivateAddr.Text);
    }

    // =====================================================================================
    // BtnOkClick
    // =====================================================================================

    private static TFrmGateSetting FormReadyForOk(string title = "R0")
    {
        var form = NewForm();
        form.CbGateList.Items.Add(title);
        P10cComboBox.SetItemIndex(form.CbGateList, 0);
        for (int i = 0; i < 10; i++) form.EdGate[i].Text = "";
        return form;
    }

    [Fact]
    public void BtnOkClick_NoGateSelected_ExitsWithoutSaving()
    {
        bool saved = false;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;
        using var form = NewForm();
        form.BtnOkClick(null);
        Assert.False(saved);
    }

    [Fact]
    public void BtnOkClick_WritesRouteAndCallsSaveGateConfig()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        bool saved = false;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;

        using var form = FormReadyForOk();
        form.EdGate[0].Text = "10.0.0.1:5500";
        form.EdGate[1].Text = "10.0.0.2:5600";
        form.CkGate[1].Checked = false;
        form.EdPrivateAddr.Text = "REMOTE";
        form.EdPublicAddr.Text = "PUBLIC";

        form.BtnOkClick(null);

        Assert.True(saved);
        Assert.Equal("REMOTE", GateSetConfigSeam.GateRoute[0].sRemoteAddr);
        Assert.Equal("PUBLIC", GateSetConfigSeam.GateRoute[0].sPublicAddr);
        Assert.Equal("10.0.0.1", GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr);
        Assert.Equal(5500, GateSetConfigSeam.GateRoute[0].Gate[0].nPort);
        Assert.True(GateSetConfigSeam.GateRoute[0].Gate[0].boEnable);
        Assert.Equal("10.0.0.2", GateSetConfigSeam.GateRoute[0].Gate[1].sIPaddr);
        Assert.False(GateSetConfigSeam.GateRoute[0].Gate[1].boEnable);
        // 未被填写的槽位被**无条件清空**（原文如此）
        Assert.Equal("", GateSetConfigSeam.GateRoute[0].Gate[9].sIPaddr);
    }

    [Fact]
    public void BtnOkClick_EmptyFirstSlot_BeepsAndExits()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        bool beeped = false, saved = false;
        GateSetConfigSeam.Beep = () => beeped = true;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;

        using var form = FormReadyForOk();
        form.EdGate[0].Text = "";
        form.BtnOkClick(null);

        Assert.True(beeped);
        Assert.False(saved);
    }

    [Fact]
    public void BtnOkClick_PortMissing_BeepsAndExits()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        bool beeped = false;
        GateSetConfigSeam.Beep = () => beeped = true;
        GateSetConfigSeam.SaveGateConfig = () => { };

        using var form = FormReadyForOk();
        form.EdGate[0].Text = "10.0.0.1";              // 无 ':' ⇒ sPort = ''
        form.BtnOkClick(null);

        Assert.True(beeped);
    }

    [Fact]
    public void BtnOkClick_EmptyLaterSlots_PassValidation_OriginalFlaw()
    {
        // ★★ 原文缺陷：校验循环**不逐轮重置** sIPaddr/sPort —— 空槽跳过 GetValidStr3，
        //    于是沿用上一轮的取值，「第 0 槽有效、后面留空」会通过校验。
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        bool beeped = false, saved = false;
        GateSetConfigSeam.Beep = () => beeped = true;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;

        using var form = FormReadyForOk();
        form.EdGate[0].Text = "10.0.0.1:5500";
        for (int i = 1; i < 10; i++) form.EdGate[i].Text = "";      // 全部留空
        form.BtnOkClick(null);

        Assert.False(beeped);
        Assert.True(saved);
        Assert.Equal("10.0.0.1", GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr);
        for (int i = 1; i < 10; i++) Assert.Equal("", GateSetConfigSeam.GateRoute[0].Gate[i].sIPaddr);
    }

    [Fact]
    public void BtnOkClick_RouteIndex59_IsNeverMatched_OriginalFlaw()
    {
        // ★ 原文缺陷：路由查找循环写的是 `if nGateIdx >= 59 then break;`
        //   ⇒ 只检查 0..58，**59 号槽永远匹配不到** ⇒ 标题命中 59 号路由时静默不保存。
        for (int i = 0; i < 60; i++) SetServer(i, "S", "R" + i);
        GateSetConfigSeam.nRouteCount = 60;
        bool saved = false;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;

        using var form = FormReadyForOk("R59");
        form.EdGate[0].Text = "10.0.0.1:5500";
        form.BtnOkClick(null);

        Assert.False(saved);
        Assert.Equal("", GateSetConfigSeam.GateRoute[59].sRemoteAddr);
    }

    [Fact]
    public void BtnOkClick_RouteIndex58_IsMatched()
    {
        // 对照组：58 在循环覆盖范围内，正常保存。
        for (int i = 0; i < 60; i++) SetServer(i, "S", "R" + i);
        GateSetConfigSeam.nRouteCount = 60;
        bool saved = false;
        GateSetConfigSeam.SaveGateConfig = () => saved = true;

        using var form = FormReadyForOk("R58");
        form.EdGate[0].Text = "10.0.0.1:5500";
        form.BtnOkClick(null);

        Assert.True(saved);
        Assert.Equal("10.0.0.1", GateSetConfigSeam.GateRoute[58].Gate[0].sIPaddr);
    }

    // =====================================================================================
    // BtnChangeTitleClick
    // =====================================================================================

    [Fact]
    public void BtnChangeTitleClick_ReplacesSpacesWithUnderscore()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        using var form = NewForm();
        form.CbGateList.Items.Add("R0");
        P10cComboBox.SetItemIndex(form.CbGateList, 0);
        form.EdTitle.Text = "  a b  ";
        form.BtnChangeTitleClick(null);
        Assert.Equal("a_b", form.CbGateList.Items[0].ToString());
        Assert.Equal("a_b", GateSetConfigSeam.GateRoute[0].sTitle);
    }

    [Fact]
    public void BtnChangeTitleClick_WritesToWrongRoute_OriginalFlaw()
    {
        // ★★ 原文缺陷（本单元最有价值的一条）：写回用的是
        //   `Config.GateRoute[nTitleIdx]` —— nTitleIdx 是**过滤后的下拉框序号**，不是 GateRoute 下标。
        //   选第 2 个服务器（其唯一路由在 GateRoute[1]）时，改名会写到 GateRoute[0] 上。
        SetServer(0, "S1", "R0");
        SetServer(1, "S2", "R1");
        GateSetConfigSeam.nRouteCount = 2;

        using var form = NewForm();
        form.RefRouteList();                                  // CbServerList = [S1, S2]，选中 S1
        Assert.Equal("R0", form.EdTitle.Text);

        P10cComboBox.SetItemIndex(form.CbServerList, 1);      // 切到 S2
        form.CbServerListChange(null);
        Assert.Equal(new[] { "R1" }, form.CbGateList.Items.Cast<object>().Select(o => o.ToString()).ToArray());
        Assert.Equal("R1", form.EdTitle.Text);

        form.EdTitle.Text = "RENAMED";
        form.BtnChangeTitleClick(null);

        // 下拉框显示改对了（就地下拉项），但 GateRoute 改错了对象：
        Assert.Equal("RENAMED", form.CbGateList.Items[0].ToString());
        Assert.Equal("RENAMED", GateSetConfigSeam.GateRoute[0].sTitle);   // ← 被误改的是 0 号（S1 的路由）
        Assert.Equal("R1", GateSetConfigSeam.GateRoute[1].sTitle);        // ← 目标路由（S2 的）没变
    }

    [Fact]
    public void BtnChangeTitleClick_NoSelection_Exits()
    {
        using var form = NewForm();
        form.EdTitle.Text = "X";
        form.BtnChangeTitleClick(null);
        Assert.Equal("X", form.EdTitle.Text);
    }

    // =====================================================================================
    // Open
    // =====================================================================================

    [Fact]
    public void Open_ModalOk_ReturnsTrue()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        GateSetUi.ShowModal = f => DialogResult.OK;
        using var form = NewForm();
        Assert.True(form.Open());
    }

    [Fact]
    public void Open_ModalCancel_ReturnsFalse()
    {
        SetServer(0, "S1", "R0");
        GateSetConfigSeam.nRouteCount = 1;
        GateSetUi.ShowModal = f => DialogResult.Cancel;
        using var form = NewForm();
        Assert.False(form.Open());
    }

    [Fact]
    public void Open_CallsShowModalOnItself()
    {
        GateSetConfigSeam.nRouteCount = 0;
        Form? seen = null;
        GateSetUi.ShowModal = f => { seen = f; return DialogResult.Cancel; };
        using var form = NewForm();
        form.Open();
        Assert.Same(form, seen);
    }

    // =====================================================================================
    // SaveGateConfig（LSShare.pas:507-535 的 1:1 落地）
    // =====================================================================================

    private static string ReadGbk(string path) => Encoding.GetEncoding(936).GetString(File.ReadAllBytes(path));

    [Fact]
    public void SaveGateConfig_WritesHeaderAndPaddedRouteLines()
    {
        string file = Path.Combine(_dir, "!addrtable.txt");
        GateSetConfigSeam.AddrTableFile = file;
        SetServer(0, "Srv", "T1", "1.1.1.1", "2.2.2.2");
        GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr = "10.0.0.1";
        GateSetConfigSeam.GateRoute[0].Gate[0].nPort = 5500;
        GateSetConfigSeam.GateRoute[0].Gate[0].boEnable = true;
        GateSetConfigSeam.nRouteCount = 1;

        GateSetConfigSeam.DefaultSaveGateConfig();

        string[] lines = ReadGbk(file).Split(new[] { "\r\n" }, StringSplitOptions.None);
        Assert.Equal(";No space allowed", lines[0]);
        // GenSpaceString(s, n) 的长度 = max(n + 1, len(s) + 1)
        Assert.Equal(GS(";Server", 15) + GS("Title", 15) + GS("Remote", 17) + GS("Public", 17) + "Gate...", lines[1]);
        Assert.Equal(GS("Srv", 15) + GS("T1", 15) + GS("1.1.1.1", 17) + GS("2.2.2.2", 17) + GS("10.0.0.1:5500", 17), lines[2]);
    }

    [Fact]
    public void SaveGateConfig_DisabledGateGetsStarPrefix_OriginalFlaw()
    {
        string file = Path.Combine(_dir, "!addrtable.txt");
        GateSetConfigSeam.AddrTableFile = file;
        SetServer(0, "Srv", "T1");
        GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr = "10.0.0.1";
        GateSetConfigSeam.GateRoute[0].Gate[0].nPort = 5500;
        GateSetConfigSeam.GateRoute[0].Gate[0].boEnable = false;
        GateSetConfigSeam.nRouteCount = 1;

        GateSetConfigSeam.DefaultSaveGateConfig();

        string[] lines = ReadGbk(file).Split(new[] { "\r\n" }, StringSplitOptions.None);
        Assert.Contains(GS("*10.0.0.1:5500", 17), lines[2]);
    }

    [Fact]
    public void SaveGateConfig_EmptyGateAddress_TruncatesTheRest_OriginalFlaw()
    {
        // ★ 原文如此：网关列的 while 以 `sIPaddr = ''` 为**终止条件**（不是"跳过空槽"）
        //   ⇒ 中间出现空槽会截断其后所有网关。
        string file = Path.Combine(_dir, "!addrtable.txt");
        GateSetConfigSeam.AddrTableFile = file;
        SetServer(0, "Srv", "T1");
        GateSetConfigSeam.GateRoute[0].Gate[0].sIPaddr = "10.0.0.1";
        GateSetConfigSeam.GateRoute[0].Gate[0].nPort = 1;
        GateSetConfigSeam.GateRoute[0].Gate[1].sIPaddr = "";             // 空槽
        GateSetConfigSeam.GateRoute[0].Gate[2].sIPaddr = "10.0.0.3";     // 被截断
        GateSetConfigSeam.GateRoute[0].Gate[2].nPort = 3;
        GateSetConfigSeam.nRouteCount = 1;

        GateSetConfigSeam.DefaultSaveGateConfig();

        string line = ReadGbk(file).Split(new[] { "\r\n" }, StringSplitOptions.None)[2];
        // boEnable 未置真 ⇒ 该网关字段带 '*' 前缀（原文如此）
        Assert.True(line.Contains(GS("*10.0.0.1:1", 17)), "route line=[" + line + "]");
        Assert.DoesNotContain("10.0.0.3", line);
    }

    [Fact]
    public void SaveGateConfig_ZeroRoutes_WritesHeaderOnly()
    {
        string file = Path.Combine(_dir, "!addrtable.txt");
        GateSetConfigSeam.AddrTableFile = file;
        GateSetConfigSeam.nRouteCount = 0;
        GateSetConfigSeam.DefaultSaveGateConfig();
        string[] lines = ReadGbk(file).Split(new[] { "\r\n" }, StringSplitOptions.None);
        Assert.Equal(";No space allowed", lines[0]);
        Assert.StartsWith(";Server", lines[1]);
    }

    /// <summary>Delphi `GenSpaceString(sStr, nSpaceCOunt)`（LSShare.pas:554）的测试侧复刻。</summary>
    private static string GS(string s, int n)
    {
        string r = s + " ";
        for (int i = 1; i <= n - s.Length; i++) r += " ";
        return r;
    }

    // =====================================================================================
    // P10cComboBox：Delphi ItemIndex 语义
    // =====================================================================================

    [Fact]
    public void ComboBox_SetItemIndex_EmptyList_DoesNotThrow()
    {
        var combo = new ComboBox();
        var ex = Record.Exception(() => P10cComboBox.SetItemIndex(combo, 0));
        Assert.Null(ex);
        Assert.Equal(-1, P10cComboBox.GetItemIndex(combo));
    }

    [Fact]
    public void ComboBox_SetItemIndex_ClampsNegativeAndIgnoresOutOfRange()
    {
        var combo = new ComboBox();
        combo.Items.Add("a");
        P10cComboBox.SetItemIndex(combo, 5);              // 越界 ⇒ 不变
        Assert.Equal(-1, combo.SelectedIndex);
        P10cComboBox.SetItemIndex(combo, 0);
        Assert.Equal(0, combo.SelectedIndex);
        P10cComboBox.SetItemIndex(combo, -7);             // 钳到 -1
        Assert.Equal(-1, combo.SelectedIndex);
    }

    [Fact]
    public void SeamModels_MatchLsShareFieldLayout()
    {
        Assert.Equal(60, GateSetConfigSeam.GateRoute.Length);
        Assert.Equal(10, GateSetConfigSeam.GateRoute[0].Gate.Length);
        Assert.NotNull(GateSetConfigSeam.GateRoute[59].Gate[9]);       // 静态数组元素已初始化（Delphi 记录语义）
        Assert.Equal("", GateSetConfigSeam.GateRoute[0].sServerName);
        Assert.Equal(0, GateSetConfigSeam.GateRoute[0].nSelIdx);
        Assert.False(GateSetConfigSeam.GateRoute[0].Gate[0].boEnable);
    }

    [Fact]
    public void ResetForTests_RestoresGlobals()
    {
        GateSetConfigSeam.nRouteCount = 7;
        GateSetConfigSeam.GateRoute[0].sTitle = "X";
        GateSetConfigSeam.ResetForTests();
        Assert.Equal(0, GateSetConfigSeam.nRouteCount);
        Assert.Equal("", GateSetConfigSeam.GateRoute[0].sTitle);
    }
}
