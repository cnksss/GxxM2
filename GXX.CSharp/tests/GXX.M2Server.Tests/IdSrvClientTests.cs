using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J13：IdSrvClient.pas TFrmIDSoc 会话管理全量移植测试。</summary>
public sealed class IdSrvClientTests
{
    public IdSrvClientTests()
    {
        IdSocState.FrmIDSoc.m_SessionList.Clear();
        IdSrvClient.Reset();
    }

    private static string Md5Of(int n) => (n.ToString("D32") + "0000000000000000000000000000")[..32];

    [Fact]
    public void NewSession_CreatesAndRefreshes()
    {
        IdSrvClient.NewSession("acc1", "127.0.0.1", 1001, 1, 2);
        Assert.Equal(1, IdSocState.FrmIDSoc.m_SessionList.Count);
        var s = IdSocState.FrmIDSoc.m_SessionList[0];
        Assert.False(s.boClose);
        Assert.Equal(1, s.nRefCount);

        // 同帐号再次 NewSession → 刷新而非新建
        IdSrvClient.NewSession("acc1", "192.168.1.9", 2002, 0, 3);
        Assert.Equal(1, IdSocState.FrmIDSoc.m_SessionList.Count);
        Assert.Equal("192.168.1.9", IdSocState.FrmIDSoc.m_SessionList[0].sIPaddr);
        Assert.Equal(2002, IdSocState.FrmIDSoc.m_SessionList[0].nSessionID);
    }

    [Fact]
    public void GetPasswdSuccess_ParsesFiveParts()
    {
        IdSrvClient.GetPasswdSuccess("acc2/3003/1/2/10.0.0.8");
        var s = IdSocState.FrmIDSoc.m_SessionList[0];
        Assert.Equal("acc2", s.sAccount);
        Assert.Equal(3003, s.nSessionID);
        Assert.Equal(1, s.nPayMent);
        Assert.Equal(2, s.nPayMode);
        Assert.Equal("10.0.0.8", s.sIPaddr);
    }

    [Fact]
    public void SendHumanLogOutMsg_MarksClosedAndSends()
    {
        IdSrvClient.NewSession("acc3", "1.1.1.1", 5001, 0, 0);
        M2Config.sServerName = "srv";
        M2ShareState.nServerIndex = 0;
        IdSrvClient.SendHumanLogOutMsg("acc3", 5001);
        Assert.True(IdSocState.FrmIDSoc.m_SessionList[0].boClose);
        Assert.Single(IdSrvClient.SentMessages);
        Assert.Contains("(2102/acc3/5001)", IdSrvClient.SentMessages[0]);
    }

    [Fact]
    public void DelSession_RemovesAndKicks()
    {
        string? kickedAccount = null;
        int? kickedSession = null;
        IdSrvClient.KickUserHandler = (acc, sid) => { kickedAccount = acc; kickedSession = sid; };

        IdSrvClient.NewSession("acc4", "2.2.2.2", 6001, 0, 0);
        IdSrvClient.DelSession(6001);
        Assert.Equal(0, IdSocState.FrmIDSoc.m_SessionList.Count);
        Assert.Equal("acc4", kickedAccount);
        Assert.Equal(6001, kickedSession);
    }

    [Fact]
    public void GetAdmission_PaymentConversion_AndClosedSkip()
    {
        IdSrvClient.NewSession("acc5", "3.3.3.3", 7001, 1, 5); // nPayMent=1 → 折算 2

        var found = IdSrvClient.GetAdmission("acc5", "3.3.3.3", 7001, out int payMode, out int payMent);
        Assert.NotNull(found);
        Assert.Equal(2, payMent);
        Assert.Equal(5, payMode);

        // 关闭会话后再取 → 命中 boClose 但不返回
        IdSrvClient.SendHumanLogOutMsg("acc5", 7001);
        var closed = IdSrvClient.GetAdmission("acc5", "3.3.3.3", 7001, out _, out _);
        Assert.Null(closed);
    }

    [Fact]
    public void GetAdmissionEx_MatchesByAccountOnly()
    {
        IdSrvClient.NewSession("acc6", "4.4.4.4", 8001, 2, 7);
        var found = IdSrvClient.GetAdmissionEx("acc6", "5.5.5.5", out int sessionId, out int payMode, out int payMent);
        Assert.NotNull(found);
        Assert.Equal(8001, sessionId);
        Assert.Equal(3, payMent); // nPayMent=2 → 折算 3
        Assert.Equal(7, payMode);
    }

    [Fact]
    public void Run_DispatchesSessionFrames()
    {
        IdSrvClient.Run("(100/acc7/9001/0/1/6.6.6.6)(104/55)");
        Assert.Equal(1, IdSocState.FrmIDSoc.m_SessionList.Count);
        Assert.Equal("acc7", IdSocState.FrmIDSoc.m_SessionList[0].sAccount);
        Assert.Equal(9001, IdSocState.FrmIDSoc.m_SessionList[0].nSessionID);
        Assert.Equal(55, IdSrvClient.g_nTotalHumCount);
    }

    [Fact]
    public void ClearSession_EmptiesAll()
    {
        IdSrvClient.NewSession("a", "1.1.1.1", 1, 0, 0);
        IdSrvClient.NewSession("b", "1.1.1.2", 2, 0, 0);
        IdSrvClient.ClearSession();
        Assert.Equal(0, IdSocState.FrmIDSoc.m_SessionList.Count);
    }
}

/// <summary>批次J13：HumanInfo 属性点编辑接 RecalcAdjusBonus 测试。</summary>
public sealed class HumanInfoBonusPointTests
{
    [Fact]
    public void SaveClick_BonusPointFeedsRecalcAdjusBonus()
    {
        M2Config.ResetServerValueDefaults();
        M2ShareAbilConfig.ResetDefaults();
        // 战士裸体 DC 成长 1/1 → BonusTick.DC=1；已分配 4 点 DC → adc=4 → DC2 + 3/lo + 1
        M2Config.NakedAbilofWarr.DC = 0x00010001;
        M2Config.BonusAbilofWarr.DC = 1;
        M2Config.NakedAbilofWarr.HP = 10;
        M2Config.BonusAbilofWarr.HP = 50;

        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        player.m_wAbil.DC2 = 1; // RecalcLevelAbilitys 战士 1 级基础 DC2 = max(1, 0) = 1
        player.m_BonusAbil.DC = 4; // 已分配 4 点 DC（adc = 4/1 = 4）

        StaRunner.New(() =>
        {
            using var form = new HumanInfoForm { BaseObject = player };
            form.Open(showModal: false);
            form.EditLevel.Text = "1";
            form.EditBonusPoint.Text = "4";
            form.EditGold.Text = "0";
            form.EditPKPoint.Text = "0";
            form.EditGameGold.Text = "0";
            form.EditGamePoint.Text = "0";
            form.seGameDiamond.Text = "0";
            form.seGameGird.Text = "0";
            form.EditCreditPoint.Text = "0";
            form.EditGameGlory.Text = "0";
            Assert.True(form.ButtonSaveClick(form));

            // m_BonusAbil.BonusPoint 已写，RecalcAdjusBonus 按 BonusAbilofWarr 配置折算
            Assert.Equal(4, player.m_BonusAbil.BonusPoint);
            // naked.DC=1/1，BonusTick.DC=1 → adc=4 → AdjustAb2(1/1, 4)：3 点进上限、1 点进下限
            Assert.Equal(4, player.m_wAbil.DC2);
        });
    }
}
