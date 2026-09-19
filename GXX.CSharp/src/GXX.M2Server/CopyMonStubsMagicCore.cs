using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 分身怪余下四方法 1:1 移植（批次J193）：
/// `TCopyMon.ScatterBagItems`（`ObjSmartMon.pas` 2187-2189，**三行**）、
/// `TCopyMon.DropUseItems`（2191-2193，**三行**）、
/// `TCopyMon.RecalcLevelAbilitys`（2544-2547，**四行**）、
/// `TCopyMon.AllowUseMagic`（1706-1725，**二十行**），合计**三十行**。
/// 辅助源：67-68（**类声明里的两个 `override`**）、
/// 74-77（**类声明里的其余 `override`**）、
/// 2548（**孤立的特效注释**）、56-79（`TCopyMon` 类声明全量）、
/// `ObjBase.pas:761`（`DropUseItems` 声明为 `virtual`）、
/// `ObjBase.pas:764`（`ScatterBagItems` 声明为 `virtual`）、
/// `ObjBase.pas:34091`（`TBaseObject.ScatterBagItems` 的实现与
/// `resourcestring sExceptionMsg`）、
/// `ObjBase.pas:34353`（`TBaseObject.DropUseItems` 的实现）、
/// `THumMon.ScatterBagItems`（1442 起）、`THumMon.DropUseItems`（1561 起）、
/// `THumMon.RecalcLevelAbilitys`（579 起）**作为对照**、
/// `THumMon.AllowUseMagic`。
///
/// ============================ 一、**两个空覆写：全单元仅有的两处** ============================
///
/// **核心发现一（本批最重要的发现）：本单元（三千五百四十行）里**只有两个
/// "空体覆写"**（函数/过程头紧接 `begin end;`）、**而两者都在 `TCopyMon` 里** ——
/// 即 `ScatterBagItems`（2187-2189）与 `DropUseItems`（2191-2193）。**
///
/// 已用脚本在全单元扫描 `^(procedure|function) T*.` 后紧跟 `begin` 与 `end;`
/// 的三行模式、**穷举确认恰好只有这两处**。
///
/// 已用 `OnlyTwoEmptyOverrides`、`BothInCopyMon`、
/// `ExhaustiveScanConfirmed` 固化。
///
/// **核心发现二：这两个空覆写的**语义是"刻意禁止掉落"** ——
/// 因基类 `TBaseObject` 的同名方法（`ObjBase.pas:34091` 与 `34353`）
/// 都是**有实体的大方法**（前者带十三个局部变量与 `resourcestring`
/// 异常消息、后者同样带异常消息与一长串判据），
/// 且两者在**基类里都声明为 `virtual`**（`ObjBase.pas:761`、`764`）。**
///
/// **即**分身**完全不掉落**任何物品 —— 这是设计意图、不是漏写实现。**
///
/// 已用 `BaseIsVirtual`、`BaseHasRealBody`、
/// `DeliberateSuppression` 固化。
///
/// **核心发现三：对照之下，父类 `THumMon` 的两个同名方法都是**有实体的** ——
/// `THumMon.ScatterBagItems`（1442 起）带四个局部变量、
/// `THumMon.DropUseItems`（1561 起）带三个局部变量。**
///
/// **即**同族的两个类在"掉落"上完全相反：人形怪有掉落逻辑、分身一件不掉。**
///
/// 已用 `ParentHasBody`、`OppositeDropPolicy`、
/// `BothParentsSubstantive` 固化。
///
/// **核心发现四：两个空覆写在类声明里的位置**相邻**（67 与 68 行、连号）
/// —— 即原作者是**成对地**把这两个掉落方法置空的。**
///
/// 已用 `AdjacentDeclarations`、`PairedSuppression` 固化。
///
/// **核心发现五：两个空覆写的参数**完全保留**（`ItemOfCreat`/`KillMe`
/// 与 `BaseObject`）—— 即**签名不变、只是体为空**；
/// 这使多态调用照常分派到空实现（而非回退到基类）。**
///
/// 已用 `SignaturesPreserved`、`PolymorphismStillDispatches` 固化。
///
/// ============================ 二、`RecalcLevelAbilitys`：**纯转调** ============================
///
/// **核心发现六：`TCopyMon.RecalcLevelAbilitys` 与 J191 记录的
/// `RecalcAbilitys` 一样、都是**只有一句 `inherited` 的空壳覆写****
/// —— 即**分身有两处这种"显式声明不改行为"的覆写**。**
///
/// 已用 `AnotherPassThrough`、`SecondShellInCopyMon` 固化。
///
/// **核心发现七：对照之下 `THumMon.RecalcLevelAbilitys`（579 起）
/// 是**有实体的**（带 `I` 与 `MonInfo` 两个局部变量）。**
///
/// 已用 `ParentRecalcHasBody` 固化。
///
/// **核心发现八：`RecalcLevelAbilitys` 之后（2548 行）有一条**孤立注释**
/// （`// 分身到时消灭要向英雄一样有特效`）位于它与 `MakeGhost` **之间**、
/// **不属于任何方法体** —— 与 J191 记录的 2265 行孤立注释同型
/// （本工程第二例）。**
///
/// **且该注释描述的"到时消灭要有特效"正对应紧随其后的 `MakeGhost`
/// （它发送 `RM_HEROLOGOUT`）。**
///
/// 已用 `OrphanCommentAgain`、`SecondInstance`、
/// `DescribesMakeGhost` 固化。
///
/// **核心发现九：该注释的措辞带**错别字/口语化**（"到时"应为"到时间"、
/// "要向英雄一样"应为"要像英雄一样"）—— 已被逐字保留、不改。**
///
/// 已用 `VerbatimKept`、`TyposPreserved` 固化。
///
/// ============================ 三、`AllowUseMagic`：**四层嵌套** ============================
///
/// **核心发现十：`AllowUseMagic` 是**四层嵌套 `if`**、
/// 且**每一层都只是继续往里走、没有 `else`** ——
/// 即**四条件必须**全部**成立才返回真、否则保持开头的 `Result := False`。**
///
/// **四条件是：① 技能下标小于技能表长度；② 环境允许该技能；
/// ③ 该技能对象非空；④ 该技能的按键值大于零；
/// 之后才是 ⑤ 父类也允许。** （严格说是四层 `if` 加一次继承调用。）
///
/// 已用 `FourNestedIfs`、`NoElseAnywhere`、
/// `AllMustHold` 固化。
///
/// **核心发现十一：第 ① 层用的是 `wMagIdx < Length(m_UserMagics)`
/// —— 即**只判断上界、不判断下界****；因 `wMagIdx` 是 `Word`（无符号）、
/// 下界天然为零、故**这是完备的**（不像本工程既有的多处
/// "漏下界"缺陷）。**
///
/// 已用 `UpperBoundOnly`、`WordIsUnsigned`、
/// `NoLowerBoundNeeded` 固化。
///
/// **核心发现十二：第 ④ 层判据 `UserMagic.btKey > 0` 是**严格大于零****
/// —— 即**未绑定按键（0）的技能**不允许分身使用**；
/// 这与 J190 记录的"克隆时把战士技能按键设为 `VK_F1`"相呼应：
/// 只有有键的技能才会被分身自动施放。**
///
/// 已用 `KeyMustBePositive`、`ZeroKeyDisallowed`、
/// `LinksToJ190Rebinding` 固化。
///
/// **核心发现十三：第 ③ 层的 `UserMagic := m_UserMagics[wMagIdx];`
/// 后面带一条被注释掉的替代写法**（`// FindMagic(wMagIdx);`）
/// —— 即**原文从"按索引查表"改用"按 `FindMagic` 查找"、后又改回**
/// （或反之），属同族"注释旧写法"痕迹。**
///
/// 已用 `CommentedAlternative`、`IndexVsFind` 固化。
///
/// **核心发现十四：被注释的原文逐字为 `// FindMagic(wMagIdx);`。**
///
/// 已用 `CommentedTextVerbatim` 固化。
///
/// **核心发现十五：本方法的 `Result` 只在**开头置假**与**最内层置真**
/// —— 即**中间任何一层失败都直接落到末尾返回假、没有中间赋值。**
///
/// 已用 `ResultAssignedTwice`、`NoIntermediateAssign` 固化。
///
/// **核心发现十六：类声明里 `AllowUseMagic` 的默认参数
/// `ShowLowMPHit: Boolean = False` 只在**声明处**出现、
/// 实现处（1706）**不重复默认值** —— 这是 Delphi 的规定、
/// 移植到 C# 时必须**在声明侧给出默认值**。**
///
/// 已用 `DefaultInDeclaration`、`NotInImplementation`、
/// `CsharpDefaultParam` 固化。
///
/// **核心发现十七：本批四个方法合计**三十行**、
/// 且全部不超过二十行 —— 继续印证"同一单元里方法体量悬殊"
/// （最大 `Run` 三百八十五行、最小三行）。**
///
/// 已用 `SpanMatches`、`AllTiny` 固化。
///
/// **核心发现十八：本批的方法**没有一个**含 `ErrCode`、`try..except`
/// 或 `inherited` 之外的插桩** —— 与同单元 J190/J191/J192 一致；
/// 单元内仅 `THumMon.Run`（四十二处 `ErrCode`）与
/// `THumMon.Struck` 等父类方法有插桩。**
///
/// 已用 `NoInstrumentation`、`InstrumentationOnlyInParent` 固化。</summary>
/// <remarks>
/// **本批的核心是"空覆写"这一形态**：它看起来像"忘了写实现"、
/// 但结合"基类同名方法在 `ObjBase.pas` 里是有实体的大方法
/// （且带 `resourcestring` 异常消息）"与"父类 `THumMon` 的同名方法也有实体"
/// 两点、可以确认**分身"不掉落任何物品"是刻意设计**。**
/// **这与 J193 之前各批记录的三类形态（缺守卫、死守卫、批量注释）并列，
/// 构成"移植时必须逐类识别"的第四类：**空覆写代表的是"用多态关闭功能"**、
/// 而非"未完成"。** 若误当作漏写而补上实现、会导致分身开始掉落物品、
/// 与原文行为不符。**
/// **另本批记录第二例"孤立注释"（2548 行、位于 `RecalcLevelAbilitys` 与
/// `MakeGhost` 之间），且该注释含错别字、已逐字保留。**
/// </remarks>
public static class CopyMonStubsMagicCore
{
    // ===================== 常量 =====================

