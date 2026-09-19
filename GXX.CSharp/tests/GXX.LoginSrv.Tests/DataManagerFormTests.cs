using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Util;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

internal sealed class FakeHumanDb : IHumanRoleDB
{
    public readonly List<TSerarchRoleData> SearchResult = new();
    public TSearchMatchType? LastAccountMatch;
    public TSearchMatchType? LastNameMatch;
    public string LastSearchText = "";

    public bool GetResult;
    public int GetId = 77;
    public bool SetEnabledResult = true;
    public string LastSetEnabledAccount = "";
    public string LastSetEnabledName = "";
    public int LastSetEnabledValue;
    public bool EraseResult = true;
    public string LastEraseAccount = "";
    public string LastEraseName = "";

    public int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        LastAccountMatch = MatchType;
        LastSearchText = Account;
        foreach (var r in SearchResult) RoleList.Add(r);
        return SearchResult.Count;
    }

    public int SearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        LastNameMatch = MatchType;
        LastSearchText = HumanName;
        foreach (var r in SearchResult) RoleList.Add(r);
        return SearchResult.Count;
    }

    public bool Get(string Account, string HumanName, ref THumData? HumData, out int HumanID)
    {
        HumData = new THumData();
        HumanID = GetId;
        return GetResult;
    }

    public bool SetEnabled(string Account, string HumanName, int Enabled)
    {
        LastSetEnabledAccount = Account;
        LastSetEnabledName = HumanName;
        LastSetEnabledValue = Enabled;
        return SetEnabledResult;
    }

    public bool Erase(string Account, string HumanName)
    {
        LastEraseAccount = Account;
        LastEraseName = HumanName;
        return EraseResult;
    }
}

internal sealed class FakeHeroDb : IHeroRoleDB
{
    public readonly List<TSerarchRoleData> SearchResult = new();
    public TSearchMatchType? LastAccountMatch;
    public TSearchMatchType? LastNameMatch;

    public bool GetResult;
    public int GetId = 88;
    public bool EraseResult = true;
    public string LastEraseName = "";

    public int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        LastAccountMatch = MatchType;
        foreach (var r in SearchResult) RoleList.Add(r);
        return SearchResult.Count;
    }

    public int SearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList)
    {
        LastNameMatch = MatchType;
        foreach (var r in SearchResult) RoleList.Add(r);
        return SearchResult.Count;
    }

    public bool Get(string HeroName, ref THeroData? HeroData, out int HeroID)
    {
        HeroData = new THeroData();
        HeroID = GetId;
        return GetResult;
    }

    public bool Erase(string HeroName)
    {
        LastEraseName = HeroName;
        return EraseResult;
    }
}

internal sealed class FakeRoleDb : IRoleDB
{
    public readonly FakeHumanDb Human = new();
    public readonly FakeHeroDb Hero = new();
    public IHumanRoleDB HumanDB => Human;
    public IHeroRoleDB HeroDB => Hero;
}

/// <summary>uFrmDataManager.pas TFrmDataManager 1:1 测试。</summary>
public sealed class DataManagerFormTests : IDisposable
{
    private readonly FakeRoleDb _db = new();

    public DataManagerFormTests()
    {
        LoginSrvShare.ResetForTests();
        LoginSrvRoleDb.ResetForTests();
        LoginSrvRoleDb.g_RoleDB = _db;
        LoginSrvForms.MessageBoxHandler = (_, _, _) => LoginSrvForms.IDOK;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
    }

    public void Dispose()
    {
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
        LoginSrvRoleDb.ResetForTests();
        LoginSrvShare.ResetForTests();
    }

    private static TSerarchRoleData Role(string account, string name, bool hero = false,
        int isDelete = 0, int sex = 0, int job = 0, uint level = 1)
        => new()
        {
            Account = account,
            RoleName = name,
            IsHero = hero,
            IsDelete = isDelete,
            Sex = sex,
            Job = job,
            Level = level,
        };

