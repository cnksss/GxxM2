using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J51：THumActor.LoadSurface Hum 类图库选择与 sex/dress/weapon/hair/horse 分支 1:1 测试。</summary>
public sealed class HumSurfacePlanTests
{
    // ---- 身体（衣服 shape → CboHum 库与图号） ----

    [Fact]
    public void Body_ShapeUnder100_HumWzl()
    {
        // 男（sex=0）、dress=4 → shape=2 ≤99 → Hum.wzl，nDress=dress=4，图号 = 4×2000+帧
        var plan = HumSurfacePlan.Body(4, 0, 40);
        Assert.Equal(HumLib.CboHum, plan.Lib);
        Assert.Equal(0, plan.FileIndex);
        Assert.Equal(4 * 2000 + 40, plan.ImageIndex);
        // 女（sex=1）、dress=5 → shape=2 → nDress=5
        var planF = HumSurfacePlan.Body(5, 1, 30);
        Assert.Equal(5 * 2000 + 30, planF.ImageIndex);
    }

    [Fact]
    public void Body_ShapeOver100_ApprLib()
    {
        // dress=300, sex=0 → shape=150 → nAppr=(150-100)/50+2=3、nDress=(150%50)×2+0=0 → 库 CboHum[nAppr-1=2]
        var plan = HumSurfacePlan.Body(300, 0, 10);
        Assert.Equal(HumLib.CboHum, plan.Lib);
        Assert.Equal(2, plan.FileIndex);
        Assert.Equal(10, plan.ImageIndex);
        // nAppr=2 → 库 Index[0] 且 nDress += 24：dress=200 sex=0 → shape=100 → nAppr=2、nDress=0 → 图号 (0+24)×2000
        var plan2 = HumSurfacePlan.Body(200, 0, 5);
        Assert.Equal(0, plan2.FileIndex);
        Assert.Equal(24 * 2000 + 5, plan2.ImageIndex);
        // nAppr=5 → 光身
        // shape=300 → nAppr=(300-100)/50+2=6? 需 shape 满足 (shape-100)/50+2=5 → shape=250：dress=500 sex=0
        var naked = HumSurfacePlan.Body(500, 0, 10);
        Assert.Equal(HumLib.None, naked.Lib);
    }

    [Fact]
    public void Body_TinyFallback()
    {
        // 面积过小回退：(sex+1)×2×2000 + 帧
        var plan = HumSurfacePlan.BodyTinyFallback(0, 40);
        Assert.Equal(2 * 2000 + 40, plan.ImageIndex);
        var planF = HumSurfacePlan.BodyTinyFallback(1, 40);
        Assert.Equal(4 * 2000 + 40, planF.ImageIndex);
    }

    [Fact]
    public void BodyDiY_FileAndImgIndex()
    {
        // file=(diy-1)/30、img=(diy-1)%30、图号 = img×4000 + sex×2000 + 帧
        var plan = HumSurfacePlan.BodyDiY(35, 1, 77);
        Assert.Equal(HumLib.HumDiY, plan.Lib);
        Assert.Equal(1, plan.FileIndex);
        Assert.Equal(4 * 4000 + 1 * 2000 + 77, plan.ImageIndex);
        // diy=1 → file=0、img=0
        var plan0 = HumSurfacePlan.BodyDiY(1, 0, 10);
        Assert.Equal(0, plan0.FileIndex);
        Assert.Equal(10, plan0.ImageIndex);
    }

    // ---- 发型 ----

    [Fact]
    public void Hair_MaleOffsetTable()
    {
        // 男 hair 0..3：offset = hair×2000 + currentFrame
        Assert.Equal(100, HumSurfacePlan.Hair(0, 0, 100).ImageIndex);
        Assert.Equal(2100, HumSurfacePlan.Hair(1, 0, 100).ImageIndex);
        Assert.Equal(4100, HumSurfacePlan.Hair(2, 0, 100).ImageIndex);
        Assert.Equal(6100, HumSurfacePlan.Hair(3, 0, 100).ImageIndex);
    }

