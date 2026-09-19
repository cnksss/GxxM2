using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>magiceff.pas 场景环境接缝（g_MySelf/PlayScene 坐标变换/Random；headless 测试可注入）。</summary>
public static class MagicEffEnv
{
    public static int SelfRx;
    public static int SelfRy;
    public static int SelfShiftX;
    public static int SelfShiftY;

    /// <summary>Delphi Random(n)（0..n-1；测试注入固定值）。</summary>
    public static Func<int, int> RandomFn = _ => 0;

    /// <summary>PlayScene.ScreenXYfromMCXY（地图格 → 屏幕像素；默认线性等效）。</summary>
    public static Func<int, int, (int Sx, int Sy)> ScreenXYfromMCXY = (x, y) => (x * 48, y * 32);

    /// <summary>PlayScene.CXYfromMouseXY（屏幕像素 → 地图格；默认线性等效）。</summary>
    public static Func<int, int, (int Mcx, int Mcy)> CXYfromMouseXY = (x, y) => (x / 48, y / 32);

    /// <summary>观察者（主角）死亡 → 特效取灰度图（各 DrawEff 的 g_MySelf.m_boDeath 判定）。</summary>
    public static bool SelfDead;

    public static void Reset()
    {
        SelfRx = SelfRy = SelfShiftX = SelfShiftY = 0;
        SelfDead = false;
        RandomFn = _ => 0;
        ScreenXYfromMCXY = (x, y) => (x * 48, y * 32);
        CXYfromMouseXY = (x, y) => (x / 48, y / 32);
    }
}

/// <summary>飞行目标接缝（TActor(TargetActor) 的坐标面）。</summary>
public interface IMagicTarget
{
    int Rx { get; }
    int Ry { get; }
    int ShiftX { get; }
    int ShiftY { get; }
}

/// <summary>
/// magiceff.pas TMagicEff（70-127 字段族 + 925-1230 构造 + 1238-1398 Shift/GetFlyXY + 1410-1437 Run
/// + 1479-1533 DrawEff 图号计算）headless 移植（批次J48）。绘制以「图号/坐标计算」产出替代画布调用。
/// </summary>
public class TMagicEff
{
    public int ServerMagicId;
    public int MagicId;
    public int EffectNumber;
    public int EffectBase;
    public int MagExplosionBase;
    public int px, py;
    public int rx, ry;
    public int Dir16, OldDir16;
    public int targetx, targety;
    public int FlyX, FlyY, OldFlyX, OldFlyY;
    public double FlyXf, FlyYf;
    public bool Repetition;
    public bool FixedEffect;
    public TMagicType MagicType;
    public int ExplosionFrame;
    public int NextFrameTime;
    public int light;
    public int bt80;
    public int bt81;
    public int n7C;
    public int start;
    public int curframe;
    public int frame;
    public int NewLevel;
    public int m_nCurrentFrame;
    public bool m_DrawBlend = true;
    public uint m_LastRunTick;
    public uint m_dwRunTime;
    public uint m_dwFrameTime;
    public uint m_dwStartTime;
    public uint m_dwStartTimeSet;
    public uint steptime;
    public uint repeattime;
    public int fireX, fireY;
    public int firedisX, firedisY;
    public int newfiredisX, newfiredisY;
    public int FireMyselfX, FireMyselfY;
    public int prevdisx, prevdisy;
    public bool m_boActive;
    public bool LastCrashDebug;
    public int m_dwGhostTick;

    public int ImgLibId;                  // ImgLib 图库标识（0=WMagicImages；2=WMagic2Images；21=WMonImages.Indexs[21]）
    public bool ImgLibIsMon;              // mt13 → g_WMonImages.Indexs[21]
    public IMagicTarget? TargetActor;
    public object? MagOwner;
    public bool m_boUseMagicSurface;
    public bool MagicBlend = true;

    /// <summary>时间接缝（SceneTime.TickNow）。</summary>
    public Func<uint> Tick = () => SceneTime.TickNow();

