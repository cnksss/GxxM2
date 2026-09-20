// 测试：Source\RunGate\GateShare.pas（实测 LF 3595）的**判定谓词 / 定时 / 口令 / 版本 / GameCenter**族
//   → src/GXX.RunGate/GateShareRuntime.cs
//
// 覆盖策略：每个公开成员 ≥3 用例；原文缺陷写成差异断言并标注 `原文缺陷 X`（编号与 GateShareRuntime.cs 文件头一致）。
using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]      // 共享 FormGlobals / GateShareGlobals 静态量，必须串行
public sealed class GateShareRuntimeTests : IDisposable
{
    public GateShareRuntimeTests()
    {
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
    }

    public void Dispose()
    {
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
    }

    // ================= tick_diff（原 :1458-1464） =================

    [Fact]
    public void TickDiff_无回绕()
    {
        Assert.Equal(100u, GateShareRuntime.tick_diff(1000, 1100));
        Assert.Equal(0u, GateShareRuntime.tick_diff(1000, 1000));
    }

    [Fact]
    public void TickDiff_回绕()
    {
        Assert.Equal(8u, GateShareRuntime.tick_diff(uint.MaxValue - 4, 4));   // 4294967295 - (2^32-5) + 4 = 8
        Assert.Equal(uint.MaxValue, GateShareRuntime.tick_diff(0, uint.MaxValue));
    }

    [Fact]
    public void TickDiff_反向也返回正差值()
    {
        Assert.Equal(uint.MaxValue - 500, GateShareRuntime.tick_diff(1000, 500));
    }

    // ================= CheckInFYDenyIPList / Pass（原 :2992-3030） =================

    [Fact]
    public void CheckInFYDenyIPList_本地名单命中()
    {
        GateShareGlobals.g_FYDenyIPList.Add("1.2.3.4");
        Assert.True(GateShareRuntime.CheckInFYDenyIPList("1.2.3.4"));
        Assert.False(GateShareRuntime.CheckInFYDenyIPList("1.2.3.5"));
    }

    [Fact]
    public void CheckInFYDenyIPList_回落到下载名单()
    {
        GateShareGlobals.g_FYDownDenyIPList.Add("9.9.9.9");
        Assert.True(GateShareRuntime.CheckInFYDenyIPList("9.9.9.9"));
        Assert.False(GateShareRuntime.CheckInFYDenyIPList("9.9.9.8"));
    }

    [Fact]
    public void CheckInFYDenyIPList_两边都空()
    {
        Assert.False(GateShareRuntime.CheckInFYDenyIPList("1.1.1.1"));
        Assert.False(GateShareRuntime.CheckInFYDenyIPList(""));
    }

    [Fact]
    public void CheckInFYPassIPList_本地与下载名单()
    {
        GateShareGlobals.g_FYPassIPList.Add("2.2.2.2");
        GateShareGlobals.g_FYDownPassIPList.Add("3.3.3.3");
        Assert.True(GateShareRuntime.CheckInFYPassIPList("2.2.2.2"));
        Assert.True(GateShareRuntime.CheckInFYPassIPList("3.3.3.3"));
        Assert.False(GateShareRuntime.CheckInFYPassIPList("4.4.4.4"));
    }

    [Fact]
    public void CheckInFYPassIPList_不与Deny名单串场()
    {
        GateShareGlobals.g_FYDenyIPList.Add("5.5.5.5");
        Assert.False(GateShareRuntime.CheckInFYPassIPList("5.5.5.5"));
    }

    // ================= IsBlockIP（原 :3032-3075） =================

    [Fact]
    public void IsBlockIP_临时名单优先()
    {
        GateShareLists.AddTempBlockIP("1.2.3.4");
        Assert.True(GateShareRuntime.IsBlockIP("1.2.3.4"));
        Assert.False(GateShareRuntime.IsBlockIP("1.2.3.5"));
    }

    [Fact]
    public void IsBlockIP_永久名单()
    {
        GateShareLists.AddBlockIP("10.1.1.1");
        Assert.True(GateShareRuntime.IsBlockIP("10.1.1.1"));
    }

