using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J60：uCustomHeroMagic.pas TCustomHeroMagicMgr 1:1 测试。
/// 枚举/名称表、AddDefMagic 新建与已存在两分支、默认三职业表逐条、
/// FindMagic/Remove/越界、LoadFromFile（含自定义条件段）/SaveToFile（含原文重复键瑕疵）。
/// </summary>
public sealed class CustomHeroMagicTests : IDisposable
{
    private readonly string _dir;
    private readonly TCustomHeroMagicMgr _mgr;

    public CustomHeroMagicTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j60_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        _mgr = new TCustomHeroMagicMgr();
        _mgr.FindHeroMagicHandler = _ => true;                 // UserEngine.FindHeroMagic(id, mtHero) 命中
        _mgr.FindHeroMagicAnyHandler = _ => true;              // 自定义技能命中
        _mgr.CheckIsCustomMagicHandler = id => id >= 1000;
    }

    public void Dispose()
    {
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    [Fact]
    public void NameTables_MatchDelphiConstants()
    {
        Assert.Equal(new[] { "敌人", "自己", "主人", "伙伴" }, CustomHeroMagicState.MagicAttackTargetNames);
        Assert.Equal(new[] { "<", "<=", "=", ">", ">=" }, CustomHeroMagicState.CompareSymbolNames);
        Assert.Equal(new[] { "目标等级", "固定等级" }, CustomHeroMagicState.HeroLevelCompareTypeNames);
        Assert.Equal(new[] { "固定值", "百分比" }, CustomHeroMagicState.HeroHPCompareTypeNames);

        // 枚举顺序（INI 整数取值语义）
        Assert.Equal(0, (int)THeroMagicType.mtWarrAttack);
        Assert.Equal(2, (int)THeroMagicType.mtTaosAttack);
        Assert.Equal(0, (int)THeroMagicAttackTarget.matEnemy);
        Assert.Equal(3, (int)THeroMagicAttackTarget.matPartner);
        Assert.Equal(4, (int)TCompareSymbol.csGreaterorEqual);
        Assert.Equal(1, (int)THeroLevelCompareType.hlctLevelNumber);
        Assert.Equal(1, (int)THeroHPCompareType.hhpctPercentage);
    }

    [Fact]
    public void AddDefMagic_CreatesWhenFound_AndSetsChecked()
    {
        var m = _mgr.AddDefMagic(THeroMagicType.mtWarrAttack, 7, 10);
        Assert.NotNull(m);
        Assert.Equal(1, _mgr.Count);
        Assert.True(m!.Checked);
        Assert.False(m.IsChanged);
        Assert.Equal(THeroMagicType.mtWarrAttack, m.MagicType);
        Assert.Equal(7, m.MagicID);
        Assert.False(m.IsCustomMagic);
        Assert.Equal(10, m.UseRate);
        Assert.Equal(1, m.AttackRange);       // 默认参数
        Assert.Equal(THeroMagicAttackTarget.matEnemy, m.AttackTarget);
    }

    [Fact]
    public void AddDefMagic_SkipsWhenFindHeroMagicMisses()
    {
        _mgr.FindHeroMagicHandler = _ => false;
        var m = _mgr.AddDefMagic(THeroMagicType.mtWarrAttack, 7, 10);
        Assert.Null(m);
        Assert.Equal(0, _mgr.Count);
    }

    [Fact]
    public void AddDefMagic_ExistingEntry_UpdatesRangeTargetKeepsUseRate()
    {
        var first = _mgr.AddDefMagic(THeroMagicType.mtWizardAttack, 75, 3, 0, THeroMagicAttackTarget.matSelf);
        Assert.NotNull(first);
        first!.IsCustomMagic = true;
        first.IsChanged = true;

        var again = _mgr.AddDefMagic(THeroMagicType.mtWizardAttack, 75, 99, 5, THeroMagicAttackTarget.matMaster);
        Assert.Same(first, again);
        Assert.Equal(1, _mgr.Count);
        Assert.False(again!.IsChanged);                     // 原文清 IsChanged
        Assert.False(again.IsCustomMagic);                  // 原文清 IsCustomMagic
        Assert.Equal(5, again.AttackRange);                 // 覆盖
        Assert.Equal(THeroMagicAttackTarget.matMaster, again.AttackTarget);
        Assert.Equal(3, again.UseRate);                     // 已存在分支不写 UseRate
    }

    [Fact]
    public void LoadDefaultMagics_MatchPerJobTables()
    {
        _mgr.LoadDefaultMagics();

        // 战士 18 条 / 法师 27 条 / 道士 24 条 = 69（逐条点数 uCustomHeroMagic.pas 350-422）
        int warr = _mgr.All().Count(m => m.MagicType == THeroMagicType.mtWarrAttack);
        int wiz = _mgr.All().Count(m => m.MagicType == THeroMagicType.mtWizardAttack);
        int taos = _mgr.All().Count(m => m.MagicType == THeroMagicType.mtTaosAttack);
        Assert.Equal(18, warr);
        Assert.Equal(27, wiz);
        Assert.Equal(24, taos);
        Assert.Equal(69, _mgr.Count);

        // 抽条校验（原文 350-422 逐条）
        var whirl = _mgr.FindMagic(THeroMagicType.mtWarrAttack, 208)!;
        Assert.Equal(10, whirl.AttackRange);
        Assert.Equal(0, whirl.UseRate);

        var shield = _mgr.FindMagic(THeroMagicType.mtWarrAttack, 75)!;
        Assert.Equal(0, shield.AttackRange);
        Assert.Equal(THeroMagicAttackTarget.matSelf, shield.AttackTarget);

        var mote = _mgr.FindMagic(THeroMagicType.mtWarrAttack, 27)!;
        Assert.Equal(4, mote.UseRate);
        Assert.Equal(2, mote.AttackRange);

        // 法师 1（火球术）：AddDefMagic(mtWizardAttack, 1, 0) → AttackRange 取默认参数 1
        var fireball = _mgr.FindMagic(THeroMagicType.mtWizardAttack, 1)!;
        Assert.Equal(0, fireball.UseRate);
        Assert.Equal(1, fireball.AttackRange);

        // 显式传 0 的条目（74 分身术 / 31 魔法盾）；114 倚天辟地未传 → 默认 1
        Assert.Equal(0, _mgr.FindMagic(THeroMagicType.mtWizardAttack, 74)!.AttackRange);
        Assert.Equal(0, _mgr.FindMagic(THeroMagicType.mtWizardAttack, 31)!.AttackRange);
        Assert.Equal(0, _mgr.FindMagic(THeroMagicType.mtWizardAttack, 74)!.UseRate);
        Assert.Equal(1, _mgr.FindMagic(THeroMagicType.mtWizardAttack, 114)!.AttackRange);

        var tamming = _mgr.FindMagic(THeroMagicType.mtWizardAttack, 20)!;
        Assert.Equal(7, tamming.UseRate);

        var bigCloak = _mgr.FindMagic(THeroMagicType.mtTaosAttack, 19)!;
        Assert.Equal(10, bigCloak.UseRate);
        Assert.Equal(0, bigCloak.AttackRange);
        Assert.Equal(THeroMagicAttackTarget.matPartner, bigCloak.AttackTarget);

        var dejiwonho = _mgr.FindMagic(THeroMagicType.mtTaosAttack, 15)!;
        Assert.Equal(THeroMagicAttackTarget.matMaster, dejiwonho.AttackTarget);

        // 同 ID 不同职业互不干扰
        Assert.NotNull(_mgr.FindMagic(THeroMagicType.mtWarrAttack, 114));
        Assert.NotNull(_mgr.FindMagic(THeroMagicType.mtTaosAttack, 114));
        // 职业维度隔离：208（旋风转）只登记在战士组，法师/道士组查不到；114 三职业各自独立
        Assert.Null(_mgr.FindMagic(THeroMagicType.mtWizardAttack, 208));
        Assert.Null(_mgr.FindMagic(THeroMagicType.mtTaosAttack, 208));
    }

    [Fact]
    public void AddRemove_IndexerOutOfRange()
    {
        var a = _mgr.Add();
        _mgr.Add();
        Assert.Equal(2, _mgr.Count);
        Assert.Null(_mgr[-1]);
        Assert.Same(a, _mgr[0]);
        Assert.Null(_mgr[2]);

        Assert.True(_mgr.Remove(a));
        Assert.False(_mgr.Remove(a));
        Assert.Equal(1, _mgr.Count);

        _mgr.Clear();
        Assert.Equal(0, _mgr.Count);
        Assert.Null(_mgr[0]);
    }

    [Fact]
    public void LoadFromFile_ReadsCustomEntryWithConditions()
    {
        File.WriteAllText(Path.Combine(_dir, "CustomHeroMagic.ini"),
            "[Setup]" + Environment.NewLine + "MagicCount=2" + Environment.NewLine
            + "[HeroMagic1]" + Environment.NewLine
            + "Checked=1" + Environment.NewLine + "MagicID=1001" + Environment.NewLine
            + "MagicType=1" + Environment.NewLine + "UseRate=6" + Environment.NewLine
            + "AttackRange=9" + Environment.NewLine + "AttackTarget=2" + Environment.NewLine
            + "LevelChecked=-1" + Environment.NewLine + "LevelCheckSymbol=4" + Environment.NewLine
            + "LevelCheckType=1" + Environment.NewLine + "LevelCheckValue=55" + Environment.NewLine
            + "HeroHPChecked=1" + Environment.NewLine + "HeroHPCheckSymbol=1" + Environment.NewLine
            + "HeroHPCheckType=1" + Environment.NewLine + "HeroHPCheckValue=80" + Environment.NewLine
            + "Poisoning=1" + Environment.NewLine + "UnFrozen=1" + Environment.NewLine
            + "FriendCountChecked=1" + Environment.NewLine + "FriendCountCheckRange=6" + Environment.NewLine
            + "FriendCountCheckValue=3" + Environment.NewLine
            + "StraightLineChecked=1" + Environment.NewLine
            + "[HeroMagic2]" + Environment.NewLine
            + "Checked=0" + Environment.NewLine + "MagicID=8" + Environment.NewLine
            + "MagicType=1" + Environment.NewLine + "UseRate=2" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        _mgr.LoadFromFile();

        // 自定义条目（MagicID 1001 >= CUSTOM_MAGIC_START_ID）
        var custom = _mgr.FindMagic(THeroMagicType.mtWizardAttack, 1001)!;
        Assert.True(custom.IsCustomMagic);
        Assert.True(custom.Checked);
        Assert.Equal(6, custom.UseRate);
        Assert.Equal(9, custom.AttackRange);
        Assert.Equal(THeroMagicAttackTarget.matMaster, custom.AttackTarget);
        Assert.True(custom.Condition.HeroLevelCheck.boChecked);
        Assert.Equal(TCompareSymbol.csGreaterorEqual, custom.Condition.HeroLevelCheck.CompareSymbol);
        Assert.Equal(THeroLevelCompareType.hlctLevelNumber, custom.Condition.HeroLevelCheck.CompareType);
        Assert.Equal(55u, custom.Condition.HeroLevelCheck.CompareValue);
        Assert.Equal(80u, custom.Condition.HeroHPCheck.CompareValue);
        Assert.True(custom.Condition.TargetStatusCheck.boPoisoning);
        Assert.True(custom.Condition.TargetStatusCheck.boUnFrozen);
        Assert.True(custom.Condition.FriendCountCheck.boChecked);
        Assert.Equal(6, custom.Condition.FriendCountCheck.nCheckRange);
        Assert.Equal(3, custom.Condition.FriendCountCheck.nCheckValue);
        Assert.True(custom.Condition.boStraightLineCheck);   // INI 键名为 StraightLineChecked

        // 内置条目：不读条件段
        var builtin = _mgr.FindMagic(THeroMagicType.mtWizardAttack, 8)!;
        Assert.False(builtin.IsCustomMagic);
        Assert.False(builtin.Checked);

        // 尾部默认表已追加：69 条默认 + 自定义 1001（法师，无 ID 冲突）= 70；
        // 自定义 8（法师）与默认「抗拒火环」同 type 同 ID → 被 AddDefMagic 已存在分支合并（不新增）
        Assert.Equal(69 + 1, _mgr.Count);
    }

    [Fact]
    public void LoadFromFile_SkipsUnfoundAndZeroIds()
    {
        _mgr.FindHeroMagicAnyHandler = _ => false;
        File.WriteAllText(Path.Combine(_dir, "CustomHeroMagic.ini"),
            "[Setup]" + Environment.NewLine + "MagicCount=3" + Environment.NewLine
            + "[HeroMagic1]" + Environment.NewLine + "MagicID=0" + Environment.NewLine
            + "[HeroMagic2]" + Environment.NewLine + "MagicID=1001" + Environment.NewLine
            + "[HeroMagic3]" + Environment.NewLine + "MagicID=1002" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        _mgr.LoadFromFile();
        // MagicID=0 跳过；1001/1002 FindHeroMagicAny 未命中 → 全部跳过，仅默认表
        Assert.Equal(69, _mgr.Count);
        Assert.Null(_mgr.FindMagic(THeroMagicType.mtWarrAttack, 1001));
    }

    [Fact]
    public void SaveToFile_WritesCountAndOnlyCustomConditions()
    {
        var custom = _mgr.Add();
        custom.MagicID = 1005;
        custom.MagicType = THeroMagicType.mtTaosAttack;
        custom.IsCustomMagic = true;
        custom.Checked = true;
        custom.UseRate = 4;
        custom.AttackRange = 7;
        custom.AttackTarget = THeroMagicAttackTarget.matPartner;
        var cond = custom.Condition;
        cond.HeroLevelCheck.boChecked = true;
        cond.HeroLevelCheck.CompareSymbol = TCompareSymbol.csEqual;
        cond.HeroLevelCheck.CompareType = THeroLevelCompareType.hlctTargetLevel;
        cond.HeroLevelCheck.CompareValue = 42;
        cond.TargetHPCheck.CompareType = THeroHPCompareType.hhpctPercentage;
        cond.TargetHPCheck.CompareValue = 66;
        cond.boStraightLineCheck = true;
        custom.Condition = cond;
        custom.IsChanged = true;

        var builtin = _mgr.Add();
        builtin.MagicID = 9;
        builtin.MagicType = THeroMagicType.mtWizardAttack;
        builtin.IsCustomMagic = false;
        builtin.UseRate = 3;

        _mgr.SaveToFile();

        Assert.False(custom.IsChanged);   // SaveToFile 清 IsChanged

        string text = File.ReadAllText(Path.Combine(_dir, "CustomHeroMagic.ini"), GXX.Core.EncodingInit.GBK);
        Assert.Contains("[Setup]", text);
        Assert.Contains("MagicCount=2", text);
        Assert.Contains("[HeroMagic1]", text);
        Assert.Contains("MagicID=1005", text);
        Assert.Contains("MagicType=2", text);
        Assert.Contains("UseRate=4", text);
        Assert.Contains("AttackRange=7", text);
        Assert.Contains("AttackTarget=3", text);
        Assert.Contains("LevelChecked=1", text);
        Assert.Contains("LevelCheckSymbol=2", text);
        Assert.Contains("LevelCheckValue=42", text);
        Assert.Contains("StraightLineChecked=1", text);
        // 原文瑕疵：TargetHPCheckType 出现两次（第二次写 CompareValue 66）
        Assert.Contains("TargetHPCheckType=1", text);
        Assert.Contains("TargetHPCheckType=66", text);

        // 内置条目（IsCustomMagic=False）不写条件段
        Assert.DoesNotContain("[HeroMagic2]" + Environment.NewLine + "Checked=0" + Environment.NewLine
            + "MagicID=9" + Environment.NewLine + "MagicType=1" + Environment.NewLine
            + "UseRate=3" + Environment.NewLine + "AttackRange", text);
        Assert.Contains("[HeroMagic2]", text);
    }

    [Fact]
    public void SaveLoad_RoundTripKeepsCustomConditions()
    {
        var custom = _mgr.Add();
        custom.MagicID = 1009;
        custom.MagicType = THeroMagicType.mtWizardAttack;
        custom.IsCustomMagic = true;
        custom.Checked = true;
        custom.UseRate = 8;
        custom.AttackRange = 3;
        custom.AttackTarget = THeroMagicAttackTarget.matSelf;
        var cond = custom.Condition;
        cond.EnemyCountCheck.boChecked = true;
        cond.EnemyCountCheck.nCheckRange = 5;
        cond.EnemyCountCheck.nCheckValue = 2;
        cond.TargetStatusCheck.boCobwebWinding = true;
        custom.Condition = cond;

        _mgr.SaveToFile();

        var reloaded = new TCustomHeroMagicMgr
        {
            FindHeroMagicHandler = _ => true,
            FindHeroMagicAnyHandler = _ => true,
            CheckIsCustomMagicHandler = id => id >= 1000,
        };
        reloaded.LoadFromFile();

        var r = reloaded.FindMagic(THeroMagicType.mtWizardAttack, 1009)!;
        Assert.True(r.IsCustomMagic);
        Assert.True(r.Checked);
        Assert.Equal(8, r.UseRate);
        Assert.Equal(3, r.AttackRange);
        Assert.Equal(THeroMagicAttackTarget.matSelf, r.AttackTarget);
        Assert.True(r.Condition.EnemyCountCheck.boChecked);
        Assert.Equal(5, r.Condition.EnemyCountCheck.nCheckRange);
        Assert.Equal(2, r.Condition.EnemyCountCheck.nCheckValue);
        Assert.True(r.Condition.TargetStatusCheck.boCobwebWinding);
        // 自定义 1009（法师）与默认表无冲突 → 69 + 1
        Assert.Equal(70, reloaded.Count);
    }

    [Fact]
    public void DefaultIsCustomMagic_UsesCustomMagicStartId()
    {
        var mgr = new TCustomHeroMagicMgr { FindHeroMagicHandler = _ => true, FindHeroMagicAnyHandler = _ => true };
        // 未注入 CheckIsCustomMagicHandler → 按 CUSTOM_MAGIC_START_ID(1000) 判定
        File.WriteAllText(Path.Combine(_dir, "CustomHeroMagic.ini"),
            "[Setup]" + Environment.NewLine + "MagicCount=2" + Environment.NewLine
            + "[HeroMagic1]" + Environment.NewLine + "MagicID=999" + Environment.NewLine
            + "[HeroMagic2]" + Environment.NewLine + "MagicID=1000" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);
        mgr.LoadFromFile();
        Assert.False(mgr.FindMagic(THeroMagicType.mtWarrAttack, 999)!.IsCustomMagic);
        Assert.True(mgr.FindMagic(THeroMagicType.mtWarrAttack, 1000)!.IsCustomMagic);
    }
}
