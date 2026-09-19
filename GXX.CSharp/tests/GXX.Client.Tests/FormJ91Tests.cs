using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J91：Actor.pas 时装/护盾特效子过程族 1:1 测试 ——
/// TActor.DrawShieldEffect(5915-5933)、TActor.DrawDressEffect(5974-5987)、
/// THumActor.DrawDressEffect(11260-11304)、DrawDressEffectEx 已注释(5991/11306)。
/// </summary>
public sealed class DressEffectRenderTests
{
    private static List<DressEffectDrawOp> RunAll(
        bool hasHum = false, bool normalDraw = false,
        bool hasHum30 = false, bool normal30 = false,
        bool hasHeroM2 = false, bool heroNoBlend = false,
        bool hasMedal = false, bool medalNoBlend = false,
        bool hasDress = false, bool dressNoBlend = false,
        int ddx = 0, int ddy = 0, int shiftX = 0, int shiftY = 0)
        => DressEffectRender.DrawHumDressEffect(
            ddx, ddy, shiftX, shiftY,
            hasHum, 1, 2, normalDraw,
            hasHum30, 3, 4, normal30,
            hasHeroM2, 5, 6, heroNoBlend,
            hasMedal, 7, 8, medalNoBlend,
            hasDress, 9, 10, dressNoBlend);

    // ===================== 护盾特效门禁（5917 的 or） =====================

    [Fact]
    public void ShieldGateIsOrNotAnd()
    {
        // (not hideWeaponEffect) or (not plugIn) —— 只隐藏、插件可用时**仍然绘制**
        Assert.True(DressEffectRender.ShieldEffectGate(hideWeaponEffectChecked: true, plugInEnabled: false));
        Assert.True(DressEffectRender.ShieldEffectGate(hideWeaponEffectChecked: false, plugInEnabled: true));
        Assert.True(DressEffectRender.ShieldEffectGate(false, false));
    }

    [Fact]
    public void ShieldGateBlocksOnlyWhenBothTrue()
    {
        Assert.False(DressEffectRender.ShieldEffectGate(hideWeaponEffectChecked: true, plugInEnabled: true));
    }

    [Fact]
    public void ShieldEffectBlockedWhenBothFlagsSet()
    {
        var op = DressEffectRender.DrawShieldEffect(
            0, 0, hasShieldEffectSurface: true, shieldEffectDrawNoBlend: false,
            1, 2, 0, 0, hideWeaponEffectChecked: true, plugInEnabled: true);

        Assert.Null(op);
    }

    [Fact]
    public void ShieldEffectDrawnWhenOnlyHidden()
    {
        var op = DressEffectRender.DrawShieldEffect(
            0, 0, true, false, 1, 2, 0, 0,
            hideWeaponEffectChecked: true, plugInEnabled: false);

        Assert.NotNull(op);
    }

    [Fact]
    public void ShieldEffectRequiresSurface()
    {
        var op = DressEffectRender.DrawShieldEffect(
            0, 0, hasShieldEffectSurface: false, shieldEffectDrawNoBlend: false,
            1, 2, 0, 0, false, false);

        Assert.Null(op);
    }

    [Fact]
    public void ShieldEffectNoBlendTrueMeansDraw()
    {
        var op = DressEffectRender.DrawShieldEffect(
            10, 20, true, shieldEffectDrawNoBlend: true, 1, 2, 3, 4, false, false);

        Assert.NotNull(op);
        Assert.False(op!.Blend);                     // NoBlend 为真 → 不混合
        Assert.Equal(10 + 1 + 3, op.X);
        Assert.Equal(20 + 2 + 4, op.Y);
    }

    [Fact]
    public void ShieldEffectNoBlendFalseMeansBlend()
    {
        var op = DressEffectRender.DrawShieldEffect(
            0, 0, true, shieldEffectDrawNoBlend: false, 0, 0, 0, 0, false, false);

        Assert.True(op!.Blend);
    }

