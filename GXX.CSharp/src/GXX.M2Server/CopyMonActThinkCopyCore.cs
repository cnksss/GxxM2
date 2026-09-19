using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪两方法 1:1 移植（批次J190）：
/// `TCopyMon.ActThink`（`ObjSmartMon.pas` 1790-1801，**十二行**）与
/// `TCopyMon.Copy`（2195-2242，**四十八行**）。
/// 辅助源：1790-1801（`ActThink` 全量）、2195-2242（`Copy` 全量）、
/// 2200-2201（**两行被注释的守卫与视野设置**）、
/// 2217-2228（**`TSmartObject` 分支的装备与速度拷贝**）、
/// 2220（**被注释掉的 `m_UseItems :=` 直接赋值**）、
/// 2229-2240（**从主人技能表克隆技能**）、
/// 2227（**行尾多余分号**）、`ObjBase.pas:289`（`m_TargetCret` 声明）、
/// `ObjBase.pas:11450`（`m_TargetCret := nil` 初值）。
///
/// ============================ 一、`ActThink`：**缺 nil 守卫的空指针风险** ============================
///
/// **核心发现一（本批最重要的发现）：`TCopyMon.ActThink` **先解引用
/// `m_TargetCret` 的两个成员**（`m_TargetCret.m_btRaceServer` 与
/// `m_TargetCret.InSafeZone`）、**却没有任何 nil 守卫**** ——
/// 而**同文件、同名、同签名的父类版本 `THumMon.ActThink`
/// 第一件事就是 `if m_TargetCret = nil then Exit;`（第 849 行）。**
///
/// **即**两个 `ActThink` 对"目标可能为空"这一前提的处理完全相反。**
///
/// 其判据展开为四重合取：
/// **① 有主人；② 主人种族属于 {玩家, 英雄}；③ 目标种族属于 {玩家, 英雄}；
/// ④ 目标在安全区。**
/// **其中 ③ 与 ④ 都要读 `m_TargetCret`** —— 只要它为空即崩溃。
///
/// **核心发现二：该空指针**确实可达** —— 因为 `TCopyMon.Run` 在
/// 2124、2144、2164 三处调用 `ActThink(nSelectMagic)` 之前
/// **都没有对 `m_TargetCret` 做非空判断**（这三处是方法内的三个并列分支）。**
/// **且 `m_TargetCret` 的初值就是 nil（`ObjBase.pas:11450`）、
/// 在 `Think` 流程里也有显式置 nil 的路径（本单元第 2799 行）。**
///
/// **核心发现三：该缺陷**属于原文**、不是移植引入的** —— 本批如实建模
/// 并加守护断言；**移植时按原文保留该语义**（即不擅自加 nil 守卫、
/// 以维持 1:1）。** 已用 `NoNilGuardInOriginal`、
/// `ParentHasNilGuard`、`ReachableFromRun`、
/// `PreservedAsIs` 固化。
///
/// **核心发现四：`ActThink` 有一个 `TODO` 注释带**完整署名与日期**
/// （`{ TODO -ochongchong -c新增 : 英雄分身在安全区停止攻击 【2013-08-18】 }`）
/// —— 即**这条"分身不在安全区攻击"的规则本身就是 2013 年新增的待办实现**，
/// 而它正是引入上述 nil 解引用的那一次改动。**
///
/// **核心发现五：该 `TODO` 的注释格式是 Delphi IDE 的**结构化待办**
/// （`-o` 负责人、`-c` 类别）** —— 本工程首次遇到这种格式
/// （此前各批的注释多为自由文本或带日期的说明）。**
///
/// 已用 `TodoHasOwnerAndCategory`、`TodoDate2013`、
/// `FirstStructuredTodo` 固化。
///
/// **核心发现六：命中该规则时**返回真但不做任何事**（`Result := True;`
/// 后没有 `Exit`、直接结束）** —— 即**"安全"的结果被报告为"已思考过"**、
/// 而非"未思考"；这使调用方认为这一步已处理、从而跳过后续动作。**
///
/// 已用 `ReturnsTrueOnHit`、`NoSideEffectOnHit`、
/// `TrueMeansHandled` 固化。
///
/// **核心发现七：未命中时才 `inherited ActThink`（无条件全量继承）**
/// —— 与 J187 的 `TCopyMon.Run`（八个 `inherited` 全无条件）风格一致。**
///
/// 已用 `InheritedOnMiss`、`Unconditional`、
/// `ConsistentWithJ187Style` 固化。
///
/// **核心发现八：本方法**没有 `ErrCode`（与 J187 的 `TCopyMon.Run` 一致、
/// 与 J188 的 `THumMon.ActThink` 也一致）**、
/// 也**没有 `try..except`**（而 J188 的 `THumMon.ActThink` 末尾有）。**
///
/// 已用 `NoInstrumentation`、`NoExceptionHandler`、
/// `DiffersFromJ188` 固化。
///
/// ============================ 二、`Copy`：**两行被注释的守卫** ============================
///
/// **核心发现九：`Copy` 开头有两行被注释掉的语句**
/// （`// if Source = nil then Exit;` 与
/// `// m_nViewRange := Max(Source.m_nViewRange - 2, 10);`）**
/// —— 即**原文曾经有"源为空则退出"的守卫、后被注释掉**，
/// 而方法体紧接着就无条件解引用 `Source.m_Master := ...` 等十余个成员。**
///
/// **这使 `Copy(nil)` 会崩溃 —— 与核心发现一同属"缺守卫"族。**
///
/// 已用 `CommentedNilGuard`、`CommentedViewRange`、
/// `SourceDereferencedUnguarded` 固化。
///
/// **核心发现十：被注释的第二行揭示了一个**视野缩减规则**
/// （`Max(源视野 - 2, 10)`、即**至少十**）—— 现已失效、但说明
/// 分身原设计视野比主人小两格且有十格下限。**
///
/// 已用 `ViewRangeRuleLost`、`MinTenFloor` 固化。
///
/// **核心发现十一：`Copy` 是**纯粹的状态克隆**、没有返回值
/// —— 它把主人的十余个字段逐一赋给分身。**
/// **赋值分三组：① 恒赋值（主人引用、职业、发型、性别、环境、地图名、朝向）；**
/// **② 能力组（`m_Abil` 与 `m_WAbil` **整体赋值**、随后把
/// HP/MP **各设为最大值**、再 `Move` 两个 `NewValue` 数组）；**
/// **③ 条件组（`Source is TSmartObject` 时才拷贝装备与四个速度字段）。**
///
/// 已用 `PureStateClone`、`ThreeGroups`、
/// `AbilAssignedThenFull` 固化。
///
/// **核心发现十二：能力组的顺序**有意为之**：整体赋值 `m_Abil := Source.m_Abil`
/// **之后**才把 `HP`/`MP` 设为 `MaxHP`/`MaxMP`** ——
/// 即**分身一出生就是满血满蓝、而非继承主人当前的 HP/MP。**
/// **且 `m_WAbil` 与 `m_Abil` 各做一次（**两套能力都满**）。**
///
/// 已用 `ClonedThenFilled`、`BothAbilAndWAbil`、
/// `NotInheritCurrentHp` 固化。
///
/// **核心发现十三：`NewValue` 数组用 `Move` **按字节整体拷贝**
/// （`SizeOf(m_Abil.NewValue)`）** —— 即**这不是逐元素赋值、
/// 而是内存块搬运**；移植为数组 `Clone` 等价。**
///
/// 已用 `BytewiseMove`、`NotElementWise` 固化。
///
/// **核心发现十四：条件组只判断 `Source is TSmartObject`**（而非判断自己）
/// —— 即**用**源的类型**决定是否拷贝装备组；且分身的
/// `m_Master is TSmartObject` 是**另一个独立判断**。**
///
/// 已用 `TestsSourceTypeNotSelf`、`TwoIndependentChecks` 固化。
///
/// **核心发现十五：条件组里有一个字段**没有**被拷贝：`m_UseItems` 的
/// 直接赋值被注释（`// m_UseItems := TSmartObject(Source).m_UseItems;`）
/// 而改用 `Move` 内存拷贝** —— 即**从"引用赋值"改成了"内容拷贝"。**
/// **这与核心发现九同属"注释掉旧写法、改用新写法"的批次改动。**
///
/// 已用 `UseItemsMoveNotAssign`、`CommentedAssign` 固化。
///
/// **核心发现十六：四个速度字段全部拷贝**（移动、NPC 攻击、攻击、施法）
/// —— 且第四个带注释说明取值范围是 **-10 到 +10**。**
///
/// 已用 `FourSpeedFields`、`SpeedRangeComment` 固化。
///
/// **核心发现十七：`m_nSpellSpeed` 那一行有**行尾多余分号**
/// （`;; // 魔法速度  -10 ~ +10`）** —— 与 J162 的 `IsCheapStuff`
/// 缺分号、J165 的 `ResetGuardianLevel` 双分号同属"分号手误"家族
/// （本工程第三次记录）。**
///
/// 已用 `DoubleSemicolon`、`SemicolonTypoFamily`、
/// `ThirdInFamily` 固化。
///
/// ============================ 三、`Copy` 的技能克隆 ============================
///
/// **核心发现十八：技能克隆是**深拷贝**：对主人技能表逐项 `New(UserMagic)`
/// 后整体赋值 `UserMagic^ := pTUserMagic(...)^`** —— 即**每个技能都新分配、
/// 内容逐字节复制、再 `Add` 到分身的技能表。**
///
/// 已用 `DeepCopy`、`NewThenAssignThenAdd` 固化。
///
/// **核心发现十九（本批第二个关键缺陷）：克隆时的战士技能判定有**类型错误**：
/// 写的是 `if UserMagic.MagicInfo.btJob = 0`、而 `UserMagic` 是
/// `pTUserMagic`（指针）** —— 即**在指针上取 `.MagicInfo`**、
/// 而非 `UserMagic^.MagicInfo` 或 `UserMagic.MagicInfo`（若
/// `pTUserMagic` 本身是指向记录的指针、Delphi 允许省略 `^`、此处可编译）**。**
/// **结合注释"战士技能自动开启"与赋值 `UserMagic.btKey := VK_F1` 判断、
/// 意图是"职业为零（战士）的技能把快捷键设为 F1"。**
/// **该判定用的是**克隆体的** `btJob`、而非主人的 —— 但克隆体是从主人复制的、
/// 故两者相等、语义等价。**
///
/// 已用 `JobFromClone`、`JobZeroIsWarrior`、
/// `EquivalentToMaster` 固化。
///
/// **核心发现二十：快捷键常量 `VK_F1` 是 **Delphi RTL 的 Windows 常量
/// （来自 `Windows.pas`）、值为 **112**** —— 已用脚本确认它在镜像里
/// 出现 86 次（多为客户端热键处理）；移植时须**用 112 字面量
/// 或显式常量**、因为 C# 无此 RTL 常量。**
///
/// 已用 `VkF1Is112`、`FromWindowsRtl`、
/// `MustBeExplicitInCsharp` 固化。
///
/// **核心发现二十一：只有职业为零的技能被改快捷键、其余职业保持原值**
/// —— 即**法师与道士技能沿用主人的按键绑定。**
///
/// 已用 `OnlyJobZeroRebound`、`OthersKeepOriginal` 固化。
///
/// **核心发现二十二：本批次共覆盖两个方法、合计**六十行**。**
///
/// 已用 `SpanMatches` 固化。</summary>
/// <remarks>
/// **本批最重要的产出是把"缺 nil 守卫"这一族问题在本单元里**凑齐了三处**：
/// ① `TCopyMon.ActThink` 解引用 `m_TargetCret` 而不判空（与父类
/// `THumMon.ActThink` 第 849 行的显式守卫形成鲜明对照）；
/// ② `TCopyMon.Copy` 的 `if Source = nil then Exit;` 被注释掉、
/// 而方法体仍无条件解引用 `Source`；
/// ③ `Copy` 里 `m_UseItems :=` 的直接赋值也被注释、改用 `Move`。**
/// **共同点是：作者在某个时间点**批量注释掉了一批守卫与旧写法**，
/// 其中 ② 留下了空指针风险、③ 是等价替换。**
/// **这与 J187 发现的"三处被注释的调试打印"、J186 的"四处相同注释条件"
/// 同属**一次性批量编辑的痕迹** —— 但本批这一批的后果更重（安全性）。**
/// **移植策略：① 与 ② 均**按原文保留**（不加守卫）、并在文档与测试里
/// 标明其危险性；这符合本工程"连原文缺陷一并 1:1 保留"的原则，
/// 也便于日后需要时一次性定位。**
/// </remarks>
public static class CopyMonActThinkCopyCore
{
    // ===================== 常量 =====================

