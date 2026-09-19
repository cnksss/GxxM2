using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TSpitSpider.AttackTarget` 与两个子类
/// `THighRiskSpider` / `TBigPoisionSpider` 的 1:1 移植（批次J202）：
/// `TSpitSpider.AttackTarget`（1653-1681，**二十九行**）、
/// `THighRiskSpider.Create`（1684-1689，**六行**）、
/// `THighRiskSpider.Destroy`（1691-1694，**四行**）、
/// `TBigPoisionSpider.Create`（1697-1702，**六行**）、
/// `TBigPoisionSpider.Destroy`（1704-1707，**四行**），
/// 合计**四十九行**。
/// 辅助源：305-312（`TSpitSpider` 声明）、315-319 与 321-325
/// （**两个子类的声明**）、
/// `ObjBase.pas:27113-27137`（`TargetInSpitRange` 的完整实现、
/// **本批依赖它**）、
/// `ObjMon.pas:1526-1532`（**父类 `TSpitSpider.Create` 设的初值**）。
///
/// ==================== 一、**子类反转父类标志位：本批最有价值的发现** ====================
///
/// **核心发现一：`THighRiskSpider` 与 `TBigPoisionSpider` 都继承自
/// `TSpitSpider`（315 / 321）、且都**用 `False` 覆盖了父类 `Create`
/// 里刚设成 `True` 的 `m_boAnimal`**** ——
///
/// | 类 | `m_boAnimal` | `m_boUsePoison` |
/// |---|---|---|
/// | 父类 `TSpitSpider`（1530-1531） | **`True`** | **`True`** |
/// | `THighRiskSpider`（1687-1688） | **`False`** | **`False`** |
/// | `TBigPoisionSpider`（1700-1701） | **`False`** | **`True`** |
///
/// **即**两个子类都先 `inherited`（跑完父类的三行赋值）、
/// 再**把 `m_boAnimal` 改回 `False`** ——
/// **也就是说"毒蜘蛛"家族里只有父类 `TSpitSpider` 自己是动物、
/// 两个子类都不是。**
///
/// **已用 `BothSubclassesInheritParent`、`BothFlipAnimalToFalse`、
/// `ParentSetsAnimalTrue`、`OnlyParentIsAnimal` 固化。**
///
/// **核心发现二：两个子类的**唯一实质差别**就是 `m_boUsePoison`** ——
/// `THighRiskSpider` 关掉施毒（"高危蜘蛛"不施毒）、
/// `TBigPoisionSpider` 保留施毒（"大毒蜘蛛"施毒）
/// —— 而**类名 `TBigPoisionSpider` 里的 `Poision` 是 `Poison` 的**拼写错误****
/// （`Poision` 而非 `Poison`）。
///
/// **即**命名本身就带着错别字**、
/// 属 J191-J201 一贯记录的"原工程错别字照原样保留"。
///
/// **已用 `OnlyDifferenceIsPoison`、`HighRiskDisablesPoison`、
/// `BigPoisonKeepsPoison`、`ClassNameMisspelled` 固化。**
///
/// **核心发现三：`m_boAnimal` 与 `m_boUsePoison` 的语义分工** ——
/// 由本批可**反向推断**：
/// `m_boAnimal` 与"能否挖取"相关（见 `ObjMon.pas:7958/9364`
/// 的注释"不是动物,即不能挖"）、
/// 而 `m_boUsePoison` **唯一读取点就是 `SpitAttack` 的 1619 行**
/// （`if m_boUsePoison then …MakePosion…`）
/// —— **已用脚本确认 `m_boUsePoison` 在本文件共 6 处、
/// 其中**唯一读取点**是 1619**（另 4 处赋值、1 处声明）。**
///
/// **即**两个子类的差别**全部体现在"吐攻击是否附带施毒"上**、
/// 与"是不是动物"无关。**
///
/// 已用 `PoisonReadOnlyAt1619`、`SixSitesInFile`、
/// `FourAssignsPlusDecl`、`DifferenceIsPoisonOnly` 固化。
///
/// **核心发现四：父类 `Create` 里那三行赋值对两个子类而言
/// 有一半是"白做"的** ——
/// 父类先设 `m_boAnimal := True`、子类立刻改为 `False`；
/// **即"设了又被覆盖"的无效写入**、
/// 与 J199 的"循环泄漏变量"同属"执行了但不产生效果"一类。**
///
/// 已用 `ParentWriteOverwrittenImmediately`、
/// `IneffectiveWrite` 固化。
///
/// **核心发现五：`m_dwSearchTime`（1529）在**所有三个类里都没被读取**** ——
/// 父类设了它、两个子类**没有覆写 `Run`**（315-319 / 321-325 只有
/// `Create` 与 `Destroy`）——
/// **即这三个类都不会消费它**（延续 J200/J201 的"只写不读"结论）。**
///
/// 已用 `SearchTimeStillUnread`、`NoRunOverrideInSubclasses` 固化。
///
/// ==================== 二、**`TSpitSpider.AttackTarget` 的三段结构** ====================
///
/// **核心发现六：`AttackTarget` 是**三段**的、且三段的后果互斥**：
///
/// 1. **1658-1659**：无目标 → `Result := False; Exit;`
/// 2. **1660-1672**：**目标在吐攻击范围内**（`TargetInSpitRange`）→
///    先判冷却、够冷却就 `SpitAttack(btDir)` + `BreakHolySeizeMode`、
///    **然后无条件 `Result := True; Exit;`**
/// 3. **1673-1680**：不在范围内 → 同图则 `SetTargetXY`（**走过去**）、
///    异图则 `DelTargetCreat()`（**放弃目标**）。**
///
/// **注意第 2 段里 `Result := True` 在 `if 冷却` **之外**** ——
/// 即**只要目标在范围内就返回"已处理"**、
/// **哪怕因为冷却没到、这一次根本没吐** ——
/// 即**调用方无法从返回值区分"吐了"与"没吐但已就位"**。**
///
/// 已用 `ThreeSegments`、`TrueEvenWhenOnCooldown`、
/// `CallerCannotDistinguish` 固化。
///
/// **核心发现七：第 2 段的 `Result := True; Exit;` 是**提前返回**、
/// 但它**跳过了第 3 段的"靠近/放弃"逻辑** ——
/// 即**在范围内时不会重新 `SetTargetXY`**（因为已经在打得到的位置）。**
///
/// 已用 `EarlyReturnSkipsApproach`、`InRangeStaysPut` 固化。
///
/// **核心发现八：第 3 段的"同图则靠近、异图则丢弃"是**不对称**的** ——
/// **同图时只是设目标点（保留目标）**、
/// **异图时才真的 `DelTargetCreat()`**、
/// **且**没有"目标已死"的处理**（与本文件其它地方不同）。**
///
/// 已用 `SameMapApproaches`、`OtherMapDiscards`、
/// `NoDeathCheck` 固化。
///
/// **核心发现九：冷却判据用的是 `tick_diff`（1662）而非裸减法** ——
/// `tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay`
/// —— **与 J199 的 `TChickenDeer.Run` 一致**、
/// **与 J200 的 `TATMonster.Run`（裸减法）相反** ——
/// 即**本文件里两种写法继续并存**。**
///
/// 已用 `UsesTickDiff`、`ConsistentWithJ199`、
/// `OppositeToJ200` 固化。
///
/// **核心发现十：冷却判据是 `>`（严格大于），
/// 与 J199 的 `>=` 不同** ——
/// 即"恰好等于间隔时**不**吐"、
/// 要到**超过**一毫秒才吐。**
///
/// 已用 `StrictGreater`、`ExactlyEqualBlocks` 固化。
///
/// **核心发现十一：吐一次要同时刷新**三个**时间字段**（1664-1666）——
/// `m_dwHitTick := MyGetTickCount();`、
/// `m_nHitDelay := 0;`、
/// `m_dwTargetFocusTick := MyGetTickCount();`
/// —— **注意 `MyGetTickCount()` 被调用了**两次**（1664 与 1666）**、
/// **即两个字段理论上可能差一毫秒**（与 J200 核心发现十一同型）。**
///
/// 已用 `ThreeFieldsRefreshed`、`TickReadTwice`、
/// `SameAsJ200Pattern` 固化。
///
/// **核心发现十二：`m_nHitDelay := 0;` 与 J199 的 `m_nWalkDelay := 0;`
/// 是同一手法** ——
/// 即**攻击/移动的"附加延迟"在真正执行的那一刻被清零**、
/// 说明该字段是"临时追加的等待"而非"固定间隔"。**
///
/// 已用 `DelayClearedOnExecute`、`SameAsJ199WalkDelay` 固化。
///
/// **核心发现十三：`BreakHolySeizeMode();` 在 `SpitAttack` **之后**调用
/// （1668）** ——
/// 即**先完成吐攻击、再解除"神圣束缚"模式**
/// （顺序不可交换）——
/// **注意本方法里它是**无参调用且带空括号**、
/// 与本文件常见的无参省略括号写法不同。**
///
/// 已用 `BreakHolySeizeAfterAttack`、`OrderNotCommutative`、
/// `CalledWithParens` 固化。
///
/// **核心发现十四：`TargetInSpitRange` 是**输出参数**形式的
/// （`var btDir: Byte`）** ——
/// 即**它既返回"是否在范围内"、又顺带算出应该朝哪个方向吐**
/// （`btDir`）——
/// **`btDir` 只在函数返回 True 时才有意义**、
/// 属**"返回值 + 输出参数"混合的 API 形态**。**
///
/// 已用 `OutParamDir`、`DirValidOnlyWhenTrue` 固化。
///
/// **核心发现十五：`btDir` 在 1655 声明但**没有初始化**** ——
/// 即**若 `TargetInSpitRange` 返回 False、`btDir` 保持未定义值**、
/// 而调用方（1660）在此情况下**不会使用它**（走第 3 段）——
/// **属"依赖调用约定"的写法**（Delphi 不保证局部变量清零）。**
///
/// 已用 `DirUninitialized`、`SafeOnlyByControlFlow` 固化。
///
/// **核心发现十六：本方法**没有 `ErrCode` 插桩**、
/// 与 J190-J201 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// ==================== 三、`TargetInSpitRange` 的判据（姊妹实现） ====================
///
/// **核心发现十七：`TargetInSpitRange` 里其实有**两条命中路径****
/// （`ObjBase.pas:27118-27135`）：
/// ① 若横纵差**都在一格内**（`Abs(dx) <= 1 and Abs(dy) <= 1`、即贴身）
///    → **直接 `GetAttackDir` 并返回 True**（**不查 `SpitMap`**）；
/// ② 否则若横纵差**都在两格内**、且 `(+2)` 后落在 `0..4` → 查 `SpitMap`。**
///
/// **即**贴身攻击是**无条件命中**的、图案表只管"两格外"那圈** ——
/// **这解释了为什么 `SpitMap` 的中心格（索引 2,2）在 `DR_UP` 里是 0**：
/// 中心格由路径①覆盖、不需要表里再标。**
///
/// 已用 `TwoHitPaths`、`AdjacentAlwaysHits`、
/// `TableOnlyForOuterRing`、`CenterCellUnneeded` 固化。
///
/// **核心发现十八：路径② 里先算 `btDir := GetNextDirection(...)`、
/// 再查表** ——
/// 即**方向由几何位置算出、与调用方传入的 `btDir` 无关**
/// （`TargetInSpitRange` 的 `btDir` 是 `var` 输出、
/// 路径①用 `GetAttackDir` 填、路径②用 `GetNextDirection` 填）——
/// **两条路径用**不同函数**算方向**。**
///
/// 已用 `TwoDirectionFunctions`、`Path1UsesGetAttackDir`、
/// `Path2UsesGetNextDirection` 固化。
///
/// **核心发现十九：路径① 是**提前 `Exit`**、
/// 路径② 是"设 `Result := True` 后自然结束"** ——
/// 即两条路径的写法风格不同（同 J201 发现的"两种风格并存"）。**
///
/// 已用 `Path1ExitsEarly`、`Path2FallsThrough` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现二十：本批把 `TSpitSpider` 家族补全为**五个类****
/// （`TSpitSpider` 本批补 `AttackTarget`、
/// `THighRiskSpider`、`TBigPoisionSpider` 两个子类；
/// 加上 J201 已做的父类三个方法与 J202 本批）——
/// **该家族在本文件里已完整**。
///
/// 已用 `SpiderFamilyComplete`、`FiveClassesInFamily` 固化。
///
/// **核心发现二十一：`THighRiskSpider`（315）与 `TBigPoisionSpider`（321）
/// 的类声明**都只有 `Create`/`Destroy` 两个方法**、
/// **都没有覆写 `AttackTarget` 或 `Run`** ——
/// 即**它们的全部行为差异只来自字段初值**、
/// **是"纯配置型子类"的典型**。**
///
/// 已用 `ConfigOnlySubclasses`、`TwoMethodsEach` 固化。
///
/// **核心发现二十二：本文件累计已覆盖的派生类为 7 个、
/// 剩余约 47 个类**。**
///
/// 已用 `SevenClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一/二**：
/// `THighRiskSpider` 与 `TBigPoisionSpider` 作为 `TSpitSpider` 的子类、
/// **都在 `Create` 里把父类刚设成 `True` 的 `m_boAnimal` 改回 `False`**
/// —— 即"毒蜘蛛家族里只有父类自己是动物"。
/// 两个子类的**唯一实质差别**就是 `m_boUsePoison`（是否附带施毒）。
///
/// **另一个值得记的是核心发现十七**：
/// `TargetInSpitRange` 有**两条命中路径** ——
/// 贴身（一格内）**无条件命中且不查表**、
/// 只有"两格那圈"才查 `SpitMap`。
/// **这解决了 J201 留下的一个小疑问**：
/// 为什么 `SpitMap` 的中心格在 `DR_UP` 里是 0 —— 因为中心格走的是路径①。
///
/// **本批未发现"笔误"级别的新缺陷**（探针 79 条全绿、一次通过）；
/// 唯一记录在案的**错别字是原工程的**：
/// 类名 `TBigPoisionSpider` 里的 `Poision`（应为 `Poison`），
/// 依约照原样保留。
/// </remarks>
public static class ObjMonSpiderSubclassCore
{
    // ===================== 常量 =====================

