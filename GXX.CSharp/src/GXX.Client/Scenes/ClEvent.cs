using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>事件图号产物：图库标识 + 图号（headless 无纹理本体；List 见 ClEventImages）。</summary>
public sealed record EventImage(string List, int ImageIndex, int Px, int Py, bool Gray);

/// <summary>clEvent.pas 所引用的图库清单（1:1 对应 g_W*Images 全家）。</summary>
public static class ClEventImages
{
    public const string Mon6 = "WMonImages[6]";          // ET_DIGOUTZOMBI 尸尘
    public const string Mon7 = "WMonImages[7]";          // ET_SCULPEICE 石雕碎片
    public const string Mon14 = "WMonImages[14]";        // ET_DEDING 地钉
    public const string Mon27 = "WMonImages[27]";        // ET_ICEPEAK 冰峰
    public const string Mon33 = "WMonImages[33]";        // ET_FIREMON33_7 光圈
    public const string Effect = "WEffectImg";           // ET_PILESTONES 碎石
    public const string Magic = "WMagicImages";          // ET_HOLYCURTAIN / ET_FIRE
    public const string Magic2 = "WMagic2Images";        // ET_THUNDER2 白色雷电
    public const string Magic3 = "WMagic3Images";        // ET_FIREFLOWER / ET_FLASHLIGHT
    public const string Magic7x16 = "WMagic7Images16";   // ET_FIRELevel1..3
    public const string Magic10 = "WMagic10Images";      // ET_SAFERECT / ET_HOLYCURTAIN2
    public const string Main2 = "WMain2Images";          // ET_SPRINGS / ET_SPRINGS_LIGHT
    public const string Dragon = "WDragonImg";           // ET_THUNDER / ET_LAVA / ET_FIREDRAGON / ET_LAVA2 / ET_FIREDRAGON2
    public const string NpcImg0 = "WNpcImgImages[0]";    // ET_DOOR1..5 传送门
    public const string SafePoint = "SafePointEffect";   // ET_CUSTOM_SAFE_POINT1..2
    public const string EffectList = "EffectImageList";  // TMapEffectEvent / TCustomEffectEvent 动态库
}

/// <summary>事件基图号常量（clEvent.pas const 段）。</summary>
public static class ClEventConsts
{
    public const int ZombieDigUpDustBase = 420;
    public const int StoneFragmentBase = 64;
    public const int HolyCurtainBase = 1390;
    public const int FireBurnBase = 1630;
    public const int SculptureFragment = 1349;
    public const int FireFlowerBase = 60;
}

/// <summary>headless 接缝：时钟 / 自身死亡 / 特效库数 / 火墙淡化开关。</summary>
public static class ClEventEnv
{
    public static Func<uint> NowFn = () => 0;
    public static Func<bool> SelfDeadFn = () => false;
    public static Func<int> EffectImageCountFn = () => 0;
    public static Func<bool> DimFireEffectFn = () => false;

    public static uint Now() => NowFn();
    public static bool SelfDead => SelfDeadFn();

    public static void Reset()
    {
        NowFn = () => 0;
        SelfDeadFn = () => false;
        EffectImageCountFn = () => 0;
        DimFireEffectFn = () => false;
    }
}

/// <summary>DrawEvent 绘制接收器（headless：测试用记录实现复现 GameCanvas 调用序）。</summary>
public interface IEventCanvas
{
    void Draw(int x, int y, EventImage image);
    void DrawBlend(int x, int y, EventImage image);
    void DrawAlpha(int x, int y, EventImage image, int alpha);
    void DrawColorAlpha(int x, int y, EventImage image, int color, int alpha);
}