    // ===================== 基类 DrawDressEffect（5974） =====================

    [Fact]
    public void BaseDressEffectRequiresSurface()
    {
        Assert.Null(DressEffectRender.DrawBaseDressEffect(
            0, 0, false, false, 0, 0, 0, 0));
    }

    [Fact]
    public void BaseDressEffectNoBlendTrueMeansDraw()
    {
        var op = DressEffectRender.DrawBaseDressEffect(10, 20, true, true, 1, 2, 3, 4);

        Assert.NotNull(op);
        Assert.False(op!.Blend);
        Assert.Equal(10 + 1 + 3, op.X);
        Assert.Equal(20 + 2 + 4, op.Y);
        Assert.Equal("DressEffect", op.Kind);
    }

    [Fact]
    public void BaseDressEffectNoBlendFalseMeansBlend()
    {
        Assert.True(DressEffectRender.DrawBaseDressEffect(0, 0, true, false, 0, 0, 0, 0)!.Blend);
    }

    // ===================== THumActor.DrawDressEffect 四块独立性 =====================

    [Fact]
    public void AllAbsentProducesNoOps()
    {
        Assert.Empty(RunAll());
    }

    [Fact]
    public void EachBlockAloneProducesOneOp()
    {
        Assert.Equal("HumWin", Assert.Single(RunAll(hasHum: true)).Kind);
        Assert.Equal("HumWin30", Assert.Single(RunAll(hasHum30: true)).Kind);
        Assert.Equal("HeroM2Dress", Assert.Single(RunAll(hasHeroM2: true)).Kind);
        Assert.Equal("MedalEffect", Assert.Single(RunAll(hasMedal: true)).Kind);
        Assert.Equal("DressEffect", Assert.Single(RunAll(hasDress: true)).Kind);
    }

    [Fact]
    public void BlocksAreIndependentNotElseChain()
    {
        // 四块同时成立 → 四条 op（外加 inherited 的第五条）
        var ops = RunAll(hasHum: true, hasHum30: true, hasHeroM2: true, hasMedal: true, hasDress: true);
        Assert.Equal(5, ops.Count);
    }

    [Fact]
    public void OrderMatchesSourceOrder()
    {
        var ops = RunAll(hasHum: true, hasHum30: true, hasHeroM2: true, hasMedal: true, hasDress: true);

        Assert.Equal("HumWin", ops[0].Kind);
        Assert.Equal("HumWin30", ops[1].Kind);
        Assert.Equal("HeroM2Dress", ops[2].Kind);
        Assert.Equal("MedalEffect", ops[3].Kind);
        Assert.Equal("DressEffect", ops[4].Kind);     // inherited 在最后
    }

    [Fact]
    public void InheritedBaseBlockComesLast()
    {
        var ops = RunAll(hasDress: true, hasHum: true);
        Assert.Equal("HumWin", ops[0].Kind);
        Assert.Equal("DressEffect", ops[1].Kind);
    }

    // ===================== 两套命名约定的分流方向 =====================

    [Fact]
    public void HumWinNormalDrawTrueMeansDraw()
    {
        // 字段名是 NormalDraw（与 NoBlend 命名相反），但为真同样走 Draw
        var ops = RunAll(hasHum: true, normalDraw: true);
        Assert.False(Assert.Single(ops).Blend);
    }

    [Fact]
    public void HumWinNormalDrawFalseMeansBlend()
    {
        var ops = RunAll(hasHum: true, normalDraw: false);
        Assert.True(Assert.Single(ops).Blend);
    }

    [Fact]
    public void HumWin30NormalDrawTrueMeansDraw()
    {
        var ops = RunAll(hasHum30: true, normal30: true);
        Assert.False(Assert.Single(ops).Blend);
    }

    [Fact]
    public void HumWin30NormalDrawFalseMeansBlend()
    {
        var ops = RunAll(hasHum30: true, normal30: false);
        Assert.True(Assert.Single(ops).Blend);
    }