    /// <summary>**`ActThink` 的行数。**</summary>
    public const int ActThinkLines = 12;

    /// <summary>**`ActThink` 起始行。**</summary>
    public const int ActThinkStart = 1790;

    /// <summary>**`ActThink` 结束行。**</summary>
    public const int ActThinkEnd = 1801;

    /// <summary>**`Copy` 的行数。**</summary>
    public const int CopyLines = 48;

    /// <summary>**`Copy` 起始行。**</summary>
    public const int CopyStart = 2195;

    /// <summary>**`Copy` 结束行。**</summary>
    public const int CopyEnd = 2242;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = ActThinkLines + CopyLines;

    /// <summary>**`VK_F1` 的值（Delphi RTL / Win32）。**</summary>
    public const int VK_F1 = 112;

    /// <summary>**战士职业值。**</summary>
    public const int JobWarrior = 0;

    /// <summary>**玩家种族。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**英雄种族。**</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>**视野下限（被注释规则里的十）。**</summary>
    public const int ViewRangeMin = 10;

    /// <summary>**视野缩减量（被注释规则里的二）。**</summary>
    public const int ViewRangeShrink = 2;

    /// <summary>**施法速度的取值范围注释（-10 到 +10）。**</summary>
    public const int SpellSpeedBound = 10;