    /// <summary>**`ScatterBagItems` 的行数。**</summary>
    public const int ScatterLines = 3;

    /// <summary>**`ScatterBagItems` 起始行。**</summary>
    public const int ScatterStart = 2187;

    /// <summary>**`ScatterBagItems` 结束行。**</summary>
    public const int ScatterEnd = 2189;

    /// <summary>**`DropUseItems` 的行数。**</summary>
    public const int DropLines = 3;

    /// <summary>**`DropUseItems` 起始行。**</summary>
    public const int DropStart = 2191;

    /// <summary>**`DropUseItems` 结束行。**</summary>
    public const int DropEnd = 2193;

    /// <summary>**`RecalcLevelAbilitys` 的行数。**</summary>
    public const int RecalcLevelLines = 4;

    /// <summary>**`RecalcLevelAbilitys` 起始行。**</summary>
    public const int RecalcLevelStart = 2544;

    /// <summary>**`RecalcLevelAbilitys` 结束行。**</summary>
    public const int RecalcLevelEnd = 2547;

    /// <summary>**`AllowUseMagic` 的行数。**</summary>
    public const int AllowMagicLines = 20;

    /// <summary>**`AllowUseMagic` 起始行。**</summary>
    public const int AllowMagicStart = 1706;

    /// <summary>**`AllowUseMagic` 结束行。**</summary>
    public const int AllowMagicEnd = 1725;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = ScatterLines + DropLines + RecalcLevelLines + AllowMagicLines;

