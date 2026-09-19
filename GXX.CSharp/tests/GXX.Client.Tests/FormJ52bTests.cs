using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>PlayScn.pas 掉落物特效族（批次J52）。</summary>
public sealed class DropItemFxTests
{
    private static DropItemEffectDef Eff(int fileIndex = 3, int startIndex = 4, int time = 100, int imageCount = 3,
        bool drawCenter = false, bool noBlend = false, bool belowItem = false, int offX = 1, int offY = 2)
        => new()
        {
            ItemEffectIndex = 7,
            FileIndex = fileIndex,
            StartIndex = startIndex,
            Time = time,
            ImageCount = imageCount,
            DrawCenter = drawCenter,
            NoBlend = noBlend,
            BelowItem = belowItem,
            OffsetX = offX,
            OffsetY = offY,
        };

    [Fact]
    public void FlashInterval_SpecialGate()
    {
        Assert.Equal(300u, DropItemFx.FlashInterval(quickFlashChecked: true, showSpecial: true));
        Assert.Equal(5000u, DropItemFx.FlashInterval(quickFlashChecked: true, showSpecial: false));
        Assert.Equal(5000u, DropItemFx.FlashInterval(quickFlashChecked: false, showSpecial: true));
    }

    [Fact]
    public void UpdateFlash_WindowThenTenStepsOff()
    {
        var st = new DropItemFxState { FlashTime = 0 };
        DropItemFx.UpdateFlash(st, 5000, 5000); // 5000-0 > 5000? 否（严格大于）
        Assert.False(st.ShowFlash);

        DropItemFx.UpdateFlash(st, 5001, 5000); // 窗口到 → 开闪
        Assert.True(st.ShowFlash);
        Assert.Equal(0, st.FlashStep);

        uint t = 5001;
        for (int i = 1; i <= 9; i++)
        {
            t += 20;
            DropItemFx.UpdateFlash(st, t, 5000);
            Assert.True(st.ShowFlash); // 步进 1..9 保持
            Assert.Equal(i, st.FlashStep);
        }

        t += 20;
        DropItemFx.UpdateFlash(st, t, 5000); // 第 10 步 → 关闪
        Assert.False(st.ShowFlash);
    }

    [Fact]
    public void PlanItemEffect_AllGuards()
    {
        var st = new DropItemFxState { ItemEffectFrame = 4, ItemEffectTick = 0 };
        FxImage? Resolve(int i) => new(i, 20, 10, 3, 5, false);

        Assert.Null(DropItemFx.PlanItemEffect(Eff(), st, 0, hideItemEffectChecked: true, hideItemEffect: true, false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(fileIndex: -1), st, 0, false, false, false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(fileIndex: 10), st, 0, false, false, false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(imageCount: 0), st, 0, false, false, false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(belowItem: true), st, 0, false, false, isBelowItem: false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(time: 0), st, 0, false, false, false, false, 48, 48, 0, 0, 10, Resolve));
        Assert.Null(DropItemFx.PlanItemEffect(Eff(), st, 0, false, false, false, false, 48, 48, 0, 0, 10, _ => null));
    }

    [Fact]
    public void PlanItemEffect_FrameAdvanceAndWrap()
    {
        var st = new DropItemFxState { ItemEffectFrame = 4, ItemEffectTick = 0 };
        var eff = Eff(startIndex: 4, time: 100, imageCount: 3);
        FxImage? Resolve(int idx) => new(idx, 20, 10, 0, 0, false);

        var op = DropItemFx.PlanItemEffect(eff, st, 100, false, false, false, false, 48, 48, 10, 10, 10, Resolve);
        Assert.NotNull(op);
        Assert.Equal(5, st.ItemEffectFrame); // 100-0 ≥ 100 → 推进
        Assert.Equal(100u, st.ItemEffectTick);
        Assert.Equal(5, op!.Image.ImageIndex);

        DropItemFx.PlanItemEffect(eff, st, 150, false, false, false, false, 48, 48, 10, 10, 10, Resolve);
        Assert.Equal(5, st.ItemEffectFrame); // 50 < 100 → 不推进

        st.ItemEffectFrame = 6; // StartIndex+ImageCount-1
        DropItemFx.PlanItemEffect(eff, st, 300, false, false, false, false, 48, 48, 10, 10, 10, Resolve);
        Assert.Equal(4, st.ItemEffectFrame); // 推进 7 ≥ 4+3 → 回卷 StartIndex
    }