/// <summary>
/// clEvent.pas TClEvent（201-445）1:1：事件类型驱动的图号选择（LoadSurface）、
/// 帧推进与各类型循环帧数/循环后隐身/首帧音效（Run）、火墙淡化与泉水合成绘制（DrawEvent）。
/// LoadSurface 调度以 LoadSurfaceRequests 计数复现 PlayScene.LoadSurface(LoadSurface) 排队。
/// </summary>
public class ClEvent
{
    public int X, Y, Dir, Px, Py;
    public int EventType, EventParam;
    public long ServerId;
    public EventImage? Surface;
    public bool Blend;
    public uint FrameTime, FrameTickTime;
    public int CurFrame;
    public int Light;
    public bool Visible = true;
    public uint LoadSurfaceTime;
    public bool KeepShow, LoadSound;
    public int LoadSurfaceRequests;
    /// <summary>m_dwGhostTick（ClearEvents 入闲链时间戳）。</summary>
    public uint GhostTick;
    /// <summary>AddEvent 去重命中时弃置新事件（Delphi evn.Free 的 headless 标记）。</summary>
    public bool Discarded;

    public ClEvent(long svid, int ax, int ay, int evtype)
    {
        ServerId = svid;
        X = ax;
        Y = ay;
        EventType = evtype;
        EventParam = 0;
        Blend = false;
        FrameTime = ClEventEnv.Now();
        CurFrame = 0;
        Light = 0;
        Visible = true;
        FrameTickTime = 20;
        Surface = null;
        LoadSurfaceTime = 0;
        KeepShow = false;
        LoadSound = false;
    }

    /// <summary>PlayScene.LoadSurface(LoadSurface) 排队（headless 计数）。</summary>
    protected void RequestLoadSurface() => LoadSurfaceRequests++;

    /// <summary>死亡灰度取图：g_MySelf 非空且 m_boDeath 时取灰图（各分支 1:1）。</summary>
    protected EventImage Img(string list, int imageIndex) =>
        new(list, imageIndex, Px, Py, ClEventEnv.SelfDead);

    public virtual void LoadSurface()
    {
        if (!Visible)
            return;
        LoadSurfaceTime = ClEventEnv.Now();
        Surface = null;
        switch (EventType)
        {
            case Grobal2Const.ET_DIGOUTZOMBI:
                Surface = Img(ClEventImages.Mon6, ClEventConsts.ZombieDigUpDustBase + Dir);
                break;
            case Grobal2Const.ET_PILESTONES:
                Surface = Img(ClEventImages.Effect, ClEventConsts.StoneFragmentBase + (EventParam - 1));
                break;
            case Grobal2Const.ET_HOLYCURTAIN:
                Surface = Img(ClEventImages.Magic, ClEventConsts.HolyCurtainBase + (CurFrame % 10));
                break;
            case Grobal2Const.ET_SAFERECT:
            {
                int offset = Dir switch
                {
                    0 => 2050,  // DR_UP
                    1 => 2060,  // DR_UPRIGHT
                    2 => 2070,  // DR_RIGHT
                    3 => 2080,  // DR_DOWNRIGHT
                    4 => 2090,  // DR_DOWN
                    5 => 2100,  // DR_DOWNLEFT
                    6 => 2110,  // DR_LEFT
                    7 => 2040,  // DR_UPLEFT
                    _ => -1,
                };
                if (offset > 0)
                {
                    Surface = Img(ClEventImages.Magic10, offset);
                    Px = Dir is 5 or 7 ? 6 : 0;
                    Py = 0;
                }
                break;
            }
            case Grobal2Const.ET_FIRE:
                Surface = Img(ClEventImages.Magic, ClEventConsts.FireBurnBase + (CurFrame / 2 % 6));
                break;
            case Grobal2Const.ET_FIRELevel1 or Grobal2Const.ET_FIRELevel2 or Grobal2Const.ET_FIRELevel3:
                Surface = Img(ClEventImages.Magic7x16, 90 + (EventType - Grobal2Const.ET_FIRELevel1) * 10 + (CurFrame / 2 % 8));
                break;
            case Grobal2Const.ET_SCULPEICE:
                Surface = Img(ClEventImages.Mon7, ClEventConsts.SculptureFragment);
                break;
            case >= Grobal2Const.ET_FIREFLOWER_1 and <= Grobal2Const.ET_FIREFLOWER_8:
                Surface = Img(ClEventImages.Magic3, ClEventConsts.FireFlowerBase + 20 * (EventType - Grobal2Const.ET_FIREFLOWER_1) + CurFrame);
                break;
            case Grobal2Const.ET_ICEPEAK:
                Surface = Img(ClEventImages.Mon27, 2010 + 10 * EventParam + 9);
                break;
            case Grobal2Const.ET_HOLYCURTAIN2:
                Surface = Img(ClEventImages.Magic10, 970 + (CurFrame % 15));
                break;
            case Grobal2Const.ET_THUNDER:
                Surface = Img(ClEventImages.Dragon, 420 + CurFrame);
                break;
            case Grobal2Const.ET_LAVA:
                Surface = Img(ClEventImages.Dragon, 470 + CurFrame);
                break;
            case Grobal2Const.ET_FIREDRAGON:
                Surface = Img(ClEventImages.Dragon, 350 + CurFrame);
                break;
            case Grobal2Const.ET_FIREMON33_7:
                Surface = Img(ClEventImages.Mon33, 2670 + CurFrame);
                break;
            case Grobal2Const.ET_DEDING:
                Surface = Img(ClEventImages.Mon14, 410 + CurFrame);
                break;
            case Grobal2Const.ET_FLASHLIGHT:
                Surface = Img(ClEventImages.Magic3, 20 + CurFrame);
                break;
            case Grobal2Const.ET_LAVA2:
                Surface = Img(ClEventImages.Dragon, 440 + CurFrame);
                break;
            case >= Grobal2Const.ET_DOOR1 and <= Grobal2Const.ET_DOOR5:
                Surface = Img(ClEventImages.NpcImg0, 4490 + (EventType - Grobal2Const.ET_DOOR1) * 10 + CurFrame);
                break;
            case Grobal2Const.ET_THUNDER2:
                Surface = Img(ClEventImages.Magic2, 10 + CurFrame);
                break;
            case >= Grobal2Const.ET_SPRINGS1 and <= Grobal2Const.ET_SPRINGS3:
                Surface = Img(ClEventImages.Main2, 550 + CurFrame);
                break;
            case Grobal2Const.ET_SPRINGS_LIGHT:
                Surface = Img(ClEventImages.Main2, 670 + CurFrame);
                break;
            case >= Grobal2Const.ET_CUSTOM_SAFE_POINT1 and <= Grobal2Const.ET_CUSTOM_SAFE_POINT2:
                Surface = Img(ClEventImages.SafePoint, (EventType - Grobal2Const.ET_CUSTOM_SAFE_POINT1) * 10 + (CurFrame % 10));
                break;
        }
    }

