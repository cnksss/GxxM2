using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// ObjBase.pas TBaseObject.RecalcLevelAbilitys 1:1 移植（批次J5正片）。
/// boUseSysDef 分派（UseDefault/IsSysDef/超1000级 AutoCalcLevel1000）→ 三职业系统公式
/// （Delphi Round 银行家舍入）/ 自定义表查表 / 超1000级 Add 增量外推；尾部 MaxHP·MP 兜底与 clamp。
/// </summary>
public static class AbilRecalc
{
    public const int MAXUPLEVEL = 65535; // M2Share: MAXUPLEVEL = High(Word)
    public const int MAXCHANGELEVEL = 1000;

    /// <summary>ObjBase.pas TBaseObject.GetLevelExp 1:1（英雄/宠物/玩家三路 + 超千级固定经验外推）。
    /// 经验表内容（dwNeedExps 族）由 ExpCalc 表达式配置装载批次填充。</summary>
    public static uint GetLevelExp(TCreature self, uint nLevel)
    {
        if (self.m_btRace == Grobal2Const.RC_HEROOBJECT)
        {
            if (nLevel <= MAXCHANGELEVEL)
                return M2Config.dwHeroNeedExps[nLevel];
            long i64 = (long)M2Config.dwHeroNeedExps[^1] + Math.Max(0, (int)(nLevel - MAXCHANGELEVEL)) * M2Config.dwHeroLevel1000FixedExp;
            return (uint)Math.Min(uint.MaxValue, i64);
        }

        if (nLevel <= MAXCHANGELEVEL)
        {
            if (self.m_boGamePet)
            {
                if (nLevel == 0)
                    nLevel = 1;
                return M2Config.dwPetNeedExps[nLevel];
            }
            return M2Config.dwNeedExps[nLevel];
        }

        if (self.m_boGamePet)
        {
            if (M2Config.boPetUseFixExp)
                return (uint)Math.Min(uint.MaxValue, (long)M2Config.dwPetNeedExps[^1] + Math.Max(0, (int)(nLevel - MAXCHANGELEVEL)) * 10000000L);
            return (uint)Math.Min(uint.MaxValue, (long)(nLevel - MAXCHANGELEVEL) * M2Config.nPetAddExp + M2Config.nPetBaseExp);
        }

        if (M2Config.boUseFixExp)
        {
            long i64;
            if (self.m_btRace == Grobal2Const.RC_PLAYOBJECT)
                i64 = (long)M2Config.dwNeedExps[^1] + Math.Max(0, (int)(nLevel - MAXCHANGELEVEL)) * 10000000L;
            else
                i64 = (long)M2Config.dwHeroNeedExps[^1] + Math.Max(0, (int)(nLevel - MAXCHANGELEVEL)) * 10000000L;
            return (uint)Math.Min(uint.MaxValue, i64);
        }

        // 非固定经验：比率外推（nAddExp/nBaseExp）
        return (uint)Math.Min(uint.MaxValue, (long)(nLevel - MAXCHANGELEVEL) * M2Config.nAddExp + M2Config.nBaseExp);
    }

    /// <summary>boUseSysDef 判定（ObjBase.pas 1:1，High(Base)+1 = 1000）。</summary>
    public static bool UseSysDef(byte btJob, uint nLevel, bool isSysDef)
    {
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        return cfg.UseDefault || isSysDef ||
            (!cfg.UseDefault && nLevel > (uint)cfg.HumAbil[0].Base.Length && cfg.HumAbil[btJob].AutoCalcLevel1000);
    }

