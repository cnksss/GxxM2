using System;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J189：`GetNextPosition` 对**非法方向**的处理对齐 Delphi `case` 语义。
///
/// **背景**：批次J188 发现本方法的修正前实现用 `DirDeltaX(dir)` →
/// `s_DirX[Math.Min(dir, (byte)7)]`，把 `-1` 截断成的 `255` **夹到 7**、
/// 于是按 `DR_UPLEFT` 移动；而原文 `Envir.pas` 4528 是 `case nDir of` 加八个
/// 显式标签、且坐标先赋原值，**未列举值不匹配任何标签即"原地不动"**。
/// 本批次按原文语义修正。
///
/// **本类的断言分三组**：① 修正后的语义与原文 `case` 逐方向一致；
/// ② 五个生产调用点（`Magic.cs` ×4、`MagicGroup.cs` ×1）传入的都是
/// `GetNextDirection` 的返回值（恒在 0..7）、**行为完全不变**；
/// ③ 非法方向不再产生位移。
/// </summary>
public sealed class GetNextPositionFixTests
{
    // ===================== 一、八个合法方向 =====================

    [Fact]
    public void AllEightLegalDirectionsMatchDelphi()
    {
        // **原文八个 case 分支的增量**
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

        for (byte d = 0; d <= 7; d++)
        {
            TCreature.GetNextPosition(10, 10, d, 1, out int nx, out int ny);

            Assert.Equal(10 + dx[d], nx);
            Assert.Equal(10 + dy[d], ny);
        }
    }

    [Fact]
    public void StepCountMultiplies()
    {
        // **n 步即增量的 n 倍（原文 nFlag）**
        TCreature.GetNextPosition(10, 10, 0, 5, out int nx, out int ny);
        Assert.Equal(10, nx);
        Assert.Equal(5, ny);

        TCreature.GetNextPosition(10, 10, 2, 8, out nx, out ny);
        Assert.Equal(18, nx);
        Assert.Equal(10, ny);
    }

    [Fact]
    public void LegalDirectionsUnalteredByFix()
    {
        // **修正前后合法方向的结果必须相同 —— 这是"零回归"的核心保证**
        int[] oldDx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] oldDy = { -1, -1, 0, 1, 1, 1, 0, -1 };

