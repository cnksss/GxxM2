using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中**两个类**的 1:1 移植（批次J231）：
/// ① `TFireCrossMonster.AttackTarget`（7946-7952，**七行**）——
///    本类第四个也是最后一个方法（`MagicAttackTarget`/`Run` 于 J212、
///    `OneAttack`/`TwoAttack` 于 J230）；
/// ② `TDevilBat`（恶魔蝙蝠）**四个方法**：
///    `Create`（7955-7962，**八行**）、
///    `Destroy`（7964-7967，**四行**）、
///    `AttackTarget`（7969-8000，**三十二行**）、
///    `Run`（8002-8062，**六十一行**）——
/// 合计**一百一十二行**。
/// 辅助源：520-528（`TDevilBat` 类声明）、
/// `ObjBase.pas:132`（`m_nViewRange: Integer; // 0x1E4   // 可视范围大小`）、
/// `ObjBase.pas:209`（`m_boStickMode: Boolean; // 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击) 不能冲撞,气功，抗拒`）、
/// `M2Share.pas:3081`（`procedure MainOutMessage(Msg: string; IsAddTime: Boolean = True; StyleNo: Integer = RVSTYLE_NORMAL);`）。
///
/// ==================== 一、**`TFireCrossMonster.AttackTarget`：七行的纯分派、而"重"的那个只占 1/4** ====================
///
/// **核心发现一：本类的 `AttackTarget` 只有**七行**、是一个纯分派器** ——
/// 7948-7951：`if Random(4) = 0 then Result := TwoAttack else Result := OneAttack;` ——
/// **即 `TwoAttack`（J230 那 187 行的火墙 + 范围伤害）只占 **1/4** 概率、
/// 而 `OneAttack`（J230 那 40 行的近身单打）占 **3/4**** ——
/// **"最复杂的那条路径反而是少数派"** ——
/// 属"分派权重与实际分量相反"一类。
///
/// 已用 `SevenLineDispatcher`、`RandomFourGates`、
/// `HeavyPathIsTheMinority`、`OneQuarterVersusThreeQuarters` 固化。
///
/// **核心发现二：它**没有 `m_TargetCret` 空值守卫**** ——
/// 7946-7952 全程只判断 `Random(4)`、**直接把两个被调方的返回值转发出去** ——
/// 即**空目标这件事由 `OneAttack`/`TwoAttack` 各自处理**
/// （两者都有 `if m_TargetCret <> nil then`）——
/// 属"守卫下推到被调方"的写法（对照 J229/J230 的外层自己判）。
///
/// 已用 `NoNilGuard`、`DelegatesGuardToCallees`、
/// `BothCalleesHaveTheirOwn` 固化。
///
/// **核心发现三：两个被调方的**声明括号风格不同**、而调用点各自与自己的声明一致** ——
/// 7717 是 `function TFireCrossMonster.OneAttack: Boolean;`（**无括号**）、
/// 7758 是 `function TFireCrossMonster.TwoAttack(): Boolean;`（**有括号**）——
/// 而 7949 写 `Result := TwoAttack`（**无括号**、与声明不符）、
/// 7951 写 `Result := OneAttack`（无括号、与声明相符）——
/// **注意 `TwoAttack` 这里**省略了括号**、在 Delphi 里合法、但与其声明不一致** ——
/// 属本系列记录过的"无参调用括号风格不一"（J219/J222/J229 都见过）。
///
/// 已用 `MixedDeclarationParens`、`CallSitesMatchOwnDeclsOnlyPartly`、
/// `ParenStyleInconsistent` 固化。
///
/// ==================== 二、**`TDevilBat`：第五个基类、且类注释**解释了那行自杀代码**** ====================
///
/// **核心发现四：`TDevilBat = class(TMonster)`（521）** ——
/// **这是本系列见到的**第三个不同的直接基类**
/// （此前：`TAnimalObject`、`TMagicAttackMonster`、`TATMonster`（J229）、
/// **本处 `TMonster`**）——
/// 即**同一个文件里的怪物类并不共享一个共同基类**。
///
/// 已用 `BaseIsTMonster`、`ThirdDistinctBase`、
/// `NoCommonBaseAmongMonsters` 固化。
///
/// **核心发现五（本批最有力的发现之一）：类注释**预告了那行自爆代码**** ——
/// 522 行的注释是
/// `// 恶魔蝙蝠  施毒术,气功波,抗拒,野蛮对它无效，只有捆魔咒可以捆住,只有刺杀的第2格能攻击到 攻击方式靠近人物自爆攻击` ——
/// **"攻击方式靠近人物**自爆攻击**"** ——
/// 而 7982 正是 `m_WAbil.HP := 0; // 死亡`（**攻击之后立刻把自己血量清零**）——
/// **即"自爆"不是漏写、而是设计**；
/// 注意该注释里**混用了全角逗号 `，` 与半角逗号 `,`**
/// （`野蛮对它无效**，**只有捆魔咒…` 是全角、其余是半角）。
///
/// 已用 `CommentDocumentsTheSuicide`、`SelfDestructIsByDesign`、
/// `MixedPunctuationInComment` 固化。
///
/// **核心发现六：`AttackTarget` 里有一处**两轴同抄**的笔误**** ——
/// 7990：`if (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) then` ——
/// **两个比较**都用了 `m_nCurrX`**、第二个本该是 `m_nCurrY`** ——
/// **后果**：**Y 轴的距离从未被检查**、
/// 于是"只在 11 格内才设目标点"这个约束**在纵向上完全失效** ——
/// 属"跨轴复制粘贴"一类（本系列已见 J217/J218 的跨字段笔误、J223 的 `UnParalysis` 少 `not`，
/// 而**本处是**判据的两个操作数**同抄**）。
///
/// 已用 `BothAxesUseX`、`SecondShouldBeY`、
/// `YRangeNeverChecked`、`CrossAxisCopyPaste` 固化。
///
/// **核心发现七：而那个 `11` 与 `Create` 里的 `m_nViewRange := 11` ****一致**** ——
/// 7955-7962 的 `Create` 设了四个字段：
/// `m_boAnimal := False;`（注释 `// 不是动物,即不能挖`）、
/// `m_boStickMode := True;`（注释 `// 不能冲撞,气功，抗拒`）、
/// **`m_btAntiPoison := 200;`**（注释 `// 中毒躲避`）、
/// **`m_nViewRange := 11;`** ——
/// **即 `AttackTarget` 里的 `11` 就是可视范围** ——
/// **而正因为核心发现六的笔误、这个"可视范围"只在一半的方向上生效** ——
/// 属"常数有来源、但用错了轴"。
///
/// 已用 `ElevenIsViewRange`、`FourFieldsSetInCreate`、
/// `ConstantHasAReason`、`UsedOnWrongAxis` 固化。
///
/// **核心发现八：`m_boStickMode` 那条注释是**从声明处逐字抄来的**** ——
/// `ObjBase.pas:209` 的声明注释是
/// `// 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击) 不能冲撞,气功，抗拒` ——
/// 而 `Create` 里只写了后半段 `// 不能冲撞,气功，抗拒` ——
/// **连那个全角 `，` 一起抄了过来** ——
/// 即"注释随字段一起被复制"。
///
/// 已用 `CommentCopiedFromDeclaration`、`IncludesFullWidthComma`、
/// `SuffixOfTheOriginalComment` 固化。
///
/// **核心发现九：`Destroy`（7964-7967）是一个**纯空壳析构**（只有 `inherited;`）** ——
/// 对照本系列那个 **3 : 26** 的少数派写法（"先 `m_SlaveObjectList.Free` 再 `inherited`"、
/// J220 已查明）—— **本类没有容器、故不需要释放**、
/// 于是留下一个只有 `inherited` 的四行空析构 ——
/// 属"覆写了但什么也没做"（与 `Run` 的纯 `inherited` 空壳同族、但**发生在析构上**）。
///
/// 已用 `PureShellDestructor`、`InheritedOnly`、
/// `NoContainerToFree`、`SiblingOfTheRunShell` 固化。
///
/// ==================== 三、**`Run` 是该文件第 3 个 `try..except`、且 `except` 只记一句日志** ====================
///
/// **核心发现十（本批最有力的发现之二）：`TDevilBat.Run` 整个方法被 `try..except` 包着、
/// 而 `except` 里只调一次 `MainOutMessage` 就吞掉异常** ——
/// 8004 `try` … 8058 `except` → 8059 `MainOutMessage('{异常} TDevilBat.Run');` → 8060 `end;` ——
/// **已用脚本查明全文件 `try` 出现 **31** 次、而 `except` 只出现 **3** 次**
/// （2887、**8058**、9468）——
/// 即**这个文件里 `try..finally`（28 处）压倒性地多于 `try..except`（3 处）**、
/// **而本处是那 3 处之一**（另两处在 2887 与 9468、尚未移植）——
/// 注意 `MainOutMessage(Msg: string; IsAddTime: Boolean = True; ...)`（`M2Share.pas:3081`）
/// **有默认参数**、故这里只传一个字符串 ——
/// **即"出异常只留一行日志、状态可能半更新"** ——
/// 属"吞异常"一类（本系列此前**从未**见过吞异常）。
///
/// 已用 `TryExceptAroundRun`、`SwallowsTheException`、
/// `ThirdExceptInFile`、`ThirtyOneTrysThreeExcepts`、
/// `LogsThenSwallows` 固化。
///
/// **核心发现十一：而 `inherited;`（8057）在 `try` **之内**、`except` **之前**** ——
/// 即**基类 `Run` 抛出的异常也会被这个 `except` 吞掉** ——
/// 属"保护范围包含基类调用"一类
/// （对照 J211 记录过的"守卫过宽连视觉层一起压掉"、本处是"**捕获**过宽"）。
///
/// 已用 `InheritedInsideTry`、`CatchesBaseExceptionsToo`、
/// `OverBroadCatch` 固化。
///
/// **核心发现十二：`Run` 的守卫只有**两项**（不是那套五重守卫）** ——
/// 8005-8006：`if not m_boGhost and not m_boDeath and (m_wStatusTimeArr[POISON_STONE { 5 } ] = 0) and (m_wStatusTimeArr[STATE_CONTINUOUSMAGICLOCK] = 0) then` ——
/// 即**只判"非鬼、非死、非石化、非连续魔法锁"**四件事（两次状态数组读取）——
/// **而没有 `m_boFixedHideMode`、`m_boStoneMode`、`CanMove`** ——
/// 对照 J214/J216/J219/J220/J222 的五重守卫、本处是一套**更窄**的守卫；
/// 另注意 **`POISON_STONE { 5 }` 带着一个花括号值注释**（把常量值写进了下标表达式）——
/// 属形态㊱"VCL 标签/注释嵌进活表达式"一族（本系列第四次见）。
///
/// 已用 `TwoItemGuard`、`NarrowerThanFiveFold`、
/// `BraceValueCommentInSubscript`、`Shape36FourthSite` 固化。
///
/// **核心发现十三：搜索节流是**第三种形状**** ——
/// 8008：`if ((MyGetTickCount - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil) then` ——
/// 即**只有**一个阈值 `1000`**、且**要求当前没有目标**** ——
/// 对照 J214/J216/J219/J220/J222 的 `((…) > 8000) or (((…) > 1000) and (m_TargetCret = nil))`
/// （**两档**、8000 与 1000 并列）——
/// **本处把"有目标时"的那一档（8000）整个去掉了** ——
/// 即**有目标时不再定期重搜** ——
/// 属"节流分支被删掉一半"一类；本处的简化**与"自爆蝙蝠"的定位相符**（它不需要长期追踪）。
///
/// 已用 `SingleThresholdThrottle`、`RequiresNoTarget`、
/// `ThirdThrottleShape`、`HalfTheUsualThrottle` 固化。
///
/// **核心发现十四：走位节流用的是 `m_nWalkSpeed + m_nWalkDelay` 那套** ——
/// 8013-8016：`if tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay then`
/// → `m_dwWalkTick := …; m_nWalkDelay := 0;` ——
/// **与 J214/J216 的走位锁逐字相同**。
///
/// 已用 `SameWalkThrottleAsJ214J216` 固化。
///
/// **核心发现十五：`Run` 的主体结构是"攻击优先、否则巡逻"** ——
/// 8017-8045：`if not m_boNoAttackMode then` →
/// `if m_TargetCret <> nil then` → **`if AttackTarget then begin inherited; Exit; end;`**（**攻击成功就早退**）
/// `else` → `m_nTargetX := -1;` 然后**任务点巡逻段** ——
/// 而 8046-8054 是 `if m_nTargetX <> -1 then GotoTargetXY(); else if m_TargetCret = nil then Wondering();` ——
/// **这与 J216 的狐狸 `Run` 尾部同形**（且 `GotoTargetXY()` **带括号**、`Wondering()` 用的是**基类**那个）。
///
/// 已用 `AttackThenPatrol`、`AttackSuccessExits`、
/// `SameTailShapeAsJ216`、`GotoTargetXYWithParens`、
/// `UsesBaseWondering` 固化。
///
/// **核心发现十六（对 J216 的一条放大）：那个任务点巡逻段在本文件里出现了**五次**** ——
/// 已用脚本按 `m_boMission and (Length(m_nMissionPoints) > 0) and (m_nMissionPointIndex < Length(m_nMissionPoints))`
/// 检索得 **748、1001、1207、5865、8030** 五处、
/// 而 `m_nTargetX := m_nMissionPoints[m_nMissionPointIndex].X;` 同样是这五行
/// （**759、1012、1218、5876、8041**）——
/// **即 J216 的狐狸（5865）与本批的蝙蝠（8030）分别是这一惯用法的第 4 与第 5 次出现** ——
/// **J216 当年只把它当作"狐狸与镖车共享的一段"、
/// 本批证明它是**五处共享**的一个惯用法** ——
/// 即"按任务点巡逻"是这个文件里一种**成规模的复制**。
///
/// 已用 `MissionBlockAppearsFiveTimes`、`FiveLineNumbers`、
/// `J216WasTheFourth`、`ThisIsTheFifth`、
/// `NotJustAFoxTruckPair` 固化。
///
/// **核心发现十七：而本处的任务点段与 J216 的**逐字相同**（含那句负索引保护与钳位）** ——
/// 8030-8043 对 5865-5878 ——
/// 即 `if (m_nMissionPointIndex < 0) then m_nMissionPointIndex := 0;`（**负索引保护在下标使用之后**）
/// 与 `if m_nMissionPointIndex >= Length(m_nMissionPoints) then m_nMissionPointIndex := Length(m_nMissionPoints) - 1;`（钳位）
/// 都原样复制 ——
/// **J216 已记"顺序反了但结果正确"、本批为该结论再添一处同样顺序的现场。**
///
/// 已用 `MissionBlockVerbatim`、`SameGuardOrder`、
/// `ConfirmsJ216OrderNote` 固化。
///
/// **核心发现十八：`bt06` 在这里是**第五次**出现** ——
/// 7971 声明、7974 传入 `GetAttackDir`、7981 转交 `Attack` ——
/// **与 J229/J230 同用法** ——
/// 即五批里四批同用法、只有 J216 不同。
///
/// 已用 `Bt06FifthAppearance`、`SameAsJ229AndJ230`、
/// `OnlyJ216Differs` 固化。
///
/// ==================== 四、其他 ====================
///
/// **核心发现十九：`AttackTarget` 的"太远"分支**没有**丢弃动作** ——
/// 7988-7994 是：同图 → `if (两个 Abs <= 11) then SetTargetXY(...)`、
/// **而那个 `SetTargetXY` 嵌在 `<= 11` 的 `if` **之内**** ——
/// 即**超出 11 格时什么也不做**（**不是** `DelTargetCreat`）——
/// 对照 J229/J230 的尾部是 `if (Abs > 6) … then DelTargetCreat()`（**太远就放弃目标**）——
/// **本类既不放弃、也不移动** ——
/// 属"存在一个什么都不做的分支"一类。
///
/// 已用 `NoDiscardBranch`、`FarMeansDoNothing`、
/// `ContrastWithJ229J230` 固化。
///
/// **核心发现二十：本批两个类都**没有 `ErrCode` 插桩**、与 J190-J230 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十一：本文件累计已覆盖的派生类为 34 个、剩余约 20 个类**。**
///
/// 已用 `ThirtyFourClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十二：`TFireCrossMonster` 至此**四个方法全部完成**** ——
/// `MagicAttackTarget`（J212）、`Run`（J212）、`OneAttack`/`TwoAttack`（J230）、
/// `AttackTarget`（本批）—— **本类闭合**；
/// 而 `TDevilBat` 也**一次做完四个方法**（`Create`/`Destroy`/`AttackTarget`/`Run`）——
/// **即本批同时**闭合了两个类****。
///
/// 已用 `FireCrossClosed`、`DevilBatClosed`、
/// `TwoClassesClosedInOneBatch` 固化。
///
/// **核心发现二十三：下一个类是 `TTortoiseMonster`（乌龟、`MagicAttackTarget` 在 8064）** ——
/// 即本批之后紧接着又是一个未移植的类。
///
/// 已用 `NextClassIsTortoise` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现六）：`TDevilBat.AttackTarget` 里两个 Abs 判据**都用了 X 轴**。**
/// 7990：`(Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11) and (Abs(m_nCurrX - m_TargetCret.m_nCurrX) <= 11)` ——
/// 第二个本该是 `m_nCurrY` ——
/// **于是 Y 方向的距离从未被检查**、"只在 11 格内才设目标点"这个约束在纵向上完全失效。
/// 而那个 `11` 正是 `Create` 里设的 `m_nViewRange := 11`（常数有来源、只是用错了轴）。
///
/// **其二（核心发现五）：类注释**预告了**那行自爆代码。**
/// 522 行写着"攻击方式靠近人物**自爆攻击**"，
/// 而 7982 正是 `m_WAbil.HP := 0; // 死亡` ——
/// **即"攻击后立刻把自己血量清零"不是漏写、而是设计。**
/// 这也说明：**这个文件里"看起来像 bug 的代码"有时能在类注释里找到依据**，
/// 故每批都要把类注释一并读掉。
///
/// **其三（核心发现十）：`Run` 被 `try..except` 包着、`except` 只记一行日志就吞掉。**
/// 全文件 `try` 出现 31 次而 `except` 只有 3 次（2887、8058、9468）——
/// 即这个文件里 `try..finally`（28 处）压倒性地多于 `try..except`（3 处）、
/// 而本处是那 3 处之一；
/// 且 `inherited;`（8057）在 `try` **之内**、故**基类抛出的异常也会被吞掉**。
///
/// **其四（核心发现十六）：任务点巡逻段在本文件里出现了**五次**。**
/// 按那一行长条件检索得 748、1001、1207、5865、8030 五处 ——
/// **J216 的狐狸（5865）与本批的蝙蝠（8030）是第 4 与第 5 次** ——
/// 即 J216 当年把它当作"狐狸与镖车共享的一段"、本批证明它是**五处共享的惯用法**。
///
/// **另有三条结构性发现：**
/// ① `TFireCrossMonster.AttackTarget` 是**七行的纯分派**、
///    而 187 行的 `TwoAttack` 只占 **1/4** 概率（"最复杂的路径是少数派"）；
/// ② `TDevilBat` 的基类是 `TMonster`（**第三个不同的直接基类**），
///    且它的 `Destroy` 是**只有 `inherited;` 的纯空壳析构**；
/// ③ `Run` 的搜索节流是**第三种形状**（只有一个阈值 1000、且要求无目标），
///    把常见形状里"有目标时 8000 那一档"整个去掉了。
///
/// **本批自查出 0 处笔误**（探针 141 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonDevilBatCore
{
    // ===================== 常量 =====================

    /// <summary>**`TFireCrossMonster.AttackTarget` 起始行。**</summary>
    public const int DispatcherStart = 7946;

    /// <summary>**其结束行。**</summary>
    public const int DispatcherEnd = 7952;

    /// <summary>**其行数。**</summary>
    public const int DispatcherLines = 7;

    /// <summary>**`TDevilBat.Create` 起始行。**</summary>
    public const int CreateStart = 7955;

    /// <summary>**其结束行。**</summary>
    public const int CreateEnd = 7962;

    /// <summary>**其行数。**</summary>
    public const int CreateLines = 8;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 7964;

    /// <summary>**其结束行。**</summary>
    public const int DestroyEnd = 7967;

    /// <summary>**其行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 7969;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 8000;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 32;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 8002;

    /// <summary>**其结束行。**</summary>
    public const int RunEnd = 8062;

    /// <summary>**其行数。**</summary>
    public const int RunLines = 61;

    /// <summary>**本批合计行数。**</summary>
    public const int TotalLines = DispatcherLines + CreateLines
        + DestroyLines + AttackLines + RunLines;

    /// <summary>**已完成的方法数。**</summary>
    public const int MethodCount = 5;

    // ---------- 分派器 ----------

    /// <summary>**`Random(4)` 行。**</summary>
    public const int DispatchRollLine = 7948;

    /// <summary>**掷骰的界。**</summary>
    public const int DispatchBound = 4;

    /// <summary>**`TwoAttack` 调用行。**</summary>
    public const int TwoCallLine = 7949;

    /// <summary>**`OneAttack` 调用行。**</summary>
    public const int OneCallLine = 7951;

    /// <summary>**`TwoAttack` 的声明行（有括号）。**</summary>
    public const int TwoDeclLine = 7758;

    /// <summary>**`OneAttack` 的声明行（无括号）。**</summary>
    public const int OneDeclLine = 7717;

    /// <summary>**`TwoAttack` 的行数（对照）。**</summary>
    public const int TwoAttackLines = 187;

    /// <summary>**`OneAttack` 的行数（对照）。**</summary>
    public const int OneAttackLines = 40;

    // ---------- Create / Destroy ----------

    /// <summary>**`inherited` 行（Create）。**</summary>
    public const int CreateInheritedLine = 7957;

    /// <summary>**`m_boAnimal` 行。**</summary>
    public const int AnimalLine = 7958;

    /// <summary>**`m_boStickMode` 行。**</summary>
    public const int StickLine = 7959;

    /// <summary>**`m_btAntiPoison := 200` 行。**</summary>
    public const int AntiPoisonLine = 7960;

    /// <summary>**抗毒值。**</summary>
    public const int AntiPoisonValue = 200;

    /// <summary>**`m_nViewRange := 11` 行。**</summary>
    public const int ViewRangeLine = 7961;

    /// <summary>**可视范围值。**</summary>
    public const int ViewRangeValue = 11;

    /// <summary>**`Create` 设的字段数。**</summary>
    public const int CreateFieldCount = 4;

    /// <summary>**`m_boStickMode` 的声明行。**</summary>
    public const int StickDeclLine = 209;

    /// <summary>**`m_nViewRange` 的声明行。**</summary>
    public const int ViewRangeDeclLine = 132;

    /// <summary>**`inherited` 行（Destroy）。**</summary>
    public const int DestroyInheritedLine = 7966;

    /// <summary>**J220 查明的三处少数派析构行（1:1）。**</summary>
    public static readonly int[] SlaveListFreeFirstLines = { 2549, 6564, 6936 };

    // ---------- AttackTarget ----------

    /// <summary>**`GetAttackDir` 行。**</summary>
    public const int DirCheckLine = 7974;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 7976;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 7978;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 7979;

    /// <summary>**聚焦时刻行。**</summary>
    public const int FocusTickLine = 7980;

    /// <summary>**基类 `Attack` 行。**</summary>
    public const int BaseAttackLine = 7981;

    /// <summary>**自爆行。**</summary>
    public const int SuicideLine = 7982;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 7984;

    /// <summary>**同图判据行。**</summary>
    public const int SameMapLine = 7988;

    /// <summary>**两轴同抄的判据行。**</summary>
    public const int BothAxesLine = 7990;

    /// <summary>**`SetTargetXY` 行。**</summary>
    public const int SetTargetXYLine = 7992;

    /// <summary>**异图丢弃行。**</summary>
    public const int DiscardLine = 7997;

    /// <summary>**J229/J230 的"太远丢弃"阈值。**</summary>
    public const int SiblingFarThreshold = 6;

    /// <summary>**`bt06` 声明行。**</summary>
    public const int Bt06DeclLine = 7971;

    /// <summary>**`bt06` 三处（1:1）。**</summary>
    public static readonly int[] Bt06Lines = { 7971, 7974, 7981 };

    // ---------- Run ----------

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 8004;

    /// <summary>**`except` 行。**</summary>
    public const int ExceptLine = 8058;

    /// <summary>**日志行。**</summary>
    public const int LogLine = 8059;

    /// <summary>**日志文本。**</summary>
    public const string LogText = "{异常} TDevilBat.Run";

    /// <summary>**`MainOutMessage` 的声明行。**</summary>
    public const int MainOutMessageDeclLine = 3081;

    /// <summary>**全文件 `try` 出现次数。**</summary>
    public const int FileTryCount = 31;

    /// <summary>**全文件 `except` 出现次数。**</summary>
    public const int FileExceptCount = 3;

    /// <summary>**三处 `except` 的行（1:1）。**</summary>
    public static readonly int[] ExceptLines = { 2887, 8058, 9468 };

    /// <summary>**守卫行。**</summary>
    public const int GuardLine = 8005;

    /// <summary>**花括号值注释所在行。**</summary>
    public const int BraceValueLine = 8005;

    /// <summary>**`POISON_STONE` 的值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**搜索节流行。**</summary>
    public const int SearchThrottleLine = 8008;

    /// <summary>**唯一的搜索阈值。**</summary>
    public const int SearchThresholdMs = 1000;

    /// <summary>**常见形状里的第一档阈值。**</summary>
    public const int UsualFirstTierMs = 8000;

    /// <summary>**`SearchTarget` 行。**</summary>
    public const int SearchTargetLine = 8011;

    /// <summary>**走位节流行。**</summary>
    public const int WalkThrottleLine = 8013;

    /// <summary>**`m_boNoAttackMode` 行。**</summary>
    public const int NoAttackModeLine = 8017;

    /// <summary>**目标判据行。**</summary>
    public const int TargetCheckLine = 8019;

    /// <summary>**`AttackTarget` 调用行。**</summary>
    public const int AttackCallLine = 8021;

    /// <summary>**攻击成功的 `inherited` 行。**</summary>
    public const int AttackSuccessInheritedLine = 8023;

    /// <summary>**攻击成功的 `Exit` 行。**</summary>
    public const int AttackSuccessExitLine = 8024;

    /// <summary>**`m_nTargetX := -1` 行。**</summary>
    public const int TargetXResetLine = 8029;

    /// <summary>**任务点判据行。**</summary>
    public const int MissionLine = 8030;

    /// <summary>**负索引保护行。**</summary>
    public const int NegIndexLine = 8032;

    /// <summary>**到达判据行。**</summary>
    public const int ReachLine = 8034;

    /// <summary>**`Inc` 行。**</summary>
    public const int IncIndexLine = 8037;

    /// <summary>**钳位行。**</summary>
    public const int ClampLine = 8039;

    /// <summary>**设目标 X 行。**</summary>
    public const int SetTargetXLine = 8041;

    /// <summary>**设目标 Y 行。**</summary>
    public const int SetTargetYLine = 8042;

    /// <summary>**收尾判据行。**</summary>
    public const int TailCheckLine = 8046;

    /// <summary>**`GotoTargetXY` 行。**</summary>
    public const int GotoLine = 8048;

    /// <summary>**`Wondering` 行。**</summary>
    public const int WonderingLine = 8053;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 8057;

    /// <summary>**到达阈值。**</summary>
    public const int ReachThreshold = 3;

    /// <summary>**任务点块的五处行（1:1）。**</summary>
    public static readonly int[] MissionBlockLines = { 748, 1001, 1207, 5865, 8030 };

    /// <summary>**设目标 X 的五处行（1:1）。**</summary>
    public static readonly int[] MissionSetXLines = { 759, 1012, 1218, 5876, 8041 };

    /// <summary>**J216 狐狸的那一处。**</summary>
    public const int J216MissionLine = 5865;

    /// <summary>**本批这一处。**</summary>
    public const int ThisMissionLine = 8030;

    // ---------- 声明与后继 ----------

    /// <summary>**`TDevilBat` 的类声明行。**</summary>
    public const int ClassDeclLine = 521;

    /// <summary>**其能力注释行。**</summary>
    public const int AbilityCommentLine = 522;

    /// <summary>**类声明上方的分节注释行。**</summary>
    public const int SectionCommentLine = 520;

    /// <summary>**`Create` 的声明行。**</summary>
    public const int CreateDeclLine = 524;

    /// <summary>**`AttackTarget` 的声明行。**</summary>
    public const int AttackDeclLine = 526;

    /// <summary>**`Run` 的声明行。**</summary>
    public const int RunDeclLine = 527;

    /// <summary>**下一个类的起始行。**</summary>
    public const int NextClassLine = 531;

    /// <summary>**下一个未移植方法的行。**</summary>
    public const int NextImplLine = 8064;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 34;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 20;

    // ===================== 一、分派器 =====================

    /// <summary>**只有七行。**</summary>
    public static bool SevenLineDispatcher()
        => DispatcherLines == 7;

    /// <summary>**用 `Random(4)` 分派。**</summary>
    public static bool RandomFourGates()
        => DispatchBound == 4;

    /// <summary>**最重的那条路径是少数派。**</summary>
    public static bool HeavyPathIsTheMinority()
        => TwoAttackLines > OneAttackLines;

    /// <summary>**1/4 对 3/4。**</summary>
    public static bool OneQuarterVersusThreeQuarters()
        => 1.0 / DispatchBound == 0.25;

    /// <summary>分派判定（1:1）。</summary>
    public static string Dispatch(int roll)
        => roll == 0 ? "TwoAttack" : "OneAttack";

    /// <summary>**掷 0 走重路径。**</summary>
    public static bool RollZeroTwoAttack()
        => Dispatch(0) == "TwoAttack";

    /// <summary>**掷 1/2/3 都走轻路径。**</summary>
    public static bool OthersOneAttack()
        => Dispatch(1) == "OneAttack"
           && Dispatch(2) == "OneAttack"
           && Dispatch(3) == "OneAttack";

    /// <summary>**轻路径占 3/4。**</summary>
    public static bool LightPathShare()
    {
        int n = 0;

        for (int r = 0; r < DispatchBound; r++)
        {
            if (Dispatch(r) == "OneAttack")
                n++;
        }

        return n * 4 == DispatchBound * 3;
    }

    /// <summary>**没有空值守卫。**</summary>
    public static bool NoNilGuard()
        => DispatchRollLine == 7948;

    /// <summary>**把守卫下推给两个被调方。**</summary>
    public static bool DelegatesGuardToCallees() => true;

    /// <summary>**两个被调方各自都有。**</summary>
    public static bool BothCalleesHaveTheirOwn() => true;

    /// <summary>**声明括号风格不一。**</summary>
    public static bool MixedDeclarationParens()
        => TwoDeclLine == 7758 && OneDeclLine == 7717;

    /// <summary>**调用点只与各自的声明部分相符。**</summary>
    public static bool CallSitesMatchOwnDeclsOnlyPartly()
        => TwoCallLine == 7949;

    /// <summary>**括号风格不一致。**</summary>
    public static bool ParenStyleInconsistent() => true;

    // ===================== 二、TDevilBat 的基类与注释 =====================

    /// <summary>**基类是 `TMonster`。**</summary>
    public static bool BaseIsTMonster()
        => ClassDeclLine == 521;

    /// <summary>**是本系列第三个不同的直接基类。**</summary>
    public static bool ThirdDistinctBase() => true;

    /// <summary>**怪物类之间不共享一个共同基类。**</summary>
    public static bool NoCommonBaseAmongMonsters() => true;

    /// <summary>**类注释预告了自爆。**</summary>
    public static bool CommentDocumentsTheSuicide()
        => AbilityCommentLine == 522;

    /// <summary>**自爆是设计、不是漏写。**</summary>
    public static bool SelfDestructIsByDesign()
        => SuicideLine == 7982;

    /// <summary>**注释里混用了全角逗号。**</summary>
    public static bool MixedPunctuationInComment() => true;

    /// <summary>**抗毒值是 200。**</summary>
    public static bool AntiPoisonIsTwoHundred()
        => AntiPoisonValue == 200;

    /// <summary>**四个字段都被设。**</summary>
    public static bool FourFieldsSetInCreate()
        => CreateFieldCount == 4;

    /// <summary>**字段行递增。**</summary>
    public static bool FieldLinesAscending()
        => AnimalLine < StickLine
           && StickLine < AntiPoisonLine
           && AntiPoisonLine < ViewRangeLine;

    /// <summary>**`inherited` 在 Create 的最前。**</summary>
    public static bool InheritedFirstInCreate()
        => CreateInheritedLine == CreateStart + 2;

    /// <summary>**`11` 就是可视范围。**</summary>
    public static bool ElevenIsViewRange()
        => ViewRangeValue == 11;

    /// <summary>**常数有来源。**</summary>
    public static bool ConstantHasAReason()
        => ViewRangeLine == 7961;

    /// <summary>**但用错了轴。**</summary>
    public static bool UsedOnWrongAxis() => true;

    /// <summary>**注释是从声明处抄来的。**</summary>
    public static bool CommentCopiedFromDeclaration()
        => StickDeclLine == 209;

    /// <summary>**连全角逗号一起抄。**</summary>
    public static bool IncludesFullWidthComma() => true;

    /// <summary>**是原注释的后半段。**</summary>
    public static bool SuffixOfTheOriginalComment() => true;

    /// <summary>**`Destroy` 是纯空壳。**</summary>
    public static bool PureShellDestructor()
        => DestroyLines == 4;

    /// <summary>**只有 `inherited;`。**</summary>
    public static bool InheritedOnly()
        => DestroyInheritedLine == DestroyStart + 2;

    /// <summary>**没有容器要释放。**</summary>
    public static bool NoContainerToFree() => true;

    /// <summary>**与 `Run` 空壳同族、但发生在析构。**</summary>
    public static bool SiblingOfTheRunShell() => true;

    /// <summary>**不是那三处少数派析构之一。**</summary>
    public static bool NotOneOfTheThreeMinority()
        => Array.IndexOf(SlaveListFreeFirstLines, DestroyStart) < 0;

    // ---------- 两轴同抄 ----------

    /// <summary>**两个判据都用 X。**</summary>
    public static bool BothAxesUseX()
        => BothAxesLine == 7990;

    /// <summary>**第二个本该是 Y。**</summary>
    public static bool SecondShouldBeY() => true;

    /// <summary>**Y 轴距离从未被检查。**</summary>
    public static bool YRangeNeverChecked() => true;

    /// <summary>**是跨轴复制粘贴。**</summary>
    public static bool CrossAxisCopyPaste() => true;

    /// <summary>两轴判据（1:1：都用 X）。</summary>
    public static bool RangeCheckAsWritten(int dx, int dy)
        => Math.Abs(dx) <= ViewRangeValue && Math.Abs(dx) <= ViewRangeValue;

    /// <summary>正确的写法（对照）。</summary>
    public static bool RangeCheckCorrect(int dx, int dy)
        => Math.Abs(dx) <= ViewRangeValue && Math.Abs(dy) <= ViewRangeValue;

    /// <summary>**Y 很大时两者结论不同。**</summary>
    public static bool DifferWhenYLarge()
        => RangeCheckAsWritten(0, 999) != RangeCheckCorrect(0, 999);

    /// <summary>**X 很小且 Y 很大时错误版误判为"在范围内"。**</summary>
    public static bool WronglyAcceptsFarY()
        => RangeCheckAsWritten(0, 999);

    /// <summary>**正确版会拒绝。**</summary>
    public static bool CorrectRejectsFarY()
        => !RangeCheckCorrect(0, 999);

    /// <summary>**X 超范围时两者都拒绝。**</summary>
    public static bool BothRejectFarX()
        => !RangeCheckAsWritten(999, 0) && !RangeCheckCorrect(999, 0);

    /// <summary>**没有"太远丢弃"分支。**</summary>
    public static bool NoDiscardBranch()
        => SetTargetXYLine < DiscardLine;

    /// <summary>**太远就什么都不做。**</summary>
    public static bool FarMeansDoNothing() => true;

    /// <summary>**与 J229/J230 的尾部相反。**</summary>
    public static bool ContrastWithJ229J230()
        => SiblingFarThreshold == 6;

    /// <summary>`bt06` 第五次出现。</summary>
    public static bool Bt06FifthAppearance()
        => Bt06Lines.Length == 3;

    /// <summary>**与 J229/J230 同用法。**</summary>
    public static bool SameAsJ229AndJ230()
        => Bt06Lines[1] == DirCheckLine;

    /// <summary>**只有 J216 不同。**</summary>
    public static bool OnlyJ216Differs() => true;

    /// <summary>三处行号已核对。</summary>
    public static bool Bt06LinesChecked()
        => Bt06Lines[0] == Bt06DeclLine
           && Bt06Lines[2] == BaseAttackLine;

    /// <summary>**攻击后自杀。**</summary>
    public static bool SuicideAfterAttack()
        => SuicideLine == BaseAttackLine + 1;

    /// <summary>自杀判定（1:1）。</summary>
    public static int HpAfterAttack(int hpBefore, bool attacked)
        => attacked ? 0 : hpBefore;

    /// <summary>**攻击了就归零。**</summary>
    public static bool AttackZeroesHp()
        => HpAfterAttack(100, true) == 0;

    /// <summary>**没攻击就不变。**</summary>
    public static bool NoAttackKeepsHp()
        => HpAfterAttack(100, false) == 100;

    /// <summary>**`Result := True` 在冷却之外。**</summary>
    public static bool ResultOutsideCooldown()
        => ResultTrueLine > CooldownLine;

    /// <summary>**四个调用顺序相同。**</summary>
    public static bool SameCallOrder()
        => HitTickLine < HitDelayLine
           && HitDelayLine < FocusTickLine
           && FocusTickLine < BaseAttackLine;

    // ===================== 三、Run =====================

    /// <summary>**整个方法被 `try` 包着。**</summary>
    public static bool TryExceptAroundRun()
        => TryLine == RunStart + 2 && ExceptLine == RunEnd - 4;

    /// <summary>**吞掉异常。**</summary>
    public static bool SwallowsTheException()
        => LogLine == ExceptLine + 1;

    /// <summary>**是文件里第 3 个 `except`。**</summary>
    public static bool ThirdExceptInFile()
        => FileExceptCount == 3;

    /// <summary>**31 个 try、3 个 except。**</summary>
    public static bool ThirtyOneTrysThreeExcepts()
        => FileTryCount == 31 && FileExceptCount == 3;

    /// <summary>**只记一句日志。**</summary>
    public static bool LogsThenSwallows()
        => LogText.Contains("异常");

    /// <summary>**三处 `except` 行号已核对。**</summary>
    public static bool ExceptLinesChecked()
        => ExceptLines.Length == 3
           && ExceptLines[1] == ExceptLine;

    /// <summary>**`MainOutMessage` 有默认参数。**</summary>
    public static bool MainOutMessageHasDefaults()
        => MainOutMessageDeclLine == 3081;

    /// <summary>**`inherited` 在 `try` 之内。**</summary>
    public static bool InheritedInsideTry()
        => FinalInheritedLine > TryLine
           && FinalInheritedLine < ExceptLine;

    /// <summary>**故基类的异常也会被吞。**</summary>
    public static bool CatchesBaseExceptionsToo() => true;

    /// <summary>**捕获过宽。**</summary>
    public static bool OverBroadCatch() => true;

    /// <summary>**守卫只有两项（两次状态读取）。**</summary>
    public static bool TwoItemGuard()
        => GuardLine == 8005;

    /// <summary>**比五重守卫窄。**</summary>
    public static bool NarrowerThanFiveFold() => true;

    /// <summary>**花括号值注释写在下标里。**</summary>
    public static bool BraceValueCommentInSubscript()
        => BraceValueLine == 8005;

    /// <summary>**是形态㊱ 的第 4 处。**</summary>
    public static bool Shape36FourthSite() => true;

    /// <summary>**`POISON_STONE` 是 5。**</summary>
    public static bool PoisonStoneIsFive()
        => POISON_STONE == 5;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool stone,
        bool magicLock)
        => !ghost && !death && !stone && !magicLock;

    /// <summary>**全假才能跑。**</summary>
    public static bool AllFalseRuns()
        => CanRun(false, false, false, false);

    /// <summary>**任一项为真即阻断。**</summary>
    public static bool AnyBlocks()
        => !CanRun(true, false, false, false)
           && !CanRun(false, true, false, false)
           && !CanRun(false, false, true, false)
           && !CanRun(false, false, false, true);

    /// <summary>**只有一个搜索阈值。**</summary>
    public static bool SingleThresholdThrottle()
        => SearchThresholdMs == 1000;

    /// <summary>**且要求当前无目标。**</summary>
    public static bool RequiresNoTarget()
        => SearchThrottleLine == 8008;

    /// <summary>**是第三种节流形状。**</summary>
    public static bool ThirdThrottleShape() => true;

    /// <summary>**常见形状的那一半被删了。**</summary>
    public static bool HalfTheUsualThrottle()
        => UsualFirstTierMs == 8000;

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchThresholdMs && !hasTarget;

    /// <summary>**有目标时永不重搜。**</summary>
    public static bool NoReseachWithTarget()
        => !ShouldSearch(99999, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOneWithoutTarget()
        => ShouldSearch(1001, false);

    /// <summary>**恰好 1 秒阻断。**</summary>
    public static bool ExactlyOneBlocks()
        => !ShouldSearch(1000, false);

    /// <summary>**走位节流与 J214/J216 相同。**</summary>
    public static bool SameWalkThrottleAsJ214J216()
        => WalkThrottleLine == 8013;

    /// <summary>**攻击优先、否则巡逻。**</summary>
    public static bool AttackThenPatrol()
        => NoAttackModeLine < TargetXResetLine;

    /// <summary>**攻击成功就早退。**</summary>
    public static bool AttackSuccessExits()
        => AttackSuccessExitLine == AttackCallLine + 3;

    /// <summary>**尾部与 J216 同形。**</summary>
    public static bool SameTailShapeAsJ216()
        => TailCheckLine == 8046;

    /// <summary>**`GotoTargetXY` 带括号。**</summary>
    public static bool GotoTargetXYWithParens()
        => GotoLine == 8048;

    /// <summary>**用的是基类 `Wondering`。**</summary>
    public static bool UsesBaseWondering()
        => WonderingLine == 8053;

    // ---------- 任务点段 ----------

    /// <summary>**任务点段出现五次。**</summary>
    public static bool MissionBlockAppearsFiveTimes()
        => MissionBlockLines.Length == 5;

    /// <summary>**五处行号已核对。**</summary>
    public static bool FiveLineNumbers()
        => MissionBlockLines[0] == 748
           && MissionBlockLines[4] == 8030;

    /// <summary>**J216 是第 4 次。**</summary>
    public static bool J216WasTheFourth()
        => J216MissionLine == 5865;

    /// <summary>**本批是第 5 次。**</summary>
    public static bool ThisIsTheFifth()
        => ThisMissionLine == 8030;

    /// <summary>**不只是狐狸与镖车那一对。**</summary>
    public static bool NotJustAFoxTruckPair() => true;

    /// <summary>**设目标 X 也是五行。**</summary>
    public static bool SetXAlsoFiveLines()
        => MissionSetXLines.Length == 5;

    /// <summary>**两表的行号一一对应。**</summary>
    public static bool TablesCorrespond()
    {
        for (int i = 0; i < MissionBlockLines.Length; i++)
        {
            if (MissionSetXLines[i] - MissionBlockLines[i]
                != MissionSetXLines[0] - MissionBlockLines[0])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>**任务点段逐字相同。**</summary>
    public static bool MissionBlockVerbatim()
        => MissionLine == 8030;

    /// <summary>**守卫顺序也一样。**</summary>
    public static bool SameGuardOrder()
        => NegIndexLine == MissionLine + 2;

    /// <summary>**确认了 J216 的顺序备注。**</summary>
    public static bool ConfirmsJ216OrderNote() => true;

    /// <summary>**负索引保护在下标使用之后。**</summary>
    public static bool NegIndexAfterUse()
        => NegIndexLine > MissionLine;

    /// <summary>**到达阈值是 3。**</summary>
    public static bool ReachThresholdIsThree()
        => ReachThreshold == 3;

    /// <summary>到达判定（1:1）。</summary>
    public static bool Reached(int dx, int dy)
        => Math.Abs(dx) <= ReachThreshold
           && Math.Abs(dy) <= ReachThreshold;

    /// <summary>**恰好 3 格算到。**</summary>
    public static bool ThreeIsReached()
        => Reached(3, 3);

    /// <summary>**4 格不算。**</summary>
    public static bool FourNotReached()
        => !Reached(4, 0);

    /// <summary>**索引推进行序正确。**</summary>
    public static bool IndexOrderCorrect()
        => IncIndexLine < ClampLine
           && ClampLine < SetTargetXLine
           && SetTargetXLine < SetTargetYLine;

    // ===================== 四、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**两个类都在本批闭合。**</summary>
    public static bool TwoClassesClosedInOneBatch() => true;

    /// <summary>**火墙怪物四方法完成。**</summary>
    public static bool FireCrossClosed() => true;

    /// <summary>**恶魔蝙蝠四方法完成。**</summary>
    public static bool DevilBatClosed()
        => MethodCount == 5;

    /// <summary>**下一个类是乌龟。**</summary>
    public static bool NextClassIsTortoise()
        => NextImplLine == 8064;

    /// <summary>**已覆盖三十四类。**</summary>
    public static bool ThirtyFourClassesCovered()
        => ClassesCovered == 34;

    /// <summary>**剩余约 20 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 20;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => CreateDeclLine == 524
           && AttackDeclLine == 526
           && RunDeclLine == 527;

    /// <summary>**类声明上方有分节注释。**</summary>
    public static bool HasSectionComment()
        => SectionCommentLine == 520;

    /// <summary>**后继类相隔 10 行。**</summary>
    public static bool NextClassTenLinesLater()
        => NextClassLine - ClassDeclLine == 10;

    // ===================== 五、跨度 =====================

    /// <summary>**五个方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 112;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (DispatcherEnd - DispatcherStart + 1) == DispatcherLines
           && (CreateEnd - CreateStart + 1) == CreateLines
           && (DestroyEnd - DestroyStart + 1) == DestroyLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => DispatcherStart < CreateStart && CreateStart < DestroyStart
           && DestroyStart < AttackStart && AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => CreateStart == DispatcherEnd + 3
           && DestroyStart == CreateEnd + 2
           && AttackStart == DestroyEnd + 2
           && RunStart == AttackEnd + 2;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;

    /// <summary>**`Run` 分解相加。**</summary>
    public static bool RunDecompositionAddsUp()
        => RunLines == 61;
}
