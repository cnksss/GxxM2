using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J27：FunctionConfig.pas 巨片第四片（Master 拜师/MakeMine 挖矿/WinLottery 赌博页）1:1 测试。</summary>
public sealed class FunctionConfigMineLotteryTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigMineLotteryTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j27_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetFunctionMineDefaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new FunctionConfigForm();
            f.GetMonRaceHandler = _ => 5;
            return f;
        });
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
    public void Open_RefMasterMineLottery()
    {
        StaRunner.New(() =>
        {
            M2Config.nMasterOKLevel = 40;
            M2Config.nGoldStoneMax = 5;
            M2Config.nWinLottery1Gold = 2000000;

            _form.Open(showModal: false);

            Assert.Equal(40, (int)_form.EditMasterOKLevel.Value);
            Assert.Equal(5, (int)_form.EditGoldStoneMax.Value);
            Assert.Equal(2000000, (int)_form.EditWinLottery1Gold.Value);
            Assert.Equal(4, (int)_form.EditMakeMineHitRate.Value); // 其余默认回显
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void MasterHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMasterOKLevel.Value = 35;
            _form.EditMasterOKLevelChange(_form);
            _form.EditMasterOKCreditPoint.Value = 10;
            _form.EditMasterOKCreditPointChange(_form);
            _form.EditMasterOKBonusPoint.Value = 20;
            _form.EditMasterOKBonusPointChange(_form);
            Assert.Equal(35, (int)M2Config.nMasterOKLevel);
            Assert.Equal(10, (int)M2Config.nMasterOKCreditPoint);
            Assert.Equal(20, (int)M2Config.nMasterOKBonusPoint);

            _form.ButtonMasterSaveClick(_form);
            Assert.Equal(35, M2ShareState.ConfigIni.ReadInteger("Setup", "MasterOKLevel", -1));
            Assert.Equal(10, M2ShareState.ConfigIni.ReadInteger("Setup", "MasterOKCreditPoint", -1));
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "MasterOKBonusPoint", -1));
            Assert.Equal(5, M2ShareState.ConfigIni.ReadInteger("Setup", "MasterCount", -1));
            Assert.False(_form.ButtonMasterSave.Enabled);
        });
    }

    [Fact]
    public void MineHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMakeMineHitRate.Value = 6;
            _form.EditMakeMineHitRateChange(_form);
            _form.EditBlackStoneMax.Value = 60;
            _form.EditBlackStoneMaxChange(_form);
            Assert.Equal(6, (int)M2Config.nMakeMineHitRate);
            Assert.Equal(60, (int)M2Config.nBlackStoneMax);

            _form.ButtonMakeMineSaveClick(_form);
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "MakeMineHitRate", -1));
            Assert.Equal(12, M2ShareState.ConfigIni.ReadInteger("Setup", "MakeMineRate", -1));
            Assert.Equal(120, M2ShareState.ConfigIni.ReadInteger("Setup", "StoneTypeRate", -1));
            Assert.Equal(60, M2ShareState.ConfigIni.ReadInteger("Setup", "BlackStoneMax", -1));
            Assert.Equal(3000, M2ShareState.ConfigIni.ReadInteger("Setup", "StoneMinDura", -1));
            Assert.Equal(10000, M2ShareState.ConfigIni.ReadInteger("Setup", "StoneAddDuraMax", -1));
            Assert.False(_form.ButtonMakeMineSave.Enabled);
        });
    }

    [Fact]
    public void WinLotteryGoldHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditWinLottery3Gold.Value = 150000;
            _form.EditWinLottery3GoldChange(_form);
            Assert.Equal(150000, (int)M2Config.nWinLottery3Gold);
            Assert.True(_form.ButtonWinLotterySave.Enabled);
        });
    }

    [Fact]
    public void WinLotteryRateScroll_CascadesAndSaves()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ScrollBarWinLotteryRate.Value = 40000;
            _form.ScrollBarWinLotteryRateChange(_form);
            Assert.Equal(40000, (int)M2Config.nWinLotteryRate);
            Assert.Equal("40000", _form.EditWinLotteryRate.Text);
            Assert.Equal(40000, (int)_form.ScrollBarWinLottery1Max.Maximum); // 级联上限

            _form.ScrollBarWinLottery1Max.Value = 17000;
            _form.ScrollBarWinLottery1MaxChange(_form);
            Assert.Equal(17000, (int)M2Config.nWinLottery1Max);
            Assert.Equal("16180-17000", _form.EditWinLottery1Max.Text); // Min-Max 原文格式

            _form.ButtonWinLotterySaveClick(_form);
            Assert.Equal(40000, M2ShareState.ConfigIni.ReadInteger("Setup", "WinLotteryRate", -1));
            Assert.Equal(17000, M2ShareState.ConfigIni.ReadInteger("Setup", "WinLottery1Max", -1));
            Assert.Equal(1000000, M2ShareState.ConfigIni.ReadInteger("Setup", "WinLottery1Gold", -1));
            Assert.Equal(500, M2ShareState.ConfigIni.ReadInteger("Setup", "WinLottery6Gold", -1));
            Assert.False(_form.ButtonWinLotterySave.Enabled);
        });
    }

    [Fact]
    public void ButtonWinLotteryDefaulf_IDYES_Restores()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.nWinLottery1Gold = 9;
            M2Config.nWinLottery1Max = 17000;
            M2Forms.NextAnswer = M2Forms.IDNO;
            _form.ButtonWinLotteryDefaulfClick(_form);
            Assert.Equal(9, (int)M2Config.nWinLottery1Gold);

            M2Forms.NextAnswer = M2Forms.IDYES;
            _form.ButtonWinLotteryDefaulfClick(_form);
            Assert.Equal(1000000, (int)M2Config.nWinLottery1Gold);
            Assert.Equal(500, (int)M2Config.nWinLottery6Gold);
            Assert.Equal(16185, (int)M2Config.nWinLottery1Max);
            Assert.Equal(30000, (int)M2Config.nWinLotteryRate);
            Assert.Equal("1000000", _form.EditWinLottery1Gold.Text); // RefWinLottery 回显
        });
    }
}
