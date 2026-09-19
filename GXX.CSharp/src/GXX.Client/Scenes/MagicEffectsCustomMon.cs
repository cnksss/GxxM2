using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// magiceff.pas 自定义怪物特效两大类（批次J71，3577-4193）：
/// TCustomMonFlyEffect（飞行-爆燃双阶段 + 地狱火模式火节点队列 + TigerOnExplosion/OnFinished 事件 +
/// FlyEff/Explosion 双图库与 DrawMode + ExplosionLockTarget）与 TCustomMonTargetEffect（目标点双层爆燃 +
/// FTigerOnFinished 末两帧事件 + MagicId=66 边界修正）。DrawEff 以 ComputeDrawLayers 产出。
/// </summary>
public static class CustomMonEffConsts
{
    public const int MdmBlend = 0;  // TCustomDrawMode.mdmBlend（枚举首项）
    public const int MdmNormal = 1; // TCustomDrawMode.mdmNormal
}

/// <summary>TCustomMonFlyEffect（自定义怪飞行特效：飞行双图库 → 固定态爆炸双段 + 事件钩子；地狱火模式独立 Run/Draw）。</summary>
public class TCustomMonFlyEffect : TMagicEff
{
    public int MagExplosionBase2 = -1;
    public bool MagicBlend2 = true;
    public int FlyEffImgLibId = -1;       // FlyEffImgLib（nil → -1）
    public int FlyEffStartIndex = -1;
    public int FlyDrawMode = CustomMonEffConsts.MdmBlend;
    public int FlyEffDrawMode = CustomMonEffConsts.MdmBlend;
    public int NextExplosionFrameTime;
    public bool ExplosionLockTarget;
    public byte FlyLightRange;
    public byte ExplosionLightRange;
    public long LockTarget;
    public int LockTargetX, LockTargetY;
    /// <summary>ClientConfig 集成位（TMagicClientConfig 引用；后续自定义技能批次接入）。</summary>
    public object? ClientConfig;
    /// <summary>FTargetList（目标 recogid 列表）。</summary>
    public readonly List<long> TargetList = new();

    public bool IsFireGunMode;
    public bool OutofOil;
    public uint Firetime;
    public TFireNode[] FireNodes = Array.Empty<TFireNode>();
    public bool IsPlaySound;
    public bool TigerOnExplosion;
    public bool TigerOnFinished;
    public int ExplosionImgLibId = -1;    // FExplosionImgLib（nil → -1）

    public Action? OnExplosion;
    public Action? OnFinished;

    /// <summary>MagOwner 坐标接缝（TActor(MagOwner).m_nRx/Ry）。</summary>
    public IMagicTarget? MagOwnerPosition;
    /// <summary>MagOwner 爆炸音效 id 接缝（TActor(MagOwner).m_nMagicExplosionSound）。</summary>
    public Func<int>? MagOwnerExplosionSound;
    /// <summary>爆炸音播放记录（g_PlaySound.PlaySound headless）。</summary>
    public List<int> PlayedSounds { get; } = new();