    [Fact]
    public void Hair_FemaleOffsetTable()
    {
        // 女：0→0、1→2000、3→6000；2 无偏移（原文 case 缺省 → 不绘）——均含 currentFrame
        Assert.Equal(100, HumSurfacePlan.Hair(0, 1, 100).ImageIndex);
        Assert.Equal(2100, HumSurfacePlan.Hair(1, 1, 100).ImageIndex);
        Assert.Equal(6100, HumSurfacePlan.Hair(3, 1, 100).ImageIndex);
        Assert.Equal(HumLib.None, HumSurfacePlan.Hair(2, 1, 100).Lib);
        // 女且骑马（horse>0）→ hair<4 分支即使无偏移也走偏移表；hair 2 无偏移仍不绘
        Assert.Equal(HumLib.None, HumSurfacePlan.Hair(2, 1, 100, onHorse: true).Lib);
        // 男 hair 4/5（非骑马）→ 无偏移不绘
        Assert.Equal(HumLib.None, HumSurfacePlan.Hair(4, 0, 100).Lib);
        Assert.Equal(HumLib.None, HumSurfacePlan.Hair(5, 0, 100).Lib);
    }

    [Fact]
    public void Hair_ExtendedLibraries()
    {
        // hair 100..109 → 偏移 12000 + (hair-100)×4000 + sex×2000
        var plan100 = HumSurfacePlan.Hair(100, 0, 0);
        Assert.Equal(HumLib.CboHair, plan100.Lib);
        Assert.Equal(12000, plan100.ImageIndex);
        var plan101 = HumSurfacePlan.Hair(101, 1, 0);
        Assert.Equal(12000 + 4000 + 2000, plan101.ImageIndex);
        // hair 50..59 → cboHair10，offset=(hair-50)×2000
        var plan50 = HumSurfacePlan.Hair(52, 0, 0);
        Assert.Equal(HumLib.CboHair10, plan50.Lib);
        Assert.Equal(2 * 2000, plan50.ImageIndex);
        // hair 60..69 → cboHair11，offset=(hair-60)×2000
        var plan60 = HumSurfacePlan.Hair(63, 1, 0);
        Assert.Equal(HumLib.CboHair11, plan60.Lib);
        Assert.Equal(3 * 2000, plan60.ImageIndex);
        // ShowHair=false → 不绘
        Assert.Equal(HumLib.None, HumSurfacePlan.Hair(3, 0, 100, showHair: false).Lib);
    }

    // ---- 武器 ----

    [Fact]
    public void Weapon_ShapeUnder99()
    {
        // weapon=11, sex=1 → shape=(11-1)/2=5 ≤99 → index=weapon=11，图号 = 11×2000+帧
        var plan = HumSurfacePlan.Weapon(11, 1, 55);
        Assert.Equal(HumLib.CboWeapon, plan.Lib);
        Assert.Equal(0, plan.FileIndex);
        Assert.Equal(11 * 2000 + 55, plan.ImageIndex);
    }

    [Fact]
    public void Weapon_Over100_ApprSwap()
    {
        // weapon=300, sex=0 → shape=150 → nAppr=(150-100)/50+2=3、weaponIndex=(50%50)×2+0=0
        var plan = HumSurfacePlan.Weapon(300, 0, 7);
        Assert.Equal(0 * 2000 + 7, plan.ImageIndex);
        Assert.Equal(2, plan.FileIndex);
        // weapon=240 sex=0 → shape=120 → nAppr=(20)/50+2=2、weaponIndex=(20%50)*2+0=40 → case 2 → +76
        var plan240 = HumSurfacePlan.Weapon(240, 0, 3);
        Assert.Equal((40 + 76) * 2000 + 3, plan240.ImageIndex);
        // 美工帧序互换：weapon=520 sex=0 → shape=260 → nAppr=(160)/50+2=5 → else → 原值（20%50)*2+9
        var plan520 = HumSurfacePlan.Weapon(520, 0, 9);
        Assert.Equal(20 * 2000 + 9, plan520.ImageIndex);
        // 互换配对：weapon=540 → shape=270 → nWeaponIndex=(70%50)*2+0=40 不在互换表 → 原值
        var plan540 = HumSurfacePlan.Weapon(540, 0, 1);
        Assert.Equal(40 * 2000 + 1, plan540.ImageIndex);
    }

