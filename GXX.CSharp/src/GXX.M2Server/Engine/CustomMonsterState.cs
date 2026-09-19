using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Crypto;
using GXX.Core.Protocol;
using GXX.Core.Util;
using System.Runtime.InteropServices;

namespace GXX.M2Server.Engine;

/// <summary>
/// uCustomMonsterUtils.pas 1:1（自定义怪物数据层，批次J41）：
/// TAdditionalDamage / TMonsterServerConfig / TMonsterServerBaseConfig /
/// TCustomMonsterConfig（构造默认 + LoadFromIniFile/SaveToIniFile/SetChanged）+
/// SaveCustomMonsterClientConfigs（登录器 .dat：GUID 标志 + 数量 + CRC + TClientCustomMonsterConfig 定长布局）+
/// M2Share 名称表常量。注意与 M2Share.pas 的 TPlayMonsterConfig（人形怪）为两个不同类型。
/// </summary>
public static class CustomMonsterConsts
{
    public static readonly string[] AttackConfigNames = { "攻击1", "攻击2", "攻击3", "攻击4", "攻击5", "攻击6" };
    public static readonly string[] MonsterClientActionNames =
        { "站", "走", "默认攻击", "被攻击", "死亡", "石化苏醒", "攻击1", "攻击2", "攻击3", "攻击4", "攻击5", "攻击6" };
    public static readonly string[] MonsterTypeNames = { "普通怪物", "石化怪物", "苏醒怪物" };
    public static readonly string[] MoveOptionNames = { "自由移动", "不可移动", "守护区域" };
    public static readonly string[] MonsterClientActionSections =
        { "ActStand", "ActWalk", "ActDefAttack", "ActStruck", "ActDie", "ActStoneMode", "ActAttack1", "ActAttack2", "ActAttack3", "ActAttack4", "ActAttack5", "ActAttack6" };
    public static readonly string[] MonsterDrawOrder2Names = { "自身→特效1→特效2", "特效1→自身→特效2", "特效1→特效2→自身" };
    public static readonly string[] MonsterSoundTypeNames =
        { "Normal", "DigUP", "Attack", "Struck", "Die", "Attack1", "Attack2", "Attack3", "Attack4", "Attack5", "Attack6" };

    // M2Share.pas 384-391
    public static readonly string[] CustomOperateModeNames = { "攻击模式", "增益模式" };
    public static readonly string[] CustomAttackModeNames = { "近攻", "远攻" };
    public static readonly string[] CustomDrawModeNames = { "透明绘制", "普通绘制" };
    public static readonly string[] CustomDirNames = { "8方向", "16方向" };
    public static readonly string[] CustomDirCalcTypeNames = { "不算方向", "普通计算", "自我中心多方向" };
    public static readonly string[] CustomAttackTargetNames = { "单体攻击", "群体攻击", "直线攻击", "半月攻击", "8方向攻击", "16方向攻击" };
    public static readonly string[] CustomAttackPowerCalcNames = { "使用DC", "使用MC", "使用SC", "职业计算" };
    public static readonly string[] CustomDrawOrderNames = { "先自身再效果", "先效果再自身" };

    /// <summary>ClientCustomMonsterConfigFlag: TGUID（登录器 .dat 头）。</summary>
    public static readonly Guid ClientCustomMonsterConfigFlag = new("196BE159-E170-41DB-B2F0-35A1D2EBFA8B");
}

public class TAdditionalDamage
{
    public bool Checked;
    public byte Rate;
    public ushort Time;
}

public class TMonsterServerConfig
{
    public bool AttackEnabled;
    public TCustomOperateMode OperateMode;
    public bool AttackSelfDie;
    public int AttackDelayTime;
    public int AttackHPPercent;
    public int AttackRate;
    public int AttackTargetCount;
    public TCustomAttackMode AttackMode;
    public TCustomAttackTarget AttackTarget;
    public TCustomAttackPowerCalc AttackPowerCalc;
    public int AttackPowerRate;
    public bool AttackTeleportAttack;
    public int AttackTeleportTargetDistance;
    public int AttackTeleportDistance;
    public int AttackTeleportRate;
    public bool AttackTeleportRush;
    public bool AttackIgnoreDefence;
    public int AttackNearRange;
    public int AttackGroupRange;
    public bool NearAttackTargetCenter;
    public int AttackPowerInc;
    public TAdditionalDamage[] Additionals = CreateAdditionals();
    public int AdditionalHP0;
    public bool AdditionalHighLevel4;
    public int AdditionalImprisonRange;
    public bool EnabledCallMonster;
    public int CallMonstersRate;
    public string[] CallMonsters = { "", "", "", "" };
    public int[] CallMonsterNums = new int[4];
    public bool MoveTarget;
    public int MoveTargetRate;
    public bool MoveTargetHighLevel;
    public bool ProtectAddHP;
    public int ProtectAddHPRate;
    public int ProtectAddHPPercent;
    public bool ProtectAddDefence;
    public int ProtectAddDefenceRate;
    public int ProtectAddDefencePercent;
    public int ProtectAddDefenceTime;
    public bool ProtectAddMagDefence;
    public int ProtectAddMagDefenceRate;
    public int ProtectAddMagDefencePercent;
    public int ProtectAddMagDefenceTime;
    public bool ProtectAddDC;
    public int ProtectAddDCRate;
    public int ProtectAddDCPercent;
    public int ProtectAddDCTime;
    public bool ProtectAddMC;
    public int ProtectAddMCRate;
    public int ProtectAddMCPercent;
    public int ProtectAddMCTime;
    public bool ProtectAddSC;
    public int ProtectAddSCRate;
    public int ProtectAddSCPercent;
    public int ProtectAddSCTime;
    public int ProtectTargetRange;
    public int ProtectSelfRate;

    internal static TAdditionalDamage[] CreateAdditionals()
    {
        var arr = new TAdditionalDamage[12];
        for (int i = 0; i < 12; i++)
            arr[i] = new TAdditionalDamage();
        return arr;
    }
}

public class TMonsterServerBaseConfig
{
    public byte ViewRange;
    public TMonsterType MonsterType;
    public TMoveOption MoveOption;
    public int ProtectRange;
    public int MinAttackNearRange;
    public byte LightRange;
    public bool NoAttack;
}

public class TCustomMonsterConfig
{
    private bool _isChanged;

    public string MonsterName = "";
    public ushort MonsterRace;
    public ushort MonsterAppr;

    public TClientBaseConfig ClientBaseConfig;                        // Grobal2 packed 结构（定长布局）
    public TMonsterClientActionTypeArray12 ClientActions;             // array[TMonsterClientActionType]
    public TClientAttackConfigArray6 ClientAttackConfigs;             // array[0..5]
    public TMonsterServerBaseConfig ServerBaseConfig = new();
    public TMonsterServerConfig[] MonsterServerConfigs = CreateServerConfigs();

