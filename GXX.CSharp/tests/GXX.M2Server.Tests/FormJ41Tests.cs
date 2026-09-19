using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J41：MonsterConfig.pas 自定义怪编辑树（uCustomMonsterUtils 数据层 + vstCustomMonster/vstAction）1:1 测试。</summary>
public sealed class MonsterConfigCustomMonsterTests : IDisposable
{
    private readonly string _dir;
    private readonly MonsterConfigForm _form;

    public MonsterConfigCustomMonsterTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j41_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetMonsterConfigSliceDefaults();
        M2Config.ResetMonsterSlice2Defaults();
        DropLimitGlobals.ResetForTests();
        M2Config.sItemDropLimit = Path.Combine(_dir, "ItemDropLimit") + "\\";
        M2Config.sItemDropLogDir = Path.Combine(_dir, "ItemDropLimit", "DropLog") + "\\";
        M2Config.sSmartMonsterDir = Path.Combine(_dir, "SmartMonster") + "\\";
        M2Config.sCustomMonsterClientConfigFileName = "";
        DropLimitGlobals.g_DropLimitMgr.LoadConfig();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string>();
            f.MonsterListHandler = () => new List<string>();
            f.CustomMonsterListHandler = () => new List<string>();
            f.AllItemsHandler = () => new List<string>();
            f.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig>();
            f.InputQueryHandler = (_, _) => false;
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        DropLimitGlobals.ResetForTests();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private TCustomMonsterConfig NewConfig(string name, ushort race = 156, ushort appr = 10)
        => new(name, race, appr);

    [Fact]
    public void Config_ConstructorDefaults()
    {
        var cfg = NewConfig("白野猪");
        Assert.Equal("白野猪", cfg.MonsterName);
        Assert.Equal((ushort)156, cfg.MonsterRace);
        Assert.Equal((ushort)10, cfg.MonsterAppr);
        Assert.False(cfg.IsChanged);

        // TClientBaseConfig 默认（Delphi 223-238）
        Assert.Equal(TCustomDrawMode.mdmBlend, cfg.ClientBaseConfig.DrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, cfg.ClientBaseConfig.DrawMode2);
        Assert.Equal(TMonsterDrawOrder2.mdoSelf_Eff1_Eff2, cfg.ClientBaseConfig.DrawOrder);
        Assert.Equal(0, cfg.ClientBaseConfig.DieNoCalcDir);
        Assert.Equal(-1, cfg.ClientBaseConfig.HPFile);
        Assert.Equal(-1, cfg.ClientBaseConfig.HPStartIndex);
        Assert.Equal("", cfg.ClientBaseConfig.Sounds[0].Value);

        // 12 动作默认（Delphi 240-253）：PlayTime=100、文件 -1、CalcDir=True
        for (int i = 0; i < 12; i++)
        {
            ref readonly var a = ref cfg.ClientActions[i];
            Assert.Equal((TMonsterClientActionType)i, a.ActionType);
            Assert.Equal(-1, a.ActionFile);
            Assert.Equal(-1, a.StartIndex);
            Assert.Equal(0, a.PlayCount);
            Assert.Equal(100, a.PlayTime);
            Assert.Equal(1, a.CalcDir);
        }

        // 6 组客户端攻击配置默认（Delphi 255-333）：KeepTime=30、KeepAttackInterval=5
        for (int i = 0; i < 6; i++)
        {
            ref readonly var c = ref cfg.ClientAttackConfigs[i];
            Assert.Equal(-1, c.Fly_File);
            Assert.Equal(100, c.Fly_PlayTime);
            Assert.Equal(TCustomDrawMode.mdmBlend, c.Fly_DrawMode);
            Assert.Equal(TCustomDirCount.mdcDir8, c.Fly_DirCount);
            Assert.Equal(1, c.Fly_CalcDir);
            Assert.Equal(TCustomDirCalcType.mdctNone, c.Self_DirCalcType);
            Assert.Equal(TCustomDrawOrder.mdoPriorSelf, c.SelfKeep_DrawOrder);
            Assert.Equal(30, c.Explosion_KeepTime);
            Assert.Equal(5, c.Explosion_KeepAttackInterval);
            Assert.Equal(30, c.Target_KeepTime);
            Assert.Equal(5, c.Target_KeepAttackInterval);
        }

        // ServerBaseConfig 默认（Delphi 335-341）
        Assert.Equal(8, cfg.ServerBaseConfig.ViewRange);
        Assert.Equal(TMonsterType.mtNormal, cfg.ServerBaseConfig.MonsterType);
        Assert.Equal(TMoveOption.moMoveNormal, cfg.ServerBaseConfig.MoveOption);
        Assert.Equal(1, cfg.ServerBaseConfig.MinAttackNearRange);

        // 6 组服务端攻击配置默认（Delphi 343-444）
        for (int i = 0; i < 6; i++)
        {
            var sc = cfg.MonsterServerConfigs[i];
            Assert.False(sc.AttackEnabled);
            Assert.Equal(TCustomOperateMode.momAttack, sc.OperateMode);
            Assert.Equal(400, sc.AttackDelayTime);
            Assert.Equal(101, sc.AttackHPPercent);
            Assert.Equal(100, sc.AttackRate);
            Assert.Equal(TCustomAttackTarget.matSingle, sc.AttackTarget);
            Assert.Equal(TCustomAttackPowerCalc.mapcDC, sc.AttackPowerCalc);
            Assert.Equal(5, sc.AttackTeleportTargetDistance);
            Assert.Equal(4, sc.AttackTeleportDistance);
            Assert.Equal(2, sc.AttackGroupRange);
            Assert.All(sc.Additionals, a => { Assert.False(a.Checked); Assert.Equal(3, a.Rate); Assert.Equal(3, a.Time); });
            Assert.Equal(3, sc.AdditionalHP0);
            Assert.Equal(3, sc.AdditionalImprisonRange);
            Assert.Equal(50, sc.ProtectAddHPPercent);
            Assert.Equal(10, sc.ProtectAddDCPercent);
            Assert.Equal(10, sc.ProtectAddDCTime);
            Assert.Equal(3, sc.ProtectTargetRange);
            Assert.Equal(50, sc.ProtectSelfRate);
        }
    }

