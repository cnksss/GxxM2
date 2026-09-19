using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// ObjBase.pas TBaseObject.RecalcAbilitys 组件移植（批次J8）：
/// ① AddGroupItemValue（套装组字段值聚合，GroupFldValues 0..13 → 属性叠加 + 上限钳制，1:1）；
/// ② AddAbilitysByCode 特戒代码开关核心（Shape/Anicount case 111..153：隐身/传送/麻痹/复活/火焰/恢复/护身/肌肉/技巧/探测/防麻/超人/经验/力量/防护身/防复活/行会传送/防毒/防诱惑/防火墙 及 150-153 组合）。
/// RecalcAbilitys 主循环（m_UseItems 遍历 + GetAccessory 千行 StdMode 分支）为下一批次。
/// </summary>
public static class RecalcAbilitys
{
    /// <summary>AddGroupItemValue 1:1（ObjBase.pas 18178 行内嵌过程）。
    /// GroupFldValues: 0=MaxHP 1=MaxMP 2=AC1 3=MAC1 4=DC1 5=MC1 6=SC1 7=准确 8=敏捷
    /// 9=魔法躲避 10=毒躲避 11=毒恢复 12=HP恢复 13=MP恢复 14=AC2 15=MAC2 16=DC2 17=MC2 18=SC2。</summary>
    public static void AddGroupItemValue(this TCreature self, uint nMaxValue, int[] groupFldValues)
    {
        ref var abil = ref self.m_wAbil;
        abil.MaxHP = (uint)Math.Min(nMaxValue, groupFldValues[0] + (long)abil.MaxHP);
        abil.MaxMP = (uint)Math.Min(nMaxValue, groupFldValues[1] + (long)abil.MaxMP);

        int nMaxAC = M2Config.btMaxAC == 0 ? ushort.MaxValue : int.MaxValue;

        abil.AC1 = (int)Math.Min(nMaxAC, groupFldValues[2] + (long)abil.AC1);
        abil.AC2 = (int)Math.Min(nMaxAC, groupFldValues[14] + (long)abil.AC2);
        abil.MAC1 = (int)Math.Min(nMaxAC, groupFldValues[3] + (long)abil.MAC1);
        abil.MAC2 = (int)Math.Min(nMaxAC, groupFldValues[15] + (long)abil.MAC2);
        abil.DC1 = (int)Math.Min(nMaxAC, groupFldValues[4] + (long)abil.DC1);
        abil.DC2 = (int)Math.Min(nMaxAC, groupFldValues[16] + (long)abil.DC2);
        abil.MC1 = (int)Math.Min(nMaxAC, groupFldValues[5] + (long)abil.MC1);
        abil.MC2 = (int)Math.Min(nMaxAC, groupFldValues[17] + (long)abil.MC2);
        abil.SC1 = (int)Math.Min(nMaxAC, groupFldValues[6] + (long)abil.SC1);
        abil.SC2 = (int)Math.Min(nMaxAC, groupFldValues[18] + (long)abil.SC2);

        // 扩展准确、敏捷为65536 chongchong 2018-01-11
        if (M2Config.btMaxHitPoint == 1)
        {
            self.m_btHitPoint = (ushort)Math.Min(ushort.MaxValue, groupFldValues[7] + self.m_btHitPoint);
            self.m_btSpeedPoint = (ushort)Math.Min(ushort.MaxValue, groupFldValues[8] + self.m_btSpeedPoint);
        }
        else
        {
            self.m_btHitPoint = (byte)Math.Min(byte.MaxValue, groupFldValues[7] + self.m_btHitPoint);
            self.m_btSpeedPoint = (byte)Math.Min(byte.MaxValue, groupFldValues[8] + self.m_btSpeedPoint);
        }

        self.m_nAntiMagic = Math.Min(255, groupFldValues[9] + self.m_nAntiMagic);        // 魔法躲避
        self.m_btAntiPoison = (byte)Math.Min(255, groupFldValues[10] + self.m_btAntiPoison);   // 毒躲避
        self.m_nPoisonRecover = (byte)Math.Min(255, groupFldValues[11] + self.m_nPoisonRecover); // 毒恢复
        self.m_nHealthRecover = (byte)Math.Min(255, groupFldValues[12] + self.m_nHealthRecover); // HP恢复
        self.m_nSpellRecover = (byte)Math.Min(255, groupFldValues[13] + self.m_nSpellRecover);   // MP恢复
    }

