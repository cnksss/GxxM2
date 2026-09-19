using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J58：uCombatPowerUtils.pas 1:1 测试。
/// 属性枚举/名称表/标识表、GetValNameNo 全前缀、变量管理器（二分/增删/INI 往返）、
/// RecalcPlayCombatPower 三职业加权与变量加成、运算符开关门控。
/// </summary>
public sealed class CombatPowerTests : IDisposable
{
    private readonly string _dir;

    public CombatPowerTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j58_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        M2Config.ResetViewList2ConfigDefaults();
        CombatPowerUtils.CombatPowerVarMgr.Clear();
        for (int job = 0; job < 3; job++)
            Array.Clear(CombatPowerUtils.DefCombatPowerValue[job]);
    }

    public void Dispose()
    {
        CombatPowerUtils.CombatPowerVarMgr.Clear();
        M2Config.ResetViewList2ConfigDefaults();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    private void SetAttrib(int job, TCombatPowerAttrib attr, int value)
        => CombatPowerUtils.DefCombatPowerValue[job][(int)attr] = value;

    // ================= 表与枚举 =================

    [Fact]
    public void AttribTables_AlignedAndOrdered()
    {
        Assert.Equal(53, CombatPowerUtils.AttribCount);
        Assert.Equal(53, CombatPowerUtils.AttribNames.Length);
        Assert.Equal(53, CombatPowerUtils.AttribIdents.Length);

        // 枚举顺序即数组下标（Delphi array[TCombatPowerAttrib] 语义）
        Assert.Equal(0, (int)TCombatPowerAttrib.cpaMaxHP);
        Assert.Equal(1, (int)TCombatPowerAttrib.cpaMaxMP);
        Assert.Equal(11, (int)TCombatPowerAttrib.cpaSC2);
        Assert.Equal(12, (int)TCombatPowerAttrib.cpaAntiMagic);
        Assert.Equal(28, (int)TCombatPowerAttrib.cpaBlastHit);
        Assert.Equal(52, (int)TCombatPowerAttrib.cpaUnBlastHit);

        Assert.Equal("MaxHP", CombatPowerUtils.AttribNames[0]);
        Assert.Equal("暴击抗性", CombatPowerUtils.AttribNames[52]);
        Assert.Equal("MaxHP", CombatPowerUtils.AttribIdents[0]);
        Assert.Equal("UnBlastHit", CombatPowerUtils.AttribIdents[52]);
        Assert.Equal("LuckOrUnLuck", CombatPowerUtils.AttribIdents[(int)TCombatPowerAttrib.cpaLuckOrUnLuck]);
        Assert.Equal("AddUndropRate", CombatPowerUtils.AttribIdents[(int)TCombatPowerAttrib.cpaAddUndropRate]);
    }

    // ================= GetValNameNo =================

    [Fact]
    public void GetValNameNo_AllPrefixes()
    {
        Assert.Equal(0, CombatPowerUtils.GetValNameNo("P0"));
        Assert.Equal(999, CombatPowerUtils.GetValNameNo("p999"));       // UpCase
        Assert.Equal(1000, CombatPowerUtils.GetValNameNo("D0"));
        Assert.Equal(1999, CombatPowerUtils.GetValNameNo("D999"));
        Assert.Equal(2000, CombatPowerUtils.GetValNameNo("M0"));
        Assert.Equal(3000, CombatPowerUtils.GetValNameNo("N0"));
        Assert.Equal(4000, CombatPowerUtils.GetValNameNo("I0"));
        Assert.Equal(5000, CombatPowerUtils.GetValNameNo("G0"));
        Assert.Equal(6000, CombatPowerUtils.GetValNameNo("A0"));
        Assert.Equal(7000, CombatPowerUtils.GetValNameNo("S0"));
        Assert.Equal(8000, CombatPowerUtils.GetValNameNo("U0"));
        Assert.Equal(8499, CombatPowerUtils.GetValNameNo("U499"));
        Assert.Equal(8500, CombatPowerUtils.GetValNameNo("T0"));
        Assert.Equal(9000, CombatPowerUtils.GetValNameNo("J0"));
        Assert.Equal(9500, CombatPowerUtils.GetValNameNo("Z0"));
        Assert.Equal(10000, CombatPowerUtils.GetValNameNo("L0"));
        Assert.Equal(10999, CombatPowerUtils.GetValNameNo("L999"));
    }

    [Fact]
    public void GetValNameNo_RejectsDollarPrefixedAndOutOfRange()
    {
        // N$/S$/L$ 走自定义变量（M2Share.pas 11490/11524/11570 原文注释）
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("N$1"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("S$1"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("L$1"));

        // U/T/J/Z 上界 500
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("U500"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("T500"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("J500"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("Z500"));

        // P/D/M/N/I/G/A/S/L 上界 1000
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("P1000"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("D1000"));

        // 未知前缀 / 太短 / 非数字
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("Q1"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("P"));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo(""));
        Assert.Equal(-1, CombatPowerUtils.GetValNameNo("Pabc"));

        // Trim 语义
        Assert.Equal(3000, CombatPowerUtils.GetValNameNo("  N0  "));
    }

    // ================= 变量管理器 =================

    [Fact]
    public void VarMgr_AddRejectsDuplicate_RemoveKeepsSortedList()
    {
        var mgr = new TCombatPowerVarMgr();
        Assert.NotNull(mgr.Add("N0", 10, 20, 30, "测试"));
        Assert.Null(mgr.Add("N0", 1, 1, 1, "重复"));
        Assert.Null(mgr.Add("n0", 1, 1, 1, "大小写重复"));
        Assert.Equal(1, mgr.Count);

        // 大小写不敏感的有序插入：B 在 a 之后
        mgr.Add("a1", 1, 1, 1, "");
        mgr.Add("B1", 1, 1, 1, "");
        mgr.Add("c1", 1, 1, 1, "");
        Assert.True(mgr.DoSearch("B1", out int idxB));
        Assert.True(mgr.DoSearch("c1", out int idxC));
        Assert.True(idxB < idxC);

        Assert.NotNull(mgr.GetValueRecord("a1"));
        Assert.True(mgr.DoSearch("b1", out _));        // 不区分大小写命中
        Assert.Null(mgr.GetValueRecord("zz"));

        Assert.True(mgr.Remove("B1"));
        Assert.False(mgr.Remove("B1"));
        Assert.Equal(3, mgr.Count);
    }

    [Fact]
    public void VarMgr_LoadSaveConfig_RoundTrip()
    {
        // 先写 [DefaultAttrib] + [N0] + [N1]
        Directory.CreateDirectory(M2Config.sEnvirDir);
        File.WriteAllText(M2Config.sEnvirDir + "CombatPower.ini",
            "[DefaultAttrib]" + Environment.NewLine + "MaxHP_0=7" + Environment.NewLine
            + "[N0]" + Environment.NewLine + "Value0=10" + Environment.NewLine + "Value1=20"
            + Environment.NewLine + "Value2=30" + Environment.NewLine + "Desc=战士变量" + Environment.NewLine
            + "[N1]" + Environment.NewLine + "Value0=1" + Environment.NewLine + "Value1=2"
            + Environment.NewLine + "Value2=3" + Environment.NewLine + "Desc=" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        // DefaultAttrib 读入 3×53
        CombatPowerUtils.LoadDefCombatPowerConfig();
        Assert.Equal(7, CombatPowerUtils.DefCombatPowerValue[0][(int)TCombatPowerAttrib.cpaMaxHP]);
        Assert.Equal(0, CombatPowerUtils.DefCombatPowerValue[1][(int)TCombatPowerAttrib.cpaMaxHP]);

        // 变量表跳过 DefaultAttrib 节
        var mgr = CombatPowerUtils.CombatPowerVarMgr;
        mgr.LoadConfig();
        Assert.Equal(2, mgr.Count);
        var rec = mgr.GetValueRecord("N0")!;
        Assert.Equal(10, rec.Value0);
        Assert.Equal(20, rec.Value1);
        Assert.Equal(30, rec.Value2);
        Assert.Equal("战士变量", rec.Desc);

        // 存盘后 DefaultAttrib 键值保留、变量节重写
        SetAttrib(0, TCombatPowerAttrib.cpaMaxHP, 7);
        SetAttrib(1, TCombatPowerAttrib.cpaMaxHP, 9);
        CombatPowerUtils.SaveDefCombatPowerConfig();
        mgr.SaveConfig();

        string text = File.ReadAllText(M2Config.sEnvirDir + "CombatPower.ini", GXX.Core.EncodingInit.GBK);
        Assert.Contains("MaxHP_0=7", text);
        Assert.Contains("MaxHP_1=9", text);
        Assert.Contains("[N0]", text);
        Assert.Contains("Value0=10", text);
        Assert.Contains("Desc=战士变量", text);

        var mgr2 = new TCombatPowerVarMgr();
        mgr2.LoadConfig();
        Assert.Equal(2, mgr2.Count);
        Assert.Equal(10, mgr2.GetValueRecord("N0")!.Value0);
    }

    // ================= RecalcPlayCombatPower =================

    [Fact]
    public void Recalc_DisabledSwitch_ZeroesPower()
    {
        M2Config.boOpenCombatPowerCalc = false;
        var obj = new TSpellCaster { m_nCombatPower = 12345 };
        CombatPowerUtils.RecalcPlayCombatPower(obj);
        Assert.Equal(0, obj.m_nCombatPower);
    }

    [Fact]
    public void Recalc_JobWeightedSum_MatchesFormula()
    {
        M2Config.boOpenCombatPowerCalc = true;
        M2Config.boOpenCombatPowerVarCalc = false;

        var obj = new TSpellCaster { m_btJob = 0, m_btRaceServer = Grobal2Const.RC_MONSTER };
        obj.m_wAbil.MaxHP = 1000;      // Round(1000/1000 * 2) = 2
        obj.m_wAbil.MaxMP = 500;       // Round(0.5 * 4) = 2（银行家舍入 .5→2）
        obj.m_wAbil.AC1 = 3;
        obj.m_wAbil.DC2 = 10;
        obj.m_nAntiMagic = 5;
        obj.m_btHitPoint = 4;
        obj.m_nLuck = 2;
        obj.m_dwParalysisRate = 1;
        obj.m_boRevival = true;
        obj.m_boMagicShield = true;
        obj.m_wAbil.SetNewValue(0, 6);   // 暴击几率

        SetAttrib(0, TCombatPowerAttrib.cpaMaxHP, 2);
        SetAttrib(0, TCombatPowerAttrib.cpaMaxMP, 4);
        SetAttrib(0, TCombatPowerAttrib.cpaAC1, 5);
        SetAttrib(0, TCombatPowerAttrib.cpaDC2, 3);
        SetAttrib(0, TCombatPowerAttrib.cpaAntiMagic, 7);
        SetAttrib(0, TCombatPowerAttrib.cpaHitPoint, 2);
        SetAttrib(0, TCombatPowerAttrib.cpaLuckOrUnLuck, 10);
        SetAttrib(0, TCombatPowerAttrib.cpaParalysisRate, 100);
        SetAttrib(0, TCombatPowerAttrib.cpaRevival, 1000);
        SetAttrib(0, TCombatPowerAttrib.cpaMagicShield, 2000);
        SetAttrib(0, TCombatPowerAttrib.cpaBlastHit, 50);

        // 2 + 2 + 15 + 30 + 35 + 8 + 20 + 100 + 1000 + 2000 + 300 = 3512
        CombatPowerUtils.RecalcPlayCombatPower(obj);
        Assert.Equal(3512, obj.m_nCombatPower);

        // 法师用 [1] 组（全 0）→ 仅 MaxHP/MaxMP 的 Round(0 * x) = 0
        var mage = new TSpellCaster { m_btJob = 1 };
        mage.m_wAbil.MaxHP = 1000;
        mage.m_wAbil.AC1 = 3;
        CombatPowerUtils.RecalcPlayCombatPower(mage);
        Assert.Equal(0, mage.m_nCombatPower);

        SetAttrib(1, TCombatPowerAttrib.cpaMaxHP, 2);
        SetAttrib(1, TCombatPowerAttrib.cpaAC1, 5);
        CombatPowerUtils.RecalcPlayCombatPower(mage);
        Assert.Equal(17, mage.m_nCombatPower);   // 2 + 15
    }

    [Fact]
    public void Recalc_VarBonus_OnlyForPlayersAndWhenVarSwitchOn()
    {
        M2Config.boOpenCombatPowerCalc = true;
        SetAttrib(0, TCombatPowerAttrib.cpaMaxHP, 1);

        var player = new TPlayObject { m_btJob = 0 };
        player.m_wAbil.MaxHP = 1000;                     // 基础 = 1
        player.m_DyVal[5] = 7;                           // D5
        player.m_nMval[3] = 2;                           // M3
        player.m_nInteger[9] = 5;                        // N9
        player.m_UVal[10] = 4;                           // U10
        player.m_JVal[1] = 6;                            // J1
        player.m_StringList.Add("S$A", "11");            // S$A = 11
        player.m_IntegerList.Add("N$B", 13);             // N$B = 13

        CombatPowerUtils.CombatPowerVarMgr.Add("D5", 100, 0, 0, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("M3", 0, 1, 0, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("N9", 0, 0, 1, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("U10", 2, 0, 0, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("J1", 3, 0, 0, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("S$A", 1, 0, 0, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("N$B", 1, 0, 0, "");
        // P 与 I/G/A/S/T 在原文被注释掉 → 视为无加成
        CombatPowerUtils.CombatPowerVarMgr.Add("P0", 999, 999, 999, "");
        CombatPowerUtils.CombatPowerVarMgr.Add("T0", 999, 999, 999, "");

        // 变量开关关闭 → 仅基础 1
        M2Config.boOpenCombatPowerVarCalc = false;
        CombatPowerUtils.RecalcPlayCombatPower(player);
        Assert.Equal(1, player.m_nCombatPower);

        // 开启 → 1 + 7*100 + 2*1(法师组) + 5*1(道士组) + 4*2 + 6*3 + 11*1 + 13*1
        M2Config.boOpenCombatPowerVarCalc = true;
        CombatPowerUtils.RecalcPlayCombatPower(player);
        Assert.Equal(1 + 700 + 0 + 0 + 8 + 18 + 11 + 13, player.m_nCombatPower);

        // 法师：D5 的 Value1=0 → 无加成；M3 的 Value1=1 → 2*1；N9 的 Value1=0 → 0
        var mage = new TPlayObject { m_btJob = 1 };
        mage.m_DyVal[5] = 7;
        mage.m_nMval[3] = 2;
        mage.m_nInteger[9] = 5;
        CombatPowerUtils.RecalcPlayCombatPower(mage);
        Assert.Equal(2, mage.m_nCombatPower);

        // 非玩家（怪物）即使开关打开也不计变量
        var mon = new TSpellCaster { m_btJob = 0, m_btRaceServer = Grobal2Const.RC_MONSTER };
        mon.m_wAbil.MaxHP = 1000;
        CombatPowerUtils.RecalcPlayCombatPower(mon);
        Assert.Equal(1, mon.m_nCombatPower);
    }

    [Fact]
    public void Recalc_MissingVarYieldsNoBonus_AndZeroPowerVarSkipped()
    {
        M2Config.boOpenCombatPowerCalc = true;
        M2Config.boOpenCombatPowerVarCalc = true;
        var player = new TPlayObject { m_btJob = 0 };
        // 变量表有记录但玩家槽位为 0 → 无加成
        CombatPowerUtils.CombatPowerVarMgr.Add("D1", 1000, 1000, 1000, "");
        CombatPowerUtils.RecalcPlayCombatPower(player);
        Assert.Equal(0, player.m_nCombatPower);

        // 槽位非 0 但职业威力值为 0 → 仍无加成
        player.m_DyVal[1] = 5;
        CombatPowerUtils.CombatPowerVarMgr.Clear();
        CombatPowerUtils.CombatPowerVarMgr.Add("D1", 0, 0, 0, "");
        CombatPowerUtils.RecalcPlayCombatPower(player);
        Assert.Equal(0, player.m_nCombatPower);
    }

    [Fact]
    public void PlayObject_VarArrays_SizedPerGrobal2()
    {
        var player = new TPlayObject();
        Assert.Equal(1000, player.m_DyVal.Length);
        Assert.Equal(1000, player.m_nMval.Length);
        Assert.Equal(1000, player.m_nInteger.Length);
        Assert.Equal(500, player.m_UVal.Length);
        Assert.Equal(500, player.m_JVal.Length);
        Assert.Equal(500, player.m_ZVal.Length);
        Assert.Equal(-1, player.m_StringList.GetIndex("X"));
        Assert.Equal(-1, player.m_IntegerList.GetIndex("X"));
    }
}
