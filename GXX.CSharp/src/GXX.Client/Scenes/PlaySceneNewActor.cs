using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// PlayScn.pas 6969-7486 NewActor 1:1（批次J72）：
/// 换图守卫 → 既有角色复位复用 → 英雄槽位复用 → 人物/怪物形象字段装载 →
/// wRaceImg 分派建类（简装替换与自定义怪查找）→ 字段写入 → DoAddActor + 魔法锁定修正。
/// </summary>
public partial class TPlayScene
{
    /// <summary>
    /// g_CustomMonsterConfig 接缝（外观 → 自定义怪配置；null = 未配置）。
    /// 与 <see cref="TActorCore.CustomMonsterConfigResolver"/>（ActorMotion.cs:180）同型，
    /// 以便 TCustomActor.Config 直接用协议结构体 TClientCustomMonsterConfig。
    /// </summary>
    public Func<int, TClientCustomMonsterConfig?>? CustomMonsterConfigResolver;

    /// <summary>自定义怪缺配置提示接缝（DScreen.AddChatBoardString(SCustomMonNoConfig, 外观)）。</summary>
    public Action<int>? OnCustomMonMissingConfig;

    /// <summary>IsChangingFace(chrid) 接缝（换脸中禁止新建）。</summary>
    public Func<long, bool>? IsChangingFaceFn;

    /// <summary>LoadSurface(Actor) 调用计数（headless 语义位）。</summary>
    public int LoadSurfaceCount;