    public virtual void DrawEvent(int ax, int ay, IEventCanvas canvas)
    {
        if (!Visible)
            return;
        if (Surface == null)
            return;
        int nPx = Px, nPy = Py;

        // 几张图合成一个效果 chongchong 2013-09-14（泉水）
        if (EventType is >= Grobal2Const.ET_SPRINGS1 and <= Grobal2Const.ET_SPRINGS3)
        {
            var rrr = Img(ClEventImages.Main2, 530 + EventType - Grobal2Const.ET_SPRINGS1);
            if (Blend)
                canvas.DrawBlend(ax + Px, ay + Py, rrr);
            else
                canvas.Draw(ax + Px, ay + Py, rrr);
            // 对坐标，让泉水和泉眼对上
            nPx = 7;
            nPy = -50;
        }

        // HZQ 20230601 火墙(5)淡化 g_ClientConfig.boDimFireEffect
        if (EventType == 5 && ClEventEnv.DimFireEffectFn())
        {
            if (Blend)
                canvas.DrawColorAlpha(ax + nPx, ay + nPy, Surface, unchecked((int)0x00FFFFFF), 96);
            else
                canvas.DrawAlpha(ax + nPx, ay + nPy, Surface, 96);
        }
        else
        {
            if (Blend)
                canvas.DrawBlend(ax + nPx, ay + nPy, Surface);
            else
                canvas.Draw(ax + nPx, ay + nPy, Surface);
        }
    }