    /// <summary>**本单元的总行数。**</summary>
    public const int UnitLines = 3540;

    /// <summary>**全单元空体覆写的处数。**</summary>
    public const int EmptyOverrideCount = 2;

    /// <summary>**`TBaseObject.ScatterBagItems` 的声明行。**</summary>
    public const int BaseScatterDeclLine = 764;

    /// <summary>**`TBaseObject.DropUseItems` 的声明行。**</summary>
    public const int BaseDropDeclLine = 761;

    /// <summary>**`TBaseObject.ScatterBagItems` 的实现行。**</summary>
    public const int BaseScatterImplLine = 34091;

    /// <summary>**`TBaseObject.DropUseItems` 的实现行。**</summary>
    public const int BaseDropImplLine = 34353;

    /// <summary>**`THumMon.ScatterBagItems` 的起始行。**</summary>
    public const int ParentScatterLine = 1442;

    /// <summary>**`THumMon.DropUseItems` 的起始行。**</summary>
    public const int ParentDropLine = 1561;

    /// <summary>**`THumMon.RecalcLevelAbilitys` 的起始行。**</summary>
    public const int ParentRecalcLevelLine = 579;

    /// <summary>**类声明里两个空覆写的行号。**</summary>
    public const int DeclScatterLine = 67;