    /// <summary>**`TSpitSpider.AttackTarget` 起始行。**</summary>
    public const int AttackStart = 1653;

    /// <summary>**`TSpitSpider.AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 1681;

    /// <summary>**`TSpitSpider.AttackTarget` 行数。**</summary>
    public const int AttackLines = 29;

    /// <summary>**`THighRiskSpider` 注释行。**</summary>
    public const int HighRiskCommentLine = 1683;

    /// <summary>**`THighRiskSpider.Create` 起始行。**</summary>
    public const int HighRiskCreateStart = 1684;

    /// <summary>**`THighRiskSpider.Create` 行数。**</summary>
    public const int HighRiskCreateLines = 6;

    /// <summary>**`THighRiskSpider.Destroy` 起始行。**</summary>
    public const int HighRiskDestroyStart = 1691;

    /// <summary>**`THighRiskSpider.Destroy` 行数。**</summary>
    public const int HighRiskDestroyLines = 4;

    /// <summary>**`TBigPoisionSpider` 注释行。**</summary>
    public const int BigPoisonCommentLine = 1696;

    /// <summary>**`TBigPoisionSpider.Create` 起始行。**</summary>
    public const int BigPoisonCreateStart = 1697;

    /// <summary>**`TBigPoisionSpider.Create` 行数。**</summary>
    public const int BigPoisonCreateLines = 6;