    [Fact]
    public void IsBlockIP_IP段区间含端点()
    {
        GateShareGlobals.AddIPSection(new TIPSection
        {
            nBeginAddr = GateShareInet.IP2Long("10.0.0.10"),
            nEndAddr = GateShareInet.IP2Long("10.0.0.20"),
        });
        Assert.True(GateShareRuntime.IsBlockIP("10.0.0.10"));    // 左端点含
        Assert.True(GateShareRuntime.IsBlockIP("10.0.0.20"));    // 右端点含
        Assert.True(GateShareRuntime.IsBlockIP("10.0.0.15"));
        Assert.False(GateShareRuntime.IsBlockIP("10.0.0.9"));
        Assert.False(GateShareRuntime.IsBlockIP("10.0.0.21"));
    }

    [Fact]
    public void IsBlockIP_空IP段与空名单()
    {
        Assert.False(GateShareRuntime.IsBlockIP("8.8.8.8"));
        GateShareGlobals.AddIPSection(new TIPSection { nBeginAddr = 100, nEndAddr = 50 });
        Assert.False(GateShareRuntime.IsBlockIP("8.8.8.8"));     // begin > end 永不命中
    }

    [Fact]
    public void IsBlockIP_非法IP与已入库的非法项()
    {
        // 非法字符串以 nIPaddr = -1 入库（容器缺陷 1），用同一个非法串仍能命中
        GateShareLists.AddBlockIP("garbage");
        Assert.True(GateShareRuntime.IsBlockIP("garbage"));
        // IP2Long 对任意非法串都给 0xFFFFFFFF（不在任何合法区间内）
        Assert.False(GateShareRuntime.IsBlockIP("1.1.1.1"));
    }

    // ================= IsBlockMac（原 :3077-3116） =================

    [Fact]
    public void IsBlockMac_四级短路()
    {
        GateShareLists.AddTempBlockMac("MAC-TEMP");
        GateShareLists.AddBlockMac("MAC-BLOCK");
        GateShareGlobals.g_FYDenyMACList.Add("MAC-FY");
        GateShareGlobals.g_FYDownDenyMACList.Add("MAC-DOWN");

        Assert.True(GateShareRuntime.IsBlockMac("MAC-TEMP"));
        Assert.True(GateShareRuntime.IsBlockMac("MAC-BLOCK"));
        Assert.True(GateShareRuntime.IsBlockMac("MAC-FY"));
        Assert.True(GateShareRuntime.IsBlockMac("MAC-DOWN"));
        Assert.False(GateShareRuntime.IsBlockMac("MAC-OTHER"));
    }

    [Fact]
    public void IsBlockMac_大小写不敏感()
    {
        GateShareGlobals.g_FYDenyMACList.Add("AA-BB-CC");
        Assert.True(GateShareRuntime.IsBlockMac("aa-bb-cc"));
        GateShareGlobals.g_TempMacList.Add("DD-EE-FF");
        Assert.True(GateShareRuntime.IsBlockMac("dd-ee-ff"));
    }

    [Fact]
    public void IsBlockMac_空串与空名单()
    {
        Assert.False(GateShareRuntime.IsBlockMac(""));
        GateShareGlobals.g_TempMacList.Add("");                  // 空串可入表
        Assert.True(GateShareRuntime.IsBlockMac(""));
    }

    // ================= IsConnLimited（原 :3118-3168） =================

    private static void SetupLimits(uint time1, uint limit1, uint time2, uint limit2, int maxConn, uint defense)
    {
        FormGlobals.g_dwIPCountLimitTime1 = time1;
        FormGlobals.g_dwIPCountLimit1 = limit1;
        FormGlobals.g_dwIPCountLimitTime2 = time2;
        FormGlobals.g_dwIPCountLimit2 = limit2;
        FormGlobals.g_nMaxConnOfIPaddr = maxConn;
        FormGlobals.g_dwDefenseLevel = defense;
    }

    [Fact]
    public void IsConnLimited_首次调用只建表并置nCount为1()
    {
        GateShareRuntime.TickProvider = () => 5000;
        SetupLimits(1000, 1, 1000, 1, 100, 1);

        Assert.False(GateShareRuntime.IsConnLimited("1.2.3.4"));   // 原 :3161-3163 只 Add + nCount := 1
        Assert.Equal(1, GateShareRuntime.GetConnectCountOfIP("1.2.3.4"));
        Assert.Equal(0, GateShareGlobals.g_CurrIPList.Find("1.2.3.4").nIPCount1);
    }

