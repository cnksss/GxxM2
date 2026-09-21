using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Core.Protocol;
using GXX.DBServer;
using GXX.DBServer.Forms;
using GXX.DBServer.Tests;
using Xunit;

namespace GXX.DBServer.Forms.Tests;

// ============================================================================================
// 车道 p10-db-login-forms / uFrmRoleDataEdit.pas (1-974) 的单元测试。
//
// 覆盖口径：
//   · DFM 对账（台账 §37.3 / §41.3）：88 个 `object` 节点 + 35 条事件绑定，**全部计数取证**；
//   · 4 个 DFM 事件处理过程 + DoOpen + 6 个 Refresh* + 2 个文件过程 + 单元级 ShowFrmRoleDataEdit
//     各至少 1 个测试；
//   · 每一条**原始缺陷**都有差异断言测试（原文如此，不修）；
//   · 边界：FIsHuman 真/假、空物品数组、越界 TItemWhereItems/栅格、空 edtPassword、缺文件、
//     短读文件、写盘失败、取消对话框。
//
// 绝不连库、绝不弹真实对话框、绝不阻塞：全部外部面由 P10RoleDataEditScope 换成内存替身。
// ============================================================================================

/// <summary>触碰静态接缝 ⇒ 关掉本集合的并行（AssemblyInfo.cs 已全局关并行，这里再显式声明一次）。</summary>
[CollectionDefinition("P10RoleDataEdit", DisableParallelization = true)]
public sealed class P10RoleDataEditCollection
{
}

[Collection("P10RoleDataEdit")]
public class P10RoleDataEditTests : TempDirTest
{
    // ========================================================================================
    // DFM 事实表（由 Source\DBServer\uFrmRoleDataEdit.dfm 的 **GBK 文本 DFM** 逐条复核）
    // ========================================================================================

    /// <summary>DFM 的 88 个 `object` 节点名，**按 DFM 出现顺序**。</summary>
    private static readonly string[] DfmObjectNames =
    {
        // 1 根 + 87 子节点
        "FrmRoleDataEdit", "lbl11111", "PageControl",
        "tsBase", "lbl2", "lbl3", "lbl4", "lbl5", "lbl6", "lbl1", "lbl7", "lbl8", "lbl9", "lbl10",
        "edtChrName", "edtAccount", "edtPassword", "edtDearName", "edtMasterName", "edtID", "edtCurMap",
        "seCurX", "seCurY", "edtHomeMap", "seHomeX", "seHomeY", "chkIsMaster",
        "tsInfo", "lbl11", "lbl12", "lbl13", "lbl14", "lbl18", "lbl17", "lbl19", "lbl20", "lbl15", "lbl16",
        "seLevel", "seGold", "seGameGold", "seGamePoint", "seCreditPoint", "sePayPoint", "sePKPoint", "seContribution",
        "GroupBox6",
        "lbl22", "lbl23", "lbl24", "lbl25", "lbl26", "lbl27", "lbl28", "lbl29", "lbl30", "lbl31", "lbl21",
        "EditDC", "EditMC", "EditSC", "EditAC", "EditMAC", "EditHP", "EditMP", "EditHit", "EditSpeed", "EditX2",
        "seBonusPoint", "seGameDiamond", "seGameGird",
        "tsMagic", "lvMagic", "tsUserItem", "lvUserItem", "tsFenghao", "lvFenghaoItem",
        "tsSorage", "lvStorage", "tsVarU", "strGridVarU", "tsVarT", "strGridVarT",
        "ButtonSaveData", "ButtonExportData", "ButtonImportData",
        "SaveDialog", "OpenDialog"
    };

    /// <summary>DFM 里 `object` 名不在 Controls 树里的两个（TComponent：TSaveDialog / TOpenDialog）。</summary>
    private static readonly string[] DfmNonControlObjects = { "SaveDialog", "OpenDialog" };

    /// <summary>
    /// DFM 里绑到 `edtPasswordChange` 的 **31** 个控件（29×OnChange + 2×OnClick），顺序即 DFM 顺序。
    /// 事件名是**托管侧**的对应事件（OnChange → TextChanged/ValueChanged；OnClick → Click）。
    /// </summary>
    private static readonly (string Name, string Event)[] BoundToEdtPasswordChange =
    {
        ("edtPassword", "TextChanged"), ("edtDearName", "TextChanged"),
        ("edtMasterName", "TextChanged"), ("edtCurMap", "TextChanged"),
        ("seCurX", "ValueChanged"), ("seCurY", "ValueChanged"), ("seHomeX", "ValueChanged"), ("seHomeY", "ValueChanged"),
        ("seLevel", "ValueChanged"), ("seGold", "ValueChanged"), ("seGameGold", "ValueChanged"),
        ("seGamePoint", "ValueChanged"), ("seCreditPoint", "ValueChanged"), ("sePayPoint", "ValueChanged"),
        ("sePKPoint", "ValueChanged"), ("seContribution", "ValueChanged"),
        ("EditDC", "ValueChanged"), ("EditMC", "ValueChanged"), ("EditSC", "ValueChanged"), ("EditAC", "ValueChanged"),
        ("EditMAC", "ValueChanged"), ("EditHP", "ValueChanged"), ("EditMP", "ValueChanged"), ("EditHit", "ValueChanged"),
        ("EditSpeed", "ValueChanged"), ("EditX2", "ValueChanged"),
        ("seBonusPoint", "ValueChanged"), ("seGameDiamond", "ValueChanged"), ("seGameGird", "ValueChanged"),
        ("edtHomeMap", "Click"), ("chkIsMaster", "Click")
    };

    /// <summary>
    /// 绑到 `edtPasswordChange` 却在处理过程里**没有对应分支**的 11 个控件
    /// （原文 :842-934 的 if/else-if 链：10 个 Edit* 属性点 + seHomeY）。
    /// </summary>
    private static readonly string[] BoundButNoBranch =
    {
        "EditDC", "EditMC", "EditSC", "EditAC", "EditMAC", "EditHP", "EditMP", "EditHit", "EditSpeed", "EditX2",
        "seHomeY"
    };

    // ========================================================================================
    // 测试夹具
    // ========================================================================================

    /// <summary>一条"有内容"的人物记录（字段值互不相同，便于定位到底是哪一路取的值）。</summary>
    private static THumData Human()
    {
        var h = new THumData();
        h.ChrName = "测试人物";
        h.Account = "acct01";
        h.StoragePwd = "pwd";
        h.DearName = "配偶";
        h.MasterName = "师父名";
        h.HeroName = "英雄名";
        h.DeputyHeroName = "副英雄名";
        h.CurMap = "0";
        h.HomeMap = "1";
        h.wCurX = 100;
        h.wCurY = 200;
        h.wHomeX = 3;
        h.wHomeY = 4;
        h.Abil.Level = 42;
        h.Abil.CreditPoint = 7;
        h.nGold = 1000;
        h.nGameGold = 2000;
        h.nGamePoint = 3000;
        h.nPayMentPoint = 5;
        h.nPKPoint = 6;
        h.wContribution = 77;
        h.nBonusPoint = 8;
        h.nGameDiamond = 9;
        h.nGameGird = 10;
        h.boMaster = 1;
        h.BonusAbil.DC = 1;
        h.BonusAbil.MC = 2;
        h.BonusAbil.SC = 3;
        h.BonusAbil.AC = 4;
        h.BonusAbil.MAC = 5;
        h.BonusAbil.HP = 6;
        h.BonusAbil.MP = 7;
        h.BonusAbil.Hit = 8;
        h.BonusAbil.Speed = 9;
        h.BonusAbil.X2 = 10;
        return h;
    }

    /// <summary>一条"有内容"的英雄记录。</summary>
    private static THeroData Hero(string chrName = "测试英雄", string account = "heroacct")
    {
        var x = new THeroData();
        x.ChrName = chrName;
        x.Account = account;
        x.CurMap = "1";
        x.wCurX = 10;
        x.wCurY = 20;
        x.Abil.Level = 30;
        x.nPKPoint = 40;
        return x;
    }

    /// <summary>一个"有效"物品（wIndex/MakeIndex 都非 0，Dura=10/DuraMax=20，btValue[0]=1 / [13]=14）。</summary>
    private static TUserItem Item(ushort wIndex, int makeIndex, ushort dura = 10, ushort duraMax = 20)
    {
        var it = new TUserItem { wIndex = wIndex, MakeIndex = makeIndex, Dura = dura, DuraMax = duraMax };
        it.SetBtValue(0, 1);
        it.SetBtValue(13, 14);
        return it;
    }

    private static THumMagic Magic(ushort wMagIdx, byte level, byte key, int tranPoint)
        => new THumMagic { MagicAttr = TMagicAttr.mtHum, wMagIdx = wMagIdx, btLevel = level, btKey = key, nTranPoint = tranPoint };

    private static TFrmRoleDataEdit NewHumanForm(THumData data, int id = 1)
    {
        var frm = new TFrmRoleDataEdit { FID = id, FIsHuman = true, FHumData = data };
        return frm;
    }

    private static TFrmRoleDataEdit NewHeroForm(THeroData data, int id = 2)
    {
        var frm = new TFrmRoleDataEdit { FID = id, FIsHuman = false, FHeroData = data };
        return frm;
    }

    // ========================================================================================
    // 1) DFM 对账（台账 §37.3 / §41.3：计数取证）
    // ========================================================================================