    /// <summary>PlayScn.pas 6969-7486 NewActor 主体 1:1。</summary>
    public TActorCore? NewActor(long chrid, int cx, int cy, int cdir, TFeature feature, long cState,
        bool boLockList = true)
    {
        if (Map.MapMoving)
            return null; // 正在更换地图禁止创建角色

        var found = FindActorList(chrid);
        bool boCreate = true;
        if (found != null)
        {
            found.m_boDelActor = false;
            return found;   // 已存在角色：复位删除标记后直接返回（原文 6988-6993）
        }

        var actor = G.MyHero;
        if (boCreate && actor != null && actor.m_nRecogId == chrid)
        {
            actor.m_boDelActor = false;
            actor.m_nRecogId = chrid;
            actor.m_nCurrX = cx;
            actor.m_nCurrY = cy;
            actor.m_nRx = cx;
            actor.m_nRy = cy;
            actor.m_btDir = (byte)cdir;
            actor.m_btRace = 1;

            var hf = feature.HumFeature();
            actor.m_HumFeature = hf;
            actor.m_nChangeAppr = hf.nChangeAppr;
            actor.m_wAppearance = hf.nChangeAppr >= 0 ? (ushort)hf.nChangeAppr : (ushort)0;
            actor.m_btHorse = hf.btHorseType;
            actor.m_btDoubleHumHorse = hf.btDoubleHumHorseType;
            actor.m_boShowHorseWingsEffect = hf.boShowHorseWingsEffect;
            actor.m_btHorseHum = hf.btHorseHum;
            actor.m_btHorseHumExpand = hf.btHorseHumExpand;
            actor.m_btHorseHair = hf.btHorseHair;
            actor.m_btHorseEffectType = hf.btHorseEffectType;
            actor.m_boShowFashion = hf.boShowFashion;
            actor.m_boMagicShield = hf.boMagicShield;
            actor.m_btReLevel = hf.btReLevel;
            actor.m_btSex = hf.btGender;
            actor.m_btJob = hf.btJob;
            actor.m_btHair = hf.btHair;
            actor.m_wDress = hf.wDress;
            actor.m_wWeapon = hf.wWeapon;
            actor.m_wWeaponSound = hf.wWeaponSound;
            actor.m_wEffect = hf.wDressEffType;
            actor.m_wEffect_30 = hf.wDressEffType_30;
            actor.m_boEffectNormalDraw = hf.boDressEffNormalDraw;
            actor.m_boEffect_30NormalDraw = hf.boDressEff_30NormalDraw;
            actor.m_boDressEffNoSex = hf.boDressEffNoSex;
            actor.m_boDressEff_30NoSex = hf.boDressEff_30NoSex;
            actor.m_wShield = hf.wShield;
            actor.m_btOldHair = hf.btOldHair;
            actor.m_boPlayMoster = hf.boPlayMoster;
            actor.m_btCaseltGuild = hf.btCaseltGuild;
            actor.m_nWeaponEffectIndex = hf.nWeaponEffectIndex;
            actor.m_wDBWeaponEffectOffSet = hf.wDBWeaponEffectOffSet;
            actor.m_wWeaponEffectOffSet = hf.wWeaponEffectOffSet;
            actor.m_nDressEffectIndex = hf.nDressEffectIndex;
            actor.m_wDressEffectOffSet = hf.wDressEffectOffSet;
            actor.m_nShieldEffectIndex = hf.nShieldEffectIndex;
            actor.m_wShieldEffectOffSet = hf.wShieldEffectOffSet;
            actor.m_boShopStall = hf.boShopStall;
            actor.m_boDressEffectNoBlend = hf.boDressEffectNoBlend;
            actor.m_boDressEffectNoSex = hf.boDressEffectNoSex;
            actor.m_nMedalEffectIndex = hf.nMedalEffectIndex;
            actor.m_wMedalEffectOffSet = hf.wMedalEffectOffSet;
            actor.m_boMedalEffectNoBlend = hf.boMedalEffectNoBlend;
            actor.m_boMedalEffectNoSex = hf.boMedalEffectNoSex;
            actor.m_boWeaponEffectNoBlend = hf.boWeaponEffectNoBlend;
            actor.m_boWeaponEffectNoSex = hf.boWeaponEffectNoSex;
            actor.m_boShieldEffectNoBlend = hf.boShieldEffectNoBlend;
            actor.m_boShieldEffectNoSex = hf.boShieldEffectNoSex;
            actor.m_boShowHair = hf.boShowHair;
            actor.m_nDressAddEffectIndex = hf.nDressAddEffectIndex;
            actor.m_btCboDressUseDiyImage = hf.btCboDressUseDiyImage;
            actor.m_btCboWeaponUseDiyImage = hf.btCboWeaponUseDiyImage;
            actor.m_sActiveFengHaoName = hf.sActiveFengHaoName;
            actor.m_nActiveFengHaoID = hf.nActiveFengHaoID;
            actor.m_dwActiveFengHaoLooks = hf.dwActiveFengHaoLooks;
            actor.m_btActiveFengHaoReserved = hf.btActiveFengHaoReserved;
            actor.m_nActiveFengHaoColor = hf.btActiveFengHaoColor;
            actor.m_btBodyColor = hf.btBodyColor;
            actor.m_nState = (int)cState;
            actor.MsgList.Clear();
            LoadSurfaceCount++;

            DoAddActor(actor);
            return actor;
        }

        if (boCreate && (IsChangingFaceFn?.Invoke(chrid) ?? false))
            boCreate = false;

        if (boCreate)
        {
            var mf = feature.MonFeature();
            int raceImg = mf.wRaceImg;
            int race = mf.btRace;

            // 简装替换（7129-7170）
            if (!IsGuardExemptRace(race, raceImg))
            {
                if (mf.HumBBType == THumBBType.bbNo)
                {
                    if (!mf.IsDisableSimpleActor
                        && G.boSimpleShowActor && G.ckSimpleShowActor)
                    {
                        if (!G.boCustomActorSimpleShow)
                        {
                            mf.wRaceImg = 18;
                            mf.wAppr = 27;
                            mf.btRace = 83;
                        }
                        else
                        {
                            mf.wRaceImg = (ushort)G.nSimpleActorRaceImg;
                            mf.wAppr = (ushort)G.nSimpleActorAppr;
                            mf.btRace = (byte)G.nSimpleActorRace;
                        }
                    }
                }
                else
                {
                    if (G.boSimpleShowBB && G.ckSimpleShowBB)
                    {
                        if (!G.boCustomBBSimpleShow)
                        {
                            mf.wRaceImg = 18;
                            mf.wAppr = 27;
                            mf.btRace = 83;
                        }
                        else
                        {
                            mf.wRaceImg = (ushort)G.nSimpleBBSimpleRaceImg;
                            mf.wAppr = (ushort)G.nSimpleBBAppr;
                            mf.btRace = (byte)G.nSimpleBBRace;
                        }
                    }
                }
            }

            actor = CreateActorForCustomMon(mf.wRaceImg, mf, out bool customMissing);
            if (customMissing)
            {
                OnCustomMonMissingConfig?.Invoke(mf.wAppr);
                actor = new TActor();
            }
        }
        else
            actor = new TActor();

        actor.m_nRecogId = chrid;
        actor.m_nCurrX = cx;
        actor.m_nCurrY = cy;
        actor.m_nRx = cx;
        actor.m_nRy = cy;
        actor.m_btDir = (byte)cdir;

        var mon = feature.MonFeature();
        actor.m_MonFeature = mon;
        actor.m_btRace = (byte)mon.wRaceImg;
        actor.m_btRealRace = mon.btRace;
        actor.m_HumsBBType = mon.HumBBType;
        actor.m_IsExploreItem = mon.IsExploreItem;

        if (actor.m_btRace is 0 or 1)
        {
            var hf = feature.HumFeature();
            actor.m_HumFeature = hf;
            actor.m_nChangeAppr = hf.nChangeAppr;
            actor.m_wAppearance = hf.nChangeAppr >= 0 ? (ushort)hf.nChangeAppr : (ushort)0;
            actor.m_btHorse = hf.btHorseType;
            actor.m_btDoubleHumHorse = hf.btDoubleHumHorseType;
            actor.m_boShowHorseWingsEffect = hf.boShowHorseWingsEffect;
            actor.m_btHorseHum = hf.btHorseHum;
            actor.m_btHorseHumExpand = hf.btHorseHumExpand;
            actor.m_btHorseHair = hf.btHorseHair;
            actor.m_btHorseEffectType = hf.btHorseEffectType;
            actor.m_boShowFashion = hf.boShowFashion;
            actor.m_boMagicShield = hf.boMagicShield;
            actor.m_btReLevel = hf.btReLevel;
            actor.m_btSex = hf.btGender;
            actor.m_btJob = hf.btJob;
            actor.m_btHair = hf.btHair;
            actor.m_wDress = hf.wDress;
            actor.m_wWeapon = hf.wWeapon;
            actor.m_wWeaponSound = hf.wWeaponSound;
            actor.m_wEffect = hf.wDressEffType;
            actor.m_wEffect_30 = hf.wDressEffType_30;
            actor.m_wShield = hf.wShield;
            actor.m_boEffectNormalDraw = hf.boDressEffNormalDraw;
            actor.m_boEffect_30NormalDraw = hf.boDressEff_30NormalDraw;
            actor.m_boDressEffNoSex = hf.boDressEffNoSex;
            actor.m_boDressEff_30NoSex = hf.boDressEff_30NoSex;
            actor.m_btOldHair = hf.btOldHair;
            actor.m_boPlayMoster = hf.boPlayMoster;
            actor.m_btCaseltGuild = hf.btCaseltGuild;
            actor.m_nWeaponEffectIndex = hf.nWeaponEffectIndex;
            actor.m_wDBWeaponEffectOffSet = hf.wDBWeaponEffectOffSet;
            actor.m_wWeaponEffectOffSet = hf.wWeaponEffectOffSet;
            actor.m_nDressEffectIndex = hf.nDressEffectIndex;
            actor.m_wDressEffectOffSet = hf.wDressEffectOffSet;
            actor.m_nShieldEffectIndex = hf.nShieldEffectIndex;
            actor.m_wShieldEffectOffSet = hf.wShieldEffectOffSet;
            actor.m_boShopStall = hf.boShopStall;
            actor.m_btBodyColor = hf.btBodyColor;
            actor.m_boDressEffectNoBlend = hf.boDressEffectNoBlend;
            actor.m_boDressEffectNoSex = hf.boDressEffectNoSex;
            actor.m_nMedalEffectIndex = hf.nMedalEffectIndex;
            actor.m_wMedalEffectOffSet = hf.wMedalEffectOffSet;
            actor.m_boMedalEffectNoBlend = hf.boMedalEffectNoBlend;
            actor.m_boMedalEffectNoSex = hf.boMedalEffectNoSex;
            actor.m_btCboDressUseDiyImage = hf.btCboDressUseDiyImage;
            actor.m_btCboWeaponUseDiyImage = hf.btCboWeaponUseDiyImage;
            actor.m_boWeaponEffectNoBlend = hf.boWeaponEffectNoBlend;
            actor.m_boWeaponEffectNoSex = hf.boWeaponEffectNoSex;
            actor.m_boShieldEffectNoBlend = hf.boShieldEffectNoBlend;
            actor.m_boShieldEffectNoSex = hf.boShieldEffectNoSex;
            actor.m_boShowHair = hf.boShowHair;
            actor.m_nDressAddEffectIndex = hf.nDressAddEffectIndex;
            actor.m_sActiveFengHaoName = hf.sActiveFengHaoName;
            actor.m_nActiveFengHaoID = hf.nActiveFengHaoID;
            actor.m_dwActiveFengHaoLooks = hf.dwActiveFengHaoLooks;
            actor.m_btActiveFengHaoReserved = hf.btActiveFengHaoReserved;
            actor.m_nActiveFengHaoColor = hf.btActiveFengHaoColor;

            actor.m_HumsBBType = THumBBType.bbNo; // 7411：人物强制 bbNo
        }
        else
        {
            actor.m_btRace = (byte)mon.wRaceImg;
            actor.m_wWeapon = 0;
            actor.m_wWeaponSound = 0;
            actor.m_wAppearance = mon.wAppr;
            actor.m_wEffect = mon.wWeapon;
            actor.m_btBodyColor = mon.btBodyColor;
            actor.m_boShowHair = true;
            actor.m_nChangeAppr = mon.nChangeAppr;

            if (mon.MonLevel > 0)
                actor.m_nLevel = (int)mon.MonLevel;
        }

        actor.m_Action = null;
        if (actor.m_btRace is not (0 or 1))
            actor.m_btSex = 0;
        actor.m_nState = (int)cState;
        actor.MsgList.Clear();
        LoadSurfaceCount++;

        DoAddActor(actor);

        if (chrid == G.g_nMagicTargetRecogId)
        {
            G.g_nMagicTargetRecogId = 0;
            if (G.g_MagicLockActor == null)
                G.g_MagicLockActor = actor;
        }

        return actor;
    }