    public virtual void Run()
    {
        if (!Visible)
            return;
        int dwCurHolyCurtainframe = CurFrame % 10;
        int dwCurHolyCurtainframe2 = CurFrame % 15;
        int dwCurFireframe = 0;

        if (EventType is Grobal2Const.ET_FIRELevel1 or Grobal2Const.ET_FIRELevel2 or Grobal2Const.ET_FIRELevel3)
            dwCurFireframe = CurFrame / 2 % 8;
        else if (EventType == Grobal2Const.ET_FIRE)
            dwCurFireframe = CurFrame / 2 % 6;

        if (ClEventEnv.Now() - FrameTime > FrameTickTime)
        {
            FrameTime = ClEventEnv.Now();
            CurFrame++;
        }

        switch (EventType)
        {
            case Grobal2Const.ET_DIGOUTZOMBI:
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_PILESTONES:
            {
                int dwCurframe = EventParam;
                if (EventParam <= 0) EventParam = 1;
                if (EventParam > 5) EventParam = 5;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurframe != EventParam)
                    RequestLoadSurface();
                break;
            }
            case Grobal2Const.ET_HOLYCURTAIN:
                Blend = true;
                Light = 1;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurHolyCurtainframe != CurFrame % 10)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_SAFERECT:
                Blend = true;
                Light = 1;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_FIRE:
                Blend = true;
                Light = 1;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame / 2 % 6)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_FIRELevel1 or Grobal2Const.ET_FIRELevel2 or Grobal2Const.ET_FIRELevel3:
                Blend = true;
                Light = 2;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame / 2 % 8)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_SCULPEICE:
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000)
                    RequestLoadSurface();
                break;
            case >= Grobal2Const.ET_FIREFLOWER_1 and <= Grobal2Const.ET_FIREFLOWER_8:
                if (CurFrame >= 20)
                {
                    CurFrame = 0;
                    Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 0);
                        LoadSound = true;
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case Grobal2Const.ET_ICEPEAK:
                Blend = false;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || Surface == null)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_HOLYCURTAIN2:
                Blend = true;
                Light = 1;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurHolyCurtainframe2 != CurFrame % 15)
                    RequestLoadSurface();
                break;
            case Grobal2Const.ET_THUNDER:
                if (CurFrame >= 4)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 1923);
                        LoadSound = true;
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case Grobal2Const.ET_LAVA:
                if (CurFrame >= 10)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 11055);
                        LoadSound = true;
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case Grobal2Const.ET_FIREDRAGON:
                if (CurFrame >= 35)
                {
                    CurFrame = 0;
                    Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case Grobal2Const.ET_FIREMON33_7:
                if (CurFrame >= 7)
                {
                    CurFrame = 1;
                    //m_boVisible := False;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case Grobal2Const.ET_DEDING:
                if (CurFrame >= 6)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_FLASHLIGHT:
                Blend = true;
                Light = 1;
                if (CurFrame >= 10)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_LAVA2:
                Blend = true;
                Light = 1;
                if (CurFrame >= 20)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 10090);
                        LoadSound = false; // Delphi 原义：置 False（非 True），可重复触发
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_FIREDRAGON2:
                if (CurFrame >= 35)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 10090);
                        LoadSound = false;
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                Blend = true;
                Light = 1;
                break;
            case >= Grobal2Const.ET_DOOR1 and <= Grobal2Const.ET_DOOR5:
                Blend = true;
                Light = 1;
                if (CurFrame >= 10)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_THUNDER2:
                Blend = true;
                Light = 1;
                if (CurFrame >= 5)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (CurFrame == 0 && !LoadSound)
                    {
                        SoundPlayed?.Invoke(this, 1923);
                        LoadSound = false;
                    }
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_SPRINGS1 or Grobal2Const.ET_SPRINGS2 or Grobal2Const.ET_SPRINGS3:
                Blend = true;
                Light = 1;
                if (CurFrame >= 12)
                {
                    CurFrame = 0;
                    if (!KeepShow)
                        Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case Grobal2Const.ET_SPRINGS_LIGHT:
                Blend = true;
                Light = 1;
                if (CurFrame >= 18)
                {
                    CurFrame = 0;
                    Visible = false;
                }
                else
                {
                    if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurFireframe != CurFrame)
                        RequestLoadSurface();
                }
                break;
            case >= Grobal2Const.ET_CUSTOM_SAFE_POINT1 and <= Grobal2Const.ET_CUSTOM_SAFE_POINT2:
                Blend = EventType <= 55;
                Light = 1;
                if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurHolyCurtainframe != CurFrame % 10)
                    RequestLoadSurface();
                break;
        }
    }

    /// <summary>首帧音效回调（g_PlaySound.PlaySound 的 headless 接缝；参数=音效 id，烟花记 0）。</summary>
    public static Action<ClEvent, int>? SoundPlayed;
}