    public TCustomMonFlyEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target,
        int flyFrameCount, int explosionImgLibId, bool explosionLockTarget, bool isFireGunMode)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = flyFrameCount;
        TargetActor = target;
        NextFrameTime = 50;
        MagicBlend = true;
        ExplosionImgLibId = explosionImgLibId;
        ExplosionLockTarget = explosionLockTarget;

        FlyEffImgLibId = -1;
        FlyEffStartIndex = -1;
        FlyDrawMode = CustomMonEffConsts.MdmBlend;
        FlyEffDrawMode = CustomMonEffConsts.MdmBlend;

        if (flyFrameCount > 2)
        {
            IsFireGunMode = isFireGunMode;
            FireNodes = new TFireNode[flyFrameCount];
            OutofOil = false;
            Firetime = Tick();

            // 不把这个去掉，一大块全堆在目标身上 chongchong 2016-08-19
            if (IsFireGunMode)
                TargetActor = null;
        }

        IsPlaySound = false;
    }

    public override bool Shift() => base.Shift();

    /// <summary>Run（3646-3698）：地狱火模式独立节点队列推进；否则基类 Run + 固定态首拍爆炸音。</summary>
    public override bool Run()
    {
        if (IsFireGunMode)
        {
            bool result = true;
            bool boLoadSurface = false;

            if (Tick() - steptime > (uint)NextFrameTime)
            {
                Shift();
                boLoadSurface = true;
                steptime = Tick();
                if (!OutofOil)
                {
                    if (MagOwner != null && (Math.Abs(rx - (MagOwnerPosition?.Rx ?? rx)) >= 5 ||
                        Math.Abs(ry - (MagOwnerPosition?.Ry ?? ry)) >= 5 || Tick() - Firetime > 800))
                        OutofOil = true;
                    for (int i = FireNodes.Length - 2; i >= 0; i--)
                    {
                        FireNodes[i].firenumber++;
                        FireNodes[i + 1] = FireNodes[i];
                    }
                    FireNodes[0].firenumber = 1;
                    FireNodes[0].X = FlyX;
                    FireNodes[0].Y = FlyY;
                }
                else
                {
                    bool allgone = true;
                    for (int i = FireNodes.Length - 2; i >= 0; i--)
                    {
                        if (FireNodes[i].firenumber <= FireNodes.Length)
                        {
                            FireNodes[i].firenumber++;
                            FireNodes[i + 1] = FireNodes[i];
                            allgone = false;
                            boLoadSurface = false;
                        }
                    }
                    if (allgone) result = false;
                }
            }

            if (boLoadSurface)
                SurfaceReloads++;
            return result;
        }

        var baseResult = base.Run();
        if (FixedEffect && !IsPlaySound)
        {
            IsPlaySound = true;
            if (MagOwner != null)
            {
                PlayedSounds.Add(MagOwnerExplosionSound?.Invoke() ?? 0);
            }
        }
        return baseResult;
    }

    /// <summary>DrawEff（3699-3956）headless：地狱火模式产出节点双图库层；常规产出飞行双层/爆燃双段层。</summary>
    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (IsFireGunMode)
        {
            if (m_boActive && (Math.Abs(FlyX - fireX) > 1 || Math.Abs(FlyY - fireY) > 1))
            {
                if (!FixedEffect || ExplosionImgLibId < 0 || ExplosionFrame == 0)
                {
                    int prx = -1, pry = -1, prx2 = -1, pry2 = -1;
                    for (int i = 0; i < FireNodes.Length; i++)
                    {
                        if (FireNodes[i].firenumber > FireNodes.Length || FireNodes[i].firenumber <= 0)
                            continue;
                        if (FireNodes[i].firenumber < i + 1) // 修正未初始化坐标播放 2019-04-30
                            continue;

                        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
                        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;

                        if (FlyEffImgLibId >= 0 && FlyEffStartIndex >= 0)
                        {
                            int img = FlyEffStartIndex + FireNodes[i].firenumber - 1;
                            int fx = FireNodes[i].X + px - 24 - shx;
                            int fy = FireNodes[i].Y + py - 16 - shy;
                            if (fx != prx || fy != pry)
                            {
                                prx = fx;
                                pry = fy;
                                layers.Add(new(img, fx, fy, FlyEffDrawMode == CustomMonEffConsts.MdmBlend));
                            }
                        }

                        int img2 = EffectBase + FireNodes[i].firenumber - 1;
                        int fx2 = FireNodes[i].X + px - 24 - shx;
                        int fy2 = FireNodes[i].Y + py - 16 - shy;
                        if (fx2 != prx2 || fy2 != pry2)
                        {
                            prx2 = fx2;
                            pry2 = fy2;
                            layers.Add(new(img2, fx2, fy2, FlyDrawMode == CustomMonEffConsts.MdmBlend));
                        }
                    }
                }
            }
            return layers;
        }

        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return layers;
        int sh = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int sy2 = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            // 飞行特效双图库
            if (FlyEffImgLibId >= 0 && FlyEffStartIndex >= 0)
            {
                layers.Add(new(FlyEffStartIndex + curframe, FlyX + px - 24 - sh, FlyY + py - 16 - sy2,
                    FlyEffDrawMode == CustomMonEffConsts.MdmBlend));
            }
            if (EffectBase >= 0)
            {
                layers.Add(new(EffectBase + curframe, FlyX + px - 24 - sh, FlyY + py - 16 - sy2,
                    FlyDrawMode == CustomMonEffConsts.MdmBlend));
            }
        }
        else
        {
            if (NextFrameTime != NextExplosionFrameTime)
                NextFrameTime = NextExplosionFrameTime;

            if (!TigerOnExplosion)
            {
                OnExplosion?.Invoke();
                TigerOnExplosion = true;
            }

            if ((ExplosionImgLibId < 0 || ExplosionFrame == 0 || curframe >= ExplosionFrame - 2) && !TigerOnFinished)
            {
                OnFinished?.Invoke();
                TigerOnFinished = true;
            }

            if (ExplosionImgLibId >= 0)
            {
                if (MagExplosionBase >= 0)
                {
                    // 爆炸效果不锁定目标 chongchong 2014-10-18
                    if (!ExplosionLockTarget)
                        TargetActor = null;
                    layers.Add(new(MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, MagicBlend));
                }
                if (MagExplosionBase2 >= 0)
                {
                    if (!ExplosionLockTarget)
                        TargetActor = null;
                    layers.Add(new(MagExplosionBase2 + curframe, FlyX + px - 24, FlyY + py - 16, MagicBlend2));
                }
            }
        }
        return layers;
    }
}