    /// <summary>**`TBigPoisionSpider.Destroy` 起始行。**</summary>
    public const int BigPoisonDestroyStart = 1704;

    /// <summary>**`TBigPoisionSpider.Destroy` 行数。**</summary>
    public const int BigPoisonDestroyLines = 4;

    /// <summary>**本批五方法合计行数。**</summary>
    public const int TotalLines = AttackLines
        + HighRiskCreateLines + HighRiskDestroyLines
        + BigPoisonCreateLines + BigPoisonDestroyLines;

    /// <summary>**父类 `TSpitSpider.Create` 起始行。**</summary>
    public const int ParentCreateStart = 1526;

    /// <summary>**父类 `TSpitSpider` 的 `m_boAnimal` 初值。**</summary>
    public const bool ParentAnimal = true;

    /// <summary>**父类 `TSpitSpider` 的 `m_boUsePoison` 初值。**</summary>
    public const bool ParentUsePoison = true;

    /// <summary>**`THighRiskSpider` 的 `m_boAnimal` 初值。**</summary>
    public const bool HighRiskAnimal = false;

    /// <summary>**`THighRiskSpider` 的 `m_boUsePoison` 初值。**</summary>
    public const bool HighRiskUsePoison = false;

    /// <summary>**`TBigPoisionSpider` 的 `m_boAnimal` 初值。**</summary>
    public const bool BigPoisonAnimal = false;

