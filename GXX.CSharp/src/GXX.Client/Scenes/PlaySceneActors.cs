using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// Grobal2.pas 3303-3390 THumFeature（人物形象结构；字段顺序与原文 1:1，headless 用类承载）。
/// 末尾补充 boDressEffNoSex/boDressEff_30NoSex（原文 3320/3322 已声明，这里以属性读写同一槽位保持唯一）。
/// </summary>
public sealed class THumFeature
{
    public ushort wRace;                 // 脸型
    public ushort wWeapon;               // 武器
    public ushort wWeaponSound;
    public ushort wDress;                // 衣服
    public ushort wShield;               // 盾牌
    public byte btGender;
    public byte btJob;
    public byte btHair;
    public byte btHorseType;
    public byte btCaseltGuild;
    public bool boShopStall;
    public byte btBodyColor;
    public byte btShopStallDir;
    public ushort wDressEffType;
    public ushort wDressEffType_30;
    public bool boDressEffNormalDraw;
    public bool boDressEffNoSex;
    public bool boDressEff_30NormalDraw;
    public bool boDressEff_30NoSex;
    public int nChangeAppr;
    public short nWeaponEffectIndex;
    public ushort wWeaponEffectOffSet;
    public ushort wDBWeaponEffectOffSet;
    public short nDressEffectIndex;
    public ushort wDressEffectOffSet;
    public short nShieldEffectIndex;
    public ushort wShieldEffectOffSet;
    public bool boDressEffectNoBlend;
    public bool boDressEffectNoSex;
    public bool boWeaponEffectNoBlend;
    public bool boWeaponEffectNoSex;
    public bool boShieldEffectNoBlend;
    public bool boShieldEffectNoSex;
    public short nShieldAddEffectIndex;
    public ushort wShieldAddEffectOffSet;
    public short nDressAddEffectIndex;
    public byte bDressAddEffectOrder;
    public ushort wDressAddEffectOffSet;
    public ushort wDressAddEffectCount;
    public ushort wDressAddEffectTime;
    public bool boDressAddEffectNoBlend;
    public bool boDressAddEffectDrawCenter;
    public short nMedalEffectIndex;
    public ushort wMedalEffectOffSet;
    public bool boMedalEffectNoBlend;
    public bool boMedalEffectNoSex;
    public byte btDoubleHumHorseType;
    public bool boShowHorseWingsEffect;
    public byte btHorseEffectType;
    public ushort btHorseHum;
    public byte btHorseHumExpand;
    public byte btHorseHair;
    public bool boShowFashion;
    public bool boMagicShield;
    public int btReLevel;
    public string sActiveFengHaoName = "";
    public int nActiveFengHaoID;
    public ushort dwActiveFengHaoLooks;
    public byte btActiveFengHaoReserved;
    public byte btActiveFengHaoColor;
    public bool boShowHair;
    public byte btCboDressUseDiyImage;
    public byte btCboWeaponUseDiyImage;
    public byte btOldHair;
    public bool boPlayMoster;
}

/// <summary>Grobal2.pas 3395 THumBBType（人物宝宝类型）。</summary>
public enum THumBBType
{
    bbNo = 0,      // 不是宝宝
    bbSlave = 1,   // 普通宝宝
    bbGamePet = 2, // 宠物宝宝
}

/// <summary>Grobal2.pas 3397-3408 TMonFeature（怪物形象结构）。</summary>
public sealed class TMonFeature
{
    public ushort wRaceImg;
    public ushort wWeapon;
    public ushort wAppr;
    public byte btBodyColor;
    public byte btRace;
    public uint MonLevel;
    public THumBBType HumBBType;
    public bool IsExploreItem;
    public bool IsDisableSimpleActor;
    public int nChangeAppr;
}

/// <summary>Grobal2.pas 3413-3416 TFeature（形象结构：判别 + 256 字节缓冲）。</summary>
public sealed class TFeature
{
    public int Feature;
    public byte[] Buffer = new byte[256];

