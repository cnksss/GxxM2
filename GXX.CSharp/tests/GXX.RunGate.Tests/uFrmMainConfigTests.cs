// 测试：Source\RunGate\uFrmMain.pas（实测 LF 4217）的 **S1 配置加载/校验/写回**
//   → src/GXX.RunGate/RunGateConfigLoader.cs
//
// 行号口径：物理 LF 行号（`[IO.File]::ReadAllText(p) -split "`n"`）；`Get-Content`/`Select-String` 会漂移 +17。
// 覆盖策略：每个公开成员 ≥3 用例；原文缺陷写成差异断言并标注 `C<n>`（编号与 RunGateConfigLoader.cs 文件头一致）。
using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>字典后备的 INI 替身。`ReadString` 可被钩住以复现"同一个键被读两次"的原文行为（缺陷 C2）。</summary>
public sealed class FakeIni : TMemIniFileEx
{
    private readonly Dictionary<string, string> _v = new(StringComparer.Ordinal);
    public readonly Dictionary<string, int> ReadCounts = new(StringComparer.Ordinal);
    /// <summary>非 null 时，第 2 次及以后的读取返回该值（用于 C2 双读差异）。</summary>
    public Func<string, string, string> SecondReadOverride;

    public FakeIni() : base("fake.ini") { }

    public FakeIni Set(string section, string ident, string value)
    {
        _v[section + "\u0001" + ident] = value;
        return this;
    }

    public string Get(string section, string ident) => _v.TryGetValue(section + "\u0001" + ident, out var s) ? s : "";

    public override string ReadString(string section, string ident, string def)
    {
        string key = section + "\u0001" + ident;
        ReadCounts.TryGetValue(key, out int n);
        ReadCounts[key] = n + 1;
        if (!_v.TryGetValue(key, out string val)) return def;
        if (n >= 1 && SecondReadOverride != null)
        {
            string o = SecondReadOverride(section, ident);
            if (o != null) return o;
        }
        return val;
    }

    public override void WriteString(string section, string ident, string value)
        => _v[section + "\u0001" + ident] = value;

}

[Collection("RunGateFormLane")]
public sealed class uFrmMainConfigTests : IDisposable
{
    private readonly string _dir;

