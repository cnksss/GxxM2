// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：**名字族** `MakeHeroName`(2347-2406) + `MakeDeputyHeroName`(2408-2459)
//   及分支 2846-2850 / 2851-2855。
// ★ 这一对是「名字相似 + 看起来一样实则不同」的**双料高危区**：
//   四处差异（普查所得）逐条写差异断言 —— 这里用**同一输入跑两个过程**来取证。
// ⚠ 未覆盖（如实登记）：2373-2393 / 2426-2446 两段"字符过滤"**在 `{ }` 里**，
//   原文从不执行 ⇒ 无可测行为（这也是**不建 `GetNameInFilterList` 接缝**的依据）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectHeroNameTests : IDisposable
{
    private readonly List<string> _labels = new();

    public NpcObjNpcUserSelectHeroNameTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.GotoLable = (npc, p, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
    }

    private static TMerchant M() => new() { m_sCharName = "商人" };

    private static TPlayObject P(string hero = "", string deputy = "", string temp = "", bool waiting = false)
        => new()
        {
            m_sCharName = "玩家", m_sHeroName = hero, m_sDeputyHeroName = deputy,
            m_sTempHeroName = temp, m_boWaitHeroDate = waiting,
        };

    private string Last() => Assert.Single(_labels);
    private void Clear() => _labels.Clear();

    // -----------------------------------------------------------------------
    // 差异①：首判字段不同（但**同一标签**）
    // -----------------------------------------------------------------------

    [Fact]
    public void AlreadyHasHeroName_BothGotoHaveHero()
    {
        var a = P(hero: "已有英雄");
        M().MakeHeroName(a, "@@CreateHero", "新名");
        Assert.Equal("@HaveHero", Last());

        Clear();
        var b = P(deputy: "已有副将");
        M().MakeDeputyHeroName(b, "@@BuHero", "新名");
        Assert.Equal("@HaveHero", Last());
    }

    [Fact]
    public void AlreadyHasHeroName_DoesNotClearIt()
    {
        var a = P(hero: "已有英雄");
        M().MakeHeroName(a, "@@CreateHero", "新名");
        Assert.Equal("已有英雄", a.m_sHeroName);
        Assert.Equal("", a.m_sTempHeroName);
    }

    // -----------------------------------------------------------------------
    // ★★ 差异②：2359 的"创建中"门**少半个条件**（本族最关键的一条）
    // -----------------------------------------------------------------------

    [Fact]
    public void TempNameSetButNotWaiting_MakeHeroName_DoesNotGotoCreatingHero()
    {
        // ★★ 同一输入（`m_sTempHeroName` 非空、`m_boWaitHeroDate = false`）跑两个过程：
        //    `MakeHeroName` 的 2359 前半被 `{ }` 注释掉 ⇒ **不进** `@CreateingHero`；
        //    `MakeDeputyHeroName` 的 2419 两条件都活 ⇒ **进** `@CreateingHero`。
        var a = P(temp: "暂存名");
        M().MakeHeroName(a, "@@CreateHero", "新名");
        Assert.NotEqual("@CreateingHero", Last());          // ★ 不跳 CreateingHero

        Clear();
        var b = P(temp: "暂存名");
        M().MakeDeputyHeroName(b, "@@BuHero", "新名");
        Assert.Equal("@CreateingHero", Last());             // ★ 跳 CreateingHero
    }

    [Fact]
    public void WaitingFlag_MakeHeroName_GotoCreatingHero()
    {
        var a = P(waiting: true);
        M().MakeHeroName(a, "@@CreateHero", "新名");
        Assert.Equal("@CreateingHero", Last());
    }

    [Fact]
    public void WaitingFlag_MakeDeputyHeroName_GotoCreatingHero()
    {
        var b = P(waiting: true);
        M().MakeDeputyHeroName(b, "@@BuHero", "新名");
        Assert.Equal("@CreateingHero", Last());
    }

    [Fact]
    public void CreatingHeroPath_ExitsBeforeAnyLengthCheck()
    {
        // 2421-2422 / 2361-2362：`@CreateingHero` 后立即 Exit ⇒ **不改** `m_sTempHeroName`
        var a = P(waiting: true, temp: "旧暂存");
        M().MakeHeroName(a, "@@CreateHero", "新名");
        Assert.Equal("旧暂存", a.m_sTempHeroName);
    }

    // -----------------------------------------------------------------------
    // ★★ 差异③：2365-2369"长度 ≥ 4"门**只在 MakeHeroName**
    // -----------------------------------------------------------------------

    [Fact]
    public void ThreeCharName_MakeHeroName_GotoSetHeroName()
    {
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", "abc");          // 3 < 4
        Assert.Equal("@SetHeroName", Last());
        Assert.Equal("", a.m_sTempHeroName);
    }

    [Fact]
    public void ThreeCharName_MakeDeputyHeroName_Succeeds()
    {
        // ★★ 同一输入：副将版**没有**那道门 ⇒ 直接成功（走 2424 的区间门）
        var b = P();
        M().MakeDeputyHeroName(b, "@@BuHero", "abc");
        Assert.NotEqual("@SetHeroName", Last());
        Assert.Equal("abc", b.m_sTempHeroName);
        Assert.Equal("@BuHero", Last());                     // Copy(sLabel, 2, Len-1) 去掉第 1 个 '@'
    }

    [Fact]
    public void FourCharName_MakeHeroName_PassesTheGate()
    {
        // 边界：恰好 4 个字符 >= 4 → 过门
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", "abcd");
        Assert.Equal("@CreateHero", Last());
        Assert.Equal("abcd", a.m_sTempHeroName);
    }

    [Fact]
    public void EmptyName_MakeHeroName_GotoSetHeroName_NotHeroNameFilter()
    {
        // 0 < 4 ⇒ 走"太短"门（**不是** @HeroNameFilter）
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", "");
        Assert.Equal("@SetHeroName", Last());
    }

    [Fact]
    public void EmptyName_MakeDeputyHeroName_GotoHeroNameFilter()
    {
        // ★ 同一输入：副将版没有"太短"门 ⇒ 落进 2424 的 `(Length > 0)` 为假 → else → @HeroNameFilter
        var b = P();
        M().MakeDeputyHeroName(b, "@@BuHero", "");
        Assert.Equal("@HeroNameFilter", Last());
    }

    // -----------------------------------------------------------------------
    // 2370/2424 区间门：`> 0 and < 15`
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(4, "@CreateHero")]       // ★ 下限：恰好 4（`>= 4` 门刚过）
    [InlineData(14, "@CreateHero")]      // 上限内侧
    public void MakeHeroName_WithinRange_Succeeds(int len, string expectedLabel)
    {
        // ⚠ 注意期望标签是 `Copy("@@CreateHero", 2, Len-1)` = `"@CreateHero"`（**只剩一个 `@`**）
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", new string('x', len));
        Assert.Equal(expectedLabel, Last());
        Assert.Equal(new string('x', len), a.m_sTempHeroName);
    }

    [Fact]
    public void MakeHeroName_FifteenChars_GotoHeroNameFilter()
    {
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", new string('x', 15));
        Assert.Equal("@HeroNameFilter", Last());
        Assert.Equal("", a.m_sTempHeroName);
    }

    [Fact]
    public void MakeDeputyHeroName_FifteenChars_GotoHeroNameFilter()
    {
        var b = P();
        M().MakeDeputyHeroName(b, "@@BuHero", new string('x', 15));
        Assert.Equal("@HeroNameFilter", Last());
    }

    // -----------------------------------------------------------------------
    // ★★ 差异④：else 分支清的字段不同
    // -----------------------------------------------------------------------

    [Fact]
    public void TooLong_MakeHeroName_ClearsHeroNameNotDeputyName()
    {
        var a = P(hero: "", deputy: "副将名");
        M().MakeHeroName(a, "@@CreateHero", new string('x', 15));
        Assert.Equal("", a.m_sHeroName);                     // 被清（本来就是空）
        Assert.Equal("副将名", a.m_sDeputyHeroName);          // ★ 未被触碰
    }

    [Fact]
    public void TooLong_MakeDeputyHeroName_ClearsDeputyNameNotHeroName()
    {
        var b = P(hero: "英雄名", deputy: "");
        M().MakeDeputyHeroName(b, "@@BuHero", new string('x', 15));
        Assert.Equal("", b.m_sDeputyHeroName);               // 被清
        Assert.Equal("英雄名", b.m_sHeroName);                // ★ 未被触碰
    }

    [Fact]
    public void BothElseBranches_ClearTempHeroName()
    {
        // ⚠ 前提：`m_sTempHeroName` 必须**为空**，否则副将版会在 2419 提前 Exit（差异②）——
        //   故这里用 `temp: ""`，只考验"else 分支会 clear"这一点。
        var a = P(temp: "");
        M().MakeHeroName(a, "@@CreateHero", new string('x', 15));
        Assert.Equal("", a.m_sTempHeroName);
        Assert.Equal("@HeroNameFilter", Last());

        Clear();
        var b = P(temp: "");
        M().MakeDeputyHeroName(b, "@@BuHero", new string('x', 15));
        Assert.Equal("", b.m_sTempHeroName);
        Assert.Equal("@HeroNameFilter", Last());
    }

    [Fact]
    public void MakeDeputyHeroName_WithTempName_ExitsAtCreatingHero_InsteadOfClearing()
    {
        // ★★ 差异②的**副作用**（我写上一版用例时踩到）：副将版只要 `m_sTempHeroName` 非空，
        //    就在 2419 提前 Exit ⇒ **连 15 字符的超长名也不会走到 else 分支、不会清 `m_sTempHeroName`**；
        //    而 `MakeHeroName` 在同样输入下**会**走到 else 并清空（因为它的 2359 只判 `m_boWaitHeroDate`）。
        var a = P(temp: "暂存");
        M().MakeHeroName(a, "@@CreateHero", new string('x', 15));
        Assert.Equal("", a.m_sTempHeroName);                 // 被清
        Assert.Equal("@HeroNameFilter", Last());

        Clear();
        var b = P(temp: "暂存");
        M().MakeDeputyHeroName(b, "@@BuHero", new string('x', 15));
        Assert.Equal("暂存", b.m_sTempHeroName);              // ★ 未被清（提前 Exit）
        Assert.Equal("@CreateingHero", Last());
    }

    [Fact]
    public void SuccessLabel_IsLabelMinusFirstChar_ForBoth()
    {
        var a = P();
        M().MakeHeroName(a, "@@CreateHero", "abcd");
        Assert.Equal("@CreateHero", Last());

        Clear();
        var b = P();
        M().MakeDeputyHeroName(b, "@@BuHero", "abcd");
        Assert.Equal("@BuHero", Last());
    }

    // -----------------------------------------------------------------------
    // 派发分支（2846-2850 / 2851-2855）—— ★ 两个守卫名字都"不像"
    // -----------------------------------------------------------------------

    [Fact]
    public void CreateHeroArm_GuardOn_Dispatches()
    {
        var m = M();
        m.m_boCreateHeroName = true;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateHero, "abcd", "@@CreateHero"));
        Assert.Equal("@CreateHero", Last());
    }

    [Fact]
    public void CreateHeroArm_GuardOff_DoesNothing()
    {
        var m = M();
        m.m_boCreateHeroName = false;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateHero, "abcd", "@@CreateHero"));
        Assert.Empty(_labels);
    }

    [Fact]
    public void CreateDeputyArm_GuardIsBuHero_NotCreateDeputy()
    {
        // ★ "名字相似"差异断言：命令号 `nNF_CreateDeputy`、判定串 `'@@BuHero'`、守卫 `m_boBuHero`
        //   三者名字各不相同。把 `m_boCreateHeroName` 打开、`m_boBuHero` 关闭 ⇒ **不发**
        var m = M();
        m.m_boCreateHeroName = true;
        m.m_boBuHero = false;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateDeputy, "abcd", "@@BuHero"));
        Assert.Empty(_labels);                               // 只认 m_boBuHero
    }

    [Fact]
    public void CreateDeputyArm_GuardOn_Dispatches()
    {
        var m = M();
        m.m_boBuHero = true;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateDeputy, "abcd", "@@BuHero"));
        Assert.Equal("@BuHero", Last());
    }

    [Fact]
    public void TwoArmsGuardsAreIndependent()
    {
        // ★ 唯一变量对照：同一命令号，只换守卫
        var m = M();
        m.m_boBuHero = true;
        m.m_boCreateHeroName = false;
        m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateHero, "abcd", "@@CreateHero");
        Assert.Empty(_labels);                               // CreateHero 分支不放行
        m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_CreateDeputy, "abcd", "@@BuHero");
        Assert.Single(_labels);
    }

    [Fact]
    public void CommandIdsAreOriginal()
    {
        // NpcCommon.pas:18/20
        Assert.Equal(5, NpcProcessCmd.nNF_CreateHero);
        Assert.Equal(6, NpcProcessCmd.nNF_CreateDeputy);
    }
}
