// ============================================================================
// 测试：`TWarrContinueHitManager`（ObjPlayer.pas 的**第 2 个类**，原文 1398-1482）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TWarrContinueHitManager.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:1398-1409（声明）/ 1417-1482（实现）
// 用例覆盖：4/4 例程（Create / CanOpenMagic / CanUseMagic / UseMagic）
//           + 2 处「原文如此」缺陷的锁定断言 + 1 处回绕差异断言。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection(PlayerSurfacePortLedgerSerialCollection.Name)]
public class ObjPlayerWarrContinueHitManagerTests : IDisposable
{
    public ObjPlayerWarrContinueHitManagerTests() => PlayerSurfaceWarrContinueConfig.ResetDefaults();
    public void Dispose() => PlayerSurfaceWarrContinueConfig.ResetDefaults();

    private static TWarrContinueHitManager NewManager() => new(new TPlayObject());

    // ==================================================================
    // Create（原文 1418-1423）
    // ==================================================================

    /// <summary>原文 1420/1405：`FPlayer := APlayer`，`Player` 只读属性直接暴露它。</summary>
    [Fact]
    public void Create_StoresPlayer()
    {
        var player = new TPlayObject();
        var m = new TWarrContinueHitManager(player);
        Assert.Same(player, m.Player);
    }

    /// <summary>
    /// ★ 原文如此（1418-1423）：`Create` 只置 `FLastUseMagicTick := 0`，
    /// **没有**给 `FLastUseMagicID` 赋值 —— Delphi 字段零填充使其为 0。此处锁死"初值为 0"。
    /// </summary>
    [Fact]
    public void Create_InitialisesOnlyTick_LastMagicIdIsZeroByZeroFill_OriginalDefect()
    {
        var m = NewManager();
        Assert.Equal(0u, m.LastUseMagicTickForTest);
        Assert.Equal((ushort)0, m.LastUseMagicIdForTest);   // 靠零填充，而非显式赋值
    }

    // ==================================================================
    // CanOpenMagic（原文 1425-1428）
    // ==================================================================