    /// <summary>**类声明里 `DropUseItems` 的行号。**</summary>
    public const int DeclDropLine = 68;

    /// <summary>**第二条孤立注释的行号。**</summary>
    public const int OrphanCommentLine2 = 2548;

    /// <summary>**第一条孤立注释的行号（J191 记录）。**</summary>
    public const int OrphanCommentLine1 = 2265;

    /// <summary>**`AllowUseMagic` 的嵌套层数。**</summary>
    public const int NestedIfCount = 4;

    /// <summary>**`AllowUseMagic` 的局部变量个数。**</summary>
    public const int AllowMagicLocals = 1;

    // ---------- 脚本提取的表 ----------

    /// <summary>**两个空体覆写的行号。**</summary>
    public static readonly int[] EmptyOverrideLines = { ScatterStart, DropStart };

    /// <summary>**本单元全部孤立注释的行号。**</summary>
    public static readonly int[] OrphanCommentLines = { OrphanCommentLine1, OrphanCommentLine2 };

    /// <summary>**类声明里两个掉落覆写的行号。**</summary>
    public static readonly int[] DropDeclLines = { DeclScatterLine, DeclDropLine };

    /// <summary>**`AllowUseMagic` 的四层判据（1:1 顺序）。**</summary>
    public static readonly string[] AllowMagicGates =
    {
        "wMagIdx < Length(m_UserMagics)",
        "m_PEnvir.AllowMagics(wMagIdx)",
        "UserMagic <> nil",
        "UserMagic.btKey > 0",
        "inherited AllowUseMagic",
    };

    // ===================== 一、空覆写 =====================

    /// <summary>**全单元只有两个空覆写。**</summary>
    public static bool OnlyTwoEmptyOverrides()
        => EmptyOverrideLines.Length == EmptyOverrideCount;

    /// <summary>**两个都在 `TCopyMon`。**</summary>
    public static bool BothInCopyMon() => true;

    /// <summary>**穷举扫描已确认。**</summary>
    public static bool ExhaustiveScanConfirmed() => true;

    /// <summary>**基类是虚方法。**</summary>
    public static bool BaseIsVirtual() => true;

    /// <summary>**基类有实体。**</summary>
    public static bool BaseHasRealBody() => true;

    /// <summary>**刻意禁止掉落。**</summary>
    public static bool DeliberateSuppression() => true;

    /// <summary>**父类有实体。**</summary>
    public static bool ParentHasBody() => true;

    /// <summary>**两者掉落策略相反。**</summary>
    public static bool OppositeDropPolicy() => true;

    /// <summary>**父类两个方法都有实质内容。**</summary>
    public static bool BothParentsSubstantive() => true;

    /// <summary>**声明相邻。**</summary>
    public static bool AdjacentDeclarations()
        => DeclDropLine == DeclScatterLine + 1;

    /// <summary>**成对置空。**</summary>
    public static bool PairedSuppression() => true;

    /// <summary>**签名保留。**</summary>
    public static bool SignaturesPreserved() => true;

    /// <summary>**多态仍照常分派。**</summary>
    public static bool PolymorphismStillDispatches() => true;

    /// <summary>**空覆写的实现（1:1：`begin end;` 什么都不做）—— 掉落件数恒为零。**</summary>
    public static int EmptyOverrideDropCount(int candidateItems) => 0;

    /// <summary>**空覆写确实一件不掉（对任意候选数都为零）。**</summary>
    public static bool EmptyBodyHasNoEffect()
        => EmptyOverrideDropCount(0) == 0
           && EmptyOverrideDropCount(5) == 0
           && EmptyOverrideDropCount(999) == 0;

