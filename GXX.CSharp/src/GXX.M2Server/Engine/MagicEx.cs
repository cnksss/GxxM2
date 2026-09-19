using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>Magic.pas 批次G 配置扩展（g_Config 技能/召唤字段子集）。</summary>
public static partial class M2Config
{
    // ---- 召唤系 ----
    public static string sBoneFamm = "白骨精";
    public static int nBoneFammCount = 2;
    public static bool boBonePlugSettingPriority;
    public static string sPlusBoneFammName1_3 = "白骨精";
    public static string sPlusBoneFammName4_6 = "强化白骨精";
    public static string sPlusBoneFammName7_9 = "超级白骨精";
    public static string sPlusBoneFammName9_N = "终极白骨精";
    public static readonly int[] dwPlusBoneFammLevels = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
    public static int dwPlusBoneFammAddLevelAfter9 = 10;
    public static (int nHumLevel, string sMonName, int nLevel, int nCount)[] BoneFammArray =
        new (int, string, int, int)[10];

    public static string sDogz = "神兽";
    public static int nDogzCount = 1;
    public static bool boDogzPlugSettingPriority;
    public static string sPlusDogzName1_3 = "神兽";
    public static string sPlusDogzName4_6 = "强化神兽";
    public static string sPlusDogzName7_9 = "超级神兽";
    public static string sPlusDogzName9_N = "终极神兽";
    public static readonly int[] dwPlusDogzLevels = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
    public static int dwPlusDogzAddLevelAfter9 = 10;
    public static (int nHumLevel, string sMonName, int nLevel, int nCount)[] DogzArray =
        new (int, string, int, int)[10];

    public static string sBigDogz = "圣兽";
    public static int nBigDogzCount = 1;
    public static (int nHumLevel, string sMonName, int nLevel, int nCount)[] BigDogzArray =
        new (int, string, int, int)[10];

    public static string sMoonFamm = "月灵";
    public static int nMoonCount = 1;
    public static uint dwHeroCallBBCount;
    public static bool boBBMonAutoChangeColor;
    public static bool boBBAttrPlusAddOnlyMagic = true;

    // ---- 组队系 ----
    public static int nSkill37Range = 4;
    public static int nSkill37RangeAdd = 1;
    public static int nSkillGroupLighteningPowerRate = 100;
    public static bool boSkillGroupAmyounsulRed = true;
    public static bool boSkillGroupAmyounsulGreen = true;
    public static int nSkill39Range = 4;
    public static int nSkill39FreezeTime = 3000;

    // ---- 合击/连击系 ----
    public static int nSkillJointAttackLevelRate = 20;
    public static int nSkill61PowerRate = 100;
    public static int nSkill61AttackHumPowerRate = 50;
    public static int nSkill62PowerRate = 100;
    public static int nSkill62AttackHumPowerRate = 50;
    public static bool boSkill62NotMagBubbleDefence;
    public static readonly int[] SkillContinuousPowerRates = { 100, 110, 120, 130, 140, 150, 160, 170, 180, 190 };
    public static int nContinuousAttackLevelRate = 10;

    /// <summary>测试辅助：恢复本批次全部默认值。</summary>
    public static void ResetMagicExDefaults()
    {
        sBoneFamm = "白骨精";
        nBoneFammCount = 2;
        boBonePlugSettingPriority = false;
        BoneFammArray = new (int, string, int, int)[10];
        sDogz = "神兽";
        nDogzCount = 1;
        boDogzPlugSettingPriority = false;
        sPlusDogzName1_3 = "神兽";
        sPlusDogzName4_6 = "强化神兽";
        sPlusDogzName7_9 = "超级神兽";
        sPlusDogzName9_N = "终极神兽";
        DogzArray = new (int, string, int, int)[10];
        sBigDogz = "圣兽";
        nBigDogzCount = 1;
        BigDogzArray = new (int, string, int, int)[10];
        sMoonFamm = "月灵";
        nMoonCount = 1;
        boSkillGroupAmyounsulRed = true;
        boSkillGroupAmyounsulGreen = true;
    }
}

/// <summary>TCreature 召唤/合击扩展（ObjBase.MakeSlave + 主从关系）。</summary>
public partial class TCreature
{
    public TCreature? m_Master;
    public byte m_btJob;
    public readonly List<TCreature> m_SlaveList = new();
    public int PoisonGreenTick;   // 绿毒剩余 tick（POISON_DECHEALTH 简化模型）
    public int PoisonRedTick;     // 红毒剩余 tick（POISON_DAMAGEARMOR）
    public bool m_boFreeze;       // 彻地钉冰冻

    public int SlaveCount
    {
        get { lock (m_SlaveList) return m_SlaveList.Count; }
    }

    /// <summary>MakeSlave：召唤宝宝（ObjBase.MakeSlave 核心）—— 创建怪物、登记主从、放入地图。</summary>
    public TCreature? MakeSlave(string monName, int makeLevel, int expLevel, int count, uint royaltySec, byte bbType, int newLevel, bool addOnlyMagic)
    {
        if (m_PEnvir == null || monName == "") return null;
        int created = 0;
        for (int i = 0; i < count; i++)
        {
            // 在主人周围找空位
            int sx = m_nCurrX + i % 3 - 1;
            int sy = m_nCurrY + i / 3 % 3 - 1;
            if (!m_PEnvir.CanWalk(sx, sy))
                sx = m_nCurrX;
            if (!m_PEnvir.CanWalk(sx, sy))
                break;

            var slave = new TMonster
            {
                m_sCharName = monName,
                m_sMapName = m_sMapName,
                m_PEnvir = m_PEnvir,
                m_nCurrX = sx,
                m_nCurrY = sy,
                m_Master = this,
                SlaveTag = (byte)bbType
            };
            slave.m_wAbil.Level = (uint)Math.Max(expLevel, 1);
            m_PEnvir.AddToMap(sx, sy, slave);
            lock (m_SlaveList) m_SlaveList.Add(slave);
            created++;
        }
        return created > 0 ? m_SlaveList[^1] : null;
    }

    public byte SlaveTag;
}
