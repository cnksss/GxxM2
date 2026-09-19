using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J4b：ViewOnlineHuman.pas TfrmViewOnlineHuman 1:1 转换测试（在线玩家/英雄 19 列表）。
/// </summary>
public sealed class FormViewOnlineHumanTests : IDisposable
{
    private readonly TUserEngine _engine = new();

    public FormViewOnlineHumanTests()
    {
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        M2ShareGlobals.g_SellPlayerList.Clear();
        _engine.PlayObjects.Clear();
        _engine.HeroObjects.Clear();
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        M2ShareGlobals.g_SellPlayerList.Clear();
    }

    private static TPlayObject MakePlayer(string name, string userId, string ip, string ipLocal, int perm,
        int gold, int point, int payPoint, int diamond, int gird, bool dummy, bool offline)
    {
        return new TPlayObject
        {
            m_sCharName = name,
            m_sUserID = userId,
            m_sIPaddr = ip,
            m_sIPLocal = ipLocal,
            m_btPermission = perm,
            m_nGameGold = gold,
            m_nGamePoint = point,
            m_nPayMentPoint = payPoint,
            m_nGameDiamond = diamond,
            m_nGameGird = gird,
            m_boDummyObject = dummy,
            m_boOffLine = offline,
            m_btGender = 1,
            m_btJob = 2,
            m_wAbil = { Level = 42 },
            m_sMapName = "3",
            m_nCurrX = 100,
            m_nCurrY = 200
        };
    }

    [StaFact]
    public void GetOnlineList_FiltersByCheckbox()
    {
        var human = MakePlayer("玩家甲", "acc1", "1.1.1.1", "北京", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        var dummy = MakePlayer("假人乙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: true, offline: false);
        var hero = MakePlayer("英雄丙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        hero.m_btRace = Grobal2Const.RC_HEROOBJECT;
        _engine.PlayObjects.Add(human);
        _engine.PlayObjects.Add(dummy);
        _engine.HeroObjects.Add(hero);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.GetOnlineList(); // 默认仅玩家
            Assert.Equal(1, form.ViewListCount);
            Assert.Same(human, form.GetViewObject(0));

            form.chkDummy.Checked = true;
            form.GetOnlineList();
            Assert.Equal(2, form.ViewListCount);

            form.chkHumanHero.Checked = true;
            form.GetOnlineList();
            Assert.Equal(3, form.ViewListCount);
        });
    }

    [StaFact]
    public void RefGridSession_Fills19Columns_PlayerAndHero()
    {
        var master = MakePlayer("主人甲", "acc0", "8.8.8.8", "上海", 5, 0, 0, 0, 0, 0, dummy: false, offline: false);
        var hero = MakePlayer("英雄乙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        hero.m_btRace = Grobal2Const.RC_HEROOBJECT;
        hero.m_Master = master;
        var player = MakePlayer("玩家丙", "acc2", "9.9.9.9", "广州", 2, 100, 200, 300, 400, 500, dummy: false, offline: true);
        player.m_sAutoSendMsg = "自动回复内容";
        _engine.PlayObjects.Add(master);
        _engine.PlayObjects.Add(player);
        _engine.HeroObjects.Add(hero);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.chkHumanHero.Checked = true;
            form.Open(showModal: false);

            // 行序：英雄先（HeroObjects 先入 ViewList）→ 玩家两行
            Assert.Equal(3, form.GridHuman.RowCount);
            Assert.Equal("英雄乙", form.GridHuman[1, 0].Value);
            Assert.Equal("玩家英雄", form.GridHuman[2, 0].Value);
            Assert.Equal("女", form.GridHuman[3, 0].Value);
            Assert.Equal("道士", form.GridHuman[4, 0].Value);
            Assert.Equal("8.8.8.8", form.GridHuman[9, 0].Value);   // 主人 IP
            Assert.Equal("5", form.GridHuman[10, 0].Value);        // 主人权限
            Assert.Equal("上海", form.GridHuman[11, 0].Value);     // 主人地区
            Assert.Equal("否", form.GridHuman[17, 0].Value);        // 英雄离线列恒 '否'（BoolStr[False]）
            Assert.Equal("是", form.GridHuman[17, 2].Value);       // 玩家丙离线挂机
            Assert.Equal("acc2", form.GridHuman[8, 2].Value);
            Assert.Equal("100", form.GridHuman[12, 2].Value);
            Assert.Equal("500", form.GridHuman[16, 2].Value);
            Assert.Equal("自动回复内容", form.GridHuman[18, 2].Value);
        });
    }

