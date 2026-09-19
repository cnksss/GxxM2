using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>GetEffectBase 的取图产物（wimg/idx 变参对；Lib=None 即 Delphi wimg=nil）。</summary>
public readonly record struct EffectBaseImage(MagicImgLib Lib, int BaseIndex)
{
    /// <summary>Delphi wimg=nil（越界 mag 等）：idx 虽为 0 但无图可取。</summary>
    public static readonly EffectBaseImage Empty = new(MagicImgLib.None, 0);
}

/// <summary>
/// magiceff.pas GetEffectBase（474-536）1:1：魔法预备效果取图。
/// mtype=0：mag 80..82 → g_WDragonImg 且按 g_MySelf 坐标分档（80：x≥84→130 否则 140；
/// 81：x≥78∧y≥48→150 否则 160；82→180）；mag 89 → 350；mag 98（富贵兽）→ g_WMonImages.Images[240] @1010；
/// 其余查 EffectBase[NewLevel, mag]（mag ∈ [0..254]）；mtype=1 查 HitEffectBase[NewLevel, mag]（∈ [0..26]）。
/// NewLevel 钳制 0..9。越界 → wimg=nil（Empty）。
/// </summary>
public static class MagicEffectBaseLookup
{
    public static EffectBaseImage Resolve(int mag, int mtype, int newLevel, int selfX, int selfY)
    {
        if (newLevel < 0)
            newLevel = 0;
        if (newLevel > 9)
            newLevel = 9;

        switch (mtype)
        {
            case 0:
            {
                if (mag is >= 80 and <= 82)
                {
                    int idx = mag switch
                    {
                        80 => selfX >= 84 ? 130 : 140,
                        81 => (selfX >= 78 && selfY >= 48) ? 150 : 160,
                        _ => 180,
                    };
                    return new EffectBaseImage(MagicImgLib.WDragonImg, idx);
                }
                if (mag == 89)
                    return new EffectBaseImage(MagicImgLib.WDragonImg, 350);
                if (mag == 98) // 富贵兽攻击效果
                    return new EffectBaseImage(MagicImgLib.WMonImages240, 1010);
                if (mag is >= 0 and < MagicImageOffsetTable.MaxEffect)
                {
                    var (offset, lib) = MagicImageOffsetTable.Effect(newLevel, mag);
                    return new EffectBaseImage(lib, offset);
                }
                return EffectBaseImage.Empty;
            }
            case 1:
            {
                if (mag is >= 0 and < MagicImageOffsetTable.MaxHitEffect)
                {
                    var (offset, lib) = MagicImageOffsetTable.Hit(newLevel, mag);
                    return new EffectBaseImage(lib, offset);
                }
                return EffectBaseImage.Empty;
            }
            default:
                return EffectBaseImage.Empty;
        }
    }

    /// <summary>DrawChr 施法层入口（g_MySelf 坐标接缝由调用方传入）。</summary>
    public static EffectBaseImage ResolveForSpell(int magicEffectNumber, int newLevel, int selfX, int selfY)
        => Resolve(magicEffectNumber - 1, 0, newLevel, selfX, selfY);
}
