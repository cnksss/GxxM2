using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>uCustomHeroMagic.pas TMagicType 1:1。</summary>
public enum THeroMagicType
{
    mtWarrAttack,
    mtWizardAttack,
    mtTaosAttack,
}

/// <summary>uCustomHeroMagic.pas TMagicAttackTarget 1:1。</summary>
public enum THeroMagicAttackTarget
{
    matEnemy,
    matSelf,
    matMaster,
    matPartner,
}

/// <summary>uCustomHeroMagic.pas TCompareSymbol 1:1。</summary>
public enum TCompareSymbol
{
    csLess,
    csLessOrEqual,
    csEqual,
    csGreater,
    csGreaterorEqual,
}

/// <summary>uCustomHeroMagic.pas THeroLevelCompareType 1:1。</summary>
public enum THeroLevelCompareType
{
    hlctTargetLevel,
    hlctLevelNumber,
}

/// <summary>uCustomHeroMagic.pas THeroHPCompareType 1:1。</summary>
public enum THeroHPCompareType
{
    hhpctNumber,
    hhpctPercentage,
}

/// <summary>THeroLevelCheck record 1:1。</summary>
public struct THeroLevelCheck
{
    public bool boChecked;
    public TCompareSymbol CompareSymbol;
    public THeroLevelCompareType CompareType;
    public uint CompareValue;
}

/// <summary>THeroHPCheck record 1:1（HP/MP/目标 HP/目标 MP 共用）。</summary>
public struct THeroHPCheck
{
    public bool boChecked;
    public TCompareSymbol CompareSymbol;
    public THeroHPCompareType CompareType;
    public uint CompareValue;
}

/// <summary>TTargetStatusCheck record 1:1（7 个状态 + 7 个防状态）。</summary>
public struct TTargetStatusCheck
{
    public bool boPoisonDamageArmor;
    public bool boPoisonDecHealth;
    public bool boPoisoning;
    public bool boPoisonStone;
    public bool boFrozen;
    public bool boForeverFrozen;
    public bool boCobwebWinding;

    public bool boUnPoisonDamageArmor;
    public bool boUnPoisonDecHealth;
    public bool boUnPoisoning;
    public bool boUnPoisonStone;
    public bool boUnFrozen;
    public bool boUnForeverFrozen;
    public bool boUnCobwebWinding;
}

/// <summary>TActorCountCheck record 1:1（友方/敌方数量检查）。</summary>
public struct TActorCountCheck
{
    public bool boChecked;
    public int nCheckRange;
    public int nCheckValue;
}

/// <summary>THeroMagicUseCondition record 1:1。</summary>
public struct THeroMagicUseCondition
{
    public THeroLevelCheck HeroLevelCheck;
    public THeroHPCheck HeroHPCheck;
    public THeroHPCheck HeroMPCheck;
    public THeroHPCheck TargetHPCheck;
    public THeroHPCheck TargetMPCheck;
    public TTargetStatusCheck TargetStatusCheck;
    public TActorCountCheck FriendCountCheck;
    public TActorCountCheck EnemyCountCheck;
    public bool boStraightLineCheck;
}

/// <summary>THeroMagic record 1:1（英雄自动施法技能条目）。</summary>
public sealed class THeroMagic
{
    public bool Checked;
    public bool IsChanged;
    public THeroMagicType MagicType;
    public int MagicID;
    public bool IsCustomMagic;
    public int UseRate;
    public int AttackRange;
    public THeroMagicAttackTarget AttackTarget;
    public THeroMagicUseCondition Condition;
}

/// <summary>
/// uCustomHeroMagic.pas TCustomHeroMagicMgr 1:1（批次J60）：英雄技能自动施法表。
/// LoadFromFile：CustomHeroMagic.ini（Setup/MagicCount + HeroMagic&lt;n&gt; 节），
/// 仅收录 FindHeroMagic 命中的技能（自定义技能走无 mtype 重载），随后追加三职业默认表
/// （战士 18 / 法师 27 / 道士 25 条，AttackRange/AttackTarget 按原文逐条 1:1）。
/// SaveToFile：写 MagicCount + 逐节；仅 IsCustomMagic 条目写条件段
/// （保留原文 TargetHPCheckType 键名重复写 CompareValue 的瑕疵）。
/// </summary>
public sealed class TCustomHeroMagicMgr
{
    private readonly List<THeroMagic> _list = new();