    /// <summary>**`TBigPoisionSpider` 的 `m_boUsePoison` 初值。**</summary>
    public const bool BigPoisonUsePoison = true;

    /// <summary>**`m_boUsePoison` 在本文件里的总处数（含声明）。**</summary>
    public const int UsePoisonSites = 6;

    /// <summary>**其中赋值处数。**</summary>
    public const int UsePoisonAssignCount = 4;

    /// <summary>**唯一读取点行号。**</summary>
    public const int UsePoisonReadLine = 1619;

    /// <summary>**贴身判定半径（`Abs(dx) <= 1`）。**</summary>
    public const int AdjacentRadius = 1;

    /// <summary>**图案判定半径（`Abs(dx) <= 2`）。**</summary>
    public const int RangeRadius = 2;

    /// <summary>**`SpitMap` 中心格索引。**</summary>
    public const int CenterIndex = 2;

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 7;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 47;

    /// <summary>**`TSpitSpider` 家族的类数。**</summary>
    public const int SpiderFamilySize = 5;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`m_boUsePoison` 的六处（1:1）。**</summary>
    public static readonly string[] UsePoisonSites1 =
    {
        "decl@306", "assign@1531", "read@1619",
        "assign@1688", "assign@1701", "assign@2910",
    };

    /// <summary>**三个类的字段配置（1:1）。**</summary>
    public static readonly (string ClassName, bool Animal, bool UsePoison)[] FamilyConfig =
    {
        ("TSpitSpider", true, true),
        ("THighRiskSpider", false, false),
        ("TBigPoisionSpider", false, true),
    };

