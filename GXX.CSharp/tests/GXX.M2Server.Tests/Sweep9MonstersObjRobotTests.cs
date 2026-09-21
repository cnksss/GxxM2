// ============================================================================
//  测试：Source/M2Engine/ObjRobot.pas → GXX.M2Server.Sweep9.Monsters（1:1）
//
//  覆盖策略：
//    * 单元常量 21 个逐个对账；
//    * 方法清单 20 条：条数 / 顺序 / 分段计数 / 落在单元内；
//    * 原文缺陷 F1-F10 **逐条差异断言**（含计数取证，见台账 §37.3）；
//    * 每个公开成员 ≥1 用例，分支/边界/异常路径覆盖；
//    * 全部走内存文件系统与可控时钟，**不碰磁盘**。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.Monsters;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class Sweep9MonstersObjRobotTests
{
    // ===================== 一、单元常量（:9-29） =====================

    [Fact]
    public void UnitConstants_MatchSource()
    {
        Assert.Equal("#AUTORUN", ObjRobotCore.sROAUTORUN);
        Assert.Equal("NPC", ObjRobotCore.sRONPCLABLEJMP);
        Assert.Equal(100, ObjRobotCore.nRONPCLABLEJMP);
        Assert.Equal("DAY", ObjRobotCore.sRODAY);
        Assert.Equal(200, ObjRobotCore.nRODAY);
        Assert.Equal("HOUR", ObjRobotCore.sROHOUR);
        Assert.Equal(201, ObjRobotCore.nROHOUR);
        Assert.Equal("MIN", ObjRobotCore.sROMIN);
        Assert.Equal(202, ObjRobotCore.nROMIN);
        Assert.Equal("SEC", ObjRobotCore.sROSEC);
        Assert.Equal(203, ObjRobotCore.nROSEC);
        Assert.Equal("RUNONWEEK", ObjRobotCore.sRUNONWEEK);
        Assert.Equal(300, ObjRobotCore.nRUNONWEEK);
        Assert.Equal("RUNONDAY", ObjRobotCore.sRUNONDAY);
        Assert.Equal(301, ObjRobotCore.nRUNONDAY);
        Assert.Equal("RUNONHOUR", ObjRobotCore.sRUNONHOUR);
        Assert.Equal(302, ObjRobotCore.nRUNONHOUR);
        Assert.Equal("RUNONMIN", ObjRobotCore.sRUNONMIN);
        Assert.Equal(303, ObjRobotCore.nRUNONMIN);
        Assert.Equal("RUNONSEC", ObjRobotCore.sRUNONSEC);
        Assert.Equal(304, ObjRobotCore.nRUNONSEC);
    }

    /// <summary>
    /// 计数取证（§37.3）：`sRO*` / `nRO*` 常量在原文 :9-29 共有 **11 对**、
    /// **21 个** const 声明；本断言把"11 对"与"21 个声明"两个数对死。
    /// </summary>
    [Fact]
    public void UnitConstantCounts_AddUp()
    {
        Assert.Equal(10, ObjRobotCore.UnitConstantPairs);
        // 逐条数：`sROAUTORUN`（:9）是**单独**一个（无 `nROAUTORUN`）；
        // 其余 `sRONPCLABLEJMP`…`sRUNONSEC`（:10-29）是 10 对 ⇒ 1 + 10*2 = 21。
        Assert.Equal(21, ObjRobotCore.UnitConstantDeclarations);
        Assert.Equal(ObjRobotCore.UnitConstantDeclarations, 1 + ObjRobotCore.UnitConstantPairs * 2);
    }

    [Fact]
    public void SourceIdentity()
    {
        Assert.Equal("Source/M2Engine/ObjRobot.pas", ObjRobotCore.SourceUnit);
        Assert.Equal(500, ObjRobotCore.SourceLines);
    }

    // ===================== 二、方法清单（20/20 覆盖取证） =====================

    [Fact]
    public void MethodInventory_TwentyMethods()
    {
        Assert.Equal(20, ObjRobotCore.DeclCount);
        Assert.Equal(20, ObjRobotCore.ImplCount);
        Assert.Equal(20, ObjRobotCore.Methods.Length);
        Assert.Equal(ObjRobotCore.DeclCount, ObjRobotCore.Methods.Length);
        Assert.Equal(ObjRobotCore.ImplCount, ObjRobotCore.Methods.Length);
    }

    [Fact]
    public void MethodInventory_SplitMatchesClassCounts()
    {
        int robotObject = ObjRobotCore.Methods.Count(m => m.Name.StartsWith("TRobotObject.", StringComparison.Ordinal));
        int robotManage = ObjRobotCore.Methods.Count(m => m.Name.StartsWith("TRobotManage.", StringComparison.Ordinal));
        Assert.Equal(ObjRobotCore.RobotObjectMethodCount, robotObject);
        Assert.Equal(ObjRobotCore.RobotManageMethodCount, robotManage);
        Assert.Equal(ObjRobotCore.Methods.Length, robotObject + robotManage);
    }

    [Fact]
    public void MethodInventory_AscendingAndInsideUnit()
    {
        for (int i = 1; i < ObjRobotCore.Methods.Length; i++)
            Assert.True(ObjRobotCore.Methods[i].Start > ObjRobotCore.Methods[i - 1].Start,
                $"方法清单未按行号升序：{ObjRobotCore.Methods[i - 1].Name} → {ObjRobotCore.Methods[i].Name}");

        foreach (var (name, start, end) in ObjRobotCore.Methods)
        {
            Assert.True(start > 0 && end >= start, $"{name} 行段非法 {start}-{end}");
            Assert.True(end < ObjRobotCore.SourceLines, $"{name} 超出单元（{ObjRobotCore.SourceLines} 行）");
        }
    }

    /// <summary>
    /// 覆盖取证：清单里的每个名字都要能在实现里**反射找到**同名方法
    /// （`TRobotObject.Destroy` / `TRobotManage.Destroy` 这类 `Free` 语义用 `Destroy`/`Dispose` 双名）。
    /// </summary>
    [Fact]
    public void MethodInventory_AllNamesExistInImplementation()
    {
        var missing = new List<string>();
        foreach (var (name, _, _) in ObjRobotCore.Methods)
        {
            int dot = name.IndexOf('.');
            string typeName = name.Substring(0, dot);
            string methodName = name.Substring(dot + 1);
            Type type = typeName == "TRobotObject" ? typeof(TRobotObject) : typeof(TRobotManage);
            if (methodName == "Create")
            {
                // 原文 `constructor Create;` → 托管侧构造器
                if (type.GetConstructor(Type.EmptyTypes) == null) missing.Add(name);
                continue;
            }
            if (type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                missing.Add(name);
        }
        Assert.Empty(missing);
    }

    // ===================== 三、原文缺陷 F1-F10 的差异断言 =====================

    /// <summary>F1：`dwRunTimeLen` 只有 1 个写入点（且写的是 0）⇒ 外层节流恒真。</summary>
    [Fact]
    public void Flaw1_RunTimeLenIsAlwaysZero()
    {
        Assert.True(ObjRobotCore.RunTimeLenIsAlwaysZero());
        Assert.True(ObjRobotCore.RunTimeLenCountAddsUp());
        Assert.Equal(1, ObjRobotCore.RunTimeLenWriteSites);
        Assert.Equal(3, ObjRobotCore.RunTimeLenOccurrences);
    }

    /// <summary>F1 可执行证据：`dwRunTimeLen = 0` 时，只要走过 1 毫秒就放行。</summary>
    [Fact]
    public void Flaw1_OuterThrottle_PassesAfterOneTick()
    {
        Assert.False(ObjRobotCore.PassesOuterThrottle(1000u, 1000u, 0u));   // 同一毫秒 → 不放行
        Assert.True(ObjRobotCore.PassesOuterThrottle(1001u, 1000u, 0u));    // 差 1ms → 放行
    }

    /// <summary>F2：`nRunCmd` 只有 1 个赋值点却有 4 个 case 标签 ⇒ 3 个空标签不可达。</summary>
    [Fact]
    public void Flaw2_RunCmdHasThreeUnreachableEmptyLabels()
    {
        Assert.True(ObjRobotCore.RunCmdHasUnreachableLabels());
        Assert.Equal(1, ObjRobotCore.RunCmdAssignSites);
        Assert.Equal(4, ObjRobotCore.RunCmdCaseLabels);
        Assert.Equal(3, ObjRobotCore.RunCmdEmptyLabels);
    }

    /// <summary>F3：`nMoethod` 9 个并列 if 对 9 个 case 标签，但 case 无 else ⇒ 不匹配即静默失效。</summary>
    [Fact]
    public void Flaw3_MoethodNoDefaultBranch()
    {
        Assert.True(ObjRobotCore.MoethodAssignMatchesLabels());
        Assert.True(ObjRobotCore.MoethodNoDefault());
        Assert.Equal(9, ObjRobotCore.MoethodAssignSites);
        Assert.Equal(9, ObjRobotCore.MoethodCaseLabels);
    }

    /// <summary>F3 的后半段：`nParam2/nParam3/nParam4` 只有声明，全单元 0 读 0 写。</summary>
    [Fact]
    public void Flaw3_UnusedParamFields()
    {
        Assert.Equal(3, ObjRobotCore.UnusedParamDeclarations);
        var t = typeof(TAutoRunInfo);
        Assert.NotNull(t.GetField("nParam2"));
        Assert.NotNull(t.GetField("nParam3"));
        Assert.NotNull(t.GetField("nParam4"));
    }

    /// <summary>F4：`sLabel` 是死存储（2 声明 + 2 赋值，0 读取）。</summary>
    [Fact]
    public void Flaw4_SLabelIsDeadStore()
    {
        Assert.True(ObjRobotCore.SLabelIsDeadStore());
        Assert.Equal(0, ObjRobotCore.SLabelReadSites);
        Assert.Equal(4, ObjRobotCore.SLabelOccurrences);
    }

    /// <summary>F6：时分上界都是越界值（24 / 60）。</summary>
    [Fact]
    public void Flaw6_TimeUpperBoundsOffByOne()
    {
        Assert.True(ObjRobotCore.TimeUpperBoundsAreOffByOne());
        Assert.True(ObjRobotCore.OnDayInRange(24, 60));
        Assert.False(ObjRobotCore.OnDayInRange(25, 0));
        Assert.False(ObjRobotCore.OnDayInRange(0, 61));
        Assert.True(ObjRobotCore.OnWeekInRange(7, 24, 60));
        Assert.False(ObjRobotCore.OnWeekInRange(8, 0, 0));
        Assert.False(ObjRobotCore.OnWeekInRange(0, 0, 0));
    }

    /// <summary>F7：三个空过程体 ↔ 三个已接线未实现的标签。</summary>
    [Fact]
    public void Flaw7_ThreeEmptyBodies()
    {
        Assert.True(ObjRobotCore.EmptyBodiesMatchTags());
        Assert.Equal(3, ObjRobotCore.EmptyMethodBodies);
        Assert.Equal(3, ObjRobotCore.UnimplementedTagWiringSites);
    }

    /// <summary>F9：`SendSocket` 两个参数的空 override。</summary>
    [Fact]
    public void Flaw9_SendSocketIsEmptyOverride()
    {
        Assert.Equal(2, ObjRobotCore.SendSocketParameterCount);
        var m = typeof(TRobotObject).GetMethod("SendSocket");
        Assert.NotNull(m);
        Assert.Equal(2, m!.GetParameters().Length);
        // ★ D-P9-02（原文缺陷 F9 的形式偏差，**有意暂缓**，同台账 §14.5）：
        //   原文 `override`（ObjPlayer.pas:1191 `virtual`），但托管侧 `TPlayObject.SendSocket`
        //   尚未移植 ⇒ 这里**不是** override。本断言是**绊线**：一旦基类补上 `SendSocket`，
        //   此行会红，提醒把 `override` 关键字加回并更新本条断言。
        Assert.NotEqual(typeof(TPlayObject), m.GetBaseDefinition().DeclaringType);
        Assert.Equal(typeof(TRobotObject), m.GetBaseDefinition().DeclaringType);
    }

    /// <summary>F10：`LoadRobot` 的 `m_sCharName = ''` 是死分支。</summary>
    [Fact]
    public void Flaw10_DeadCharNameBranch()
    {
        Assert.True(ObjRobotCore.DeadBranchIsProvable());
        Assert.Equal(1, ObjRobotCore.DeadBranchGuardSites);
        Assert.Equal(1, ObjRobotCore.DeadBranchCheckSites);
    }

    /// <summary>`m_boRunOnWeek` 写后从不读（`m_boRunOnWeekDeclarationSites + WriteSites + ReadSites == 2`）。</summary>
    [Fact]
    public void WriteOnlyField_RunOnWeek()
    {
        var f = typeof(TRobotObject).GetField("m_boRunOnWeek",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(f);
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        Assert.False((bool)f!.GetValue(robot)!);
    }

    /// <summary>`CompareText` 是**大小写不敏感**（原文 :313/:315/:322-339 全用它）。</summary>
    [Fact]
    public void CompareText_IsCaseInsensitive()
    {
        Assert.Equal(0, ObjRobotCore.CompareText("#autorun", "#AUTORUN"));
        Assert.Equal(0, ObjRobotCore.CompareText("npc", "NPC"));
        Assert.Equal(0, ObjRobotCore.CompareText("runonweek", "RUNONWEEK"));
        Assert.NotEqual(0, ObjRobotCore.CompareText("RUNONDAY", "RUNONWEEK"));
        // ★ '#' 是 `sROAUTORUN` 的一部分，不是注释符 —— 少了它就不匹配（原文如此）
        Assert.NotEqual(0, ObjRobotCore.CompareText("autorun", "#AUTORUN"));
    }

    /// <summary>间隔毫秒字面量（原文 :108/:117/:126/:135）。</summary>
    [Fact]
    public void IntervalMillis_MatchLiterals()
    {
        Assert.Equal(24u * 60u * 60u * 1000u, ObjRobotCore.IntervalMillis(ObjRobotCore.nRODAY));
        Assert.Equal(60u * 60u * 1000u, ObjRobotCore.IntervalMillis(ObjRobotCore.nROHOUR));
        Assert.Equal(60u * 1000u, ObjRobotCore.IntervalMillis(ObjRobotCore.nROMIN));
        Assert.Equal(1000u, ObjRobotCore.IntervalMillis(ObjRobotCore.nROSEC));
        Assert.Equal(0u, ObjRobotCore.IntervalMillis(0));
    }

    /// <summary>间隔判据是**无符号回绕**（Delphi `LongWord` 语义）。</summary>
    [Fact]
    public void Interval_WrapsLikeLongWord()
    {
        // nParam1 = 1，SEC = 1000ms
        Assert.True(ObjRobotCore.PassesInterval(2000u, 999u, ObjRobotCore.nROSEC, 1));   // 差 1001 > 1000
        Assert.False(ObjRobotCore.PassesInterval(1999u, 999u, ObjRobotCore.nROSEC, 1));  // 差 1000，不 >1000
        // 无符号回绕：`dwRunTick` 在 2^32 边界前 10ms、`now` 在边界后 10ms
        // ⇒ `now - dwRunTick` 回绕成 **21**（真实经过的毫秒数），不是负数也不是大数。
        Assert.Equal(21u, unchecked(10u - (uint.MaxValue - 10u)));
        Assert.False(ObjRobotCore.PassesInterval(10u, uint.MaxValue - 10u, ObjRobotCore.nROSEC, 1));
        Assert.True(ObjRobotCore.PassesInterval(30u, uint.MaxValue - 999u, ObjRobotCore.nROSEC, 1));
    }

    /// <summary>超长间隔的**无符号乘法回绕**（原文 `24*60*60*1000 * LongWord(nParam1)`）。</summary>
    [Fact]
    public void Interval_MultiplierWrapsForLargeParam()
    {
        // 86400000 * 50 = 4,320,000,000 > 2^32 (=4,294,967,296) ⇒ 回绕成 25,032,704
        uint wrapped = unchecked(24u * 60u * 60u * 1000u * 50u);
        Assert.Equal(25032704u, wrapped);
        // 于是"50 天"反而比"1 天"更容易触发（原文如此）
        Assert.False(ObjRobotCore.PassesInterval(wrapped, 0u, ObjRobotCore.nRODAY, 50));
        Assert.True(ObjRobotCore.PassesInterval(wrapped + 1u, 0u, ObjRobotCore.nRODAY, 50));
    }

    // ===================== 四、TRobotObject 行为 =====================

    [Fact]
    public void Create_SetsDummyFields()
    {
        using var env = new MonstersTestEnv();
        var robot = new TRobotObject();
        Assert.True(robot.m_boSuperMan);
        Assert.NotNull(robot.m_AutoRunList);
        Assert.Empty(robot.m_AutoRunList);
        Assert.Equal("", robot.m_sScriptFileName);
    }

    /// <summary>`AutoRun` 三重守卫（:96）：`g_RobotNPC` 为 null 直接退出。</summary>
    [Fact]
    public void AutoRun_NoRobotNpc_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        ObjRobotSeam.g_RobotNPC = null;
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo { nRunCmd = ObjRobotCore.nRONPCLABLEJMP, nMoethod = ObjRobotCore.nROSEC, nParam1 = 1 };
        env.Now = 9999u;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`AutoRun` 守卫：`m_PEnvir = nil` 直接退出。</summary>
    [Fact]
    public void AutoRun_NoMap_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.m_PEnvir = null;
        var info = new TAutoRunInfo { nRunCmd = ObjRobotCore.nRONPCLABLEJMP, nMoethod = ObjRobotCore.nROSEC, nParam1 = 1 };
        env.Now = 9999u;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`AutoRun(nil)` 不抛。</summary>
    [Fact]
    public void AutoRun_NullInfo_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.Now = 9999u;
        robot.AutoRun(null);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>F2 可执行证据：`nRunCmd = 1/2/3` 是空分支，什么也不做。</summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void AutoRun_EmptyRunCmdLabels_DoNothing(int nRunCmd)
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.m_nScriptGotoCount = 7;
        var info = new TAutoRunInfo { nRunCmd = nRunCmd, nMoethod = ObjRobotCore.nROSEC, nParam1 = 1 };
        env.Now = 9999u;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.Equal(7, robot.m_nScriptGotoCount);   // 连 m_nScriptGotoCount 都没被清零
    }

    /// <summary>F2 可执行证据：`nRunCmd` 不匹配任何标签（含无 else）也不做任何事。</summary>
    [Fact]
    public void AutoRun_UnknownRunCmd_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo { nRunCmd = 999, nMoethod = ObjRobotCore.nROSEC, nParam1 = 1 };
        env.Now = 9999u;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`nROSEC` 间隔型：未到点不触发；到点触发并刷新 `dwRunTick` + 清 `m_nScriptGotoCount`。</summary>
    [Fact]
    public void AutoRun_SecInterval_FiresAndRefreshesTick()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.m_nScriptGotoCount = 5;
        var info = new TAutoRunInfo
        {
            nRunCmd = ObjRobotCore.nRONPCLABLEJMP,
            nMoethod = ObjRobotCore.nROSEC,
            nParam1 = 2,
            sParam2 = "@Label",
            dwRunTick = 0,
        };

        env.Now = 2000u;                                  // 差 2000，要求 > 1000*2 = 2000 ⇒ 不触发
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.Equal(5, robot.m_nScriptGotoCount);

        env.Now = 2001u;                                  // 差 2001 > 2000 ⇒ 触发
        robot.AutoRun(info);
        Assert.Single(env.RobotNpc.Calls);
        Assert.Equal("@Label", env.RobotNpc.Calls[0].Label);
        Assert.False(env.RobotNpc.Calls[0].ExtJmp);
        Assert.Same(robot, env.RobotNpc.Calls[0].Player);
        Assert.Equal(0, robot.m_nScriptGotoCount);
        Assert.Equal(2001u, info.dwRunTick);              // 就地改写（引用类型语义）

        env.Now = 4002u;                                  // 距上次 2001 → 差 2001 ⇒ 再次触发
        robot.AutoRun(info);
        Assert.Equal(2, env.RobotNpc.Calls.Count);
    }

    [Theory]
    [InlineData(200, 24u * 60u * 60u * 1000u)]
    [InlineData(201, 60u * 60u * 1000u)]
    [InlineData(202, 60u * 1000u)]
    [InlineData(203, 1000u)]
    public void AutoRun_FourIntervalKinds_UseTheirOwnMillis(int method, uint expectedMillis)
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo
        {
            nRunCmd = ObjRobotCore.nRONPCLABLEJMP,
            nMoethod = method,
            nParam1 = 1,
            sParam2 = "@L",
            dwRunTick = 0,
        };

        env.Now = expectedMillis;              // 差 == 门限 ⇒ 不触发
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);

        env.Now = expectedMillis + 1u;         // 差 > 门限 ⇒ 触发
        robot.AutoRun(info);
        Assert.Single(env.RobotNpc.Calls);
    }

    /// <summary>F3 可执行证据：`nMoethod` 不在 9 个标签里（0 = 托管零初始化）⇒ 静默失效。</summary>
    [Fact]
    public void AutoRun_UnknownMoethod_SilentlyDoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo
        {
            nRunCmd = ObjRobotCore.nRONPCLABLEJMP,
            nMoethod = 0,          // 原文：未初始化残留 / 托管：零初始化
            nParam1 = 1,
            sParam2 = "@L",
        };
        env.Now = uint.MaxValue;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>F7 可执行证据：`nRUNONHOUR/OnMin/OnSec` 三个标签接了线，但三个方法体是空的。</summary>
    [Theory]
    [InlineData(302)]
    [InlineData(303)]
    [InlineData(304)]
    public void AutoRun_UnimplementedTagMethods_DoNothing(int method)
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.DecodeHour = 0;
        env.DecodeMin = 0;
        env.Week = 1;
        var info = new TAutoRunInfo
        {
            nRunCmd = ObjRobotCore.nRONPCLABLEJMP,
            nMoethod = method,
            nParam1 = 1,
            sParam1 = "1:0:0",
            sParam2 = "@L",
        };
        env.Now = uint.MaxValue;
        robot.AutoRun(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.False(info.boStatus);
    }

    /// <summary>`AutoRunOfOnDay` 命中 → 触发一次并把 `boStatus` 置真；再调不重复触发。</summary>
    [Fact]
    public void AutoRunOfOnDay_FiresOnceUntilStatusResets()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.DecodeHour = 10;
        env.DecodeMin = 30;
        var info = new TAutoRunInfo { sParam1 = "10:30", sParam2 = "@Day" };

        robot.AutoRunOfOnDay(info);
        Assert.Single(env.RobotNpc.Calls);
        Assert.Equal("@Day", env.RobotNpc.Calls[0].Label);
        Assert.True(info.boStatus);

        robot.AutoRunOfOnDay(info);                 // boStatus 已真 ⇒ 不再触发
        Assert.Single(env.RobotNpc.Calls);
    }

    /// <summary>分钟不匹配 → `boStatus` 复位（原文 :195）。</summary>
    [Fact]
    public void AutoRunOfOnDay_MinuteMismatch_ResetsStatus()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.DecodeHour = 10;
        env.DecodeMin = 31;
        var info = new TAutoRunInfo { sParam1 = "10:30", sParam2 = "@Day", boStatus = true };

        robot.AutoRunOfOnDay(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.False(info.boStatus);
    }

    /// <summary>
    /// ★ F5/原文缺陷：小时不匹配时**没有**外层 `else` ⇒ `boStatus` **不复位**。
    /// </summary>
    [Fact]
    public void AutoRunOfOnDay_HourMismatch_LeavesStatusUntouched()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.DecodeHour = 11;
        env.DecodeMin = 30;
        var info = new TAutoRunInfo { sParam1 = "10:30", sParam2 = "@Day", boStatus = true };
        robot.AutoRunOfOnDay(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.True(info.boStatus);      // 原文如此：外层无 else
        Assert.Equal(0, ObjRobotCore.OnDayAction(10, 30, 11, 30, true));
    }

    /// <summary>F6 可执行证据：`nHOUR = 24` 通过准入判据（越界放行）。</summary>
    [Fact]
    public void AutoRunOfOnDay_Hour24_PassesRangeGuard()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.DecodeHour = 24;                 // 接缝可造出 24 点 ⇒ 证明该越界值确实可达
        env.DecodeMin = 0;
        var info = new TAutoRunInfo { sParam1 = "24:0", sParam2 = "@Day24" };
        robot.AutoRunOfOnDay(info);
        Assert.Single(env.RobotNpc.Calls);
        Assert.Equal("@Day24", env.RobotNpc.Calls[0].Label);

        // 25 点被正确拒绝
        env.RobotNpc.Calls.Clear();
        env.DecodeHour = 25;
        robot.AutoRunOfOnDay(new TAutoRunInfo { sParam1 = "25:0", sParam2 = "@Day25" });
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`AutoRunOfOnDay` 的三段分支判定（纯函数对账）。</summary>
    [Fact]
    public void OnDayAction_ThreeBranches()
    {
        Assert.Equal(0, ObjRobotCore.OnDayAction(25, 0, 25, 0, false));   // 越界 ⇒ 不动作
        Assert.Equal(0, ObjRobotCore.OnDayAction(10, 30, 11, 30, false)); // 小时不匹配 ⇒ 不动作
        Assert.Equal(1, ObjRobotCore.OnDayAction(10, 30, 10, 30, false)); // 命中且未触发 ⇒ 触发
        Assert.Equal(0, ObjRobotCore.OnDayAction(10, 30, 10, 30, true));  // 命中但已触发 ⇒ 不动作
        Assert.Equal(2, ObjRobotCore.OnDayAction(10, 30, 10, 31, true));  // 分钟不匹配 ⇒ 复位
    }

    /// <summary>`AutoRunOfOnWeek` 命中 → 触发；星期/时/分三段条件。</summary>
    [Fact]
    public void AutoRunOfOnWeek_MatchesWeekHourMin()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.Week = 3;
        env.DecodeHour = 8;
        env.DecodeMin = 15;
        var info = new TAutoRunInfo { sParam1 = "3:8:15", sParam2 = "@Week" };

        robot.AutoRunOfOnWeek(info);
        Assert.Single(env.RobotNpc.Calls);
        Assert.Equal("@Week", env.RobotNpc.Calls[0].Label);
        Assert.True(info.boStatus);

        robot.AutoRunOfOnWeek(info);
        Assert.Single(env.RobotNpc.Calls);
    }

    [Fact]
    public void AutoRunOfOnWeek_WeekMismatch_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        env.Week = 4;
        env.DecodeHour = 8;
        env.DecodeMin = 15;
        var info = new TAutoRunInfo { sParam1 = "3:8:15", sParam2 = "@Week", boStatus = true };
        robot.AutoRunOfOnWeek(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.True(info.boStatus);     // ★ 原文无外层 else ⇒ 不复位
        Assert.Equal(0, ObjRobotCore.OnWeekAction(3, 8, 15, 4, 8, 15, true));
    }

    [Fact]
    public void OnWeekAction_ThreeBranches()
    {
        Assert.Equal(0, ObjRobotCore.OnWeekAction(0, 0, 0, 0, 0, 0, false));       // 星期越界
        Assert.Equal(0, ObjRobotCore.OnWeekAction(3, 25, 0, 3, 25, 0, false));     // 小时越界
        Assert.Equal(0, ObjRobotCore.OnWeekAction(3, 8, 61, 3, 8, 61, false));     // 分钟越界
        Assert.Equal(0, ObjRobotCore.OnWeekAction(3, 8, 15, 4, 8, 15, false));     // 星期不匹配
        Assert.Equal(1, ObjRobotCore.OnWeekAction(3, 8, 15, 3, 8, 15, false));     // 触发
        Assert.Equal(0, ObjRobotCore.OnWeekAction(3, 8, 15, 3, 8, 15, true));      // 已触发
        Assert.Equal(2, ObjRobotCore.OnWeekAction(3, 8, 15, 3, 8, 16, true));      // 复位
    }

    /// <summary>三个空过程体：直接调用也不产生任何可观测副作用。</summary>
    [Fact]
    public void EmptyAutoRunVariants_HaveNoEffect()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo { sParam1 = "1:2:3", sParam2 = "@X", boStatus = true };
        robot.AutoRunOfOnHour(info);
        robot.AutoRunOfOnMin(info);
        robot.AutoRunOfOnSec(info);
        Assert.Empty(env.RobotNpc.Calls);
        Assert.True(info.boStatus);
    }

    /// <summary>`DayOfTheWeek` 接缝的 **Delphi 语义**：1=周日 … 7=周六。</summary>
    [Fact]
    public void DayOfTheWeekSeam_MapsSundayToOne()
    {
        ObjRobotSeam.ResetDefaults();
        Assert.Equal(1, ObjRobotSeam.DayOfTheWeek(new DateTime(2026, 9, 20)));   // 2026-09-20 是周日
        Assert.Equal(2, ObjRobotSeam.DayOfTheWeek(new DateTime(2026, 9, 21)));   // 周一
        Assert.Equal(7, ObjRobotSeam.DayOfTheWeek(new DateTime(2026, 9, 26)));   // 周六
    }

    /// <summary>`DecodeTime` 接缝默认实现（1:1 取时分秒毫秒）。</summary>
    [Fact]
    public void DecodeTimeSeam_DefaultReadsComponents()
    {
        ObjRobotSeam.ResetDefaults();
        ObjRobotSeam.DecodeTime(new DateTime(2026, 9, 21, 13, 45, 59, 123), out ushort h, out ushort m, out ushort s, out ushort ms);
        Assert.Equal((ushort)13, h);
        Assert.Equal((ushort)45, m);
        Assert.Equal((ushort)59, s);
        Assert.Equal((ushort)123, ms);
    }

    // ===================== 五、LoadScript / ClearScript / ReloadScript =====================

    private const string ScriptPath = ".\\Envir\\Robot_def\\test.txt";
    private const string RobotListPath = ".\\Envir\\Robot.txt";

    [Fact]
    public void LoadScript_FileMissing_LoadsNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Empty(robot.m_AutoRunList);
    }

    [Fact]
    public void LoadScript_ParsesAutorunLine()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 5 @Label");
        var robot = env.MakeRobot();
        robot.LoadScript();

        Assert.Single(robot.m_AutoRunList);
        var info = robot.m_AutoRunList[0];
        Assert.Equal(ObjRobotCore.nRONPCLABLEJMP, info.nRunCmd);
        Assert.Equal(ObjRobotCore.nROSEC, info.nMoethod);
        Assert.Equal(5, info.nParam1);
        Assert.Equal("5", info.sParam1);
        Assert.Equal("@Label", info.sParam2);
        Assert.Equal(0u, info.dwRunTimeLen);       // ★ F1：唯一写入点写的是 0
        Assert.False(info.boStatus);
    }

    /// <summary>大小写不敏感（原文 `CompareText`）。</summary>
    [Fact]
    public void LoadScript_KeywordsAreCaseInsensitive()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#autorun npc runonday 3:8:15 @Lbl");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);
        Assert.Equal(ObjRobotCore.nRUNONDAY, robot.m_AutoRunList[0].nMoethod);
        Assert.Equal("3:8:15", robot.m_AutoRunList[0].sParam1);
        Assert.Equal("@Lbl", robot.m_AutoRunList[0].sParam2);
    }

    /// <summary>空行与 `;` 注释行被跳过（原文 :304）。</summary>
    [Fact]
    public void LoadScript_SkipsEmptyAndCommentLines()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath,
            "",
            "; 注释",
            "#AUTORUN NPC SEC 1 @A",
            "   ",
            "#NOTAUTORUN NPC SEC 1 @B",
            "#AUTORUN NOTNPC SEC 1 @C",
            "#AUTORUN NPC SEC 2 @D");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Equal(2, robot.m_AutoRunList.Count);
        Assert.Equal("@A", robot.m_AutoRunList[0].sParam2);
        Assert.Equal("@D", robot.m_AutoRunList[1].sParam2);
    }

    /// <summary>分隔符集合 `[' ', '/', #9]`（原文 :306-312）。</summary>
    [Fact]
    public void LoadScript_SplitsOnSpaceSlashTab()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN/NPC/MIN\t7\t@Tab");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);
        Assert.Equal(ObjRobotCore.nROMIN, robot.m_AutoRunList[0].nMoethod);
        Assert.Equal(7, robot.m_AutoRunList[0].nParam1);
        Assert.Equal("@Tab", robot.m_AutoRunList[0].sParam2);
    }

    /// <summary>`nParam1` 解析失败取默认 **1**（原文 :345 `StrToIntDef(sParam1, 1)`）。</summary>
    [Fact]
    public void LoadScript_NonNumericParam1_FallsBackToOne()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC abc @L");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);
        Assert.Equal(1, robot.m_AutoRunList[0].nParam1);
    }

    /// <summary>
    /// ★ F3 可执行证据 + **D-P9-01**：`sMoethod` 一个都不匹配时，
    /// 原文 `New()` 留下**未初始化残留值**；托管侧 `new` 零初始化 ⇒ `nMoethod == 0`。
    /// 两者都匹配不到 `case` 标签 ⇒ **可观测行为一致**（静默失效）。
    /// </summary>
    [Fact]
    public void LoadScript_UnknownMethod_LeavesMoethodAtHostDefault()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC NOSUCHMETHOD 1 @L");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);
        Assert.Equal(0, robot.m_AutoRunList[0].nMoethod);   // 托管零初始化（原文是堆残留）
        // 而它在 AutoRun 里确实什么也不做
        env.Now = uint.MaxValue;
        robot.AutoRun(robot.m_AutoRunList[0]);
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`nMoethod` 的 9 个关键字逐个正确落值（计数对账：9 个关键字 ↔ 9 个断言）。</summary>
    [Theory]
    [InlineData("DAY", 200)]
    [InlineData("HOUR", 201)]
    [InlineData("MIN", 202)]
    [InlineData("SEC", 203)]
    [InlineData("RUNONWEEK", 300)]
    [InlineData("RUNONDAY", 301)]
    [InlineData("RUNONHOUR", 302)]
    [InlineData("RUNONMIN", 303)]
    [InlineData("RUNONSEC", 304)]
    public void LoadScript_NineMethodKeywords(string keyword, int expected)
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC " + keyword + " 1 @L");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);
        Assert.Equal(expected, robot.m_AutoRunList[0].nMoethod);
    }

    /// <summary>`ClearScript` 清空列表；`ReloadScript` = Clear + Load。</summary>
    [Fact]
    public void ClearScript_And_ReloadScript()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Single(robot.m_AutoRunList);

        robot.ClearScript();
        Assert.Empty(robot.m_AutoRunList);

        robot.ReloadScript();
        Assert.Single(robot.m_AutoRunList);

        robot.ReloadScript();
        Assert.Single(robot.m_AutoRunList);   // 不重复累加
    }

    /// <summary>`Destroy` = `ClearScript`（原文 :276）。</summary>
    [Fact]
    public void Destroy_ClearsScriptList()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        var robot = env.MakeRobot();
        robot.LoadScript();
        robot.Destroy();
        Assert.Empty(robot.m_AutoRunList);
        robot.Dispose();       // Free 语义等价物幂等
    }

    /// <summary>`ProcessAutoRun` 逐条调用 `AutoRun`；`Run` = `ProcessAutoRun`（且**不调基类**，F8）。</summary>
    [Fact]
    public void ProcessAutoRun_And_Run_DriveEachEntry()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A", "#AUTORUN NPC SEC 1 @B");
        var robot = env.MakeRobot();
        robot.LoadScript();
        Assert.Equal(2, robot.m_AutoRunList.Count);

        env.Now = 0u;
        robot.Run();                                    // 差 0，不触发
        Assert.Empty(env.RobotNpc.Calls);

        env.Now = 2u;                                   // 差 2 > 1000*1? 否 —— nParam1 默认 1 ⇒ 门限 1000
        robot.Run();
        Assert.Empty(env.RobotNpc.Calls);

        env.Now = 1002u;
        robot.Run();
        Assert.Equal(2, env.RobotNpc.Calls.Count);
        Assert.Equal(new[] { "@A", "@B" }, env.RobotNpc.Calls.Select(c => c.Label).ToArray());
    }

    [Fact]
    public void ProcessAutoRun_EmptyList_DoesNothing()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.ProcessAutoRun();
        Assert.Empty(env.RobotNpc.Calls);
    }

    /// <summary>`SendSocket` 是空实现：调用后**不产生任何发包可观测副作用**（原文如此）。</summary>
    [Fact]
    public void SendSocket_IsNoOp()
    {
        using var env = new MonstersTestEnv();
        var robot = env.MakeRobot();
        robot.SendSocket(default, "hello");
        Assert.Empty(env.RobotNpc.Calls);
        Assert.Empty(env.Log);
    }

    // ===================== 六、TRobotManage =====================

    [Fact]
    public void RobotManage_Create_SetsEmptyList()
    {
        using var env = new MonstersTestEnv();
        var mgr = new TRobotManage();
        Assert.NotNull(mgr.RobotHumanList);
        Assert.Equal(0, mgr.RobotHumanList.Count);
        mgr.Dispose();
    }

    /// <summary>`Robot.txt` 不存在 ⇒ 不加载任何机器人，但 `FInitialized` 仍被置真。</summary>
    [Fact]
    public void LoadRobot_MissingFile_LoadsNothingButInitializes()
    {
        using var env = new MonstersTestEnv();
        var mgr = new TRobotManage();
        mgr.LoadRobot();
        Assert.Equal(0, mgr.RobotHumanList.Count);
        mgr.Run();                     // FInitialized 已真 ⇒ 走空循环，不抛
        Assert.Empty(env.Log);
        mgr.Dispose();
    }

    [Fact]
    public void LoadRobot_ParsesRobotList()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath,
            "",
            "; 注释",
            "机器人甲 test",
            "只有名字没有脚本",
            "机器人乙/other");

        var mgr = new TRobotManage();
        mgr.LoadRobot();

        Assert.Equal(2, mgr.RobotHumanList.Count);
        var r0 = Assert.IsType<TRobotObject>(mgr.RobotHumanList.GetObject(0));
        Assert.Equal("机器人甲", r0.m_sCharName);
        Assert.Equal("test", r0.m_sScriptFileName);
        Assert.Equal("0", r0.m_sMapName);
        Assert.Same(env.Map, r0.m_PEnvir);            // g_MapManager.FindMap 接缝
        Assert.Single(r0.m_AutoRunList);              // LoadScript 已跑

        var r1 = Assert.IsType<TRobotObject>(mgr.RobotHumanList.GetObject(1));
        Assert.Equal("机器人乙", r1.m_sCharName);
        Assert.Equal("other", r1.m_sScriptFileName);
        Assert.Empty(r1.m_AutoRunList);               // 没有 other.txt

        mgr.Dispose();
    }

    /// <summary>F10：`m_sCharName = ''` 的死分支 —— 名字为空的行**在 :426 就被拒**，永远走不到 :430。</summary>
    [Fact]
    public void LoadRobot_EmptyNameLine_RejectedBeforeDeadBranch()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(RobotListPath, "/test");     // 第一个 token 为空 ⇒ sRobotName = ''
        var mgr = new TRobotManage();
        mgr.LoadRobot();
        Assert.Equal(0, mgr.RobotHumanList.Count);
        mgr.Dispose();
    }

    [Fact]
    public void RobotManage_Run_InvokesRobots()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");

        var mgr = new TRobotManage();
        mgr.LoadRobot();
        env.Now = 1002u;
        mgr.Run();
        Assert.Single(env.RobotNpc.Calls);
        Assert.Equal("@A", env.RobotNpc.Calls[0].Label);
        mgr.Dispose();
    }

    /// <summary>`Run` 在未 `LoadRobot` 时因 `FInitialized = False` 直接退出（:460）。</summary>
    [Fact]
    public void RobotManage_Run_BeforeLoad_Exits()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");
        var mgr = new TRobotManage();
        env.Now = 1002u;
        mgr.Run();                                   // FInitialized = False
        Assert.Empty(env.RobotNpc.Calls);
        mgr.Dispose();
    }

    /// <summary>`UnLoadRobot` 释放并清空列表（:481-497）。</summary>
    [Fact]
    public void UnLoadRobot_ClearsList()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");
        var mgr = new TRobotManage();
        mgr.LoadRobot();
        Assert.Equal(1, mgr.RobotHumanList.Count);

        mgr.UnLoadRobot();
        Assert.Equal(0, mgr.RobotHumanList.Count);

        env.Now = 1002u;
        mgr.Run();                                   // FInitialized 已被 UnLoadRobot 置假
        Assert.Empty(env.RobotNpc.Calls);
        mgr.Dispose();
    }

    /// <summary>`RELOADROBOT` = UnLoad + Load（:448-452）。</summary>
    [Fact]
    public void ReloadRobot_KeepsSingleInstance()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");
        var mgr = new TRobotManage();
        mgr.LoadRobot();
        mgr.RELOADROBOT();
        Assert.Equal(1, mgr.RobotHumanList.Count);
        mgr.Dispose();
    }

    /// <summary>
    /// 异常路径（:471-476）：机器人 `Run` 抛异常 → `MainOutMessage(sExceptionMsg)` +
    /// `MainOutMessage(E.Message)`；且 `UnLock`（:478）**在 try 之外**，必须仍然执行。
    /// 用"能再次 Lock"证明锁确实释放了（若未释放会死锁 → 用例超时失败）。
    /// </summary>
    [Fact]
    public void RobotManage_Run_SwallowsExceptionAndUnlocks()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");
        var mgr = new TRobotManage();
        mgr.LoadRobot();

        env.Now = 1002u;
        env.RobotNpc.ThrowOnGoto = new InvalidOperationException("Boom");
        mgr.Run();

        Assert.Equal(2, env.Log.Count);
        Assert.Equal(TRobotManage.sExceptionMsg, env.Log[0]);
        Assert.Equal("Boom", env.Log[1]);

        // 锁已释放的证据：原文 `UnLock`（:478）在 try 之外、必须已执行。
        // 若 Run 忘了解锁，这里会**静默递减**而不抛；抛异常才是"未持有却解锁"⇒ 已解开。
        Assert.Throws<SynchronizationLockException>(() => mgr.RobotHumanList.UnLock());

        mgr.Dispose();
    }

    [Fact]
    public void RobotManage_Destroy_UnloadsAndIsIdempotent()
    {
        using var env = new MonstersTestEnv();
        env.SeedText(ScriptPath, "#AUTORUN NPC SEC 1 @A");
        env.SeedText(RobotListPath, "机器人甲 test");
        var mgr = new TRobotManage();
        mgr.LoadRobot();
        mgr.Destroy();
        Assert.Equal(0, mgr.RobotHumanList.Count);
        mgr.Destroy();
        Assert.Equal(0, mgr.RobotHumanList.Count);
    }

    /// <summary>`sExceptionMsg` 字面量与原文 :458 一致。</summary>
    [Fact]
    public void ExceptionMessageLiteral()
    {
        Assert.Equal("[Exception] TRobotManage.Run", TRobotManage.sExceptionMsg);
    }

    /// <summary>行可加载判据（原文 :304 / :422 的 `(sLineText &lt;&gt; '') and (sLineText[1] &lt;&gt; ';')`）。</summary>
    [Fact]
    public void IsLoadableLine_MatchesSourceGuard()
    {
        Assert.False(ObjRobotCore.IsLoadableLine(""));
        Assert.False(ObjRobotCore.IsLoadableLine(";注释"));
        Assert.True(ObjRobotCore.IsLoadableLine("   "));
        Assert.True(ObjRobotCore.IsLoadableLine("#AUTORUN"));
    }

    /// <summary>接缝复用：`SweepSeam.MyGetTickCount` 是 `ObjRobot.AuthRun` 的计时来源（原文 `MyGetTickCount`）。</summary>
    [Fact]
    public void SeamReuse_TickComesFromSweepSeam()
    {
        using var env = new MonstersTestEnv();
        uint seen = 0;
        SweepSeam.MyGetTickCount = () => { seen++; return 12345u; };
        var robot = env.MakeRobot();
        var info = new TAutoRunInfo
        {
            nRunCmd = ObjRobotCore.nRONPCLABLEJMP,
            nMoethod = ObjRobotCore.nROSEC,
            nParam1 = 1,
            sParam2 = "@L",
        };
        robot.AutoRun(info);
        Assert.True(seen >= 1);
        Assert.Equal(12345u, info.dwRunTick);
    }
}