    public bool FIsChanged;


    /// <summary>最近一次 StraightLineCheck 原始值（诊断用）。</summary>



    public int Count => _list.Count;

    /// <summary>Items[index]：越界返回 null（Delphi GetItems 1:1）。</summary>
    public THeroMagic? this[int index]
        => index >= 0 && index < _list.Count ? _list[index] : null;

    /// <summary>UserEngine.FindHeroMagic(MagicID, mtHero) / (MagicID) 接缝（null = 未找到）。</summary>
    public Func<int, bool>? FindHeroMagicHandler;

    /// <summary>UserEngine.FindHeroMagic(MagicID)（自定义技能，无 mtype）接缝。</summary>
    public Func<int, bool>? FindHeroMagicAnyHandler;

    /// <summary>CheckIsCustomMagic(MagicID) 接缝（默认按 MagicID &gt;= 1000 判定）。</summary>
    public Func<int, bool>? CheckIsCustomMagicHandler;

    public void Clear() => _list.Clear();

    /// <summary>Add（146-151）：New + FillChar 零初始化后入表。</summary>
    public THeroMagic Add()
    {
        var heroMagic = new THeroMagic();
        _list.Add(heroMagic);
        return heroMagic;
    }

    /// <summary>Remove（153-167）：按引用匹配删除。</summary>
    public bool Remove(THeroMagic item)
    {
        int index = _list.IndexOf(item);
        if (index < 0)
            return false;
        _list.RemoveAt(index);
        return true;
    }

    /// <summary>FindMagic（186-202）：类型 + 技能 ID 双匹配，首个命中。</summary>
    public THeroMagic? FindMagic(THeroMagicType magicType, int magicId)
    {
        foreach (var heroMagic in _list)
        {
            if (heroMagic.MagicType == magicType && heroMagic.MagicID == magicId)
                return heroMagic;
        }
        return null;
    }

    /// <summary>
    /// AddDefMagic（204-235）1:1：未存在且 FindHeroMagic 命中时新建（Checked=True）；
    /// 已存在时清 IsChanged + IsCustomMagic，并覆盖 AttackRange/AttackTarget（UseRate 保留）。
    /// </summary>
    public THeroMagic? AddDefMagic(THeroMagicType magicType, int magicId, int useRate,
        int attackRange = 1, THeroMagicAttackTarget attackTarget = THeroMagicAttackTarget.matEnemy)
    {
        var heroMagic = FindMagic(magicType, magicId);
        if (heroMagic == null)
        {
            if (FindHeroMagicHandler?.Invoke(magicId) ?? false)
            {
                heroMagic = Add();
                heroMagic.Checked = true;
                heroMagic.IsChanged = false;
                heroMagic.MagicType = magicType;
                heroMagic.MagicID = magicId;
                heroMagic.IsCustomMagic = false;
                heroMagic.UseRate = useRate;
                heroMagic.AttackRange = attackRange;
                heroMagic.AttackTarget = attackTarget;
            }
        }
        else
        {
            heroMagic.IsChanged = false;
            heroMagic.IsCustomMagic = false;
            // 原文连写两次 IsCustomMagic := False（1:1 保留冗余）
            heroMagic.IsCustomMagic = false;
            heroMagic.AttackRange = attackRange;
            heroMagic.AttackTarget = attackTarget;
        }
        return heroMagic;
    }

    private bool IsCustomMagic(int magicId)
        => CheckIsCustomMagicHandler?.Invoke(magicId) ?? magicId >= Grobal2Const.CUSTOM_MAGIC_START_ID;