    [Fact]
    public void Config_SaveLoadIni_RoundTrip()
    {
        var cfg = NewConfig("沃玛战士", 156, 20);
        cfg.ClientBaseConfig.HPBgOffsetX = 3;
        cfg.ClientBaseConfig.HPFile = 7;
        cfg.ClientBaseConfig.Sounds[2].Value = "mon-2";
        cfg.ClientActions[2].ActionFile = 5;
        cfg.ClientActions[2].StartIndex = 10;
        cfg.ClientActions[2].CalcDir = 0;
        cfg.ClientAttackConfigs[1].Fly_File = 3;
        cfg.ClientAttackConfigs[1].Self_DirCalcType = TCustomDirCalcType.mdctCenter;
        cfg.ServerBaseConfig.ViewRange = 9;
        cfg.ServerBaseConfig.MonsterType = TMonsterType.mtStoneMode;
        cfg.ServerBaseConfig.MoveOption = TMoveOption.moProtect;
        cfg.ServerBaseConfig.ProtectRange = 4;
        cfg.ServerBaseConfig.NoAttack = true;
        cfg.MonsterServerConfigs[2].AttackEnabled = true;
        cfg.MonsterServerConfigs[2].AttackMode = TCustomAttackMode.mamFar;
        cfg.MonsterServerConfigs[2].Additionals[4].Checked = true;
        cfg.MonsterServerConfigs[2].Additionals[4].Rate = 8;
        cfg.MonsterServerConfigs[2].CallMonsters[0] = "黑野猪";
        cfg.MonsterServerConfigs[2].CallMonsterNums[0] = 2;
        cfg.MonsterServerConfigs[2].ProtectAddHPPercent = 30;
        cfg.SetChanged(true);

        cfg.SaveToIniFile();
        Assert.False(cfg.IsChanged); // 保存复位

        // 逐键形态断言（IniFileEx 等效）
        string iniPath = M2Config.sSmartMonsterDir + "沃玛战士.ini";
        Assert.True(File.Exists(iniPath));
        using (var ini = new TFastIniFile(iniPath))
        {
            Assert.Equal("3", ini.ReadString("ClientConfig", "HPBgOffsetX", ""));
            Assert.Equal("7", ini.ReadString("ClientConfig", "HPFile", ""));
            Assert.Equal("mon-2", ini.ReadString("ClientSounds", "Attack", ""));
            Assert.Equal("5", ini.ReadString("ActDefAttack", "ActionFile", ""));
            Assert.Equal("0", ini.ReadString("ActDefAttack", "CalcDir", ""));
            Assert.Equal("3", ini.ReadString("ClientAttack1", "Fly_File", ""));
            Assert.Equal("1", ini.ReadString("ServerConfig", "MonsterType", ""));
            Assert.Equal("1", ini.ReadString("ServerConfig", "NoAttack", ""));
            Assert.Equal("1", ini.ReadString("ServerAttack2", "AttackEnabled", ""));
            Assert.Equal("1", ini.ReadString("Additionals2", "Checked4", ""));
            Assert.Equal("8", ini.ReadString("Additionals2", "Rate4", ""));
            Assert.Equal("黑野猪", ini.ReadString("CallMonster2", "MonsterName0", ""));
            Assert.Equal("30", ini.ReadString("Protect2", "ProtectAddHPPercent", ""));
        }

        // 重建 → LoadFromIniFile 回读一致
        var cfg2 = NewConfig("沃玛战士", 156, 20);
        Assert.False(cfg2.IsChanged);
        Assert.Equal(3, cfg2.ClientBaseConfig.HPBgOffsetX);
        Assert.Equal(7, cfg2.ClientBaseConfig.HPFile);
        Assert.Equal("mon-2", cfg2.ClientBaseConfig.Sounds[2].Value);
        Assert.Equal(5, cfg2.ClientActions[2].ActionFile);
        Assert.Equal(0, cfg2.ClientActions[2].CalcDir);
        Assert.Equal(3, cfg2.ClientAttackConfigs[1].Fly_File);
        Assert.Equal(TCustomDirCalcType.mdctCenter, cfg2.ClientAttackConfigs[1].Self_DirCalcType);
        Assert.Equal(9, cfg2.ServerBaseConfig.ViewRange);
        Assert.Equal(TMonsterType.mtStoneMode, cfg2.ServerBaseConfig.MonsterType);
        Assert.Equal(TMoveOption.moProtect, cfg2.ServerBaseConfig.MoveOption);
        Assert.Equal(4, cfg2.ServerBaseConfig.ProtectRange);
        Assert.True(cfg2.ServerBaseConfig.NoAttack);
        Assert.True(cfg2.MonsterServerConfigs[2].AttackEnabled);
        Assert.Equal(TCustomAttackMode.mamFar, cfg2.MonsterServerConfigs[2].AttackMode);
        Assert.True(cfg2.MonsterServerConfigs[2].Additionals[4].Checked);
        Assert.Equal(8, cfg2.MonsterServerConfigs[2].Additionals[4].Rate);
        Assert.Equal("黑野猪", cfg2.MonsterServerConfigs[2].CallMonsters[0]);
        Assert.Equal(2, cfg2.MonsterServerConfigs[2].CallMonsterNums[0]);
        Assert.Equal(30, cfg2.MonsterServerConfigs[2].ProtectAddHPPercent);
    }