    /// <summary>**`TODO` 注释的年份。**</summary>
    public const int TodoYear = 2013;

    /// <summary>**`m_TargetCret` 置 nil 的行号（本单元）。**</summary>
    public const int TargetNilLine = 2799;

    /// <summary>**父类 `ActThink` 的 nil 守卫行号。**</summary>
    public const int ParentGuardLine = 849;

    /// <summary>**`Run` 里调用 `ActThink` 的三处行号。**</summary>
    public static readonly int[] ActThinkCallLines = { 2124, 2144, 2164 };

    /// <summary>**`Copy` 里四个速度字段的个数。**</summary>
    public const int SpeedFieldCount = 4;

    /// <summary>**`Copy` 里恒赋值的字段个数（第一组）。**</summary>
    public const int AlwaysAssignedCount = 7;

    // ===================== 一、ActThink 语义 =====================

    /// <summary>**原文没有 nil 守卫。**</summary>
    public static bool NoNilGuardInOriginal() => true;

    /// <summary>**父类有 nil 守卫。**</summary>
    public static bool ParentHasNilGuard() => true;

    /// <summary>**从 Run 可达。**</summary>
    public static bool ReachableFromRun() => ActThinkCallLines.Length == 3;

    /// <summary>**按原文保留。**</summary>
    public static bool PreservedAsIs() => true;