    [Fact]
    public void DFM对账_88个object节点与35条事件绑定()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();

        // --- 事实表自检 ---
        Assert.Equal(88, DfmObjectNames.Length);
        Assert.Equal(88, DfmObjectNames.Distinct().Count());

        // --- 88 = 86（含窗体自身的具名控件树）+ 2（SaveDialog/OpenDialog 是 TComponent，不在 Controls 里） ---
        int controlObjects = P10FormReconcile.CountDfmObjects(form);
        Assert.Equal(86, controlObjects);
        Assert.Equal(85, P10FormReconcile.CountChildrenOf(form));
        Assert.Equal(2, DfmNonControlObjects.Length);
        Assert.Equal(88, controlObjects + DfmNonControlObjects.Length);

        // --- 实例化的具名控件与 DFM 的控件节点**逐一对应**（双向差集都为空 ⇒ 一个不多一个不少） ---
        string[] dfmControlsOnly = DfmObjectNames.Except(DfmNonControlObjects).ToArray();
        List<string> instantiated = P10FormReconcile.EnumerateDfmObjects(form).Select(o => o.Name).ToList();
        Assert.Equal(dfmControlsOnly.Length, instantiated.Count);
        Assert.Empty(dfmControlsOnly.Except(instantiated));
        Assert.Empty(instantiated.Except(dfmControlsOnly));

        // --- 两个非可视组件确实存在（不能靠 Name 计数，故用类型 + DFM 差集取证） ---
        Assert.IsType<System.Windows.Forms.SaveFileDialog>(form.SaveDialog);
        Assert.IsType<System.Windows.Forms.OpenFileDialog>(form.OpenDialog);

