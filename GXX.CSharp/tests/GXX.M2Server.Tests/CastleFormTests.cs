using System.Linq;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J2：CastleManage.pas + AttackSabukWallConfig.pas 窗体族 1:1 转换测试。
/// 覆盖 AddAttackerInfo 持久化、增加/编辑攻城申请全部分支（Delphi case FAddAttackGuild）、
/// 城堡管理窗体列表/信息刷新、保存写盘、开战/停战按钮。
/// </summary>
public sealed class CastleFormTests : IDisposable
{
    private readonly string _dir;
    private readonly CastleManageForm _form;

    public CastleFormTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j2_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2Config.sCastleDir = _dir + "\\";
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        CastleState.g_CastleManager.m_CastleList.Clear();
        CastleState.g_GuildManager.GuildList.Clear();
        CastleManageForm.frmCastleManage = null;
        CastleManageForm.CurCastle = null;
        CastleManageForm.SelAttackGuildInfo = null;

        _form = StaRunner.New(() => new CastleManageForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        CastleState.g_CastleManager.m_CastleList.Clear();
        CastleState.g_GuildManager.GuildList.Clear();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private TUserCastle NewCastleInManager()
    {
        var castle = new TUserCastle("沙巴克");
        castle.m_sConfigDir = "Castle1";
        var master = CastleState.g_GuildManager.FindGuild("守方行会");
        if (master != null) castle.SetMasterGuild(master);
        CastleState.g_CastleManager.Add(castle);
        return castle;
    }

    private void AddGuilds(params string[] names)
    {
        foreach (var n in names)
            Assert.True(CastleState.g_GuildManager.AddGuild(n, "掌门"));
    }

    private string CastleFile(string name) => Path.Combine(_dir, "Castle1", name);

    // ---------- AddAttackerInfo / SaveAttackSabukWall ----------

    [Fact]
    public void AddAttackerInfo_DuplicateRejected_AndPersistsFile()
    {
        AddGuilds("攻方行会");
        var castle = NewCastleInManager();
        var guild = CastleState.g_GuildManager.FindGuild("攻方行会")!;
        var date = new DateTime(2026, 9, 20);

        Assert.True(castle.AddAttackerInfo(guild, date));
        Assert.False(castle.AddAttackerInfo(guild, date), "重复申请应失败");
        Assert.Single(castle.m_AttackWarList);
        Assert.Equal("攻方行会", castle.m_AttackWarList[0].sGuildName);
        Assert.Equal(date, castle.m_AttackWarList[0].AttackDate);

        string content = File.ReadAllText(CastleFile("AttackSabukWall.txt"), System.Text.Encoding.GetEncoding(936));
        Assert.Contains("攻方行会       \"2026-9-20\"", content);
    }

    [Fact]
    public void Save_WritesSabukWConfigWithDefenseKeys()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        castle.m_MainDoor.nX = 100;
        castle.m_MainDoor.nY = 200;
        castle.m_MainDoor.sName = "沙巴克城门";
        castle.Save();

        Assert.True(File.Exists(CastleFile("SabukW.txt")));
        var ini = new GXX.Core.Util.TFastIniFile(CastleFile("SabukW.txt"));
        Assert.Equal("沙巴克", ini.ReadString("Setup", "CastleName", ""));
        Assert.Equal("守方行会", ini.ReadString("Setup", "OwnGuild", ""));
        Assert.Equal("沙巴克城门", ini.ReadString("Defense", "MainDoorName", ""));
        Assert.Equal(100, ini.ReadInteger("Defense", "MainDoorX", 0));
        Assert.Equal(200, ini.ReadInteger("Defense", "MainDoorY", 0));
    }

    // ---------- AttackSabukWallForm ----------

