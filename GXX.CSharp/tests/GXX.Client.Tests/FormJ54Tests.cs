using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>MagicImageOffset.pas 表机器提取锁定（批次J54）。</summary>
public sealed class MagicImageOffsetTableTests
{
    [Fact]
    public void Effect_MagicShield_LevelSplit()
    {
        // 行 0：28 不在 case 库表内 → 缺省 WMagicImages；行 1..9：InitLevel 覆写 690/WMagic6Images
        Assert.Equal((3880, MagicImgLib.WMagicImages), MagicImageOffsetTable.Effect(0, 28));
        Assert.Equal((690, MagicImgLib.WMagic6Images), MagicImageOffsetTable.Effect(1, 28));
        Assert.Equal((690, MagicImgLib.WMagic6Images), MagicImageOffsetTable.Effect(9, 28));
    }

    [Fact]
    public void Effect_NewShield_MagicreImages()
    {
        // 行 0：offset 940 + case WMagic2Images；行 1..9：InitLevel 覆写 900/g_WMagicreImages
        Assert.Equal((940, MagicImgLib.WMagic2Images), MagicImageOffsetTable.Effect(0, 66));
        Assert.Equal((900, MagicImgLib.WMagicreImages), MagicImageOffsetTable.Effect(1, 66));
        Assert.Equal((900, MagicImgLib.WMagicreImages), MagicImageOffsetTable.Effect(5, 66));
    }