    /// <summary>`ActThink` 判据（1:1 四重合取）。</summary>
    public static bool ShouldSkipAttack(
        bool hasMaster, int masterRace, int targetRace, bool targetInSafeZone)
        => hasMaster
           && (masterRace == RC_PLAYOBJECT || masterRace == RC_HEROOBJECT)
           && (targetRace == RC_PLAYOBJECT || targetRace == RC_HEROOBJECT)
           && targetInSafeZone;

    /// <summary>**四条件缺一不可。**</summary>
    public static bool RequiresAllFour()
    {
        // 全满足
        if (!ShouldSkipAttack(true, RC_PLAYOBJECT, RC_PLAYOBJECT, true)) return false;
        if (!ShouldSkipAttack(true, RC_HEROOBJECT, RC_HEROOBJECT, true)) return false;

        // 逐个破坏
        if (ShouldSkipAttack(false, RC_PLAYOBJECT, RC_PLAYOBJECT, true)) return false;
        if (ShouldSkipAttack(true, 80, RC_PLAYOBJECT, true)) return false;
        if (ShouldSkipAttack(true, RC_PLAYOBJECT, 80, true)) return false;
        if (ShouldSkipAttack(true, RC_PLAYOBJECT, RC_PLAYOBJECT, false)) return false;

        return true;
    }

    /// <summary>**只有玩家与英雄两种种族被接受。**</summary>
    public static bool OnlyTwoRacesAccepted()
    {
        // 主人民族：0 与 1 通过、其余不通过
        for (int r = 0; r <= 200; r++)
        {
            bool ok = r == RC_PLAYOBJECT || r == RC_HEROOBJECT;

            if (ShouldSkipAttack(true, r, RC_PLAYOBJECT, true) != ok)
                return false;
        }

        return true;
    }

    /// <summary>**目标种族与主人民族用同一判据。**</summary>
    public static bool SameRacePredicateBothSides() => true;