    [Fact]
    public void PlanItemEffect_CenterVsOrigin()
    {
        var st = new DropItemFxState { ItemEffectFrame = 4, ItemEffectTick = 0 };
        FxImage? Resolve(int i) => new(i, 20, 10, 3, 5, false);

        // DrawCenter：nX + OffX + (texW − w) div 2
        var center = DropItemFx.PlanItemEffect(Eff(drawCenter: true, offX: 1, offY: 2), st, 0, false, false, false, false, 48, 48, 100, 200, 10, Resolve);
        Assert.Equal(100 + 1 + (48 - 20) / 2, center!.X);
        Assert.Equal(200 + 2 + (48 - 10) / 2, center.Y);
        Assert.True(center.Blend); // 默认非 NoBlend → DrawBlend

        // 原点：nX + OffX + oX
        var origin = DropItemFx.PlanItemEffect(Eff(drawCenter: false, offX: 1, offY: 2), st, 0, false, false, false, false, 48, 48, 100, 200, 10, Resolve);
        Assert.Equal(100 + 1 + 3, origin!.X);
        Assert.Equal(200 + 2 + 5, origin.Y);

        // NoBlend → Blend=false
        var noBlend = DropItemFx.PlanItemEffect(Eff(noBlend: true), st, 0, false, false, false, false, 48, 48, 0, 0, 10, Resolve);
        Assert.False(noBlend!.Blend);
    }

    [Fact]
    public void PlanValueItemEffect_GatesInOrder()
    {
        var st = new DropItemFxState();
        FxImage? Resolve(int i) => new(i, 16, 16, 2, 4, false);

        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, showDropValueItemEff: false, true, true, 6, 100, 1190, 1, 1, true, false, 0, 0, Resolve));
        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, true, showValueItemEffectChecked: false, true, 6, 100, 1190, 1, 1, true, false, 0, 0, Resolve));
        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, true, true, showValueItemEffect: false, 6, 100, 1190, 1, 1, true, false, 0, 0, Resolve));
        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, true, true, true, valueItemCount: 0, 100, 1190, 1, 1, true, false, 0, 0, Resolve));
        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, true, true, true, 6, valueItemPlayTime: 0, 1190, 1, 1, true, false, 0, 0, Resolve));
        Assert.Null(DropItemFx.PlanValueItemEffect(st, 0, true, true, true, 6, 100, 1190, 1, 1, boValueItem: false, false, 0, 0, Resolve));
    }

    [Fact]
    public void PlanValueItemEffect_FrameWrapAndIndex()
    {
        var st = new DropItemFxState { ValueItemEffectFrame = 5, ValueItemEffectTick = 0 };
        FxImage? Resolve(int idx) => new(idx, 16, 16, 2, 4, false);

        var op = DropItemFx.PlanValueItemEffect(st, 100, true, true, true, 6, 100, 1190, 1, 1, true, false, 50, 60, Resolve);
        Assert.Equal(0, st.ValueItemEffectFrame); // 5→6 ≥ Count 6 → 回卷 0
        Assert.Equal(1190, op!.Image.ImageIndex); // 回卷 0 → 0 + 1190（锁定特效 index）
        Assert.Equal(50 + 1 + 2, op.X);
        Assert.Equal(60 + 1 + 4, op.Y);
        Assert.False(op.Blend); // 恒 Draw 无混合
    }

    [Fact]
    public void NamePos_CenterAbove()
    {
        var (x, y) = DropItemFx.NamePos(100, 50, 40, 8);
        Assert.Equal(100 + 24 - 40 / 2, x);
        Assert.Equal(50 + 16 - 8 * 2, y);
    }
}

/// <summary>TDropItemsMgr headless 镜像（特效绑定/点表/删除）。</summary>
public sealed class DropItemsStoreTests
{
    [Fact]
    public void AddDropItem_EffectBindingAndHeadInsert()
    {
        uint now = 0;
        var store = new DropItemsStore(
            idx => idx == 7 ? new DropItemEffectDef { ItemEffectIndex = 7, FileIndex = 3, StartIndex = 4, Time = 100, ImageCount = 3 } : null,
            () => now);

        var item = store.AddDropItem(1, 10, 20, "屠龙刀", 7);
        Assert.Equal(3, item.ItemEffect.FileIndex);
        Assert.Equal(4, item.Fx.ItemEffectFrame); // 帧置 StartIndex
        Assert.Equal(now, item.Fx.FlashTime);

        var noEff = store.AddDropItem(2, 10, 20, "木剑", 0);
        Assert.Equal(-1, noEff.ItemEffect.FileIndex); // EffectIndex=0 → 无特效

        var list = store.GetPointList(10, 20)!;
        Assert.Equal(2, list.Items.Count);
        Assert.Same(item, list.Items[0]); // 特效命中 → 头部插入

        var missing = store.AddDropItem(3, 10, 20, "金创药", 9); // 特效表无 9
        Assert.Equal(-1, missing.ItemEffect.FileIndex);
        Assert.Same(noEff, list.Items[1]); // 无特效 → 尾插
    }