    /// <summary>AddAbilitysByCode 特戒代码开关（ObjBase.pas 17675 行 case Code of 核心分支 1:1）。</summary>
    public static void ApplySpecialItemCode(this TCreature self, ushort code)
    {
        switch (code)
        {
            case 111: // 隐身
                self.m_boHideMode = true;
                self.m_boTransparent = false;
                break;
            case 112:
                self.m_boTeleport = true;
                break;
            case 113: // 麻痹
                self.m_boParalysis = true;
                break;
            case 114: // 复活
                self.m_boRevival = true;
                break;
            case 115:
                self.m_boFlameRing = true;
                break;
            case 116:
                self.m_boRecoveryRing = true;
                break;
            case 117:
                self.m_boAngryRing = true;
                break;
            case 118: // 魔法盾
                self.m_boMagicShield = true;
                break;
            case 119:
                self.m_boMuscleRing = true;
                break;
            case 120: // 技巧（快速修炼）
                self.m_boFastTrain = true;
                break;
            case 121:
                self.m_boProbeNecklace = true;
                break;
            case 139: // 防麻
                self.UnParalysis = true;
                break;
            case 140:
                self.m_boSupermanItem = true;
                break;
            case 141: // 经验物品（Dura/nItemExpRate 累计，费率配置接入前记 0 增量）
                self.m_boExpItem = true;
                break;
            case 142: // 力量物品
                self.m_boPowerItem = true;
                break;
            case 143: // 防护身
                self.UnMagicShield = true;
                break;
            case 144: // 防复活
                self.UnRevival = true;
                break;
            case 145:
                self.m_boGuildMove = true;
                break;
            case 146: // 防毒
                self.UnPosion = true;
                break;
            case 147: // 防诱惑
                self.UnTamming = true;
                break;
            case 148: // 防火墙
                self.UnFireCross = true;
                break;
            case 150: // 麻痹护身
                self.m_boParalysis = true;
                self.m_boMagicShield = true;
                break;
            case 151: // 麻痹火球
                self.m_boParalysis = true;
                self.m_boFlameRing = true;
                break;
            case 152: // 麻痹防御
                self.m_boParalysis = true;
                self.m_boRecoveryRing = true;
                break;
            case 153: // 麻痹负载
                self.m_boParalysis = true;
                self.m_boMuscleRing = true;
                break;
        }
    }
}

/// <summary>TCreature 批次J8 扩展：RecalcAbilitys 依赖字段（准确/敏捷/四恢复/魔躲/毒躲 + 特戒标志位）。</summary>
public abstract partial class TCreature
{
    public ushort m_btHitPoint;      // 准确
    public ushort m_btSpeedPoint;    // 敏捷
    public byte m_btAntiPoison;      // 毒躲避
    public byte m_nPoisonRecover;    // 毒恢复
    public byte m_nHealthRecover;    // HP恢复
    public byte m_nSpellRecover;     // MP恢复

    // ---- 特戒/状态标志（AddAbilitysByCode） ----
    public bool m_boHideMode;
    public bool m_boTransparent;
    public bool m_boTeleport;
    public bool m_boParalysis;
    public bool m_boRevival;
    public bool m_boFlameRing;
    public bool m_boRecoveryRing;
    public bool m_boAngryRing;
    public bool m_boMagicShield;
    public bool m_boMuscleRing;
    public bool m_boFastTrain;
    public bool m_boProbeNecklace;
    public bool m_boSupermanItem;
    public bool m_boExpItem;
    public bool m_boPowerItem;
    public bool m_boGuildMove;

    public bool UnParalysis;
    public bool UnMagicShield;
    public bool UnRevival;
    public bool UnPosion;
    public bool UnTamming;
    public bool UnFireCross;
}
