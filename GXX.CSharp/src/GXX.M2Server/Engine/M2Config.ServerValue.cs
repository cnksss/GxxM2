using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config 字段子集（批次J4a：FSrvValue 依赖，typed constant 默认值 1:1）。
/// </summary>
public static partial class M2Config
{
    public static uint nSendBlock = 8000;          // 与网关一次传输数据块大小
    public static uint nCheckBlock = 20000;        // 传输指定大小后自检
    public static uint nAvailableBlock = 8000;
    public static uint nGateLoad;
    public static bool boViewHackMessage;
    public static bool boViewAdmissionFailure;
    public static uint nMonGenRate = 10;           // 刷怪倍率
    public static uint dwProcessMonstersTime = 250; // 处理怪物时间间隔
    public static uint dwRegenMonstersTime = 200;  // 刷怪间隔
    public static uint nProcessMonsterInterval = 2; // 空闲时处理怪物检测次数
    public static bool boSendCompressDataToRunGate;
    public static bool boIsOldClient = true;

    // ---- 等级属性公式参数（RecalcLevelAbilitys 使用，typed constant 默认值 1:1） ----
    public static byte btMaxLevel;
            public static byte btMaxAC;          // 0=AC 上限 High(Word)
            public static byte btMaxHitPoint = 1; // 1=准确敏捷上限 High(Word)                     // 0=上限65535 1=High(Integer) 其他=High(LongWord)
    public static int nLevelValueOfTaosHP = 6;
    public static double nLevelValueOfTaosHPRate = 2.5;
    public static int nLevelValueOfTaosMP = 8;
    public static int nLevelValueOfWizardHP = 15;
    public static double nLevelValueOfWizardHPRate = 1.8;
    public static int nLevelValueOfWarrHP = 4;
    public static double nLevelValueOfWarrHPRate = 4.5;

    // ---- 升级经验表（GetLevelExp 使用；索引 n = 等级 n，索引 0 未用。缺省内容 = g_dwOldNeedExps 族） ----
    public static uint[] dwNeedExps = new uint[1001];       // 玩家 1..1000 级（MAXCHANGELEVEL）
    public static uint[] dwHeroNeedExps = new uint[1001];   // 英雄
    public static uint[] dwPetNeedExps = new uint[1001];    // 宠物（Delphi LoadExp 缺省写入英雄表值）
    public static bool boUseFixExp = true; // typed constant: boUseFixExp: True
    public static bool boPetUseFixExp;
    public static int nPetAddExp;
    public static int nBaseExp = 100000000;  // typed constant
    public static int nAddExp = 1000000;     // typed constant
    public static int nPetBaseExp;
    public static uint dwHeroLevel1000FixedExp;
    public static bool boAddUserItemNewValue = true; // typed constant: True
    public static bool boHeroCalcWeaponSpeed;        // 英雄计算武器速度（GetAccessory 使用）
    public static bool boClientCheckModule;          // 客户端校验模块（ClientModules）
    public static bool boClientAddModule;            // 客户端添加模块
    public static bool boAddModuleList;              // 添加模块名单
    public static bool boPetHPToMaster;              // 宠物 HP 加成到主人
    public static bool boPetDCToMaster;              // 宠物 DC 加成到主人
    public static int nPetAbilToMasterRate;          // 宠物加成百分比
    public static readonly TBonusAbil BonusAbilofWarr = new();
    public static readonly TBonusAbil BonusAbilofWizard = new();
    public static readonly TBonusAbil BonusAbilofTaos = new();
    public static readonly TNakedAbility NakedAbilofWarr = new();
    public static readonly TNakedAbility NakedAbilofWizard = new();
    public static readonly TNakedAbility NakedAbilofTaos = new();

    /// <summary>M2Share.pas LoadExp() 核心 1:1：Exp INI 的 Level1..1000 缺省落 g_dwOldNeedExps[i]；
    /// HeroExp 缺省落 g_dwOldHeroNeedExps[i]；GamePetExp 缺省同样落英雄表（Delphi 原文如此）。</summary>
    public static void LoadExp(TFastIniFile expConfig)
    {
        for (int i = 1; i <= 1000; i++)
        {
            string s = expConfig.ReadString("Exp", "Level" + i, "");
            long v = GXX.Core.Rtl.DelphiRTL.StrToInt64Def(s, 0);
            v = Math.Max(0, Math.Min(v, int.MaxValue));
            if (v == 0)
            {
                expConfig.WriteString("Exp", "Level" + i, OldNeedExps[i].ToString());
                dwNeedExps[i] = OldNeedExps[i];
            }
            else
            {
                dwNeedExps[i] = (uint)v;
            }
        }
        for (int i = 1; i <= 1000; i++)
        {
            string s = expConfig.ReadString("HeroExp", "Level" + i, "");
            long v = GXX.Core.Rtl.DelphiRTL.StrToInt64Def(s, 0);
            v = Math.Max(0, Math.Min(v, int.MaxValue));
            if (v == 0)
            {
                expConfig.WriteString("HeroExp", "Level" + i, OldHeroNeedExps[i].ToString());
                dwHeroNeedExps[i] = OldHeroNeedExps[i];
            }
            else
            {
                dwHeroNeedExps[i] = (uint)v;
            }
        }
        for (int i = 1; i <= 1000; i++)
        {
            string s = expConfig.ReadString("GamePetExp", "Level" + i, "");
            long v = GXX.Core.Rtl.DelphiRTL.StrToInt64Def(s, 0);
            v = Math.Max(0, Math.Min(v, int.MaxValue));
            if (v == 0)
            {
                // Delphi 原文误写英雄表：ExpConfig.WriteString('GamePetExp', ..., g_dwOldHeroNeedExps[i])
                expConfig.WriteString("GamePetExp", "Level" + i, OldHeroNeedExps[i].ToString());
                dwPetNeedExps[i] = OldHeroNeedExps[i];
            }
            else
            {
                dwPetNeedExps[i] = (uint)v;
            }
        }
        expConfig.UpdateFile();
    }

    public static void ResetServerValueDefaults()
    {
        nSendBlock = 8000;
        nCheckBlock = 20000;
        nAvailableBlock = 8000;
        nGateLoad = 0;
        boViewHackMessage = false;
        boViewAdmissionFailure = false;
        nMonGenRate = 10;
        dwProcessMonstersTime = 250;
        dwRegenMonstersTime = 200;
        nProcessMonsterInterval = 2;
        boSendCompressDataToRunGate = false;
        boIsOldClient = true;
        btMaxLevel = 0;
                btMaxAC = 0;
                btMaxHitPoint = 1;
        nLevelValueOfTaosHP = 6;
        nLevelValueOfTaosHPRate = 2.5;
        nLevelValueOfTaosMP = 8;
        nLevelValueOfWizardHP = 15;
        nLevelValueOfWizardHPRate = 1.8;
        nLevelValueOfWarrHP = 4;
        nLevelValueOfWarrHPRate = 4.5;
        OldNeedExps.AsSpan().CopyTo(dwNeedExps);
        OldHeroNeedExps.AsSpan().CopyTo(dwHeroNeedExps);
        OldHeroNeedExps.AsSpan().CopyTo(dwPetNeedExps);
    }
}