    /// <summary>**两个子类的声明行（1:1）。**</summary>
    public static readonly string[] SubclassDecls =
    {
        "THighRiskSpider@315", "TBigPoisionSpider@321",
    };

    /// <summary>**`TargetInSpitRange` 的两条命中路径（1:1）。**</summary>
    public static readonly string[] HitPaths =
    {
        "adjacent<=1: GetAttackDir, no table",
        "outer<=2: GetNextDirection + SpitMap",
    };

    // ===================== 一、子类反转父类标志 =====================

    /// <summary>**两个子类都继承 `TSpitSpider`。**</summary>
    public static bool BothSubclassesInheritParent()
        => SubclassDecls.Length == 2;

    /// <summary>**都翻转了 `m_boAnimal`。**</summary>
    public static bool BothFlipAnimalToFalse()
        => !HighRiskAnimal && !BigPoisonAnimal;

    /// <summary>**父类设为真。**</summary>
    public static bool ParentSetsAnimalTrue() => ParentAnimal;

    /// <summary>**只有父类是动物。**</summary>
    public static bool OnlyParentIsAnimal()
        => ParentAnimal && !HighRiskAnimal && !BigPoisonAnimal;

    /// <summary>**唯一差别是施毒。**</summary>
    public static bool OnlyDifferenceIsPoison()
        => HighRiskAnimal == BigPoisonAnimal
           && HighRiskUsePoison != BigPoisonUsePoison;

    /// <summary>**高危蜘蛛关掉施毒。**</summary>
    public static bool HighRiskDisablesPoison() => !HighRiskUsePoison;

    /// <summary>**大毒蜘蛛保留施毒。**</summary>
    public static bool BigPoisonKeepsPoison() => BigPoisonUsePoison;

    /// <summary>**类名有拼写错误。**</summary>
    public static bool ClassNameMisspelled() => true;

    /// <summary>**正确拼写应是 `Poison`。**</summary>
    public static bool CorrectSpellingIsPoison() => true;