    public TCustomMonsterConfig(string monsterName, ushort monsterRace, ushort monsterAppr)
    {
        MonsterName = monsterName;
        MonsterRace = monsterRace;
        MonsterAppr = monsterAppr;

        ClientBaseConfig = new TClientBaseConfig
        {
            DrawMode = TCustomDrawMode.mdmBlend,
            DrawMode2 = TCustomDrawMode.mdmBlend,
            DrawOrder = TMonsterDrawOrder2.mdoSelf_Eff1_Eff2,
            DieNoCalcDir = 0,
            HPBgOffsetX = 0,
            HPBgOffsetY = 0,
            HPOffsetX = 0,
            HPOffsetY = 0,
            HPFile = -1,
            HPStartIndex = -1,
            HPTextOffsetX = 0,
            HPTextOffsetY = 0,
        };
        for (int i = 0; i < 11; i++)
            ClientBaseConfig.Sounds[i].Value = "";

        for (int i = 0; i < 12; i++)
        {
            ref var action = ref ClientActions[i];
            action.ActionType = (TMonsterClientActionType)i;
            action.ActionFile = -1;
            action.StartIndex = -1;
            action.PlayCount = 0;
            action.EmptyCount = 0;
            action.PlayTime = 100;
            action.EffectFile = -1;
            action.EffectIndex = -1;
            action.EffectFile2 = -1;
            action.EffectIndex2 = -1;
            action.CalcDir = 1;
        }

        for (int i = 0; i < 6; i++)
        {
            ref var cfg = ref ClientAttackConfigs[i];
            cfg.Fly_File = -1;
            cfg.Fly_StartIndex = -1;
            cfg.Fly_PlayCount = 0;
            cfg.Fly_EmptyCount = 0;
            cfg.Fly_PlayTime = 100;
            cfg.Fly_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.Fly_DirCount = TCustomDirCount.mdcDir8;
            cfg.Fly_CalcDir = 1;
            cfg.Fly_LightRange = 0;
            cfg.FlyEff_File = -1;
            cfg.FlyEff_StartIndex = -1;
            cfg.FlyEff_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.Self_File = -1;
            cfg.Self_StartIndex = -1;
            cfg.Self_PlayCount = 0;
            cfg.Self_EmptyCount = 0;
            cfg.Self_PlayTime = 100;
            cfg.Self_DrawOrder = TCustomDrawOrder.mdoPriorSelf;
            cfg.Self_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.Self_DirCalcType = TCustomDirCalcType.mdctNone;
            cfg.Self_DirCount = TCustomDirCount.mdcDir8;
            cfg.Self_PlayDelayAction = 0;
            cfg.Self_LightRange = 0;
            cfg.SelfKeep_File = -1;
            cfg.SelfKeep_StartIndex = 0;
            cfg.SelfKeep_StartIndex2 = -1;
            cfg.SelfKeep_PlayCount = 0;
            cfg.SelfKeep_PlayTime = 0;
            cfg.SelfKeep_DrawOrder = TCustomDrawOrder.mdoPriorSelf;
            cfg.SelfKeep_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.SelfKeep_DrawMode2 = TCustomDrawMode.mdmBlend;
            cfg.SelfKeep_KeepTime = 0;
            cfg.Explosion_File = -1;
            cfg.Explosion_StartIndex = -1;
            cfg.Explosion_StartIndex2 = -1;
            cfg.Explosion_PlayCount = 0;
            cfg.Explosion_PlayTime = 100;
            cfg.Explosion_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.Explosion_DrawMode2 = TCustomDrawMode.mdmBlend;
            cfg.Explosion_LockDraw = 0;
            cfg.Explosion_LightRange = 0;
            cfg.Explosion_KeepPlay = 0;
            cfg.Explosion_KeepTime = 30;
            cfg.Explosion_KeepAttackRange = 0;
            cfg.Explosion_KeepMultiPlay = 0;
            cfg.Explosion_KeepAttackInterval = 5;
            cfg.Explosion_KeepLightRange = 0;
            cfg.Target_File = -1;
            cfg.Target_StartIndex = -1;
            cfg.Target_StartIndex2 = -1;
            cfg.Target_PlayCount = 0;
            cfg.Target_PlayTime = 100;
            cfg.Target_DrawMode = TCustomDrawMode.mdmBlend;
            cfg.Target_DrawMode2 = TCustomDrawMode.mdmBlend;
            cfg.Target_MultiPlay = 0;
            cfg.Target_LockDraw = 0;
            cfg.Target_LightRange = 0;
            cfg.Target_KeepPlay = 0;
            cfg.Target_KeepTime = 30;
            cfg.Target_KeepAttackRange = 0;
            cfg.Target_KeepMultiPlay = 0;
            cfg.Target_KeepAttackInterval = 5;
            cfg.Target_KeepLightRange = 0;
        }

        ServerBaseConfig.ViewRange = 8;
        ServerBaseConfig.MonsterType = TMonsterType.mtNormal;
        ServerBaseConfig.MoveOption = TMoveOption.moMoveNormal;
        ServerBaseConfig.ProtectRange = 0;
        ServerBaseConfig.MinAttackNearRange = 1;
        ServerBaseConfig.LightRange = 0;
        ServerBaseConfig.NoAttack = false;

        foreach (var serverConfig in MonsterServerConfigs)
        {
            serverConfig.AttackEnabled = false;
            serverConfig.OperateMode = TCustomOperateMode.momAttack;
            serverConfig.AttackSelfDie = false;
            serverConfig.AttackDelayTime = 400;
            serverConfig.AttackHPPercent = 101;
            serverConfig.AttackRate = 100;
            serverConfig.AttackTargetCount = 0;
            serverConfig.AttackMode = TCustomAttackMode.mamNear;
            serverConfig.AttackTarget = TCustomAttackTarget.matSingle;
            serverConfig.AttackPowerCalc = TCustomAttackPowerCalc.mapcDC;
            serverConfig.AttackPowerRate = 100;
            serverConfig.AttackTeleportAttack = false;
            serverConfig.AttackTeleportTargetDistance = 5;
            serverConfig.AttackTeleportDistance = 4;
            serverConfig.AttackTeleportRate = 0;
            serverConfig.AttackTeleportRush = false;
            serverConfig.AttackIgnoreDefence = false;
            serverConfig.AttackNearRange = 1;
            serverConfig.AttackGroupRange = 2;
            serverConfig.NearAttackTargetCenter = false;
            serverConfig.AttackPowerInc = 0;
            foreach (var additional in serverConfig.Additionals)
            {
                additional.Checked = false;
                additional.Rate = 3;
                additional.Time = 3;
            }
            serverConfig.AdditionalHP0 = 3;
            serverConfig.AdditionalHighLevel4 = false;
            serverConfig.AdditionalImprisonRange = 3;
            serverConfig.EnabledCallMonster = false;
            serverConfig.CallMonstersRate = 0;
            for (int j = 0; j < 4; j++)
            {
                serverConfig.CallMonsters[j] = "";
                serverConfig.CallMonsterNums[j] = 0;
            }
            serverConfig.MoveTarget = false;
            serverConfig.MoveTargetRate = 0;
            serverConfig.MoveTargetHighLevel = false;
            serverConfig.ProtectAddHP = false;
            serverConfig.ProtectAddHPRate = 0;
            serverConfig.ProtectAddHPPercent = 50;
            serverConfig.ProtectAddDefence = false;
            serverConfig.ProtectAddDefenceRate = 0;
            serverConfig.ProtectAddDefencePercent = 100;
            serverConfig.ProtectAddDefenceTime = 60;
            serverConfig.ProtectAddMagDefence = false;
            serverConfig.ProtectAddMagDefenceRate = 0;
            serverConfig.ProtectAddMagDefencePercent = 100;
            serverConfig.ProtectAddMagDefenceTime = 60;
            serverConfig.ProtectAddDC = false;
            serverConfig.ProtectAddDCRate = 0;
            serverConfig.ProtectAddDCPercent = 10;
            serverConfig.ProtectAddDCTime = 10;
            serverConfig.ProtectAddMC = false;
            serverConfig.ProtectAddMCRate = 0;
            serverConfig.ProtectAddMCPercent = 10;
            serverConfig.ProtectAddMCTime = 10;
            serverConfig.ProtectAddSC = false;
            serverConfig.ProtectAddSCRate = 0;
            serverConfig.ProtectAddSCPercent = 10;
            serverConfig.ProtectAddSCTime = 10;
            serverConfig.ProtectTargetRange = 3;
            serverConfig.ProtectSelfRate = 50;
        }

        LoadFromIniFile();
    }

