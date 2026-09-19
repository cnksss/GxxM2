using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>ObjBase.pas TAddAbility 字段子集（装备属性聚合目标，批次J9）。</summary>
public class TAddAbility
{
    public int nAC1;
    public int nAC2;
    public int nMAC1;
    public int nMAC2;
    public int nDC1;
    public int nDC2;
    public int nMC1;
    public int nMC2;
    public int nSC1;
    public int nSC2;
    public uint wHP;
    public uint wMP;
    public int wHitPoint;     // 准确
    public int wSpeedPoint;   // 敏捷
    public int wAntiMagic;    // 魔法躲避
    public int wAntiPoison;   // 毒躲避
    public int wPoisonRecover;
    public int wHealthRecover;
    public int wSpellRecover;
    public int nHitSpeed;     // 攻击速度
    public int btLuck;        // 幸运
    public int btUnLuck;      // 诅咒
    public int wNPRecoverTime;
    public int wNPRecoverPoint;
    public byte btWeaponStrong;
    public int bt1DF;
}

/// <summary>物品属性视图（pTStdItem 数值子集 + 装备升级合成的 AC/MAC/DC/MC/SC/HP/MP）。</summary>
public class TStdItemView
{
    public byte StdMode;
    public ushort Shape;
    public ushort Anicount;
    public byte Need;
    public byte NeedLevel;
    public int Source; // 神圣
    public int Stock;
    public int AC1;
    public int AC2;
    public int MAC1;
    public int MAC2;
    public int DC1;
    public int DC2;
    public int MC1;
    public int MC2;
    public int SC1;
    public int SC2;
    public uint HP;
    public uint MP;
    public string DBName = "";
    public string Name = "";   // 物品名（套装组/物品规则匹配用）
    public ushort AniCount;
}

/// <summary>TUserItem 视图（wIndex + CustomProperty 绑定属性子集）。</summary>
public class TUserItemView
{
    public ushort wIndex;
    public readonly byte[] BtValue = new byte[14]; // Delphi btValue[0..13] 升级/极品属性
    public List<(byte btBindType, byte btPercent, int nValue)> CustomProperties = new();
}

