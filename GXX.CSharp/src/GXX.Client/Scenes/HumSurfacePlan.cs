using System;

namespace GXX.Client.Scenes;

/// <summary>Hum 类 WIL 图库标识（headless 化：库号即规划产物）。</summary>
public enum HumLib
{
    None = -1,      // 光着身子 / 无图（原文 cboGameImages = nil）
    CboHum = 0,     // g_WCboHum.Indexs[n]
    CboHair,        // g_cboHair
    CboHair10,      // g_cboHair10（hair 50..59）
    CboHair11,      // g_cboHair11（hair 60..69）
    CboWeapon,      // g_WCboWeaponList.Indexs[n]
    HumDiY,         // g_cboHumDiys[fileIdx]
    WeaponDiY,      // g_cboWeaponDiys[fileIdx]
    HorseL,         // g_WLHorseImg（horse 20..28）
    HorseL1,        // g_WLHorseImg1（horse 29..49）
    HorseL2,        // g_WLHorseImg2（horse 50..99 / 3..5 官方 horse2）
    HorseHum,       // g_WLHorseHumImg（horseHum < 50）
    HorseHum1,      // g_WLHorseHumImg1（50..99）
    HorseHum2,      // g_WLHorseHumImg2（100..149）
    HorseHum3,      // g_WLHorseHumImg3（150..199）
    HorseHum4,      // g_WLHorseHumImg4（200..249）
    HorseHum5,      // g_WLHorseHumImg5（250..299）
    HorseHum6,      // g_WLHorseHumImg6（>=300）
    HorseHair,      // g_WLHorseHairImg（马上发型）
    HumEffectDiY,   // g_cboHumEffectDiys[fileIdx]（连击 DIY 翅膀）
}

/// <summary>Hum 表面规划产物：图库 + 图号（headless 无纹理本体）。</summary>
public readonly struct HumSurfaceImage
{
    public HumSurfaceImage(HumLib lib, int imageIndex, int fileIndex = 0)
    {
        Lib = lib;
        ImageIndex = imageIndex;
        FileIndex = fileIndex;
    }

    public HumLib Lib { get; }
    public int ImageIndex { get; }
    public int FileIndex { get; }

    public static readonly HumSurfaceImage Empty = new(HumLib.None, 0);
}

/// <summary>
/// Actor.pas THumActor.LoadSurface（14532-15486+）Hum 类图库选择与图号计算 1:1（批次J51）：
/// 身体（衣服 shape → CboHum 库与 nDress×2000+帧；nAppr=2 时 nDress+24、nAppr=5 光身、
/// nil 回退 CboHum[0]/sex；面积过小回退 (sex+1)×2×2000）、发型（hair&lt;6 男女偏移表 /
/// 100..109 / 50..59 / 60..69 分库）、武器（shape 换算、nAppr=2 +76、nAppr=4 美工帧序互换、
/// nil 回退 CboWeapon[0]/sex）、翅膀（effect≠0/50 → (effect-1)×2000，帧&lt;64 加 dir×8）、
/// 坐骑（3..5 官方 horse2、20..28、29..49、50..99 分段公式）。
/// </summary>
public static class HumSurfacePlan
{
    /// <summary>身体：普通/简化模式衣服 shape → 图库与图号（15061-15184）。</summary>
    public static HumSurfaceImage Body(int dress, int sex, int currentFrame, bool surfaceTiny = false)
    {
        int nShape = (dress - sex) / 2;

        int nAppr;
        int nDress;
        if (nShape < 1000)
        {
            if (nShape <= 99)
            {
                // Hum.wzl 0~99
                nAppr = 0;
                nDress = dress;
            }
            else
            {
                nAppr = (nShape - 100) / 50 + 2;
                nDress = ((nShape - 100) % 50) * 2 + sex;
            }

            switch (nAppr)
            {
                case 0:
                    return Lib(HumLib.CboHum, 0, nDress * 2000 + currentFrame);
                case 2:
                    return Lib(HumLib.CboHum, 0, (nDress + 24) * 2000 + currentFrame);
                case 5:
                    return HumSurfaceImage.Empty; // 光着身子
                default:
                    return Lib(HumLib.CboHum, nAppr - 1, nDress * 2000 + currentFrame);
            }
        }

        // nShape >= 1000 → 无库匹配，回退 CboHum[0] / sex
        return Lib(HumLib.CboHum, 0, sex * 2000 + currentFrame);
    }