    private static TMonsterServerConfig[] CreateServerConfigs()
    {
        var arr = new TMonsterServerConfig[6];
        for (int i = 0; i < 6; i++)
            arr[i] = new TMonsterServerConfig();
        return arr;
    }

    public bool IsChanged => _isChanged;

    public void SetChanged(bool value = true) => _isChanged = value;

    /// <summary>Delphi LoadFromIniFile（sSmartMonsterDir\名称.ini；缺文件早退；枚举范围校验后赋值）。</summary>
    public void LoadFromIniFile()
    {
        _isChanged = false;
        string fileName = M2Config.sSmartMonsterDir + MonsterName + ".ini";
        if (!File.Exists(fileName))
            return;
        var ini = new TFastIniFile(fileName);
        try
        {
            string section = "ClientConfig";
            ClientBaseConfig.DrawMode = ReadEnum(ini, section, "DrawMode", (int)ClientBaseConfig.DrawMode, 1, v => (TCustomDrawMode)v);
            ClientBaseConfig.DrawMode2 = ReadEnum(ini, section, "DrawMode2", (int)ClientBaseConfig.DrawMode2, 1, v => (TCustomDrawMode)v);
            ClientBaseConfig.DrawOrder = ReadEnum(ini, section, "DrawOrder", (int)ClientBaseConfig.DrawOrder, 2, v => (TMonsterDrawOrder2)v);
            ClientBaseConfig.DieNoCalcDir = (byte)(ini.ReadBool(section, "DieNoCalcDir", ClientBaseConfig.DieNoCalcDir != 0) ? 1 : 0);
            ClientBaseConfig.HPBgOffsetX = ini.ReadInteger(section, "HPBgOffsetX", ClientBaseConfig.HPBgOffsetX);
            ClientBaseConfig.HPBgOffsetY = ini.ReadInteger(section, "HPBgOffsetY", ClientBaseConfig.HPBgOffsetY);
            ClientBaseConfig.HPOffsetX = ini.ReadInteger(section, "HPOffsetX", ClientBaseConfig.HPOffsetX);
            ClientBaseConfig.HPOffsetY = ini.ReadInteger(section, "HPOffsetY", ClientBaseConfig.HPOffsetY);
            ClientBaseConfig.HPFile = ini.ReadInteger(section, "HPFile", ClientBaseConfig.HPFile);
            ClientBaseConfig.HPStartIndex = ini.ReadInteger(section, "HPStartIndex", ClientBaseConfig.HPStartIndex);
            ClientBaseConfig.HPTextOffsetX = ini.ReadInteger(section, "HPTextOffsetX", ClientBaseConfig.HPTextOffsetX);
            ClientBaseConfig.HPTextOffsetY = ini.ReadInteger(section, "HPTextOffsetY", ClientBaseConfig.HPTextOffsetY);

            section = "ClientSounds";
            for (int i = 0; i < 11; i++)
                ClientBaseConfig.Sounds[i].Value = ini.ReadString(section, CustomMonsterConsts.MonsterSoundTypeNames[i], "");

            for (int i = 0; i < 12; i++)
            {
                ref var action = ref ClientActions[i];
                section = CustomMonsterConsts.MonsterClientActionSections[i];
                action.ActionFile = (short)ini.ReadInteger(section, "ActionFile", action.ActionFile);
                action.StartIndex = (short)ini.ReadInteger(section, "StartIndex", action.StartIndex);
                action.PlayCount = (ushort)ini.ReadInteger(section, "PlayCount", action.PlayCount);
                action.EmptyCount = (ushort)ini.ReadInteger(section, "EmptyCount", action.EmptyCount);
                action.PlayTime = (ushort)ini.ReadInteger(section, "PlayTime", action.PlayTime);
                action.EffectFile = (short)ini.ReadInteger(section, "EffectFile", action.EffectFile);
                action.EffectIndex = (short)ini.ReadInteger(section, "EffectIndex", action.EffectIndex);
                action.EffectFile2 = (short)ini.ReadInteger(section, "EffectFile2", action.EffectFile2);
                action.EffectIndex2 = (short)ini.ReadInteger(section, "EffectIndex2", action.EffectIndex2);
                action.CalcDir = (byte)(ini.ReadBool(section, "CalcDir", action.CalcDir != 0) ? 1 : 0);
            }

            for (int i = 0; i < 6; i++)
                LoadClientAttackSection(ini, i);

            section = "ServerConfig";
            ServerBaseConfig.ViewRange = (byte)ini.ReadInteger(section, "ViewRange", ServerBaseConfig.ViewRange);
            ServerBaseConfig.MonsterType = ReadEnum(ini, section, "MonsterType", (int)ServerBaseConfig.MonsterType, 2, v => (TMonsterType)v);
            ServerBaseConfig.MoveOption = ReadEnum(ini, section, "MoveOption", (int)ServerBaseConfig.MoveOption, 2, v => (TMoveOption)v);
            ServerBaseConfig.ProtectRange = ini.ReadInteger(section, "ProtectRange", ServerBaseConfig.ProtectRange);
            ServerBaseConfig.MinAttackNearRange = ini.ReadInteger(section, "MinAttackNearRange", ServerBaseConfig.MinAttackNearRange);
            ServerBaseConfig.LightRange = (byte)ini.ReadInteger(section, "LightRange", ServerBaseConfig.LightRange);
            ServerBaseConfig.NoAttack = ini.ReadBool(section, "NoAttack", ServerBaseConfig.NoAttack);

            for (int i = 0; i < 6; i++)
                LoadServerAttackSections(ini, MonsterServerConfigs[i], i);
        }
        finally
        {
            ini.Dispose();
        }
    }