    /// <summary>7129-7137 守卫豁免简装判定：race 属守卫族/练功师/沙巴克门墙时，RaceImg ∈ {0,1,50} 豁免简装。</summary>
    public static bool IsGuardExemptRace(int race, int raceImg)
    {
        if (!(IsGuardRace(race) || race == 55 || race is 110 or 111))
            return false;
        return raceImg is 0 or 1 or 50;
    }

    /// <summary>wRaceImg 分派建类表 1:1（7172-7306）。customMissing = 自定义怪缺配置。</summary>
    internal static TActorCore CreateActorByRaceImg(int raceImg, TMonFeature feature, out bool customMissing)
    {
        customMissing = false;
        switch (raceImg)
        {
            case 0: return new THumActor();
            case 1: return new THeroActor();
            case 9: return new TSoccerBall();
            case 13: return new TKillingHerb();
            case 14: return new TSkeletonOma();
            case 15: return new TDualAxeOma();
            case 16: return new TGasKuDeGi();
            case 17: return new TCatMon();
            case 18: return new THuSuABi();
            case 19: return new TCatMon();
            case 20: return new TFireCowFaceMon();
            case 21: return new TCowFaceKing();
            case 22: return new TDualAxeOma();
            case 23: return new TWhiteSkeleton();
            case 24: return new TSuperiorGuard();
            case 30: return new TCatMon();
            case 31: return new TCatMon();
            case 32: return new TScorpionMon();
            case 33: return new TCentipedeKingMon();
            case 34: return new TBigHeartMon();
            case 35: return new TSpiderHouseMon();
            case 36: return new TExplosionSpider();
            case 37: return new TFlyingSpider();
            case 40: return new TZombiLighting();
            case 41: return new TZombiDigOut();
            case 42: return new TZombiZilkin();
            case 43: return new TBeeQueen();
            case 45:
            case 210:
            case 224: return new TArcherMon();
            case 47:
            case 48: return new TSculptureMon();
            case 49: return new TSculptureKingMon();
            case 50:
                return feature.wAppr == 273 ? new TStatuaryNpcActor() : new TNpcActor();
            case 52:
            case 53: return new TGasKuDeGi();
            case 54: return new TSmallElfMonster();
            case 55: return new TWarriorElfMonster();
            case 56: return new TMoonMon();
            case 60: return new TElectronicScolpionMon();
            case 61: return new TBossPigMon();
            case 62: return new TKingOfSculpureKingMon();
            case 63: return new TSkeletonKingMon();
            case 64: return new TGasKuDeGi();
            case 65: return new TSamuraiMon();
            case 66:
            case 67:
            case 68: return new TSkeletonSoldierMon();
            case 69: return new TSkeletonArcherMon();
            case 70:
            case 71:
            case 72: return new TBanyaGuardMon();
            case 73: return new TPBOMA1Mon();
            case 74: return new TCatMon();
            case 75:
            case 77: return new TStoneMonster();
            case 76: return new TSuperiorGuard();
            case 78: return new TBanyaGuardMon();
            case 79: return new TPBOMA6Mon();
            case 80: return new TMineMon();
            case 81: return new TAngel();
            case 83: return new TFireDragon();
            case 84: return new TDragonStatue();
            case 90: return new TDragonBody();
            case 95: return new TFireDragonGuard();
            case 98: return new TWallStructure();
            case 99: return new TCastleDoor();
            case 100: return new TMon23_1();
            case 101:
            case 103:
            case 104:
            case 105:
            case 115:
            case 116: return new TMonEffect();
            case 102: return new TMon24_4();
            case 106: return new TMon26_6();
            case 107: return new TMon27_3();
            case 108:
            case 109:
            case 110:
            case 350: return new TDragonBallMonster();
            case 111: return new TFireSpiritMonster();
            case 112: return new TEggMonster();
            case 113: return new TLionMonster();
            case 114: return new TMon35FireDragon();
            case 117: return new TMon27_6();
            case 118: return new TMon29_0();
            case 156:
                if (feature.btRace is 154 or 155 or 156 or 157)
                {
                    // 自定义怪：配置存在 → TCustomActor，否则提示后 TActor
                    return new TCustomActor();
                }
                return new TActor();
            case 200: return new TMonKuLou_1();
            case 201: return new TMon27_3();
            case 202:
            case 203:
            case 204:
            case 205:
            case 206:
            case 207:
            case 208:
            case 209: return new TMon36_X();
            case 220:
            case 221:
            case 222:
            case 223:
            case 225: return new TMon38_X();
            case 253: return new TXueLingLeaderMonster();
            case 254: return new TSkeletonOma();
            case 255: return new TWarriorElfMonster();
            default: return new TActor();
        }
    }