    /// <summary>身体表面积过小（≤16px）时的回退：图号 = (sex+1)×2×2000 + 帧（15125-15132）。</summary>
    public static HumSurfaceImage BodyTinyFallback(int sex, int currentFrame)
        => Lib(HumLib.CboHum, 0, (sex + 1) * 2 * 2000 + currentFrame);

    /// <summary>连击 DIY 衣服（15188-15200）：file=(diy-1) div 30，img=(diy-1) mod 30，图号 = img×4000 + sex×2000 + 帧。</summary>
    public static HumSurfaceImage BodyDiY(int cboDressUseDiyImage, int sex, int currentFrame)
    {
        int fileIndex = (cboDressUseDiyImage - 1) / 30;
        int imgIndex = (cboDressUseDiyImage - 1) % 30;
        if (fileIndex < 0 || fileIndex >= CboHumDiyFileCount)
            return HumSurfaceImage.Empty;
        return new HumSurfaceImage(HumLib.HumDiY, imgIndex * 4000 + sex * 2000 + currentFrame, fileIndex);
    }

    /// <summary>发型（15203-15239）：hair&lt;6 男女偏移表 / 100..109 / 50..59 / 60..69 分库；horse&gt;0 时女发 1/2 也给偏移。</summary>
    public static HumSurfaceImage Hair(int hair, int sex, int currentFrame, bool onHorse = false, bool showHair = true)
    {
        if (!showHair)
            return HumSurfaceImage.Empty;

        if (hair < 6)
        {
            int offset = -1;
            if (hair < 4 || onHorse)
            {
                switch (sex)
                {
                    case 0:
                        offset = hair * 2000;
                        break;
                    case 1:
                        switch (hair)
                        {
                            case 0: offset = 0; break;
                            case 1: offset = 2000; break;
                            case 3: offset = 6000; break;
                        }
                        break;
                }
            }
            return offset >= 0 ? new HumSurfaceImage(HumLib.CboHair, offset + currentFrame) : HumSurfaceImage.Empty;
        }
        if (hair is >= 100 and <= 109)
        {
            int offset = 12000 + (hair - 100) * 2000 * 2 + sex * 2000;
            return new HumSurfaceImage(HumLib.CboHair, offset + currentFrame);
        }
        if (hair is >= 50 and <= 59)
            return new HumSurfaceImage(HumLib.CboHair10, (hair - 50) * 2000 + currentFrame);
        if (hair is >= 60 and <= 69)
            return new HumSurfaceImage(HumLib.CboHair11, (hair - 60) * 2000 + currentFrame);
        return HumSurfaceImage.Empty;
    }

    /// <summary>武器（15241-15386）：shape 换算 + nAppr 库选择 + nAppr=4 美工帧序互换 + nil 回退 CboWeapon[0]/sex。</summary>
    public static HumSurfaceImage Weapon(int weapon, int sex, int currentFrame)
    {
        int nShape = (weapon - sex) / 2;
        int nAppr;
        int nWeaponIndex;
        if (nShape < 1000)
        {
            if (nShape <= 99)
            {
                nWeaponIndex = weapon;
                nAppr = 0;
            }
            else
            {
                nWeaponIndex = ((nShape - 100) % 50) * 2 + sex;
                nAppr = (nShape - 100) / 50 + 2;
            }

            switch (nAppr)
            {
                case 0:
                    return Lib(HumLib.CboWeapon, 0, nWeaponIndex * 2000 + currentFrame);
                case 1:
                    return HumSurfaceImage.Empty;
                case 2:
                    return Lib(HumLib.CboWeapon, 0, (nWeaponIndex + 76) * 2000 + currentFrame);
                case 4:
                    // 美工搞错了图片帧序，特殊处理下 piaoyun 2013-07-29
                    switch (nWeaponIndex)
                    {
                        case 10 * 2: nWeaponIndex = 11 * 2; break;
                        case 11 * 2: nWeaponIndex = 10 * 2; break;
                        case 13 * 2: nWeaponIndex = 14 * 2; break;
                        case 14 * 2: nWeaponIndex = 13 * 2; break;
                        case 16 * 2: nWeaponIndex = 17 * 2; break;
                        case 17 * 2: nWeaponIndex = 16 * 2; break;
                    }
                    return Lib(HumLib.CboWeapon, nAppr - 1, nWeaponIndex * 2000 + currentFrame);
                default:
                    return Lib(HumLib.CboWeapon, nAppr - 1, nWeaponIndex * 2000 + currentFrame);
            }
        }

        // nShape >= 1000 → 回退 CboWeapon[0] / sex
        return Lib(HumLib.CboWeapon, 0, sex * 2000 + currentFrame);
    }