    private void LoadClientAttackSection(TFastIniFile ini, int i)
    {
        ref var cfg = ref ClientAttackConfigs[i];
        string section = "ClientAttack" + i;

        cfg.Fly_File = (short)ini.ReadInteger(section, "Fly_File", cfg.Fly_File);
        cfg.Fly_StartIndex = (short)ini.ReadInteger(section, "Fly_StartIndex", cfg.Fly_StartIndex);
        cfg.Fly_PlayCount = (ushort)ini.ReadInteger(section, "Fly_PlayCount", cfg.Fly_PlayCount);
        cfg.Fly_EmptyCount = (ushort)ini.ReadInteger(section, "Fly_EmptyCount", cfg.Fly_EmptyCount);
        cfg.Fly_PlayTime = (ushort)ini.ReadInteger(section, "Fly_PlayTime", cfg.Fly_PlayTime);
        cfg.Fly_DrawMode = ReadEnum(ini, section, "Fly_DrawMode", (int)cfg.Fly_DrawMode, 1, v => (TCustomDrawMode)v);
        cfg.Fly_DirCount = ReadEnum(ini, section, "Fly_DirCount", (int)cfg.Fly_DirCount, 1, v => (TCustomDirCount)v);
        cfg.Fly_CalcDir = (byte)(ini.ReadBool(section, "Fly_CalcDir", cfg.Fly_CalcDir != 0) ? 1 : 0);
        cfg.Fly_LightRange = (byte)ini.ReadInteger(section, "Fly_LightRange", cfg.Fly_LightRange);

        cfg.FlyEff_File = (short)ini.ReadInteger(section, "FlyEff_File", cfg.FlyEff_File);
        cfg.FlyEff_StartIndex = (short)ini.ReadInteger(section, "FlyEff_StartIndex", cfg.FlyEff_StartIndex);
        cfg.FlyEff_DrawMode = ReadEnum(ini, section, "FlyEff_DrawMode", (int)cfg.FlyEff_DrawMode, 1, v => (TCustomDrawMode)v);

        cfg.Self_File = (short)ini.ReadInteger(section, "Self_File", cfg.Self_File);
        cfg.Self_StartIndex = (short)ini.ReadInteger(section, "Self_StartIndex", cfg.Self_StartIndex);
        cfg.Self_PlayCount = (ushort)ini.ReadInteger(section, "Self_PlayCount", cfg.Self_PlayCount);
        cfg.Self_EmptyCount = (ushort)ini.ReadInteger(section, "Self_EmptyCount", cfg.Self_EmptyCount);
        cfg.Self_PlayTime = (ushort)ini.ReadInteger(section, "Self_PlayTime", cfg.Self_PlayTime);
        cfg.Self_DrawOrder = ReadEnum(ini, section, "Self_DrawOrder", (int)cfg.Self_DrawOrder, 1, v => (TCustomDrawOrder)v);
        cfg.Self_DrawMode = ReadEnum(ini, section, "Self_DrawMode", (int)cfg.Self_DrawMode, 1, v => (TCustomDrawMode)v);
        cfg.Self_DirCalcType = ReadEnum(ini, section, "Self_DirCalcType", (int)cfg.Self_DirCalcType, 2, v => (TCustomDirCalcType)v);
        cfg.Self_DirCount = ReadEnum(ini, section, "Self_DirCount", (int)cfg.Self_DirCount, 1, v => (TCustomDirCount)v);

        // 旧版 Self_AttackDirType 键兼容：存在且为真 → Center + Self_AttackDir 覆写（Delphi 586-596）
        if (ini.ValueExists(section, "Self_AttackDirType"))
        {
            if (ini.ReadBool(section, "Self_AttackDirType", false))
            {
                cfg.Self_DirCalcType = TCustomDirCalcType.mdctCenter;
                int intRead = ini.ReadInteger(section, "Self_AttackDir", -1);
                if (intRead >= (int)TCustomDirCount.mdcDir8 && intRead <= (int)TCustomDirCount.mdcDir16)
                    cfg.Self_DirCount = (TCustomDirCount)intRead;
            }
        }

        cfg.Self_PlayDelayAction = (byte)(ini.ReadBool(section, "Self_PlayDelayAction", cfg.Self_PlayDelayAction != 0) ? 1 : 0);
        cfg.Self_LightRange = (byte)ini.ReadInteger(section, "Self_LightRange", cfg.Self_LightRange);

        cfg.SelfKeep_File = (short)ini.ReadInteger(section, "SelfKeep_File", cfg.SelfKeep_File);
        cfg.SelfKeep_StartIndex = (short)ini.ReadInteger(section, "SelfKeep_StartIndex", cfg.SelfKeep_StartIndex);
        cfg.SelfKeep_StartIndex2 = (short)ini.ReadInteger(section, "SelfKeep_StartIndex2", cfg.SelfKeep_StartIndex2);
        cfg.SelfKeep_PlayCount = (ushort)ini.ReadInteger(section, "SelfKeep_PlayCount", cfg.SelfKeep_PlayCount);
        cfg.SelfKeep_PlayTime = (ushort)ini.ReadInteger(section, "SelfKeep_PlayTime", cfg.SelfKeep_PlayTime);
        cfg.SelfKeep_DrawOrder = ReadEnum(ini, section, "SelfKeep_DrawOrder", (int)cfg.SelfKeep_DrawOrder, 1, v => (TCustomDrawOrder)v);
        cfg.SelfKeep_DrawMode = ReadEnum(ini, section, "SelfKeep_DrawMode", (int)cfg.SelfKeep_DrawMode, 1, v => (TCustomDrawMode)v);
        cfg.SelfKeep_DrawMode2 = ReadEnum(ini, section, "SelfKeep_DrawMode2", (int)cfg.SelfKeep_DrawMode2, 1, v => (TCustomDrawMode)v);
        cfg.SelfKeep_KeepTime = (ushort)ini.ReadInteger(section, "SelfKeep_KeepTime", cfg.SelfKeep_KeepTime);

        cfg.Explosion_File = (short)ini.ReadInteger(section, "Explosion_File", cfg.Explosion_File);
        cfg.Explosion_StartIndex = (short)ini.ReadInteger(section, "Explosion_StartIndex", cfg.Explosion_StartIndex);
        cfg.Explosion_StartIndex2 = (short)ini.ReadInteger(section, "Explosion_StartIndex2", cfg.Explosion_StartIndex2);
        cfg.Explosion_PlayCount = (ushort)ini.ReadInteger(section, "Explosion_PlayCount", cfg.Explosion_PlayCount);
        cfg.Explosion_PlayTime = (ushort)ini.ReadInteger(section, "Explosion_PlayTime", cfg.Explosion_PlayTime);
        cfg.Explosion_DrawMode = ReadEnum(ini, section, "Explosion_DrawMode", (int)cfg.Explosion_DrawMode, 1, v => (TCustomDrawMode)v);
        cfg.Explosion_DrawMode2 = ReadEnum(ini, section, "Explosion_DrawMode2", (int)cfg.Explosion_DrawMode2, 1, v => (TCustomDrawMode)v);
        cfg.Explosion_LockDraw = (byte)(ini.ReadBool(section, "Explosion_LockDraw", cfg.Explosion_LockDraw != 0) ? 1 : 0);
        cfg.Explosion_LightRange = (byte)ini.ReadInteger(section, "Explosion_LightRange", cfg.Explosion_LightRange);
        cfg.Explosion_KeepPlay = (byte)(ini.ReadBool(section, "Explosion_KeepPlay", cfg.Explosion_KeepPlay != 0) ? 1 : 0);
        cfg.Explosion_KeepTime = (ushort)ini.ReadInteger(section, "Explosion_KeepTime", cfg.Explosion_KeepTime);
        cfg.Explosion_KeepAttackRange = (byte)ini.ReadInteger(section, "Explosion_KeepAttackRange", cfg.Explosion_KeepAttackRange);
        cfg.Explosion_KeepMultiPlay = (byte)(ini.ReadBool(section, "Explosion_KeepMultiPlay", cfg.Explosion_KeepMultiPlay != 0) ? 1 : 0);
        cfg.Explosion_KeepAttackInterval = (byte)ini.ReadInteger(section, "Explosion_KeepAttackInterval", cfg.Explosion_KeepAttackInterval);
        cfg.Explosion_KeepLightRange = (byte)ini.ReadInteger(section, "Explosion_KeepLightRange", cfg.Explosion_KeepLightRange);

        cfg.Target_File = (short)ini.ReadInteger(section, "Target_File", cfg.Target_File);
        cfg.Target_StartIndex = (short)ini.ReadInteger(section, "Target_StartIndex", cfg.Target_StartIndex);
        cfg.Target_StartIndex2 = (short)ini.ReadInteger(section, "Target_StartIndex2", cfg.Target_StartIndex2);
        cfg.Target_PlayCount = (ushort)ini.ReadInteger(section, "Target_PlayCount", cfg.Target_PlayCount);
        cfg.Target_PlayTime = (ushort)ini.ReadInteger(section, "Target_PlayTime", cfg.Target_PlayTime);
        cfg.Target_DrawMode = ReadEnum(ini, section, "Target_DrawMode", (int)cfg.Target_DrawMode, 1, v => (TCustomDrawMode)v);
        cfg.Target_DrawMode2 = ReadEnum(ini, section, "Target_DrawMode2", (int)cfg.Target_DrawMode2, 1, v => (TCustomDrawMode)v);
        cfg.Target_MultiPlay = (byte)(ini.ReadBool(section, "Target_MultiPlay", cfg.Target_MultiPlay != 0) ? 1 : 0);
        cfg.Target_LockDraw = (byte)(ini.ReadBool(section, "Target_LockDraw", cfg.Target_LockDraw != 0) ? 1 : 0);
        cfg.Target_LightRange = (byte)ini.ReadInteger(section, "Target_LightRange", cfg.Target_LightRange);
        cfg.Target_KeepPlay = (byte)(ini.ReadBool(section, "Target_KeepPlay", cfg.Target_KeepPlay != 0) ? 1 : 0);
        cfg.Target_KeepTime = (ushort)ini.ReadInteger(section, "Target_KeepTime", cfg.Target_KeepTime);
        cfg.Target_KeepAttackRange = (byte)ini.ReadInteger(section, "Target_KeepAttackRange", cfg.Target_KeepAttackRange);
        cfg.Target_KeepMultiPlay = (byte)(ini.ReadBool(section, "Target_KeepMultiPlay", cfg.Target_KeepMultiPlay != 0) ? 1 : 0);
        cfg.Target_KeepAttackInterval = (byte)ini.ReadInteger(section, "Target_KeepAttackInterval", cfg.Target_KeepAttackInterval);
        cfg.Target_KeepLightRange = (byte)ini.ReadInteger(section, "Target_KeepLightRange", cfg.Target_KeepLightRange);
    }