    [Fact]
    public void IsConnLimited_第二个窗口阈值触发()
    {
        GateShareRuntime.TickProvider = () => 5000;
        SetupLimits(1000, 1, 1000, 1, 100, 1);

        GateShareRuntime.IsConnLimited("1.2.3.4");   // 建表
        Assert.False(GateShareRuntime.IsConnLimited("1.2.3.4"));   // tick_diff(0,5000)=5000 >= 1000 → 重置窗口
        Assert.True(GateShareRuntime.IsConnLimited("1.2.3.4"));    // tick_diff(5000,5000)=0 < 1000 → nIPCount1=1 >= 1 → true
        Assert.Equal(3, GateShareRuntime.GetConnectCountOfIP("1.2.3.4"));
    }

    [Fact]
    public void IsConnLimited_连接数超过上限触发()
    {
        GateShareRuntime.TickProvider = () => 100;
        SetupLimits(100000, 1000, 100000, 1000, 2, 1);             // 窗口极大 → 只走 nCount 判据

        GateShareRuntime.IsConnLimited("2.2.2.2");                 // nCount = 1
        Assert.False(GateShareRuntime.IsConnLimited("2.2.2.2"));   // nCount = 2，2 > 2 为假
        Assert.True(GateShareRuntime.IsConnLimited("2.2.2.2"));    // nCount = 3 > 2 → true
    }

    [Fact]
    public void IsConnLimited_防御等级0被回写为1_原文缺陷I()
    {
        GateShareRuntime.TickProvider = () => 100;
        SetupLimits(1000, 5, 1000, 5, 100, 0);
        GateShareRuntime.IsConnLimited("3.3.3.3");
        Assert.Equal(1u, FormGlobals.g_dwDefenseLevel);            // 原 :3123 回写全局
    }

    [Fact]
    public void IsConnLimited_阈值乘以防御等级()
    {
        GateShareRuntime.TickProvider = () => 5000;
        SetupLimits(1000, 5, 1000, 5, 100, 3);                     // 阈值 = 5 * 3 = 15

        GateShareRuntime.IsConnLimited("4.4.4.4");                 // 建表
        GateShareRuntime.IsConnLimited("4.4.4.4");                 // 重置窗口
        for (int i = 0; i < 14; i++) Assert.False(GateShareRuntime.IsConnLimited("4.4.4.4"));
        Assert.True(GateShareRuntime.IsConnLimited("4.4.4.4"));    // 第 15 次 → 触发
    }

    [Fact]
    public void IsConnLimited_不同IP各算一份()
    {
        GateShareRuntime.TickProvider = () => 5000;
        SetupLimits(1000, 1, 1000, 1, 100, 1);
        GateShareRuntime.IsConnLimited("1.1.1.1");
        GateShareRuntime.IsConnLimited("2.2.2.2");
        Assert.Equal(1, GateShareRuntime.GetConnectCountOfIP("1.1.1.1"));
        Assert.Equal(1, GateShareRuntime.GetConnectCountOfIP("2.2.2.2"));
        Assert.Equal(0, GateShareRuntime.GetConnectCountOfIP("3.3.3.3"));
    }

    [Fact]
    public void IsConnLimited_窗口计时推进后再计数()
    {
        uint now = 0;
        GateShareRuntime.TickProvider = () => now;
        SetupLimits(1000, 2, 1000, 2, 100, 1);

        GateShareRuntime.IsConnLimited("5.5.5.5");   // 建表
        now = 0;
        GateShareRuntime.IsConnLimited("5.5.5.5");   // nCount=2，tick 未走 → nIPCount1 = 1
        Assert.Equal(1, GateShareGlobals.g_CurrIPList.Find("5.5.5.5").nIPCount1);
        now = 1001;
        GateShareRuntime.IsConnLimited("5.5.5.5");   // tick_diff(0,1001) >= 1000 → 重置
        Assert.Equal(0, GateShareGlobals.g_CurrIPList.Find("5.5.5.5").nIPCount1);
        Assert.Equal(1001u, GateShareGlobals.g_CurrIPList.Find("5.5.5.5").dwIPCountTick1);
    }