/// <summary>clEvent.pas TMapEffectEvent（130-199）：MAPEFFECT 脚本地图特效（循环次数控制隐身）。</summary>
public class MapEffectEvent : ClEvent
{
    public int FileIndex, ImageIndex, ImageCount, LoopCount;
    private int _playCount;

    public MapEffectEvent(long nServerId, int nX, int nY, int nFileIndex, int nImageIndex,
        int nImageCount, int nSpeedTime, int nLoopCount, bool boBlend, int btLight)
        : base(nServerId, nX, nY, Grobal2Const.ET_MAPEFFECT)
    {
        Blend = boBlend;
        FileIndex = nFileIndex;
        ImageIndex = nImageIndex;
        ImageCount = nImageCount;
        FrameTickTime = (uint)nSpeedTime;
        LoopCount = nLoopCount;
        CurFrame = -1;
        Light = btLight;
        _playCount = 0;
    }

    public override void LoadSurface()
    {
        if (!Visible)
            return;
        if (FileIndex < 0 || FileIndex >= ClEventEnv.EffectImageCountFn())
            return;
        LoadSurfaceTime = ClEventEnv.Now();
        Surface = Img(ClEventImages.EffectList, ImageIndex + CurFrame);
    }

    public override void Run()
    {
        if (!Visible)
            return;
        if (FileIndex < 0 || FileIndex >= ClEventEnv.EffectImageCountFn())
            return;
        int dwCurframe = CurFrame;
        if (ClEventEnv.Now() - FrameTime > FrameTickTime)
        {
            FrameTime = ClEventEnv.Now();
            CurFrame++;
        }
        if (CurFrame < 0 || CurFrame >= ImageCount)
        {
            CurFrame = 0;
            _playCount++;
        }
        if (_playCount > LoopCount)
        {
            Visible = false;
            return;
        }
        if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurframe != CurFrame)
            RequestLoadSurface();
    }
}

/// <summary>自定义怪攻击目标/爆燃配置组（g_CustomMonsterConfig.AttackConfigs[AttackIndex] headless 镜像）。</summary>
public sealed class CustomMonsterAttackEff
{
    public int TargetDrawMode = 1;      // TCustomDrawMode: mdmBlend=0, mdmNormal=1
    public int TargetDrawMode2 = 1;
    public int TargetPlayTime = 20;
    public int FlyStartIndex, FlyPlayCount;
    public int ExplosionStartIndex, ExplosionStartIndex2 = -1, ExplosionPlayCount;
    public bool ExplosionKeepPlay;
    public int ExplosionKeepTime;
    public int ExplosionFile = -1;
    public int ExplosionKeepLightRange;
    public int TargetFile = -1;
    public int TargetStartIndex, TargetStartIndex2 = -1, TargetPlayCount;
    public int TargetKeepLightRange;
}

/// <summary>自定义魔法目标配置组（g_CustomMagicConfig.MagicConfigs[PlusLevel] headless 镜像）。</summary>
public sealed class CustomMagicEff
{
    public int TargetDrawMode = 1;
    public int TargetDrawMode2 = 1;
    public int TargetPlayTime = 20;
    public int TargetFile = -1;
    public int TargetStartIndex, TargetStartIndex2 = -1, TargetPlayCount;
    public int TargetKeepLightRange;
}

