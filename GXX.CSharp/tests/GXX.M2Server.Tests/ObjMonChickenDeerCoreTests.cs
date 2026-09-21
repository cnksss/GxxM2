using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

// ============================================================================
//  ⚠ 本文件是**沉默桩清理**的目标（车道 `p13-m2-objmon-real`，台账 §48.1/§49.3）
//
//  修前：本文件里的每一个 `Assert.True(ObjMonChickenDeerCore.Xxx())` 都是
//        "断言 `=> true;` 返回真" —— 编译过、测试过、**证明不了任何事**。
//        它们锁住的不是原文，而是移植者写下的一个恒真式。
//
//  修后：被断言的谓词已全部改成**真的去读 `ObjMon.pas` 原文再判定**的实现体
//        （判定不成立即抛异常，不再静默为真）。
//        本文件保留**不依赖被清理谓词**的部分（常量表、纯函数边界）；
//        原文取证部分整段迁往 `ObjMonChickenDeerRealTests`（那里逐条用原文重算）。
// ============================================================================

/// <summary>
/// 批次J199 的常量/跨度断言（保留）。
/// 谓词层面的原文取证见 <see cref="ObjMonChickenDeerRealTests"/>。
/// </summary>
public sealed class ObjMonChickenDeerCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1382, ObjMonChickenDeerCore.CreateStart);
        Assert.Equal(5, ObjMonChickenDeerCore.CreateLines);
        Assert.Equal(1388, ObjMonChickenDeerCore.DestroyStart);
        Assert.Equal(4, ObjMonChickenDeerCore.DestroyLines);
        Assert.Equal(1393, ObjMonChickenDeerCore.RunStart);
        Assert.Equal(1464, ObjMonChickenDeerCore.RunEnd);
        Assert.Equal(72, ObjMonChickenDeerCore.RunLines);
        Assert.Equal(81, ObjMonChickenDeerCore.TotalLines);
        Assert.Equal(25, ObjMonChickenDeerCore.ClassDeclStart);
        Assert.Equal(30, ObjMonChickenDeerCore.ClassDeclEnd);
        Assert.Equal(5, ObjMonChickenDeerCore.ViewRange);
        Assert.Equal(9999, ObjMonChickenDeerCore.DistanceSeed);
        Assert.Equal(6, ObjMonChickenDeerCore.NearThreshold);
        Assert.Equal(5, ObjMonChickenDeerCore.NextPositionParam);
        Assert.Equal(0, ObjMonChickenDeerCore.RC_PLAYOBJECT);
        Assert.Equal(11, ObjMonChickenDeerCore.Bo554InMonster);
        Assert.Equal(182, ObjMonChickenDeerCore.Bo554InFox);
        Assert.Equal(821, ObjMonChickenDeerCore.RunAwayModeDeclLine);
        Assert.Equal(7, ObjMonChickenDeerCore.RunAwayAssignCount);
        Assert.Equal(1, ObjMonChickenDeerCore.RunAwayTrueCount);
        Assert.Equal(6, ObjMonChickenDeerCore.RunAwayFalseCount);
        Assert.Equal(189, ObjMonChickenDeerCore.RemainingImplCount);
        Assert.Equal(54, ObjMonChickenDeerCore.RemainingClassCount);
        Assert.Equal(1456, ObjMonChickenDeerCore.DuplicatedConditionLine);
        Assert.Equal(1444, ObjMonChickenDeerCore.RunAwayTrueLine);
        Assert.Equal(1191, ObjMonChickenDeerCore.BaseRunAwayGuardLine);
        Assert.Equal(1429, ObjMonChickenDeerCore.NearestScanLine);
    }

    /// <summary>脚本抽取出来的三张行号表仍按原样锁定。</summary>
    [Fact]
    public void AssignSites()
    {
        Assert.Equal(new[] { 769, 1075, 1332, 1444, 1449, 5517, 5911 },
            ObjMonChickenDeerCore.RunAwayAssignSites);
        Assert.Equal(new[] { 769, 5517, 5911 },
            ObjMonChickenDeerCore.RunAwayTimeoutSites);
        Assert.Equal(new[] { 1404, 1453 }, ObjMonChickenDeerCore.StageGuardLines);
    }

    /// <summary>纯函数（不依赖原文）的对照表：基类语义 vs 本类语义。</summary>
    [Fact]
    public void SemanticsAreOpposed()
    {
        Assert.False(ObjMonChickenDeerCore.BaseProceedsWithMove(true));
        Assert.True(ObjMonChickenDeerCore.BaseProceedsWithMove(false));
        Assert.True(ObjMonChickenDeerCore.HasLockedTarget(true));
        Assert.NotEqual(ObjMonChickenDeerCore.BaseProceedsWithMove(true),
            ObjMonChickenDeerCore.HasLockedTarget(true));
    }

    /// <summary>走速 / 距离两个纯函数的边界值（不依赖原文）。</summary>
    [Fact]
    public void WalkAndDistanceBoundaries()
    {
        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 500, 500, 0));
        Assert.False(ObjMonChickenDeerCore.CanWalk(0, 499, 500, 0));
        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 501, 500, 0));
        Assert.Equal(2, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 1));
        Assert.Equal(1, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 0));
        Assert.Equal(0, ObjMonChickenDeerCore.Manhattan(5, 5, 5, 5));
    }
}