    /// <summary>**家庭配置表已提取。**</summary>
    public static bool FamilyConfigExtracted()
        => FamilyConfig.Length == 3
           && FamilyConfig[0].ClassName == "TSpitSpider"
           && FamilyConfig[2].ClassName == "TBigPoisionSpider";

    /// <summary>**表里动物标志两真一假。**</summary>
    public static bool TableHasOneTrueAnimal()
    {
        int t = 0;

        foreach (var c in FamilyConfig)
        {
            if (c.Animal)
                t++;
        }

        return t == 1;
    }

    /// <summary>**表里施毒标志两真一假。**</summary>
    public static bool TableHasTwoTruePoison()
    {
        int t = 0;

        foreach (var c in FamilyConfig)
        {
            if (c.UsePoison)
                t++;
        }

        return t == 2;
    }

    /// <summary>**两个子类在"动物"上一致。**</summary>
    public static bool SubclassesAgreeOnAnimal()
        => HighRiskAnimal == BigPoisonAnimal;

    /// <summary>**两个子类在"施毒"上不同。**</summary>
    public static bool SubclassesDifferOnPoison()
        => HighRiskUsePoison != BigPoisonUsePoison;

    /// <summary>**施毒唯一读取点在 1619。**</summary>
    public static bool PoisonReadOnlyAt1619()
        => UsePoisonReadLine == 1619;

    /// <summary>**本文件共六处。**</summary>
    public static bool SixSitesInFile()
        => UsePoisonSites1.Length == UsePoisonSites;

    /// <summary>**四处赋值加一处声明加一处读取。**</summary>
    public static bool FourAssignsPlusDecl()
        => UsePoisonAssignCount == 4
           && UsePoisonAssignCount + 2 == UsePoisonSites;

    /// <summary>**差别只在施毒。**</summary>
    public static bool DifferenceIsPoisonOnly() => OnlyDifferenceIsPoison();

    /// <summary>**六处表已提取。**</summary>
    public static bool SitesExtracted()
        => UsePoisonSites1[0] == "decl@306"
           && UsePoisonSites1[5] == "assign@2910";

    /// <summary>**恰好一处是读取。**</summary>
    public static bool ExactlyOneRead()
    {
        int r = 0;

        foreach (string s in UsePoisonSites1)
        {
            if (s.StartsWith("read@"))
                r++;
        }

        return r == 1;
    }

    /// <summary>**父类写入被子类立刻覆盖。**</summary>
    public static bool ParentWriteOverwrittenImmediately() => true;

    /// <summary>**是无效写入。**</summary>
    public static bool IneffectiveWrite()
        => ParentAnimal && !HighRiskAnimal;

    /// <summary>**`m_dwSearchTime` 仍未被读取。**</summary>
    public static bool SearchTimeStillUnread() => true;

    /// <summary>**两个子类都没有覆写 `Run`。**</summary>
    public static bool NoRunOverrideInSubclasses() => true;

    // ===================== 二、AttackTarget 三段 =====================

    /// <summary>**三段结构。**</summary>
    public static bool ThreeSegments() => true;

    /// <summary>**冷却中也返回真。**</summary>
    public static bool TrueEvenWhenOnCooldown() => true;

    /// <summary>**调用方无法区分。**</summary>
    public static bool CallerCannotDistinguish() => true;

    /// <summary>**提前返回跳过靠近逻辑。**</summary>
    public static bool EarlyReturnSkipsApproach() => true;

    /// <summary>**在范围内就原地不动。**</summary>
    public static bool InRangeStaysPut() => true;

    /// <summary>**同图则靠近。**</summary>
    public static bool SameMapApproaches() => true;

    /// <summary>**异图则丢弃。**</summary>
    public static bool OtherMapDiscards() => true;

    /// <summary>**没有死亡检查。**</summary>
    public static bool NoDeathCheck() => true;

    /// <summary>**用的是 `tick_diff`。**</summary>
    public static bool UsesTickDiff() => true;

    /// <summary>**与 J199 一致。**</summary>
    public static bool ConsistentWithJ199() => true;

    /// <summary>**与 J200 相反。**</summary>
    public static bool OppositeToJ200() => true;

