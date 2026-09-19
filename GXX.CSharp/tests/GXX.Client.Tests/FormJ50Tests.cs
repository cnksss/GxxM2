using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J50：THumActor 绘制核心（WORDER 武器前后层排序 + DrawChr 分层序列）1:1 测试。</summary>
public sealed class HumActorDrawTests
{
    private static SceneComposer.SceneActorInput Noop() => new() { RecogId = 0, Rx = 0, Ry = 0 };

    [Fact]
    public void Worder_Table_MachineExtraction()
    {
        // 机器提取锁定：男 站立帧 0..7（ActStand start=0）全 0 = 武器在身体后
        for (int f = 0; f < 8; f++)
            Assert.Equal(0, HumActorDrawTables.WorderMale[f]);
        // 男 攻击帧 200 起（ActHit start=200）：前几帧武器在前
        Assert.Equal(1, HumActorDrawTables.WorderMale[200]);
        Assert.Equal(1, HumActorDrawTables.WorderMale[201]);
        // 表长度 600
        Assert.Equal(600, HumActorDrawTables.WorderMale.Length);
        Assert.Equal(600, HumActorDrawTables.WorderFemale.Length);
        // 男女 war模式（frame 192..195）差异：男首帧 0，女首帧 1
        Assert.Equal(0, HumActorDrawTables.WorderMale[192]);
        Assert.Equal(1, HumActorDrawTables.WorderFemale[192]);
    }

    [Fact]
    public void WeaponOrder_SexAndFrameLookup()
    {
        Assert.Equal(0, HumActorDrawPlanner.WeaponOrder(0, 0));    // 男站立：武器在后
        Assert.Equal(1, HumActorDrawPlanner.WeaponOrder(0, 200));  // 男攻击：武器在前
        // 女 war模式 frame192 = 1（男为 0）：原文男女差异
        Assert.Equal(1, HumActorDrawPlanner.WeaponOrder(1, 192));
        Assert.Equal(0, HumActorDrawPlanner.WeaponOrder(0, 192));
        // 越界帧（600+）→ 缺省 0（m_nWpord 保持）
        Assert.Equal(0, HumActorDrawPlanner.WeaponOrder(0, 600));
        Assert.Equal(0, HumActorDrawPlanner.WeaponOrder(0, -1));
    }

    [Fact]
    public void PlanLayers_WeaponBehind_ForUpDirections()
    {
        // 男、站立帧 0（Wpord=0）→ 武器在身体后层
        var input = new HumActorDrawPlanner.HumDrawInput
        {
            Sex = 0, CurrentFrame = 0, Dir = 0, Weapon = 5, Shield = 1,
            HasShield = true,
        };
        var layers = HumActorDrawPlanner.PlanLayers(input);
        int bodyIdx = layers.IndexOf(HumDrawLayer.Body);
        int weaponIdx = layers.IndexOf(HumDrawLayer.WeaponBehind);
        Assert.True(weaponIdx >= 0);
        Assert.True(weaponIdx < bodyIdx); // 武器先绘（身体后）
    }

    [Fact]
    public void PlanLayers_WeaponFront_ForAttackFrames()
    {
        // 男攻击帧 200（Wpord=1）→ 武器在身体前层
        var input = new HumActorDrawPlanner.HumDrawInput
        {
            Sex = 0, CurrentFrame = 200, Dir = 4, Weapon = 5, HasWeapon = true, Shield = 1, HasShield = true,
        };
        var layers = HumActorDrawPlanner.PlanLayers(input);
        int bodyIdx = layers.IndexOf(HumDrawLayer.Body);
        int weaponFrontIdx = layers.IndexOf(HumDrawLayer.WeaponFront);
        Assert.True(weaponFrontIdx >= 0);
        Assert.True(weaponFrontIdx > bodyIdx); // 武器后绘（身体前）
        Assert.DoesNotContain(HumDrawLayer.WeaponBehind, layers);
        // 盾牌：Wpord=1 且 dir=5? dir=4 → 盾在身体后层
        Assert.Contains(HumDrawLayer.ShieldBehind, layers);
        Assert.DoesNotContain(HumDrawLayer.ShieldFront, layers);
    }

    [Fact]
    public void PlanLayers_ShieldFront_Dir5_WithWpord1()
    {
        // Wpord=1 且 dir=5（左下）→ 盾牌透明绘制在人前面
        var input = new HumActorDrawPlanner.HumDrawInput
        {
            Sex = 0, CurrentFrame = 200, Dir = 5, Weapon = 5, Shield = 1, HasShield = true, HasWeapon = true,
        };
        var layers = HumActorDrawPlanner.PlanLayers(input);
        int bodyIdx = layers.IndexOf(HumDrawLayer.Body);
        int shieldFrontIdx = layers.IndexOf(HumDrawLayer.ShieldFront);
        Assert.Contains(HumDrawLayer.ShieldFront, layers);
        Assert.True(shieldFrontIdx > bodyIdx);
    }

