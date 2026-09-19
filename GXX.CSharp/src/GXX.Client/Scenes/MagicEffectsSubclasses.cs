using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>角色目标扩展（构造需要 m_nCurrX/m_nCurrY 的子类使用）。</summary>
public interface IMagicTargetEx : IMagicTarget
{
    int CurrX { get; }
    int CurrY { get; }
}

/// <summary>
/// magiceff.pas 特效子类余部（批次J49，headless 移植）：
/// TCharEffect（角色身上特效）/TMapEffect（地图固定特效，RepeatCount 回卷）/
/// TScrollHideEffect（隐身术：frame==7 标记焦点角色删除）/TLightingEffect（空构造恒假）/
/// TThuderEffect（同位置雷）/TLightingThunder（疾光电影：Dir16×10 图号 + curframe&lt;6 才绘 +
/// MagOwner 位置）/TExploBujaukEffect（爆破符：frame=3、MagicBlend=false、Shift/Run 同基类）。
/// </summary>
public class TCharEffect : TMagicEff
{
    public TCharEffect(int effbase, int effframe, IMagicTargetEx target)
        : base(111, effbase, target.CurrX, target.CurrY, target.CurrX, target.CurrY, TMagicType.mtExplosion, false, 0)
    {
        TargetActor = target;
        frame = effframe;
        NextFrameTime = 30;
    }

    /// <summary>Run 1:1（1811-1829）：NextFrameTime(30) 节流推进，到末帧返回假。</summary>
    public override bool Run()
    {
        bool result = true;
        m_nCurrentFrame = curframe;
        if (Tick() - steptime > (uint)NextFrameTime)
        {
            steptime = Tick();
            curframe++;
            if (curframe > start + frame - 1)
            {
                curframe = start + frame - 1;
                result = false;
            }
        }
        if (result && m_nCurrentFrame != curframe)
            SurfaceReloads++;
        return result;
    }

    /// <summary>DrawEff 1:1（1847-1868）：跟随目标屏幕坐标，图号 = EffectBase + curframe。</summary>
    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        if (TargetActor == null)
            return null;
        rx = TargetActor.Rx;
        ry = TargetActor.Ry;
        var (sx, sy) = MagicEffEnv.ScreenXYfromMCXY(rx, ry);
        FlyX = sx + TargetActor.ShiftX;
        FlyY = sy + TargetActor.ShiftY;
        int img = EffectBase + curframe;
        bool gray = MagicEffEnv.SelfDead;
        return (gray ? -img : img, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>magiceff.pas TMapEffect（1872-1936：地图固定特效，RepeatCount 支持重播）。</summary>
public class TMapEffect : TMagicEff
{
    public int RepeatCount;

    public TMapEffect(int effbase, int effframe, int x, int y)
        : base(111, effbase, x, y, x, y, TMagicType.mtExplosion, false, 0)
    {
        TargetActor = null;
        frame = effframe;
        NextFrameTime = 30;
        RepeatCount = 0;
    }

    /// <summary>Run 1:1（1886-1905）：到末帧时 RepeatCount>0 → 回卷 start 并递减，否则结束。</summary>
    public override bool Run()
    {
        bool result = true;
        m_nCurrentFrame = curframe;
        if (Tick() - steptime > (uint)NextFrameTime)
        {
            steptime = Tick();
            curframe++;
            if (curframe > start + frame - 1)
            {
                curframe = start + frame - 1;
                if (RepeatCount > 0)
                {
                    RepeatCount--;
                    curframe = start;
                }
                else
                    result = false;
            }
        }
        if (result && m_nCurrentFrame != curframe)
            SurfaceReloads++;
        return result;
    }

    /// <summary>DrawEff 1:1（1919-1936）：固定于 targetx/targety，图号 = EffectBase + curframe。</summary>
    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        rx = targetx;
        ry = targety;
        var (sx, sy) = MagicEffEnv.ScreenXYfromMCXY(rx, ry);
        int img = EffectBase + curframe;
        bool gray = MagicEffEnv.SelfDead;
        return (gray ? -img : img, sx + px - 24, sy + py - 16, true);
    }
}

/// <summary>magiceff.pas TScrollHideEffect（随机传送卷隐身：frame==7 时标记焦点角色删除）。</summary>
public class TScrollHideEffect : TMapEffect
{
    /// <summary>焦点角色删除标记接缝（Delphi 直接改写 g_FocusCret 的 m_boDelActor）。</summary>
    public Func<long?, long?>? FocusCretHandler;
    public bool LastFocusMarkedForDelete;

