using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J64：uFrmCombatPowerSetting.pas 1:1（1473 行）测试。
/// 属性页 53 行 × 三职业、MaxHP/MaxMP 的 '‰' 后缀与 Hint、奇偶行底纹、
/// 编辑门控与 EndEdit、确定时的整表回写与 INI 落盘；
/// 变量页增删/搜索/校验三分支/清表重建；右键复制的文案与六向复制矩阵；重算在线人物。
/// </summary>
public sealed class CombatPowerSettingTests : IDisposable
{
    private readonly string _dir;

    public CombatPowerSettingTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j64_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        M2Config.ResetViewList2ConfigDefaults();
        CombatPowerUtils.CombatPowerVarMgr.Clear();
        for (int job = 0; job < 3; job++)
            Array.Clear(CombatPowerUtils.DefCombatPowerValue[job]);
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
    }

    public void Dispose()
    {
        CombatPowerUtils.CombatPowerVarMgr.Clear();
        M2Forms.MessageBoxHandler = null;
        M2Config.ResetViewList2ConfigDefaults();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    private CombatPowerSettingForm NewForm() => StaRunner.New(() => new CombatPowerSettingForm());

    // ================= 属性页 =================

    [Fact]
    public void FormCreate_Fills53RowsFromConfig()
    {
        CombatPowerUtils.DefCombatPowerValue[0][(int)TCombatPowerAttrib.cpaAC1] = 5;
        CombatPowerUtils.DefCombatPowerValue[1][(int)TCombatPowerAttrib.cpaAC1] = 6;
        CombatPowerUtils.DefCombatPowerValue[2][(int)TCombatPowerAttrib.cpaAC1] = 7;
        CombatPowerUtils.CombatPowerVarMgr.Add("D1", 10, 20, 30, "说明");

        var form = NewForm();
        try
        {
            Assert.Equal(53, form.DefNodeCount);
            Assert.Equal(1, form.VarNodeCount);
            Assert.Equal(0, form.ActivePageIndex);
            Assert.False(form.btnOK.Enabled);
            Assert.Equal(4, form.vstDefPower.Columns.Count);
            Assert.Equal("属性", form.vstDefPower.Columns[0]!.Text);
            Assert.Equal("战士", form.vstDefPower.Columns[1]!.Text);
            Assert.Equal("道士", form.vstDefPower.Columns[3]!.Text);
            Assert.Equal(6, form.vstVarPower.Columns.Count);
            Assert.Equal("序号", form.vstVarPower.Columns[0]!.Text);
            Assert.Equal("说明", form.vstVarPower.Columns[5]!.Text);

            int ac1 = (int)TCombatPowerAttrib.cpaAC1;
            Assert.Equal(5, form.GetDefNode(ac1).Value0);
            Assert.Equal(6, form.GetDefNode(ac1).Value1);
            Assert.Equal(7, form.GetDefNode(ac1).Value2);
            Assert.Equal(TCombatPowerAttrib.cpaAC1, form.GetDefNode(ac1).Attrib);
            Assert.Equal(0, form.GetDefNode(0).Attrib == TCombatPowerAttrib.cpaMaxHP ? 0 : 1);

            var v = form.GetVarNode(0);
            Assert.Equal("D1", v.VarName);
            Assert.Equal(10, v.Value0);
            Assert.Equal(30, v.Value2);
            Assert.Equal("说明", v.Desc);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void DefCellText_PermilleSuffixOnlyForHpMp_AndHint()
    {
        var hp = new CombatPowerSettingForm.TDefNodeData { Attrib = TCombatPowerAttrib.cpaMaxHP, Value0 = 1, Value1 = 2, Value2 = 3 };
        Assert.Equal("1‰", CombatPowerSettingForm.DefCellText(hp, 1));
        Assert.Equal("2‰", CombatPowerSettingForm.DefCellText(hp, 2));
        Assert.Equal("3‰", CombatPowerSettingForm.DefCellText(hp, 3));
        Assert.Equal("1000点MaxHP/MaxMP增加多少点战斗力", CombatPowerSettingForm.DefHint(hp));

        var mp = new CombatPowerSettingForm.TDefNodeData { Attrib = TCombatPowerAttrib.cpaMaxMP, Value0 = 4 };
        Assert.Equal("4‰", CombatPowerSettingForm.DefCellText(mp, 1));
        Assert.Equal("1000点MaxHP/MaxMP增加多少点战斗力", CombatPowerSettingForm.DefHint(mp));

        var ac = new CombatPowerSettingForm.TDefNodeData { Attrib = TCombatPowerAttrib.cpaAC1, Value0 = 9 };
        Assert.Equal("9", CombatPowerSettingForm.DefCellText(ac, 1));
        Assert.Equal("", CombatPowerSettingForm.DefHint(ac));

        // 列 0 为属性中文名
        Assert.Equal("MaxHP", CombatPowerSettingForm.DefCellText(hp, 0));
        Assert.Equal("防御下限", CombatPowerSettingForm.DefCellText(ac, 0));
    }

    [Fact]
    public void ItemEraseColor_AlternatesOnOddRows()
    {
        var def = System.Drawing.Color.White;
        Assert.Equal(def, CombatPowerSettingForm.ItemEraseColor(0, def));
        Assert.Equal(System.Drawing.Color.FromArgb(0xFB, 0xFB, 0xFB), CombatPowerSettingForm.ItemEraseColor(1, def));
        Assert.Equal(def, CombatPowerSettingForm.ItemEraseColor(2, def));
        Assert.Equal(System.Drawing.Color.FromArgb(0xFB, 0xFB, 0xFB), CombatPowerSettingForm.ItemEraseColor(53, def));
    }

    [Fact]
    public void DefEditingGate_AndPrepareEdit()
    {
        var form = NewForm();
        try
        {
            Assert.False(form.DefEditingAllowed(0, 0));      // 列 0 不可编辑
            Assert.True(form.DefEditingAllowed(0, 1));
            Assert.True(form.DefEditingAllowed(52, 3));
            Assert.False(form.DefEditingAllowed(-1, 1));
            Assert.False(form.DefEditingAllowed(53, 1));

            Assert.False(form.DefPrepareEdit(0));
            Assert.True(form.DefPrepareEdit(1));
            Assert.True(form.DefPrepareEdit(2));
            Assert.True(form.DefPrepareEdit(3));
            Assert.False(form.DefPrepareEdit(4));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void DefEndEdit_WritesBackAndMarksDefChanged()
    {
        var form = NewForm();
        try
        {
            int row = (int)TCombatPowerAttrib.cpaAC1;
            Assert.True(form.DefEndEdit(row, 1, 11));
            Assert.Equal(11, form.GetDefNode(row).Value0);
            Assert.True(form.FDefChanged);
            Assert.False(form.FVarChanged);
            Assert.True(form.btnOK.Enabled);
            Assert.Equal("11", form.vstDefPower.Items[row].SubItems[1].Text);

            Assert.False(form.DefEndEdit(row, 1, 11));       // 同值不变更

            Assert.True(form.DefEndEdit(row, 2, 12));
            Assert.True(form.DefEndEdit(row, 3, 13));
            Assert.Equal(12, form.GetDefNode(row).Value1);
            Assert.Equal(13, form.GetDefNode(row).Value2);

            Assert.False(form.DefEndEdit(9999, 1, 1));       // 越界
            Assert.False(form.DefEndEdit(row, 0, 1));        // 列 0 不写
            Assert.False(form.DefEndEdit(row, 9, 1));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void DefNodeClickAndKeyDown_RecordEditingRequest()
    {
        var form = NewForm();
        try
        {
            form.DefNodeClick(3, 2);
            Assert.NotNull(form.LastEditingRequest);
            Assert.True(form.LastEditingRequest!.Value.IsDef);
            Assert.Equal(3, form.LastEditingRequest!.Value.Index);
            Assert.Equal(2, form.LastEditingRequest!.Value.Column);

            form.LastEditingRequest = null;
            form.DefNodeClick(3, 0);                          // 列 0 不进入编辑
            Assert.Null(form.LastEditingRequest);

            form.DefKeyDown((int)System.Windows.Forms.Keys.Return, 5, 3);
            Assert.NotNull(form.LastEditingRequest);
            Assert.Equal(5, form.LastEditingRequest!.Value.Index);

            form.LastEditingRequest = null;
            form.DefKeyDown((int)System.Windows.Forms.Keys.A, 5, 3);   // 非回车
            Assert.Null(form.LastEditingRequest);

            form.DefChange(7, 1);
            Assert.Equal(7, form.LastEditingRequest!.Value.Index);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void BtnOk_WritesBackTableAndSavesIni()
    {
        var form = NewForm();
        try
        {
            int row = (int)TCombatPowerAttrib.cpaMaxHP;   // 索引 0
            form.DefEndEdit(row, 1, 7);
            form.DefEndEdit(row, 2, 8);
            form.DefEndEdit(row, 3, 9);

            Assert.True(form.BtnOkClick(out string err));
            Assert.Equal("", err);
            Assert.Equal(7, CombatPowerUtils.DefCombatPowerValue[0][row]);
            Assert.Equal(8, CombatPowerUtils.DefCombatPowerValue[1][row]);
            Assert.Equal(9, CombatPowerUtils.DefCombatPowerValue[2][row]);
            Assert.False(form.FDefChanged);
            Assert.False(form.btnOK.Enabled);

            string ini = File.ReadAllText(Path.Combine(_dir, "CombatPower.ini"), GXX.Core.EncodingInit.GBK);
            Assert.Contains("[DefaultAttrib]", ini);
            Assert.Contains("MaxHP_0=7", ini);
            Assert.Contains("MaxHP_1=8", ini);
            Assert.Contains("MaxHP_2=9", ini);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 变量页 =================

    [Fact]
    public void VarCellText_AndEditingGate()
    {
        var node = new CombatPowerSettingForm.TVarNodeData
        { VarName = "D1", Value0 = 1, Value1 = 2, Value2 = 3, Desc = "描述" };
        Assert.Equal("4", CombatPowerSettingForm.VarCellText(node, 4, 0));   // 列 0 = 节点序号
        Assert.Equal("D1", CombatPowerSettingForm.VarCellText(node, 4, 1));
        Assert.Equal("1", CombatPowerSettingForm.VarCellText(node, 4, 2));
        Assert.Equal("3", CombatPowerSettingForm.VarCellText(node, 4, 4));
        Assert.Equal("描述", CombatPowerSettingForm.VarCellText(node, 4, 5));

        CombatPowerUtils.CombatPowerVarMgr.Add("D1", 1, 1, 1, "");
        var form = NewForm();
        try
        {
            Assert.Equal(1, form.VarNodeCount);
            Assert.False(form.VarEditingAllowed(0, 0));       // 列 0 不可编辑
            Assert.True(form.VarEditingAllowed(0, 1));
            Assert.True(form.VarEditingAllowed(0, 5));

            Assert.False(form.VarPrepareEdit(0));
            Assert.True(form.VarPrepareEdit(1));
            Assert.True(form.VarPrepareEdit(5));
            Assert.False(form.VarPrepareEdit(6));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarEndEdit_AllColumns()
    {
        var mgr = CombatPowerUtils.CombatPowerVarMgr;
        mgr.Clear();
        mgr.Add("D1", 1, 2, 3, "旧说明");

        var form = NewForm();
        form.FormCreate();   // 重新装载变量行（Delphi 由 FormCreate 一次性装载）
        try
        {
            Assert.True(form.VarEndEdit(0, 1, 0, "N5"));
            Assert.Equal("N5", form.GetVarNode(0).VarName);
            Assert.True(form.FVarChanged);
            Assert.False(form.FDefChanged);
            Assert.Equal("N5", form.vstVarPower.Items[0].SubItems[1].Text);

            Assert.True(form.VarEndEdit(0, 2, 11));
            Assert.True(form.VarEndEdit(0, 3, 12));
            Assert.True(form.VarEndEdit(0, 4, 13));
            Assert.Equal(11, form.GetVarNode(0).Value0);
            Assert.Equal(12, form.GetVarNode(0).Value1);
            Assert.Equal(13, form.GetVarNode(0).Value2);

            Assert.True(form.VarEndEdit(0, 5, 0, "新说明"));
            Assert.Equal("新说明", form.GetVarNode(0).Desc);
            Assert.Equal("新说明", form.vstVarPower.Items[0].SubItems[5].Text);

            Assert.False(form.VarEndEdit(0, 2, 11));          // 同值
            Assert.False(form.VarEndEdit(9999, 2, 1));        // 越界
            Assert.False(form.VarEndEdit(0, 0, 1));           // 列 0 不写
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarKeyUp_InsertAndDeleteSelected()
    {
        var form = NewForm();
        try
        {
            form.VarKeyUp((int)System.Windows.Forms.Keys.Insert, true);
            form.VarKeyUp((int)System.Windows.Forms.Keys.Insert, true);
            Assert.Equal(2, form.VarNodeCount);
            Assert.True(form.FVarChanged);

            // 序号列随删除重排
            form.GetVarNode(1).VarName = "D2";
            form.DeleteSelectedVarNodes(new[] { 1 });
            Assert.Equal(1, form.VarNodeCount);
            Assert.Equal("0", form.vstVarPower.Items[0].SubItems[0].Text);

            // 无 Ctrl → 不动作
            int before = form.VarNodeCount;
            form.VarKeyUp((int)System.Windows.Forms.Keys.Insert, false);
            Assert.Equal(before, form.VarNodeCount);

            form.VarKeyUp((int)System.Windows.Forms.Keys.Delete, false, new[] { 0 });
            Assert.Equal(before, form.VarNodeCount);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarAdd_SingleDedupeAndBatch()
    {
        var form = NewForm();
        try
        {
            form.BtnAddVarClick("D1", false);
            Assert.Equal(1, form.VarNodeCount);
            Assert.Equal("D1", form.GetVarNode(0).VarName);

            // 重名 → 不新增，仅定位
            form.BtnAddVarClick("d1", false);
            Assert.Equal(1, form.VarNodeCount);
            Assert.Equal(0, form.FocusedVarIndex);

            // 批量：VarName + I
            form.BtnAddVarClick("M", true, 1, 3);
            Assert.Equal(4, form.VarNodeCount);
            Assert.Equal("M1", form.GetVarNode(1).VarName);
            Assert.Equal("M2", form.GetVarNode(2).VarName);
            Assert.Equal("M3", form.GetVarNode(3).VarName);

            // 批量去重
            form.BtnAddVarClick("M", true, 2, 2);
            Assert.Equal(4, form.VarNodeCount);

            // 搜索定位
            Assert.Equal(2, form.SearchVarNode("m2"));
            Assert.Equal(-1, form.SearchVarNode("X9"));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarAdd_CancelledDialogDoesNothing()
    {
        var form = NewForm();
        form.AddVarDialogHandler = () => (false, "D1", false, 0, 0);
        try
        {
            form.BtnAddVarClick();
            Assert.Equal(0, form.VarNodeCount);
            Assert.False(form.FVarChanged);
            Assert.False(form.btnOK.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarSearch_FiltersBySubstringCaseInsensitive()
    {
        var form = NewForm();
        try
        {
            form.BtnAddVarClick("D1", false);
            form.BtnAddVarClick("M22", false);
            form.BtnAddVarClick("U3", false);

            Assert.True(form.VarRowVisible(0));
            Assert.True(form.VarRowVisible(1));
            Assert.True(form.VarRowVisible(2));

            form.edtItemSearch.Text = "m2";
            Assert.False(form.VarRowVisible(0));
            Assert.True(form.VarRowVisible(1));
            Assert.False(form.VarRowVisible(2));

            form.edtItemSearch.Text = "";
            Assert.True(form.VarRowVisible(0));

            form.edtItemSearch.Text = "zz";
            Assert.False(form.VarRowVisible(0));
            Assert.False(form.VarRowVisible(1));
            Assert.False(form.VarRowVisible(2));
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void BtnOk_ValidatesThreeCasesAndAborts()
    {
        var form = NewForm();
        try
        {
            // 空名
            form.BtnAddVarClick("", false);
            Assert.False(form.BtnOkClick(out string err1));
            Assert.Equal("变量名不能为空", err1);
            Assert.Equal(0, form.FocusedVarIndex);
            Assert.True(form.btnOK.Enabled);          // Exit 未清标志

            // 不支持的变量名（首字符不在 D/M/N/U/J 且非 N$ 前缀）
            form.VarEndEdit(0, 1, 0, "X9");
            Assert.False(form.BtnOkClick(out string err2));
            Assert.Equal("不支持的变量名", err2);

            // 三值全 0
            form.VarEndEdit(0, 1, 0, "D9");
            Assert.False(form.BtnOkClick(out string err3));
            Assert.Equal("战斗力+不能全为0", err3);

            // 合法
            form.VarEndEdit(0, 2, 5);
            Assert.True(form.BtnOkClick(out string err4));
            Assert.Equal("", err4);
            Assert.Equal(1, CombatPowerUtils.CombatPowerVarMgr.Count);
            Assert.Equal("D9", CombatPowerUtils.CombatPowerVarMgr.GetItems(0).VarName);
            Assert.Equal(5, CombatPowerUtils.CombatPowerVarMgr.GetItems(0).Value0);
            Assert.True(File.Exists(Path.Combine(_dir, "CombatPower.ini")));
            Assert.False(form.btnOK.Enabled);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void VarFreeNode_ClearsDesc()
    {
        var form = NewForm();
        try
        {
            form.BtnAddVarClick("D1", false);
            form.VarEndEdit(0, 5, 0, "说明");
            Assert.Equal("说明", form.GetVarNode(0).Desc);
            form.VarFreeNode(0);
            Assert.Equal("", form.GetVarNode(0).Desc);
            form.VarFreeNode(99);   // 越界不崩
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    // ================= 开关 / 重算 =================

    [Fact]
    public void Toggles_WriteConfig()
    {
        var form = NewForm();
        var writes = new List<(string Key, bool Value)>();
        form.WriteBoolHandler = (k, v) => writes.Add((k, v));
        try
        {
            form.chkOpenCombatPowerCalc.Checked = true;
            Assert.True(M2Config.boOpenCombatPowerCalc);
            Assert.Equal(("OpenCombatPowerCalc", true), writes[0]);

            form.chkOpenCombatPowerVarCalc.Checked = true;
            Assert.True(M2Config.boOpenCombatPowerVarCalc);
            Assert.Equal(("OpenCombatPowerVarCalc", true), writes[1]);
            Assert.Equal(2, writes.Count);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void RecalcOnlineHumans_SkipsGhostDeathDummy_AndRecalcsHero()
    {
        M2Config.boOpenCombatPowerCalc = true;
        CombatPowerUtils.DefCombatPowerValue[0][(int)TCombatPowerAttrib.cpaMaxHP] = 1;

        var live = new TPlayObject();
        live.m_wAbil.MaxHP = 1000;           // → 1
        var ghost = new TPlayObject { m_boGhost = true };
        ghost.m_wAbil.MaxHP = 5000;
        var dead = new TPlayObject { m_boDeath = true };
        dead.m_wAbil.MaxHP = 5000;
        var dummy = new TPlayObject { m_boDummyObject = true };
        dummy.m_wAbil.MaxHP = 5000;
        var hero = new TPlayObject();
        hero.m_wAbil.MaxHP = 3000;           // → 3

        var form = NewForm();
        form.PlayObjectListHandler = () => new[] { live, ghost, dead, dummy };
        form.MyHeroHandler = p => ReferenceEquals(p, live) ? hero : null;
        try
        {
            form.BtnRecalHumanCombatPowerClick();
            Assert.Equal(1, live.m_nCombatPower);
            Assert.Equal(3, hero.m_nCombatPower);
            Assert.Equal(0, ghost.m_nCombatPower);
            Assert.Equal(0, dead.m_nCombatPower);
            Assert.Equal(0, dummy.m_nCombatPower);
        }
        finally { StaRunner.New(() => form.Dispose()); }

        // 无接缝（UserEngine.m_PlayObjectList 为 nil）
        var form2 = NewForm();
        try
        {
            form2.BtnRecalHumanCombatPowerClick();
            Assert.Equal(0, form2.DefNodeCount == 0 ? 1 : 0);
        }
        finally { StaRunner.New(() => form2.Dispose()); }
    }

    // ================= 右键复制 =================

    [Fact]
    public void PmCopyPopup_CaptionsPerPageAndColumn()
    {
        var form = NewForm();
        try
        {
            // 属性页
            form.HeaderClickIndex = 1;
            form.PmCopyPopup();
            Assert.True(form.mniCopy1.Available);
            Assert.Equal("从战士复制到法师为0的值", form.mniCopy1.Text);
            Assert.Equal("从战士复制到道士为0的值", form.mniCopy2.Text);

            form.HeaderClickIndex = 2;
            form.PmCopyPopup();
            Assert.Equal("从法师复制到战士为0的值", form.mniCopy1.Text);
            Assert.Equal("从法师复制到道士为0的值", form.mniCopy2.Text);

            form.HeaderClickIndex = 3;
            form.PmCopyPopup();
            Assert.Equal("从道士复制到战士为0的值", form.mniCopy1.Text);
            Assert.Equal("从道士复制到法师为0的值", form.mniCopy2.Text);

            form.HeaderClickIndex = 0;
            form.PmCopyPopup();
            Assert.False(form.mniCopy1.Available);
            Assert.False(form.mniCopy2.Available);

            // 变量页
            form.pgcMain.SelectedIndex = 1;
            form.HeaderClickIndex = 2;
            form.PmCopyPopup();
            Assert.True(form.mniCopy1.Available);
            Assert.Equal("从战士复制到法师为0的值", form.mniCopy1.Text);
            Assert.Equal("从战士复制到道士为0的值", form.mniCopy2.Text);

            form.HeaderClickIndex = 3;
            form.PmCopyPopup();
            Assert.Equal("从法师复制到战士为0的值", form.mniCopy1.Text);

            form.HeaderClickIndex = 4;
            form.PmCopyPopup();
            Assert.Equal("从道士复制到战士为0的值", form.mniCopy1.Text);
            Assert.Equal("从道士复制到法师为0的值", form.mniCopy2.Text);

            form.HeaderClickIndex = 1;
            form.PmCopyPopup();
            Assert.False(form.mniCopy1.Available);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void MniCopy_DefPage_SixDirections()
    {
        var form = NewForm();
        try
        {
            int a = (int)TCombatPowerAttrib.cpaAC1;      // 战士有值，法师/道士为 0
            int b = (int)TCombatPowerAttrib.cpaAC2;      // 法师有值，战士/道士为 0
            int c = (int)TCombatPowerAttrib.cpaMAC1;     // 道士有值，战士/法师为 0
            form.GetDefNode(a).Value0 = 100;
            form.GetDefNode(b).Value1 = 200;
            form.GetDefNode(c).Value2 = 300;

            // 战士 → 法师（mniCopy1）
            form.HeaderClickIndex = 1;
            Assert.True(form.MniCopy1Click());
            Assert.Equal(100, form.GetDefNode(a).Value1);
            Assert.True(form.FDefChanged);
            Assert.Equal("100", form.vstDefPower.Items[a].SubItems[2].Text);

            // 战士 → 道士（mniCopy2）
            Assert.True(form.MniCopy2Click());
            Assert.Equal(100, form.GetDefNode(a).Value2);

            // 法师 → 战士（mniCopy1）
            form.HeaderClickIndex = 2;
            Assert.True(form.MniCopy1Click());
            Assert.Equal(200, form.GetDefNode(b).Value0);

            // 法师 → 道士（mniCopy2）
            Assert.True(form.MniCopy2Click());
            Assert.Equal(200, form.GetDefNode(b).Value2);

            // 道士 → 战士（mniCopy1）
            form.HeaderClickIndex = 3;
            Assert.True(form.MniCopy1Click());
            Assert.Equal(300, form.GetDefNode(c).Value0);

            // 道士 → 法师（mniCopy2）
            Assert.True(form.MniCopy2Click());
            Assert.Equal(300, form.GetDefNode(c).Value1);

            // 再复制一次：目标已非 0 → 无变更
            Assert.False(form.MniCopy1Click());
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [Fact]
    public void MniCopy_VarPage_SixDirections()
    {
        var mgr = CombatPowerUtils.CombatPowerVarMgr;
        mgr.Clear();
        mgr.Add("D1", 10, 0, 0, "");     // 仅战士
        mgr.Add("D2", 0, 20, 0, "");     // 仅法师
        mgr.Add("D3", 0, 0, 30, "");     // 仅道士

        var form = NewForm();
        form.FormCreate();   // 重新装载变量行
        try
        {
            form.pgcMain.SelectedIndex = 1;

            form.HeaderClickIndex = 2;   // 战士 → 法师
            Assert.True(form.MniCopy1Click());
            Assert.Equal(10, form.GetVarNode(0).Value1);
            Assert.True(form.FVarChanged);

            Assert.True(form.MniCopy2Click());   // 战士 → 道士
            Assert.Equal(10, form.GetVarNode(0).Value2);

            form.HeaderClickIndex = 3;   // 法师 → 战士
            Assert.True(form.MniCopy1Click());
            Assert.Equal(20, form.GetVarNode(1).Value0);

            Assert.True(form.MniCopy2Click());   // 法师 → 道士
            Assert.Equal(20, form.GetVarNode(1).Value2);

            form.HeaderClickIndex = 4;   // 道士 → 战士
            Assert.True(form.MniCopy1Click());
            Assert.Equal(30, form.GetVarNode(2).Value0);

            Assert.True(form.MniCopy2Click());   // 道士 → 法师
            Assert.Equal(30, form.GetVarNode(2).Value1);

            Assert.False(form.MniCopy1Click());  // 无 0 值可填

            // 变量页复制只置 FVarChanged
            Assert.False(form.FDefChanged);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}