    public void LoadFromFile()
    {
        FIsChanged = false;
        _list.Clear();

        string fileName = M2Config.sEnvirDir + "CustomHeroMagic.ini";
        if (File.Exists(fileName))
        {
            var ini = TGroupItems.ReadIniAll(fileName);
            int heroMagicCount = TGroupItems.ReadIniInt(ini, "Setup", "MagicCount", 0);
            if (heroMagicCount > 0)
            {
                for (int i = 0; i < heroMagicCount; i++)
                {
                    string sectionName = "HeroMagic" + (i + 1).ToString(CultureInfo.InvariantCulture);
                    int magicId = TGroupItems.ReadIniInt(ini, sectionName, "MagicID", 0);
                    if (magicId <= 0)
                        continue;

                    bool isCustom = IsCustomMagic(magicId);
                    bool isFound = isCustom
                        ? (FindHeroMagicAnyHandler?.Invoke(magicId) ?? false)
                        : (FindHeroMagicHandler?.Invoke(magicId) ?? false);
                    if (!isFound)
                        continue;

                    var heroMagic = Add();
                    heroMagic.MagicID = magicId;
                    heroMagic.IsCustomMagic = isCustom;
                    heroMagic.Checked = ReadBool(ini, sectionName, "Checked", false);
                    heroMagic.MagicType = (THeroMagicType)TGroupItems.ReadIniInt(ini, sectionName, "MagicType", 0);
                    heroMagic.UseRate = TGroupItems.ReadIniInt(ini, sectionName, "UseRate", 0);
                    heroMagic.IsChanged = false;

                    if (isCustom)
                        ReadConditions(ini, sectionName, heroMagic);
                }
            }
        }

        LoadDefaultMagics();
    }