    [StaFact]
    public void FormCreate_ControlsMatchDfm()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            Assert.Equal("角色数据管理", f.Text);
            Assert.Equal(576, f.ClientSize.Width);
            Assert.Equal(362, f.ClientSize.Height);
            Assert.Equal("角色：", f.lblRole.Text);
            Assert.Equal("帐户：", f.lblAccount.Text);
            Assert.Equal("创建角色(&C)", f.BtnCreateChr.Text);
            Assert.False(f.BtnCreateChr.Enabled);
            Assert.Equal("删除角色(&D)", f.btnDeleteRole.Text);
            Assert.False(f.btnDeleteRole.Visible);
            Assert.Equal("按角色名", f.btnSearchRole.Text);
            Assert.Equal("按帐户名", f.btnSearchAccount.Text);
            Assert.Equal("禁用人物(&D)", f.btnDisableHuman.Text);
            Assert.Equal("启用人物(&U)", f.btnEnableHuman.Text);
            Assert.Equal("编辑数据(&E)", f.btnEditData.Text);
            Assert.Equal("模糊", f.chkRoleFuzzy.Text);
            Assert.Equal("模糊", f.chkAccountFuzzy.Text);
            Assert.Equal(new[] { "启用", "登录帐户", "角色名称", "是否英雄", "人物删除", "性别", "职业", "等级" },
                TFrmDataManager.RoleColumnTitles);
            Assert.Equal(new[] { 40, 120, 98, 60, 60, 0, 0, 80 }, TFrmDataManager.RoleColumnWidths);
        });
    }

    [StaFact]
    public void BtnCreateChrClick_IsCommentedOutInSource()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.BtnCreateChrClick(f);   // 原文方法体整体被 (* *) 注释 → 无副作用
            Assert.Equal(0, f.vstRole.RootNodeCount);
        });
    }

    [StaTheory]
    [InlineData(0, "战士")]
    [InlineData(1, "法师")]
    [InlineData(2, "道士")]
    [InlineData(3, "-")]
    [InlineData(-1, "-")]
    public void GetJobName_MapsDelphiCases(int job, string expected)
        => Assert.Equal(expected, TFrmDataManager.GetJobName(job));

    [StaFact]
    public void RefreshRoleList_AddsOneNodePerRow()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            _db.Human.SearchResult.Add(Role("a", "r1", level: 11));
            _db.Human.SearchResult.Add(Role("a", "r2", level: 12));
            f.vstRole.AddChild(null).Data = Role("x", "old");

            f.edtAccount.Text = "a";
            f.btnSearchAccountClick(f);

            Assert.Equal(2, f.vstRole.RootNodeCount);   // RefreshRoleList 先 Clear
            Assert.Equal(11u, ((TSerarchRoleData)f.vstRole.Roots[0].Data!).Level);
            Assert.Equal(12u, ((TSerarchRoleData)f.vstRole.Roots[1].Data!).Level);
        });
    }

    [StaFact]
    public void edtAccountKeyPress_Enter_ClearsKeyAndSearches()
    {
        StaRunner.New(() =>
        {
            _db.Human.SearchResult.Add(Role("acc", "hero1"));
            using var f = new TFrmDataManager();
            f.edtAccount.Text = "acc";
            char k = '\r';
            f.edtAccountKeyPress(f, ref k);

            Assert.Equal('\0', k);
            Assert.Equal(1, f.vstRole.RootNodeCount);
            Assert.Equal("acc", _db.Human.LastSearchText);
        });
    }

    [StaFact]
    public void edtAccountKeyPress_NonEnter_DoesNotSearch()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.edtAccount.Text = "acc";
            char k = 'a';
            f.edtAccountKeyPress(f, ref k);
            Assert.Equal('a', k);
            Assert.Null(_db.Human.LastAccountMatch);
        });
    }

    [StaFact]
    public void edtRoleKeyPress_Enter_ClearsKeyAndSearches()
    {
        StaRunner.New(() =>
        {
            _db.Human.SearchResult.Add(Role("acc", "role1"));
            using var f = new TFrmDataManager();
            f.edtRole.Text = "role1";
            char k = '\r';
            f.edtRoleKeyPress(f, ref k);

            Assert.Equal('\0', k);
            Assert.Equal(1, f.vstRole.RootNodeCount);
            Assert.Equal("role1", _db.Human.LastSearchText);
        });
    }

    [StaFact]
    public void btnSearchAccountClick_Empty_IsNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.edtAccount.Text = "";
            f.btnSearchAccountClick(f);
            Assert.Null(_db.Human.LastAccountMatch);
            Assert.Equal(0, f.vstRole.RootNodeCount);
        });
    }

    [StaFact]
    public void btnSearchAccountClick_Fuzzy_UsesFuzzyForHumanAndHero_AndMergesLists()
    {
        StaRunner.New(() =>
        {
            _db.Human.SearchResult.Add(Role("acc", "h1"));
            _db.Hero.SearchResult.Add(Role("acc", "hero1", hero: true));
            using var f = new TFrmDataManager();
            f.chkAccountFuzzy.Checked = true;
            f.edtAccount.Text = "acc";
            f.btnSearchAccountClick(f);

            Assert.Equal(TSearchMatchType.smtFuzzy, _db.Human.LastAccountMatch);
            Assert.Equal(TSearchMatchType.smtFuzzy, _db.Hero.LastAccountMatch);
            Assert.Equal(2, f.vstRole.RootNodeCount);       // 人物 + 英雄
            Assert.False(f.btnDisableHuman.Enabled);
            Assert.False(f.btnEnableHuman.Enabled);
            Assert.False(f.btnEditData.Enabled);
        });
    }

    [StaFact]
    public void btnSearchAccountClick_NotFuzzy_UsesComplete()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.chkAccountFuzzy.Checked = false;
            f.edtAccount.Text = "acc";
            f.btnSearchAccountClick(f);
            Assert.Equal(TSearchMatchType.smtComplete, _db.Human.LastAccountMatch);
            Assert.Equal(TSearchMatchType.smtComplete, _db.Hero.LastAccountMatch);
        });
    }

    [StaFact]
    public void btnSearchRoleClick_FuzzyAndComplete()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.chkRoleFuzzy.Checked = true;
            f.edtRole.Text = "r";
            f.btnSearchRoleClick(f);
            Assert.Equal(TSearchMatchType.smtFuzzy, _db.Human.LastNameMatch);

            f.chkRoleFuzzy.Checked = false;
            f.btnSearchRoleClick(f);
            Assert.Equal(TSearchMatchType.smtComplete, _db.Human.LastNameMatch);
        });
    }

    [StaFact]
    public void btnSearchRoleClick_Empty_IsNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.edtRole.Text = "";
            f.btnSearchRoleClick(f);
            Assert.Null(_db.Human.LastNameMatch);
        });
    }

    // ------------------------------------------------------------------
    // vstRoleGetText 列映射
    // ------------------------------------------------------------------

    [StaFact]
    public void vstRoleGetText_MapsAllEightColumns()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "角色甲", hero: true, isDelete: 3, sex: 1, job: 2, level: 42);

            Assert.Equal("", GetCell(f, node, 0));      // BOOL_NAME[(IsDelete and 2) = 0] → 3&2=2≠0 → index 0 = ''
            Assert.Equal("acc", GetCell(f, node, 1));
            Assert.Equal("角色甲", GetCell(f, node, 2));
            Assert.Equal("√", GetCell(f, node, 3));     // IsHero → '√'
            Assert.Equal("√", GetCell(f, node, 4));     // IsDelete and 1 <> 0
            Assert.Equal("女", GetCell(f, node, 5));    // Sex = 1
            Assert.Equal("道士", GetCell(f, node, 6));
            Assert.Equal("42", GetCell(f, node, 7));
        });
    }

    [StaFact]
    public void vstRoleGetText_EnabledNotDeletedSex0Job0()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", hero: false, isDelete: 0, sex: 0, job: 0, level: 7);

            Assert.Equal("√", GetCell(f, node, 0));     // (0 and 2) = 0 → '√'
            Assert.Equal("", GetCell(f, node, 3));      // 非英雄
            Assert.Equal("", GetCell(f, node, 4));      // (0 and 1) = 0
            Assert.Equal("男", GetCell(f, node, 5));
            Assert.Equal("战士", GetCell(f, node, 6));
            Assert.Equal("7", GetCell(f, node, 7));
        });
    }

    [StaFact]
    public void vstRoleGetText_NullNodeData_LeavesCellText()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            string cell = "untouched";
            f.vstRoleGetText(f.vstRole, node, 1, ref cell);
            Assert.Equal("untouched", cell);
        });
    }

    private static string GetCell(TFrmDataManager f, TVirtualNode node, int col)
    {
        string cell = "";
        f.vstRoleGetText(f.vstRole, node, col, ref cell);
        return cell;
    }

    // ------------------------------------------------------------------
    // vstRoleFocusChanged / 启用禁用删除
    // ------------------------------------------------------------------

    [StaFact]
    public void vstRoleFocusChanged_NullNode_DisablesAllButtons()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.btnDisableHuman.Enabled = true;
            f.btnEnableHuman.Enabled = true;
            f.btnEditData.Enabled = true;

            f.vstRoleFocusChanged(f.vstRole, null, 0);

            Assert.False(f.btnDisableHuman.Enabled);
            Assert.False(f.btnEnableHuman.Enabled);
            Assert.False(f.btnEditData.Enabled);
        });
    }

    [StaTheory]
    [InlineData(0, false, true, false)]
    [InlineData(2, false, false, true)]
    [InlineData(0, true, false, false)]
    [InlineData(2, true, false, false)]
    public void vstRoleFocusChanged_EnablesButtonsByFlags(int isDelete, bool isHero, bool disableEnabled, bool enableEnabled)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("a", "r", hero: isHero, isDelete: isDelete);

            f.vstRoleFocusChanged(f.vstRole, node, 0);

            Assert.Equal(disableEnabled, f.btnDisableHuman.Enabled);
            Assert.Equal(enableEnabled, f.btnEnableHuman.Enabled);
            Assert.True(f.btnEditData.Enabled);
        });
    }

    [StaFact]
    public void btnDisableHumanClick_NoFocus_IsNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.btnDisableHumanClick(f);
            Assert.False(_db.Human.SetEnabledResult && _db.Human.LastSetEnabledName != "");
        });
    }

    [StaFact]
    public void btnDisableHumanClick_ConfirmYes_SetsEnabledAndFlipsButtons()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", isDelete: 0);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDisableHumanClick(f);

            Assert.Equal("acc", _db.Human.LastSetEnabledAccount);
            Assert.Equal("r", _db.Human.LastSetEnabledName);
            Assert.Equal(2, _db.Human.LastSetEnabledValue);      // IsDelete or 2
            Assert.Equal(2, ((TSerarchRoleData)node.Data!).IsDelete);
            Assert.False(f.btnDisableHuman.Enabled);
            Assert.True(f.btnEnableHuman.Enabled);
            Assert.Contains("是否禁用人物 \"r\" ?", LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void btnDisableHumanClick_ConfirmNo_DoesNothing()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r");
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDNO;

            f.btnDisableHumanClick(f);

            Assert.Equal("", _db.Human.LastSetEnabledName);
            Assert.Equal(0, ((TSerarchRoleData)node.Data!).IsDelete);
        });
    }

    [StaTheory]
    [InlineData(true, 0)]
    [InlineData(false, 2)]
    public void btnDisableHumanClick_HeroOrAlreadyDisabled_Exits(bool hero, int isDelete)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", hero: hero, isDelete: isDelete);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDisableHumanClick(f);

            Assert.Equal("", _db.Human.LastSetEnabledName);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void btnDisableHumanClick_SetEnabledFails_NoStateChange()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r");
            f.vstRole.FocusedNode = node;
            _db.Human.SetEnabledResult = false;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDisableHumanClick(f);

            Assert.Equal(0, ((TSerarchRoleData)node.Data!).IsDelete);
            Assert.False(f.btnEnableHuman.Enabled);
        });
    }

    [StaFact]
    public void btnEnableHumanClick_ConfirmYes_ClearsBit2()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", isDelete: 3);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnEnableHumanClick(f);

            Assert.Equal(1, _db.Human.LastSetEnabledValue);   // 3 and (not 2) = 1
            Assert.Equal(1, ((TSerarchRoleData)node.Data!).IsDelete);
            Assert.True(f.btnDisableHuman.Enabled);
            Assert.False(f.btnEnableHuman.Enabled);
            Assert.Equal("是否启用人物 \"r\" ?", LoginSrvForms.LastMessage);
        });
    }

    [StaTheory]
    [InlineData(true, 3)]
    [InlineData(false, 0)]
    public void btnEnableHumanClick_HeroOrNotDisabled_Exits(bool hero, int isDelete)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", hero: hero, isDelete: isDelete);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnEnableHumanClick(f);

            Assert.Equal("", _db.Human.LastSetEnabledName);
        });
    }

    [StaFact]
    public void btnEnableHumanClick_ConfirmNo_DoesNothing()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", isDelete: 2);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDNO;

            f.btnEnableHumanClick(f);

            Assert.Equal(2, ((TSerarchRoleData)node.Data!).IsDelete);
        });
    }

    [StaFact]
    public void btnDeleteRoleClick_Human_UsesRoleTypeNameAndErases()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", hero: false);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDeleteRoleClick(f);

            Assert.Equal("acc", _db.Human.LastEraseAccount);
            Assert.Equal("r", _db.Human.LastEraseName);
            Assert.Contains("你确定要删除人物 \"r\" 吗?", LoginSrvForms.LastMessage);
            Assert.Equal(0, f.vstRole.RootNodeCount);   // DeleteNode
        });
    }

    [StaFact]
    public void btnDeleteRoleClick_Hero_UsesHeroTypeNameAndErases()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "h", hero: true);
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDeleteRoleClick(f);

            Assert.Equal("h", _db.Hero.LastEraseName);
            Assert.Contains("你确定要删除英雄 \"h\" 吗?", LoginSrvForms.LastMessage);
            Assert.Equal(0, f.vstRole.RootNodeCount);
        });
    }

    [StaFact]
    public void btnDeleteRoleClick_EraseFails_KeepsNode()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r");
            f.vstRole.FocusedNode = node;
            _db.Human.EraseResult = false;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;

            f.btnDeleteRoleClick(f);

            Assert.Equal(1, f.vstRole.RootNodeCount);
        });
    }

    [StaFact]
    public void btnDeleteRoleClick_ConfirmNo_DoesNothing()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r");
            f.vstRole.FocusedNode = node;
            LoginSrvForms.NextAnswer = LoginSrvForms.IDNO;

            f.btnDeleteRoleClick(f);

            Assert.Equal("", _db.Human.LastEraseName);
            Assert.Equal(1, f.vstRole.RootNodeCount);
        });
    }

    [StaFact]
    public void btnEditDataClick_Human_OpensEditorWithHumanId()
    {
        StaRunner.New(() =>
        {
            var shown = new List<string>();
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (id, hum, hero) => shown.Add($"h{id}:{hum != null}:{hero != null}");
            _db.Human.GetResult = true;
            _db.Human.GetId = 55;

            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r", hero: false);
            f.vstRole.FocusedNode = node;

            f.btnEditDataClick(f);

            Assert.Equal(new[] { "h55:True:False" }, shown);
        });
    }

    [StaFact]
    public void btnEditDataClick_Hero_OpensEditorWithHeroId()
    {
        StaRunner.New(() =>
        {
            var shown = new List<string>();
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (id, hum, hero) => shown.Add($"h{id}:{hum != null}:{hero != null}");
            _db.Hero.GetResult = true;
            _db.Hero.GetId = 66;

            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "h", hero: true);
            f.vstRole.FocusedNode = node;

            f.btnEditDataClick(f);

            Assert.Equal(new[] { "h66:False:True" }, shown);
        });
    }

    [StaFact]
    public void btnEditDataClick_GetFails_NoEditor()
    {
        StaRunner.New(() =>
        {
            int calls = 0;
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (_, _, _) => calls++;
            _db.Human.GetResult = false;

            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "r");
            f.vstRole.FocusedNode = node;

            f.btnEditDataClick(f);

            Assert.Equal(0, calls);
        });
    }

    [StaFact]
    public void btnEditDataClick_NoFocus_IsNoOp()
    {
        StaRunner.New(() =>
        {
            int calls = 0;
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (_, _, _) => calls++;
            _db.Human.GetResult = true;
            using var f = new TFrmDataManager();
            f.btnEditDataClick(f);
            Assert.Equal(0, calls);
        });
    }

    [StaFact]
    public void vstRoleNodeDblClick_NoHitNode_IsNoOp()
    {
        StaRunner.New(() =>
        {
            int calls = 0;
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (_, _, _) => calls++;
            using var f = new TFrmDataManager();
            f.vstRoleNodeDblClick(f.vstRole, null);
            Assert.Equal(0, calls);
        });
    }

    [StaFact]
    public void vstRoleNodeDblClick_Hero_OpensEditor()
    {
        StaRunner.New(() =>
        {
            var shown = new List<string>();
            LoginSrvRoleDb.ShowFrmRoleDataEdit = (id, hum, hero) => shown.Add($"h{id}");
            _db.Hero.GetResult = true;
            _db.Hero.GetId = 12;

            using var f = new TFrmDataManager();
            var node = f.vstRole.AddChild(null);
            node.Data = Role("acc", "h", hero: true);

            f.vstRoleNodeDblClick(f.vstRole, node);

            Assert.Equal(new[] { "h12" }, shown);
        });
    }

    [StaFact]
    public void ShowAskMsg_And_ShowInfoMsg_UseDelphiCaptions()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            LoginSrvForms.NextAnswer = LoginSrvForms.IDYES;
            Assert.True(f.ShowAskMsg("q"));
            Assert.Equal("询问", LoginSrvForms.LastCaption);

            f.ShowInfoMsg("i");
            Assert.Equal("提示", LoginSrvForms.LastCaption);
            Assert.Equal("i", LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void FormDestroy_ClearsRoleList()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmDataManager();
            f.FormDestroy(f);
            // FSerarchRoleList 已释放 → 再次搜索会 NRE（对应原文 use-after-free 语义）
            Assert.Throws<NullReferenceException>(() =>
            {
                f.edtAccount.Text = "a";
                f.btnSearchAccountClick(f);
            });
        });
    }
}