    [Fact]
    public void Config_PackedLayoutSizes()
    {
        // Delphi packed 定长布局：BaseConfig=377（4+32+11×31）、Action=20、AttackConfig=93、客户端块=2+377+240+558=1177
        Assert.Equal(377, System.Runtime.InteropServices.Marshal.SizeOf<TClientBaseConfig>());
        Assert.Equal(20, System.Runtime.InteropServices.Marshal.SizeOf<TMonsterClientAction>());
        Assert.Equal(93, System.Runtime.InteropServices.Marshal.SizeOf<TClientAttackConfig>());
        Assert.Equal(1177, System.Runtime.InteropServices.Marshal.SizeOf<TClientCustomMonsterConfig>());
    }

    [Fact]
    public void SaveClientConfigs_Dat_Layout_And_Crc()
    {
        var cfg1 = NewConfig("怪A", 156, 30);
        var cfg2 = NewConfig("怪B", 157, 40);
        string path = Path.Combine(_dir, "MonsterConfig.dat");
        CustomMonsterClientWriter.Save(new List<TCustomMonsterConfig> { cfg1, cfg2 }, path);

        byte[] bytes = File.ReadAllBytes(path);
        // 头：GUID 标志（16）+ 数量（4）+ CRC（4）
        var flag = new Guid("196BE159-E170-41DB-B2F0-35A1D2EBFA8B").ToByteArray();
        Assert.Equal(flag, bytes.AsSpan(0, 16).ToArray());
        Assert.Equal(2, BitConverter.ToInt32(bytes, 16));
        // 逐怪块 = TClientCustomMonsterConfig 定长 1177 字节
        Assert.Equal(24 + 2 * 1177, bytes.Length);
        // 怪A Appr=30 在块首（Word LE）
        Assert.Equal(30, BitConverter.ToUInt16(bytes, 24));
        Assert.Equal(40, BitConverter.ToUInt16(bytes, 24 + 1177));
        // CRC 校验：偏移 20 处值 = BufferCRC(载荷 24..end)
        uint stored = BitConverter.ToUInt32(bytes, 20);
        var payload = new byte[bytes.Length - 24];
        Array.Copy(bytes, 24, payload, 0, payload.Length);
        Assert.Equal(GXX.Core.Crypto.CheckCrc.BufferCRC(payload, payload.Length), stored);
    }