    public TScrollHideEffect(int effbase, int effframe, int x, int y)
        : base(effbase, effframe, x, y)
    {
        // Delphi：TargetCret := TActor(target) 注释保留（不设置目标）
    }

    /// <summary>Run 1:1（1946-1966）：inherited Run + frame==7 时标记焦点角色删除。</summary>
    public override bool Run()
    {
        bool result = base.Run();
        if (frame == 7)
        {
            var chosen = FocusCretHandler?.Invoke(null);
            if (chosen != null)
            {
                LastFocusMarkedForDelete = true;
            }
        }
        return result;
    }
}

/// <summary>magiceff.pas TLightingEffect（空构造恒假：不调用继承构造，字段保持默认）。</summary>
public class TLightingEffect : TMagicEff
{
    public TLightingEffect(int effbase, int effframe, int x, int y)
        : base(0, 0, 0, 0, 0, 0, TMagicType.mtReady, false, 0)
    {
        // Delphi 构造体为空（未调用 inherited）→ 字段保持默认
    }

    public override bool Run() => false; // Result := False // Jacky
}

/// <summary>magiceff.pas TThuderEffect（同位置雷：图号 = EffectBase + curframe，绘制于 FlyX/FlyY）。</summary>
public class TThuderEffect : TMagicEff
{
    public TThuderEffect(int effbase, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, tx, ty, tx, ty, TMagicType.mtThunder, false, 0)
    {
        TargetActor = target;
    }

    /// <summary>DrawEff 1:1（2118-2134）：img = EffectBase + curframe。</summary>
    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        int img = EffectBase + curframe;
        bool gray = MagicEffEnv.SelfDead;
        return (gray ? -img : img, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>magiceff.pas TLightingThunder（疾光电影：curframe&lt;6 才绘，img=EffectBase+Dir16×10+curframe，MagOwner 定位）。</summary>
public class TLightingThunder : TMagicEff
{
    public TLightingThunder(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtLightingThunder, false, 0)
    {
        TargetActor = target;
    }

    /// <summary>DrawEff 1:1（2165-2193）。</summary>
    public override (int Img, int X, int Y, bool Blend)? ComputeDraw()
    {
        int img = EffectBase + Dir16 * 10;
        if (curframe >= 6)
            return null;
        if (MagOwner is IMagicTarget owner)
        {
            var (sx, sy) = MagicEffEnv.ScreenXYfromMCXY(owner.Rx, owner.Ry);
            return (img + curframe, sx + owner.ShiftX + px - 24, sy + owner.ShiftY + py - 16, true);
        }
        return (img + curframe, FlyX + px - 24, FlyY + py - 16, true);
    }
}

/// <summary>magiceff.pas TExploBujaukEffect（爆破符：frame=3、MagicBlend=false、mtExploBujauk 飞行重复；Run/Shift 与基类同构）。</summary>
public class TExploBujaukEffect : TMagicEff
{
    public bool MagicBlend = true;

    public TExploBujaukEffect(int effbase, int sx, int sy, int tx, int ty, IMagicTarget? target)
        : base(111, effbase, sx, sy, tx, ty, TMagicType.mtExploBujauk, true, 0)
    {
        frame = 3;
        TargetActor = target;
        NextFrameTime = 50;
        MagicBlend = false;
    }

    /// <summary>Run 1:1（2211-2214）：恒转基类。</summary>
    public override bool Run() => base.Run();

    /// <summary>Shift 1:1（2216-2360 与基类同构）→ 直接沿用基类 Shift。</summary>
}