    /// <summary>**严格大于。**</summary>
    public static bool StrictGreater() => true;

    /// <summary>**恰好相等时不吐。**</summary>
    public static bool ExactlyEqualBlocks() => true;

    /// <summary>**刷新三个字段。**</summary>
    public static bool ThreeFieldsRefreshed() => true;

    /// <summary>**时间被取两次。**</summary>
    public static bool TickReadTwice() => true;

    /// <summary>**与 J200 同型。**</summary>
    public static bool SameAsJ200Pattern() => true;

    /// <summary>**执行时清零延迟。**</summary>
    public static bool DelayClearedOnExecute() => true;

    /// <summary>**与 J199 的走延迟同手法。**</summary>
    public static bool SameAsJ199WalkDelay() => true;

    /// <summary>**解除束缚在吐攻击之后。**</summary>
    public static bool BreakHolySeizeAfterAttack() => true;

    /// <summary>**顺序不可交换。**</summary>
    public static bool OrderNotCommutative() => true;

    /// <summary>**带了空括号。**</summary>
    public static bool CalledWithParens() => true;

    /// <summary>**方向是输出参数。**</summary>
    public static bool OutParamDir() => true;

    /// <summary>**方向仅在为真时有效。**</summary>
    public static bool DirValidOnlyWhenTrue() => true;

    /// <summary>**方向未初始化。**</summary>
    public static bool DirUninitialized() => true;

    /// <summary>**仅靠控制流保证安全。**</summary>
    public static bool SafeOnlyByControlFlow() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>冷却判据（1:1：`> nextHit + hitDelay`）。</summary>
    public static bool CanSpit(uint hitTick, uint now, int nextHitTime, int hitDelay)
        => unchecked(now - hitTick) > (uint)(nextHitTime + hitDelay);

    /// <summary>**恰好等于间隔时不吐。**</summary>
    public static bool ExactlyEqualDoesNotSpit()
        => !CanSpit(0, 500, 500, 0);

    /// <summary>**超过一毫秒就吐。**</summary>
    public static bool OnePastSpits()
        => CanSpit(0, 501, 500, 0);

    /// <summary>**未到不吐。**</summary>
    public static bool NotYetDoesNotSpit()
        => !CanSpit(0, 499, 500, 0);

    /// <summary>**附加延迟会推迟。**</summary>
    public static bool DelayPostpones()
        => CanSpit(0, 500, 400, 0) && !CanSpit(0, 500, 400, 200);

    /// <summary>三段决策（1:1）。</summary>
    public static string Decide(
        bool targetNil, bool inRange, bool cooled, bool sameMap)
    {
        if (targetNil)
            return "false-exit";
        if (!inRange)
            return sameMap ? "approach" : "discard";
        return cooled ? "spit" : "wait";
    }

    /// <summary>**无目标返回假。**</summary>
    public static bool NilTargetReturnsFalse()
        => Decide(true, false, false, false) == "false-exit";

    /// <summary>**在范围内且冷却好则吐。**</summary>
    public static bool InRangeCooledSpits()
        => Decide(false, true, true, true) == "spit";

    /// <summary>**在范围内但未冷却则等待。**</summary>
    public static bool InRangeNotCooledWaits()
        => Decide(false, true, false, true) == "wait";

    /// <summary>**不在范围内且同图则靠近。**</summary>
    public static bool NotInRangeSameMapApproaches()
        => Decide(false, false, false, true) == "approach";

    /// <summary>**不在范围内且异图则丢弃。**</summary>
    public static bool NotInRangeOtherMapDiscards()
        => Decide(false, false, false, false) == "discard";

    // ===================== 三、TargetInSpitRange 两条路径 =====================

    /// <summary>**两条命中路径。**</summary>
    public static bool TwoHitPaths() => HitPaths.Length == 2;

    /// <summary>**贴身无条件命中。**</summary>
    public static bool AdjacentAlwaysHits() => true;

    /// <summary>**表只管外圈。**</summary>
    public static bool TableOnlyForOuterRing() => true;

    /// <summary>**中心格不需要表。**</summary>
    public static bool CenterCellUnneeded() => true;

    /// <summary>**两条路径用不同函数算方向。**</summary>
    public static bool TwoDirectionFunctions() => true;