    // ================= GetAttackCountOfIP（原 :3170-3183） =================

    [Fact]
    public void GetAttackCountOfIP_命中与未命中()
    {
        var a = GateShareGlobals.g_AttackIPaddrList.Add("1.2.3.4");
        a.nAttackCount = 7;
        Assert.Equal(7, GateShareRuntime.GetAttackCountOfIP("1.2.3.4"));
        Assert.Equal(0, GateShareRuntime.GetAttackCountOfIP("1.2.3.5"));
    }

    [Fact]
    public void GetAttackCountOfIP_空名单为0()
    {
        Assert.Equal(0, GateShareRuntime.GetAttackCountOfIP("9.9.9.9"));
        Assert.Equal(0, GateShareRuntime.GetAttackCountOfIP(""));
    }

    [Fact]
    public void GetAttackCountOfIP_负计数值原样返回()
    {
        var a = GateShareGlobals.g_AttackIPaddrList.Add("1.1.1.1");
        a.nAttackCount = -3;
        Assert.Equal(-3, GateShareRuntime.GetAttackCountOfIP("1.1.1.1"));
    }

    // ================= GetConnectCountOfIP（原 :3185-3198） =================

    [Fact]
    public void GetConnectCountOfIP_命中与未命中()
    {
        GateShareGlobals.g_CurrIPList.Add("1.1.1.1").nCount = 42;
        Assert.Equal(42, GateShareRuntime.GetConnectCountOfIP("1.1.1.1"));
        Assert.Equal(0, GateShareRuntime.GetConnectCountOfIP("2.2.2.2"));
    }

    [Fact]
    public void GetConnectCountOfIP_空名单为0()
    {
        Assert.Equal(0, GateShareRuntime.GetConnectCountOfIP("1.1.1.1"));
        Assert.Equal(0, GateShareRuntime.GetConnectCountOfIP(""));
    }

    [Fact]
    public void GetConnectCountOfIP_只读不改()
    {
        GateShareGlobals.g_CurrIPList.Add("1.1.1.1").nCount = 5;
        GateShareRuntime.GetConnectCountOfIP("1.1.1.1");
        GateShareRuntime.GetConnectCountOfIP("1.1.1.1");
        Assert.Equal(5, GateShareGlobals.g_CurrIPList.Find("1.1.1.1").nCount);
    }

    // ================= InitIntervals（原 :3200-3224） =================

    [Fact]
    public void InitIntervals_每个模式填满401槽()
    {
        GateShareRuntime.InitIntervals();
        for (int m = 0; m < RunGateConst.ActionModeCount; m++)
        {
            Assert.Equal(RunGateConst.SpeedIntervalsCount, FormGlobals.g_wActionSpeedIntervals[m].Length);
            ushort v = FormGlobals.g_wActionSpeedIntervals[m][0];
            for (int i = 0; i < RunGateConst.SpeedIntervalsCount; i++)
                Assert.Equal(v, FormGlobals.g_wActionSpeedIntervals[m][i]);
        }
    }

