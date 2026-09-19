using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// magiceff.pas 特效子类余部第一片（批次J70，18 族；TCustomMonFlyEffect/TCustomMonTargetEffect 两大类留 J71）。
/// DrawEff 以「图号/坐标/混合」产出（ComputeDraw 单主图层；双图层类产出 Layers）。
/// </summary>
public static class MagicTailConsts
{
    public const int FlyBase = MagicEffConsts.FLYBASE;
    public const int FireGunFrameSanYanZhou = 3;

    /// <summary>ClFunc.pas 1496 GetFlyDirection（8 方向，2.5/3 斜率分段）。</summary>
    public static int GetFlyDirection(int sx, int sy, int ttx, int tty)
    {
        double fx = ttx - sx;
        double fy = tty - sy;
        const int DR_UP = 0, DR_UPRIGHT = 1, DR_RIGHT = 2, DR_DOWNRIGHT = 3;
        const int DR_DOWN = 4, DR_DOWNLEFT = 5, DR_LEFT = 6, DR_UPLEFT = 7;
        if (fx == 0)
            return fy < 0 ? DR_UP : DR_DOWN;
        if (fy == 0)
            return fx < 0 ? DR_LEFT : DR_RIGHT;
        if (fx > 0 && fy < 0)
        {
            if (-fy > fx * 2.5) return DR_UP;
            if (-fy < fx / 3) return DR_RIGHT;
            return DR_UPRIGHT;
        }
        if (fx > 0 && fy > 0)
        {
            if (fy < fx / 3) return DR_RIGHT;
            if (fy > fx * 2.5) return DR_DOWN;
            return DR_DOWNRIGHT;
        }
        if (fx < 0 && fy > 0)
        {
            if (fy < -fx / 3) return DR_LEFT;
            if (fy > -fx * 2.5) return DR_DOWN;
            return DR_DOWNLEFT;
        }
        if (-fy > -fx * 2.5) return DR_UP;
        if (-fy < -fx / 3) return DR_LEFT;
        return DR_UPLEFT;
    }
}

/// <summary>绘制层产出（多图层类：Layers；单图层类 Draw/DrawBlend 由 Blend 表达）。</summary>
public readonly record struct EffDrawLayer(int Img, int X, int Y, bool Blend);

/// <summary>TCopySelf（分身：Dir16→8 向 btDir 折叠，img=EffectBase+FLYBASE+btDir×10+curframe）。</summary>
public class TCopySelf : TMagicEff
{
    public TCopySelf(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        int btDir;
        if (!FixedEffect)
        {
            btDir = Dir16 / 2; // 0,1→0 … 14,15→7（越界原文 else btDir := 0 永不到达）
            int img = EffectBase + MagicTailConsts.FlyBase + btDir * 10 + curframe;
            return (img, FlyX + px - 24 - shx, FlyY + py - 16 - shy, true);
        }
        int img2 = MagExplosionBase + curframe;
        return (img2, FlyX + px - 24 - shx, FlyY + py - 16 - shy, true);
    }
}

/// <summary>TFlyingFireBall（火球：门=ReadyFrame、img=FlyImageBase+GetFlyDirection×10+curframe）。</summary>
public class TFlyingFireBall : TFlyingAxe
{
    public TFlyingFireBall(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
    }

    public override bool Run() => base.Run();

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= ReadyFrame && Math.Abs(FlyY - fireY) <= ReadyFrame))
            return null;
        int img = FlyImageBase + MagicTailConsts.GetFlyDirection(FlyX, FlyY, targetx, targety) * 10 + curframe;
        return (img, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TExploBujaukEffect4（4 级灵魂火符：主符 Draw + img+170 副图恒 Blend；WMagic6Images/MagExplosionBase=300）。</summary>
public class TExploBujaukEffect4 : TMagicEff
{
    public bool MagicBlend;

    public TExploBujaukEffect4(int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, 140, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 3;
        TargetActor = target;
        NextFrameTime = 50;
        MagicBlend = false;
        ImgLibId = 6; // g_WMagic6Images
        MagExplosionBase = 300;
    }

    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return layers;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            int img = EffectBase + Dir16 * 10;
            int x = FlyX + px - 24 - shx;
            int y = FlyY + py - 16 - shy;
            layers.Add((img + curframe, x, y, MagicBlend));
            layers.Add((img + 170 + curframe, x, y, true));
        }
        else
        {
            layers.Add((MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, true));
        }
        return layers;
    }
}