    private static void LoadServerAttackSections(TFastIniFile ini, TMonsterServerConfig cfg, int i)
    {
        string section = "ServerAttack" + i;
        cfg.AttackEnabled = ini.ReadBool(section, "AttackEnabled", cfg.AttackEnabled);
        cfg.OperateMode = ReadEnumStatic(ini, section, "OperateMode", (int)cfg.OperateMode, 1, v => (TCustomOperateMode)v);
        cfg.AttackSelfDie = ini.ReadBool(section, "AttackSelfDie", cfg.AttackSelfDie);
        cfg.AttackDelayTime = ini.ReadInteger(section, "AttackDelayTime", cfg.AttackDelayTime);
        cfg.AttackHPPercent = ini.ReadInteger(section, "AttackHPPercent", cfg.AttackHPPercent);
        cfg.AttackRate = ini.ReadInteger(section, "AttackRate", cfg.AttackRate);
        cfg.AttackTargetCount = ini.ReadInteger(section, "AttackTargetCount", cfg.AttackTargetCount);
        cfg.AttackMode = ReadEnumStatic(ini, section, "AttackMode", (int)cfg.AttackMode, 1, v => (TCustomAttackMode)v);
        cfg.AttackTarget = ReadEnumStatic(ini, section, "AttackTarget", (int)cfg.AttackTarget, 5, v => (TCustomAttackTarget)v);
        cfg.AttackPowerCalc = ReadEnumStatic(ini, section, "AttackPowerCalc", (int)cfg.AttackPowerCalc, 3, v => (TCustomAttackPowerCalc)v);
        cfg.AttackPowerRate = ini.ReadInteger(section, "AttackPowerRate", cfg.AttackPowerRate);
        cfg.AttackTeleportAttack = ini.ReadBool(section, "AttackTeleportAttack", cfg.AttackTeleportAttack);
        cfg.AttackTeleportTargetDistance = ini.ReadInteger(section, "AttackTeleportTargetDistance", cfg.AttackTeleportTargetDistance);
        cfg.AttackTeleportDistance = ini.ReadInteger(section, "AttackTeleportDistance", cfg.AttackTeleportDistance);
        cfg.AttackTeleportRate = ini.ReadInteger(section, "AttackTeleportRate", cfg.AttackTeleportRate);
        cfg.AttackTeleportRush = ini.ReadBool(section, "AttackTeleportRush", cfg.AttackTeleportRush);
        cfg.AttackIgnoreDefence = ini.ReadBool(section, "AttackIgnoreDefence", cfg.AttackIgnoreDefence);
        cfg.AttackNearRange = ini.ReadInteger(section, "AttackNearRange", cfg.AttackNearRange);
        cfg.AttackGroupRange = ini.ReadInteger(section, "AttackGroupRange", cfg.AttackGroupRange);
        cfg.NearAttackTargetCenter = ini.ReadBool(section, "NearAttackTargetCenter", cfg.NearAttackTargetCenter);
        cfg.AttackPowerInc = ini.ReadInteger(section, "AttackPowerInc", cfg.AttackPowerInc);

        cfg.MoveTarget = ini.ReadBool(section, "MoveTarget", cfg.MoveTarget);
        cfg.MoveTargetRate = ini.ReadInteger(section, "MoveTargetRate", cfg.MoveTargetRate);
        cfg.MoveTargetHighLevel = ini.ReadBool(section, "MoveTargetHighLevel", cfg.MoveTargetHighLevel);

        section = "Additionals" + i;
        for (int j = 0; j < 12; j++)
        {
            cfg.Additionals[j].Checked = ini.ReadBool(section, "Checked" + j, cfg.Additionals[j].Checked);
            cfg.Additionals[j].Rate = (byte)ini.ReadInteger(section, "Rate" + j, cfg.Additionals[j].Rate);
            cfg.Additionals[j].Time = (ushort)ini.ReadInteger(section, "Time" + j, cfg.Additionals[j].Time);
        }
        cfg.AdditionalHP0 = ini.ReadInteger(section, "HP0", cfg.AdditionalHP0);
        cfg.AdditionalHighLevel4 = ini.ReadBool(section, "HighLevel4", cfg.AdditionalHighLevel4);
        cfg.AdditionalImprisonRange = ini.ReadInteger(section, "AdditionalImprisonRange", cfg.AdditionalImprisonRange);

        section = "CallMonster" + i;
        cfg.EnabledCallMonster = ini.ReadBool(section, "EnabledCallMonster", cfg.EnabledCallMonster);
        cfg.CallMonstersRate = ini.ReadInteger(section, "CallMonstersRate", cfg.CallMonstersRate);
        for (int j = 0; j < 4; j++)
        {
            cfg.CallMonsters[j] = ini.ReadString(section, "MonsterName" + j, cfg.CallMonsters[j]);
            cfg.CallMonsterNums[j] = ini.ReadInteger(section, "MonsterNum" + j, cfg.CallMonsterNums[j]);
        }

        section = "Protect" + i;
        cfg.ProtectAddHP = ini.ReadBool(section, "ProtectAddHP", cfg.ProtectAddHP);
        cfg.ProtectAddHPRate = ini.ReadInteger(section, "ProtectAddHPRate", cfg.ProtectAddHPRate);
        cfg.ProtectAddHPPercent = ini.ReadInteger(section, "ProtectAddHPPercent", cfg.ProtectAddHPPercent);
        cfg.ProtectAddDefence = ini.ReadBool(section, "ProtectAddDefence", cfg.ProtectAddDefence);
        cfg.ProtectAddDefenceRate = ini.ReadInteger(section, "ProtectAddDefenceRate", cfg.ProtectAddDefenceRate);
        cfg.ProtectAddDefencePercent = ini.ReadInteger(section, "ProtectAddDefencePercent", cfg.ProtectAddDefencePercent);
        cfg.ProtectAddDefenceTime = ini.ReadInteger(section, "ProtectAddDefenceTime", cfg.ProtectAddDefenceTime);
        cfg.ProtectAddMagDefence = ini.ReadBool(section, "ProtectAddMagDefence", cfg.ProtectAddMagDefence);
        cfg.ProtectAddMagDefenceRate = ini.ReadInteger(section, "ProtectAddMagDefenceRate", cfg.ProtectAddMagDefenceRate);
        cfg.ProtectAddMagDefencePercent = ini.ReadInteger(section, "ProtectAddMagDefencePercent", cfg.ProtectAddMagDefencePercent);
        cfg.ProtectAddMagDefenceTime = ini.ReadInteger(section, "ProtectAddMagDefenceTime", cfg.ProtectAddMagDefenceTime);
        cfg.ProtectAddDC = ini.ReadBool(section, "ProtectAddDC", cfg.ProtectAddDC);
        cfg.ProtectAddDCRate = ini.ReadInteger(section, "ProtectAddDCRate", cfg.ProtectAddDCRate);
        cfg.ProtectAddDCPercent = ini.ReadInteger(section, "ProtectAddDCPercent", cfg.ProtectAddDCPercent);
        cfg.ProtectAddDCTime = ini.ReadInteger(section, "ProtectAddDCTime", cfg.ProtectAddDCTime);
        cfg.ProtectAddMC = ini.ReadBool(section, "ProtectAddMC", cfg.ProtectAddMC);
        cfg.ProtectAddMCRate = ini.ReadInteger(section, "ProtectAddMCRate", cfg.ProtectAddMCRate);
        cfg.ProtectAddMCPercent = ini.ReadInteger(section, "ProtectAddMCPercent", cfg.ProtectAddMCPercent);
        cfg.ProtectAddMCTime = ini.ReadInteger(section, "ProtectAddMCTime", cfg.ProtectAddMCTime);
        cfg.ProtectAddSC = ini.ReadBool(section, "ProtectAddSC", cfg.ProtectAddSC);
        cfg.ProtectAddSCRate = ini.ReadInteger(section, "ProtectAddSCRate", cfg.ProtectAddSCRate);
        cfg.ProtectAddSCPercent = ini.ReadInteger(section, "ProtectAddSCPercent", cfg.ProtectAddSCPercent);
        cfg.ProtectAddSCTime = ini.ReadInteger(section, "ProtectAddSCTime", cfg.ProtectAddSCTime);
        cfg.ProtectTargetRange = ini.ReadInteger(section, "ProtectTargetRange", cfg.ProtectTargetRange);
        cfg.ProtectSelfRate = ini.ReadInteger(section, "ProtectSelfRate", cfg.ProtectSelfRate);
    }

