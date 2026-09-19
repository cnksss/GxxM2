using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>Delphi TNakedAbility（裸体成长：每级下/上限对）。</summary>
public class TNakedAbility
{
    public int DC;   // LowByte=下限成长 HighByte=上限成长（pack 对）
    public int MC;
    public int SC;
    public int AC;
    public int MAC;
    public int HP;
    public int MP;
    public int Hit;
    public int Speed;
}

/// <summary>Delphi m_BonusAbil（属性点总量）。</summary>
public class TBonusAbil
{
    public int DC;
    public int MC;
    public int SC;
    public int AC;
    public int MAC;
    public int HP;
    public int MP;
    public int Hit;
    public int Speed;
    public int BonusPoint; // 未分配属性点（HumanInfo 编辑用）
}

/// <summary>
/// 批次J11：RecalcAbilitys 主循环 Delphi 扩展分支移植：
/// ① GetItemAddValue（ItmUnit.pas 1143：UserItem.btValue 升级值按 StdMode 合成到物品视图，
///    武器含诅咒/幸运/攻速三分支与神圣 btValue[7]）；
/// ② RecalcAdjusBonus（ObjPlayer.pas 10947：属性点按职业 BonusTick/NakedAbil 配置
///    经 AdjustAb2 均衡分配下/上限后并入 DC/MC/SC/AC/MAC，MaxHP/MP 按 BonusTick.HP/MP 折算）；
/// ③ 宠物加成（boPetHPToMaster/boPetDCToMaster 按 nPetAbilToMasterRate 百分比）。
/// </summary>
public static class RecalcBonus
{
    /// <summary>ItmUnit.pas GetItemAddValue 1:1（数值分支；特效/颜色/限时等展示字段略）。</summary>
    public static void GetItemAddValue(TUserItemView userItem, TStdItemView std)
    {
        long maxValue = int.MaxValue;
        byte[] v = userItem.BtValue;
        switch (std.StdMode)
        {
            case 5:
            case 6:
            case 68:
            case 69: // 时装武器
            {
                std.DC1 = (int)Math.Min(maxValue, std.DC1 + (long)v[9]);
                std.DC2 = (int)Math.Min(maxValue, std.DC2 + (long)v[0]);
                std.MC1 = (int)Math.Min(maxValue, std.MC1 + (long)v[11]);
                std.MC2 = (int)Math.Min(maxValue, std.MC2 + (long)v[1]);
                std.SC1 = (int)Math.Min(maxValue, std.SC1 + (long)v[12]);
                std.SC2 = (int)Math.Min(maxValue, std.SC2 + (long)v[2]);
                std.AC1 = (int)Math.Min(maxValue, std.AC1 + (long)v[3]); // 诅咒
                std.AC2 = (int)Math.Min(maxValue, std.AC2 + (long)v[5]);

                // 攻速三分支（Mac2 == 0 / < 10 / >= 10）
                int mac2 = std.MAC2;
                if (v[6] > 0)
                {
                    if (mac2 == 0)
                        mac2 = (int)Math.Min(10L + v[6], maxValue);
                    else if (mac2 < 10)
                    {
                        mac2 -= v[6];
                        if (mac2 < 0)
                            mac2 = (int)Math.Min(10L + Math.Abs(mac2), maxValue);
                    }
                    else
                        mac2 = (int)Math.Min(mac2 + (long)v[6], maxValue);
                }
                std.MAC2 = mac2;

                std.MAC1 = (int)Math.Min(maxValue, std.MAC1 + (long)v[4]); // 幸运

                if ((byte)(v[7] - 1) < 10) // 神圣
                    std.Source = v[7];
                break;
            }
            case 10:
            case 11:
            case 66:
            case 67:
            {
                std.AC2 = (int)Math.Min(maxValue, std.AC2 + (long)v[0]);
                std.MAC2 = (int)Math.Min(maxValue, std.MAC2 + (long)v[1]);
                std.DC1 = (int)Math.Min(maxValue, std.DC1 + (long)v[9]);
                std.DC2 = (int)Math.Min(maxValue, std.DC2 + (long)v[2]);
                std.MC1 = (int)Math.Min(maxValue, std.MC1 + (long)v[11]);
                std.MC2 = (int)Math.Min(maxValue, std.MC2 + (long)v[3]);
                std.SC1 = (int)Math.Min(maxValue, std.SC1 + (long)v[12]);
                std.SC2 = (int)Math.Min(maxValue, std.SC2 + (long)v[4]);
                break;
            }
            case 15:
            case 12: // 盾牌
            case 28: // 马牌
            case 16: // 斗笠
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
            case 26:
            case 29:
            case 30:
            case 51:
            case 52:
            case 53:
            case 54:
            case 62:
            case 63:
            case 64:
            case 65:
            case 75:
            case 76:
            case 77:
            case 78:
            case 79:
            case 80:
            case 81:
            case 82:
            case 83:
            case 84:
            case 85:
            case 86:
            case 87:
            case 88:
            case 89:
            case 90:
            {
                std.AC2 = (int)Math.Min(maxValue, std.AC2 + (long)v[0]);
                std.MAC2 = (int)Math.Min(maxValue, std.MAC2 + (long)v[1]);
                std.DC1 = (int)Math.Min(maxValue, std.DC1 + (long)v[9]);
                std.DC2 = (int)Math.Min(maxValue, std.DC2 + (long)v[2]);
                std.MC1 = (int)Math.Min(maxValue, std.MC1 + (long)v[11]);
                std.MC2 = (int)Math.Min(maxValue, std.MC2 + (long)v[3]);
                std.SC1 = (int)Math.Min(maxValue, std.SC1 + (long)v[12]);
                std.SC2 = (int)Math.Min(maxValue, std.SC2 + (long)v[4]);
                if (v[5] > 0) std.Need = v[5];
                if (v[6] > 0) std.NeedLevel = v[6];
                break;
            }
        }
    }
}