    /// <summary>LoadFromFile 尾部三职业默认表（350-423 逐条 1:1）。</summary>
    public void LoadDefaultMagics()
    {
        // ---- 战士技能 ----
        AddDefMagic(THeroMagicType.mtWarrAttack, 208, 0, 10);                    // 旋风转
        AddDefMagic(THeroMagicType.mtWarrAttack, 204, 0, 10);                    // 十步一杀
        AddDefMagic(THeroMagicType.mtWarrAttack, 56, 0, 4);                      // 逐日剑法
        AddDefMagic(THeroMagicType.mtWarrAttack, 66, 0, 2);                      // 开天斩
        AddDefMagic(THeroMagicType.mtWarrAttack, 113, 0, 4);                     // 断空斩
        AddDefMagic(THeroMagicType.mtWarrAttack, 115, 0, 4);                     // 血魄一击
        AddDefMagic(THeroMagicType.mtWarrAttack, 12, 0, 2);                      // 刺杀剑术
        AddDefMagic(THeroMagicType.mtWarrAttack, 27, 4, 2);                      // 野蛮冲撞
        AddDefMagic(THeroMagicType.mtWarrAttack, 7, 10);                         // 攻杀剑术
        AddDefMagic(THeroMagicType.mtWarrAttack, 114, 0);                        // 倚天辟地
        AddDefMagic(THeroMagicType.mtWarrAttack, 26, 0);                         // 烈火剑法
        AddDefMagic(THeroMagicType.mtWarrAttack, 39, 3);                         // 彻地钉
        AddDefMagic(THeroMagicType.mtWarrAttack, 41, 10);                        // 狮子吼
        AddDefMagic(THeroMagicType.mtWarrAttack, 75, 3, 0, THeroMagicAttackTarget.matSelf);   // 护体神盾
        AddDefMagic(THeroMagicType.mtWarrAttack, 25, 0);                         // 半月弯刀
        AddDefMagic(THeroMagicType.mtWarrAttack, 40, 2);                         // 双龙斩
        AddDefMagic(THeroMagicType.mtWarrAttack, 42, 2);                         // 龙影剑法
        AddDefMagic(THeroMagicType.mtWarrAttack, 43, 3);                         // 雷霆剑法

        // ---- 法师攻击技能 ----
        AddDefMagic(THeroMagicType.mtWizardAttack, 74, 0, 0, THeroMagicAttackTarget.matSelf);   // 分身术
        AddDefMagic(THeroMagicType.mtWizardAttack, 75, 3, 0, THeroMagicAttackTarget.matSelf);   // 护体神盾
        AddDefMagic(THeroMagicType.mtWizardAttack, 114, 0);                      // 倚天辟地
        AddDefMagic(THeroMagicType.mtWizardAttack, 116, 0, 0);                   // 血魄一击
        AddDefMagic(THeroMagicType.mtWizardAttack, 31, 0, 0, THeroMagicAttackTarget.matSelf);   // 魔法盾
        AddDefMagic(THeroMagicType.mtWizardAttack, 8, 2);                        // 抗拒火环
        AddDefMagic(THeroMagicType.mtWizardAttack, 47, 10);                      // 火龙烈焰
        AddDefMagic(THeroMagicType.mtWizardAttack, 33, 8);                       // 冰咆哮
        AddDefMagic(THeroMagicType.mtWizardAttack, 205, 10);                     // 冰霜雪雨
        AddDefMagic(THeroMagicType.mtWizardAttack, 206, 10);                     // 冰霜群雨
        AddDefMagic(THeroMagicType.mtWizardAttack, 209, 10);                     // 五雷轰
        AddDefMagic(THeroMagicType.mtWizardAttack, 45, 3);                       // 灭天火
        AddDefMagic(THeroMagicType.mtWizardAttack, 11, 3);                       // 雷电术
        AddDefMagic(THeroMagicType.mtWizardAttack, 37, 0);                       // 群雷术
        AddDefMagic(THeroMagicType.mtWizardAttack, 22, 8);                       // 火墙
        AddDefMagic(THeroMagicType.mtWizardAttack, 24, 3);                       // 地狱雷光
        AddDefMagic(THeroMagicType.mtWizardAttack, 23, 3);                       // 爆裂火焰
        AddDefMagic(THeroMagicType.mtWizardAttack, 58, 2);                       // 流星火雨
        AddDefMagic(THeroMagicType.mtWizardAttack, 67, 3);                       // 先天元力
        AddDefMagic(THeroMagicType.mtWizardAttack, 68, 3);                       // 酒气护体
        AddDefMagic(THeroMagicType.mtWizardAttack, 9, 3);                        // 地狱火
        AddDefMagic(THeroMagicType.mtWizardAttack, 20, 7);                       // 诱惑之光
        AddDefMagic(THeroMagicType.mtWizardAttack, 10, 3);                       // 疾光电影
        AddDefMagic(THeroMagicType.mtWizardAttack, 44, 3);                       // 寒冰掌
        AddDefMagic(THeroMagicType.mtWizardAttack, 32, 3);                       // 圣言术
        AddDefMagic(THeroMagicType.mtWizardAttack, 1, 0);                        // 火球术
        AddDefMagic(THeroMagicType.mtWizardAttack, 5, 0);                        // 大火球

        // ---- 道士攻击技能 ----
        AddDefMagic(THeroMagicType.mtTaosAttack, 114, 0);                        // 倚天辟地
        AddDefMagic(THeroMagicType.mtTaosAttack, 117, 0, 0);                     // 血魄一击
        AddDefMagic(THeroMagicType.mtTaosAttack, 50, 0, 0, THeroMagicAttackTarget.matSelf);     // 无极真气
        AddDefMagic(THeroMagicType.mtTaosAttack, 51, 6);                         // 群体施毒术
        AddDefMagic(THeroMagicType.mtTaosAttack, 6, 0);                          // 施毒术
        AddDefMagic(THeroMagicType.mtTaosAttack, 57, 0);                         // 噬血术
        AddDefMagic(THeroMagicType.mtTaosAttack, 13, 5);                         // 灵魂火符
        AddDefMagic(THeroMagicType.mtTaosAttack, 52, 5);                         // 飓风破
        AddDefMagic(THeroMagicType.mtTaosAttack, 202, 5);                        // 裂神符
        AddDefMagic(THeroMagicType.mtTaosAttack, 203, 6);                        // 死亡之眼
        AddDefMagic(THeroMagicType.mtTaosAttack, 210, 6);                        // 幽冥火符
        AddDefMagic(THeroMagicType.mtTaosAttack, 48, 6);                         // 气功波
        AddDefMagic(THeroMagicType.mtTaosAttack, 18, 10, 0, THeroMagicAttackTarget.matSelf);    // 隐身术
        AddDefMagic(THeroMagicType.mtTaosAttack, 19, 10, 0, THeroMagicAttackTarget.matPartner); // 集体隐身术
        AddDefMagic(THeroMagicType.mtTaosAttack, 55, 0, 0, THeroMagicAttackTarget.matSelf);     // 召唤月灵
        AddDefMagic(THeroMagicType.mtTaosAttack, 30, 0, 0, THeroMagicAttackTarget.matSelf);     // 召唤神兽
        AddDefMagic(THeroMagicType.mtTaosAttack, 17, 0, 0, THeroMagicAttackTarget.matSelf);     // 召唤骷髅
        AddDefMagic(THeroMagicType.mtTaosAttack, 15, 0, 0, THeroMagicAttackTarget.matMaster);   // 神圣战甲术 防
        AddDefMagic(THeroMagicType.mtTaosAttack, 14, 0, 0, THeroMagicAttackTarget.matMaster);   // 幽灵盾 魔
        AddDefMagic(THeroMagicType.mtTaosAttack, 34, 0, 0, THeroMagicAttackTarget.matPartner);  // 解毒术
        AddDefMagic(THeroMagicType.mtTaosAttack, 49, 10, 0, THeroMagicAttackTarget.matPartner); // 净化术
        AddDefMagic(THeroMagicType.mtTaosAttack, 2, 8, 0, THeroMagicAttackTarget.matPartner);   // 治愈术
        AddDefMagic(THeroMagicType.mtTaosAttack, 29, 8, 0, THeroMagicAttackTarget.matPartner);  // 群体治愈术
        AddDefMagic(THeroMagicType.mtTaosAttack, 75, 3, 0, THeroMagicAttackTarget.matSelf);     // 护体神盾
    }

