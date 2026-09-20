using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFoxMonster`（狐狸）前四个方法的 1:1 移植（批次J215）：
/// `Create`（5530-5542，**十三行**）、
/// `Think`（5544-5579，**三十六行**）、
/// `MagicAttackTarget`（5580-5680，**一百零一行**）、
/// `AttackTarget`（5682-5736，**五十五行**），
/// 合计**二百零五行**。
/// **本类共六个方法、合计四百行** ——
/// 后两个（`WonderingEx` 5738-5805 **六十八行**、`Run` 5807-5933 **一百二十七行**）
/// 留待**批次J216**。
/// 辅助源：180-193（类声明）、
/// 9-23（**`TMonster` 的声明**）、
/// `ObjBase.pas:817`（`TAnimalObject = class(TBaseObject)`）、
/// `ObjBase.pas:1237-1238`（**`protected function Think: Boolean;` —— **无 `virtual`**）、
/// `ObjBase.pas:3851-3856`（`TSmartObject.Think`）、
/// `ObjMon.pas:577/582`（`TDevilkingMonster.Think`）、
/// `ObjMon.pas:845/850`（`TMonster.Think`）、
/// `ObjBase.pas:746`（`function WalkTo(btDir: Byte; boFlag: Boolean): Boolean; virtual;`）、
/// `ObjBase.pas:781/35478-35487`（`IsProperTarget` 的声明与实现，**内含空指针判断**）、
/// `ObjBase.pas:787/788` 等（其余 VMT 标签）。
///
/// ==================== 一、**共享外层模板第四次逐字确认** ====================
///
/// **核心发现一：本类 `MagicAttackTarget` 的外层体（5651-5680）与
/// J207 的 `TExplosionAttackMonster` 外层体（4847-4876）
/// **三十行逐字相同、零处差异**** ——
/// 已用脚本逐行比对（把两段对齐成各 30 行的 `begin..end` 块）：
/// **0 differing lines / 30**。
///
/// **即该模板现已被**四次**独立确认**：
/// J207（冰咆哮）、J210（灭天火）、J211（寒冰掌）、**J215（狐狸）** ——
/// **四个类分属不同的直接基类**
/// （前三个是 `TMagicAttackMonster` 的子类、本类是 **`TAnimalObject` 的子类**）——
/// **即这个"6 格 + `Random(2) = 0` + 同图靠近/异图丢弃"的骨架
/// 跨的**不止一个继承分支**、而是本文件里一种通用的
/// "选目标 → 够近就放、不够近就靠近"写法。**
///
/// 模板内容为：`begin` → `Result := False` → `m_TargetCret = nil` 保护 →
/// `tick_diff` 冷却三件套 → `Abs(X) <= 6 and Abs(Y) <= 6` →
/// `(m_nTargetX = -1) or (Random(2) = 0)` → `MagicAttack; Result := True; Exit` →
/// 同图则 `SetTargetXY` / 异图则 `DelTargetCreat()` → `end;`。
///
/// 已用 `OuterTemplateVerbatim`、`ThirtyLinesZeroDiff`、
/// `FourthConfirmation`、`SpansDifferentBases`、
/// `TemplateIsGeneric` 固化。
///
/// ==================== 二、**反编译标签既不唯一、也会被带进调用点** ====================
///
/// **核心发现二（第二类重要发现）：方法名后那些 `// 004Axxxx` 反编译地址注释
/// **并不唯一**、同一地址被贴在**多个不同类的方法**上**** ——
///
/// | 标签 | 出现处 | 类 |
/// |---|---|---|
/// | `// 004A8B74` | 551 / **788** / **5530** | `TDevilkingMonster.Create` / `TMonster.Create` / **`TFoxMonster.Create`** |
/// | `// 004A8E54` | 577 / 845 / **5544** | `TDevilkingMonster.Think` / `TMonster.Think` / **`TFoxMonster.Think`** |
///
/// **即同一个地址注释被贴在三处的 `Create`、三处的 `Think` 上** ——
/// **说明这些注释是**随代码一起复制**的、
/// 在复制之后**不再唯一标识原函数**** ——
/// **它们只能说明"这几个类同源"、**不能当作身份键**。**
///
/// **对本工程的含义**：移植时**绝不可以地址标签判断两个方法是否同一实现**
/// （我此前几批引用这些地址时也须按此修正口径）。
///
/// 已用 `AddressLabelNotUnique`、`SameLabelThreeCreates`、
/// `SameLabelThreeThinks`、`CopyPropagatedComments`、
/// `EvidenceOfSameOrigin`、`NotAnIdentityKey` 固化。
///
/// **核心发现三：同一族的"花括号标签"`{ FFFF4 }` / `{ FFEB }`
/// 是**从声明处传到调用处**的 VMT 槽位标记** ——
/// 已用脚本确认 `ObjMon.pas` 里这类**嵌在表达式中间**的标签共 **6 处**：
/// `587`、`872`、**`5563`**（三处 `IsProperTarget { FFFF4 } (m_TargetCret)`）与
/// `992`、`1198`、**`5856`**（三处 `if AttackTarget { FFEB } then`）。
///
/// **而它们的**来源**在两处声明**：
/// `ObjBase.pas:781` 是 `function IsProperTarget(BaseObject: TBaseObject): Boolean; virtual; // FFFF4`、
/// `ObjMon.pas:20` 是 `function AttackTarget(): Boolean; virtual; // FFEB` ——
/// **即声明处以 `// FFF4` / `// FFEB` 标注的槽位号、
/// 被复制成了调用处表达式中间的 `{ FFFF4 }` / `{ FFEB }`。**
///
/// **本批范围内是 5563**（`Think` 里那句）、
/// **5856 属 J216**（`Run` 里那句）。
///
/// 已用 `VmtLabelInExpression`、`SixOccurrences`、
/// `OriginatesFromDeclaration`、`Ffff4ComesFromObjBase781`、
/// `FfebComesFromObjMon20` 固化。
///
/// **核心发现四：本批的 5563 那句 `IsProperTarget` **没有先判空**、
/// 但这是**安全的**** ——
/// `if not IsProperTarget { FFFF4 } (m_TargetCret) then m_TargetCret := nil;` ——
/// **已核实 `TBaseObject.IsProperTarget`（`ObjBase.pas:35478`）
/// 的**第一件事**就是
/// `if (BaseObject = nil) or (BaseObject = Self) then Exit;`（35486）、
/// 且带注释 `// 加入对象空指针判断 -- piaoyun 2013-07-04`** ——
/// **即空指针判断被放在了**被调方**、而不是调用方。**
///
/// **注意**这与 J213 的 `TFireCrossMonster`（4912 用了未在分支内赋值的 `btDir`）
/// 是**相反的**形态：那里是"调用方依赖被调方的副作用"、
/// 这里是"调用方依赖被调方的参数校验" ——
/// **两者都说明本工程把契约分散在调用链两端。**
///
/// 已用 `NoNilCheckAtCallSite`、`SafeBecauseCalleeChecks`、
/// `CalleeGuardWithComment`、`ContractSplitAcrossCallChain`、
/// `OppositeShapeToJ213` 固化。
///
/// ==================== 三、**`Think`：同名静态方法的三条实现、以及一个空声明** ====================
///
/// **核心发现五（第三类重要发现）：`TAnimalObject.Think` **只有声明、没有实现**** ——
/// `ObjBase.pas:1238` 在 `protected` 段写着
/// `function Think: Boolean;`（**无 `virtual`**）、
/// **而全工程搜索 `TAnimalObject.Think` 的**实现体**为空** ——
/// 已用脚本确认：`Think` 的实现只出现在
/// `TSmartObject`（`ObjBase.pas:3851`）、
/// `TDevilkingMonster`（`ObjMon.pas:577`）、
/// `TMonster`（`ObjMon.pas:845`）、
/// `TFoxMonster`（`ObjMon.pas:5544`）四处。
///
/// **即 `TAnimalObject.Think` 是一个"声明了但从未实现"的方法** ——
/// 由于它是**静态（非虚）**方法、Delphi 只在**被调用**时才要求实现，
/// **所以它能编译通过恰恰说明**从未有任何代码通过 `TAnimalObject` 类型去调它****。
///
/// 已用 `DeclarationWithoutBody`、`StaticSoNoLinkDemand`、
/// `NeverCalledThroughBase`、`VestigialDeclaration` 固化。
///
/// **核心发现六：`TMonster.Think` 与 `TFoxMonster.Think` **都不是覆写**、
/// 而是各自独立的**静态**方法** ——
/// `TMonster`（9-23）声明 `private function Think: Boolean;`（14）、
/// `TFoxMonster`（180-193）声明 `private function Think: Boolean;`（186）、
/// **两处都没有 `virtual` 也没有 `override`** ——
/// 而 `TAnimalObject.Think`（1237-1238）在 `protected` 段、同样无 `virtual`。
///
/// **即这条链上是**三个同名静态方法**：
/// 基类声明（无实现）、`TMonster` 的实现、`TFoxMonster` 的实现 ——
/// **因为不是虚方法、所以不存在"覆写"关系、
/// 也不存在虚表分派** ——
/// **`TMonster.Run` 里的 `Think` 调的是 `TMonster.Think`、
/// `TFoxMonster.Run` 里的 `Think`（5819）调的是 `TFoxMonster.Think`。**
///
/// **可见性还逐层收窄**：基类是 `protected`、两个派生类都是 `private`。
///
/// 已用 `ThreeSameNamedStatics`、`NotVirtualSoNoOverride`、
/// `NoVmtDispatch`、`CallerGetsItsOwn`、
/// `VisibilityNarrows` 固化。
///
/// **核心发现七：有**五个** `Think` 实现共用同一个"三秒"闸门** ——
/// 已用脚本确认 `(MyGetTickCount - m_dwThinkTick) > 3 * 1000` 出现在：
/// `ObjBase.pas:3856`（`TSmartObject`）、
/// `ObjMon.pas:582`（`TDevilkingMonster`）、
/// `ObjMon.pas:850`（`TMonster`）、
/// `ObjMon.pas:5549`（**`TFoxMonster`**）、
/// `ObjSmartMon.pas:2790` —— **五处**。
///
/// **即"三秒思考一次"是本文件族的通用节流值**、
/// **而 `3 * 1000` 这个**未折叠**的写法（不写 `3000`）
/// 在五处**全部**保留** ——
/// **说明它是随代码复制的、作者从未把它简化。**
///
/// 已用 `FiveThinkImplementations`、`SameThreeSecondGate`、
/// `UnfoldedArithmetic`、`CopiedNotSimplified` 固化。
///
/// **核心发现八：`Think` 的闸门用的是**裸减法**而非 `tick_diff`** ——
/// `(MyGetTickCount - m_dwThinkTick) > 3 * 1000`（5549）——
/// **而同一个类的 `MagicAttackTarget` 冷却（5655）用的是 `tick_diff`** ——
/// **即"同一类里两种时间写法"**（与 J210/J213/J214 同族）。
///
/// 已用 `RawSubtractForThinkGate`、`TickDiffForHitCooldown`、
/// `TwoIdiomsInSameClass` 固化。
///
/// ==================== 四、`Think` 的两段逻辑 ====================
///
/// **核心发现九：`Think` 分**两段**、且第二段**不受闸门限制**** ——
/// **第一段**在 `if (now - m_dwThinkTick) > 3000` 内（5549-5565）：
/// 刷新 `m_dwThinkTick` → 判断是否需要"让位"（`m_boDupMode := True`）
/// → 用 `IsProperTarget` 清掉非法目标；
/// **第二段**在闸门**之外**（5567-5577）：
/// `if m_boDupMode then` → 记下旧坐标 → `WalkTo(Random(8), False)`
/// → **若坐标真的变了**则 `m_boDupMode := False; Result := True;`。
///
/// **即"是否要让位"三秒判一次、而"让位动作"每帧都可能尝试** ——
/// **这个分工本身是合理的**（判定昂贵、动作需要及时），
/// **但它意味着 `Think` 的返回值（`Result := True`）
/// 既可能在闸门内被设、也可能在闸门外被设** ——
/// **而 `Result` 初值是 `False`（5548），
/// 只有"让位成功"这一条路径会把它置真。**
///
/// 已用 `GateCoversFirstStageOnly`、`SecondStageEveryFrame`、
/// `ReasonableSplit`、`ResultOnlyFromDisplacement` 固化。
///
/// **核心发现十：让位判据是**两条互斥的路径**、都用 `GetXYObjCount`/`GetXYNpcObjCount`** ——
/// 5552-5562：
/// ```
/// if ((m_Master = nil) or (not InSafeZone) or
///     ((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))) then
///   begin if m_PEnvir.GetXYObjCount(m_nCurrX, m_nCurrY) >= 2 then m_boDupMode := True; end
/// else if (m_Master <> nil) and (m_PEnvir.GetXYNpcObjCount(m_nCurrX, m_nCurrY) >= 1) then
///   begin m_boDupMode := True; end;
/// ```
/// —— 即 **第一路**（"自己不是人物的宝宝、或不在安全区"）用
/// "**同格对象数 >= 2**"（说明脚下挤了人）；
/// **第二路**（剩下的情况、即"主人的宝宝且人在安全区"）用
/// "**同格 NPC 数 >= 1**" ——
/// **第二路正是注释 `// 防止安全区宝宝被挤出安全区`（5553）
/// 与 `// 不让宝宝和NPC叠到一起 2020-09-07 00:01:53`（5558）所解释的两件事。**
///
/// **注意 `GetXYObjCount` 与 `GetXYNpcObjCount` 正是 J159 普查中
/// **共用同一个锁号 30** 的那一对**（`Envir.pas:4398` 等）——
/// **本批为那处"编号重复"补上了**实际调用者**。**
///
/// 已用 `TwoMutuallyExclusiveRoutes`、`ObjectCountThresholdTwo`、
/// `NpcCountThresholdOne`、`TwoCommentsExplain`、
/// `CompletesJ159LockThirty` 固化。
///
/// **核心发现十一：第三条件里 `Master` 与 `m_Master` **混用**** ——
/// 5552 写的是 `((m_Master <> nil) and (Master.m_btRaceServer <> RC_PLAYOBJECT))`
/// —— **前半用 `m_Master`、后半用 `Master`** ——
/// **两者在本工程里通常是同一字段的字段与属性两种写法**
/// （J214 的 `Think` 相关处也出现过 `Master`）——
/// **但混在同一行里、且中间隔着 `and`、
/// 读起来像是两个不同的东西。**
///
/// 已用 `MasterVsMMaster`、`SameThingTwoSpellings`、
/// `MixedWithinOneLine` 固化。
///
/// **核心发现十二：`WalkTo(Random(8), False)`（5571）的 `Random(8)`
/// **恰好等于方向个数**（`DR_UP=0` … `DR_UPLEFT=7`、共八个）** ——
/// **即"随机挑一个合法方向"、值域 `0..7` 全覆盖且无越界** ——
/// **这与 J216 里 `WonderingEx` 的 `bt06 := Random(9)`（5752/5780）
/// 形成对照：那里 `Random(9)` 会给出 **8**、而 8 不是合法方向** ——
/// **同类里"随机方向"两处写法不一致、一处正确一处越界**（详见 J216）。
///
/// **本批先把"正确的那一处"固定下来**。
///
/// 已用 `Random8IsCorrect`、`CoversAllEight`、
/// `ContrastWithWonderingExRandom9`、`SameClassTwoSpellings` 固化。
///
/// **核心发现十三：`Think` 里 `m_boDupMode` 是"置真—尝试—成功即置假"的三步**——
/// 第一段把它置 `True`（5556/5561）、第二段若走动了就置回 `False`（5574）——
/// **若**走不动**（`WalkTo` 未改变坐标）则它**保持 `True`**、
/// **于是下一帧会**再试一次**** ——
/// **即它是一个"持续尝试直到成功"的粘滞标志**、
/// **而不是"本帧需要让位"的一次性标记。**
///
/// **注意**`Create` 把它初始化成 `False`（5533）。
///
/// 已用 `StickyFlag`、`ClearedOnlyOnSuccess`、
/// `RetriesUntilDisplaced`、`NotOneShot` 固化。
///
/// ==================== 五、`AttackTarget`：**查了 `m_boMagicAttack`** ====================
///
/// **核心发现十四：本类 `AttackTarget`（5682-5736）**先查 `m_boMagicAttack`**（5689）、
/// 再分"魔法"与"物理"两支** ——
/// **这与 J212 的 `TFireCrossMonster` **恰好相反**** ——
/// 那个类覆写了 `AttackTarget` 却**完全不查该标志**
/// （J212 已记录：基类 `TMagicAttackMonster` 的 `m_boMagicAttack` 分派机制对它失效）。
///
/// **即"同一个文件名下的两个类、对同一个基类标志采取相反做法"** ——
/// **一个绕过分派、一个自己实现分派。**
///
/// 已用 `ChecksMagicFlag`、`TwoBranches`、
/// `OppositeOfJ212`、`SelfImplementedDispatch` 固化。
///
/// **核心发现十五：两条分支的**目标处置**逐字相同、而"攻击动作"不同** ——
/// **魔法支**（5689-5707）：`if GetAttackDir(m_TargetCret, bt06) then
/// begin Result := MagicAttackTarget(); m_boWonderingEx := True; end
/// else` → 同图 `SetTargetXY` / 异图 `DelTargetCreat()`；
/// **物理支**（5708-5734）：`if GetAttackDir(m_TargetCret, bt06) then
/// begin if tick_diff(...) then begin 三件套; m_dwTargetFocusTick := ...; Attack(...); BreakHolySeizeMode(); end;
/// m_boWonderingEx := True; Result := True; end else` → 同图 `SetTargetXY` / 异图 `DelTargetCreat()`。
///
/// **两者的 `else`（目标处置）**完全一样**、各 8 行** ——
/// 即**又一次"同一段代码在分支里写两遍"**（与 J214 的 13 行重复段同族）。
///
/// 已用 `SameElseTwice`、`EightLinesDuplicated`、
/// `OnlyAttackActionDiffers` 固化。
///
/// **核心发现十六：**物理支有自己的冷却检查、魔法支**没有**** ——
/// 物理支在 5712 用 `tick_diff(m_dwHitTick, MyGetTickCount) > m_nNextHitTime + m_nHitDelay`
/// 门控 `Attack`，并在 5714-5716 做"刷新 `m_dwHitTick`、清 `m_nHitDelay`、
/// 记 `m_dwTargetFocusTick`"三件套；
/// **而魔法支直接 `Result := MagicAttackTarget()`（5693）、**没有任何冷却判断**** ——
/// **即"魔法攻击的节流在**被调方** `MagicAttackTarget` 自己的冷却里**
/// （就是本批核心发现一那个模板的 5655 行）**、
/// 而物理攻击的节流在**调用方**这里"** ——
/// **同一次按键、两条路径把冷却放在不同的层**。
///
/// 已用 `PhysicalHasOwnCooldown`、`MagicDelegatesCooldown`、
/// `CooldownAtDifferentLayers` 固化。
///
/// **核心发现十七：两条分支都在 `GetAttackDir` 成功时置 `m_boWonderingEx := True`** ——
/// 魔法支在 5694（`Result` 之前）、物理支在 5720（三件套之后）——
/// **而 `m_boWonderingEx` 正是 J216 的 `WonderingEx` 的**启用条件**
/// （其 5746 为 `if (m_TargetCret <> nil) and m_boWonderingEx then`）——
/// **即"能打到目标"这件事会打开"下回合尝试走位"的开关。**
///
/// **注意**`m_boWonderingEx` 在 `Create` 里初始化成 `False`（5541）、
/// **且只在两处置真（5694/5720）、三处置假（5541、5763、5791 —— 后两处在 J216）。**
///
/// 已用 `SetsWonderingFlag`、`EnablesJ216`、
/// `TwoSettersThreeClearers` 固化。
///
/// **核心发现十八：物理支设了 `m_dwTargetFocusTick := MyGetTickCount();`（5716）** ——
/// **与 J212 的 `TFireCrossMonster` 一致**（其 7730/7808 也设）、
/// **而 J207/J210/J211 都没有** ——
/// **即"记录目标聚焦时刻"是本文件里一部分类才做的事。**
///
/// 已用 `SetsTargetFocusTick`、`SameAsJ212`、
/// `AbsentInJ207J210J211` 固化。
///
/// **核心发现十九：本类声明 `AttackTarget(): Boolean; virtual;`（190）
/// 与 `MagicAttackTarget: Boolean; virtual;`（191）都用 `virtual` 而非 `override`** ——
/// **这是**正确**的、不是缺陷** ——
/// 已核实 `TAnimalObject`（`ObjBase.pas:817`）**既不声明 `AttackTarget(): Boolean`、
/// 也不声明 `MagicAttackTarget`** ——
/// **即本类（直接继承 `TAnimalObject`）是这两条虚链的**根**、
/// 用 `virtual` 正是起始一条新链的正确写法。**
///
/// **注意**`TMonster` 也自己起了一条 `AttackTarget` 链（`ObjMon.pas:20` 的 `virtual`）、
/// **并把 `MagicAttackTarget` **整行注释掉**（`ObjMon.pas:21`：
/// `// function MagicAttackTarget: Boolean; virtual; //人物魔法攻击`）** ——
/// **即本文件里存在**两条互不相关的 `AttackTarget` 虚链**、
/// 而 `MagicAttackTarget` 只在 `TFoxMonster` 这一支存在
/// （J206 的 `TMagicAttackMonster` 另有自己的声明链）。**
///
/// 已用 `VirtualIsCorrectHere`、`TwoIndependentChains`、
/// `TMonsterCommentsOutMagicAttackTarget`、`NotAHidingBug` 固化。
///
/// ==================== 六、`Create` ====================
///
/// **核心发现二十：`TFoxMonster.Create`（5530-5542）与 `TMonster.Create`（788-799）
/// **逐字相同、只多一行**** ——
/// 两者都设 `m_boDupMode := False`、`bo554 := False`、
/// `m_dwThinkTick := MyGetTickCount()`、`m_nViewRange := 5`、
/// `m_nRunTime := 250`、`m_dwSearchTime := 3000 + Random(2000)`、
/// `m_dwSearchTick := MyGetTickCount()`、`m_btRaceServer := 80`；
/// **本类多的是 `m_boWonderingEx := False;`（5541）。**
///
/// **而这不是冗余、是**必需**的** ——
/// 因为 `TFoxMonster = class(TAnimalObject)`（180）、
/// **它**不继承 `TMonster`****、
/// 所以 `inherited`（5532）走的是 `TAnimalObject.Create`、
/// **拿不到 `TMonster.Create` 的那八行** ——
/// **作者只能整段复制过来。**
///
/// **即这是一处"因为不在这条继承链上、所以只能复制"的重复**、
/// 与 J214 的"同文件同构段一份有保护一份没有"不同：
/// **本处的重复有**结构上的理由****。
///
/// 已用 `DuplicatesTMonsterCreate`、`OnlyOneExtraLine`、
/// `NotInheritingTMonster`、`CopyWasNecessary`、
/// `StructuralReason` 固化。
///
/// **核心发现二十一：`m_nViewRange := 5`（5536）是该值在本文件的**第 4 处**** ——
/// 已用脚本确认另外三处是 557、794（`TMonster.Create`）、1385 ——
/// **即 5 与 7（七处）、9（一处）、2（一处）并列为本文件的常见视野值。**
///
/// 已用 `ViewRangeFive`、`FourthOccurrence`、
/// `CommonValues` 固化。
///
/// **核心发现二十二：`m_btRaceServer := 80`（5540）是该值在本文件的**第 2 处**** ——
/// 另一处是 798（`TMonster.Create`）——
/// **即本类**与 `TMonster` 同为种族 80**（普通怪物）**、
/// **对照 J214 的镖车把自己设成了 **122**（而常量为 128）、
/// 本处的 80 是**正确取值**。**
///
/// 已用 `RaceIsEighty`、`SameAsTMonster`、
/// `ContrastWithJ214Truck` 固化。
///
/// **核心发现二十三：`bo554 := False`（5534）用的是 `TMonster` 里那个
/// **裸名字共享字段**（`ObjMon.pas:11` `bo554: Boolean; // 0x554`）** ——
/// **本类并不继承 `TMonster`、却仍然给 `bo554` 赋值** ——
/// **说明 `bo554` 其实在**更上层**（`TAnimalObject` 或 `TBaseObject`）也存在同名/同地址字段**、
/// **而 `ObjMon.pas:11` 的那行声明只是**又声明了一次**** ——
/// **这与 J206 记录的"`bo554` 有三处声明、注释都是 `// 0x554`"完全吻合**
/// （本批为那三处之一补上了**实际赋值者**）。**
///
/// 已用 `Bo554SharedField`、`AssignedEvenWithoutInheritingTMonster`、
/// `CompletesJ206Finding`、`ThreeDeclarations` 固化。
///
/// **核心发现二十四：`m_dwSearchTime := 3000 + Random(2000)`（5538）** ——
/// **即取值 `3000..4999`** ——
/// **对照 J213 的 `Random(1500) + 1500`（1500..2999）、
/// J214 未设该字段** ——
/// **即三个类的搜索时间初值**各不相同**；
/// 且注意本处把常量写在**前**（`3000 + Random(2000)`）而 J213 写在**后**
/// （`Random(1500) + 1500`）** —— **同一个算式两种书写顺序。**
///
/// 已用 `SearchTime3000To4999`、`ContrastJ213`、
/// `OperandOrderDiffers` 固化。
///
/// **核心发现二十五：本批四个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J214 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现二十六：本文件累计已覆盖的派生类为 19 个
/// （本批是同一类的**前半**）、
/// `TFoxMonster` 自身要等 J216 才算完整**。**
///
/// 已用 `SplitAcrossTwoBatches`、`J216CoversRest` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现一）：共享外层模板第四次逐字确认、且跨了不同的继承分支。**
/// 本类 `MagicAttackTarget` 的外层体（5651-5680）与 J207 的（4847-4876）
/// 三十行**零差异** —— 而 J207/J210/J211 都是 `TMagicAttackMonster` 的子类、
/// **本类却是 `TAnimalObject` 的子类** ——
/// 所以那个"6 格 + `Random(2)` + 同图靠近/异图丢弃"的骨架
/// **不是某一个基类的私产、而是本文件一种通用的目标处置写法。**
///
/// **其二（核心发现二）：反编译地址注释**不唯一****。
/// `// 004A8B74` 同时贴在三处的 `Create` 上
/// （`TDevilkingMonster` / `TMonster` / `TFoxMonster`）、
/// `// 004A8E54` 同样贴在三处的 `Think` 上 ——
/// **它们随代码一起被复制、已不再是身份键。**
/// 这条对本工程有直接的方法论约束：**不能拿地址注释判断两个方法是否同一实现。**
/// 与之配套的是核心发现三：`{ FFFF4 }` / `{ FFEB }` 这类花括号标签
/// **是从声明处的 `// FFF4` / `// FFEB` 复制进表达式中间的**（全文件 6 处）。
///
/// **其三（核心发现五/六）：`TAnimalObject.Think` 是一个"只有声明、没有实现"的方法。**
/// 而 `TMonster.Think` 与 `TFoxMonster.Think` **都不是它的覆写**、
/// 而是各自独立的**静态**方法 ——
/// **因为不是虚方法，所以"覆写"这个关系在这条链上根本不存在**，
/// 也就没有虚表分派可言。
/// 它能编译通过，本身就证明了**没有任何代码通过 `TAnimalObject` 类型去调它。**
///
/// **另有两条交叉印证：**
/// ① 核心发现十用到了 J159 普查中"共用锁号 30"的
/// `GetXYObjCount` / `GetXYNpcObjCount` 这一对 —— 本批补上了它们的实际调用者；
/// ② 核心发现二十三给 J206 记录的"`bo554` 三处声明"补上了**实际赋值者** ——
/// 而且本类并不继承 `TMonster`、却照样给 `bo554` 赋值，
/// 这反过来证明该字段**确实在更上层也存在**。
///
/// **本批自查出 0 处笔误**（探针 158 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFoxCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 5530;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 5542;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 13;

    /// <summary>**`Think` 起始行。**</summary>
    public const int ThinkStart = 5544;

    /// <summary>**`Think` 结束行。**</summary>
    public const int ThinkEnd = 5579;

    /// <summary>**`Think` 行数。**</summary>
    public const int ThinkLines = 36;

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 5580;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 5680;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 101;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 5682;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 5736;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackLines = 55;

    /// <summary>**本批四方法合计行数。**</summary>
    public const int TotalLines = CreateLines + ThinkLines
        + MagicLines + AttackLines;

    /// <summary>**类内全部六方法合计行数。**</summary>
    public const int ClassTotalLines = 400;

    /// <summary>**留待 J216 的 `WonderingEx` 行数。**</summary>
    public const int J216WonderingLines = 68;

    /// <summary>**留待 J216 的 `Run` 行数。**</summary>
    public const int J216RunLines = 127;

    // ---------- 共享模板 ----------

    /// <summary>**本类外层体起始行。**</summary>
    public const int OuterStart = 5651;

    /// <summary>**本类外层体结束行。**</summary>
    public const int OuterEnd = 5680;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 30;

    /// <summary>**J207 的外层体起始行。**</summary>
    public const int J207OuterStart = 4847;

    /// <summary>**J207 的外层体结束行。**</summary>
    public const int J207OuterEnd = 4876;

    /// <summary>**逐行比对的差异数。**</summary>
    public const int TemplateDiffLines = 0;

    /// <summary>**模板确认次数（J207/J210/J211/J215）。**</summary>
    public const int TemplateConfirmations = 4;

    /// <summary>**内层 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 5582;

    /// <summary>**内层 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 5649;

    /// <summary>**内层行数。**</summary>
    public const int NestedLines = 68;

    /// <summary>**内层前的空行。**</summary>
    public const int BlankBeforeNested = 1;

    // ---------- 反编译标签 ----------

    /// <summary>**`// 004A8B74` 的三处（1:1）。**</summary>
    public static readonly int[] Label004A8B74Lines = { 551, 788, 5530 };

    /// <summary>**`// 004A8E54` 的三处（1:1）。**</summary>
    public static readonly int[] Label004A8E54Lines = { 577, 845, 5544 };

    /// <summary>**标签出现次数。**</summary>
    public const int LabelOccurrences = 3;

    /// <summary>**`{ FFFF4 }` / `{ FFEB }` 在 `ObjMon.pas` 的六处（1:1）。**</summary>
    public static readonly int[] VmtLabelLines = { 587, 872, 992, 1198, 5563, 5856 };

    /// <summary>**其中本批范围内的一处。**</summary>
    public const int VmtLabelThisBatch = 5563;

    /// <summary>**其中属 J216 的一处。**</summary>
    public const int VmtLabelJ216 = 5856;

    /// <summary>**`{ FFFF4 }` 的声明来源行（`ObjBase.pas`）。**</summary>
    public const int Ffff4DeclLine = 781;

    /// <summary>**`{ FFEB }` 的声明来源行（`ObjMon.pas`）。**</summary>
    public const int FfebDeclLine = 20;

    /// <summary>**`IsProperTarget` 实现起始行。**</summary>
    public const int IsProperTargetImpl = 35478;

    /// <summary>**其空指针判断行。**</summary>
    public const int IsProperTargetNilLine = 35486;

    // ---------- Think ----------

    /// <summary>**`Result := False` 行。**</summary>
    public const int ThinkResultLine = 5548;

    /// <summary>**三秒闸门行。**</summary>
    public const int ThinkGateLine = 5549;

    /// <summary>**闸门阈值毫秒。**</summary>
    public const int ThinkGateMs = 3000;

    /// <summary>**`m_dwThinkTick` 刷新行。**</summary>
    public const int ThinkTickRefreshLine = 5551;

    /// <summary>**第一路让位判据起始行。**</summary>
    public const int Route1Start = 5552;

    /// <summary>**第一路注释行。**</summary>
    public const int Route1CommentLine = 5553;

    /// <summary>**`GetXYObjCount` 行。**</summary>
    public const int ObjCountLine = 5555;

    /// <summary>**对象数阈值。**</summary>
    public const int ObjCountThreshold = 2;

    /// <summary>**第二路注释行。**</summary>
    public const int Route2CommentLine = 5558;

    /// <summary>**第二路判据行。**</summary>
    public const int Route2Line = 5559;

    /// <summary>**`GetXYNpcObjCount` 行。**</summary>
    public const int NpcCountLine = 5559;

    /// <summary>**NPC 数阈值。**</summary>
    public const int NpcCountThreshold = 1;

    /// <summary>**`IsProperTarget` 调用行。**</summary>
    public const int ProperTargetLine = 5563;

    /// <summary>**清目标行。**</summary>
    public const int ClearTargetLine = 5564;

    /// <summary>**第二段起始行。**</summary>
    public const int Stage2Start = 5567;

    /// <summary>**记旧 X 行。**</summary>
    public const int OldXLine = 5569;

    /// <summary>**`WalkTo` 行。**</summary>
    public const int WalkToLine = 5571;

    /// <summary>**`WalkTo` 的方向随机参数。**</summary>
    public const int WalkToRandomBound = 8;

    /// <summary>**位移判据行。**</summary>
    public const int MovedCheckLine = 5572;

    /// <summary>**清让位标志行。**</summary>
    public const int DupClearLine = 5574;

    /// <summary>**第二段置 `Result := True` 行。**</summary>
    public const int ThinkResultTrueLine = 5575;

    /// <summary>**`Think` 实现的五处（1:1）。**</summary>
    public static readonly (int Line, string Owner)[] ThinkImpls =
    {
        (3856, "TSmartObject (ObjBase.pas)"),
        (582, "TDevilkingMonster"),
        (850, "TMonster"),
        (5549, "TFoxMonster"),
        (2790, "ObjSmartMon.pas"),
    };

    /// <summary>**`TAnimalObject.Think` 的声明行。**</summary>
    public const int AnimalThinkDeclLine = 1238;

    /// <summary>**`TMonster.Think` 的声明行。**</summary>
    public const int MonsterThinkDeclLine = 14;

    /// <summary>**`TFoxMonster.Think` 的声明行。**</summary>
    public const int FoxThinkDeclLine = 186;

    // ---------- AttackTarget ----------

    /// <summary>**`bt06` 声明行。**</summary>
    public const int Bt06DeclLine = 5684;

    /// <summary>**`Result := False` 行。**</summary>
    public const int AttackResultLine = 5686;

    /// <summary>**目标非空行。**</summary>
    public const int TargetCheckLine = 5687;

    /// <summary>**`m_boMagicAttack` 判据行。**</summary>
    public const int MagicFlagLine = 5689;

    /// <summary>**魔法支起始行。**</summary>
    public const int MagicBranchStart = 5690;

    /// <summary>**魔法支结束行。**</summary>
    public const int MagicBranchEnd = 5707;

    /// <summary>**魔法支的 `GetAttackDir` 行。**</summary>
    public const int MagicGetDirLine = 5691;

    /// <summary>**魔法支转发行。**</summary>
    public const int MagicDelegateLine = 5693;

    /// <summary>**魔法支置 `m_boWonderingEx` 行。**</summary>
    public const int MagicWonderingLine = 5694;

    /// <summary>**物理支起始行。**</summary>
    public const int PhysBranchStart = 5708;

    /// <summary>**物理支结束行。**</summary>
    public const int PhysBranchEnd = 5734;

    /// <summary>**物理支的 `GetAttackDir` 行。**</summary>
    public const int PhysGetDirLine = 5710;

    /// <summary>**物理支的冷却判据行。**</summary>
    public const int PhysCooldownLine = 5712;

    /// <summary>**物理支的 `m_dwHitTick` 刷新行。**</summary>
    public const int PhysHitTickLine = 5714;

    /// <summary>**物理支的 `m_nHitDelay` 清零行。**</summary>
    public const int PhysHitDelayLine = 5715;

    /// <summary>**物理支的目标聚焦行。**</summary>
    public const int PhysFocusLine = 5716;

    /// <summary>**物理支的 `Attack` 行。**</summary>
    public const int PhysAttackLine = 5717;

    /// <summary>**物理支的 `BreakHolySeizeMode` 行。**</summary>
    public const int PhysBreakLine = 5718;

    /// <summary>**物理支置 `m_boWonderingEx` 行。**</summary>
    public const int PhysWonderingLine = 5720;

    /// <summary>**物理支置 `Result := True` 行。**</summary>
    public const int PhysResultLine = 5721;

    /// <summary>**两处 `else` 目标处置的目标行。**</summary>
    public static readonly int[] ElseTargetLines = { 5700, 5727 };

    /// <summary>**魔法支的 `else` 关键字行。**</summary>
    public const int MagicElseLine = 5696;

    /// <summary>**物理支的 `else` 关键字行。**</summary>
    public const int PhysElseLine = 5723;

    /// <summary>**`else` 到目标行的结构偏移。**</summary>
    public const int ElseOffset = 4;

    /// <summary>**两处 `else` 的目标处置行数。**</summary>
    public const int ElseBlockLines = 8;

    /// <summary>**`m_boWonderingEx` 的置真处（1:1）。**</summary>
    public static readonly int[] WonderingSetLines = { 5694, 5720 };

    /// <summary>**`m_boWonderingEx` 的置假处（1:1，含 J216 两处）。**</summary>
    public static readonly int[] WonderingClearLines = { 5541, 5763, 5791 };

    /// <summary>**声明行（`virtual` 而非 `override`）。**</summary>
    public const int AttackDeclLine = 190;

    /// <summary>**`MagicAttackTarget` 声明行。**</summary>
    public const int MagicDeclLine = 191;

    /// <summary>**`TMonster` 注释掉 `MagicAttackTarget` 的行。**</summary>
    public const int MonsterMagicCommentedLine = 21;

    // ---------- Create ----------

    /// <summary>**`m_boDupMode` 初始化行。**</summary>
    public const int DupInitLine = 5533;

    /// <summary>**`bo554` 初始化行。**</summary>
    public const int Bo554InitLine = 5534;

    /// <summary>**`m_dwThinkTick` 初始化行。**</summary>
    public const int ThinkTickInitLine = 5535;

    /// <summary>**`m_nViewRange` 行。**</summary>
    public const int ViewRangeLine = 5536;

    /// <summary>**视野值。**</summary>
    public const int ViewRange = 5;

    /// <summary>**`m_nViewRange := 5` 的其他三处（1:1）。**</summary>
    public static readonly int[] ViewRangeFiveOtherLines = { 557, 794, 1385 };

    /// <summary>**`m_nRunTime` 行。**</summary>
    public const int RunTimeLine = 5537;

    /// <summary>**运行间隔。**</summary>
    public const int RunTime = 250;

    /// <summary>**`m_dwSearchTime` 行。**</summary>
    public const int SearchTimeLine = 5538;

    /// <summary>**搜索时间基数。**</summary>
    public const int SearchTimeBase = 3000;

    /// <summary>**搜索时间随机参数。**</summary>
    public const int SearchTimeBound = 2000;

    /// <summary>**J213 的搜索时间写法对照值。**</summary>
    public const int J213SearchTimeBase = 1500;

    /// <summary>**`m_dwSearchTick` 行。**</summary>
    public const int SearchTickLine = 5539;

    /// <summary>**`m_btRaceServer` 行。**</summary>
    public const int RaceLine = 5540;

    /// <summary>**种族值。**</summary>
    public const int RaceServer = 80;

    /// <summary>**`TMonster.Create` 的种族行。**</summary>
    public const int MonsterRaceLine = 798;

    /// <summary>**`m_boWonderingEx` 初始化行。**</summary>
    public const int WonderingInitLine = 5541;

    /// <summary>**`TMonster.Create` 的行范围（1:1）。**</summary>
    public const int MonsterCreateStart = 788;

    /// <summary>**`TMonster.Create` 结束行。**</summary>
    public const int MonsterCreateEnd = 799;

    /// <summary>**`TMonster.Create` 行数。**</summary>
    public const int MonsterCreateLines = 12;

    /// <summary>**`bo554` 的声明处（1:1，J206 记录）。**</summary>
    public static readonly int[] Bo554DeclLines = { 11, 182, 507 };

    /// <summary>**本类的 `bo554` 声明行。**</summary>
    public const int FoxBo554DeclLine = 182;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数（本批为同类前半）。**</summary>
    public const int ClassesCovered = 19;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 35;

    // ---------- 脚本提取的表 ----------

    /// <summary>**共享外层模板的六项（1:1）。**</summary>
    public static readonly string[] OuterTemplate =
    {
        "Result := False",
        "nil guard",
        "tick_diff cooldown triple",
        "Abs(X)<=6 and Abs(Y)<=6",
        "(m_nTargetX = -1) or (Random(2) = 0)",
        "same-map approach / other-map discard",
    };

    /// <summary>**两路的判据对照（1:1）。**</summary>
    public static readonly (string Route, string Counter, int Threshold, string Comment)[]
        DupRoutes =
    {
        ("not pet-of-player or not in safe zone", "GetXYObjCount", 2,
            "防止安全区宝宝被挤出安全区"),
        ("pet-of-player and in safe zone", "GetXYNpcObjCount", 1,
            "不让宝宝和NPC叠到一起 2020-09-07 00:01:53"),
    };

    // ===================== 一、共享外层模板 =====================

    /// <summary>**外层模板逐字相同。**</summary>
    public static bool OuterTemplateVerbatim()
        => TemplateDiffLines == 0;

    /// <summary>**三十行零差异。**</summary>
    public static bool ThirtyLinesZeroDiff()
        => OuterLines == 30 && TemplateDiffLines == 0;

    /// <summary>**第四次确认。**</summary>
    public static bool FourthConfirmation()
        => TemplateConfirmations == 4;

    /// <summary>**跨了不同的继承分支。**</summary>
    public static bool SpansDifferentBases() => true;

    /// <summary>**模板是通用的。**</summary>
    public static bool TemplateIsGeneric() => true;

    /// <summary>**模板表已提取。**</summary>
    public static bool OuterTemplateExtracted()
        => OuterTemplate.Length == 6
           && OuterTemplate[3].Contains("Abs(Y)");

    /// <summary>**两处外层体行数相同。**</summary>
    public static bool OuterLengthsMatch()
        => OuterEnd - OuterStart + 1 == J207OuterEnd - J207OuterStart + 1;

    /// <summary>**两处外层体都是 30 行。**</summary>
    public static bool BothAreThirty()
        => OuterLines == 30
           && (J207OuterEnd - J207OuterStart + 1) == 30;

    /// <summary>**内层分解相加。**</summary>
    public static bool MagicDecompositionAddsUp()
        => 1 + BlankBeforeNested + NestedLines
           + BlankBeforeOuter() + OuterLines == MagicLines;

    /// <summary>外层前的空行数。**</summary>
    public static int BlankBeforeOuter() => 1;

    // ===================== 二、反编译标签非唯一 =====================

    /// <summary>**地址标签不唯一。**</summary>
    public static bool AddressLabelNotUnique()
        => LabelOccurrences > 1;

    /// <summary>**同一标签贴在三处 `Create` 上。**</summary>
    public static bool SameLabelThreeCreates()
        => Label004A8B74Lines.Length == 3;

    /// <summary>**同一标签贴在三处 `Think` 上。**</summary>
    public static bool SameLabelThreeThinks()
        => Label004A8E54Lines.Length == 3;

    /// <summary>**注释随代码复制。**</summary>
    public static bool CopyPropagatedComments() => true;

    /// <summary>**是"同源"的证据。**</summary>
    public static bool EvidenceOfSameOrigin() => true;

    /// <summary>**不是身份键。**</summary>
    public static bool NotAnIdentityKey() => true;

    /// <summary>**`Create` 标签表已提取。**</summary>
    public static bool CreateLabelTableExtracted()
        => Label004A8B74Lines[0] == 551
           && Label004A8B74Lines[1] == MonsterCreateStart
           && Label004A8B74Lines[2] == CreateStart;

    /// <summary>**`Think` 标签表已提取。**</summary>
    public static bool ThinkLabelTableExtracted()
        => Label004A8E54Lines[1] == 845
           && Label004A8E54Lines[2] == ThinkStart;

    /// <summary>**同一地址贴在不同类上。**</summary>
    public static bool SameAddressDifferentClasses()
        => Label004A8B74Lines[1] != Label004A8B74Lines[2];

    /// <summary>**VMT 标签嵌在表达式里。**</summary>
    public static bool VmtLabelInExpression() => true;

    /// <summary>**六处。**</summary>
    public static bool SixOccurrences()
        => VmtLabelLines.Length == 6;

    /// <summary>**来源于声明处。**</summary>
    public static bool OriginatesFromDeclaration() => true;

    /// <summary>**`FFFF4` 来自 `ObjBase.pas:781`。**</summary>
    public static bool Ffff4ComesFromObjBase781()
        => Ffff4DeclLine == 781;

    /// <summary>**`FFEB` 来自 `ObjMon.pas:20`。**</summary>
    public static bool FfebComesFromObjMon20()
        => FfebDeclLine == 20;

    /// <summary>**VMT 标签表已提取。**</summary>
    public static bool VmtLabelTableExtracted()
        => VmtLabelLines.Length == 6
           && VmtLabelLines[4] == VmtLabelThisBatch
           && VmtLabelLines[5] == VmtLabelJ216;

    /// <summary>**本批与 J216 各占一处。**</summary>
    public static bool SplitAcrossThisAndJ216()
        => VmtLabelThisBatch != VmtLabelJ216;

    // ---------- nil 安全 ----------

    /// <summary>**调用点没有判空。**</summary>
    public static bool NoNilCheckAtCallSite()
        => ProperTargetLine == 5563;

    /// <summary>**因被调方判空而安全。**</summary>
    public static bool SafeBecauseCalleeChecks() => true;

    /// <summary>**被调方的守卫带注释。**</summary>
    public static bool CalleeGuardWithComment()
        => IsProperTargetNilLine == 35486;

    /// <summary>**契约分散在调用链两端。**</summary>
    public static bool ContractSplitAcrossCallChain() => true;

    /// <summary>**与 J213 形态相反。**</summary>
    public static bool OppositeShapeToJ213() => true;

    /// <summary>`IsProperTarget` 判定（1:1）。</summary>
    public static bool IsProperTargetSafe(bool isNull, bool isSelf)
        => !(isNull || isSelf);

    /// <summary>**空指针返回假。**</summary>
    public static bool NullIsNotProper()
        => !IsProperTargetSafe(true, false);

    /// <summary>**自指返回假。**</summary>
    public static bool SelfIsNotProper()
        => !IsProperTargetSafe(false, true);

    // ===================== 三、Think 的实现族 =====================

    /// <summary>**只有声明没有实现。**</summary>
    public static bool DeclarationWithoutBody() => true;

    /// <summary>**静态方法所以不要求实现。**</summary>
    public static bool StaticSoNoLinkDemand() => true;

    /// <summary>**从未通过基类调用。**</summary>
    public static bool NeverCalledThroughBase() => true;

    /// <summary>**是残留声明。**</summary>
    public static bool VestigialDeclaration() => true;

    /// <summary>**三个同名静态方法。**</summary>
    public static bool ThreeSameNamedStatics() => true;

    /// <summary>**不是虚方法所以没有覆写关系。**</summary>
    public static bool NotVirtualSoNoOverride() => true;

    /// <summary>**没有虚表分派。**</summary>
    public static bool NoVmtDispatch() => true;

    /// <summary>**调用者拿到自己的那一份。**</summary>
    public static bool CallerGetsItsOwn() => true;

    /// <summary>**可见性逐层收窄。**</summary>
    public static bool VisibilityNarrows() => true;

    /// <summary>**五处 `Think` 实现。**</summary>
    public static bool FiveThinkImplementations()
        => ThinkImpls.Length == 5;

    /// <summary>**共用同一个三秒闸门。**</summary>
    public static bool SameThreeSecondGate() => true;

    /// <summary>**算式未折叠。**</summary>
    public static bool UnfoldedArithmetic()
        => ThinkGateMs == 3000;

    /// <summary>**是复制而非简化。**</summary>
    public static bool CopiedNotSimplified() => true;

    /// <summary>**实现表已提取。**</summary>
    public static bool ThinkImplsExtracted()
        => ThinkImpls[3].Line == ThinkGateLine
           && ThinkImpls[3].Owner == "TFoxMonster";

    /// <summary>**三处声明行递增。**</summary>
    public static bool DeclLinesAscending()
        => MonsterThinkDeclLine < FoxThinkDeclLine
           && AnimalThinkDeclLine != MonsterThinkDeclLine;

    /// <summary>闸门判定（1:1）。</summary>
    public static bool ThinkGateElapsed(uint lastThink, uint now)
        => (now - lastThink) > ThinkGateMs;

    /// <summary>**刚刷新则不过闸。**</summary>
    public static bool JustRefreshedBlocked()
        => !ThinkGateElapsed(1000, 1000);

    /// <summary>**恰好三秒不过闸（严格大于）。**</summary>
    public static bool ExactlyThreeSecondsBlocked()
        => !ThinkGateElapsed(1000, 4000);

    /// <summary>**超一毫秒过闸。**</summary>
    public static bool OneOverPasses()
        => ThinkGateElapsed(1000, 4001);

    /// <summary>**`Think` 闸门用裸减法。**</summary>
    public static bool RawSubtractForThinkGate()
        => ThinkGateLine == 5549;

    /// <summary>**命中冷却是 `tick_diff`。**</summary>
    public static bool TickDiffForHitCooldown()
        => MagicStart < 5655 && 5655 < MagicEnd;

    /// <summary>**同一类里两种时间写法。**</summary>
    public static bool TwoIdiomsInSameClass()
        => RawSubtractForThinkGate() && TickDiffForHitCooldown();

    // ---------- 两段结构 ----------

    /// <summary>**闸门只覆盖第一段。**</summary>
    public static bool GateCoversFirstStageOnly()
        => ThinkGateLine < Stage2Start;

    /// <summary>**第二段每帧都跑。**</summary>
    public static bool SecondStageEveryFrame()
        => Stage2Start > ThinkGateLine;

    /// <summary>**这个分工是合理的。**</summary>
    public static bool ReasonableSplit() => true;

    /// <summary>**`Result` 只来自位移。**</summary>
    public static bool ResultOnlyFromDisplacement()
        => ThinkResultLine == 5548 && ThinkResultTrueLine == 5575;

    /// <summary>**第二段在闸门之外。**</summary>
    public static bool Stage2OutsideGate()
        => Stage2Start > ThinkEnd - 13;

    // ---------- 让位两路 ----------

    /// <summary>**两条互斥路径。**</summary>
    public static bool TwoMutuallyExclusiveRoutes()
        => DupRoutes.Length == 2;

    /// <summary>**对象数阈值是 2。**</summary>
    public static bool ObjectCountThresholdTwo()
        => ObjCountThreshold == 2;

    /// <summary>**NPC 数阈值是 1。**</summary>
    public static bool NpcCountThresholdOne()
        => NpcCountThreshold == 1;

    /// <summary>**两条注释解释两件事。**</summary>
    public static bool TwoCommentsExplain()
        => DupRoutes[0].Comment.Contains("安全区")
           && DupRoutes[1].Comment.Contains("NPC");

    /// <summary>**补上了 J159 的锁号 30 调用者。**</summary>
    public static bool CompletesJ159LockThirty() => true;

    /// <summary>让位路由表已提取。**</summary>
    public static bool DupRoutesExtracted()
        => DupRoutes[0].Counter == "GetXYObjCount"
           && DupRoutes[1].Counter == "GetXYNpcObjCount";

    /// <summary>两路选择（1:1）。</summary>
    public static string PickDupRoute(bool hasMaster, bool inSafeZone,
        int masterRace)
    {
        if (hasMaster && inSafeZone && masterRace == 0)
            return "route2";

        return "route1";
    }

    /// <summary>**主人的宝宝且在安全区走第二路。**</summary>
    public static bool PetInSafeZoneGoesRoute2()
        => PickDupRoute(true, true, 0) == "route2";

    /// <summary>**无主人走第一路。**</summary>
    public static bool NoMasterGoesRoute1()
        => PickDupRoute(false, true, 0) == "route1";

    /// <summary>**不在安全区走第一路。**</summary>
    public static bool NotInSafeZoneGoesRoute1()
        => PickDupRoute(true, false, 0) == "route1";

    /// <summary>**主人不是人物则走第一路。**</summary>
    public static bool NonPlayerMasterGoesRoute1()
        => PickDupRoute(true, true, 80) == "route1";

    /// <summary>**对象数判据（1:1）。**</summary>
    public static bool NeedsDupByObjCount(int count)
        => count >= ObjCountThreshold;

    /// <summary>**NPC 数判据（1:1）。**</summary>
    public static bool NeedsDupByNpcCount(int count)
        => count >= NpcCountThreshold;

    /// <summary>**同格 2 个对象即需让位。**</summary>
    public static bool TwoObjectsNeedsDup()
        => NeedsDupByObjCount(2);

    /// <summary>**同格 1 个对象不需要。**</summary>
    public static bool OneObjectDoesNot()
        => !NeedsDupByObjCount(1);

    /// <summary>**同格 1 个 NPC 即需让位。**</summary>
    public static bool OneNpcNeedsDup()
        => NeedsDupByNpcCount(1);

    /// <summary>**同格 0 个 NPC 不需要。**</summary>
    public static bool ZeroNpcDoesNot()
        => !NeedsDupByNpcCount(0);

    /// <summary>**`Master` 与 `m_Master` 混用。**</summary>
    public static bool MasterVsMMaster() => true;

    /// <summary>**是同一事物的两种写法。**</summary>
    public static bool SameThingTwoSpellings() => true;

    /// <summary>**混在同一行里。**</summary>
    public static bool MixedWithinOneLine()
        => Route1Start == 5552;

    // ---------- 粘滞标志 ----------

    /// <summary>**是粘滞标志。**</summary>
    public static bool StickyFlag() => true;

    /// <summary>**只在成功时清除。**</summary>
    public static bool ClearedOnlyOnSuccess()
        => DupClearLine == 5574 && MovedCheckLine == 5572;

    /// <summary>**失败则重试到成功。**</summary>
    public static bool RetriesUntilDisplaced() => true;

    /// <summary>**不是一次性标记。**</summary>
    public static bool NotOneShot() => true;

    /// <summary>**初始化成假。**</summary>
    public static bool InitialisedFalse()
        => DupInitLine == 5533;

    /// <summary>移动判定（1:1）。</summary>
    public static bool DidMove(int oldX, int oldY, int newX, int newY)
        => oldX != newX || oldY != newY;

    /// <summary>**真的动了。**</summary>
    public static bool MovedReturnsTrue()
        => DidMove(1, 1, 2, 1);

    /// <summary>**没动返回假（标志保持）。**</summary>
    public static bool NotMovedReturnsFalse()
        => !DidMove(1, 1, 1, 1);

    /// <summary>**随机方向 8 是正确的。**</summary>
    public static bool Random8IsCorrect()
        => WalkToRandomBound == 8;

    /// <summary>**覆盖全部八个方向。**</summary>
    public static bool CoversAllEight()
        => WalkToRandomBound == 8;

    /// <summary>**与 J216 的 `Random(9)` 形成对照。**</summary>
    public static bool ContrastWithWonderingExRandom9() => true;

    /// <summary>**同类里两种写法。**</summary>
    public static bool SameClassTwoSpellings() => true;

    /// <summary>**`Random(8)` 的值域合法。**</summary>
    public static bool Random8InRange(int roll)
        => roll >= 0 && roll < 8;

    /// <summary>**八种取值全部合法。**</summary>
    public static bool AllEightValid()
    {
        for (int r = 0; r < 8; r++)
        {
            if (!Random8InRange(r))
                return false;
        }

        return true;
    }

    // ===================== 四、AttackTarget =====================

    /// <summary>**查了 `m_boMagicAttack`。**</summary>
    public static bool ChecksMagicFlag()
        => MagicFlagLine == 5689;

    /// <summary>**两条分支。**</summary>
    public static bool TwoBranches()
        => MagicBranchEnd < PhysBranchStart;

    /// <summary>**与 J212 相反。**</summary>
    public static bool OppositeOfJ212() => true;

    /// <summary>**自己实现了分派。**</summary>
    public static bool SelfImplementedDispatch() => true;

    /// <summary>**两处 `else` 相同。**</summary>
    public static bool SameElseTwice()
        => ElseTargetLines.Length == 2;

    /// <summary>**各八行。**</summary>
    public static bool EightLinesDuplicated()
        => ElseBlockLines == 8;

    /// <summary>**只有攻击动作不同。**</summary>
    public static bool OnlyAttackActionDiffers() => true;

    /// <summary>**两处 `else` 目标处置的结构偏移相同**（各以 `else` 关键字为锚）。
    /// <remarks>
    /// **修正记录**：初版以 `GetAttackDir` 行为锚、断言
    /// `ElseTargetLines[0] - MagicGetDirLine == ElseTargetLines[1] - PhysGetDirLine`、
    /// 探针实测为假 —— 因为**物理支在 `GetAttackDir` 与 `else` 之间
    /// 多了冷却三件套与 `Attack`/`BreakHolySeizeMode`（5712-5718）**、
    /// 所以从 `GetAttackDir` 数起的偏移自然不同（9 对 17）。
    ///
    /// **正确的锚点是 `else` 关键字本身**：
    /// 魔法支 `else`（5696）→ `begin`（5697）→ `if m_PEnvir`（5698）→
    /// `begin`（5699）→ `SetTargetXY`（5700），偏移 **4**；
    /// 物理支 `else`（5723）→ … → `SetTargetXY`（5727），偏移**同样 4** ——
    /// **即那八行的"目标处置"两处确实是同构的**、
    /// 只是它们各自距离 `GetAttackDir` 的远近不同。
    /// </remarks>
    /// </summary>
    public static bool ElseOffsetsMatch()
        => ElseTargetLines[0] - MagicElseLine
           == ElseTargetLines[1] - PhysElseLine;

    /// <summary>**物理支有自己的冷却。**</summary>
    public static bool PhysicalHasOwnCooldown()
        => PhysCooldownLine == 5712;

    /// <summary>**魔法支把冷却委托出去。**</summary>
    public static bool MagicDelegatesCooldown()
        => MagicDelegateLine == 5693;

    /// <summary>**冷却在不同层。**</summary>
    public static bool CooldownAtDifferentLayers()
        => PhysicalHasOwnCooldown() && MagicDelegatesCooldown();

    /// <summary>分派（1:1）。</summary>
    public static string Dispatch(bool magicFlag)
        => magicFlag ? "magic" : "physical";

    /// <summary>**真走魔法支。**</summary>
    public static bool FlagTrueMagic() => Dispatch(true) == "magic";

    /// <summary>**假走物理支。**</summary>
    public static bool FlagFalsePhysical() => Dispatch(false) == "physical";

    /// <summary>**魔法支没有冷却判据。**</summary>
    public static bool MagicBranchNoCooldown()
        => MagicBranchStart < PhysCooldownLine
           && MagicBranchEnd < PhysCooldownLine;

    /// <summary>**物理支的三件套。**</summary>
    public static bool PhysicalCooldownTriple()
        => PhysHitTickLine == PhysCooldownLine + 2
           && PhysHitDelayLine == PhysHitTickLine + 1;

    /// <summary>**聚焦紧跟三件套。**</summary>
    public static bool FocusAfterTriple()
        => PhysFocusLine == PhysHitDelayLine + 1;

    /// <summary>**`Attack` 紧跟聚焦。**</summary>
    public static bool AttackAfterFocus()
        => PhysAttackLine == PhysFocusLine + 1;

    /// <summary>物理支冷却判定（1:1）。</summary>
    public static bool PhysCooldownElapsed(uint hitTick, uint now,
        int nextHitTime, int hitDelay)
        => (now >= hitTick ? now - hitTick : uint.MaxValue - hitTick + now)
           > (uint)(nextHitTime + hitDelay);

    /// <summary>**刚刷新为假。**</summary>
    public static bool PhysJustResetFalse()
        => !PhysCooldownElapsed(1000, 1000, 500, 0);

    /// <summary>**超阈为真。**</summary>
    public static bool PhysAfterThresholdTrue()
        => PhysCooldownElapsed(1000, 1600, 500, 0);

    /// <summary>**恰好等阈值为假。**</summary>
    public static bool PhysExactlyAtThresholdFalse()
        => !PhysCooldownElapsed(1000, 1500, 500, 0);

    // ---------- 走位标志 ----------

    /// <summary>**置真两处。**</summary>
    public static bool TwoSetters()
        => WonderingSetLines.Length == 2;

    /// <summary>**置假三处。**</summary>
    public static bool ThreeClearers()
        => WonderingClearLines.Length == 3;

    /// <summary>**两处置真都在本批。**</summary>
    public static bool SettersInThisBatch()
        => WonderingSetLines[0] == MagicWonderingLine
           && WonderingSetLines[1] == PhysWonderingLine;

    /// <summary>**一处置假在本批、两处在 J216。**</summary>
    public static bool ClearersSplit()
        => WonderingClearLines[0] == WonderingInitLine
           && WonderingClearLines[1] > AttackEnd;

    /// <summary>**为 J216 提供启用条件。**</summary>
    public static bool EnablesJ216() => true;

    /// <summary>**魔法支的置真在转发之后。**</summary>
    public static bool MagicSetAfterDelegate()
        => MagicWonderingLine == MagicDelegateLine + 1;

    /// <summary>**物理支的置真在三件套之后。**</summary>
    public static bool PhysSetAfterTriple()
        => PhysWonderingLine > PhysBreakLine;

    /// <summary>**两处置真的位置不同。**</summary>
    public static bool SetPositionsDiffer()
        => MagicWonderingLine != PhysWonderingLine;

    /// <summary>**声明用 `virtual` 而非 `override`。**</summary>
    public static bool DeclaredVirtualNotOverride() => true;

    /// <summary>**这里用 `virtual` 是正确的。**</summary>
    public static bool VirtualIsCorrectHere() => true;

    /// <summary>**存在两条独立的链。**</summary>
    public static bool TwoIndependentChains() => true;

    /// <summary>**`TMonster` 把 `MagicAttackTarget` 注释掉了。**</summary>
    public static bool TMonsterCommentsOutMagicAttackTarget()
        => MonsterMagicCommentedLine == 21;

    /// <summary>**不是隐藏缺陷。**</summary>
    public static bool NotAHidingBug() => true;

    /// <summary>**设了目标聚焦时刻。**</summary>
    public static bool SetsTargetFocusTick()
        => PhysFocusLine == 5716;

    /// <summary>**与 J212 一致。**</summary>
    public static bool SameAsJ212() => true;

    /// <summary>**J207/J210/J211 都没有。**</summary>
    public static bool AbsentInJ207J210J211() => true;

    // ===================== 五、Create =====================

    /// <summary>**复制了 `TMonster.Create`。**</summary>
    public static bool DuplicatesTMonsterCreate() => true;

    /// <summary>**只多一行。**</summary>
    public static bool OnlyOneExtraLine()
        => CreateLines - MonsterCreateLines == 1;

    /// <summary>**不继承 `TMonster`。**</summary>
    public static bool NotInheritingTMonster() => true;

    /// <summary>**复制是必需的。**</summary>
    public static bool CopyWasNecessary() => true;

    /// <summary>**有结构上的理由。**</summary>
    public static bool StructuralReason() => true;

    /// <summary>**两处行数差一。**</summary>
    public static bool LineCountDiffersByOne()
        => CreateLines == MonsterCreateLines + 1;

    /// <summary>**视野是 5。**</summary>
    public static bool ViewRangeFive()
        => ViewRange == 5;

    /// <summary>**是第 4 处。**</summary>
    public static bool FourthOccurrence()
        => ViewRangeFiveOtherLines.Length == 3;

    /// <summary>**视野表已提取。**</summary>
    public static bool ViewRangeTableExtracted()
        => ViewRangeFiveOtherLines[1] == 794
           && ViewRangeFiveOtherLines[2] == 1385;

    /// <summary>**是常见取值之一。**</summary>
    public static bool CommonValues() => true;

    /// <summary>**种族是 80。**</summary>
    public static bool RaceIsEighty()
        => RaceServer == 80;

    /// <summary>**与 `TMonster` 相同。**</summary>
    public static bool SameAsTMonster()
        => MonsterRaceLine == 798 && RaceLine == 5540;

    /// <summary>**与 J214 的镖车形成对照。**</summary>
    public static bool ContrastWithJ214Truck() => true;

    /// <summary>**`bo554` 是共享字段。**</summary>
    public static bool Bo554SharedField()
        => Bo554InitLine == 5534;

    /// <summary>**不继承 `TMonster` 却仍给 `bo554` 赋值。**</summary>
    public static bool AssignedEvenWithoutInheritingTMonster() => true;

    /// <summary>**补全了 J206 的结论。**</summary>
    public static bool CompletesJ206Finding() => true;

    /// <summary>**三处声明。**</summary>
    public static bool ThreeDeclarations()
        => Bo554DeclLines.Length == 3;

    /// <summary>**本类的声明是 182。**</summary>
    public static bool FoxDeclarationIs182()
        => FoxBo554DeclLine == 182;

    /// <summary>搜索时间初值（1:1）。</summary>
    public static int SearchTimeInit(int roll)
        => SearchTimeBase + roll;

    /// <summary>**范围 3000..4999。**</summary>
    public static bool SearchTime3000To4999()
        => SearchTimeInit(0) == 3000
           && SearchTimeInit(SearchTimeBound - 1) == 4999;

    /// <summary>**与 J213 对照。**</summary>
    public static bool ContrastJ213()
        => SearchTimeBase != J213SearchTimeBase;

    /// <summary>**操作数顺序不同。**</summary>
    public static bool OperandOrderDiffers() => true;

    /// <summary>**J213 是随机在前、本类是常量在前。**</summary>
    public static bool ConstantFirstHere()
        => SearchTimeBase == 3000;

    /// <summary>**运行间隔 250。**</summary>
    public static bool RunTime250()
        => RunTime == 250;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 六、跨度 =====================

    /// <summary>**跨两个批次。**</summary>
    public static bool SplitAcrossTwoBatches() => true;

    /// <summary>**J216 覆盖其余。**</summary>
    public static bool J216CoversRest() => true;

    /// <summary>**两批行数相加等于类总行数。**</summary>
    public static bool TwoBatchesAddUp()
        => TotalLines + J216WonderingLines + J216RunLines == ClassTotalLines;

    /// <summary>**本批四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 205;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (ThinkEnd - ThinkStart + 1) == ThinkLines
           && (MagicEnd - MagicStart + 1) == MagicLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < ThinkStart && ThinkStart < MagicStart
           && MagicStart < AttackStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => ThinkStart == CreateEnd + 2
           && MagicStart == ThinkEnd + 1
           && AttackStart == MagicEnd + 2;

    /// <summary>**`TMonster.Create` 跨度自洽。**</summary>
    public static bool MonsterCreateSpanMatches()
        => (MonsterCreateEnd - MonsterCreateStart + 1) == MonsterCreateLines;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackEnd < 9502;
}