/// <summary>TCustomMonTargetEffect（自定义怪目标特效：EffectImg 库双层爆燃 + 末两帧 OnFinished + MagicId=66 修正）。</summary>
public class TCustomMonTargetEffect : TMagicEff
{
    public int MagExplosionBase2 = -1;
    public int DrawMode = CustomMonEffConsts.MdmBlend;
    public int DrawMode2 = CustomMonEffConsts.MdmBlend;
    public readonly List<long> TargetList = new();
    public object? ClientConfig;
    public long LockTarget;
    public int LockX, LockY;

    public bool TigerOnFinished;
    public Action? OnFinished;

    /// <summary>位置版（PlayScene.ScreenXYfromMCXY(nX,nY) 预变换 + targetx/y 记录）。</summary>
    public TCustomMonTargetEffect(int effbase, int effBase2, int effframe, int nX, int nY)
        : base(111, effbase, 0, 0, 0, 0, TMagicType.mtExplosion, false, 0)
    {
        var (newX, newY) = MagicEffEnv.ScreenXYfromMCXY(nX, nY);
        // Delphi 用预变换坐标直接作 sx/sy/tx/ty（覆盖基类参数）
        targetx = newX;
        targety = newY;
        FlyX = newX;
        FlyY = newY;
        rx = newX;
        ry = newY;
        FireMyselfX = newX;
        FireMyselfY = newY;

        MagExplosionBase2 = effBase2;
        ImgLibId = 1; // g_WEffectImg
        TargetActor = null;
        MagExplosionBase = effbase;
        ExplosionFrame = effframe;
        NextFrameTime = 100;
    }

    /// <summary>目标版（构造尾调用 Shift 修正爆炸首帧错位 chongchong 2018-02-12）。</summary>
    public TCustomMonTargetEffect(int effbase, int effBase2, int effframe, IMagicTarget target)
        : base(111, effbase, target.Rx, target.Ry, target.Rx, target.Ry, TMagicType.mtExplosion, false, 0)
    {
        MagExplosionBase2 = effBase2;
        ImgLibId = 1;
        TargetActor = target;
        MagExplosionBase = effbase;
        ExplosionFrame = effframe;
        NextFrameTime = 100;

        Shift();
    }

    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 15 && Math.Abs(FlyY - fireY) <= 15 && !FixedEffect))
            return layers;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            int img = EffectBase + MagicTailConsts.FlyBase + Dir16 * 10;
            layers.Add(new(img + curframe, FlyX + px - 24 - shx, FlyY + py - 16 - shy,
                DrawMode == CustomMonEffConsts.MdmBlend));
            return layers;
        }

        if (MagExplosionBase >= 0)
        {
            layers.Add(new(MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16,
                DrawMode == CustomMonEffConsts.MdmBlend));
        }
        if (MagExplosionBase2 >= 0)
        {
            layers.Add(new(MagExplosionBase2 + curframe, FlyX + px - 24, FlyY + py - 16,
                DrawMode2 == CustomMonEffConsts.MdmBlend));
        }
        return layers;
    }

    /// <summary>OnFinished 检查（DrawEff 内嵌于固定分支两段各一次；headless 抽为显式调用点）。</summary>
    public void CheckOnFinished()
    {
        if (curframe >= ExplosionFrame - 2 && !TigerOnFinished)
        {
            OnFinished?.Invoke();
            TigerOnFinished = true;
        }
    }

    /// <summary>MagicId=66 边界修正（冰霜雪雨族 py−225/px+25，基类同款）。</summary>
    public void ApplyMagicId66BoundaryFix()
    {
        if (MagicId == 66 && curframe < 20)
        {
            py -= 225;
            px += 25;
        }
    }
}