    // pTHumFeature(@Buffer)/pTMonFeature(@Buffer) 的 headless 等价字节
    internal THumFeature? _hum;
    internal TMonFeature? _mon;
}

/// <summary>
/// Actor.pas TActor 场景交互所需字段扩展（批次J72）：
/// CheckSelect/CharWidth/CharHeight（5595-5650）与 PlayScn 判定字段
/// （m_boVisible/m_boHoldPlace/m_boGhost/m_btRealRace/m_btHorse/m_HumsBBType…）。
/// </summary>
public partial class TActorCore
{
    // ---- 可见性与占位（PlayScn 碰撞/查找族） ----
    public bool m_boVisible = true;
    public bool m_boHoldPlace = true;
    public bool m_boGhost;

    // ---- 形象 ----
    public THumFeature? m_HumFeature;      // m_btRace in [0,1] 时的人物形象（别名）
    public TMonFeature? m_MonFeature;      // 怪物形象
    public int m_btRealRace;
    public int m_btHorse;
    public byte m_btDoubleHumHorse;
    public bool m_boShowHorseWingsEffect;
    public ushort m_btHorseHum;
    public byte m_btHorseHumExpand;
    public byte m_btHorseHair;
    public byte m_btHorseEffectType;
    public bool m_boShowFashion;
    public bool m_boMagicShield;
    public int m_btReLevel;
    public byte m_btSex;
    public byte m_btJob;
    public byte m_btHair;
    public byte m_btBodyColor;
    public bool m_boShowHair = true;
    public bool m_boPlayMoster;
    public byte m_btOldHair;
    public byte m_btCaseltGuild;
    public bool m_boShopStall;
    public ushort m_wDress;
    public ushort m_wWeapon;
    public ushort m_wWeaponSound;
    public ushort m_wEffect;
    public ushort m_wEffect_30;
    public ushort m_wShield;
    public bool m_boEffectNormalDraw;
    public bool m_boEffect_30NormalDraw;
    public bool m_boDressEffNoSex;
    public bool m_boDressEff_30NoSex;
    public bool m_boDressEffectNoBlend;
    public bool m_boDressEffectNoSex;
    public bool m_boWeaponEffectNoBlend;
    public bool m_boWeaponEffectNoSex;
    public bool m_boShieldEffectNoBlend;
    public bool m_boShieldEffectNoSex;
    public bool m_boMedalEffectNoBlend;
    public bool m_boMedalEffectNoSex;
    public short m_nWeaponEffectIndex;
    public ushort m_wDBWeaponEffectOffSet;
    public ushort m_wWeaponEffectOffSet;
    public short m_nDressEffectIndex;
    public ushort m_wDressEffectOffSet;
    public short m_nShieldEffectIndex;
    public ushort m_wShieldEffectOffSet;
    public short m_nMedalEffectIndex;
    public ushort m_wMedalEffectOffSet;
    public byte m_btCboDressUseDiyImage;
    public byte m_btCboWeaponUseDiyImage;
    // 附加衣服特效（THumFeature 3350-3356 / 3347-3348）
    public short m_nDressAddEffectIndex;
    public byte m_bDressAddEffectOrder;
    public ushort m_wDressAddEffectOffSet;
    public ushort m_wDressAddEffectCount;
    public ushort m_wDressAddEffectTime;
    public bool m_boDressAddEffectNoBlend;
    public bool m_boDressAddEffectDrawCenter;
    public short m_nShieldAddEffectIndex;
    public ushort m_wShieldAddEffectOffSet;
    public string m_sActiveFengHaoName = "";
    public int m_nActiveFengHaoID;
    public ushort m_dwActiveFengHaoLooks;
    public byte m_btActiveFengHaoReserved;
    public int m_nActiveFengHaoColor;   // GetRGB 后的 TColor
    public THumBBType m_HumsBBType = THumBBType.bbNo;
    public bool m_IsExploreItem;
    public int m_nState;
    public string m_sUserName = "";