/// <summary>自定义特效配置解析接缝（TList 线性查找 wMonsterAppr/wMagicID 的 headless 化）。</summary>
public static class CustomEventEffEnv
{
    /// <summary>(wAppr, AttackIndex) → 配置；未命中返回 null（g_CustomMonsterConfig 无匹配）。</summary>
    public static Func<int, int, CustomMonsterAttackEff?> AttackEffFn = (_, _) => null;
    /// <summary>(wMagicID, plusLevel 桶 0..3) → 配置；未命中返回 null。</summary>
    public static Func<int, int, CustomMagicEff?> MagicEffFn = (_, _) => null;

    public static void Reset()
    {
        AttackEffFn = (_, _) => null;
        MagicEffFn = (_, _) => null;
    }
}

/// <summary>clEvent.pas TCustomEffectEvent（1029-1160）：自定义怪目标点特效（Fly 齐备走爆燃循环，否则目标组）。</summary>
public class CustomEffectEvent : ClEvent
{
    private int _fileIndex = -2;
    private int _imageIndex = -1;
    private int _imageIndex2 = -1;
    private int _imageCount;
    private readonly int _appr;
    private bool _blend2;
    private int _px2, _py2;
    private EventImage? _surface2;

    public CustomEffectEvent(long svid, int ax, int ay, int evtype, int wAppr, int attackIndex)
        : base(svid, ax, ay, Grobal2Const.ET_CUSTOM_EFF)
    {
        _appr = wAppr;

        if (attackIndex is >= 0 and <= 5) // Low/High(TClientAttackConfigs) = 攻击1..6
        {
            var cfg = CustomEventEffEnv.AttackEffFn(wAppr, attackIndex);
            if (cfg != null)
            {
                Blend = cfg.TargetDrawMode == 0; // mdmBlend
                _blend2 = cfg.TargetDrawMode2 == 0;
                FrameTickTime = (uint)cfg.TargetPlayTime;

                if (cfg.FlyStartIndex >= 0 && cfg.FlyPlayCount > 0 &&
                    (cfg.ExplosionStartIndex >= 0 || cfg.ExplosionStartIndex2 >= 0) &&
                    cfg.ExplosionPlayCount > 0 && cfg.ExplosionKeepPlay && cfg.ExplosionKeepTime > 0)
                {
                    _fileIndex = cfg.ExplosionFile;
                    _imageIndex = cfg.ExplosionStartIndex;
                    _imageIndex2 = cfg.ExplosionStartIndex2;
                    _imageCount = cfg.ExplosionPlayCount;
                    Light = cfg.ExplosionKeepLightRange;
                }
                else
                {
                    _fileIndex = cfg.TargetFile;
                    _imageIndex = cfg.TargetStartIndex;
                    _imageIndex2 = cfg.TargetStartIndex2;
                    _imageCount = cfg.TargetPlayCount;
                    Light = cfg.TargetKeepLightRange;
                }
            }
        }
    }

    public int FileIndex => _fileIndex;
    public int ImageIndex => _imageIndex;
    public int ImageIndex2 => _imageIndex2;
    public int ImageCount => _imageCount;
    public EventImage? Surface2 => _surface2;

    public override void LoadSurface()
    {
        if (!Visible)
            return;
        if (_fileIndex == -2)
            return;
        if (_imageIndex == -1 && _imageIndex2 == -1)
            return;
        if (_imageCount == 0)
            return;

        LoadSurfaceTime = ClEventEnv.Now();

        int count = ClEventEnv.EffectImageCountFn();
        bool inList = _fileIndex >= 0 && _fileIndex < count;
        if (!inList)
        {
            // 回退 g_WMonImages.Images[FAppr]（headless 同一 EffectList 标识 + Appr 记入 Px 语义由调用方区分）
            Surface = new EventImage(ClEventImages.EffectList, _imageIndex + CurFrame, Px, Py, ClEventEnv.SelfDead);
            if (_imageIndex2 >= 0)
                _surface2 = new EventImage(ClEventImages.EffectList, _imageIndex2 + CurFrame, _px2, _py2, ClEventEnv.SelfDead);
            return;
        }

        if (_imageIndex >= 0)
            Surface = Img(ClEventImages.EffectList, _imageIndex + CurFrame);
        if (_imageIndex2 >= 0)
            _surface2 = Img(ClEventImages.EffectList, _imageIndex2 + CurFrame);
    }

