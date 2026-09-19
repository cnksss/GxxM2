namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas 属性基础配置体系数据层（批次J5 前置：ViewLevel/RecalcHuman 依赖）。
/// TBaseAbilInfo/TActorBaseAbil/THumBaseAbil/TBaseAbilConfig 全字段 1:1；
/// RecalcLevelAbilitys 算法体于下一批次接入（ViewLevel/HumanInfo 窗体依赖它）。
/// </summary>
public struct TBaseAbilInfo
{
    public uint AC1;
    public uint AC2;
    public uint MAC1;
    public uint MAC2;
    public uint DC1;
    public uint DC2;
    public uint MC1;
    public uint MC2;
    public uint SC1;
    public uint SC2;
    public uint MaxHP;
    public uint MaxMP;
    public int MaxWeight;       // 背包
    public int MaxWearWeight;   // 负重
    public int MaxHandWeight;   // 腕力
}

/// <summary>TActorBaseAbil：单职业 0..999 级基础属性表 + Add 增量。</summary>
public class TActorBaseAbil
{
    public bool AutoCalcLevel1000;
    public TBaseAbilInfo[] Base = new TBaseAbilInfo[1000];
    public TBaseAbilInfo Add;
}

/// <summary>THumBaseAbil = array[JOB_WARR..JOB_TAOS] of TActorBaseAbil（3 职业）。</summary>
public class THumBaseAbil
{
    public const int JOB_WARR = 0;
    public const int JOB_WIZ = 1;
    public const int JOB_TAOS = 2;

    public readonly TActorBaseAbil[] Items = new TActorBaseAbil[3];

    public THumBaseAbil()
    {
        for (int i = 0; i < Items.Length; i++) Items[i] = new TActorBaseAbil();
    }

    public TActorBaseAbil this[int job] => Items[job];
}

/// <summary>TBaseAbilConfig：UseDefault（引擎默认表）/自定义（HumAbil/HeroAbil）。</summary>
public class TBaseAbilConfig
{
    public bool UseDefault = true; // Delphi 初始化：g_BaseAbilConfig.UseDefault := True
    public readonly THumBaseAbil HumAbil = new();
    public readonly THumBaseAbil HeroAbil = new();
}

public static class M2ShareAbilConfig
{
    /// <summary>M2Share 全局 g_BaseAbilConfig。</summary>
    public static readonly TBaseAbilConfig g_BaseAbilConfig = new();

    /// <summary>FillChar 等效复位（UseDefault 保持 True 与 Delphi 初始化一致）。</summary>
    public static void ResetDefaults()
    {
        g_BaseAbilConfig.UseDefault = true;
        g_BaseAbilConfig.HumAbil.Items[0] = new TActorBaseAbil();
        g_BaseAbilConfig.HumAbil.Items[1] = new TActorBaseAbil();
        g_BaseAbilConfig.HumAbil.Items[2] = new TActorBaseAbil();
        g_BaseAbilConfig.HeroAbil.Items[0] = new TActorBaseAbil();
        g_BaseAbilConfig.HeroAbil.Items[1] = new TActorBaseAbil();
        g_BaseAbilConfig.HeroAbil.Items[2] = new TActorBaseAbil();
    }
}