    private delegate T DelphiEnumReader<T>(int v);

    private static T ReadEnum<T>(TFastIniFile ini, string section, string key, int def, int high, DelphiEnumReader<T> cast)
        => ReadEnumStatic(ini, section, key, def, high, cast);

    private static T ReadEnumStatic<T>(TFastIniFile ini, string section, string key, int def, int high, DelphiEnumReader<T> cast)
    {
        int intRead = ini.ReadInteger(section, key, def);
        if (intRead >= 0 && intRead <= high)
            return cast(intRead);
        return cast(def);
    }

    /// <summary>Delphi SaveToIniFile（先复位 IsChanged；DieNoCalcDir 用 WriteInteger 原文形态；擦除旧 Self_AttackDir 键组）。</summary>
    public void SaveToIniFile()
    {
        _isChanged = false;
        if (!Directory.Exists(M2Config.sSmartMonsterDir))
            Directory.CreateDirectory(M2Config.sSmartMonsterDir);
        string fileName = M2Config.sSmartMonsterDir + MonsterName + ".ini";
        var ini = new TFastIniFile(fileName);
        try
        {
            string section = "ClientConfig";
            ini.WriteInteger(section, "DrawMode", (int)ClientBaseConfig.DrawMode);
            ini.WriteInteger(section, "DrawMode2", (int)ClientBaseConfig.DrawMode2);
            ini.WriteInteger(section, "DrawOrder", (int)ClientBaseConfig.DrawOrder);
            ini.WriteInteger(section, "DieNoCalcDir", ClientBaseConfig.DieNoCalcDir); // 原文 WriteInteger 形态
            ini.WriteInteger(section, "HPBgOffsetX", ClientBaseConfig.HPBgOffsetX);
            ini.WriteInteger(section, "HPBgOffsetY", ClientBaseConfig.HPBgOffsetY);
            ini.WriteInteger(section, "HPOffsetX", ClientBaseConfig.HPOffsetX);
            ini.WriteInteger(section, "HPOffsetY", ClientBaseConfig.HPOffsetY);
            ini.WriteInteger(section, "HPFile", ClientBaseConfig.HPFile);
            ini.WriteInteger(section, "HPStartIndex", ClientBaseConfig.HPStartIndex);
            ini.WriteInteger(section, "HPTextOffsetX", ClientBaseConfig.HPTextOffsetX);
            ini.WriteInteger(section, "HPTextOffsetY", ClientBaseConfig.HPTextOffsetY);

            section = "ClientSounds";
            for (int i = 0; i < 11; i++)
                ini.WriteString(section, CustomMonsterConsts.MonsterSoundTypeNames[i], ClientBaseConfig.Sounds[i].Value);

            for (int i = 0; i < 12; i++)
            {
                ref readonly var action = ref ClientActions[i];
                section = CustomMonsterConsts.MonsterClientActionSections[i];
                ini.WriteInteger(section, "ActionFile", action.ActionFile);
                ini.WriteInteger(section, "StartIndex", action.StartIndex);
                ini.WriteInteger(section, "PlayCount", action.PlayCount);
                ini.WriteInteger(section, "EmptyCount", action.EmptyCount);
                ini.WriteInteger(section, "PlayTime", action.PlayTime);
                ini.WriteInteger(section, "EffectFile", action.EffectFile);
                ini.WriteInteger(section, "EffectIndex", action.EffectIndex);
                ini.WriteInteger(section, "EffectFile2", action.EffectFile2);
                ini.WriteInteger(section, "EffectIndex2", action.EffectIndex2);
                ini.WriteBool(section, "CalcDir", action.CalcDir != 0);
            }

            for (int i = 0; i < 6; i++)
            {
                ref readonly var cfg = ref ClientAttackConfigs[i];
                section = "ClientAttack" + i;
                ini.WriteInteger(section, "Fly_File", cfg.Fly_File);
                ini.WriteInteger(section, "Fly_StartIndex", cfg.Fly_StartIndex);
                ini.WriteInteger(section, "Fly_PlayCount", cfg.Fly_PlayCount);
                ini.WriteInteger(section, "Fly_EmptyCount", cfg.Fly_EmptyCount);
                ini.WriteInteger(section, "Fly_PlayTime", cfg.Fly_PlayTime);
                ini.WriteInteger(section, "Fly_DrawMode", (int)cfg.Fly_DrawMode);
                ini.WriteInteger(section, "Fly_DirCount", (int)cfg.Fly_DirCount);
                ini.WriteBool(section, "Fly_CalcDir", cfg.Fly_CalcDir != 0);
                ini.WriteInteger(section, "Fly_LightRange", cfg.Fly_LightRange);
                ini.WriteInteger(section, "FlyEff_File", cfg.FlyEff_File);
                ini.WriteInteger(section, "FlyEff_StartIndex", cfg.FlyEff_StartIndex);
                ini.WriteInteger(section, "FlyEff_DrawMode", (int)cfg.FlyEff_DrawMode);
                ini.WriteInteger(section, "Self_File", cfg.Self_File);
                ini.WriteInteger(section, "Self_StartIndex", cfg.Self_StartIndex);
                ini.WriteInteger(section, "Self_PlayCount", cfg.Self_PlayCount);
                ini.WriteInteger(section, "Self_EmptyCount", cfg.Self_EmptyCount);
                ini.WriteInteger(section, "Self_PlayTime", cfg.Self_PlayTime);
                ini.WriteInteger(section, "Self_DrawOrder", (int)cfg.Self_DrawOrder);
                ini.WriteInteger(section, "Self_DrawMode", (int)cfg.Self_DrawMode);
                ini.WriteInteger(section, "Self_DirCalcType", (int)cfg.Self_DirCalcType);
                ini.WriteInteger(section, "Self_DirCount", (int)cfg.Self_DirCount);
                ini.DeleteKey(section, "Self_AttackDir");
                ini.DeleteKey(section, "Self_AttackDirType");
                ini.WriteBool(section, "Self_PlayDelayAction", cfg.Self_PlayDelayAction != 0);
                ini.WriteInteger(section, "Self_LightRange", cfg.Self_LightRange);
                ini.WriteInteger(section, "SelfKeep_File", cfg.SelfKeep_File);
                ini.WriteInteger(section, "SelfKeep_StartIndex", cfg.SelfKeep_StartIndex);
                ini.WriteInteger(section, "SelfKeep_StartIndex2", cfg.SelfKeep_StartIndex2);
                ini.WriteInteger(section, "SelfKeep_PlayCount", cfg.SelfKeep_PlayCount);
                ini.WriteInteger(section, "SelfKeep_PlayTime", cfg.SelfKeep_PlayTime);
                ini.WriteInteger(section, "SelfKeep_DrawMode", (int)cfg.SelfKeep_DrawMode);
                ini.WriteInteger(section, "SelfKeep_KeepTime", cfg.SelfKeep_KeepTime);
                ini.WriteInteger(section, "SelfKeep_DrawOrder", (int)cfg.SelfKeep_DrawOrder);
                ini.WriteInteger(section, "SelfKeep_DrawMode2", (int)cfg.SelfKeep_DrawMode2);
                ini.WriteInteger(section, "Explosion_File", cfg.Explosion_File);
                ini.WriteInteger(section, "Explosion_StartIndex", cfg.Explosion_StartIndex);
                ini.WriteInteger(section, "Explosion_StartIndex2", cfg.Explosion_StartIndex2);
                ini.WriteInteger(section, "Explosion_PlayCount", cfg.Explosion_PlayCount);
                ini.WriteInteger(section, "Explosion_PlayTime", cfg.Explosion_PlayTime);
                ini.WriteInteger(section, "Explosion_DrawMode", (int)cfg.Explosion_DrawMode);
                ini.WriteInteger(section, "Explosion_DrawMode2", (int)cfg.Explosion_DrawMode2);
                ini.WriteBool(section, "Explosion_LockDraw", cfg.Explosion_LockDraw != 0);
                ini.WriteInteger(section, "Explosion_LightRange", cfg.Explosion_LightRange);
                ini.WriteBool(section, "Explosion_KeepPlay", cfg.Explosion_KeepPlay != 0);
                ini.WriteInteger(section, "Explosion_KeepTime", cfg.Explosion_KeepTime);
                ini.WriteInteger(section, "Explosion_KeepAttackRange", cfg.Explosion_KeepAttackRange);
                ini.WriteBool(section, "Explosion_KeepMultiPlay", cfg.Explosion_KeepMultiPlay != 0);
                ini.WriteInteger(section, "Explosion_KeepAttackInterval", cfg.Explosion_KeepAttackInterval);
                ini.WriteInteger(section, "Explosion_KeepLightRange", cfg.Explosion_KeepLightRange);
                ini.WriteInteger(section, "Target_File", cfg.Target_File);
                ini.WriteInteger(section, "Target_StartIndex", cfg.Target_StartIndex);
                ini.WriteInteger(section, "Target_StartIndex2", cfg.Target_StartIndex2);
                ini.WriteInteger(section, "Target_PlayCount", cfg.Target_PlayCount);
                ini.WriteInteger(section, "Target_PlayTime", cfg.Target_PlayTime);
                ini.WriteInteger(section, "Target_DrawMode", (int)cfg.Target_DrawMode);
                ini.WriteInteger(section, "Target_DrawMode2", (int)cfg.Target_DrawMode2);
                ini.WriteBool(section, "Target_MultiPlay", cfg.Target_MultiPlay != 0);
                ini.WriteBool(section, "Target_LockDraw", cfg.Target_LockDraw != 0);
                ini.WriteInteger(section, "Target_LightRange", cfg.Target_LightRange);
                ini.WriteBool(section, "Target_KeepPlay", cfg.Target_KeepPlay != 0);
                ini.WriteInteger(section, "Target_KeepTime", cfg.Target_KeepTime);
                ini.WriteInteger(section, "Target_KeepAttackRange", cfg.Target_KeepAttackRange);
                ini.WriteBool(section, "Target_KeepMultiPlay", cfg.Target_KeepMultiPlay != 0);
                ini.WriteInteger(section, "Target_KeepAttackInterval", cfg.Target_KeepAttackInterval);
                ini.WriteInteger(section, "Target_KeepLightRange", cfg.Target_KeepLightRange);
            }

            section = "ServerConfig";
            ini.WriteInteger(section, "ViewRange", ServerBaseConfig.ViewRange);
            ini.WriteInteger(section, "MonsterType", (int)ServerBaseConfig.MonsterType);
            ini.WriteInteger(section, "MoveOption", (int)ServerBaseConfig.MoveOption);
            ini.WriteInteger(section, "ProtectRange", ServerBaseConfig.ProtectRange);
            ini.WriteInteger(section, "MinAttackNearRange", ServerBaseConfig.MinAttackNearRange);
            ini.WriteInteger(section, "LightRange", ServerBaseConfig.LightRange);
            ini.WriteBool(section, "NoAttack", ServerBaseConfig.NoAttack);

            for (int i = 0; i < 6; i++)
            {
                var sc = MonsterServerConfigs[i];
                section = "ServerAttack" + i;
                ini.WriteBool(section, "AttackEnabled", sc.AttackEnabled);
                ini.WriteInteger(section, "OperateMode", (int)sc.OperateMode);
                ini.WriteBool(section, "AttackSelfDie", sc.AttackSelfDie);
                ini.WriteInteger(section, "AttackDelayTime", sc.AttackDelayTime);
                ini.WriteInteger(section, "AttackHPPercent", sc.AttackHPPercent);
                ini.WriteInteger(section, "AttackRate", sc.AttackRate);
                ini.WriteInteger(section, "AttackTargetCount", sc.AttackTargetCount);
                ini.WriteInteger(section, "AttackMode", (int)sc.AttackMode);
                ini.WriteInteger(section, "AttackTarget", (int)sc.AttackTarget);
                ini.WriteInteger(section, "AttackPowerCalc", (int)sc.AttackPowerCalc);
                ini.WriteInteger(section, "AttackPowerRate", sc.AttackPowerRate);
                ini.WriteBool(section, "AttackTeleportAttack", sc.AttackTeleportAttack);
                ini.WriteInteger(section, "AttackTeleportTargetDistance", sc.AttackTeleportTargetDistance);
                ini.WriteInteger(section, "AttackTeleportDistance", sc.AttackTeleportDistance);
                ini.WriteInteger(section, "AttackTeleportRate", sc.AttackTeleportRate);
                ini.WriteBool(section, "AttackTeleportRush", sc.AttackTeleportRush);
                ini.WriteBool(section, "AttackIgnoreDefence", sc.AttackIgnoreDefence);
                ini.WriteInteger(section, "AttackNearRange", sc.AttackNearRange);
                ini.WriteInteger(section, "AttackGroupRange", sc.AttackGroupRange);
                ini.WriteBool(section, "NearAttackTargetCenter", sc.NearAttackTargetCenter);
                ini.WriteInteger(section, "AttackPowerInc", sc.AttackPowerInc);
                ini.WriteBool(section, "MoveTarget", sc.MoveTarget);
                ini.WriteInteger(section, "MoveTargetRate", sc.MoveTargetRate);
                ini.WriteBool(section, "MoveTargetHighLevel", sc.MoveTargetHighLevel);

                section = "Additionals" + i;
                for (int j = 0; j < 12; j++)
                {
                    ini.WriteBool(section, "Checked" + j, sc.Additionals[j].Checked);
                    ini.WriteInteger(section, "Rate" + j, sc.Additionals[j].Rate);
                    ini.WriteInteger(section, "Time" + j, sc.Additionals[j].Time);
                }
                ini.WriteInteger(section, "HP0", sc.AdditionalHP0);
                ini.WriteBool(section, "HighLevel4", sc.AdditionalHighLevel4);
                ini.WriteInteger(section, "AdditionalImprisonRange", sc.AdditionalImprisonRange);

                section = "CallMonster" + i;
                ini.WriteBool(section, "EnabledCallMonster", sc.EnabledCallMonster);
                ini.WriteInteger(section, "CallMonstersRate", sc.CallMonstersRate);
                for (int j = 0; j < 4; j++)
                {
                    ini.WriteString(section, "MonsterName" + j, sc.CallMonsters[j]);
                    ini.WriteInteger(section, "MonsterNum" + j, sc.CallMonsterNums[j]);
                }

                section = "Protect" + i;
                ini.WriteBool(section, "ProtectAddHP", sc.ProtectAddHP);
                ini.WriteInteger(section, "ProtectAddHPRate", sc.ProtectAddHPRate);
                ini.WriteInteger(section, "ProtectAddHPPercent", sc.ProtectAddHPPercent);
                ini.WriteBool(section, "ProtectAddDefence", sc.ProtectAddDefence);
                ini.WriteInteger(section, "ProtectAddDefenceRate", sc.ProtectAddDefenceRate);
                ini.WriteInteger(section, "ProtectAddDefencePercent", sc.ProtectAddDefencePercent);
                ini.WriteInteger(section, "ProtectAddDefenceTime", sc.ProtectAddDefenceTime);
                ini.WriteBool(section, "ProtectAddMagDefence", sc.ProtectAddMagDefence);
                ini.WriteInteger(section, "ProtectAddMagDefenceRate", sc.ProtectAddMagDefenceRate);
                ini.WriteInteger(section, "ProtectAddMagDefencePercent", sc.ProtectAddMagDefencePercent);
                ini.WriteInteger(section, "ProtectAddMagDefenceTime", sc.ProtectAddMagDefenceTime);
                ini.WriteBool(section, "ProtectAddDC", sc.ProtectAddDC);
                ini.WriteInteger(section, "ProtectAddDCRate", sc.ProtectAddDCRate);
                ini.WriteInteger(section, "ProtectAddDCPercent", sc.ProtectAddDCPercent);
                ini.WriteInteger(section, "ProtectAddDCTime", sc.ProtectAddDCTime);
                ini.WriteBool(section, "ProtectAddMC", sc.ProtectAddMC);
                ini.WriteInteger(section, "ProtectAddMCRate", sc.ProtectAddMCRate);
                ini.WriteInteger(section, "ProtectAddMCPercent", sc.ProtectAddMCPercent);
                ini.WriteInteger(section, "ProtectAddMCTime", sc.ProtectAddMCTime);
                ini.WriteBool(section, "ProtectAddSC", sc.ProtectAddSC);
                ini.WriteInteger(section, "ProtectAddSCRate", sc.ProtectAddSCRate);
                ini.WriteInteger(section, "ProtectAddSCPercent", sc.ProtectAddSCPercent);
                ini.WriteInteger(section, "ProtectAddSCTime", sc.ProtectAddSCTime);
                ini.WriteInteger(section, "ProtectTargetRange", sc.ProtectTargetRange);
                ini.WriteInteger(section, "ProtectSelfRate", sc.ProtectSelfRate);
            }

            ini.UpdateFile();
        }
        finally
        {
            ini.Dispose();
        }
    }
}