    public override void Run()
    {
        if (!Visible)
            return;
        if (_fileIndex >= -1 && _fileIndex < ClEventEnv.EffectImageCountFn())
        {
            int dwCurframe = CurFrame;
            if (ClEventEnv.Now() - FrameTime > FrameTickTime)
            {
                FrameTime = ClEventEnv.Now();
                CurFrame++;
            }
            if (CurFrame < 0 || CurFrame >= _imageCount)
                CurFrame = 0;
            if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurframe != CurFrame)
                RequestLoadSurface();
        }
    }

    public override void DrawEvent(int ax, int ay, IEventCanvas canvas)
    {
        if (!Visible)
            return;
        base.DrawEvent(ax, ay, canvas);

        if (_surface2 != null)
        {
            int nPx = _px2, nPy = _py2;
            if (_blend2)
                canvas.DrawBlend(ax + nPx, ay + nPy, _surface2);
            else
                canvas.Draw(ax + nPx, ay + nPy, _surface2);
        }
    }
}

/// <summary>clEvent.pas TCustomMagicEffectEvent（1164-1281）：自定义魔法目标点特效（按加成等级桶取配置）。</summary>
public class CustomMagicEffectEvent : ClEvent
{
    private int _fileIndex = -2;
    private int _imageIndex = -1;
    private int _imageIndex2 = -1;
    private int _imageCount;
    private bool _blend2;
    private int _px2, _py2;
    private EventImage? _surface2;

    public CustomMagicEffectEvent(long svid, int ax, int ay, int evtype, int wMagicId, int wNewLevel)
        : base(svid, ax, ay, Grobal2Const.ET_CUSTOM_EFF + wMagicId) // 自定义特效可多次播放
    {
        // TMagicPlusLevel: 0→mplNone(0)、1..3→mpl1_3(1)、4..6→mpl4_6(2)、else→mpl7_9(3)
        int plusLevel = wNewLevel switch
        {
            0 => 0,
            >= 1 and <= 3 => 1,
            >= 4 and <= 6 => 2,
            _ => 3,
        };

        var cfg = CustomEventEffEnv.MagicEffFn(wMagicId, plusLevel);
        if (cfg != null)
        {
            Blend = cfg.TargetDrawMode == 0;
            _blend2 = cfg.TargetDrawMode2 == 0;
            FrameTickTime = (uint)cfg.TargetPlayTime;
            _fileIndex = cfg.TargetFile;
            _imageIndex = cfg.TargetStartIndex;
            _imageIndex2 = cfg.TargetStartIndex2;
            _imageCount = cfg.TargetPlayCount;
            Light = cfg.TargetKeepLightRange;
        }
    }

    public int FileIndex => _fileIndex;
    public int ImageIndex => _imageIndex;
    public EventImage? Surface2 => _surface2;

    public override void LoadSurface()
    {
        if (!Visible)
            return;
        int count = ClEventEnv.EffectImageCountFn();
        if (_fileIndex < 0 || _fileIndex >= count)
            return;
        if (_imageIndex < 0 && _imageIndex2 < 0)
            return;
        if (_imageCount == 0)
            return;

        LoadSurfaceTime = ClEventEnv.Now();

        if (_imageIndex >= 0)
            Surface = Img(ClEventImages.EffectList, _imageIndex + CurFrame);
        if (_imageIndex2 >= 0)
            _surface2 = Img(ClEventImages.EffectList, _imageIndex2 + CurFrame);
    }

    public override void Run()
    {
        if (!Visible)
            return;
        if (_fileIndex >= -1 && _fileIndex < ClEventEnv.EffectImageCountFn())
        {
            int dwCurframe = CurFrame;
            if (ClEventEnv.Now() - FrameTime > FrameTickTime)
            {
                FrameTime = ClEventEnv.Now();
                CurFrame++;
            }
            if (CurFrame < 0 || CurFrame >= _imageCount)
                CurFrame = 0;
            if (ClEventEnv.Now() - LoadSurfaceTime >= 2 * 1000 || dwCurframe != CurFrame)
                RequestLoadSurface();
        }
    }