    /// <summary>RecalcLevelAbilitys 主体（TCreature.m_wAbil 就地重算）。</summary>
    public static void RecalcLevelAbilitys(this TCreature self, bool isSysDef)
    {
        // Delphi: if m_boGamePet then Exit
        if (self.m_boGamePet)
            return;

        long nMaxValue;
        if (M2Config.btMaxLevel == 0)
            nMaxValue = MAXUPLEVEL;
        else if (M2Config.btMaxLevel == 1)
            nMaxValue = int.MaxValue;
        else
            nMaxValue = uint.MaxValue;

        uint nLevel = self.m_wAbil.Level;
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        bool boUseSysDef = UseSysDef(self.m_btJob, nLevel, isSysDef);

        if (boUseSysDef)
        {
            switch (self.m_btJob)
            {
                case 2: // 道士
                {
                    long int64Value = M2ShareFuncs.DelphiRound(((long)nLevel / (double)M2Config.nLevelValueOfTaosHP + M2Config.nLevelValueOfTaosHPRate) * nLevel);
                    int64Value = 14 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxHP = (uint)Math.Min(nMaxValue, int64Value);
                    int64Value = M2ShareFuncs.DelphiRound((long)nLevel / (double)M2Config.nLevelValueOfTaosMP * 2.2 * nLevel);
                    int64Value = 13 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxMP = (uint)Math.Min(nMaxValue, int64Value);
                    if (nLevel > 1000)
                    {
                        self.m_wAbil.MaxWeight = ushort.MaxValue;
                        self.m_wAbil.MaxWearWeight = ushort.MaxValue;
                        self.m_wAbil.MaxHandWeight = ushort.MaxValue;
                    }
                    else
                    {
                        self.m_wAbil.MaxWeight = (int)Math.Min(ushort.MaxValue, Math.Max(50 + M2ShareFuncs.DelphiRound(nLevel / 4.0 * nLevel), 0));
                        self.m_wAbil.MaxWearWeight = (int)Math.Min(ushort.MaxValue, Math.Max(15 + M2ShareFuncs.DelphiRound(nLevel / 50.0 * nLevel), 0));
                        self.m_wAbil.MaxHandWeight = (int)Math.Min(ushort.MaxValue, Math.Max(12 + M2ShareFuncs.DelphiRound(nLevel / 42.0 * nLevel), 0));
                    }

                    uint n = nLevel / 7;
                    int dc1 = (int)n - 1;
                    self.m_wAbil.DC1 = Math.Max(dc1, 0);
                    self.m_wAbil.DC2 = Math.Max(1, (int)n);
                    self.m_wAbil.MC1 = 0;
                    self.m_wAbil.MC2 = 0;
                    int sc1 = (int)n - 1;
                    self.m_wAbil.SC1 = Math.Max(sc1, 0);
                    self.m_wAbil.SC2 = Math.Max(1, (int)n);
                    self.m_wAbil.AC1 = 0;
                    self.m_wAbil.AC2 = 0;
                    uint mn = (uint)M2ShareFuncs.DelphiRound(nLevel / 6.0);
                    self.m_wAbil.MAC1 = (int)(mn / 2);
                    self.m_wAbil.MAC2 = (int)(mn + 1);
                    break;
                }
                case 1: // 法师
                {
                    long int64Value = M2ShareFuncs.DelphiRound(((long)nLevel / (double)M2Config.nLevelValueOfWizardHP + M2Config.nLevelValueOfWizardHPRate) * nLevel);
                    int64Value = 14 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxHP = (uint)Math.Min(nMaxValue, int64Value);
                    int64Value = M2ShareFuncs.DelphiRound((nLevel / 5.0 + 2) * 2.2 * nLevel);
                    int64Value = 13 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxMP = (uint)Math.Min(nMaxValue, int64Value);
                    if (nLevel > 1000)
                    {
                        self.m_wAbil.MaxWeight = ushort.MaxValue;
                        self.m_wAbil.MaxWearWeight = ushort.MaxValue;
                        self.m_wAbil.MaxHandWeight = ushort.MaxValue;
                    }
                    else
                    {
                        self.m_wAbil.MaxWeight = (int)Math.Min(ushort.MaxValue, Math.Max(50 + M2ShareFuncs.DelphiRound(nLevel / 5.0 * nLevel), 0));
                        self.m_wAbil.MaxWearWeight = (int)Math.Min(ushort.MaxValue, Math.Max(15 + M2ShareFuncs.DelphiRound(nLevel / 100.0 * nLevel), 0));
                        self.m_wAbil.MaxHandWeight = (int)Math.Min(ushort.MaxValue, Math.Max(12 + M2ShareFuncs.DelphiRound(nLevel / 90.0 * nLevel), 0));
                    }

                    uint n = nLevel / 7;
                    int dc1 = (int)n - 1;
                    self.m_wAbil.DC1 = Math.Max(dc1, 0);
                    self.m_wAbil.DC2 = Math.Max(1, (int)n);
                    int mc1 = (int)n - 1;
                    self.m_wAbil.MC1 = Math.Max(mc1, 0);
                    self.m_wAbil.MC2 = Math.Max(1, (int)n);
                    self.m_wAbil.SC1 = 0;
                    self.m_wAbil.SC2 = 0;
                    self.m_wAbil.AC1 = 0;
                    self.m_wAbil.AC2 = 0;
                    self.m_wAbil.MAC1 = 0;
                    self.m_wAbil.MAC2 = 0;
                    break;
                }
                case 0: // 战士
                {
                    long int64Value = M2ShareFuncs.DelphiRound(((long)nLevel / (double)M2Config.nLevelValueOfWarrHP + M2Config.nLevelValueOfWarrHPRate + nLevel / 20.0)
                        * nLevel);
                    int64Value = 14 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxHP = (uint)Math.Min(nMaxValue, int64Value);
                    int64Value = M2ShareFuncs.DelphiRound(nLevel * 3.5);
                    int64Value = 11 + Math.Max(0, int64Value);
                    self.m_wAbil.MaxMP = (uint)Math.Min(nMaxValue, int64Value);
                    if (nLevel > 1000)
                    {
                        self.m_wAbil.MaxWeight = ushort.MaxValue;
                        self.m_wAbil.MaxWearWeight = ushort.MaxValue;
                        self.m_wAbil.MaxHandWeight = ushort.MaxValue;
                    }
                    else
                    {
                        self.m_wAbil.MaxWeight = (int)Math.Min(ushort.MaxValue, Math.Max(50 + M2ShareFuncs.DelphiRound(nLevel / 3.0 * nLevel), 0));
                        self.m_wAbil.MaxWearWeight = (int)Math.Min(ushort.MaxValue, Math.Max(15 + M2ShareFuncs.DelphiRound(nLevel / 20.0 * nLevel), 0));
                        self.m_wAbil.MaxHandWeight = (int)Math.Min(ushort.MaxValue, Math.Max(12 + M2ShareFuncs.DelphiRound(nLevel / 13.0 * nLevel), 0));
                    }

                    if (nLevel / 5 > 0)
                    {
                        int dc1 = (int)(nLevel / 5) - 1;
                        self.m_wAbil.DC1 = Math.Max(dc1, 0);
                    }
                    else
                        self.m_wAbil.DC1 = 0;

                    self.m_wAbil.DC2 = Math.Max(1, (int)(nLevel / 5));
                    self.m_wAbil.SC1 = 0;
                    self.m_wAbil.SC2 = 0;
                    self.m_wAbil.MC1 = 0;
                    self.m_wAbil.MC2 = 0;
                    self.m_wAbil.AC1 = 0;
                    self.m_wAbil.AC2 = (int)(nLevel / 7);
                    self.m_wAbil.MAC1 = 0;
                    self.m_wAbil.MAC2 = 0;
                    break;
                }
            }
        }
        else
        {
            if (nLevel <= 0)
                nLevel = 1;

            if (nLevel > 0 && nLevel <= (uint)cfg.HumAbil[0].Base.Length)
            {
                var baseAbil = cfg.HumAbil[self.m_btJob].Base[nLevel - 1];
                self.m_wAbil.MaxHP = (uint)Math.Min(nMaxValue, baseAbil.MaxHP);
                self.m_wAbil.MaxMP = (uint)Math.Min(nMaxValue, baseAbil.MaxMP);
                self.m_wAbil.MaxWeight = (int)Math.Min(ushort.MaxValue, baseAbil.MaxWeight);
                self.m_wAbil.MaxWearWeight = (int)Math.Min(ushort.MaxValue, baseAbil.MaxWearWeight);
                self.m_wAbil.MaxHandWeight = (int)Math.Min(ushort.MaxValue, baseAbil.MaxHandWeight);
                self.m_wAbil.DC1 = (int)baseAbil.DC1;
                self.m_wAbil.DC2 = (int)baseAbil.DC2;
                self.m_wAbil.MC1 = (int)baseAbil.MC1;
                self.m_wAbil.MC2 = (int)baseAbil.MC2;
                self.m_wAbil.SC1 = (int)baseAbil.SC1;
                self.m_wAbil.SC2 = (int)baseAbil.SC2;
                self.m_wAbil.AC1 = (int)baseAbil.AC1;
                self.m_wAbil.AC2 = (int)baseAbil.AC2;
                self.m_wAbil.MAC1 = (int)baseAbil.MAC1;
                self.m_wAbil.MAC2 = (int)baseAbil.MAC2;
            }
            else
            {
                int nLevelSub = (int)(nLevel - (uint)cfg.HumAbil[self.m_btJob].Base.Length); // High(Base)=999 → nLevel-(999+1)
                var baseAbil = cfg.HumAbil[self.m_btJob].Base[^1];
                var addAbil = cfg.HumAbil[self.m_btJob].Add;
                long int64Value = baseAbil.MaxHP + addAbil.MaxHP * nLevelSub;
                self.m_wAbil.MaxHP = (uint)Math.Min(nMaxValue, int64Value);
                int64Value = baseAbil.MaxMP + addAbil.MaxMP * nLevelSub;
                self.m_wAbil.MaxMP = (uint)Math.Min(nMaxValue, int64Value);
                int64Value = baseAbil.MaxWeight + addAbil.MaxWeight * nLevelSub;
                self.m_wAbil.MaxWeight = (int)Math.Min(ushort.MaxValue, int64Value);
                int64Value = baseAbil.MaxWearWeight + addAbil.MaxWearWeight * nLevelSub;
                self.m_wAbil.MaxWearWeight = (int)Math.Min(ushort.MaxValue, int64Value);
                int64Value = baseAbil.MaxHandWeight + addAbil.MaxHandWeight * nLevelSub;
                self.m_wAbil.MaxHandWeight = (int)Math.Min(ushort.MaxValue, int64Value);
                int64Value = baseAbil.DC1 + addAbil.DC1 * nLevelSub;
                self.m_wAbil.DC1 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.DC2 + addAbil.DC2 * nLevelSub;
                self.m_wAbil.DC2 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.MC1 + addAbil.MC1 * nLevelSub;
                self.m_wAbil.MC1 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.MC2 + addAbil.MC2 * nLevelSub;
                self.m_wAbil.MC2 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.SC1 + addAbil.SC1 * nLevelSub;
                self.m_wAbil.SC1 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.SC2 + addAbil.SC2 * nLevelSub;
                self.m_wAbil.SC2 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.AC1 + addAbil.AC1 * nLevelSub;
                self.m_wAbil.AC1 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.AC2 + addAbil.AC2 * nLevelSub;
                self.m_wAbil.AC2 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.MAC1 + addAbil.MAC1 * nLevelSub;
                self.m_wAbil.MAC1 = (int)Math.Min(int.MaxValue, int64Value);
                int64Value = baseAbil.MAC2 + addAbil.MAC2 * nLevelSub;
                self.m_wAbil.MAC2 = (int)Math.Min(int.MaxValue, int64Value);
            }
        }

        if (self.m_wAbil.MaxHP <= 0)
            self.m_wAbil.MaxHP = 15;
        if (self.m_wAbil.MaxMP <= 0)
            self.m_wAbil.MaxMP = 15;
        if (self.m_wAbil.HP > self.m_wAbil.MaxHP)
            self.m_wAbil.HP = self.m_wAbil.MaxHP;
        if (self.m_wAbil.MP > self.m_wAbil.MaxMP)
            self.m_wAbil.MP = self.m_wAbil.MaxMP;
    }
}