    // ---- 批次J79：血条绘制族（DrawActorLabel）新增字段 ----
    /// <summary>m_Abil（TAbility；血条按 HP/MaxHP、MP/MaxMP 比例收缩）。</summary>
    public TAbility m_Abil;

    /// <summary>m_AbilNG（TAbilityNG；内功黄条 NH/MaxNH）。</summary>
    public TAbilityNG m_AbilNG;

    /// <summary>m_boTrainingNG（是否正在修炼内功）。</summary>
    public bool m_boTrainingNG;

    /// <summary>m_noInstanceOpenHealth（临时显血；超时自动关闭）。</summary>
    public bool m_noInstanceOpenHealth;

    /// <summary>m_dwOpenHealthStart（临时显血起始时刻）。</summary>
    public long m_dwOpenHealthStart;

    /// <summary>m_dwOpenHealthTime（临时显血持续时间）。</summary>
    public long m_dwOpenHealthTime;

    /// <summary>是否为 TCustomActor 见 ActorLabelRender.cs（本批次直接复用）。</summary>

    /// <summary>(Self as TCustomActor).Config.BaseConfig 的血条相关字段。</summary>
    public HpBarBaseConfig? CustomBaseConfig;

    // ---- 当前动作与行走像素（GetCharacter 命中反算用） ----
    public int m_nPx;
    public int m_nPy;

    /// <summary>m_nDownDrawLevel（SortYDrawActor 排序键；Actor.pas）。</summary>
    public int m_nDownDrawLevel;

    /// <summary>Finalize 钩子（PlayScene.Finalize 遍历调用）。</summary>
    public Action? OnFinalize;

    /// <summary>场景角色显示名（Delphi 各类 T*Actor.ClassName；headless 用字符串承载）。</summary>
    public virtual string ActorClass => "TActor";

    /// <summary>身体/坐骑图尺寸（GameCanvas.TTexture 的 headless 镜像；null = 图未就绪）。</summary>
    public SurfaceSize? BodySurface;
    public SurfaceSize? HorseSurface;

    /// <summary>CheckTextureAlpha 接缝（默认空实现 → 恒不可选中）。</summary>
    public Func<SurfaceSize, int, int, bool>? CheckTextureAlpha;

    /// <summary>按 CheckTextureAlpha 的设置者所在图选择（true = 用坐骑图）。</summary>
    public SurfaceSize? SelectSurfaceForSelect => m_btHorse == 0 ? BodySurface : HorseSurface;

    /// <summary>Actor.pas 5595-5609 CharWidth 1:1（步行 48 缺省 / 骑马 100 缺省）。</summary>
    public int CharWidth
    {
        get
        {
            if (m_btHorse == 0)
                return BodySurface is { } s ? s.Width : 48;
            return HorseSurface is { } h ? h.Width : 100;
        }
    }

    /// <summary>Actor.pas 5611-5625 CharHeight 1:1（步行 70 缺省 / 骑马 70 缺省）。</summary>
    public int CharHeight
    {
        get
        {
            if (m_btHorse == 0)
                return BodySurface is { } s ? s.Height : 70;
            return HorseSurface is { } h ? h.Height : 70;
        }
    }

    /// <summary>Actor.pas 5627-5650 CheckSelect 1:1（十字五像素 alpha 命中）。</summary>
    public bool CheckSelect(int dx, int dy)
    {
        var surf = SelectSurfaceForSelect;
        if (surf is not { } s)
            return false;
        var probe = CheckTextureAlpha;
        if (probe == null)
            return false;
        return probe(s, dx, dy) && probe(s, dx - 1, dy) && probe(s, dx + 1, dy)
            && probe(s, dx, dy - 1) && probe(s, dx, dy + 1);
    }
}

/// <summary>纹理尺寸（GameCanvas TTexture.Width/Height 的 headless 镜像）。</summary>
public readonly record struct SurfaceSize(int Width, int Height);