    [Fact]
    public void InitIntervals_按数组序取值_注释错位_原文缺陷H()
    {
        GateShareRuntime.InitIntervals();

        // ★ 缺陷 H：原文 :3207 的行尾注释写"走路到魔法, 魔法到走路"，但下标 8/9 实为 amRunToHit/amHitToRun。
        //   本断言只认**数组序**（下标 6/7 同样如此：注释说"走路到攻击, 攻击到走路"恰好对上）。
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][0]);
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpell][0]);
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalk][0]);
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amRun][0]);
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amTurn][0]);
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amCutMeat][0]);

        Assert.Equal(540, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalkToHit][0]);     // 下标 6
        Assert.Equal(600, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHitToWalk][0]);     // 下标 7
        Assert.Equal(540, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amRunToHit][0]);      // 下标 8（注释错位）
        Assert.Equal(1200, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHitToRun][0]);     // 下标 9
        Assert.Equal(540, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalkToSpell][0]);   // 下标 10
        Assert.Equal(1200, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpellToWalk][0]);  // 下标 11
    }

    [Fact]
    public void InitIntervals_末尾两个并发模式落回0_原文缺陷H()
    {
        GateShareRuntime.InitIntervals();
        // 原文只有 25 个初始值，枚举有 27 个成员 → amSpellConcurrent(25)/amMoveConcurrent(26) 为 0
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amMoveToCutMeat][0]);      // 下标 22
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amCutMeatToMove][0]);      // 下标 23
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHitConcurrent][0]);      // 下标 24
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpellConcurrent][0]);    // 下标 25（无初始值）
        Assert.Equal(0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amMoveConcurrent][0]);     // 下标 26（无初始值）

        Assert.Equal(25, GateShareRuntime.InitActionIntervals.Length);   // ← 字面量个数
        Assert.Equal(27, RunGateConst.ActionModeCount);                  // ← 枚举成员个数
    }

    [Fact]
    public void InitIntervals_可重复调用且覆盖旧值()
    {
        FormGlobals.g_wActionSpeedIntervals[0][400] = 12345;
        GateShareRuntime.InitIntervals();
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[0][400]);
        GateShareRuntime.InitIntervals();
        Assert.Equal(1000, FormGlobals.g_wActionSpeedIntervals[0][400]);
    }

    // ================= InputPassword / InputPasswordEx（原 :3389-3473） =================

    [Fact]
    public void InputPasswordEx_确定时回写Value并返回True()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => { v = "typed"; return true; };
            string value = "old";
            Assert.True(GateShareRuntime.InputPasswordEx("c", "p", ref value));
            Assert.Equal("typed", value);
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPasswordEx_取消时不改Value并返回False()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => false;
            string value = "old";
            Assert.False(GateShareRuntime.InputPasswordEx("c", "p", ref value));
            Assert.Equal("old", value);
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPasswordEx_把初值传给对话框()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            string seen = null;
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => { seen = v; return false; };
            string value = "preset";
            GateShareRuntime.InputPasswordEx("caption", "prompt", ref value);
            Assert.Equal("preset", seen);      // 原 :3442 `Edit.Text := Value;`
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPassword_取消时返回ADefault()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => false;
            Assert.Equal("dflt", GateShareRuntime.InputPassword("c", "p", "dflt"));
            Assert.Equal("", GateShareRuntime.InputPassword("c", "p"));    // 默认参数 = ''
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPassword_确定时返回输入值()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => { v = "abc"; return true; };
            Assert.Equal("abc", GateShareRuntime.InputPassword("c", "p", "dflt"));
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPassword_忽略布尔返回值_取消且无默认时返回空串()
    {
        // 原文 `Result := ADefault; InputPasswordEx(...)` —— 返回值被丢弃（:3392）
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            bool called = false;
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => { called = true; v = "ignored"; return false; };
            Assert.Equal("", GateShareRuntime.InputPassword("c", "p"));
            Assert.True(called);
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    [Fact]
    public void InputPassword_null默认值按空串处理()
    {
        var original = GateShareRuntime.InputPasswordQuery;
        try
        {
            GateShareRuntime.InputPasswordQuery = (string cap, string prm, ref string v) => false;
            Assert.Equal("", GateShareRuntime.InputPassword("c", "p", null));
        }
        finally { GateShareRuntime.InputPasswordQuery = original; }
    }

    // ================= MyGetTickCount（原 :3496-3502） =================

    [Fact]
    public void MyGetTickCount_可注入()
    {
        var original = GateShareRuntime.TickProvider;
        try
        {
            GateShareRuntime.TickProvider = () => 123456u;
            Assert.Equal(123456u, GateShareRuntime.MyGetTickCount());
        }
        finally { GateShareRuntime.TickProvider = original; }
    }

    [Fact]
    public void MyGetTickCount_默认实现单调不减()
    {
        uint a = GateShareRuntime.MyGetTickCount();
        uint b = GateShareRuntime.MyGetTickCount();
        Assert.True(unchecked(b - a) < 60000u);      // 同一进程内两次调用相隔 < 60s
    }

    [Fact]
    public void MyGetTickCount_每次调用都重新求值()
    {
        var original = GateShareRuntime.TickProvider;
        try
        {
            int calls = 0;
            GateShareRuntime.TickProvider = () => (uint)(++calls);
            Assert.Equal(1u, GateShareRuntime.MyGetTickCount());
            Assert.Equal(2u, GateShareRuntime.MyGetTickCount());
            Assert.Equal(2, calls);
        }
        finally { GateShareRuntime.TickProvider = original; }
    }

    // ================= GetFileVersionNumber / Str（原 :3505-3543） =================

    [Fact]
    public void GetFileVersionNumber_文件不存在返回全0()
    {
        string p = Path.Combine(Path.GetTempPath(), "p2rg-nover-" + Guid.NewGuid().ToString("N") + ".exe");
        var v = GateShareRuntime.GetFileVersionNumber(p);
        Assert.Equal(0, v.Major);
        Assert.Equal(0, v.Minor);
        Assert.Equal(0, v.Release);
        Assert.Equal(0, v.Build);
    }

    [Fact]
    public void GetFileVersionNumber_取不到版本资源返回全0()
    {
        var original = GateShareRuntime.FileVersionProvider;
        try
        {
            string p = Path.GetTempFileName();
            try
            {
                GateShareRuntime.FileVersionProvider = _ => null;
                var v = GateShareRuntime.GetFileVersionNumber(p);
                Assert.Equal(0, v.Major);
            }
            finally { File.Delete(p); }
        }
        finally { GateShareRuntime.FileVersionProvider = original; }
    }

    [Fact]
    public void GetFileVersionNumber_按注入值映射四个字段()
    {
        var original = GateShareRuntime.FileVersionProvider;
        try
        {
            string p = Path.GetTempFileName();
            try
            {
                GateShareRuntime.FileVersionProvider = _ => new GateShareRuntime.TVersionNumber
                {
                    Major = 1, Minor = 2, Release = 3, Build = 4,
                };
                var v = GateShareRuntime.GetFileVersionNumber(p);
                Assert.Equal(1, v.Major);
                Assert.Equal(2, v.Minor);
                Assert.Equal(3, v.Release);
                Assert.Equal(4, v.Build);
            }
            finally { File.Delete(p); }
        }
        finally { GateShareRuntime.FileVersionProvider = original; }
    }

    [Fact]
    public void GetFileVersionStr_四段点分()
    {
        var original = GateShareRuntime.FileVersionProvider;
        try
        {
            string p = Path.GetTempFileName();
            try
            {
                GateShareRuntime.FileVersionProvider = _ => new GateShareRuntime.TVersionNumber
                {
                    Major = 10, Minor = 20, Release = 30, Build = 40,
                };
                Assert.Equal("10.20.30.40", GateShareRuntime.GetFileVersionStr(p));
            }
            finally { File.Delete(p); }
        }
        finally { GateShareRuntime.FileVersionProvider = original; }
    }

    [Fact]
    public void GetFileVersionStr_不存在文件给0_0_0_0()
    {
        string p = Path.Combine(Path.GetTempPath(), "p2rg-nover2-" + Guid.NewGuid().ToString("N") + ".exe");
        Assert.Equal("0.0.0.0", GateShareRuntime.GetFileVersionStr(p));
    }

    [Fact]
    public void GetFileVersionNumber_Word字段截断()
    {
        // 原文 `Result.Minor := dwFileVersionMS;` 把 32 位值赋给 Word → 低 16 位（缺陷 L）
        var original = GateShareRuntime.FileVersionProvider;
        try
        {
            string p = Path.GetTempFileName();
            try
            {
                GateShareRuntime.FileVersionProvider = _ => new GateShareRuntime.TVersionNumber
                {
                    Major = 0xFFFF, Minor = 0xFFFF, Release = 0xFFFF, Build = 0xFFFF,
                };
                var v = GateShareRuntime.GetFileVersionNumber(p);
                Assert.Equal(ushort.MaxValue, v.Major);      // 高位被截断（字段类型本身是 ushort）
            }
            finally { File.Delete(p); }
        }
        finally { GateShareRuntime.FileVersionProvider = original; }
    }

    [Fact]
    public void GetFileVersionNumber_真实文件不抛异常()
    {
        var ex = Record.Exception(() => GateShareRuntime.GetFileVersionNumber(typeof(GateShareRuntimeTests).Assembly.Location));
        Assert.Null(ex);
    }

    [Fact]
    public void GetFileVersionNumber_空路径不抛异常()
    {
        Assert.Equal(0, GateShareRuntime.GetFileVersionNumber("").Major);
        Assert.Equal(0, GateShareRuntime.GetFileVersionNumber(null).Major);
    }

    // ================= SendGameCenterMsg（原 :2523-2534） =================

    private sealed class CopyDataProbe
    {
        public IntPtr Hwnd;
        public int WParam;
        public int CbData;
        public byte[] Data;
        public int Calls;
        public void Capture(IntPtr hwnd, int wParam, int cbData, byte[] data)
        {
            Calls++; Hwnd = hwnd; WParam = wParam; CbData = cbData; Data = data;
        }
    }

    [Fact]
    public void SendGameCenterMsg_wParam是MakeLong()
    {
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareGlobals.g_dwGameCenterHandle = (IntPtr)0x1234;
            GateShareRuntime.SendGameCenterMsg(0xABCD, "abc");

            Assert.Equal(1, probe.Calls);
            Assert.Equal((IntPtr)0x1234, probe.Hwnd);
            Assert.Equal(unchecked((int)((uint)GateShareRuntime.tRunGate | ((uint)0xABCD << 16))), probe.WParam);   // 原 :2528
            Assert.Equal(unchecked((int)0xABCD0008u), probe.WParam);
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    [Fact]
    public void SendGameCenterMsg_负载含结尾NUL()
    {
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareRuntime.SendGameCenterMsg(1, "abc");
            Assert.Equal(new byte[] { (byte)'a', (byte)'b', (byte)'c', 0 }, probe.Data);
            Assert.Equal(4, probe.CbData);                              // 原 :2529 `Length + 1`
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    [Fact]
    public void SendGameCenterMsg_空串()
    {
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareRuntime.SendGameCenterMsg(0, "");
            Assert.Equal(new byte[] { 0 }, probe.Data);
            Assert.Equal(1, probe.CbData);
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    [Fact]
    public void SendGameCenterMsg_null按空串处理()
    {
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareRuntime.SendGameCenterMsg(0, null);
            Assert.Equal(new byte[] { 0 }, probe.Data);
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    [Fact]
    public void SendGameCenterMsg_cbData是字符数而负载是字节数_口径不一致()
    {
        // ★ 登记：原文 `cbData := Length(sSendMsg) + 1` 是**字符数**，而 `StrCopy` 拷进缓冲的是
        //   AnsiString 的**字节**（GBK 下中文 2 字节/字）→ cbData 会小于实际字节数。
        //   接收方 `MyMessage` 用 `StrPas(lpData)` 读到 NUL 为止，故功能上无碍；但协议字段口径不一致。
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareRuntime.SendGameCenterMsg(2, "中");
            Assert.Equal(2, probe.CbData);                              // 1 个字符 + 1
            Assert.Equal(3, probe.Data.Length);                         // 2 字节 GBK + 1 个 NUL
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    [Fact]
    public void SendGameCenterMsg_句柄为0仍会调用发送接缝()
    {
        var probe = new CopyDataProbe();
        var original = GateShareRuntime.SendCopyData;
        try
        {
            GateShareRuntime.SendCopyData = probe.Capture;
            GateShareGlobals.ResetForTest();                            // g_dwGameCenterHandle = IntPtr.Zero
            GateShareRuntime.SendGameCenterMsg(1, "x");
            Assert.Equal(IntPtr.Zero, probe.Hwnd);                      // 原文不做句柄校验
        }
        finally { GateShareRuntime.SendCopyData = original; }
    }

    // ================= 常量登记 =================

    [Fact]
    public void 常量_tRunGate与更新时间()
    {
        Assert.Equal(8, GateShareRuntime.tRunGate);                     // 原 :14
        Assert.Equal("2023-07-01", GateShareRuntime.g_sUpdateTime);     // 原 :15
    }
}