    [Fact]
    public void HeroM2NoBlendTrueMeansDraw()
    {
        var ops = RunAll(hasHeroM2: true, heroNoBlend: true);
        Assert.False(Assert.Single(ops).Blend);
    }

    [Fact]
    public void HeroM2NoBlendFalseMeansBlend()
    {
        var ops = RunAll(hasHeroM2: true, heroNoBlend: false);
        Assert.True(Assert.Single(ops).Blend);
    }

    [Fact]
    public void MedalNoBlendTrueMeansDraw()
    {
        var ops = RunAll(hasMedal: true, medalNoBlend: true);
        Assert.False(Assert.Single(ops).Blend);
    }

    [Fact]
    public void MedalNoBlendFalseMeansBlend()
    {
        var ops = RunAll(hasMedal: true, medalNoBlend: false);
        Assert.True(Assert.Single(ops).Blend);
    }

    [Fact]
    public void TwoNamingConventionsAgreeOnDirection()
    {
        // NormalDraw 与 NoBlend 命名相反，但"为真 → Draw"一致
        var normalTrue = RunAll(hasHum: true, normalDraw: true);
        var noBlendTrue = RunAll(hasHeroM2: true, heroNoBlend: true);

        Assert.False(normalTrue[0].Blend);
        Assert.False(noBlendTrue[0].Blend);
    }

    // ===================== 各块使用自己的坐标字段 =====================

    [Fact]
    public void EachBlockUsesItsOwnOffsetFields()
    {
        var ops = RunAll(hasHum: true, hasHum30: true, hasHeroM2: true, hasMedal: true, hasDress: true,
            ddx: 100, ddy: 200, shiftX: 1, shiftY: 2);

        Assert.Equal(100 + 1 + 1, ops[0].X);          // spX = 1
        Assert.Equal(200 + 2 + 2, ops[0].Y);          // spY = 2
        Assert.Equal(100 + 3 + 1, ops[1].X);          // spX30 = 3
        Assert.Equal(200 + 4 + 2, ops[1].Y);          // spY30 = 4
        Assert.Equal(100 + 5 + 1, ops[2].X);          // heroM2X = 5
        Assert.Equal(200 + 6 + 2, ops[2].Y);          // heroM2Y = 6
        Assert.Equal(100 + 7 + 1, ops[3].X);          // medalX = 7
        Assert.Equal(200 + 8 + 2, ops[3].Y);          // medalY = 8
        Assert.Equal(100 + 9 + 1, ops[4].X);          // dressEffectX = 9
        Assert.Equal(200 + 10 + 2, ops[4].Y);         // dressEffectY = 10
    }

    [Fact]
    public void ShiftAppliedToAllBlocks()
    {
        var ops = RunAll(hasHum: true, hasDress: true, shiftX: 50, shiftY: 60);
        Assert.Equal(1 + 50, ops[0].X);
        Assert.Equal(2 + 60, ops[0].Y);
        Assert.Equal(9 + 50, ops[1].X);
        Assert.Equal(10 + 60, ops[1].Y);
    }

    // ===================== 已注释代码 =====================

    [Fact]
    public void DrawDressEffectExIsCommentedOut()
    {
        Assert.True(DressEffectRender.DrawDressEffectExIsCommentedOut);
    }

    // ===================== blend / ceff 参数在 Hum 版未被使用 =====================

    [Fact]
    public void HumVersionIgnoresBlendAndColorEffect()
    {
        // 11260 的签名带 blend/ceff，但函数体内从未引用——行为只由各 NoBlend 字段决定
        var a = DressEffectRender.DrawHumDressEffect(
            0, 0, 0, 0,
            true, 1, 2, false,
            false, 0, 0, false,
            false, 0, 0, false,
            false, 0, 0, false,
            false, 0, 0, false);

        // 与调用方传入的 blend 无关：正常混合路径固定由 normalDraw = false 决定
        Assert.True(Assert.Single(a).Blend);
    }
}