    [StaFact]
    public void AttackWall_OpenAddMode_CaptionEmptyDateToday()
    {
        AddGuilds("行会一", "行会二");
        NewCastleInManager();
        CastleManageForm.CurCastle = CastleState.g_CastleManager.m_CastleList[0];

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(true, showModal: false);
            Assert.Equal("增加攻城行会", frm.Text);
            Assert.Equal("", frm.EditGuildName.Text);
            Assert.Equal(DateTime.Today.Date, frm.RzDateTimeEditAttackDate.Value.Date);
            Assert.Equal(2, frm.ListBoxGuild.Items.Count);
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_OpenEditMode_PrefillsSelAttackGuildInfo()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        var date = new DateTime(2026, 10, 1);
        var info = new TAttackerInfo(CastleState.g_GuildManager.FindGuild("守方行会")!, date) { sGuildName = "守方行会" };
        castle.m_AttackWarList.Add(info);
        CastleManageForm.CurCastle = castle;
        CastleManageForm.SelAttackGuildInfo = info;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(false, showModal: false);
            Assert.Equal("编辑攻城行会 守方行会", frm.Text);
            Assert.Equal("守方行会", frm.EditGuildName.Text);
            Assert.Equal(date, frm.RzDateTimeEditAttackDate.Value);
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_AddSingleExistingGuild_SavesAndRefreshes()
    {
        AddGuilds("攻方行会");
        var castle = NewCastleInManager();
        CastleManageForm.CurCastle = castle;
        AttackSabukWallForm.frmCastleManage = _form;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(true, showModal: false);
            frm.EditGuildName.Text = "攻方行会";
            frm.RzDateTimeEditAttackDate.Value = new DateTime(2026, 11, 11);
            var r = frm.ButtonOKClick(frm);
            Assert.True(r.ok);
            Assert.Single(castle.m_AttackWarList);
            Assert.True(File.Exists(CastleFile("AttackSabukWall.txt")));
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_AddUnknownGuild_ShowsMessage()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        CastleManageForm.CurCastle = castle;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(true, showModal: false);
            frm.EditGuildName.Text = "不存在的行会";
            var r = frm.ButtonOKClick(frm);
            Assert.False(r.ok);
            Assert.Equal("输入的行会不存在！", r.msg);
            Assert.Empty(castle.m_AttackWarList);
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_AddAllGuilds_CheckBoxAll()
    {
        AddGuilds("行会一", "行会二", "行会三");
        var castle = NewCastleInManager();
        CastleManageForm.CurCastle = castle;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(true, showModal: false);
            frm.CheckBoxAll.Checked = true;
            frm.CheckBoxAllClick(frm);
            Assert.False(frm.EditGuildName.Enabled); // 全部行会时禁用名称输入
            var r = frm.ButtonOKClick(frm);
            Assert.True(r.ok);
            Assert.Equal(3, castle.m_AttackWarList.Count);
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_EditSingle_UpdatesDate()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        var info = new TAttackerInfo(CastleState.g_GuildManager.FindGuild("守方行会")!, new DateTime(2026, 9, 1)) { sGuildName = "守方行会" };
        castle.m_AttackWarList.Add(info);
        CastleManageForm.CurCastle = castle;
        CastleManageForm.SelAttackGuildInfo = info;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(false, showModal: false);
            frm.RzDateTimeEditAttackDate.Value = new DateTime(2026, 12, 31);
            var r = frm.ButtonOKClick(frm);
            Assert.True(r.ok);
            Assert.Equal(new DateTime(2026, 12, 31), info.AttackDate);
            Assert.Single(castle.m_AttackWarList);
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    [StaFact]
    public void AttackWall_EditAll_UpdatesAllDates()
    {
        AddGuilds("甲", "乙");
        var castle = NewCastleInManager();
        var g1 = CastleState.g_GuildManager.FindGuild("甲")!;
        var g2 = CastleState.g_GuildManager.FindGuild("乙")!;
        castle.m_AttackWarList.Add(new TAttackerInfo(g1, new DateTime(2026, 1, 1)) { sGuildName = "甲" });
        castle.m_AttackWarList.Add(new TAttackerInfo(g2, new DateTime(2026, 2, 2)) { sGuildName = "乙" });
        CastleManageForm.CurCastle = castle;

        var frm = StaRunner.New(() => new AttackSabukWallForm());
        try
        {
            frm.Open(false, showModal: false);
            frm.CheckBoxAll.Checked = true;
            frm.RzDateTimeEditAttackDate.Value = new DateTime(2027, 3, 3);
            Assert.True(frm.ButtonOKClick(frm).ok);
            Assert.All(castle.m_AttackWarList, a => Assert.Equal(new DateTime(2027, 3, 3), a.AttackDate));
        }
        finally
        {
            StaRunner.New(() => frm.Dispose());
        }
    }

    // ---------- CastleManageForm ----------

    [StaFact]
    public void Manage_Open_SelectsFirstCastle_AndFillsInfo()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        castle.m_nTotalGold = 123456;
        castle.m_nTechLevel = 3;
        castle.m_MainDoor.sName = "沙巴克城门";
        castle.m_MainDoor.nX = 100;
        castle.m_MainDoor.nY = 200;

        _form.Open(showModal: false);
        _form.Timer1Timer(_form); // 测试无消息泵，手动触发定时器首选逻辑
        Assert.Same(castle, CastleManageForm.CurCastle); // Timer1 自动选中第一行
        Assert.Equal("守方行会", _form.EditOwenGuildName.Text);
        Assert.Equal(123456, (int)_form.EditTotalGold.Value);
        Assert.Equal(3, (int)_form.EditTechLevel.Value);
        Assert.Equal("沙巴克", _form.EditCastleName.Text);
        Assert.Equal("停战中...", _form.EditWarStatus.Text);
        Assert.True(_form.ButtonStartWar.Enabled);
        Assert.False(_form.ButtonStopWar.Enabled);
        Assert.True(_form.ButtonAttackAdd.Enabled);
        // 守卫列表：城门+三墙+12 弓箭手+4 卫兵 = 20 行
        Assert.Equal(20, _form.ListViewGuard.Items.Count);
        Assert.Equal("沙巴克城门", _form.ListViewGuard.Items[0].SubItems[1].Text);
        Assert.Equal("100:200", _form.ListViewGuard.Items[0].SubItems[2].Text);
    }

    [StaFact]
    public void Manage_ButtonSave_WritesEditsAndFile()
    {
        AddGuilds("守方行会");
        var castle = NewCastleInManager();
        _form.Open(showModal: false);
        _form.Timer1Timer(_form); // 测试无消息泵，手动触发定时器首选逻辑

        _form.EditHomeMap.Text = "0151x";
        _form.SpinEditNomeX.Value = 111;
        _form.SpinEditNomeY.Value = 222;
        _form.EditTunnelMap.Text = "0152";
        _form.ButtonSaveClick(_form);

        Assert.Equal("0151x", castle.m_sHomeMap);
        Assert.Equal(111, castle.m_nHomeX);
        Assert.Equal(222, castle.m_nHomeY);
        Assert.Equal("0152", castle.m_sSecretMap);
        Assert.True(File.Exists(CastleFile("SabukW.txt")));
        Assert.False(_form.ButtonSave.Enabled);
    }

    [StaFact]
    public void Manage_AttackListFlow_AddSelectDelete()
    {
        AddGuilds("攻方行会");
        var castle = NewCastleInManager();
        _form.Open(showModal: false);
        _form.Timer1Timer(_form); // 测试无消息泵，手动触发定时器首选逻辑

        // 增加（经 AttackSabukWallForm，Delphi 全局 FrmAttackSabukWall 引用）
        _form.ButtonAttackAddClick(_form);
        var addFrm = AttackSabukWallForm.FrmAttackSabukWall;
        Assert.NotNull(addFrm);
        addFrm!.EditGuildName.Text = "攻方行会";
        Assert.True(addFrm.ButtonOKClick(addFrm).ok);
        StaRunner.New(() => addFrm.Dispose());

        Assert.Single(_form.ListViewAttackSabukWall.Items); // RefCastleAttackSabukWall 已刷新
        // 选中 → SelAttackGuildInfo + 按钮使能
        _form.ListViewAttackSabukWall.Items[0].Selected = true;
        _form.ListViewAttackSabukWallClick(_form);
        Assert.NotNull(CastleManageForm.SelAttackGuildInfo);
        Assert.True(_form.ButtonAttackEdit.Enabled);
        Assert.True(_form.ButtonAttackDel.Enabled);

        // 删除（确认 YES）
        M2Forms.NextAnswer = M2Forms.IDYES;
        _form.ButtonAttackDelClick(_form);
        Assert.Empty(castle.m_AttackWarList);
        Assert.Empty(_form.ListViewAttackSabukWall.Items);
        Assert.True(File.Exists(CastleFile("AttackSabukWall.txt")));
    }

    [StaFact]
    public void Manage_AttackDelete_RefuseKeepsEntry()
    {
        AddGuilds("攻方行会");
        var castle = NewCastleInManager();
        var g = CastleState.g_GuildManager.FindGuild("攻方行会")!;
        castle.m_AttackWarList.Add(new TAttackerInfo(g, new DateTime(2026, 9, 9)) { sGuildName = "攻方行会" });
        _form.Open(showModal: false);
        _form.Timer1Timer(_form); // 测试无消息泵，手动触发定时器首选逻辑
        _form.RefCastleAttackSabukWall();
        _form.ListViewAttackSabukWall.Items[0].Selected = true;
        _form.ListViewAttackSabukWallClick(_form);

        M2Forms.NextAnswer = M2Forms.IDNO;
        _form.ButtonAttackDelClick(_form);
        Assert.Single(castle.m_AttackWarList);
    }

    [StaFact]
    public void Manage_StartStopWar_Buttons()
    {
        AddGuilds("守方行会", "攻方行会");
        var castle = NewCastleInManager();
        _form.Open(showModal: false);
        _form.Timer1Timer(_form); // 测试无消息泵，手动触发定时器首选逻辑

        _form.ButtonStartWarClick(_form);
        Assert.True(castle.m_boUnderWar);
        // 攻方 + 守方（申请列表）+ 守方（占领者重复加入，Delphi 同构）
        Assert.Equal(3, castle.m_AttackGuildList.Count);
        Assert.Equal("攻城中...", _form.EditWarStatus.Text);
        Assert.False(_form.ButtonStartWar.Enabled);
        Assert.True(_form.ButtonStopWar.Enabled);

        _form.ButtonStopWarClick(_form);
        Assert.False(castle.m_boUnderWar);
        Assert.Equal("停战中...", _form.EditWarStatus.Text);
    }
}