    [Fact]
    public void PlanLayers_FullOrder_HorseAndDress()
    {
        // 女角色、攻击帧、带坐骑/发型/衣服附加特效/时装特效的全层序
        var input = new HumActorDrawPlanner.HumDrawInput
        {
            Sex = 1, CurrentFrame = 200, Dir = 2, Weapon = 5, Shield = 1,
            HasWeapon = true, HasShield = true, HasHair = true,
            HasHorse = true, HasHorseHum = true, HasHorseHair = true,
            HasDressAddEffect = true, DressAddEffectOrder = 0,
            Effect = 50,
        };
        var layers = HumActorDrawPlanner.PlanLayers(input);
        int bodyIdx = layers.IndexOf(HumDrawLayer.Body);
        // 女攻击帧 Wpord=1 → 盾牌后层（dir≠5）在坐骑之前；随后坐骑族 → 身体 → 上层附加 → 发型 → 前层武器 → 前层时装
        Assert.Equal(HumDrawLayer.ShieldBehind, layers[0]);
        Assert.Equal(HumDrawLayer.Horse, layers[1]);
        Assert.Equal(HumDrawLayer.HorseHum, layers[2]);
        Assert.Equal(HumDrawLayer.HorseHair, layers[3]);
        Assert.Equal(HumDrawLayer.Body, layers[4]);
        Assert.Equal(HumDrawLayer.DressAddEffectAbove, layers[5]);
        Assert.Equal(HumDrawLayer.Hair, layers[6]);
        Assert.Equal(HumDrawLayer.WeaponFront, layers[7]);
        Assert.Equal(HumDrawLayer.DressEffectFront, layers[8]);
        Assert.True(layers.IndexOf(HumDrawLayer.Horse) < bodyIdx);
        // dir=2 不在 3/4/5 → 无下层层特效；wpord=1 且 dir=2 → 前层盾不绘（盾在身体后层）
        Assert.DoesNotContain(HumDrawLayer.DressEffectBehind, layers);
        Assert.DoesNotContain(HumDrawLayer.ShieldFront, layers);
    }

    [Fact]
    public void PlanLayers_DressEffectBehind_Dirs345()
    {
        // 有特效组且方向 3/4/5 → 时装特效提前到身体下层；方向 0/1/2 → 前层
        foreach (var dir in new byte[] { 3, 4, 5 })
        {
            var input = new HumActorDrawPlanner.HumDrawInput
            {
                Sex = 0, CurrentFrame = 0, Dir = dir, Effect = 1, HasBody = true,
            };
            var layers = HumActorDrawPlanner.PlanLayers(input);
            Assert.Contains(HumDrawLayer.DressEffectBehind, layers);
        }
        foreach (var dir in new byte[] { 0, 1, 2, 6, 7 })
        {
            var input = new HumActorDrawPlanner.HumDrawInput
            {
                Sex = 0, CurrentFrame = 0, Dir = dir, Effect = 1, HasBody = true,
            };
            var layers = HumActorDrawPlanner.PlanLayers(input);
            Assert.Contains(HumDrawLayer.DressEffectFront, layers);
            Assert.DoesNotContain(HumDrawLayer.DressEffectBehind, layers);
        }
    }

    [Fact]
    public void PlanLayers_WeaponHidden_And_LowWeapon()
    {
        // m_wWeapon < 2 → 不绘武器
        var low = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 200, Dir = 4, Weapon = 1, HasWeapon = true };
        Assert.DoesNotContain(HumDrawLayer.WeaponFront, HumActorDrawPlanner.PlanLayers(low));
        // m_boHideWeapon → 不绘武器
        var hidden = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 200, Dir = 4, Weapon = 5, HasWeapon = true, HideWeapon = true };
        Assert.DoesNotContain(HumDrawLayer.WeaponFront, HumActorDrawPlanner.PlanLayers(hidden));
        // 非混合绘制才绘后层武器（blend=true 时 Wpord=0 武器不绘）
        var blend = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 0, Dir = 4, Weapon = 5, HasWeapon = true, Blend = true };
        Assert.DoesNotContain(HumDrawLayer.WeaponBehind, HumActorDrawPlanner.PlanLayers(blend));
    }

    [Fact]
    public void PlanLayers_DirGuard_And_NoBody()
    {
        // 方向越界 → 空层序（DrawChr 直接 Exit）
        var bad = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 0, Dir = 9, HasBody = true };
        Assert.Empty(HumActorDrawPlanner.PlanLayers(bad));
        // 无身体 → 层序不含 Body 但其余层照常
        var noBody = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 0, Dir = 0, HasBody = false, HasHair = true };
        var layers = HumActorDrawPlanner.PlanLayers(noBody);
        Assert.Contains(HumDrawLayer.Hair, layers);
        Assert.DoesNotContain(HumDrawLayer.Body, layers);
    }

    [Fact]
    public void PlanLayers_DressAddEffect_OrderSides()
    {
        // order≠0 → 下层（身体前绘）；order=0 → 上层（身体后绘）
        var below = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 0, Dir = 0, HasDressAddEffect = true, DressAddEffectOrder = 1 };
        var belowLayers = HumActorDrawPlanner.PlanLayers(below);
        int bodyIdx = belowLayers.IndexOf(HumDrawLayer.Body);
        Assert.Equal(0, belowLayers.IndexOf(HumDrawLayer.DressAddEffectBelow)); // 下层特效在身体之前
        Assert.Equal(1, bodyIdx);

        var above = new HumActorDrawPlanner.HumDrawInput { Sex = 0, CurrentFrame = 0, Dir = 0, HasDressAddEffect = true, DressAddEffectOrder = 0 };
        var aboveLayers = HumActorDrawPlanner.PlanLayers(above);
        bodyIdx = aboveLayers.IndexOf(HumDrawLayer.Body);
        Assert.Equal(0, bodyIdx);
        Assert.Equal(1, aboveLayers.IndexOf(HumDrawLayer.DressAddEffectAbove)); // 上层特效在身体之后
    }

    [Fact]
    public void ShopStall_DrawsLast_OnDirs45()
    {
        Assert.False(HumActorDrawPlanner.ShopStallDrawsLast(0));
        Assert.True(HumActorDrawPlanner.ShopStallDrawsLast(4));
        Assert.True(HumActorDrawPlanner.ShopStallDrawsLast(5));
    }
}