    [StaFact]
    public void SortOnlineList_ByTypeThenLevel()
    {
        var dummyPlayer = MakePlayer("假人", "", "", "", 0, 0, 0, 0, 0, 0, dummy: true, offline: false);
        var playerLow = MakePlayer("玩家低", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        playerLow.m_wAbil.Level = 10;
        var playerHigh = MakePlayer("玩家高", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        playerHigh.m_wAbil.Level = 99;
        _engine.PlayObjects.Add(playerHigh);
        _engine.PlayObjects.Add(dummyPlayer);
        _engine.PlayObjects.Add(playerLow);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.chkDummy.Checked = true;
            form.GetOnlineList();
            form.SortOnlineList(1); // 类型：玩家0/假人1
            Assert.Same(playerHigh, form.GetViewObject(0));
            Assert.Same(playerLow, form.GetViewObject(1));
            Assert.Same(dummyPlayer, form.GetViewObject(2));

            form.SortOnlineList(4); // 等级（字符串排序 1:1：10 < 42 < 99）
            Assert.Same(playerLow, form.GetViewObject(0));
        });
    }

    [StaFact]
    public void ButtonSearch_EmptyName_Found_Miss()
    {
        var player = MakePlayer("目标玩家", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        _engine.PlayObjects.Add(player);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.Open(showModal: false);

            form.EditSearchName.Text = "";
            form.ButtonSearchClick(form);
            Assert.Equal("请输入一个角色名称！", M2Forms.LastMessage);

            form.EditSearchName.Text = "目标玩家";
            form.ButtonSearchClick(form);
            Assert.Equal(1, form.GridSelectedRow); // Row := I + 1

            form.EditSearchName.Text = "不在线的人";
            form.ButtonSearchClick(form);
            Assert.Equal("角色没有在线！", M2Forms.LastMessage);
        });
    }

    [StaFact]
    public void ShowHumanInfo_SelectionGuardAndHandler()
    {
        var player = MakePlayer("选中玩家", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        _engine.PlayObjects.Add(player);
        TCreature? received = null;
        int callCount = 0;

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.ShowHumanInfoHandler = obj => { received = obj; callCount++; return 0; };
            form.Open(showModal: false);

            form.GridSelectedRow = -1;
            form.ShowHumanInfo();
            Assert.Equal("请先选择一个要查看的角色！", M2Forms.LastMessage);

            form.GridSelectedRow = 1; // 行 1 → ViewList[0]
            form.ShowHumanInfo();
            Assert.Equal(1, callCount);
            Assert.Same(player, received);
        });
    }

    [StaFact]
    public void ButtonKickPlayOffLine_SkipsSellList()
    {
        var offline1 = MakePlayer("挂机甲", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: true);
        var offline2 = MakePlayer("寄售乙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: true);
        var online = MakePlayer("在线丙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        _engine.PlayObjects.Add(offline1);
        _engine.PlayObjects.Add(offline2);
        _engine.PlayObjects.Add(online);
        M2ShareGlobals.g_SellPlayerList.Add("寄售乙"); // 寄售名单不踢

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.Open(showModal: false);
            form.ButtonKickPlayOffLineClick(form);

            Assert.True(offline1.m_boGhost);
            Assert.False(offline2.m_boGhost); // 在寄售名单 → 跳过
            Assert.False(online.m_boGhost);
        });
    }

    [StaFact]
    public void ButtonKickDummyObject_OnlyDummies()
    {
        var dummy = MakePlayer("假人甲", "", "", "", 0, 0, 0, 0, 0, 0, dummy: true, offline: false);
        var human = MakePlayer("玩家乙", "", "", "", 0, 0, 0, 0, 0, 0, dummy: false, offline: false);
        _engine.PlayObjects.Add(dummy);
        _engine.PlayObjects.Add(human);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.Open(showModal: false);
            form.ButtonKickDummyObjectClick(form);
            Assert.True(dummy.m_boGhost);
            Assert.False(human.m_boGhost);
        });
    }

    [StaFact]
    public void CheckboxToggle_RefreshesList()
    {
        var dummy = MakePlayer("假人甲", "", "", "", 0, 0, 0, 0, 0, 0, dummy: true, offline: false);
        _engine.PlayObjects.Add(dummy);

        StaRunner.New(() =>
        {
            using var form = new ViewOnlineHumanForm(_engine);
            form.Open(showModal: false);
            Assert.Equal(1, form.GridHuman.RowCount); // 默认不显示假人
            form.chkDummy.Checked = true;
            form.chkDummyClick(form);
            Assert.Equal(1, form.GridHuman.RowCount);
            Assert.Equal("假人甲", form.GridHuman[1, 0].Value);
        });
    }
}