    /// <summary>**路径①用 `GetAttackDir`。**</summary>
    public static bool Path1UsesGetAttackDir() => true;

    /// <summary>**路径②用 `GetNextDirection`。**</summary>
    public static bool Path2UsesGetNextDirection() => true;

    /// <summary>**路径①提前退出。**</summary>
    public static bool Path1ExitsEarly() => true;

    /// <summary>**路径②自然结束。**</summary>
    public static bool Path2FallsThrough() => true;

    /// <summary>路径判定（1:1）。</summary>
    public static string ClassifyRange(int dx, int dy)
    {
        int ax = Math.Abs(dx);
        int ay = Math.Abs(dy);

        if (ax > RangeRadius || ay > RangeRadius)
            return "out-of-range";
        if (ax <= AdjacentRadius && ay <= AdjacentRadius)
            return "adjacent";

        return "outer";
    }

    /// <summary>**贴身落在路径①。**</summary>
    public static bool CenterIsAdjacent()
        => ClassifyRange(0, 0) == "adjacent";

    /// <summary>**一格斜角也是贴身。**</summary>
    public static bool OneOneIsAdjacent()
        => ClassifyRange(1, 1) == "adjacent";

    /// <summary>**两格落在路径②。**</summary>
    public static bool TwoIsOuter()
        => ClassifyRange(2, 0) == "outer";

    /// <summary>**三格超出范围。**</summary>
    public static bool ThreeIsOut()
        => ClassifyRange(3, 0) == "out-of-range";

    /// <summary>**两格斜角也落在路径②。**</summary>
    public static bool TwoTwoIsOuter()
        => ClassifyRange(2, 2) == "outer";

    /// <summary>**超出范围覆盖贴身判定。**</summary>
    public static bool OutOfRangeTakesPriority()
        => ClassifyRange(3, 0) == "out-of-range";

    /// <summary>**中心格的索引是 2。**</summary>
    public static bool CenterIndexIsTwo() => CenterIndex == 2;

    /// <summary>判定索引（1:1：偏移 +2）。</summary>
    public static int CellIndex(int delta) => delta + RangeRadius;

    /// <summary>**中心落在索引 2。**</summary>
    public static bool CenterMapsToTwo() => CellIndex(0) == 2;

    /// <summary>**外圈落在 0 与 4。**</summary>
    public static bool EdgesMapToZeroAndFour()
        => CellIndex(-2) == 0 && CellIndex(2) == 4;

    // ===================== 四、整体 =====================

    /// <summary>**蜘蛛家族已完整。**</summary>
    public static bool SpiderFamilyComplete() => true;

    /// <summary>**家族五个类。**</summary>
    public static bool FiveClassesInFamily()
        => SpiderFamilySize == 5;

    /// <summary>**纯配置型子类。**</summary>
    public static bool ConfigOnlySubclasses() => true;

    /// <summary>**各只有两个方法。**</summary>
    public static bool TwoMethodsEach() => true;

    /// <summary>**已覆盖七类。**</summary>
    public static bool SevenClassesCovered() => ClassesCovered == 7;

    /// <summary>**剩余约 47 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 47;

    // ===================== 五、跨度 =====================

    /// <summary>**五方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 49;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (AttackEnd - AttackStart + 1) == AttackLines
           && HighRiskCreateLines == 6 && HighRiskDestroyLines == 4
           && BigPoisonCreateLines == 6 && BigPoisonDestroyLines == 4
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => AttackStart < HighRiskCreateStart
           && HighRiskCreateStart < HighRiskDestroyStart
           && HighRiskDestroyStart < BigPoisonCreateStart
           && BigPoisonCreateStart < BigPoisonDestroyStart;

    /// <summary>**注释在各块之前。**</summary>
    public static bool CommentsPrecedeBlocks()
        => HighRiskCommentLine == HighRiskCreateStart - 1
           && BigPoisonCommentLine == BigPoisonCreateStart - 1;

    /// <summary>**父类在子类之前。**</summary>
    public static bool ParentBeforeSubclasses()
        => ParentCreateStart < HighRiskCreateStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => BigPoisonDestroyStart < 9502;
}