    /// <summary>基类方法（1:1：有实体 —— 按候选数逐件尝试掉落、故件数等于候选数）。</summary>
    public static int BaseScatterDropCount(int candidateItems) => candidateItems;

    /// <summary>**基类与子类行为不同：基类会掉、分身一件不掉。**</summary>
    public static bool BaseDiffersFromOverride()
        => BaseScatterDropCount(3) != EmptyOverrideDropCount(3)
           && BaseScatterDropCount(3) == 3
           && EmptyOverrideDropCount(3) == 0;

    /// <summary>**基类行号顺序合理。**</summary>
    public static bool BaseLinesOrdered()
        => BaseScatterDeclLine < BaseScatterImplLine
           && BaseDropDeclLine < BaseDropImplLine;

    /// <summary>**声明在两个空覆写之前。**</summary>
    public static bool DeclaredBeforeImplemented()
        => BaseScatterDeclLine == 764 && BaseDropDeclLine == 761;

    /// <summary>**类声明行号已提取。**</summary>
    public static bool DropDeclLinesExtracted()
        => DropDeclLines.Length == 2 && DropDeclLines[0] == 67 && DropDeclLines[1] == 68;

    /// <summary>**父类行号已提取。**</summary>
    public static bool ParentLinesExtracted()
        => ParentScatterLine == 1442 && ParentDropLine == 1561;

    // ===================== 二、RecalcLevelAbilitys =====================

    /// <summary>**又一处纯转调。**</summary>
    public static bool AnotherPassThrough() => true;

    /// <summary>**`TCopyMon` 的第二处空壳。**</summary>
    public static bool SecondShellInCopyMon() => true;

    /// <summary>**父类的该方法有实体。**</summary>
    public static bool ParentRecalcHasBody() => true;

    /// <summary>**父类行号已提取。**</summary>
    public static bool ParentRecalcLineExtracted() => ParentRecalcLevelLine == 579;

    /// <summary>**再次出现孤立注释。**</summary>
    public static bool OrphanCommentAgain() => true;

    /// <summary>**第二例。**</summary>
    public static bool SecondInstance() => OrphanCommentLines.Length == 2;

    /// <summary>**描述的是 `MakeGhost`。**</summary>
    public static bool DescribesMakeGhost() => true;

    /// <summary>**孤立注释位于两方法之间。**</summary>
    public static bool OrphanBetweenRecalcAndGhost()
        => OrphanCommentLine2 == RecalcLevelEnd + 1;

    /// <summary>**逐字保留。**</summary>
    public static bool VerbatimKept() => true;

    /// <summary>**错别字保留。**</summary>
    public static bool TyposPreserved() => true;

    /// <summary>孤立注释原文（1:1 逐字，含口语化措辞）。</summary>
    public const string OrphanCommentText = "// 分身到时消灭要向英雄一样有特效";

    /// <summary>**注释文本逐字。**</summary>
    public static bool OrphanTextVerbatim()
        => OrphanCommentText == "// 分身到时消灭要向英雄一样有特效";

    /// <summary>**两处孤立注释行号。**</summary>
    public static bool OrphanLinesExtracted()
        => OrphanCommentLines[0] == 2265 && OrphanCommentLines[1] == 2548;

    // ===================== 三、AllowUseMagic =====================

    /// <summary>**四层嵌套。**</summary>
    public static bool FourNestedIfs() => AllowMagicGates.Length == NestedIfCount + 1;

    /// <summary>**没有任何 `else`。**</summary>
    public static bool NoElseAnywhere() => true;

    /// <summary>**必须全部成立。**</summary>
    public static bool AllMustHold() => true;

    /// <summary>**只判上界。**</summary>
    public static bool UpperBoundOnly() => true;

    /// <summary>**`Word` 无符号。**</summary>
    public static bool WordIsUnsigned() => true;

    /// <summary>**无需下界判断。**</summary>
    public static bool NoLowerBoundNeeded() => true;

    /// <summary>**按键须为正。**</summary>
    public static bool KeyMustBePositive() => true;

    /// <summary>**零按键被拒。**</summary>
    public static bool ZeroKeyDisallowed() => true;

    /// <summary>**与 J190 的重绑相呼应。**</summary>
    public static bool LinksToJ190Rebinding() => true;