    /// <summary>连击 DIY 武器（15372-15385）。</summary>
    public static HumSurfaceImage WeaponDiY(int cboWeaponUseDiyImage, int sex, int currentFrame)
    {
        int fileIndex = (cboWeaponUseDiyImage - 1) / 30;
        int imgIndex = (cboWeaponUseDiyImage - 1) % 30;
        if (fileIndex < 0 || fileIndex >= CboHumDiyFileCount)
            return HumSurfaceImage.Empty;
        return new HumSurfaceImage(HumLib.WeaponDiY, imgIndex * 4000 + sex * 2000 + currentFrame, fileIndex);
    }

    /// <summary>翅膀（15390-15426）：effect≠0/50 → 偏移 (effect-1)×2000；帧&lt;64 → +dir×8，否则 +currentFrame。</summary>
    public static HumSurfaceImage Wings(int effect, int dir, int frame, int currentFrame)
    {
        if (effect == 0 || effect == 50)
            return HumSurfaceImage.Empty;
        int offset = (effect - 1) * 2000;
        int index = frame < 64
            ? offset + dir * 8 + frame
            : offset + currentFrame;
        return new HumSurfaceImage(HumLib.CboHum, index);
    }

    /// <summary>坐骑（14876-15034）分段公式。</summary>
    public static HumSurfaceImage Horse(int horse, int sex, int currentFrame)
    {
        if (horse is >= 3 and <= 5)
        {
            // 官方 horse2
            return new HumSurfaceImage(HumLib.HorseL2, (horse - 3) * 640 + sex * 320 + currentFrame);
        }
        if (horse is >= 20 and <= 28)
            return new HumSurfaceImage(HumLib.HorseL, 600 * (horse - 20) + currentFrame);
        if (horse is >= 29 and <= 49)
            return new HumSurfaceImage(HumLib.HorseL1, 600 * (horse - 29) + currentFrame);
        if (horse is >= 50 and <= 99)
            return new HumSurfaceImage(HumLib.HorseL2, 600 * (horse - 50) + currentFrame);
        return HumSurfaceImage.Empty;
    }

    /// <summary>坐骑特效图号（14880-15033）：horse 3..5 → 1920+(type-1)×320；20..28 → 600×(type-1)；29..49 → 600×(type-1)。</summary>
    public static HumSurfaceImage HorseEffect(int horse, int horseEffectType, int currentFrame)
    {
        if (horseEffectType < 1)
            return HumSurfaceImage.Empty;
        if (horse is >= 3 and <= 5)
        {
            if (horseEffectType > 6) return HumSurfaceImage.Empty;
            return new HumSurfaceImage(HumLib.HorseL2, 1920 + (horseEffectType - 1) * 320 + currentFrame);
        }
        if (horse is >= 20 and <= 28)
        {
            if (horseEffectType > 9) return HumSurfaceImage.Empty;
            return new HumSurfaceImage(HumLib.HorseL, 600 * (horseEffectType - 1) + currentFrame);
        }
        if (horse is >= 29 and <= 49)
        {
            if (horseEffectType > 21) return HumSurfaceImage.Empty;
            return new HumSurfaceImage(HumLib.HorseL1, 600 * (horseEffectType - 1) + currentFrame);
        }
        return HumSurfaceImage.Empty;
    }

    private const int CboHumDiyFileCount = 4; // CBOHUMDIYFILE_COUNT

    private static HumSurfaceImage Lib(HumLib lib, int libIndex, int imageIndex)
        => new(lib, imageIndex, libIndex);