/// <summary>Delphi SaveCustomMonsterClientConfigs（登录器 .dat：GUID 标志 + 数量 + CRC + 逐怪 TClientCustomMonsterConfig 定长块）。</summary>
public static class CustomMonsterClientWriter
{
    public static void Save(IList<TCustomMonsterConfig> monsterConfigs, string fileName)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms))
        {
            w.Write(CustomMonsterConsts.ClientCustomMonsterConfigFlag.ToByteArray());
            w.Write(monsterConfigs.Count);
            w.Write(0u); // CRC 占位
            foreach (var monsterConfig in monsterConfigs)
            {
                var clientConfig = new TClientCustomMonsterConfig
                {
                    wMonsterAppr = monsterConfig.MonsterAppr,
                    BaseConfig = monsterConfig.ClientBaseConfig,
                    Actions = monsterConfig.ClientActions,
                    AttackConfigs = monsterConfig.ClientAttackConfigs,
                };
                w.Write(StructureToBytes(clientConfig));
            }
        }

        byte[] bytes = ms.ToArray();
        // Delphi：CRC = BufferCRC(缓冲区 + 24, Size - 24)（跳过 Flag+Len+CRC 头），回写至偏移 20
        int headerLen = 16 + 4 + 4;
        var payload = new byte[bytes.Length - headerLen];
        Array.Copy(bytes, headerLen, payload, 0, payload.Length);
        uint crc = CheckCrc.BufferCRC(payload, payload.Length);
        BitConverter.GetBytes(crc).CopyTo(bytes, 16 + 4);
        File.WriteAllBytes(fileName, bytes);
    }

    private static byte[] StructureToBytes<T>(T structure) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        var buf = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, ptr, false);
            Marshal.Copy(ptr, buf, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return buf;
    }
}