    /// <summary>自定义怪分派（156 + btRace 154..157）：按 wAppr 查 g_CustomMonsterConfig。</summary>
    internal TActorCore CreateActorForCustomMon(int raceImg, TMonFeature feature, out bool customMissing)
    {
        customMissing = false;
        if (raceImg == 156 && feature.btRace is 154 or 155 or 156 or 157)
        {
            var cfg = CustomMonsterConfigResolver?.Invoke(feature.wAppr);
            if (cfg.HasValue)
                return new TCustomActor(cfg.Value);
            customMissing = true;
        }
        return CreateActorByRaceImg(raceImg, feature, out _);
    }
}

/// <summary>TFeature 缓冲读取（Delphi pTHumFeature/pTMonFeature 的 headless 等价：按需构造）。</summary>
public static class FeatureReader
{
    public static THumFeature HumFeature(this TFeature f)
    {
        if (f._hum != null)
            return f._hum;
        f._hum = new THumFeature();
        return f._hum;
    }

    public static void SetHumFeature(this TFeature f, THumFeature hum) => f._hum = hum;

    public static TMonFeature MonFeature(this TFeature f)
    {
        if (f._mon != null)
            return f._mon;
        f._mon = new TMonFeature();
        return f._mon;
    }

    public static void SetMonFeature(this TFeature f, TMonFeature mon) => f._mon = mon;
}