        for (byte d = 0; d <= 7; d++)
        {
            for (int n = 1; n <= 9; n++)
            {
                TCreature.GetNextPosition(20, 20, d, n, out int nx, out int ny);

                // **修正前：20 + DirDeltaX(d) * n**
                Assert.Equal(20 + oldDx[d] * n, nx);
                Assert.Equal(20 + oldDy[d] * n, ny);
            }
        }
    }

    // ===================== 二、非法方向 =====================

    [Fact]
    public void IllegalDirectionDoesNotMove()
    {
        // **255（即 (byte)(-1)）—— 修正前会按 DR_UPLEFT 移动**
        TCreature.GetNextPosition(10, 10, 255, 1, out int nx, out int ny);
        Assert.Equal(10, nx);
        Assert.Equal(10, ny);

        // **修正前的错误行为（探针实测）：变为 (9,9)**
        Assert.NotEqual(9, nx);
        Assert.NotEqual(9, ny);
    }

    [Fact]
    public void IllegalDirectionIgnoresStepCount()
    {
        // **非法方向下 n 再大也不动**
        TCreature.GetNextPosition(10, 10, 255, 5, out int nx, out int ny);
        Assert.Equal(10, nx);
        Assert.Equal(10, ny);

        TCreature.GetNextPosition(10, 10, 255, 100, out nx, out ny);
        Assert.Equal(10, nx);
        Assert.Equal(10, ny);
    }

    [Fact]
    public void AllIllegalDirectionsDoNotMove()
    {
        // **8..255 全部非法值都不动**
        for (int d = 8; d <= 255; d++)
        {
            TCreature.GetNextPosition(7, 9, (byte)d, 3, out int nx, out int ny);

            Assert.Equal(7, nx);
            Assert.Equal(9, ny);
        }
    }

    [Fact]
    public void MinimalIllegalDirectionIsEight()
    {
        // **7 是合法上界（DR_UPLEFT）、8 是最小非法值**
        TCreature.GetNextPosition(10, 10, 7, 1, out int nx, out int ny);
        Assert.Equal(9, nx);
        Assert.Equal(9, ny);

        TCreature.GetNextPosition(10, 10, 8, 1, out nx, out ny);
        Assert.Equal(10, nx);
        Assert.Equal(10, ny);
    }

    [Fact]
    public void IllegalDirectionBoundaryPair()
    {
        // **合法与非法在 7/8 之间分界**
        Assert.True(TCreature.IsLegalDirection(7));
        Assert.False(TCreature.IsLegalDirection(8));
        Assert.True(TCreature.IsLegalDirection(0));
        Assert.False(TCreature.IsLegalDirection(255));
    }

    // ===================== 三、与 J188 记录的偏差对照 =====================

    [Fact]
    public void FixMatchesDelphiCaseSemantics()
    {
        // **原文 case：未列举值不动 —— 修正后一致**
        var (delphiX, delphiY) = HumMonActThinkCore.DelphiGetNextPosition(10, 10, -1, 1);

        TCreature.GetNextPosition(10, 10, unchecked((byte)(-1)), 1, out int nx, out int ny);

        Assert.Equal(delphiX, nx);
        Assert.Equal(delphiY, ny);
    }

    [Fact]
    public void J188DivergenceNowResolved()
    {
        // **J188 记录的偏差：Delphi 不动、C# 走左上 —— 现已消除**
        TCreature.GetNextPosition(10, 10, 255, 1, out int nx, out int ny);

        // **不再是修正前的 (9,9)**
        Assert.False(nx == 9 && ny == 9);
        // **而是原文的 (10,10)**
        Assert.True(nx == 10 && ny == 10);
    }

    // ===================== 四、生产调用点不受影响 =====================

    [Fact]
    public void ProductionCallSitesUseLegalDirectionsOnly()
    {
        // **五个调用点都先经 GetNextDirection —— 其返回值恒在 0..7**
        // Magic.cs:334/351 与 MagicGroup.cs 的同名前置调用
        int[][] coords =
        {
            new[] { 100, 100, 105, 103 },
            new[] { 50, 50, 50, 40 },
            new[] { 20, 30, 20, 30 },
            new[] { 7, 7, 1, 1 },
            new[] { 88, 99, 90, 95 },
        };

        foreach (int[] c in coords)
        {
            byte dir = TCreature.GetNextDirection(c[0], c[1], c[2], c[3]);

            // **恒为合法方向**
            Assert.True(dir <= 7);

            // **故修正后行为与修正前一致（用同一个合法方向比对）**
            TCreature.GetNextPosition(c[0], c[1], dir, 1, out int nx, out int ny);

            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

            Assert.Equal(c[0] + dx[dir], nx);
            Assert.Equal(c[1] + dy[dir], ny);
        }
    }

    [Fact]
    public void GetNextDirectionNeverReturnsIllegal()
    {
        // **穷举一块网格内的所有坐标对 —— GetNextDirection 恒返回 0..7**
        for (int sx = 0; sx <= 6; sx++)
        {
            for (int sy = 0; sy <= 6; sy++)
            {
                for (int tx = 0; tx <= 6; tx++)
                {
                    for (int ty = 0; ty <= 6; ty++)
                    {
                        byte d = TCreature.GetNextDirection(sx, sy, tx, ty);

                        Assert.True(d <= 7);
                    }
                }
            }
        }
    }

    [Fact]
    public void HellFireRangeBehaviourUnchanged()
    {
        // **MagMakeHellFire 用 n=5、MagMakeQuickLighting 用 n=8 —— 合法方向下不变**
        byte dir = TCreature.GetNextDirection(100, 100, 105, 105);

        TCreature.GetNextPosition(100, 100, dir, 5, out int fx, out int fy);
        TCreature.GetNextPosition(100, 100, dir, 8, out int qx, out int qy);

        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

        Assert.Equal(100 + dx[dir] * 5, fx);
        Assert.Equal(100 + dy[dir] * 5, fy);

        Assert.Equal(100 + dx[dir] * 8, qx);
        Assert.Equal(100 + dy[dir] * 8, qy);
    }

    [Fact]
    public void OutputsAlwaysAssigned()
    {
        // **原文先赋 snX := sX —— 修正后同样保证 out 参数必被赋值**
        TCreature.GetNextPosition(3, 4, 255, 1, out int nx, out int ny);
        Assert.Equal(3, nx);
        Assert.Equal(4, ny);
    }
}
