using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J88：Actor.pas WORder 绘制顺序表（1206-1292）与查表逻辑（16675-16676）1:1 测试。
/// 表本身由脚本从原文逐字提取，此处断言其结构性质与若干锚点，
/// 以防后续有人手工"整理"这张表。
/// </summary>
public sealed class ActorWordOrderTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(600, ActorWordOrder.FrameCount);
        Assert.Equal(2, ActorWordOrder.SexCount);
    }

    // ===================== 表值与校验和 =====================

    [Fact]
    public void RowSumsMatchOriginalExtraction()
    {
        // 逐字提取时的校验和：行 0 = 344、行 1 = 345
        int s0 = 0, s1 = 0;
        for (int f = 0; f < ActorWordOrder.FrameCount; f++)
        {
            s0 += ActorWordOrder.WOrder(0, f);
            s1 += ActorWordOrder.WOrder(1, f);
        }

        Assert.Equal(344, s0);
        Assert.Equal(345, s1);
    }

    [Fact]
    public void AllValuesAreZeroOrOne()
    {
        for (int sex = 0; sex < ActorWordOrder.SexCount; sex++)
            for (int f = 0; f < ActorWordOrder.FrameCount; f++)
            {
                int v = ActorWordOrder.WOrder(sex, f);
                Assert.True(v == 0 || v == 1, $"WOrder[{sex},{f}] = {v}");
            }
    }

    // ===================== 锚点（逐字比对原文） =====================

    [Fact]
    public void Row0LeadingValues()
    {
        // 原文 1209：0×8 后接 1×16
        for (int i = 0; i < 8; i++)
            Assert.Equal(0, ActorWordOrder.WOrder(0, i));
        for (int i = 8; i < 24; i++)
            Assert.Equal(1, ActorWordOrder.WOrder(0, i));
    }

    [Fact]
    public void Row0BlockAt24To31()
    {
        // 原文 1210 剩余：1×8
        for (int i = 24; i < 32; i++)
            Assert.Equal(1, ActorWordOrder.WOrder(0, i));
    }

    [Fact]
    public void Row0TailOfFirstChunk()
    {
        // 原文 1211：0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,1 → 帧 48..63
        int[] expect = { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 };
        for (int i = 0; i < expect.Length; i++)
            Assert.Equal(expect[i], ActorWordOrder.WOrder(0, 48 + i));
    }

    [Fact]
    public void Row0LastValues()
    {
        // 原文 1247（row0 末行）：0,0,0,1,1,1,1,1,0,0,0,1,1,1,1,1
        int[] expect = { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 };
        for (int i = 0; i < expect.Length; i++)
            Assert.Equal(expect[i], ActorWordOrder.WOrder(0, 584 + i));
    }

    [Fact]
    public void Row1WarLine()
    {
        // row1 长度与 row0 一致，各 600 项
        int[] row1 = Row1Values();
        Assert.Equal(600, row1.Length);
    }

    [Fact]
    public void RowsDifferAtExactlyOneFrame()
    {
        // 实测：两行**仅在帧 192 处不同**（r0 = 0、r1 = 1），其余 599 帧完全相同。
        // 这正是"男/女武器顺序表"在本作中唯一的差异点——若有人"整理"这张表，
        // 此断言会立刻失败。
        int diffCount = 0;
        for (int f = 0; f < ActorWordOrder.FrameCount; f++)
            if (ActorWordOrder.WOrder(0, f) != ActorWordOrder.WOrder(1, f))
                diffCount++;

        Assert.Equal(1, diffCount);
        Assert.NotEqual(ActorWordOrder.WOrder(0, 192), ActorWordOrder.WOrder(1, 192));
        Assert.Equal(0, ActorWordOrder.WOrder(0, 192));
        Assert.Equal(1, ActorWordOrder.WOrder(1, 192));
    }

    [Fact]
    public void RowsAreIdenticalBeforeFrame192()
    {
        for (int f = 0; f < 192; f++)
            Assert.Equal(ActorWordOrder.WOrder(0, f), ActorWordOrder.WOrder(1, f));
    }

    [Fact]
    public void RowsAreIdenticalAfterFrame192()
    {
        for (int f = 193; f < ActorWordOrder.FrameCount; f++)
            Assert.Equal(ActorWordOrder.WOrder(0, f), ActorWordOrder.WOrder(1, f));
    }

    [Fact]
    public void Row1DiffersFromRow0Somewhere()
    {
        bool anyDiff = false;
        for (int f = 0; f < ActorWordOrder.FrameCount; f++)
            if (ActorWordOrder.WOrder(0, f) != ActorWordOrder.WOrder(1, f))
                anyDiff = true;

        Assert.True(anyDiff, "两行不应完全相同");
    }

    [Fact]
    public void Row0SharesRow1ForFirstFortyEightFrames()
    {
        // 前 48 帧两行一致（原文 1209 与 1252 相同）
        for (int i = 0; i < 48; i++)
            Assert.Equal(ActorWordOrder.WOrder(0, i), ActorWordOrder.WOrder(1, i));
    }

    private static int[] Row1Values()
    {
        var a = new int[ActorWordOrder.FrameCount];
        for (int f = 0; f < ActorWordOrder.FrameCount; f++)
            a[f] = ActorWordOrder.WOrder(1, f);
        return a;
    }

    // ===================== 越界 =====================

    [Fact]
    public void FrameAtBoundsIsValid()
    {
        Assert.InRange(ActorWordOrder.WOrder(0, 0), 0, 1);
        Assert.InRange(ActorWordOrder.WOrder(0, 599), 0, 1);
        Assert.InRange(ActorWordOrder.WOrder(1, 0), 0, 1);
        Assert.InRange(ActorWordOrder.WOrder(1, 599), 0, 1);
    }

    [Fact]
    public void FrameBelowZeroReturnsMinusOne()
    {
        Assert.Equal(-1, ActorWordOrder.WOrder(0, -1));
    }

    [Fact]
    public void FrameAtSixHundredReturnsMinusOne()
    {
        Assert.Equal(-1, ActorWordOrder.WOrder(0, 600));
    }

    [Fact]
    public void SexOutOfRangeReturnsMinusOne()
    {
        Assert.Equal(-1, ActorWordOrder.WOrder(-1, 0));
        Assert.Equal(-1, ActorWordOrder.WOrder(2, 0));
    }

    // ===================== Lookup（16675-16676） =====================

    [Fact]
    public void LookupWritesValueInRange()
    {
        int wpord = 99;
        ActorWordOrder.Lookup(0, 8, ref wpord);
        Assert.Equal(1, wpord);
    }

    [Fact]
    public void LookupWritesZeroCorrectly()
    {
        int wpord = 99;
        ActorWordOrder.Lookup(0, 0, ref wpord);
        Assert.Equal(0, wpord);
    }

    [Fact]
    public void LookupLeavesValueUnchangedWhenFrameNegative()
    {
        // 16675：越界时**不查表也不清零**，保持原值
        int wpord = 7;
        ActorWordOrder.Lookup(0, -1, ref wpord);
        Assert.Equal(7, wpord);
    }

    [Fact]
    public void LookupLeavesValueUnchangedWhenFrameAbove599()
    {
        int wpord = 7;
        ActorWordOrder.Lookup(0, 600, ref wpord);
        Assert.Equal(7, wpord);
    }

    [Fact]
    public void LookupBoundaryFramesAreInclusive()
    {
        int a = -1, b = -1;
        ActorWordOrder.Lookup(0, 0, ref a);
        ActorWordOrder.Lookup(0, 599, ref b);

        Assert.InRange(a, 0, 1);
        Assert.InRange(b, 0, 1);
    }

    [Fact]
    public void LookupSelectsRowBySex()
    {
        // 用唯一差异帧 192 验证行选择：r0 = 0、r1 = 1
        int r0 = 9, r1 = 9;
        ActorWordOrder.Lookup(0, 192, ref r0);
        ActorWordOrder.Lookup(1, 192, ref r1);

        Assert.Equal(0, r0);
        Assert.Equal(1, r1);
    }

    // ===================== 查表语义（16685 的判据） =====================

    [Fact]
    public void WpordZeroMeansWeaponDrawnFirst()
    {
        // 16685：`m_nWpord = 0` 时画武器（在身体之前）
        int wpord = 5;
        ActorWordOrder.Lookup(0, 0, ref wpord);
        Assert.Equal(0, wpord);
    }

    [Fact]
    public void WpordOneMeansWeaponDrawnAfter()
    {
        int wpord = 5;
        ActorWordOrder.Lookup(0, 8, ref wpord);
        Assert.NotEqual(0, wpord);
    }
}
