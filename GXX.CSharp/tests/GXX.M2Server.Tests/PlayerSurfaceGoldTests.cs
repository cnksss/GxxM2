// ============================================================================
// 测试：本车道 **金额容器片**（切片 2 / 4）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Gold.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:1181 / 201 / 250 / 48 / 3231-3249 /
//           3284-3293 / 3251-3260 / 3295-3303 / 1168-1171 / 2529-2547
//           Source/M2Engine/ObjBase.pas:105 / 106 / 11276 / 11311
// 用例 ≥3/方法：0 / 边界 / 无符号回绕 / 超上限 / 差异断言。
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class PlayerSurfaceGoldTests : IDisposable
{
    public PlayerSurfaceGoldTests() => PlayerSurfaceMsgSeams.ResetDefaults();
    public void Dispose() => PlayerSurfaceMsgSeams.ResetDefaults();

    private static TPlayObject NewPlayer(uint gold = 0)
        => new TPlayObject { m_nGold = gold, m_nGoldMax = 5_000_000 };

    // ---------------------------------------------------------------
    // 字段形状
    // ---------------------------------------------------------------

    [Fact]
    public void GoldFields_ExistWithOriginalTypes()
    {
        // 原文 ObjBase.pas:105 m_nGold: LongWord（**已存在**于 ObjBase.OnlineMsg.cs:44，未重复声明）
        // 原文 ObjPlayer.pas:201 m_nGameGoldEx / :250 m_nDealGoldPose / :48 m_nBigStoragePage
        var p = new TPlayObject();
        Assert.Equal(0u, p.m_nGold);
        Assert.Equal(0, p.m_nGameGold);
        Assert.Equal(0, p.m_nGameGoldEx);
        Assert.Equal(0, p.m_nDealGoldPose);
        Assert.Equal(0, p.m_nBigStoragePage);
        // 原文 ObjBase.pas:11311 m_nGoldMax := g_Config.nHumanMaxGold
        Assert.Equal((uint)M2Config.nHumanMaxGold, p.m_nGoldMax);
    }

    // ---------------------------------------------------------------
    // IncGold（原文 3231-3249）
    // ---------------------------------------------------------------

    [Fact]
    public void IncGold_ZeroAmount_ReturnsFalseAndDoesNotChange()
    {
        // 原文 3239 `if tGold > 0` 为假 → Result 保持 False（重点：0 **不算**成功）
        var p = NewPlayer(100);
        Assert.False(p.IncGold(0));
        Assert.Equal(100u, p.m_nGold);
    }

    [Fact]
    public void IncGold_NormalAmount_AddsAndReturnsTrue()
    {
        var p = NewPlayer(100);
        Assert.True(p.IncGold(25));
        Assert.Equal(125u, p.m_nGold);
    }

    [Fact]
    public void IncGold_ClampsAtGoldMax()
    {
        // 原文 3242-3245：tmpValue > m_nGoldMax → m_nGold := m_nGoldMax
        var p = NewPlayer(4_999_990);
        Assert.True(p.IncGold(1000));
        Assert.Equal(5_000_000u, p.m_nGold);
    }

    [Fact]
    public void IncGold_AtGoldMax_ReturnsFalse()
    {
        // 原文 3247 Result := m_nGold > tmpGold → 已在 max 时 Inc 不改变 → False
        var p = NewPlayer(5_000_000);
        Assert.False(p.IncGold(1));
        Assert.Equal(5_000_000u, p.m_nGold);
    }

    [Fact]
    public void IncGold_CardinalWraparound_DiffersFromInt64Addition()
    {
        // ★ 差异断言（本片最重要的一条）：
        //   原文 3241 `tmpValue := Min(m_nGold + tGold, MAXDWORD - 1)` 里的加法是
        //   **Cardinal（32 位无符号）加法**，即在 `Min` **之前**就已回绕。
        //   取 m_nGold = 4_000_000_000、tGold = 1_000_000_000：
        //     · Cardinal 路径：(4_000_000_000 + 1_000_000_000) mod 2^32 = 705_032_704
        //       → Min(705_032_704, 4_294_967_294) = 705_032_704
        //     · Int64 路径  ：5_000_000_000 → Min(..., 4_294_967_294) = 4_294_967_294
        //   两条路径给出**完全不同的** m_nGold，故可用一个用例把回绕锁死。
        //   把 m_nGoldMax 设到 MAXDWORD-1 之上，避免被夹到上限而掩盖差异。
        var p = new TPlayObject { m_nGold = 4_000_000_000u, m_nGoldMax = uint.MaxValue };

        // m_nGold 从 4_000_000_000 变成 705_032_704（变小）→ Result := m_nGold > tmpGold = False
        Assert.False(p.IncGold(1_000_000_000u));

        // Cardinal 回绕的**确证**：705_032_704
        Assert.Equal(705_032_704u, p.m_nGold);
        // 差异：Int64 语义会得到 4_294_967_294
        Assert.NotEqual(4_294_967_294u, p.m_nGold);
    }

    [Fact]
    public void IncGold_WrapToSmallValue_BelowMax_IsAccepted()
    {
        // 同上，但构造「回绕后变大」的一例以覆盖 Result := True 的分支：
        //   m_nGold = 4_294_967_295（MAXDWORD）+ 1_000 → 回绕成 999
        //   → 999 <= m_nGoldMax(=1000) → m_nGold = 999 → Result := 999 > 4_294_967_295 = False
        // 即：**回绕后即使数值合法，只要小于原值 Result 仍为 False**。
        var p = new TPlayObject { m_nGold = uint.MaxValue, m_nGoldMax = 1000 };
        Assert.False(p.IncGold(1000u));
        Assert.Equal(999u, p.m_nGold);
    }

    [Fact]
    public void IncGold_WrapToZero_ReturnsFalseBecauseNotGreater()
    {
        // m_nGold = 4_294_967_295（MAXDWORD）+ 1 → 回绕 0 → Min(0, MAXDWORD-1) = 0
        // → 0 <= m_nGoldMax → m_nGold = 0；Result := 0 > 4_294_967_295 → False
        var p = new TPlayObject { m_nGold = uint.MaxValue, m_nGoldMax = uint.MaxValue };
        Assert.False(p.IncGold(1));
        Assert.Equal(0u, p.m_nGold);
    }

    [Fact]
    public void IncGold_MaxDwordMinus1_IsTheCeilingOfTmpValue()
    {
        // 原文 3241 用 `MAXDWORD - 1`（= 4294967294，**不是** MAXDWORD）作 Min 的上界
        var q = new TPlayObject { m_nGold = 4_294_967_290u, m_nGoldMax = uint.MaxValue - 1 };
        Assert.True(q.IncGold(4));   // 4_294_967_294 = MAXDWORD - 1，正好是上界
        Assert.Equal(4_294_967_294u, q.m_nGold);

        // 再加 1：tmpValue = Min(4_294_967_295, 4_294_967_294) = 4_294_967_294（被 Min 夹住）
        // 但因 m_nGold 已等于 tmpValue → 不变 → Result := m_nGold > tmpGold → False
        Assert.False(q.IncGold(1));
        Assert.Equal(4_294_967_294u, q.m_nGold);
    }

    // ---------------------------------------------------------------
    // DecGold（原文 3284-3293）—— 与 DecGameGold 语义不同
    // ---------------------------------------------------------------

    [Fact]
    public void DecGold_Enough_SubtractsAndReturnsTrue()
    {
        var p = NewPlayer(100);
        Assert.True(p.DecGold(30));
        Assert.Equal(70u, p.m_nGold);
    }

    [Fact]
    public void DecGold_ExactlyEnough_SucceedsToZero()
    {
        // 原文 3288 是 `>=`（可等于）
        var p = NewPlayer(100);
        Assert.True(p.DecGold(100));
        Assert.Equal(0u, p.m_nGold);
    }

    [Fact]
    public void DecGold_NotEnough_LeavesGoldUntouched_AndReturnsFalse()
    {
        // ★ 差异断言：DecGold 不够时**原地不动**（不是夹到 0）
        var p = NewPlayer(100);
        Assert.False(p.DecGold(101));
        Assert.Equal(100u, p.m_nGold);
    }

    [Fact]
    public void DecGold_Zero_IsAlwaysSuccessWhenGoldNonNegative()
    {
        // nGold = 0：m_nGold >= 0 恒真 → 扣 0 → True
        var p = NewPlayer(0);
        Assert.True(p.DecGold(0));
        Assert.Equal(0u, p.m_nGold);
    }

    [Fact]
    public void DecGold_FromZeroByPositive_ReturnsFalse()
    {
        var p = NewPlayer(0);
        Assert.False(p.DecGold(1));
        Assert.Equal(0u, p.m_nGold);
    }

    // ---------------------------------------------------------------
    // IncGameGold / DecGameGold（原文 3251-3260 / 3295-3303）
    // ---------------------------------------------------------------

    [Fact]
    public void IncGameGold_ClampsAtHighLongWord_NotAtGoldMax()
    {
        // ★ 差异断言：IncGameGold **不**受 m_nGoldMax 约束，只夹 High(LongWord)
        var p = new TPlayObject { m_nGameGold = 10, m_nGoldMax = 50 };
        p.IncGameGold(1000);
        Assert.Equal(1010, p.m_nGameGold);   // 未被 50 夹住

        p.m_nGameGold = unchecked((int)uint.MaxValue);
        p.IncGameGold(1);
        Assert.Equal(unchecked((int)uint.MaxValue), p.m_nGameGold);
    }

    [Fact]
    public void IncGameGold_Zero_NoChange()
    {
        var p = new TPlayObject { m_nGameGold = 7 };
        p.IncGameGold(0);
        Assert.Equal(7, p.m_nGameGold);
    }

    [Fact]
    public void DecGameGold_NotEnough_ClampsToZero_DiffersFromDecGold()
    {
        // ★ 差异断言：DecGameGold 不够时**夹到 0**（DecGold 是原地不动）
        var p = new TPlayObject { m_nGameGold = 100 };
        p.DecGameGold(101);
        Assert.Equal(0, p.m_nGameGold);

        var q = NewPlayer(100);
        Assert.False(q.DecGold(101));
        Assert.Equal(100u, q.m_nGold);   // 对照
    }

    [Fact]
    public void DecGameGold_ExactlyEnough_ReachesZero()
    {
        var p = new TPlayObject { m_nGameGold = 100 };
        p.DecGameGold(100);
        Assert.Equal(0, p.m_nGameGold);
    }

    [Fact]
    public void DecGameGold_Zero_NoChange()
    {
        var p = new TPlayObject { m_nGameGold = 100 };
        p.DecGameGold(0);
        Assert.Equal(100, p.m_nGameGold);
    }

    // ---------------------------------------------------------------
    // Changed 通知（原文 2529-2547）
    // ---------------------------------------------------------------

    [Fact]
    public void GoldChanged_SendsRmGoldChanged()
    {
        var p = new TPlayObject();
        int ident = -1;
        PlayerSurfaceMsgSeams.SendUpdateMsg = (_, w) => ident = w;

        p.GoldChanged();

        Assert.Equal(Grobal2Const.RM_GOLDCHANGED, ident);
        Assert.Equal(20096, Grobal2Const.RM_GOLDCHANGED);
    }

    [Fact]
    public void GameGoldChanged_SendsRmGameGoldChanged()
    {
        var p = new TPlayObject();
        int ident = -1;
        PlayerSurfaceMsgSeams.SendUpdateMsg = (_, w) => ident = w;

        p.GameGoldChanged();

        Assert.Equal(Grobal2Const.RM_GAMEGOLDCHANGED, ident);
        Assert.Equal(20146, Grobal2Const.RM_GAMEGOLDCHANGED);
    }

    [Fact]
    public void NewGamePointChanged_And_GameGloryChanged_SendTheirIdents()
    {
        var p = new TPlayObject();
        var seen = new System.Collections.Generic.List<int>();
        PlayerSurfaceMsgSeams.SendUpdateMsg = (_, w) => seen.Add(w);

        p.NewGamePointChanged();
        p.GameGloryChanged();

        Assert.Equal(new[] { Grobal2Const.RM_GAMEPOINTCHANGED, Grobal2Const.RM_GAMEGLORY }, seen);
    }

    [Fact]
    public void ChangedNotifications_DefaultSeam_IsNoOp_AndDoesNotThrow()
    {
        var p = new TPlayObject();
        p.GoldChanged();
        p.GameGoldChanged();
        p.NewGamePointChanged();
        p.GameGloryChanged();
    }
}