    public TMagicEff(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
    {
        NewLevel = 0;
        MagicType = mtype;
        m_nCurrentFrame = -1;
        MagicId = 0;
        m_DrawBlend = true;
        switch (mtype)
        {
            case TMagicType.mtFly:
            case TMagicType.mtBujaukGroundEffect:
            case TMagicType.mtExploBujauk:
                start = 0;
                frame = 6;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 10;
                if (id == 38) frame = 10;
                if (id == 39)
                {
                    frame = 4;
                    ExplosionFrame = 8;
                }
                if (id - 81 - 3 < 0)
                {
                    bt80 = 1;
                    Repetition = true;
                    if (id == 81)
                    {
                        EffectBase = MagicEffEnv.SelfRx >= 84 ? 130 : 140;
                        bt81 = 1;
                    }
                    if (id == 82)
                    {
                        EffectBase = MagicEffEnv.SelfRx >= 78 && MagicEffEnv.SelfRy >= 48 ? 150 : 160;
                        bt81 = 2;
                    }
                    if (id == 83)
                    {
                        EffectBase = 180;
                        bt81 = 3;
                    }
                    start = 0;
                    frame = 10;
                    MagExplosionBase = 190;
                    ExplosionFrame = 10;
                }
                break;
            case TMagicType.mt12:
                start = 0;
                frame = 6;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 1;
                break;
            case TMagicType.mt13:
                start = 0;
                frame = 20;
                curframe = start;
                FixedEffect = true;
                Repetition = false;
                ExplosionFrame = 20;
                ImgLibId = 21;
                ImgLibIsMon = true;
                break;
            case TMagicType.mtExplosion:
            case TMagicType.mtThunder:
            case TMagicType.mtLightingThunder:
                start = 0;
                frame = -1;
                ExplosionFrame = 10;
                curframe = start;
                FixedEffect = true;
                Repetition = false;
                if (id == 80)
                {
                    bt80 = 2;
                    EffectBase = MagicEffEnv.RandomFn(6) switch { 1 or 4 => 240, 2 or 5 => 250, _ => 230 };
                    light = 4;
                    ExplosionFrame = 5;
                }
                if (id == 70)
                {
                    bt80 = 3;
                    EffectBase = MagicEffEnv.RandomFn(3) switch { 1 => 410, 2 => 420, _ => 400 };
                    light = 4;
                    ExplosionFrame = 5;
                }
                if (id == 71)
                {
                    bt80 = 3;
                    ExplosionFrame = 20;
                }
                if (id == 72)
                {
                    bt80 = 3;
                    light = 3;
                    ExplosionFrame = 10;
                }
                if (id == 73)
                {
                    bt80 = 3;
                    light = 5;
                    ExplosionFrame = 20;
                }
                if (id == 74)
                {
                    bt80 = 3;
                    light = 4;
                    ExplosionFrame = 35;
                }
                if (id == 90)
                {
                    EffectBase = 350;
                    MagExplosionBase = 350;
                    ExplosionFrame = 30;
                }
                break;
            case TMagicType.mt14:
                start = 0;
                frame = -1;
                curframe = start;
                FixedEffect = true;
                Repetition = false;
                ImgLibId = 2; // g_WMagic2Images
                break;
            case TMagicType.mtFlyAxe:
                start = 0;
                frame = 3;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 3;
                break;
            case TMagicType.mtFlyArrow:
                start = 0;
                frame = 1;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 1;
                break;
            case TMagicType.mt15:
                start = 0;
                frame = 6;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 2;
                break;
            case TMagicType.mt16:
                start = 0;
                frame = 1;
                curframe = start;
                FixedEffect = false;
                Repetition = recusion;
                ExplosionFrame = 1;
                break;
        }
        n7C = 0;
        ServerMagicId = id;
        EffectBase = effnum;
        targetx = tx;
        targety = ty;

        if (bt80 == 1)
        {
            if (id == 81)
            {
                sx -= 14;
                sy += 20;
            }
            if (id == 81)
            { // 原文重复判定 id=81：两个减量都执行
                sx -= 70;
                sy -= 10;
            }
            if (id == 83)
            {
                sx -= 60;
                sy -= 70;
            }
            LastPlaySound = 8208; // g_PlaySound.PlaySound(8208)
        }
        fireX = sx;
        fireY = sy;
        FlyX = sx;
        FlyY = sy;
        OldFlyX = sx;
        OldFlyY = sy;
        FlyXf = sx;
        FlyYf = sy;
        FireMyselfX = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX;
        FireMyselfY = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY;
        if (bt80 == 0)
            MagExplosionBase = EffectBase + MagicEffConsts.EXPLOSIONBASE;

        light = 1; // 尾部无条件覆写（case 内 light 赋值为原文死代码保留）

        int tax = fireX != targetx ? Math.Abs(targetx - fireX) : 1;
        int tay = fireY != targety ? Math.Abs(targety - fireY) : 1;
        if (Math.Abs(fireX - targetx) > Math.Abs(fireY - targety))
        {
            firedisX = (int)Math.Round((targetx - fireX) * (500.0 / tax));
            firedisY = (int)Math.Round((targety - fireY) * (500.0 / tax));
        }
        else
        {
            firedisX = (int)Math.Round((targetx - fireX) * (500.0 / tay));
            firedisY = (int)Math.Round((targety - fireY) * (500.0 / tay));
        }

        NextFrameTime = 50;
        m_dwFrameTime = Tick();
        m_dwStartTime = Tick();
        m_dwRunTime = Tick();
        steptime = Tick();
        repeattime = (uint)anitime;
        Dir16 = GetFlyDirection16(sx, sy, tx, ty);
        OldDir16 = Dir16;
        m_boActive = true;
        prevdisx = 99999;
        prevdisy = 99999;
        m_LastRunTick = Tick();
    }

    public int LastPlaySound = -1;

    /// <summary>ClFunc.pas 1551 GetFlyDirection16（16 方向）。</summary>
    public static int GetFlyDirection16(int sx, int sy, int ttx, int tty)
    {
        double fx = ttx - sx;
        double fy = tty - sy;
        int result = 0;
        if (fx == 0)
            return fy < 0 ? 0 : 8;
        if (fy == 0)
            return fx < 0 ? 12 : 4;
        if (fx > 0 && fy < 0)
        {
            result = 4;
            if (-fy > fx / 4) result = 3;
            if (-fy > fx / 1.9) result = 2;
            if (-fy > fx * 1.4) result = 1;
            if (-fy > fx * 4) result = 0;
        }
        if (fx > 0 && fy > 0)
        {
            result = 4;
            if (fy > fx / 4) result = 5;
            if (fy > fx / 1.9) result = 6;
            if (fy > fx * 1.4) result = 7;
            if (fy > fx * 4) result = 8;
        }
        if (fx < 0 && fy > 0)
        {
            result = 12;
            if (fy > -fx / 4) result = 11;
            if (fy > -fx / 1.9) result = 10;
            if (fy > -fx * 1.4) result = 9;
            if (fy > -fx * 4) result = 8;
        }
        if (fx < 0 && fy < 0)
        {
            result = 12;
            if (-fy > -fx / 4) result = 13;
            if (-fy > -fx / 1.9) result = 14;
            if (-fy > -fx * 1.4) result = 15;
            if (-fy > -fx * 4) result = 0;
        }
        return result;
    }

    private static bool OverThrough(int olddir, int newdir)
    {
        bool result = false;
        if (Math.Abs(olddir - newdir) >= 2)
        {
            result = true;
            if ((olddir == 0 && newdir == 15) || (olddir == 15 && newdir == 0))
                result = false;
        }
        return result;
    }

    /// <summary>TMagicEff.Shift 1:1（帧推进 + 飞行步进 + 惯爆切换 + 固定效果目标跟随）。</summary>
    public virtual bool Shift()
    {
        bool result = true;
        if (Repetition)
        {
            if (Tick() - steptime > NextFrameTime)
            {
                steptime = Tick();
                curframe++;
                if (curframe > start + frame - 1)
                    curframe = start;
            }
        }
        else
        {
            if (frame > 0 && Tick() - steptime > NextFrameTime)
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

        if (!FixedEffect)
        {
            bool crash = false;
            LastCrashDebug = false;
            int ms, newstepx, newstepy;
            int tax, tay, shx, shy, passdir16;
            double stepxf, stepyf;
            if (TargetActor != null)
            {
                ms = (int)(Tick() - m_dwFrameTime);
                m_dwFrameTime = Tick();
                var (tScreenX, tScreenY) = MagicEffEnv.ScreenXYfromMCXY(TargetActor.Rx, TargetActor.Ry);
                targetx = tScreenX;
                targety = tScreenY;
                shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
                shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
                targetx += shx;
                targety += shy;

                tax = FlyX != targetx ? Math.Abs(targetx - FlyX) : 1;
                tay = FlyY != targety ? Math.Abs(targety - FlyY) : 1;
                if (Math.Abs(FlyX - targetx) > Math.Abs(FlyY - targety))
                {
                    newfiredisX = (int)Math.Round((targetx - FlyX) * (500.0 / tax));
                    newfiredisY = (int)Math.Round((targety - FlyY) * (500.0 / tax));
                }
                else
                {
                    newfiredisX = (int)Math.Round((targetx - FlyX) * (500.0 / tay));
                    newfiredisY = (int)Math.Round((targety - FlyY) * (500.0 / tay));
                }

                if (firedisX < newfiredisX) firedisX += Math.Max(1, (newfiredisX - firedisX) / 10);
                if (firedisX > newfiredisX) firedisX -= Math.Max(1, (firedisX - newfiredisX) / 10);
                if (firedisY < newfiredisY) firedisY += Math.Max(1, (newfiredisY - firedisY) / 10);
                if (firedisY > newfiredisY) firedisY -= Math.Max(1, (firedisY - newfiredisY) / 10);

                stepxf = firedisX / 700.0 * ms;
                stepyf = firedisY / 700.0 * ms;
                FlyXf += stepxf;
                FlyYf += stepyf;
                FlyX = (int)Math.Round(FlyXf);
                FlyY = (int)Math.Round(FlyYf);

                OldFlyX = FlyX;
                OldFlyY = FlyY;

                passdir16 = GetFlyDirection16(FlyX, FlyY, targetx, targety);

                if ((Math.Abs(targetx - FlyX) <= 15 && Math.Abs(targety - FlyY) <= 15) ||
                    (Math.Abs(targetx - FlyX) >= prevdisx && Math.Abs(targety - FlyY) >= prevdisy) ||
                    OverThrough(OldDir16, passdir16))
                {
                    crash = true;
                }
                else
                {
                    prevdisx = Math.Abs(targetx - FlyX);
                    prevdisy = Math.Abs(targety - FlyY);
                }
                OldDir16 = passdir16;
            }
            else
            {
                ms = (int)(Tick() - m_dwFrameTime);
                int rrx = targetx - fireX;
                int rry = targety - fireY;
                _ = rrx; _ = rry; // Delphi 计算但未使用（原文形态）
                newstepx = (int)Math.Round(firedisX / 900.0 * ms);
                newstepy = (int)Math.Round(firedisY / 900.0 * ms);
                FlyX = fireX + newstepx;
                FlyY = fireY + newstepy;
            }

            var (mcx, mcy) = MagicEffEnv.CXYfromMouseXY(FlyX, FlyY);
            rx = mcx;
            ry = mcy;

            LastCrashDebug = crash;
            if (crash && TargetActor != null)
            {
                FixedEffect = true; // 爆燃
                start = 0;
                frame = ExplosionFrame;
                curframe = start;
                Repetition = false;
            }
        }
        if (FixedEffect)
        {
            bool boValue = false;
            if (frame == -1)
            {
                frame = ExplosionFrame;
                boValue = true;
            }
            if (TargetActor == null)
            {
                FlyX = targetx - (MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX);
                FlyY = targety - (MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY);
                if (boValue)
                {
                    var (mcx, mcy) = MagicEffEnv.CXYfromMouseXY(FlyX, FlyY);
                    rx = mcx;
                    ry = mcy;
                }
            }
            else
            {
                rx = TargetActor.Rx;
                ry = TargetActor.Ry;
                var (sx, sy) = MagicEffEnv.ScreenXYfromMCXY(rx, ry);
                FlyX = sx + TargetActor.ShiftX;
                FlyY = sy + TargetActor.ShiftY;
            }
        }
        return result;
    }

    /// <summary>GetFlyXY 1:1（1400-1408）。</summary>
    public void GetFlyXY(int ms, out int fx, out int fy)
    {
        int stepx = (int)Math.Round(firedisX / 900.0 * ms);
        int stepy = (int)Math.Round(firedisY / 900.0 * ms);
        fx = fireX + stepx;
        fy = fireY + stepy;
    }

    /// <summary>Run 1:1（1410-1437）：Shift 推进 + 10 秒超时 + 状态变化触发 LoadSurface。</summary>
    public virtual bool Run()
    {
        bool boFixedEffect = FixedEffect;
        int nOFlyX = FlyX;
        int nOFlyY = FlyY;
        bool result = Shift();
        if (result)
        {
            if (Tick() - m_dwStartTime > 10000)
                result = false;
            else
            {
                if (boFixedEffect != FixedEffect || m_nCurrentFrame != curframe || nOFlyX != FlyX || nOFlyY != FlyY)
                    SurfaceReloads++; // PlayScene.LoadSurface(LoadSurface) 调度
                m_boUseMagicSurface = m_boActive;
            }
        }
        m_nCurrentFrame = curframe;
        m_LastRunTick = Tick();
        return result;
    }

    /// <summary>LoadSurface 调度计数（PlayScene.LoadSurface 接缝审计）。</summary>
    public int SurfaceReloads;

    /// <summary>DrawEff 图号/位置计算 1:1（1479-1533，headless 产出绘制参数）。</summary>
    public virtual (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (!m_boActive || (Math.Abs(FlyX - fireX) <= 1 && Math.Abs(FlyY - fireY) <= 1 && !FixedEffect))
            return null;
        int shx = MagicEffEnv.SelfRx * 48 + MagicEffEnv.SelfShiftX - FireMyselfX;
        int shy = MagicEffEnv.SelfRy * 32 + MagicEffEnv.SelfShiftY - FireMyselfY;
        int img;
        int x, y;
        if (!FixedEffect)
        {
            img = EffectBase + MagicEffConsts.FLYBASE + Dir16 * 10 + curframe;
            x = FlyX + px - 48 / 2 - shx;
            y = FlyY + py - 32 / 2 - shy;
            return (img, x, y, true);
        }
        img = MagExplosionBase + curframe;
        if (MagicId == 66 && curframe < 20)
        {
            py -= 225;
            px += 25;
        }
        x = FlyX + px - 48 / 2;
        y = FlyY + py - 32 / 2;
        return (img, x, y, m_DrawBlend);
    }

    /// <summary>多图层绘制产出（J70 双图层子类覆写；缺省退化为单层 ComputeDraw）。</summary>
    public virtual List<(int Img, int X, int Y, bool Blend)> ComputeDrawLayers()
    {
        var single = ComputeDraw();
        var layers = new List<(int Img, int X, int Y, bool Blend)>();
        if (single.HasValue)
            layers.Add(single.Value);
        return layers;
    }
}

/// <summary>magiceff.pas TFireNode（火炮火节点）。</summary>
public struct TFireNode
{
    public int firenumber;
    public int X;
    public int Y;
}

/// <summary>magiceff.pas TFlyingAxe（掷斧/半兽统领：Run 50ms 节流；DrawEff 阈值 ReadyFrame=65、img=447+Dir16×10）。</summary>
public class TFlyingAxe : TMagicEff
{
    public int FlyImageBase;
    public int ReadyFrame;

    public TFlyingAxe(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
        FlyImageBase = MagicEffConsts.FLYOMAAXEBASE;
        ReadyFrame = 65;
    }

    /// <summary>Run 节流：>=50ms 才推进（修正飞行魔法不显示 chong 2018-12-22）。</summary>
    public override bool Run()
    {
        bool result = true;
        if (Tick() - m_dwRunTime >= 50)
        {
            m_dwRunTime = Tick();
            result = base.Run();
        }
        return result;
    }
}

/// <summary>magiceff.pas TFlyingBug（FlyImageBase/ReadyFrame 字段族）。</summary>
public class TFlyingBug : TMagicEff
{
    public int FlyImageBase;
    public int ReadyFrame;

    public TFlyingBug(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
    }
}

/// <summary>magiceff.pas TFlyingArrow（Run 100ms 节流不回写 m_dwRunTime 原文缺陷；DrawEff img=FlyImageBase+Dir16、Y 偏移 -46）。</summary>
public class TFlyingArrow : TFlyingAxe
{
    public TFlyingArrow(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
    }

    /// <summary>Run 节流：>100ms 推进但不回写 m_dwRunTime（弓箭可视性原文缺陷保留）。</summary>
    public override bool Run()
    {
        bool result = true;
        if (Tick() - m_dwRunTime > 100)
        {
            // 原文注释：不能回写 m_dwRunTime（弓箭可视性缺陷保留）
            result = base.Run();
        }
        return result;
    }
}

/// <summary>magiceff.pas TFlyingArrowEx（Run 100ms 节流且回写 m_dwRunTime——与 TFlyingArrow 差异点）。</summary>
public class TFlyingArrowEx : TFlyingAxe
{
    public TFlyingArrowEx(int id, int effnum, int sx, int sy, int tx, int ty, TMagicType mtype, bool recusion, int anitime)
        : base(id, effnum, sx, sy, tx, ty, mtype, recusion, anitime)
    {
    }

    /// <summary>Run 节流：>100ms 推进且回写 m_dwRunTime（与 TFlyingArrow 的原文差异点）。</summary>
    public override bool Run()
    {
        bool result = true;
        if (Tick() - m_dwRunTime > 100)
        {
            m_dwRunTime = Tick();
            result = base.Run();
        }
        return result;
    }
}

/// <summary>magiceff.pas TFireGunEffect（地狱火：FIREGUNFRAME=6 火节点队列 + OutofOil 油尽逻辑）。</summary>
public class TFireGunEffect : TMagicEff
{
    public TFireNode[] FireNodes = new TFireNode[MagicEffConsts.FIREGUNFRAME];
    public bool OutofOil;
    public uint firetime;

    public TFireGunEffect(int effbase, int sx, int sy, int tx, int ty)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtFireGun, true, 0)
    {
        NextFrameTime = 50;
        OutofOil = false;
        firetime = Tick();
    }

    /// <summary>MagOwner 所属者坐标面（Delphi TActor(MagOwner) 等效，由 NewMagic 装配）。</summary>
    public IMagicTarget? Owner;

    /// <summary>Run 1:1（1996-2037）：NextFrameTime 节流 → Shift → 火节点推进/油尽燃尽。</summary>
    public override bool Run()
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
                if (Owner != null && (Math.Abs(rx - Owner.Rx) >= 5 || Math.Abs(ry - Owner.Ry) >= 5 || Tick() - firetime > 800))
                    OutofOil = true;
                for (int i = MagicEffConsts.FIREGUNFRAME - 2; i >= 0; i--)
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
                for (int i = MagicEffConsts.FIREGUNFRAME - 2; i >= 0; i--)
                {
                    if (FireNodes[i].firenumber <= MagicEffConsts.FIREGUNFRAME)
                    {
                        FireNodes[i].firenumber++;
                        FireNodes[i + 1] = FireNodes[i];
                        allgone = false;
                        boLoadSurface = false;
                    }
                }
                if (allgone)
                    result = false;
            }
        }

        if (boLoadSurface)
            SurfaceReloads++;
        return result;
    }
}