    public void SaveToFile()
    {
        FIsChanged = false;

        if (!string.IsNullOrEmpty(M2Config.sEnvirDir) && !Directory.Exists(M2Config.sEnvirDir))
            Directory.CreateDirectory(M2Config.sEnvirDir);

        string fileName = M2Config.sEnvirDir + "CustomHeroMagic.ini";
        var sb = new StringBuilder();
        sb.Append("[Setup]").Append("\r\n")
          .Append("MagicCount=").Append(_list.Count.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

        for (int i = 0; i < _list.Count; i++)
        {
            var heroMagic = _list[i];
            string sectionName = "HeroMagic" + (i + 1).ToString(CultureInfo.InvariantCulture);

            sb.Append('[').Append(sectionName).Append(']').Append("\r\n");
            sb.Append("Checked=").Append(heroMagic.Checked ? 1 : 0).Append("\r\n");
            sb.Append("MagicID=").Append(heroMagic.MagicID.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("MagicType=").Append(((int)heroMagic.MagicType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("UseRate=").Append(heroMagic.UseRate.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            heroMagic.IsChanged = false;
            if (!heroMagic.IsCustomMagic)
                continue;

            sb.Append("AttackRange=").Append(heroMagic.AttackRange.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("AttackTarget=").Append(((int)heroMagic.AttackTarget).ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            var cond = heroMagic.Condition;
            sb.Append("LevelChecked=").Append(cond.HeroLevelCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("LevelCheckSymbol=").Append(((int)cond.HeroLevelCheck.CompareSymbol).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("LevelCheckType=").Append(((int)cond.HeroLevelCheck.CompareType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("LevelCheckValue=").Append(cond.HeroLevelCheck.CompareValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("HeroHPChecked=").Append(cond.HeroHPCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("HeroHPCheckSymbol=").Append(((int)cond.HeroHPCheck.CompareSymbol).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("HeroHPCheckType=").Append(((int)cond.HeroHPCheck.CompareType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("HeroHPCheckValue=").Append(cond.HeroHPCheck.CompareValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("HeroMPChecked=").Append(cond.HeroMPCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("HeroMPCheckSymbol=").Append(((int)cond.HeroMPCheck.CompareSymbol).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("HeroMPCheckType=").Append(((int)cond.HeroMPCheck.CompareType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("HeroMPCheckValue=").Append(cond.HeroMPCheck.CompareValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("TargetHPChecked=").Append(cond.TargetHPCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("TargetHPCheckSymbol=").Append(((int)cond.TargetHPCheck.CompareSymbol).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("TargetHPCheckType=").Append(((int)cond.TargetHPCheck.CompareType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            // 原文瑕疵保留：第二行键名重复（TargetHPCheckType 写 CompareValue）
            sb.Append("TargetHPCheckType=").Append(cond.TargetHPCheck.CompareValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("TargetMPChecked=").Append(cond.TargetMPCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("TargetMPCheckSymbol=").Append(((int)cond.TargetMPCheck.CompareSymbol).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("TargetMPCheckType=").Append(((int)cond.TargetMPCheck.CompareType).ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("TargetMPCheckValue=").Append(cond.TargetMPCheck.CompareValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("PoisonDamageArmor=").Append(cond.TargetStatusCheck.boPoisonDamageArmor ? 1 : 0).Append("\r\n");
            sb.Append("PoisonDecHealth=").Append(cond.TargetStatusCheck.boPoisonDecHealth ? 1 : 0).Append("\r\n");
            sb.Append("Poisoning=").Append(cond.TargetStatusCheck.boPoisoning ? 1 : 0).Append("\r\n");
            sb.Append("PoisonStone=").Append(cond.TargetStatusCheck.boPoisonStone ? 1 : 0).Append("\r\n");
            sb.Append("Frozen=").Append(cond.TargetStatusCheck.boFrozen ? 1 : 0).Append("\r\n");
            sb.Append("ForeverFrozen=").Append(cond.TargetStatusCheck.boForeverFrozen ? 1 : 0).Append("\r\n");
            sb.Append("CobwebWinding=").Append(cond.TargetStatusCheck.boCobwebWinding ? 1 : 0).Append("\r\n");

            sb.Append("UnPoisonDamageArmor=").Append(cond.TargetStatusCheck.boUnPoisonDamageArmor ? 1 : 0).Append("\r\n");
            sb.Append("UnPoisonDecHealth=").Append(cond.TargetStatusCheck.boUnPoisonDecHealth ? 1 : 0).Append("\r\n");
            sb.Append("UnPoisoning=").Append(cond.TargetStatusCheck.boUnPoisoning ? 1 : 0).Append("\r\n");
            sb.Append("UnPoisonStone=").Append(cond.TargetStatusCheck.boUnPoisonStone ? 1 : 0).Append("\r\n");
            sb.Append("UnFrozen=").Append(cond.TargetStatusCheck.boUnFrozen ? 1 : 0).Append("\r\n");
            sb.Append("UnForeverFrozen=").Append(cond.TargetStatusCheck.boUnForeverFrozen ? 1 : 0).Append("\r\n");
            sb.Append("UnCobwebWinding=").Append(cond.TargetStatusCheck.boUnCobwebWinding ? 1 : 0).Append("\r\n");

            sb.Append("FriendCountChecked=").Append(cond.FriendCountCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("FriendCountCheckRange=").Append(cond.FriendCountCheck.nCheckRange.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("FriendCountCheckValue=").Append(cond.FriendCountCheck.nCheckValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("EnemyCountChecked=").Append(cond.EnemyCountCheck.boChecked ? 1 : 0).Append("\r\n");
            sb.Append("EnemyCountCheckRange=").Append(cond.EnemyCountCheck.nCheckRange.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("EnemyCountCheckValue=").Append(cond.EnemyCountCheck.nCheckValue.ToString(CultureInfo.InvariantCulture)).Append("\r\n");

            sb.Append("StraightLineChecked=").Append(cond.boStraightLineCheck ? 1 : 0).Append("\r\n");
        }

        try
        {
            // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
            File.WriteAllText(fileName, sb.ToString(), GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // 忽略
        }
    }

    private static void ReadConditions(Dictionary<string, Dictionary<string, string>> ini, string section, THeroMagic heroMagic)
    {
        var cond = heroMagic.Condition;

        heroMagic.AttackRange = TGroupItems.ReadIniInt(ini, section, "AttackRange", 0);
        heroMagic.AttackTarget = (THeroMagicAttackTarget)TGroupItems.ReadIniInt(ini, section, "AttackTarget", 0);

        cond.HeroLevelCheck.boChecked = ReadBool(ini, section, "LevelChecked", false);
        cond.HeroLevelCheck.CompareSymbol = (TCompareSymbol)TGroupItems.ReadIniInt(ini, section, "LevelCheckSymbol", 0);
        cond.HeroLevelCheck.CompareType = (THeroLevelCompareType)TGroupItems.ReadIniInt(ini, section, "LevelCheckType", 0);
        cond.HeroLevelCheck.CompareValue = (uint)TGroupItems.ReadIniInt(ini, section, "LevelCheckValue", 0);

        cond.HeroHPCheck.boChecked = ReadBool(ini, section, "HeroHPChecked", false);
        cond.HeroHPCheck.CompareSymbol = (TCompareSymbol)TGroupItems.ReadIniInt(ini, section, "HeroHPCheckSymbol", 0);
        cond.HeroHPCheck.CompareType = (THeroHPCompareType)TGroupItems.ReadIniInt(ini, section, "HeroHPCheckType", 0);
        cond.HeroHPCheck.CompareValue = (uint)TGroupItems.ReadIniInt(ini, section, "HeroHPCheckValue", 0);

        cond.HeroMPCheck.boChecked = ReadBool(ini, section, "HeroMPChecked", false);
        cond.HeroMPCheck.CompareSymbol = (TCompareSymbol)TGroupItems.ReadIniInt(ini, section, "HeroMPCheckSymbol", 0);
        cond.HeroMPCheck.CompareType = (THeroHPCompareType)TGroupItems.ReadIniInt(ini, section, "HeroMPCheckType", 0);
        cond.HeroMPCheck.CompareValue = (uint)TGroupItems.ReadIniInt(ini, section, "HeroMPCheckValue", 0);

        cond.TargetHPCheck.boChecked = ReadBool(ini, section, "TargetHPChecked", false);
        cond.TargetHPCheck.CompareSymbol = (TCompareSymbol)TGroupItems.ReadIniInt(ini, section, "TargetHPCheckSymbol", 0);
        cond.TargetHPCheck.CompareType = (THeroHPCompareType)TGroupItems.ReadIniInt(ini, section, "TargetHPCheckType", 0);
        cond.TargetHPCheck.CompareValue = (uint)TGroupItems.ReadIniInt(ini, section, "TargetHPCheckValue", 0);

        cond.TargetMPCheck.boChecked = ReadBool(ini, section, "TargetMPChecked", false);
        cond.TargetMPCheck.CompareSymbol = (TCompareSymbol)TGroupItems.ReadIniInt(ini, section, "TargetMPCheckSymbol", 0);
        cond.TargetMPCheck.CompareType = (THeroHPCompareType)TGroupItems.ReadIniInt(ini, section, "TargetMPCheckType", 0);
        cond.TargetMPCheck.CompareValue = (uint)TGroupItems.ReadIniInt(ini, section, "TargetMPCheckValue", 0);

        cond.TargetStatusCheck.boPoisonDamageArmor = ReadBool(ini, section, "PoisonDamageArmor", false);
        cond.TargetStatusCheck.boPoisonDecHealth = ReadBool(ini, section, "PoisonDecHealth", false);
        cond.TargetStatusCheck.boPoisoning = ReadBool(ini, section, "Poisoning", false);
        cond.TargetStatusCheck.boPoisonStone = ReadBool(ini, section, "PoisonStone", false);
        cond.TargetStatusCheck.boFrozen = ReadBool(ini, section, "Frozen", false);
        cond.TargetStatusCheck.boForeverFrozen = ReadBool(ini, section, "ForeverFrozen", false);
        cond.TargetStatusCheck.boCobwebWinding = ReadBool(ini, section, "CobwebWinding", false);

        cond.TargetStatusCheck.boUnPoisonDamageArmor = ReadBool(ini, section, "UnPoisonDamageArmor", false);
        cond.TargetStatusCheck.boUnPoisonDecHealth = ReadBool(ini, section, "UnPoisonDecHealth", false);
        cond.TargetStatusCheck.boUnPoisoning = ReadBool(ini, section, "UnPoisoning", false);
        cond.TargetStatusCheck.boUnPoisonStone = ReadBool(ini, section, "UnPoisonStone", false);
        cond.TargetStatusCheck.boUnFrozen = ReadBool(ini, section, "UnFrozen", false);
        cond.TargetStatusCheck.boUnForeverFrozen = ReadBool(ini, section, "UnForeverFrozen", false);
        cond.TargetStatusCheck.boUnCobwebWinding = ReadBool(ini, section, "UnCobwebWinding", false);

        cond.FriendCountCheck.boChecked = ReadBool(ini, section, "FriendCountChecked", false);
        cond.FriendCountCheck.nCheckRange = TGroupItems.ReadIniInt(ini, section, "FriendCountCheckRange", 0);
        cond.FriendCountCheck.nCheckValue = TGroupItems.ReadIniInt(ini, section, "FriendCountCheckValue", 0);

        cond.EnemyCountCheck.boChecked = ReadBool(ini, section, "EnemyCountChecked", false);
        cond.EnemyCountCheck.nCheckRange = TGroupItems.ReadIniInt(ini, section, "EnemyCountCheckRange", 0);
        cond.EnemyCountCheck.nCheckValue = TGroupItems.ReadIniInt(ini, section, "EnemyCountCheckValue", 0);

        // INI 键名为 StraightLineChecked（原文字段 boStraightLineCheck，键名带 Checked 后缀）
        cond.boStraightLineCheck = ReadBool(ini, section, "StraightLineChecked", false);

        heroMagic.Condition = cond;
    }

    /// <summary>ReadIni bool（Delphi TIniFile ReadBoolean：'1'/'-1'/True → true）。</summary>
    private static bool ReadBool(Dictionary<string, Dictionary<string, string>> ini, string section, string key, bool def)
    {
        string raw = TGroupItems.ReadIniString(ini, section, key, def ? "1" : "0");
        raw = raw.Trim();
        if (raw.Length == 0)
            return def;
        if (raw == "0" || raw.Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;
        if (raw == "-1" || raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;
        return def;
    }

    public IEnumerable<THeroMagic> All()
    {
        foreach (var m in _list) yield return m;
    }
}

/// <summary>uCustomHeroMagic.pas 单元级（批次J60）：g_CustomHeroMagicMgr。</summary>
public static class CustomHeroMagicState
{
    /// <summary>g_CustomHeroMagicMgr。</summary>
    public static readonly TCustomHeroMagicMgr CustomHeroMagicMgr = new();

    /// <summary>TMagicAttackTargetNames（18）。</summary>
    public static readonly string[] MagicAttackTargetNames = { "敌人", "自己", "主人", "伙伴" };

    /// <summary>TCompareSymbolNames（20）。</summary>
    public static readonly string[] CompareSymbolNames = { "<", "<=", "=", ">", ">=" };

    /// <summary>THeroLevelCompareTypeNames（21）。</summary>
    public static readonly string[] HeroLevelCompareTypeNames = { "目标等级", "固定等级" };

    /// <summary>THeroHPCompareTypeNames（22）。</summary>
    public static readonly string[] HeroHPCompareTypeNames = { "固定值", "百分比" };

    /// <summary>测试隔离。</summary>
    public static void ResetForTests() => CustomHeroMagicMgr.Clear();
}
