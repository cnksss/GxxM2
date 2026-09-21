// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：`TNormNpc.UserSelect`（原文 **9807-9835，30 行**）
//   —— `UserSelect` 虚分派链的**基类落点**（原文 :305 `virtual`；4 个覆写点：
//      TMerchant 2087 / TGuildOfficial 10101 / TCastleOfficial 1186 + 声明区 411/444/481）。
// 另外覆盖本轮新增接缝 `NpcSeams.GetCastleUnderWar`（偏差 D37，默认抛异常）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;
using TNormNpc = GXX.M2Server.Npc.TNormNpc;   // ★ 命名仲裁：全仓两处同名（Engine 与 Npc），本文件明确取 Npc 侧

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcNormUserSelectTests : IDisposable
{
    private readonly List<string> _labels = new();

    public NpcObjNpcNormUserSelectTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.GotoLable = (npc, player, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
    }

    private static TNormNpc Npc() => new() { m_sCharName = "NPC" };

    private static TPlayObject Player(int gotoCount = 9, string curr = "", string back = "")
        => new() { m_sCharName = "玩家", m_nScriptGotoCount = gotoCount,
                   m_sScriptCurrLable = curr, m_sScriptGoBackLable = back };

    // -----------------------------------------------------------------------
    // 虚分派链（★ 基类必须是 virtual，否则 4 处覆写的 inherited 全落空）
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelect_IsVirtual()
    {
        var m = typeof(TNormNpc).GetMethod("UserSelect");
        Assert.NotNull(m);
        Assert.True(m!.IsVirtual, "TNormNpc.UserSelect 必须是 virtual（原文 ObjBase/ObjNpc.pas:305）");
    }

    [Fact]
    public void UserSelect_IsOverridableViaBaseCall()
    {
        // 派生类覆写并调 base → 必须真的走到基类体（"只写外壳不接 base"是完全无效果的）
        var probe = new ProbeNpc();
        probe.UserSelect(Player(gotoCount: 9), "@foo");
        Assert.True(probe.BaseCalled, "派生类的 base.UserSelect 必须落到基类体");
    }

    private sealed class ProbeNpc : TNormNpc
    {
        public bool BaseCalled;
        public override void UserSelect(TPlayObject PlayObject, string sData)
        {
            base.UserSelect(PlayObject, sData);
            BaseCalled = true;
        }
    }

    // -----------------------------------------------------------------------
    // 9811：m_nScriptGotoCount 归零
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelect_AlwaysResetsScriptGotoCount()
    {
        var p = Player(gotoCount: 9);
        Npc().UserSelect(p, "@foo");
        Assert.Equal(0, p.m_nScriptGotoCount);
    }

    [Fact]
    public void UserSelect_ResetsGotoCount_EvenWhenDataNotLabel()
    {
        // 9811 在 9814 的判定**之前**，故非标签串也归零
        var p = Player(gotoCount: 9);
        Npc().UserSelect(p, "not_a_label");
        Assert.Equal(0, p.m_nScriptGotoCount);
    }

    [Fact]
    public void UserSelect_EmptyData_DoesNotTouchLabelStack()
    {
        // 9814：`(sData <> '')` 先判空 → 空串直接跳过
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "");
        Assert.Equal("C", p.m_sScriptCurrLable);
        Assert.Equal("B", p.m_sScriptGoBackLable);
    }

    [Fact]
    public void UserSelect_NonAtData_DoesNotTouchLabelStack()
    {
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "foo");
        Assert.Equal("C", p.m_sScriptCurrLable);
        Assert.Equal("B", p.m_sScriptGoBackLable);
    }

    // -----------------------------------------------------------------------
    // 9816-9825：标签入栈
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelect_NewLabel_PushesStackInOriginalOrder()
    {
        // 9823-9824：先 `GoBackLable := 旧 CurrLable`，再 `CurrLable := sLabel`
        var p = Player(curr: "旧", back: "更旧");
        Npc().UserSelect(p, "@新");
        Assert.Equal("@新", p.m_sScriptCurrLable);
        Assert.Equal("旧", p.m_sScriptGoBackLable);   // 旧值被搬进 GoBack
    }

    [Fact]
    public void UserSelect_SameLabel_DoesNotPush()
    {
        // 9819：`m_sScriptCurrLable <> sLabel` 才动栈 → 同标签重复点击不入栈
        var p = Player(curr: "@同", back: "B");
        Npc().UserSelect(p, "@同");
        Assert.Equal("@同", p.m_sScriptCurrLable);
        Assert.Equal("B", p.m_sScriptGoBackLable);    // 未被覆盖
    }

    [Fact]
    public void UserSelect_TrailingSegmentAfterCr_IsDiscarded()
    {
        // 9816：`GetValidStr3_Ex(sData, sLabel, #13)` 取 #13 前的第一段作为 sLabel
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "@第一段\r剩下的");
        Assert.Equal("@第一段", p.m_sScriptCurrLable);
    }

    // -----------------------------------------------------------------------
    // 9826-9831：@back 只清一层
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelect_BackWithNonEmptyCurr_ClearsCurrOnly()
    {
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "@back");
        Assert.Equal("", p.m_sScriptCurrLable);
        Assert.Equal("B", p.m_sScriptGoBackLable);    // GoBack 保留
    }

    [Fact]
    public void UserSelect_BackWithEmptyCurr_ClearsGoBack()
    {
        // 9828-9831 的 else 分支
        var p = Player(curr: "", back: "B");
        Npc().UserSelect(p, "@back");
        Assert.Equal("", p.m_sScriptCurrLable);
        Assert.Equal("", p.m_sScriptGoBackLable);
    }

    [Fact]
    public void UserSelect_BackIsCaseInsensitive_LikeDelphiSameText()
    {
        // 9821 用 `SameText` → 不区分大小写
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "@BACK");
        Assert.Equal("", p.m_sScriptCurrLable);
    }

    [Fact]
    public void UserSelect_Back_WhenCurrEqualsBack_IsSkippedByStackGuard()
    {
        // ★ 差异断言：9819 的"同标签不入栈"守卫在 9821 之**先** ——
        //   若 CurrLable 恰好就是 `@back`，9821 那套清栈逻辑**根本不执行**
        var p = Player(curr: "@back", back: "B");
        Npc().UserSelect(p, "@back");
        Assert.Equal("@back", p.m_sScriptCurrLable);   // 未被清空
        Assert.Equal("B", p.m_sScriptGoBackLable);
    }

    // -----------------------------------------------------------------------
    // 9817-9818：@HeroMap 特殊直跳（不走标签栈）
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelect_HeroMap_GotoLableDirectly_WithoutTouchingStack()
    {
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "@HeroMap");
        Assert.Equal("@HeroMap", Assert.Single(_labels));   // 直跳
        Assert.Equal("C", p.m_sScriptCurrLable);            // 栈未动
        Assert.Equal("B", p.m_sScriptGoBackLable);
    }

    [Fact]
    public void UserSelect_HeroMapWithTrailingSegment_StillDirectGoto()
    {
        // `GetValidStr3_Ex` 先取第一段 → 仍是 @HeroMap
        var p = Player(curr: "C", back: "B");
        Npc().UserSelect(p, "@HeroMap\rxyz");
        Assert.Equal("@HeroMap", Assert.Single(_labels));
        Assert.Equal("C", p.m_sScriptCurrLable);
    }

    // -----------------------------------------------------------------------
    // 接缝 D37：GetCastleUnderWar 默认**抛异常**（不静默返回 false）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetCastleUnderWar_DefaultThrows_NotSilentFalse()
    {
        NpcSeams.ResetDefaults();
        var ex = Assert.Throws<NotSupportedException>(() => NpcSeams.GetCastleUnderWar(new object()));
        Assert.Contains("m_boUnderWar", ex.Message);
    }

    [Fact]
    public void GetCastleUnderWar_AfterWiring_ReturnsInjectedValue()
    {
        NpcSeams.GetCastleUnderWar = _ => true;
        Assert.True(NpcSeams.GetCastleUnderWar(new object()));
    }
}