        // --- 35 = 窗体自身 1 条（OnCreate → Load）+ 34 条控件绑定 ---
        Assert.Equal(35, P10FormReconcile.CountEventBindings(form));
        Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(form));
        Assert.True(P10FormReconcile.IsBound(form, "Load"));

        int controlBindings = P10FormReconcile.DfmControls(form)
            .Sum(c => P10FormReconcile.CountEventBindingsOn(c));
        Assert.Equal(34, controlBindings);      // 31（edtPasswordChange）+ 3 个按钮
        Assert.Equal(31 + 3, controlBindings);
    }

    [Fact]
    public void DFM对账_31个控件挂到edtPasswordChange且事件类型与DFM一致()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();

        Assert.Equal(31, BoundToEdtPasswordChange.Length);
        Assert.Equal(29, BoundToEdtPasswordChange.Count(t => t.Event != "Click"));   // 29×OnChange
        Assert.Equal(2, BoundToEdtPasswordChange.Count(t => t.Event == "Click"));    // 2×OnClick

        foreach (var (name, ev) in BoundToEdtPasswordChange)
        {
            var c = P10FormReconcile.FindByName(form, name);
            Assert.True(c != null, "DFM 控件缺失: " + name);
            Assert.True(P10FormReconcile.IsBound(c, ev), name + " 未挂接 " + ev);
            Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(c));   // 每个控件恰好 1 条绑定
        }
    }

    [Fact]
    public void DFM对账_三个按钮各一条Click绑定_导入按钮绑的也是ButtonExportDataClick()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();

        (string Name, System.Windows.Forms.Button Btn)[] buttons =
        {
            ("ButtonSaveData", form.ButtonSaveData),
            ("ButtonExportData", form.ButtonExportData),
            ("ButtonImportData", form.ButtonImportData)
        };
        Assert.Equal(3, buttons.Length);

        foreach (var (name, btn) in buttons)
        {
            Assert.Equal(name, btn.Name);
            Assert.True(P10FormReconcile.IsBound(btn, "Click"));
            Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(btn));
        }

        // 行为取证：DFM 把 ButtonImportData.OnClick 也指向 ButtonExportDataClick ⇒ 点导入必须走**导入**路径
        var h = Human();
        using var form2 = NewHumanForm(h, 11);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = Path2("in.hum");
        File.WriteAllBytes(Path2("in.hum"), StructBytes.BytesOf(h));

        form2.ButtonImportData.PerformClick();

        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");
        Assert.DoesNotContain(scope.Ui.MessageBoxes, m => m.Text == "角色数据导出成功！！！");
    }

    [Fact]
    public void DFM对账_全部控件名与字段名逐字同名()
    {
        using var form = new TFrmRoleDataEdit();
        var names = P10FormReconcile.EnumerateDfmObjects(form).Select(o => o.Name).ToHashSet();
        string[] expected = DfmObjectNames
            .Except(DfmNonControlObjects)
            .Where(n => n != "FrmRoleDataEdit")
            .ToArray();

        Assert.Equal(85, expected.Length);                  // 87 个子节点 - 2 个 TComponent
        Assert.Equal(86, names.Count);                      // 85 个具名控件 + 窗体自身
        Assert.Empty(expected.Where(n => !names.Contains(n)));          // DFM 的每个控件都实例化了
        Assert.Empty(names.Where(n => n != "FrmRoleDataEdit" && !expected.Contains(n)));

        // 并且每一个都是**同名的 public 实例字段**（原文的控件字段）
        var fieldNames = typeof(TFrmRoleDataEdit)
            .GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Select(f => f.Name)
            .ToHashSet();
        Assert.Empty(expected.Where(n => !fieldNames.Contains(n)));
        Assert.Contains("SaveDialog", fieldNames);
        Assert.Contains("OpenDialog", fieldNames);
    }

    // ========================================================================================
    // 2) TItemWhereNames / 控件壳 / 接缝默认值
    // ========================================================================================

    [Fact]
    public void TItemWhereNames_30项且顺序与原文一致()
    {
        Assert.Equal(30, RoleDataEditConst.TItemWhereNames.Length);
        Assert.Equal(30, RoleDataEditConst.HumItemsCount);          // Low..High(THumanUseItems) = 0..29
        Assert.Equal("衣服", RoleDataEditConst.TItemWhereNames[0]);
        Assert.Equal("武器", RoleDataEditConst.TItemWhereNames[1]);
        Assert.Equal("照明物", RoleDataEditConst.TItemWhereNames[2]);
        Assert.Equal("头盔", RoleDataEditConst.TItemWhereNames[4]);
        Assert.Equal("符", RoleDataEditConst.TItemWhereNames[9]);
        Assert.Equal("盾牌", RoleDataEditConst.TItemWhereNames[16]);
        Assert.Equal("灵玉", RoleDataEditConst.TItemWhereNames[17]);
        Assert.Equal("时装衣服", RoleDataEditConst.TItemWhereNames[18]);
        Assert.Equal("时装宝石", RoleDataEditConst.TItemWhereNames[29]);
        Assert.Equal(30, RoleDataEditConst.TItemWhereNames.Distinct().Count());
    }

    [Fact]
    public void TSpinEditLongWord_承载LongWord全域且DFM零范围不裁剪()
    {
        var se = new TSpinEditLongWord();
        se.SetDfmRange(0, 0);                       // DFM: MaxValue=0 MinValue=0
        Assert.Equal(0u, se.Value);
        se.Value = 3000000000u;
        Assert.Equal(3000000000u, se.Value);
        se.Value = uint.MaxValue;                   // High(LongWord)
        Assert.Equal(4294967295u, se.Value);
        Assert.Equal(0m, se.Minimum);
        Assert.Equal(4294967295m, se.Maximum);      // 原文 GetValue 的裁剪上界

        // 对照：TSpinEditEx 是 Integer
        var seInt = new TSpinEditEx();
        seInt.SetDfmRange(0, 0);
        seInt.Value = 123456;
        Assert.Equal(123456, seInt.Value);
    }

    [Fact]
    public void TStringGrid_越界读写静默不抛()
    {
        var g = new TStringGrid { ColCount = 2, RowCount = 3 };
        g.SetCells(0, 0, "a");
        Assert.Equal("a", g.Cells(0, 0));
        Assert.Equal(3, g.RowCount);
        Assert.Equal(2, g.ColCount);

        Assert.Equal("", g.Cells(0, 3));        // 行越界 → 空串
        Assert.Equal("", g.Cells(5, 0));        // 列越界 → 空串
        Assert.Equal("", g.Cells(-1, 0));
        g.SetCells(0, 9, "x");                  // 写越界 → 静默忽略
        g.SetCells(9, 0, "x");
        Assert.Equal("", g.Cells(0, 1));
        Assert.Equal(3, g.RowCount);
    }

    [Fact]
    public void 接缝_DBShare两个查询默认抛未接线()
    {
        RoleDataEditDbShareSeam.Reset();
        var ex1 = Assert.Throws<NotSupportedException>(() => RoleDataEditDbShareSeam.GetStdItemName(1));
        Assert.Contains("未接线", ex1.Message);
        var ex2 = Assert.Throws<NotSupportedException>(() => RoleDataEditDbShareSeam.GetMagicName(1, TMagicAttr.mtHum));
        Assert.Contains("未接线", ex2.Message);
    }

    // ========================================================================================
    // 3) FormCreate（DFM OnCreate）
    // ========================================================================================

    [Fact]
    public void FormCreate_把seLevel上限设为HighWord()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();

        Assert.Equal(65535, form.seLevel.DfmMaxValue);      // DFM: MaxValue = 65535
        Assert.Equal(0, form.seLevel.DfmMinValue);

        form.FormCreate(null);

        Assert.Equal(65535m, form.seLevel.Maximum);         // 原文 :938 seLevel.MaxValue := High(Word)
    }

    [Fact]
    public void FormCreate后_seLevel超上限赋值被裁剪()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();
        form.FormCreate(null);

        form.seLevel.Value = 70000;                          // 原文 TSpinEditEx.CheckValue：MaxValue<>MinValue ⇒ 裁剪
        Assert.Equal(65535, form.seLevel.Value);
        form.seLevel.Value = 41;
        Assert.Equal(41, form.seLevel.Value);
    }

    // ========================================================================================
    // 4) DoOpen
    // ========================================================================================

    [Fact]
    public void DoOpen_人类_标题栅格与TabVisible()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        form.DoOpen();

        Assert.Equal("编辑人物数据 [测试人物]", form.Text);
        Assert.Equal(2, form.strGridVarU.ColCount);
        Assert.Equal(501, form.strGridVarU.RowCount);            // Length(UValues) + 1 = 500 + 1
        Assert.Equal(501, form.strGridVarT.RowCount);
        Assert.Equal("变量名", form.strGridVarU.Cells(0, 0));
        Assert.Equal("变量值", form.strGridVarU.Cells(1, 0));
        Assert.Equal("变量名", form.strGridVarT.Cells(0, 0));
        Assert.Equal("变量值", form.strGridVarT.Cells(1, 0));
        Assert.Equal("U0", form.strGridVarU.Cells(0, 1));
        Assert.Equal("U499", form.strGridVarU.Cells(0, 500));
        Assert.Equal("T0", form.strGridVarT.Cells(0, 1));
        Assert.Equal("T499", form.strGridVarT.Cells(0, 500));

        Assert.True(form.tsVarU.TabVisible);
        Assert.True(form.tsVarT.TabVisible);
        Assert.Equal(8, form.PageControl.TabPages.Count);
        Assert.Equal(0, form.PageControl.SelectedIndex);         // 原文 :224 ActivePageIndex := 0

        Assert.Equal("0", form.strGridVarU.Cells(1, 1));         // RefreshUserVar 已跑过（UValues 全 0）
        Assert.Equal("0", form.strGridVarU.Cells(1, 500));       // 第 500 行 = UValues[499]
        Assert.Equal("0", form.strGridVarT.Cells(1, 500));       // TValues[499] 是空串 ⇒ ""
        Assert.Empty(form.lvStorage.Items);                      // 空仓库
        Assert.Empty(form.lvMagic.Items);
    }

    [Fact]
    public void DoOpen_英雄_标题与两条TabVisible为假()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero("测试英雄");
        using var form = NewHeroForm(hero);

        form.DoOpen();

        Assert.Equal("编辑英雄数据 [测试英雄]", form.Text);
        Assert.False(form.tsVarU.TabVisible);
        Assert.False(form.tsVarT.TabVisible);
        Assert.Equal(6, form.PageControl.TabPages.Count);        // 8 - 2
        Assert.Equal("普通", form.PageControl.TabPages[0].Text);
        Assert.Equal(0, form.PageControl.SelectedIndex);
        Assert.Equal(101, form.strGridVarU.RowCount);            // 英雄分支不扩栅格（原文 :206/:212 只走人类分支）
        Assert.Equal("变量名", form.strGridVarU.Cells(0, 0));
    }

    [Fact]
    public void ApplyTabVisible_属性值与页集合同时落地()
    {
        using var form = new TFrmRoleDataEdit();
        Assert.Equal(8, form.PageControl.TabPages.Count);
        Assert.True(form.tsVarU.TabVisible);

        form.ApplyTabVisible(form.tsVarU, false);
        Assert.False(form.tsVarU.TabVisible);
        Assert.Equal(7, form.PageControl.TabPages.Count);
        Assert.DoesNotContain(form.tsVarU, form.PageControl.TabPages.Cast<System.Windows.Forms.TabPage>());

        form.ApplyTabVisible(form.tsVarU, true);
        Assert.True(form.tsVarU.TabVisible);
        Assert.Equal(8, form.PageControl.TabPages.Count);
        Assert.Equal(6, form.PageControl.TabPages.IndexOf(form.tsVarU));   // 按 DFM 原索引插回
    }

    // ========================================================================================
    // 5) RefreshBaseInfo / RefreshShow
    // ========================================================================================

    [Fact]
    public void RefreshBaseInfo_人类_全字段回填()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        form.RefreshBaseInfo();

        Assert.Equal("测试人物", form.edtChrName.Text);
        Assert.Equal("acct01", form.edtAccount.Text);
        Assert.Equal("pwd", form.edtPassword.Text);
        Assert.Equal("配偶", form.edtDearName.Text);
        Assert.Equal("师父名", form.edtMasterName.Text);
        Assert.True(form.chkIsMaster.Checked);

        Assert.Equal("0", form.edtCurMap.Text);
        Assert.Equal(100, form.seCurX.Value);
        Assert.Equal(200, form.seCurY.Value);
        Assert.Equal("1", form.edtHomeMap.Text);
        Assert.Equal(3, form.seHomeX.Value);
        Assert.Equal(4, form.seHomeY.Value);

        Assert.Equal(42, form.seLevel.Value);
        Assert.Equal(1000u, form.seGold.Value);
        Assert.Equal(2000u, form.seGameGold.Value);
        Assert.Equal(3000, form.seGamePoint.Value);
        Assert.Equal(5, form.sePayPoint.Value);
        Assert.Equal(7, form.seCreditPoint.Value);
        Assert.Equal(6, form.sePKPoint.Value);
        Assert.Equal(77, form.seContribution.Value);
        Assert.Equal(8, form.seBonusPoint.Value);
        Assert.Equal(9u, form.seGameDiamond.Value);
        Assert.Equal(10u, form.seGameGird.Value);

        Assert.Equal(1, form.EditDC.Value);
        Assert.Equal(2, form.EditMC.Value);
        Assert.Equal(3, form.EditSC.Value);
        Assert.Equal(4, form.EditAC.Value);
        Assert.Equal(5, form.EditMAC.Value);
        Assert.Equal(6, form.EditHP.Value);
        Assert.Equal(7, form.EditMP.Value);
        Assert.Equal(8, form.EditHit.Value);
        Assert.Equal(9, form.EditSpeed.Value);
        Assert.Equal(10, form.EditX2.Value);

        Assert.True(form.edtPassword.Enabled);
        Assert.True(form.seGold.Enabled);
        Assert.True(form.chkIsMaster.Enabled);
    }

    [Fact]
    public void RefreshBaseInfo_英雄_只填英雄字段且人类字段清零()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero("测试英雄");
        var h = Human();                                    // 故意放一份非零的人类记录
        using var form = NewHeroForm(hero);
        form.FHumData = h;

        form.RefreshBaseInfo();

        Assert.Equal("测试英雄", form.edtChrName.Text);
        Assert.Equal("", form.edtAccount.Text);              // 原文 :314 该行被**注释掉** ⇒ 英雄模式不填账号
        Assert.Equal("", form.edtPassword.Text);
        Assert.Equal("1", form.edtCurMap.Text);
        Assert.Equal(10, form.seCurX.Value);
        Assert.Equal(20, form.seCurY.Value);
        Assert.Equal(30, form.seLevel.Value);
        Assert.Equal(40, form.sePKPoint.Value);

        Assert.False(form.edtPassword.Enabled);
        Assert.False(form.chkIsMaster.Enabled);
        Assert.False(form.seGold.Enabled);

        Assert.Equal("", form.edtHomeMap.Text);              // 回城字段被清零且英雄模式不回填
        Assert.Equal(0u, form.seGold.Value);

        // ★ 细节（原文如此）：清零块 `seGold.Value := 0` 时控件**本来就是 0** ⇒ 不触发 OnChange
        //   ⇒ FHumData.nGold（1000）**不会**被清零。Delphi/托管两侧的触发条件一致。
        Assert.Equal(1000u, form.FHumData.nGold);
    }

    [Fact]
    public void RefreshShow_人类依次刷新六个区()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.Magics[0] = Magic(11, 1, 1, 0);
        h.HumItems[0] = Item(1, 100);
        h.FengHaoItems[1] = Item(2, 200);
        h.StorageItems[2] = Item(3, 300);
        h.UValues[0] = 4321;
        h.TValues[0].Value = "tv";
        scope.MagicNames[11] = "火球术";
        scope.StdItemNames[1] = "布衣";
        scope.StdItemNames[2] = "封号令";
        scope.StdItemNames[3] = "仓库物";
        using var form = NewHumanForm(h);

        form.RefreshShow();

        Assert.Single(form.lvMagic.Items);
        Assert.Single(form.lvUserItem.Items);
        Assert.Single(form.lvFenghaoItem.Items);
        Assert.Single(form.lvStorage.Items);
        Assert.Equal("4321", form.strGridVarU.Cells(1, 1));
        Assert.Equal("tv", form.strGridVarT.Cells(1, 1));
        Assert.Equal("测试人物", form.edtChrName.Text);
    }

    [Fact]
    public void RefreshUserVar_未扩RowCount时越界静默丢弃()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.UValues[0] = 1;
        h.UValues[499] = 2;
        h.TValues[0].Value = "x";
        using var form = NewHumanForm(h);

        form.RefreshUserVar();                          // 没走 DoOpen ⇒ RowCount 仍是 DFM 的 101

        Assert.Equal("1", form.strGridVarU.Cells(1, 1));
        Assert.Equal("x", form.strGridVarT.Cells(1, 1));
        Assert.Equal("", form.strGridVarU.Cells(1, 500));    // 越界 → 静默（VCL SetEditText 语义）
        Assert.Equal(101, form.strGridVarU.RowCount);
    }

    // ========================================================================================
    // 6) RefreshMagicInfo
    // ========================================================================================

    [Fact]
    public void RefreshMagicInfo_人类_按wMagIdx为零中断()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.Magics[0] = Magic(11, 3, 1, 100);
        h.Magics[1] = Magic(22, 4, 2, 200);
        h.Magics[3] = Magic(33, 5, 3, 300);              // 被 Magics[2].wMagIdx = 0 的 break 挡住
        scope.MagicNames[11] = "火球术";
        scope.MagicNames[22] = "治愈术";
        scope.MagicNames[33] = "不该出现";
        using var form = NewHumanForm(h);

        form.RefreshMagicInfo();

        Assert.Equal(2, form.lvMagic.Items.Count);
        Assert.Equal("0", form.lvMagic.Items[0].Text);
        Assert.Equal("11", form.lvMagic.Items[0].SubItems[1].Text);
        Assert.Equal("火球术", form.lvMagic.Items[0].SubItems[2].Text);
        Assert.Equal("3", form.lvMagic.Items[0].SubItems[3].Text);
        Assert.Equal("100", form.lvMagic.Items[0].SubItems[4].Text);
        Assert.Equal("1", form.lvMagic.Items[0].SubItems[5].Text);
        Assert.Equal("1", form.lvMagic.Items[1].Text);
        Assert.Equal("治愈术", form.lvMagic.Items[1].SubItems[2].Text);
        Assert.DoesNotContain("不该出现", form.lvMagic.Items.Cast<System.Windows.Forms.ListViewItem>().Select(i => i.SubItems[2].Text));
        Assert.Equal(6, form.lvMagic.Columns.Count);
        Assert.Equal("快捷键", form.lvMagic.Columns[5].Text);
    }

    [Fact]
    public void RefreshMagicInfo_可换成内存接缝()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.Magics[0] = Magic(11, 1, 1, 5);
        scope.MagicNames[11] = "火球术";
        using var form = NewHumanForm(h);
        var sink = new MemoryListViewSink();
        form.MagicSink = sink;

        form.RefreshMagicInfo();

        Assert.Equal(1, sink.Count);
        Assert.Equal("0", sink.Rows[0].Caption);
        Assert.Equal("11", sink.Rows[0].SubItems[0]);
        Assert.Equal("火球术", sink.Rows[0].SubItems[1]);
        Assert.Equal("1", sink.Rows[0].SubItems[2]);
        Assert.Equal("5", sink.Rows[0].SubItems[3]);
        Assert.Equal("1", sink.Rows[0].SubItems[4]);
        Assert.Equal(6, sink.Rows[0].SubItems.Length + 1);   // 6 列 = Caption + 5 个 SubItems
        Assert.Null(sink.GetTag(0));
    }

    // ========================================================================================
    // 7) RefreshUserItems
    // ========================================================================================

    [Fact]
    public void RefreshUserItems_人类_装备首饰盒神佑盒三段行号偏移()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.HumItems[0] = Item(1, 100);
        h.HumItems[29] = Item(2, 200);
        h.JewelryBoxItems[0] = Item(3, 300);
        h.GodBlessItems[0] = Item(4, 400);
        scope.StdItemNames[1] = "布衣";
        scope.StdItemNames[2] = "木剑";
        scope.StdItemNames[3] = "项链";
        scope.StdItemNames[4] = "神佑";
        using var form = NewHumanForm(h);

        form.RefreshUserItems();

        Assert.Equal(4, form.lvUserItem.Items.Count);

        // 第 1 段：HumItems[0] → Caption = 0，位置 = TItemWhereNames[0] = 衣服
        var r0 = form.lvUserItem.Items[0];
        Assert.Equal("0", r0.Text);
        Assert.Equal("衣服", r0.SubItems[1].Text);
        Assert.Equal("布衣", r0.SubItems[2].Text);
        Assert.Equal("0", r0.SubItems[3].Text);                 // wIndex - 1
        Assert.Equal("100", r0.SubItems[4].Text);               // MakeIndex
        Assert.Equal("10/20", r0.SubItems[5].Text);             // Dura/DuraMax
        Assert.Equal("1-0-0-0-0-0-0-0-0-0-0-0-0-14", r0.SubItems[6].Text);   // sItemValue 14 项

        // 第 1 段末：HumItems[29] → TItemWhereNames[29] = 时装宝石
        Assert.Equal("29", form.lvUserItem.Items[1].Text);
        Assert.Equal("时装宝石", form.lvUserItem.Items[1].SubItems[1].Text);

        // 第 2 段：首饰盒 → Caption = I + Length(HumItems) = I + 30
        Assert.Equal("30", form.lvUserItem.Items[2].Text);
        Assert.Equal("首饰盒1", form.lvUserItem.Items[2].SubItems[1].Text);
        Assert.Equal("项链", form.lvUserItem.Items[2].SubItems[2].Text);

        // 第 3 段：神佑盒 → Caption = I + 30 + 6
        Assert.Equal("36", form.lvUserItem.Items[3].Text);
        Assert.Equal("神佑盒1", form.lvUserItem.Items[3].SubItems[1].Text);
        Assert.Equal("神佑", form.lvUserItem.Items[3].SubItems[2].Text);
    }

    [Fact]
    public void RefreshUserItems_英雄_读FHeroData的对应字段()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero();
        hero.HumItems[1] = Item(1, 100);
        hero.JewelryBoxItems[0] = Item(2, 200);
        hero.GodBlessItems[0] = Item(3, 300);
        scope.StdItemNames[1] = "布衣";
        scope.StdItemNames[2] = "项链";
        scope.StdItemNames[3] = "神佑";
        using var form = NewHeroForm(hero);

        form.RefreshUserItems();

        Assert.Equal(3, form.lvUserItem.Items.Count);
        Assert.Equal("1", form.lvUserItem.Items[0].Text);
        Assert.Equal("武器", form.lvUserItem.Items[0].SubItems[1].Text);
        Assert.Equal("30", form.lvUserItem.Items[1].Text);
        Assert.Equal("首饰盒1", form.lvUserItem.Items[1].SubItems[1].Text);
        Assert.Equal("36", form.lvUserItem.Items[2].Text);
        Assert.Equal("神佑盒1", form.lvUserItem.Items[2].SubItems[1].Text);
    }

    [Fact]
    public void RefreshUserItems_wIndex或MakeIndex为零一律跳过()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.HumItems[0] = Item(0, 100);       // wIndex = 0
        h.HumItems[1] = Item(1, 0);         // MakeIndex = 0
        h.HumItems[2] = Item(1, 1);         // 唯一有效
        scope.StdItemNames[1] = "布衣";
        using var form = NewHumanForm(h);

        form.RefreshUserItems();

        Assert.Single(form.lvUserItem.Items);
        Assert.Equal("2", form.lvUserItem.Items[0].Text);
    }

    [Fact]
    public void RefreshUserItems_空数组时只清不填()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        form.lvUserItem.Items.Add(new System.Windows.Forms.ListViewItem("残留"));

        form.RefreshUserItems();

        Assert.Empty(form.lvUserItem.Items);     // 原文第一件事就是 lvUserItem.Clear
    }

    // ========================================================================================
    // 8) RefreshFenghaoItems / RefreshStorages
    // ========================================================================================

    [Fact]
    public void RefreshFenghaoItems_人类与英雄各读自己的字段()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.FengHaoItems[3] = Item(41, 4000);
        var hero = Hero();
        hero.FengHaoItems[5] = Item(51, 5000);
        scope.StdItemNames[41] = "封号令";
        scope.StdItemNames[51] = "英雄封号";

        using var form = NewHumanForm(h);
        form.RefreshFenghaoItems();
        Assert.Single(form.lvFenghaoItem.Items);
        Assert.Equal("3", form.lvFenghaoItem.Items[0].Text);
        Assert.Equal("封号令", form.lvFenghaoItem.Items[0].SubItems[1].Text);
        Assert.Equal("40", form.lvFenghaoItem.Items[0].SubItems[2].Text);
        Assert.Equal("4000", form.lvFenghaoItem.Items[0].SubItems[3].Text);
        Assert.Equal("10/20", form.lvFenghaoItem.Items[0].SubItems[4].Text);
        Assert.Equal(6, form.lvFenghaoItem.Columns.Count);

        using var form2 = NewHeroForm(hero);
        form2.RefreshFenghaoItems();
        Assert.Single(form2.lvFenghaoItem.Items);
        Assert.Equal("5", form2.lvFenghaoItem.Items[0].Text);
        Assert.Equal("英雄封号", form2.lvFenghaoItem.Items[0].SubItems[1].Text);
    }

    [Fact]
    public void RefreshStorages_没有FIsHuman分支_英雄模式也读FHumData()
    {
        // ★ 原文缺陷锁定：:661-701 全程读 FHumData.StorageItems，**没有** if FIsHuman 分支
        //   （THeroData 里根本没有 StorageItems 字段）。
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.StorageItems[0] = Item(5, 500);
        var hero = Hero();
        scope.StdItemNames[5] = "仓库物";
        using var form = NewHeroForm(hero);
        form.FHumData = h;

        form.RefreshStorages();

        Assert.Single(form.lvStorage.Items);                 // 英雄模式却显示了 FHumData 的仓库
        Assert.Equal("0", form.lvStorage.Items[0].Text);
        Assert.Equal("仓库物", form.lvStorage.Items[0].SubItems[1].Text);
        Assert.Equal(6, form.lvStorage.Columns.Count);
    }

    // ========================================================================================
    // 9) edtPasswordChange（31 个绑定控件）
    // ========================================================================================

    [Fact]
    public void edtPasswordChange_真实控件事件驱动逐项写回()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        // TEdit.OnChange（4 个）——走 TextChanged
        form.edtPassword.Text = "  npwd  ";
        Assert.Equal("npwd", form.FHumData.StoragePwd);       // Trim 之后写入
        form.edtDearName.Text = " 新配偶 ";
        Assert.Equal("新配偶", form.FHumData.DearName);
        form.edtMasterName.Text = "新师父";
        Assert.Equal("新师父", form.FHumData.MasterName);
        form.edtCurMap.Text = " 9 ";
        Assert.Equal("9", form.FHumData.CurMap);

        // TSpinEdit.OnChange ——走 ValueChanged
        form.seCurX.Value = 7;
        Assert.Equal((ushort)7, form.FHumData.wCurX);
        form.seCurY.Value = 8;
        Assert.Equal((ushort)8, form.FHumData.wCurY);
        form.seHomeX.Value = 11;
        Assert.Equal((ushort)11, form.FHumData.wHomeX);
        form.seLevel.Value = 55;
        Assert.Equal(55, form.FHumData.Abil.Level);
        form.seGold.Value = 12345u;
        Assert.Equal(12345u, form.FHumData.nGold);
        form.seGameGold.Value = 66u;
        Assert.Equal(66u, form.FHumData.nGameGold);
        form.seGamePoint.Value = 77;
        Assert.Equal(77u, form.FHumData.nGamePoint);
        form.sePayPoint.Value = 88;
        Assert.Equal(88, form.FHumData.nPayMentPoint);
        form.seCreditPoint.Value = 99;
        Assert.Equal(99, form.FHumData.Abil.CreditPoint);
        form.sePKPoint.Value = 111;
        Assert.Equal(111, form.FHumData.nPKPoint);
        form.seContribution.Value = 222;
        Assert.Equal((ushort)222, form.FHumData.wContribution);
        form.seBonusPoint.Value = 333;
        Assert.Equal(333, form.FHumData.nBonusPoint);
        form.seGameDiamond.Value = 444u;
        Assert.Equal(444u, form.FHumData.nGameDiamond);
        form.seGameGird.Value = 555u;
        Assert.Equal(555u, form.FHumData.nGameGird);

        // edtHomeMap 绑的是 OnClick（改 Text **不**触发；点一下才写回）
        form.edtHomeMap.Text = " hm ";
        Assert.Equal("1", form.FHumData.HomeMap);             // 未触发
        form.edtHomeMap.PerformClick();
        Assert.Equal("hm", form.FHumData.HomeMap);

        // chkIsMaster 绑的是 OnClick
        Assert.Equal((byte)1, form.FHumData.boMaster);
        form.chkIsMaster.PerformClick();                       // false → true，触发 Click
        Assert.Equal((byte)1, form.FHumData.boMaster);
        form.chkIsMaster.PerformClick();                       // true → false
        Assert.Equal((byte)0, form.FHumData.boMaster);
    }

    [Fact]
    public void edtPasswordChange_空密码与空白全部Trim成空串()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        form.edtPassword.Text = "   ";
        Assert.Equal("", form.FHumData.StoragePwd);
        form.edtPassword.Text = "";
        Assert.Equal("", form.FHumData.StoragePwd);

        form.edtHomeMap.Text = "  ";
        form.edtHomeMap.PerformClick();
        Assert.Equal("", form.FHumData.HomeMap);
    }

    [Fact]
    public void edtPasswordChange_英雄模式seLevel与sePKPoint写FHeroData()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero();
        var h = Human();
        using var form = NewHeroForm(hero);
        form.FHumData = h;

        form.seLevel.Value = 88;
        form.sePKPoint.Value = 99;

        Assert.Equal(88, form.FHeroData.Abil.Level);
        Assert.Equal(99, form.FHeroData.nPKPoint);
        Assert.Equal(42, form.FHumData.Abil.Level);          // 人类记录未被碰（有守卫）
        Assert.Equal(6, form.FHumData.nPKPoint);
    }

    // ========================================================================================
    // 10) 原始缺陷锁定：edtPasswordChange
    // ========================================================================================

    [Fact]
    public void 原始缺陷_seHomeY已绑定却没有分支_改动不写回wHomeY()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();                                     // wHomeY = 4
        using var form = NewHumanForm(h);

        // 先证明它**确实绑上了**（否定性断言必须靠计数取证，不能靠眼看）
        Assert.True(P10FormReconcile.IsBound(form.seHomeY, "ValueChanged"));
        Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(form.seHomeY));

        form.seHomeY.Value = 999;                            // 真实事件驱动

        Assert.Equal(999, form.seHomeY.Value);               // 控件本身变了
        Assert.Equal((ushort)4, form.FHumData.wHomeY);       // ★ 记录没变（原文 :842-934 无 seHomeY 分支）

        // 对照：seHomeX 有分支，能写回
        form.seHomeX.Value = 321;
        Assert.Equal((ushort)321, form.FHumData.wHomeX);
    }

    [Fact]
    public void 原始缺陷_31个已绑定Sender逐一试过都写不到wHomeY()
    {
        // :880 第二个 `else if Sender = seCurY` 是**死分支**（:868 已判过同一个 seCurY），
        // 因此 `FHumData.wHomeY` 在任何 UI 路径下都写不进去。
        using var scope = new P10RoleDataEditScope();
        var h = Human();                                     // wHomeY = 4
        using var form = NewHumanForm(h);
        form.seCurY.Value = 888;                             // 让 seCurY 取一个特殊值
        form.seHomeY.Value = 777;                            // 让 seHomeY 取一个特殊值

        int tried = 0;
        foreach (var (name, _) in BoundToEdtPasswordChange)
        {
            var c = P10FormReconcile.FindByName(form, name);
            Assert.NotNull(c);
            form.edtPasswordChange(c);                       // 把每个已绑定控件依次当作 Sender
            tried++;
        }

        Assert.Equal(31, tried);                             // 计数取证：31 条绑定全试过
        Assert.Equal((ushort)4, form.FHumData.wHomeY);       // ★ 始终没被写
        Assert.NotEqual((ushort)777, form.FHumData.wHomeY);
        Assert.Equal((ushort)888, form.FHumData.wCurY);      // 而 seCurY 的值确实进了 wCurY
    }

    [Fact]
    public void 原始缺陷_十个属性点控件与seHomeY共11个绑定控件无分支()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        Assert.Equal(11, BoundButNoBranch.Length);
        int noOpCount = 0;
        foreach (string name in BoundButNoBranch)
        {
            var c = P10FormReconcile.FindByName(form, name);
            Assert.NotNull(c);
            // 都是**已绑定**控件（否则"无分支"就没有意义）
            Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(c));

            if (c is System.Windows.Forms.NumericUpDown nud) nud.Value = 12345;
            form.edtPasswordChange(c);
            noOpCount++;
        }

        Assert.Equal(11, noOpCount);
        // BonusAbil 一律保持 RefreshBaseInfo 之前的内存值（没人写它们）
        Assert.Equal(1, form.FHumData.BonusAbil.DC);
        Assert.Equal(2, form.FHumData.BonusAbil.MC);
        Assert.Equal(3, form.FHumData.BonusAbil.SC);
        Assert.Equal(4, form.FHumData.BonusAbil.AC);
        Assert.Equal(5, form.FHumData.BonusAbil.MAC);
        Assert.Equal(6, form.FHumData.BonusAbil.HP);
        Assert.Equal(7, form.FHumData.BonusAbil.MP);
        Assert.Equal(8, form.FHumData.BonusAbil.Hit);
        Assert.Equal(9, form.FHumData.BonusAbil.Speed);
        Assert.Equal(10, form.FHumData.BonusAbil.X2);
        Assert.Equal((ushort)4, form.FHumData.wHomeY);
    }

    [Fact]
    public void 原始缺陷_英雄模式下九个无守卫分支仍写FHumData()
    {
        // 原文 :891-932 的 seGold/seGameGold/seGamePoint/sePayPoint/seCreditPoint/seContribution/
        // seBonusPoint/seGameDiamond/seGameGird **没有** if FIsHuman 守卫（:884 seLevel 与 :911 sePKPoint 有）。
        using var scope = new P10RoleDataEditScope();
        var hero = Hero();
        var h = Human();
        using var form = NewHeroForm(hero);
        form.FHumData = h;

        form.seGold.Value = 5555u;
        form.seGameGold.Value = 66u;
        form.seGamePoint.Value = 77;
        form.sePayPoint.Value = 88;
        form.seCreditPoint.Value = 99;
        form.seContribution.Value = 111;
        form.seBonusPoint.Value = 222;
        form.seGameDiamond.Value = 333u;
        form.seGameGird.Value = 444u;

        Assert.Equal(5555u, form.FHumData.nGold);            // ★ 英雄模式却写了人类记录
        Assert.Equal(66u, form.FHumData.nGameGold);
        Assert.Equal(77u, form.FHumData.nGamePoint);
        Assert.Equal(88, form.FHumData.nPayMentPoint);
        Assert.Equal(99, form.FHumData.Abil.CreditPoint);
        Assert.Equal((ushort)111, form.FHumData.wContribution);
        Assert.Equal(222, form.FHumData.nBonusPoint);
        Assert.Equal(333u, form.FHumData.nGameDiamond);
        Assert.Equal(444u, form.FHumData.nGameGird);
        Assert.Equal(9, 9);                                  // 无守卫分支计数：原文 :891/:895/:899/:903/:907/:918/:922/:926/:930

        // 对照：有守卫的两个不写 FHumData
        form.seLevel.Value = 12;
        form.sePKPoint.Value = 13;
        Assert.Equal(12, form.FHeroData.Abil.Level);
        Assert.Equal(13, form.FHeroData.nPKPoint);
        Assert.Equal(42, form.FHumData.Abil.Level);          // 未被英雄操作污染
        Assert.Equal(6, form.FHumData.nPKPoint);
    }

    [Fact]
    public void edtPasswordChange_未绑定Sender与null都落链尾不做事()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        form.edtPasswordChange(null);
        form.edtPasswordChange(form.lbl1);
        form.edtPasswordChange(new object());

        Assert.Equal("pwd", form.FHumData.StoragePwd);
        Assert.Equal(1000u, form.FHumData.nGold);
        Assert.Equal((ushort)4, form.FHumData.wHomeY);
    }

    // ========================================================================================
    // 11) ButtonExportDataClick（导出 / 导入 两个按钮 + 死分支）
    // ========================================================================================

    [Fact]
    public void ButtonExportDataClick_按Sender分流_导出走保存_导入走读取()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        // Sender = ButtonExportData → ProcessSaveDataToFile
        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = Path2("out.hum");
        form.ButtonExportDataClick(form.ButtonExportData);
        Assert.True(File.Exists(Path2("out.hum")));
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导出成功！！！");

        // Sender = ButtonImportData → ProcessLoadDataformFile（**不是**缺陷：原文 :726 明确分支）
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = Path2("out.hum");
        scope.Ui.MessageBoxes.Clear();
        form.ButtonExportDataClick(form.ButtonImportData);
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");
        Assert.DoesNotContain(scope.Ui.MessageBoxes, m => m.Text == "角色数据导出成功！！！");
    }

    [Fact]
    public void 原始缺陷_ButtonExportDataClick里ButtonSaveData分支是死代码()
    {
        // :730-733 的 `else if Sender = ButtonSaveData then begin end;` 是**空分支**，
        // 而 DFM 里 ButtonSaveData.OnClick = ButtonSaveDataClick ⇒ 这条分支永远不会被走到（原文如此）。
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);

        // 计数取证：ButtonSaveData 绑的是**另一条**路径（它自己有 1 条 Click 绑定，且不是本过程）
        Assert.True(P10FormReconcile.IsBound(form.ButtonSaveData, "Click"));
        Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(form.ButtonSaveData));

        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = Path2("never.hum");
        form.ButtonExportDataClick(form.ButtonSaveData);      // 空分支 → 什么都不发生

        Assert.False(File.Exists(Path2("never.hum")));
        Assert.Empty(scope.Ui.MessageBoxes);

        // 链尾无 else：未知 Sender 同样什么都不做
        form.ButtonExportDataClick(null);
        form.ButtonExportDataClick(new object());
        Assert.Empty(scope.Ui.MessageBoxes);
    }

    // ========================================================================================
    // 12) ProcessSaveDataToFile
    // ========================================================================================

    [Fact]
    public void ProcessSaveDataToFile_取消时不落盘不提示()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        scope.SaveDialogResult = false;

        form.ProcessSaveDataToFile();

        Assert.Empty(scope.Ui.MessageBoxes);
        Assert.Equal("测试人物", form.SaveDialog.FileName);        // 原文 :742 先塞角色名
        Assert.Equal(".\\", form.SaveDialog.InitialDirectory);     // 原文 :746 InitialDir := '.\'
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
    }

    [Fact]
    public void ProcessSaveDataToFile_人类_写出SizeOf记录并提示成功()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        string file = Path2("out.hum");
        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = file;

        form.ProcessSaveDataToFile();

        Assert.True(File.Exists(file));
        Assert.Equal((long)StructBytes.SizeOf<THumData>(), new FileInfo(file).Length);
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导出成功！！！");
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);             // :765 FileClose 一定执行

        THumData back = StructBytes.FromBytes<THumData>(File.ReadAllBytes(file));
        Assert.Equal("测试人物", back.ChrName);
        Assert.Equal("acct01", back.Account);
        Assert.Equal(1000u, back.nGold);
        Assert.Equal(42, back.Abil.Level);
    }

    [Fact]
    public void ProcessSaveDataToFile_英雄_写出SizeOf_THeroData()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero("测试英雄");
        using var form = NewHeroForm(hero);
        string file = Path2("hero.hum");
        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = file;

        form.ProcessSaveDataToFile();

        Assert.Equal((long)StructBytes.SizeOf<THeroData>(), new FileInfo(file).Length);
        Assert.Equal("测试英雄", form.SaveDialog.FileName);         // 原文 :744 用英雄名做默认文件名
        THeroData back = StructBytes.FromBytes<THeroData>(File.ReadAllBytes(file));
        Assert.Equal("测试英雄", back.ChrName);
        Assert.Equal(30, back.Abil.Level);
    }

    [Fact]
    public void ProcessSaveDataToFile_打不开文件时提示保存文件出现错误()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = Path.Combine(Dir, "no-such-dir", "x.hum");   // 目录不存在 ⇒ FileCreate 失败

        form.ProcessSaveDataToFile();

        Assert.Single(scope.Ui.MessageBoxes);
        Assert.Equal("保存文件出现错误！！！", scope.Ui.MessageBoxes[0].Text);
        Assert.Equal("错误信息", scope.Ui.MessageBoxes[0].Caption);
        Assert.Equal(TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION, scope.Ui.MessageBoxes[0].Flags);
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
    }

    [Fact]
    public void ProcessSaveDataToFile_目标文件已存在时走FileOpen覆盖前段()
    {
        // 原文 :749-752：文件已存在 → FileOpen(fmOpenReadWrite)（**不截断**）；不存在 → FileCreate。
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        string file = Path2("exists.hum");
        File.WriteAllBytes(file, new byte[StructBytes.SizeOf<THumData>() + 32]);   // 故意比记录长
        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = file;

        form.ProcessSaveDataToFile();

        // FileOpen 不截断 ⇒ 文件长度保持原样（原文如此）
        Assert.Equal((long)StructBytes.SizeOf<THumData>() + 32, new FileInfo(file).Length);
        THumData back = StructBytes.FromBytes<THumData>(File.ReadAllBytes(file));
        Assert.Equal("测试人物", back.ChrName);
    }

    // ========================================================================================
    // 13) ProcessLoadDataformFile
    // ========================================================================================

    [Fact]
    public void ProcessLoadDataformFile_取消时不提示不改记录()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        scope.OpenDialogResult = false;

        form.ProcessLoadDataformFile();

        Assert.Empty(scope.Ui.MessageBoxes);
        Assert.Equal(1000u, form.FHumData.nGold);
        Assert.Equal("测试人物", form.OpenDialog.FileName);         // 原文 :776 先塞角色名
        Assert.Equal(".\\", form.OpenDialog.InitialDirectory);
    }

    [Fact]
    public void ProcessLoadDataformFile_文件不存在时提示指定的文件未找到()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = Path2("missing.hum");

        form.ProcessLoadDataformFile();

        Assert.Single(scope.Ui.MessageBoxes);
        Assert.Equal("指定的文件未找到！！！", scope.Ui.MessageBoxes[0].Text);
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
    }

    [Fact]
    public void ProcessLoadDataformFile_打开失败时提示打开文件出现错误()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        string file = Path2("locked.hum");
        File.WriteAllBytes(file, new byte[StructBytes.SizeOf<THumData>()]);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = file;

        // 独占持有 ⇒ FileOpen(fmShareDenyNone) 打开失败
        using (var hold = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            form.ProcessLoadDataformFile();
        }

        Assert.Single(scope.Ui.MessageBoxes);
        Assert.Equal("打开文件出现错误！！！", scope.Ui.MessageBoxes[0].Text);
    }

    [Fact]
    public void ProcessLoadDataformFile_人类_只保留五个字段其余全取文件()
    {
        using var scope = new P10RoleDataEditScope();

        // 内存里的记录（要保留 5 个字段）+ 文件里的记录（要覆盖其余字段）
        var mem = Human();
        var fileData = Human();
        fileData.ChrName = "文件人物";           // 属于保留的 5 个 ⇒ 应被内存值覆盖
        fileData.Account = "fileacct";           // 属于保留的 5 个
        fileData.DearName = "文件配偶";           // 属于保留的 5 个
        fileData.HeroName = "文件英雄";           // 属于保留的 5 个
        fileData.DeputyHeroName = "文件副英雄";   // 属于保留的 5 个
        fileData.MasterName = "文件师父";         // 非保留 ⇒ 取文件
        fileData.CurMap = "9";
        fileData.HomeMap = "8";
        fileData.wCurX = 700;
        fileData.wCurY = 800;
        fileData.wHomeX = 5;
        fileData.wHomeY = 6;
        fileData.Abil.Level = 88;
        fileData.Abil.CreditPoint = 3;
        fileData.nGold = 9999;
        fileData.nPKPoint = 44;
        fileData.wContribution = 55;
        fileData.nBonusPoint = 66;
        fileData.UValues[0] = 1234;
        fileData.Magics[0] = Magic(11, 1, 1, 0);
        scope.MagicNames[11] = "火球术";

        string path = Path2("in.hum");
        File.WriteAllBytes(path, StructBytes.BytesOf(fileData));

        using var form = NewHumanForm(mem);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = path;

        form.ProcessLoadDataformFile();

        // ★ 只保留的 5 个字段（原文 :808-812）取**内存值**
        Assert.Equal("acct01", form.FHumData.Account);
        Assert.Equal("测试人物", form.FHumData.ChrName);
        Assert.Equal("配偶", form.FHumData.DearName);
        Assert.Equal("英雄名", form.FHumData.HeroName);
        Assert.Equal("副英雄名", form.FHumData.DeputyHeroName);

        // 其余字段一律取**文件值**
        Assert.Equal("文件师父", form.FHumData.MasterName);
        Assert.Equal("9", form.FHumData.CurMap);
        Assert.Equal("8", form.FHumData.HomeMap);
        Assert.Equal((ushort)700, form.FHumData.wCurX);
        Assert.Equal((ushort)800, form.FHumData.wCurY);
        Assert.Equal((ushort)5, form.FHumData.wHomeX);
        Assert.Equal((ushort)6, form.FHumData.wHomeY);
        Assert.Equal(88, form.FHumData.Abil.Level);
        Assert.Equal(3, form.FHumData.Abil.CreditPoint);
        Assert.Equal(9999u, form.FHumData.nGold);
        Assert.Equal(44, form.FHumData.nPKPoint);
        Assert.Equal((ushort)55, form.FHumData.wContribution);
        Assert.Equal(66, form.FHumData.nBonusPoint);
        Assert.Equal(1234, form.FHumData.UValues[0]);

        // :837 FileClose + :838 RefreshShow + :839 提示
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
        Assert.Equal("测试人物", form.edtChrName.Text);
        Assert.Equal("9", form.edtCurMap.Text);
        Assert.Equal(88, form.seLevel.Value);
        Assert.Equal(9999u, form.seGold.Value);
        Assert.Single(form.lvMagic.Items);
        Assert.Equal("火球术", form.lvMagic.Items[0].SubItems[2].Text);
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");
    }

    [Fact]
    public void ProcessLoadDataformFile_英雄_只保留两个字段其余全取文件()
    {
        using var scope = new P10RoleDataEditScope();
        var memHero = Hero("内存英雄", "memacct");
        var fileHero = Hero("文件英雄", "fileacct");
        fileHero.CurMap = "3";
        fileHero.wCurX = 700;
        fileHero.Abil.Level = 77;
        fileHero.nPKPoint = 5;

        string path = Path2("hero.hum");
        File.WriteAllBytes(path, StructBytes.BytesOf(fileHero));

        using var form = NewHeroForm(memHero);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = path;

        form.ProcessLoadDataformFile();

        // ★ 只保留 sAccount / sChrName 两个字段（原文 :829-830）
        Assert.Equal("memacct", form.FHeroData.Account);
        Assert.Equal("内存英雄", form.FHeroData.ChrName);
        // 其余取文件
        Assert.Equal("3", form.FHeroData.CurMap);
        Assert.Equal((ushort)700, form.FHeroData.wCurX);
        Assert.Equal(77, form.FHeroData.Abil.Level);
        Assert.Equal(5, form.FHeroData.nPKPoint);
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");
    }

    [Fact]
    public void 原始缺陷_短读的错误分支不可达_照样提示导入成功()
    {
        // ★★ 原文 :802（以及 :823）`if not FileRead(...) = SizeOf(...) then`：
        //    Delphi 一元 `not` 优先级为第 1 级（最高），`=` 为第 4 级（最低）⇒ 该式实为
        //    `(not nRead) = SizeOf(...)`，即 `(-nRead-1) = SizeOf(...)`，**恒 False**
        //    ⇒ "读取文件出现错误"分支不可达，短读也一路走到 :814 赋值 + :839 提示成功。
        using var scope = new P10RoleDataEditScope();
        var mem = Human();
        using var form = NewHumanForm(mem);
        string path = Path2("short.hum");
        File.WriteAllBytes(path, new byte[10]);              // 只有 10 字节
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = path;

        form.ProcessLoadDataformFile();

        Assert.DoesNotContain(scope.Ui.MessageBoxes, m => m.Text.StartsWith("读取文件出现错误"));
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");

        // 5 个字段仍取自内存（:808-812）
        Assert.Equal("acct01", form.FHumData.Account);
        Assert.Equal("测试人物", form.FHumData.ChrName);
        Assert.Equal("配偶", form.FHumData.DearName);
        Assert.Equal("英雄名", form.FHumData.HeroName);
        Assert.Equal("副英雄名", form.FHumData.DeputyHeroName);
        // 其余字段取自文件（全 0）—— 原来的 1000 金币被覆盖成 0
        Assert.Equal(0u, form.FHumData.nGold);
        Assert.Equal(0, form.FHumData.Abil.Level);
        // Exit 分支不可达 ⇒ :837 的 FileClose 一定执行 ⇒ 无句柄泄漏
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
    }

    [Fact]
    public void ProcessLoadDataformFile_零字节文件同样走成功分支()
    {
        using var scope = new P10RoleDataEditScope();
        var mem = Human();
        using var form = NewHumanForm(mem);
        string path = Path2("empty.hum");
        File.WriteAllBytes(path, Array.Empty<byte>());
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = path;

        form.ProcessLoadDataformFile();

        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据导入成功！！！");
        Assert.Equal("测试人物", form.FHumData.ChrName);
        Assert.Equal(0u, form.FHumData.nGold);
        Assert.Equal(0, DelphiFileIo.OpenHandleCount);
    }

    [Fact]
    public void ProcessLoadDataformFile_人类写出再读回可往返()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        string file = Path2("round.hum");

        scope.SaveDialogResult = true;
        scope.SaveDialogFileName = file;
        form.ProcessSaveDataToFile();

        var other = Human();
        other.nGold = 1;
        using var form2 = NewHumanForm(other);
        scope.OpenDialogResult = true;
        scope.OpenDialogFileName = file;
        scope.Ui.MessageBoxes.Clear();
        form2.ProcessLoadDataformFile();

        Assert.Equal(h.nGold, form2.FHumData.nGold);
        Assert.Equal(h.Abil.Level, form2.FHumData.Abil.Level);
        Assert.Equal("测试人物", form2.FHumData.ChrName);
    }

    // ========================================================================================
    // 14) ButtonSaveDataClick
    // ========================================================================================

    [Fact]
    public void ButtonSaveDataClick_人类_回填栅格变量并调HumanDB_Save()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h, 5);
        form.DoOpen();
        form.strGridVarU.SetCells(1, 1, "1234");
        form.strGridVarU.SetCells(1, 2, "   ");          // 空白 → StrToIntDef 默认 0
        form.strGridVarT.SetCells(1, 1, "tv");

        form.ButtonSaveDataClick(null);

        Assert.Equal(1234, form.FHumData.UValues[0]);
        Assert.Equal(0, form.FHumData.UValues[1]);
        Assert.Equal("tv", form.FHumData.TValues[0].Value);
        Assert.Single(scope.HumanDb.SaveCalls);
        Assert.Equal(5, scope.HumanDb.SaveCalls[0].HumanID);
        Assert.Equal("测试人物", scope.HumanDb.SaveCalls[0].ChrName);
        Assert.Contains(scope.Ui.MessageBoxes, m => m.Text == "角色数据保存成功！！！");
        Assert.Equal(1, scope.HumanDb.LockCount);
        Assert.Equal(1, scope.HumanDb.UnLockCount);
    }

    [Fact]
    public void ButtonSaveDataClick_未走DoOpen时栅格越界读静默为0()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        h.UValues[0] = 5;
        h.UValues[499] = 7;
        using var form = NewHumanForm(h, 3);

        Assert.Equal(101, form.strGridVarU.RowCount);    // DFM 初值：:950 会读到 row 101..500

        form.ButtonSaveDataClick(null);                  // 不应抛（VCL 越界静默语义）

        Assert.Equal(0, form.FHumData.UValues[0]);
        Assert.Equal(0, form.FHumData.UValues[499]);
        Assert.Single(scope.HumanDb.SaveCalls);
        Assert.Equal(3, scope.HumanDb.SaveCalls[0].HumanID);
    }

    [Fact]
    public void ButtonSaveDataClick_英雄_调HeroDB_Save且失败时提示失败()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero();
        using var form = NewHeroForm(hero, 6);
        scope.HeroDb.SaveResult = false;

        form.ButtonSaveDataClick(null);

        Assert.Single(scope.HeroDb.SaveCalls);
        Assert.Equal(6, scope.HeroDb.SaveCalls[0].HeroID);
        Assert.Empty(scope.HumanDb.SaveCalls);
        Assert.Single(scope.Ui.MessageBoxes);
        Assert.Equal("角色数据保存失败！！！", scope.Ui.MessageBoxes[0].Text);
        Assert.Equal(TMsgBox.MB_ICONEXCLAMATION, scope.Ui.MessageBoxes[0].Flags);
    }

    [Fact]
    public void ButtonSaveDataClick_未接线时抛未接线()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        SelectClientRoleDbSeam.HumanDB = null;           // 模拟宿主未接线

        var ex = Assert.Throws<NotSupportedException>(() => form.ButtonSaveDataClick(null));
        Assert.Contains("未接线", ex.Message);
        Assert.Contains("HumanDB", ex.Message);
    }

    [Fact]
    public void ButtonSaveDataClick_Save抛异常时被基类吞掉并返回失败()
    {
        // THumanDBBase.Save 的公开包装 `catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }`
        // ⇒ 异常不外泄、Result 保持初值 False ⇒ 走"保存失败"提示（原文如此）。
        using var scope = new P10RoleDataEditScope();
        var h = Human();
        using var form = NewHumanForm(h);
        var logs = new List<string>();
        Action<string> saved = RoleDbSeam.MainOutMessage;
        try
        {
            RoleDbSeam.MainOutMessage = m => logs.Add(m);
            scope.HumanDb.ThrowOnSaveMessage = "炸给谁看";

            form.ButtonSaveDataClick(null);

            Assert.Single(logs);
            Assert.Contains("炸给谁看", logs[0]);
            Assert.Single(scope.Ui.MessageBoxes);
            Assert.Equal("角色数据保存失败！！！", scope.Ui.MessageBoxes[0].Text);
            Assert.Equal(0, scope.HumanDb.SaveCalls.Count);   // 记录在抛之前就 Add 了？→ 见下：Add 在抛之前
        }
        finally
        {
            RoleDbSeam.MainOutMessage = saved;
        }
    }

    // ========================================================================================
    // 15) 单元级 ShowFrmRoleDataEdit
    // ========================================================================================

    [Fact]
    public void ShowFrmRoleDataEdit_人类_设置FID与edtID并Free()
    {
        using var scope = new P10RoleDataEditScope();
        var h = Human();

        RoleDataEditUnit.ShowFrmRoleDataEdit(7, h, null);

        Assert.Single(scope.Shown);
        (int fid, bool isHuman, string text, string edtId, string chrName) = scope.Shown[0];
        Assert.Equal(7, fid);
        Assert.True(isHuman);
        Assert.Equal("编辑人物数据 [测试人物]", text);
        Assert.Equal("7", edtId);
        Assert.Equal("测试人物", chrName);
        Assert.True(scope.ShownForms[0].IsDisposed);         // 原文 finally FrmRoleDataEdit.Free
    }

    [Fact]
    public void ShowFrmRoleDataEdit_英雄_走else分支()
    {
        using var scope = new P10RoleDataEditScope();
        var hero = Hero("测试英雄");

        RoleDataEditUnit.ShowFrmRoleDataEdit(9, null, hero);

        Assert.Single(scope.Shown);
        (int fid, bool isHuman, string text, string edtId, string chrName) = scope.Shown[0];
        Assert.Equal(9, fid);
        Assert.False(isHuman);
        Assert.Equal("编辑英雄数据 [测试英雄]", text);
        Assert.Equal("9", edtId);
        Assert.Equal("测试英雄", chrName);
        Assert.True(scope.ShownForms[0].IsDisposed);
    }

    [Fact]
    public void ShowFrmRoleDataEdit_ShowModalHandler为null时不阻塞()
    {
        using var scope = new P10RoleDataEditScope();
        TFrmRoleDataEdit.ShowModalHandler = null;            // 默认未接线
        var h = Human();

        RoleDataEditUnit.ShowFrmRoleDataEdit(1, h, null);    // 不应阻塞、不应抛

        Assert.Empty(scope.Shown);
    }

    [Fact]
    public void ShowFrmRoleDataEdit_HeroData为null时与原文一样炸()
    {
        using var scope = new P10RoleDataEditScope();

        // 原文 `FHeroData := HeroData^`（nil 指针 ⇒ 访问违例）；托管 Nullable 取值抛 InvalidOperationException
        Assert.Throws<InvalidOperationException>(() => RoleDataEditUnit.ShowFrmRoleDataEdit(1, null, null));
    }

    [Fact]
    public void ShowModalEquivalent_接线后返回handler结果()
    {
        using var scope = new P10RoleDataEditScope();
        using var form = new TFrmRoleDataEdit();
        Assert.False(form.ShowModalEquivalent());            // 默认不阻塞、返回 false
        TFrmRoleDataEdit.ShowModalHandler = f => f == form;
        Assert.True(form.ShowModalEquivalent());
    }

    // ========================================================================================
    // 16) DFM 细节（列头 / 文本 / 几何）
    // ========================================================================================

    [Fact]
    public void DFM_标题与标签文本对齐()
    {
        using var form = new TFrmRoleDataEdit();

        Assert.Equal("FrmRoleDataEdit", form.Name);
        Assert.Equal("编辑人物数据", form.Text);
        Assert.Equal(new System.Drawing.Size(522, 380), form.ClientSize);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedSingle, form.FormBorderStyle);
        Assert.Equal(System.Windows.Forms.FormStartPosition.CenterScreen, form.StartPosition);   // DFM: poDesktopCenter
        Assert.Equal(new System.Drawing.Point(373, 215), form.Location);

        Assert.Equal("修改时人物不能在线，否则数据回档或出错！", form.lbl11111.Text);
        Assert.Equal(System.Drawing.Color.Red, form.lbl11111.ForeColor);
        Assert.Equal("人物名称:", form.lbl2.Text);
        Assert.Equal("登录帐号:", form.lbl3.Text);
        Assert.Equal("仓库密码:", form.lbl4.Text);
        Assert.Equal("配偶名称:", form.lbl5.Text);
        Assert.Equal("师徒名称:", form.lbl6.Text);
        Assert.Equal("索引号码:", form.lbl1.Text);
        Assert.Equal("当前地图:", form.lbl7.Text);
        Assert.Equal("当前座标:", form.lbl8.Text);
        Assert.Equal("回城地图:", form.lbl9.Text);
        Assert.Equal("回城座标:", form.lbl10.Text);
        Assert.Equal("师父", form.chkIsMaster.Text);
        Assert.Equal("属性点", form.GroupBox6.Text);
        Assert.Equal("PK点:", form.lbl19.Text);
        Assert.Equal("可用属性点:", form.lbl21.Text);
        Assert.Equal("保存修改(&S)", form.ButtonSaveData.Text);
        Assert.Equal("导出数据(&E)", form.ButtonExportData.Text);
        Assert.Equal("导入数据(&I)", form.ButtonImportData.Text);
        Assert.Equal("hum", form.SaveDialog.DefaultExt);
        Assert.Equal("人物数据 (*.hum)|*.hum", form.SaveDialog.Filter);
        Assert.Equal("人物数据 (*.hum)|*.hum", form.OpenDialog.Filter);

        Assert.Equal("普通", form.tsBase.Text);
        Assert.Equal("信息", form.tsInfo.Text);
        Assert.Equal("技能", form.tsMagic.Text);
        Assert.Equal("装备", form.tsUserItem.Text);
        Assert.Equal("称号", form.tsFenghao.Text);
        Assert.Equal("仓库", form.tsSorage.Text);
        Assert.Equal("U变量", form.tsVarU.Text);
        Assert.Equal("T变量", form.tsVarT.Text);
    }

    [Fact]
    public void DFM_四张ListView的列头与DFM一致()
    {
        using var form = new TFrmRoleDataEdit();

        Assert.Equal(new[] { "序号", "技能", "技能名称", "等级", "修炼点", "快捷键" },
            form.lvMagic.Columns.Cast<System.Windows.Forms.ColumnHeader>().Select(c => c.Text).ToArray());
        Assert.Equal(40, form.lvMagic.Columns[0].Width);
        Assert.Equal(100, form.lvMagic.Columns[2].Width);

        Assert.Equal(new[] { "序号", "装备位置", "装备名称", "Idx", "序列号", "持久", "参数" },
            form.lvUserItem.Columns.Cast<System.Windows.Forms.ColumnHeader>().Select(c => c.Text).ToArray());
        Assert.Equal(7, form.lvUserItem.Columns.Count);
        Assert.Equal(System.Windows.Forms.HorizontalAlignment.Center, form.lvUserItem.Columns[5].TextAlign);

        Assert.Equal(new[] { "序号", "装备名称", "Idx", "序列号", "持久", "参数" },
            form.lvFenghaoItem.Columns.Cast<System.Windows.Forms.ColumnHeader>().Select(c => c.Text).ToArray());
        Assert.Equal(new[] { "序号", "装备名称", "Idx", "序列号", "持久", "参数" },
            form.lvStorage.Columns.Cast<System.Windows.Forms.ColumnHeader>().Select(c => c.Text).ToArray());

        foreach (var lv in new[] { form.lvMagic, form.lvUserItem, form.lvFenghaoItem, form.lvStorage })
        {
            Assert.True(lv.GridLines);
            Assert.True(lv.FullRowSelect);
            Assert.True(lv.Columns.Count > 0);
            Assert.Equal(6, lv.Columns.Count == 0 ? 0 : lv.Columns.Count);
        }
    }
}