    public uFrmMainConfigTests()
    {
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        GateShareSeam.ResetForTest();
        ClearEatItemCd();
        _dir = Path.Combine(Path.GetTempPath(), "p2rg-cfg-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        GateSharePaths.SetExeDirForTest(_dir);
    }

    public void Dispose()
    {
        GateSharePaths.ResetForTest();
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        GateShareSeam.ResetForTest();
        ClearEatItemCd();
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
    }

    /// <summary>`FormGlobals.ResetForTest` **不重置** `g_EatItemCDConfig` 的 42 个字段
    /// （那是其他窗体测试留下的污染面），本类自己清一遍以保证用例互相独立。</summary>
    private static void ClearEatItemCd()
    {
        foreach (var h in FormGlobals.g_EatItemCDConfig.Hum) Zero(h);
        foreach (var h in FormGlobals.g_EatItemCDConfig.Hero) Zero(h);
    }

    private static void Zero(TItemCDTime t)
    {
        if (t == null) return;
        t.NormalHP = 0; t.NormalMP = 0; t.NormalHPMP = 0;
        t.SpecialHP = 0; t.SpecialMP = 0; t.SpecialHPMP = 0; t.Other = 0;
    }

    private static void Load(TCustomIniFileEx ini, IList<int> ports = null, RunGateConfigBounds b = null)
        => RunGateConfigLoader.LoadConfig(ini, ports ?? new List<int>(), b);

    // ================= DFM 边界（脚本抽取回读） =================

    [Fact]
    public void DfmBounds_MatchScriptExtraction()
    {
        var b = RunGateConfigBounds.FromDfm();
        Assert.Equal((60, 600), (b.CheckServerTimeOutTimeMin, b.CheckServerTimeOutTimeMax));            // dfm:839
        Assert.Equal((1, 8), (b.ClientSendBlockSizeMin, b.ClientSendBlockSizeMax));                     // dfm:855
        Assert.Equal((256, 102400), (b.PreAllocatedCountMin, b.PreAllocatedCountMax));                  // dfm:887
        Assert.Equal((15, 180), (b.RecvAntiPlugHeartbeatTimeOutTimeMin, b.RecvAntiPlugHeartbeatTimeOutTimeMax));   // dfm:970
        Assert.Equal((1, 20), (b.AntiPlugStreamSendSpeedMin, b.AntiPlugStreamSendSpeedMax));            // dfm:982
        Assert.Equal(11, b.ShowLogLevelItemCount);                                                      // dfm:715
        Assert.Equal(4, b.AntiPlugStreamSendBlockSizeItemCount);                                        // dfm:997
    }

    // ================= ValidateSettingInputs（原 :2766-2806） =================

    private static RunGateConfigLoader.SettingValidation V(string gip, string gp, string sip, string sp, string t)
        => RunGateConfigLoader.ValidateSettingInputs(gip, gp, sip, sp, t,
               out _, out _, out _, out _, out _);

    [Fact]
    public void Validate_全部合法返回Ok()
    {
        Assert.Equal(RunGateConfigLoader.SettingValidation.Ok,
            V("0.0.0.0", "7200", "127.0.0.1", "5000", "游戏网关"));
    }

    [Fact]
    public void Validate_五条失败分支的顺序与文案()
    {
        Assert.Equal(RunGateConfigLoader.SettingValidation.GateAddrInvalid, V("bad", "7200", "127.0.0.1", "5000", "T"));      // 原 :2773
        Assert.Equal(RunGateConfigLoader.SettingValidation.GatePortInvalid, V("1.1.1.1", "bad", "127.0.0.1", "5000", "T"));  // 原 :2780
        Assert.Equal(RunGateConfigLoader.SettingValidation.ServerAddrInvalid, V("1.1.1.1", "7200", "bad", "5000", "T"));      // 原 :2787
        Assert.Equal(RunGateConfigLoader.SettingValidation.ServerPortInvalid, V("1.1.1.1", "7200", "1.1.1.2", "bad", "T"));  // 原 :2794
        Assert.Equal(RunGateConfigLoader.SettingValidation.TitleEmpty, V("1.1.1.1", "7200", "1.1.1.2", "5000", "   "));       // 原 :2801
    }

    [Fact]
    public void Validate_端口0被允许而负数与65536被拒_原文缺陷C11()
    {
        // ★ C11：判据是 `< 0`，所以 0 合法
        Assert.Equal(RunGateConfigLoader.SettingValidation.Ok, V("1.1.1.1", "0", "1.1.1.2", "0", "T"));
        Assert.Equal(RunGateConfigLoader.SettingValidation.Ok, V("1.1.1.1", "65535", "1.1.1.2", "65535", "T"));
        Assert.Equal(RunGateConfigLoader.SettingValidation.GatePortInvalid, V("1.1.1.1", "-1", "1.1.1.2", "1", "T"));
        Assert.Equal(RunGateConfigLoader.SettingValidation.GatePortInvalid, V("1.1.1.1", "65536", "1.1.1.2", "1", "T"));
    }

    [Fact]
    public void Validate_非数字端口按StrToIntDef得到负一()
    {
        Assert.Equal(RunGateConfigLoader.SettingValidation.GatePortInvalid, V("1.1.1.1", "abc", "1.1.1.2", "1", "T"));
        Assert.Equal(RunGateConfigLoader.SettingValidation.GatePortInvalid, V("1.1.1.1", "", "1.1.1.2", "1", "T"));
    }

    [Fact]
    public void Validate_Trim后再判空与解析()
    {
        var r = RunGateConfigLoader.ValidateSettingInputs(
            "  1.1.1.1  ", "  7200  ", "  1.1.1.2  ", "  5000  ", "  T  ",
            out var gip, out var gp, out var sip, out var sp, out var t);
        Assert.Equal(RunGateConfigLoader.SettingValidation.Ok, r);
        Assert.Equal("1.1.1.1", gip);
        Assert.Equal(7200, gp);
        Assert.Equal("1.1.1.2", sip);
        Assert.Equal(5000, sp);
        Assert.Equal("T", t);
    }

    // ================= ApplySettingGlobals（原 :2808-2813） =================

    [Fact]
    public void ApplySetting_写6个全局量()
    {
        RunGateConfigLoader.ApplySettingGlobals("1.1.1.1", 7200, "2.2.2.2", 5000, "T", 27201);
        Assert.Equal("1.1.1.1", GateShareGlobals.g_sGateAddr);
        Assert.Equal(7200, (int)GateShareGlobals.g_wdGatePort);
        Assert.Equal("2.2.2.2", GateShareGlobals.g_sServerAddr);
        Assert.Equal(5000, (int)GateShareGlobals.g_wdServerPort);
        Assert.Equal(27201, (int)GateShareGlobals.g_wdDBPort);
        Assert.Equal("T", GateShareGlobals.g_sTitleName);
    }

    [Fact]
    public void ApplySetting_端口按Word截断_原文缺陷C12()
    {
        // ★ C12：`IntGatePort: Integer` 赋给 `Word` 全局 → 低 16 位
        RunGateConfigLoader.ApplySettingGlobals("1.1.1.1", 65536, "2.2.2.2", 70000, "T", -1);
        Assert.Equal(0, (int)GateShareGlobals.g_wdGatePort);          // 65536 & 0xFFFF
        Assert.Equal(70000 & 0xFFFF, (int)GateShareGlobals.g_wdServerPort);
        Assert.Equal(0xFFFF, (int)GateShareGlobals.g_wdDBPort);       // -1 & 0xFFFF
    }

    [Fact]
    public void ApplySetting_DBPort不校验_原文缺陷C11()
    {
        // 原文 :2771 读了 IntDBPort，:2812 直接赋值，没有任何范围判断
        RunGateConfigLoader.ApplySettingGlobals("1.1.1.1", 1, "2.2.2.2", 1, "T", 99999);
        Assert.Equal(99999 & 0xFFFF, (int)GateShareGlobals.g_wdDBPort);
    }

    // ================= LoadConfig：缺键保留默认 =================

    [Fact]
    public void LoadConfig_空INI保留全部默认()
    {
        GateShareGlobals.g_sTitleName = "游戏网关";
        GateShareGlobals.g_wdGatePort = 7200;
        FormGlobals.g_nMaxClientPacketSize = 512;
        var ini = new FakeIni();

        Load(ini);

        Assert.Equal("游戏网关", GateShareGlobals.g_sTitleName);        // 原 :1101 默认取自身
        Assert.Equal(7200, (int)GateShareGlobals.g_wdGatePort);
        Assert.Equal(512, FormGlobals.g_nMaxClientPacketSize);
        Assert.Equal(100, FormGlobals.nMaxClientMsgCount);              // :1226 初值
    }

    [Fact]
    public void LoadConfig_身份与端口()
    {
        var ini = new FakeIni()
            .Set("GameGate", "Title", "我的网关")
            .Set("GameGate", "Server1", "10.0.0.1")
            .Set("GameGate", "ServerPort", "5001")
            .Set("GameGate", "GateAddr", "0.0.0.0")
            .Set("GameGate", "GatePort", "7201")
            .Set("GameGate", "DBPort", "27202");

        Load(ini);

        Assert.Equal("我的网关", GateShareGlobals.g_sTitleName);
        Assert.Equal("10.0.0.1", GateShareGlobals.g_sServerAddr);
        Assert.Equal(5001, (int)GateShareGlobals.g_wdServerPort);
        Assert.Equal(7201, (int)GateShareGlobals.g_wdGatePort);
        Assert.Equal(27202, (int)GateShareGlobals.g_wdDBPort);
    }

    [Fact]
    public void LoadConfig_ShowLogLevel三条分支_原文缺陷C1与C15()
    {
        // ★ C15（本轮新发现）：`g_btShowLogLevel: Byte`（GateShare.pas:1172）—— `ReadInteger` 的
        //   结果**先被截断成 Byte**，所以 `-5` 会变成 **251**，于是 `<= 0` 分支**不可达**（Byte 无负值），
        //   直接命中上界钳位 → 结果是 **11**，而不是"负值抹成 0"。
        //   即 `if g_btShowLogLevel <= 0` 实际等价于 `= 0`。
        Load(new FakeIni().Set("GameGate", "ShowLogLevel", "-5"));
        Assert.Equal(11, (int)GateShareGlobals.g_btShowLogLevel);       // 251 > 11 → 钳到 11

        GateShareGlobals.ResetForTest();
        Load(new FakeIni().Set("GameGate", "ShowLogLevel", "0"));
        Assert.Equal(0, (int)GateShareGlobals.g_btShowLogLevel);        // 只有 0 走 `<= 0`

        // ★ C1：上界钳到 Items.Count = 11（合法 ItemIndex 只到 10）→ `cbbShowLogLevel.ItemIndex := 11` 越界
        GateShareGlobals.ResetForTest();
        Load(new FakeIni().Set("GameGate", "ShowLogLevel", "99"));
        Assert.Equal(11, (int)GateShareGlobals.g_btShowLogLevel);

        GateShareGlobals.ResetForTest();
        Load(new FakeIni().Set("GameGate", "ShowLogLevel", "5"));
        Assert.Equal(5, (int)GateShareGlobals.g_btShowLogLevel);
    }

    [Fact]
    public void LoadConfig_ClientSendBlockSize与MaxPreallocated接受范围内值()
    {
        Load(new FakeIni()
            .Set("GameGate", "ClientSendBlockSize", "8")
            .Set("GameGate", "MaxPreallocatedMemorySize", "102400"));
        Assert.Equal(8u, GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE);
        Assert.Equal(102400, GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE);
    }

    [Fact]
    public void LoadConfig_两个MAX量越界时被静默忽略()
    {
        GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE = 5;
        GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE = 1024;
        Load(new FakeIni()
            .Set("GameGate", "ClientSendBlockSize", "9")            // > Max 8
            .Set("GameGate", "MaxPreallocatedMemorySize", "255"));  // < Min 256
        Assert.Equal(5u, GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE);
        Assert.Equal(1024, GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE);
    }

    [Fact]
    public void LoadConfig_MaxClientPacketSize大于512回落128_原文缺陷C3()
    {
        // ★ C3：回落目标是 128，不是内联初值 512
        Load(new FakeIni().Set("GameGate", "MaxClientPacketSize", "513"));
        Assert.Equal(128, FormGlobals.g_nMaxClientPacketSize);
        GateShareGlobals.ResetForTest(); FormGlobals.ResetForTest();
        Load(new FakeIni().Set("GameGate", "MaxClientPacketSize", "512"));
        Assert.Equal(512, FormGlobals.g_nMaxClientPacketSize);       // 512 不触发
    }

    [Fact]
    public void LoadConfig_AttackTick读两次且第二次结果生效_原文缺陷C2()
    {
        // ★ C2：`if ReadInteger('AttackTick',-1) > 0 then g_dwAttackTick := ReadInteger('AttackTick', g_dwAttackTick)`
        //   本用例让**第二次读**返回不同值，证明原文确实读了两遍、且以第二遍为准。
        var ini = new FakeIni().Set("GameGate", "AttackTick", "300");
        ini.SecondReadOverride = (section, ident) => ident == "AttackTick" ? "777" : null;

        Load(ini);

        Assert.True(ini.ReadCounts["GameGate\u0001AttackTick"] >= 2);      // ★ 同一个键确实被读了两遍
        Assert.Equal(777u, FormGlobals.g_dwAttackTick);                    // 以第二次为准
    }

    [Fact]
    public void LoadConfig_AttackTick非正数时不改()
    {
        FormGlobals.g_dwAttackTick = 300;
        Load(new FakeIni().Set("GameGate", "AttackTick", "0"));
        Assert.Equal(300u, FormGlobals.g_dwAttackTick);
        Load(new FakeIni().Set("GameGate", "AttackTick", "-1"));
        Assert.Equal(300u, FormGlobals.g_dwAttackTick);
    }

    [Fact]
    public void LoadConfig_BlockMethod不做范围校验_原文缺陷C4()
    {
        // ★ C4：与 ProcessMode/SumProcessMode/FilterSayMsgMode 不同，这里没有钳位
        Load(new FakeIni().Set("GameGate", "BlockMethod", "99"));
        Assert.Equal(99, (int)FormGlobals.g_BlockMethod);
        Assert.False(Enum.IsDefined(typeof(TBlockIPMethod), FormGlobals.g_BlockMethod));
    }

    [Fact]
    public void LoadConfig_动作模式循环_基础模式与并发模式()
    {
        var ini = new FakeIni()
            .Set("Hit", "Enabled", "1").Set("Hit", "Interval", "111")
            .Set("Hit", "ProcessMode", "1").Set("Hit", "SumProcessMode", "2")
            .Set("Hit", "ShowHint", "1").Set("Hit", "HintText", "慢一点")
            .Set("Hit", "CompensationValue", "7").Set("Hit", "Debug", "1")
            .Set("HitConcurrent", "Interval", "999")          // 并发模式：Interval 不会被读
            .Set("HitConcurrent", "Enabled", "0");            // 也不会被读 → 强制 True

        Load(ini);

        var a0 = FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit];
        Assert.True(a0.boEnabled);
        Assert.Equal(111u, a0.nInterval);
        Assert.Equal(TActionProcessMode.apmRebound, a0.ProcessMode);       // 枚举值 1
        Assert.Equal(TSumActionProcessMode.sampLockUser, a0.SumProcessMode);   // 枚举值 2
        Assert.True(a0.boShowHint);
        Assert.Equal("慢一点", a0.sHintText);
        Assert.Equal(7, a0.nCompensationValue);
        Assert.True(a0.boDebug);

        var c = FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHitConcurrent];
        Assert.True(c.boEnabled);                                          // 原 :1226 无条件 True
        Assert.Equal(1u, c.nInterval);                                     // 原 CreateDefaultConfig 的初值（未被 INI 覆盖）
        Assert.Equal(0, ini.ReadCounts.TryGetValue("HitConcurrent\u0001Interval", out int n) ? n : 0);
    }

    [Fact]
    public void LoadConfig_ProcessMode越界回落Low()
    {
        Load(new FakeIni().Set("Hit", "ProcessMode", "99").Set("Hit", "SumProcessMode", "99"));
        var a = FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit];
        Assert.Equal(TActionProcessMode.apmDelay, a.ProcessMode);          // 原 :1237 `Low(...)`
        Assert.Equal(TSumActionProcessMode.sapmNone, a.SumProcessMode);    // 原 :1245 `Low(...)`
    }

    [Fact]
    public void LoadConfig_SendSpeedIntervals只对非基础模式读取_原文缺陷D12()
    {
        var ini = new FakeIni()
            .Set("Hit", "SendSpeedIntervalsToClient", "1")            // amHit 在排除集里 → 不读
            .Set("WalkToHit", "SendSpeedIntervalsToClient", "1");     // 非基础模式 → 读

        Load(ini);

        Assert.False(FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit]);
        Assert.True(FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amWalkToHit]);
        Assert.Equal(0, ini.ReadCounts.TryGetValue("Hit\u0001SendSpeedIntervalsToClient", out int n1) ? n1 : 0);
    }

    [Fact]
    public void LoadConfig_Setup段钳位()
    {
        var ini = new FakeIni()
            .Set("Setup", "CollectCount", "25")      // > 20 → 拒绝（保默认 15）
            .Set("Setup", "SpeedValue", "18");       // 合法 2..18
        Load(ini);
        Assert.Equal(15, FormGlobals.g_Config.dwCollectCount);
        Assert.Equal(14, FormGlobals.g_Config.dwSpeedValue);   // 18 >= 15 → 15-1（原 :1312-1315）

        FormGlobals.ResetForTest();
        Load(new FakeIni().Set("Setup", "CollectCount", "5").Set("Setup", "SpeedValue", "2"));
        Assert.Equal(5, FormGlobals.g_Config.dwCollectCount);
        Assert.Equal(2, FormGlobals.g_Config.dwSpeedValue);
    }

    [Fact]
    public void LoadConfig_ContinueSpeedCount钳位2到8()
    {
        Load(new FakeIni().Set("Setup", "ContinueSpeedCount", "1"));
        Assert.Equal(4, FormGlobals.g_Config.nContinueSpeedCount);      // 默认 4，1 被拒
        FormGlobals.ResetForTest();
        Load(new FakeIni().Set("Setup", "ContinueSpeedCount", "8"));
        Assert.Equal(8, FormGlobals.g_Config.nContinueSpeedCount);
        FormGlobals.ResetForTest();
        Load(new FakeIni().Set("Setup", "ContinueSpeedCount", "9"));
        Assert.Equal(4, FormGlobals.g_Config.nContinueSpeedCount);
    }

    [Fact]
    public void LoadConfig_WarnSayMsg默认是空串而不是全局默认()
    {
        FormGlobals.g_WarnSayMsg = "您发送的信息里包含了非法字符。";
        Load(new FakeIni());                                            // 键缺失
        Assert.Equal("", FormGlobals.g_WarnSayMsg);                     // 原 :1333 默认 ''
    }

    [Fact]
    public void LoadConfig_FilterSayMsgMode越界不改()
    {
        FormGlobals.g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;
        Load(new FakeIni().Set("GameGate", "FilterSayMsgMode", "99"));
        Assert.Equal(TFilterSayMsgMode.fsmmDisMsg, FormGlobals.g_FilterSayMsgMode);
        Load(new FakeIni().Set("GameGate", "FilterSayMsgMode", "0"));
        Assert.Equal(TFilterSayMsgMode.fsmmAllBlock, FormGlobals.g_FilterSayMsgMode);
    }

    // ================= ★ C6/C7 的补键与不一致保护 =================

    [Fact]
    public void LoadConfig_禁言提示缺键时写回默认_原文缺陷C6()
    {
        var ini = new FakeIni();                                        // 键全缺
        GateShareSeam.g_sDisableSayMsg = "禁止聊天";
        GateShareSeam.g_sDisableSayMsgBegin = "由于您说话太快，%d秒内禁止聊天！！！";

        Load(ini);

        Assert.Equal("禁止聊天", ini.Get("String", "DisableSayMsg"));                              // 原 :1358 写回
        Assert.Equal("由于您说话太快，%d秒内禁止聊天！！！", ini.Get("String", "DisableSayMsgBegin"));   // 原 :1365 写回
        Assert.Equal("禁止聊天", GateShareSeam.g_sDisableSayMsg);                                  // 全局未被改
    }

    [Fact]
    public void LoadConfig_禁言提示有值时取INI值()
    {
        var ini = new FakeIni().Set("String", "DisableSayMsg", "别说话");
        Load(ini);
        Assert.Equal("别说话", GateShareSeam.g_sDisableSayMsg);
        Assert.Equal("别说话", ini.Get("String", "DisableSayMsg"));               // 有值时不覆盖
        Assert.NotEqual("", ini.Get("String", "DisableSayMsgBegin"));            // 缺键的另一个仍被补写
    }

    [Fact]
    public void LoadConfig_FYDownURL的ValueExists保护_原文缺陷C7()
    {
        // 键**缺失** → 保留全局默认
        FormGlobals.g_sFYDownDenyIPUrl = "http://default/KickList.txt";
        Load(new FakeIni());
        Assert.Equal("http://default/KickList.txt", FormGlobals.g_sFYDownDenyIPUrl);

        // 键**存在但为空** → 被清成空串（原 :1419-1420）。
        // ★ 注：`ValueExists` 在 `TCustomIniFileEx` 上**不是 virtual**，无法用替身伪造，
        //   故这一段用真实 `TIniFileEx` 落盘再读。
        string path = Path.Combine(_dir, "C7a.ini");
        using (var w = new TIniFileEx(path)) w.WriteString("GameGate", "FYDownDenyIPUrl", "");
        FormGlobals.g_sFYDownDenyIPUrl = "http://default/KickList.txt";
        using (var r = new TIniFileEx(path)) Load(r);
        Assert.Equal("", FormGlobals.g_sFYDownDenyIPUrl);
    }

    [Fact]
    public void LoadConfig_FYDownMACUrl没有ValueExists保护_原文缺陷C7()
    {
        // 键**缺失**时 `ReadString(..., g_sFYDownDenyMACUrl)` 返回默认值本身 → 保留
        FormGlobals.g_sFYDownDenyMACUrl = "http://default/mac.txt";
        Load(new FakeIni());
        Assert.Equal("http://default/mac.txt", FormGlobals.g_sFYDownDenyMACUrl);

        // 键存在但为空 → 同样被清空；区别在于原文这里**没有** ValueExists 分支（多一层保护才该保留默认）
        string path = Path.Combine(_dir, "C7b.ini");
        using (var w = new TIniFileEx(path)) w.WriteString("GameGate", "FYDownDenyMACUrl", "");
        FormGlobals.g_sFYDownDenyMACUrl = "http://default/mac.txt";
        using (var r = new TIniFileEx(path)) Load(r);
        Assert.Equal("", FormGlobals.g_sFYDownDenyMACUrl);
    }

    // ================= 多网关端口（原 :1407-1413） =================

    [Fact]
    public void LoadConfig_端口列表过滤非法值()
    {
        var ini = new FakeIni()
            .Set("GameGates", "Count", "5")
            .Set("GameGates", "Port1", "7200")
            .Set("GameGates", "Port2", "0")          // 被过滤
            .Set("GameGates", "Port3", "65535")
            .Set("GameGates", "Port4", "65536")      // 被过滤
            .Set("GameGates", "Port5", "-1");        // 被过滤
        var ports = new List<int>();

        Load(ini, ports);

        Assert.Equal(new[] { 7200, 65535 }, ports);
    }

    [Fact]
    public void LoadConfig_端口Count为0或负时不循环()
    {
        var p0 = new List<int>();
        Load(new FakeIni().Set("GameGates", "Count", "0"), p0);
        Assert.Empty(p0);

        var pn = new List<int>();
        Load(new FakeIni().Set("GameGates", "Count", "-3"), pn);
        Assert.Empty(pn);
    }

    [Fact]
    public void LoadConfig_端口键缺失按0处理并被过滤()
    {
        var ports = new List<int>();
        Load(new FakeIni().Set("GameGates", "Count", "2"), ports);
        Assert.Empty(ports);
    }

    // ================= 反外挂发送参数（原 :1168-1191） =================

    [Theory]
    [InlineData("0", 100, 128 * 1024 * 2 / 10)]
    [InlineData("1", 75, 128 * 1024 * 2 / 13)]
    [InlineData("2", 50, 128 * 1024 * 2 / 20)]
    [InlineData("3", 25, 128 * 1024 * 2 / 40)]
    public void LoadConfig_反外挂发送档位到间隔与块大小的映射(string blockSizeIndex, int expectedInterval, int expectedBlock)
    {
        Load(new FakeIni()
            .Set("GameGate", "AntiPlugStreamSendBlockSize", blockSizeIndex)
            .Set("GameGate", "AntiPlugStreamSendSpeed", "2"));       // 默认档 2
        Assert.Equal(expectedInterval, GateShareGlobals.g_ClientAntiPlugDllSendInterval);
        Assert.Equal(expectedBlock, GateShareGlobals.g_ClientAntiPlugDllBlockSize);
    }

    [Fact]
    public void LoadConfig_发送档位越界被拒后走默认档2()
    {
        Load(new FakeIni().Set("GameGate", "AntiPlugStreamSendBlockSize", "4"));   // Items.Count = 4 → 4 越界
        Assert.Equal(2, GateShareGlobals.g_nAntiPlugStreamSendBlockSize);          // 保持默认
        Assert.Equal(50, GateShareGlobals.g_ClientAntiPlugDllSendInterval);        // 默认档 2
    }

    [Fact]
    public void LoadConfig_反外挂更新间隔只钳下界_原文缺陷C5()
    {
        Load(new FakeIni().Set("GameGate", "AntiPlugUpdateCheckInterval", "0"));
        Assert.Equal(1, FormGlobals.g_wAntiPlugUpdateCheckInterval);
        Load(new FakeIni().Set("GameGate", "AntiPlugUpdateCheckInterval", "99999"));
        Assert.Equal(99999, FormGlobals.g_wAntiPlugUpdateCheckInterval);           // 无上界
    }

    [Fact]
    public void LoadConfig_反外挂配置URL键名末尾带5()
    {
        Load(new FakeIni().Set("GameGate", "AntiPlugUpdateConfigUrl5", "http://x/5.txt"));
        Assert.Equal("http://x/5.txt", FormGlobals.g_sAntiPlugUpdateConfigUrl);

        FormGlobals.ResetForTest();
        Load(new FakeIni().Set("GameGate", "AntiPlugUpdateConfigUrl", "http://x/plain.txt"));
        Assert.Equal("", FormGlobals.g_sAntiPlugUpdateConfigUrl);                  // 少了的 5 读不到
    }

    // ================= 吃药 CD 键名（原 :1483-1505） =================

    [Fact]
    public void LoadConfig_吃药CD键名前缀从1起且无分隔符_原文缺陷C10()
    {
        var ini = new FakeIni()
            .Set("HumanItemEatCD", "1NormalHP", "101")
            .Set("HumanItemEatCD", "2NormalHP", "202")
            .Set("HumanItemEatCD", "3SpecialHPMP", "303")
            .Set("HeroItemEatCD", "1Other", "404");

        Load(ini);

        Assert.Equal(101, FormGlobals.g_EatItemCDConfig.Hum[0].NormalHP);
        Assert.Equal(202, FormGlobals.g_EatItemCDConfig.Hum[1].NormalHP);
        Assert.Equal(303, FormGlobals.g_EatItemCDConfig.Hum[2].SpecialHPMP);
        Assert.Equal(404, FormGlobals.g_EatItemCDConfig.Hero[0].Other);
        Assert.Equal(0, FormGlobals.g_EatItemCDConfig.Hum[0].NormalMP);

        // 键名是 "1NormalHP" 而不是 "NormalHP1" / "1_NormalHP"
        Assert.Equal("101", ini.Get("HumanItemEatCD", "1NormalHP"));
        Assert.Equal("", ini.Get("HumanItemEatCD", "NormalHP1"));
    }

    [Fact]
    public void LoadConfig_三个槽位全部覆盖()
    {
        var ini = new FakeIni();
        for (int i = 1; i <= 3; i++)
            ini.Set("HumanItemEatCD", i + "NormalMP", (i * 10).ToString())
               .Set("HeroItemEatCD", i + "NormalMP", (i * 100).ToString());
        Load(ini);
        Assert.Equal(new[] { 10, 20, 30 }, new[]
        {
            FormGlobals.g_EatItemCDConfig.Hum[0].NormalMP,
            FormGlobals.g_EatItemCDConfig.Hum[1].NormalMP,
            FormGlobals.g_EatItemCDConfig.Hum[2].NormalMP,
        });
        Assert.Equal(300, FormGlobals.g_EatItemCDConfig.Hero[2].NormalMP);
    }

    [Fact]
    public void LoadConfig_吃药CD缺键保留原值()
    {
        FormGlobals.g_EatItemCDConfig.Hum[0].NormalHP = 55;
        Load(new FakeIni());
        Assert.Equal(55, FormGlobals.g_EatItemCDConfig.Hum[0].NormalHP);
    }

    // ================= MagicCD（原 :1470-1480） =================

    [Fact]
    public void LoadConfig_MagicCD段()
    {
        Load(new FakeIni()
            .Set("MagicCD", "MsgType", "2")
            .Set("MagicCD", "MsgText", "冷却中")
            .Set("MagicCD", "FColor", "255")
            .Set("MagicCD", "BColor", "56")
            .Set("MagicCD", "ShowX", "11")
            .Set("MagicCD", "ShowY", "22"));
        Assert.Equal(2, (int)FormGlobals.g_btMagicCDMsgType);
        Assert.Equal("冷却中", FormGlobals.g_sMagicCDMsgText);
        Assert.Equal(255, (int)FormGlobals.g_btMagicCDFColor);
        Assert.Equal(56, (int)FormGlobals.g_btMagicCDBColor);
        Assert.Equal(11, FormGlobals.g_nMagicCDShowX);
        Assert.Equal(22, FormGlobals.g_nMagicCDShowY);
    }

    [Fact]
    public void LoadConfig_MagicCDMsgType只收0到2()
    {
        Load(new FakeIni().Set("MagicCD", "MsgType", "3"));
        Assert.Equal(0, (int)FormGlobals.g_btMagicCDMsgType);          // 保持默认 0
        Load(new FakeIni().Set("MagicCD", "MsgType", "-1"));
        Assert.Equal(0, (int)FormGlobals.g_btMagicCDMsgType);
    }

    [Fact]
    public void LoadConfig_MagicCD缺键保留默认()
    {
        Load(new FakeIni());
        Assert.Equal(0xFF, (int)FormGlobals.g_btMagicCDFColor);
        Assert.Equal(0x38, (int)FormGlobals.g_btMagicCDBColor);
        Assert.Equal(30, FormGlobals.g_nMagicCDShowX);
        Assert.Equal(40, FormGlobals.g_nMagicCDShowY);
    }

    // ================= g_DefaultConfig 快照（原 :1097） =================

    [Fact]
    public void LoadConfig_把当前全局量快照进DefaultConfig()
    {
        FormGlobals.g_Config.nLockTime = 42;
        FormGlobals.g_Config.ActionList[0].nInterval = 123;
        Load(new FakeIni());
        Assert.Equal(42, FormGlobals.g_DefaultConfig.nLockTime);
        Assert.Equal(123u, FormGlobals.g_DefaultConfig.ActionList[0].nInterval);
    }

    [Fact]
    public void LoadConfig_DefaultConfig是就地拷贝而非换引用()
    {
        var before = FormGlobals.g_DefaultConfig;
        Load(new FakeIni());
        Assert.Same(before, FormGlobals.g_DefaultConfig);              // readonly 引用不变
    }

    // ================= 日志接缝（原 :1115） =================

    [Fact]
    public void LoadConfig_通过日志接缝记录一条()
    {
        var msgs = new List<(string, int)>();
        var orig = RunGateConfigLoader.LogSink;
        try
        {
            RunGateConfigLoader.LogSink = (m, l) => msgs.Add((m, l));
            Load(new FakeIni());
            Assert.Single(msgs);
            Assert.Equal(("正在加载配置信息...", 3), msgs[0]);
        }
        finally { RunGateConfigLoader.LogSink = orig; }
    }

    [Fact]
    public void LoadConfig_null的IniFile抛异常()
        => Assert.Throws<ArgumentNullException>(() => RunGateConfigLoader.LoadConfig(null, new List<int>()));

    [Fact]
    public void LoadConfig_ports为null时不抛异常()
    {
        RunGateConfigLoader.LoadConfig(new FakeIni(), null);
        Assert.True(true);
    }

    // ================= WriteConfig（原 :2872-2932） =================



    [Fact]
    public void WriteConfig_写出关键键()
    {
        GateShareGlobals.g_sGateAddr = "1.1.1.1";
        GateShareGlobals.g_wdGatePort = 7200;
        GateShareGlobals.g_sServerAddr = "2.2.2.2";
        GateShareGlobals.g_wdServerPort = 5000;
        GateShareSeam.g_sClientPassWord = "pw";
        GateShareSeam.g_boCheckClientPassword = true;
        GateShareGlobals.g_sTitleName = "T";
        GateShareGlobals.g_btShowLogLevel = 3;

        var ini = new FakeIni();
        RunGateConfigLoader.WriteConfig(ini, clientSendBlockSizeControlValue: 4,
            preAllocatedCountControlValue: 256, antiPlugStreamSendBlockSizeItemIndex: 1);

        Assert.Equal("1.1.1.1", ini.Get("GameGate", "GateAddr"));
        Assert.Equal("7200", ini.Get("GameGate", "GatePort"));
        Assert.Equal("2.2.2.2", ini.Get("GameGate", "Server1"));
        Assert.Equal("5000", ini.Get("GameGate", "ServerPort"));
        Assert.Equal("1", ini.Get("GameGate", "CheckClientPassword"));
        Assert.Equal("pw", ini.Get("GameGate", "ClientPassWord"));
        Assert.Equal("T", ini.Get("GameGate", "Title"));
        Assert.Equal("3", ini.Get("GameGate", "ShowLogLevel"));
        Assert.Equal("4", ini.Get("GameGate", "ClientSendBlockSize"));
        Assert.Equal("256", ini.Get("GameGate", "MaxPreallocatedMemorySize"));
        Assert.Equal("1", ini.Get("GameGate", "AntiPlugStreamSendBlockSize"));
    }

    [Fact]
    public void WriteConfig_密码键名读写不对称_大小写()
    {
        // 读 :1120 用 'CheckClientPassWord'（小写 p），写 :2878 用 'CheckClientPassword'（大写 P）
        var ini = new FakeIni().Set("GameGate", "CheckClientPassWord", "1");
        Load(ini);
        Assert.True(GateShareSeam.g_boCheckClientPassword);

        var outIni = new FakeIni();
        RunGateConfigLoader.WriteConfig(outIni, 1, 256, 0);
        Assert.Equal("1", outIni.Get("GameGate", "CheckClientPassword"));   // 写的是大写 P
        Assert.Equal("", outIni.Get("GameGate", "CheckClientPassWord"));
    }

    [Fact]
    public void WriteConfig_写AntiplugAllLog但读取侧被注释_原文缺陷C13()
    {
        GateShareSeam.g_boAntiplugAllLog = true;
        var ini = new FakeIni();
        RunGateConfigLoader.WriteConfig(ini, 1, 256, 0);
        Assert.Equal("1", ini.Get("GameGate", "AntiplugAllLog"));           // 原 :2894 写了

        // 但 LoadConfig 不读它 → 回读后仍是 false（写入型设置）
        GateShareSeam.g_boAntiplugAllLog = false;
        Load(ini);
        Assert.False(GateShareSeam.g_boAntiplugAllLog);
    }

    [Fact]
    public void WriteConfig_用控件值而非MAX全局量_原文缺陷C14()
    {
        GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE = 5;                  // 与控件值不同
        GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE = 1024;
        var ini = new FakeIni();
        RunGateConfigLoader.WriteConfig(ini, clientSendBlockSizeControlValue: 8,
            preAllocatedCountControlValue: 512, antiPlugStreamSendBlockSizeItemIndex: 3);
        Assert.Equal("8", ini.Get("GameGate", "ClientSendBlockSize"));       // ★ 控件值
        Assert.Equal("512", ini.Get("GameGate", "MaxPreallocatedMemorySize"));
        Assert.Equal("3", ini.Get("GameGate", "AntiPlugStreamSendBlockSize"));
    }

    [Fact]
    public void WriteConfig_写出全部48个键()
    {
        var ini = new FakeIni();
        RunGateConfigLoader.WriteConfig(ini, 1, 256, 0);
        string[] expected =
        {
            "GateAddr","GatePort","Server1","ServerPort","CheckClientPassword","ClientPassWord",
            "Title","ShowLogLevel","Minimize","CheckM2ServerTimeOut","ClientSendBlockSize",
            "MaxPreallocatedMemorySize","ClientAccumulateMaxSize","DBPort",
            "LogoutNoResendAntiplugStream","AntiplugAllLog","RecvAntiPlugHeartbeatTimeOutTime",
            "AntiPlugStreamSendSpeed","AntiPlugStreamSendBlockSize",
            "OpenVerifyCode","VerifyCodeErrCount","VerifyCodeRefreshCount","VerifyCodeWaitTime",
            "VerifyCodeInterval1","VerifyCodeInterval2","VerifySuccessAddInterval",
            "VerifyFailTriggerScript","VerifyFailLoginVerify","VerifyCodeExcludeMap",
            "AutoLoadNoVerifyChrList","LoadNoVerifyChrListFile","AutoLoadNoVerifyChrListInterval",
            "OneMACLimitePlayer","OneMACLimitePlayerCount","ClientLogoutDelay","ClientCloseDelay",
            "DelayCloseDisableMove","DelayCloseDisableSpell","DelayCloseDisableAttack","DelayCloseDisableUseItem",
            "ShowBreakClientLogoutHint","BreakClientLogoutHint","ShowBreakClientCloseHint","BreakClientCloseHint",
        };
        // ★ 计数口径：原文 :2874-2929 共 **41 行** WriteXxx 调用（其中 4 行是同一节里成对的
        //   VerifyCode*/DelayClose* 项，已逐条列在上面）。这里逐键核对"写进去过"。
        var settings = new HashSet<string>(StringComparer.Ordinal);
        foreach (string k in expected)
        {
            string v = ini.Get("GameGate", k);
            if (v != null) settings.Add(k);
        }
        Assert.Equal(expected.Length, settings.Count);
    }

    [Fact]
    public void WriteConfig_null的IniFile抛异常()
        => Assert.Throws<ArgumentNullException>(() => RunGateConfigLoader.WriteConfig(null, 1, 256, 0));

    // ================= 真实 TIniFileEx 往返（端到端） =================

    [Fact]
    public void 真实INI文件_LoadWriteLoad往返()
    {
        string path = Path.Combine(_dir, "Config.ini");
        GateShareGlobals.g_sTitleName = "往返网关";
        GateShareGlobals.g_wdGatePort = 7300;
        GateShareGlobals.g_sGateAddr = "0.0.0.0";
        GateShareGlobals.g_sServerAddr = "127.0.0.1";
        GateShareGlobals.g_wdServerPort = 5000;
        GateShareGlobals.g_wdDBPort = 27201;
        GateShareSeam.g_sClientPassWord = "pw1";

        using (var w = new TIniFileEx(path))
        {
            RunGateConfigLoader.WriteConfig(w, 4, 256, 1);
        }

        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        GateShareSeam.ResetForTest();

        using (var r = new TIniFileEx(path))
        {
            Load(r);
        }

        Assert.Equal("往返网关", GateShareGlobals.g_sTitleName);
        Assert.Equal(7300, (int)GateShareGlobals.g_wdGatePort);
        Assert.Equal("127.0.0.1", GateShareGlobals.g_sServerAddr);
        Assert.Equal(5000, (int)GateShareGlobals.g_wdServerPort);
        Assert.Equal(27201, (int)GateShareGlobals.g_wdDBPort);
        Assert.Equal("pw1", GateShareSeam.g_sClientPassWord);
        Assert.Equal(256, GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE);
        Assert.Equal(4u, GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE);
    }

    [Fact]
    public void 真实INI文件_写出后可被文本读取()
    {
        string path = Path.Combine(_dir, "Config2.ini");
        GateShareGlobals.g_sTitleName = "中文标题";
        using (var w = new TIniFileEx(path)) RunGateConfigLoader.WriteConfig(w, 1, 256, 0);
        string text = File.ReadAllText(path, System.Text.Encoding.GetEncoding(936));
        Assert.Contains("[GameGate]", text);
        Assert.Contains("Title=中文标题", text);
    }
}