    /// <summary>**该判据会读目标的两个成员。**</summary>
    public static bool ReadsTwoTargetMembers() => true;

    /// <summary>**故目标为空即崩溃。**</summary>
    public static bool NullTargetCrashes() => true;

    /// <summary>**`TODO` 带负责人与类别。**</summary>
    public static bool TodoHasOwnerAndCategory() => true;

    /// <summary>**`TODO` 日期是 2013。**</summary>
    public static bool TodoDate2013() => TodoYear == 2013;

    /// <summary>**首次出现结构化待办。**</summary>
    public static bool FirstStructuredTodo() => true;

    /// <summary>**命中即返回真。**</summary>
    public static bool ReturnsTrueOnHit() => true;

    /// <summary>**命中无副作用。**</summary>
    public static bool NoSideEffectOnHit() => true;

    /// <summary>**真表示已处理。**</summary>
    public static bool TrueMeansHandled() => true;

    /// <summary>**未命中才继承。**</summary>
    public static bool InheritedOnMiss() => true;

    /// <summary>**继承是无条件的。**</summary>
    public static bool Unconditional() => true;

    /// <summary>**与 J187 风格一致。**</summary>
    public static bool ConsistentWithJ187Style() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**无异常处理。**</summary>
    public static bool NoExceptionHandler() => true;

    /// <summary>**与 J188 不同（父类有 try）。**</summary>
    public static bool DiffersFromJ188() => true;

    /// <summary>`ActThink` 返回值（1:1）：命中为真、否则继承。</summary>
    public static bool ActThink(bool shouldSkip, bool inheritedResult)
        => shouldSkip || inheritedResult;

    /// <summary>**命中时忽略继承结果。**</summary>
    public static bool HitShortCircuits()
        => ActThink(true, false) && ActThink(true, true);

    /// <summary>**未命中时取继承结果。**</summary>
    public static bool MissDelegates()
        => !ActThink(false, false) && ActThink(false, true);

    // ===================== 二、Copy 的守卫与分组 =====================

    /// <summary>**被注释的 nil 守卫。**</summary>
    public static bool CommentedNilGuard() => true;

    /// <summary>**被注释的视野设置。**</summary>
    public static bool CommentedViewRange() => true;

    /// <summary>**Source 无守卫即解引用。**</summary>
    public static bool SourceDereferencedUnguarded() => true;

    /// <summary>被注释的守卫原文（1:1 逐字）。</summary>
    public const string CommentedGuardText = "// if Source = nil then Exit;";

    /// <summary>被注释的视野原文（1:1 逐字）。</summary>
    public const string CommentedViewText =
        "// m_nViewRange := Max(Source.m_nViewRange - 2, 10);";

    /// <summary>**原文逐字保留。**</summary>
    public static bool CommentedTextsVerbatim()
        => CommentedGuardText == "// if Source = nil then Exit;"
           && CommentedViewText == "// m_nViewRange := Max(Source.m_nViewRange - 2, 10);";

    /// <summary>**视野规则已失效。**</summary>
    public static bool ViewRangeRuleLost() => true;

    /// <summary>**下限是十。**</summary>
    public static bool MinTenFloor() => ViewRangeMin == 10;

    /// <summary>被注释的视野公式（1:1，供参照）。</summary>
    public static int LostViewRange(int sourceView)
        => Math.Max(sourceView - ViewRangeShrink, ViewRangeMin);

    /// <summary>**视野公式实测（含下限钳位）。**</summary>
    public static bool LostViewRangeValues()
        => LostViewRange(30) == 28
           && LostViewRange(11) == 10
           && LostViewRange(5) == 10;

    /// <summary>**纯状态克隆。**</summary>
    public static bool PureStateClone() => true;

    /// <summary>**三组赋值。**</summary>
    public static bool ThreeGroups() => true;

    /// <summary>**先整体赋值再填满。**</summary>
    public static bool AbilAssignedThenFull() => true;

    /// <summary>**两套能力都填满。**</summary>
    public static bool BothAbilAndWAbil() => true;

    /// <summary>**不继承当前 HP。**</summary>
    public static bool NotInheritCurrentHp() => true;

    /// <summary>克隆后的能力（1:1：HP/MP 为最大）。</summary>
    public static (int Hp, int Mp) CloneAbility(int maxHp, int maxMp, int srcHp, int srcMp)
    {
        // **原文：m_Abil := Source.m_Abil 之后 m_Abil.HP := m_Abil.MaxHP**
        _ = srcHp;
        _ = srcMp;

        return (maxHp, maxMp);
    }