    [Fact]
    public void Form_Open_BuildsTree_And_NodeClick_Fills()
    {
        var cfg = NewConfig("白野猪", 156, 10);
        cfg.ClientBaseConfig.Sounds[0].Value = "mon-normal";
        cfg.ClientBaseConfig.HPBgOffsetX = 5;
        cfg.ClientActions[0].CalcDir = 0; // 站：不算方向
        cfg.ServerBaseConfig.ViewRange = 9;
        cfg.ServerBaseConfig.MonsterType = TMonsterType.mtDigUP;
        cfg.ServerBaseConfig.MoveOption = TMoveOption.moProtect;
        cfg.MonsterServerConfigs[3].AttackEnabled = true;
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg };

        _form.Open(showModal: false);
        Assert.False(_form.PgcMainEnabled); // FormCreate 尾部禁用
        Assert.Equal(1, _form.VstCustomMonster.Items.Count);
        Assert.Equal("白野猪", _form.VstCustomMonster.Items[0].Text);
        Assert.Null(_form.FCurrentMonsterCustomConfig);
        Assert.Equal(0, _form.VstAction.Items.Count);

        // 点击节点 → 全链回填
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.True(_form.PgcMainEnabled);
        Assert.Same(cfg, _form.FCurrentMonsterCustomConfig);
        Assert.Equal(12, _form.VstAction.Items.Count);
        Assert.Equal("站", _form.VstAction.Items[0].Text);
        Assert.Equal("攻击6", _form.VstAction.Items[11].Text);
        Assert.False(_form.VstAction.Items[0].Checked); // CalcDir=0
        Assert.True(_form.VstAction.Items[1].Checked);  // 默认 CalcDir=True
        // 客户端基础回填
        Assert.Equal("mon-normal", _form.Edits["edtSoundNormal"].Text);
        Assert.Equal(5, _form.Spins["seHPBgOffsetX"].Value);
        Assert.Equal(0, _form.Cbbs["cbbClientDrawMode"].SelectedIndex); // mdmBlend
        // 服务端基础回填
        Assert.Equal(9, _form.Spins["seViewRange"].Value);
        Assert.Equal(2, _form.Cbbs["cbbMonsterType"].SelectedIndex); // mtDigUP
        Assert.Equal(2, _form.Cbbs["cbbMoveOption"].SelectedIndex); // moProtect
        Assert.True(_form.LblProtectVisible); // 守护区可见
        // 攻击组回填（ClientIndex/ServerIndex=0 + 组名）
        Assert.Equal(0, _form.ClientConfigIndex);
        Assert.Equal(0, _form.ServerConfigIndex);
        Assert.Equal("攻击1的攻击效果配置", _form.GrpClientAttackConfigsCaption);
        Assert.Equal("攻击1的攻击效果配置", _form.GrpServerAttackConfigsCaption);
        Assert.False(_form.Chks["chkAttackEnabled"].Checked); // 回填组 0（默认未启用）；组 3 的启用不在此组
        // Race 描述与页签
        Assert.Equal("  Race = 156; 普通怪物; 主动攻击目标", _form.PnlMonDescCaption);
        Assert.True(_form.TsAttackVisible);
        Assert.True(_form.TsServerAttackVisible);
        // 回填不改状态：IsChanged 保持 False、保存钮关
        Assert.False(cfg.IsChanged);
        Assert.False(_form.FIsMonsterChanged);
        Assert.False(_form.btnSave.Enabled);
    }

    [Fact]
    public void Form_NodeClick_Race157_HidesAttackTabs()
    {
        // Delphi：仅 Race ∈ [154,159] 隐藏攻击页签；157（不主动攻击）仍显示
        var cfg = NewConfig("鹿", 157, 5);
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg };
        _form.Open(showModal: false);
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.Equal("  Race = 157; 普通怪物; 不会主动攻击目标，如鸡羊鹿", _form.PnlMonDescCaption);
        Assert.True(_form.TsAttackVisible);
        Assert.True(_form.TsServerAttackVisible);

        // Race 154（魔王岭）/159（采集类）→ 页签隐藏
        var cfg154 = NewConfig("魔王岭怪", 154, 6);
        var cfg159 = NewConfig("采集怪", 159, 7);
        _form.VstCustomMonster.Items.Add(new System.Windows.Forms.ListViewItem(cfg154.MonsterName) { Tag = cfg154 });
        _form.VstCustomMonster.Items.Add(new System.Windows.Forms.ListViewItem(cfg159.MonsterName) { Tag = cfg159 });
        _form.VstCustomMonster.Items[0].Selected = false; // 无句柄环境需手工取消原选中（真机点击自动取消）
        _form.VstCustomMonster.Items[1].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.False(_form.TsAttackVisible);
        Assert.False(_form.TsServerAttackVisible);
        Assert.Equal("  Race = 154; 魔王岭怪物; 不攻击目标", _form.PnlMonDescCaption);
        _form.VstCustomMonster.Items[1].Selected = false;
        _form.VstCustomMonster.Items[2].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.False(_form.TsAttackVisible);
        Assert.False(_form.TsServerAttackVisible);
        Assert.Equal("  Race = 159; 采集类怪物；不攻击目标", _form.PnlMonDescCaption);
    }

    [Fact]
    public void Form_VstActionChecked_WritesCalcDir()
    {
        var cfg = NewConfig("钉耙猫", 156, 8);
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg };
        _form.Open(showModal: false);
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.True(_form.VstAction.Items[0].Checked);

        _form.VstActionChecked(0, false);
        Assert.Equal(0, cfg.ClientActions[0].CalcDir);
        Assert.True(cfg.IsChanged);
        Assert.True(_form.FIsMonsterChanged);
        Assert.True(_form.btnSave.Enabled);

        _form.VstActionChecked(0, true);
        Assert.Equal(1, cfg.ClientActions[0].CalcDir);
    }

    [Fact]
    public void Form_BtnSave_PersistsChangedConfigs()
    {
        var changed = NewConfig("已改怪", 156, 1);
        changed.ClientBaseConfig.HPOffsetX = 7;
        var unchanged = NewConfig("未改怪", 156, 2);
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { changed, unchanged };
        _form.Open(showModal: false);
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        _form.VstActionChecked(0, false); // 触发改标志

        Assert.True(_form.btnSave.Enabled);
        _form.BtnSaveClick();
        Assert.False(_form.FIsMonsterChanged);
        Assert.False(_form.btnSave.Enabled);
        Assert.True(File.Exists(M2Config.sSmartMonsterDir + "已改怪.ini"));
        Assert.False(File.Exists(M2Config.sSmartMonsterDir + "未改怪.ini")); // 未改不落盘
        Assert.False(changed.IsChanged);
    }

    [Fact]
    public void Form_CtrlF_FindsAndSelects()
    {
        var cfg1 = NewConfig("黑野猪");
        var cfg2 = NewConfig("红野猪");
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg1, cfg2 };
        _form.Open(showModal: false);

        bool asked = false;
        _form.InputQueryHandler = (caption, prompt) =>
        {
            asked = caption == "关键字查找" && prompt == "输入关键字:";
            _form.LastInputQueryText = "红野猪";
            return true;
        };
        _form.VstCustomMonsterCtrlF();
        Assert.True(asked);
        Assert.Same(cfg2, _form.FCurrentMonsterCustomConfig);
        Assert.Equal(12, _form.VstAction.Items.Count);

        // 未命中保持
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.Same(cfg1, _form.FCurrentMonsterCustomConfig);
        _form.InputQueryHandler = (_, _) => true;
        _form.LastInputQueryText = "不存在的怪";
        _form.VstCustomMonsterCtrlF();
        Assert.Same(cfg1, _form.FCurrentMonsterCustomConfig);
        // 取消对话框 → 不动
        _form.InputQueryHandler = (_, _) => false;
        _form.VstCustomMonsterCtrlF();
        Assert.Same(cfg1, _form.FCurrentMonsterCustomConfig);
    }

    [Fact]
    public void Form_Btn1_GeneratesClientDat()
    {
        var cfg = NewConfig("白野猪", 156, 30);
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg };
        _form.Open(showModal: false);

        string target = Path.Combine(_dir, "登录器配置");
        _form.SaveMonsterDialogHandler = _ => target;
        _form.Btn1Click();
        string datPath = Path.ChangeExtension(target, ".dat");
        Assert.True(File.Exists(datPath));
        Assert.Equal(datPath, M2Config.sCustomMonsterClientConfigFileName);
        Assert.Equal(datPath, M2ShareState.ConfigIni.ReadString("Setup", "CustomMonsterClientConfigFileName", ""));
        Assert.Equal("已经生成自定义怪物登录器配置文件", _form.LastShowMessage);
        byte[] bytes = File.ReadAllBytes(datPath);
        Assert.Equal(24 + 1177, bytes.Length);

        // 取消 → 不生成不提示
        _form.SaveMonsterDialogHandler = _ => null;
        _form.Btn1Click();
        Assert.Null(_form.LastShowMessage);
    }

    [Fact]
    public void Form_ComboItems_AreDelphiNameTables()
    {
        Assert.Equal(new List<string> { "攻击1", "攻击2", "攻击3", "攻击4", "攻击5", "攻击6" }, _form.Cbbs["cbbClientAttackConfig"].Items.Cast<object>().Select(o => o.ToString()!).ToList());
        Assert.Equal(new List<string> { "普通怪物", "石化怪物", "苏醒怪物" }, _form.Cbbs["cbbMonsterType"].Items.Cast<object>().Select(o => o.ToString()!).ToList());
        Assert.Equal(new List<string> { "自由移动", "不可移动", "守护区域" }, _form.Cbbs["cbbMoveOption"].Items.Cast<object>().Select(o => o.ToString()!).ToList());
        Assert.Equal(new List<string> { "攻击模式", "增益模式" }, _form.Cbbs["cbbOperateMode"].Items.Cast<object>().Select(o => o.ToString()!).ToList());
        Assert.Equal(new List<string> { "近攻", "远攻" }, _form.Cbbs["cbbAttackMode"].Items.Cast<object>().Select(o => o.ToString()!).ToList());
        Assert.Equal("根据Appr计算", _form.Cbbs["cbbClientFlyFile"].Items[0].ToString());
        // 文件下拉首项占用：Fly_File=-1 → ItemIndex 0
        var cfg = NewConfig("白野猪");
        _form.CustomMonsterConfigsHandler = () => new List<TCustomMonsterConfig> { cfg };
        _form.Open(showModal: false);
        _form.VstCustomMonster.Items[0].Selected = true;
        _form.VstCustomMonsterNodeClick();
        Assert.Equal(0, _form.Cbbs["cbbClientFlyFile"].SelectedIndex);
        Assert.Equal(-1, cfg.ClientAttackConfigs[0].Fly_File);
    }
}