/// <summary>
/// TActor 默认实现（Actor.pas TActor 的 headless 承载类）。
///
/// <para><b>车道 p7-client-virtual</b>：在此**新增** 4 个虚成员（原文 TActor 的虚方法表成员，
/// 此前托管侧基类**没有**这 4 个名字，故 TCustomActor 的同名实现被基类静态类型调用点整段旁路）。</para>
///
/// <para><b>★ 车道 p7-client-actor-family</b>：那 4 个成员**当时是空实现**（只补了分派槽位）。
/// 本车道把原文本体落进 <c>Scenes/ActorFamilyBase.cs</c>（<c>partial class TActor</c>，
/// 与本类**同一个类**的两半），并据此做了两处机械改动：</para>
/// <list type="number">
/// <item>本类声明加 <c>partial</c>（与 <c>TCreature</c>/<c>TPlayObject</c> 一致 ——
///   "用自有文件补成员"是本工程消除"两个写者"风险的既定手法）；</item>
/// <item>删除原先那 4 个**空虚成员**（<c>LoadSurface()</c> / <c>DrawChr(int,int,bool,bool)</c> /
///   <c>RunSound()</c> / <c>RunActSound(int)</c>），实体改由 <c>ActorFamilyBase.cs</c> 提供。</item>
/// </list>
/// <para>另：原文 `TActor.DrawStateEffSurface`（Actor.pas 5654-5702）与本类的
/// <c>DrawChr</c> 同族，一并在 <c>ActorFamilyBase.cs</c> 落地。</para>
/// </summary>
public partial class TActor : TActorCore
{
}

