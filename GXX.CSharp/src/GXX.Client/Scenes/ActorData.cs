using System;
using System.IO;
using GXX.Client.Scenes;

#pragma warning disable CS0649 // Delphi record 字段按原义命名（machines / 下划线字段保留）

namespace GXX.Client.Scenes;

/// <summary>Actor.pas 80-97 THealthNumber（飘血数字）。</summary>
public class THealthNumber
{
    public string sNumber = "";
    public int nNumber;
    public int nOffsetX;
    public int nOffsetY;
    public int nWidth;
    public int nHeight;
    public uint dwUpdateHPTick;
    public uint dwStartHPTick;
    public int nResID;
    public int nResStartIdx;
    public int nNmType;
    public int nDrawStyle;
    public byte byAlpha;

    /// <summary>ImageIndexs:array of Integer（批次J80 补全；原文动态数组）。</summary>
    public System.Collections.Generic.List<int> ImageIndexs = new();
}

/// <summary>Actor.pas 99-120 TCustomMagicStatusEffect（自定义魔法状态特效）。</summary>
public struct TCustomMagicStatusEffect
{
    public bool boShow;
    public short Status1_File;
    public ushort Status1_StartIndex;
    public ushort Status1_PlayCount;
    public ushort Status1_EmptyCount;
    public int Status1_DrawMode;
    public bool Status1_CalcDir;
    public short Status2_File;
    public ushort Status2_StartIndex;
    public ushort Status2_PlayCount;
    public ushort Status2_EmptyCount;
    public int Status2_DrawMode;
    public bool Status2_CalcDir;
    public uint m_nGenAniTick;
    public int m_nGenAniIndex;
    public int m_nStruck;
}

/// <summary>Actor.pas 123-143 TClientActorEffect（脚本播放特效）。</summary>
public struct TClientActorEffect
{
    public short nEffectFileIndex;
    public int nEffectImageOffSet;
    public ushort wEffectImageCount;
    public ushort wEffectFrameTime;
    public int nOldLoopCount;
    public int nLoopCount;
    public int nOldCurrentFrame;
    public int nCurrentFrame;
    public uint dwEffectTick;
    public object Texture;
    public int nX, nY;
    public byte btDrawOrder;
    public int nOffsetX, nOffsetY;
    public bool boBlendMode;
    public bool boWantDelete;
}

/// <summary>
/// 播放特效队列元素（Actor.pas 123-143 + 144 `pTClientActorEffect = ^TClientActorEffect`）。
/// Delphi 侧 `m_ActorEffects` 存的是**指针**，故推进/删除作用于同一实例；
/// C# 侧用引用类型承载这一语义（struct 版 TClientActorEffect 保留为线格式定义）。
/// </summary>
public sealed class TClientActorEffectRef
{
    public short nEffectFileIndex;
    public int nEffectImageOffSet;
    public ushort wEffectImageCount;
    public ushort wEffectFrameTime;
    public int nOldLoopCount;
    public int nLoopCount;
    public int nOldCurrentFrame;
    public int nCurrentFrame;
    public uint dwEffectTick;
    public object? Texture;
    public int nX, nY;
    public byte btDrawOrder;
    public int nOffsetX, nOffsetY;
    public bool boBlendMode;
    public bool boWantDelete;

    /// <summary>按线格式结构体复制一份（便于与 struct 版本互转）。</summary>
    public TClientActorEffect ToStruct() => new()
    {
        nEffectFileIndex = nEffectFileIndex,
        nEffectImageOffSet = nEffectImageOffSet,
        wEffectImageCount = wEffectImageCount,
        wEffectFrameTime = wEffectFrameTime,
        nOldLoopCount = nOldLoopCount,
        nLoopCount = nLoopCount,
        nOldCurrentFrame = nOldCurrentFrame,
        nCurrentFrame = nCurrentFrame,
        dwEffectTick = dwEffectTick,
        Texture = Texture!,
        nX = nX,
        nY = nY,
        btDrawOrder = btDrawOrder,
        nOffsetX = nOffsetX,
        nOffsetY = nOffsetY,
        boBlendMode = boBlendMode,
        boWantDelete = boWantDelete,
    };
}

/// <summary>magiceff.pas 45-52 TMagicType（魔法特效类型）。</summary>
public enum TMagicType
{
    mtReady, mtFly, mtExplosion, mtFlyAxe, mtFireWind, mtFireGun,
    mtLightingThunder, mtThunder, mtExploBujauk, mtBujaukGroundEffect, mtKyulKai, mtFlyArrow,
    mt12, mt13, mt14, mt15, mt16, mtRedThunder, mtLava, mtFlyArrowEx,
}

/// <summary>magiceff.pas 54-67 TUseMagicInfo（服务端下发的施法信息）。</summary>
public struct TUseMagicInfo
{
    public int ServerMagicCode;
    public int MagicSerial;
    public long target;
    public TMagicType EffectType;
    public int EffectNumber;
    public int targx;
    public int targy;
    public bool Recusion;
    public int anitime;
    public byte NewLevel;
    public byte MagicLevel;
    public bool MagicItemType;
}

/// <summary>magiceff.pas 18-36 常量。</summary>
public static class MagicEffConsts
{
    public const int MG_READY = 10;
    public const int MG_FLY = 6;
    public const int MG_EXPLOSION = 10;
    public const int READYTIME = 120;
    public const int EXPLOSIONTIME = 100;
    public const int FLYBASE = 10;
    public const int EXPLOSIONBASE = 170;
    public const int MAXMAGIC = 10;
    public const int FLYOMAAXEBASE = 447;
    public const int THORNBASE = 2967;
    public const int ARCHERBASE = 2607;
    public const int ARCHERBASE2 = 272;
    public const int FLYFORSEC = 500;
    public const int FIREGUNFRAME = 6;
    public const int MAXMAGICTYPE = 16;
}

/// <summary>M2Share.pas 8931 GetMonAction（MONPMFILE = 'Graphics\Monster\%d.pm' 外置动作表覆写；缺文件返回 null）。</summary>
public static class ActorActionFiles
{
    public const string MONPMFILE = "Graphics\\Monster\\{0}.pm";
    public static Func<string> SelfResourcePath = () => "";
    public static Func<string> SelfFilePath = () => ".";

    public static TMonsterAction? GetMonAction(int appr)
    {
        string fileName = SelfResourcePath().Length > 0
            ? SelfResourcePath() + string.Format(MONPMFILE, appr)
            : SelfFilePath() + string.Format(MONPMFILE, appr);
        if (!File.Exists(fileName))
            return null;
        var bytes = File.ReadAllBytes(fileName);
        if (bytes.Length < 108) // sizeof(TMonsterAction) = 9 × 12 (packed)
            return null;
        var result = new TMonsterAction
        {
            ActStand = ReadAction(bytes, 0),
            ActWalk = ReadAction(bytes, 12),
            ActRun = ReadAction(bytes, 24),
            ActAttack = ReadAction(bytes, 36),
            ActCritical = ReadAction(bytes, 48),
            ActStruck = ReadAction(bytes, 60),
            ActDie = ReadAction(bytes, 72),
            ActDeath = ReadAction(bytes, 84),
            ActAttack2 = ReadAction(bytes, 96),
        };
        return result;
    }

    private static TActionInfo ReadAction(byte[] b, int off) => new()
    {
        start = BitConverter.ToInt32(b, off),
        frame = BitConverter.ToUInt16(b, off + 4),
        skip = BitConverter.ToUInt16(b, off + 6),
        ftime = BitConverter.ToUInt16(b, off + 8),
        usetick = BitConverter.ToUInt16(b, off + 10),
    };
}