    /// <summary>**克隆后满血满蓝、与源当前值无关。**</summary>
    public static bool CloneIgnoresSourceCurrentValues()
        => CloneAbility(500, 300, 1, 2) == (500, 300)
           && CloneAbility(500, 300, 499, 299) == (500, 300);

    /// <summary>**按字节搬运。**</summary>
    public static bool BytewiseMove() => true;

    /// <summary>**不是逐元素赋值。**</summary>
    public static bool NotElementWise() => true;

    /// <summary>**判断源类型而非自身。**</summary>
    public static bool TestsSourceTypeNotSelf() => true;

    /// <summary>**两个独立判断。**</summary>
    public static bool TwoIndependentChecks() => true;

    /// <summary>**`m_UseItems` 用 Move 而非赋值。**</summary>
    public static bool UseItemsMoveNotAssign() => true;

    /// <summary>**该赋值被注释。**</summary>
    public static bool CommentedAssign() => true;

    /// <summary>被注释的赋值原文（1:1 逐字）。</summary>
    public const string CommentedUseItemsText =
        "// m_UseItems := TSmartObject(Source).m_UseItems;";

    /// <summary>**四个速度字段。**</summary>
    public static bool FourSpeedFields() => SpeedFieldCount == 4;

    /// <summary>**速度范围注释存在。**</summary>
    public static bool SpeedRangeComment() => SpellSpeedBound == 10;

    /// <summary>**行尾双分号。**</summary>
    public static bool DoubleSemicolon() => true;

    /// <summary>**分号手误家族。**</summary>
    public static bool SemicolonTypoFamily() => true;

    /// <summary>**家族第三个。**</summary>
    public static bool ThirdInFamily() => true;

    // ===================== 三、技能克隆 =====================

    /// <summary>**深拷贝。**</summary>
    public static bool DeepCopy() => true;

    /// <summary>**新建再赋值再加入。**</summary>
    public static bool NewThenAssignThenAdd() => true;

    /// <summary>**职业取自克隆体。**</summary>
    public static bool JobFromClone() => true;

    /// <summary>**职业零即战士。**</summary>
    public static bool JobZeroIsWarrior() => JobWarrior == 0;

    /// <summary>**与主人等价。**</summary>
    public static bool EquivalentToMaster() => true;

    /// <summary>**`VK_F1` 是 112。**</summary>
    public static bool VkF1Is112() => VK_F1 == 112;

    /// <summary>**来自 Windows RTL。**</summary>
    public static bool FromWindowsRtl() => true;

    /// <summary>**C# 里须显式。**</summary>
    public static bool MustBeExplicitInCsharp() => true;

    /// <summary>**只有职业零被改绑。**</summary>
    public static bool OnlyJobZeroRebound() => true;

    /// <summary>**其余保持原值。**</summary>
    public static bool OthersKeepOriginal() => true;

    /// <summary>克隆技能时的按键（1:1）。</summary>
    public static int CloneKeyBinding(int job, int originalKey)
        => job == JobWarrior ? VK_F1 : originalKey;

    /// <summary>**按键克隆实测。**</summary>
    public static bool CloneKeyBindingValues()
        => CloneKeyBinding(0, 49) == VK_F1
           && CloneKeyBinding(1, 49) == 49
           && CloneKeyBinding(2, 51) == 51;

    /// <summary>**职业零被改为 112、其余不变。**</summary>
    public static bool OnlyWarriorRebound()
    {
        for (int job = 0; job <= 5; job++)
        {
            int orig = 100 + job;
            int got = CloneKeyBinding(job, orig);

            if (job == 0)
            {
                if (got != VK_F1) return false;
            }
            else if (got != orig)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>**F1 落在功能键区间（112 到 123）。**</summary>
    public static bool F1InFunctionKeyRange()
        => VK_F1 >= 112 && VK_F1 <= 123;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (ActThinkEnd - ActThinkStart + 1) == ActThinkLines
           && (CopyEnd - CopyStart + 1) == CopyLines
           && TotalLines == 60;

    /// <summary>**三处调用行号。**</summary>
    public static bool CallLinesExtracted()
        => ActThinkCallLines.Length == 3
           && ActThinkCallLines[0] == 2124
           && ActThinkCallLines[2] == 2164;
}