    [Fact]
    public void DelDropItem_VisibleFalseAndRemoved()
    {
        var store = new DropItemsStore(_ => null);
        store.AddDropItem(1, 10, 20, "木剑", 0);
        var removed = store.DelDropItem(1);
        Assert.NotNull(removed);
        Assert.False(removed!.Visible);
        Assert.False(store.ById.ContainsKey(1));
        Assert.Null(store.GetPointList(10, 20)); // 空点表移除
        Assert.Null(store.DelDropItem(99));
    }

    [Fact]
    public void Points_SortedByYThenX()
    {
        var store = new DropItemsStore(_ => null);
        store.AddDropItem(1, 10, 20, "a", 0);
        store.AddDropItem(2, 5, 1, "b", 0);
        Assert.Equal(5, store.Points[0].PointX); // (5,1) key 小 → 在前
        Assert.Equal(1, store.Points[0].PointY);
        Assert.Equal(10, store.Points[1].PointX);
    }
}

/// <summary>THumActor.LoadSurface 余支（批次J52）：马上人/马上发/简化武器/连击 DIY 翅膀。</summary>
public sealed class HumSurfaceRemainderTests
{
    [Fact]
    public void HorseHum_Segments()
    {
        var base0 = HumSurfacePlan.HorseHum(0, 1, expanded: false, currentFrame: 9);
        Assert.Equal(HumLib.HorseHum, base0.Lib);
        Assert.Equal(600 * (1 + 0 * 2) + 9, base0.ImageIndex); // expand=0 → (sex, 每马 2)

        var seg50 = HumSurfacePlan.HorseHum(55, 1, false, 3);
        Assert.Equal(HumLib.HorseHum1, seg50.Lib);
        Assert.Equal(600 * (1 + 5 * 2) + 3, seg50.ImageIndex);

        var seg300 = HumSurfacePlan.HorseHum(320, 0, expanded: true, currentFrame: 7);
        Assert.Equal(HumLib.HorseHum6, seg300.Lib);
        Assert.Equal(600 * (0 + 20 * 1) + 7, seg300.ImageIndex); // expand → (0, 1)

        var seg120 = HumSurfacePlan.HorseHum(120, 1, false, 0);
        Assert.Equal(HumLib.HorseHum2, seg120.Lib);
        Assert.Equal(600 * (1 + 20 * 2), seg120.ImageIndex);
    }

    [Fact]
    public void HorseHair_Formula()
    {
        var h = HumSurfacePlan.HorseHair(1, 3, 5);
        Assert.Equal(HumLib.HorseHair, h.Lib);
        Assert.Equal(600 * (1 + 3 * 2) + 5, h.ImageIndex);
    }

    [Fact]
    public void WeaponSimple_JobShapes()
    {
        var warrior = HumSurfacePlan.WeaponSimple(0, 0, 9);
        Assert.Equal(HumLib.CboWeapon, warrior.Lib);
        Assert.Equal(0 * 2000, warrior.FileIndex);
        Assert.Equal(24 * 2 * 2000 + 9, warrior.ImageIndex);

        var taoist = HumSurfacePlan.WeaponSimple(1, 0, 0);
        Assert.Equal(28 * 2 * 2000, taoist.ImageIndex); // 骨玉权杖 shape=28

        var unknownJob = HumSurfacePlan.WeaponSimple(3, 1, 5);
        Assert.Equal((24 * 2 + 1) * 2000 + 5, unknownJob.ImageIndex); // else → 24
    }

    [Fact]
    public void WeaponSimple_CustomShapes()
    {
        var custom = HumSurfacePlan.WeaponSimple(2, 0, 5, custom: true, customShapes: new[] { 30, 31, 32 });
        Assert.Equal(32 * 2 * 2000 + 5, custom.ImageIndex); // arr[job=2]
        var fallback = HumSurfacePlan.WeaponSimple(4, 0, 5, custom: true, customShapes: new[] { 30, 31, 32 });
        Assert.Equal(30 * 2 * 2000 + 5, fallback.ImageIndex); // else → arr[0]
    }

    [Fact]
    public void WingsDiY_GateAndFormula()
    {
        var diy = HumSurfacePlan.WingsDiY(65, 1, 100);
        Assert.Equal(HumLib.HumEffectDiY, diy.Lib);
        Assert.Equal(2, diy.FileIndex); // (65-1)/30
        Assert.Equal(4 * 4000 + 1 * 2000 + 100, diy.ImageIndex);

        var outOfRange = HumSurfacePlan.WingsDiY(125, 0, 0); // file=4 ≥ CBOHUMDIYFILE_COUNT(4)
        Assert.Equal(HumLib.None, outOfRange.Lib);
    }
}