/// <summary>uFrmBatchEditAccountInfo.pas TFrmBatchEditAccountInfo 1:1 测试。</summary>
public sealed class BatchEditAccountInfoTests : IDisposable
{
    private readonly InMemoryAccountDb _db = new();
    private readonly string _dir;

    public BatchEditAccountInfoTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_batch_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        LoginSrvShare.ResetForTests();
        LoginSrvShare.g_AccountDB = _db;
        LoginSrvForms.MessageBoxHandler = (_, _, _) => LoginSrvForms.IDOK;
        LoginSrvForms.LastMessage = null;
        TFrmBatchEditAccountInfo.OpenFileProvider = null;
    }

    public void Dispose()
    {
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.LastMessage = null;
        TFrmBatchEditAccountInfo.OpenFileProvider = null;
        LoginSrvShare.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private void Seed(params (string name, int disable)[] rows)
    {
        foreach (var (name, disable) in rows)
        {
            GXX.Core.Protocol.TAccountInfo info = default;
            info.AccountNameStr = name;
            info.IsDisable = (byte)disable;
            _db.Store[name] = info;
        }
    }

    // ------------------------------------------------------------------
    // FormCreate / 树节点数据
    // ------------------------------------------------------------------

    private static TBatchEditNodeData D(TVirtualNode n) => (TBatchEditNodeData)n.Data!;

    private static TVirtualNode? Find(TVirtualStringTreeModel tree, string account)
    {
        foreach (var n in tree.Roots)
            if (D(n).AccountName == account) return n;
        return null;
    }

    private static string[] Names(TVirtualStringTreeModel tree)
    {
        var list = new List<string>();
        foreach (var n in tree.Roots) list.Add(D(n).AccountName);
        list.Sort(StringComparer.Ordinal);
        return list.ToArray();
    }

    [StaFact]
    public void FormCreate_SplitsAccountsIntoTwoTrees()
    {
        StaRunner.New(() =>
        {
            Seed(("a1", 0), ("a2", 1), ("a3", 0));
            using var f = new TFrmBatchEditAccountInfo();

            Assert.Equal(new[] { "a1", "a3" }, Names(f.vstEnableAccount));
            Assert.Equal(new[] { "a2" }, Names(f.vstDisableAccount));
            Assert.Equal(TCheckType.ctCheckBox, f.vstEnableAccount.Roots[0].CheckType);
            Assert.Equal(TCheckState.csUncheckedNormal, f.vstEnableAccount.Roots[0].CheckState);
        });
    }

    [StaFact]
    public void NodeDataAccountName_TruncatesToShortString14()
    {
        var d = new TBatchEditNodeData { AccountName = "0123456789ABCDEFGHIJ" };
        Assert.Equal("0123456789ABCD", d.AccountName);
    }

    [StaFact]
    public void vstEnableAccountGetText_ReturnsAccountName()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 0));
            using var f = new TFrmBatchEditAccountInfo();
            string cell = "";
            f.vstEnableAccountGetText(f.vstEnableAccount, f.vstEnableAccount.Roots[0], 0, ref cell);
            Assert.Equal("alice", cell);
        });
    }

    [StaTheory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    public void vstEnableAccountBeforeItemErase_OddIndexesGetZebraColor(int index, bool expectColor)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmBatchEditAccountInfo();
            var node = new TVirtualNode { Index = index };
            int color = 0;
            var action = TItemEraseAction.eaDefault;

            f.vstEnableAccountBeforeItemErase(f.vstEnableAccount, node, ref color, ref action);

            if (expectColor)
            {
                Assert.Equal(0x00FFFBF7, color);
                Assert.Equal(TItemEraseAction.eaColor, action);
            }
            else
            {
                Assert.Equal(0, color);
                Assert.Equal(TItemEraseAction.eaDefault, action);
            }
        });
    }

    [StaFact]
    public void btnSetDisableClick_MovesCheckedNodesAndCallsEnabledAccountsFalse()
    {
        StaRunner.New(() =>
        {
            Seed(("a1", 0), ("a2", 0));
            using var f = new TFrmBatchEditAccountInfo();
            Find(f.vstEnableAccount, "a1")!.CheckState = TCheckState.csCheckedNormal;

            f.btnSetDisableClick(f);

            Assert.Equal(new[] { "a1=0" }, _db.EnabledCalls);
            Assert.Equal(new[] { "a2" }, Names(f.vstEnableAccount));
            Assert.Equal(new[] { "a1" }, Names(f.vstDisableAccount));
            Assert.Equal("已成功禁用1个帐户", LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void btnSetDisableClick_NoneChecked_DoesNothing()
    {
        StaRunner.New(() =>
        {
            Seed(("a1", 0));
            using var f = new TFrmBatchEditAccountInfo();
            f.btnSetDisableClick(f);
            Assert.Empty(_db.EnabledCalls);
            Assert.Equal(1, f.vstEnableAccount.RootNodeCount);
        });
    }

    [StaFact]
    public void btnSetEnabledClick_MovesCheckedNodesAndCallsEnabledAccountsTrue()
    {
        StaRunner.New(() =>
        {
            Seed(("a1", 1));
            using var f = new TFrmBatchEditAccountInfo();
            Find(f.vstDisableAccount, "a1")!.CheckState = TCheckState.csCheckedNormal;

            f.btnSetEnabledClick(f);

            Assert.Equal(new[] { "a1=1" }, _db.EnabledCalls);
            Assert.Equal(new[] { "a1" }, Names(f.vstEnableAccount));
            Assert.Equal(0, f.vstDisableAccount.RootNodeCount);
            Assert.Equal("已成功解禁1个帐户", LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void btnSetEnabledClick_NoneChecked_DoesNothing()
    {
        StaRunner.New(() =>
        {
            Seed(("a1", 1));
            using var f = new TFrmBatchEditAccountInfo();
            f.btnSetEnabledClick(f);
            Assert.Empty(_db.EnabledCalls);
        });
    }

    [StaFact]
    public void edtEnabledAccountChange_EmptyText_MakesAllVisible()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 0), ("bob", 0));
            using var f = new TFrmBatchEditAccountInfo();
            f.vstEnableAccount.SetVisible(f.vstEnableAccount.Roots[0], false);

            f.edtEnabledAccount.Text = "";
            f.edtEnabledAccountChange(f);

            Assert.All(f.vstEnableAccount.Roots, n => Assert.True(n.Visible));
        });
    }

    [StaFact]
    public void edtEnabledAccountChange_FiltersBySubstring()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 0), ("bob", 0), ("alan", 0));
            using var f = new TFrmBatchEditAccountInfo();
            f.edtEnabledAccount.Text = "al";
            f.edtEnabledAccountChange(f);

            Assert.True(Find(f.vstEnableAccount, "alice")!.Visible);
            Assert.False(Find(f.vstEnableAccount, "bob")!.Visible);
            Assert.True(Find(f.vstEnableAccount, "alan")!.Visible);
        });
    }

    [StaFact]
    public void edtDisableAccountChange_FiltersBySubstring()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 1), ("bob", 1));
            using var f = new TFrmBatchEditAccountInfo();
            f.edtDisableAccount.Text = "bob";
            f.edtDisableAccountChange(f);

            Assert.False(Find(f.vstDisableAccount, "alice")!.Visible);
            Assert.True(Find(f.vstDisableAccount, "bob")!.Visible);

            f.edtDisableAccount.Text = "";
            f.edtDisableAccountChange(f);
            Assert.All(f.vstDisableAccount.Roots, n => Assert.True(n.Visible));
        });
    }

    /// <summary>
    /// ★ 差异断言：原文 `SL.IndexOf(name) > 0`（应为 &gt;= 0）→ **首行永远不被选中**；
    /// 且原文在 vstEnableAccount 循环里写的是 vstDisableAccount.IsVisible[Node]。
    /// </summary>
    [StaFact]
    public void btn2Click_ImportList_FirstLineNeverMatches()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 0), ("bob", 0), ("carol", 0));
            string file = Path.Combine(_dir, "list.txt");
            File.WriteAllText(file, "alice\r\nbob\r\n", GXX.Core.EncodingInit.GBK);
            TFrmBatchEditAccountInfo.OpenFileProvider = () => file;

            using var f = new TFrmBatchEditAccountInfo();
            f.btn2Click(f);

            // alice 在第 0 行 → IndexOf=0 → 不满足 > 0 → 未选中
            Assert.Equal(TCheckState.csUnCheckedNormal, Find(f.vstEnableAccount, "alice")!.CheckState);
            // bob 在第 1 行 → IndexOf=1 → 选中
            Assert.Equal(TCheckState.csCheckedNormal, Find(f.vstEnableAccount, "bob")!.CheckState);
            Assert.Equal(TCheckState.csUnCheckedNormal, Find(f.vstEnableAccount, "carol")!.CheckState);
            Assert.Equal("已成功导入并选中1个帐户", LoginSrvForms.LastMessage);
            Assert.Equal(1, f.vstEnableAccount.InvalidateCount);
            // 原文把 IsVisible 写到了 vstDisableAccount 上
            Assert.All(f.vstEnableAccount.Roots, n => Assert.True(n.Visible));
        });
    }

    [StaFact]
    public void btn2Click_Cancelled_DoesNothing()
    {
        StaRunner.New(() =>
        {
            Seed(("alice", 0));
            TFrmBatchEditAccountInfo.OpenFileProvider = () => null;
            using var f = new TFrmBatchEditAccountInfo();
            f.btn2Click(f);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void FormCreate_WithoutAccountDb_ProducesEmptyTrees()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_AccountDB = null;
            using var f = new TFrmBatchEditAccountInfo();
            Assert.Equal(0, f.vstEnableAccount.RootNodeCount);
            Assert.Equal(0, f.vstDisableAccount.RootNodeCount);
        });
    }
}
