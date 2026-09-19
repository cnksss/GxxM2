namespace GXX.M2Server.Engine;

/// <summary>Delphi TPlayMonsterConfig（人形怪/自定义怪物个体配置，批次J37）。</summary>
public class TPlayMonsterConfig
{
    public string Name = "";
    public int Job;
    public int Gender;
    public int Hair;
    public bool boDieDropUseItem;
    public int nDieDropUseItemRate;
    public bool boDieDropBagItem;
    public bool boButchUseItem;
    public int nButchUseItemRate;
    public bool boButchListItem;
    public bool boButchItemTrigger;
    public int nButchChargeMode;
    public int nButchChargeCount;
    public bool boOnlyButchItemDelGold;
    public bool boProtectMode;
    public int nProtectRange;
    public bool NonUseSpellPoint;
    public bool boRunWithAttack;
    public int nRunWithAttackRate;
    public bool boNoAttackMode;
    /// <summary>装备槽名（下标 = U_DRESS..U_SHIELD 等，见 RecalcChain 槽位常量）。</summary>
    public string[] UseItems = new string[17];
    public List<string> Magics = new();
    public bool IsChanged;
}

public static partial class M2Config
{
    /// <summary>人形怪个体配置注册表（Delphi GetPlayMonsterConfig 按名字查找等效）。</summary>
    public static readonly System.Collections.Generic.Dictionary<string, TPlayMonsterConfig> PlayMonsterConfigs = new();

    /// <summary>已保存过的人形怪配置名（SavePlayMonsterConfigList 接缝计数/记录）。</summary>
    public static readonly System.Collections.Generic.List<string> SavedPlayMonsterNames = new();

    /// <summary>测试隔离：清空人形怪注册表与保存记录。</summary>
    public static void ResetMonsterSlice2Defaults()
    {
        PlayMonsterConfigs.Clear();
        SavedPlayMonsterNames.Clear();
    }

    /// <summary>Delphi GetPlayMonsterConfig：按名字取配置，不存在返回 null。</summary>
    public static TPlayMonsterConfig? GetPlayMonsterConfig(string name)
        => PlayMonsterConfigs.TryGetValue(name, out var cfg) ? cfg : null;

    /// <summary>Delphi SavePlayMonsterConfigList 接缝（真实落盘随引擎批次接入）。</summary>
    public static void SavePlayMonsterConfig(TPlayMonsterConfig cfg)
        => SavedPlayMonsterNames.Add(cfg.Name);
}