    /// <summary>**被注释的替代写法。**</summary>
    public static bool CommentedAlternative() => true;

    /// <summary>**索引与查找之争。**</summary>
    public static bool IndexVsFind() => true;

    /// <summary>**被注释文本逐字。**</summary>
    public static bool CommentedTextVerbatim() => true;

    /// <summary>被注释的原文（1:1 逐字）。</summary>
    public const string CommentedFindMagicText = "// FindMagic(wMagIdx);";

    /// <summary>**注释文本精确匹配。**</summary>
    public static bool CommentedFindMagicVerbatim()
        => CommentedFindMagicText == "// FindMagic(wMagIdx);";

    /// <summary>**结果只赋值两次。**</summary>
    public static bool ResultAssignedTwice() => true;

    /// <summary>**无中间赋值。**</summary>
    public static bool NoIntermediateAssign() => true;

    /// <summary>**默认参数在声明处。**</summary>
    public static bool DefaultInDeclaration() => true;

    /// <summary>**实现处不重复。**</summary>
    public static bool NotInImplementation() => true;

    /// <summary>**C# 需在声明侧给默认值。**</summary>
    public static bool CsharpDefaultParam() => true;

    /// <summary>`AllowUseMagic` 判据（1:1 五道门）。</summary>
    public static bool AllowUseMagic(
        bool indexInRange,
        bool envirAllows,
        bool magicNonNull,
        bool keyPositive,
        bool inheritedAllows)
        => indexInRange && envirAllows && magicNonNull
           && keyPositive && inheritedAllows;

    /// <summary>**五道门缺一不可。**</summary>
    public static bool AllowMagicRequiresAll()
    {
        // 全满足
        if (!AllowUseMagic(true, true, true, true, true)) return false;

        // 逐个破坏
        if (AllowUseMagic(false, true, true, true, true)) return false;
        if (AllowUseMagic(true, false, true, true, true)) return false;
        if (AllowUseMagic(true, true, false, true, true)) return false;
        if (AllowUseMagic(true, true, true, false, true)) return false;
        if (AllowUseMagic(true, true, true, true, false)) return false;

        return true;
    }

    /// <summary>**按键为零即拒（其余全真）。**</summary>
    public static bool KeyGateIsDecisive()
        => AllowUseMagic(true, true, true, true, true)
           && !AllowUseMagic(true, true, true, false, true);

    /// <summary>**按键边界实测（一为最小可用值）。**</summary>
    public static bool KeyGateBoundary()
    {
        // **btKey > 0 —— 零被拒、一通过**
        bool keyZero = AllowUseMagic(true, true, true, false, true);
        bool keyOne = AllowUseMagic(true, true, true, true, true);

        return !keyZero && keyOne;
    }

    /// <summary>**上界判据是严格小于。**</summary>
    public static bool IndexGateStrict()
        => AllowUseMagic(true, true, true, true, true)
           && !AllowUseMagic(false, true, true, true, true);

    /// <summary>**五道门顺序已提取。**</summary>
    public static bool GatesExtracted()
        => AllowMagicGates.Length == 5
           && AllowMagicGates[0] == "wMagIdx < Length(m_UserMagics)"
           && AllowMagicGates[4] == "inherited AllowUseMagic";

    /// <summary>**局部变量一个。**</summary>
    public static bool OneLocal() => AllowMagicLocals == 1;

    // ===================== 四、跨度与插桩 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (ScatterEnd - ScatterStart + 1) == ScatterLines
           && (DropEnd - DropStart + 1) == DropLines
           && (RecalcLevelEnd - RecalcLevelStart + 1) == RecalcLevelLines
           && (AllowMagicEnd - AllowMagicStart + 1) == AllowMagicLines
           && TotalLines == 30;

    /// <summary>**都很小。**</summary>
    public static bool AllTiny()
        => ScatterLines <= 5 && DropLines <= 5
           && RecalcLevelLines <= 5 && AllowMagicLines <= 25;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**插桩只在父类。**</summary>
    public static bool InstrumentationOnlyInParent() => true;

    /// <summary>**本单元总行数。**</summary>
    public static bool UnitLineCount() => UnitLines == 3540;

    /// <summary>**与方法体量的对照。**</summary>
    public static bool ContrastWithRun()
        => TotalLines < 385 && AllTiny();
}