    [Fact]
    public void Weapon_NilFallback_And_DiY()
    {
        // nAppr=3（shape-100 在 50..99 → shape 150..199 → weapon 300..399 sex=0）：else 分支 → CboWeapon[2]
        var plan = HumSurfacePlan.Weapon(350, 0, 5);
        Assert.Equal(2, plan.FileIndex);
        Assert.Equal(50 * 2000 + 5, plan.ImageIndex);
        // DIY：file=(diy-1)/30、img=(diy-1)%30、图号 = img×4000 + sex×2000 + 帧
        var diy = HumSurfacePlan.WeaponDiY(65, 1, 12);
        Assert.Equal(HumLib.WeaponDiY, diy.Lib);
        Assert.Equal(2, diy.FileIndex);
        Assert.Equal(4 * 4000 + 1 * 2000 + 12, diy.ImageIndex);
    }

    // ---- 翅膀 ----

    [Fact]
    public void Wings_OffsetAndDirCalc()
    {
        // effect=3 → offset=(3-1)×2000=4000；帧<64 → dir×8 + m_nFrame
        var plan = HumSurfacePlan.Wings(3, dir: 5, frame: 10, currentFrame: 999);
        Assert.Equal(4000 + 40 + 10, plan.ImageIndex);
        // 帧≥64 → 直接 currentFrame
        var planHi = HumSurfacePlan.Wings(3, dir: 5, frame: 999, currentFrame: 700);
        Assert.Equal(4000 + 700, planHi.ImageIndex);
        // effect=50 → 无翅膀
        Assert.Equal(HumLib.None, HumSurfacePlan.Wings(50, 0, 0, 0).Lib);
    }

    // ---- 坐骑 ----

    [Fact]
    public void Horse_SegmentedFormulas()
    {
        // 官方 horse2（3..5）：(horse-3)×640 + sex×320 + 帧
        var h2 = HumSurfacePlan.Horse(4, 1, 33);
        Assert.Equal(640 + 320 + 33, h2.ImageIndex);
        // 20..28：600×(horse-20) + 帧
        var h22 = HumSurfacePlan.Horse(22, 0, 9);
        Assert.Equal(600 * 2 + 9, h22.ImageIndex);
        // 29..49：600×(horse-29) + 帧
        var h30 = HumSurfacePlan.Horse(30, 0, 9);
        Assert.Equal(600 + 9, h30.ImageIndex);
        // 50..99：600×(horse-50) + 帧
        var h60 = HumSurfacePlan.Horse(60, 0, 9);
        Assert.Equal(600 * 10 + 9, h60.ImageIndex);
        // 无坐骑
        Assert.Equal(HumLib.None, HumSurfacePlan.Horse(0, 0, 0).Lib);
    }

    [Fact]
    public void HorseEffect_Gates()
    {
        // horse 3..5：1920 + (type-1)×320，type≤6
        var h2 = HumSurfacePlan.HorseEffect(4, 2, 10);
        Assert.Equal(1920 + 320 + 10, h2.ImageIndex);
        var h2over = HumSurfacePlan.HorseEffect(4, 7, 10);
        Assert.Equal(HumLib.None, h2over.Lib);
        // horse 20..28：600×(type-1)，type≤9
        var h22 = HumSurfacePlan.HorseEffect(22, 3, 5);
        Assert.Equal(600 * 2 + 5, h22.ImageIndex);
        var h22over = HumSurfacePlan.HorseEffect(22, 10, 5);
        Assert.Equal(HumLib.None, h22over.Lib);
        // type<1 → 无
        Assert.Equal(HumLib.None, HumSurfacePlan.HorseEffect(22, 0, 5).Lib);
    }
}