/// <summary>TMoonMonEffect（月灵：飞行 img=EffectBase+curframe（无方向）Blend=MagicBlend=true）。</summary>
public class TMoonMonEffect : TMagicEff
{
    public bool MagicBlend;

    public TMoonMonEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 6;
        TargetActor = target;
        NextFrameTime = 50;
        MagicBlend = true;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
            return (EffectBase + curframe, FlyX + px - 24 - shx, FlyY + py - 16 - shy, MagicBlend);
        return (MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TBujaukGroundEffect（群体施毒/幽灵盾/神圣战甲地面符：30ms 节流抵达爆炸 + NewLevel/MagicNumber 分图号）。</summary>
public class TBujaukGroundEffect : TMagicEff
{
    public int MagicNumber;
    public bool BoGroundEffect;
    public int ExplosionSoundCalls; // PlaySound(MagOwner.m_nMagicExplosionSound) 接缝计数

    public TBujaukGroundEffect(int effbase, int magicnumb, int sx, int sy, int tx, int ty)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtBujaukGroundEffect, true, 0)
    {
        frame = 3;
        MagicNumber = magicnumb;
        BoGroundEffect = false;
        NextFrameTime = 50;
    }

    public override bool Run()
    {
        bool result = true;
        if (Tick() - m_dwRunTime > 30 || FixedEffect)
        {
            m_dwRunTime = Tick();
            result = base.Run();
            if (!FixedEffect)
            {
                if ((Math.Abs(targetx - FlyX) <= 15 && Math.Abs(targety - FlyY) <= 15) ||
                    (Math.Abs(targetx - FlyX) >= prevdisx && Math.Abs(targety - FlyY) >= prevdisy))
                {
                    FixedEffect = true;
                    start = 0;
                    frame = ExplosionFrame;
                    curframe = start;
                    Repetition = false;
                    if (MagOwner != null)
                        ExplosionSoundCalls++;
                    SurfaceReloads++;
                    result = true;
                }
                else
                {
                    prevdisx = Math.Abs(targetx - FlyX);
                    prevdisy = Math.Abs(targety - FlyY);
                }
            }
        }
        return result;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            int img = EffectBase + Dir16 * 10 + curframe;
            return (img, FlyX + px - 24 - shx, FlyY + py - 16 - shy, false); // 恒 Draw
        }

        int img2;
        bool useMagic7 = false;
        if (NewLevel == 0 || MagicNumber == 46 || MagicNumber == 67)
        {
            if (MagicNumber == 11)
                img2 = EffectBase + 16 * 10 + curframe;
            else
                img2 = EffectBase + 18 * 10 + curframe;
            if (MagicNumber == 46)
            {
                var baseImg = MagicEffectBaseLookup.ResolveForSpell(MagicNumber, 0, MagicEffEnv.SelfRx, MagicEffEnv.SelfRy);
                img2 = baseImg.BaseIndex + 170 + curframe; // 诅咒术目标物效修改 chongchong 2015-05-23
            }
            else if (MagicNumber == 67)
            {
                var baseImg = MagicEffectBaseLookup.ResolveForSpell(MagicNumber, 0, MagicEffEnv.SelfRx, MagicEffEnv.SelfRy);
                img2 = baseImg.BaseIndex + 10 + curframe; // 新诅咒术 chongchong 2015-07-25
            }
        }
        else
        {
            useMagic7 = true;
            if (NewLevel is >= 1 and <= 3)
                img2 = MagicNumber switch { 11 => 2470 + curframe, 12 => 2410 + curframe, _ => EffectBase + 18 * 10 + curframe };
            else if (NewLevel is >= 4 and <= 6)
                img2 = MagicNumber switch { 11 => 2490 + curframe, 12 => 2430 + curframe, _ => EffectBase + 18 * 10 + curframe };
            else
                img2 = MagicNumber switch { 11 => 2520 + curframe, 12 => 2450 + curframe, _ => EffectBase + 18 * 10 + curframe };
        }

        _ = useMagic7; // 图库差异由 ImgLibId 消费方按 NewLevel 分流；此处只锁图号
        return (img2, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TNormalDrawEffect（普通播放：mtReady、boC8 决定 Blend/Draw、ScreenXYfromMCXY(FlyX,FlyY) 定位）。</summary>
public class TNormalDrawEffect : TMagicEff
{
    public bool boC8;

    public TNormalDrawEffect(int xx, int yy, int imgLibId, int effbase, int nX, uint frmTime, bool boFlag)
        : base(111, effbase, xx, yy, xx, yy, TMagicType.mtReady, true, 0)
    {
        ImgLibId = imgLibId;
        EffectBase = effbase;
        start = 0;
        curframe = 0;
        frame = nX;
        NextFrameTime = (int)frmTime;
        boC8 = boFlag;
    }

    public override bool Run()
    {
        bool result = true;
        m_nCurrentFrame = curframe;
        if (m_boActive && Tick() - steptime > (uint)NextFrameTime)
        {
            steptime = Tick();
            curframe++;
            if (curframe > start + frame - 1)
            {
                curframe = start;
                result = false;
            }
        }
        if (result && m_nCurrentFrame != curframe)
            SurfaceReloads++;
        return result;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        var (nRx, nRy) = MagicEffEnv.ScreenXYfromMCXY(FlyX, FlyY);
        return (EffectBase + curframe, nRx + px - 24, nRy + py - 16, boC8);
    }
}

/// <summary>TBloodBiteEffect（噬血术：NewLevel 三段 MagExplosionBase=690/840/990，curframe≥20 加成段）。</summary>
public class TBloodBiteEffect : TMagicEff
{
    public TBloodBiteEffect(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime, int aNewLevel)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
        NewLevel = aNewLevel;
        TargetActor = null;
        NextFrameTime = 40;
        ExplosionFrame = 20;
        ImgLibId = 9; // g_WMagic9Images
        light = 3;
        MagExplosionBase = NewLevel switch
        {
            >= 1 and <= 3 => 690,
            >= 4 and <= 6 => 840,
            _ => 990,
        };
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        int nIdx;
        if (curframe >= 20)
        {
            nIdx = MagExplosionBase + (NewLevel - 1) * 20 + curframe;
            if (NewLevel is >= 4 and <= 6)
                nIdx = MagExplosionBase + (NewLevel - 4) * 20 + curframe;
            else if (NewLevel >= 7 || NewLevel <= 0)
                nIdx = MagExplosionBase + (NewLevel - 7) * 20 + curframe;
        }
        else
        {
            nIdx = MagExplosionBase + curframe;
        }
        return (nIdx, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TContinuousEffect（连击：EffectNumber=105 飞行无方向项；109/111 固定加 Dir16×10；飞行图号 +2×curframe 原文缺陷保留）。</summary>
public class TContinuousEffect : TMagicEff
{
    public TContinuousEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 3;
        TargetActor = target;
        NextFrameTime = 50;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 15 && Math.Abs(FlyY - fireY) <= 15 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            int img = EffectNumber == 105
                ? EffectBase + MagicTailConsts.FlyBase + curframe
                : EffectBase + Dir16 * 10 + curframe;
            // Delphi 原文：GetCachedGrayImage(img + curframe, ...) → 图号再 +curframe（+2×curframe 缺陷保留）
            return (img + curframe, FlyX + px - 24 - shx, FlyY + py - 16 - shy, true);
        }
        int img2 = EffectNumber switch
        {
            109 => MagExplosionBase + curframe + Dir16 * 10,  // 八卦掌
            111 => MagExplosionBase + curframe + Dir16 * 10,  // 万剑归宗
            _ => MagExplosionBase + curframe,
        };
        return (img2, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TExploBingtianxuediEffect（冰天雪地：img=MagExplosionBase+(Dir16 div 2)×10+curframe，m_boActive 即绘）。</summary>
public class TExploBingtianxuediEffect : TMagicEff
{
    public TExploBingtianxuediEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, 0, sx, sy, tx, ty, TMagicType.mtExplosion, false, 0)
    {
        MagExplosionBase = effbase;
        TargetActor = null;
        NextFrameTime = 50;
        ExplosionFrame = 8;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive)
            return null;
        return (MagExplosionBase + (Dir16 / 2) * 10 + curframe, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TExploSanYanZhouEffect（三焰咒：FIREGUNFRAME=3 火节点双队列推进 + 油尽（MagOwner 距离≥10 或 800ms）+ 双图层节点绘制）。</summary>
public class TExploSanYanZhouEffect : TMagicEff
{
    public bool OutofOil;
    public uint firetime;
    public uint fire2time;
    public TFireNode[] FireNodes = new TFireNode[MagicTailConsts.FireGunFrameSanYanZhou];

    public TExploSanYanZhouEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 5;
        TargetActor = target;
        NextFrameTime = 50;
        OutofOil = false;
        firetime = Tick();
        fire2time = Tick();
    }

    public override bool Run()
    {
        bool result = true;
        bool boLoadSurface = false;

        if (Tick() - steptime >= (uint)NextFrameTime)
        {
            Shift();
            boLoadSurface = true;
            steptime = Tick();

            if (Tick() - fire2time >= (uint)(NextFrameTime * 2))
            {
                fire2time = Tick();
                if (!OutofOil)
                {
                    if (MagOwner != null && (Math.Abs(rx - (MagOwnerPosition?.Rx ?? rx)) >= 10 ||
                        Math.Abs(ry - (MagOwnerPosition?.Ry ?? ry)) >= 10 || Tick() - firetime > 800))
                        OutofOil = true;
                    for (int i = MagicTailConsts.FireGunFrameSanYanZhou - 2; i >= 0; i--)
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
                    for (int i = MagicTailConsts.FireGunFrameSanYanZhou - 2; i >= 0; i--)
                    {
                        if (FireNodes[i].firenumber <= MagicTailConsts.FireGunFrameSanYanZhou)
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
        }

        if (boLoadSurface)
            SurfaceReloads++;
        return result;
    }

    /// <summary>MagOwner 坐标接缝（TActor(MagOwner).m_nRx/Ry）。</summary>
    public IMagicTarget? MagOwnerPosition { get; set; }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 30 && Math.Abs(FlyY - fireY) <= 30 && !FixedEffect))
            return null;
        if (!FixedEffect)
        {
            int img = EffectBase + Dir16 * 10;
            return (img + curframe + 160, FlyX + px - 24, FlyY + py - 16, false); // d1 主图 Draw
        }
        return (MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TExploHuXiaoJueZhouEffect（虎啸诀：飞行 3580+Dir16×5 双图 +80 Blend；固定 3740+Dir16×5 与 MagExplosionBase 双 Blend）。</summary>
public class TExploHuXiaoJueZhouEffect : TMagicEff
{
    public TExploHuXiaoJueZhouEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 5;
        TargetActor = target;
        NextFrameTime = 80;
    }

    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 30 && Math.Abs(FlyY - fireY) <= 30 && !FixedEffect))
            return layers;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            if (curframe >= 5) curframe = 0; // 修正虎啸诀发出后的动作 chongchong 2013-11-09
            int img = 3580 + Dir16 * 5 + curframe;
            int x = FlyX + px - 24 - shx;
            int y = FlyY + py - 16 - shy;
            layers.Add((img, x, y, false));
            layers.Add((img + 80, x, y, true));
        }
        else
        {
            int img = 3740 + Dir16 * 5 + curframe;
            int x = FlyX + px - 24 - shx;
            int y = FlyY + py - 16 - shy;
            layers.Add((img, x, y, false));
            layers.Add((img, FlyX + px - 24, FlyY + py - 16, true));
            layers.Add((MagExplosionBase + Dir16 * 5 + curframe, FlyX + px - 24, FlyY + py - 16, true));
        }
        return layers;
    }
}

/// <summary>TRedThunderEffect（红色闪电：mtRedThunder、n0=Random(7)、img=EffectBase+7×n0+curframe、WDragonImg）。</summary>
public class TRedThunderEffect : TMagicEff
{
    public int n0;

    public TRedThunderEffect(int effbase, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, tx, ty, tx, ty, TMagicType.mtRedThunder, false, 0)
    {
        TargetActor = target;
        n0 = MagicEffEnv.RandomFn(7);
        ImgLibId = 16; // g_WDragonImg
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
        => (EffectBase + 7 * n0 + curframe, FlyX + px - 24, FlyY + py - 16, true);
}

/// <summary>TFireDragonEffect（火龙：飞行仅 Dir16∈[7..12]，img=EffectBase+FLYBASE×(Dir16−7)）。</summary>
public class TFireDragonEffect : TMagicEff
{
    public int FlyX1, FlyY1, FlyX2, FlyY2;
    public bool boflyFixedEffect;

    public TFireDragonEffect(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
        FlyX1 = FlyY1 = FlyX2 = FlyY2 = 0;
        boflyFixedEffect = false;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 15 && Math.Abs(FlyY - fireY) <= 15 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            if (Dir16 is < 7 or > 12)
                return null; // img := -1 → 不绘
            int img = EffectBase + MagicTailConsts.FlyBase * (Dir16 - 7);
            return (img + curframe, FlyX + px - 24 - shx, FlyY + py - 16 - shy, true);
        }
        return (MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>TJNExploBujaukEffect（经 Wrapper id=112：frame=8、飞行 (Dir16 div 2)×10 + EffectBase=140 加 img+170 副图）。</summary>
public class TJNExploBujaukEffect : TMagicEff
{
    public TJNExploBujaukEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(112, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 8;
        TargetActor = target;
        NextFrameTime = 50;
    }

    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 50 && Math.Abs(FlyY - fireY) <= 50 && !FixedEffect))
            return layers;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (!FixedEffect)
        {
            int img = EffectBase + Dir16 / 2 * 10;
            int x = FlyX + px - 24 - shx;
            int y = FlyY + py - 16 - shy;
            layers.Add((img + curframe, x, y, false));
            if (EffectBase == 140)
                layers.Add((img + curframe + 170, x, y, true));
        }
        else
        {
            layers.Add((MagExplosionBase + curframe, FlyX + px - 24, FlyY + py - 16, true));
        }
        return layers;
    }
}

/// <summary>TExplosion2Effect（二段爆炸：首播完 FixedEffect → 二段 frame/ExplosionBase 换装再播一次）。</summary>
public class TExplosion2Effect : TMagicEff
{
    public bool IsPlayExplosion2;
    public int MagExplosionBase_2;
    public int ExplosionFrame_2;

    public TExplosion2Effect(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
        IsPlayExplosion2 = false;
    }

    public override bool Run()
    {
        bool result = base.Run();
        if (!result && FixedEffect && !IsPlayExplosion2)
        {
            IsPlayExplosion2 = true;
            curframe = 0;
            frame = ExplosionFrame_2;
            MagExplosionBase = MagExplosionBase_2;
            result = true;
        }
        return result;
    }
}

/// <summary>TPlayEffect（场景播放：Repetition 循环 / 播 m_nMaxCount 遍 / ≤0 无限）。</summary>
public class TPlayEffect : TMagicEff
{
    public int m_nMaxCount;
    public int m_nPlayCount;
    public bool m_boBlend;
    /// <summary>TActor(TargetActor).m_boGhost 接缝。</summary>
    public Func<bool>? TargetGhostFn;

    public TPlayEffect(int images, int nCurrX, int nCurrY, int nImageStart, int nImageCount, int nPlayCount, bool boBlend, IMagicTarget? target)
        : base(111, nImageStart, nCurrX, nCurrY, nCurrX, nCurrY, TMagicType.mtExplosion, false, 0)
    {
        m_nMaxCount = nPlayCount;
        m_nPlayCount = 0;
        m_boBlend = boBlend;
        MagExplosionBase = nImageStart;
        TargetActor = target;
        ExplosionFrame = nImageCount;
        ImgLibId = images;
        NextFrameTime = 100;
    }

    public override bool Run()
    {
        if (TargetActor != null && TargetGhostFn != null && TargetGhostFn())
            return false;

        m_nCurrentFrame = curframe;
        bool result = Shift();
        if (!result)
        {
            if (m_nPlayCount < m_nMaxCount)
            {
                m_nPlayCount++;
                curframe = start;
                result = Shift();
            }
            else if (m_nMaxCount <= 0)
            {
                curframe = start;
                result = Shift();
            }
        }
        if (result && m_nCurrentFrame != curframe)
            SurfaceReloads++;
        return result;
    }

    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return null;
        int img;
        int x, y;
        if (!FixedEffect)
        {
            img = EffectBase + MagicTailConsts.FlyBase + Dir16 * 10;
        }
        else
        {
            img = MagExplosionBase + curframe;
            if (MagicId == 66 && curframe < 20)
            {
                py -= 225;
                px += 25;
            }
        }
        if (TargetActor != null)
        {
            var (fx, fy) = MagicEffEnv.ScreenXYfromMCXY(TargetActor.Rx, TargetActor.Ry);
            x = fx + TargetActor.ShiftX + px - 24;
            y = fy + TargetActor.ShiftY + py - 16;
        }
        else
        {
            x = FlyX + px - 24;
            y = FlyY + py - 16;
        }
        return (img + (FixedEffect ? curframe : curframe), x, y, m_boBlend);
    }
}

/// <summary>THeroShowEffect（英雄出场特效：WEffectImg、双构造、Shift 固定分支 nil 时 rx=targetx 再取屏坐标）。</summary>
public class THeroShowEffect : TMagicEff
{
    public THeroShowEffect(int effbase, int effframe, int nX, int nY)
        : base(111, effbase, nX, nY, nX, nY, TMagicType.mtExplosion, false, 0)
    {
        ImgLibId = 1; // g_WEffectImg
        TargetActor = null;
        MagExplosionBase = effbase;
        ExplosionFrame = effframe;
        NextFrameTime = 100;
    }

    public THeroShowEffect(int effbase, int effframe, IMagicTarget target)
        : base(111, effbase, target.Rx, target.Ry, target.Rx, target.Ry, TMagicType.mtExplosion, false, 0)
    {
        ImgLibId = 1;
        TargetActor = target;
        MagExplosionBase = effbase;
        ExplosionFrame = effframe;
        NextFrameTime = 100;
    }

    public override bool Shift()
    {
        bool result = true;
        if (Repetition)
        {
            if (Tick() - steptime > (uint)NextFrameTime)
            {
                steptime = Tick();
                curframe++;
                if (curframe > start + frame - 1)
                    curframe = start;
            }
        }
        else
        {
            if (frame > 0 && Tick() - steptime > (uint)NextFrameTime)
            {
                steptime = Tick();
                curframe++;
                if (curframe > start + frame - 1)
                {
                    curframe = start + frame - 1;
                    result = false;
                }
            }
        }

        if (FixedEffect)
        {
            if (frame == -1)
                frame = ExplosionFrame;
            if (TargetActor == null)
            {
                rx = targetx;
                ry = targety;
                FlyX = MagicEffEnv.ScreenXYfromMCXY(rx, ry).Sx;
            }
            else
            {
                FlyX = MagicEffEnv.ScreenXYfromMCXY(TargetActor.Rx, TargetActor.Ry).Sx + TargetActor.ShiftX;
                FlyY = MagicEffEnv.ScreenXYfromMCXY(TargetActor.Rx, TargetActor.Ry).Sy + TargetActor.ShiftY;
            }
        }
        return result;
    }
}

/// <summary>TShowPlayEffect（护体神盾：WMain2Images、NFT=60、双分支绘制 + MagicId=66 py−225/px+25 修正）。</summary>
public class TShowPlayEffect : TMagicEff
{
    public int nX, nY; // 修正护体神盾特效有偏移 chongchong 2016-12-17

    public TShowPlayEffect(int effbase, int effframe, IMagicTarget target)
        : base(111, effbase, target.Rx, target.Ry, target.Rx, target.Ry, TMagicType.mtExplosion, false, 0)
    {
        ImgLibId = 17; // g_WMain2Images
        TargetActor = target;
        MagExplosionBase = effbase;
        ExplosionFrame = effframe;
        NextFrameTime = 60;
        nX = MagicEffEnv.ScreenXYfromMCXY(target.Rx, target.Ry).Sx;
        nY = MagicEffEnv.ScreenXYfromMCXY(target.Rx, target.Ry).Sy;
    }

    public override List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return layers;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        if (TargetActor == null)
            return layers;
        int nx = nX + TargetActor.ShiftX;
        int ny = nY + TargetActor.ShiftY;
        if (!FixedEffect)
        {
            int img = EffectBase + MagicTailConsts.FlyBase + Dir16 * 10;
            layers.Add((img + curframe, FlyX + px - 24 - shx, FlyY + py - 16 - shy, true));
        }
        else
        {
            int img = MagExplosionBase + curframe;
            int px0 = px, py0 = py;
            if (MagicId == 66 && curframe < 20)
            {
                py0 -= 225;
                px0 += 25;
            }
            if (m_DrawBlend)
                layers.Add((img, nx + px0 - 24, ny + py0 - 16, true));
            else
                layers.Add((img, FlyX + px0 - 24, FlyY + py0 - 16, false));
        }
        return layers;
    }
}