/// <summary>
/// ObjBase.pas TSmartObject.GetAccessory 1:1 移植（批次J9，28343 行起全 300 行）：
/// 按 StdMode 分支聚合装备配件属性到 AddAbility（武器/时装 5·6·68·69、项链 19·70·75、新物品 53·88、
/// 手镯 20·24·71·76·79、鞋 52·86、腰带 21·54·72·77·84、防毒 23·74·82、else 通用 AC/MAC）+
/// 通用 DC/MC/SC/HP/MP 累加 + CustomProperty btBindType 1..7 绑定属性（固定值/本级百分比/全件百分比三模式）。
/// GetItemAddValue（装备升级值合成）随物品升级批次接入。
/// </summary>
public static class GetAccessory
{
    public static void Apply(TCreature self, TUserItemView item, TStdItemView? stdItem,
        TAddAbility addAbility, TAddAbility allAddAbility, Func<ushort, TStdItemView?>? getStdItem = null)
    {
        if (stdItem == null)
            return;
        var std = stdItem;

        int nMaxAC = M2Config.btMaxAC == 0 ? ushort.MaxValue : int.MaxValue;

        switch (std.Need)
        {
            case 18:
            case 19:
            case 20:
            case 21: // 内功恢复时间
                addAbility.wNPRecoverTime = (int)Math.Min(ushort.MaxValue, addAbility.wNPRecoverTime + Math.Min(ushort.MaxValue, std.Stock));
                break;
            case 22:
            case 23:
            case 24:
            case 25: // 内功恢复点
                addAbility.wNPRecoverPoint = (int)Math.Min(ushort.MaxValue, addAbility.wNPRecoverPoint + Math.Min(ushort.MaxValue, std.Stock));
                break;
        }

        switch (std.StdMode)
        {
            case 5:
            case 6:
            case 68:
            case 69: // 时装武器 chongchong 2013-10-26
            {
                addAbility.wHitPoint = (int)Math.Min(addAbility.wHitPoint + (long)std.AC2, ushort.MaxValue);
                bool calcSpeed = self.m_btRace != Grobal2Const.RC_HEROOBJECT || M2Config.boHeroCalcWeaponSpeed;
                if (calcSpeed)
                {
                    if (std.MAC2 > 10)
                        addAbility.nHitSpeed = (int)Math.Min(addAbility.nHitSpeed + (long)std.MAC2 - 10, int.MaxValue);
                    else
                        addAbility.nHitSpeed -= std.MAC2;
                }
                addAbility.btLuck = (int)Math.Min(addAbility.btLuck + (long)std.AC1, byte.MaxValue);
                addAbility.btUnLuck = (int)Math.Min(addAbility.btUnLuck + (long)std.MAC1, byte.MaxValue);
                break;
            }
            case 19:
            case 70:
            case 75: // 项链+幸运
            {
                addAbility.wAntiMagic = (int)Math.Min(addAbility.wAntiMagic + (long)std.AC2, ushort.MaxValue);
                addAbility.btUnLuck = (int)Math.Min(addAbility.btUnLuck + (long)std.MAC1, byte.MaxValue);
                addAbility.btLuck = (int)Math.Min(addAbility.btLuck + (long)std.MAC2, byte.MaxValue);
                break;
            }
            case 53:
            case 88: // 新加物品属性
            {
                if (!M2Config.boAddUserItemNewValue)
                {
                    addAbility.wAntiMagic = (int)Math.Min(ushort.MaxValue, addAbility.wAntiMagic + (long)std.AC2);
                    addAbility.btUnLuck = (int)Math.Min(addAbility.btUnLuck + (long)std.MAC1, byte.MaxValue);
                    addAbility.btLuck = (int)Math.Min(addAbility.btLuck + (long)std.MAC2, byte.MaxValue);
                }
                else
                {
                    addAbility.nAC1 = (int)Math.Min(nMaxAC, addAbility.nAC1 + (long)std.AC1);
                    addAbility.nAC2 = (int)Math.Min(nMaxAC, addAbility.nAC2 + (long)std.AC2);
                    addAbility.nMAC1 = (int)Math.Min(nMaxAC, addAbility.nMAC1 + (long)std.MAC1);
                    addAbility.nMAC2 = (int)Math.Min(nMaxAC, addAbility.nMAC2 + (long)std.MAC2);
                }
                break;
            }
            case 20:
            case 24:
            case 71:
            case 76:
            case 79: // 手镯
            {
                addAbility.wHitPoint = (int)Math.Min(addAbility.wHitPoint + (long)std.AC2, ushort.MaxValue);
                addAbility.wSpeedPoint = (int)Math.Min(addAbility.wSpeedPoint + (long)std.MAC2, ushort.MaxValue);
                break;
            }
            case 52:
            case 86: // 鞋子
            {
                addAbility.wHitPoint = (int)Math.Min(addAbility.wHitPoint + (long)std.AC2, ushort.MaxValue);
                addAbility.wSpeedPoint = (int)Math.Min(addAbility.wSpeedPoint + (long)std.MAC2, ushort.MaxValue);
                break;
            }
            case 21:
            case 54: // 腰带
            case 72:
            case 77:
            case 84: // 时装腰带
            {
                addAbility.wHealthRecover = (int)Math.Min(addAbility.wHealthRecover + (long)std.AC2, ushort.MaxValue);
                addAbility.wSpellRecover = (int)Math.Min(addAbility.wSpellRecover + (long)std.MAC2, ushort.MaxValue);
                bool calcSpeed = self.m_btRace != Grobal2Const.RC_HEROOBJECT || M2Config.boHeroCalcWeaponSpeed;
                if (calcSpeed)
                {
                    addAbility.nHitSpeed = (int)Math.Min(addAbility.nHitSpeed + (long)std.AC1, int.MaxValue);
                    addAbility.nHitSpeed -= std.MAC1;
                }
                break;
            }
            case 23:
            case 74:
            case 82: // 防毒
            {
                addAbility.wAntiPoison = (int)Math.Min(addAbility.wAntiPoison + (long)std.AC2, ushort.MaxValue);
                addAbility.wPoisonRecover = (int)Math.Min(addAbility.wPoisonRecover + (long)std.MAC2, ushort.MaxValue);
                bool calcSpeed = self.m_btRace != Grobal2Const.RC_HEROOBJECT || M2Config.boHeroCalcWeaponSpeed;
                if (calcSpeed)
                {
                    addAbility.nHitSpeed = (int)Math.Min(addAbility.nHitSpeed + (long)std.AC1, int.MaxValue);
                    addAbility.nHitSpeed -= std.MAC1;
                }
                break;
            }
            default:
            {
                addAbility.nAC1 = (int)Math.Min(nMaxAC, addAbility.nAC1 + (long)std.AC1);
                addAbility.nAC2 = (int)Math.Min(nMaxAC, addAbility.nAC2 + (long)std.AC2);
                addAbility.nMAC1 = (int)Math.Min(nMaxAC, addAbility.nMAC1 + (long)std.MAC1);
                addAbility.nMAC2 = (int)Math.Min(nMaxAC, addAbility.nMAC2 + (long)std.MAC2);
                break;
            }
        }

        // 通用累加（case 外，1:1）
        addAbility.wHP = (uint)Math.Min(uint.MaxValue, addAbility.wHP + (long)std.HP);
        addAbility.wMP = (uint)Math.Min(uint.MaxValue, addAbility.wMP + (long)std.MP);
        addAbility.nDC1 = (int)Math.Min(nMaxAC, addAbility.nDC1 + (long)std.DC1);
        addAbility.nDC2 = (int)Math.Min(nMaxAC, addAbility.nDC2 + (long)std.DC2);
        addAbility.nMC1 = (int)Math.Min(nMaxAC, addAbility.nMC1 + (long)std.MC1);
        addAbility.nMC2 = (int)Math.Min(nMaxAC, addAbility.nMC2 + (long)std.MC2);
        addAbility.nSC1 = (int)Math.Min(nMaxAC, addAbility.nSC1 + (long)std.SC1);
        addAbility.nSC2 = (int)Math.Min(nMaxAC, addAbility.nSC2 + (long)std.SC2);

        // CustomProperty btBindType 1..7（固定值/本级百分比/全件百分比）
        foreach (var (btBindType, btPercent, nValue) in item.CustomProperties)
        {
            if (nValue <= 0)
                continue;
            bool skipAc = std.StdMode is 5 or 6 or 68 or 69 or 19 or 70 or 75 or 53 or 88 or 20 or 24 or 71 or 76 or 79
                or 52 or 86 or 21 or 54 or 72 or 77 or 84 or 23 or 74 or 82;
            switch (btBindType)
            {
                case 1:
                    if (!skipAc)
                    {
                        if (btPercent == 0)
                            addAbility.nAC2 = (int)Math.Min(nMaxAC, addAbility.nAC2 + (long)nValue);
                        else if (btPercent == 1)
                            addAbility.nAC2 = (int)Math.Min(nMaxAC, addAbility.nAC2 + M2ShareFuncs.DelphiRound((long)std.AC2 / 100.0 * nValue));
                        else if (btPercent == 2)
                            allAddAbility.nAC2 = (int)Math.Min(int.MaxValue, allAddAbility.nAC2 + (long)nValue);
                    }
                    break;
                case 2:
                    if (!skipAc)
                    {
                        if (btPercent == 0)
                            addAbility.nMAC2 = (int)Math.Min(nMaxAC, addAbility.nMAC2 + (long)nValue);
                        else if (btPercent == 1)
                            addAbility.nMAC2 = (int)Math.Min(nMaxAC, addAbility.nMAC2 + M2ShareFuncs.DelphiRound((long)std.MAC2 / 100.0 * nValue));
                        else if (btPercent == 2)
                            allAddAbility.nMAC2 = (int)Math.Min(int.MaxValue, allAddAbility.nMAC2 + (long)nValue);
                    }
                    break;
                case 3:
                    if (btPercent == 0)
                        addAbility.nDC2 = (int)Math.Min(nMaxAC, addAbility.nDC2 + (long)nValue);
                    else if (btPercent == 1)
                        addAbility.nDC2 = (int)Math.Min(nMaxAC, addAbility.nDC2 + M2ShareFuncs.DelphiRound((long)std.DC2 / 100.0 * nValue));
                    else if (btPercent == 2)
                        allAddAbility.nDC2 = (int)Math.Min(int.MaxValue, allAddAbility.nDC2 + (long)nValue);
                    break;
                case 4:
                    if (btPercent == 0)
                        addAbility.nMC2 = (int)Math.Min(nMaxAC, addAbility.nMC2 + (long)nValue);
                    else if (btPercent == 1)
                        addAbility.nMC2 = (int)Math.Min(nMaxAC, addAbility.nMC2 + M2ShareFuncs.DelphiRound((long)std.MC2 / 100.0 * nValue));
                    else if (btPercent == 2)
                        allAddAbility.nMC2 = (int)Math.Min(int.MaxValue, allAddAbility.nMC2 + (long)nValue);
                    break;
                case 5:
                    if (btPercent == 0)
                        addAbility.nSC2 = (int)Math.Min(nMaxAC, addAbility.nSC2 + (long)nValue);
                    else if (btPercent == 1)
                        addAbility.nSC2 = (int)Math.Min(nMaxAC, addAbility.nSC2 + M2ShareFuncs.DelphiRound((long)std.SC2 / 100.0 * nValue));
                    else if (btPercent == 2)
                        allAddAbility.nSC2 = (int)Math.Min(int.MaxValue, allAddAbility.nSC2 + (long)nValue);
                    break;
                case 6:
                    if (btPercent == 0)
                        addAbility.wHP = (uint)Math.Min(uint.MaxValue, addAbility.wHP + (long)nValue);
                    else if (btPercent == 1)
                        addAbility.wHP = (uint)Math.Min(uint.MaxValue, addAbility.wHP + M2ShareFuncs.DelphiRound((long)std.HP / 100.0 * nValue));
                    else if (btPercent == 2)
                        allAddAbility.wHP = (uint)Math.Min(int.MaxValue, allAddAbility.wHP + (long)nValue);
                    break;
                case 7:
                    if (btPercent == 0)
                        addAbility.wMP = (uint)Math.Min(uint.MaxValue, addAbility.wMP + (long)nValue);
                    else if (btPercent == 1)
                        addAbility.wMP = (uint)Math.Min(uint.MaxValue, addAbility.wMP + M2ShareFuncs.DelphiRound((long)std.MP / 100.0 * nValue));
                    else if (btPercent == 2)
                        allAddAbility.wMP = (uint)Math.Min(int.MaxValue, allAddAbility.wMP + (long)nValue);
                    break;
            }
        }
    }
}