    public override void DrawEvent(int ax, int ay, IEventCanvas canvas)
    {
        if (!Visible)
            return;
        base.DrawEvent(ax, ay, canvas);

        if (_surface2 != null)
        {
            int nPx = _px2, nPy = _py2;
            if (_blend2)
                canvas.DrawBlend(ax + nPx, ay + nPy, _surface2);
            else
                canvas.Draw(ax + nPx, ay + nPy, _surface2);
        }
    }
}

/// <summary>clEvent.pas TClEventManager（823-1025）：事件去重登记/按 id 删除/查找/批量 Run。</summary>
public class ClEventManager
{
    public sealed record DiscardedEvent(ClEvent Event);

    public List<ClEvent> EventList { get; } = new();

    /// <summary>ClearEvents 时入闲链（AddFreeEventList headless 接缝）。</summary>
    public Action<ClEvent>? AddFreeEvent;

    public void ClearEvents(uint now)
    {
        foreach (var evn in EventList)
        {
            evn.GhostTick = now;
            AddFreeEvent?.Invoke(evn);
        }
        EventList.Clear();
    }

    /// <summary>
    /// AddEvent（882-958）1:1：普通事件按 类型/参数/X/Y/Dir 全匹配去重（烟花 79..86 除外可重复），
    /// MAPEFFECT 另加 FileIndex/ImageIndex/ImageCount/LoopCount；命中 → 旧事件保留、新事件弃置（Delphi evn.Free）。
    /// 返回新事件（Discarded=true 表示被弃置，Delphi 语义为已释放）。
    /// </summary>
    public ClEvent AddEvent(ClEvent evn)
    {
        evn.Discarded = false;
        bool boFind = false;
        if (evn is MapEffectEvent mapEvent1)
        {
            foreach (var e in EventList)
            {
                if (e is not MapEffectEvent mapEvent2)
                    continue;
                if (mapEvent2.EventType == mapEvent1.EventType &&
                    mapEvent2.EventParam == mapEvent1.EventParam &&
                    mapEvent2.X == mapEvent1.X && mapEvent2.Y == mapEvent1.Y &&
                    mapEvent2.Dir == mapEvent1.Dir && mapEvent2.FileIndex == mapEvent1.FileIndex &&
                    mapEvent2.ImageIndex == mapEvent1.ImageIndex &&
                    mapEvent2.ImageCount == mapEvent1.ImageCount &&
                    mapEvent2.LoopCount == mapEvent1.LoopCount)
                {
                    mapEvent1.CurFrame = 0;
                    mapEvent1.Visible = true;
                    boFind = true;
                    break;
                }
            }
        }
        else
        {
            foreach (var e in EventList)
            {
                if (e.EventType == evn.EventType &&
                    e.EventParam == evn.EventParam &&
                    e.X == evn.X && e.Y == evn.Y &&
                    e.Dir == evn.Dir &&
                    !(e.EventType >= Grobal2Const.ET_FIREFLOWER_1 && e.EventType <= Grobal2Const.ET_FIREFLOWER_8))
                {
                    evn.CurFrame = 0;
                    evn.Visible = true;
                    boFind = true;
                    break;
                }
            }
        }

        if (!boFind)
        {
            EventList.Add(evn);
        }
        else
        {
            evn.Discarded = true; // Delphi: evn.Free
        }
        return evn;
    }

    public void DelEvent(ClEvent evn)
    {
        for (int i = 0; i < EventList.Count; i++)
        {
            if (ReferenceEquals(EventList[i], evn))
            {
                evn.Visible = false;
                break;
            }
        }
    }

    public void DelEventById(long svid)
    {
        foreach (var evn in EventList)
        {
            if (evn.ServerId == svid)
            {
                evn.Visible = false;
                break;
            }
        }
    }

    public ClEvent? GetEvent(int ax, int ay, int etype)
    {
        foreach (var e in EventList)
        {
            if (e.X == ax && e.Y == ay && e.EventType == etype)
                return e;
        }
        return null;
    }

    public void Execute()
    {
        foreach (var e in EventList)
            e.Run();
    }
}