// ★ 车道 p17-client-actor：原在此处的 TActor.pas 五个实体类的「空壳」已**迁走** ——
//   THumActor        → Scenes/ActorHumActor.cs      （原文 Actor.pas 1935 / 11130-17436）
//   THeroActor       → Scenes/ActorHeroActor.cs     （原文 Actor.pas 2049 / 17437-17533，基类更正为 THumActor）
//   TNpcActor        → Scenes/ActorNpcActor.cs      （原文 Actor.pas 1877 /  9936-11122）
//   TStatuaryNpcActor→ Scenes/ActorStatuaryNpc.cs   （原文 Actor.pas 1903 / 17537-18008，基类 TNpcActor）
//   TActor           → Scenes/ActorFamilyBase.cs 等 （partial，原本就在 Actor* 文件里）
//   迁走的原因：这些类名原本是从 **PlayScn.pas 的 wRaceImg 分派表**照抄来的，
//   与 Actor.pas 的同名类只是「同名」，实现为空 —— 详见
//   docs/并行报告-p12-e2only-review.md §5.2/§5.3。

// TCustomActor（自定义怪）已 1:1 移植到独立文件 Scenes/CustomActor.cs
// （源单元 Source\Client-HGE\CustomActor.pas 1,130 行）。此处原为 6 行桩，已删除以免重名。

