using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>Grobal2.pas U_ 装备槽位常量（1:1）。</summary>
public static class UseSlots
{
    public const int U_DRESS = 0;          // 衣服
    public const int U_WEAPON = 1;         // 武器
    public const int U_NECKLACE = 3;       // 项链
    public const int U_HELMET = 4;         // 头盔
    public const int U_ARMRINGL = 5;       // 左手镯
    public const int U_ARMRINGR = 6;       // 右手镯
    public const int U_RINGL = 7;          // 左戒指
    public const int U_RINGR = 8;          // 右戒指
    public const int U_BUJUK = 9;          // 符
    public const int U_BELT = 10;          // 腰带
    public const int U_BOOTS = 11;         // 鞋
    public const int U_CHARM = 12;         // 宝石
    public const int U_SHIELD = 16;        // 盾牌 chongchong 2013-09-16
    public const int SlotCount = 21;       // m_UseItems 槽位数
}

/// <summary>
/// 批次J10：RecalcAbilitys 主循环整合（TPlayObject.RecalcAbilitys 核心链）：
/// ① 等级基础（RecalcLevelAbilitys，boUseSysDef 判定）→
/// ② m_UseItems 装备槽遍历（GetAccessory 聚合到 m_AddAbil + AddAbilitysByCode 特戒开关）→
/// ③ m_AddAbil 并入 m_WAbil（基础 + 装备）→ ④ 尾部 HP/MP clamp。
/// </summary>
public static class RecalcAbilitysChain
{
    public static void RecalcAbilitys(this TPlayObject self)
    {
        // ① 等级基础
        self.RecalcLevelAbilitys(false);

        // ② 装备槽遍历
        var add = self.m_AddAbil;
        var all = self.m_AllAddAbil;
        ResetAdd(add);

        for (int i = 0; i < UseSlots.SlotCount; i++)
        {
            var userItem = self.m_UseItems[i];
            if (userItem == null || userItem.wIndex == 0)
                continue;
            var std = self.StdItemResolver?.Invoke(userItem.wIndex);
            if (std == null)
                continue;
            RecalcBonus.GetItemAddValue(userItem, std); // 装备升级值合成（J11）
            GetAccessory.Apply(self, userItem, std, add, all);
            // 特戒开关（Delphi AddAbilitysByCode IsShape 双路：主循环传 I <> U_SHIELD）
            self.ApplySpecialItemCode(std.Shape);
            if (i != UseSlots.U_SHIELD)
                self.ApplySpecialItemCode(std.Anicount);
        }

        // ③ m_AddAbil 并入 m_WAbil（基础 + 装备）
        ref var abil = ref self.m_wAbil;
        abil.DC1 += add.nDC1;
        abil.DC2 += add.nDC2;
        abil.MC1 += add.nMC1;
        abil.MC2 += add.nMC2;
        abil.SC1 += add.nSC1;
        abil.SC2 += add.nSC2;
        abil.AC1 += add.nAC1;
        abil.AC2 += add.nAC2;
        abil.MAC1 += add.nMAC1;
        abil.MAC2 += add.nMAC2;
        self.RecalcAdjusBonus(); // 属性点分配（J11）

        if (self.m_MyGamePet != null)
            self.ApplyPetBonus(self.m_MyGamePet); // 宠物加成（J11）

        abil.MaxHP = (uint)Math.Min(uint.MaxValue, abil.MaxHP + (long)add.wHP);
        abil.MaxMP = (uint)Math.Min(uint.MaxValue, abil.MaxMP + (long)add.wMP);

        // ④ 尾部 HP/MP clamp
        if (abil.HP > abil.MaxHP)
            abil.HP = abil.MaxHP;
        if (abil.MP > abil.MaxMP)
            abil.MP = abil.MaxMP;
    }

    private static void ResetAdd(TAddAbility add)
    {
        add.nAC1 = add.nAC2 = add.nMAC1 = add.nMAC2 = 0;
        add.nDC1 = add.nDC2 = add.nMC1 = add.nMC2 = 0;
        add.nSC1 = add.nSC2 = 0;
        add.wHP = add.wMP = 0;
        add.wHitPoint = add.wSpeedPoint = 0;
        add.wAntiMagic = add.wAntiPoison = 0;
        add.wPoisonRecover = add.wHealthRecover = add.wSpellRecover = 0;
        add.nHitSpeed = add.btLuck = add.btUnLuck = 0;
    }
}

public partial class TPlayObject
{
    /// <summary>m_UseItems 装备槽（索引 = U_ 常量）。</summary>
    public TUserItemView?[] m_UseItems = new TUserItemView?[UseSlots.SlotCount];

    /// <summary>m_AddAbil（装备聚合）与 AllAddAbility。</summary>
    public readonly TAddAbility m_AddAbil = new();
    public readonly TAddAbility m_AllAddAbil = new();

    /// <summary>UserEngine.GetStdItem 接缝（主循环注入）。</summary>
    public Func<ushort, TStdItemView?>? StdItemResolver;
}
