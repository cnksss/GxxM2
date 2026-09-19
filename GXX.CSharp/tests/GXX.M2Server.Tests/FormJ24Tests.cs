using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J24：GameConfig.pas 巨片第三片（General 经验页/Msg/Time/Price 页）1:1 转换测试。</summary>
public sealed class GameConfigExpMsgTimePriceTests : IDisposable
{
    private readonly string _dir;
    private readonly GameConfigForm _form;

    public GameConfigExpMsgTimePriceTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j24_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGameSpeedDefaults();
        M2Config.ResetGameOptionDefaults();
        M2Config.ResetGameMsgTimeDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new GameConfigForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void Open_RefGameVarConf_And_MsgTimePrice()
    {
        StaRunner.New(() =>
        {
            Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001); // Open 前预置经验表
            M2Config.nSoftVersionDate = 20020522;
            M2Config.dwConsoleShowUserCountTime = 600000;
            M2Config.dwShowLineNoticeTime = 120000;
            M2Config.nLineNoticeColor = 2;
            M2Config.boSendOnlineCount = true;
            M2Config.dwSayMsgTime = 5000;
            M2Config.dwDisableSayMsgTime = 90000;
            M2Config.nStartCastleWarDays = 6;
            M2Config.dwCastleWarTime = 10800000;
            M2Config.nBuildGuildPrice = 2000000;

            _form.Open(showModal: false);

            Assert.Equal("20020522", _form.EditSoftVersionDate.Text);
            Assert.Equal(600, (int)_form.EditConsoleShowUserCountTime.Value); // div 1000
            Assert.Equal(120, (int)_form.EditShowLineNoticeTime.Value);
            Assert.Equal(2, _form.ComboBoxLineNoticeColor.SelectedIndex);
            Assert.True(_form.EditSendOnlineTime.Enabled); // SendOnlineCount 级联
            Assert.Equal(5, (int)_form.EditSayMsgTime.Value);
            Assert.Equal(90, (int)_form.EditDisableSayMsgTime.Value);
            Assert.Equal(6, (int)_form.EditStartCastleWarDays.Value);
            Assert.Equal(180, (int)_form.EditCastleWarTime.Value); // 10800000 div (60*1000)
            Assert.Equal(2000000, (int)_form.EditBuildGuildPrice.Value);
            Assert.Equal("100", _form.GridLevelExp.Rows[0].Cells[1].Value?.ToString()); // Open 前预置 Level1=100
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void ButtonGeneralSave_VersionValidation_ThenWrite()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditKillMonExpMultiple.Value = 3;
            _form.EditKillMonExpMultipleChange(_form);
            _form.EditSoftVersionDate.Text = "abc";
            _form.ButtonGeneralSaveClick(_form);
            Assert.Equal("客户端版号设置错误！", M2Forms.LastMessage);
            Assert.True(_form.ButtonGeneralSave.Enabled); // Exit 未落盘

            _form.EditSoftVersionDate.Text = "20020522";
            _form.CheckBoxGetAllNpcTaxClick(_form); // 任意 ModValue 保持保存钮可用
            _form.ButtonGeneralSaveClick(_form);
            Assert.Equal(20020522, M2ShareState.ConfigIni.ReadInteger("Setup", "SoftVersionDate", -1));
            Assert.Equal(3, (int)M2Config.dwKillMonExpMultiple); // 仅写配置，不落盘（归 ExpSave）
            Assert.False(_form.ButtonGeneralSave.Enabled);
        });
    }

    [Fact]
    public void ComboBoxLevelExp_StdPlan()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001);
            _form.ComboBoxLevelExp.SelectedIndex = 1; // 标准经验值（SelectedIndexChanged 触发处理器）
            Assert.True(_form.IsModValued);           // IDNO（MessageBoxHandler 默认 IDOK≠IDNO）→ 应用
            Assert.Equal(100u, M2Config.dwNeedExps[1]);           // 1..26 保持原始表
            Assert.Equal(M2Config.OldNeedExps[26], M2Config.dwNeedExps[26]);
            Assert.Equal(4000000u, M2Config.dwNeedExps[27]);      // 4000000000 div 1000 * 1
            Assert.Equal(296000000u, M2Config.dwNeedExps[100]);   // *74（26+74=100）
            Assert.Equal(3896000000u, M2Config.dwNeedExps[1000]); // *974（26+974=1000）
        });
    }

    [Fact]
    public void ComboBoxLevelExp_MultPlans_Sequential()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001);
            M2Config.dwNeedExps[1] = 100;
            M2Config.dwNeedExps[5] = 1;
            M2Forms.NextAnswer = M2Forms.IDNO; // 先验证拒绝路径：表不变
            _form.ComboBoxLevelExp.SelectedIndex = 2;
            Assert.Equal(100u, M2Config.dwNeedExps[1]);

            M2Forms.NextAnswer = M2Forms.IDYES; // 当前1/5倍
            _form.ComboBoxLevelExp.SelectedIndex = 3;
            Assert.Equal(20u, M2Config.dwNeedExps[1]); // 100 div 5
            Assert.Equal(1u, M2Config.dwNeedExps[5]);  // div 5 = 0 → 1

            M2Forms.NextAnswer = M2Forms.IDYES; // 当前1/60倍（在 1/5 基础上顺序应用）
            _form.ComboBoxLevelExp.SelectedIndex = 10;
            Assert.Equal(1u, M2Config.dwNeedExps[1]); // 20 div 60 = 0 → 1
        });
    }

    [Fact]
    public void ButtonExpSave_ZeroValidation_ThenWriteExpsIni()
    {
        StaRunner.New(() =>
        {
            Array.Copy(M2Config.OldNeedExps, M2Config.dwNeedExps, 1001); // Open 前预置有效表
            _form.Open(showModal: false);
            _form.EditKillMonExpMultiple.Value = 2;
            _form.EditKillMonExpMultipleChange(_form);
            _form.GridLevelExp.Rows[6].Cells[1].Value = "0"; // 等级 7 非法
            _form.ButtonExpSaveClick(_form);
            Assert.Equal("等级 7 升级经验设置错误！", M2Forms.LastMessage);
            Assert.True(_form.ButtonExpSave.Enabled);

            _form.GridLevelExp.Rows[6].Cells[1].Value = "1700";
            _form.ButtonExpSaveClick(_form);
            Assert.Equal(2, M2ShareState.ExpConfigIni.ReadInteger("Exp", "KillMonExpMultiple", -1));
            Assert.Equal(100, M2ShareState.ExpConfigIni.ReadInteger("Exp", "Level1", -1));
            Assert.Equal(1700, M2ShareState.ExpConfigIni.ReadInteger("Exp", "Level7", -1));
            Assert.Equal(10, M2ShareState.ExpConfigIni.ReadInteger("Exp", "MaxUpLevelCount", -1));
            Assert.Equal(0, M2ShareState.ExpConfigIni.ReadInteger("Exp", "LevelExpRate1", -1));
            Assert.False(_form.ButtonExpSave.Enabled);
        });
    }

    [Fact]
    public void MsgHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditSayMsgTime.Value = 4;
            _form.EditSayMsgTimeChange(_form);
            Assert.Equal(4000, (int)M2Config.dwSayMsgTime);
            _form.EditSayMsgMaxLen.Value = 100;
            _form.EditSayMsgMaxLenChange(_form); // 置 boSendServerConfig
            _form.EditGMRedMsgCmd.Text = "!!";
            _form.EditGMRedMsgCmdChange(_form);
            Assert.Equal('!', M2ShareState.g_GMRedMsgCmd);

            _form.EditShowWhisperLevelMsg.Text = "没有占位符";
            _form.ButtonMsgSaveClick(_form);
            Assert.Equal("私聊后缀信息设置错误！", M2Forms.LastMessage);
            Assert.True(_form.ButtonMsgSave.Enabled);

            _form.EditShowWhisperLevelMsg.Text = "[Lv%u]";
            _form.ButtonMsgSaveClick(_form);
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "SayMsgMaxLen", -1));
            Assert.Equal(4000, M2ShareState.ConfigIni.ReadInteger("Setup", "SayMsgTime", -1));
            Assert.Equal("!", M2ShareState.CommandConfIni.ReadString("Command", "GMRedMsgCmd", ""));
            Assert.Equal("[Lv%u]", M2ShareState.StringConfIni.ReadString("String", "ShowWhisperLevelMsg", ""));
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // boSendServerConfig 置位后下发
            Assert.False(_form.ButtonMsgSave.Enabled);
        });
    }

    [Fact]
    public void TimeHandlers_Scale_AndSave_WithDearRecallQuirk()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditCastleWarTime.Value = 100;
            _form.EditCastleWarTimeChange(_form);
            Assert.Equal(6000000, (int)M2Config.dwCastleWarTime);
            M2Config.dwDearRecallTime = 10;
            M2Config.dwMasterRecallTime = 50;
            _form.ButtonTimeSaveClick(_form);
            Assert.Equal(4, M2ShareState.ConfigIni.ReadInteger("Setup", "StartCastleWarDays", -1));
            Assert.Equal(6000000, M2ShareState.ConfigIni.ReadInteger("Setup", "CastleWarTime", -1)); // 100 分 ×60000
            Assert.Equal(600000, M2ShareState.ConfigIni.ReadInteger("Setup", "GetCastleTime", -1));
            // Delphi 原文第二行重复写 'DearRecallTime' 键但取 dwMasterRecallTime 值
            Assert.Equal(50, M2ShareState.ConfigIni.ReadInteger("Setup", "DearRecallTime", -1));
            Assert.Equal(300, M2ShareState.ConfigIni.ReadInteger("Setup", "NpcButtonClickTime", -1));
            Assert.False(_form.ButtonTimeSave.Enabled);
        });
    }

    [Fact]
    public void PriceHandlers_Write_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditBuildGuildPrice.Value = 1500000;
            _form.EditBuildGuildPriceChange(_form);
            Assert.Equal(1500000, (int)M2Config.nBuildGuildPrice);
            _form.ButtonPriceSaveClick(_form);
            Assert.Equal(1500000, M2ShareState.ConfigIni.ReadInteger("Setup", "BuildGuild", -1));
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "MakeDurg", -1));
            Assert.Equal(30000, M2ShareState.ConfigIni.ReadInteger("Setup", "GuildWarFee", -1));
            Assert.Equal(3, M2ShareState.ConfigIni.ReadInteger("Setup", "SuperRepairPriceRate", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "RepairItemDecDura", -1));
            Assert.False(_form.ButtonPriceSave.Enabled);
        });
    }
}