    [Fact]
    public void Effect_PerLevelFormulas()
    {
        Assert.Equal((160, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Effect(5, 8));   // 120+(5-1)×10
        Assert.Equal((80, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Effect(9, 19));   // (9-1)×10
        Assert.Equal((510, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(9, 50));    // 430+(9-1)×10 流星火雨
        Assert.Equal((360, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(9, 33));    // 280+(9-1)×10 灭天火
        Assert.Equal((600, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Effect(9, 3));   // 440+(9-1)×20 施毒术
        Assert.Equal((1020, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Effect(1, 14)); // 召唤骷髅（注释块外的真实现）
    }

    [Fact]
    public void Effect_SoulStealer_ThreeSegments()
    {
        // 噬血术 47：三段 630+(I-1)×20 / 780+(I-4)×20 / 930+(I-7)×20
        Assert.Equal((650, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(2, 47));
        Assert.Equal((800, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(5, 47)); // 780+(5-4)×20
        Assert.Equal((950, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(8, 47)); // 930+(8-7)×20
        Assert.Equal((630, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(1, 47)); // 630+(1-1)×20
        Assert.Equal((970, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Effect(9, 47)); // 930+(9-7)×20
    }

    [Fact]
    public void Effect_SummonLibs()
    {
        // 召唤神兽 27 / 召唤圣兽 46：段内恒定 160/180/200
        Assert.Equal((160, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Effect(1, 27));
        Assert.Equal((180, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Effect(5, 27));
        Assert.Equal((200, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Effect(9, 27));
        Assert.Equal((160, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Effect(1, 46));
        Assert.Equal((200, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Effect(9, 46));
    }

    [Fact]
    public void Effect_Level0Row_MoveCopiedToAllLevels()
    {
        Assert.Equal((0, MagicImgLib.WMagicImages), MagicImageOffsetTable.Effect(0, 0));
        Assert.Equal((0, MagicImgLib.WMagicImages), MagicImageOffsetTable.Effect(7, 0));
        // 行 0 特例：42=-1（狮子吼）；36=WMon22；98=100；行 7 同步继承 42
        Assert.Equal((-1, MagicImgLib.WMagic2Images), MagicImageOffsetTable.Effect(0, 42));
        Assert.Equal((-1, MagicImgLib.WMagic2Images), MagicImageOffsetTable.Effect(7, 42));
        Assert.Equal((0, MagicImgLib.WMonImages22), MagicImageOffsetTable.Effect(0, 36));
        Assert.Equal((100, MagicImgLib.WMagicImages), MagicImageOffsetTable.Effect(0, 98));
    }

    [Fact]
    public void Effect_Level0CaseMapping()
    {
        Assert.Equal((2040, MagicImgLib.WMagic8Images), MagicImageOffsetTable.Effect(0, 115));  // 血魄一击(法)
        Assert.Equal((2180, MagicImgLib.WMagic8Images), MagicImageOffsetTable.Effect(0, 116));  // 血魄一击(道)
        Assert.Equal((200, MagicImgLib.WMagic10Images), MagicImageOffsetTable.Effect(0, 203));  // 202..225 → WMagic10
        Assert.Equal((120, MagicImgLib.WMagic6Images), MagicImageOffsetTable.Effect(3, 201));  // 裂神符行 0 经 Move 继承（201 case → WMagic6）
        Assert.Equal((460, MagicImgLib.WMagic4Images), MagicImageOffsetTable.Effect(0, 60));    // 59..64 → WMagic4
        Assert.Equal((0, MagicImgLib.WMagic5Images), MagicImageOffsetTable.Effect(0, 73));      // 分身术
        Assert.Equal((400, MagicImgLib.CboEffect), MagicImageOffsetTable.Effect(0, 54));        // 倚天辟地
        Assert.Equal((0, MagicImgLib.CboEffect), MagicImageOffsetTable.Effect(0, 100));         // 99..110 连击
        Assert.Equal((100, MagicImgLib.WMagic5Images), MagicImageOffsetTable.Effect(0, 198));   // 月灵
        Assert.Equal((280, MagicImgLib.WMagic5Images), MagicImageOffsetTable.Effect(0, 199));
    }

    [Fact]
    public void HitTable_TriplesAndRowCopies()
    {
        Assert.Equal((800, MagicImgLib.WMagicImages), MagicImageOffsetTable.Hit(0, 0));
        Assert.Equal((1600, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Hit(1, 0)); // 攻杀剑术
        Assert.Equal((1600, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Hit(3, 0)); // 2..3 拷贝行 1
        Assert.Equal((1690, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Hit(4, 0));
        Assert.Equal((1780, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Hit(9, 0)); // 8..9 拷贝行 7
        Assert.Equal((2230, MagicImgLib.WMagic7Images16), MagicImageOffsetTable.Hit(6, 1)); // 刺杀 5..6 拷贝行 4
        Assert.Equal((1840, MagicImgLib.WMagic8Images16), MagicImageOffsetTable.Hit(9, 3)); // 烈火
        // 逐日剑法 13：行 0=512/WMagic6，行 1=0/WMagic9，行 3 拷贝行 1，行 9=180
        Assert.Equal((512, MagicImgLib.WMagic6Images), MagicImageOffsetTable.Hit(0, 13));
        Assert.Equal((0, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Hit(1, 13));
        Assert.Equal((0, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Hit(3, 13));
        Assert.Equal((180, MagicImgLib.WMagic9Images), MagicImageOffsetTable.Hit(9, 13));
        // Move 拷贝且未覆写：Hit(2,5) = 行 0 的 (40, WMagic2Images)
        Assert.Equal((40, MagicImgLib.WMagic2Images), MagicImageOffsetTable.Hit(2, 5));
        Assert.Equal((2380, MagicImgLib.WMagic8Images), MagicImageOffsetTable.Hit(0, 26));  // 血魄一击
    }
}

/// <summary>magiceff.pas GetEffectBase（474-536）1:1（批次J54）。</summary>
public sealed class MagicEffectBaseLookupTests
{
    [Fact]
    public void Resolve_Mag80_XThreshold()
    {
        var (lib80hi, idx80hi) = MagicEffectBaseLookup.Resolve(80, 0, 0, selfX: 84, selfY: 0);
        Assert.Equal((MagicImgLib.WDragonImg, 130), (lib80hi, idx80hi));
        var (lib80lo, idx80lo) = MagicEffectBaseLookup.Resolve(80, 0, 0, selfX: 83, selfY: 0);
        Assert.Equal((MagicImgLib.WDragonImg, 140), (lib80lo, idx80lo));
    }

    [Fact]
    public void Resolve_Mag81_XyThreshold()
    {
        var (l1, i1) = MagicEffectBaseLookup.Resolve(81, 0, 0, selfX: 78, selfY: 48);
        Assert.Equal((MagicImgLib.WDragonImg, 150), (l1, i1));
        var (l2, i2) = MagicEffectBaseLookup.Resolve(81, 0, 0, selfX: 77, selfY: 47);
        Assert.Equal((MagicImgLib.WDragonImg, 160), (l2, i2));
        var (l3, i3) = MagicEffectBaseLookup.Resolve(81, 0, 0, selfX: 78, selfY: 47); // y 不足
        Assert.Equal((MagicImgLib.WDragonImg, 160), (l3, i3));
    }

    [Fact]
    public void Resolve_Mag82_89_98_Specials()
    {
        var (l82, i82) = MagicEffectBaseLookup.Resolve(82, 0, 0, 0, 0);
        Assert.Equal((MagicImgLib.WDragonImg, 180), (l82, i82));
        var (l89, i89) = MagicEffectBaseLookup.Resolve(89, 0, 7, 0, 0); // NewLevel 不影响特例
        Assert.Equal((MagicImgLib.WDragonImg, 350), (l89, i89));
        var (l98, i98) = MagicEffectBaseLookup.Resolve(98, 0, 0, 0, 0); // 富贵兽
        Assert.Equal((MagicImgLib.WMonImages240, 1010), (l98, i98));
    }

    [Fact]
    public void Resolve_TablePath_AndLevelClamp()
    {
        var (l28, i28) = MagicEffectBaseLookup.Resolve(28, 0, 3, 0, 0);
        Assert.Equal((MagicImgLib.WMagic6Images, 690), (l28, i28));
        var (l0, i0) = MagicEffectBaseLookup.Resolve(0, 0, 0, 0, 0);
        Assert.Equal((MagicImgLib.WMagicImages, 0), (l0, i0));
        // NewLevel 钳制 0..9：99 → 9
        var (l99, i99) = MagicEffectBaseLookup.Resolve(8, 0, 99, 0, 0);
        Assert.Equal((MagicImgLib.WMagic7Images16, 200), (l99, i99)); // 120+(9-1)×10
        var (lneg, ineg) = MagicEffectBaseLookup.Resolve(8, 0, -5, 0, 0);
        Assert.Equal((MagicImgLib.WMagic2Images, 20), (lneg, ineg));  // 钳到 0 → 行 0 (20, WMagic2)
    }

    [Fact]
    public void Resolve_OutOfRange_MagsAndMtypes()
    {
        Assert.Equal(MagicImgLib.None, MagicEffectBaseLookup.Resolve(255, 0, 0, 0, 0).Lib);  // ≥ MAXEFFECT
        Assert.Equal(MagicImgLib.None, MagicEffectBaseLookup.Resolve(27, 1, 0, 0, 0).Lib);   // ≥ MAXHITEFFECT
        Assert.Equal(MagicImgLib.None, MagicEffectBaseLookup.Resolve(0, 2, 0, 0, 0).Lib);    // 未知 mtype
    }

    [Fact]
    public void Resolve_HitEffectPath()
    {
        var (l1, i1) = MagicEffectBaseLookup.Resolve(0, 1, 1, 0, 0); // 攻杀行 1
        Assert.Equal((MagicImgLib.WMagic7Images16, 1600), (l1, i1));
        var (l2, i2) = MagicEffectBaseLookup.Resolve(26, 1, 0, 0, 0); // 血魄一击
        Assert.Equal((MagicImgLib.WMagic8Images, 2380), (l2, i2));
    }

    [Fact]
    public void ResolveForSpell_MinusOneEntry()
    {
        // DrawChr 调用形态：EffectNumber−1 作 mag；42（狮子吼）行 0 idx=-1 但库非空（Delphi wimg 非nil）
        var (lneg, ineg) = MagicEffectBaseLookup.ResolveForSpell(43, 0, 0, 0);
        Assert.Equal((MagicImgLib.WMagic2Images, -1), (lneg, ineg));
        var (lshield, ishield) = MagicEffectBaseLookup.ResolveForSpell(29, 3, 0, 0); // mag=28 魔法盾
        Assert.Equal((MagicImgLib.WMagic6Images, 690), (lshield, ishield));
    }
}
