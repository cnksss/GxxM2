using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J6：HumanInfo.pas TfrmHumanInfo 1:1 核心转换测试。
/// 全场景合并于单一 STA 测试体（此类多测试并行收集存在框架层挂起，单测试体为稳定等价形态）。
/// </summary>
public sealed class FormHumanInfoTests
{
    private static TPlayObject MakePlayer()
    {
        return new TPlayObject
        {
            m_sCharName = "查看对象",
            m_btRace = Grobal2Const.RC_PLAYOBJECT,
            m_wAbil = { Level = 42 },
            m_nGold = 12345,
            m_nPKPOINT = 100,
            m_nGameGold = 50,
            m_nGamePoint = 60,
            m_nGameDiamond = 7,
            m_nGameGird = 8,
            m_nGameGlory = 9,
            m_nBonusPoint = 10,
            m_sAutoSendMsg = "回复内容"
        };
    }

    [StaFact]
    public void HumanInfo_AllCoreScenarios()
    {
        StaRunner.New(() =>
        {
            // ---- 场景1：RefHumanInfo 核心字段回填 ----
            var player = MakePlayer();
            using (var form = new HumanInfoForm { BaseObject = player })
            {
                form.Open(showModal: false);
                Assert.Equal("42", form.EditLevel.Text);
                Assert.Equal("12345", form.EditGold.Text);
                Assert.Equal("100", form.EditPKPoint.Text);
                Assert.Equal("50", form.EditGameGold.Text);
                Assert.Equal("回复内容", form.EditSayMsg.Text);
                Assert.True(form.ButtonKick.Enabled);

                // ---- 场景2：合法保存写回 ----
                form.EditLevel.Text = "60";
                form.EditGold.Text = "999";
                form.EditPKPoint.Text = "0";
                form.EditGameGold.Text = "500";
                Assert.True(form.ButtonSaveClick(form));
                Assert.Equal(60u, player.m_wAbil.Level);
                Assert.Equal(999u, player.m_nGold);
                Assert.Equal(0, player.m_nPKPOINT);
                Assert.Equal(500, player.m_nGameGold);

                // ---- 场景3：非法 PK 值拒绝 ----
                form.EditPKPoint.Text = "2000001";
                M2Forms.NextAnswer = M2Forms.IDOK;
                Assert.False(form.ButtonSaveClick(form));
                Assert.Equal("输入数据不正确！", M2Forms.LastMessage);
                Assert.Equal(0, player.m_nPKPOINT); // 校验前值（场景2 已写 0）

                // ---- 场景4：负属性点拒绝（再次弹窗，需再次注入应答） ----
                M2Forms.NextAnswer = M2Forms.IDOK;
                form.EditBonusPoint.Text = "-1";
                Assert.False(form.ButtonSaveClick(form));
                Assert.Equal(10, player.m_nBonusPoint);

                // ---- 场景5：监视开关使能联动 ----
                form.CheckBoxMonitor.Checked = true;
                form.CheckBoxMonitorClick(form);
                Assert.False(form.ButtonSave.Enabled);
                form.CheckBoxMonitor.Checked = false;
                form.CheckBoxMonitorClick(form);
                Assert.True(form.ButtonSave.Enabled);

                // ---- 场景6：踢下线（紧急关闭标记） ----
                form.ButtonKickClick(form);
                Assert.True(player.m_boEmergencyClose);
                Assert.False(player.m_boOffLine);
                Assert.False(player.m_boPlayOffLine);
                Assert.False(form.ButtonKick.Enabled);
            }

            // ---- 场景7：Ghost → 状态'下线'并清引用 ----
            var player2 = MakePlayer();
            using (var form2 = new HumanInfoForm { BaseObject = player2 })
            {
                form2.Open(showModal: false);
                player2.m_boGhost = true;
                form2.TimerTimer(form2);
                Assert.Equal("下线", form2.EditHumanStatus.Text);
                Assert.Null(form2.BaseObject);
            }

            M2Forms.NextAnswer = null;
        });
    }
}