// ---- wRaceImg 分派表承载类（Class name 与 Delphi 1:1；行为随后续批次深化） ----
// ★ 车道 p7-client-actor-family：HerbActor.pas 族（TKillingHerb/TMineMon/TBeeQueen/
//   TCentipedeKingMon/TBigHeartMon/TSpiderHouseMon/TDragonBody）与结构族
//   （TWallStructure/TNewWallStructure/TCastleDoor）的类头改为 `partial`，
//   1:1 方法体落在 **本车道自有文件** Scenes/ActorFamilyHerb*.cs
//   （与 Scenes/ActorFamilyStructures.cs）。**基类关系一并按原文更正** ——
//   原文层级是 TKillingHerb : TActor，而 TMineMon/TCentipedeKingMon/TBigHeartMon/
//   TSpiderHouseMon/TDragonBody **都** : TKillingHerb（若继续平铺为 : TActor，
//   `inherited` 会整段绕过 TKillingHerb 的覆写 —— 属"看起来一样实则不同"的典型）。

public class TSoccerBall : TActor { public override string ActorClass => "TSoccerBall"; }
public partial class TKillingHerb : TActor { public override string ActorClass => "TKillingHerb"; }
public class TSkeletonOma : TActor { public override string ActorClass => "TSkeletonOma"; }
public class TDualAxeOma : TActor { public override string ActorClass => "TDualAxeOma"; }
public class TGasKuDeGi : TActor { public override string ActorClass => "TGasKuDeGi"; }
public class TCatMon : TActor { public override string ActorClass => "TCatMon"; }
public class THuSuABi : TActor { public override string ActorClass => "THuSuABi"; }
public class TFireCowFaceMon : TActor { public override string ActorClass => "TFireCowFaceMon"; }
public class TCowFaceKing : TActor { public override string ActorClass => "TCowFaceKing"; }
public class TWhiteSkeleton : TActor { public override string ActorClass => "TWhiteSkeleton"; }
public class TSuperiorGuard : TActor { public override string ActorClass => "TSuperiorGuard"; }
public class TScorpionMon : TActor { public override string ActorClass => "TScorpionMon"; }
public partial class TCentipedeKingMon : TKillingHerb { public override string ActorClass => "TCentipedeKingMon"; }
public partial class TBigHeartMon : TKillingHerb { public override string ActorClass => "TBigHeartMon"; }
public partial class TSpiderHouseMon : TKillingHerb { public override string ActorClass => "TSpiderHouseMon"; }
public class TExplosionSpider : TActor { public override string ActorClass => "TExplosionSpider"; }
public class TFlyingSpider : TActor { public override string ActorClass => "TFlyingSpider"; }
public class TZombiLighting : TActor { public override string ActorClass => "TZombiLighting"; }
public class TZombiDigOut : TActor { public override string ActorClass => "TZombiDigOut"; }
public class TZombiZilkin : TActor { public override string ActorClass => "TZombiZilkin"; }
public partial class TBeeQueen : TActor { public override string ActorClass => "TBeeQueen"; }
public class TArcherMon : TActor { public override string ActorClass => "TArcherMon"; }
public class TSculptureMon : TActor { public override string ActorClass => "TSculptureMon"; }
public class TSculptureKingMon : TActor { public override string ActorClass => "TSculptureKingMon"; }
// TNpcActor / TStatuaryNpcActor 的空壳已迁至 Scenes/ActorNpcActor.cs / ActorStatuaryNpc.cs
// （见上方 p17-client-actor 迁移说明）。
public class TSmallElfMonster : TActor { public override string ActorClass => "TSmallElfMonster"; }
public class TWarriorElfMonster : TActor { public override string ActorClass => "TWarriorElfMonster"; }
public class TMoonMon : TActor { public override string ActorClass => "TMoonMon"; }
public class TElectronicScolpionMon : TActor { public override string ActorClass => "TElectronicScolpionMon"; }
public class TBossPigMon : TActor { public override string ActorClass => "TBossPigMon"; }
public class TKingOfSculpureKingMon : TActor { public override string ActorClass => "TKingOfSculpureKingMon"; }
public class TSkeletonKingMon : TActor { public override string ActorClass => "TSkeletonKingMon"; }
public class TSamuraiMon : TActor { public override string ActorClass => "TSamuraiMon"; }
public class TSkeletonSoldierMon : TActor { public override string ActorClass => "TSkeletonSoldierMon"; }
public class TSkeletonArcherMon : TActor { public override string ActorClass => "TSkeletonArcherMon"; }
public class TBanyaGuardMon : TActor { public override string ActorClass => "TBanyaGuardMon"; }
public class TPBOMA1Mon : TActor { public override string ActorClass => "TPBOMA1Mon"; }
public class TPBOMA6Mon : TActor { public override string ActorClass => "TPBOMA6Mon"; }
public class TStoneMonster : TActor { public override string ActorClass => "TStoneMonster"; }
public partial class TMineMon : TKillingHerb { public override string ActorClass => "TMineMon"; }
public class TAngel : TActor { public override string ActorClass => "TAngel"; }
public class TFireDragon : TActor { public override string ActorClass => "TFireDragon"; }
public class TDragonStatue : TActor { public override string ActorClass => "TDragonStatue"; }
public partial class TDragonBody : TKillingHerb { public override string ActorClass => "TDragonBody"; }
public class TFireDragonGuard : TActor { public override string ActorClass => "TFireDragonGuard"; }
public partial class TWallStructure : TActor { public override string ActorClass => "TWallStructure"; }
public partial class TCastleDoor : TActor { public override string ActorClass => "TCastleDoor"; }
public partial class TNewWallStructure : TActor { public override string ActorClass => "TNewWallStructure"; }
public class TMon23_1 : TActor { public override string ActorClass => "TMon23_1"; }
public class TMon24_4 : TActor { public override string ActorClass => "TMon24_4"; }
public class TMon26_6 : TActor { public override string ActorClass => "TMon26_6"; }
public class TMon27_3 : TActor { public override string ActorClass => "TMon27_3"; }
public class TMon27_6 : TActor { public override string ActorClass => "TMon27_6"; }
public class TMon29_0 : TActor { public override string ActorClass => "TMon29_0"; }
public class TMonEffect : TActor { public override string ActorClass => "TMonEffect"; }
public class TDragonBallMonster : TActor { public override string ActorClass => "TDragonBallMonster"; }
public class TFireSpiritMonster : TActor { public override string ActorClass => "TFireSpiritMonster"; }
public class TEggMonster : TActor { public override string ActorClass => "TEggMonster"; }
public class TLionMonster : TActor { public override string ActorClass => "TLionMonster"; }
public class TMon35FireDragon : TActor { public override string ActorClass => "TMon35FireDragon"; }
public class TMonKuLou_1 : TActor { public override string ActorClass => "TMonKuLou_1"; }
public class TMon36_X : TActor { public override string ActorClass => "TMon36_X"; }
public class TMon38_X : TActor { public override string ActorClass => "TMon38_X"; }
public class TXueLingLeaderMonster : TActor { public override string ActorClass => "TXueLingLeaderMonster"; }