    /// <summary>
    /// 骑马马上人（14901-14959，m_btHorse≥20 分支）：expand=0 → (sex, 每马 2 帧)；否则 (0, 1)；
    /// horseHum 按 50/100/150/200/250/300 分段选 7 库，图号 = 600×(sexAdj + (hum−base)×count) + 帧。
    /// </summary>
    public static HumSurfaceImage HorseHum(int horseHum, int sex, bool expanded, int currentFrame)
    {
        int sexAdj = expanded ? 0 : sex;
        int count = expanded ? 1 : 2;
        int baseIndex;
        HumLib lib;
        if (horseHum >= 300)
        {
            baseIndex = 300;
            lib = HumLib.HorseHum6;
        }
        else if (horseHum >= 250)
        {
            baseIndex = 250;
            lib = HumLib.HorseHum5;
        }
        else if (horseHum >= 200)
        {
            baseIndex = 200;
            lib = HumLib.HorseHum4;
        }
        else if (horseHum >= 150)
        {
            baseIndex = 150;
            lib = HumLib.HorseHum3;
        }
        else if (horseHum >= 100)
        {
            baseIndex = 100;
            lib = HumLib.HorseHum2;
        }
        else if (horseHum >= 50)
        {
            baseIndex = 50;
            lib = HumLib.HorseHum1;
        }
        else
        {
            baseIndex = 0;
            lib = HumLib.HorseHum;
        }
        return new HumSurfaceImage(lib, 600 * (sexAdj + (horseHum - baseIndex) * count) + currentFrame);
    }

    /// <summary>马上发型（14990-15030 各 horse 段共用）：600×(sex + horseHair×2) + 帧。</summary>
    public static HumSurfaceImage HorseHair(int sex, int horseHair, int currentFrame)
        => new(HumLib.HorseHair, 600 * (sex + horseHair * 2) + currentFrame);

    /// <summary>简化显示武器（15273-15360）：职业缺省 shape（战 24 / 法 25 / 道 28），自定义走 nSimpleWeaponShapeArr；图号 = shape×2+sex。</summary>
    public static HumSurfaceImage WeaponSimple(int job, int sex, int currentFrame, bool custom = false, int[]? customShapes = null)
    {
        int shape = custom
            ? (customShapes != null && customShapes.Length > 0 ? customShapes[job >= 0 && job < customShapes.Length ? job : 0] : 24)
            : job switch
            {
                0 => 24,
                1 => 28, // 骨玉权杖
                2 => 25,
                _ => 24,
            };

        int nAppr;
        int nWeaponIndex;
        if (shape < 1000)
        {
            if (shape <= 99)
            {
                nWeaponIndex = shape * 2 + sex;
                nAppr = 0;
            }
            else
            {
                nWeaponIndex = ((shape - 100) % 50) * 2 + sex;
                nAppr = (shape - 100) / 50 + 2;
            }
            switch (nAppr)
            {
                case 0:
                    return Lib(HumLib.CboWeapon, 0, nWeaponIndex * 2000 + currentFrame);
                case 1:
                    return HumSurfaceImage.Empty;
                case 2:
                    return Lib(HumLib.CboWeapon, 0, (nWeaponIndex + 76) * 2000 + currentFrame);
                default:
                    return Lib(HumLib.CboWeapon, nAppr - 1, nWeaponIndex * 2000 + currentFrame);
            }
        }
        return Lib(HumLib.CboWeapon, 0, sex * 2000 + currentFrame);
    }

    /// <summary>连击 DIY 翅膀（15416-15430）：file=(diy-1) div 30 门控 + img×4000 + sex×2000 + 帧 @g_cboHumEffectDiys。</summary>
    public static HumSurfaceImage WingsDiY(int cboDressUseDiyImage, int sex, int currentFrame)
    {
        int fileIndex = (cboDressUseDiyImage - 1) / 30;
        int imgIndex = (cboDressUseDiyImage - 1) % 30;
        if (fileIndex < 0 || fileIndex >= CboHumDiyFileCount)
            return HumSurfaceImage.Empty;
        return new HumSurfaceImage(HumLib.HumEffectDiY, imgIndex * 4000 + sex * 2000 + currentFrame, fileIndex);
    }
}