/// <summary>TPlayObject 批次J11 扩展：属性点/宠物加成。</summary>
public static class RecalcBonusExt
{
    /// <summary>ObjPlayer.pas AdjustAb2 内嵌过程 1:1：Val 点按"低限先涨"均衡分配到下/上限。</summary>
    public static void AdjustAb2(int abil, ushort val, out ushort lov, out ushort hiv)
    {
        byte lo = (byte)(abil & 0xFF);
        byte hi = (byte)((abil >> 16) & 0xFF);
        lov = 0;
        hiv = 0;
        for (int i = 1; i <= val; i++)
        {
            if (lo + 1 < hi)
            {
                lo++;
                lov++;
            }
            else
            {
                hi++;
                hiv++;
            }
        }
    }

    /// <summary>ObjPlayer.pas RecalcAdjusBonus 1:1（属性点 → DC/MC/SC/AC/MAC + MaxHP/MP 折算）。</summary>
    public static void RecalcAdjusBonus(this TPlayObject self)
    {
        (TBonusAbil bonus, TNakedAbility naked) = self.m_btJob switch
        {
            0 => (M2Config.BonusAbilofWarr, M2Config.NakedAbilofWarr),
            1 => (M2Config.BonusAbilofWizard, M2Config.NakedAbilofWizard),
            _ => (M2Config.BonusAbilofTaos, M2Config.NakedAbilofTaos)
        };

        int adc = self.m_BonusAbil.DC / Math.Max(1, bonus.DC); // Delphi: div BonusTick.DC
        int amc = self.m_BonusAbil.MC / Math.Max(1, bonus.MC);
        int asc = self.m_BonusAbil.SC / Math.Max(1, bonus.SC);
        int aac = self.m_BonusAbil.AC / Math.Max(1, bonus.AC);
        int amac = self.m_BonusAbil.MAC / Math.Max(1, bonus.MAC);

        AdjustAb2(naked.DC, (ushort)adc, out var ldc, out var hdc);
        AdjustAb2(naked.MC, (ushort)amc, out var lmc, out var hmc);
        AdjustAb2(naked.SC, (ushort)asc, out var lsc, out var hsc);
        AdjustAb2(naked.AC, (ushort)aac, out var lac, out var hac);
        AdjustAb2(naked.MAC, (ushort)amac, out var lmac, out var hmac);

        int nMaxAC = M2Config.btMaxAC == 0 ? ushort.MaxValue : int.MaxValue;
        ref var abil = ref self.m_wAbil;
        abil.DC1 = (int)Math.Min(nMaxAC, abil.DC1 + (long)ldc);
        abil.DC2 = (int)Math.Min(nMaxAC, abil.DC2 + (long)hdc);
        abil.MC1 = (int)Math.Min(nMaxAC, abil.MC1 + (long)lmc);
        abil.MC2 = (int)Math.Min(nMaxAC, abil.MC2 + (long)hmc);
        abil.SC1 = (int)Math.Min(nMaxAC, abil.SC1 + (long)lsc);
        abil.SC2 = (int)Math.Min(nMaxAC, abil.SC2 + (long)hsc);
        abil.AC1 = (int)Math.Min(nMaxAC, abil.AC1 + (long)lac);
        abil.AC2 = (int)Math.Min(nMaxAC, abil.AC2 + (long)hac);
        abil.MAC1 = (int)Math.Min(nMaxAC, abil.MAC1 + (long)lmac);
        abil.MAC2 = (int)Math.Min(nMaxAC, abil.MAC2 + (long)hmac);

        long hpCap = M2Config.btMaxLevel switch { 0 => ushort.MaxValue, 1 => int.MaxValue, _ => uint.MaxValue };
        abil.MaxHP = (uint)Math.Min(hpCap, abil.MaxHP + (long)(bonus.HP / Math.Max(1, naked.HP)));
        abil.MaxMP = (uint)Math.Min(hpCap, abil.MaxMP + (long)(bonus.MP / Math.Max(1, naked.MP)));
    }

    /// <summary>主循环宠物加成分支 1:1（m_MyGamePet 非空且 nPetAbilToMasterRate > 0）。</summary>
    public static void ApplyPetBonus(this TPlayObject self, TPlayObject pet)
    {
        if (M2Config.nPetAbilToMasterRate <= 0)
            return;
        ref var abil = ref self.m_wAbil;
        int rate = M2Config.nPetAbilToMasterRate;
        if (M2Config.boPetHPToMaster)
        {
            long v = abil.MaxHP + M2ShareFuncs.DelphiRound(pet.m_wAbil.MaxHP / 100.0 * rate);
            abil.MaxHP = (uint)Math.Min(v, uint.MaxValue);
        }
        if (M2Config.boPetDCToMaster)
        {
            abil.DC1 = (int)Math.Min(int.MaxValue, abil.DC1 + M2ShareFuncs.DelphiRound(pet.m_wAbil.DC1 / 100.0 * rate));
            abil.DC2 = (int)Math.Min(int.MaxValue, abil.DC2 + M2ShareFuncs.DelphiRound(pet.m_wAbil.DC2 / 100.0 * rate));
        }
    }
}

public partial class TPlayObject
{
    /// <summary>m_BonusAbil：已分配属性点总量。</summary>
    public readonly TBonusAbil m_BonusAbil = new();

    /// <summary>m_MyGamePet：跟随宠物。</summary>
    public TPlayObject? m_MyGamePet;
}