    /// <summary>
    /// ★★ 原始缺陷锁定（原文如此，1425-1428）：两个形参一个都不用、恒返回 True，
    /// 且 **`MagicName`（var 出参）从不被写**。若有人"顺手实现"了它，本用例会失败。
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(65535)]
    public void CanOpenMagic_AlwaysTrue_AndNeverWritesMagicName(int magicId)
    {
        var m = NewManager();

        // 各种入参都必须返回 True
        string magicName = "sentinel-未改动";
        bool r = m.CanOpenMagic((ushort)magicId, ref magicName);

        Assert.True(r);
        // var 出参保持调用方传入的原值 —— 一个字节都没改
        Assert.Equal("sentinel-未改动", magicName);
    }

    // ==================================================================
    // CanUseMagic（原文 1430-1459）
    // ==================================================================

    /// <summary>
    /// 门 1（原文 1437-1438，**反逻辑**）：`not g_Config.boDisableWarrContinueHit` 为真
    /// （即"未启用该限制"）时**立刻返回 True** —— 哪怕 FLastUseMagicID 已记录、白名单命中。
    /// </summary>
    [Fact]
    public void CanUseMagic_Gate1_WhenDisableSwitchOff_AlwaysTrue()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = false;   // 未启用禁用 → 门 1 放行
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;

        var m = NewManager();
        m.UseMagic(100);                     // 记录成功（白名单命中）
        Assert.Equal((ushort)100, m.LastUseMagicIdForTest);

        // 立刻再用另一个白名单技能 → 若门 1 不存在，会因间隔不足返回 False；门 1 使其返回 True
        Assert.True(m.CanUseMagic(200));
    }

    /// <summary>门 2（原文 1440-1441）：`FLastUseMagicID = 0` → 直接 True。</summary>
    [Fact]
    public void CanUseMagic_Gate2_NoRecordYet_ReturnsTrue()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);

        var m = NewManager();
        Assert.Equal((ushort)0, m.LastUseMagicIdForTest);
        Assert.True(m.CanUseMagic(100));
    }

    /// <summary>门 3（原文 1443-1444）：`FLastUseMagicID = MagicID`（同一技能）→ 直接 True，不受间隔约束。</summary>
    [Fact]
    public void CanUseMagic_Gate3_SameMagicId_ReturnsTrue_DespiteInterval()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;   // 一分钟
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);

        var m = NewManager();
        m.UseMagic(100);
        // 同一技能 → 门 3 早退 True（若没有门 3，间隔 0 < 60000 会返回 False）
        Assert.True(m.CanUseMagic(100));
    }

    /// <summary>
    /// 白名单未命中（原文 1446-1457）：循环跑完 `IsFound` 仍为 False → **不进入限流判定**，
    /// 返回初始 `Result := True`。即"非连续攻击技能不受本管理器限制"。
    /// </summary>
    [Fact]
    public void CanUseMagic_NotInWhitelist_ReturnsInitialTrue_NotThrottled()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(101);

        var m = NewManager();
        m.UseMagic(100);        // FLastUseMagicID = 100

        // 999 不在白名单 → IsFound False → 返回初始 True（不受 60s 间隔影响）
        Assert.True(m.CanUseMagic(999));
    }

    /// <summary>
    /// 白名单命中且间隔不足（原文 1457-1458）：`Tick_Diff(上次, 现在) >= 间隔` 为假 → **False**。
    /// 这是本管理器唯一会返回 False 的路径。
    /// </summary>
    [Fact]
    public void CanUseMagic_WhitelistHit_AndIntervalNotElapsed_ReturnsFalse()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;   // 60 秒
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(200);

        var m = NewManager();
        m.UseMagic(100);                     // 刚记录 tick=now

        // 立刻切到另一个白名单技能 → 间隔 ≈ 0 < 60000 → False
        Assert.False(m.CanUseMagic(200));
    }

    /// <summary>
    /// 间隔边界（原文 1458 的 `>=`）：间隔取 **1** 时，`Tick_Diff` 若为 0 则不满足（0 >= 1 为假）。
    /// 把间隔设为 0 则恒满足 —— 证明比较是 `>=` 而不是 `>`。
    /// </summary>
    [Fact]
    public void CanUseMagic_IntervalBoundary_UsesGreaterOrEqual()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(100);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(200);

        var m = NewManager();
        m.UseMagic(100);

        // 间隔 = int.MaxValue → Tick_Diff（uint）几乎不可能 >= 2^31-1 → False（上限方向）
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = int.MaxValue;
        Assert.False(m.CanUseMagic(200));

        // 间隔 = 0 → Tick_Diff(..) >= 0 恒真 → True（若写成 > 则此处为 False）
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 0;
        Assert.True(m.CanUseMagic(200));
    }

    /// <summary>
    /// ★ 差异断言（原文 1458 的 `Tick_Diff(旧, 新)` 参数顺序 + 无符号回绕）：
    /// `Tick_Diff(a, b) = b - a`（**不是** `a - b`）。若把参数顺序写反，
    /// "上次 tick 大于现在"的场景会得到巨大正数（假通过限流）。
    /// </summary>
    [Fact]
    public void Tick_Diff_IsNewMinusOld_NotOldMinusNew()
    {
        Assert.Equal(10u, PlayerSurfaceTick.Tick_Diff(100, 110));      // 新 - 旧
        Assert.Equal(0u, PlayerSurfaceTick.Tick_Diff(110, 110));

        // 旧 > 新（回绕或时钟未更新）→ 无符号回绕成巨大值
        Assert.Equal(uint.MaxValue, PlayerSurfaceTick.Tick_Diff(110, 109));

        // 与"写反"的语义对照：两者在旧<新时也相同，故必须用旧>新的场景区分
        Assert.NotEqual(PlayerSurfaceTick.Tick_Diff(109, 110), PlayerSurfaceTick.Tick_Diff(110, 109));
    }

    // ==================================================================
    // UseMagic（原文 1461-1482）
    // ==================================================================

    /// <summary>白名单命中 → 记录 ID 与 tick（原文 1479-1480）。</summary>
    [Fact]
    public void UseMagic_WhitelistHit_RecordsIdAndTick()
    {
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(55);

        var m = NewManager();
        m.UseMagic(55);

        Assert.Equal((ushort)55, m.LastUseMagicIdForTest);
        Assert.True(m.LastUseMagicTickForTest > 0);   // MyGetTickCount 的当前值（进程 tick > 0）
    }

    /// <summary>
    /// ★ 原文如此（原文 1467-1481）：**未命中白名单的调用完全不留痕** ——
    /// ID 与 tick 都保持原值。这是最容易写错的一处（直觉会"无论如何都记录"）。
    /// </summary>
    [Fact]
    public void UseMagic_WhitelistMiss_LeavesNoTrace_OriginalDefect()
    {
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(55);

        var m = NewManager();
        m.UseMagic(55);                              // 先记录一次
        ushort idAfterHit = m.LastUseMagicIdForTest;
        uint tickAfterHit = m.LastUseMagicTickForTest;

        m.UseMagic(999);                             // 不在白名单 → 不留痕

        Assert.Equal(idAfterHit, m.LastUseMagicIdForTest);
        Assert.Equal(tickAfterHit, m.LastUseMagicTickForTest);
    }

    /// <summary>空白名单：`for I := 0 to Count - 1` 在 Count=0 时是 `0 to -1` → **零次迭代**，不留痕。</summary>
    [Fact]
    public void UseMagic_EmptyWhitelist_ZeroIterations_NoTrace()
    {
        var m = NewManager();
        m.UseMagic(55);
        Assert.Equal((ushort)0, m.LastUseMagicIdForTest);
    }

    /// <summary>
    /// 白名单里**重复项**不影响结果（命中即 `Break`）；且比较是 `Integer` 值比较（原文 1470）。
    /// </summary>
    [Fact]
    public void UseMagic_DuplicateWhitelistEntries_BreakOnFirstHit()
    {
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(7);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(7);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(7);

        var m = NewManager();
        m.UseMagic(7);
        Assert.Equal((ushort)7, m.LastUseMagicIdForTest);
    }

    /// <summary>原文 1449/1470：比较用的是 `Integer(...)`（有符号）—— 0xFFFF 类 ID 不作为负数参与比较。</summary>
    [Fact]
    public void WhWhitelistComparison_IsSignedIntegerValue()
    {
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(65535);   // ushort.MaxValue

        var m = NewManager();
        m.UseMagic(65535);
        Assert.Equal((ushort)65535, m.LastUseMagicIdForTest);
    }

    // ==================================================================
    // 端到端节奏（把三条门串起来）
    // ==================================================================

    /// <summary>
    /// 完整节奏：开限制 + 60s 间隔 + 白名单 {A,B}。
    /// A → A（门 3 放行）→ B（限流拒绝）→ 间隔归零后 B（放行）。
    /// </summary>
    [Fact]
    public void CanUseMagic_FullRhythm_GatesCompose()
    {
        PlayerSurfaceWarrContinueConfig.BoDisableWarrContinueHit = true;
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(10);
        PlayerSurfaceWarrContinueConfig.WarrContinueMagicIdList.Add(20);
        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 60_000;

        var m = NewManager();

        m.UseMagic(10);
        Assert.True(m.CanUseMagic(10));     // 门 3：同技能
        Assert.False(m.CanUseMagic(20));    // 白名单命中 + 间隔不足

        PlayerSurfaceWarrContinueConfig.NWarrContinueHitMinInterval = 0;
        Assert.True(m.CanUseMagic(20));     // 间隔已"过"

        m.UseMagic(20);
        Assert.Equal((ushort)20, m.LastUseMagicIdForTest);
    }
}
